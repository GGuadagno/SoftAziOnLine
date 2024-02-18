Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_AltriIndirizziClienti
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

#Region "Costanti"

    Private Const F_NAZIONI As String = "ElencoNazioniAltriInd"
    Private Const F_PROVINCE As String = "ElencoProvinceAltriInd"
    Private Const SWMYOP As String = "SWOperazioneAltriInd"
    Private Const PROGRKEY As String = "ProgresssivoAltriInd" 'giu170714

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSIndirCF.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        If (Not IsPostBack) Then
            CampiSetEnabledTo(False)
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            If GridViewBody.Rows.Count = 0 Then
                SvuotaCampi()
            End If
            Session(SWMYOP) = SWOPNESSUNA
        End If

        ModalPopup.WucElement = Me
        WFPElencoNazioni.WucElement = Me
        WFPElencoProvince.WucElement = Me

        If Session(F_ELENCO_APERTA) = F_NAZIONI Then
            WFPElencoNazioni.Show()
        End If
        If Session(F_ELENCO_APERTA) = F_PROVINCE Then
            WFPElencoProvince.Show()
        End If
    End Sub

    Private Enum CellIdx
        Sigla = 1
        RagSoc = 2
        Denom = 3
        Rif = 4
        Ind = 5
        CAP = 6
        Loc = 7
        Pr = 8
        Naz = 9
        Tel1 = 10
        Tel2 = 11
        Fax = 12
        Email = 13
    End Enum

    Private Sub GridViewBody_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.PageIndexChanged
        SvuotaCampi()
        Session(PROGRKEY) = ""
        GridViewBody.SelectedIndex = -1
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = False
        '---------
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'giu151117 commentato altrimenti non funziona pagina avanti o pagine indietro
        ' ''e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub
    'giu170714 i dati dalDB e non dalla grid altrimenti i caratteri speciali vengono convertiti in HTML
    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Try
            Session(PROGRKEY) = GridViewBody.SelectedDataKey.Value
            PopolaCampiGridSel()
            '--
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            btnModifica.Enabled = True
            btnElimina.Enabled = True
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore seleziona riga", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    'giu170714
    Private Sub PopolaCampiGridSel()
        Try
            Dim dv As DataView
            dv = SqlDSIndirCF.Select(DataSourceSelectArguments.Empty)
            If (dv Is Nothing) Then
                SvuotaCampi()
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato in tabella l'elemento selezionato.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If dv.Count > 0 Then
                dv.RowFilter = "Tipo = '" & Trim(Session(PROGRKEY)) & "'"
                If dv.Count > 1 Then
                    SvuotaCampi()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Trovati in tabella più volte l'elemento selezionato.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                ElseIf dv.Count = 1 Then
                    'ok posso visualizzare
                    If Not IsDBNull(dv.Item(0).Item("Tipo")) Then
                        txtTipo.Text = dv.Item(0).Item("Tipo").ToString.Trim
                    Else
                        txtTipo.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Ragione_Sociale")) Then
                        txtRagSoc.Text = dv.Item(0).Item("Ragione_Sociale").ToString.Trim
                    Else
                        txtRagSoc.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Denominazione")) Then
                        txtDenominazione.Text = dv.Item(0).Item("Denominazione").ToString.Trim
                    Else
                        txtDenominazione.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Riferimento")) Then
                        txtRiferimento.Text = dv.Item(0).Item("Riferimento").ToString.Trim
                    Else
                        txtRiferimento.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Indirizzo")) Then
                        txtIndirizzo.Text = dv.Item(0).Item("Indirizzo").ToString.Trim
                    Else
                        txtIndirizzo.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("CAP")) Then
                        txtCap.Text = dv.Item(0).Item("CAP").ToString.Trim
                    Else
                        txtCap.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Localita")) Then
                        txtLocalita.Text = dv.Item(0).Item("Localita").ToString.Trim
                    Else
                        txtLocalita.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Provincia")) Then
                        txtProvincia.Text = dv.Item(0).Item("Provincia").ToString.Trim
                    Else
                        txtProvincia.Text = ""
                    End If
                    txtProvincia.ToolTip = App.GetValoreFromChiave(txtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO))
                    If Not IsDBNull(dv.Item(0).Item("Stato")) Then
                        txtCodNazione.Text = dv.Item(0).Item("Stato").ToString.Trim
                    Else
                        txtCodNazione.Text = ""
                    End If
                    txtCodNazione.ToolTip = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
                    If Not IsDBNull(dv.Item(0).Item("Telefono1")) Then
                        txtTel1.Text = dv.Item(0).Item("Telefono1").ToString.Trim
                    Else
                        txtTel1.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Telefono2")) Then
                        txtTel2.Text = dv.Item(0).Item("Telefono2").ToString.Trim
                    Else
                        txtTel2.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Fax")) Then
                        txtFax.Text = dv.Item(0).Item("Fax").ToString.Trim
                    Else
                        txtFax.Text = ""
                    End If
                    If Not IsDBNull(dv.Item(0).Item("Email")) Then
                        txtEMail.Text = dv.Item(0).Item("Email").ToString.Trim
                    Else
                        txtEMail.Text = ""
                    End If
                Else
                    SvuotaCampi()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato in tabella l'elemento selezionato.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            Else
                SvuotaCampi()
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato in tabella l'elemento selezionato.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Catch ex As Exception
            SvuotaCampi()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Non trovato in tabella l'elemento selezionato. Errore:" & vbCrLf & _
                            ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
    End Sub
    '---------
    Private Sub btnTrovaProv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaProv.Click
        Session(F_ELENCO_APERTA) = F_PROVINCE
        WFPElencoProvince.Show(True)
    End Sub

    Private Sub btnTrovaNazione_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaNazione.Click
        Session(F_ELENCO_APERTA) = F_NAZIONI
        WFPElencoNazioni.Show(True)
    End Sub

    Private Sub CampiSetEnabledTo(ByVal valore As Boolean)
        txtTipo.Enabled = valore
        txtragSoc.Enabled = valore
        txtDenominazione.Enabled = valore
        txtRiferimento.Enabled = valore
        txtIndirizzo.Enabled = valore
        txtCap.Enabled = valore
        txtLocalita.Enabled = valore
        btnTrovaProv.Enabled = valore
        txtProvincia.Enabled = valore
        btnTrovaNazione.Enabled = valore
        txtCodNazione.Enabled = valore
        txtTel1.Enabled = valore
        txtTel2.Enabled = valore
        txtFax.Enabled = valore
        txtEMail.Enabled = valore
    End Sub

