using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class LostItemRepo:Repository<LostItem>
    {
        public LostItemRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<List<LostItem>> FindNoTrackinWithIncludesAsync(Expression<Func<LostItem, bool>> expression,
            bool includeInvoice = false, bool includeStore = false, bool includeBookEdition = false)
        {
            IQueryable<LostItem> sellItems;

            sellItems = GetQueryable();

            //Set includes
            if (includeInvoice)
            {
                sellItems = sellItems.Include(i => i.Invoice);
            }

            if (includeBookEdition)
            {
                sellItems = sellItems.Include(i => i.BookEdition);
            }

            if (includeStore)
            {
                sellItems = sellItems.Include(i => i.Store);
            }

            return await sellItems.Where(expression).AsNoTracking().ToListAsync();
        }
    }
}