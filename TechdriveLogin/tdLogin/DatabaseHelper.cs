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

        // Run schema migrations on startup
        static DatabaseHelper()
        {
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
                using (var conn = GetConnection())
                {
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

                    // Query to fetch all unresolved active warnings
                    string fetchAlertsQuery = "SELECT description FROM alerts WHERE is_resolved = FALSE ORDER BY created_at DESC;";
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
                // Return fallback alerts if database is offline
            }
            return activeAlertsList;
        }

        // 4. Insert a Booking record into CockroachDB
        public static bool BookVehicle(string name, string address, string contact, string license, int vehicleId, DateTime bookingDate, DateTime endDate, decimal subtotal, string status)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string insertQuery = @"
                        INSERT INTO bookings (customer_name, address, contact_number, license_number, vehicle_id, booking_date, end_date, subtotal, status)
                        VALUES (@name, @address, @contact, @license, @vehicleId, @bookingDate, @endDate, @subtotal, @status);";

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

                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Database Booking Error: " + ex.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        // 5. Get Upcoming Bookings Count
        public static int GetUpcomingBookingsCount()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT COUNT(*) FROM bookings WHERE booking_date >= CURRENT_DATE AND status != 'Discarded';";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
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

        // 7. Get Vehicles List
        public static List<VehicleInfo> GetVehicles(int limit = 7)
        {
            var list = new List<VehicleInfo>();
            try
            {
                using (var conn = GetConnection())
                {
                    string query = "SELECT vehicle_id, make, model, plate_number, status FROM vehicles ORDER BY vehicle_id LIMIT @limit;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new VehicleInfo
                                {
                                    VehicleId = reader.GetInt32(0),
                                    Make = reader.GetString(1),
                                    Model = reader.GetString(2),
                                    PlateNumber = reader.GetString(3),
                                    Status = reader.GetString(4),
                                    Remarks = reader.GetString(4) == "In Maintenance" ? "Under Maintenance" : "Good Condition"
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
    }

    public class VehicleReportInfo
    {
        public string VehicleModel { get; set; }
        public string PlateNumber { get; set; }
        public string LastMaintenance { get; set; }
        public int TotalTrips { get; set; }
        public decimal NetEarnings { get; set; }
    }
}
