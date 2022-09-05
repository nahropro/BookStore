using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class IncomeInvoiceConfiguration : EntityTypeConfiguration<IncomeInvoice>
    {
        public IncomeInvoiceConfiguration()
        {
            this.HasOptional<Customer>(i => i.Customer)
                .WithMany(i => i.IncomeInvoices)
                .HasForeignKey(i => i.CustomerId)
                .WillCascadeOnDelete(false);

            this.HasOptional<Vault>(i => i.Vault)
                .WithMany(i => i.IncomeInvoices)
                .HasForeignKey(i => i.VaultId)
                .WillCascadeOnDelete(false);
        }
    }
}