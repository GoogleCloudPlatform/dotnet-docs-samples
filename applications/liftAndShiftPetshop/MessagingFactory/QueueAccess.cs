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

namespace PetShop.MessagingFactory
{
    /// <summary>
    /// This class is implemented following the Abstract Factory pattern to create the Order
    /// Messaging implementation specified from the configuration file
    /// </summary>
    public sealed class QueueAccess
    {
        // Look up the Messaging implementation we should be using
        private static readonly string s_path = ConfigurationManager.AppSettings["OrderMessaging"];

        private QueueAccess() { }

        public static PetShop.IMessaging.IOrder CreateOrder()
        {
            string className = s_path + ".Order";
            return (PetShop.IMessaging.IOrder)Assembly.Load(s_path).CreateInstance(className);
        }
    }
}
