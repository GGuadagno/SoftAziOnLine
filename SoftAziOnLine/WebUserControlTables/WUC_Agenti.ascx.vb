Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Agenti
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

    Public rk As StrAgenti

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSAgenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCapiGruppo.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Gestione anagrafiche Agenti"
        If (Not IsPostBack) Then
            ddlAgenti.Items.Clear()
            ddlAgenti.Items.Add("")
            ddlAgenti.DataBind()
            '-
            DDLCapiGruppo.Items.Clear()
            DDLCapiGruppo.Items.Add("")
            DDLCapiGruppo.DataBind()
        End If
        ModalPopup.WucElement = Me
        ' ''WFPElencoFor.WucElement = Me

        ' ''If Session(F_ELENCO_CLIFORN_APERTA) Then
        ' ''    WFPElencoFor.Show()
        ' ''End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDAGENTI) = ""
        ddlAgenti.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        TxtResidenza.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        txtPartitaIVA.Text = ""
        txtCodice_CoGe.Text = ""
        DDLCapiGruppo.SelectedIndex = 0 : DDLCapiGruppo.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKAGENTI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvAgenti As DataView
        dvAgenti = SqlDSAgenti.Select(DataSourceSelectArguments.Empty)
        If (dvAgenti Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvAgenti.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvAgenti.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvAgenti.Count > 0 Then
                Session(IDAGENTI) = dvAgenti.Item(0).Item("Codice")
                txtCodice.Text = dvAgenti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvAgenti.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvAgenti.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvAgenti.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvAgenti.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvAgenti.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvAgenti.Item(0).Item("Codice_CoGe").ToString.Trim
                PosizionaItemDDL(dvAgenti.Item(0).Item("CodCapogruppo").ToString.Trim, DDLCapiGruppo)
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDAgenti = IIf(IsNumeric(Session(IDAGENTI)), Session(IDAGENTI), 0)
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
        If DDLCapiGruppo.SelectedIndex < 1 Then
            DDLCapiGruppo.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            DDLCapiGruppo.BackColor = SEGNALA_OK
        End If
        If PopolaEntityDati = False Then Exit Function

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        rk.Residenza = Mid(TxtResidenza.Text.Trim, 1, 50)
        rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        rk.Partita_IVA = Mid(txtPartitaIVA.Text.Trim, 1, 16)
        rk.Codice_CoGe = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
        rk.IDCapoGruppo = DDLCapiGruppo.SelectedValue
        Session(RKAGENTI) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlAgenti.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        DDLCapiGruppo.SelectedIndex = 0
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
        If DDLCapiGruppo.SelectedIndex < 1 Then
            DDLCapiGruppo.BackColor = SEGNALA_KO
            Aggiorna = False
        Else
            DDLCapiGruppo.BackColor = SEGNALA_OK
        End If
        If Aggiorna = False Then Exit Function

        Try
            If ddlAgenti.SelectedIndex > 0 Then
                Session(IDAGENTI) = IIf(IsNumeric(ddlAgenti.SelectedValue), ddlAgenti.SelectedValue, 0)
            Else
                Session(IDAGENTI) = "0"
            End If
            SqlDSAgenti.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSAgenti.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSAgenti.UpdateParameters.Item("Residenza").DefaultValue = Mid(TxtResidenza.Text.Trim, 1, 50)
            SqlDSAgenti.UpdateParameters.Item("Localita").DefaultValue = Mid(TxtLocalita.Text.Trim, 1, 50)
            SqlDSAgenti.UpdateParameters.Item("Provincia").DefaultValue = Mid(TxtProvincia.Text.Trim, 1, 2)
            SqlDSAgenti.UpdateParameters.Item("Partita_IVA").DefaultValue = Mid(txtPartitaIVA.Text.Trim, 1, 11)
            SqlDSAgenti.UpdateParameters.Item("Codice_Coge").DefaultValue = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
            SqlDSAgenti.UpdateParameters.Item("CodCapogruppo").DefaultValue = IIf(IsNumeric(DDLCapiGruppo.SelectedValue), DDLCapiGruppo.SelectedValue, 0)
            SqlDSAgenti.Update()
            SqlDSAgenti.DataBind()
            '-----
            ddlAgenti.Items.Clear()
            ddlAgenti.Items.Add("")
            ddlAgenti.DataBind()
            PopolaEntityDati()
            Dim strErrore As String = ""
            App.CaricaAgenti(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
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

    Private Sub ddlAgenti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgenti.SelectedIndexChanged
        Session(IDAGENTI) = IIf(IsNumeric(ddlAgenti.SelectedValue), ddlAgenti.SelectedValue, 0)
        If ddlAgenti.SelectedIndex = 0 Then
            Session(IDAGENTI) = "0"
            txtCodice.Text = Session(IDAGENTI)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDAGENTI)
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
        Dim dvAgenti As DataView
        dvAgenti = SqlDSAgenti.Select(DataSourceSelectArguments.Empty)
        If (dvAgenti Is Nothing) Then
            TxtResidenza.Focus()
            Exit Sub
        End If
        If dvAgenti.Count > 0 Then
            dvAgenti.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvAgenti.Count > 0 Then
                Session(IDAGENTI) = dvAgenti.Item(0).Item("Codice")
                txtCodice.Text = dvAgenti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvAgenti.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvAgenti.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvAgenti.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvAgenti.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvAgenti.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvAgenti.Item(0).Item("Codice_CoGe").ToString.Trim
                PosizionaItemDDL(dvAgenti.Item(0).Item("CodCapogruppo").ToString.Trim, DDLCapiGruppo)
            End If
        End If
        TxtResidenza.Focus()
    End Sub

End Class