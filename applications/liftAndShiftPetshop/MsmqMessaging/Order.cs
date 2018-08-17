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
using System.Configuration;
using System.Messaging;
using PetShop.Model;

namespace PetShop.MSMQMessaging
{
    /// <summary>
    /// This class is an implementation for sending and receiving orders to and from MSMQ
    /// </summary>
    public class Order : PetShopQueue, PetShop.IMessaging.IOrder
    {
        // Path example - FormatName:DIRECT=OS:MyMachineName\Private$\OrderQueueName
        private static readonly string s_queuePath = ConfigurationManager.AppSettings["OrderQueuePath"];
        private static readonly int s_queueTimeout = 20;

        public Order() : base(s_queuePath, s_queueTimeout)
        {
            // Set the queue to use Binary formatter for smaller foot print and performance
            queue.Formatter = new BinaryMessageFormatter();
        }

        /// <summary>
        /// Method to retrieve order messages from Pet Shop Message Queue
        /// </summary>
        /// <returns>All information for an order</returns>
        public new OrderInfo Receive()
        {
            // This method involves in distributed transaction and need Automatic Transaction type
            base.transactionType = MessageQueueTransactionType.Automatic;
            return (OrderInfo)((Message)base.Receive()).Body;
        }

        public OrderInfo Receive(int timeout)
        {
            base.timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeout));
            return Receive();
        }

        /// <summary>
        /// Method to send asynchronous order to Pet Shop Message Queue
        /// </summary>
        /// <param name="orderMessage">All information for an order</param>
        public void Send(OrderInfo orderMessage)
        {
            // This method does not involve in distributed transaction and optimizes performance using Single type
            base.transactionType = MessageQueueTransactionType.Single;
            base.Send(orderMessage);
        }
    }
}
