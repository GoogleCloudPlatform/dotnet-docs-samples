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

// [START googlegenaisdk_tools_func_desc_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ToolsFuncDescWithTxt
{
    public async Task<FunctionCall> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        var parametersJsonSchema = new Dictionary<string, object>
        {
            { "type", "object" },
            {
                "properties", new Dictionary<string, object>
                {
                    {
                        "albums", new Dictionary<string, object>
                        {
                            { "type", "array" },
                            { "description", "List of albums" },
                            {
                                "items", new Dictionary<string, object>
                                {
                                    { "type", "object" },
                                    { "description", "Album and its sales" },
                                    {
                                        "properties", new Dictionary<string, object>
                                        {
                                            {
                                                "album_name", new Dictionary<string, object>
                                                {
                                                    { "type", "string" },
                                                    { "description", "Name of the music album" }
                                                }
                                            },
                                            {
                                                "copies_sold", new Dictionary<string, object>
                                                {
                                                    { "type", "integer" },
                                                    { "description", "Number of copies sold" }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var responseJsonSchema = new Dictionary<string, object>
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
                                 "album_name", new Dictionary<string, object>
                                 {
                                     { "type", "string" }
                                 }
                             },
                             {
                                 "copies_sold", new Dictionary<string, object>
                                 {
                                     { "type", "integer" }
                                 }
                             }
                         }
                    }
                }
            }
        };

        List<FunctionDeclaration> getAlbumSales = new List<FunctionDeclaration>
        {
            new FunctionDeclaration
            {
                Name = "get_album_sales",
                Description = "Gets the number of albums sold",
                ParametersJsonSchema = parametersJsonSchema,
                ResponseJsonSchema = responseJsonSchema
            }
        };

        string contents = "At Stellar Sounds, a music label, 2024 was a rollercoaster." +
            " \"Echoes of the Night,\" a debut synth-pop album, surprisingly sold 350,000 copies," +
            " while veteran rock band \"Crimson Tide\\'s\" latest, \"Reckless Hearts,\" lagged at 120,000." +
            " Their up-and-coming indie artist, \"Luna Bloom\\'s\" EP, \"Whispers of Dawn,\" secured 75,000 sales." +
            " The biggest disappointment was the highly-anticipated rap album \"Street Symphony\" only reaching 100,000" +
            " units. Overall, Stellar Sounds moved over 645,000 units this year, revealing unexpected trends in music consumption.";

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: contents,
            config: new GenerateContentConfig
            {
                Temperature = 0,
                Tools = new List<Tool>
                {
                    new Tool { FunctionDeclarations = getAlbumSales }
                }
            });

        FunctionCall functionCall = response.Candidates[0].Content.Parts[0].FunctionCall;

        Console.WriteLine($"Name: {functionCall.Name}");

        foreach (var parameter in functionCall.Args)
        {
            Console.WriteLine(parameter);
        }
        // Example output:
        // Name: get_album_sales
        //
        // [albums, [{"album_name": "Echoes of the Night", "copies_sold": 350000},
        // {"album_name": "Reckless Hearts", "copies_sold": 120000},
        // {"album_name": "Whispers of Dawn", "copies_sold": 75000},
        // {"album_name": "Street Symphony","copies_sold": 100000}]]
        return functionCall;
    }
}
// [END googlegenaisdk_tools_func_desc_with_txt]
