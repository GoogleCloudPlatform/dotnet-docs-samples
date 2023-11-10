/*
 * Copyright 2023 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START aiplatform_sdk_chat]

using Google.Cloud.AIPlatform.V1;
using wkt = Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class PredictChatPromptSample
{
    public string PredictChatPrompt(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "chat-bison@001"
    )
    {
        // Initialize client that will be used to send requests.
        // This client only needs to be created once,
        // and can be reused for multiple requests.
        var client = new PredictionServiceClientBuilder
        {
            Endpoint = $"{locationId}-aiplatform.googleapis.com"
        }.Build();

        // Configure the parent resource.
        var endpoint = EndpointName.FromProjectLocationPublisherModel(projectId, locationId, publisher, model);

        // Initialize request argument(s).
        var prompt = "How many planets are there in the solar system?";

        // You can construct Protobuf from JSON.
        var instanceJson = JsonConvert.SerializeObject(new
        {
            context = "My name is Miles. You are an astronomer, knowledgeable about the solar system.",
            examples = new[]
            {
                new
                {
                    input = new { content = "How many moons does Mars have?" },
                    output = new { content = "The planet Mars has two moons, Phobos and Deimos." }
                }
            },
            messages = new[]
            {
                new
                {
                    author = "user",
                    content = prompt
                }
            }
        });
        // var instance = wkt::Value.Parser.ParseJson(instanceJson);

        // Or, you can construct Protobuf directly.
        var instance = new wkt::Value
        {
            StructValue = new()
            {
                Fields =
                {
                    ["context"] = wkt::Value.ForString("My name is Miles. You are an astronomer, knowledgeable about the solar system."),
                    ["examples"] = wkt::Value.ForList(wkt::Value.ForStruct(new()
                    {
                        Fields =
                        {
                            ["input"] = wkt::Value.ForStruct(new()
                            {
                                Fields =
                                {
                                    ["content"] = wkt::Value.ForString("How many moons does Mars have?")
                                }
                            }),
                            ["output"] = wkt::Value.ForStruct(new()
                            {
                                Fields =
                                {
                                    ["content"] = wkt::Value.ForString("The planet Mars has two moons, Phobos and Deimos.")
                                }
                            })
                        }
                    })),
                    ["messages"] = wkt::Value.ForList(wkt::Value.ForStruct(new()
                    {
                        Fields =
                        {
                            ["author"] = wkt::Value.ForString("user"),
                            ["content"] = wkt::Value.ForString(prompt),
                        }
                    }))
                }
            }
        };

        var instances = new List<wkt::Value>
        {
            instance
        };

        // You can construct Protobuf from JSON.
        var parametersJson = JsonConvert.SerializeObject(new
        {
            temperature = 0.3,
            maxDecodeSteps = 200,
            topP = 0.8,
            topK = 40
        });
        // var parameters = wkt::Value.Parser.ParseJson(parametersJson);

        // Or, you can construct Protobuf directly.
        var parameters = new wkt::Value
        {
            StructValue = new wkt::Struct
            {
                Fields =
                {
                    { "temperature", new wkt::Value { NumberValue = 0.3 } },
                    { "maxDecodeSteps", new wkt::Value { NumberValue = 200 } },
                    { "topP", new wkt::Value { NumberValue = 0.8 } },
                    { "topK", new wkt::Value { NumberValue = 40 } }
                }
            }
        };

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse the response and return the content.
        var content = response.Predictions[0].StructValue.Fields["candidates"].ListValue.Values[0].StructValue.Fields["content"].StringValue;
        Console.WriteLine($"Content: {content}");
        return content;
    }
}

// [END aiplatform_sdk_chat]
