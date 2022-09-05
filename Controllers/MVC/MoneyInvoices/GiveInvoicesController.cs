using BookStore.Data.Repository;
using BookStoreModel.StaticData;
using BookStoreModel.GeneralModels;
using BookStoreModel.ViewModels.GiveInvoice;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.Other;

namespace BookStore.Controllers.MVC.MoneyInvoices
{
    [AuthorizeFilter(Roles =RoleNames.ADMIN+","+RoleNames.MANAGER+","+RoleNames.EMPLOYEE)]
    public class GiveInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private GiveInvoiceManager giveInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/MoneyInvoices/GiveInvoices/Index.cshtml";
        private const string CREATE = "~/Views/MoneyInvoices/GiveInvoices/Create.cshtml";
        private const string EDIT = "~/Views/MoneyInvoices/GiveInvoices/Edit.cshtml";

        public GiveInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            giveInvoiceManager = new GiveInvoiceManager(bookStoreUnitOfWork);
        }

        // GET: GiveInvoices
        public async Task<ActionResult> Index(PayGiveInvoiceFilter filter=null)
        {
            //If filter is null, there is no querystring
            if (Request.QueryString.Count==0)
            {
                //Set today date to start and end date
                //Set only date and time 12:00:00 AM
                filter.StartDate = DateTimeManager.GetNowDateOnly();
                filter.EndDate = DateTimeManager.GetNowDateOnly();
            }

            //Return the filter object throw ViewBag
            ViewBag.Filter = filter;

            //Create selectlists for filter
            await CreateSelectListsAsync(customerId: filter?.CustomerId, vaultId: filter?.VaultId);

            //Return index view with
            return View(INDEX,
                (await giveInvoiceManager.GetAllNoTrackingAsync(filter))
                .OrderByDescending(gi=> gi.Id).ToList());
        }

        // GET: GiveInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create selectlists
            await CreateSelectListsAsync();

            //Retrun view with some default values
            return View(CREATE, new CreateEditGiveInvoiceViewModel
            {
                Amount = 0,
                Discount = 0,
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: GiveInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditGiveInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && (model.Discount+model.Amount)>0)
                {
                    //Add data and save it to database
                    bookStoreUnitOfWork.GiveInvoices.Add(model, User.Identity.GetUserId());
                    await bookStoreUnitOfWork.CompleteAsync();

                    //If return url is null redirect to index
                    if (returnUrl==null)
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
            //Create selectlists and store them in viewbags
            await CreateSelectListsAsync(model.CustomerId, model.VaultId);
            //Return to create view with the same data
            return View(CREATE,model);
        }

        // GET: GiveInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            CreateEditGiveInvoiceViewModel model;

            //Get giveinvoice and map it for edit
            model =await bookStoreUnitOfWork.GiveInvoices.SingleOrDefaultNoTrackingAsync(gi=> gi.Id==id);

            //If invoice not exists return notfound http status code
            if (model==null)
            {
                return HttpNotFound();
            }

            //Create selectlists with select ids
            await CreateSelectListsAsync(customerId: model.CustomerId, vaultId: model.VaultId);

            return View(EDIT,model);
        }

        // POST: GiveInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditGiveInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && (model.Discount + model.Amount) > 0)
                {
                    bookStoreUnitOfWork.GiveInvoices.Edit(model,User.Identity.GetUserId());

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
            catch (Exception ex)
            {
                
            }

            //If not succeded
            //Create selectlists with select ids
            await CreateSelectListsAsync(customerId: model.CustomerId, vaultId: model.VaultId);

            return View(EDIT, model);
        }

        #region Helpers
        
        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? customerId=null, long? vaultId = null)
        {
            //Get all customers and make them for slectlist and map to keyvaluepair for the view
            ViewBag.CustomerId = new SelectList(
                (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.FullName + " (" + c.WorkPlace + ")",
                }),
                "Key", "Value",customerId);
            //Get all vaults and make them for slectlist and map to keyvaluepair for the view
            ViewBag.VaultId = new SelectList(
                (await bookStoreUnitOfWork.Vaults.GetAllNoTrackingAsync()).Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Name,
                }),
                "Key", "Value",vaultId);
        }
        
        #endregion
    }
}
