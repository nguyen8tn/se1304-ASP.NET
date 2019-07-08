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
        private readonly PRN292PrjContext _context;

        public AccountController(IConfiguration configuration, PRN292PrjContext context)
        {
            this.configuration = configuration;
            _context = context;
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

        public IActionResult addUser(User user, string confirm)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }
            else
            {
                DataAccess dataAccess = new DataAccess(configuration);
                var name = from t in _context.User where t.Username == user.Username select t.Name;
                if (name.Equals(user.Username))
                {
                    TempData["Register"] = "This Username is existed";
                    return View("Register");
                }
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
                //else
                //{
                //    ViewData["UsernameInvalid"] = error.UserError;
                //    return View("Register");
                //}
            }
        }
    }
}