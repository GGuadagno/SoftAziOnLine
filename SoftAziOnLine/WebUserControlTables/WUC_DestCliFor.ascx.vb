Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
'giu080113
Imports System.Data.SqlClient

Partial Public Class WUC_DestCliFor
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

    Public Rk As DestCliForEntity

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSAnagrCliFor.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Inserimento/Modifica Destinazioni Clienti"
    End Sub

    Public Sub SvuotaCampi()
        txtTipo.Text = "" : txtTipo.BackColor = SEGNALA_OK
        txtRagioneSoc.Text = "" : txtRagioneSoc.BackColor = SEGNALA_OK
        txtDenominazione.Text = ""
        txtRiferimento.Text = ""
        txtRagSoc35.Text = ""
        txtRiferimento35.Text = ""
        TxtIndirizzo.Text = ""
        TxtCAP.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        TxtStato.Text = "" : TxtStato.BackColor = SEGNALA_OK
        txtTelefono1.Text = ""
        txtTelefono2.Text = ""
        txtFax.Text = ""
        txtEmail.Text = "" : txtEmail.BackColor = SEGNALA_OK
        Rk = Nothing
        Session(RKANAGRDESTCLI) = Rk
    End Sub

    Public Sub NewSvuotaCampi()
        lblCodice.Text = "-1"
        txtTipo.Text = GetNewTipo()
        txtRagioneSoc.BackColor = SEGNALA_OK
        'txtRagioneSoc.Text = "" : txtRagioneSoc.BackColor = SEGNALA_OK
        ' ''txtDenominazione.Text =
        ' ''txtRiferimento.Text = ""
        ' ''TxtIndirizzo.Text = ""
        ' ''TxtCAP.Text = ""
        ' ''TxtLocalita.Text = ""
        ' ''TxtProvincia.Text = ""
        TxtProvincia.BackColor = SEGNALA_OK
        ' ''TxtStato.Text = "" 
        TxtStato.BackColor = SEGNALA_OK
        ' ''txtTelefono1.Text = ""
        ' ''txtTelefono2.Text = ""
        ' ''txtFax.Text = ""
        ' ''txtEmail.Text = ""
        lblMessDett.Text = "INSERIMENTO NUOVA ANAGRAFICA"
    End Sub
    Private Function GetNewTipo() As Long
        GetNewTipo = 0
        Try
            Dim strSQL As String = ""
            strSQL = "Select MAX(CONVERT(INT, Tipo)) AS Numero From DestClienti WHERE Codice = '" & Session(CSTCODCOGE).ToString.Trim & "'"
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                    GetNewTipo = ds.Tables(0).Rows(0).Item("Numero") + 1
                Else
                    GetNewTipo = 1
                End If
            End If
        Catch ex As Exception
            GetNewTipo = 0
            txtTipo.BackColor = SEGNALA_KO
            Exit Function
        End Try
    End Function

    Public Sub PopolaCampi(Optional ByRef pNuovo As String = "")
        lblCodice.Text = Session(IDDESTCLIFOR).ToString.Trim
        If (Session(RKANAGRDESTCLI) Is Nothing) Then
            SvuotaCampi()
            Exit Sub
        End If
        Rk = Session(RKANAGRDESTCLI)
        If (Rk.Ragione_Sociale Is Nothing) Then
            SvuotaCampi()
            Exit Sub
        End If
        txtTipo.Text = Rk.Tipo
        txtRagioneSoc.Text = Rk.Ragione_Sociale : txtRagioneSoc.BackColor = SEGNALA_OK
        txtDenominazione.Text = Rk.Denominazione
        txtRiferimento.Text = Rk.Riferimento
        txtRagSoc35.Text = Rk.Ragione_Sociale35
        txtRiferimento35.Text = Rk.Riferimento35
        TxtIndirizzo.Text = Rk.Indirizzo
        TxtCAP.Text = Rk.Cap
        TxtLocalita.Text = Rk.Localita
        TxtProvincia.Text = Rk.Provincia
        TxtStato.Text = Rk.Stato
        txtTelefono1.Text = Rk.Telefono1
        txtTelefono2.Text = Rk.Telefono2
        txtFax.Text = Rk.Fax
        txtEmail.Text = Rk.EMail
        txtEmail.BackColor = SEGNALA_OK
        If lblCodice.Text.Trim <> "-1" Then
            lblMessDett.Text = "MODIFICA ANAGRAFICA"
            pNuovo = "N"
        Else
            txtTipo.Text = GetNewTipo() 'GIU020523
            lblMessDett.Text = "INSERIMENTO NUOVA ANAGRAFICA"
            pNuovo = "S"
        End If
    End Sub

    Public Function AggiornaDestCliFor() As Boolean
        AggiornaDestCliFor = True
        If txtTipo.Text.Trim = "" Then
            txtTipo.BackColor = SEGNALA_KO : AggiornaDestCliFor = False
        ElseIf Not IsNumeric(txtTipo.Text.Trim) Then
            txtTipo.BackColor = SEGNALA_KO : AggiornaDestCliFor = False
        Else
            txtTipo.BackColor = SEGNALA_OK
        End If
        'GIU170423 BLOCCO SOLO PER I CLIENTI
        Dim myCodice As String = Session(CSTCODCOGE)
        If IsNothing(myCodice) Then
            myCodice = ""
        End If
        If String.IsNullOrEmpty(myCodice) Then
            myCodice = ""
        End If
        If myCodice.Trim = "" Then
            AggiornaDestCliFor = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Destinazione Merce", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        ElseIf Left(myCodice, 1) <> "1" And Left(myCodice, 1) <> "9" Then
            AggiornaDestCliFor = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Destinazione Merce", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '------------
        If txtRagioneSoc.Text.Trim = "" Then
            txtRagioneSoc.BackColor = SEGNALA_KO : AggiornaDestCliFor = False
        Else
            txtRagioneSoc.BackColor = SEGNALA_OK
        End If
        'giu210423 tolto obbligo email Ilaria del 21/04 GIU170423 EMAIL 12/04/23 ILARIA
        '''If Left(myCodice, 1) = "1" Then
        '''    If txtRagSoc35.Text.Trim = "" Then
        '''        txtRagSoc35.BackColor = SEGNALA_KO : AggiornaDestCliFor = False
        '''    Else
        '''        txtRagSoc35.BackColor = SEGNALA_OK
        '''    End If
        '''    If txtRiferimento35.Text.Trim = "" Then
        '''        txtRiferimento35.BackColor = SEGNALA_KO : AggiornaDestCliFor = False
        '''    Else
        '''        txtRiferimento35.BackColor = SEGNALA_OK
        '''    End If
        '''End If
        '----
        If TxtProvincia.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
                TxtProvincia.BackColor = SEGNALA_KO
                AggiornaDestCliFor = False
            Else
                TxtProvincia.BackColor = SEGNALA_OK
            End If
        Else
            TxtProvincia.BackColor = SEGNALA_OK
        End If
        If TxtStato.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtStato.Text, Def.NAZIONI, Session(ESERCIZIO)) = "" Then
                TxtStato.BackColor = SEGNALA_KO
                AggiornaDestCliFor = False
            Else
                TxtStato.BackColor = SEGNALA_OK
            End If
        Else
            TxtStato.BackColor = SEGNALA_OK
        End If
        'GIU190122
        If txtEmail.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmail.Text.Trim) = False Then
                txtEmail.BackColor = SEGNALA_KO
                AggiornaDestCliFor = False
            Else
                txtEmail.Text = txtEmail.Text.Trim.ToLower
                txtEmail.BackColor = SEGNALA_OK
            End If
        Else
            txtEmail.BackColor = SEGNALA_OK
        End If
        If AggiornaDestCliFor = False Then
            Exit Function
        End If
        '-----------------------------
        If (Session(RKANAGRDESTCLI) Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        Rk = Session(RKANAGRDESTCLI)
        Rk.Ragione_Sociale = Mid(txtRagioneSoc.Text.Trim, 1, 50)
        If (Rk.Ragione_Sociale Is Nothing) Then
            SvuotaCampi()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento anagrafica non riuscito.", WUC_ModalPopup.TYPE_ERROR)
            AggiornaDestCliFor = False
            Exit Function
        End If
        If lblCodice.Text.Trim = "-1" Then
            If ControlloDoppio(Rk.Ragione_Sociale.Trim, Mid(TxtIndirizzo.Text.Trim, 1, 50), Mid(TxtLocalita.Text.Trim, 1, 50)) = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Ragione Sociale, Indirizzo e Località già presenti.", WUC_ModalPopup.TYPE_ERROR)
                AggiornaDestCliFor = False
                Exit Function
            End If
        Else
            If ControlloDoppio(Rk.Ragione_Sociale.Trim, Mid(TxtIndirizzo.Text.Trim, 1, 50), Mid(TxtLocalita.Text.Trim, 1, 50), , lblCodice.Text.Trim) = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Ragione Sociale, Indirizzo e Località già presenti.", WUC_ModalPopup.TYPE_ERROR)
                AggiornaDestCliFor = False
                Exit Function
            End If
        End If
        'ok
        Rk.Denominazione = Mid(txtDenominazione.Text.Trim, 1, 50)
        Rk.Riferimento = Mid(txtRiferimento.Text.Trim, 1, 500) 'giu310123
        Rk.Ragione_Sociale35 = Mid(txtRagSoc35.Text.Trim, 1, 35)
        Rk.Riferimento35 = Mid(txtRiferimento35.Text.Trim, 1, 35)
        Rk.Indirizzo = Mid(TxtIndirizzo.Text.Trim, 1, 50)
        Rk.Cap = Mid(TxtCAP.Text.Trim, 1, 5)
        Rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        Rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        Rk.Stato = Mid(TxtStato.Text.Trim, 1, 3)
        Rk.Telefono1 = Mid(txtTelefono1.Text.Trim, 1, 30)
        Rk.Telefono2 = Mid(txtTelefono2.Text.Trim, 1, 30)
        Rk.Fax = Mid(txtFax.Text.Trim, 1, 30)
        Rk.EMail = Mid(txtEmail.Text.Trim, 1, 100)
        Rk.Tipo = Mid(txtTipo.Text, 1, 5)
        Session(RKANAGRDESTCLI) = Rk
        Try
            SqlDSAnagrCliFor.UpdateParameters.Item("Progressivo").DefaultValue = lblCodice.Text.Trim 'no l SESSIONE Session(IDDESTCLIFOR)
            SqlDSAnagrCliFor.UpdateParameters.Item("Codice").DefaultValue = Session(CSTCODCOGE)
            SqlDSAnagrCliFor.UpdateParameters.Item("Ragione_Sociale").DefaultValue = Rk.Ragione_Sociale
            SqlDSAnagrCliFor.UpdateParameters.Item("Denominazione").DefaultValue = Rk.Denominazione
            SqlDSAnagrCliFor.UpdateParameters.Item("Riferimento").DefaultValue = Rk.Riferimento
            SqlDSAnagrCliFor.UpdateParameters.Item("Indirizzo").DefaultValue = Rk.Indirizzo
            SqlDSAnagrCliFor.UpdateParameters.Item("Cap").DefaultValue = Rk.Cap
            SqlDSAnagrCliFor.UpdateParameters.Item("Localita").DefaultValue = Rk.Localita
            SqlDSAnagrCliFor.UpdateParameters.Item("Provincia").DefaultValue = Rk.Provincia
            SqlDSAnagrCliFor.UpdateParameters.Item("Stato").DefaultValue = Rk.Stato
            SqlDSAnagrCliFor.UpdateParameters.Item("Telefono1").DefaultValue = Rk.Telefono1
            SqlDSAnagrCliFor.UpdateParameters.Item("Telefono2").DefaultValue = Rk.Telefono2
            SqlDSAnagrCliFor.UpdateParameters.Item("Fax").DefaultValue = Rk.Fax
            SqlDSAnagrCliFor.UpdateParameters.Item("EMail").DefaultValue = Rk.EMail
            SqlDSAnagrCliFor.UpdateParameters.Item("Tipo").DefaultValue = Rk.Tipo
            SqlDSAnagrCliFor.UpdateParameters.Item("Ragione_Sociale35").DefaultValue = Rk.Ragione_Sociale35
            SqlDSAnagrCliFor.UpdateParameters.Item("Riferimento35").DefaultValue = Rk.Riferimento35
            SqlDSAnagrCliFor.Update()
            SqlDSAnagrCliFor.DataBind()
            'giu080423 leggo i dati appena inseriti
            Dim SQLCmd As New SqlCommand
            Dim SQLAdap As New SqlDataAdapter
            Dim ds As New dsClienti
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Try
                SQLCmd.Connection = New SqlConnection
                SQLCmd.Connection.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
                SQLCmd.CommandType = CommandType.Text
                SQLCmd.CommandText = "SELECT * FROM DestClienti WHERE Codice='" & Session(CSTCODCOGE).ToString.Trim & "' " & _
                                                             "AND Ragione_Sociale = '" & Controlla_Apice(Rk.Ragione_Sociale.Trim) & "' AND isnull(Indirizzo,'') = '" & Controlla_Apice(Rk.Indirizzo.Trim) & "' AND isnull(Localita,'') = '" & Controlla_Apice(Rk.Localita.Trim) & "'"
                SQLAdap.SelectCommand = SQLCmd
                SQLAdap.Fill(ds.Doppi)
                If ds.Doppi.Rows.Count > 0 Then
                    Session(IDDESTCLIFOR) = ds.Doppi.Item(0).Item("Progressivo")
                    lblCodice.Text = ds.Doppi.Item(0).Item("Progressivo")
                    Rk.Progressivo = lblCodice.Text.Trim
                    Session(RKANAGRDESTCLI) = Rk
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Errore durante la lettura Ragione Sociale, Indirizzo e Località.", WUC_ModalPopup.TYPE_ERROR)
            End Try
            '--------------------------------------------
            PopolaCampi()
            Session(DESTCLIFOR) = Nothing 'cosi verra ricaricato dal DB le destinazioni
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Function ControlloDoppio(ByVal RagSoc As String, ByVal Indir As String, ByVal Loc As String, Optional ByVal Ricarica As Boolean = False, Optional ByVal pProgr As String = "") As Boolean
        If String.IsNullOrEmpty(RagSoc) Then RagSoc = ""
        If String.IsNullOrEmpty(Indir) Then Indir = ""
        If String.IsNullOrEmpty(Loc) Then Loc = ""
        Dim SQLCmd As New SqlCommand
        Dim SQLAdap As New SqlDataAdapter
        Dim ds As New dsClienti
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Try
            SQLCmd.Connection = New SqlConnection
            SQLCmd.Connection.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SQLCmd.CommandType = CommandType.Text
            If pProgr.Trim = "" Then
                SQLCmd.CommandText = "SELECT * FROM DestClienti WHERE Codice='" & Session(CSTCODCOGE).ToString.Trim & "' " & _
                                                        "AND Ragione_Sociale = '" & Controlla_Apice(RagSoc) & "' AND isnull(Indirizzo,'') = '" & Controlla_Apice(Indir) & "' AND isnull(Localita,'') = '" & Controlla_Apice(Loc) & "'"
            Else
                SQLCmd.CommandText = "SELECT * FROM DestClienti WHERE Codice='" & Session(CSTCODCOGE).ToString.Trim & "' " & _
                                                        "AND Ragione_Sociale = '" & Controlla_Apice(RagSoc) & "' AND isnull(Indirizzo,'') = '" & Controlla_Apice(Indir) & "' AND isnull(Localita,'') = '" & Controlla_Apice(Loc) & "'" & _
                                                        "AND Progressivo<>" & pProgr.Trim
            End If
            SQLAdap.SelectCommand = SQLCmd
            SQLAdap.Fill(ds.Doppi)
            If ds.Doppi.Rows.Count > 0 Then
                ControlloDoppio = True
                If Ricarica Then
                    Session(IDDESTCLIFOR) = ds.Doppi.Item(0).Item("Progressivo")
                    lblCodice.Text = ds.Doppi.Item(0).Item("Progressivo")
                    Rk.Progressivo = lblCodice.Text.Trim
                    Session(RKANAGRDESTCLI) = Rk
                End If
            Else
                ControlloDoppio = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Errore durante il controllo Ragione Sociale, Indirizzo e Località.", WUC_ModalPopup.TYPE_ERROR)
            ControlloDoppio = False
        End Try
    End Function

End Class