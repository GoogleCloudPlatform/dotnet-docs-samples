using System.Configuration;
using System.Web.Caching;
using System.Collections.Generic;
using PetShop.ICacheDependency;

namespace PetShop.CacheDependencyFactory {
    /// <summary>
    /// This class is provided to ease the usage of DependencyFactory from the client.
    /// It's main usage is to determine whether to invoke the DependencyFactory.  
    /// When no assembly is specified under CacheDependencyAssembly section in configuraion file, 
    /// then this class will return null
    /// Notice that this assembly reference System.Web
    /// </summary>
    public static class DependencyFacade {
        private static readonly string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];

        public static AggregateCacheDependency GetCategoryDependency() {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateCategoryDependency().GetDependency();
            else
                return null;
        }

        public static AggregateCacheDependency GetProductDependency() {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateProductDependency().GetDependency();
            else
                return null;
        }

        public static AggregateCacheDependency GetItemDependency() {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateItemDependency().GetDependency();
            else
                return null;
        }
    }
}
