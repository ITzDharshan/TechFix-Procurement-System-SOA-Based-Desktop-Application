using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TechFix_Computer_Shop_System.PAL;

namespace TechFix_Computer_Shop_System.TechFix_Client.Admin
{
    public partial class ManageInventory : Form
    {
        private Timer timer1;
        private readonly HttpClient client = new HttpClient();
        private string inventoryApiUrl = "http://localhost:52450/api/Inventory";
        private string productApiUrl = "http://localhost:52450/api/Products";

        public ManageInventory()
        {
            InitializeComponent();
            LoadUsers();
            LoadProducts();
            LoadInventory();
            this.Load += new EventHandler(ManageInventory_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();

        }

        private async void LoadUsers()
        {
            try
            {
                // Use HttpClient to make an API call to the Users endpoint
                HttpResponseMessage response = await client.GetAsync("http://localhost:52450/api/Users");

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the response data
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON data into a list of Users
                List<Users> users = JsonConvert.DeserializeObject<List<Users>>(responseData);

                // 🔹 Filter the list to include only users with the role "Supplier"
                List<Users> suppliers = users.Where(u => u.Role == "Supplier").ToList();

                // Bind the filtered list to the DataGridView
                dvgSuppliers.DataSource = suppliers;

                // 🔹 Fix Column Widths to Fit 730px
                dvgSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                dvgSuppliers.Columns["Id"].Width = 72;
                dvgSuppliers.Columns["Username"].Width = 182;
                dvgSuppliers.Columns["Role"].Width = 110;
                dvgSuppliers.Columns["Email"].Width = 220;
                dvgSuppliers.Columns["Contact"].Width = 143;

                // Make columns non-resizable
                foreach (DataGridViewColumn column in dvgSuppliers.Columns)
                {
                    column.Resizable = DataGridViewTriState.False;
                }
            }
            catch (Exception ex)
            {
                // Show error message if there is an issue
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void LoadInventory()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(inventoryApiUrl);
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(responseData);

                // Check for products with low inventory (quantity < 5)
                List<string> lowStockItems = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    if (quantity < 5)
                    {
                        lowStockItems.Add($"{row["ItemName"]} (Quantity: {quantity})");
                    }
                }

                if (lowStockItems.Count > 0)
                {
                    string message = "The following products have low inventory and need to be restocked:\n" + string.Join("\n", lowStockItems);
                    MessageBox.Show(message, "Low Inventory Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Bind data to DataGridView
                dvgInventory.DataSource = dt;

                // Fix Column Widths to Fit 720px
                dvgInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dvgInventory.Columns["Id"].Width = 72;
                dvgInventory.Columns["ItemName"].Width = 256;
                dvgInventory.Columns["Quantity"].Width = 102;
                dvgInventory.Columns["Price"].Width = 140;
                dvgInventory.Columns["Discount"].Width = 140;

                // Prevent Resizing
                foreach (DataGridViewColumn column in dvgInventory.Columns)
                {
                    column.Resizable = DataGridViewTriState.False;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void UpdateTimeAndDate(object sender, EventArgs e)
        {
            // Get Sri Lanka's current date and time (UTC +5:30)
            DateTime sriLankaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Sri Lanka Standard Time");

            // Format the date and time
            lblTimeAndDate.Text = sriLankaTime.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void ManageInventory_Load(object sender, EventArgs e)
        {
            lblUsername.Text = Login.LoggedInUsername;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbSuppliers.SelectedItem == null || cmbProducts.SelectedItem == null || string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    MessageBox.Show("Please fill in all fields before adding an item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string supplierUsername = cmbSuppliers.SelectedItem.ToString();
                string itemName = cmbProducts.SelectedItem.ToString();
                int quantity = int.Parse(txtQuantity.Text);
                decimal price = decimal.Parse(txtPrice.Text);
                decimal discount = string.IsNullOrWhiteSpace(txtDiscount.Text) ? 0 : decimal.Parse(txtDiscount.Text.Replace("%", ""));
                string status = "Pending";

                var orderData = new
                {
                    SupplierUsername = supplierUsername,
                    ItemName = itemName,
                    Quantity = quantity,
                    Price = price,
                    Discount = discount,
                    Status = status
                };

                string json = JsonConvert.SerializeObject(orderData);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:52450/api/Orders", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Order added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add order. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void LoadProducts()
        {
            try
            {
                // 1️⃣ Load suppliers into cmbSuppliers
                string suppliersApiUrl = "http://localhost:52450/api/Products/suppliers";
                HttpResponseMessage response = await client.GetAsync(suppliersApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var suppliers = JsonConvert.DeserializeObject<List<string>>(responseBody);

                    cmbSuppliers.Items.Clear();
                    cmbProducts.Items.Clear(); // Reset products when loading suppliers

                    foreach (var supplier in suppliers)
                    {
                        cmbSuppliers.Items.Add(supplier);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load suppliers.");
                }

                // 2️⃣ Load ALL products into DataGridView
                string productsApiUrl = "http://localhost:52450/api/Products";
                response = await client.GetAsync(productsApiUrl);
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(responseData);

                dgvProducts.DataSource = dt;

                // Set column widths
                dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvProducts.Columns["ProductId"].Width = 72;
                dgvProducts.Columns["ItemName"].Width = 216;
                dgvProducts.Columns["Quantity"].Width = 88;
                dgvProducts.Columns["Price"].Width = 108;
                dgvProducts.Columns["Discount"].Width = 108;
                dgvProducts.Columns["SupplierUsername"].Width = 128;

                // Prevent resizing
                foreach (DataGridViewColumn column in dgvProducts.Columns)
                {
                    column.Resizable = DataGridViewTriState.False;
                }

                // 3️⃣ Add "Request Quotation" Button if not already added
                if (!dgvProducts.Columns.Contains("RequestQuotation"))
                {
                    DataGridViewButtonColumn btnRequestQuotation = new DataGridViewButtonColumn
                    {
                        Name = "RequestQuotation",
                        HeaderText = "Quotation",
                        Text = "Request Quotation",
                        UseColumnTextForButtonValue = true
                    };
                    dgvProducts.Columns.Add(btnRequestQuotation);
                }

                // 4️⃣ Handle "Request Quotation" Button Click
                dgvProducts.CellContentClick += async (sender, e) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex == dgvProducts.Columns["RequestQuotation"].Index)
                    {
                        var selectedRow = dgvProducts.Rows[e.RowIndex];
                        string itemName = selectedRow.Cells["ItemName"].Value.ToString();
                        decimal price = Convert.ToDecimal(selectedRow.Cells["Price"].Value);
                        string supplierUsername = selectedRow.Cells["SupplierUsername"].Value.ToString();

                        var quotationData = new
                        {
                            ItemName = itemName,
                            Price = price,
                            SupplierUsername = supplierUsername,
                            Response = (string)null,
                            Status = "Pending",
                            RequestDate = DateTime.UtcNow // Ensure a valid date is sent
                        };

                        string json = JsonConvert.SerializeObject(quotationData);
                        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                        try
                        {
                            HttpResponseMessage quotationResponse = await client.PostAsync("http://localhost:52450/api/Quotations", content);
                            quotationResponse.EnsureSuccessStatusCode();
                            MessageBox.Show("Quotation request sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error while requesting quotation: " + ex.Message);
                        }
                    }
                };


                // 5️⃣ Handle Supplier ComboBox selection
                cmbSuppliers.SelectedIndexChanged += async (sender, e) =>
                {
                    if (cmbSuppliers.SelectedItem == null) return;

                    string selectedSupplier = cmbSuppliers.SelectedItem.ToString();
                    string productsBySupplierUrl = $"http://localhost:52450/api/Products/{selectedSupplier}";

                    try
                    {
                        HttpResponseMessage productsResponse = await client.GetAsync(productsBySupplierUrl);
                        if (productsResponse.IsSuccessStatusCode)
                        {
                            string productsResponseBody = await productsResponse.Content.ReadAsStringAsync();
                            var products = JsonConvert.DeserializeObject<List<Products>>(productsResponseBody); // Assuming Product class exists

                            cmbProducts.Items.Clear();
                            foreach (var product in products)
                            {
                                cmbProducts.Items.Add(product.ItemName);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to load products for the selected supplier.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                };

                // 6️⃣ Handle Product ComboBox selection
                cmbProducts.SelectedIndexChanged += async (sender, e) =>
                {
                    if (cmbProducts.SelectedItem == null) return;

                    string selectedProduct = cmbProducts.SelectedItem.ToString();
                    await LoadProductDetails(selectedProduct);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🆕 Load product details (Price & Discount)
        private async Task LoadProductDetails(string selectedProduct)
        {
            try
            {
                // Fetch Price
                string priceApiUrl = $"http://localhost:52450/api/price?product={selectedProduct}";
                HttpResponseMessage priceResponse = await client.GetAsync(priceApiUrl);
                if (priceResponse.IsSuccessStatusCode)
                {
                    string priceResponseBody = await priceResponse.Content.ReadAsStringAsync();
                    var price = JsonConvert.DeserializeObject<decimal>(priceResponseBody);
                    txtPrice.Text = price.ToString();
                    txtPrice.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("Failed to load price.");
                    txtPrice.Text = string.Empty;
                }

                // Fetch Discount
                string discountApiUrl = $"http://localhost:52450/api/discount?product={selectedProduct}";
                HttpResponseMessage discountResponse = await client.GetAsync(discountApiUrl);
                if (discountResponse.IsSuccessStatusCode)
                {
                    string discountResponseBody = await discountResponse.Content.ReadAsStringAsync();
                    var discount = JsonConvert.DeserializeObject<decimal>(discountResponseBody);

                    txtDiscount.Text = $"{discount}%";
                    txtDiscount.ReadOnly = false;

                }
                else
                {
                    MessageBox.Show("Failed to load discount.");
                    txtDiscount.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        private async void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchProduct.Text))
            {
                await SearchProduct(txtSearchProduct.Text); // No supplierUsername needed for admin search
            }
        }

        private async Task SearchProduct(string searchTerm)
        {
            try
            {
                // Ensure proper URL encoding for search term
                string encodedSearchTerm = Uri.EscapeDataString(searchTerm);

                // Call API without supplierUsername (admin searches all products)
                HttpResponseMessage response = await client.GetAsync($"{productApiUrl}/search?search={encodedSearchTerm}");
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                List<Products> products = JsonConvert.DeserializeObject<List<Products>>(responseData);

                // Update DataGridView safely in the UI thread
                dgvProducts.Invoke((MethodInvoker)(() => dgvProducts.DataSource = products));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private async void txtSearchInventory_TextChanged(object sender, EventArgs e)
        {
            await SearchInventory(txtSearchInventory.Text);
        }

        private async Task SearchInventory(string searchTerm)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{inventoryApiUrl}/search?search={searchTerm}");
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                List<Inventory> inventory = JsonConvert.DeserializeObject<List<Inventory>>(responseData);

                dvgInventory.Invoke((MethodInvoker)(() => dvgInventory.DataSource = inventory));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            txtPrice.ReadOnly = true;
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

    public class Inventory
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
