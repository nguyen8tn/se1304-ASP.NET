using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PRN292Prj.Models
{
    public class PRN292PrjContext : DbContext
    {
        public PRN292PrjContext (DbContextOptions<PRN292PrjContext> options)
            : base(options)
        {

        }

        public DbSet<PRN292Prj.Models.User> User { get; set; }

    }
}
