namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_discount_from_spendinvoice : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SpendInvoices", "Discount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SpendInvoices", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
