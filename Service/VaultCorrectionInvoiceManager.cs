using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.Shared;
using BookStoreModel.ViewModels.VaultCorrectionInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class VaultCorrectionInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public VaultCorrectionInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public VaultCorrectionInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectVaultCorrectionInvoiceViewModel>> GetAllNoTrackingAsync(VaultCorrectionFilter filter = null)
        {
            List<SelectVaultCorrectionInvoiceViewModel> result;
            List<VaultCorrectionInvoice> invoce;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all giveinvoices with include nessesary objects and filtered if has any
            invoce = await bookStoreUnitOfWork.VaultCorrectionInvoices
                .FilterNoTrackingAsync(filter: filter, includeVault: true);

            //Add creator userids for userIds collection
            userIds.AddRange(invoce.Select(gi => gi.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoce.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary load
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map give invoice to selectgiveinvoice
            result = invoce.Select(i => new SelectVaultCorrectionInvoiceViewModel
            {
                Id = i.Id,
                Amount = i.Amount,
                InvoiceDate=i.InvoiceDate,
                Type=i.CorrectionType,
                Vault=i.Vault,
                MoreInfo = new VaultCorrectionInvoiceMoreInfoViewModel
                {
                    Note=i.Note,
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