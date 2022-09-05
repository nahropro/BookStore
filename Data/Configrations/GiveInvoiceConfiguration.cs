using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class GiveInvoiceConfiguration: EntityTypeConfiguration<GiveInvoice>
    {
        public GiveInvoiceConfiguration()
        {
            //Relashinship between giveinvoice and vault
            this.HasRequired<Vault>(gi => gi.Vault)
                .WithMany(v => v.GiveInvoices)
                .HasForeignKey(gi => gi.VaultId)
                .WillCascadeOnDelete(false);

            //Relashinship between giveinvoice and customer
            this.HasRequired<Customer>(gi => gi.Customer)
                .WithMany(c => c.GiveInvoices)
                .HasForeignKey(gi => gi.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}