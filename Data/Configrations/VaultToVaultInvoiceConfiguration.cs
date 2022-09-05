using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class VaultToVaultInvoiceConfiguration : EntityTypeConfiguration<VaultToVaultInvoice>
    {
        public VaultToVaultInvoiceConfiguration()
        {
            //Relationship between vaulttovaultinvoice and pay customer
            this.HasRequired<Vault>(i => i.PayVault)
                .WithMany(i => i.VaultToVaultInvoicePays)
                .HasForeignKey(i => i.PayVaultId)
                .WillCascadeOnDelete(false);

            //Relationship between vaulttovaultinvoice and give customer
            this.HasRequired<Vault>(i => i.GiveVault)
                .WithMany(i => i.VaultToVaultInvoiceGives)
                .HasForeignKey(i => i.GiveVaultId)
                .WillCascadeOnDelete(false);
        }
    }
}