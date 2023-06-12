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

using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using Pubsub.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Pubsub.Controllers
{
    public class HomeController : Controller
    {
        readonly PubsubOptions _options;
        // Keep the messages received on /Push endpoint.
        static ConcurrentBag<string> s_receivedMessages = new ConcurrentBag<string>();
        // Keep the messages received on /AuthPush endpoint.
        static ConcurrentBag<string> s_authenticatedMessages = new ConcurrentBag<string>();
        readonly PublisherClient _publisher;

        public HomeController(IOptions<PubsubOptions> options,
            PublisherClient publisher)
        {
            _options = options.Value;
            _publisher = publisher;
        }

        // [START gae_flex_pubsub_index]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IndexAsync(MessageForm messageForm)
        {
            var model = new MessageList();
            if (!_options.HasGoodProjectId())
            {
                model.MissingProjectId = true;
                return View(model);
            }
            if (!string.IsNullOrEmpty(messageForm.Message))
            {
                // Publish the message.
                var pubsubMessage = new PubsubMessage()
                {
                    Data = ByteString.CopyFromUtf8(messageForm.Message)
                };
                pubsubMessage.Attributes["token"] = _options.VerificationToken;
                await _publisher.PublishAsync(pubsubMessage);
                model.PublishedMessage = messageForm.Message;
            }
            // Render the current list of messages.
            model.Messages = s_receivedMessages.ToArray();
            model.AuthMessages = s_authenticatedMessages.ToArray();
            return View(model);
        }
        // [END gae_flex_pubsub_index]

        // [START gae_flex_pubsub_push]
        /// <summary>
        /// Handle a push request coming from pubsub.
        /// </summary>
        [HttpPost]
        [Route("/Push")]
        public async Task<IActionResult> PushAsync([FromBody]PushBody body,
            [FromQuery]string token)
        {
            string verificationToken =
                token ?? body.message.attributes["token"];
            if (verificationToken != _options.VerificationToken)
            {
                return new BadRequestResult();
            }
            var messageBytes = Convert.FromBase64String(body.message.data);
            string message = System.Text.Encoding.UTF8.GetString(messageBytes);
            s_receivedMessages.Add(message);
            return new OkResult();
        }
        // [END gae_flex_pubsub_push]

        // [START gaeflex_net_pubsub_auth_push]
        /// <summary>
        /// Extended JWT payload to match the pubsub payload format.
        /// </summary>
        public class PubSubPayload : JsonWebSignature.Payload
        {
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("email_verified")]
            public string EmailVerified { get; set; }
        }
        /// <summary>
        /// Handle authenticated push request coming from pubsub.
        /// </summary>
        [HttpPost]
        [Route("/AuthPush")]
        public async Task<IActionResult> AuthPushAsync([FromBody] PushBody body, [FromQuery] string token)
        {
            // Get the Cloud Pub/Sub-generated "Authorization" header.
            string authorizaionHeader = HttpContext.Request.Headers["Authorization"];
            string verificationToken = token ?? body.message.attributes["token"];
            // JWT token comes in `Bearer <JWT>` format substring 7 specifies the position of first JWT char.
            string authToken = authorizaionHeader.StartsWith("Bearer ") ? authorizaionHeader.Substring(7) : null;
            if (verificationToken != _options.VerificationToken || authToken is null)
            {
                return new BadRequestResult();
            }
            // Verify and decode the JWT.
            // Note: For high volume push requests, it would save some network
            // overhead if you verify the tokens offline by decoding them using
            // Google's Public Cert; caching already seen tokens works best when
            // a large volume of messages have prompted a single push server to
            // handle them, in which case they would all share the same token for
            // a limited time window.
            var payload = await JsonWebSignature.VerifySignedTokenAsync<PubSubPayload>(authToken);

            // IMPORTANT: you should validate payload details not covered
            // by signature and audience verification above, including:
            //   - Ensure that `payload.Email` is equal to the expected service
            //     account set up in the push subscription settings.
            //   - Ensure that `payload.Email_verified` is set to true.

            var messageBytes = Convert.FromBase64String(body.message.data);
            string message = System.Text.Encoding.UTF8.GetString(messageBytes);
            s_authenticatedMessages.Add(message);
            return new OkResult();
        }
        // [END gaeflex_net_pubsub_auth_push]

        public IActionResult Error()
        {
            return View();
        }
    }

    /// <summary>
    /// Pubsub messages will arrive in this format.
    /// </summary>
    public class PushBody
    {
        public PushMessage message { get; set; }
        public string subscription { get; set; }
    }

    public class PushMessage
    {
        public Dictionary<string, string> attributes { get; set; }
        public string data { get; set; }
        public string message_id { get; set; }
        public string publish_time { get; set; }
    }
    

}
