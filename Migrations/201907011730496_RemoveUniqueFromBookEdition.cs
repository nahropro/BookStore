namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUniqueFromBookEdition : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BookEditions", new[] { "EditionInNumber" });
            DropIndex("dbo.BookEditions", new[] { "EditionInString" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.BookEditions", "EditionInString", unique: true);
            CreateIndex("dbo.BookEditions", "EditionInNumber", unique: true);
        }
    }
}
