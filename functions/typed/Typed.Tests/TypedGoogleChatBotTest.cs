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
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using GoogleChatBot;
using System.Net;
using System.Text;

namespace Typed.Tests;

public class TypedGoogleChatBotTest : FunctionTestBase<TypedFunction>
{
    private readonly ITestOutputHelper output;

    public TypedGoogleChatBotTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async Task ChatBotTest()
    {
        JToken expected = JToken.Parse(@"{
            cardsV2: [
                {
                cardId: ""avatarCard"",
                    card: {
                        name: ""Avatar Card"",
                        header: {
                            title: ""Hello janedoe!"",
                        },
                        sections: [
                            {
                                widgets: [
                                    {
                                        textParagraph: {
                                            text: ""Your avatar picture: "",
                                        },
                                    },
                                    {
                                        image: {
                                            imageUrl: ""example.com/avatar.png"",
                                        },
                                    },
                                ],
                            },
                        ],
                    },
                },
            ],
        }");

        var request = new HttpRequestMessage(HttpMethod.Post, "example.com");
        request.Content = new StringContent(@"{
            ""message"": {
                ""sender"": {
                    ""displayName"": ""janedoe"",
                    ""imageUrl"": ""example.com/avatar.png""
                }
            }
        }", Encoding.UTF8);

        await ExecuteHttpRequestAsync(request, async response =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JToken actual = JToken.Parse(await response.Content.ReadAsStringAsync());
            Assert.True(JToken.DeepEquals(actual, expected), string.Format("JSON response {0} did not match expected {1}", actual, expected));
        });
    }
}