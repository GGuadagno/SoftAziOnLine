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

Partial Public Class WUC_ElencoMovMag
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""

    Private Enum CellIdxT
        TipoDC = 1
        Causale = 2
        Magazzino = 3
        NumDoc = 4
        DataDoc = 5
        CodCliForProvv = 6
        RagSoc = 7
        DataCons = 13
        DataVal = 14
        Riferimento = 15
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        QtaOr = 3
        QtaEv = 4
        QtaRe = 5
        IVA = 6
        Prz = 7
        ScV = 8
        Sc1 = 9
        Importo = 10
        ScR = 11
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ElencoMovMag.aspx?labelForm=Gestione movimenti di magazzino"
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSCausale.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        'giu15112011 da ATTIVARE
        btnCambiaStato.Visible = False
        '-----------------------
        If (Not IsPostBack) Then
            Try
                btnSblocca.Text = "Sblocca documento"
                btnSblocca.Visible = False
                btnResoClienteFornitore.Text = "Creazione Reso"
                btnStampaNoLotti.Text = "Stampa movim. (No Lotti)"
                btnStampaSiLotti.Text = "Stampa movim. (Si Lotti)"
                'GIU210612 
                Dim SWRbtnMM As String = Session(CSTSWRbtnMM)
                If IsNothing(SWRbtnMM) Then
                    SWRbtnMM = ""
                End If
                If String.IsNullOrEmpty(SWRbtnMM) Then
                    SWRbtnMM = ""
                End If
                ' ''ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                If SWRbtnMM = "" Or SWRbtnMM = "TUTTI" Then
                    rbtnTutti.Checked = True
                    Session(CSTSWRbtnMM) = "TUTTI"
                    ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = "ZZ"
                    Session(CSTTIPODOCSEL) = "ZZ"
                    Session(CSTSTATODOC) = "999"
                ElseIf SWRbtnMM = "CAR" Then
                    rbtnCarichi.Checked = True
                    ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = "ZZ" 'SWTD(TD.CaricoMagazzino)
                    Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.CaricoMagazzino)
                    Session(CSTSTATODOC) = "999"
                ElseIf SWRbtnMM = "CONF" Then
                    rbtnDocStato5.Checked = True
                    ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) '"ZZ" '
                    Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.CaricoMagazzino)
                    'GIU110512
                    Session(CSTSTATODOC) = "5"
                ElseIf SWRbtnMM = "SCAR" Then
                    rbtnScarichi.Checked = True
                    ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = "ZZ" 'SWTD(TD.ScaricoMagazzino)
                    Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.ScaricoMagazzino)
                    Session(CSTSTATODOC) = "999"
                ElseIf SWRbtnMM = "CAUS" Then
                    rbtnCausale.Checked = True
                    ddlCausale.Enabled = True : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
                    Session(CSTTIPODOCSEL) = "Z"
                    Session(CSTSTATODOC) = "999"
                Else
                    rbtnTutti.Checked = True
                    Session(CSTSWRbtnMM) = "TUTTI"
                    ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
                    Session(CSTSORTPREVTEL) = "D"
                    Session(CSTTIPODOC) = "ZZ"
                    Session(CSTTIPODOCSEL) = "ZZ"
                    Session(CSTSTATODOC) = "999"
                End If
                '---------
                ddlRicerca.Items.Add("Numero documento")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data documento")
                ddlRicerca.Items(1).Value = "D"
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(2).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(3).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(4).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(5).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(6).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(7).Value = "M"
                ddlRicerca.Items.Add("Data consegna")
                ddlRicerca.Items(8).Value = "C"
                ddlRicerca.Items.Add("Data validità")
                ddlRicerca.Items(9).Value = "V"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(10).Value = "NR"
                'GIU270617
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(11).Value = "CG"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(12).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(13).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(14).Value = "D3"
                '--
                'giu160412
                ddlCausale.Items.Clear()
                ddlCausale.Items.Add("")
                ddlCausale.DataBind()
                '---------
                'GIU210612 ddlCausale.Enabled = False
                '--
                BuidDett()
                Session(SWOP) = SWOPNESSUNA
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Movimenti di magazzino: " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        WFPResoDaCliente.WucElement = Me
        If Session(F_RESODAC_APERTA) Then
            WFPResoDaCliente.Show()
        End If
        WFPResoAFornitore.WucElement = Me
        If Session(F_RESOAF_APERTA) Then
            WFPResoAFornitore.Show()
        End If
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub

