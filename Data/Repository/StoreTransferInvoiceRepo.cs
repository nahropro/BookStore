using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class StoreTransferInvoiceRepo:Repository<StoreTransferInvoice>
    {
        public StoreTransferInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public override StoreTransferInvoice Add(StoreTransferInvoice entity)
        {
            //Check if invoice at least has one item and qtt is greater than 0
            //And from-store and to store must not be the same
            if (entity.Items.Count > 0 &&
                entity.Items.All(i=> i.Qtt>0) &&
                entity.FromStoreId!=entity.ToStoreId)
            {
                //Get creation datetime
                entity.CreationDateTime = DateTime.UtcNow;

                //Prevent add unnecessary datas
                entity.LastEditedDateTime = null;
                entity.EditorUserId = null;

                return base.Add(entity);
            }

            //Throw exception with incorrect data
            throw new Exception("Incorrect data");
        }

        public async Task<List<StoreTransferInvoice>> FilterNoTrackingWithIncludesAsync(StoreTransferInvoiceFilter filter = null,
            bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<StoreTransferInvoice> invoice;

            invoice = GetQueryable();

            //Includes
            if (includeItems)
            {
                invoice = invoice.Include(i => i.Items);
            }

            if (includeBookEdition)
            {
                invoice = invoice.Include(i => i.Items.Select(s => s.BookEdition))
                    .Include(i => i.Items.Select(s => s.BookEdition.Book));
            }

            if (includeStore)
            {
                invoice = invoice.Include(i => i.FromStore).Include(i=> i.ToStore);
            }

            if (filter != null)
            {
                invoice = invoice.Where(i =>
                    (filter.InvoiceId.HasValue ? i.Id == filter.InvoiceId.Value :
                    (filter.FromStoreId.HasValue ? i.FromStoreId == filter.FromStoreId.Value : true) &&
                    (filter.ToStoreId.HasValue ? i.ToStoreId == filter.ToStoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    ));
            }

            return await invoice.AsNoTracking().ToListAsync();
        }

        public async Task<StoreTransferInvoice> GetNoTrackingWithIncludesAsync(
            long id, bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<StoreTransferInvoice> invoice;

            invoice = GetQueryable();

            //Includes
            if (includeItems)
            {
                invoice = invoice.Include(i => i.Items);
            }

            if (includeBookEdition)
            {
                invoice = invoice.Include(i => i.Items.Select(s => s.BookEdition))
                    .Include(i => i.Items.Select(s => s.BookEdition.Book));
            }

            if (includeStore)
            {
                invoice = invoice.Include(i => i.FromStore).Include(i=> i.ToStore);
            }

            return await invoice.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<StoreTransferInvoice> GetWithIncludesAsync(
            long id, bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<StoreTransferInvoice> invoice;

            invoice = GetQueryable();

            //Includes
            if (includeItems)
            {
                invoice = invoice.Include(i => i.Items);
            }

            if (includeBookEdition)
            {
                invoice = invoice.Include(i => i.Items.Select(s => s.BookEdition))
                    .Include(i => i.Items.Select(s => s.BookEdition.Book));
            }

            if (includeStore)
            {
                invoice = invoice.Include(i => i.FromStore).Include(i=> i.ToStore);
            }

            return await invoice.SingleOrDefaultAsync(i => i.Id == id);
        }
    }
}