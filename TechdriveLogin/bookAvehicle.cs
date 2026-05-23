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
        public bookAvehicle()
        {
            InitializeComponent();
        }

       

        private void priceLbl_Click(object sender, EventArgs e)
        {

        }
      
        private void VehicleButton_Click(object sender, EventArgs e)
        {
            // 1. Loop through all buttons in the container (e.g., your Panel or GroupBox)
            // This resets EVERY button back to the original dark blue
            foreach (Control c in panelAvailableVehicles.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(2, 36, 78); // Your original dark blue
                    btn.ForeColor = Color.White;
                }
            }

            // 2. Now, highlight the specific button that was just clicked
            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.FromArgb(135, 226, 98); // Your prototype green
            clickedButton.ForeColor = Color.Black; // Dark text looks better on bright green

            // 3. Update the subtotal with the Peso sign
            if (clickedButton.Tag != null)
            {
                priceLbl.Text = $"Subtotal: ₱{clickedButton.Tag}";
            }
        }
        private int GetSelectedVehicleId(Button btn)
        {
            if (btn == null) return 1;
            switch (btn.Name)
            {
                case "vhclBtn1": return 1;
                case "button2": return 2;
                case "button3": return 3;
                case "button4": return 4;
                case "button5": return 5;
                case "button6": return 6;
                case "vhclBtn7": return 7;
                case "vhclBtn8": return 8;
                case "vhclBtn9": return 9;
                case "vhclBtn10": return 10;
                case "vhclBtn11": return 11;
                case "vhclBtn12": return 12;
                default: return 1;
            }
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
            string rentalID = "#16657";

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter Customer Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTb.Focus();
                return;
            }

            // 2. Get dates from Picker
            DateTime bookingDate = datePicker.Value;
            DateTime endDate = bookingDate.AddDays(1); // Default rental to 1 day duration
            string selectedDate = bookingDate.ToLongDateString();

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
            decimal subtotal = 0;
            if (selectedBtn.Tag != null)
            {
                decimal.TryParse(selectedBtn.Tag.ToString(), out subtotal);
            }

            // 4. Construct message box description
            string actionText = status == "Confirmed" ? "confirm" : status == "Draft" ? "draft" : "discard";
            string message = $"--- TECHDRIVE RENTAL SUMMARY ---\n\n" +
                  $"Rental ID: {rentalID}\n" +
                  $"Vehicle: {selectedVehicle}\n" +
                  $"Total Price: {priceLbl.Text}\n" +
                  $"--------------------------------\n" +
                  $"Customer: {name}\n" +
                  $"Address: {address}\n" +
                  $"Contact: {contact}\n" +
                  $"License: {license}\n" +
                  $"Date: {selectedDate}\n\n" +
                  $"Do you want to {actionText} this booking?";

            // 5. Ask user validation confirm
            var confirm = MessageBox.Show(message, "Booking Action Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                // Write directly to CockroachDB!
                bool success = DatabaseHelper.BookVehicle(name, address, contact, license, vehicleId, bookingDate, endDate, subtotal, status);
                if (success)
                {
                    MessageBox.Show($"Booking successfully saved to CockroachDB with status: '{status}'!", "Database Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Clear inputs on success
                    nameTb.Text = "";
                    addressTb.Text = "";
                    cntctnumTb.Text = "";
                    lcnsenumTb.Text = "";
                    priceLbl.Text = "";
                    
                    // Reset all button styles
                    foreach (Control c in panelAvailableVehicles.Controls)
                    {
                        if (c is Button btn)
                        {
                            btn.BackColor = Color.FromArgb(2, 36, 78);
                            btn.ForeColor = Color.White;
                        }
                    }
                }
            }
        }
    }
}
