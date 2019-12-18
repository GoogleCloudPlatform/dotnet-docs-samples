// Copyright (c) 2019 Google LLC.
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

using CommandLine;
using Google.Cloud.AutoML.V1;
using System;

namespace GoogleCloudSamples
{
    [Verb("language_entity_extraction_predict", HelpText = "Extract the text content")]
    public class LanguageEntityExtractionPredictOptions : PredictOptions
    {
        [Value(2, HelpText = "Location of file with text to translate")]
        public string Content { get; set; }
    }
    class AutoMLLanguageEntityExtractionPredict
    {
        // [START automl_language_entity_extraction_predict]
        /// <summary>
        /// Demonstrates using the AutoML client to predict the text content using given model.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        /// <param name="content">Text content for prediction.</param>
        public static object LanguageEntityExtractionPredict(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID",
            string content = "YOUR TEXT TO PREDICT")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            PredictionServiceClient client = PredictionServiceClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);

            TextSnippet textSnippet = new TextSnippet
            {
                Content = content,
                MimeType = "text/plain"
            };
            ExamplePayload payload = new ExamplePayload
            {
                TextSnippet = textSnippet
            };

            PredictRequest predictRequest = new PredictRequest
            {
                Name = modelFullId,
                Payload = payload
            };

            PredictResponse response = client.Predict(predictRequest);

            foreach (AnnotationPayload annotationPayload in response.Payload)
            {
                Console.WriteLine($"Text Extract Entity Type: {annotationPayload.DisplayName}");
                Console.WriteLine($"Text score: {annotationPayload.TextExtraction.Score.ToString("0.00")}");
                TextSegment textSegment = annotationPayload.TextExtraction.TextSegment;
                Console.WriteLine($"Text Extract Entity Content: {textSegment.Content}");
                Console.WriteLine($"Text Start Offset: {textSegment.StartOffset}");
                Console.WriteLine($"Text End Offset: {textSegment.EndOffset}");
            }
            return 0;
        }
        // [END automl_language_entity_extraction_predict]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((LanguageEntityExtractionPredictOptions opts) =>
                AutoMLLanguageEntityExtractionPredict.LanguageEntityExtractionPredict(
                    opts.ProjectID,
                    opts.ModelID,
                    opts.Content));
        }
    }
}
