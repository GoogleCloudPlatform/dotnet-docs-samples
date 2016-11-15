// Copyright(c) 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START language_quickstart]

using Google.Cloud.Language.V1Beta1;
using System;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // The text to analyze.
            string text = "Hello World!";
            var client = LanguageServiceClient.Create();
            var response = client.AnalyzeSentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            var sentiment = response.DocumentSentiment;
            Console.WriteLine($"Polarity: {sentiment.Polarity}");
            Console.WriteLine($"Magnitude: {sentiment.Magnitude}");
        }
    }
}
// [END language_quickstart]
