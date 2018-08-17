<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CartList.ascx.cs" Inherits="PetShop.Web.CartList" %>
<asp:Repeater ID="repOrdered" runat="server">
    <HeaderTemplate><table cellspacing="0" cellpadding="0" border="0" style="width:100%;border-collapse:collapse;">
        <tr align="left">
            <th scope="col">Name</th><th scope="col">Qty</th>
        </tr></HeaderTemplate>
    <ItemTemplate><tr valign="top">
            <td><%# Eval("Name") + " " + Eval("Type")%></td>
            <td><%# Eval("Quantity") %></td>
        </tr></ItemTemplate>
    <FooterTemplate></table></FooterTemplate>
</asp:Repeater>  