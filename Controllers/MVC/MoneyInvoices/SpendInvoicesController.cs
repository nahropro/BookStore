using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Models;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.SpendIncome;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.MoneyInvoices
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class SpendInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly SpendInvoiceManager spendInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/MoneyInvoices/SpendInvoices/Index.cshtml";
        private const string CREATE = "~/Views/MoneyInvoices/SpendInvoices/Create.cshtml";
        private const string EDIT = "~/Views/MoneyInvoices/SpendInvoices/Edit.cshtml";
        private const string INVOICE = "~/Views/MoneyInvoices/SpendInvoices/invoice.cshtml";
        private const string ITEM_ROW = "~/Views/MoneyInvoices/SpendInvoices/_ItemRow.cshtml";

        public SpendInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            spendInvoiceManager = new SpendInvoiceManager(bookStoreUnitOfWork);
        }


        // GET: SpendInvoices
        public async Task<ActionResult> Index(SpendIncomeFilter filter)
        {
            //If filter is null, there is no querystring
            if (Request.QueryString.Count == 0)
            {
                //Set today date to start and end date
                //Set only date and time 00:00:00
                filter.StartDate = DateTime.Today;
                filter.EndDate = DateTime.Today;
            }

            //Return the filter object throw ViewBag
            ViewBag.Filter = filter;

            //Create select lists
            await CreateSelectListsAsync(customerId: filter.CustomerId, vaultId: filter.VaultId);

            return View(INDEX,
                (await spendInvoiceManager.GetAllNoTrackingAsync(filter)).OrderByDescending(i=> i.Id));
        }

        // GET: SpendInvoices/Details/5
        public async Task<ActionResult> Invoice(long id)
        {
            SelectSpendIncomeInvoiceViewModel invoiceViewModel;
            SpendInvoice invoiceModel;

            //Get model invoice
            invoiceModel = await bookStoreUnitOfWork.SpendInvoices.GetNoTrackingWithIncludesAsync(id, true, true, true, true);

            //Map invoice-model to invoice-slect-vie-model
            invoiceViewModel =(SelectSpendIncomeInvoiceViewModel) invoiceModel;

            //Return view injected with invoice-select-view-model
            return View(INVOICE, invoiceViewModel);
        }

        public ActionResult GetItemRow(CreateEditSpendIncomeItemViewModel model)
        {
            //Get all stores and make them for slectlist and map to keyvaluepair for the view
            ViewBag.Types = new SelectList(
                bookStoreUnitOfWork.SpendTypes.GetAllNoTracking()
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value", model.TypeId);

            return PartialView(ITEM_ROW, model);
        }

        // GET: SpendInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create nessessary selectlists
            await CreateSelectListsAsync();

            //Return view with some default value in model
            return View(CREATE,new CreateEditSpendIncomeInvoiceViewModel
            {
                InvoiceDate= DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: SpendInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditSpendIncomeInvoiceViewModel model,string returnUrl)
        {
            SpendInvoice invoice;

            try
            {
                if (ModelState.IsValid &&
                    model.Items.LongCount() > 0 &&
                    model.Items.All(i => i.Amount > 0) &&
                    model.Loan.GetValueOrDefault() + model.Cash.GetValueOrDefault() == model.Items.Select(i => i.Amount).DefaultIfEmpty(0).Sum() &&
                    model.Loan.GetValueOrDefault() >= 0 &&
                    model.Cash.GetValueOrDefault() >= 0 &&
                    (model.Loan.GetValueOrDefault() > 0 ? model.CustomerId.HasValue : true) &&
                    (model.Cash.GetValueOrDefault() > 0 ? model.VaultId.HasValue : true))
                {
                    //Map view-model to model
                    invoice = (SpendInvoice)model;
                    invoice.CreatorUserId = User.Identity.GetUserId();  //Set current user if for invoice creator

                    bookStoreUnitOfWork.SpendInvoices.Add(invoice);

                    await bookStoreUnitOfWork.CompleteAsync();

                    //If return url is null redirect to index
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

            //If not success
            //Create nessessary selectlists
            await CreateSelectListsAsync();

            return View(CREATE,model);
        }

        // GET: SpendInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            SpendInvoice invoice;
            CreateEditSpendIncomeInvoiceViewModel createEdit;

            //Get the invoice and snd it to the view for editing
            invoice = await bookStoreUnitOfWork.SpendInvoices.GetNoTrackingWithIncludesAsync(id,includeItems:true);

            if (invoice==null)
            {
                return HttpNotFound();
            }

            //Create select lists
            await CreateSelectListsAsync(customerId:invoice.CustomerId,vaultId:invoice.VaultId);

            //Cast model to view-model
            createEdit =(CreateEditSpendIncomeInvoiceViewModel) invoice;

            //Return view with injected invoice-view-mode for creare and edit
            return View(EDIT,createEdit);
        }

        // POST: SpendInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditSpendIncomeInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid &&
                    model.Items.LongCount() > 0 &&
                    model.Items.All(i => i.Amount > 0) &&
                    model.Loan.GetValueOrDefault() + model.Cash.GetValueOrDefault() == model.Items.Select(i => i.Amount).DefaultIfEmpty(0).Sum() &&
                    model.Loan.GetValueOrDefault() >= 0 &&
                    model.Cash.GetValueOrDefault() >= 0 &&
                    (model.Loan.GetValueOrDefault() > 0 ? model.CustomerId.HasValue : true) &&
                    (model.Cash.GetValueOrDefault() > 0 ? model.VaultId.HasValue : true))
                {
                    //Call edit in invoice-manager
                    await spendInvoiceManager.Edit(model, User.Identity.GetUserId());

                    await bookStoreUnitOfWork.CompleteAsync();

                    //If return url is null redirect to index
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

            //If not success
            //Create nessessary selectlists
            await CreateSelectListsAsync();

            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? customerId = null, long? vaultId = null)
        {
            //Get all custoemr and make them for slectlist and map to keyvaluepair for the view
            ViewBag.CustomerId = new SelectList(
                (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync())
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.FullName +" ("+c.WorkPlace+")",
                }),
                "Key", "Value", customerId);

            //Get all book-vaults and make them for slectlist and map to keyvaluepair for the view
            ViewBag.VaultId = new SelectList(
                (await bookStoreUnitOfWork.Vaults.GetAllNoTrackingAsync())
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value", vaultId);
        }

        #endregion

    }
}
