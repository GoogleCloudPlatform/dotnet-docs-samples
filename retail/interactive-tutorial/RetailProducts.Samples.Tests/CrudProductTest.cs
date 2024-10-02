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
    public class CrudProductTest
    {
        [Fact]
        public void TestCrudProduct()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

            // Create product.
            Product createdProduct = CrudProductSample.CreateRetailProduct(defaultBranchName);

            try
            {
                const string ExpectedProductTitle = "Nest Mini";
                const string ExpectedCurrencyCode = "USD";
                const float ExpectedProductPrice = 30.0f;
                const float ExpectedProductOriginalPrice = 35.5f;
                const Product.Types.Availability ExpectedProductAvailability = Product.Types.Availability.InStock;

                const string ExpectedUpdatedProductTitle = "Updated Nest Mini";
                const string ExpectedUpdatedCurrencyCode = "EUR";
                const float ExpectedUpdatedProductPrice = 20.0f;
                const float ExpectedUpdatedProductOriginalPrice = 25.5f;
                const Product.Types.Availability ExpectedUpdatedProductAvailability = Product.Types.Availability.OutOfStock;

                Assert.Equal(ExpectedProductTitle, createdProduct.Title);
                Assert.Equal(ExpectedCurrencyCode, createdProduct.PriceInfo.CurrencyCode);
                Assert.Equal(ExpectedProductPrice, createdProduct.PriceInfo.Price);
                Assert.Equal(ExpectedProductOriginalPrice, createdProduct.PriceInfo.OriginalPrice);
                Assert.Equal(ExpectedProductAvailability, createdProduct.Availability);

                // Get product.
                Product retrievedProduct = CrudProductSample.GetRetailProduct(createdProduct.Name);

                Assert.Equal(ExpectedProductTitle, retrievedProduct.Title);
                Assert.Equal(ExpectedCurrencyCode, retrievedProduct.PriceInfo.CurrencyCode);
                Assert.Equal(ExpectedProductPrice, retrievedProduct.PriceInfo.Price);
                Assert.Equal(ExpectedProductOriginalPrice, retrievedProduct.PriceInfo.OriginalPrice);
                Assert.Equal(ExpectedProductAvailability, retrievedProduct.Availability);

                // Update product.
                Product updatedProduct = CrudProductSample.UpdateRetailProduct(retrievedProduct.Name);

                Assert.Equal(ExpectedUpdatedProductTitle, updatedProduct.Title);
                Assert.Equal(ExpectedUpdatedCurrencyCode, updatedProduct.PriceInfo.CurrencyCode);
                Assert.Equal(ExpectedUpdatedProductPrice, updatedProduct.PriceInfo.Price);
                Assert.Equal(ExpectedUpdatedProductOriginalPrice, updatedProduct.PriceInfo.OriginalPrice);
                Assert.Equal(ExpectedUpdatedProductAvailability, updatedProduct.Availability);
            }
            finally
            {
                // Delete product.
                CrudProductSample.DeleteRetailProduct(createdProduct.Name);
            }
        }
    }
}
