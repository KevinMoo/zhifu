using Hzsp.Helper;
using LitJson;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZhifuWeb.Helper;
using ZhifuWeb.lib.WeChat;
using ZhifuWeb.Models;

namespace ZhifuWeb.Controllers
{
    public class WxPayController : BasicController
    {
        private object _obj = new object();
        public ActionResult WxPayIndex()
        {
            var shopcarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopcarViewModel == null)
                return RedirectToAction("CheckInfoTicket", "Booking");

            _totalFee = Convert.ToInt32(shopcarViewModel.UserInfo.SumPrice * 100).ToString();
            LogHelper.CreateLog("进入WxPayIndex" + Request.QueryString["code"], "wxdebug");
            if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                Session["Car"] = null;
                var orderId = shopcarViewModel.OrderId;
                string code = Request.QueryString["code"];
                LogHelper.CreateLog(string.Format("WxPayController.WxPayIndex  code:{0}", code), "wxdebug");
                GetOpenidAndAccessTokenFromCode(code);
                var wxjsParam = PrePay(orderId);
                try
                {
                    PrePayModel prePayModel = JsonConvert.DeserializeObject<PrePayModel>(wxjsParam);
                    if (prePayModel != null)
                    {
                        //                        var wxJsApiParam = string.Format(@"timestamp:{0},nonceStr:'{1}',package:'{2}'
                        //                                    ,signType:'{3}',paySign:'{4}'", prePayModel.TimeStamp, prePayModel.NonceStr,
                        //                            prePayModel.PackAge,
                        //                            prePayModel.SignType, prePayModel.PaySign);
                        //                        LogHelper.CreateLog(wxJsApiParam, "wxdebug");

                        ViewData["timestamp2"] = prePayModel.TimeStamp;
                        ViewData["nonceStr2"] = prePayModel.NonceStr;
                        ViewData["package2"] = prePayModel.PackAge;
                        ViewData["signType2"] = prePayModel.SignType;
                        ViewData["paySign2"] = prePayModel.PaySign;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.CreateLog(ex.StackTrace, "wxdebug");
                }

                var wxConfigResult = Jssdk.GetWxConfigResult(WxPayConfig.APPID, WxPayConfig.APPSECRET, Request.Url.AbsoluteUri);
                ViewData["appid"] = wxConfigResult.appid;
                ViewData["timestamp"] = wxConfigResult.timestamp;
                ViewData["noncestr"] = wxConfigResult.noncestr;
                ViewData["signature"] = wxConfigResult.signature;
            }
            else
            {
                //构造网页授权获取code的URL
                string host = Request.Url.Host;
                string path = Request.Path;
                string redirect_uri = HttpUtility.UrlEncode("http://" + host + path);
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_base");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                data.SetValue("showwxpaytitle", "1");
                string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
                LogHelper.CreateLog(string.Format("WxPayController.WxPayIndex      Will Redirect to URL :{0} ", url), "wxdebug");
                try
                {
                    //触发微信返回code码 
                    Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
                }
                catch (System.Threading.ThreadAbortException)
                {
                }
            }
            return View();
        }

        public ActionResult WxPayResult(string status)
        {
            LogHelper.CreateLog("进入WxPayResult", "wxdebug");
            var payResult = new PayResultViewModel() { Status = false };
            if (status != "error")
            {
                payResult.Status = true;
            }
            return View(payResult);
        }

        public void WxNotify()
        {
            lock (_obj)
            {
                LogHelper.CreateLog("进入回调方法\r\n\r\n", "wxdebug");
                Notify notify = new Notify(this);
                WxPayData notifyData = notify.GetNotifyData();
                LogHelper.CreateLog(string.Format("notifyData:{0}", notifyData.ToJson()), "wxdebug");

                //检查支付结果中transaction_id是否存在
                if (!notifyData.IsSet("transaction_id"))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "支付结果中微信订单号不存在");
                    LogHelper.CreateLog(res.ToXml(), "wxdebug");
                }

                string transactionId = notifyData.GetValue("transaction_id").ToString();
                var orderId = notifyData.GetValue("out_trade_no").ToString();

