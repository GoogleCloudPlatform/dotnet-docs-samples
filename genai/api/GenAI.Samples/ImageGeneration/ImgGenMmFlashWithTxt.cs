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

// [START googlegenaisdk_imggen_mmflash_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImgGenMmFlashWithTxt
{
    public async Task<FileInfo> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash-image")
    {
        await using var client = new Client(project: projectId, location: location, vertexAI: true);

        var contentConfig = new GenerateContentConfig
        {
            ResponseModalities = new List<string> { "TEXT", "IMAGE" },
            CandidateCount = 1,
            SafetySettings = new List<SafetySetting>
            {
                new SafetySetting
                {
                    Method = HarmBlockMethod.PROBABILITY,
                    Category = HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT,
                    Threshold = HarmBlockThreshold.BLOCK_MEDIUM_AND_ABOVE
                }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "Generate an image of the Eiffel tower with fireworks in the background.",
            config: contentConfig);

        var outputFile = "example-image-eiffel-tower.png";
        FileInfo fileInfo = null;

        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();

        foreach (Part part in parts)
        {
            if (!string.IsNullOrEmpty(part.Text))
            {
                Console.WriteLine(part.Text);
            }
            else if (part.InlineData?.Data != null)
            {
                File.WriteAllBytes(outputFile, part.InlineData.Data);
                fileInfo = new FileInfo(Path.GetFullPath(outputFile));
                Console.WriteLine($"Created output image using {fileInfo.Length} bytes");
            }
        }
        // Example response:
        // Absolutely! Here's the Eiffel Tower with fireworks:
        // 
        // Created output image using 1897804 bytes
        return fileInfo;
    }
}
// [END googlegenaisdk_imggen_mmflash_with_txt]
