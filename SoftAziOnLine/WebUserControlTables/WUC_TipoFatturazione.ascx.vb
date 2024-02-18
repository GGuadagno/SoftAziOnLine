Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_TipoFatturazione
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

    Public rk As StrTipoFatt

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione Tipo Fatturazione"
        If (Not IsPostBack) Then
            DDLTipoFatt.Items.Clear()
            DDLTipoFatt.Items.Add("")
            DDLTipoFatt.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(CODICETIPOFATT) = ""
        DDLTipoFatt.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKTIPOFATT) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvTipoFatt As DataView
        dvTipoFatt = SqlDSTipoFatt.Select(DataSourceSelectArguments.Empty)
        If (dvTipoFatt Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvTipoFatt.Count > 0 Then
            If txtCodice.Text.Trim = "" Then txtCodice.Text = "0"
            dvTipoFatt.RowFilter = "Codice = '" & txtCodice.Text.Trim & "'"
            If dvTipoFatt.Count > 0 Then
                Session(CODICETIPOFATT) = dvTipoFatt.Item(0).Item("Codice")
                txtCodice.Text = dvTipoFatt.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvTipoFatt.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.Codice = Session(CODICETIPOFATT)
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

        If PopolaEntityDati = False Then Exit Function

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        Session(RKTIPOFATT) = rk
    End Function

    Public Sub SetNewCodice()
        DDLTipoFatt.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = ""
        txtDescrizione.Text = ""
        txtCodice.Focus()
    End Sub
    ' ''Public Function GetNewCodice() As Int32
    ' ''    If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
    ' ''        GetNewCodice = 0
    ' ''        Exit Function
    ' ''    End If
    ' ''    GetNewCodice = CLng(txtCodice.Text.Trim)
    ' ''End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True

        If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
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

        If Aggiorna = False Then Exit Function

        Try
            If DDLTipoFatt.SelectedIndex > 0 Then
                Session(CODICETIPOFATT) = DDLTipoFatt.SelectedValue
            Else
                Session(CODICETIPOFATT) = ""
            End If
            SqlDSTipoFatt.UpdateParameters.Item("Codice").DefaultValue = txtCodice.Text.Trim
            SqlDSTipoFatt.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSTipoFatt.Update()
            SqlDSTipoFatt.DataBind()
            '-----
            DDLTipoFatt.Items.Clear()
            DDLTipoFatt.Items.Add("")
            DDLTipoFatt.DataBind()
            PopolaEntityDati()
            '' ''Dim strErrore As String = ""
            '' ''App.CaricaCategorie(Session(ESERCIZIO), strErrore) 'pier110112 AGGIORNO LA CACHE
            '' ''If strErrore.Trim <> "" Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '' ''    ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            '' ''End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub DDLTipoFatt_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoFatt.SelectedIndexChanged
        Session(CODICETIPOFATT) = DDLTipoFatt.SelectedValue
        If DDLTipoFatt.SelectedIndex = 0 Then
            Session(CODICETIPOFATT) = ""
            txtCodice.Text = Session(CODICETIPOFATT)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(CODICETIPOFATT)
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
            _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        End If
    End Sub
    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        Dim dvTipoFatt As DataView
        dvTipoFatt = SqlDSTipoFatt.Select(DataSourceSelectArguments.Empty)
        If (dvTipoFatt Is Nothing) Then
            Exit Sub
        End If
        If dvTipoFatt.Count > 0 Then
            dvTipoFatt.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvTipoFatt.Count > 0 Then
                Session(CODICETIPOFATT) = dvTipoFatt.Item(0).Item("Codice")
                txtCodice.Text = dvTipoFatt.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvTipoFatt.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub
End Class