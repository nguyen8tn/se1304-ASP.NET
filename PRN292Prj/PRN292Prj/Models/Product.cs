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
        private int? quantity;
        private string name;
        private string img;
        private double price;
        private string description;
        private string scale;
        private DateTime? release;
        private DateTime created;

        public Product()
        {

        }

        public Product(string name, double price, string description, string scale, DateTime release, int quantity)
        {
            this.name = name;
            this.price = price;
            this.description = description;
            this.scale = scale;
            this.release = release;
            PriceEF = (decimal)price;
            this.quantity = quantity;
        }

        [Column("created_date")]
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        [Required(ErrorMessage = "Release is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Column("release_date")]
        public  DateTime? Release
        {
            get { return release; }
            set { release = value; }
        }

        [Required(ErrorMessage = "Select Scale!")]
        [NotMapped]
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

        //[RegularExpression(@"^\d+\.\d{0,2}$")]

        [NotMapped]
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

        [Column("price")]
        [Required(ErrorMessage = "Price is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a positive number")]
        public decimal PriceEF
        {
            get { return (decimal)price; }
            set { price =(double)value; }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter valid number")]
        [Required(ErrorMessage = "Quantity is required")]
        public int? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
    }
}
