// Copyright(c) 2017 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Diagnostics.AspNet;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Trace.Controllers
{
    // [START trace_setup_aspnet_framework_example]
    public class TraceController : Controller
    {
        // This incoming HTTP request should be captured by Trace.
        public ActionResult Index()
        {
            using (CloudTrace.Tracer.StartSpan(nameof(Index)))
            {
                string url = "https://www.googleapis.com/discovery/v1/apis";
                var response = TraceOutgoing(url);
                ViewData["text"] = response.Result.ToString();
                return View();
            }
        }

        public async Task<string> TraceOutgoing(string url)
        {
            // Manually trace a specific operation.
            using (CloudTrace.Tracer.StartSpan("get-api-discovery-doc"))
            {
                using (var httpClient = new HttpClient())
                {
                    // This outgoing HTTP request should be captured by Trace.
                    using (var response = await httpClient.GetAsync(url)
                        .ConfigureAwait(false))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
    // [END trace_setup_aspnet_framework_example]
}