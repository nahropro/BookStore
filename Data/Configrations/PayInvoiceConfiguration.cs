using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class PayInvoiceConfiguration: EntityTypeConfiguration<PayInvoice>
    {
        public PayInvoiceConfiguration()
        {
            //Relationship between PayInvoice and vault
            this.HasRequired<Vault>(pi => pi.Vault)
                .WithMany(v => v.PayInvoices)
                .HasForeignKey(pi => pi.VaultId)
                .WillCascadeOnDelete(false);

            //Relationship between PayInvoice and customer
            this.HasRequired<Customer>(pi => pi.Customer)
                .WithMany(c => c.PayInvoices)
                .HasForeignKey(pi => pi.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}