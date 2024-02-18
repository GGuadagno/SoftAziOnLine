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


Partial Public Class WUC_ProvvigioniFatture
    Inherits System.Web.UI.UserControl



    'alberto 22/03/2012 spostato nell'evento click del pulsante aggiorna
    'Dim SqlCmdUpdateDocumentiDProvv As New SqlCommand
    Dim SqlConnUpd As New SqlConnection

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""

    Private Enum CellIdxT
        NumDoc = 1
        TipoDC = 2
        DataDoc = 3
        'Stato = 2
        CodCliForProvv = 4
        RagSoc = 5
        'DataCons = 12
        'DataVal = 13
        'Riferimento = 14
    End Enum
    Private Enum CellIdxD
        CodArt = 1
        DesArt = 2
        UM = 3
        QtaEv = 4
        IVA = 5
        Prz = 6
        ScV = 7
        Sc1 = 8
        Importo = 9
        ScR = 10
        Cod_Agente = 11
        DescrizioneAgente = 11
        Provv = 13
        ImportoProvv = 14
        Riga = 15
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
        SqlDSAgenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlConnUpd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        'alberto 22/03/2012 spostato nell'evento click del pulsante aggiorna
        'SqlCmdUpdateDocumentiDProvv.Connection = SqlConnUpd
        'SqlCmdUpdateDocumentiDProvv.CommandType = CommandType.StoredProcedure
        'SqlCmdUpdateDocumentiDProvv.CommandText = "Update_DocumentiDProvv"
        'SqlCmdUpdateDocumentiDProvv.Parameters.Add("@IDDocumenti", SqlDbType.Int, 4)
        'SqlCmdUpdateDocumentiDProvv.Parameters("@IDDocumenti").Direction = ParameterDirection.Input
        'SqlCmdUpdateDocumentiDProvv.Parameters.Add("@Riga", SqlDbType.Int, 4)
        'SqlCmdUpdateDocumentiDProvv.Parameters("@Riga").Direction = ParameterDirection.Input
        'SqlCmdUpdateDocumentiDProvv.Parameters.Add("@Provv", SqlDbType.Decimal)
        'SqlCmdUpdateDocumentiDProvv.Parameters("@Provv").Direction = ParameterDirection.Input

        If (Not IsPostBack) Then
            Try
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
                Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)

                ddlAgenti.Enabled = False
                Session(CSTSTATODOC) = "999"
                Session(CSTSORTPREVTEL) = "N"

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
                'ddlRicerca.Items.Add("Data consegna")
                'ddlRicerca.Items(8).Value = "C"
                'ddlRicerca.Items.Add("Data validità")
                'ddlRicerca.Items(9).Value = "V"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(8).Value = "NR"
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(9).Value = "CG"

                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    GridViewPrevT.SelectedIndex = 0
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    Try
                        btnModifica.Enabled = False
                        txtProvv.Enabled = False
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                        BtnSetEnabledTo(True)
                        txtProvv.Text = ""
                    Catch ex As Exception
                    End Try
                Else
                    BtnSetEnabledTo(False)
                    Session(IDDOCUMENTI) = ""
                    Session(CSTTIPODOC) = ""
                    Session(CSTTIPODOCSEL) = ""
                End If

                rbtnTuttiAgenti.Checked = True
                ddlAgenti.Enabled = False

                Session(SWOP) = SWOPNESSUNA

                ddlAgenti.Items.Clear()
                ddlAgenti.Items.Add("")
                ddlAgenti.DataBind()
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Documenti: " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
    End Sub

