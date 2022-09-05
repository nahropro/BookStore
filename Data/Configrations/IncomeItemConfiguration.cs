using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class IncomeItemConfiguration : EntityTypeConfiguration<IncomeItem>
    {
        public IncomeItemConfiguration()
        {
            //Set relationship between invoice-item and invoice
            this.HasRequired<IncomeInvoice>(i => i.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.InvoiceId)
                .WillCascadeOnDelete(true);

            //Set relationship between invoice-item and spend-type
            this.HasRequired<IncomeType>(i => i.IncomeType)
                .WithMany(i => i.IncomeItems)
                .HasForeignKey(i => i.IncomeTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}