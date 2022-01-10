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

// [START retail_add_fulfillment_places]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;

namespace grs_product
{
    public static class AddFulfillmentPlaces
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string ProductId = "add_fulfillment_test_product_id";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string ProductName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch/products/{ProductId}";

        // The request timestamp
        private static readonly DateTime RequestTimeStamp = DateTime.Now.ToUniversalTime();

        // The outdated request timestamp
        // request_time = datetime.datetime.now() - datetime.timedelta(days=1)

        private static ProductServiceClient GetProductServiceClient()
        {
            var productServiceClientBuilder = new ProductServiceClientBuilder 
            {
                Endpoint = Endpoint
            };

            var productServiceClient = productServiceClientBuilder.Build();
            return productServiceClient;
        }

        private static AddFulfillmentPlacesRequest GetAddFulfillmentRequest(string productName)
        {
            var addFulfillmentRequest = new AddFulfillmentPlacesRequest
            {
                Product = productName,
                Type = "pickup-in-store",
                AddTime = Timestamp.FromDateTime(RequestTimeStamp.AddMinutes(-1)),
                AllowMissing = true
            };

            string[] placeIds = { "store2", "store3", "store4" };

            addFulfillmentRequest.PlaceIds.AddRange(placeIds);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var addFulfillmentRequestJson = JsonConvert.SerializeObject(addFulfillmentRequest, jsonSerializeSettings);

            Console.WriteLine("\nAdd fulfillment places. request: \n" + addFulfillmentRequestJson);
            return addFulfillmentRequest;
        }

        private static void AddFulfillment(string productName)
        {
            var addFulfillmentRequest = GetAddFulfillmentRequest(productName);
            GetProductServiceClient().AddFulfillmentPlaces(addFulfillmentRequest);

            // This is a long running operation and its result is not immediately present with get operations,
            // thus we simulate wait with sleep method.
            Console.WriteLine("\nAdd fulfillment places. Wait 2 minutes:");
            Thread.Sleep(120000);
        }

        [Attributes.Example]
        public static Product PerformAddFulfillment()
        {
            CreateProduct.CreateRetailProductWithFulfillment(ProductId);
            AddFulfillment(ProductName);
            var inventoryProduct = GetProduct.GetRetailProduct(ProductName);
            // DeleteProduct.DeleteRetailProduct(ProductName);

            return inventoryProduct;
        }
    }
}
// [END retail_add_fulfillment_places]