﻿// Copyright 2021 Google Inc. All Rights Reserved.
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

// [START retail_simple_prediction]
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
    /// The prediction simple sample class.
    /// </summary>
    public class PredictionSimpleSample
    {
        /// <summary>
        /// Get Prediction Service
        /// </summary>
        /// <returns></returns>
        private static PredictionServiceClient GetPredictionService()
        {
            PredictionServiceClientBuilder predictionServiceClientBuilder = new PredictionServiceClientBuilder();

            PredictionServiceClient predictionServiceClient = predictionServiceClientBuilder.Build();
            return predictionServiceClient;
        }

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

            // Try to update `returnProduct` here
            Value returnProduct = new Value();
            returnProduct.BoolValue = true;

            Dictionary<string, Value> entries = new Dictionary<string, Value>()
            {
                { "returnProduct", returnProduct }
            };

            predictRequest.Params.Add(entries);

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
    // [END retail_simple_prediction]

    /// <summary>
    /// The prediction simple tutorial class.
    /// </summary>
    public static class PredictionSimpleTutorial
    {
        [Example]
        public static PredictResponse PerformGetPrediction()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            PredictResponse predictResponse = PredictionSimpleSample.GetPrediction(projectId);
            return predictResponse;
        }
    }
}
