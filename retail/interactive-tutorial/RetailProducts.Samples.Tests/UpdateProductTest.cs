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
            var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Create product.
            Product originalProduct = CreateProductSample.CreateRetailProduct(projectId);

            try
            {
                const string ExpectedProductTitle = "Updated Nest Mini";
                const string ExpectedCurrencyCode = "EUR";
                const float ExpectedProductPrice = 20.0f;
                const float ExpectedProductOriginalPrice = 25.5f;
                const Product.Types.Availability ExpectedProductAvailability = Product.Types.Availability.OutOfStock;

                var sample = new UpdateProductSample();

                // Update original product.
                Product updatedProduct = sample.PerformUpdateProductOperation(originalProduct);

                Assert.Equal(ExpectedProductTitle, updatedProduct.Title);
                Assert.Equal(ExpectedCurrencyCode, updatedProduct.PriceInfo.CurrencyCode);
                Assert.Equal(ExpectedProductPrice, updatedProduct.PriceInfo.Price);
                Assert.Equal(ExpectedProductOriginalPrice, updatedProduct.PriceInfo.OriginalPrice);
                Assert.Equal(ExpectedProductAvailability, updatedProduct.Availability);
            }
            finally
            {
                // Delete updated product.
                DeleteProductSample.DeleteRetailProduct(originalProduct.Name);
            }
        }
    }
}