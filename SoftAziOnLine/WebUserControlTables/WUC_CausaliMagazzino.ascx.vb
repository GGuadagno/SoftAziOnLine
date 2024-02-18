Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient

Partial Public Class WUC_CausaliMagazzino
    Inherits System.Web.UI.UserControl

    Private Enum CellIdxT
        CodArt = 1
        DesArt = 2
        Tipo = 3
        Giacenza = 4
        Fatturabile = 5
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDSTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCausMagC.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCausMagP.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCausMagM2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSMagazzino2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            Try
                ddlRicerca.Items.Add("Codice")
                ddlRicerca.Items(0).Value = "C" 'Cod_Articolo
                ddlRicerca.Items.Add("Descrizione")
                ddlRicerca.Items(1).Value = "D" 'Decrizione
                BuidDett()
                Session(SWOP) = SWOPNESSUNA
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Causali : " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnNuovo.Enabled = True
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        If txtCodice.Text.Trim = "" Then btnModifica.Enabled = False : btnElimina.Enabled = False
        'dettaglio
        SetDettaglio(False)
    End Sub
    Private Sub SetDettaglio(ByVal Valore As Boolean)
        ddlRicerca.Enabled = Not Valore
        GridViewPrevT.Enabled = Not Valore
        checkParoleContenute.Enabled = Not Valore
        txtRicerca.Enabled = Not Valore
        btnRicerca.Enabled = Not Valore
        '-
        btnAggiorna.Enabled = Valore
        btnAnnulla.Enabled = Valore

        txtCodice.Enabled = Valore : txtDescrizione.Enabled = Valore
        rbtnCliente.Enabled = Valore : rbtnFornitore.Enabled = Valore : rbtnNeutro.Enabled = Valore
        chkFatturabile.Enabled = Valore : ChkMovimento.Enabled = Valore
        chkCausCostoVenduto.Enabled = Valore : rbtnCausVend.Enabled = Valore : rbtnReso.Enabled = Valore : rbtnVendResoNo.Enabled = Valore
        rbtnCVisione.Enabled = Valore : rbtnCDeposito.Enabled = Valore : rbtnCNessuna.Enabled = Valore
        rbtnPrezzoAcquisto.Enabled = Valore : rbtnPrezzoListino.Enabled = Valore
        rbtnSegnoGiacenzaM.Enabled = Valore : rbtnSegnoGiacenzaN.Enabled = Valore : rbtnSegnoGiacenzaP.Enabled = Valore
        rbtnSegnoProdM.Enabled = Valore : rbtnSegnoProdN.Enabled = Valore : rbtnSegnoProdP.Enabled = Valore
        rbtnSegnoConfM.Enabled = Valore : rbtnSegnoConfN.Enabled = Valore : rbtnSegnoConfP.Enabled = Valore
        rbtnSegnoOrdM.Enabled = Valore : rbtnSegnoOrdN.Enabled = Valore : rbtnSegnoOrdP.Enabled = Valore
        rbtnSegnoVendM.Enabled = Valore : rbtnSegnoVendN.Enabled = Valore : rbtnSegnoVendP.Enabled = Valore
        rbtnSegnoDepM.Enabled = Valore : rbtnSegnoDepN.Enabled = Valore : rbtnSegnoDepP.Enabled = Valore
        'giu230614 giu250814
        chkDistintaBase.Enabled = Valore
        DDLCausaleIndottaC.Enabled = Valore : DDLCausaleIndottaP.Enabled = Valore
        If Valore = True Then
            DDLCausaleIndottaC.Enabled = chkDistintaBase.Checked
            DDLCausaleIndottaP.Enabled = chkDistintaBase.Checked
        End If
        chkMovMag.Enabled = Valore
        DDLCausaleMag2.Enabled = Valore
        DDLMagazzino2.Enabled = Valore
        If Valore = True Then
            DDLCausaleMag2.Enabled = chkMovMag.Checked
            DDLMagazzino2.Enabled = chkMovMag.Checked
        End If
    End Sub
    Private Sub InitDettaglio()
        txtCodice.Text = ""
        txtCodice.ToolTip = "" : txtCodice.BackColor = SEGNALA_OK
        txtDescrizione.Text = ""
        txtDescrizione.ToolTip = "" : txtDescrizione.BackColor = SEGNALA_OK
        rbtnCliente.Checked = False : rbtnFornitore.Checked = False : rbtnNeutro.Checked = True
        chkFatturabile.Checked = False : ChkMovimento.Checked = False
        chkCausCostoVenduto.Checked = False : rbtnCausVend.Checked = False : rbtnReso.Checked = False : rbtnVendResoNo.Checked = True
        rbtnCVisione.Checked = False : rbtnCDeposito.Checked = False : rbtnCNessuna.Checked = True
        rbtnPrezzoAcquisto.Checked = False : rbtnPrezzoListino.Checked = True
        rbtnSegnoGiacenzaM.Checked = False : rbtnSegnoGiacenzaN.Checked = True : rbtnSegnoGiacenzaP.Checked = False
        rbtnSegnoProdM.Checked = False : rbtnSegnoProdN.Checked = True : rbtnSegnoProdP.Checked = False
        rbtnSegnoConfM.Checked = False : rbtnSegnoConfN.Checked = True : rbtnSegnoConfP.Checked = False
        rbtnSegnoOrdM.Checked = False : rbtnSegnoOrdN.Checked = True : rbtnSegnoOrdP.Checked = False
        rbtnSegnoVendM.Checked = False : rbtnSegnoVendN.Checked = True : rbtnSegnoVendP.Checked = False
        rbtnSegnoDepM.Checked = False : rbtnSegnoDepN.Checked = True : rbtnSegnoDepP.Checked = False
        'GIU230614
        chkDistintaBase.Checked = False
        DDLCausaleIndottaC.SelectedIndex = -1 : DDLCausaleIndottaC.Enabled = False
        DDLCausaleIndottaP.SelectedIndex = -1 : DDLCausaleIndottaP.Enabled = False
        chkMovMag.Checked = False
        DDLCausaleMag2.SelectedIndex = -1 : DDLCausaleMag2.Enabled = False
        DDLMagazzino2.SelectedIndex = -1 : DDLMagazzino2.Enabled = False
    End Sub

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDCAUSMAG) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDCAUSMAG) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
    End Sub

    Private Sub BuidDett()
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
    End Sub
    '--

    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged

        SqlDSTElenco.FilterExpression = "" : txtRicerca.Text = ""
        SqlDSTElenco.DataBind()
        '---------
        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        If ddlRicerca.SelectedValue = "C" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        SqlDSTElenco.FilterExpression = ""
        If txtRicerca.Text.Trim = "" Then
            SqlDSTElenco.DataBind()
            '---------
            BtnSetEnabledTo(False)
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Try
                    Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                    txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
                    '-
                    LeggiDati(Session(IDCAUSMAG))

                Catch ex As Exception
                    InitDettaglio()
                    BtnSetEnabledTo(False)
                    Session(IDCAUSMAG) = ""
                End Try
            Else
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "C" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Codice >= " & txtRicerca.Text.Trim & ""
            Else
                SqlDSTElenco.FilterExpression = "Codice = " & txtRicerca.Text.Trim & ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSTElenco.FilterExpression = "Descrizione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        End If
        '
        SqlDSTElenco.DataBind()
        '---------
        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
                '-
                LeggiDati(Session(IDCAUSMAG))
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
    End Sub
