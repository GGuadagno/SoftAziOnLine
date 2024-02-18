Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework

Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Partial Public Class WF_MenuStatisPR
    Inherits System.Web.UI.Page
    Private InizialiUT As String = ""
    Private CodiceDitta As String = ""
    Dim composeChiave As String = ""
    Dim myObject As Object = Nothing
    Dim UtenteConnesso As OperatoreConnessoEntity
    Dim SWGetUltSess As Boolean = False
    Private mylabelForm As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        Session(CSTChiamatoDa) = "WF_MenuCA.aspx?labelForm=Menu Gestione CONTRATTI"
        lblDataOdierna.Text = Format(Now, "dddd d MMMM yyyy, HH:mm")
        '-
        Try
            composeChiave = String.Format("{0}_{1}", _
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            GetObjectToCache(composeChiave, myObject)
            SWGetUltSess = myObject
            '-
            myObject = False
            composeChiave = String.Format("{0}_{1}", _
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        Catch ex As Exception
            SWGetUltSess = False
            myObject = False
            composeChiave = String.Format("{0}_{1}", _
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        End Try
        UtenteConnesso = SessionUtility.GetLogOnUtente("", "", Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo, Mid(Session.SessionID, 1, 50), -1, "", "", "", "")
        If (UtenteConnesso Is Nothing) Then
            composeChiave = String.Format("{0}_{1}", _
            "UtenteConnesso", Mid(Request.UserHostAddress.Trim, 1, 50))
            GetObjectToCache(composeChiave, myObject)
            UtenteConnesso = myObject
            If (UtenteConnesso Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta Oltre 1 ora: utente non valido.(Load)")
                Exit Sub
            End If
        Else
            myObject = UtenteConnesso
            composeChiave = String.Format("{0}_{1}", _
            "UtenteConnesso", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        End If
        myObject = SWGetUltSess
        composeChiave = String.Format("{0}_{1}", _
        "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
        SetObjectToCache(composeChiave, myObject)
        '-----------------------------------------------------
        Dim sIDAzienda As String = ""
        Dim sRifAzienda As String = ""
        Dim sEsercizio As String = ""
        Dim sTipoUtente As String = ""

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
        CodiceDitta = sIDAzienda
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
        '-
        labelIdentificaUtente.Text = UtenteConnesso.NomeOperatore
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
        If sIDAzienda = "00" Then 'GIU160418
            labelIdentificaUtente.BackColor = Drawing.Color.Red
        End If
        Session("CodiceUTENTE") = UtenteConnesso.Codice
        Session(CSTUTENTE) = UtenteConnesso.NomeOperatore
        Session(CSTAZIENDARPT) = sRifAzienda & " - Esercizio: " & sEsercizio
        '----------------------------------
        sTipoUtente = UtenteConnesso.Tipo
        Session(CSTTIPOUTENTE) = sTipoUtente 'giu080312
        Session(ESERCIZIO) = sEsercizio
        Dim setMyNNAAAA As New It.SoftAzi.SystemFramework.ApplicationConfiguration
        setMyNNAAAA.setNNAAAA = sIDAzienda & sEsercizio
        'CONNESSIONE AL DB
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '''SqlDSLead.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        mylabelForm = Request.QueryString("labelForm")
        If InStr(mylabelForm.Trim.ToUpper, "AGENTE") > 0 Then
            PanelSelezionaAgente.Visible = True
            '''PanelLead.Visible = False
            PanelSintAnal.Visible = False
            If Not String.IsNullOrEmpty(Session(SWSINTANAL)) Then
                If Session(SWSINTANAL) = SWSI Then
                    PanelSintAnal.Visible = True
                End If
            End If
            PanelOrdinamento.Visible = True
        Else
            PanelSelezionaAgente.Visible = False
            '''PanelLead.Visible = True
            PanelSintAnal.Visible = True
            PanelOrdinamento.Visible = False
        End If
        '-
        If (Not IsPostBack) Then
            SetLnk()
            'GIU030722 MEM.DATI QUANDO RITORNA
            Dim rkFormCampi As StrFormCampi = Nothing
            Dim arrFormDati As ArrayList = Nothing
            arrFormDati = New ArrayList
            If InStr(mylabelForm.Trim.ToUpper, "AGENTE") > 0 Then
                arrFormDati = Session(Me.ID.Trim + "AG")
            Else
                arrFormDati = Session(Me.ID.Trim + "LS")
            End If
            '------------------------------------------------------------
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            txtDataDa.Text = "01/01/2012" 'GIU230123 TEL ZIBORDI "01/01/" & Session(ESERCIZIO) ' Format(Val(Session(ESERCIZIO)) - 1, "0000")
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            chkTuttiClienti.Checked = True
            txtCodCli1.Enabled = False
            txtCodCli2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            '-
            chkTuttiAgenti.AutoPostBack = False
            chkTuttiAgenti.Checked = False
            ddlAgenti.Enabled = True
            If Not (arrFormDati Is Nothing) Then
                If GetValoreFromChiaveFC(chkTuttiAgenti.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    chkTuttiAgenti.Checked = rkFormCampi.TrueFalse
                    ddlAgenti.Enabled = Not chkTuttiAgenti.Checked
                    If chkTuttiAgenti.Checked = False Then
                        If GetValoreFromChiaveFC(ddlAgenti.ID.Trim, arrFormDati, rkFormCampi) = True Then
                            PosizionaItemDDL(rkFormCampi.Valore.Trim, ddlAgenti)
                        End If
                    End If
                End If
            End If
            chkTuttiAgenti.AutoPostBack = True
            '-
            '''chkTuttiLead.AutoPostBack = False
            '''chkTuttiLead.Checked = False
            '''DDLLead.Enabled = True
            '''If Not (arrFormDati Is Nothing) Then
            '''    If GetValoreFromChiaveFC(chkTuttiLead.ID.Trim, arrFormDati, rkFormCampi) = True Then
            '''        chkTuttiLead.Checked = rkFormCampi.TrueFalse
            '''        DDLLead.Enabled = Not chkTuttiLead.Checked
            '''        If chkTuttiLead.Checked = False Then
            '''            If GetValoreFromChiaveFC(DDLLead.ID.Trim, arrFormDati, rkFormCampi) = True Then
            '''                PosizionaItemDDL(rkFormCampi.Valore.Trim, DDLLead)
            '''            End If
            '''        End If
            '''    End If
            '''End If
            '''chkTuttiLead.AutoPostBack = True
            'giu040722 giu150622
            chkTutteRegioni.AutoPostBack = False
            chkTutteRegioni.Checked = True
            ddlRegioni.AutoPostBack = False
            ddlRegioni.SelectedIndex = 0
            ddlRegioni.Enabled = False
            ddlProvince.SelectedIndex = 0
            ddlProvince.Enabled = False
            If Not (arrFormDati Is Nothing) Then
                If GetValoreFromChiaveFC(chkTutteRegioni.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    chkTutteRegioni.Checked = rkFormCampi.TrueFalse
                    ddlRegioni.Enabled = Not chkTutteRegioni.Checked
                    ddlProvince.Enabled = Not chkTutteRegioni.Checked
                    If chkTutteRegioni.Checked = False Then
                        If GetValoreFromChiaveFC(ddlRegioni.ID.Trim, arrFormDati, rkFormCampi) = True Then
                            PosizionaItemDDL(rkFormCampi.Valore.Trim, ddlRegioni)
                        End If
                        If GetValoreFromChiaveFC(ddlProvince.ID.Trim, arrFormDati, rkFormCampi) = True Then
                            PosizionaItemDDL(rkFormCampi.Valore.Trim, ddlProvince)
                            ddlProvince.Enabled = rkFormCampi.Abilitato
                        End If
                    End If
                End If
            End If
            ddlRegioni.AutoPostBack = True
            chkTutteRegioni.AutoPostBack = True
            '-
            rbtnAnalitico.AutoPostBack = False
            rbtnAnalitico.Checked = True
            '-
            rbtnSintetico.AutoPostBack = False
            rbtnSintetico.Checked = False
            'giu010722
            rbtnOrdAgPrevCli.Checked = True
            rbtOrdAgCliPrev.Checked = False
            'Giu040722 Date e Stato - fORNITORI - ARTICOLI - tipo stampa e ordine stampa
            If Not (arrFormDati Is Nothing) Then
                If GetValoreFromChiaveFC(txtDataDa.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    txtDataDa.Text = rkFormCampi.Valore
                End If
                If GetValoreFromChiaveFC(txtDataA.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    txtDataA.Text = rkFormCampi.Valore
                End If
                If GetValoreFromChiaveFC(rbtnConfermati.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    rbtnConfermati.Checked = rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(rbtnDaConfermare.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    rbtnDaConfermare.Checked = rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(rbtnNonConferm.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    rbtnNonConferm.Checked = rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(rbtnChiusoNoConf.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    rbtnChiusoNoConf.Checked = rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(rbtnTutti.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    rbtnTutti.Checked = rkFormCampi.TrueFalse
                End If
                'Fornitor
                If GetValoreFromChiaveFC(chkTuttiFornitori.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    chkTuttiFornitori.Checked = rkFormCampi.TrueFalse
                    btnFornitore.Enabled = Not rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(txtCodFornitore.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    txtCodFornitore.Text = rkFormCampi.Valore
                    txtCodFornitore.Enabled = rkFormCampi.Abilitato
                    If txtCodFornitore.Text.Trim <> "" Then
                        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
                    End If
                End If
                'Articoli
                If GetValoreFromChiaveFC(chkTuttiArticoli.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    chkTuttiArticoli.Checked = rkFormCampi.TrueFalse
                    btnCod1.Enabled = Not rkFormCampi.TrueFalse
                    btnCod2.Enabled = Not rkFormCampi.TrueFalse
                End If
                If GetValoreFromChiaveFC(txtCod1.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    txtCod1.Text = rkFormCampi.Valore
                    txtCod1.Enabled = rkFormCampi.Abilitato
                    If txtCod1.Text.Trim <> "" Then
                        txtDesArt1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
                    End If
                End If
                If GetValoreFromChiaveFC(txtCod2.ID.Trim, arrFormDati, rkFormCampi) = True Then
                    txtCod2.Text = rkFormCampi.Valore
                    txtCod2.Enabled = rkFormCampi.Abilitato
                    If txtCod2.Text.Trim <> "" Then
                        txtDesArt2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
                    End If
                End If
                'tipo stampa 
                If PanelSintAnal.Visible Then
                    If GetValoreFromChiaveFC(rbtnAnalitico.ID.Trim, arrFormDati, rkFormCampi) = True Then
                        rbtnAnalitico.Checked = rkFormCampi.TrueFalse
                    End If
                    If GetValoreFromChiaveFC(rbtnSintetico.ID.Trim, arrFormDati, rkFormCampi) = True Then
                        rbtnSintetico.Checked = rkFormCampi.TrueFalse
                    End If
                End If
                'ordine stampa
                If PanelOrdinamento.Visible Then
                    If GetValoreFromChiaveFC(rbtnOrdAgPrevCli.ID.Trim, arrFormDati, rkFormCampi) = True Then
                        rbtnOrdAgPrevCli.Checked = rkFormCampi.TrueFalse
                    End If
                    If GetValoreFromChiaveFC(rbtOrdAgCliPrev.ID.Trim, arrFormDati, rkFormCampi) = True Then
                        rbtOrdAgCliPrev.Checked = rkFormCampi.TrueFalse
                    End If
                End If

            End If
            rbtnAnalitico.AutoPostBack = True
            rbtnSintetico.AutoPostBack = True
            'tipo stato preventivo
            If GetValoreFromChiaveFC(rbtnChiusoNoConf.ID.Trim, arrFormDati, rkFormCampi) = True Then
                If rkFormCampi.TrueFalse Then rbtnChiusoNoConf.Checked = rkFormCampi.TrueFalse
            End If
            If GetValoreFromChiaveFC(rbtnConfermati.ID.Trim, arrFormDati, rkFormCampi) = True Then
                If rkFormCampi.TrueFalse Then rbtnConfermati.Checked = rkFormCampi.TrueFalse
            End If
            If GetValoreFromChiaveFC(rbtnConfermati.ID.Trim, arrFormDati, rkFormCampi) = True Then
                If rkFormCampi.TrueFalse Then rbtnConfermati.Checked = rkFormCampi.TrueFalse
            End If
        End If
        ModalPopup.WucElement = Me
        '''WFP_Articolo_SelezSing1.WucElement = Me
        '''WFP_ElencoCliForn1.WucElement = Me
        '''WFP_ElencoCliForn2.WucElement = Me
        '''If Session(F_SEL_ARTICOLO_APERTA) = True Then
        '''    WFP_Articolo_SelezSing1.Show()
        '''End If
        '''If Session(F_ELENCO_CLIFORN_APERTA) Then
        '''    If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
        '''        WFP_ElencoCliForn1.Show()
        '''    ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
        '''        WFP_ElencoCliForn1.Show()
        '''    ElseIf Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
        '''        WFP_ElencoCliForn2.Show()
        '''    End If
        '''End If
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        'GIU030722 MEM.DATI QUANDO RITORNA
        Call MemDatiSel()
        '------------------------------------------------------------
        SWGetUltSess = True
        myObject = True
        composeChiave = String.Format("{0}_{1}", _
        "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
        SetObjectToCache(composeChiave, myObject)
        Response.Redirect(Session(CSTChiamatoDa))
    End Sub

    Private Sub btnUscita_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUscita.Click
        composeChiave = String.Format("{0}_{1}", _
           SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        If myObject <> "" Then 'Session(SWOP) <> "" Then
            lblMessAttivita.Text = "Attenzione: Completare prima l'operazione di Modifica: "
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!<br>", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        SessionUtility.LogOutUtente(Session, Response)
    End Sub

    Private Sub SetLnk()
        LnkApriStampa.Visible = False : LnkStampa.Visible = False
    End Sub
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        SetLnk()
        Dim DsOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""
        Dim ErroreCampi As Boolean = False
        ' ''Dim Ordinamento As String
        Dim codCli1 As String = ""
        Dim codCli2 As String = ""
        '-
        Dim codArt1 As String = ""
        Dim codArt2 As String = ""
        '-
        Dim CodForn As String = ""
        Dim Agente As Integer = -1
        Dim LeadSource As Integer = -1
        Dim Stato As Integer = -1

        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtDataDa.Text = "" Then
            StrErrore = StrErrore & "<BR>- inserire la data di inizio periodo."
            ErroreCampi = True
        End If
        If txtDataA.Text = "" Then
            StrErrore = StrErrore & "<BR>- inserire la data di fine periodo."
            ErroreCampi = True
        End If

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    StrErrore = StrErrore & "<BR>- data inizio periodo superiore alla data fine periodo."
                    ErroreCampi = True
                End If
            End If
        End If
        '-
        If PanelSelezioneCli.Visible Then
            If chkTuttiClienti.Checked = False Then
                If txtCodCli1.Text.Trim <> "" And txtCodCli2.Text.Trim <> "" Then
                    If txtCodCli1.Text.Trim > txtCodCli2.Text.Trim Then
                        StrErrore = StrErrore & "<BR>- Il codice Cliente di inizio intervallo è superiore a quello finale."
                        ErroreCampi = True
                    Else
                        codCli1 = txtCodCli1.Text.Trim
                        codCli2 = txtCodCli2.Text.Trim
                    End If
                End If
            End If
        End If

        If PanelSelezionaAgente.Visible = True Then
            If chkTuttiAgenti.Checked = False Then
                If ddlAgenti.SelectedValue < 1 Then
                    StrErrore = StrErrore & "<BR>- Selezionare l'Agente."
                    ErroreCampi = True
                End If
            End If
        Else
            '''If chkTuttiLead.Checked = False Then
            '''    If DDLLead.SelectedValue < 1 Then
            '''        StrErrore = StrErrore & "<BR>- Selezionare Lead Source."
            '''        ErroreCampi = True
            '''    End If
            '''End If
        End If

        If chkTuttiFornitori.Checked = False Then
            If txtCodFornitore.Text.Trim = "" Then
                StrErrore = StrErrore & "<BR>- Selezionare il Fornitore."
                ErroreCampi = True
            Else
                CodForn = txtCodFornitore.Text.Trim
            End If
        End If
        '-
        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text.Trim = "" Or txtCod2.Text.Trim = "" Then
                StrErrore = StrErrore & "<BR>- Selezionare gli articoli."
                ErroreCampi = True
            Else
                If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
                    If txtCod1.Text.Trim > txtCod2.Text.Trim Then
                        StrErrore = StrErrore & "<BR>- Il codice Articolo di inizio intervallo è superiore a quello finale."
                        ErroreCampi = True
                    Else
                        codArt1 = txtCod1.Text.Trim
                        codArt2 = txtCod2.Text.Trim
                    End If
                End If
            End If
        End If
        '-
        If ErroreCampi Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        StrErrore = ""
        '-
        Try
            If PanelSelezionaAgente.Visible = True Then
                If chkTuttiAgenti.Checked Then
                    Agente = -1
                Else
                    Agente = ddlAgenti.SelectedValue
                End If
                If PanelOrdinamento.Visible = True Then
                    If rbtOrdAgCliPrev.Checked = True Then
                        Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA 'giu050722 ordinamento di default
                        TitoloRpt = "Preventivi per Agente/cliente/preventivo"
                    Else
                        Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA
                        TitoloRpt = "Preventivi per Agente/preventivo"
                    End If
                Else
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA 'giu050722 ordinamento di default
                    TitoloRpt = "Preventivi per Agente/cliente/preventivo"
                End If
            Else
                '''Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineLS
                '''If chkTuttiLead.Checked Then
                '''    LeadSource = -1
                '''Else
                '''    LeadSource = DDLLead.SelectedValue
                '''End If
                '''TitoloRpt = "Preventivi per Lead Source/cliente/preventivo"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa non prevista(Lead Source)", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '-
            Dim TipoStampaSA As Integer = -1
            If PanelSintAnal.Visible = True Then
                If rbtnSintetico.Checked = True Then
                    TipoStampaSA = 1
                    If PanelSelezionaAgente.Visible = True Then
                        TitoloRpt = "Preventivi per Agente - Sintetico" 'giu050722 PER ATTIVARLO SWSINTANAL=SI
                    Else
                        TitoloRpt = "Preventivi per Lead Source - Sintetico"
                    End If
                Else
                    TipoStampaSA = 0
                End If
            Else
                TipoStampaSA = 0
                rbtnAnalitico.Checked = True
                rbtnSintetico.Checked = False
            End If
            'GIU031221
            If codCli1.Trim <> "" And codCli2.Trim <> "" And PanelSelezioneCli.Visible = True Then
                TitoloRpt += " - Clienti dal " & codCli1.Trim & " al " & codCli2.Trim
            End If
            'giu041221
            If codArt1.Trim <> "" And codArt2.Trim <> "" And PanelArticolo.Visible = True Then
                If codArt1.Trim <> codArt2.Trim Then
                    TitoloRpt += " - Articoli dal " & codArt1.Trim & " al " & codArt2.Trim
                Else
                    TitoloRpt += " - Articolo " & codArt1.Trim & " - " & txtDesArt1.Text.Trim
                End If
            End If
            If CodForn.Trim <> "" And PanelFornitori.Visible = True Then
                TitoloRpt += " - Fornitore " & CodForn.Trim & " - " & txtDescFornitore.Text.Trim
            End If
            '-
            TitoloRpt += " - Periodo " & txtDataDa.Text.Trim & " al " & txtDataA.Text.Trim
            '---------
            '        CASE StatoDoc
            'WHEN 0 THEN 'Da confermare'
            'WHEN 1 THEN 'Confermato'  
            'WHEN 2 THEN 'Parz. confermato' NON ESISTE
            'WHEN 3 THEN 'Chiuso non confermato'
            'WHEN 4 THEN 'Non confermabile'
            'ELSE ''
            '        END AS DesStatoPR
            If rbtnDaConfermare.Checked = True Then
                Stato = 0
                TitoloRpt += " - Stato Preventivo: Da Confermare"
            ElseIf rbtnConfermati.Checked = True Then
                Stato = 1
                TitoloRpt += " - Stato Preventivo: Confermati"
            ElseIf rbtnChiusoNoConf.Checked = True Then
                Stato = 3
                TitoloRpt += " - Stato Preventivo: Chiuso non confermato"
            ElseIf rbtnNonConferm.Checked = True Then
                Stato = 4
                TitoloRpt += " - Stato Preventivo: Non confermabile"
            Else
                Stato = -1
                TitoloRpt += " - Stato Preventivo: Tutti"
            End If
            '-
            'giu031221
            Dim strWhere As String = ""
            'giu170622 ora PARAMETRI NELLA FUNZIONE FISSI
            ' ''strWhere = " Data_Doc >= CONVERT(DATETIME, '" & Format(CDate(txtDataDa.Text), FormatoData) & "', 103) AND "
            ' ''" Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtDataA.Text), FormatoData) & "', 103) "
            If PanelSelezioneCli.Visible = True Then 'GIU041221
                If codCli2.Trim = "" Then
                    codCli2 = "ZZZZZZZZZZZZZZZZ"
                End If
                If codCli1.Trim <> "" And codCli2.Trim <> "" Then
                    If strWhere.Trim <> "" Then 'GIU170622
                        strWhere += " AND "
                    End If
                    strWhere = strWhere & " Cod_Cliente >= '" & Controlla_Apice(codCli1.Trim) & _
                                "' AND Cod_Cliente <= '" & Controlla_Apice(codCli2.Trim) & "' "
                End If
            End If
            'giu041221
            If PanelArticolo.Visible = True Then
                If codArt2.Trim = "" Then
                    codArt2 = "ZZZZZZZZZZZZZZZZ"
                End If
                If codArt1.Trim <> "" And codArt2.Trim <> "" Then
                    If strWhere.Trim <> "" Then 'GIU170622
                        strWhere += " AND "
                    End If
                    strWhere = strWhere & " Cod_Articolo >= '" & Controlla_Apice(codArt1.Trim) & _
                                "' AND Cod_Articolo <= '" & Controlla_Apice(codArt2.Trim) & "' "
                End If
            End If
            If PanelFornitori.Visible = True Then
                If CodForn.Trim <> "" Then
                    If strWhere.Trim <> "" Then 'GIU170622
                        strWhere += " AND "
                    End If
                    strWhere = strWhere & " CodiceFornitore = '" & Controlla_Apice(CodForn.Trim) & "' "
                End If
            End If
            '-
            If PanelSelezionaAgente.Visible = True Then
                If Agente <> -1 Then
                    ' ''If strWhere.Trim <> "" Then 'GIU170622
                    ' ''    strWhere += " AND "
                    ' ''End If
                    'giu170622 ora PARAMETRI NELLA FUNZIONE FISSI
                    ' ''strWhere = strWhere & " Agente = " & Agente
                End If
            Else
                If LeadSource <> -1 Then
                    ' ''If strWhere.Trim <> "" Then 'GIU170622
                    ' ''    strWhere += " AND "
                    ' ''End If
                    'giu170622 ora PARAMETRI NELLA FUNZIONE FISSI
                    ' ''strWhere = strWhere & " LeadSource = " & LeadSource
                End If
            End If
            '-
            'giu150622 sempre attivo
            Dim pRegione As Integer = -1
            If chkTutteRegioni.Checked = False And ddlRegioni.SelectedValue > 0 Then
                ' ''If strWhere.Trim <> "" Then 'GIU170622
                ' ''    strWhere += " AND "
                ' ''End If
                'giu170622 ora PARAMETRI NELLA FUNZIONE FISSI
                ' ''strWhere = strWhere & " (CodRegione = " & ddlRegioni.SelectedValue & ") "
                pRegione = ddlRegioni.SelectedValue 'giu170622
                TitoloRpt += " - Regione: " & ddlRegioni.SelectedItem.Text
                '-
                If ddlProvince.SelectedValue <> "" Then
                    If strWhere.Trim <> "" Then 'GIU170622
                        strWhere += " AND "
                    End If
                    strWhere = strWhere & " (CodProvincia = '" & ddlProvince.SelectedValue & "') "
                    TitoloRpt += " Provincia: " & ddlProvince.SelectedItem.Text
                    
                End If
                '-------------
            End If
            '-
            If Stato <> -1 Then
                If strWhere.Trim <> "" Then 'GIU170622
                    strWhere += " AND "
                End If
                strWhere = strWhere & " StatoDoc = " & Stato
            End If
            '-----------
            'GIU030722 MEM.DATI QUANDO RITORNA
            Call MemDatiSel()
            '------------------------------------------------------------
            If PanelSelezionaAgente.Visible = True Then
                If ClsPrint.StampaPrevClienteOrdineAG(Session(CSTAZIENDARPT), TitoloRpt, DsOrdinatoClienteOrdine1, ObjReport, StrErrore, _
                    Format(CDate(txtDataDa.Text), FormatoData), Format(CDate(txtDataA.Text), FormatoData), _
                    Agente, LeadSource, pRegione, strWhere, TipoStampaSA) Then 'GIU170622
                    If DsOrdinatoClienteOrdine1.OrdinatoClienteOrdine.Count > 0 Then
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDsPrinWebDoc) = DsOrdinatoClienteOrdine1
                        Session(CSTNOBACK) = 0 'giu040512
                        If chkStampaPDF.Checked Then
                            Call OKApriStampa()
                        Else
                            Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
                        End If
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If ClsPrint.StampaPrevClienteOrdineLS(Session(CSTAZIENDARPT), TitoloRpt, DsOrdinatoClienteOrdine1, ObjReport, StrErrore, _
                    Format(CDate(txtDataDa.Text), FormatoData), Format(CDate(txtDataA.Text), FormatoData), _
                    Agente, LeadSource, pRegione, strWhere, TipoStampaSA) Then 'GIU170622
                    If DsOrdinatoClienteOrdine1.OrdinatoClienteOrdine.Count > 0 Then
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDsPrinWebDoc) = DsOrdinatoClienteOrdine1
                        Session(CSTNOBACK) = 0 'giu040512
                        If chkStampaPDF.Checked Then
                            Call OKApriStampa()
                        Else
                            Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
                        End If
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If

        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in PrevClienteOrdineAG.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    'giu060823
    Private Sub MemDatiSel()
        'GIU030722 MEM.DATI QUANDO RITORNA
        Dim RkFormDati As StrFormCampi
        Dim arrFormDati As ArrayList = Nothing
        arrFormDati = New ArrayList
        '------------------------------------------------------------
        RkFormDati.Campo = txtDataDa.ID
        RkFormDati.Abilitato = txtDataDa.Enabled
        RkFormDati.Visibile = txtDataDa.Visible
        RkFormDati.Valore = txtDataDa.Text.Trim
        arrFormDati.Add(RkFormDati)
        '-
        RkFormDati.Campo = txtDataA.ID
        RkFormDati.Abilitato = txtDataA.Enabled
        RkFormDati.Visibile = txtDataA.Visible
        RkFormDati.Valore = txtDataA.Text.Trim
        arrFormDati.Add(RkFormDati)
        '-
        If PanelSelezioneCli.Visible Then
            '-
            RkFormDati.Campo = chkTuttiClienti.ID
            RkFormDati.Abilitato = chkTuttiClienti.Enabled
            RkFormDati.Visibile = chkTuttiClienti.Visible
            RkFormDati.Valore = chkTuttiClienti.Checked
            arrFormDati.Add(RkFormDati)
            '-
            If chkTuttiClienti.Checked = False Then
                If txtCodCli1.Text.Trim <> "" And txtCodCli2.Text.Trim <> "" Then
                    If txtCodCli1.Text.Trim > txtCodCli2.Text.Trim Then
                        
                    Else
                        '-
                        RkFormDati.Campo = txtCodCli1.ID
                        RkFormDati.Abilitato = txtCodCli1.Enabled
                        RkFormDati.Visibile = txtCodCli1.Visible
                        RkFormDati.Valore = txtCodCli1.Text.Trim
                        arrFormDati.Add(RkFormDati)
                        '-
                        RkFormDati.Campo = txtCodCli2.ID
                        RkFormDati.Abilitato = txtCodCli2.Enabled
                        RkFormDati.Visibile = txtCodCli2.Visible
                        RkFormDati.Valore = txtCodCli2.Text.Trim
                        arrFormDati.Add(RkFormDati)
                        '-
                    End If
                End If
            End If
        End If

        If PanelSelezionaAgente.Visible = True Then
            '-
            RkFormDati.Campo = chkTuttiAgenti.ID
            RkFormDati.Abilitato = chkTuttiAgenti.Enabled
            RkFormDati.Visibile = chkTuttiAgenti.Visible
            RkFormDati.Valore = chkTuttiAgenti.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = ddlAgenti.ID
            RkFormDati.Abilitato = ddlAgenti.Enabled
            RkFormDati.Visibile = ddlAgenti.Visible
            RkFormDati.Valore = ddlAgenti.SelectedValue.Trim
            arrFormDati.Add(RkFormDati)
            
        Else
            '-
            '''RkFormDati.Campo = chkTuttiLead.ID
            '''RkFormDati.Abilitato = chkTuttiLead.Enabled
            '''RkFormDati.Visibile = chkTuttiLead.Visible
            '''RkFormDati.Valore = chkTuttiLead.Checked
            '''arrFormDati.Add(RkFormDati)
            ''''-
            '''RkFormDati.Campo = DDLLead.ID
            '''RkFormDati.Abilitato = DDLLead.Enabled
            '''RkFormDati.Visibile = DDLLead.Visible
            '''RkFormDati.Valore = DDLLead.SelectedValue.Trim
            '''arrFormDati.Add(RkFormDati)
            
        End If
        'giu041221
        '-
        RkFormDati.Campo = chkTuttiFornitori.ID
        RkFormDati.Abilitato = chkTuttiFornitori.Enabled
        RkFormDati.Visibile = chkTuttiFornitori.Visible
        RkFormDati.Valore = chkTuttiFornitori.Checked
        arrFormDati.Add(RkFormDati)
        '- 
        RkFormDati.Campo = txtCodFornitore.ID
        RkFormDati.Abilitato = txtCodFornitore.Enabled
        RkFormDati.Visibile = txtCodFornitore.Visible
        RkFormDati.Valore = txtCodFornitore.Text.Trim
        arrFormDati.Add(RkFormDati)
        '-
        RkFormDati.Campo = chkTuttiArticoli.ID
        RkFormDati.Abilitato = chkTuttiArticoli.Enabled
        RkFormDati.Visibile = chkTuttiArticoli.Visible
        RkFormDati.Valore = chkTuttiArticoli.Checked
        arrFormDati.Add(RkFormDati)
        '- 
        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text.Trim = "" Or txtCod2.Text.Trim = "" Then
                
            Else
                RkFormDati.Campo = txtCod1.ID
                RkFormDati.Abilitato = txtCod1.Enabled
                RkFormDati.Visibile = txtCod1.Visible
                RkFormDati.Valore = txtCod1.Text.Trim
                arrFormDati.Add(RkFormDati)
                '-
                RkFormDati.Campo = txtCod2.ID
                RkFormDati.Abilitato = txtCod2.Enabled
                RkFormDati.Visibile = txtCod2.Visible
                RkFormDati.Valore = txtCod2.Text.Trim
                arrFormDati.Add(RkFormDati)
            End If
        End If
        '-
        Try
            RkFormDati.Campo = rbtnSintetico.ID
            RkFormDati.Abilitato = rbtnSintetico.Enabled
            RkFormDati.Visibile = rbtnSintetico.Visible
            RkFormDati.Valore = rbtnSintetico.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnAnalitico.ID
            RkFormDati.Abilitato = rbtnAnalitico.Enabled
            RkFormDati.Visibile = rbtnAnalitico.Visible
            RkFormDati.Valore = rbtnAnalitico.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnDaConfermare.ID
            RkFormDati.Abilitato = rbtnDaConfermare.Enabled
            RkFormDati.Visibile = rbtnDaConfermare.Visible
            RkFormDati.Valore = rbtnDaConfermare.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnConfermati.ID
            RkFormDati.Abilitato = rbtnConfermati.Enabled
            RkFormDati.Visibile = rbtnConfermati.Visible
            RkFormDati.Valore = rbtnConfermati.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnChiusoNoConf.ID
            RkFormDati.Abilitato = rbtnChiusoNoConf.Enabled
            RkFormDati.Visibile = rbtnChiusoNoConf.Visible
            RkFormDati.Valore = rbtnChiusoNoConf.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnNonConferm.ID
            RkFormDati.Abilitato = rbtnNonConferm.Enabled
            RkFormDati.Visibile = rbtnNonConferm.Visible
            RkFormDati.Valore = rbtnNonConferm.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnTutti.ID
            RkFormDati.Abilitato = rbtnTutti.Enabled
            RkFormDati.Visibile = rbtnTutti.Visible
            RkFormDati.Valore = rbtnTutti.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = chkTutteRegioni.ID
            RkFormDati.Abilitato = chkTutteRegioni.Enabled
            RkFormDati.Visibile = chkTutteRegioni.Visible
            RkFormDati.Valore = chkTutteRegioni.Checked
            arrFormDati.Add(RkFormDati)
            
            If chkTutteRegioni.Checked = False And ddlRegioni.SelectedValue > 0 Then
                RkFormDati.Campo = ddlRegioni.ID
                RkFormDati.Abilitato = ddlRegioni.Enabled
                RkFormDati.Visibile = ddlRegioni.Visible
                RkFormDati.Valore = ddlRegioni.SelectedValue.Trim
                arrFormDati.Add(RkFormDati)
                '
                If ddlProvince.SelectedValue <> "" Then
                    RkFormDati.Campo = ddlProvince.ID
                    RkFormDati.Abilitato = ddlProvince.Enabled
                    RkFormDati.Visibile = ddlProvince.Visible
                    RkFormDati.Valore = ddlProvince.SelectedValue.Trim
                    arrFormDati.Add(RkFormDati)
                End If
                '-------------
            End If
            '-
            '-Ordinamento
            If PanelOrdinamento.Visible = True Then
                RkFormDati.Campo = rbtnOrdAgPrevCli.ID
                RkFormDati.Abilitato = rbtnOrdAgPrevCli.Enabled
                RkFormDati.Visibile = rbtnOrdAgPrevCli.Visible
                RkFormDati.Valore = rbtnOrdAgPrevCli.Checked
                arrFormDati.Add(RkFormDati)
                '-
                RkFormDati.Campo = rbtOrdAgCliPrev.ID
                RkFormDati.Abilitato = rbtOrdAgCliPrev.Enabled
                RkFormDati.Visibile = rbtOrdAgCliPrev.Visible
                RkFormDati.Valore = rbtOrdAgCliPrev.Checked
                arrFormDati.Add(RkFormDati)
                '-
            End If
            '-
            'tipo stato Prev
            RkFormDati.Campo = rbtnChiusoNoConf.ID
            RkFormDati.Abilitato = rbtnChiusoNoConf.Enabled
            RkFormDati.Visibile = rbtnChiusoNoConf.Visible
            RkFormDati.Valore = rbtnChiusoNoConf.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnConfermati.ID
            RkFormDati.Abilitato = rbtnConfermati.Enabled
            RkFormDati.Visibile = rbtnConfermati.Visible
            RkFormDati.Valore = rbtnConfermati.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnDaConfermare.ID
            RkFormDati.Abilitato = rbtnDaConfermare.Enabled
            RkFormDati.Visibile = rbtnDaConfermare.Visible
            RkFormDati.Valore = rbtnDaConfermare.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnNonConferm.ID
            RkFormDati.Abilitato = rbtnNonConferm.Enabled
            RkFormDati.Visibile = rbtnNonConferm.Visible
            RkFormDati.Valore = rbtnNonConferm.Checked
            arrFormDati.Add(RkFormDati)
            '-
            RkFormDati.Campo = rbtnTutti.ID
            RkFormDati.Abilitato = rbtnTutti.Enabled
            RkFormDati.Visibile = rbtnTutti.Visible
            RkFormDati.Valore = rbtnTutti.Checked
            arrFormDati.Add(RkFormDati)
            'GIU030722 MEM.DATI QUANDO RITORNA
            mylabelForm = Request.QueryString("labelForm")
            If InStr(mylabelForm.Trim.ToUpper, "AGENTE") > 0 Then
                Session(Me.ID.Trim + "AG") = arrFormDati
            Else
                Session(Me.ID.Trim + "LS") = arrFormDati
            End If
            '------------------------------------------------------------
        Catch ex As Exception

        End Try
    End Sub
    Private Sub OKApriStampa()
        lblMessAttivita.Text = ""
        Dim Rpt As Object = Nothing
        Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
        DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
        If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA Then 'giu080421
            Rpt = New PrevClienteOrdineAG
            '''Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            '''DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            '''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            '''Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            '''CrystalReportViewer1.DisplayGroupTree = True
            '''CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA Then 'giu050722
            Rpt = New PrevClienteOrdineAGPrev
            '''Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            '''DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            '''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            '''Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            '''CrystalReportViewer1.DisplayGroupTree = True
            '''CrystalReportViewer1.ReportSource = Rpt
            '''ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineLS Then 'giu190421
            '''    Rpt = New PrevClienteOrdineLS
            '''Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            '''DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            '''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            '''Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            '''CrystalReportViewer1.DisplayGroupTree = True
            '''CrystalReportViewer1.ReportSource = Rpt
        Else
            lblMessAttivita.Text = "Errore: TIPO STAMPA ORDINATO SCONOSCIUTA"
            Exit Sub
        End If
        '---------
        Dim NomeStampa As String = "ELENCO_PREV_" + Format(Now, "yyyyMMddHHmmss") + ".PDF"
        Session(CSTNOMEPDF) = NomeStampa
        Dim SubDirDOC As String = "Preventivi"
        '-
        Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
        '-----------------------------------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            lblMessAttivita.Text = "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message.Trim
            Rpt = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Stampa", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            ' ''Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        LnkStampa.HRef = LnkName
        LnkApriStampa.HRef = LnkName
        LnkStampa.Visible = True
        LnkApriStampa.Visible = True
    End Sub
    '---------
    Private Sub chkTuttiClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampi()
        If chkTuttiClienti.Checked Then
            txtCodCli1.Enabled = False
            txtCodCli2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnStampa.Focus()
        Else
            'If rbtnCodice.Checked = True Then
            AbilitaDisabilitaComponenti(True)
            txtCodCli1.Focus()
            'Else
            'AbilitaDisabilitaComponenti(False)
            'txtDesc1.Focus()
            'End If
        End If
    End Sub
    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCodCli1.Enabled = Abilita
        txtCodCli2.Enabled = Abilita
        btnCercaAnagrafica1.Enabled = Abilita
        btnCercaAnagrafica2.Enabled = Abilita
        txtDesc1.Enabled = Not (Abilita)
        txtDesc2.Enabled = Not (Abilita)
    End Sub

    '''Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
    '''    Try
    '''        Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
    '''    Catch ex As Exception
    '''        Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
    '''    End Try
    '''End Sub
    Private Sub pulisciCampi()
        txtCodCli1.Text = ""
        txtCodCli2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub

    '''Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

    '''    Session(F_CLI_RICERCA) = True
    '''    ApriElencoClienti1()

    '''End Sub
    '''Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
    '''    If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
    '''        txtCodCli1.Text = codice
    '''        txtDesc1.Text = descrizione
    '''        If txtCodCli2.Text.Trim = "" Then
    '''            txtCodCli2.Text = codice
    '''            txtDesc2.Text = descrizione
    '''        End If
    '''    ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
    '''        txtCodCli2.Text = codice
    '''        txtDesc2.Text = descrizione
    '''    ElseIf Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
    '''        txtCodFornitore.Text = codice
    '''        txtDescFornitore.Text = descrizione
    '''    End If
    '''    '
    '''    Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
    '''    Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
    '''    Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
    '''    Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    '''End Sub
    '''Private Sub ApriElencoClienti1()
    '''    Session(F_ELENCO_CLIFORN_APERTA) = True
    '''    Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
    '''    WFP_ElencoCliForn1.Show(True)
    '''End Sub
    '''Private Sub ApriElencoClienti2()
    '''    Session(F_ELENCO_CLIFORN_APERTA) = True
    '''    Session(OSCLI_F_ELENCO_CLI2_APERTA) = True
    '''    WFP_ElencoCliForn2.Show(True)
    '''End Sub

    '''Private Sub btnCercaAnagrafica2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica2.Click
    '''    Session(F_CLI_RICERCA) = True
    '''    ApriElencoClienti2()
    '''End Sub
    Private Sub txtCodCli1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCli1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCodCli1.Text, Def.CLIENTI, Session(ESERCIZIO))
        If txtDesc1.Text.Trim <> "" And txtCodCli2.Text.Trim = "" Then
            txtCodCli2.Text = txtCodCli1.Text.Trim
            txtDesc2.Text = txtDesc1.Text.Trim
        End If
    End Sub

    Private Sub txtCodCli2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCli2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCodCli2.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub chkTuttiAgenti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiAgenti.CheckedChanged
        ddlAgenti.SelectedIndex = -1
        If chkTuttiAgenti.Checked Then
            ddlAgenti.Enabled = False
            'GIU040722
            ' ''rbtnAnalitico.AutoPostBack = False
            ' ''rbtnAnalitico.Checked = False
            ' ''rbtnSintetico.AutoPostBack = False
            ' ''rbtnSintetico.Checked = True
            ' ''rbtnAnalitico.AutoPostBack = True
            ' ''rbtnSintetico.AutoPostBack = True
        Else
            ddlAgenti.Enabled = True
        End If
    End Sub

    '''Private Sub chkTuttiLead_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiLead.CheckedChanged
    '''    DDLLead.SelectedIndex = -1
    '''    If chkTuttiLead.Checked Then
    '''        DDLLead.Enabled = False
    '''        rbtnAnalitico.AutoPostBack = False
    '''        rbtnAnalitico.Checked = False
    '''        rbtnSintetico.AutoPostBack = False
    '''        rbtnSintetico.Checked = True
    '''        rbtnAnalitico.AutoPostBack = True
    '''        rbtnSintetico.AutoPostBack = True
    '''    Else
    '''        DDLLead.Enabled = True
    '''    End If
    '''End Sub

    Private Sub rbtnAnalitico_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnAnalitico.CheckedChanged
        If rbtnAnalitico.Checked And chkTuttiClienti.Checked And chkTuttiFornitori.Checked And chkTuttiArticoli.Checked And chkTutteRegioni.Checked Then
            ddlAgenti.Enabled = True
            '''DDLLead.Enabled = True
            chkTuttiAgenti.AutoPostBack = False
            chkTuttiAgenti.Checked = False
            chkTuttiAgenti.AutoPostBack = True
            '''chkTuttiLead.AutoPostBack = False
            '''chkTuttiLead.Checked = False
            '''chkTuttiLead.AutoPostBack = True
        End If
    End Sub

    Private Sub rbtnSintetico_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnSintetico.CheckedChanged
        If rbtnSintetico.Checked = False And chkTuttiClienti.Checked And chkTuttiFornitori.Checked And chkTuttiArticoli.Checked And chkTutteRegioni.Checked Then
            ddlAgenti.Enabled = True
            '''DDLLead.Enabled = True
            chkTuttiAgenti.AutoPostBack = False
            chkTuttiAgenti.Checked = False
            chkTuttiAgenti.AutoPostBack = True
            '''chkTuttiLead.AutoPostBack = False
            '''chkTuttiLead.Checked = False
            '''chkTuttiLead.AutoPostBack = True
        End If
    End Sub
    'giu041221
    '''Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
    '''    Session(SWCOD1COD2) = 1
    '''    WFP_Articolo_SelezSing1.WucElement = Me
    '''    Session(F_SEL_ARTICOLO_APERTA) = True
    '''    WFP_Articolo_SelezSing1.Show()
    '''End Sub
    '''Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
    '''    Session(SWCOD1COD2) = 2
    '''    WFP_Articolo_SelezSing1.WucElement = Me
    '''    Session(F_SEL_ARTICOLO_APERTA) = True
    '''    WFP_Articolo_SelezSing1.Show()
    '''End Sub
    '''Public Sub CallBackWFPArticoloSelSing()
    '''    'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
    '''    'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
    '''    'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
    '''    'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
    '''    'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
    '''    '-----------------------------------------------------------------------------------------------
    '''    If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
    '''        txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''        txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        txtDesArt1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        If txtCod2.Text.Trim = "" Then
    '''            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            txtDesArt2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        End If
    '''    ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
    '''        If Session(SWCOD1COD2).ToString.Trim = "1" Then
    '''            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            txtDesArt1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            If txtCod2.Text.Trim = "" Then
    '''                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''                txtDesArt2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            End If
    '''        ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
    '''            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            txtDesArt2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        Else
    '''            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            txtDesArt1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            If txtCod2.Text.Trim = "" Then
    '''                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''                txtDesArt2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            End If
    '''        End If
    '''    Else
    '''        txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''        txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        txtDesArt1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        If txtCod2.Text.Trim = "" Then
    '''            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
    '''            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''            txtDesArt2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
    '''        End If
    '''    End If

    '''End Sub
    'giu06040
    Private Sub chkTuttiFornitori_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiFornitori.CheckedChanged
        pulisciCampiFornitore()
        If chkTuttiFornitori.Checked Then
            AbilitaDisabilitaCampiFornitore(False)
        Else
            AbilitaDisabilitaCampiFornitore(True)
            txtCodFornitore.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiFornitore(ByVal Abilita As Boolean)
        txtCodFornitore.Enabled = Abilita
        btnFornitore.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiFornitore()
        txtCodFornitore.Text = ""
        txtDescFornitore.Text = ""
    End Sub
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub
    '''Private Sub ApriElencoFornitori1()
    '''    Session(F_ELENCO_CLIFORN_APERTA) = True
    '''    Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
    '''    WFP_ElencoCliForn2.Show(True)
    '''End Sub

    '''Private Sub btnFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFornitore.Click
    '''    Session(F_FOR_RICERCA) = True
    '''    ApriElencoFornitori1()
    '''End Sub

    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesArt1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
        If txtCod2.Text.Trim = "" Then
            txtCod2.Text = txtCod1.Text
            txtDesArt2.Text = txtDesc1.Text
        End If
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesArt2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub
    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampiArticolo()
        If chkTuttiArticoli.Checked Then
            AbilitaDisabilitaCampiArticolo(False)
        Else
            AbilitaDisabilitaCampiArticolo(True)
            txtCod1.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiArticolo(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiArticolo()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesArt1.Text = ""
        txtDesArt2.Text = ""
    End Sub

    Private Sub chkTutteRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        If chkTutteRegioni.Checked Then
            ddlRegioni.SelectedIndex = 0
            ddlRegioni.Enabled = False
            ddlProvince.SelectedIndex = 0
            ddlProvince.Enabled = False
        Else
            ddlRegioni.Enabled = True
            ddlRegioni.Focus()
            ddlProvince.Enabled = True
            Session("CodRegione") = ddlRegioni.SelectedValue
        End If
    End Sub

    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        Session("CodRegione") = ddlRegioni.SelectedValue
    End Sub

End Class