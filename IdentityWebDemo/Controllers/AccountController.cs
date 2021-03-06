﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace IdentityWebDemo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<ExtendedUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendedUser>>();
        public SignInManager<ExtendedUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendedUser, string>>();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var signInStatus = await SignInManager.PasswordSignInAsync(model.Username, model.Password, true, true);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            //Do this to avoid "username is already taken" validation
            //begin
            var identityUser = await UserManager.FindByNameAsync(model.Username);
            if(identityUser != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //end

            var user = new ExtendedUser
            {
                UserName = model.Username,
                FullName = model.FullName
            };
            user.Addresses.Add(new Address { AddressLine = model.AddressLine, Country = model.Country, UserId = user.Id });
            var identityResult = await UserManager.CreateAsync(user, model.Password);

            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());

            return View(model);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
    }
    
}