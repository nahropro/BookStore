using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.GiveInvoice;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class GiveInvoiceRepo:Repository<GiveInvoice>
    {
        public GiveInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {
            
        }

        public GiveInvoice Add(CreateEditGiveInvoiceViewModel model,string userId)
        {
            //Map CreateGiveInvoiceViewModel to GiveInvoice model
            GiveInvoice giveInvoice = new GiveInvoice
            {
                Amount = model.Amount,
                CreatorUserId = userId,
                CustomerId = model.CustomerId,
                Discount = model.Discount,
                InvoiceDate = model.InvoiceDate,
                VaultId = model.VaultId,
                AmountNote=model.AmountNote,
                DiscountNote=model.DiscountNote,
            };
            
            //Add this using add function
            return Add(giveInvoice);
        }

        public override GiveInvoice Add(GiveInvoice entity)
        {
            //Check if amount and discount is positive numbers
            if (entity.Amount>=0 && entity.Discount>=0 && (entity.Amount+entity.Discount)>0)
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

        //Get single giveinvoice with includes
        public async Task<GiveInvoice> SingleOrDefaultNoTrackingWithIncludesAsync(
            Expression<Func<GiveInvoice, bool>> expression,bool includeCustomer=false,bool includeVault=false)
        {
            IQueryable<GiveInvoice> giveInvoices;

            giveInvoices = GetQueryable();

            //If include customer
            if (includeCustomer)
            {
                giveInvoices = giveInvoices.Include(gi => gi.Customer);
            }

            //If include vault
            if (includeVault)
            {
                giveInvoices = giveInvoices.Include(gi => gi.Vault);
            }

            return await giveInvoices.SingleOrDefaultAsync(expression);
        }
        
        //Get giveinvoices filter it with filter model and with includes
        public async Task<List<GiveInvoice>> FilterNoTrackingWithIncludesAsync
            (PayGiveInvoiceFilter filter=null,bool includeCustomer = false, bool includeVault = false)
        {
            IQueryable<GiveInvoice> giveInvoices=GetQueryable();

            //Check if includes customer is ture, then include it
            if (includeCustomer)
            {
                giveInvoices = giveInvoices.Include(gi => gi.Customer);
            }

            //Check if includes vault is ture, then include it
            if (includeVault)
            {
                giveInvoices = giveInvoices.Include(gi => gi.Vault);
            }

            //If filter is not null
            if (filter!=null)
            {
                //Check for filters if not null,
                //Then filter giveinvoices accourding filter property
                giveInvoices = giveInvoices.Where(g =>
                    (filter.InvoiceId.HasValue?g.Id==filter.InvoiceId.Value:
                    (filter.CustomerId.HasValue ? g.CustomerId == filter.CustomerId.Value : true) &&
                    (filter.VaultId.HasValue ? g.VaultId == filter.VaultId.Value : true) &&
                    (filter.StartDate.HasValue ? g.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? g.InvoiceDate <= filter.EndDate.Value : true)
                    )
                );
            }

            return await giveInvoices.AsNoTracking().ToListAsync();
        }

        public GiveInvoice Edit(CreateEditGiveInvoiceViewModel model,string userId)
        {
            GiveInvoice giveInvoice = new GiveInvoice
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

            return Edit(giveInvoice);
        }

        public override GiveInvoice Edit(GiveInvoice entity)
        {
            GiveInvoice realModel;

            //Check if amount and discount is positive numbers and editor userid not null
            if (entity.Amount >= 0 && entity.Discount >= 0 && 
                entity.EditorUserId!=null && (entity.Amount + entity.Discount) > 0)
            {
                

                //Get the giveinvoice from database
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