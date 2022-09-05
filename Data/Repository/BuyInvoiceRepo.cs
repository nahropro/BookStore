using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.ItemInvoices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class BuyInvoiceRepo:Repository<BuyInvoice>
    {
        public BuyInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public BuyInvoice Add(CreateEditItemInvoiceViewModel model, string userId)
        {
            BuyInvoice invoice;

            //Map view model to model class
            invoice = model;

            //Add creator-user-id to the model
            invoice.CreatorUserId = userId;

            return Add(invoice);
        }

        public override BuyInvoice Add(BuyInvoice entity)
        {
            //Check if invoice at least has one item, and discount must be positive
            //And all item qtts greater than 0 and price must be positive
            if (entity.Items.Count > 0 &&
                entity.Discount.GetValueOrDefault() >= 0 &&
                entity.Items.All(i => i.Qtt > 0) &&
                entity.Items.All(i => i.Price >= 0))
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

        public async Task<List<BuyInvoice>> FilterNoTrackingWithIncludesAsync(ItemInvoiceFilter filter = null,
            bool includeCustomer = false, bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<BuyInvoice> invoice;

            invoice = GetQueryable();

            //If include customers
            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Customer);
            }

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
                invoice = invoice.Include(i => i.Items.Select(s => s.Store));
            }

            if (filter != null)
            {
                invoice = invoice.Where(i =>
                    (filter.InvoiceId.HasValue ? i.Id == filter.InvoiceId.Value :
                    (filter.CustomerId.HasValue ? i.CustomerId == filter.CustomerId.Value : true) &&
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    ));
            }

            return await invoice.AsNoTracking().ToListAsync();
        }

        public async Task<BuyInvoice> GetNoTrackingWithIncludesAsync(
            long id, bool includeCustomer = false, bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<BuyInvoice> invoice;

            invoice = GetQueryable();

            //If include customers
            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Customer);
            }

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
                invoice = invoice.Include(i => i.Items.Select(s => s.Store));
            }

            return await invoice.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<BuyInvoice> GetWithIncludesAsync(
            long id, bool includeCustomer = false, bool includeItems = false, bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<BuyInvoice> invoice;

            invoice = GetQueryable();

            //If include customers
            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Customer);
            }

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
                invoice = invoice.Include(i => i.Items.Select(s => s.Store));
            }

            return await invoice.SingleOrDefaultAsync(i => i.Id == id);
        }
    }
}