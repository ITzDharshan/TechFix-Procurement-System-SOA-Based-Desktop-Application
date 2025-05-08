using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechFix_Computer_Shop_System.PAL;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace TechFix_Computer_Shop_System.TechFix_Client.Supplier
{
    public partial class ProductStore : Form

    {
        private Timer timer1;

        private readonly HttpClient client = new HttpClient();
        private string productApiUrl = "http://localhost:52450/api/Products";

        public ProductStore()
        {
            InitializeComponent();
            LoadProducts();

            this.Load += new EventHandler(ProductStore_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();
        }


        private void ProductStore_Load(object sender, EventArgs e)
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

        private async void LoadProducts()
        {
            try
            {
                string supplierUsername = Login.LoggedInUsername;
                HttpResponseMessage response = await client.GetAsync($"{productApiUrl}/{supplierUsername}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Products>>(data);
                    dataGridView5.DataSource = products;
                    // Adjust DataGridView column widths
                    dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dataGridView5.Columns["ProductId"].Width = 95;
                    dataGridView5.Columns["ItemName"].Width = 238;
                    dataGridView5.Columns["Quantity"].Width = 129;
                    dataGridView5.Columns["Price"].Width = 130;
                    dataGridView5.Columns["Discount"].Width = 130;

                    // Prevent column resizing
                    foreach (DataGridViewColumn column in dataGridView5.Columns)
                    {
                        column.Resizable = DataGridViewTriState.False;
                    }
                }
                else
                {
                    MessageBox.Show("Error: Could not retrieve products.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void flowPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTimeAndDate_Click(object sender, EventArgs e)
        {

        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string supplierUsername = Login.LoggedInUsername;
            if (!string.IsNullOrEmpty(textBoxSearch.Text))
            {
                await SearchProduct(textBoxSearch.Text, supplierUsername);
            }
        }


        private async Task SearchProduct(string searchTerm, string supplierUsername)
        {
            try
            {
                // Corrected API request URL with proper query parameters
                HttpResponseMessage response = await client.GetAsync($"{productApiUrl}/search?supplierUsername={supplierUsername}&search={searchTerm}");
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                List<Products> products = JsonConvert.DeserializeObject<List<Products>>(responseData);

                dataGridView5.Invoke((MethodInvoker)(() => dataGridView5.DataSource = products));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();

            this.Close();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();

            this.Hide();
        }

        private void btnOrderManage_Click(object sender, EventArgs e)
        {
            OrderManagement orderManagement = new OrderManagement();
            orderManagement.Show();

            this.Hide();
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            ProductStore productStore = new ProductStore();
            productStore.Show();

            this.Hide();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SupplierDashboard supplierDashboard = new SupplierDashboard();
            supplierDashboard.Show();

            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxItemName.Text))
            {
                MessageBox.Show("Please enter an item name to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string itemName = textBoxItemName.Text;
                string supplierUsername = Login.LoggedInUsername;

                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete '{itemName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    HttpResponseMessage response = await client.DeleteAsync($"{productApiUrl}?itemName={itemName}&supplierUsername={supplierUsername}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Product Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts(); // Refresh the product list
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageBox.Show("Error: Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Error: Could not delete the product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void btnAddItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxItemName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPrice.Text) ||
                string.IsNullOrWhiteSpace(textBoxDiscount.Text) ||
                numericUpDownQuantity.Value <= 0)
            {
                MessageBox.Show("Please fill in all fields and ensure Quantity is greater than zero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newProduct = new
            {
                ItemName = textBoxItemName.Text,
                Quantity = (int)numericUpDownQuantity.Value,
                Price = decimal.Parse(textBoxPrice.Text),
                Discount = decimal.Parse(textBoxDiscount.Text),
                SupplierUsername = Login.LoggedInUsername
            };

            try
            {
                string json = JsonConvert.SerializeObject(newProduct);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(productApiUrl, content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProducts(); // Refresh product list after adding
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

    public class Products
    {
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string SupplierUsername { get; set; }
    }

}