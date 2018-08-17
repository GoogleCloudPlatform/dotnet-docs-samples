using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using PetProduct.Configuration;
using ServiceSharedCore.Utilities;
using ServiceSharedCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductServiceCore.Controllers
{
    public class ProductController : Controller
    {
        private bool CachingEnabled { get; set; }
        public ProductController(IOptions<ConnectionSettings> settings)
        {
            ConfigSettings = settings.Value;
            CachingEnabled = settings.Value.CachingEnabled == "true";
        }

        public ConnectionSettings ConfigSettings { get; private set; }

        private const string SQL_SELECT_PRODUCTS_BY_CATEGORY = "SELECT Product.ProductId AS Id, Product.Name, Product.Descn AS Description, Product.Image, Product.CategoryId FROM Product WHERE Product.CategoryId = :categoryId";
        //"SELECT Product.ProductId AS Id, Product.Name, Product.Descn AS Description, Product.Image, Product.CategoryId FROM \"MSPETSHOP4\".Product WHERE Product.CategoryId = :categoryId";
        [HttpGet]
        [Route("product/productbycategory/{category}")]
        public IList<ProductInfo> GetProductsByCategory(string category)
        {
            string key = "product_" + category;
            if (CachingEnabled)
            {
                var cached = ServiceSharedCore.RedisCacheManager.Get<List<ProductInfo>>(key);
                if (cached != null) return cached;
            }

            var dbResult = DBFacilitator.GetList<ProductInfo>(
            ConfigSettings.PostgreSQLConnectionString,
                SQL_SELECT_PRODUCTS_BY_CATEGORY,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":categoryId", category, NpgsqlDbType.Text) } });
            if (CachingEnabled)
                ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }


        private const string SQL_SELECT_PRODUCT = "SELECT Product.ProductId AS Id, Product.Name, Product.Descn AS Description, Product.Image, Product.CategoryId FROM Product WHERE Product.ProductId  = :productId";
        //"SELECT Product.ProductId AS Id, Product.Name, Product.Descn AS Description, Product.Image, Product.CategoryId FROM \"MSPETSHOP4\".Product WHERE Product.ProductId  = :productId";
        [HttpGet]
        [Route("product/{productId}")]
        public ProductInfo GetProduct(string productId)
        {
            string key = "product_" + productId;
            var cached = ServiceSharedCore.RedisCacheManager.Get<ProductInfo>(key);
            if (cached != null) return cached;

            var dbResult = DBFacilitator.GetOne<ProductInfo>(
            ConfigSettings.PostgreSQLConnectionString,
                SQL_SELECT_PRODUCT,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":productId", productId, NpgsqlDbType.Text) } });

            ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }

        private const string SQL_SELECT_PRODUCTS_BY_SEARCH1 = "SELECT ProductId, Name, Descn, Product.Image, Product.CategoryId FROM Product WHERE ((";
        //"SELECT ProductId, Name, Descn, Product.Image, Product.CategoryId FROM \"MSPETSHOP4\".Product WHERE ((";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH2 = "LOWER(Name) LIKE '%{0}%' OR LOWER(CategoryId) LIKE '%{0}%'";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH3 = ") OR (";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH4 = "))";
        [HttpGet]
        [Route("product/productbykeyword/{terms}")]
        public IList<ProductInfo> GetProductsBySearch(string terms)
        {
            string key = "productsearch_" + terms;
            var cached = ServiceSharedCore.RedisCacheManager.Get<List<ProductInfo>>(key);
            if (cached != null) return cached;

            var keywords = terms.Split(',');
            var sb = new StringBuilder(SQL_SELECT_PRODUCTS_BY_SEARCH1);
            for (int i = 0; i < keywords.Length; i++)
            {
                sb.Append(String.Format(SQL_SELECT_PRODUCTS_BY_SEARCH2, keywords[i]));
                if (i < keywords.Length - 1) sb.Append(SQL_SELECT_PRODUCTS_BY_SEARCH3);
            }
            sb.Append(SQL_SELECT_PRODUCTS_BY_SEARCH4);

            var dbResult = DBFacilitator.GetList<ProductInfo>(
            ConfigSettings.PostgreSQLConnectionString,
                sb.ToString(),
                new List<Tuple<string, string, NpgsqlDbType>>());

            ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }
    }
}
