<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Late Attendance Report</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
    <script language=javascript type="text/javascript">
    if(!window.opener){ 
        newWin=window.open (this.location,'newbintana','toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,fullscreen=no,directories=no,status=no') 
        newWin.moveTo(0,0); 
        newWin.resizeTo(screen.availWidth,screen.availHeight) 
        closeWindow()
    }     
    function ViewPrint() {
        window.print()
    }    
    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            if (row == 'alt')
            {
                img.src = "minus.gif";
            }
            else
            {
                img.src = "minus.gif";
            }
            img.alt = "Close to view other Employees";
        }
        else
        {
            div.style.display = "none";
            if (row == 'alt')
            {
                img.src = "plus.gif";
            }
            else
            {
                img.src = "plus.gif";
            }
            img.alt = "Expand to show Details";
        }
    }
    function closeWindow() {

     //var browserName = navigator.appName;

     //var browserVer = parseInt(navigator.appVersion);

     var ie7 = (document.all && !window.opera && window.XMLHttpRequest) ? true : false;  

     if (ie7) 

           {     

           //This method is required to close a window without any prompt for IE7

           window.open('','_self','');

           window.close();

           }

     else 

           {

           //This method is required to close a window without any prompt for IE6

        window.opener='a'; 
        window.close();     

           }

     }     
    </script>
