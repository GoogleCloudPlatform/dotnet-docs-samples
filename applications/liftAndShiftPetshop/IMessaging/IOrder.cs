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

using PetShop.Model;

namespace PetShop.IMessaging
{
    /// <summary>
    /// This is the interface that the MessagingFactory returns.
    /// To create a new implementation of Order Messaging, developers 
    /// need to implement this interface to comply with Abstract Factory Design Pattern
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Method to retrieve order information from a messaging queue
        /// </summary>
        /// <returns>All information about an order</returns>
        OrderInfo Receive();

        /// <summary>
        /// Method to retrieve order information from a messaging queue
        /// </summary>
        /// <returns>All information about an order</returns>
        OrderInfo Receive(int timeout);

        /// <summary>
        /// Method to send an order to a message queue for later processing
        /// </summary>
        /// <param name="body">All information about an order</param>
        void Send(OrderInfo orderMessage);
    }
}