#Region " Funzioni e Procedure"
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        'btnCambiaStato.Enabled = Valore
        'btnNuovo.Enabled = Valore
        'btnModifica.Enabled = Valore
        'btnElimina.Enabled = Valore
        'btnCreaFattura.Enabled = Valore
        'btnStampaEti.Enabled = Valore
        'btnListaCarico.Enabled = Valore
        'btnStampa.Enabled = Valore
        'btnCopia.Enabled = Valore
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
        GridViewPrevT.SelectedIndex = 0
        Session(IDDOCUMENTI) = ""
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            'If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
            'e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            'End If
            'If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
            '    e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            'End If
        End If
    End Sub
    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
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

            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = 3 Then
                If IsNumeric(e.Row.Cells(CellIdxD.Provv).Text) Then
                    If CDec(e.Row.Cells(CellIdxD.Provv).Text) <> 0 Then
                        txtProvv.Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Provv).Text), 2).ToString
                    Else
                        txtProvv.Text = ""
                    End If
                End If
                If txtProvv.Text <> "" Then
                    btnModifica.Enabled = True
                Else
                    btnModifica.Enabled = False
                    txtProvv.Enabled = False
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Provv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Provv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Provv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Provv).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Provv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ImportoProvv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ImportoProvv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ImportoProvv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ImportoProvv).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ImportoProvv).Text = ""
                End If
            End If
        End If
    End Sub
    Private Sub BuidDett()
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                btnModifica.Enabled = False
                txtProvv.Enabled = False
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                BtnSetEnabledTo(True)
                txtProvv.Text = ""
            Catch ex As Exception
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged

        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""

        'GIU210312
        If ddlRicerca.SelectedValue <> "CG" Then 'CODICE COGE
            Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        Else
            Session(CSTSORTPREVTEL) = "R" 'PER RAGIONE SOCIALE
        End If
        '---------
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Catch ex As Exception
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        ddlAgenti.SelectedIndex = 0
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
        If txtRicerca.Text.Trim = "" Then
            SqlDSPrevTElenco.FilterExpression = ""
            SqlDSPrevTElenco.DataBind()
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Try
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Catch ex As Exception
                End Try
            Else
                BtnSetEnabledTo(False)
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
        ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU210312
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
        End If
        SqlDSPrevTElenco.DataBind()

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Catch ex As Exception
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
#End Region

#Region "Scelta tipo documenti"
 

    Private Sub rbtnAgente_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnAgente.CheckedChanged
        If rbtnAgente.Checked = True Then
            ddlAgenti.Enabled = True
        Else
            ddlAgenti.Enabled = False
        End If
        ddlAgenti.SelectedIndex = -1
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTSTATODOC) = "999"
        BuidDett()
        ddlAgenti.Focus()
    End Sub
    
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTuttiAgenti.CheckedChanged

        If rbtnTuttiAgenti.Checked Then
            ddlAgenti.Enabled = False
            ddlAgenti.SelectedIndex = -1
            Session(CSTSORTPREVTEL) = "N"
            Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
            Session(CSTSTATODOC) = "999"
            SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
            BuidDett()
        End If
    End Sub
