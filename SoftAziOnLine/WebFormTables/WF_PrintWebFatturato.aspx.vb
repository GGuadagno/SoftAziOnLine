Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework
Imports CrystalDecisions.Shared
Imports System.IO

Partial Public Class WF_PrintWebFatturato
    Inherits System.Web.UI.Page
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private Sub WF_PrintWebFatturato_Load(sender As Object, e As EventArgs) Handles Me.Load
        'GIU100324
        Try
            '''Dim strLabelForm As String = Request.QueryString("labelForm")
            '''If InStr(strLabelForm.Trim.ToUpper, "ESPORTA") > 0 Then
            '''    'OK PROSEGUO
            '''Else
            '''    VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
            '''    Exit Sub
            '''End If
            '-NON va bene per il NOBACK 
            '''If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            '''    If Session(CSTNOBACK) = 1 Then
            '''        LnkRitorno.Visible = False
            '''        VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
            '''        Exit Sub
            '''    End If
            '''End If
        Catch ex As Exception
        End Try
        '-
        If IsPostBack Then
            If Request.Params.Get("__EVENTTARGET").ToString = "LnkStampaOK" Then
                'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
                VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
                Exit Sub
            End If
            If Request.Params.Get("__EVENTTARGET").ToString = "LnkRitornoOK" Then
                'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
                subRitorno()
                Exit Sub
            End If
        End If
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDoc Then
            Dim Rpt As New FatturatoClienteFattura
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortClienteNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Or
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
                CrystalReportViewer1.DisplayGroupTree = False
                CrystalReportViewer1.ReportSource = Rpt
                'giu090324
                Session("NomeRpt") = "FatturatoClienteFatturaMarg"
                getOutputRPT(Rpt)
            ElseIf mySWFor.Trim = "Si" Then
                If mySWAge = "Si" Then
                    Dim Rpt As New FatturatoClienteFatturaMargForAG
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = False
                    CrystalReportViewer1.ReportSource = Rpt
                    'giu090324
                    Session("NomeRpt") = "FatturatoClienteFatturaMargForAG"
                    getOutputRPT(Rpt)
                ElseIf mySWReg = "Si" Then
                    Dim Rpt As New FatturatoClienteFatturaMargForReg
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = False
                    CrystalReportViewer1.ReportSource = Rpt
                    'giu090324
                    Session("NomeRpt") = "FatturatoClienteFatturaMargForReg"
                    getOutputRPT(Rpt)
                Else
                    Dim Rpt As New FatturatoClienteFatturaMargFor
                    Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                    dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                    CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                    Rpt.SetDataSource(dsFatturatoClienteFattura1)
                    CrystalReportViewer1.DisplayGroupTree = False
                    CrystalReportViewer1.ReportSource = Rpt
                    'giu090324
                    Session("NomeRpt") = "FatturatoClienteFatturaMargFor"
                    getOutputRPT(Rpt)
                End If

            ElseIf mySWFor.Trim = "S" Then 'GIU121221 SINTETICO
                Dim Rpt As New FatturatoClienteFatturaMargForS
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = False
                CrystalReportViewer1.ReportSource = Rpt
                'giu090324
                Session("NomeRpt") = "FatturatoClienteFatturaMargForS"
                getOutputRPT(Rpt)
            ElseIf mySWAge.Trim = "Si" Then
                Dim Rpt As New FatturatoClienteFatturaMargAG
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = False
                CrystalReportViewer1.ReportSource = Rpt
                'giu090324
                Session("NomeRpt") = "FatturatoClienteFatturaMargAG"
                getOutputRPT(Rpt)
            ElseIf mySWReg.Trim = "Si" Then
                Dim Rpt As New FatturatoClienteFatturaMargReg
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = False
                CrystalReportViewer1.ReportSource = Rpt
                'giu090324
                Session("NomeRpt") = "FatturatoClienteFatturaMargReg"
                getOutputRPT(Rpt)
            Else
                Dim Rpt As New FatturatoClienteFatturaMarg
                Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
                dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
                CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(dsFatturatoClienteFattura1)
                CrystalReportViewer1.DisplayGroupTree = False
                CrystalReportViewer1.ReportSource = Rpt
                'giu090324
                Session("NomeRpt") = "FatturatoClienteFatturaMarg"
                getOutputRPT(Rpt)
            End If

        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDoc Then
            Dim Rpt As New FatturatoOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortByNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDoc Then
            Dim Rpt As New FatturatoOrdineSortByDataDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortByDataDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDoc Then
            Dim Rpt As New FattSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattSintOrdineSortByNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DiffFTDTSintOrdineSortByNDoc Then
            Dim Rpt As New DiffFTDTSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "DiffFTDTSintOrdineSortByNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DTFTDoppiSintOrdineSortByNDoc Then
            Dim Rpt As New DTFTDoppiSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "DTFTDoppiSintOrdineSortByNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocAG Then
            Dim Rpt As New FatturatoClienteFatturaAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortClienteNDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDocAG Then
            Dim Rpt As New FatturatoOrdineSortByNDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortByNDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDocAG Then
            Dim Rpt As New FatturatoOrdineSortByDataDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattOrdineSortByDataDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocAG Then
            Dim Rpt As New FattSintOrdineSortByNDocAG
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattSintOrdineSortByNDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocReg Then
            Dim Rpt As New FattSintOrdineSortByNDocReg
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FattSintOrdineSortByNDocReg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FTNCCCausErrSintOrdineSortByNDoc Then
            Dim Rpt As New FattSintOrdineSortByNDoc
            Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
            dsFatturatoClienteFattura1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFatturatoClienteFattura1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "FTNCCCausErrSintOrdineSortByNDoc"
            getOutputRPT(Rpt)
        Else
            Chiudi("Errore: TIPO STAMPA FATTURATO SCONOSCIUTA")
        End If
        If IsNothing(Session("StampaMovMag")) Then
            lblVuota.Visible = True
        End If
    End Sub

    Private Sub subRitorno()
        Dim strRitorno As String = ""

        If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDoc Or
            Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Or
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
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDoc Or
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

    Private Function getOutputRPT(ByVal _Rpt As Object) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            '''If _Formato = ReportFormatEnum.Pdf Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            '''ElseIf _Formato = ReportFormatEnum.Excel Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.ExcelRecord)
            '''End If
            myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("StampaMovMag") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function

    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    Private Sub VisualizzaRpt(ByVal byteReport() As Byte, ByVal _NomeRpt As String)
        Dim sErrore As String = ""
        Try
            If byteReport.Length > 0 Then
                With Me.Page
                    Response.Clear()
                    Response.Buffer = True
                    Response.ClearHeaders()

                    Response.AddHeader("Accept-Header", byteReport.Length.ToString())
                    Response.AddHeader("Cache-Control", "private")
                    Response.AddHeader("cache-control", "max-age=1")
                    Response.AddHeader("content-length", byteReport.Length.ToString())
                    Response.AppendHeader("content-disposition", "inline; filename=" & "" & _NomeRpt & ".pdf")
                    'Response.AppendHeader("content-disposition", "attachment; filename=" & "RicevutaAcquisto_" & sCodiceTransazione & ".pdf")      ' per download diretto
                    Response.AddHeader("Expires", "0")
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Accept-Ranges", "bytes")

                    Response.BinaryWrite(byteReport)
                    Response.Flush()
                    Response.End()
                End With
            Else
                lblVuota.Visible = True
            End If
        Catch ex As Exception
            lblVuota.Visible = True
        End Try
    End Sub
End Class