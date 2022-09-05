using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.FilterModels;
using BookStoreModel.GeneralModels;
using BookStoreModel.Other;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.VaultReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC
{
    [AuthorizeFilter(Roles = RoleNames.ADMIN + "," + RoleNames.MANAGER + "," + RoleNames.EMPLOYEE)]
    public class VaultReportsController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly VaultManager vaultManager;

        //View page addresses
        private const string VAULT_BALANCE = "~/Views/VaultReports/VaultBalance/VaultBalance.cshtml";
        private const string VAULT_DETAILS = "~/Views/VaultReports/VaultDetails/VaultDetails.cshtml";

        public VaultReportsController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            vaultManager = new VaultManager(bookStoreUnitOfWork);
        }

        // GET: VaultReports
        public async Task<ActionResult> GetBalances(VaultBalanceFilter filter=null)
        {
            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Return view and vault balances data
            return View(VAULT_BALANCE, await bookStoreUnitOfWork.Vaults.GetBalancesAsync(filter));
        }

        public async Task<ActionResult> GetDetails(VaultDetailsFilter filter = null)
        {
            List<VaultDetailsViewModel> details = new List<VaultDetailsViewModel>();

            //Store filter object in view-bag
            ViewBag.Filter = filter;

            //Set vault-id view-bag
            ViewBag.VaultId= new SelectList(
                (await bookStoreUnitOfWork.Vaults.GetAllNoTrackingAsync()).Select(v => new KeyValuePairViewModel<long, string>
                {
                    Key = v.Id,
                    Value = v.Id + ": " + v.Name,
                }),
                "Key", "Value", filter.VaultId);

            //If filter has vault-id then get details
            if (filter.VaultId!=0)
            {
                details = await vaultManager.GetDetailsAsync(filter);
            }

            //Return view and vault balances data
            return View(VAULT_DETAILS, details);
        }
    }
}