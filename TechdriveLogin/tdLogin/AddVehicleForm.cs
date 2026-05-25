using System;
using System.Drawing;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public class AddVehicleForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Button closeButton;

        private Label lblMake;
        private TextBox txtMake;
        private Label lblModel;
        private TextBox txtModel;
        private Label lblRate;
        private TextBox txtRate;
        private Label lblPlate;
        private TextBox txtPlate;
        private Label lblStatus;
        private ComboBox cbStatus;

        private Button btnSave;
        private Button btnCancel;

        public AddVehicleForm()
        {
            this.Size = new Size(460, 440);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(2, 36, 78); // Deep Navy

            // Custom border painting
            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(29, 59, 172), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            InitializeControls();
        }

        private void InitializeControls()
        {
            // Header Panel
            headerPanel = new Panel();
            headerPanel.Height = 55;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.BackColor = Color.FromArgb(29, 59, 172); // Techdrive Blue

            titleLabel = new Label();
            titleLabel.Text = "Add New Fleet Vehicle";
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

            int startX = 40;
            int labelWidth = 100;
            int inputWidth = 260;
            int spacing = 45;

            // Make Input
            lblMake = CreateLabel("Make:", startX, 90, labelWidth);
            txtMake = CreateTextBox(startX + labelWidth, 87, inputWidth);
            this.Controls.Add(lblMake);
            this.Controls.Add(txtMake);

            // Model Input
            lblModel = CreateLabel("Model:", startX, 90 + spacing, labelWidth);
            txtModel = CreateTextBox(startX + labelWidth, 87 + spacing, inputWidth);
            this.Controls.Add(lblModel);
            this.Controls.Add(txtModel);

            // Daily Rate Input
            lblRate = CreateLabel("Daily Rate:", startX, 90 + (spacing * 2), labelWidth);
            txtRate = CreateTextBox(startX + labelWidth, 87 + (spacing * 2), inputWidth);
            this.Controls.Add(lblRate);
            this.Controls.Add(txtRate);

            // Plate Number Input
            lblPlate = CreateLabel("Plate No:", startX, 90 + (spacing * 3), labelWidth);
            txtPlate = CreateTextBox(startX + labelWidth, 87 + (spacing * 3), inputWidth);
            this.Controls.Add(lblPlate);
            this.Controls.Add(txtPlate);

            // Status ComboBox Input
            lblStatus = CreateLabel("Status:", startX, 90 + (spacing * 4), labelWidth);
            cbStatus = new ComboBox();
            cbStatus.Font = new Font("Century Gothic", 10F);
            cbStatus.Location = new Point(startX + labelWidth, 87 + (spacing * 4));
            cbStatus.Size = new Size(inputWidth, 25);
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.Items.AddRange(new string[] { "Available", "In Maintenance" });
            cbStatus.SelectedIndex = 0;
            cbStatus.BackColor = Color.FromArgb(2, 36, 78);
            cbStatus.ForeColor = Color.White;
            this.Controls.Add(lblStatus);
            this.Controls.Add(cbStatus);

            // Save Button
            btnSave = new Button();
            btnSave.Text = "Save Vehicle";
            btnSave.Font = new Font("Century Gothic", 11F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(135, 226, 98); // Techdrive Green
            btnSave.ForeColor = Color.Black;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Size = new Size(160, 42);
            btnSave.Location = new Point(90, 340);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Font = new Font("Century Gothic", 11F, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(29, 59, 172); // Techdrive Blue
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Size = new Size(110, 42);
            btnCancel.Location = new Point(265, 340);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private Label CreateLabel(string text, int x, int y, int width)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Century Gothic", 10.5F, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(width, 25);
            return lbl;
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            TextBox txt = new TextBox();
            txt.Font = new Font("Century Gothic", 10.5F);
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 25);
            txt.BackColor = Color.FromArgb(2, 36, 78);
            txt.ForeColor = Color.White;
            txt.BorderStyle = BorderStyle.FixedSingle;
            return txt;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string make = txtMake.Text.Trim();
            string model = txtModel.Text.Trim();
            string plate = txtPlate.Text.Trim();
            string rateStr = txtRate.Text.Trim();
            string status = cbStatus.SelectedItem.ToString();

            if (string.IsNullOrEmpty(make) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(plate) || string.IsNullOrEmpty(rateStr))
            {
                MessageBox.Show("Please fill in all inputs before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(rateStr, out decimal dailyRate) || dailyRate <= 0)
            {
                MessageBox.Show("Daily rate must be a valid positive decimal value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRate.Focus();
                return;
            }

            // Write to database!
            bool success = DatabaseHelper.AddVehicle(make, model, dailyRate, plate, status);
            if (success)
            {
                MessageBox.Show($"Successfully added new vehicle: '{make} {model}' to fleet database!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
