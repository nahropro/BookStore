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
    public class StoresController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/Stores/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/Stores/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/Stores/Edit.cshtml";

        public StoresController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: Stores
        public async Task<ActionResult> Index()
        {
            return View(INDEX,(await this.bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync()).OrderByDescending(s=> s.Id));
        }

        // GET: Stores/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include ="Name,Address")] Store store)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(CREATE,store);
                }

                this.bookStoreUnitOfWork.Stores.Add(store);

                await this.bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(CREATE,store);
            }
        }

        // GET: Stores/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            return View(EDIT,await this.bookStoreUnitOfWork.Stores.SingleOrDefaultNoTrackingAsync(s=> s.Id==id));
        }

        // POST: Stores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, [Bind(Include = "Id,Name,Address")] Store store)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(EDIT,store);
                }

                this.bookStoreUnitOfWork.Stores.Edit(store);
                await this.bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(EDIT,store);
            }
        }
    }
}
