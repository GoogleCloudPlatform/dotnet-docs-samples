// Copyright (c) 2018 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using PetShop.Model;

namespace PetShop.IDAL
{
    /// <summary>
    /// Interface for the Product DAL
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Method to search products by category name
        /// </summary>
        /// <param name="category">Name of the category to search by</param>
        /// <returns>Interface to Model Collection Generic of search results</returns>
        IList<ProductInfo> GetProductsByCategory(string category);

        /// <summary>
        /// Method to search products by a set of keyword
        /// </summary>
        /// <param name="keywords">An array of keywords to search by</param>
        /// <returns>Interface to Model Collection Generic of search results</returns>
        IList<ProductInfo> GetProductsBySearch(string[] keywords);

        /// <summary>
        /// Query for a product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Interface to Model ProductInfo for requested product</returns>
        ProductInfo GetProduct(string productId);
    }
}
