using Microsoft.Extensions.Configuration;
using PRN292Prj.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Data
{
    public class DataAccess
    {

        public IConfiguration Configuration { get; private set; }

        public DataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string checkLogin(User user)
        {
            string role = "fail";
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_CheckLogin", conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);

            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        role = rd.GetString(rd.GetOrdinal("Role"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return role;
        }
    }
}
