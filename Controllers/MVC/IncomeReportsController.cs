using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC
{
    public class IncomeReportsController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private const string INDEX = "~/Views/SpendIncomeReports/IncomeReports/Index.cshtml";

        public IncomeReportsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: SpendReports
        public async Task<ActionResult> Index(SpendIncomeReportFilter filter)
        {
            if (Request.QueryString.Count == 0)
            {
                //If filter is not set, put default today to start and end date
                filter.StartDate = DateTime.Today;
                filter.EndDate = DateTime.Today;
            }

            ViewBag.Filter = filter;

            //Send Stores to view by view-bag
            ViewBag.TypeIds = (await bookStoreUnitOfWork.IncomeTypes.GetAllNoTrackingAsync()).Select(i => new KeyValuePairViewModel<long, string>
            {
                Key = i.Id,
                Value = i.Name,
            }).ToList();

            return View(INDEX, await bookStoreUnitOfWork.IncomeItems.GetIncomeReportAsync(filter));
        }
    }
}