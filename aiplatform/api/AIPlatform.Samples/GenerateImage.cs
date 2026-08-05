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

// [START generativeaionvertexai_imagen_generate_image]

using Google.Cloud.AIPlatform.V1;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class GenerateImage
{
    public async Task<FileInfo> Generate(
        string projectId = "your-project-id")
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = "us-central1-aiplatform.googleapis.com"
        }.Build();

        string prompt = "a dog reading a newspaper";
        string outputFileName = "dog_newspaper.png";
        string location = "us-central1";
        string model = "gemini-2.5-flash-image";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/google/models/{model}",
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

        var imagePart = response.Candidates.FirstOrDefault()?.Content?.Parts?.FirstOrDefault(p => p.InlineData != null);
        
        if (imagePart == null)
        {
            throw new Exception("No image data was returned by the model.");
        }

        byte[] imageBytes = imagePart.InlineData.Data.ToByteArray();

        File.WriteAllBytes(outputFileName, imageBytes);
        FileInfo fileInfo = new FileInfo(Path.GetFullPath(outputFileName));

        Console.WriteLine($"Created output image {fileInfo.FullName} with {fileInfo.Length} bytes");
        return fileInfo;
    }
}

// [END generativeaionvertexai_imagen_generate_image]