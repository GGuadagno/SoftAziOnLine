Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_CausaliNonEvasione
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

    Public rk As StrCausNonEvasione

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSCausNonEvasione.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione Causali di Non Evasione"
        If (Not IsPostBack) Then
            ddlCausNonEvasione.Items.Clear()
            ddlCausNonEvasione.Items.Add("")
            ddlCausNonEvasione.DataBind()
        End If
        ModalPopup.WucElement = Me
    End Sub

    Public Sub SvuotaCampi()
        Session(IDCAUSNONEVASIONE) = ""
        ddlCausNonEvasione.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKCAUSNONEVASIONE) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvCNonEv As DataView
        dvCNonEv = SqlDSCausNonEvasione.Select(DataSourceSelectArguments.Empty)
        If (dvCNonEv Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvCNonEv.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvCNonEv.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvCNonEv.Count > 0 Then
                Session(IDCAUSNONEVASIONE) = dvCNonEv.Item(0).Item("Codice")
                txtCodice.Text = dvCNonEv.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCNonEv.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.Codice = IIf(IsNumeric(Session(IDCAUSNONEVASIONE)), Session(IDCAUSNONEVASIONE), 0)
        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : PopolaEntityDati = False
        ElseIf Not IsNumeric(txtCodice.Text.Trim) Then
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
        Session(RKCAUSNONEVASIONE) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlCausNonEvasione.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        txtDescrizione.Focus()
    End Sub
    Public Function GetNewCodice() As Int32
        If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
            GetNewCodice = 0
            Exit Function
        End If
        GetNewCodice = CLng(txtCodice.Text.Trim)
    End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True
        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : Aggiorna = False
        ElseIf Not IsNumeric(txtCodice.Text.Trim) Then
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
            If ddlCausNonEvasione.SelectedIndex > 0 Then
                Session(IDCAUSNONEVASIONE) = IIf(IsNumeric(ddlCausNonEvasione.SelectedValue), ddlCausNonEvasione.SelectedValue, 0)
            Else
                Session(IDCAUSNONEVASIONE) = "0"
            End If
            SqlDSCausNonEvasione.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSCausNonEvasione.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSCausNonEvasione.Update()
            SqlDSCausNonEvasione.DataBind()
            '-----
            ddlCausNonEvasione.Items.Clear()
            ddlCausNonEvasione.Items.Add("")
            ddlCausNonEvasione.DataBind()
            PopolaEntityDati()
            ' ''Dim strErrore As String = ""
            ' ''App.CaricaAgenti(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
            ' ''If strErrore.Trim <> "" Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            ' ''End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlCausNonEvasione_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCausNonEvasione.SelectedIndexChanged
        Session(IDCAUSNONEVASIONE) = IIf(IsNumeric(ddlCausNonEvasione.SelectedValue), ddlCausNonEvasione.SelectedValue, 0)
        If ddlCausNonEvasione.SelectedIndex = 0 Then
            Session(IDCAUSNONEVASIONE) = "0"
            txtCodice.Text = Session(IDCAUSNONEVASIONE)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDCAUSNONEVASIONE)
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
        Dim dvCNonEv As DataView
        dvCNonEv = SqlDSCausNonEvasione.Select(DataSourceSelectArguments.Empty)
        If (dvCNonEv Is Nothing) Then
            Exit Sub
        End If
        If dvCNonEv.Count > 0 Then
            dvCNonEv.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvCNonEv.Count > 0 Then
                Session(IDCAUSNONEVASIONE) = dvCNonEv.Item(0).Item("Codice")
                txtCodice.Text = dvCNonEv.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCNonEv.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub

End Class