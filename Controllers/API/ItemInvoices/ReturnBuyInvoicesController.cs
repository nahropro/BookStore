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

namespace BookStore.Controllers.API.ItemInvoices
{
    [AuthorizeFilterApi(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class ReturnBuyInvoicesController : ApiController
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public ReturnBuyInvoicesController()
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(long id)
        {
            try
            {
                await this.bookStoreUnitOfWork.ReturnBuyInvoices.RemoveAsync(id);
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
