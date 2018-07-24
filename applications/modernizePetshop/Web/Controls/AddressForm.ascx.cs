using PetShop.Model;
using System.Text.RegularExpressions;

namespace PetShop.Web {

	public partial class AddressForm : System.Web.UI.UserControl {
						
		/// <summary>
		/// Control property to set or get the address
		/// </summary>
		public AddressInfo Address {
			get {

                // Return null if control is empty
                if (string.IsNullOrEmpty(txtFirstName.Text) && 
                   string.IsNullOrEmpty(txtLastName.Text) && 
                   string.IsNullOrEmpty(txtAddress1.Text) && 
                   string.IsNullOrEmpty(txtAddress2.Text) && 
                   string.IsNullOrEmpty(txtCity.Text) && 
                   string.IsNullOrEmpty(txtZip.Text) && 
                   string.IsNullOrEmpty(txtEmail.Text) && 
                   string.IsNullOrEmpty(txtPhone.Text))
                    return null;

				// Make sure we clean the input
				string firstName = WebUtility.InputText(txtFirstName.Text, 50);
				string lastName = WebUtility.InputText(txtLastName.Text, 50);
				string address1 = WebUtility.InputText(txtAddress1.Text, 50);
				string address2 = WebUtility.InputText(txtAddress2.Text, 50);
				string city = WebUtility.InputText(txtCity.Text, 50);
				string zip = WebUtility.InputText(txtZip.Text, 10);
				string phone = WebUtility.InputText(WebUtility.CleanNonWord(txtPhone.Text), 10);
				string email = WebUtility.InputText(txtEmail.Text, 80);		  
				string state = WebUtility.InputText(listState.SelectedItem.Value, 2);
				string country = WebUtility.InputText(listCountry.SelectedItem.Value, 50);
                
				return new AddressInfo(firstName, lastName, address1, address2, city, state, zip, country, phone, email);
			}
			set {
				if(value != null) {
					if(!string.IsNullOrEmpty(value.FirstName))
						txtFirstName.Text = value.FirstName;
					if(!string.IsNullOrEmpty(value.LastName))
						txtLastName.Text = value.LastName;
					if(!string.IsNullOrEmpty(value.Address1))
						txtAddress1.Text = value.Address1;
					if(!string.IsNullOrEmpty(value.Address2))
						txtAddress2.Text = value.Address2;
					if(!string.IsNullOrEmpty(value.City))
						txtCity.Text = value.City;
					if(!string.IsNullOrEmpty(value.Zip))
						txtZip.Text = value.Zip;
					if(!string.IsNullOrEmpty(value.Phone))
						txtPhone.Text = value.Phone;
					if(!string.IsNullOrEmpty(value.Email))
						txtEmail.Text = value.Email;
					if(!string.IsNullOrEmpty(value.State)) {
						listState.ClearSelection();
						listState.SelectedValue = value.State;
					}
					if(!string.IsNullOrEmpty(value.Country)) {
						listCountry.ClearSelection();
						listCountry.SelectedValue = value.Country;
					}
				}
			} 
		}

	}
}