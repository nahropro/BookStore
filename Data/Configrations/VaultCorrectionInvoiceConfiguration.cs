using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class VaultCorrectionInvoiceConfiguration : EntityTypeConfiguration<VaultCorrectionInvoice>
    {
        public VaultCorrectionInvoiceConfiguration()
        {
            this.HasRequired<Vault>(i => i.Vault)
                .WithMany(i => i.VaultCorrectionInvoices)
                .HasForeignKey(i => i.VaultId)
                .WillCascadeOnDelete(false);
        }
    }
}