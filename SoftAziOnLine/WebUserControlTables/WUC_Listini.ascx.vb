Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_Listini
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSListini.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSValuta.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSPagamenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCategorie.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSArtLis.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----CategArt
        SqlDSArtIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSArtOu.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        If (Not IsPostBack) Then
            Try

                Session("SortListVenD") = "C"

                DDLTipoLis.Items.Add("Normale")
                DDLTipoLis.Items(1).Value = 1
                DDLTipoLis.Items.Add("Categoria")
                DDLTipoLis.Items(2).Value = 2
                DDLTipoLis.Items.Add("Cliente")
                DDLTipoLis.Items(3).Value = 3

                ddlRicerca.Items.Add("Codice")
                ddlRicerca.Items(0).Value = "C"
                ddlRicerca.Items.Add("Descrizione")
                ddlRicerca.Items(1).Value = "D1"
                ddlRicerca.Items.Add("Descrizione (Parole contenute)")
                ddlRicerca.Items(2).Value = "D2"
                ddlRicerca.Items.Add("Codice (Senza prezzo)")
                ddlRicerca.Items(3).Value = "C1"
                ddlRicerca.Items.Add("Descrizione (Senza prezzo)")
                ddlRicerca.Items(4).Value = "D3"

                ddlRicercaArtIn.Items.Add("Codice")
                ddlRicercaArtIn.Items(0).Value = "C"
                ddlRicercaArtIn.Items.Add("Descrizione")
                ddlRicercaArtIn.Items(1).Value = "D1"
                ddlRicercaArtIn.Items.Add("Descrizione (Parole contenute)")
                ddlRicercaArtIn.Items(2).Value = "D2"
                ddlRicercaArtIn.Items.Add("Codice (Senza prezzo)")
                ddlRicercaArtIn.Items(3).Value = "C2"
                ddlRicercaArtIn.Items.Add("Descrizione (Senza prezzo)")
                ddlRicercaArtIn.Items(4).Value = "D3"

                DDLRicercaArtOu.Items.Add("Codice")
                DDLRicercaArtOu.Items(0).Value = "C"
                DDLRicercaArtOu.Items.Add("Descrizione")
                DDLRicercaArtOu.Items(1).Value = "D1"
                DDLRicercaArtOu.Items.Add("Descrizione (Parole contenute)")
                DDLRicercaArtOu.Items(2).Value = "D2"

                CampiSetEnabledTo(False)
                BtnSetEnabledTo(True)
                btnAggiorna.Enabled = False
                btnAnnulla.Enabled = False
                btnElimina.Visible = True
                btnEliminaArt.Visible = False
                btnIncludiArt.Visible = False : btnIncludiAll.Visible = False
                If GridViewListini.Rows.Count > 0 Then
                    GridViewListini.SelectedIndex = 0
                    Session(IDLISTINO) = GridViewListini.SelectedDataKey.Value
                    LTSelezionato(Session(IDLISTINO))
                    ' ''quin non leggi i dettagli devo aspettare che venga ricaricato il dettaglio
                Else
                    Session(IDLISTINO) = "-1" 'SOLO LA PRIMA VOLTA QUANDO LA TABELLA è VUOTA
                End If
                TabPanel4.Visible = False 'IN SVILUPPO
                Session(SWOP) = SWOPNESSUNA
                Session("TabListini") = TB0
                TabContainer1.ActiveTabIndex = 0
            Catch Ex As Exception
                Chiudi("Errore: Caricamento Elenco Listini: " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        WFPElencoCli.WucElement = Me
        WUC_SceltaOrdinamentoRiepListino1.WucElement = Me
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            WFPElencoCli.Show()
        End If
    End Sub
    Public Sub Chiudi(ByVal strErrore As String)
       
        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale" & strErrore)
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
        Dim strRitorno As String = ""
        strRitorno = ""
        If strErrore.Trim = "" Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub
#Region "Gestione Grid"

    Private Sub GridViewListini_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewListini.RowDataBound
        e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewListini, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(4).Text) Then
                e.Row.Cells(4).Text = Format(CDate(e.Row.Cells(4).Text), FormatoData).ToString
            End If
        End If
    End Sub

    Protected Sub GridViewListini_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewListini.SelectedIndexChanged
        Session(IDLISTINO) = GridViewListini.SelectedDataKey.Value
        LTSelezionato(Session(IDLISTINO))
        ' ''quin non leggi i dettagli devo aspettare che venga ricaricato il dettaglio
    End Sub

    Private Sub GridViewArtLis_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewArtLis.RowDataBound
        ' ''e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewArtLis, "Select$" + e.Row.RowIndex.ToString()))
        ' ''Non cancellare serve 
        ' ''If e.Row.DataItemIndex > -1 Then
        ' ''    If IsNumeric(e.Row.Cells(3).Text) Then
        ' ''        e.Row.Cells(3).Text = Format(CDbl(e.Row.Cells(3).Text), FormatoValMag).ToString
        ' ''    End If
        ' ''End If
    End Sub
    Private Sub GridViewArtLis_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtLis.SelectedIndexChanged

        LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
    End Sub

    Private Sub GridViewArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtIn.SelectedIndexChanged
        ' ''btnEliminaArt.Enabled = True
        Dim row As GridViewRow = GridViewArtIn.Rows(GridViewArtIn.SelectedIndex)
        Dim strDesArt As String = row.Cells(2).Text
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaLTDIn"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo ? " & vbCrLf & strDesArt.Trim, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub

    Private Sub GridViewArtOu_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtOu.SelectedIndexChanged
        ' ''btnIncludiArt.Enabled = True
        Dim SWErr As Boolean = False
        Try
            SqlDSArtOu.InsertParameters.Item("CodLis").DefaultValue = Int(txtCodice.Text.Trim)
            SqlDSArtOu.InsertParameters.Item("Cod_Articolo").DefaultValue = GridViewArtOu.SelectedDataKey.Value.ToString.Trim
            SqlDSArtOu.InsertParameters.Item("Prezzo").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("Sconto_1").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("Sconto_2").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("PrezzoMinimo").DefaultValue = CDbl(0)
            SqlDSArtOu.Insert()
            ' ''If SqlDSArtOu.InsertParameters.Item("RetVal").DefaultValue < 1 Then
            ' ''    SWErr = True
            ' ''End If
            SqlDSArtOu.DataBind()
            SqlDSArtIn.DataBind()
            GridViewArtOu.DataBind()
            GridViewArtIn.DataBind()
            SqlDSArtLis.DataBind()
            GridViewArtLis.DataBind()
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            If SWErr = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Operazione di Includi articolo fallita", WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtIn.SelectedIndex = 0
            btnEliminaArt.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtIn.SelectedDataKey.Value.ToString.Trim)
        Else
            btnEliminaArt.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtOu.SelectedIndex = 0
            btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtOu.SelectedDataKey.Value.ToString.Trim)
        Else
            btnIncludiArt.Enabled = False : btnIncludiAll.Enabled = False
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
#End Region

