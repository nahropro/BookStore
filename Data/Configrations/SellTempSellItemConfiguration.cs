using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class SellTempSellItemConfiguration : EntityTypeConfiguration<SellTempSellItem>
    {
        public SellTempSellItemConfiguration()
        {
            //Set relationship between invoice-item and invoice
            this.HasRequired<SellTempSellInvoice>(i => i.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            //Set relationship between invoice-item and book-edition
            this.HasRequired<BookEdition>(i => i.BookEdition)
                .WithMany(i => i.SellTempSellItems)
                .HasForeignKey(i => i.BookEditionId)
                .WillCascadeOnDelete(false);
        }
    }
}