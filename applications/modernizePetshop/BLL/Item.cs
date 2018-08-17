using System.Collections.Generic;
using PetShop.Model;
using System.Configuration;
using Newtonsoft.Json;

namespace PetShop.BLL
{

    /// <summary>
    /// A business component to manage product items
    /// </summary>
    public class Item {
		        
        string itemURL = ConfigurationManager.AppSettings["ProductBaseURL"] + "item";

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

            var requestURL = itemURL;
            var result = Utilities.WebAPICall(requestURL + "/byproduct/" + productId);
            IList<ItemInfo> items = JsonConvert.DeserializeObject<IList<ItemInfo>>(result);
            return items;
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

            var requestURL = itemURL;
            var result = Utilities.WebAPICall(requestURL + "/" + itemId);
            ItemInfo categories = JsonConvert.DeserializeObject<ItemInfo>(result);
            return categories;
        }
    }
}