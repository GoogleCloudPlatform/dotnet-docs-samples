using System;

namespace PetShop.Model {

    /// <summary>
    /// Business entity used to model addresses
    /// </summary>
    [Serializable]
    public class AddressInfo {

        // Internal member variables
        private string firstName;
        private string lastName;
        private string address1;
        private string address2;
        private string city;
        private string state;
        private string zip;
        private string country;
        private string phone;
        private string email;

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
        public AddressInfo(string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country, string phone, string email) {
            this.firstName = firstName;
            this.lastName = lastName;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.state = state;
            this.zip = zip;
            this.country = country;
            this.phone = phone;
            this.email = email;
        }

        // Properties
        public string FirstName {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Address1 {
            get { return address1; }
            set { address1 = value; }
        }

        public string Address2 {
            get { return address2; }
            set { address2 = value; }
        }

        public string City {
            get { return city; }
            set { city = value; }
        }

        public string State {
            get { return state; }
            set { state = value; }
        }

        public string Zip {
            get { return zip; }
            set { zip = value; }
        }

        public string Country {
            get { return country; }
            set { country = value; }
        }

        public string Phone {
            get { return phone; }
            set { phone = value; }
        }

        public string Email {
            get { return email; }
            set { email = value; }
        }
    }
}