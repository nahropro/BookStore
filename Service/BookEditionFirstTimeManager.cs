using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.BookEditionFirstTime;
using BookStoreModel.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class BookEditionFirstTimeManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public BookEditionFirstTimeManager(BookStoreUnitOfWork bookStoreUnitOfWork)
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

        public BookEditionFirstTimeManager(BookStoreDbContext bookStoreDbContext = null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectBookEditionFirstTimeViewModel>> GetAllNoTrackingAsync(BookEditionFirstTimeFilter filter = null)
        {
            List<SelectBookEditionFirstTimeViewModel> result;
            List<BookEditionFirstTime> firstTimes;
            List<ApplicationUser> users;
            List<string> userIds = new List<string>();

            //Get all invoice with include nessesary objects and filtered if has any
            firstTimes = await bookStoreUnitOfWork.BookEditionFirstTimes
                .FilterNoTrackingWithIncludesAsync(filter: filter,includeBookEdition:true, includeStore:true);

            //Add creator userids for userIds collection
            userIds.AddRange(firstTimes.Select(i => i.CreatorUserId).ToList());

            //Add editor userids for userIds collection
            userIds.AddRange(firstTimes.Select(gi => gi.EditorUserId).ToList());

            //Distinct userids for redusing unnessesary calculation
            userIds = userIds.Distinct().ToList();

            //Get all user that has invoice, creator or editor
            users = await bookStoreUnitOfWork.Users.FindNoTrackingAsync(u => userIds.Contains(u.Id));

            //Map invoice to select view model
            result = firstTimes.Select(i => new SelectBookEditionFirstTimeViewModel
            {
                BookEditionId = i.BookEditionId,
                BookEditionInText = i.BookEdition.EditionInString,
                BookEdtionInNumber = i.BookEdition.EditionInNumber,
                BookName = i.BookEdition.Book.Name,
                Price = i.Price,
                Qtt = i.Qtt,
                StoreId = i.StoreId,
                StoreName = i.Store.Name,
                Total = i.Total,
                ChangeInfo = new ChangeInfoViewModel
                {
                    CreatorUserFullName = users.SingleOrDefault(u => u.Id == i.CreatorUserId).UserExtend.FullName,
                    CreationDateTime = i.CreationDateTime,
                    CreatorUserId = i.CreatorUserId,
                    EditorUserFullName = users.SingleOrDefault(u => u.Id == i.EditorUserId)?.UserExtend.FullName,
                    EditorUserId = i.EditorUserId,
                    LastEditedDateTime = i.LastEditedDateTime,
                },
            }).ToList();

            return result;
        }
    }
}