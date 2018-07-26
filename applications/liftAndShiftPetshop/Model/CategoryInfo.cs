using System;

namespace PetShop.Model
{
    /// <summary>
    /// Business entity used to model a product
    /// </summary>
    [Serializable]
    public class CategoryInfo
    {
        // Internal member variables
        private readonly string _id;
        private readonly string _name;
        private readonly string _description;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CategoryInfo() { }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <param name="name">Category Name</param>
        /// <param name="description">Category Description</param>
        public CategoryInfo(string id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
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
    }
}