<%@ Control AutoEventWireup="true" CodeFile="CreditCardForm.ascx.cs" Inherits="PetShop.Web.CreditCardForm"
    Language="C#" %>
    

    <script language="javascript">
        function ClientValidate(source, arguments) {
            var dtNow = new Date();
            var tmp = new String(arguments.Value);            
            var dtCard = new Date();
            dtCard.setFullYear(tmp.split("/")[1]);
            dtCard.setMonth(tmp.split("/")[0]-1);
            dtCard.setDate(30);
            if (dtNow < dtCard)
                arguments.IsValid = true;
            else
                arguments.IsValid = false;
        }
    </script>

<table border="0" cellpadding="0" cellspacing="0" class="track3" width="413">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>
            <div class="label">
                Credit Card Type<br />
                <asp:DropDownList ID="listCctype" runat="server"  Width="150px" CssClass="checkoutDropdown">
                    <asp:ListItem>Visa</asp:ListItem>
                    <asp:ListItem>Master Card</asp:ListItem>
                    <asp:ListItem>American Express</asp:ListItem>
                    <asp:ListItem>Discovery</asp:ListItem>
                </asp:DropDownList><br><br></div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="label">
                Card Number
                <br />
                <asp:TextBox ID="txtCcnumber" runat="server" Width="145px" CssClass="checkoutTextbox"></asp:TextBox>
                <span class="asterisk">*</span><br />
                <asp:RequiredFieldValidator ID="valRequiredFieldValidator1" runat="server" ControlToValidate="txtCcnumber"
                    ErrorMessage="Please enter card number." Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                        ID="valRegularExpressionValidator1" runat="server" ControlToValidate="txtCcnumber"
                        ErrorMessage="Card number invalid." ValidationExpression="^([0-9]{15,16})$" Display="Dynamic"></asp:RegularExpressionValidator>&nbsp;
                <br>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="label">
                Expiration Date (MM/YYYY)
                <br />
                <asp:TextBox ID="txtExpdate" runat="server" Width="70px" CssClass="checkoutTextbox"></asp:TextBox>
                <span class="asterisk">*</span><br />
                <asp:RequiredFieldValidator ID="valRequiredFieldValidator2" runat="server" ControlToValidate="txtExpdate"
                    ErrorMessage="Please enter expiration date." Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                        ID="valRegularExpressionValidator2" runat="server" ControlToValidate="txtExpdate"
                        ErrorMessage="Invalid date format." ValidationExpression="^((0[1-9])|(1[0-2]))\/(\d{4})$" Display="Dynamic"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="valCustomValidator" runat="server" ClientValidationFunction="ClientValidate"
                            ControlToValidate="txtExpdate" Display="Dynamic" ErrorMessage="Expiration date invalid."
                            OnServerValidate="ServerValidate"></asp:CustomValidator>&nbsp;
                <br>
            </div>
        </td>
    </tr>    
</table>  