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

// [START googlegenaisdk_textgen_config_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TextGenConfigWithTxt
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
            contents: "Why is the sky blue?",
            // See the SDK documentation at
            // https://googleapis.github.io/dotnet-genai/api/Google.GenAI.Types.GenerateContentConfig.html
            config: new GenerateContentConfig
            {
                Temperature = 0,
                CandidateCount = 1,
                ResponseMimeType = "application/json",
                TopP = 0.95,
                TopK = 20,
                Seed = 5,
                MaxOutputTokens = 500,
                StopSequences = new List<string> { "STOP!" },
                PresencePenalty = 0,
                FrequencyPenalty = 0
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // {
        //  "explanation": "The sky appears blue due to a phenomenon called Rayleigh scattering...
        // }
        return responseText;
    }
}
// [END googlegenaisdk_textgen_config_with_txt]
