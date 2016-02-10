Imports System
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Data.OleDb

Partial Class _Default
    Inherits System.Web.UI.Page
    Public gvUniqueID As String = String.Empty

    Public gRidViewUniqueId As String = String.Empty

    Public gRidview2ChkEnabled As Boolean = False
    Dim pubUser As String
    Dim pubServerPeriod As String
    Dim pubEmpName As String
    Dim pubEmpNo As String
    Dim gEmpNo As String
    Dim gLateCountEmp As Integer = 0
    Dim gIfCheck2 As Boolean = False
    Dim gStrDate2 As String
    Dim pubCountTemp As Integer = 0

#Region "Variables"
    Protected sbExpandLink As StringBuilder = New System.Text.StringBuilder("")
    'Private gvUniqueID As String = String.Empty
    Private gvNewPageIndex As Integer = 0
    Private gvEditIndex As Integer = -1
    Private gvSortExpr As String = [String].Empty
    Public gvSortDir As String = "ASC"

    '    Private Property gvSortDir() As String

    '       Get
    '     Return If(TryCast(ViewState("SortDirection"), String), "ASC")
    '     End Get

    '    Set(ByVal value As String)
    '       ViewState("SortDirection") = value
    '  End Set
    'End Property

#End Region

    'This procedure returns the Sort Direction
    'Private Function GetSortDirection() As String
    '   Select Case gvSortDir
    '      Case "ASC"
    '         gvSortDir = "DESC"
    '        Exit Select
    '
    '           Case "DESC"
    '              gvSortDir = "ASC"
    '            Exit Select
    '     End Select
    '   Return gvSortDir
    ' End Function

    'This procedure prepares the query to bind the child GridView

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim sUser() As String = Split(User.Identity.Name, "\")
        Dim sDomain As String = sUser(0)
        Dim sUserId As String = sUser(1)

        pubUser = UCase(sUserId)

        If Not IsPostBack Then
            GetUserAccess()
            GetServerDateTime()
            FillupWeek()
            If Label5.Text.Trim = "0" Then
                FillUpDepartment()
            Else
                FillUpDepartmentAll()
            End If

        End If

    End Sub

    Private Function ChildDataSource(ByVal strEmployeeId As String, ByVal strWeekNo As String, ByVal strSort As String) As DataSet

        Dim pubConnstr As String
        pubConnstr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2"
        Dim strQRY As String

        Dim myConnection As SqlConnection = New SqlConnection(pubConnstr)

        strQRY = "SELECT Employee_Number, Week_No, " & _
                    "rtrim(convert(char,datepart(dd,Cat_Date)))+' '+datename(m,Cat_Date)+' '+rtrim(convert(char,datepart(yyyy,Cat_Date))) as Cat_Date," & _
                    "in_am, out_am, in_pm, out_pm, late_note, " & _
                    "check_late FROM Ericsson.scheme.att_late_summary_detail where Employee_Number = '" & strEmployeeId & _
                    "' and Week_No = '" & strWeekNo & "' and Week_Year = '" & Label6.Text.Trim & "'"

        Dim ad As SqlDataAdapter = New SqlDataAdapter(strQRY, myConnection)

        Dim ds As Data.DataSet = New Data.DataSet()

        ad.Fill(ds, "tblDates")

        Return ds

    End Function

#Region "GridView1 Event Handlers"

    'This event occurs for each row
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim row As GridViewRow = e.Row
        Dim strSort As String = String.Empty

        ' Make sure we aren't in header/footer rows
        If row.DataItem Is Nothing Then
            Exit Sub
        End If

        If e.Row.RowType = DataControlRowType.DataRow Then
            If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "Count_30")) Then
                If Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Count_30")) = 0 Then
                    e.Row.Cells(3).Text = ""
                End If
            End If

            If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "Count_60")) Then
                If Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Count_60")) = 0 Then
                    e.Row.Cells(4).Text = ""
                End If
            End If

            If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "CountLate")) Then
                If Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "CountLate")) = 0 Then
                    e.Row.Cells(5).Text = ""
                End If
            End If

            If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "Deduct")) Then
                If Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Deduct")).Trim = "1" Then
                    DirectCast(e.Row.Cells(6).FindControl("RadioButtonList1"), RadioButtonList).SelectedValue = "0"
                Else
                    DirectCast(e.Row.Cells(6).FindControl("RadioButtonList1"), RadioButtonList).SelectedValue = "1"
                End If
            End If

        End If


        'Find Child GridView control
        Dim gv As New GridView()
        gv = DirectCast(row.FindControl("GridView2"), GridView)

        'Check if any additional conditions (Paging, Sorting, Editing, etc) to be applied on child GridView
        If gv.UniqueID = gvUniqueID Then
            gv.PageIndex = gvNewPageIndex
            gv.EditIndex = gvEditIndex
            'Check if Sorting used
            If gvSortExpr <> String.Empty Then
                'GetSortDirection()
                strSort = " ORDER BY " & String.Format("{0} {1}", gvSortExpr, gvSortDir)
            End If
            'Expand the Child grid
            ClientScript.RegisterStartupScript([GetType](), "Expand", "<SCRIPT LANGUAGE='javascript'>expandcollapse('div" & DirectCast(e.Row.DataItem, DataRowView)("Employee_Number").ToString() & "','one');</script>")
        End If

        'Prepare the query for Child GridView by passing the Customer ID of the parent row
        gv.DataSource = ChildDataSource(DirectCast(e.Row.DataItem, DataRowView)("Employee_Number").ToString(), DirectCast(e.Row.DataItem, DataRowView)("Week_No").ToString(), strSort)
        gv.DataBind()

        sbExpandLink.Append("expandcollapse('div" & DirectCast(e.Row.DataItem, DataRowView)("Employee_Number").ToString() & "', 'one');")

        'Add delete confirmation message for Customer
        'Dim l As LinkButton = DirectCast(e.Row.FindControl("linkDeleteCust"), LinkButton)

        'l.Attributes.Add("onclick", ("javascript:return " & "confirm('Are you sure you want to delete this Customer ") + DataBinder.Eval(e.Row.DataItem, "Employee_Number") & "')")
    End Sub

