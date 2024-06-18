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

// [START generativeaionvertexai_gemini_token_count]
// [START aiplatform_gemini_token_count]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;

public class GetTokenCount
{
    public async Task<int> CountTokens(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001"
    )
    {
        var client = new LlmUtilityServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var request = new CountTokensRequest
        {
            Endpoint = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts = { new Part { Text = "Why is the sky blue?" } }
                }
            }
        };

        var response = await client.CountTokensAsync(request);
        int tokenCount = response.TotalTokens;
        Console.WriteLine($"There are {tokenCount} tokens in the prompt.");
        return tokenCount;
    }
}

// [END aiplatform_gemini_token_count]
// [END generativeaionvertexai_gemini_token_count]