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

using System.Collections.Generic;
using PetShop.Model;
using PetShop.IDAL;

namespace PetShop.BLL
{
    /// <summary>
    /// A business component to manage categories
    /// </summary>
    public class Category
    {
        // Get an instance of the Category DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly ICategory s_dal = PetShop.DALFactory.DataAccess.CreateCategory();

        /// <summary>
        /// Method to get all categories
        /// </summary>										
        /// <returns>Generic List of CategoryInfo</returns>	   		
        public IList<CategoryInfo> GetCategories()
        {
            return s_dal.GetCategories();
        }

        /// <summary>
        /// Search for a category given it's unique identifier
        /// </summary>
        /// <param name="categoryId">Unique identifier for a Category</param>
        /// <returns>A Category business entity</returns>
        public CategoryInfo GetCategory(string categoryId)
        {
            // Validate input
            if (string.IsNullOrEmpty(categoryId))
                return null;

            // Use the dal to search by category Id
            return s_dal.GetCategory(categoryId);
        }
    }
}
