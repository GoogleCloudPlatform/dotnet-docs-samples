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

// [START googlegenaisdk_ctrlgen_with_nullable_schema]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ControlledGenWithNullableSchema
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

        string prompt = @"The week ahead brings a mix of weather conditions.
        Sunday is expected to be sunny with a temperature of 77°F and a humidity level
        of 50%. Winds will be light at around 10 km/h.
        Monday will see partly cloudy skies with a slightly cooler temperature of 72°F
        and the winds will pick up slightly to around 15 km/h.
        Tuesday brings rain showers, with temperatures dropping to 64°F and humidity rising to 70%.
        Wednesday may see thunderstorms, with a temperature of 68°F.
        Thursday will be cloudy with a temperature of 66°F and moderate humidity at 60%.
        Friday returns to partly cloudy conditions, with a temperature of 73°F and the
        Winds will be light at 12 km/h.
        Finally, Saturday rounds off the week with sunny skies, a temperature of 80°F,
        and a humidity level of 40%. Winds will be gentle at 8 km/h.";

        var responseSchema = new Dictionary<string, object>
        {
            { "type", "object" },
            {
                "properties", new Dictionary<string, object>
                {
                    {
                        "forecast", new Dictionary<string, object>
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
                                                "Day", new Dictionary<string, object>
                                                {
                                                    { "type", "string" },
                                                    { "nullable", true }
                                                }
                                            },
                                            {
                                                "Forecast", new Dictionary<string, object>
                                                {
                                                    { "type", "string" },
                                                    { "nullable", true }
                                                }
                                            },
                                            {
                                                "Temperature", new Dictionary<string, object>
                                                {
                                                    { "type", "integer" },
                                                    { "nullable", true }
                                                }
                                            },
                                            {
                                                "Humidity", new Dictionary<string, object>
                                                {
                                                    { "type", "string" },
                                                    { "nullable", true }
                                                }
                                            },
                                            {
                                                "Wind Speed", new Dictionary<string, object>
                                                {
                                                    { "type", "integer" },
                                                    { "nullable", true }
                                                }
                                            }
                                        }
                                    },
                                    { "required", new List<string> { "Day", "Temperature", "Forecast", "Wind Speed" } }
                                }
                            }
                        }
                    }
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
        // {"forecast": [{"Day": "Sunday", "Temperature": 77, "Forecast": "sunny", "Wind Speed": 10, "Humidity": "50%"},
        //  {"Day": "Monday", "Temperature": 72, "Forecast": "partly cloudy", "Wind Speed": 15, "Humidity": null},
        //  {"Day": "Tuesday", "Temperature": 64, "Forecast": "rain showers", "Wind Speed": null, "Humidity": "70%"},
        //  {"Day": "Wednesday", "Temperature": 68, "Forecast": "thunderstorms", "Wind Speed": null, "Humidity": null},
        //  {"Day": "Thursday", "Temperature": 66, "Forecast": "cloudy", "Wind Speed": null, "Humidity": "60%"},
        //  {"Day": "Friday", "Temperature": 73, "Forecast": "partly cloudy", "Wind Speed": 12, "Humidity": null},
        //  {"Day": "Saturday", "Temperature": 80, "Forecast": "sunny", "Wind Speed": 8, "Humidity": "40%"}]}
        return responseText;
    }
}
// [END googlegenaisdk_ctrlgen_with_nullable_schema]
