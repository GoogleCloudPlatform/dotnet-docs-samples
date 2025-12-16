/*
 * Copyright 2025 Google LLC
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

// [START googlegenaisdk_textgen_with_video]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TextGenWithVideo
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        string prompt = "Analyze the provided video file, including its audio. " +
            "Summarize the main points of the video concisely. " +
            "Create a chapter breakdown with timestamps for key sections or topics discussed.";

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part
                    {
                        FileData = new FileData
                        {
                            FileUri = "gs://cloud-samples-data/generative-ai/video/pixel8.mp4",
                            MimeType = "video/mp4"
                        }
                    },
                    new Part { Text = prompt }
                }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: contents);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // This video features Saeka Shimada, a Tokyo photographer, as she explores the city's vibrant nightlife
        // using the new Google Pixel. She highlights the phone's "Video Boost" feature, which leverages
        // "Night Sight" for superior low-light video quality...
        return responseText;
    }
}
// [END googlegenaisdk_textgen_with_video]
