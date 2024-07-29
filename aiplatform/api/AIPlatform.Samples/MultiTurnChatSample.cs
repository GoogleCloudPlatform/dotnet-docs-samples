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

// [START generativeaionvertexai_gemini_multiturn_chat]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MultiTurnChatSample
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001"
    )
    {
        // Create a chat session to keep track of the context
        ChatSession chatSession = new ChatSession($"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}", location);

        string prompt = "Hello.";
        Console.WriteLine($"\nUser: {prompt}");

        string response = await chatSession.SendMessageAsync(prompt);
        Console.WriteLine($"Response: {response}");

        prompt = "What are all the colors in a rainbow?";
        Console.WriteLine($"\nUser: {prompt}");

        response = await chatSession.SendMessageAsync(prompt);
        Console.WriteLine($"Response: {response}");

        prompt = "Why does it appear when it rains?";
        Console.WriteLine($"\nUser: {prompt}");

        response = await chatSession.SendMessageAsync(prompt);
        Console.WriteLine($"Response: {response}");

        return response;
    }

    private class ChatSession
    {
        private readonly string _modelPath;
        private readonly PredictionServiceClient _predictionServiceClient;

        private readonly List<Content> _contents;

        public ChatSession(string modelPath, string location)
        {
            _modelPath = modelPath;

            _predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com"
            }.Build();

            // Initialize contents to send over in every request.
            _contents = new List<Content>();
        }

        public async Task<string> SendMessageAsync(string prompt)
        {
            var content = new Content
            {
                Role = "USER",
                Parts =
                {
                    new Part { Text = prompt }
                }
            };
            _contents.Add(content);

            var generateContentRequest = new GenerateContentRequest
            {
                Model = _modelPath,
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0.9f,
                    TopP = 1,
                    TopK = 32,
                    CandidateCount = 1,
                    MaxOutputTokens = 2048
                }
            };
            generateContentRequest.Contents.AddRange(_contents);

            GenerateContentResponse response = await _predictionServiceClient.GenerateContentAsync(generateContentRequest);

            _contents.Add(response.Candidates[0].Content);

            return response.Candidates[0].Content.Parts[0].Text;
        }
    }
}

// [END generativeaionvertexai_gemini_multiturn_chat]
