using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Data.Repository
{
    public class IncomeTypeRepo:Repository<IncomeType>
    {
        public IncomeTypeRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }
    }
}