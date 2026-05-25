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
            // Set settings button text to Logout
            if (this.btnSettings != null)
            {
                this.btnSettings.Text = "Logout";
            }
            this.Resize += Template_Resize;
        }

        private void Template_Resize(object sender, EventArgs e)
        {
            if (this.pictureBox1 != null)
            {
                this.pictureBox1.Left = (this.ClientSize.Width - this.pictureBox1.Width) / 2;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            DatabaseHelper.Logout(this);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                // We are hosted as a child view inside the master single-form container!
                // Simply close this view to return to the dashboard.
                this.Close();
                return;
            }

            // Centralized navigation fallback:
            tdDashboard dashboard = new tdDashboard();
            dashboard.FormClosed += (s, args) => this.Show();
            dashboard.Show();
            this.Hide();
        }

        private void btnCntctUs_Click(object sender, EventArgs e)
        {
            string contactInfo = "TechDrive - Support & Localized Presence\n\n" +
                                 "📍 Headquarters (HQ):\n" +
                                 "TechDrive HQ, Angeles City, Pampanga, Philippines\n\n" +
                                 "📞 Primary Contact Hotline:\n" +
                                 "09153442904\n\n" +
                                 "✉️ Corporate Email:\n" +
                                 "support.techdrive@gmail.com\n\n" +
                                 "Feel free to reach out to us for any business inquiries or support requests!";
            MessageBox.Show(contactInfo, "Contact Us - TechDrive", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            string aboutInfo = "About TechDrive\n\n" +
                               "TechDrive is a premium software-as-a-service (SaaS) platform built specifically to empower local car rental owners by delivering enterprise-grade digital fleet tracking, automated scheduling, and real-time financial insights.\n\n" +
                               "💡 Our Vision:\n" +
                               "To eliminate manual tracking friction (like paper notebooks and spreadsheets) and prevent double-bookings, all for a hyper-affordable price of just ₱166 per day.\n\n" +
                               "🚀 Project Objectives:\n" +
                               "• Digital Transformation: Integrated real-time tracking.\n" +
                               "• Operational Efficiency: Complete booking overlap prevention.\n" +
                               "• Financial Empowerment: Granular business analytics.\n\n" +
                               "Created by Rei Payumo (Admin) & team.";
            MessageBox.Show(aboutInfo, "About Us - TechDrive", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - enables double-buffering recursively
                return cp;
            }
        }
    }
}
