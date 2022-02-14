﻿// Copyright 2021 Google Inc. All Rights Reserved.
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
    public class SearchWithQueryExpansionTest
    {
        [Fact]
        public void TestSearchWithQueryExpansion()
        {
            const string ExpectedProductTitle = "Google Youth Hero Tee Grey";

            var searchResultPages = SearchWithQueryExpansionTutorial.Search();

            var topPages = searchResultPages.Take(3).ToList();
            var firstPage = topPages[0];
            var thirdPage = topPages[2];

            Assert.Contains(firstPage, result => result.Product.Title.Contains(ExpectedProductTitle));
            Assert.Contains(thirdPage, result => !result.Product.Title.Contains(ExpectedProductTitle));
        }
    }
}