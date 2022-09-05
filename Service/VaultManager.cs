using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.VaultReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class VaultManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public VaultManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public VaultManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<VaultDetailsViewModel>> GetDetailsAsync(VaultDetailsFilter filter)
        {
            Vault vault;
            List<VaultDetailsViewModel> details = new List<VaultDetailsViewModel>();
            decimal firstTimeBalance;
            decimal balanceInTime = 0;
            
            //If filter has strat-date
            if (filter.StartDate.HasValue)
            {
                //Calulate first-time one day before start-date
                firstTimeBalance = await bookStoreUnitOfWork.Vaults.GetBalanceUntilAsync(filter.VaultId, filter.StartDate.Value.AddDays(-1));
            }
            else
            {
                //Get the vault for getting first amount
                vault = await bookStoreUnitOfWork.Vaults.SingleOrDefaultNoTrackingAsync(i => i.Id == filter.VaultId);
                firstTimeBalance = vault.FirstAmount;
            }

            //Add firt-time to the list
            details.Add(new VaultDetailsViewModel
            {
                Amount = firstTimeBalance,
                IsInOrOut = true,
            });

            //Add pay invoices
            details.AddRange((await bookStoreUnitOfWork.PayInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.VaultId == filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.AmountNote,
                    IsInOrOut = false,
                    Type = VaultDetailsViewModel.InvoiceType.Pay_Invoice,
                }).ToList());

            //Add give invoices
            details.AddRange((await bookStoreUnitOfWork.GiveInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.VaultId == filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.AmountNote,
                    IsInOrOut = true,
                    Type = VaultDetailsViewModel.InvoiceType.Give_Invoice,
                }).ToList());

            //Add spend invoices
            details.AddRange((await bookStoreUnitOfWork.SpendInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.VaultId == filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Cash,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    IsInOrOut = false,
                    Type = VaultDetailsViewModel.InvoiceType.Spend_Invoice,
                }).ToList());

            //Add income invoices
            details.AddRange((await bookStoreUnitOfWork.IncomeInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.VaultId == filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Cash,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    IsInOrOut = true,
                    Type = VaultDetailsViewModel.InvoiceType.Income_Invoice,
                }).ToList());

            //Add vault correction invoices
            details.AddRange((await bookStoreUnitOfWork.VaultCorrectionInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.VaultId == filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    IsInOrOut = (i.CorrectionType==VaultCorrectionInvoice.VaulltCorrectionType.Increase?true:false),
                    Type = VaultDetailsViewModel.InvoiceType.Vault_Correction_Invoice,
                }).ToList());

            //Add pay-transfer invoices
            details.AddRange((await bookStoreUnitOfWork.VaultToVaultInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.PayVaultId==filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    IsInOrOut = false,
                    Type = VaultDetailsViewModel.InvoiceType.Transfer,
                }).ToList());

            //Add give-transfer invoices
            details.AddRange((await bookStoreUnitOfWork.VaultToVaultInvoices.FindNoTrackingAsync(i =>
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true) &&
                    i.GiveVaultId==filter.VaultId
                )).Select(i => new VaultDetailsViewModel
                {
                    Amount = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    IsInOrOut = true,
                    Type = VaultDetailsViewModel.InvoiceType.Transfer,
                }).ToList());

            //Sort accending by invoice-date
            details = details.OrderBy(i => i.InvoiceDate).ToList();

            //Calculate in time balance
            foreach (var detail in details)
            {
                //Calculate balance-in-time
                balanceInTime = balanceInTime + (detail.Amount * (detail.IsInOrOut ? 1 : -1));

                //Set balance-in-time to detail object
                detail.BalanceInTime = balanceInTime;
            }

            return details;
        }
    }
}