/*
 * Copyright (c) 2015 Google Inc.
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

/*
  To test in PowerShell:
    Invoke-RestMethod
      -Uri http://localhost:5000
      -Method POST
      -ContentType "application/json"
      -Body '{"subscription":"SubscriptionName","message":{"message_id":"1234","data":"SGVsbG8gd29ybGQ=","attributes":{"attr2":"value1","attr2":"value2"}}}'
 */

using Microsoft.AspNet.Mvc;
using System.Collections.Generic;

namespace GoogleCloudSamples.PushMessageEndpointSample
{
    public class Message
    {
        public string Data;
        public string Message_Id;
        public IDictionary<string, string> Attributes;

        public string Text
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString(
                  System.Convert.FromBase64String(Data)
                );
            }
        }
    }

    public class PushMessage
    {
        public Message Message;
        public string Subscription;
    }

    [Route("/")]
    public class ReceivePubsubMessageController : Controller
    {
        [HttpPost]
        public string ReceiveMessage([FromBody] PushMessage pushMessage)
        {
            System.Console.WriteLine("HI THERE");
            System.Diagnostics.Trace.WriteLine("HI THERE - TRACE");

            // 200x
            HttpContext.Response.StatusCode = 200;

            if (pushMessage == null)
                return "NULL PUSH MESSAGE!!";

            var message = pushMessage.Message;

            var output = $"Received Message Id: {message.Message_Id}\n" +
                         $"Text: {message.Text}\n" +
                         $"From Subscription: {pushMessage.Subscription}\n";

            foreach (var attributeName in message.Attributes.Keys)
                output += $"Attribute {attributeName} = {message.Attributes[attributeName]}\n";

            return output;
        }
    }
}