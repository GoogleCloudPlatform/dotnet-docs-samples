using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace TwelveFactor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

         public IActionResult Index()
        {
            // Sends a message to configured loggers, including the Stackdriver logger.
            // The Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker logger will log all controller actions with
            // log level information. This log is for additional information.
            _logger.LogInformation("Home page hit!");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            // Log messages with different log levels.
            _logger.LogError("Error page hit!");
            return View();
        }

        public async Task<IActionResult> Metadata()
        {
            var metadata = await Services.GoogleCloudPlatform.Metadata
                .Create(_logger);
            if (metadata == null) {
                return new ContentResult() {
                    Content = "Not running on Google Cloud Platform",
                    ContentType = "text/plain"
                };
            }
            var content = (await metadata.Http
                .GetAsync("instance/?recursive=true")).Content;
            return new FileStreamResult(await content.ReadAsStreamAsync(),
                content.Headers.ContentType.MediaType);            
        }
    }
}
