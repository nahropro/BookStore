using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.CustomerReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class CustomerManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public CustomerManager(BookStoreUnitOfWork bookStoreUnitOfWork)
        {
            if (bookStoreUnitOfWork == null)
            {
                this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
            }
            else
            {
                this.bookStoreUnitOfWork = bookStoreUnitOfWork;
            }
        }

        public CustomerManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<CustomerDetailsViewModel>> GetCustomerDetailsAsync(CustomerDetailsFilter filter)
        {
            List<CustomerDetailsViewModel> details = new List<CustomerDetailsViewModel>();
            Customer customer;
            decimal firstTimeBalance = 0;
            decimal balanceInTime = 0;

            //If there is startdate filter calculate firttime balance until one day before
            if (filter.StartDate.HasValue)
            {
                firstTimeBalance = await bookStoreUnitOfWork.Customers.GetLoanAsync(filter.CustomerId,
                    filter.StartDate.Value.AddDays(-1));
            }
            else
            {
                //Otherwise get customer first-time-balance
                customer = await bookStoreUnitOfWork.Customers.SingleOrDefaultNoTrackingAsync(i => i.Id == filter.CustomerId);
                firstTimeBalance = customer.FirstTimeBalance;   //Set customer first time balance
            }

            //Add first time to details
            details.Add(new CustomerDetailsViewModel
            {
                BalanceGive = firstTimeBalance,
                Type = CustomerDetailsViewModel.InvoiceType.First_Time,
            });

            //Add pay invoices
            details.AddRange((await bookStoreUnitOfWork.PayInvoices.FindNoTrackingAsync(i=>
                        i.CustomerId==filter.CustomerId &&
                        (filter.StartDate.HasValue?i.InvoiceDate>=filter.StartDate.Value:true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i=> new CustomerDetailsViewModel
                {
                    BalanceGive=i.Total,
                    Discount=i.Discount,
                    InvoiceDate=i.InvoiceDate,
                    InvoiceId=i.Id,
                    Note=i.AmountNote + " | " + i.DiscountNote,
                    Type=CustomerDetailsViewModel.InvoiceType.Pay_Invoice,
                })
            );

            //Add give invoices
            details.AddRange((await bookStoreUnitOfWork.GiveInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Total,
                    Discount = i.Discount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.AmountNote + " | " + i.DiscountNote,
                    Type = CustomerDetailsViewModel.InvoiceType.Give_Invoice,
                })
            );

            //Add spend invoices
            details.AddRange((await bookStoreUnitOfWork.SpendInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Loan,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Spend_Invoice,
                })
            );

            //Add income invoices
            details.AddRange((await bookStoreUnitOfWork.IncomeInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Loan,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Income_Invoice,
                })
            );


            //Add pay customer transfer invoices
            details.AddRange((await bookStoreUnitOfWork.CustomerToCustomerInvoices.FindNoTrackingAsync(i =>
                        i.PayCustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Customer_Transfer,
                })
            );

            //Add give customer transfer invoices
            details.AddRange((await bookStoreUnitOfWork.CustomerToCustomerInvoices.FindNoTrackingAsync(i =>
                        i.GiveCustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Amount,
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Customer_Transfer,
                })
            );

            //Add sell invoices
            details.AddRange((await bookStoreUnitOfWork.SellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Sell_Invoice,
                })
            );

            //Add return buy invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnBuyInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Return_Buy_Invoice,
                })
            );

            //Add sell temp sell invoices
            details.AddRange((await bookStoreUnitOfWork.SellTempSellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Sell_Discharge_Sell_Invoice,
                })
            );

            //Add buy invoices
            details.AddRange((await bookStoreUnitOfWork.BuyInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Sell_Discharge_Sell_Invoice,
                })
            );

            //Add return sell invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnSellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Sell_Discharge_Sell_Invoice,
                })
            );



            //Sort accending by invoice-date
            details = details.OrderBy(i => i.InvoiceDate).ToList();

            //Calculate in time balance
            foreach (var detail in details)
            {
                //Calculate balance-in-time
                balanceInTime = balanceInTime + detail.BalanceGive-detail.BalancePay;

                //Set balance-in-time to detail object
                detail.BalanceInTime = balanceInTime;
            }

            return details;
        }

        public async Task<List<CustomerDetailsViewModel>> GetCustomerDischargeDetailsAsync(CustomerDetailsFilter filter)
        {
            List<CustomerDetailsViewModel> details = new List<CustomerDetailsViewModel>();
            decimal firstTimeBalance = 0;
            decimal balanceInTime = 0;

            //If there is startdate filter calculate firttime balance until one day before
            if (filter.StartDate.HasValue)
            {
                firstTimeBalance = await bookStoreUnitOfWork.Customers.GetDischargeLoanAsync(filter.CustomerId,
                    filter.StartDate.Value.AddDays(-1));

                //Add first time to details
                details.Add(new CustomerDetailsViewModel
                {
                    BalanceGive = firstTimeBalance,
                    Type = CustomerDetailsViewModel.InvoiceType.First_Time,
                });
            }
            

            //Add temp sell invoices
            details.AddRange((await bookStoreUnitOfWork.TempSellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalanceGive = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Discharge_Sell_Invoice,
                })
            );

            //Add return temp sell invoices
            details.AddRange((await bookStoreUnitOfWork.ReturnTempSellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay = i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Return_Discharge_Sell_Invoice,
                })
            );

            //Add sell temp sell invoices
            details.AddRange((await bookStoreUnitOfWork.SellTempSellInvoices.FindNoTrackingAsync(i =>
                        i.CustomerId == filter.CustomerId &&
                        (filter.StartDate.HasValue ? i.InvoiceDate >= filter.StartDate.Value : true) &&
                        (filter.EndDate.HasValue ? i.InvoiceDate <= filter.EndDate.Value : true)
                    )
                ).Select(i => new CustomerDetailsViewModel
                {
                    BalancePay= i.Total,
                    Discount = i.Discount.GetValueOrDefault(),
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.Id,
                    Note = i.Note,
                    Type = CustomerDetailsViewModel.InvoiceType.Sell_Discharge_Sell_Invoice,
                })
            );

            //Sort accending by invoice-date
            details = details.OrderBy(i => i.InvoiceDate).ToList();

            //Calculate in time balance
            foreach (var detail in details)
            {
                //Calculate balance-in-time
                balanceInTime = balanceInTime + detail.BalanceGive - detail.BalancePay;

                //Set balance-in-time to detail object
                detail.BalanceInTime = balanceInTime;
            }

            return details;
        }
    }
}