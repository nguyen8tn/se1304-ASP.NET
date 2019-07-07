using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PRN292Prj.Data;
using PRN292Prj.Models;

namespace PRN292Prj.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]")]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly PRN292PrjContext _context;

        public HomeController(IConfiguration configuration, PRN292PrjContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            var listProduct = (from t in _context.Product select new Product { Name = t.Name, Price = Convert.ToDouble(t.Price),Created = t.Created}).Take(5);
            var listUser = (from t in _context.User select new User {Username = t.Username, Name = t.Name, DOC = t.DOC}).Take(5);
            var totalU = _context.User.Count();
            var totalP = _context.Product.Count();
            ViewBag.TotalU = totalU;
            ViewBag.TotalP = totalP;
            ViewBag.Products = listProduct;
            ViewBag.Users = listUser;
            return View();
        }

        [Route("CreateProduct")]
        public IActionResult CreateProduct()
        {
            AddToComboBox();
            return View();
        }

        [Route("Insert")]
        public IActionResult Insert(IFormFile files, Product product)
        {
            long size = files.Length;
            bool checkFile = false;
            var filePath = Path.GetTempFileName();
            if (size > 0)
            {
                checkFile = files.ContentType.Contains("ima");
            }
            if (!ModelState.IsValid || !checkFile)
            {
                AddToComboBox();
                ViewData["ERROR"] = "Select Box(1-Img File Only)!";
                return View("CreateProduct");
            }
            var stream = new FileStream(filePath, FileMode.Open);
            files.CopyTo(stream);
            stream.Position = 0;
            string extension = Path.GetExtension(files.FileName); 
            string fileName = (product.Name + extension);
            AzureCloud cloud = new AzureCloud(configuration);
            DataAccess dataAccess = new DataAccess(configuration);
            try
            {
                string uri = cloud.UploadFile(fileName, stream);
                product.Img = uri;
                dataAccess.InsertProduct(product);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("dup"))
                {
                    ViewData["ERROR"] = "Cannot Create Product!-This Name is exsited!";
                }
                else ViewData["ERROR"] = "Cannot Create Product!";
                return View("CreateProduct");
            }
            return RedirectToAction("CreateProduct", "Home");
        }

        [Route("Table")]
        public IActionResult Table(string id)
        {
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            if (id.Equals("product"))
            {
                List<Product> list = data.GetAllProduct();
                foreach (var item in list)
                {
                    item.Img += cloud.getSAS();
                }
                ViewBag.PList = list;
            }
            else if (id.Equals("User"))
            {
                List<User> list = data.GetAllUser();
                ViewBag.UList = list;

            } else
            {
                return RedirectToAction("Index", "Home");
            }
            return View();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
        }
        private void AddToComboBox()
        {
            DataAccess dataAccess = new DataAccess(configuration);
            List<Scale> list = dataAccess.GetAllScales();
            List<SelectListItem> item = new List<SelectListItem>();
            foreach (var i in list)
            {
                SelectListItem t = new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                };
                item.Add(t);
            }
            ViewBag.list = item;
        }
    }
}