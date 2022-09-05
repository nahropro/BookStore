using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Data.Repository
{
    public class StoreRepo:Repository<Store>
    {
        public StoreRepo(BookStoreDbContext  bookStoreDbContext):base(bookStoreDbContext)
        {

        }
    }
}