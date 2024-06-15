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

// [START generativeaionvertexai_gemini_function_calling]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;
using Type = Google.Cloud.AIPlatform.V1.Type;
using Value = Google.Protobuf.WellKnownTypes.Value;

public class FunctionCalling
{
    public async Task<string> GenerateFunctionCall(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.5-flash-001")
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = $"{location}-aiplatform.googleapis.com"
        }.Build();

        // Define the user's prompt in a Content object that we can reuse in
        // model calls
        var userPromptContent = new Content
        {
            Role = "USER",
            Parts =
            {
                new Part { Text = "What is the weather like in Boston?" }
            }
        };

        // Specify a function declaration and parameters for an API request
        var functionName = "get_current_weather";
        var getCurrentWeatherFunc = new FunctionDeclaration
        {
            Name = functionName,
            Description = "Get the current weather in a given location",
            Parameters = new OpenApiSchema
            {
                Type = Type.Object,
                Properties =
                {
                    ["location"] = new()
                    {
                        Type = Type.String,
                        Description = "Get the current weather in a given location"
                    },
                    ["unit"] = new()
                    {
                        Type = Type.String,
                        Description = "The unit of measurement for the temperature",
                        Enum = {"celsius", "fahrenheit"}
                    }
                },
                Required = { "location" }
            }
        };

        // Send the prompt and instruct the model to generate content using the tool that you just created
        var generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0f
            },
            Contents =
            {
                userPromptContent
            },
            Tools =
            {
                new Tool
                {
                    FunctionDeclarations = { getCurrentWeatherFunc }
                }
            }
        };

        GenerateContentResponse response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        var functionCall = response.Candidates[0].Content.Parts[0].FunctionCall;
        Console.WriteLine(functionCall);

        string apiResponse = "";

        // Check the function name that the model responded with, and make an API call to an external system
        if (functionCall.Name == functionName)
        {
            // Extract the arguments to use in your API call
            string locationCity = functionCall.Args.Fields["location"].StringValue;

            // Here you can use your preferred method to make an API request to
            // fetch the current weather

            // In this example, we'll use synthetic data to simulate a response
            // payload from an external API
            apiResponse = @"{ ""location"": ""Boston, MA"",
                    ""temperature"": 38, ""description"": ""Partly Cloudy""}";
        }

        // Return the API response to Gemini so it can generate a model response or request another function call
        generateContentRequest = new GenerateContentRequest
        {
            Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            Contents =
            {
                userPromptContent, // User prompt
                response.Candidates[0].Content, // Function call response,
                new Content
                {
                    Parts =
                    {
                        new Part
                        {
                            FunctionResponse = new()
                            {
                                Name = functionName,
                                Response = new()
                                {
                                    Fields =
                                    {
                                        { "content", new Value { StringValue = apiResponse } }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Tools =
            {
                new Tool
                {
                    FunctionDeclarations = { getCurrentWeatherFunc }
                }
            }
        };

        response = await predictionServiceClient.GenerateContentAsync(generateContentRequest);

        string responseText = response.Candidates[0].Content.Parts[0].Text;
        Console.WriteLine(responseText);

        return responseText;
    }
}

// [END generativeaionvertexai_gemini_function_calling]
