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

namespace PetShop.ProfileDALFactory
{
    /// <summary>
    /// This class is implemented following the Abstract Factory pattern to create the ProfileDAL implementation
    /// specified from the configuration file
    /// </summary>
    public sealed class DataAccess
    {
        private static readonly string s_profilePath = ConfigurationManager.AppSettings["ProfileDAL"];

        public static PetShop.IProfileDAL.IPetShopProfileProvider CreatePetShopProfileProvider()
        {
            string className = s_profilePath + ".PetShopProfileProvider";
            return (PetShop.IProfileDAL.IPetShopProfileProvider)Assembly.Load(s_profilePath).CreateInstance(className);
        }
    }
}
