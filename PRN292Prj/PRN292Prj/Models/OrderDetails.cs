using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    [Table("tbl_OrderDetails")]
    public class OrderDetails
    {
        public string ID { get; set; }
        public string Order_ID { get; set; }
        public string Product_ID { get; set; }
        public int Quantity { get; set; }

    }
}
