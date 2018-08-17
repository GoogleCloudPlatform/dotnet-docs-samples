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
using PetShop.ICacheDependency;

namespace PetShop.CacheDependencyFactory
{
    public static class DependencyAccess
    {
        /// <summary>
        /// Method to create an instance of Category dependency implementation
        /// </summary>
        /// <returns>Category Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateCategoryDependency()
        {
            return LoadInstance("Category");
        }

        /// <summary>
        /// Method to create an instance of Product dependency implementation
        /// </summary>
        /// <returns>Product Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateProductDependency()
        {
            return LoadInstance("Product");
        }

        /// <summary>
        /// Method to create an instance of Item dependency implementation
        /// </summary>
        /// <returns>Item Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateItemDependency()
        {
            return LoadInstance("Item");
        }

        /// <summary>
        /// Common method to load dependency class from information provided from configuration file 
        /// </summary>
        /// <param name="className">Type of dependency</param>
        /// <returns>Concrete Dependency Implementation instance</returns>
        private static IPetShopCacheDependency LoadInstance(string className)
        {
            string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
            string fullyQualifiedClass = path + "." + className;

            // Using the evidence given in the config file load the appropriate assembly and class
            return (IPetShopCacheDependency)Assembly.Load(path).CreateInstance(fullyQualifiedClass);
        }
    }
}
