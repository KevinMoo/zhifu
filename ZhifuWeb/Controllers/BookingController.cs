using Hzsp.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using ZhifuWeb.EF.Models;
using ZhifuWeb.Helper;
using ZhifuWeb.Models;

namespace ZhifuWeb.Controllers
{
    public class BookingController : BasicController
    {
        #region Ticket

        public ActionResult ChooseTicket()
        {
            var bookingViewModel = new BookingViewModel();
            return View(bookingViewModel);
        }

        [HttpPost]
        public ActionResult ChooseTicket(FormCollection formCollection)
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel ?? new ShopCarViewModel();
            shopCarViewModel.TicketInfo.Clear();
            shopCarViewModel.CardInfo.Clear();
            var db = ConnHelper.CreateDb();
            decimal sum = 0;
            foreach (var item in formCollection.AllKeys)
            {
                var id = Convert.ToInt32(item);
                var happyCardClass = db.HappyCard_Class.FirstOrDefault(s => s.ClassID == id);
                if (happyCardClass != null)
                {
                    shopCarViewModel.TicketInfo.Add(new GoodsMessage
                    {
                        ClassId = happyCardClass.ClassID,
                        ClassName = happyCardClass.ClassName,
                        SumCount = Convert.ToInt32(formCollection[item]),
                        SumPrice =
                            Convert.ToDecimal(Convert.ToInt32(formCollection[item]) * happyCardClass.DiscountPrice)
                    });
                    sum += Convert.ToDecimal(Convert.ToInt32(formCollection[item]) * happyCardClass.DiscountPrice);
                }
            }
            shopCarViewModel.UserInfo.SumPrice = sum;
            Session["Car"] = shopCarViewModel;

            return RedirectToAction("InformationTicket");
        }

