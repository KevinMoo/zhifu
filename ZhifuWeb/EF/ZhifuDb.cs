using System.Data.Entity;
using ZhifuWeb.EF.Models;

namespace ZhifuWeb.EF
{
    public class ZhifuDb : DbContext
    {
        public ZhifuDb()
            : base("ZhifuDb")
        {

        }

        public DbSet<HappyCard_Class> HappyCard_Class { get; set; }

        public DbSet<Order_Info> Order_Info { get; set; }
    }
}