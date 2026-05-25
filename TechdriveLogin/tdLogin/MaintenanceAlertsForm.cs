using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public class MaintenanceAlertsForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Button closeButton;
        private Panel contentPanel;
        private DataGridView dgvMaintenance;

        public MaintenanceAlertsForm()
        {
            this.Size = new Size(780, 480);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(2, 36, 78); // Deep Navy matching Dashboard

            // Form border painting for premium look
            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(29, 59, 172), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            InitializeControls();
            LoadMaintenanceData();
        }

        private void InitializeControls()
        {
            // Header Panel
            headerPanel = new Panel();
            headerPanel.Height = 55;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.BackColor = Color.FromArgb(29, 59, 172); // Techdrive Blue

            titleLabel = new Label();
            titleLabel.Text = "Vehicle Maintenance Schedules";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Century Gothic", 13.5F, FontStyle.Bold);
            titleLabel.Location = new Point(20, 16);
            titleLabel.AutoSize = true;
            headerPanel.Controls.Add(titleLabel);

            closeButton = new Button();
            closeButton.Text = "✕";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = Color.Transparent;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 49, 49); // Turn red on hover
            closeButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            closeButton.Size = new Size(45, 45);
            closeButton.Location = new Point(this.Width - 50, 5);
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(closeButton);

            this.Controls.Add(headerPanel);

            // Content Panel
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(15, 70, 15, 15); // Give room under header panel
            this.Controls.Add(contentPanel);

            // DataGridView
            dgvMaintenance = new DataGridView();
            dgvMaintenance.Dock = DockStyle.Fill;
            StyleDgv(dgvMaintenance);
            contentPanel.Controls.Add(dgvMaintenance);
        }

        private void StyleDgv(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.FromArgb(2, 36, 78);
            dgv.ForeColor = Color.White;
            dgv.GridColor = Color.FromArgb(29, 59, 172);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Header styling
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(29, 59, 172);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(135, 226, 98); // Techdrive Green
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10.5F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 36;
            dgv.EnableHeadersVisualStyles = false;
            
            // Row styling
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(2, 36, 78);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Century Gothic", 9.5F, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(29, 59, 172);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 34;
            
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;

            dgv.CellFormatting += Dgv_CellFormatting;
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMaintenance.Columns[e.ColumnIndex].Name == "Maintenance Status" && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val.Contains("OVERDUE"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 49, 49); // Vibrant Red
                    e.CellStyle.Font = new Font("Century Gothic", 9.5F, FontStyle.Bold);
                }
                else if (val.Contains("Due TODAY") || (val.Contains("days left") && !val.Contains("OVERDUE") && int.TryParse(val.Split(' ')[0], out int days) && days <= 7))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 222, 89); // Vibrant Yellow
                    e.CellStyle.Font = new Font("Century Gothic", 9.5F, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(135, 226, 98); // Vibrant Green
                    e.CellStyle.Font = new Font("Century Gothic", 9.5F, FontStyle.Bold);
                }
            }
        }

        private void LoadMaintenanceData()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Vehicle Model", typeof(string));
                dt.Columns.Add("Plate Number", typeof(string));
                dt.Columns.Add("Last Maintenance", typeof(string));
                dt.Columns.Add("Next Maintenance Due", typeof(string));
                dt.Columns.Add("Maintenance Status", typeof(string));

                var vehicles = DatabaseHelper.GetVehiclesMaintenanceProximity();
                foreach (var v in vehicles)
                {
                    dt.Rows.Add(v.ModelName, v.PlateNumber, v.LastMaintenanceStr, v.NextMaintenanceStr, v.DaysLeftDetail);
                }

                dgvMaintenance.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading maintenance information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
