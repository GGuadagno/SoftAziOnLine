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

Partial Public Class WUC_ArticoliUbicazione
    Inherits System.Web.UI.UserControl

    Private Enum CellIdxT
        Sel = 1
        CodArt = 2
        DesArt = 3
        SottoSc = 4
        CReparto = 5
        CScaffale = 6
        Piano = 7
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSReparto.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSScaffale.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            Try
                Session(IDMAGAZZINO) = 1 'fisso
                Try
                    Call PosizionaItemDDL(Session(IDMAGAZZINO).ToString.Trim, ddlMagazzino, True) 'true per Ok a codici a 0
                Catch ex As Exception
                    ddlMagazzino.SelectedIndex = 0
                End Try
                btnModifica.Text = "Modifica ubicazione"
                btnSelTutti.Text = "Seleziona tutti"
                btnDeselTutti.Text = "Deseleziona tutti"
                btnDeselTutti.Enabled = False
                btnStampaElenco.Text = "Stampa elenco"

                ddlRicerca.Items.Add("Codice Articolo")
                ddlRicerca.Items(0).Value = "C" 'Cod_Articolo
                ddlRicerca.Items.Add("Descrizione Articolo")
                ddlRicerca.Items(1).Value = "D" 'Decrizione
                '-
                ddlRicerca.Items.Add("Reparto")
                ddlRicerca.Items(2).Value = "R" 'Reparto
                ddlRicerca.Items.Add("Scaffale")
                ddlRicerca.Items(3).Value = "S" 'Scaffale
                ddlRicerca.Items.Add("Piano")
                ddlRicerca.Items(4).Value = "P" 'Piano
                '-
                BuidDett()
                Session(SWOP) = SWOPNESSUNA
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Articoli : " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        WFP_Reparti1.WucElement = Me
        If Session(F_ANAGRREP_APERTA) = True Then
            WFP_Reparti1.Show()
        End If
        '-
        WFP_Scaffali1.WucElement = Me
        If Session(F_ANAGRSCAFFALI_APERTA) = True Then
            WFP_Scaffali1.Show()
        End If
        '-
        WUC_SceltaStampaUbiArt1.WucElement = Me
    End Sub

    Private Sub BtnSetEnabledTo(ByRef Valore As Boolean)
        btnStampaElenco.Enabled = Valore
        btnSelTutti.Enabled = Valore
        btnDeselTutti.Enabled = Valore
        If lblCodArticolo.Text.Trim = "" Then
            Valore = False
        End If
        btnModifica.Enabled = Valore
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

        txtSottoscorta.Enabled = Valore
        btnReparto.Enabled = Valore : txtReparto.Enabled = Valore : ddlReparto.Enabled = Valore
        btnScaffale.Enabled = Valore : txtScaffale.Enabled = Valore : DDLScaffale.Enabled = Valore
        txtPiano.Enabled = Valore
        CheckAggiornaSel.Enabled = Valore
    End Sub
    Private Sub InitDettaglio()
        lblCodArticolo.Text = "" : lblDescrizione.Text = ""
        txtSottoscorta.Text = ""
        txtReparto.Text = "" : PosizionaItemDDLByTxt(txtReparto, ddlReparto)
        txtScaffale.Text = "" : PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
        txtPiano.Text = ""
        CheckAggiornaSel.Enabled = False
    End Sub

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(COD_ARTICOLO) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(COD_ARTICOLO) = ""
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            ' ''e.Row.Cells(CellIdxT.DesArt).Text = Mid(e.Row.Cells(CellIdxT.DesArt).Text.Trim, 1, 100)
            If IsNumeric(e.Row.Cells(CellIdxT.SottoSc).Text) Then
                If CLng(e.Row.Cells(CellIdxT.SottoSc).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.SottoSc).Text = FormattaNumero(CLng(e.Row.Cells(CellIdxT.SottoSc).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.SottoSc).Text = ""
                End If
            Else
                e.Row.Cells(CellIdxT.CReparto).Text = ""
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.CReparto).Text) Then
                If CLng(e.Row.Cells(CellIdxT.CReparto).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.CReparto).Text = FormattaNumero(CLng(e.Row.Cells(CellIdxT.CReparto).Text)).ToString
                Else
                    e.Row.Cells(CellIdxT.CReparto).Text = ""
                End If
            Else
                e.Row.Cells(CellIdxT.CReparto).Text = ""
            End If
            '-
            If IsNumeric(e.Row.Cells(CellIdxT.CScaffale).Text) Then
                If CLng(e.Row.Cells(CellIdxT.CScaffale).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.CScaffale).Text = FormattaNumero(CLng(e.Row.Cells(CellIdxT.CScaffale).Text)).ToString
                Else
                    e.Row.Cells(CellIdxT.CScaffale).Text = ""
                End If
            Else
                e.Row.Cells(CellIdxT.CScaffale).Text = ""
            End If
            '-
            If IsNumeric(e.Row.Cells(CellIdxT.Piano).Text) Then
                If CLng(e.Row.Cells(CellIdxT.Piano).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Piano).Text = FormattaNumero(CLng(e.Row.Cells(CellIdxT.Piano).Text)).ToString
                Else
                    e.Row.Cells(CellIdxT.Piano).Text = ""
                End If
            Else
                e.Row.Cells(CellIdxT.Piano).Text = ""
            End If
        End If
    End Sub

    Private Sub BuidDett()
        CheckAggiornaSel.Checked = False
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
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
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        If ddlRicerca.SelectedValue = "R" Or ddlRicerca.SelectedValue = "S" Or ddlRicerca.SelectedValue = "P" Then
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
                    Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                Catch ex As Exception
                    InitDettaglio()
                    BtnSetEnabledTo(False)
                    Session(COD_ARTICOLO) = ""
                End Try
            Else
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "C" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Cod_Articolo like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSTElenco.FilterExpression = "Cod_Articolo = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSTElenco.FilterExpression = "Descrizione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "R" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Reparto >= " & txtRicerca.Text.Trim
            Else
                SqlDSTElenco.FilterExpression = "Reparto = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "S" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Scaffale >= " & txtRicerca.Text.Trim
            Else
                SqlDSTElenco.FilterExpression = "Scaffale = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "P" Then
            If checkParoleContenute.Checked = True Then
                SqlDSTElenco.FilterExpression = "Piano >= " & txtRicerca.Text.Trim
            Else
                SqlDSTElenco.FilterExpression = "Piano = " & txtRicerca.Text.Trim
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
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
    End Sub
