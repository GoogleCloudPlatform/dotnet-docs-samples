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
    public class CreateProductTest
    {

        [Fact]
        public void TestCreateProduct()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Create product.
            Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

            Assert.Equal("Nest Mini", createdProduct.Title);
            Assert.Equal("USD", createdProduct.PriceInfo.CurrencyCode);
            Assert.Equal(30.0f, createdProduct.PriceInfo.Price);
            Assert.Equal(35.5f, createdProduct.PriceInfo.OriginalPrice);
            Assert.Equal(Product.Types.Availability.InStock, createdProduct.Availability);

            // Delete created product.
            DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
        }
    }
}
