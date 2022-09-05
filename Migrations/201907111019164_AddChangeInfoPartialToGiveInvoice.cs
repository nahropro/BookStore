namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeInfoPartialToGiveInvoice : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.GiveInvoices", name: "CreationUserId", newName: "CreatorUserId");
            RenameColumn(table: "dbo.GiveInvoices", name: "LastEditedUserId", newName: "EditorUserId");
            RenameIndex(table: "dbo.GiveInvoices", name: "IX_CreationUserId", newName: "IX_CreatorUserId");
            RenameIndex(table: "dbo.GiveInvoices", name: "IX_LastEditedUserId", newName: "IX_EditorUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.GiveInvoices", name: "IX_EditorUserId", newName: "IX_LastEditedUserId");
            RenameIndex(table: "dbo.GiveInvoices", name: "IX_CreatorUserId", newName: "IX_CreationUserId");
            RenameColumn(table: "dbo.GiveInvoices", name: "EditorUserId", newName: "LastEditedUserId");
            RenameColumn(table: "dbo.GiveInvoices", name: "CreatorUserId", newName: "CreationUserId");
        }
    }
}
