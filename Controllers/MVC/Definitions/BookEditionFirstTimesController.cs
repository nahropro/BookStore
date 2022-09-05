using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Models;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.BookEditionFirstTime;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.Definitions
{
    [AuthorizeFilter(Roles = RoleNames.MANAGER + "," + RoleNames.ADMIN)]
    public class BookEditionFirstTimesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly BookEditionFirstTimeManager bookEditionFirstTimeManager;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/BookEditionFirstTimes/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/BookEditionFirstTimes/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/BookEditionFirstTimes/Edit.cshtml";
        private const string ROW_PARTIAL = "~/Views/Definitions/BookEditionFirstTimes/_ItemRow.cshtml";

        public BookEditionFirstTimesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            bookEditionFirstTimeManager = new BookEditionFirstTimeManager(bookStoreUnitOfWork);
        }

        // GET: BookEditionFirstTimes
        public async Task<ActionResult> Index(BookEditionFirstTimeFilter filter=null)
        {
            List<SelectBookEditionFirstTimeViewModel> selectFistTimes;

            //Get filtered first time
            selectFistTimes = await bookEditionFirstTimeManager.GetAllNoTrackingAsync(filter);

            //Create select lists
            await CreateSelectListsAsync(storeId:filter.StoreId,bookEditionId:filter.BookEditionId);

            //Store filter to send it to view
            ViewBag.Filter = filter;

            return View(INDEX, selectFistTimes);
        }

        public ActionResult GetRowPartial(CreateEditBookEditionFirstTimeViewModel row)
        {
            //Create book-editions and stores select list
            CreateSelectListsForPartialRow(storeId: row.StoreId, bookEditionId: row.BookEditionId);

            return PartialView(ROW_PARTIAL, row);
        }

        // GET: BookEditionFirstTimes/Create
        public ActionResult Create()
        {
            return View(CREATE);
        }

        // POST: BookEditionFirstTimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(List<CreateEditBookEditionFirstTimeViewModel> rows,string returnUrl)
        {
            try
            {
                //Check if model-state is valid and qtt is not zero
                //And price must be positive
                if (ModelState.IsValid && rows.All(i=> i.Qtt!=0 && i.Price>=0))
                {
                    //Check if all rows dose not exists in the database with the same store and book-edition
                    if (!await CheckForExistence(rows))
                    {
                        //Map row view-model to model class and add it
                        bookStoreUnitOfWork.BookEditionFirstTimes.AddRange(rows.Select(i => new BookEditionFirstTime
                        {
                            BookEditionId = i.BookEditionId,
                            StoreId = i.StoreId,
                            Qtt = i.Qtt,
                            Price = i.Price,
                            Total = i.Qtt * i.Price,
                            CreatorUserId = User.Identity.GetUserId(),
                            CreationDateTime = DateTime.UtcNow,
                        }));

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
            }
            catch
            {

            }

            //If not success
            return View(CREATE, rows);
        }

        // GET: BookEditionFirstTimes/Edit/5
        public async Task<ActionResult> Edit(long storeId,long bookEditionId)
        {
            CreateEditBookEditionFirstTimeViewModel model;

            //Get the item and map it to view-model
            model = await bookStoreUnitOfWork.BookEditionFirstTimes.SingleOrDefaultNoTrackingAsync(i =>
                  i.StoreId == storeId && i.BookEditionId == bookEditionId);

            //Create viemodels for store name and book-name
            await ViewBagStoreNameAndBookName(storeId:model.StoreId, bookEditionId: model.BookEditionId);

            return View(EDIT,model);
        }

        // POST: BookEditionFirstTimes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long storeId, long bookEditionId, CreateEditBookEditionFirstTimeViewModel model, string returnUrl)
        {
            BookEditionFirstTime firstTime;

            try
            {
                //If model-state is valid and qtt is not zero and price must be positive
                if (ModelState.IsValid && model.Qtt!=0 && model.Price>=0)
                {
                    //Map view model to model
                    firstTime = model;
                    firstTime.EditorUserId = User.Identity.GetUserId(); //Set user-id to editor id

                    bookStoreUnitOfWork.BookEditionFirstTimes.Edit(firstTime);

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
            //Create viemodels for store name and book-name
            await ViewBagStoreNameAndBookName(storeId: model.StoreId, bookEditionId: model.BookEditionId);

            return View(EDIT, model);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private void CreateSelectListsForPartialRow(long? storeId = null, long? bookEditionId = null)
        {
            //Get all stores and make them for slectlist and map to keyvaluepair for the view
            ViewBag.StoreId = new SelectList(
                bookStoreUnitOfWork.Stores.GetAllNoTracking().Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value", storeId);

            //Get all book-editions and make them for slectlist and map to keyvaluepair for the view
            ViewBag.BookEditionId = new SelectList(
                bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludes(includeBook: true)
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Book.Name + " (" + c.EditionInNumber + " - " + c.EditionInString + ")",
                }),
                "Key", "Value", bookEditionId);
        }

        private async Task CreateSelectListsAsync(long? storeId = null, long? bookEditionId = null)
        {
            //Get all stores and make them for slectlist and map to keyvaluepair for the view
            ViewBag.StoreId = new SelectList(
                (await bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync()).Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value", storeId);

            //Get all book-editions and make them for slectlist and map to keyvaluepair for the view
            ViewBag.BookEditionId = new SelectList(
                (await bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludesAsync(includeBook: true))
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Book.Name + " (" + c.EditionInNumber + " - " + c.EditionInString + ")",
                }),
                "Key", "Value", bookEditionId);
        }

        private async Task ViewBagStoreNameAndBookName(long storeId,long bookEditionId)
        {
            Store store;
            BookEdition bookEdition;

            //Get the store and book-edition of the item
            store = await bookStoreUnitOfWork.Stores.SingleOrDefaultNoTrackingAsync(i => i.Id == storeId);
            bookEdition = await bookStoreUnitOfWork.BookEditions.SingleOrDefaultNoTrackingWithIncludesAsync(i =>
                i.Id == bookEditionId, includeBook: true);

            //Set store-name and book-name in viewbag, for using it in view
            ViewBag.StoreName = store.Name;
            ViewBag.BookName = bookEdition.Book.Name + "( " + bookEdition.EditionInNumber.ToString() +
                " - " + bookEdition.EditionInString + " )";
        }

        private async Task<bool> CheckForExistence(List<CreateEditBookEditionFirstTimeViewModel> items)
        {
            bool result = false;

            foreach (var item in items)
            {
                if (await bookStoreUnitOfWork.BookEditionFirstTimes.IsExists(item.StoreId,item.BookEditionId))
                {
                    item.IsExists = true;
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}
