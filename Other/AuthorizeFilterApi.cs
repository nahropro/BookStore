using BookStore.Data;
using BookStore.Data.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace BookStore.Other
{
    public class AuthorizeFilterApi:AuthorizeAttribute
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public AuthorizeFilterApi():base()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //Get current userid
            string userId = HttpContext.Current.User.Identity.GetUserId();
            //Get User
            ApplicationUser user = bookStoreUnitOfWork.Users.SingleOrDefaultNoTracking(u => u.Id == userId);

            if (user != null)
            {
                if (user.LockoutEnabled)
                {
                    return false;
                }
            }

            return base.IsAuthorized(actionContext);
        }
    }
}