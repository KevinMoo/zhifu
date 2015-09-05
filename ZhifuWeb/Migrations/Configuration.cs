using System;
using System.Collections.Generic;
using ZhifuWeb.EF.Models;

namespace ZhifuWeb.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ZhifuWeb.EF.ZhifuDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ZhifuWeb.EF.ZhifuDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );

            var happycards = new List<HappyCard_Class>
            {
                new HappyCard_Class {ClassName = "ȫ��Ʊ", AddTime = DateTime.Now, Price = 100, DiscountPrice = 200,State = 1,Type = "��Ʊ"},
                new HappyCard_Class {ClassName = "���Ʊ", AddTime = DateTime.Now, Price = 50, DiscountPrice = 120,State = 1,Type = "��Ʊ"},
                new HappyCard_Class {ClassName = "��ѧ����Ʊ", AddTime = DateTime.Now, Price = 10, DiscountPrice = 60,State = 1,Type = "��Ʊ"},
                new HappyCard_Class {ClassName = "������Ʊ", AddTime = DateTime.Now, Price = 30, DiscountPrice = 90,State = 1,Type = "��Ʊ"},
                new HappyCard_Class {ClassName = "��ͥ��Ʊ", AddTime = DateTime.Now, Price = 80, DiscountPrice = 250,State = 1,Type = "��Ʊ"},
                new HappyCard_Class {ClassName = "��ͥ��", AddTime = DateTime.Now, Price = 1000, DiscountPrice = 550,State = 1,Type = "�꿨"},
                new HappyCard_Class {ClassName = "���˿�", AddTime = DateTime.Now, Price = 2000, DiscountPrice = 950,State = 1,Type = "�꿨"},
                new HappyCard_Class {ClassName = "��ۿ�", AddTime = DateTime.Now, Price = 3000, DiscountPrice = 1850,State = 1,Type = "�꿨"},
                new HappyCard_Class {ClassName = "˫���꿨", AddTime = DateTime.Now, Price = 4000, DiscountPrice = 2250,State = 1,Type = "�꿨"},
            };
            context.HappyCard_Class.AddRange(happycards);
            context.SaveChanges();
        }
    }
}
