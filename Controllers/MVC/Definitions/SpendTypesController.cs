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
    public class SpendTypesController : Controller
    {
        BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/SpendTypes/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/SpendTypes/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/SpendTypes/Edit.cshtml";

        public SpendTypesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: SpendTypes
        public async Task<ActionResult> Index()
        {
            return View(INDEX,
                (await bookStoreUnitOfWork.SpendTypes.GetAllNoTrackingAsync()).OrderByDescending(i=> i.Id).ToList());
        }
        
        // GET: SpendTypes/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: SpendTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")]SpendType model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bookStoreUnitOfWork.SpendTypes.Add(model);

                    await bookStoreUnitOfWork.CompleteAsync();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }

            //If not success
            return View(CREATE,model);
        }

        // GET: SpendTypes/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            SpendType spendType;

            //Get spend-type for editiong
            spendType = await bookStoreUnitOfWork.SpendTypes.SingleOrDefaultNoTrackingAsync(i => i.Id == id);

            if (spendType==null)
            {
                return HttpNotFound();
            }

            return View(EDIT,spendType);
        }

        // POST: SpendTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, [Bind(Include = "Id,Name")]SpendType model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bookStoreUnitOfWork.SpendTypes.Edit(model);

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
