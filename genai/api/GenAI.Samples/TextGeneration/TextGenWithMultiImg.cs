// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START googlegenaisdk_textgen_with_multi_img]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class TextGenWithMultiImg
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash",
        string localImageFilePath = "path/to/img.jpg")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        // Read local image content.
        byte[] localImgBytes = File.ReadAllBytes(localImageFilePath);

        // Image from GCS
        string gcsImageFilePath = "gs://cloud-samples-data/generative-ai/image/scones.jpg";

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part { FileData = new FileData { FileUri = gcsImageFilePath, MimeType = "image/jpeg" } },
                    new Part { InlineData = new Blob { Data = localImgBytes, MimeType = "image/jpeg" } },
                    new Part { Text = "Generate a list of all the objects contained in both images." }
                }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: contents);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // The objects contained in both images are:
        // *Coffee
        // * Cups(or mugs)
        return responseText;
    }
}
// [END googlegenaisdk_textgen_with_multi_img]
