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
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Configuration;
using PetShop.Model;
using PetShop.BLL;
using PetShop.CacheDependencyFactory;

namespace PetShop.Web {
    public static class ProductDataProxy {

        private static readonly int productTimeout = int.Parse(ConfigurationManager.AppSettings["ProductCacheDuration"]);
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying data has already been cached.
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>List of ProductInfo from Cache or Business component</returns>
        public static IList<ProductInfo> GetProductsByCategory(string category) {

            Product product = new Product();

            if (!enableCaching)
                return product.GetProductsByCategory(category);

            string key = "product_by_category_" + category;
            IList<ProductInfo> data = (IList<ProductInfo>)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {

                // If the data is not in the cache then fetch the data from the business logic tier
                data = product.GetProductsByCategory(category);

                // Create a AggregateCacheDependency object from the factory
                //AggregateCacheDependency cd = DependencyFacade.GetProductDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                //HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(productTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying search result has already been cached.
        /// </summary>
        /// <param name="text">Search Text</param>
        /// <returns>List of ProductInfo from Cache or Business component</returns>
        public static IList<ProductInfo> GetProductsBySearch(string text) {

            Product product = new Product();

            if (!enableCaching)
                return product.GetProductsBySearch(text);

            string key = "product_search_" + text;
            IList<ProductInfo> data = (IList<ProductInfo>)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {

                // If the data is not in the cache then fetch the data from the business logic tier
                data = product.GetProductsBySearch(text);

                // Create a AggregateCacheDependency object from the factory
                AggregateCacheDependency cd = DependencyFacade.GetProductDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(productTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying product has already been cached.
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>ProductInfo from Cache or Business component</returns>
        public static ProductInfo GetProduct(string productId) {

            Product product = new Product();

            if (!enableCaching)
                return product.GetProduct(productId);

            string key = "product_" + productId;
            ProductInfo data = (ProductInfo)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {

                // If the data is not in the cache then fetch the data from the business logic tier
                data = product.GetProduct(productId);

                // Create a AggregateCacheDependency object from the factory
                AggregateCacheDependency cd = DependencyFacade.GetProductDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(productTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return data;
        }
    }
}
