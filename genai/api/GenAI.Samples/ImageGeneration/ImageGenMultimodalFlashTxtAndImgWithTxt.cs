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

// [START googlegenaisdk_imggen_mmflash_txt_and_img_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImageGenMultimodalFlashTxtAndImgWithTxt
{
    public async Task<FileInfo> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash-image")
    {
        await using var client = new Client(project: projectId, location: location, vertexAI: true);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: new List<Content>
            {
                new Content
                {
                    Role= "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = "Generate an illustrated recipe for a paella." },
                        new Part { Text = "Create images to go alongside the text as you generate the recipe."}
                    }
                }
            },
            config: new GenerateContentConfig { ResponseModalities = new List<string> { "TEXT", "IMAGE" } });

        // Get parts of the response.
        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();

        using (StreamWriter writer = new StreamWriter("paella-recipe.md"))
        {
            int imageCounter = 1;
            foreach (Part part in parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    writer.WriteLine(part.Text);
                }
                else if (part.InlineData?.Data != null)
                {
                    string filename = $"example-image-{imageCounter}.png";
                    File.WriteAllBytes(filename, part.InlineData.Data);
                    writer.WriteLine($"\n![image]({filename})\n");
                    imageCounter++;
                }
            }
        }

        FileInfo fileInfo = new FileInfo(Path.GetFullPath("paella-recipe.md"));
        Console.WriteLine($"Created output image using {fileInfo.Length} bytes");
        // Example response:
        // Created output image using 2329 bytes
        return fileInfo;
    }
}
// [END googlegenaisdk_imggen_mmflash_txt_and_img_with_txt]
