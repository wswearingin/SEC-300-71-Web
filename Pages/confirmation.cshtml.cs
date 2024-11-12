using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System;

namespace SSD_web.Pages
{
    public class ConfirmationModel : PageModel
    {
        public bool IsConfirmed { get; set; } = false;
        public string Message { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }

        public void OnGet(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                Message = "Invalid confirmation link.";
                return;
            }

            MySqlConnection con =
                new MySqlConnection("server=YOUR_DB_SERVER_IP;user id=USERNAME;database=DB_NAME;password=PASSWORD");
            con.Open();

            using (con)
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandText = "SELECT fname, email, rdate, used FROM requests WHERE uid = @uid AND used = '0'";
                    command.Parameters.AddWithValue("@uid", uid);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FirstName = reader["fname"].ToString();
                            Email = reader["email"].ToString();
                            int regEpoch = Convert.ToInt32(reader["rdate"]);
                            int currentEpoch = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            int timeElapsed = currentEpoch - regEpoch;

                            if (timeElapsed <= 300)
                            {
                                IsConfirmed = true;
                                Message = "Your registration has been confirmed!";
                            }
                            else
                            {
                                Message = "This confirmation link has expired.";
                            }
                        }
                        else
                        {
                            Message = "This  confirmation link is invalid or has already been used.";
                        }
                    }

                    using (var updateCommand = con.CreateCommand())
                    {
                        updateCommand.CommandText = "UPDATE requests SET used = '1' WHERE uid = @uid";
                        updateCommand.Parameters.AddWithValue("@uid", uid);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
