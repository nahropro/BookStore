using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.Book;
using BookStoreModel.ViewModels.BookEdition;
using BookStoreModel.ViewModels.StoreReoports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class BookEditionRepo:Repository<BookEdition>
    {
        public BookEditionRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<BookEdition> SingleOrDefaultNoTrackingWithIncludesAsync(
            Expression<Func<BookEdition, bool>> expression,bool includeBook = false)
        {
            IQueryable<BookEdition> bookEditions;

            bookEditions = GetQueryable();

            if (includeBook)
            {
                bookEditions = bookEditions.Include(be => be.Book);
            }

            return await bookEditions.AsNoTracking().SingleOrDefaultAsync(expression);
        }

        public async Task<List<BookEdition>> GetAllNoTrackingWithIncludesAsync(bool includeBook = false)
        {
            IQueryable<BookEdition> bookEditions;

            //Get queryable of book-editions
            bookEditions = GetQueryable();

            if (includeBook)
            {
                bookEditions = bookEditions.Include(i => i.Book);
            }

            return await bookEditions.ToListAsync();
        }

        public List<BookEdition> GetAllNoTrackingWithIncludes(bool includeBook = false)
        {
            IQueryable<BookEdition> bookEditions;

            //Get queryable of book-editions
            bookEditions = GetQueryable();

            if (includeBook)
            {
                bookEditions = bookEditions.Include(i => i.Book);
            }

            return bookEditions.ToList();
        }

        public async Task<List<BookEditionRemainsViewModel>> GetRemainsAsync(StoreRemainsFilter filter = null)
        {
            IQueryable<BookEdition> bookEditions;
            IQueryable<BookEditionRemainsViewModel> result;

            //Get queryable boook-edition
            bookEditions = GetQueryable();

            //Set includes
            bookEditions = bookEditions.Include(i => i.FirstTimes)
                .Include(i => i.SellItems)
                .Include(i => i.SellItems.Select(s=> s.Invoice))
                .Include(i => i.ReturnSellItems)
                .Include(i => i.ReturnSellItems.Select(s => s.Invoice))
                .Include(i => i.BuyItems)
                .Include(i => i.BuyItems.Select(s => s.Invoice))
                .Include(i => i.ReturnBuyItems)
                .Include(i => i.ReturnBuyItems.Select(s => s.Invoice))
                .Include(i => i.StoreTransferItems)
                .Include(i => i.StoreTransferItems.Select(s => s.Invoice))
                .Include(i => i.LostItems)
                .Include(i => i.LostItems.Select(s => s.Invoice))
                .Include(i => i.TempSellItems)
                .Include(i => i.TempSellItems.Select(s => s.Invoice))
                .Include(i => i.ReturnTempSellItems)
                .Include(i => i.ReturnTempSellItems.Select(s => s.Invoice))
                .Include(i => i.Book);

            //Map to view model and calculate
            result = bookEditions.Select(i => new BookEditionRemainsViewModel
            {
                BookEdition = new SelectBookEditionViewModel
                {
                    Id=i.Id,
                    EditionInNumber = i.EditionInNumber,
                    EditionInString = i.EditionInString,
                    Book = new SelectBookViewModel
                    {
                        Id = i.BookId,
                        Name = i.Book.Name,
                    },
                },
                In = i.BuyItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                          (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                        (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnTempSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                        (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.FirstTimes.Where(s => (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    (filter.IncludeSotoreFilter ? i.StoreTransferItems.Where(s => filter.StoreIds.Contains(s.Invoice.ToStoreId))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum() : 0),
                Out = i.SellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                          (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnBuyItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                        (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.TempSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                        (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.LostItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true) &&
                        (filter.IncludeSotoreFilter ? filter.StoreIds.Contains(s.StoreId) : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    (filter.IncludeSotoreFilter ? i.StoreTransferItems.Where(s => filter.StoreIds.Contains(s.Invoice.FromStoreId))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum() : 0),
            });

            //Filtering
            result = result.Where(i =>
                (filter.ZeroRemains?(i.In-i.Out)==0:false) ||
                (filter.PositiveRemains ? (i.In - i.Out) > 0 : false) ||
                (filter.NegativeRemains ? (i.In - i.Out) < 0 : false)
            );

            //Sorting
            if (!filter.SortByDesc)
            {
                //If sort is acesnding
                if (filter.SortBy==StoreRemainsFilter.SortByEnum.Book)
                {
                    result = result.OrderBy(i => i.BookEdition.Book.Name);
                }
                else if (filter.SortBy==StoreRemainsFilter.SortByEnum.Remains)
                {
                    result = result.OrderBy(i => (i.In-i.Out));
                }
            }
            else
            {
                //If sort is decending
                if (filter.SortBy == StoreRemainsFilter.SortByEnum.Book)
                {
                    result = result.OrderByDescending(i => i.BookEdition.Book.Name);
                }
                else if (filter.SortBy == StoreRemainsFilter.SortByEnum.Remains)
                {
                    result = result.OrderByDescending(i => (i.In - i.Out));
                }
            }

            //Return list of viewmodel result
            return await result.AsNoTracking().ToListAsync();
        }

        public async Task<List<BookEditionRemainsViewModel>> GetDischargeRemainsAsync(StoreRemainsFilter filter = null)
        {
            IQueryable<BookEdition> bookEditions;
            IQueryable<BookEditionRemainsViewModel> result;

            //Get queryable boook-edition
            bookEditions = GetQueryable();

            //Set includes
            bookEditions = bookEditions
                .Include(i => i.SellTempSellItems)
                .Include(i => i.SellTempSellItems.Select(s => s.Invoice))
                .Include(i => i.TempSellItems)
                .Include(i => i.TempSellItems.Select(s => s.Invoice))
                .Include(i => i.ReturnTempSellItems)
                .Include(i => i.ReturnTempSellItems.Select(s => s.Invoice))
                .Include(i => i.Book);

            //Map to view model and calculate
            result = bookEditions.Select(i => new BookEditionRemainsViewModel
            {
                BookEdition = new SelectBookEditionViewModel
                {
                    Id = i.Id,
                    EditionInNumber = i.EditionInNumber,
                    EditionInString = i.EditionInString,
                    Book = new SelectBookViewModel
                    {
                        Id = i.BookId,
                        Name = i.Book.Name,
                    },
                },
                In = i.TempSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum(),
                Out = i.SellTempSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnTempSellItems.Where(s => (filter.UntilDate.HasValue ? s.Invoice.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum(),
            });

            //Filtering
            result = result.Where(i =>
                (filter.ZeroRemains ? (i.In - i.Out) == 0 : false) ||
                (filter.PositiveRemains ? (i.In - i.Out) > 0 : false) ||
                (filter.NegativeRemains ? (i.In - i.Out) < 0 : false)
            );

            //Sorting
            if (!filter.SortByDesc)
            {
                //If sort is acesnding
                if (filter.SortBy == StoreRemainsFilter.SortByEnum.Book)
                {
                    result = result.OrderBy(i => i.BookEdition.Book.Name);
                }
                else if (filter.SortBy == StoreRemainsFilter.SortByEnum.Remains)
                {
                    result = result.OrderBy(i => (i.In - i.Out));
                }
            }
            else
            {
                //If sort is decending
                if (filter.SortBy == StoreRemainsFilter.SortByEnum.Book)
                {
                    result = result.OrderByDescending(i => i.BookEdition.Book.Name);
                }
                else if (filter.SortBy == StoreRemainsFilter.SortByEnum.Remains)
                {
                    result = result.OrderByDescending(i => (i.In - i.Out));
                }
            }

            //Return list of viewmodel result
            return await result.AsNoTracking().ToListAsync();
        }

        public async Task<long> GetRemainsAsync(long bookEditionId, long? storeId = null, DateTime? UntilDate=null)
        {
            IQueryable<BookEdition> bookEditions;
            long remains = 0;

            //Get queryable book-edition
            bookEditions = GetQueryable();

            //Setup includes
            bookEditions = bookEditions.Include(i => i.FirstTimes)
                .Include(i => i.SellItems).Include(i => i.SellItems.Select(s => s.Invoice))
                .Include(i => i.ReturnSellItems).Include(i => i.ReturnSellItems.Select(s => s.Invoice))
                .Include(i => i.BuyItems).Include(i => i.BuyItems.Select(s => s.Invoice))
                .Include(i => i.ReturnBuyItems).Include(i => i.ReturnBuyItems.Select(s => s.Invoice))
                .Include(i => i.TempSellItems).Include(i => i.TempSellItems.Select(s => s.Invoice))
                .Include(i => i.ReturnTempSellItems).Include(i => i.ReturnTempSellItems.Select(s => s.Invoice))
                .Include(i => i.LostItems).Include(i => i.LostItems.Select(s => s.Invoice))
                .Include(i => i.StoreTransferItems).Include(i => i.StoreTransferItems.Select(s => s.Invoice));

            //Caculate the result object, contain Id, In and Out
            var result =await bookEditions.Select(i => new
            {
                Id = i.Id,
                In = i.FirstTimes.Where(s => (storeId.HasValue ? s.StoreId == storeId.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.BuyItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnSellItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnTempSellItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    (storeId.HasValue ? i.StoreTransferItems.Where(s =>
                         s.Invoice.ToStoreId == storeId.Value &&
                         (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() : 0),
                Out = i.SellItems.Where(s =>
                          (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                          (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.ReturnBuyItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.TempSellItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    i.LostItems.Where(s =>
                        (storeId.HasValue ? s.StoreId == storeId.Value : true) &&
                        (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                    (storeId.HasValue ? i.StoreTransferItems.Where(s =>
                         s.Invoice.FromStoreId == storeId.Value &&
                         (UntilDate.HasValue ? s.Invoice.InvoiceDate <= UntilDate.Value : true)).Select(s => s.Qtt).DefaultIfEmpty(0).Sum() : 0),
            }).AsNoTracking().SingleOrDefaultAsync(i=> i.Id==bookEditionId);

            //Calculate Remains
            remains = result.In - result.Out;

            return remains;
        }
    }
}