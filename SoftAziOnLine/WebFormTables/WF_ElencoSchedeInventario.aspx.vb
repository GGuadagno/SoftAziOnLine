Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_ElencoSchedeInventario
    Inherits System.Web.UI.Page
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
    End Sub

End Class