using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    [Table("tbl_Product")]
    public class Product
    {
        private int id;
        private string name;
        private string img;
        private double price;
        private string description;
        private string scale;
        private DateTime release;
        private DateTime created;

        public Product()
        {

        }

        public Product(string name, double price, string description, string scale, DateTime release)
        {
            this.name = name;
            this.price = price;
            this.description = description;
            this.scale = scale;
            this.release = release;
        }

        [Column("created_date")]
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        [Required(ErrorMessage = "Release is required")]
        [DataType(DataType.Date)]
        public  DateTime Release
        {
            get { return release; }
            set { release = value; }
        }

        [Required(ErrorMessage = "Select Scale!")]
        public string Scale
        {
            get { return scale; }
            set { scale = value; }
        }


        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description must be a string with min.length = 3, max.length = 500", MinimumLength = 3)]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency, ErrorMessage = "Price must be a number")]
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public string Img
        {
            get { return img; }
            set { img = value; }
        }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be a string with min.length = 3, max.length = 50", MinimumLength = 3)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public int ID
        {
            get { return id; }
            set { id = value; }
        }

    }
}
