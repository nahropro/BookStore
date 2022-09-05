using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class BookRepo:Repository<Book>
    {
        public BookRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {
            
        }

        public async Task<List<Book>> GetAllNoTrackingWithIncludesAsync(bool includeBookEdition = false)
        {
            IQueryable<Book> books;

            books = GetQueryable();

            if (includeBookEdition)
            {
                books = books.Include(b => b.BookEditions);
            }

            return await books.AsNoTracking().ToListAsync();
        }
    }
}