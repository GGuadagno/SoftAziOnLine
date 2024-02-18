Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_CausaliMagazzino
    Inherits System.Web.UI.Page
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Private Sub WF_ArticoliUbicazione_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
        ' ''If (Not IsPostBack) Then
        ' ''    Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
        ' ''    If IsNothing(SWRbtnTD) Then
        ' ''        SWRbtnTD = ""
        ' ''    End If
        ' ''    If String.IsNullOrEmpty(SWRbtnTD) Then
        ' ''        SWRbtnTD = ""
        ' ''    End If
        ' ''    If SWRbtnTD = "" Then
        ' ''        Session(CSTSWRbtnTD) = "rbtnTutti"
        ' ''    End If
        ' ''End If
    End Sub

End Class