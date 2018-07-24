using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceSharedCore.Configuration;
using Microsoft.Extensions.Options;
using ServiceSharedCore.Utilities;
using NpgsqlTypes;
using ServiceSharedCore.Models;

namespace ProfileServiceCore.Controllers
{
    public class ProfileController : Controller
    {
        private string PostgreSQLConnectionString { get; set; }
        public ProfileController(IOptions<ConnectionSettings> settings)
        {
            PostgreSQLConnectionString = settings.Value.PostgreSQLConnectionString;
        }

        private const string GET_ACCOUNT_INFO = "SELECT Account.Email, Account.FirstName, Account.LastName, Account.Address1, Account.Address2, Account.City, Account.State, Account.Zip, Account.Country, Account.Phone FROM Account, Profiles " +
            "WHERE Account.UniqueID = Profiles.UniqueID AND Profiles.Username = :Username AND Profiles.ApplicationName = :ApplicationName";
        //"SELECT Account.Email, Account.FirstName, Account.LastName, Account.Address1, Account.Address2, Account.City, Account.State, Account.Zip, Account.Country, Account.Phone FROM \"MSPETSHOP4PROFILE\".Account, \"MSPETSHOP4PROFILE\".Profiles " +
        //    "WHERE Account.UniqueID = Profiles.UniqueID AND Profiles.Username = :Username AND Profiles.ApplicationName = :ApplicationName";
        private const string CREATE_ACCOUNT = "INSERT INTO users (USERID, APPLICATIONID, USERNAME, ISANONYMOUS, LASTACTIVITYDATE) VALUES (:UserID, :AppID, :UserName, :IsAnonymous, :LastActivityDate)";
        //"INSERT INTO \"MSPETSHOP4SERVICES\".USERS (USERID, APPLICATIONID, USERNAME, ISANONYMOUS, LASTACTIVITYDATE) VALUES (:UserID, :AppID, :UserName, :IsAnonymous, :LastActivityDate)";
        [HttpPost]
        [Route("profile/getaccountinfo")]
        public AddressInfo GetAccountInfo([FromBody]AccountInfo info)
        {
            var accounts = DBFacilitator.GetList<AddressInfo>(
                PostgreSQLConnectionString,
                GET_ACCOUNT_INFO,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", info.FirebaseGUID, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName",info.AppName, NpgsqlDbType.Text) } });
            if (accounts.Count >= 1) return accounts.First();
            return null;
        }

        private const string GET_CART_ITEMS = "SELECT Cart.ItemId, Cart.Name, Cart.Type, Cart.Price, Cart.CategoryId, Cart.ProductId, Cart.Quantity FROM Profiles INNER JOIN Cart ON Profiles.UniqueID = Cart.UniqueId" +
            " WHERE Profiles.Username = :Username AND Profiles.ApplicationName = :ApplicationName AND IsShoppingCart = :IsShoppingCart";
        //"SELECT Cart.ItemId, Cart.Name, Cart.Type, Cart.Price, Cart.CategoryId, Cart.ProductId, Cart.Quantity FROM \"MSPETSHOP4PROFILE\".Profiles INNER JOIN \"MSPETSHOP4PROFILE\".Cart ON Profiles.UniqueID = Cart.UniqueId" +
        //    " WHERE Profiles.Username = :Username AND Profiles.ApplicationName = :ApplicationName AND IsShoppingCart = :IsShoppingCart";
        [HttpPost]
        [Route("profile/getcartitems")]
        public IList<CartItemInfo> GetCartItems([FromBody] GetCartItems cartitems)
        {
            return DBFacilitator.GetList<CartItemInfo>(
                PostgreSQLConnectionString,
                GET_CART_ITEMS,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", cartitems.Username, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", cartitems.AppName, NpgsqlDbType.Text) } ,
                    { new Tuple<string, string, NpgsqlDbType>(":IsShoppingCart", cartitems.IsShoppingCart ? "Y" : "N", NpgsqlDbType.Char) } }
                );
        }

        private const string GET_UNIQUEID_FOR_USER = "SELECT UniqueID FROM Profiles WHERE Username = :Username AND ApplicationName = :ApplicationName";
        //"SELECT UniqueID FROM \"MSPETSHOP4PROFILE\".Profiles WHERE Username = :Username AND ApplicationName = :ApplicationName";
        [HttpPost]
        [Route("profile/getuniqueid")]
        public Cart GetUniqueID([FromBody]GetUniqueId uniqueIdInfo)
        {
            return DBFacilitator.GetOne<Cart>(
                PostgreSQLConnectionString,
                GET_UNIQUEID_FOR_USER,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", uniqueIdInfo.UserName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", uniqueIdInfo.AppName, NpgsqlDbType.Text) }}
                );
        }

        private const string CREATE_USER = "INSERT INTO Profiles (UniqueId, Username, ApplicationName, LastActivityDate, LastUpdatedDate, IsAnonymous)" +
            " Values((SELECT COALESCE((SELECT MAX(uniqueid) + 1 FROM Profiles), 1)), :Username, :ApplicationName, :LastActivityDate, :LastUpdatedDate, :IsAnonymous) RETURNING uniqueId";
        //"INSERT INTO \"MSPETSHOP4PROFILE\".Profiles (UniqueId, Username, ApplicationName, LastActivityDate, LastUpdatedDate, IsAnonymous)" +
        //    " Values((SELECT COALESCE((SELECT MAX(uniqueid) + 1 FROM \"MSPETSHOP4PROFILE\".Profiles), 1)), :Username, :ApplicationName, :LastActivityDate, :LastUpdatedDate, :IsAnonymous) RETURNING uniqueId";

        [HttpPost]
        [Route("profile/create")]
        public int Create([FromBody]CreateProfile createProfileInfo)
        {
            return DBFacilitator.GetInteger(
                PostgreSQLConnectionString,
                CREATE_USER,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", createProfileInfo.Username, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", createProfileInfo.AppName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastActivityDate", DateTime.Now.ToString(), NpgsqlDbType.Date) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastUpdatedDate", DateTime.Now.ToString(), NpgsqlDbType.Date) },
                    { new Tuple<string, string, NpgsqlDbType>(":IsAnonymous", createProfileInfo.IsAuthenticated ? "N" : "Y", NpgsqlDbType.Char) } }
                ).Value;
        }

        private const string DELETE_CART = "DELETE FROM Cart WHERE UniqueID = :UniqueID AND IsShoppingCart = :IsShoppingCart";
        //"DELETE FROM \"MSPETSHOP4PROFILE\".Cart WHERE UniqueID = :UniqueID AND IsShoppingCart = :IsShoppingCart";
        private const string INSERT_CART_ITEMS = "INSERT INTO Cart (UniqueID, ItemId, Name, Type, Price, CategoryId, ProductId, " +
            "IsShoppingCart, Quantity) VALUES (:UniqueID, :ItemId, :Name, :Type, :Price, :CategoryId, :ProductId, :IsShoppingCart, :Quantity)";
        //"INSERT INTO \"MSPETSHOP4PROFILE\".Cart (UniqueID, ItemId, Name, Type, Price, CategoryId, ProductId, " +
        //            "IsShoppingCart, Quantity) VALUES (:UniqueID, :ItemId, :Name, :Type, :Price, :CategoryId, :ProductId, :IsShoppingCart, :Quantity)";
        [HttpPost]
        [Route("profile/setcartitems")]
        public void SetCartItems([FromBody]SetCartItems cartItemInfo)
        {
            string uniqueId = cartItemInfo.UniqueID.ToString();
            string isCart = cartItemInfo.IsShoppingCart ? "Y" : "N";
            DBFacilitator.ExecuteCommand(
                PostgreSQLConnectionString,
                DELETE_CART,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":UniqueId", uniqueId, NpgsqlDbType.Integer) },
                    { new Tuple<string, string, NpgsqlDbType>(":IsShoppingCart", isCart, NpgsqlDbType.Char) }}
                );


            foreach (var item in cartItemInfo.CartItems)
            {
                DBFacilitator.ExecuteCommand(
                PostgreSQLConnectionString,
                INSERT_CART_ITEMS,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":UniqueId", uniqueId, NpgsqlDbType.Integer) },
                    { new Tuple<string, string, NpgsqlDbType>(":IsShoppingCart", isCart, NpgsqlDbType.Char) },
                    { new Tuple<string, string, NpgsqlDbType>(":ItemId", item.ItemId, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Name", item.Name, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Type", item.Type, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":CategoryId", item.CategoryId, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ProductId", item.ProductId, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Quantity", item.Quantity.ToString(), NpgsqlDbType.Numeric) },
                    { new Tuple<string, string, NpgsqlDbType>(":Price", item.Price.ToString(), NpgsqlDbType.Numeric) }}
                );
            }
        }
        private const string UPDATE_ACTIVITY_DATES_ACTIVITY_ONLY = "UPDATE Profiles Set LastActivityDate = :LastActivityDate WHERE Username = :Username AND ApplicationName = :ApplicationName";
        //"UPDATE \"MSPETSHOP4PROFILE\".Profiles Set LastActivityDate = :LastActivityDate WHERE Username = :Username AND ApplicationName = :ApplicationName";
        private const string UPDATE_ACTIVITY_DATES = "UPDATE Profiles SET LastActivityDate = :LastActivityDate, LastUpdatedDate = :LastUpdatedDate WHERE Username = :Username AND ApplicationName = :ApplicationName";
        //"UPDATE \"MSPETSHOP4PROFILE\".Profiles SET LastActivityDate = :LastActivityDate, LastUpdatedDate = :LastUpdatedDate WHERE Username = :Username AND ApplicationName = :ApplicationName";

        [HttpPost]
        [Route("profile/updateactivitydates")]
        public void UpdateActivityDates([FromBody]ActivityDates dates)
        {
            if (dates.ActivityOnly)
            {
                DBFacilitator.ExecuteCommand(
                PostgreSQLConnectionString,
                UPDATE_ACTIVITY_DATES_ACTIVITY_ONLY,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", dates.Username, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", dates.AppName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastActivityDate", dates.ActivityOnly ? "Y" : "N", NpgsqlDbType.Date)  } }
                );
            }
            else
            {
                DBFacilitator.ExecuteCommand(
                PostgreSQLConnectionString,
                UPDATE_ACTIVITY_DATES,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":Username", dates.Username, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", dates.AppName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastActivityDate", DateTime.Now.ToString(), NpgsqlDbType.Date) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastUpdatedDate", DateTime.Now.ToString(), NpgsqlDbType.Date) } }
                );
            }
        }
        private const string GET_INACTIVE_PROFILES = "SELECT Username FROM Profiles WHERE ApplicationName = :ApplicationName AND LastActivityDate <= :LastActivityDate  AND IsAnonymous = :IsAnonymous";
        //"SELECT Username FROM \"MSPETSHOP4PROFILE\".Profiles WHERE ApplicationName = :ApplicationName AND LastActivityDate <= :LastActivityDate  AND IsAnonymous = :IsAnonymous";
        [HttpPost]
        [Route("profile/getinactiveprofiles")]
        public IList<string> GetInactiveProfiles([FromBody]InactiveProfiles inactiveInfo)
        {
            return DBFacilitator.GetList<string>(
            PostgreSQLConnectionString,
            UPDATE_ACTIVITY_DATES_ACTIVITY_ONLY,
            new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":LastActivityDate", inactiveInfo.UserInactiveSinceDate.ToString(), NpgsqlDbType.Date) },
                    { new Tuple<string, string, NpgsqlDbType>(":ApplicationName", inactiveInfo.AppName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":IsAnonymous", DateTime.Now.ToString(), NpgsqlDbType.Date)  } }
            );
        }

        private string SELECT_PROFILE_COUNT = "SELECT COUNT(*) FROM Profiles WHERE ApplicationName = :ApplicationName";
        //"SELECT COUNT(*) FROM \"MSPETSHOP4PROFILE\".Profiles WHERE ApplicationName = :ApplicationName";
        private string SELECT_PROFILES = "SELECT Username, LastActivityDate, LastUpdatedDate, IsAnonymous FROM Profiles WHERE ApplicationName = :ApplicationName";
        //"SELECT Username, LastActivityDate, LastUpdatedDate, IsAnonymous FROM \"MSPETSHOP4PROFILE\".Profiles WHERE ApplicationName = :ApplicationName";
        [HttpPost]
        [Route("profile/getprofileinfo")]
        public IList<CustomProfileInfo> GetProfileInfo([FromBody]ProfileInfo profileInfo)
        {
            string qualifiers = "";
            var parameters = new List<Tuple<string, string, NpgsqlDbType>>();
            if(!string.IsNullOrEmpty(profileInfo.UsernameToMatch))
            {
                qualifiers += " AND Username LIKE :Username";
                parameters.Add(new Tuple<string, string, NpgsqlDbType>(":Username", profileInfo.UsernameToMatch, NpgsqlDbType.Text));
            }
            if(profileInfo.UserInactiveSinceDate != null)
            {
                qualifiers += " AND LastActivityDate >= :LastActivityDate";
                parameters.Add(new Tuple<string, string, NpgsqlDbType>("LastActivityDate", profileInfo.UserInactiveSinceDate.ToString(), NpgsqlDbType.Date));
            }
            if(profileInfo.AuthenticationOption != 2)
            {
                qualifiers += " AND IsAnonymous = " + (profileInfo.AuthenticationOption == 0 ? "TRUE" : "FALSE");
            }
            var totalRecords = DBFacilitator.GetInteger(
                PostgreSQLConnectionString,
                SELECT_PROFILE_COUNT + qualifiers,
                parameters);

            if (totalRecords <= 0)
                return new List<CustomProfileInfo>();

            return DBFacilitator.GetList<CustomProfileInfo>(
                PostgreSQLConnectionString,
                SELECT_PROFILES + qualifiers,
                parameters);
        }

        private const string DELETE_PROFILE = "DELETE FROM Profiles WHERE UniqueID = :UniqueID";
        //"DELETE FROM \"MSPETSHOP4PROFILE\".Profiles WHERE UniqueID = :UniqueID";
        [HttpPost]
        [Route("profile/delete")]
        public bool DeleteProfile([FromBody]AccountInfo accountInfo)
        {
            var cart = GetUniqueID(new GetUniqueId(accountInfo.FirebaseGUID, false, true, accountInfo.AppName));
            if (cart != null)
            {
                int uniqueId = cart.UniqueId;
                var deletedRecords = DBFacilitator.GetInteger(
                    PostgreSQLConnectionString,
                    DELETE_PROFILE,
                    new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":UniqueId", uniqueId.ToString(), NpgsqlDbType.Integer) } });
                return deletedRecords == 0;
            }
            return true;
            
        }

        private const string DELETE_ACCOUNT = "DELETE FROM Account WHERE UniqueID = :UniqueID";
        //"DELETE FROM \"MSPETSHOP4PROFILE\".Account WHERE UniqueID = :UniqueID";
        private const string INSERT_ACCOUNT = "INSERT INTO Account (UniqueID, Email, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone) VALUES (:UniqueID, :Email, :FirstName, :LastName, :Address1, :Address2, :City, :State, :Zip, :Country, :Phone)";
        //"INSERT INTO \"MSPETSHOP4PROFILE\".Account (UniqueID, Email, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone) VALUES (:UniqueID, :Email, :FirstName, :LastName, :Address1, :Address2, :City, :State, :Zip, :Country, :Phone)";
        [HttpPost]
        [Route("profile/setaccountinfo")]
        public void SetAccountInfo([FromBody]SetAccountInfo accountInfo)
        {
            //TODO: use transactions here and in any other multi-call endpoints
            var deletedRecords = DBFacilitator.GetInteger(
                PostgreSQLConnectionString,
                DELETE_ACCOUNT,
                new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":UniqueId", accountInfo.UniqueId.ToString(), NpgsqlDbType.Integer) }
                });
            DBFacilitator.ExecuteCommand(
                PostgreSQLConnectionString,
                INSERT_ACCOUNT,
            new List<Tuple<string, string, NpgsqlDbType>>() {
                    { new Tuple<string, string, NpgsqlDbType>(":UniqueId", accountInfo.UniqueId.ToString(), NpgsqlDbType.Integer) },
                    { new Tuple<string, string, NpgsqlDbType>(":Email", accountInfo.AddressInfo.Email, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":FirstName", accountInfo.AddressInfo.FirstName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":LastName", accountInfo.AddressInfo.LastName, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Address1", accountInfo.AddressInfo.Address1, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Address2", accountInfo.AddressInfo.Address2, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":City", accountInfo.AddressInfo.City, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":State", accountInfo.AddressInfo.State, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Zip", accountInfo.AddressInfo.Zip, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Country", accountInfo.AddressInfo.Country, NpgsqlDbType.Text) },
                    { new Tuple<string, string, NpgsqlDbType>(":Phone", accountInfo.AddressInfo.Phone, NpgsqlDbType.Text) }
            });
        }
    }
}