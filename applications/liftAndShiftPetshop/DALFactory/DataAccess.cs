using System.Reflection;
using System.Configuration;

namespace PetShop.DALFactory {

    /// <summary>
    /// This class is implemented following the Abstract Factory pattern to create the DAL implementation
    /// specified from the configuration file
    /// </summary>
    public sealed class DataAccess {

        // Look up the DAL implementation we should be using
        private static readonly string path = ConfigurationManager.AppSettings["WebDAL"];
        private static readonly string orderPath = ConfigurationManager.AppSettings["OrdersDAL"];
        
        private DataAccess() { }

        public static PetShop.IDAL.ICategory CreateCategory() {
            string className = path + ".Category";
            return (PetShop.IDAL.ICategory)Assembly.Load(path).CreateInstance(className);
        }

        public static PetShop.IDAL.IInventory CreateInventory() {
            string className = path + ".Inventory";
            return (PetShop.IDAL.IInventory)Assembly.Load(path).CreateInstance(className);
        }

        public static PetShop.IDAL.IItem CreateItem() {
            string className = path + ".Item";
            return (PetShop.IDAL.IItem)Assembly.Load(path).CreateInstance(className);
        }

        public static PetShop.IDAL.IOrder CreateOrder() {
            string className = orderPath + ".Order";
            return (PetShop.IDAL.IOrder)Assembly.Load(orderPath).CreateInstance(className);
        }

        public static PetShop.IDAL.IProduct CreateProduct() {
            string className = path + ".Product";
            return (PetShop.IDAL.IProduct)Assembly.Load(path).CreateInstance(className);
        }

    }
}
