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

// [START functions_http_unit_test]
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloHttp.Tests;

public class FunctionUnitTest
{
    [Fact]
    public async Task GetRequest_NoParameters()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        string text = await ExecuteRequest(context);
        Assert.Equal("Hello world!", text);
    }

    [Fact]
    public async Task GetRequest_UrlParameters()
    {
        DefaultHttpContext context = new DefaultHttpContext
        {
            Request = { QueryString = new QueryString("?name=Cho") }
        };
        string text = await ExecuteRequest(context);
        Assert.Equal("Hello Cho!", text);
    }

    [Fact]
    public async Task PostRequest_BodyParameters()
    {
        string json = "{\"name\":\"Julie\"}";
        DefaultHttpContext context = new DefaultHttpContext
        {
            Request =
            {
                Method = HttpMethods.Post,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(json))
            }
        };
        string text = await ExecuteRequest(context);
        Assert.Equal("Hello Julie!", text);
    }

    /// <summary>
    /// Executes the given request in the function, validates that the response
    /// status code is 200, and returns the text of the response body.
    /// </summary>
    private static async Task<string> ExecuteRequest(HttpContext context)
    {
        MemoryStream responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        Function function = new Function(new NullLogger<Function>());
        await function.HandleAsync(context);
        Assert.Equal(200, context.Response.StatusCode);
        context.Response.BodyWriter.Complete();
        context.Response.Body.Position = 0;

        TextReader reader = new StreamReader(responseStream);
        return reader.ReadToEnd();
    }
}
// [END functions_http_unit_test]