                //由于微信会多次发出返回提示,所以需要判断下数据是否已经处理
                if (!UpdatePayStatus.CheckIsUpdate(orderId))
                {
                    //查询订单，判断订单真实性
                    if (!notify.QueryOrder(transactionId))
                    {
                        //若订单查询失败，则立即返回结果给微信支付后台
                        WxPayData res = new WxPayData();
                        res.SetValue("return_code", "FAIL");
                        res.SetValue("return_msg", "订单查询失败");
                        LogHelper.CreateLog(transactionId + Environment.NewLine + res.ToXml(), "wxdebug");
                    }
                    //查询订单成功
                    else
                    {
                        notifyData.SetValue("is_subscribe", "Y");
                        UpdatePayStatus.WxUpdate(orderId, _totalFee);
                        WxPayData res = new WxPayData();
                        res.SetValue("return_code", "SUCCESS");
                        res.SetValue("return_msg", "OK");
                        LogHelper.CreateLog(
                            "微信支付订单Success" + Environment.NewLine + transactionId + Environment.NewLine + res.ToXml(),
                            "wxdebug");

                    }
                }
            }
        }

        #region 对支付封装调用
        static string _openId, _accessToken, _totalFee;

        private string PrePay(string orderId)
        {
            //JSAPI支付预处理
            try
            {
                WxPayData unifiedOrderResult = GetUnifiedOrderResult(orderId);
                string wxJsApiParam = GetJsApiParameters();//获取H5调起JS API参数    
                LogHelper.CreateLog(string.Format("WxPayController.PrePay    wxJsApiParam:{0}", wxJsApiParam), "wxdebug");
                //在页面上显示订单信息
                LogHelper.CreateLog("WxPayController.PrePay      订单详情：" + Environment.NewLine + unifiedOrderResult.ToPrintStr(), "wxdebug");
                return wxJsApiParam;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(string.Format("WxPayController.PrePay       {0}{1}{2}", ex.StackTrace, Environment.NewLine, ex.Message), "~/logs/微信支付下单失败");
            }
            return string.Empty;
        }
        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        WxPayData _unifiedOrderResult;
        private void GetOpenidAndAccessTokenFromCode(string code)
        {
            try
            {
                //构造获取openid及access_token的url
                WxPayData data = new WxPayData();
                data.SetValue("appid", WxPayConfig.APPID);
                data.SetValue("secret", WxPayConfig.APPSECRET);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();

                //请求url以获取数据
                string result = HttpService.Get(url);

                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                _accessToken = (string)jd["access_token"];

                //获取用户openid
                _openId = (string)jd["openid"];

                LogHelper.CreateLog(string.Format("WxPayController.GetOpenidAndAccessTokenFromCode     openid:{0}\r\naccess_token:{1}", _openId, _accessToken), "wxdebug");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "~/logs/GeneralLog");
                throw new WxPayException(ex.ToString());
            }
        }

        /**
        * 调用统一下单，获得下单结果
        * @return 统一下单结果
        * @失败时抛异常WxPayException
        */
        private WxPayData GetUnifiedOrderResult(string orderId)
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", "test");
            data.SetValue("attach", orderId);
            data.SetValue("out_trade_no", orderId);
            data.SetValue("total_fee", _totalFee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", "test");
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", _openId);
            LogHelper.CreateLog(string.Format("WxPayController.GetUnifiedOrderResult     下单数据:{0}", data.ToJson()), "wxdebug");

            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                //Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            _unifiedOrderResult = result;
            return result;
        }

        /**
        *  
        * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        * 微信浏览器调起JSAPI时的输入参数格式如下：
        * {
        *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        *   "signType" : "MD5",         //微信签名方式:    
        *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        * }
        * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        * 
        */
        private string GetJsApiParameters()
        {
            //Log.Debug(this.GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", _unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + _unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            //Log.Debug(this.GetType().ToString(), "Get jsApiParam : " + parameters);
            return parameters;
        }


        /**
        * 
        * 获取收货地址js函数入口参数,详情请参考收货地址共享接口：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_9
        * @return string 共享收货地址js函数需要的参数，json格式可以直接做参数使用
        */
        private string GetEditAddressParameters()
        {
            string parameter = "";
            try
            {
                string host = Request.Url.Host;
                string path = Request.Path;
                string queryString = Request.Url.Query;
                //这个地方要注意，参与签名的是网页授权获取用户信息时微信后台回传的完整url
                string url = "http://" + host + path + queryString;

                //构造需要用SHA1算法加密的数据
                WxPayData signData = new WxPayData();
                signData.SetValue("appid", WxPayConfig.APPID);
                signData.SetValue("url", url);
                signData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
                signData.SetValue("noncestr", WxPayApi.GenerateNonceStr());
                signData.SetValue("accesstoken", _accessToken);
                string param = signData.ToUrl();

                //Log.Debug(this.GetType().ToString(), "SHA1 encrypt param : " + param);
                //SHA1加密
                string addrSign = FormsAuthentication.HashPasswordForStoringInConfigFile(param, "SHA1");
                //Log.Debug(this.GetType().ToString(), "SHA1 encrypt result : " + addrSign);

                //获取收货地址js函数入口参数
                WxPayData afterData = new WxPayData();
                afterData.SetValue("appId", WxPayConfig.APPID);
                afterData.SetValue("scope", "jsapi_address");
                afterData.SetValue("signType", "sha1");
                afterData.SetValue("addrSign", addrSign);
                afterData.SetValue("timeStamp", signData.GetValue("timestamp"));
                afterData.SetValue("nonceStr", signData.GetValue("noncestr"));

                //转为json格式
                parameter = afterData.ToJson();
                //Log.Debug(this.GetType().ToString(), "Get EditAddressParam : " + parameter);
            }
            catch (Exception ex)
            {
                //Log.Error(this.GetType().ToString(), ex.ToString());
                throw new WxPayException(ex.ToString());
            }

            return parameter;
        }
        #endregion
    }
}
