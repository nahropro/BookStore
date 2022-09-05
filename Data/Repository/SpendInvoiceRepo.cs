using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class SpendInvoiceRepo:Repository<SpendInvoice>
    {
        public SpendInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public override SpendInvoice Add(SpendInvoice entity)
        {
            //Calculate invoice total
            entity.Total = entity.Items.Select(i => i.Amount).DefaultIfEmpty(0).Sum();

            //Make sures invoce has at least one item,
            //all itemes has non zero positive amount,
            //cash and loan is equal to total,
            //Loan and Cash must be positive or zero
            //if there is any loan then has customer-id and
            //if there is any cash then has vault-id
            if (entity.Items.LongCount()>0 &&
                entity.Items.All(i=> i.Amount>0) && 
                entity.Loan+entity.Cash==entity.Total &&
                entity.Loan>=0 &&
                entity.Cash>=0 &&
                (entity.Loan>0 ? entity.CustomerId.HasValue:true) &&
                (entity.Cash>0 ? entity.VaultId.HasValue:true))
            {
                //Prevent customer if loand is zero
                if (entity.Loan==0)
                {
                    entity.CustomerId = null;
                }

                //Prevent vault if chash is zero
                if (entity.Cash == 0)
                {
                    entity.VaultId = null;
                }

                //Get creation datetime
                entity.CreationDateTime = DateTime.UtcNow;

                //Prevent add unnecessary datas
                entity.LastEditedDateTime = null;
                entity.EditorUserId = null;

                return base.Add(entity);
            }
            else
            {
                throw new Exception("Invalid data");
            }
        }

        public override IEnumerable<SpendInvoice> AddRange(IEnumerable<SpendInvoice> entities)
        {
            List<SpendInvoice> result = new List<SpendInvoice>();

            foreach (var entity in entities)
            {
                result.Add(Add(entity));
            }

            return result;
        }

        public async Task<List<SpendInvoice>> FilterNoTrackingWithIncludesAsync(SpendIncomeFilter filter, bool includeCustomer=false,
            bool includeVault=false, bool includeItems=false, bool includeSpendType = false)
        {
            IQueryable<SpendInvoice> invoice;

            invoice = GetQueryable();

            //If include customer
            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Customer);
            }

            //If include vault
            if (includeVault)
            {
                invoice = invoice.Include(i => i.Vault);
            }

            //If include item
            if (includeItems)
            {
                invoice = invoice.Include(i => i.Items);
            }

            //If include spend-type
            if (includeSpendType)
            {
                invoice = invoice.Include(i => i.Items.Select(s => s.SpendType));
            }

            //Filtering
            if (filter!=null)
            {
                invoice = invoice.Where(i =>
                    (filter.InvoiceId.HasValue ? i.Id == filter.InvoiceId.Value :
                        (filter.CustomerId.HasValue ? i.CustomerId == filter.CustomerId.Value : true) &&
                        (filter.VaultId.HasValue ? i.VaultId == filter.VaultId.Value : true) &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                );
            }

            return await invoice.AsNoTracking().ToListAsync();
        }

        private IQueryable<SpendInvoice> GetQueryableWithIncludesAsync(
            bool includeCustomer = false, bool includeVault = false, bool includeItems = false, bool includeSpendType = false)
        {
            IQueryable<SpendInvoice> invoice;

            invoice = GetQueryable();

            //If include customers
            if (includeCustomer)
            {
                invoice = invoice.Include(i => i.Customer);
            }

            //Include items
            if (includeItems)
            {
                invoice = invoice.Include(i => i.Items);
            }

            //Include vaults
            if (includeVault)
            {
                invoice = invoice.Include(i => i.Vault);
            }

            if (includeSpendType)
            {
                invoice = invoice.Include(i => i.Items.Select(s => s.SpendType));
            }

            return invoice;
        }

        public async Task<SpendInvoice> GetWithIncludesAsync(long id,
            bool includeCustomer = false, bool includeVault = false, bool includeItems = false, bool includeSpendType = false)
        {
            return await GetQueryableWithIncludesAsync(includeCustomer: includeCustomer, includeVault: includeVault,
                includeItems: includeItems, includeSpendType: includeSpendType).SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<SpendInvoice> GetNoTrackingWithIncludesAsync(long id,
            bool includeCustomer = false, bool includeVault = false, bool includeItems = false, bool includeSpendType = false)
        {
            return await GetQueryableWithIncludesAsync(includeCustomer: includeCustomer, includeVault: includeVault,
                includeItems: includeItems, includeSpendType: includeSpendType).AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
        }
    }
}