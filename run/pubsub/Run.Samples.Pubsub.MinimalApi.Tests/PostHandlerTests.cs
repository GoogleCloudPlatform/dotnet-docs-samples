// Copyright 2022 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class PostHandlerTests
{
    [Fact]
    public async Task HandleRequet_ValidObject_ReturnsSuccessAccepted()
    {
        var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();
        var jsonPayload = @"{
            ""message"": {
                ""data"":""dGVzdA=="", 
                ""attributes"": {},
                ""messageId"": ""9101075178894"", 
                ""publishTime"":""2017-09-25T23:16:42.302Z""
                }
            }";

        var response = await client.PostAsync("/", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task HandleRequet_EmptyObject_ReturnsBadRequest()
    {
        var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();
        var jsonPayload = "{}";

        var response = await client.PostAsync("/", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleRequet_NoData_ReturnsBadRequest()
    {
        var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();
        var jsonPayload = string.Empty;

        var response = await client.PostAsync("/", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}