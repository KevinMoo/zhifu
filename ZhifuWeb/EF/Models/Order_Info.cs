
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZhifuWeb.EF.Models
{
    public partial class Order_Info
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string OrderID { get; set; }

        public string Ticketer { get; set; }

        public string Ticket { get; set; }

        public string Cred { get; set; }

        public string CredNo { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int ClassID { get; set; }

        public int Rtong { get; set; }

        public int Cren { get; set; }

        public int Lren { get; set; }

        public decimal MunPrice { get; set; }

        public int Type { get; set; }

        public string RTime { get; set; }

        public System.DateTime AddTime { get; set; }

        public int State { get; set; }

        public int IsLingPiao { get; set; }

        public int Isfs { get; set; }

        public DateTime? LingpiaoTime { get; set; }

        public string PayType { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? IsUpdate { get; set; }

        public string Detail { get; set; }

        public int? DaStu { get; set; }

        public int? IsPost { get; set; }

        public DateTime? PostTime { get; set; }

        public int? IsSMPost { get; set; }

        public int? OrderState { get; set; }

        public string ChangeFrom { get; set; }

        public DateTime? ChangeTime { get; set; }
    }
}