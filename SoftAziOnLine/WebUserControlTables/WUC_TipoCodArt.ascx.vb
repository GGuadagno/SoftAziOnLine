Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_TipoCodArt
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

    Public rk As StrTipoCodBarArt

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTipoCodBar.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione anagrafiche Linea prodotto"
        If (Not IsPostBack) Then
            ddlTipoCodBar.Items.Clear()
            ddlTipoCodBar.Items.Add("")
            ddlTipoCodBar.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(CODICETIPOCODBARART) = ""
        ddlTipoCodBar.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKTIPOCODBARART) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvTipoCodBar As DataView
        dvTipoCodBar = SqlDSTipoCodBar.Select(DataSourceSelectArguments.Empty)
        If (dvTipoCodBar Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvTipoCodBar.Count > 0 Then
            If txtCodice.Text.Trim = "" Then txtCodice.Text = "0"
            dvTipoCodBar.RowFilter = "Tipo_Codice = '" & txtCodice.Text.Trim & "'"
            If dvTipoCodBar.Count > 0 Then
                Session(CODICETIPOCODBARART) = dvTipoCodBar.Item(0).Item("Tipo_Codice")
                txtCodice.Text = dvTipoCodBar.Item(0).Item("Tipo_Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvTipoCodBar.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.Tipo_Codice = Session(CODICETIPOCODBARART)
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

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 30)
        Session(RKTIPOCODBARART) = rk
    End Function

    Public Sub SetNewCodice()
        ddlTipoCodBar.SelectedIndex = 0
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

        If Aggiorna = False Then Exit Function

        Try
            If DDLTipoCodBar.SelectedIndex > 0 Then
                Session(CODICETIPOCODBARART) = DDLTipoCodBar.SelectedValue
            Else
                Session(CODICETIPOCODBARART) = ""
            End If
            SqlDSTipoCodBar.UpdateParameters.Item("Tipo_Codice").DefaultValue = txtCodice.Text.Trim
            SqlDSTipoCodBar.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 30)
            SqlDSTipoCodBar.Update()
            SqlDSTipoCodBar.DataBind()
            '-----
            DDLTipoCodBar.Items.Clear()
            DDLTipoCodBar.Items.Add("")
            DDLTipoCodBar.DataBind()
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

    Private Sub DDLTipoCodBar_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoCodBar.SelectedIndexChanged
        Session(CODICETIPOCODBARART) = DDLTipoCodBar.SelectedValue
        If DDLTipoCodBar.SelectedIndex = 0 Then
            Session(CODICETIPOCODBARART) = ""
            txtCodice.Text = Session(CODICETIPOCODBARART)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(CODICETIPOCODBARART)
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
        Dim dvTipoCodBar As DataView
        dvTipoCodBar = SqlDSTipoCodBar.Select(DataSourceSelectArguments.Empty)
        If (dvTipoCodBar Is Nothing) Then
            Exit Sub
        End If
        If dvTipoCodBar.Count > 0 Then
            dvTipoCodBar.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvTipoCodBar.Count > 0 Then
                Session(CODICETIPOCODBARART) = dvTipoCodBar.Item(0).Item("Tipo_Codice")
                txtCodice.Text = dvTipoCodBar.Item(0).Item("Tipo_Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvTipoCodBar.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub
End Class