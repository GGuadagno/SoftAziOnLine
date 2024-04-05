Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework
Imports CrystalDecisions.Shared
Imports System.IO

Partial Public Class WF_PrintWebControllo
    Inherits System.Web.UI.Page
    Private Sub WF_PrintWebControllo_Load(sender As Object, e As EventArgs) Handles Me.Load
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
        If Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffPrezzoListino Then
            Dim Rpt As New DiffPrezzoListinoPerArt
            Dim DSDiffPrezzoListino1 As New DSControlli
            DSDiffPrezzoListino1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DSDiffPrezzoListino1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "DiffPrezzoListino"
            getOutputRPT(Rpt)
        ElseIf Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffImportoRiga Then
            Dim Rpt As New DiffImportoRigaPerArt
            Dim DSDiffImportoRiga1 As New DSControlli
            DSDiffImportoRiga1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DSDiffImportoRiga1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "DiffImportoRiga"
            getOutputRPT(Rpt)
        ElseIf Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.CKSerieLotto Then
            Dim Rpt As New ContrattiCKSerieLotto
            Dim DSCKSerieLotto1 As New DSPrintWeb_Documenti
            DSCKSerieLotto1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DSCKSerieLotto1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "CKSerieLotto"
            getOutputRPT(Rpt)
        Else
            Chiudi("Errore: TIPO STAMPA CONTROLLO SCONOSCIUTA")
        End If
        If IsNothing(Session("StampaMovMag")) Then
            lblVuota.Visible = True
        End If

    End Sub


    Private Sub subRitorno()
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