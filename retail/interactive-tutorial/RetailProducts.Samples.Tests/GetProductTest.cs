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
using Xunit;

namespace RetailProducts.Samples.Tests
{
    public class GetProductTest
    {
        [Fact]
        public void TestGetProduct()
        {
            const string ExpectedProductTitle = "Nest Mini";
            const string ExpectedCurrencyCode = "USD";
            const float ExpectedProductPrice = 30.0f;
            const float ExpectedProductOriginalPrice = 35.5f;
            const Product.Types.Availability ExpectedProductAvailability = Product.Types.Availability.InStock;

            var retrievedProduct = GetProductTutorial.PerformGetProductOperation();

            Assert.Equal(ExpectedProductTitle, retrievedProduct.Title);
            Assert.Equal(ExpectedCurrencyCode, retrievedProduct.PriceInfo.CurrencyCode);
            Assert.Equal(ExpectedProductPrice, retrievedProduct.PriceInfo.Price);
            Assert.Equal(ExpectedProductOriginalPrice, retrievedProduct.PriceInfo.OriginalPrice);
            Assert.Equal(ExpectedProductAvailability, retrievedProduct.Availability);
        }
    }
}
