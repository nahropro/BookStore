using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BookStore.Controllers.MVC;
using BookStore.Data.Repository;
using BookStoreModel.ViewModels.Users;
using System.Net;
using BookStoreModel.ViewModels.ManageAccount;
using BookStore.Data;
using BookStoreModel.StaticData;
using BookStoreModel.Models;

namespace BookStore.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly BookStore.Service.UserManager userManager;

        public ManageController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            userManager = new Service.UserManager(bookStoreUnitOfWork);
        }

        public ManageController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            userManager = new Service.UserManager(bookStoreUnitOfWork);
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index()
        {
            SelectUserViewModel user;

            //Get user
            user =await userManager.GettNoTrackingAsync(User.Identity.GetUserId());

            //If user is't exists return badrequest
            if (user==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(user);
        }
        
        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            //Check for validations
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Change password for the user
            var result = await bookStoreUnitOfWork.UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            //If success login again
            if (result.Succeeded)
            {
                var user = await bookStoreUnitOfWork.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        public ActionResult ChangeUserName()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserName(ChangeUserNameViewModel model)
        {
            ApplicationUser user;

            try
            {
                //Check for validation
                if (ModelState.IsValid)
                {
                    //Get currennt user
                    user = await bookStoreUnitOfWork.Users.GetAsync(User.Identity.GetUserId());

                    if (user!=null)
                    {
                        //Security check: if user has the entered password
                        if (await bookStoreUnitOfWork.UserManager.CheckPasswordAsync(user,model.PasswordForSecurity))
                        {
                            //Put username to user username and user email
                            user.UserName = model.UserName;
                            user.Email = model.UserName;
                            
                            //Save changes
                            await bookStoreUnitOfWork.CompleteAsync();

                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return View(model);
        }

        //This is special just for manager
        public async Task<ActionResult> ChangeInformation()
        {
            SelectUserViewModel user;

            //If not manager return badrequest
            if (!User.IsInRole(RoleNames.MANAGER))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the user
            user = await userManager.GettNoTrackingAsync(User.Identity.GetUserId());

            //If user not exists return badrequest
            if (user==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(new EditInfoViewModel
            {
                Address = user.Address,
                FullName = user.FullName,
                Phone = user.Phone,
            });
        }

        //This is special just for manager
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeInformation(EditInfoViewModel model)
        {
            ApplicationUser user;

            //Security check: If not manager return badrequest
            if (!User.IsInRole(RoleNames.MANAGER))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get user
            user =await bookStoreUnitOfWork.Users.GetAsync(User.Identity.GetUserId());

            //If user not exists return badrequest
            if (user==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Security check: if password is correct
            if (await bookStoreUnitOfWork.UserManager.CheckPasswordAsync(user,model.PasswordForSecurity))
            {
                //Set informations
                user.UserExtend.Address = model.Address;
                user.UserExtend.Phone = model.Phone;
                user.UserExtend.FullName = model.FullName;

                //Save changes
                await bookStoreUnitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _signInManager != null)
            {
                _signInManager.Dispose();
                _signInManager = null;
            }

            base.Dispose(disposing);
        }
    }
}