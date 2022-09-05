using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class StoreTransferInvoiceConfiguration : EntityTypeConfiguration<StoreTransferInvoice>
    {
        public StoreTransferInvoiceConfiguration()
        {
            //Relationship between invoice and from-store
            this.HasRequired<Store>(i => i.FromStore)
                .WithMany(i => i.StoreTransferInvoiceFroms)
                .HasForeignKey(i => i.FromStoreId)
                .WillCascadeOnDelete(false);

            //Relationship between invoice and to-store
            this.HasRequired<Store>(i => i.ToStore)
                .WithMany(i => i.StoreTransferInvoiceTos)
                .HasForeignKey(i => i.ToStoreId)
                .WillCascadeOnDelete(false);
        }
    }
}