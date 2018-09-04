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
using System;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents
{
    /// <summary>
    /// Handler for all "exception.throw" DialogFlow intent.
    /// </summary>
    [Intent("exception.throw")]
    public class ExceptionThrowHandler : BaseHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation"></param>
        public ExceptionThrowHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override WebhookResponse Handle(WebhookRequest req)
        {
            var exception = req.QueryResult.Parameters.Fields["exception"].StringValue;

            // Throw the requested exception type which will be picked up by Error-Reporting
            switch (exception.ToLowerInvariant())
            {
                case null: return new WebhookResponse { FulfillmentText = "Oops, not sure what to throw." };
                case "exception": throw new Exception();
                case "argumentexception": throw new ArgumentException();
                case "invalidoperationexception": throw new InvalidOperationException();
                case "notsupportedexception": throw new NotSupportedException();
                case "notimplementedexception": throw new NotImplementedException();
                case "argumentoutofrangeexception": throw new ArgumentOutOfRangeException();
                case "argumentnullexception": throw new ArgumentNullException();
                case "nullreferenceexception": throw new NullReferenceException();
                case "timeoutexception": throw new TimeoutException();
                case "stackoverflowexception": throw new StackOverflowException();
                case "formatexception": throw new FormatException();
                default: return new WebhookResponse { FulfillmentText = $"Sorry, I don't know the exception type: '{exception}'" };
            }
        }
    }
}
