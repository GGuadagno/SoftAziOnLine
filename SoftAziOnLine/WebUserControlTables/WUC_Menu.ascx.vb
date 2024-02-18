Imports SoftAziOnLine.WebFormUtility
Imports It.SoftAzi.Model.Entity
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App

Partial Public Class WUC_Menu
    Inherits System.Web.UI.UserControl
    'Al momento non usato ma potrebbe servire per cambiare le visualizzazioni documenti nel menu dei documenti 
    Private Enum TipoSWWhere
        Scaduti = 1
        RD = 2
        RiBa = 3
        Bonifici = 4
        '---
        LimitePresRiBa = 5
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        lblDataOdierna.Text = "Oggi, " & Format(Now, "dddd d MMMM yyyy, HH:mm")
        ModalPopup.WucElement = Me
        'giu080312 viene fatto sempre dal MASTERPAGE
        'giu080312
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
        '-----------
        'giu080620
        If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Or _
               (sTipoUtente.Equals(CSTTECNICO)) Or _
               (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
            LnkAlert.Visible = True
        Else
            LnkAlert.Visible = False
        End If
        'giu130320
        If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Or _
               (sTipoUtente.Equals(CSTTECNICO)) Or _
               (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
            'GIU200412
            Dim strErrore As String = "" : Dim strValore As String = ""
            If String.IsNullOrEmpty(Session("NGGDistRiBa")) Then
                If App.GetDatiAbilitazioni(CSTABILCOGE, "NGGDistRiBa", strValore, strErrore) = True Then
                    Session("NGGDistRiBa") = strValore
                Else
                    Session("NGGDistRiBa") = SWOPNESSUNA
                End If
            End If
            If String.IsNullOrEmpty(Session("NGGCAScadPag")) Then
                If App.GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadPag", strValore, strErrore) = True Then
                    Session("NGGCAScadPag") = strValore
                Else
                    Session("NGGCAScadPag") = SWOPNESSUNA
                End If
            End If
            If String.IsNullOrEmpty(Session("NGGCAScadFin")) Then
                If App.GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadFin", strValore, strErrore) = True Then
                    Session("NGGCAScadFin") = strValore
                Else
                    Session("NGGCAScadFin") = SWOPNESSUNA
                End If
            End If
            If String.IsNullOrEmpty(Session("NGGCAScadAtt")) Then
                If App.GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadAtt", strValore, strErrore) = True Then
                    Session("NGGCAScadAtt") = strValore
                Else
                    Session("NGGCAScadAtt") = SWOPNESSUNA
                End If
            End If
            '-
        End If
        '---------
        If (Not IsPostBack) Then

            lblUltimoAccesso.Text = ""
            lblUtente.Text = Session(CSTUTENTE) 'giu080312 Utente.NomeOperatore
            lblTipoUtente.Text = sTipoUtente 'giu080312 Utente.Tipo

            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            'giu100312
            If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Or _
               (sTipoUtente.Equals(CSTTECNICO)) Or _
               (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
                ControllaSalvaDB()
                '---------
                If GetDDTByDataScadPag() = False Then
                    Session("GetDDTByDataScadPag") = lblDDT1.Text
                End If
                GetNDocByDataScadPagINCoGeRB()
                If GetNDocByDataScadPag() = False Then
                    Session("GetNDocByDataScadPag") = lblDoc10.Text
                End If
                If GetScadProdCons() = False Then
                    Session("GetScadProdCons") = lblScProdCons.Text.Trim
                End If
                If GetCAByDataScadPag() = False Then
                    Session("GetCAByDataScadPag") = lblSCContr1.Text
                End If
                If GetCAByDataScadAtt() = False Then
                    Session("GetCAByDataScadAtt") = lblSCMan1.Text
                End If
                '-
                Dim ClsDB As New DataBaseUtility
                Dim myStr As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))

                lblMessaggiMenu2.Text = "Invio E-Mail: " & myStr.Trim
                myStr = ClsDB.GetLogBKAuto(Session(ESERCIZIO))
                lblMessaggiMenu1.Text = "Salvataggio: " & myStr.Trim
                '-
                ClsDB = Nothing
            Else
                lblDDT.Text = ""
                lblDDT1.Text = "" : lblDDT2.Text = "" : lblDDT3.Text = "" : lblDDT4.Text = ""
                lblDDT5.Text = "" : lblDDT6.Text = "" : lblDDT7.Text = "" : lblDDT8.Text = ""
                'giu290119
                lblSCMan.Text = ""
                lblSCMan1.Text = "" : lblSCMan2.Text = "" : lblSCMan3.Text = "" : lblSCMan4.Text = ""
                lblSCMan5.Text = "" : lblSCMan6.Text = "" : lblScMan7.Text = "" : lblScMan8.Text = ""
                lblSCContr.Text = ""
                lblSCContr1.Text = "" : lblSCContr2.Text = "" : lblSCContr3.Text = "" : lblSCContr4.Text = ""
                lblSCContr5.Text = "" : lblSCContr6.Text = "" : lblSCContr7.Text = "" : lblSCContr8.Text = "" : lblSCContr9.Text = ""
                '-
                lblDDT1.Text = "" : lblDDT2.Text = "" : lblDDT3.Text = "" : lblDDT4.Text = ""
                lblDDT5.Text = "" : lblDDT6.Text = "" : lblDDT7.Text = "" : lblDDT8.Text = ""
                If GetNDocByDC() = False Then
                    Session("GetNDocByDC") = lblDoc1.Text.Trim
                End If
                lblElencoDocCoGe.Text = ""
                lblDoc10.Text = "" : lblDoc11.Text = "" : lblDoc12.Text = ""
                lblDoc13.Text = "" : lblDoc14.Text = "" : lblDoc15.Text = ""
                lblDoc16.Text = "" : lblDoc17.Text = "" : lblDoc18.Text = ""
                lblDoc10.ToolTip = "" : lblDoc11.ToolTip = "" : lblDoc12.ToolTip = ""
                lblDoc13.ToolTip = "" : lblDoc14.ToolTip = "" : lblDoc15.ToolTip = ""
                lblDoc16.ToolTip = "" : lblDoc17.ToolTip = "" : lblDoc18.ToolTip = ""
                '-
                lblMessaggiMenu1.Text = ""
                lblMessaggiMenu2.Text = ""
            End If
            GetOpConnessi()
            'GIU110512 Controllo se ci sono dei documenti di tipo CM in stato '5' CARICO IN CORSO
            ControllaDocOFStato5()
            ControllaDocStato9() '161220QUALSIASI TIPO DOCUMENTO PER ERRORI TIPO QTA' EVASA PIU' DELLA QTA' ORDINATA QUINDI DOPPIA FATTURAZIONE
            'giu
        Else
            If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Or _
               (sTipoUtente.Equals(CSTTECNICO)) Or _
               (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
                '-
                If GetScadProdCons() = False Then
                    Session("GetScadProdCons") = lblScProdCons.Text.Trim
                End If
                '-
                Dim ClsDB As New DataBaseUtility
                Dim myStr As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))

                lblMessaggiMenu2.Text = "Invio E-Mail: " & myStr.Trim
                myStr = ClsDB.GetLogBKAuto(Session(ESERCIZIO))
                lblMessaggiMenu1.Text = "Salvataggio: " & myStr.Trim
                '-
                ClsDB = Nothing
            Else
                lblMessaggiMenu1.Text = ""
                lblMessaggiMenu2.Text = ""
            End If
        End If
    End Sub
    'GIU110512 Controllo se ci sono dei documenti di tipo CM in stato '5' CARICO IN CORSO
    Private Function ControllaDocOFStato5() As Boolean
        lblMessBenvenuti.ForeColor = Drawing.Color.Black
        lblMessBenvenuti.ToolTip = ""
        ControllaDocOFStato5 = True
        Dim strSQL As String = ""
        strSQL = "SELECT  COUNT(IDDocumenti) AS TotDocStato5 FROM DocumentiT WHERE (Tipo_Doc = 'OF') AND  (StatoDoc = 5)"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("TotDocStato5")) Then
                        If ds.Tables(0).Rows(0).Item("TotDocStato5") <> 0 Then
                            lblMessBenvenuti.ForeColor = Drawing.Color.DarkRed
                            lblMessBenvenuti.Text = "N° Ordini Fornitori in carico: [" & Formatta.FormattaNumero(ds.Tables(0).Rows(0).Item("TotDocStato5")) & "]"
                            lblMessBenvenuti.ToolTip = "Nel caso nessuno sta eseguendo il carico, andare in Movimenti di magazzino:Carichi da confermare e Modifica/Aggiorna"
                        End If
                    End If
                End If
            End If
        Catch Ex As Exception
            ControllaDocOFStato5 = False
            lblMessBenvenuti.ForeColor = Drawing.Color.DarkRed
            lblMessBenvenuti.Text = "Errore controllo Ordini Fornitori in Carico"
            Exit Function
        End Try
    End Function
    Private Function ControllaSalvaDB() As Boolean 'GIU040412
        ControllaSalvaDB = True
        LnkSalvaDataBase.Visible = False
        lblMessBenvenuti.ForeColor = Drawing.Color.Black
        lblMessBenvenuti.Text = "Benvenuti su Soft Azienda OnLine"
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Exit Function
        End If
        LnkSalvaDataBase.BackColor = Drawing.Color.DarkRed
        LnkSalvaDataBase.ForeColor = Drawing.Color.White
        Dim strSQL As String = ""
        strSQL = "SELECT * FROM Esercizi WHERE Ditta = '" & Session(CSTCODDITTA) & "' AND Esercizio = '" & Session(ESERCIZIO) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataUltimoBK")) Then
                        If DateDiff(DateInterval.Day, ds.Tables(0).Rows(0).Item("DataUltimoBK"), Now) > 1 Then
                            LnkSalvaDataBase.Visible = True
                            lblMessBenvenuti.Text = "Richiesto il salvataggio [Eseguito il: " & Format(ds.Tables(0).Rows(0).Item("DataUltimoBK"), FormatoDataOra) & "]"
                        End If
                    Else
                        LnkSalvaDataBase.Visible = True
                        lblMessBenvenuti.Text = "Richiesto il salvataggio [Mai eseguito]"
                    End If
                End If
            End If
        Catch Ex As Exception
            ControllaSalvaDB = False
            LnkSalvaDataBase.Visible = True
            lblMessBenvenuti.Text = "Errore controllo salvataggio archivi: " + Ex.Message
            Exit Function
        End Try
    End Function
    Private Sub LnkSalvaDataBase_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkSalvaDataBase.Click
        '--------------------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = "SalvaDB"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        If lblUtConn2.Text.Trim = "" Then
            ModalPopup.Show("Salvataggio DataBase", "Confermi il salvataggio del DataBase ?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            ModalPopup.Show("Salvataggio DataBase", "Confermi il salvataggio del DataBase ? <br><strong><span> " & _
                            "ATTENZIONE, sono collegati altri utenti!!!</span></strong> <br>" & _
                            "si consiglia di effettuare il salvataggio senza altri utenti collegati.", WUC_ModalPopup.TYPE_CONFIRM)
        End If
        
    End Sub
    Public Sub SalvaDB()
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Salvataggio DataBase", "Attenzione, Sessione scaduta: Codice ditta/Esercizio non validi.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        LnkSalvaDataBase.Text = "Backup ..."
        lblMessBenvenuti.Text = "Salvataggio del DataBase in corso ......."

        Dim strSQL As String = ""
        strSQL = "SELECT * FROM Esercizi WHERE Ditta = '" & myCodDitta & "' AND Esercizio = '" & myEsercizio & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataUltimoBK")) Then
                        If ds.Tables(0).Rows(0).Item("DataUltimoBK").date <> Now.Date Then
                            'ok procedi
                        Else
                            LnkSalvaDataBase.Visible = False
                            lblMessBenvenuti.Text = "Benvenuti su Soft Azienda OnLine"
                            Session(MODALPOPUP_CALLBACK_METHOD) = "OKSalvaDB"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Salvataggio DataBase", "Salvataggio del DataBase già eseguito in data odierna. <br><br> Vuole procedere ugualmente ?", WUC_ModalPopup.TYPE_CONFIRM)
                            Exit Sub
                        End If
                    Else
                        'ok procedi
                    End If
                Else
                    'ok procedi
                End If
            Else
                'ok procedi
            End If
        Catch Ex As Exception
            ChiudiErrore("Errore Salvataggio DataBase: " & Ex.Message.Trim)
            Exit Sub
        End Try
        'OK PROCEDO
        OKSalvaDB() 'giu130412

    End Sub
    Public Sub OKSalvaDB() 'giu130412
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            ModalPopup.Show("Salavataggio DataBase", "Attenzione, Sessione scaduta: Codice ditta/Esercizio non validi.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        'OK PROCEDO
        lblMessBenvenuti.ForeColor = Drawing.Color.Black
        Dim strErrore As String = ""
        If SessionUtility.UpdDataUltimoBK(myCodDitta, myEsercizio, strErrore) = False Then
            LnkSalvaDataBase.Text = "ERRORE"
            lblMessBenvenuti.Text = "Salvataggio del DataBase non eseguito."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("ERRORE Salvataggio DataBase", "Salvataggio del DataBase non eseguito. <br>" & strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf SessionUtility.BKAll(myCodDitta, myEsercizio, strErrore) = False Then
            SessionUtility.UpdDataUltimoBKErr(myCodDitta, myEsercizio, "")
            LnkSalvaDataBase.Text = "ERRORE"
            lblMessBenvenuti.Text = "Salvataggio del DataBase non eseguito."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("ERRORE Salvataggio DataBase", "Salvataggio del DataBase non eseguito. <br>" & strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Else
            LnkSalvaDataBase.Visible = False
            lblMessBenvenuti.Text = "Benvenuti su Soft Azienda OnLine"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Salvataggio DataBase", "Salvataggio del DataBase eseguito correttamente.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
    End Sub
    Private Sub ChiudiErrore(ByVal strErrore As String)
        Try
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=" & strErrore.Trim)
        Catch ex As Exception
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=" & strErrore.Trim)
        End Try
        Exit Sub
    End Sub

    Private Function GetNDocByDC() As Boolean
        GetNDocByDC = True
        lblOrdini_DocINCoGe.Text = "ELENCO ORDINI (N° Ordini e consegna prevista)"
        lblDoc1.Text = "" : lblDoc2.Text = "" : lblDoc3.Text = ""
        lblDoc4.Text = "" : lblDoc5.Text = "" : lblDoc6.Text = ""
        lblDoc7.Text = "" : lblDoc8.Text = "" : lblDoc9.Text = ""
        'giu070220
        Dim sGetNDocByDC As String = ";"
        If String.IsNullOrEmpty(Session("GetNDocByDC")) Then
            'OK CARICO
        Else ' 
            sGetNDocByDC = Session("GetNDocByDC")
            If sGetNDocByDC.Trim <> "" Then
                If Not String.IsNullOrEmpty(Session("lblDoc1")) Then
                    lblDoc1.Text = Session("lblDoc1")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc2")) Then
                    lblDoc2.Text = Session("lblDoc2")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc3")) Then
                    lblDoc3.Text = Session("lblDoc3")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc4")) Then
                    lblDoc4.Text = Session("lblDoc4")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc5")) Then
                    lblDoc5.Text = Session("lblDoc5")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc6")) Then
                    lblDoc6.Text = Session("lblDoc6")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc7")) Then
                    lblDoc7.Text = Session("lblDoc7")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc8")) Then
                    lblDoc8.Text = Session("lblDoc8")
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc9")) Then
                    lblDoc9.Text = Session("lblDoc9")
                End If
                '---------------------
                GetNDocByDC = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0
        strSQL = "get_NDocByDataConsegna"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("NDoc")) Then
                            If ds.Tables(0).Rows(ii).Item("NDoc") > 0 Then
                                Select Case i
                                    Case 1, 10
                                        If lblDoc1.Text.Trim <> "" Then lblDoc1.Text += " - .... - "
                                        lblDoc1.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc1.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc1.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc1.Text += " - .... - "
                                    Case 2, 11
                                        If lblDoc2.Text.Trim <> "" Then lblDoc2.Text += " - .... - "
                                        lblDoc2.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc2.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc2.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc2.Text += " - .... - "
                                    Case 3, 12
                                        If lblDoc3.Text.Trim <> "" Then lblDoc3.Text += " - .... - "
                                        lblDoc3.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc3.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc3.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc3.Text += " - .... - "
                                    Case 4, 13
                                        If lblDoc4.Text.Trim <> "" Then lblDoc4.Text += " - .... - "
                                        lblDoc4.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc4.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc4.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc4.Text += " - .... - "
                                    Case 5, 14
                                        If lblDoc5.Text.Trim <> "" Then lblDoc5.Text += " - .... - "
                                        lblDoc5.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc5.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc5.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc5.Text += " - .... - "
                                    Case 6, 15
                                        If lblDoc6.Text.Trim <> "" Then lblDoc6.Text += " - .... - "
                                        lblDoc6.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc6.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc6.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc6.Text += " - .... - "
                                    Case 7, 16
                                        If lblDoc7.Text.Trim <> "" Then lblDoc7.Text += " - .... - "
                                        lblDoc7.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc7.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc7.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc7.Text += " - .... - "
                                    Case 8, 17
                                        If lblDoc8.Text.Trim <> "" Then lblDoc8.Text += " - .... - "
                                        lblDoc8.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc8.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc8.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then lblDoc8.Text += " - .... - "
                                    Case 9, 18
                                        If lblDoc9.Text.Trim <> "" Then lblDoc9.Text += " - .... - "
                                        lblDoc9.Text += "N° " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("NDoc")) & " Ordini"
                                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraConsegna")) Then
                                            lblDoc9.Text += " consegna " & Format(ds.Tables(0).Rows(ii).Item("DataOraConsegna"), FormatoData)
                                        Else
                                            lblDoc9.Text += " consegna [Non definita]"
                                        End If
                                        If i > 9 Then
                                            If ds.Tables(0).Rows.Count > 18 Then
                                                lblDoc9.Text += " [...]"
                                            Else
                                                lblDoc9.Text += " - .... - "
                                            End If
                                        End If
                                End Select
                            End If
                        End If
                        '-
                        ii += 1
                        If i > 18 Then
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblDoc1.Text = "Errore caricamento dati"
            lblDoc2.Text = Ex.Message
            lblDoc3.Text = ""
            lblDoc4.Text = "" : lblDoc5.Text = "" : lblDoc6.Text = ""
            lblDoc7.Text = "" : lblDoc8.Text = "" : lblDoc9.Text = ""
            GetNDocByDC = False
            Exit Function
        End Try
        Session("GetNDocByDC") = sGetNDocByDC
        Session("lblDoc1") = lblDoc1.Text.Trim
        Session("lblDoc2") = lblDoc2.Text.Trim
        Session("lblDoc3") = lblDoc3.Text.Trim
        Session("lblDoc4") = lblDoc4.Text.Trim
        Session("lblDoc5") = lblDoc5.Text.Trim
        Session("lblDoc6") = lblDoc6.Text.Trim
        Session("lblDoc7") = lblDoc7.Text.Trim
        Session("lblDoc8") = lblDoc8.Text.Trim
        Session("lblDoc9") = lblDoc9.Text.Trim
    End Function

    'giu130312 "ELENCO DOCUMENTI da fatturare"
    Private Function GetDDTByDataScadPag(Optional ByVal SWWhere As Integer = 0, Optional ByVal strWhere As String = "", Optional ByVal SortBy As String = "ORDER BY Data_Scadenza_1") As Boolean
        GetDDTByDataScadPag = True

        lblDDT.Text = "ELENCO DOCUMENTI da fatturare"
        lblDDT1.Text = "" : lblDDT2.Text = "" : lblDDT3.Text = "" : lblDDT4.Text = ""
        lblDDT5.Text = "" : lblDDT6.Text = "" : lblDDT7.Text = "" : lblDDT8.Text = ""
        lblDDT1.ToolTip = "" : lblDDT2.ToolTip = "" : lblDDT3.ToolTip = "" : lblDDT4.ToolTip = ""
        lblDDT5.ToolTip = "" : lblDDT6.ToolTip = "" : lblDDT7.ToolTip = "" : lblDDT8.ToolTip = ""
        'giu070220
        Dim sGetDDTByDataScadPag As String = ";"
        If String.IsNullOrEmpty(Session("GetDDTByDataScadPag")) Then
            'OK CARICO
        Else ' 
            sGetDDTByDataScadPag = Session("GetDDTByDataScadPag")
            If sGetDDTByDataScadPag.Trim <> "" Then
                If Not String.IsNullOrEmpty(Session("lblDDT1")) Then
                    lblDDT1.Text = Session("lblDDT1")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT2")) Then
                    lblDDT2.Text = Session("lblDDT2")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT3")) Then
                    lblDDT3.Text = Session("lblDDT3")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT4")) Then
                    lblDDT4.Text = Session("lblDDT4")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT5")) Then
                    lblDDT5.Text = Session("lblDDT5")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT6")) Then
                    lblDDT6.Text = Session("lblDDT6")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT7")) Then
                    lblDDT7.Text = Session("lblDDT7")
                End If
                If Not String.IsNullOrEmpty(Session("lblDDT8")) Then
                    lblDDT8.Text = Session("lblDDT8")
                End If
                '---------------------
                GetDDTByDataScadPag = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        Dim strSQL As String = "SELECT TOP(10) * FROM View_DDTByDataScadPag"
        Dim i As Integer = 0 : Dim ii As Integer = 0
        If strWhere.Trim <> "" Then
            strSQL += " " + strWhere
        End If
        If SortBy.Trim <> "" Then
            strSQL += " " + SortBy
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        Select Case i
                            Case 1
                                lblDDT1.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT1.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT1.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT1.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT1.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT1.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT1.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT1.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT1.Text += " [Bonifico]"
                                Else
                                    lblDDT1.Text += " [Non definita]"
                                End If
                                If lblDDT1.Text.Length > 45 Then
                                    lblDDT1.Text = Mid(lblDDT1.Text, 1, 45)
                                End If
                                lblDDT1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 2
                                lblDDT2.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT2.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT2.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT2.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT2.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT2.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT2.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT2.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT2.Text += " [Bonifico]"
                                Else
                                    lblDDT2.Text += " [Non definita]"
                                End If
                                If lblDDT2.Text.Length > 45 Then
                                    lblDDT2.Text = Mid(lblDDT2.Text, 1, 45)
                                End If
                                lblDDT2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 3
                                lblDDT3.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT3.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT3.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT3.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT3.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT3.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT3.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT3.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT3.Text += " [Bonifico]"
                                Else
                                    lblDDT3.Text += " [Non definita]"
                                End If
                                If lblDDT3.Text.Length > 45 Then
                                    lblDDT3.Text = Mid(lblDDT3.Text, 1, 45)
                                End If
                                lblDDT3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 4
                                lblDDT4.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT4.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT4.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT4.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT4.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT4.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT4.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT4.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT4.Text += " [Bonifico]"
                                Else
                                    lblDDT4.Text += " [Non definita]"
                                End If
                                If lblDDT4.Text.Length > 45 Then
                                    lblDDT4.Text = Mid(lblDDT4.Text, 1, 45)
                                End If
                                lblDDT4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 5
                                lblDDT5.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT5.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT5.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT5.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT5.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT5.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT5.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT5.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT5.Text += " [Bonifico]"
                                Else
                                    lblDDT5.Text += " [Non definita]"
                                End If
                                If lblDDT5.Text.Length > 45 Then
                                    lblDDT5.Text = Mid(lblDDT5.Text, 1, 45)
                                End If
                                lblDDT5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 6
                                lblDDT6.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT6.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT6.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT6.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT6.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT6.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT6.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT6.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT6.Text += " [Bonifico]"
                                Else
                                    lblDDT6.Text += " [Non definita]"
                                End If
                                If lblDDT6.Text.Length > 45 Then
                                    lblDDT6.Text = Mid(lblDDT6.Text, 1, 45)
                                End If
                                lblDDT6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 7
                                lblDDT7.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT7.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT7.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT7.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT7.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT7.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT7.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT7.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT7.Text += " [Bonifico]"
                                Else
                                    lblDDT7.Text += " [Non definita]"
                                End If
                                If lblDDT7.Text.Length > 45 Then
                                    lblDDT7.Text = Mid(lblDDT7.Text, 1, 45)
                                End If
                                lblDDT7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                            Case 8
                                lblDDT8.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDDT8.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDDT8.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblDDT8.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                Else
                                    lblDDT8.Text += " [Non definita]"
                                End If
                                If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                    lblDDT8.Text += " [Non definita]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                    lblDDT8.Text += " [RD]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                    lblDDT8.Text += " [RiBa]"
                                ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                    lblDDT8.Text += " [Bonifico]"
                                Else
                                    lblDDT8.Text += " [Non definita]"
                                End If
                                If lblDDT8.Text.Length > 45 Then
                                    lblDDT8.Text = Mid(lblDDT8.Text, 1, 45)
                                End If
                                lblDDT8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                        End Select
                        '-
                        ii += 1
                        If i > 8 Then
                            If ds.Tables(0).Rows.Count > 8 Then
                                ' ''lblDDT8.Text += " [...]"
                            End If
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblDDT1.Text = "Errore caricamento dati"
            lblDDT2.Text = Ex.Message
            lblDDT3.Text = ""
            lblDDT4.Text = "" : lblDDT5.Text = "" : lblDDT6.Text = ""
            lblDDT7.Text = "" : lblDDT8.Text = ""
            GetDDTByDataScadPag = False
            Exit Function
        End Try
        Session("GetDDTByDataScadPag") = sGetDDTByDataScadPag
        Session("lblDDT1") = lblDDT1.Text.Trim
        Session("lblDDT2") = lblDDT2.Text.Trim
        Session("lblDDT3") = lblDDT3.Text.Trim
        Session("lblDDT4") = lblDDT4.Text.Trim
        Session("lblDDT5") = lblDDT5.Text.Trim
        Session("lblDDT6") = lblDDT6.Text.Trim
        Session("lblDDT7") = lblDDT7.Text.Trim
        Session("lblDDT8") = lblDDT8.Text.Trim
    End Function

    'giu080312 View_NDocByDataScadPag NGGDistRiBa
    Private Function GetNDocByDataScadPag(Optional ByVal SWWhere As Integer = 0, Optional ByVal strWhere As String = "", Optional ByVal SortBy As String = "ORDER BY Data_Scadenza_1") As Boolean
        GetNDocByDataScadPag = True
        'GIU200412 N. GG. LIMITE PER LA PRESENTAZIONE Ri.Ba.
        Dim NGGDistRiBa As Integer = 30
        If Not String.IsNullOrEmpty(Session("NGGDistRiBa")) Then
            If IsNumeric(Session("NGGDistRiBa")) Then
                NGGDistRiBa = Session("NGGDistRiBa")
            End If
        End If
        lblDoc10.ForeColor = Drawing.Color.Black : lblDoc11.ForeColor = Drawing.Color.Black : lblDoc12.ForeColor = Drawing.Color.Black
        lblDoc13.ForeColor = Drawing.Color.Black : lblDoc14.ForeColor = Drawing.Color.Black : lblDoc15.ForeColor = Drawing.Color.Black
        lblDoc16.ForeColor = Drawing.Color.Black : lblDoc17.ForeColor = Drawing.Color.Black : lblDoc18.ForeColor = Drawing.Color.Black
        '---------------------------------------------------
        lblDoc10.Text = "" : lblDoc11.Text = "" : lblDoc12.Text = ""
        lblDoc13.Text = "" : lblDoc14.Text = "" : lblDoc15.Text = ""
        lblDoc16.Text = "" : lblDoc17.Text = "" : lblDoc18.Text = ""
        lblDoc10.ToolTip = "" : lblDoc11.ToolTip = "" : lblDoc12.ToolTip = ""
        lblDoc13.ToolTip = "" : lblDoc14.ToolTip = "" : lblDoc15.ToolTip = ""
        lblDoc16.ToolTip = "" : lblDoc17.ToolTip = "" : lblDoc18.ToolTip = ""
        'giu070220
        Dim sGetNDocByDataScadPag As String = ";"
        If String.IsNullOrEmpty(Session("GetNDocByDataScadPag")) Then
            'OK CARICO
        Else ' in sGetNDocByDataScadPag sono definite le Label in ROSSO lblDoc10.ForeColor = Drawing.Color.DarkRed
            sGetNDocByDataScadPag = Session("GetNDocByDataScadPag")
            If sGetNDocByDataScadPag.Trim <> "" Then
                'lblScProdCons.Text = sGetNDocByDataScadPag.Trim
                '-
                If Not String.IsNullOrEmpty(Session("lblDoc10")) Then
                    lblDoc10.Text = Session("lblDoc10")
                    If InStr(sGetNDocByDataScadPag, "10") > 0 Then
                        lblDoc10.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc11")) Then
                    lblDoc11.Text = Session("lblDoc11")
                    If InStr(sGetNDocByDataScadPag, "11") > 0 Then
                        lblDoc11.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc12")) Then
                    lblDoc12.Text = Session("lblDoc12")
                    If InStr(sGetNDocByDataScadPag, "12") > 0 Then
                        lblDoc12.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc13")) Then
                    lblDoc13.Text = Session("lblDoc13")
                    If InStr(sGetNDocByDataScadPag, "13") > 0 Then
                        lblDoc13.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc14")) Then
                    lblDoc14.Text = Session("lblDoc14")
                    If InStr(sGetNDocByDataScadPag, "14") > 0 Then
                        lblDoc14.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc15")) Then
                    lblDoc15.Text = Session("lblDoc15")
                    If InStr(sGetNDocByDataScadPag, "15") > 0 Then
                        lblDoc15.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc16")) Then
                    lblDoc16.Text = Session("lblDoc16")
                    If InStr(sGetNDocByDataScadPag, "16") > 0 Then
                        lblDoc16.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc17")) Then
                    lblDoc17.Text = Session("lblDoc17")
                    If InStr(sGetNDocByDataScadPag, "17") > 0 Then
                        lblDoc17.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc18")) Then
                    lblDoc18.Text = Session("lblDoc18")
                    If InStr(sGetNDocByDataScadPag, "18") > 0 Then
                        lblDoc18.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                '---------------------
                GetNDocByDataScadPag = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        'giu030412 le NC se trasferite in COGE ma in RIBA no non le visualizzo, perche queste
        'in RIBA non vengono trasferite
        Dim strSQL As String = "SELECT TOP(10) * FROM View_NDocByDataScadPag"
        Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim iii As Integer = 0
        If strWhere.Trim <> "" Then
            strSQL += " " + strWhere
        End If
        If SortBy.Trim <> "" Then
            strSQL += " " + SortBy
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0 : iii = 1
                    For i = 1 To ds.Tables(0).Rows.Count
                        Select Case iii
                            Case 1
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc10.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc10.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc10.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc10.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc10.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";10"
                                            Else
                                                lblDoc10.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc10.Text += " [Non definita]"
                                        lblDoc10.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";10"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc10.Text += " [Non definita]"
                                        lblDoc10.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";10"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc10.Text += " [RD]"
                                        lblDoc10.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc10.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc10.Text += " [Bonifico]"
                                        lblDoc10.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc10.Text += " [Non definita]"
                                        lblDoc10.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";10"
                                    End If
                                    If lblDoc10.Text.Length > 45 Then
                                        lblDoc10.Text = Mid(lblDoc10.Text, 1, 45)
                                    End If
                                    lblDoc10.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 2
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc11.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc11.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc11.Text += " [Non definita]"
                                        lblDoc11.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";11"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc11.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc11.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";11"
                                            Else
                                                lblDoc11.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc11.Text += " [Non definita]"
                                        lblDoc11.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";11"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc11.Text += " [Non definita]"
                                        lblDoc11.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";11"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc11.Text += " [RD]"
                                        lblDoc11.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc11.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc11.Text += " [Bonifico]"
                                        lblDoc11.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc11.Text += " [Non definita]"
                                        lblDoc11.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";11"
                                    End If
                                    If lblDoc11.Text.Length > 45 Then
                                        lblDoc11.Text = Mid(lblDoc11.Text, 1, 45)
                                    End If
                                    lblDoc11.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 3
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc12.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc12.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc12.Text += " [Non definita]"
                                        lblDoc12.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";12"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc12.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc12.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";12"
                                            Else
                                                lblDoc12.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc12.Text += " [Non definita]"
                                        lblDoc12.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";12"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc12.Text += " [Non definita]"
                                        lblDoc12.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";12"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc12.Text += " [RD]"
                                        lblDoc12.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc12.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc12.Text += " [Bonifico]"
                                        lblDoc12.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc12.Text += " [Non definita]"
                                        lblDoc12.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";12"
                                    End If
                                    If lblDoc12.Text.Length > 45 Then
                                        lblDoc12.Text = Mid(lblDoc12.Text, 1, 45)
                                    End If
                                    lblDoc12.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 4
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc13.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc13.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc13.Text += " [Non definita]"
                                        lblDoc13.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";13"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc13.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc13.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";13"
                                            Else
                                                lblDoc13.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc13.Text += " [Non definita]"
                                        lblDoc13.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";13"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc13.Text += " [Non definita]"
                                        lblDoc13.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";13"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc13.Text += " [RD]"
                                        lblDoc13.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc13.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc13.Text += " [Bonifico]"
                                        lblDoc13.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc13.Text += " [Non definita]"
                                        lblDoc13.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";13"
                                    End If
                                    If lblDoc13.Text.Length > 45 Then
                                        lblDoc13.Text = Mid(lblDoc13.Text, 1, 45)
                                    End If
                                    lblDoc13.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 5
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc14.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc14.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc14.Text += " [Non definita]"
                                        lblDoc14.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";14"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc14.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc14.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";14"
                                            Else
                                                lblDoc14.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc14.Text += " [Non definita]"
                                        lblDoc14.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";14"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc14.Text += " [Non definita]"
                                        lblDoc14.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";14"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc14.Text += " [RD]"
                                        lblDoc14.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc14.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc14.Text += " [Bonifico]"
                                        lblDoc14.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc14.Text += " [Non definita]"
                                        lblDoc14.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";14"
                                    End If
                                    If lblDoc14.Text.Length > 45 Then
                                        lblDoc14.Text = Mid(lblDoc14.Text, 1, 45)
                                    End If
                                    lblDoc14.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 6
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc15.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc15.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc15.Text += " [Non definita]"
                                        lblDoc15.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";15"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc15.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc15.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";15"
                                            Else
                                                lblDoc15.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc15.Text += " [Non definita]"
                                        lblDoc15.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";15"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc15.Text += " [Non definita]"
                                        lblDoc15.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";15"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc15.Text += " [RD]"
                                        lblDoc15.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc15.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc15.Text += " [Bonifico]"
                                        lblDoc15.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc15.Text += " [Non definita]"
                                        lblDoc15.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";15"
                                    End If
                                    If lblDoc15.Text.Length > 45 Then
                                        lblDoc15.Text = Mid(lblDoc15.Text, 1, 45)
                                    End If
                                    lblDoc15.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 7
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc16.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc16.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc16.Text += " [Non definita]"
                                        lblDoc16.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";16"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc16.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc16.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";16"
                                            Else
                                                lblDoc16.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc16.Text += " [Non definita]"
                                        lblDoc16.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";16"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc16.Text += " [Non definita]"
                                        lblDoc16.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";16"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc16.Text += " [RD]"
                                        lblDoc16.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc16.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc16.Text += " [Bonifico]"
                                        lblDoc16.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc16.Text += " [Non definita]"
                                        lblDoc16.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";16"
                                    End If
                                    If lblDoc16.Text.Length > 45 Then
                                        lblDoc16.Text = Mid(lblDoc16.Text, 1, 45)
                                    End If
                                    lblDoc16.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 8
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc17.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc17.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc17.Text += " [Non definita]"
                                        lblDoc17.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";17"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc17.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc17.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";17"
                                            Else
                                                lblDoc17.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc17.Text += " [Non definita]"
                                        lblDoc17.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";17"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc17.Text += " [Non definita]"
                                        lblDoc17.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";17"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc17.Text += " [RD]"
                                        lblDoc17.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc17.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc17.Text += " [Bonifico]"
                                        lblDoc17.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc17.Text += " [Non definita]"
                                        lblDoc17.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";17"
                                    End If
                                    If lblDoc17.Text.Length > 45 Then
                                        lblDoc17.Text = Mid(lblDoc17.Text, 1, 45)
                                    End If
                                    lblDoc17.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                            Case 9
                                If ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "NC" And ds.Tables(0).Rows(ii).Item("RiBa") = 0 And _
                                    ds.Tables(0).Rows(ii).Item("CoGe") <> 0 Then
                                    'nulla non le presento
                                Else
                                    lblDoc18.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblDoc18.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblDoc18.Text += " [Non definita]"
                                        lblDoc18.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";18"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblDoc18.Text += " scad(1) " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGGDistRiBa > 0 Then
                                            If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblDoc18.ForeColor = Drawing.Color.DarkRed
                                                sGetNDocByDataScadPag += ";18"
                                            Else
                                                lblDoc18.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblDoc18.Text += " [Non definita]"
                                        lblDoc18.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";18"
                                    End If
                                    If IsDBNull(ds.Tables(0).Rows(ii).Item("Tipo_Pagamento")) Then
                                        lblDoc18.Text += " [Non definita]"
                                        lblDoc18.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";18"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 0 Then
                                        lblDoc18.Text += " [RD]"
                                        lblDoc18.ForeColor = Drawing.Color.Black
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 1 Then
                                        lblDoc18.Text += " [RiBa]"
                                    ElseIf ds.Tables(0).Rows(ii).Item("Tipo_Pagamento") = 2 Then
                                        lblDoc18.Text += " [Bonifico]"
                                        lblDoc18.ForeColor = Drawing.Color.Black
                                    Else
                                        lblDoc18.Text += " [Non definita]"
                                        lblDoc18.ForeColor = Drawing.Color.DarkRed
                                        sGetNDocByDataScadPag += ";18"
                                    End If
                                    If lblDoc18.Text.Length > 45 Then
                                        lblDoc18.Text = Mid(lblDoc18.Text, 1, 45)
                                    End If
                                    lblDoc18.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Des_Pagamento")), "", ds.Tables(0).Rows(ii).Item("Des_Pagamento"))
                                    iii += 1
                                End If
                        End Select
                        '-
                        ii += 1
                        If iii > 9 Then
                            If ds.Tables(0).Rows.Count > 9 Then
                                ' ''lblDoc18.Text += " [...]"
                            End If
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblDoc10.Text = "Errore caricamento dati"
            lblDoc10.ForeColor = Drawing.Color.DarkRed
            lblDoc11.Text = Ex.Message
            lblDoc12.Text = ""
            lblDoc13.Text = "" : lblDoc14.Text = "" : lblDoc15.Text = ""
            lblDoc16.Text = "" : lblDoc17.Text = "" : lblDoc18.Text = ""
            GetNDocByDataScadPag = False
            Exit Function
        End Try
        Session("GetNDocByDataScadPag") = sGetNDocByDataScadPag
        Session("lblDoc10") = lblDoc10.Text.Trim
        Session("lblDoc11") = lblDoc11.Text.Trim
        Session("lblDoc12") = lblDoc12.Text.Trim
        Session("lblDoc13") = lblDoc13.Text.Trim
        Session("lblDoc14") = lblDoc14.Text.Trim
        Session("lblDoc15") = lblDoc15.Text.Trim
        Session("lblDoc16") = lblDoc16.Text.Trim
        Session("lblDoc17") = lblDoc17.Text.Trim
        Session("lblDoc18") = lblDoc18.Text.Trim
    End Function
    'giu100312 View_NDocByDataScadPagINCoGeRB NGGDistRiBa
    Private Function GetNDocByDataScadPagINCoGeRB(Optional ByVal SWWhere As Integer = 0, Optional ByVal strWhere As String = "", Optional ByVal SortBy As String = "ORDER BY Data_Scadenza") As Boolean
        GetNDocByDataScadPagINCoGeRB = True
        Dim NGGDistRiBa As Integer = 30
        If Not String.IsNullOrEmpty(Session("NGGDistRiBa")) Then
            If IsNumeric(Session("NGGDistRiBa")) Then
                NGGDistRiBa = Session("NGGDistRiBa")
            End If
        End If
        lblDoc1.ForeColor = Drawing.Color.Black : lblDoc2.ForeColor = Drawing.Color.Black : lblDoc3.ForeColor = Drawing.Color.Black
        lblDoc4.ForeColor = Drawing.Color.Black : lblDoc5.ForeColor = Drawing.Color.Black : lblDoc6.ForeColor = Drawing.Color.Black
        lblDoc7.ForeColor = Drawing.Color.Black : lblDoc8.ForeColor = Drawing.Color.Black : lblDoc9.ForeColor = Drawing.Color.Black
        '---------------------------------------------------
        lblOrdini_DocINCoGe.Text = "ELENCO Ri.Ba. da presentare (Ordinate per Data di scadenza)"
        lblDoc1.Text = "" : lblDoc2.Text = "" : lblDoc3.Text = ""
        lblDoc4.Text = "" : lblDoc5.Text = "" : lblDoc6.Text = ""
        lblDoc7.Text = "" : lblDoc8.Text = "" : lblDoc9.Text = ""
        '-
        lblDoc1.ToolTip = "" : lblDoc2.ToolTip = "" : lblDoc3.ToolTip = ""
        lblDoc4.ToolTip = "" : lblDoc5.ToolTip = "" : lblDoc6.ToolTip = ""
        lblDoc7.ToolTip = "" : lblDoc8.ToolTip = "" : lblDoc9.ToolTip = ""
        'giu070220
        Dim sGetNDocByDataScadPagINCoGeRB As String = ";"
        If String.IsNullOrEmpty(Session("GetNDocByDataScadPagINCoGeRB")) Then
            'OK CARICO
        Else ' in sGetNDocByDataScadPagINCoGeRB sono definite le Label in ROSSO lblDoc10.ForeColor = Drawing.Color.DarkRed
            sGetNDocByDataScadPagINCoGeRB = Session("GetNDocByDataScadPagINCoGeRB")
            If sGetNDocByDataScadPagINCoGeRB.Trim <> "" Then
                If Not String.IsNullOrEmpty(Session("lblDoc1")) Then
                    lblDoc1.Text = Session("lblDoc1")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "1") > 0 Then
                        lblDoc1.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc2")) Then
                    lblDoc2.Text = Session("lblDoc2")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "2") > 0 Then
                        lblDoc2.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc3")) Then
                    lblDoc3.Text = Session("lblDoc3")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "3") > 0 Then
                        lblDoc3.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc4")) Then
                    lblDoc4.Text = Session("lblDoc4")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "4") > 0 Then
                        lblDoc4.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc5")) Then
                    lblDoc5.Text = Session("lblDoc5")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "5") > 0 Then
                        lblDoc5.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc6")) Then
                    lblDoc6.Text = Session("lblDoc6")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "6") > 0 Then
                        lblDoc6.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc7")) Then
                    lblDoc7.Text = Session("lblDoc7")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "7") > 0 Then
                        lblDoc7.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc8")) Then
                    lblDoc8.Text = Session("lblDoc8")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "8") > 0 Then
                        lblDoc8.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblDoc9")) Then
                    lblDoc9.Text = Session("lblDoc9")
                    If InStr(sGetNDocByDataScadPagINCoGeRB, "9") > 0 Then
                        lblDoc9.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                '---------------------
                GetNDocByDataScadPagINCoGeRB = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        Dim strSQL As String = "SELECT TOP(10) * FROM View_NDocByDataScadPagINCoGeRB"
        Dim i As Integer = 0 : Dim ii As Integer = 0
        If strWhere.Trim <> "" Then
            strSQL += " " + strWhere
        End If
        If SortBy.Trim <> "" Then
            strSQL += " " + SortBy
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        Select Case i
                            Case 1
                                lblDoc1.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc1.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc1.Text += " [Non definita]"
                                    lblDoc1.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "1;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc1.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc1.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "1;"
                                        Else
                                            lblDoc1.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc1.Text += " [Non definita]"
                                    lblDoc1.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "1;"
                                End If
                                lblDoc1.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc1.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc1.Text.Length > 70 Then
                                    lblDoc1.Text = Mid(lblDoc1.Text, 1, 65)
                                End If
                            Case 2
                                lblDoc2.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc2.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc2.Text += " [Non definita]"
                                    lblDoc2.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "2;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc2.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc2.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "2;"
                                        Else
                                            lblDoc2.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc2.Text += " [Non definita]"
                                    lblDoc2.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "2;"
                                End If
                                lblDoc2.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc2.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc2.Text.Length > 70 Then
                                    lblDoc2.Text = Mid(lblDoc2.Text, 1, 65)
                                End If
                            Case 3
                                lblDoc3.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc3.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc3.Text += " [Non definita]"
                                    lblDoc3.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "3;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc3.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc3.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "3;"
                                        Else
                                            lblDoc3.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc3.Text += " [Non definita]"
                                    lblDoc3.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "3;"
                                End If
                                lblDoc3.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc3.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc3.Text.Length > 70 Then
                                    lblDoc3.Text = Mid(lblDoc3.Text, 1, 65)
                                End If
                            Case 4
                                lblDoc4.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc4.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc4.Text += " [Non definita]"
                                    lblDoc4.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "4;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc4.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc4.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "4;"
                                        Else
                                            lblDoc4.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc4.Text += " [Non definita]"
                                    lblDoc4.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "4;"
                                End If
                                lblDoc4.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc4.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc4.Text.Length > 70 Then
                                    lblDoc4.Text = Mid(lblDoc4.Text, 1, 65)
                                End If
                            Case 5
                                lblDoc5.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc5.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc5.Text += " [Non definita]"
                                    lblDoc5.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "5;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc5.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc5.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "5;"
                                        Else
                                            lblDoc5.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc5.Text += " [Non definita]"
                                    lblDoc5.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "5;"
                                End If
                                lblDoc5.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc5.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc5.Text.Length > 70 Then
                                    lblDoc5.Text = Mid(lblDoc5.Text, 1, 65)
                                End If
                            Case 6
                                lblDoc6.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc6.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc6.Text += " [Non definita]"
                                    lblDoc6.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "6;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc6.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc6.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "6;"
                                        Else
                                            lblDoc6.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc6.Text += " [Non definita]"
                                    lblDoc6.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "6;"
                                End If
                                lblDoc6.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc6.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc6.Text.Length > 70 Then
                                    lblDoc6.Text = Mid(lblDoc6.Text, 1, 65)
                                End If
                            Case 7
                                lblDoc7.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc7.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc7.Text += " [Non definita]"
                                    lblDoc7.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "7;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc7.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc7.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "7;"
                                        Else
                                            lblDoc7.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc7.Text += " [Non definita]"
                                    lblDoc7.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "7;"
                                End If
                                lblDoc7.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc7.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc7.Text.Length > 70 Then
                                    lblDoc7.Text = Mid(lblDoc7.Text, 1, 65)
                                End If
                            Case 8
                                lblDoc8.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc8.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc8.Text += " [Non definita]"
                                    lblDoc8.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "8;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc8.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc8.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "8;"
                                        Else
                                            lblDoc8.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc8.Text += " [Non definita]"
                                    lblDoc8.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "8;"
                                End If
                                lblDoc8.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc8.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc8.Text.Length > 70 Then
                                    lblDoc8.Text = Mid(lblDoc8.Text, 1, 65)
                                End If
                            Case 9
                                lblDoc9.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero_Doc"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblDoc9.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblDoc9.Text += " [Non definita]"
                                    lblDoc9.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "9;"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza")) Then
                                    lblDoc9.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza"), FormatoData)
                                    If NGGDistRiBa > 0 Then
                                        If DateAdd(DateInterval.Day, NGGDistRiBa, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza") Then
                                            lblDoc9.ForeColor = Drawing.Color.DarkRed
                                            sGetNDocByDataScadPagINCoGeRB += "9;"
                                        Else
                                            lblDoc9.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblDoc9.Text += " [Non definita]"
                                    lblDoc9.ForeColor = Drawing.Color.DarkRed
                                    sGetNDocByDataScadPagINCoGeRB += "9;"
                                End If
                                lblDoc9.Text += " " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                lblDoc9.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Codice_CoGe")), "", ds.Tables(0).Rows(ii).Item("Codice_CoGe"))
                                lblDoc9.ToolTip += " - " & IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("Rag_Soc"))
                                If lblDoc9.Text.Length > 70 Then
                                    lblDoc9.Text = Mid(lblDoc9.Text, 1, 65)
                                End If
                        End Select
                        '-
                        ii += 1
                        If i > 9 Then
                            If ds.Tables(0).Rows.Count > 9 Then
                                ' ''lblDoc9.Text += " [...]"
                            End If
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblDoc1.Text = "Errore caricamento dati"
            lblDoc1.ForeColor = Drawing.Color.DarkRed
            lblDoc2.Text = Ex.Message
            lblDoc3.Text = ""
            lblDoc4.Text = "" : lblDoc5.Text = "" : lblDoc6.Text = ""
            lblDoc7.Text = "" : lblDoc8.Text = "" : lblDoc9.Text = ""
            '-
            lblDoc1.ToolTip = "" : lblDoc2.ToolTip = "" : lblDoc3.ToolTip = ""
            lblDoc4.ToolTip = "" : lblDoc5.ToolTip = "" : lblDoc6.ToolTip = ""
            lblDoc7.ToolTip = "" : lblDoc8.ToolTip = "" : lblDoc9.ToolTip = ""
            GetNDocByDataScadPagINCoGeRB = False
            Exit Function
        End Try
        Session("GetNDocByDataScadPagINCoGeRB") = sGetNDocByDataScadPagINCoGeRB
        Session("lblDoc1") = lblDoc1.Text.Trim
        Session("lblDoc2") = lblDoc2.Text.Trim
        Session("lblDoc3") = lblDoc3.Text.Trim
        Session("lblDoc4") = lblDoc4.Text.Trim
        Session("lblDoc5") = lblDoc5.Text.Trim
        Session("lblDoc6") = lblDoc6.Text.Trim
        Session("lblDoc7") = lblDoc7.Text.Trim
        Session("lblDoc8") = lblDoc8.Text.Trim
        Session("lblDoc9") = lblDoc9.Text.Trim
    End Function

    'GIU060220 View_CAByDataScadPag NGGCAScadPag NGGCAScadAtt NGGCAScadFin
    Private Function GetCAByDataScadPag(Optional ByVal SWWhere As Integer = 0, Optional ByVal strWhere As String = "", Optional ByVal SortBy As String = "ORDER BY Data_Scadenza_1") As Boolean
        GetCAByDataScadPag = True
        'N. GG. LIMITE PER LA SEGNALAZIONE FISSO 
        Dim NGG As Long = 30
        If Not String.IsNullOrEmpty(Session("NGGCAScadPag")) Then
            If IsNumeric(Session("NGGCAScadPag")) Then
                NGG = Session("NGGCAScadPag")
            End If
        End If
        'giu270720 arr. al fine mese
        Dim myAllaData As Date = DateAdd(DateInterval.Day, NGG, Now.Date)
        myAllaData = DateAdd(DateInterval.Month, 1, myAllaData).ToString("01" + "/MM/yyyy")
        myAllaData = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
        NGG = DateDiff(DateInterval.Day, Now.Date, myAllaData)
        If NGG < 0 Then NGG = NGG * -1
        '--------------------------------------
        Dim NGGFineCA As Integer = 30
        If Not String.IsNullOrEmpty(Session("NGGCAScadFin")) Then
            If IsNumeric(Session("NGGCAScadFin")) Then
                NGGFineCA = Session("NGGCAScadFin")
            End If
        End If
        'giu270720 arr. al fine mese
        myAllaData = DateAdd(DateInterval.Day, NGGFineCA, Now.Date)
        myAllaData = DateAdd(DateInterval.Month, 1, myAllaData).ToString("01" + "/MM/yyyy")
        myAllaData = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
        NGGFineCA = DateDiff(DateInterval.Day, Now.Date, myAllaData)
        If NGGFineCA < 0 Then NGGFineCA = NGGFineCA * -1
        '--------------------------------------
        lblSCContr1.ForeColor = Drawing.Color.Black : lblSCContr2.ForeColor = Drawing.Color.Black : lblSCContr3.ForeColor = Drawing.Color.Black
        lblSCContr4.ForeColor = Drawing.Color.Black : lblSCContr5.ForeColor = Drawing.Color.Black : lblSCContr6.ForeColor = Drawing.Color.Black
        lblSCContr7.ForeColor = Drawing.Color.Black : lblSCContr8.ForeColor = Drawing.Color.Black : lblSCContr9.ForeColor = Drawing.Color.Black
        '---------------------------------------------------
        lblSCContr1.Text = "" : lblSCContr2.Text = "" : lblSCContr3.Text = ""
        lblSCContr4.Text = "" : lblSCContr5.Text = "" : lblSCContr6.Text = ""
        lblSCContr7.Text = "" : lblSCContr8.Text = "" : lblSCContr9.Text = ""
        lblSCContr1.ToolTip = "" : lblSCContr2.ToolTip = "" : lblSCContr3.ToolTip = ""
        lblSCContr4.ToolTip = "" : lblSCContr5.ToolTip = "" : lblSCContr6.ToolTip = ""
        lblSCContr7.ToolTip = "" : lblSCContr8.ToolTip = "" : lblSCContr9.ToolTip = ""
        '-
        'giu070220
        Dim sGetCAByDataScadPag As String = ";"
        If String.IsNullOrEmpty(Session("GetCAByDataScadPag")) Then
            'OK CARICO
        Else ' in sGetCAByDataScadPag sono definite le Label in ROSSO lblSCContr10.ForeColor = Drawing.Color.DarkRed
            sGetCAByDataScadPag = Session("GetCAByDataScadPag")
            If sGetCAByDataScadPag.Trim <> "" Then
                If Not String.IsNullOrEmpty(Session("lblSCContr1")) Then
                    lblSCContr1.Text = Session("lblSCContr1")
                    If InStr(sGetCAByDataScadPag, "1") > 0 Then
                        lblSCContr1.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr2")) Then
                    lblSCContr2.Text = Session("lblSCContr2")
                    If InStr(sGetCAByDataScadPag, "2") > 0 Then
                        lblSCContr2.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr3")) Then
                    lblSCContr3.Text = Session("lblSCContr3")
                    If InStr(sGetCAByDataScadPag, "3") > 0 Then
                        lblSCContr3.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr4")) Then
                    lblSCContr4.Text = Session("lblSCContr4")
                    If InStr(sGetCAByDataScadPag, "4") > 0 Then
                        lblSCContr4.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr5")) Then
                    lblSCContr5.Text = Session("lblSCContr5")
                    If InStr(sGetCAByDataScadPag, "5") > 0 Then
                        lblSCContr5.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr6")) Then
                    lblSCContr6.Text = Session("lblSCContr6")
                    If InStr(sGetCAByDataScadPag, "6") > 0 Then
                        lblSCContr6.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr7")) Then
                    lblSCContr7.Text = Session("lblSCContr7")
                    If InStr(sGetCAByDataScadPag, "7") > 0 Then
                        lblSCContr7.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr8")) Then
                    lblSCContr8.Text = Session("lblSCContr8")
                    If InStr(sGetCAByDataScadPag, "8") > 0 Then
                        lblSCContr8.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblSCContr9")) Then
                    lblSCContr9.Text = Session("lblSCContr9")
                    If InStr(sGetCAByDataScadPag, "9") > 0 Then
                        lblSCContr9.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                '---------------------
                GetCAByDataScadPag = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        '-------
        Dim strSQL As String = "SELECT TOP(10) * FROM View_CAByDataScadPag"
        Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim iii As Integer = 0
        If strWhere.Trim <> "" Then
            strSQL += " " + strWhere
        End If
        If SortBy.Trim <> "" Then
            strSQL += " " + SortBy
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myTipoCA As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0 : iii = 1
                    For i = 1 To ds.Tables(0).Rows.Count
                        If InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "MANUT") > 0 Then
                            myTipoCA = "CM" 'ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CM"
                        ElseIf InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "TELEC") > 0 Then
                            myTipoCA = "CT" 'ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CT"
                        ElseIf InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "LOCAZ") > 0 Then
                            myTipoCA = "CL" 'ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CL"
                        End If
                        Select Case iii
                            Case 1
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr1.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr1.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr1.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr1.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr1.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "1;"
                                            Else
                                                lblSCContr1.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr1.Text += " [Non definita]"
                                        lblSCContr1.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "1;"
                                    End If
                                    If lblSCContr1.Text.Length > 40 Then
                                        lblSCContr1.Text = Mid(lblSCContr1.Text, 1, 45)
                                    End If
                                    lblSCContr1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr1.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "1;"
                                    lblSCContr1.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr1.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr1.Text += " [Non definita]"
                                    End If
                                    If lblSCContr1.Text.Length > 40 Then
                                        lblSCContr1.Text = Mid(lblSCContr1.Text, 1, 45)
                                    End If
                                    lblSCContr1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr1.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "1;"
                                        Else
                                            'non la presento lblSCContr1.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr1.Text = myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr1.Text.Length > 40 Then
                                        lblSCContr1.Text = Mid(lblSCContr1.Text, 1, 45)
                                    End If
                                    lblSCContr1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr1.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr1.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 2
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr2.Text = myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr2.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr2.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr2.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr2.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "2;"
                                            Else
                                                lblSCContr2.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr2.Text += " [Non definita]"
                                        lblSCContr2.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "2;"
                                    End If
                                    If lblSCContr2.Text.Length > 40 Then
                                        lblSCContr2.Text = Mid(lblSCContr2.Text, 1, 45)
                                    End If
                                    lblSCContr2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr2.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "2;"
                                    lblSCContr2.Text = myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr2.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr2.Text += " [Non definita]"
                                    End If
                                    If lblSCContr2.Text.Length > 40 Then
                                        lblSCContr2.Text = Mid(lblSCContr2.Text, 1, 45)
                                    End If
                                    lblSCContr2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr2.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "2;"
                                        Else
                                            'non la presento lblSCContr2.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr2.Text = myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr2.Text.Length > 40 Then
                                        lblSCContr2.Text = Mid(lblSCContr2.Text, 1, 45)
                                    End If
                                    lblSCContr2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr2.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr2.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 3
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr3.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr3.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr3.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr3.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr3.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "3;"
                                            Else
                                                lblSCContr3.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr3.Text += " [Non definita]"
                                        lblSCContr3.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "3;"
                                    End If
                                    If lblSCContr3.Text.Length > 40 Then
                                        lblSCContr3.Text = Mid(lblSCContr3.Text, 1, 45)
                                    End If
                                    lblSCContr3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr3.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "3;"
                                    lblSCContr3.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr3.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr3.Text += " [Non definita]"
                                    End If
                                    If lblSCContr3.Text.Length > 40 Then
                                        lblSCContr3.Text = Mid(lblSCContr3.Text, 1, 45)
                                    End If
                                    lblSCContr3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr3.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "3;"
                                        Else
                                            'non la presento lblSCContr3.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr3.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr3.Text.Length > 40 Then
                                        lblSCContr3.Text = Mid(lblSCContr3.Text, 1, 45)
                                    End If
                                    lblSCContr3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr3.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr3.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 4
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr4.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr4.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr4.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr4.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr4.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "4;"
                                            Else
                                                lblSCContr4.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr4.Text += " [Non definita]"
                                        lblSCContr4.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "4;"
                                    End If
                                    If lblSCContr4.Text.Length > 40 Then
                                        lblSCContr4.Text = Mid(lblSCContr4.Text, 1, 45)
                                    End If
                                    lblSCContr4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr4.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "4;"
                                    lblSCContr4.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr4.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr4.Text += " [Non definita]"
                                    End If
                                    If lblSCContr4.Text.Length > 40 Then
                                        lblSCContr4.Text = Mid(lblSCContr4.Text, 1, 45)
                                    End If
                                    lblSCContr4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr4.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "4;"
                                        Else
                                            'non la presento lblSCContr4.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr4.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr4.Text.Length > 40 Then
                                        lblSCContr4.Text = Mid(lblSCContr4.Text, 1, 45)
                                    End If
                                    lblSCContr4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr4.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr4.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 5
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr5.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr5.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr5.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr5.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr5.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "5;"
                                            Else
                                                lblSCContr5.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr5.Text += " [Non definita]"
                                        lblSCContr5.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "5;"
                                    End If
                                    If lblSCContr5.Text.Length > 40 Then
                                        lblSCContr5.Text = Mid(lblSCContr5.Text, 1, 45)
                                    End If
                                    lblSCContr5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr5.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "5;"
                                    lblSCContr5.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr5.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr5.Text += " [Non definita]"
                                    End If
                                    If lblSCContr5.Text.Length > 40 Then
                                        lblSCContr5.Text = Mid(lblSCContr5.Text, 1, 45)
                                    End If
                                    lblSCContr5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr5.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "5;"
                                        Else
                                            'non la presento lblSCContr5.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr5.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr5.Text.Length > 40 Then
                                        lblSCContr5.Text = Mid(lblSCContr5.Text, 1, 45)
                                    End If
                                    lblSCContr5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr5.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr5.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 6
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr6.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr6.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr6.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr6.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr6.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "6;"
                                            Else
                                                lblSCContr6.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr6.Text += " [Non definita]"
                                        lblSCContr6.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "6;"
                                    End If
                                    If lblSCContr6.Text.Length > 40 Then
                                        lblSCContr6.Text = Mid(lblSCContr6.Text, 1, 45)
                                    End If
                                    lblSCContr6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr6.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "6;"
                                    lblSCContr6.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr6.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr6.Text += " [Non definita]"
                                    End If
                                    If lblSCContr6.Text.Length > 40 Then
                                        lblSCContr6.Text = Mid(lblSCContr6.Text, 1, 45)
                                    End If
                                    lblSCContr6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr6.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "6;"
                                        Else
                                            'non la presento lblSCContr6.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr6.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr6.Text.Length > 40 Then
                                        lblSCContr6.Text = Mid(lblSCContr6.Text, 1, 45)
                                    End If
                                    lblSCContr6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr6.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr6.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 7
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr7.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr7.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr7.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr7.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr7.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "7;"
                                            Else
                                                lblSCContr7.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr7.Text += " [Non definita]"
                                        lblSCContr7.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "7;"
                                    End If
                                    If lblSCContr7.Text.Length > 40 Then
                                        lblSCContr7.Text = Mid(lblSCContr7.Text, 1, 45)
                                    End If
                                    lblSCContr7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr7.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "7;"
                                    lblSCContr7.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr7.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr7.Text += " [Non definita]"
                                    End If
                                    If lblSCContr7.Text.Length > 40 Then
                                        lblSCContr7.Text = Mid(lblSCContr7.Text, 1, 45)
                                    End If
                                    lblSCContr7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr7.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "7;"
                                        Else
                                            'non la presento lblSCContr7.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr7.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr7.Text.Length > 40 Then
                                        lblSCContr7.Text = Mid(lblSCContr7.Text, 1, 45)
                                    End If
                                    lblSCContr7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr7.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr7.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 8
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr8.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr8.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr8.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr8.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr8.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "8;"
                                            Else
                                                lblSCContr8.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr8.Text += " [Non definita]"
                                        lblSCContr8.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "8;"
                                    End If
                                    If lblSCContr8.Text.Length > 40 Then
                                        lblSCContr8.Text = Mid(lblSCContr8.Text, 1, 45)
                                    End If
                                    lblSCContr8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr8.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "8;"
                                    lblSCContr8.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr8.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr8.Text += " [Non definita]"
                                    End If
                                    If lblSCContr8.Text.Length > 40 Then
                                        lblSCContr8.Text = Mid(lblSCContr8.Text, 1, 45)
                                    End If
                                    lblSCContr8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr8.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "8;"
                                        Else
                                            'non la presento lblSCContr8.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr8.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr8.Text.Length > 40 Then
                                        lblSCContr8.Text = Mid(lblSCContr8.Text, 1, 45)
                                    End If
                                    lblSCContr8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr8.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr8.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                            Case 9
                                If Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "FINE" And Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) <> "INCO" Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                'OK
                                            Else
                                                Continue For
                                            End If
                                        End If
                                    End If
                                    lblSCContr9.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr9.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr9.Text += " [Non definita]"
                                    End If
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                        lblSCContr9.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                        If NGG > 0 Then
                                            If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                                lblSCContr9.ForeColor = Drawing.Color.DarkRed
                                                sGetCAByDataScadPag += "9;"
                                            Else
                                                lblSCContr9.ForeColor = Drawing.Color.Black
                                            End If
                                        End If
                                    Else
                                        lblSCContr9.Text += " [Non definita]"
                                        lblSCContr9.ForeColor = Drawing.Color.DarkRed
                                        sGetCAByDataScadPag += "9;"
                                    End If
                                    If lblSCContr9.Text.Length > 40 Then
                                        lblSCContr9.Text = Mid(lblSCContr9.Text, 1, 45)
                                    End If
                                    lblSCContr9.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                    iii += 1
                                ElseIf Left(ds.Tables(0).Rows(ii).Item("Tipo_Doc"), 4) = "INCO" Then
                                    lblSCContr9.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadPag += "9;"
                                    lblSCContr9.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc")
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr9.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr9.Text += " [Non definita]"
                                    End If
                                    If lblSCContr9.Text.Length > 40 Then
                                        lblSCContr9.Text = Mid(lblSCContr9.Text, 1, 45)
                                    End If
                                    lblSCContr9.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    iii += 1
                                Else
                                    If NGGFineCA > 0 Then
                                        If DateAdd(DateInterval.Day, NGGFineCA, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCContr9.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadPag += "9;"
                                        Else
                                            'non la presento lblSCContr9.ForeColor = Drawing.Color.Black
                                            Continue For
                                        End If
                                    End If
                                    lblSCContr9.Text += myTipoCA & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero")) & " " & ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " al " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If lblSCContr9.Text.Length > 40 Then
                                        lblSCContr9.Text = Mid(lblSCContr9.Text, 1, 45)
                                    End If
                                    lblSCContr9.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione")) & " N°: " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                        lblSCContr9.ToolTip += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                    Else
                                        lblSCContr9.ToolTip += " [Non definita]"
                                    End If
                                    iii += 1
                                End If
                        End Select
                        '-
                        ii += 1
                        If iii > 9 Then
                            If ds.Tables(0).Rows.Count > 9 Then
                                lblSCContr9.Text += " [...]"
                            End If
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblSCContr1.Text = "Errore caricamento dati"
            lblSCContr2.ForeColor = Drawing.Color.DarkRed
            sGetCAByDataScadPag += "1;2;"
            lblSCContr3.Text = Ex.Message
            lblSCContr4.Text = ""
            lblSCContr5.Text = "" : lblSCContr6.Text = "" : lblSCContr7.Text = ""
            lblSCContr8.Text = "" : lblSCContr9.Text = ""
            GetCAByDataScadPag = False
            Exit Function
        End Try
        Session("GetCAByDataScadPag") = sGetCAByDataScadPag
        Session("lblSCContr1") = lblSCContr1.Text.Trim
        Session("lblSCContr2") = lblSCContr2.Text.Trim
        Session("lblSCContr3") = lblSCContr3.Text.Trim
        Session("lblSCContr4") = lblSCContr4.Text.Trim
        Session("lblSCContr5") = lblSCContr5.Text.Trim
        Session("lblSCContr6") = lblSCContr6.Text.Trim
        Session("lblSCContr7") = lblSCContr7.Text.Trim
        Session("lblSCContr8") = lblSCContr8.Text.Trim
        Session("lblSCContr9") = lblSCContr9.Text.Trim
    End Function
    'giu190220
    Private Function GetCAByDataScadAtt(Optional ByVal SWWhere As Integer = 0, Optional ByVal strWhere As String = "", Optional ByVal SortBy As String = "ORDER BY Data_Scadenza_1") As Boolean
        GetCAByDataScadAtt = True
        'N. GG. LIMITE PER LA SEGNALAZIONE FISSO 
        Dim NGG As Integer = 30
        If Not String.IsNullOrEmpty(Session("NGGCAScadAtt")) Then
            If IsNumeric(Session("NGGCAScadAtt")) Then
                NGG = Session("NGGCAScadAtt")
            End If
        End If
        'giu270720 arr. al fine mese
        Dim myAllaData As Date = DateAdd(DateInterval.Day, NGG, Now.Date)
        myAllaData = DateAdd(DateInterval.Month, 1, myAllaData).ToString("01" + "/MM/yyyy")
        myAllaData = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
        NGG = DateDiff(DateInterval.Day, Now.Date, myAllaData)
        If NGG < 0 Then NGG = NGG * -1
        '--------------------------------------
        lblSCMan1.ForeColor = Drawing.Color.Black : lblSCMan2.ForeColor = Drawing.Color.Black : lblSCMan3.ForeColor = Drawing.Color.Black
        lblSCMan4.ForeColor = Drawing.Color.Black : lblSCMan5.ForeColor = Drawing.Color.Black : lblSCMan6.ForeColor = Drawing.Color.Black
        lblScMan7.ForeColor = Drawing.Color.Black : lblScMan8.ForeColor = Drawing.Color.Black
        '---------------------------------------------------
        lblSCMan1.Text = "" : lblSCMan2.Text = "" : lblSCMan3.Text = ""
        lblSCMan4.Text = "" : lblSCMan5.Text = "" : lblSCMan6.Text = ""
        lblScMan7.Text = "" : lblScMan8.Text = ""
        lblSCMan1.ToolTip = "" : lblSCMan2.ToolTip = "" : lblSCMan3.ToolTip = ""
        lblSCMan4.ToolTip = "" : lblSCMan5.ToolTip = "" : lblSCMan6.ToolTip = ""
        lblScMan7.ToolTip = "" : lblScMan8.ToolTip = ""
        '-
        'giu070220
        Dim sGetCAByDataScadAtt As String = ";"
        If String.IsNullOrEmpty(Session("GetCAByDataScadAtt")) Then
            'OK CARICO
        Else ' in sGetCAByDataScadAtt sono definite le Label in ROSSO lblScMan10.ForeColor = Drawing.Color.DarkRed
            sGetCAByDataScadAtt = Session("GetCAByDataScadAtt")
            If sGetCAByDataScadAtt.Trim <> "" Then
                If Not String.IsNullOrEmpty(Session("lblScMan1")) Then
                    lblSCMan1.Text = Session("lblScMan1")
                    If InStr(sGetCAByDataScadAtt, "1") > 0 Then
                        lblSCMan1.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan2")) Then
                    lblSCMan2.Text = Session("lblScMan2")
                    If InStr(sGetCAByDataScadAtt, "2") > 0 Then
                        lblSCMan2.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan3")) Then
                    lblSCMan3.Text = Session("lblScMan3")
                    If InStr(sGetCAByDataScadAtt, "3") > 0 Then
                        lblSCMan3.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan4")) Then
                    lblSCMan4.Text = Session("lblScMan4")
                    If InStr(sGetCAByDataScadAtt, "4") > 0 Then
                        lblSCMan4.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan5")) Then
                    lblSCMan5.Text = Session("lblScMan5")
                    If InStr(sGetCAByDataScadAtt, "5") > 0 Then
                        lblSCMan5.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan6")) Then
                    lblSCMan6.Text = Session("lblScMan6")
                    If InStr(sGetCAByDataScadAtt, "6") > 0 Then
                        lblSCMan6.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan7")) Then
                    lblScMan7.Text = Session("lblScMan7")
                    If InStr(sGetCAByDataScadAtt, "7") > 0 Then
                        lblScMan7.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                If Not String.IsNullOrEmpty(Session("lblScMan8")) Then
                    lblScMan8.Text = Session("lblScMan8")
                    If InStr(sGetCAByDataScadAtt, "8") > 0 Then
                        lblScMan8.ForeColor = Drawing.Color.DarkRed
                    End If
                End If
                '---------------------
                GetCAByDataScadAtt = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        'end 070220
        '-------
        Dim strSQL As String = "SELECT TOP(10) * FROM View_CAByDataScadAtt"
        Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim iii As Integer = 0
        If strWhere.Trim <> "" Then
            strSQL += " " + strWhere
        End If
        If SortBy.Trim <> "" Then
            strSQL += " " + SortBy
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0 : iii = 1
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                            If NGG > 0 Then
                                If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                    'ok
                                Else
                                    Continue For
                                End If
                            End If
                        End If
                        ds.Tables(0).Rows(ii).BeginEdit()
                        If InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "MANUT") > 0 Then
                            ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CM"
                        ElseIf InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "TELEC") > 0 Then
                            ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CT"
                        ElseIf InStr(ds.Tables(0).Rows(ii).Item("Descrizione"), "LOCAZ") > 0 Then
                            ds.Tables(0).Rows(ii).Item("Tipo_Doc") = "CL"
                        End If
                        ds.Tables(0).Rows(ii).EndEdit()
                        Select Case iii
                            Case 1
                                lblSCMan1.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan1.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan1.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan1.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan1.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "1;"
                                        Else
                                            lblSCMan1.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan1.Text += " [Non definita]"
                                    lblSCMan1.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "1;"
                                End If
                                If lblSCMan1.Text.Length > 40 Then
                                    lblSCMan1.Text = Mid(lblSCMan1.Text, 1, 45)
                                End If
                                lblSCMan1.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 2
                                lblSCMan2.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan2.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan2.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan2.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan2.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "2;"
                                        Else
                                            lblSCMan2.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan2.Text += " [Non definita]"
                                    lblSCMan2.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "2;"
                                End If
                                If lblSCMan2.Text.Length > 40 Then
                                    lblSCMan2.Text = Mid(lblSCMan2.Text, 1, 45)
                                End If
                                lblSCMan2.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 3
                                lblSCMan3.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan3.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan3.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan3.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan3.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "3;"
                                        Else
                                            lblSCMan3.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan3.Text += " [Non definita]"
                                    lblSCMan3.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "3;"
                                End If
                                If lblSCMan3.Text.Length > 40 Then
                                    lblSCMan3.Text = Mid(lblSCMan3.Text, 1, 45)
                                End If
                                lblSCMan3.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 4
                                lblSCMan4.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan4.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan4.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan4.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan4.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "4;"
                                        Else
                                            lblSCMan4.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan4.Text += " [Non definita]"
                                    lblSCMan4.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "4;"
                                End If
                                If lblSCMan4.Text.Length > 40 Then
                                    lblSCMan4.Text = Mid(lblSCMan4.Text, 1, 45)
                                End If
                                lblSCMan4.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 5
                                lblSCMan5.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan5.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan5.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan5.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan5.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "5;"
                                        Else
                                            lblSCMan5.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan5.Text += " [Non definita]"
                                    lblSCMan5.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "5;"
                                End If
                                If lblSCMan5.Text.Length > 40 Then
                                    lblSCMan5.Text = Mid(lblSCMan5.Text, 1, 45)
                                End If
                                lblSCMan5.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 6
                                lblSCMan6.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblSCMan6.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblSCMan6.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblSCMan6.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblSCMan6.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "6;"
                                        Else
                                            lblSCMan6.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblSCMan6.Text += " [Non definita]"
                                    lblSCMan6.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "6;"
                                End If
                                If lblSCMan6.Text.Length > 40 Then
                                    lblSCMan6.Text = Mid(lblSCMan6.Text, 1, 45)
                                End If
                                lblSCMan6.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 7
                                lblScMan7.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblScMan7.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblScMan7.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblScMan7.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblScMan7.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "7;"
                                        Else
                                            lblScMan7.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblScMan7.Text += " [Non definita]"
                                    lblScMan7.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "7;"
                                End If
                                If lblScMan7.Text.Length > 40 Then
                                    lblScMan7.Text = Mid(lblScMan7.Text, 1, 45)
                                End If
                                lblScMan7.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                            Case 8
                                lblScMan8.Text += ds.Tables(0).Rows(ii).Item("Tipo_Doc") & " " & Formatta.FormattaNumero(ds.Tables(0).Rows(ii).Item("Numero"))
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Doc")) Then
                                    lblScMan8.Text += " del " & Format(ds.Tables(0).Rows(ii).Item("Data_Doc"), FormatoData)
                                Else
                                    lblScMan8.Text += " [Non definita]"
                                End If
                                If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1")) Then
                                    lblScMan8.Text += " scad. " & Format(ds.Tables(0).Rows(ii).Item("Data_Scadenza_1"), FormatoData)
                                    If NGG > 0 Then
                                        If DateAdd(DateInterval.Day, NGG, Now.Date) > ds.Tables(0).Rows(ii).Item("Data_Scadenza_1") Then
                                            lblScMan8.ForeColor = Drawing.Color.DarkRed
                                            sGetCAByDataScadAtt += "8;"
                                        Else
                                            lblScMan8.ForeColor = Drawing.Color.Black
                                        End If
                                    End If
                                Else
                                    lblScMan8.Text += " [Non definita]"
                                    lblScMan8.ForeColor = Drawing.Color.DarkRed
                                    sGetCAByDataScadAtt += "8;"
                                End If
                                If lblScMan8.Text.Length > 40 Then
                                    lblScMan8.Text = Mid(lblScMan8.Text, 1, 45)
                                End If
                                lblScMan8.ToolTip = IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Rag_Soc")), "", ds.Tables(0).Rows(ii).Item("COD_CLIENTE") + " - " + ds.Tables(0).Rows(ii).Item("Rag_Soc")) + " - " + IIf(IsDBNull(ds.Tables(0).Rows(ii).Item("Descrizione")), "", ds.Tables(0).Rows(ii).Item("Descrizione"))
                                iii += 1
                        End Select
                        '-
                        ii += 1
                        If iii > 8 Then
                            If ds.Tables(0).Rows.Count > 8 Then
                                lblScMan8.Text += " [...]"
                            End If
                            Exit For
                        End If

                    Next
                End If
            End If
        Catch Ex As Exception
            lblSCMan1.Text = "Errore caricamento dati"
            lblSCMan2.ForeColor = Drawing.Color.DarkRed
            sGetCAByDataScadAtt += "1;2;"
            lblSCMan3.Text = Ex.Message
            lblSCMan4.Text = ""
            lblSCMan5.Text = "" : lblSCMan6.Text = "" : lblScMan7.Text = ""
            lblScMan8.Text = ""
            GetCAByDataScadAtt = False
            Exit Function
        End Try
        Session("GetCAByDataScadAtt") = sGetCAByDataScadAtt
        Session("lblScMan1") = lblSCMan1.Text.Trim
        Session("lblScMan2") = lblSCMan2.Text.Trim
        Session("lblScMan3") = lblSCMan3.Text.Trim
        Session("lblScMan4") = lblSCMan4.Text.Trim
        Session("lblScMan5") = lblSCMan5.Text.Trim
        Session("lblScMan6") = lblSCMan6.Text.Trim
        Session("lblScMan7") = lblScMan7.Text.Trim
        Session("lblScMan8") = lblScMan8.Text.Trim
    End Function
    'giu010312 GIU290618
    Private Function GetOpConnessi(Optional ByVal SortBy As String = "Order by DataOraUltAzione DESC") As Boolean
        GetOpConnessi = True
        lblUtConn0.Text = "" : lblUtConn0.ToolTip = ""
        Dim strErrore As String = ""
        If SessionUtility.CTROpTimeOUT(strErrore, lblUtente.Text) = False Then
            lblUtConn1.Text = "Errore cancella OperatoriConnessi"
            lblUtConn2.Text = strErrore
            Return False
        End If
        '---------
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0
        strSQL = "SELECT * FROM View_OperatoriConnessi " & SortBy
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    lblUtConn0.Text = "Totale connessioni utenti attualmente attive: " & ds.Tables(0).Rows.Count.ToString
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If ds.Tables(0).Rows(ii).Item("NomeOperatore").ToString.Trim = lblUtente.Text.Trim Then
                            If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraAccessoPrec")) Then
                                lblUltimoAccesso.Text = "Accesso precedente: " & Format(ds.Tables(0).Rows(ii).Item("DataOraAccessoPrec"), "dddd d MMMM yyyy, HH:mm")
                            Else
                                lblUltimoAccesso.Text = "[Non effettuato]"
                            End If
                        End If
                        '-----
                        lblUtConn0.ToolTip += "(" & ds.Tables(0).Rows(ii).Item("Postazione") & ") (" & ds.Tables(0).Rows(ii).Item("Azienda") & ")"
                        lblUtConn0.ToolTip += "(" & ds.Tables(0).Rows(ii).Item("NomeOperatore") & ") (" & ds.Tables(0).Rows(ii).Item("Modulo") & " " & ds.Tables(0).Rows(ii).Item("Esercizio") & ") "
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraUltAzione")) Then
                            lblUtConn0.ToolTip += "Ultima azione: (" & Format(ds.Tables(0).Rows(ii).Item("DataOraUltAzione"), FormatoDataOra) & ")"
                        Else
                            lblUtConn0.ToolTip += "[...]"
                        End If
                        lblUtConn0.ToolTip += " - "
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraUltimoAccesso")) Then
                            lblUtConn0.ToolTip += "Accesso: (" & Format(ds.Tables(0).Rows(ii).Item("DataOraUltimoAccesso"), FormatoDataOra) & ")"
                        Else
                            lblUtConn0.ToolTip += "[Non effettuato]"
                        End If
                        lblUtConn0.ToolTip += vbCr
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            lblUtConn0.Text = "Errore caricamento connessioni utenti."
            lblUtConn0.ToolTip = Ex.Message.Trim
            GetOpConnessi = False
            Exit Function
        End Try

    End Function

    'giu280818 PRODOTTI CONSUMABILI
    Dim txtDallaData As String = ""
    Dim txtAllaData As String = ""
    Dim ddlCatCli As Integer = -1
    Dim chkRaggrCatCli As Boolean = False
    Dim chkSelCategorie As Boolean = False
    Dim chkTutteCatCli As Boolean = True
    '-
    Dim chkSelScGa As Boolean = False
    Dim chkSelScEl As Boolean = False
    Dim chkSelScBa As Boolean = False

    Private Function GetScadProdCons() As Boolean
        GetScadProdCons = False
        lblUtConn1.Text = "" : lblUtConn2.Text = "" : lblUtConn3.Text = "" : lblUtConn4.Text = "" : lblUtConn5.Text = ""
        lblUtConn1.ToolTip = "" : lblUtConn2.ToolTip = "" : lblUtConn3.ToolTip = "" : lblUtConn4.ToolTip = "" : lblUtConn5.ToolTip = "" '
        lblScProdCons.ForeColor = Drawing.Color.White
        lblScProdCons.Text = ""
        '- USO LA SESSIONE PER NON RALLENTARE
        Dim sGetScadProdCons As String = ""
        'Dim strUtConn1 As String = "" : Dim strUtConn2 As String = ""
        'Dim strUtConn3 As String = "" : Dim strUtConn4 As String = "" : Dim strUtConn5 As String = ""
        'GIU210918 NEL MODULO MasterPage.Master.vb controllo se sono state aggiornate le tabelle interessate azzerando il campo:
        'Session("GetScadProdCons")
        If String.IsNullOrEmpty(Session("GetScadProdCons")) Then
            'OK CARICO
        Else
            sGetScadProdCons = Session("GetScadProdCons")
            If sGetScadProdCons.Trim <> "" Then
                lblScProdCons.Text = sGetScadProdCons.Trim
                '-
                If Not String.IsNullOrEmpty(Session("lblUtConn1")) Then
                    lblUtConn1.Text = Session("lblUtConn1")
                End If
                If Not String.IsNullOrEmpty(Session("lblUtConn2")) Then
                    lblUtConn2.Text = Session("lblUtConn2")
                End If
                If Not String.IsNullOrEmpty(Session("lblUtConn3")) Then
                    lblUtConn3.Text = Session("lblUtConn3")
                End If
                If Not String.IsNullOrEmpty(Session("lblUtConn4")) Then
                    lblUtConn4.Text = Session("lblUtConn4")
                End If
                If Not String.IsNullOrEmpty(Session("lblUtConn5")) Then
                    lblUtConn5.Text = Session("lblUtConn5")
                End If
                '---------------------
                GetScadProdCons = True
                Exit Function
            Else
                'OK CARICO
            End If
        End If
        '
        Dim strErrore As String = ""
        Try
            Dim myDallaData As DateTime = Now.Date
            Dim myAllaData As DateTime = Now.Date
            If CaricaParametri(Session(ESERCIZIO), strErrore) Then
                myDallaData = DateAdd(DateInterval.Month, Val(GetParamGestAzi(Session(ESERCIZIO)).SelAIDaData) * -1, Now.Date)
                myAllaData = DateAdd(DateInterval.Month, Val(GetParamGestAzi(Session(ESERCIZIO)).SelAIAData), Now.Date)
            Else
                lblScProdCons.ForeColor = SEGNALA_KO
                lblScProdCons.Text = "Errore: Caricamento parametri generali. " & strErrore
                Exit Function
            End If
            Dim myAnno As Integer = myDallaData.Year
            Dim MyMese As Integer = myDallaData.Month
            Dim MyDalPeriodo As New Date(myAnno, MyMese, 1)
            txtDallaData = MyDalPeriodo.ToString("MM/yyyy")  'seleziono il mese attuale

            Dim myNextAnno As Integer = myAllaData.Year
            Dim MyNextMese As Integer = myAllaData.Month
           
            Dim MyAlPeriodo As New Date(myNextAnno, MyNextMese, 1)
            txtAllaData = MyAlPeriodo.ToString("MM/yyyy")
            '
            If Not IsDBNull(GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli) Then
                If GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli = -1 Then 'TUTTE LE CATEGORIE
                    ddlCatCli = -1
                    chkRaggrCatCli = False
                    chkSelCategorie = False
                    chkTutteCatCli = True
                ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli = 0 Then 'SELEZIONE MULTIPLA CATEGORIE
                    ddlCatCli = -1
                    chkRaggrCatCli = False
                    chkSelCategorie = True
                    chkTutteCatCli = False
                    Dim NSel As Integer = 0
                    If LeggiCategorie("", NSel) = True Then
                        'OK Selezione multipla categorie
                    Else
                        chkSelCategorie = False
                    End If
                ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli > 0 Then 'SELEZIONE SINGOLA CATEGORIA
                    ddlCatCli = GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli
                    chkRaggrCatCli = False
                    chkSelCategorie = False
                    chkTutteCatCli = False
                ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli < 0 Then 'SELEZIONE SINGOLA CATEGORIA RAGGRUPPATA
                    ddlCatCli = GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli * -1
                    chkRaggrCatCli = True
                    chkSelCategorie = False
                    chkTutteCatCli = False
                End If
            Else
                ddlCatCli = -1
                chkRaggrCatCli = False
                chkSelCategorie = False
                chkTutteCatCli = False
            End If
            chkSelScGa = GetParamGestAzi(Session(ESERCIZIO)).SelAIScGa
            chkSelScEl = GetParamGestAzi(Session(ESERCIZIO)).SelAIScEl
            chkSelScBa = GetParamGestAzi(Session(ESERCIZIO)).SelAIScBa
            '--
        Catch ex As Exception
            lblScProdCons.ForeColor = SEGNALA_KO
            lblScProdCons.Text = "Errore: Caricamento parametri generali. " & strErrore
            Exit Function
        End Try
        GetScadProdCons = GetScadProdConsOK()
        If GetScadProdCons = False Then
            Exit Function
        End If
        'giu100918 STATISTICHE EMAIL
        Dim strSQL As String = ""
        strSQL = "SELECT ID, Stato FROM EmailInviateT ORDER BY Stato"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    lblUtConn2.Text = "Totale E-mail: " & Format(ds.Tables(0).Rows.Count, "###,##0") & " di cui: "
                    Session("lblUtConn2") = lblUtConn2.Text.Trim
                    lblUtConn3.Text = "- Da inviare: " & Format(ds.Tables(0).Select("Stato=0").Length, "###,##0")
                    lblUtConn3.Text += " - Inviate: " & Format(ds.Tables(0).Select("Stato=1").Length, "###,##0")
                    lblUtConn3.Text += " - Invio Errato: " & Format(ds.Tables(0).Select("Stato=-1").Length, "###,##0")
                    Session("lblUtConn3") = lblUtConn3.Text.Trim
                    lblUtConn4.Text = "- Sollecito inviato: " & Format(ds.Tables(0).Select("Stato=2").Length, "###,##0")
                    If ds.Tables(0).Select("Stato=-2").Length > 0 Then
                        lblUtConn4.Text += " - Invio in corso Sollecito: " & Format(ds.Tables(0).Select("Stato=-2").Length, "###,##0")
                    End If
                    lblUtConn4.Text += " - Parz.Concluse: " & Format(ds.Tables(0).Select("Stato=3").Length, "###,##0")
                    If ds.Tables(0).Select("Stato=-3").Length > 0 Then
                        lblUtConn4.Text += " - Invio in corso Parz.Concluse: " & Format(ds.Tables(0).Select("Stato=-3").Length, "###,##0")
                    End If
                    Session("lblUtConn4") = lblUtConn4.Text.Trim
                    lblUtConn5.Text = "- Concluse: " & Format(ds.Tables(0).Select("Stato=99").Length, "###,##0")
                    If ds.Tables(0).Select("Stato=-99").Length > 0 Then
                        lblUtConn5.Text += " - Invio in corso Concluse: " & Format(ds.Tables(0).Select("Stato=-99").Length, "###,##0")
                    End If
                    lblUtConn5.Text += " - Annullate: " & Format(ds.Tables(0).Select("Stato=9").Length, "###,##0")
                    Session("lblUtConn5") = lblUtConn5.Text.Trim
                Else
                    lblUtConn2.Text = "Nessuna E-mail presente in archivio."
                    Session("lblUtConn2") = lblUtConn2.Text.Trim
                End If
            End If
        Catch Ex As Exception
            lblUtConn2.Text = "Errore caricamento E-mail."
            lblUtConn2.ToolTip = Ex.Message.Trim
            GetScadProdCons = False
            Exit Function
        End Try
        '---------------------------
    End Function
    Private Function LeggiCategorie(ByRef CodCategSel As String, ByRef NSel As Integer) As Boolean
        LeggiCategorie = False
        CodCategSel = ""
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            strSQL = "SELECT Codice FROM Categorie WHERE ISNULL(SelSc,0)<>0 AND ISNULL(InvioMailSc,0)<>0"
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 1) Then
                    NSel = ds.Tables(0).Rows.Count
                    'ok
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        CodCategSel &= ds.Tables(0).Rows(i).Item("Codice").ToString & ";"
                    Next
                    CodCategSel = CodCategSel.Substring(0, CodCategSel.Length - 1) 'rimuovo ultimo ;
                Else
                    lblScProdCons.ForeColor = SEGNALA_KO
                    lblScProdCons.Text = "Selezionare almeno 2 categorie."
                    Exit Function
                End If
            Else
                lblScProdCons.ForeColor = SEGNALA_KO
                lblScProdCons.Text = "Selezionare almeno 2 categorie."
                Exit Function
            End If
        Catch Ex As Exception
            lblScProdCons.ForeColor = SEGNALA_KO
            lblScProdCons.Text = "Errore LeggiCategorie: " & Ex.Message.Trim
            Exit Function
        End Try
        LeggiCategorie = True
    End Function
    Protected Function GetScadProdConsOK() As Boolean
        GetScadProdConsOK = False

        lblScProdCons.ForeColor = Drawing.Color.White

        Dim SelScGa As Boolean = chkSelScGa
        Dim SelScEl As Boolean = chkSelScEl
        Dim SelScBa As Boolean = chkSelScBa
        Dim SelTutteCatCli As Boolean = chkTutteCatCli
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli
        'TUTTI
        Dim SelCliSenzaMail As Boolean = False
        Dim SelCliConMail As Boolean = False
        Dim SelCliNoInvioEmail As Boolean = False
        '---------
        Dim DataScadenzaDA As DateTime
        Dim DataScadenzaA As DateTime

        'gestione categorie
        Dim DescCatCli As String = ""
        Dim CodCategoria As Integer = ddlCatCli
        'Sel.Multipla
        Dim CodcategSel As String = ""
        Dim NSel As Integer = 0
        '--
        Dim SWTratt As Integer = 0
        If ddlCatCli = -1 Then
            DescCatCli = "Tutte"
        ElseIf ddlCatCli = 0 Then
            DescCatCli = "Selezione multipla"
        ElseIf ddlCatCli > 0 Then
            DescCatCli = App.GetValoreFromChiave(ddlCatCli.ToString.Trim, Def.CATEGORIE, Session(ESERCIZIO))
        End If
        SWTratt = InStr(DescCatCli, " - ")
        If chkTutteCatCli = False Then
            If chkSelCategorie = True Then
                If LeggiCategorie(CodcategSel, NSel) = True Then
                    DescCatCli = "Selezione multipla"
                Else
                    lblScProdCons.ForeColor = SEGNALA_KO
                    lblScProdCons.Text = "Attenzione, selezionare almeno 2 categorie."
                    Exit Function
                End If
            ElseIf chkRaggrCatCli = True Then
                If SWTratt = 0 Then
                    DescCatCli = DescCatCli
                Else
                    DescCatCli = Left(DescCatCli, SWTratt - 1)
                End If
            Else
                DescCatCli = DescCatCli
            End If
        ElseIf chkSelCategorie = True Then
            lblScProdCons.ForeColor = SEGNALA_KO
            lblScProdCons.Text = "Attenzione, selezionare tutte le caterorie oppure Selezione multipla ma non entrambe."
            Exit Function
        Else
            DescCatCli = "Tutte"
        End If
        '-----

        DataScadenzaDA = CDate(txtDallaData.Trim).Date
        DataScadenzaA = CDate(txtAllaData.Trim).Date

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataScadenzaA.Year, DataScadenzaA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataScadenzaA.Year, DataScadenzaA.Month, DaysInMonth)
        DataScadenzaA = LastDayInMonthDate
        '-
        If DataScadenzaA < DataScadenzaDA Then
            lblScProdCons.ForeColor = SEGNALA_KO
            lblScProdCons.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Function
        End If
        '-
        'OK FILL GRIGLIA PASSANDO PARAMETRI
        Dim CountGrid As Integer = -1
        CountGrid = PopolaGridT(DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, "", "", _
                                         SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                                         chkSelCategorie, CodcategSel, _
                                         SelCliSenzaMail, SelCliConMail, SelCliNoInvioEmail)
        If CountGrid = -1 Then
            GetScadProdConsOK = False
            Exit Function
        End If
        lblScProdCons.Text = "N° Scadenze Clienti prodotti consumabili: " & Format(CountGrid, "###,##0") & " - dal " & Format(DataScadenzaDA, FormatoData) & " al " & Format(DataScadenzaA, FormatoData)
        Session("GetScadProdCons") = lblScProdCons.Text.Trim
        GetScadProdConsOK = True

    End Function
    Private Function PopolaGridT(ByVal DataScadenzaDA As DateTime, ByVal DataScadenzaA As DateTime, _
            ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, _
            ByVal Codice_CoGe As String, ByVal Codice_Art As String, _
            ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, _
            ByVal DescCatCli As String, ByVal CodCategoria As Integer, _
            ByVal SelCategorie As Boolean, ByVal CodCategSel As String, _
            ByVal CliSenzaMail As Boolean, ByVal CliConMail As Boolean, ByVal CliNoInvioEmail As Boolean) As Integer
        '-
        Dim CountGrid As Integer = -1
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        '-
        Try
            DsPrinWebDoc.Clear() 'giu260618 qui ok tanto dopo carica i dettagli
            If ClsPrint.BuildArtInstCliDett("", "", "", "", _
                DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, Codice_CoGe, Codice_Art, _
                DsPrinWebDoc, ObjReport, StrErrore, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                SelCategorie, CodCategSel, _
                CliSenzaMail, CliConMail, CliNoInvioEmail, False, False, False, "") = True Then 'giu261018 Ok CliConMailErr FALSE

                CountGrid = DsPrinWebDoc.ArticoliInstEmail.Select("").Length
                lblUtConn1.Text = "di cui Clienti senza E-mail: " & Format(DsPrinWebDoc.ArticoliInstEmail.Select("EmailInvio=''").Length, "###,##0")
                lblUtConn1.Text += " con E-mail: " & Format(DsPrinWebDoc.ArticoliInstEmail.Select("EmailInvio<>''").Length, "###,##0")
                lblUtConn1.Text += " Totale: " & Format(CountGrid, "###,##0")
                Session("lblUtConn1") = lblUtConn1.Text.Trim
            Else
                lblScProdCons.ForeColor = SEGNALA_KO
                lblScProdCons.Text = StrErrore
            End If
        Catch ex As Exception
            lblScProdCons.ForeColor = SEGNALA_KO
            lblScProdCons.Text = ex.Message
        End Try

        Return CountGrid 'ritorno n. elementi griglia
    End Function
    
    Private Sub LnkAlert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkAlert.Click
        Session("GetDDTByDataScadPag") = Nothing
        Session("GetNDocByDataScadPag") = Nothing
        Session("GetScadProdCons") = Nothing
        Session("GetCAByDataScadPag") = Nothing
        Session("GetCAByDataScadAtt") = Nothing
        Session("GetNDocByDataScadPagINCoGeRB") = Nothing
        If GetDDTByDataScadPag() = False Then
            Session("GetDDTByDataScadPag") = lblDDT1.Text
        End If
        GetNDocByDataScadPagINCoGeRB()
        If GetNDocByDataScadPag() = False Then
            Session("GetNDocByDataScadPag") = lblDoc10.Text
        End If
        If GetScadProdCons() = False Then
            Session("GetScadProdCons") = lblScProdCons.Text.Trim
        End If
        If GetCAByDataScadPag() = False Then
            Session("GetCAByDataScadPag") = lblSCContr1.Text
        End If
        If GetCAByDataScadAtt() = False Then
            Session("GetCAByDataScadAtt") = lblSCMan1.Text
        End If
        'GIU161220
        GetOpConnessi()
        ControllaDocOFStato5()
        ControllaDocStato9() '161220QUALSIASI TIPO DOCUMENTO PER ERRORI TIPO QTA' EVASA PIU' DELLA QTA' ORDINATA QUINDI DOPPIA FATTURAZIONE
    End Sub

    Private Function ControllaDocStato9() As Boolean
        LinkDocBloccati.Visible = False
        ControllaDocStato9 = True
        Dim strSQL As String = ""
        strSQL = "SELECT COUNT(IDDocumenti) AS TotDocStato9, Tipo_Doc, Numero FROM DocumentiT WHERE (StatoDoc = 9) GROUP BY Tipo_Doc, Numero"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    LinkDocBloccati.Visible = True
                End If
            End If
        Catch Ex As Exception
            ControllaDocStato9 = False
            LinkDocBloccati.Visible = True
            LinkDocBloccati.Text = "Errore controllo Documenti stato Errato (9): " + Ex.Message.Trim
            Exit Function
        End Try
    End Function

    Private Sub LinkDocBloccati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkDocBloccati.Click
        Try
            Response.Redirect("..\WebFormTables\WF_ElencoBloccati.aspx?labelForm=Elenco Documenti bloccati")
        Catch ex As Exception
            Response.Redirect("..\WebFormTables\WF_ElencoBloccati.aspx?labelForm=Elenco Documenti bloccati")
        End Try
    End Sub
End Class