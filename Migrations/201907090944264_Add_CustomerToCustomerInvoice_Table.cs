namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CustomerToCustomerInvoice_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerToCustomerInvoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PayCustomerId = c.Long(nullable: false),
                        GiveCustomerId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 4000),
                        InvoiceDate = c.DateTime(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreationUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        LastEditedUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.GiveCustomerId)
                .ForeignKey("dbo.Customers", t => t.PayCustomerId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.LastEditedUserId)
                .Index(t => t.PayCustomerId)
                .Index(t => t.GiveCustomerId)
                .Index(t => t.CreationUserId)
                .Index(t => t.LastEditedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerToCustomerInvoices", "LastEditedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomerToCustomerInvoices", "CreationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomerToCustomerInvoices", "PayCustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerToCustomerInvoices", "GiveCustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerToCustomerInvoices", new[] { "LastEditedUserId" });
            DropIndex("dbo.CustomerToCustomerInvoices", new[] { "CreationUserId" });
            DropIndex("dbo.CustomerToCustomerInvoices", new[] { "GiveCustomerId" });
            DropIndex("dbo.CustomerToCustomerInvoices", new[] { "PayCustomerId" });
            DropTable("dbo.CustomerToCustomerInvoices");
        }
    }
}
