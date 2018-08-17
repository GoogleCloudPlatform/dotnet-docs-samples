// Copyright (c) 2018 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using PetShop.Model;
using System.Data;
using PetShop.DBUtility;
using PetShop.IProfileDAL;

namespace PetShop.SQLProfileDAL
{
    class PetShopProfileProvider : IPetShopProfileProvider
    {
        // Const matching System.Web.Profile.ProfileAuthenticationOption.Anonymous
        private const int AUTH_ANONYMOUS = 0;

        // Const matching System.Web.Profile.ProfileAuthenticationOption.Authenticated
        private const int AUTH_AUTHENTICATED = 1;

        // Const matching System.Web.Profile.ProfileAuthenticationOption.All
        private const int AUTH_ALL = 2;

        /// <summary>
        /// Retrieve account information for current username and application.
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="appName">Application Name</param>
        /// <returns>Account information for current user</returns>
		public AddressInfo GetAccountInfo(string userName, string appName)
        {
            string sqlSelect = "SELECT Account.Email, Account.FirstName, Account.LastName, Account.Address1, Account.Address2, Account.City, Account.State, Account.Zip, Account.Country, Account.Phone FROM Account INNER JOIN Profiles ON Account.UniqueID = Profiles.UniqueID WHERE Profiles.Username = @Username AND Profiles.ApplicationName = @ApplicationName;";
            SqlParameter[] parms = {
                new SqlParameter("@Username", SqlDbType.VarChar, 256),
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256)};
            parms[0].Value = userName;
            parms[1].Value = appName;

            AddressInfo addressInfo = null;

            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect, parms);
            while (dr.Read())
            {
                string address2 = string.Empty;
                if (!dr.IsDBNull(4))
                    address2 = dr.GetString(4);
                addressInfo = new AddressInfo(dr.GetString(1), dr.GetString(2), dr.GetString(3), address2, dr.GetString(5), dr.GetString(6), dr.GetString(7), dr.GetString(8), dr.GetString(9), dr.GetString(0));
            }
            dr.Close();

