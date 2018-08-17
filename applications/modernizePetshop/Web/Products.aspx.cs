using System;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace PetShop.Web {
    
    public partial class Products : System.Web.UI.Page {

        protected void Page_Load(object sender, EventArgs e) {
            //get page header and title
            Page.Title = WebUtility.GetCategoryName(Request.QueryString["categoryId"]);

            if (!string.IsNullOrEmpty(Thread.CurrentPrincipal.Identity.Name))
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                string cookiePath = ticket.CookiePath;
                DateTime expiration = ticket.Expiration;
                bool expired = ticket.Expired;
                bool isPersistent = ticket.IsPersistent;
                DateTime issueDate = ticket.IssueDate;
                string name = ticket.Name;
                string userData = ticket.UserData;
                int version = ticket.Version;
            }
        }
    }
}