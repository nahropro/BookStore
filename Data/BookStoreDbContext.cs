using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStoreModel.Models;
using System.Data.Entity;
using BookStore.Data.Configrations;

namespace BookStore.Data
{
    public class BookStoreDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookEdition> BookEditions { get; set; }
        public DbSet<SpendType> SpendTypes { get; set; }
        public DbSet<IncomeType> IncomeTypes { get; set; }
        public DbSet<GiveInvoice> GiveInvoices { get; set; }
        public DbSet<PayInvoice> PayInvoices { get; set; }
        public DbSet<CustomerToCustomerInvoice> CustomerToCustomers { get; set; }
        public DbSet<VaultToVaultInvoice> VaultToVaultInvoices { get; set; }
        public DbSet<SellInvoice> SellInvoices { get; set; }
        public DbSet<SellItem> SellItems { get; set; }
        public DbSet<ReturnSellInvoice> ReturnSellInvoices { get; set; }
        public DbSet<ReturnSellItem> ReturnSellItems { get; set; }
        public DbSet<BuyInvoice> BuyInvoices { get; set; }
        public DbSet<BuyItem> BuyItems { get; set; }
        public DbSet<ReturnBuyInvoice> ReturnBuyInvoices { get; set; }
        public DbSet<ReturnBuyItem> ReturnBuyItems { get; set; }
        public DbSet<LostInvoice> LostInvoices { get; set; }
        public DbSet<LostItem> LostItems { get; set; }
        public DbSet<StoreTransferInvoice> StoreTransferInvoices { get; set; }
        public DbSet<StoreTransferItem> StoreTransferItems { get; set; }
        public DbSet<BookEditionFirstTime> BookEditionFirstTimes { get; set; }
        public DbSet<TempSellInvoice> TempSellInvoices { get; set; }
        public DbSet<TempSellItem> TempSellItems { get; set; }
        public DbSet<SellTempSellInvoice> SellTempSellInvoices { get; set; }
        public DbSet<SellTempSellItem> SellTempSellItems { get; set; }
        public DbSet<ReturnTempSellInvoice> ReturnTempSellInvoices { get; set; }
        public DbSet<ReturnTempSellItem> ReturnTempSellItems { get; set; }
        public DbSet<SpendInvoice> SpendInvoices { get; set; }
        public DbSet<SpendItem> SpendItems { get; set; }
        public DbSet<IncomeInvoice> IncomeInvoices { get; set; }
        public DbSet<IncomeItem> IncomeItems { get; set; }
        public DbSet<VaultCorrectionInvoice> VaultCorrectionInvoices { get; set; }

