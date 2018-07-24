using System;
using System.Collections.Generic;
using PetShop.Model;

namespace PetShop.BLL {

    /// <summary>
    /// An object to represent a customer's shopping cart.
    /// This class is also used to keep track of customer's wish list.
    /// </summary>
    [Serializable]
    public class Cart {

        // Internal storage for a cart	  
        private Dictionary<string, CartItemInfo> cartItems = new Dictionary<string, CartItemInfo>();

        /// <summary>
        /// Calculate the total for all the cartItems in the Cart
        /// </summary>
        public decimal Total {
            get {
                decimal total = 0;
                foreach (CartItemInfo item in cartItems.Values)
                    total += item.Price * item.Quantity;
                return total;
            }
        }

        /// <summary>
        /// Update the quantity for item that exists in the cart
        /// </summary>
        /// <param name="itemId">Item Id</param>
        /// <param name="qty">Quantity</param>
        public void SetQuantity(string itemId, int qty) {
            cartItems[itemId].Quantity = qty;
        }

        /// <summary>
        /// Return the number of unique items in cart
        /// </summary>
        public int Count {
            get { return cartItems.Count; }
        }

        /// <summary>
        /// Add an item to the cart.
        /// When ItemId to be added has already existed, this method will update the quantity instead.
        /// </summary>
        /// <param name="itemId">Item Id of item to add</param>
        public void Add(string itemId) {
            CartItemInfo cartItem;
            if (!cartItems.TryGetValue(itemId, out cartItem)) {
                Item item = new Item();
                ItemInfo data = item.GetItem(itemId);
                if (data != null) {
                    CartItemInfo newItem = new CartItemInfo(itemId, data.ProductName, 1, (decimal)data.Price, data.Name, data.CategoryId, data.ProductId);
                    cartItems.Add(itemId, newItem);
                }
            }
            else
                cartItem.Quantity++;
        }

        /// <summary>
        /// Add an item to the cart.
        /// When ItemId to be added has already existed, this method will update the quantity instead.
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(CartItemInfo item) {
            CartItemInfo cartItem;
            if (!cartItems.TryGetValue(item.ItemId, out cartItem))
                cartItems.Add(item.ItemId, item);
            else
                cartItem.Quantity += item.Quantity;
        }

        /// <summary>
        /// Remove item from the cart based on itemId
        /// </summary>
        /// <param name="itemId">ItemId of item to remove</param>
        public void Remove(string itemId) {
            cartItems.Remove(itemId);
        }

        /// <summary>
        /// Returns all items in the cart. Useful for looping through the cart.
        /// </summary>
        /// <returns>Collection of CartItemInfo</returns>
        public ICollection<CartItemInfo> CartItems {
            get { return cartItems.Values; }
        }

        /// <summary>
        /// Method to convert all cart items to order line items
        /// </summary>
        /// <returns>A new array of order line items</returns>
        public LineItemInfo[] GetOrderLineItems() {

            LineItemInfo[] orderLineItems = new LineItemInfo[cartItems.Count];
            int lineNum = 0;

            foreach (CartItemInfo item in cartItems.Values)
                orderLineItems[lineNum] = new LineItemInfo(item.ItemId, item.Name, ++lineNum, item.Quantity, item.Price);

            return orderLineItems;
        }

        /// <summary>
        /// Clear the cart
        /// </summary>
        public void Clear() {
            cartItems.Clear();
        }
    }
}