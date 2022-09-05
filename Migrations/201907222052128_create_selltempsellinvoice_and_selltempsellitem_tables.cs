namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_selltempsellinvoice_and_selltempsellitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SellTempSellItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BookEditionId = c.Long(nullable: false),
                        StoreId = c.Long(nullable: false),
                        InvoiceId = c.Long(nullable: false),
                        Qtt = c.Long(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookEditions", t => t.BookEditionId)
                .ForeignKey("dbo.SellTempSellInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.SellTempSellInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        Discount = c.Decimal(precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellTempSellInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SellTempSellInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SellTempSellItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.SellTempSellItems", "InvoiceId", "dbo.SellTempSellInvoices");
            DropForeignKey("dbo.SellTempSellInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.SellTempSellItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.SellTempSellInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.SellTempSellInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.SellTempSellInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SellTempSellItems", new[] { "InvoiceId" });
            DropIndex("dbo.SellTempSellItems", new[] { "StoreId" });
            DropIndex("dbo.SellTempSellItems", new[] { "BookEditionId" });
            DropTable("dbo.SellTempSellInvoices");
            DropTable("dbo.SellTempSellItems");
        }
    }
}
