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

// [START retail_crud_product]
// Create product in a catalog using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace grs_product
{
    public static class CrudProduct
    {
        private const string Endpoint = "retail.googleapis.com";

        private const string ProductId = "crud_product_id";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string DefaultBranchName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch";
        private static readonly string ProductName = $"{DefaultBranchName}/products/{ProductId}";

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

        // Get product for update
        private static Product GenerateProductForUpdate()
        {
            var updatedPriceInfo = new PriceInfo
            {
                Price = 20.0f,
                OriginalPrice = 25.5f,
                CurrencyCode = "EUR"
            };

            string[] categories = { "Updated Speakers and displays" };
            string[] brands = { "Updated Google" };

            var generatedProduct = new Product
            {
                Id = ProductId,
                Name = ProductName,
                Title = "Updated Nest Mini",
                Type = Product.Types.Type.Primary,
                PriceInfo = updatedPriceInfo,
                Availability = Product.Types.Availability.OutOfStock
            };

            generatedProduct.Categories.Add(categories);
            generatedProduct.Brands.Add(brands);

            return generatedProduct;
        }

        // Call the Retail API to create a product
        private static Product CreateRetailProduct()
        {
            var generatedProduct = GenerateProduct();
            var createProductRequest = new CreateProductRequest
            {
                Product = generatedProduct,
                ProductId = ProductId,
                Parent = DefaultBranchName
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var createProductRequestJson = JsonConvert.SerializeObject(createProductRequest, jsonSerializeSettings);

            Console.WriteLine("\nCreate product. request: \n\n" + createProductRequestJson);

            var createdProduct = GetProductServiceClient().CreateProduct(createProductRequest);

            var createProductJson = JsonConvert.SerializeObject(createdProduct, jsonSerializeSettings);

            Console.WriteLine("\nCreated product: \n" + createProductJson);

            return createdProduct;
        }

        // Call the Retail API to get a product
        private static Product GetRetailProduct()
        {
            var getProductRequest = new GetProductRequest
            {
                Name = ProductName
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var getProductRequestJson = JsonConvert.SerializeObject(getProductRequest, jsonSerializeSettings);

            Console.WriteLine("\nGet product. request: \n\n" + getProductRequestJson);

            var product = GetProductServiceClient().GetProduct(getProductRequest);

            var productJson = JsonConvert.SerializeObject(product, jsonSerializeSettings);

            Console.WriteLine("\nGet product. response: \n" + productJson);

            return product;
        }

        // Call the Retail API to update a product
        private static Product UpdateRetailProduct()
        {
            var generatedProductForUpdate = GenerateProductForUpdate();

            var updateProductRequest = new UpdateProductRequest
            {
                Product = generatedProductForUpdate,
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

            var updatedProduct = GetProductServiceClient().UpdateProduct(updateProductRequest);

            var updatedProductJson = JsonConvert.SerializeObject(updatedProduct, jsonSerializeSettings);

            Console.WriteLine("\nUpdated product: " + updatedProductJson);
            return updatedProduct;
        }

        // Call the Retail API to delete a product
        private static void DeleteRetailProduct()
        {
            var deleteProductRequest = new DeleteProductRequest
            {
                Name = ProductName
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var deleteProductRequestJson = JsonConvert.SerializeObject(deleteProductRequest, jsonSerializeSettings);

            Console.WriteLine("\nDelete product. request: \n\n" + deleteProductRequestJson);

            GetProductServiceClient().DeleteProduct(deleteProductRequest);

            Console.WriteLine($"\nDeleting product:\nProduct {ProductName} was deleted");
        }

        // Perform CRUD Product Operations
        [Attributes.Example]
        public static void PerformCRUDProductOperations()
        {
            // Call the methods
            CreateRetailProduct();
            GetRetailProduct();
            UpdateRetailProduct();
            DeleteRetailProduct();
        }
    }
}
// [END retail_crud_product]