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

// [START generativeaionvertexai_gemini_safety_settings]

using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.AIPlatform.V1.SafetySetting.Types;

public class WithSafetySettings
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001"
    )
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();


        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = "Hello!" }
                    }
                }
            },
            SafetySettings =
            {
                new SafetySetting
                {
                    Category = HarmCategory.HateSpeech,
                    Threshold = HarmBlockThreshold.BlockLowAndAbove
                },
                new SafetySetting
                {
                    Category = HarmCategory.DangerousContent,
                    Threshold = HarmBlockThreshold.BlockMediumAndAbove
                }
            }
        };

        using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

        StringBuilder fullText = new();

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

// [END generativeaionvertexai_gemini_safety_settings]
