using System;
using System.Data;
using System.Data.SqlClient;
using PetShop.Model;
using PetShop.IDAL;
using System.Collections.Generic;
using PetShop.DBUtility;

namespace PetShop.SQLServerDAL {

    public class Item : IItem {

        // Static constants
        private const string SQL_SELECT_ITEMS_BY_PRODUCT = "SELECT Item.ItemId, Item.Name, Inventory.Qty, Item.ListPrice, Product.Name, Item.Image, Product.CategoryId, Product.ProductId FROM Item INNER JOIN Product ON Item.ProductId = Product.ProductId INNER JOIN Inventory ON Item.ItemId = Inventory.ItemId WHERE Item.ProductId = @ProductId";
        
        private const string SQL_SELECT_ITEM = "SELECT Item.ItemId, Item.Name, Item.ListPrice, Product.Name, Item.Image, Product.CategoryId, Product.ProductId FROM Item INNER JOIN Product ON Item.ProductId = Product.ProductId WHERE Item.ItemId = @ItemId";

        private const string PARM_PRODUCT_ID = "@ProductId";
        private const string PARM_ITEM_ID = "@ItemId";

        /// <summary>
        /// Function to get a list of items within a product group
        /// </summary>
		/// <param name="productId">Product Id</param>	   	 
        /// <returns>A Generic List of ItemInfo</returns>
		public IList<ItemInfo> GetItemsByProduct(string productId) {

            IList<ItemInfo> itemsByProduct = new List<ItemInfo>();

            SqlParameter parm = new SqlParameter(PARM_PRODUCT_ID, SqlDbType.VarChar, 10);
            parm.Value = productId;

            //Execute the query against the database
			using(SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_ITEMS_BY_PRODUCT, parm)) {
                // Scroll through the results
                while (rdr.Read()) {
                    ItemInfo item = new ItemInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2), rdr.GetDecimal(3), rdr.GetString(4), rdr.GetString(5), rdr.GetString(6), rdr.GetString(7));
                    //Add each item to the arraylist
                    itemsByProduct.Add(item);
                }
            }
            return itemsByProduct;
        }


        /// <summary>
        /// Get an individual item based on a unique key
        /// </summary>
        /// <param name="itemId">unique key</param>
        /// <returns>Details about the Item</returns>
        public ItemInfo GetItem(string itemId) {

            //Set up a return value
            ItemInfo item = null;

            //Create a parameter
            SqlParameter parm = new SqlParameter(PARM_ITEM_ID, SqlDbType.VarChar, 10);
            //Bind the parameter
            parm.Value = itemId;

            //Execute the query	
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_ITEM, parm)) {
                if (rdr.Read())
                    item = new ItemInfo(rdr.GetString(0), rdr.GetString(1), 0, rdr.GetDecimal(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetString(6));
                else
                    item = new ItemInfo();
            }
            return item;
        }

        /// <summary>
        /// Get the SqlCommand used to retrieve a list of items by product
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Sql Command object used to retrieve the data</returns>
        public static SqlCommand GetCommand(string id) {

            //Create a parameter
            SqlParameter parm = new SqlParameter(PARM_PRODUCT_ID, SqlDbType.VarChar, 10);
            parm.Value = id;

            // Create and return SqlCommand object
            SqlCommand command = new SqlCommand(SQL_SELECT_ITEMS_BY_PRODUCT);
            command.Parameters.Add(parm);
            return command;
        }
    }
}
