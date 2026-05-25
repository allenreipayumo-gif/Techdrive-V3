using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TechdriveLogin
{
    public partial class bookAvehicle : TechdriveLogin.tdLogin.Template
    {
        private Dictionary<Button, int> _buttonToVehicleIdMap = new Dictionary<Button, int>();
        private Label lblFrom;
        private Label lblTo;
        private DateTimePicker datePickerTo;
        private int _nextBookingId = 16657;
        private TextBox emailTb;
        private Label lblEmail;

        public bookAvehicle()
        {
            InitializeComponent();
            ConfigureDateRangeLayout();
            this.Load += (s, e) => {
                if (this.panel7 != null)
                {
                    this.panel7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                if (this.panel8 != null)
                {
                    this.panel8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                }
                if (this.panel9 != null)
                {
                    this.panel9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                if (this.panelAvailableVehicles != null)
                {
                    this.panelAvailableVehicles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    this.panelAvailableVehicles.AutoScroll = true;
                }
                if (this.panel10 != null)
                {
                    this.panel10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                if (this.label4 != null)
                {
                    this.label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.priceLbl != null)
                {
                    this.priceLbl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                }
                if (this.bookBtn != null)
                {
                    this.bookBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.draftBtn != null)
                {
                    this.draftBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.discardBtn != null)
                {
                    this.discardBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.rentalId != null)
                {
                    this.rentalId.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                }
                if (this.datePicker != null)
                {
                    this.datePicker.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.lblFrom != null)
                {
                    this.lblFrom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.lblTo != null)
                {
                    this.lblTo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                if (this.datePickerTo != null)
                {
                    this.datePickerTo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
                LoadVehiclesFromDatabase();
            };
        }

        private void RefreshRentalId()
        {
            _nextBookingId = DatabaseHelper.GetNextBookingId();
            if (this.rentalId != null)
            {
                this.rentalId.Text = $"Rental ID: #{_nextBookingId}";
            }
        }

        private void ConfigureDateRangeLayout()
        {
            // Move rentalId to top-left of customer details section, just under the title
            if (this.rentalId != null)
            {
                this.rentalId.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
                this.rentalId.ForeColor = Color.FromArgb(255, 222, 89); // Yellow accent
                this.rentalId.Location = new Point(20, 18);
                this.rentalId.AutoSize = true;
            }

            // Shift "Customer Details" title to the right to make room
            if (this.label3 != null)
            {
                this.label3.AutoSize = true;
                this.label3.Location = new Point(215, 18);
            }

            // Set font and size for all labels to be smaller and styled above textboxes
            Font labelFont = new Font("Century Gothic", 9.75F, FontStyle.Bold);
            Font tbFont = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular);
            Size tbSize = new Size(270, 22);

            if (this.label5 != null) { this.label5.Font = labelFont; this.label5.Location = new Point(60, 42); }
            if (this.nameTb != null) { this.nameTb.Font = tbFont; this.nameTb.Size = tbSize; this.nameTb.Location = new Point(60, 60); }

            if (this.label6 != null) { this.label6.Font = labelFont; this.label6.Location = new Point(60, 92); }
            if (this.addressTb != null) { this.addressTb.Font = tbFont; this.addressTb.Size = tbSize; this.addressTb.Location = new Point(60, 110); }

            if (this.label8 != null) { this.label8.Font = labelFont; this.label8.Location = new Point(60, 142); }
            if (this.cntctnumTb != null) { this.cntctnumTb.Font = tbFont; this.cntctnumTb.Size = tbSize; this.cntctnumTb.Location = new Point(60, 160); }

            if (this.label9 != null) { this.label9.Font = labelFont; this.label9.Location = new Point(60, 192); }
            if (this.lcnsenumTb != null) { this.lcnsenumTb.Font = tbFont; this.lcnsenumTb.Size = tbSize; this.lcnsenumTb.Location = new Point(60, 210); }

            // Dynamically instantiate and position the Email Address textbox and label
            emailTb = new TextBox();
            emailTb.Font = tbFont;
            emailTb.Size = tbSize;
            emailTb.Location = new Point(60, 260);

            lblEmail = new Label();
            lblEmail.Text = "Email Address";
            lblEmail.Font = labelFont;
            lblEmail.ForeColor = Color.White;
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(60, 242);

            if (this.panel10 != null)
            {
                this.panel10.Controls.Add(emailTb);
                this.panel10.Controls.Add(lblEmail);
            }

            if (this.paymentsTb != null) { this.paymentsTb.Font = tbFont; this.paymentsTb.Size = tbSize; this.paymentsTb.Location = new Point(60, 310); }
            if (this.lblPayments != null) { this.lblPayments.Font = labelFont; this.lblPayments.Location = new Point(60, 292); }

            if (this.datePicker != null)
            {
                this.datePicker.Location = new Point(120, 362);
                this.datePicker.Size = new Size(190, 23);
                this.datePicker.ValueChanged += (s, e) => UpdatePriceDisplay();
            }

            // Create and position dynamic "From" Label
            lblFrom = new Label();
            lblFrom.Text = "From:";
            lblFrom.ForeColor = Color.White;
            lblFrom.Font = new Font("Century Gothic", 9.75F, FontStyle.Bold);
            lblFrom.Location = new Point(60, 365);
            lblFrom.AutoSize = true;
            if (this.panel10 != null) this.panel10.Controls.Add(lblFrom);

            // Create and position dynamic "To" Label
            lblTo = new Label();
            lblTo.Text = "To:";
            lblTo.ForeColor = Color.White;
            lblTo.Font = new Font("Century Gothic", 9.75F, FontStyle.Bold);
            lblTo.Location = new Point(60, 395);
            lblTo.AutoSize = true;
            if (this.panel10 != null) this.panel10.Controls.Add(lblTo);

            // Create and position dynamic "To" DateTimePicker
            datePickerTo = new DateTimePicker();
            datePickerTo.Font = new Font("Century Gothic", 9.75F, FontStyle.Regular);
            datePickerTo.Location = new Point(120, 392);
            datePickerTo.Size = new Size(190, 23);
            datePickerTo.Value = DateTime.Today.AddDays(1);
            datePickerTo.ValueChanged += (s, e) => UpdatePriceDisplay();
            if (this.panel10 != null) this.panel10.Controls.Add(datePickerTo);

            // Shift booking buttons down slightly to Y=425 to prevent overlapping
            if (this.bookBtn != null) this.bookBtn.Location = new Point(this.bookBtn.Location.X, 425);
            if (this.draftBtn != null) this.draftBtn.Location = new Point(this.draftBtn.Location.X, 425);
            if (this.discardBtn != null) this.discardBtn.Location = new Point(this.discardBtn.Location.X, 425);
        }

        private void UpdatePriceDisplay()
        {
            Button selectedBtn = null;
            foreach (Control c in panelAvailableVehicles.Controls)
            {
                if (c is Button btn && btn.BackColor == Color.FromArgb(135, 226, 98))
                {
                    selectedBtn = btn;
                    break;
                }
            }

            if (selectedBtn != null && selectedBtn.Tag != null)
            {
                decimal.TryParse(selectedBtn.Tag.ToString(), out decimal rate);
                int days = (datePickerTo.Value.Date - datePicker.Value.Date).Days;
                if (days <= 0) days = 1;
                priceLbl.Text = $"Subtotal: ₱{(rate * days):N2} ({days} day{(days > 1 ? "s" : "")})";
            }
        }

        private void LoadVehiclesFromDatabase()
        {
            // Also refresh the rental ID preview from DB
            RefreshRentalId();

            try
            {
                panelAvailableVehicles.Controls.Clear();
                _buttonToVehicleIdMap.Clear();

                // Fetch vehicles from the database (exactly as the Vehicles section does)
                var vehicles = DatabaseHelper.GetVehicles(100);

                foreach (var v in vehicles)
                {
                    // Filter out non-available / under-maintenance vehicles
                    if (v.Status != "Available")
                    {
                        continue;
                    }

                    // Dynamically create a premium styled selection button
                    Button btn = new Button();
                    btn.BackColor = Color.FromArgb(2, 36, 78);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(29, 59, 172);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
                    btn.ForeColor = Color.White;
                    btn.Size = new Size(170, 65);
                    btn.Text = $"{v.Make} {v.Model}\n{v.PlateNumber}";
                    btn.Tag = v.DailyRate;
                    btn.UseVisualStyleBackColor = false;

                    // Wire event handler
                    btn.Click += VehicleButton_Click;

                    // Map button to vehicle ID
                    _buttonToVehicleIdMap[btn] = v.VehicleId;

                    // Add to FlowLayoutPanel
                    panelAvailableVehicles.Controls.Add(btn);
                }
            }
            catch (Exception)
            {
                // Fallback offline mock mapping
                panelAvailableVehicles.Controls.Clear();
                _buttonToVehicleIdMap.Clear();

                var mockVehicles = new[]
                {
                    new { Make = "Toyota", Model = "Vios", Rate = 1500.00m, Id = 1, Plate = "NDG-4812" },
                    new { Make = "Ford", Model = "Everest", Rate = 3500.00m, Id = 2, Plate = "NFC-2930" },
                    new { Make = "Mitsubishi", Model = "Mirage", Rate = 1200.00m, Id = 3, Plate = "AAA-8765" },
                    new { Make = "Toyota", Model = "Fortuner", Rate = 3200.00m, Id = 4, Plate = "NDG-9102" },
                    new { Make = "Toyota", Model = "Veloz", Rate = 2200.00m, Id = 5, Plate = "NFI-4821" },
                    new { Make = "Toyota", Model = "Hiace", Rate = 4000.00m, Id = 6, Plate = "NDG-1667" },
                    new { Make = "Toyota", Model = "Rush", Rate = 2000.00m, Id = 7, Plate = "NFI-3098" },
                    new { Make = "Ford", Model = "Ranger", Rate = 3000.00m, Id = 8, Plate = "NFC-8371" },
                    new { Make = "Mitsubishi", Model = "Xpander", Rate = 2400.00m, Id = 9, Plate = "AAA-4321" },
                    new { Make = "Toyota", Model = "Hilux", Rate = 2800.00m, Id = 10, Plate = "NDG-7741" },
                    new { Make = "Honda", Model = "BR-V", Rate = 2200.00m, Id = 11, Plate = "NFK-5928" },
                    new { Make = "Toyota", Model = "Vios Copy", Rate = 1500.00m, Id = 12, Plate = "NDG-4812" }
                };

                foreach (var mock in mockVehicles)
                {
                    Button btn = new Button();
                    btn.BackColor = Color.FromArgb(2, 36, 78);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(29, 59, 172);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
                    btn.ForeColor = Color.White;
                    btn.Size = new Size(170, 65);
                    btn.Text = $"{mock.Make} {mock.Model}\n{mock.Plate}";
                    btn.Tag = mock.Rate;
                    btn.UseVisualStyleBackColor = false;
                    btn.Click += VehicleButton_Click;

                    _buttonToVehicleIdMap[btn] = mock.Id;
                    panelAvailableVehicles.Controls.Add(btn);
                }
            }
        }

        private int GetSelectedVehicleId(Button btn)
        {
            if (btn != null && _buttonToVehicleIdMap.TryGetValue(btn, out int vehicleId))
            {
                return vehicleId;
            }
            return 1;
        }

        private void priceLbl_Click(object sender, EventArgs e)
        {
        }

        private void VehicleButton_Click(object sender, EventArgs e)
        {
            // 1. Loop through all buttons in the FlowLayoutPanel
            // This resets EVERY button back to the original dark blue
            foreach (Control c in panelAvailableVehicles.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(2, 36, 78);
                    btn.ForeColor = Color.White;
                }
            }

            // 2. Now, highlight the specific button that was just clicked
            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.FromArgb(135, 226, 98); // Your prototype green
            clickedButton.ForeColor = Color.Black;

            // 3. Update the subtotal dynamically
            UpdatePriceDisplay();
        }

        private void bookBtn_Click(object sender, EventArgs e)
        {
            ProcessBooking("Confirmed");
        }

        private void draftBtn_Click(object sender, EventArgs e)
        {
            ProcessBooking("Draft");
        }

        private void discardBtn_Click(object sender, EventArgs e)
        {
            ProcessBooking("Discarded");
        }

        private void ProcessBooking(string status)
        {
            // 1. Gather all information from input fields
            string name = nameTb.Text.Trim();
            string address = addressTb.Text.Trim();
            string contact = cntctnumTb.Text.Trim();
            string license = lcnsenumTb.Text.Trim();
            string payments = paymentsTb.Text.Trim();
            string email = emailTb != null ? emailTb.Text.Trim() : "";
            string rentalID = $"#{_nextBookingId}";

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter Customer Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTb.Focus();
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter Customer Email Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (emailTb != null) emailTb.Focus();
                return;
            }

            // 2. Get dates from Pickers
            DateTime bookingDate = datePicker.Value.Date;
            DateTime endDate = datePickerTo.Value.Date;

            if (endDate < bookingDate)
            {
                MessageBox.Show("End date cannot be earlier than start date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int days = (endDate - bookingDate).Days;
            if (days <= 0) days = 1;

            string selectedDate = $"{bookingDate.ToString("MMM dd, yyyy")} to {endDate.ToString("MMM dd, yyyy")} ({days} day{(days > 1 ? "s" : "")})";

            // 3. Find selected vehicle button
            Button selectedBtn = null;
            string selectedVehicle = "None";
            foreach (Control c in panelAvailableVehicles.Controls)
            {
                if (c is Button btn && btn.BackColor == Color.FromArgb(135, 226, 98))
                {
                    selectedVehicle = btn.Text;
                    selectedBtn = btn;
                    break;
                }
            }

            if (selectedBtn == null)
            {
                MessageBox.Show("Please select a vehicle from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int vehicleId = GetSelectedVehicleId(selectedBtn);
            decimal dailyRate = 0;
            if (selectedBtn.Tag != null)
            {
                decimal.TryParse(selectedBtn.Tag.ToString(), out dailyRate);
            }
            decimal subtotal = dailyRate * days;

            // 4. Construct message box description
            string actionText = status == "Confirmed" ? "confirm" : status == "Draft" ? "draft" : "discard";
            string message = $"--- TECHDRIVE RENTAL SUMMARY ---\n\n" +
                  $"Rental ID: {rentalID}\n" +
                  $"Vehicle: {selectedVehicle.Replace("\n", " ")}\n" +
                  $"Total Price: ₱{subtotal:N2}\n" +
                  $"--------------------------------\n" +
                  $"Customer: {name}\n" +
                  $"Email: {email}\n" +
                  $"Address: {address}\n" +
                  $"Contact: {contact}\n" +
                  $"License: {license}\n" +
                  $"Payment Ref: {(string.IsNullOrEmpty(payments) ? "N/A" : payments)}\n" +
                  $"Date: {selectedDate}\n\n" +
                  $"Do you want to {actionText} this booking?";

            // 5. Ask user validation confirm
            var confirm = MessageBox.Show(message, "Booking Action Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                // Write directly to CockroachDB!
                int createdBookingId = DatabaseHelper.BookVehicle(name, address, contact, license, vehicleId, bookingDate, endDate, subtotal, status, payments, email);
                if (createdBookingId > 0)
                {
                    if (status == "Confirmed" || status == "Draft")
                    {
                        string cleanVehicle = selectedVehicle.Split('\n')[0];
                        string pickupStr = bookingDate.ToString("MMM dd, yyyy") + " @ 9:00 AM";
                        string dropoffStr = endDate.ToString("MMM dd, yyyy") + " @ 5:00 PM";
                        
                        EmailHelper.SendBookingConfirmationEmail(email, name, createdBookingId.ToString(), pickupStr, dropoffStr, "TechDrive Hub - Pampanga", subtotal, payments, status);
                    }

                    MessageBox.Show($"Booking #{createdBookingId} successfully saved to CockroachDB with status: '{status}'!", "Database Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Clear inputs on success
                    nameTb.Text = "";
                    addressTb.Text = "";
                    cntctnumTb.Text = "";
                    lcnsenumTb.Text = "";
                    paymentsTb.Text = "";
                    if (emailTb != null) emailTb.Text = "";
                    priceLbl.Text = "";
                    datePicker.Value = DateTime.Today;
                    datePickerTo.Value = DateTime.Today.AddDays(1);
                    
                    // Reset all button styles
                    foreach (Control c in panelAvailableVehicles.Controls)
                    {
                        if (c is Button btn)
                        {
                            btn.BackColor = Color.FromArgb(2, 36, 78);
                            btn.ForeColor = Color.White;
                        }
                    }

                    // Refresh the rental ID for the next booking
                    RefreshRentalId();
                }
            }
        }
    }
}
