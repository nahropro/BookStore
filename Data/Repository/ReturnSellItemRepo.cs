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
    public class ReturnSellItemRepo:Repository<ReturnSellItem>
    {
        public ReturnSellItemRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<List<ReturnSellItem>> FindNoTrackinWithIncludesAsync(Expression<Func<ReturnSellItem, bool>> expression,
            bool includeInvoice = false, bool includeStore = false, bool includeBookEdition = false,bool includeCustomer=false)
        {
            IQueryable<ReturnSellItem> sellItems;

            sellItems = GetQueryable();

            //Set includes
            if (includeInvoice)
            {
                sellItems = sellItems.Include(i => i.Invoice);
            }

            if (includeCustomer)
            {
                sellItems = sellItems.Include(i => i.Invoice.Customer);
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