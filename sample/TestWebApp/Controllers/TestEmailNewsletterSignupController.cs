using Microsoft.AspNetCore.Mvc;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using TestWebApp.Models.TestEmailNewsletterSignup;

namespace TestWebApp.Controllers
{
    public class TestEmailNewsletterSignupController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult Index(TestEmailNewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ThankYou), new { name = viewModel.Name });
            }

            return View();
        }

        [HttpGet]
        public IActionResult ThankYou(string name)
        {
            return View(nameof(ThankYou), name);
        }
    }
}
