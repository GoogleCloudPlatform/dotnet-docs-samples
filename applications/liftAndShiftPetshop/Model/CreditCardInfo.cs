using System;

namespace PetShop.Model {

    /// <summary>
    /// Business entity used to model credit card information.
    /// </summary>
    [Serializable]
    public class CreditCardInfo {

        // Internal member variables
        private string cardType;
        private string cardNumber;
        private string cardExpiration;

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
        public CreditCardInfo(string cardType, string cardNumber, string cardExpiration) {
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.cardExpiration = cardExpiration;
        }

        // Properties
        public string CardType {
            get { return cardType; }
            set { cardType = value; }
        }

        public string CardNumber {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public string CardExpiration {
            get { return cardExpiration; }
            set { cardExpiration = value; }
        }
    }
}
