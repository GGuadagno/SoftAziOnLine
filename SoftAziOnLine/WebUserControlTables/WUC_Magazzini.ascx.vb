Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Magazzini
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

    Public rk As StrMagazzini

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSMagazzini.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione Magazzini"
        If (Not IsPostBack) Then
            ddlMagazzini.Items.Clear()
            ddlMagazzini.Items.Add("")
            ddlMagazzini.DataBind()
        End If
        ModalPopup.WucElement = Me
    End Sub

    Public Sub SvuotaCampi()
        Session(IDMAGAZZINI) = ""
        ddlMagazzini.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKMAGAZZINI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvMagazzini As DataView
        dvMagazzini = SqlDSMagazzini.Select(DataSourceSelectArguments.Empty)
        If (dvMagazzini Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvMagazzini.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "-1"
            dvMagazzini.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvMagazzini.Count > 0 Then
                Session(IDMAGAZZINI) = dvMagazzini.Item(0).Item("Codice")
                txtCodice.Text = dvMagazzini.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvMagazzini.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDMagazzini = IIf(IsNumeric(Session(IDMAGAZZINI)), Session(IDMAGAZZINI), -1)
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
        Session(RKMAGAZZINI) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlMagazzini.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        txtDescrizione.Focus()
    End Sub
    Public Function GetNewCodice() As Int32
        If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
            GetNewCodice = -1
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
            If ddlMagazzini.SelectedIndex > 0 Then
                Session(IDMAGAZZINI) = IIf(IsNumeric(ddlMagazzini.SelectedValue), ddlMagazzini.SelectedValue, 0)
            Else
                Session(IDMAGAZZINI) = "-1"
            End If
            SqlDSMagazzini.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSMagazzini.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSMagazzini.Update()
            SqlDSMagazzini.DataBind()
            '-----
            ddlMagazzini.Items.Clear()
            ddlMagazzini.Items.Add("")
            ddlMagazzini.DataBind()
            PopolaEntityDati()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlMagazzini_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzini.SelectedIndexChanged
        Session(IDMAGAZZINI) = IIf(IsNumeric(ddlMagazzini.SelectedValue), ddlMagazzini.SelectedValue, 0)
        If ddlMagazzini.SelectedIndex = 0 Then
            Session(IDMAGAZZINI) = "-1"
            txtCodice.Text = Session(IDMAGAZZINI)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDMAGAZZINI)
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
        Dim dvMagazzini As DataView
        dvMagazzini = SqlDSMagazzini.Select(DataSourceSelectArguments.Empty)
        If (dvMagazzini Is Nothing) Then
            Exit Sub
        End If
        If dvMagazzini.Count > 0 Then
            dvMagazzini.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvMagazzini.Count > 0 Then
                Session(IDMAGAZZINI) = dvMagazzini.Item(0).Item("Codice")
                txtCodice.Text = dvMagazzini.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvMagazzini.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub

End Class