#Region "Gestione BTN"

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session(SWOP) = SWOPMODIFICA : Session(SWMODIFICATO) = SWNO
        If Session("TabListini") = TB0 Then
            GridViewListini.Enabled = False
            CampiSetEnabledTo(True)
            BtnSetEnabledTo(False)
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
        ElseIf Session("TabListini") = TB1 Then
            GridViewArtLis.Enabled = False
            CampiSetEnabledToLTD(True)
            BtnSetEnabledTo(False)
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            txtPrezzo.Focus()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("MODIFICA", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
        End If

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click

        If Session("TabListini") = TB0 Then
            If Session(SWMODIFICATO) = SWSI Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "ConfAnnullaModLT"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Modifiche Listino", "Si vuole annullare le modifiche?", WUC_ModalPopup.TYPE_CONFIRM)
            Else
                Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
                ConfAnnullaModLT()
            End If
        ElseIf Session("TabListini") = TB1 Then
            If Session(SWMODIFICATO) = SWSI Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "ConfAnnullaModLTD"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Modifiche articoli nel Listino", "Si vuole annullare le modifiche?", WUC_ModalPopup.TYPE_CONFIRM)
            Else
                Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
                ConfAnnullaModLTD()
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("ANNULLA", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Public Sub ConfAnnullaModLT()
        GridViewListini.Enabled = True
        CampiSetEnabledTo(False)
        BtnSetEnabledTo(True)
        btnAggiorna.Enabled = False
        btnAnnulla.Enabled = False
        If GridViewListini.Rows.Count > 0 Then
            GridViewListini.SelectedIndex = 0
            Session(IDLISTINO) = GridViewListini.SelectedDataKey.Value
            If Session(IDLISTINO) = 1 Then
                btnElimina.Enabled = False : btnEliminaArt.Enabled = False
            End If
            LTSelezionato(Session(IDLISTINO))
            ' ''quin non leggi i dettagli devo aspettare che venga ricaricato il dettaglio
            ' ''If GridViewArtLis.Rows.Count > 0 Then
            ' ''    GridViewArtLis.SelectedIndex = 0
            ' ''    LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
            ' ''Else
            ' ''    AzzeraCampiLTD()
            ' ''End If
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraCampiLTD()
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
    Public Sub ConfAnnullaModLTD()
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        GridViewArtLis.Enabled = True
        CampiSetEnabledToLTD(False)
        BtnSetEnabledTo(True)
        btnAggiorna.Enabled = False
        btnAnnulla.Enabled = False
        If GridViewListini.Rows.Count > 0 And GridViewArtLis.Rows.Count > 0 Then
            GridViewArtLis.SelectedIndex = 0
            LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value)
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraCampiLTD()
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        If TabContainer1.ActiveTabIndex = TB0 Then
            AggiornaLT()
            Exit Sub
        ElseIf TabContainer1.ActiveTabIndex = TB1 Then
            AggiornaLTD()
            Exit Sub
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Private Function AggiornaLT() As Boolean
        If ControllaCampi() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati listino", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        Dim SWErr As Boolean = False
        Try
            ' ''-InsertUpdate è uguale
            If Session(SWOP) = SWOPMODIFICA Then
                SqlDSListini.UpdateParameters.Item("Codice").DefaultValue = Int(txtCodice.Text.Trim)
                SqlDSListini.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
                SqlDSListini.UpdateParameters.Item("Tipo").DefaultValue = DDLTipoLis.SelectedValue
                SqlDSListini.UpdateParameters.Item("Data_Inizio_Validita").DefaultValue = CDate(txtDataVal.Text.Trim)
                SqlDSListini.UpdateParameters.Item("Abilitato").DefaultValue = CBool(ChkAbilitato.Checked)
                SqlDSListini.UpdateParameters.Item("Valuta").DefaultValue = DDLValuta.SelectedValue
                If DDLPagamento.SelectedIndex <> 0 Then
                    SqlDSListini.UpdateParameters.Item("Cod_Pagamento").DefaultValue = DDLPagamento.SelectedValue
                End If
                If DDLCategoria.SelectedIndex <> 0 Then
                    SqlDSListini.UpdateParameters.Item("Categoria").DefaultValue = DDLCategoria.SelectedValue
                End If
                SqlDSListini.UpdateParameters.Item("Cliente").DefaultValue = lblCodCliFor.Text.Trim
                If txtNote.Text.Trim <> "" Then
                    SqlDSListini.UpdateParameters.Item("Note").DefaultValue = Mid(txtNote.Text.Trim, 1, 50)
                End If
                SqlDSListini.Update()
                ' ''If SqlDSListini.UpdateParameters.Item("RetVal").DefaultValue < 1 Then
                ' ''    SWErr = True
                ' ''End If
                SqlDSListini.DataBind()
                AggiornaLT = True
            Else
                SqlDSListini.InsertParameters.Item("Codice").DefaultValue = Int(txtCodice.Text.Trim)
                SqlDSListini.InsertParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
                SqlDSListini.InsertParameters.Item("Tipo").DefaultValue = DDLTipoLis.SelectedValue
                SqlDSListini.InsertParameters.Item("Data_Inizio_Validita").DefaultValue = CDate(txtDataVal.Text.Trim)
                SqlDSListini.InsertParameters.Item("Abilitato").DefaultValue = CBool(ChkAbilitato.Checked)
                SqlDSListini.InsertParameters.Item("Valuta").DefaultValue = DDLValuta.SelectedValue
                If DDLPagamento.SelectedIndex <> 0 Then
                    SqlDSListini.InsertParameters.Item("Cod_Pagamento").DefaultValue = DDLPagamento.SelectedValue
                End If
                If DDLCategoria.SelectedIndex <> 0 Then
                    SqlDSListini.InsertParameters.Item("Categoria").DefaultValue = DDLCategoria.SelectedValue
                End If
                SqlDSListini.InsertParameters.Item("Cliente").DefaultValue = lblCodCliFor.Text.Trim
                If txtNote.Text.Trim <> "" Then
                    SqlDSListini.InsertParameters.Item("Note").DefaultValue = Mid(txtNote.Text.Trim, 1, 50)
                End If
                SqlDSListini.Insert()
                ' ''If SqlDSListini.InsertParameters.Item("RetVal").DefaultValue < 1 Then
                ' ''    SWErr = True
                ' ''End If
                SqlDSListini.DataBind()
                AggiornaLT = True
            End If
            Session(IDLISTINO) = txtCodice.Text.Trim
            TabPanel2.HeaderText = "Articoli presenti nel listino: (" & txtCodice.Text.Trim & ")"
            TabPanel3.HeaderText = "Includi/Escludi articoli dal listino: (" & txtCodice.Text.Trim & ")"
            TabPanel4.HeaderText = "Duplica listino: (" & txtCodice.Text.Trim & ")"

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            If SWErr = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Operazione di Inserimento/Aggiornamento fallita", WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Function
        End If
        '-----------
        GridViewListini.Enabled = True
        If GridViewListini.Rows.Count > 0 Then
            CampiSetEnabledTo(False)
            BtnSetEnabledTo(True)
            If Session(IDLISTINO) = 1 Then
                btnElimina.Enabled = False : btnEliminaArt.Enabled = False
            End If
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
    End Function
    Private Function AggiornaLTD() As Boolean
        Dim SWPrezzo As Boolean = False : Dim SWPrezzoMin As Boolean = False
        If ControllaCampiLTD(SWPrezzo, SWPrezzoMin) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati listino", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        Else
            If SWPrezzo = True And SWPrezzoMin = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AggiornaLTDOK"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati listino", "Prezzo di listino uguale a zero, proseguo ?", WUC_ModalPopup.TYPE_CONFIRM)
            Else
                AggiornaLTDOK()
            End If
        End If


    End Function
    Public Sub AggiornaLTDOK()
        Dim objLTDCampiCambio As New LTDCampiCambio
        If Not Session("LTDCampiCambio") Is Nothing Then
            objLTDCampiCambio = Session("LTDCampiCambio")
        End If

        Dim SWErr As Boolean = False
        Try
            ' ''-InsertUpdate è uguale
            If Session(SWOP) = SWOPMODIFICA Then
                SqlDSArtLis.UpdateParameters.Item("CodLis").DefaultValue = Int(txtCodice.Text.Trim)
                SqlDSArtLis.UpdateParameters.Item("Cod_Articolo").DefaultValue = lblCodArticolo.Text.Trim
                SqlDSArtLis.UpdateParameters.Item("Prezzo").DefaultValue = CDbl(IIf(txtPrezzo.Text.Trim = "", 0, txtPrezzo.Text))
                SqlDSArtLis.UpdateParameters.Item("Sconto_1").DefaultValue = CDbl(IIf(txtSconto1.Text.Trim = "", 0, txtSconto1.Text))
                SqlDSArtLis.UpdateParameters.Item("Sconto_2").DefaultValue = CDbl(IIf(txtSconto2.Text.Trim = "", 0, txtSconto2.Text))
                SqlDSArtLis.UpdateParameters.Item("PrezzoMinimo").DefaultValue = CDbl(IIf(txtPrezzoMinimo.Text.Trim = "", 0, txtPrezzoMinimo.Text))
                If Not Session("LTDCampiCambio") Is Nothing Then
                    SqlDSArtLis.UpdateParameters.Item("Prezzo_Valuta").DefaultValue = objLTDCampiCambio.Cambio * CDbl(txtPrezzo.Text)
                    SqlDSArtLis.UpdateParameters.Item("Cambio").DefaultValue = objLTDCampiCambio.Cambio
                    SqlDSArtLis.UpdateParameters.Item("Data_Cambio").DefaultValue = objLTDCampiCambio.Data_Cambio
                End If

                SqlDSArtLis.Update()
                ' ''If SqlDSArtLis.UpdateParameters.Item("RetVal").DefaultValue < 1 Then
                ' ''    SWErr = True
                ' ''End If
                SqlDSArtLis.DataBind()
            Else
                SqlDSArtLis.InsertParameters.Item("CodLis").DefaultValue = Int(txtCodice.Text.Trim)
                SqlDSArtLis.InsertParameters.Item("Cod_Articolo").DefaultValue = lblCodArticolo.Text.Trim
                SqlDSArtLis.InsertParameters.Item("Prezzo").DefaultValue = CDbl(IIf(txtPrezzo.Text.Trim = "", 0, txtPrezzo.Text))
                SqlDSArtLis.InsertParameters.Item("Sconto_1").DefaultValue = CDbl(IIf(txtSconto1.Text.Trim = "", 0, txtSconto1.Text))
                SqlDSArtLis.InsertParameters.Item("Sconto_2").DefaultValue = CDbl(IIf(txtSconto2.Text.Trim = "", 0, txtSconto2.Text))
                SqlDSArtLis.InsertParameters.Item("PrezzoMinimo").DefaultValue = CDbl(IIf(txtPrezzoMinimo.Text.Trim = "", 0, txtPrezzoMinimo.Text))
                If Not Session("LTDCampiCambio") Is Nothing Then
                    SqlDSArtLis.InsertParameters.Item("Prezzo_Valuta").DefaultValue = objLTDCampiCambio.Cambio * CDbl(IIf(txtPrezzo.Text.Trim = "", 0, txtPrezzo.Text))
                    SqlDSArtLis.InsertParameters.Item("Cambio").DefaultValue = objLTDCampiCambio.Cambio
                    SqlDSArtLis.InsertParameters.Item("Data_Cambio").DefaultValue = objLTDCampiCambio.Data_Cambio
                End If
                SqlDSArtLis.Insert()
                ' ''If SqlDSArtLis.InsertParameters.Item("RetVal").DefaultValue < 1 Then
                ' ''    SWErr = True
                ' ''End If
                SqlDSArtLis.DataBind()
            End If

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            If SWErr = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Operazione di Inserimento/Aggiornamento fallita", WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtLis.Enabled = True
        CampiSetEnabledToLTD(False)
        If GridViewArtLis.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
    End Sub
    Private Sub btnIncludiArt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnIncludiArt.Click
        Dim SWErr As Boolean = False
        Try
            SqlDSArtOu.InsertParameters.Item("CodLis").DefaultValue = Int(txtCodice.Text.Trim)
            SqlDSArtOu.InsertParameters.Item("Cod_Articolo").DefaultValue = GridViewArtOu.SelectedDataKey.Value.ToString.Trim
            SqlDSArtOu.InsertParameters.Item("Prezzo").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("Sconto_1").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("Sconto_2").DefaultValue = CDbl(0)
            SqlDSArtOu.InsertParameters.Item("PrezzoMinimo").DefaultValue = CDbl(0)
            SqlDSArtOu.Insert()
            ' ''If SqlDSArtOu.InsertParameters.Item("RetVal").DefaultValue < 1 Then
            ' ''    SWErr = True
            ' ''End If
            SqlDSArtOu.DataBind()
            SqlDSArtIn.DataBind()
            GridViewArtOu.DataBind()
            GridViewArtIn.DataBind()
            SqlDSArtLis.DataBind()
            GridViewArtLis.DataBind()
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            If SWErr = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Operazione di Includi articolo fallita", WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtIn.SelectedIndex = 0
            btnEliminaArt.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtIn.SelectedDataKey.Value.ToString.Trim)
        Else
            btnEliminaArt.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtOu.SelectedIndex = 0
            btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtOu.SelectedDataKey.Value.ToString.Trim)
        Else
            btnIncludiArt.Enabled = False : btnIncludiAll.Enabled = False
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnIncludiAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnIncludiAll.Click
        If GridViewArtOu.Rows.Count < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun articolo risulta escluso in questo listino.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        btnEliminaArt.Enabled = False : btnIncludiArt.Enabled = False : btnIncludiAll.Enabled = False
        GridViewArtIn.Enabled = False : GridViewArtOu.Enabled = False

        Dim LTDSys As New ListVenD
        If LTDSys.InsertiArticoliOu(Int(txtCodice.Text.Trim)) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Operazione di Includi tutti gli articoli fallita", WUC_ModalPopup.TYPE_INFO)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Includi tutti", "Operazione di Includi tutti gli articoli riuscita con successo", WUC_ModalPopup.TYPE_INFO)
        End If

        SqlDSArtOu.DataBind()
        SqlDSArtIn.DataBind()
        GridViewArtOu.DataBind()
        GridViewArtIn.DataBind()
        SqlDSArtLis.DataBind()
        GridViewArtLis.DataBind()
        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtIn.SelectedIndex = 0
            btnEliminaArt.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtIn.SelectedDataKey.Value.ToString.Trim)
        Else
            btnEliminaArt.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtOu.SelectedIndex = 0
            btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtOu.SelectedDataKey.Value.ToString.Trim)
        Else
            btnIncludiArt.Enabled = False : btnIncludiAll.Enabled = False
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If TabContainer1.ActiveTabIndex = TB0 Then
            If Int(GridViewListini.SelectedDataKey.Value) = 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Impossibile cancellare il listino base", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaLT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Confermi l'eliminazione del listino selezionato ? Attenzione saranno cancellati anche gli articoli collegati.", WUC_ModalPopup.TYPE_CONFIRM)
        ElseIf TabContainer1.ActiveTabIndex = TB1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaLTD"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("ELIMINA", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
        End If

    End Sub
    Private Sub btnEliminaArt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaArt.Click
        If TabContainer1.ActiveTabIndex = TB1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaLTD"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
        ElseIf TabContainer1.ActiveTabIndex = TB2 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaLTDIn"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub ConfEliminaLT()
        Session(SWOP) = SWOPELIMINA
        Try
            SqlDSListini.DeleteParameters.Item("Codice").DefaultValue = Int(GridViewListini.SelectedDataKey.Value)
            SqlDSListini.Delete()
            SqlDSListini.DataBind()
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Cancellazione Listino", "Operazione di cancellazione riuscita con successo", WUC_ModalPopup.TYPE_CONFIRM)
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewListini.Enabled = True
        GridViewListini.DataBind()
        If GridViewListini.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            '-
            GridViewListini.SelectedIndex = 0
            Session(IDLISTINO) = GridViewListini.SelectedDataKey.Value
            LTSelezionato(Session(IDLISTINO))
            ' ''quin non leggi i dettagli devo aspettare che venga ricaricato il dettaglio
            ' ''If GridViewArtLis.Rows.Count > 0 Then
            ' ''    GridViewArtLis.SelectedIndex = 0
            ' ''    LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
            ' ''Else
            ' ''    AzzeraCampiLTD()
            ' ''End If
        Else
            AzzeraCampi()
            AzzeraCampiLTD()
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
    Public Sub ConfEliminaLTD()
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        Session(SWOP) = SWOPELIMINA
        Try
            SqlDSArtLis.DeleteParameters.Item("CodLis").DefaultValue = Int(Session(IDLISTINO))
            SqlDSArtLis.DeleteParameters.Item("Cod_Articolo").DefaultValue = GridViewArtLis.SelectedDataKey.Value.ToString.Trim
            SqlDSArtLis.Delete()
            SqlDSArtLis.DataBind()
            GridViewArtLis.DataBind()
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Cancellazione Articolo", "Operazione di cancellazione riuscita con successo", WUC_ModalPopup.TYPE_CONFIRM)
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtLis.Enabled = True
        If GridViewArtLis.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            '-
            GridViewArtLis.SelectedIndex = 0
            LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
        Else
            AzzeraCampiLTD()
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
    Public Sub ConfEliminaLTDIn()
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        Session(SWOP) = SWOPELIMINA
        Try
            SqlDSArtIn.DeleteParameters.Item("CodLis").DefaultValue = Int(Session(IDLISTINO))
            SqlDSArtIn.DeleteParameters.Item("Cod_Articolo").DefaultValue = GridViewArtIn.SelectedDataKey.Value.ToString.Trim
            SqlDSArtIn.Delete()
            SqlDSArtIn.DataBind()
            GridViewArtIn.DataBind()
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Cancellazione Articolo", "Operazione di cancellazione riuscita con successo", WUC_ModalPopup.TYPE_CONFIRM)
            SqlDSArtOu.DataBind()
            GridViewArtOu.DataBind()
            '
            SqlDSArtLis.DataBind()
            GridViewArtLis.DataBind()
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtIn.SelectedIndex = 0
            btnEliminaArt.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtIn.SelectedDataKey.Value.ToString.Trim)
        Else
            btnEliminaArt.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewArtOu.SelectedIndex = 0
            btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = True
            ' ''LTDSelezionato(Session(IDLISTINO), GridViewArtIn.SelectedDataKey.Value.ToString.Trim)
        Else
            btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = False
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If TabContainer1.ActiveTabIndex = TB0 Then
            Session(SWOP) = SWOPNUOVO : Session(SWMODIFICATO) = SWNO

            GridViewListini.Enabled = False
            CampiSetEnabledTo(True)
            BtnSetEnabledTo(False)
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            AzzeraCampi()
            txtCodice.Focus()
        ElseIf TabContainer1.ActiveTabIndex = TB1 Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("NUOVO TB1", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
            ' ''Else
            ' ''    ModalPopup.Show("NUOVO", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        ' ''Session(SWOP) = SWOPSTAMPA
        WUC_SceltaOrdinamentoRiepListino1.Show()
    End Sub
    Public Sub CallBackTipoOrdine(ByVal TipoOrdine As String, ByVal CFornitore As String, ByVal DFornitore As String)
        'Roberto 07/12/2011---
        Dim dsListino1 As New DSListino
        Dim ObjReport As New Object
        Dim ClsPrint As New Listini
        Dim StrErrore As String = ""

        Dim SWSconti As Boolean = False
        Try
            Dim strNomeAz As String = Session(CSTAZIENDARPT)
            Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.Listini
            Dim IDLT As String = Session(IDLISTINO)
            If IsNothing(IDLT) Then IDLT = ""
            If String.IsNullOrEmpty(IDLT) Then IDLT = ""
            Dim TitoloRpt As String = "Riepilogo Listini"
            If CFornitore.Trim <> "" Then
                TitoloRpt += " - Fornitore: (" + CFornitore.Trim + ") - " + DFornitore.Trim
            End If
            If ClsPrint.StampaListino(strNomeAz, TitoloRpt, dsListino1, ObjReport, StrErrore, IDLT, TipoOrdine, CFornitore, DFornitore) Then
                If dsListino1.ListVenD.Rows.Count = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "CallStampa"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CancelStampa"
                    ModalPopup.Show("Stampa Listini di Vendita", "Nessun articolo selezionato", WUC_ModalPopup.TYPE_CONFIRM_YNA)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsListino1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebCR_Mag.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Listini.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub
    Public Sub CallStampa()
        WUC_SceltaOrdinamentoRiepListino1.Show()
    End Sub
    Public Sub CancelStampa()
        'nulla
    End Sub
    Private Sub btnStampaSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSc.Click
        ' ''Session(SWOP) = SWOPSTAMPA
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''ModalPopup.Show("STAMPA SC.", "IN FASE DI SVILUPPO", WUC_ModalPopup.TYPE_INFO)
    End Sub
#End Region

#Region "Gestione Controlli"
    Private Sub TabContainer1_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabContainer1.ActiveTabChanged
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        If Session(SWOP) = SWOPNUOVO Or Session(SWOP) = SWOPMODIFICA Then
            TabContainer1.ActiveTabIndex = Int(Session("TabListini"))
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TabListini") = TabContainer1.ActiveTabIndex
        TabPanel2.HeaderText = "Articoli presenti nel listino: (" & txtCodice.Text.Trim & ")"
        TabPanel3.HeaderText = "Includi/Escludi articoli dal listino: (" & txtCodice.Text.Trim & ")"
        TabPanel4.HeaderText = "Duplica listino: (" & txtCodice.Text.Trim & ")"
        If TabContainer1.ActiveTabIndex = TB0 Then
            btnNuovo.Visible = True
            btnModifica.Visible = True
            btnAggiorna.Visible = True
            btnAnnulla.Visible = True
            btnElimina.Visible = True : btnEliminaArt.Visible = False
            lblDescIncl.Visible = False 'alb080213
            'DA ATTIVARE btnStampa.Visible = True : btnStampaSc.Visible = True
            btnIncludiArt.Visible = False : btnIncludiAll.Visible = False
        ElseIf TabContainer1.ActiveTabIndex = TB1 Then
            btnNuovo.Visible = False
            btnModifica.Visible = True
            btnAggiorna.Visible = True
            btnAnnulla.Visible = True
            'giu290719
            btnModifica.Enabled = False
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            '---------
            btnElimina.Visible = False : btnEliminaArt.Visible = True
            'DA ATTIVARE btnStampa.Visible = True : btnStampaSc.Visible = True
            btnIncludiArt.Visible = False : btnIncludiAll.Visible = False
            lblDescIncl.Visible = False 'alb080213
            TabPanel2.HeaderText = "Articoli presenti nel listino: (" & txtCodice.Text.Trim & ") - " & Mid(txtDescrizione.Text.Trim, 1, 25)
            If GridViewListini.Rows.Count > 0 And GridViewArtLis.Rows.Count > 0 Then
                GridViewArtLis.SelectedIndex = 0
                LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
                btnEliminaArt.Enabled = True
                btnModifica.Enabled = True
                'DA ATTIVARE btnStampa.Enabled = True : btnStampaSc.Enabled = True
            Else
                AzzeraCampiLTD()
                btnEliminaArt.Enabled = False
                btnModifica.Enabled = False
                'DA ATTIVARE btnStampa.Enabled = False : btnStampaSc.Enabled = False
            End If
            CampiSetEnabledToLTD(False)
        ElseIf TabContainer1.ActiveTabIndex = TB2 Then
            'commentato da alb070213
            'ddlRicercaArtIn.Enabled = False : DDLRicercaArtOu.Enabled = False
            'txtRicercaArtIn.Enabled = False : txtRicercaArtOu.Enabled = False
            'btnRicercaArtIn.Enabled = False : btnRicercaArtOu.Enabled = False
            '
            ' ''ddlRicercaArtIn.Visible = False : DDLRicercaArtOu.Visible = False
            ' ''txtRicercaArtIn.Visible = False : txtRicercaArtOu.Visible = False
            ' ''btnRicercaArtIn.Visible = False : btnRicercaArtOu.Visible = False
            '----------------------------------------------------------------
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnAggiorna.Visible = False
            btnAnnulla.Visible = False
            btnElimina.Visible = False : btnEliminaArt.Visible = False : btnEliminaArt.Enabled = False
            'DA ATTIVARE btnStampa.Visible = False : btnStampaSc.Visible = False
            btnIncludiArt.Visible = False : btnIncludiArt.Enabled = False
            btnIncludiAll.Visible = False 'giu290719 True 
            btnIncludiAll.Enabled = False
            lblDescIncl.Visible = True 'alb080213
            TabPanel3.HeaderText = "Includi/Escludi articoli dal listino: (" & txtCodice.Text.Trim & ") - " & Mid(txtDescrizione.Text.Trim, 1, 25)
            SqlDSArtIn.DataBind() : SqlDSArtOu.DataBind()
            GridViewArtIn.DataBind() : GridViewArtOu.DataBind()
            If GridViewListini.Rows.Count > 0 And GridViewArtOu.Rows.Count > 0 Then
                GridViewArtOu.SelectedIndex = 0
                btnIncludiArt.Enabled = True : btnIncludiAll.Enabled = True
            End If

        ElseIf TabContainer1.ActiveTabIndex = TB3 Then
            btnNuovo.Visible = False : btnModifica.Visible = False
            btnElimina.Visible = False : btnEliminaArt.Visible = False
            lblDescIncl.Visible = False 'alb080213
            'DA ATTIVARE btnStampa.Visible = False : btnStampaSc.Visible = False
            btnIncludiArt.Visible = False : btnIncludiAll.Visible = False
            TabPanel4.HeaderText = "Duplica listino: (" & txtCodice.Text.Trim & ") - " & Mid(txtDescrizione.Text.Trim, 1, 25)
        End If

    End Sub

    Private Sub ChkPagamento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkPagamento.CheckedChanged
        If ChkPagamento.Checked = False Then
            txtPagamento.Enabled = True : DDLPagamento.Enabled = True
        Else
            txtPagamento.Text = "" : txtPagamento.Enabled = False
            DDLPagamento.SelectedIndex = 0 : DDLPagamento.Enabled = False
        End If
        Session(SWMODIFICATO) = SWSI
        txtCategoria.Focus()
    End Sub

    Private Sub ChkCategoria_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkCategoria.CheckedChanged
        If ChkCategoria.Checked = False Then
            txtCategoria.Enabled = True : DDLCategoria.Enabled = True
        Else
            txtCategoria.Text = "" : txtCategoria.Enabled = False
            DDLCategoria.SelectedIndex = 0 : DDLCategoria.Enabled = False
        End If
        Session(SWMODIFICATO) = SWSI
        txtNote.Focus()
    End Sub

    Private Sub ChkClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkClienti.CheckedChanged
        If ChkClienti.Checked = False Then
            btnCercaAnagrafica.Enabled = True
        Else
            btnCercaAnagrafica.Enabled = False
            lblCodCliFor.Text = "" : lblRagSoc.Text = HTML_SPAZIO
        End If
        Session(SWMODIFICATO) = SWSI
        txtNote.Focus()
    End Sub

    Private Sub DDLValuta_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLValuta.SelectedIndexChanged
        txtValuta.Text = DDLValuta.SelectedValue
        Session(SWMODIFICATO) = SWSI
        txtPagamento.Focus()
    End Sub

    Private Sub DDLPagamento_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLPagamento.SelectedIndexChanged
        txtPagamento.Text = DDLPagamento.SelectedValue
        Session(SWMODIFICATO) = SWSI
        txtCategoria.Focus()
    End Sub

    Private Sub DDLCategoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCategoria.SelectedIndexChanged
        txtCategoria.Text = DDLCategoria.SelectedValue
        Session(SWMODIFICATO) = SWSI
        txtNote.Focus()
    End Sub

    Private Sub ChkAbilitato_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkAbilitato.CheckedChanged
        If Session(SWOP) = SWOPMODIFICA Then
            If Int(GridViewListini.SelectedDataKey.Value) = 1 And ChkAbilitato.Checked = False Then
                ChkAbilitato.Checked = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Impossibile disattivare il listino base", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        Session(SWMODIFICATO) = SWSI
        txtValuta.Focus()
    End Sub

    Private Sub txtValuta_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtValuta.TextChanged
        PosizionaItemDDLByTxt(txtValuta, DDLValuta)
        Session(SWMODIFICATO) = SWSI
        DDLValuta.Focus()
    End Sub

    Private Sub txtPagamento_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPagamento.TextChanged
        PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
        Session(SWMODIFICATO) = SWSI
        DDLPagamento.Focus()
    End Sub

    Private Sub txtCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCategoria.TextChanged
        PosizionaItemDDLByTxt(txtCategoria, DDLCategoria)
        Session(SWMODIFICATO) = SWSI
        DDLCategoria.Focus()
    End Sub

    Private Sub txtCodice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodice.TextChanged
        Session(SWMODIFICATO) = SWSI
        CheckNewCodLTOnTab()
        txtDescrizione.Focus()
    End Sub

    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        Session(SWMODIFICATO) = SWSI
        DDLTipoLis.Focus()
    End Sub

    Private Sub txtDataVal_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataVal.TextChanged
        Session(SWMODIFICATO) = SWSI
        txtValuta.Focus()
    End Sub

    Private Sub DDLTipoLis_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoLis.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        If DDLTipoLis.SelectedValue = 1 Then 'Normale
            If ChkCategoria.Checked = False And DDLCategoria.SelectedIndex = 0 Then
                ChkCategoria.Checked = True
                txtCategoria.Text = "" : txtCategoria.Enabled = False : DDLCategoria.Enabled = False
            End If

            If ChkClienti.Checked = False Then
                ChkClienti.Checked = True
                lblCodCliFor.Text = "" : lblRagSoc.Text = HTML_SPAZIO
            End If
        ElseIf DDLTipoLis.SelectedValue = 2 Then 'Categoria
            ChkCategoria.Checked = False
            txtCategoria.Enabled = True : DDLCategoria.Enabled = True
            If ChkClienti.Checked = False Then
                ChkClienti.Checked = True
                lblCodCliFor.Text = "" : lblRagSoc.Text = HTML_SPAZIO
            End If
        ElseIf DDLTipoLis.SelectedValue = 3 Then 'Cliente
            ChkClienti.Checked = False
            If ChkCategoria.Checked = False And DDLCategoria.SelectedIndex = 0 Then
                ChkCategoria.Checked = True
                txtCategoria.Text = "" : txtCategoria.Enabled = False : DDLCategoria.Enabled = False
            End If
        End If
        txtDataVal.Focus()
    End Sub

    Private Sub txtNote_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNote.TextChanged
        btnAggiorna.Focus()
    End Sub
#End Region

#Region "Funzioni e procedure"
    Private Sub LTSelezionato(ByVal Codice As Integer)

        Session(IDLISTINO) = Codice
        Dim LTSys As New ListVenT
        Dim myLT As ListVenTEntity
        Dim arrLT As ArrayList

        Try
            arrLT = LTSys.getListVenTByCodice(Codice)
            If (arrLT.Count > 0) Then
                myLT = CType(arrLT(0), ListVenTEntity)
                CampiSetEnabledTo(False)
                PopolaCampi(myLT)
                btnModifica.Enabled = True
                'Listino Base Codice 1 fisso
                If myLT.Codice = 1 Then
                    btnElimina.Enabled = False
                Else
                    btnElimina.Enabled = True
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Listino in tabella. Cause possibili: Cancellato da altro utente.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    Private Sub LTDSelezionato(ByVal CodLis As Integer, ByVal CodArt As String)
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        Dim LTDSys As New ListVenD
        Dim myLTD As ListVenDEntity
        Dim arrLTD As ArrayList

        Try
            arrLTD = LTDSys.getListVenDByCodLisCodArt(CodLis, CodArt)
            If (arrLTD.Count > 0) Then
                myLTD = CType(arrLTD(0), ListVenDEntity)
                CampiSetEnabledTo(False)
                PopolaCampiLTD(myLTD)
                btnModifica.Enabled = True
                btnElimina.Enabled = True : btnEliminaArt.Enabled = True
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Articolo nel Listino. Cause possibili: Cancellato da altro utente.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    Private Sub PopolaCampi(ByVal LT As ListVenTEntity)
        SfondoCampi()
        ' ''" & Mid(LT.Descrizione.Trim, 1, 20)
        TabPanel2.HeaderText = "Articoli presenti nel listino: (" & LT.Codice.ToString.Trim & ")"
        TabPanel3.HeaderText = "Includi/Escludi articoli dal listino: (" & LT.Codice.ToString.Trim & ")"
        TabPanel4.HeaderText = "Duplica listino: (" & LT.Codice.ToString.Trim & ")"
        txtCodice.Text = LT.Codice
        txtDescrizione.Text = LT.Descrizione
        PosizionaItemDDL(LT.Tipo.Trim.ToString, DDLTipoLis)
        txtDataVal.Text = Format(LT.Data_Inizio_Validita, FormatoData)
        ChkAbilitato.Checked = LT.Abilitato
        If LT.Valuta.Trim = "" Then
            txtValuta.Text = ""
            DDLValuta.SelectedIndex = 0
        Else
            txtValuta.Text = LT.Valuta
            PosizionaItemDDLByTxt(txtValuta, DDLValuta)
        End If
        If LT.Cod_Pagamento = 0 Then
            txtPagamento.Text = ""
            DDLPagamento.SelectedIndex = 0
            ChkPagamento.Checked = True
        Else
            txtPagamento.Text = LT.Cod_Pagamento
            PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
            ChkPagamento.Checked = False
        End If
        If LT.Categoria = 0 Then
            txtCategoria.Text = ""
            DDLCategoria.SelectedIndex = 0
            ChkCategoria.Checked = True
        Else
            txtCategoria.Text = LT.Categoria
            PosizionaItemDDLByTxt(txtCategoria, DDLCategoria)
            ChkCategoria.Checked = False
        End If
        If LT.Cliente.Trim = "" Then
            lblCodCliFor.Text = ""
            lblRagSoc.Text = HTML_SPAZIO
            ChkClienti.Checked = True
        Else
            lblCodCliFor.Text = LT.Cliente
            'lblRagSoc.Text = "Ragione Sociale"
            VisualizzaRagSoc(LT.Cliente)
            ChkClienti.Checked = False
        End If
        txtNote.Text = LT.Note
        Session(SWMODIFICATO) = SWNO

    End Sub
    Private Sub VisualizzaRagSoc(ByVal CodCoGe As String)
        Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
        If (listaClienti Is Nothing) Then
            lblRagSoc.Text = CodCoGe & " - non trovato in tabella"
            Exit Sub
        End If
        Dim myClienti As ClientiEntity
        Try
            If Not String.IsNullOrEmpty(CodCoGe) Then
                Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(CodCoGe)
                myClienti = f(0)
                If (myClienti Is Nothing) Then
                    'Non trovato Cliente
                    lblRagSoc.Text = CodCoGe & " - non trovato in tabella"
                Else
                    lblRagSoc.Text = myClienti.Rag_Soc
                End If
            Else
                lblRagSoc.Text = HTML_SPAZIO
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub PopolaCampiLTD(ByVal LTD As ListVenDEntity)
        Dim objLTDCampiCambio As New LTDCampiCambio

        SfondoCampiLTD()
        lblCodArticolo.Text = LTD.Cod_Articolo
        lblDescrizione.Text = LTD.Descrizione
        txtPrezzo.Text = Format(LTD.Prezzo, FormatoValMag)
        txtPrezzoMinimo.Text = Format(LTD.PrezzoMinimo, FormatoValMag)
        txtSconto1.Text = Format(LTD.Sconto_1, FormatoSCMag)
        txtSconto2.Text = Format(LTD.Sconto_2, FormatoSCMag)
        objLTDCampiCambio.Prezzo_Valuta = LTD.Prezzo_Valuta
        objLTDCampiCambio.Data_Cambio = LTD.Data_Cambio
        objLTDCampiCambio.Cambio = LTD.Cambio
        Session("LTDCampiCambio") = objLTDCampiCambio
        Session(SWMODIFICATO) = SWNO
        ' ''If (Not IsPostBack) Then
        ' ''    SWModificato.Value = SWNO
        ' ''End If

    End Sub

    Private Sub AzzeraCampi()

        SfondoCampi()
        txtCodice.Text = ""
        txtDescrizione.Text = ""
        If Session(SWOP) = SWOPNUOVO Then
            DDLTipoLis.SelectedIndex = 1
            txtDataVal.Text = Format(Now, FormatoData)
            ChkAbilitato.Checked = True
            txtValuta.Text = "Euro"
            PosizionaItemDDL(txtValuta.Text.Trim, DDLValuta)
        Else
            DDLTipoLis.SelectedIndex = 0
            txtDataVal.Text = ""
            ChkAbilitato.Checked = False
            txtValuta.Text = ""
            DDLValuta.SelectedIndex = 0
        End If

        txtPagamento.Text = ""
        DDLPagamento.SelectedIndex = 0
        ChkPagamento.Checked = True

        txtCategoria.Text = ""
        DDLCategoria.SelectedIndex = 0
        ChkCategoria.Checked = True

        lblCodCliFor.Text = ""
        lblRagSoc.Text = HTML_SPAZIO
        ChkClienti.Checked = True

        txtNote.Text = ""
        Session(SWMODIFICATO) = SWNO ': SWModificato.Value = SWNO
    End Sub
    Private Sub AzzeraCampiLTD()

        SfondoCampiLTD()
        lblCodArticolo.Text = ""
        lblDescrizione.Text = ""
        If Session(SWOP) = SWOPNUOVO Then
            txtPrezzo.Text = "" : txtPrezzoMinimo.Text = ""
            txtSconto1.Text = "" : txtSconto2.Text = ""
        Else
            txtPrezzo.Text = "" : txtPrezzoMinimo.Text = ""
            txtSconto1.Text = "" : txtSconto2.Text = ""
        End If

        Session(SWMODIFICATO) = SWNO ': SWModificato.Value = SWNO
    End Sub

    Private Sub SfondoCampi()

        txtCodice.BackColor = SEGNALA_OK
        txtDescrizione.BackColor = SEGNALA_OK

        DDLTipoLis.BackColor = SEGNALA_OK
        txtDataVal.BackColor = SEGNALA_OK
        ' ''ChkAbilitato.Checked = False
        txtValuta.BackColor = SEGNALA_OK
        DDLValuta.BackColor = SEGNALA_OK

        txtPagamento.BackColor = SEGNALA_OK
        DDLPagamento.BackColor = SEGNALA_OK
        ' ''ChkPagamento.Checked = True

        txtCategoria.BackColor = SEGNALA_OK
        DDLCategoria.BackColor = SEGNALA_OK
        ' ''ChkCategoria.Checked = True

        lblCodCliFor.BackColor = SEGNALA_OK
        ChkClienti.Checked = True

        txtNote.BackColor = SEGNALA_OK
    End Sub
    Private Sub SfondoCampiLTD()

        txtPrezzo.BackColor = SEGNALA_OK
        txtPrezzoMinimo.BackColor = SEGNALA_OK

        txtSconto1.BackColor = SEGNALA_OK
        txtSconto2.BackColor = SEGNALA_OK

    End Sub

    Private Sub CampiSetEnabledTo(ByVal Valore As Boolean)
        If Session(SWOP) = SWOPMODIFICA Then
            txtCodice.Enabled = False
        Else
            txtCodice.Enabled = Valore
        End If
        txtDescrizione.Enabled = Valore
        DDLTipoLis.Enabled = Valore
        txtDataVal.Enabled = Valore : imgBtnShowCalendar.Enabled = Valore : ChkAbilitato.Enabled = Valore
        txtValuta.Enabled = Valore : DDLValuta.Enabled = Valore
        ChkPagamento.Enabled = Valore
        If ChkPagamento.Checked = False Then
            txtPagamento.Enabled = Valore : DDLPagamento.Enabled = Valore
        Else
            txtPagamento.Enabled = False : DDLPagamento.Enabled = False
        End If
        ChkCategoria.Enabled = Valore
        If ChkCategoria.Checked = False Then
            txtCategoria.Enabled = Valore : DDLCategoria.Enabled = Valore
        Else
            txtCategoria.Enabled = False : DDLCategoria.Enabled = False
        End If
        btnCercaAnagrafica.Enabled = Valore
        ChkClienti.Enabled = Valore

        txtNote.Enabled = Valore
    End Sub
    Private Sub CampiSetEnabledToLTD(ByVal Valore As Boolean)
        txtPrezzo.Enabled = Valore : txtPrezzoMinimo.Enabled = Valore
        txtSconto1.Enabled = Valore : txtSconto2.Enabled = Valore
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        ddlRicerca.Enabled = Valore : txtRicerca.Enabled = Valore : btnRicercaArticolo.Enabled = Valore
        btnAggiorna.Enabled = Valore
        btnAnnulla.Enabled = Valore
        btnElimina.Enabled = Valore : btnEliminaArt.Enabled = Valore
        btnModifica.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnStampa.Enabled = Valore
        btnStampaSc.Enabled = Valore
        btnIncludiArt.Enabled = Valore : btnIncludiAll.Enabled = Valore
    End Sub

    Private Function ControllaCampi() As Boolean
        ControllaCampi = True : SfondoCampi()
        If Not IsNumeric(txtCodice.Text.Trim) Then
            txtCodice.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            If CheckNewCodLTOnTab() = True Then
                txtCodice.BackColor = SEGNALA_KO : ControllaCampi = False
            Else
                txtCodice.BackColor = SEGNALA_OK
            End If
        End If
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
        If DDLTipoLis.SelectedIndex = 0 Then
            DDLTipoLis.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            DDLTipoLis.BackColor = SEGNALA_OK
        End If
        If Not IsDate(txtDataVal.Text.Trim) Then
            txtDataVal.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            txtDataVal.BackColor = SEGNALA_OK
        End If

        If txtValuta.Text.Trim = "" Then
            txtValuta.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            txtValuta.BackColor = SEGNALA_OK
        End If
        If DDLValuta.SelectedIndex = 0 Then
            DDLValuta.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            DDLValuta.BackColor = SEGNALA_OK
        End If
        If txtPagamento.Text.Trim <> "" Then
            If Int(txtPagamento.Text.Trim) <> 0 Then
                If Not IsNumeric(txtPagamento.Text.Trim) Then
                    txtPagamento.BackColor = SEGNALA_KO : ControllaCampi = False
                Else
                    txtPagamento.BackColor = SEGNALA_OK
                End If
                If UCase(txtPagamento.Text.Trim) <> DDLPagamento.SelectedValue Then
                    txtPagamento.BackColor = SEGNALA_KO : ControllaCampi = False
                    DDLPagamento.BackColor = SEGNALA_KO
                Else
                    txtPagamento.BackColor = SEGNALA_OK
                    DDLPagamento.BackColor = SEGNALA_OK
                End If
            ElseIf DDLPagamento.SelectedIndex <> 0 Then
                txtPagamento.BackColor = SEGNALA_KO : ControllaCampi = False
                DDLPagamento.BackColor = SEGNALA_KO
            End If
        ElseIf DDLPagamento.SelectedIndex <> 0 Then
            txtPagamento.BackColor = SEGNALA_KO : ControllaCampi = False
            DDLPagamento.BackColor = SEGNALA_KO
        End If
        If txtCategoria.Text.Trim <> "" Then
            If Int(txtCategoria.Text.Trim) <> 0 Then
                If Not IsNumeric(txtCategoria.Text.Trim) Then
                    txtCategoria.BackColor = SEGNALA_KO : ControllaCampi = False
                Else
                    txtCategoria.BackColor = SEGNALA_OK
                End If
                If UCase(txtCategoria.Text.Trim) <> DDLCategoria.SelectedValue Then
                    txtCategoria.BackColor = SEGNALA_KO : ControllaCampi = False
                    DDLCategoria.BackColor = SEGNALA_KO
                Else
                    txtCategoria.BackColor = SEGNALA_OK
                    DDLCategoria.BackColor = SEGNALA_OK
                End If
            ElseIf DDLCategoria.SelectedIndex <> 0 Then
                txtCategoria.BackColor = SEGNALA_KO : ControllaCampi = False
                DDLCategoria.BackColor = SEGNALA_KO
            End If
        ElseIf DDLCategoria.SelectedIndex <> 0 Then
            txtCategoria.BackColor = SEGNALA_KO : ControllaCampi = False
            DDLCategoria.BackColor = SEGNALA_KO
        Else 'SE IL LISTINO E' DI TIPO CATEGORIA è OBBLIGATORIA
            If DDLTipoLis.SelectedValue = 2 Then
                txtCategoria.BackColor = SEGNALA_KO : ControllaCampi = False
                DDLCategoria.BackColor = SEGNALA_KO
                DDLTipoLis.BackColor = SEGNALA_KO
            End If
        End If
        If lblCodCliFor.Text.Trim <> "" Then
            If DDLTipoLis.SelectedValue = 3 Then
                lblCodCliFor.BackColor = SEGNALA_KO : ControllaCampi = False
                DDLTipoLis.BackColor = SEGNALA_KO
            End If
        End If
        ' ''If lblCodCliFor.Text.Trim <> "" Then
        ' ''    If UCase(txtCodCliente.Text.Trim) <> DDLClienti.SelectedValue Then
        ' ''        txtCodCliente.BackColor = SEGNALA_KO : ControllaCampi = False
        ' ''        DDLClienti.BackColor = SEGNALA_KO
        ' ''    Else
        ' ''        txtCodCliente.BackColor = SEGNALA_OK
        ' ''        DDLClienti.BackColor = SEGNALA_OK
        ' ''    End If
        ' ''ElseIf DDLClienti.SelectedIndex <> 0 Then
        ' ''    txtCodCliente.BackColor = SEGNALA_KO : ControllaCampi = False
        ' ''    DDLClienti.BackColor = SEGNALA_KO
        ' ''Else 'SE IL LISTINO E' DI TIPO CLIENTE è OBBLIGATORIO
        ' ''    If DDLTipoLis.SelectedValue = 3 Then
        ' ''        txtCodCliente.BackColor = SEGNALA_KO : ControllaCampi = False
        ' ''        DDLClienti.BackColor = SEGNALA_KO
        ' ''        DDLTipoLis.BackColor = SEGNALA_KO
        ' ''    End If
        ' ''End If
    End Function
    Private Function ControllaCampiLTD(ByRef SWPrezzo0 As Boolean, ByRef SWPrezzoMin As Boolean) As Boolean
        SWPrezzo0 = False : SWPrezzoMin = False
        ControllaCampiLTD = True : SfondoCampiLTD()
        If Not IsNumeric(txtPrezzo.Text.Trim) Then
            If txtPrezzo.Text.Trim <> "" Then
                txtPrezzo.BackColor = SEGNALA_KO : ControllaCampiLTD = False
            End If
        Else
            If CDbl(txtPrezzo.Text) = 0 Then
                SWPrezzo0 = True
            End If
        End If
        If Not IsNumeric(txtPrezzoMinimo.Text.Trim) Then
            If txtPrezzoMinimo.Text.Trim <> "" Then
                txtPrezzoMinimo.BackColor = SEGNALA_KO : ControllaCampiLTD = False
            End If
        ElseIf SWPrezzo0 = False Then
            If CDbl(txtPrezzo.Text) < CDbl(txtPrezzoMinimo.Text) Then
                SWPrezzoMin = True
                txtPrezzoMinimo.BackColor = SEGNALA_KO : ControllaCampiLTD = False
            End If
        End If
        If Not IsNumeric(txtSconto1.Text.Trim) Then
            If txtSconto1.Text.Trim <> "" Then
                txtSconto1.BackColor = SEGNALA_KO : ControllaCampiLTD = False
            End If
        End If
        If Not IsNumeric(txtSconto2.Text.Trim) Then
            If txtSconto2.Text.Trim <> "" Then
                txtSconto2.BackColor = SEGNALA_KO : ControllaCampiLTD = False
            End If
        End If
    End Function

    Private Function CheckNewCodLTOnTab() As Boolean
        If Session(SWOP) <> SWOPNUOVO Then
            Exit Function
        End If
        If txtCodice.Text.Trim <> "" And IsNumeric(txtCodice.Text.Trim) Then
            Dim Codice As Integer = Int(txtCodice.Text.Trim)
            If Codice > 0 Then
                Dim LTSys As New ListVenT
                Dim arrLT As ArrayList

                Try
                    arrLT = LTSys.getListVenTByCodice(Codice)
                    If (arrLT.Count > 0) Then
                        CheckNewCodLTOnTab = True
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Codice listino già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                        Exit Function
                    End If
                Catch
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End Try
            End If
        End If
    End Function
#End Region


    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        '---- SESSIONE SCADUTA ???
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = ""
        If String.IsNullOrEmpty(IDLT) Then IDLT = ""
        If IDLT = "" Then
            Response.Redirect("..\Login.aspx?SessioneScaduta=1")
            Exit Sub
        End If
        '-----------
        SqlDSArtLis.FilterExpression = "" : txtRicerca.Text = ""
        Session("SortListVenD") = Mid(ddlRicerca.SelectedValue.Trim, 1, 1)
        If ddlRicerca.SelectedValue = "C1" Then
            SqlDSArtLis.FilterExpression = "Prezzo = 0"
        ElseIf ddlRicerca.SelectedValue = "D3" Then
            SqlDSArtLis.FilterExpression = "Prezzo = 0"
        End If
        SqlDSArtLis.DataBind()
        AzzeraCampiLTD() : CampiSetEnabledToLTD(False)
        BtnSetEnabledTo(False)
        ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewArtLis.DataBind()
        If GridViewArtLis.Rows.Count > 0 Then
            GridViewArtLis.SelectedIndex = 0
            LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
        Else
            AzzeraCampiLTD()
        End If
    End Sub

    Protected Sub btnRicercaArticolo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicercaArticolo.Click

        If txtRicerca.Text.Trim = "" Then
            SqlDSArtLis.FilterExpression = ""
            SqlDSArtLis.DataBind()
            AzzeraCampiLTD() : CampiSetEnabledToLTD(False)
            BtnSetEnabledTo(False)
            ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
            btnNuovo.Enabled = True

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewArtLis.DataBind()
            If GridViewArtLis.Rows.Count > 0 Then
                GridViewArtLis.SelectedIndex = 0
                LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
            Else
                AzzeraCampiLTD()
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "C" Then
            SqlDSArtLis.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
        ElseIf ddlRicerca.SelectedValue = "D1" Then
            SqlDSArtLis.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
        ElseIf ddlRicerca.SelectedValue = "D2" Then
            SqlDSArtLis.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
        ElseIf ddlRicerca.SelectedValue = "C1" Then
            SqlDSArtLis.FilterExpression = "Prezzo = 0"
        ElseIf ddlRicerca.SelectedValue = "D3" Then
            SqlDSArtLis.FilterExpression = "Prezzo = 0"
        End If
        SqlDSArtLis.DataBind()
        AzzeraCampiLTD() : CampiSetEnabledToLTD(False)
        BtnSetEnabledTo(False)
        ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewArtLis.DataBind()
        If GridViewArtLis.Rows.Count > 0 Then
            GridViewArtLis.SelectedIndex = 0
            LTDSelezionato(Session(IDLISTINO), GridViewArtLis.SelectedDataKey.Value.ToString.Trim)
        Else
            AzzeraCampiLTD()
        End If

    End Sub

#Region "Ricerca Clienti/Fornitori"

    Private Sub btnCercaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti()
    End Sub

    Private Sub ApriElencoClienti()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoCli.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        lblCodCliFor.Text = codice
        lblRagSoc.Text = descrizione
        ChkClienti.Checked = False
        Session(SWMODIFICATO) = SWSI
    End Sub
#End Region

    'alb070213
#Region "Ricerche in includi/escludi"
    Private Sub btnRicercaArtIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicercaArtIn.Click
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3
        Session("btnIN") = True
        If ddlRicercaArtIn.SelectedValue = "C" Then
            SqlDSArtIn.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
        ElseIf ddlRicercaArtIn.SelectedValue = "D1" Then
            SqlDSArtIn.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "'"
        ElseIf ddlRicercaArtIn.SelectedValue = "D2" Then
            SqlDSArtIn.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
        ElseIf ddlRicercaArtIn.SelectedValue = "C2" Then
            SqlDSArtIn.FilterExpression = "Prezzo = 0"
        ElseIf ddlRicercaArtIn.SelectedValue = "D3" Then
            SqlDSArtIn.FilterExpression = "Prezzo = 0"
        End If
        SqlDSArtIn.DataBind()
        If Not Session("btnOUT") Then
            btnRicercaArtOu_Click(Nothing, Nothing)
        ElseIf GridViewArtIn.Rows.Count = 0 Then
            btnRicercaArtOu_Click(Nothing, Nothing)
        End If
        Session("btnIN") = False
    End Sub

    Private Sub btnRicercaArtOu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicercaArtOu.Click
        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        Session("btnOUT") = True
        If DDLRicercaArtOu.SelectedValue = "C" Then
            SqlDSArtOu.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "%'"
        ElseIf DDLRicercaArtOu.SelectedValue = "D1" Then
            SqlDSArtOu.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "'"
        ElseIf DDLRicercaArtOu.SelectedValue = "D2" Then
            SqlDSArtOu.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "%'"
        End If
        SqlDSArtOu.DataBind()
        If Not Session("btnIN") Then
            btnRicercaArtIn_Click(Nothing, Nothing)
        ElseIf GridViewArtOu.Rows.Count = 0 Then
            btnRicercaArtIn_Click(Nothing, Nothing)
        End If
        Session("btnOUT") = False
    End Sub

    Private Sub ddlRicercaArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicercaArtIn.SelectedIndexChanged
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3

        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        If ddlRicercaArtIn.SelectedValue = "C2" Then
            DDLRicercaArtOu.SelectedValue = "C"
        ElseIf ddlRicercaArtIn.SelectedValue = "D3" Then
            DDLRicercaArtOu.SelectedValue = "D1"
        Else
            DDLRicercaArtOu.SelectedValue = ddlRicercaArtIn.SelectedValue
        End If
        DDLRicercaArtOu_SelectedIndexChanged(Nothing, Nothing)
        'Session("SortListVenD") = Mid(ddlRicercaArtIn.SelectedValue.Trim, 1, 1)
        Select Case ddlRicercaArtIn.SelectedValue
            Case "C"
                GridViewArtIn.Sort("Cod_Articolo", SortDirection.Ascending)
            Case "D1"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
            Case "D2"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
            Case "C2"
                GridViewArtIn.Sort("Cod_Articolo", SortDirection.Ascending)
            Case "D3"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
        End Select
    End Sub

    Private Sub DDLRicercaArtOu_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRicercaArtOu.SelectedIndexChanged
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3

        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Session("SortListVenD") = Mid(ddlRicercaArtIn.SelectedValue.Trim, 1, 1)
        Select Case DDLRicercaArtOu.SelectedValue
            Case "C"
                GridViewArtOu.Sort("Cod_Articolo", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "C" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
            Case "D1"
                GridViewArtOu.Sort("Descrizione", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "D" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
            Case "D2"
                GridViewArtOu.Sort("Descrizione", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "D" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
        End Select
    End Sub

    Private Sub txtRicercaArtIn_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaArtIn.TextChanged
        txtRicercaArtOu.Text = txtRicercaArtIn.Text
    End Sub

    Private Sub txtRicercaArtOu_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaArtOu.TextChanged
        txtRicercaArtIn.Text = txtRicercaArtOu.Text
    End Sub
#End Region
    'alb070213 END
End Class