#End Region

#Region "GridView2 Event Handlers"

#End Region

    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvNewPageIndex = e.NewPageIndex
        GridView1.DataBind()
    End Sub

    Protected Sub GridView2_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvEditIndex = e.NewEditIndex
        GridView1.DataBind()
    End Sub

    Protected Sub GridView2_CancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvEditIndex = -1
        GridView1.DataBind()
    End Sub

    Protected Sub GridView2_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Try
            Dim gvTemp As GridView = DirectCast(sender, GridView)
            gvUniqueID = gvTemp.UniqueID

            'Get the values stored in the text boxes
            Dim strEmployeeID As String = DirectCast(gvTemp.Rows(e.RowIndex).FindControl("lblEmployeeID"), Label).Text

            gEmpNo = strEmployeeID

            Dim strCatDate As String = DirectCast(gvTemp.Rows(e.RowIndex).FindControl("lblCatDate"), Label).Text
            Dim strLateNote As String = DirectCast(gvTemp.Rows(e.RowIndex).FindControl("late_note"), TextBox).Text

            Dim IfCheck As Boolean = DirectCast(gvTemp.Rows(e.RowIndex).FindControl("CheckBox1"), CheckBox).Checked

            Dim strConnStr As String
            strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
            Dim MySqlConn As New SqlConnection(strConnStr)

            Dim cmdUpdate As New SqlCommand

            'cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary_detail set late_note = '" & Single.Parse(strLateNote) & _
            '            "' WHERE Employee_Number = '" & strEmployeeID & "' and Cat_Date = '" & strCatDate & "'"
            If InStr(1, strLateNote.Trim, "'") > 0 Then
                strLateNote = Mid(strLateNote.Trim, 1, InStr(1, strLateNote.Trim, "'") - 1) & _
                        Mid(strLateNote.Trim, InStr(1, strLateNote.Trim, "'") + 1, Len(strLateNote.Trim) - InStr(1, strLateNote.Trim, "'"))
            End If

            If InStr(1, strLateNote.Trim, "'") > 0 Then
                strLateNote = Mid(strLateNote.Trim, 1, InStr(1, strLateNote.Trim, "'") - 1) & _
                        Mid(strLateNote.Trim, InStr(1, strLateNote.Trim, "'") + 1, Len(strLateNote.Trim) - InStr(1, strLateNote.Trim, "'"))
            End If

            If InStr(1, strLateNote.Trim, "'") > 0 Then
                strLateNote = Mid(strLateNote.Trim, 1, InStr(1, strLateNote.Trim, "'") - 1) & _
                        Mid(strLateNote.Trim, InStr(1, strLateNote.Trim, "'") + 1, Len(strLateNote.Trim) - InStr(1, strLateNote.Trim, "'"))
            End If

            If IfCheck = False Then
                cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary_detail set late_note = '" & strLateNote.Trim & _
                            "', check_late = '0' WHERE Employee_Number = '" & strEmployeeID & "' and Cat_Date = '" & strCatDate & "'"
            Else
                cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary_detail set late_note = '" & strLateNote.Trim & _
                            "', check_late = '1'  WHERE Employee_Number = '" & strEmployeeID & "' and Cat_Date = '" & strCatDate & "'"
            End If



            cmdUpdate.Connection = MySqlConn
            cmdUpdate.Connection.Open()
            cmdUpdate.ExecuteNonQuery()
            MySqlConn.Close()


            'ClientScript.RegisterStartupScript([GetType](), "Message", "<SCRIPT LANGUAGE='javascript'>alert('Updated successfully');</script>")

            'Reset Edit Index
            gvEditIndex = -1

            'Check and Edit Rows - Deduct or not
            CheckEmpLateCount()
            Dim i As Integer = 0

            For i = 0 To gLateCountEmp - 1
                Dim IfCheck2 As Boolean = DirectCast(gvTemp.Rows(i).FindControl("CheckBox1"), CheckBox).Checked
                Dim strCatDate2 As String = DirectCast(gvTemp.Rows(i).FindControl("lblCatDate"), Label).Text

                gIfCheck2 = IfCheck2

                gStrDate2 = strCatDate2

                UpdateEmpDetailsDeduct()

                If i = gLateCountEmp - 1 Then
                    CountEmpDeductDetails()
                    UpdateCountDeduct()

                End If

            Next


            GridView1.DataBind()
        Catch

        End Try
    End Sub

    Protected Sub GridView2_RowUpdated(ByVal sender As Object, ByVal e As GridViewUpdatedEventArgs)
        'Check if there is any exception while deleting
        If e.Exception IsNot Nothing Then
            ClientScript.RegisterStartupScript([GetType](), "Message", "<SCRIPT LANGUAGE='javascript'>alert('" & e.Exception.Message.ToString().Replace("'", "") & "');</script>")
            e.ExceptionHandled = True
        End If
    End Sub

    Protected Sub GridView2_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID

        'Get the value 
        Dim strOrderID As String = DirectCast(gvTemp.Rows(e.RowIndex).FindControl("lblEmployeeID"), Label).Text

        'Prepare the Update Command of the DataSource control
        Dim strSQL As String = ""

        Try
            strSQL = "DELETE from Orders WHERE OrderID = " & strOrderID
            Dim dsTemp As New AccessDataSource()
            dsTemp.DataFile = "App_Data/Northwind.mdb"
            dsTemp.DeleteCommand = strSQL
            dsTemp.Delete()
            ClientScript.RegisterStartupScript([GetType](), "Message", "<SCRIPT LANGUAGE='javascript'>alert('Order deleted successfully');</script>")
            GridView1.DataBind()
        Catch
        End Try
    End Sub

    Protected Sub GridView2_RowDeleted(ByVal sender As Object, ByVal e As GridViewDeletedEventArgs)
        'Check if there is any exception while deleting
        If e.Exception IsNot Nothing Then
            ClientScript.RegisterStartupScript([GetType](), "Message", "<SCRIPT LANGUAGE='javascript'>alert('" & e.Exception.Message.ToString().Replace("'", "") & "');</script>")
            e.ExceptionHandled = True
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        'Check if this is our Blank Row being databound, if so make the row invisible
        If e.Row.RowType = DataControlRowType.DataRow Then
            If DirectCast(e.Row.DataItem, DataRowView)("Employee_Number").ToString() = [String].Empty Then
                e.Row.Visible = False
            End If

            'If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "Cat_Date")) Then
            'e.Row.Cells(1).Text = Format(CDate(Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Cat_Date"))), "dd MMM yyyy")
            'End If
            If Not IsDBNull(DataBinder.Eval(e.Row.DataItem, "check_late")) Then
                If Convert.ToString(DataBinder.Eval(e.Row.DataItem, "check_late")).Trim = "1" Then
                    DirectCast(e.Row.Cells(6).FindControl("CheckBox1"), CheckBox).Checked = True
                Else
                    DirectCast(e.Row.Cells(6).FindControl("CheckBox1"), CheckBox).Checked = False
                End If
                If gRidview2ChkEnabled = False Then
                    DirectCast(e.Row.Cells(6).FindControl("CheckBox1"), CheckBox).Enabled = False
                Else
                    DirectCast(e.Row.Cells(6).FindControl("CheckBox1"), CheckBox).Enabled = True
                End If
            End If

        End If

    End Sub

    Protected Sub GridView2_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

        Select Case e.CommandName
            Case "Edit"
                gRidview2ChkEnabled = True
            Case "Cancel"
                gRidview2ChkEnabled = False
            Case "Update"
                gRidview2ChkEnabled = False
        End Select

    End Sub

    Protected Sub GridView2_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvSortExpr = e.SortExpression
        GridView1.DataBind()
    End Sub

    Private Sub GetServerDateTime()

        Dim ConnStr As String
        Dim sSql As String
        Dim mServerdate As Date
        ConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            sSql = "Select top 1 getdate() as logdate from and_rmperiod"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then

                While mReader.Read()

                    mServerdate = mReader("logdate".ToString)

                    pubServerPeriod = Format(mServerdate, "yyyyMM")

                End While

            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub GetUserAccess()

        Dim ConnStr As String
        Dim sSql As String

        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            sSql = "Select userid, full_access from att_web_users where userid= '" & pubUser.Trim & "'"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then
                While mReader.Read()
                    Label5.Text = mReader("full_access")
                End While
            Else
                MySqlConn.Close()
                Response.Redirect("MgtUnauthorized.aspx")
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub FillupWeek()

        Dim ConnStr As String
        Dim sSql As String
        Dim mctr As Integer = 0
        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            '            If Label5.Text.Trim = "0" Then
            'sSql = "SELECT att_Web_Weeks.Weekno_and_Date FROM att_Web_Weeks INNER JOIN " & _
            '           "(att_web_weekstatus INNER JOIN att_web_users_cc ON att_web_weekstatus.cc = att_web_users_cc.cc) " & _
            '          "ON att_Web_Weeks.Week_no = att_web_weekstatus.Week_no " & _
            '         "GROUP BY att_Web_Weeks.Weekno_and_Date, att_web_users_cc.userid " & _
            '        "HAVING att_web_users_cc.userid='" & pubUser.Trim & _
            '       "' ORDER BY att_Web_Weeks.Weekno_and_Date DESC"

            'sSql = "SELECT att_Web_Weeks.Weekno_and_Date FROM att_Web_Weeks INNER JOIN " & _
            '        "(att_web_weekstatus INNER JOIN att_web_users_cc ON att_web_weekstatus.cc = att_web_users_cc.cc) " & _
            '        "ON att_Web_Weeks.Week_no = att_web_weekstatus.Week_no GROUP BY att_Web_Weeks.Weekno_and_Date, " & _
            '        "att_web_weekstatus.Status, att_web_users_cc.userid HAVING " & _
            '        "att_web_users_cc.userid='" & pubUser.Trim & "' AND att_web_weekstatus.Status='0' ORDER BY att_Web_Weeks.Weekno_and_Date DESC"
            'Else
            'sSql = "SELECT att_Web_Weeks.Weekno_and_Date FROM att_Web_Weeks INNER JOIN " & _
            '        "att_web_weekstatus ON att_Web_Weeks.Week_no = att_web_weekstatus.Week_no " & _
            '        "GROUP BY att_Web_Weeks.Weekno_and_Date ORDER BY att_Web_Weeks.Weekno_and_Date DESC"
            'End If
            sSql = "select Week_Description as Weekno_and_Date from att_Web_CurrentWeeks order by Serial_number desc"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then

                While mReader.Read()
                    mctr = mctr + 1
                    DropDownList1.Items.Add(mReader("Weekno_and_Date"))
                    If mctr = 1 Then
                        Label1.Text = Mid(Trim(mReader("Weekno_and_Date")), 1, 2)
                        Label6.Text = Right(Trim(mReader("Weekno_and_Date")), 4)
                    End If

                End While
            Else
                Label1.Text = "888"
                Button1.Visible = False
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub FillUpDepartment()

        Dim ConnStr As String
        Dim sSql As String
        Dim mctr As Integer = 0
        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try
            sSql = "SELECT att_web_weekstatus.cc+' - '+scheme_nlcostm.long_description AS ccdesc, " & _
                    "att_web_users_cc.submit_access, att_web_weekstatus.Status FROM " & _
                    "att_web_weekstatus INNER JOIN scheme_nlcostm ON att_web_weekstatus.cc = scheme_nlcostm.cost_code " & _
                    "INNER JOIN att_web_users_cc ON att_web_weekstatus.cc = att_web_users_cc.cc WHERE " & _
                    "att_web_users_cc.userid = '" & pubUser.Trim & _
                    "' and att_web_weekstatus.Week_no ='" & Label1.Text.Trim & _
                    "' and att_web_weekstatus.Week_Year ='" & Label6.Text.Trim & "' ORDER BY att_web_weekstatus.cc"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then

                While mReader.Read()
                    mctr = mctr + 1
                    'DropDownList2.Items.Add(mReader("ccdesc"))
                    Select Case mReader("Status")
                        Case "0"
                            DropDownList2.Items.Add(mReader("ccdesc"))
                        Case "1"
                            DropDownList2.Items.Add(Trim(mReader("ccdesc")) + " - Submitted")
                        Case "3"
                            DropDownList2.Items.Add(Trim(mReader("ccdesc")) + " - Approved")
                    End Select
                    If mctr = 1 Then
                        Label2.Text = Mid(Trim(mReader("ccdesc")), 1, 3)
                        If Trim(mReader("submit_access")) = "0" Then
                            Button1.Visible = False
                        End If
                    End If

                End While
            Else
                Label2.Text = "888"
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub FillUpDepartmentAll()

        Dim ConnStr As String
        Dim sSql As String
        Dim mctr As Integer = 0
        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try
            sSql = "SELECT att_web_weekstatus.cc+' - '+scheme_nlcostm.long_description AS ccdesc, att_web_weekstatus.Status, att_web_weekstatus.Week_no " & _
                        "FROM att_web_weekstatus INNER JOIN scheme_nlcostm ON att_web_weekstatus.cc = scheme_nlcostm.cost_code " & _
                        "WHERE att_web_weekstatus.Week_no = '" & Label1.Text.Trim & _
                        "' and att_web_weekstatus.Week_Year = '" & Label6.Text.Trim & "' ORDER BY att_web_weekstatus.cc"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then

                While mReader.Read()
                    mctr = mctr + 1
                    Select Case mReader("Status")
                        Case "0"
                            DropDownList2.Items.Add(mReader("ccdesc"))
                        Case "1"
                            DropDownList2.Items.Add(Trim(mReader("ccdesc")) + " - Submitted")
                        Case "3"
                            DropDownList2.Items.Add(Trim(mReader("ccdesc")) + " - Approved")
                    End Select
                    If mctr = 1 Then
                        Label2.Text = Mid(Trim(mReader("ccdesc")), 1, 3)
                        If mReader("Status") = "1" Then
                            Button1.Visible = True
                            Button1.Text = "Return"
                        Else
                            Button1.Visible = False
                        End If

                    End If

                End While

            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Protected Sub DropDownList2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownList2.SelectedIndexChanged
        Label2.Text = Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3)
        If InStr(1, DropDownList2.SelectedItem.Text.Trim, "Submitted") > 0 Then
            If Label5.Text.Trim = "1" Then
                Button1.Visible = True
                Button1.Text = "Return"
            Else
                If Label5.Text.Trim = "3" Then
                    Button1.Visible = True
                    Button1.Text = "Approve"
                End If
            End If
        Else
            If Label5.Text.Trim = "1" Or Label5.Text.Trim = "3" Then
                If Label2.Text.Trim = "300" Then
                    If Label5.Text.Trim = "3" Then
                        Button1.Visible = True
                        Button1.Text = "Approve"
                    Else
                        Button1.Visible = False
                        Button1.Text = "Submit"
                    End If
                Else
                    Button1.Visible = False
                    Button1.Text = "Submit"
                End If
            End If
        End If
    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownList1.SelectedIndexChanged

        Label1.Text = Mid(DropDownList1.SelectedItem.Text.Trim, 1, 2)
        Label6.Text = Right(DropDownList1.SelectedItem.Text.Trim, 4)
        DropDownList2.Items.Clear()
        If Label5.Text.Trim = "0" Then
            FillUpDepartment()
        Else
            FillUpDepartmentAll()
        End If

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        If Label5.Text.Trim = "0" Then
            If InStr(1, DropDownList2.SelectedItem.Text.Trim, "Submitted") > 0 Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Submitted", "<script language='javascript'>" & Environment.NewLine _
                  & "window.alert('Submitted Already...')</script>")
                Exit Sub
            End If
        End If

        If Label5.Text.Trim = "0" Then
            UpdateWeekStatus()
            DropDownList2.Items.Clear()
            DropDownList1.Items.Clear()
            FillupWeek()
            FillUpDepartment()
        Else
            If Label5.Text.Trim = "1" Then
                UpdateWeekStatusReturn()
            Else
                UpdateWeekStatusApproved()
            End If

            DropDownList2.Items.Clear()
            DropDownList1.Items.Clear()
            FillupWeek()
            FillUpDepartmentAll()
        End If

    End Sub

    Private Sub UpdateWeekStatus()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_web_weekstatus set Status = '1' where cc = '" & _
                                    Label2.Text.Trim & "' and Week_no = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateWeekStatusReturn()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_web_weekstatus set Status = '0' where cc = '" & _
                                    Label2.Text.Trim & "' and Week_no = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateWeekStatusApproved()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_web_weekstatus set Status = '3' where cc = '" & _
                                    Label2.Text.Trim & "' and Week_no = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateEmployeeDeductToYes()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary set Deduct = '1', Count_Deduct = [Count_30]+[Count_60]+[Count_1] where Employee_Number = '" & _
                                    pubEmpNo.Trim & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateEmployeeDeductDetailToYes()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary_detail set check_late = '1' where Employee_Number = '" & _
                                    pubEmpNo.Trim & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateEmployeeDeductToNo()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary set Deduct = '0', Count_Deduct = 0 where Employee_Number = '" & _
                                    pubEmpNo.Trim & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateEmployeeDeductDetailToNo()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary_detail set check_late = '0' where Employee_Number = '" & _
                                    pubEmpNo.Trim & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductAllToNo()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary set Deduct = '0' where Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductAllToYes()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary set Deduct = '1' where Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductAllDetailsToNo()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary_detail set check_late = '0' where Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductSummToNo()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary set Count_Deduct = '0', Deduct = '0' WHERE Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductSummToYes()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary set Count_Deduct = [Count_30]+[Count_60]+[Count_1], Deduct = '1' WHERE Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateCcDeductAllDetailsToYes()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "update att_late_summary_detail set check_late = '1' where Employee_Number " & _
                                "in(SELECT a.Employee_Number FROM att_late_summary AS a INNER JOIN " & _
                                "dbo.View_sir_emplist AS b ON a.Employee_Number = b.EmployeeNo " & _
                                "WHERE a.Week_No = '" & Label1.Text.Trim & "' and b.DepartmentCode = '" & _
                                Mid(DropDownList2.SelectedItem.Text.Trim, 1, 3) & "') and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub UpdateEmpDetailsDeduct()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand


        If gIfCheck2 = False Then
            cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary_detail set check_late = '0' WHERE Employee_Number = '" & _
                       gEmpNo & "' and Cat_Date = '" & gStrDate2 & "' and Week_Year = '" & Label6.Text.Trim & "'"
        Else
            cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary_detail set check_late = '1'  WHERE Employee_Number = '" & _
                        gEmpNo & "' and Cat_Date = '" & gStrDate2 & "' and Week_Year = '" & Label6.Text.Trim & "'"
        End If

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub CountEmpDeductDetails()

        Dim ConnStr As String
        Dim sSql As String

        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            sSql = "Select count(Employee_Number) as countemp from att_late_summary_detail where " & _
                        "Employee_Number = '" & gEmpNo & "' and Week_No = '" & Label1.Text.Trim & _
                        "' and check_late = '1' and Week_Year = '" & Label6.Text.Trim & "'"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then
                While mReader.Read()
                    If mReader("countemp") = 0 Then
                        pubCountTemp = 0
                    Else
                        pubCountTemp = mReader("countemp")
                    End If
                End While
            Else
                pubCountTemp = 0
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub UpdateCountDeduct()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand


        If pubCountTemp = 0 Then
            cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary set Count_Deduct = '0', Deduct = '0' WHERE Employee_Number = '" & _
                       gEmpNo & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"
        Else
            cmdUpdate.CommandText = "UPDATE Ericsson.scheme.att_late_summary set Count_Deduct = '" & pubCountTemp & "', Deduct = '1' WHERE Employee_Number = '" & _
                       gEmpNo & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"
        End If

        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub GetEmployeeNumber()

        Dim ConnStr As String
        Dim sSql As String

        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            sSql = "Select EmployeeNo from dbo.View_sir_emplist where EmployeeName ='" & pubEmpName.Trim & "'"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then
                While mReader.Read()
                    pubEmpNo = mReader("EmployeeNo")
                End While
            Else
                MySqlConn.Close()
                Response.Redirect("MgtUnauthorized.aspx")
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Private Sub CheckEmpLateCount()

        Dim ConnStr As String
        Dim sSql As String

        ConnStr = "Data Source=SESLSVRHO;User ID=scheme;Password=Er1c550n2;initial catalog=Ericsson"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            sSql = "Select count(Employee_Number) as countemp from att_late_summary_detail where Employee_Number = '" & _
                        gEmpNo.Trim & "' and Week_No = '" & Label1.Text.Trim & "' and Week_Year = '" & Label6.Text.Trim & "'"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then
                While mReader.Read()
                    gLateCountEmp = mReader("countemp")
                End While
            Else
                MySqlConn.Close()
                Response.Redirect("MgtUnauthorized.aspx")
            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

    Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkButton1.Click
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub rbl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim RadioButtonList1 As RadioButtonList = CType(sender, RadioButtonList)
        Dim gvr As GridViewRow = CType(RadioButtonList1.Parent.Parent, GridViewRow)

        If InStr(1, DropDownList2.SelectedItem.Text.Trim, "Submitted") = 0 Then

            pubEmpName = gvr.Cells(2).Text.Trim

            If InStr(1, pubEmpName.Trim, "'") > 0 Then
                pubEmpName = Mid(pubEmpName.Trim, 1, InStr(1, pubEmpName.Trim, "'") - 1) & _
                            "''" & Mid(pubEmpName.Trim, InStr(1, pubEmpName.Trim, "'") + 1, Len(pubEmpName.Trim) - InStr(1, pubEmpName.Trim, "'"))
            End If

            GetEmployeeNumber()

            If RadioButtonList1.SelectedValue = "1" Then
                'No
                UpdateEmployeeDeductToNo()
                UpdateEmployeeDeductDetailToNo()
            ElseIf RadioButtonList1.SelectedValue = "0" Then
                'Yes
                UpdateEmployeeDeductToYes()
                UpdateEmployeeDeductDetailToYes()
            End If

            GridView1.DataBind()
        Else
            If Label5.Text.Trim <> "0" Then
                pubEmpName = gvr.Cells(2).Text.Trim

                If InStr(1, pubEmpName.Trim, "'") > 0 Then
                    pubEmpName = Mid(pubEmpName.Trim, 1, InStr(1, pubEmpName.Trim, "'") - 1) & _
                                "''" & Mid(pubEmpName.Trim, InStr(1, pubEmpName.Trim, "'") + 1, Len(pubEmpName.Trim) - InStr(1, pubEmpName.Trim, "'"))
                End If

                GetEmployeeNumber()

                If RadioButtonList1.SelectedValue = "1" Then
                    'No
                    UpdateEmployeeDeductToNo()
                    UpdateEmployeeDeductDetailToNo()
                ElseIf RadioButtonList1.SelectedValue = "0" Then
                    'Yes
                    UpdateEmployeeDeductToYes()
                    UpdateEmployeeDeductDetailToYes()
                End If

                GridView1.DataBind()
            End If
        End If

    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        UpdateCcDeductAllToNo()
        UpdateCcDeductAllDetailsToNo()
        UpdateCcDeductSummToNo()
        GridView1.DataBind()
    End Sub

    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        UpdateCcDeductAllToYes()
        UpdateCcDeductAllDetailsToYes()
        UpdateCcDeductSummToYes()
        GridView1.DataBind()
    End Sub

End Class
