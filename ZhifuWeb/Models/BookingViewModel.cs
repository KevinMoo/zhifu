using System.Collections.Generic;
using System.Linq;
using ZhifuWeb.EF.Models;
using ZhifuWeb.Helper;

namespace ZhifuWeb.Models
{
    public class BookingViewModel
    {
        public BookingViewModel()
        {
            var db = ConnHelper.CreateDb();
            Tickets = db.HappyCard_Class.Where(s => s.Type == "门票" && s.State == 1).ToList();
            Cards = db.HappyCard_Class.Where(s => s.Type == "年卡" && s.State == 1).ToList();
        }

        public List<HappyCard_Class> Tickets { get; set; }

        public List<HappyCard_Class> Cards { get; set; }
    }
}