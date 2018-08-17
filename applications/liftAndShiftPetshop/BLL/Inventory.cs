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

using PetShop.IDAL;
using PetShop.Model;

namespace PetShop.BLL
{
    /// <summary>
    /// A business component to manage the inventory management for an item
    /// </summary>
    public class Inventory
    {
        // Get an instance of the Inventory DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IInventory s_dal = PetShop.DALFactory.DataAccess.CreateInventory();

        /// <summary>
        /// A method to get the current quanity in stock for an individual item
        /// </summary>
        /// <param name="itemId">A unique identifier for an item</param>
        /// <returns>Current quantity in stock</returns>
        public int CurrentQuantityInStock(string itemId)
        {
            // Validate input
            if (string.IsNullOrEmpty(itemId.Trim()))
                return 0;

            // Query the DAL for the current quantity in stock
            return s_dal.CurrentQtyInStock(itemId);
        }

        /// <summary>
        /// Reduce the current quantity in stock for an order's lineitems
        /// </summary>
        /// <param name="items">An array of order line items</param>
        public void TakeStock(LineItemInfo[] items)
        {
            // Reduce the stock level in the data store
            s_dal.TakeStock(items);
        }
    }
}
