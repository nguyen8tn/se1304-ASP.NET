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
            string role = dataAccess.checkLogin(user);
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
    }
}