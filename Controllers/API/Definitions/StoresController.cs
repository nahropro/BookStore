using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.APIModels.Stores;
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
    public class StoresController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public StoresController()
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            List<StoreAPI> stores;

            //Get all stores and map it to api model
            stores = (await bookStoreUnitOfWork.Stores.GetAllNoTrackingAsync())
                .Select(s => new StoreAPI
                {
                    Address = s.Address,
                    Id = s.Id,
                    Name = s.Name,
                }).ToList();


            return Ok(stores);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(long id)
        {
            StoreAPI store;

            //Get the store and map it to api model
            store =await bookStoreUnitOfWork.Stores.SingleOrDefaultNoTrackingAsync(s=> s.Id==id);


            return Ok(store);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(long id)
        {
            try
            {   
                await this.bookStoreUnitOfWork.Stores.RemoveAsync(id);
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
