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

// [START aiplatform_gemini_safety_settings]

using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.AIPlatform.V1.SafetySetting.Types;

public class WithSafetySettings
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-pro-vision"
    )
    {
        // Create client
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();


        // Prompt
        string prompt = "Hello!";

        // Initialize request argument(s)
        var content = new Content
        {
            Role = "USER"
        };
        content.Parts.AddRange(new List<Part>()
        {
            new()
            {
                Text = prompt
            }
        });

        var safetySettings = new List<SafetySetting>()
        {
            new()
            {
                Category = HarmCategory.HateSpeech,
                Threshold = HarmBlockThreshold.BlockLowAndAbove
            },
            new()
            {
                Category = HarmCategory.DangerousContent,
                Threshold = HarmBlockThreshold.BlockMediumAndAbove
            }
        };

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.4f,
                TopP = 1,
                TopK = 32,
                MaxOutputTokens = 2048
            },
        };
        generateContentRequest.Contents.Add(content);
        generateContentRequest.SafetySettings.AddRange(safetySettings);

        // Make the request, returning a streaming response
        using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

        StringBuilder fullText = new();

        // Read streaming responses from server until complete
        AsyncResponseStream<GenerateContentResponse> responseStream = response.GetResponseStream();
        await foreach (GenerateContentResponse responseItem in responseStream)
        {
            // Check if the content has been blocked for safety reasons.
            bool blockForSafetyReason = responseItem.Candidates[0].FinishReason == Candidate.Types.FinishReason.Safety;
            if (blockForSafetyReason)
            {
                fullText.Append("Blocked for safety reasons");
            }
            else
            {
                fullText.Append(responseItem.Candidates[0].Content.Parts[0].Text);
            }
        }

        return fullText.ToString();
    }
}

// [END aiplatform_gemini_safety_settings]
