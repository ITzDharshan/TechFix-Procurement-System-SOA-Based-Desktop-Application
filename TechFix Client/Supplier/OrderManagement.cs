using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TechFix_Computer_Shop_System.PAL;
using TechFix_Computer_Shop_System.TechFix_Client.Admin;

namespace TechFix_Computer_Shop_System.TechFix_Client.Supplier
{
    public partial class OrderManagement : Form
    {
        private Timer timer1;

        private static readonly HttpClient client = new HttpClient();
        private string baseApiUrl = "http://localhost:52450/api/";

        public OrderManagement()
        {
            InitializeComponent();
            LoadOrderManagement();
            this.Load += new EventHandler(OrderManagement_Load);

            LoadQuotations();
            this.Load += new EventHandler(Quotations_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();

            dvgOrders.CellContentClick += new DataGridViewCellEventHandler(dvgOrders_CellContentClick);
            dvgQuotations.CellContentClick += new DataGridViewCellEventHandler(dvgQuotations_CellContentClick);
        }

        private async void LoadOrderManagement()
        {
            string supplierUsername = Login.LoggedInUsername;

            try
            {
                string url = $"{baseApiUrl}Orders?supplierUsername={supplierUsername}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Orders>>(content);

                // Bind data to the DataGridView
                dvgOrders.DataSource = orders;

                // Add the ComboBox column for status (Pending, Shipping, Delivered)
                if (!dvgOrders.Columns.Contains("StatusComboBox"))
                {
                    DataGridViewComboBoxColumn statusComboBoxColumn = new DataGridViewComboBoxColumn();
                    statusComboBoxColumn.Name = "StatusComboBox";
                    statusComboBoxColumn.HeaderText = "Select";
                    statusComboBoxColumn.DataSource = new string[] { "Pending", "Shipping", "Delivered" };
                    statusComboBoxColumn.DataPropertyName = "Status";

                    dvgOrders.Columns.Add(statusComboBoxColumn);
                }

                // Add the Update Status button as a new column
                if (!dvgOrders.Columns.Contains("UpdateStatus"))
                {
                    DataGridViewButtonColumn btnUpdateStatus = new DataGridViewButtonColumn();
                    btnUpdateStatus.Name = "UpdateStatus";
                    btnUpdateStatus.HeaderText = "Update Status";
                    btnUpdateStatus.Text = "Update";
                    btnUpdateStatus.UseColumnTextForButtonValue = true;

                    dvgOrders.Columns.Add(btnUpdateStatus);
                }

                dvgOrders.Columns["StatusComboBox"].DisplayIndex = dvgOrders.Columns["UpdateStatus"].DisplayIndex - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order summary: " + ex.Message);
            }
        }

        private async void dvgOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dvgOrders.Columns["UpdateStatus"].Index)
            {
                int rowIndex = e.RowIndex;

                if (rowIndex >= 0)
                {
                    // Get the selected status from the ComboBox cell
                    string selectedStatus = dvgOrders.Rows[rowIndex].Cells["StatusComboBox"].Value?.ToString().Trim();

                    // Define valid statuses
                    string[] validStatuses = { "Pending", "Shipping", "Delivered" };

                    // Check if the selected status is valid
                    if (!validStatuses.Contains(selectedStatus))
                    {
                        MessageBox.Show("Invalid status selected. Please choose a valid status.");
                        return;
                    }

                    int orderId = Convert.ToInt32(dvgOrders.Rows[rowIndex].Cells["OrderId"].Value);

                    // Show a confirmation message
                    MessageBox.Show($"Updating Order ID: {orderId} with Status: {selectedStatus}");

                    // Prepare the data to update
                    var updateData = new
                    {
                        Status = selectedStatus
                    };

                    try
                    {
                        // API URL for the order update endpoint
                        string url = $"{baseApiUrl}Orders/{orderId}";

                        // Serialize data to JSON format
                        var content = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");

                        // Perform the API PUT request to update the order status
                        HttpResponseMessage response = await client.PutAsync(url, content);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Order status updated successfully!");
                            LoadOrderManagement(); // Optionally, refresh the orders list
                        }
                        else
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Error updating order status: {response.StatusCode} - {errorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Show error message in case of any exceptions
                        MessageBox.Show($"Error updating order status: {ex.Message}");
                    }
                }
            }
        }



        private void OrderManagement_Load(object sender, EventArgs e)
        {
            lblUsername.Text = Login.LoggedInUsername;
        }

