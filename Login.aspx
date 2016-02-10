<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Login ID="Login1" runat="server" BackColor="#E3EAEB" BorderColor="#E6E2D8" 
            BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" 
            CreateUserText="Register as a new user" CreateUserUrl="~/SignUp.aspx" 
            Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" 
            TextLayout="TextOnTop">
            <TextBoxStyle Font-Size="0.8em" />
            <LoginButtonStyle BackColor="White" BorderColor="#C5BBAF" BorderStyle="Solid" 
                BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#1C5E55" />
            <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
            <TitleTextStyle BackColor="#1C5E55" Font-Bold="True" Font-Size="0.9em" 
                ForeColor="White" />
        </asp:Login>
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
    
    </div>
    </form>
</body>
</html>
