using BookStore.Data.Repository;
using BookStore.Other;
using BookStoreModel.APIModels.BookEditions;
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
    public class BookEditionsController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public BookEditionsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            return NotFound();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(long id)
        {
            BookEditionAPI bookEdtion;

            //Get the book-edition with id and map it to api model
            bookEdtion = await bookStoreUnitOfWork.BookEditions
                .SingleOrDefaultNoTrackingWithIncludesAsync(i => i.Id == id, includeBook: true);

            return Ok(bookEdtion);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(long id)
        {
            try
            {
                await this.bookStoreUnitOfWork.BookEditions.RemoveAsync(id);
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
