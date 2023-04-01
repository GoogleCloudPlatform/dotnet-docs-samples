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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Http.Tests;

public class HttpFormDataTest : FunctionTestBase<HttpFormData.Function>
{
    [Fact]
    public async Task FilesProcessed()
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent("text"), "name1", "text.txt" },
            { new ByteArrayContent(new byte[] { 1, 2, 3 }), "name2", "binary.bin" }
        };
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("uri", UriKind.Relative),
            Content = content
        };
        await ExecuteHttpRequestAsync(request,
            response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));

        var logEntries = GetFunctionLogEntries().ToList();

        Assert.All(logEntries, entry => Assert.Equal(LogLevel.Information, entry.Level));
        Assert.Collection(logEntries,
            entry0 => Assert.Equal("Processed file: text.txt", entry0.Message),
            entry1 => Assert.Equal("Processed file: binary.bin", entry1.Message));
    }

    [Fact]
    public async Task QueryParametersProcessed()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("uri", UriKind.Relative),
            Content = new FormUrlEncodedContent(new[] { KeyValuePair.Create("foo", "bar") })
        };

        await ExecuteHttpRequestAsync(request,
            response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));

        var logEntry = Assert.Single(GetFunctionLogEntries());
        Assert.Equal(LogLevel.Information, logEntry.Level);
        Assert.Equal("Processed field 'foo' (value: 'bar')", logEntry.Message);
    }

    [Fact]
    public async Task NonPostRequestRejected()
    {
        await ExecuteHttpRequestAsync(new HttpRequestMessage(HttpMethod.Get, "uri"),
            response => Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode));
    }
}
