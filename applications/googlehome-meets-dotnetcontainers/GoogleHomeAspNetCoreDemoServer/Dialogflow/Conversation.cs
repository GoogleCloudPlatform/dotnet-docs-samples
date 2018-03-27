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
using Google;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow
{
    /// <summary>
    /// Conversation for each user (per sessionId).
    /// </summary>
    public class Conversation
    {
        public ConvState State { get; private set; }
 
        private readonly IExceptionLogger _exceptionLogger;
        private readonly IManagedTracer _tracer;
 
        /// <summary>
        /// Initialize a conversation.
        /// </summary>
        /// <param name="exceptionLogger">Exception logger</param>
        /// <param name="tracer">Tracer</param>
        public Conversation(IExceptionLogger exceptionLogger,IManagedTracer tracer)
        {
            _exceptionLogger = exceptionLogger;
            _tracer = tracer;
            State = new ConvState();
        }

        /// <summary>
        /// Handle a DialogFlow conversation request.
        /// An instance of this class handles just one conversation,
        /// which is tracked external to this class, using the DialogFlow sessionID.
        /// </summary>
        /// <param name="req">Conversation request</param>
        /// <returns>A JSON response which is passed back to DialogFlow</returns>
        public async Task<string> HandleAsync(ConvRequest req)
        {
            // Use reflection to find the handler method for the requested DialogFlow intent
            var handler = FindHandler(req.IntentName);
            if (handler == null)
            {
                return DialogflowApp.Tell($"Sorry, no handler found for intent: {req.IntentName}");
            }

            try
            {
                using (_tracer.StartSpan(req.IntentName))
                {
                    var result = await handler.Handle(req);
                    return result;
                }
            }
            catch (Exception e) when (req.IntentName != "exception.throw")
            {
                _exceptionLogger.Log(e);
                var msg = (e as GoogleApiException)?.Error.Message ?? e.Message;
                return DialogflowApp.Tell($"Sorry, there's a problem: {msg}");
            }
        }

        /// <summary>
        /// Given an Intent name, finds a handler matching the intent name attribute.
        /// </summary>
        /// <param name="intentName">Intent name</param>
        /// <returns>Handler or null, if no intent found</returns>
        private BaseHandler FindHandler(string intentName)
        {
            var baseHandlerTypes = typeof(BaseHandler).Assembly.GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseHandler)));

            var typeList = from baseHandlerType in baseHandlerTypes
                          from attribute in baseHandlerType.GetCustomAttributes(typeof(IntentAttribute), true)
                          where ((IntentAttribute)attribute).Name == intentName
                          select baseHandlerType;

            var type = typeList.FirstOrDefault();
            if (type == null) return null;

            var constructorInfo = type.GetConstructor(new[] { GetType() });
            var instance = (BaseHandler)constructorInfo.Invoke(new object[] { this });
            return instance;
        }
    }
}
