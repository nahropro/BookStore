using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.ItemInvoices;
using BookStoreModel.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class ReturnTempSellInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public ReturnTempSellInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
        {
            if (bookStoreUnitOfWork == null)
            {
                this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
            }
            else
            {
                this.bookStoreUnitOfWork = bookStoreUnitOfWork;
            }
        }

        public ReturnTempSellInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectItemInvoiceViewModel>> GetAllNoTrackingAsync(ItemInvoiceFilter filter = null)
        {
            List<SelectItemInvoiceViewModel> result;
            List<ReturnTempSellInvoice> invoices;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            invoices = await bookStoreUnitOfWork.ReturnTempSellInvoices
                .FilterNoTrackingWithIncludesAsync(filter: filter, includeCustomer: true);

            //Add creator userids for userIds collection
            userIds.AddRange(invoices.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map invoice to select view model
            result = invoices.Select(i => new SelectItemInvoiceViewModel
            {
                Id = i.Id,
                CustomerFullName = i.Customer.FullName,
                CustomerId = i.CustomerId,
                CustomerWorkPlace = i.Customer.WorkPlace,
                Discount = i.Discount,
                InvoiceDate = i.InvoiceDate,
                Total = i.Total,
                MoreInfo = new ItemInvoiceMoreInfoViewModel
                {
                    Note = i.Note,
                    ChangeInfo = new ChangeInfoViewModel
                    {
                        CreatorUserFullName = users.SingleOrDefault(u => u.Id == i.CreatorUserId).UserExtend.FullName,
                        CreationDateTime = i.CreationDateTime,
                        CreatorUserId = i.CreatorUserId,
                        EditorUserFullName = users.SingleOrDefault(u => u.Id == i.EditorUserId)?.UserExtend.FullName,
                        EditorUserId = i.EditorUserId,
                        LastEditedDateTime = i.LastEditedDateTime,
                    },
                }
            }).ToList();

            return result;
        }

        public async Task<ReturnTempSellInvoice> Edit(CreateEditItemInvoiceViewModel model, string userId)
        {
            ReturnTempSellInvoice invoice;

            //If userId is null throw bad userid
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Bad UserId");
            }

            //Get the invoice with items
            invoice = await bookStoreUnitOfWork.ReturnTempSellInvoices.GetWithIncludesAsync(model.Id, includeItems: true);

            if (invoice != null)
            {
                //Remove all items, then after add edited items
                bookStoreUnitOfWork.ReturnTempSellItems.RemoveRange(invoice.Items);

                //Change properties
                invoice.CustomerId = model.CustomerId;
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.Discount = model.Discount;
                invoice.Note = model.Note;
                invoice.Items = model.Items.Select(i => new ReturnTempSellItem
                {
                    BookEditionId = i.BookEditionId,
                    Qtt = i.Qtt,
                    Price = i.Price,
                    StoreId = i.StoreId,
                    Total = i.Price * i.Qtt,
                }).ToList();    //Set new items for adding
                //Add total of items subtrackt it with discount
                invoice.Total = invoice.Items.Sum(i => i.Total) - invoice.Discount.GetValueOrDefault();
                invoice.EditorUserId = userId;  //Set editor userId
                invoice.LastEditedDateTime = DateTime.UtcNow;
            }
            else
            {
                throw new Exception("Not found for editing");
            }

            return invoice;
        }
    }
}