using System;
using System.Web;
using System.Web.UI.WebControls;
using PetShop.BLL;
using PetShop.CacheDependencyFactory;

namespace PetShop.Web {
    public partial class ItemsControl : System.Web.UI.UserControl {
        
        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e) {
            //reset index
            itemsGrid.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string productKey = Request.QueryString["productId"];

            //bind data            
            Item item = new Item();
            itemsGrid.DataSource = item.GetItemsByProduct(productKey);
            itemsGrid.DataBind();

        }
    }
}