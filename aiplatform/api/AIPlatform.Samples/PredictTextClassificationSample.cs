/*
 * Copyright 2023 Google LLC
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

// [START aiplatform_sdk_classify_news_items]

using Google.Cloud.AIPlatform.V1;
using wkt = Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

// Text Classification with a Large Language Model
public class PredictTextClassificationSample
{
    public string PredictTextClassification(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "text-bison@001")
    {
        // Initialize client that will be used to send requests.
        // This client only needs to be created once,
        // and can be reused for multiple requests.
        var client = new PredictionServiceClientBuilder
        {
            Endpoint = $"{locationId}-aiplatform.googleapis.com"
        }.Build();

        // Configure the parent resource.
        var endpoint = EndpointName.FromProjectLocationPublisherModel(projectId, locationId, publisher, model);

        // Initialize request argument(s).
        var content = @"What is the topic for a given news headline?
- business
- entertainment
- health
- sports
- technology

Text: Pixel 7 Pro Expert Hands On Review, the Most Helpful Google Phones.
The answer is: technology

Text: Quit smoking?
The answer is: health

Text: Roger Federer reveals why he touched Rafael Nadals hand while they were crying
The answer is: sports

Text: Business relief from Arizona minimum-wage hike looking more remote
The answer is: business

Text: #TomCruise has arrived in Bari, Italy for #MissionImpossible.
The answer is: entertainment

Text: CNBC Reports Rising Digital Profit as Print Advertising Falls
The answer is:";

        var instances = new List<wkt::Value>
        {
            wkt::Value.ForStruct(new()
            {
                Fields =
                {
                    ["content"] = wkt::Value.ForString(content),
                }
            })
        };

        var parameters = wkt::Value.ForStruct(new()
        {
            Fields =
            {
                { "temperature", new wkt::Value { NumberValue = 0 } },
                { "maxDecodeSteps", new wkt::Value { NumberValue = 5 } },
                { "topP", new wkt::Value { NumberValue = 0 } },
                { "topK", new wkt::Value { NumberValue = 1 } }
            }
        });

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse and return the content.
        var responseContent = response.Predictions[0].StructValue.Fields["content"].StringValue;
        Console.WriteLine($"Content: {responseContent}");
        return responseContent;
    }
}

// [END aiplatform_sdk_classify_news_items]
