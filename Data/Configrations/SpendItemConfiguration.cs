using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class SpendItemConfiguration : EntityTypeConfiguration<SpendItem>
    {
        public SpendItemConfiguration()
        {
            //Set relationship between invoice-item and invoice
            this.HasRequired<SpendInvoice>(i => i.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            //Set relationship between invoice-item and spend-type
            this.HasRequired<SpendType>(i => i.SpendType)
                .WithMany(i => i.SpendItems)
                .HasForeignKey(i => i.SpendTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}