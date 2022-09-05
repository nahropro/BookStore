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
    public class VaultCorrectionInvoiceRepo:Repository<VaultCorrectionInvoice>
    {
        public VaultCorrectionInvoiceRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public override VaultCorrectionInvoice Add(VaultCorrectionInvoice entity)
        {
            if (entity.Amount>0)
            {
                //Get creation datetime
                entity.CreationDateTime = DateTime.UtcNow;

                //Prevent add unnecessary datas
                entity.LastEditedDateTime = null;
                entity.EditorUserId = null;

                return base.Add(entity);
            }
            else
            {
                throw new Exception("Invalid data entry");
            }
        }

        public override IEnumerable<VaultCorrectionInvoice> AddRange(IEnumerable<VaultCorrectionInvoice> entities)
        {
            List<VaultCorrectionInvoice> result = new List<VaultCorrectionInvoice>();

            foreach (var item in entities)
            {
                result.Add(Add(item));
            }

            return result;
        }

        public VaultCorrectionInvoice Edit(VaultCorrectionInvoice entity,string userId)
        {
            VaultCorrectionInvoice invoice = new VaultCorrectionInvoice
            {
                Id = entity.Id,
                Amount = entity.Amount,
                CorrectionType = entity.CorrectionType,
                InvoiceDate = entity.InvoiceDate,
                Note = entity.Note,
                VaultId = entity.VaultId,
                EditorUserId = userId,
            };

            return Edit(invoice);
        }

        public override VaultCorrectionInvoice Edit(VaultCorrectionInvoice entity)
        {
            VaultCorrectionInvoice invoice;

            if (entity.Amount>0 && !string.IsNullOrWhiteSpace(entity.EditorUserId))
            {
                invoice = Get(entity.Id);

                if (invoice!=null)
                {
                    //Set data
                    invoice.VaultId = entity.VaultId;
                    invoice.Amount = entity.Amount;
                    invoice.CorrectionType = entity.CorrectionType;
                    invoice.Note = entity.Note;
                    invoice.InvoiceDate = entity.InvoiceDate;
                    invoice.EditorUserId = entity.EditorUserId;

                    //Get the datetime of edited
                    invoice.LastEditedDateTime = DateTime.UtcNow;

                    return invoice;
                }
                else
                {
                    throw new Exception("Invoice not found!");
                }
            }
            else
            {
                throw new Exception("Invalid data input");
            }
        }

        public async Task<List<VaultCorrectionInvoice>> FilterNoTrackingAsync(VaultCorrectionFilter filter,bool includeVault = false)
        {
            IQueryable<VaultCorrectionInvoice> invoice;

            invoice = GetQueryable();

            //Include vault
            if (includeVault)
            {
                invoice = invoice.Include(i => i.Vault);
            }

            //Perform filters
            invoice = invoice.Where(i =>
                      (filter.InvoiceId.HasValue ? i.Id == filter.InvoiceId.Value :
                          (filter.VaultId.HasValue ? i.VaultId == filter.VaultId.Value : true) &&
                          (filter.Type.HasValue ? i.CorrectionType == filter.Type.Value : true) &&
                          (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                          (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                      )
                  );

            return await invoice.AsNoTracking().ToListAsync();
        }
    }
}