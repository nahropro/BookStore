using BookStore.Data.Repository;
using BookStore.Other;
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
    public class BookEditionFirstTimesController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public BookEditionFirstTimesController()
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }
        

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri]long storeId, [FromUri]long bookEditionId)
        {
            BookEditionFirstTime model;

            try
            {
                //Get the item for deleting
                model = await bookStoreUnitOfWork.BookEditionFirstTimes.SingleOrDefaultAsync(i =>
                  i.StoreId == storeId && i.BookEditionId == bookEditionId);

                //Romeve item
                this.bookStoreUnitOfWork.BookEditionFirstTimes.Remove(model);
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
