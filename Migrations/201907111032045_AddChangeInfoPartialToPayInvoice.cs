namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeInfoPartialToPayInvoice : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PayInvoices", name: "CreationUserId", newName: "CreatorUserId");
            RenameColumn(table: "dbo.PayInvoices", name: "LastEditedUserId", newName: "EditorUserId");
            RenameIndex(table: "dbo.PayInvoices", name: "IX_CreationUserId", newName: "IX_CreatorUserId");
            RenameIndex(table: "dbo.PayInvoices", name: "IX_LastEditedUserId", newName: "IX_EditorUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PayInvoices", name: "IX_EditorUserId", newName: "IX_LastEditedUserId");
            RenameIndex(table: "dbo.PayInvoices", name: "IX_CreatorUserId", newName: "IX_CreationUserId");
            RenameColumn(table: "dbo.PayInvoices", name: "EditorUserId", newName: "LastEditedUserId");
            RenameColumn(table: "dbo.PayInvoices", name: "CreatorUserId", newName: "CreationUserId");
        }
    }
}
