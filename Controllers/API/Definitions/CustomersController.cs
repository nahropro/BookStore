using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.APIModels.Customers;
using BookStoreModel.Models;
using BookStoreModel.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookStore.Controllers.API.Definitions
{
    [AuthorizeFilterApi(Roles = RoleNames.MANAGER + "," + RoleNames.ADMIN)]
    public class CustomersController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public CustomersController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            List<CustomerAPI> customers;

            //Get customer and map it to api model
            customers = (await bookStoreUnitOfWork.Customers.GetAllNoTrackingAsync())
                .Select(c => new CustomerAPI
                {
                    Active = c.Active,
                    Address = c.Address,
                    FirstTimeBalance = c.FirstTimeBalance,
                    FullName = c.FullName,
                    Id = c.Id,
                    Phone = c.Phone,
                    WorkPlace = c.WorkPlace,
                }).ToList();

            return Ok(customers);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(long id)
        {
            CustomerAPI customer;

            //Get customer and map it to api model
            customer = await bookStoreUnitOfWork.Customers.SingleOrDefaultNoTrackingAsync(c => c.Id == id);

            return Ok(customer);
        }


        [HttpDelete]
        public async Task<IHttpActionResult> Delete(long id)
        {
            try
            {
                await this.bookStoreUnitOfWork.Customers.RemoveAsync(id);
                await this.bookStoreUnitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