</head>
<body style="background-color: aliceblue">

    <form id="form1" runat="server">
    <div class="header">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td style="width: 100px; vertical-align: middle;">
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/attendance.jpg" CausesValidation="False" /></td>
                <td style="width: 100%; text-align: right;">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/ses_logo.png" />
                    </td>
            </tr>
        </table>
    </div>        
    <div>
        <asp:GridView ID="GridView1" runat="server" BackColor="#F1F1F1" 
            AutoGenerateColumns=False DataSourceID="ObjectDataSource1" DataKeyNames="Employee_Number"
            style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 124px" ShowFooter=True Font-Size=Small
            Font-Names="Verdana" GridLines=None OnRowDataBound="GridView1_RowDataBound" 
            BorderStyle=Outset
            AllowSorting=True Width="100%">
            <RowStyle BackColor="Gainsboro" />
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#0083C1" ForeColor="White" BorderColor="#FFC0C0"/>
            <FooterStyle BackColor="White" /> 
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <a href="javascript:expandcollapse('div<%# Eval("Employee_Number") %>', 'one');">
                            <img id="imgdiv<%# Eval("Employee_Number") %>" alt="Click to show/hide Employee Details-<%# Eval("Employee_Number") %>"  width="9px" border="0" src="plus.gif"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Employee No." SortExpression="Employee_Number">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Employee_Number") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Employee_Number") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:BoundField DataField="EmployeeName" HeaderText="Name" SortExpression="EmployeeName" />
                <asp:BoundField DataField="Count_30" HeaderText="11 to 30 mins" SortExpression="Count_30">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Count_60" HeaderText="31 to 60 mins" SortExpression="Count_60">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="CountLate" HeaderText="&gt; 60 mins" ReadOnly="True" SortExpression="CountLate" >
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Count_Deduct" HeaderText="Total" SortExpression="Count_Deduct">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Deduct" SortExpression="Deduct">
                    <ItemTemplate>
                    <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"
                            Width="136px" AutoPostBack="True" Enabled="true" OnSelectedIndexChanged="rbl_SelectedIndexChanged">
                            <asp:ListItem Selected="True" Value="0">Yes</asp:ListItem>
                            <asp:ListItem Value="1">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                
			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">
                                <div id="div<%# Eval("Employee_Number") %>" style="display:none;position:relative;left:15px;OVERFLOW: auto;WIDTH:97%" >
                                
                                    <asp:GridView ID="GridView2" AllowSorting="True" BackColor="White" Width=100% Font-Size=X-Small
                                        AutoGenerateColumns=False Font-Names="Verdana" runat="server" DataKeyNames="Employee_Number" ShowFooter=True
                                        OnPageIndexChanging="GridView2_PageIndexChanging" OnRowUpdating = "GridView2_RowUpdating"
                                        OnRowCommand = "GridView2_RowCommand" OnRowEditing = "GridView2_RowEditing" GridLines=None
                                        OnRowUpdated = "GridView2_RowUpdated" OnRowCancelingEdit = "GridView2_CancelingEdit" OnRowDataBound = "GridView2_RowDataBound"
                                        OnSorting = "GridView2_Sorting"
                                        BorderStyle=Double BorderColor="#0083C1">
                                        <RowStyle BackColor="Gainsboro" Height="24px" />
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle BackColor="#0083C1" ForeColor="White"/>
                                        <FooterStyle BackColor="White" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Employee No." SortExpression="Employee_Number">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEmployeeID" Text='<%# Eval("Employee_Number") %>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />                                            
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" SortExpression="Cat_Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCatDate" Text='<%# Eval("Cat_Date") %>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />                                            
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="IN" SortExpression="in_am">
                                                <ItemTemplate><%# Eval("in_am")%></ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="OUT" SortExpression="out_am">
                                                <ItemTemplate><%#Eval("out_am")%></ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="IN" SortExpression="in_pm">
                                                <ItemTemplate><%#Eval("in_pm")%></ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="OUT" SortExpression="out_pm">
                                                <ItemTemplate><%#Eval("out_pm")%></ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Deduct">
                                                <ItemTemplate>
                                                       <asp:CheckBox ID="CheckBox1" runat="server" Checked="True" AutoPostBack="False" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />                        
                                            </asp:TemplateField>                                            
                                            <asp:TemplateField HeaderText="Note" SortExpression="late_note">
                                                <ItemTemplate><%#Eval("late_note")%></ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="late_note" Text='<%# Eval("late_note")%>' Width="400px" runat="server"></asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            
			                                <asp:CommandField ShowEditButton="True" ButtonType="Image" CancelImageUrl="~/images/cancel.png" CancelText="Cancel" EditImageUrl="~/images/edit.png" EditText="Edit" UpdateText="Update" UpdateImageUrl="~/images/update.png" >
			                                <ItemStyle Width="60px" />
			                                <HeaderStyle Width="60px" />
                                            </asp:CommandField>
                                        </Columns>
                                    </asp:GridView>
                                
                                
                                </div>
                             </td>
                        </tr>
			        </ItemTemplate>			       
			    </asp:TemplateField>			                                    
                                                
                
            </Columns>
        </asp:GridView>

        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="GetDataEmpListByCc" TypeName="CCEmpListTableAdapters.DataTable1TableAdapter" UpdateMethod="GetDataEmpListByCc">
            <SelectParameters>
                <asp:ControlParameter ControlID="Label1" Name="WeekNo" PropertyName="Text" Type="Int32" />
                <asp:ControlParameter ControlID="Label2" Name="CcCode" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="Label6" Name="CcYear" PropertyName="Text" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="WeekNo" Type="Int32" />
                <asp:Parameter Name="CcCode" Type="String" />
                <asp:Parameter Name="CcYear" Type="Int32" />
            </UpdateParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:Label ID="Label1" runat="server" Text="41" Width="104px" Visible="False"></asp:Label>
        <asp:Label ID="Label2" runat="server" Text="300" Width="120px" Visible="False"></asp:Label>
        <asp:Label ID="Label6" runat="server" Text="2011" Visible="False" Width="64px"></asp:Label>
        <table style="width: 1120px">
            <tr>
                <td style="width: 100px">
                    <asp:Label ID="Label3" runat="server" Text="Select Week :" Width="96px" Font-Bold="True" ForeColor="DodgerBlue"></asp:Label></td>
                <td style="width: 100px">
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" Width="212px">
                    </asp:DropDownList></td>
                <td style="width: 100px">
                    <asp:Label ID="Label5" runat="server" BackColor="AliceBlue" Text="0" Visible="False"></asp:Label></td>
                <td style="width: 100px">
                    <asp:Label ID="Label4" runat="server" Text="Department :" Width="88px" Font-Bold="True" ForeColor="DodgerBlue"></asp:Label></td>
                <td style="width: 100px">
                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" Width="352px">
                    </asp:DropDownList></td>
                <td style="width: 100px">
                    <asp:Button ID="Button1" runat="server" Text="Submit" Width="70px" Font-Size="Small" /></td>
                <td style="width: 113px">
                    <asp:Button ID="Button2" runat="server" Text="Select No to All" Width="108px" Font-Size="Small" /></td>
                <td style="width: 100px">
                    <asp:Button ID="Button3" runat="server" Font-Size="Small" Text="Select Yes to All"
                        Width="116px" /></td>
                <td style="width: 100px">
                    <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="ViewPrint()">Print</asp:LinkButton></td>
            </tr>
        </table>
    </div>
               
    </form>
    <div style="">	<a id="a" href="#" onclick="<%= sbExpandLink.ToString() %>return(false);">[+/-]</a></div>
</body>
</html>
