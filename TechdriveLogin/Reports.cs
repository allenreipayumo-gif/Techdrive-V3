using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public partial class Reports : TechdriveLogin.tdLogin.Template
    {
        public Reports()
        {
            InitializeComponent();
            this.Load += (s, e) => {
                if (this.panel7 != null)
                {
                    this.panel7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                LoadReportsData();
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

        private void LoadReportsData()
        {
            try
            {
                // Clear static controls from panel7
                panel7.Controls.Clear();
                
                // Create and style dynamic DataGridView
                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                StyleDataGridView(dgv);
                dgv.CellFormatting += Dgv_CellFormatting;
                panel7.Controls.Add(dgv);
                
                // Fetch reports up to a limit of 100 for high scalability
                var reports = DatabaseHelper.GetVehicleReports(100);
                
                // Create a DataTable to bind
                DataTable dt = new DataTable();
                dt.Columns.Add("Vehicle Model", typeof(string));
                dt.Columns.Add("Plate Number", typeof(string));
                dt.Columns.Add("Last Maintenance", typeof(string));
                dt.Columns.Add("Total Bookings", typeof(int));
                dt.Columns.Add("Net Earnings", typeof(string));
                
                foreach (var rep in reports)
                {
                    dt.Rows.Add(rep.VehicleModel, rep.PlateNumber, rep.LastMaintenance, rep.TotalTrips, $"₱{rep.NetEarnings:N2}");
                }
                
                // Fetch dynamic monthly stats for bookings and net earnings
                var monthlyStats = DatabaseHelper.GetMonthlyReportStats();
                
                // Append highlighted monthly summary rows
                dt.Rows.Add("Total bookings this month", "", "", monthlyStats.totalBookings, "");
                dt.Rows.Add("Net earnings for this month", "", "", DBNull.Value, $"₱{monthlyStats.netEarnings:N2}");
                
                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView dgv && e.RowIndex >= 0)
            {
                var modelVal = dgv.Rows[e.RowIndex].Cells["Vehicle Model"].Value;
                if (modelVal != null)
                {
                    string modelText = modelVal.ToString();
                    if (modelText == "Total bookings this month" || modelText == "Net earnings for this month")
                    {
                        // Accent highlight the summary rows in bold green with specialized corporate blue background
                        e.CellStyle.BackColor = Color.FromArgb(29, 59, 172);
                        e.CellStyle.ForeColor = Color.FromArgb(135, 226, 98);
                        e.CellStyle.Font = new Font("Century Gothic", 10.5F, FontStyle.Bold);
                    }
                }
            }
        }
    }
}
