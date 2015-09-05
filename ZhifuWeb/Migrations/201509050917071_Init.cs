namespace ZhifuWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HappyCard_Class",
                c => new
                    {
                        ClassID = c.Int(nullable: false, identity: true),
                        ClassName = c.String(),
                        AddTime = c.DateTime(nullable: false),
                        Content = c.String(),
                        UpDateBy = c.String(),
                        UpDateTime = c.DateTime(),
                        IsDelete = c.Int(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        DiscountPrice = c.Decimal(precision: 18, scale: 2),
                        Type = c.String(),
                        goodsCode = c.String(),
                        goodsName = c.String(),
                        State = c.Int(),
                    })
                .PrimaryKey(t => t.ClassID);
            
            CreateTable(
                "dbo.Order_Info",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderID = c.String(),
                        Ticketer = c.String(),
                        Ticket = c.String(),
                        Cred = c.String(),
                        CredNo = c.String(),
                        Email = c.String(),
                        Mobile = c.String(),
                        ClassID = c.Int(nullable: false),
                        Rtong = c.Int(nullable: false),
                        Cren = c.Int(nullable: false),
                        Lren = c.Int(nullable: false),
                        MunPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        RTime = c.String(),
                        AddTime = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                        IsLingPiao = c.Int(nullable: false),
                        Isfs = c.Int(nullable: false),
                        LingpiaoTime = c.DateTime(),
                        PayType = c.String(),
                        UpdateBy = c.String(),
                        UpdateTime = c.DateTime(),
                        IsUpdate = c.Int(),
                        Detail = c.String(),
                        DaStu = c.Int(),
                        IsPost = c.Int(),
                        PostTime = c.DateTime(),
                        IsSMPost = c.Int(),
                        OrderState = c.Int(),
                        ChangeFrom = c.String(),
                        ChangeTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Order_Info");
            DropTable("dbo.HappyCard_Class");
        }
    }
}
