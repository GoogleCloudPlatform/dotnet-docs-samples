using System;

namespace PetShop.Model {

    /// <summary>
    /// Business entity used to model a product
    /// </summary>
    [Serializable]
    public class ProductInfo {

        // Internal member variables
        private string id;
        private string name;
        private string description;
        private string image;
        private string categoryId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="name">Product Name</param>
        /// <param name="description">Product Description</param>
        /// <param name="image">Product image</param>
        /// <param name="categoryId">Category Id</param>
        public ProductInfo(string id, string name, string description, string image, string categoryId) {
            this.id = id;
            this.name = name;
            this.description = description;
            this.image = image;
            this.categoryId = categoryId;
        }

        // Properties
        public string Id {
            get { return id; }
        }

        public string Name {
            get { return name; }
        }

        public string Description {
            get { return description; }
        }

        public string Image {
            get { return image; }
        }

        public string CategoryId {
            get { return categoryId; }
        }
        
    }
}