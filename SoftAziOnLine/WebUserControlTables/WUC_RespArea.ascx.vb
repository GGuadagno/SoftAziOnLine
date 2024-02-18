Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_RespArea
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

    Public rk As StrRespArea

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRespArea.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        lblLabelTipoRK.Text = "Gestione anagrafiche Responsabili Area"
        If (Not IsPostBack) Then
            ddlRespAreaTABRA.Items.Clear()
            ddlRespAreaTABRA.Items.Add("")
            ddlRespAreaTABRA.DataBind()
           
        End If
        ModalPopup.WucElement = Me
       
    End Sub

    Public Sub SvuotaCampi()
        Session(IDRESPAREA) = ""
        ddlRespAreaTABRA.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        TxtResidenza.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        txtPartitaIVA.Text = ""
        txtCodice_CoGe.Text = ""
        TxtEmail.Text = "" : TxtEmail.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKRESPAREA) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvRespArea As DataView
        dvRespArea = SqlDSRespArea.Select(DataSourceSelectArguments.Empty)
        If (dvRespArea Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvRespArea.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvRespArea.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvRespArea.Count > 0 Then
                Session(IDRESPAREA) = dvRespArea.Item(0).Item("Codice")
                txtCodice.Text = dvRespArea.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvRespArea.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvRespArea.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvRespArea.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvRespArea.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvRespArea.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvRespArea.Item(0).Item("Codice_CoGe").ToString.Trim
                TxtEmail.Text = dvRespArea.Item(0).Item("EMail").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDRespArea = IIf(IsNumeric(Session(IDRESPAREA)), Session(IDRESPAREA), 0)
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
        'giu190122
        If TxtEmail.Text.Trim <> "" Then
            If ConvalidaEmail(TxtEmail.Text.Trim) = False Then
                TxtEmail.BackColor = SEGNALA_KO
                PopolaEntityDati = False
            Else
                TxtEmail.BackColor = SEGNALA_OK
            End If
        Else
            TxtEmail.BackColor = SEGNALA_OK
        End If
        '---------
        If PopolaEntityDati = False Then Exit Function

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        rk.Residenza = Mid(TxtResidenza.Text.Trim, 1, 50)
        rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        rk.Partita_IVA = Mid(txtPartitaIVA.Text.Trim, 1, 16)
        rk.Codice_CoGe = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
        rk.Email = Mid(TxtEmail.Text.Trim, 1, 310)
        Session(RKRESPAREA) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlRespAreaTABRA.SelectedIndex = 0
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
        If TxtProvincia.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
                TxtProvincia.BackColor = SEGNALA_KO
                Aggiorna = False
            Else
                TxtProvincia.BackColor = SEGNALA_OK
            End If
        Else
            TxtProvincia.BackColor = SEGNALA_OK
        End If
        'giu200312
        If txtCodice_CoGe.Text.Trim <> "" Then
            If App.GetValoreFromChiave(txtCodice_CoGe.Text, Def.FORNITORI, Session(ESERCIZIO)) = "" Then
                txtCodice_CoGe.BackColor = SEGNALA_KO
                Aggiorna = False
            Else
                txtCodice_CoGe.BackColor = SEGNALA_OK
            End If
        Else
            txtCodice_CoGe.BackColor = SEGNALA_OK
        End If
        If TxtEmail.Text.Trim <> "" Then
            If ConvalidaEmail(TxtEmail.Text.Trim) = False Then
                TxtEmail.BackColor = SEGNALA_KO
                Aggiorna = False
            Else
                TxtEmail.Text = TxtEmail.Text.Trim.ToLower
                TxtEmail.BackColor = SEGNALA_OK
            End If
        Else
            TxtEmail.BackColor = SEGNALA_OK
        End If
       
        If Aggiorna = False Then Exit Function

        Try
            If ddlRespAreaTABRA.SelectedIndex > 0 Then
                Session(IDRESPAREA) = IIf(IsNumeric(ddlRespAreaTABRA.SelectedValue), ddlRespAreaTABRA.SelectedValue, 0)
            Else
                Session(IDRESPAREA) = "0"
            End If
            SqlDSRespArea.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSRespArea.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSRespArea.UpdateParameters.Item("Residenza").DefaultValue = Mid(TxtResidenza.Text.Trim, 1, 50)
            SqlDSRespArea.UpdateParameters.Item("Localita").DefaultValue = Mid(TxtLocalita.Text.Trim, 1, 50)
            SqlDSRespArea.UpdateParameters.Item("Provincia").DefaultValue = Mid(TxtProvincia.Text.Trim, 1, 2)
            SqlDSRespArea.UpdateParameters.Item("Partita_IVA").DefaultValue = Mid(txtPartitaIVA.Text.Trim, 1, 11)
            SqlDSRespArea.UpdateParameters.Item("Codice_Coge").DefaultValue = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
            SqlDSRespArea.UpdateParameters.Item("EMail").DefaultValue = Mid(TxtEmail.Text.Trim, 1, 310)
            SqlDSRespArea.Update()
            SqlDSRespArea.DataBind()
            '-----
            ddlRespAreaTABRA.Items.Clear()
            ddlRespAreaTABRA.Items.Add("")
            ddlRespAreaTABRA.DataBind()
            PopolaEntityDati()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlRespAreaTABRA_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRespAreaTABRA.SelectedIndexChanged
        Session(IDRESPAREA) = IIf(IsNumeric(ddlRespAreaTABRA.SelectedValue), ddlRespAreaTABRA.SelectedValue, 0)
        If ddlRespAreaTABRA.SelectedIndex = 0 Then
            Session(IDRESPAREA) = "0"
            txtCodice.Text = Session(IDRESPAREA)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDRESPAREA)
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
        If txtDescrizione.Text.Trim = "" Then Exit Sub
        Dim dvRespArea As DataView
        dvRespArea = SqlDSRespArea.Select(DataSourceSelectArguments.Empty)
        If (dvRespArea Is Nothing) Then
            TxtResidenza.Focus()
            Exit Sub
        End If
        If dvRespArea.Count > 0 Then
            dvRespArea.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If (dvRespArea Is Nothing) Then
                TxtResidenza.Focus()
                Exit Sub
            End If
            If dvRespArea.Count > 0 Then
                Session(IDRESPAREA) = dvRespArea.Item(0).Item("Codice")
                txtCodice.Text = dvRespArea.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvRespArea.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvRespArea.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvRespArea.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvRespArea.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvRespArea.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvRespArea.Item(0).Item("Codice_CoGe").ToString.Trim
                TxtEmail.Text = dvRespArea.Item(0).Item("EMail").ToString.Trim
                _WucElement.SetlblMessaggi("Responsabile già in tabella.")
            End If
        End If
        TxtResidenza.Focus()
    End Sub

End Class