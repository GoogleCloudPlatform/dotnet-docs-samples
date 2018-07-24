using System;

namespace PetShop.Model {
    /// <summary>
    /// Business entity used to model an item.
    /// </summary>
    [Serializable]
    public class ItemInfo {

        // Internal member variables
        private string id;
        private string name;
        private int quantity;
        private decimal price;
        private string productName;
        private string image;
        private string categoryId;
        private string productId;

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
        public ItemInfo(string id, string name, int quantity, decimal price, string productName, string image, string categoryId, string productId) {
            this.id = id;
            this.name = name;
            this.quantity = quantity;
            this.price = price;
            this.productName = productName;
            this.image = image;
            this.categoryId = categoryId;
            this.productId = productId;
        }

        // Properties
        public string Id {
            get { return id; }
        }

        public string Name {
            get { return name; }
        }

        public string ProductName {
            get { return productName; }
        }

        public int Quantity {
            get { return quantity; }
        }

        public decimal Price {
            get { return price; }
        }

        public string Image {
            get { return image; }
        }

        public string CategoryId {
            get { return categoryId; }
        }

        public string ProductId {
            get { return productId; }
        }


    }
}
