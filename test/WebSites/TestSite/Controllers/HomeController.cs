using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaulMiami.AspNetCore.Mvc.Recaptcha;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TestSite.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return Content("HELLO");
        }

        [HttpPost]
        [ValidateRecaptcha]
        public IActionResult Index(string model)
        {
            if (ModelState.IsValid)
            {
                return Content("OK");
            }

            return Content("FAIL");
        }
    }
}
