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
    public class LostInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public LostInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public LostInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectLostInvoiceViewModel>> GetAllNoTrackingAsync(ItemInvoiceFilter filter = null)
        {
            List<SelectLostInvoiceViewModel> result;
            List<LostInvoice> invoices;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            invoices = await bookStoreUnitOfWork.LostInvoices
                .FilterNoTrackingWithIncludesAsync(filter: filter);

            //Add creator userids for userIds collection
            userIds.AddRange(invoices.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map invoice to select view model
            result = invoices.Select(i => new SelectLostInvoiceViewModel
            {
                Id = i.Id,
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

        public async Task<LostInvoice> Edit(CreateEditLostInvoiceViewModel model, string userId)
        {
            LostInvoice invoice;

            //If userId is null throw bad userid
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Bad UserId");
            }

            //Get the invoice with items
            invoice = await bookStoreUnitOfWork.LostInvoices.GetWithIncludesAsync(model.Id, includeItems: true);

            if (invoice != null)
            {
                //Remove all items, then after add edited items
                bookStoreUnitOfWork.LostItems.RemoveRange(invoice.Items);

                //Change properties
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.Note = model.Note;
                invoice.Items = model.Items.Select(i => new LostItem
                {
                    BookEditionId = i.BookEditionId,
                    Qtt = i.Qtt,
                    Price = i.Price,
                    StoreId = i.StoreId,
                    Total = i.Price * i.Qtt,
                }).ToList();    //Set new items for adding
                //Add total of items
                invoice.Total = invoice.Items.Sum(i => i.Total);
                invoice.EditorUserId = userId;  //Set editor userId
                invoice.LastEditedDateTime = DateTime.UtcNow;   //Set edited time
            }
            else
            {
                throw new Exception("Not found for editing");
            }

            return invoice;
        }
    }
}