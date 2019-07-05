using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    public class Scale
    {
        private int id;
        private string name;

        public Scale(int id, string name)
        {
            this.id = id;
            this.name = name;
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
