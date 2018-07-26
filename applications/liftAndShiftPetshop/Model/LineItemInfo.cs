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