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

//References to PetShop specific libraries
//PetShop busines entity library
using PetShop.Model;

namespace PetShop.IDAL
{
    /// <summary>
    /// Interface to the Item DAL
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Search items by productId
        /// </summary>
        /// <param name="productId">ProductId to search for</param>
        /// <returns>Interface to Model Collection Generic of the results</returns>
        IList<ItemInfo> GetItemsByProduct(string productId);

        /// <summary>
        /// Get information on a specific item
        /// </summary>
        /// <param name="itemId">Unique identifier for an item</param>
        /// <returns>Business Entity representing an item</returns>
        ItemInfo GetItem(string itemId);
    }
}
