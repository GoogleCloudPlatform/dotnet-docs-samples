using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PetProduct.Configuration;
using Microsoft.Extensions.Options;
using ServiceSharedCore.Utilities;
using NpgsqlTypes;
using ServiceSharedCore.Models;

namespace ProductServiceCore.Controllers
{
    public class CategoryController : Controller
    {
        private const string SQL_GET_PRODUCTS = "SELECT CategoryId AS Id, Name, Descn AS Description FROM category";
        //"SELECT CategoryId AS Id, Name, Descn AS Description FROM \"MSPETSHOP4\".category";
        private const string SQL_GET_PRODUCTS_BY_CATEGORY = "SELECT CategoryId AS Id, Name, Descn AS Description FROM category WHERE CategoryId = :categoryId";
        //"SELECT CategoryId AS Id, Name, Descn AS Description FROM \"MSPETSHOP4\".category WHERE CategoryId = :categoryId";
        private string PostgreSQLConnectionString { get; set; }
        private bool CachingEnabled { get; set; }
        public CategoryController(IOptions<ConnectionSettings> settings)
        {
            PostgreSQLConnectionString = settings.Value.PostgreSQLConnectionString;
            CachingEnabled = settings.Value.CachingEnabled == "true";
        }
        // GET category
        [HttpGet]
        [Route("category")]
        public List<CategoryInfo> Get()
        {
            string key = "category_all";
            if (CachingEnabled)
            {
                var cached = ServiceSharedCore.RedisCacheManager.Get<List<CategoryInfo>>(key);
                if (cached != null) return cached;
            }
            var dbResult = DBFacilitator.GetList<CategoryInfo>(
                PostgreSQLConnectionString,
                SQL_GET_PRODUCTS);
            if (CachingEnabled)
                ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }
        [HttpGet]
        [Route("category/{categoryId}")]
        public CategoryInfo Get(string categoryId)
        {
            string key = "category_" + categoryId;
            if (CachingEnabled)
            {
                var cached = ServiceSharedCore.RedisCacheManager.Get<CategoryInfo>(key);
                if (cached != null) return cached;
            }
            var dbResult = DBFacilitator.GetList<CategoryInfo>(
                PostgreSQLConnectionString,
                SQL_GET_PRODUCTS_BY_CATEGORY,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>("categoryId", categoryId, NpgsqlDbType.Text) } })
                .First();
            if (CachingEnabled)
                ServiceSharedCore.RedisCacheManager.Store(key, dbResult);
            return dbResult;
        }
    }
}