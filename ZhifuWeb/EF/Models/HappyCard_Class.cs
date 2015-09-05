
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZhifuWeb.EF.Models
{
    public partial class HappyCard_Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassID { get; set; }

        public string ClassName { get; set; }

        public System.DateTime AddTime { get; set; }

        public string Content { get; set; }

        public string UpDateBy { get; set; }

        public DateTime? UpDateTime { get; set; }

        public int? IsDelete { get; set; }

        public decimal? Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public string Type { get; set; }

        public string goodsCode { get; set; }

        public string goodsName { get; set; }

        public int? State { get; set; }
    }
}