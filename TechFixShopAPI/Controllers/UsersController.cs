using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using TechFixShopAPI.DataAccees;
using TechFixShopAPI.Models;

namespace TechFixShopAPI.Controllers
{
    public class UsersController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        public List<Users> GetUsers()
        {
            DataTable dt = new DataTable();
            List<Users> userList = new List<Users>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Users", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                userList.Add(new Users
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Username = row["Username"].ToString(),
                    Password_Hash = row["Password_Hash"].ToString(),
                    Role = row["Role"].ToString(),
                    Email = row["Email"].ToString(),
                    Contact = row["Contact"].ToString()
                });
            }
            return userList;
        }

        [HttpGet]
        public Users GetUser(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Users user = null;
            if (reader.Read())
            {
                user = new Users
                {
                    Id = (int)reader["Id"],
                    Username = reader["Username"].ToString(),
                    Password_Hash = reader["Password_Hash"].ToString(),
                    Role = reader["Role"].ToString(),
                    Email = reader["Email"].ToString(),
                    Contact = reader["Contact"].ToString()
                };
            }
            con.Close();
            return user;
        }

        [HttpPost]
        public int InsertUser([FromBody] Users user)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Users " +
                                "(Username, Password_Hash, Role, Email, Contact) VALUES (@Username, @PasswordHash, @Role, @Email, @Contact)", con);

            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.Password_Hash);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Contact", user.Contact);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpPut]
        public int UpdateUser(int id, [FromBody] Users user)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Users SET Username=@Username, Password_Hash=@PasswordHash, Role=@Role, Email=@Email, Contact=@Contact WHERE Id=@Id", con);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.Password_Hash);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Contact", user.Contact);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpDelete]
        public int DeleteUser(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpGet]
        [Route("api/users/search")]
        public IHttpActionResult SearchUsers(string search)
        {
            List<Users> userList = new List<Users>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Username LIKE @Search OR Email LIKE @Search OR Contact LIKE @Search", con);
            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(new Users
                {
                    Id = (int)reader["Id"],
                    Username = reader["Username"].ToString(),
                    Password_Hash = reader["Password_Hash"].ToString(),
                    Role = reader["Role"].ToString(),
                    Email = reader["Email"].ToString(),
                    Contact = reader["Contact"].ToString()
                });
            }
            con.Close();

            return Ok(userList);
        }

        [HttpGet]
        [Route("api/users/login")]
        public IHttpActionResult LoginUser(string username, string password)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", con);
            cmd.Parameters.AddWithValue("@Username", username);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Users user = null;
            if (reader.Read())
            {
                user = new Users
                {
                    Id = (int)reader["Id"],
                    Username = reader["Username"].ToString(),
                    Password_Hash = reader["Password_Hash"].ToString(),
                    Role = reader["Role"].ToString(),
                    Email = reader["Email"].ToString(),
                    Contact = reader["Contact"].ToString()
                };

            }
            con.Close();

            return Unauthorized();
        }

    }
}

