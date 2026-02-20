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

// [START googlegenaisdk_counttoken_resp_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;

public class CountTokenRespWithTxt
{
    public async Task<GenerateContentResponseUsageMetadata> CountTokens(
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
            contents: "Why is the sky blue?");

        GenerateContentResponseUsageMetadata usageMetadata = response.UsageMetadata;
        Console.WriteLine(usageMetadata);
        // Example response:
        // GenerateContentResponseUsageMetadata { CacheTokensDetails = ,
        // CachedContentTokenCount = , CandidatesTokenCount = 589,
        // ...
        // ThoughtsTokenCount = 945, ToolUsePromptTokenCount = ,
        // ToolUsePromptTokensDetails = , TotalTokenCount = 1540,
        // TrafficType = ON_DEMAND }
        return usageMetadata;
    }
}
// [END googlegenaisdk_counttoken_resp_with_txt]
