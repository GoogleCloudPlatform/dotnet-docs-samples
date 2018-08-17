<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchControl.ascx.cs" Inherits="PetShop.Web.SearchControl" %>
<%@ OutputCache Duration="100000" VaryByParam="page;keywords" Shared="true" %>
<%@ Register TagPrefix="PetShopControl" Namespace="PetShop.Web" %>
<div align="center" class="searchPosition">
    <div class="label">Search results for <b><%= Request.QueryString["keywords"] %></b>:</div>
    <PetShopControl:CustomList ID="searchList" runat="server" EmptyText="No products found." OnPageIndexChanged="PageChanged" PageSize="4" RepeatColumns="2" CellPadding="16" CellSpacing="0" Width="500px">        
        <ItemTemplate>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td valign="top" width="91"><a href='Items.aspx?productId=<%# Eval("Id") %>&categoryId=<%# Eval("categoryId") %>'><img id="imgProduct" alt='<%# Eval("Name") %>' src='<%# Eval("Image") %>' style="border-width: 0px;" runat="server" /></a></td>
                    <td width="26">&nbsp;</td>
                    <td valign="top" width="120"><a href='Items.aspx?productId=<%# Eval("Id") %>&categoryId=<%# Eval("categoryId") %>'><div class="productName"><%# Eval("Name") %></div></a><div class="productDescription"><%# Eval("Description") %></div></td>                    
                </tr>               
            </table>            
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />        
    </PetShopControl:CustomList>
 </div>      
