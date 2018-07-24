using System;
using System.Data;
using System.Data.SqlClient;
using PetShop.Model;
using PetShop.IDAL;
using System.Collections.Generic;
using PetShop.DBUtility;

namespace PetShop.SQLServerDAL {

    public class Category : ICategory {

        // Static constants
        private const string SQL_SELECT_CATEGORIES = "SELECT CategoryId, Name, Descn FROM Category";
        private const string SQL_SELECT_CATEGORY = "SELECT CategoryId, Name, Descn FROM Category WHERE CategoryId = @CategoryId";
        private const string PARM_CATEGORY_ID = "@CategoryId";


        /// <summary>
        /// Method to get all categories
		/// </summary>	    	 
        public IList<CategoryInfo> GetCategories() {

			IList<CategoryInfo> categories = new List<CategoryInfo>();

            //Execute a query to read the categories
			using(SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_CATEGORIES, null)) {
                while (rdr.Read()) {
                    CategoryInfo cat = new CategoryInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2));
                    categories.Add(cat);
                }
            }  
            return categories;
        }

        /// <summary>
        /// Get an individual category based on a provided id
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <returns>Details about the Category</returns>
        public CategoryInfo GetCategory(string categoryId) {

            //Set up a return value
            CategoryInfo category = null;

            //Create a parameter
            SqlParameter parm = new SqlParameter(PARM_CATEGORY_ID, SqlDbType.VarChar, 10);
            //Bind the parameter
            parm.Value = categoryId;

            //Execute the query	
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, SQL_SELECT_CATEGORY, parm)) {
                if (rdr.Read())

                    category = new CategoryInfo(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2));
                else
                    category = new CategoryInfo();
            }
            return category;
        }

        /// <summary>
        /// Get the SqlCommand used to retrieve a list of categories
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Sql Command object used to retrieve the data</returns>
        public static SqlCommand GetCommand() {
            return new SqlCommand(SQL_SELECT_CATEGORIES);               
        }
    }
}
