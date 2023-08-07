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

// [START functions_typed_googlechatbot]
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleChatBot;

/// <summary>
/// The startup class can be used to perform additional configuration, including
/// adding application configuration sources, reconfiguring logging, providing services
/// for dependency injection, and adding middleware to the eventual application pipeline.
/// In this case, we simply add an IHttpRequestReader and IHttpRequestWriter that will 
/// handle JSON encoded content for a typed function.
/// </summary>
public class Startup : FunctionsStartup
{
    // Provide implementations for IHttpRequestReader and IHttpResponseWriter.
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
        services
                .AddSingleton(typeof(IHttpRequestReader<JObject>), new CustomJsonReader<JObject>())
                .AddSingleton(typeof(IHttpResponseWriter<JObject>), new CustomJsonWriter<JObject>());

}

/// <summary>
/// Implementation of <see cref="ITypedFunction{JObject, JObject}"/> for a Google Chat Bot that uses Newtonsoft.Json to parse requests and serialize responses.
/// </summary>
[FunctionsStartup(typeof(Startup))]
public class TypedFunction : ITypedFunction<JObject, JObject>
{
    public Task<JObject> HandleAsync(JObject request, CancellationToken cancellationToken)
    {
        string displayName = (string) request.SelectToken("$.message.sender.displayName");
        string imageUrl = (string) request.SelectToken("$.message.sender.imageUrl");
        var response = new
        {
            cardsV2 = new[] {
                new {
                    cardId = "avatarCard",
                    card = new
                    {
                        name = "Avatar Card",
                        header = new
                        {
                            title = string.Format("Hello {0}!", displayName),
                        },
                        sections = new[] {
                            new {
                                widgets = new dynamic [] {
                                    new {
                                        textParagraph = new {
                                            text = "Your avatar picture: ",
                                        },
                                    },
                                    new {
                                        image = new {
                                            imageUrl = imageUrl,
                                        }
                                    }
                                }
                            }
                        },
                    }
                }
            }
        };
        return Task.FromResult(JObject.FromObject(response));
    }
}

public class ChatRequest
{
    [JsonPropertyName("message")]
    public Message Message { get; set; }
}

public class Message {
    [JsonPropertyName("sender")]
    public Sender Sender;
}

public class Sender {
    [JsonPropertyName("displayName")]
    public string DisplayName;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl;
}

public class ChatResponse {
    [JsonProperty["cardsV2"]]
    public CardV2 CardV2;
}

class CardV2 {
    [JsonPropertyName("cardId")]
    public string CardID;

    [JsonPropertyName("card")]
    public Card Card;
}

class Card {
    [JsonPropertyName("name")]
    public String Name;

    [JsonPropertyName("header")]
    public Header Header;

    [JsonPropertyName("sections")]
    public string[] Sections;
}

class Header {
    [JsonPropertyName("title")]
    public string Title;
}

class Section {
    [JsonPropertyName("widgets")]
    public Object Widgets; 
}

class TextWidget {
    [JsonPropertyName("textParagraph")]
    public string TextParagraph;
}

class ImageWidget {
    [JsonPropertyName("imageWidget")]
    public string TextParagraph;
}
