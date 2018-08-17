<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WishListControl.ascx.cs" Inherits="PetShop.Web.WishListControl"%>
<div align="left" class="cartHeader">Items in your Wish List</div>
<asp:Label runat="server" ID="lblMsg" EnableViewState="false"  CssClass="label" />

<asp:Repeater ID="repWishList" runat="server">
    <HeaderTemplate>
        <table cellspacing="0" cellpadding="3" rules="all" border="0" class="cart" align="center" width="387">
            <tr class="labelLists">
				<th scope="col">&nbsp;</th>
				<th scope="col">Name</th>
				<th align="right" scope="col">Price</th>
				<th scope="col">&nbsp;</th>
			</tr>
    </HeaderTemplate>
    <ItemTemplate>
            <tr class="listItem">
				<td>
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete" CausesValidation="false"
                    CommandArgument='<%# Eval("ItemId") %>' CommandName="Del" ImageUrl="~/Comm_Images/button-delete.gif"
                    OnCommand="CartItem_Command" ToolTip="Delete" />
                </td>
				<td style="width:100%;">
                    <a runat="server" href='<%# string.Format("~/Items.aspx?itemId={0}&productId={1}&categoryId={2}", Eval("ItemId"), Eval("ProductId"), Eval("CategoryId")) %>'><%# string.Format("{0} {1}", Eval("Name"), Eval("Type")) %></a>
                </td>
				<td align="right"><%# Eval("Price", "{0:c}")%></td><td>
                   <asp:ImageButton ID="btnToWishList" runat="server" AlternateText="Move to cart" CausesValidation="false"
                    CommandArgument='<%# Eval("ItemId") %>' CommandName="Move" ImageUrl="~/Comm_Images/button-cart.gif"
                    OnCommand="CartItem_Command" ToolTip="Move to cart" />
                </td>
			</tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater> 
