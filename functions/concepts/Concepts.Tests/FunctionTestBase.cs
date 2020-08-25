// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CloudNative.CloudEvents;
using Google.Cloud.Functions.Invoker.Testing;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace Concepts.Tests
{
    /// <summary>
    /// Simple base class for Function tests.
    /// </summary>
    public abstract class FunctionTestBase<TFunction> : IDisposable
    {
        protected FunctionTestServer<TFunction> Server { get; }

        public FunctionTestBase() => Server = new FunctionTestServer<TFunction>();

        public void Dispose() => Server.Dispose();

        /// <summary>
        /// Convenience method to retrieve all the log entries with a category
        /// specified by the function's type name.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<TestLogEntry> GetFunctionLogEntries() =>
            Server.GetLogEntries(typeof(TFunction));

        /// <summary>
        /// Executes a simple HttpFunction, which is assumed to not care about
        /// the URI or body. The response is returned as text.
        /// </summary>
        protected async Task<string> ExecuteHttpFunctionAsync()
        {
            using (var client = Server.CreateClient())
            {
                var response = await client.GetAsync("uri");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Calls the function with the given CloudEvent type and data. Note
        /// that the source of the event will not correspond to the "real" source,
        /// so this can't be used in situations where the function is source-sensitive.
        /// </summary>
        protected Task ExecuteCloudEventFunctionAsync(string cloudEventType, IMessage data)
        {
            var cloudEvent = new CloudEvent(cloudEventType, new Uri("//source.googleapis.com"), "1234")
            {
                Data = data
            };
            return ExecuteCloudEventFunctionAsync(cloudEvent);
        }

        /// <summary>
        /// Calls the function with the given CloudEvent. Note
        /// that the source of the event will not correspond to the "real" source,
        /// so this can't be used in situations where the function is source-sensitive.
        /// </summary>
        protected async Task ExecuteCloudEventFunctionAsync(CloudEvent cloudEvent)
        {
            using (var client = Server.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("uri", UriKind.Relative),
                    // CloudEvent headers
                    Headers =
                    {
                        { "ce-type", cloudEvent.Type },
                        { "ce-id", cloudEvent.Id },
                        { "ce-source", cloudEvent.Source.ToString() },
                        { "ce-datacontenttype", cloudEvent.DataContentType?.ToString() ?? "application/json" },
                        { "ce-specversion", "1.0" }
                    },
                    Content = new StringContent(cloudEvent.Data.ToString()),
                    Method = HttpMethod.Post
                };
                if (cloudEvent.Time is DateTime dt)
                {
                    request.Headers.Add("ce-time", dt.ToString("o"));
                }
                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
