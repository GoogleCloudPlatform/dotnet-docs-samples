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
using System.Web;
using System.Web.UI.WebControls;
using PetShop.BLL;
using PetShop.CacheDependencyFactory;

namespace PetShop.Web {

    public partial class ProductsControl : System.Web.UI.UserControl {

        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e) {
            //reset index
            productsList.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string categoryKey = Request.QueryString["categoryId"];

            //bind data
            Product product = new Product();
            productsList.DataSource = product.GetProductsByCategory(categoryKey);
            productsList.DataBind();

        }

        /// <summary>
        /// Add cache dependency
        /// </summary>
        protected void Page_Load(object sender, EventArgs e) {
            //this.CachePolicy.Dependency = DependencyFacade.GetProductDependency();
        }
    }
}
