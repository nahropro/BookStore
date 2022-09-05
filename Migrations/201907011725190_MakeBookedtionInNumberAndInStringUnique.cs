namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeBookedtionInNumberAndInStringUnique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.BookEditions", "EditionInNumber", unique: true);
            CreateIndex("dbo.BookEditions", "EditionInString", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.BookEditions", new[] { "EditionInString" });
            DropIndex("dbo.BookEditions", new[] { "EditionInNumber" });
        }
    }
}
