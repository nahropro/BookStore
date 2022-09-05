using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Data.Repository
{
    public class RoleRepo:Repository<IdentityRole>
    {
        public RoleRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }
    }
}