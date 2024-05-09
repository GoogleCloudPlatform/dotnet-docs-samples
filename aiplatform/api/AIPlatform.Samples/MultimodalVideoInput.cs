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

// [START aiplatform_gemini_single_turn_video]

using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using System.Text;
using System.Threading.Tasks;

public class MultimodalVideoInput
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.0-pro-vision"
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
                        new Part { Text = "What's in the video?" },
                        new Part { FileData = new() { MimeType = "video/mp4", FileUri = "gs://cloud-samples-data/video/animals.mp4" }}
                    }
                }
            }
        };

        using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

        StringBuilder fullText = new();

        AsyncResponseStream<GenerateContentResponse> responseStream = response.GetResponseStream();
        await foreach (GenerateContentResponse responseItem in responseStream)
        {
            fullText.Append(responseItem.Candidates[0].Content.Parts[0].Text);
        }
        return fullText.ToString();
    }
}

// [END aiplatform_gemini_single_turn_video]
