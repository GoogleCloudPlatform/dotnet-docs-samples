using System.Reflection;
using System.Configuration;
using PetShop.ICacheDependency;

namespace PetShop.CacheDependencyFactory {
    public static class DependencyAccess {
        /// <summary>
        /// Method to create an instance of Category dependency implementation
        /// </summary>
        /// <returns>Category Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateCategoryDependency() {
            return LoadInstance("Category");
        }

        /// <summary>
        /// Method to create an instance of Product dependency implementation
        /// </summary>
        /// <returns>Product Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateProductDependency() {
            return LoadInstance("Product");
        }

        /// <summary>
        /// Method to create an instance of Item dependency implementation
        /// </summary>
        /// <returns>Item Dependency Implementation</returns>
        public static IPetShopCacheDependency CreateItemDependency() {
            return LoadInstance("Item");
        }

        /// <summary>
        /// Common method to load dependency class from information provided from configuration file 
        /// </summary>
        /// <param name="className">Type of dependency</param>
        /// <returns>Concrete Dependency Implementation instance</returns>
        private static IPetShopCacheDependency LoadInstance(string className) {

            string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
            string fullyQualifiedClass = path + "." + className;

            // Using the evidence given in the config file load the appropriate assembly and class
            return (IPetShopCacheDependency)Assembly.Load(path).CreateInstance(fullyQualifiedClass);
        }
    }
}
