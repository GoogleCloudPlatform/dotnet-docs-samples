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
using Google.Cloud.Diagnostics.Common;
using GoogleHomeAspNetCoreDemoServer.Dialogflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Controllers
{
    /// <summary>
    /// The main controller that handles the conversation requests from Dialogflow.
    /// </summary>
    public class ConversationController : Controller
    {
        private readonly DialogflowApp _dialogFlowApp;
  
        /// <summary>
        /// Consructor for Conversation controller.
        /// </summary>
        /// <param name="exceptionLogger">Exception logger</param>
        /// <param name="logger">Regular logger</param>
        /// <param name="tracer">Tracer</param>
        public ConversationController(
            IExceptionLogger exceptionLogger,
            ILogger<ConversationController> logger,
            IManagedTracer tracer)
        {
            _dialogFlowApp = new DialogflowApp(exceptionLogger, logger, tracer);
        }

        [Route("Conversation")]
        public async Task<IActionResult> Conversation()
        {
            var response = await _dialogFlowApp.HandleRequest(Request);
            var responseJson = response.ToString();
            return Content(responseJson, "application/json; charset=utf-8");
        }
    }
}