'Roberto 07/12/2011

Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebCR_Mag
    Inherits System.Web.UI.Page
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                btnRitorno.Visible = False
                Label1.Visible = False
            End If
        End If

        If Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.Listini Then
            Dim Rpt As New Listino
            Dim dsListino1 As New DSListino
            dsListino1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsListino1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliAnalitica Then
            Dim Rpt As New AnagArticAnalit
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliSintetica Then
            Dim Rpt As New AnagArtic
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazione Or _
            Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazioneGM Then
            Dim Rpt As New UbiArtic
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCOD Then
            Dim Rpt As New AnagArticFornCOD
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreDES Then
            Dim Rpt As New AnagArticFornDES
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCODP Then
            Dim Rpt As New AnagArticFornCODP
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreDESP Then
            Dim Rpt As New AnagArticFornDESP
            Dim dsAnaMag1 As New DSAnaMag
            dsAnaMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsAnaMag1)
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
        End If

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

    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        
        Dim strRitorno As String = ""
        If Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.Listini Then
            strRitorno = "WF_Listini.aspx?labelForm=Listini di vendita"
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliAnalitica Then
            strRitorno = "WF_AnagraficaArticoli.aspx?labelForm=Anagrafica articoli"
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliSintetica Then
            strRitorno = "WF_AnagraficaArticoli.aspx?labelForm=Anagrafica articoli"
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazione Then
            strRitorno = "WF_ArticoliUbicazione.aspx?labelForm=Ubicazione articoli"
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazioneGM Then
            strRitorno = "WF_GesMagazzini.aspx?labelForm=Gestione Magazzini"
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCOD Then
            strRitorno = "WF_StampaArticoliFornitori.aspx?labelForm=Stampa articoli fornitori."
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCOD Then
            strRitorno = "WF_StampaArticoliFornitori.aspx?labelForm=Stampa articoli fornitori."
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCODP Then
            strRitorno = "WF_StampaArticoliFornitori.aspx?labelForm=Stampa articoli fornitori CONFRONTO con Storico Prezzi."
        ElseIf Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCODP Then
            strRitorno = "WF_StampaArticoliFornitori.aspx?labelForm=Stampa articoli fornitori CONFRONTO con Storico Prezzi."
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
 
End Class