using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.CustomerToCustomerInvoice;
using BookStoreModel.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class CustomerToCustomerInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public CustomerToCustomerInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public CustomerToCustomerInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectCustomerToCustomerInvoiceViewModel>> GetAllNoTrackingAsync(CustomerToCustomerInvoiceFilter filter = null)
        {
            List<SelectCustomerToCustomerInvoiceViewModel> result;
            List<CustomerToCustomerInvoice> invoices;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            invoices = await bookStoreUnitOfWork.CustomerToCustomerInvoices
                .FilterNoTrackingWithIncludesAsync(filter: filter, includePayCustomer: true, includeGiveCustomer: true);

            //Add creator userids for userIds collection
            userIds.AddRange(invoices.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map give invoice to selectgiveinvoice
            result = invoices.Select(i => new SelectCustomerToCustomerInvoiceViewModel
            {
                Id = i.Id,
                PayCustomerFullName = i.PayCustomer.FullName,
                PayCustomerWorkPlace=i.PayCustomer.WorkPlace,
                GiveCustomerFullName = i.GiveCustomer.FullName,
                GiveCustomerWorkPlace=i.GiveCustomer.WorkPlace,
                InvoiceDate = i.InvoiceDate,
                Amount = i.Amount,
                PayCustomerId = i.PayCustomerId,
                GiveCustomerId = i.GiveCustomerId,
                MoreInfo = new VaultCustomerTransferInvoiceMoreInfoViewModel
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
                },
            }).ToList();

            return result;
        }
    }
}