Imports SoftAziOnLine.Def

Partial Public Class WF_PrintWebCR_Clienti
    Inherits System.Web.UI.Page
    Dim CliFor As String
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                btnRitorno.Visible = False
                Label1.Visible = False
            End If
        End If
        '-
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
                    Case 2
                        Select Case Dettagli
                            Case 0
                                rpt = New rptCliRub
                            Case 1
                                rpt = New rptCliRubContatti
                            Case 2
                                rpt = New rptCliRubDest
                            Case 3
                                rpt = New rptCliRubContattiDest
                        End Select
                    Case 3
                        rpt = New rptCliAnalit
                    Case 4
                        rpt = New rptCliCod
                    Case 5
                        rpt = New ECC
                    Case 6
                        rpt = New SchedaContA4
                End Select
            ElseIf CliFor = "Fornitori" Then
                Select Case TipoStampa
                    Case 1
                        rpt = New rptForSint
                    Case 2
                        Select Case Dettagli
                            Case 0
                                rpt = New rptForRub
                            Case 1
                                rpt = New rptForRubContatti
                            Case 2
                                rpt = New rptForRubDest
                            Case 3
                                rpt = New rptForRubContattiDest
                        End Select
                    Case 3
                        rpt = New rptForAnalit
                    Case 4
                        rpt = New rptForCod
                End Select
            Else
                Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
            End If

            If TipoStampa = 7 Then
                rpt = New rptCFPivaDoppi
                btnRitorno.Visible = False
            End If

            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.ReportSource = rpt
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

    Protected Sub btnRitorno_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRitorno.Click
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
End Class