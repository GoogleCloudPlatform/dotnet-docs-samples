<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CheckOut.aspx.cs" Inherits="PetShop.Web.CheckOut" Title="Check Out" %>

<%@ Register Src="Controls/CartList.ascx" TagName="CartList" TagPrefix="PetShopControl" %>

<%@ Register Src="Controls/AddressConfirm.ascx" TagName="AddressConfirm" TagPrefix="PetShopControl" %>
<%@ Register Src="Controls/AddressForm.ascx" TagName="AddressForm" TagPrefix="PetShopControl" %>

<asp:Content ID="cntPage" ContentPlaceHolderID="cphPage" Runat="Server" EnableViewState="false">
    <div align="center" class="checkoutPosition">

    <script type="text/javascript" language="javascript">
        function ClientValidate(source, arguments) {
            var dtNow = new Date();
            var tmp = new String(arguments.Value);            
            var dtCard = new Date();
            dtCard.setFullYear(tmp.split("/")[1]);
            dtCard.setMonth(tmp.split("/")[0]-1);
            dtCard.setDate(30);
            if (dtNow < dtCard)
                arguments.IsValid = true;
            else
                arguments.IsValid = false;
        }
    </script>

    <table border="0" cellpadding="0" cellspacing="0" class="checkoutContent" width="100%">
        <tr>
            <td>
                <div class="label">
                    <asp:Label ID="lblMsg" runat="server" EnableViewState="false"></asp:Label>
                </div>
                <asp:Wizard ID="wzdCheckOut" runat="server" DisplaySideBar="False" OnActiveStepChanged="wzdCheckOut_ActiveStepChanged"
                    OnFinishButtonClick="wzdCheckOut_FinishButtonClick" SkinID="wzdCheckOut">
                    <WizardSteps>
                        <asp:WizardStep ID="wzdStep1" runat="server" Title="Billing Address">
                        <asp:Panel ID="panFicusStep1" runat="server" DefaultButton="StartNextButton">
                            <PetShopControl:AddressForm ID="billingForm" runat="server" />
                            <table border="0" cellpadding="0" cellspacing="0" class="checkoutButtonBg" width="100%">
                                <tr>
                                    <td></td>
                                    <td align="right"><asp:LinkButton ID="StartNextButton" runat="server" CommandName="MoveNext" CssClass="continue">Next</asp:LinkButton></td>
                                </tr>
                            </table>
                        </asp:Panel>    
                        </asp:WizardStep>
                        <asp:WizardStep ID="wzdStep2" runat="server" Title="Shipping Address">
                            <asp:Panel ID="panFicusStep2" runat="server" DefaultButton="StepNextButton">
                            <div class="checkOutLabel">
                                <asp:CheckBox ID="chkShipToBilling" runat="server" AutoPostBack="True" OnCheckedChanged="chkShipToBilling_CheckedChanged"
                                    Text="Ship to billing address" /></div>
                            <PetShopControl:AddressForm ID="shippingForm" runat="server" />
                            <table border="0" cellpadding="0" cellspacing="0" class="checkoutButtonBg" width="100%">
                                <tr>
                                    <td align="left"><asp:LinkButton ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                CssClass="back">Previous</asp:LinkButton></td><td align="right"><asp:LinkButton ID="StepNextButton" runat="server" CommandName="MoveNext" CssClass="continue">Next</asp:LinkButton></td>
                                </tr>
                            </table>
                            </asp:Panel>
                        </asp:WizardStep>
                        <asp:WizardStep ID="wzdStep3" runat="server" Title="Payment Information">
                            <asp:Panel ID="panFicusStep3" runat="server" DefaultButton="StepNextButton2">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="label">Credit Card Number<br />
                                        <asp:TextBox ID="txtCCNumber" runat="server" CssClass="checkoutTextbox" Width="330px">4444123412341234</asp:TextBox><br />
                                        <asp:RequiredFieldValidator ID="valCCNumber" runat="server" ControlToValidate="txtCCNumber"
                                            Display="Dynamic" ErrorMessage="Please enter card number."></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                                ID="valCCNumber1" runat="server" ControlToValidate="txtCCNumber"
                                                Display="Dynamic" ErrorMessage="Card number invalid." ValidationExpression="^([0-9]{15,16})$"></asp:RegularExpressionValidator>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="label">Expiration Date (MM/YYYY)<br />
                                        <asp:TextBox ID="txtExpDate" runat="server" CssClass="checkoutTextbox" Width="155px">12/2009</asp:TextBox><br />
                                        <asp:RequiredFieldValidator ID="valExpDate" runat="server" ControlToValidate="txtExpDate"
                                            Display="Dynamic" ErrorMessage="Please enter expiration date."></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                        ID="valExpDate1" runat="server" ControlToValidate="txtExpDate"
                        ErrorMessage="Invalid date format." ValidationExpression="^((0[1-9])|(1[0-2]))\/(\d{4})$" Display="Dynamic"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="valExpDate2" runat="server" ClientValidationFunction="ClientValidate"
                            ControlToValidate="txtExpDate" Display="Dynamic" ErrorMessage="Expiration date invalid."
                            OnServerValidate="ServerValidate"></asp:CustomValidator>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="label">Credit card Type<br />
                                        <asp:DropDownList ID="listCCType" runat="server" CssClass="checkoutDropdown">
                                            <asp:ListItem>Visa</asp:ListItem>
                                            <asp:ListItem>Master Card</asp:ListItem>
                                            <asp:ListItem>American Express</asp:ListItem>
                                            <asp:ListItem>Discovery</asp:ListItem>
                                        </asp:DropDownList></td><td>&nbsp;</td>
                                </tr>
                            </table>
                            <p>&nbsp;</p>
                            <table border="0" cellpadding="0" cellspacing="0" class="checkoutButtonBg" width="100%">
                                <tr>
                                    <td align="left"><asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                CssClass="back">Previous</asp:LinkButton></td><td align="right"><asp:LinkButton ID="StepNextButton2" runat="server" CommandName="MoveNext"
                                        CssClass="continue">Next</asp:LinkButton></td>
                                </tr>
                            </table>
                            </asp:Panel>
                        </asp:WizardStep>
                        <asp:WizardStep ID="wzdStep4" runat="server" StepType="Finish" Title="Confirmation">
                            <asp:Panel ID="panFicusStep4" runat="server" DefaultButton="FinishButton">
                            <div class="checkOutLabel">
                                Your order will not be processed until you select "Submit Order" below.<br /><br />
                                <strong>Billing address:<br />
                                </strong>
                                <PetShopControl:AddressConfirm ID="billingConfirm" runat="server">
                                </PetShopControl:AddressConfirm><br />
                                <strong>Shipping address:<br />
                                </strong>
                                <PetShopControl:AddressConfirm ID="shippingConfirm" runat="server">
                                </PetShopControl:AddressConfirm>
                            </div>
                            <div class="checkOutLabel">
                                A total of <span class="labelBold">
                                    <asp:Literal ID="ltlTotal" runat="server"></asp:Literal></span> will be charged
                                to your credit card, ending with <span class="labelBold">
                                    <asp:Literal ID="ltlCreditCard" runat="server"></asp:Literal></span>.
                            </div>
                            <table border="0" cellpadding="0" cellspacing="0" class="checkoutButtonBg" width="100%">
                                <tr>
                                    <td align="left"><asp:LinkButton ID="FinishPreviousButton" runat="server" CausesValidation="False"
                                CommandName="MovePrevious" CssClass="back">Previous</asp:LinkButton></td><td align="right"><asp:LinkButton ID="FinishButton" runat="server" CommandName="MoveComplete" CssClass="submit">Submit Order</asp:LinkButton></td>
                                </tr>
                            </table>
                            </asp:Panel>
                        </asp:WizardStep>
                        <asp:WizardStep ID="wzdStep5" runat="server" AllowReturn="False" StepType="Complete"
                            Title="Receipt">
                            <div class="checkOutLabel">
                                Thank you for your order!<br /><br />
                                <PetShopControl:CartList ID="CartListOrdered" runat="server" />
                                <br />
                               
                                <p>
                                    A total of <strong>
                                        <asp:Literal ID="ltlTotalComplete" runat="server"></asp:Literal></strong> is
                                    being charged to your credit card, ending with <strong>
                                        <asp:Literal ID="ltlCreditCardComplete" runat="server"></asp:Literal></strong>.</p>
                                <p>
                                    If you have any questions regarding this order, please contact our customer service
                                    at anytime.</p>
                                <p>
                                    The .NET Pet Shop Team</p>
                            </div>                           
                        </asp:WizardStep>
                    </WizardSteps>
                    <HeaderStyle HorizontalAlign="Left" />
                    <HeaderTemplate>
                        <%= wzdCheckOut.ActiveStep.Title %>
                    </HeaderTemplate>
                    <StartNavigationTemplate>
                        
                    </StartNavigationTemplate>
                    <StepNavigationTemplate>
                       
                    </StepNavigationTemplate>
                    <FinishNavigationTemplate>
                        
                    </FinishNavigationTemplate>
                </asp:Wizard>
            </td>
        </tr>
    </table>
    
    </div>
</asp:Content>

