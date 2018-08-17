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

using System.Reflection;
using System.Configuration;

namespace PetShop.DALFactory
{
    /// <summary>
    /// This class is implemented following the Abstract Factory pattern to create the DAL implementation
    /// specified from the configuration file
    /// </summary>
    public sealed class DataAccess
    {
        // Look up the DAL implementation we should be using
        private static readonly string s_path = ConfigurationManager.AppSettings["WebDAL"];
        private static readonly string s_orderPath = ConfigurationManager.AppSettings["OrdersDAL"];

        private DataAccess() { }

        public static PetShop.IDAL.ICategory CreateCategory()
        {
            string className = s_path + ".Category";
            return (PetShop.IDAL.ICategory)Assembly.Load(s_path).CreateInstance(className);
        }

        public static PetShop.IDAL.IInventory CreateInventory()
        {
            string className = s_path + ".Inventory";
            return (PetShop.IDAL.IInventory)Assembly.Load(s_path).CreateInstance(className);
        }

        public static PetShop.IDAL.IItem CreateItem()
        {
            string className = s_path + ".Item";
            return (PetShop.IDAL.IItem)Assembly.Load(s_path).CreateInstance(className);
        }

        public static PetShop.IDAL.IOrder CreateOrder()
        {
            string className = s_orderPath + ".Order";
            return (PetShop.IDAL.IOrder)Assembly.Load(s_orderPath).CreateInstance(className);
        }

        public static PetShop.IDAL.IProduct CreateProduct()
        {
            string className = s_path + ".Product";
            return (PetShop.IDAL.IProduct)Assembly.Load(s_path).CreateInstance(className);
        }
    }
}
