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

Partial Public Class WF_MenuCA
    Inherits System.Web.UI.Page
    Private TipoDoc As String = SWTD(TD.ContrattoAssistenza) : Private TabCliFor As String = "Cli"
    Private InizialiUT As String = ""
    Private CodiceDitta As String = ""

    Private aDataViewScAtt As DataView
    Private DVScadAtt As DataView

    Private SqlAdapDocDett As SqlDataAdapter
    Private SqlConnDocDett As SqlConnection
    Private SqlDbSelectCmd As SqlCommand

    Private Enum CellIdxT
        RespArea = 1
        RespVisite = 2
        DataSc = 3
        CodArt = 4
        DesArt = 5
        SerieLotto = 6
        Modello = 7
        NoteApp = 8
        UM = 9
        Qta = 10
        QtaEv = 11

        RagSoc = 12
        Denom = 13
        'giu131221
        IndirApp = 14
        LuogoApp = 15
        CAPApp = 16
        PrApp = 17
        '-
        Importo = 18 'non visible
        Telefono12 = 19
        '-
        Referente = 20 'giu220123
        EmailVerbale = 21
        Note = 22

        NumDoc = 23
        Riga = 24
        DataDoc = 25
        CodCliForProvv = 26
        Loc = 27
        CAP = 28
        Pr = 29
        Riferimento = 30
        Dest1 = 31
        Dest2 = 32
        Dest3 = 33
        Stato = 34
        DNR = 35

    End Enum
    Public Enum CellCAAtt
        Riga = 0
        SerieLotto = 1
        Articolo = 2
        Evasa = 3
        DataSc = 4
        DataEv = 5
        DataEvN = 6
        SWModAgenti = 7
        SWModificato = 8
    End Enum

    Dim FileCaricato As Boolean
    'giu281022
    Dim composeChiave As String = ""
    Dim myObject As Object = Nothing
    Dim UtenteConnesso As OperatoreConnessoEntity
    Dim SWGetUltSess As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LnkVerbale.Visible = False
        lblMessAttivita.Text = ""
        ModalPopup.WucElement = Me
        Session(CSTChiamatoDa) = "WF_MenuCA.aspx?labelForm=Menu Gestione CONTRATTI"
        lblDataOdierna.Text = Format(Now, "dddd d MMMM yyyy, HH:mm")
        '-
        Try
            If ddlClienti.SelectedValue.Trim <> "" Then
                GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = False
            Else
                GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = True
            End If
            '-
            If chkRespArea.Checked Then
                GridViewPrevT.Columns.Item(CellIdxT.RespVisite).Visible = True
            Else
                GridViewPrevT.Columns.Item(CellIdxT.RespVisite).Visible = False
            End If
        Catch ex As Exception

        End Try
        '-------------------------
        If IsPostBack Then
            If Request.Params.Get("__EVENTTARGET").ToString = "LnkUltimaSessOK" Then
                '' se servisse Dim arg As String = Request.Form("__EVENTARGUMENT").ToString

                myObject = True
                composeChiave = String.Format("{0}_{1}",
                "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
                SetObjectToCache(composeChiave, myObject)

            End If
        End If

        Try
            composeChiave = String.Format("{0}_{1}",
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            GetObjectToCache(composeChiave, myObject)
            SWGetUltSess = myObject
            '-
            myObject = False
            composeChiave = String.Format("{0}_{1}",
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        Catch ex As Exception
            SWGetUltSess = False
            myObject = False
            composeChiave = String.Format("{0}_{1}",
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        End Try
        UtenteConnesso = SessionUtility.GetLogOnUtente("", "", Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo, Mid(Session.SessionID, 1, 50), -1, "", "", "", "")
        If (UtenteConnesso Is Nothing) Then
            composeChiave = String.Format("{0}_{1}",
            "UtenteConnesso", Mid(Request.UserHostAddress.Trim, 1, 50))
            GetObjectToCache(composeChiave, myObject)
            UtenteConnesso = myObject
            If (UtenteConnesso Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta Oltre 1 ora: utente non valido.(Load)")
                Exit Sub
            End If
        Else
            myObject = UtenteConnesso
            composeChiave = String.Format("{0}_{1}",
            "UtenteConnesso", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)
        End If
        'giu031122 verifico se la sessione in cache è scaduta
        composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If (myObject Is Nothing) Then
            SWGetUltSess = False
        End If
        myObject = SWGetUltSess
        composeChiave = String.Format("{0}_{1}",
        "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
        SetObjectToCache(composeChiave, myObject)
        '-----------------------------------------------------
        If (Not IsPostBack) And SWGetUltSess = False Then
            ' ''aDataViewScAtt = Nothing
            ' ''myObject = aDataViewScAtt
            ' ''composeChiave = String.Format("{0}_{1}", _
            ' ''"aDataViewScAtt", UtenteConnesso.Codice)
            ' ''SetObjectToCache(composeChiave, myObject)
            '-
            chkEvadiTutte.AutoPostBack = False
            chkEvadiTutte.Checked = False
            chkEvadiTutte.Visible = False : chkEvadiTutte.Enabled = False
            chkEvadiTutte.AutoPostBack = False
        End If
        '----------------------------------
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        '-
        If Not (myObject Is Nothing) Then

            aDataViewScAtt = myObject
        Else

            If aDataViewScAtt Is Nothing Then
                aDataViewScAtt = New DataView
            End If
            BtnSetEnabledTo(False)
            btnModScAttCA.Enabled = False
        End If
        myObject = aDataViewScAtt
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        GridViewPrevT.DataSource = aDataViewScAtt
        '---------------------------------
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
        'giu240212
        ' ''If (Not UtenteConnesso.SessionID.Equals(Mid(Session.SessionID, 1, 50))) Then 'GIU150319 MID
        ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione non valida: ID Sessione diversa dalla sessione corrente.")
        ' ''    Exit Sub
        ' ''End If
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
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        '---------
        'GIU040823 ZIBORDI
        Dim strValore As String = ""
        Dim strErrore As String = ""
        LnkMenuStatisPR.Visible = False
        Try
            If String.IsNullOrEmpty(Session("mnuStatPR")) Then
                If App.GetDatiAbilitazioni(CSTABILAZI, "mnuStatPR", strValore, strErrore) = True Then
                    Session("mnuStatPR") = SWSI
                    If Not String.IsNullOrEmpty(strValore) Then
                        If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                            LnkMenuStatisPR.Visible = True
                            Session("mnuStatPRValore") = strValore.Trim.ToUpper
                        Else
                            Session("mnuStatPR") = SWNO
                        End If
                    Else
                        Session("mnuStatPR") = SWNO
                    End If
                Else
                    Session("mnuStatPR") = SWNO
                    'Session(ERRORE) = strErrore
                End If
            ElseIf Session("mnuStatPR").ToString.Trim = "" Then
                If App.GetDatiAbilitazioni(CSTABILAZI, "mnuStatPR", strValore, strErrore) = True Then
                    Session("mnuStatPR") = SWSI
                    If Not String.IsNullOrEmpty(strValore) Then
                        If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                            LnkMenuStatisPR.Visible = True
                            Session("mnuStatPRValore") = strValore.Trim.ToUpper
                        Else
                            Session("mnuStatPR") = SWNO
                        End If
                    Else
                        Session("mnuStatPR") = SWNO
                    End If
                Else
                    Session("mnuStatPR") = SWNO
                    'Session(ERRORE) = strErrore
                End If
            ElseIf Session("mnuStatPR") = SWSI Then
                If Not String.IsNullOrEmpty(Session("mnuStatPRValore")) Then
                    strValore = Session("mnuStatPRValore")
                    If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                        LnkMenuStatisPR.Visible = True
                    Else
                        Session("mnuStatPR") = SWNO
                    End If
                Else
                    If App.GetDatiAbilitazioni(CSTABILAZI, "mnuStatPR", strValore, strErrore) = True Then
                        Session("mnuStatPR") = SWSI
                        If Not String.IsNullOrEmpty(strValore) Then
                            If InStr(strValore.Trim.ToUpper, UtenteConnesso.NomeOperatore.Trim.ToUpper) > 0 Then
                                LnkMenuStatisPR.Visible = True
                                Session("mnuStatPRValore") = strValore.Trim.ToUpper
                            Else
                                Session("mnuStatPR") = SWNO
                            End If
                        Else
                            Session("mnuStatPR") = SWNO
                        End If
                    Else
                        Session("mnuStatPR") = SWNO
                        'Session(ERRORE) = strErrore
                    End If
                End If
            End If
        Catch ex As Exception
            Session("mnuStatPR") = SWNO
            LnkMenuStatisPR.Visible = False
        End Try
        '-----------------
        If (Not IsPostBack) And SWGetUltSess = False Then
            'giu010523
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "RicaricaDati", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '---------------------
            GetOpConnessi(Session(CSTUTENTE).ToString.Trim)

            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)

            Session(SWOPDETTDOC) = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPDETTDOC, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)

            Session(SWOPDETTDOCL) = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPDETTDOCL, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPMODSCATT) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPMODSCATT, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            '- Cercare la prima scadenza 
            txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
            txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
            myObject = txtDallaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtDallaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtAllaData.Text = Now.Date.ToString("31/12/" + "yyyy")
            myObject = txtAllaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtAllaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            'GIU100222
            ' ''Dim myAllaData As DateTime = DateAdd(DateInterval.Month, 1, Now.Date).ToString("01" + "/MM/yyyy")
            ' ''txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
            ' ''txtDallaData.Text = GetPrimaDataSc()
            ' ''If txtDallaData.Text.Trim = "" Then
            ' ''    txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
            ' ''End If
            ' ''myAllaData = DateAdd(DateInterval.Month, 1, Now.Date).ToString("01" + "/MM/yyyy")
            ' ''txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
            ' ''If CDate(txtDallaData.Text.Trim) > CDate(txtAllaData.Text.Trim) Then
            ' ''    txtDallaData.Text = txtAllaData.Text.Trim
            ' ''End If
            'giu140224
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "GetPrimaDataSc", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-----------------------------------------
            Call GetPrimaDataSc()
            txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
            '----------
            BtnSetEnabledTo(False)
            btnModScAttCA.Enabled = False
            GridViewDettCAAtt.Enabled = False
        ElseIf (Not IsPostBack) And SWGetUltSess = True Then 'GIU140423 SOLO LA PRIMA VOLTA
            'giu010523
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "RicaricaDati", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '---------------------
            GetOpConnessi(Session(CSTUTENTE).ToString.Trim)

            SWGetUltSess = False
            myObject = False
            composeChiave = String.Format("{0}_{1}",
            "SWGetUltSess", Mid(Request.UserHostAddress.Trim, 1, 50))
            SetObjectToCache(composeChiave, myObject)

            BuidDett()
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)

            Session(SWOPDETTDOC) = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPDETTDOC, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)

            Session(SWOPDETTDOCL) = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPDETTDOCL, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPMODSCATT) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOPMODSCATT, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtEmailInvio.Enabled = False : txtNoteDocumento.Enabled = False 'giu200122
            txtAllaPresezaDi.Enabled = False
            GridViewDettCAAtt.Enabled = False
            btnModScAttCA.Visible = True
            btnAggScAttCA.Visible = False
            btnAnnScAttCA.Visible = False
            'giu131221
            chkEvadiTutte.AutoPostBack = False
            chkEvadiTutte.Checked = False
            chkEvadiTutte.Visible = False : chkEvadiTutte.Enabled = False
            chkEvadiTutte.AutoPostBack = False
            '-------------------------------
            GridViewPrevT.Enabled = True
            '-
            'GIU260123
            txtRicercaClienteSede.AutoPostBack = False : txtRicercaNContr.AutoPostBack = False
            composeChiave = String.Format("{0}_{1}",
            "txtRicercaClienteSede.Text", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            txtRicercaClienteSede.Text = myObject
            '-
            composeChiave = String.Format("{0}_{1}",
            "txtRicercaNContr.Text", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            txtRicercaNContr.Text = myObject
            txtRicercaClienteSede.AutoPostBack = True : txtRicercaNContr.AutoPostBack = True
            '-
            txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
            composeChiave = String.Format("{0}_{1}",
                "txtDallaData.Text", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If IsDate(myObject) Then
                    txtDallaData.Text = myObject
                Else
                    txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
                End If
            Catch ex As Exception
                txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
            End Try
            '-
            composeChiave = String.Format("{0}_{1}",
            "txtAllaData.Text", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If IsDate(myObject) Then
                    txtAllaData.Text = myObject
                Else
                    txtAllaData.Text = Now.Date.ToString("31/12/" + "yyyy")
                End If
            Catch ex As Exception
                txtAllaData.Text = Now.Date.ToString("31/12/" + "yyyy")
            End Try
            txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
            '-
            myObject = txtDallaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtDallaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            myObject = txtAllaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtAllaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '----------------------------------
            txtDallaData.Enabled = True : txtAllaData.Enabled = True
            btnRicerca.Enabled = True : btnVerbale.Enabled = True : btnVerbale2.Enabled = True : btnElencoSc.Enabled = True
            imgBtnShowCalendarDD.Enabled = True : imgBtnShowCalendarAD.Enabled = True
            '-
            chkSoloDaEv.Enabled = True
            chkRespArea.Enabled = True
            ddlClienti.Enabled = True
            txtRicercaClienteSede.Enabled = True : txtRicercaNContr.Enabled = True 'giu240123
            '-
            ddlClienti.AutoPostBack = False
            composeChiave = String.Format("{0}_{1}",
            "ddlClienti", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If Not (myObject Is Nothing) Then
                    ddlClienti.Items.Clear()
                    For I = 0 To myObject.Items.Count - 1
                        ddlClienti.Items.Add(myObject.Items(I).Text)
                        ddlClienti.Items(I).Value = myObject.Items(I).Value
                    Next
                    composeChiave = String.Format("{0}_{1}",
                    "ddlClienti.SelectedValue", UtenteConnesso.Codice)
                    GetObjectToCache(composeChiave, myObject)
                    If Not (myObject Is Nothing) Then
                        If myObject.ToString.Trim <> "" Then
                            PosizionaItemDDL(myObject, ddlClienti)
                        End If
                    End If
                    'giu140224
                    DDLSediApp.Enabled = True
                    DDLSediApp.AutoPostBack = False
                    composeChiave = String.Format("{0}_{1}",
                    "DDLSediApp", UtenteConnesso.Codice)
                    GetObjectToCache(composeChiave, myObject)
                    Try
                        If Not (myObject Is Nothing) Then
                            DDLSediApp.Items.Clear()
                            For I = 0 To myObject.Items.Count - 1
                                DDLSediApp.Items.Add(myObject.Items(I).Text)
                                DDLSediApp.Items(I).Value = myObject.Items(I).Value
                            Next
                            composeChiave = String.Format("{0}_{1}",
                            "DDLSediApp.SelectedValue", UtenteConnesso.Codice)
                            GetObjectToCache(composeChiave, myObject)
                            If Not (myObject Is Nothing) Then
                                If myObject.ToString.Trim <> "" Then
                                    PosizionaItemDDL(myObject, DDLSediApp)
                                End If
                            End If
                        Else
                            DDLSediApp.Items.Clear()
                        End If
                    Catch ex As Exception
                        DDLSediApp.Items.Clear()
                    End Try
                    DDLSediApp.AutoPostBack = True
                    '-
                Else
                    'giu140224
                    myObject = SWSI
                    composeChiave = String.Format("{0}_{1}",
                    "GetPrimaDataSc", UtenteConnesso.Codice)
                    SetObjectToCache(composeChiave, myObject)
                    '-----------------------------------------
                    Call GetPrimaDataSc() 'giu140224
                End If
            Catch ex As Exception
                ddlClienti.Items.Clear()
                DDLSediApp.Items.Clear()
            End Try
            ddlClienti.AutoPostBack = True : DDLSediApp.AutoPostBack = True
            '-
            composeChiave = String.Format("{0}_{1}",
            "chkSoloDaEv.Checked", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If Not (myObject Is Nothing) Then
                    chkSoloDaEv.AutoPostBack = False
                    chkSoloDaEv.Checked = myObject
                    chkSoloDaEv.AutoPostBack = True
                End If
            Catch ex As Exception
                chkSoloDaEv.AutoPostBack = True
            End Try

            '-
            composeChiave = String.Format("{0}_{1}",
            "chkRespArea.Checked", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If Not (myObject Is Nothing) Then
                    chkRespArea.AutoPostBack = False
                    chkRespArea.Checked = myObject
                    chkRespArea.AutoPostBack = True
                End If
            Catch ex As Exception
                chkRespArea.AutoPostBack = True
            End Try
            '-
            composeChiave = String.Format("{0}_{1}",
            "chkSingoloNS.Checked", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            If Not (myObject Is Nothing) Then
                chkSingoloNS.AutoPostBack = False : chkSingoloNS2.AutoPostBack = False
                chkSingoloNS.Checked = myObject
                chkSingoloNS2.Checked = chkSingoloNS.Checked
                chkSingoloNS.AutoPostBack = True : chkSingoloNS2.AutoPostBack = True
                '-
                lblNoteIntervento.Visible = chkSingoloNS.Checked
                txtNoteDocumento.Visible = chkSingoloNS.Checked
                lblInfoNoteIntervento.Visible = Not chkSingoloNS.Checked
                lblAllaPresenzaDi.Visible = chkSingoloNS.Checked
                txtAllaPresezaDi.Visible = chkSingoloNS.Checked
            End If
            '-
            composeChiave = String.Format("{0}_{1}",
            "chkAllScCA.Checked", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Try
                If Not (myObject Is Nothing) Then
                    chkAllScCA.AutoPostBack = False
                    chkAllScCA.Checked = myObject
                    chkAllScCA.AutoPostBack = True
                End If
            Catch ex As Exception
                chkAllScCA.AutoPostBack = True
            End Try
            '-
            composeChiave = String.Format("{0}_{1}",
            CSTSCADATTCA, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            If Not (myObject Is Nothing) Then 'Session(CSTSCADATTCA) Is Nothing) Then
                DVScadAtt = myObject 'Session(CSTSCADATTCA)
                lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU280222
            Else
                lblTotaleAtt.Text = ""
                DVScadAtt = myObject 'Session(CSTSCADATTCA)
                lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU280222
            End If
        End If
        'XKE' PRIMA I VALORI POSSONO CAMBIARE SE RECUPERA LA SESSIONE
        If ddlClienti.SelectedValue.Trim <> "" Then
            GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = False
        Else
            GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = True
        End If
        '-
        If chkRespArea.Checked Then
            GridViewPrevT.Columns.Item(CellIdxT.RespVisite).Visible = True
        Else
            GridViewPrevT.Columns.Item(CellIdxT.RespVisite).Visible = False
        End If
    End Sub

    'giu270520 prima data scadenza ATTIVITA
    Private Function GetPrimaDataSc() As String
        GetPrimaDataSc = ""
        'giu140224 ricarico nel LOAD OPPURE NEL CHANGE DATE E EVASE SI/NO
        composeChiave = String.Format("{0}_{1}",
        "GetPrimaDataSc", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject Is Nothing Then
            'ricarico
        ElseIf String.IsNullOrEmpty(myObject) Then
            'ricarico
        ElseIf myObject <> SWSI Then
            Exit Function
        End If
        myObject = SWNO
        composeChiave = String.Format("{0}_{1}",
        "GetPrimaDataSc", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-----------------------------------------
        'giu010523
        composeChiave = String.Format("{0}_{1}",
        "RicaricaDati", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject Is Nothing Then
            'ricarico
        ElseIf String.IsNullOrEmpty(myObject) Then
            'ricarico
        ElseIf myObject = SWNO Then
            Exit Function
        End If
        myObject = SWNO
        composeChiave = String.Format("{0}_{1}",
        "RicaricaDati", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-----------------------------------------
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        'giu150224 selezione Resp.Visita/Area anche a livello di Dettaglio
        '''strSQL = "SELECT ContrattiT.Cod_Cliente, ContrattiD.DataSc, Clienti.Rag_Soc, Clienti.Denominazione " & _
        '''                "FROM     ContrattiT INNER JOIN " & _
        '''                "ContrattiD ON ContrattiT.IDDocumenti = ContrattiD.IDDocumenti INNER JOIN " & _
        '''                "RespArea ON ContrattiT.RespArea = RespArea.Codice INNER JOIN " & _
        '''                "RespVisite ON ContrattiT.RespVisite = RespVisite.Codice INNER JOIN " & _
        '''                "Clienti ON ContrattiT.Cod_Cliente = Clienti.Codice_CoGe " & _
        '''                "WHERE  (ContrattiT.StatoDoc < 3)  AND ContrattiD.DurataNum=1 "
        '-
        strSQL = "SELECT ContrattiT.Cod_Cliente, ContrattiD.DataSc, Clienti.Rag_Soc, Clienti.Denominazione " &
                    "FROM ContrattiT INNER JOIN " &
                    "ContrattiD ON ContrattiT.IDDocumenti = ContrattiD.IDDocumenti LEFT OUTER JOIN " &
                    "DestClienti ON ContrattiT.Cod_Filiale = DestClienti.Progressivo LEFT OUTER JOIN " &
                    "DestClienti AS DestClientiD ON ContrattiD.Cod_Filiale = DestClientiD.Progressivo LEFT OUTER JOIN " &
                    "RespArea AS RespAreaD ON ContrattiD.RespArea = RespAreaD.Codice LEFT OUTER JOIN " &
                    "RespVisite AS RespVisiteD ON ContrattiD.RespVisite = RespVisiteD.Codice LEFT OUTER JOIN " &
                    "AnagrProvv ON ContrattiT.IDAnagrProvv = AnagrProvv.IDAnagrProvv LEFT OUTER JOIN " &
                    "Clienti ON ContrattiT.Cod_Cliente = Clienti.Codice_CoGe LEFT OUTER JOIN " &
                    "CausMag ON ContrattiT.Cod_Causale = CausMag.Codice LEFT OUTER JOIN " &
                    "RespArea ON ContrattiT.RespArea = RespArea.Codice LEFT OUTER JOIN " &
                    "RespVisite ON ContrattiT.RespVisite = RespVisite.Codice " &
                    "WHERE  (ContrattiT.StatoDoc < 3)  AND ContrattiD.DurataNum=1 "
        '---------
        'GIU110222 RICHIESTA Francesca
        'GIU260123
        myObject = txtRicercaClienteSede.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaClienteSede.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = txtRicercaNContr.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaNContr.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
        If Not IsDate(txtDallaData.Text.Trim) Then
            txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
        End If
        myObject = txtDallaData.Text
        composeChiave = String.Format("{0}_{1}",
        "txtDallaData.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        If Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.Text = Now.Date.ToString("31/12/" + "yyyy")
        End If
        myObject = txtAllaData.Text
        composeChiave = String.Format("{0}_{1}",
        "txtAllaData.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        If CDate(txtDallaData.Text.Trim) > CDate(txtAllaData.Text.Trim) Then
            txtDallaData.Text = txtAllaData.Text.Trim
            myObject = txtDallaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtDallaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
        strSQL += " AND DataSc >= CONVERT(datetime, '" & txtDallaData.Text.Trim & "', 103) "
        strSQL += " AND DataSc <= CONVERT(datetime, '" & txtAllaData.Text.Trim & "', 103) "
        '-----------------------------
        If chkSoloDaEv.Checked = True Then
            strSQL += " AND (ContrattiD.Qta_Ordinata <> ContrattiD.Qta_Evasa)  "
        End If
        Dim myRespVisita As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            myRespVisita = ""
        Else
            myRespVisita = Session(CSTUTENTE).ToString.Trim
        End If
        If chkRespArea.Checked = False Then
            If myRespVisita.Trim <> "" Then
                'GIU150224 strSQL += "AND (RespVisite.Descrizione LIKE N'%" & Controlla_Apice(myRespVisita.Trim) & "%') "
                strSQL += " AND (CASE WHEN ContrattiD.RespVisite IS NULL THEN ISNULL(RespVisite.Descrizione, '') ELSE ISNULL(RespVisiteD.Descrizione, '') END LIKE N'%" & Controlla_Apice(myRespVisita.Trim) & "%') "
            End If
        Else
            If myRespVisita.Trim <> "" Then
                strSQL += " AND (CASE WHEN ContrattiD.RespArea IS NULL THEN ISNULL(RespArea.Descrizione, '') ELSE ISNULL(RespAreaD.Descrizione, '') END LIKE N'%" & Controlla_Apice(myRespVisita.Trim) & "%' OR "
                strSQL += " CASE WHEN ContrattiD.RespVisite IS NULL THEN ISNULL(RespVisite.Descrizione, '') ELSE ISNULL(RespVisiteD.Descrizione, '') END LIKE N'%" & Controlla_Apice(myRespVisita.Trim) & "%') "
            End If
        End If

        strSQL += " GROUP BY ContrattiD.DataSc, Clienti.Rag_Soc, Clienti.Denominazione, ContrattiT.Cod_Cliente " &
                          "ORDER BY Clienti.Rag_Soc,Clienti.Denominazione,ContrattiD.DataSc"
        '--------
        Dim ds As New DataSet
        Try
            GetPrimaDataSc = ""
            DDLSediApp.Items.Clear()
            ddlClienti.Items.Clear() : Dim SaveCliente As String = "" : Dim Comodo As String = "" : Dim Ind As Integer = 1
            ddlClienti.Items.Add("")
            ddlClienti.Items(0).Value = ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataSc")) Then
                        GetPrimaDataSc = Format(ds.Tables(0).Rows(0).Item("DataSc"), FormatoData)
                    End If
                    '-Carico la combo CLIENTI
                    For Each rsDate In ds.Tables(0).Select("", "Rag_Soc,Denominazione,DataSc")
                        Comodo = IIf(IsDBNull(rsDate!Rag_Soc), "", rsDate!Rag_Soc.ToString.Trim) + IIf(IsDBNull(rsDate!Denominazione), "", " " + rsDate!Denominazione.ToString.Trim)
                        If Comodo <> SaveCliente Then
                            ddlClienti.Items.Add(Comodo + " (" + IIf(IsDBNull(rsDate!DataSc), "", Format(rsDate!DataSc, FormatoData)) + ")" + " (" + IIf(IsDBNull(rsDate!Cod_Cliente), "", rsDate!Cod_Cliente.ToString.Trim) + ")")
                            ddlClienti.Items(Ind).Value = IIf(IsDBNull(rsDate!Cod_Cliente), "", rsDate!Cod_Cliente.ToString.Trim)
                            SaveCliente = Comodo
                            Ind += 1
                        End If
                    Next
                    'ok fatto
                End If
            End If
            ObjDB = Nothing
            myObject = ddlClienti
            composeChiave = String.Format("{0}_{1}",
            "ddlClienti", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        Catch Ex As Exception
            GetPrimaDataSc = ""
            Exit Function
        End Try
    End Function
    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        If (aDataViewScAtt Is Nothing) Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
        End If
        If GridViewPrevT.Rows.Count = 0 Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
        End If
        If btnRicerca.BackColor = SEGNALA_KO Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
        End If
        'If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        If myObject <> SWOPNESSUNA Then
            lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica!!!"
        End If
        If ddlClienti.SelectedValue.Trim <> "" Then
            'NULLA
        Else
            Call GetPrimaDataSc()
        End If
        '-
        Call CaricaDatiScAtt(False)
    End Sub
    'giu190620 
    'giu06032023 estraggo le scadenze SELEZIONATE MA NEL CASO IN CUI VI SIA UNA OLTRE MA DELLO STESSO PERIODO OK VIENE CONSIDERATA
    Private Function CaricaDatiScAtt(ByVal SWElenco As Boolean) As Boolean
        CaricaDatiScAtt = False
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject <> SWOPNESSUNA Then 'giu130423
            lblMessAttivita.Text = "Impossibilie elaborare Funzione di CaricaDatiScAtt in fase di modifica: " + Format(Now, "dd/MM/yyyy, HH:mm:ss")
            Exit Function
        End If
        SetLnk()
        Dim strFiltroRicerca As String = ""
        Dim SWSingolo As Boolean = False
        'GIU260123
        'GIU260123
        myObject = txtRicercaClienteSede.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaClienteSede.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = txtRicercaNContr.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaNContr.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Function
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Function
        End If
        '---------
        txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
        myObject = txtDallaData.Text
        composeChiave = String.Format("{0}_{1}",
        "txtDallaData.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = txtAllaData.Text
        composeChiave = String.Format("{0}_{1}",
        "txtAllaData.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
        '---------

        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        '-
        myObject = ddlClienti
        composeChiave = String.Format("{0}_{1}",
        "ddlClienti", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = ddlClienti.SelectedValue
        composeChiave = String.Format("{0}_{1}",
        "ddlClienti.SelectedValue", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = DDLSediApp
        composeChiave = String.Format("{0}_{1}",
        "DDLSediApp", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = DDLSediApp.SelectedValue
        composeChiave = String.Format("{0}_{1}",
        "DDLSediApp.SelectedValue", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = IIf(chkSoloDaEv.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSoloDaEv.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = IIf(chkRespArea.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkRespArea.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = IIf(chkSingoloNS.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSingoloNS.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = IIf(chkAllScCA.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkAllScCA.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        'PARAMETRI SP
        SqlDSPrevTElenco.SelectParameters("DallaData").DefaultValue = txtDallaData.Text.Trim
        If SWElenco = False Then 'giu150223
            SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = txtAllaData.Text.Trim
        ElseIf chkScadAnno.Checked = True Then
            SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = "31/12/" + CDate(txtAllaData.Text.Trim).Year.ToString.Trim
        Else
            SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = txtAllaData.Text.Trim
        End If
        strFiltroRicerca = "Elenco Scadenze Attività " + IIf(chkSoloDaEv.Checked, "non ancora evase", " tutte") + " nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        '-
        'GIU290623 NON UTILIZZATO E NE GESTITO NELLA SP
        SqlDSPrevTElenco.SelectParameters("Escludi").DefaultValue = True 'chkNoEscludiInvioM.Checked 
        SWSingolo = False
        SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("SoloDaEv").DefaultValue = chkSoloDaEv.Checked
        '' ''------------------------
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        myObject = SWTD(TD.ContrattoAssistenza)
        composeChiave = String.Format("{0}_{1}",
            CSTTIPODOC, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDOCUMENTI) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
            IDDOCUMENTI, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDURATANUMRIGA) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(CSTSERIELOTTO) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtNoteDocumento.Text = ""
        txtAllaPresezaDi.Text = ""
        txtEmailInvio.Text = ""
        '' ''-
        ImpostaFiltro()
        aDataViewScAtt = Nothing
        aDataViewScAtt = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)

        Dim dsDoc As New DSDocumenti
        If (aDataViewScAtt Is Nothing) Then
            'Session("dsDocElencoScad") = dsDoc
            myObject = dsDoc
            composeChiave = String.Format("{0}_{1}",
            "dsDocElencoScad", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)

            aDataViewScAtt = New DataView(dsDoc.ScadAtt)
            'Session("aDataViewScAtt") = aDataViewScAtt
            myObject = aDataViewScAtt
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            BuidDett()
            Exit Function
        End If
        'giu180620 GIU160520 PER SAPERE I CODICI VISITA     If InStr(myCodVisita, RowScadAtt.Cod_Articolo.Trim) > 0 Then
        Dim myCodVisita As String = GetMyCodVisita() 'giu250123
        '-----------------------------------------------------------------
        Dim dc As DataColumn
        Dim SWApp1 As Boolean = False
        Dim SWApp2 As Boolean = False
        Dim SWApp3 As Boolean = False
        Dim SWApp4 As Boolean = False
        'giu060323 per selezionare le scadenze oltre a quelle richieste ma nello stesso periodo
        Dim myIDDocumenti As Long = -1
        Dim myDurataNumRiga As Integer = -1
        Dim myNSerieLotto As String = "" 'giu230923
        Dim SWSelezionato As Boolean = False 'giu010523
        '--------------------------------------------------------------------------------------
        Try
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = "IDDocumenti,DurataNumRiga,SerieLotto,DataSc,Riga" 'GIU060323
            For i = 0 To aDataViewScAtt.Count - 1
                'giu190620 
                If SWElenco = False Then
                    If myIDDocumenti <> aDataViewScAtt.Item(i)("IDDocumenti") Then 'giu010523
                        myIDDocumenti = aDataViewScAtt.Item(i)("IDDocumenti")
                        myDurataNumRiga = aDataViewScAtt.Item(i)("DurataNumRiga")
                        myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                        SWSelezionato = False
                    End If
                    If InStr(myCodVisita, aDataViewScAtt.Item(i)("Cod_Articolo").ToString.Trim) > 0 Or
                            aDataViewScAtt.Item(i)("SiglaCA").ToString.Trim <> "CM" Then 'PER VISUAL.ANCHE I CT
                        'ok
                        SWSelezionato = True
                        myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                    Else 'scarto SOLO CHECK  
                        Try
                            If aDataViewScAtt.Item(i)("Cod_Articolo").ToString.Trim = "" Then
                                Continue For
                            ElseIf i + 1 <= aDataViewScAtt.Count - 1 Then
                                If myIDDocumenti <> aDataViewScAtt.Item(i + 1)("IDDocumenti") Then 'giu010523
                                    If SWSelezionato = True Then
                                        'giu230923 Continue For
                                        If myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto")) Then
                                            Continue For
                                        Else
                                            myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                                        End If
                                    Else
                                        'almeno l'ultimo lo prendo
                                    End If
                                Else 'GIU230923 se i ck sono già stati evasi e rimagono solo consumabili
                                    'quindi seleziono per SerieLotto
                                    ''' Continue For
                                    If SWSelezionato = True Then
                                        If myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto")) Then
                                            Continue For
                                        Else
                                            myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                                        End If
                                    Else
                                        SWSelezionato = True
                                        myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                                    End If
                                End If
                            Else
                                If SWSelezionato = True Then
                                    'giu230923 Continue For
                                    If myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto")) Then
                                        Continue For
                                    Else
                                        myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                                    End If
                                Else
                                    'almeno l'ultimo lo prendo
                                    myNSerieLotto = IIf(IsDBNull(aDataViewScAtt.Item(i)("SerieLotto")), "", aDataViewScAtt.Item(i)("SerieLotto"))
                                End If
                            End If
                        Catch ex As Exception
                            '''Continue For
                        End Try
                        'giu010523 Continue For
                    End If
                End If
                If DDLSediApp.SelectedValue.Trim <> "" Then
                    SWApp1 = False : SWApp2 = False : SWApp3 = False : SWApp4 = False
                    If InStr(DDLSediApp.SelectedValue.Trim, aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim) > 0 Then
                        SWApp1 = True
                    End If
                    If InStr(DDLSediApp.SelectedValue.Trim, aDataViewScAtt.Item(i)("IndirApp").ToString.Trim) > 0 Then
                        SWApp2 = True
                    End If
                    If InStr(DDLSediApp.SelectedValue.Trim, aDataViewScAtt.Item(i)("LocApp").ToString.Trim) > 0 Then
                        SWApp3 = True
                    End If
                    If InStr(DDLSediApp.SelectedValue.Trim, aDataViewScAtt.Item(i)("PrApp").ToString.Trim) > 0 Then
                        SWApp4 = True
                    End If
                    If SWApp1 = True And SWApp2 = True And SWApp3 = True And SWApp4 = True Then
                        'ok
                    Else
                        Continue For
                    End If
                End If
                'giu060323 per selezionare le scadenze oltre a quelle richieste ma nello stesso periodo
                If SWElenco = True Then
                    If CDate(aDataViewScAtt.Item(i)("DataSc")) > CDate(txtAllaData.Text) Then
                        If myIDDocumenti <> aDataViewScAtt.Item(i)("IDDocumenti") Then
                            Continue For
                        ElseIf myDurataNumRiga = aDataViewScAtt.Item(i)("DurataNumRiga") Then
                            'ok lo prendo comunque
                        Else
                            Continue For
                        End If
                    Else
                        myIDDocumenti = aDataViewScAtt.Item(i)("IDDocumenti")
                        myDurataNumRiga = aDataViewScAtt.Item(i)("DurataNumRiga")
                    End If
                End If
                '--------------------------------------------------------------------------------------
                Dim newRow As DSDocumenti.ScadAttRow = dsDoc.ScadAtt.NewScadAttRow
                newRow.BeginEdit()
                For Each dc In dsDoc.ScadAtt.Columns
                    If UCase(dc.ColumnName) = "NOSTAMPA" Then
                        newRow.Item(dc.ColumnName) = False
                    ElseIf UCase(dc.ColumnName) = "FILTRORICERCA" Then
                        newRow.Item(dc.ColumnName) = strFiltroRicerca.Trim
                    ElseIf UCase(dc.ColumnName) = "SWNOVISITA" Then
                        newRow.Item(dc.ColumnName) = 0
                    ElseIf UCase(dc.ColumnName) = "SWSINGOLO" Then
                        newRow.Item(dc.ColumnName) = SWSingolo
                        'DALLA TESTATA CAMPO ASPETTO
                        ' ''ElseIf UCase(dc.ColumnName) = "EMAILVERBALE" Then
                        ' ''    newRow.Item(dc.ColumnName) = ""
                    Else
                        If IsDBNull(aDataViewScAtt.Item(i)(dc.ColumnName)) Then
                            newRow.Item(dc.ColumnName) = DBNull.Value
                        Else
                            newRow.Item(dc.ColumnName) = aDataViewScAtt.Item(i)(dc.ColumnName)
                        End If
                    End If
                Next
                '-
                newRow.EndEdit()
                dsDoc.ScadAtt.AddScadAttRow(newRow)
            Next
            ' ''dsDoc.ScadAtt.AcceptChanges()
            Dim rsScadAtt As DSDocumenti.ScadAttRow
            For Each rsScadAtt In dsDoc.Tables("ScadAtt").Select("")
                rsScadAtt.BeginEdit()
                If rsScadAtt.IsSerieLottoNull Then rsScadAtt.SerieLotto = "" 'giu010523
                rsScadAtt!SerieLotto = Formatta.FormattaNomeFile(rsScadAtt!SerieLotto) 'GIU180723
                'dalla SP non arrivera' mai NULL altrimenti va in errore
                If rsScadAtt.IsEmailVerbaleNull Then
                    rsScadAtt.EmailVerbale = ""
                ElseIf rsScadAtt.EmailVerbale.Trim = "CARTONI" Then
                    rsScadAtt.EmailVerbale = ""
                End If
                rsScadAtt.EmailVerbale = "" 'GIU270423 PER OTTENERE L'EMAIL ORIGINARIA E SE MODIFICATA ORA SARA' RIPORTATA NELLE NOTE E LI PRESA 
                If rsScadAtt.IsEmailNull Then rsScadAtt.Email = ""
                If rsScadAtt.IsEmailDestAppNull Then rsScadAtt.EmailDestApp = ""
                If rsScadAtt.IsEmailDestNull Then rsScadAtt.EmailDest = ""
                If rsScadAtt.IsEmailInvioScadNull Then rsScadAtt.EmailInvioScad = ""
                '-
                If rsScadAtt.EmailVerbale.Trim <> "" Then
                    rsScadAtt.EmailVerbale = rsScadAtt.EmailVerbale.Trim
                ElseIf rsScadAtt.EmailDestApp.Trim <> "" Then
                    rsScadAtt.EmailVerbale = rsScadAtt.EmailDestApp.Trim
                ElseIf rsScadAtt.EmailDest.Trim <> "" Then
                    rsScadAtt.EmailVerbale = rsScadAtt.EmailDest.Trim
                ElseIf rsScadAtt.EmailInvioScad.Trim <> "" Then
                    rsScadAtt.EmailVerbale = rsScadAtt.EmailInvioScad.Trim
                Else
                    rsScadAtt.EmailVerbale = rsScadAtt.Email.Trim
                End If
                'giu220123
                If rsScadAtt.IsNoteRitiroNull Then rsScadAtt.NoteRitiro = ""
                rsScadAtt!NoteRitiro = NoCarSpecNoteSL(rsScadAtt!NoteRitiro.ToString.Trim) 'GIU180723
                If rsScadAtt.IsNoteSerieLottoNull Then rsScadAtt.NoteSerieLotto = ""
                rsScadAtt.NoteSerieLotto = GetNoteSerieLotto(rsScadAtt.NoteRitiro.Trim, rsScadAtt.SerieLotto.Trim)
                '---------
                rsScadAtt.EndEdit()
            Next
            dsDoc.ContrattiD.AcceptChanges()
        Catch ex As Exception
            lblMessAttivita.Text = "Errore: Seleziona elenco scadenze: " & ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Seleziona elenco scadenze: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '-
        'Session("dsDocElencoScad") = dsDoc
        myObject = dsDoc
        composeChiave = String.Format("{0}_{1}",
        "dsDocElencoScad", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        aDataViewScAtt = New DataView(dsDoc.ScadAtt)
        'Session("aDataViewScAtt") = aDataViewScAtt
        myObject = aDataViewScAtt
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '---------------------------------
        BuidDett()
        CaricaDatiScAtt = True
    End Function
    'giu180723 per i caratteri speciali NSerie/Lotto
    Private Function NoCarSpecNoteSL(ByVal pNoteRitiro As String) As String
        NoCarSpecNoteSL = ""
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I))
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
            Next
            'ok qui ripasso tutta la stringa senza i caratteri
            For II = 0 To ListaSL.Count - 1
                If NoCarSpecNoteSL.Trim <> "" Then
                    NoCarSpecNoteSL += "§"
                End If
                NoCarSpecNoteSL += ListaSL.Item(II).Trim
            Next
            If NoCarSpecNoteSL.Trim = "" Then
                NoCarSpecNoteSL = pNoteRitiro.Trim
            End If
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            'vediamo dopo Call SetNoteSLALLApp(pNoteRitiro)
            NoCarSpecNoteSL = pNoteRitiro.Trim
        End If
    End Function
    'giu220122
    Private Function GetNoteSerieLotto(ByVal pNoteRitiro As String, ByVal pSerieLotto As String) As String
        GetNoteSerieLotto = ""
        If pNoteRitiro.Trim = "" Then Exit Function
        '-
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        '-
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I)) 'giu070523
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
                If mySL.Trim = pSerieLotto.Trim Then
                    GetNoteSerieLotto = myNoteRitiro.Trim
                    Exit For
                End If
            Next
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            GetNoteSerieLotto = pNoteRitiro.Trim
        End If
    End Function
    Private Sub ImpostaFiltro()
        SqlDSPrevTElenco.FilterExpression = ""
        Dim myRespVisita As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            myRespVisita = ""
        Else
            myRespVisita = Session(CSTUTENTE).ToString.Trim
        End If
        'GIU210122 SE E' RESP.AREA ESTRAGGO TUTTO RICHIESTA ELENA 10/11
        If chkRespArea.Checked = False Then
            If myRespVisita.Trim <> "" Then
                If txtRicercaNContr.Text.Trim <> "" Then
                    SqlDSPrevTElenco.FilterExpression = "(DesRespArea LIKE '%" & Controlla_Apice(myRespVisita.Trim) & "%' OR "
                    SqlDSPrevTElenco.FilterExpression += "DesRespVisite LIKE '%" & Controlla_Apice(myRespVisita.Trim) & "%')"
                Else
                    SqlDSPrevTElenco.FilterExpression = "DesRespVisite LIKE '%" & Controlla_Apice(myRespVisita.Trim) & "%'"
                End If
            End If
        Else
            If myRespVisita.Trim <> "" Then
                SqlDSPrevTElenco.FilterExpression = "(DesRespArea LIKE '%" & Controlla_Apice(myRespVisita.Trim) & "%' OR "
                SqlDSPrevTElenco.FilterExpression += "DesRespVisite LIKE '%" & Controlla_Apice(myRespVisita.Trim) & "%')"
            End If
        End If
        '-
        'giu240123 ricerca per Cliente/Sedi oppure N° Contratto
        txtRicercaNContr.BackColor = SEGNALA_OK
        If txtRicercaNContr.Text.Trim <> "" Then
            If Not IsNumeric(txtRicercaNContr.Text.Trim) Then
                txtRicercaNContr.BackColor = SEGNALA_KO
                txtRicercaNContr.AutoPostBack = False
                txtRicercaNContr.Text = ""
                txtRicercaNContr.AutoPostBack = True
            End If
        End If
        If txtRicercaNContr.Text.Trim <> "" Then
            If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Numero = " & txtRicercaNContr.Text.Trim & ""
            If ddlClienti.SelectedValue.Trim <> "" Then
                ddlClienti.AutoPostBack = False
                ddlClienti.SelectedIndex = -1
                ddlClienti.AutoPostBack = True
                '-
                DDLSediApp.AutoPostBack = False
                DDLSediApp.SelectedIndex = -1
                DDLSediApp.AutoPostBack = True
            End If
            If txtRicercaClienteSede.Text.Trim <> "" Then
                txtRicercaClienteSede.AutoPostBack = False
                txtRicercaClienteSede.Text = ""
                txtRicercaClienteSede.AutoPostBack = True
            End If
        Else 'non filto null'altro
            If ddlClienti.SelectedValue.Trim <> "" Then
                If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "Cod_Cliente = '" & Controlla_Apice(ddlClienti.SelectedValue.Trim) & "'"
                'GIU240123
                If txtRicercaClienteSede.Text.Trim <> "" Then
                    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                        SqlDSPrevTElenco.FilterExpression += " AND "
                    End If
                    SqlDSPrevTElenco.FilterExpression += "RagSocApp LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%'"
                End If
                '--------
            Else
                'GIU240123
                If txtRicercaClienteSede.Text.Trim <> "" Then
                    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                        SqlDSPrevTElenco.FilterExpression += " AND ("
                    End If
                    SqlDSPrevTElenco.FilterExpression += "Rag_SocDenom LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%' OR "
                    SqlDSPrevTElenco.FilterExpression += "RagSocApp LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%')"
                End If
                '--------
            End If
        End If
        'GIU260123
        myObject = txtRicercaClienteSede.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaClienteSede.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = txtRicercaNContr.Text.Trim
        composeChiave = String.Format("{0}_{1}",
        "txtRicercaNContr.Text", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        '------------------------------------------------------
        myObject = ddlClienti.SelectedValue
        composeChiave = String.Format("{0}_{1}",
            "ddlClienti.SelectedValue", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
    End Sub
    Private Sub BuidDett()
        btnRicerca.BackColor = btnElencoSc.BackColor 'giu100222 SEGNALA_OKBTN
        btnVerbale.BackColor = btnElencoSc.BackColor
        btnVerbale2.BackColor = btnElencoSc.BackColor
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        'Session("aDataViewScAtt") = aDataViewScAtt
        myObject = aDataViewScAtt
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '---------
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = SWNO
        composeChiave = String.Format("{0}_{1}",
            SWMODIFICATO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'giu280222
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                    IDDOCUMENTI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(IDDURATANUMRIGA) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                    IDDURATANUMRIGA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(CSTSERIELOTTO) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                    CSTSERIELOTTO, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                txtNoteDocumento.Text = ""
                txtAllaPresezaDi.Text = ""
                txtEmailInvio.Text = ""
                DVScadAtt = Nothing
                DVScadAtt = New DataView
                'GIU041122
                myObject = DVScadAtt
                composeChiave = String.Format("{0}_{1}",
                    CSTSCADATTCA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '--
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
            End Try
        Else
            BtnSetEnabledTo(False)
            ' ''btnSblocca.Visible = False
            ' ''btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(IDDURATANUMRIGA) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
                IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtNoteDocumento.Text = ""
            txtAllaPresezaDi.Text = ""
            txtEmailInvio.Text = ""
            DVScadAtt = Nothing
            DVScadAtt = New DataView
            'GIU041122
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '--
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
        End If
    End Sub

    Private Function GetOpConnessi(ByVal lblUtente As String) As Boolean
        GetOpConnessi = True
        lblUltimoAccesso.Text = ""
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0
        strSQL = "SELECT * FROM Operatori WHERE Nome='" & Controlla_Apice(lblUtente.Trim) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If ds.Tables(0).Rows(ii).Item("Nome").ToString.Trim = lblUtente.Trim Then
                            If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraAccessoPrec")) Then
                                lblUltimoAccesso.Text = "Accesso precedente: " & Format(ds.Tables(0).Rows(ii).Item("DataOraAccessoPrec"), "dddd d MMMM yyyy, HH:mm")
                                labelIdentificaUtente.ToolTip = lblUltimoAccesso.Text.Trim
                                lblDataOdierna.ToolTip = lblUltimoAccesso.Text.Trim
                            End If
                            Exit For
                        End If
                        '-----
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            lblUltimoAccesso.Text = "Errore caricamento connessioni utenti.: " & Ex.Message.Trim
            GetOpConnessi = False
            Exit Function
        End Try
    End Function
    Private Sub LnkLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkLogOut.Click
        If (aDataViewScAtt Is Nothing) Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPCLI) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        If GridViewPrevT.Rows.Count = 0 Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPCLI) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim mySL As String = myObject
        Session(CSTSERIELOTTO) = mySL
        If IsNothing(mySL) Then
            mySL = ""
        End If
        If String.IsNullOrEmpty(mySL) Then
            mySL = ""
        End If
        If chkSingoloNS.Checked = False Then
            mySL = ""
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
           SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        If myObject <> "" Then 'Session(SWOP) <> "" Then
            lblMessAttivita.Text = "Attenzione: Completare prima l'operazione di Modifica: " & mySL.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!<br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
           SWOPCLI, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOPCLI) = myObject
        If myObject <> "" Then 'If Session(SWOPCLI) <> "" Then
            lblMessAttivita.Text = "Attenzione: Completare prima l'operazione di Modifica: " & mySL.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!<br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        myObject = IIf(chkSingoloNS.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSingoloNS.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session("mnuStatPR") = Nothing
        SessionUtility.LogOutUtente(Session, Response)
    End Sub

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        SetLnk()
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        'aDataViewScAtt = Session("aDataViewScAtt")
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        aDataViewScAtt = myObject
        '-
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(IDDURATANUMRIGA) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDURATANUMRIGA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(CSTSERIELOTTO) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                txtNoteDocumento.Text = ""
                txtAllaPresezaDi.Text = ""
                txtEmailInvio.Text = ""
                DVScadAtt = Nothing
                DVScadAtt = New DataView
                'GIU041122
                myObject = DVScadAtt
                composeChiave = String.Format("{0}_{1}",
                    CSTSCADATTCA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '--
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
            End Try
        Else
            BtnSetEnabledTo(False)
            ' ''btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDOCUMENTI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(IDDURATANUMRIGA) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtNoteDocumento.Text = ""
            txtAllaPresezaDi.Text = ""
            txtEmailInvio.Text = ""
            DVScadAtt = Nothing
            DVScadAtt = New DataView
            'GIU041122
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '--
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        SetLnk()
        GridViewPrevT.SelectedIndex = -1
        'aDataViewScAtt = Session("aDataViewScAtt")
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        aDataViewScAtt = myObject
        '-
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(IDDURATANUMRIGA) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDURATANUMRIGA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(CSTSERIELOTTO) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                txtNoteDocumento.Text = ""
                txtAllaPresezaDi.Text = ""
                txtEmailInvio.Text = ""
                DVScadAtt = Nothing
                DVScadAtt = New DataView
                'GIU041122
                myObject = DVScadAtt
                composeChiave = String.Format("{0}_{1}",
                    CSTSCADATTCA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '--
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
            End Try
        Else
            BtnSetEnabledTo(False)
            ' ''btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDOCUMENTI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(IDDURATANUMRIGA) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtNoteDocumento.Text = ""
            txtAllaPresezaDi.Text = ""
            txtEmailInvio.Text = ""
            DVScadAtt = Nothing
            DVScadAtt = New DataView
            'GIU041122
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '--
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
        End If
    End Sub
    Private Sub GridViewPrevT_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevT.Sorting
        SetLnk()
        Dim sortExpression As String = TryCast(ViewState("_GridView1LastSortExpression_"), String)
        Dim sortDirection As String = TryCast(ViewState("_GridView1LastSortDirection_"), String)

        If e.SortExpression <> sortExpression Then
            sortExpression = e.SortExpression
            sortDirection = "ASC"
        Else

            If sortDirection = "ASC" Then
                sortExpression = e.SortExpression
                sortDirection = "DESC"
            Else
                sortExpression = e.SortExpression
                sortDirection = "ASC"
            End If
        End If

        Try
            ViewState("_GridView1LastSortDirection_") = sortDirection
            ViewState("_GridView1LastSortExpression_") = sortExpression
            'aDataViewScAtt = Session("aDataViewScAtt")
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            aDataViewScAtt = myObject
            '-
            aDataViewScAtt.Sort = ""
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = sortExpression & " " + sortDirection
            GridViewPrevT.DataSource = aDataViewScAtt
            'GIU061122
            myObject = aDataViewScAtt
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            GridViewPrevT.DataBind()
        Catch
        End Try
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        SetLnk()
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            If (aDataViewScAtt Is Nothing) Then
                'aDataViewScAtt = Session("aDataViewScAtt")
                composeChiave = String.Format("{0}_{1}",
                "aDataViewScAtt", UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                aDataViewScAtt = myObject
                GridViewPrevT.DataSource = aDataViewScAtt
                GridViewPrevT.DataBind()
            End If
            Dim myRowIndex As Integer = GridViewPrevT.SelectedIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            Dim Stato As String = ""
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                myObject = Session(IDDOCUMENTI)
                composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(IDDURATANUMRIGA) = aDataViewScAtt.Item(myRowIndex).Item("DurataNumRiga").ToString.Trim
                myObject = Session(IDDURATANUMRIGA)
                composeChiave = String.Format("{0}_{1}",
                IDDURATANUMRIGA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(CSTSERIELOTTO) = Formatta.FormattaNomeFile(aDataViewScAtt.Item(myRowIndex).Item("SerieLotto").ToString.Trim) 'giu070523
                myObject = Session(CSTSERIELOTTO)
                composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                txtEmailInvio.Text = aDataViewScAtt.Item(myRowIndex).Item("EmailVerbale").ToString.Trim
                txtNoteDocumento.Text = GetNoteSL(aDataViewScAtt.Item(myRowIndex).Item("NoteRitiro").ToString.Trim)
                'giu290123 qui popolare txtAllaPresezaDi 
                txtAllaPresezaDi.Text = GetAllaPresenzaDi(txtNoteDocumento.Text)
                '---------------------------------------
                btnVerbale.BackColor = btnElencoSc.BackColor
                btnVerbale2.BackColor = btnElencoSc.BackColor
                BtnSetByStato(Stato)
                PopolaGridScAtt()
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(IDDURATANUMRIGA) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                IDDURATANUMRIGA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(CSTSERIELOTTO) = ""
                myObject = ""
                composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                txtNoteDocumento.Text = ""
                txtAllaPresezaDi.Text = ""
                txtEmailInvio.Text = ""
                DVScadAtt = Nothing
                DVScadAtt = New DataView
                'GIU041122
                myObject = DVScadAtt
                composeChiave = String.Format("{0}_{1}",
                    CSTSCADATTCA, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '--
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
            End Try
            '---------------------------------------------

        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDOCUMENTI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(IDDURATANUMRIGA) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtNoteDocumento.Text = ""
            txtAllaPresezaDi.Text = ""
            txtEmailInvio.Text = ""
            DVScadAtt = Nothing
            DVScadAtt = New DataView
            'GIU041122
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '--
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
        End Try
    End Sub
    'giu280222 NOTE PUNTUALI AL N° SERIE
    Private Function GetNoteSL(ByVal pNoteRitiro As String) As String
        GetNoteSL = ""
        '-
        Dim NSL As String = ""
        'Dim NSL As String = Session(CSTSERIELOTTO)
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        CSTSERIELOTTO, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        NSL = myObject
        Session(CSTSERIELOTTO) = NSL
        '-
        If IsNothing(NSL) Then
            NSL = ""
        End If
        If String.IsNullOrEmpty(NSL) Then
            NSL = ""
        End If
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        'Session(L_NSERIELOTTO) = Nothing
        myObject = Nothing
        composeChiave = String.Format("{0}_{1}",
        L_NSERIELOTTO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I)) 'giu070523
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
                If mySL.Trim = NSL.Trim Then
                    GetNoteSL = myNoteRitiro.Trim
                End If
            Next
            'Session(L_NSERIELOTTO) = ListaSL
            myObject = ListaSL
            composeChiave = String.Format("{0}_{1}",
            L_NSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            Call SetNoteSLALLApp(pNoteRitiro)
            GetNoteSL = pNoteRitiro.Trim
        End If
    End Function
    Private Function SetNoteSLALLApp(ByVal pNoteRitiro As String) As Boolean
        SetNoteSLALLApp = False
        'Dim myID As String = Session(IDDOCUMENTI)
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMENTI, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim myID As String = myObject
        Session(IDDOCUMENTI) = myID
        '-
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Or GridViewPrevT.Rows.Count = 0 Then
            lblMessAttivita.Text = "Attenzione, Nessun documento selezionato"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Serie,'') + ISNULL(Lotto,'') AS SerieLotto From ContrattiD " &
                 "WHERE (IDDocumenti = " + myID.Trim + ") AND (DurataNum =0) " &
                 "GROUP BY ISNULL(Serie,'') + ISNULL(Lotto,'')"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim ListaSL As New List(Of String)
        Dim mySL As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        mySL = Formatta.FormattaNomeFile(IIf(IsDBNull(row.Item("SerieLotto")), "", row.Item("SerieLotto"))) 'giu070523
                        If mySL.Trim <> "" Then
                            ListaSL.Add(mySL + "§" + pNoteRitiro.Trim)
                        End If
                    Next
                Else
                    'per adesso proseguo
                    lblMessAttivita.Text = "Errore: Assegna Note Intervento(SetNoteSLALLApp): Nessuna App.presente"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): Nessuna App.presente", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                'per adesso proseguo
                lblMessAttivita.Text = "Errore: Assegna Note Intervento(SetNoteSLALLApp): Nessuna App.presente"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): Nessuna App.presente", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            'per adesso proseguo
            lblMessAttivita.Text = "Errore: Assegna Note Intervento(SetNoteSLALLApp): " & Ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        SetNoteSLALLApp = True
        'Session(L_NSERIELOTTO) = ListaSL
        myObject = ListaSL
        composeChiave = String.Format("{0}_{1}",
            L_NSERIELOTTO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
    End Function
    '-----------------------------------
    Private Function GetAllaPresenzaDi(ByRef pNoteRitiro As String) As String
        GetAllaPresenzaDi = ""
        Dim StrDato() As String
        Dim myPos As Integer = InStr(pNoteRitiro, "|")
        Try
            If myPos > 0 Then
                StrDato = pNoteRitiro.Trim.Split("|")
                pNoteRitiro = StrDato(0)
                GetAllaPresenzaDi = StrDato(1)
            End If
            If Not (StrDato Is Nothing) Then 'giu270423
                pNoteRitiro += " "
                For I = 2 To StrDato.Count - 1
                    pNoteRitiro += StrDato(I)
                Next
            Else
                GetAllaPresenzaDi = ""
            End If
        Catch ex As Exception
            GetAllaPresenzaDi = ""
        End Try
        pNoteRitiro = Trim(pNoteRitiro)
        GetAllaPresenzaDi = Trim(GetAllaPresenzaDi)
    End Function
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))

        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataSc).Text) Then
                e.Row.Cells(CellIdxT.DataSc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataSc).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxT.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxT.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxT.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxT.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxT.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.Importo).Text = ""
                End If
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
            If Len(e.Row.Cells(CellIdxT.DesArt).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.DesArt).Text = Mid(e.Row.Cells(CellIdxT.DesArt).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.RagSoc).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.RagSoc).Text = Mid(e.Row.Cells(CellIdxT.RagSoc).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Denom).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Denom).Text = Mid(e.Row.Cells(CellIdxT.Denom).Text, 1, 20)
            End If
            'GIU200122
            ' ''If Len(e.Row.Cells(CellIdxT.IndirApp).Text.Trim) > 20 Then
            ' ''    e.Row.Cells(CellIdxT.IndirApp).Text = Mid(e.Row.Cells(CellIdxT.IndirApp).Text, 1, 20)
            ' ''End If
            If Len(e.Row.Cells(CellIdxT.LuogoApp).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.LuogoApp).Text = Mid(e.Row.Cells(CellIdxT.LuogoApp).Text, 1, 20)
            End If
            ' ''If Len(e.Row.Cells(CellIdxT.Referente).Text.Trim) > 20 Then
            ' ''    e.Row.Cells(CellIdxT.Referente).ToolTip = e.Row.Cells(CellIdxT.Referente).Text.Trim 'giu190123
            ' ''    e.Row.Cells(CellIdxT.Referente).Text = Mid(e.Row.Cells(CellIdxT.Referente).Text, 1, 20)
            ' ''End If
            If Len(e.Row.Cells(CellIdxT.Note).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Note).ToolTip = e.Row.Cells(CellIdxT.Note).Text.Trim 'giu190123
                e.Row.Cells(CellIdxT.Note).Text = Mid(e.Row.Cells(CellIdxT.Note).Text, 1, 20)
            End If
            '---------
            If Len(e.Row.Cells(CellIdxT.Riferimento).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Riferimento).Text = Mid(e.Row.Cells(CellIdxT.Riferimento).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest1).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest1).Text = Mid(e.Row.Cells(CellIdxT.Dest1).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest2).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest2).Text = Mid(e.Row.Cells(CellIdxT.Dest2).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest3).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest3).Text = Mid(e.Row.Cells(CellIdxT.Dest3).Text, 1, 20)
            End If
        End If
    End Sub
    Private Sub GridViewDettCAAtt_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettCAAtt.RowDataBound
        'giu080123 non va bene e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewDettCAAtt, "Select$" + e.Row.RowIndex.ToString()))

        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellCAAtt.DataSc).Text) Then
                e.Row.Cells(CellCAAtt.DataSc).Text = Format(CDate(e.Row.Cells(CellCAAtt.DataSc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellCAAtt.DataEv).Text) Then
                e.Row.Cells(CellCAAtt.DataEv).Text = Format(CDate(e.Row.Cells(CellCAAtt.DataEv).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellCAAtt.DataEvN).Text) Then
                e.Row.Cells(CellCAAtt.DataEvN).Text = Format(CDate(e.Row.Cells(CellCAAtt.DataEvN).Text), FormatoData).ToString
            End If
            '-
            If CBool(e.Row.Cells(CellCAAtt.SWModAgenti).Text.Trim) = False Then
                e.Row.Cells(CellCAAtt.DataEvN).BackColor = SEGNALA_OKBTN
                e.Row.Cells(CellCAAtt.DataEvN).Enabled = False
            Else
                e.Row.Cells(CellCAAtt.DataEvN).BackColor = SEGNALA_OK
                e.Row.Cells(CellCAAtt.DataEvN).Enabled = True
            End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
            ' ''If Len(e.Row.Cells(CellCAAtt.Articolo).Text.Trim) > 20 Then
            ' ''    e.Row.Cells(CellCAAtt.Articolo).Text = Mid(e.Row.Cells(CellCAAtt.Articolo).Text, 1, 20)
            ' ''End If
        End If
    End Sub

    Private Sub txtDaAllaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDallaData.TextChanged, txtAllaData.TextChanged
        'giu010523
        myObject = SWSI
        composeChiave = String.Format("{0}_{1}",
        "RicaricaDati", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '---------------------
        'GIU060322
        ddlClienti.AutoPostBack = False
        ddlClienti.Items.Clear()
        ddlClienti.Items.Add("")
        ddlClienti.Items(0).Value = ""
        ddlClienti.AutoPostBack = True
        '-
        DDLSediApp.AutoPostBack = False
        DDLSediApp.Items.Clear()
        DDLSediApp.Items.Add("")
        DDLSediApp.Items(0).Value = ""
        DDLSediApp.AutoPostBack = True
        'giu140224
        myObject = SWSI
        composeChiave = String.Format("{0}_{1}",
        "GetPrimaDataSc", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-----------------------------------------
        SvuodaGridT()
        sender.Text = ConvData(sender.Text.ToString.Trim)
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtAllaData.Text = txtDallaData.Text
            If sender.id = txtAllaData.ID Then
                btnRicerca.Focus()
            Else
                txtAllaData.Focus()
            End If
            ' ''txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            ' ''txtDallaData.Focus()
            ' ''Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
    End Sub
    Private Sub SetLnk()
        LnkVerbale.Visible = False
        lnkElencoSc.Visible = False ' ''LnkStampa.Visible = False : : btnOKModInviati.Visible = False
        LnkApriVerbale.Visible = False : LnkApriVerbale2.Visible = False
        lblMessDebug.Visible = False
    End Sub
    Private Sub SvuodaGridT()
        SetLnk()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        myObject = SWTD(TD.ContrattoAssistenza)
        composeChiave = String.Format("{0}_{1}",
        CSTTIPODOC, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDOCUMENTI) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMENTI, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDURATANUMRIGA) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        IDDURATANUMRIGA, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(CSTSERIELOTTO) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        CSTSERIELOTTO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtNoteDocumento.Text = "" : txtAllaPresezaDi.Text = ""
        txtEmailInvio.Text = ""
        txtEmailInvio.Enabled = False : txtNoteDocumento.Enabled = False 'giu200122
        txtAllaPresezaDi.Enabled = False
        btnRicerca.BackColor = SEGNALA_KO
        aDataViewScAtt = Nothing
        aDataViewScAtt = New DataView
        'Session("aDataViewScAtt") = aDataViewScAtt
        ' ''myObject = aDataViewScAtt
        ' ''composeChiave = String.Format("{0}_{1}", _
        ' ''    "aDataViewScAtt", UtenteConnesso.Codice)
        ' ''SetObjectToCache(composeChiave, myObject)
        '--
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.Enabled = True

        txtDallaData.Enabled = True : txtAllaData.Enabled = True
        btnRicerca.Enabled = True : btnVerbale.Enabled = True : btnVerbale2.Enabled = True : btnElencoSc.Enabled = True
        imgBtnShowCalendarDD.Enabled = True : imgBtnShowCalendarAD.Enabled = True
        chkSoloDaEv.Enabled = True
        chkRespArea.Enabled = True
        txtRicercaClienteSede.Enabled = True : txtRicercaNContr.Enabled = True 'giu240123
        ddlClienti.Enabled = True
        DDLSediApp.Enabled = True
        '-
        DVScadAtt = Nothing
        DVScadAtt = New DataView
        'GIU041122()
        ' ''myObject = DVScadAtt
        ' ''composeChiave = String.Format("{0}_{1}", _
        ' ''    CSTSCADATTCA, UtenteConnesso.Codice)
        ' ''SetObjectToCache(composeChiave, myObject)
        '--
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Enabled = False : btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False : btnAnnScAttCA.Visible = False
        lblTotaleAtt.Text = "0"
        ' ''btnSblocca.Visible = False
        BtnSetEnabledTo(False)
        '---
        GridViewPrevT.DataBind()
    End Sub
    Private Sub BtnSetByStato(ByVal myStato As String)
        'giu270612
        ' ''btnSblocca.Visible = False
        ' ''Dim SWBloccoModifica As Boolean = False
        ' ''Dim SWBloccoElimina As Boolean = False
        ' ''Dim SWBloccoCambiaStato As Boolean = False
        '--
        ' ''BtnSetEnabledTo(True)
        ' ''btnAllestimento.Text = "Allestimento"
        ' ''If myStato.Trim = "Evaso" Or _
        ' ''                myStato.Trim = "Chiuso non evaso" Or _
        ' ''                myStato.Trim = "Non evadibile" Or _
        ' ''                myStato.Trim = "Parz. evaso" Then
        ' ''    btnCambiaStato.Enabled = False : SWBloccoCambiaStato = True
        ' ''    btnModifica.Enabled = False : SWBloccoModifica = True
        ' ''    btnElimina.Enabled = False : SWBloccoElimina = True
        ' ''    btnAllestimento.Text = "Allestimento"
        ' ''    btnAllestimento.Enabled = False
        ' ''    btnCreaDDT.Enabled = False
        ' ''    btnFatturaCA.Enabled = False
        ' ''    btnFatturaCAAC.Enabled = False
        ' ''End If
        ' ''If myStato.Trim = "In Allestimento" Then
        ' ''    btnAllestimento.Text = "NO Allestimento"
        ' ''    btnAllestimento.Enabled = True
        ' ''End If
        ' ''If myStato.Trim = "Parz. evaso" Then
        ' ''    btnElimina.Enabled = False : SWBloccoElimina = True
        ' ''End If
        ' ''If SWBloccoElimina Or SWBloccoModifica Or SWBloccoCambiaStato Then
        ' ''    btnSblocca.Visible = True
        ' ''End If
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        ' ''btnCambiaStato.Enabled = Valore
        ' ''btnVisualizza.Enabled = Valore
        ' ''btnNuovo.Enabled = Valore
        ' ''btnModifica.Enabled = Valore
        ' ''btnElimina.Enabled = Valore
        ' ''btnAllestimento.Enabled = Valore
        ' ''btnCreaDDT.Enabled = Valore
        ' ''btnFatturaCA.Enabled = Valore
        ' ''btnFatturaCAAC.Enabled = Valore
        ' ''btnStampa.Enabled = Valore
        If Valore = False Then
            If btnVerbale.BackColor = Drawing.Color.Green Then
                btnVerbale.Enabled = True
                btnVerbale2.Enabled = True
            Else
                btnVerbale.Enabled = Valore
                btnVerbale2.Enabled = Valore
            End If
        Else
            btnVerbale.Enabled = Valore
            btnVerbale2.Enabled = Valore
        End If

        btnElencoSc.Enabled = Valore
    End Sub
    Private Sub btnVerbale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerbale.Click, btnVerbale2.Click
        SetLnk()
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject <> SWOPNESSUNA Then
            lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica!!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTASTOST) = btnVerbale.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Verbale
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        'giu180123
        Dim myID As String = ""
        Dim mySerieLotto As String = ""
        If btnVerbale.BackColor = Drawing.Color.Green Then
            btnVerbale.BackColor = btnElencoSc.BackColor 'giu100222 SEGNALA_OKBTN
            btnVerbale2.BackColor = btnElencoSc.BackColor
            '-verifico se è ok la sessione precedente altrimenti assegno la corrente
            composeChiave = String.Format("{0}_{1}",
            IDDOCUMSAVE, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            myID = myObject
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            '- Serie Lotto
            composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTOSAVE, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            mySerieLotto = myObject
            If IsNothing(mySerieLotto) Then
                mySerieLotto = ""
            End If
            If String.IsNullOrEmpty(mySerieLotto) Then
                mySerieLotto = ""
            End If
            If myID.Trim = "" Or mySerieLotto.Trim = "" Then
                'assegno quelli correti
                composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                myID = myObject
                '- Serie Lotto
                composeChiave = String.Format("{0}_{1}",
                    CSTSERIELOTTO, UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                mySerieLotto = myObject
            End If
            '------------------------------------------------------------------------
        Else
            'Dim myID As String = Session(IDDOCUMENTI)
            composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            myID = myObject
            '- Serie Lotto
            composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            mySerieLotto = myObject
        End If
        '--
        If IsNothing(myID) Then
            lblMessAttivita.Text = "Attenzione, Nessun documento selezionato"
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            lblMessAttivita.Text = "Attenzione, Nessun documento selezionato"
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If IsNothing(mySerieLotto) Then
            mySerieLotto = ""
        End If
        If String.IsNullOrEmpty(mySerieLotto) Then
            mySerieLotto = ""
        End If
        '------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        '-
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(myID, SWTD(TD.ContrattoAssistenza), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                If IsNothing(mySerieLotto) Then
                    mySerieLotto = ""
                End If
                If String.IsNullOrEmpty(mySerieLotto) Then
                    mySerieLotto = ""
                End If
                If chkSingoloNS.Checked = True And mySerieLotto.Trim <> "" Then
                    For Each rsDettagli In DsPrinWebDoc.Tables("ContrattiD").Select("SerieLotto<>'" + mySerieLotto.Trim + "'")
                        rsDettagli.Delete()
                    Next
                    DsPrinWebDoc.ContrattiD.AcceptChanges()
                Else 'GIU290123 SOLO I CONCLUSI O MODIFICATI
                    'giu070423 non funge Session("SAVEDsPrinWebDoc") = DsPrinWebDoc
                    Dim DsSavePrinWebDoc As DSPrintWeb_Documenti
                    DsSavePrinWebDoc = DsPrinWebDoc.Copy()
                    mySerieLotto = ""
                    Dim OKCancella As Boolean = False
                    Dim SWEvasaInPeriodo As Boolean = False
                    If chkAllScCA.Checked = False Then
                        For Each rsDettagli In DsPrinWebDoc.Tables("ContrattiD").Select("", "SerieLotto")
                            If mySerieLotto <> rsDettagli!SerieLotto.ToString.Trim Then
                                mySerieLotto = rsDettagli!SerieLotto.ToString.Trim
                                If DsPrinWebDoc.Tables("ContrattiD").Select("SerieLotto='" + mySerieLotto.Trim + "' AND Qta_Evasa<>0").Length = 0 Then
                                    OKCancella = True
                                Else
                                    OKCancella = False
                                    SWEvasaInPeriodo = False
                                    For Each rsDettagliCK In DsPrinWebDoc.Tables("ContrattiD").Select("SerieLotto='" + mySerieLotto.Trim + "'")
                                        If CDate(rsDettagliCK.Item("DataSc")) >= CDate(txtDallaData.Text) And
                                               CDate(rsDettagliCK.Item("DataSc")) <= CDate(txtAllaData.Text) And
                                               CBool(rsDettagliCK.Item("Qta_Evasa")) = True Then
                                            SWEvasaInPeriodo = True
                                        End If
                                    Next
                                    If SWEvasaInPeriodo = False Then
                                        OKCancella = True
                                    End If
                                End If
                            End If
                            If OKCancella = True Then
                                rsDettagli.Delete()
                            End If
                        Next
                    Else
                        For Each rsDettagli In DsPrinWebDoc.Tables("ContrattiD").Select("", "SerieLotto")
                            If mySerieLotto <> rsDettagli!SerieLotto.ToString.Trim Then
                                mySerieLotto = rsDettagli!SerieLotto.ToString.Trim
                                If DsPrinWebDoc.Tables("ContrattiD").Select("SerieLotto='" + mySerieLotto.Trim + "' AND Qta_Evasa<>0").Length = 0 Then
                                    OKCancella = True
                                Else
                                    OKCancella = False
                                End If
                            End If
                            If OKCancella = True Then rsDettagli.Delete()
                        Next
                    End If
                    ''giu070423 non funge  DsPrinWebDoc.ContrattiD.AcceptChanges()
                    If DsPrinWebDoc.Tables("ContrattiD").Select("", "SerieLotto").Length = 0 Then
                        DsPrinWebDoc = DsSavePrinWebDoc.Copy() 'giu070423 non funge la sessione Session("SAVEDsPrinWebDoc") 'giu120223 per evitare la stampa vuota SEMBRA NON FUNZIONARE
                    End If
                    DsPrinWebDoc.ContrattiD.AcceptChanges()
                End If
                '-------------------------------------------------------------------------------------
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 1
                Session(CSTNOBACK) = 0 'giu040512
            Else
                lblMessAttivita.Text = "Errore: " + StrErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            lblMessAttivita.Text = "Errore: " + ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc, mySerieLotto)
    End Sub
    Private Sub btnElencoSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElencoSc.Click
        SetLnk()
        composeChiave = String.Format("{0}_{1}",
            SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject <> SWOPNESSUNA Then
            lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica!!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CaricaDatiScAtt(True) = False Then
            Exit Sub
        End If
        Dim SWSingolo As Boolean = False

        Session(CSTTASTOST) = btnElencoSc.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivita
        Session("TipoDocInStampa") = SWTD(TD.ContrattoAssistenza)
        'If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        '-
        If myObject <> SWOPNESSUNA Then Exit Sub
        If GridViewPrevT.Rows.Count = 0 Then
            lblMessAttivita.Text = "Attenzione, Nessun dato da stampare."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        composeChiave = String.Format("{0}_{1}",
                "dsDocElencoScad", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If myObject Is Nothing Then 'If Session("dsDocElencoScad") Is Nothing Then
            lblMessAttivita.Text = "Attenzione, Sessione dati scaduta, ricaricare i dati e riprovare."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione dati scaduta, ricaricare i dati e riprovare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSDocumenti
        DsPrinWebDoc = myObject
        '---------------------------------------------------------------------------------------------------------------------
        Dim StrErrore As String = ""
        'GIU160520 PER SAPERE I CODICI VISITA
        Dim myCodVisita As String = GetMyCodVisita() 'giu250123
        'giu130520 accorpa scadenze nell'anno SOLO quando ci sono 2 visite nell'anno
        Dim findRow As DSDocumenti.ScadAttRow
        Dim RowScadAtt As DSDocumenti.ScadAttRow
        For Each RowScadAtt In DsPrinWebDoc.ScadAtt.Select("", "DataSc,Cod_Cliente,SerieLotto")
            If InStr(myCodVisita, RowScadAtt.Cod_Articolo.Trim) > 0 Then
                RowScadAtt.BeginEdit()
                RowScadAtt.SWNoVisita = 0
                RowScadAtt.SWSingolo = SWSingolo
                RowScadAtt.EndEdit()
            Else
                RowScadAtt.BeginEdit()
                RowScadAtt.SWNoVisita = 1
                RowScadAtt.SWSingolo = SWSingolo
                RowScadAtt.EndEdit()
                Continue For
            End If
            If RowScadAtt.NoStampa = True Then
                Continue For
            ElseIf Not IsDBNull(RowScadAtt.CodArt2V) Then
                If RowScadAtt.CodArt2V.Trim <> "" Then
                    Continue For
                End If
            ElseIf Not IsDBNull(RowScadAtt.DesArt2V) Then
                If RowScadAtt.DesArt2V.Trim <> "" Then
                    Continue For
                End If
            End If
            '-
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + RowScadAtt.Cod_Cliente + "' AND SerieLotto='" + RowScadAtt.SerieLotto + "'", "DataSc,Cod_Cliente,SerieLotto")
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then

                Else
                    Continue For
                End If
                If findRow.DurataNumRiga = RowScadAtt.DurataNumRiga And findRow.Riga = RowScadAtt.Riga Then
                    Continue For
                End If
                If findRow.DurataNumRiga = RowScadAtt.DurataNumRiga Then
                Else
                    Continue For
                End If
                If findRow.NoStampa = True Then
                    Continue For
                End If
                If Not IsDBNull(findRow.CodArt2V) Then
                    If findRow.CodArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                If Not IsDBNull(findRow.DesArt2V) Then
                    If findRow.DesArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                '-
                If RowScadAtt.NoStampa = True Then
                    Continue For
                End If
                If Not IsDBNull(RowScadAtt.CodArt2V) Then
                    If RowScadAtt.CodArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                If Not IsDBNull(RowScadAtt.DesArt2V) Then
                    If RowScadAtt.DesArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                '-
                If RowScadAtt.DataSc.Year <> findRow.DataSc.Year Then
                    Continue For
                End If
                '-
                RowScadAtt.BeginEdit()
                RowScadAtt.CodArt2V = findRow.Cod_Articolo
                RowScadAtt.DesArt2V = findRow.Descrizione
                RowScadAtt.DataSc2V = findRow.DataSc
                RowScadAtt.EndEdit()
                '-
                findRow.BeginEdit()
                findRow.NoStampa = True
                findRow.EndEdit()
                Exit For
            Next
        Next
        'giu240520 accorpo per SerieLotto la data di scadenza al CKECK
        Dim RowScadAttS As DSDocumenti.ScadAttRow
        Dim mySerieLotto As String = "Z" : Dim myCodCliente As String = "Z" : Dim myDataScCK As String = "" : Dim myDurataNumRiga As Integer = -1
        Dim SWAgg As Boolean = True
        For Each RowScadAttS In DsPrinWebDoc.ScadAtt.Select("", "DurataNumRiga,SerieLotto,SWNoVisita,Cod_Cliente")
            If mySerieLotto <> RowScadAttS.SerieLotto Or myCodCliente <> RowScadAttS.Cod_Cliente Or myDurataNumRiga <> RowScadAttS.DurataNumRiga Then
                mySerieLotto = RowScadAttS.SerieLotto
                myCodCliente = RowScadAttS.Cod_Cliente
                myDurataNumRiga = RowScadAttS.DurataNumRiga
                SWAgg = True
                myDataScCK = ""
            End If
            '-
            If InStr(myCodVisita, RowScadAttS.Cod_Articolo.Trim) > 0 Then
                myDataScCK = Format(RowScadAttS.DataSc, FormatoData)
                SWAgg = False
            Else
                Continue For
            End If
            If SWAgg = True Then
                Continue For
            End If
            '-
            SWAgg = True
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + RowScadAttS.Cod_Cliente + "' AND SerieLotto='" + RowScadAttS.SerieLotto + "' AND DurataNumRiga=" + RowScadAttS.DurataNumRiga.ToString.Trim)
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then
                    Continue For
                End If
                '-
                findRow.BeginEdit()
                findRow.DataSc = CDate(myDataScCK)
                findRow.EndEdit()
            Next
            myDataScCK = ""
        Next
        If SWAgg = False And myDataScCK.Trim <> "" Then
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + myCodCliente + "' AND SerieLotto='" + mySerieLotto + "' AND DurataNumRiga=" + myDurataNumRiga.ToString.Trim)
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then
                    Continue For
                End If
                '-
                findRow.BeginEdit()
                findRow.DataSc = myDataScCK
                findRow.EndEdit()
            Next
        End If
        '-----------
        DsPrinWebDoc.AcceptChanges()
        '-------------------------------------------------------
        Session(CSTDsPrinWebDoc) = DsPrinWebDoc
        Session(CSTNOBACK) = 0

        Call OKApriStampaElScadCA(DsPrinWebDoc)

        'qui ricarico solo le attivita CHECK/VISITA
        If CaricaDatiScAtt(False) = False Then
            Exit Sub
        End If
        lnkElencoSc.Visible = True
    End Sub
    Private Sub OKApriStampa(ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByVal mySerieLotto As String)
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        ' ''If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
        ' ''    If Session(CSTNOBACK) = 1 Then
        ' ''        btnRitorno.Visible = False
        ' ''        Label1.Visible = False
        ' ''    End If
        ' ''End If
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
        ' ''Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        ' ''DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ContrattiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
            lblMessAttivita.Text = "Errore, Verifica dati Testata.: " + ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
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

        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
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
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
            NomeStampa = "PREVOFF.PDF"
            SubDirDOC = "Preventivi"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
            NomeStampa = "ORDINE.PDF"
            SubDirDOC = "Ordini"
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
                '''    NomeStampa = "CONFORDINE.PDF"
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
                '''    NomeStampa = "CONFORDINE.PDF"
                '''    Rpt = New ConfermaOrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdineNoSconti0501
                '''    End If
                '''End If

            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
            SubDirDOC = "DDTClienti"
            NomeStampa = "DDTCLIENTE.PDF"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
            SubDirDOC = "DDTFornit"
            NomeStampa = "DDTFORNIT.PDF"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
            NomeStampa = "FATTURA.PDF"
            SubDirDOC = "Fatture"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            NomeStampa = "NOTACREDITO.PDF"
            SubDirDOC = "NoteCredito"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
            NomeStampa = "ORDINEFOR.PDF"
            SubDirDOC = "Ordini"
            Rpt = New OrdineFornitore
            If CodiceDitta = "01" Then
                Rpt = New OrdineFornitore01
            ElseIf CodiceDitta = "05" Then
                Rpt = New OrdineFornitore05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New OrdineFornitore0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
            NomeStampa = "MOVMAG.PDF"
            SubDirDOC = "MovMag"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            NomeStampa = "VERBALE.PDF"
            Rpt = New VerbaleVACA05
            If CodiceDitta = "01" Then
                Rpt = New VerbaleVACA05 '01
            ElseIf CodiceDitta = "05" Then
                Rpt = New VerbaleVACA05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New VerbaleVACA05 '0501
            End If
            ' ''If Session(CSTTASTOST) = btnStampa.ID Then
            ' ''    NomeStampa = "PROFORMACA.PDF"
            ' ''    Rpt = New ProformaCA05 'Contratti
            ' ''    If CodiceDitta = "01" Then
            ' ''        Rpt = New ProformaCA05 '01
            ' ''    ElseIf CodiceDitta = "05" Then
            ' ''        Rpt = New ProformaCA05
            ' ''    ElseIf CodiceDitta = "0501" Then
            ' ''        Rpt = New ProformaCA05 '0501
            ' ''    End If
            ' ''ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            ' ''    NomeStampa = "VERBALE.PDF"
            ' ''    Rpt = New VerbaleVACA05
            ' ''    If CodiceDitta = "01" Then
            ' ''        Rpt = New VerbaleVACA05 '01
            ' ''    ElseIf CodiceDitta = "05" Then
            ' ''        Rpt = New VerbaleVACA05
            ' ''    ElseIf CodiceDitta = "0501" Then
            ' ''        Rpt = New VerbaleVACA05 '0501
            ' ''    End If
            ' ''End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or
            Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
            Dim strRitorno As String = "WF_MenuCA.aspx?labelForm=Menu Gestione CONTRATTI"
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
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'GIU210120 ESEGUITO IN CKSTTipoDocST
        'Per evitare che solo un utente possa elaborare le stampe
        ' ''Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        ' ''If (Utente Is Nothing) Then
        ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
        ' ''    Exit Sub
        ' ''End If
        'giu180123 passato dal chiamante
        '' ''Dim mySerieLotto As String = Session(CSTSERIELOTTO)
        ' ''composeChiave = String.Format("{0}_{1}", _
        ' ''        CSTSERIELOTTO, UtenteConnesso.Codice)
        ' ''GetObjectToCache(composeChiave, myObject)
        ' ''Dim mySerieLotto As String = myObject
        ' ''Session(CSTSERIELOTTO) = mySerieLotto
        '-
        If IsNothing(mySerieLotto) Then
            mySerieLotto = ""
        End If
        If String.IsNullOrEmpty(mySerieLotto) Then
            mySerieLotto = ""
        End If
        If mySerieLotto.Trim <> "" Then
            mySerieLotto += "_" + Format(Now, "yyyyMMdd")
        Else
            mySerieLotto += Format(Now, "yyyyMMdd")
        End If
        If chkSingoloNS.Checked = False Then
            mySerieLotto += "_ALL"
        End If
        'giu090123 tolgo i taratteri speciali dal N° di serie per evitare errore nel salvataggio del PDF
        mySerieLotto = FormattaNomeFile(mySerieLotto)
        '-----------------------------------------------------------------------------------------------
        Session(CSTNOMEPDF) = mySerieLotto.Trim & "_" & NomeStampa.Trim
        '---------
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
        LnkVerbale.HRef = LnkName
        LnkApriVerbale.HRef = LnkName
        LnkApriVerbale2.HRef = LnkName
        LnkVerbale.Visible = True
        LnkApriVerbale.Visible = True
        LnkApriVerbale2.Visible = True
    End Sub

    Private Sub OKApriStampaElScadCA(ByRef DsPrinWebDoc As DSDocumenti)

        Dim SWTabCliFor As String = ""
        Dim Rpt As Object = Nothing
        Try
            If (DsPrinWebDoc.Tables("ScadAtt").Rows.Count > 0) Then
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ScadAtt").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
            End If
        Catch ex As Exception
            lblMessAttivita.Text = "Errore, Verifica dati Testata."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
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
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            If chkElencoXLS.Checked = False Then
                NomeStampa = "ELENCOSCAD.PDF"
            Else
                NomeStampa = "ELENCOSCAD.XLS"
            End If
            '-
            If rbtnOrdScadenza.Checked Then
                Rpt = New ElencoScadCA05 'Scadenze Contratti ordinato per Data Scadenza
                If CodiceDitta = "01" Then
                    Rpt = New ElencoScadCA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ElencoScadCA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ElencoScadCA05 '0501
                End If
            Else
                Rpt = New ElencoScadCAOrdCli05 'Scadenze Contratti ordinato per Cliente
                If CodiceDitta = "01" Then
                    Rpt = New ElencoScadCAOrdCli05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ElencoScadCAOrdCli05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ElencoScadCAOrdCli05 '0501
                End If
            End If
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
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'riporto il nome del RESP.AREA nel nome del file creato
        Dim strRespArea As String = Session(CSTUTENTE) 'DDLRespArea.SelectedItem.Text.Trim
        strRespArea = strRespArea.ToString.Replace(",", " ")
        strRespArea = strRespArea.ToString.Replace(".", " ")
        Dim strDalAl As String = txtDallaData.Text + "_" + txtAllaData.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        If strRespArea.Trim <> "" Then
            Session(CSTNOMEPDF) = strRespArea.Trim & "_" & strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        Else
            Session(CSTNOMEPDF) = strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        End If
        '---------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            If chkElencoXLS.Checked = False Then
                Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            Else
                Rpt.ExportToDisk(ExportFormatType.ExcelRecord, Trim(stPathReport & Session(CSTNOMEPDF)))
            End If
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            lblMessAttivita.Text = "Errore in esporta: " & Session(CSTNOMEPDF) & " " & ex.Message.Trim
            Rpt = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore in esporta: " & Session(CSTNOMEPDF) & " " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        lnkElencoSc.Visible = True
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnElencoSc.ID Then
            lnkElencoSc.HRef = LnkName
        Else
            lnkElencoSc.HRef = LnkName
        End If
    End Sub
    Public Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocST = True
        TipoDoc = Session(CSTTIPODOC)

        composeChiave = String.Format("{0}_{1}",
                CSTTIPODOC, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        If IsNothing(TipoDoc) Then
            TipoDoc = myObject
            Session(CSTTIPODOC) = TipoDoc
        End If
        '-
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
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            composeChiave = String.Format("{0}_{1}",
            "UtenteConnesso", Mid(Request.UserHostAddress.Trim, 1, 50))
            GetObjectToCache(composeChiave, myObject)
            Utente = myObject
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta Oltre 1 ora: utente non valido.(CKCSTTipoDocST)")
                Exit Function
            End If
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim 'GIU210120
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        CodiceDitta = Session(CSTCODDITTA)
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
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
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

#Region "EVASIONE ATTIVITA"
    Private Function SetCdmDAdp() As Boolean

        SqlConnDocDett = New SqlConnection
        SqlAdapDocDett = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDett.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        SqlDbSelectCmd.CommandText = "get_ConDByIDDocumenti" 'ok select *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDocDett
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        SqlAdapDocDett.SelectCommand = SqlDbSelectCmd

    End Function
    Private Function UpgStatoDocNoteEmail(ByVal NewStatoDoc As Integer, ByVal pNoteIntervento As String, ByVal pAllaPresenza As String, ByRef strErrore As String) As Boolean
        UpgStatoDocNoteEmail = False
        strErrore = ""
        'Dim myID As String = Session(IDDOCUMENTI)
        composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim myID As String = myObject
        Session(IDDOCUMENTI) = myID
        '-
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando (UpgStatoDocNoteEmail)"
            lblMessAttivita.Text = strErrore
            Exit Function
        End If
        '-
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        'giu030322
        'Dim NSL As String = Session(CSTSERIELOTTO)
        composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim NSL As String = myObject
        Session(CSTSERIELOTTO) = NSL
        '-
        If IsNothing(NSL) Then
            NSL = ""
        End If
        If String.IsNullOrEmpty(NSL) Then
            NSL = ""
        End If
        If NSL = "" Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (NSL) - Riprovate la modifica uscendo e rientrando (UpgStatoDocNoteEmail)"
            lblMessAttivita.Text = strErrore
            Exit Function
        End If
        '-
        'GIU160123 CARICO LE SCADENZE PER SAPERE QUALI SERIALI SONO STATI MODIFICATI COSI RIPORTO LE NOTE (VIS.TUTTI I SERIALI)
        composeChiave = String.Format("{0}_{1}",
               CSTSCADATTCA, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        '-
        If Not (myObject Is Nothing) Then 'Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = myObject 'Session(CSTSCADATTCA)
        Else
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (DVScadAtt Nothing UpgStatoDocNoteEmail)"
            lblMessAttivita.Text = strErrore
            Exit Function
        End If
        Dim saveFilter As String = DVScadAtt.RowFilter
        'giu270423 leggo i dati dove è memorizz email
        If (aDataViewScAtt Is Nothing) Then
            'aDataViewScAtt = Session("aDataViewScAtt")
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            aDataViewScAtt = myObject
        End If
        If (aDataViewScAtt Is Nothing) Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (aDataViewScAtt  Nothing UpgStatoDocNoteEmail)"
            lblMessAttivita.Text = strErrore
            Exit Function
        End If
        Dim saveFilter2 As String = aDataViewScAtt.RowFilter
        '----------------------------------------------------------------------------------------------------------------------
        'GIU020322 NOTE PUNTUALI PER N° SERIE LOTTO
        Dim ListaSL As New List(Of String)
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim OKNoteRitiro As String = ""
        txtNoteDocumento.Text.Trim.Replace("§", " ")
        Dim StrDato() As String
        Dim swOKAggiornato As Boolean = False
        Dim SWTrovatoNSL As Boolean = False 'GIU070423 SE NON TROVO NSL LO INSERISCO
        Try
            composeChiave = String.Format("{0}_{1}",
                L_NSERIELOTTO, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            If Not (myObject Is Nothing) Then 'Session(L_NSERIELOTTO) Is Nothing) Then
                'ListaSL = Session(L_NSERIELOTTO)
                ListaSL = myObject
            Else
                Call SetNoteSLALLApp(txtNoteDocumento.Text.Trim)
                'ListaSL = Session(L_NSERIELOTTO)
                composeChiave = String.Format("{0}_{1}",
                L_NSERIELOTTO, UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                ListaSL = myObject
                '-
            End If
            OKNoteRitiro = "" 'giu270423
            '-------------------------------------------
            swOKAggiornato = False
            Dim strComodo As String = ""
            If ListaSL.Count > 0 Then
                OKNoteRitiro = ""
                SWTrovatoNSL = False 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                For I = 0 To ListaSL.Count - 1
                    strComodo = ""
                    StrDato = ListaSL(I).Split("§")
                    mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                    myNoteRitiro = StrDato(1)
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    aDataViewScAtt.RowFilter = "SerieLotto='" + mySL.Trim + "'" 'giu270423
                    'GIU160123 QUI DEVO VERIFICARE SE E' STATO FATTA UNA MODIFICA
                    If mySL = NSL And chkSingoloNS.Checked = True Then
                        SWTrovatoNSL = True
                        swOKAggiornato = True
                        If pNoteIntervento.Trim <> "" Then 'giu301023
                            If aDataViewScAtt.Count > 0 Then
                                If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                    strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                            Else
                                strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                            End If
                            If txtNoteDocumento.Text.Trim.ToUpper.Contains(strComodo) Then
                                'ok
                            Else
                                txtNoteDocumento.Text += vbCr + strComodo + vbCr
                            End If
                        End If
                        OKNoteRitiro += mySL + "§" + Trim(txtNoteDocumento.Text.Trim + " " + pNoteIntervento.Trim)
                        If aDataViewScAtt.Count > 0 Then 'GIU280423 
                            If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                                If strComodo.Trim <> "" Then
                                    OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                Else
                                    If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                        strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                    Else
                                        strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                    End If
                                    If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                        'ok
                                    Else
                                        OKNoteRitiro += vbCr + strComodo + vbCr
                                    End If
                                    OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                    OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                End If
                            End If
                        End If
                        If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                    Else 'GIU160123 QUI DEVO VERIFICARE SE E' STATO FATTA UNA MODIFICA
                        If mySL = NSL Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                            SWTrovatoNSL = True
                        End If
                        DVScadAtt.RowFilter = "SerieLotto='" + mySL.Trim + "' AND TipoScontoMerce = 'S'"
                        If DVScadAtt.Count = 0 Then
                            OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                        Else
                            swOKAggiornato = True
                            'giu301023
                            If pNoteIntervento.Trim <> "" Then 'giu301023
                                If aDataViewScAtt.Count > 0 Then
                                    If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                        strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                    Else
                                        strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                    End If
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                                If myNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                    'ok
                                Else
                                    myNoteRitiro += vbCr + strComodo + vbCr
                                End If
                            End If
                            '---------
                            OKNoteRitiro += mySL + "§" + Trim(myNoteRitiro.Trim + " " + pNoteIntervento.Trim)
                            If aDataViewScAtt.Count > 0 Then 'GIU280423 
                                If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                                    'giu301023
                                    If strComodo.Trim <> "" Then
                                        OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                    Else
                                        If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                            strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                        Else
                                            strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                        End If
                                        If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                            'ok
                                        Else
                                            OKNoteRitiro += vbCr + strComodo + vbCr
                                        End If
                                        OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                        OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                    End If
                                    '---------
                                End If
                            End If
                            If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                        End If
                    End If
                Next
                If SWTrovatoNSL = False Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                    swOKAggiornato = True 'giu131223
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    'giu301023
                    strComodo = ""
                    If pNoteIntervento.Trim <> "" Then
                        If aDataViewScAtt.Count > 0 Then
                            If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                            Else
                                strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                            End If
                        Else
                            strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                        End If
                        If txtNoteDocumento.Text.Trim.ToUpper.Contains(strComodo) Then
                            'ok
                        Else
                            txtNoteDocumento.Text += vbCr + strComodo + vbCr
                        End If
                    End If
                    '------------
                    OKNoteRitiro += NSL + "§" + Trim(txtNoteDocumento.Text.Trim + " " + pNoteIntervento.Trim)
                    aDataViewScAtt.RowFilter = "SerieLotto='" + NSL.Trim + "'" 'giu270423
                    If aDataViewScAtt.Count > 0 Then 'GIU280423 
                        If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                            'giu301023
                            If strComodo.Trim <> "" Then
                                OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                            Else
                                If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                    strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                                If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                    'ok
                                Else
                                    OKNoteRitiro += vbCr + strComodo + vbCr
                                End If
                                OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                            End If
                            '---------
                        End If
                    End If
                    If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                End If
            Else 'qui xke sono i contratti precedenti dove le note erano legati a tutti i numeri di serie
                Call SetNoteSLALLApp(txtNoteDocumento.Text.Trim) 'ok assegna le note a tutti i i numeri di serie
                'ListaSL = Session(L_NSERIELOTTO)
                composeChiave = String.Format("{0}_{1}",
                L_NSERIELOTTO, UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                ListaSL = myObject
                '-
                OKNoteRitiro = "" 'giu270423
                strComodo = ""
                If ListaSL.Count > 0 Then
                    OKNoteRitiro = ""
                    SWTrovatoNSL = False 'giu270429
                    For I = 0 To ListaSL.Count - 1
                        StrDato = ListaSL(I).Split("§")
                        mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                        myNoteRitiro = StrDato(1)
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        aDataViewScAtt.RowFilter = "SerieLotto='" + mySL.Trim + "'" 'giu270423
                        'GIU160123 QUI DEVO VERIFICARE SE E' STATO FATTA UNA MODIFICA
                        strComodo = ""
                        If mySL = NSL And chkSingoloNS.Checked = True Then
                            SWTrovatoNSL = True
                            swOKAggiornato = True
                            If pNoteIntervento.Trim <> "" Then
                                If aDataViewScAtt.Count > 0 Then
                                    If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                        strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                    Else
                                        strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                    End If
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                                If txtNoteDocumento.Text.Trim.ToUpper.Contains(strComodo) Then
                                    'ok
                                Else
                                    txtNoteDocumento.Text += vbCr + strComodo + vbCr
                                End If
                            End If
                            '------------
                            OKNoteRitiro += mySL + "§" + Trim(txtNoteDocumento.Text.Trim + " " + pNoteIntervento.Trim)
                            If aDataViewScAtt.Count > 0 Then 'GIU280423 
                                If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                                    'giu301023
                                    If strComodo.Trim <> "" Then
                                        OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                    Else
                                        If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                            strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                        Else
                                            strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                        End If
                                        If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                            'ok
                                        Else
                                            OKNoteRitiro += vbCr + strComodo + vbCr
                                        End If
                                        OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                        OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                    End If
                                    '---------
                                End If
                            End If
                            If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                        Else 'GIU160123 QUI DEVO VERIFICARE SE E' STATO FATTA UNA MODIFICA
                            strComodo = ""
                            If mySL = NSL Then 'giu270423 GIU070423 SE NON TROVO NSL LO INSERISCO 
                                SWTrovatoNSL = True
                            End If
                            DVScadAtt.RowFilter = "SerieLotto='" + mySL.Trim + "' AND TipoScontoMerce = 'S'"
                            If DVScadAtt.Count = 0 Then
                                OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                            Else
                                swOKAggiornato = True
                                'giu301023
                                If pNoteIntervento.Trim <> "" Then 'giu301023
                                    If aDataViewScAtt.Count > 0 Then
                                        If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                            strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                        Else
                                            strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                        End If
                                    Else
                                        strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                    End If
                                    If myNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                        'ok
                                    Else
                                        myNoteRitiro += vbCr + strComodo + vbCr
                                    End If
                                End If
                                '---------
                                OKNoteRitiro += mySL + "§" + Trim(myNoteRitiro.Trim + " " + pNoteIntervento.Trim)
                                If aDataViewScAtt.Count > 0 Then 'GIU280423 
                                    If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                                        'giu301023
                                        If strComodo.Trim <> "" Then
                                            OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                        Else
                                            If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                                strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                            Else
                                                strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                            End If
                                            If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                                'ok
                                            Else
                                                OKNoteRitiro += vbCr + strComodo + vbCr
                                            End If
                                            OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                            OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                        End If
                                        '---------
                                    End If
                                End If
                                If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                            End If
                        End If
                    Next
                    If SWTrovatoNSL = False Then 'giu270423 GIU070423 SE NON TROVO NSL LO INSERISCO 
                        swOKAggiornato = True 'giu131223
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        strComodo = ""
                        If pNoteIntervento.Trim <> "" Then
                            If aDataViewScAtt.Count > 0 Then
                                If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                    strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                            Else
                                strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                            End If
                            If txtNoteDocumento.Text.Trim.ToUpper.Contains(strComodo) Then
                                'ok
                            Else
                                txtNoteDocumento.Text += vbCr + strComodo + vbCr
                            End If
                        End If
                        '------------
                        OKNoteRitiro += NSL + "§" + Trim(txtNoteDocumento.Text.Trim + " " + pNoteIntervento.Trim)
                        aDataViewScAtt.RowFilter = "SerieLotto='" + NSL.Trim + "'" 'giu270423
                        If aDataViewScAtt.Count > 0 Then 'GIU280423 
                            If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                                'giu301023
                                If strComodo.Trim <> "" Then
                                    OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                Else
                                    If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                        strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                    Else
                                        strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                    End If
                                    If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                        'ok
                                    Else
                                        OKNoteRitiro += vbCr + strComodo + vbCr
                                    End If
                                    OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                    OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                                End If
                                '---------
                            End If
                        End If
                        If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                    End If
                Else 'non è possibile che non vi sia nessun n° di serie
                    lblMessAttivita.Text = "Errore: Nessun N° Serie assegnato (UpgStatoDocNoteEmail) N°Serie: " + NSL.Trim
                    strErrore = lblMessAttivita.Text
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    strComodo = ""
                    If pNoteIntervento.Trim <> "" Then
                        If aDataViewScAtt.Count > 0 Then
                            If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                            Else
                                strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                            End If
                        Else
                            strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                        End If
                        If txtNoteDocumento.Text.Trim.ToUpper.Contains(strComodo) Then
                            'ok
                        Else
                            txtNoteDocumento.Text += vbCr + strComodo + vbCr
                        End If
                    End If
                    '------------
                    OKNoteRitiro = NSL + "§" + Trim(txtNoteDocumento.Text.Trim + " " + pNoteIntervento.Trim)
                    aDataViewScAtt.RowFilter = "SerieLotto='" + NSL.Trim + "'" 'giu270423
                    If aDataViewScAtt.Count > 0 Then 'GIU280423 
                        If aDataViewScAtt.Item(0).Item("EmailVerbale").ToString.Trim <> txtEmailInvio.Text.Trim Then
                            'giu301023
                            If strComodo.Trim <> "" Then
                                OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                            Else
                                If IsDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim) Then
                                    strComodo = "INTERVENTO " + CDate(aDataViewScAtt.Item(0).Item("DataSc").ToString.Trim).Year.ToString.Trim
                                Else
                                    strComodo = "INTERVENTO " + Now.Year.ToString.Trim
                                End If
                                If OKNoteRitiro.Trim.ToUpper.Contains(strComodo) Then
                                    'ok
                                Else
                                    OKNoteRitiro += vbCr + strComodo + vbCr
                                End If
                                OKNoteRitiro += " - " + Format(Now, "dddd d MMMM yyyy, HH:mm")
                                OKNoteRitiro += " - " + txtEmailInvio.Text.Trim
                            End If
                            '---------
                        End If
                    End If
                    If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                End If
            End If
        Catch ex As Exception
            strErrore = "Errore: Aggiorna Note Intervento (UpgStatoDocNoteEmail): " + ex.Message.Trim
            Exit Function
        End Try
        DVScadAtt.RowFilter = saveFilter 'giu160123
        aDataViewScAtt.RowFilter = saveFilter2 'giu270423
        '---------------------------------------------
        'GIU060422 NON FUNZIONA L'AGG NOTERITITO CON QUESTO METODO: "NoteRitiro='" & Controlla_Apice(OKNoteRitiro) & "', " 
        'giu270423 non aggiorno più l'email verbale ma nel caso venisse modificata la riporto prima nelle Note Intervento
        '''strSQL = "UPDATE ContrattiT SET StatoDoc=" & NewStatoDoc.ToString.Trim & ", " & _
        '''         "Aspetto='" & Controlla_Apice(txtEmailInvio.Text.Trim) & "' Where IDDocumenti=" & myID.Trim
        strSQL = "UPDATE ContrattiT SET StatoDoc=" & NewStatoDoc.ToString.Trim & " Where IDDocumenti=" & myID.Trim
        '---------
        Dim ObjDB As New DataBaseUtility
        Try
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL)
            If SWOk = False Then
                ObjDB = Nothing
                strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgStatoDocEmail)"
                Exit Function
            End If
            'GIU060422 ''ObjDB = Nothing
        Catch Ex As Exception
            strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgStatoDocEmail) - " & Ex.Message.Trim
            Exit Function
        End Try
        'GIU060422
        Dim myErrore As String = ""
        '---------
        Try
            'giu031122
            myObject = OKNoteRitiro
            composeChiave = String.Format("{0}_{1}",
                "OKNoteRitiro", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            'giu040324 corretto errore ''''''' aggiungeva ad ogni agg un apice in piu tolto il contolla_apice
            SWOk = ObjDB.ExecUpgNoteRitiro(myID.Trim, OKNoteRitiro.Trim, myErrore)
            If SWOk = False Then
                ObjDB = Nothing
                strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento)"
                Exit Function
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento) - " & Ex.Message.Trim & " -  " & myErrore
            Exit Function
        End Try
        '---------
        UpgStatoDocNoteEmail = True
        If swOKAggiornato = False And (pNoteIntervento.Trim <> "" Or pAllaPresenza.Trim <> "") Then
            lblMessAttivita.Text = "ATTENZIONE, Nessun aggiornamento per memorizzare le Note Intervento/Alla presenza: " + pNoteIntervento.Trim + " - " + pAllaPresenza.Trim
            strErrore = lblMessAttivita.Text
        ElseIf swOKAggiornato = False Then
            lblMessAttivita.Text = "ATTENZIONE, Nessun aggiornamento effettuato"
        End If
    End Function
    Public Function CKAttEvPagEv(Optional ByRef strSegnala As String = "") As Boolean
        strSegnala = ""
        ' ''CKAttEvPagEv = False
        ' ''Dim AllPagScad As Boolean = True
        ' ''Dim SWOKPagScad As Boolean = False
        '-
        Dim AllSCAtt As Boolean = True
        Dim SWOKEvasa As Boolean = False
        '--
        Dim chkEvasa As CheckBox
        'giu280520
        ' ''Dim FinoAAAA As Integer = -1
        Dim txtDFC As TextBox
        ' ''Dim txtDScAtt As TextBox
        Try
            ' ''For Each row As GridViewRow In GridViewDettCASC.Rows
            ' ''    chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            ' ''    txtDScAtt = CType(row.FindControl("txtDataScAtt"), TextBox)
            ' ''    txtDFC = CType(row.FindControl("txtDataFC"), TextBox)
            ' ''    If chkEvasa.Checked = True Then
            ' ''        SWOKPagScad = True
            ' ''        If IsDate(txtDFC.Text) Then
            ' ''            If CDate(txtDFC.Text).Year > FinoAAAA Then
            ' ''                FinoAAAA = CDate(txtDFC.Text).Year
            ' ''            End If
            ' ''        ElseIf IsNumeric(txtDFC.Text) Then 'nel caso ci sia solo l'anno 
            ' ''            If CLng(txtDFC.Text) > FinoAAAA Then
            ' ''                FinoAAAA = CLng(txtDFC.Text)
            ' ''            End If
            ' ''        Else
            ' ''            If IsDate(txtDScAtt.Text) Then
            ' ''                If CDate(txtDScAtt.Text).Year > FinoAAAA Then
            ' ''                    FinoAAAA = CDate(txtDScAtt.Text).Year
            ' ''                End If
            ' ''            End If
            ' ''        End If
            ' ''    Else
            ' ''        AllPagScad = False
            ' ''    End If
            ' ''Next
            '-attivita
            For Each row As GridViewRow In GridViewDettCAAtt.Rows
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                ' ''txtNFC = CType(row.FindControl("txtNFC"), TextBox)
                'giu200122 txtDFC = CType(row.FindControl("txtDataSC"), TextBox)
                If chkEvasa.Checked = True Then
                    SWOKEvasa = True
                Else
                    AllSCAtt = False
                    ' ''If strSegnala.Trim = "" Then 'giu280520 segnalo che vi sono delle attività non chiuse a fronte di FATTURE EMESSE 
                    ' ''    If IsDate(txtDFC.Text) Then
                    ' ''        If CDate(txtDFC.Text).Year <= FinoAAAA Then
                    ' ''            strSegnala = "Attenzione, sono presenti attività non ancora evase a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
                    ' ''        End If
                    ' ''    ElseIf IsNumeric(txtDFC.Text) Then 'nel caso ci sia solo l'anno 
                    ' ''        If CDate(txtDFC.Text).Year <= FinoAAAA Then
                    ' ''            strSegnala = "Attenzione, sono presenti attività non ancora evase a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
                    ' ''        End If
                    ' ''    End If
                    ' ''End If

                End If
            Next
        Catch ex As Exception

        End Try

        If SWOKEvasa = True Then 'SWOKPagScad = True Or  Then
            CKAttEvPagEv = True
        Else
        End If
    End Function
    Private Function PopolaGridScAtt() As Boolean
        Try
            Dim SWSviluppo As String = ConfigurationManager.AppSettings("debug")
            If Not String.IsNullOrEmpty(SWSviluppo) Then
                If SWSviluppo.Trim.ToUpper = "TRUE" Then
                    lblMessDebug.Visible = True
                    lblMessDebug.Text = "DEBUG - RIASSEGNA TUTTO LA SESSIONE PopolaGridScAtt: " + Format(Now, "dd/MM/yyyy, HH:mm:ss")
                End If
            End If
            '-----------
            'Dim myID As String = Session(IDDOCUMENTI)
            composeChiave = String.Format("{0}_{1}",
                IDDOCUMENTI, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Dim myID As String = myObject
            Session(IDDOCUMENTI) = myID
            '-
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                If GridViewPrevT.Rows.Count = 0 Then
                    lblMessAttivita.Text = "Attenzione, Nessun documento selezionato.(PopolaGridScAtt)"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun documento selezionato.(PopolaGridScAtt)", WUC_ModalPopup.TYPE_ALERT)
                Else
                    lblMessAttivita.Text = "Attenzione, Sessione scaduta: IDDocumento non valido.(PopolaGridScAtt)"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Sessione scaduta: IDDocumento non valido.(PopolaGridScAtt)", WUC_ModalPopup.TYPE_ALERT)
                End If
                Exit Function
            End If
            'Dim myIDDNR As String = Session(IDDURATANUMRIGA)
            composeChiave = String.Format("{0}_{1}",
                    IDDURATANUMRIGA, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Dim myIDDNR As String = myObject
            Session(IDDURATANUMRIGA) = myIDDNR
            '-
            If IsNothing(myIDDNR) Then
                myIDDNR = ""
            End If
            If String.IsNullOrEmpty(myIDDNR) Then
                myIDDNR = ""
            End If
            SetCdmDAdp()
            Dim DsContrattiDettALLAtt As New DSDocumenti
            DsContrattiDettALLAtt.ContrattiD.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
            If chkAllScCA.Checked = False Then
                SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = IIf(myIDDNR.Trim = "", DBNull.Value, myIDDNR.Trim) 'giu271122
            Else
                SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            End If
            SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
            DVScadAtt = New DataView(DsContrattiDettALLAtt.ContrattiD)
            Dim RowD As DSDocumenti.ContrattiDRow
            'giu250123 PER SAPERE I CODICI VISITA     If InStr(myCodVisita, RowScadAtt.Cod_Articolo.Trim) > 0 Then
            Dim myCodVisita As String = GetMyCodVisita()
            '-----------------------------------------------------------------
            If DVScadAtt.Count > 0 Then
                Dim RowsEvasa() As DataRow = DsContrattiDettALLAtt.ContrattiD.Select("") 'Qta_Evasa<>Qta_Selezionata")
                For Each RowD In RowsEvasa
                    RowD.BeginEdit()
                    RowD.Qta_Selezionata = RowD.Qta_Evasa
                    If RowD.IsDataEvNull Then
                        RowD.TextDataEv = ""
                    Else
                        RowD.TextDataEv = Format(RowD.DataEv, FormatoData)
                    End If
                    'giu250123
                    If RowD.IsRefDataNCNull Then
                        RowD.TextRefDataNC = ""
                    Else
                        RowD.TextRefDataNC = Format(RowD.RefDataNC, FormatoData)
                    End If
                    'If RowD.Descrizione.Trim.ToUpper.Contains("CK") Then GIU250123
                    If myCodVisita.Trim.ToUpper.Contains(RowD.Cod_Articolo.Trim.ToUpper) Then '"CK") Then'GIU250123
                        RowD.SWModAgenti = False
                    Else
                        RowD.SWModAgenti = True
                    End If
                    'giu180723
                    If RowD.IsSerieNull Then RowD.Serie = ""
                    RowD.Serie = Formatta.FormattaNomeFile(RowD.Serie)
                    If RowD.IsLottoNull Then RowD.Lotto = ""
                    RowD.Lotto = Formatta.FormattaNomeFile(RowD.Lotto)
                    If IsDBNull(RowD!SerieLotto) Then RowD!SerieLotto = ""
                    RowD!SerieLotto = Formatta.FormattaNomeFile(RowD!SerieLotto)
                    '-
                    RowD.EndEdit()
                Next
                DsContrattiDettALLAtt.AcceptChanges()
                btnModScAttCA.Enabled = True
            Else
                btnModScAttCA.Enabled = False
            End If
            '-
            If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,SerieLotto"
            'Dim mySerieLotto As String = Session(CSTSERIELOTTO)
            composeChiave = String.Format("{0}_{1}",
                  CSTSERIELOTTO, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Dim mySerieLotto As String = myObject
            Session(CSTSERIELOTTO) = mySerieLotto
            '-
            If IsNothing(mySerieLotto) Then
                mySerieLotto = ""
            End If
            If String.IsNullOrEmpty(mySerieLotto) Then
                mySerieLotto = ""
            End If
            If chkSingoloNS.Checked = True And mySerieLotto.Trim <> "" Then
                DVScadAtt.RowFilter = "SerieLotto='" + mySerieLotto.Trim + "'"
            End If
            lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
            ' ''lblTotaleAtt.Text += " di cui Evase: " & DsContrattiDettALLAtt.ContrattiD.Select("Qta_Evasa<>0").Length.ToString.Trim
            'Session(CSTSCADATTCA) = DVScadAtt
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                   CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
        Catch ex As Exception
            lblMessAttivita.Text = "Errore: Caricamento dati attività: " & ex.Message.Trim
            'per adesso proseguo
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "Caricamento dati attività: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    'giu250123
    Private Function GetMyCodVisita() As String
        Dim myCodVisita As String = "" : Dim Comodo As String = ""
        composeChiave = String.Format("{0}_{1}",
                "myCodVisita", UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        myCodVisita = myObject
        If IsNothing(myCodVisita) Then
            myCodVisita = ""
        End If
        If String.IsNullOrEmpty(myCodVisita) Then
            myCodVisita = ""
        End If
        If myCodVisita.Trim <> "" Then
            GetMyCodVisita = myCodVisita
            Exit Function
        End If

        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = "Select * From TipoContratto"

        Dim DsDoc As New DSDocumenti
        Try
            DsDoc.Tables("TipoContratto").Clear()
            DsDoc.Tables("TipoContratto").AcceptChanges()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsDoc, "TipoContratto")
            For Each rsTC In DsDoc.Tables("TipoContratto").Select("")
                Comodo = IIf(IsDBNull(rsTC!Descrizione), "", rsTC!CodVisita.ToString.Trim)
                If InStr(myCodVisita, Comodo.Trim) > 0 Then
                Else
                    myCodVisita += Comodo.Trim + ","
                End If
            Next
        Catch ex As Exception
            lblMessAttivita.Text = "Errore, Caricamento Tabella TipoContratto. : " & ex.Message.Trim
            myCodVisita = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento Tabella TipoContratto. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
        End Try
        ObjDB = Nothing
        myObject = myCodVisita
        composeChiave = String.Format("{0}_{1}",
                "myCodVisita", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        GetMyCodVisita = myCodVisita
    End Function
    '-
    Private Sub btnModScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModScAttCA.Click
        composeChiave = String.Format("{0}_{1}",
               SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        '-
        If myObject <> SWOPNESSUNA Then 'Session(SWOP) <> SWOPNESSUNA Then
            lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica!!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica: " + Session(SWOP).ToString.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Call SetLnk()
        'GIU041122()
        If Not (DVScadAtt Is Nothing) Then
            myObject = DVScadAtt
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
        Else
            composeChiave = String.Format("{0}_{1}",
                CSTSCADATTCA, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            DVScadAtt = myObject
        End If
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()
        '--------------
        If GridViewDettCAAtt.Rows.Count > 0 Then
            'OK
        Else
            lblMessAttivita.Text = "Attenzione, Nessun dato presente."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        Session(SWOP) = SWOPMODSCATT
        myObject = SWOPMODSCATT
        composeChiave = String.Format("{0}_{1}",
               SWOP, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(SWOPMODSCATT) = SWSI
        myObject = SWSI
        composeChiave = String.Format("{0}_{1}",
               SWOPMODSCATT, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        GridViewDettCAAtt.Enabled = True
        GridViewPrevT.Enabled = False
        txtDallaData.Enabled = False : txtAllaData.Enabled = False
        btnRicerca.Enabled = False : btnVerbale.Enabled = False : btnVerbale2.Enabled = False : btnElencoSc.Enabled = False
        imgBtnShowCalendarDD.Enabled = False : imgBtnShowCalendarAD.Enabled = False
        chkSoloDaEv.Enabled = False
        chkRespArea.Enabled = False
        txtRicercaClienteSede.Enabled = False : txtRicercaNContr.Enabled = False 'giu240123
        ddlClienti.Enabled = False
        DDLSediApp.Enabled = False
        'giu140423
        chkElencoXLS.Enabled = False
        rbtnOrdCliente.Enabled = False
        rbtnOrdScadenza.Enabled = False
        chkScadAnno.Enabled = False
        '---------
        btnModScAttCA.Visible = False
        btnAggScAttCA.Visible = True
        btnAnnScAttCA.Visible = True
        'giu131221
        chkEvadiTutte.AutoPostBack = False
        chkEvadiTutte.Checked = False
        chkEvadiTutte.Visible = True : chkEvadiTutte.Enabled = True
        chkEvadiTutte.AutoPostBack = True
        'giu200122 'GIU280222 POSSO MODIFICARE SOLO SE VISUALIZZO UN SINGOLO SERIALE
        If chkSingoloNS.Checked = True Then
            txtEmailInvio.Enabled = True : txtNoteDocumento.Enabled = True : txtAllaPresezaDi.Enabled = True
        Else
            'giu100123 per permettere la modifica
            txtEmailInvio.Enabled = True
        End If
        '-
    End Sub

    'giu210122 CONFERMA CON AGG.CON NOTE E SENZA AGG.NOTE O RITORNO PER CONTINUARE LA MODIFICA
    Private Sub btnAggScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggScAttCA.Click
        Try
            'giu200122 richiesta NOTE INTERVENTO e AGGIORNAMENTO Scadenze attività
            SetLnk() 'giu070423 cosi evita di aprire un verbale non aggiornato
            'giu070423 in debug va in errore la richiesta note intervento
            Dim SWSviluppo As String = ConfigurationManager.AppSettings("debug")
            If Not String.IsNullOrEmpty(SWSviluppo) Then
                If SWSviluppo.Trim.ToUpper = "TRUE" Then
                    Call OKAggScadAtt("", "")
                    Exit Sub
                End If
            End If
            '------------
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Conferma Aggiornamento Dati", "Note Intervento:", WUC_ModalPopup.TYPE_GETVALUE, txtNoteDocumento.Text.Trim, "ALLA PRESENZA DI:", txtAllaPresezaDi.Text.Trim)
            '-------------------------------
        Catch ex As Exception
            lblMessAttivita.Text = "Errore: Conferma Aggiornamento Dati - Riprovate la modifica.(btnAggScAttCA): " + ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "Conferma Aggiornamento Dati - Riprovate la modifica.(btnAggScAttCA)<br>" + ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

    End Sub
    Public Function GetValue() As String
        GetValue = ""
        Dim GetValue1 As String = ModalPopup.Valore.Trim
        Dim GetValue2 As String = ModalPopup.Valore2.Trim
        'giu190123 giu260123
        GetValue1.Replace("§", " ")
        GetValue2.Replace("§", " ")
        GetValue1.Replace("|", " ") 'giu280123 serve a determinare se ci fosse ALLA PRESENZA DI.....
        GetValue2.Replace("|", " ")
        Dim pNoteIntevento As String = ""
        If GetValue1.Trim <> "" Or GetValue2.Trim <> "" Then
            pNoteIntevento = IIf(GetValue1.Trim <> "", " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + GetValue1.Trim, "")
        End If
        Call OKAggScadAtt(pNoteIntevento.Trim, GetValue2)
    End Function
    Private Sub OKAggScadAtt(ByVal pNoteIntevento As String, ByVal pAllaPresenza As String)
        'GIU291123
        lblMessDebug.Visible = False
        Dim strDateScadCons As String = ""
        '---------
        composeChiave = String.Format("{0}_{1}",
               CSTSCADATTCA, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        '-
        If Not (myObject Is Nothing) Then 'Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = myObject 'Session(CSTSCADATTCA)
        Else
            lblMessAttivita.Text = "Errore: SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (OKAggScadAtt myObject)"
            GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = True
            SvuodaGridT()
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (OKAggScadAtt myObject)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'controllo date scadenze sempre valide per tutte
        Dim SWOKEvasa As Boolean = False
        Dim rowScadAttCa As DSDocumenti.ContrattiDRow
        Dim idxRow As Integer = 0
        Dim chkEvasa As CheckBox
        Dim txtDEV As TextBox
        Dim txtDEVN As TextBox
        Dim SWErrore As Boolean = False
        Dim strErrore As String = ""
        For Each row As GridViewRow In GridViewDettCAAtt.Rows
            chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            txtDEV = CType(row.FindControl("txtDataEV"), TextBox)
            txtDEVN = CType(row.FindControl("txtDataEVN"), TextBox)
            If chkEvasa.Checked = True Then
                'GIU140123 giu160123
                If CBool(DVScadAtt.Item(idxRow).Item("Qta_Selezionata").ToString.Trim) = False Then
                    'OK POSSO METTERE LA DATA ODIERNA ALTRIMENTI E PRECEDENTE QUINDI NON LASCIO IL DATO PREC
                    If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now, FormatoData)
                Else 'PRECEDENTE QUINDI NON LASCIO IL DATO PREC
                    If txtDEV.Text.Trim = "" Then txtDEV.Text = DVScadAtt.Item(idxRow).Item("TextDataSc").ToString.Trim()
                End If
                '---------
                txtDEV.Text = ConvData(txtDEV.Text.ToString.Trim)
                If txtDEV.Text.Trim.Length <> 10 Then
                    txtDEV.BackColor = SEGNALA_KO
                    strErrore += " - Data Esecuzione errata"
                    SWErrore = True
                ElseIf Not IsDate(txtDEV.Text.Trim) Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                    strErrore += " - Data Esecuzione errata"
                Else
                    txtDEV.BackColor = SEGNALA_OK
                End If
                'giu250123
                If txtDEVN.Enabled = True And txtDEVN.Text.Trim <> "" Then
                    txtDEVN.Text = ConvData(txtDEVN.Text.ToString.Trim)
                    If txtDEVN.Text.Trim.Length <> 10 Then
                        txtDEVN.BackColor = SEGNALA_KO
                        SWErrore = True
                        strErrore += " - Data scadenza consumabile errata"
                    ElseIf Not IsDate(txtDEVN.Text.Trim) Then
                        txtDEVN.BackColor = SEGNALA_KO
                        SWErrore = True
                        strErrore += " - Data scadenza consumabile errata"
                    Else
                        txtDEVN.BackColor = SEGNALA_OK
                    End If
                End If
                'non dev'essere inferiore ne alla scadenza che esecuzione
                'giu150223 tolto controllo solo se il text è abilitato perche da Ufficio possono mettere una data sui check
                ' ''If txtDEVN.Text.Trim <> "" And txtDEVN.BackColor = SEGNALA_OK And txtDEVN.Enabled = True Then
                ' ''    If txtDEV.BackColor = SEGNALA_OK And txtDEV.Text.Trim <> "" Then
                ' ''        Try
                ' ''            If CDate(txtDEVN.Text.Trim) < CDate(txtDEV.Text.Trim) Then
                ' ''                txtDEVN.BackColor = SEGNALA_KO
                ' ''                SWErrore = True
                ' ''                strErrore += " - Data scadenza consumabile < Data"
                ' ''            End If
                ' ''        Catch ex As Exception
                ' ''        End Try
                ' ''    End If
                ' ''End If
            Else
                If txtDEV.Text.Trim <> "" Or txtDEVN.Text.Trim <> "" Then
                    lblMessDebug.Text = "Attenzione, alcune date di Scadenza/Esecuzione consumabile sono state azzerate perchè la riga non risulta Evasa"
                    lblMessDebug.Visible = True
                    lblMessAttivita.Text = "Attenzione, alcune date di Scadenza/Esecuzione consumabile sono state azzerate perchè la riga non risulta Evasa"
                End If
                'nessun controllo
                txtDEV.Text = "" 'GIU140123
                'giu150223 controllo solo se il text è abilitato perche da Ufficio possono mettere una data sui check
                If txtDEVN.Enabled = True Then txtDEVN.Text = ""
            End If
            idxRow += 1
        Next
        'giu200122
        If txtEmailInvio.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmailInvio.Text.Trim) = False Then
                txtEmailInvio.BackColor = SEGNALA_KO
                SWErrore = True
                strErrore += " - E-Mail invio Verbale non valida"
            Else
                txtEmailInvio.BackColor = SEGNALA_OK
            End If
        Else
            txtEmailInvio.BackColor = SEGNALA_OK
        End If
        '----------
        If SWErrore Then
            lblMessAttivita.Text = "Errore in aggiornamento Attività: " + strErrore.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        'OK Riporto le modifiche
        txtEmailInvio.Enabled = False : txtNoteDocumento.Enabled = False : txtAllaPresezaDi.Enabled = False
        Session(SWOP) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
               SWOP, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(SWOPMODSCATT) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
               SWOPMODSCATT, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False
        btnAnnScAttCA.Visible = False
        'giu131221
        chkEvadiTutte.AutoPostBack = False
        chkEvadiTutte.Checked = False
        chkEvadiTutte.Visible = False : chkEvadiTutte.Enabled = False
        chkEvadiTutte.AutoPostBack = False
        '-------------------------------
        strErrore = ""
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        Dim SqlUpd_ConTScadPag = New System.Data.SqlClient.SqlCommand
        SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        Dim strValore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '--- Parametri
        SqlUpd_ConTScadPag.CommandText = "[update_ConDByIDDocRiga]"
        SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
        SqlUpd_ConTScadPag.Connection = SqlConn
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataSC", System.Data.SqlDbType.DateTime, 8, "DataSC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        idxRow = 0
        Dim SWModDett As String = ""
        strErrore = ""
        For Each row As GridViewRow In GridViewDettCAAtt.Rows
            SWErrore = False
            chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            txtDEV = CType(row.FindControl("txtDataEV"), TextBox)
            txtDEVN = CType(row.FindControl("txtDataEVN"), TextBox)
            SWModDett = ""
            If chkEvasa.Checked = True Then
                'giu160123 If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now, FormatoData)
                'GIU140123 giu160123
                If CBool(DVScadAtt.Item(idxRow).Item("Qta_Selezionata").ToString.Trim) = False Then
                    'OK POSSO METTERE LA DATA ODIERNA ALTRIMENTI E PRECEDENTE QUINDI NON LASCIO IL DATO PREC
                    If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now, FormatoData)
                    SWModDett = "S"
                Else 'PRECEDENTE QUINDI NON LASCIO IL DATO PREC
                    If txtDEV.Text.Trim = "" Then txtDEV.Text = DVScadAtt.Item(idxRow).Item("TextDataSc").ToString.Trim()
                End If
                '---------
                txtDEV.Text = ConvData(txtDEV.Text.ToString.Trim)
                If txtDEV.Text.Trim.Length <> 10 Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                    strErrore += " - Data Esecuzione errata"
                ElseIf Not IsDate(txtDEV.Text.Trim) Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                    strErrore += " - Data Esecuzione errata"
                End If
                'giu250123
                If txtDEVN.Enabled = True And txtDEVN.Text.Trim <> "" Then
                    txtDEVN.Text = ConvData(txtDEVN.Text.ToString.Trim)
                    If txtDEVN.Text.Trim.Length <> 10 Then
                        txtDEVN.BackColor = SEGNALA_KO
                        SWErrore = True
                        strErrore += " - Data scadenza consumabile errata"
                    ElseIf Not IsDate(txtDEVN.Text.Trim) Then
                        txtDEVN.BackColor = SEGNALA_KO
                        SWErrore = True
                        strErrore += " - Data scadenza consumabile errata"
                    Else
                        txtDEVN.BackColor = SEGNALA_OK
                        If DVScadAtt.Item(idxRow).Item("TextRefDataNC").ToString.Trim = "" Then
                            SWModDett = "S"
                            strDateScadCons += IIf(IsDBNull(DVScadAtt.Item(idxRow).Item("TextDataSc")), "", "(Anno Rif.") +
                                                     CDate(DVScadAtt.Item(idxRow).Item("TextDataSc")).Date.ToString("yyyy") + ") " +
                                                     DVScadAtt.Item(idxRow).Item("Cod_Articolo").ToString.Replace("§", " ").Trim _
                                                     + " - " + DVScadAtt.Item(idxRow).Item("Descrizione").ToString.Replace("§", " ").Trim _
                                                     + " Scadenza: " + txtDEVN.Text.Trim + vbCr
                        ElseIf DVScadAtt.Item(idxRow).Item("TextRefDataNC").ToString.Trim <> txtDEVN.Text.Trim Then
                            SWModDett = "S"
                            strDateScadCons += IIf(IsDBNull(DVScadAtt.Item(idxRow).Item("TextDataSc")), "", "(Anno Rif.") +
                                                     CDate(DVScadAtt.Item(idxRow).Item("TextDataSc")).Date.ToString("yyyy") + ") " +
                                                     DVScadAtt.Item(idxRow).Item("Cod_Articolo").ToString.Replace("§", " ").Trim _
                                                     + " - " + DVScadAtt.Item(idxRow).Item("Descrizione").ToString.Replace("§", " ").Trim _
                                                     + " Scadenza: " + txtDEVN.Text.Trim + vbCr
                        End If
                    End If
                End If
                'non dev'essere inferiore ne alla scadenza che esecuzione
                'giu150223 tolto controllo solo se il text è abilitato perche da Ufficio possono mettere una data sui check
                ' ''If txtDEVN.Text.Trim <> "" And txtDEVN.BackColor = SEGNALA_OK And txtDEVN.Enabled = True Then
                ' ''    If txtDEV.BackColor = SEGNALA_OK And txtDEV.Text.Trim <> "" Then
                ' ''        Try
                ' ''            If CDate(txtDEVN.Text.Trim) < CDate(txtDEV.Text.Trim) Then
                ' ''                txtDEVN.BackColor = SEGNALA_KO
                ' ''                SWErrore = True
                ' ''            End If
                ' ''        Catch ex As Exception

                ' ''        End Try
                ' ''    End If
                ' ''End If
            Else
                'NESSUN CONTROLLO
                If CBool(DVScadAtt.Item(idxRow).Item("Qta_Selezionata").ToString.Trim) <> False Then
                    SWModDett = "S"
                End If
                If DVScadAtt.Item(idxRow).Item("TextDataEv").ToString.Trim <> "" Then
                    SWModDett = "S"
                End If
                If DVScadAtt.Item(idxRow).Item("TextRefDataNC").ToString.Trim <> "" Then
                    SWModDett = "S"
                End If
            End If
            'OK
            If SWErrore Then
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                Session(SWOP) = SWOPMODSCATT
                myObject = SWOPMODSCATT
                composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(SWOPMODSCATT) = SWSI
                myObject = SWSI
                composeChiave = String.Format("{0}_{1}",
                SWOPMODSCATT, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                GridViewDettCAAtt.Enabled = True
                btnModScAttCA.Visible = False
                btnAggScAttCA.Visible = True
                btnAnnScAttCA.Visible = True
                '-
                lblMessAttivita.Text = "Errore in aggiornamento Attività: " + strErrore.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            '-
            rowScadAttCa = DVScadAtt.Item(idxRow).Row
            If chkEvasa.Checked Then
                DVScadAtt.BeginInit()
                rowScadAttCa.Qta_Selezionata = rowScadAttCa.Qta_Ordinata
                rowScadAttCa.Qta_Evasa = rowScadAttCa.Qta_Ordinata
                rowScadAttCa.Qta_Residua = 0
                If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now, FormatoData)
                rowScadAttCa.TextDataEv = txtDEV.Text.Trim
                rowScadAttCa.TipoScontoMerce = SWModDett 'giu160123
                'GIU250123
                If txtDEVN.Text.Trim <> "" Then
                    rowScadAttCa.TextRefDataNC = txtDEVN.Text.Trim
                Else
                    rowScadAttCa.TextRefDataNC = ""
                End If
                '-
                DVScadAtt.EndInit()
            Else
                DVScadAtt.BeginInit()
                rowScadAttCa.Qta_Selezionata = 0
                rowScadAttCa.Qta_Evasa = 0
                rowScadAttCa.Qta_Residua = rowScadAttCa.Qta_Ordinata
                rowScadAttCa.TextDataEv = "" '= txtDEV.Text.Trim
                rowScadAttCa.TipoScontoMerce = SWModDett 'giu160123
                rowScadAttCa.TextRefDataNC = txtDEVN.Text.Trim
                DVScadAtt.EndInit()
            End If
            If rowScadAttCa.Qta_Evasa <> 0 Then SWOKEvasa = True
            Try
                SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = rowScadAttCa.IDDocumenti
                SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = rowScadAttCa.DurataNum
                SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = rowScadAttCa.DurataNumRiga
                SqlUpd_ConTScadPag.Parameters("@Riga").Value = rowScadAttCa.Riga
                SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = rowScadAttCa.Qta_Evasa
                SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = rowScadAttCa.Qta_Residua
                SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(rowScadAttCa.TextDataSc)
                If chkEvasa.Checked Then
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = CDate(rowScadAttCa.TextDataEv)
                Else
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = DBNull.Value
                End If
                'GIU250123
                If chkEvasa.Checked Then
                    If rowScadAttCa.TextRefDataNC.Trim <> "" Then
                        SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(rowScadAttCa.TextRefDataNC)
                    Else
                        SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                    End If
                Else
                    If rowScadAttCa.TextRefDataNC.Trim <> "" Then
                        SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(rowScadAttCa.TextRefDataNC)
                    Else
                        SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                    End If
                End If
                'OK
                SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
                SqlUpd_ConTScadPag.ExecuteNonQuery()
            Catch exSQL As SqlException
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = exSQL.Message.Trim
                lblMessAttivita.Text = "Errore in aggiornamento Attività: " + strErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch ex As Exception
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = ex.Message.Trim
                lblMessAttivita.Text = "Errore in aggiornamento Attività: " + strErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            idxRow += 1
        Next
        'giu051122 non azzero i dati appena aggiornati riasegno il ds alla Casche
        myObject = DVScadAtt
        composeChiave = String.Format("{0}_{1}",
        CSTSCADATTCA, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '------------------------------------------------------------------------
        'GIU150620 PER STAMPA IL VERBALE APPENA AGGIORNATO
        Session(IDDOCUMSAVE) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMSAVE, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(CSTSERIELOTTOSAVE) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        CSTSERIELOTTOSAVE, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        btnVerbale.BackColor = btnElencoSc.BackColor 'giu100222 SEGNALA_OKBTN
        btnVerbale2.BackColor = btnElencoSc.BackColor
        '----
        'Dim myID As String = Session(IDDOCUMENTI)
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMENTI, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim myID As String = myObject
        Session(IDDOCUMENTI) = myID
        '-
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            lblMessAttivita.Text = "Errore SESSIONE SCADENZE SCADUTA - IDDOCUMENTO NON VALIDO (OKAggScadAtt myID)"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - IDDOCUMENTO NON VALIDO (OKAggScadAtt myID)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'GIU150620 PER STAMPA IL VERBALE APPENA AGGIORNATO
        'Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)
        myObject = myID
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMSAVE, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        Session(IDDOCUMSAVE) = myID
        '------------------------
        SetCdmDAdp()
        Dim DsContrattiDettALLAtt As New DSDocumenti
        DsContrattiDettALLAtt.ContrattiD.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
        SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
        Dim NewStatoDoc As Integer = 0
        Dim RowD As DSDocumenti.ContrattiDRow
        If DsContrattiDettALLAtt.ContrattiD.Select("").Length > 0 Then
            Dim RowsEvasa() As DataRow = DsContrattiDettALLAtt.ContrattiD.Select("")
            Dim AllEvase As Boolean = True
            Dim SWEvase As Boolean = False
            For Each RowD In RowsEvasa
                If RowD.Qta_Ordinata = RowD.Qta_Evasa Then
                    SWEvase = True
                Else
                    AllEvase = False
                End If
            Next
            If AllEvase = True Then
                NewStatoDoc = 1
            ElseIf SWEvase = True Then
                NewStatoDoc = 2
            Else
                NewStatoDoc = 0
            End If
        Else
            NewStatoDoc = 5
        End If
        '-
        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        'giu291123
        If strDateScadCons.Trim <> "" Then
            If pNoteIntevento.Trim <> "" Then
                pNoteIntevento += vbCr + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " " + strDateScadCons
            Else
                pNoteIntevento += Format(Now, "dddd d MMMM yyyy, HH:mm") + " " + strDateScadCons
            End If
        End If
        '---------
        If Me.UpgStatoDocNoteEmail(NewStatoDoc, pNoteIntevento, pAllaPresenza, strErrore) = False Then
            lblMessAttivita.Text = "Errore in aggiornamento UpgStatoDocNoteEmail: " + strErrore
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento UpgStatoDocNoteEmail: ", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
        ElseIf pNoteIntevento.Trim <> "" And strErrore.Trim <> "" Then
            lblMessAttivita.Text = strErrore.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("AGGIORNAMENTO NOTE INTERVENTO", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
        ElseIf strErrore.Trim <> "" Then
            lblMessAttivita.Text = strErrore.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("AGGIORNAMENTO NOTE INTERVENTO", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
        End If
        '---------
        'giu021122
        If (aDataViewScAtt Is Nothing) Then
            'aDataViewScAtt = Session("aDataViewScAtt")
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            aDataViewScAtt = myObject
            '-
        End If
        Dim myRowIndex As Integer = GridViewPrevT.SelectedIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
        Dim Stato As String = ""
        Try
            composeChiave = String.Format("{0}_{1}",
                "OKNoteRitiro", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            'giu190123 aggiorno tutte le righe del contratto (myID)
            Dim SaveRowFilter As String = aDataViewScAtt.RowFilter.Trim
            aDataViewScAtt.RowFilter = "IDDocumenti=" + myID.Trim + ""
            For i = 0 To aDataViewScAtt.Count - 1
                aDataViewScAtt.BeginInit()
                'GIU280423 NEL CASO CAMBI VERRA' RIPORTATO NELLE NOTE aDataViewScAtt.Item(i).Item("EmailVerbale") = txtEmailInvio.Text.Trim
                aDataViewScAtt.Item(i).Item("NoteRitiro") = myObject.ToString.Trim
                'giu220123
                aDataViewScAtt.Item(i).Item("NoteSerieLotto") = GetNoteSerieLotto(aDataViewScAtt.Item(i).Item("NoteRitiro"), aDataViewScAtt.Item(i).Item("SerieLotto"))
                '---------
                aDataViewScAtt.EndInit()
            Next
            aDataViewScAtt.RowFilter = SaveRowFilter
            '-
            myObject = aDataViewScAtt
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(IDDURATANUMRIGA) = aDataViewScAtt.Item(myRowIndex).Item("DurataNumRiga").ToString.Trim
            myObject = Session(IDDURATANUMRIGA)
            composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = Formatta.FormattaNomeFile(aDataViewScAtt.Item(myRowIndex).Item("SerieLotto").ToString.Trim) 'giu070523
            myObject = Session(CSTSERIELOTTO)
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            btnVerbale.BackColor = Drawing.Color.Green
            btnVerbale2.BackColor = Drawing.Color.Green
            '-
            Session(CSTSERIELOTTOSAVE) = Session(CSTSERIELOTTO)
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTOSAVE, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtEmailInvio.Text = aDataViewScAtt.Item(myRowIndex).Item("EmailVerbale").ToString.Trim
            txtNoteDocumento.Text = GetNoteSL(aDataViewScAtt.Item(myRowIndex).Item("NoteRitiro").ToString.Trim)
            txtAllaPresezaDi.Text = GetAllaPresenzaDi(txtNoteDocumento.Text)
            '-
        Catch ex As Exception
            lblMessAttivita.Text = "Errore in OKAggScadAtt: " + ex.Message.Trim
            Session(IDDURATANUMRIGA) = Nothing
            myObject = Nothing
            composeChiave = String.Format("{0}_{1}",
            IDDURATANUMRIGA, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(CSTSERIELOTTO) = ""
            myObject = ""
            composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            txtEmailInvio.Text = ""
            txtNoteDocumento.Text = ""
            txtAllaPresezaDi.Text = ""
        End Try
        '---------------------------------------------
        'giu051122 BuidDett()
        Session(SWOP) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
        SWOP, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(SWOPMODSCATT) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
        SWOPMODSCATT, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtEmailInvio.Enabled = False : txtNoteDocumento.Enabled = False 'giu200122
        txtAllaPresezaDi.Enabled = False
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False
        btnAnnScAttCA.Visible = False
        'giu131221
        chkEvadiTutte.AutoPostBack = False
        chkEvadiTutte.Checked = False
        chkEvadiTutte.Visible = False : chkEvadiTutte.Enabled = False
        chkEvadiTutte.AutoPostBack = False
        '-------------------------------
        GridViewPrevT.Enabled = True
        txtDallaData.Enabled = True : txtAllaData.Enabled = True
        btnRicerca.Enabled = True : btnVerbale.Enabled = True : btnVerbale2.Enabled = True : btnElencoSc.Enabled = True
        imgBtnShowCalendarDD.Enabled = True : imgBtnShowCalendarAD.Enabled = True
        chkSoloDaEv.Enabled = True
        chkRespArea.Enabled = True
        txtRicercaClienteSede.Enabled = True : txtRicercaNContr.Enabled = True 'giu240123
        ddlClienti.Enabled = True
        DDLSediApp.Enabled = True
        'giu140423
        chkElencoXLS.Enabled = True
        rbtnOrdCliente.Enabled = True
        rbtnOrdScadenza.Enabled = True
        chkScadAnno.Enabled = True
        '---------
        'giu220123
        btnRicerca.BackColor = SEGNALA_KO
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        'giu220123 GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
        '---------
        'giu051122
        ' ''composeChiave = String.Format("{0}_{1}", _
        ' ''CSTSCADATTCA, UtenteConnesso.Codice)
        ' ''GetObjectToCache(composeChiave, myObject)
        '' ''-
        ' ''If Not (myObject Is Nothing) Then 'Session(CSTSCADATTCA) Is Nothing) Then
        ' ''    DVScadAtt = myObject 'Session(CSTSCADATTCA)
        ' ''    lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
        ' ''    ' ''DVScadAtt.RowFilter = "Qta_Evasa<>0"
        ' ''    ' ''lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
        ' ''    ' ''DVScadAtt.RowFilter = ""
        ' ''    GridViewDettCAAtt.DataSource = DVScadAtt
        ' ''    GridViewDettCAAtt.DataBind()
        ' ''    GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU280222
        ' ''Else
        ' ''    lblTotaleAtt.Text = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (myObject dopo agg.)", WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Sub
        ' ''End If
        '---------------------------------------------------------------------------------------
        ' ''Try
        ' ''    '- Cercare la prima scadenza 
        ' ''    txtDallaData.AutoPostBack = False
        ' ''    'GIU100222
        ' ''    ' ''txtDallaData.Text = GetPrimaDataSc()
        ' ''    If Not IsDate(txtDallaData.Text.Trim) Then
        ' ''        txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
        ' ''    End If
        ' ''    If CDate(txtDallaData.Text.Trim) > CDate(txtAllaData.Text.Trim) Then
        ' ''        txtDallaData.Text = txtAllaData.Text.Trim
        ' ''    End If
        ' ''    Call GetPrimaDataSc()
        ' ''    txtDallaData.AutoPostBack = True
        ' ''Catch ex As Exception

        ' ''End Try
        ' ''GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = True
        ' ''SvuodaGridT()
        '-
        'giu180123 180123
        'giu220123 troopo lento meglio che lo esegua direttamente Call btnVerbale_Click(Nothing, Nothing)

    End Sub
    Private Sub btnAnnScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnScAttCA.Click
        Session(SWOP) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
        SWOP, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(SWOPMODSCATT) = SWOPNESSUNA
        myObject = SWOPNESSUNA
        composeChiave = String.Format("{0}_{1}",
        SWOPMODSCATT, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtEmailInvio.Enabled = False : txtNoteDocumento.Enabled = False 'giu200122
        txtAllaPresezaDi.Enabled = False
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False
        btnAnnScAttCA.Visible = False
        'giu131221
        chkEvadiTutte.AutoPostBack = False
        chkEvadiTutte.Checked = False
        chkEvadiTutte.Visible = False : chkEvadiTutte.Enabled = False
        chkEvadiTutte.AutoPostBack = False
        '-------------------------------
        GridViewPrevT.Enabled = True
        '-
        txtDallaData.Enabled = True : txtAllaData.Enabled = True
        btnRicerca.Enabled = True : btnVerbale.Enabled = True : btnVerbale2.Enabled = True : btnElencoSc.Enabled = True
        imgBtnShowCalendarDD.Enabled = True : imgBtnShowCalendarAD.Enabled = True
        chkSoloDaEv.Enabled = True
        chkRespArea.Enabled = True
        txtRicercaClienteSede.Enabled = True : txtRicercaNContr.Enabled = True 'giu240123
        ddlClienti.Enabled = True
        DDLSediApp.Enabled = True
        'giu140423
        chkElencoXLS.Enabled = True
        rbtnOrdCliente.Enabled = True
        rbtnOrdScadenza.Enabled = True
        chkScadAnno.Enabled = True
        '---------
        composeChiave = String.Format("{0}_{1}",
        CSTSCADATTCA, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        '-
        If Not (myObject Is Nothing) Then 'Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = myObject 'Session(CSTSCADATTCA)
            lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
            GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU280222
        Else
            lblMessAttivita.Text = "Errore: SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (btnAnnScAttCA_Click)"
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando (btnAnnScAttCA_Click)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

    End Sub
#End Region

#Region "FILTRI E AZIONI SUL GRID"
    Private Sub chkSoloDaEv_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSoloDaEv.CheckedChanged

        myObject = IIf(chkSoloDaEv.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSoloDaEv.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        Try
            '- Cercare la prima scadenza 
            txtDallaData.AutoPostBack = False
            'GIU100222
            ' ''txtDallaData.Text = GetPrimaDataSc()
            If Not IsDate(txtDallaData.Text.Trim) Then
                txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
            End If
            If CDate(txtDallaData.Text.Trim) > CDate(txtAllaData.Text.Trim) Then
                txtDallaData.Text = txtAllaData.Text.Trim
            End If
            myObject = txtDallaData.Text
            composeChiave = String.Format("{0}_{1}",
            "txtDallaData.Text", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            'GIU290623
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "RicaricaDati", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '---------------------
            'giu140224
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "GetPrimaDataSc", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-----------------------------------------
            Call GetPrimaDataSc()
            txtDallaData.AutoPostBack = True
        Catch ex As Exception

        End Try

        SvuodaGridT()
    End Sub

    Private Sub chkAllScCA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAllScCA.CheckedChanged
        SetLnk()
        myObject = IIf(chkAllScCA.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkAllScCA.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        Call PopolaGridScAtt()
    End Sub

    Private Sub chkSingoloNS_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSingoloNS.CheckedChanged,
        chkSingoloNS2.CheckedChanged
        chkSingoloNS.AutoPostBack = False : chkSingoloNS2.AutoPostBack = False
        If sender.ID = chkSingoloNS2.ID Then
            chkSingoloNS.Checked = chkSingoloNS2.Checked
        Else
            chkSingoloNS2.Checked = chkSingoloNS.Checked
        End If
        chkSingoloNS.AutoPostBack = True : chkSingoloNS2.AutoPostBack = True
        lblNoteIntervento.Visible = chkSingoloNS.Checked
        txtNoteDocumento.Visible = chkSingoloNS.Checked
        lblInfoNoteIntervento.Visible = Not chkSingoloNS.Checked
        lblAllaPresenzaDi.Visible = chkSingoloNS.Checked
        txtAllaPresezaDi.Visible = chkSingoloNS.Checked
        myObject = IIf(chkSingoloNS.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSingoloNS.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        SetLnk()
        Call PopolaGridScAtt()
    End Sub

    Private Sub ddlClienti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlClienti.SelectedIndexChanged
        SetLnk()
        Dim SAVEStatobtnRicerca As Object = btnRicerca.BackColor
        SvuodaGridT()
        If ddlClienti.SelectedValue.Trim <> "" Then
            GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = False
            'GIU240123
            If txtRicercaNContr.Text.Trim <> "" Then
                txtRicercaNContr.AutoPostBack = False
                txtRicercaNContr.Text = ""
                txtRicercaNContr.AutoPostBack = True
            End If
            If txtRicercaClienteSede.Text.Trim <> "" Then
                txtRicercaClienteSede.AutoPostBack = False
                txtRicercaClienteSede.Text = ""
                txtRicercaClienteSede.AutoPostBack = True
            End If
            Call CaricaDatiSedi(False)
            'GIU290623 PER VELOCIZZARE LA RICERCA SE NON HO CAMBIATO PARAMETRI FONDAMENTALI ESEGUO IL FILTER SU DS
            If SAVEStatobtnRicerca <> SEGNALA_KO Then
                If RicercaDS(sender.ID) = True Then Exit Sub
            End If
            '-
        Else
            GridViewPrevT.Columns.Item(CellIdxT.RagSoc).Visible = True
            DDLSediApp.Items.Clear()
            DDLSediApp.Items.Add("")
            DDLSediApp.Items(0).Value = ""
            myObject = DDLSediApp
            composeChiave = String.Format("{0}_{1}",
                "DDLSediApp", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        myObject = ddlClienti.SelectedValue
        composeChiave = String.Format("{0}_{1}",
            "ddlClienti.SelectedValue", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub

    Private Sub chkEvadiTutte_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEvadiTutte.CheckedChanged
        'GIU130123 DISASSEGNO 
        'If chkEvadiTutte.Checked = False Or chkEvadiTutte.Visible = False Then Exit Sub
        If chkEvadiTutte.Visible = False Then Exit Sub
        '-
        Dim chkEvasa As CheckBox
        Try
            If chkEvadiTutte.Checked = True Then
                For Each row As GridViewRow In GridViewDettCAAtt.Rows
                    chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                    If chkEvasa.Checked = False Then
                        chkEvasa.Checked = True
                    End If
                Next
            Else
                'prendo la versione prima delle modifiche x evitare di disassegnare quelle evase in precedenza
                composeChiave = String.Format("{0}_{1}",
                   CSTSCADATTCA, UtenteConnesso.Codice)
                GetObjectToCache(composeChiave, myObject)
                DVScadAtt = myObject
                If Not (DVScadAtt Is Nothing) Then
                    Dim ind As Integer = 0
                    For Each row As GridViewRow In GridViewDettCAAtt.Rows
                        chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                        If chkEvasa.Checked = True Then
                            If CBool(DVScadAtt.Item(ind).Item("Qta_Selezionata").ToString.Trim) = False Then
                                chkEvasa.Checked = False
                            End If
                        End If
                        ind += 1
                    Next
                End If
            End If
        Catch ex As Exception
            lblMessAttivita.Text = "Errore, Seleziona tutte: " + ex.Message.Trim
        End Try
    End Sub

    Private Sub chkRespArea_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRespArea.CheckedChanged
        SetLnk()
        Try
            '- Cercare la prima scadenza 
            txtDallaData.AutoPostBack = False
            'GIU100222
            ' ''txtDallaData.Text = GetPrimaDataSc()
            If Not IsDate(txtDallaData.Text.Trim) Then
                txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
            End If
            If CDate(txtDallaData.Text.Trim) > CDate(txtAllaData.Text.Trim) Then
                txtDallaData.Text = txtAllaData.Text.Trim
            End If
            'GIU290623
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "RicaricaDati", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '---------------------
            'giu140224
            myObject = SWSI
            composeChiave = String.Format("{0}_{1}",
            "GetPrimaDataSc", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-----------------------------------------
            Call GetPrimaDataSc()
            txtDallaData.AutoPostBack = True
        Catch ex As Exception

        End Try
        SvuodaGridT()
    End Sub

    'giu110222 Carico SEDI
    Private Function CaricaDatiSedi(ByVal SWElenco As Boolean) As Boolean
        CaricaDatiSedi = False
        Dim strFiltroRicerca As String = ""
        Dim SWSingolo As Boolean = False
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Function
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Function
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        'PARAMETRI SP
        SqlDSPrevTElenco.SelectParameters("DallaData").DefaultValue = txtDallaData.Text.Trim
        SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = txtAllaData.Text.Trim
        strFiltroRicerca = "Elenco Scadenze Attività " + IIf(chkSoloDaEv.Checked, "non ancora evase", " tutte") + " nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        SqlDSPrevTElenco.SelectParameters("Escludi").DefaultValue = True 'chkNoEscludiInvioM.Checked
        SWSingolo = False
        SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = 0
        SqlDSPrevTElenco.SelectParameters("SoloDaEv").DefaultValue = chkSoloDaEv.Checked
        '' ''------------------------
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        myObject = SWTD(TD.ContrattoAssistenza)
        composeChiave = String.Format("{0}_{1}",
        CSTTIPODOC, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDOCUMENTI) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        IDDOCUMENTI, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(IDDURATANUMRIGA) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        IDDURATANUMRIGA, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Session(CSTSERIELOTTO) = ""
        myObject = ""
        composeChiave = String.Format("{0}_{1}",
        CSTSERIELOTTO, UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        txtNoteDocumento.Text = ""
        txtAllaPresezaDi.Text = ""
        txtEmailInvio.Text = ""
        '' ''-
        ImpostaFiltro()
        aDataViewScAtt = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)

        Dim dsDoc As New DSDocumenti
        If (aDataViewScAtt Is Nothing) Then
            'Session("dsDocElencoScad") = dsDoc
            myObject = dsDoc
            composeChiave = String.Format("{0}_{1}",
            "dsDocElencoScad", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            aDataViewScAtt = New DataView(dsDoc.ScadAtt)
            'Session("aDataViewScAtt") = aDataViewScAtt
            myObject = aDataViewScAtt
            composeChiave = String.Format("{0}_{1}",
            "aDataViewScAtt", UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            BuidDett()
            Exit Function
        End If
        'giu180620 GIU160520 PER SAPERE I CODICI VISITA     If InStr(myCodVisita, RowScadAtt.Cod_Articolo.Trim) > 0 Then
        Dim Comodo As String = ""
        Dim myCodVisita As String = GetMyCodVisita() 'giu250123
        ' ''Dim ObjDB As New DataBaseUtility
        ' ''Dim strSQL As String = "Select * From TipoContratto"
        ' ''Dim myCodVisita As String = "" : Dim Comodo As String = ""
        ' ''Try
        ' ''    dsDoc.Tables("TipoContratto").Clear()
        ' ''    dsDoc.Tables("TipoContratto").AcceptChanges()
        ' ''    ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsDoc, "TipoContratto")
        ' ''    For Each rsTC In dsDoc.Tables("TipoContratto").Select("")
        ' ''        Comodo = IIf(IsDBNull(rsTC!Descrizione), "", rsTC!CodVisita.ToString.Trim)
        ' ''        If InStr(myCodVisita, Comodo.Trim) > 0 Then
        ' ''        Else
        ' ''            myCodVisita += Comodo.Trim + ","
        ' ''        End If
        ' ''    Next
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "Caricamento Tabella TipoContratto. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Function
        ' ''End Try
        ' ''ObjDB = Nothing
        '-----------------------------------------------------------------
        Dim dc As DataColumn
        Dim myNSerieLottoPrec As String = "" : Dim SaveCliente As String = ""
        DDLSediApp.Items.Clear()
        DDLSediApp.Items.Add("")
        DDLSediApp.Items(0).Value = ""
        Dim Ind As Integer = 1
        Try
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = "RagSocApp,IndirApp,LocApp,SerieLotto"
            For i = 0 To aDataViewScAtt.Count - 1
                'giu190620
                If SWElenco = False Then
                    If InStr(myCodVisita, aDataViewScAtt.Item(i)("Cod_Articolo").ToString.Trim) > 0 Or
                             aDataViewScAtt.Item(i)("SiglaCA").ToString.Trim <> "CM" Then 'PER VISUAL.ANCHE I CT
                        'ok
                    Else 'scarto SOLO CHECK
                        Continue For
                    End If
                End If
                '-
                'giu110222
                If myNSerieLottoPrec <> Formatta.FormattaNomeFile(aDataViewScAtt.Item(i)("SerieLotto").ToString.Trim) Then
                    'ok
                    myNSerieLottoPrec = Formatta.FormattaNomeFile(aDataViewScAtt.Item(i)("SerieLotto").ToString.Trim) 'giu070523
                    SaveCliente = aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("IndirApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("LocApp").ToString.Trim + " (" +
                        aDataViewScAtt.Item(i)("PrApp").ToString.Trim + ")"
                    '-
                    Comodo = aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("IndirApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("LocApp").ToString.Trim + " (" +
                        aDataViewScAtt.Item(i)("PrApp").ToString.Trim + ")"
                    '-
                    DDLSediApp.Items.Add(Comodo)
                    DDLSediApp.Items(Ind).Value = Comodo
                    Ind += 1
                ElseIf SaveCliente <> aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("IndirApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("LocApp").ToString.Trim + " (" +
                        aDataViewScAtt.Item(i)("PrApp").ToString.Trim + ")" Then
                    'ok
                    myNSerieLottoPrec = Formatta.FormattaNomeFile(aDataViewScAtt.Item(i)("SerieLotto").ToString.Trim) 'giu070523
                    SaveCliente = aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("IndirApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("LocApp").ToString.Trim + " (" +
                        aDataViewScAtt.Item(i)("PrApp").ToString.Trim + ")"
                    '-
                    Comodo = aDataViewScAtt.Item(i)("RagSocApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("IndirApp").ToString.Trim + " - " +
                        aDataViewScAtt.Item(i)("LocApp").ToString.Trim + " (" +
                        aDataViewScAtt.Item(i)("PrApp").ToString.Trim + ")"
                    '-
                    DDLSediApp.Items.Add(Comodo)
                    DDLSediApp.Items(Ind).Value = Comodo
                    Ind += 1
                Else
                    Continue For
                End If
            Next
        Catch ex As Exception
            lblMessAttivita.Text = "Errore in Carica Sedi: " + ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Carica Sedi: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '-
        'Session("dsDocElencoScad") = Nothing
        myObject = Nothing
        composeChiave = String.Format("{0}_{1}",
        "dsDocElencoScad", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        aDataViewScAtt = Nothing
        'Session("aDataViewScAtt") = Nothing
        myObject = Nothing
        composeChiave = String.Format("{0}_{1}",
        "aDataViewScAtt", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        myObject = DDLSediApp
        composeChiave = String.Format("{0}_{1}",
            "DDLSediApp", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        CaricaDatiSedi = True
    End Function

    Private Sub DDLSediApp_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLSediApp.SelectedIndexChanged
        SetLnk()
        SvuodaGridT()
        If ddlClienti.SelectedValue.Trim <> "" Then
            'GIU240123
            If txtRicercaNContr.Text.Trim <> "" Then
                txtRicercaNContr.AutoPostBack = False
                txtRicercaNContr.Text = ""
                txtRicercaNContr.AutoPostBack = True
            End If
            '-
            If txtRicercaClienteSede.Text.Trim <> "" Then
                txtRicercaClienteSede.AutoPostBack = False
                txtRicercaClienteSede.Text = ""
                txtRicercaClienteSede.AutoPostBack = True
            End If
            'Call btnRicerca_Click(Nothing, Nothing)
            Call CaricaDatiScAtt(False)
        End If
        '-
        myObject = DDLSediApp.SelectedValue
        composeChiave = String.Format("{0}_{1}",
            "DDLSediApp.SelectedValue", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSelectEventArgs) Handles GridViewPrevT.SelectedIndexChanging
        Try
            If (aDataViewScAtt Is Nothing) Then
                Session(SWOP) = SWOPNESSUNA
                myObject = SWOPNESSUNA
                composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(SWOPCLI) = SWOPNESSUNA
                myObject = SWOPNESSUNA
                composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
            End If
            If GridViewPrevT.Rows.Count = 0 Then
                Session(SWOP) = SWOPNESSUNA
                myObject = SWOPNESSUNA
                composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
                Session(SWOPCLI) = SWOPNESSUNA
                myObject = SWOPNESSUNA
                composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
            End If
            'Dim mySL As String = Session(CSTSERIELOTTO)
            composeChiave = String.Format("{0}_{1}",
                CSTSERIELOTTO, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Dim mySL As String = myObject
            Session(CSTSERIELOTTO) = mySL
            '-
            If IsNothing(mySL) Then
                mySL = ""
            End If
            If String.IsNullOrEmpty(mySL) Then
                mySL = ""
            End If
            If chkSingoloNS.Checked = False Then
                mySL = ""
            End If
            composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Session(SWOP) = myObject
            '-
            If myObject <> "" Then 'Session(SWOP) <> "" Then
                e.Cancel = True
                lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica: " & mySL.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica: <br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            composeChiave = String.Format("{0}_{1}",
               SWOPCLI, UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            Session(SWOPCLI) = myObject
            '-
            If myObject <> "" Then 'Session(SWOPCLI) <> "" Then
                e.Cancel = True
                lblMessAttivita.Text = "Attenzione, Completare prima l'operazione di Modifica: " & mySL.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica: <br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Catch ex As Exception
            'NULLA
        End Try

    End Sub

    Private Sub rbtnOrdCliente_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnOrdCliente.CheckedChanged
        lnkElencoSc.Visible = False
    End Sub

    Private Sub rbtnOrdScadenza_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnOrdScadenza.CheckedChanged
        lnkElencoSc.Visible = False
    End Sub

    ' ''Private Sub GridViewDettCAAtt_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDettCAAtt.SelectedIndexChanged
    ' ''    Try
    ' ''        If (aDataViewScAtt Is Nothing) Then
    ' ''            composeChiave = String.Format("{0}_{1}", _
    ' ''            "aDataViewScAtt", UtenteConnesso.Codice)
    ' ''            GetObjectToCache(composeChiave, myObject)
    ' ''            aDataViewScAtt = myObject
    ' ''        End If
    ' ''        Dim myRowIndex1 As Integer = GridViewPrevT.SelectedIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
    ' ''        '-
    ' ''        If (DVScadAtt Is Nothing) Then
    ' ''            composeChiave = String.Format("{0}_{1}", _
    ' ''            CSTSCADATTCA, UtenteConnesso.Codice)
    ' ''            GetObjectToCache(composeChiave, myObject)
    ' ''            DVScadAtt = myObject
    ' ''        End If
    ' ''        Dim myRowIndex2 As Integer = GridViewDettCAAtt.SelectedIndex + (GridViewDettCAAtt.PageSize * GridViewDettCAAtt.PageIndex)
    ' ''        Dim pSerieLotto As String = ""
    ' ''        Try
    ' ''            If Not (DVScadAtt Is Nothing) And Not (aDataViewScAtt Is Nothing) Then
    ' ''                pSerieLotto = DVScadAtt.Item(myRowIndex2).Item("SerieLotto").ToString.Trim
    ' ''                txtNoteDocumento.Text = GetNoteSL(aDataViewScAtt.Item(myRowIndex1).Item("NoteRitiro").ToString.Trim)
    ' ''            End If
    ' ''        Catch ex As Exception
    ' ''            'NON MI INTERESSA
    ' ''        End Try
    ' ''    Catch ex As Exception
    ' ''        'NON MI INTERESSA
    ' ''    End Try
    ' ''End Sub

    Private Sub txtRicercaClienteSede_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaClienteSede.TextChanged
        'GIU290623 PER VELOCIZZARE LA RICERCA SE NON HO CAMBIATO PARAMETRI FONDAMENTALI ESEGUO IL FILTER SU DS
        If btnRicerca.BackColor <> SEGNALA_KO Then
            If RicercaDS(sender.ID) = True Then Exit Sub
        End If
        '-
        btnRicerca.BackColor = SEGNALA_KO
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub

    Private Sub txtRicercaNContr_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaNContr.TextChanged
        If txtRicercaNContr.Text.Trim <> "" Then
            If Not IsNumeric(txtRicercaNContr.Text.Trim) Then
                txtRicercaNContr.BackColor = SEGNALA_KO
                Exit Sub
            End If
        End If
        'GIU290623 PER VELOCIZZARE LA RICERCA SE NON HO CAMBIATO PARAMETRI FONDAMENTALI ESEGUO IL FILTER SU DS
        If btnRicerca.BackColor <> SEGNALA_KO Then
            If RicercaDS(sender.ID) = True Then Exit Sub
        End If
        '-
        btnRicerca.BackColor = SEGNALA_KO
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub
    'GIU290623 PER VELOCIZZARE LA RICERCA SE NON HO CAMBIATO PARAMETRI FONDAMENTALI ESEGUO IL FILTER SU DS
    Private Function RicercaDS(ByVal sender As Object) As Boolean
        RicercaDS = False
        Dim SWSviluppo As String = ""
        lblMessDebug.Visible = False
        'Exit Function
        Try
            SWSviluppo = ConfigurationManager.AppSettings("sviluppo")
            Dim dsDoc As New DSDocumenti
            composeChiave = String.Format("{0}_{1}",
            "dsDocElencoScad", UtenteConnesso.Codice)
            GetObjectToCache(composeChiave, myObject)
            If (myObject Is Nothing) Then
                Exit Function
            End If
            dsDoc = myObject
            aDataViewScAtt = New DataView(dsDoc.ScadAtt)
            If (aDataViewScAtt Is Nothing) Then
                Exit Function
            End If
            If sender = txtRicercaNContr.ID Then
                If txtRicercaNContr.Text.Trim <> "" Then
                    If ddlClienti.SelectedValue.Trim <> "" Then
                        ddlClienti.AutoPostBack = False
                        ddlClienti.SelectedIndex = -1
                        ddlClienti.AutoPostBack = True
                        '-
                        DDLSediApp.AutoPostBack = False
                        DDLSediApp.SelectedIndex = -1
                        DDLSediApp.AutoPostBack = True
                    End If
                    If txtRicercaClienteSede.Text.Trim <> "" Then
                        txtRicercaClienteSede.AutoPostBack = False
                        txtRicercaClienteSede.Text = ""
                        txtRicercaClienteSede.AutoPostBack = True
                    End If
                    aDataViewScAtt.RowFilter = "Numero=" + txtRicercaNContr.Text.Trim
                End If
            ElseIf sender = txtRicercaClienteSede.ID Then
                If txtRicercaClienteSede.Text.Trim <> "" Then
                    If txtRicercaNContr.Text.Trim <> "" Then
                        txtRicercaNContr.AutoPostBack = False
                        txtRicercaNContr.Text = ""
                        txtRicercaNContr.AutoPostBack = True
                        Exit Function
                    End If
                    txtRicercaNContr.AutoPostBack = False
                    txtRicercaNContr.Text = ""
                    txtRicercaNContr.AutoPostBack = True

                    '---
                    If ddlClienti.SelectedValue.Trim <> "" Then
                        txtRicercaNContr.AutoPostBack = False
                        txtRicercaNContr.Text = ""
                        txtRicercaNContr.AutoPostBack = True
                        '-
                        DDLSediApp.AutoPostBack = False
                        DDLSediApp.SelectedIndex = -1
                        DDLSediApp.AutoPostBack = True
                        '-
                        aDataViewScAtt.RowFilter = "Cod_Cliente = '" & Controlla_Apice(ddlClienti.SelectedValue.Trim) & "'"
                        aDataViewScAtt.RowFilter += " AND RagSocApp LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%'"
                    Else
                        aDataViewScAtt.RowFilter = "Rag_SocDenom LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%'"
                        aDataViewScAtt.RowFilter += " OR RagSocApp LIKE '%" & Controlla_Apice(txtRicercaClienteSede.Text.Trim) & "%'"
                    End If
                End If
            ElseIf sender = ddlClienti.ID Then
                If ddlClienti.SelectedValue.Trim <> "" Then
                    txtRicercaNContr.AutoPostBack = False
                    txtRicercaNContr.Text = ""
                    txtRicercaNContr.AutoPostBack = True
                    '-
                    txtRicercaClienteSede.AutoPostBack = False
                    txtRicercaClienteSede.Text = ""
                    txtRicercaClienteSede.AutoPostBack = True
                    '---
                    aDataViewScAtt.RowFilter = "Cod_Cliente = '" & Controlla_Apice(ddlClienti.SelectedValue.Trim) & "'"
                End If
            End If
            If aDataViewScAtt.RowFilter = "" Then Exit Function
            If aDataViewScAtt.Count = 0 Then Exit Function
            RicercaDS = True
            '-
            If ddlClienti.SelectedIndex = -1 Then
                DDLSediApp.Items.Clear()
                DDLSediApp.Items.Add("")
                DDLSediApp.Items(0).Value = ""
                myObject = DDLSediApp
                composeChiave = String.Format("{0}_{1}",
                    "DDLSediApp", UtenteConnesso.Codice)
                SetObjectToCache(composeChiave, myObject)
                '-
            End If

            If Not String.IsNullOrEmpty(SWSviluppo) Then
                If SWSviluppo.Trim.ToUpper = "TRUE" Then
                    lblMessDebug.Visible = True
                    lblMessDebug.Text = "DEBUG - RicercaDS: OK CON SUCCESSO: " + Format(Now, "dd/MM/yyyy, HH:mm:ss")
                End If
            End If
            '-----------
            'OK
            myObject = aDataViewScAtt
            BuidDett()
        Catch ex As Exception
            If Not String.IsNullOrEmpty(SWSviluppo) Then
                If SWSviluppo.Trim.ToUpper = "TRUE" Then
                    lblMessDebug.Visible = True
                    lblMessDebug.Text = "DEBUG - ERRORE RicercaDS: " + ex.Message.Trim
                End If
            End If
            '-----------
            RicercaDS = False
            Exit Function
        End Try
    End Function
#End Region

    Private Sub LnkMenuStatisPR_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkMenuStatisPR.Click
        If (aDataViewScAtt Is Nothing) Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPCLI) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        If GridViewPrevT.Rows.Count = 0 Then
            Session(SWOP) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOP, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
            Session(SWOPCLI) = SWOPNESSUNA
            myObject = SWOPNESSUNA
            composeChiave = String.Format("{0}_{1}",
                SWOPCLI, UtenteConnesso.Codice)
            SetObjectToCache(composeChiave, myObject)
            '-
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
            CSTSERIELOTTO, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Dim mySL As String = myObject
        Session(CSTSERIELOTTO) = mySL
        If IsNothing(mySL) Then
            mySL = ""
        End If
        If String.IsNullOrEmpty(mySL) Then
            mySL = ""
        End If
        If chkSingoloNS.Checked = False Then
            mySL = ""
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
           SWOP, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOP) = myObject
        If myObject <> "" Then 'Session(SWOP) <> "" Then
            lblMessAttivita.Text = "Attenzione: Completare prima l'operazione di Modifica: " & mySL.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!<br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        composeChiave = String.Format("{0}_{1}",
           SWOPCLI, UtenteConnesso.Codice)
        GetObjectToCache(composeChiave, myObject)
        Session(SWOPCLI) = myObject
        If myObject <> "" Then 'If Session(SWOPCLI) <> "" Then
            lblMessAttivita.Text = "Attenzione: Completare prima l'operazione di Modifica: " & mySL.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione di Modifica!!!<br>" & mySL.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        myObject = IIf(chkSingoloNS.Checked, True, False)
        composeChiave = String.Format("{0}_{1}",
        "chkSingoloNS.Checked", UtenteConnesso.Codice)
        SetObjectToCache(composeChiave, myObject)
        '-
        Response.Redirect("WF_MenuStatisPR.aspx?labelForm=Preventivi per Agente/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]")
    End Sub
End Class