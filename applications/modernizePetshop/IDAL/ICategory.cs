using System;
using PetShop.Model;
using System.Collections.Generic;

namespace PetShop.IDAL{

	/// <summary>
	/// Interface for the Category DAL
	/// </summary>
	public interface ICategory {

		/// <summary>
		/// Method to get all categories
		/// </summary>		
        /// <returns>Interface to Model Collection Generic of categories</returns>
		IList<CategoryInfo> GetCategories();

        /// <summary>
        /// Get information on a specific category
        /// </summary>
        /// <param name="categoryId">Unique identifier for a category</param>
        /// <returns>Business Entity representing an category</returns>
        CategoryInfo GetCategory(string categoryId);
	}
}
