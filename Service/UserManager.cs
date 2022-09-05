using BookStore.Data;
using BookStore.Data.Repository;
using BookStoreModel.StaticData;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.Account;
using BookStoreModel.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Service
{
    public class UserManager
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;

        public UserManager(BookStoreUnitOfWork bookStoreUnitOfWork)
        {
            if (bookStoreUnitOfWork==null)
            {
                this.bookStoreUnitOfWork = new BookStoreUnitOfWork();
            }
            else
            {
                this.bookStoreUnitOfWork = bookStoreUnitOfWork;
            }
        }

        public UserManager(BookStoreDbContext bookStoreDbContext=null)
        {
            this.bookStoreUnitOfWork = new BookStoreUnitOfWork(bookStoreDbContext);
        }

        public async Task<List<SelectUserViewModel>> GetAllNoTrackingAsync()
        {
            List<SelectUserViewModel> selectUsers;
            List<ApplicationUser> users;
            List<IdentityRole> roles;

            //Get all rolse
            roles = await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync();

            //Get all users
            users = await bookStoreUnitOfWork.Users.GetAllNoTrackingAsync();

            //Join th users and roles in list not in database
            selectUsers = users.Select(u => new SelectUserViewModel
            {
                Address = u.UserExtend.Address,
                FullName = u.UserExtend.FullName,
                Id = u.Id,
                Phone = u.UserExtend.Phone,
                UserName = u.UserName,
                Active=!u.LockoutEnabled,
                Roles = roles.Where(r => u.Roles.Select(ur => ur.RoleId).ToList().Contains(r.Id)).Select(r => r.Name).ToList(),
            }).ToList();

            //Filter manager user
            selectUsers = selectUsers.Where(su => !su.Roles.Contains(RoleNames.MANAGER)).ToList();

            return selectUsers;
        }

        public async Task<SelectUserViewModel> GettNoTrackingAsync(string id)
        {
            List<IdentityRole> roles;
            ApplicationUser user;
            SelectUserViewModel selectUserViewModel;

            //Get all rolse
            roles = await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync();

            //Get the users
            user = await bookStoreUnitOfWork.Users.SingleOrDefaultNoTrackingAsync(u=> u.Id==id);

            //Map user to selectUserViewModle and join with roles
            selectUserViewModel = new SelectUserViewModel
            {
                Active = !user.LockoutEnabled,
                Id = user.Id,
                Address = user.UserExtend.Address,
                FullName = user.UserExtend.FullName,
                Phone = user.UserExtend.Phone,
                UserName = user.UserName,
                Roles = roles.Where(r => user.Roles.Select(ur => ur.RoleId).ToList().Contains(r.Id)).Select(r => r.Name).ToList(),
            };

            return selectUserViewModel;
        }

        //Create normal user
        public async Task<bool> CreateUserAsync(CreateUserViewModel createUserViewModel)
        {
            ApplicationUser user;

            using(DbContextTransaction transaction = bookStoreUnitOfWork.BookStoreDbContext.Database.BeginTransaction())
            {
                try
                {
                    //Check security if the selected role is manager
                    //Do nothing and return false
                    if (createUserViewModel.Role==RoleNames.MANAGER)
                    {
                        return false;
                    }

                    //Set the user
                    user = new ApplicationUser
                    {
                        UserName=createUserViewModel.UserName,
                        Email=createUserViewModel.UserName,
                        LockoutEnabled=!createUserViewModel.Active,
                        UserExtend=new UserExtend
                        {
                            Address=createUserViewModel.Address,
                            FullName=createUserViewModel.FullName,
                            Phone=createUserViewModel.Phone,
                        },
                    };

                    //Check if deactive selected, set lockout time to infinity
                    if (user.LockoutEnabled)
                    {
                        user.LockoutEndDateUtc = new DateTime(9999, 12, 31);
                    }

                    //Create the user
                    await bookStoreUnitOfWork.UserManager.CreateAsync(user, createUserViewModel.Password);

                    //Set the role to the user
                    await bookStoreUnitOfWork.UserManager.AddToRoleAsync(user.Id, createUserViewModel.Role);

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> EditUserAsync(EditUserViewModel editUserViewModel)
        {
            ApplicationUser user;
            IList<string> userRoles;

            //Get user
            user =await bookStoreUnitOfWork.Users.GetAsync(editUserViewModel.Id);

            //If user is't exists return false
            if (user==null)
            {
                return false;
            }

            //Get the roles that this user has
            userRoles = await bookStoreUnitOfWork.UserManager.GetRolesAsync(user.Id);
            
            //Security check: If user has manager role do nothing and return false
            if (userRoles.Contains(RoleNames.MANAGER))
            {
                return false;
            }

            //Security check: If selected role for edit is manager do nothing and  return false
            if (editUserViewModel.Role==RoleNames.MANAGER)
            {
                return false;
            }

            //Begin transaction
            using (DbContextTransaction transaction=bookStoreUnitOfWork.BookStoreDbContext.Database.BeginTransaction())
            {
                try
                {
                    //Remove all roles that this user has, 
                    //for assining the selected role in edit
                    await bookStoreUnitOfWork.UserManager.RemoveFromRolesAsync(user.Id, userRoles.ToArray());

                    //Update user data
                    user.UserName = editUserViewModel.UserName;
                    user.UserExtend.Address = editUserViewModel.Address;
                    user.UserExtend.FullName = editUserViewModel.FullName;
                    user.UserExtend.Phone = editUserViewModel.Phone;
                    user.LockoutEnabled = !editUserViewModel.Active;

                    //Check if LockOutEnabled, set LockOutTime to infinity
                    if (user.LockoutEnabled)
                    {
                        user.LockoutEndDateUtc = new DateTime(9999, 12, 31);
                    }

                    //Save changes of user
                    await bookStoreUnitOfWork.CompleteAsync();

                    //Add new selected role to the user
                    await bookStoreUnitOfWork.UserManager.AddToRoleAsync(user.Id, editUserViewModel.Role);

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        //Create the intialize mangaer
        public async Task<bool> SetUpManager(RegisterManagerViewModel registerManager)
        {
            //Create roles and manager in transaction
            using (DbContextTransaction transaction = bookStoreUnitOfWork.BookStoreDbContext.Database.BeginTransaction())
            {
                try
                {
                    //Create roles if not exists
                    await CreateRolesAsync();

                    //Check if any user exists
                    if (await bookStoreUnitOfWork.Users.CountAsync() != 0)
                    {
                        return false;
                    }

                    ApplicationUser user = new ApplicationUser
                    {
                        Email = registerManager.UserName,
                        UserName = registerManager.UserName,
                        UserExtend = new UserExtend
                        {
                            Address = registerManager.Address,
                            FullName = registerManager.FullName,
                            Phone = registerManager.Phone,
                        },
                    };

                    //Create the user
                    await bookStoreUnitOfWork.UserManager.CreateAsync(user, registerManager.Password);

                    //Set manager role to the user
                    await bookStoreUnitOfWork.UserManager.AddToRoleAsync(user.Id, RoleNames.MANAGER);

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        //Create roles if not exists
        private async Task CreateRolesAsync()
        {
            if (await bookStoreUnitOfWork.Roles.CountAsync() == 0)
            {
                await bookStoreUnitOfWork.RoleManager.CreateAsync(new IdentityRole { Name = RoleNames.MANAGER });
                await bookStoreUnitOfWork.RoleManager.CreateAsync(new IdentityRole { Name = RoleNames.ADMIN });
                await bookStoreUnitOfWork.RoleManager.CreateAsync(new IdentityRole { Name = RoleNames.EMPLOYEE });
            }
        }

        public static bool IsUserActive(string userId)
        {
            if (userId!=null)
            {
                return !(new BookStoreUnitOfWork().Users.
                SingleOrDefaultNoTracking(u => u.Id == userId)
                .LockoutEnabled);
            }

            return false;
        }
    }
}