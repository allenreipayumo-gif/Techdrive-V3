using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechdriveLogin.tdLogin
{
    public partial class Template : Form
    {
        public Template()
        {
            InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            // 1. Create the dashboard instance
            // Ensure the class name matches 'tdDashboard' exactly
            tdDashboard dashboard = new tdDashboard();

            // 2. Set the "Return to Login" logic
            // This makes Form1 show up again when the dashboard is closed
            dashboard.FormClosed += (s, args) => this.Show();

            // 3. Show the dashboard
            dashboard.Show();

            // 4. Hide the login form
            this.Hide();
        }

        private void btnCntctUs_Click(object sender, EventArgs e)
        {

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {

        }
    }
}
