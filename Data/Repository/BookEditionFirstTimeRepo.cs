using BookStoreModel.FilterModels;
using BookStoreModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class BookEditionFirstTimeRepo:Repository<BookEditionFirstTime>
    {
        public BookEditionFirstTimeRepo(BookStoreDbContext bookStoreDbContext):base(bookStoreDbContext)
        {

        }

        public async Task<bool> IsExists(long storeId,long bookEditionId)
        {
            return await base.GetQueryable().AnyAsync(i => i.StoreId == storeId && i.BookEditionId == bookEditionId);
        }

        public override BookEditionFirstTime Add(BookEditionFirstTime entity)
        {
            //Check if price greater than zero and qtt is not zero
            if (entity.Price>=0 && entity.Qtt!=0)
            {
                entity.Total = entity.Qtt * entity.Price;   //Set total price and calulate it
                entity.CreationDateTime = DateTime.UtcNow;  //Set creation datetime

                return base.Add(entity);
            }

            throw new Exception("Invalid Data");
        }

        public override IEnumerable<BookEditionFirstTime> AddRange(IEnumerable<BookEditionFirstTime> entities)
        {
            //Check if price greater than zero and qtt is not zero
            if (entities.All(i=> i.Qtt!=0 && i.Price>=0))
            {
                //Go throw all entities
                foreach (var entity in entities)
                {
                    entity.Total = entity.Qtt * entity.Price;   //Calculate total
                    entity.CreationDateTime = DateTime.UtcNow;
                }

                return base.AddRange(entities);
            }

            throw new Exception("Invalid data");
        }

        public async Task<List<BookEditionFirstTime>> FilterNoTrackingWithIncludesAsync(BookEditionFirstTimeFilter filter = null,
            bool includeBookEdition = false, bool includeStore = false)
        {
            IQueryable<BookEditionFirstTime> firstTimes;

            firstTimes = GetQueryable();
            if (includeBookEdition)
            {
                firstTimes = firstTimes.Include(i => i.BookEdition)
                    .Include(i => i.BookEdition.Book);
            }

            if (includeStore)
            {
                firstTimes = firstTimes.Include(i => i.Store);
            }

            if (filter != null)
            {
                firstTimes = firstTimes.Where(i =>
                    (filter.StoreId.HasValue ? i.StoreId == filter.StoreId.Value : true) &&
                    (filter.BookEditionId.HasValue ? i.BookEditionId == filter.BookEditionId.Value : true)
                );
            }

            return await firstTimes.AsNoTracking().ToListAsync();
        }

        public override BookEditionFirstTime Edit(BookEditionFirstTime entity)
        {
            BookEditionFirstTime realModel;

            //Check if qtt is not zer and price is positive and has editor user id
            if (entity.Qtt!=0 && entity.Price>=0 && !string.IsNullOrWhiteSpace(entity.EditorUserId))
            {
                //Get the real item from databae
                realModel =SingleOrDefault(i=> i.StoreId==entity.StoreId && i.BookEditionId==entity.BookEditionId);

                //Update ony allowed datas
                realModel.Qtt = entity.Qtt;
                realModel.Price = entity.Price;

                realModel.Total = realModel.Qtt * realModel.Price;  //Calcualte total
                realModel.LastEditedDateTime = DateTime.UtcNow; //Set edited datetime
                realModel.EditorUserId = entity.EditorUserId;   //Set ediitor user-id

                return realModel;
            }
            else
            {
                //Throw exception with incorrect data
                throw new Exception("Incorrect data");
            }
        }

        public async Task<List<BookEditionFirstTime>> FindNoTrackinWithIncludesAsync(Expression<Func<BookEditionFirstTime, bool>> expression,
             bool includeStore = false, bool includeBookEdition = false)
        {
            IQueryable<BookEditionFirstTime> sellItems;

            sellItems = GetQueryable();

            //Set includes
            if (includeBookEdition)
            {
                sellItems = sellItems.Include(i => i.BookEdition);
            }

            if (includeStore)
            {
                sellItems = sellItems.Include(i => i.Store);
            }

            return await sellItems.Where(expression).AsNoTracking().ToListAsync();
        }
    }
}