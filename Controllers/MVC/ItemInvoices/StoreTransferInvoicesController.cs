using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Models;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.ItemInvoices.StoreTransfer;
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
    public class StoreTransferInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly StoreTransferInvoiceManager storeTransferInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/ItemInvoices/StoreTransferInvoices/Index.cshtml";
        private const string ROW_PARTIAL = "~/Views/ItemInvoices/StoreTransferInvoices/_ItemRow.cshtml";
        private const string CREATE = "~/Views/ItemInvoices/StoreTransferInvoices/Create.cshtml";
        private const string EDIT = "~/Views/ItemInvoices/StoreTransferInvoices/Edit.cshtml";
        private const string INVOICE = "~/Views/ItemInvoices/StoreTransferInvoices/Invoice.cshtml";

        public StoreTransferInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            storeTransferInvoiceManager = new StoreTransferInvoiceManager(bookStoreUnitOfWork);
        }

        // GET: StoreTransferInvoices
        public async Task<ActionResult> Index(StoreTransferInvoiceFilter filter = null)
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
            await CreateSelectListsAsync(fromStoreId: filter.FromStoreId, toStoreId:filter.ToStoreId);

            //Return index view with filterd data
            return View(INDEX,
                (await storeTransferInvoiceManager.GetAllNoTrackingAsync(filter))
                .OrderByDescending(gi => gi.Id).ToList());
        }

        public ActionResult GetRowPartial(CreateEditStoreTransferItemViewModel row)
        {
            //Create book-edtion select list
            CreateBookEditionSelectList(bookEdtionId: row.BookEditionId);

            return PartialView(ROW_PARTIAL,row);

        }

        // GET: StoreTransferInvoices/Details/5
        public async Task<ActionResult> Invoice(long id)
        {
            SelectStoreTransferInvoiceViewModel model;

            //Return invoice and map it to view model
            model = await bookStoreUnitOfWork.StoreTransferInvoices
                .GetNoTrackingWithIncludesAsync(id, includeItems: true, includeBookEdition: true, includeStore: true);

            return View(INVOICE, model);
        }

        // GET: StoreTransferInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create select lists
            await CreateSelectListsAsync();

            //Return view with some default values
            return View(CREATE,new CreateEditStoreTransferInvoiceViewModel
            {
                InvoiceDate= DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: StoreTransferInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditStoreTransferInvoiceViewModel model,string returnUrl)
        {
            StoreTransferInvoice invoice;

            try
            {
                //If model state is valid and from-store and to-store is not the same and
                //the invlice at least has one item and qtt of of all items must be greater than zero
                if (ModelState.IsValid && model.ToStoreId!=model.FromStoreId && model.Items.Count>0 &&
                    model.Items.All(i=> i.Qtt>0))
                {
                    //Map view model to model class invoice
                    invoice = model;

                    //Set user-id to model
                    invoice.CreatorUserId = User.Identity.GetUserId();

                    bookStoreUnitOfWork.StoreTransferInvoices.Add(invoice);

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
            catch(Exception ex)
            {
                
            }

            //IF not success
            //Create select lists
            await CreateSelectListsAsync();

            return View(CREATE,model);
        }

        // GET: StoreTransferInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            CreateEditStoreTransferInvoiceViewModel model;

            //Get the invoice and map it to modelview
            model = await bookStoreUnitOfWork.StoreTransferInvoices.GetNoTrackingWithIncludesAsync(id, includeItems: true);

            //Create nessessary selectlists
            await CreateSelectListsAsync(fromStoreId: model.FromStoreId,toStoreId:model.ToStoreId);

            return View(EDIT, model);
        }

        // POST: StoreTransferInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditStoreTransferInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    await storeTransferInvoiceManager.Edit(model, User.Identity.GetUserId());

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
            await CreateSelectListsAsync(fromStoreId: model.FromStoreId, toStoreId: model.ToStoreId);

            //Return view with the model
            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? fromStoreId = null,long? toStoreId=null)
        {
            List<Store> stores;

            //Get stores
            stores =await bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync();

            //Create select list for from-store and map to keyvaluepair for the view
            ViewBag.FromStoreId = new SelectList(stores.Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value", fromStoreId);

            //Create select list for to-store and map to keyvaluepair for the view
            ViewBag.ToStoreId = new SelectList(stores.Select(c => new KeyValuePairViewModel<long, string>
            {
                Key = c.Id,
                Value = c.Id.ToString() + ": " + c.Name,
            }),
                "Key", "Value", toStoreId);
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
                "Key", "Value",bookEdtionId);
        }

        #endregion
    }
}
