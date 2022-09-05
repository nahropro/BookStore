using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.VaultReports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class VaultRepo:Repository<Vault>
    {
        public VaultRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<List<VaultBalanceViewModel>> GetBalancesAsync(VaultBalanceFilter filter=null)
        {
            IQueryable<Vault> vaults;
            List<VaultBalanceViewModel> balannces;

            //Get queryable
            vaults = GetQueryable();

            //Setup neccessary includes
            vaults = vaults.Include(i => i.PayInvoices)
                .Include(i => i.GiveInvoices)
                .Include(i => i.VaultToVaultInvoicePays)
                .Include(i => i.VaultToVaultInvoiceGives);

            //Map to balance view-model and caclualte balance
            balannces =await vaults.Select(i => new VaultBalanceViewModel
            {
                VaultId=i.Id,
                VaultName=i.Name,
                InCash=i.GiveInvoices.Where(s=> (filter.UntilDate.HasValue?s.InvoiceDate<=filter.UntilDate.Value:true))
                        .Select(s=> s.Amount).DefaultIfEmpty(0).Sum() +
                    i.VaultToVaultInvoiceGives.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s=> s.Amount).DefaultIfEmpty(0).Sum() +
                    i.IncomeInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Cash).DefaultIfEmpty(0).Sum() +
                    i.VaultCorrectionInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true) &&
                        s.CorrectionType==VaultCorrectionInvoice.VaulltCorrectionType.Increase)
                            .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    (i.FirstAmount>0? i.FirstAmount:0),
                OutCash= i.PayInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    i.VaultToVaultInvoicePays.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    i.SpendInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Cash).DefaultIfEmpty(0).Sum() +
                    i.VaultCorrectionInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true) &&
                        s.CorrectionType == VaultCorrectionInvoice.VaulltCorrectionType.Decrease)
                            .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    (i.FirstAmount < 0 ? i.FirstAmount*(-1) : 0),
            }).AsNoTracking().ToListAsync();

            return balannces;
        }

        public async Task<decimal> GetBalanceUntilAsync(long vaultId,DateTime? UntilDate = null)
        {
            IQueryable<Vault> vaults;
            decimal balannce;

            //Get queryable
            vaults = GetQueryable();

            //Setup neccessary includes
            vaults = vaults.Include(i => i.PayInvoices)
                .Include(i => i.GiveInvoices)
                .Include(i => i.VaultToVaultInvoicePays)
                .Include(i => i.VaultToVaultInvoiceGives);

            //Calculate balance until-date
            balannce = (await vaults.Select(i => new VaultBalanceViewModel
            {
                VaultId = i.Id,
                VaultName = i.Name,
                InCash = i.GiveInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                         .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     i.VaultToVaultInvoiceGives.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                         .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     i.IncomeInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                        .Select(s => s.Cash).DefaultIfEmpty(0).Sum() +
                    i.VaultCorrectionInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true) &&
                        s.CorrectionType == VaultCorrectionInvoice.VaulltCorrectionType.Increase)
                            .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     (i.FirstAmount > 0 ? i.FirstAmount : 0),
                OutCash = i.PayInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                         .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     i.VaultToVaultInvoicePays.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                         .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     i.SpendInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true))
                        .Select(s => s.Cash).DefaultIfEmpty(0).Sum() +
                    i.VaultCorrectionInvoices.Where(s => (UntilDate.HasValue ? s.InvoiceDate <= UntilDate.Value : true) &&
                        s.CorrectionType == VaultCorrectionInvoice.VaulltCorrectionType.Decrease)
                            .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                     (i.FirstAmount < 0 ? i.FirstAmount * (-1) : 0),
            }).AsNoTracking().SingleOrDefaultAsync(i=> i.VaultId==vaultId)).Balance;

            return balannce;
        }
    }
}