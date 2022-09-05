using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class SellInvoiceConfiguration: EntityTypeConfiguration<SellInvoice>
    {
        public SellInvoiceConfiguration()
        {
            this.HasRequired<Customer>(i => i.Customer)
                .WithMany(i => i.SellInvoices)
                .HasForeignKey(i => i.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}