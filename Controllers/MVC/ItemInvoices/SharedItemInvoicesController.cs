using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.APIModels.Customers;
using BookStoreModel.APIModels.Stores;
using BookStoreModel.GeneralModels;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.ItemInvoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.ItemInvoices
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class SharedItemInvoicesController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        //View page addresses
        private const string ROW_PARTIAL = "~/Views/ItemInvoices/Shared/_ItemRow.cshtml";

        public SharedItemInvoicesController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        public ActionResult GetRow(CreateEditItemViewModel row)
        {
            //Create book-editions and stores select list
            CreateSelectLists(storeId:row.StoreId, bookEditionId:row.BookEditionId);

            return PartialView(ROW_PARTIAL,row);
        }

        #region Helpers

        //Create the select lists with parameters for select data
        private void CreateSelectLists(long? storeId = null,long? bookEditionId=null)
        {
            //Get all stores and make them for slectlist and map to keyvaluepair for the view
            ViewBag.Stores = new SelectList(
                bookStoreUnitOfWork.Stores.GetAllNoTracking().Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Name,
                }),
                "Key", "Value",storeId);

            //Get all book-editions and make them for slectlist and map to keyvaluepair for the view
            ViewBag.BookEditions = new SelectList(
                bookStoreUnitOfWork.BookEditions.GetAllNoTrackingWithIncludes(includeBook: true)
                .Select(c => new KeyValuePairViewModel<long, string>
                {
                    Key = c.Id,
                    Value = c.Id.ToString() + ": " + c.Book.Name + " (" + c.EditionInNumber + " - " + c.EditionInString + ")",
                }),
                "Key", "Value",bookEditionId);
        }

        #endregion
    }
}