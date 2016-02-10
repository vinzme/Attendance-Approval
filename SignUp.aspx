<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SignUp.aspx.vb" Inherits="SignUp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" BackColor="#E3EAEB" 
            BorderColor="#E6E2D8" BorderStyle="Solid" BorderWidth="1px" 
            Font-Names="Verdana" Font-Size="0.8em">
            <SideBarStyle BackColor="#1C5E55" Font-Size="0.9em" VerticalAlign="Top" />
            <SideBarButtonStyle ForeColor="White" />
            <ContinueButtonStyle BackColor="White" BorderColor="#C5BBAF" 
                BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
                ForeColor="#1C5E55" />
            <NavigationButtonStyle BackColor="White" BorderColor="#C5BBAF" 
                BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
                ForeColor="#1C5E55" />
            <HeaderStyle BackColor="#666666" BorderColor="#E6E2D8" BorderStyle="Solid" 
                BorderWidth="2px" Font-Bold="True" Font-Size="0.9em" ForeColor="White" 
                HorizontalAlign="Center" />
            <CreateUserButtonStyle BackColor="White" BorderColor="#C5BBAF" 
                BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
                ForeColor="#1C5E55" />
            <TitleTextStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <StepStyle BorderWidth="0px" />
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server" />
                <asp:CompleteWizardStep runat="server" />
            </WizardSteps>
        </asp:CreateUserWizard>
    
    </div>
    </form>
</body>
</html>
