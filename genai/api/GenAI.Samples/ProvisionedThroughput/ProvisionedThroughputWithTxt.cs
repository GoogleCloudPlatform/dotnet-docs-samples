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

// [START googlegenaisdk_provisionedthroughput_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProvisionedThroughputWithTxt
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string model = "gemini-2.5-flash")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions
            {
                ApiVersion = "v1",
                // Options:
                // - "dedicated": Use Provisioned Throughput
                // - "shared": Use pay-as-you-go
                // https://cloud.google.com/vertex-ai/generative-ai/docs/use-provisioned-throughput
                Headers = new Dictionary<string, string> { { "X-Vertex-AI-LLM-Request-Type", "shared" } }
            });

        GenerateContentResponse response = await client.Models.GenerateContentAsync(model: model, contents: "How does AI work?");

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // At its core, **Artificial Intelligence (AI)** works by enabling machines to learn from data,
        // identify patterns, and make decisions or predictions without being explicitly programmed for
        // every possible scenario...
        return responseText;
    }
}
// [END googlegenaisdk_provisionedthroughput_with_txt]

