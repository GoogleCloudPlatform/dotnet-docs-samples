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

// [START functions_http_integration_test]
using Google.Cloud.Functions.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HelloHttp.Tests;

public class FunctionIntegrationTest
{
    [Fact]
    public async Task GetRequest_NoParameters()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "uri");
        string text = await ExecuteRequest(request);
        Assert.Equal("Hello world!", text);
    }

    [Fact]
    public async Task GetRequest_UrlParameters()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "uri?name=Cho");
        string text = await ExecuteRequest(request);
        Assert.Equal("Hello Cho!", text);
    }

    [Fact]
    public async Task PostRequest_BodyParameters()
    {
        string json = "{\"name\":\"Julie\"}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "uri")
        {
            Content = new StringContent(json)
        };
        string text = await ExecuteRequest(request);
        Assert.Equal("Hello Julie!", text);
    }

    /// <summary>
    /// Executes the given request in the function in an in-memory test server,
    /// validates that the response status code is 200, and returns the text of the
    /// response body. FunctionTestServer{T} is provided by the
    /// Google.Cloud.Functions.Testing package.
    /// </summary>
    private static async Task<string> ExecuteRequest(HttpRequestMessage request)
    {
        using (var server = new FunctionTestServer<Function>())
        {
            using (HttpClient client = server.CreateClient())
            {
                HttpResponseMessage response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
// [END functions_http_integration_test]
