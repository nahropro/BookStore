using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.Shared;
using BookStoreModel.ViewModels.SpendIncome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class SpendInvoiceManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public SpendInvoiceManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public SpendInvoiceManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectSpendIncomeInvoiceViewModel>> GetAllNoTrackingAsync(SpendIncomeFilter filter = null)
        {
            List<SelectSpendIncomeInvoiceViewModel> result;
            List<SpendInvoice> invoices;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            invoices = await bookStoreUnitOfWork.SpendInvoices
                .FilterNoTrackingWithIncludesAsync(filter: filter, includeCustomer: true, includeVault:true);

            //Add creator userids for userIds collection
            userIds.AddRange(invoices.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(invoices.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map invoice to select view model
            result = invoices.Select(i => new SelectSpendIncomeInvoiceViewModel
            {
                Id = i.Id,
                Customer = i.Customer,
                Vault = i.Vault,
                InvoiceDate = i.InvoiceDate,
                Total = i.Total,
                Cash = i.Cash,
                Loan = i.Loan,
                MoreInfo = new SpendIncomeInvoiceMoreInfoViewModel
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

        public async Task<SpendInvoice> Edit(CreateEditSpendIncomeInvoiceViewModel model, string userId)
        {
            SpendInvoice invoice;

            //If userId is null throw bad userid
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Bad UserId");
            }

            //Get the invoice with items
            invoice = await bookStoreUnitOfWork.SpendInvoices.GetWithIncludesAsync(model.Id, includeItems: true);

            if (invoice != null &&
                invoice.Items.LongCount() > 0 &&
                invoice.Items.All(i => i.Amount > 0) &&
                invoice.Loan + invoice.Cash == invoice.Items.Select(i => i.Amount).DefaultIfEmpty(0).Sum() &&
                invoice.Loan >= 0 &&
                invoice.Cash >= 0 &&
                (invoice.Loan > 0 ? invoice.CustomerId.HasValue : true) &&
                (invoice.Cash > 0 ? invoice.VaultId.HasValue : true))
            {
                //Remove all items, then after add edited items
                bookStoreUnitOfWork.SpendItems.RemoveRange(invoice.Items);

                //Change properties
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.Note = model.Note;
                invoice.Loan = model.Loan.GetValueOrDefault();
                invoice.CustomerId = model.CustomerId;
                invoice.Cash = model.Cash.GetValueOrDefault();
                invoice.VaultId = model.VaultId;
                invoice.Items = model.Items.Select(i => new SpendItem
                {
                    Amount=i.Amount,
                    Note=i.Note,
                    SpendTypeId=i.TypeId,
                }).ToList();    //Set new items for adding
                //Add total of items subtrackt it with discount
                invoice.Total = invoice.Items.Sum(i => i.Amount);
                invoice.EditorUserId = userId;  //Set editor userId
                invoice.LastEditedDateTime = DateTime.UtcNow;
            }
            else
            {
                throw new Exception("Invalid data");
            }

            return invoice;
        }
    }
}