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

// [START retail_get_product]
// Get product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace grs_product
{
    public static class GetProduct
    {
        private const string Endpoint = "retail.googleapis.com";

        private static readonly Random Random = new ();
        private static readonly string GeneratedProductId = RandomAlphanumericString(14);

        public static string RandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        // Get product service client
        private static ProductServiceClient GetProductServiceClient()
        {
            var productServiceClientBuilder = new ProductServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var productServiceClient = productServiceClientBuilder.Build();
            return productServiceClient;
        }

        // Get create product request
        private static GetProductRequest GetProductRequest(string productName)
        {
            var getProductRequest = new GetProductRequest
            {
                Name = productName
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var getProductRequestJson = JsonConvert.SerializeObject(getProductRequest, jsonSerializeSettings);

            Console.WriteLine("\nGet product. request: \n\n" + getProductRequestJson);
            return getProductRequest;
        }

        // Call the Retail API to get a product
        public static Product GetRetailProduct(string productName)
        {
            var getProductRequest = GetProductRequest(productName);

            var product = GetProductServiceClient().GetProduct(getProductRequest);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var productJson = JsonConvert.SerializeObject(product, jsonSerializeSettings);

            Console.WriteLine("\nGet product. response: \n" + productJson);

            return product;
        }

        // Perform product retrieval
        [Attributes.Example]
        public static void PerformGetProductOperation()
        {
            // Create product
            var createdProduct = CreateProduct.CreateRetailProduct(GeneratedProductId);

            // Get created product
            var retrievedProduct = GetRetailProduct(createdProduct.Name);

            // Delete created product
            DeleteProduct.DeleteRetailProduct(retrievedProduct.Name);
        }
    }
}
// [END retail_get_product]