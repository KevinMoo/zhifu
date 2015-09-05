using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using Hzsp.Helper;
using ZhifuWeb.Helper;
using ZhifuWeb.lib.ChinaUnion;
using ZhifuWeb.Models;

namespace ZhifuWeb.Controllers
{
    public class ChinaUnionController : BasicController
    {
        private object _obj = new object();

        public ActionResult FrontConsume()
        {
            var shopcarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopcarViewModel == null)
                return RedirectToAction("CheckInfoTicket", "Booking");
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel
            {
                OrderId = shopcarViewModel.OrderId,
                TxnAmt = Convert.ToInt32(shopcarViewModel.UserInfo.SumPrice * 100).ToString()
            };
            Session["Car"] = null;
            return View(chinaUnionViewModel);
        }

        /// <summary>
        /// 前台通知页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FrontUrl()
        {
            var payResult = new PayResultViewModel();
            if (Request.HttpMethod == "POST")
            {
                #region 使用Dictionary保存参数
                Dictionary<string, string> resData = new Dictionary<string, string>();

                NameValueCollection coll = Request.Form;

                string[] requestItem = coll.AllKeys;

                for (int i = 0; i < requestItem.Length; i++)
                {
                    resData.Add(requestItem[i], Request.Form[requestItem[i]]);
                }

                string respcode = resData["respCode"];
                #endregion 使用Dictionary保存参数
                // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
                if (SDKUtil.Validate(resData, Encoding.UTF8))
                {
                    #region 报文结果写入文件
                    //商户端根据返回报文内容处理自己的业务逻辑 ,DEMO此处只输出报文结果
                    StringBuilder builder = new StringBuilder();

                    builder.Append("ChinaUnion FrontUrl: <table><tr><td align=\"center\" colspan=\"2\"><b>商户端接收银联返回报文并按照表格形式输出结果</b></td></tr>");

                    for (int i = 0; i < requestItem.Length; i++)
                    {
                        builder.Append("<tr><td width=\"30%\" align=\"right\">" + requestItem[i] + "</td><td style='word-break:break-all'>" + Request.Form[requestItem[i]] + "</td></tr>");
                    }

                    builder.Append("<tr><td width=\"30%\" align=\"right\">商户端验证银联返回报文结果</td><td>验证签名成功.</td></tr></table>");
                    LogHelper.CreateLog(builder.ToString(), resData["orderId"]);
                    #endregion 报文结果写入文件
                    if (resData["respMsg"] == "success")
                    {
                        payResult.Status = true;
                    }
                    else
                    {//支付失败
                        payResult.Status = false;
                    }
                }
                else
                {
                    payResult.Status = false;
                    LogHelper.WriteLog(">商户端验证银联返回报文结果</td><td>验证签名失败.", "~/log/银联Front验证失败", "error_____" + DateTime.Now.Ticks);
                }
            }
            return View(payResult);
        }

        /// <summary>
        /// 后台通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void BackReceive()
        {
            lock (_obj)
            {
                if (Request.HttpMethod == "POST")
                {
                    #region 使用Dictionary保存参数
                    Dictionary<string, string> resData = new Dictionary<string, string>();

                    NameValueCollection coll = Request.Form;

                    string[] requestItem = coll.AllKeys;

                    for (int i = 0; i < requestItem.Length; i++)
                    {
                        resData.Add(requestItem[i], Request.Form[requestItem[i]]);
                    }

                    string respcode = resData["respCode"];
                    #endregion 使用Dictionary保存参数
                    // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
                    if (SDKUtil.Validate(resData, Encoding.UTF8))
                    {
                        #region
                        //我先将所有的返回值答应出来
                        //商户端根据返回报文内容处理自己的业务逻辑 ,DEMO此处只输出报文结果
                        StringBuilder builder = new StringBuilder();

                        builder.Append("ChinaUnion BackReceiver:<table><tr><td align=\"center\" colspan=\"2\"><b>商户端接收银联返回报文并按照表格形式输出结果</b></td></tr>");

                        for (int i = 0; i < requestItem.Length; i++)
                        {
                            builder.Append("<tr><td width=\"30%\" align=\"right\">" + requestItem[i] + "</td><td style='word-break:break-all'>" + Request.Form[requestItem[i]] + "</td></tr>");
                        }
                        builder.Append("<tr><td width=\"30%\" align=\"right\">商户端验证银联返回报文结果</td><td>验证签名成功.</td></tr><table>");
                        LogHelper.CreateLog(builder.ToString(), resData["orderId"]);
                        #endregion

                        if (resData["respMsg"] == "Success!")
                        {
                            var orderId = resData["orderId"];
                            var totalFee = resData["settleAmt"];
                            if (!UpdatePayStatus.CheckIsUpdate(orderId))
                            {
                                UpdatePayStatus.ChinaUnionUpdate(orderId, totalFee);
                            }
                        }
                        else
                        {//支付失败
                            RedirectToAction("FrontConsume");
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(">商户端验证银联返回报文结果</td><td>验证签名失败.", "~/log/银联BackReceive验证失败");
                    }
                }
            }
        }

        #region

        public ActionResult Query()
        {
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel();
            return View(chinaUnionViewModel);
        }

        /// <summary>
        /// 消费撤销
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsumeUndo()
        {
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel();
            return View(chinaUnionViewModel);
        }

        /// <summary>
        /// 退货接口
        /// </summary>
        /// <returns></returns>
        public ActionResult Refund()
        {
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel();
            return View(chinaUnionViewModel);
        }

        /// <summary>
        /// 前台通知
        /// </summary>
        /// <returns></returns>
        public ActionResult FrontReceive()
        {
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel();
            return View(chinaUnionViewModel);
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public ActionResult FileTransfer()
        {
            ChinaUnionViewModel chinaUnionViewModel = new ChinaUnionViewModel();
            return View(chinaUnionViewModel);
        }

        #endregion
    }
}