using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.GiveInvoice;
using BookStoreModel.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class GiveInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public GiveInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public GiveInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectGiveInvoiceViewModel>> GetAllNoTrackingAsync(PayGiveInvoiceFilter filter=null)
        {
            List<SelectGiveInvoiceViewModel> result;
            List<GiveInvoice> giveInvoices;
            List<ApplicationUser> users;
            List<string> userIds=new List<string>();
        
            //Get all giveinvoices with include nessesary objects and filtered if has any
            giveInvoices =await bookStoreUnitOfWork.GiveInvoices
                .FilterNoTrackingWithIncludesAsync(filter:filter,includeCustomer: true, includeVault: true);

            //Add creator userids for userIds collection
            userIds.AddRange(giveInvoices.Select(gi => gi.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(giveInvoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary load
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users =await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map give invoice to selectgiveinvoice
            result = giveInvoices.Select(i => new SelectGiveInvoiceViewModel
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
                },
            }).ToList();

            return result;
        }
    }
}