using System.Collections.Generic;
using PetShop.Model;
using PetShop.IDAL;

namespace PetShop.BLL {
    /// <summary>
    /// A business component to manage categories
    /// </summary>
    public class Category {

        // Get an instance of the Category DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly ICategory dal = PetShop.DALFactory.DataAccess.CreateCategory();

        /// <summary>
        /// Method to get all categories
        /// </summary>										
        /// <returns>Generic List of CategoryInfo</returns>	   		
        public IList<CategoryInfo> GetCategories() {
            return dal.GetCategories();
        }

        /// <summary>
        /// Search for a category given it's unique identifier
        /// </summary>
        /// <param name="categoryId">Unique identifier for a Category</param>
        /// <returns>A Category business entity</returns>
        public CategoryInfo GetCategory(string categoryId) {

            // Validate input
            if (string.IsNullOrEmpty(categoryId))
                return null;

            // Use the dal to search by category Id
            return dal.GetCategory(categoryId);
        }
    }
}
