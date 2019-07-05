using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PRN292Prj.Models;

namespace PRN292Prj.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]")]
    public class HomeController : Controller
    {
        //private const string AwsAccessKeyId = "ASIA3VTIDDEVDG4AAOUG";
        //private const string AwsSecretAccessKey = "oSw8ATyHjOn/XL9XKX7P/Ct3vylQDKZj+aMNkT0B";
        private const string BucketName = "se1304";
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("CreateProduct")]
        public IActionResult CreateProduct()
        {
            Data.DataAccess dataAccess = new Data.DataAccess(configuration);
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
            return View();
        }

        [Route("Insert")]
        public async Task<IActionResult> Insert(IFormFile files, Product product)
        {
            long size = files.Length;
            var filePath = Path.GetTempFileName();
            if (size > 0)
            {
                using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USGovCloudEast1))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                        PutObjectRequest request = new PutObjectRequest
                        {
                            InputStream = stream,
                            Key = files.FileName,
                            BucketName = BucketName
                        };
                        PutObjectResponse response1 = await client.PutObjectAsync(request);
                        //asas
                    }
                }
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return RedirectToAction("CreateProduct", "Home");
        }
    }
}