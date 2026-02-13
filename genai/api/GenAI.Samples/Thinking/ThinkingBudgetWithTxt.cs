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

// [START googlegenaisdk_thinking_budget_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;

public class ThinkingBudgetWithTxt
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash")
    {
        var client = new Client(project: projectId, location: location, vertexAI: true);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "solve x^2 + 4x + 4 = 0",
            config: new GenerateContentConfig
            {
                ThinkingConfig = new ThinkingConfig { ThinkingBudget = 1024 }
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example reponse:
        // To solve the equation $x^2 + 4x + 4 = 0$, we can use several methods:
        // **Method 1: Factoring(Recognizing a Perfect Square Trinomial) * *
        // Notice that the left side of the equation is a perfect square trinomial.
        // It follows the pattern $a ^ 2 + 2ab + b ^ 2 = (a + b) ^ 2$.
        // ...
        // All methods yield the same solution.
        // The solution is $x = -2$.

        Console.WriteLine($"Token count for thinking: {response.UsageMetadata.ThoughtsTokenCount}");
        // Example response:
        // Token count for thinking: 804
        return responseText;
    }
}
// [END googlegenaisdk_thinking_budget_with_txt]
