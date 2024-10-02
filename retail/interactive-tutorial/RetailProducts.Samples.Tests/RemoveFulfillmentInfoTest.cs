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
    public class RemoveFulfillmentInfoTest
    {
        [Fact]
        public void TestRemoveFulfillmentPlaces()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Create product.
            Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

            try
            {
                const string expectedPlaceId = "store0";

                Assert.Contains(expectedPlaceId, createdProduct.FulfillmentInfo[0].PlaceIds);

                // Remove fulfillment from product.
                RemoveFulfillmentPlaces.RemoveFulfillment(createdProduct.Name);

                // Get product.
                Product productWithUpdatedFulfillment = GetProductSample.GetRetailProduct(createdProduct.Name);

                Assert.DoesNotContain(expectedPlaceId, productWithUpdatedFulfillment.FulfillmentInfo[0].PlaceIds);
            }
            finally
            {
                // Delete product.
                DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
            }
        }
    }
}
