using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Data.Repository
{
    public class UserRepo:Repository<ApplicationUser>
    {
        public UserRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }
    }
}