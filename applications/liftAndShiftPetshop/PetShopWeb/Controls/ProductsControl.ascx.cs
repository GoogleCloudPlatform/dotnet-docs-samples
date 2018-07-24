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
            this.CachePolicy.Dependency = DependencyFacade.GetProductDependency();
        }
    }
}