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