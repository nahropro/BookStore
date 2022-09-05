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
    public class IncomeItemRepo:Repository<IncomeItem>
    {
        public IncomeItemRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        private IQueryable<IncomeItem> GetQueryableWithIncludes(bool includeInvoice=false, bool includeType=false,
            bool includeCustomer=false, bool includeVault=false)
        {
            IQueryable<IncomeItem> invoice;

            invoice = GetQueryable();

            if (includeInvoice)
            {
                invoice = invoice.Include(i => i.Invoice);
            }

            if (includeType)
            {
                invoice = invoice.Include(i => i.IncomeType);
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

        public async Task<List<SpendIncomeReport>> GetIncomeReportAsync(SpendIncomeReportFilter filter)
        {
            IQueryable<IncomeItem> items;
            List<SpendIncomeReport> result;

            //Get items include type and invoice 
            items = GetQueryableWithIncludes(true, true);
            
            //Filter 
            items = items.Where(i =>
                (filter.StartDate.HasValue?i.Invoice.InvoiceDate>=filter.StartDate.Value:true) &&
                (filter.EndDate.HasValue?i.Invoice.InvoiceDate<=filter.EndDate.Value:true) &&
                filter.TypeIds.Contains(i.IncomeTypeId)
            );

            result = await items.Select(i => new SpendIncomeReport
            {
                Amount = i.Amount,
                InvoiceDate = i.Invoice.InvoiceDate,
                InvoiceId = i.InvoiceId,
                Type = new SpendIncomeTypeViewModel
                {
                    Id=i.IncomeType.Id,
                    Name=i.IncomeType.Name,
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