using System.Web.Caching;

namespace PetShop.TableCacheDependency {
    /// <summary>
    /// Implementation of Category Cache Dependency for SQL Server 2000
    /// </summary>
    public class Category : TableDependency {

        /// <summary>
        /// Call its base constructor by passing its specific configuration key
        /// </summary>
        public Category() : base("CategoryTableDependency") { }
    }
}
