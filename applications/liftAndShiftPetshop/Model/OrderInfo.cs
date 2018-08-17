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
    /// Business entity used to model an order
    /// </summary>
    [Serializable]
    public class OrderInfo
    {
        // Internal member variables
        private int _orderId;
        private DateTime _date;
        private string _userId;
        private CreditCardInfo _creditCard;
        private AddressInfo _billingAddress;
        private AddressInfo _shippingAddress;
        private decimal _orderTotal;
        private LineItemInfo[] _lineItems;
        private Nullable<int> _authorizationNumber;

        /// <summary>
        /// Default constructor
        /// This is required by web services serialization mechanism
        /// </summary>
        public OrderInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="orderId">Unique identifier</param>
        /// <param name="date">Order date</param>
        /// <param name="userId">User placing order</param>
        /// <param name="creditCard">Credit card used for order</param>
        /// <param name="billing">Billing address for the order</param>
        /// <param name="shipping">Shipping address for the order</param>
        /// <param name="total">Order total value</param>
		/// <param name="line">Ordered items</param>
		/// <param name="authorization">Credit card authorization number</param>
		public OrderInfo(int orderId, DateTime date, string userId, CreditCardInfo creditCard, AddressInfo billing, AddressInfo shipping, decimal total, LineItemInfo[] line, Nullable<int> authorization)
        {
            _orderId = orderId;
            _date = date;
            _userId = userId;
            _creditCard = creditCard;
            _billingAddress = billing;
            _shippingAddress = shipping;
            _orderTotal = total;
            _lineItems = line;
            _authorizationNumber = authorization;
        }

        // Properties
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public CreditCardInfo CreditCard
        {
            get { return _creditCard; }
            set { _creditCard = value; }
        }

        public AddressInfo BillingAddress
        {
            get { return _billingAddress; }
            set { _billingAddress = value; }
        }

        public AddressInfo ShippingAddress
        {
            get { return _shippingAddress; }
            set { _shippingAddress = value; }
        }

        public decimal OrderTotal
        {
            get { return _orderTotal; }
            set { _orderTotal = value; }
        }

        public LineItemInfo[] LineItems
        {
            get { return _lineItems; }
            set { _lineItems = value; }
        }

        public Nullable<int> AuthorizationNumber
        {
            get { return _authorizationNumber; }
            set { _authorizationNumber = value; }
        }
    }
}
