namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_storetransferinvoce_and_storetransferitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreTransferInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceDate = c.DateTime(nullable: false),
                        FromStoreId = c.Long(nullable: false),
                        ToStoreId = c.Long(nullable: false),
                        Note = c.String(maxLength: 4000),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.FromStoreId)
                .ForeignKey("dbo.Stores", t => t.ToStoreId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.FromStoreId)
                .Index(t => t.ToStoreId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
            CreateTable(
                "dbo.StoreTransferItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BookEditionId = c.Long(nullable: false),
                        Qtt = c.Long(nullable: false),
                        InvoiceId = c.Long(nullable: false),
                        Note = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookEditions", t => t.BookEditionId)
                .ForeignKey("dbo.StoreTransferInvoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.BookEditionId)
                .Index(t => t.InvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreTransferInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreTransferInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StoreTransferInvoices", "ToStoreId", "dbo.Stores");
            DropForeignKey("dbo.StoreTransferItems", "InvoiceId", "dbo.StoreTransferInvoices");
            DropForeignKey("dbo.StoreTransferItems", "BookEditionId", "dbo.BookEditions");
            DropForeignKey("dbo.StoreTransferInvoices", "FromStoreId", "dbo.Stores");
            DropIndex("dbo.StoreTransferItems", new[] { "InvoiceId" });
            DropIndex("dbo.StoreTransferItems", new[] { "BookEditionId" });
            DropIndex("dbo.StoreTransferInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.StoreTransferInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.StoreTransferInvoices", new[] { "ToStoreId" });
            DropIndex("dbo.StoreTransferInvoices", new[] { "FromStoreId" });
            DropTable("dbo.StoreTransferItems");
            DropTable("dbo.StoreTransferInvoices");
        }
    }
}
