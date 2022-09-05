using BookStore.Data;
using BookStore.Data.Repository;
using BookStore.Other;
using BookStore.Service;
using BookStoreModel.StaticData;
using BookStoreModel.ViewModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.Definitions
{
    [AuthorizeFilter(Roles = RoleNames.MANAGER)]
    public class UsersController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly UserManager userManager;

        //View page addresses
        private const string INDEX = "~/Views/Definitions/Users/Index.cshtml";
        private const string CREATE = "~/Views/Definitions/Users/Create.cshtml";
        private const string EDIT = "~/Views/Definitions/Users/Edit.cshtml";
        private const string RESET_PASSWORD = "~/Views/Definitions/Users/ResetPassword.cshtml";

        public UsersController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            userManager = new UserManager(bookStoreUnitOfWork);
        }

        // GET: Users
        public async Task<ActionResult> Index()
        {
            return View(INDEX,await userManager.GetAllNoTrackingAsync());
        }

        public async Task<ActionResult> ResetPassword(string id)
        {
            SelectUserViewModel user;

            //If id null return bas request
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user =await userManager.GettNoTrackingAsync(id);

            //If user is null return notfound
            if (user==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            //Security check: if user is manager return badrequest
            if (await bookStoreUnitOfWork.UserManager.IsInRoleAsync(user.Id,RoleNames.MANAGER))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.User = user;

            return View(RESET_PASSWORD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id, BookStoreModel.ViewModels.Users.ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Reset the password
                    var provider = new DpapiDataProtectionProvider("BookStore");

                    bookStoreUnitOfWork.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                        provider.Create("RestPasswordToken"));

                    string restPasswordToken = bookStoreUnitOfWork.UserManager.GeneratePasswordResetToken(resetPasswordViewModel.Id);
                    bookStoreUnitOfWork.UserManager.ResetPassword(resetPasswordViewModel.Id, restPasswordToken, resetPasswordViewModel.NewPassword);

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    
                }
            }

            SelectUserViewModel user;

            //Get user
            user = await userManager.GettNoTrackingAsync(resetPasswordViewModel.Id);

            //If user is null return notfound
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            //Store user in the viewbag
            ViewBag.User = user;

            return View(RESET_PASSWORD, resetPasswordViewModel);
        }
        
        //Toggel user activity
        public async Task<ActionResult> UserToggleActive(string id)
        {
            ApplicationUser user;

            //Get the user
            user = await bookStoreUnitOfWork.Users.GetAsync(id);

            if (user!=null)
            {
                //Check if the selected user is not manager
                if (!(await bookStoreUnitOfWork.UserManager.IsInRoleAsync(user.Id,RoleNames.MANAGER)))
                {
                    //Toggle the lockout
                    user.LockoutEnabled = !user.LockoutEnabled;
                    user.LockoutEndDateUtc = new DateTime(9999, 12, 31);

                    await bookStoreUnitOfWork.CompleteAsync();
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Users/Create
        public async Task<ActionResult> Create()
        {
            //Return roles except manager role
            //and save it in viewbag for using it to dropdownlist
            ViewBag.Role = new SelectList((await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync()).Where(r => r.Name != RoleNames.MANAGER).ToList()
                , "Name", "Name");
            return View(CREATE);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel createUserViewModel)
        {
            if (ModelState.IsValid)
            {
                if (await userManager.CreateUserAsync(createUserViewModel))
                {
                    return RedirectToAction("Index");
                }
            }

            //Return roles except manager role
            //and save it in viewbag for using it to dropdownlist
            ViewBag.Role = new SelectList((await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync()).Where(r => r.Name != RoleNames.MANAGER).ToList()
                , "Name", "Name",createUserViewModel.Role);

            return View(CREATE, createUserViewModel);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            SelectUserViewModel user;
            EditUserViewModel editUser;

            user =await userManager.GettNoTrackingAsync(id);

            if (user==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (user.Roles.Contains(RoleNames.MANAGER))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            editUser = user;

            //Return roles except manager role
            //and save it in viewbag for using it to dropdownlist
            ViewBag.Role = new SelectList((await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync()).Where(r => r.Name != RoleNames.MANAGER).ToList()
                , "Name", "Name", editUser.Role);

            return View(EDIT, editUser);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (await userManager.EditUserAsync(user))
                {
                    return RedirectToAction("Index");
                }
            }

            //Return roles except manager role
            //and save it in viewbag for using it to dropdownlist
            ViewBag.Role = new SelectList((await bookStoreUnitOfWork.Roles.GetAllNoTrackingAsync()).Where(r => r.Name != RoleNames.MANAGER).ToList()
                , "Name", "Name", user.Role);

            return View(EDIT, user);
        }
    }
}
