/*
 * Copyright 2024 Google LLC
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

// [START generativeaionvertexai_gemini_grounding_with_vais]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;

public class GroundingVertexAiSearchSample
{
    public async Task<string> GenerateTextWithVertexAiSearch(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001",
        string dataStoreLocation = "global",
        string dataStoreId = "your-datastore-id")
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.0f
            },
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts = { new Part { Text = "How do I make an appointment to renew my driver's license?" } }
                }
            },
            Tools =
            {
                new Tool
                {
                    Retrieval = new Retrieval
                    {
                        VertexAiSearch = new VertexAISearch
                        {
                            Datastore = $"projects/{projectId}/locations/{dataStoreLocation}/collections/default_collection/dataStores/{dataStoreId}"
                        }
                    }
                }
            }
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
}

// [END generativeaionvertexai_gemini_grounding_with_vais]
