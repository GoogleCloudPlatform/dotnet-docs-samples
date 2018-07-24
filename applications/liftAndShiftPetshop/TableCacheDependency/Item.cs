using System.Web.Caching;

namespace PetShop.TableCacheDependency {
    /// <summary>
    /// Implementation of Item Cache Dependency for SQL Server 2000
    /// </summary>
    public class Item : TableDependency {

        /// <summary>
        /// Call its base constructor by passing its specific configuration key
        /// </summary>
        public Item() : base("ItemTableDependency") { }
    }
}
