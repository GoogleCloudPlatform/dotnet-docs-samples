<%@ Page AutoEventWireup="true" Language="C#" MasterPageFile="~/MasterPage.master" Title="New User" %>
<asp:Content ID="cntPage" ContentPlaceHolderID="cphPage" Runat="Server" EnableViewState="false">
<div align="center" class="signinPosition">
    <asp:CreateUserWizard ID="CreateUserWizard" runat="server" CreateUserButtonText="Sign Up"
        InvalidPasswordErrorMessage="Please enter a more secure password." PasswordRegularExpressionErrorMessage="Please enter a more secure password."
        RequireEmail="False" SkinID="NewUser">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                <CustomNavigationTemplate>
                   
                </CustomNavigationTemplate>
                <ContentTemplate>
                <asp:Panel ID="panFocus" runat="server" DefaultButton="StepNextButton">
                    <table border="0" width="387">
                        <tr>
                            <td align="left" class="signinHeader" colspan="2" >
                                Sign Up for Your New Account</td>
                        </tr>
                        <tr>
                            <td align="left">
                                <label for="UserName">
                                    User Name:</label></td><td>
                                        <asp:TextBox ID="UserName" runat="server" CssClass="signinTextbox" Width="155px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                            ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="CreateUserWizard">*</asp:RequiredFieldValidator>
                                    </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <label for="Password">
                                    Password:</label></td><td>
                                        <asp:TextBox ID="Password" runat="server" CssClass="signinTextbox" TextMode="Password"
                                            Width="155px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                            ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard">*</asp:RequiredFieldValidator>
                                    </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <label for="ConfirmPassword">
                                    Confirm Password:</label></td><td>
                                        <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="signinTextbox" TextMode="Password"
                                            Width="155px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                                            ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                            ValidationGroup="CreateUserWizard">*</asp:RequiredFieldValidator>
                                    </td>
                        </tr>

                        <tr>
                            <td align="center" colspan="2">
                                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                    ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                    ValidationGroup="CreateUserWizard"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2" style="color: red">
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False" />
                            </td>
                        </tr>
                    </table>
                    <table border="0" cellspacing="5" width="387">
                        <tr align="right">
                            <td align="right">
                                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" CssClass="signinButton"
                                    Text="Sign Up" ValidationGroup="CreateUserWizard" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </asp:Panel>
                </ContentTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                <ContentTemplate>
                    <p class="signinLabel">
                        <br />
                        <b>Thank you for signing up.</b></p>
                    <p class="signinLabel">
                        Your account has been created. Now you can:</p>
                    </p>
                    <p class="signinLabel">
                        <a class="signinNewUser" href="Default.aspx">Continue shopping</a>
                    </p>
                    <p class="signinLabel">
                        <a class="signinNewUser" href="CheckOut.aspx">Check out</a>
                    </p>
                    <p class="signinLabel">
                        <a class="signinNewUser" href="UserProfile.aspx">Update your profile</a>
                    </p>
                    <p>
                        &nbsp;</p>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    </div>
</asp:Content>

