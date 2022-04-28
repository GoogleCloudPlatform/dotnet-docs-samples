// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START retail_prediction_with_parameters]
// Get predictions from catalog using Retail API

using Google.Cloud.Retail.V2;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Runner.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetailPredictions.Samples
{
    /// <summary>
    /// The prediction with parameters sample class.
    /// </summary>
    public class PredictionWithParametersSample
    {
        /// <summary>
        /// Get Prediction Request
        /// </summary>
        /// <param name="projectId">The current project id.</param>
        /// <returns></returns>
        private static PredictRequest GetPredictRequest(string projectId)
        {
            string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/prediction";

            PredictRequest predictRequest = new PredictRequest
            {
                Placement = defaultBranchName,
                UserEvent = new UserEvent()
                {
                    EventType = "detail-page-view",
                    VisitorId = "test_visitor_id", // A unique identifier to track visitors
                    EventTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    ProductDetails = { new ProductDetail { Product = new Product { Id = "test_id" } } } // An id of real product
                },
                Params =
                {
                    new MapField<string, Value>
                    {
                        new Dictionary<string, Value>
                        {
                            // Try to update `priceRerankLevel` here
                            { "priceRerankLevel", Value.ForString("low-price-reranking") },

                             // Try to update `diversityLevel` here
                            { "diversityLevel", Value.ForString("low-diversity") },

                            // Try to update `returnProduct` here
                            { "returnProduct", Value.ForBool(true) }
                        }
                    }
                }
            };

            return predictRequest;
        }

        /// <summary>
        /// Call the Retail API to get prediction
        /// </summary>
        /// <param name="projectId">The current project id.</param>
        /// <returns></returns>
        public static PredictResponse GetPrediction(string projectId)
        {
            PredictRequest predictRequest = GetPredictRequest(projectId);

            PredictionServiceClient client = PredictionServiceClient.Create();
            PredictResponse predictResponse = client.Predict(predictRequest);

            if (predictResponse.Results.Count == 0)
            {
                Console.WriteLine("The prediction operation returned no matching results.");
            }
            else
            {
                Console.WriteLine("Prediction results:");
                Console.WriteLine($"AttributionToken: {predictResponse.AttributionToken},");
                Console.WriteLine($"Total Count: {predictResponse.Results.Count},");
                Console.WriteLine("Items found");

                foreach (PredictResponse.Types.PredictionResult item in predictResponse.Results)
                {
                    Console.WriteLine($"Id: {item.Id}");
                }
            }

            return predictResponse;
        }
    }
    // [END retail_prediction_with_parameters]

    /// <summary>
    /// The prediction with parameters tutorial class.
    /// </summary>
    public static class PredictionWithParametersTutorial
    {
        [Example]
        public static PredictResponse PerformGetPrediction()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            PredictResponse predictResponse = PredictionWithParametersSample.GetPrediction(projectId);
            return predictResponse;
        }
    }
}