#End Region

#Region "Metodi publici"

    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        Select Case (finestra)
            Case F_NAZIONI
                txtCodNazione.Text = codice
                txtCodNazione.BackColor = Def.SEGNALA_OK
                txtCodNazione.ToolTip = descrizione
            Case F_PROVINCE
                txtProvincia.Text = codice
                txtProvincia.BackColor = Def.SEGNALA_OK
                txtProvincia.ToolTip = descrizione
        End Select
    End Sub

    Public Sub setPulsantiModalitaConsulta()
        btnNuovo.Enabled = True
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        Try
            If GridViewBody.Rows.Count = 0 Then
                btnModifica.Enabled = False
                btnElimina.Enabled = False
            Else
                If txtTipo.Text.Trim <> "" Then
                    btnModifica.Enabled = True
                    btnElimina.Enabled = True
                Else
                    btnModifica.Enabled = False
                    btnElimina.Enabled = False
                End If
            End If
        Catch ex As Exception
            btnModifica.Enabled = False
            btnElimina.Enabled = False
        End Try
        
    End Sub

    Public Sub setPulsantiModalitaAggiorna()
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = True
        btnAggiorna.Enabled = True
    End Sub

    Public Sub setPulsantiModalitaBlocco()
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
    End Sub

    Public Sub SvuotaCampi()
        txtTipo.Text = "" : txtTipo.BackColor = SEGNALA_OK
        txtragSoc.Text = "" : txtragSoc.BackColor = SEGNALA_OK
        txtDenominazione.Text = ""
        txtRiferimento.Text = ""
        txtIndirizzo.Text = ""
        txtCap.Text = ""
        txtLocalita.Text = ""
        txtProvincia.Text = ""
        txtProvincia.BackColor = SEGNALA_OK
        txtCodNazione.Text = ""
        txtCodNazione.BackColor = SEGNALA_OK
        txtCodNazione.ToolTip = ""
        txtTel1.Text = ""
        txtTel2.Text = ""
        txtFax.Text = ""
        txtEMail.Text = ""
    End Sub
    'GIU290323 PER RICARICARE I DATI DA GESTIONE ANAGRAFICA LA PRIMA VOLTA NON LO CARICA
    Public Sub CaricaAltriIndir()
        Try
            CampiSetEnabledTo(False)
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            SqlDSIndirCF.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SqlDSIndirCF.DataBind()
            GridViewBody.DataBind()
            If GridViewBody.Rows.Count > 0 Then
                GridViewBody.SelectedIndex = 0
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
            Else
                GridViewBody.SelectedIndex = -1
            End If
        Catch ex As Exception
            CampiSetEnabledTo(False)
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            SvuotaCampi()
        End Try
    End Sub
