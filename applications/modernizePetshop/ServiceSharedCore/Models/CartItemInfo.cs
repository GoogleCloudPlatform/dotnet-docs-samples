namespace ServiceSharedCore.Models
{
    public class CartItemInfo
    {

        // Internal member variables
        private int quantity = 1;
        private string itemId;
        private string name;
        private string type;
        private decimal price;
        private string categoryId;
        private string productId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CartItemInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="itemId">Id of item to add to cart</param></param>
        /// <param name="name">Name of item</param>
        /// <param name="qty">Quantity to purchase</param>
        /// <param name="price">Price of item</param>
        /// <param name="type">Item type</param>	  
        /// <param name="categoryId">Parent category id</param>
        /// <param name="productId">Parent product id</param>
        public CartItemInfo(string itemId, string name, int qty, decimal price, string type, string categoryId, string productId)
        {
            this.itemId = itemId;
            this.name = name;
            this.quantity = qty;
            this.price = price;
            this.type = type;
            this.categoryId = categoryId;
            this.productId = productId;
        }

        // Properties
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public decimal Subtotal
        {
            get { return (decimal)(this.quantity * this.price); }
            set {  }
        }

        public string ItemId
        {
            get { return itemId; }
            set { this.itemId = value; }
        }

        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }
        public decimal Price
        {
            get { return price; }
            set
            {
                this.price = value;
            }
        }

        public string CategoryId
        {
            get
            {
                return categoryId;
            }
            set
            {
                this.categoryId = value;
            }
        }
        public string ProductId
        {
            get
            {
                return productId;
            }
            set
            {
                this.productId = value;
            }
        }
    }
}
