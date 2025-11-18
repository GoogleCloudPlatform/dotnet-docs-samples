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

// [START googlegenaisdk_thinking_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Threading.Tasks;

public class ThinkingWithTxt
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-pro")
    {
        var client = new Client(project: projectId, location: location, vertexAI: true);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "solve x^2 + 4x + 4 = 0");

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example reponse:
        // Of course! We can solve the equation **x² + 4x + 4 = 0** in a couple of ways.
        // ### Method 1: Factoring
        // This is the quickest method for this particular problem.
        // 1.  * *Recognize the pattern:**The expression `x² +4x + 4` is a perfect square trinomial
        // ...
        // Both methods give the same result.
        // **The solution is x = -2.**
        return responseText;
    }
}
// [END googlegenaisdk_thinking_with_txt]
