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
    /// Business entity used to model credit card information.
    /// </summary>
    [Serializable]
    public class CreditCardInfo
    {
        // Internal member variables
        private string _cardType;
        private string _cardNumber;
        private string _cardExpiration;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CreditCardInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="cardType">Card type, e.g. Visa, Master Card, American Express</param>
        /// <param name="cardNumber">Number on the card</param>
        /// <param name="cardExpiration">Expiry Date, form  MM/YY</param>
        public CreditCardInfo(string cardType, string cardNumber, string cardExpiration)
        {
            _cardType = cardType;
            _cardNumber = cardNumber;
            _cardExpiration = cardExpiration;
        }

        // Properties
        public string CardType
        {
            get { return _cardType; }
            set { _cardType = value; }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; }
        }

        public string CardExpiration
        {
            get { return _cardExpiration; }
            set { _cardExpiration = value; }
        }
    }
}
