<%@ Page AutoEventWireup="true" CodeFile="~/Default.aspx.cs" EnableViewState="false" Inherits="PetShop.Web.Default" Language="C#" %>
<%@ Register Src="Controls/NavigationControl.ascx" TagName="NavigationControl" TagPrefix="PetShopControl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Welcome to .NET Pet Shop</title>
</head>
<body class="homeBody">
    <form id="form1" runat="server">
        <asp:Panel ID="panFocus" runat="server" DefaultButton="btnSearch">
            <table align="center" border="0" cellpadding="0" cellspacing="0" width="780">
                <tr valign="top">
                    <td>
                        <img src="Comm_Images/Logo-home.gif" alt="home" /></td>
                    <td class="homeBgSearch" height="25" width="141">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="homeSearchBox" Width="130px"></asp:TextBox></td>
                    <td class="homeBgSearch" width="50">
                        <asp:ImageButton ID="btnSearch" runat="server" AlternateText="Search" CausesValidation="false"
                            CssClass="paddingSearchicon" ImageUrl="Comm_Images/button-search.gif" OnClick="btnSearch_Click" /></td>
                    <td class="homeBgSearch" width="50">
                        <asp:LoginStatus ID="lgnStatus" runat="server" CssClass="homeLink" LoginText="sign in"
                            LogoutAction="Redirect" LogoutPageUrl="~/Default.aspx" LogoutText="sign out" />
                    </td>
                    <td width="66">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table align="center" border="0" cellpadding="0" cellspacing="0" width="780">
            <tr>
                <td width="112">
                </td>
                <td bgcolor="#FFFFFF" width="5">
                </td>
                <td bgcolor="#FFFFFF" height="5" width="436">
                </td>
                <td bgcolor="#FFFFFF" width="5">
                </td>
                <td width="224">
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td bgcolor="#FFFFFF">
                    &nbsp;</td>
                <td class="welcome">
                    Welcome to the world of animals
                </td>
                <td bgcolor="#FFFFFF">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="5" height="1">
                </td>
            </tr>
            <tr>
                <td width="112">
                    <img height="290" src="Comm_Images/seahorse.gif" width="112" alt="Sea Horse" /></td>
                <td bgcolor="#FFFFFF" width="5">
                    &nbsp;</td>
                <td class="bgControl" valign="top" width="436">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="intro">
                                Explore our wide selection of what could be your future pet.
                            </td>
                        </tr>
                        <tr>
                            <td class="navigationLabel">
                                Our Categories are:
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 20px">
                                <PetShopControl:NavigationControl ID="Categories" runat="server"></PetShopControl:NavigationControl>
                                <p>
                                    &nbsp;</p>
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
                <td bgcolor="#FFFFFF" valign="top">
                    &nbsp;</td>
                <td class="fishPosition" valign="top">
                    <img src="Comm_Images/home-fish.gif" alt="Created by Vertigo Software, Inc." usemap="#vertigosoftware"
                        border="0" /></td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
                <td class="footerHome">
                    Version 4.0 - Powered by .NET 2.0</td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
        <map id="vertigosoftware" name="vertigosoftware">
            <area shape="rect" coords="240,235,140,300" href="http://www.vertigosoftware.com"
                target="_blank" alt="Vertigo Software" />
        </map>
    </form>
</body>
</html>
