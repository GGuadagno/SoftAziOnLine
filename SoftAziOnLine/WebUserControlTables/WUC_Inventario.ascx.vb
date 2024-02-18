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
Imports SoftAziOnLine.Magazzino

Partial Public Class WUC_Inventario
    Inherits System.Web.UI.UserControl

    Private dsMagazzino1 As New DsMagazzino
    Private dvOrdineInv As New DataView
    Private dvOrdineInvS As New DataView

    Private TipoDoc As String = "" : Private TabCliFor As String = "" 'alb28012013 x stampa
    '
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceReparto.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceCategoria.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceLinea.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceFornitori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi) 'alb28012013
        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("..\WebFormTables\WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnGeneraInventario.Visible = False
        End If

        btnElencoSchedeIN.Visible = CKSchedeIN()
        btnEliminaSchede.Visible = btnElencoSchedeIN.Visible
        If (Not IsPostBack) Then
            Session("Codice_Mag") = 0
            ddlMagazzino.SelectedValue = 1
            btnGeneraInventario.Text = "Genera schede inventario"
            btnElencoSchedeIN.Text = "Gestione schede inventario"
            btnEliminaSchede.Text = "Elimina tutte le schede inventario"
            '==========================
            Dim strValore As String = "" : Dim StrErrore As String = ""
            If App.GetDatiAbilitazioni(CSTABILAZI, "NrPagIN", strValore, StrErrore) = True Then
                If StrErrore.Trim = "" Then
                    If IsNumeric(strValore.Trim) Then
                        If Val(strValore.Trim) > 0 Then
                            txtNRigheXPag.Text = Val(strValore.Trim)
                        Else
                            txtNRigheXPag.Text = "30"
                        End If
                    Else
                        txtNRigheXPag.Text = "30"
                    End If
                Else
                    txtNRigheXPag.Text = "30"
                End If
            Else
                txtNRigheXPag.Text = "30"
            End If
            '==========================
            'Ordinamento
            dsMagazzino1.Clear()
            Dim newRow1 As DsMagazzino.OrdineInventarioRow = dsMagazzino1.OrdineInventario.NewOrdineInventarioRow
            With newRow1
                .BeginEdit()
                .Riga = 1
                .NomeCampo = "Cod_Articolo"
                .Descrizione = "Codice articolo"
                .Selezionato = False
                .EndEdit()
            End With
            dsMagazzino1.OrdineInventario.AddOrdineInventarioRow(newRow1)
            newRow1 = Nothing
            '-
            Dim newRow2 As DsMagazzino.OrdineInventarioRow = dsMagazzino1.OrdineInventario.NewOrdineInventarioRow
            With newRow2
                .BeginEdit()
                .Riga = 2
                .NomeCampo = "Descrizione"
                .Descrizione = "Descrizione articolo"
                .Selezionato = False
                .EndEdit()
            End With
            dsMagazzino1.OrdineInventario.AddOrdineInventarioRow(newRow2)
            newRow2 = Nothing
            '-
            Dim newRow3 As DsMagazzino.OrdineInventarioRow = dsMagazzino1.OrdineInventario.NewOrdineInventarioRow
            With newRow3
                .BeginEdit()
                .Riga = 3
                .NomeCampo = "Reparto, Scaffale, Piano"
                .Descrizione = "Ubicazione [Reparto,Scaffale,Piano]"
                .Selezionato = False
                .EndEdit()
            End With
            dsMagazzino1.OrdineInventario.AddOrdineInventarioRow(newRow3)
            newRow3 = Nothing
            dvOrdineInv = New DataView(dsMagazzino1.OrdineInventario, "Selezionato=false", "Riga", DataViewRowState.CurrentRows)
            Session("dvOrdineInv") = dvOrdineInv
            GridView_OrdineInv.DataSource = dvOrdineInv
            GridView_OrdineInv.DataBind()
            '--
            dvOrdineInvS = New DataView(dsMagazzino1.SelOrdineInventario, "", "Ordine", DataViewRowState.CurrentRows)
            Session("dvOrdineInvS") = dvOrdineInvS
            GridView_OrdineInvSel.DataSource = dvOrdineInvS
            GridView_OrdineInvSel.DataBind()
            '---
            Session("dsMagazzino1") = dsMagazzino1
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub
    Private Function CKSchedeIn() As Boolean
        Dim StrErrore As String = ""
        Dim ClsMag As New Magazzino
        If ClsMag.GetPresenzaInventario(StrErrore) Then
            CKSchedeIn = True
        Else
            If StrErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        End If
    End Function

#Region "Ricerca e scelta articoli"

    Private Sub BtnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub
    Private Sub BtnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub
    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub
#End Region

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale ")
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''strRitorno = ""
        ' ''If strErrore.Trim = "" Then
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''Else
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        ' ''End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub

    Private Sub txtCodLinea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodLinea.TextChanged
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
        txtCod1.Focus()
    End Sub
    Private Sub ddlLinea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLinea.SelectedIndexChanged
        txtCodLinea.Text = ddlLinea.SelectedValue
        txtCodLinea.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
    End Sub

    'alb28012013
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        PosizionaItemDDLTxt(txtCodFornitore, DDLFornitori)
        txtCod1.Focus()
    End Sub
    Private Sub DDLFornitori_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLFornitori.SelectedIndexChanged
        txtCodFornitore.Text = DDLFornitori.SelectedValue
        txtCodFornitore.BackColor = SEGNALA_OK
        DDLFornitori.BackColor = SEGNALA_OK
    End Sub
    '----------
    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        If ddlMagazzino.SelectedIndex > 0 Then
            Session("Codice_Mag") = ddlMagazzino.SelectedValue
            ddlMagazzino.BackColor = SEGNALA_OK
        Else
            Session("Codice_Mag") = -1
            ddlMagazzino.BackColor = SEGNALA_KO
        End If
    End Sub

    Private Sub txtCodReparto_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodReparto.TextChanged
        PosizionaItemDDLTxt(txtCodReparto, DDLReparto)
        txtCodCategoria.Focus()
    End Sub
    Private Sub DDLReparto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLReparto.SelectedIndexChanged
        txtCodReparto.Text = DDLReparto.SelectedValue
        txtCodReparto.BackColor = SEGNALA_OK
        DDLReparto.BackColor = SEGNALA_OK
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        txtCodLinea.Focus()
    End Sub
    Private Sub ddlCatgoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatgoria.SelectedIndexChanged
        txtCodCategoria.Text = ddlCatgoria.SelectedValue
        txtCodCategoria.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
    End Sub

    Private Sub GridView_OrdineInv_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView_OrdineInv.SelectedIndexChanged
        dsMagazzino1 = Session("dsMagazzino1")
        Try
            Dim RigaSel As Integer = GridView_OrdineInv.SelectedDataKey.Value
            Dim RowsO() As DataRow = Me.dsMagazzino1.OrdineInventario.Select("Riga=" & RigaSel)
            Dim RowO As DsMagazzino.OrdineInventarioRow
            For Each RowO In RowsO
                If RowO.Riga = RigaSel Then
                    RowO.BeginEdit()
                    RowO.Selezionato = True
                    RowO.EndEdit()
                    '-
                    Dim TotRigaSel = Me.dsMagazzino1.SelOrdineInventario.Select().Length
                    Dim newRowOS As DsMagazzino.SelOrdineInventarioRow = Me.dsMagazzino1.SelOrdineInventario.NewSelOrdineInventarioRow
                    With newRowOS
                        .BeginEdit()
                        .Riga = RigaSel
                        .NomeCampo = RowO.NomeCampo
                        .Descrizione = RowO.Descrizione
                        .Ordine = TotRigaSel + 1
                        .EndEdit()
                    End With
                    Me.dsMagazzino1.SelOrdineInventario.AddSelOrdineInventarioRow(newRowOS)
                    newRowOS = Nothing
                End If
            Next
            dsMagazzino1.AcceptChanges()
            dvOrdineInv = New DataView(dsMagazzino1.OrdineInventario, "Selezionato=false", "Riga", DataViewRowState.CurrentRows)
            Session("dvOrdineInv") = dvOrdineInv
            GridView_OrdineInv.DataSource = dvOrdineInv
            GridView_OrdineInv.DataBind()
            '--
            dvOrdineInvS = New DataView(dsMagazzino1.SelOrdineInventario, "", "Ordine", DataViewRowState.CurrentRows)
            Session("dvOrdineInvS") = dvOrdineInvS
            GridView_OrdineInvSel.DataSource = dvOrdineInvS
            GridView_OrdineInvSel.DataBind()
            '--
            Session("dsMagazzino1") = dsMagazzino1
            '------------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in seleziona ordine: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Private Sub GridView_OrdineInvSel_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView_OrdineInvSel.SelectedIndexChanged
        dsMagazzino1 = Session("dsMagazzino1")
        Try
            Dim RigaSel As Integer = GridView_OrdineInvSel.SelectedDataKey.Value
            Dim RowsOS() As DataRow = Me.dsMagazzino1.SelOrdineInventario.Select("Riga=" & RigaSel)
            Dim RowOS As DsMagazzino.SelOrdineInventarioRow
            For Each RowOS In RowsOS
                If RowOS.Riga = RigaSel Then
                    RowOS.Delete()
                End If
            Next
            '--
            Dim RowsO() As DataRow = Me.dsMagazzino1.OrdineInventario.Select("Riga=" & RigaSel)
            Dim RowO As DsMagazzino.OrdineInventarioRow
            For Each RowO In RowsO
                If RowO.Riga = RigaSel Then
                    RowO.BeginEdit()
                    RowO.Selezionato = False
                    RowO.EndEdit()
                End If
            Next
            '---------------
            dsMagazzino1.AcceptChanges()
            dvOrdineInv = New DataView(dsMagazzino1.OrdineInventario, "Selezionato=false", "Riga", DataViewRowState.CurrentRows)
            Session("dvOrdineInv") = dvOrdineInv
            GridView_OrdineInv.DataSource = dvOrdineInv
            GridView_OrdineInv.DataBind()
            '--
            dvOrdineInvS = New DataView(dsMagazzino1.SelOrdineInventario, "", "Ordine", DataViewRowState.CurrentRows)
            Session("dvOrdineInvS") = dvOrdineInvS
            GridView_OrdineInvSel.DataSource = dvOrdineInvS
            GridView_OrdineInvSel.DataBind()
            '--
            Session("dsMagazzino1") = dsMagazzino1
            '------------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in seleziona ordine: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnGeneraInventario_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGeneraInventario.Click
        'SELEZIONE
        Dim Errore As Boolean = False
        If ddlMagazzino.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If DDLReparto.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlCatgoria.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlLinea.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlMagazzino.Text = "" Then
            Errore = True
        End If
        If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
            If txtCod2.Text.Trim < txtCod1.Text.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codici articoli incongruenti. <br> (Al codice < di Dal codice).", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Not IsNumeric(txtNRigheXPag.Text.Trim) Then
            If Val(txtNRigheXPag.Text.Trim) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, N° righe per pagina obbligatorio.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Errore Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati selezione", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim StrErrore As String = ""
        Dim ClsMag As New Magazzino

        If ClsMag.GetPresenzaInventario(StrErrore) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "" 'giu020714 "CreaNewIN"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera schede inventario", "In archivio sono già presenti delle schede inventario, cancellare o chiudere prima le schede inventario aperte.", WUC_ModalPopup.TYPE_ALERT)
        Else
            If StrErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            btnElencoSchedeIN.Visible = True
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewIN"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera schede inventario", "Confermi la generazione delle schede inventario ? " & vbCr & _
                                "NOTA: L'operazione potrebbe richiedere alcui minuti. " & vbCr & _
                                "Se dopo alcuni muniti l'applicazione non rispondesse ripondere No " & vbCr & _
                                "e andare in gestione schede inventario per verificare le schede create.", WUC_ModalPopup.TYPE_CONFIRM)
        End If

    End Sub
    Public Sub CreaNewIN()
        'definito sopra Dim dsMagazzino1 As New DsMagazzino
        dsMagazzino1 = Session("dsMagazzino1")
        Dim ClsPrint As New Magazzino
        Dim Errore As Boolean = False
        Dim Filtri As String = ""
        Dim Selezione As String = ""
        Dim StrErroreDisp As String = ""
        'SELEZIONE
        If ddlMagazzino.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If DDLReparto.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlCatgoria.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlLinea.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        'alb28012013
        If DDLFornitori.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlMagazzino.Text = "" Then
            Errore = True
        End If
        If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
            If txtCod2.Text.Trim < txtCod1.Text.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codici articoli incongruenti. <br> (Al codice < di Dal codice).", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Not IsNumeric(txtNRigheXPag.Text.Trim) Then
            If Val(txtNRigheXPag.Text.Trim) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, N° righe per pagina obbligatorio.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Errore Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati selezione", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim strErroreGiac As String = ""
        Dim SWNegativi As Boolean = False
        If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then 'giu190613 aggiunto SWNegativi
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        'FILTRO PER MAGAZZINO
        Selezione = "WHERE Codice_Magazzino = " & ddlMagazzino.SelectedValue
        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
        'FILTRO PER FORNITORE alb28012013
        If txtCodFornitore.Text <> "" Then
            Selezione = Selezione & " AND Cod_Fornitore = '" & txtCodFornitore.Text & "'"
            Filtri = Filtri & " | Fornitore - " & DDLFornitori.SelectedItem.Text.ToUpper
        End If
        '--------------------
        'REPARTO
        If txtCodReparto.Text.Trim <> "" Then
            If IsNumeric(txtCodReparto.Text.Trim) Then
                If Val(txtCodReparto.Text.Trim) > 0 Then
                    Selezione = Selezione & " AND Reparto = " & txtCodReparto.Text.Trim & ""
                    Filtri = Filtri & " | Reparto - " & DDLReparto.SelectedItem.Text.ToUpper
                End If
            End If
        End If
        'FILTRO PER CATEGORIA
        If txtCodCategoria.Text <> "" Then
            Selezione = Selezione & " AND CodCategoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
        End If
        'FILTRO PER LINEA
        If txtCodLinea.Text <> "" Then
            Selezione = Selezione & " AND CodLinea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
        End If
        'FILTRO DA ARTICOLO 
        If txtCod1.Text <> "" Then
            Selezione = Selezione & " AND Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
        End If
        'FILTRO A ARTICOLO
        If txtCod2.Text <> "" Then
            Selezione = Selezione & " AND Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
        End If

        'ORDINAMENTO PER:
        If dsMagazzino1.SelOrdineInventario.Select("").Length = 0 Then
            Selezione = Selezione & " ORDER BY Cod_Articolo"
        Else
            Dim myOrdine As String = ""
            Dim RowsOS() As DataRow = Me.dsMagazzino1.SelOrdineInventario.Select("", "Ordine")
            Dim RowOS As DsMagazzino.SelOrdineInventarioRow
            For Each RowOS In RowsOS
                If myOrdine.Trim <> "" And RowOS.NomeCampo.Trim <> "" Then
                    myOrdine += ","
                End If
                myOrdine += RowOS.NomeCampo
            Next
            If myOrdine.Trim = "" Then
                Selezione = Selezione & " ORDER BY Cod_Articolo"
            Else
                Selezione = Selezione & " ORDER BY " & myOrdine.Trim
            End If
        End If
        '
        'ELABORO LA DISPONIBILITA'
        'GIU040920 INVIO CODICE MAGAZZINO SU CUI OPERARE
        If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Schede inventario", Filtri, Selezione, dsMagazzino1, StrErroreDisp, False, False, False, False, True, ddlMagazzino.SelectedValue) Then
            If dsMagazzino1.DispMagazzino.Rows.Count > 0 Then
                Session("dsMagazzino1") = dsMagazzino1
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaIn"
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                Call OKCreaIn()
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Genera schede inventario", "Nessun articolo selezionato.", WUC_ModalPopup.TYPE_INFO)
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErroreDisp, WUC_ModalPopup.TYPE_ERROR)
        End If
    End Sub
    Private Function GetNewIN() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE Tipo_Doc = '" & SWTD(TD.Inventari) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewIN = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewIN = 1
                    End If
                    Exit Function
                Else
                    GetNewIN = 1
                    Exit Function
                End If
            Else
                GetNewIN = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewIN = -1
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function
    Public Sub OKCreaIn()
        dsMagazzino1 = Session("dsMagazzino1")
        Dim ClsPrint As New Magazzino
        Dim StrErrore As String = ""
        Dim NDoc As Long = GetNewIN()
        Dim NRev As Integer = 0 : Dim RighePerPaginaIN As Integer = Val(txtNRigheXPag.Text.Trim)
        'giu180714-------------------------------
        If ClsPrint.CreateInventario(dsMagazzino1, NDoc, NRev, RighePerPaginaIN, chkUnaPagina.Checked, _
                                     Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50), ddlMagazzino.SelectedValue, StrErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore creazione schede inventario", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------------------------------------
        'OK FATTO
        Session(SWOP) = SWOPNESSUNA
        Session(CSTTIPODOC) = SWTD(TD.Inventari)
        'giu170714
        ' ''If Not chkUnaPagina.Checked Or Not chkStampa.Checked Then
        ' ''    Response.Redirect("..\WebFormTables\WF_ElencoSchedeInventario.aspx?labelForm=Gestione schede inventario")
        ' ''Else
        ' ''    StampaInventarioCreato()
        ' ''End If
        Response.Redirect("..\WebFormTables\WF_ElencoSchedeInventario.aspx?labelForm=Gestione schede inventario")
        '-----------------------------------------------------------------------
    End Sub


    Private Sub btnElencoSchedeIN_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElencoSchedeIN.Click
        Session(SWOP) = SWOPNESSUNA
        Session(CSTTIPODOC) = SWTD(TD.Inventari)
        Response.Redirect("..\WebFormTables\WF_ElencoSchedeInventario.aspx?labelForm=Gestione schede inventario")
    End Sub

    Private Sub btnEliminaSchede_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaSchede.Click
        'DelInventario
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKEliminaSchede"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Elimina schede inventario", "Confermi l'eliminazione delle schede inventario ancora aperte?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub OKEliminaSchede()
       
        Dim strErrore As String = ""
        Dim ClsMag As New Magazzino
        If ClsMag.DelInventario(strErrore) = True Then
            btnElencoSchedeIN.Visible = CKSchedeIn()
            btnEliminaSchede.Visible = btnElencoSchedeIN.Visible

            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina schede inventario aperte", "Operazione copletata.", WUC_ModalPopup.TYPE_INFO)

        Else
            btnElencoSchedeIN.Visible = CKSchedeIn()
            btnEliminaSchede.Visible = btnElencoSchedeIN.Visible
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina schede inventario aperte", "Operazione fallita. Err.: " & strErrore.Trim, WUC_ModalPopup.TYPE_ALERT)

        End If
    End Sub

    Private Sub chkUnaPagina_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUnaPagina.CheckedChanged
        If chkUnaPagina.Checked Then
            txtNRigheXPag.Enabled = False
        Else
            txtNRigheXPag.Enabled = True
        End If
    End Sub

    'alb28012013
    Private Sub StampaInventarioCreato()
        Dim myID As String = Session(IDDOCUMENTI)

        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If

        Try
            DsPrinWebDoc.Clear()
            If ClsPrint.StampaSchedaIN(Session(IDDOCUMENTI), DsPrinWebDoc, ObjReport, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTNOBACK) = 0
                Session(CSTTipoStampaIN) = 1
                Response.Redirect("..\WebFormTables\WF_PrintWeb_IN.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                Exit Sub
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Public Sub Chiudi(ByVal strErrore As String)

        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale " & strErrore)
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

    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function
    '------------------
End Class