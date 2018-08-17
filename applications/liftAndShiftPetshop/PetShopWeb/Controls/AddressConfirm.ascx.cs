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

using PetShop.Model;

namespace PetShop.Web
{
    public partial class AddressConfirm : System.Web.UI.UserControl
    {
        /// <summary>
        ///	Control property to set the address
        /// </summary>
        public AddressInfo Address
        {
            set
            {
                if (value != null)
                {
                    //if(!string.IsNullOrEmpty(value.FirstName))
                    //	ltlFirstName.Text = value.FirstName;
                    //if(!string.IsNullOrEmpty(value.LastName))
                    //	ltlLastName.Text = value.LastName;
                    //if(!string.IsNullOrEmpty(value.Address1))
                    //	ltlAddress1.Text = value.Address1;
                    //if(!string.IsNullOrEmpty(value.Address2))
                    //	ltlAddress2.Text = value.Address2;
                    //if(!string.IsNullOrEmpty(value.City))
                    //	ltlCity.Text = value.City;
                    //if(!string.IsNullOrEmpty(value.Zip))
                    //	ltlZip.Text = value.Zip;	   
                    //if(!string.IsNullOrEmpty(value.State))
                    //	ltlState.Text = value.State;
                    //if(!string.IsNullOrEmpty(value.Country))
                    //	ltlCountry.Text = value.Country;
                    //if(!string.IsNullOrEmpty(value.Phone))
                    //	ltlPhone.Text = value.Phone;
                    //if(!string.IsNullOrEmpty(value.Email))
                    //	ltlEmail.Text = value.Email;
                }
            }
        }
    }
}
