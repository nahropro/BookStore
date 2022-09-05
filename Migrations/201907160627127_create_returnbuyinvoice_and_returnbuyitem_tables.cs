namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_returnbuyinvoice_and_returnbuyitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReturnBuyInvoices",
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
            
            CreateTable(
                "dbo.ReturnBuyItems",
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
                .ForeignKey("dbo.ReturnBuyInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReturnBuyInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnBuyInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnBuyItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ReturnBuyItems", "InvoiceId", "dbo.ReturnBuyInvoices");
            DropForeignKey("dbo.ReturnBuyItems", "BookEditionId", "dbo.BookEditions");
            DropForeignKey("dbo.ReturnBuyInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.ReturnBuyItems", new[] { "InvoiceId" });
            DropIndex("dbo.ReturnBuyItems", new[] { "StoreId" });
            DropIndex("dbo.ReturnBuyItems", new[] { "BookEditionId" });
            DropIndex("dbo.ReturnBuyInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.ReturnBuyInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.ReturnBuyInvoices", new[] { "CustomerId" });
            DropTable("dbo.ReturnBuyItems");
            DropTable("dbo.ReturnBuyInvoices");
        }
    }
}
