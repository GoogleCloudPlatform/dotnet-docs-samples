using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using PetShop.Model;
using System.Collections.Generic;

namespace PetShop.Web {
    public partial class CartList : System.Web.UI.UserControl {

        /// <summary>
        /// Bind control
        /// </summary>
        public void Bind(ICollection<CartItemInfo> cart) {
            if (cart != null) {
                repOrdered.DataSource = cart;
                repOrdered.DataBind();               
            }

        }
    }
}