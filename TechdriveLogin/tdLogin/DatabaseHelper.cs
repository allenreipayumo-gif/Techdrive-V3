using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace TechdriveLogin
{
    public static class DatabaseHelper
    {
        // Connection string pointing to your CockroachDB cluster
        // Adjust the Host, Database, Username, and Password to match your database settings.
        private static readonly string ConnectionString = 
            "Host=nordic-coyote-16113.jxf.gcp-asia-southeast1.cockroachlabs.cloud;" +
            "Port=26257;" +
            "Database=defaultdb;" +
            "Username=rome;" +
            "Password=j6fSYFN3UndFa7-smeTaKg;" +
            "SSL Mode=Require;";

        // Google Forms Responses Sheet Web API URL for counting unprocessed entries
        // Paste your deployed Google Apps Script Web App URL here!
        public static string GoogleFormsSheetsUrl = "https://script.google.com/macros/s/AKfycbzcs_oQqn2ZAwRGgnAFUkK9x18z5qy6op0q2LlyQ_aQDwo2QoW6vFGZoIj6YVgpnOSj/exec";

        // Run schema migrations on startup
        static DatabaseHelper()
        {
            try
            {
                // Force TLS 1.2 and 1.3 protocols to prevent secure channel handshake errors with Google Cloud APIs
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | (System.Net.SecurityProtocolType)3072 | (System.Net.SecurityProtocolType)12288;
            }
            catch { }

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    string alterQuery = "ALTER TABLE vehicles ADD COLUMN IF NOT EXISTS last_maintenance DATE;";
                    using (var cmd = new NpgsqlCommand(alterQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Add payments column to bookings if not exists
                    string alterBookingsQuery = "ALTER TABLE bookings ADD COLUMN IF NOT EXISTS payments VARCHAR(100);";
                    using (var cmdBookings = new NpgsqlCommand(alterBookingsQuery, conn))
                    {
                        cmdBookings.ExecuteNonQuery();
                    }

                    // Add customer_email column to bookings if not exists
                    string alterEmailQuery = "ALTER TABLE bookings ADD COLUMN IF NOT EXISTS customer_email VARCHAR(255);";
                    using (var cmdEmail = new NpgsqlCommand(alterEmailQuery, conn))
                    {
                        cmdEmail.ExecuteNonQuery();
                    }

                    // Add expiry_email_sent column to bookings if not exists
                    string alterExpirySentQuery = "ALTER TABLE bookings ADD COLUMN IF NOT EXISTS expiry_email_sent BOOLEAN DEFAULT FALSE;";
                    using (var cmdExpiry = new NpgsqlCommand(alterExpirySentQuery, conn))
                    {
                        cmdExpiry.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // Ignore silent failure if offline
            }
        }

        // Get an active open connection
        public static NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        // Flag to prevent Application.Exit from running when logging out programmatically
        public static bool IsLoggingOut { get; set; } = false;

        // 0. Global Logout routing and session clearance
        public static void Logout(System.Windows.Forms.Form currentForm)
        {
            IsLoggingOut = true;
            try
            {
                System.Windows.Forms.Form loginForm = null;
                List<System.Windows.Forms.Form> formsToClose = new List<System.Windows.Forms.Form>();
                
                foreach (System.Windows.Forms.Form openForm in System.Windows.Forms.Application.OpenForms)
                {
                    if (openForm.GetType().Name == "Form1")
                    {
                        loginForm = openForm;
                    }
                    else
                    {
                        formsToClose.Add(openForm);
                    }
                }
                
                if (loginForm != null)
                {
                    loginForm.Show();
                }
                
                // Close other forms safely by hiding them first to avoid screen flickers
                foreach (var form in formsToClose)
                {
                    form.Hide();
                }
                foreach (var form in formsToClose)
                {
                    form.Close();
                }
            }
            finally
            {
                IsLoggingOut = false;
            }
        }

        // 1. Authenticate a User
        public static bool ValidateUser(string username, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT password_hash FROM users WHERE username = @username LIMIT 1;";
                    string correctPassword = null;

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        correctPassword = cmd.ExecuteScalar() as string;
                    }

                    bool loginSuccess = correctPassword != null && correctPassword == password;
                    string action = loginSuccess ? "Login Success" : "Login Failed";

                    // Insert audit log in CockroachDB
                    string logQuery = "INSERT INTO audit_logs (username, action) VALUES (@username, @action);";
                    using (var cmdLog = new NpgsqlCommand(logQuery, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@username", username);
                        cmdLog.Parameters.AddWithValue("@action", action);
                        cmdLog.ExecuteNonQuery();
                    }

                    return loginSuccess;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database Error: " + ex.Message, "Database Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        // 2. Fetch Fleet Availability Statistics for Dashboard
        public static Dictionary<string, int> GetFleetStats()
        {
            var stats = new Dictionary<string, int>
            {
                { "Available", 0 },
                { "RentInProgress", 0 },
                { "InMaintenance", 0 }
            };

            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT status, COUNT(*) FROM vehicles GROUP BY status;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string status = reader.GetString(0);
                            int count = Convert.ToInt32(reader.GetValue(1));

                            if (status == "Available") stats["Available"] = count;
                            else if (status == "Rent in progress") stats["RentInProgress"] = count;
                            else if (status == "In Maintenance") stats["InMaintenance"] = count;
                        }
                    }

                    // Count how many 'Available' vehicles are actually booked today
                    string bookedQuery = @"
                        SELECT COUNT(DISTINCT v.vehicle_id) 
                        FROM vehicles v
                        JOIN bookings b ON v.vehicle_id = b.vehicle_id
                        WHERE v.status = 'Available'
                          AND b.status = 'Confirmed'
                          AND CURRENT_DATE BETWEEN b.booking_date AND b.end_date;";
                    using (var cmdBooked = new NpgsqlCommand(bookedQuery, conn))
                    {
                        int bookedCount = Convert.ToInt32(cmdBooked.ExecuteScalar());
                        stats["Available"] = Math.Max(0, stats["Available"] - bookedCount);
                        stats["RentInProgress"] = stats["RentInProgress"] + bookedCount;
                    }
                }
            }
            catch (Exception)
            {
                // Return default zeros on error
            }
            return stats;
        }

        // 3. Automated warning notifier for ended bookings
        public static List<string> CheckAndGenerateBookingAlerts()
        {
            var activeAlertsList = new List<string>();
            try
            {
                // First, fetch and prepend nearest vehicles ONLY if they are 7 days or shorter away, or overdue
                var maintProximity = GetVehiclesMaintenanceProximity();
                foreach (var v in maintProximity)
                {
                    if (v.DaysLeftValue <= 7)
                    {
                        activeAlertsList.Add($"Maintenance Due: {v.ModelName} ({v.PlateNumber}) - {v.DaysLeftDetail}");
                    }
                }

                using (var conn = GetConnection())
                {
                    // Clean up and auto-resolve ended booking alerts if booking is no longer Confirmed
                    string resolveEndedAlertsQuery = @"
                        UPDATE alerts 
                        SET is_resolved = TRUE 
                        FROM bookings b 
                        WHERE alerts.description LIKE 'Warning: Rental period has ended for Booking #' || b.booking_id || '%' 
                          AND b.status <> 'Confirmed';";

                    using (var cmdResolveEnded = new NpgsqlCommand(resolveEndedAlertsQuery, conn))
                    {
                        cmdResolveEnded.ExecuteNonQuery();
                    }

                    // Clean up and auto-resolve payment alerts if payment reference is added or status is no longer Draft
                    string resolvePaymentAlertsQuery = @"
                        UPDATE alerts 
                        SET is_resolved = TRUE 
                        FROM bookings b 
                        WHERE alerts.description LIKE 'Outstanding Payment: Booking #' || b.booking_id || '%' 
                          AND (b.payments IS NOT NULL AND b.payments <> '' OR b.status <> 'Draft');";

                    using (var cmdResolvePayment = new NpgsqlCommand(resolvePaymentAlertsQuery, conn))
                    {
                        cmdResolvePayment.ExecuteNonQuery();
                    }

                    // Transact/Query to auto-generate warning alerts for ended bookings
                    string insertAlertsQuery = @"
                        INSERT INTO alerts (vehicle_id, description, severity, due_date)
                        SELECT 
                            b.vehicle_id, 
                            'Warning: Rental period has ended for Booking #' || CAST(b.booking_id AS VARCHAR) || ' (Customer: ' || b.customer_name || '). Vehicle is due for return.', 
                            'High', 
                            b.end_date
                        FROM bookings b
                        LEFT JOIN alerts a ON a.description LIKE '%Booking #' || CAST(b.booking_id AS VARCHAR) || '%'
                        WHERE b.end_date <= CURRENT_DATE 
                          AND b.status = 'Confirmed'
                          AND a.alert_id IS NULL;";

                    using (var cmdInsert = new NpgsqlCommand(insertAlertsQuery, conn))
                    {
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Transact/Query to auto-generate maintenance alerts (due in less than 7 days, 3-month cycle)
                    string insertMaintAlertsQuery = @"
                        INSERT INTO alerts (vehicle_id, description, severity, due_date)
                        SELECT 
                            v.vehicle_id,
                            'Maintenance Due: ' || v.make || ' ' || v.model || ' (Plate: ' || v.plate_number || ') is due for its 3-month routine maintenance check.',
                            'Medium',
                            (v.last_maintenance + INTERVAL '3 months')::DATE
                        FROM vehicles v
                        LEFT JOIN alerts a ON a.vehicle_id = v.vehicle_id AND a.description LIKE 'Maintenance Due:%' AND a.is_resolved = FALSE
                        WHERE v.last_maintenance IS NOT NULL
                          AND CURRENT_DATE >= (v.last_maintenance + INTERVAL '3 months' - INTERVAL '7 days')::DATE
                          AND a.alert_id IS NULL;";

                    using (var cmdInsertMaint = new NpgsqlCommand(insertMaintAlertsQuery, conn))
                    {
                        cmdInsertMaint.ExecuteNonQuery();
                    }

                    // Transact/Query to auto-generate outstanding payments alerts
                    string insertPaymentAlertsQuery = @"
                        INSERT INTO alerts (vehicle_id, description, severity, due_date)
                        SELECT 
                            b.vehicle_id, 
                            'Outstanding Payment: Booking #' || CAST(b.booking_id AS VARCHAR) || ' (Customer: ' || b.customer_name || ') has no payment reference recorded.', 
                            'Low', 
                            b.booking_date
                        FROM bookings b
                        LEFT JOIN alerts a ON a.description LIKE '%Outstanding Payment: Booking #' || CAST(b.booking_id AS VARCHAR) || '%'
                        WHERE b.status = 'Draft' 
                          AND (b.payments IS NULL OR b.payments = '')
                          AND a.alert_id IS NULL;";

                    using (var cmdInsertPayment = new NpgsqlCommand(insertPaymentAlertsQuery, conn))
                    {
                        cmdInsertPayment.ExecuteNonQuery();
                    }

                    // Query to fetch all unresolved active warnings (excluding static maintenance alerts to prevent duplication)
                    string fetchAlertsQuery = "SELECT description FROM alerts WHERE is_resolved = FALSE AND description NOT LIKE 'Maintenance Due:%' ORDER BY created_at DESC;";
                    using (var cmdFetch = new NpgsqlCommand(fetchAlertsQuery, conn))
                    using (var reader = cmdFetch.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activeAlertsList.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Dynamic maintenance alerts will already be loaded from GetVehiclesMaintenanceProximity offline fallback!
            }
            return activeAlertsList;
        }

        public static int GetNextBookingId()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT COALESCE(MAX(booking_id), 16656) + 1 FROM bookings;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                return 16657; // Fallback default
            }
        }

        // 4. Insert a Booking record into CockroachDB
        public static int BookVehicle(string name, string address, string contact, string license, int vehicleId, DateTime bookingDate, DateTime endDate, decimal subtotal, string status, string payments, string email)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string insertQuery = @"
                        INSERT INTO bookings (customer_name, address, contact_number, license_number, vehicle_id, booking_date, end_date, subtotal, status, payments, customer_email)
                        VALUES (@name, @address, @contact, @license, @vehicleId, @bookingDate, @endDate, @subtotal, @status, @payments, @email)
                        RETURNING booking_id;";

                    using (var cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@contact", contact);
                        cmd.Parameters.AddWithValue("@license", license);
                        cmd.Parameters.AddWithValue("@vehicleId", vehicleId);
                        cmd.Parameters.AddWithValue("@bookingDate", bookingDate.Date);
                        cmd.Parameters.AddWithValue("@endDate", endDate.Date);
                        cmd.Parameters.AddWithValue("@subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@payments", payments);
                        cmd.Parameters.AddWithValue("@email", email);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database Booking Error: " + ex.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return -1;
            }
        }

        // 5. Get Upcoming Bookings Count (Queries Google Forms Responses Sheet Web API completely, default is 0)
        public static int GetUpcomingBookingsCount()
        {
            if (!string.IsNullOrEmpty(GoogleFormsSheetsUrl) && !GoogleFormsSheetsUrl.StartsWith("YOUR_"))
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);
                        // Fetch JSON from the Apps Script Web App (returns {"unprocessed": X})
                        var response = client.GetStringAsync(GoogleFormsSheetsUrl).Result;
                        
                        // Extract digits following the unprocessed key using robust Regex
                        var match = System.Text.RegularExpressions.Regex.Match(response, @"""unprocessed""\s*:\s*(\d+)");
                        if (match.Success)
                        {
                            return int.Parse(match.Groups[1].Value);
                        }
                    }
                }
                catch (Exception)
                {
                    // Default to 0 on network/timeout errors
                }
            }

            return 0;
        }

        // 6. Get Details of the Latest Booking
        public static LatestBookingInfo GetLatestBooking()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT customer_name, booking_date, status FROM bookings ORDER BY created_at DESC LIMIT 1;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LatestBookingInfo
                            {
                                CustomerName = reader.GetString(0),
                                BookingDate = reader.GetDateTime(1),
                                Status = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to null on failure
            }
            return null;
        }

        // 6b. Get Details of the Recent Bookings
        public static List<LatestBookingInfo> GetRecentBookings(int limit = 4)
        {
            var list = new List<LatestBookingInfo>();
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT customer_name, booking_date, status FROM bookings ORDER BY created_at DESC LIMIT @limit;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new LatestBookingInfo
                                {
                                    CustomerName = reader.GetString(0),
                                    BookingDate = reader.GetDateTime(1),
                                    Status = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to empty list on failure
            }
            return list;
        }

        // 7. Get Vehicles List
        public static List<VehicleInfo> GetVehicles(int limit = 7)
        {
            var list = new List<VehicleInfo>();
            try
            {
                using (var conn = GetConnection())
                {
                    string query = @"
                        SELECT v.vehicle_id, v.make, v.model, v.plate_number, v.status, v.daily_rate, v.last_maintenance,
                               (SELECT COUNT(*) FROM bookings b 
                                WHERE b.vehicle_id = v.vehicle_id 
                                  AND b.status = 'Confirmed' 
                                  AND CURRENT_DATE BETWEEN b.booking_date AND b.end_date) > 0 AS is_booked_today
                        FROM vehicles v
                        ORDER BY v.vehicle_id
                        LIMIT @limit;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int vehicleId = reader.GetInt32(0);
                                string make = reader.GetString(1);
                                string model = reader.GetString(2);
                                string plate = reader.GetString(3);
                                string status = reader.GetString(4);
                                decimal dailyRate = reader.GetDecimal(5);
                                object lastMaintObj = reader.GetValue(6);
                                bool isBookedToday = reader.GetBoolean(7);

                                if (status == "Available" && isBookedToday)
                                {
                                    status = "Out for Rental";
                                }

                                DateTime? lastMaint = null;
                                if (lastMaintObj != null && lastMaintObj != DBNull.Value)
                                {
                                    lastMaint = Convert.ToDateTime(lastMaintObj);
                                }

                                string remarks = "Good Condition";
                                if (status == "In Maintenance")
                                {
                                    remarks = "Under Maintenance";
                                }
                                else
                                {
                                    if (lastMaint == null)
                                    {
                                        remarks = "Dated for Maintenance";
                                    }
                                    else
                                    {
                                        DateTime nextMaint = lastMaint.Value.AddMonths(3);
                                        if (nextMaint.Date < DateTime.Today)
                                        {
                                            remarks = "Dated for Maintenance";
                                        }
                                    }
                                }

                                list.Add(new VehicleInfo
                                {
                                    VehicleId = vehicleId,
                                    Make = make,
                                    Model = model,
                                    PlateNumber = plate,
                                    Status = status,
                                    Remarks = remarks,
                                    DailyRate = dailyRate
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to offline mock database list if offline
                var mockVehicles = new[]
                {
                    new { Make = "Toyota", Model = "Vios", Rate = 1500.00m, Plate = "NDG-4812", Remarks = "Good Condition" },
                    new { Make = "Ford", Model = "Everest", Rate = 3500.00m, Plate = "NFC-2930", Remarks = "Dated for Maintenance" },
                    new { Make = "Mitsubishi", Model = "Mirage", Rate = 1200.00m, Plate = "AAA-8765", Remarks = "Good Condition" },
                    new { Make = "Toyota", Model = "Fortuner", Rate = 3200.00m, Plate = "NDG-9102", Remarks = "Dated for Maintenance" },
                    new { Make = "Toyota", Model = "Veloz", Rate = 2200.00m, Plate = "NFI-4821", Remarks = "Good Condition" },
                    new { Make = "Toyota", Model = "Hiace", Rate = 4000.00m, Plate = "NDG-1667", Remarks = "Good Condition" },
                    new { Make = "Toyota", Model = "Rush", Rate = 2000.00m, Plate = "NFI-3098", Remarks = "Good Condition" },
                    new { Make = "Ford", Model = "Ranger", Rate = 3000.00m, Plate = "NFC-8371", Remarks = "Good Condition" },
                    new { Make = "Mitsubishi", Model = "Xpander", Rate = 2400.00m, Plate = "AAA-4321", Remarks = "Good Condition" },
                    new { Make = "Toyota", Model = "Hilux", Rate = 2800.00m, Plate = "NDG-7741", Remarks = "Good Condition" },
                    new { Make = "Honda", Model = "BR-V", Rate = 2200.00m, Plate = "NFK-5928", Remarks = "Good Condition" },
                    new { Make = "Toyota", Model = "Vios", Rate = 1500.00m, Plate = "NDG-4812", Remarks = "Good Condition" }
                };

                int id = 1;
                foreach (var v in mockVehicles)
                {
                    list.Add(new VehicleInfo
                    {
                        VehicleId = id++,
                        Make = v.Make,
                        Model = v.Model,
                        PlateNumber = v.Plate,
                        Status = "Available",
                        Remarks = v.Remarks,
                        DailyRate = v.Rate
                    });
                }
            }
            return list;
        }

        // 8. Toggle Vehicle Status (Available <-> In Maintenance)
        public static bool ToggleVehicleStatus(int vehicleId, string currentStatus)
        {
            try
            {
                string newStatus = currentStatus == "Available" ? "In Maintenance" : "Available";
                using (var conn = GetConnection())
                {
                    string query;
                    if (newStatus == "In Maintenance")
                    {
                        query = "UPDATE vehicles SET status = @status, last_maintenance = CURRENT_DATE WHERE vehicle_id = @id;";
                    }
                    else
                    {
                        query = "UPDATE vehicles SET status = @status WHERE vehicle_id = @id;";
                    }
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 9. Get Vehicle Report Statistics (trips, earnings, maintenance dates)
        public static List<VehicleReportInfo> GetVehicleReports(int limit = 8)
        {
            var list = new List<VehicleReportInfo>();
            try
            {
                using (var conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            v.make || ' ' || v.model,
                            v.plate_number,
                            COALESCE(to_char(v.last_maintenance, 'Mon DD, YYYY'), 'Jan 15, 2025'),
                            (SELECT COUNT(*) FROM bookings b WHERE b.vehicle_id = v.vehicle_id),
                            COALESCE((SELECT SUM(subtotal) FROM bookings b WHERE b.vehicle_id = v.vehicle_id AND b.status = 'Confirmed'), 0)
                        FROM vehicles v
                        ORDER BY 5 DESC, 4 DESC
                        LIMIT @limit;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new VehicleReportInfo
                                {
                                    VehicleModel = reader.GetString(0),
                                    PlateNumber = reader.GetString(1),
                                    LastMaintenance = reader.GetString(2),
                                    TotalTrips = reader.GetInt32(3),
                                    NetEarnings = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to empty list
            }
            return list;
        }

        public static List<VehicleMaintenanceProximityInfo> GetVehiclesMaintenanceProximity()
        {
            var list = new List<VehicleMaintenanceProximityInfo>();
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT make, model, plate_number, last_maintenance FROM vehicles;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string make = reader.GetString(0);
                            string model = reader.GetString(1);
                            string plate = reader.GetString(2);
                            object lastMaintObj = reader.GetValue(3);

                            string lastMaintStr;
                            string nextMaintStr;
                            string daysLeftDetail;
                            int daysLeftValue;

                            if (lastMaintObj == null || lastMaintObj == DBNull.Value)
                            {
                                lastMaintStr = "Never";
                                nextMaintStr = "Immediately";
                                daysLeftDetail = "OVERDUE (No maintenance history)";
                                daysLeftValue = -9999;
                            }
                            else
                            {
                                DateTime lastMaint = Convert.ToDateTime(lastMaintObj);
                                DateTime nextMaint = lastMaint.AddMonths(3);
                                lastMaintStr = lastMaint.ToString("MMM dd, yyyy");
                                nextMaintStr = nextMaint.ToString("MMM dd, yyyy");

                                int daysLeft = (nextMaint.Date - DateTime.Today).Days;
                                daysLeftValue = daysLeft;

                                if (daysLeft < 0)
                                {
                                    daysLeftDetail = $"OVERDUE by {Math.Abs(daysLeft)} days";
                                }
                                else if (daysLeft == 0)
                                {
                                    daysLeftDetail = "Due TODAY";
                                }
                                else
                                {
                                    daysLeftDetail = $"{daysLeft} days left";
                                }
                            }

                            list.Add(new VehicleMaintenanceProximityInfo
                            {
                                ModelName = make + " " + model,
                                PlateNumber = plate,
                                LastMaintenanceStr = lastMaintStr,
                                NextMaintenanceStr = nextMaintStr,
                                DaysLeftDetail = daysLeftDetail,
                                DaysLeftValue = daysLeftValue
                            });
                        }
                    }
                }
                list.Sort((x, y) => x.DaysLeftValue.CompareTo(y.DaysLeftValue));
            }
            catch (Exception)
            {
                // Fallback mock data for offline mode
                list.Add(new VehicleMaintenanceProximityInfo
                {
                    ModelName = "Toyota Fortuner",
                    PlateNumber = "NDG-9102",
                    LastMaintenanceStr = "Feb 10, 2026",
                    NextMaintenanceStr = "May 10, 2026",
                    DaysLeftDetail = "OVERDUE by 14 days",
                    DaysLeftValue = -14
                });
                list.Add(new VehicleMaintenanceProximityInfo
                {
                    ModelName = "Ford Ranger",
                    PlateNumber = "NFC-8371",
                    LastMaintenanceStr = "Feb 26, 2026",
                    NextMaintenanceStr = "May 26, 2026",
                    DaysLeftDetail = "2 days left",
                    DaysLeftValue = 2
                });
                list.Add(new VehicleMaintenanceProximityInfo
                {
                    ModelName = "Mitsubishi Xpander",
                    PlateNumber = "AAA-4321",
                    LastMaintenanceStr = "Mar 01, 2026",
                    NextMaintenanceStr = "Jun 01, 2026",
                    DaysLeftDetail = "8 days left",
                    DaysLeftValue = 8
                });
            }
            return list;
        }

        public static (int totalBookings, decimal netEarnings) GetMonthlyReportStats()
        {
            int totalBookings = 0;
            decimal netEarnings = 0;
            try
            {
                using (var conn = GetConnection())
                {
                    string query = @"
                        SELECT 
                            (SELECT COUNT(*) FROM bookings WHERE date_trunc('month', booking_date) = date_trunc('month', CURRENT_DATE) AND status != 'Discarded'),
                            (SELECT COALESCE(SUM(subtotal), 0) FROM bookings WHERE date_trunc('month', booking_date) = date_trunc('month', CURRENT_DATE) AND status = 'Confirmed');";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalBookings = Convert.ToInt32(reader.GetValue(0));
                            netEarnings = Convert.ToDecimal(reader.GetValue(1));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback mock values for offline mode
                totalBookings = 18;
                netEarnings = 42500.00m;
            }
            return (totalBookings, netEarnings);
        }

        public static bool AddVehicle(string make, string model, decimal dailyRate, string plateNumber, string status)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string query = @"
                        INSERT INTO vehicles (make, model, daily_rate, plate_number, status)
                        VALUES (@make, @model, @dailyRate, @plate, @status);";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@make", make);
                        cmd.Parameters.AddWithValue("@model", model);
                        cmd.Parameters.AddWithValue("@dailyRate", dailyRate);
                        cmd.Parameters.AddWithValue("@plate", plateNumber);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database Add Vehicle Error: " + ex.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DeleteVehicle(int vehicleId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    // 1. Delete associated bookings first to prevent foreign key constraint violation (Error 23503)
                    string deleteBookingsQuery = "DELETE FROM bookings WHERE vehicle_id = @id;";
                    using (var cmd = new NpgsqlCommand(deleteBookingsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Delete associated alerts
                    string deleteAlertsQuery = "DELETE FROM alerts WHERE vehicle_id = @id;";
                    using (var cmd = new NpgsqlCommand(deleteAlertsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Finally delete the vehicle
                    string query = "DELETE FROM vehicles WHERE vehicle_id = @id;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database Delete Vehicle Error: " + ex.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool EndBooking(int vehicleId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    // 1. Update the active confirmed booking(s) for this vehicle today
                    // Set status to 'Completed' and adjust end_date to yesterday so it is marked inactive
                    string updateBookingQuery = @"
                        UPDATE bookings 
                        SET status = 'Completed', end_date = CURRENT_DATE - 1 
                        WHERE vehicle_id = @id 
                          AND status = 'Confirmed' 
                          AND CURRENT_DATE BETWEEN booking_date AND end_date;";
                    
                    using (var cmd = new NpgsqlCommand(updateBookingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Set the vehicle status back to 'Available' in case it was out
                    string updateVehicleQuery = "UPDATE vehicles SET status = 'Available' WHERE vehicle_id = @id;";
                    using (var cmd = new NpgsqlCommand(updateVehicleQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database End Booking Error: " + ex.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        public static void CheckAndSendExpiringEmails()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    // Find confirmed bookings expiring within the next 24 hours that haven't sent the expiry email yet
                    string query = @"
                        SELECT b.booking_id, b.customer_name, b.customer_email, b.end_date, v.make, v.model
                        FROM bookings b
                        JOIN vehicles v ON b.vehicle_id = v.vehicle_id
                        WHERE b.status = 'Confirmed' 
                          AND b.end_date <= CURRENT_DATE + 1 
                          AND b.end_date >= CURRENT_DATE
                          AND COALESCE(b.expiry_email_sent, FALSE) = FALSE;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var expiringList = new List<ExpiringBookingInfo>();
                        while (reader.Read())
                        {
                            expiringList.Add(new ExpiringBookingInfo
                            {
                                BookingId = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                CustomerEmail = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                EndDate = reader.GetDateTime(3),
                                VehicleName = reader.GetString(4) + " " + reader.GetString(5)
                            });
                        }
                        reader.Close();

                        foreach (var b in expiringList)
                        {
                            if (!string.IsNullOrEmpty(b.CustomerEmail))
                            {
                                string expiryStr = b.EndDate.ToString("yyyy-MM-dd") + "T17:00:00+08:00"; // Format for template
                                EmailHelper.SendBookingExpiryEmail(b.CustomerEmail, b.CustomerName, b.BookingId.ToString(), expiryStr);

                                // Mark as sent in DB
                                string updateQuery = "UPDATE bookings SET expiry_email_sent = TRUE WHERE booking_id = @id;";
                                using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@id", b.BookingId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silent catch
            }
        }
    }

    public class LatestBookingInfo
    {
        public string CustomerName { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }

    public class VehicleInfo
    {
        public int VehicleId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string PlateNumber { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public decimal DailyRate { get; set; }
    }

    public class VehicleReportInfo
    {
        public string VehicleModel { get; set; }
        public string PlateNumber { get; set; }
        public string LastMaintenance { get; set; }
        public int TotalTrips { get; set; }
        public decimal NetEarnings { get; set; }
    }

    public class VehicleMaintenanceProximityInfo
    {
        public string ModelName { get; set; }
        public string PlateNumber { get; set; }
        public string LastMaintenanceStr { get; set; }
        public string NextMaintenanceStr { get; set; }
        public string DaysLeftDetail { get; set; }
        public int DaysLeftValue { get; set; }
    }

    public class ExpiringBookingInfo
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime EndDate { get; set; }
        public string VehicleName { get; set; }
    }
}
