using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class BookStoreUnitOfWork
    {
        public BookStoreDbContext BookStoreDbContext { get; }

        private readonly UserStore<ApplicationUser> userStore;
        private readonly RoleStore<IdentityRole> roleStore;

        public StoreRepo Stores { get; private set; }
        public VaultRepo Vaults { get; private set; }
        public CustomerRepo Customers { get; private set; }
        public BookRepo Books { get; private set; }
        public BookEditionRepo BookEditions { get; private set; }
        public SpendTypeRepo SpendTypes { get; private set; }
        public IncomeTypeRepo IncomeTypes { get; private set; }
        public UserRepo Users { get; private set; }
        public RoleRepo Roles { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public GiveInvoiceRepo GiveInvoices { get; private set; }
        public PayInvoiceRepo PayInvoices { get;private set; }
        public VaultToVaultInvoiceRepo VaultToVaultInvoices { get; private set; }
        public CustomerToCustomerInvoiceRpo CustomerToCustomerInvoices { get; private set; }
        public SellInvoiceRepo SellInvoices { get; private set; }
        public SellItemRepo SellItems { get; private set; }
        public ReturnSellInvoiceRepo ReturnSellInvoices { get; private set; }
        public ReturnSellItemRepo ReturnSellItems { get; private set; }
        public BuyInvoiceRepo BuyInvoices { get; private set; }
        public BuyItemRepo BuyItems { get; private set; }
        public ReturnBuyInvoiceRepo ReturnBuyInvoices { get; private set; }
        public ReturnBuyItemRepo ReturnBuyItems { get; private set; }
        public LostInvoiceRepo LostInvoices { get; private set; }
        public LostItemRepo LostItems { get; private set; }
        public StoreTransferInvoiceRepo StoreTransferInvoices { get; private set; }
        public StoreTransferItemRepo StoreTransferItems { get; private set; }
        public BookEditionFirstTimeRepo BookEditionFirstTimes { get; private set; }
        public TempSellInvoiceRepo TempSellInvoices { get; private set; }
        public TempSellItemRepo TempSellItems { get; private set; }
        public SellTempSellInvoiceRepo SellTempSellInvoices { get; private set; }
        public SellTempSellItemRepo SellTempSellItems { get; private set; }
        public ReturnTempSellInvoiceRepo ReturnTempSellInvoices { get; private set; }
        public ReturnTempSellItemRepo ReturnTempSellItems { get; private set; }
        public SpendInvoiceRepo SpendInvoices { get; private set; }
        public SpendItemRepo SpendItems { get; private set; }
        public IncomeInvoiceRepo IncomeInvoices { get;private set; }
        public IncomeItemRepo IncomeItems { get; private set; }
        public VaultCorrectionInvoiceRepo VaultCorrectionInvoices { get; private set; }

        public BookStoreUnitOfWork(BookStoreDbContext bookStoreDbContext=null)
        {
            if (bookStoreDbContext==null)
            {
                BookStoreDbContext = new BookStoreDbContext();
            }
            else
            {
                BookStoreDbContext = bookStoreDbContext;
            }

            userStore = new UserStore<ApplicationUser>(BookStoreDbContext);
            roleStore = new RoleStore<IdentityRole>(BookStoreDbContext);

            Stores = new StoreRepo(BookStoreDbContext);
            Vaults = new VaultRepo(BookStoreDbContext);
            Customers = new CustomerRepo(BookStoreDbContext);
            Books = new BookRepo(BookStoreDbContext);
            BookEditions = new BookEditionRepo(BookStoreDbContext);
            Users = new UserRepo(BookStoreDbContext);
            Roles = new RoleRepo(BookStoreDbContext);
            UserManager = new UserManager<ApplicationUser>(userStore);
            RoleManager = new RoleManager<IdentityRole>(roleStore);
            GiveInvoices = new GiveInvoiceRepo(BookStoreDbContext);
            PayInvoices = new PayInvoiceRepo(BookStoreDbContext);
            VaultToVaultInvoices = new VaultToVaultInvoiceRepo(BookStoreDbContext);
            CustomerToCustomerInvoices = new CustomerToCustomerInvoiceRpo(BookStoreDbContext);
            SellInvoices = new SellInvoiceRepo(BookStoreDbContext);
            SellItems = new SellItemRepo(BookStoreDbContext);
            ReturnSellInvoices = new ReturnSellInvoiceRepo(BookStoreDbContext);
            ReturnSellItems = new ReturnSellItemRepo(BookStoreDbContext);
            BuyInvoices = new BuyInvoiceRepo(BookStoreDbContext);
            BuyItems = new BuyItemRepo(BookStoreDbContext);
            ReturnBuyInvoices = new ReturnBuyInvoiceRepo(BookStoreDbContext);
            ReturnBuyItems = new ReturnBuyItemRepo(BookStoreDbContext);
            LostInvoices = new LostInvoiceRepo(BookStoreDbContext);
            LostItems = new LostItemRepo(BookStoreDbContext);
            StoreTransferInvoices = new StoreTransferInvoiceRepo(BookStoreDbContext);
            StoreTransferItems = new StoreTransferItemRepo(BookStoreDbContext);
            BookEditionFirstTimes = new BookEditionFirstTimeRepo(BookStoreDbContext);
            TempSellInvoices = new TempSellInvoiceRepo(BookStoreDbContext);
            TempSellItems = new TempSellItemRepo(BookStoreDbContext);
            SellTempSellInvoices = new SellTempSellInvoiceRepo(BookStoreDbContext);
            SellTempSellItems = new SellTempSellItemRepo(BookStoreDbContext);
            ReturnTempSellInvoices = new ReturnTempSellInvoiceRepo(BookStoreDbContext);
            ReturnTempSellItems = new ReturnTempSellItemRepo(BookStoreDbContext);
            SpendTypes = new SpendTypeRepo(BookStoreDbContext);
            IncomeTypes = new IncomeTypeRepo(BookStoreDbContext);
            SpendInvoices = new SpendInvoiceRepo(BookStoreDbContext);
            SpendItems = new SpendItemRepo(BookStoreDbContext);
            IncomeInvoices = new IncomeInvoiceRepo(BookStoreDbContext);
            IncomeItems = new IncomeItemRepo(BookStoreDbContext);
            VaultCorrectionInvoices = new VaultCorrectionInvoiceRepo(BookStoreDbContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await BookStoreDbContext.SaveChangesAsync();
        }
    }
}