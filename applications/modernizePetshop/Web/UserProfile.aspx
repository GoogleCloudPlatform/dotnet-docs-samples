<%@ Page AutoEventWireup="true" CodeFile="UserProfile.aspx.cs" Inherits="PetShop.Web.UserProfile"
    Language="C#" MasterPageFile="~/MasterPage.master" Title="User Profile" %>

<%@ Register Src="Controls/AddressForm.ascx" TagName="AddressForm" TagPrefix="PetShopControl" %>
<asp:Content ID="cntPage" ContentPlaceHolderID="cphPage" Runat="Server" EnableViewState="false">
<div align="center" class="profilePosition">
    <table border="0" cellpadding="0" cellspacing="0" class="formContent" width="380">
        <tr>
            <td>
                <div class="checkoutHeaders" align="left">
                    Billing Information</div>
                <div class="info">
                    User Name:
                    <asp:LoginName ID="LoginName" runat="server" />
                    <br />
                </div>
                <asp:Panel ID="panFocus" runat="server" DefaultButton="btnSubmit">
                <PetShopControl:AddressForm ID="AddressForm" runat="server" />
                <asp:Label ID="lblMessage" runat="server" cssclass="label"></asp:Label>
                <div align="right" class="checkoutButtonBg">
                    <asp:LinkButton ID="btnSubmit" runat="server" CssClass="submit" OnClick="btnSubmit_Click"
                        Text="Update">
                    </asp:LinkButton></div>
                </asp:Panel>
            </td>
        </tr>
    </table>
</div>
    </asp:Content>

