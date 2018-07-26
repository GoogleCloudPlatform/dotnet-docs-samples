using System;

namespace PetShop.Model
{
    /// <summary>
    /// Business entity used to model a product
    /// </summary>
    [Serializable]
    public class ProductInfo
    {
        // Internal member variables
        private readonly string _id;
        private readonly string _name;
        private readonly string _description;
        private readonly string _image;
        private readonly string _categoryId;

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
        public ProductInfo(string id, string name, string description, string image, string categoryId)
        {
            _id = id;
            _name = name;
            _description = description;
            _image = image;
            _categoryId = categoryId;
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

        public string Description
        {
            get { return _description; }
        }

        public string Image
        {
            get { return _image; }
        }

        public string CategoryId
        {
            get { return _categoryId; }
        }
    }
}