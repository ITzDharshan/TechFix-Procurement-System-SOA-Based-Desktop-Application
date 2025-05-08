using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TechFix_Computer_Shop_System.PAL;

namespace TechFix_Computer_Shop_System.TechFix_Client.Admin
{

    public partial class OrderSummary : Form
    {
        private Timer timer1;

        private static readonly HttpClient client = new HttpClient();

        // API Base URL
        string baseUrl = "http://localhost:52450/api/";

        public OrderSummary()
        {
            InitializeComponent();
            LoadOrderSummary();
            this.Load += new EventHandler(OrderSummary_Load);

            LoadQuotations();
            this.Load += new EventHandler(Quotations_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();

            dvgOrderSummary.CellContentClick += new DataGridViewCellEventHandler(dvgOrderSummary_CellContentClick);
        }

        private async void LoadOrderSummary()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(baseUrl + "Orders"); // API call to get orders
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response to a list of orders
                var orders = JsonConvert.DeserializeObject<List<Orders>>(responseBody);

                // Create a DataTable to bind to DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("OrderId");
                dt.Columns.Add("SupplierUsername");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Price");
                dt.Columns.Add("Discount");
                dt.Columns.Add("Status");
                dt.Columns.Add("OrderDate");

                foreach (var order in orders)
                {
                    dt.Rows.Add(order.OrderId, order.SupplierUsername, order.ItemName, order.Quantity, order.Price, order.Discount, order.Status, order.OrderDate);
                }

                dvgOrderSummary.DataSource = dt;

                // Set column widths
                dvgOrderSummary.Columns["OrderId"].Width = 72;
                dvgOrderSummary.Columns["SupplierUsername"].Width = 182;
                dvgOrderSummary.Columns["ItemName"].Width = 146;
                dvgOrderSummary.Columns["Quantity"].Width = 92;
                dvgOrderSummary.Columns["Price"].Width = 92;
                dvgOrderSummary.Columns["Discount"].Width = 92;
                dvgOrderSummary.Columns["Status"].Width = 92;
                dvgOrderSummary.Columns["OrderDate"].Width = 132;

                // Add "Add to Inventory" button column if not already added
                if (!dvgOrderSummary.Columns.Contains("AddToInventory"))
                {
                    DataGridViewButtonColumn btnAddToInventory = new DataGridViewButtonColumn
                    {
                        Name = "AddToInventory",
                        HeaderText = "Update",
                        Text = "Add to Inventory",
                        UseColumnTextForButtonValue = true
                    };
                    dvgOrderSummary.Columns.Add(btnAddToInventory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order summary: " + ex.Message);
            }
        }

        private async void dvgOrderSummary_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dvgOrderSummary.Columns["AddToInventory"].Index && e.RowIndex >= 0)
            {
                string status = dvgOrderSummary.Rows[e.RowIndex].Cells["Status"].Value.ToString();
                if (status == "Delivered")
                {
                    string itemName = dvgOrderSummary.Rows[e.RowIndex].Cells["ItemName"].Value.ToString();
                    int quantity = Convert.ToInt32(dvgOrderSummary.Rows[e.RowIndex].Cells["Quantity"].Value);
                    decimal price = Convert.ToDecimal(dvgOrderSummary.Rows[e.RowIndex].Cells["Price"].Value);
                    decimal discount = Convert.ToDecimal(dvgOrderSummary.Rows[e.RowIndex].Cells["Discount"].Value);

                    try
                    {
                        // Check if item already exists in inventory
                        HttpResponseMessage checkResponse = await client.GetAsync(baseUrl + $"Inventory/Exists?itemName={itemName}");
                        if (checkResponse.IsSuccessStatusCode)
                        {
                            bool itemExists = JsonConvert.DeserializeObject<bool>(await checkResponse.Content.ReadAsStringAsync());
                            if (itemExists)
                            {
                                // Display message with item name
                                MessageBox.Show($"{itemName} is already added to inventory.");
                                return;
                            }
                        }

                        // Add item to inventory
                        var inventoryData = new
                        {
                            ItemName = itemName,
                            Quantity = quantity,
                            Price = price,
                            Discount = discount
                        };

                        HttpContent content = new StringContent(JsonConvert.SerializeObject(inventoryData), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(baseUrl + "Inventory", content);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Item added to inventory successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Error: " + await response.Content.ReadAsStringAsync());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding item to inventory: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Only 'Delivered' orders can be added to inventory.");
                }
            }
        }



        private void OrderSummary_Load(object sender, EventArgs e)
        {
            lblUsername.Text = Login.LoggedInUsername;
        }

        private async void LoadQuotations()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(baseUrl + "Quotations"); // API call to get quotations
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response to a list of quotations
                var quotations = JsonConvert.DeserializeObject<List<Quotations>>(responseBody);

                // Create a DataTable to bind to DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("SupplierUsername");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("Price");
                dt.Columns.Add("Response");
                dt.Columns.Add("Status");
                dt.Columns.Add("RequestDate");

                foreach (var quotation in quotations)
                {
                    dt.Rows.Add(quotation.Id, quotation.SupplierUsername, quotation.ItemName, quotation.Price, quotation.Response, quotation.Status, quotation.RequestDate);
                }

                dvgQuotations.DataSource = dt;

                // Set column widths
                dvgQuotations.Columns["Id"].Width = 72;
                dvgQuotations.Columns["SupplierUsername"].Width = 182;
                dvgQuotations.Columns["ItemName"].Width = 146;
                dvgQuotations.Columns["Price"].Width = 92;
                dvgQuotations.Columns["Response"].Width = 182;
                dvgQuotations.Columns["Status"].Width = 92;
                dvgQuotations.Columns["RequestDate"].Width = 132;

                // Set vertical and horizontal alignment to Top-Left for all columns
                foreach (DataGridViewColumn column in dvgQuotations.Columns)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }

                // Set the AutoSizeRowsMode to adjust the row height based on content
                dvgQuotations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Adjust the row height for multiline text
                // You can manually adjust this if needed, but AutoSizeRowsMode should handle it.
                // dvgQuotations.RowTemplate.Height = 120;  // You can remove this if AutoSizeRowsMode works
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading quotations: " + ex.Message);
            }
        }


        private void Quotations_Load(object sender, EventArgs e)
        {
            lblUsername.Text = Login.LoggedInUsername;
        }

        private void UpdateTimeAndDate(object sender, EventArgs e)
        {
            // Get Sri Lanka's current date and time (UTC +5:30)
            DateTime sriLankaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Sri Lanka Standard Time");

            // Format the date and time
            lblTimeAndDate.Text = sriLankaTime.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ManageUsers manageUsers = new ManageUsers();
            manageUsers.Show();

            this.Hide();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            ManageInventory manageInventory = new ManageInventory();
            manageInventory.Show();

            this.Hide();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            OrderSummary orderSummary = new OrderSummary();
            orderSummary.Show();

            this.Hide();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();

            this.Hide();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();

            this.Close();
        }
    }

    public class Orders
    {
        public int OrderId { get; set; }
        public string SupplierUsername { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class Quotations
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string SupplierUsername { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
