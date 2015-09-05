using Hzsp.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ZhifuWeb.Helper;
using ZhifuWeb.lib.Alipay;
using ZhifuWeb.Models;

namespace ZhifuWeb.Controllers
{
    public class AliPayController : BasicController
    {
        #region 发起一个支付宝的请求

        public ActionResult AlipayIndex()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel == null)
                return RedirectToAction("chooseTicket", "Booking");
            Session["Car"] = null;
            var tradeno = shopCarViewModel.OrderId;
            var totalfee = Convert.ToDouble(shopCarViewModel.UserInfo.SumPrice).ToString();
            var notifyurl = "http://" + Request.Url.Host + "/AliPay/Notify";
            var callbackurl = "http://" + Request.Url.Host + "/AliPay/Success";
            var merchanturl = "http://" + Request.Url.Host + "/booking/CheckInfoTicket";
            var cussubject = "门票";
            AlipayRequest(tradeno, totalfee, notifyurl, callbackurl, merchanturl, cussubject);

            return View();
        }

        #endregion 发起一个支付宝的请求

        #region 封装一个Alipay请求

        /// <summary>
        ///     发起一个支付宝的请求(包括授权接口和交易接口)
        /// </summary>
        /// <param name="tradeno">订单号</param>
        /// <param name="totalfee">付款金额</param>
        /// <param name="merchanturl"></param>
        /// <param name="notifyurl">异步回调地址</param>
        /// <param name="callbackurl">同步回调地址</param>
        /// <param name="cussubject">订单名称</param>
        public void AlipayRequest(string tradeno, string totalfee,
            string notifyurl, string callbackurl, string merchanturl,
            string cussubject)
        {
            //支付宝网关地址
            var GATEWAY_NEW = "http://wappaygw.alipay.com/service/rest.htm?";

            ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////

            var format = "xml";
            var v = "2.0";
            //请求号
            var req_id = DateTime.Now.ToString("yyyyMMddHHmmss");

            //req_data详细信息
            var notify_url = notifyurl;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数
            var call_back_url = callbackurl;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数
            var merchant_url = merchanturl;
            //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数
            //商户订单号
            var out_trade_no = tradeno;
            var subject = cussubject;
            //付款金额
            var total_fee = totalfee;
            //请求业务参数详细
            var req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" +
                                call_back_url + "</call_back_url><seller_account_name>" + Config.Seller_email +
                                "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" +
                                subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" +
                                merchant_url + "</merchant_url></direct_trade_create_req>";
            //把请求参数打包成数组
            var sParaTempToken = new Dictionary<string, string>();
            sParaTempToken.Add("partner", Config.Partner);
            sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
            sParaTempToken.Add("format", format);
            sParaTempToken.Add("v", v);
            sParaTempToken.Add("req_id", req_id);
            sParaTempToken.Add("req_data", req_dataToken);

            //建立请求
            var sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
            //URLDECODE返回的信息
            var code = Encoding.GetEncoding(Config.Input_charset);
            sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

            //解析远程模拟提交后返回的信息
            var dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

            //获取token
            if (!dicHtmlTextToken.ContainsKey("request_token"))
            {
                Response.Write("获取request_token失败" + JsonConvert.SerializeObject(dicHtmlTextToken));
                return;
            }
            var request_token = dicHtmlTextToken["request_token"];

            LogHelper.CreateLog(string.Format("获取request_token:{0}", request_token), tradeno);
            #region 根据授权码token调用交易接口alipay.wap.auth.authAndExecute

            //业务详细
            var req_data = "<auth_and_execute_req><request_token>" + request_token +
                           "</request_token></auth_and_execute_req>";
            //必填

            //把请求参数打包成数组
            var sParaTemp = new Dictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
            sParaTemp.Add("format", format);
            sParaTemp.Add("v", v);
            sParaTemp.Add("req_data", req_data);

            //建立请求
            var sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");
            LogHelper.CreateLog(string.Format("成功获取支付页面"), tradeno);
            Response.Write(sHtmlText);

            #endregion 根据授权码token调用交易接口alipay.wap.auth.authAndExecute
        }

        #endregion 封装一个Alipay请求

        #region 支付宝回调地址

        #region Call_back

        public ActionResult Success()
        {
            var payResult = new PayResultViewModel();
            var sPara = GetRequestGet();

            if (sPara.Count > 0) //判断是否有带返回参数
            {
                var aliNotify = new Notify();
                var verifyResult = aliNotify.VerifyReturn(sPara, Request.QueryString["sign"]);

                if (verifyResult) //验证成功
                {
                    //商户订单号
                    var out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号
                    var trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    var result = Request.QueryString["result"];
                    payResult.Status = true;
                }
                else //验证失败
                {
                    payResult.Status = false;
                }
            }
            else
            {
                payResult.Status = false;
            }
            return View(payResult);
        }

        /// <summary>
        ///     获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestGet()
        {
            var i = 0;
            var sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            var requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        #endregion Call_back

        #region Notify

        [ValidateInput(false)]
        /// <summary>
        /// 该方式的朱勇主要防止订单丢失,即页面条状同步通知没有处理订单更新,它则去处理
        /// </summary>
        /// <returns></returns>
        public void Notify()
        {
            var sPara = GetRequestPost();

            if (sPara.Count > 0) //判断是否有带返回参数
            {
                var aliNotify = new Notify();
                var verifyResult = aliNotify.VerifyNotify(sPara, Request.Form["sign"]);

                if (verifyResult) //验证成功
                {
                    //XML解析notify_data数据
                    try
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sPara["notify_data"]);

                        //商户订单号
                        var out_trade_no = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;
                        //支付宝交易号
                        var trade_no = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;
                        //交易状态
                        var trade_status = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;

                        var orderId = out_trade_no;
                        //这里需要注意
                        var totalFee = xmlDoc.SelectSingleNode("/notify/total_fee").InnerText;
                        var setting = new JsonSerializerSettings()
                        {
                            Formatting = Newtonsoft.Json.Formatting.Indented
                        };
                        LogHelper.CreateLog(JsonConvert.SerializeObject(sPara["notify_data"], setting), orderId);
                        LogHelper.CreateLog(JsonConvert.SerializeObject(Request.Form), orderId);
                        if (trade_status == "TRADE_FINISHED")
                        {
                            #region
                            if (!UpdatePayStatus.CheckIsUpdate(orderId))
                                UpdatePayStatus.AlipayUpdate(orderId, totalFee);

                            #endregion Notify

                            //程序执行完成后必须打印输出"success"这7个字符,否则支付包会不断重发通知
                            Response.Write("success");
                        }
                        else if (trade_status == "TRADE_SUCCESS")
                        {
                            #region
                            if (!UpdatePayStatus.CheckIsUpdate(orderId))
                                UpdatePayStatus.AlipayUpdate(orderId, totalFee);

                            #endregion 支付宝回调地址

                            Response.Write("success");
                        }
                        else
                        {
                            Response.Write(trade_status);
                        }
                    }
                    catch (Exception exc)
                    {
                        Response.Write(exc.ToString());
                    }
                }
                else //验证失败
                {
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }
        }

        /// <summary>
        ///     获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        [ValidateInput(false)] //防止保一个错误"A potentially dangerous Request.Form value was detected from the client".
        public Dictionary<string, string> GetRequestPost()
        {
            var i = 0;
            var sArray = new Dictionary<string, string>();

            NameValueCollection coll = Request.Form;

            // Get names of all forms into a string array.
            var requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        #endregion

        #endregion
    }
}