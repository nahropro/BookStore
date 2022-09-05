using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.CustomerReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class CustomerReportsController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly CustomerManager customerManager;

        //View page addresses
        private const string LOAN = "~/Views/CustomerReports/Loans/Loan.cshtml";
        private const string DISCHARGE_LOAN = "~/Views/CustomerReports/Loans/DischargeLoan.cshtml";
        private const string CUSTOMER_DETAILS = "~/Views/CustomerReports/CustomerDetails/CustomerDetails.cshtml";
        private const string CUSTOMER_DISCHARGE_DETAILS = "~/Views/CustomerReports/CustomerDetails/CustomerDischargeDetails.cshtml";

        public CustomerReportsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            customerManager = new CustomerManager(bookStoreUnitOfWork);
        }

        public async Task<ActionResult> GetLoans(LoanFilter filter)
        {
            //Store filter to viewbag
            ViewBag.Filter = filter;

            //Return filtered loan to view
            return View(LOAN, await bookStoreUnitOfWork.Customers.GetLoansAsync(filter));
        }

        public async Task<ActionResult> GetDischargeLoans(LoanFilter filter)
        {
            //Store filter to viewbag
            ViewBag.Filter = filter;

            //Return filtered loan to view
            return View(DISCHARGE_LOAN, await bookStoreUnitOfWork.Customers.GetDischargeLoansAsync(filter));
        }

        public async Task<ActionResult> GetCustomerDetails(CustomerDetailsFilter filter = null)
        {
            List<CustomerDetailsViewModel> details = new List<CustomerDetailsViewModel>();

            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Get all customers and make them for slectlist and map to keyvaluepair for the view
            ViewBag.CustomerId = new SelectList(
                (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.FullName + " (" + c.WorkPlace + ")",
                }),
                "Key", "Value", filter.CustomerId);

            //If filter has vault-id then get details
            if (filter.CustomerId != 0)
            {
                details = await customerManager.GetCustomerDetailsAsync(filter);
            }

            //Return view and vault balances data
            return View(CUSTOMER_DETAILS, details);
        }

        public async Task<ActionResult> GetCustomerDischargeDetails(CustomerDetailsFilter filter = null)
        {
            List<CustomerDetailsViewModel> details = new List<CustomerDetailsViewModel>();

            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Get all customers and make them for slectlist and map to keyvaluepair for the view
            ViewBag.CustomerId = new SelectList(
                (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.FullName + " (" + c.WorkPlace + ")",
                }),
                "Key", "Value", filter.CustomerId);

            //If filter has vault-id then get details
            if (filter.CustomerId != 0)
            {
                details = await customerManager.GetCustomerDischargeDetailsAsync(filter);
            }

            //Return view and vault balances data
            return View(CUSTOMER_DISCHARGE_DETAILS, details);
        }
    }
}