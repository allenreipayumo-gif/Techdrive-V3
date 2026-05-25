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
            this.Load += (s, e) => {
                if (this.panel7 != null)
                {
                    this.panel7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                LoadVehiclesData();
            };
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.FromArgb(2, 36, 78);
            dgv.ForeColor = Color.White;
            dgv.GridColor = Color.FromArgb(29, 59, 172);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Header styling
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(29, 59, 172);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(135, 226, 98);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.EnableHeadersVisualStyles = false;
            
            // Row styling
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(2, 36, 78);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Century Gothic", 10F, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(29, 59, 172);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 35;
            
            // Auto size and layout
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
        }

        private void BtnAddVehicle_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddVehicleForm())
            {
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadVehiclesData(); // Refresh the list automatically upon successful database save!
                }
            }
        }

        private void LoadVehiclesData()
        {
            try
            {
                _loadedVehicles = DatabaseHelper.GetVehicles(100);
                
                // Clear static controls from panel7
                panel7.Controls.Clear();

                // Create a dynamic, premium action panel at the top
                Panel actionPanel = new Panel();
                actionPanel.Dock = DockStyle.Top;
                actionPanel.Height = 46;
                actionPanel.BackColor = Color.FromArgb(2, 36, 78);

                Button btnAddVehicle = new Button();
                btnAddVehicle.Text = "+ Add Vehicle";
                btnAddVehicle.ForeColor = Color.White;
                btnAddVehicle.BackColor = Color.FromArgb(29, 59, 172);
                btnAddVehicle.FlatStyle = FlatStyle.Flat;
                btnAddVehicle.FlatAppearance.BorderSize = 0;
                btnAddVehicle.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
                btnAddVehicle.Size = new Size(140, 32);
                btnAddVehicle.Location = new Point(10, 6);
                btnAddVehicle.Cursor = Cursors.Hand;
                btnAddVehicle.Click += BtnAddVehicle_Click;
                actionPanel.Controls.Add(btnAddVehicle);

                panel7.Controls.Add(actionPanel);
                
                // Create and style dynamic DataGridView
                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                StyleDataGridView(dgv);
                
                dgv.CellFormatting += Dgv_CellFormatting;
                dgv.CellContentClick += Dgv_CellContentClick;
                
                panel7.Controls.Add(dgv);
                dgv.BringToFront(); // Necessary to layout correctly below the top docked actionPanel!
                
                // Populate DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Vehicle Model", typeof(string));
                dt.Columns.Add("Plate Number", typeof(string));
                dt.Columns.Add("Remarks", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                
                foreach (var v in _loadedVehicles)
                {
                    dt.Rows.Add(v.VehicleId, $"{v.Make} {v.Model}", v.PlateNumber, v.Remarks, v.Status);
                }
                
                dgv.DataSource = dt;
                
                // Hide database primary key ID
                if (dgv.Columns.Contains("ID")) dgv.Columns["ID"].Visible = false;
                
                // Add Dynamic Button Column for Maintenance toggles
                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                btnCol.Name = "Action";
                btnCol.HeaderText = "Maintenance Action";
                btnCol.UseColumnTextForButtonValue = false;
                btnCol.FlatStyle = FlatStyle.Flat;
                btnCol.DefaultCellStyle.BackColor = Color.FromArgb(29, 59, 172);
                btnCol.DefaultCellStyle.ForeColor = Color.White;
                btnCol.DefaultCellStyle.SelectionBackColor = Color.FromArgb(29, 59, 172);
                btnCol.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.Columns.Add(btnCol);
                
                // Adjust Maintenance column width specifically to fit "Send to maintenance" beautifully
                dgv.Columns["Action"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["Action"].Width = 185;

                // Add Dynamic Button Column for Ending Bookings
                DataGridViewButtonColumn btnEndCol = new DataGridViewButtonColumn();
                btnEndCol.Name = "EndBooking";
                btnEndCol.HeaderText = "End Rental";
                btnEndCol.UseColumnTextForButtonValue = false;
                btnEndCol.FlatStyle = FlatStyle.Flat;
                btnEndCol.DefaultCellStyle.BackColor = Color.FromArgb(0, 168, 204); // Premium Teal-blue
                btnEndCol.DefaultCellStyle.ForeColor = Color.White;
                btnEndCol.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 168, 204);
                btnEndCol.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.Columns.Add(btnEndCol);

                // Adjust End Booking column width
                dgv.Columns["EndBooking"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["EndBooking"].Width = 135;

                // Add Dynamic Button Column for Deleting vehicles
                DataGridViewButtonColumn btnDeleteCol = new DataGridViewButtonColumn();
                btnDeleteCol.Name = "Delete";
                btnDeleteCol.HeaderText = "Delete";
                btnDeleteCol.Text = "Delete";
                btnDeleteCol.UseColumnTextForButtonValue = false;
                btnDeleteCol.FlatStyle = FlatStyle.Flat;
                btnDeleteCol.DefaultCellStyle.BackColor = Color.FromArgb(255, 49, 49);
                btnDeleteCol.DefaultCellStyle.ForeColor = Color.White;
                btnDeleteCol.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 49, 49);
                btnDeleteCol.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.Columns.Add(btnDeleteCol);

                dgv.Columns["Delete"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["Delete"].Width = 90;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicle data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView dgv && e.RowIndex >= 0)
            {
                // Color code Status column horizontal row cells
                if (dgv.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
                {
                    string status = e.Value.ToString();
                    if (status == "Available")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(135, 226, 98); // Green
                    }
                    else if (status == "In Maintenance")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(255, 222, 89); // Yellow
                    }
                    else // Rent in progress or Out for Rental
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(255, 49, 49); // Red
                    }
                    e.CellStyle.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
                }
                
                // Set text for Toggle action buttons based on active vehicle statuses
                if (dgv.Columns[e.ColumnIndex].Name == "Action")
                {
                    var statusValue = dgv.Rows[e.RowIndex].Cells["Status"].Value;
                    if (statusValue != null)
                    {
                        string status = statusValue.ToString();
                        if (status == "Available")
                        {
                            e.Value = "Send to maintenance";
                        }
                        else if (status == "In Maintenance")
                        {
                            e.Value = "Make Available";
                        }
                        else
                        {
                            e.Value = "Rented";
                        }
                    }
                }

                // Format the dynamic "End Booking" cell values based on rental state
                if (dgv.Columns[e.ColumnIndex].Name == "EndBooking")
                {
                    var statusValue = dgv.Rows[e.RowIndex].Cells["Status"].Value;
                    if (statusValue != null)
                    {
                        string status = statusValue.ToString();
                        if (status == "Rent in progress" || status == "Rent in Progress" || status == "Out for Rental")
                        {
                            e.Value = "End Booking";
                        }
                        else
                        {
                            e.Value = "—";
                            e.CellStyle.ForeColor = Color.DarkGray;
                        }
                    }
                }

                if (dgv.Columns[e.ColumnIndex].Name == "Delete")
                {
                    e.Value = "Delete";
                }
            }
        }

        private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dgv && e.RowIndex >= 0)
            {
                if (dgv.Columns[e.ColumnIndex].Name == "Action")
                {
                    var idValue = dgv.Rows[e.RowIndex].Cells["ID"].Value;
                    var statusValue = dgv.Rows[e.RowIndex].Cells["Status"].Value;
                    if (idValue != null && statusValue != null)
                    {
                        int vehicleId = Convert.ToInt32(idValue);
                        string status = statusValue.ToString();
                        
                        if (status == "Rent in progress" || status == "Rent in Progress" || status == "Out for Rental")
                        {
                            MessageBox.Show("Cannot toggle maintenance status while vehicle is rented.", "Active Rental", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        bool success = DatabaseHelper.ToggleVehicleStatus(vehicleId, status);
                        if (success)
                        {
                            LoadVehiclesData(); // Refresh list on UI
                        }
                    }
                }
                else if (dgv.Columns[e.ColumnIndex].Name == "EndBooking")
                {
                    var idValue = dgv.Rows[e.RowIndex].Cells["ID"].Value;
                    var statusValue = dgv.Rows[e.RowIndex].Cells["Status"].Value;
                    var modelValue = dgv.Rows[e.RowIndex].Cells["Vehicle Model"].Value;
                    
                    if (idValue != null && statusValue != null)
                    {
                        string status = statusValue.ToString();
                        if (status == "Rent in progress" || status == "Rent in Progress" || status == "Out for Rental")
                        {
                            int vehicleId = Convert.ToInt32(idValue);
                            var confirm = MessageBox.Show($"Are you sure that {modelValue} has been returned early, and you want to terminate its active rental booking?", 
                                "Confirm End Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            
                            if (confirm == DialogResult.Yes)
                            {
                                bool success = DatabaseHelper.EndBooking(vehicleId);
                                if (success)
                                {
                                    MessageBox.Show("Rental booking successfully terminated! The vehicle is now marked Available in your fleet.", 
                                        "Booking Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadVehiclesData(); // Refresh list on UI
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("This vehicle is not currently active on an ongoing rental.", "No Active Rental", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else if (dgv.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var idValue = dgv.Rows[e.RowIndex].Cells["ID"].Value;
                    var modelValue = dgv.Rows[e.RowIndex].Cells["Vehicle Model"].Value;
                    if (idValue != null)
                    {
                        int vehicleId = Convert.ToInt32(idValue);
                        var confirm = MessageBox.Show($"Are you sure you want to delete vehicle {modelValue}? This will cascadingly delete all past booking history and alerts for this vehicle.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm == DialogResult.Yes)
                        {
                            bool success = DatabaseHelper.DeleteVehicle(vehicleId);
                            if (success)
                            {
                                LoadVehiclesData(); // Refresh list on UI
                            }
                        }
                    }
                }
            }
        }
    }
}
