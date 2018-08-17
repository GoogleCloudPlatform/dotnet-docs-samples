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
    public static class ItemDataProxy {

        private static readonly int itemTimeout = int.Parse(ConfigurationManager.AppSettings["ItemCacheDuration"]);
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying data has already been cached.
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>List of ItemInfo from Cache or Business component</returns>
        public static IList<ItemInfo> GetItemsByProduct(string productId) {

            Item item = new Item();

            if (!enableCaching)
                return item.GetItemsByProduct(productId);

            string key = "item_by_product_" + productId;
            IList<ItemInfo> data = (IList<ItemInfo>)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {
                // If the data is not in the cache then fetch the data from the business logic tier
                data = item.GetItemsByProduct(productId);

                // Create a AggregateCacheDependency object from the factory
                //AggregateCacheDependency cd = DependencyFacade.GetItemDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                //HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(itemTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying item has already been cached.
        /// </summary>
        /// <param name="itemId">Item Id</param>
        /// <returns>ItemInfo from Cache or Business component</returns>
        public static ItemInfo GetItem(string itemId) {

            Item item = new Item();

            if (!enableCaching)
                return item.GetItem(itemId);

            string key = "item_" + itemId;
            ItemInfo data = (ItemInfo)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {

                // If the data is not in the cache then fetch the data from the business logic tier
                data = item.GetItem(itemId);

                // Create a AggregateCacheDependency object from the factory
                AggregateCacheDependency cd = DependencyFacade.GetItemDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(itemTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return data;
        }
    }
}
