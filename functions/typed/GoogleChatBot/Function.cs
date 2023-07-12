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
/// In this case, we simply add an IHttpRequestReader and IHttpRequestWriter that will handle JSON encoded content for a typed function.
/// </summary>
public class Startup : FunctionsStartup
{
    // Provide implementations for IOperationSingleton, and IOperationScoped.
    // The implementation is the same for both interfaces (the Operation class)
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

        return Task.FromResult(JObject.FromObject(new
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
        }));
    }
}

/// <summary>
/// Implementation of <see cref="IHttpRequestReader{TRequest}"/> using Newtonsoft.Json
/// </summary>
class CustomJsonReader<TRequest> : IHttpRequestReader<TRequest>
{
    public async Task<TRequest> ReadRequestAsync(HttpRequest httpRequest)
    {
        var bodyStream = new StreamReader(httpRequest.Body);
        var bodyText = await bodyStream.ReadToEndAsync();
        return JsonConvert.DeserializeObject<TRequest>(bodyText);
    }
}

/// <summary>
/// Implementation of <see cref="IHttpResponseWriter{TResponse}"/> using Newtonsoft.Json
/// </summary>
class CustomJsonWriter<TResponse> : IHttpResponseWriter<TResponse>
{
    public async Task WriteResponseAsync(HttpResponse httpResponse, TResponse functionResponse)
    {
        await httpResponse.WriteAsync(JsonConvert.SerializeObject(functionResponse));
    }
}
// [END functions_typed_googlechatbot]
