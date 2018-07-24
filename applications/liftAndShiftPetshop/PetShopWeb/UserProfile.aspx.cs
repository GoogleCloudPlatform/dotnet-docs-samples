using System;

namespace PetShop.Web {
	public partial class UserProfile : System.Web.UI.Page {	   

		/// <summary>
		/// Update profile
		/// </summary>
		protected void btnSubmit_Click(object sender, EventArgs e) {
			Profile.AccountInfo = AddressForm.Address;
			Profile.Save();
            lblMessage.Text = "Your profile information has been successfully updated.<br>&nbsp;";
		}

        /// <summary>
        /// Handle Page load event 
        /// </summary>
		protected void Page_Load(object sender, EventArgs e) {
			if(!IsPostBack)
				BindUser();
		}

		/// <summary>
		/// Bind controls to profile
		/// </summary>
		private void BindUser() {
			AddressForm.Address = Profile.AccountInfo;
		}
}
}