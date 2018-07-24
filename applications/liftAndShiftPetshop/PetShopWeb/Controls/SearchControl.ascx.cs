using System;
using System.Web;
using System.Web.UI.WebControls;
using PetShop.BLL;
using PetShop.CacheDependencyFactory;

namespace PetShop.Web {
    public partial class SearchControl : System.Web.UI.UserControl {
        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e) {
            //reset index
            searchList.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string keywordKey = Request.QueryString["keywords"];

            //bind data
            Product product = new Product();
            searchList.DataSource = product.GetProductsBySearch(keywordKey);
            searchList.DataBind();
        }

        /// <summary>
        /// Add cache dependency
        /// </summary>
        protected void Page_Load(object sender, EventArgs e) {
            this.CachePolicy.Dependency = DependencyFacade.GetItemDependency();
        }
    }
}