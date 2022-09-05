using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.StoreReoports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class StoreReportsController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly BookEditionManager bookEditionManager;

        //View page addresses
        private const string STORE_REMAINS = "~/Views/StoreReports/Remains/StoreRemains.cshtml";
        private const string BOOKEDITION_DETAILS = "~/Views/StoreReports/BookEditionDetails/BookEditionDetails.cshtml";
        private const string DISCHARGE_REMAINS= "~/Views/StoreReports/DischargeRemains/DischargeRemains.cshtml";
        private const string DISCHARGE_BY_CUSTOMER_REMAINS = "~/Views/StoreReports/DischargeRemainsByCustomer/DischargeRemainsByCustomer.cshtml";


        public StoreReportsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            bookEditionManager = new BookEditionManager(bookStoreUnitOfWork);
        }

        public async Task<ActionResult> GetStoreRemains(StoreRemainsFilter filter = null)
        {
            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Send Stores to view by view-bag
            ViewBag.Stores = (await bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync()).Select(i => new KeyValuePairViewModel<long, string>
            {
                Key = i.Id,
                Value = i.Name,
            }).ToList();

            return View(STORE_REMAINS, await bookStoreUnitOfWork.BookEditions.GetRemainsAsync(filter));
        }

        public async Task<ActionResult> GetDischargeRemains(StoreRemainsFilter filter = null)
        {
            //Store filter object in view-bag
            ViewBag.Filter = filter;

            return View(DISCHARGE_REMAINS, await bookStoreUnitOfWork.BookEditions.GetDischargeRemainsAsync(filter));
        }

        public async Task<ActionResult> GetDischargeRemainsByCustomer(long? bookEditionId=null)
        {
            List<BookEditionDischargeRemainsByCustomerViewModel> result = new List<BookEditionDischargeRemainsByCustomerViewModel>();

            if (bookEditionId.HasValue)
            {
                result = await bookStoreUnitOfWork.Customers.GetDischargeBookEditionRemainsAsync(bookEditionId.Value);
            }

            //Set book-edition-id view-bag
            ViewBag.BookEditionId = new SelectList(
                (await bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludesAsync(includeBook: true))
                .Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Book.Name + " ( " + v.EditionInNumber + " - " + v.EditionInString + " )",
                }),
                "Key", "Value", bookEditionId);

            return View(DISCHARGE_BY_CUSTOMER_REMAINS, result);
        }

        public async Task<ActionResult> GetBookEditionDetails(BookEditionDetailsFilter filter = null)
        {
            List<BookEditionDetailsViewModel> details = new List<BookEditionDetailsViewModel>();

            //If filter is null, there is no querystring
            if (Request.QueryString.Count == 0)
            {
                //Set today date to start date
                //Set only date and time 00:00:00
                filter.StartDate = DateTimeManager.GetNowDateOnly();
            }

            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Set vault-id view-bag
            ViewBag.StoreId = new SelectList(
                (await bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync()).Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Name,
                }),
                "Key", "Value", filter.StoreId);

            //Set book-edition-id view-bag
            ViewBag.BookEditionId = new SelectList(
                (await bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludesAsync(includeBook: true))
                .Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Book.Name + " ( " + v.EditionInNumber + " - " + v.EditionInString + " )",
                }),
                "Key", "Value", filter.BookEditionId);

            //If filter has vault-id then get details
            if (filter.BookEditionId != 0)
            {
                details = await bookEditionManager.GetDetailsAsync(filter);
            }

            //Return view and vault balances data
            return View(BOOKEDITION_DETAILS, details);
        }
    }
}