#End Region

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        'giu290819
        If GridViewBody.Rows.Count > 0 Then
            GridViewBody_SelectedIndexChanged(Nothing, Nothing)
        Else
            SvuotaCampi()
            GridViewBody.SelectedIndex = -1
            Exit Sub
        End If
        '---------
        CampiSetEnabledTo(True)
        setPulsantiModalitaAggiorna()
        WucElement.setPulsantiModalitaBloccoByWUC("DAAD")
        GridViewBody.Enabled = False
        txtTipo.Enabled = False
        Session(SWMYOP) = SWOPMODIFICA
        txtragSoc.Focus()
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        'giu290819
        Session(PROGRKEY) = ""
        GridViewBody.SelectedIndex = -1
        '---------
        CampiSetEnabledTo(False)
        setPulsantiModalitaConsulta() 'giu010714 setPulsantiModalitaBlocco()
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = False
        '---------
        WucElement.setPulsantiModalitaSbloccoByWUC()
        GridViewBody.Enabled = True
        SvuotaCampi()
        Session(SWMYOP) = SWOPNESSUNA
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim SWErr As Boolean = False
        If txtTipo.Text.Trim = "" Then
            txtTipo.BackColor = SEGNALA_KO : SWErr = True
        Else
            txtTipo.BackColor = SEGNALA_OK
        End If
        If Session(SWMYOP) = SWOPNUOVO Then
            If CheckNew() = False Then
                txtTipo.BackColor = SEGNALA_KO : SWErr = True
            End If
        End If
        If txtragSoc.Text.Trim = "" Then
            txtragSoc.BackColor = SEGNALA_KO : SWErr = True
        Else
            txtragSoc.BackColor = SEGNALA_OK
        End If
        If txtCodNazione.Text.Trim = "" Then
            txtCodNazione.ToolTip = ""
            txtCodNazione.BackColor = SEGNALA_OK
        Else
            txtCodNazione.ToolTip = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
            If txtCodNazione.ToolTip = "" Then
                txtCodNazione.BackColor = SEGNALA_KO : SWErr = True
            Else
                txtCodNazione.Text = UCase(txtCodNazione.Text.Trim)
                txtCodNazione.BackColor = SEGNALA_OK
            End If
        End If
        If txtProvincia.Text.Trim = "" Then
            txtProvincia.ToolTip = ""
            txtProvincia.BackColor = SEGNALA_OK
        Else
            txtProvincia.ToolTip = App.GetValoreFromChiave(txtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO))
            If txtProvincia.ToolTip = "" Then
                txtProvincia.BackColor = SEGNALA_KO : SWErr = True
            Else
                txtProvincia.Text = UCase(txtProvincia.Text.Trim)
                txtProvincia.BackColor = SEGNALA_OK
            End If
        End If
        Dim myCodice As String = Session(CSTCODCOGEDM)
        If IsNothing(myCodice) Then
            myCodice = ""
        End If
        If String.IsNullOrEmpty(myCodice) Then
            myCodice = ""
        End If
        If myCodice.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Altri indirizzi", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf Left(myCodice, 1) <> "1" And Left(myCodice, 1) <> "9" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Altri indirizzi", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If SWErr = True Then Exit Sub

        Try
            SqlDSIndirCF.UpdateParameters.Item("Codice").DefaultValue = myCodice 'giu310123
            SqlDSIndirCF.UpdateParameters.Item("Tipo").DefaultValue = Mid(txtTipo.Text.Trim, 1, 5)
            SqlDSIndirCF.UpdateParameters.Item("Ragione_Sociale").DefaultValue = Mid(txtragSoc.Text.Trim, 1, 50)
            SqlDSIndirCF.UpdateParameters.Item("Denominazione").DefaultValue = Mid(txtDenominazione.Text.Trim, 1, 50)
            SqlDSIndirCF.UpdateParameters.Item("Riferimento").DefaultValue = Mid(txtRiferimento.Text.Trim, 1, 500)
            '-----
            SqlDSIndirCF.UpdateParameters.Item("Indirizzo").DefaultValue = Mid(txtIndirizzo.Text.Trim, 1, 50)
            SqlDSIndirCF.UpdateParameters.Item("Cap").DefaultValue = Mid(txtCap.Text.Trim, 1, 5)
            SqlDSIndirCF.UpdateParameters.Item("Localita").DefaultValue = Mid(txtLocalita.Text.Trim, 1, 50)
            SqlDSIndirCF.UpdateParameters.Item("Provincia").DefaultValue = Mid(txtProvincia.Text.Trim, 1, 2)
            SqlDSIndirCF.UpdateParameters.Item("Stato").DefaultValue = Mid(txtCodNazione.Text.Trim, 1, 3)
            SqlDSIndirCF.UpdateParameters.Item("Telefono1").DefaultValue = Mid(txtTel1.Text.Trim, 1, 30)
            SqlDSIndirCF.UpdateParameters.Item("Telefono2").DefaultValue = Mid(txtTel2.Text.Trim, 1, 30)
            SqlDSIndirCF.UpdateParameters.Item("Fax").DefaultValue = Mid(txtFax.Text.Trim, 1, 30)
            SqlDSIndirCF.UpdateParameters.Item("EMail").DefaultValue = Mid(txtEMail.Text.Trim, 1, 100)
            SqlDSIndirCF.Update()
            SqlDSIndirCF.DataBind()
            'giu290819
            GridViewBody.DataBind()
            If GridViewBody.Rows.Count > 0 Then
                GridViewBody.SelectedIndex = 0
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
            Else
                GridViewBody.SelectedIndex = -1
            End If
            '---------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        CampiSetEnabledTo(False)
        setPulsantiModalitaConsulta()
        WucElement.setPulsantiModalitaSbloccoByWUC()
        GridViewBody.Enabled = True
        Session(SWMYOP) = SWOPNESSUNA
    End Sub

    Private Function CheckNew() As Boolean
        CheckNew = True
        Dim dv As DataView
        dv = SqlDSIndirCF.Select(DataSourceSelectArguments.Empty)
        If (dv Is Nothing) Then
            Exit Function
        End If
        If dv.Count > 0 Then
            dv.RowFilter = "Tipo = '" & txtTipo.Text.Trim & "'"
            If dv.Count > 0 Then
                CheckNew = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice Sigla già presente.", WUC_ModalPopup.TYPE_ALERT)
            Else
                Exit Function
            End If
        Else
            Exit Function
        End If
       
    End Function

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        SvuotaCampi()
        CampiSetEnabledTo(True)
        WucElement.GetDatiClientiByWUC(txtragSoc.Text, txtProvincia.Text, txtCodNazione.Text)
        setPulsantiModalitaAggiorna()
        WucElement.setPulsantiModalitaBloccoByWUC("DAAD")
        GridViewBody.Enabled = False
        Session(SWMYOP) = SWOPNUOVO
        txtTipo.Focus()
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        'giu290819
        If GridViewBody.Rows.Count > 0 Then
            GridViewBody_SelectedIndexChanged(Nothing, Nothing)
        Else
            SvuotaCampi()
            GridViewBody.SelectedIndex = -1
            Exit Sub
        End If
        '---------
        Try
            SqlDSIndirCF.DeleteParameters.Item("Codice").DefaultValue = Session(CSTCODCOGEDM)
            SqlDSIndirCF.DeleteParameters.Item("Tipo").DefaultValue = Mid(txtTipo.Text.Trim, 1, 5)
            SqlDSIndirCF.Delete()
            SqlDSIndirCF.DataBind()
            'giu290819
            GridViewBody.DataBind()
            If GridViewBody.Rows.Count > 0 Then
                GridViewBody.SelectedIndex = 0
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
            Else
                SvuotaCampi()
                GridViewBody.SelectedIndex = -1
            End If
            '---------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu290819 SvuotaCampi()
        setPulsantiModalitaBlocco()
        btnNuovo.Enabled = True 'giu290819
        GridViewBody.Enabled = True
    End Sub

    Private Sub GridViewBody_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.Sorted
        SvuotaCampi()
        Session(PROGRKEY) = ""
        GridViewBody.SelectedIndex = -1
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = False
        '---------
    End Sub
End Class