using System;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Globalization;
using System.Collections.Specialized;
using System.Data;
using System.Data.OracleClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration.Provider;
using PetShop.DBUtility;

namespace PetShop.Membership {
	class OracleMembershipProvider : MembershipProvider {

        // Private members
		private const int SALT_SIZE_IN_BYTES = 16;
		private bool enablePasswordRetrieval;
		private bool enablePasswordReset;
		private bool requiresQuestionAndAnswer;
		private string applicationName;
		private int applicationId = 0;
		private bool requiresUniqueEmail;
		private string connectionStringName;
		private string hashAlgorithmType;
		private int maxInvalidPasswordAttempts;
		private int passwordAttemptWindow;
		private int minRequiredPasswordLength;
		private int minRequiredNonalphanumericCharacters;
		private string passwordStrengthRegularExpression;
		private MembershipPasswordFormat passwordFormat;
        
		/// <summary>
        /// The name of the application using the custom membership provider.
		/// </summary>
		public override bool EnablePasswordRetrieval {
			get {
				return enablePasswordRetrieval;
			}
		}

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
		public override bool EnablePasswordReset {
			get {
				return enablePasswordReset;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
		public override bool RequiresQuestionAndAnswer {
			get {
				return requiresQuestionAndAnswer;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
		public override bool RequiresUniqueEmail {
			get {
				return requiresUniqueEmail;
			}
		}

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
		public override MembershipPasswordFormat PasswordFormat {
			get {
				return passwordFormat;
			}
		}

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
		public override int MaxInvalidPasswordAttempts {
			get {
				return maxInvalidPasswordAttempts;
			}
		}

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
		public override int PasswordAttemptWindow {
			get {
				return passwordAttemptWindow;
			}
		}

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
		public override int MinRequiredPasswordLength {
			get {
				return minRequiredPasswordLength;
			}
		}

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
		public override int MinRequiredNonAlphanumericCharacters {
			get {
				return minRequiredNonalphanumericCharacters;
			}
		}

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
		public override string PasswordStrengthRegularExpression {
			get {
				return passwordStrengthRegularExpression;
			}
		}

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
		public override string ApplicationName {
			get {
				return applicationName;
			}
			set {
				applicationName = value;
			}
		}

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection config) {

            // Check config
			if(config == null)
				throw new ArgumentNullException("config");

            // Get name
			if(String.IsNullOrEmpty(name))
				name = "OracleMembershipProvider";

            // Get descripton
			if(string.IsNullOrEmpty(config["description"])) {
				config.Remove("description");
				config.Add("description", "Oracle Membership Provider");
			}

            // Initialize base class
			base.Initialize(name, config);

            // Get values from config
			enablePasswordRetrieval = GetBooleanValue(config, "enablePasswordRetrieval", false);
			enablePasswordReset = GetBooleanValue(config, "enablePasswordReset", true);
			requiresQuestionAndAnswer = GetBooleanValue(config, "requiresQuestionAndAnswer", true);
			requiresUniqueEmail = GetBooleanValue(config, "requiresUniqueEmail", true);
			maxInvalidPasswordAttempts = GetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
			passwordAttemptWindow = GetIntValue(config, "passwordAttemptWindow", 10, false, 0);
			minRequiredPasswordLength = GetIntValue(config, "minRequiredPasswordLength", 7, false, 128);
			minRequiredNonalphanumericCharacters = GetIntValue(config, "minRequiredNonalphanumericCharacters", 1, true, 128);
			
            // Get hash algorhithm
            hashAlgorithmType = config["hashAlgorithmType"];
			if(String.IsNullOrEmpty(hashAlgorithmType))
				hashAlgorithmType = "SHA1";

            // Get password validation Regular Expression
			passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
			if(passwordStrengthRegularExpression != null) {
				passwordStrengthRegularExpression = passwordStrengthRegularExpression.Trim();
				if(passwordStrengthRegularExpression.Length != 0) {
					try {
						Regex regex = new Regex(passwordStrengthRegularExpression);
					}
					catch(ArgumentException e) {
						throw new ProviderException(e.Message, e);
					}
				}
			}
			else
				passwordStrengthRegularExpression = string.Empty;

            // Get application name
			applicationName = config["applicationName"];
			if(string.IsNullOrEmpty(applicationName))
				applicationName = GetDefaultAppName();
			if(applicationName.Length > 255)
				throw new ProviderException("Provider application name is too long, max length is 255.");

            // Get application ID
			applicationId = GetApplicationID(applicationName, true);
			if(applicationId == 0)
				throw new ProviderException("Provider application ID error.");

            // Get password format
			string strTemp = config["passwordFormat"];
			if(strTemp == null)
				strTemp = "Hashed";
			switch(strTemp) {
				case "Clear":
					passwordFormat = MembershipPasswordFormat.Clear;
					break;
				case "Encrypted":
					passwordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Hashed":
					passwordFormat = MembershipPasswordFormat.Hashed;
					break;
				default:
					throw new ProviderException("Bad password format");
			}

			if(passwordFormat == MembershipPasswordFormat.Hashed && enablePasswordRetrieval)
				throw new ProviderException("Provider cannot retrieve hashed password");

            // Get connection string name
			connectionStringName = config["connectionStringName"];
			if(connectionStringName == null || connectionStringName.Length < 1)
				throw new ProviderException("Connection name not specified");

            // Clean up config
			config.Remove("connectionStringName");
			config.Remove("enablePasswordRetrieval");
			config.Remove("enablePasswordReset");
			config.Remove("requiresQuestionAndAnswer");
			config.Remove("applicationName");
			config.Remove("requiresUniqueEmail");
			config.Remove("maxInvalidPasswordAttempts");
			config.Remove("passwordAttemptWindow");
			config.Remove("passwordFormat");
			config.Remove("name");
			config.Remove("description");
			config.Remove("minRequiredPasswordLength");
			config.Remove("minRequiredNonalphanumericCharacters");
			config.Remove("passwordStrengthRegularExpression");
			config.Remove("hashAlgorithmType");

			if(config.Count > 0) {
				string attribUnrecognized = config.GetKey(0);
				if(!String.IsNullOrEmpty(attribUnrecognized))
					throw new ProviderException("Provider unrecognized attribute: " + attribUnrecognized);
			}
		}

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="userId">The user ID</param>
        /// <param name="status">A System.Web.Security.MembershipCreateStatus enumeration value indicating whether the user was created successfully.</param>
        /// <returns></returns>
		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object userId, out MembershipCreateStatus status) {
			
			
			if(!ValidateParameter(ref password, true, true, false, 0)) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if(passwordStrengthRegularExpression != null) {
				if(!Regex.IsMatch(password, passwordStrengthRegularExpression)) {
					status = MembershipCreateStatus.InvalidPassword;
					return null;
				}
			}

			if(minRequiredNonalphanumericCharacters > 0) {
				if(Regex.Matches(password, @"\W").Count < minRequiredNonalphanumericCharacters) {
					status = MembershipCreateStatus.InvalidPassword;
					return null;
				}
			}

			if(password.Length < minRequiredPasswordLength) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			string salt = GenerateSalt();
			string pass = EncodePassword(password, (int)passwordFormat, salt);
			if(pass.Length > 128) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if(!ValidateParameter(ref username, true, true, true, 255)) {
				status = MembershipCreateStatus.InvalidUserName;
				return null;
			}

			if(!ValidateParameter(ref email, RequiresUniqueEmail, RequiresUniqueEmail, false, 128)) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if(!ValidateParameter(ref passwordQuestion, RequiresQuestionAndAnswer, true, false, 255)) {
				status = MembershipCreateStatus.InvalidQuestion;
				return null;
			}

			if(!ValidateParameter(ref passwordAnswer, RequiresQuestionAndAnswer, true, false, 128)) {
				status = MembershipCreateStatus.InvalidAnswer;
				return null;
			}

			if(userId != null)
				throw new ArgumentException("userId Parameter must be null");

			status = MembershipCreateStatus.UserRejected;

			//Create connection
			OracleConnection connection = new OracleConnection(OracleHelper.ConnectionStringMembership);
			connection.Open();
			OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

			try {

				DateTime dt = DateTime.Now;
				bool isUserNew = true;

				// Step 1: Check if the user exists in the Users table: create if not	   
				int uid = GetUserID(transaction, applicationId, username, true, false, dt, out isUserNew);
				if(uid == 0) { // User not created successfully!
					status = MembershipCreateStatus.ProviderError;
					return null;
				}

				// Step 2: Check if the user exists in the Membership table: Error if yes.
				if(IsUserInMembership(transaction, uid)) {
					status = MembershipCreateStatus.DuplicateUserName;
					return null;
				}

				// Step 3: Check if Email is duplicate
				if(IsEmailInMembership(transaction, email, applicationId)) {
					status = MembershipCreateStatus.DuplicateEmail;
					return null;
				}

				// Step 4: Create user in Membership table					
				int pFormat = (int)passwordFormat;
				if(!InsertUser(transaction, uid, email, pass, pFormat, salt, "", "", isApproved, dt)) {
					status = MembershipCreateStatus.ProviderError;
					return null;
				}

				// Step 5: Update activity date if user is not new
				if(!isUserNew) {
					if(!UpdateLastActivityDate(transaction, uid, dt)) {
						status = MembershipCreateStatus.ProviderError;
						return null;
					}
				}

				status = MembershipCreateStatus.Success;

				return new MembershipUser(this.Name, username, uid, email, passwordQuestion, null, isApproved, false, dt, dt, dt, dt, DateTime.MinValue);
			}
			catch(Exception) {
				if(status == MembershipCreateStatus.Success)
					status = MembershipCreateStatus.ProviderError;
				throw;
			}
			finally {
				if(status == MembershipCreateStatus.Success)
					transaction.Commit();
				else
					transaction.Rollback();
				connection.Close();
				connection.Dispose();
			}

		}
        
        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns></returns>
		public override bool ValidateUser(string username, string password) {
			if(!ValidateParameter(ref username, true, true, false, 255))
				return false;

			if(!ValidateParameter(ref password, true, true, false, 128))
				return false;

			//Create connection
			OracleConnection connection = new OracleConnection(OracleHelper.ConnectionStringMembership);
			connection.Open();
			OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

			bool success = false;

			try {

				DateTime dt = DateTime.Now;
				bool isUserNew = false;
				bool userIsApproved = false;

				int uid = GetUserID(transaction, applicationId, username, false, false, dt, out isUserNew);
				if(uid <= 0)
					success = false;

				if(CheckPassword(uid, password, out userIsApproved) && userIsApproved) {

					if(!UpdateLastActivityDate(transaction, uid, dt))
						success = false;

					if(!UpdateLastLoginDate(transaction, uid, dt))
						success = false;

					success = true;
				}
			}
			finally {
				if(success)
					transaction.Commit();
				else
					transaction.Rollback();
				connection.Close();
				connection.Dispose();
			}

			return success;
		}
        
        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns></returns>
		public override bool ChangePassword(string username, string oldPassword, string newPassword) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns></returns>
		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A System.Web.Security.MembershipUserCollection collection that contains a page of pageSizeSystem.Web.Security.MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A System.Web.Security.MembershipUserCollection collection that contains a page of pageSizeSystem.Web.Security.MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A System.Web.Security.MembershipUserCollection collection that contains a page of pageSizeSystem.Web.Security.MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>The number of users currently accessing the application.</returns>
		public override int GetNumberOfUsersOnline() {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>The password for the specified user name.</returns>
		public override string GetPassword(string username, string answer) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A System.Web.Security.MembershipUser object populated with the specified user's information from the data source.</returns>
		public override MembershipUser GetUser(string username, bool userIsOnline) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets information from the data source for a user based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A System.Web.Security.MembershipUser object populated with the specified user's information from the data source.</returns>
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>The user name associated with the specified e-mail address. If no match is found, return null.</returns>
		public override string GetUserNameByEmail(string email) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
		public override string ResetPassword(string username, string answer) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <param name="userName">The membership user to clear the lock status for.</param>
        /// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
		public override bool UnlockUser(string userName) {
			throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A System.Web.Security.MembershipUser object that represents the user to update and the updated information for the user.</param>
		public override void UpdateUser(MembershipUser user) {
			throw new Exception("The method or operation is not implemented.");
		}

		// Private utility methods

        // Oveloaded method to check password
        private bool CheckPassword(int userId, string password, out int status) {
            string salt;
            bool userIsApproved;
            int passwordFormat;
            string pass = GetPasswordWithFormat(userId, null, false, out passwordFormat, out status, out salt, out userIsApproved);
            string pass2 = EncodePassword(password, passwordFormat, salt);
            return (pass == pass2);
        }
        private bool CheckPassword(int userId, string password, out bool userIsApproved) {
            string salt;
            int passwordFormat, status;
            string pass = GetPasswordWithFormat(userId, null, false, out passwordFormat, out status, out salt, out userIsApproved);
            string pass2 = EncodePassword(password, passwordFormat, salt);
            return (pass == pass2);
        }

        // Generate salt to secure password encryption
        private string GenerateSalt() {
            byte[] buffer = new byte[SALT_SIZE_IN_BYTES];
            (new RNGCryptoServiceProvider()).GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        // Encode password
        private string EncodePassword(string pass, int passwordFormat, string salt) {
            if (passwordFormat == 0) // MembershipPasswordFormat.Clear
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            if (passwordFormat == 1) { // MembershipPasswordFormat.Hashed
                HashAlgorithm s = HashAlgorithm.Create(hashAlgorithmType);

                // If the hash algorithm is null (and came from config), throw a config exception
                if (s == null)
                    throw new ProviderException("Could not create a hash algorithm");

                bRet = s.ComputeHash(bAll);
            }
            else
                bRet = EncryptPassword(bAll);

            return Convert.ToBase64String(bRet);
        }
        
        // Discover booean value from config by specifying name
		private static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue) {
			string sValue = config[valueName];
			if(sValue == null)
				return defaultValue;

			if(sValue == "true")
				return true;

			if(sValue == "false")
				return false;

			throw new Exception("The value must be a boolean for property '" + valueName + "'");
		}

        // Discover int value from config by specifying name
		private static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed) {
			string sValue = config[valueName];

			if(sValue == null) {
				return defaultValue;
			}

			int iValue;
			try {
				iValue = Convert.ToInt32(sValue, CultureInfo.InvariantCulture);
			}
			catch(InvalidCastException e) {
				if(zeroAllowed)
					throw new Exception("The value must be a positive integer for property '" + valueName + "'", e);

				throw new Exception("The value must be a positive integer for property '" + valueName + "'", e);
			}

			if(zeroAllowed && iValue < 0)
				throw new Exception("The value must be a non-negative integer for property '" + valueName + "'");

			if(!zeroAllowed && iValue <= 0)
				throw new Exception("The value must be a non-negative integer for property '" + valueName + "'");

			if(maxValueAllowed > 0 && iValue > maxValueAllowed)
				throw new Exception("The value is too big for '" + valueName + "' must be smaller than " + maxValueAllowed.ToString(CultureInfo.InvariantCulture));

			return iValue;
		}

        // Create default application name
		private static string GetDefaultAppName() {
			string appName = System.Web.HttpRuntime.AppDomainAppVirtualPath;
			if(string.IsNullOrEmpty(appName))
				return "/";

			return appName;
		}

        // Parameter validator
		private static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize) {
			if(param == null) {
				if(checkForNull)
					return false;

				return true;
			}

			param = param.Trim();
			if((checkIfEmpty && param.Length < 1) || (maxSize > 0 && param.Length > maxSize) || (checkForCommas && param.IndexOf(",") != -1)) 
				return false;
			
			return true;
		}

                // Check for reader for null
		private static string GetNullableString(OracleDataReader reader, int col) {
			if(reader.IsDBNull(col) == false)
				return reader.GetString(col);
			return null;
		}

		
		// Data Access methods

        // Retrieve password
		private static string GetPasswordWithFormat(int userId, string passwordAnswer, bool requiresQuestionAndAnswer, out int passwordFormat, out int status, out string passwordSalt, out bool userIsApproved) {

			string storedPasswordAnswer;
			string pass;

			passwordFormat = 0;
			status = 1; // status = user not found
			passwordSalt = String.Empty;
			userIsApproved = false;
			if(userId == 0)
				return null;

			string select = "SELECT PASSWORDFORMAT, PASSWORD, PASSWORDANSWER, PASSWORDSALT, ISAPPROVED FROM MEMBERSHIP m, USERS u WHERE m.USERID = :UserId AND m.USERID = u.UserId";

			OracleParameter selectParam = new OracleParameter(":UserId", OracleType.Number, 10);
			selectParam.Value = userId;

			OracleDataReader reader = OracleHelper.ExecuteReader(OracleHelper.ConnectionStringMembership, CommandType.Text, select, selectParam);

			if(!reader.Read()) { // Zero rows read = user-not-found
				reader.Close();
				return null;
			}

			passwordFormat = reader.GetInt32(0);
			pass = GetNullableString(reader, 1);
			storedPasswordAnswer = GetNullableString(reader, 2);
			passwordSalt = GetNullableString(reader, 3);
			userIsApproved = OracleHelper.OraBool(reader.GetString(4));

			if(requiresQuestionAndAnswer && String.Compare(storedPasswordAnswer, passwordAnswer, true, CultureInfo.InvariantCulture) != 0) {
				status = 3;
				pass = null;
			}
			else
				status = 0;

			reader.Close();
			return pass;
		}

        // Update last login date and time
		private static bool UpdateLastLoginDate(OracleTransaction transaction, int userId, DateTime dt) {

			string update = "UPDATE MEMBERSHIP SET LASTLOGINDATE = :LastLoginDate WHERE USERID = :UserID";
			OracleParameter[] updateParms = { new OracleParameter(":UserID", OracleType.Number, 10), new OracleParameter(":LastLoginDate", OracleType.DateTime) };
			updateParms[0].Value = userId;
			updateParms[1].Value = dt;

			if(OracleHelper.ExecuteNonQuery(transaction, CommandType.Text, update, updateParms) != 1)
				return false;
			else
				return true;
		}

        // Update last activity date and time
		private static bool UpdateLastActivityDate(OracleTransaction transaction, int userId, DateTime dt) {

			string update = "UPDATE  USERS SET LASTACTIVITYDATE = :LastActivityDate WHERE USERID = :UserID";
			OracleParameter[] updateParms = { new OracleParameter(":UserID", OracleType.Number, 10), new OracleParameter(":LastActivityDate", OracleType.DateTime) };
			updateParms[0].Value = userId;
			updateParms[1].Value = dt;

			if(OracleHelper.ExecuteNonQuery(transaction, CommandType.Text, update, updateParms) != 1)
				return false;
			else
				return true;
		}

        // Insert a new user
		private static bool InsertUser(OracleTransaction transaction, int userId, string email, string password, int passFormat, string passSalt, string passQuestion, string passAnswer, bool isApproved, DateTime dt) {

			string insert = "INSERT INTO MEMBERSHIP (USERID, EMAIL, PASSWORD, PASSWORDFORMAT, PASSWORDSALT, PASSWORDQUESTION, PASSWORDANSWER, ISAPPROVED, CREATEDDATE, LASTLOGINDATE, LASTPASSWORDCHANGEDDATE) VALUES (:UserID, :Email, :Pass, :PasswordFormat, :PasswordSalt, :PasswordQuestion, :PasswordAnswer, :IsApproved, :CDate, :LLDate, :LPCDate)";
			OracleParameter[] insertParms = { new OracleParameter(":UserID", OracleType.Number, 10), new OracleParameter(":Email", OracleType.VarChar, 128), new OracleParameter(":Pass", OracleType.VarChar, 128), new OracleParameter(":PasswordFormat", OracleType.Number, 10), new OracleParameter(":PasswordSalt", OracleType.VarChar, 128), new OracleParameter(":PasswordQuestion", OracleType.VarChar, 256), new OracleParameter(":PasswordAnswer", OracleType.VarChar, 128), new OracleParameter(":IsApproved", OracleType.VarChar, 1), new OracleParameter(":CDate", OracleType.DateTime), new OracleParameter(":LLDate", OracleType.DateTime), new OracleParameter(":LPCDate", OracleType.DateTime) };
			insertParms[0].Value = userId;
			insertParms[1].Value = email;
			insertParms[2].Value = password;
			insertParms[3].Value = passFormat;
			insertParms[4].Value = passSalt;
			insertParms[5].Value = passQuestion;
			insertParms[6].Value = passAnswer;
			insertParms[7].Value = OracleHelper.OraBit(isApproved);
			insertParms[8].Value = dt;
			insertParms[9].Value = dt;
			insertParms[10].Value = dt;

			if(OracleHelper.ExecuteNonQuery(transaction, CommandType.Text, insert, insertParms) != 1)
				return false;
			else
				return true;

		}

        // Check if email exist in membership database
		private static bool IsEmailInMembership(OracleTransaction transaction, string email, int applicationID) {
			string select = "SELECT u.UserId FROM MEMBERSHIP m, USERS u WHERE u.APPLICATIONID = :AppId AND m.USERID = u.USERID AND m.EMAIL = :Email";
			OracleParameter[] selectParms = { new OracleParameter(":AppId", OracleType.Number, 10), new OracleParameter(":Email", OracleType.VarChar, 128) };
			selectParms[0].Value = applicationID;
			selectParms[1].Value = email;

			object result = OracleHelper.ExecuteScalar(transaction, CommandType.Text, select, selectParms);
			if(result != null && (Convert.ToInt32(result) != 0))
				return true;
			else
				return false;
		}

        // Check if user exist in membership database
		private static bool IsUserInMembership(OracleTransaction transaction, int UserId) {

			string select = "SELECT USERID FROM MEMBERSHIP WHERE USERID = :UserID";
			OracleParameter selectParam = new OracleParameter(":UserID", OracleType.Number, 10);
			selectParam.Value = UserId;

			object result = OracleHelper.ExecuteScalar(transaction, CommandType.Text, select, selectParam);
			if(result != null && (Convert.ToInt32(result) != 0))
				return true;
			else
				return false;
		}

        // Get user ID
		private static int GetUserID(OracleTransaction transaction, int applicationID, string userName, bool createIfNeeded, bool newUserIsAnonymous, DateTime lastActivityDate, out bool isUserNew) {

			isUserNew = false;

			string select = "SELECT UserId FROM USERS WHERE APPLICATIONID = :AppId AND USERNAME = :UserName";
			string selectId = "SELECT USERID.NEXTVAL FROM DUAL";
			string insert = "INSERT INTO USERS (USERID, APPLICATIONID, USERNAME, ISANONYMOUS, LASTACTIVITYDATE) VALUES (:UserID, :AppID, :UserName, :IsAnonymous, :LastActivityDate)";

			//Get UserId
			OracleParameter[] selectParms = { new OracleParameter(":AppId", OracleType.Number, 10), new OracleParameter(":UserName", OracleType.VarChar, 256) };
			selectParms[0].Value = applicationID;
			selectParms[1].Value = userName;


			object lookupResult = OracleHelper.ExecuteScalar(transaction, CommandType.Text, select, selectParms);

			if(lookupResult != null)
				return Convert.ToInt32(lookupResult);

			// Create new user
			isUserNew = true;
			if(createIfNeeded) {
				object userId = OracleHelper.ExecuteScalar(transaction, CommandType.Text, selectId, null);
				if(userId != null) {
					int uid = Convert.ToInt32(userId);
					OracleParameter[] insertParms = { new OracleParameter(":UserID", OracleType.Number, 10), new OracleParameter(":AppID", OracleType.Number, 10), new OracleParameter(":UserName", OracleType.VarChar, 256), new OracleParameter(":IsAnonymous", OracleType.VarChar, 1), new OracleParameter(":LastActivityDate", OracleType.DateTime) };
					insertParms[0].Value = uid;
					insertParms[1].Value = applicationID;
					insertParms[2].Value = userName;
					insertParms[3].Value = OracleHelper.OraBit(newUserIsAnonymous);
					insertParms[4].Value = new DateTime(lastActivityDate.Year, lastActivityDate.Month, lastActivityDate.Day, lastActivityDate.Hour, lastActivityDate.Minute, lastActivityDate.Second);

					if(OracleHelper.ExecuteNonQuery(transaction, CommandType.Text, insert, insertParms) != 0)
						return uid;
				}
			}

			//Something went wrong
			return 0;
		}

        // Get application ID
		private static int GetApplicationID(string applicationName, bool createIfNeeded) {

			string select = "SELECT APPLICATIONID FROM APPLICATIONS WHERE APPLICATIONNAME = :AppName";
			string selectId = "SELECT APPLICATIONID.NEXTVAL FROM DUAL";
			string insert = "INSERT INTO APPLICATIONS (APPLICATIONID, APPLICATIONNAME) VALUES (:AppID, :AppName)";

			//Create connection
			OracleConnection connection = new OracleConnection(OracleHelper.ConnectionStringMembership);
			connection.Open();

			try {

				OracleParameter selectParam = new OracleParameter(":AppName", OracleType.VarChar, 256);
				selectParam.Value = applicationName;

				object lookupResult = OracleHelper.ExecuteScalar(connection, CommandType.Text, select, selectParam);

				if(lookupResult != null)
					return Convert.ToInt32(lookupResult);

				if(createIfNeeded) {

					object appId = OracleHelper.ExecuteScalar(connection, CommandType.Text, selectId, null);

					if((appId != null)) {
						int aid = Convert.ToInt32(appId);
						OracleParameter[] insertParms = { new OracleParameter(":AppID", OracleType.Number, 10), new OracleParameter(":AppName", OracleType.VarChar, 256) };
						insertParms[0].Value = aid;
						insertParms[1].Value = applicationName;
						if(OracleHelper.ExecuteNonQuery(connection, CommandType.Text, insert, insertParms) == 1)
							return aid;
					}
				}
			}
			finally {
				connection.Close();
				connection.Dispose();
			}

			return 0;
		}
	}
}
