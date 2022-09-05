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
    public class StoreTransferItemRepo:Repository<StoreTransferItem>
    {
        public StoreTransferItemRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<List<StoreTransferItem>> FindNoTrackinWithIncludesAsync(Expression<Func<StoreTransferItem, bool>> expression,
            bool includeInvoice = false, bool includeStore = false, bool includeBookEdition = false)
        {
            IQueryable<StoreTransferItem> sellItems;

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
                sellItems = sellItems.Include(i => i.Invoice.ToStore)
                    .Include(i=> i.Invoice.FromStore);
            }

            return await sellItems.Where(expression).AsNoTracking().ToListAsync();
        }
    }
}