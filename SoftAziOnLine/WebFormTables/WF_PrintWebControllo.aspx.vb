Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebControllo
    Inherits System.Web.UI.Page
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffPrezzoListino Then
            Dim Rpt As New DiffPrezzoListinoPerArt
            Dim DSDiffPrezzoListino1 As New DSControlli
            DSDiffPrezzoListino1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSDiffPrezzoListino1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffImportoRiga Then
            Dim Rpt As New DiffImportoRigaPerArt
            Dim DSDiffImportoRiga1 As New DSControlli
            DSDiffImportoRiga1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSDiffImportoRiga1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.CKSerieLotto Then
            Dim Rpt As New ContrattiCKSerieLotto
            Dim DSCKSerieLotto1 As New DSPrintWeb_Documenti
            DSCKSerieLotto1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSCKSerieLotto1)
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA CONTROLLO SCONOSCIUTA")
        End If


    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String

        If Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffPrezzoListino Then
            strRitorno = "WF_DiffPrezzoListino.aspx?labelForm=Controllo differenza Prezzo/Prezzo di listino"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If

        Try
            Response.Redirect(strRitorno)
            Exit Sub
        Catch ex As Exception
            Response.Redirect(strRitorno)
            Exit Sub
        End Try
    End Sub

    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub
End Class