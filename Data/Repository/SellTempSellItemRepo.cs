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
    public class SellTempSellItemRepo:Repository<SellTempSellItem>
    {
        public SellTempSellItemRepo(BookStoreDbContext bookStoreDbContext):base (bookStoreDbContext)
        {

        }

        public async Task<List<SellTempSellItem>> FindNoTrackinWithIncludesAsync(Expression<Func<SellTempSellItem, bool>> expression,
            bool includeInvoice = false, bool includeBookEdition = false, bool includeCustomer = false)
        {
            IQueryable<SellTempSellItem> sellItems;

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

            return await sellItems.Where(expression).AsNoTracking().ToListAsync();
        }
    }
}