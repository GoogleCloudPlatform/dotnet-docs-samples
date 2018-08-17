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
    /// Business entity used to model an item.
    /// </summary>
    [Serializable]
    public class ItemInfo
    {
        // Internal member variables
        private readonly string _id;
        private readonly string _name;
        private readonly int _quantity;
        private readonly decimal _price;
        private readonly string _productName;
        private readonly string _image;
        private readonly string _categoryId;
        private readonly string _productId;

        public ItemInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <param name="name">Item Name</param>
        /// <param name="quantity">Quantity in stock</param>
        /// <param name="price">Price</param>
        /// <param name="productName">Parent product name</param>
        /// <param name="image">Item image</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="productId">Product Id</param>
        public ItemInfo(string id, string name, int quantity, decimal price, string productName, string image, string categoryId, string productId)
        {
            _id = id;
            _name = name;
            _quantity = quantity;
            _price = price;
            _productName = productName;
            _image = image;
            _categoryId = categoryId;
            _productId = productId;
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

        public string ProductName
        {
            get { return _productName; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public decimal Price
        {
            get { return _price; }
        }

        public string Image
        {
            get { return _image; }
        }

        public string CategoryId
        {
            get { return _categoryId; }
        }

        public string ProductId
        {
            get { return _productId; }
        }
    }
}