        public ActionResult InformationTicket()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel != null && shopCarViewModel.TicketInfo.Count != 0)
            {
                return View(shopCarViewModel.UserInfo);
            }
            return RedirectToAction("ChooseTicket");
        }

        [HttpPost]
        public ActionResult InformationTicket(UserViewModel userViewModel)
        {
            #region

            var name = userViewModel.Name.SetNull();
            var phone = userViewModel.Phone.SetNull();
            var usercard = userViewModel.UserCard.SetNull();
            var goDate = userViewModel.GoDate;
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "姓名不能为空");
                return View(userViewModel);
            }
            if (!RegexHelpers.IsCNMobileNum(phone))
            {
                ModelState.AddModelError("phone", "手机号格式错误");
                return View(userViewModel);
            }
            if (!RegexHelpers.IsUserCard(usercard))
            {
                ModelState.AddModelError("usercard", "身份证号格式错误");
                return View(userViewModel);
            }
            try
            {
                var date = Convert.ToDateTime(goDate);
                if (date.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("goDate", "请选择正确的日期");
                    return View(userViewModel);
                }
                if (date.Date == DateTime.Now.Date)
                {
                    if (DateTime.Now.Hour >= 8)
                    {
                        ModelState.AddModelError("goDate", "8点后不能预定当天的票");
                        return View(userViewModel);
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("goDate", "请选择正确的日期");
                return View(userViewModel);
            }

            #endregion Ticket

            if (ModelState.IsValid)
            {
                var shopCarViewModel = Session["Car"] as ShopCarViewModel;
                if (shopCarViewModel != null && shopCarViewModel.TicketInfo.Count != 0)
                {
                    userViewModel.SumPrice = shopCarViewModel.UserInfo.SumPrice;
                    shopCarViewModel.UserInfo = userViewModel;

                    Session["Car"] = shopCarViewModel;
                }
                return RedirectToAction("CheckInfoTicket");
            }
            return View(userViewModel);
        }

        public ActionResult CheckInfoTicket()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel == null)
                return RedirectToAction("chooseTicket");
            if (shopCarViewModel.TicketInfo.Count == 0)
                return RedirectToAction("chooseTicket");
            if (shopCarViewModel.UserInfo == null)
                return RedirectToAction("InformationTicket");
            return View(shopCarViewModel);
        }

        #endregion

        #region Card

        public ActionResult ChooseCard()
        {
            var bookingViewModel = new BookingViewModel();
            return View(bookingViewModel);
        }

        [HttpPost]
        public ActionResult ChooseCard(FormCollection formCollection)
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel ?? new ShopCarViewModel();

            shopCarViewModel.TicketInfo.Clear();
            shopCarViewModel.CardInfo.Clear();
            var db = ConnHelper.CreateDb();
            decimal sum = 0;
            foreach (var item in formCollection.AllKeys)
            {
                var id = Convert.ToInt32(item);
                var happyCardClass = db.HappyCard_Class.SingleOrDefault(s => s.ClassID == id);
                if (happyCardClass != null)
                {
                    shopCarViewModel.CardInfo.Add(new GoodsMessage
                    {
                        ClassId = happyCardClass.ClassID,
                        ClassName = happyCardClass.ClassName,
                        SumCount = Convert.ToInt32(formCollection[item]),
                        SumPrice =
                            Convert.ToDecimal(Convert.ToInt32(formCollection[item]) * happyCardClass.DiscountPrice)
                    });
                    sum += Convert.ToDecimal(Convert.ToInt32(formCollection[item]) * happyCardClass.DiscountPrice);
                }
            }
            shopCarViewModel.UserInfo.SumPrice = sum;
            Session["Car"] = shopCarViewModel;

            return RedirectToAction("InformationCard");
        }

        public ActionResult InformationCard()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel != null && shopCarViewModel.CardInfo.Count != 0)
            {
                return View(shopCarViewModel.UserInfo);
            }
            return RedirectToAction("ChooseTicket");
        }

        [HttpPost]
        public ActionResult InformationCard(UserViewModel userViewModel)
        {
            #region

            var name = userViewModel.Name.SetNull();
            var phone = userViewModel.Phone.SetNull();
            var usercard = userViewModel.UserCard.SetNull();
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "姓名不能为空");
                return View(userViewModel);
            }
            if (!RegexHelpers.IsCNMobileNum(phone))
            {
                ModelState.AddModelError("phone", "手机号格式错误");
                return View(userViewModel);
            }
            if (!RegexHelpers.IsUserCard(usercard))
            {
                ModelState.AddModelError("usercard", "身份证号格式错误");
                return View(userViewModel);
            }

            #endregion

            if (ModelState.IsValid)
            {
                var shopCarViewModel = Session["Car"] as ShopCarViewModel;
                if (shopCarViewModel != null && shopCarViewModel.CardInfo.Count != 0)
                {
                    userViewModel.SumPrice = shopCarViewModel.UserInfo.SumPrice;
                    shopCarViewModel.UserInfo = userViewModel;

                    Session["Car"] = shopCarViewModel;
                }
                return RedirectToAction("CheckInfoCard");
            }
            return View(userViewModel);
        }

        public ActionResult CheckInfoCard()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel == null)
                return RedirectToAction("chooseCard");
            if (shopCarViewModel.CardInfo.Count == 0)
                return RedirectToAction("chooseCard");
            if (shopCarViewModel.UserInfo == null)
                return RedirectToAction("InformationTicket");
            return View(shopCarViewModel);
        }

        public JsonResult GetServerDate()
        {
            return Json(DateTime.Now.ToString(CultureInfo.InvariantCulture), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 添加订单信息,  根据不同的浏览器跳转到不同的支付页面

        public ActionResult SubmitOrder()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel == null)
                return RedirectToAction("chooseTicket");
            if (shopCarViewModel.TicketInfo.Count == 0 && shopCarViewModel.CardInfo.Count == 0)
                return RedirectToAction("chooseTicket");
            if (shopCarViewModel.UserInfo == null)
                return RedirectToAction("InformationTicket");

            #region 先添加所有的订单到数据库中
            var orderId = AddOrder(shopCarViewModel);
            shopCarViewModel.OrderId = orderId.ToString();
            #endregion

            if (Request.ServerVariables["HTTP_REFERER"].Contains("Ticket"))
            {
                ViewData["Prev"] = "/booking/CheckInfoTicket";
            }
            else
            {
                ViewData["Prev"] = "/booking/CheckInfoCard";
            }
            return View();
        }

        public ActionResult SubmitOrderWx()
        {
            var shopCarViewModel = Session["Car"] as ShopCarViewModel;
            if (shopCarViewModel == null)
                return RedirectToAction("chooseTicket");
            //如果没有票或卡的信息返回购票首页
            if (shopCarViewModel.TicketInfo.Count == 0 && shopCarViewModel.CardInfo.Count == 0)
                return RedirectToAction("chooseTicket");
            if (shopCarViewModel.UserInfo == null)
                return RedirectToAction("InformationTicket");

            #region 先添加所有的订单到数据库中

            var orderId = AddOrder(shopCarViewModel);
            LogHelper.CreateLog("微信支付,游客开始在官方网站下订单->当前未支付！(Booking.SubmitOrderWx)", orderId.ToString());
            shopCarViewModel.OrderId = orderId.ToString();
            #endregion

            if (Request.ServerVariables["HTTP_REFERER"].Contains("Ticket"))
            {
                ViewData["Prev"] = "/booking/CheckInfoTicket";
            }
            else
            {
                ViewData["Prev"] = "/booking/CheckInfoCard";
            }
            return View();
        }

        private static string AddOrder(ShopCarViewModel shopCarViewModel)
        {
            string str = "";
            var db = ConnHelper.CreateDb();
            Random random = new Random();
            var orderId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + random.Next(0, 9);
            List<GoodsMessage> items;
            if (shopCarViewModel.CardInfo.Count == 0)
            {
                items = shopCarViewModel.TicketInfo;
                str = "购买票";
            }
            else
            {
                items = shopCarViewModel.CardInfo;
                str = "购买年卡";
            }
            foreach (var item in items)
            {
                if (item.SumCount == 0)
                    continue;
                var orderInfoEntity = new Order_Info();
                orderInfoEntity.ClassID = item.ClassId;
                orderInfoEntity.Cred = "身份证";
                orderInfoEntity.CredNo = shopCarViewModel.UserInfo.UserCard;
                orderInfoEntity.Cren = 0;
                orderInfoEntity.Email = "";
                orderInfoEntity.Isfs = 0;
                orderInfoEntity.IsLingPiao = 0;
                orderInfoEntity.Lren = 0;
                orderInfoEntity.Mobile = shopCarViewModel.UserInfo.Phone;
                orderInfoEntity.MunPrice = item.SumPrice;
                orderInfoEntity.OrderID = orderId;
                orderInfoEntity.RTime = shopCarViewModel.UserInfo.GoDate.ToShortDateString();
                orderInfoEntity.Rtong = item.SumCount;
                orderInfoEntity.State = 0;
                orderInfoEntity.Ticket = shopCarViewModel.UserInfo.Name;
                orderInfoEntity.Ticketer = shopCarViewModel.UserInfo.Name;
                orderInfoEntity.Type = 0;
                orderInfoEntity.AddTime = DateTime.Now;
                orderInfoEntity.Detail = "手机网站";
                orderInfoEntity.IsPost = 0;
                orderInfoEntity.IsSMPost = 0;
                db.Order_Info.Add(orderInfoEntity);
            }
            db.SaveChanges();
            LogHelper.CreateLog("游客开始在官方网站下订单->当前未支付！(" + str + ")", orderId.ToString());
            return orderId.ToString();
        }

        #endregion

        #region 查询订单

        public ActionResult Query(string credNo)
        {
            QueryViewModel query = new QueryViewModel()
            {
                CredNo = credNo
            };
            var db = ConnHelper.CreateDb();
            if (!db.Order_Info.Any(s => s.CredNo == credNo))
            {
                ModelState.AddModelError("error", "该身份证不存在!");
                return View(query);
            }
            var list = db.Order_Info.Where(s => s.State == 1 && s.CredNo == credNo).OrderByDescending(s => s.AddTime).ToList();
            foreach (var item in list)
            {
                var happy_card = db.HappyCard_Class.Single(s => s.State == 1 && s.ClassID == item.ClassID);
                query.Orders.Add(new OrderInfoViewModel()
                {
                    AddTime = item.AddTime,
                    ClassId = item.ClassID,
                    ClassName = happy_card.ClassName,
                    DiscountPrice = Convert.ToDecimal(happy_card.DiscountPrice),
                    MunPrice = item.MunPrice,
                    Rtong = item.Rtong,
                    IsLingPiao = item.IsLingPiao
                });
            }
            return View(query);
        }

        #endregion

        #region 检查用户信息

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)] //清除缓存
        public JsonResult IsPhone(string phone)
        {
            var valid = RegexHelpers.IsCNMobileNum(phone);
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsUserCard(string userCard)
        {
            var valid = RegexHelpers.IsIDCard(userCard);
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     检查入园日期,如果当天预定的晚于8点不能预定
        /// </summary>
        /// <param name="goDate"></param>
        /// <returns></returns>
        public JsonResult CheckGoDate(string goDate)
        {
            var valid = false;
            try
            {
                var date = Convert.ToDateTime(goDate);
                if (date.Date < DateTime.Now.Date)
                {
                    valid = false;
                }
                if (date.Date == DateTime.Now.Date)
                {
                    valid = date.Hour <= 8;
                }
                if (date.Date > DateTime.Now.Date)
                {
                    valid = true;
                }
            }
            catch
            {
                valid = false;
            }
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}