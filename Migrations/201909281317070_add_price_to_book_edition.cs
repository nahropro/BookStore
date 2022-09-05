namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_price_to_book_edition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookEditions", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookEditions", "Price");
        }
    }
}
