namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeInfoPartialToCustomerToCustomerInvoice : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CustomerToCustomerInvoices", name: "CreationUserId", newName: "CreatorUserId");
            RenameColumn(table: "dbo.CustomerToCustomerInvoices", name: "LastEditedUserId", newName: "EditorUserId");
            RenameIndex(table: "dbo.CustomerToCustomerInvoices", name: "IX_CreationUserId", newName: "IX_CreatorUserId");
            RenameIndex(table: "dbo.CustomerToCustomerInvoices", name: "IX_LastEditedUserId", newName: "IX_EditorUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CustomerToCustomerInvoices", name: "IX_EditorUserId", newName: "IX_LastEditedUserId");
            RenameIndex(table: "dbo.CustomerToCustomerInvoices", name: "IX_CreatorUserId", newName: "IX_CreationUserId");
            RenameColumn(table: "dbo.CustomerToCustomerInvoices", name: "EditorUserId", newName: "LastEditedUserId");
            RenameColumn(table: "dbo.CustomerToCustomerInvoices", name: "CreatorUserId", newName: "CreationUserId");
        }
    }
}
