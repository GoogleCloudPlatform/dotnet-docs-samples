using PetShop.Model;

namespace PetShop.Web {
	public partial class AddressConfirm : System.Web.UI.UserControl {
													  
		/// <summary>
		///	Control property to set the address
		/// </summary>
		public AddressInfo Address {
			set {
				if(value != null) {					
					if(!string.IsNullOrEmpty(value.FirstName))
						ltlFirstName.Text = value.FirstName;
					if(!string.IsNullOrEmpty(value.LastName))
						ltlLastName.Text = value.LastName;
					if(!string.IsNullOrEmpty(value.Address1))
						ltlAddress1.Text = value.Address1;
					if(!string.IsNullOrEmpty(value.Address2))
						ltlAddress2.Text = value.Address2;
					if(!string.IsNullOrEmpty(value.City))
						ltlCity.Text = value.City;
					if(!string.IsNullOrEmpty(value.Zip))
						ltlZip.Text = value.Zip;	   
					if(!string.IsNullOrEmpty(value.State))
						ltlState.Text = value.State;
					if(!string.IsNullOrEmpty(value.Country))
						ltlCountry.Text = value.Country;
					if(!string.IsNullOrEmpty(value.Phone))
						ltlPhone.Text = value.Phone;
					if(!string.IsNullOrEmpty(value.Email))
						ltlEmail.Text = value.Email;
				}
			}
		}
	}
}