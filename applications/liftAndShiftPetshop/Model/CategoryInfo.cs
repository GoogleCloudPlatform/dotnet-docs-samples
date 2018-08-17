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

namespace PetShop.Model
{
    /// <summary>
    /// Business entity used to model a product
    /// </summary>
    [Serializable]
    public class CategoryInfo
    {
        // Internal member variables
        private readonly string _id;
        private readonly string _name;
        private readonly string _description;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CategoryInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <param name="name">Category Name</param>
        /// <param name="description">Category Description</param>
        public CategoryInfo(string id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        // Properties
        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}
