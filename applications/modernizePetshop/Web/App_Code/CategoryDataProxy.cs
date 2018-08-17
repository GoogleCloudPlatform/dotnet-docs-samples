using System.Web;
using System.Collections.Generic;
using System.Configuration;
using PetShop.Model;
using PetShop.BLL;


namespace PetShop.Web
{
    public class CategoryDataProxy {

        private static readonly int categoryTimeout = int.Parse(ConfigurationManager.AppSettings["CategoryCacheDuration"]);
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying data has already been cached.
        /// </summary>
        /// <returns>List of CategoryInfo from Cache or Business component</returns>
        public static IList<CategoryInfo> GetCategories() {

            Category cat = new Category();

            if (!enableCaching)
                return cat.GetCategories();

            string key = "category_all";
            IList<CategoryInfo> data = (IList<CategoryInfo>)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {
                // If the data is not in the cache then fetch the data from the business logic tier
                data = cat.GetCategories();

                // Create a AggregateCacheDependency object from the factory
                //AggregateCacheDependency cd = DependencyFacade.GetCategoryDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                //HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(categoryTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// This method acts as a proxy between the web and business components to check whether the 
        /// underlying category has already been cached.
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <returns>CategoryInfo from Cache or Business component</returns>
        public static CategoryInfo GetCategory(string categoryId) {

            Category category = new Category();

            if (!enableCaching)
                return category.GetCategory(categoryId);

            string key = "category_" + categoryId;
            CategoryInfo data = (CategoryInfo)HttpRuntime.Cache[key];

            // Check if the data exists in the data cache
            if (data == null) {

                // If the data is not in the cache then fetch the data from the business logic tier
                data = category.GetCategory(categoryId);

                // Create a AggregateCacheDependency object from the factory
                //AggregateCacheDependency cd = DependencyFacade.GetCategoryDependency();

                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                //HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(categoryTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return data;
        }
    }
}
