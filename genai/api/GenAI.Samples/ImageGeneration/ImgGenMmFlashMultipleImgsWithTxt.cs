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

// [START googlegenaisdk_imggen_mmflash_multiple_imgs_with_txt]]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImgGenMmFlashMultipleImgsWithTxt
{
    public async Task<List<string>> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash-image")
    {
        await using var client = new Client(project: projectId, location: location, vertexAI: true);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "Generate 3 images of a cat sitting on a chair.",
            config: new GenerateContentConfig { ResponseModalities = new List<string> { "TEXT", "IMAGE" } });

        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();

        var generatedFiles = new List<string>();
        int imageCounter = 1;

        foreach (Part part in parts)
        {
            if (!string.IsNullOrEmpty(part.Text))
            {
                Console.WriteLine(part.Text);
            }
            else if (part.InlineData?.Data != null)
            {
                var filename = $"example-cats-0{imageCounter}.png";
                File.WriteAllBytes(filename, part.InlineData.Data);
                generatedFiles.Add(filename);
                imageCounter++;
            }
        }
        // Example response:
        // Here are three images of cats sitting on chairs.
        return generatedFiles;
    }
}
// [END googlegenaisdk_imggen_mmflash_multiple_imgs_with_txt]]
