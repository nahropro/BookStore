using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class ReturnSellItemConfiguration : EntityTypeConfiguration<ReturnSellItem>
    {
        public ReturnSellItemConfiguration()
        {
            //Set relationship between invoice-item and invoice
            this.HasRequired<ReturnSellInvoice>(i => i.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            //Set relationship between invoice-item and book-edition
            this.HasRequired<BookEdition>(i => i.BookEdition)
                .WithMany(i => i.ReturnSellItems)
                .HasForeignKey(i => i.BookEditionId)
                .WillCascadeOnDelete(false);

            //Set relationship between invoice-item and store
            this.HasRequired<Store>(i => i.Store)
                .WithMany(i => i.ReturnSellItems)
                .HasForeignKey(i => i.StoreId)
                .WillCascadeOnDelete(false);
        }
    }
}