namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_spendinvoice_and_spenditem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpendInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VaultId = c.Long(),
                        Loan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerId = c.Long(),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Vaults", t => t.VaultId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.VaultId)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
            CreateTable(
                "dbo.SpendItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceId = c.Long(nullable: false),
                        SpendTypeId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SpendInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.SpendTypes", t => t.SpendTypeId)
                .Index(t => t.InvoiceId)
                .Index(t => t.SpendTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpendInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SpendInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SpendInvoices", "VaultId", "dbo.Vaults");
            DropForeignKey("dbo.SpendItems", "SpendTypeId", "dbo.SpendTypes");
            DropForeignKey("dbo.SpendItems", "InvoiceId", "dbo.SpendInvoices");
            DropForeignKey("dbo.SpendInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SpendItems", new[] { "SpendTypeId" });
            DropIndex("dbo.SpendItems", new[] { "InvoiceId" });
            DropIndex("dbo.SpendInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.SpendInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.SpendInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SpendInvoices", new[] { "VaultId" });
            DropTable("dbo.SpendItems");
            DropTable("dbo.SpendInvoices");
        }
    }
}
