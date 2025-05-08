using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechFix_Computer_Shop_System.PAL;

namespace TechFix_Computer_Shop_System.TechFix_Client.Supplier
{
    public partial class SupplierDashboard : Form
    {
        private Timer timer1;

        public SupplierDashboard()
        {
            InitializeComponent();
            this.Load += new EventHandler(SupplierDashboard_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();
        }

        private void SupplierDashboard_Load(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OrderManagement orderManagement = new OrderManagement();
            orderManagement.Show();

            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductStore productStore = new ProductStore();
            productStore.Show();

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
