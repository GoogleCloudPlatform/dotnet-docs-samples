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

namespace PetShop.Web
{
    public partial class ShoppingCartControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handle Page load event
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCart();
            }
        }

        /// <summary>
        /// Bind repeater to Cart object in Profile
        /// </summary>
        private void BindCart()
        {
            ICollection<CartItemInfo> cart = Profile.ShoppingCart.CartItems;
            if (cart.Count > 0)
            {
                repShoppingCart.DataSource = cart;
                repShoppingCart.DataBind();
                PrintTotal();
                plhTotal.Visible = true;
            }
            else
            {
                repShoppingCart.Visible = false;
                plhTotal.Visible = false;
                lblMsg.Text = "Your cart is empty.";
            }
        }

        /// <summary>
        /// Recalculate the total
        /// </summary>
        private void PrintTotal()
        {
            if (Profile.ShoppingCart.CartItems.Count > 0)
                ltlTotal.Text = Profile.ShoppingCart.Total.ToString("c");
        }

        /// <summary>
        /// Calculate total
        /// </summary>
        protected void BtnTotal_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            TextBox txtQuantity;
            ImageButton btnDelete;
            int qty = 0;
            foreach (RepeaterItem row in repShoppingCart.Items)
            {
                txtQuantity = (TextBox)row.FindControl("txtQuantity");
                btnDelete = (ImageButton)row.FindControl("btnDelete");
                if (int.TryParse(WebUtility.InputText(txtQuantity.Text, 10), out qty))
                {
                    if (qty > 0)
                        Profile.ShoppingCart.SetQuantity(btnDelete.CommandArgument, qty);
                    else if (qty == 0)
                        Profile.ShoppingCart.Remove(btnDelete.CommandArgument);
                }
            }
            Profile.Save();
            BindCart();
        }

        /// <summary>
        /// Handler for Delete/Move buttons
        /// </summary>
        protected void CartItem_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {
                case "Del":
                    Profile.ShoppingCart.Remove(e.CommandArgument.ToString());
                    break;
                case "Move":
                    Profile.ShoppingCart.Remove(e.CommandArgument.ToString());
                    Profile.WishList.Add(e.CommandArgument.ToString());
                    break;
            }
            Profile.Save();
            BindCart();
        }
    }
}
