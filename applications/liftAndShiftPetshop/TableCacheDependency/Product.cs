using System.Web.Caching;

namespace PetShop.TableCacheDependency {
    /// <summary>
    /// Implementation of Product Cache Dependency for SQL Server 2000
    /// </summary>
    public class Product : TableDependency {

        /// <summary>
        /// Call its base constructor by passing its specific configuration key
        /// </summary>
        public Product() : base("ProductTableDependency") { }
    }
}
