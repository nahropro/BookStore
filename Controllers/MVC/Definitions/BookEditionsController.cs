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
    [AuthorizeFilter(Roles =RoleNames.MANAGER +","+RoleNames.ADMIN)]
    public class BookEditionsController : Controller
    {

        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string CREATE = "~/Views/Definitions/BookEditions/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/BookEditions/Edit.cshtml";

        public BookEditionsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }


        [Route("Books/{id:long}/BookEditions/Create")]
        public async Task<ActionResult> Create(long id)
        {
            ViewBag.Book=await bookStoreUnitOfWork.Books.SingleOrDefaultNoTrackingAsync(b=> b.Id==id);
            return View(CREATE);
        }

        // POST: BookEditions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Books/{id:long}/BookEditions/Create")]
        public async Task<ActionResult> Create(long id,
            [Bind(Include ="EditionInNumber,EditionInString,YearOfPrint,PlaceOfPrint,NumberOfCopies,BookId,SellPrice,DischargePrice,Price")] BookEdition bookEdition)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                bookStoreUnitOfWork.BookEditions.Add(bookEdition);
                await bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index","Books");
            }
            catch
            {
                ViewBag.Book = await bookStoreUnitOfWork.Books.SingleOrDefaultNoTrackingAsync(b => b.Id == bookEdition.BookId);
                return View(CREATE,bookEdition);
            }
        }

        // GET: BookEditions/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            return View(EDIT,await bookStoreUnitOfWork.BookEditions.SingleOrDefaultNoTrackingWithIncludesAsync(be=> be.Id==id,includeBook:true));
        }

        // POST: BookEditions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id,
            [Bind(Include = "Id,EditionInNumber,EditionInString,YearOfPrint,PlaceOfPrint,NumberOfCopies,BookId,SellPrice,DischargePrice,Price")] BookEdition bookEdition)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                bookStoreUnitOfWork.BookEditions.Edit(bookEdition);
                await bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index","Books");
            }
            catch
            {
                return View(EDIT,bookEdition);
            }
        }
    }
}
