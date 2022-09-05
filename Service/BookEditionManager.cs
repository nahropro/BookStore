using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.StoreReoports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class BookEditionManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public BookEditionManager(BookStoreUnitOfWork bookStoreUnitOfWork)
        {
            if (bookStoreUnitOfWork == null)
            {
                this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
            }
            else
            {
                this.bookStoreUnitOfWork = bookStoreUnitOfWork;
            }
        }

        public BookEditionManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<BookEditionDetailsViewModel>> GetDetailsAsync (BookEditionDetailsFilter filter)
        {
            List<BookEditionDetailsViewModel> details = new List<BookEditionDetailsViewModel>();
            long remainsInTime = 0;

            //Adding first time
            if (filter.StartDate.HasValue)
            {
                //If there is start time, get first time until one day before that start date
                details.Add(new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = filter.BookEditionId,
                    },
                    In =await bookStoreUnitOfWork.BookEditions.GetRemainsAsync(
                        filter.BookEditionId, filter.StoreId, filter.StartDate.Value.AddDays(-1)),
                    Type=BookEditionDetailsViewModel.InvoiceType.First_Time,
                });
            }
            else
            {
                //If no start date add first times than has that soter id in the filter,
                //If no store id in the filter return first time for all stores
                details.AddRange((await bookStoreUnitOfWork.BookEditionFirstTimes.FindNoTrackinWithIncludesAsync(i =>
                        i.BookEditionId == filter.BookEditionId &&
                        (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true)
                    , includeStore: true)).Select(i => new BookEditionDetailsViewModel
                    {
                        BookEdition = new BookEdition
                        {
                            Id = i.BookEditionId,
                        },
                        In = i.Qtt,
                        Price = i.Price,
                        Store = i.Store,
                        Type = BookEditionDetailsViewModel.InvoiceType.First_Time,
                    }).ToList());
            }

            //Add buy invoices
            details.AddRange((await bookStoreUnitOfWork.BuyItems.FindNoTrackinWithIncludesAsync(i=>
                    i.BookEditionId==filter.BookEditionId &&
                    (filter.StoreId.HasValue? i.StoreId==filter.StoreId.Value:true) &&
                    (filter.StartDate.HasValue? i.Invoice.InvoiceDate>=filter.StartDate.Value:true) && 
                    (filter.EndDate.HasValue? i.Invoice.InvoiceDate<=filter.EndDate.Value:true)
                ,includeCustomer:true, includeInvoice:true, includeStore:true)).Select(i=> new BookEditionDetailsViewModel{
                    BookEdition=new BookEdition
                    {
                        Id=i.BookEditionId,
                    },
                    Customer=i.Invoice.Customer,
                    Store=i.Store,
                    In=i.Qtt,
                    InvoiceDate=i.Invoice.InvoiceDate,
                    InvoiceId=i.InvoiceId,
                    Price=i.Price,
                    Type=BookEditionDetailsViewModel.InvoiceType.Buy_Invoice,
                })
            );

            //Add return-sell invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnSellItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeCustomer: true, includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Customer = i.Invoice.Customer,
                    Store = i.Store,
                    In = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Return_Sell_Invoice,
                })
            );

            //Add return-temp-sell invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnTempSellItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeCustomer: true, includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Customer = i.Invoice.Customer,
                    Store = i.Store,
                    In = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Return_Discharge_Sell_Invoice,
                })
            );

            //Add store-transfer-to invoices
            details.AddRange((await bookStoreUnitOfWork.StoreTransferItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.Invoice.ToStoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Store = i.Invoice.ToStore,
                    In = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Type = BookEditionDetailsViewModel.InvoiceType.Store_Transfer_Invoice,
                })
            );

            //Add sell invoices
            details.AddRange((await bookStoreUnitOfWork.SellItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeCustomer: true, includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Customer = i.Invoice.Customer,
                    Store = i.Store,
                    Out = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Sell_Invoice,
                })
            );

            //Add return-buy invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnBuyItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeCustomer: true, includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Customer = i.Invoice.Customer,
                    Store = i.Store,
                    Out = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Return_Buy_Invoice,
                })
            );

            //Add temp-sell invoices
            details.AddRange((await bookStoreUnitOfWork.TempSellItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeCustomer: true, includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Customer = i.Invoice.Customer,
                    Store = i.Store,
                    Out = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Discharge_Sell_Invoice,
                })
            );

            //Add lost invoices
            details.AddRange((await bookStoreUnitOfWork.LostItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Store = i.Store,
                    Out = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Price = i.Price,
                    Type = BookEditionDetailsViewModel.InvoiceType.Lost_Invoice,
                })
            );

            //Add store-transfer-From invoices
            details.AddRange((await bookStoreUnitOfWork.StoreTransferItems.FindNoTrackinWithIncludesAsync(i =>
                    i.BookEditionId == filter.BookEditionId &&
                    (filter.StoreId.HasValue ? i.Invoice.FromStoreId == filter.StoreId.Value : true) &&
                    (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true)
                , includeInvoice: true, includeStore: true)).Select(i => new BookEditionDetailsViewModel
                {
                    BookEdition = new BookEdition
                    {
                        Id = i.BookEditionId,
                    },
                    Store = i.Invoice.FromStore,
                    Out = i.Qtt,
                    InvoiceDate = i.Invoice.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Type = BookEditionDetailsViewModel.InvoiceType.Store_Transfer_Invoice,
                })
            );


            //Sort accending by invoice-date
            details = details.OrderBy(i => i.InvoiceDate).ToList();

            //Calculate in time remains
            foreach (var detail in details)
            {
                //Calculate balance-in-time
                remainsInTime = remainsInTime + detail.In - detail.Out;

                //Set balance-in-time to detail object
                detail.RemainsInTime = remainsInTime;
            }

            return details;
        }
    }
}