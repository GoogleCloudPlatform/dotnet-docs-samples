using System.Collections.Generic;
using PetShop.Model;
using PetShop.IDAL;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System;

namespace PetShop.BLL {
    /// <summary>
    /// A business component to manage categories
    /// </summary>
    public class Category  : ICategory{

        /// <summary>
        /// Method to get all categories
        /// </summary>										
        /// <returns>Generic List of CategoryInfo</returns>	   		
        public IList<CategoryInfo> GetCategories() {
            var requestURL = ConfigurationManager.AppSettings["ProductBaseURL"] + "category";
            var result = Utilities.WebAPICall(requestURL);
            List<CategoryInfo> categories = JsonConvert.DeserializeObject<List<CategoryInfo>>(result);
            return categories;
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

            var requestURL = ConfigurationManager.AppSettings["ProductBaseURL"] + "category/" + categoryId;
            var result = Utilities.WebAPICall(requestURL);
            CategoryInfo category = JsonConvert.DeserializeObject<CategoryInfo>(result);
            return category;
        }
    }
}
