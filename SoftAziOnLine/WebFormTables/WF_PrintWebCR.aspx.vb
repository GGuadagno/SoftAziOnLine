Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity

Imports It.SoftAzi.SystemFramework
Imports CrystalDecisions.Shared
Imports System.IO

Partial Public Class WF_PrintWebCR
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private Sub WF_PrintWebCR_Load(sender As Object, e As EventArgs) Handles Me.Load
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
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                LnkRitorno.Visible = False
                'Label1.Visible = False
            End If
        End If
        Dim SWSconti As Integer = 1
        If Not String.IsNullOrEmpty(Session(CSTSWScontiDoc)) Then
            If IsNumeric(Session(CSTSWScontiDoc)) Then
                SWSconti = Session(CSTSWScontiDoc)
            End If
        End If
        '-
        Dim SWConfermaDoc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWConfermaDoc)) Then
            If IsNumeric(Session(CSTSWConfermaDoc)) Then
                SWConfermaDoc = Session(CSTSWConfermaDoc)
            End If
        End If
        'giu110319
        Dim SWRitAcc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWRitAcc)) Then
            If IsNumeric(Session(CSTSWRitAcc)) Then
                SWRitAcc = Session(CSTSWRitAcc)
            End If
        End If
        '---------
        Dim Rpt As Object = Nothing
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'giu110319 giu200423 NON TOTACE MAI CSTTIPODOC usatoin gest.Doc,
                Session("TipoDocInStampa") = DsPrinWebDoc.Tables("DocumentiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
        End Try
        'giu120319
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Select("RitAcconto=true").Count > 0) Then
                Session(CSTSWRitAcc) = 1
                SWRitAcc = 1
            Else
                Session(CSTSWRitAcc) = 0
                SWRitAcc = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("DocumentiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDoc() = False Then
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        '---------------------
        CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        CrystalReportViewer1.DisplayGroupTree = False
        'giu090324
        Session("NomeRpt") = "Stampa"
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        If Session("TipoDocInStampa") = SWTD(TD.Preventivi) Then
            If SWSconti = 1 Then
                Rpt = New Preventivo
                If CodiceDitta = "01" Then
                    Rpt = New Preventivo01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Preventivo05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Preventivo0501
                End If
            Else
                Rpt = New PreventivoNOSconti
                If CodiceDitta = "01" Then
                    Rpt = New PreventivoNOSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New PreventivoNOSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New PreventivoNOSconti0501
                End If
            End If
            Session("NomeRpt") = "Preventivo"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.OrdClienti) Then
            If SWSconti = 1 Then
                Rpt = New Ordine
                If CodiceDitta = "01" Then
                    Rpt = New Ordine01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Ordine05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Ordine0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New Ordine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New Ordine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New Ordine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New Ordine0501
                '''    End If
                '''Else
                '''    Rpt = New ConfermaOrdine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdine0501
                '''    End If
                '''End If
            Else
                Rpt = New OrdineNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New OrdineNoSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New OrdineNoSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New OrdineNoSconti0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New OrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New OrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New OrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New OrdineNoSconti0501
                '''    End If
                '''Else
                '''    Rpt = New ConfermaOrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdineNoSconti0501
                '''    End If
                '''End If
                Session("NomeRpt") = "Ordine"
            End If
        ElseIf Session("TipoDocInStampa") = SWTD(TD.DocTrasportoClienti) Then
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New DDTNoPrezzi05
                Else
                    Rpt = New DDTNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
            Session("NomeRpt") = "DDT"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.DocTrasportoFornitori) Then
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New DDTNoPrezzi05
                Else
                    Rpt = New DDTNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
            Session("NomeRpt") = "DDT"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.FatturaCommerciale) Then
            'giu251211
            If SWSconti = 1 Then
                Rpt = New Fattura
                If CodiceDitta = "01" Then
                    Rpt = New Fattura01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New Fattura05
                    Else
                        Rpt = New Fattura05LT
                    End If
                    '-
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Fattura0501
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                End If
                Session("NomeRpt") = "Fattura"
            Else
                Rpt = New FatturaNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New FatturaNoSconti01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New FatturaNoSconti05
                    Else
                        Rpt = New FatturaNoSconti05LT
                    End If
                    '-
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New FatturaNoSconti0501
                    If SWRitAcc = True <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                End If
            End If
            Session("NomeRpt") = "Fattura"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.NotaCredito) Then
            Rpt = New NotaCredito
            If CodiceDitta = "01" Then
                Rpt = New NotaCredito01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New NotaCredito05
                Else
                    Rpt = New NotaCredito05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New NotaCredito0501
            End If
            Session("NomeRpt") = "NC"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.OrdFornitori) Then
            Rpt = New OrdineFornitore
            If CodiceDitta = "01" Then
                Rpt = New OrdineFornitore01
            ElseIf CodiceDitta = "05" Then
                Rpt = New OrdineFornitore05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New OrdineFornitore0501
            End If
            Session("NomeRpt") = "OrdineFOR"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.MovimentoMagazzino) Or _
                Session("TipoDocInStampa") = SWTD(TD.CaricoMagazzino) Or _
                Session("TipoDocInStampa") = SWTD(TD.ScaricoMagazzino) Then
            Rpt = New MMNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New MMNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New MMNoPrezzi05
                Else
                    Rpt = New MMNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New MMNoPrezzi0501
            End If
            Session("NomeRpt") = "MovMag"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.BuonoConsegna) Or _
            Session("TipoDocInStampa") = SWTD(TD.DocTrasportoCLavoro) Or _
            Session("TipoDocInStampa") = SWTD(TD.FatturaAccompagnatoria) Or _
            Session("TipoDocInStampa") = SWTD(TD.FatturaScontrino) Or _
            Session("TipoDocInStampa") = SWTD(TD.NotaCorrispondenza) Then
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale STAMPA DOCUMENTO DA COMPLETARE"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        Else
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)
        CrystalReportViewer1.ReportSource = Rpt
        getOutputRPT(Rpt)
    End Sub

    Private Sub subRitorno()
        Dim strRitorno As String = ""
        'giu150312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Try
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            End Try
            Exit Sub
        End If
        '-----------
        If CKCSTTipoDoc() = False Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
            Exit Sub
        End If
        Session(CSTSWRbtnTD) = Session("TipoDocInStampa")
        If Session("TipoDocInStampa") = SWTD(TD.Preventivi) Then
            strRitorno = "WF_ElencoPreventivi.aspx?labelForm=Gestione preventivi/offerte"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.OrdClienti) Then
            strRitorno = "WF_ElencoOrdini.aspx?labelForm=Gestione ordini CLIENTI"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.OrdFornitori) Then
            strRitorno = "WF_ElencoOrdiniFornitori.aspx?labelForm=Gestione ordini FORNITORI"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.DocTrasportoFornitori) Then
            strRitorno = "WF_DocumentiElencoFor.aspx?labelForm=Gestione documenti"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.MovimentoMagazzino) Or
            Session("TipoDocInStampa") = SWTD(TD.CaricoMagazzino) Or
            Session("TipoDocInStampa") = SWTD(TD.ScaricoMagazzino) Then
            strRitorno = "WF_ElencoMovMag.aspx?labelForm=Gestione movimenti di magazzino"
        ElseIf Session("TipoDocInStampa") = SWTD(TD.DocTrasportoClienti) Or
            Session("TipoDocInStampa") = SWTD(TD.BuonoConsegna) Or
            Session("TipoDocInStampa") = SWTD(TD.DocTrasportoCLavoro) Or
            Session("TipoDocInStampa") = SWTD(TD.FatturaAccompagnatoria) Or
            Session("TipoDocInStampa") = SWTD(TD.FatturaCommerciale) Or
            Session("TipoDocInStampa") = SWTD(TD.FatturaScontrino) Or
            Session("TipoDocInStampa") = SWTD(TD.NotaCorrispondenza) Or
            Session("TipoDocInStampa") = SWTD(TD.NotaCredito) Then
            strRitorno = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
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
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = Session("TipoDocInStampa")
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            Return False
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Return False
        End If
        If CodiceDitta = "" Then
            Return False
        End If
        '-------------------------------------------------------------------
    End Function

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