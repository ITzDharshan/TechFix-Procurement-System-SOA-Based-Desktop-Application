using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TechFix_Computer_Shop_System.TechFix_Client.Admin;
using TechFix_Computer_Shop_System.TechFix_Client.Supplier;

namespace TechFix_Computer_Shop_System.PAL
{
    public partial class Login : Form
    {
        public static string LoggedInUsername { get; private set; }
        private readonly HttpClient client = new HttpClient();

        public Login()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true; // Mask password initially
            checkPassword.Checked = false; // Ensure checkbox is unchecked initially
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string userRole = await GetUserRoleAsync(username, password);

            if (!string.IsNullOrEmpty(userRole))
            {
                LoggedInUsername = username; 
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (userRole == "Admin")
                {
                    AdminDashboard adminDashboard = new AdminDashboard();
                    adminDashboard.Show();
                }
                else if (userRole == "Supplier")
                {
                    SupplierDashboard supplierDashboard = new SupplierDashboard();
                    supplierDashboard.Show();
                }

                this.Hide(); // Hide login form
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> GetUserRoleAsync(string username, string password)
        {
            string role = string.Empty;

            try
            {
                // Create HTTP client
                using (HttpClient client = new HttpClient())
                {
                    // Define the API URL with the username
                    string requestUrl = $"http://localhost:52450/api/Users/search?search={username}";

                    // Send GET request to API
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response JSON to get the user data
                        string content = await response.Content.ReadAsStringAsync();
                        List<Users> users = JsonConvert.DeserializeObject<List<Users>>(content);

                        // Check if user exists and password matches
                        var user = users.FirstOrDefault(u => u.Username == username && u.Password_Hash == password);
                        if (user != null)
                        {
                            role = user.Role; // Get the role from the user data
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return role;
        }


        private void picClose_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to close?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void checkPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !checkPassword.Checked;
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