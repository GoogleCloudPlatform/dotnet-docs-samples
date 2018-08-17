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
using System.Transactions;
using PetShop.IBLLStrategy;

namespace PetShop.BLL
{
    /// <summary>
    /// This is a synchronous implementation of IOrderStrategy 
    /// By implementing IOrderStrategy interface, the developer can add a new order insert strategy without re-compiling the whole BLL 
    /// </summary>
    public class OrderSynchronous : IOrderStrategy
    {
        // Get an instance of the Order DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly PetShop.IDAL.IOrder s_dal = PetShop.DALFactory.DataAccess.CreateOrder();

        /// <summary>
        /// Inserts the order and updates the inventory stock within a transaction.
        /// </summary>
        /// <param name="order">All information about the order</param>
        public void Insert(PetShop.Model.OrderInfo order)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
            {
                s_dal.Insert(order);

                // Update the inventory to reflect the current inventory after the order submission
                Inventory inventory = new Inventory();
                inventory.TakeStock(order.LineItems);

                // Calling Complete commits the transaction.
                // Excluding this call by the end of TransactionScope's scope will rollback the transaction
                ts.Complete();
            }
        }
    }
}
