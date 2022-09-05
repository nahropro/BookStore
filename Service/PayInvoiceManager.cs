using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.PayInvoice;
using BookStoreModel.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class PayInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public PayInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public PayInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectPayInvoiceViewModel>> GetAllNoTrackingAsync(PayGiveInvoiceFilter filter = null)
        {
            List<SelectPayInvoiceViewModel> result;
            List<PayInvoice> invoices;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            invoices = await bookStoreUnitOfWork.PayInvoices
                .FilterNoTrackingWithIncludesAsync(filter: filter, includeCustomer: true, includeVault: true);

            //Add creator userids for userIds collection
            userIds.AddRange(invoices.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map give invoice to selectgiveinvoice
            result = invoices.Select(i => new SelectPayInvoiceViewModel
            {
                Id = i.Id,
                Amount = i.Amount,
                
                CustomerFullName = i.Customer.FullName,
                CustomerId = i.CustomerId,
                CustomerWorkPlace = i.Customer.WorkPlace,
                Discount = i.Discount,
                InvoiceDate = i.InvoiceDate,
                VaultName = i.Vault.Name,
                Total = i.Total,
                VaultId = i.VaultId,
                MoreInfo =new PayGiveInvoiceMoreInfoViewModel
                {
                    AmountNote = i.AmountNote,
                    DiscountNote = i.DiscountNote,
                    ChangeInfo=new ChangeInfoViewModel
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
    }
}