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

namespace TechFix_Computer_Shop_System.TechFix_Client.Admin
{
    public partial class AdminDashboard : Form
    {
        private Timer timer1;

        public AdminDashboard()
        {
            InitializeComponent();
            this.Load += new EventHandler(AdminDashboard_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            ManageUsers manageUsers = new ManageUsers();
            manageUsers.Show();

            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();

            this.Close();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();

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

        private void button1_Click_1(object sender, EventArgs e)
        {
            ManageUsers manageUsers = new ManageUsers();
            manageUsers.Show();

            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OrderSummary orderSummary = new OrderSummary();
            orderSummary.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManageInventory manageInventory = new ManageInventory();
            manageInventory.Show();

            this.Hide();
        }
    }
}
