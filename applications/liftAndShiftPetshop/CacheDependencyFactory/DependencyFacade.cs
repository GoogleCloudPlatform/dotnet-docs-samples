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

using System.Configuration;
using System.Web.Caching;
using System.Collections.Generic;
using PetShop.ICacheDependency;

namespace PetShop.CacheDependencyFactory
{
    /// <summary>
    /// This class is provided to ease the usage of DependencyFactory from the client.
    /// It's main usage is to determine whether to invoke the DependencyFactory.  
    /// When no assembly is specified under CacheDependencyAssembly section in configuraion file, 
    /// then this class will return null
    /// Notice that this assembly reference System.Web
    /// </summary>
    public static class DependencyFacade
    {
        private static readonly string s_path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];

        public static AggregateCacheDependency GetCategoryDependency()
        {
            if (!string.IsNullOrEmpty(s_path))
                return DependencyAccess.CreateCategoryDependency().GetDependency();
            else
                return null;
        }

        public static AggregateCacheDependency GetProductDependency()
        {
            if (!string.IsNullOrEmpty(s_path))
                return DependencyAccess.CreateProductDependency().GetDependency();
            else
                return null;
        }

        public static AggregateCacheDependency GetItemDependency()
        {
            if (!string.IsNullOrEmpty(s_path))
                return DependencyAccess.CreateItemDependency().GetDependency();
            else
                return null;
        }
    }
}
