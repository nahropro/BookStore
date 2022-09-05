using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class BookEditionFirstTimeConfigurtion: EntityTypeConfiguration<BookEditionFirstTime>
    {
        public BookEditionFirstTimeConfigurtion()
        {
            //Set primary keys
            this.HasKey(i => new { i.BookEditionId, i.StoreId });

            //Set relationships between this and bookedition
            this.HasRequired<BookEdition>(i => i.BookEdition)
                .WithMany(i => i.FirstTimes)
                .HasForeignKey(i => i.BookEditionId)
                .WillCascadeOnDelete(false);

            //Set relationship with store
            this.HasRequired<Store>(i => i.Store)
                .WithMany(i => i.BookEditionFirstTimes)
                .HasForeignKey(i => i.StoreId)
                .WillCascadeOnDelete(false);
        }
    }
}