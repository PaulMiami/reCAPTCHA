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
        public IActionResult Localization()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult Localization(TestEmailNewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ThankYou), new { name = viewModel.Name });
            }

            return View();
        }

        [HttpGet]
        public IActionResult DarkTheme()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult DarkTheme(TestEmailNewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ThankYou), new { name = viewModel.Name });
            }

            return View();
        }

        [HttpGet]
        public IActionResult JqueryValidationDisabled()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult JqueryValidationDisabled(TestEmailNewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ThankYou), new { name = viewModel.Name });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Compact()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult Compact(TestEmailNewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ThankYou), new { name = viewModel.Name });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Audio()
        {
            return View();
        }

        [ValidateRecaptcha]
        [HttpPost]
        public IActionResult Audio(TestEmailNewsletterViewModel viewModel)
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
