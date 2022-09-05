namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_vault_correction_invoice_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VaultCorrectionInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VaultId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceDate = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 4000),
                        CorrectionType = c.Int(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vaults", t => t.VaultId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.VaultId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VaultCorrectionInvoices", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VaultCorrectionInvoices", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VaultCorrectionInvoices", "VaultId", "dbo.Vaults");
            DropIndex("dbo.VaultCorrectionInvoices", new[] { "EditorUserId" });
            DropIndex("dbo.VaultCorrectionInvoices", new[] { "CreatorUserId" });
            DropIndex("dbo.VaultCorrectionInvoices", new[] { "VaultId" });
            DropTable("dbo.VaultCorrectionInvoices");
        }
    }
}