#End Region

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKAbilitato(SWOPMODIFICA) = False Then Exit Sub
        
        Dim myID As String = Session(COD_ARTICOLO)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Articolo selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Articolo selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        SetDettaglio(True)
        btnModifica.Enabled = False : btnStampaElenco.Enabled = False
        btnSelTutti.Enabled = False : btnDeselTutti.Enabled = False
        btnAggiorna.Enabled = True : btnAnnulla.Enabled = True
        txtSottoscorta.Focus()
    End Sub
    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim myID As String = Session(COD_ARTICOLO)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Articolo selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Articolo selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWErrore As Boolean = False
        If txtSottoscorta.Text.Trim = "" Then txtSottoscorta.Text = "0"
        If Not IsNumeric(txtSottoscorta.Text.Trim) Then txtSottoscorta.BackColor = SEGNALA_KO : SWErrore = True
        PosizionaItemDDLByTxt(txtReparto, ddlReparto)
        If txtReparto.BackColor = SEGNALA_KO Then SWErrore = True
        PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
        If txtScaffale.BackColor = SEGNALA_KO Then SWErrore = True
        If ddlReparto.BackColor = SEGNALA_KO Then SWErrore = True
        If DDLScaffale.BackColor = SEGNALA_KO Then SWErrore = True
        If txtPiano.BackColor = SEGNALA_KO Then SWErrore = True
        If txtPiano.Text.Trim = "" Then txtPiano.Text = "0"
        If Not IsNumeric(txtPiano.Text.Trim) Then txtPiano.BackColor = SEGNALA_KO : SWErrore = True
        If SWErrore = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU270820 NON CSERVE A UN CAZZO
        ' ''If CheckAggiornaSel.Checked = False Then
        ' ''    If CKSel() = True Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Controllo dati", "Attenzione, sono stati selezionati articoli <br> " & _
        ' ''                        "ma non è stato selezionato -Aggiorna tutti gli articoli selezionati- <br> " & _
        ' ''                        "Per evitare questa segnalazione Deselezionate tutti.", WUC_ModalPopup.TYPE_ALERT)
        ' ''        Exit Sub
        ' ''    End If
        ' ''Else
        ' ''    If CKSel() = False Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Controllo dati", "Attenzione, è stato selezionato <br> " & _
        ' ''                        "-Aggiorna tutti gli articoli selezionati- <br> " & _
        ' ''                        "ma non è stato selezionato alcun altricolo <br> " & _
        ' ''                        "Per evitare questa segnalazione Selezionare uno o più articoli.", WUC_ModalPopup.TYPE_ALERT)
        ' ''        Exit Sub
        ' ''    End If
        ' ''End If
        'OK AGGIORNO L'ARTICOLO SELEZIONATO E POI GLI EVENTUALI SELEZIONATI
        If txtSottoscorta.Text.Trim = "" Then txtSottoscorta.Text = "0"
        If Not IsNumeric(txtSottoscorta.Text.Trim) Then txtSottoscorta.Text = "0"
        If txtReparto.Text.Trim = "" Then txtReparto.Text = "0"
        If Not IsNumeric(txtReparto.Text.Trim) Then txtReparto.Text = "0"
        If txtScaffale.Text.Trim = "" Then txtScaffale.Text = "0"
        If Not IsNumeric(txtScaffale.Text.Trim) Then txtScaffale.Text = "0"
        If txtPiano.Text.Trim = "" Then txtPiano.Text = "0"
        If Not IsNumeric(txtPiano.Text.Trim) Then txtPiano.Text = "0"
        Dim myErrore As String = ""
        If OKAggiorna(Session(COD_ARTICOLO).ToString.Trim, txtReparto.Text.Trim, txtScaffale.Text.Trim, txtPiano.Text.Trim, txtSottoscorta.Text.Trim, myErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiornamento", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf CheckAggiornaSel.Checked = False Then
            Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
            lblCodArticolo.Text = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            row.Cells(CellIdxT.SottoSc).Text = txtSottoscorta.Text.Trim
            row.Cells(CellIdxT.CReparto).Text = txtReparto.Text.Trim
            row.Cells(CellIdxT.CScaffale).Text = txtScaffale.Text.Trim
            row.Cells(CellIdxT.Piano).Text = txtPiano.Text.Trim
            BtnSetEnabledTo(True)
            Exit Sub
        End If
        If CheckAggiornaSel.Checked = True Then
            If OKAggiornaSel(Session(COD_ARTICOLO).ToString.Trim, txtReparto.Text.Trim, txtScaffale.Text.Trim, txtPiano.Text.Trim, txtSottoscorta.Text.Trim, myErrore) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiornamento (Tutti gli articoli selezionati)", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        CheckAggiornaSel.Checked = False
        SqlDSTElenco.DataBind()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
    End Sub
    ' ''Private Function CKSel() As Boolean
    ' ''    For Each row As GridViewRow In GridViewPrevT.Rows
    ' ''        Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
    ' ''        If checkSel.Checked = True Then
    ' ''            CKSel = True
    ' ''            Exit Function
    ' ''        End If
    ' ''    Next
    ' ''End Function
    Private Function OKAggiornaSel(ByVal _CArt As String, ByVal _CRep As String, ByVal _CScaff As String, ByVal _Piano As String, ByVal _SottoSc As String, ByRef _strErrore As String) As Boolean
        'GIU280820
        OKAggiornaSel = False
        Dim myCMag As String = Session(IDMAGAZZINO)
        If IsNothing(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        End If
        If String.IsNullOrEmpty(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        ElseIf Not IsNumeric(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        End If
        '-
        Dim myCArt As String = ""
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            myCArt = row.Cells(CellIdxT.CodArt).Text.Trim
            If checkSel.Checked = True Then
                If myCArt.Trim <> _CArt.Trim Then
                    If OKAggiorna(myCArt.Trim, _CRep.Trim, _CScaff.Trim, _Piano.Trim, _SottoSc, _strErrore) = False Then
                        Exit Function
                    End If
                End If
            End If
        Next
        OKAggiornaSel = True
    End Function
    Private Function OKAggiorna(ByVal _CArt As String, ByVal _CRep As String, ByVal _CScaff As String, ByVal _Piano As String, ByVal _SottoSc As String, ByRef _strErrore As String) As Boolean
        _strErrore = ""
        'GIU280820
        OKAggiorna = False
        Dim myCMag As String = Session(IDMAGAZZINO)
        If IsNothing(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        End If
        If String.IsNullOrEmpty(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        ElseIf Not IsNumeric(myCMag) Then
            _strErrore = "Attenzione, nessun MAGAZZINO selezionato"
            Exit Function
        End If
        '-
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            If CLng(myCMag) = 0 Or CLng(myCMag) = 1 Then
                SQLStr = "UPDATE AnaMag SET Reparto= " & _CRep.Trim
                SQLStr += ",Scaffale= " & _CScaff.Trim
                SQLStr += ",Piano= " & _Piano.Trim
                SQLStr += ",Sottoscorta= " & _SottoSc.ToString.Trim.Replace(",", ".")
                SQLStr += " WHERE (Cod_Articolo = '" & _CArt.Trim & "')"
                OKAggiorna = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
            End If
            SQLStr = "UPDATE ArtDiMag SET Reparto= " & _CRep.Trim
            SQLStr += ",Scaffale= " & _CScaff.Trim
            SQLStr += ",Piano= " & _Piano.Trim
            SQLStr += ",Sottoscorta= " & _SottoSc.ToString.Trim.Replace(",", ".")
            SQLStr += " WHERE (Codice_Magazzino = " & myCMag.Trim & ") AND (Cod_Articolo = '" & _CArt.Trim & "')"
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
        btnModifica.Enabled = True : btnStampaElenco.Enabled = True
        btnAggiorna.Enabled = False : btnAnnulla.Enabled = False
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
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
        WUC_SceltaStampaUbiArt1.Show("Stampa Sottoscorta - Ubicazione articoli<br>Magazzino:" + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", ""))
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
            Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
            lblCodArticolo.Text = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            lblDescrizione.Text = row.Cells(CellIdxT.DesArt).Text.Trim
            '-
            txtSottoscorta.Text = row.Cells(CellIdxT.SottoSc).Text.Trim
            txtReparto.Text = row.Cells(CellIdxT.CReparto).Text.Trim
            Session(IDREPARTO) = txtReparto.Text.Trim
            SqlDSScaffale.DataBind()
            DDLScaffale.Items.Clear()
            DDLScaffale.Items.Add("")
            DDLScaffale.DataBind()
            PosizionaItemDDLByTxt(txtReparto, ddlReparto)
            '-
            txtScaffale.Text = row.Cells(CellIdxT.CScaffale).Text.Trim
            PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
            '-
            txtPiano.Text = row.Cells(CellIdxT.Piano).Text.Trim
            BtnSetEnabledTo(True)
        Catch ex As Exception
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End Try
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelTutti.Click
        btnDeselTutti.Enabled = True
        SelezionaTutti()
        CheckAggiornaSel.Checked = True
    End Sub
    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeselTutti.Click
        btnDeselTutti.Enabled = False
        DeselezionaTutti()
        CheckAggiornaSel.Checked = False
    End Sub
    Private Sub SelezionaTutti()
        Try
            For Each row As GridViewRow In GridViewPrevT.Rows
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                checkSel.Checked = True
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub DeselezionaTutti()
        Try
            For Each row As GridViewRow In GridViewPrevT.Rows
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                checkSel.Checked = False
            Next
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub ddlReparto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlReparto.SelectedIndexChanged
        Session(IDREPARTO) = ddlReparto.SelectedValue
        txtReparto.Text = ddlReparto.SelectedValue
        SqlDSScaffale.DataBind()
        DDLScaffale.Items.Clear()
        DDLScaffale.Items.Add("")
        DDLScaffale.DataBind()
        txtScaffale.Text = "" : txtPiano.Text = "" : txtScaffale.Focus()
    End Sub

    Private Sub DDLScaffale_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLScaffale.SelectedIndexChanged
        txtScaffale.Text = DDLScaffale.SelectedValue
        txtPiano.Focus()
    End Sub

    Private Sub txtReparto_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtReparto.TextChanged
        Session(IDREPARTO) = txtReparto.Text.Trim
        SqlDSScaffale.DataBind()
        DDLScaffale.Items.Clear()
        DDLScaffale.Items.Add("")
        DDLScaffale.DataBind()
        PosizionaItemDDLByTxt(txtReparto, ddlReparto)
        txtScaffale.Text = "" : txtPiano.Text = "" : txtScaffale.Focus()
    End Sub

    Private Sub txtScaffale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtScaffale.TextChanged
        PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
        txtPiano.Focus()
    End Sub

    Private Sub btnReparto_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReparto.Click
        WFP_Reparti1.WucElement = Me
        WFP_Reparti1.SvuotaCampi()
        WFP_Reparti1.SetlblMessaggi("")
        Session(F_ANAGRREP_APERTA) = True
        WFP_Reparti1.Show()
    End Sub
    Public Sub CallBackWFPReparti()
        Session(IDREPARTO) = ""
        Dim rk As StrReparti
        rk = Session(RKREPARTI)
        If IsNothing(rk.IDReparto) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDREPARTO) = rk.IDReparto
        txtReparto.Text = rk.IDReparto
        SqlDSReparto.DataBind()
        ddlReparto.Items.Clear()
        ddlReparto.Items.Add("")
        ddlReparto.DataBind()
        PosizionaItemDDLByTxt(txtReparto, ddlReparto)
    End Sub
    Public Sub CancBackWFPReparti()
        SqlDSReparto.DataBind()
        ddlReparto.Items.Clear()
        ddlReparto.Items.Add("")
        ddlReparto.DataBind()
        PosizionaItemDDLByTxt(txtReparto, ddlReparto)
    End Sub

    Private Sub btnScaffale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnScaffale.Click
        WFP_Scaffali1.WucElement = Me
        WFP_Scaffali1.SvuotaCampi()
        WFP_Scaffali1.SetlblMessaggi("")
        Session(F_ANAGRSCAFFALI_APERTA) = True
        WFP_Scaffali1.Show()
    End Sub
    Public Sub CallBackWFPScaffali()
        Session(IDSCAFFALE) = ""
        Dim rk As StrScaffali
        rk = Session(RKSCAFFALI)
        If IsNothing(rk.IDScaffale) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDSCAFFALE) = rk.IDScaffale
        txtScaffale.Text = rk.IDScaffale
        SqlDSScaffale.DataBind()
        DDLScaffale.Items.Clear()
        DDLScaffale.Items.Add("")
        DDLScaffale.DataBind()
        PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
    End Sub
    Public Sub CancBackWFPScaffali()
        SqlDSScaffale.DataBind()
        DDLScaffale.Items.Clear()
        DDLScaffale.Items.Add("")
        DDLScaffale.DataBind()
        PosizionaItemDDLByTxt(txtScaffale, DDLScaffale)
    End Sub

    Public Sub CallBackSceltaStampaUbiMag(ByVal Ordinamento As String)
        Dim dsAnaMag1 As New DSAnaMag
        Dim ObjReport As New Object
        Dim ClsPrint As New Listini
        Dim StrErrore As String = ""
        Dim strTmpTitoloRpt As String

        Dim SWSconti As Boolean = False


        Try
            Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazione
            strTmpTitoloRpt = "Riepilogo Sottoscorta - Ubicazione articoli"
            Dim strNomeAz As String = ""
            strNomeAz = Session(CSTAZIENDARPT).ToString.Trim
            Select Case Ordinamento
                Case "Cod_Articolo"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per codice articolo"
                Case "Descrizione"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per descrizione articolo"
                Case "Reparto, Scaffale, Piano, Descrizione"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per reparto, scaffale e piano"
            End Select

            strTmpTitoloRpt += " - Magazzino: " + ddlMagazzino.SelectedItem.Text.ToUpper.Trim.Replace("MAGAZZINO", "")

            If ClsPrint.StampaUbiMag(strNomeAz, strTmpTitoloRpt, dsAnaMag1, ObjReport, StrErrore, Ordinamento, ddlMagazzino.SelectedValue.Trim) Then
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
            ModalPopup.Show("Errore in ArticoliUbicazione.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        InitDettaglio()
        BtnSetEnabledTo(False)
        Session(COD_ARTICOLO) = ""
        If ddlMagazzino.SelectedIndex > 0 Then
            Session(IDMAGAZZINO) = ddlMagazzino.SelectedValue
        Else
            Session(IDMAGAZZINO) = -1
        End If
        '-
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
                Session(COD_ARTICOLO) = GridViewPrevT.SelectedDataKey.Value
            Catch ex As Exception
                InitDettaglio()
                BtnSetEnabledTo(False)
                Session(COD_ARTICOLO) = ""
            End Try
        Else
            InitDettaglio()
            BtnSetEnabledTo(False)
            Session(COD_ARTICOLO) = ""
        End If
    End Sub
End Class