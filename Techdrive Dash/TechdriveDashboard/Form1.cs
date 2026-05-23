using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechdriveDashboard
{
    public partial class tdDashboard : Form
    {
        //Booking Status Conditional Formatting
        public tdDashboard()
        {
            InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the SETTINGS button");
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the HOME button");
        }

        private void btnCntctUs_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the CONTACT US button");
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the ABOUT button");
        }

        private void btnBookAV_Click(object sender, EventArgs e)
        {
            lblCst1.Text = ("Kim Vidal");
            recbookDte1.Text = ("June 16, 2024");
            recbookSts1.Text = ("Available");
            lblAvailCars.Text = ("29");
            lblCarsOut.Text = ("5");
            lblUpcoBook.Text = ("8");
            lblAlerts.Text = ("9");
            lblAlert1.Text = ("Maintenance Due for Car #20 (Toyota Avanza)");
            lblAlert2.Text = ("Maintenance Due for Car #19 (Honda BR-V)");
            lblAlert3.Text = ("Maintenance Due for Car #18 (Isuzu mu-X)");
            if (recbookSts1.Text == "Available")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(135, 226, 98);

            }
            else if (recbookSts1.Text == "Rent in Progress")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 49, 49);
            }
            else if (recbookSts1.Text == "In Maintenance")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 222, 89);
            }
            else {
                recbookSts1.ForeColor = System.Drawing.Color.White;
            }
        }
        /*
         * //135, 226, 98 - Green
         * //255, 222, 89 - Yellow
         * //255, 49, 49 - Red
         */
        private void btnVehicles_Click(object sender, EventArgs e)
        {
            lblCst1.Text = ("MC Fernandez");
            recbookDte1.Text = ("Mar 27, 2023");
            recbookSts1.Text = ("Rent in Progress");
            lblAvailCars.Text = ("22");
            lblCarsOut.Text = ("12");
            lblUpcoBook.Text = ("2");
            lblAlerts.Text = ("13");
            lblAlert1.Text = ("Maintenance Due for Car #9 (Toyota Hilux)");
            lblAlert2.Text = ("Maintenance Due for Car #11 (Toyota Hiace)");
            lblAlert3.Text = ("Maintenance Due for Car #12 (Suzuki Ertiga)");
            if (recbookSts1.Text == "Available")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(135, 226, 98);

            }
            else if (recbookSts1.Text == "Rent in Progress")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 49, 49);
            }
            else if (recbookSts1.Text == "In Maintenance")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 222, 89);
            }
            else
            {
                recbookSts1.ForeColor = System.Drawing.Color.White;
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            lblCst1.Text = ("N/A");
            recbookDte1.Text = ("Jan 28, 2025");
            recbookSts1.Text = ("In Maintenance");
            lblAvailCars.Text = ("1");
            lblCarsOut.Text = ("32");
            lblUpcoBook.Text = ("10");
            lblAlerts.Text = ("6");
            lblAlert1.Text = ("Maintenance Due for Car #5 (Mitsubishi Xpander)");
            lblAlert2.Text = ("Maintenance Due for Car #6 (Hyundai Stargazer)");
            lblAlert3.Text = ("Maintenance Due for Car #8 (Toyota Fortuner)");
            if (recbookSts1.Text == "Available")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(135, 226, 98);

            }
            else if (recbookSts1.Text == "Rent in Progress")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 49, 49);
            }
            else if (recbookSts1.Text == "In Maintenance")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 222, 89);
            }
            else
            {
                recbookSts1.ForeColor = System.Drawing.Color.White;
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            lblCst1.Text = ("James Gonzales");
            recbookDte1.Text = ("Dec 18, 2022");
            recbookSts1.Text = ("Available");
            lblAvailCars.Text = ("21");
            lblCarsOut.Text = ("2");
            lblUpcoBook.Text = ("6");
            lblAlerts.Text = ("26");
            lblAlert1.Text = ("Maintenance Due for Car #1 (Toyota Vios)");
            lblAlert2.Text = ("Maintenance Due for Car #2 (Mitsubishi Mirage G4)");
            lblAlert3.Text = ("Maintenance Due for Car #4 (Toyota Innova)");
            if (recbookSts1.Text == "Available")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(135, 226, 98);

            }
            else if (recbookSts1.Text == "Rent in Progress")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 49, 49);
            }
            else if (recbookSts1.Text == "In Maintenance")
            {
                recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 222, 89);
            }
            else
            {
                recbookSts1.ForeColor = System.Drawing.Color.White;
            }
        }
    }
}
