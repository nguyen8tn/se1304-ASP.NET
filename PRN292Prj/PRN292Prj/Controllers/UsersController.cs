using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PRN292Prj.Data;

namespace PRN292Prj.Models
{
    public class UsersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly PRN292PrjContext _context;

        public UsersController(IConfiguration configuration, PRN292PrjContext context)
        {
            this.configuration = configuration;
            _context = context;
        }
        public IActionResult Index()
        {
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            List<UserIndexPage> list = data.SearchProductNewArrival();
            foreach (UserIndexPage item in list)
            {
                item.Img += cloud.GetSAS();
            }
            ViewBag.PList = list;
            UserIndexPage uIP = new UserIndexPage();
            //null ref
            uIP.ListScale = new List<SelectListItem>();
            AddToComboBox(uIP,"");
            return View(uIP);
        }
        public IActionResult BestSeller(UserIndexPage model)
        {
            if (model.Search == null)
            {
                model.Search = "";
            }
            if (model.Scale == null)
            {
                model.Scale = "";
            }
            UserIndexPage uIP = new UserIndexPage
            {
                Search = model.Search,
                ListScale = new List<SelectListItem>()
            };
            AddToComboBox(uIP, model.Scale);
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            List<UserIndexPage> list = data.SearchProductBestSale();
            foreach (UserIndexPage item in list)
            {
                item.Img += cloud.GetSAS();
            }
            ViewBag.PList = list;
            return View("Index", uIP);
        }
        public IActionResult ViewDetails(string id)
        {
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            Product p = data.GetProductDetails(id);
            p.Img += cloud.GetSAS();
            return View("Product",p);
        }
        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Username == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            string role = CheckLogin(user);
            if (role == null)
            {
                return View("~/Views/Account/Login.cshtml", user);
            }
            else if (role.Equals("admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string CheckLogin(User user)
        {
            string result = null;
            var cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_CheckLogin";
            var param = cmd.CreateParameter();
            param.ParameterName = "@Username";
            param.Value = user.Username;

            var param2 = cmd.CreateParameter();
            param2.ParameterName = "@Password";
            param2.Value = user.Password;
            cmd.Parameters.Add(param);
            cmd.Parameters.Add(param2);

            _context.Database.OpenConnection();
            var rs = cmd.ExecuteReader();
            if (rs.Read())
            {
                result = rs.GetString(rs.GetOrdinal("Role"));
            }
            return result;
        }
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult Search(UserIndexPage model)
        {
            if (model.Search == null)
            {
                model.Search = "";
            }
            if (model.Scale == null)
            {
                model.Scale = "";
            }
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            List <UserIndexPage> list = data.SearchProductByUser(model.Search, model.Scale);
            foreach (var item in list)
            {
                item.Img += cloud.GetSAS();
            }
            UserIndexPage uIP = new UserIndexPage();
            uIP.Search = model.Search;
            uIP.ListScale = new List<SelectListItem>();
            AddToComboBox(uIP, model.Scale);
            if (list.Count == 0)
            {
                ViewBag.PList = null;
            }
            else {ViewBag.PList = list; }
            return View(uIP);
        }
        private void AddToComboBox(UserIndexPage uIP, string scaleID)
        {
            DataAccess dataAccess = new DataAccess(configuration);
            List<Scale> list = dataAccess.GetAllScales();
            foreach (var i in list)
            {
                SelectListItem t = new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                };
                if (i.ID.ToString().Equals(scaleID))
                {
                    t.Selected = true;
                }
                uIP.ListScale.Add(t);
            }
        }
        public IActionResult ViewProfile()
        {
            string username = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var profile = _context.User.FirstOrDefault(t => t.Username == username);
            return View("Profile", profile);
        }
        public IActionResult Update(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["UpdateF"] = "Update Fail";
                return View("Profile", user);
            }
            var obj = _context.User.SingleOrDefault(t => t.Username.Equals(user.Username));
            obj.Name = user.Name;
            obj.Gender = user.Gender;
            obj.Email = user.Email;
            try
            {
                _context.SaveChanges();
                TempData["UpdateS"] = "Update Success";
                return RedirectToAction("ViewProfile", obj);
            }
            catch (Exception)
            {
                TempData["UpdateF"] = "Update Fail";
                return View("ViewProfile", user);
            }
        }
    }
}
