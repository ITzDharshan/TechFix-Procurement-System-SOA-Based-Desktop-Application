using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using TechFixShopAPI.DataAccees;
using TechFixShopAPI.Models;

namespace TechFixShopAPI.Controllers
{
    public class InventoryController : ApiController
    {
        SqlConnection con = DataAccessLayer.CreateConnection();

        public List<Inventory> GetItems()
        {
            DataTable dt = new DataTable();
            List<Inventory> itemList = new List<Inventory>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Inventory", con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                itemList.Add(new Inventory
                {
                    Id = int.Parse(row["Id"].ToString()),
                    ItemName = row["ItemName"].ToString(),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Discount = decimal.Parse(row["Discount"].ToString())
                });
            }
            return itemList;
        }

        [HttpGet]
        public Inventory GetItem(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Inventory WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Inventory item = null;
            if (reader.Read())
            {
                item = new Inventory
                {
                    Id = (int)reader["Id"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    Discount = (decimal)reader["Discount"]
                };
            }
            con.Close();
            return item;
        }

        [HttpPost]
        public IHttpActionResult InsertInventory([FromBody] Inventory inv)
        {
            // Check if the item already exists in the inventory
            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Inventory WHERE ItemName = @ItemName", con);
            checkCmd.Parameters.AddWithValue("@ItemName", inv.ItemName);
            con.Open();
            int itemExists = (int)checkCmd.ExecuteScalar();
            con.Close();

            if (itemExists > 0)
            {
                // Return a custom message with the item name if it already exists
                return BadRequest($"{inv.ItemName} is already added to inventory.");
            }

            // If the item doesn't exist, proceed to add it
            SqlCommand cmd = new SqlCommand("INSERT INTO Inventory (ItemName, Quantity, Price, Discount) VALUES " +
                "(@ItemName, @Quantity, @Price, @Discount)", con);
            cmd.Parameters.AddWithValue("@ItemName", inv.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", inv.Quantity);
            cmd.Parameters.AddWithValue("@Price", inv.Price);
            cmd.Parameters.AddWithValue("@Discount", inv.Discount);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();

            if (result > 0)
            {
                return Ok("Item added to inventory successfully.");
            }
            else
            {
                return BadRequest("Failed to add item to inventory.");
            }
        }



        [HttpPut]
        public int UpdateInventory(int id, [FromBody] Inventory inv)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Inventory SET ItemName=@ItemName, Quantity=@Quantity, Price=@Price, Discount=@Discount WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ItemName", inv.ItemName);
            cmd.Parameters.AddWithValue("@Quantity", inv.Quantity);
            cmd.Parameters.AddWithValue("@Price", inv.Price);
            cmd.Parameters.AddWithValue("@Discount", inv.Discount);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpDelete]
        public int DeleteInventory(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Inventory WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        [HttpGet]
        [Route("api/inventory/search")]
        public IHttpActionResult SearchInventory(string search)
        {
            List<Inventory> itemList = new List<Inventory>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Inventory WHERE ItemName LIKE @Search OR CAST(Price AS NVARCHAR) LIKE @Search OR CAST(Discount AS NVARCHAR) LIKE @Search", con);
            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                itemList.Add(new Inventory
                {
                    Id = (int)reader["Id"],
                    ItemName = reader["ItemName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    Discount = (decimal)reader["Discount"]
                });
            }
            con.Close();

            return Ok(itemList);
        }
    }
}
