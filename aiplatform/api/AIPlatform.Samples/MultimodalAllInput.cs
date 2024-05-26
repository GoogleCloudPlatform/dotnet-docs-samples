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

// [START generativeaionvertexai_gemini_all_modalities]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;

public class MultimodalAllInput
{
    public async Task<string> AnswerFromMultimodalInput(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        string prompt = "Watch each frame in the video carefully and answer the questions.\n"
                  + "Only base your answers strictly on what information is available in "
                  + "the video attached. Do not make up any information that is not part "
                  + "of the video and do not be too verbose, be to the point.\n\n"
                  + "Questions:\n"
                  + "- When is the moment in the image happening in the video? "
                  + "Provide a timestamp.\n"
                  + "- What is the context of the moment and what does the narrator say about it?";

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
                        new Part { Text = prompt },
                        new Part { FileData = new() { MimeType = "video/mp4", FileUri = "gs://cloud-samples-data/generative-ai/video/behind_the_scenes_pixel.mp4" } },
                        new Part { FileData = new() { MimeType = "image/png", FileUri = "gs://cloud-samples-data/generative-ai/image/a-man-and-a-dog.png" } }
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

// [END generativeaionvertexai_gemini_all_modalities]
