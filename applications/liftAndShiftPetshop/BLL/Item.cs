using System.Collections.Generic;
using PetShop.Model;
using PetShop.IDAL;

namespace PetShop.BLL {

    /// <summary>
    /// A business component to manage product items
    /// </summary>
    public class Item {

        // Get an instance of the Item DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IItem dal = PetShop.DALFactory.DataAccess.CreateItem();
		        
		/// <summary>
		/// A method to list items by productId
		/// Every item is associated with a parent product
		/// </summary>
		/// <param name="productId">The productId to search by</param> 
		/// <returns>A Generic List of ItemInfo</returns>
		public IList<ItemInfo> GetItemsByProduct(string productId) {

			// Validate input
			if(string.IsNullOrEmpty(productId))
				return new List<ItemInfo>();

			// Use the dal to search by productId
			return dal.GetItemsByProduct(productId);
		}

        /// <summary>
        /// Search for an item given it's unique identifier
        /// </summary>
        /// <param name="itemId">Unique identifier for an item</param>
        /// <returns>An Item business entity</returns>
        public ItemInfo GetItem(string itemId) {

            // Validate input
            if (string.IsNullOrEmpty(itemId))
                return null;

            // Use the dal to search by ItemId
            return dal.GetItem(itemId);
        }
    }
}