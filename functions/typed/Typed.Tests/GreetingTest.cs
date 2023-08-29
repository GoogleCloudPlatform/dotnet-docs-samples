// Copyright 2023 Google LLC
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
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Greeting;
using System.Text;

namespace Typed.Tests;

public class GreetingTest : FunctionTestBase<Function>
{
    [Fact]
    public async Task TestRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "example.com");
        request.Content = new StringContent(@"{
            ""first_name"": ""Jane"",
            ""last_name"": ""Doe""
        }", Encoding.UTF8);

        await ExecuteHttpRequestAsync(request, async response =>
        {
            JToken actual = JToken.Parse(await response.Content.ReadAsStringAsync());
            JToken expected = JToken.Parse(@"{
                ""message"": ""Hello Jane Doe!""
            }");
            Assert.True(JToken.DeepEquals(actual, expected), $"JSON response {actual} did not match expected {expected}");
        });
    }
}
