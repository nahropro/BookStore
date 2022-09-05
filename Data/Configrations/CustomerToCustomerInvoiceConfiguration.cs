using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class CustomerToCustomerInvoiceConfiguration: EntityTypeConfiguration<CustomerToCustomerInvoice>
    {
        public CustomerToCustomerInvoiceConfiguration()
        {
            //Relationship between pay cusetomertocustomer and customer
            this.HasRequired<Customer>(i => i.PayCustomer)
                .WithMany(i => i.CustomerToCustomerInvoicePays)
                .HasForeignKey(i => i.PayCustomerId)
                .WillCascadeOnDelete(false);

            //Relationship between give cusetomertocustomer and customer
            this.HasRequired<Customer>(i => i.GiveCustomer)
                .WithMany(i => i.CustomerToCustomerInvoiceGives)
                .HasForeignKey(i => i.GiveCustomerId)
                .WillCascadeOnDelete(false);
        }
    }
}