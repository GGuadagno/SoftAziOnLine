Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity

Imports It.SoftAzi.SystemFramework
'GIU230514
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
'---------

Partial Public Class WF_PrintWeb_AICA
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'GIU100324
        Try
            Dim strLabelForm As String = Request.QueryString("labelForm")
            If InStr(strLabelForm.Trim.ToUpper, "ESPORTA") > 0 Then
                'OK PROSEGUO
            Else
                VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
                Exit Sub
            End If
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
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                LnkRitorno.Visible = False
                'lblMessaggi.Visible = False
            End If
        End If
        '-
        Dim SWTipoStampa As Integer = -1
        If Not String.IsNullOrEmpty(Session(CSTTipoStampaAICA)) Then
            If IsNumeric(Session(CSTTipoStampaAICA)) Then
                SWTipoStampa = Session(CSTTipoStampaAICA)
            End If
        End If
        '-
        'GIU230514 Dim Rpt As Object = Nothing
        Dim Rpt As ReportClass
        '-------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            CodiceDitta = ""
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            CodiceDitta = ""
        End If
        '---------------------
        CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        CrystalReportViewer1.DisplayGroupTree = False
        If SWTipoStampa = TipoStampaAICA.SingoloAICA Then
            Rpt = New SchedaAICA
            'Uguale per tutte le eventuali aziende, diversamente cambiare il RPT per il CodiceDitta
            If CodiceDitta = "01" Then
                Rpt = New SchedaAICA '01
            ElseIf CodiceDitta = "05" Then
                Rpt = New SchedaAICA '05
            End If
            'giu090324
            Session("NomeRpt") = "SchedaAICA"
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICACliArtNSerie Then
            Rpt = New ElencoAICliArtNSerie
            Session("NomeRpt") = "ElencoAICliArtNSerie"
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAArtCliNSerie Then
            Rpt = New ElencoAIArtCliNSerie
            Session("NomeRpt") = "SchedaAICA"
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScGaArtCliNSerie Then
            Rpt = New ElencoAIScGaArtCliNSerie
            Session("NomeRpt") = "ElencoAIArtCliNSerie"
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScElArtCliNSerie Then
            Rpt = New ElencoAIScElArtCliNSerie
            Session("NomeRpt") = "ElencoAICAScElArtCliNSerie"
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScBaArtCliNSerie Then
            Rpt = New ElencoAIScBaArtCliNSerie
            Session("NomeRpt") = "ElencoAICAScBaArtCliNSerie"
        Else
            Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
            Exit Sub
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)
        CrystalReportViewer1.ReportSource = Rpt
        getOutputRPT(Rpt)
    End Sub

    Private Sub subRitorno()
        Dim strRitorno As String = "WF_ArticoliInst_ContrattiAssElenco.aspx?labelForm=Articoli consumabili Clienti"
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
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
    End Sub

    Private Function getOutputRPT(ByVal _Rpt As Object) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            '''If _Formato = ReportFormatEnum.Pdf Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            '''ElseIf _Formato = ReportFormatEnum.Excel Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.Excel)
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