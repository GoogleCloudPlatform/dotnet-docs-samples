using PetShop.Model;
using System;
using System.Web.UI.WebControls;

namespace PetShop.Web {
	public partial class CreditCardForm : System.Web.UI.UserControl {

		/// <summary>
		/// Custom validator to check the expiration date
		/// </summary>
		protected void ServerValidate(object source, ServerValidateEventArgs value) {
			DateTime dt;
			if(DateTime.TryParse(value.Value, out dt))
				value.IsValid = (dt > DateTime.Now);
			else
				value.IsValid = false;
		}

		/// <summary>
		/// Property to set/get credit card info
		/// </summary>
		public CreditCardInfo CreditCard {
			get {
				// Make sure we clean the input
				string type = WebUtility.InputText(listCctype.SelectedValue, 40);
				string exp = WebUtility.InputText(txtExpdate.Text, 7);
				string number = WebUtility.InputText(txtCcnumber.Text, 20);
				return new CreditCardInfo(type, number, exp);
			}
			set {
				if(value != null) {
					if(!string.IsNullOrEmpty(value.CardNumber))
						txtCcnumber.Text = value.CardNumber;
					if(!string.IsNullOrEmpty(value.CardExpiration))
						txtExpdate.Text = value.CardExpiration;
					if(!string.IsNullOrEmpty(value.CardType))
						listCctype.Items.FindByValue(value.CardType).Selected = true;	  
				}
			}
		}
	}
}