Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_AnagrProvv_Insert
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

    Public rk As StrAnagrProvv

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Session(F_ANAGR_PROVV_APERTA) = True Then
                SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE Tipo<>'A' ORDER BY [Ragione_Sociale]"
            Else
                SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
            End If
        Catch ex As Exception
            SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
        End Try
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSAnagrProvv.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If Session(TIPORK) = "C" Then
            lblLabelTipoRK.Text = "Gestione anagrafiche provvisorie Clienti"
        ElseIf Session(TIPORK) = "F" Then
            lblLabelTipoRK.Text = "Gestione anagrafiche provvisorie Fornitori"
        Else
            If Session(CSTTABCLIFOR) = "Cli" Then
                Session(TIPORK) = "C"
                lblLabelTipoRK.Text = "Gestione anagrafiche provvisorie Clienti"
            ElseIf Session(CSTTABCLIFOR) = "For" Then
                Session(TIPORK) = "F"
                lblLabelTipoRK.Text = "Gestione anagrafiche provvisorie Fornitori"
            Else 'sessione scaduta??
                lblLabelTipoRK.Text = "Gestione anagrafiche provvisorie"
            End If
        End If
        If (Not IsPostBack) Then
            ddlAnagrProvv.Items.Clear()
            ddlAnagrProvv.Items.Add("")
            EnabledCampi(True)
            Session(IDANAGRREALESN) = False 'Codice anagrafica provvisoria
        End If
        'Simone310317
        _WucElement.SetLblMessUtente("")
    End Sub
    Public Function PopolaDLL(ByRef strMessErr As String) As Boolean
        PopolaDLL = True
        Try
            Try
                If Session(F_ANAGR_PROVV_APERTA) = True Then
                    SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE Tipo<>'A' ORDER BY [Ragione_Sociale]"
                    EnabledCampi(True)
                    Session(IDANAGRREALESN) = False 'Codice anagrafica provvisoria
                Else
                    SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
                End If
            Catch ex As Exception
                SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
            End Try
            ddlAnagrProvv.Items.Clear()
            ddlAnagrProvv.Items.Add("")
            ddlAnagrProvv.DataBind()
        Catch ex As Exception
            PopolaDLL = False
            strMessErr = "Errore caricamento Elenco Anagrafiche provvisorie: " & ex.Message
            Exit Function
        End Try
    End Function

    Public Sub SvuotaCampi()
        Session(IDANAGRPROVV) = ""
        ddlAnagrProvv.AutoPostBack = False
        ddlAnagrProvv.SelectedIndex = 0
        ddlAnagrProvv.AutoPostBack = True
        txtRagioneSoc.AutoPostBack = False
        txtRagioneSoc.Text = "" : txtRagioneSoc.BackColor = SEGNALA_OK
        txtRagioneSoc.AutoPostBack = True
        TxtIndirizzo.Text = ""
        TxtCAP.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        TxtStato.Text = "" : TxtStato.BackColor = SEGNALA_OK
        txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
        txtCodiceFiscale.Text = ""
        txtPartitaIVA.Text = ""
        txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
        rk = Nothing
        Session(RKANAGRPROVV) = rk
        'Simone310317
        _WucElement.SetLblMessUtente("")
        EnabledCampi(True)
        Session(IDANAGRREALESN) = False
    End Sub
    'GIU230819 ABILITO/DISABILITO CAMPI IN FUNZIONE DEI DATI PRESI DA ANAGRPROVV/CLIENTI
    Public Sub EnabledCampi(ByVal valore As Boolean)
        txtRagioneSoc.Enabled = valore
        TxtIndirizzo.Enabled = valore
        TxtCAP.Enabled = valore
        TxtLocalita.Enabled = valore
        TxtProvincia.Enabled = valore
        TxtStato.Enabled = valore
        txtCodiceFiscale.Enabled = valore
        txtPartitaIVA.Enabled = valore
    End Sub
    'GIU150513 PER LA MODIFICA
    Public Sub PopolaCampiModifica()
        Session(IDANAGRREALESN) = False 'giu201119
        'giu100118
        Dim myIDAnagrProvv As String = Session(IDANAGRPROVV)
        If IsNothing(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        If String.IsNullOrEmpty(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        Try
            Try
                If IsNumeric(myIDAnagrProvv) Then
                    SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=" & Trim(myIDAnagrProvv)
                ElseIf Session(F_ANAGR_PROVV_APERTA) = True Then
                    SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE Tipo<>'A' ORDER BY [Ragione_Sociale]"
                Else
                    SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
                End If
            Catch ex As Exception
                SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
            End Try
            ddlAnagrProvv.Items.Clear()
            ddlAnagrProvv.Items.Add("")
            ddlAnagrProvv.DataBind()
        Catch ex As Exception
            SqlDSAnagrProvv.SelectCommand = "SELECT * FROM [AnagrProvv] WHERE IDAnagrProvv=-1"
        End Try
        '---------------
        Dim dvAnagrProvv As DataView
        dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
        If (dvAnagrProvv Is Nothing) Then
            SvuotaCampi()
            Session(IDANAGRPROVV) = ""
            Exit Sub
        End If
        If dvAnagrProvv.Count > 0 Then
            dvAnagrProvv.RowFilter = "IDAnagrProvv = " & IIf(IsNumeric(myIDAnagrProvv), Trim(Session(IDANAGRPROVV)), "-1")
            If dvAnagrProvv.Count > 0 Then
                Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                ddlAnagrProvv.AutoPostBack = False
                ddlAnagrProvv.SelectedIndex = 1
                ddlAnagrProvv.AutoPostBack = True
                txtRagioneSoc.AutoPostBack = False
                txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                txtRagioneSoc.AutoPostBack = True
                TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                Call PopolaEntityDatiAnagrProvv()
                EnabledCampi(True)
            Else
                SvuotaCampi()
                Session(IDANAGRPROVV) = ""
                Exit Sub
            End If
        Else
            SvuotaCampi()
            Session(IDANAGRPROVV) = ""
            Exit Sub
        End If
    End Sub
    '-------------------------

    Public Function PopolaEntityDatiAnagrProvv() As Boolean
        PopolaEntityDatiAnagrProvv = True

        'giu220819
        Dim strMess As String = ""
        If txtRagioneSoc.Text.Trim <> "" Then
            Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim f = From x In listaClienti Where x.Rag_Soc.ToString.Trim.ToUpper().Equals(txtRagioneSoc.Text.Trim.ToUpper())

            If Not (f Is Nothing) Then
                Dim a = 0
                For Each El In f
                    a += 1
                Next

                If a > 0 Then
                    If strMess.Trim <> "" Then
                        strMess += "<br>"
                    End If
                    strMess += "Anagrafica già presente in anagrafica reale.!"
                End If
            End If
        End If
        '-
        If txtPartitaIVA.Text.Trim <> "" Then
            Dim arrTabella As ArrayList = Nothing
            arrTabella = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim res = From x In arrTabella Where x.Partita_Iva.ToString.ToUpper.Equals(txtPartitaIVA.Text.Trim.ToUpper())

            If (Not res Is Nothing) Then
                Dim a = 0
                For Each El In res
                    a += 1
                Next

                If a > 0 Then
                    If strMess.Trim <> "" Then
                        strMess += "<br>"
                    End If
                    strMess += "Partita IVA presente in anagrafica reale! Click sul bottone se si vuol caricare i dati associati."
                End If
            End If
        End If
        '-
        If txtCodiceFiscale.Text.Trim <> "" Then
            Dim arrTabella As ArrayList = Nothing
            arrTabella = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim res = From x In arrTabella Where x.Codice_Fiscale.ToString.Trim.ToUpper().Equals(txtCodiceFiscale.Text.Trim.ToUpper())

            If (Not res Is Nothing) Then
                Dim a = 0
                For Each El In res
                    a += 1
                Next

                If a > 0 Then
                    If strMess.Trim <> "" Then
                        strMess += "<br>"
                    End If
                    strMess += "Codice fiscale presente in anagrafica reale! Click sul bottone se si vuol caricare i dati associati."
                End If
            End If
        End If
        '---------
        If strMess.Trim <> "" Then
            _WucElement.SetLblMessUtente(strMess.Trim)
        End If
        '---------
        rk.IDAnagrProvv = Session(IDANAGRPROVV) 'GIU221119
        If txtRagioneSoc.Text.Trim = "" Then
            txtRagioneSoc.BackColor = SEGNALA_KO : PopolaEntityDatiAnagrProvv = False
        Else
            txtRagioneSoc.BackColor = SEGNALA_OK
        End If
        rk.Ragione_Sociale = Mid(txtRagioneSoc.Text.Trim, 1, 50)
        rk.Indirizzo = Mid(TxtIndirizzo.Text.Trim, 1, 50)
        rk.Cap = Mid(TxtCAP.Text.Trim, 1, 5)
        rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        rk.Stato = Mid(TxtStato.Text.Trim, 1, 3)
        rk.Codice_Fiscale = Mid(txtCodiceFiscale.Text.Trim, 1, 16)
        rk.Partita_IVA = Mid(txtPartitaIVA.Text.Trim, 1, 12)
        rk.Tipo = Session(TIPORK)
        Session(RKANAGRPROVV) = rk
        ddlAnagrProvv.SelectedIndex = -1
    End Function

    Public Function AggiornaAnagrProvv() As Boolean
        AggiornaAnagrProvv = True

        If txtRagioneSoc.Text.Trim = "" Then
            txtRagioneSoc.BackColor = SEGNALA_KO : AggiornaAnagrProvv = False
        Else
            txtRagioneSoc.BackColor = SEGNALA_OK
        End If
        If TxtProvincia.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
                TxtProvincia.BackColor = SEGNALA_KO
                AggiornaAnagrProvv = False
            Else
                TxtProvincia.BackColor = SEGNALA_OK
            End If
        Else
            TxtProvincia.BackColor = SEGNALA_OK
        End If
        If TxtStato.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtStato.Text, Def.NAZIONI, Session(ESERCIZIO)) = "" Then
                TxtStato.BackColor = SEGNALA_KO
                AggiornaAnagrProvv = False
            Else
                TxtStato.BackColor = SEGNALA_OK
            End If
        Else
            TxtStato.BackColor = SEGNALA_OK
            TxtStato.Text = "I" 'giu150513 obb. quando stampo il documento
        End If
        '---------
        If AggiornaAnagrProvv = False Then Exit Function

        Try
            PopolaEntityDatiAnagrProvv()
            Dim myIDAnagrProvv As String = Session(IDANAGRPROVV)
            If IsNothing(myIDAnagrProvv) Then
                myIDAnagrProvv = ""
            End If
            If String.IsNullOrEmpty(myIDAnagrProvv) Then
                myIDAnagrProvv = ""
            End If
            If IsNumeric(myIDAnagrProvv) Then
                If CLng(myIDAnagrProvv) > 0 Then
                    'OK
                Else
                    myIDAnagrProvv = ""
                End If
            Else
                myIDAnagrProvv = ""
            End If
            If Session(IDANAGRREALESN) = False Then 'Simone040417: Se non esiste l'anagrafica reale aggiunge l'anagrafica provvisoria in tabella
                SqlDSAnagrProvv.UpdateParameters.Item("Ragione_Sociale").DefaultValue = Mid(txtRagioneSoc.Text.Trim, 1, 50)
                SqlDSAnagrProvv.UpdateParameters.Item("Indirizzo").DefaultValue = Mid(TxtIndirizzo.Text.Trim, 1, 50)
                SqlDSAnagrProvv.UpdateParameters.Item("Cap").DefaultValue = Mid(TxtCAP.Text.Trim, 1, 5)
                SqlDSAnagrProvv.UpdateParameters.Item("Localita").DefaultValue = Mid(TxtLocalita.Text.Trim, 1, 50)
                SqlDSAnagrProvv.UpdateParameters.Item("Provincia").DefaultValue = Mid(TxtProvincia.Text.Trim, 1, 2)
                SqlDSAnagrProvv.UpdateParameters.Item("Stato").DefaultValue = Mid(TxtStato.Text.Trim, 1, 3)
                SqlDSAnagrProvv.UpdateParameters.Item("Codice_Fiscale").DefaultValue = Mid(txtCodiceFiscale.Text.Trim, 1, 16)
                SqlDSAnagrProvv.UpdateParameters.Item("Partita_IVA").DefaultValue = Mid(txtPartitaIVA.Text.Trim, 1, 12)
                SqlDSAnagrProvv.UpdateParameters.Item("Tipo").DefaultValue = Session(TIPORK) 'giu121011Mid(Session(CSTTABCLIFOR), 1, 1)
                SqlDSAnagrProvv.Update()
                SqlDSAnagrProvv.DataBind()
                'giu221119 errore non riporta il nuovo codice (se nuovo)
                Dim dvAnagrProvv As DataView
                dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
                If (dvAnagrProvv Is Nothing) Then
                    _WucElement.SetLblMessUtente("Nessuna Anagrafica provvisoria inserita.")
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Nessuna Anagrafica provvisoria inserita.", WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Ragione_Sociale = '" & Controlla_Apice(txtRagioneSoc.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                        myIDAnagrProvv = Session(IDANAGRPROVV)
                        rk.IDAnagrProvv = myIDAnagrProvv
                        rk.Ragione_Sociale = Mid(txtRagioneSoc.Text.Trim, 1, 50)
                        rk.Indirizzo = Mid(TxtIndirizzo.Text.Trim, 1, 50)
                        rk.Cap = Mid(TxtCAP.Text.Trim, 1, 5)
                        rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
                        rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
                        rk.Stato = Mid(TxtStato.Text.Trim, 1, 3)
                        rk.Codice_Fiscale = Mid(txtCodiceFiscale.Text.Trim, 1, 16)
                        rk.Partita_IVA = Mid(txtPartitaIVA.Text.Trim, 1, 12)
                        rk.Tipo = Session(TIPORK)
                        Session(RKANAGRPROVV) = rk
                    Else
                        _WucElement.SetLblMessUtente("Nessuna Anagrafica provvisoria inserita.")
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Nessuna Anagrafica provvisoria inserita.", WUC_ModalPopup.TYPE_ERROR)
                        Return False
                    End If
                Else
                    _WucElement.SetLblMessUtente("Nessuna Anagrafica provvisoria inserita.")
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Nessuna Anagrafica provvisoria inserita.", WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
                '---------
                If IsNumeric(myIDAnagrProvv) Then 'GIU220819
                    'GIU140417
                    'giu180912 giu101012 aggiorna MySQL Clienti per IREDEEM
                    Dim errorMsg As String = ""
                    If App.GetDatiAbilitazioni(CSTABILCOGE, "MySQLCli" + Session(ESERCIZIO).ToString.Trim, "", errorMsg) = True Then
                        'ok chiamo la SP per aggiornare ma senza TRANSAZIONE
                        'Session(COD_CLIENTE)
                        errorMsg = ""
                        If DataBaseUtility.MySQLClienti(myIDAnagrProvv.Trim, CSTPotenziale, errorMsg, True) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Aggiorna dati cliente (MySQL)", _
                                String.Format("Errore, contattare l'amministratore di sistema. Errore: {0}", errorMsg), _
                                WUC_ModalPopup.TYPE_ERROR)
                        End If
                    End If
                    '--------------------------------------------
                End If
            End If

        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function
    Private Sub CheckInserimentoCampoObbligatorio(ByRef txt As TextBox)
        If (String.IsNullOrEmpty(txt.Text)) Then
            txt.BackColor = Def.SEGNALA_KO
        Else
            txt.BackColor = Def.SEGNALA_OK
        End If
    End Sub

    Private Sub ddlAnagrProvv_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAnagrProvv.SelectedIndexChanged
        Session(IDANAGRREALESN) = False 'giu201119
        Session(IDANAGRPROVV) = IIf(IsNumeric(ddlAnagrProvv.SelectedValue), ddlAnagrProvv.SelectedValue, "") 'GIU201119
        If ddlAnagrProvv.SelectedIndex = 0 Then Session(IDANAGRPROVV) = ""
        Dim dvAnagrProvv As DataView
        dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
        If (dvAnagrProvv Is Nothing) Then
            SvuotaCampi()
            Session(IDANAGRPROVV) = ""
            Exit Sub
        End If
        Dim myIDAnagrProvv As String = Session(IDANAGRPROVV)
        If IsNothing(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        'giu201119
        If String.IsNullOrEmpty(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        If IsNumeric(myIDAnagrProvv) Then
            If CLng(myIDAnagrProvv) > 0 Then
                'OK
            Else
                myIDAnagrProvv = ""
            End If
        Else
            myIDAnagrProvv = ""
        End If
        '---------
        If dvAnagrProvv.Count > 0 Then
            dvAnagrProvv.RowFilter = "IDAnagrProvv = " & IIf(myIDAnagrProvv = "", "-1", myIDAnagrProvv.Trim) 'giu201119 Session(IDANAGRPROVV)
            If dvAnagrProvv.Count > 0 Then
                txtRagioneSoc.AutoPostBack = False
                txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                txtRagioneSoc.AutoPostBack = True
                TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                PopolaEntityDatiAnagrProvv()
                EnabledCampi(True)
            Else
                SvuotaCampi()
                Session(IDANAGRPROVV) = ""
                Exit Sub
            End If
        Else
            SvuotaCampi()
            Session(IDANAGRPROVV) = ""
        End If
    End Sub

    Private Sub txtRagioneSoc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRagioneSoc.TextChanged
        Session(IDANAGRREALESN) = False 'giu201119
        'GIU210819
        If txtRagioneSoc.Text.Trim <> "" Then
            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If (dvAnagrProvv Is Nothing) Then
                _WucElement.SetLblMessUtente("Nuova Anagrafica provvisoria.")
                EnabledCampi(True)
                TxtIndirizzo.Focus()
                Exit Sub
            End If
            If dvAnagrProvv.Count > 0 Then
                dvAnagrProvv.RowFilter = "Ragione_Sociale = '" & Controlla_Apice(txtRagioneSoc.Text.Trim) & "'"
                If dvAnagrProvv.Count > 0 Then
                    Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                    txtRagioneSoc.AutoPostBack = False
                    txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                    txtRagioneSoc.AutoPostBack = True
                    ddlAnagrProvv.AutoPostBack = False
                    If ddlAnagrProvv.Items.Count > 0 Then
                        ddlAnagrProvv.SelectedIndex = 1
                    End If
                    ddlAnagrProvv.AutoPostBack = True
                    TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                    TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                    TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                    TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                    TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                    txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                    txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                    txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                    txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                    _WucElement.SetLblMessUtente("Anagrafica provvisoria già presente. Per inserire una nuova modificare la Ragione sociale")
                    EnabledCampi(True)
                Else
                    _WucElement.SetLblMessUtente("Nuova Anagrafica provvisoria.")
                    EnabledCampi(True)
                End If
            Else
                _WucElement.SetLblMessUtente("Nuova Anagrafica provvisoria.")
                EnabledCampi(True)
            End If
            PopolaEntityDatiAnagrProvv()
        End If
        TxtIndirizzo.Focus()
    End Sub

    Public Sub PopolaPerRag()
        Session(IDANAGRREALESN) = False 'giu201119
        If txtRagioneSoc.Text.Trim <> "" Then
            Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim myClienti As ClientiEntity
            Dim f = From x In listaClienti Where x.Rag_Soc.ToString.Trim.ToUpper().Equals(txtRagioneSoc.Text.Trim.ToUpper())

            If Not (f Is Nothing) Then
                Dim a = 0
                For Each El In f
                    a += 1
                Next

                If a > 0 Then
                    myClienti = f(0)
                    Session(IDANAGRPROVV) = myClienti.Codice_CoGe
                    txtRagioneSoc.AutoPostBack = False
                    txtRagioneSoc.Text = myClienti.Rag_Soc
                    txtRagioneSoc.AutoPostBack = True
                    TxtIndirizzo.Text = myClienti.Indirizzo
                    TxtCAP.Text = myClienti.CAP
                    TxtLocalita.Text = myClienti.Localita
                    TxtProvincia.Text = myClienti.Provincia
                    TxtStato.Text = myClienti.Nazione
                    txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                    txtCodiceFiscale.Text = myClienti.Codice_Fiscale
                    txtPartitaIVA.Text = myClienti.Partita_IVA
                    txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                    _WucElement.SetLblMessUtente("Dati associati per partita Ragione Sociale da anagrafica reale. Click su seleziona e aggiorna per confermare.")
                    TxtIndirizzo.Focus()
                    Session(IDANAGRREALESN) = True 'Codice anagrafica reale
                    EnabledCampi(False)
                    Exit Sub
                End If
            End If

            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If Not (dvAnagrProvv Is Nothing) Then
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Ragione_Sociale = '" & Controlla_Apice(txtRagioneSoc.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                        txtRagioneSoc.AutoPostBack = False
                        txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                        txtRagioneSoc.AutoPostBack = True
                        TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                        TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                        TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                        TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                        TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                        txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                        txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                        txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                        txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                        _WucElement.SetLblMessUtente("Dati associati per partita Ragione Sociale da anagrafica provvisoria. Click su seleziona e aggiorna per confermare.")
                        Session(IDANAGRREALESN) = False 'Codice anagrafica provvisoria
                        EnabledCampi(True)
                    End If
                End If
            End If
        End If
        TxtIndirizzo.Focus()
    End Sub

    'Simone 300317
    Private Sub txtPartitaIVA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPartitaIVA.TextChanged
        If txtPartitaIVA.Text.Trim <> "" Then
            Dim arrTabella As ArrayList = Nothing
            arrTabella = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim res = From x In arrTabella Where x.Partita_Iva.ToString.ToUpper.Equals(txtPartitaIVA.Text.Trim.ToUpper())

            If (Not res Is Nothing) Then
                Dim a = 0
                For Each El In res
                    a += 1
                Next

                If a > 0 Then
                    _WucElement.SetLblMessUtente("Partita IVA presente in anagrafica reale! Click sul bottone se si vuol caricare i dati associati.")
                    txtCodiceFiscale.Focus()
                    Exit Sub
                End If
            End If

            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If Not (dvAnagrProvv Is Nothing) Then
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Partita_IVA = '" & Controlla_Apice(txtPartitaIVA.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        _WucElement.SetLblMessUtente("Partita IVA presente in anagrafica provvisoria! Click sul bottone se si vuol caricare i dati associati.")
                    End If
                End If
            End If
        End If
        txtCodiceFiscale.Focus()
    End Sub

    Public Sub PopolaPerPIVA()
        Session(IDANAGRREALESN) = False 'giu201119
        If txtPartitaIVA.Text.Trim <> "" Then
            Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim myClienti As ClientiEntity
            Dim f = From x In listaClienti Where x.Partita_Iva.ToString.Trim.ToUpper().Equals(txtPartitaIVA.Text.Trim.ToUpper())

            If Not (f Is Nothing) Then
                Dim a = 0
                For Each El In f
                    a += 1
                Next

                If a > 0 Then
                    myClienti = f(0)
                    Session(IDANAGRPROVV) = myClienti.Codice_CoGe
                    txtRagioneSoc.AutoPostBack = False
                    txtRagioneSoc.Text = myClienti.Rag_Soc
                    txtRagioneSoc.AutoPostBack = True
                    TxtIndirizzo.Text = myClienti.Indirizzo
                    TxtCAP.Text = myClienti.CAP
                    TxtLocalita.Text = myClienti.Localita
                    TxtProvincia.Text = myClienti.Provincia
                    TxtStato.Text = myClienti.Nazione
                    txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                    txtCodiceFiscale.Text = myClienti.Codice_Fiscale
                    txtPartitaIVA.Text = myClienti.Partita_IVA
                    txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                    _WucElement.SetLblMessUtente("Dati associati per partita IVA da anagrafica reale. Click su seleziona e aggiorna per confermare.")
                    txtCodiceFiscale.Focus()
                    Session(IDANAGRREALESN) = True 'Codice anagrafica reale
                    EnabledCampi(False)
                    Exit Sub
                End If
            End If

            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If Not (dvAnagrProvv Is Nothing) Then
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Partita_IVA = '" & Controlla_Apice(txtPartitaIVA.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                        txtRagioneSoc.AutoPostBack = False
                        txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                        txtRagioneSoc.AutoPostBack = True
                        TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                        TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                        TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                        TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                        TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                        txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                        txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                        txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                        txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                        _WucElement.SetLblMessUtente("Dati associati per partita IVA da anagrafica provvisoria. Click su seleziona e aggiorna per confermare.")
                        Session(IDANAGRREALESN) = False 'Codice anagrafica provvisoria
                        EnabledCampi(True)
                    End If
                End If
            End If
        End If
        txtCodiceFiscale.Focus()
    End Sub

    Private Sub txtCodiceFiscale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodiceFiscale.TextChanged
        If txtCodiceFiscale.Text.Trim <> "" Then
            Dim arrTabella As ArrayList = Nothing
            arrTabella = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim res = From x In arrTabella Where x.Codice_Fiscale.ToString.Trim.ToUpper().Equals(txtCodiceFiscale.Text.Trim.ToUpper())

            If (Not res Is Nothing) Then
                Dim a = 0
                For Each El In res
                    a += 1
                Next

                If a > 0 Then
                    _WucElement.SetLblMessUtente("Codice fiscale presente in anagrafica reale! Click sul bottone se si vuol caricare i dati associati.")
                    TxtIndirizzo.Focus()
                    Exit Sub
                End If
            End If

            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If Not (dvAnagrProvv Is Nothing) Then
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Codice_Fiscale = '" & Controlla_Apice(txtCodiceFiscale.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        _WucElement.SetLblMessUtente("Codice fiscale presente in anagrafica provvisoria! Click sul bottone se si vuol caricare i dati associati.")
                    End If
                End If
            End If
        End If
        TxtIndirizzo.Focus()
    End Sub

    Public Sub PopolaPerCF()
        Session(IDANAGRREALESN) = False 'giu201119
        If txtCodiceFiscale.Text.Trim <> "" Then
            Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
            Dim myClienti As ClientiEntity
            Dim f = From x In listaClienti Where x.Codice_Fiscale.ToString.Trim.ToUpper().Equals(txtCodiceFiscale.Text.Trim.ToUpper())

            If Not (f Is Nothing) Then
                Dim a = 0
                For Each El In f
                    a += 1
                Next

                If a > 0 Then
                    myClienti = f(0)
                    Session(IDANAGRPROVV) = myClienti.Codice_CoGe
                    txtRagioneSoc.AutoPostBack = False
                    txtRagioneSoc.Text = myClienti.Rag_Soc
                    txtRagioneSoc.AutoPostBack = True
                    TxtIndirizzo.Text = myClienti.Indirizzo
                    TxtCAP.Text = myClienti.CAP
                    TxtLocalita.Text = myClienti.Localita
                    TxtProvincia.Text = myClienti.Provincia
                    TxtStato.Text = myClienti.Nazione
                    txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                    txtCodiceFiscale.Text = myClienti.Codice_Fiscale
                    txtPartitaIVA.Text = myClienti.Partita_IVA
                    txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                    _WucElement.SetLblMessUtente("Dati associati per codice fiscale da anagrafica reale. Click su seleziona e aggiorna per confermare.")
                    TxtIndirizzo.Focus()
                    Session(IDANAGRREALESN) = True 'Codice anagrafica reale
                    EnabledCampi(False)
                    Exit Sub
                End If
            End If

            Dim dvAnagrProvv As DataView
            dvAnagrProvv = SqlDSAnagrProvv.Select(DataSourceSelectArguments.Empty)
            If Not (dvAnagrProvv Is Nothing) Then
                If dvAnagrProvv.Count > 0 Then
                    dvAnagrProvv.RowFilter = "Partita_IVA = '" & Controlla_Apice(txtCodiceFiscale.Text.Trim) & "'"
                    If dvAnagrProvv.Count > 0 Then
                        Session(IDANAGRPROVV) = dvAnagrProvv.Item(0).Item("IDAnagrProvv")
                        txtRagioneSoc.AutoPostBack = False
                        txtRagioneSoc.Text = dvAnagrProvv.Item(0).Item("Ragione_Sociale").ToString
                        txtRagioneSoc.AutoPostBack = True
                        TxtIndirizzo.Text = dvAnagrProvv.Item(0).Item("Indirizzo").ToString
                        TxtCAP.Text = dvAnagrProvv.Item(0).Item("Cap").ToString
                        TxtLocalita.Text = dvAnagrProvv.Item(0).Item("Localita").ToString
                        TxtProvincia.Text = dvAnagrProvv.Item(0).Item("Provincia").ToString
                        TxtStato.Text = dvAnagrProvv.Item(0).Item("Stato").ToString
                        txtCodiceFiscale.AutoPostBack = False : txtPartitaIVA.AutoPostBack = False
                        txtCodiceFiscale.Text = dvAnagrProvv.Item(0).Item("Codice_Fiscale").ToString
                        txtPartitaIVA.Text = dvAnagrProvv.Item(0).Item("Partita_IVA").ToString
                        txtCodiceFiscale.AutoPostBack = True : txtPartitaIVA.AutoPostBack = True
                        _WucElement.SetLblMessUtente("Dati associati per codice fiscale da anagrafica provvisoria. Click su seleziona e aggiorna per confermare.")
                        Session(IDANAGRREALESN) = False 'Codice anagrafica provvisoria
                        EnabledCampi(True)
                    End If
                End If
            End If
        End If
        TxtIndirizzo.Focus()
    End Sub
    '---
End Class