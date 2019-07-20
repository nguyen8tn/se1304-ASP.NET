using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN292Prj.Models;
using PRN292Prj.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Users");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            HttpContext.Session.SetString("name", user.Username);
            if (!role.Equals("fail"))
            {
                AddCookieAuth(user,role);
                if (role.Equals("admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Users");
            }
            else
            {
                ViewData["Invalid"] = "Username or Password is wrong";
                return View("Login");
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
        private void AddCookieAuth(User user,string role)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(30),
            };
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }
    }
}