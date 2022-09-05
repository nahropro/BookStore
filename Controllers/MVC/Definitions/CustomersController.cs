using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.StaticData;
using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.Definitions
{
    [AuthorizeFilter(Roles = RoleNames.MANAGER + "," + RoleNames.ADMIN)]
    public class CustomersController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/Customers/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/Customers/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/Customers/Edit.cshtml";

        public CustomersController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: Customers
        public async Task<ActionResult> Index()
        {
            return View(INDEX,(await this.bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync()).OrderByDescending(c=> c.Id));
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: Customers/Create
        [HttpPost]
        public async Task<ActionResult> Create(
            [Bind(Include ="FullName,WorkPlace,Phone,Address,Active,FirstTimeBalance")] Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(CREATE,customer);
                }

                this.bookStoreUnitOfWork.Customers.Add(customer);
                await this.bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(CREATE,customer);
            }
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            return View(EDIT,await this.bookStoreUnitOfWork.Customers.SingleOrDefaultNoTrackingAsync(c=> c.Id==id));
        }

        // POST: Customers/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(long id, 
            [Bind(Include = "Id,FullName,WorkPlace,Phone,Address,Active,FirstTimeBalance")] Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(EDIT,customer);
                }

                this.bookStoreUnitOfWork.Customers.Edit(customer);
                await this.bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(EDIT,customer);
            }
        }
    }
}
