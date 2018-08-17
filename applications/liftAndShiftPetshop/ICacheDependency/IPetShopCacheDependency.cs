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

using System.Web.Caching;

namespace PetShop.ICacheDependency
{
    /// <summary>
    /// This is the interface that the DependencyFactory (Factory Pattern) returns.
    /// Developers could implement this interface to add different types of Cache Dependency to Pet Shop.
    /// </summary>
    public interface IPetShopCacheDependency
    {
        /// <summary>
        /// Method to create the appropriate implementation of Cache Dependency
        /// </summary>
        /// <returns>CacheDependency object(s) embedded in AggregateCacheDependency</returns>
        AggregateCacheDependency GetDependency();
    }
}
