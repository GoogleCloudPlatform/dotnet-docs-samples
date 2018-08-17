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
    /// A business component to manage product items
    /// </summary>
    public class Item
    {
        // Get an instance of the Item DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IItem s_dal = PetShop.DALFactory.DataAccess.CreateItem();

        /// <summary>
        /// A method to list items by productId
        /// Every item is associated with a parent product
        /// </summary>
        /// <param name="productId">The productId to search by</param> 
        /// <returns>A Generic List of ItemInfo</returns>
        public IList<ItemInfo> GetItemsByProduct(string productId)
        {
            // Validate input
            if (string.IsNullOrEmpty(productId))
                return new List<ItemInfo>();

            // Use the dal to search by productId
            return s_dal.GetItemsByProduct(productId);
        }

        /// <summary>
        /// Search for an item given it's unique identifier
        /// </summary>
        /// <param name="itemId">Unique identifier for an item</param>
        /// <returns>An Item business entity</returns>
        public ItemInfo GetItem(string itemId)
        {
            // Validate input
            if (string.IsNullOrEmpty(itemId))
                return null;

            // Use the dal to search by ItemId
            return s_dal.GetItem(itemId);
        }
    }
}
