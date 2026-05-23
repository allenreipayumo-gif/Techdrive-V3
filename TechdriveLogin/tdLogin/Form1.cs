using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Remember Me: restore saved username if it was checked before
            if (Properties.Settings.Default.RememberMe)
            {
                tbUsername.Text = Properties.Settings.Default.SavedUsername;
                cbRemeber.Checked = true;
                // Move focus to password since username is prefilled
                tbPassword.Focus();
            }
            else
            {
                tbUsername.Focus();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            var users = new Dictionary<string, string>
            {
                // Username and password combinations
                { "admin", "adminpassword" },
                { "Rei", "PayumoPassword" },
                { "Kim", "VidalPassword" },
                { "MC", "FernandezPassword" },
                { "James", "GonzalesPassword" }
            };

            string inputUsername = tbUsername.Text.Trim();
            string inputPassword = tbPassword.Text;

            if (string.IsNullOrEmpty(inputUsername))
            {
                tbUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(inputPassword))
            {
                tbPassword.Focus();
                return;
            }

            bool isValidUser = DatabaseHelper.ValidateUser(inputUsername, inputPassword);

            // Fallback to local dictionary for offline testing/development
            if (!isValidUser)
            {
                isValidUser = users.TryGetValue(inputUsername, out string correctPassword) && correctPassword == inputPassword;
            }

            if (isValidUser)
            {
                // Remember Me: save or clear username based on checkbox state
                Properties.Settings.Default.RememberMe = cbRemeber.Checked;
                Properties.Settings.Default.SavedUsername = cbRemeber.Checked ? inputUsername : "";
                Properties.Settings.Default.Save();

                // 1. CREATE the dashboard form
                tdDashboard dashboard = new tdDashboard();

                // 2. SET UP the behavior (Show Login again when Dashboard closes)
                dashboard.FormClosed += (s, args) =>
                {
                    this.Show();
                    // If remember me is off, clear fields for next login
                    if (!cbRemeber.Checked)
                    {
                        tbUsername.Text = "";
                    }
                    tbPassword.Text = "";
                    tbUsername.Focus();
                };

                // 3. SHOW the dashboard
                dashboard.Show();

                // 4. HIDE the login form
                this.Hide();

                // Clear password field (but keep username if Remember Me is on)
                tbPassword.Text = "";
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPassword.Text = "";
                tbPassword.Focus();
            }
        }

        // Enter key on username field moves focus to password field
        private void tbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                tbPassword.Focus();
            }
        }

        // Enter key on password field triggers login
        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                DoLogin();
            }
        }

        private void cbUnhidepassword_CheckedChanged(object sender, EventArgs e)
        {
            tbPassword.UseSystemPasswordChar = !cbUnhidepassword.Checked;
        }

        private void lblLinkForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Please coordinate with your business agent for password retrieval.",
                "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
