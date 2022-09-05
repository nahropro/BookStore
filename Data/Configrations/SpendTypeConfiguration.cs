using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class SpendTypeConfiguration : EntityTypeConfiguration<SpendType>
    {
        public SpendTypeConfiguration()
        {
            //Set index and unique for store name
            this.HasIndex(s => s.Name)
                .IsUnique();
        }
    }
}