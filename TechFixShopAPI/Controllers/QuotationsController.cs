using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using TechFixShopAPI.DataAccees;
using TechFixShopAPI.Models;

namespace TechFixShopAPI.Controllers
{
    public class QuotationsController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        // Get all Quotations
        public List<Quotations> GetQuotations(string supplierUsername = null)
        {
            DataTable dt = new DataTable();
            List<Quotations> quotationList = new List<Quotations>();

            string query = "SELECT * FROM Quotations";

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
                quotationList.Add(new Quotations
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ItemName = row["ItemName"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    SupplierUsername = row["SupplierUsername"].ToString(),
                    Response = row["Response"] != DBNull.Value ? row["Response"].ToString() : null,
                    Status = row["Status"].ToString(),
                    RequestDate = row["RequestDate"] != DBNull.Value ? Convert.ToDateTime(row["RequestDate"]) : DateTime.MinValue
                });
            }
            return quotationList;
        }


        [HttpGet]
        public Quotations GetQuotation(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Quotations WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Quotations quotation = null;
            if (reader.Read())
            {
                quotation = new Quotations
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    ItemName = reader["ItemName"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    SupplierUsername = reader["SupplierUsername"].ToString(),
                    Response = reader["Response"] != DBNull.Value ? reader["Response"].ToString() : null,
                    Status = reader["Status"].ToString(),
                    RequestDate = reader["RequestDate"] != DBNull.Value ? Convert.ToDateTime(reader["RequestDate"]) : DateTime.MinValue
                };
            }
            con.Close();
            return quotation;
        }


        [HttpPost]
        public int InsertQuotation([FromBody] Quotations quotation)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Quotations (ItemName, Price, SupplierUsername, Response, Status, RequestDate) " +
                    "VALUES (@ItemName, @Price, @SupplierUsername, @Response, @Status, @RequestDate)", con);

                cmd.Parameters.AddWithValue("@ItemName", quotation.ItemName);
                cmd.Parameters.AddWithValue("@Price", quotation.Price);
                cmd.Parameters.AddWithValue("@SupplierUsername", quotation.SupplierUsername);
                cmd.Parameters.AddWithValue("@Response", string.IsNullOrEmpty(quotation.Response) ? (object)DBNull.Value : quotation.Response);
                cmd.Parameters.AddWithValue("@Status", quotation.Status);
                cmd.Parameters.AddWithValue("@RequestDate", quotation.RequestDate);

                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting quotation: " + ex.Message);
                return 0;
            }
            finally
            {
                con.Close();
            }
        }

        [HttpPut]
        public int UpdateQuotation(int id, [FromBody] Quotations quotation)
        {
            // Ensure the connection is opened
            con.Open();

            // SQL query to update only the Response and Status of the quotation
            SqlCommand cmd = new SqlCommand(
                "UPDATE Quotations SET Response = @Response, Status = @Status WHERE Id = @Id",
                con);

            // Add parameters for Id, Response, and Status
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Response", string.IsNullOrEmpty(quotation.Response) ? (object)DBNull.Value : quotation.Response); // Handle null or empty Response
            cmd.Parameters.AddWithValue("@Status", quotation.Status); // Using the status update object

            try
            {
                // Execute the query
                int result = cmd.ExecuteNonQuery();
                return result; // Return the number of rows affected (1 if successful)
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
        public int DeleteQuotation(int id)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Quotations WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);

                int result = cmd.ExecuteNonQuery();
                return result; // Returns the number of rows affected
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting quotation: " + ex.Message);
                return 0; // Return 0 to indicate failure
            }
            finally
            {
                con.Close();
            }
        }

    }
}
