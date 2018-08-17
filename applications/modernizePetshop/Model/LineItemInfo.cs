using System;

namespace PetShop.Model {

    /// <summary>
    /// Business entity used to model an order line item.
    /// </summary>
    [Serializable]
    public class LineItemInfo {

        // Internal member variables
        private string itemId;
        private string productName;
        private int line;
        private int quantity;
        private decimal price;
        private string productId;

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
        public LineItemInfo(string id, string name, int line, int qty, decimal price, string productId) {
            this.itemId = id;
            this.productName = name;
            this.line = line;
            this.price = price;
            this.quantity = qty;
            this.productId = productId;
        }

        // Properties
        public string ItemId {
            get { return itemId; }
            set { itemId = value; }
        }

        public string ProductName {
            get { return productName; }
            set { productName = value; }
        }

        public int Line {
            get { return line; }
            set { line = value; }
        }

        public int Quantity {
            get { return quantity; }
            set { quantity = value; }
        }

        public decimal Price {
            get { return price; }
            set { price = value; }
        }

        public decimal Subtotal {
            get { return price * quantity; }
        }
        public string ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
    }
}