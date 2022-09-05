namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNoteToGiveInvoiceTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiveInvoices", "AmountNote", c => c.String(maxLength: 4000));
            AddColumn("dbo.GiveInvoices", "DiscountNote", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GiveInvoices", "DiscountNote");
            DropColumn("dbo.GiveInvoices", "AmountNote");
        }
    }
}