#End Region

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If CKAbilitato(SWOPMODIFICA) = False Then Exit Sub

        Session(SWOP) = SWOPNUOVO
        InitDettaglio()
        SetDettaglio(True)
        btnNuovo.Enabled = False : btnModifica.Enabled = False : btnElimina.Enabled = False : btnStampaElenco.Enabled = False
        btnAggiorna.Enabled = True : btnAnnulla.Enabled = True
        txtCodice.Text = GetNewCodice()
        txtCodice.Focus()
    End Sub
    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKAbilitato(SWOPMODIFICA) = False Then Exit Sub

        Dim myID As String = Session(IDCAUSMAG)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun elemento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun elemento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        SetDettaglio(True)
        txtCodice.Enabled = False
        btnNuovo.Enabled = False : btnModifica.Enabled = False : btnElimina.Enabled = False : btnStampaElenco.Enabled = False
        btnAggiorna.Enabled = True : btnAnnulla.Enabled = True
        txtDescrizione.Focus()
    End Sub
    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim myID As String = Session(IDCAUSMAG)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun elemento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun elemento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWErrore As Boolean = False
        If txtCodice.Text.Trim = "" Then txtCodice.BackColor = SEGNALA_KO : If txtDescrizione.Text.Trim = "" Then txtDescrizione.BackColor = SEGNALA_KO
        If rbtnCliente.Checked = False And rbtnFornitore.Checked = False Then rbtnNeutro.Checked = True
        If rbtnCausVend.Checked = False And rbtnReso.Checked = False Then rbtnVendResoNo.Checked = True
        If rbtnCVisione.Checked = False And rbtnCDeposito.Checked = False Then rbtnCNessuna.Checked = True
        If rbtnPrezzoAcquisto.Checked = False Then rbtnPrezzoListino.Checked = True
        If rbtnSegnoGiacenzaM.Checked = False And rbtnSegnoGiacenzaN.Checked = False And rbtnSegnoGiacenzaP.Checked = False Then rbtnSegnoGiacenzaN.Checked = True
        If rbtnSegnoProdM.Checked = False And rbtnSegnoProdN.Checked = False And rbtnSegnoProdP.Checked = False Then rbtnSegnoProdN.Checked = True
        If rbtnSegnoConfM.Checked = False And rbtnSegnoConfN.Checked = False And rbtnSegnoConfP.Checked = False Then rbtnSegnoConfN.Checked = True
        If rbtnSegnoOrdM.Checked = False And rbtnSegnoOrdN.Checked = False And rbtnSegnoOrdP.Checked = False Then rbtnSegnoOrdN.Checked = True
        If rbtnSegnoVendM.Checked = False And rbtnSegnoVendN.Checked = False And rbtnSegnoVendP.Checked = False Then rbtnSegnoVendN.Checked = True
        If rbtnSegnoDepM.Checked = False And rbtnSegnoDepN.Checked = False And rbtnSegnoDepP.Checked = False Then rbtnSegnoDepN.Checked = True
        'giu250814
        If chkDistintaBase.Checked = False Then
            DDLCausaleIndottaC.SelectedIndex = -1
            DDLCausaleIndottaC.BackColor = SEGNALA_OK
            DDLCausaleIndottaP.SelectedIndex = -1
            DDLCausaleIndottaP.BackColor = SEGNALA_OK
        ElseIf DDLCausaleIndottaC.SelectedIndex < 1 Then
            DDLCausaleIndottaC.BackColor = SEGNALA_KO : SWErrore = True
        Else
            DDLCausaleIndottaC.BackColor = SEGNALA_OK
            DDLCausaleIndottaP.BackColor = SEGNALA_OK
        End If
        '
        If chkMovMag.Checked = False Then
            DDLCausaleMag2.SelectedIndex = -1
            DDLCausaleMag2.BackColor = SEGNALA_OK
            '-
            DDLMagazzino2.SelectedIndex = -1
            DDLMagazzino2.BackColor = SEGNALA_OK
        Else
            If DDLCausaleMag2.SelectedIndex < 1 Then
                DDLCausaleMag2.BackColor = SEGNALA_KO : SWErrore = True
            Else
                DDLCausaleMag2.BackColor = SEGNALA_OK
            End If
            '-
            If DDLMagazzino2.SelectedIndex < 1 Then
                DDLMagazzino2.BackColor = SEGNALA_KO : SWErrore = True
            Else
                DDLMagazzino2.BackColor = SEGNALA_OK
            End If
        End If
        'ERRORE se inserisco la stessa causale .. sempre diversa
        If DDLCausaleMag2.SelectedValue.Trim = txtCodice.Text.Trim Then
            DDLCausaleMag2.BackColor = SEGNALA_KO : SWErrore = True
        End If
        '---
        If txtCodice.BackColor = SEGNALA_KO Then SWErrore = True
        If txtDescrizione.BackColor = SEGNALA_KO Then SWErrore = True
        If SWErrore = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'OK AGGIORNO 
        Dim myErrore As String = ""
        If OKAggiorna(Session(IDCAUSMAG).ToString.Trim, myErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiornamento", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            'GIU250614 ''GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
        Session(SWOP) = SWOPNESSUNA
        'giu250814
        DDLCausaleIndottaC.Items.Clear()
        DDLCausaleIndottaC.Items.Add("")
        DDLCausaleIndottaC.Items(0).Value = 0
        SqlDSCausMagC.DataBind()
        DDLCausaleIndottaC.DataBind()
        '-
        DDLCausaleIndottaP.Items.Clear()
        DDLCausaleIndottaP.Items.Add("")
        DDLCausaleIndottaP.Items(0).Value = 0
        SqlDSCausMagP.DataBind()
        DDLCausaleIndottaP.DataBind()
        '-
        DDLCausaleMag2.Items.Clear()
        DDLCausaleMag2.Items.Add("")
        DDLCausaleMag2.Items(0).Value = 0
        SqlDSCausMagM2.DataBind()
        DDLCausaleMag2.DataBind()
        '-
        DDLMagazzino2.Items.Clear()
        DDLMagazzino2.Items.Add("")
        DDLMagazzino2.Items(0).Value = 0
        SqlDSMagazzino2.DataBind()
        DDLMagazzino2.DataBind()
    End Sub

    Private Function OKAggiorna(ByVal _Codice As Integer, ByRef _strErrore As String) As Boolean
        _strErrore = ""
        Dim SQLStr As String = ""
        If chkDistintaBase.Checked = False Then
            DDLCausaleIndottaC.SelectedIndex = -1
            DDLCausaleIndottaC.BackColor = SEGNALA_OK
            DDLCausaleIndottaP.SelectedIndex = -1
            DDLCausaleIndottaP.BackColor = SEGNALA_OK
        End If
        If chkMovMag.Checked = False Then
            DDLCausaleMag2.SelectedIndex = -1
            DDLCausaleMag2.BackColor = SEGNALA_OK
            '-
            DDLMagazzino2.SelectedIndex = -1
            DDLMagazzino2.BackColor = SEGNALA_OK
        End If
        If Session(SWOP) = SWOPMODIFICA Then
            SQLStr = "UPDATE CausMag SET Descrizione= '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            SQLStr += ",Tipo= '"
            If rbtnCliente.Checked = True Then
                SQLStr += "C'"
            ElseIf rbtnFornitore.Checked = True Then
                SQLStr += "F'"
            Else
                SQLStr += "'"
            End If
            SQLStr += ",Segno_Giacenza= '"
            If rbtnSegnoGiacenzaM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoGiacenzaP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",Fatturabile="
            If chkFatturabile.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            SQLStr += ",Password=0"
            '-
            SQLStr += ",Segno_Prodotto= '"
            If rbtnSegnoProdM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoProdP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",Segno_Confezionato= '"
            If rbtnSegnoConfM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoConfP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",Segno_Ordinato= '"
            If rbtnSegnoOrdM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoOrdP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",Segno_Venduto= '"
            If rbtnSegnoVendM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoVendP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-giu250814
            SQLStr += ",Componenti=" & IIf(chkDistintaBase.Checked = True, "1", "0")
            '-
            SQLStr += ",CausaleIndotta=" & IIf(DDLCausaleIndottaC.SelectedIndex > 0, DDLCausaleIndottaC.SelectedValue.Trim, 0)
            '-
            SQLStr += ",Cod_Utente=" & IIf(DDLCausaleIndottaP.SelectedIndex > 0, DDLCausaleIndottaP.SelectedValue.Trim, 0)
            '-
            SQLStr += ",Movimento_Magazzini="
            If chkMovMag.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",Movimento="
            If ChkMovimento.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",CodContoCoGe=''"
            SQLStr += ",Segno_Deposito= '"
            If rbtnSegnoDepM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoDepP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",CausVend="
            If rbtnCausVend.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",Segno_Lotti= '"
            If rbtnSegnoGiacenzaM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoGiacenzaP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",Segno_CL=''"
            SQLStr += ",CausCostoVenduto="
            If chkCausCostoVenduto.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",CVisione="
            If rbtnCVisione.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",CDeposito="
            If rbtnCDeposito.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",CausCVenditaDaCVisione=" & IIf(DDLMagazzino2.SelectedIndex > 0, DDLMagazzino2.SelectedValue.Trim, 0) 'giu150224
            SQLStr += ",CausCVenditaDaCDeposito=NULL"
            SQLStr += ",CausResoDaCVisione=NULL"
            SQLStr += ",CausResoDaCDeposito=NULL"
            SQLStr += ",PrezzoAL='"
            If rbtnPrezzoAcquisto.Checked = True Then
                SQLStr += "A'"
            ElseIf rbtnPrezzoListino.Checked = True Then
                SQLStr += "L'"
            Else
                SQLStr += "L'"
            End If
            '-
            SQLStr += ",Reso="
            If rbtnReso.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",CausMag2=" & IIf(DDLCausaleMag2.SelectedIndex > 0, DDLCausaleMag2.SelectedValue.Trim, 0)
            SQLStr += " WHERE(Codice = " & _Codice.ToString.Trim & ")"
        ElseIf Session(SWOP) = SWOPNUOVO Then
            SQLStr = "INSERT INTO CausMag ([Codice]" &
               ",[Descrizione]" &
               ",[Tipo]" &
               ",[Segno_Giacenza]" &
               ",[Fatturabile]" &
               ",[Password]" &
               ",[Segno_Prodotto]" &
               ",[Segno_Confezionato]" &
               ",[Segno_Ordinato]" &
               ",[Segno_Venduto]" &
               ",[Componenti]" &
               ",[CausaleIndotta]" &
               ",[Cod_Utente]" &
               ",[Movimento_Magazzini]" &
               ",[Movimento]" &
               ",[CodContoCoGE]" &
               ",[Segno_Deposito]" &
               ",[CausVend]" &
               ",[Segno_Lotti]" &
               ",[Segno_CL]" &
               ",[CausCostoVenduto]" &
               ",[CVisione]" &
               ",[CDeposito]" &
               ",[CausCVenditaDaCVisione]" &
               ",[CausCVenditaDaCDeposito]" &
               ",[CausResoDaCVisione]" &
               ",[CausResoDaCDeposito]" &
               ",[PrezzoAL]" &
               ",[Reso]" &
               ",[CausMag2]) VALUES("
            SQLStr += Val(txtCodice.Text.Trim).ToString.Trim
            SQLStr += ",'" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            SQLStr += ",'"
            If rbtnCliente.Checked = True Then
                SQLStr += "C'"
            ElseIf rbtnFornitore.Checked = True Then
                SQLStr += "F'"
            Else
                SQLStr += "'"
            End If
            SQLStr += ",'"
            If rbtnSegnoGiacenzaM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoGiacenzaP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ","
            If chkFatturabile.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            SQLStr += ",0"
            '-
            SQLStr += ",'"
            If rbtnSegnoProdM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoProdP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",'"
            If rbtnSegnoConfM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoConfP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",'"
            If rbtnSegnoOrdM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoOrdP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",'"
            If rbtnSegnoVendM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoVendP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            'giu250814
            SQLStr += "," & IIf(chkDistintaBase.Checked = True, "1", "0")
            '-
            SQLStr += "," & IIf(DDLCausaleIndottaC.SelectedIndex > 0, DDLCausaleIndottaC.SelectedValue.Trim, 0)
            '-
            SQLStr += "," & IIf(DDLCausaleIndottaP.SelectedIndex > 0, DDLCausaleIndottaP.SelectedValue.Trim, 0)
            '-
            SQLStr += ","
            If chkMovMag.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ","
            If ChkMovimento.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",''"
            SQLStr += ",'"
            If rbtnSegnoDepM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoDepP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ","
            If rbtnCausVend.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ",'"
            If rbtnSegnoGiacenzaM.Checked = True Then
                SQLStr += "-'"
            ElseIf rbtnSegnoGiacenzaP.Checked = True Then
                SQLStr += "+'"
            Else
                SQLStr += "'"
            End If
            '-
            SQLStr += ",''"
            SQLStr += ","
            If chkCausCostoVenduto.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ","
            If rbtnCVisione.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += ","
            If rbtnCDeposito.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += "," & IIf(DDLMagazzino2.SelectedIndex > 0, DDLMagazzino2.SelectedValue.Trim, 0) 'giu150224
            SQLStr += ",NULL"
            SQLStr += ",NULL"
            SQLStr += ",NULL"
            SQLStr += ",'"
            If rbtnPrezzoAcquisto.Checked = True Then
                SQLStr += "A'"
            ElseIf rbtnPrezzoListino.Checked = True Then
                SQLStr += "L'"
            Else
                SQLStr += "L'"
            End If
            '-
            SQLStr += ","
            If rbtnReso.Checked = True Then
                SQLStr += "1"
            Else
                SQLStr += "0"
            End If
            '-
            SQLStr += "," & IIf(DDLCausaleMag2.SelectedIndex > 0, DDLCausaleMag2.SelectedValue.Trim, 0)
            SQLStr += ")"
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiornamento", "Operazione di aggiornamento non definita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If

        Dim ObjDB As New DataBaseUtility
        Try
            OKAggiorna = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            _strErrore = ex.Message.Trim
            OKAggiorna = False
            ObjDB = Nothing
        End Try
        ObjDB = Nothing
    End Function

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Session(SWOP) = SWOPNESSUNA
        SetDettaglio(False)
        btnNuovo.Enabled = True : btnModifica.Enabled = True : btnElimina.Enabled = True : btnStampaElenco.Enabled = True
        btnAggiorna.Enabled = False : btnAnnulla.Enabled = False
        Dim myID As String = Session(IDCAUSMAG)
        If IsNothing(myID) Then
            InitDettaglio()
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            InitDettaglio()
            Exit Sub
        End If
        LeggiDati(Session(IDCAUSMAG))
    End Sub

    Private Function CKAbilitato(ByVal _Op As String) As Boolean
        ' ''Dim sTipoUtente As String = ""
        ' ''If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
        ' ''    Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        ' ''    If (Utente Is Nothing) Then
        ' ''        Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
        ' ''        Exit Function
        ' ''    End If
        ' ''    sTipoUtente = Utente.Tipo
        ' ''Else
        ' ''    sTipoUtente = Session(CSTTIPOUTENTE)
        ' ''End If
        '' ''-----------
        ' ''If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Function
        ' ''End If
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Function
        ' ''End If
        ' ''If (sTipoUtente.Equals(CSTACQUISTI)) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Function
        ' ''End If
        '----------------------------
        CKAbilitato = True
    End Function

    Private Sub btnStampaElenco_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaElenco.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
    End Sub

    Private Sub Chiudi(ByVal strErrore As String)
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
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Try
            Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
            txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
            '-
            LeggiDati(Session(IDCAUSMAG))
            BtnSetEnabledTo(True)
        Catch ex As Exception
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End Try
    End Sub

    Private Sub LeggiDati(ByVal _Codice As Integer)
        Try
            Dim dvCausMag As DataView
            dvCausMag = SqlDSCausMagC.Select(DataSourceSelectArguments.Empty)
            If (dvCausMag Is Nothing) Then
                Exit Sub
            End If
            If dvCausMag.Count > 0 Then
                dvCausMag.RowFilter = "Codice = " & _Codice & ""
                If dvCausMag.Count > 0 Then
                    txtCodice.Text = dvCausMag.Item(0).Item("Codice").ToString.Trim
                    txtCodice.ToolTip = "" : txtCodice.BackColor = SEGNALA_OK
                    txtDescrizione.Text = dvCausMag.Item(0).Item("Descrizione").ToString.Trim
                    txtDescrizione.ToolTip = "" : txtDescrizione.BackColor = SEGNALA_OK
                    Dim myStr As String = dvCausMag.Item(0).Item("Tipo").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "C" Then
                        rbtnCliente.Checked = True : rbtnFornitore.Checked = False : rbtnNeutro.Checked = False
                    ElseIf myStr.Trim = "F" Then
                        rbtnCliente.Checked = False : rbtnFornitore.Checked = True : rbtnNeutro.Checked = False
                    Else
                        rbtnCliente.Checked = False : rbtnFornitore.Checked = False : rbtnNeutro.Checked = True
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("Fatturabile")) Then
                        chkFatturabile.Checked = False
                    ElseIf dvCausMag.Item(0).Item("Fatturabile") <> 0 Then
                        chkFatturabile.Checked = True
                    Else
                        chkFatturabile.Checked = False
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("Movimento_Magazzini")) Then
                        chkMovMag.Checked = False
                    ElseIf dvCausMag.Item(0).Item("Movimento_Magazzini") <> 0 Then
                        chkMovMag.Checked = True
                    Else
                        chkMovMag.Checked = False
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("Movimento")) Then
                        ChkMovimento.Checked = False
                    ElseIf dvCausMag.Item(0).Item("Movimento") <> 0 Then
                        ChkMovimento.Checked = True
                    Else
                        ChkMovimento.Checked = False
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("CausCostoVenduto")) Then
                        chkCausCostoVenduto.Checked = False
                    ElseIf dvCausMag.Item(0).Item("CausCostoVenduto") <> 0 Then
                        chkCausCostoVenduto.Checked = True
                    Else
                        chkCausCostoVenduto.Checked = False
                    End If
                    rbtnVendResoNo.Checked = False
                    If IsDBNull(dvCausMag.Item(0).Item("CausVend")) Then
                        rbtnCausVend.Checked = False
                    ElseIf dvCausMag.Item(0).Item("CausVend") <> 0 Then
                        rbtnCausVend.Checked = True
                    Else
                        rbtnCausVend.Checked = False
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("Reso")) Then
                        rbtnReso.Checked = False
                    ElseIf dvCausMag.Item(0).Item("Reso") <> 0 Then
                        rbtnReso.Checked = True
                    Else
                        rbtnReso.Checked = False
                    End If
                    If rbtnCausVend.Checked = False And rbtnReso.Checked = False Then rbtnVendResoNo.Checked = True
                    '-
                    rbtnCNessuna.Checked = False
                    If IsDBNull(dvCausMag.Item(0).Item("CVisione")) Then
                        rbtnCVisione.Checked = False
                    ElseIf dvCausMag.Item(0).Item("CVisione") <> 0 Then
                        rbtnCVisione.Checked = True
                    Else
                        rbtnCVisione.Checked = False
                    End If
                    If IsDBNull(dvCausMag.Item(0).Item("CDeposito")) Then
                        rbtnCDeposito.Checked = False
                    ElseIf dvCausMag.Item(0).Item("CDeposito") <> 0 Then
                        rbtnCDeposito.Checked = True
                    Else
                        rbtnCDeposito.Checked = False
                    End If
                    If rbtnCVisione.Checked = False And rbtnCDeposito.Checked = False Then rbtnCNessuna.Checked = True
                    '-
                    myStr = dvCausMag.Item(0).Item("PrezzoAL").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "L" Then
                        rbtnPrezzoListino.Checked = True : rbtnPrezzoAcquisto.Checked = False
                    ElseIf myStr.Trim = "A" Then
                        rbtnPrezzoListino.Checked = False : rbtnPrezzoAcquisto.Checked = True
                    Else
                        rbtnPrezzoListino.Checked = True : rbtnPrezzoAcquisto.Checked = False
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Giacenza").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoGiacenzaP.Checked = True : rbtnSegnoGiacenzaM.Checked = False : rbtnSegnoGiacenzaN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoGiacenzaP.Checked = False : rbtnSegnoGiacenzaM.Checked = True : rbtnSegnoGiacenzaN.Checked = False
                    Else
                        rbtnSegnoGiacenzaP.Checked = False : rbtnSegnoGiacenzaM.Checked = False : rbtnSegnoGiacenzaN.Checked = True
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Prodotto").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoProdP.Checked = True : rbtnSegnoProdM.Checked = False : rbtnSegnoProdN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoProdP.Checked = False : rbtnSegnoProdM.Checked = True : rbtnSegnoProdN.Checked = False
                    Else
                        rbtnSegnoProdP.Checked = False : rbtnSegnoProdM.Checked = False : rbtnSegnoProdN.Checked = True
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Confezionato").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoConfP.Checked = True : rbtnSegnoConfM.Checked = False : rbtnSegnoConfN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoConfP.Checked = False : rbtnSegnoConfM.Checked = True : rbtnSegnoConfN.Checked = False
                    Else
                        rbtnSegnoConfP.Checked = False : rbtnSegnoConfM.Checked = False : rbtnSegnoConfN.Checked = True
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Ordinato").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoOrdP.Checked = True : rbtnSegnoOrdM.Checked = False : rbtnSegnoOrdN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoOrdP.Checked = False : rbtnSegnoOrdM.Checked = True : rbtnSegnoOrdN.Checked = False
                    Else
                        rbtnSegnoOrdP.Checked = False : rbtnSegnoOrdM.Checked = False : rbtnSegnoOrdN.Checked = True
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Venduto").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoVendP.Checked = True : rbtnSegnoVendM.Checked = False : rbtnSegnoVendN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoVendP.Checked = False : rbtnSegnoVendM.Checked = True : rbtnSegnoVendN.Checked = False
                    Else
                        rbtnSegnoVendP.Checked = False : rbtnSegnoVendM.Checked = False : rbtnSegnoVendN.Checked = True
                    End If
                    '-
                    myStr = dvCausMag.Item(0).Item("Segno_Deposito").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "+" Then
                        rbtnSegnoDepP.Checked = True : rbtnSegnoDepM.Checked = False : rbtnSegnoDepN.Checked = False
                    ElseIf myStr.Trim = "-" Then
                        rbtnSegnoDepP.Checked = False : rbtnSegnoDepM.Checked = True : rbtnSegnoDepN.Checked = False
                    Else
                        rbtnSegnoDepP.Checked = False : rbtnSegnoDepM.Checked = False : rbtnSegnoDepN.Checked = True
                    End If
                    'giu250814
                    If IsDBNull(dvCausMag.Item(0).Item("Componenti")) Then
                        chkDistintaBase.Checked = False
                    Else
                        chkDistintaBase.Checked = CBool(dvCausMag.Item(0).Item("Componenti"))
                    End If
                    '-
                    PosizionaItemDDL(dvCausMag.Item(0).Item("CausaleIndotta").ToString.Trim, DDLCausaleIndottaC)
                    PosizionaItemDDL(dvCausMag.Item(0).Item("Cod_Utente").ToString.Trim, DDLCausaleIndottaP)
                    '---------
                    PosizionaItemDDL(dvCausMag.Item(0).Item("CausMag2").ToString.Trim, DDLCausaleMag2)
                    '-
                    PosizionaItemDDL(dvCausMag.Item(0).Item("CausCVenditaDaCVisione").ToString.Trim, DDLMagazzino2)
                End If
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura dati Causale", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub

    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(Codice) AS Numero From CausMag"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewCodice = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewCodice = 1
                    End If
                    Exit Function
                Else
                    GetNewCodice = 1
                    Exit Function
                End If
            Else
                GetNewCodice = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCodice = -1
            '_strErrore = "Errore Verifica Codice da impegnare: " & Ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Lettura CausMag", "Verifica Codice da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Private Sub txtCodice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodice.TextChanged
        txtCodice.BackColor = SEGNALA_OK : txtCodice.ToolTip = ""
        If txtCodice.Text.Trim = "" Then txtCodice.BackColor = SEGNALA_KO : txtCodice.ToolTip = "Codice obbligatorio."
        If Not IsNumeric(txtCodice.Text.Trim) Then txtCodice.BackColor = SEGNALA_KO : txtCodice.ToolTip = "Codice non numerico."
        If Val(txtCodice.Text.Trim) < 1 Then txtCodice.BackColor = SEGNALA_KO : txtCodice.ToolTip = "Codice uguale a zero."
        If Val(txtCodice.Text.Trim) < 1 Then Exit Sub
        If CheckNewCod(Val(txtCodice.Text.Trim)) = False Then txtCodice.BackColor = SEGNALA_KO
    End Sub
    Private Function CheckNewCod(ByVal _Codice As Integer) As Boolean
        CheckNewCod = True
        Try
            Dim dvCausMag As DataView
            dvCausMag = SqlDSCausMagC.Select(DataSourceSelectArguments.Empty)
            If (dvCausMag Is Nothing) Then
                Exit Function
            End If
            If dvCausMag.Count > 0 Then
                dvCausMag.RowFilter = "Codice = " & _Codice & ""
                If dvCausMag.Count > 0 Then
                    CheckNewCod = False
                    txtCodice.ToolTip = "Codice già presente in tabella."
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Verifica nuovo codice", "Attenzione, codice già presente in tabella.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            End If
        Catch ex As Exception
            CheckNewCod = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura dati Causale (CheckNewCod)", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        txtDescrizione.BackColor = SEGNALA_OK : txtDescrizione.ToolTip = ""
        If txtDescrizione.Text.Trim = "" Then txtDescrizione.BackColor = SEGNALA_KO : txtDescrizione.ToolTip = "Descrizione obbligatoria."
        If txtDescrizione.Text.Trim = "" Then Exit Sub
        If CheckNewDes() = False Then txtDescrizione.BackColor = SEGNALA_KO
    End Sub
    Private Function CheckNewDes() As Boolean
        CheckNewDes = True
        Try
            Dim dvCausMag As DataView
            dvCausMag = SqlDSCausMagC.Select(DataSourceSelectArguments.Empty)
            If (dvCausMag Is Nothing) Then
                Exit Function
            End If
            If dvCausMag.Count > 0 Then
                dvCausMag.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
                If dvCausMag.Count > 0 Then
                    Dim myCod As Integer = dvCausMag.Item(0).Item("Codice")
                    If myCod = Val(txtCodice.Text.Trim) Then
                        Exit Function
                    End If
                    CheckNewDes = False
                    txtDescrizione.ToolTip = "Descrizione già presente in tabella."
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Verifica descrizione", "Attenzione, descrizione già presente in tabella.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            End If
        Catch ex As Exception
            CheckNewDes = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura dati Causale (CheckNewDes)", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CheckDoc() = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina Causale: " & txtDescrizione.Text.Trim, "Attenzione, impossibile eliminare la causale. <br> Utilizzata in uno o piu documenti", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKCancella"
        ModalPopup.Show("Elimina Causale: " & txtDescrizione.Text.Trim, "Confermi l'eliminazione della Causale ? <br> " & txtDescrizione.Text.Trim, WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub OKCancella()

        Dim _strErrore As String = "" : Dim SWOK As Boolean = True
        Dim SQLStr As String = "DELETE FROM CausMag WHERE Codice=" & Val(txtCodice.Text.Trim)
        Dim ObjDB As New DataBaseUtility
        Try
            SWOK = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            _strErrore = ex.Message.Trim
            SWOK = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina Causale", "Errore nella cancellazione causale. <br> " & _strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        GridViewPrevT.SelectedIndex = -1
        Session(IDCAUSMAG) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDCAUSMAG) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDCAUSMAG) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDCAUSMAG) = ""
        End If
        'giu250814
        DDLCausaleIndottaC.Items.Clear()
        DDLCausaleIndottaC.Items.Add("")
        DDLCausaleIndottaC.Items(0).Value = 0
        SqlDSCausMagC.DataBind()
        DDLCausaleIndottaC.DataBind()
        DDLCausaleIndottaP.Items.Clear()
        DDLCausaleIndottaP.Items.Add("")
        DDLCausaleIndottaP.Items(0).Value = 0
        SqlDSCausMagP.DataBind()
        DDLCausaleIndottaP.DataBind()
    End Sub

    Private Function CheckDoc() As Boolean
        CheckDoc = False
        Dim strSQL As String = ""
        strSQL = "Select TOP 1 IDDocumenti From DocumentiT WHERE Cod_Causale=" & Val(txtCodice.Text.Trim)
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("IDDocumenti")) Then
                        CheckDoc = True
                    End If
                    Exit Function
                End If
            End If
        Catch Ex As Exception
            CheckDoc = True
            '_strErrore = "Errore Verifica Codice da impegnare: " & Ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Lettura Documenti", "Verifica Codice da eliminare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Private Sub chkDistintaBase_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDistintaBase.CheckedChanged
        If chkDistintaBase.Checked = True Then
            DDLCausaleIndottaC.Enabled = True : DDLCausaleIndottaP.Enabled = True
        Else
            DDLCausaleIndottaC.SelectedIndex = -1
            DDLCausaleIndottaC.Enabled = False
            DDLCausaleIndottaC.BackColor = SEGNALA_OK
            '-
            DDLCausaleIndottaP.SelectedIndex = -1
            DDLCausaleIndottaP.Enabled = False
            DDLCausaleIndottaP.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub chkMovMag_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkMovMag.CheckedChanged
        If chkMovMag.Checked = True Then
            DDLCausaleMag2.Enabled = True
            DDLMagazzino2.Enabled = True
        Else
            DDLCausaleMag2.SelectedIndex = -1
            DDLCausaleMag2.Enabled = False
            DDLCausaleMag2.BackColor = SEGNALA_OK
            '-
            DDLMagazzino2.SelectedIndex = -1
            DDLMagazzino2.Enabled = False
            DDLMagazzino2.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub DDLCausaleMag2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausaleMag2.SelectedIndexChanged
        If DDLCausaleMag2.SelectedValue.Trim = txtCodice.Text.Trim Then
            DDLCausaleMag2.BackColor = SEGNALA_KO
        End If
    End Sub
End Class