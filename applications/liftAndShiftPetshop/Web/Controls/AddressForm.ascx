<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressForm.ascx.cs" Inherits="PetShop.Web.AddressForm" %>
<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td class="label" valign="top" width="50%">First Name<br />
            <asp:TextBox ID="txtFirstName" runat="server" Columns="30" CssClass="checkoutTextbox"
                MaxLength="80" Width="155px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtFirstName"
                CssClass="asterisk" ErrorMessage="Please enter first name."></asp:RequiredFieldValidator>&nbsp;&nbsp;
        </td><td class="label" colspan="2" valign="top">Last Name<br />
            <asp:TextBox ID="txtLastName" runat="server" Columns="30" CssClass="checkoutTextbox"
                MaxLength="80" Width="155px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="txtLastName"
                CssClass="asterisk" ErrorMessage="Please enter last name."></asp:RequiredFieldValidator>&nbsp;&nbsp;
        </td>
    </tr>
    <tr>
        <td class="label" colspan="3" valign="top">Address<br />
            <asp:TextBox ID="txtAddress1" runat="server" Columns="55" CssClass="checkoutTextbox"
                MaxLength="80" Width="330px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valAddress1" runat="server" ControlToValidate="txtAddress1"
                CssClass="asterisk" ErrorMessage="Please enter street address."></asp:RequiredFieldValidator>&nbsp;
            <br />
            <asp:TextBox ID="txtAddress2" runat="server" Columns="55" CssClass="checkoutTextbox"
                MaxLength="80" Width="330px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="label" style="height: 19px" valign="top" width="50%">City<br />
            <asp:TextBox ID="txtCity" runat="server" Columns="55" CssClass="checkoutTextbox"
                MaxLength="80" Width="155px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valCity" runat="server" ControlToValidate="txtCity"
                CssClass="asterisk" ErrorMessage="Please enter city."></asp:RequiredFieldValidator>&nbsp;&nbsp;
        </td><td class="label" style="height: 19px" valign="top" width="20%">State<br />
            <asp:DropDownList ID="listState" runat="server" CssClass="checkoutDropdown">
                <asp:ListItem Value="CA">CA</asp:ListItem>
                <asp:ListItem Value="NY">NY</asp:ListItem>
                <asp:ListItem Value="TX">TX</asp:ListItem>
            </asp:DropDownList><br />
        </td><td class="label" style="width: 100px; height: 19px;" valign="top">Postal Code<br />
            <asp:TextBox ID="txtZip" runat="server" Columns="12" CssClass="checkoutTextbox" MaxLength="20"
                Width="65px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valZip" runat="server" ControlToValidate="txtZip"
                CssClass="asterisk" Display="Dynamic" ErrorMessage="Please enter postal code."></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                    ID="valZip1" runat="server" ControlToValidate="txtZip" CssClass="asterisk" Display="Dynamic"
                    ErrorMessage="Postal code invalid." ValidationExpression="\d{5}(-\d{4})?"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class="label" style="height: 19px" valign="top" width="50%">Country<br />
            <asp:DropDownList ID="listCountry" runat="server" CssClass="checkoutDropdown">
                <asp:ListItem Value="USA">USA</asp:ListItem>
                <asp:ListItem Value="Canada">Canada</asp:ListItem>
                <asp:ListItem Value="Japan">Japan</asp:ListItem>
            </asp:DropDownList><br />
            &nbsp; </td><td class="label" style="height: 19px" valign="top" width="20%"></td>
        <td class="label" style="width: 100px; height: 19px;" valign="top"></td>
    </tr>
    <tr>
        <td class="label" colspan="3" valign="top">Phone Number<br />
            <asp:TextBox ID="txtPhone" runat="server" Columns="20" CssClass="checkoutTextbox"
                MaxLength="20" Width="155px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valPhone" runat="server" ControlToValidate="txtPhone"
                CssClass="asterisk" ErrorMessage="Please enter telephone number."></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                    ID="valPhone1" runat="server" ControlToValidate="txtPhone" CssClass="asterisk"
                    Display="Dynamic" ErrorMessage="Phone number invalid." ValidationExpression="((\(?\d{3}\)? ?)|(\d{3}-?))?\d{3}-?\d{4}"></asp:RegularExpressionValidator>&nbsp;&nbsp;</td>
    </tr>
    <tr>
        <td class="label" colspan="3" style="height: 62px" valign="top">Email<br />
            <asp:TextBox ID="txtEmail" runat="server" Columns="55" CssClass="checkoutTextbox"
                MaxLength="80" Width="330px"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail"
                CssClass="asterisk" Display="Dynamic" ErrorMessage="Please enter email."></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                    ID="valEmail1" runat="server" ControlToValidate="txtEmail" CssClass="asterisk"
                    Display="Dynamic" ErrorMessage="Email invalid." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>&nbsp;&nbsp;
        </td>
    </tr>
</table>
