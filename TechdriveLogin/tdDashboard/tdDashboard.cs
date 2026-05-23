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
    public partial class tdDashboard : Form
    {
        //Booking Status Conditional Formatting
        public tdDashboard()
        {
            InitializeComponent();
            this.Load += (s, e) => RefreshDashboardData();
        }

        private void RefreshDashboardData()
        {
            try
            {
                // 1. Fetch fleet stats from CockroachDB
                var stats = DatabaseHelper.GetFleetStats();
                lblAvailCars.Text = stats["Available"].ToString();
                lblCarsOut.Text = stats["RentInProgress"].ToString();
                
                // 2. Fetch upcoming bookings count
                int upcomingCount = DatabaseHelper.GetUpcomingBookingsCount();
                lblUpcoBook.Text = upcomingCount.ToString();

                // 3. Fetch latest booking details for the tracking panel
                var latestBooking = DatabaseHelper.GetLatestBooking();
                if (latestBooking != null)
                {
                    lblCst1.Text = latestBooking.CustomerName;
                    recbookDte1.Text = latestBooking.BookingDate.ToString("MMM dd, yyyy");
                    recbookSts1.Text = latestBooking.Status;

                    // Color code the status label
                    string currentSts = latestBooking.Status;
                    if (currentSts == "Available" || currentSts == "Confirmed")
                    {
                        recbookSts1.ForeColor = System.Drawing.Color.FromArgb(135, 226, 98); // Green
                    }
                    else if (currentSts == "Rent in progress" || currentSts == "Rent in Progress" || currentSts == "Discarded")
                    {
                        recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 49, 49); // Red
                    }
                    else if (currentSts == "In Maintenance" || currentSts == "Draft")
                    {
                        recbookSts1.ForeColor = System.Drawing.Color.FromArgb(255, 222, 89); // Yellow
                    }
                    else
                    {
                        recbookSts1.ForeColor = System.Drawing.Color.White;
                    }
                }
                else
                {
                    lblCst1.Text = "No Bookings";
                    recbookDte1.Text = "N/A";
                    recbookSts1.Text = "N/A";
                    recbookSts1.ForeColor = System.Drawing.Color.White;
                }

                // 4. Check and generate warning alerts
                var alerts = DatabaseHelper.CheckAndGenerateBookingAlerts();
                lblAlerts.Text = alerts.Count.ToString();

                // Display up to 3 alerts on the dashboard
                lblAlert1.Text = alerts.Count > 0 ? alerts[0] : "No active alerts";
                lblAlert2.Text = alerts.Count > 1 ? alerts[1] : "";
                lblAlert3.Text = alerts.Count > 2 ? alerts[2] : "";
            }
            catch (Exception)
            {
                // Fallback to static mockup values on any error
                lblCst1.Text = "Offline Mode";
                recbookDte1.Text = DateTime.Today.ToString("MMM dd, yyyy");
                recbookSts1.Text = "Offline";
                recbookSts1.ForeColor = System.Drawing.Color.White;
                
                lblAvailCars.Text = "11";
                lblCarsOut.Text = "0";
                lblUpcoBook.Text = "0";
                lblAlerts.Text = "0";
                lblAlert1.Text = "Database connection offline.";
                lblAlert2.Text = "";
                lblAlert3.Text = "";
            }
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
            string contactInfo = "TechDrive - Support & Localized Presence\n\n" +
                                 "📍 Headquarters (HQ):\n" +
                                 "TechDrive HQ, Angeles City, Pampanga, Philippines\n\n" +
                                 "📞 Primary Contact Hotline:\n" +
                                 "09153442904\n\n" +
                                 "✉️ Corporate Email:\n" +
                                 "support@techdrive.com\n\n" +
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

        private void btnBookAV_Click(object sender, EventArgs e)
        {
            bookAvehicle bookingForm = new bookAvehicle();
            bookingForm.FormClosed += (s, args) => this.Show();
            bookingForm.Show();
            this.Hide();
        }
        /*
         * //135, 226, 98 - Green
         * //255, 222, 89 - Yellow
         * //255, 49, 49 - Red
         */
        private void btnVehicles_Click(object sender, EventArgs e)
        {
            {
                Vehicles vehicleForm = new Vehicles();
                vehicleForm.FormClosed += (s, args) => this.Show();
                vehicleForm.Show();
                this.Hide();
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            // Open the real-time Vercel tracker URL in the default browser
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://techdrive-tracker-ooum8alvn-reiyourva.vercel.app/",
                    UseShellExecute = true // Required for .NET Core / modern Windows compatibility
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Could not launch default web browser automatically. Please visit:\nhttps://techdrive-tracker-ooum8alvn-reiyourva.vercel.app/", 
                                "Web Tracker Link", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            {
               
                Reports reportsForm = new Reports();
                reportsForm.FormClosed += (s, args) => this.Show();
                reportsForm.Show();
                this.Hide();
            }
        }
    }
}
