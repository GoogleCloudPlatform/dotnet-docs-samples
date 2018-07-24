<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NavigationControl.ascx.cs" Inherits="PetShop.Web.NavigationControl" %>
<%@ OutputCache Duration="100000" VaryByParam="*" %>

<asp:Repeater ID="repCategories" runat="server">
<HeaderTemplate>
<table cellspacing="0" border="0" style="border-collapse: collapse;">
</HeaderTemplate>
<ItemTemplate>
<tr>
<td class="<%= ControlStyle %>"><asp:HyperLink runat="server" ID="lnkCategory"  NavigateUrl='<%# string.Format("~/Products.aspx?page=0&categoryId={0}", Eval("Id")) %>' Text='<%# Eval("Name") %>' /><asp:HiddenField runat="server" ID="hidCategoryId" Value='<%# Eval("Id") %>' /></td>
</tr>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:Repeater>
