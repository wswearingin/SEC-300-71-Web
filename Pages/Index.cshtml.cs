using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

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
        public string email {  get; set; }

        public bool isSuccess { get; set; } = false;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void SendConfirmationEmail()
        {
            string host = "smtp.gmail.com";
            int port = 587;
            var from = "william.swearingin@mymail.champlain.edu";
            var username = "william.swearingin@mymail.champlain.edu";
            var password = "PASSWORD";

            var customerName = this.firstName;
            var to = this.email;

            var subject = "Confirm Registration";
            var body = "Hi " + customerName + " here is your link to confirm: <link>";

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
        public void OnGet()
        {
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
                SendConfirmationEmail();
                firstName = string.Empty;
                email = string.Empty;
                return RedirectToPage();
            }

            return Page();
        }
    }
}
