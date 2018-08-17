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
using System.Web.UI.WebControls;
using PetShop.BLL;
using PetShop.Model;
using System.Collections.Generic;

namespace PetShop.Web {
	public partial class WishListControl : System.Web.UI.UserControl {

        /// <summary>
        /// Handle Page load event
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e) {
            if (!IsPostBack) {
                BindCart();
            }
        }

        /// <summary>
        /// Bind repeater to Cart object in Profile
        /// </summary>
        private void BindCart() {
            ICollection<CartItemInfo> wishList = Profile.WishList.CartItems;
            if (wishList.Count > 0) {
                repWishList.DataSource = wishList;
                repWishList.DataBind();
            }
            else {
                repWishList.Visible = false;
                lblMsg.Text = "Your wish list is empty.";
            }

        }

		/// <summary>
		/// Handler for Delete/Move buttons
		/// </summary>
		protected void CartItem_Command(object sender, CommandEventArgs e) {
			switch(e.CommandName.ToString()) {
				case "Del":
					Profile.WishList.Remove(e.CommandArgument.ToString());
					break;
				case "Move":
					Profile.WishList.Remove(e.CommandArgument.ToString());
                    Profile.ShoppingCart.Add(e.CommandArgument.ToString());
					break;
			}
			Profile.Save();
            BindCart();
		}
	}
}
