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

using Google.Cloud.Functions.Invoker.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Http.Tests
{
    public class CorsTest : FunctionTestBase<Cors.Function>
    {
        [Fact]
        public async Task GetRequest()
        {
            using var client = Server.CreateClient();
            using var response = await client.GetAsync("uri");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin"), "*");
            var actualContent = await response.Content.ReadAsStringAsync();
            var expectedContent = "CORS headers set successfully!";
            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public async Task OptionsRequest()
        {
            using var client = Server.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Options,
                RequestUri = new Uri("uri", UriKind.Relative)
            };
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin"), "*");
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Methods"), "GET");
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Headers"), "Content-Type");
            Assert.Single(response.Headers.GetValues("Access-Control-Max-Age"), "3600");

            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.Empty(actualContent);
        }
    }
}
