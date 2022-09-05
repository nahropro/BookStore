namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_returntesmpsellinvoice_and_returntempsellitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReturnTempSellItems",
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
                .ForeignKey("dbo.ReturnTempSellInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.ReturnTempSellInvoices",
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
            DropForeignKey("dbo.ReturnTempSellInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnTempSellInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReturnTempSellItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ReturnTempSellItems", "InvoiceId", "dbo.ReturnTempSellInvoices");
            DropForeignKey("dbo.ReturnTempSellInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.ReturnTempSellItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.ReturnTempSellInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.ReturnTempSellInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.ReturnTempSellInvoices", new[] { "CustomerId" });
            DropIndex("dbo.ReturnTempSellItems", new[] { "InvoiceId" });
            DropIndex("dbo.ReturnTempSellItems", new[] { "StoreId" });
            DropIndex("dbo.ReturnTempSellItems", new[] { "BookEditionId" });
            DropTable("dbo.ReturnTempSellInvoices");
            DropTable("dbo.ReturnTempSellItems");
        }
    }
}
