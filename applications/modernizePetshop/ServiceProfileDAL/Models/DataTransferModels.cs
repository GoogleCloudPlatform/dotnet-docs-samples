using PetShop.Model;
using System;
using System.Collections.Generic;

namespace ServiceProfileDAL.Models
{
    public class CreateProfile
    {
        public string Username { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AppName { get; set; }
        public CreateProfile(string username, bool isAuthd, string appname)
        {
            Username = username;
            IsAuthenticated = isAuthd;
            AppName = appname;
        }
    }
    public class AccountInfo
    {
        public string FirebaseGuid { get; set; }
        public string AppName { get; set; }
        public AccountInfo(string firebaseGuid, string appname)
        {
            FirebaseGuid = firebaseGuid;
            AppName = appname;
        }
    }
    public class GetCartItems
    {
        public string Username { get; set; }
        public string AppName { get; set; }
        public bool IsShoppingCart { get; set; }
        public GetCartItems(string userName, string appName, bool isShoppingCart)
        {
            Username = userName;
            AppName = appName;
            IsShoppingCart = isShoppingCart;
        }
    }
    public class InactiveProfiles
    {
        public int AuthenticationOption { get; set; }
        public DateTime UserInactiveSinceDate { get; set; }
        public string AppName { get; set; }
        public InactiveProfiles(int authOpt, DateTime inactiveDate, string appname)
        {
            AuthenticationOption = authOpt;
            UserInactiveSinceDate = inactiveDate;
            AppName = appname;
        }
    }
    public class ProfileInfo
    {
        public int AuthenticationOption { get; set; }
        public string UsernameToMatch { get; set; }
        public DateTime UserInactiveSinceDate { get; set; }
        public string AppName { get; set; }
        public ProfileInfo(int authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, string appName)
        {
            AuthenticationOption = authenticationOption;
            UsernameToMatch = usernameToMatch;
            UserInactiveSinceDate = userInactiveSinceDate;
            AppName = appName;
        }
    }
    public class ProfileInfoReturn
    {
        public IList<CustomProfileInfo> profiles { get; set; }
        public int numOfRows { get; set; }
    }
    public class ProfileQueryParameter
    {
        public string UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IgnoreAuthenticationType { get; set; }
        public string AppName { get; set; }
        public ProfileQueryParameter(string username, bool isAuthd, bool ignoreAuthType, string appName)
        {
            UserName = username;
            IsAuthenticated = isAuthd;
            IgnoreAuthenticationType = ignoreAuthType;
            AppName = appName;
        }
    }
    public class SetAccountInfo
    {
        public int UniqueId { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public SetAccountInfo(int uniqueId, AddressInfo addressInfo)
        {
            UniqueId = uniqueId;
            AddressInfo = addressInfo;
        }
    }
    public class SetCartItems
    {
        public int UniqueID { get; set; }
        public ICollection<CartItemInfo> CartItems { get; set; }
        public bool IsShoppingCart { get; set; }
        public SetCartItems(int uniqueId, ICollection<CartItemInfo> info, bool isCart)
        {
            UniqueID = uniqueId;
            CartItems = info;
            IsShoppingCart = isCart;
        }
    }
    public class ActivityDates
    {
        public string Username { get; set; }
        public bool ActivityOnly { get; set; }
        public string AppName { get; set; }
        public ActivityDates(string username, bool activityOnly, string appName)
        {
            Username = username;
            ActivityOnly = activityOnly;
            AppName = appName;
        }
    }
    public class UserProfileId
    {
        public int UniqueId { get; set; }
    }
}
