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

// [START generativeaionvertexai_gemini_single_turn_multi_image]

using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class MultimodalMultiImage
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

        ByteString colosseum = await ReadImageFileAsync(
            "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark1.png");

        ByteString forbiddenCity = await ReadImageFileAsync(
            "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark2.png");

        ByteString christRedeemer = await ReadImageFileAsync(
            "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark3.png");

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
                        new Part { InlineData = new() { MimeType = "image/png", Data = colosseum }},
                        new Part { Text = "city: Rome, Landmark: the Colosseum" },
                        new Part { InlineData = new() { MimeType = "image/png", Data = forbiddenCity }},
                        new Part { Text = "city: Beijing, Landmark: Forbidden City"},
                        new Part { InlineData = new() { MimeType = "image/png", Data = christRedeemer }}
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

    private static async Task<ByteString> ReadImageFileAsync(string url)
    {
        using HttpClient client = new();
        using var response = await client.GetAsync(url);
        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
        return ByteString.CopyFrom(imageBytes);
    }
}

// [END generativeaionvertexai_gemini_single_turn_multi_image]