#Region " Funzioni e Procedure"
    'giu210612
    Private Sub BtnSetByTD(ByVal myTD As String, ByVal CodiceCoGe As String)
        'GIU020914 ADESSO I MOVIMENTI MM SONO RISERVATI AI MOVIMENTI DI DISTINTA BASE
        If myTD <> "MM" And myTD <> "CM" And myTD <> "SM" Then
            btnVisualizza.Visible = False
            btnSblocca.Visible = True
            btnModifica.Enabled = False
            btnElimina.Enabled = False
            btnResoClienteFornitore.Enabled = False
            btnCopia.Enabled = False
            If myTD = "DT" Then
                If Mid(CodiceCoGe, 1, 1) = "1" Then
                    btnResoClienteFornitore.Enabled = True 'RESO DA CLIENTE
                End If
            End If
        Else
            btnVisualizza.Visible = True
            btnSblocca.Visible = False
            btnResoClienteFornitore.Enabled = False
            If myTD = "CM" Then
                If Mid(CodiceCoGe, 1, 1) = "9" Then
                    btnResoClienteFornitore.Enabled = True 'RESO A FORNITORE
                End If
            End If
        End If
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnResoClienteFornitore.Enabled = Valore
        btnStampaNoLotti.Enabled = Valore
        btnCopia.Enabled = Valore
    End Sub

    Private Sub SetFilter() 'giu160412
        If rbtnTutti.Checked = False Then
            If rbtnCarichi.Checked Then
                If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "(Segno_Giacenza = '+' OR Segno_Giacenza = '=')"
            ElseIf rbtnScarichi.Checked Then
                If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "(Segno_Giacenza = '-' OR Segno_Giacenza = '=')"
            ElseIf rbtnMM.Checked Then
                If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "Tipo_Doc = '" & SWTD(TD.MovimentoMagazzino) & "'"
            ElseIf rbtnCausale.Checked Then
                If ddlCausale.SelectedIndex = 0 Then
                    'nessun filtro sulla causale, quindi tuti
                Else
                    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                        SqlDSPrevTElenco.FilterExpression += " AND "
                    End If
                    SqlDSPrevTElenco.FilterExpression += "Cod_Causale = " & ddlCausale.SelectedValue.ToString.Trim
                End If
            End If
            '---
        End If
        If IsNumeric(ddlMagazzino.SelectedValue.Trim) Then
            If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "CodiceMagazzino=" & ddlMagazzino.SelectedValue.Trim
        End If
    End Sub
#End Region

#Region " Ordinamento e ricerca"
    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu160412
        SetFilter()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu160412
        SetFilter()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
   
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
                e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            End If
        End If
    End Sub
    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaOr).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaOr).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaOr).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaOr).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaOr).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxD.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
        End If
    End Sub
    
    Private Sub BuidDett()
        'giu160412
        SetFilter()
        SqlDSPrevTElenco.DataBind()
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged

        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        'giu160412
        SetFilter()
        SqlDSPrevTElenco.DataBind()
        '---------

        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        ddlCausale.SelectedIndex = 0
        If ddlRicerca.SelectedValue <> "CG" Then 'CODICE COGE
            Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        Else
            Session(CSTSORTPREVTEL) = "R" 'PER RAGIONE SOCIALE
        End If
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "V" Or _
               ddlRicerca.SelectedValue = "C" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        SqlDSPrevTElenco.FilterExpression = ""
        If txtRicerca.Text.Trim = "" Then
            'giu160412
            SetFilter()
            SqlDSPrevTElenco.DataBind()
            '---------

            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Try
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                    Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                    BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
                Catch ex As Exception
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                    Session(CSTTIPODOC) = ""
                    Session(CSTTIPODOCSEL) = ""
                End Try
            Else
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "N" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
            Else
                SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "CG" Then 'giu270617
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "R" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "C" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "V" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "L" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "M" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "S" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "P" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "F" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "NR" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Riferimento >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D1" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Destinazione1 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Destinazione1 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D2" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Destinazione2 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Destinazione2 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D3" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Destinazione3 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Destinazione3 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        End If
        'giu160412
        SetFilter()
        SqlDSPrevTElenco.DataBind()
        '---------

        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
