// Copyright(c) 2018 Google Inc.
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
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoogleHomeAspNetCoreDemoServer.Controllers
{
    /// <summary>
    /// Controller for the main page and also for long poll requests.
    /// </summary>
    public class HomeController : Controller
    {
        // A dictionary of client buffer queues keyed by client ids. 
        private static Dictionary<object, BufferBlock<string>> clientBufferQueues = new Dictionary<object, BufferBlock<string>>();

        private readonly IExceptionLogger _exceptionLogger;
        private ILogger<HomeController> _logger;

        /// <summary>
        /// Constructor for home controller.
        /// </summary>
        /// <param name="exceptionLogger">Exception logger</param>
        /// <param name="logger">Regular logger</param>
        public HomeController(
            IExceptionLogger exceptionLogger,
            ILogger<HomeController> logger)
        {
            _exceptionLogger = exceptionLogger;
            _logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("LongPoll")]
        public async Task<IActionResult> LongPoll()
        {
            var clientId = Request.QueryString.Value;
            var queue = GetOrCreateClientBufferQueue(clientId);

            var page = string.Empty;
            try
            {
                page = await queue.ReceiveAsync(TimeSpan.FromSeconds(15));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _exceptionLogger.Log(e);
            }

            return Content(page, "text/plain");
        }

        [Route("Test")]
        public IActionResult Test()
        {
            return Content("Hello World!", "text/plain");
        }


        /// <summary>
        /// Flushes the passed in HTML page to the client.
        /// </summary>
        /// <param name="page">Page to flush</param>
        public static void SetPage(string page)
        {
            lock (clientBufferQueues)
            {
                var toRemove = new List<object>();
                foreach (var buffer in clientBufferQueues)
                {
                    buffer.Value.Post(page);
                    if (buffer.Value.Count > 20)
                    {
                        toRemove.Add(buffer.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    clientBufferQueues.Remove(id);
                }
            }
        }

        /// <summary>
        /// Given a client id, either returns the buffer queue for that client or creates
        /// a new queue, if it doesn't exist yet.
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <returns>Client's buffer queue</returns>
        private static BufferBlock<string> GetOrCreateClientBufferQueue(string clientId)
        {
            BufferBlock<string> queue;
            lock (clientBufferQueues)
            {
                if (!clientBufferQueues.TryGetValue(clientId, out queue))
                {
                    queue = new BufferBlock<string>();
                    clientBufferQueues.Add(clientId, queue);
                }
            }

            return queue;
        }
    }
}