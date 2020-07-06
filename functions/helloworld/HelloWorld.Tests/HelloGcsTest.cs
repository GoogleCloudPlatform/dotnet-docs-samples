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

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public class HelloGcsTest : FunctionTestBase<HelloGcs.Function>
    {
        [Fact]
        public async Task CloudEventInput()
        {
            var client = Server.CreateClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("uri", System.UriKind.Relative),
                // CloudEvent headers
                Headers =
                {
                    { "ce-type", "com.google.cloud.storage.object.finalized.v0" },
                    { "ce-id", "1234" },
                    { "ce-source", "//storage.googleapis.com/" },
                    { "ce-specversion", "1.0" }
                },
                Content = new StringContent("{\"name\":\"file\"}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };
            var response = await client.SendAsync(request);
            // Currently we're just testing that the request was successful.
            // It would be nice to test the log as well.
            // See https://github.com/GoogleCloudPlatform/functions-framework-dotnet/issues/93
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task NotCloudEvent()
        {
            var client = Server.CreateClient();
            var response = await client.GetAsync("uri");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
