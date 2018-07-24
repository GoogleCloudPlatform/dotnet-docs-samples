using System;
using System.Threading;
using System.Web.Security;

namespace PetShop.Web
{
    public partial class Default : System.Web.UI.Page {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty( Thread.CurrentPrincipal.Identity.Name))
            {
                btnAuth.Text = "Sign In";
            }
            else
            {
                btnAuth.Text = "Sign Out";
            }
        }


        /// <summary>
        /// Redirect to Search page
        /// </summary>
		protected void btnSearch_Click(object sender, EventArgs e) {
            WebUtility.SearchRedirect(txtSearch.Text);
		}

        protected void btnAuth_Click(object sender, EventArgs e)
        {
            if (btnAuth.Text == "Sign Out")
            {
                FormsAuthentication.SignOut();
                Response.Redirect("default.aspx");
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
    }
}
