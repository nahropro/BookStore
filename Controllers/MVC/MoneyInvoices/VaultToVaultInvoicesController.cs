using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.VaultToVaultInvoice;
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
    public class VaultToVaultInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly VaultToVaultInvoiceManager vaultToVaultInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/MoneyInvoices/VaultToVaultInvoices/Index.cshtml";
        private const string CREATE = "~/Views/MoneyInvoices/VaultToVaultInvoices/Create.cshtml";
        private const string EDIT = "~/Views/MoneyInvoices/VaultToVaultInvoices/Edit.cshtml";

        public VaultToVaultInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            vaultToVaultInvoiceManager = new VaultToVaultInvoiceManager(bookStoreUnitOfWork);
        }

        // GET: VaultToVaultInvoices
        public async Task<ActionResult> Index(VaultToVaultInvoiceFilter filter=null)
        {
            //If filter is null, there is no querystring
            if (Request.QueryString.Count == 0)
            {
                //Set today date to start and end date
                //Set only date and time 00:00:00
                filter = new VaultToVaultInvoiceFilter
                {
                    StartDate = DateTimeManager.GetNowDateOnly(),
                    EndDate = DateTimeManager.GetNowDateOnly(),
                };
            }

            //Return the filter object throw ViewBag
            ViewBag.Filter = filter;

            //Create selectlists for filter
            await CreateSelectListsAsync(payVaultId:filter?.PayVaultId,giveVaultId: filter?.GiveVaultId);

            //Return index view with filterd data
            return View(INDEX,
                (await vaultToVaultInvoiceManager.GetAllNoTrackingAsync(filter)).OrderByDescending(i=> i.Id));
        }

        // GET: VaultToVaultInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create nessessary select lists
            await CreateSelectListsAsync();

            //Return view with some default values
            return View(CREATE, new CreateEditVaultToVaultInvoiceViewModel
            {
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: VaultToVaultInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditVaultToVaultInvoiceViewModel model,string returnUrl)
        {
            try
            {
                //Check for validation and pay and give vault are not the same
                if (ModelState.IsValid && model.PayVaultId!=model.GiveVaultId)
                {
                    //Add the model
                    bookStoreUnitOfWork.VaultToVaultInvoices.Add(model,User.Identity.GetUserId());

                    //Save changes
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

            //Create nessessary select lists
            await CreateSelectListsAsync();

            return View(CREATE, model);
        }

        // GET: VaultToVaultInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            CreateEditVaultToVaultInvoiceViewModel model;

            //Get the invoice and map it to createeditviewmodel
            model = await bookStoreUnitOfWork.VaultToVaultInvoices.SingleOrDefaultNoTrackingAsync(i => i.Id == id);

            if (model==null)
            {
                return HttpNotFound();
            }

            //Create nessesary select lists
            await CreateSelectListsAsync(payVaultId: model.PayVaultId, giveVaultId: model.GiveVaultId);

            return View(EDIT,model);
        }

        // POST: VaultToVaultInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditVaultToVaultInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.GiveVaultId!=model.PayVaultId)
                {
                    bookStoreUnitOfWork.VaultToVaultInvoices.Edit(model, User.Identity.GetUserId());

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

            //Create nessesary select lists
            await CreateSelectListsAsync(payVaultId: model.PayVaultId, giveVaultId: model.GiveVaultId);

            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? payVaultId = null, long? giveVaultId = null)
        {
            List<KeyValuePairViewModel<long, string>> vaults;

            //Get all vaults
            vaults= (await bookStoreUnitOfWork.Vaults.GetAllNoTrackingAsync()).Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Name,
                }).ToList();
 
            //Get all vaults for pay and make them for slectlist and map to keyvaluepair for the view
            ViewBag.PayVaultId = new SelectList(vaults,"Key", "Value", payVaultId);

            //Get all vaults for give and make them for slectlist and map to keyvaluepair for the view
            ViewBag.GiveVaultId = new SelectList(vaults,"Key", "Value", giveVaultId);
        }

        #endregion
    }
}
