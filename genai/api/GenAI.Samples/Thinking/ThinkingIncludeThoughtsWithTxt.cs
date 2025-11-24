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

// [START googlegenaisdk_thinking_includethoughts_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;

public class ThinkingIncludeThoughtsWithTxt
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-pro")
    {
        var client = new Client(project: projectId, location: location, vertexAI: true);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "solve x^2 + 4x + 4 = 0",
            config: new GenerateContentConfig
            {
                ThinkingConfig = new ThinkingConfig { IncludeThoughts = true }
            });

        foreach (Part part in response.Candidates[0].Content.Parts)
        {
            if (part.Thought.GetValueOrDefault(false))
            {
                Console.WriteLine(part.Text);
            }
        }
        // Example response:
        // Alright, let's break down this quadratic equation, `x^2 + 4x + 4 = 0`. First things first, I recognize
        // this is a standard quadratic form, perfect for applying my knowledge.
        // 1. **Analyze and Prep:**Okay, the user wants the solution.Got it. This is a quadratic in standard
        // form, `ax ^ 2 + bx + c = 0`. That means I have several solution strategies at my disposal.
        // 2. **Coefficients in Hand: **  `a` is 1, `b` is 4, and `c` is also 4.Easy enough.
        // 3. **Method Selection - The Decision Point:**Now, I've got to choose the most efficient path.
        // I could *always* use the quadratic formula, but that might be overkill. Factoring is always worth a
        // look, especially when dealing with smaller coefficients. Completing the square is a possibility, but
        // less elegant here. Graphing... well, that's more for visualization, not direct algebraic solution.
        // 4. **Evaluate for *This * Case:**Let's see if factoring holds up.
        // ...
        // * End with a concise summary of the key takeaway, emphasizing that it's a repeated root.
        // Perfect.The plan is set, the path is clear.
        return response.Candidates[0].Content.Parts[0].Text;
    }
}
// [END googlegenaisdk_thinking_includethoughts_with_txt]
