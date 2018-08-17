<%@ Page AutoEventWireup="true" CodeFile="Items.aspx.cs" Inherits="PetShop.Web.Items" Language="C#" MasterPageFile="~/MasterPage.master" Title="Items" %>

<%@ Register Src="Controls/ItemsControl.ascx" TagName="ItemsControl" TagPrefix="PetShopControl" %>

<asp:Content ID="cntPage" runat="Server" ContentPlaceHolderID="cphPage" EnableViewState="false">
    <PetShopControl:ItemsControl ID="ItemsControl1" runat="server" />
</asp:Content>