            return addressInfo;
        }

        /// <summary>
        /// Update account for current user
        /// </summary>
        /// <param name="uniqueID">User id</param>
        /// <param name="addressInfo">Account information for current user</param>   
		public void SetAccountInfo(int uniqueID, AddressInfo addressInfo)
        {
            string sqlDelete = "DELETE FROM Account WHERE UniqueID = @UniqueID;";
            SqlParameter param = new SqlParameter("@UniqueID", SqlDbType.Int);
            param.Value = uniqueID;

            string sqlInsert = "INSERT INTO Account (UniqueID, Email, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone) VALUES (@UniqueID, @Email, @FirstName, @LastName, @Address1, @Address2, @City, @State, @Zip, @Country, @Phone);";

            SqlParameter[] parms = {
            new SqlParameter("@UniqueID", SqlDbType.Int),
            new SqlParameter("@Email", SqlDbType.VarChar, 80),
            new SqlParameter("@FirstName", SqlDbType.VarChar, 80),
            new SqlParameter("@LastName", SqlDbType.VarChar, 80),
            new SqlParameter("@Address1", SqlDbType.VarChar, 80),
            new SqlParameter("@Address2", SqlDbType.VarChar, 80),
            new SqlParameter("@City", SqlDbType.VarChar, 80),
            new SqlParameter("@State", SqlDbType.VarChar, 80),
            new SqlParameter("@Zip", SqlDbType.VarChar, 80),
            new SqlParameter("@Country", SqlDbType.VarChar, 80),
            new SqlParameter("@Phone", SqlDbType.VarChar, 80)};

            parms[0].Value = uniqueID;
            parms[1].Value = addressInfo.Email;
            parms[2].Value = addressInfo.FirstName;
            parms[3].Value = addressInfo.LastName;
            parms[4].Value = addressInfo.Address1;
            parms[5].Value = addressInfo.Address2;
            parms[6].Value = addressInfo.City;
            parms[7].Value = addressInfo.State;
            parms[8].Value = addressInfo.Zip;
            parms[9].Value = addressInfo.Country;
            parms[10].Value = addressInfo.Phone;

            SqlConnection conn = new SqlConnection(SqlHelper.ConnectionStringProfile);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, param);
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlInsert, parms);
                trans.Commit();
            }
            catch (Exception e)
            {
                trans.Rollback();
                throw new ApplicationException(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Retrieve collection of shopping cart items
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="appName">Application Name</param>
        /// <param name="isShoppingCart">Shopping cart flag</param>
        /// <returns>Collection of shopping cart items</returns>
		public IList<CartItemInfo> GetCartItems(string userName, string appName, bool isShoppingCart)
        {
            string sqlSelect = "SELECT Cart.ItemId, Cart.Name, Cart.Type, Cart.Price, Cart.CategoryId, Cart.ProductId, Cart.Quantity FROM Profiles INNER JOIN Cart ON Profiles.UniqueID = Cart.UniqueID WHERE Profiles.Username = @Username AND Profiles.ApplicationName = @ApplicationName AND IsShoppingCart = @IsShoppingCart;";

            SqlParameter[] parms = {
                new SqlParameter("@Username", SqlDbType.VarChar, 256),
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256),
                new SqlParameter("@IsShoppingCart", SqlDbType.Bit)};
            parms[0].Value = userName;
            parms[1].Value = appName;
            parms[2].Value = isShoppingCart;

            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect, parms);

            IList<CartItemInfo> cartItems = new List<CartItemInfo>();

            while (dr.Read())
            {
                CartItemInfo cartItem = new CartItemInfo(dr.GetString(0), dr.GetString(1), dr.GetInt32(6), dr.GetDecimal(3), dr.GetString(2), dr.GetString(4), dr.GetString(5));
                cartItems.Add(cartItem);
            }
            dr.Close();
            return cartItems;
        }

        /// <summary>
        /// Update shopping cart for current user
        /// </summary>
        /// <param name="uniqueID">User id</param>
        /// <param name="cartItems">Collection of shopping cart items</param>
        /// <param name="isShoppingCart">Shopping cart flag</param>
		public void SetCartItems(int uniqueID, ICollection<CartItemInfo> cartItems, bool isShoppingCart)
        {
            string sqlDelete = "DELETE FROM Cart WHERE UniqueID = @UniqueID AND IsShoppingCart = @IsShoppingCart;";

            SqlParameter[] parms1 = {
                new SqlParameter("@UniqueID", SqlDbType.Int),
                new SqlParameter("@IsShoppingCart", SqlDbType.Bit)};
            parms1[0].Value = uniqueID;
            parms1[1].Value = isShoppingCart;

            if (cartItems.Count > 0)
            {
                // update cart using SqlTransaction
                string sqlInsert = "INSERT INTO Cart (UniqueID, ItemId, Name, Type, Price, CategoryId, ProductId, IsShoppingCart, Quantity) VALUES (@UniqueID, @ItemId, @Name, @Type, @Price, @CategoryId, @ProductId, @IsShoppingCart, @Quantity);";

                SqlParameter[] parms2 = {
                new SqlParameter("@UniqueID", SqlDbType.Int),
                new SqlParameter("@IsShoppingCart", SqlDbType.Bit),
                new SqlParameter("@ItemId", SqlDbType.VarChar, 10),
                new SqlParameter("@Name", SqlDbType.VarChar, 80),
                new SqlParameter("@Type", SqlDbType.VarChar, 80),
                new SqlParameter("@Price", SqlDbType.Decimal, 8),
                new SqlParameter("@CategoryId", SqlDbType.VarChar, 10),
                new SqlParameter("@ProductId", SqlDbType.VarChar, 10),
                new SqlParameter("@Quantity", SqlDbType.Int)};
                parms2[0].Value = uniqueID;
                parms2[1].Value = isShoppingCart;


                SqlConnection conn = new SqlConnection(SqlHelper.ConnectionStringProfile);
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms1);

                    foreach (CartItemInfo cartItem in cartItems)
                    {
                        parms2[2].Value = cartItem.ItemId;
                        parms2[3].Value = cartItem.Name;
                        parms2[4].Value = cartItem.Type;
                        parms2[5].Value = cartItem.Price;
                        parms2[6].Value = cartItem.CategoryId;
                        parms2[7].Value = cartItem.ProductId;
                        parms2[8].Value = cartItem.Quantity;
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlInsert, parms2);
                    }
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw new ApplicationException(e.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
                // delete cart
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlDelete, parms1);
        }

        /// <summary>
        /// Update activity dates for current user and application
        /// </summary>
        /// <param name="userName">USer name</param>
        /// <param name="activityOnly">Activity only flag</param>
        /// <param name="appName">Application Name</param>
		public void UpdateActivityDates(string userName, bool activityOnly, string appName)
        {
            DateTime activityDate = DateTime.Now;

            string sqlUpdate;
            SqlParameter[] parms;

            if (activityOnly)
            {
                sqlUpdate = "UPDATE Profiles Set LastActivityDate = @LastActivityDate WHERE Username = @Username AND ApplicationName = @ApplicationName;";
                parms = new SqlParameter[]{
                    new SqlParameter("@LastActivityDate", SqlDbType.DateTime),
                    new SqlParameter("@Username", SqlDbType.VarChar, 256),
                    new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256)};

                parms[0].Value = activityDate;
                parms[1].Value = userName;
                parms[2].Value = appName;
            }
            else
            {
                sqlUpdate = "UPDATE Profiles Set LastActivityDate = @LastActivityDate, LastUpdatedDate = @LastUpdatedDate WHERE Username = @Username AND ApplicationName = @ApplicationName;";
                parms = new SqlParameter[]{
                    new SqlParameter("@LastActivityDate", SqlDbType.DateTime),
                    new SqlParameter("@LastUpdatedDate", SqlDbType.DateTime),
                    new SqlParameter("@Username", SqlDbType.VarChar, 256),
                    new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256)};

                parms[0].Value = activityDate;
                parms[1].Value = activityDate;
                parms[2].Value = userName;
                parms[3].Value = appName;
            }

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlUpdate, parms);
        }

        /// <summary>
        /// Retrive unique id for current user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="isAuthenticated">Authentication flag</param>
        /// <param name="ignoreAuthenticationType">Ignore authentication flag</param>
        /// <param name="appName">Application Name</param>
        /// <returns>Unique id for current user</returns>
		public int GetUniqueID(string userName, bool isAuthenticated, bool ignoreAuthenticationType, string appName)
        {
            string sqlSelect = "SELECT UniqueID FROM Profiles WHERE Username = @Username AND ApplicationName = @ApplicationName";

            SqlParameter[] parms = {
                new SqlParameter("@Username", SqlDbType.VarChar, 256),
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256)};
            parms[0].Value = userName;
            parms[1].Value = appName;

            if (!ignoreAuthenticationType)
            {
                sqlSelect += " AND IsAnonymous = @IsAnonymous";
                Array.Resize<SqlParameter>(ref parms, parms.Length + 1);
                parms[2] = new SqlParameter("@IsAnonymous", SqlDbType.Bit);
                parms[2].Value = !isAuthenticated;
            }

            int uniqueID = 0;

            object retVal = null;
            retVal = SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect, parms);

            if (retVal == null)
                uniqueID = CreateProfileForUser(userName, isAuthenticated, appName);
            else
                uniqueID = Convert.ToInt32(retVal);
            return uniqueID;
        }

        /// <summary>
        /// Create profile record for current user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="isAuthenticated">Authentication flag</param>
        /// <param name="appName">Application Name</param>
        /// <returns>Number of records created</returns>
		public int CreateProfileForUser(string userName, bool isAuthenticated, string appName)
        {
            string sqlInsert = "INSERT INTO Profiles (Username, ApplicationName, LastActivityDate, LastUpdatedDate, IsAnonymous) Values(@Username, @ApplicationName, @LastActivityDate, @LastUpdatedDate, @IsAnonymous); SELECT @@IDENTITY;";

            SqlParameter[] parms = {
                new SqlParameter("@Username", SqlDbType.VarChar, 256),
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256),
                new SqlParameter("@LastActivityDate", SqlDbType.DateTime),
                new SqlParameter("@LastUpdatedDate", SqlDbType.DateTime),
                new SqlParameter("@IsAnonymous", SqlDbType.Bit)};

            parms[0].Value = userName;
            parms[1].Value = appName;
            parms[2].Value = DateTime.Now;
            parms[3].Value = DateTime.Now;
            parms[4].Value = !isAuthenticated;

            int uniqueID = 0;
            int.TryParse(SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlInsert, parms).ToString(), out uniqueID);

            return uniqueID;
        }

        /// <summary>
        /// Retrieve colection of inactive user id's
        /// </summary>
        /// <param name="authenticationOption">Authentication option</param>
        /// <param name="userInactiveSinceDate">Date to start search from</param>
        /// <param name="appName">Application Name</param>
        /// <returns>Collection of inactive profile id's</returns>
		public IList<string> GetInactiveProfiles(int authenticationOption, DateTime userInactiveSinceDate, string appName)
        {
            StringBuilder sqlSelect = new StringBuilder("SELECT Username FROM Profiles WHERE ApplicationName = @ApplicationName AND LastActivityDate <= @LastActivityDate");

            SqlParameter[] parms = {
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256),
                new SqlParameter("@LastActivityDate", SqlDbType.DateTime)};
            parms[0].Value = appName;
            parms[1].Value = userInactiveSinceDate;

            switch (authenticationOption)
            {
                case AUTH_ANONYMOUS:
                    sqlSelect.Append(" AND IsAnonymous = @IsAnonymous");
                    Array.Resize<SqlParameter>(ref parms, parms.Length + 1);
                    parms[2] = new SqlParameter("@IsAnonymous", SqlDbType.Bit);
                    parms[2].Value = true;
                    break;
                case AUTH_AUTHENTICATED:
                    sqlSelect.Append(" AND IsAnonymous = @IsAnonymous");
                    Array.Resize<SqlParameter>(ref parms, parms.Length + 1);
                    parms[2] = new SqlParameter("@IsAnonymous", SqlDbType.Bit);
                    parms[2].Value = false;
                    break;
                default:
                    break;
            }

            IList<string> usernames = new List<string>();

            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect.ToString(), parms);
            while (dr.Read())
            {
                usernames.Add(dr.GetString(0));
            }

            dr.Close();
            return usernames;
        }

        /// <summary>
        /// Delete user's profile
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="appName">Application Name</param>
        /// <returns>True, if profile successfully deleted</returns>
		public bool DeleteProfile(string userName, string appName)
        {
            int uniqueID = GetUniqueID(userName, false, true, appName);

            string sqlDelete = "DELETE FROM Profiles WHERE UniqueID = @UniqueID;";
            SqlParameter param = new SqlParameter("@UniqueId", SqlDbType.Int, 4);
            param.Value = uniqueID;

            int numDeleted = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlDelete, param);

            if (numDeleted <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retrieve profile information
        /// </summary>
        /// <param name="authenticationOption">Authentication option</param>
        /// <param name="usernameToMatch">User name</param>
        /// <param name="userInactiveSinceDate">Date to start search from</param>
        /// <param name="appName">Application Name</param>
        /// <param name="totalRecords">Number of records to return</param>
        /// <returns>Collection of profiles</returns>
		public IList<CustomProfileInfo> GetProfileInfo(int authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, string appName, out int totalRecords)
        {
            // Retrieve the total count.
            StringBuilder sqlSelect1 = new StringBuilder("SELECT COUNT(*) FROM Profiles WHERE ApplicationName = @ApplicationName");
            SqlParameter[] parms1 = {
                new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256)};
            parms1[0].Value = appName;

            // Retrieve the profile data.
            StringBuilder sqlSelect2 = new StringBuilder("SELECT Username, LastActivityDate, LastUpdatedDate, IsAnonymous FROM Profiles WHERE ApplicationName = @ApplicationName");
            SqlParameter[] parms2 = { new SqlParameter("@ApplicationName", SqlDbType.VarChar, 256) };
            parms2[0].Value = appName;

            int arraySize;

            // If searching for a user name to match, add the command text and parameters.
            if (usernameToMatch != null)
            {
                arraySize = parms1.Length;

                sqlSelect1.Append(" AND Username LIKE @Username ");
                Array.Resize<SqlParameter>(ref parms1, arraySize + 1);
                parms1[arraySize] = new SqlParameter("@Username", SqlDbType.VarChar, 256);
                parms1[arraySize].Value = usernameToMatch;

                sqlSelect2.Append(" AND Username LIKE @Username ");
                Array.Resize<SqlParameter>(ref parms2, arraySize + 1);
                parms2[arraySize] = new SqlParameter("@Username", SqlDbType.VarChar, 256);
                parms2[arraySize].Value = usernameToMatch;
            }


            // If searching for inactive profiles, 
            // add the command text and parameters.
            if (userInactiveSinceDate != null)
            {
                arraySize = parms1.Length;

                sqlSelect1.Append(" AND LastActivityDate >= @LastActivityDate ");
                Array.Resize<SqlParameter>(ref parms1, arraySize + 1);
                parms1[arraySize] = new SqlParameter("@LastActivityDate", SqlDbType.DateTime);
                parms1[arraySize].Value = (DateTime)userInactiveSinceDate;

                sqlSelect2.Append(" AND LastActivityDate >= @LastActivityDate ");
                Array.Resize<SqlParameter>(ref parms2, arraySize + 1);
                parms2[arraySize] = new SqlParameter("@LastActivityDate", SqlDbType.DateTime);
                parms2[arraySize].Value = (DateTime)userInactiveSinceDate;
            }


            // If searching for a anonymous or authenticated profiles,    
            // add the command text and parameters.	
            if (authenticationOption != AUTH_ALL)
            {
                arraySize = parms1.Length;

                Array.Resize<SqlParameter>(ref parms1, arraySize + 1);
                sqlSelect1.Append(" AND IsAnonymous = @IsAnonymous");
                parms1[arraySize] = new SqlParameter("@IsAnonymous", SqlDbType.Bit);

                Array.Resize<SqlParameter>(ref parms2, arraySize + 1);
                sqlSelect2.Append(" AND IsAnonymous = @IsAnonymous");
                parms2[arraySize] = new SqlParameter("@IsAnonymous", SqlDbType.Bit);

                switch (authenticationOption)
                {
                    case AUTH_ANONYMOUS:
                        parms1[arraySize].Value = true;
                        parms2[arraySize].Value = true;
                        break;
                    case AUTH_AUTHENTICATED:
                        parms1[arraySize].Value = false;
                        parms2[arraySize].Value = false;
                        break;
                    default:
                        break;
                }
            }

            IList<CustomProfileInfo> profiles = new List<CustomProfileInfo>();

            // Get the profile count.
            totalRecords = (int)SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect1.ToString(), parms1);

            // No profiles found.
            if (totalRecords <= 0)
                return profiles;

            SqlDataReader dr;
            dr = SqlHelper.ExecuteReader(SqlHelper.ConnectionStringProfile, CommandType.Text, sqlSelect2.ToString(), parms2);
            while (dr.Read())
            {
                CustomProfileInfo profile = new CustomProfileInfo(dr.GetString(0), dr.GetDateTime(1), dr.GetDateTime(2), dr.GetBoolean(3));
                profiles.Add(profile);
            }
            dr.Close();

            return profiles;
        }
    }
}
