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

        public List<string> getAllUsername()
        {
            List<string> list = new List<string>();
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_getAllUsername", conn);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string username = dr.GetString(dr.GetOrdinal("Username"));
                    list.Add(username);
                }
            }
            return list;
        }
        public string CheckLogin(User user)
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

        public bool InsertUser(User user)
        {
            DateTime now = DateTime.Now;
            user.DOC = now;
            bool check = true;
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_insertUser", conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Role", "User");
            cmd.Parameters.AddWithValue("@DateOfCreate", user.DOC);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                check = false;
            }
            return check;
        }
        public List<Scale> GetAllScales()
        {
            List<Scale> list = new List<Scale>();
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_GetAllScales", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        int id = int.Parse(rd["id"].ToString());
                        string name = rd["name"].ToString();
                        Scale t = new Scale(id, name);
                        list.Add(t);
                    }
                }
            }          
            catch (Exception)
            {
                throw;
            }
            return list;
        }
    }
}
