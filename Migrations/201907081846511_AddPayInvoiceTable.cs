namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPayInvoiceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        VaultId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountNote = c.String(maxLength: 4000),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountNote = c.String(maxLength: 4000),
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
            DropForeignKey("dbo.PayInvoices", "LastEditedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayInvoices", "CreationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayInvoices", "VaultId", "dbo.Vaults");
            DropForeignKey("dbo.PayInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.PayInvoices", new[] { "LastEditedUserId" });
            DropIndex("dbo.PayInvoices", new[] { "CreationUserId" });
            DropIndex("dbo.PayInvoices", new[] { "VaultId" });
            DropIndex("dbo.PayInvoices", new[] { "CustomerId" });
            DropTable("dbo.PayInvoices");
        }
    }
}
