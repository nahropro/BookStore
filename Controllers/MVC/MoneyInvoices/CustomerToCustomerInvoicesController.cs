using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.CustomerToCustomerInvoice;
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
    public class CustomerToCustomerInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly CustomerToCustomerInvoiceManager ctocInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/MoneyInvoices/CustomerToCustomerInvoices/Index.cshtml";
        private const string CREATE = "~/Views/MoneyInvoices/CustomerToCustomerInvoices/Create.cshtml";
        private const string EDIT = "~/Views/MoneyInvoices/CustomerToCustomerInvoices/Edit.cshtml";

        public CustomerToCustomerInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            ctocInvoiceManager = new CustomerToCustomerInvoiceManager(bookStoreUnitOfWork);
        }

        // GET: CustomerToCustomerInvoices
        public async Task<ActionResult> Index(CustomerToCustomerInvoiceFilter filter = null)
        {
            //If filter is null, there is no querystring
            if (Request.QueryString.Count == 0)
            {
                //Set today date to start and end date
                //Set only date and time 00:00:00
                filter = new CustomerToCustomerInvoiceFilter
                {
                    StartDate = DateTimeManager.GetNowDateOnly(),
                    EndDate = DateTimeManager.GetNowDateOnly(),
                };
            }

            //Return the filter object throw ViewBag
            ViewBag.Filter = filter;

            //Create selectlists for filter
            await CreateSelectListsAsync(payCustomerId: filter?.PayCustomerId, giveCustomerId: filter?.GiveCustomerId);

            return View(INDEX,
                (await ctocInvoiceManager.GetAllNoTrackingAsync(filter)).OrderByDescending(i => i.Id));
        }

        // GET: CustomerToCustomerInvoices/Create
        public async Task<ActionResult> Create()
        {
            //Create nessesary selectlists and store them in viewbag
            await CreateSelectListsAsync();

            //Return view with some default values
            return View(CREATE, new CreateEditCustomerToCustomerInvooiceViewModel
            {
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: CustomerToCustomerInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditCustomerToCustomerInvooiceViewModel model,string returnUrl)
        {
            try
            {
                //Check for validation and pay and give customer are not the same
                if (ModelState.IsValid && model.PayCustomerId != model.GiveCustomerId)
                {
                    //Add the model
                    bookStoreUnitOfWork.CustomerToCustomerInvoices.Add(model, User.Identity.GetUserId());

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

            //If not success
            //Create nessesary selectlists and store them in viewbag
            await CreateSelectListsAsync(payCustomerId:model.PayCustomerId, giveCustomerId:model.GiveCustomerId);

            return View(CREATE, model);
        }

        // GET: CustomerToCustomerInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {

            CreateEditCustomerToCustomerInvooiceViewModel model;

            //Get the invoice and map it to createeditviewmodel
            model = await bookStoreUnitOfWork.CustomerToCustomerInvoices.SingleOrDefaultNoTrackingAsync(i => i.Id == id);

            if (model == null)
            {
                return HttpNotFound();
            }

            //Create nessesary select lists
            await CreateSelectListsAsync(payCustomerId: model.PayCustomerId, giveCustomerId: model.GiveCustomerId);

            return View(EDIT, model);
        }

        // POST: CustomerToCustomerInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditCustomerToCustomerInvooiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.GiveCustomerId != model.PayCustomerId)
                {
                    bookStoreUnitOfWork.CustomerToCustomerInvoices.Edit(model, User.Identity.GetUserId());

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
            await CreateSelectListsAsync(payCustomerId: model.PayCustomerId, giveCustomerId: model.GiveCustomerId);

            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private async Task CreateSelectListsAsync(long? payCustomerId = null, long? giveCustomerId = null)
        {
            List<KeyValuePairViewModel<long, string>> customers;

            //Get all vaults
            customers = (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
            {
                Key = c.Id,
                Value = c.Id.ToString() + ": " + c.FullName + " (" + c.WorkPlace + ")",
            }).ToList();

            //Get all vaults for pay and make them for slectlist and map to keyvaluepair for the view
            ViewBag.PayCustomerId = new SelectList(customers, "Key", "Value", payCustomerId);

            //Get all vaults for give and make them for slectlist and map to keyvaluepair for the view
            ViewBag.GiveCustomerId = new SelectList(customers, "Key", "Value", giveCustomerId);
        }

        #endregion
    }
}
