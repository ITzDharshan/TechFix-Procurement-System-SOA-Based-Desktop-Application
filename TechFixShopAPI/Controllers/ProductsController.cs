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
    public class ProductsController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        public List<Products> GetProducts(string supplierUsername = null)
        {
            DataTable dt = new DataTable();
            List<Products> productList = new List<Products>();

            string query = "SELECT * FROM Products";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            // If supplierUsername is provided (for suppliers), filter by SupplierUsername
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
                productList.Add(new Products
                {
                    ProductId = int.Parse(row["ProductId"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Discount = decimal.Parse(row["Discount"].ToString()),
                    SupplierUsername = row["SupplierUsername"].ToString()
                });
            }

            return productList;
        }



        [HttpGet]
        public Products GetProduct(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductId = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Products product = null;
            if (reader.Read())
            {
                product = new Products
                {
                    ProductId = (int)reader["ProductId"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    Discount = (decimal)reader["Discount"],
                    SupplierUsername = reader["SupplierUsername"].ToString()
                };
            }
            con.Close();

            return product;
        }

        [HttpPost]
        public int InsertProduct([FromBody] Products product)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Products (ItemName, Quantity, Price, Discount, SupplierUsername) VALUES (@ItemName, @Quantity, @Price, @Discount, @SupplierUsername)", con);

            cmd.Parameters.AddWithValue("@ItemName", product.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Discount", product.Discount);
            cmd.Parameters.AddWithValue("@SupplierUsername", product.SupplierUsername); // Added SupplierUsername

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpPut]
        public int UpdateProduct(int id, [FromBody] Products product)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Products SET ItemName = @ItemName, Quantity = @Quantity, Price = @Price, Discount = @Discount, SupplierUsername = @SupplierUsername WHERE ProductId = @ProductId", con);

            cmd.Parameters.AddWithValue("@ProductId", id);
            cmd.Parameters.AddWithValue("@ItemName", product.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Discount", product.Discount);
            cmd.Parameters.AddWithValue("@SupplierUsername", product.SupplierUsername); // Added SupplierUsername

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpDelete]
        public int DeleteProduct(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductId = @ProductId", con);
            cmd.Parameters.AddWithValue("@ProductId", id);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpGet]
        [Route("api/Products")]
        public async Task<IHttpActionResult> GetProducts()
        {
            List<Products> productList = new List<Products>();
            using (SqlConnection con = DataAccessLayer.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products ORDER BY ProductId", con))
            {
                try
                {
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productList.Add(new Products
                            {
                                ProductId = (int)reader["ProductId"],
                                ItemName = reader["ItemName"].ToString(),
                                Quantity = (int)reader["Quantity"],
                                Price = (decimal)reader["Price"],
                                Discount = (decimal)reader["Discount"],
                                SupplierUsername = reader["SupplierUsername"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            return Ok(productList);
        }


        [HttpPost]
        [Route("api/Products")]
        public async Task<IHttpActionResult> AddProduct([FromBody] Products product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.ItemName) ||
                product.Quantity <= 0 || product.Price <= 0)
            {
                return BadRequest("Invalid product data.");
            }

            using (SqlConnection con = DataAccessLayer.CreateConnection())
            using (SqlCommand cmd = new SqlCommand(
                "INSERT INTO Products (ItemName, Quantity, Price, Discount, SupplierUsername) VALUES " +
                "(@ItemName, @Quantity, @Price, @Discount, @SupplierUsername)", con))
            {
                cmd.Parameters.AddWithValue("@ItemName", product.ItemName);
                cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Discount", product.Discount);
                cmd.Parameters.AddWithValue("@SupplierUsername", product.SupplierUsername);

                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return Ok(new { Message = "Product added successfully" });
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [HttpDelete]
        [Route("api/Products")]
        public async Task<IHttpActionResult> DeleteProduct(string itemName, string supplierUsername)
        {
            if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(supplierUsername))
            {
                return BadRequest("Invalid product details.");
            }

            using (SqlConnection con = DataAccessLayer.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ItemName = @ItemName AND SupplierUsername = @SupplierUsername", con))
            {
                cmd.Parameters.AddWithValue("@ItemName", itemName);
                cmd.Parameters.AddWithValue("@SupplierUsername", supplierUsername);

                try
                {
                    await con.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return Ok("Product deleted successfully.");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }


        [HttpGet]
        [Route("api/Products/search")]
        public IHttpActionResult SearchProducts(string search, string supplierUsername = null)
        {
            List<Products> productList = new List<Products>();

            // Build SQL query dynamically based on whether supplierUsername is provided
            string query = "SELECT * FROM Products WHERE ItemName LIKE @Search";
            SqlCommand cmd = new SqlCommand();

            if (!string.IsNullOrEmpty(supplierUsername))
            {
                query += " AND SupplierUsername = @SupplierUsername";
                cmd.Parameters.AddWithValue("@SupplierUsername", supplierUsername);
            }

            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    productList.Add(new Products
                    {
                        ProductId = (int)reader["ProductId"],
                        ItemName = reader["ItemName"].ToString(),
                        Quantity = (int)reader["Quantity"],
                        Price = (decimal)reader["Price"],
                        Discount = (decimal)reader["Discount"],
                        SupplierUsername = reader["SupplierUsername"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                con.Close();
            }

            return Ok(productList);
        }



        [HttpGet]
        [Route("api/Products/suppliers")]
        public async Task<IHttpActionResult> GetSuppliersAsync()
        {
            List<string> supplierList = new List<string>();

            SqlCommand cmd = new SqlCommand("SELECT DISTINCT SupplierUsername FROM Products", con);

            try
            {
                await con.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    supplierList.Add(reader["SupplierUsername"].ToString());
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                con.Close();
            }

            return Ok(supplierList);
        }


        [HttpGet]
        [Route("api/Products/{supplierUsername}")]
        public IHttpActionResult GetProductsBySupplier(string supplierUsername)
        {
            List<Products> productList = new List<Products>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE SupplierUsername = @SupplierUsername", con);
            cmd.Parameters.AddWithValue("@SupplierUsername", supplierUsername);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    productList.Add(new Products
                    {
                        ProductId = (int)reader["ProductId"],
                        ItemName = reader["ItemName"].ToString(),
                        Quantity = (int)reader["Quantity"],
                        Price = (decimal)reader["Price"],
                        Discount = (decimal)reader["Discount"],
                        SupplierUsername = reader["SupplierUsername"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                con.Close();
            }

            return Ok(productList);
        }

        [HttpGet]
        [Route("api/price")]
        public IHttpActionResult GetProductPrice(string product)
        {
            SqlCommand cmd = new SqlCommand("SELECT Price FROM Products WHERE ItemName = @Product", con);
            cmd.Parameters.AddWithValue("@Product", product);

            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    decimal price = Convert.ToDecimal(result);
                    return Ok(price);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                con.Close();
            }
        }

        [HttpGet]
        [Route("api/discount")]
        public IHttpActionResult GetProductDiscount(string product)
        {
            SqlCommand cmd = new SqlCommand("SELECT Discount FROM Products WHERE ItemName = @Product", con);
            cmd.Parameters.AddWithValue("@Product", product);

            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    decimal discount = Convert.ToDecimal(result);
                    return Ok(discount);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                con.Close();
            }
        }



    }
}
