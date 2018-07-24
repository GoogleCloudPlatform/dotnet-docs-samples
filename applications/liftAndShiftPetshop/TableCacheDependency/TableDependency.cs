using System.Web.Caching;
using System.Configuration;

namespace PetShop.TableCacheDependency {

    /// <summary>
    /// This is the base class for SQL2KCacheDependency implementation that encapsulates common
    /// algorithm to retrieve database and table names from configuration file and create
    /// the necessary AggregateCacheDependency object
    /// </summary>
    public abstract class TableDependency : PetShop.ICacheDependency.IPetShopCacheDependency {

        // This is the separator that's used in web.config
        protected char[] configurationSeparator = new char[] { ',' };

        protected AggregateCacheDependency dependency = new AggregateCacheDependency();

        /// <summary>
        /// The constructor retrieves all related configuration and add CacheDependency object accordingly
        /// </summary>
        /// <param name="configKey">Configuration key for specific derived class implementation</param>
        protected TableDependency(string configKey) {

            string dbName = ConfigurationManager.AppSettings["CacheDatabaseName"];
            string tableConfig = ConfigurationManager.AppSettings[configKey];
            string[] tables = tableConfig.Split(configurationSeparator);

            foreach (string tableName in tables)
                dependency.Add(new SqlCacheDependency(dbName, tableName));
        }

        public AggregateCacheDependency GetDependency() {
            return dependency;
        }
    }
}
