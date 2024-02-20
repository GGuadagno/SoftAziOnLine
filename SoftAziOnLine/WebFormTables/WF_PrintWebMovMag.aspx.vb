Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebMovMag
    Inherits System.Web.UI.Page
   
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                btnRitorno.Visible = False
                Label1.Visible = False
            End If
        End If

        If Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagDaDataAData Or
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti Or
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.VendutoLeadSourceA Or
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.VendutoLeadSourceS Then
            Dim Rpt As New MovMag
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ElencoDDTMagCaus Then
            Dim Rpt As New ElDDTMagCaus
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByArticolo Then
            Dim Rpt As New MovMagPerArt
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = True
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortCliForNDoc Then
            Dim Rpt As New ValCMSMCliForNDoc 'FatturatoClienteFattura
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByNDoc Then
            Dim Rpt As New ValCMSMOrdineSortByNDoc 'FatturatoOrdineSortByNDoc
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByDataDoc Then
            Dim Rpt As New ValCMSMOrdineSortByDataDoc 'FatturatoOrdineSortByDataDoc
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMSintOrdineSortByNDoc Then
            Dim Rpt As New ValCMSMSintOrdineSortByNDoc 'FattSintOrdineSortByNDoc
            Dim DsMovMag1 As New DSMovMag
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsMovMag1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA MOVIMENTI DI MAGAZZINO SCONOSCIUTA")
        End If
    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = ""

        If Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagDaDataAData Then
            strRitorno = "WF_StatMovMag.aspx?labelForm=Stampa movimenti di magazzino"
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.VendutoLeadSourceA Or Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.VendutoLeadSourceS Then
            strRitorno = "WF_StatMovMag.aspx?labelForm=Venduto per Lead Source"
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti Then
            strRitorno = "WF_ElencoMovMag.aspx?labelForm=Gestione movimenti di magazzino"
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortCliForNDoc Or _
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByNDoc Or _
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByDataDoc Or _
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMSintOrdineSortByNDoc Then
            strRitorno = "WF_ValorizzaCarichiScarichiDoc.aspx?labelForm=Valorizza Carichi/Scarichi Fornitori"
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByArticolo Then
            strRitorno = "WF_StatMovMagArt.aspx?labelForm=Stampa movimenti di magazzino per articolo"
        ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ElencoDDTMagCaus Then
            strRitorno = "WF_ElDDTMagCaus.aspx?labelForm=Elenco DDT Clienti per Magazzino/Causale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If
        'strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
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