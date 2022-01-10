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

// [START retail_update_product]
// Update product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace grs_product
{
    public static class UpdateProduct
    {
        private const string Endpoint = "retail.googleapis.com";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
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

        // Get product for update
        private static Product GenerateProductForUpdate(string productId)
        {
            var updatedPriceInfo = new PriceInfo
            {
                Price = 20.0f,
                OriginalPrice = 25.5f,
                CurrencyCode = "EUR"
            };

            var generatedProduct = new Product
            {
                Id = productId,
                Name = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch/products/{productId}",
                Title = "Updated Nest Mini",
                Type = Product.Types.Type.Primary,
                PriceInfo = updatedPriceInfo,
                Availability = Product.Types.Availability.OutOfStock
            };

            generatedProduct.Categories.Add("Updated Speakers and displays");
            generatedProduct.Brands.Add("Updated Google");

            return generatedProduct;
        }

        // Get update product request
        private static UpdateProductRequest GetUpdateProductRequest(Product productToUpdate)
        {
            var updateProductRequest = new UpdateProductRequest
            {
                Product = productToUpdate,
                AllowMissing = true
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var updateProductRequestJson = JsonConvert.SerializeObject(updateProductRequest, jsonSerializeSettings);

            Console.WriteLine("\nUpdate product. request: \n\n" + updateProductRequestJson);
            return updateProductRequest;
        }

        // Call the Retail API to update a product
        public static Product UpdateRetailProduct(Product originalProduct)
        {
            var productForUpdate = GenerateProductForUpdate(originalProduct.Id);
            var updateProductRequest = GetUpdateProductRequest(productForUpdate);
            var updatedProduct = GetProductServiceClient().UpdateProduct(updateProductRequest);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var updatedProductJson = JsonConvert.SerializeObject(updatedProduct, jsonSerializeSettings);

            Console.WriteLine("\nUpdated product: " + updatedProductJson);
            return updatedProduct;
        }

        // Perform product update
        [Attributes.Example]
        public static Product PerformUpdateProductOperation()
        {
            // Create product
            var originalProduct = CreateProduct.CreateRetailProduct(GeneratedProductId);

            // Update created product
            var updatedProduct = UpdateRetailProduct(originalProduct);

            // Delete updated product
            DeleteProduct.DeleteRetailProduct(updatedProduct.Name);

            return updatedProduct;
        }
    }
}
// [END retail_update_product]