using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    public class UserIndexPage
    {
        public int ID  { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Remain { get; set; }
        public string Search { get; set; }
        public string Scale { get; set; }
        public List<SelectListItem> ListScale { get; set; }

    }
}
