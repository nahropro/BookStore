namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_lostinvoice_and_lostitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LostItems",
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
                .ForeignKey("dbo.LostInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.LostInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LostInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LostInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LostItems", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.LostItems", "InvoiceId", "dbo.LostInvoices");
            DropForeignKey("dbo.LostItems", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.LostInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.LostInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.LostItems", new[] { "InvoiceId" });
            DropIndex("dbo.LostItems", new[] { "StoreId" });
            DropIndex("dbo.LostItems", new[] { "BookEditionId" });
            DropTable("dbo.LostInvoices");
            DropTable("dbo.LostItems");
        }
    }
}
