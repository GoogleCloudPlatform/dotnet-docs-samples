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

// [START generativeaionvertexai_gemini_system_instruction]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;

public class SystemInstruction
{
    public async Task<string> SetSystemInstruction(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-2.0-flash-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        string prompt = @"User input: I like bagels.
Answer:";

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
                    }
                }
            },
            SystemInstruction = new()
            {
                Parts =
                {
                    new Part { Text = "You are a helpful assistant." },
                    new Part { Text = "Your mission is to translate text in English to French." },
                }
            }
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
}

// [END generativeaionvertexai_gemini_system_instruction]
