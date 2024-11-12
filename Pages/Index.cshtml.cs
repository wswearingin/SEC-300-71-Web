using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using MySqlConnector;

namespace SSD_web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(14, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 12 characters.")]
        public string firstName {  get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(25, ErrorMessage = "Maximum email length is 25 characters")]
        public string email {  get; set; }

        public bool isSuccess { get; set; } = false;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        private static Random random = new Random();
        private string RandomLinkGenerator(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void InsertRequest(string firstName, string email, string uid)
        {
            using (var con = new MySqlConnection("server=10.211.55.6;user id=webserver;database=registration;password=COURAGE7fluke9escrow3repay"))
            {
                con.Open();
                using (var command = con.CreateCommand())
                {
                    command.CommandText = "INSERT INTO requests (fname, email, rdate, uid, used) VALUES (@firstName, @Email, @RDate, @Uid, @Used)";

                    // Parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@RDate", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    command.Parameters.AddWithValue("@Uid", uid);
                    command.Parameters.AddWithValue("@Used", "0");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SendConfirmationEmail(string uid)
        {
            string host = "smtp.gmail.com";
            int port = 587;
            var from = "EMAIL";
            var username = "EMAIL";
            var password = "PASSWORD";

            var customerName = this.firstName;
            var to = this.email;

            var urlBase = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}{HttpContext.Request.PathBase.ToUriComponent()}";
            var uidURL = $"{urlBase}/confirmation?uid={uid}";

            var subject = "Confirm Registration";
            var body = $"Hi " + customerName + $" here is your link to confirm: {uidURL}";

            MailMessage msg = new MailMessage(from, to, subject, body);
            SmtpClient smtp = new SmtpClient(host, port);

            smtp.Credentials = new NetworkCredential(username, password);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;

            try
            {
                smtp.Send(msg);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "An exception occured");
            }
        }

        public void OnGet(string firstName, string email)
        {
            /*MySqlConnection con =
                new MySqlConnection("server=YOUR_DB_IP;user id=USERNAME;database=DB_NAME;password=PASSWORD");
            con.Open();
            using (con)
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM requests";
                    using (var reader = command.ExecuteReader())
                    {
                        var indexOfCol1 = reader.GetOrdinal("fName");
                        var indexOfCol2 = reader.GetOrdinal("email");
                        while (reader.Read())
                        {
                            this.firstName = reader.GetValue(indexOfCol1).ToString();
                            this.email = reader.GetValue(indexOfCol2).ToString();
                        }
                    }
                }
            }*/
                if (TempData["isSuccess"] != null)
                {
                    isSuccess = (bool)TempData["isSuccess"];
                }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                TempData["isSuccess"] = true;
                var uid = RandomLinkGenerator(32);
                SendConfirmationEmail(uid);
                InsertRequest(firstName, email, uid);
                firstName = string.Empty;
                email = string.Empty;
                return RedirectToPage();
            }

            return Page();
        }
    }
}
