namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_bookeditionfirsttime_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookEditionFirstTimes",
                c => new
                    {
                        BookEditionId = c.Long(nullable: false),
                        StoreId = c.Long(nullable: false),
                        Qtt = c.Long(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreatorUserId = c.String(nullable: false, maxLength: 128),
                        LastEditedDateTime = c.DateTime(),
                        EditorUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BookEditionId, t.StoreId })
                .ForeignKey("dbo.BookEditions", t => t.BookEditionId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.EditorUserId)
                .Index(t => t.BookEditionId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatorUserId)
                .Index(t => t.EditorUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookEditionFirstTimes", "EditorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookEditionFirstTimes", "CreatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookEditionFirstTimes", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.BookEditionFirstTimes", "BookEditionId", "dbo.BookEditions");
            DropIndex("dbo.BookEditionFirstTimes", new[] { "EditorUserId" });
            DropIndex("dbo.BookEditionFirstTimes", new[] { "CreatorUserId" });
            DropIndex("dbo.BookEditionFirstTimes", new[] { "StoreId" });
            DropIndex("dbo.BookEditionFirstTimes", new[] { "BookEditionId" });
            DropTable("dbo.BookEditionFirstTimes");
        }
    }
}
