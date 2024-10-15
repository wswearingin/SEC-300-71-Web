using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //public void OnGet(string firstName, string email)
        //{
        //    this.firstName = firstName;
        //    this.email = email;
        //}

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
