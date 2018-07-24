using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PetProduct.Configuration;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using ServiceSharedCore.Utilities;
using ServiceSharedCore.Models;

namespace ProductServiceCore.Conrollers
{
    public class ItemController : Controller
    {
        private const string SQL_SELECT_ITEMS_BY_PRODUCT = "SELECT Item.ItemId AS id, Item.Name, Inventory.Qty AS quantity, Item.ListPrice AS price, Product.Name as productName, Item.Image, Product.CategoryId, Product.ProductId FROM Item INNER JOIN" +
                                                           " product ON Item.ProductId = Product.ProductId INNER JOIN inventory ON Item.ItemId = Inventory.ItemId WHERE Item.ProductId = :productId";
        //"SELECT Item.ItemId AS id, Item.Name, Inventory.Qty AS quantity, Item.ListPrice AS price, Product.Name as productName, Item.Image, Product.CategoryId, Product.ProductId FROM \"MSPETSHOP4\".Item INNER JOIN" +
        //                                                   " \"MSPETSHOP4\".product ON Item.ProductId = Product.ProductId INNER JOIN \"MSPETSHOP4\".inventory ON Item.ItemId = Inventory.ItemId WHERE Item.ProductId = :productId";



        private const string SQL_SELECT_ITEMS_BY_ID = "SELECT Item.ItemId AS id, Item.Name, Item.ListPrice as price, Product.Name AS ProductName, Item.Image, Product.CategoryId, Product.ProductId FROM Item " +
                                                      "INNER JOIN Product ON Item.ProductId = Product.ProductId WHERE Item.ItemId = :itemId";
        //"SELECT Item.ItemId AS id, Item.Name, Item.ListPrice as price, Product.Name AS ProductName, Item.Image, Product.CategoryId, Product.ProductId FROM \"MSPETSHOP4\".Item " +
        //                                              "INNER JOIN \"MSPETSHOP4\".Product ON Item.ProductId = Product.ProductId WHERE Item.ItemId = :itemId";

        private string PostgreSQLConnectionString { get; set; }
        private bool CachingEnabled { get; set; }

        public ItemController(IOptions<ConnectionSettings> settings)
        {
            PostgreSQLConnectionString = settings.Value.PostgreSQLConnectionString;
            CachingEnabled = settings.Value.CachingEnabled == "true";
        }

        [Route("item/byproduct/{productid}")]
        [HttpGet]
        public IList<ItemInfo> GetItemByProduct(string productId)
        {
            string key = "itembyproduct_" + productId;
            if (CachingEnabled)
            {
                var cached = ServiceSharedCore.RedisCacheManager.Get<List<ItemInfo>>(key);
                if (cached != null) return cached;
            }
            var dbResult = DBFacilitator.GetList<ItemInfo>(
                PostgreSQLConnectionString,
                SQL_SELECT_ITEMS_BY_PRODUCT,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>("productId", productId, NpgsqlDbType.Text) } });
            if (CachingEnabled)
                ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }

        // GET api/values/5
        [Route("item/{itemId}")]
        public ItemInfo GetItem(string itemId)
        {
            string key = "item_" + itemId;
            if (CachingEnabled)
            {
                var cached = ServiceSharedCore.RedisCacheManager.Get<ItemInfo>(key);
                if (cached != null) return cached;
            }
            var dbResult = DBFacilitator.GetOne<ItemInfo>(
                PostgreSQLConnectionString,
                SQL_SELECT_ITEMS_BY_ID,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>("itemId", itemId, NpgsqlDbType.Text) } });
            if (CachingEnabled)
                ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }

    }
}
