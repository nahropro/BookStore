using BookStore.Data.Repository;
using BookStore.Other;
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
    [AuthorizeFilterApi(Roles = RoleNames.MANAGER)]
    public class UsersController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public UsersController()
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            try
            {
                await this.bookStoreUnitOfWork.Users.RemoveAsync(id);
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
