using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechFix_Computer_Shop_System.PAL;
using System.Net.Http;
using Newtonsoft.Json;

namespace TechFix_Computer_Shop_System.TechFix_Client.Admin
{
    public partial class ManageUsers : Form
    {
        private Timer timer1;

        private readonly HttpClient client = new HttpClient();
        private string apiBaseUrl = "http://localhost:52450/api/Users";

        public ManageUsers()
        {
            InitializeComponent();
            LoadUsers();

            this.Load += new EventHandler(ManageUsers_Load);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(UpdateTimeAndDate);
            timer1.Start();
        }

        private void ManageUsers_Load(object sender, EventArgs e)
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

        private async void LoadUsers()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiBaseUrl);
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                List<Users> users = JsonConvert.DeserializeObject<List<Users>>(responseData);

                dgvUsers.DataSource = users;

                // 🔹 Fix Column Widths to Fit 730px
                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                dgvUsers.Columns["Id"].Width = 72;
                dgvUsers.Columns["Username"].Width = 182;
                dgvUsers.Columns["Role"].Width = 110;
                dgvUsers.Columns["Email"].Width = 220;
                dgvUsers.Columns["Contact"].Width = 143;

                foreach (DataGridViewColumn column in dgvUsers.Columns)
                {
                    column.Resizable = DataGridViewTriState.False;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();
            this.Hide();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ManageUsers manageUsers = new ManageUsers();
            manageUsers.Show();
            this.Hide();

            MessageBox.Show("You are already in the Manage Users section!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
        }

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newUser = new Users
            {
                Username = txtUsername.Text,
                Password_Hash = txtPassword.Text, // Consider encrypting before sending
                Role = cmbRole.Text,
                Email = txtEmail.Text,
                Contact = txtContact.Text
            };

            try
            {
                string json = JsonConvert.SerializeObject(newUser);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiBaseUrl, content);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{apiBaseUrl}/{userId}");
                response.EnsureSuccessStatusCode();

                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            await SearchUsers(txtSearch.Text);
        }

        private async Task SearchUsers(string searchTerm)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/search?search={searchTerm}");
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                List<Users> users = JsonConvert.DeserializeObject<List<Users>>(responseData);

                dgvUsers.Invoke((MethodInvoker)(() => dgvUsers.DataSource = users));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password_Hash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

    }
}
