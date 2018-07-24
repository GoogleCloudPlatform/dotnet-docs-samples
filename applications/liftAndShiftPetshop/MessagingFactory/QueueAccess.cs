using System;
using System.Reflection;
using System.Configuration;

namespace PetShop.MessagingFactory {

    /// <summary>
    /// This class is implemented following the Abstract Factory pattern to create the Order
    /// Messaging implementation specified from the configuration file
    /// </summary>
    public sealed class QueueAccess {

        // Look up the Messaging implementation we should be using
        private static readonly string path = ConfigurationManager.AppSettings["OrderMessaging"];

        private QueueAccess() { }

        public static PetShop.IMessaging.IOrder CreateOrder() {
            string className = path + ".Order";
            return (PetShop.IMessaging.IOrder)Assembly.Load(path).CreateInstance(className);
        }
    }
}
