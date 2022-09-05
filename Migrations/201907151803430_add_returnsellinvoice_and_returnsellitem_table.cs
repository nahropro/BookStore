namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_returnsellinvoice_and_returnsellitem_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReturnSellItems",
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
                .ForeignKey("dbo.ReturnSellInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.ReturnSellInvoices",
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
            DropForeignKey("dbo.ReturnSellInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnSellInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnSellItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ReturnSellItems", "InvoiceId", "dbo.ReturnSellInvoices");
            DropForeignKey("dbo.ReturnSellInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.ReturnSellItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.ReturnSellInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.ReturnSellInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.ReturnSellInvoices", new[] { "CustomerId" });
            DropIndex("dbo.ReturnSellItems", new[] { "InvoiceId" });
            DropIndex("dbo.ReturnSellItems", new[] { "StoreId" });
            DropIndex("dbo.ReturnSellItems", new[] { "BookEditionId" });
            DropTable("dbo.ReturnSellInvoices");
            DropTable("dbo.ReturnSellItems");
        }
    }
}
