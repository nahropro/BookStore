namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserExtend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserExtend_FullName", c => c.String(nullable: false, maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "UserExtend_Phone", c => c.String(maxLength: 1000));
            AddColumn("dbo.AspNetUsers", "UserExtend_Address", c => c.String(maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "UserExtend_Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserExtend_Active");
            DropColumn("dbo.AspNetUsers", "UserExtend_Address");
            DropColumn("dbo.AspNetUsers", "UserExtend_Phone");
            DropColumn("dbo.AspNetUsers", "UserExtend_FullName");
        }
    }
}
