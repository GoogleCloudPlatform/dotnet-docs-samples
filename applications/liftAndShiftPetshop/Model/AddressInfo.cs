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
    /// Business entity used to model addresses
    /// </summary>
    [Serializable]
    public class AddressInfo
    {
        // Internal member variables
        private string _firstName;
        private string _lastName;
        private string _address1;
        private string _address2;
        private string _city;
        private string _state;
        private string _zip;
        private string _country;
        private string _phone;
        private string _email;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddressInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <param name="address1">Address line 1</param>
        /// <param name="address2">Address line 2</param>
        /// <param name="city">City</param>
        /// <param name="state">State</param>
        /// <param name="zip">Postal Code</param>
        /// <param name="country">Country</param>
        /// <param name="phone">Phone number at this address</param>
        /// <param name="email">Email at this address</param>
        public AddressInfo(string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country, string phone, string email)
        {
            _firstName = firstName;
            _lastName = lastName;
            _address1 = address1;
            _address2 = address2;
            _city = city;
            _state = state;
            _zip = zip;
            _country = country;
            _phone = phone;
            _email = email;
        }

        // Properties
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }

        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
    }
}
