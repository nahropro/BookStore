namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeInfoPartialToVaultToVaultInvoice : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.VaultToVaultInvoices", name: "CreationUserId", newName: "CreatorUserId");
            RenameColumn(table: "dbo.VaultToVaultInvoices", name: "LastEditedUserId", newName: "EditorUserId");
            RenameIndex(table: "dbo.VaultToVaultInvoices", name: "IX_CreationUserId", newName: "IX_CreatorUserId");
            RenameIndex(table: "dbo.VaultToVaultInvoices", name: "IX_LastEditedUserId", newName: "IX_EditorUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.VaultToVaultInvoices", name: "IX_EditorUserId", newName: "IX_LastEditedUserId");
            RenameIndex(table: "dbo.VaultToVaultInvoices", name: "IX_CreatorUserId", newName: "IX_CreationUserId");
            RenameColumn(table: "dbo.VaultToVaultInvoices", name: "EditorUserId", newName: "LastEditedUserId");
            RenameColumn(table: "dbo.VaultToVaultInvoices", name: "CreatorUserId", newName: "CreationUserId");
        }
    }
}
