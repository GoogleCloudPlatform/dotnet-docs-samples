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

// [START googlegenaisdk_tools_vais_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ToolsVaisWithTxt
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

        string datastore = "projects/{projectId}/locations/{location}/collections/default_collection/dataStores/{data-store-id}";

        List<Tool> tools = new List<Tool>
        {
            new Tool
            {
                Retrieval = new Retrieval { VertexAiSearch = new VertexAISearch { Datastore = datastore } }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "How do I make an appointment to renew my driver's license?",
            config: new GenerateContentConfig { Tools = tools });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // Making an appointment to renew your driver's license typically involves a few common steps,
        // though the exact process can vary depending on your location (state, province, or country).
        // Here's a general guide:
        // 1. **Identify Your Local Licensing Authority:** The first step is to determine the governmental...
        return responseText;
    }
}
// [END googlegenaisdk_tools_vais_with_txt]
