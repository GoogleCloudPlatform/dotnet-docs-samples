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
    /// Business entity used to model items in a shopping cart
    /// </summary>
    [Serializable]
    public class CartItemInfo
    {
        // Internal member variables
        private int _quantity = 1;
        private readonly string _itemId;
        private readonly string _name;
        private readonly string _type;
        private readonly decimal _price;
        private readonly string _categoryId;
        private readonly string _productId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CartItemInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="itemId">Id of item to add to cart</param></param>
        /// <param name="name">Name of item</param>
        /// <param name="qty">Quantity to purchase</param>
        /// <param name="price">Price of item</param>
        /// <param name="type">Item type</param>	  
        /// <param name="categoryId">Parent category id</param>
        /// <param name="productId">Parent product id</param>
        public CartItemInfo(string itemId, string name, int qty, decimal price, string type, string categoryId, string productId)
        {
            _itemId = itemId;
            _name = name;
            _quantity = qty;
            _price = price;
            _type = type;
            _categoryId = categoryId;
            _productId = productId;
        }

        // Properties
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public decimal Subtotal
        {
            get { return (decimal)(_quantity * _price); }
        }

        public string ItemId
        {
            get { return _itemId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }
        public decimal Price
        {
            get { return _price; }
        }

        public string CategoryId
        {
            get
            {
                return _categoryId;
            }
        }
        public string ProductId
        {
            get
            {
                return _productId;
            }
        }
    }
}
