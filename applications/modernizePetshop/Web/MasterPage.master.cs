using System;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace PetShop.Web {

    public partial class MasterPage : System.Web.UI.MasterPage {

        private const string HEADER_PREFIX = ".NET Pet Shop :: {0}";

        /// <summary>
        /// Create page header on Page PreRender event
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e) {	
		    ltlHeader.Text = Page.Header.Title;
            Page.Header.Title = string.Format(HEADER_PREFIX, Page.Header.Title);


            if (Page.ToString().ToLower().Contains("login_aspx"))
            {
                btnAuth.Visible = false;
                return;
            }
            else
                btnAuth.Visible = true;

            if (string.IsNullOrEmpty(Thread.CurrentPrincipal.Identity.Name))
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