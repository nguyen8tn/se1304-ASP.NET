using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
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

        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }


        public  DateTime Release
        {
            get { return release; }
            set { release = value; }
        }


        public string Scale
        {
            get { return scale; }
            set { scale = value; }
        }



        public string Description
        {
            get { return description; }
            set { description = value; }
        }


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
