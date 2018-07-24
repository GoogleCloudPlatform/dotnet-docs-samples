using System;
using System.Transactions;
using PetShop.IBLLStrategy;

namespace PetShop.BLL {
    /// <summary>
    /// This is a synchronous implementation of IOrderStrategy 
    /// By implementing IOrderStrategy interface, the developer can add a new order insert strategy without re-compiling the whole BLL 
    /// </summary>
    public class OrderSynchronous : IOrderStrategy {

        // Get an instance of the Order DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly PetShop.IDAL.IOrder dal = PetShop.DALFactory.DataAccess.CreateOrder();

        /// <summary>
        /// Inserts the order and updates the inventory stock within a transaction.
        /// </summary>
        /// <param name="order">All information about the order</param>
        public void Insert(PetShop.Model.OrderInfo order) {

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required)) {

                dal.Insert(order);

                // Update the inventory to reflect the current inventory after the order submission
                Inventory inventory = new Inventory();
                inventory.TakeStock(order.LineItems);

                // Calling Complete commits the transaction.
                // Excluding this call by the end of TransactionScope's scope will rollback the transaction
                ts.Complete();
            }
        }
    }
}