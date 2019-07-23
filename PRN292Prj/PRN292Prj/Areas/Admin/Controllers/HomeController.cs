using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PRN292Prj.Data;
using PRN292Prj.Models;

namespace PRN292Prj.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]")]
    [Authorize(Roles = "admin")]
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
            var listProduct = (from t in _context.Product orderby t.Created descending select new Product { Name = t.Name, Price = Convert.ToDouble(t.Price), Created = t.Created }).Take(3);
            var listUser = (from t in _context.User orderby t.DOC descending select new User { Username = t.Username, Name = t.Name, DOC = t.DOC }).Take(3);
            var listOrder = (from t in _context.Order orderby t.DOC descending select new Order { Username = t.Username, ID = t.ID, DOC = t.DOC }).Take(3);
            //---------
            var totalU = _context.User.Count();
            var totalP = _context.Product.Count();
            var totalO = _context.Order.Count();
            //---------------- get number record
            ViewBag.TotalU = totalU;
            ViewBag.TotalP = totalP;
            ViewBag.TotalO = totalO;
            //----------- get top 3
            ViewBag.Products = listProduct;
            ViewBag.Users = listUser;
            ViewBag.Orders = listOrder;
            return View();
        }

        [Route("CreateProduct")]
        public IActionResult CreateProduct()
        {
            AddToComboBox("");
            return View();
        }

        [Route("ProductDetail")]
        public IActionResult ProductDetail(int id)
        {
            DataAccess dataAccess = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            Product product = dataAccess.SearchByPrimarykey(id);
            product.Img += cloud.GetSAS();
            AddToComboBox(product.Scale);
            return View("ProductDetail", product);
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
            var name = _context.Product.FirstOrDefault(t => t.Name.Equals(product.Name));
            AddToComboBox("");
            if (name != null)
            {
                TempData["InsertF"] = "Cannot Create Product!";
                ModelState.AddModelError("Name", "This name is existed!");
                return View("CreateProduct");
            }
            if (!ModelState.IsValid)
            {
                TempData["InsertF"] = "Cannot Create Product!";
                return View("CreateProduct");
            }
            if (!checkFile)
            {
                TempData["InsertF"] = "Cannot Create Product!";
                ModelState.AddModelError("Img", "Select Box(1-Img File Only)!");
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
            catch (Exception e)
            {
                TempData["InsertF"] = e.Message;
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
            ViewBag.table = id;
            if (id.Equals("product"))
            {
                TempData["id"] = "product";
                List<Product> list = data.GetAllProduct();
                foreach (var item in list)
                {
                    //img = (img + token)
                    item.Img += cloud.GetSAS();
                }
                ViewBag.PList = list;
            }
            else if (id.Equals("user"))
            {
                TempData["id"] = "user";
                List<User> list = data.GetAllUser();
                ViewBag.UList = list;
            }
            else if (id.Equals("order"))
            {
                TempData["id"] = "order";
                List<Order> list = data.GetAllOrder();
                ViewBag.OList = list;
            }
            else
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
                return RedirectToAction("Table", "Home", new { id = "product" });
            }

        }

        [Route("Update")]
        public IActionResult Update(List<IFormFile> files, [Bind("Created,ID,Quantity,Name,Img,Price,Description,Scale,Release")]Product product)
        {
            long size = files.Count();
            bool checkFile;
            var filePath = Path.GetTempFileName();
            if (size > 0)
            {
                checkFile = files[0].ContentType.Contains("ima");
                if (!checkFile)
                {
                    TempData["UpdateF"] = "Cannot Update Product!";
                    ViewData["ERROR"] = "1-Img File Only";
                    return View("ProductDetail");
                }
            }
            else
            {
                product.Img = product.Img.Split("?sv=")[0];
            }
            if (!ModelState.IsValid)
            {
                TempData["UpdateF"] = "Cannot Update Product!";
                return View("ProductDetail");
            }
            AzureCloud cloud = new AzureCloud(configuration);
            DataAccess dataAccess = new DataAccess(configuration);
            try
            {
                if (size > 0)
                {
                    var stream = new FileStream(filePath, FileMode.Open);
                    files[0].CopyTo(stream);
                    stream.Position = 0;
                    string extension = Path.GetExtension(files[0].FileName);
                    string fileName = (product.Name + extension);
                    cloud.DeleteBlob(product.Img);
                    string uri = cloud.UploadFile(fileName, stream);
                    product.Img = uri;
                }
                dataAccess.UpdateProduct(product);
                TempData["UpdateS"] = "Update Product Success!";
                return RedirectToAction("ProductDetail", "Home", new { id = product.ID });
            }
            catch (Exception e)
            {
                TempData["UpdateF"] = "Cannot Update Product!" + e.Message;
                return View("ProductDetail");
            }              
        }
        [Route("Search")]
        public IActionResult Search(string search, string id, string from, string to)
        {
            if (search == null)
            {
                search = "";
            }
            TempData["id"] = id;
            DataAccess data = new DataAccess(configuration);
            AzureCloud cloud = new AzureCloud(configuration);
            if (id.Equals("product"))
            {
                List<Product> list = data.SearchProductByName(search);
                foreach (var item in list)
                {
                    //img = (img + token)
                    item.Img += cloud.GetSAS();
                }
                ViewBag.PList = list;
            }
            else if (id.Equals("user"))
            {
                List<User> list = data.SearchUserByName(search);
                ViewBag.UList = list;
            }
            else
            {
                List<Order> list = new List<Order>();
                if (from == null && to == null)
                {
                    list = (from t in _context.Order select t).ToList();
                }
                else if (to == null)
                {
                    list = (from t in _context.Order where t.DOC > DateTime.Parse(@from) select t).ToList();
                }
                else if (from == null)
                {
                    list = (from t in _context.Order where DateTime.Parse(to) > t.DOC select t).ToList();
                }
                else
                {
                    list = (from t in _context.Order where DateTime.Parse(to) > t.DOC && t.DOC > DateTime.Parse(@from) select t).ToList();
                }
                if (list.Count() == 0)
                {
                    list = null;
                }
                ViewBag.OList = list;
            }
            return View("Table");
        }
        private void AddToComboBox(string scaleID)
        {
            DataAccess dataAccess = new DataAccess(configuration);
            List<Scale> list = dataAccess.GetAllScales();
            List<SelectListItem> item = new List<SelectListItem>();

            foreach (var i in list)
            {
                SelectListItem t = new SelectListItem();
                t.Text = i.Name;
                t.Value = i.ID.ToString();
                if (i.Equals(scaleID))
                {
                    t.Selected = true;
                }
                item.Add(t);
            }
            ViewBag.list = item;
        }
        [Route("Profile")]
        public IActionResult Profile()
        {
            string username = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var profile = _context.User.FirstOrDefault(t => t.Username == username);
            return View(profile);
        }
        [Route("OrderDetail")]
        public IActionResult OrderDetail(string id)
        {
            DataAccess data = new DataAccess(configuration);
            List<OrderDetails> orders = data.GetAllOrderDetails(id);
            ViewBag.OList = orders;
            ViewData["order_id"] = id;
            return View();
        }
        [Route("UpdateProfile")]
        public IActionResult UpdateProfile(User user)
        {
            if (!ModelState.IsValid)
            {
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
                return RedirectToAction("Profile", obj);
            }
            catch (Exception)
            {
                TempData["UpdateF"] = "Update Fail";
                return View("Profile", user);
            }
        }
    }
}