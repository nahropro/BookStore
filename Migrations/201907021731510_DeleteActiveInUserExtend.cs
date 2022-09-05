namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteActiveInUserExtend : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "UserExtend_Active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserExtend_Active", c => c.Boolean(nullable: false));
        }
    }
}
