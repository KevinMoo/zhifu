using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ZhifuWeb.Models
{
    public class ShopCarViewModel
    {
        public ShopCarViewModel()
        {
            TicketInfo = new List<GoodsMessage>();
            CardInfo = new List<GoodsMessage>();
            UserInfo = new UserViewModel();
        }

        public List<GoodsMessage> TicketInfo { get; set; }

        public List<GoodsMessage> CardInfo { get; set; }

        public UserViewModel UserInfo { get; set; }

        /// <summary>
        /// 存入数据库中的订单号,唯一,防止重复添加数据到数据库中
        /// </summary>
        public string OrderId { get; set; }
    }

    /// <summary>
    ///     商品信息
    /// </summary>
    public class GoodsMessage
    {
        /// <summary>
        ///     数据库中的主键
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        ///     某类票/卡的总购买数量
        /// </summary>
        public int SumCount { get; set; }

        /// <summary>
        ///     某类票/卡的总价
        /// </summary>
        public decimal SumPrice { get; set; }

        /// <summary>
        ///     票/卡的名称
        /// </summary>
        public string ClassName { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
            Name = "";
            UserCard = "";
            Phone = "";
            GoDate = DateTime.Now.Hour > 8 ? DateTime.Now.AddDays(1) : DateTime.Now;
        }

        [Required(ErrorMessage = "姓名不能为空")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "身份证号不能为空")]
        [Display(Name = "身份证号")]
        [Remote("IsUserCard", "Booking", "请输入正确的身份证号")]
        public string UserCard { get; set; }

        [Required(ErrorMessage = "移动电话不能为空")]
        [Display(Name = "移动电话")]
        [Remote("IsPhone", "Booking", "请输入正确的号码")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "入园日期不能为空")]
        [Display(Name = "入园日期")]
        [Remote("CheckGoDate", "Booking", "请选择正确的时间(8点后当天的票不能预定)")]
        public DateTime GoDate { get; set; }

        public Decimal SumPrice { get; set; }
    }
}