        public BookStoreDbContext()
            : base("LocalConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Add configuration classes
            modelBuilder.Configurations.Add(new BookEditionConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new VaultConfiguration());
            modelBuilder.Configurations.Add(new StoreConfiguration());
            modelBuilder.Configurations.Add(new PayInvoiceConfiguration());
            modelBuilder.Configurations.Add(new GiveInvoiceConfiguration());
            modelBuilder.Configurations.Add(new CustomerToCustomerInvoiceConfiguration());
            modelBuilder.Configurations.Add(new VaultToVaultInvoiceConfiguration());
            modelBuilder.Configurations.Add(new SellInvoiceConfiguration());
            modelBuilder.Configurations.Add(new SellItemConfiguration());
            modelBuilder.Configurations.Add(new ReturnSellInvoiceConfiguration());
            modelBuilder.Configurations.Add(new ReturnSellItemConfiguration());
            modelBuilder.Configurations.Add(new BuyInvoiceConfiguration());
            modelBuilder.Configurations.Add(new BuyItemConfiguration());
            modelBuilder.Configurations.Add(new ReturnBuyInvoiceConfiguration());
            modelBuilder.Configurations.Add(new ReturnBuyItemConfiguration());
            modelBuilder.Configurations.Add(new LostItemConfiguration());
            modelBuilder.Configurations.Add(new StoreTransferInvoiceConfiguration());
            modelBuilder.Configurations.Add(new StoreTransferItemConfiguration());
            modelBuilder.Configurations.Add(new BookEditionFirstTimeConfigurtion());
            modelBuilder.Configurations.Add(new TempSellInvoiceConfiguration());
            modelBuilder.Configurations.Add(new TempSellItemConfiguration());
            modelBuilder.Configurations.Add(new SellTempSellInvoiceConfiguration());
            modelBuilder.Configurations.Add(new SellTempSellItemConfiguration());
            modelBuilder.Configurations.Add(new ReturnTempSellInvoiceConfiguration());
            modelBuilder.Configurations.Add(new ReturnTempSellItemConfiguration());
            modelBuilder.Configurations.Add(new IncomeTypeConfiguration());
            modelBuilder.Configurations.Add(new SpendTypeConfiguration());
            modelBuilder.Configurations.Add(new SpendInvoiceConfiguration());
            modelBuilder.Configurations.Add(new SpendItemConfiguration());
            modelBuilder.Configurations.Add(new IncomeInvoiceConfiguration());
            modelBuilder.Configurations.Add(new IncomeItemConfiguration());
            modelBuilder.Configurations.Add(new VaultCorrectionInvoiceConfiguration());


            //Relationships between user and other beacuse others not referencing user
            //Relationships between crates giveinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<GiveInvoice>(u => u.GiveInvoiceCreates)
                .WithRequired()
                .HasForeignKey(gi => gi.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edited giveinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<GiveInvoice>(u => u.GiveInvoiceEditeds)
                .WithOptional()
                .HasForeignKey(gi => gi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates PayInvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<PayInvoice>(u => u.PayInvoiceCreates)
                .WithRequired()
                .HasForeignKey(pi => pi.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edited PayInvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<PayInvoice>(u => u.PayInvoiceEditeds)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates customertocustomer and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<CustomerToCustomerInvoice>(u => u.CustomerToCustomerInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edited customertocustomer and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany <CustomerToCustomerInvoice>(u => u.CustomerToCustomerInvoiceEditeds)
                .WithOptional()
                .HasForeignKey(i=> i.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates vaulttovaultinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<VaultToVaultInvoice>(u => u.VaultToVaultInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edited vaulttovaultinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<VaultToVaultInvoice>(u => u.VaultToVaultInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates sellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SellInvoice>(u => u.SellInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites sellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SellInvoice>(u => u.SellInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates returnsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnSellInvoice>(u => u.ReturnSellInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites returnsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnSellInvoice>(u => u.ReturnSellInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates buyinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<BuyInvoice>(u => u.BuyInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites buyinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<BuyInvoice>(u => u.BuyInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates returnbuyinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnBuyInvoice>(u => u.ReturnBuyInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites returnbuyinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnBuyInvoice>(u => u.ReturnBuyInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates lostinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<LostInvoice>(u => u.LostInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites lostinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<LostInvoice>(u => u.LostInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates StoreTransferinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<StoreTransferInvoice>(u => u.StoreTransferInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites StoreTransferinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<StoreTransferInvoice>(u => u.StoreTransferInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates book-edition-first-time and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<BookEditionFirstTime>(u => u.BookEditionFirstTimeCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites book-edition-first-time and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<BookEditionFirstTime>(u => u.BookEditionFirstTimeEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates tempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<TempSellInvoice>(u => u.TempSellInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites tempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<TempSellInvoice>(u => u.TempSellInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates selltempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SellTempSellInvoice>(u => u.SellTempSellInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites selltempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SellTempSellInvoice>(u => u.SellTempSellInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates returntempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnTempSellInvoice>(u => u.ReturnTempSellInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites returntempsellinvoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ReturnTempSellInvoice>(u => u.ReturnTempSellInvoicesEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates spend-invoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SpendInvoice>(u => u.SpendInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites spend-invoice  and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<SpendInvoice>(u => u.SpendInvoiceEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates income-invoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<IncomeInvoice>(u => u.IncomeInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites income-invoice  and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<IncomeInvoice>(u => u.IncomeInvoiceEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between crates vault-correction-invoice and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<VaultCorrectionInvoice>(u => u.VaultCorrectionInvoiceCreates)
                .WithRequired()
                .HasForeignKey(i => i.CreatorUserId)
                .WillCascadeOnDelete(false);

            //Relationships between  edites vault-correction-invoice  and user
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<VaultCorrectionInvoice>(u => u.VaultCorrectionInvoiceEdites)
                .WithOptional()
                .HasForeignKey(pi => pi.EditorUserId)
                .WillCascadeOnDelete(false);
        }

        public static BookStoreDbContext Create()
        {
            return new BookStoreDbContext();
        }
    }
}