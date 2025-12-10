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

// [START googlegenaisdk_ctrlgen_with_enum_schema]

using Google.GenAI;
using Google.GenAI.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CtrlGenWithEnumSchema
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

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "What type of instrument is an oboe?",
            config: new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseJsonSchema = new Dictionary<string, object>
                {
                    { "type", "string" },
                    { "enum", new List<string> { "Percussion", "String", "Woodwind", "Brass", "Keyboard" } }
                }
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        System.Console.WriteLine(responseText);
        // Example response:
        // Woodwind
        return responseText;
    }
}
// [END googlegenaisdk_ctrlgen_with_enum_schema]
