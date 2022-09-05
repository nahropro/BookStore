using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.CustomerReports;
using BookStoreModel.ViewModels.Shared;
using BookStoreModel.ViewModels.StoreReoports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class CustomerRepo:Repository<Customer>
    {
        public CustomerRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<List<LoanViewModel>> GetLoansAsync(LoanFilter filter)
        {
            IQueryable<LoanViewModel> loans;  //For storing loans
            IQueryable<Customer> customers; //For quering customer

            //Get queryable customer
            customers = GetQueryable();

            //Setup includes
            customers = customers.Include(i => i.PayInvoices)
                .Include(i => i.GiveInvoices)
                .Include(i => i.CustomerToCustomerInvoiceGives)
                .Include(i => i.CustomerToCustomerInvoicePays)
                .Include(i => i.SellInvoices)
                .Include(i => i.ReturnSellInvoices)
                .Include(i => i.BuyInvoices)
                .Include(i => i.SellTempSellInvoices)
                .Include(i => i.ReturnBuyInvoices);

            //Map customer to loan view model with filtering until-date
            loans = customers.Select(i => new LoanViewModel
            {
                Custoemr = new CustomerSharedViewModel
                {
                    FullName = i.FullName,
                    Id = i.Id,
                    Phone = i.Phone,
                    WorkPlace = i.WorkPlace,
                },
                Give =i.PayInvoices.Where(s =>(filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s=> s.Total).DefaultIfEmpty(0).Sum() +
                    i.CustomerToCustomerInvoicePays.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s=> s.Amount).DefaultIfEmpty(0).Sum() +
                    i.SellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s=> s.Total).DefaultIfEmpty(0).Sum() +
                    i.ReturnBuyInvoices.Where(s =>(filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s=> s.Total).DefaultIfEmpty(0).Sum() +
                    i.SellTempSellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.IncomeInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Loan).DefaultIfEmpty(0).Sum() +
                    (i.FirstTimeBalance > 0 ? i.FirstTimeBalance : 0),
                Pay = i.GiveInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.CustomerToCustomerInvoiceGives.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    i.BuyInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.ReturnSellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.SpendInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Loan).DefaultIfEmpty(0).Sum() +
                    (i.FirstTimeBalance < 0 ? i.FirstTimeBalance*(-1) : 0),
            });

            //Filter loans
            loans = loans.Where(i =>
                  ((filter.ZeroBalance ? (i.Give - i.Pay) == 0 : false) ||
                  (filter.PositiveBalance ? (i.Give - i.Pay) > 0 : false) ||
                  (filter.NegativeBalance ? (i.Give - i.Pay) < 0 : false)) &&
                  ((filter.BalanceGreaterThanOrEqual.HasValue ? (i.Give - i.Pay) >= filter.BalanceGreaterThanOrEqual.Value : true) &&
                  (filter.BalanceLessThanOrEqual.HasValue ? (i.Give - i.Pay) <= filter.BalanceLessThanOrEqual.Value : true))
            );

            //Set sorting
            //Check of assending or descending order by
            if (!filter.SortByDesc)
            {
                //Sort ascending
                //Check if sort by customer or balance
                if (filter.SortBy == LoanFilter.SortByEnum.Customer)
                {
                    loans = loans.OrderBy(i => i.Custoemr.FullName);
                }
                else if (filter.SortBy == LoanFilter.SortByEnum.Balance)
                {
                    loans = loans.OrderBy(i => (i.Give - i.Pay));
                }
            }
            else
            {
                //Sort Descending
                //Check if sort by customer or balance
                if (filter.SortBy == LoanFilter.SortByEnum.Customer)
                {
                    loans = loans.OrderByDescending(i => i.Custoemr.FullName);
                }
                else if (filter.SortBy == LoanFilter.SortByEnum.Balance)
                {
                    loans = loans.OrderByDescending(i => (i.Give - i.Pay));
                }
            }

            return await loans.AsNoTracking().ToListAsync();
        }

        public async Task<List<LoanViewModel>> GetDischargeLoansAsync(LoanFilter filter)
        {
            IQueryable<LoanViewModel> loans;  //For storing loans
            IQueryable<Customer> customers; //For quering customer

            //Get queryable customer
            customers = GetQueryable();

            //Setup includes
            customers = customers.Include(i => i.TempSellInvoices)
                .Include(i => i.SellTempSellInvoices)
                .Include(i => i.ReturnTempSellInvoices);

            //Map customer to loan view model with filtering until-date
            loans = customers.Select(i => new LoanViewModel
            {
                Custoemr = new CustomerSharedViewModel
                {
                    FullName = i.FullName,
                    Id = i.Id,
                    Phone = i.Phone,
                    WorkPlace = i.WorkPlace,
                },
                Give = i.TempSellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum(),
                Pay = i.ReturnTempSellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.SellTempSellInvoices.Where(s => (filter.UntilDate.HasValue ? s.InvoiceDate <= filter.UntilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() ,
            });

            //Filter loans
            loans = loans.Where(i =>
                  ((filter.ZeroBalance ? (i.Give - i.Pay) == 0 : false) ||
                  (filter.PositiveBalance ? (i.Give - i.Pay) > 0 : false) ||
                  (filter.NegativeBalance ? (i.Give - i.Pay) < 0 : false)) &&
                  ((filter.BalanceGreaterThanOrEqual.HasValue ? (i.Give - i.Pay) >= filter.BalanceGreaterThanOrEqual.Value : true) &&
                  (filter.BalanceLessThanOrEqual.HasValue ? (i.Give - i.Pay) <= filter.BalanceLessThanOrEqual.Value : true))
            );

            //Set sorting
            //Check of assending or descending order by
            if (!filter.SortByDesc)
            {
                //Sort ascending
                //Check if sort by customer or balance
                if (filter.SortBy == LoanFilter.SortByEnum.Customer)
                {
                    loans = loans.OrderBy(i => i.Custoemr.FullName);
                }
                else if (filter.SortBy == LoanFilter.SortByEnum.Balance)
                {
                    loans = loans.OrderBy(i => (i.Give - i.Pay));
                }
            }
            else
            {
                //Sort Descending
                //Check if sort by customer or balance
                if (filter.SortBy == LoanFilter.SortByEnum.Customer)
                {
                    loans = loans.OrderByDescending(i => i.Custoemr.FullName);
                }
                else if (filter.SortBy == LoanFilter.SortByEnum.Balance)
                {
                    loans = loans.OrderByDescending(i => (i.Give - i.Pay));
                }
            }

            return await loans.AsNoTracking().ToListAsync();
        }

        public async Task<decimal> GetLoanAsync(long customerId,DateTime? untilDate)
        {
            LoanViewModel loan = new LoanViewModel();
            IQueryable<Customer> customers; //For quering customer

            //Get queryable customer
            customers = GetQueryable();

            //Setup includes
            customers = customers.Include(i => i.PayInvoices)
                .Include(i => i.GiveInvoices)
                .Include(i => i.CustomerToCustomerInvoiceGives)
                .Include(i => i.CustomerToCustomerInvoicePays)
                .Include(i => i.SellInvoices)
                .Include(i => i.ReturnSellInvoices)
                .Include(i => i.BuyInvoices)
                .Include(i => i.SellTempSellInvoices)
                .Include(i => i.ReturnBuyInvoices);

            //Map customer to loan view model with filtering until-date
            loan =await customers.Select(i => new LoanViewModel
            {
                Custoemr = new CustomerSharedViewModel
                {
                    Id = i.Id,
                },
                Give = i.PayInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.CustomerToCustomerInvoicePays.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    i.SellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.ReturnBuyInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.SellTempSellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.IncomeInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Loan).DefaultIfEmpty(0).Sum() +
                    i.FirstTimeBalance,
                Pay = i.GiveInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.CustomerToCustomerInvoiceGives.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Amount).DefaultIfEmpty(0).Sum() +
                    i.BuyInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.ReturnSellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.SpendInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Loan).DefaultIfEmpty(0).Sum(),
            }).AsNoTracking().SingleOrDefaultAsync(i=> i.Custoemr.Id==customerId);          

            return loan.Balance;
        }

        public async Task<decimal> GetDischargeLoanAsync(long customerId, DateTime? untilDate)
        {
            LoanViewModel loan;  //For storing loans
            IQueryable<Customer> customers; //For quering customer

            //Get queryable customer
            customers = GetQueryable();

            //Setup includes
            customers = customers.Include(i => i.TempSellInvoices)
                .Include(i => i.SellTempSellInvoices)
                .Include(i => i.ReturnTempSellInvoices);

            //Map customer to loan view model with filtering until-date
            loan =await customers.Select(i => new LoanViewModel
            {
                Custoemr = new CustomerSharedViewModel
                {
                    Id = i.Id,
                },
                Give = i.TempSellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum(),
                Pay = i.ReturnTempSellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum() +
                    i.SellTempSellInvoices.Where(s => (untilDate.HasValue ? s.InvoiceDate <= untilDate.Value : true))
                        .Select(s => s.Total).DefaultIfEmpty(0).Sum(),
            }).AsNoTracking().SingleOrDefaultAsync(i=> i.Custoemr.Id==customerId);
            

            return loan.Balance;
        }

        public async Task<List<BookEditionDischargeRemainsByCustomerViewModel>> GetDischargeBookEditionRemainsAsync(long bookEditionId)
        {
            IQueryable<Customer> customers;
            List<BookEditionDischargeRemainsByCustomerViewModel> result;

            customers = GetQueryable();

            //Setup includes
            customers = customers
                .Include(i => i.TempSellInvoices)
                .Include(i => i.TempSellInvoices.Select(s=> s.Items))
                .Include(i => i.SellTempSellInvoices)
                .Include(i => i.SellTempSellInvoices.Select(s=> s.Items))
                .Include(i => i.ReturnTempSellInvoices)
                .Include(i => i.ReturnTempSellInvoices.Select(s=> s.Items));

            //Map to view model and filter zero remains
            result = await customers.Select(i => new BookEditionDischargeRemainsByCustomerViewModel
            {
                CustomerId=i.Id,
                CustomerFullName=i.FullName,
                CustomerPhone=i.Phone,
                CustomerWorkPlace=i.WorkPlace,
                In = i.TempSellInvoices.SelectMany(s => s.Items).Where(s => s.BookEditionId == bookEditionId)
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum(),
                Out = i.SellTempSellInvoices.SelectMany(s => s.Items).Where(s => s.BookEditionId == bookEditionId)
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum() +
                     i.ReturnTempSellInvoices.SelectMany(s => s.Items).Where(s => s.BookEditionId == bookEditionId)
                        .Select(s => s.Qtt).DefaultIfEmpty(0).Sum(),
            }).Where(i => (i.In - i.Out) != 0).AsNoTracking().ToListAsync();

            return result;
        }
    }
}