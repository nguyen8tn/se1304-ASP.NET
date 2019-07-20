using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    [Table("tbl_Order")]
    public class Order
    {
        public int ID { get; set; }
        public string Username { get; set; }
        [Column("created_date")]
        public DateTime? DOC { get; set; }
    }
}
