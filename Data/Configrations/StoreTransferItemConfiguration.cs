using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class StoreTransferItemConfiguration : EntityTypeConfiguration<StoreTransferItem>
    {
        public StoreTransferItemConfiguration()
        {
            //Relationship between item and invoice
            this.HasRequired<StoreTransferInvoice>(i => i.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            //Relationship between item and bookedition
            this.HasRequired<BookEdition>(i => i.BookEdition)
                .WithMany(i => i.StoreTransferItems)
                .HasForeignKey(i => i.BookEditionId)
                .WillCascadeOnDelete(false);
        }
    }
}