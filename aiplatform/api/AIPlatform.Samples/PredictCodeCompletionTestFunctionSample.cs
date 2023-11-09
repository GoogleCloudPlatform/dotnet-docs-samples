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

// [START aiplatform_sdk_code_completion_test_function]

using Google.Cloud.AIPlatform.V1;
using wkt = Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class PredictCodeCompletionTestFunctionSample
{
    public string PredictTestFunction(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "code-gecko@001")
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

        // Learn how to create prompts to work with a code model to create code completion suggestions:
        // https://cloud.google.com/vertex-ai/docs/generative-ai/code/code-completion-prompts
        var prefix = @"
def reverse_string(s):
    return s[::-1]
def test_empty_input_string()";

        var instances = new List<wkt::Value>
        {
            wkt::Value.ForStruct(new()
            {
                Fields =
                {
                    ["prefix"] = wkt::Value.ForString(prefix),
                }
            })
        };

        var parameters = new wkt::Value
        {
            StructValue = new wkt::Struct
            {
                Fields =
                {
                    { "temperature", new wkt::Value { NumberValue = 0.2 } },
                    { "maxOutputTokens", new wkt::Value { NumberValue = 64 } }
                }
            }
        };

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse and return the content.
        var content = response.Predictions[0].StructValue.Fields["content"].StringValue;
        return content;
    }
}

// [END aiplatform_sdk_code_completion_test_function]
