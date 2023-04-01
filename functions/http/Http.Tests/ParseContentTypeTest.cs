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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Http.Tests;

public class ParseContentTypeTest : FunctionTestBase<ParseContentType.Function>
{
    public static TheoryData<HttpContent, string> TestData =
        new TheoryData<HttpContent, string>
        {
            { new StringContent("{\"name\":\"John\"}", Encoding.UTF8, "application/json"), "John" },
            { new StringContent("{\"not-name\":\"foo\"}", Encoding.UTF8, "application/json"), null },
            { new StringContent(Convert.ToBase64String(Encoding.UTF8.GetBytes("Fred")), Encoding.UTF8, "application/octet-stream"), "Fred" },
            { new StringContent("Jane\nBill", Encoding.UTF8, "text/plain"), "Jane" },
            { new FormUrlEncodedContent(new[] { KeyValuePair.Create("name", "Ginger") }), "Ginger" },
            { new FormUrlEncodedContent(new[] { KeyValuePair.Create("not-name", "foo") }), null} ,
            { new StringContent("other-text", Encoding.UTF8, "application/other-content"), null },
        };

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task Process(HttpContent content, string expectedName)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("uri", UriKind.Relative),
            Content = content
        };

        var expectedStatusCode = expectedName is null ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
        var expectedContent = expectedName is null ? "" : $"Hello {expectedName}!";

        await ExecuteHttpRequestAsync(request, async response =>
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedContent, actualContent);
        });
    }
}
