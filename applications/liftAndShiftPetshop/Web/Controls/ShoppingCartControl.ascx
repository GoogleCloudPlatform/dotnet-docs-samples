<%@ Control AutoEventWireup="true" CodeFile="ShoppingCartControl.ascx.cs" Inherits="PetShop.Web.ShoppingCartControl" Language="C#" %>
<asp:Panel ID="panFocus" runat="server" DefaultButton="btnTotal">

        <div align="left" class="cartHeader">Items in your Shopping Cart</div>
<asp:Label runat="server" ID="lblMsg" EnableViewState="false"  CssClass="label" />

<asp:Repeater ID="repShoppingCart" runat="server">
    <HeaderTemplate>
        <table cellspacing="0" cellpadding="3" rules="all" border="0" class="cart" align="center" width="387">
            <tr class="labelLists">
				<th scope="col">&nbsp;</th>
				<th scope="col">Name</th>
				<th scope="col">Qty</th>
				<th align="right" scope="col">Price</th>
				<th scope="col">&nbsp;</th>
			</tr>
    </HeaderTemplate>
    <ItemTemplate>
            <tr class="listItem">
				<td>
                    <asp:ImageButton ID="btnDelete" runat="server" BorderStyle="None" CausesValidation="false"
                    CommandArgument='<%# Eval("ItemId") %>' CommandName="Del" ImageUrl="~/Comm_Images/button-delete.gif"
                    OnCommand="CartItem_Command" ToolTip="Delete" />
                </td>
				<td style="width:100%;">
                    <a runat="server" href='<%# string.Format("~/Items.aspx?itemId={0}&productId={1}&categoryId={2}", Eval("ItemId"), Eval("ProductId"), Eval("CategoryId")) %>'><%# string.Format("{0} {1}", Eval("Name"), Eval("Type")) %></a>
                </td>
				<td>
				    <asp:TextBox ID="txtQuantity" runat="server" Columns="3" Text='<%# Eval("Quantity") %>' Width="20px"></asp:TextBox>
                </td>
				<td align="right"><%# Eval("Price", "{0:c}")%></td><td>
                   <asp:ImageButton ID="btnToWishList" runat="server" AlternateText="Move to wish list" CausesValidation="false" CommandArgument='<%# Eval("ItemId") %>' CommandName="Move" ImageUrl="~/Comm_Images/button-wishlist.gif" OnCommand="CartItem_Command" ToolTip="Move to wish list" />
                </td>
			</tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>  
<asp:PlaceHolder ID="plhTotal" runat="server" EnableViewState="false">
    <table border="0" cellpadding="0" cellspacing="0" width="387">
        <tr>
            <td class="dottedLineCentered" colspan="4">&nbsp;</td>
        </tr>
        <tr>
            <td class="total" width="30">Total </td><td>
                <asp:ImageButton ID="btnTotal" runat="server" AlternateText="Calculate total" CausesValidation="false"
                    ImageUrl="~/Comm_Images/button-calculate.gif" OnClick="BtnTotal_Click" EnableViewState="false" /></td>
            <td align="right" class="total">
                <asp:Literal ID="ltlTotal" runat="server" EnableViewState="false"></asp:Literal></td><td width="30">&nbsp;</td>
        </tr>
    </table>
</asp:PlaceHolder>
</asp:Panel>
