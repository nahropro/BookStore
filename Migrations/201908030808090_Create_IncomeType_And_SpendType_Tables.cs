namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_IncomeType_And_SpendType_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncomeTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.SpendTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpendTypes", new[] { "Name" });
            DropIndex("dbo.IncomeTypes", new[] { "Name" });
            DropTable("dbo.SpendTypes");
            DropTable("dbo.IncomeTypes");
        }
    }
}
