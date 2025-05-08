using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TechFix_Computer_Shop_System.PAL;
using TechFix_Computer_Shop_System.TechFix_Client.Admin;

namespace TechFix_Computer_Shop_System.TechFix_Client.Supplier
{
    public partial class SalesReport : Form
    {
        private Timer timer1;

        private readonly HttpClient client = new HttpClient();
        private string orderApiUrl = "http://localhost:52450/api/Orders";

        public SalesReport()
        {
            InitializeComponent();
            this.Load += new EventHandler(SalesReport_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();

        }

        private void SalesReport_Load(object sender, EventArgs e)
        {
            lblUsername.Text = Login.LoggedInUsername;
            LoadSalesReport();
            UpdateTotalSalesAmount();
        }

        private void UpdateTotalSalesAmount()
        {
            try
            {
                decimal totalAmount = 0;

                // Check if DataGridView has rows
                if (dvgReport.Rows.Count == 0)
                {
                    lblSales.Text = "No sales data available.";
                    return;
                }

                // Loop through each row in the DataGridView
                foreach (DataGridViewRow row in dvgReport.Rows)
                {
                    // Ensure the row is not a new row (empty row in DataGridView)
                    if (!row.IsNewRow)
                    {
                        // Get the value from the TotalAmount column and sum it up
                        var totalAmountCellValue = row.Cells["TotalAmount"].Value;
                        if (totalAmountCellValue != DBNull.Value)
                        {
                            decimal totalAmountForRow = Convert.ToDecimal(totalAmountCellValue);
                            totalAmount += totalAmountForRow;

                            // Debug: Log the value for each row
                            Console.WriteLine($"Row Total: {totalAmountForRow}, Running Total: {totalAmount}");
                        }
                    }
                }

                // Debug: Log the final total amount
                Console.WriteLine($"Final Total Sales: {totalAmount}");

                // Display the total amount in the lblSales label formatted as currency
                lblSales.Text = totalAmount.ToString("C2");  // Format as currency (C2)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total amount: " + ex.Message);
            }
        }


        private async Task LoadSalesReport()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(orderApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var orders = System.Text.Json.JsonSerializer.Deserialize<List<Orders>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (orders != null)
                    {
                        // Filter orders for the logged-in supplier
                        var supplierOrders = orders.Where(o => o.SupplierUsername == Login.LoggedInUsername && o.Status == "Delivered").ToList();

                        // Convert list to DataTable
                        DataTable dt = ConvertToDataTable(supplierOrders);
                        dvgReport.DataSource = dt;

                        FormatDataGridView();

                        // Update the total sales amount after data is loaded
                        UpdateTotalSalesAmount();
                    }
                    else
                    {
                        MessageBox.Show("No sales data available.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load sales report from API.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales report: " + ex.Message);
            }
        }

        private void FormatDataGridView()
        {
            // 🔹 Set Column Widths to Fit 730px
            dvgReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dvgReport.Columns["OrderID"].Width = 70;
            dvgReport.Columns["ItemName"].Width = 190;
            dvgReport.Columns["Quantity"].Width = 70;
            dvgReport.Columns["Price"].Width = 100;
            dvgReport.Columns["Discount"].Width = 70;
            dvgReport.Columns["SubTotal"].Width = 120;
            dvgReport.Columns["TotalAmount"].Width = 110;
        }

        private DataTable ConvertToDataTable(List<Orders> orders)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("SubTotal", typeof(decimal));
            dt.Columns.Add("TotalAmount", typeof(decimal));

            foreach (var order in orders)
            {
                dt.Rows.Add(order.OrderId, order.ItemName, order.Quantity, order.Price, order.Discount, order.Quantity * order.Price, (order.Quantity * order.Price) - order.Discount);
            }

            return dt;
        }



        private void UpdateTimeAndDate(object sender, EventArgs e)
        {
            // Get Sri Lanka's current date and time (UTC +5:30)
            DateTime sriLankaTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Sri Lanka Standard Time");

            // Format the date and time
            lblTimeAndDate.Text = sriLankaTime.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void label5_Click(object sender, EventArgs e)
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

    }
}