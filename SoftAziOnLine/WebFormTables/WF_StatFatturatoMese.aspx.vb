Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_StatFatturatoMese
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
        Dim labelForm As String = Request.QueryString("labelForm")
        If IsNothing(labelForm) Then
            labelForm = ""
        End If
        If String.IsNullOrEmpty(labelForm) Then
            labelForm = ""
        End If
        If (Not IsPostBack) Then
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr
            If Mid(UCase(labelForm.Trim), 1, 18) = "FATTURATO ARTICOLI" Then
                Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt
            End If
        End If
        
    End Sub

End Class