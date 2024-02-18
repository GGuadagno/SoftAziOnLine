Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_DestMerce
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

    Private Const F_NAZIONI As String = "ElencoNazioniDestMerce"
    Private Const F_PROVINCE As String = "ElencoProvinceDestMerce"
    Private Const SWMYOP As String = "SWOperazioneDestMerce"
    Private Const PROGRKEY As String = "ProgresssivoDestMerce"

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSDestMerce.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        If (Not IsPostBack) Then
            CampiSetEnabledTo(False)
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            If GridViewBody.Rows.Count = 0 Then
                SvuotaCampi()
            Else
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
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
        Session(PROGRKEY) = "0"
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
    'giu130714 i dati dalDB e non dalla grid altrimenti i caratteri speciali vengono convertiti in HTML
    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Try
            Session(PROGRKEY) = GridViewBody.SelectedDataKey.Value
            PopolaCampiGridSel()
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
    Private Sub PopolaCampiGridSel()
        'GIU151117
        Try
            Dim strSQL As String = ""
            strSQL = "Select * From DestClienti WHERE Codice = '" & Session(CSTCODCOGEDM).ToString.Trim & "' AND Progressivo = " & CLng(Session(PROGRKEY)) & ""
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Dim row() As DataRow
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    ' ''GetDatiDitta.Descrizione = IIf(IsDBNull(row(0).Item("Descrizione")), "", row(0).Item("Descrizione"))
                    'ok posso visualizzare
                    If Not IsDBNull(row(0).Item("Tipo")) Then
                        txtTipo.Text = row(0).Item("Tipo").ToString.Trim
                    Else
                        txtTipo.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Ragione_Sociale")) Then
                        txtRagSoc.Text = row(0).Item("Ragione_Sociale").ToString.Trim
                    Else
                        txtRagSoc.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Denominazione")) Then
                        txtDenominazione.Text = row(0).Item("Denominazione").ToString.Trim
                    Else
                        txtDenominazione.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Riferimento")) Then
                        txtRiferimento.Text = row(0).Item("Riferimento").ToString.Trim
                    Else
                        txtRiferimento.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Ragione_Sociale35")) Then
                        txtRagSoc35.Text = row(0).Item("Ragione_Sociale35").ToString.Trim
                    Else
                        txtRagSoc35.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Riferimento35")) Then
                        txtRiferimento35.Text = row(0).Item("Riferimento35").ToString.Trim
                    Else
                        txtRiferimento35.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Indirizzo")) Then
                        txtIndirizzo.Text = row(0).Item("Indirizzo").ToString.Trim
                    Else
                        txtIndirizzo.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("CAP")) Then
                        txtCap.Text = row(0).Item("CAP").ToString.Trim
                    Else
                        txtCap.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Localita")) Then
                        txtLocalita.Text = row(0).Item("Localita").ToString.Trim
                    Else
                        txtLocalita.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Provincia")) Then
                        txtProvincia.Text = row(0).Item("Provincia").ToString.Trim
                    Else
                        txtProvincia.Text = ""
                    End If
                    txtProvincia.ToolTip = App.GetValoreFromChiave(txtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO))
                    If Not IsDBNull(row(0).Item("Stato")) Then
                        txtCodNazione.Text = row(0).Item("Stato").ToString.Trim
                    Else
                        txtCodNazione.Text = ""
                    End If
                    txtCodNazione.ToolTip = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
                    If Not IsDBNull(row(0).Item("Telefono1")) Then
                        txtTel1.Text = row(0).Item("Telefono1").ToString.Trim
                    Else
                        txtTel1.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Telefono2")) Then
                        txtTel2.Text = row(0).Item("Telefono2").ToString.Trim
                    Else
                        txtTel2.Text = ""
                    End If
                    'giu180722
                    lblMessTel.Visible = False
                    txtTel1.BackColor = SEGNALA_OK : txtTel2.BackColor = SEGNALA_OK
                    Dim CKTelefono As Boolean = True
                    If (String.IsNullOrEmpty(txtTel1.Text.Trim) And String.IsNullOrEmpty(txtTel2.Text.Trim)) Then
                        CKTelefono = False
                    ElseIf txtTel1.Text.Trim = "" And txtTel2.Text.Trim = "" Then
                        CKTelefono = False
                    End If
                    If CKTelefono = False Then
                        If WucElement.CKTelefono() = False Then
                            lblMessTel.Visible = True
                            txtTel1.BackColor = SEGNALA_KO : txtTel2.BackColor = SEGNALA_KO
                        End If
                    End If
                    '-----------
                    If Not IsDBNull(row(0).Item("Fax")) Then
                        txtFax.Text = row(0).Item("Fax").ToString.Trim
                    Else
                        txtFax.Text = ""
                    End If
                    If Not IsDBNull(row(0).Item("Email")) Then
                        txtEMail.Text = row(0).Item("Email").ToString.Trim
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
        txtRagSoc35.Enabled = valore
        txtRiferimento35.Enabled = valore
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
        WucElement.setPulsantiModalitaSbloccoByWUC() 'giu130714
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
        txtRagSoc35.Text = ""
        txtRiferimento35.Text = ""
        txtIndirizzo.Text = ""
        txtCap.Text = ""
        txtLocalita.Text = ""
        txtProvincia.Text = ""
        txtProvincia.BackColor = SEGNALA_OK
        txtCodNazione.Text = ""
        txtCodNazione.BackColor = SEGNALA_OK
        txtCodNazione.ToolTip = ""
        txtTel1.Text = ""
        txtTel1.BackColor = SEGNALA_OK
        txtTel2.Text = ""
        txtTel2.BackColor = SEGNALA_OK
        txtFax.Text = ""
        txtEMail.Text = ""
    End Sub
    'GIU290323 PER RICARICARE I DATI DA GESTIONE ANAGRAFICA LA PRIMA VOLTA NON LO CARICA
    Public Sub CaricaDest()
        Try
            CampiSetEnabledTo(False)
            setPulsantiModalitaBlocco()
            btnNuovo.Enabled = True
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            SqlDSDestMerce.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SqlDSDestMerce.DataBind()
            GridViewBody.DataBind()
            If GridViewBody.Rows.Count > 0 Then
                GridViewBody.SelectedIndex = 0
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
            Else
                SvuotaCampi()
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
        WucElement.setPulsantiModalitaBloccoByWUC("DM")
        GridViewBody.Enabled = False
        txtTipo.Enabled = False
        Session(SWMYOP) = SWOPMODIFICA
        txtRagSoc.Focus()
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        'giu290819
        Session(PROGRKEY) = "0"
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
            txtTipo.Enabled = True
            txtTipo.BackColor = SEGNALA_KO : txtTipo.ToolTip = "Codice obbligatorio" : SWErr = True
        ElseIf Not IsNumeric(txtTipo.Text.Trim) Then
            txtTipo.Enabled = True
            txtTipo.BackColor = SEGNALA_KO : txtTipo.ToolTip = "Codice dev'essere solo numerico" : SWErr = True
        Else
            txtTipo.BackColor = SEGNALA_OK : txtTipo.ToolTip = ""
        End If
        If Session(SWMYOP) = SWOPNUOVO Then
            Session(PROGRKEY) = "-1"
            If CheckNew() = False Then
                txtTipo.Enabled = True
                txtTipo.BackColor = SEGNALA_KO : SWErr = True
            End If
        End If
        'GIU170423 BLOCCO SOLO PER I CLIENTI
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
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Destinazione Merce", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf Left(myCodice, 1) <> "1" And Left(myCodice, 1) <> "9" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Codice Cliente/Fornitore non valido, Sessione scaduta, si prega di riprovare a inserire Destinazione Merce", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '------------
        If txtragSoc.Text.Trim = "" Then
            txtragSoc.BackColor = SEGNALA_KO : SWErr = True
        Else
            txtragSoc.BackColor = SEGNALA_OK
        End If
        ''giu210423 tolto obbligo email Ilaria del 21/04 GIU170423 EMAIL 12/04 ILARIA
        '''If Left(myCodice, 1) = "1" Then
        '''    If txtRagSoc35.Text.Trim = "" Then
        '''        txtRagSoc35.BackColor = SEGNALA_KO : SWErr = True
        '''    Else
        '''        txtRagSoc35.BackColor = SEGNALA_OK
        '''    End If
        '''    If txtRiferimento35.Text.Trim = "" Then
        '''        txtRiferimento35.BackColor = SEGNALA_KO : SWErr = True
        '''    Else
        '''        txtRiferimento35.BackColor = SEGNALA_OK
        '''    End If
        '''End If
        '-------------------
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
        'giu180722
        lblMessTel.Visible = False
        txtTel1.BackColor = SEGNALA_OK : txtTel2.BackColor = SEGNALA_OK
        Dim CKTelefono As Boolean = True
        If (String.IsNullOrEmpty(txtTel1.Text.Trim) And String.IsNullOrEmpty(txtTel2.Text.Trim)) Then
            CKTelefono = False
        ElseIf txtTel1.Text.Trim = "" And txtTel2.Text.Trim = "" Then
            CKTelefono = False
        End If
        If CKTelefono = False Then
            If WucElement.CKTelefono() = False Then
                lblMessTel.Visible = True
                txtTel1.BackColor = SEGNALA_KO : txtTel2.BackColor = SEGNALA_KO
            End If
        End If
        '-----------
        If SWErr = True Then Exit Sub

        Try
            SqlDSDestMerce.UpdateParameters.Item("Progressivo").DefaultValue = CLng(Session(PROGRKEY))
            SqlDSDestMerce.UpdateParameters.Item("Codice").DefaultValue = myCodice 'GIU310123 Session(CSTCODCOGEDM)
            SqlDSDestMerce.UpdateParameters.Item("Tipo").DefaultValue = Mid(txtTipo.Text.Trim, 1, 5)
            SqlDSDestMerce.UpdateParameters.Item("Ragione_Sociale").DefaultValue = Mid(txtRagSoc.Text.Trim, 1, 50)
            SqlDSDestMerce.UpdateParameters.Item("Denominazione").DefaultValue = Mid(txtDenominazione.Text.Trim, 1, 50)
            SqlDSDestMerce.UpdateParameters.Item("Riferimento").DefaultValue = Mid(txtRiferimento.Text.Trim, 1, 500)
            SqlDSDestMerce.UpdateParameters.Item("Indirizzo").DefaultValue = Mid(txtIndirizzo.Text.Trim, 1, 50)
            SqlDSDestMerce.UpdateParameters.Item("Cap").DefaultValue = Mid(txtCap.Text.Trim, 1, 5)
            SqlDSDestMerce.UpdateParameters.Item("Localita").DefaultValue = Mid(txtLocalita.Text.Trim, 1, 50)
            SqlDSDestMerce.UpdateParameters.Item("Provincia").DefaultValue = Mid(txtProvincia.Text.Trim, 1, 2)
            SqlDSDestMerce.UpdateParameters.Item("Stato").DefaultValue = Mid(txtCodNazione.Text.Trim, 1, 3)
            SqlDSDestMerce.UpdateParameters.Item("Telefono1").DefaultValue = Mid(txtTel1.Text.Trim, 1, 30)
            SqlDSDestMerce.UpdateParameters.Item("Telefono2").DefaultValue = Mid(txtTel2.Text.Trim, 1, 30)
            SqlDSDestMerce.UpdateParameters.Item("Fax").DefaultValue = Mid(txtFax.Text.Trim, 1, 30)
            SqlDSDestMerce.UpdateParameters.Item("EMail").DefaultValue = Mid(txtEMail.Text.Trim, 1, 100)
            SqlDSDestMerce.UpdateParameters.Item("Ragione_Sociale35").DefaultValue = Mid(txtRagSoc35.Text.Trim, 1, 35)
            SqlDSDestMerce.UpdateParameters.Item("Riferimento35").DefaultValue = Mid(txtRiferimento35.Text.Trim, 1, 35)
            SqlDSDestMerce.Update()
            SqlDSDestMerce.DataBind()
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
        'giu030714 WucElement.setPulsantiModalitaSbloccoByWUC()
        WucElement.setPulsantiModalitaConsulta()
        '------------------------------------------------------
        GridViewBody.Enabled = True
        Session(SWMYOP) = SWOPNESSUNA
    End Sub

    Private Function CheckNew() As Boolean
        CheckNew = True
        'GIU151117
        Try
            Dim strSQL As String = ""
            strSQL = "Select * From DestClienti WHERE Codice = '" & Session(CSTCODCOGEDM).ToString.Trim & "' AND Tipo = '" & Val(txtTipo.Text.Trim) & "'"
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Dim row() As DataRow
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CheckNew = False
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Codice Sigla già presente.", WUC_ModalPopup.TYPE_ALERT)
                End If
            End If
        Catch ex As Exception
            CheckNew = False
            SvuotaCampi()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Non trovato in tabella l'elemento selezionato. Errore:" & vbCrLf & _
                            ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End Try
    End Function

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        SvuotaCampi()
        WucElement.GetDatiClientiByWUC(txtRagSoc.Text, txtProvincia.Text, txtCodNazione.Text)
        CampiSetEnabledTo(True)
        setPulsantiModalitaAggiorna()
        WucElement.setPulsantiModalitaBloccoByWUC("DM")
        GridViewBody.Enabled = False
        Session(SWMYOP) = SWOPNUOVO
        txtTipo.Text = GetNewTipo().ToString.Trim
        If txtTipo.Text.Trim = "0" Then
            txtTipo.Focus()
        Else
            txtRagSoc.Focus()
        End If
    End Sub
    Private Function GetNewTipo() As Long
        GetNewTipo = 0
        'GIU270819
        Try
            Dim strSQL As String = ""
            strSQL = "Select MAX(CONVERT(INT, Tipo)) AS Numero From DestClienti WHERE Codice = '" & Session(CSTCODCOGEDM).ToString.Trim & "'"
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati. Errore:" & vbCrLf & _
                            ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End Try
    End Function

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
        If CheckElimina() = False Then
            Exit Sub
        End If
        Try
            SqlDSDestMerce.DeleteParameters.Item("Progressivo").DefaultValue = CLng(Session(PROGRKEY))
            SqlDSDestMerce.Delete()
            SqlDSDestMerce.DataBind()
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
    Private Function CheckElimina() As Boolean
        CheckElimina = True
        'GIU270819
        Try
            Dim strSQL As String = ""
            strSQL = "Select TOP 1 * From DocumentiT WHERE Cod_Filiale =" & CLng(Session(PROGRKEY))
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CheckElimina = False
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella gestione aziendale, <br> impossibile eliminare", WUC_ModalPopup.TYPE_ALERT)
                End If
            End If
        Catch ex As Exception
            CheckElimina = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Non trovato in tabella l'elemento selezionato. Errore:" & vbCrLf & _
                            ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End Try
    End Function

    Private Sub GridViewBody_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.Sorted
        SvuotaCampi()
        Session(PROGRKEY) = "0"
        GridViewBody.SelectedIndex = -1
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnAnnulla.Enabled = False
        '---------
    End Sub
End Class