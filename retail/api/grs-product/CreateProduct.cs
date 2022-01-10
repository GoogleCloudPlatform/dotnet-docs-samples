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

// [START retail_create_product]
// Create product in a catalog using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace grs_product
{
    public static class CreateProduct
    {
        private const string Endpoint = "retail.googleapis.com";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string DefaultBranchName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch";

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

        // Generate product
        private static Product GenerateProduct()
        {
            var priceInfo = new PriceInfo
            {
                Price = 30.0f,
                OriginalPrice = 35.5f,
                CurrencyCode = "USD"
            };

            string[] brands = { "Google" };
            string[] categories = { "Speakers and displays" };

            var generatedProduct = new Product
            {
                Title = "Nest Mini",
                Type = Product.Types.Type.Primary,
                PriceInfo = priceInfo,
                Availability = Product.Types.Availability.InStock
            };

            generatedProduct.Categories.Add(categories);
            generatedProduct.Brands.Add(brands);

            return generatedProduct;
        }

        // Generate fulfillment product
        private static Product GenerateProductWithFulfillment()
        {
            var generatedProduct = GenerateProduct();

            string[] placeIds = { "store0", "store1" };

            var fulfillmentInfo = new FulfillmentInfo
            {
                Type = "pickup-in-store"
            };

            fulfillmentInfo.PlaceIds.AddRange(placeIds);

            generatedProduct.FulfillmentInfo.Add(fulfillmentInfo);

            return generatedProduct;
        }

        // Get create product request
        private static CreateProductRequest GetCreateProductRequest(Product productToCreate, string productId)
        {
            var createProductRequest = new CreateProductRequest
            {
                Product = productToCreate,
                ProductId = productId,
                Parent = DefaultBranchName
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var createProductRequestJson = JsonConvert.SerializeObject(createProductRequest, jsonSerializeSettings);

            Console.WriteLine("Create product. request: \n\n" + createProductRequestJson);

            return createProductRequest;
        }

        // Call the Retail API to create a product
        public static Product CreateRetailProduct(string productId)
        {
            var generatedProduct = GenerateProduct();
            var createProductRequest = GetCreateProductRequest(generatedProduct, productId);

            var createdProduct = GetProductServiceClient().CreateProduct(createProductRequest);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var createdProductJson = JsonConvert.SerializeObject(createdProduct, jsonSerializeSettings);

            Console.WriteLine("\nCreated product: \n" + createdProductJson);

            return createdProduct;
        }

        // Call the Retail API to create a product with fulfillment
        public static Product CreateRetailProductWithFulfillment(string productId)
        {
            var generatedProduct = GenerateProductWithFulfillment();
            var createProductRequest = GetCreateProductRequest(generatedProduct, productId);

            var createdProduct = GetProductServiceClient().CreateProduct(createProductRequest);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var createdProductJson = JsonConvert.SerializeObject(createdProduct, jsonSerializeSettings);

            Console.WriteLine("\nCreated product: \n" + createdProductJson);

            return createdProduct;
        }

        // Perform product creation
        [Attributes.Example]
        public static Product PerformCreateProductOperation()
        {
            // Create product
            var createdProduct = CreateRetailProduct(GeneratedProductId);

            // Delete created product
            DeleteProduct.DeleteRetailProduct(createdProduct.Name);

            return createdProduct; 
        }
    }
}
// [END retail_create_product]