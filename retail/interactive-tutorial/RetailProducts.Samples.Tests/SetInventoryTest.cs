// Copyright 2022 Google Inc.

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

using Google.Cloud.Retail.V2;
using System;
using Xunit;

namespace RetailProducts.Samples.Tests
{
    public class SetInventoryTest
    {
        [Fact]
        public void TestSetInventory()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Create product.
            Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

            try
            {
                // Set inventory for product.
                SetInventorySample.SetProductInventory(createdProduct.Name);

                // Get product.
                Product inventoryProduct = GetProductSample.GetRetailProduct(createdProduct.Name);

                Assert.Contains("store1", inventoryProduct.FulfillmentInfo[0].PlaceIds);
                Assert.Contains("store2", inventoryProduct.FulfillmentInfo[0].PlaceIds);
                Assert.Equal("pickup-in-store", inventoryProduct.FulfillmentInfo[0].Type);
                Assert.Equal(15, inventoryProduct.PriceInfo.Price);
                Assert.Equal(Product.Types.Availability.InStock, inventoryProduct.Availability);
            }
            finally
            {
                // Delete product.
                DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
            }
        }
    }
}
