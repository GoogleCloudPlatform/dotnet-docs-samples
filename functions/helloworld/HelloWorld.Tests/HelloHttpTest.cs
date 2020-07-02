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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public class HelloHttpTest : IDisposable
    {
        private readonly FunctionTestServer<HelloHttp.Function> _server;

        public HelloHttpTest() => _server = new FunctionTestServer<HelloHttp.Function>();

        public void Dispose() => _server.Dispose();

        [Fact]
        public async Task EmptyRequest()
        {
            var client = _server.CreateClient();
            var response = await client.GetAsync("uri");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello world!", responseBody);
        }

        [Fact]
        public async Task NameInQuery()
        {
            var client = _server.CreateClient();
            var response = await client.GetAsync("uri?name=test");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello test!", responseBody);
        }

        [Fact]
        public async Task JsonWithNameInBody()
        {
            var client = _server.CreateClient();
            var content = new StringContent("{\"name\":\"test\"}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("uri", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello test!", responseBody);
        }

        [Fact]
        public async Task JsonWithoutNameInBody()
        {
            var client = _server.CreateClient();
            var content = new StringContent("{\"foo\":\"test\"}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("uri", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello world!", responseBody);
        }

        [Fact]
        public async Task InvalidJson()
        {
            // If the JSON is invalid, we log an error but don't fail.
            // TODO: Check that the error is logged. (Maybe this is something we should do in the FunctionTestServer...)
            var client = _server.CreateClient();
            var content = new StringContent("{\"name\":\"test\",invalid}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("uri", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello world!", responseBody);
        }
    }
}
