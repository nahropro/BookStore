namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDefineTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookEditions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EditionInNumber = c.Int(nullable: false),
                        EditionInString = c.String(maxLength: 1000),
                        YearOfPrint = c.Int(),
                        PlaceOfPrint = c.String(maxLength: 1000),
                        NumberOfCopies = c.Int(),
                        BookId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Author = c.String(maxLength: 4000),
                        Translators = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 4000),
                        WorkPlace = c.String(maxLength: 4000),
                        Phone = c.String(maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FullName, unique: true);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Vaults",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                        FirstAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookEditions", "BookId", "dbo.Books");
            DropIndex("dbo.Vaults", new[] { "Name" });
            DropIndex("dbo.Stores", new[] { "Name" });
            DropIndex("dbo.Customers", new[] { "FullName" });
            DropIndex("dbo.BookEditions", new[] { "BookId" });
            DropTable("dbo.Vaults");
            DropTable("dbo.Stores");
            DropTable("dbo.Customers");
            DropTable("dbo.Books");
            DropTable("dbo.BookEditions");
        }
    }
}
