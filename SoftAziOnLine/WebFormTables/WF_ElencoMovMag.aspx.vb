Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_ElencoMovMag
    Inherits System.Web.UI.Page
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
        If (Not IsPostBack) Then
            Dim SWRbtnMM As String = Session(CSTSWRbtnMM)
            If IsNothing(CSTSWRbtnMM) Then
                SWRbtnMM = ""
            End If
            If String.IsNullOrEmpty(CSTSWRbtnMM) Then
                SWRbtnMM = ""
            End If
            If SWRbtnMM = "" Then
                Session(CSTSWRbtnMM) = "TUTTI"
            End If
        End If
    End Sub

End Class