#Region "Imports"

Imports System.Data.SqlClient
Imports System.Security.Principal
Imports System.Web.UI.Page
Imports System.IO
Imports Microsoft.Reporting.WebForms

#End Region

Partial Class MgtUnauthorized
    Inherits System.Web.UI.Page

    Dim pubUser As String
    Dim pubDomain As String
    Dim pubServerDate As String
    Dim pubServerTime As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim sUser() As String = Split(User.Identity.Name, "\")
        Dim sDomain As String = sUser(0)
        Dim sUserId As String = sUser(1)

        pubUser = UCase(sUserId)

        pubDomain = UCase(sDomain)
        GetServerDateTime()
        LogUnauthorizedUser()
        Response.Write("<script>self.close()</script>")
        Response.Redirect("http://inside.saudi-ericsson.com/it/denied.html")

    End Sub

    Private Sub LogUnauthorizedUser()

        Dim strConnStr As String
        strConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=SES"
        Dim MySqlConn As New SqlConnection(strConnStr)

        Dim cmdUpdate As New SqlCommand

        cmdUpdate.CommandText = "insert into and_user_unauthorized(userid,app_module,app_domain,app_date,app_time) " & _
                        "values('" & pubUser.Trim & "','Attendance','" & pubDomain.Trim & "','" & pubServerDate & "','" & _
                        pubServerTime & "')"
        cmdUpdate.Connection = MySqlConn
        cmdUpdate.Connection.Open()
        cmdUpdate.ExecuteNonQuery()
        MySqlConn.Close()

    End Sub

    Private Sub GetServerDateTime()

        Dim ConnStr As String
        Dim sSql As String
        Dim mServerdate As Date
        ConnStr = "Data Source=seslsvrho;User ID=scheme;Password=Er1c550n2;Initial Catalog=SES"
        Dim MySqlConn As New SqlConnection(ConnStr)
        MySqlConn.Open()
        Try

            'sSql = "Select top 1 getdate() as logdate from and_poshipment_users"
            sSql = "select getdate() as logdate"

            Dim MySqlCmd As New SqlCommand(sSql, MySqlConn)
            Dim mReader As SqlDataReader

            mReader = MySqlCmd.ExecuteReader()
            If mReader.HasRows Then

                While mReader.Read()

                    mServerdate = mReader("logdate".ToString)

                    pubServerDate = Format(mServerdate, "dd MMM yyyy")
                    pubServerTime = Format(mServerdate, "hh:mm")

                End While

            End If

        Catch ex As Exception

        Finally
            MySqlConn.Close()
        End Try

    End Sub

End Class
