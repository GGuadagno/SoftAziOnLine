Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class MasterPage
    Inherits System.Web.UI.MasterPage
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me

        Dim sIDAzienda As String = ""
        Dim sRifAzienda As String = ""
        Dim sEsercizio As String = ""
        Dim sTipoUtente As String = ""

        Dim UtenteConnesso As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Mid(Session.SessionID, 1, 50), -1, "", "", "", "") 'GIU150319 MID
        If (UtenteConnesso Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido. Sessione corrente: " & Session.SessionID)
            Exit Sub
        End If
        'giu280722 2 UTENTI con stesso browser non possono esserci  labelForm
        Dim UtenteOraConnesso As String = Session("UtenteOraConnesso")
        Try
            If (Session("UtenteOraConnesso") Is Nothing) Then
                UtenteOraConnesso = ""
            ElseIf String.IsNullOrEmpty(Session("UtenteOraConnesso")) Then
                UtenteOraConnesso = ""
            End If
            If UtenteOraConnesso.Trim <> "" Then
                If UtenteConnesso.NomeOperatore.Trim <> UtenteOraConnesso.Trim Then
                    Dim OpSys As New Operatori
                    If OpSys.DelOperatoreConnesso(UtenteOraConnesso.Trim, Session(CSTCODDITTA), NomeModulo) = True Then
                        'OK cancellato cosi le altre sessioni saranno sconnesse alla prima operazione lato server
                    Else
                        'MessageLabel.Text = "Errore: cancella operatore connesso; chiudere tutte le finetre attive e riprovare."
                        Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Cancella operatore connesso; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Non è consentito usare lo stesso Browser con 2 Utenti diversi. chiudere tutte le finetre attive e riprovare." & UtenteOraConnesso + " " + UtenteConnesso.NomeOperatore.Trim)
                    Exit Sub
                End If
            Else
                Session("UtenteOraConnesso") = UtenteConnesso.NomeOperatore.Trim 'Prima volta
            End If
        Catch ex As Exception
            'nulla
        End Try
        '----------------------------------
        If (String.IsNullOrEmpty(UtenteConnesso.CodiceDitta) Or String.IsNullOrEmpty(UtenteConnesso.Esercizio)) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: IdAzienda/Esercizio non validi.")
            Exit Sub
        End If
        sIDAzienda = UtenteConnesso.CodiceDitta
        If IsNothing(sIDAzienda) Then
            sIDAzienda = ""
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: IdAzienda non valido.")
            Exit Sub
        End If
        If Not IsNumeric(sIDAzienda) Then
            sIDAzienda = ""
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: IdAzienda non valido.")
            Exit Sub
        End If
        Session(CSTCODDITTA) = sIDAzienda
        '-
        If (String.IsNullOrEmpty(UtenteConnesso.Azienda) Or String.IsNullOrEmpty(UtenteConnesso.Azienda)) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: Azienda non valida.")
            Exit Sub
        End If
        sRifAzienda = UtenteConnesso.Azienda
        If IsNothing(sRifAzienda) Then
            sRifAzienda = ""
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: Azienda non valida.")
            Exit Sub
        End If
        If sRifAzienda = "" Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: Azienda non valida.")
            Exit Sub
        End If
        '-
        sEsercizio = UtenteConnesso.Esercizio
        If IsNothing(sEsercizio) Then
            sEsercizio = ""
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: Esercizio non valido.")
            Exit Sub
        End If
        If Not IsNumeric(sEsercizio) Then
            sEsercizio = ""
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: Esercizio non valido.")
            Exit Sub
        End If
        'giu240212
        If (Not UtenteConnesso.SessionID.Equals(Mid(Session.SessionID, 1, 50))) Then 'GIU150319 MID
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione non valida: ID Sessione diversa dalla sessione corrente.")
            Exit Sub
        End If
        'GIU150319 MI SA CHE NON SERVE IN QUANTO LA PRIMA ISTRUZIONE LEGGE PROPRIO GLI OPERATORI CONNESSI PER SESSIONID, ANCHE IL TEST SUBITO QUI PRECEDENTE 
        'NON DOVREBBE MAI VERIFICARSI
        ' ''If SessionUtility.CTROpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo) = False Then
        ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Operatore NON connesso dal sistema; chiudere tutte le finetre attive e riprovare.")
        ' ''    Exit Sub
        ' ''End If
        '----------------------------------
        sTipoUtente = UtenteConnesso.Tipo
        Session(CSTTIPOUTENTE) = sTipoUtente 'giu080312
        Session(ESERCIZIO) = sEsercizio


        Dim setMyNNAAAA As New It.SoftAzi.SystemFramework.ApplicationConfiguration
        setMyNNAAAA.setNNAAAA = sIDAzienda & sEsercizio
        'giu080620 GIU090620 NON VA BENE, LA PAGINA NON SI ADATTA AL TABLET
        ' ''If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Or _
        ' ''       (sTipoUtente.Equals(CSTTECNICO)) Or _
        ' ''       (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
        ' ''    Menu.Height = Unit.Pixel(22)
        ' ''    Menu.DynamicMenuItemStyle.Height = Unit.Pixel(20)
        ' ''    LnkMenu.Visible = True : LnkLogOut.Visible = True
        ' ''Else
        ' ''    form1.Style.Item("width") = "980px"
        ' ''    Menu.DynamicMenuItemStyle.Height = Unit.Pixel(40)
        ' ''    Menu.Height = Unit.Pixel(50)
        ' ''    Menu.Font.Size = FontUnit.Large
        ' ''    labelIdentificaUtente.Height = Unit.Pixel(40)
        ' ''    LnkMenu.Visible = False : LnkMenu.Height = Unit.Pixel(40)
        ' ''    LnkLogOut.Width = Unit.Pixel(50) : LnkLogOut.Height = Unit.Pixel(40) : LnkLogOut.BackColor = Drawing.Color.DarkRed
        ' ''    ': LnkLogOut.Visible = False
        ' ''End If
        'GIU080312
        Dim strErrore As String = "" : Dim strValore As String = ""
        'giu301220 mettere lo sw perattivarlo -devomodificare lavalorizzazione prima
        If String.IsNullOrEmpty(Session(CSTLEAD)) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTLEAD, strValore, strErrore) = True Then
                Session(CSTLEAD) = SWSI
            Else
                Session(CSTLEAD) = SWNO
            End If
        ElseIf Session(CSTLEAD).ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTLEAD, strValore, strErrore) = True Then
                Session(CSTLEAD) = SWSI
            Else
                Session(CSTLEAD) = SWNO
            End If
        End If
        'giu050722 per visulizzare panale Sintetico/Analitico per le stampe dove è disabilitato
        'WUC_pREVcLIENTEoRDINEaG
        If String.IsNullOrEmpty(Session(SWSINTANAL)) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, SWSINTANAL, strValore, strErrore) = True Then
                Session(SWSINTANAL) = SWSI
            Else
                Session(SWSINTANAL) = SWNO
            End If
        ElseIf Session(SWSINTANAL).ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, SWSINTANAL, strValore, strErrore) = True Then
                Session(SWSINTANAL) = SWSI
            Else
                Session(SWSINTANAL) = SWNO
            End If
        End If
        '---------
        'GIU150319
        If String.IsNullOrEmpty(Session("mnuCSVFIFO")) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, "mnuCSVFIFO", strValore, strErrore) = True Then
                Session("mnuCSVFIFO") = SWSI
                'giu210814 mnuCSVFIFO è il MASTER dopo il controllo utente
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'OK
                    Else
                        Session("mnuCSVFIFO") = SWNO
                    End If
                Else
                    Session("mnuCSVFIFO") = SWNO
                End If
            Else
                Session("mnuCSVFIFO") = SWNO
                Session(ERRORE) = strErrore
            End If
        ElseIf Session("mnuCSVFIFO").ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, "mnuCSVFIFO", strValore, strErrore) = True Then
                Session("mnuCSVFIFO") = SWSI
                'giu210814 mnuCSVFIFO è il MASTER dopo il controllo utente
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'OK
                    Else
                        Session("mnuCSVFIFO") = SWNO
                    End If
                Else
                    Session("mnuCSVFIFO") = SWNO
                End If
            Else
                Session("mnuCSVFIFO") = SWNO
                Session(ERRORE) = strErrore
            End If
        Else
            'OK è VALORIZZATO COSI NON RALLENTO L'ACCESSO
        End If
        'giu150420
        If String.IsNullOrEmpty(Session("mnuMargFor")) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, "mnuMargFor", strValore, strErrore) = True Then
                Session("mnuMargFor") = SWSI
                'giu210814 mnuCSVFIFO è il MASTER dopo il controllo utente
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'OK
                    Else
                        Session("mnuMargFor") = SWNO
                    End If
                Else
                    Session("mnuMargFor") = SWNO
                End If
            Else
                Session("mnuMargFor") = SWNO
                Session(ERRORE) = strErrore
            End If
        ElseIf Session("mnuMargFor").ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, "mnuMargFor", strValore, strErrore) = True Then
                Session("mnuMargFor") = SWSI
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'OK
                    Else
                        Session("mnuMargFor") = SWNO
                    End If
                Else
                    Session("mnuMargFor") = SWNO
                End If
            Else
                Session("mnuMargFor") = SWNO
                Session(ERRORE) = strErrore
            End If
        Else
            'OK è VALORIZZATO COSI NON RALLENTO L'ACCESSO
        End If
        '-
        'giu290722 
        If String.IsNullOrEmpty(Session(CSTGESTSPEDVETT)) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTGESTSPEDVETT, strValore, strErrore) = True Then
                Session(CSTGESTSPEDVETT) = SWSI
                Session(CSTCKSPEDVETT) = SWSI
                'giu210814 mnuCSVFIFO è il MASTER dopo il controllo utente
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If strValore.Trim = "*" Then
                        'tutti
                    ElseIf InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'solo alcuni
                    Else
                        Session(CSTGESTSPEDVETT) = SWNO
                    End If
                Else
                    Session(CSTGESTSPEDVETT) = SWNO
                End If
            Else
                Session(CSTGESTSPEDVETT) = SWNO
                Session(CSTCKSPEDVETT) = SWNO
                Session(ERRORE) = strErrore
            End If
        ElseIf Session(CSTGESTSPEDVETT).ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTGESTSPEDVETT, strValore, strErrore) = True Then
                Session(CSTGESTSPEDVETT) = SWSI
                Session(CSTCKSPEDVETT) = SWSI
                If (sTipoUtente.Equals(CSTTECNICO)) Then
                    'sempre
                ElseIf Not String.IsNullOrEmpty(strValore) Then
                    If strValore.Trim = "*" Then
                        'tutti
                    ElseIf InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        'solo alcuni
                    Else
                        Session(CSTGESTSPEDVETT) = SWNO
                    End If
                Else
                    Session(CSTGESTSPEDVETT) = SWNO
                End If
            Else
                Session(CSTGESTSPEDVETT) = SWNO
                Session(CSTCKSPEDVETT) = SWNO
                Session(ERRORE) = strErrore
            End If
        Else
            'OK è VALORIZZATO COSI NON RALLENTO L'ACCESSO
        End If
        'giu060423 se attivo anche solo per un Operatore, devo comunque controllare 
        If String.IsNullOrEmpty(Session(CSTCKSPEDVETT)) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTGESTSPEDVETT, strValore, strErrore) = True Then
                Session(CSTCKSPEDVETT) = SWSI
            Else
                Session(CSTCKSPEDVETT) = SWNO
            End If
        ElseIf Session(CSTCKSPEDVETT).ToString.Trim = "" Then
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTGESTSPEDVETT, strValore, strErrore) = True Then
                Session(CSTCKSPEDVETT) = SWSI
            Else
                Session(CSTCKSPEDVETT) = SWNO
            End If
        End If
        '-
        'giu040212 labelIdentificaUtente.Text = "Utente: " & Utente.Nome & " (" & sRifAzienda & " - Esercizio: " & sEsercizio & ")"
        labelIdentificaUtente.Text = UtenteConnesso.NomeOperatore & " (" & sRifAzienda & " - " & sEsercizio & ")"
        'GIU150319
        If String.IsNullOrEmpty(Session(ERRORE)) Then
            'OK NESSUN ERRORE IN CORSO
            Session(ERRORE) = ""
        ElseIf Session(ERRORE).ToString.Trim = "" Then
            'OK NESSUN ERRORE IN CORSO
        Else
            labelIdentificaUtente.BackColor = Drawing.Color.Red
            labelIdentificaUtente.Text = Session(ERRORE).ToString.Trim
            Session(ERRORE) = "" ' SOLO UNA VOLTA
        End If
        'giu060224
        Dim SWSviluppo As String = ConfigurationManager.AppSettings("debug")
        Dim SWDebug As Boolean = False
        If Not String.IsNullOrEmpty(SWSviluppo) Then
            If SWSviluppo.Trim.ToUpper = "TRUE" Then
                SWDebug = True
            End If
        End If
        '-----------
        If sIDAzienda = "00" Or SWDebug = True Then 'giu060224 'GIU160418
            labelIdentificaUtente.BackColor = Drawing.Color.Red
        End If
        Session(CSTUTENTE) = UtenteConnesso.NomeOperatore
        Session(CSTAZIENDARPT) = sRifAzienda & " - Esercizio: " & sEsercizio

        labelForm.Text = Request.QueryString("labelForm")
        If InStr(labelForm.Text.Trim.ToUpper, "ACCONTO/SALDO") > 0 Then 'GIU290419
            Session(CSTFCACSA) = SWSI
        Else
            Session(CSTFCACSA) = SWNO
        End If
        If InStr(labelForm.Text.Trim.ToUpper, "ERRORE") > 0 Then
            labelForm.ForeColor = Drawing.Color.Red
            If InStr(labelForm.Text.Trim.ToUpper, "<%----%>") > 0 Then
                Session(CSTSALVADB) = False
                Session(F_ANAGRAGENTI_APERTA) = False
                Session(F_ANAGRNAZIONI_APERTA) = False
                Session(F_BANCHEIBAN_APERTA) = False
                Session(F_ANAGRCAPIGR_APERTA) = False
                Session(F_ANAGRCATEGORIEART_APERTA) = False
                Session(F_ANAGRCATEGORIE_APERTA) = False
                Session(F_ANAGRLINEEART_APERTA) = False
                Session(F_ANAGRTIPOCODART_APERTA) = False
                Session(F_ANAGRMISURE_APERTA) = False
                Session(F_ANAGRVETTORI_APERTA) = False
                Session(F_ANAGRZONE_APERTA) = False
                Session(F_PAGAMENTI_APERTA) = False
                Session(F_SEL_ARTICOLO_APERTA) = False
                Session(F_ANAGR_PROVV_APERTA) = False
                Session(F_ANAGRCLIFOR_APERTA) = False
                Session(F_FORNSEC_APERTA) = False
                Session(F_SCELTALISTINI_APERTA) = False
                Session(F_SCELTAMOVMAG_APERTA) = False
                Session(F_EVASIONEPARZ_APERTA) = False
                Session(F_ELENCO_CLIFORN_APERTA) = False
                Session(F_CLI_RICERCA) = False
                Session(F_FOR_RICERCA) = False
                Session(LISTINI_DA_AGG) = False
                Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
                Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
                Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
                Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
                Session(OSART_F_ELENCO_ART1_APERTA) = False
                Session(OSART_F_ELENCO_ART2_APERTA) = False
                Session(F_SCELTASPED_APERTA) = False
                Session(F_DOCCOLL_APERTA) = False
                Session(F_ELENCO_APERTA) = False
                Session(F_CAMBIOSTATO_APERTA) = False
                Session(F_ANAGRCAUSNONEV_APERTA) = False
                Session(F_ANAGR_PROVV_APERTA) = False
                Session(F_ANAGRCLIFOR_APERTA) = False
                Session(F_ANAGRVETTORI_APERTA) = False
                Session(F_ANAGRAGENTI_APERTA) = False
                Session(F_ANAGRCAPIGR_APERTA) = False
                Session(F_ANAGRZONE_APERTA) = False
                Session(F_ANAGRTIPOFATT_APERTA) = False
                Session(F_ANAGRCATEGORIE_APERTA) = False
                Session(F_ANAGRCATEGORIEART_APERTA) = False
                Session(F_ANAGRLINEEART_APERTA) = False
                Session(F_ANAGRTIPOCODART_APERTA) = False
                Session(F_ANAGRMISURE_APERTA) = False
                Session(F_PAGAMENTI_APERTA) = False
                Session(F_SEL_ARTICOLO_APERTA) = False
                Session(F_BANCHEIBAN_APERTA) = False
                Session(F_FORNSEC_APERTA) = False
                Session(F_SCELTALISTINI_APERTA) = False
                Session(F_ANAGRREP_APERTA) = False
                Session(F_ANAGRSCAFFALI_APERTA) = False
                'giu240112 Carico di Magazzino da OF
                Session(F_SCELTAMOVMAG_APERTA) = False
                Session(F_EVASIONEPARZ_APERTA) = False
                Session(F_ELENCO_CLIFORN_APERTA) = False
                Session(LISTINI_DA_AGG) = False
                'giu140612 GESTIONE INVENTARIO
                Session(F_MODIFICASCHEDAIN_APERTA) = False
                Session(L_SCHEDAIN_DA_AGG) = False
                'giu220612 GESTIONE RESI
                Session(F_RESODAC_APERTA) = False
                Session(F_RESOAF_APERTA) = False
                Session(F_NCAAC_APERTA) = False
                Session(L_RESODACF) = False
                Session(F_GESTIONETESTIEMAIL_APERTA) = False    'gestione testi email sim040618
                Session(F_ANAGRRESPAREA_APERTA) = False
                Session(F_ANAGRRESPVISITE_APERTA) = False
                Session(F_MAGAZZINI_APERTA) = False
                Session(F_LEAD_APERTA) = False
            End If
        Else
            labelForm.ForeColor = Drawing.Color.Black
            If Mid(UCase(labelForm.Text.Trim), 1, 16) = "GESTIONE TABELLE" Then
                labelForm.Text = "Menu principale"
            End If
        End If
        
        If (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            Menu.DataSourceID = SiteMapDataSourceAdmin.ID
        ElseIf (sTipoUtente.Equals(CSTUFFICIO_AMMINISTRATIVO)) Then
            Menu.DataSourceID = SiteMapDataSourceAzienda.ID
        ElseIf (sTipoUtente.Equals(CSTTECNICO)) Then
            Menu.DataSourceID = SiteMapDataSourceTecnico.ID
        ElseIf (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Menu.DataSourceID = SiteMapMagazzino.ID
        ElseIf (sTipoUtente.Equals(CSTACQUISTI)) Then
            Menu.DataSourceID = SiteMapAcquisti.ID
        ElseIf (sTipoUtente.Equals(CSTVENDITE)) Then
            Menu.DataSourceID = SiteMapVendite.ID
        Else
            Menu.DataSourceID = SiteMapDataSourceAzienda.ID
        End If

        Session("Menu") = Menu

        If (Not IsPostBack) Then
            Dim OpSys As New Operatori
            If OpSys.UpdOperatoriDataOraUltAzione(UtenteConnesso.NomeOperatore) = False Then
                labelForm.Text += " Errore aggiornamento operatore!!"
                labelForm.ForeColor = Drawing.Color.Red
            End If
            OpSys = Nothing
            If UCase(labelForm.Text.Trim) = UCase("Menu principale") Or _
                Session(CSTVISUALMENU) = SWSI Then
                SiteMapDataSourceAdmin.ShowStartingNode = False
                SiteMapDataSourceAzienda.ShowStartingNode = False
                SiteMapDataSourceTecnico.ShowStartingNode = False
                SiteMapMagazzino.ShowStartingNode = False
                SiteMapAcquisti.ShowStartingNode = False
                SiteMapVendite.ShowStartingNode = False
                Menu.Enabled = True
                ' ''LnkLogOut.Visible = True
                LnkMenu.BorderStyle = BorderStyle.Inset
            End If
        End If
        'giu220113
        If labelForm.Text.Trim = "Anagrafica clienti (Scheda CoGe/E.C.)" Then
            Session(CSTNOBACK) = 1 'giu230113
            Session(CSTRitornoDaStampa) = True
            Session("StampaSCEC") = 1 'giu240113
            SiteMapDataSourceAdmin.ShowStartingNode = True
            SiteMapDataSourceAzienda.ShowStartingNode = True
            SiteMapDataSourceTecnico.ShowStartingNode = True
            SiteMapMagazzino.ShowStartingNode = True
            SiteMapAcquisti.ShowStartingNode = True
            SiteMapVendite.ShowStartingNode = True
            Menu.Enabled = False
            ' ''LnkLogOut.Visible = False 'giu230113
            LnkMenu.BorderStyle = BorderStyle.Outset
            ' ''LnkMenu.Visible = False 'giu230113
        ElseIf labelForm.Text.Trim = "Anagrafica clienti" Then 'Anagrafica clienti
            Session(CSTNOBACK) = 0 'giu230113
            Session("StampaSCEC") = 0
        End If
        'giu080620
        If (UtenteConnesso.Tipo.Equals(CSTVENDITE)) Then
            Exit Sub
        End If
        '---------
        'giu060412 CONTROLLO MODIFICHE TABELLE DA COGE 
        Dim ElencoTabAgg As String = "" : Dim ErrorMsg As String = ""
        If (Not SessionUtility.GetAggTabCG(Session(ESERCIZIO), ElencoTabAgg, ErrorMsg)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Caricamento Dati", _
                String.Format("Errore nel caricamento Tabelle CoGe, contattare l'amministratore di sistema. <br> La sessione utente verrà chiusa. Errore: {0}", ErrorMsg), _
                WUC_ModalPopup.TYPE_INFO)
            SessionUtility.LogOutUtente(Session, Response)
            Exit Sub
        ElseIf (Not SessionUtility.GetAggTabAZ(Session(ESERCIZIO), ElencoTabAgg, ErrorMsg)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Caricamento Dati", _
                String.Format("Errore nel caricamento Tabelle Aziendale, contattare l'amministratore di sistema. <br> La sessione utente verrà chiusa. Errore: {0}", ErrorMsg), _
                WUC_ModalPopup.TYPE_INFO)
            SessionUtility.LogOutUtente(Session, Response)
            Exit Sub
        ElseIf (Not SessionUtility.GetAggTabSC(Session(ESERCIZIO), ElencoTabAgg, ErrorMsg)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Caricamento Dati", _
                String.Format("Errore nel caricamento Tabelle Scadenzario, contattare l'amministratore di sistema. <br> La sessione utente verrà chiusa. Errore: {0}", ErrorMsg), _
                WUC_ModalPopup.TYPE_INFO)
            SessionUtility.LogOutUtente(Session, Response)
            Exit Sub
        ElseIf ElencoTabAgg.Trim <> "" Then 'ok se sono state aggiornate le tabelle INTERESSARE AZZERO LA VARIABILE DI SESSIONE
            Session("GetScadProdCons") = ""
        End If
        'GIU110512 Controllo se ci sono dei documenti di tipo CM in stato '5' CARICO IN CORSO
        'INSERITO IN MENU E VISUALIZZO IL MESSAGGIO SUL MENU
    End Sub

    Protected Overrides Sub AddedControl(ByVal control As Control, ByVal index As Integer)
        If (Request.ServerVariables("http_user_agent").IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
            Me.Page.ClientTarget = "uplevel"
        End If

        MyBase.AddedControl(control, index)
    End Sub

    Private Sub LnkLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkLogOut.Click

        'giu220113
        labelForm.Text = Request.QueryString("labelForm")
        If labelForm.Text.Trim = "Anagrafica clienti (Scheda CoGe/E.C.)" Then
            Session("StampaSCEC") = 0 'giu220321
            SiteMapDataSourceAdmin.ShowStartingNode = True
            SiteMapDataSourceAzienda.ShowStartingNode = True
            SiteMapDataSourceTecnico.ShowStartingNode = True
            SiteMapMagazzino.ShowStartingNode = True
            SiteMapAcquisti.ShowStartingNode = True
            SiteMapVendite.ShowStartingNode = True
            Menu.Enabled = False
            ' ''LnkLogOut.Visible = False
            LnkMenu.BorderStyle = BorderStyle.Outset
            'giu130715
            ' ''Response.Redirect("..\Login.aspx?SessioneScaduta=TERMINE Anagrafica clienti (Scheda CoGe/E.C.)")
            Response.Redirect("..\GenericErrorPage.aspx?labelForm=TERMINE Anagrafica clienti (Scheda CoGe/E.C.)")
            Exit Sub
        End If
        '----------
        If Session(SWOP) <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPCLI) <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOPCLI), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        SessionUtility.LogOutUtente(Session, Response)
    End Sub
    'GIU131011 magari rallenta tanto è una funzione temporanea
    'giu070312 Costo del venduto - WebFormTables\WF_ValMagCostoVenduto
    Dim ContaMenu As Integer = 0 'giu320222
    Private Sub Menu_MenuItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles Menu.MenuItemDataBound
        'visualizzare su abilitazionu MNU
        If String.IsNullOrEmpty(Session("mnuCSVFIFO")) Then
            Session("mnuCSVFIFO") = SWNO
        End If
        If Session("mnuCSVFIFO") <> SWSI Then
            If (e.Item.NavigateUrl.ToString.IndexOf("WF_ValMagCostoVenduto", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
                e.Item.Text = "" : e.Item.ToolTip = ""
            End If
            If (e.Item.NavigateUrl.ToString.IndexOf("WF_StatFatturatoMese", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
                e.Item.Text = "" : e.Item.ToolTip = ""
            End If
            If (e.Item.NavigateUrl.ToString.IndexOf("WF_RicollegaVisteCoGe", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
                e.Item.Text = "" : e.Item.ToolTip = ""
            End If
        End If
        'GIU150420
        If String.IsNullOrEmpty(Session("mnuMargFor")) Then
            Session("mnuMargFor") = SWNO
        End If
        If Session("mnuMargFor") <> SWSI Then
            If (e.Item.NavigateUrl.ToString.IndexOf("WF_FatturatoClientiMargineFor", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
               e.Item.Text = "" : e.Item.ToolTip = ""
            End If
        End If
        'giu301220
        If String.IsNullOrEmpty(Session(CSTLEAD)) Then
            Session(CSTLEAD) = SWNO
        End If
        If Session(CSTLEAD) = SWNO Then
            If (e.Item.NavigateUrl.ToString.IndexOf("Lead Source", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
                e.Item.Text = "" : e.Item.ToolTip = ""
            End If
        End If
    End Sub

    Private Sub LnkMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkMenu.Click
        'giu220113
        labelForm.Text = Request.QueryString("labelForm")
        If labelForm.Text.Trim = "Anagrafica clienti (Scheda CoGe/E.C.)" Then
            Session("StampaSCEC") = 0 'giu220321
            SiteMapDataSourceAdmin.ShowStartingNode = True
            SiteMapDataSourceAzienda.ShowStartingNode = True
            SiteMapDataSourceTecnico.ShowStartingNode = True
            SiteMapMagazzino.ShowStartingNode = True
            SiteMapAcquisti.ShowStartingNode = True
            SiteMapVendite.ShowStartingNode = True
            Menu.Enabled = False
            LnkMenu.BorderStyle = BorderStyle.Outset
            'giu130715
            ' ''Response.Redirect("..\Login.aspx?SessioneScaduta=TERMINE Anagrafica clienti (Scheda CoGe/E.C.)")
            Response.Redirect("..\GenericErrorPage.aspx?labelForm=TERMINE Anagrafica clienti (Scheda CoGe/E.C.)")
            Exit Sub
        End If
        '----------
        If Session(SWOP) <> "" And LnkMenu.BorderStyle = BorderStyle.Outset Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf Session(SWOP) <> "" Then 'GIU020712
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu220113
        If Session(SWOPCLI) <> "" And LnkMenu.BorderStyle = BorderStyle.Outset Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOPCLI), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf Session(SWOPCLI) <> "" Then 'GIU020712
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOPCLI), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
       
        If LnkMenu.BorderStyle = BorderStyle.Outset Then
            SiteMapDataSourceAdmin.ShowStartingNode = False
            SiteMapDataSourceAzienda.ShowStartingNode = False
            SiteMapDataSourceTecnico.ShowStartingNode = False
            SiteMapMagazzino.ShowStartingNode = False
            SiteMapAcquisti.ShowStartingNode = False
            SiteMapVendite.ShowStartingNode = False
            Menu.Enabled = True
            ' ''LnkLogOut.Visible = True
            LnkMenu.BorderStyle = BorderStyle.Inset
        ElseIf UCase(labelForm.Text.Trim) = UCase("Menu principale") Then
            SiteMapDataSourceAdmin.ShowStartingNode = False
            SiteMapDataSourceAzienda.ShowStartingNode = False
            SiteMapDataSourceTecnico.ShowStartingNode = False
            SiteMapMagazzino.ShowStartingNode = False
            SiteMapAcquisti.ShowStartingNode = False
            SiteMapVendite.ShowStartingNode = False
            Menu.Enabled = True
            ' ''LnkLogOut.Visible = True
            LnkMenu.BorderStyle = BorderStyle.Inset
        Else
            SiteMapDataSourceAdmin.ShowStartingNode = True
            SiteMapDataSourceAzienda.ShowStartingNode = True
            SiteMapDataSourceTecnico.ShowStartingNode = True
            SiteMapMagazzino.ShowStartingNode = True
            SiteMapAcquisti.ShowStartingNode = True
            SiteMapVendite.ShowStartingNode = True
            Menu.Enabled = False
            ' ''LnkLogOut.Visible = False
            LnkMenu.BorderStyle = BorderStyle.Outset
        End If
    End Sub
    
End Class