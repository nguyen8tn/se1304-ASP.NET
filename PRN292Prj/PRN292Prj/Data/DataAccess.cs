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
            bool check = false ;
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_insertUser",conn);
            cmd.Parameters.AddWithValue("@Username",user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Gender", user.Gender);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@DateOfCreate", user.DOC);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
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
        public bool InsertProduct(Product product)
        {
            bool check = false;
            string connectionString = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_InsertProduct", connection);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Img", product.Img);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Des", product.Description);
            cmd.Parameters.AddWithValue("@Scale_ID", product.Scale);
            cmd.Parameters.AddWithValue("@Release", product.Release);
            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return check;
        }
        public List<Product> GetAllProduct()
        {
            List<Product> list = new List<Product>();
            string connStr = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("sp_GetAllProduct", conn);
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
                        string name = rd["PName"].ToString();
                        double price = Double.Parse(rd["price"].ToString());
                        string img = rd["img"].ToString();
                        string scale = rd["SName"].ToString();
                        Product product = new Product
                        {
                            ID = id,
                            Name = name,
                            Price = price,
                            Img = img,
                            Scale = scale
                        };
                        list.Add(product);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
        public List<User> GetAllUser()
        {
            List<User> list = new List<User>();
            string connStr = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("sp_GetAllUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        string username = rd["username"].ToString();
                        string name = rd["name"].ToString();
                        bool gender = (bool)rd["gender"];
                        DateTime date = Convert.ToDateTime(rd["dateofcreate"]);
                        User user = new User
                        {
                            Username = username,
                            Name = name,
                            Gender = gender,
                            DOC = date
                        };
                        list.Add(user);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
        public bool DeleteProduct(string id)
        {
            bool check = false;
            string constr = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand("sp_DeleteProduct", conn);
            cmd.Parameters.AddWithValue("@id", int.Parse(id));
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    check = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return check;
        }
        public bool DeleteUser(string id)
        {
            bool check = false;
            string constr = Configuration.GetConnectionString("PRN292PrjContext");
            SqlConnection conn = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand("sp_DeleteUser", conn);
            cmd.Parameters.AddWithValue("@id", int.Parse(id));
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    check = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return check;
        }

        public Product SearchByPrimarykey(int id)
        {
            Product product = null;
            string connStr = Configuration.GetConnectionString("Text");
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("sp_SearchByPrimarykey", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        string name = rd["name"].ToString();
                        double price = Double.Parse(rd["price"].ToString());
                        string img = rd["img"].ToString();
                        string description = rd["description"].ToString();
                        string scale = rd["scale_id"].ToString();
                        DateTime release = DateTime.Parse(rd["release_date"].ToString());
                        product = new Product
                        {
                            ID = id,
                            Name = name,
                            Price = price,
                            Description = description,
                            Img = img,
                            Scale = scale,
                            Release = release
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return product;
        }

        public bool UpdateProduct(Product product)
        {
            bool check = false;
            string connectionString = Configuration.GetConnectionString("Text");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_UpdateProduct", connection);
            cmd.Parameters.AddWithValue("@id", product.ID);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Img", product.Img);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Des", product.Description);
            cmd.Parameters.AddWithValue("@Scale_ID", product.Scale);
            cmd.Parameters.AddWithValue("@Release", product.Release);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return check;
        }

        public List<Product> SearchProductByName(string search)
        {
            List<Product> list = new List<Product>();
            string connStr = Configuration.GetConnectionString("Text");
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("sp_SearchProduct", conn);
            cmd.Parameters.AddWithValue("@Name", search);
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
                        string name = rd["PName"].ToString();
                        double price = Double.Parse(rd["price"].ToString());
                        string img = rd["img"].ToString();
                        string scale = rd["SName"].ToString();
                        Product product = new Product
                        {
                            ID = id,
                            Name = name,
                            Price = price,
                            Img = img,
                            Scale = scale
                        };
                        list.Add(product);
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
