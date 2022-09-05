using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.ItemInvoices;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.ItemInvoices
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class TempSellInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly TempSellInvoiceManager tempSellInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/ItemInvoices/TempSellInvoices/Index.cshtml";
        private const string CREATE = "~/Views/ItemInvoices/TempSellInvoices/Create.cshtml";
        private const string EDIT = "~/Views/ItemInvoices/TempSellInvoices/Edit.cshtml";
        private const string INVOICE = "~/Views/ItemInvoices/TempSellInvoices/Invoice.cshtml";

        public TempSellInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            tempSellInvoiceManager = new TempSellInvoiceManager(bookStoreUnitOfWork);
        }


        // GET: SellInvoices
        public async Task<ActionResult> Index(ItemInvoiceFilter filter = null)
        {
            //If filter is null, there is no querystring
            if (Request.QueryString.Count == 0)
            {
                //Set today date to start and end date
                //Set only date and time 00:00:00
                filter.StartDate = DateTimeManager.GetNowDateOnly();
                filter.EndDate = DateTimeManager.GetNowDateOnly();
            }

            //Return the filter object throw ViewBag
            ViewBag.Filter = filter;

            //Create selectlists for filter
            await CreateSelectListsAsync(customerId: filter.CustomerId);

            //Return index view with filterd data
            return View(INDEX,
                (await tempSellInvoiceManager.GetAllNoTrackingAsync(filter))
                .OrderByDescending(gi => gi.Id).ToList());
        }

        // GET: SellInvoices/Details/5
        public async Task<ActionResult> Invoice(long id)
        {
            SelectItemInvoiceViewModel model;

            //Return invoice and map it to view model
            model = await bookStoreUnitOfWork.TempSellInvoices
                .GetNoTrackingWithIncludesAsync(id, includeCustomer: true, includeItems: true, includeBookEdition: true, includeStore: true);

            return View(INVOICE, model);
        }

        // GET: SellInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create nessessary selectlists
            await CreateSelectListsAsync();

            //Return view and some default values
            return View(CREATE, new CreateEditItemInvoiceViewModel
            {
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: SellInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditItemInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    bookStoreUnitOfWork.TempSellInvoices.Add(model, User.Identity.GetUserId());

                    await bookStoreUnitOfWork.CompleteAsync();

                    //If return url redirect to it,
                    //If not redirect to index
                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {

            }

            //If not succeded
            //Create select lists
            await CreateSelectListsAsync(customerId: model.CustomerId);

            //Return view with the model
            return View(CREATE, model);
        }

        // GET: SellInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            CreateEditItemInvoiceViewModel model;

            //Get the invoice and map it to modelview
            model = await bookStoreUnitOfWork.TempSellInvoices.GetNoTrackingWithIncludesAsync(id, includeItems: true);

            //Create nessessary selectlists
            await CreateSelectListsAsync(customerId: model.CustomerId);

            return View(EDIT, model);
        }

        // POST: SellInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditItemInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    await tempSellInvoiceManager.Edit(model, User.Identity.GetUserId());

                    await bookStoreUnitOfWork.CompleteAsync();

                    //If return url redirect to it,
                    //If not redirect to index
                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
            }

            //If not succeded
            //Create select lists
            await CreateSelectListsAsync(customerId: model.CustomerId);

            //Return view with the model
            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? customerId = null)
        {
            //Get all customers and make them for slectlist and map to keyvaluepair for the view
            ViewBag.CustomerId = new SelectList(
                (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.FullName + " (" + c.WorkPlace + ")",
                }),
                "Key", "Value", customerId);
        }

        #endregion
    }
}