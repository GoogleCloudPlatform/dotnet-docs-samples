using System;

public partial class WishList : System.Web.UI.Page {

    protected void Page_PreInit(object sender, EventArgs e) {
        if (!IsPostBack) {
            string itemId = Request.QueryString["addItem"];
            if (!string.IsNullOrEmpty(itemId)) {
                Profile.WishList.Add(itemId);
                Profile.Save();
                // Redirect to prevent duplictations in the wish list if user hits "Refresh"
                Response.Redirect("~/WishList.aspx", true);
            }
        }
    }
}
