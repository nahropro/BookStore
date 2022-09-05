using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace BookStore.Data.Configrations
{
    public class VaultConfiguration: EntityTypeConfiguration<Vault>
    {
        public VaultConfiguration()
        {
            //Set index and unique for vault name
            this.HasIndex(v => v.Name)
                 .IsUnique();
        }
    }
}