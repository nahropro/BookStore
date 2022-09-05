namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class set_discount_on_item_invoices_nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SellInvoices", "Discount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SellInvoices", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
