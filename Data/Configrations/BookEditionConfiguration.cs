using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class BookEditionConfiguration:EntityTypeConfiguration<BookEdition>
    {
        public BookEditionConfiguration()
        {
            //Relashin ship between book and bookedition
            this.HasRequired<Book>(be => be.Book)
                .WithMany(b => b.BookEditions)
                .HasForeignKey(be => be.BookId)
                .WillCascadeOnDelete(false);
        }
    }
}