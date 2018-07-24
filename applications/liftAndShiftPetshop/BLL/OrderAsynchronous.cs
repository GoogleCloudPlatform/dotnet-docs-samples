using PetShop.IBLLStrategy;

namespace PetShop.BLL
{

    /// <summary>
    /// This is an asynchronous implementation of IOrderStrategy 
    /// By implemeneting IOrderStrategy interface, developer can add a new order insert strategy without re-compiling the whole BLL 
    /// </summary>
    public class OrderAsynchronous : IOrderStrategy
    {
        // Get an instance of the MessagingFactory
        // Making this static will cache the Messaging instance after the initial load
        private static readonly PetShop.IMessaging.IOrder asynchOrder = PetShop.MessagingFactory.QueueAccess.CreateOrder();

        /// <summary>
        /// This method serializes the order object and send it to the queue for asynchronous processing
        /// </summary>
        /// <param name="order">All information about the order</param>
        public void Insert(PetShop.Model.OrderInfo order) {

            asynchOrder.Send(order);
        }
    }
}
