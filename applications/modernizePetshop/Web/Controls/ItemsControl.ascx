<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ItemsControl.ascx.cs" Inherits="PetShop.Web.ItemsControl" %>
<%@ Register TagPrefix="PetShopControl" Namespace="PetShop.Web" %>

<div class="paging"><a href='Products.aspx?categoryId=<%=Request.QueryString["categoryId"] %>&productId=<%=Request.QueryString["productId"] %>'>&#060;&nbsp;Back to list</a></div>
<div class="itemsPosition" align="center">
<PetShopControl:CustomGrid ID="itemsGrid" runat="server" EmptyText="No items found." OnPageIndexChanged="PageChanged" PageSize="2">
        <HeaderTemplate>
            <table cellspacing="0" cellpadding="0" border="0" width="387">
        </HeaderTemplate>
        <ItemTemplate>
            <tr align="left" valign="top">
                <td valign="top" width="148">
                    <img id="imgItem" alt='<%# Eval("Name") %>' src='<%# Eval("Image") %>' style="border-width: 0px;" runat="server" /></td>
                <td width="33">&nbsp;</td>
                <td valign="top" width="206">
                <table cellspacing="0" cellpadding="0" border="0">
		            <tr>
			            <td class="itemText">Name:</td>
			            <td class="itemName"><%# string.Format("{0} {1}", Eval("ProductName"), Eval("Name")) %></td>
		            </tr>
		            <tr class="itemText">
			            <td>Quantity:</td>
			            <td><%# Eval("Quantity") %></td>
		            </tr>
		            <tr class="itemText">
			            <td>Price:</td>
			            <td><%# Eval("Price", "{0:c}") %></td>
		            </tr>
		            <tr class="itemText">
			            <td colspan="2"><asp:HyperLink ID="lnkCart" runat="server" NavigateUrl='<%# string.Format("~/ShoppingCart.aspx?addItem={0}", Eval("Id")) %>'  SkinID="lnkCart"></asp:HyperLink></td>
		            </tr>
		            <tr class="itemText">
			            <td colspan="2"><asp:HyperLink ID="lnkWishList" runat="server" NavigateUrl='<%# string.Format("~/WishList.aspx?addItem={0}", Eval("Id")) %>'  SkinID="lnkWishlist"></asp:HyperLink></td>
		            </tr>
	            </table>            
	            </td>
            </tr>            
        </ItemTemplate>
        <SeparatorTemplate>
            <tr>
                <td height="50" colspan="3">&nbsp;</td>
            </tr>
        </SeparatorTemplate>
        <FooterTemplate>
            </table></FooterTemplate>
    </PetShopControl:CustomGrid>
</div>        
