Imports CrystalDecisions.Shared
Imports System.IO
Imports SoftAziOnLine.Def

Partial Public Class WF_PrintWebCR_Clienti
    Inherits System.Web.UI.Page
    Dim CliFor As String
    Private Sub WF_PrintWebCR_Clienti_Load(sender As Object, e As EventArgs) Handles Me.Load
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
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                LnkRitorno.Visible = False
                'Label1.Visible = False
            End If
        End If
        '-
        Session("NomeRpt") = "Stampa"
        CliFor = Session(CSTFinestraChiamante)
        Try
            Dim TipoStampa As Integer = Session("TipoStampa")
            Dim Dettagli As Integer = Session("DettagliStampa")
            Dim dsReport As New dsClienti
            Dim rpt As Object = Nothing

            dsReport = Session("dsStampa")

            If CliFor = "Clienti" Then
                Select Case TipoStampa
                    Case 1
                        rpt = New rptCliSint
                        Session("NomeRpt") = "CliSint"
                    Case 2
                        Select Case Dettagli
                            Case 0
                                rpt = New rptCliRub
                                Session("NomeRpt") = "CliRub"
                            Case 1
                                rpt = New rptCliRubContatti
                                Session("NomeRpt") = "CliRubContatti"
                            Case 2
                                rpt = New rptCliRubDest
                                Session("NomeRpt") = "CliRubDest"
                            Case 3
                                rpt = New rptCliRubContattiDest
                                Session("NomeRpt") = "CliRubContattiDest"
                        End Select
                    Case 3
                        rpt = New rptCliAnalit
                        Session("NomeRpt") = "CliAnalit"
                    Case 4
                        rpt = New rptCliCod
                        Session("NomeRpt") = "CliCod"
                    Case 5
                        rpt = New ECC
                        Session("NomeRpt") = "ECC"
                    Case 6
                        rpt = New SchedaContA4
                        Session("NomeRpt") = "SchedaContA4"
                End Select
            ElseIf CliFor = "Fornitori" Then
                Select Case TipoStampa
                    Case 1
                        rpt = New rptForSint
                        Session("NomeRpt") = "ForSint"
                    Case 2
                        Select Case Dettagli
                            Case 0
                                rpt = New rptForRub
                                Session("NomeRpt") = "ForRub"
                            Case 1
                                rpt = New rptForRubContatti
                                Session("NomeRpt") = "ForRubContatti"
                            Case 2
                                rpt = New rptForRubDest
                                Session("NomeRpt") = "ForRubDest"
                            Case 3
                                rpt = New rptForRubContattiDest
                                Session("NomeRpt") = "ForRubContattiDest"
                        End Select
                    Case 3
                        rpt = New rptForAnalit
                        Session("NomeRpt") = "ForAnalit"
                    Case 4
                        rpt = New rptForCod
                        Session("NomeRpt") = "ForCod"
                End Select
            Else
                Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
            End If

            If TipoStampa = 7 Then
                rpt = New rptCFPivaDoppi
                Session("NomeRpt") = "CFPivaDoppi"
                LnkRitorno.Visible = False
            End If

            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            'assegnato sopra  Session("NomeRpt") = "Stampa"
            getOutputRPT(rpt)
            If IsNothing(Session("StampaMovMag")) Then
                lblVuota.Visible = True
            End If
        Catch ex As Exception
            If CliFor = "Clienti" Then
                Response.Redirect("WF_AnagraficaClienti.aspx?labelForm=Errore in stampa Anagrafica clienti: " & ex.Message)
            Else
                Response.Redirect("WF_AnagraficaFornitori.aspx?labelForm=Errore in stampa Anagrafica fornitori: " & ex.Message)
            End If
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

    Protected Sub subRitorno()
        Dim TipoStampa As Integer = Session("TipoStampa")
        Try
            If CliFor = "Clienti" Then
                'Response.Redirect("WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
                If TipoStampa <> 5 And TipoStampa <> 6 Then
                    Response.Redirect("WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica clienti")
                Else
                    Response.Redirect("WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
                End If
                Exit Sub
            ElseIf CliFor = "Fornitori" Then
                'Response.Redirect("WF_AnagraficaFornitori.aspx?labelForm=Anagrafica fornitori")
                Response.Redirect("WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica fornitori")
                Exit Sub
            Else
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            End If
        Catch ex As Exception
            If CliFor = "Clienti" Then
                'Response.Redirect("WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
                If TipoStampa <> 5 And TipoStampa <> 6 Then
                    Response.Redirect("WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica clienti")
                Else
                    Response.Redirect("WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
                End If
            Else
                'Response.Redirect("WF_AnagraficaFornitori.aspx?labelForm=Anagrafica fornitori")
                Response.Redirect("WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica fornitori")
            End If
            Exit Sub
        End Try
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