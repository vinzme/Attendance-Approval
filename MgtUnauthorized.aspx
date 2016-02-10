<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MgtUnauthorized.aspx.vb" Inherits="MgtUnauthorized" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title>M E M O R A N D U M</title>
<script> 
if(!window.opener){ 
newWin=window.open (this.location,'newwindow','toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,directories=no,status=no') 
newWin.moveTo(0,0); 
newWin.resizeTo(screen.availWidth,screen.availHeight) 
window.opener='a'; 
window.close(); 
} 
</script> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Font-Names="Verdana" ForeColor="Red" Text="You are not authorized to view this site!!!"
            Width="472px"></asp:Label></div>
    </form>
</body>
</html>
