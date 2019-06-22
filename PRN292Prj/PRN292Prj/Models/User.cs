using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    public class User
    {
        [Key]
        private string username;
        private string name;
        private string password;
        private string email;
        private string role;
        private bool? gender;
        private DateTime? Doc;

        public User()
        {

        }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public User(string username, string name, string password, string email, string role, bool gender, DateTime doc)
        {
            this.username = username;
            this.name = name;
            this.password = password;
            this.email = email;
            this.role = role;
            this.gender = gender;
            Doc = doc;
        }

        [Column("DateOfCreate")]
        public DateTime? DOC
        {
            get { return Doc; }
            set { Doc = value; }
        }

        public bool? Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public string Role
        {
            get { return role; }
            set { role = value; }
        }


        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [Required]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [Key]
        [Required]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
    }
}
