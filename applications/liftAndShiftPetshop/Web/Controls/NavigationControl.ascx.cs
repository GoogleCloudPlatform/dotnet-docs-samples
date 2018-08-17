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
using PetShop.CacheDependencyFactory;

namespace PetShop.Web {
    public partial class NavigationControl : System.Web.UI.UserControl {
               
        private string controlStyle;

        // control layout property
        protected string ControlStyle {
            get { return controlStyle; }
        }
	
        // Get properties based on control consumer
        protected void GetControlStyle() {
            if (Request.ServerVariables["SCRIPT_NAME"].ToLower().IndexOf("default.aspx") > 0)
                controlStyle = "navigationLinks";
            else
                controlStyle = "mainNavigation";
        }
       

        protected void Page_Load(object sender, EventArgs e) {
            GetControlStyle();
            BindCategories();

            // Select current category
            string categoryId = Request.QueryString["categoryId"];
            if (!string.IsNullOrEmpty(categoryId))
                SelectCategory(categoryId);

            // Add cache dependency
            //this.CachePolicy.Dependency = DependencyFacade.GetCategoryDependency();
        }

        // Select current category.
        private void SelectCategory(string categoryId) {
            foreach (RepeaterItem item in repCategories.Items) {
                HiddenField hidCategoryId = (HiddenField)item.FindControl("hidCategoryId");
                if(hidCategoryId.Value.ToLower() == categoryId.ToLower()) {
                    HyperLink lnkCategory = (HyperLink)item.FindControl("lnkCategory");
                    lnkCategory.ForeColor = System.Drawing.Color.FromArgb(199, 116, 3);
                    break;
                }
            }
        }

        // Bind categories
        private void BindCategories() {
            Category category = new Category();
            repCategories.DataSource = category.GetCategories();
            repCategories.DataBind();            
        }
    }
}
