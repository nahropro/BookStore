namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_incomeinvoice_and_incomeitem_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncomeInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
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
                "dbo.IncomeItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceId = c.Long(nullable: false),
                        IncomeTypeId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IncomeTypes", t => t.IncomeTypeId)
                .ForeignKey("dbo.IncomeInvoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.IncomeTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncomeInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.IncomeInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.IncomeInvoices", "VaultId", "dbo.Vaults");
            DropForeignKey("dbo.IncomeItems", "InvoiceId", "dbo.IncomeInvoices");
            DropForeignKey("dbo.IncomeItems", "IncomeTypeId", "dbo.IncomeTypes");
            DropForeignKey("dbo.IncomeInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.IncomeItems", new[] { "IncomeTypeId" });
            DropIndex("dbo.IncomeItems", new[] { "InvoiceId" });
            DropIndex("dbo.IncomeInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.IncomeInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.IncomeInvoices", new[] { "CustomerId" });
            DropIndex("dbo.IncomeInvoices", new[] { "VaultId" });
            DropTable("dbo.IncomeItems");
            DropTable("dbo.IncomeInvoices");
        }
    }
}
