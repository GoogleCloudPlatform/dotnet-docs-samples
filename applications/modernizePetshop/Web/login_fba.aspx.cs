using System;
using System.Web;
using System.Web.Security;

public partial class login_fba : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(Request.QueryString["uid"]))
        {            
            FormsAuthenticationTicket tkt;
            string cookiestr;
            HttpCookie ck;
            tkt = new FormsAuthenticationTicket(1, Request.QueryString["displayname"], DateTime.Now,
                                                DateTime.Now.AddMinutes(30), true, Request.QueryString["uid"]);
            cookiestr = FormsAuthentication.Encrypt(tkt);
            ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
            ck.Expires = tkt.Expiration;
            ck.Path = FormsAuthentication.FormsCookiePath;
            Response.Cookies.Add(ck);

            
            string strRedirect = Request["ReturnUrl"];
            if (string.IsNullOrEmpty(strRedirect))
                strRedirect = "default.aspx";
            Response.Redirect(strRedirect, true);
        }
     }
}