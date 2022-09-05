using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStoreModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookStore.Data
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public UserExtend UserExtend { get; set; }

        public ICollection<GiveInvoice> GiveInvoiceCreates { get; set; }
        public ICollection<GiveInvoice> GiveInvoiceEditeds { get; set; }
        public ICollection<PayInvoice> PayInvoiceCreates { get; set; }
        public ICollection<PayInvoice> PayInvoiceEditeds { get; set; }
        public ICollection<CustomerToCustomerInvoice> CustomerToCustomerInvoiceCreates { get; set; }
        public ICollection<CustomerToCustomerInvoice> CustomerToCustomerInvoiceEditeds { get; set; }
        public ICollection<VaultToVaultInvoice> VaultToVaultInvoiceCreates { get; set; }
        public ICollection<VaultToVaultInvoice> VaultToVaultInvoicesEdites { get; set; }
        public ICollection<SellInvoice> SellInvoiceCreates { get; set; }
        public ICollection<SellInvoice> SellInvoicesEdites { get; set; }
        public ICollection<ReturnSellInvoice> ReturnSellInvoiceCreates { get; set; }
        public ICollection<ReturnSellInvoice> ReturnSellInvoicesEdites { get; set; }
        public ICollection<BuyInvoice> BuyInvoiceCreates { get; set; }
        public ICollection<BuyInvoice> BuyInvoicesEdites { get; set; }
        public ICollection<ReturnBuyInvoice> ReturnBuyInvoiceCreates { get; set; }
        public ICollection<ReturnBuyInvoice> ReturnBuyInvoicesEdites { get; set; }
        public ICollection<TempSellInvoice> TempSellInvoiceCreates { get; set; }
        public ICollection<TempSellInvoice> TempSellInvoicesEdites { get; set; }
        public ICollection<SellTempSellInvoice> SellTempSellInvoiceCreates { get; set; }
        public ICollection<SellTempSellInvoice> SellTempSellInvoicesEdites { get; set; }
        public ICollection<ReturnTempSellInvoice> ReturnTempSellInvoiceCreates { get; set; }
        public ICollection<ReturnTempSellInvoice> ReturnTempSellInvoicesEdites { get; set; }
        public ICollection<LostInvoice> LostInvoiceCreates { get; set; }
        public ICollection<LostInvoice> LostInvoicesEdites { get; set; }
        public ICollection<StoreTransferInvoice> StoreTransferInvoiceCreates { get; set; }
        public ICollection<StoreTransferInvoice> StoreTransferInvoicesEdites { get; set; }
        public ICollection<BookEditionFirstTime> BookEditionFirstTimeCreates { get; set; }
        public ICollection<BookEditionFirstTime> BookEditionFirstTimeEdites { get; set; }
        public ICollection<SpendInvoice> SpendInvoiceCreates { get; set; }
        public ICollection<SpendInvoice> SpendInvoiceEdites { get; set; }
        public ICollection<IncomeInvoice> IncomeInvoiceCreates { get; set; }
        public ICollection<IncomeInvoice> IncomeInvoiceEdites { get; set; }
        public ICollection<VaultCorrectionInvoice> VaultCorrectionInvoiceCreates { get; set; }
        public ICollection<VaultCorrectionInvoice> VaultCorrectionInvoiceEdites { get; set; }

        public ApplicationUser()
        {
            //Set objects
            GiveInvoiceCreates = new HashSet<GiveInvoice>();
            GiveInvoiceEditeds = new HashSet<GiveInvoice>();
            PayInvoiceCreates = new HashSet<PayInvoice>();
            PayInvoiceEditeds = new HashSet<PayInvoice>();
            CustomerToCustomerInvoiceCreates = new HashSet<CustomerToCustomerInvoice>();
            CustomerToCustomerInvoiceEditeds = new HashSet<CustomerToCustomerInvoice>();
            VaultToVaultInvoiceCreates = new HashSet<VaultToVaultInvoice>();
            VaultToVaultInvoicesEdites = new HashSet<VaultToVaultInvoice>();
            SellInvoiceCreates = new HashSet<SellInvoice>();
            SellInvoicesEdites = new HashSet<SellInvoice>();
            ReturnSellInvoiceCreates = new HashSet<ReturnSellInvoice>();
            ReturnSellInvoicesEdites = new HashSet<ReturnSellInvoice>();
            BuyInvoiceCreates = new HashSet<BuyInvoice>();
            BuyInvoicesEdites = new HashSet<BuyInvoice>();
            ReturnBuyInvoiceCreates = new HashSet<ReturnBuyInvoice>();
            ReturnBuyInvoicesEdites = new HashSet<ReturnBuyInvoice>();
            LostInvoiceCreates = new HashSet<LostInvoice>();
            LostInvoicesEdites = new HashSet<LostInvoice>();
            StoreTransferInvoiceCreates = new HashSet<StoreTransferInvoice>();
            StoreTransferInvoicesEdites = new HashSet<StoreTransferInvoice>();
            BookEditionFirstTimeCreates = new HashSet<BookEditionFirstTime>();
            BookEditionFirstTimeEdites = new HashSet<BookEditionFirstTime>();
            TempSellInvoiceCreates = new HashSet<TempSellInvoice>();
            TempSellInvoicesEdites = new HashSet<TempSellInvoice>();
            SellTempSellInvoiceCreates = new HashSet<SellTempSellInvoice>();
            SellTempSellInvoicesEdites = new HashSet<SellTempSellInvoice>();
            ReturnTempSellInvoiceCreates = new HashSet<ReturnTempSellInvoice>();
            ReturnTempSellInvoicesEdites = new HashSet<ReturnTempSellInvoice>();
            SpendInvoiceCreates = new HashSet<SpendInvoice>();
            SpendInvoiceEdites = new HashSet<SpendInvoice>();
            IncomeInvoiceCreates = new HashSet<IncomeInvoice>();
            IncomeInvoiceEdites = new HashSet<IncomeInvoice>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}