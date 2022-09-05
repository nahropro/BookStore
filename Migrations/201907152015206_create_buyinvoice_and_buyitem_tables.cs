namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_buyinvoice_and_buyitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyItems",
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
                .ForeignKey("dbo.BuyInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.BuyInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        Discount = c.Decimal(precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            DropForeignKey("dbo.BuyInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BuyInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BuyItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.BuyItems", "InvoiceId", "dbo.BuyInvoices");
            DropForeignKey("dbo.BuyInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.BuyItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.BuyInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.BuyInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.BuyInvoices", new[] { "CustomerId" });
            DropIndex("dbo.BuyItems", new[] { "InvoiceId" });
            DropIndex("dbo.BuyItems", new[] { "StoreId" });
            DropIndex("dbo.BuyItems", new[] { "BookEditionId" });
            DropTable("dbo.BuyInvoices");
            DropTable("dbo.BuyItems");
        }
    }
}
