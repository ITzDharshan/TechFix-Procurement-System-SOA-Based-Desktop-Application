using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using TechFixShopAPI.DataAccees;
using TechFixShopAPI.Models;

namespace TechFixShopAPI.Controllers
{
    public class OrdersController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        public List<Orders> GetOrders(string supplierUsername = null)
        {
            DataTable dt = new DataTable();
            List<Orders> orderList = new List<Orders>();

            string query = "SELECT * FROM Orders";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            // If supplierUsername is provided, filter by SupplierUsername
            if (!string.IsNullOrEmpty(supplierUsername))
            {
                query += " WHERE SupplierUsername = @SupplierUsername";
                cmd.Parameters.AddWithValue("@SupplierUsername", supplierUsername);
            }

            cmd.CommandText = query;

            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                orderList.Add(new Orders
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    SupplierUsername = row["SupplierUsername"].ToString(),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    Discount = row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0,
                    Status = row["Status"].ToString(),
                    OrderDate = row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : (DateTime?)null
                });
            }
            return orderList;
        }



        [HttpGet]
        public Orders GetOrder(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Orders WHERE OrderId = @OrderId", con);
            cmd.Parameters.AddWithValue("@OrderId", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Orders order = null;
            if (reader.Read())
            {
                order = new Orders
                {
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    SupplierUsername = reader["SupplierUsername"].ToString(),
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["Price"]),
                    Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : 0,
                    Status = reader["Status"].ToString(),
                    OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : (DateTime?)null
                };
            }
            con.Close();
            return order;
        }

        [HttpPost]
        public async Task<IHttpActionResult> InsertOrder([FromBody] Orders order)
        {
            if (order == null)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                using (SqlConnection con = DataAccessLayer.CreateConnection())
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Orders (SupplierUsername, ItemName, Quantity, Price, Discount, Status, OrderDate) 
                VALUES (@SupplierUsername, @ItemName, @Quantity, @Price, @Discount, @Status, @OrderDate);
                SELECT SCOPE_IDENTITY();", con))
                    {
                        cmd.Parameters.AddWithValue("@SupplierUsername", order.SupplierUsername);
                        cmd.Parameters.AddWithValue("@ItemName", order.ItemName);
                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        cmd.Parameters.AddWithValue("@Price", order.Price);
                        cmd.Parameters.AddWithValue("@Discount", (object)order.Discount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", order.Status ?? "Pending");
                        cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate == null ? DateTime.UtcNow : order.OrderDate);

                        object insertedId = await cmd.ExecuteScalarAsync();
                        return Ok(new { Id = insertedId });
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error inserting order: " + ex.Message));
            }
        }


        [HttpPut]
        public int UpdateOrder(int id, [FromBody] Orders order)
        {
            // Ensure the connection is opened
            con.Open();

            // SQL query to update only the status of the order
            SqlCommand cmd = new SqlCommand("UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId", con);

            // Add parameters for OrderId and Status
            cmd.Parameters.AddWithValue("@OrderId", id);
            cmd.Parameters.AddWithValue("@Status", order.Status); // Using the status update object

            try
            {
                // Execute the query
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error: " + ex.Message);
                return 0; // Return 0 in case of error
            }
            finally
            {
                // Ensure the connection is closed after execution
                con.Close();
            }
        }


        [HttpDelete]
        public int DeleteOrder(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE OrderId = @OrderId", con);
            cmd.Parameters.AddWithValue("@OrderId", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }
    }
}
