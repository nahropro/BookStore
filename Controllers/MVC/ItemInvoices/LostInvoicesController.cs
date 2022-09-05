using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
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
    public class LostInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly LostInvoiceManager lostInvoiceManager;

        //View page addresses
        private const string INDEX = "~/Views/ItemInvoices/LostInvoices/Index.cshtml";
        private const string CREATE = "~/Views/ItemInvoices/LostInvoices/Create.cshtml";
        private const string EDIT = "~/Views/ItemInvoices/LostInvoices/Edit.cshtml";
        private const string INVOICE = "~/Views/ItemInvoices/LostInvoices/Invoice.cshtml";

        public LostInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            lostInvoiceManager = new LostInvoiceManager(bookStoreUnitOfWork);
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

            //Return index view with filterd data
            return View(INDEX,
                (await lostInvoiceManager.GetAllNoTrackingAsync(filter))
                .OrderByDescending(gi => gi.Id).ToList());
        }

        // GET: SellInvoices/Details/5
        public async Task<ActionResult> Invoice(long id)
        {
            SelectLostInvoiceViewModel model;

            //Return invoice and map it to view model
            model = await bookStoreUnitOfWork.LostInvoices
                .GetNoTrackingWithIncludesAsync(id, includeItems: true, includeBookEdition: true, includeStore: true);

            return View(INVOICE, model);
        }

        // GET: SellInvoices/Create
        public ActionResult Create()
        {
            //Return view and some default values
            return View(CREATE, new CreateEditLostInvoiceViewModel
            {
                InvoiceDate = DateTimeManager.GetNowDateOnly(),
            });
        }

        // POST: SellInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditLostInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    bookStoreUnitOfWork.LostInvoices.Add(model, User.Identity.GetUserId());

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
            //Return view with the model
            return View(CREATE, model);
        }

        // GET: SellInvoices/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            CreateEditLostInvoiceViewModel model;

            //Get the invoice and map it to modelview
            model = await bookStoreUnitOfWork.LostInvoices.GetNoTrackingWithIncludesAsync(id, includeItems: true);
            
            return View(EDIT, model);
        }

        // POST: SellInvoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CreateEditLostInvoiceViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid && model.Items.Count > 0)
                {
                    await lostInvoiceManager.Edit(model, User.Identity.GetUserId());

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
            //Return view with the model
            return View(EDIT, model);
        }
    }
}