        private async void LoadQuotations()
        {
            string supplierUsername = Login.LoggedInUsername;

            try
            {
                string url = $"{baseApiUrl}Quotations?supplierUsername={Login.LoggedInUsername}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var quotations = JsonConvert.DeserializeObject<List<Quotations>>(content);

                // Bind data to DataGridView
                dvgQuotations.DataSource = quotations;

                // Set column widths
                dvgQuotations.Columns["Id"].Width = 72;
                dvgQuotations.Columns["SupplierUsername"].Width = 182;
                dvgQuotations.Columns["ItemName"].Width = 146;
                dvgQuotations.Columns["Price"].Width = 92;
                dvgQuotations.Columns["Response"].Width = 182;
                dvgQuotations.Columns["Status"].Width = 92;
                dvgQuotations.Columns["RequestDate"].Width = 132;

                foreach (DataGridViewColumn column in dvgQuotations.Columns)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }

                // Set AutoSizeRowsMode to automatically adjust row heights based on content
                dvgQuotations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Optionally, set a default height for rows
                dvgQuotations.RowTemplate.Height = 120;


                dvgQuotations.Columns["Response"].ReadOnly = false;

                if (!dvgQuotations.Columns.Contains("StatusComboBox"))
                {
                    DataGridViewComboBoxColumn statusComboBoxColumn = new DataGridViewComboBoxColumn();
                    statusComboBoxColumn.Name = "StatusComboBox";
                    statusComboBoxColumn.HeaderText = "Status";
                    statusComboBoxColumn.DataSource = new string[] { "Pending", "Responded" };

                    statusComboBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

                    dvgQuotations.Columns.Add(statusComboBoxColumn);
                }

                if (!dvgQuotations.Columns.Contains("Update"))
                {
                    DataGridViewButtonColumn btnUpdateStatus = new DataGridViewButtonColumn();
                    btnUpdateStatus.Name = "Update";
                    btnUpdateStatus.HeaderText = "Update";
                    btnUpdateStatus.Text = "Update";
                    btnUpdateStatus.UseColumnTextForButtonValue = true;

                    btnUpdateStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

                    dvgQuotations.Columns.Add(btnUpdateStatus);
                }

                dvgQuotations.Columns["StatusComboBox"].DisplayIndex = dvgQuotations.Columns["Update"].DisplayIndex - 1;
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

        private async void dvgQuotations_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the "Update" button in the column was clicked
            if (e.ColumnIndex == dvgQuotations.Columns["Update"].Index)
            {
                int rowIndex = e.RowIndex;

                // Ensure a valid row index is selected
                if (rowIndex >= 0)
                {
                    // Get the selected status and response from the cells
                    string selectedStatus = dvgQuotations.Rows[rowIndex].Cells["StatusComboBox"].Value?.ToString().Trim();
                    string responseText = dvgQuotations.Rows[rowIndex].Cells["Response"].Value?.ToString().Trim();

                    // Define valid statuses
                    string[] validStatuses = { "Pending", "Responded" };

                    // Check if a valid status is selected
                    if (string.IsNullOrEmpty(selectedStatus) || !validStatuses.Contains(selectedStatus))
                    {
                        MessageBox.Show("Please select a valid status before updating.");
                        return;
                    }

                    // Check if the selected status is 'Pending' (cannot be updated)
                    if (selectedStatus == "Pending")
                    {
                        MessageBox.Show("Status is 'Pending' and cannot be updated until changed.");
                        return;
                    }

                    // Ensure a response message is entered before updating
                    if (string.IsNullOrEmpty(responseText))
                    {
                        MessageBox.Show("Please enter a response message before updating.");
                        return;
                    }

                    // Get the quotation ID
                    int quotationId = Convert.ToInt32(dvgQuotations.Rows[rowIndex].Cells["Id"].Value);

                    // Create an object to send the updated data
                    var updateData = new
                    {
                        Status = selectedStatus,
                        Response = responseText
                    };

                    try
                    {
                        // Prepare the URL for the API call
                        string url = $"{baseApiUrl}Quotations/{quotationId}";

                        // Create the HTTP content with the updated data
                        var content = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");

                        // Send the PUT request to update the quotation
                        HttpResponseMessage response = await client.PutAsync(url, content);

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Show a success message
                        MessageBox.Show("Quotation status and response updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Handle errors and show the error message
                        MessageBox.Show("Error updating quotation: " + ex.Message);
                    }
                }
            }
        }



        private void UpdateTimeAndDate(object sender, EventArgs e)
        {
            // Get Sri Lanka's current date and time (UTC +5:30)
            DateTime sriLankaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Sri Lanka Standard Time");

            // Format the date and time
            lblTimeAndDate.Text = sriLankaTime.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SupplierDashboard supplierDashboard = new SupplierDashboard();
            supplierDashboard.Show();

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

        private void btnReport_Click(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();

            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();

            this.Close();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}
