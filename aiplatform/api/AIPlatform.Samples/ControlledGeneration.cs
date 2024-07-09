/*
 * Copyright 2024 Google LLC
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

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;
using Type = Google.Cloud.AIPlatform.V1.Type;

public class ControlledGeneration
{
    // [START generativeaionvertexai_gemini_controlled_generation_response_mime_type]
    public async Task<string> GenerateContentWithResponseMimeType(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        string prompt = @"
            List a few popular cookie recipes using this JSON schema:
            Recipe = {""recipe_name"": str}
            Return: `list[Recipe]`";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = prompt }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json"
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_mime_type]

    // [START generativeaionvertexai_gemini_controlled_generation_response_schema]
    public async Task<string> GenerateContentWithResponseSchema(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var responseSchema = new OpenApiSchema
        {
            Type = Type.Array,
            Items = new()
            {
                Type = Type.Object,
                Properties =
                {
                    ["recipe_name"] = new() { Type = Type.String },
                },
                Required = { "recipe_name" }
            }
        };

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = "List a few popular popular cookie recipes" }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_schema]

    // [START generativeaionvertexai_gemini_controlled_generation_response_schema_2]
    public async Task<string> GenerateContentWithResponseSchema2(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var responseSchema = new OpenApiSchema
        {
            Type = Type.Array,
            Items = new()
            {
                Type = Type.Object,
                Properties =
                {
                    ["rating"] = new() { Type = Type.Integer },
                    ["flavor"] = new() { Type = Type.String }
                },
                Required = { "rating", "flavor" }
            }
        };

        string prompt = @"
            Reviews from our social media:

            - ""Absolutely loved it! Best ice cream I've ever had."" Rating: 4, Flavor: Strawberry Cheesecake
            - ""Quite good, but a bit too sweet for my taste."" Rating: 1, Flavor: Mango Tango";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = prompt }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_schema_2]

    // [START generativeaionvertexai_gemini_controlled_generation_response_schema_3]
    public async Task<string> GenerateContentWithResponseSchema3(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var responseSchema = new OpenApiSchema
        {
            Type = Type.Object,
            Properties =
            {
                ["forecast"] = new()
                {
                    Type = Type.Array,
                    Items = new()
                    {
                        Type = Type.Object,
                        Properties =
                        {
                            ["Forecast"] = new() { Type = Type.String },
                            ["Humidity"] = new() { Type = Type.String },
                            ["Temperature"] = new() { Type = Type.Integer },
                            ["Wind Speed"] = new() { Type = Type.Integer }
                        }
                    }
                }
            }
        };

        string prompt = @"
        The week ahead brings a mix of weather conditions.
        Sunday is expected to be sunny with a temperature of 77°F and a humidity level of 50%. Winds will be light at around 10 km/h.
        Monday will see partly cloudy skies with a slightly cooler temperature of 72°F and humidity increasing to 55%. Winds will pick up slightly to around 15 km/h.
        Tuesday brings rain showers, with temperatures dropping to 64°F and humidity rising to 70%. Expect stronger winds at 20 km/h.
        Wednesday may see thunderstorms, with a temperature of 68°F and high humidity of 75%. Winds will be gusty at 25 km/h.
        Thursday will be cloudy with a temperature of 66°F and moderate humidity at 60%. Winds will ease slightly to 18 km/h.
        Friday returns to partly cloudy conditions, with a temperature of 73°F and lower humidity at 45%. Winds will be light at 12 km/h.
        Finally, Saturday rounds off the week with sunny skies, a temperature of 80°F, and a humidity level of 40%. Winds will be gentle at 8 km/h.";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = prompt }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_schema_3]

    // [START generativeaionvertexai_gemini_controlled_generation_response_schema_4]
    public async Task<string> GenerateContentWithResponseSchema4(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var responseSchema = new OpenApiSchema
        {
            Type = Type.Array,
            Items = new()
            {
                Type = Type.Object,
                Properties =
                {
                    ["to_discard"] = new() { Type = Type.Integer },
                    ["subcategory"] = new() { Type = Type.String },
                    ["safe_handling"] = new() { Type = Type.Integer },
                    ["item_category"] = new()
                    {
                        Type = Type.String,
                        Enum =
                        {
                            "clothing",
                            "winter apparel",
                            "specialized apparel",
                            "furniture",
                            "decor",
                            "tableware",
                            "cookware",
                            "toys"
                        }
                    },
                    ["for_resale"] = new() { Type = Type.Integer },
                    ["condition"] = new()
                    {
                        Type = Type.String,
                        Enum =
                        {
                            "new in package",
                            "like new",
                            "gently used",
                            "used",
                            "damaged",
                            "soiled"
                        }
                    }
                }
            }
        };

        string prompt = @"
            Item description:
            The item is a long winter coat that has many tears all around the seams and is falling apart.
            It has large questionable stains on it.";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = prompt }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_schema_4]

    // [START generativeaionvertexai_gemini_controlled_generation_response_schema_6]
    public async Task<string> GenerateContentWithResponseSchema6(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-pro-001")
    {

        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        var responseSchema = new OpenApiSchema
        {
            Type = Type.Object,
            Properties =
            {
                ["playlist"] = new()
                {
                    Type = Type.Array,
                    Items = new()
                    {
                        Type = Type.Object,
                        Properties =
                        {
                            ["artist"] = new() { Type = Type.String },
                            ["song"] = new() { Type = Type.String },
                            ["era"] = new() { Type = Type.String },
                            ["released"] = new() { Type = Type.Integer }
                        }
                    }
                },
                ["time_start"] = new() { Type = Type.String }
            }
        };

        string prompt = @"
        We have two friends of the host who have requested a few songs for us to play. We're going to start this playlist at 8:15.
        They'll want to hear Black Hole Sun by Soundgarden because their son was born in 1994. They will also want Loser by Beck
        coming right after which is a funny choice considering it's also the same year as their son was born, but that's probably
        just a coincidence. Add Take On Me from A-ha to the list since they were married when the song released in 1985. Their final
        request is Sweet Child O' Mine by Guns N Roses, which I think came out in 1987 when they both finished university.
        Thank you, this party should be great!";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                new Content
                {
                    Role = "USER",
                    Parts =
                    {
                        new Part { Text = prompt }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema
            },
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
    // [END generativeaionvertexai_gemini_controlled_generation_response_schema_6]
}