#End Region

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

    Private Sub ddlAgenti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgenti.SelectedIndexChanged
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTSTATODOC) = "999"
        If ddlAgenti.SelectedIndex = 0 Then
            ddlAgenti.SelectedIndex = -1
            BuidDett()
            Exit Sub
        End If

        SqlDSPrevTElenco.FilterExpression = "Cod_Agente = " & ddlAgenti.SelectedValue.ToString.Trim
        SqlDSPrevTElenco.DataBind()
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Catch ex As Exception
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOC) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
        Try
            btnModifica.Enabled = False
            txtProvv.Enabled = False
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            BtnSetEnabledTo(True)
            txtProvv.Text = ""
        Catch ex As Exception
        End Try
    End Sub


    Private Sub GridViewPrevD_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevD.SelectedIndexChanged
        Try
            Dim row As GridViewRow = GridViewPrevD.SelectedRow
            btnModifica.Enabled = True
            txtProvv.Text = CInt(row.Cells(CellIdxD.Provv).Text.Trim)
        Catch ex As Exception
            btnModifica.Enabled = False
            txtProvv.Enabled = False
        End Try
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
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
        btnModifica.Enabled = False
        btnAggiornaProvv.Enabled = True
        btnAnnulla.Enabled = True
        txtProvv.Enabled = True
        btnCambiaAgente.Enabled = False

        rbtnAgente.Enabled = False
        rbtnTuttiAgenti.Enabled = False
        ddlAgenti.Enabled = False
        ddlRicerca.Enabled = False
        checkParoleContenute.Enabled = False
        txtRicerca.Enabled = False

        GridViewPrevT.Enabled = False
        GridViewPrevD.Enabled = False

        btnRicerca.Enabled = False

        txtProvv.Focus()
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        btnModifica.Enabled = True
        btnAggiornaProvv.Enabled = False
        btnAnnulla.Enabled = False
        txtProvv.Enabled = False
        btnCambiaAgente.Enabled = True

        rbtnAgente.Enabled = True
        rbtnTuttiAgenti.Enabled = True
        If rbtnAgente.Checked Then ddlAgenti.Enabled = True

        ddlRicerca.Enabled = True
        checkParoleContenute.Enabled = True
        txtRicerca.Enabled = True

        GridViewPrevT.Enabled = True
        GridViewPrevD.Enabled = True

        btnRicerca.Enabled = True
        BuidDett()
    End Sub

    Private Sub btnAggiornaProvv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiornaProvv.Click
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
        Dim SqlCmdUpdateDocumentiDProvv As New SqlCommand

        If txtProvv.Text.Trim.Length <= 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Percentuale provvigione obbligatoria.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If Not IsNumeric(txtProvv.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Percentuale provvigione deve essere numerica.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If


        Try
            Dim row As GridViewRow = GridViewPrevD.SelectedRow

            SqlCmdUpdateDocumentiDProvv.Connection = SqlConnUpd
            SqlCmdUpdateDocumentiDProvv.CommandType = CommandType.StoredProcedure
            SqlCmdUpdateDocumentiDProvv.CommandText = "Update_DocumentiDProvv"
            SqlCmdUpdateDocumentiDProvv.Parameters.Add("@IDDocumenti", SqlDbType.Int, 4)
            SqlCmdUpdateDocumentiDProvv.Parameters("@IDDocumenti").Direction = ParameterDirection.Input
            SqlCmdUpdateDocumentiDProvv.Parameters.Add("@Riga", SqlDbType.Int, 4)
            SqlCmdUpdateDocumentiDProvv.Parameters("@Riga").Direction = ParameterDirection.Input
            SqlCmdUpdateDocumentiDProvv.Parameters.Add("@Provv", SqlDbType.Decimal)
            SqlCmdUpdateDocumentiDProvv.Parameters("@Provv").Direction = ParameterDirection.Input

            SqlCmdUpdateDocumentiDProvv.Parameters("@IDDocumenti").Value = CLng(myID)
            SqlCmdUpdateDocumentiDProvv.Parameters("@Riga").Value = GridViewPrevD.SelectedDataKey.Value 'CInt(row.Cells(CellIdxD.Riga).Text.Trim)
            SqlCmdUpdateDocumentiDProvv.Parameters("@Provv").Value = CInt(txtProvv.Text.Trim)

            If SqlCmdUpdateDocumentiDProvv.Connection.State <> ConnectionState.Open Then
                SqlCmdUpdateDocumentiDProvv.Connection.Open()
            End If
            SqlCmdUpdateDocumentiDProvv.ExecuteNonQuery()
            SqlCmdUpdateDocumentiDProvv.Connection.Close()

            btnModifica.Enabled = True
            btnAggiornaProvv.Enabled = False
            btnAnnulla.Enabled = False
            txtProvv.Enabled = False
            btnCambiaAgente.Enabled = True

            rbtnAgente.Enabled = True
            rbtnTuttiAgenti.Enabled = True
            If rbtnAgente.Checked Then ddlAgenti.Enabled = True
            ddlRicerca.Enabled = True
            checkParoleContenute.Enabled = True
            txtRicerca.Enabled = True

            GridViewPrevT.Enabled = True
            GridViewPrevD.Enabled = True
            btnRicerca.Enabled = True
            BuidDett()
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
       

    End Sub
End Class