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
    public class BooksController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/Books/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/Books/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/Books/Edit.cshtml";
        

        public BooksController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        // GET: Book
        public async Task<ActionResult> Index()
        {
            return View(INDEX,(await this.bookStoreUnitOfWork.Books.GetAllNoTrackingWithIncludesAsync(includeBookEdition:true))
                .OrderByDescending(b=> b.Id));
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include ="Name,Authors,Translators")] Book book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(CREATE, book);
                }

                this.bookStoreUnitOfWork.Books.Add(book);
                await this.bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(CREATE,book);
            }
        }

        // GET: Book/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            return View(EDIT,await bookStoreUnitOfWork.Books.SingleOrDefaultNoTrackingAsync(b=> b.Id==id));
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, [Bind(Include = "Id,Name,Authors,Translators")] Book book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(EDIT, book);
                }

                bookStoreUnitOfWork.Books.Edit(book);
                await bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(EDIT,book);
            }
        }

    }
}
