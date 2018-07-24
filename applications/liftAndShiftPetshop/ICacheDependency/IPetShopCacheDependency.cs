using System.Web.Caching;

namespace PetShop.ICacheDependency {
    /// <summary>
    /// This is the interface that the DependencyFactory (Factory Pattern) returns.
    /// Developers could implement this interface to add different types of Cache Dependency to Pet Shop.
    /// </summary>
    public interface IPetShopCacheDependency {

        /// <summary>
        /// Method to create the appropriate implementation of Cache Dependency
        /// </summary>
        /// <returns>CacheDependency object(s) embedded in AggregateCacheDependency</returns>
        AggregateCacheDependency GetDependency();
    }
}
