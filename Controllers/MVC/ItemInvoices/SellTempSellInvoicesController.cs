using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.ItemInvoices;
using BookStoreModel.ViewModels.ItemInvoices.SellTempSell;
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
    public class SellTempSellInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly SellTempSellInvoiceManager sellTempSellInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/ItemInvoices/SellTempSellInvoices/Index.cshtml";
        private const string ROW_PARTIAL = "~/Views/ItemInvoices/SellTempSellInvoices/_ItemRow.cshtml";
        private const string CREATE = "~/Views/ItemInvoices/SellTempSellInvoices/Create.cshtml";
        private const string EDIT = "~/Views/ItemInvoices/SellTempSellInvoices/Edit.cshtml";
        private const string INVOICE = "~/Views/ItemInvoices/SellTempSellInvoices/Invoice.cshtml";

        public SellTempSellInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            sellTempSellInvoiceManager = new SellTempSellInvoiceManager(bookStoreUnitOfWork);
        }


        // GET: SellInvoices
        public async Task<ActionResult> Index(ItemInvoiceFilter filter = null)
        {
            List<SelectItemInvoiceViewModel> selectItemInvoices;

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

            //Get filtered data and mapp it to select-item-view-moodel
            selectItemInvoices = (await sellTempSellInvoiceManager.GetAllNoTrackingAsync(filter))
                .OrderByDescending(gi => gi.Id).Select(i=> new SelectItemInvoiceViewModel
                {
                    CustomerFullName=i.CustomerFullName,
                    CustomerId=i.CustomerId,
                    CustomerWorkPlace=i.CustomerWorkPlace,
                    Discount=i.Discount,
                    Id=i.Id,
                    InvoiceDate=i.InvoiceDate,
                    Total=i.Total,
                    MoreInfo=i.MoreInfo,
                }).ToList();

            //Return index view with filterd data
            return View(INDEX, selectItemInvoices);
        }

        public ActionResult GetRowPartial(CreateEditSellTempSellItemViewModel row)
        {
            //Create book-edtion select list
            CreateBookEditionSelectList(bookEdtionId: row.BookEditionId);

            return PartialView(ROW_PARTIAL, row);

        }

        // GET: SellInvoices/Details/5
        public async Task<ActionResult> Invoice(long id)
        {
            SelectSellTempSellInvoiceViewModel model;

            //Return invoice and map it to view model
            model = await bookStoreUnitOfWork.SellTempSellInvoices
                .GetNoTrackingWithIncludesAsync(id, includeCustomer: true, includeItems: true, includeBookEdition: true);

            return View(INVOICE, model);
        }

        // GET: SellInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create nessessary selectlists
            await CreateSelectListsAsync();

            //Return view and some default values
            return View(CREATE, new CreateEditSellTempSellInvoiceViewModel
            {
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: SellInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditSellTempSellInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    bookStoreUnitOfWork.SellTempSellInvoices.Add(model, User.Identity.GetUserId());

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
            CreateEditSellTempSellInvoiceViewModel model;

            //Get the invoice and map it to modelview
            model = await bookStoreUnitOfWork.SellTempSellInvoices.GetNoTrackingWithIncludesAsync(id, includeItems: true);

            //Create nessessary selectlists
            await CreateSelectListsAsync(customerId: model.CustomerId);

            return View(EDIT, model);
        }

        // POST: SellInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditSellTempSellInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    await sellTempSellInvoiceManager.Edit(model, User.Identity.GetUserId());

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

        private void CreateBookEditionSelectList(long? bookEdtionId)
        {
            //Get all book-editions and make them for slectlist and map to keyvaluepair for the view
            ViewBag.BookEditions = new SelectList(
                bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludes(includeBook: true)
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Book.Name + " (" + c.EditionInNumber + " - " + c.EditionInString + ")",
                }),
                "Key", "Value", bookEdtionId);
        }

        #endregion
    }
}