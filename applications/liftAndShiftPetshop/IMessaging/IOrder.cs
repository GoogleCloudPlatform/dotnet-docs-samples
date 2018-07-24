using PetShop.Model;

namespace PetShop.IMessaging {
    /// <summary>
    /// This is the interface that the MessagingFactory returns.
    /// To create a new implementation of Order Messaging, developers 
    /// need to implement this interface to comply with Abstract Factory Design Pattern
    /// </summary>
    public interface IOrder {
        /// <summary>
        /// Method to retrieve order information from a messaging queue
        /// </summary>
        /// <returns>All information about an order</returns>
        OrderInfo Receive();

        /// <summary>
        /// Method to retrieve order information from a messaging queue
        /// </summary>
        /// <returns>All information about an order</returns>
        OrderInfo Receive(int timeout);

        /// <summary>
        /// Method to send an order to a message queue for later processing
        /// </summary>
        /// <param name="body">All information about an order</param>
        void Send(OrderInfo orderMessage);
    }
}
