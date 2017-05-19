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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Diagnostics.Common;
using System.Threading;
using System.Net.Http;

namespace Stackdriver.Controllers
{
    // [START trace_example]
    public class TraceController : Controller
    {
        // This incoming HTTP request should be captured by Trace.
        public IActionResult Index([FromServices] IManagedTracer tracer)
        {
            using (tracer.StartSpan(nameof(Index)))
            {
                var traceHeaderHandler = new TraceHeaderPropagatingHandler(() => tracer);
                var response = TraceOutgoing(traceHeaderHandler);
                ViewData["text"] = response.Result.ToString();
                return View();
            }
        }

        // This outgoing HTTP request should be captured by Trace.
        public async Task<string> TraceOutgoing(TraceHeaderPropagatingHandler traceHeaderHandler)
        {
            // Add a handler to trace outgoing requests and to propagate the trace header.
            using (var httpClient = new HttpClient(traceHeaderHandler))
            {
                string url = "https://www.googleapis.com/discovery/v1/apis";
                using (var response = await httpClient.GetAsync(url))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
    // [END trace_example]

}
