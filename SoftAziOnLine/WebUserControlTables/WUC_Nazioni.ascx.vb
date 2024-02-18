Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Nazioni
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

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSNazioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Gestione Tabelle Nazioni"
        If (Not IsPostBack) Then
            DDLNazioni.Items.Clear()
            DDLNazioni.Items.Add("")
            DDLNazioni.DataBind()
        End If
        ModalPopup.WucElement = Me
    End Sub

    Public Sub SvuotaCampi()
        DDLNazioni.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        txtCodiceISO.Text = "" : txtCodiceISO.BackColor = SEGNALA_OK
        txtCUnico.Text = "" : txtCUnico.BackColor = SEGNALA_OK
        txtPrefisso.Text = "" : txtPrefisso.BackColor = SEGNALA_OK
        DDLProdottoDHL.SelectedIndex = 0 : DDLProdottoDHL.BackColor = SEGNALA_OK
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvNazioni As DataView
        dvNazioni = SqlDSNazioni.Select(DataSourceSelectArguments.Empty)
        If (dvNazioni Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvNazioni.Count > 0 Then
            dvNazioni.RowFilter = "Codice = '" & txtCodice.Text.Trim + "'"
            If dvNazioni.Count > 0 Then
                txtCodice.Text = dvNazioni.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvNazioni.Item(0).Item("Descrizione").ToString.Trim
                txtCodiceISO.Text = dvNazioni.Item(0).Item("Codice_ISO").ToString.Trim
                txtCUnico.Text = dvNazioni.Item(0).Item("CUnico").ToString.Trim
                txtPrefisso.Text = dvNazioni.Item(0).Item("Prefisso").ToString.Trim
                PosizionaItemDDL(dvNazioni.Item(0).Item("Prodotto_DHL").ToString.Trim, DDLProdottoDHL)
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtCodice.BackColor = SEGNALA_OK
        End If
        '-
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
        If txtCodiceISO.Text.Trim = "" Then
            txtCodiceISO.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtCodiceISO.BackColor = SEGNALA_OK
        End If
        If txtPrefisso.Text.Trim = "" Then
            txtPrefisso.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtPrefisso.BackColor = SEGNALA_OK
        End If
        If DDLProdottoDHL.SelectedIndex < 1 Then
            DDLProdottoDHL.BackColor = SEGNALA_KO
            PopolaEntityDati = False
        Else
            DDLProdottoDHL.BackColor = SEGNALA_OK
        End If
    End Function

    Public Sub SetNewCodice(ByVal _ID As String)
        DDLNazioni.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        txtCodiceISO.Text = ""
        txtCUnico.Text = ""
        txtPrefisso.Text = ""
        DDLProdottoDHL.SelectedIndex = 0
        txtDescrizione.Focus()
    End Sub
    Public Function GetNewCodice() As String
        GetNewCodice = txtCodice.Text.Trim
    End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True

        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtCodice.BackColor = SEGNALA_OK
        End If
        '-
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
        'giu230722
        If txtCodiceISO.Text.Trim = "" Then
            txtCodiceISO.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtCodiceISO.BackColor = SEGNALA_OK
        End If
        If txtPrefisso.Text.Trim = "" Then
            txtPrefisso.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtPrefisso.BackColor = SEGNALA_OK
        End If
        If DDLProdottoDHL.SelectedIndex < 1 Then
            DDLProdottoDHL.BackColor = SEGNALA_KO
            Aggiorna = False
        Else
            DDLProdottoDHL.BackColor = SEGNALA_OK
        End If
        If Aggiorna = False Then Exit Function

        Try
            SqlDSNazioni.UpdateParameters.Item("Codice").DefaultValue = txtCodice.Text.Trim
            SqlDSNazioni.UpdateParameters.Item("Descrizione").DefaultValue = txtDescrizione.Text.Trim
            SqlDSNazioni.UpdateParameters.Item("Codice_ISO").DefaultValue = txtCodiceISO.Text.Trim
            SqlDSNazioni.UpdateParameters.Item("CUnico").DefaultValue = txtCUnico.Text.Trim
            SqlDSNazioni.UpdateParameters.Item("Prefisso").DefaultValue = txtPrefisso.Text.Trim
            SqlDSNazioni.UpdateParameters.Item("Prodotto_DHL").DefaultValue = DDLProdottoDHL.SelectedValue
            SqlDSNazioni.Update()
            SqlDSNazioni.DataBind()
            '-----
            DDLNazioni.Items.Clear()
            DDLNazioni.Items.Add("")
            DDLNazioni.DataBind()
            PopolaEntityDati()
            Dim strErrore As String = ""
            App.CaricaNazioni(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlNazioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLNazioni.SelectedIndexChanged
        If DDLNazioni.SelectedIndex = 0 Then
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Aggiorna elemento in tabella")
        txtCodice.Text = DDLNazioni.SelectedValue.Trim
        PopolaEntityDati()
    End Sub

    Private Sub txtCodice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodice.TextChanged
        Dim SAVECodice As String = txtCodice.Text.Trim
        PopolaEntityDati()
        txtDescrizione.Focus()
        If txtCodice.Text.Trim = "" Then
            txtCodice.Text = SAVECodice
        Else
            txtCodice.Enabled = False
            _WucElement.SetlblMessaggi("Aggiorna elemento in tabella")
        End If
    End Sub
    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        Dim dvNazioni As DataView
        dvNazioni = SqlDSNazioni.Select(DataSourceSelectArguments.Empty)
        If (dvNazioni Is Nothing) Then
            txtCodiceISO.Focus()
            Exit Sub
        End If
        If dvNazioni.Count > 0 Then
            dvNazioni.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvNazioni.Count > 0 Then
                txtCodice.Text = dvNazioni.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvNazioni.Item(0).Item("Descrizione").ToString.Trim
                txtCodiceISO.Text = dvNazioni.Item(0).Item("Codice_ISO").ToString.Trim
                txtCUnico.Text = dvNazioni.Item(0).Item("CUnico").ToString.Trim
                txtPrefisso.Text = dvNazioni.Item(0).Item("Prefisso").ToString.Trim
                PosizionaItemDDL(dvNazioni.Item(0).Item("Prodotto_DHL").ToString.Trim, DDLProdottoDHL)
            End If
        End If
        txtCodiceISO.Focus()
    End Sub

End Class