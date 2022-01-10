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

// [START retail_set_inventory]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;

namespace grs_product
{
    public static class SetInventory
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string ProductId = "inventory_test_product_id";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string ProductName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch/products/{ProductId}";

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

        private static Product GetProductWithInventoryInfo(string productName)
        {
            var priceInfo = new PriceInfo
            {
                Price = 15.0f,
                OriginalPrice = 20.0f,
                Cost = 8.0f,
                CurrencyCode = "USD"
            };

            var fulfillmentInfo = new FulfillmentInfo
            {
                Type = "pickup-in-store"
            };

            string[] placeIds = { "store1", "store2" };

            fulfillmentInfo.PlaceIds.Add(placeIds);

            var product = new Product
            {
                Name = productName,
                PriceInfo = priceInfo,
                Availability = Product.Types.Availability.InStock
            };

            product.FulfillmentInfo.Add(fulfillmentInfo);

            return product;
        }

        // Get set inventory request
        private static SetInventoryRequest GetSetInventoryRequest(string productName)
        {
            // The request timestamp
            DateTime requestTimeStamp = DateTime.Now.ToUniversalTime();

            // The out-of-order request timestamp
            // request_time = datetime.datetime.now() - datetime.timedelta(days=1)

            string[] paths = { "price_info", "availability", "fulfillment_info", "available_quantity" };
            var setMask = new FieldMask();
            setMask.Paths.AddRange(paths);

            var setInventoryRequest = new SetInventoryRequest
            {
                Inventory = GetProductWithInventoryInfo(productName),
                SetTime = Timestamp.FromDateTime(requestTimeStamp),
                AllowMissing = true,
                SetMask = setMask
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var setInventoryRequestJson = JsonConvert.SerializeObject(setInventoryRequest, jsonSerializeSettings);

            Console.WriteLine("\nSet inventory. request: \n\n" + setInventoryRequestJson);
            return setInventoryRequest;
        }

        // Call the Retail API to set product inventory
        private static void SetProductInventory(string productName)
        {
            var setInventoryRequest = GetSetInventoryRequest(productName);
            GetProductServiceClient().SetInventory(setInventoryRequest);

            // This is a long running operation and its result is not immediately present with get operations,
            // thus we simulate wait with sleep method.
            Console.WriteLine("\nSet inventory. Wait 50 seconds:\n");
            Thread.Sleep(50000);
        }

        // Perform inventory setting
        [Attributes.Example]
        public static Product PerformSetInventoryOperation()
        {
            CreateProduct.CreateRetailProductWithFulfillment(ProductId);
            SetProductInventory(ProductName);
            var inventoryProduct = GetProduct.GetRetailProduct(ProductName);
            DeleteProduct.DeleteRetailProduct(ProductName);

            return inventoryProduct;
        }
    }
}
// [END retail_set_inventory]