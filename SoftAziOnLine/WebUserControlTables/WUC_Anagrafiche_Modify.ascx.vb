Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
'giu080113
Imports System.Data.SqlClient

Partial Public Class WUC_Anagrafiche_Modify
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Public Rk As StrAnagrCliFor

    'GIU080112
    Dim Tab_Vocali() As String
    Dim Tab_Consonanti() As String
    Dim car As String
    Dim Ind1 As Integer
    Dim Ind2 As Integer
    Dim Ind3 As Integer
    Dim Ind4 As Integer
    Dim Cod_Fisc As String
    Dim Codice_Fiscale(17) As String

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSAnagrCliFor.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSIVA.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        If TxtStato.Text.Trim <> "I" And TxtStato.Text.Trim <> "IT" And TxtStato.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
        If Session(TIPORK) = "C" Then
            lblLabelTipoRK.Text = "Modifica anagrafica Clienti"
        ElseIf Session(TIPORK) = "F" Then
            lblLabelTipoRK.Text = "Modifica anagrafica Fornitori"
        Else
            If Session(CSTTABCLIFOR) = "Cli" Then
                Session(TIPORK) = "C"
                lblLabelTipoRK.Text = "Modifica anagrafica Clienti"
            ElseIf Session(CSTTABCLIFOR) = "For" Then
                Session(TIPORK) = "F"
                lblLabelTipoRK.Text = "Modifica anagrafica Fornitori"
            Else 'sessione scaduta??
                lblLabelTipoRK.Text = "Modifica anagrafica"
            End If
        End If
        'giu140113 viene sempre testato
        ' ''If (Not IsPostBack) Then
        If Session(CSTTABCLIFOR) = "Cli" Then
            SqlDSAnagrCliFor.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
            SqlDSAnagrCliFor.UpdateCommand = "Update_AnagrCliFor_C"
            lblIPA.Visible = True : txtIPA.Visible = True : chkSplitIVA.Visible = True
        ElseIf Session(CSTTABCLIFOR) = "For" Then
            SqlDSAnagrCliFor.SelectCommand = "SELECT * FROM [Fornitori] WHERE ([Codice_CoGe] = @Codice)"
            SqlDSAnagrCliFor.UpdateCommand = "Update_AnagrCliFor_F"
            lblIPA.Visible = False : txtIPA.Visible = False : chkSplitIVA.Visible = False
        Else 'DEFAULT
            SqlDSAnagrCliFor.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
            SqlDSAnagrCliFor.UpdateCommand = "Update_AnagrCliFor_C"
            lblIPA.Visible = True : txtIPA.Visible = True : chkSplitIVA.Visible = True
        End If
        ' ''End If
    End Sub
    'giu190122 se volessero anche le atre email implementare update_AnagrCliFor_?? 
    Public Sub SvuotaCampi()
        chkSplitIVA.Checked = False
        txtIPA.Text = "" : txtIPA.BackColor = SEGNALA_OK
        txtRagioneSoc.Text = "" : txtRagioneSoc.BackColor = SEGNALA_OK
        txtDenominazione.Text = ""
        txtRiferimento.Text = ""
        TxtIndirizzo.Text = ""
        TxtCAP.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        TxtStato.Text = "" : TxtStato.BackColor = SEGNALA_OK
        txtCodiceFiscale.Text = ""
        txtPartitaIVA.Text = ""
        txtTelefono1.Text = ""
        txtTelefono2.Text = ""
        txtFax.Text = ""
        PosizionaItemDDL(Rk.Regime_Iva, ddlIVASpese)
        txtEmail.Text = "" 'giu060514
        txtPECEmail.Text = "" 'giu190122
        Rk = Nothing
        Session(RKANAGRCLIFOR) = Rk
    End Sub

    Public Sub PopolaCampi()
        lblCodice.Text = Session(IDANAGRCLIFOR).ToString.Trim
        Rk = Session(RKANAGRCLIFOR)
        If (Rk.Rag_Soc Is Nothing) Then
            SvuotaCampi()
            Exit Sub
        End If
        chkSplitIVA.Checked = Rk.SplitIVA
        txtIPA.Text = Rk.IPA : txtIPA.BackColor = SEGNALA_OK
        txtRagioneSoc.Text = Rk.Rag_Soc : txtRagioneSoc.BackColor = SEGNALA_OK
        txtDenominazione.Text = Rk.Denominazione
        txtRiferimento.Text = Rk.Riferimento
        TxtIndirizzo.Text = Rk.Indirizzo
        TxtCAP.Text = Rk.Cap
        TxtLocalita.Text = Rk.Localita
        TxtProvincia.Text = Rk.Provincia
        TxtStato.Text = Rk.Nazione
        txtCodiceFiscale.Text = Rk.Codice_Fiscale : txtCodiceFiscale.BackColor = SEGNALA_OK
        txtPartitaIVA.Text = Rk.Partita_IVA : txtPartitaIVA.BackColor = SEGNALA_OK
        txtTelefono1.Text = Rk.Telefono1
        txtTelefono2.Text = Rk.Telefono2
        txtFax.Text = Rk.Fax
        txtEmail.Text = Rk.EMail 'giu060514
        txtEmail.BackColor = SEGNALA_OK
        txtPECEmail.Text = Rk.PECEMail
        txtPECEmail.BackColor = SEGNALA_OK
        PosizionaItemDDL(Rk.Regime_Iva, ddlIVASpese, True)
    End Sub

    Public Function AggiornaAnagrCliFor() As Boolean
        AggiornaAnagrCliFor = True
        If Not (String.IsNullOrEmpty(txtIPA.Text)) Then 'giu020119
            If txtIPA.Text.Trim.Length <> 6 And txtIPA.Text.Trim.Length <> 7 Then
                txtIPA.BackColor = SEGNALA_KO
                AggiornaAnagrCliFor = False
                txtIPA.ToolTip = " - Codice IPA: Lunghezza 6 per la PA altrimenti 7 per Privati/Ditte"
            End If
        End If
        If txtRagioneSoc.Text.Trim = "" Then
            txtRagioneSoc.BackColor = SEGNALA_KO : AggiornaAnagrCliFor = False
        Else
            txtRagioneSoc.BackColor = SEGNALA_OK
        End If
        If TxtProvincia.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
                TxtProvincia.BackColor = SEGNALA_KO
                AggiornaAnagrCliFor = False
            Else
                TxtProvincia.BackColor = SEGNALA_OK
            End If
        Else
            TxtProvincia.BackColor = SEGNALA_OK
        End If
        If TxtStato.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtStato.Text, Def.NAZIONI, Session(ESERCIZIO)) = "" Then
                TxtStato.BackColor = SEGNALA_KO
                AggiornaAnagrCliFor = False
            Else
                TxtStato.BackColor = SEGNALA_OK
            End If
        Else
            TxtStato.BackColor = SEGNALA_OK
        End If
        'giu190122
        If txtEmail.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmail.Text.Trim) = False Then
                txtEmail.BackColor = SEGNALA_KO
                AggiornaAnagrCliFor = False
            Else
                txtEmail.BackColor = SEGNALA_OK
            End If
        Else
            txtEmail.BackColor = SEGNALA_OK
        End If
        If txtPECEmail.Text.Trim <> "" Then
            If ConvalidaEmail(txtPECEmail.Text.Trim) = False Then
                txtPECEmail.BackColor = SEGNALA_KO
                AggiornaAnagrCliFor = False
            Else
                txtPECEmail.BackColor = SEGNALA_OK
            End If
        Else
            txtPECEmail.BackColor = SEGNALA_OK
        End If
        '-
        If AggiornaAnagrCliFor = False Then
            Exit Function
        End If
        'giu080113 Controllo P.I./C.F.
        txtCodiceFiscale.BackColor = SEGNALA_OK
        txtPartitaIVA.BackColor = SEGNALA_OK
        If txtCodiceFiscale.Text.Trim <> "" Or txtPartitaIVA.Text.Trim <> "" Then
            Dim myErrore As String = ""
            Dim SWErrore As Boolean = False
            Dim SWErroreC As Boolean = False
            SWErroreC = Not Controlla_PICF(myErrore)
            If txtCodiceFiscale.Text.Trim <> "" Then
                If ControlloDoppio(txtCodiceFiscale.Text, "CF") Then
                    txtCodiceFiscale.BackColor = SEGNALA_KO
                    SWErrore = True
                    myErrore += "Il Codice fiscale inserito è utilizzato per altre anagrafiche. <br>"
                End If
            End If
            '-
            If txtPartitaIVA.Text.Trim <> "" Then
                If ControlloDoppio(txtPartitaIVA.Text, "PIVA") Then
                    txtPartitaIVA.BackColor = SEGNALA_KO
                    SWErrore = True
                    myErrore += "La Partita IVA inserita è utilizzata per altre anagrafiche. <br>"
                End If
            End If
            '--- messaggio
            If SWErrore = True Or SWErroreC = True Then
                If SWErroreC = True Then
                    ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
                If Session("AvvisaDoppio") Then
                    Session("AvvisaDoppio") = False
                    ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
            End If
        ElseIf txtCodiceFiscale.Text.Trim = "" And txtPartitaIVA.Text.Trim = "" Then
            If Session("AvvisaDoppio") = True Then
                txtCodiceFiscale.BackColor = SEGNALA_KO
                txtPartitaIVA.BackColor = SEGNALA_KO
                Session("AvvisaDoppio") = False
                AggiornaAnagrCliFor = False
                ModalPopup.Show("Controllo dati cliente", "Attenzione, Partiva IVA e Codice fiscale mancanti. <br>" & _
                                "Aggiornare nuovamente se si vuole prosegure senza questi dati.", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        End If
        '-----------------------------
        Rk = Session(RKANAGRCLIFOR)
        If (Rk.Rag_Soc Is Nothing) Then
            SvuotaCampi()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento anagrafica non riuscito.", WUC_ModalPopup.TYPE_ERROR)
            AggiornaAnagrCliFor = False
            Exit Function
        End If
        Rk.SplitIVA = chkSplitIVA.Checked
        Rk.IPA = Mid(txtIPA.Text.Trim, 1, 10)
        Rk.Rag_Soc = Mid(txtRagioneSoc.Text.Trim, 1, 50)
        Rk.Denominazione = Mid(txtDenominazione.Text.Trim, 1, 50)
        Rk.Riferimento = Mid(txtRiferimento.Text.Trim, 1, 500)
        Rk.Indirizzo = Mid(TxtIndirizzo.Text.Trim, 1, 50)
        Rk.Cap = Mid(TxtCAP.Text.Trim, 1, 5)
        Rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        Rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        Rk.Nazione = Mid(TxtStato.Text.Trim, 1, 3)
        Rk.Codice_Fiscale = Mid(txtCodiceFiscale.Text.Trim, 1, 16)
        Rk.Partita_IVA = txtPartitaIVA.Text.Trim
        Rk.Telefono1 = Mid(txtTelefono1.Text.Trim, 1, 30)
        Rk.Telefono2 = Mid(txtTelefono2.Text.Trim, 1, 30)
        Rk.Fax = Mid(txtFax.Text.Trim, 1, 30)
        Rk.EMail = Mid(txtEmail.Text.Trim, 1, 100) 'giu060514
        Rk.PECEMail = Mid(txtPECEmail.Text.Trim, 1, 310)
        Rk.Regime_Iva = ddlIVASpese.SelectedValue
        Session(RKANAGRCLIFOR) = Rk
        Try
            SqlDSAnagrCliFor.UpdateParameters.Item("Codice_CoGe").DefaultValue = Session(IDANAGRCLIFOR)
            SqlDSAnagrCliFor.UpdateParameters.Item("Rag_Soc").DefaultValue = Rk.Rag_Soc
            SqlDSAnagrCliFor.UpdateParameters.Item("Denominazione").DefaultValue = Rk.Denominazione
            SqlDSAnagrCliFor.UpdateParameters.Item("Riferimento").DefaultValue = Rk.Riferimento
            SqlDSAnagrCliFor.UpdateParameters.Item("Indirizzo").DefaultValue = Rk.Indirizzo
            SqlDSAnagrCliFor.UpdateParameters.Item("NumeroCivico").DefaultValue = Rk.NumeroCivico
            SqlDSAnagrCliFor.UpdateParameters.Item("Cap").DefaultValue = Rk.Cap
            SqlDSAnagrCliFor.UpdateParameters.Item("Localita").DefaultValue = Rk.Localita
            SqlDSAnagrCliFor.UpdateParameters.Item("Provincia").DefaultValue = Rk.Provincia
            SqlDSAnagrCliFor.UpdateParameters.Item("Nazione").DefaultValue = Rk.Nazione
            SqlDSAnagrCliFor.UpdateParameters.Item("Codice_Fiscale").DefaultValue = Rk.Codice_Fiscale
            SqlDSAnagrCliFor.UpdateParameters.Item("Partita_IVA").DefaultValue = Rk.Partita_IVA
            SqlDSAnagrCliFor.UpdateParameters.Item("Telefono1").DefaultValue = Rk.Telefono1
            SqlDSAnagrCliFor.UpdateParameters.Item("Telefono2").DefaultValue = Rk.Telefono2
            SqlDSAnagrCliFor.UpdateParameters.Item("Fax").DefaultValue = Rk.Fax
            SqlDSAnagrCliFor.UpdateParameters.Item("Regime_Iva").DefaultValue = Rk.Regime_Iva
            SqlDSAnagrCliFor.UpdateParameters.Item("EMail").DefaultValue = Rk.EMail 'giu060514
            SqlDSAnagrCliFor.UpdateParameters.Item("PECEMail").DefaultValue = Rk.PECEMail
            If Not String.IsNullOrEmpty(txtIPA.Text.Trim) And txtIPA.Visible = True Then
                SqlDSAnagrCliFor.UpdateParameters.Item("IPA").DefaultValue = Rk.IPA
            End If
            If txtIPA.Visible = True Then
                SqlDSAnagrCliFor.UpdateParameters.Item("SplitIVA").DefaultValue = IIf(Rk.SplitIVA, 1, 0)
            End If
            SqlDSAnagrCliFor.Update()
            SqlDSAnagrCliFor.DataBind()
            'giu180912 giu101012 aggiorna MySQL Clienti per IREDEEM
            Dim errorMsg As String = ""
            If App.GetDatiAbilitazioni(CSTABILCOGE, "MySQLCli" + Session(ESERCIZIO).ToString.Trim, "", errorMsg) = True Then
                'ok chiamo la SP per aggiornare ma senza TRANSAZIONE
                'Session(COD_CLIENTE)
                errorMsg = ""
                If DataBaseUtility.MySQLClienti(Session(IDANAGRCLIFOR), CSTAttivo, errorMsg) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Aggiorna dati cliente (MySQL)", _
                        String.Format("Errore, contattare l'amministratore di sistema. Errore: {0}", errorMsg), _
                        WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
            '--------------------------------------------
            '-----
            PopolaCampi()
            'Dim errorMsg As String = ""
            errorMsg = ""
            If (Not App.CaricaClienti(Session(ESERCIZIO), errorMsg)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Caricamento Dati", _
                    String.Format("Errore nel caricamento Elenco Clienti, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_INFO)
                SessionUtility.LogOutUtente(Session, Response)
                Return False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    'giualb080113
    Private Function Controlla_PICF(ByRef _ElErr As String) As Boolean
        Controlla_PICF = False
        _ElErr = "Errore "
        Dim dvCliFor As DataView
        Try
            dvCliFor = SqlDSAnagrCliFor.Select(DataSourceSelectArguments.Empty)
            If (dvCliFor Is Nothing) Then
                _ElErr += "nella lettura anagrafica: inesistente in tabella. <br>"
                Exit Function
            End If
            If dvCliFor.Count > 0 Then
                'ok esiste
            Else
                _ElErr += "nella lettura anagrafica: inesistente in tabella. <br>"
                Exit Function
            End If
        Catch ex As Exception
            _ElErr += "nella lettura anagrafica: " & ex.Message.Trim & " <br>"
            Exit Function
        End Try
        'DATI CHE MI SERVONO
        Dim myDataNas As String = ""
        If Not IsDBNull(dvCliFor.Item(0).Item("Data_Nascita")) Then
            If Not CDate(dvCliFor.Item(0).Item("Data_Nascita")) = DATANULL Then
                myDataNas = Format(dvCliFor.Item(0).Item("Data_Nascita"), FormatoData)
            End If
        End If
        '-
        Dim myNaz As String = ""
        If Not IsDBNull(dvCliFor.Item(0).Item("Nazione")) Then
            myNaz = dvCliFor.Item(0).Item("Nazione").ToString.Trim
        End If
        '--------------------
        Controlla_PICF = True
        If CBool(dvCliFor.Item(0).Item("Societa")) = False Then 'PERSONA FISICA
            txtCodiceFiscale.Text = UCase(txtCodiceFiscale.Text).Trim
            If Not String.IsNullOrEmpty(txtCodiceFiscale.Text.Trim) Then
                If CF_Controllo("", "", myDataNas, "", "", txtCodiceFiscale.Text) = False Then
                    txtCodiceFiscale.BackColor = SEGNALA_KO
                    _ElErr += " - Codice Fiscale"
                    Controlla_PICF = False
                End If
            Else
                txtCodiceFiscale.BackColor = SEGNALA_KO
                _ElErr += " - (Persona fisica) Codice Fiscale obbligatorio <br>"
                'giu150113 bloccante Controlla_PICF = False
            End If
            If Not String.IsNullOrEmpty(txtPartitaIVA.Text.Trim) Then 'FACOLTATIVO 
                Dim Result As Integer
                'Se la nazione non è Italia salta il controllo sulla partita iva
                myNaz = TxtStato.Text.Trim
                If myNaz = "I" Or myNaz = "IT" Or myNaz = "ITA" Then
                    ' 0 = corretta
                    ' 1 = Lunghezza errata
                    ' 2 = Presenza di caratteri alfabetici
                    ' 3 = Le prime 7 cifre sono uguali a zero
                    ' 4 = Ufficio IVA errato o mancante in tabella
                    ' 5 = Codice di controllo errato
                    Result = PI_controllo(txtPartitaIVA.Text)
                    Select Case Result
                        Case 1
                            _ElErr += " - Partita IVA (deve essere di 11 cifre) <br>"
                        Case 2
                            _ElErr += " - Partita IVA (deve essere solo numerica) <br>"
                        Case 3
                            _ElErr += " - Partita IVA (le prime 7 cifre non possono essere zeri) <br>"
                        Case 4
                            _ElErr += " - Partita IVA (ufficio IVA errato) <br>"
                        Case 5
                            _ElErr += " - Partita IVA (codice controllo errato) <br>"
                    End Select
                    If Result <> 0 Then
                        txtPartitaIVA.BackColor = SEGNALA_KO
                        Controlla_PICF = False
                    End If
                End If
            End If
        Else 'PERSONA GIURIDICA
            If Not String.IsNullOrEmpty(txtCodiceFiscale.Text.Trim) Then 'FACOLTATIVO
                If IsNumeric(txtCodiceFiscale.Text.Trim) And txtCodiceFiscale.Text.Trim.Length = 11 Then
                    'ok txtCodiceFiscale.BackColor = SEGNALA_OK
                ElseIf CF_Controllo("", "", myDataNas, "", "", txtCodiceFiscale.Text) = False Then
                    txtCodiceFiscale.BackColor = SEGNALA_KO
                    _ElErr += " - Codice Fiscale <br>"
                    Controlla_PICF = False
                End If
            End If
            txtPartitaIVA.Text = txtPartitaIVA.Text.Trim
            Dim Result As Integer
            If Not (String.IsNullOrEmpty(txtPartitaIVA.Text)) Then
                'Se la nazione non è Italia salta il controllo sulla partita iva
                myNaz = TxtStato.Text.Trim
                If myNaz = "I" Or myNaz = "IT" Or myNaz = "ITA" Then
                    ' 0 = corretta
                    ' 1 = Lunghezza errata
                    ' 2 = Presenza di caratteri alfabetici
                    ' 3 = Le prime 7 cifre sono uguali a zero
                    ' 4 = Ufficio IVA errato o mancante in tabella
                    ' 5 = Codice di controllo errato
                    Result = PI_controllo(txtPartitaIVA.Text)
                    Select Case Result
                        Case 1
                            _ElErr += " - Partita IVA (deve essere di 11 cifre) <br>"
                        Case 2
                            _ElErr += " - Partita IVA (deve essere solo numerica) <br>"
                        Case 3
                            _ElErr += " - Partita IVA (le prime 7 cifre non possono essere zeri) <br>"
                        Case 4
                            _ElErr += " - Partita IVA (ufficio IVA errato) <br>"
                        Case 5
                            _ElErr += " - Partita IVA (codice controllo errato) <br>"
                    End Select
                    If Result <> 0 Then
                        txtPartitaIVA.BackColor = SEGNALA_KO
                        Controlla_PICF = False
                    End If
                End If
            Else
                _ElErr += " - (Persona giuridica) Partita IVA obbligatoria <br>"
                txtPartitaIVA.BackColor = SEGNALA_KO
                'giu150113 bloccante Controlla_PICF = False
            End If
        End If
    End Function
    'ROUTINES DI CONTROLLO P.I. E C.F.
    Function CF_Controllo(ByVal Cognome, ByVal Nome, ByVal Data_Nascita, ByVal Cod_Catasto, ByVal Sesso, ByVal Cod_Fiscale) As Object
        'Controllo del codice fiscale
        'giu160108 SOLO CONTROLLO FORMALE se i dati anagrafici sono vuoti
        Dim giorno_nascita, mese_nascita, anno_nascita, errore

        If Len(Cod_Fiscale) <> 16 Then
            CF_Controllo = False
            Exit Function
        End If

        'Trasformo il cognome in tutte lettere maiuscole
        Call Dividi_Lettere(Cognome)

        Ind4 = 0
        If Trim(Cognome) <> "" Then 'GIU160108
            If Ind2 = 0 Then
                Codice_Fiscale(1) = Tab_Vocali(1)
                Codice_Fiscale(2) = Tab_Vocali(2)
                Codice_Fiscale(3) = "X"
            Else
                If Ind2 > 2 Then
                    Codice_Fiscale(1) = Tab_Consonanti(1)
                    Codice_Fiscale(2) = Tab_Consonanti(2)
                    Codice_Fiscale(3) = Tab_Consonanti(3)
                Else
                    If Ind2 = 2 Then
                        Codice_Fiscale(1) = Tab_Consonanti(1)
                        Codice_Fiscale(2) = Tab_Consonanti(2)
                        Codice_Fiscale(3) = Tab_Vocali(1)
                    Else
                        Codice_Fiscale(1) = Tab_Consonanti(1)
                        Codice_Fiscale(2) = Tab_Vocali(1)
                        If Ind3 = 1 Then
                            Codice_Fiscale(3) = "X"
                        Else
                            Codice_Fiscale(3) = Tab_Vocali(2)
                        End If
                    End If
                End If
            End If
        Else 'metto quello che hanno digitato
            Codice_Fiscale(1) = Mid(Cod_Fiscale, 1, 1)
            Codice_Fiscale(2) = Mid(Cod_Fiscale, 2, 1)
            Codice_Fiscale(3) = Mid(Cod_Fiscale, 3, 1)
        End If
        Ind4 = 3
        'Trasformo il nome in tutte lettere maiuscole
        ReDim Tab_Vocali(0)
        ReDim Tab_Consonanti(0)
        Call Dividi_Lettere(Nome)
        If Trim(Cognome) <> "" Then 'GIU160108
            If Ind2 = 0 Then
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = Tab_Vocali(1)
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = Tab_Vocali(2)
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = "X"
            Else
                If Ind2 > 3 Then
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(3)
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(4)
                Else
                    If Ind2 > 2 Then
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(2)
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(3)
                    Else
                        If Ind2 = 2 Then
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(2)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Vocali(1)
                        Else
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Vocali(1)
                            If Ind3 = 1 Then
                                Ind4 = Ind4 + 1
                                Codice_Fiscale(Ind4) = "X"
                            Else
                                Ind4 = Ind4 + 1
                                Codice_Fiscale(Ind4) = Tab_Vocali(2)
                            End If
                        End If
                    End If
                End If
            End If
        Else 'metto quello che hanno digitato
            Codice_Fiscale(4) = Mid(Cod_Fiscale, 4, 1)
            Codice_Fiscale(5) = Mid(Cod_Fiscale, 5, 1)
            Codice_Fiscale(6) = Mid(Cod_Fiscale, 6, 1)
        End If
        'Anno di nascita
        Ind4 = Ind4 + 1
        Ind4 = 7 'giu160108
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 7, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        anno_nascita = Codice_Fiscale(Ind4)
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 8, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        anno_nascita = anno_nascita + Codice_Fiscale(Ind4)
        anno_nascita = Val(anno_nascita)

        'Mese di nascita
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 9, 1)
        errore = True
        Select Case Codice_Fiscale(Ind4)
            Case "A"
                errore = False
                mese_nascita = 1
            Case "B"
                errore = False
                mese_nascita = 2
            Case "C"
                errore = False
                mese_nascita = 3
            Case "D"
                errore = False
                mese_nascita = 4
            Case "E"
                errore = False
                mese_nascita = 5
            Case "H"
                errore = False
                mese_nascita = 6
            Case "L"
                errore = False
                mese_nascita = 7
            Case "M"
                errore = False
                mese_nascita = 8
            Case "P"
                errore = False
                mese_nascita = 9
            Case "R"
                errore = False
                mese_nascita = 10
            Case "S"
                errore = False
                mese_nascita = 11
            Case "T"
                errore = False
                mese_nascita = 12
        End Select
        If errore Then
            CF_Controllo = False
            Exit Function
        End If

        'giorno di nascita
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 10, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 11, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        '---
        giorno_nascita = Codice_Fiscale(10) + Codice_Fiscale(11)
        giorno_nascita = Val(giorno_nascita)
        If giorno_nascita > 40 Then
            giorno_nascita = giorno_nascita - 40
            Sesso = "F"
        Else
            Sesso = "M"
        End If
        If giorno_nascita < 1 Or giorno_nascita > 31 Then
            CF_Controllo = False
            Exit Function
        End If

        'Codice catasto
        If Trim(Cod_Catasto) <> "" Then 'GIU160108
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 1, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 2, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 3, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 4, 1)
        Else
            Ind4 = 12
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
        End If
        'Codice di controllo
        If Not Codice_Controllo() Then
            CF_Controllo = False
            Exit Function
        End If

        'Fine creazione porta la tabella a stringa
        Cod_Fisc = ""
        For Ind1 = 1 To Ind4
            Cod_Fisc = Cod_Fisc + Codice_Fiscale(Ind1)
        Next Ind1

        If Cod_Fisc <> Cod_Fiscale Then
            CF_Controllo = False
        Else
            CF_Controllo = True
        End If

        Data_Nascita = LTrim(str(giorno_nascita)) + "/" + LTrim(str(mese_nascita)) + "/" + LTrim(str(anno_nascita))
        Data_Nascita = Format(Data_Nascita, FormatoData)

    End Function
    Sub Dividi_Lettere(ByRef Stringa As String)
        Dim Testo As String
        Testo = Stringa

        Testo = UCase(Testo)
        Ind2 = 0
        Ind3 = 0
        For Ind1 = 1 To Len(Testo)
            'elimino i caratteri che non mi interessano
            If Asc(Mid(Testo, Ind1, 1)) < 64 Or Asc(Mid(Testo, Ind1, 1)) > 91 Then
                car = " "
            Else
                car = Mid(Testo, Ind1, 1)
            End If
            'divido le vocali dalle consonanti e le metto in
            'tabelle separate
            If car = "A" Or car = "E" Or car = "I" Or car = "O" Or car = "U" Then
                Ind3 = Ind3 + 1
                ReDim Preserve Tab_Vocali(Ind3 + 1)
                Tab_Vocali(Ind3) = car
            Else
                If car <> " " Then
                    Ind2 = Ind2 + 1
                    ReDim Preserve Tab_Consonanti(Ind2 + 1)
                    Tab_Consonanti(Ind2) = car
                End If
            End If
        Next Ind1
        'ind2 contiene il numero di consonanti
        'ind3 contiene il numero di vocali
    End Sub
    Function Codice_Controllo() As Object

        Dim Pari, Ind, Totale, resto

        Pari = 0
        Call Controllo_Pari(Pari)
        Call Controllo_Dispari(Pari)

        Totale = Int(Pari / 26)
        resto = Pari - (Totale * 26)

        Ind = 0
        Ind4 = Ind4 + 1
        Do
            If resto = Ind Then
                Codice_Fiscale(Ind4) = Chr(65 + Ind)
                Exit Do
            End If
            Ind = Ind + 1
        Loop Until Ind > 25

        'se il resto supera 25 c'è un errore
        If Ind > 25 Then
            Codice_Controllo = False
        Else
            Codice_Controllo = True
        End If

    End Function
    Sub Controllo_Pari(ByRef Pari As Integer)

        Dim Ind
        Ind = 0
        Do
            Ind = Ind + 2
            If Ind > 14 Then Exit Do

            Select Case Codice_Fiscale(Ind)
                Case "A", "0"
                    Pari = Pari + 0
                Case "B", "1"
                    Pari = Pari + 1
                Case "C", "2"
                    Pari = Pari + 2
                Case "D", "3"
                    Pari = Pari + 3
                Case "E", "4"
                    Pari = Pari + 4
                Case "F", "5"
                    Pari = Pari + 5
                Case "G", "6"
                    Pari = Pari + 6
                Case "H", "7"
                    Pari = Pari + 7
                Case "I", "8"
                    Pari = Pari + 8
                Case "J", "9"
                    Pari = Pari + 9
                Case "K"
                    Pari = Pari + 10
                Case "L"
                    Pari = Pari + 11
                Case "M"
                    Pari = Pari + 12
                Case "N"
                    Pari = Pari + 13
                Case "O"
                    Pari = Pari + 14
                Case "P"
                    Pari = Pari + 15
                Case "Q"
                    Pari = Pari + 16
                Case "R"
                    Pari = Pari + 17
                Case "S"
                    Pari = Pari + 18
                Case "T"
                    Pari = Pari + 19
                Case "U"
                    Pari = Pari + 20
                Case "V"
                    Pari = Pari + 21
                Case "W"
                    Pari = Pari + 22
                Case "X"
                    Pari = Pari + 23
                Case "Y"
                    Pari = Pari + 24
                Case "Z"
                    Pari = Pari + 25
            End Select
        Loop
    End Sub
    Sub Controllo_Dispari(ByRef Dispari As Integer)

        Dim Ind
        Ind = 1
        Do
            Select Case Codice_Fiscale(Ind)
                Case "A", "0"
                    Dispari = Dispari + 1
                Case "B", "1"
                    Dispari = Dispari + 0
                Case "C", "2"
                    Dispari = Dispari + 5
                Case "D", "3"
                    Dispari = Dispari + 7
                Case "E", "4"
                    Dispari = Dispari + 9
                Case "F", "5"
                    Dispari = Dispari + 13
                Case "G", "6"
                    Dispari = Dispari + 15
                Case "H", "7"
                    Dispari = Dispari + 17
                Case "I", "8"
                    Dispari = Dispari + 19
                Case "J", "9"
                    Dispari = Dispari + 21
                Case "K"
                    Dispari = Dispari + 2
                Case "L"
                    Dispari = Dispari + 4
                Case "M"
                    Dispari = Dispari + 18
                Case "N"
                    Dispari = Dispari + 20
                Case "O"
                    Dispari = Dispari + 11
                Case "P"
                    Dispari = Dispari + 3
                Case "Q"
                    Dispari = Dispari + 6
                Case "R"
                    Dispari = Dispari + 8
                Case "S"
                    Dispari = Dispari + 12
                Case "T"
                    Dispari = Dispari + 14
                Case "U"
                    Dispari = Dispari + 16
                Case "V"
                    Dispari = Dispari + 10
                Case "W"
                    Dispari = Dispari + 22
                Case "X"
                    Dispari = Dispari + 25
                Case "Y"
                    Dispari = Dispari + 24
                Case "Z"
                    Dispari = Dispari + 23
            End Select

            Ind = Ind + 2

        Loop Until Ind > 15
    End Sub
    Private Function ControlloDoppio(ByVal Stringa As String, ByVal Tipo As String) As Boolean
        Dim SQLCmd As New SqlCommand
        Dim SQLAdap As New SqlDataAdapter
        Dim ds As New dsClienti
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Try
            SQLCmd.Connection = New SqlConnection
            SQLCmd.Connection.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SQLCmd.CommandType = CommandType.Text
            If Tipo = "CF" Then
                If Session(CSTTABCLIFOR) = "For" Then
                    SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA FROM Fornitori WHERE Codice_Fiscale = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(IDANAGRCLIFOR).ToString.Trim & "'"
                Else
                    SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA FROM Clienti WHERE Codice_Fiscale = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(IDANAGRCLIFOR).ToString.Trim & "'"
                End If
            ElseIf Tipo = "PIVA" Then
                If Session(CSTTABCLIFOR) = "For" Then
                    SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA FROM Fornitori WHERE Partita_IVA = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(IDANAGRCLIFOR).ToString.Trim & "'"
                Else
                    SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA FROM Clienti WHERE Partita_IVA = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(IDANAGRCLIFOR).ToString.Trim & "'"
                End If
            End If
            '' ' niente stampa SQLCmd.CommandText = SQLCmd.CommandText & " UNION SELECT '" & txtCodCliente.Text.Trim & "' AS Codice_CoGe, '" & txtRagSoc.Text.Trim & "' AS Rag_Soc, '" & txtCodFiscale.Text.Trim & "' AS Codice_Fiscale, '" & txtPartitaIVA.Text.Trim & "' AS Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Clienti"

            SQLAdap.SelectCommand = SQLCmd

            SQLAdap.Fill(ds.Doppi)

            If ds.Doppi.Rows.Count > 0 Then
                ControlloDoppio = True
                '' ' niente stampa Session("dsStampa") = ds
            Else
                ControlloDoppio = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            If Tipo = "CF" Then
                ModalPopup.Show("Attenzione", "Errore durante il controllo sul codice fiscale duplicato. È possibile che sia già presente un cliente con questo codice fiscale.", WUC_ModalPopup.TYPE_ERROR)
            ElseIf Tipo = "PIVA" Then
                ModalPopup.Show("Attenzione", "Errore durante il controllo sulla partita IVA duplicata. È possibile che sia già presente un cliente con questa partita IVA.", WUC_ModalPopup.TYPE_ERROR)
            End If
            ControlloDoppio = False
        End Try
    End Function
    Function PI_controllo(ByVal Partita_Iva As String) As Integer
        ' 0 = corretta
        ' 1 = Lunghezza errata
        ' 2 = Presenza di caratteri alfabetici
        ' 3 = Le prime 7 cifre sono uguali a zero
        ' 4 = Ufficio IVA errato o mancante in tabella
        ' 5 = Codice di controllo errato
        'Controllo della partita iva
        Dim Tab_Iva(12) As String
        Dim Ind As Integer
        Dim Tot_Iva As Long
        Dim Per_Due As Long
        Dim Tot_Iva_Ult As String
        Dim Contr As Object

        PI_controllo = 0

        If Len(Partita_Iva) <> 11 Then
            PI_controllo = 1
            Exit Function
        End If

        If Not IsNumeric(Partita_Iva) Then
            PI_controllo = 2
            Exit Function
        End If

        If Left(Partita_Iva, 7) = "0000000" Then
            PI_controllo = 3
            Exit Function
        End If

        For Ind = 1 To 11
            Tab_Iva(Ind) = Mid(Partita_Iva, Ind, 1)
        Next Ind

        Tot_Iva = 0

        Per_Due = 2 * Tab_Iva(2)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(4)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(6)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(8)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(10)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Tot_Iva = Tot_Iva + Val(Tab_Iva(1))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(3))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(5))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(7))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(9))

        Tot_Iva_Ult = Mid(LTrim(Str(Tot_Iva)), Len(LTrim(Str(Tot_Iva))), 1)
        If Tot_Iva_Ult = "0" Then
            Contr = Tot_Iva_Ult
        Else
            Contr = 10 - Val(Tot_Iva_Ult)
        End If
        Contr = Right(LTrim(Str(Contr)), 1)

        If Contr <> Tab_Iva(11) Then
            PI_controllo = 5
            Exit Function
        End If

        PI_controllo = 0
    End Function
    Private Sub txtPartitaIVA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPartitaIVA.TextChanged
        If Session("AvvisaDoppio") = False Then
            Session("AvvisaDoppio") = True
        End If
        txtCodiceFiscale.Focus()
    End Sub
    Private Sub txtCodiceFiscale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodiceFiscale.TextChanged
        If Session("AvvisaDoppio") = False Then
            Session("AvvisaDoppio") = True
        End If
        txtTelefono1.Focus()
    End Sub

    Private Sub TxtStato_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtStato.TextChanged
        If TxtStato.Text.Trim <> "I" And TxtStato.Text.Trim <> "IT" And TxtStato.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
        txtPartitaIVA.Focus()
    End Sub
End Class