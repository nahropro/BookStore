using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.VaultToVaultInvoice;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class VaultToVaultInvoiceRepo:Repository<VaultToVaultInvoice>
    {
        public VaultToVaultInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public VaultToVaultInvoice Add(CreateEditVaultToVaultInvoiceViewModel model,string userId)
        {
            //Map viewmodel to the model
            VaultToVaultInvoice invoice = new VaultToVaultInvoice
            {
                PayVaultId = model.PayVaultId,
                GiveVaultId = model.GiveVaultId,
                Amount = model.Amount,
                InvoiceDate = model.InvoiceDate,
                Note = model.Note,
                CreatorUserId = userId,
            };

            //Add using base function
            return Add(invoice);
        }

        public override VaultToVaultInvoice Add(VaultToVaultInvoice entity)
        {
            //Check if amount greater than zero,
            //And PayVault and GiveVault is not the same
            if (entity.Amount>0 && entity.PayVaultId!=entity.GiveVaultId)
            {
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
                throw new Exception("Invalid data");
            }
        }

        //Get invoice, filter it with filter model and with includes
        public async Task<List<VaultToVaultInvoice>> FilterNoTrackingWithIncludesAsync
            (VaultToVaultInvoiceFilter filter = null, bool includePayVault = false, bool includeGiveVault = false)
        {
            IQueryable<VaultToVaultInvoice> invoice;

            invoice = GetQueryable();

            //Check if pay vault includes
            if (includePayVault)
            {
                invoice = invoice.Include(i => i.PayVault);
            }

            //Check if give vault includes
            if (includeGiveVault)
            {
                invoice = invoice.Include(i => i.GiveVault);
            }

            //If filte object is not null
            if (filter!=null)
            {
                //Check for filters if not null,
                //Then filter invoices accourding filter property
                invoice = invoice.Where(i =>
                    (filter.InvoiceId.HasValue?i.Id==filter.InvoiceId.Value:
                    (filter.PayVaultId.HasValue ? i.PayVaultId == filter.PayVaultId.Value : true) &&
                    (filter.GiveVaultId.HasValue ? i.GiveVaultId == filter.GiveVaultId.Value : true) &&
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                );
            }

            //Rerurn filtered invoices and make it notracking and convert it to list
            return await invoice.AsNoTracking().ToListAsync();
        }

        public VaultToVaultInvoice Edit(CreateEditVaultToVaultInvoiceViewModel model,string userId)
        {
            //Map the viewmodel to model
            VaultToVaultInvoice invoice = new VaultToVaultInvoice
            {
                Id = model.Id,
                PayVaultId = model.PayVaultId,
                GiveVaultId = model.GiveVaultId,
                Amount = model.Amount,
                Note = model.Note,
                InvoiceDate = model.InvoiceDate,
                EditorUserId = userId,
            };

            return Edit(invoice);
        }

        public override VaultToVaultInvoice Edit(VaultToVaultInvoice entity)
        {
            VaultToVaultInvoice model;

            //Check for some validations
            //Amount must be greater than zero
            //Edited userid must not empty
            if (entity.Amount>0 && entity.EditorUserId!=null && entity.PayVaultId != entity.GiveVaultId)
            {
                //Get the invoice from database
                model = Get(entity.Id);

                //Update necessary datas only, not allow others
                model.PayVaultId = entity.PayVaultId;
                model.GiveVaultId = entity.GiveVaultId;
                model.Amount = entity.Amount;
                model.Note = entity.Note;
                model.InvoiceDate = entity.InvoiceDate;
                model.EditorUserId = entity.EditorUserId;

                //Get the datetime of edited
                model.LastEditedDateTime = DateTime.UtcNow;

                //Return the model
                return model;
            }

            throw new Exception("Invalid data");
        }
    }
}