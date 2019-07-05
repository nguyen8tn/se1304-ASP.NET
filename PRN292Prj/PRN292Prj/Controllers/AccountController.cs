using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN292Prj.Models;
using PRN292Prj.Data;
using Microsoft.Extensions.Configuration;

namespace PRN292Prj.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;

        public AccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return View("Login");
        }

        public IActionResult CheckLogin(User user)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }
            DataAccess dataAccess = new DataAccess(configuration);
            string role = dataAccess.CheckLogin(user);
            ViewData["Username"] = user.Username;
            if (role.Equals("admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (role.Equals("fail"))
            {
                ViewData["Invalid"] = "Username or Password is wrong";
                return View("Login");
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        public IActionResult addUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }
            else
            {
                DataAccess dataAccess = new DataAccess(configuration);
                List<string> list = dataAccess.getAllUsername();
                ErrorObject error = new ErrorObject();
                bool validate = true;
                foreach (var username in list)
                {
                    if (user.Username.Equals(username))
                    {
                        validate = false;
                        error.UserError = "Username already exist\n";
                    }
                }
                if (validate)
                {
                    bool check = dataAccess.InsertUser(user);
                    if (check == false)
                    {
                        ViewData["Invalid"] = "Register failed";
                        return View("Register");
                    }
                    else
                    {
                        ViewData["Success"] = "Registed success";
                        return View("Login");
                    }
                }
                else
                {
                    ViewData["UsernameInvalid"] = error.UserError;
                    return View("Register");
                }
            }
        }
    }
}