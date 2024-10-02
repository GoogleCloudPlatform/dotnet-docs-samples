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

using System.Linq;
using Xunit;

namespace RetailSearch.Samples.Tests
{
    public class SearchWithOrderingTest
    {
        [Fact]
        public void TestSearchWithOrdering()
        {
            var firstPage = SearchWithOrderingTutorial.Search().First();

            var firstProduct = firstPage.Results[0].Product;
            var secondProduct = firstPage.Results[1].Product;

            Assert.True(firstProduct.PriceInfo.Price >= secondProduct.PriceInfo.Price);
        }
    }
}
