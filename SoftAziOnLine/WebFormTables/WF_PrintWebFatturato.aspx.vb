Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebFatturato
    Inherits System.Web.UI.Page
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
   
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDoc Then
            Dim Rpt As New FatturatoClienteFattura
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Or _
            Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargMP Then
            Dim mySWFor As String = ""
            Try
                mySWFor = Session("FatturatoClientiMargineFor")
                If String.IsNullOrEmpty(mySWFor) Then
                    mySWFor = ""
                End If
            Catch ex As Exception
                mySWFor = ""
            End Try
            'giu300122
            Dim mySWAge As String = ""
            Try
                mySWAge = Session("FatturatoClientiMargineAGE")
                If String.IsNullOrEmpty(mySWAge) Then
                    mySWAge = ""
                End If
            Catch ex As Exception
                mySWAge = ""
            End Try
            '-
            Dim mySWReg As String = ""
            Try
                mySWReg = Session("FatturatoClientiMargineREG")
                If String.IsNullOrEmpty(mySWReg) Then
                    mySWReg = ""
                End If
            Catch ex As Exception
                mySWReg = ""
            End Try
            '---------
            If mySWFor = "" And mySWAge = "" And mySWReg = "" Then
                Dim Rpt As New FatturatoClienteFatturaMarg
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = True
                CrystalReportViewer1.ReportSource = Rpt
            ElseIf mySWFor.Trim = "Si" Then
                If mySWAge = "Si" Then
                    Dim Rpt As New FatturatoClienteFatturaMargForAG
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = True
                    CrystalReportViewer1.ReportSource = Rpt
                ElseIf mySWReg = "Si" Then
                    Dim Rpt As New FatturatoClienteFatturaMargForReg
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = True
                    CrystalReportViewer1.ReportSource = Rpt
                Else
                    Dim Rpt As New FatturatoClienteFatturaMargFor
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = True
                    CrystalReportViewer1.ReportSource = Rpt
                End If
                
            ElseIf mySWFor.Trim = "S" Then 'GIU121221 SINTETICO
                Dim Rpt As New FatturatoClienteFatturaMargForS
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = True
                CrystalReportViewer1.ReportSource = Rpt
            ElseIf mySWAge.Trim = "Si" Then
                Dim Rpt As New FatturatoClienteFatturaMargAG
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = True
                CrystalReportViewer1.ReportSource = Rpt
            ElseIf mySWReg.Trim = "Si" Then
                Dim Rpt As New FatturatoClienteFatturaMargReg
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = True
                CrystalReportViewer1.ReportSource = Rpt
            Else
                Dim Rpt As New FatturatoClienteFatturaMarg
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = True
                CrystalReportViewer1.ReportSource = Rpt
            End If
            
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDoc Then
            Dim Rpt As New FatturatoOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDoc Then
            Dim Rpt As New FatturatoOrdineSortByDataDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDoc Then
            Dim Rpt As New FattSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DiffFTDTSintOrdineSortByNDoc Then
            Dim Rpt As New DiffFTDTSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu151012
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DTFTDoppiSintOrdineSortByNDoc Then
            Dim Rpt As New DTFTDoppiSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'alb18062012
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocAG Then
            Dim Rpt As New FatturatoClienteFatturaAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDocAG Then
            Dim Rpt As New FatturatoOrdineSortByNDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDocAG Then
            Dim Rpt As New FatturatoOrdineSortByDataDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocAG Then
            Dim Rpt As New FattSintOrdineSortByNDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            '----------------------------
            'alb19062012
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocReg Then
            Dim Rpt As New FattSintOrdineSortByNDocReg
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            '------------
            'GIU171012
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FTNCCCausErrSintOrdineSortByNDoc Then
            Dim Rpt As New FattSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA FATTURATO SCONOSCIUTA")
        End If
    End Sub

    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = ""

        If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDoc Or _
            Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Or _
            Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargMP Then
            Dim mySW As String = ""
            Try
                mySW = Session("FatturatoClientiMargineFor")
            Catch ex As Exception
                mySW = ""
            End Try
            If String.IsNullOrEmpty(mySW) Then
                strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per cliente/documento"
            ElseIf mySW.Trim = "Si" Or mySW.Trim = "S" Then
                strRitorno = "WF_FatturatoClientiMargineFor.aspx?labelForm=Fatturato Clienti per N°documento [Margine per Fornitore]"
            Else
                strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per cliente/documento"
            End If
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDoc Or _
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DiffFTDTSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Differenze Fatture/N.C. con DDT"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DTFTDoppiSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=DDT Fatturati in Fatture diverse"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per cliente/N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocReg Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FTNCCCausErrSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatture/N.C. con Codice Causale errata"
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