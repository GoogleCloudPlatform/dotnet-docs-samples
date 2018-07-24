using System;
using System.Web.UI.WebControls;
using PetShop.Model;
using PetShop.BLL;

namespace PetShop.Web {

    public partial class CheckOut : System.Web.UI.Page {

        protected void Page_Load(object sender, EventArgs e) {
            if (billingForm.Address == null) {
                billingForm.Address = Profile.AccountInfo;   
            }
        }

        /// <summary>
        /// Process the order
        /// </summary>
        protected void wzdCheckOut_FinishButtonClick(object sender, WizardNavigationEventArgs e) {
            if (Profile.ShoppingCart.CartItems.Count > 0) {
                if (Profile.ShoppingCart.Count > 0) {

                    // display ordered items
                    CartListOrdered.Bind(Profile.ShoppingCart.CartItems);

                    // display total and credit card information
                    ltlTotalComplete.Text = ltlTotal.Text;
                    ltlCreditCardComplete.Text = ltlCreditCard.Text;

                    // create order
                    OrderInfo order = new OrderInfo(int.MinValue, DateTime.Now, User.Identity.Name, GetCreditCardInfo(), billingForm.Address, shippingForm.Address, Profile.ShoppingCart.Total, Profile.ShoppingCart.GetOrderLineItems(), null);

                    // insert
                    Order newOrder = new Order();
                    newOrder.Insert(order);

                    // destroy cart
                    Profile.ShoppingCart.Clear();
                    Profile.Save();
                }
            }
            else {
                lblMsg.Text = "<p><br>Can not process the order. Your cart is empty.</p><p class=SignUpLabel><a class=linkNewUser href=Default.aspx>Continue shopping</a></p>";
                wzdCheckOut.Visible = false;
            }
        }

        /// <summary>
        /// Create CreditCardInfo object from user input
        /// </summary>
        private CreditCardInfo GetCreditCardInfo() {
            string type = WebUtility.InputText(listCCType.SelectedValue, 40);
            string exp = WebUtility.InputText(txtExpDate.Text, 7);
            string number = WebUtility.InputText(txtCCNumber.Text, 20);
            return new CreditCardInfo(type, number, exp);
        }

        /// <summary>
        /// Changing Wiszard steps
        /// </summary>
        protected void wzdCheckOut_ActiveStepChanged(object sender, EventArgs e) {
            if (wzdCheckOut.ActiveStepIndex == 3) {
                billingConfirm.Address = billingForm.Address;
                shippingConfirm.Address = shippingForm.Address;
                ltlTotal.Text = Profile.ShoppingCart.Total.ToString("c");
                if (txtCCNumber.Text.Length > 4)
                    ltlCreditCard.Text = txtCCNumber.Text.Substring(txtCCNumber.Text.Length - 4, 4);
            }
        }

        /// <summary>
        /// Handler for "Ship to Billing Addredd" checkbox.
        /// Prefill/Clear shipping address form.
        /// </summary>
        protected void chkShipToBilling_CheckedChanged(object sender, EventArgs e) {
            if (chkShipToBilling.Checked)
                shippingForm.Address = billingForm.Address;
        }

        /// <summary>
        /// Custom validator to check CC expiration date
        /// </summary>
        protected void ServerValidate(object source, ServerValidateEventArgs value) {
            DateTime dt;
            if (DateTime.TryParse(value.Value, out dt))
                value.IsValid = (dt > DateTime.Now);
            else
                value.IsValid = false;
        }

    }
}