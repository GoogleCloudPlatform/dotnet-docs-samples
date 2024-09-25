// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START apikeys_authenticate_api_key]

using Google.Cloud.Language.V1;
using System;

public class UseApiKeySample
{
    public void AnalyzeSentiment(string apiKey)
    {
        LanguageServiceClient client = new LanguageServiceClientBuilder
        {
            ApiKey = apiKey
        }.Build();

        string text = "Hello, world!";

        AnalyzeSentimentResponse response = client.AnalyzeSentiment(Document.FromPlainText(text));
        Console.WriteLine($"Text: {text}");
        Sentiment sentiment = response.DocumentSentiment;
        Console.WriteLine($"Sentiment: {sentiment.Score}, {sentiment.Magnitude}");
        Console.WriteLine("Successfully authenticated using the API key");
    }
}

// [END apikeys_authenticate_api_key]
