using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.CustomerToCustomerInvoice;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class CustomerToCustomerInvoiceRpo:Repository<CustomerToCustomerInvoice>
    {
        public CustomerToCustomerInvoiceRpo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public CustomerToCustomerInvoice Add(CreateEditCustomerToCustomerInvooiceViewModel model,string userId)
        {
            //Map viewmodel to the model
            CustomerToCustomerInvoice invoice = new CustomerToCustomerInvoice
            {
                PayCustomerId = model.PayCustomerId,
                GiveCustomerId = model.GiveCustomerId,
                Amount = model.Amount,
                Note = model.Note,
                InvoiceDate = model.InvoiceDate,
                CreatorUserId = userId,
            };

            //Add using base function
            return Add(invoice);
        }

        public override CustomerToCustomerInvoice Add(CustomerToCustomerInvoice entity)
        {
            //Check if amount greater than zero,
            //And paycustomer and givecustomer is not the same
            if (entity.Amount > 0 && entity.PayCustomerId != entity.GiveCustomerId)
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
        public async Task<List<CustomerToCustomerInvoice>> FilterNoTrackingWithIncludesAsync
            (CustomerToCustomerInvoiceFilter filter = null, bool includePayCustomer = false, bool includeGiveCustomer = false)
        {
            IQueryable<CustomerToCustomerInvoice> invoice;

            invoice = GetQueryable();

            //Check if pay customer includes
            if (includePayCustomer)
            {
                invoice = invoice.Include(i => i.PayCustomer);
            }

            //Check if give customer includes
            if (includeGiveCustomer)
            {
                invoice = invoice.Include(i => i.GiveCustomer);
            }

            //If filte object is not null
            if (filter != null)
            {
                //Check for filters if not null,
                //Then filter invoices accourding filter property
                invoice = invoice.Where(i =>
                    (filter.InvoiceId.HasValue ? i.Id == filter.InvoiceId.Value :
                    (filter.PayCustomerId.HasValue ? i.PayCustomerId == filter.PayCustomerId.Value : true) &&
                    (filter.GiveCustomerId.HasValue ? i.GiveCustomerId == filter.GiveCustomerId.Value : true) &&
                    (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                    (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                );
            }

            //Rerurn filtered invoices and make it notracking and convert it to list
            return await invoice.AsNoTracking().ToListAsync();
        }

        public CustomerToCustomerInvoice Edit(CreateEditCustomerToCustomerInvooiceViewModel model, string userId)
        {
            //Map the viewmodel to model
            CustomerToCustomerInvoice invoice = new CustomerToCustomerInvoice
            {
                Id = model.Id,
                PayCustomerId = model.PayCustomerId,
                GiveCustomerId = model.GiveCustomerId,
                Amount = model.Amount,
                Note = model.Note,
                InvoiceDate = model.InvoiceDate,
                EditorUserId = userId,
            };

            return Edit(invoice);
        }

        public override CustomerToCustomerInvoice Edit(CustomerToCustomerInvoice entity)
        {
            CustomerToCustomerInvoice model;

            //Check for some validations
            //Amount must be greater than zero
            //Edited userid must not empty
            if (entity.Amount > 0 && entity.EditorUserId != null && entity.PayCustomerId != entity.GiveCustomerId)
            {
                //Get the invoice from database
                model = Get(entity.Id);

                //Update necessary datas only, not allow others
                model.PayCustomerId = entity.PayCustomerId;
                model.GiveCustomerId = entity.GiveCustomerId;
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