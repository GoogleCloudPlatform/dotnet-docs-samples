using System;
using System.Collections.Generic;
using PetShop.Model;

namespace PetShop.IDAL{
	
	/// <summary>
	/// Interface for the Product DAL
	/// </summary>
	public interface IProduct{
	
		/// <summary>
		/// Method to search products by category name
		/// </summary>
		/// <param name="category">Name of the category to search by</param>
        /// <returns>Interface to Model Collection Generic of search results</returns>
		IList<ProductInfo> GetProductsByCategory(string category);	

		/// <summary>
		/// Method to search products by a set of keyword
		/// </summary>
		/// <param name="keywords">An array of keywords to search by</param>
		/// <returns>Interface to Model Collection Generic of search results</returns>
        IList<ProductInfo> GetProductsBySearch(string[] keywords);

		/// <summary>
		/// Query for a product
		/// </summary>
		/// <param name="productId">Product Id</param>
		/// <returns>Interface to Model ProductInfo for requested product</returns>
		ProductInfo GetProduct(string productId);
	}
}
