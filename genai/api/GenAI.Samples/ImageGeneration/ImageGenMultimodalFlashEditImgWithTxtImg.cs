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

// [START googlegenaisdk_imggen_mmflash_edit_img_with_txt_img]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImageGenMultimodalFlashEditImgWithTxtImg
{
    public async Task<FileInfo> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash-image",
        string localImageFilePath = "path/to/local_image.png")
    {
        await using var client = new Client(project: projectId, location: location, vertexAI: true);

        // Read local image content.
        byte[] imageBytes = File.ReadAllBytes(localImageFilePath);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: new List<Content>
            {
                new Content
                {
                    Role= "user",
                    Parts = new List<Part>
                    {
                        new Part { InlineData = new Blob { Data = imageBytes, MimeType = "image/png" } },
                        new Part { Text = "Edit this image to make it look like a cartoon."}
                    }
                }
            },
            config: new GenerateContentConfig { ResponseModalities = new List<string> { "TEXT", "IMAGE" } });

        // Get parts of the response.
        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();

        var outputFilename = "bw-example-image.png";

        foreach (Part part in parts)
        {
            if (!string.IsNullOrEmpty(part.Text))
            {
                Console.WriteLine(part.Text);
            }
            else if (part.InlineData?.Data != null)
            {
                File.WriteAllBytes(outputFilename, part.InlineData.Data);
            }
        }

        FileInfo fileInfo = new FileInfo(Path.GetFullPath(outputFilename));
        Console.WriteLine($"Created output image using {fileInfo.Length} bytes");
        // Example response:
        // Here's the image cartoonized for you! 
        // Created output image using 1628165 bytes
        return fileInfo;
    }
}
// [END googlegenaisdk_imggen_mmflash_edit_img_with_txt_img]
