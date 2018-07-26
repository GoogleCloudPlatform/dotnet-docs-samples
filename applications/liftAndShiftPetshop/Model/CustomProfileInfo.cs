using System;
using System.Text;

namespace PetShop.Model
{
    /// <summary>
    /// Business entity used to model a profile
    /// </summary>
    [Serializable]
    public class CustomProfileInfo
    {
        // Internal member variables
        private readonly string _userName;
        private readonly DateTime _lastActivityDate;
        private readonly DateTime _lastUpdatedDate;
        private readonly bool _isAnonymous;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomProfileInfo()
        {
        }

        /// <summary>
        /// Constructor with specified initial values
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="lastActivityDate">Last activity date</param>
        /// <param name="lastUpdatedDate">Last update date</param>
        /// <param name="isAnonymous">True if user is authenticated</param>
        public CustomProfileInfo(string userName, DateTime lastActivityDate, DateTime lastUpdatedDate, bool isAnonymous)
        {
            _userName = userName;
            _lastActivityDate = lastActivityDate;
            _lastUpdatedDate = lastUpdatedDate;
            _isAnonymous = isAnonymous;
        }

        // Properties
        public string UserName
        {
            get
            {
                return _userName;
            }
        }

        public DateTime LastActivityDate
        {
            get
            {
                return _lastActivityDate;
            }
        }

        public DateTime LastUpdatedDate
        {
            get
            {
                return _lastUpdatedDate;
            }
        }

        public bool IsAnonymous
        {
            get
            {
                return _isAnonymous;
            }
        }
    }
}
