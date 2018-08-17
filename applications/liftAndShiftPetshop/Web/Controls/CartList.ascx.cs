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
