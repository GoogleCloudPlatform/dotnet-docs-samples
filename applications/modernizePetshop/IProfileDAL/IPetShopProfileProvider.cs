using System;
using PetShop.Model;
using System.Collections.Generic;

namespace PetShop.IProfileDAL {
	
		/// <summary>
		/// Interface for the Profile DAL
		/// </summary>
		public interface IPetShopProfileProvider {
			/// <summary>
			/// Retrieve account information for current username and application.
			/// </summary>
			/// <param name="userName">User Name</param>
			/// <param name="appName">Application Name</param>
			/// <returns>Account information for current user</returns>
			AddressInfo GetAccountInfo(string userName, string appName);
			
			/// <summary>
			/// Update account for current user
			/// </summary>
			/// <param name="uniqueID">User id</param>
			/// <param name="addressInfo">Account information for current user</param>   
			void SetAccountInfo(int uniqueID, AddressInfo addressInfo);

			/// <summary>
			/// Retrieve collection of shopping cart items
			/// </summary>
			/// <param name="userName">User Name</param>
			/// <param name="appName">Application Name</param>
			/// <param name="isShoppingCart">Shopping cart flag</param>
			/// <returns>Collection of shopping cart items</returns>
			IList<CartItemInfo> GetCartItems(string userName, string appName, bool isShoppingCart);

			/// <summary>
			/// Update shopping cart for current user
			/// </summary>
			/// <param name="uniqueID">User id</param>
			/// <param name="cartItems">Collection of shopping cart items</param>
			/// <param name="isShoppingCart">Shopping cart flag</param>
			void SetCartItems(int uniqueID, ICollection<CartItemInfo> cartItems, bool isShoppingCart);

			/// <summary>
			/// Update activity dates for current user and application
			/// </summary>
			/// <param name="userName">USer name</param>
			/// <param name="activityOnly">Activity only flag</param>
			/// <param name="appName">Application Name</param>
			void UpdateActivityDates(string userName, bool activityOnly, string appName);

			/// <summary>
			/// Retrive unique id for current user
			/// </summary>
			/// <param name="userName">User name</param>
			/// <param name="isAuthenticated">Authentication flag</param>
			/// <param name="ignoreAuthenticationType">Ignore authentication flag</param>
			/// <param name="appName">Application Name</param>
			/// <returns>Unique id for current user</returns>
			int GetUniqueID(string userName, bool isAuthenticated, bool ignoreAuthenticationType, string appName);

			/// <summary>
			/// Create profile record for current user
			/// </summary>
			/// <param name="userName">User name</param>
			/// <param name="isAuthenticated">Authentication flag</param>
			/// <param name="appName">Application Name</param>
			/// <returns>Number of records created</returns>
			int CreateProfileForUser(string userName, bool isAuthenticated, string appName);

			/// <summary>
			/// Retrieve colection of inactive user id's
			/// </summary>
			/// <param name="authenticationOption">Authentication option</param>
			/// <param name="userInactiveSinceDate">Date to start search from</param>
			/// <param name="appName">Application Name</param>
			/// <returns>Collection of inactive profile id's</returns>
			IList<string> GetInactiveProfiles(int authenticationOption, DateTime userInactiveSinceDate, string appName);

			/// <summary>
			/// Delete user's profile
			/// </summary>
			/// <param name="userName">User name</param>
			/// <param name="appName">Application Name</param>
			/// <returns>True, if profile successfully deleted</returns>
			bool DeleteProfile(string userName, string appName);

			/// <summary>
			/// Retrieve profile information
			/// </summary>
			/// <param name="authenticationOption">Authentication option</param>
			/// <param name="usernameToMatch">User name</param>
			/// <param name="userInactiveSinceDate">Date to start search from</param>
			/// <param name="appName">Application Name</param>
			/// <param name="totalRecords">Number of records to return</param>
			/// <returns>Collection of profiles</returns>
			IList<CustomProfileInfo> GetProfileInfo(int authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, string appName, out int totalRecords);
		}
	}
