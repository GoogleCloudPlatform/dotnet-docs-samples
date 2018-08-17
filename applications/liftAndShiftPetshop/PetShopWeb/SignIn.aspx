<%@ Page AutoEventWireup="true" Language="C#" MasterPageFile="~/MasterPage.master"
    Title="Sign In" %>
<asp:Content ID="cntPage" runat="Server" ContentPlaceHolderID="cphPage">
<div align="center" class="signinPosition">
    <asp:Login ID="Login" runat="server" CreateUserUrl="~/NewUser.aspx" SkinID="Login" FailureText="Login failed. Please try again.">
        <LayoutTemplate>
            <table border="0" align="center" width="387">
                <tr>
                    <td>
                    <asp:Panel ID="panFocus" runat="server" DefaultButton="LoginButton">
                        <table border="0" cellpadding="0">
                            <tr>
                                <td align="left" class="signinHeader" colspan="2">Please
                                    Sign In</td>
                            </tr>
                            <tr>
                                <td align="left" class="signinLabel">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label></td>
                                <td>
                                    <asp:TextBox ID="UserName" runat="server" CssClass="signinTextbox" Width="155px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="signinLabel">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label></td>
                                <td>
                                    <asp:TextBox ID="Password" runat="server" CssClass="signinTextbox" TextMode="Password"
                                        Width="155px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2" style="color: red">
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" CssClass="signinButton"
                                        ForeColor="White" Text="Sign In" ValidationGroup="Login" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2" style="height: 50px">
                                    <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me next time." />
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="linkNewUser" colspan="2">
                                    <asp:HyperLink ID="CreateUserLink" runat="server" NavigateUrl="~/NewUser.aspx">Not registered yet?</asp:HyperLink>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
    </asp:Login>
    </div>
</asp:Content>

