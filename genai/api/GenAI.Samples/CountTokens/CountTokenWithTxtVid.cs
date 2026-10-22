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

// [START googlegenaisdk_counttoken_with_txt_vid]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CountTokenWithTxtVid
{
    public async Task<int> CountTokens(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        var contents = new List<Content>
        {
            new Content
            {
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
                    new Part { Text = "Provide a description of the video." }
                }
            }
        };

        CountTokensResponse response = await client.Models.CountTokensAsync(
            model: model,
            contents: contents);

        int totalTokens = response.TotalTokens ?? 0;
        Console.WriteLine($"Total tokens: {totalTokens}");
        // Example response:
        // Total tokens: 16708
        return totalTokens;
    }
}
// [END googlegenaisdk_counttoken_with_txt_vid]
