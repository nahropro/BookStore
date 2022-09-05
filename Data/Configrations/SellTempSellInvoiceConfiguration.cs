using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class SellTempSellInvoiceConfiguration : EntityTypeConfiguration<SellTempSellInvoice>
    {
        public SellTempSellInvoiceConfiguration()
        {
            this.HasRequired<Customer>(i => i.Customer)
                .WithMany(i => i.SellTempSellInvoices)
                .HasForeignKey(i => i.CustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}