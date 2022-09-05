namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateGiveInvoiceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiveInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        VaultId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceDate = c.DateTime(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreationUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        LastEditedUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Vaults", t => t.VaultId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.LastEditedUserId)
                .Index(t => t.CustomerId)
                .Index(t => t.VaultId)
                .Index(t => t.CreationUserId)
                .Index(t => t.LastEditedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GiveInvoices", "LastEditedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiveInvoices", "CreationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiveInvoices", "VaultId", "dbo.Vaults");
            DropForeignKey("dbo.GiveInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.GiveInvoices", new[] { "LastEditedUserId" });
            DropIndex("dbo.GiveInvoices", new[] { "CreationUserId" });
            DropIndex("dbo.GiveInvoices", new[] { "VaultId" });
            DropIndex("dbo.GiveInvoices", new[] { "CustomerId" });
            DropTable("dbo.GiveInvoices");
        }
    }
}
