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
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow
{
    /// <summary>
    /// Class that encapsulates a Dialogflow conversation request received from the client.
    /// </summary>
    public class ConvRequest
    {
        // A Protobuf JSON parser configured to ignore unknown fields. This makes
        // the action robust against new fields being introduced by Dialogflow.
        private static readonly JsonParser jsonParser =
            new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        /// <summary>
        /// Parses an HTTP request into a Conversation request that Dialogflow expects.
        /// </summary>
        /// <param name="httpRequest">HTTP request to parse</param>
        /// <returns>A conversation request</returns>
        public static async Task<ConvRequest> ParseAsync(HttpRequest httpRequest)
        {
            WebhookRequest req;

            using (var reader = new StreamReader(httpRequest.Body))
            {
                req = jsonParser.Parse<WebhookRequest>(reader);
            }

            var param = req.QueryResult.Parameters.Fields.ToDictionary(
                x => x.Key.ToString().ToLowerInvariant(),
                x =>
                {
                    return x.Value.ToString().ToLowerInvariant().Trim('"');
                });
            return new ConvRequest(req.Session, req.QueryResult.QueryText, req.QueryResult.Intent.DisplayName, param);
        }

        private ConvRequest(string sessionId, string queryText, string intentName, IReadOnlyDictionary<string, string> parameters)
        {
            SessionId = sessionId;
            QueryText = queryText;
            IntentName = intentName;
            Parameters = parameters;
        }

        /// <summary>
        /// DialogFlow session ID. Tracks a single conversation.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// The raw query text.
        /// </summary>
        public string QueryText { get; }

        /// <summary>
        /// The intent name from DialogFlow
        /// </summary>
        public string IntentName { get; }

        /// <summary>
        /// Parsed parameters from DialogFlow. Defined in the DialogFlow configuration for each intent
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; }

        private static async Task<string> ConvertToString(HttpRequest httpRequest)
        {
            var bodyStream = new MemoryStream();
            await httpRequest.Body.CopyToAsync(bodyStream);
            return Encoding.UTF8.GetString(bodyStream.ToArray());
        }
    }
}
