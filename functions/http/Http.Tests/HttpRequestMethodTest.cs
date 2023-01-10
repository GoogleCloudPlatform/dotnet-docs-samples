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
using Google.Cloud.Functions.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Http.Tests;

public class HttpRequestMethodTest : FunctionTestBase<HttpRequestMethod.Function>
{
    [Theory]
    [InlineData("GET", HttpStatusCode.OK, "Hello world!")]
    [InlineData("PUT", HttpStatusCode.Forbidden, "Forbidden!")]
    [InlineData("DELETE", HttpStatusCode.MethodNotAllowed, "Something blew up!")]
    public async Task ExecuteFunction(string method, HttpStatusCode expectedStatus, string expectedContent)
    {
        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = new Uri("uri", UriKind.Relative)
        };

        await ExecuteHttpRequestAsync(request, async response =>
        {
            Assert.Equal(expectedStatus, response.StatusCode);
            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedContent, actualContent);
        });
    }
}
