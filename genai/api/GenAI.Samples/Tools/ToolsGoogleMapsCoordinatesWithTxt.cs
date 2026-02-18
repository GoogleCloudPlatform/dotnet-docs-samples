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

// [START googlegenaisdk_tools_google_maps_coordinates_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ToolsGoogleMapsCoordinatesWithTxt
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
            contents: "Where can I get the best espresso near me?",
            config: new GenerateContentConfig
            {
                Tools = new List<Tool>
                {
                    new Tool { GoogleMaps = new GoogleMaps() }
                },
                ToolConfig = new ToolConfig
                {
                    RetrievalConfig = new RetrievalConfig
                    {
                        LatLng = new LatLng { Latitude = 40.7128, Longitude = -74.006 },
                        LanguageCode = "en_US"
                    }
                }
            });

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);
        // Example response:
        // Here are some of the top-rated coffee shops near you that are
        // open now and known for their espresso...
        return responseText;
    }
}
// [END googlegenaisdk_tools_google_maps_coordinates_with_txt]
