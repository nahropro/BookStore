namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_VaultToVaultInvoice_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VaultToVaultInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PayVaultId = c.Long(nullable: false),
                        GiveVaultId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 4000),
                        InvoiceDate = c.DateTime(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreationUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        LastEditedUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vaults", t => t.GiveVaultId)
                .ForeignKey("dbo.Vaults", t => t.PayVaultId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.LastEditedUserId)
                .Index(t => t.PayVaultId)
                .Index(t => t.GiveVaultId)
                .Index(t => t.CreationUserId)
                .Index(t => t.LastEditedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VaultToVaultInvoices", "LastEditedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VaultToVaultInvoices", "CreationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VaultToVaultInvoices", "PayVaultId", "dbo.Vaults");
            DropForeignKey("dbo.VaultToVaultInvoices", "GiveVaultId", "dbo.Vaults");
            DropIndex("dbo.VaultToVaultInvoices", new[] { "LastEditedUserId" });
            DropIndex("dbo.VaultToVaultInvoices", new[] { "CreationUserId" });
            DropIndex("dbo.VaultToVaultInvoices", new[] { "GiveVaultId" });
            DropIndex("dbo.VaultToVaultInvoices", new[] { "PayVaultId" });
            DropTable("dbo.VaultToVaultInvoices");
        }
    }
}
