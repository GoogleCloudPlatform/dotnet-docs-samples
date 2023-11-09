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

// [START aiplatform_sdk_code_chat]

using Google.Cloud.AIPlatform.V1;
using wkt = Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class PredictCodeChatSample
{
    public string PredictCodeChat(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "codechat-bison@001"
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

        var instance = new wkt::Value
        {
            StructValue = new()
            {
                Fields =
                {
                    ["messages"] = wkt::Value.ForList(
                        wkt::Value.ForStruct(new()
                        {
                            Fields =
                            {
                                ["author"] = wkt::Value.ForString("user"),
                                ["content"] = wkt::Value.ForString("Hi, how are you?"),
                            }
                        }),
                        wkt::Value.ForStruct(new()
                        {
                            Fields =
                            {
                                ["author"] = wkt::Value.ForString("system"),
                                ["content"] = wkt::Value.ForString("I am doing good. What can I help you in the coding world?"),
                            }
                        }),
                        wkt::Value.ForStruct(new()
                        {
                            Fields =
                            {
                                ["author"] = wkt::Value.ForString("user"),
                                ["content"] = wkt::Value.ForString("Please help write a function to calculate the min of two numbers."),
                            }
                        }))
                }
            }
        };

        var instances = new List<wkt::Value>
        {
            instance
        };

        var parameters = new wkt::Value
        {
            StructValue = new wkt::Struct
            {
                Fields =
                {
                    { "temperature", new wkt::Value { NumberValue = 0.3 } },
                    { "maxOutputTokens", new wkt::Value { NumberValue = 1024 } }
                }
            }
        };

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse and return the content.
        var content = response.Predictions[0].StructValue.Fields["candidates"].ListValue.Values[0].StructValue.Fields["content"].StringValue;
        Console.WriteLine($"Content: {content}");
        return content;
    }
}

// [END aiplatform_sdk_code_chat]
