using System;
using System.Text;

namespace PetShop.Model {
	
	/// <summary>
	/// Business entity used to model a profile
	/// </summary>
	[Serializable]
	public class CustomProfileInfo {

		// Internal member variables
		private string userName;
		private DateTime lastActivityDate;
		private DateTime lastUpdatedDate;
		private bool isAnonymous;

		/// <summary>
		/// Default constructor
		/// </summary>
		public CustomProfileInfo() {
		}

		/// <summary>
		/// Constructor with specified initial values
		/// </summary>
		/// <param name="userName">User Name</param>
		/// <param name="lastActivityDate">Last activity date</param>
		/// <param name="lastUpdatedDate">Last update date</param>
		/// <param name="isAnonymous">True if user is authenticated</param>
		public CustomProfileInfo(string userName, DateTime lastActivityDate, DateTime lastUpdatedDate, bool isAnonymous) {
			this.userName = userName;
			this.lastActivityDate = lastActivityDate;
			this.lastUpdatedDate = lastUpdatedDate;
			this.isAnonymous = isAnonymous;
		}

		// Properties
		public string UserName {
			get {
				return userName;
			}
		}

		public DateTime LastActivityDate {
			get {
				return lastActivityDate;
			}
		}

		public DateTime LastUpdatedDate {
			get {
				return lastUpdatedDate;
			}
		}

		public bool IsAnonymous {
			get {
				return isAnonymous;
			}
		}
	}
}
