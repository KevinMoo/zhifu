using System;
using System.Collections.Generic;

namespace ZhifuWeb.Models
{
    public class QueryViewModel
    {
        public QueryViewModel()
        {
            Orders = new List<OrderInfoViewModel>();
        }

        public List<OrderInfoViewModel> Orders { get; set; }

        public string CredNo { get; set; }
    }

    public class OrderInfoViewModel
    {
        public string ClassName { get; set; }

        public int ClassId { get; set; }

        public decimal MunPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public DateTime AddTime { get; set; }

        public int Rtong { get; set; }

        public int IsLingPiao { get; set; }
    }
}