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
    public class VaultsController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/Vaults/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/Vaults/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/Vaults/Edit.cshtml";

        public VaultsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: Vaults
        public async Task<ActionResult> Index()
        {
            return View(INDEX,(await bookStoreUnitOfWork.Vaults.GetAllNoTrackingAsync()).OrderByDescending(v=> v.Id));
        }

        // GET: Vaults/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: Vaults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include ="Name,Address,FirstAmount")]Vault vault)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(CREATE,vault);
                }

                bookStoreUnitOfWork.Vaults.Add(vault);

                await bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(CREATE,vault);
            }
        }

        // GET: Vaults/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            return View(EDIT,await this.bookStoreUnitOfWork.Vaults.SingleOrDefaultNoTrackingAsync(v=> v.Id==id));
        }

        // POST: Vaults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, [Bind(Include = "Id,Name,Address,FirstAmount")]Vault vault)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(EDIT,vault);
                }

                this.bookStoreUnitOfWork.Vaults.Edit(vault);
                await this.bookStoreUnitOfWork.CompleteAsync();


                return RedirectToAction("Index");
            }
            catch
            {
                return View(EDIT,vault);
            }
        }
        
    }
}
