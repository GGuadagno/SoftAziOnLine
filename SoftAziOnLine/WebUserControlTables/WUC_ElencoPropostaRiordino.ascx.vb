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

Partial Public Class WUC_ElencoPropostaRiordino
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""

    Private Enum CellIdxT
        ' ''Stato = 1
        NumDoc = 1
        ' ''RevN = 2
        DataDoc = 2
        CodCliForProvv = 3
        RagSoc = 4
        DataCons = 10
        DataVal = 11
        Riferimento = 12
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        IVA = 4
        Prz = 5
        ScV = 6
        Sc1 = 7
        Importo = 8
        ScR = 9
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
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        '' ''giu15112011 da ATTIVARE
        ' ''btnCambiaStato.Visible = False
        '' ''-----------------------
        btnCreaOrdine.Text = "Crea ordine Fornitore"
        If (Not IsPostBack) Then
            Try
                ' ''rbtnDaConfermare.Checked = True
                Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori)
                Session(CSTSTATODOC) = "999"
                Session(CSTSORTPREVTEL) = "N"

                ddlRicerca.Items.Add("Numero Proposta riordino")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data Proposta riordino")
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

                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    GridViewPrevT.SelectedIndex = 0
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    ' ''Try
                    ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                    ' ''    btnCreaOrdine.Enabled = True
                    ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
                    ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
                    ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
                    ' ''Catch ex As Exception
                    ' ''    btnCreaOrdine.Enabled = True
                    ' ''End Try
                Else
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End If

                Session(SWOP) = SWOPNESSUNA
            Catch ex As Exception
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                Chiudi("Errore: Caricamento Elenco Proposta riordino: " & ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        ' ''btnCambiaStato.Enabled = Valore
        btnVisualizza.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaOrdine.Enabled = Valore
        ' ''btnStampa.Enabled = Valore
        ' ''btnCopia.Enabled = Valore
        ' ''btnNuovaRev.Enabled = Valore
    End Sub

#Region " Ordinamento e ricerca"
    ' ''Private Sub rbtnDaConfermare_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    ' ''    rbtnDaConfermare.CheckedChanged
    ' ''    btnCreaOrdine.Enabled = True
    ' ''    Session(CSTSTATODOC) = "0"
    ' ''    BuidDett()
    ' ''End Sub
    ' ''Private Sub rbtnConfermati_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    ' ''    rbtnConfermati.CheckedChanged
    ' ''    btnCreaOrdine.Enabled = False
    ' ''    Session(CSTSTATODOC) = "1"
    ' ''    BuidDett()
    ' ''End Sub
    ' ''Private Sub rbtnChiusoNoConf_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    ' ''    rbtnChiusoNoConf.CheckedChanged
    ' ''    btnCreaOrdine.Enabled = False
    ' ''    Session(CSTSTATODOC) = "3"
    ' ''    BuidDett()
    ' ''End Sub
    ' ''Private Sub rbtnNonConferm_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    ' ''    rbtnNonConferm.CheckedChanged
    ' ''    btnCreaOrdine.Enabled = False
    ' ''    Session(CSTSTATODOC) = "4"
    ' ''    BuidDett()
    ' ''End Sub
    ' ''Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    ' ''    rbtnTutti.CheckedChanged
    ' ''    btnCreaOrdine.Enabled = True
    ' ''    Session(CSTSTATODOC) = "999"
    ' ''    BuidDett()
    ' ''End Sub
    Private Sub BuidDett()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            ' ''Try
            ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
            ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            ' ''    btnCreaOrdine.Enabled = True
            ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
            ' ''Catch ex As Exception
            ' ''    btnCreaOrdine.Enabled = True
            ' ''End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged

        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            ' ''Try
            ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
            ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            ' ''    btnCreaOrdine.Enabled = True
            ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
            ' ''Catch ex As Exception
            ' ''    btnCreaOrdine.Enabled = True
            ' ''End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
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

            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                ' ''Try
                ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''    btnCreaOrdine.Enabled = True
                ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
                ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
                ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
                ' ''Catch ex As Exception
                ' ''    btnCreaOrdine.Enabled = True
                ' ''End Try
            Else
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
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

        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            ' ''Try
            ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
            ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            ' ''    btnCreaOrdine.Enabled = True
            ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
            ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
            ' ''Catch ex As Exception
            ' ''    btnCreaOrdine.Enabled = True
            ' ''End Try
        Else
            BtnSetEnabledTo(False)
            ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        Session(SWOP) = SWOPNESSUNA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Proposta di riordino")
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Proposta di riordino")
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewPR"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Proposta di riordino", "Confermi la creazione della nuova Proposta di riordino ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewPR()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Proposta di riordino")
    End Sub

    Private Sub btnCreaOrdine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaOrdine.Click
        'EVASIONE CLASSICA Response.Redirect("WF_EvadiDocumenti.aspx?labelForm=Crea Ordine")
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaOrdine"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea Ordine Fornitore (Conferma Proposta di riordino)", "Confermi la creazione dell'Ordine Fornitore da Proposta di riordino ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaOrdine()
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim IDPF As String = Session(IDDOCUMENTI)
        If IsNothing(IDPF) Then
            IDPF = ""
        End If
        If String.IsNullOrEmpty(IDPF) Then
            IDPF = ""
        End If
        If Not IsNumeric(IDPF) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.OrdFornitori), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO ORDINE
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
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(IDPF)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdFornitori)
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        'GIU030212 CANCELLO LA PROPOSTA DI RIORDINO
        Dim SWOk As Boolean = True
        Dim SQLStrL As String = "" : Dim SQLStrD As String = "" : Dim SQLStrT As String = ""
        SQLStrL = "DELETE FROM [DocumentiDLotti] WHERE ([IDDocumenti] = " & IDPF & ")"
        SQLStrD = "DELETE FROM [DocumentiD] WHERE ([IDDocumenti] = " & IDPF & ")"
        SQLStrT = "DELETE FROM [DocumentiT] WHERE ([IDDocumenti] = " & IDPF & ")"
        Dim ObjDB As New DataBaseUtility
        Try
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStrL)
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStrD)
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStrT)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
        End Try
        ObjDB = Nothing
        'OK FATTO
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        'ModalPopup.Show("Crea Ordine", "Creazione Ordine avvenuta con sussesso. <br> Numero: " & NDoc.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
        Session(SWOP) = SWOPMODIFICA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Ordini Fornitori")
    End Sub

    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer) As Boolean
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        Dim strErrore As String = ""
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
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N"

        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = TDoc
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO IDENTIFICATIVO DOCUMENTO GIA' PRESENTE.", WUC_ModalPopup.TYPE_ERROR)
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ExSQL As SqlException
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
        Catch Ex As Exception
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Function

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina Proposta di riordino")
    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value

        ' ''Try
        ' ''    Dim row As GridViewRow = GridViewPrevT.SelectedRow
        ' ''    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
        ' ''    btnCreaOrdine.Enabled = True
        ' ''    If Stato.Trim = "Confermato" Then btnCreaOrdine.Enabled = False
        ' ''    If Stato.Trim = "Chiuso non confermato" Then btnCreaOrdine.Enabled = False
        ' ''    If Stato.Trim = "Non confermabile" Then btnCreaOrdine.Enabled = False
        ' ''Catch ex As Exception
        ' ''    btnCreaOrdine.Enabled = True
        ' ''End Try
    End Sub
    
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
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
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
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
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub


    Public Sub Chiudi(ByVal strErrore As String)
      
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale " & strErrore)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
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



    
End Class