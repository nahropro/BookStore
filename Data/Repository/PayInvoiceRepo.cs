using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.PayInvoice;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class PayInvoiceRepo:Repository<PayInvoice>
    {
        public PayInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {
        }

        public PayInvoice Add(CreateEditPayInvoiceViewModel model, string userId)
        {
            //Map CreateEditPayInvoiceViewModel to PayInvoice model
            PayInvoice payInvoice = new PayInvoice
            {
                Amount = model.Amount,
                CreatorUserId = userId,
                CustomerId = model.CustomerId,
                Discount = model.Discount,
                InvoiceDate = model.InvoiceDate,
                VaultId = model.VaultId,
                AmountNote = model.AmountNote,
                DiscountNote = model.DiscountNote,
            };
            
            //Add this using add function
            return Add(payInvoice);
        }

        public override PayInvoice Add(PayInvoice entity)
        {
            //Check if amount and discount is positive numbers
            if (entity.Amount >= 0 && entity.Discount >= 0 && (entity.Amount + entity.Discount) > 0)
            {
                //Calculate total and store it
                entity.Total = entity.Amount + entity.Discount;
                //Get creation datetime
                entity.CreationDateTime = DateTime.UtcNow;

                //Prevent add unnecessary datas
                entity.LastEditedDateTime = null;
                entity.EditorUserId = null;

                //Excute base class function
                return base.Add(entity);
            }
            else
            {
                //Throw exception with incorrect data
                throw new Exception("Incorrect data");
            }
        }

        //Get single invoice with includes
        public async Task<PayInvoice> SingleOrDefaultNoTrackingWithIncludesAsync(
            Expression<Func<PayInvoice, bool>> expression, bool includeCustomer = false, bool includeVault = false)
        {
            IQueryable<PayInvoice> payInvoice;

            payInvoice = GetQueryable();

            //If include customer
            if (includeCustomer)
            {
                payInvoice = payInvoice.Include(i => i.Customer);
            }

            //If include vault
            if (includeVault)
            {
                payInvoice = payInvoice.Include(i => i.Vault);
            }

            return await payInvoice.SingleOrDefaultAsync(expression);
        }

        //Get invoice, filter it with filter model and with includes
        public async Task<List<PayInvoice>> FilterNoTrackingWithIncludesAsync
            (PayGiveInvoiceFilter filter = null, bool includeCustomer = false, bool includeVault = false)
        {
            IQueryable<PayInvoice> invoice = GetQueryable();

            //Check if includes customer is ture, then include it
            if (includeCustomer)
            {
                invoice = invoice.Include(gi => gi.Customer);
            }

            //Check if includes vault is ture, then include it
            if (includeVault)
            {
                invoice = invoice.Include(gi => gi.Vault);
            }

            //If filter is not null
            if (filter != null)
            {
                //Check for filters if not null,
                //Then filter invoices accourding filter property
                invoice = invoice.Where(g =>
                    (filter.InvoiceId.HasValue ? g.Id == filter.InvoiceId.Value :
                    (filter.CustomerId.HasValue ? g.CustomerId == filter.CustomerId.Value : true) &&
                    (filter.VaultId.HasValue ? g.VaultId == filter.VaultId.Value : true) &&
                    (filter.StartDate.HasValue ? g.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? g.InvoiceDate <= filter.EndDate.Value : true)
                    )
                );
            }

            return await invoice.AsNoTracking().ToListAsync();
        }

        public PayInvoice Edit(CreateEditPayInvoiceViewModel model, string userId)
        {
            //Map editviewmodel to invoice
            PayInvoice invoice = new PayInvoice
            {
                Amount = model.Amount,
                AmountNote = model.AmountNote,
                CustomerId = model.CustomerId,
                Discount = model.Discount,
                DiscountNote = model.DiscountNote,
                Id = model.Id,
                InvoiceDate = model.InvoiceDate,
                EditorUserId = userId,
                VaultId = model.VaultId,
            };

            //using base edit function
            return Edit(invoice);
        }

        public override PayInvoice Edit(PayInvoice entity)
        {
            PayInvoice realModel;

            //Check if amount and discount is positive numbers and editor userid not null
            if (entity.Amount >= 0 && entity.Discount >= 0 &&
                entity.EditorUserId != null && (entity.Amount + entity.Discount) > 0)
            {


                //Get the invoice from database
                realModel = Get(entity.Id);

                //Update necessary datas only, not allow others
                realModel.Amount = entity.Amount;
                realModel.AmountNote = entity.AmountNote;
                realModel.CustomerId = entity.CustomerId;
                realModel.Discount = entity.Discount;
                realModel.DiscountNote = entity.DiscountNote;
                realModel.InvoiceDate = entity.InvoiceDate;
                realModel.EditorUserId = entity.EditorUserId;
                realModel.VaultId = entity.VaultId;

                //Calculate total and store it
                realModel.Total = entity.Amount + entity.Discount;
                //Get the datetime of edited
                realModel.LastEditedDateTime = DateTime.UtcNow;

                //Return the realmodel
                return realModel;
            }
            else
            {
                //Throw exception with incorrect data
                throw new Exception("Incorrect data");
            }
        }
    }
}