#End Region

#Region "Scelta tipo Movimento"
    Private Sub rbtnCarichi_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnCarichi.CheckedChanged
        Session(CSTSWRbtnMM) = "CAR"
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = "ZZ" 'SWTD(TD.CaricoMagazzino)
        Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.CaricoMagazzino)
        Session(CSTSTATODOC) = "999"
        BuidDett()
    End Sub
    'GIU110512
    Private Sub rbtnDocStato5_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDocStato5.CheckedChanged
        Session(CSTSWRbtnMM) = "CONF"
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) '"ZZ" '
        Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.CaricoMagazzino)
        'GIU110512
        Session(CSTSTATODOC) = "5" 'NOTA LO STATO DOC5 IN REALTA' E' SULL'ORDINE FORNITORE (Refint)
        'solo aggiornado uno dei carichi rifeti all'OF cambia lo stato_Doc da 5 a uno dei stati previsti
        BuidDett()
    End Sub
    Private Sub rbtnScarichi_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
       rbtnScarichi.CheckedChanged
        Session(CSTSWRbtnMM) = "SCAR"
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = "ZZ" 'SWTD(TD.ScaricoMagazzino)
        Session(CSTTIPODOCSEL) = "ZZ" 'SWTD(TD.ScaricoMagazzino)
        Session(CSTSTATODOC) = "999"
        BuidDett()
    End Sub
    Private Sub rbtnMM_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
       rbtnMM.CheckedChanged
        Session(CSTSWRbtnMM) = SWTD(TD.MovimentoMagazzino)
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino)
        Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino)
        Session(CSTSTATODOC) = "999"
        BuidDett()
    End Sub
    Private Sub rbtnCausale_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnCausale.CheckedChanged
        Session(CSTSWRbtnMM) = "CAUS"
        ddlCausale.Enabled = True : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
        Session(CSTTIPODOCSEL) = "Z"
        Session(CSTSTATODOC) = "999"
        BuidDett()
        ddlCausale.Focus()
    End Sub
    '-
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        Session(CSTSWRbtnMM) = "TUTTI"
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = "ZZ"
        Session(CSTTIPODOCSEL) = "ZZ"
        Session(CSTSTATODOC) = "999"
        BuidDett()
    End Sub
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNESSUNA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If rbtnCarichi.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.CaricoMagazzino)
        ElseIf rbtnScarichi.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.ScaricoMagazzino)
        ElseIf rbtnMM.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino)
        Else
            'GIU020914
            ' ''Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino) 'DEFAULT
            'I MM SONO RISERVATI AI MOVIMENTI DI DISTINTA BASE
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewMM"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Movimento", "Confermi la creazione del nuovo Movimento ? <br> " & strDesTipoDocumento, WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewMM()
        If rbtnCarichi.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.CaricoMagazzino)
        ElseIf rbtnScarichi.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.ScaricoMagazzino)
        Else
            Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino) 'DEFAULT
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino " & strDesTipoDocumento)
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina movimento " & strDesTipoDocumento)
    End Sub
    Private Sub btnSblocca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSblocca.Click
        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If

        If Not (sTipoUtente.Equals(CSTAMMINISTRATORE)) And _
            Not (sTipoUtente.Equals(CSTTECNICO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "SbloccaDoc"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Sblocca Documento", "Confermi lo sblocco del documento ? <br> " & strDesTipoDocumento, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub SbloccaDoc()
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        btnSblocca.Visible = False

        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If

        If (sTipoUtente.Equals(CSTTECNICO)) Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'giu230412
            ' ''If TipoDoc <> SWTD(TD.FatturaCommerciale) And TipoDoc <> SWTD(TD.FatturaAccompagnatoria) Then
            ' ''    If btnCreaFattura.Enabled = False Then btnCreaFattura.Enabled = True
            ' ''End If
            If btnModifica.Enabled = False Then btnModifica.Enabled = True
            If btnElimina.Enabled = False Then btnElimina.Enabled = True
            btnVisualizza.Visible = True : btnVisualizza.Enabled = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub
    Private Sub btnStampaNoLotti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaNoLotti.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim selezione As String = ""
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
            Session(CSTNOBACK) = 0 'giu040512
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Senza Lotti/N° Serie collegati)", selezione, dsMovMag1, Errore, "", CInt(Session(IDDOCUMENTI)), "", False, "", "", False, False) Then
                Session(CSTDsPrinWebDoc) = dsMovMag1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Private Sub btnStampaSiLotti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSiLotti.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun movimento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim selezione As String = ""
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
            Session(CSTNOBACK) = 0 'giu040512
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Lotti/N° Serie collegati)", selezione, dsMovMag1, Errore, "", CInt(Session(IDDOCUMENTI)), "", True, "", "", False, False) Then
                Session(CSTDsPrinWebDoc) = dsMovMag1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Private Sub Chiudi(ByVal strErrore As String)
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
    Private Function CKCSTTipoDocSel(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocSel = True
        TipoDoc = Session(CSTTIPODOCSEL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        strDesTipoDocumento = ""
        'DDT
        If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO CLIENTI"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO C/LAVORO"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoFornitori) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO FORNITORI"
        End If
        'fatture, NC,
        If TipoDoc = SWTD(TD.FatturaCommerciale) Then
            strDesTipoDocumento = "FATTURA COMMERCIALE"
        End If
        If TipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            strDesTipoDocumento = "FATTURA ACCOMPAGNATORIA"
        End If
        If TipoDoc = SWTD(TD.FatturaScontrino) Then
            strDesTipoDocumento = "FATTURA CON SCONTRINO"
        End If
        If TipoDoc = SWTD(TD.NotaCredito) Then
            strDesTipoDocumento = "NOTA DI CREDITO"
        End If
        If TipoDoc = SWTD(TD.NotaCorrispondenza) Then
            strDesTipoDocumento = "NOTA CORRISPONDENZA"
        End If
        If TipoDoc = SWTD(TD.BuonoConsegna) Then
            strDesTipoDocumento = "BUONO CONSEGNA"
        End If
        'Movimenti di Magazzino
        If TipoDoc = SWTD(TD.MovimentoMagazzino) Then
            strDesTipoDocumento = "MOVIMENTI DI MAGAZZINO"
        End If
        If TipoDoc = SWTD(TD.CaricoMagazzino) Then
            strDesTipoDocumento = "CARICO DI MAGAZZINO"
        End If
        If TipoDoc = SWTD(TD.ScaricoMagazzino) Then
            strDesTipoDocumento = "SCARICO DI MAGAZZINO"
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function


    Private Sub ddlCausale_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCausale.SelectedIndexChanged
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = "ZZ"
        Session(CSTTIPODOCSEL) = "ZZ"
        If ddlCausale.SelectedIndex = 0 Then
            BtnSetEnabledTo(False)
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            SqlDSPrevTElenco.FilterExpression = ""
            Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
            Session(CSTTIPODOCSEL) = "Z"
            Session(CSTSTATODOC) = "999"
            Session(IDDOCUMENTI) = ""
            BuidDett()
            Exit Sub
        End If

        SqlDSPrevTElenco.FilterExpression = "Cod_Causale = " & ddlCausale.SelectedValue.ToString.Trim
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0

            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
                BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
        BtnSetEnabledTo(True)
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Dim CodiceCoGe As String = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
            BtnSetByTD(Session(CSTTIPODOCSEL), CodiceCoGe)
        Catch ex As Exception
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End Try
    End Sub


    Private Sub btnCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopia.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        'giu120312
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            If Session(CSTTIPODOCSEL) <> "MM" And _
                           Session(CSTTIPODOCSEL) <> "CM" And _
                           Session(CSTTIPODOCSEL) <> "SM" Then
                If rbtnCarichi.Checked = True Then
                    Session(CSTTIPODOCSEL) = SWTD(TD.CaricoMagazzino)
                ElseIf rbtnScarichi.Checked = True Then
                    Session(CSTTIPODOCSEL) = SWTD(TD.ScaricoMagazzino)
                Else
                    Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino) 'DEFAULT
                End If
            End If
        Catch ex As Exception
            If rbtnCarichi.Checked = True Then
                Session(CSTTIPODOCSEL) = SWTD(TD.CaricoMagazzino)
            ElseIf rbtnScarichi.Checked = True Then
                Session(CSTTIPODOCSEL) = SWTD(TD.ScaricoMagazzino)
            Else
                Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino) 'DEFAULT
            End If
        End Try
        '---------------------------
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaCopia"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Movimento (Copia Movimento)", "Confermi la copia del Movimento selezionato ? <br><strong><span> " _
                        & "Attenzione, Il tipo Pagamento sarà aggiornato a quello in essere.<br>Si prega di verificare i dati Pagamento.</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaCopia()

        Dim StrErrore As String = ""
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO MOVIMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        StrErrore = ""
        Dim NDoc As Long = GetNewMM(StrErrore) : Dim NRev As Integer = 0
        If NDoc < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante l'impegno numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO DOCUMENTO
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDoc]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = TipoDoc
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
            myID = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO MOVIMENTO SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO MOVIMENTO SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'GIU181120 GIU191120 IMPOSTO A 'Non evadibile' cosi impongo l'aggiornamento dell'ordine appena copiato cosi se il tipo pagamento è stato cambiato aggiorna tutto
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DECLARE @CodiceCG NVARCHAR(16)=NULL, @TipoPag INT=NULL"
            SQLStr += " SELECT @CodiceCG=ISNULL(Cod_Cliente,Cod_Fornitore) FROM DocumentiT WHERE IDDocumenti=" & myID.Trim
            SQLStr += " SELECT @TipoPag=Pagamento_N FROM CliFor WHERE Codice_Coge=@CodiceCG"
            SQLStr += " IF NOT @TipoPag IS NULL"
            SQLStr += " UPDATE DocumentiT SET Cod_Pagamento=@TipoPag, Totale=0,TotNettoPagare=0 WHERE IDDocumenti=" & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Aggiornamento TIPO PAGAMENTO N° Doc.: (" & NDoc.ToString.Trim & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA

        Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino " & strDesTipoDocumento)
    End Sub
    Private Function GetNewMM(ByRef _strErrore As String) As Long
        _strErrore = ""
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewMM = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewMM = 1
                    End If
                    Exit Function
                Else
                    GetNewMM = 1
                    Exit Function
                End If
            Else
                GetNewMM = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewMM = -1
            _strErrore = "Errore Verifica N.Documento da impegnare: " & Ex.Message.Trim
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Private Sub btnStampaTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaTutti.Click
        Session(STAMPAMOVMAGTORNAAELENCO) = True
        Session(CSTNOBACK) = 0 'giu040512
        Response.Redirect("WF_StatMovMag.aspx?labelForm=Stampa movimenti di magazzino")
    End Sub

#Region "Creazione RESO"
    'giu210612
    Private Sub btnResoClienteFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResoClienteFornitore.Click
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'PER L'Eventuale riferimento
        'per sapere se possofare il RESO ho bisogno di sapere se è un CM DA FORNITORE ETC
        Dim CodiceCoGe As String = ""
        If GridViewPrevT.Rows.Count > 0 Then
            Try
                If Session(IDDOCUMENTI).ToString.Trim <> GridViewPrevT.SelectedDataKey.Value.ToString.Trim Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "IDDocumento selezionato differisce da quello in memoria. <br> " & _
                                    "riprovare a selezionare e creare il nuovo documento di reso.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                CodiceCoGe = row.Cells(CellIdxT.CodCliForProvv).Text.Trim
            Catch ex As Exception
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
            Exit Sub
        End If
        '--------------------------------------------------------------------------------
        Dim myTD As String = ""
        If CKCSTTipoDocSel(myTD) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo movimento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If myTD = SWTD(TD.DocTrasportoClienti) And Mid(CodiceCoGe, 1, 1) = "1" Then 'RESO DA CLIENTI
            Dim myErrore As String = ""
            If Documenti.BloccaDoc(CLng(Session(IDDOCUMENTI)), myErrore, myModificaDA, "", sUtente, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            WFPResoDaCliente.WucElement = Me
            WFPResoDaCliente.SetLblMessUtente("Seleziona/modifica Quantità articoli resi")
            Session(F_RESODAC_APERTA) = True
            WFPResoDaCliente.Show(True)
        ElseIf myTD = SWTD(TD.CaricoMagazzino) And Mid(CodiceCoGe, 1, 1) = "9" Then 'RESO A FORNITORI CM + Fornitore
            Dim myErrore As String = ""
            If Documenti.BloccaDoc(CLng(Session(IDDOCUMENTI)), myErrore, myModificaDA, "", sUtente, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            WFPResoAFornitore.WucElement = Me
            WFPResoAFornitore.SetLblMessUtente("Seleziona/modifica Quantità articoli da rendere")
            Session(F_RESOAF_APERTA) = True
            WFPResoAFornitore.Show(True)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Movimento di magazzino (RESO)", "Non è previsto la creazione del RESO per questo tipo di movimento.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Public Sub CancBackWFPResoDaCliente()
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'SBLOCCO IL DOCUMENTO
        Try
            Dim StrSql As String = "Update DocumentiT Set BloccatoDalPC='', Operatore=''" & _
            " Where IdDocumenti = " & Session(IDDOCUMENTI) & ""
            ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante lo sblocco del documento Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    Public Sub CallBackWFPResoDaCliente()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        ListaDocD = Session(L_RESODACF)
        If ListaDocD.Count > 0 Then
            'PROCEDO???
            Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
            Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaResoDaCliente"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Movimento di magazzino", "Confermi la creazione del RESO DA CLIENTE ?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Movimento di magazzino", "Nessun articolo è stato selezionato.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Public Sub OKCreaResoDaCliente()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        ListaDocD = Session(L_RESODACF)
        If ListaDocD.Count > 0 Then
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            Dim StrSql As String
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Selezionata = " & IIf(QtaDaEv <> 0, QtaDaEv.ToString.Replace(",", "."), -1) & _
                    " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next

            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            'giu200617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio.CommandTimeout = myTimeOUT
            '---------------------------
            Dim SqlDbNewCmd As New SqlCommand
            Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) 'serve a GetNewMM per prendere il primo numero libero
            StrErrore = ""
            Dim NDoc As Long = GetNewMM(StrErrore)
            Dim NRev As Integer = 0
            If NDoc < 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'impegno numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocMMResi]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
            '--
            SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
            SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
            SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            Try
                SqlConn.Open()
                SqlDbNewCmd.CommandTimeout = myTimeOUT
                SqlDbNewCmd.ExecuteNonQuery()
                SqlConn.Close()
                Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            Catch ExSQL As SqlException
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try

        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino)
        Response.Redirect("WF_Documenti.aspx?labelForm=Carico di magazzino RESO DA CLIENTE")
    End Sub

    'RESO A FORNITORE
    Public Sub CallBackWFPResoAFornitore()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        ListaDocD = Session(L_RESODACF)
        If ListaDocD.Count > 0 Then
            'PROCEDO???
            Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
            Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaResoAFornitoreSM"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "OKCreaResoAFornitoreDF"
            ModalPopup.Show("Crea nuovo Movimento di magazzino", "Confermi la creazione del RESO A FORNITORE ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Movimento di magazzino", "Nessun articolo è stato selezionato.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Public Sub CancBackWFPResoAFornitore()
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'SBLOCCO IL DOCUMENTO
        Try
            Dim StrSql As String = "Update DocumentiT Set BloccatoDalPC='', Operatore=''" & _
            " Where IdDocumenti = " & Session(IDDOCUMENTI) & ""
            ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante lo sblocco del documento Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    Public Sub OKCreaResoAFornitoreSM()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        ListaDocD = Session(L_RESODACF)
        If ListaDocD.Count > 0 Then
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            Dim StrSql As String
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Selezionata = " & IIf(QtaDaEv <> 0, QtaDaEv.ToString.Replace(",", "."), -1) & _
                    " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next

            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            'giu200617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio.CommandTimeout = myTimeOUT
            '---------------------------
            Dim SqlDbNewCmd As New SqlCommand
            Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) 'serve a GetNewMM per prendere il primo numero libero
            StrErrore = ""
            Dim NDoc As Long = GetNewMM(StrErrore)
            If NDoc < 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'impegno numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            Dim NRev As Integer = 0
            SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocMMResi]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
            '--
            SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
            SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.ScaricoMagazzino)
            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
            SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            Try
                SqlConn.Open()
                SqlDbNewCmd.CommandTimeout = myTimeOUT
                SqlDbNewCmd.ExecuteNonQuery()
                SqlConn.Close()
                Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            Catch ExSQL As SqlException
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try

        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino)
        Response.Redirect("WF_Documenti.aspx?labelForm=Scarico di magazzino RESO A FORNITORE")
    End Sub
    Public Sub OKCreaResoAFornitoreDF()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        ListaDocD = Session(L_RESODACF)
        If ListaDocD.Count > 0 Then
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            Dim StrSql As String
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Selezionata = " & IIf(QtaDaEv <> 0, QtaDaEv.ToString.Replace(",", "."), -1) & _
                    " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next

            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            'giu200617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio.CommandTimeout = myTimeOUT
            '---------------------------
            Dim SqlDbNewCmd As New SqlCommand
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori)
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) 'serve a GetNewNumDoc per prendere il primo numero libero
            StrErrore = "" : Dim myCodCausale As Integer = 0
            Dim NDoc As Long = GetNewNumDoc(myCodCausale, StrErrore)
            If NDoc < 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'impegno numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            Dim NRev As Integer = 0
            If AggiornaNumDoc(NDoc, NRev, StrErrore) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'aggiornamento numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If myCodCausale = 0 Then myCodCausale = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
            SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocMMResi]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
            '--
            SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
            SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.DocTrasportoFornitori)
            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
            SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = myCodCausale
            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            Try
                SqlConn.Open()
                SqlDbNewCmd.CommandTimeout = myTimeOUT
                SqlDbNewCmd.ExecuteNonQuery()
                SqlConn.Close()
                Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            Catch ExSQL As SqlException
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try

        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori)
        Response.Redirect("WF_Documenti.aspx?labelForm=Scarico di magazzino RESO A FORNITORE")
    End Sub
    Private Function GetNewNumDoc(ByRef _CodCausale As Integer, ByRef strErrore As String) As Integer
        GetNewNumDoc = -1
        If CKCSTTipoDocSel(TipoDoc, TabCliFor) = False Then
            GetNewNumDoc = False
            strErrore = "Errore: TIPO DOCUMENTO SCONOSCIUTO (GetNewNumDoc)"
            Exit Function
        End If
        strErrore = ""
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPreventivo + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroRiordinoFornitore + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodiceCausaleRiordino
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineCliente + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodiceCausaleRiordino
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
                'giu110814 qui non verranno mai creati FC,NC e simile quindi OK no FatturaPA
                ' ''ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                ' ''    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                ' ''    _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
                ' ''ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
                ' ''    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFA + 1
                ' ''    _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
                ' ''ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                ' ''    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                ' ''    _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
                ' ''ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                ' ''    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = True Then
                ' ''        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                ' ''    Else
                ' ''        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                ' ''    End If
                ' ''    _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CausNCResi
                ' ''ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                ' ''    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaCdenza + 1
                ' ''    _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroBC + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Then
                GetNewNumDoc = GetNewMM(strErrore)
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            ElseIf Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Then
                GetNewNumDoc = GetNewMM(strErrore)
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            ElseIf Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                GetNewNumDoc = GetNewMM(strErrore)
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
            Else
                GetNewNumDoc = -1
                strErrore = "Errore: Tipo documento non gestito: (" & Session(CSTTIPODOC).ToString & ")"
                Exit Function
            End If
        Else
            GetNewNumDoc = -1
            strErrore = "Errore: Caricamento parametri generali. " & strErrore
            Exit Function
        End If
    End Function
    Private Function AggiornaNumDoc(ByVal _NDoc As Long, ByVal _NRev As Integer, ByRef strErrore As String) As Boolean
        strErrore = ""
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbUpdCmd As New SqlCommand

        SqlDbUpdCmd.CommandText = "[Update_NDocPargen]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWOp", System.Data.SqlDbType.VarChar, 1))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '-- qui sempre nuovo documento
        SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N"
        '-- al trimenti per modifica sarebbe
        ' ''SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMENTI)
        ' ''SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "M"
        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = Session(CSTTIPODOC)
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = _NDoc.ToString.Trim
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = _NRev
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                strErrore = "N° Documento risulta già impegnato(-1)!!!"
                AggiornaNumDoc = False
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                strErrore = "Errore nell'aggiornamento del N° Documento(-2)!!!"
                AggiornaNumDoc = False
            End If
        Catch ExSQL As SqlException
            strErrore = "Errore nell'aggiornamento del N° Documento(SQL)!!!: <br><br> " & ExSQL.Message
            AggiornaNumDoc = False
        Catch Ex As Exception
            strErrore = "Errore nell'aggiornamento del N° Documento(Ex)!!!: <br><br> " & Ex.Message
            AggiornaNumDoc = False
        End Try
    End Function
#End Region

    Public Sub CancBackWFPDocCollegati()
        Session(CSTSWRbtnMM) = SWTD(TD.MovimentoMagazzino)
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino)
        Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino)
        Session(CSTSTATODOC) = "999"
    End Sub
    Public Sub CallBackWFPDocCollegati()
        'Dim myID As String = Session(IDDOCUMENTI)
        'If IsNothing(myID) Then
        '    myID = ""
        'End If
        'If String.IsNullOrEmpty(myID) Then
        '    myID = ""
        'End If
        'If Not IsNumeric(myID) Then
        '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        '    Exit Sub
        'End If
        Session(CSTSWRbtnMM) = SWTD(TD.MovimentoMagazzino)
        ddlCausale.Enabled = False : ddlCausale.SelectedIndex = 0
        Session(CSTSORTPREVTEL) = "D"
        Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino)
        Session(CSTTIPODOCSEL) = SWTD(TD.MovimentoMagazzino)
        Session(CSTSTATODOC) = "999"
    End Sub
    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegati()
        ' ''WFPDocCollegati.WucElement = Me
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        BuidDett()
    End Sub
End Class