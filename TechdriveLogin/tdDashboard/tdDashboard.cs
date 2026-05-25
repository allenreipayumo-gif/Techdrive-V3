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
        private Panel mainContainerPanel;

        public tdDashboard()
        {
            InitializeComponent();
            InitializeContainerPanel();
            this.Load += (s, e) => RefreshDashboardData();
            
            // Background timer to automatically check for expiring bookings every 5 minutes
            Timer expiryCheckTimer = new Timer();
            expiryCheckTimer.Interval = 300000; // 5 minutes
            expiryCheckTimer.Tick += (s, e) => {
                DatabaseHelper.CheckAndSendExpiringEmails();
            };
            expiryCheckTimer.Start();

            // Real-time background timer to automatically refresh dashboard data every 30 seconds
            Timer dashboardRefreshTimer = new Timer();
            dashboardRefreshTimer.Interval = 30000; // 30 seconds
            dashboardRefreshTimer.Tick += (s, e) => {
                if (mainContainerPanel != null && !mainContainerPanel.Visible)
                {
                    RefreshDashboardData();
                }
            };
            dashboardRefreshTimer.Start();
            
            // Hook FormClosed to cleanly terminate all processes synchronously only if not logging out
            this.FormClosed += (s, e) =>
            {
                if (!DatabaseHelper.IsLoggingOut)
                {
                    Application.Exit();
                }
            };
            
            // Set settings button text to Logout
            if (this.btnSettings != null)
            {
                this.btnSettings.Text = "Logout";
            }

            // Set dashboard label from "Cars Out" to "In Maintenance:"
            if (this.label18 != null)
            {
                this.label18.Text = "In Maintenance:";
                if (this.lblCarsOut != null)
                {
                    Size size = TextRenderer.MeasureText(this.label18.Text, this.label18.Font);
                    this.lblCarsOut.Location = new Point(this.label18.Left + size.Width + 5, this.lblCarsOut.Location.Y);
                }
            }

            // Hook Resize event for fullscreen/maximize adaptivity
            this.Resize += TdDashboard_Resize;

            // Wire alert-related controls to trigger the detailed maintenance dialog
            var alertControls = new Control[]
            {
                this.lblAlerts, this.guna2Panel1, this.label20,
                this.guna2Panel4, this.lblAlert1, this.lblAlert2, this.lblAlert3,
                this.label21, this.label22, this.label23, this.label19
            };

            foreach (var control in alertControls)
            {
                if (control != null)
                {
                    control.Cursor = Cursors.Hand;
                    control.Click += (s, e) => ShowMaintenanceDetail();
                }
            }
        }

        private void InitializeContainerPanel()
        {
            mainContainerPanel = new Panel();
            mainContainerPanel.Dock = DockStyle.Fill;
            mainContainerPanel.Visible = false;
            this.Controls.Add(mainContainerPanel);
        }

        private void ShowChildView(Form childForm)
        {
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            childForm.FormClosed += (s, e) =>
            {
                mainContainerPanel.Visible = false;
                RefreshDashboardData();
                childForm.Dispose();
            };

            mainContainerPanel.Controls.Clear();
            mainContainerPanel.Controls.Add(childForm);
            mainContainerPanel.Visible = true;
            mainContainerPanel.BringToFront();
            childForm.Show();
        }

        private async void RefreshDashboardData()
        {
            try
            {
                // Fetch all data asynchronously in a background thread to prevent UI freezing
                var stats = await System.Threading.Tasks.Task.Run(() => DatabaseHelper.GetFleetStats());
                int upcomingCount = await System.Threading.Tasks.Task.Run(() => DatabaseHelper.GetUpcomingBookingsCount());
                var recentBookings = await System.Threading.Tasks.Task.Run(() => DatabaseHelper.GetRecentBookings(4));
                var alerts = await System.Threading.Tasks.Task.Run(() => DatabaseHelper.CheckAndGenerateBookingAlerts());

                // Update UI elements (runs on the main UI thread automatically)
                if (lblAvailCars != null) lblAvailCars.Text = stats["Available"].ToString();
                if (lblCarsOut != null) lblCarsOut.Text = stats["InMaintenance"].ToString();
                if (lblUpcoBook != null) lblUpcoBook.Text = upcomingCount.ToString();

                Label[] customerLabels = { lblCst1, lblCst2, lblCst3, lblCst4 };
                Label[] dateLabels = { recbookDte1, recbookDte2, recbookDte2, recbookDte4 }; // Note: recbookDte3 fallback handled, let's keep original label structure
                // Let's verify original labels: lblCst1-4, recbookDte1-4, recbookSts1-4
                Label[] originalDteLabels = { recbookDte1, recbookDte2, recbookDte3, recbookDte4 };
                Label[] originalStsLabels = { recbookSts1, recbookSts2, recbookSts3, recbookSts4 };

                for (int i = 0; i < 4; i++)
                {
                    if (i < recentBookings.Count)
                    {
                        var booking = recentBookings[i];
                        if (customerLabels[i] != null) customerLabels[i].Text = booking.CustomerName;
                        if (originalDteLabels[i] != null) originalDteLabels[i].Text = booking.BookingDate.ToString("MMM dd, yyyy");
                        if (originalStsLabels[i] != null)
                        {
                            originalStsLabels[i].Text = booking.Status;
                            
                            // Color code status label
                            string currentSts = booking.Status;
                            if (currentSts == "Available" || currentSts == "Confirmed")
                            {
                                originalStsLabels[i].ForeColor = System.Drawing.Color.FromArgb(135, 226, 98); // Green
                            }
                            else if (currentSts == "Rent in progress" || currentSts == "Rent in Progress" || currentSts == "Discarded")
                            {
                                originalStsLabels[i].ForeColor = System.Drawing.Color.FromArgb(255, 49, 49); // Red
                            }
                            else if (currentSts == "In Maintenance" || currentSts == "Draft")
                            {
                                originalStsLabels[i].ForeColor = System.Drawing.Color.FromArgb(255, 222, 89); // Yellow
                            }
                            else
                            {
                                originalStsLabels[i].ForeColor = System.Drawing.Color.White;
                            }
                        }
                    }
                    else
                    {
                        // Clear slots if not enough bookings exist
                        if (customerLabels[i] != null) customerLabels[i].Text = "No Bookings";
                        if (originalDteLabels[i] != null) originalDteLabels[i].Text = "N/A";
                        if (originalStsLabels[i] != null)
                        {
                            originalStsLabels[i].Text = "N/A";
                            originalStsLabels[i].ForeColor = System.Drawing.Color.White;
                        }
                    }
                }

                // 4. Check and generate warning alerts
                if (lblAlerts != null) lblAlerts.Text = alerts.Count.ToString();

                // Display up to 3 alerts on the dashboard with color coding
                Label[] alertLabels = { lblAlert1, lblAlert2, lblAlert3 };
                for (int i = 0; i < 3; i++)
                {
                    if (alertLabels[i] == null) continue;

                    if (i < alerts.Count)
                    {
                        alertLabels[i].Text = alerts[i];
                        alertLabels[i].AutoEllipsis = true;

                        // Color code based on alert content
                        if (alerts[i].StartsWith("Maintenance Due"))
                        {
                            alertLabels[i].ForeColor = System.Drawing.Color.FromArgb(255, 222, 89); // Yellow
                        }
                        else if (alerts[i].StartsWith("Warning:"))
                        {
                            alertLabels[i].ForeColor = System.Drawing.Color.FromArgb(255, 49, 49); // Red
                        }
                        else if (alerts[i].StartsWith("Outstanding Payment"))
                        {
                            alertLabels[i].ForeColor = System.Drawing.Color.FromArgb(0, 200, 255); // Cyan
                        }
                        else
                        {
                            alertLabels[i].ForeColor = System.Drawing.Color.White;
                        }
                    }
                    else
                    {
                        alertLabels[i].Text = i == 0 ? "No active alerts" : "";
                        alertLabels[i].ForeColor = System.Drawing.Color.White;
                    }
                }

                // 5. Check and send expiring booking emails in background
                DatabaseHelper.CheckAndSendExpiringEmails();
            }
            catch (Exception)
            {
                // Fallback to static mockup values on any error
                lblCst1.Text = "Offline Mode";
                recbookDte1.Text = DateTime.Today.ToString("MMM dd, yyyy");
                recbookSts1.Text = "Offline";
                recbookSts1.ForeColor = System.Drawing.Color.White;

                Label[] customerLabels = { lblCst2, lblCst3, lblCst4 };
                Label[] dateLabels = { recbookDte2, recbookDte3, recbookDte4 };
                Label[] statusLabels = { recbookSts2, recbookSts3, recbookSts4 };

                for (int i = 0; i < 3; i++)
                {
                    if (customerLabels[i] != null) customerLabels[i].Text = "No Bookings";
                    if (dateLabels[i] != null) dateLabels[i].Text = "N/A";
                    if (statusLabels[i] != null)
                    {
                        statusLabels[i].Text = "N/A";
                        statusLabels[i].ForeColor = System.Drawing.Color.White;
                    }
                }
                
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
            DatabaseHelper.Logout(this);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            // Do nothing, since we are already on the homepage dashboard
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

        private void btnBookAV_Click(object sender, EventArgs e)
        {
            ShowChildView(new bookAvehicle());
        }
        /*
         * //135, 226, 98 - Green
         * //255, 222, 89 - Yellow
         * //255, 49, 49 - Red
         */
        private void btnVehicles_Click(object sender, EventArgs e)
        {
            ShowChildView(new Vehicles());
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
            ShowChildView(new Reports());
        }

        private void ShowMaintenanceDetail()
        {
            using (var alertForm = new MaintenanceAlertsForm())
            {
                alertForm.ShowDialog(this);
            }
        }

        private void TdDashboard_Resize(object sender, EventArgs e)
        {
            if (this.pictureBox1 != null)
            {
                this.pictureBox1.Left = (this.ClientSize.Width - this.pictureBox1.Width) / 2;
            }
            if (this.pictureBox2 != null)
            {
                this.pictureBox2.Left = this.ClientSize.Width - this.pictureBox2.Width - 16;
            }

            if (this.pnlInfo1 != null)
            {
                this.pnlInfo1.Width = this.ClientSize.Width - 60;
                
                // Reposition the action buttons on the right edge of pnlInfo1
                int buttonsX = this.pnlInfo1.Width - 151;
                if (this.btnBookAV != null) this.btnBookAV.Location = new Point(buttonsX, this.btnBookAV.Location.Y);
                if (this.btnVehicles != null) this.btnVehicles.Location = new Point(buttonsX, this.btnVehicles.Location.Y);
                if (this.btnTracking != null) this.btnTracking.Location = new Point(buttonsX, this.btnTracking.Location.Y);
                if (this.btnReports != null) this.btnReports.Location = new Point(buttonsX, this.btnReports.Location.Y);

                // Resize Recent Bookings panel (guna2Panel2)
                if (this.guna2Panel2 != null && this.btnBookAV != null)
                {
                    this.guna2Panel2.Width = this.btnBookAV.Left - this.guna2Panel2.Left - 23;
                    
                    // Adjust horizontal line separators
                    int separatorWidth = this.guna2Panel2.Width - 24;
                    if (this.label8 != null) this.label8.Width = separatorWidth;
                    if (this.label9 != null) this.label9.Width = separatorWidth;
                    if (this.label10 != null) this.label10.Width = separatorWidth;
                    if (this.label11 != null) this.label11.Width = separatorWidth;
                    if (this.label12 != null) this.label12.Width = separatorWidth;

                    // Reposition status labels on the right edge of guna2Panel2
                    int statusX = this.guna2Panel2.Width - 146;
                    if (this.recbookSts1 != null) this.recbookSts1.Location = new Point(statusX, this.recbookSts1.Location.Y);
                    if (this.recbookSts2 != null) this.recbookSts2.Location = new Point(statusX, this.recbookSts2.Location.Y);
                    if (this.recbookSts3 != null) this.recbookSts3.Location = new Point(statusX, this.recbookSts3.Location.Y);
                    if (this.recbookSts4 != null) this.recbookSts4.Location = new Point(statusX, this.recbookSts4.Location.Y);

                    // Position date labels to the left of the status labels
                    if (this.recbookSts1 != null && this.recbookDte1 != null) this.recbookDte1.Location = new Point(this.recbookSts1.Left - 139, this.recbookDte1.Location.Y);
                    if (this.recbookSts2 != null && this.recbookDte2 != null) this.recbookDte2.Location = new Point(this.recbookSts2.Left - 139, this.recbookDte2.Location.Y);
                    if (this.recbookSts3 != null && this.recbookDte3 != null) this.recbookDte3.Location = new Point(this.recbookSts3.Left - 139, this.recbookDte3.Location.Y);
                    if (this.recbookSts4 != null && this.recbookDte4 != null) this.recbookDte4.Location = new Point(this.recbookSts4.Left - 139, this.recbookDte4.Location.Y);

                    // Resize customer labels so they don't overlap date labels
                    if (this.recbookDte1 != null)
                    {
                        int customerWidth = this.recbookDte1.Left - 11 - 10;
                        if (customerWidth > 50)
                        {
                            if (this.lblCst1 != null) this.lblCst1.Width = customerWidth;
                            if (this.lblCst2 != null) this.lblCst2.Width = customerWidth;
                            if (this.lblCst3 != null) this.lblCst3.Width = customerWidth;
                            if (this.lblCst4 != null) this.lblCst4.Width = customerWidth;
                        }
                    }
                }
            }

            if (this.guna2Panel3 != null)
            {
                if (this.pictureBox2 != null)
                {
                    this.guna2Panel3.Width = this.pictureBox2.Left - this.guna2Panel3.Left - 10;
                }
                else
                {
                    this.guna2Panel3.Width = this.ClientSize.Width - 60;
                }

                // Resize Alerts panel (guna2Panel4)
                if (this.guna2Panel4 != null)
                {
                    this.guna2Panel4.Width = this.guna2Panel3.Width - 43;

                    // Centered title
                    if (this.label21 != null) this.label21.Left = (this.guna2Panel4.Width - this.label21.Width) / 2;

                    // Line separators
                    int alertSepWidth = this.guna2Panel4.Width - 28;
                    if (this.label19 != null) this.label19.Width = alertSepWidth;
                    if (this.label22 != null) this.label22.Width = alertSepWidth;
                    if (this.label23 != null) this.label23.Width = alertSepWidth;

                    // Alert labels
                    int alertLblWidth = this.guna2Panel4.Width - 95;
                    if (this.lblAlert1 != null) this.lblAlert1.Width = alertLblWidth;
                    if (this.lblAlert2 != null) this.lblAlert2.Width = alertLblWidth;
                    if (this.lblAlert3 != null) this.lblAlert3.Width = alertLblWidth;
                }
            }
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
