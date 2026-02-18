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

// [START googlegenaisdk_ctrlgen_with_resp_schema]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ControlledGenWithRespSchema
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

        string prompt = "List a few popular cookie recipes.";

        var responseSchema = new Dictionary<string, object>
        {
            { "type", "array" },
            {
                "items", new Dictionary<string, object>
                {
                    { "type", "object" },
                    {
                        "properties", new Dictionary<string, object>
                        {
                            {
                                "recipe_name", new Dictionary<string, object>
                                {
                                    { "type", "string" }
                                }
                            },
                            {
                                "ingredients", new Dictionary<string, object>
                                {
                                    { "type", "array" },
                                    {
                                        "items", new Dictionary<string, object>
                                        {
                                            { "type", "string" },
                                        }
                                    }
                                }
                            },
                        }
                    },
                    { "required", new List<string> { "recipe_name", "ingredients" } }
                }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: prompt,
            config: new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseJsonSchema = responseSchema
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // [
        //  {
        //      "recipe_name": "Classic Chocolate Chip Cookies",
        //      "ingredients": [
        //          "all-purpose flour",
        //          "baking soda",
        //          "salt",
        //          "unsalted butter",
        //          "granulated sugar",
        //          "brown sugar",
        //          "eggs",
        //          "vanilla extract",
        //          "chocolate chips"]
        //  },
        //  {
        //      "recipe_name": "Peanut Butter Cookies",
        //      "ingredients": [
        //          "all-purpose flour",
        //          "baking soda",
        //          "salt",
        //          "unsalted butter",
        //          "peanut butter",
        //          "granulated sugar",
        //          "brown sugar",
        //          "eggs",
        //          "vanilla extract"]
        //  }
        // ]
        return responseText;
    }
}
// [END googlegenaisdk_ctrlgen_with_resp_schema]
