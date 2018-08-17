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
using System.Reflection;
using System.Configuration;
using System.Transactions;
using PetShop.Model;
using PetShop.IDAL;

namespace PetShop.BLL
{
    /// <summary>
    /// A business component to manage the creation of orders
    /// Creation of an order requires a distributed transaction
    /// </summary>
    public class Order
    {
        // Using a static variable will cache the Order Insert strategy object for all instances of Order
        // We implement it this way to improve performance, so that the code will only load the instance once
        private static readonly PetShop.IBLLStrategy.IOrderStrategy s_orderInsertStrategy = LoadInsertStrategy();
        private static readonly PetShop.IMessaging.IOrder s_orderQueue = PetShop.MessagingFactory.QueueAccess.CreateOrder();

        // Get an instance of the Order DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IOrder s_dal = PetShop.DALFactory.DataAccess.CreateOrder();

        private const string CREDIT_CARD_ERROR_MSG = "Credit card processor thrown an exception.";

        /// <summary>
        /// A method to insert a new order into the system
        /// As part of the order creation the inventory will be reduced by the quantity ordered
        /// </summary>
        /// <param name="order">All the information about the order</param>
        public void Insert(OrderInfo order)
        {
            // Call credit card procesor
            ProcessCreditCard(order);

            // Insert the order (a)synchrounously based on configuration
            s_orderInsertStrategy.Insert(order);
        }

        /// <summary>
        /// Process credit card and get authorization number. 
        /// </summary>
        /// <param name="order">Order object, containing credit card information, total, etc.</param>
        private void ProcessCreditCard(OrderInfo order)
        {
            // In the real life environment here should be a call to the credit card processor API.
            // We simulate credit card processing and generate an authorization number.
            Random rnd = new Random();
            order.AuthorizationNumber = (int)(rnd.NextDouble() * int.MaxValue);

            // Check if authorisation succeded
            if (!order.AuthorizationNumber.HasValue)
                throw new ApplicationException(CREDIT_CARD_ERROR_MSG);
        }

        /// <summary>
        /// A method to read an order from the system
        /// </summary>
        /// <param name="orderId">Unique identifier for an order</param>
        /// <returns>All the information about the order</returns>
        public OrderInfo GetOrder(int orderId)
        {
            // Validate input
            if (orderId < 1)
                return null;

            // Return the order from the DAL
            return s_dal.GetOrder(orderId);
        }

        /// <summary>
        /// Method to process asynchronous order from the queue
        /// </summary>
        public OrderInfo ReceiveFromQueue(int timeout)
        {
            return s_orderQueue.Receive(timeout);
        }

        /// <summary>
        /// This method determines which Order Insert Strategy to use based on user's configuration.
        /// </summary>
        /// <returns>An instance of PetShop.IBLLStrategy.IOrderStrategy</returns>
        private static PetShop.IBLLStrategy.IOrderStrategy LoadInsertStrategy()
        {
            // Look up which strategy to use from config file
            string path = ConfigurationManager.AppSettings["OrderStrategyAssembly"];
            string className = ConfigurationManager.AppSettings["OrderStrategyClass"];

            // Using the evidence given in the config file load the appropriate assembly and class
            return (PetShop.IBLLStrategy.IOrderStrategy)Assembly.Load(path).CreateInstance(className);
        }
    }
}
