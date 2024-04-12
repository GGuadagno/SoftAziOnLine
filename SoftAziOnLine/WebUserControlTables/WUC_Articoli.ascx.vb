Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports System.Collections
Imports System.Linq
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_Articoli
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    ' ''Private Const CAMPI_MODIFICATI As String = "CampiModificati"
    Private Const CAMPO_PREZZO_VENDITA_OK As String = "CampoPrezzoVenditaOk"
    Private Const TIPO_AGG_ARTICOLO As String = "AggiornamentoArticolo"
    ' ''Private Const ERRORI_AGGIORNAMENTO As String = "ErroriAggiornamento"
    Private Const ELEMENTO_LISTVEND As String = "ElementoListVenD"
    Private Const F_ALIQUOTAIVA As String = "ElencoAliquotaIVA"
    Private Const F_PAGAMENTI As String = "ElencoPagamenti"

#End Region

#Region "Property"

    Private _CodiceBase As String
    Property CodiceBase() As String
        Get
            Return _CodiceBase
        End Get
        Set(ByVal value As String)
            _CodiceBase = value
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        btnStampaArtFor.Text = "Stampa articoli fornitori"
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        Session(DBCONNAZI) = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Session(DBCONNCOGE) = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDataSourceArticoli.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceCategoria.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceCodBarre.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceLinea.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceUM.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceFornitore.ConnectionString = Session(DBCONNCOGE)
        SqlDataSourceCondizioniPag.ConnectionString = Session(DBCONNCOGE)
        SqlDataSourceAliquotaIva.ConnectionString = Session(DBCONNCOGE)

        txtCodBase.MaxLength = App.GetParamGestAzi(Session(ESERCIZIO)).LunghezzaMaxCodice
        txtCodOpzione.MaxLength = CSTLOpz
        If (Not IsPostBack) Then
            SetFiltroBaseSQLDataSourceArticoli()
            Tabs.ActiveTabIndex = 0
            TabContainer1.ActiveTabIndex = 0
            CaricaVariabili()
            SelPrimoArticolo()
        End If

        ModalPopup.WucElement = Me
        WFPFornSec.WucElement = Me
        WFPListiniDaAgg.WucElement = Me
        WFPElencoAliquotaIVA.WucElement = Me
        WFPElencoForn.WucElement = Me
        WFPElencoPagamenti.WucElement = Me
        '--sTAMPE ROB 121211
        WUC_SceltaStampaAnaArt1.WucElement = Me
        '-------------------
        If Session(F_FORNSEC_APERTA) Then
            WFPFornSec.Show()
        End If
        If Session(F_SCELTALISTINI_APERTA) Then
            WFPListiniDaAgg.Show()
        End If
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            WFPElencoForn.Show()
        End If

        WFP_CategorieArt1.WucElement = Me
        If Session(F_ANAGRCATEGORIEART_APERTA) Then
            WFP_CategorieArt1.Show()
        End If

        WFP_LineeArt1.WucElement = Me
        If Session(F_ANAGRLINEEART_APERTA) Then
            WFP_LineeArt1.Show()
        End If
        'giu140419
        WFP_TipoCodArt1.WucElement = Me
        If Session(F_ANAGRTIPOCODART_APERTA) Then
            WFP_TipoCodArt1.Show()
        End If

        WFP_Misure1.WucElement = Me
        If Session(F_ANAGRMISURE_APERTA) Then
            WFP_Misure1.Show()
        End If

        WFP_Pagamenti1.WucElement = Me
        If Session(F_PAGAMENTI_APERTA) Then
            WFP_Pagamenti1.Show()
        End If

        Select Case Session(F_ELENCO_APERTA)
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show()
            Case F_PAGAMENTI
                WFPElencoPagamenti.Show()
        End Select
    End Sub

    Private Sub GridViewArticoli_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewArticoli.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewArticoli.PageIndex = 0
        Else
            GridViewArticoli.PageIndex = e.NewPageIndex
        End If
        GridViewArticoli.SelectedIndex = -1

        setPulsantiModalitaConsulta()
        If (Not String.IsNullOrEmpty(_CodiceBase)) Then
            If txtRicerca.Text.Trim = "" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", _CodiceBase)
            ElseIf ddlRicerca.SelectedValue = "Codice" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            ElseIf ddlRicerca.SelectedValue = "Descrizione" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%' AND Descrizione LIKE '{1}%'", _CodiceBase, Controlla_Apice(txtRicerca.Text.Trim))
            Else
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%' AND Descrizione LIKE '%{1}%'", _CodiceBase, Controlla_Apice(txtRicerca.Text.Trim))
            End If
        Else
            If txtRicerca.Text.Trim = "" Then
                SqlDataSourceArticoli.FilterExpression = String.Empty
            ElseIf ddlRicerca.SelectedValue = "Codice" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            ElseIf ddlRicerca.SelectedValue = "Descrizione" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Descrizione LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            Else
                SqlDataSourceArticoli.FilterExpression = String.Format("Descrizione LIKE '%{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            End If
        End If
        SqlDataSourceArticoli.DataBind()
        SelPrimoArticolo()

    End Sub

    Private Sub GridViewArticoli_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArticoli.SelectedIndexChanged
        CaricaVariabili()
        Try
            Session(COD_ARTICOLO) = GridViewArticoli.SelectedDataKey.Value
        Catch ex As Exception
            'PER EVITARE L'ERRORE DELLA PRIMA VOLTA CHE NON E' STATO ANCORA INIZIALIZZATO
        End Try
        VisualizzaArticolo()
    End Sub

#Region "TextBox"

    Private Sub txtCodOpzione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodOpzione.TextChanged
        If (String.IsNullOrEmpty(txtCodBase.Text)) Then
            txtCodBase.BackColor = Def.SEGNALA_KO
            txtCodBase.Focus()
            Exit Sub
        End If
        If (Not String.IsNullOrEmpty(txtCodOpzione.Text) And txtCodOpzione.Text.Length <> txtCodOpzione.MaxLength) Then
            txtCodOpzione.BackColor = Def.SEGNALA_KO
            lblCodArticolo.Text = txtCodBase.Text.Trim
            txtCodOpzione.Focus()
            Exit Sub
        End If
        lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
        If lblCodArticolo.Text.Length > txtCodBase.MaxLength Then
            txtCodBase.BackColor = Def.SEGNALA_KO
            txtCodOpzione.BackColor = Def.SEGNALA_KO
            txtCodBase.Focus()
            Exit Sub
        End If
        txtCodOpzione.BackColor = Def.SEGNALA_OK
        Session(SWMODIFICATO) = True
        txtDescBreve.Focus()
    End Sub

    Private Sub txtCodBase_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodBase.TextChanged
        lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
        If lblCodArticolo.Text.Length > txtCodBase.MaxLength Then
            txtCodBase.BackColor = Def.SEGNALA_KO
            txtCodOpzione.BackColor = Def.SEGNALA_KO
            txtCodBase.Focus()
            Exit Sub
        End If
        If (txtCodBase.Text.Length > txtCodBase.MaxLength) Then
            txtCodBase.BackColor = Def.SEGNALA_KO
            txtCodBase.Focus()
            Exit Sub
        End If
        txtCodBase.BackColor = Def.SEGNALA_OK
        Session(SWMODIFICATO) = True
        txtCodOpzione.Focus()
    End Sub
    'giu070312
    Private Sub txtDescBreve_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescBreve.TextChanged
        If (String.IsNullOrEmpty(txtDescBreve.Text)) Then
            txtDescBreve.BackColor = Def.SEGNALA_KO
            txtDescBreve.Focus()
            Exit Sub
        End If
        txtDescBreve.BackColor = Def.SEGNALA_OK
       Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        PosizionaItemDDLTxt(txtCodFornitore, ddlFornitore)
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtCodLinea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodLinea.TextChanged
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
       Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtCodUnitaMisura_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodUnitaMisura.TextChanged
        PosizionaItemDDLTxt(txtCodUnitaMisura, ddlUnitaMisura)
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtCodAliquotaIva_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAliquotaIva.TextChanged
        ControllaValoreNumerico(txtCodAliquotaIva, 0)
        If (txtCodAliquotaIva.BackColor = Def.SEGNALA_OK) Then
            PosizionaItemDDLTxt(txtCodAliquotaIva, ddlAliquotaIva)
            Session(SWMODIFICATO) = True
        End If
    End Sub

    Private Sub txtCodCondizioniPag_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCondizioniPag.TextChanged
        ControllaValoreNumerico(txtCodCondizioniPag, 0)
        If (txtCodCondizioniPag.BackColor = Def.SEGNALA_OK) Then
            PosizionaItemDDLTxt(txtCodCondizioniPag, ddlCondizioniPag)
            Session(SWMODIFICATO) = True
        End If
    End Sub

    Private Sub txtPeso_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPeso.TextChanged
        ControllaValoreNumerico(txtPeso, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        txtAltezza.Focus()
    End Sub

    Private Sub txtAltezza_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAltezza.TextChanged
        ControllaValoreNumerico(txtAltezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        txtLunghezza.Focus()
    End Sub

    Private Sub txtLarghezza_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLarghezza.TextChanged
        ControllaValoreNumerico(txtLarghezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtLunghezza_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLunghezza.TextChanged
        ControllaValoreNumerico(txtLunghezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        txtLarghezza.Focus()
    End Sub

    Private Sub txtUltPrezzoAcq_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtUltPrezzoAcq.TextChanged
        ControllaValoreNumerico(txtUltPrezzoAcq, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        Session(SWMODIFICATO) = True
        txtRicarico.Focus()
    End Sub

    Private Sub txtPrezzoVendita_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPrezzoVendita.TextChanged
        ControllaValoreNumerico(txtPrezzoVendita, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        Session(SWMODIFICATO) = True
        txtPrezzoMinVen.Focus()
    End Sub

    Private Sub txtPrezzoMinVen_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPrezzoMinVen.TextChanged
        ControllaValoreNumerico(txtPrezzoMinVen, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        Session(SWMODIFICATO) = True
        txtSconto1Perc.Focus()
    End Sub

    Private Sub txtSconto1Perc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSconto1Perc.TextChanged
        ControllaValoreNumerico(txtSconto1Perc, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        Session(SWMODIFICATO) = True
        txtSconto2Perc.Focus()
    End Sub

    Private Sub txtSconto2Perc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSconto2Perc.TextChanged
        ControllaValoreNumerico(txtSconto2Perc, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        Session(SWMODIFICATO) = True
        txtConfezioneDa.Focus()
    End Sub

    Private Sub txtConfezioneDa_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtConfezioneDa.TextChanged
        ControllaValoreNumerico(txtConfezioneDa, 0)
        Session(SWMODIFICATO) = True
    End Sub
    'GIU070414
    Private Sub txtNAnni_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        txtNAnniGaranzia.TextChanged, txtNAnniScadBatterie.TextChanged, txtNAnniScadElettrodi.TextChanged
        If sender.TEXT.TRIM = "" Then
            sender.TEXT = "0"
        End If
        ControllaValoreNumerico(sender, 0)
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub txtRicarico_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicarico.TextChanged
        ControllaValoreNumerico(txtRicarico, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        txtPrezzoVendita.Focus()
    End Sub

    Private Sub txtQtaRiordino_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQtaRiordino.TextChanged
        ControllaValoreNumerico(txtQtaRiordino, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        txtGiorniConsegna.Focus()
    End Sub

    Private Sub txtQtaSottoscorta_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQtaSottoscorta.TextChanged
        ControllaValoreNumerico(txtQtaSottoscorta, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
        checkSottoscorta.Focus()
    End Sub

    Private Sub txtGiorniConsegna_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGiorniConsegna.TextChanged
        ControllaValoreNumerico(txtGiorniConsegna, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        Session(SWMODIFICATO) = True
    End Sub

#End Region

#Region "Pulsanti"

    Private Sub btnRicercaArticolo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicercaArticolo.Click
        setPulsantiModalitaConsulta()
        If (Not String.IsNullOrEmpty(_CodiceBase)) Then
            If txtRicerca.Text.Trim = "" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", _CodiceBase)
            ElseIf ddlRicerca.SelectedValue = "Codice" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            ElseIf ddlRicerca.SelectedValue = "Descrizione" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%' AND Descrizione LIKE '{1}%'", _CodiceBase, Controlla_Apice(txtRicerca.Text.Trim))
            Else
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%' AND Descrizione LIKE '%{1}%'", _CodiceBase, Controlla_Apice(txtRicerca.Text.Trim))
            End If
        Else
            If txtRicerca.Text.Trim = "" Then
                SqlDataSourceArticoli.FilterExpression = String.Empty
            ElseIf ddlRicerca.SelectedValue = "Codice" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            ElseIf ddlRicerca.SelectedValue = "Descrizione" Then
                SqlDataSourceArticoli.FilterExpression = String.Format("Descrizione LIKE '{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            Else
                SqlDataSourceArticoli.FilterExpression = String.Format("Descrizione LIKE '%{0}%'", Controlla_Apice(txtRicerca.Text.Trim))
            End If
        End If
        SqlDataSourceArticoli.DataBind()
        SelPrimoArticolo()
    End Sub

    Private Sub btnTrovaAliquotaIva_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaAliquotaIva.Click
        'SqlDataSourceAliquotaIva.DataBind()
        'ddlAliquotaIva.DataBind()
        ApriElenco(F_ALIQUOTAIVA)
    End Sub

    Private Sub btnTrovaCategoria_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaCategoria.Click
        SqlDataSourceCategoria.DataBind()
        ddlCatgoria.DataBind()
    End Sub

    Private Sub btnTrovaCondizioniPag_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaCondizioniPag.Click
        'SqlDataSourceCondizioniPag.DataBind()
        'ddlCondizioniPag.DataBind()
        ApriElenco(F_PAGAMENTI)
    End Sub

    Private Sub btnTrovaFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaFornitore.Click
        'SqlDataSourceFornitore.DataBind()
        'ddlFornitore.DataBind()
        ApriElencoFornitori()
    End Sub

    Private Sub btnTrovaLinea_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaLinea.Click
        SqlDataSourceLinea.DataBind()
        ddlLinea.DataBind()
    End Sub

    Private Sub btnTrovaUnitaMisura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaUnitaMisura.Click
        SqlDataSourceUM.DataBind()
        ddlUnitaMisura.DataBind()
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        GridViewArticoli.Enabled = False
        ddlRicerca.Enabled = False
        txtRicerca.Enabled = False
        btnRicercaArticolo.Enabled = False
        SvuotaCampi()
        'Richiesta di Cinzia del 17/11/2011
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim strErrAll As String = ""
        txtConfezioneDa.Text = "1"
        txtNAnniGaranzia.Text = "0"
        txtNAnniScadBatterie.Text = "0"
        txtNAnniScadElettrodi.Text = "0"
        'giu270618
        chkID1HS1.Checked = False : chkID2FR2.Checked = False : chkID3FR3.Checked = False : chkID4FRX.Checked = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "ConfArticoli", strValore, strErrore) = True Then
            txtConfezioneDa.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If ddlCatgoria.Items.Count = 2 Then
            ddlCatgoria.SelectedIndex = 1
            txtCodCategoria.Text = ddlCatgoria.SelectedValue
            txtCodCategoria.BackColor = SEGNALA_OK
            ddlCatgoria.BackColor = SEGNALA_OK
        End If
        If ddlLinea.Items.Count = 2 Then
            ddlLinea.SelectedIndex = 1
            txtCodLinea.Text = ddlLinea.SelectedValue
            txtCodLinea.BackColor = SEGNALA_OK
            ddlLinea.BackColor = SEGNALA_OK
        End If
        txtCodUnitaMisura.Text = "NR"
        If App.GetDatiAbilitazioni(CSTABILAZI, "UMArticoli", strValore, strErrore) = True Then
            txtCodUnitaMisura.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        PosizionaItemDDLTxt(txtCodUnitaMisura, ddlUnitaMisura)
        radioArticolo.Checked = True
        txtCodAliquotaIva.Text = App.GetParamGestAzi(Session(ESERCIZIO)).IvaSpese
        If App.GetDatiAbilitazioni(CSTABILAZI, "IVArticoli", strValore, strErrore) = True Then
            txtCodAliquotaIva.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        PosizionaItemDDLTxt(txtCodAliquotaIva, ddlAliquotaIva)
        If ddlFornitore.Items.Count = 2 Then
            ddlFornitore.SelectedIndex = 1
            txtCodFornitore.Text = ddlFornitore.SelectedValue
            txtCodFornitore.BackColor = SEGNALA_OK
            ddlFornitore.BackColor = SEGNALA_OK
            PopolaTitolareRiferimento(txtCodFornitore.Text)
        End If
        CampiSetEnabledTo(True)
        ''pier17012012
        btnDefBaseOpz.Enabled = False
        TxtDefCodBase.Text = 0
        LblDefBasOpz.Text = 0

        Tabs.ActiveTabIndex = 0
        txtCodBase.Focus()
        setPulsantiModalitaAggiorna()
        Session(SWOP) = SWOPNUOVO
        GWFornitoriSec.SvuotaGridView()
        GWDescEstesa.SvuotaGridView()
        GWArticoliCTV.SvuotaGridView()
        DistintaBase1.SvuotaGridView()

        Try
            If Session(CSTABILMSG) = SWSI Then
                If strErrAll.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Lettura Abilitazioni", strErrAll.Trim, WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            End If
        Catch ex As Exception

        End Try
       
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        GridViewArticoli.Enabled = True
        ddlRicerca.Enabled = True
        txtRicerca.Enabled = True
        btnRicercaArticolo.Enabled = True
        SvuotaCampi()
        CampiSetEnabledTo(False)
        VisualizzaArticolo()
        setPulsantiModalitaConsulta()
        CaricaVariabili()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        GridViewArticoli.Enabled = False
        CampiSetEnabledTo(True)
        ddlRicerca.Enabled = False
        txtRicerca.Enabled = False
        btnRicercaArticolo.Enabled = False
        txtCodBase.Enabled = False
        txtCodOpzione.Enabled = False
        txtDescBreve.Focus()
        setPulsantiModalitaAggiorna()
        Session(SWOP) = SWOPMODIFICA
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim myAnaMag As AnaMagEntity = Nothing
        Dim myListVenD As ListVenDEntity = Nothing
        Dim myListFornSec As List(Of FornSecondariEntity) = Nothing
        Dim myListAnaMagDes As List(Of AnaMagDesEntity) = Nothing
        Dim myListAnaMagCTV As List(Of AnaMagCTVEntity) = Nothing
        Dim myListDistBase As List(Of DistBaseEntity) = Nothing

        CheckNewCodiceArticolo()
        ControllaEsistenzaCampiInErrore()
        If (Not Session(SWERRORI_AGGIORNAMENTO)) Then
            ControlloCampiObbligatori()
            LeggiCampi(myAnaMag, myListVenD, myListFornSec, myListAnaMagDes, myListAnaMagCTV, myListDistBase)
            If (Session(SWERRORI_AGGIORNAMENTO)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati articoli", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If

            If (ControlloCampiDati()) Then
                Aggiornamento(myAnaMag, myListVenD, myListFornSec, myListAnaMagDes, myListAnaMagCTV, myListDistBase)
                ddlRicerca.Enabled = True
                txtRicerca.Enabled = True
                btnRicercaArticolo.Enabled = True
            Else
                Exit Sub
            End If
            'GIU181011 giu181111
            Session(COD_ARTICOLO) = lblCodArticolo.Text.Trim
            'giu280312
            If Session(SWOP) = SWOPNUOVO Then
                SqlDataSourceArticoli.DataBind()
                GridViewArticoli.DataBind()
                _CodiceBase = Session(COD_ARTICOLO)
                SetFiltroBaseSQLDataSourceArticoli
            End If
            VisualizzaArticolo()
            setPulsantiModalitaConsulta()
            CaricaVariabili()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati articoli", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfermaEliminaArticolo"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Elimina articolo", "Si vuole cancellare l'articolo selezionato?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub

    Private Sub btnAggiungiFornSec_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiungiFornSec.Click
        WFPFornSec.WucElement = Me
        WFPFornSec.SvuotaCampi()
        Session(F_FORNSEC_APERTA) = True
        WFPFornSec.Show()
    End Sub

#End Region

#Region "DropDownList"

    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        SetFiltroBaseSQLDataSourceArticoli()
        txtRicerca.Text = String.Empty
        Session("SortArticoli") = ddlRicerca.SelectedValue
        SelPrimoArticolo()
    End Sub

    Private Sub ddlAliquotaIva_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAliquotaIva.SelectedIndexChanged
        txtCodAliquotaIva.Text = ddlAliquotaIva.SelectedValue
        txtCodAliquotaIva.BackColor = SEGNALA_OK
        ddlAliquotaIva.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub ddlCatgoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatgoria.SelectedIndexChanged
        txtCodCategoria.Text = ddlCatgoria.SelectedValue
        txtCodCategoria.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub ddlCondizioniPag_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCondizioniPag.SelectedIndexChanged
        txtCodCondizioniPag.Text = ddlCondizioniPag.SelectedValue
        txtCodCondizioniPag.BackColor = SEGNALA_OK
        ddlCondizioniPag.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub ddlFornitore_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlFornitore.SelectedIndexChanged
        txtCodFornitore.Text = ddlFornitore.SelectedValue
        txtCodFornitore.BackColor = SEGNALA_OK
        ddlFornitore.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
        PopolaTitolareRiferimento(txtCodFornitore.Text)
    End Sub

    Private Sub ddlLinea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLinea.SelectedIndexChanged
        txtCodLinea.Text = ddlLinea.SelectedValue
        txtCodLinea.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub ddlTipoCodBarre_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTipoCodBarre.SelectedIndexChanged
        ddlTipoCodBarre.BackColor = SEGNALA_OK
        lblTipoCodBarre.Text = GetDesTipoCodBarre()
        Session(SWMODIFICATO) = True
    End Sub

    Private Sub ddlUnitaMisura_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUnitaMisura.TextChanged
        txtCodUnitaMisura.Text = ddlUnitaMisura.SelectedValue
        txtCodUnitaMisura.BackColor = SEGNALA_OK
        ddlUnitaMisura.BackColor = SEGNALA_OK
        Session(SWMODIFICATO) = True
    End Sub

#End Region

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SWOP) = SWOPNESSUNA
        Session(SWMODIFICATO) = False
        Session(CAMPO_PREZZO_VENDITA_OK) = False
        Session(SWERRORI_AGGIORNAMENTO) = False
        Session(ELEMENTO_LISTVEND) = New ListVenD()
    End Sub

#End Region

#Region "Metodi private"

#Region "Gestione Campi"

    Private Sub CampiSetEnabledTo(ByVal valore As Boolean)
        txtCodBase.Enabled = valore
        txtCodOpzione.Enabled = valore
        'giu031111 non gestito per adesso EAN
        ' ''txtCodAzienda.Enabled = valore
        ' ''txtCodStEAN.Enabled = valore
        ' ''txtCodContr.Enabled = valore
        txtDescBreve.Enabled = valore
        txtPeso.Enabled = valore
        txtAltezza.Enabled = valore
        txtLunghezza.Enabled = valore
        txtLarghezza.Enabled = valore
        txtCodArtFornitore.Enabled = valore
        txtQtaSottoscorta.Enabled = valore
        txtQtaRiordino.Enabled = valore
        txtGiorniConsegna.Enabled = valore
        txtCodBarre.Enabled = valore
        txtCodCategoria.Enabled = valore
        txtCodLinea.Enabled = valore
        txtCodUnitaMisura.Enabled = valore
        txtCodFornitore.Enabled = valore
        txtCodCondizioniPag.Enabled = valore
        txtUltPrezzoAcq.Enabled = valore
        txtRicarico.Enabled = valore
        txtPrezzoVendita.Enabled = valore
        txtPrezzoMinVen.Enabled = valore
        txtSconto1Perc.Enabled = valore
        'GIU300719
        chkOkListino.Enabled = valore
        If chkOkListino.Visible = True Then
            If valore = True And chkOkListino.Checked = True Then
                txtPrezzoVendita.Enabled = valore
                txtPrezzoMinVen.Enabled = valore
                txtSconto1Perc.Enabled = valore
            Else
                txtPrezzoVendita.Enabled = False
                txtPrezzoMinVen.Enabled = False
                txtSconto1Perc.Enabled = False
            End If
        End If
        '---------
        txtSconto2Perc.Enabled = valore
        txtConfezioneDa.Enabled = valore
        txtNAnniGaranzia.Enabled = valore
        txtNAnniScadBatterie.Enabled = valore
        txtNAnniScadElettrodi.Enabled = valore
        chkID1HS1.Enabled = valore : chkID2FR2.Enabled = valore : chkID3FR3.Enabled = valore : chkID4FRX.Enabled = valore
        txtCodAliquotaIva.Enabled = valore
        checkSottoscorta.Enabled = valore
        checkArticoloDiVendita.Enabled = valore
        checkGestioneInLotti.Enabled = valore
        radioArticolo.Enabled = valore
        radioImballo.Enabled = valore
        radioBancale.Enabled = valore
        radioKit.Enabled = valore
        RadioVoceGenerica.Enabled = valore
        ddlCatgoria.Enabled = valore
        ddlCondizioniPag.Enabled = valore
        ddlFornitore.Enabled = valore
        ddlLinea.Enabled = valore
        ddlTipoCodBarre.Enabled = valore
        btnTipoCodBarre.Enabled = valore
        ddlUnitaMisura.Enabled = valore
        ddlAliquotaIva.Enabled = valore
        'giu031111 non gestito per adesso EAN btnRicalcolaCodContr.Enabled = valore
        GWDescEstesa.Enabled = valore
        GWArticoliCTV.Enabled = valore
        DistintaBase1.Enabled = valore
        GWFornitoriSec.Enabled = valore
        GWPrezziAcquisto.Enabled = valore
        btnTrovaCondizioniPag.Enabled = valore
        btnTrovaFornitore.Enabled = valore
        btnTrovaAliquotaIva.Enabled = valore
        btnCategoriaArt.Enabled = valore
        btnLinea.Enabled = valore
        btnMisure.Enabled = valore
        btnPagamenti.Enabled = valore

        TxtDefCodBase.Enabled = False
        btnAnnDefBaseOpz.Visible = False
        btnDefBaseOpz.Enabled = valore

        btnTrovaCategoria.Enabled = False
        btnTrovaLinea.Enabled = False
        btnTrovaUnitaMisura.Enabled = False
    End Sub

    Private Sub InizializzaCampiOK()
        txtCodBase.BackColor = Def.SEGNALA_OK
        txtCodOpzione.BackColor = Def.SEGNALA_OK
        'GIU290719
        txtCodBase.ToolTip = ""
        txtCodOpzione.ToolTip = ""
        '---------
        'giu031111 non gestito per adesso EAN
        ' ''txtCodArticolo.BackColor = Def.SEGNALA_OK
        ' ''txtCodAzienda.BackColor = Def.SEGNALA_OK
        ' ''txtCodStEAN.BackColor = Def.SEGNALA_OK
        ' ''txtCodContr.BackColor = Def.SEGNALA_OK
        txtDescBreve.BackColor = Def.SEGNALA_OK
        txtDescBreve.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
        lbTitolare.BackColor = Def.SEGNALA_OK
        lbRiferimento.BackColor = Def.SEGNALA_OK
        txtCodArtFornitore.BackColor = Def.SEGNALA_OK
        txtCodBarre.BackColor = Def.SEGNALA_OK
        txtCodCategoria.BackColor = Def.SEGNALA_OK
        txtCodLinea.BackColor = Def.SEGNALA_OK
        txtCodUnitaMisura.BackColor = Def.SEGNALA_OK
        txtCodFornitore.BackColor = Def.SEGNALA_OK
        txtCodCondizioniPag.BackColor = Def.SEGNALA_OK
        txtCodAliquotaIva.BackColor = Def.SEGNALA_OK
        txtPeso.BackColor = Def.SEGNALA_OK
        txtAltezza.BackColor = Def.SEGNALA_OK
        txtLunghezza.BackColor = Def.SEGNALA_OK
        txtLarghezza.BackColor = Def.SEGNALA_OK
        txtQtaSottoscorta.BackColor = Def.SEGNALA_OK
        txtQtaRiordino.BackColor = Def.SEGNALA_OK
        txtGiorniConsegna.BackColor = Def.SEGNALA_OK
        txtUltPrezzoAcq.BackColor = Def.SEGNALA_OK
        txtRicarico.BackColor = Def.SEGNALA_OK
        txtPrezzoVendita.BackColor = Def.SEGNALA_OK
        lblMessListVendD.BackColor = Def.SEGNALA_OK
        chkOkListino.BackColor = Def.SEGNALA_INFO
        txtPrezzoMinVen.BackColor = Def.SEGNALA_OK
        txtSconto1Perc.BackColor = Def.SEGNALA_OK
        txtSconto2Perc.BackColor = Def.SEGNALA_OK
        txtConfezioneDa.BackColor = Def.SEGNALA_OK
        txtNAnniGaranzia.BackColor = Def.SEGNALA_OK
        txtNAnniScadBatterie.BackColor = Def.SEGNALA_OK
        txtNAnniScadElettrodi.BackColor = Def.SEGNALA_OK
    End Sub

    Private Sub SvuotaCampi()
        InizializzaCampiOK()
        'giu210814
        lblCArtSel.Text = String.Empty
        lblDArtSel.Text = String.Empty
        '---------
        txtCodBase.Text = String.Empty
        txtCodOpzione.Text = String.Empty
        'GIU290719
        txtCodBase.ToolTip = ""
        txtCodOpzione.ToolTip = ""
        '---------
        lblCodArticolo.Text = ""
        'giu031111 non gestito per adesso EAN
        ' ''txtCodArticolo.Text = String.Empty
        ' ''txtCodAzienda.Text = String.Empty
        ' ''txtCodStEAN.Text = String.Empty
        ' ''txtCodContr.Text = String.Empty
        txtDescBreve.Text = String.Empty
        lbTitolare.Text = String.Empty
        lbRiferimento.Text = String.Empty
        txtCodArtFornitore.Text = String.Empty
        txtCodBarre.Text = String.Empty
        txtCodCategoria.Text = String.Empty
        txtCodLinea.Text = String.Empty
        txtCodUnitaMisura.Text = String.Empty
        txtCodFornitore.Text = String.Empty
        txtCodCondizioniPag.Text = FormattaNumero("0")
        txtCodAliquotaIva.Text = FormattaNumero("0")
        txtPeso.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtAltezza.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtLunghezza.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtLarghezza.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        'giu070414
        txtNAnniGaranzia.Text = FormattaNumero("0")
        txtNAnniScadBatterie.Text = FormattaNumero("0")
        txtNAnniScadElettrodi.Text = FormattaNumero("0")
        '---------
        txtQtaSottoscorta.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtQtaRiordino.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtGiorniConsegna.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtUltPrezzoAcq.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        txtRicarico.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        txtPrezzoVendita.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        lblMessListVendD.Text = ""
        chkOkListino.Visible = False
        chkOkListino.AutoPostBack = False
        txtPrezzoMinVen.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        txtSconto1Perc.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        txtSconto2Perc.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        txtConfezioneDa.Text = "0"
        txtNAnniGaranzia.Text = "0"
        txtNAnniScadBatterie.Text = "0"
        txtNAnniScadElettrodi.Text = "0"
        chkID1HS1.Checked = False : chkID2FR2.Checked = False : chkID3FR3.Checked = False : chkID4FRX.Checked = False
        checkSottoscorta.Checked = True
        checkArticoloDiVendita.Checked = True
        checkGestioneInLotti.Checked = True
        radioArticolo.Checked = False
        radioImballo.Checked = False
        radioBancale.Checked = False
        radioKit.Checked = False
        RadioVoceGenerica.Checked = False
        ddlCatgoria.Text = String.Empty
        ddlCondizioniPag.Text = String.Empty
        ddlFornitore.Text = String.Empty
        ddlLinea.SelectedValue = String.Empty
        ddlTipoCodBarre.SelectedValue = String.Empty
        lblTipoCodBarre.Text = String.Empty
        ddlUnitaMisura.SelectedValue = String.Empty
        ddlAliquotaIva.SelectedValue = String.Empty
        GWDescEstesa.SvuotaGridView()
        GWArticoliCTV.SvuotaGridView()
        DistintaBase1.SvuotaGridView()
        GWFornitoriSec.SvuotaGridView()
        GWPrezziAcquisto.SvuotaGridView()
    End Sub

    Private Sub PopolaCampi(ByVal AnaMag As AnaMagEntity)
        Dim ListVDSys As New ListVenD : Dim ListVenDSys As New ListVenD
        Dim myListVD As ListVenDEntity
        Dim arrListVD As ArrayList
        Try
            lblCodArticolo.Text = AnaMag.CodArticolo
            txtCodBase.Text = Mid(AnaMag.CodArticolo, 1, AnaMag.LBase)
            txtCodOpzione.Text = Mid(AnaMag.CodArticolo, AnaMag.LBase + 1)
            lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
            'GIU290719
            txtCodBase.ToolTip = ""
            txtCodOpzione.ToolTip = ""
            '---------
            If lblCodArticolo.Text.Length > txtCodBase.MaxLength Then
                txtCodBase.BackColor = Def.SEGNALA_KO
                txtCodOpzione.BackColor = Def.SEGNALA_KO
            End If

            'Pier170112
            TxtDefCodBase.Text = AnaMag.LBase
            LblDefBasOpz.Text = AnaMag.LOpz


            'giu031111 per adesso è tutto non gestito
            ' ''txtCodAzienda.Text = AnaMag.CodAziendaEAN
            ' ''txtCodStEAN.Text = AnaMag.CodArticoloEAN
            ' ''txtCodContr.Text = AnaMag.CodControlloEAN
            txtDescBreve.Text = AnaMag.Descrizione
            txtCodArtFornitore.Text = AnaMag.CodiceDelFornitore
            'giu210814
            lblCArtSel.Text = AnaMag.CodArticolo
            lblDArtSel.Text = Mid(AnaMag.Descrizione.Trim, 1, 50)
            '---------
            txtPeso.Text = FormattaNumero(AnaMag.PesoUnitario, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtAltezza.Text = FormattaNumero(AnaMag.Altezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtLunghezza.Text = FormattaNumero(AnaMag.Lunghezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtLarghezza.Text = FormattaNumero(AnaMag.Larghezza, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            'giu070414
            txtNAnniGaranzia.Text = FormattaNumero(AnaMag.NAnniGaranzia)
            txtNAnniScadElettrodi.Text = FormattaNumero(AnaMag.NAnniScadElettrodi)
            txtNAnniScadBatterie.Text = FormattaNumero(AnaMag.NAnniScadBatterie)
            chkID1HS1.Checked = AnaMag.IDModulo1 : chkID2FR2.Checked = AnaMag.IDModulo2 : chkID3FR3.Checked = AnaMag.IDModulo3 : chkID4FRX.Checked = AnaMag.IDModulo4
            '---------
            txtQtaSottoscorta.Text = FormattaNumero(AnaMag.Sottoscorta, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtQtaRiordino.Text = FormattaNumero(AnaMag.QtaOrdine, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtGiorniConsegna.Text = FormattaNumero(AnaMag.GiorniConsegna, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtUltPrezzoAcq.Text = FormattaNumero(AnaMag.PrezzoAcquisto, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            txtRicarico.Text = FormattaNumero(AnaMag.Ricarico, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            txtConfezioneDa.Text = FormattaNumero(AnaMag.Confezione, 0)

            checkSottoscorta.Checked = AnaMag.AvvisaSottoscorta
            checkArticoloDiVendita.Checked = AnaMag.ArticoloDiVendita
            checkGestioneInLotti.Checked = AnaMag.GestLotti
            'giu270312
            radioArticolo.Checked = False
            radioKit.Checked = False
            radioImballo.Checked = False
            radioBancale.Checked = False
            RadioVoceGenerica.Checked = False
            Select Case (AnaMag.TipoArticolo)
                Case 0 : radioArticolo.Checked = True
                Case 1 : radioKit.Checked = True
                Case 2 : radioImballo.Checked = True
                Case 3 : radioBancale.Checked = True
                Case 9 : RadioVoceGenerica.Checked = True
            End Select

            txtCodBarre.Text = AnaMag.CodBarre
            PosizionaItemDDL(AnaMag.TipoCodice.Trim, ddlTipoCodBarre) 'GIU181111
            'giu140419
            lblTipoCodBarre.Text = GetDesTipoCodBarre()
            '---------
            ValorizzaTextBoxCombinata(AnaMag.Categoria, txtCodCategoria, ddlCatgoria)
            ValorizzaTextBoxCombinata(AnaMag.Linea, txtCodLinea, ddlLinea)
            ValorizzaTextBoxCombinata(AnaMag.Um, txtCodUnitaMisura, ddlUnitaMisura)
            ValorizzaTextBoxCombinata(AnaMag.CodiceFornitore, txtCodFornitore, ddlFornitore)
            ValorizzaTextBoxCombinata(AnaMag.CodPagamento, txtCodCondizioniPag, ddlCondizioniPag)
            ValorizzaTextBoxCombinata(AnaMag.CodIva, txtCodAliquotaIva, ddlAliquotaIva)
            'giu070115
            txtSconto2Perc.Text = FormattaNumero(AnaMag.ScFornitore, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
            '---------
        Catch ex As Exception
            Session(ERRORE) = "(PopolaCampi GESTIONE ARTICOLI) " & ex.Message.Trim
        End Try
        Try
            arrListVD = ListVDSys.getListVenDByCodLisCodArt(1, AnaMag.CodArticolo)
            If (arrListVD.Count > 0) Then
                myListVD = CType(arrListVD(0), ListVenDEntity)
                txtPrezzoVendita.Text = FormattaNumero(myListVD.Prezzo, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                txtPrezzoMinVen.Text = FormattaNumero(myListVD.PrezzoMinimo, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                txtSconto1Perc.Text = FormattaNumero(myListVD.Sconto_1, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
                'giu070115 txtSconto2Perc.Text = FormattaNumero(myListVD.Sconto_2, App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
                lblMessListVendD.BackColor = Def.SEGNALA_OK
                lblMessListVendD.Text = "Articolo presente nel listino vendita"
                chkOkListino.Visible = False
                chkOkListino.AutoPostBack = False
            Else
                txtPrezzoVendita.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                txtPrezzoMinVen.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                txtSconto1Perc.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
                'giu070115 txtSconto2Perc.Text = FormattaNumero("0", App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
                If Session(SWOP) = SWOPNUOVO Then
                    lblMessListVendD.BackColor = Def.SEGNALA_KO
                    lblMessListVendD.Text = "L'Articolo sarà inserito nel listino"
                    chkOkListino.Visible = False
                    chkOkListino.AutoPostBack = False
                Else
                    lblMessListVendD.BackColor = Def.SEGNALA_KO
                    lblMessListVendD.Text = "Articolo NON presente nel listino"
                    chkOkListino.BackColor = Def.SEGNALA_INFO
                    chkOkListino.Visible = True
                    chkOkListino.AutoPostBack = False
                    chkOkListino.Checked = False
                    chkOkListino.AutoPostBack = True
                End If
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        If (Not PopolaTitolareRiferimento(AnaMag.CodiceFornitore)) Then
            Exit Sub
        End If
    End Sub
    Private Function GetDesTipoCodBarre() As String 'giu140419
        GetDesTipoCodBarre = ""
        Dim dvTipoCB As DataView
        dvTipoCB = SqlDataSourceCodBarre.Select(DataSourceSelectArguments.Empty)
        dvTipoCB.RowFilter = "Tipo_Codice='" & ddlTipoCodBarre.SelectedValue & "'"
        If dvTipoCB.Count > 0 Then
            If Not IsDBNull(dvTipoCB.Item(0).Item("Descrizione")) Then
                GetDesTipoCodBarre = dvTipoCB.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Function
    Private Function LeggiCampi(ByRef myAnaMag As AnaMagEntity, ByRef myListVD As ListVenDEntity, _
                                ByRef myListFornSec As List(Of FornSecondariEntity), _
                                ByRef myListAnaMagDes As List(Of AnaMagDesEntity), _
                                ByRef myListAnaMagCTV As List(Of AnaMagCTVEntity), _
                                ByRef myListDistBase As List(Of DistBaseEntity)) As Boolean
        Try
            myAnaMag = New AnaMagEntity
            lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
            myAnaMag.CodArticolo = lblCodArticolo.Text.Trim


            myAnaMag.LBase = CInt(txtCodBase.Text.Length)
            myAnaMag.LOpz = CInt(txtCodOpzione.Text.Length)

            'GIU031111 non gestito
            ' ''myAnaMag.CodAziendaEAN = txtCodAzienda.Text
            ' ''myAnaMag.CodArticoloEAN = txtCodStEAN.Text
            ' ''myAnaMag.CodControlloEAN = txtCodContr.Text
            myAnaMag.Descrizione = txtDescBreve.Text

            myAnaMag.PesoUnitario = CDec(txtPeso.Text)
            myAnaMag.Altezza = CDec(txtAltezza.Text)
            myAnaMag.Lunghezza = CDec(txtLunghezza.Text)
            myAnaMag.Larghezza = CDec(txtLarghezza.Text)
            'giu070414
            myAnaMag.NAnniGaranzia = CInt(txtNAnniGaranzia.Text)
            myAnaMag.NAnniScadBatterie = CInt(txtNAnniScadBatterie.Text)
            myAnaMag.NAnniScadElettrodi = CInt(txtNAnniScadElettrodi.Text)
            'giu270618
            myAnaMag.IDModulo1 = chkID1HS1.Checked : myAnaMag.IDModulo2 = chkID2FR2.Checked
            myAnaMag.IDModulo3 = chkID3FR3.Checked : myAnaMag.IDModulo4 = chkID4FRX.Checked
            '---------
            myAnaMag.Sottoscorta = CDec(txtQtaSottoscorta.Text)
            myAnaMag.QtaOrdine = CDec(txtQtaRiordino.Text)
            myAnaMag.GiorniConsegna = CInt(txtGiorniConsegna.Text)
            myAnaMag.PrezzoAcquisto = CDec(txtUltPrezzoAcq.Text)
            myAnaMag.Ricarico = CDec(txtRicarico.Text)
            myAnaMag.Confezione = CInt(txtConfezioneDa.Text)
            myAnaMag.CodIva = CInt(txtCodAliquotaIva.Text)
            myAnaMag.CodPagamento = CInt(txtCodCondizioniPag.Text)

            myAnaMag.CodiceDelFornitore = txtCodArtFornitore.Text
            'giu031111 GIU181111
            If ddlTipoCodBarre.SelectedIndex > 0 Then
                myAnaMag.TipoCodice = ddlTipoCodBarre.SelectedValue
            Else
                myAnaMag.TipoCodice = ""
            End If
            '-------
            myAnaMag.CodBarre = txtCodBarre.Text
            myAnaMag.Categoria = txtCodCategoria.Text
            myAnaMag.Linea = txtCodLinea.Text
            myAnaMag.Um = txtCodUnitaMisura.Text
            myAnaMag.CodiceFornitore = txtCodFornitore.Text

            myAnaMag.AvvisaSottoscorta = checkSottoscorta.Checked
            myAnaMag.ArticoloDiVendita = checkArticoloDiVendita.Checked
            myAnaMag.GestLotti = checkGestioneInLotti.Checked

            If (radioArticolo.Checked) Then
                myAnaMag.TipoArticolo = 0
            ElseIf (radioKit.Checked) Then
                myAnaMag.TipoArticolo = 1
            ElseIf (radioImballo.Checked) Then
                myAnaMag.TipoArticolo = 2
            ElseIf (radioBancale.Checked) Then
                myAnaMag.TipoArticolo = 3
            ElseIf (RadioVoceGenerica.Checked) Then
                myAnaMag.TipoArticolo = 9
            End If

            myAnaMag.DataInizioProd = Date.Today
            myAnaMag.DataFineProd = CDate("31/12/2100")
            myAnaMag.ScFornitore = CDec(txtSconto2Perc.Text) 'giu070115

            myListVD = New ListVenDEntity
            myListVD.Codice = 1
            myListVD.Cod_Articolo = lblCodArticolo.Text.Trim
            myListVD.Riga = 1

            myListVD.Prezzo = CDec(txtPrezzoVendita.Text)
            myListVD.PrezzoMinimo = CDec(txtPrezzoMinVen.Text)
            myListVD.Sconto_1 = CDec(txtSconto1Perc.Text)
            myListVD.Sconto_2 = 0 'giu070115 CDec(txtSconto2Perc.Text)

            myListFornSec = GWFornitoriSec.GetListFornitoriSec()
            myListAnaMagDes = GWDescEstesa.GetListDescEstesa()
            myListAnaMagCTV = GWArticoliCTV.GetListAnaMagCTV()
            myListDistBase = DistintaBase1.GetListDistBase()
        Catch ex As Exception
            Session(ERRORE) = "(LeggiCampi GESTIONE ARTICOLI per aggiornamento su DB) " & ex.Message.Trim
        End Try
    End Function

    Private Sub ControllaEsistenzaCampiInErrore()
        'giu031111 NON GESTITO
        ' ''txtCodAzienda.BackColor = Def.SEGNALA_KO Or _
        ' ''    txtCodStEAN.BackColor = Def.SEGNALA_KO Or _
        ' ''    txtCodContr.BackColor = Def.SEGNALA_KO Or _
        Session(SWERRORI_AGGIORNAMENTO) = False
        If ( _
            txtCodBase.BackColor = Def.SEGNALA_KO Or _
            txtCodOpzione.BackColor = Def.SEGNALA_KO Or _
            txtDescBreve.BackColor = Def.SEGNALA_KO Or _
            txtPeso.BackColor = Def.SEGNALA_KO Or _
            txtAltezza.BackColor = Def.SEGNALA_KO Or _
            txtLunghezza.BackColor = Def.SEGNALA_KO Or _
            txtLarghezza.BackColor = Def.SEGNALA_KO Or _
            txtCodArtFornitore.BackColor = Def.SEGNALA_KO Or _
            txtQtaSottoscorta.BackColor = Def.SEGNALA_KO Or _
            txtQtaRiordino.BackColor = Def.SEGNALA_KO Or _
            txtGiorniConsegna.BackColor = Def.SEGNALA_KO Or _
            txtCodBarre.BackColor = Def.SEGNALA_KO Or _
            txtCodCategoria.BackColor = Def.SEGNALA_KO Or _
            txtCodLinea.BackColor = Def.SEGNALA_KO Or _
            txtCodUnitaMisura.BackColor = Def.SEGNALA_KO Or _
            txtCodFornitore.BackColor = Def.SEGNALA_KO Or _
            txtCodCondizioniPag.BackColor = Def.SEGNALA_KO Or _
            txtCodAliquotaIva.BackColor = Def.SEGNALA_KO Or _
            txtUltPrezzoAcq.BackColor = Def.SEGNALA_KO Or _
            txtRicarico.BackColor = Def.SEGNALA_KO Or _
            txtPrezzoVendita.BackColor = Def.SEGNALA_KO Or _
            txtPrezzoMinVen.BackColor = Def.SEGNALA_KO Or _
            txtSconto1Perc.BackColor = Def.SEGNALA_KO Or _
            txtSconto2Perc.BackColor = Def.SEGNALA_KO Or _
            txtConfezioneDa.BackColor = Def.SEGNALA_KO Or _
            txtNAnniGaranzia.BackColor = Def.SEGNALA_KO Or _
            txtNAnniScadBatterie.BackColor = Def.SEGNALA_KO Or _
            txtNAnniScadElettrodi.BackColor = Def.SEGNALA_KO _
            ) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Sub

    Private Sub ControlloCampiObbligatori()
        Session(SWERRORI_AGGIORNAMENTO) = False
        lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
        If (String.IsNullOrEmpty(lblCodArticolo.Text)) Then
            txtCodBase.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        ElseIf Left(lblCodArticolo.Text.ToString.Trim, 1) = "_" Then 'giu190412 non possono iniziare con _
            txtCodBase.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
        If (String.IsNullOrEmpty(txtDescBreve.Text)) Then
            txtDescBreve.BackColor = SEGNALA_KO
            txtDescBreve.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
            Session(SWERRORI_AGGIORNAMENTO) = True
        ElseIf txtDescBreve.Text.Trim.Length > 150 Then
            txtDescBreve.BackColor = SEGNALA_KO
            txtDescBreve.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
        If (String.IsNullOrEmpty(txtCodUnitaMisura.Text)) Then
            txtCodUnitaMisura.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
        If (String.IsNullOrEmpty(txtCodCategoria.Text)) Then
            txtCodCategoria.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Sub

    Private Function ControlloCampiDati() As Boolean
        Dim bControlloOk As Boolean = True
        Dim bCampoPrezzoVenditaOk As Boolean = Session(CAMPO_PREZZO_VENDITA_OK)

        If ((String.IsNullOrEmpty(txtPrezzoVendita.Text) Or Not IsNumeric(txtPrezzoVendita.Text.Trim)) And Not bCampoPrezzoVenditaOk) Then
            txtPrezzoVendita.BackColor = SEGNALA_KO
            Session(CAMPO_PREZZO_VENDITA_OK) = False
            Session(MODALPOPUP_CALLBACK_METHOD) = "ForzaPrezzoVenditaControlloOk"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati articoli", "Attenzione, Articolo senza prezzo di vendita. Vuoi proseguire comunque?", WUC_ModalPopup.TYPE_CONFIRM)
            bControlloOk = False
        ElseIf CDec(txtPrezzoVendita.Text.Trim) = 0 And Not bCampoPrezzoVenditaOk Then
            txtPrezzoVendita.BackColor = SEGNALA_KO
            Session(CAMPO_PREZZO_VENDITA_OK) = False
            Session(MODALPOPUP_CALLBACK_METHOD) = "ForzaPrezzoVenditaControlloOk"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati articoli", "Attenzione, Articolo senza prezzo di vendita. Vuoi proseguire comunque?", WUC_ModalPopup.TYPE_CONFIRM)
            bControlloOk = False
        End If
        ControlloCampiDati = bControlloOk
    End Function
    Public Sub ForzaPrezzoVenditaControlloOk()
        txtPrezzoVendita.BackColor = Def.SEGNALA_OK
        Session(CAMPO_PREZZO_VENDITA_OK) = True
        RicontrollaAggiorna()
    End Sub

    Private Function PopolaTitolareRiferimento(ByVal codice As String) As Boolean
        Dim FornSys As New Fornitore
        Dim myForn As FornitoreEntity
        Dim arrForn As ArrayList

        Try
            If codice = "" Then
                lbTitolare.Text = ""
                lbRiferimento.Text = ""
            Else
                arrForn = FornSys.getFornitoreByCodice(codice)
                If (arrForn.Count > 0) Then
                    myForn = CType(arrForn(0), FornitoreEntity)
                    lbTitolare.Text = myForn.Titolare
                    lbRiferimento.Text = myForn.Riferimento
                Else
                    lbTitolare.Text = ""
                    lbRiferimento.Text = ""
                End If
            End If
            PopolaTitolareRiferimento = True
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            PopolaTitolareRiferimento = False
        End Try
    End Function

#End Region

#Region "Aggiornamento"

    Private Sub Aggiornamento(ByRef myAnaMag As AnaMagEntity, ByRef myListVD As ListVenDEntity, _
                                ByRef myListFornSec As List(Of FornSecondariEntity), _
                                ByRef myListAnaMagDes As List(Of AnaMagDesEntity), _
                                ByRef myListAnaMagCTV As List(Of AnaMagCTVEntity), _
                                ByRef myListDistBase As List(Of DistBaseEntity))

        lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
        If (AggiornaArticolo(myAnaMag)) Then
            If (AggiornaDescEstesa(myListAnaMagDes)) Then
                If (AggiornaAnaMagCTV(myListAnaMagCTV)) Then
                    If (AggiornaFornitoriSec(myListFornSec)) Then
                        If (AggiornaDistBase(myListDistBase)) Then
                            If (AggiornaListVD(myListVD)) Then
                                Session(COD_ARTICOLO) = lblCodArticolo.Text.Trim
                                GridViewArticoli.Enabled = True
                                SvuotaCampi()
                                CampiSetEnabledTo(False)
                                ' ''Tabs.ActiveTabIndex = 0
                                setPulsantiModalitaConsulta()
                                'GIU240123 LASCIO COSI SENZA AGGIORNARE LISTINI DOLLARI DA VERIFICARE XKE VA IN ERRORE
                                ' ''If (Session(SWOP).Equals(SWOPNUOVO)) Or lblMessListVendD.BackColor = SEGNALA_KO Then
                                ' ''    Session(ELEMENTO_LISTVEND) = myListVD
                                ' ''    ApriSceltaListiniDaAgg()
                                ' ''End If
                                CaricaVariabili()
                                _CodiceBase = Session(COD_ARTICOLO)
                                SetFiltroBaseSQLDataSourceArticoli()
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Function AggiornaArticolo(ByVal myAnaMag As AnaMagEntity) As Boolean
        Dim AMSys As New AnaMag

        Try
            If (Not AMSys.InsertUpdateAnaMag(myAnaMag)) Then
                If (Session(SWOP).Equals(SWOPNUOVO)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile inserire l'articolo", WUC_ModalPopup.TYPE_ALERT)
                ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile modificare l'articolo", WUC_ModalPopup.TYPE_ALERT)
                End If
                AggiornaArticolo = False
                Exit Function
            End If
            Dim errorMsg As String = ""
            Call AggArtDiMag(myAnaMag.CodArticolo.Trim, myAnaMag.Sottoscorta, errorMsg)
            If (Not App.CaricaArticoli(Session(ESERCIZIO), errorMsg)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Caricamento Dati", _
                    String.Format("Errore nel caricamento Elenco Articoli, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_INFO)
                SessionUtility.LogOutUtente(Session, Response)
                Return False
                Exit Function
            End If
        Catch EX As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", EX.Message, WUC_ModalPopup.TYPE_ERROR)
            AggiornaArticolo = False
            Exit Function
        End Try
        AggiornaArticolo = True
    End Function
    'giu280920
    Private Function AggArtDiMag(ByVal _CArt As String, ByVal _Sottoscorta As Decimal, ByRef _strErrore As String) As Boolean
        _strErrore = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE ArtDiMag SET Sottoscorta= " & _Sottoscorta.ToString.Trim.Replace(",", ".")
            SQLStr += " WHERE (Codice_Magazzino = 0 OR Codice_Magazzino = 1) AND (Cod_Articolo = '" & _CArt.Trim & "')"
            AggArtDiMag = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            _strErrore = ex.Message.Trim
            AggArtDiMag = False
            ObjDB = Nothing
        End Try
        ObjDB = Nothing
    End Function
    Private Function AggiornaDescEstesa(ByVal myListAnaMagDes As List(Of AnaMagDesEntity)) As Boolean
        Dim AMDesSys As New AnaMagDes

        Try
            AMDesSys.delAnaMagDesByCodiceArticolo(Session(COD_ARTICOLO))
            For Each myAnaMagDes As AnaMagDesEntity In myListAnaMagDes
                If (Not AMDesSys.InsertAnaMagDes(myAnaMagDes)) Then
                    If (Session(SWOP).Equals(SWOPNUOVO)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento della ""descrizione estesa""", WUC_ModalPopup.TYPE_ALERT)
                    ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato modificato, ma si è presentato un errore non gestito durante l'aggiornamento della ""descrizione estesa""", WUC_ModalPopup.TYPE_ALERT)
                    End If
                    AggiornaDescEstesa = False
                    Exit Function
                End If
            Next
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaDescEstesa = False
            Exit Function
        End Try

        AggiornaDescEstesa = True
    End Function
    'GIU011222
    Private Function AggiornaAnaMagCTV(ByVal myListAnaMagCTV As List(Of AnaMagCTVEntity)) As Boolean
        Dim AMCTVSys As New AnaMagCTV

        Try
            AMCTVSys.delAnaMagCTVByCodiceArticolo(Session(COD_ARTICOLO))
            For Each myAnaMagCTV As AnaMagCTVEntity In myListAnaMagCTV
                If (Not AMCTVSys.InsertAnaMagCTV(myAnaMagCTV)) Then
                    If (Session(SWOP).Equals(SWOPNUOVO)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento dei ""Codici Tipo Valore XML""", WUC_ModalPopup.TYPE_ALERT)
                    ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato modificato, ma si è presentato un errore non gestito durante l'aggiornamento dei ""Codici Tipo Valore XML""", WUC_ModalPopup.TYPE_ALERT)
                    End If
                    AggiornaAnaMagCTV = False
                    Exit Function
                End If
            Next
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaAnaMagCTV = False
            Exit Function
        End Try

        AggiornaAnaMagCTV = True
    End Function

    Private Function AggiornaFornitoriSec(ByVal myListFornSec As List(Of FornSecondariEntity)) As Boolean
        Dim FornSecSys As New FornSecondari

        Try
            FornSecSys.delFornSecByCodiceArticolo(Session(COD_ARTICOLO))
            For Each myFornSec As FornSecondariEntity In myListFornSec
                If (Not FornSecSys.InsertFornitoriSec(myFornSec)) Then
                    If (Session(SWOP).Equals(SWOPNUOVO)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento dei ""fornitori secondari""", WUC_ModalPopup.TYPE_ALERT)
                    ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato modificato, ma si è presentato un errore non gestito durante l'aggiornamento dei ""fornitori secondari""", WUC_ModalPopup.TYPE_ALERT)
                    End If
                    AggiornaFornitoriSec = False
                    Exit Function
                End If
            Next
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaFornitoriSec = False
            Exit Function
        End Try

        AggiornaFornitoriSec = True
    End Function

    Private Function AggiornaListVD(ByVal myListVenD As ListVenDEntity) As Boolean
        If Session(SWOP) = SWOPNUOVO Then
            'ok inserisco
        Else
            'giu120719 se è già presente nel listino ok aggiorno altrimenti no 
            Dim ListVDSys As New ListVenD
            Dim arrListVD As ArrayList
            arrListVD = ListVDSys.getListVenDByCodLisCodArt(1, myListVenD.Cod_Articolo)
            If (arrListVD.Count > 0) Then
                'OK AGGIORNO
            Else 'non aggiorno perche non presente nel LISTINO DI VENDITA
                If chkOkListino.Checked = False Then
                    AggiornaListVD = True
                    Exit Function
                End If
            End If
        End If
        Dim ListVenDSys As New ListVenD

        Try
            If (Not ListVenDSys.InsertUpdateListVenD(myListVenD)) Then
                If (Session(SWOP).Equals(SWOPNUOVO)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento dei ""dati di listino""", WUC_ModalPopup.TYPE_ALERT)
                ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "L'articolo è stato modificato, ma si è presentato un errore non gestito durante l'aggiornamento dei ""dati di listino""", WUC_ModalPopup.TYPE_ALERT)
                End If
                AggiornaListVD = False
                Exit Function
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaListVD = False
            Exit Function
        End Try

        AggiornaListVD = True
    End Function

    Private Sub AggiornaListiniVD(ByVal myListVenD As ListVenDEntity)
        Dim ListVenDSys As New ListVenD

        Try
            If (Not ListVenDSys.InsertUpdateListVenD(myListVenD)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento dei ""dati di listino""", WUC_ModalPopup.TYPE_ALERT)
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub

    Private Function AggiornaDistBase(ByVal myListDistBase As List(Of DistBaseEntity)) As Boolean
        Dim DBaseSys As New DistBase

        Try
            DBaseSys.delDistBaseByCodiceArticolo(Session(COD_ARTICOLO))
            For Each myDistBase As DistBaseEntity In myListDistBase
                If (Not DBaseSys.InsertDistBase(myDistBase)) Then
                    If (Session(SWOP).Equals(SWOPNUOVO)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato inserito, ma si è presentato un errore non gestito durante l'aggiornamento della ""distinta base""", WUC_ModalPopup.TYPE_ALERT)
                    ElseIf (Session(SWOP).Equals(SWOPMODIFICA)) Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "L'articolo è stato modificato, ma si è presentato un errore non gestito durante l'aggiornamento della ""distinta base""", WUC_ModalPopup.TYPE_ALERT)
                    End If
                    AggiornaDistBase = False
                    Exit Function
                End If
            Next
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaDistBase = False
            Exit Function
        End Try

        AggiornaDistBase = True
    End Function
#End Region

    Private Sub VisualizzaArticolo()
        Dim AMSys As New AnaMag
        Dim myAM As AnaMagEntity
        Dim arrAM As ArrayList
        'giu120424
        '''Try
        'giu200123
        Dim myID As String = Session(COD_ARTICOLO)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = "-1"
            End If
            '-------------------
            arrAM = AMSys.getAnaMagByCodice(myID)
            If (arrAM.Count > 0) Then
                myAM = CType(arrAM(0), AnaMagEntity)
                CampiSetEnabledTo(False)
                PopolaCampi(myAM)
                GWDescEstesa.PopolaGridView()
                GWArticoliCTV.PopolaGridView()
                GWFornitoriSec.PopolaGridView()
                GWPrezziAcquisto.PopolaGridView()
                DistintaBase1.PopolaGridView()
            Else
                Me.SvuotaCampi()
                btnNuovo.Enabled = True
                btnModifica.Enabled = False
                btnElimina.Enabled = False
                btnStampa.Enabled = False
                btnStampaArtFor.Enabled = False
                btnAnnulla.Enabled = False
                btnAggiorna.Enabled = False
                btnAggiungiFornSec.Visible = False
            End If
        '''Catch ex As Exception
        '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '''    ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        '''    Return
        '''End Try
    End Sub

    Private Sub ValorizzaTextBoxCombinata(ByVal valore As String, ByRef txt As TextBox, ByRef ddl As DropDownList)
        txt.Text = valore
        'giu140911 cosa serve ?????
        If ddl.Items.Count = 1 Then 'giu140911 aggiunto solo se è la prima volta
            'giu140911 cosa serve ?????
            ddl.Items.Clear()
            ddl.Items.Add("")
            ddl.DataBind()
        End If
        PosizionaItemDDLTxt(txt, ddl)
        If (txt.BackColor = SEGNALA_KO) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        Else
            Session(SWERRORI_AGGIORNAMENTO) = False
        End If
    End Sub

    Private Sub RicontrollaAggiorna()
        Call btnAggiorna_Click(Nothing, Nothing)
        '' ''GIU240920 ATTENZIONE QUI SONO LE STESSE FUNZIONI DI BTNAGGIORNA, QUINDI SE SI CAMBIASSE QUALCOSA CAMBIARLO ANCHE IN BTN
        ' ''Dim myAnaMag As AnaMagEntity = Nothing
        ' ''Dim myListVenD As ListVenDEntity = Nothing
        ' ''Dim myListFornSec As List(Of FornSecondariEntity) = Nothing
        ' ''Dim myListAnaMagDes As List(Of AnaMagDesEntity) = Nothing
        ' ''Dim myListDistBase As List(Of DistBaseEntity) = Nothing

        ' ''CheckNewCodiceArticolo()
        ' ''ControllaEsistenzaCampiInErrore()
        ' ''If (Not Session(SWERRORI_AGGIORNAMENTO)) Then
        ' ''    ControlloCampiObbligatori()
        ' ''    LeggiCampi(myAnaMag, myListVenD, myListFornSec, myListAnaMagDes, myListDistBase)
        ' ''    If (Session(SWERRORI_AGGIORNAMENTO)) Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Controllo dati articoli", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_INFO)
        ' ''        Exit Sub
        ' ''    End If

        ' ''    If (ControlloCampiDati()) Then
        ' ''        Aggiornamento(myAnaMag, myListVenD, myListFornSec, myListAnaMagDes, myListDistBase)
        ' ''        ddlRicerca.Enabled = True
        ' ''        txtRicerca.Enabled = True
        ' ''        btnRicercaArticolo.Enabled = True
        ' ''    Else
        ' ''        Exit Sub
        ' ''    End If
        ' ''    Session(COD_ARTICOLO) = lblCodArticolo.Text.Trim
        ' ''    If Session(SWOP) = SWOPNUOVO Then
        ' ''        SqlDataSourceArticoli.DataBind()
        ' ''        GridViewArticoli.DataBind()
        ' ''        If GridViewArticoli.Rows.Count > 0 Then
        ' ''            GridViewArticoli_SelectedIndexChanged(Nothing, Nothing)
        ' ''        End If
        ' ''    End If
        ' ''    VisualizzaArticolo()
        ' ''    setPulsantiModalitaConsulta()
        ' ''    CaricaVariabili()
        ' ''Else
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Controllo dati articoli", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_INFO)
        ' ''End If
    End Sub

    Private Sub setPulsantiModalitaConsulta()
        'giu190412
        If String.IsNullOrEmpty(Session(COD_ARTICOLO)) Then
            If (GridViewArticoli.Rows.Count > 0) Then
                GridViewArticoli.SelectedIndex = 0
                Session(COD_ARTICOLO) = GridViewArticoli.Rows(0).Cells(1).Text
                VisualizzaArticolo()
            Else
                btnNuovo.Enabled = True
                btnModifica.Enabled = False
                btnElimina.Enabled = False
                btnStampa.Enabled = False
                btnStampaArtFor.Enabled = False
                btnAnnulla.Enabled = False
                btnAggiorna.Enabled = False
                btnAggiungiFornSec.Visible = False
            End If
        End If
        '---------
        If (GridViewArticoli.Rows.Count > 0) Then
            btnNuovo.Enabled = True
            btnModifica.Enabled = True
            btnElimina.Enabled = True
            btnStampa.Enabled = True
            btnStampaArtFor.Enabled = True
            btnAnnulla.Enabled = False
            btnAggiorna.Enabled = False
            btnAggiungiFornSec.Visible = False
        Else
            btnNuovo.Enabled = True
            btnModifica.Enabled = False
            btnElimina.Enabled = False
            btnStampa.Enabled = False
            btnStampaArtFor.Enabled = False
            btnAnnulla.Enabled = False
            btnAggiorna.Enabled = False
            btnAggiungiFornSec.Visible = False
        End If
    End Sub

    Private Sub setPulsantiModalitaAggiorna()
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnStampa.Enabled = False
        btnStampaArtFor.Enabled = False
        btnAnnulla.Enabled = True
        btnAggiorna.Enabled = True
        btnAggiungiFornSec.Visible = True
        GWDescEstesa.PopolaGridView()
        GWArticoliCTV.PopolaGridView()
        GWFornitoriSec.PopolaGridView()
        GWPrezziAcquisto.PopolaGridView()
        DistintaBase1.PopolaGridView()
    End Sub

    Private Function CheckNewCodiceArticolo() As Boolean
        Dim AnaMagSys As AnaMag
        Dim arrAnaMag As ArrayList
        lblCodArticolo.Text = txtCodBase.Text.Trim + txtCodOpzione.Text.Trim
        Dim Codice As String = lblCodArticolo.Text.Trim
        'giu200123
        If IsNothing(Codice) Then
            Codice = ""
        End If
        If String.IsNullOrEmpty(Codice) Then
            Codice = ""
        End If
        '-------------------
        txtCodBase.BackColor = SEGNALA_OK
        txtCodOpzione.BackColor = SEGNALA_OK
        'GIU290719
        txtCodBase.ToolTip = ""
        txtCodOpzione.ToolTip = ""
        '---------
        CheckNewCodiceArticolo = False
        If (Session(SWOP).Equals(SWOPMODIFICA)) Then
            Exit Function
        End If
        If Not String.IsNullOrEmpty(codice) Then
            AnaMagSys = New AnaMag

            Try
                arrAnaMag = AnaMagSys.getAnaMagByCodice(codice)
                If (arrAnaMag.Count > 0) Then
                    CheckNewCodiceArticolo = True
                    txtCodBase.BackColor = SEGNALA_KO
                    txtCodOpzione.BackColor = SEGNALA_KO
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ' ''ModalPopup.Show("Attenzione", "Codice articolo già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                    'GIU290719
                    txtCodBase.ToolTip = "Codice articolo già presente"
                    txtCodOpzione.ToolTip = "Codice articolo già presente"
                    '---------
                    Exit Function
                End If
            Catch
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
        End If
        Session(COD_ARTICOLO) = codice
    End Function

    Private Sub SelPrimoArticolo()
        GridViewArticoli.DataBind()
        Session(COD_ARTICOLO) = String.Empty
        If (GridViewArticoli.Rows.Count > 0) Then
            GridViewArticoli.SelectedIndex = 0
            Session(COD_ARTICOLO) = GridViewArticoli.Rows(0).Cells(1).Text
            VisualizzaArticolo()
        Else
            SvuotaCampi()
        End If
        setPulsantiModalitaConsulta()
    End Sub

    Private Sub ControllaValoreNumerico(ByRef txt As TextBox, ByVal nDecimali As Integer)
        Session(SWMODIFICATO) = True
        If txt.Text.Trim = "" Then txt.Text = "0" 'GIU070414
        txt.BackColor = IIf(IsNumeric(txt.Text), Def.SEGNALA_OK, Def.SEGNALA_KO)
        If (txt.BackColor = Def.SEGNALA_KO) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        Else
            txt.Text = FormattaNumero(txt.Text, nDecimali)
        End If
    End Sub

    Private Sub ApriSceltaListiniDaAgg()
        'giu140911 Giuseppe apro la finestra solo se ci sono  piu listini 1 è quello di BASE
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, "Select Codice From ListVenT", ds)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura tabella Listini da aggiornare.: " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Finally
            ObjDB = Nothing
        End Try
        If (ds.Tables.Count > 0) Then
            If (ds.Tables(0).Rows.Count = 1) Then
                Exit Sub
            End If
        Else
            Exit Sub
        End If

        WFPListiniDaAgg.WucElement = Me
        WFPListiniDaAgg.PopolaGridView()
        Session(F_SCELTALISTINI_APERTA) = True
        WFPListiniDaAgg.Show()
    End Sub

    Private Sub SetFiltroBaseSQLDataSourceArticoli()
        If (Not String.IsNullOrEmpty(_CodiceBase)) Then
            SqlDataSourceArticoli.FilterExpression = String.Format("Cod_Articolo LIKE '{0}%'", _CodiceBase)
        Else
            SqlDataSourceArticoli.FilterExpression = String.Empty
        End If
        If Not (GridViewArticoli Is Nothing) Then
            If GridViewArticoli.Rows.Count > 0 Then
                GridViewArticoli_SelectedIndexChanged(Nothing, Nothing)
                Try
                    GridViewArticoli.SelectedIndex = 0
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub

    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show(True)
            Case F_PAGAMENTI
                WFPElencoPagamenti.Show(True)
        End Select
    End Sub

    Private Sub ApriElencoFornitori()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoForn.Show(True)
    End Sub

#End Region

#Region "Metodi public"

    Public Sub ConfermaEliminaArticolo()
        Dim AMSys As New AnaMag
        Dim bEliminaOk As Boolean = False

        Try
            If (AMSys.CIAnaMagByCodice(Session(COD_ARTICOLO))) Then
                If (AMSys.delAnaMagByCodice(Session(COD_ARTICOLO))) Then

                    SelPrimoArticolo()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "L'articolo non può essere eliminato", WUC_ModalPopup.TYPE_ALERT)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "L'articolo non può essere eliminato", WUC_ModalPopup.TYPE_ALERT)
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore Elimina Articolo", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Return
        End Try
    End Sub

    Public Sub CallBackWFPFornitoriSec()
        Dim myFornitoreSec As FornSecondariEntity = Session(FORNITORE_SEC)
        GWFornitoriSec.AggiungiRigaFornSec(myFornitoreSec)
    End Sub

    Public Sub CallBackWFPListiniDaAgg()
        'GIU240123 NON VIENE PIU RICHIAMATA 'GIU240123 LASCIO COSI SENZA AGGIORNARE LISTINI DOLLARI DA VERIFICARE XKE VA IN ERRORE
        Try
            Dim listaCodiciListino As List(Of String) = Session(LISTINI_DA_AGG)
            If (listaCodiciListino.Count > 0) Then
                Dim myListVD As ListVenDEntity = Session(ELEMENTO_LISTVEND)
                For Each codice As String In listaCodiciListino
                    myListVD.Codice = codice
                    AggiornaListiniVD(myListVD)
                Next
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        Select Case (finestra)
            Case F_ALIQUOTAIVA
                txtCodAliquotaIva.Text = codice
                txtCodAliquotaIva.BackColor = Def.SEGNALA_OK
                ddlAliquotaIva.SelectedItem.Text = descrizione
            Case F_PAGAMENTI
                txtCodCondizioniPag.Text = codice
                txtCodCondizioniPag.BackColor = Def.SEGNALA_OK
                ddlCondizioniPag.SelectedItem.Text = descrizione
        End Select
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        txtCodFornitore.Text = codice
        txtCodFornitore.BackColor = Def.SEGNALA_OK
        ddlFornitore.SelectedItem.Text = descrizione
        PopolaTitolareRiferimento(txtCodFornitore.Text)
    End Sub

#End Region

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        WUC_SceltaStampaAnaArt1.Show()
    End Sub
    'Roberto 10/12/2011---
    Public Sub CallBackSceltaStampaAnaMag(ByVal TipoOrdine As String, ByVal Analitico As Boolean, ByVal TuttiArticoli As Boolean, ByVal strDaCodice As String, ByVal strACodice As String)
        'Roberto 07/12/2011---
        Dim dsAnaMag1 As New DSAnaMag
        Dim ObjReport As New Object
        Dim ClsPrint As New Listini
        Dim StrErrore As String = ""
        Dim strTmpTitoloRpt As String

        Dim SWSconti As Boolean = False


        Try
            If Analitico Then
                Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliAnalitica
                strTmpTitoloRpt = "Riepilogo analitico anagrafica magazzino"
            Else
                Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliSintetica
                strTmpTitoloRpt = "Riepilogo sintetico anagrafica magazzino"
            End If
            Dim strNomeAz As String = ""

            strNomeAz = Session(CSTAZIENDARPT).ToString.Trim


            If ClsPrint.StampaAnaMag(strNomeAz, strTmpTitoloRpt, dsAnaMag1, ObjReport, StrErrore, strDaCodice, strACodice, TipoOrdine) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsAnaMag1
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
            ModalPopup.Show("Errore in Articoli.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub


    Private Sub btnCategoriaArt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCategoriaArt.Click
        WFP_CategorieArt1.WucElement = Me
        WFP_CategorieArt1.SvuotaCampi()
        WFP_CategorieArt1.SetlblMessaggi("")
        Session(F_ANAGRCATEGORIEART_APERTA) = True
        WFP_CategorieArt1.Show()
    End Sub

    Public Sub CallBackWFPAnagrCategorieArt()
        Session(CODICECATART) = ""
        Dim rk As StrCategorieArt
        rk = Session(RKCATEGORIEART)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(CODICECATART) = rk.Codice
        txtCodCategoria.Text = rk.Codice
        SqlDataSourceCategoria.DataBind()
        ddlCatgoria.Items.Clear()
        ddlCatgoria.Items.Add("")
        ddlCatgoria.DataBind()
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        Session(SWMODIFICATO) = True
    End Sub
    Public Sub CancBackWFPAnagrCategorieArt()

    End Sub
    Private Sub btnLinea_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLinea.Click
        WFP_LineeArt1.WucElement = Me
        WFP_LineeArt1.SvuotaCampi()
        WFP_LineeArt1.SetlblMessaggi("")
        Session(F_ANAGRLINEEART_APERTA) = True
        WFP_LineeArt1.Show()
    End Sub
    Public Sub CallBackWFPAnagrLineeArt()
        Session(CODICELINEAART) = ""
        Dim rk As StrLineaArt
        rk = Session(RKLINEEART)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(CODICELINEAART) = rk.Codice
        txtCodLinea.Text = rk.Codice
        SqlDataSourceLinea.DataBind()
        ddlLinea.Items.Clear()
        ddlLinea.Items.Add("")
        ddlLinea.DataBind()
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
        Session(SWMODIFICATO) = True
    End Sub
    Public Sub CancBackWFPAnagrLineeArt()

    End Sub
    'giu140419
    Private Sub btnTipoCodBarre_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTipoCodBarre.Click
        WFP_TipoCodArt1.WucElement = Me
        WFP_TipoCodArt1.SvuotaCampi()
        WFP_TipoCodArt1.SetlblMessaggi("")
        Session(F_ANAGRTIPOCODART_APERTA) = True
        WFP_TipoCodArt1.Show()
    End Sub
    Public Sub CallBackWFPTipoCodArt()
        Session(CODICETIPOCODBARART) = ""
        Dim rk As StrTipoCodBarArt
        rk = Session(RKTIPOCODBARART)
        If IsNothing(rk.Tipo_Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(CODICETIPOCODBARART) = rk.Tipo_Codice
        lblTipoCodBarre.Text = rk.Descrizione.Trim
        SqlDataSourceLinea.DataBind()
        ddlTipoCodBarre.Items.Clear()
        ddlTipoCodBarre.Items.Add("")
        ddlTipoCodBarre.DataBind()
        PosizionaItemDDL(rk.Tipo_Codice, ddlTipoCodBarre)
        Session(SWMODIFICATO) = True
    End Sub
    Public Sub CancBackWFPTipoCodArt()

    End Sub
    '---------
    Private Sub btnMisure_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMisure.Click
        WFP_Misure1.WucElement = Me
        WFP_Misure1.SvuotaCampi()
        WFP_Misure1.SetlblMessaggi("")
        Session(F_ANAGRMISURE_APERTA) = True
        WFP_Misure1.Show()
    End Sub
    Public Sub CallBackWFPAnagrMisure()
        Session(CODICEMISURE) = ""
        Dim rk As StrMisure
        rk = Session(RKMISURE)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(CODICEMISURE) = rk.Codice
        txtCodUnitaMisura.Text = rk.Codice
        SqlDataSourceUM.DataBind()
        ddlUnitaMisura.Items.Clear()
        ddlUnitaMisura.Items.Add("")
        ddlUnitaMisura.DataBind()
        PosizionaItemDDLTxt(txtCodUnitaMisura, ddlUnitaMisura)
        Session(SWMODIFICATO) = True
    End Sub
    Public Sub CancBackWFPAnagrMisure()

    End Sub
    Private Sub btnDefBaseOpz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDefBaseOpz.Click
        If btnDefBaseOpz.Text.ToUpper = "M" Then
            btnDefBaseOpz.Text = "S"
            btnDefBaseOpz.ToolTip = "Salva"
            btnAnnDefBaseOpz.Visible = True
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
        Else
            ''''mettere controlli
            If TxtDefCodBase.Text = "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati mancanti", "La base è obbligatoria", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If Not IsNumeric(TxtDefCodBase.Text) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati errati", "E' necessario indicare un numero.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If CInt(TxtDefCodBase.Text) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati errati", "E' necessario indicare un numero maggiore di zero.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            Dim StrCodArticolo As String = lblCodArticolo.Text
            Dim LenArticolo As Integer = 0
            Dim TmpBase As Integer = 0
            Dim TmpOpz As Integer = 0
            LenArticolo = StrCodArticolo.Length
            TmpBase = TxtDefCodBase.Text
            TmpOpz = LenArticolo - TmpBase
            If TmpBase > LenArticolo Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati errati", "La base non può essere maggiore di " & LenArticolo & ".", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If TmpOpz > 5 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati errati", "L'opzione non può essere maggiore di 5.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If (TmpOpz + TmpBase) <> LenArticolo Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati errati", "La composizione del codice articolo è errata.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            btnDefBaseOpz.Text = "M"
            btnDefBaseOpz.ToolTip = "Modifica"
            btnAnnDefBaseOpz.Visible = False
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            LblDefBasOpz.Text = StrCodArticolo.Length - TxtDefCodBase.Text
            txtCodBase.Text = Mid(StrCodArticolo, 1, TxtDefCodBase.Text)
            txtCodOpzione.Text = Mid(StrCodArticolo, TxtDefCodBase.Text + 1)
            End If
            TxtDefCodBase.Enabled = Not TxtDefCodBase.Enabled

    End Sub

    Private Sub btnAnnDefBaseOpz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnDefBaseOpz.Click
        ''''mettere controllo
        btnDefBaseOpz.Text = "M"
        btnDefBaseOpz.ToolTip = "Modifica"
        TxtDefCodBase.Enabled = False
        btnAnnDefBaseOpz.Visible = False
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        TxtDefCodBase.Text = CInt(txtCodBase.Text.Length)
        LblDefBasOpz.Text = CInt(txtCodOpzione.Text.Length)
    End Sub

    Private Sub btnPagamenti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPagamenti.Click
        WFP_Pagamenti1.WucElement = Me
        WFP_Pagamenti1.SvuotaCampi()
        WFP_Pagamenti1.SetlblMessaggi("")
        Session(F_PAGAMENTI_APERTA) = True
        WFP_Pagamenti1.Show()
    End Sub
    Public Sub CallBackWFPPagamenti()
        Session(IDPAGAMENTO) = ""
        Dim rk As StrPagamenti
        rk = Session(RKPAGAMENTI)
        If IsNothing(rk.IDPagamento) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDPAGAMENTO) = rk.IDPagamento
        txtCodCondizioniPag.Text = rk.IDPagamento
        SqlDataSourceCondizioniPag.DataBind()
        ddlCondizioniPag.Items.Clear()
        ddlCondizioniPag.Items.Add("")
        ddlCondizioniPag.DataBind()
        PosizionaItemDDLTxt(txtCodCondizioniPag, ddlCondizioniPag)
        Session(SWMODIFICATO) = True
    End Sub
    Public Sub CancBackWFPPagamenti()

    End Sub

    Private Sub btnStampaArtFor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaArtFor.Click
        Session(CSTChiamatoDa) = "WF_AnagraficaArticoli.aspx?labelForm=Anagrafica articoli"
        Try
            Response.Redirect("WF_StampaArticoliFornitori.aspx?labelForm=Stampa articoli")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub chkOkListino_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkOkListino.CheckedChanged
        txtPrezzoVendita.Enabled = chkOkListino.Checked
        txtPrezzoMinVen.Enabled = chkOkListino.Checked
        txtSconto1Perc.Enabled = chkOkListino.Checked
    End Sub
End Class