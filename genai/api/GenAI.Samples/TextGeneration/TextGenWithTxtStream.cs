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

// [START googlegenaisdk_textgen_with_txt_stream]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Text;
using System.Threading.Tasks;

public class TextGenWithTxtStream
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

        var fullResponse = new StringBuilder();

        await foreach (GenerateContentResponse response in client.Models.GenerateContentStreamAsync(
            model: model,
            contents: "Why is the sky blue?"))
        {
            string text = response.Candidates[0].Content.Parts[0].Text;
            Console.WriteLine(text);
            fullResponse.Append(text);
        }
        // Example response:
        // The sky is blue primarily due to a phenomenon called **Rayleigh Scattering**.
        // Here's a breakdown:
        // ...
        return fullResponse.ToString();
    }
}
// [END googlegenaisdk_textgen_with_txt_stream]
