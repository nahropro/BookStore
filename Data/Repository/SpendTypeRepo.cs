using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Data.Repository
{
    public class SpendTypeRepo:Repository<SpendType>
    {
        public SpendTypeRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }
    }
}