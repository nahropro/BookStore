namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_total_to_bookeditionfirsttime_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookEditionFirstTimes", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookEditionFirstTimes", "Total");
        }
    }
}
