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

// [START generativeaionvertexai_gemini_generate_from_text_input]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TextInputSample
{
    public async Task<string> TextInput(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-preview-0409")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();
        // Does the returned sentiment score match the reviewer's movie rating?
        string prompt = @"Give a score from 1 - 10 to suggest if the following
            movie review is negative or positive (1 is most negative, 10 is
            most positive, 5 will be neutral). Include an explanation.

            The movie takes some time to build, but that is part of its beauty.
            By the time you are hooked, this tale of friendship and hope is
            thrilling and affecting, until the very last scene. You will find
            yourself rooting for the hero every step of the way. This is the
            sharpest, most original animated film I have seen in years. I would
            give it 8 out of 10 stars.";

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
                        new Part { Text = prompt }
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

// [END generativeaionvertexai_gemini_generate_from_text_input]
