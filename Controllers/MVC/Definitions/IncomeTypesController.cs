using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.Models;
using BookStoreModel.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.Definitions
{
    [AuthorizeFilter(Roles = RoleNames.MANAGER + "," + RoleNames.ADMIN)]
    public class IncomeTypesController : Controller
    {
        BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/IncomeTypes/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/IncomeTypes/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/IncomeTypes/Edit.cshtml";

        public IncomeTypesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: SpendTypes
        public async Task<ActionResult> Index()
        {
            return View(INDEX,
                (await bookStoreUnitOfWork.IncomeTypes.GetAllNoTrackingAsync()).OrderByDescending(i => i.Id).ToList());
        }

        // GET: SpendTypes/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: SpendTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")]IncomeType model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bookStoreUnitOfWork.IncomeTypes.Add(model);

                    await bookStoreUnitOfWork.CompleteAsync();

                    return RedirectToAction("Index");
                }
            }
            catch
            {

            }

            //If not success
            return View(CREATE, model);
        }

        // GET: SpendTypes/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            IncomeType incomeType;

            //Get spend-type for editiong
            incomeType = await bookStoreUnitOfWork.IncomeTypes.SingleOrDefaultNoTrackingAsync(i => i.Id == id);

            if (incomeType == null)
            {
                return HttpNotFound();
            }

            return View(EDIT, incomeType);
        }

        // POST: SpendTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, [Bind(Include = "Id,Name")]IncomeType model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bookStoreUnitOfWork.IncomeTypes.Edit(model);

                    await bookStoreUnitOfWork.CompleteAsync();

                    return RedirectToAction("Index");
                }
            }
            catch
            {

            }

            //If not success
            return View(CREATE, model);
        }
    }
}