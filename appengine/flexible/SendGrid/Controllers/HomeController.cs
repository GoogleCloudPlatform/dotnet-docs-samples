/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace SendGrid.Controllers
{
    public class HomeController : Controller
    {
        string _sendgridApiKey;
        public HomeController(IOptions<SendGridOptions> options)
        {
            _sendgridApiKey = options.Value.ApiKey;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index(SendForm sendForm)
        {
            var model = new HomeIndex();
            if (string.IsNullOrEmpty(_sendgridApiKey) ||
                "your-sendgrid-api-key" == _sendgridApiKey)
            {
                model.MissingApiKey = true;
            }
            else if (ModelState.IsValid && HttpContext.Request.Method.ToUpper() == "POST")
            {
                model.Recipient = sendForm.Recipient ?? "";
                model.sendGridResponse = await CallSendGrid(sendForm.Recipient);
            }
            return View(model);
        }

        // [START gae_flex_sendgrid]
        Task<HttpResponseMessage> CallSendGrid(string recipient)
        {
            // As of 2017-02-09, the sendgrid Nuget package is not compatible
            // with .net core.  So, this code follows the web api spec:
            // https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html
            // If you're using more sendgrid features, consider using swagger
            // to generate a client from:
            // https://github.com/sendgrid/sendgrid-oai
            var request = new
            {
                personalizations = new[] {
                    new {
                        to = new[]
                        {
                            new {email = recipient}
                        },
                        subject = "Hello World!"
                    }
                },
                from = new
                {
                    email = "alice@example.com"
                },
                content = new[]
                {
                    new {
                        type = "text/plain",
                        value = "Hello, World!"
                    }
                }
            };
            HttpClient sendgrid3 = new HttpClient();
            sendgrid3.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", _sendgridApiKey);
            string jsonRequest = JsonConvert.SerializeObject(request);
            return sendgrid3.PostAsync("https://api.sendgrid.com/v3/mail/send",
                new StringContent(jsonRequest, System.Text.Encoding.UTF8,
                "application/json"));
        }
        // [END gae_flex_sendgrid]

        public IActionResult Error()
        {
            return View();
        }
    }
}
