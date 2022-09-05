namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PluralAuthorInBookTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Authors", c => c.String(maxLength: 4000));
            DropColumn("dbo.Books", "Author");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "Author", c => c.String(maxLength: 4000));
            DropColumn("dbo.Books", "Authors");
        }
    }
}
