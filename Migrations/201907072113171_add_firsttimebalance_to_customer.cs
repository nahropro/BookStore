namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_firsttimebalance_to_customer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "FirstTimeBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "FirstTimeBalance");
        }
    }
}
