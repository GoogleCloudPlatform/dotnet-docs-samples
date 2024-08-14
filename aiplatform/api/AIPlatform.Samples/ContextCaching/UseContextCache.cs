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

// [START generativeaionvertexai_gemini_use_context_cache]

using Google.Cloud.AIPlatform.V1Beta1;
using System;
using System.Threading.Tasks;

public class UseContextCache
{
    public async Task<string> Use(string projectId, CachedContentName name)
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"us-central1-aiplatform.googleapis.com"
        }.Build();

        var generateContentRequest = new GenerateContentRequest
        {
            CachedContentAsCachedContentName = name,
            Model = $"projects/{projectId}/locations/us-central1/publishers/google/models/gemini-1.5-pro-001",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = "What are the papers about?" },
                    }
                }
            }
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine($"Response: {responseText}");

        return responseText;
    }
}

// [END generativeaionvertexai_gemini_use_context_cache]
