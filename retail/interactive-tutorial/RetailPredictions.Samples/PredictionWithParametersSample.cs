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
        private const string Endpoint = "retail.googleapis.com";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string PlacementName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/placements/prediction";

        /// <summary>
        /// Get Prediction Service
        /// </summary>
        /// <returns></returns>
        private static PredictionServiceClient GetPredictionService()
        {
            var predictionServiceClientBuilder = new PredictionServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var predictionServiceClient = predictionServiceClientBuilder.Build();
            return predictionServiceClient;
        }

        /// <summary>
        /// Get Predict Request
        /// </summary>
        /// <returns></returns>
        private static PredictRequest GetPredictRequest()
        {
            var predictRequest = new PredictRequest
            {
                Placement = PlacementName,
                UserEvent = new UserEvent
                {
                    EventType = "detail-page-view",
                    VisitorId = "281639", // A unique identifier to track visitors
                    EventTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            };

            predictRequest.UserEvent.ProductDetails.Add(new ProductDetail
            {
                Product = new Product
                {
                    Id = "55106" // An id of real product
                }
            });

            // Try to update `priceRerankLevel` here
            Value priceRerankLevel = new Value();
            priceRerankLevel.StringValue = "low-price-reranking";

            // Try to update `diversityLevel` here
            Value diversityLevel = new Value();
            diversityLevel.StringValue = "low-diversity";

            // Try to update `returnProduct` here
            Value returnProduct = new Value();
            returnProduct.BoolValue = true;

            Dictionary<string, Value> entries = new Dictionary<string, Value>()
            {
                { "returnProduct", returnProduct },
                { "priceRerankLevel", priceRerankLevel },
                { "diversityLevel", diversityLevel }
            };

            predictRequest.Params.Add(entries);

            return predictRequest;
        }

        /// <summary>
        /// Get Predict
        /// </summary>
        /// <returns></returns>
        public static PredictResponse GetPrediction()
        {
            PredictRequest predictRequest = GetPredictRequest();
            PredictResponse predictResponse = GetPredictionService().Predict(predictRequest);

            if (predictResponse.Results is null || !predictResponse.Results.Any())
            {
                Console.WriteLine("The search operation returned no matching results.");
            }
            else
            {
                Console.WriteLine("Search results:");
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
            PredictResponse predictResponse = PredictionWithParametersSample.GetPrediction();
            return predictResponse;
        }
    }
}
