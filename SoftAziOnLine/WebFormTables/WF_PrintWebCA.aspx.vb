Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity

Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebCA
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    'giu200423 non modificare mai Session(CSTTIPODOC) USATO IN GEST. DOC.
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.Title = Request.QueryString("labelForm")
        Catch ex As Exception
        End Try
    End Sub
   
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                btnRitorno.Visible = False
                Label1.Visible = False
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
        Dim DsDoc As New DSDocumenti
        Dim dsOrdArtCli As New DSOrdinatoArtCli
        If Session(CSTTASTOST) = "btnElencoSc" Or _
            Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivita Or _
            Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivitaPag Then

            DsDoc = Session(CSTDsPrinWebDoc)
        ElseIf Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
            dsOrdArtCli = Session(CSTDsPrinWebDoc)
        Else
            DsPrinWebDoc = Session(CSTDsPrinWebDoc)
            'GIU 160312
            Try
                If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                    'giu110319
                    Session("TipoDocInStampa") = DsPrinWebDoc.Tables("ContrattiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                    '---------
                End If
            Catch ex As Exception
            End Try
            'giu120319
            Try
                If (DsPrinWebDoc.Tables("ContrattiT").Select("RitAcconto=true").Count > 0) Then
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
                If (DsPrinWebDoc.Tables("ContrattiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                    SWSconti = 1
                    Session(CSTSWScontiDoc) = 1
                Else
                    SWSconti = 0
                    Session(CSTSWScontiDoc) = 0
                End If
            Catch ex As Exception
                'ok
            End Try
        End If
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
            'nessun controllo
        Else
            If CKCSTTipoDoc() = False Then
                Try
                    'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: sessione scaduta o stampa non prevista")
                    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: sessione scaduta o stampa non prevista.")
                    Exit Sub
                Catch ex As Exception
                    'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: sessione scaduta o stampa non prevista")
                    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: sessione scaduta o stampa non prevista.")
                    Exit Sub
                End Try
                Exit Sub
            End If
        End If
        '---------------------
        CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        CrystalReportViewer1.DisplayGroupTree = False
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        'giu200520 sapere chi mi ha chiamato per eventuali differenza stampa elenco se SCAD. ATT. o SCAD.PAG.ATT.
        If Session("TipoDocInStampa") = SWTD(TD.ContrattoAssistenza) Then
            If Session(CSTTASTOST) = "btnStampa" Or _
                Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Proforma Then
                Rpt = New ProformaCA05 'Contratti
                If CodiceDitta = "01" Then
                    Rpt = New ProformaCA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ProformaCA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ProformaCA05 '0501
                End If
            ElseIf Session(CSTTASTOST) = "btnVerbale" Or _
                Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Verbale Then
                Rpt = New VerbaleVACA05
                If CodiceDitta = "01" Then
                    Rpt = New VerbaleVACA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New VerbaleVACA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New VerbaleVACA05 '0501
                End If
            ElseIf Session(CSTTASTOST) = "btnElencoSc" Or _
                Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivitaPag Or _
                Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivita Then

                If Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivita Then
                    Rpt = New ElencoScadCA05
                    If CodiceDitta = "01" Then
                        Rpt = New ElencoScadCA05 '01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New ElencoScadCA05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New ElencoScadCA05 '0501
                    End If
                ElseIf Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivitaPag Then
                    Rpt = New ElencoScadPagCA05
                    If CodiceDitta = "01" Then
                        Rpt = New ElencoScadPagCA05 '01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New ElencoScadPagCA05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New ElencoScadPagCA05 '0501
                    End If
                ElseIf Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
                    Rpt = New StOrdArtCliRespVisite
                ElseIf Session("TipoDocInStampa") = SWTD(TD.TipoContratto) Then
                    Rpt = New ProformaCA05
                Else
                    Try
                        'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                        Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                        Exit Sub
                    Catch ex As Exception
                        'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                        Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                        Exit Sub
                    End Try
                    Exit Sub
                End If
            ElseIf Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
                Rpt = New StOrdArtCliRespVisite
            ElseIf Session("TipoDocInStampa") = SWTD(TD.TipoContratto) Then
                Rpt = New ProformaCA05
            Else
                Try
                    'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                    Exit Sub
                Catch ex As Exception
                    'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                    Exit Sub
                End Try
                Exit Sub
            End If
        ElseIf Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
            Rpt = New StOrdArtCliRespVisite
        ElseIf Session("TipoDocInStampa") = SWTD(TD.TipoContratto) Then
            Rpt = New ProformaCA05
        Else
            Try
                'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                Exit Sub
            Catch ex As Exception
                'Response.Redirect("..\Login.aspx?SessioneScaduta=Errore in esecuzione stampa Contratti: stampa non prevista")
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore in esecuzione stampa Contratti: stampa non prevista.")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        Try
            If Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis Then
                Rpt.SetDataSource(dsOrdArtCli)
            ElseIf Session(CSTTASTOST) = "btnElencoSc" Then
                Rpt.SetDataSource(DsDoc)
            Else
                Rpt.SetDataSource(DsPrinWebDoc)
            End If
            '-
        Catch ex As Exception
            Rpt.SetDataSource(DsPrinWebDoc)
        End Try
        
        CrystalReportViewer1.ReportSource = Rpt
    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = ""
        Try
            strRitorno = "WF_ContrattiElenco.aspx?labelForm=Elenco CONTRATTI"
            If Not String.IsNullOrEmpty(Session(CSTChiamatoDa)) Then
                strRitorno = Session(CSTChiamatoDa)
            End If
            If strRitorno.Trim = "" Then
                strRitorno = "WF_ContrattiElenco.aspx?labelForm=Elenco CONTRATTI"
            End If
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

End Class