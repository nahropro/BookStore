using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Models;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.VaultCorrectionInvoice;
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
    public class VaultCorrectionInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly VaultCorrectionInvoiceManager vaultCorrectionInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/MoneyInvoices/VaultCorrectionInvoices/Index.cshtml";
        private const string CREATE = "~/Views/MoneyInvoices/VaultCorrectionInvoices/Create.cshtml";
        private const string EDIT = "~/Views/MoneyInvoices/VaultCorrectionInvoices/Edit.cshtml";

        public VaultCorrectionInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            vaultCorrectionInvoiceManager = new VaultCorrectionInvoiceManager(bookStoreUnitOfWork);
        }

        // GET: VaultCorrectionInvoices
        public async Task<ActionResult> Index(VaultCorrectionFilter filter=null)
        {
            if (Request.QueryString.Count==0)
            {
                //If filter is empty fill with some default data
                filter.StartDate = DateTime.Today;
                filter.EndDate = DateTime.Today;
            }

            //Soter filter in the viewbag
            ViewBag.Filter = filter;

            //Create select llist for vaults
            await CreateSelectListsAsync(filter.VaultId);

            //Get the data order by decending
            var result =(await vaultCorrectionInvoiceManager.GetAllNoTrackingAsync(filter)).OrderByDescending(i=> i.Id);

            return View(INDEX,result);
        }

        public async Task<ActionResult> Create()
        {
            //Create model with some default values
            CreateEditVaultCorrectionInvoiceViewModel model = new CreateEditVaultCorrectionInvoiceViewModel
            {
                InvoiceDate = DateTime.Today,
            };

            //create select lists
            await CreateSelectListsAsync();

            return View(CREATE, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditVaultCorrectionInvoiceViewModel model, string returnUrl)
        {
            VaultCorrectionInvoice invoice;

            try
            {
                if (ModelState.IsValid &&
                    model.Amount>0)
                {
                    //Map view-model to model
                    invoice = (VaultCorrectionInvoice)model;
                    invoice.CreatorUserId = User.Identity.GetUserId();  //Set current user if for invoice creator

                    bookStoreUnitOfWork.VaultCorrectionInvoices.Add(invoice);

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

            return View(CREATE, model);
        }

        public async Task<ActionResult> Edit(long id)
        {
            CreateEditVaultCorrectionInvoiceViewModel createEditViewModel;

            //Get the invoice from database
            VaultCorrectionInvoice invoice = await bookStoreUnitOfWork.VaultCorrectionInvoices.GetAsync(id);

            if (invoice is null)
            {
                return HttpNotFound();
            }

            //Map invoice model to viewmodel
            createEditViewModel = (CreateEditVaultCorrectionInvoiceViewModel)invoice;

            //Create select lists
            await CreateSelectListsAsync(createEditViewModel.VaultId);

            return View(EDIT, createEditViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreateEditVaultCorrectionInvoiceViewModel model, string returnUrl)
        {
            VaultCorrectionInvoice invoice;

            try
            {
                if (ModelState.IsValid &&
                    model.Amount > 0)
                {
                    //Map view-model to model
                    invoice = (VaultCorrectionInvoice)model;

                    bookStoreUnitOfWork.VaultCorrectionInvoices.Edit(invoice, User.Identity.GetUserId());

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
        private async Task CreateSelectListsAsync(long? vaultId = null)
        {
            //Get all custoemr and make them for slectlist and map to keyvaluepair for the view
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