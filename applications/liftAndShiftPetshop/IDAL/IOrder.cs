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

//References to PetShop specific libraries
//PetShop busines entity library
using PetShop.Model;

namespace PetShop.IDAL
{
    /// <summary>
    /// Interface for the Order DAL
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Method to insert an order header
        /// </summary>
        /// <param name="order">Business entity representing the order</param>
        /// <returns>OrderId</returns>
        void Insert(OrderInfo order);

        /// <summary>
        /// Reads the order information for a given orderId
        /// </summary>
        /// <param name="orderId">Unique identifier for an order</param>
        /// <returns>Business entity representing the order</returns>
        OrderInfo GetOrder(int orderId);
    }
}
