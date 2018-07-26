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
