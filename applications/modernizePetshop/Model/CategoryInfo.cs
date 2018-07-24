using System;

namespace PetShop.Model {

    /// <summary>
    /// Business entity used to model a product
    /// </summary>
    [Serializable]
    public class CategoryInfo {

        // Internal member variables
        private string id;
        private string name;
        private string description;

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
        public CategoryInfo(string id, string name, string description) {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        // Properties
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

    }
}