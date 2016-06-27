using System.ComponentModel.DataAnnotations;

namespace TestWebApp.Models.TestEmailNewsletterSignup
{
    public class TestEmailNewsletterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
