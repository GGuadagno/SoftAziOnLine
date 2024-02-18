Public Partial Class WF_ErroreUtenteConnesso
    Inherits System.Web.UI.Page
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MessageLabel.Text = Request.QueryString("labelForm")
        If InStr(MessageLabel.Text.Trim.ToUpper, "ERRORE") > 0 Then
            MessageLabel.ForeColor = Drawing.Color.Red
        Else
            MessageLabel.ForeColor = Drawing.Color.Black
        End If
       
    End Sub

    Private Sub LogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOut.Click
        Response.Redirect("../Login.aspx")
        Exit Sub
    End Sub
End Class