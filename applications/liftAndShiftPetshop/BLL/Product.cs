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

using System.Collections.Generic;
using PetShop.Model;
using PetShop.IDAL;

namespace PetShop.BLL
{
    /// <summary>
    /// A business component to manage products
    /// </summary>
    public class Product
    {
        // Get an instance of the Product DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IProduct s_dal = PetShop.DALFactory.DataAccess.CreateProduct();

        /// <summary>
        /// A method to retrieve products by category name
        /// </summary>
        /// <param name="category">The category name to search by</param>	
        /// <returns>A Generic List of ProductInfo</returns>
        public IList<ProductInfo> GetProductsByCategory(string category)
        {
            // Return new if the string is empty
            if (string.IsNullOrEmpty(category))
                return new List<ProductInfo>();

            // Run a search against the data store
            return s_dal.GetProductsByCategory(category);
        }

        /// <summary>
        /// A method to search products by keywords
        /// </summary>
        /// <param name="text">A list keywords delimited by a space</param>
        /// <returns>An interface to an arraylist of the search results</returns>
        public IList<ProductInfo> GetProductsBySearch(string text)
        {
            // Return new if the string is empty
            if (string.IsNullOrEmpty(text.Trim()))
                return new List<ProductInfo>();

            // Split the input text into individual words
            string[] keywords = text.Split();

            // Run a search against the data store
            return s_dal.GetProductsBySearch(keywords);
        }

        /// <summary>
        /// Query for a product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>ProductInfo object for requested product</returns>
        public ProductInfo GetProduct(string productId)
        {
            // Return empty product if the string is empty
            if (string.IsNullOrEmpty(productId))
                return new ProductInfo();

            // Get the product from the data store
            return s_dal.GetProduct(productId);
        }
    }
}
