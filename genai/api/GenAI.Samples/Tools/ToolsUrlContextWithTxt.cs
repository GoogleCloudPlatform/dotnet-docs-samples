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

// [START googlegenaisdk_tools_urlcontext_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ToolsUrlContextWithTxt
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

        string url1 = "https://cloud.google.com/vertex-ai/generative-ai/docs";
        string url2 = "https://cloud.google.com/docs/overview";

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: $"Compare the content, purpose, and audiences of {url1} and {url2}.",
            config: new GenerateContentConfig
            {
                Tools = new List<Tool>
                {
                    // Use Url Context Tool
                    new Tool { UrlContext = new UrlContext() }
                },
                ResponseModalities = new List<string> { "TEXT" }
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // The two Google Cloud documentation pages serve distinct purposes, cover different content scopes...

        // Url Context Metadata
        response.Candidates[0].UrlContextMetadata.UrlMetadata.ForEach(Console.WriteLine);
        // UrlMetadata { RetrievedUrl = https://cloud.google.com/docs/overview, UrlRetrievalStatus = URL_RETRIEVAL_STATUS_SUCCESS }
        // UrlMetadata { RetrievedUrl = https://cloud.google.com/vertex-ai/generative-ai/docs, UrlRetrievalStatus = URL_RETRIEVAL_STATUS_SUCCESS } 
        return responseText;
    }
}
// [END googlegenaisdk_tools_urlcontext_with_txt]
