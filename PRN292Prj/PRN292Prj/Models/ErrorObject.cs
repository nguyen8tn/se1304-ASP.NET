using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Models
{
    public class ErrorObject
    {
        private string userError;
        private string passwordError;

        public ErrorObject()
        {

        }

        public string UserError
        {
            get { return userError; }
            set { userError = value; }
        }

        public string PasswordError
        {
            get { return passwordError; }
            set { passwordError = value; }
        }
    }
}
