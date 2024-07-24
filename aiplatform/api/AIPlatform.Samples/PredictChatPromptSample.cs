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
// [START generativeaionvertexai_sdk_chat]

using Google.Cloud.AIPlatform.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Value = Google.Protobuf.WellKnownTypes.Value;

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
        var instance = Value.Parser.ParseJson(instanceJson);

        var instances = new List<Value>
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
        var parameters = Value.Parser.ParseJson(parametersJson);

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse the response and return the content.
        var content = response.Predictions.First().StructValue.Fields["candidates"].ListValue.Values[0].StructValue.Fields["content"].StringValue;
        Console.WriteLine($"Content: {content}");
        return content;
    }
}

// [END aiplatform_sdk_chat]
// [END generativeaionvertexai_sdk_chat]
