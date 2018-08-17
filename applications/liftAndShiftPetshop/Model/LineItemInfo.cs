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
    /// Business entity used to model an order line item.
    /// </summary>
    [Serializable]
    public class LineItemInfo
    {
        // Internal member variables
        private string _id;
        private string _productName;
        private int _line;
        private int _quantity;
        private decimal _price;

        /// <summary>
        /// Default constructor
        /// This is required by web services serialization mechanism
        /// </summary>
        public LineItemInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <param name="line">Line number</param>
        /// <param name="qty">Quanity in order</param>
        /// <param name="price">Order item price</param>
        public LineItemInfo(string id, string name, int line, int qty, decimal price)
        {
            _id = id;
            _productName = name;
            _line = line;
            _price = price;
            _quantity = qty;
        }

        // Properties
        public string ItemId
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _productName; }
            set { _productName = value; }
        }

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public decimal Subtotal
        {
            get { return _price * _quantity; }
        }
    }
}
