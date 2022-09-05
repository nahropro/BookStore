namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_selltempsell_tables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SellTempSellItems", "StoreId", "dbo.Stores");
            DropIndex("dbo.SellTempSellItems", new[] { "StoreId" });
            DropColumn("dbo.SellTempSellItems", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SellTempSellItems", "StoreId", c => c.Long(nullable: false));
            CreateIndex("dbo.SellTempSellItems", "StoreId");
            AddForeignKey("dbo.SellTempSellItems", "StoreId", "dbo.Stores", "Id");
        }
    }
}
