Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_TipoContratto
    Inherits System.Web.UI.UserControl

    Private Enum CellIdxT
        CodArt = 1
        DesArt = 2
        TipoPag = 3
        TipoSc = 4
        FineMese = 5
        Anticipato = 6
        DurataNum = 7
        NVisite = 8
        MeseCS = 9
        GiornoFisso = 10
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDSTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
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
        DDLTipoPagamento.Enabled = Valore : DDLTipoScadenza.Enabled = Valore
        CheckFineMese.Enabled = Valore : CheckAnticipato.Enabled = Valore
        DDLMeseCS.Enabled = Valore : txtGiornoFisso.Enabled = Valore
        txtCodCausale.Enabled = Valore : DDLCausali.Enabled = Valore
        txtDurataNum.Enabled = Valore
        txtNVisite.Enabled = Valore : txtCodVisita.Enabled = Valore
    End Sub
    Private Sub InitDettaglio()
        txtCodice.Text = ""
        txtCodice.ToolTip = "" : txtCodice.BackColor = SEGNALA_OK
        txtDescrizione.Text = ""
        txtDescrizione.ToolTip = "" : txtDescrizione.BackColor = SEGNALA_OK
        DDLTipoPagamento.SelectedIndex = -1 : DDLTipoPagamento.BackColor = SEGNALA_OK
        CheckFineMese.Checked = False : CheckAnticipato.Checked = False
        DDLTipoScadenza.SelectedIndex = -1 : DDLTipoScadenza.BackColor = SEGNALA_OK
        DDLMeseCS.SelectedIndex = -1 : DDLMeseCS.BackColor = SEGNALA_OK
        txtGiornoFisso.Text = "0" : txtGiornoFisso.BackColor = SEGNALA_OK
        txtCodCausale.AutoPostBack = False : DDLCausali.AutoPostBack = False
        txtCodCausale.Text = ""
        DDLCausali.SelectedIndex = -1 : DDLCausali.BackColor = SEGNALA_OK
        txtCodCausale.AutoPostBack = True : DDLCausali.AutoPostBack = True
        txtDurataNum.Text = "0" : txtDurataNum.BackColor = SEGNALA_OK
        txtNVisite.Text = "0" : txtNVisite.BackColor = SEGNALA_OK
        txtCodVisita.Text = "" : txtCodVisita.BackColor = SEGNALA_OK : lblDesVisita.Text = ""
    End Sub

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDTIPOCONTRATTO) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDTIPOCONTRATTO) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If e.Row.Cells(CellIdxT.TipoPag).Text.Trim = "1" Then
                e.Row.Cells(CellIdxT.TipoPag).Text = "Anticipato"
            ElseIf e.Row.Cells(CellIdxT.TipoPag).Text.Trim = "2" Then
                e.Row.Cells(CellIdxT.TipoPag).Text = "Posticipato"
            ElseIf e.Row.Cells(CellIdxT.TipoPag).Text.Trim = "3" Then
                e.Row.Cells(CellIdxT.TipoPag).Text = "Già fatturato all'ordine"
            ElseIf e.Row.Cells(CellIdxT.TipoPag).Text.Trim = "4" Then
                e.Row.Cells(CellIdxT.TipoPag).Text = "Alla scadenza evasione attività"
            ElseIf e.Row.Cells(CellIdxT.TipoPag).Text.Trim = "0" Then
                e.Row.Cells(CellIdxT.TipoPag).Text = ""
            End If
            '-
            If e.Row.Cells(CellIdxT.TipoSc).Text.Trim = "1" Then
                e.Row.Cells(CellIdxT.TipoSc).Text = "Data Contratto"
            ElseIf e.Row.Cells(CellIdxT.TipoSc).Text.Trim = "2" Then
                e.Row.Cells(CellIdxT.TipoSc).Text = "Data Accettazione"
            ElseIf e.Row.Cells(CellIdxT.TipoSc).Text.Trim = "3" Then
                e.Row.Cells(CellIdxT.TipoSc).Text = "Data Inizio"
            ElseIf e.Row.Cells(CellIdxT.TipoSc).Text.Trim = "0" Then
                e.Row.Cells(CellIdxT.TipoSc).Text = ""
            End If
            '-
            If e.Row.Cells(CellIdxT.FineMese).Text.Trim = "True" Then
                e.Row.Cells(CellIdxT.FineMese).Text = "Si " '& e.Row.Cells(CellIdxT.FineMese).Text.Trim
            ElseIf e.Row.Cells(CellIdxT.FineMese).Text.Trim = "False" Then
                e.Row.Cells(CellIdxT.FineMese).Text = "No " '& e.Row.Cells(CellIdxT.FineMese).Text.Trim
            End If
            '-
            If e.Row.Cells(CellIdxT.Anticipato).Text.Trim = "True" Then
                e.Row.Cells(CellIdxT.Anticipato).Text = "Si " '& e.Row.Cells(CellIdxT.Anticipato).Text.Trim
            ElseIf e.Row.Cells(CellIdxT.Anticipato).Text.Trim = "False" Then
                e.Row.Cells(CellIdxT.Anticipato).Text = "No " '& e.Row.Cells(CellIdxT.Anticipato).Text.Trim
            End If
            '-
            If e.Row.Cells(CellIdxT.MeseCS).Text.Trim = "1" Then
                e.Row.Cells(CellIdxT.MeseCS).Text = "Mese in corso (Data tipo Scadenza)"
            ElseIf e.Row.Cells(CellIdxT.MeseCS).Text.Trim = "2" Then
                e.Row.Cells(CellIdxT.MeseCS).Text = "Mese successivo (Data tipo Scadenza)"
            ElseIf e.Row.Cells(CellIdxT.MeseCS).Text.Trim = "0" Then
                e.Row.Cells(CellIdxT.MeseCS).Text = ""
            End If
            '-
            If e.Row.Cells(CellIdxT.GiornoFisso).Text.Trim = "0" Then
                e.Row.Cells(CellIdxT.GiornoFisso).Text = ""
            End If
            If e.Row.Cells(CellIdxT.NVisite).Text.Trim = "0" Then
                e.Row.Cells(CellIdxT.NVisite).Text = ""
            End If
        End If
    End Sub
    Private Sub BuidDett()
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
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
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
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
                    Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                    txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
                    '-
                    LeggiDati(Session(IDTIPOCONTRATTO))

                Catch ex As Exception
                    InitDettaglio()
                    BtnSetEnabledTo(False)
                    Session(IDTIPOCONTRATTO) = ""
                End Try
            Else
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
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
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
                '-
                LeggiDati(Session(IDTIPOCONTRATTO))
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
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

        Dim myID As String = Session(IDTIPOCONTRATTO)
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
        Dim myID As String = Session(IDTIPOCONTRATTO)
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
        If DDLTipoPagamento.SelectedValue.Trim = "" Then DDLTipoPagamento.BackColor = SEGNALA_KO : SWErrore = True
        If DDLTipoScadenza.SelectedValue.Trim = "" Then
            DDLTipoScadenza.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Val(txtGiornoFisso.Text.Trim) > 0 Then
            If DDLMeseCS.SelectedValue.Trim = "" Then
                DDLMeseCS.BackColor = SEGNALA_KO
                SWErrore = True
            End If
        Else
            DDLMeseCS.SelectedIndex = -1
            txtGiornoFisso.Text = ""
        End If
        If txtNVisite.Text.Trim = "" Then txtNVisite.Text = "0"
        If Not IsNumeric(txtNVisite.Text.Trim) Then
            txtNVisite.BackColor = SEGNALA_KO : SWErrore = True
        End If
        If txtCodVisita.Text <> "" Then
            lblDesVisita.Text = App.GetValoreFromChiave(txtCodVisita.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
            If lblDesVisita.Text.Trim = "" Then
                txtCodVisita.BackColor = SEGNALA_KO : SWErrore = True
            End If
        ElseIf IsNumeric(txtNVisite.Text.Trim) Then
            If Val(txtNVisite.Text.Trim) = 0 Then
                txtNVisite.BackColor = SEGNALA_KO
            End If
        Else
            txtNVisite.BackColor = SEGNALA_KO
        End If
        '-
        If txtDurataNum.Text.Trim = "" Then txtDurataNum.Text = "0"
        If Not IsNumeric(txtDurataNum.Text.Trim) Then
            txtDurataNum.BackColor = SEGNALA_KO : SWErrore = True
        ElseIf Val(txtDurataNum.Text.Trim) < 1 Then
            txtDurataNum.BackColor = SEGNALA_KO : SWErrore = True
        End If
        If DDLCausali.SelectedValue.Trim = "" Then DDLCausali.BackColor = SEGNALA_KO : SWErrore = True
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
        If OKAggiorna(Session(IDTIPOCONTRATTO).ToString.Trim, myErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiornamento", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            Try
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
        End If
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Private Function OKAggiorna(ByVal _Codice As Integer, ByRef _strErrore As String) As Boolean
        _strErrore = ""
        Dim SQLStr As String = ""
        If Session(SWOP) = SWOPMODIFICA Then
            SQLStr = "UPDATE TipoContratto SET Descrizione= '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            SQLStr += ",TipoPagamento=" & IIf(DDLTipoPagamento.SelectedValue.Trim = "", "NULL", DDLTipoPagamento.SelectedValue.Trim) & ""
            SQLStr += ",TipoScadenza=" & IIf(DDLTipoScadenza.SelectedValue.Trim = "", "NULL", DDLTipoScadenza.SelectedValue.Trim) & ""
            SQLStr += ",FineMese=" & IIf(CheckFineMese.Checked = True, "1", "0") & ""
            SQLStr += ",Anticipato=" & IIf(CheckAnticipato.Checked = True, "1", "0") & ""
            SQLStr += ",MeseCS=" & IIf(DDLMeseCS.SelectedValue.Trim = "", "NULL", DDLMeseCS.SelectedValue.Trim) & ""
            SQLStr += ",GiornoFisso=" & Val(IIf(txtGiornoFisso.Text.Trim = "", 0, txtGiornoFisso.Text.Trim)).ToString.Trim & ""
            SQLStr += ",Cod_Causale=" & IIf(DDLCausali.SelectedValue.Trim = "", "NULL", DDLCausali.SelectedValue.Trim) & ""
            SQLStr += ",NVisite=" & Val(IIf(txtNVisite.Text.Trim = "", 0, txtNVisite.Text.Trim)).ToString.Trim & ""
            SQLStr += ",DurataNum=" & Val(IIf(txtDurataNum.Text.Trim = "", 0, txtDurataNum.Text.Trim)).ToString.Trim & ""
            SQLStr += ",CodVisita='" & Controlla_Apice(txtCodVisita.Text.Trim) & "'"
            '-
            SQLStr += " WHERE(Codice = " & _Codice.ToString.Trim & ")"
        ElseIf Session(SWOP) = SWOPNUOVO Then
            SQLStr = "INSERT INTO TipoContratto ([Codice]" & _
               ",[Descrizione]" & _
               ",[TipoPagamento]" & _
               ",[TipoScadenza]" & _
               ",[FIneMese]" & _
               ",[Anticipato]" & _
               ",[MeseCS]" & _
               ",[GiornoFisso],[Cod_Causale],[NVisite],DurataNum,CodVisita) VALUES("
            SQLStr += Val(txtCodice.Text.Trim).ToString.Trim
            SQLStr += ",'" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            SQLStr += "," & IIf(DDLTipoPagamento.SelectedValue.Trim = "", "NULL", DDLTipoPagamento.SelectedValue.Trim) & ""
            SQLStr += "," & IIf(DDLTipoScadenza.SelectedValue.Trim = "", "NULL", DDLTipoScadenza.SelectedValue.Trim) & ""
            SQLStr += "," & IIf(CheckFineMese.Checked = True, "1", "0") & ""
            SQLStr += "," & IIf(CheckAnticipato.Checked = True, "1", "0") & ""
            SQLStr += "," & IIf(DDLMeseCS.SelectedValue.Trim = "", "NULL", DDLMeseCS.SelectedValue.Trim) & ""
            SQLStr += "," & Val(IIf(txtGiornoFisso.Text.Trim = "", 0, txtGiornoFisso.Text.Trim)).ToString.Trim
            SQLStr += "," & IIf(DDLCausali.SelectedValue.Trim = "", "NULL", DDLCausali.SelectedValue.Trim) & ""
            SQLStr += "," & Val(IIf(txtNVisite.Text.Trim = "", 0, txtNVisite.Text.Trim)).ToString.Trim
            SQLStr += "," & Val(IIf(txtDurataNum.Text.Trim = "", 0, txtDurataNum.Text.Trim)).ToString.Trim
            SQLStr += ",'" & Controlla_Apice(txtCodVisita.Text.Trim) & "'"
            '-
            SQLStr += ")"
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiornamento", "Operazione di aggiornamento non definita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If

        Dim ObjDB As New DataBaseUtility
        Try
            OKAggiorna = ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr)
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
        Dim myID As String = Session(IDTIPOCONTRATTO)
        If IsNothing(myID) Then
            InitDettaglio()
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            InitDettaglio()
            Exit Sub
        End If
        LeggiDati(Session(IDTIPOCONTRATTO))
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
            Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
            txtCodice.Text = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            txtDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
            '-
            LeggiDati(Session(IDTIPOCONTRATTO))
            BtnSetEnabledTo(True)
        Catch ex As Exception
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
        End Try
    End Sub

    Private Sub LeggiDati(ByVal _Codice As Integer)
        Try
            Call InitDettaglio()
            Dim dvTipoCA As DataView
            dvTipoCA = SqlDSTElenco.Select(DataSourceSelectArguments.Empty)
            If (dvTipoCA Is Nothing) Then
                Exit Sub
            End If
            If dvTipoCA.Count > 0 Then
                dvTipoCA.RowFilter = "Codice = " & _Codice & ""
                If dvTipoCA.Count > 0 Then
                    txtCodice.Text = dvTipoCA.Item(0).Item("Codice").ToString.Trim
                    txtCodice.ToolTip = "" : txtCodice.BackColor = SEGNALA_OK
                    txtDescrizione.Text = dvTipoCA.Item(0).Item("Descrizione").ToString.Trim
                    txtDescrizione.ToolTip = "" : txtDescrizione.BackColor = SEGNALA_OK
                    Dim myStr As String = dvTipoCA.Item(0).Item("TipoPagamento").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "" Then
                        DDLTipoPagamento.SelectedIndex = -1
                    Else
                        Call PosizionaItemDDL(myStr.Trim, DDLTipoPagamento)
                    End If
                    '-
                    myStr = dvTipoCA.Item(0).Item("TipoScadenza").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "" Then
                        DDLTipoScadenza.SelectedIndex = -1
                    Else
                        Call PosizionaItemDDL(myStr.Trim, DDLTipoScadenza)
                    End If
                    '-
                    myStr = dvTipoCA.Item(0).Item("FineMese").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = "0"
                    CheckFineMese.Checked = CBool(myStr.Trim)
                    '-
                    myStr = dvTipoCA.Item(0).Item("Anticipato").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = "0"
                    CheckAnticipato.Checked = CBool(myStr.Trim)
                    '-
                    myStr = dvTipoCA.Item(0).Item("NVisite").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    txtNVisite.Text = myStr.Trim
                    '-
                    myStr = dvTipoCA.Item(0).Item("CodVisita").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    txtCodVisita.Text = myStr.Trim
                    If myStr.Trim <> "" Then
                        lblDesVisita.Text = App.GetValoreFromChiave(myStr.Trim, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
                        If lblDesVisita.Text.Trim = "" Then
                            txtCodVisita.BackColor = SEGNALA_KO
                        End If
                    End If
                    '-
                    myStr = dvTipoCA.Item(0).Item("DurataNum").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    txtDurataNum.Text = myStr.Trim
                    '-
                    myStr = dvTipoCA.Item(0).Item("MeseCS").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "" Then
                        DDLMeseCS.SelectedIndex = -1
                    Else
                        Call PosizionaItemDDL(myStr.Trim, DDLMeseCS)
                    End If
                    myStr = dvTipoCA.Item(0).Item("GiornoFisso").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    txtGiornoFisso.Text = myStr.Trim
                    '-
                    txtCodCausale.AutoPostBack = False : DDLCausali.AutoPostBack = False
                    myStr = dvTipoCA.Item(0).Item("Cod_Causale").ToString.Trim
                    If String.IsNullOrEmpty(myStr) Then myStr = ""
                    If myStr.Trim = "" Then
                        DDLCausali.SelectedIndex = -1
                    Else
                        Call PosizionaItemDDL(myStr.Trim, DDLCausali)
                    End If
                    txtCodCausale.Text = myStr
                    txtCodCausale.AutoPostBack = True : DDLCausali.AutoPostBack = True
                End If
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura dati Contratto", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub

    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(Codice) AS Numero From TipoContratto"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
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
            ModalPopup.Show("Errore in Lettura TipoCOntratto", "Verifica Codice da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
            Dim dvTipoCA As DataView
            dvTipoCA = SqlDSTElenco.Select(DataSourceSelectArguments.Empty)
            If (dvTipoCA Is Nothing) Then
                Exit Function
            End If
            If dvTipoCA.Count > 0 Then
                dvTipoCA.RowFilter = "Codice = " & _Codice & ""
                If dvTipoCA.Count > 0 Then
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
            Dim dvTipoCA As DataView
            dvTipoCA = SqlDSTElenco.Select(DataSourceSelectArguments.Empty)
            If (dvTipoCA Is Nothing) Then
                Exit Function
            End If
            If dvTipoCA.Count > 0 Then
                dvTipoCA.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
                If dvTipoCA.Count > 0 Then
                    Dim myCod As Integer = dvTipoCA.Item(0).Item("Codice")
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
            ModalPopup.Show("Errore in lettura dati TipoContratto (CheckNewDes)", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CheckDoc() = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina Tipo Contratto: " & txtDescrizione.Text.Trim, "Attenzione, impossibile eliminare il Tipo Pagamento. <br> Utilizzato in uno o piu documenti", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKCancella"
        ModalPopup.Show("Elimina Tipo Contratto: " & txtDescrizione.Text.Trim, "Confermi l'eliminazione del Tipo Contratto ? <br> " & txtDescrizione.Text.Trim, WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub OKCancella()

        Dim _strErrore As String = "" : Dim SWOK As Boolean = True
        Dim SQLStr As String = "DELETE FROM TipoContratto WHERE Codice=" & Val(txtCodice.Text.Trim)
        Dim ObjDB As New DataBaseUtility
        Try
            SWOK = ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr)
        Catch ex As Exception
            _strErrore = ex.Message.Trim
            SWOK = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina Causale", "Errore nella cancellazione TipoContratto. <br> " & _strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        GridViewPrevT.SelectedIndex = -1
        Session(IDTIPOCONTRATTO) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDTIPOCONTRATTO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(IDTIPOCONTRATTO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(IDTIPOCONTRATTO) = ""
        End If
    End Sub

    Private Function CheckDoc() As Boolean
        CheckDoc = False
        Dim strSQL As String = ""
        strSQL = "Select TOP 1 IDDocumenti From ContrattiT WHERE TipoContratto=" & Val(txtCodice.Text.Trim)
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
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
            ModalPopup.Show("Errore in Lettura ContrattiT", "Verifica Codice da eliminare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Private Sub txtCodCausale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCausale.TextChanged
        PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
    End Sub

    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        txtCodCausale.AutoPostBack = False
        txtCodCausale.Text = DDLCausali.SelectedValue
        txtCodCausale.AutoPostBack = True
    End Sub

    Private Sub txtCodVisita_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodVisita.TextChanged
        lblDesVisita.Text = App.GetValoreFromChiave(txtCodVisita.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
        If lblDesVisita.Text.Trim = "" Then
            txtCodVisita.BackColor = SEGNALA_KO
        End If
    End Sub
End Class