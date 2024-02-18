Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Zone
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

    Public rk As StrZone

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSZone.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Gestione anagrafiche Zone"
        If (Not IsPostBack) Then
            ddlZone.Items.Clear()
            ddlZone.Items.Add("")
            ddlZone.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDZONE) = ""
        ddlZone.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKZONE) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvZone As DataView
        dvZone = SqlDSZone.Select(DataSourceSelectArguments.Empty)
        If (dvZone Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvZone.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvZone.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvZone.Count > 0 Then
                Session(IDZONE) = dvZone.Item(0).Item("Codice")
                txtCodice.Text = dvZone.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvZone.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDZone = IIf(IsNumeric(Session(IDZONE)), Session(IDZONE), 0)
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
        Session(RKZONE) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlZone.SelectedIndex = 0
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
            If ddlZone.SelectedIndex > 0 Then
                Session(IDZONE) = IIf(IsNumeric(ddlZone.SelectedValue), ddlZone.SelectedValue, 0)
            Else
                Session(IDZONE) = "0"
            End If
            SqlDSZone.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSZone.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSZone.Update()
            SqlDSZone.DataBind()
            '-----
            ddlZone.Items.Clear()
            ddlZone.Items.Add("")
            ddlZone.DataBind()
            PopolaEntityDati()
            Dim strErrore As String = ""
            App.CaricaZone(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
            If strErrore.Trim <> "" Then
                ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch Ex As Exception
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlZone.SelectedIndexChanged
        Session(IDZONE) = IIf(IsNumeric(ddlZone.SelectedValue), ddlZone.SelectedValue, 0)
        If ddlZone.SelectedIndex = 0 Then
            Session(IDZONE) = "0"
            txtCodice.Text = Session(IDZONE)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDZONE)
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
        Dim dvZone As DataView
        dvZone = SqlDSZone.Select(DataSourceSelectArguments.Empty)
        If (dvZone Is Nothing) Then
            Exit Sub
        End If
        If dvZone.Count > 0 Then
            dvZone.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvZone.Count > 0 Then
                Session(IDZONE) = dvZone.Item(0).Item("Codice")
                txtCodice.Text = dvZone.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvZone.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub
End Class