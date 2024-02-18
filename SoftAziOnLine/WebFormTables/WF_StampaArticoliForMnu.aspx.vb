Imports SoftAziOnLine.Def
Partial Public Class WF_StampaArticoliForMnu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_Menu.aspx?labelForm=Menu principale"
    End Sub

End Class