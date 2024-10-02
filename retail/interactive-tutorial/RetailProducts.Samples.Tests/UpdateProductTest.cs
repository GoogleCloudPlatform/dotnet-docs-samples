// Copyright 2021 Google Inc.

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
    public class UpdateProductTest
    {
        [Fact]
        public void TestUpdateProduct()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Create product.
            Product originalProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

            try
            {
                // Update original product.
                Product updatedProduct = UpdateProductSample.UpdateRetailProduct(originalProduct);

                Assert.Equal("Updated Nest Mini", updatedProduct.Title);
                Assert.Equal("EUR", updatedProduct.PriceInfo.CurrencyCode);
                Assert.Equal(20.0f, updatedProduct.PriceInfo.Price);
                Assert.Equal(25.5f, updatedProduct.PriceInfo.OriginalPrice);
                Assert.Equal(Product.Types.Availability.OutOfStock, updatedProduct.Availability);
            }
            finally
            {
                // Delete updated product.
                DeleteProductSample.DeleteRetailProduct(originalProduct.Name);
            }
        }
    }
}
