using System;
using PetShop.Model;

namespace PetShop.IDAL{
	
	/// <summary>
	/// Interface for the Inventory DAL
	/// </summary>
	public interface IInventory{

		/// <summary>
		/// Get the current stock level of an Item
		/// </summary>
		/// <param name="ItemId">Unique identifier for an item</param>
		/// <returns>Quantity in stock</returns>
		int CurrentQtyInStock(string itemId);

		/// <summary>
		/// Reduces the stock level by the given quantity for items in an order
		/// </summary>
		/// <param name="items">Array of order lineitem</param>
		void TakeStock(LineItemInfo[] items);
	}
}
