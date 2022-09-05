namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_sellinvoice_and_sellitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SellItems",
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
                .ForeignKey("dbo.SellInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.SellInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            DropForeignKey("dbo.SellInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SellInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SellItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.SellItems", "InvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.SellItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.SellInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.SellInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.SellInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SellItems", new[] { "InvoiceId" });
            DropIndex("dbo.SellItems", new[] { "StoreId" });
            DropIndex("dbo.SellItems", new[] { "BookEditionId" });
            DropTable("dbo.SellInvoices");
            DropTable("dbo.SellItems");
        }
    }
}
