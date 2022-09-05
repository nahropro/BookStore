using BookStore.Data;
using BookStore.Data.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Other
{
    public class AuthorizeFilter: AuthorizeAttribute
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public AuthorizeFilter():base()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //Get current userid
            string userId = httpContext.User.Identity.GetUserId();
            //Get User
            ApplicationUser user = bookStoreUnitOfWork.Users.SingleOrDefaultNoTracking(u => u.Id == userId);

            if (user != null)
            {
                if (user.LockoutEnabled)
                {
                    return false;
                }
            }

            return base.AuthorizeCore(httpContext);
        }
    }
}