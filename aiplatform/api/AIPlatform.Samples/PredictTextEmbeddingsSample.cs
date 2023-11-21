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

// [START aiplatform_sdk_embedding]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using Value = Google.Protobuf.WellKnownTypes.Value;

public class PredictTextEmbeddingsSample
{
    public int PredictTextEmbeddings(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "textembedding-gecko@001"
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
        var instances = new List<Value>
        {
            Value.ForStruct(new()
            {
                Fields =
                {
                    ["content"] = Value.ForString("What is life?"),
                }
            })
        };

        // Make the request.
        var response = client.Predict(endpoint, instances, null);

        // Parse and return the embedding vector count.
        var values = response.Predictions.First().StructValue.Fields["embeddings"].StructValue.Fields["values"].ListValue.Values;
        Console.WriteLine($"Length of embedding vector: {values.Count}");
        return values.Count;
    }
}

// [END aiplatform_sdk_embedding]
