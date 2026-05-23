using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public partial class Vehicles : TechdriveLogin.tdLogin.Template
    {
        private List<VehicleInfo> _loadedVehicles = new List<VehicleInfo>();

        public Vehicles()
        {
            InitializeComponent();
            this.Load += (s, e) => LoadVehiclesData();
        }

        private void LoadVehiclesData()
        {
            try
            {
                _loadedVehicles = DatabaseHelper.GetVehicles(7);
                
                // Map of UI controls from designer
                Label[] vmLabels = { lblVm1, lblVm2, lblVm3, lblVm4, lblVm5, lblVm6, lblVm7 };
                Label[] pnLabels = { lblPn1, lblPn2, lblPn3, lblPn4, lblPn5, lblPn6, lblPn7 };
                Label[] remarksLabels = { lblRemarks1, lblRemarks2, lblRemarks3, lblRemarks4, lblRemarks5, lblRemarks6, lblRemarks7 };
                Label[] statusLabels = { lblStatus1, lblStatus2, lblStatus3, lblStatus4, lblStatus5, lblStatus6, lblStatus7 };
                Button[] statusButtons = { btnStm1, btnStm2, btnStm3, btnStm4, btnStm5, btnStm6, btnStm7 };

                for (int i = 0; i < 7; i++)
                {
                    if (i < _loadedVehicles.Count)
                    {
                        var vehicle = _loadedVehicles[i];
                        if (vmLabels[i] != null)
                        {
                            vmLabels[i].Text = $"{vehicle.Make} {vehicle.Model}";
                            vmLabels[i].Font = new Font("Century Gothic", 12F, FontStyle.Bold); // Reduce size so names fit
                        }
                        if (pnLabels[i] != null)
                        {
                            pnLabels[i].Text = vehicle.PlateNumber;
                        }
                        if (remarksLabels[i] != null)
                        {
                            remarksLabels[i].Text = vehicle.Remarks;
                        }
                        if (statusLabels[i] != null)
                        {
                            statusLabels[i].Text = vehicle.Status;
                            // Align all status labels horizontally at X = 805 (fixing row 3 alignment)
                            statusLabels[i].Location = new Point(805, statusLabels[i].Location.Y);
                        }
                        
                        // Status color formatting and button text/font adjustments
                        if (statusLabels[i] != null)
                        {
                            if (vehicle.Status == "Available")
                            {
                                statusLabels[i].ForeColor = Color.FromArgb(135, 226, 98); // Green
                                if (statusButtons[i] != null)
                                {
                                    statusButtons[i].Text = "Maint"; // Fits in button bounds
                                    statusButtons[i].Enabled = true;
                                    statusButtons[i].Font = new Font("Century Gothic", 8F, FontStyle.Bold);
                                }
                            }
                            else if (vehicle.Status == "In Maintenance")
                            {
                                statusLabels[i].ForeColor = Color.FromArgb(255, 222, 89); // Yellow
                                if (statusButtons[i] != null)
                                {
                                    statusButtons[i].Text = "Avail"; // Fits in button bounds
                                    statusButtons[i].Enabled = true;
                                    statusButtons[i].Font = new Font("Century Gothic", 8F, FontStyle.Bold);
                                }
                            }
                            else // Rent in progress
                            {
                                statusLabels[i].ForeColor = Color.FromArgb(255, 49, 49); // Red
                                if (statusButtons[i] != null)
                                {
                                    statusButtons[i].Text = "Rented"; // Fits in button bounds
                                    statusButtons[i].Enabled = false; // Disable toggle during active rental
                                    statusButtons[i].Font = new Font("Century Gothic", 7.5F, FontStyle.Bold);
                                }
                            }
                        }

                        if (statusButtons[i] != null)
                        {
                            statusButtons[i].Tag = i; // Save index in tag
                            statusButtons[i].Click -= StatusButton_Click; // Prevent duplicate handlers
                            statusButtons[i].Click += StatusButton_Click;
                        }
                        
                        // Make elements visible
                        if (vmLabels[i] != null) vmLabels[i].Visible = true;
                        if (pnLabels[i] != null) pnLabels[i].Visible = true;
                        if (remarksLabels[i] != null) remarksLabels[i].Visible = true;
                        if (statusLabels[i] != null) statusLabels[i].Visible = true;
                        if (statusButtons[i] != null) statusButtons[i].Visible = true;
                    }
                    else
                    {
                        // Hide extra slots if there are fewer than 7 vehicles in the database
                        if (vmLabels[i] != null) vmLabels[i].Visible = false;
                        if (pnLabels[i] != null) pnLabels[i].Visible = false;
                        if (remarksLabels[i] != null) remarksLabels[i].Visible = false;
                        if (statusLabels[i] != null) statusLabels[i].Visible = false;
                        if (statusButtons[i] != null) statusButtons[i].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicle data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                if (index >= 0 && index < _loadedVehicles.Count)
                {
                    var vehicle = _loadedVehicles[index];
                    bool success = DatabaseHelper.ToggleVehicleStatus(vehicle.VehicleId, vehicle.Status);
                    if (success)
                    {
                        LoadVehiclesData(); // Refresh list on UI
                    }
                }
            }
        }
    }
}
