using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TechdriveLogin
{
    public static class EmailHelper
    {
        public static void SendBookingConfirmationEmail(string recipientEmail, string recipientName, string bookingId, string pickupDateStr, string dropoffDateStr, string hubName, decimal subtotal, string paymentRef, string status)
        {
            Task.Run(() =>
            {
                try
                {
                    string body = GetConfirmationTemplate(recipientName, bookingId, pickupDateStr, dropoffDateStr, hubName, subtotal, paymentRef, status);
                    string subject = status == "Draft" 
                        ? "Your TechDrive Booking Has Been Officially Placed (Draft)" 
                        : "Your TechDrive Booking Confirmation - Officially Placed";
                    SendEmailRaw(recipientEmail, subject, body);
                }
                catch (Exception)
                {
                    // Silent background tasks failure
                }
            });
        }

        public static void SendBookingExpiryEmail(string recipientEmail, string recipientName, string bookingId, string expiryTimeStr)
        {
            Task.Run(() =>
            {
                try
                {
                    string body = GetExpiryTemplate(recipientName, bookingId, expiryTimeStr);
                    SendEmailRaw(recipientEmail, "Your TechDrive Rental Expires in 24 Hours", body);
                }
                catch (Exception)
                {
                    // Silent background tasks failure
                }
            });
        }

        private static void SendEmailRaw(string toEmail, string subject, string body)
        {
            try
            {
                // Attempt Real SMTP (Gmail default settings)
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("support.techdrive@gmail.com", "TechDrive Car Rentals");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("support.techdrive@gmail.com", "iggw jmiu fhvo avne");
                        smtp.EnableSsl = true;
                        smtp.Timeout = 15000; // Increase to 15 seconds for robust live transmission
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback offline mode: Save the beautifully formatted HTML to the workspace so they can be inspected/displayed!
                try
                {
                    string logDir = @"C:\Users\allen\Documents\Techdrive V3\sent_emails";
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{subject.Replace(" ", "_")}.html";
                    string filePath = Path.Combine(logDir, fileName);
                    File.WriteAllText(filePath, body);

                    string errFile = $"{DateTime.Now:yyyyMMdd_HHmmss}_smtp_error.txt";
                    string errPath = Path.Combine(logDir, errFile);
                    File.WriteAllText(errPath, ex.ToString());
                }
                catch
                {
                    // Ignore local writing errors
                }
            }
        }

        private static string GetConfirmationTemplate(string name, string bookingId, string pickupStr, string dropoffStr, string hubName, decimal subtotal, string paymentRef, string status)
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss+08:00");
            string statusDisplay = status == "Draft" ? "Booking Placed (Draft)" : "Booking Confirmed";
            string statusColor = status == "Draft" ? "#FFDE59" : "#00ffcc"; // Yellow or Teal
            string messageIntro = status == "Draft"
                ? "Your ride has been successfully reserved! Your booking request has been officially placed as a draft in our system. Below are your dynamic trip summary details."
                : "Your ride is ready! Your rental has been locked into our system. Below are your dynamic trip summary details. Your vehicle features live GPS tracking for your on-road safety.";

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your TechDrive Booking Confirmation</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f6f9; margin: 0; padding: 20px; color: #333333;"">

    <script type=""application/ld+json"">
    {{
      ""@context"": ""http://schema.org"",
      ""@type"": ""RentalCarReservation"",
      ""reservationNumber"": ""{bookingId}"",
      ""reservationStatus"": ""http://schema.org/{(status == "Draft" ? "Hold" : "Confirmed")}"",
      ""underName"": {{
        ""@type"": ""Person"",
        ""name"": ""{name}""
      }},
      ""bookingAgent"": {{
        ""@type"": ""Organization"",
        ""name"": ""TechDrive"",
        ""url"": ""https://techdrive.ph""
      }},
      ""bookingTime"": ""{currentTime}"",
      ""pickupLocation"": {{
        ""@type"": ""Place"",
        ""name"": ""TechDrive Hub - Pampanga"",
        ""address"": {{
          ""@type"": ""PostalAddress"",
          ""streetAddress"": ""TechDrive HQ, Angeles City"",
          ""addressLocality"": ""Angeles City"",
          ""addressRegion"": ""Pampanga"",
          ""addressCountry"": ""PH""
        }}
      }},
      ""pickupTime"": ""{currentTime}"",
      ""dropoffLocation"": {{
        ""@type"": ""Place"",
        ""name"": ""TechDrive Hub - Pampanga"",
        ""address"": {{
          ""@type"": ""PostalAddress"",
          ""streetAddress"": ""TechDrive HQ, Angeles City"",
          ""addressLocality"": ""Angeles City"",
          ""addressRegion"": ""Pampanga"",
          ""addressCountry"": ""PH""
        }}
      }},
      ""dropoffTime"": ""{dropoffStr}"",
      ""rentalCompany"": {{
        ""@type"": ""Organization"",
        ""name"": ""TechDrive Car Rentals""
      }}
    }}
    </script>

    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05);"">
        <tr>
            <td style=""background: linear-gradient(135deg, #0f2027, #203a43, #2c5364); padding: 30px; text-align: center;"">
                <h1 style=""color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;"">TechDrive</h1>
                <p style=""color: {statusColor}; margin: 5px 0 0 0; font-size: 14px; font-weight: bold; text-transform: uppercase;"">{statusDisplay}</p>
            </td>
        </tr>
        
        <tr>
            <td style=""padding: 30px 30px 15px 30px;"">
                <h2 style=""margin: 0 0 10px 0; font-size: 20px; color: #111111;"">Hi {name},</h2>
                <p style=""margin: 0; line-height: 1.6; color: #555555;"">{messageIntro}</p>
            </td>
        </tr>

        <tr>
            <td style=""padding: 0 30px 20px 30px;"">
                <table width=""100%"" style=""background-color: #f8fafc; border-left: 4px solid #203a43; border-radius: 4px; padding: 20px;"">
                    <tr>
                        <td style=""padding-bottom: 10px;""><strong>Rental ID:</strong></td>
                        <td style=""text-align: right; padding-bottom: 10px; font-family: monospace; font-size: 14px; font-weight: bold;"">#{bookingId}</td>
                    </tr>
                    <tr>
                        <td style=""padding-bottom: 10px;""><strong>Pick-up Link:</strong></td>
                        <td style=""text-align: right; padding-bottom: 10px; color: #2c5364;"">{pickupStr}</td>
                    </tr>
                    <tr>
                        <td style=""padding-bottom: 10px;""><strong>Drop-off Link:</strong></td>
                        <td style=""text-align: right; padding-bottom: 10px; color: #2c5364;"">{dropoffStr}</td>
                    </tr>
                    <tr>
                        <td style=""padding-top: 10px; border-top: 1px solid #e2e8f0;""><strong>Station Hub:</strong></td>
                        <td style=""text-align: right; padding-top: 10px; font-size: 13px; color: #555555;"">{hubName}</td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td style=""padding: 0 30px 30px 30px;"">
                <table width=""100%"" style=""border-collapse: collapse;"">
                    <tr>
                        <td style=""padding: 10px 0; border-bottom: 1px solid #edf2f7; color: #718096;"">SaaS Platform Base</td>
                        <td style=""text-align: right; padding: 10px 0; border-bottom: 1px solid #edf2f7; font-weight: bold;"">₱{subtotal:N2}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 15px 0 5px 0; font-size: 18px; font-weight: bold; color: #111111;"">Total Account Paid{(string.IsNullOrEmpty(paymentRef) ? "" : " Ref: " + paymentRef)}</td>
                        <td style=""text-align: right; padding: 15px 0 5px 0; font-size: 18px; font-weight: bold; color: #203a43;"">₱{subtotal:N2}</td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td style=""background-color: #f7fafc; padding: 20px; text-align: center; border-top: 1px solid #edf2f7;"">
                <p style=""margin: 0; font-size: 12px; color: #a0aec0; line-height: 1.5;"">
                    Automated via TechDrive POS Network. <br>
                    By driving this vehicle, you remain bound to the registered digital platform <a href=""#"" style=""color: #2c5364; text-decoration: underline;"">Terms & Conditions</a>.
                </p>
            </td>
        </tr>
    </table>

</body>
</html>";
        }

        private static string GetExpiryTemplate(string name, string bookingId, string expiryTimeStr)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your TechDrive Rental Expires in 24 Hours</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f6f9; margin: 0; padding: 20px; color: #333333;"">

    <script type=""application/ld+json"">
    {{
      ""@context"": ""http://schema.org"",
      ""@type"": ""RentalCarReservation"",
      ""reservationNumber"": ""{bookingId}"",
      ""reservationStatus"": ""http://schema.org/Confirmed"",
      ""underName"": {{
        ""@type"": ""Person"",
        ""name"": ""{name}""
      }},
      ""bookingAgent"": {{
        ""@type"": ""Organization"",
        ""name"": ""TechDrive"",
        ""url"": ""https://techdrive.ph""
      }},
      ""dropoffTime"": ""{expiryTimeStr}""
    }}
    </script>

    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05);"">
        
        <tr>
            <td style=""background: linear-gradient(135deg, #8e1515, #b22222, #1a0505); padding: 30px; text-align: center;"">
                <h1 style=""color: #ffffff; margin: 0; font-size: 26px; letter-spacing: 1px;"">TechDrive</h1>
                <p style=""color: #ffcccc; margin: 5px 0 0 0; font-size: 13px; font-weight: bold; text-transform: uppercase; letter-spacing: 2px;"">⚠️ Action Required: 24 Hours Left</p>
            </td>
        </tr>
        
        <tr>
            <td style=""padding: 30px 30px 20px 30px;"">
                <h2 style=""margin: 0 0 12px 0; font-size: 20px; color: #111111;"">Hello {name},</h2>
                <p style=""margin: 0 0 15px 0; line-height: 1.6; color: #555555;"">
                    Your current vehicle rental period with TechDrive is scheduled to expire in exactly **24 hours**. 
                </p>
                <p style=""margin: 0 0 20px 0; line-height: 1.6; color: #555555;"">
                    To avoid scheduling conflicts, late turn-in penalties, or automated fleet management security tracking locks, the vehicle must be returned to your designated station hub on time.
                </p>
                <h3 style=""margin: 25px 0 10px 0; font-size: 16px; color: #111111; text-align: center;"">Need more time on the road?</h3>
                <p style=""margin: 0 0 25px 0; line-height: 1.6; color: #555555; text-align: center;"">
                    You can seamlessly extend your trip right now by submitting a fresh booking request via our digital portal.
                </p>
            </td>
        </tr>

        <tr>
            <td align=""center"" style=""padding: 0 30px 35px 30px;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                        <td align=""center"" bgcolor=""#b22222"" style=""border-radius: 6px;"">
                            <a href=""https://rei-website-ten.vercel.app/car-rental"" target=""_blank"" style=""display: inline-block; font-size: 16px; font-weight: bold; color: #ffffff; text-decoration: none; padding: 15px 35px; border-radius: 6px; border: 1px solid #b22222;"">
                                Extend Your Booking Now
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td style=""padding: 0 30px 30px 30px;"">
                <table width=""100%"" style=""background-color: #fff5f5; border-left: 4px solid #b22222; border-radius: 4px; padding: 15px; font-size: 14px;"">
                    <tr>
                        <td style=""color: #c53030;""><strong>Important Note:</strong> Vehicle availability is managed in real-time by our POS engine. Extensions are subject to immediate fleet availability. Secure your rebooking fast to prevent another client from reserving your vehicle.</td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td style=""background-color: #f7fafc; padding: 20px; text-align: center; border-top: 1px solid #edf2f7;"">
                <p style=""margin: 0; font-size: 11px; color: #a0aec0; line-height: 1.5;"">
                    Automated Alert Fleet System via TechDrive POS Networks. <br>
                    To review compliance parameters, check our platform <a href=""#"" style=""color: #8e1515; text-decoration: underline;"">Terms & Conditions</a>.
                </p>
            </td>
        </tr>
    </table>

</body>
</html>";
        }
    }
}
