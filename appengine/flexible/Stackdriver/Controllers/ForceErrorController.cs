using System;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Stackdriver.Controllers
{
    public class ForceErrorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Simulate an exception.
            bool exception = true;
            if (exception)
            {
                throw new Exception("Generic exception for testing Stackdriver Error Reporting");
            }
            return View();
        }
    }
}
