Imports SoftAziOnLine.Def
Partial Public Class WF_StatOrdForOrdTutti
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'giu271013 create 2 sub per 2 stampe diverse
        Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdForOrdTutti
    End Sub

End Class