using PetShop.IProfileDAL;
using System;
using PetShop.Model;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using ServiceProfileDAL.Models;

namespace ServiceProfileDAL
{
    public class PetShopProfileProvider : IPetShopProfileProvider
    {
        string requestURL = ConfigurationManager.AppSettings["ProfileBaseURL"] + "profile";
        public int CreateProfileForUser(string userName, bool isAuthenticated, string appName)
        {
            var newProfile = Utility.ServiceCall(requestURL + "/create", JsonConvert.SerializeObject(new CreateProfile(userName, isAuthenticated, appName)));
            if (string.IsNullOrEmpty(newProfile)) return 0;
            return JsonConvert.DeserializeObject<int>(newProfile);
        }

        public bool DeleteProfile(string userName, string appName)
        {
            Utility.ServiceCall(requestURL + "/delete", JsonConvert.SerializeObject(new AccountInfo(userName, appName)));
            return true;
        }

        public AddressInfo GetAccountInfo(string firebaseGuid, string appName)
        {
            var accountInfo = new AccountInfo(firebaseGuid, appName);

            var returned = Utility.ServiceCall(requestURL + "/getaccountinfo", JsonConvert.SerializeObject(accountInfo));
            return JsonConvert.DeserializeObject<AddressInfo>(returned);
        }

        public IList<CartItemInfo> GetCartItems(string userName, string appName, bool isShoppingCart)
        {
            var getCartItems = new GetCartItems(userName, appName, isShoppingCart);

            var returned = Utility.ServiceCall(requestURL + "/getcartitems", JsonConvert.SerializeObject(getCartItems));
            var converted =  JsonConvert.DeserializeObject<IList<CartItemInfo>>(returned);
            return converted != null ? converted : new List<CartItemInfo>();
        }

        public IList<string> GetInactiveProfiles(int authenticationOption, DateTime userInactiveSinceDate, string appName)
        {
            var getInactiveItems = new InactiveProfiles(authenticationOption, userInactiveSinceDate, appName);

            var returned = Utility.ServiceCall(requestURL + "/getinactiveprofiles", JsonConvert.SerializeObject(getInactiveItems));
            return JsonConvert.DeserializeObject<IList<string>>(returned);
        }

        public IList<CustomProfileInfo> GetProfileInfo(int authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, string appName, out int totalRecords)
        {
            var getProfileInfo = new ProfileInfo(authenticationOption, usernameToMatch, userInactiveSinceDate, appName);

            var returned = Utility.ServiceCall(requestURL + "/getinactiveprofiles", JsonConvert.SerializeObject(getProfileInfo));
            var returnedObject = JsonConvert.DeserializeObject<ProfileInfoReturn>(returned);
            totalRecords = returnedObject.numOfRows;
            return returnedObject.profiles;
        }

        public int GetUniqueID(string userName, bool isAuthenticated, bool ignoreAuthenticationType, string appName)
        {
            var uniqueIdQueryParam = new ProfileQueryParameter(userName, isAuthenticated, ignoreAuthenticationType, appName);
            var returned = Utility.ServiceCall(requestURL + "/getuniqueid", JsonConvert.SerializeObject(uniqueIdQueryParam));

            var userProfileId = JsonConvert.DeserializeObject<UserProfileId>(returned);
            if(userProfileId == null)
            {
                return CreateProfileForUser(userName, isAuthenticated, appName);
            }
            else
            {
                return userProfileId.UniqueId;
            }
        }

        public void SetAccountInfo(int uniqueID, AddressInfo addressInfo)
        {
            var getUniqueId = new SetAccountInfo(uniqueID, addressInfo);
            Utility.ServiceCall(requestURL + "/setaccountinfo", JsonConvert.SerializeObject(getUniqueId));
        }

        public void SetCartItems(int uniqueID, ICollection<CartItemInfo> cartItems, bool isShoppingCart)
        {
            var setCartItems = new SetCartItems(uniqueID, cartItems, isShoppingCart);
            Utility.ServiceCall(requestURL + "/setcartitems", JsonConvert.SerializeObject(setCartItems));
        }

        public void UpdateActivityDates(string userName, bool activityOnly, string appName)
        {
            var updateActivityDates = new ActivityDates(userName, activityOnly, appName);
            Utility.ServiceCall(requestURL + "/updateactivitydates", JsonConvert.SerializeObject(updateActivityDates));
        }
    }
}
