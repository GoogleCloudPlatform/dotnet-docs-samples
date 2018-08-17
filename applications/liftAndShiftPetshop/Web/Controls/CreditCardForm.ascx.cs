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
using System;
using System.Web.UI.WebControls;

namespace PetShop.Web {
	public partial class CreditCardForm : System.Web.UI.UserControl {

		/// <summary>
		/// Custom validator to check the expiration date
		/// </summary>
		protected void ServerValidate(object source, ServerValidateEventArgs value) {
			DateTime dt;
			if(DateTime.TryParse(value.Value, out dt))
				value.IsValid = (dt > DateTime.Now);
			else
				value.IsValid = false;
		}

		/// <summary>
		/// Property to set/get credit card info
		/// </summary>
		public CreditCardInfo CreditCard {
			get {
				// Make sure we clean the input
				string type = WebUtility.InputText(listCctype.SelectedValue, 40);
				string exp = WebUtility.InputText(txtExpdate.Text, 7);
				string number = WebUtility.InputText(txtCcnumber.Text, 20);
				return new CreditCardInfo(type, number, exp);
			}
			set {
				if(value != null) {
					if(!string.IsNullOrEmpty(value.CardNumber))
						txtCcnumber.Text = value.CardNumber;
					if(!string.IsNullOrEmpty(value.CardExpiration))
						txtExpdate.Text = value.CardExpiration;
					if(!string.IsNullOrEmpty(value.CardType))
						listCctype.Items.FindByValue(value.CardType).Selected = true;	  
				}
			}
		}
	}
}
