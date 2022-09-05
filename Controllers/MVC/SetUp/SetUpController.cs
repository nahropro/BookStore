using BookStore.Data;
using BookStore.Data.Repository;
using BookStore.Service;
using BookStoreModel.Models;
using BookStoreModel.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers.MVC.SetUp
{
    public class SetUpController : Controller
    {
        private readonly BookStoreUnitOfWork bookStoreUnitOfWork;
        private readonly UserManager userManager;

        public SetUpController()
        {
            bookStoreUnitOfWork = new BookStoreUnitOfWork();
            userManager = new UserManager(bookStoreUnitOfWork);
        }

        // GET: SetUp
        public async Task<ActionResult> Index()
        {
            if (await bookStoreUnitOfWork.Users.CountAsync()!=0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("SetUp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetUp(RegisterManagerViewModel registerManager)
        {
            if (!ModelState.IsValid)
            {
                return View("SetUp", registerManager);
            }

            if (await userManager.SetUpManager(registerManager))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("SetUp", registerManager);
            }
        }
    }
}