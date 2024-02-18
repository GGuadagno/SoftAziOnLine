Public Partial Class GenericErrorPage
    Inherits System.Web.UI.Page
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myLabel As String = Request.QueryString("labelForm")
        If IsNothing(myLabel) Then
            myLabel = ""
        ElseIf String.IsNullOrEmpty(myLabel) Then
            myLabel = ""
        End If
        If myLabel.Trim <> "" Then
            lblintesta.Text = myLabel.Trim
        End If
    End Sub

End Class