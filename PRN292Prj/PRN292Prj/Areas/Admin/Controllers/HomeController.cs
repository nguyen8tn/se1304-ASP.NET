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
        public IActionResult Insert(List<IFormFile> files, Product product)
        {
            long size = files.Count();
            bool checkFile = false;
            var filePath = Path.GetTempFileName();
            if (size > 0)
            {
                checkFile = files[0].ContentType.Contains("ima");
            }
            var name = from t in _context.Product where t.Name == product.Name select t.Name;
            if (name.Equals(product.Name))
            {
                Console.Write(name);
                TempData["InsertP"] = "Cannot Create Product!-This Name is exsited!";
                return View("CreateProduct");
            }
            if (!ModelState.IsValid || !checkFile)
            {
                AddToComboBox();
                TempData["InsertP"] = "Cannot Create Product!";
                ViewData["ERROR"] = "Select Box(1-Img File Only)!";
                return View("CreateProduct");
            }
            var stream = new FileStream(filePath, FileMode.Open);
            files[0].CopyTo(stream);
            stream.Position = 0;
            string extension = Path.GetExtension(files[0].FileName); 
            string fileName = (product.Name + extension);
            AzureCloud cloud = new AzureCloud(configuration);
            DataAccess dataAccess = new DataAccess(configuration);
            try
            {
                string uri = cloud.UploadFile(fileName, stream);
                product.Img = uri;
                dataAccess.InsertProduct(product);
            }
            catch (Exception)
            {
                TempData["DeleteP"] = "Cannot Create Product!";
                return View("CreateProduct");
            }
            TempData["InsertS"] = "Create Product Success";
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
                    item.Img += cloud.GetSAS();
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

        [Route("Delete")]
        public IActionResult Delete(string id, string model, string img) 
        {
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            try
            {
                if (model.Equals("user"))
                {
                    data.DeleteUser(id);
                }
                else if (model.Equals("product"))
                {
                    cloud.DeleteBlob(img);
                    data.DeleteProduct(id);
                }
                TempData["DeleteS"] = "Delete Success";
                return RedirectToAction("Table", "Home", new { id = "product" });
            }
            catch (Exception)
            {
                TempData["DeleteF"] = "Delete Fail";
                return RedirectToAction("Table", "Home", new { id = "product"});
            }
               
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