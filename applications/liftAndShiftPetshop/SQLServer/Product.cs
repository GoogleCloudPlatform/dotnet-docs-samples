using System;
using System.Data.SqlClient;
using PetShop.Model;
using PetShop.IDAL;
using System.Data;
using System.Text;
using System.Collections.Generic;
using PetShop.DBUtility;

namespace PetShop.SQLServerDAL {

    public class Product : IProduct {

        //Static constants
        private const string SQL_SELECT_PRODUCTS_BY_CATEGORY = "SELECT Product.ProductId, Product.Name, Product.Descn, Product.Image, Product.CategoryId FROM Product WHERE Product.CategoryId = @Category";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH1 = "SELECT ProductId, Name, Descn, Product.Image, Product.CategoryId FROM Product WHERE ((";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH2 = "LOWER(Name) LIKE '%' + {0} + '%' OR LOWER(CategoryId) LIKE '%' + {0} + '%'";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH3 = ") OR (";
        private const string SQL_SELECT_PRODUCTS_BY_SEARCH4 = "))";
        private const string SQL_SELECT_PRODUCT = "SELECT Product.ProductId, Product.Name, Product.Descn, Product.Image, Product.CategoryId FROM Product WHERE Product.ProductId  = @ProductId";
        private const string PARM_CATEGORY = "@Category";
        private const string PARM_KEYWORD = "@Keyword";
        private const string PARM_PRODUCTID = "@ProductId";

        /// <summary>
        /// Query for products by category
        /// </summary>
        /// <param name="category">category name</param>  
        /// <returns>A Generic List of ProductInfo</returns>
        public IList<ProductInfo> GetProductsByCategory(string category) {

            IList<ProductInfo> productsByCategory = new List<ProductInfo>();

            SqlParameter parm = new SqlParameter(PARM_CATEGORY, SqlDbType.VarChar, 10);
            parm.Value = category;

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_PRODUCTS_BY_CATEGORY, parm)) {
                while (rdr.Read()) {
                    ProductInfo product = new ProductInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4));
                    productsByCategory.Add(product);
                }
            }

            return productsByCategory;
        }

        /// <summary>
        /// Query for products by keywords. 
        /// The results will include any product where the keyword appears in the category name or product name
        /// </summary>
        /// <param name="keywords">string array of keywords</param>
        /// <returns>A Generic List of ProductInfo</returns>
        public IList<ProductInfo> GetProductsBySearch(string[] keywords) {

            IList<ProductInfo> productsBySearch = new List<ProductInfo>();

            int numKeywords = keywords.Length;

            //Create a new query string
            StringBuilder sql = new StringBuilder(SQL_SELECT_PRODUCTS_BY_SEARCH1);

            //Add each keyword to the query
            for (int i = 0; i < numKeywords; i++) {
                sql.Append(string.Format(SQL_SELECT_PRODUCTS_BY_SEARCH2, PARM_KEYWORD + i));
                sql.Append(i + 1 < numKeywords ? SQL_SELECT_PRODUCTS_BY_SEARCH3 : SQL_SELECT_PRODUCTS_BY_SEARCH4);
            }

            string sqlProductsBySearch = sql.ToString();
            SqlParameter[] parms = SqlHelper.GetCachedParameters(sqlProductsBySearch);

            // If the parameters are null build a new set
            if (parms == null) {
                parms = new SqlParameter[numKeywords];

                for (int i = 0; i < numKeywords; i++)
                    parms[i] = new SqlParameter(PARM_KEYWORD + i, SqlDbType.VarChar, 80);

                SqlHelper.CacheParameters(sqlProductsBySearch, parms);
            }

            // Bind the new parameters
            for (int i = 0; i < numKeywords; i++)
                parms[i].Value = keywords[i];

            //Finally execute the query
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, sqlProductsBySearch, parms)) {
                while (rdr.Read()) {
                    ProductInfo product = new ProductInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4));
                    productsBySearch.Add(product);
                }
            }

            return productsBySearch;
        }

        /// <summary>
        /// Query for a product
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>ProductInfo object for requested product</returns>
        public ProductInfo GetProduct(string productId) {
            ProductInfo product = null;
            SqlParameter parm = new SqlParameter(PARM_PRODUCTID, SqlDbType.VarChar, 10);
            parm.Value = productId;

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_PRODUCT, parm))
                if (rdr.Read())
                    product = new ProductInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4));
                else
                    product = new ProductInfo();

            return product;
        }

        /// <summary>
        /// Get the SqlCommand used to retrieve a list of products by category id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Sql Command object used to retrieve the data</returns>
        public static SqlCommand GetCommand(string id) {

            //Create a parameter
            SqlParameter parm = new SqlParameter(PARM_CATEGORY, SqlDbType.VarChar, 10);
            parm.Value = id;

            // Create and return SqlCommand object
            SqlCommand command = new SqlCommand(SQL_SELECT_PRODUCTS_BY_CATEGORY);
            command.Parameters.Add(parm);
            return command;
        }
    }
}
