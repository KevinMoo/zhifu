using Hzsp.Helper;
using System;
using System.Linq;

namespace ZhifuWeb.Helper
{
    public static class UpdatePayStatus
    {
        #region 网银处理订单状态

        public static void ChinaUnionUpdate(string orderId, string totalFee)
        {
            try
            {
                var db = ConnHelper.CreateDb();
                var list = db.Order_Info.Where(e => e.OrderID.ToString() == orderId).ToList();
                var countprice = list.Sum(e => e.MunPrice);
                if (list.Count > 0)
                {
                    LogHelper.CreateLog(string.Format("countprice:{0}   totalFee:{1}", countprice, totalFee), orderId);
                    if (countprice * 100 == Convert.ToDecimal(totalFee))
                    {
                        foreach (var item in list)
                        {
                            item.State = 1;
                            item.PayType = "网银";
                        }
                        db.SaveChanges();
                        LogHelper.CreateLog("db update success! 银联支付更新成功.\r\n\r\n", orderId.ToString());
                        LogHelper.CreateLog("游客开始在官方网站下订单->支付成功！(ChinaUnionUpdate)", orderId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //如果导致更新数据失败了写在这个日志文件夹下
                LogHelper.CreateLog(string.Format("{0}\r\n{1}", ex.Source, ex.StackTrace), orderId.ToString());
            }
        }

        #endregion 网银处理订单状态

        #region 支付宝处理订单状态

        public static void AlipayUpdate(string orderId, string totalFee)
        {
            try
            {
                var db = ConnHelper.CreateDb();
                var list = db.Order_Info.Where(e => e.OrderID.ToString() == orderId).ToList();
                var content = "欢迎您！您已成功在线预定购买了";
                var countprice = list.Sum(e => e.MunPrice);
                if (list.Count > 0)
                {
                    //手机支付宝是直接使用小数的
                    LogHelper.CreateLog(string.Format("countprice:{0}   totalFee:{1}", countprice, totalFee), orderId);
                    if (countprice == Convert.ToDecimal(totalFee))
                    {
                        foreach (var item in list)
                        {
                            var hc = db.HappyCard_Class.First(e => e.ClassID == item.ClassID);
                            content = content + " " + hc.ClassName;
                            content = content + item.Rtong + "张";
                            item.State = 1;
                            item.PayType = "支付宝";
                        }
                        db.SaveChanges();
                        LogHelper.CreateLog("db update success! 支付宝支付更新成功.\r\n\r\n", orderId);
                        LogHelper.CreateLog("游客开始在官方网站下订单->支付成功！(AlipayUpdate)",
                            orderId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //如果导致更新数据失败了写在这个日志文件夹下
                LogHelper.CreateLog(string.Format("{0}\r\n{1}", ex.Source, ex.StackTrace), orderId.ToString());
            }
        }

        #endregion 支付宝处理订单状态

        #region 微信更新

        public static void WxUpdate(string orderId, string totalFee)
        {
            try
            {
                var db = ConnHelper.CreateDb();
                var list = db.Order_Info.Where(e => e.OrderID.ToString() == orderId).ToList();
                var countprice = list.Sum(e => e.MunPrice);
                if (list.Count > 0)
                {
                    LogHelper.CreateLog(string.Format("countprice:{0}   totalFee:{1}", countprice, totalFee), orderId);
                    if (countprice * 100 == Convert.ToDecimal(totalFee))
                    {
                        foreach (var item in list)
                        {
                            item.State = 1;
                            item.PayType = "微信";
                        }
                        db.SaveChanges();
                        LogHelper.CreateLog("db update success! 微信支付更新成功.\r\n\r\n", orderId.ToString());
                        LogHelper.CreateLog("游客开始在官方网站下订单->支付成功！(WxUpdate)",
                            orderId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //如果导致更新数据失败了写在这个日志文件夹下
                LogHelper.CreateLog(string.Format("{0}\r\n{1}", ex.Source, ex.StackTrace), orderId.ToString());
            }
        }

        #endregion 微信更新

        internal static bool CheckIsUpdate(string orderId)
        {
            var db = ConnHelper.CreateDb();
            var orderInfo = db.Order_Info.Where(e => e.OrderID.ToString() == orderId).ToList().FirstOrDefault();
            if (orderInfo == null)
                throw new Exception("使用orderId找不到对应的数据库记录");

            if (orderInfo.State == 1)
            {
                return true;
            }
            return false;
        }
    }
}