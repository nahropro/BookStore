using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.SpendIncomeReports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class SpendItemRepo:Repository<SpendItem>
    {
        public SpendItemRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        private IQueryable<SpendItem> GetQueryableWithIncludes(bool includeInvoice = false, bool includeType = false,
            bool includeCustomer = false, bool includeVault = false)
        {
            IQueryable<SpendItem> invoice;

            invoice = GetQueryable();

            if (includeInvoice)
            {
                invoice = invoice.Include(i => i.Invoice);
            }

            if (includeType)
            {
                invoice = invoice.Include(i => i.SpendType);
            }

            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Invoice.Customer);
            }

            if (includeVault)
            {
                invoice = invoice.Include(i => i.Invoice.Vault);
            }

            return invoice;
        }

        public async Task<List<SpendIncomeReport>> GetSpendReportAsync(SpendIncomeReportFilter filter)
        {
            IQueryable<SpendItem> items;
            List<SpendIncomeReport> result;

            //Get items include type and invoice 
            items = GetQueryableWithIncludes(true, true);

            //Filter 
            items = items.Where(i =>
                (filter.StartDate.HasValue ? i.Invoice.InvoiceDate >= filter.StartDate.Value : true) &&
                (filter.EndDate.HasValue ? i.Invoice.InvoiceDate <= filter.EndDate.Value : true) &&
                filter.TypeIds.Contains(i.SpendTypeId)
            );

            result = await items.Select(i => new SpendIncomeReport
            {
                Amount = i.Amount,
                InvoiceDate = i.Invoice.InvoiceDate,
                InvoiceId = i.InvoiceId,
                Type = new SpendIncomeTypeViewModel
                {
                    Id = i.SpendType.Id,
                    Name = i.SpendType.Name,
                }
            }).ToListAsync();

            if (filter.GroupByType)
            {
                result = result.GroupBy(i => i.Type.Id).Select(i => new SpendIncomeReport
                {
                    Type = i.FirstOrDefault()?.Type,
                    Amount = i.Select(s => s.Amount).DefaultIfEmpty(0).Sum(),
                }).ToList();
            }

            return result;
        }
    }
}