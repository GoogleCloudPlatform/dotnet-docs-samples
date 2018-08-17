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
