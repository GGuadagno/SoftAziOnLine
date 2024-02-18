Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

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

Partial Public Class WUC_ElencoDistinteSped
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    '---------
    Private SWFatturaPA As Boolean = False 'giu080814
    Private SWSplitIVA As Boolean = False 'giu221217
    Const IDSPEDIZIONE As String = "IDSpedizione"
    Const NumSPEDIZIONE As String = "NumSpedizione"
    Const STATOSPED As String = "StatoSped"

    Private Enum CellIdxT
        NumSped = 1
        DataSped = 2
        NOrdine = 3
        IdDocumenti = 4
        StatoSped = 5
        Colli = 6
        Peso = 7
        Pezzi = 8
    End Enum
    Private Enum CellIdxD
        OrdineN = 0
        TipoDoc = 1
        Loc = 2
        CodArt = 3
        DesArt = 4
        QtaAllestita = 5
        ' ''QtaImpegnata = 6
        ' ''QtaResidua = 7
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni"
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
        SqlDSSpedTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSSpedDByIdSpedizione.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        btnNuovoBC.Visible = False
        '-----------------------
        If (Not IsPostBack) Then
            SetEnableTxt(False)
            Try
                'giu200722
                Dim strValore As String = "" : Dim strErrore As String = ""
                Call GetDatiAbilitazioni(CSTABILAZI, "SpedVettCsv", strValore, strErrore)
                If strErrore.Trim <> "" Then
                    strValore = ""
                End If
                Session("SpedVettCsv") = strValore
                '----------
                'DA FARE SE .....
                btnEvadi.Text = "All./Evadi spedizione"
                btnNuovoBC.Text = "Nuovo BC (Neg)"
                btnNuovoBC.Visible = False
                btnEtiSing.Text = "Etichetta singola"
                btnEtiSped.Text = "Etichette Sovracollo"
                btnChiudiAll.Text = "Chiudi allestimento"
                btnPrintBroglALL.Text = "Stampa tutte le liste di carico"
                btnPrintBrogl.Text = "Stampa singola lista di carico"

                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOC) = ""
                Session(STATOSPED) = StatoSpedizione.Allestimento
                rbtnAllestimento.Checked = True

                If GridViewSped.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    GridViewSped.SelectedIndex = 0
                    Session(IDSPEDIZIONE) = GridViewSped.SelectedDataKey.Value
                    Try
                        Dim row As GridViewRow = GridViewSped.SelectedRow
                        Session(NumSPEDIZIONE) = row.Cells(CellIdxT.NumSped).Text.Trim
                        Session(IDDOCUMENTI) = row.Cells(CellIdxT.IdDocumenti).Text.Trim
                        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
                        LblNumSped.Text = row.Cells(CellIdxT.NumSped).Text.Trim
                        LblDataSped.Text = row.Cells(CellIdxT.DataSped).Text.Trim
                        Call PopolaTXTDocumento(Session(IDDOCUMENTI))

                        GridViewSpedDett.Enabled = True
                        GridViewSped.Enabled = True
                        BtnModAgg.Enabled = True
                        btnModifica.Enabled = True
                        btnCreaDDT.Enabled = True
                        btnEtiSped.Enabled = True
                        btnPrintBrogl.Enabled = True
                        btnPrintBroglALL.Enabled = True

                        ' ''Dim Stato As String = row.Cells(CellIdxT.StatoSped).Text.Trim
                        ' ''If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
                        ' ''If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
                        ' ''If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
                    Catch ex As Exception
                    End Try
                Else
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                    Session(NumSPEDIZIONE) = ""
                    LblNumSped.Text = ""
                    LblDataSped.Text = ""
                End If

                Session(SWOP) = SWOPNESSUNA
            Catch ex As Exception
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                Chiudi("Errore: Caricamento Elenco spedizioni: " & ex.Message)
                Exit Sub
            End Try

        End If

        ModalPopup.WucElement = Me
        'giu090112 PIERANGELO LU STRUNZ CHE NON SA COPIARE
        WFPVettori.WucElement = Me
        If Session(F_ANAGRVETTORI_APERTA) = True Then
            WFPVettori.Show()
        End If
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnEvadi.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        BtnModAgg.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaDDT.Enabled = Valore
        btnPrintBrogl.Enabled = Valore
        btnPrintBroglALL.Enabled = Valore
        btnEtiSing.Enabled = Valore
        btnEtiSped.Enabled = Valore
        btnChiudiAll.Enabled = Valore
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnPrintBrogAlle_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnPrintBrogAlle.CheckedChanged
        btnCreaDDT.Enabled = True
        Session(STATOSPED) = StatoSpedizione.StampatoBrogl
        BuidDett()
    End Sub
    Private Sub rbtnAllestimento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnAllestimento.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.Allestimento
        BuidDett()
    End Sub
    Private Sub rbtnInAllestimento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInAllestimento.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.InAllestimento
        BuidDett()
    End Sub
    Private Sub rbtnParzAllestite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnParzAllestite.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.ParzAllestito
        BuidDett()
    End Sub
    Private Sub rbtnProntePerBrog_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnProntePerBrog.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.ProntoPerStamp
        BuidDett()
    End Sub
    Private Sub rbtnAllestite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnAllestite.CheckedChanged
        btnCreaDDT.Enabled = True
        Session(STATOSPED) = StatoSpedizione.Allestite
        BuidDett()
    End Sub
    Private Sub rbtnBloccate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnBloccate.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.Bloccato
        BuidDett()
    End Sub
    Private Sub rbtnChiuse_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiuse.CheckedChanged
        btnCreaDDT.Enabled = False
        Session(STATOSPED) = StatoSpedizione.Chiuso
        BuidDett()
    End Sub
    Private Sub BuidDett()
        GridViewSped.DataBind()
        If GridViewSped.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewSped.SelectedIndex = 0
            Session(IDSPEDIZIONE) = GridViewSped.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewSped.SelectedRow
                Session(NumSPEDIZIONE) = row.Cells(CellIdxT.NumSped).Text.Trim
                LblNumSped.Text = row.Cells(CellIdxT.NumSped).Text.Trim
                LblDataSped.Text = row.Cells(CellIdxT.DataSped).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.StatoSped).Text.Trim
                btnCreaDDT.Enabled = True
                If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
                If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
                If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
            Catch ex As Exception
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDSPEDIZIONE) = ""
            LblNumSped.Text = ""
            LblDataSped.Text = ""
        End If
    End Sub
    ''Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged

    ''    SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
    ''    Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
    ''    SqlDSPrevTElenco.DataBind()

    ''    BtnSetEnabledTo(False)
    ''    ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
    ''    btnNuovo.Enabled = True
    ''    Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    ''    GridViewSped.DataBind()
    ''    If GridViewSped.Rows.Count > 0 Then
    ''        BtnSetEnabledTo(True)
    ''        GridViewSped.SelectedIndex = 0
    ''        Session(IDDOCUMENTI) = GridViewSped.SelectedDataKey.Value
    ''        Try
    ''            Dim row As GridViewRow = GridViewSped.SelectedRow
    ''            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
    ''            btnCreaDDT.Enabled = True
    ''            If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
    ''            If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
    ''            If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
    ''        Catch ex As Exception
    ''            btnCreaDDT.Enabled = True
    ''        End Try
    ''    Else
    ''        BtnSetEnabledTo(False)
    ''        btnNuovo.Enabled = True
    ''        Session(IDDOCUMENTI) = ""
    ''    End If

    ''End Sub

    ''Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
    ''    If ddlRicerca.SelectedValue = "N" Then
    ''        If Not IsNumeric(txtRicerca.Text.Trim) Then
    ''            txtRicerca.Text = ""
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "D" Or _
    ''           ddlRicerca.SelectedValue = "V" Or _
    ''           ddlRicerca.SelectedValue = "C" Then
    ''        If Not IsDate(txtRicerca.Text.Trim) Then
    ''            txtRicerca.Text = ""
    ''        End If
    ''    End If
    ''    If txtRicerca.Text.Trim = "" Then
    ''        SqlDSPrevTElenco.FilterExpression = ""
    ''        SqlDSPrevTElenco.DataBind()

    ''        BtnSetEnabledTo(False)
    ''        btnNuovo.Enabled = True

    ''        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    ''        GridViewSped.DataBind()
    ''        If GridViewSped.Rows.Count > 0 Then
    ''            BtnSetEnabledTo(True)
    ''            GridViewSped.SelectedIndex = 0
    ''            Session(IDDOCUMENTI) = GridViewSped.SelectedDataKey.Value
    ''            Try
    ''                Dim row As GridViewRow = GridViewSped.SelectedRow
    ''                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
    ''                btnCreaDDT.Enabled = True
    ''                If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
    ''                If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
    ''                If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
    ''            Catch ex As Exception
    ''                btnCreaDDT.Enabled = True
    ''            End Try
    ''        Else
    ''            BtnSetEnabledTo(False)
    ''            btnNuovo.Enabled = True
    ''            Session(IDDOCUMENTI) = ""
    ''        End If
    ''        Exit Sub
    ''    End If
    ''    If ddlRicerca.SelectedValue = "N" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "D" Then
    ''        If IsDate(txtRicerca.Text.Trim) Then
    ''            txtRicerca.BackColor = SEGNALA_OK
    ''            If checkParoleContenute.Checked = True Then
    ''                SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            Else
    ''                SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            End If
    ''        Else
    ''            txtRicerca.BackColor = SEGNALA_KO
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "R" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "C" Then
    ''        If IsDate(txtRicerca.Text.Trim) Then
    ''            txtRicerca.BackColor = SEGNALA_OK
    ''            If checkParoleContenute.Checked = True Then
    ''                SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            Else
    ''                SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            End If
    ''        Else
    ''            txtRicerca.BackColor = SEGNALA_KO
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "V" Then
    ''        If IsDate(txtRicerca.Text.Trim) Then
    ''            txtRicerca.BackColor = SEGNALA_OK
    ''            If checkParoleContenute.Checked = True Then
    ''                SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            Else
    ''                SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
    ''            End If
    ''        Else
    ''            txtRicerca.BackColor = SEGNALA_KO
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "L" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "M" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "S" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "P" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "F" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    ElseIf ddlRicerca.SelectedValue = "NR" Then
    ''        If checkParoleContenute.Checked = True Then
    ''            SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
    ''        Else
    ''            SqlDSPrevTElenco.FilterExpression = "Riferimento >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
    ''        End If
    ''    End If
    ''    SqlDSPrevTElenco.DataBind()

    ''    BtnSetEnabledTo(False)
    ''    ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
    ''    btnNuovo.Enabled = True

    ''    Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    ''    GridViewSped.DataBind()
    ''    If GridViewSped.Rows.Count > 0 Then
    ''        BtnSetEnabledTo(True)
    ''        GridViewSped.SelectedIndex = 0
    ''        Session(IDDOCUMENTI) = GridViewSped.SelectedDataKey.Value
    ''        Try
    ''            Dim row As GridViewRow = GridViewSped.SelectedRow
    ''            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
    ''            btnCreaDDT.Enabled = True
    ''            If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
    ''            If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
    ''            If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
    ''        Catch ex As Exception
    ''            btnCreaDDT.Enabled = True
    ''        End Try
    ''    Else
    ''        BtnSetEnabledTo(False)
    ''        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
    ''        btnNuovo.Enabled = True
    ''        Session(IDDOCUMENTI) = ""
    ''    End If

    ''End Sub
#End Region

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
        If CType(Session(IDDOCUMENTI), String) <> "" Then
            Session(SWOP) = SWOPMODIFICA
            Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI (Allestimento)")
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun ordine selezionato.", WUC_ModalPopup.TYPE_ERROR)
        End If
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        'Session(SWOP) = SWOPNUOVO
        'Session(IDDOCUMENTI) = ""
        'Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini")
    End Sub

    Private Sub btnCreaOF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovoBC.Click
        'DA FARE SE ...
    End Sub
#Region " Crea DDT"
    Private Sub btnCreaDDT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaDDT.Click
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        'EVASIONE CLASSICA Response.Redirect("WF_EvadiDocumenti.aspx?labelForm=Crea Ordine")
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        'GIU160518 CONTROLLO CHE CI SIA ALEMNO UNA VOCE DA ALLESTIRE
        Dim dvDocumentoD As DataView
        dvDocumentoD = SqlDSSpedDByIdSpedizione.Select(DataSourceSelectArguments.Empty)
        If (dvDocumentoD Is Nothing) Then
            TxtNColli.Text = "0"
            TxtNPezzi.Text = "0"
            TxtPesoKG.Text = "0"
            DDLVettore1.SelectedIndex = 0
            optDestinatario.Checked = False
            optMittente.Checked = False
            optVettore.Checked = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione impossibile creare DDT ", "Nessuna quantità da allestire è presente - Avvisare l'ufficio vendite", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If dvDocumentoD.Count = 0 Then
            TxtNColli.Text = "0"
            TxtNPezzi.Text = "0"
            TxtPesoKG.Text = "0"
            DDLVettore1.SelectedIndex = 0
            optDestinatario.Checked = False
            optMittente.Checked = False
            optVettore.Checked = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione impossibile creare DDT ", "Nessuna quantità da allestire è presente - Avvisare l'ufficio vendite", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-----------------------------------------------------------
        'giu070814
        Dim strErrore As String = ""
        Dim CkNumDoc As Long = CheckNumDoc(SWTD(TD.DocTrasportoClienti), strErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica N° Documento da impegnare. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
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
        'giu200323
        'giu020523
        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Session(ERROREALL) = SWSI
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------

        'end 020523
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNOCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNOCVett, strErrore)
        objControllo = Nothing
        If strErrore.Trim = "" Then
            Dim swOK As Boolean = False
            strBloccoCliente = "<strong><span>Attenzione!!!, Cliente:<br>"
            If SWNoPIVACF Then
                strBloccoCliente += "SENZA P.IVA/C.F.<br>"
                swOK = True
            End If
            If SWNoCodIPA Then
                strBloccoCliente += "SENZA Codice IPA<br>"
                swOK = True
            End If
            If SWNoFatt Then
                strBloccoCliente += "NON Fatturabile (Non bloccante)<br>"
                swOK = True
            End If
            If swNOCVett Then
                strBloccoCliente += "SENZA Vettore (Non bloccante)<br>"
                swOK = True
            End If
            If swNoDestM Then
                strBloccoCliente += "SENZA Destinazione Merce<br>"
                swOK = True
            ElseIf swNODatiCorr Then
                strBloccoCliente += "SENZA Dati corriere<br>"
                swOK = True
            End If
            strBloccoCliente += "</span></strong>"
            If swOK = False Then
                strBloccoCliente = ""
            End If
        Else
            If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                'ok
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore verifica Cliente", strErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            
        End If
        'GIU200323
        If SWNoPIVACF Or SWNoCodIPA Or swNoDestM Or swNODatiCorr Then
            If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                '-
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE CREARE DDT<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If

        End If
        '--------
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaDDTRecuperaNum"
            ModalPopup.Show("Crea DDT da Ordine (Evasione ordine)", "Confermi l'evasione dell'ordine selezionato ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea DDT da Ordine (Evasione ordine)", "Confermi l'evasione dell'ordine selezionato ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
 
    End Sub
    'giu080814 GIU090814
    Public Sub CreaDDT()
        Call CreaDDTOK(False)
    End Sub
    Private Function CheckNumDoc(ByVal myTipoDoc As String, ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or _
            myTipoDoc = SWTD(TD.DocTrasportoFornitori) Or _
            myTipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "'"
            'giu110814 qui non verranno mai creati FC,NC e simile quindi OK no FatturaPA
            ' ''ElseIf myTipoDoc = SWTD(TD.FatturaCommerciale) Or _
            ' ''    myTipoDoc = SWTD(TD.FatturaScontrino) Then
            ' ''    'GIU220714
            ' ''    If rbtnFCPA.Checked = True And myTipoDoc = SWTD(TD.FatturaCommerciale) Then
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0)"
            ' ''        End If
            ' ''    Else
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
            ' ''        strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0)"
            ' ''        End If
            ' ''    End If
            ' ''ElseIf myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
            ' ''ElseIf myTipoDoc = SWTD(TD.NotaCredito) Then
            ' ''    'giu220714
            ' ''    If rbtnNCPA.Checked = True Then
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0)"
            ' ''        End If
            ' ''    Else
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
            ' ''            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            ' ''        End If
            ' ''    End If
            ' ''ElseIf myTipoDoc = SWTD(TD.NotaCorrispondenza) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
            ' ''ElseIf myTipoDoc = SWTD(TD.BuonoConsegna) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & myTipoDoc.ToString.Trim & "'"
        End If

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                        'giu260819 non va bene, es. i preventivi ci sono le REVISION quindi sicuramente il numero è superiore
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    'GIU171012
                        ' ''    CheckNumDoc = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < CheckNumDoc, CheckNumDoc, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
                        ' ''End If
                    Else
                        CheckNumDoc = 1
                    End If
                    Exit Function
                Else
                    CheckNumDoc = 1
                    Exit Function
                End If
            Else
                CheckNumDoc = 1
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            CheckNumDoc = -1
            Exit Function
        End Try

    End Function
    'giu080814
    Public Sub CreaDDTRecuperaNum()
        Call CreaDDTOK(True)
    End Sub
    '---------
    Public Sub CreaDDTOK(Optional ByVal SWRecuperaNum As Boolean = False)
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        If CType(Session(IDDOCUMENTI), String) = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun ordine selezionato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
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
        'giu080814
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA 'GIU221217
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If SWRecuperaNum Then
            Dim CkNumDoc As Long = CheckNumDoc(SWTD(TD.DocTrasportoClienti), StrErrore)
            If CkNumDoc = -1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Verifica N° Documento da impegnare. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            NDoc = CkNumDoc
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.DocTrasportoClienti), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO DDT
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocDT]"
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
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1, "FatturaPA"))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1, "SplitIVA"))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.DocTrasportoClienti)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        'giu080814
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'giu150118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Try
            myID = "" 'GIU180423
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo DDT da Ordine", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso/Parz.Evaso </span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo DDT da Ordine", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso/Parz.Evaso </span></strong><br>" _
                        & "Errore: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        Try
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuovo DDT da Ordine", "Verificare i dati del DDT appena creato <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso/Parz.Evaso </span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo DDT da Ordine", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso/Parz.Evaso </span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=DOCUMENTO DI TRASPORTO CLIENTI")
    End Sub
#End Region
    Public Sub AvviaRicerca()
        'qui eventualmente eliminare la distinta ?????? Call btnRicerca_Click(Nothing, Nothing)
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
        ''Session(SWOP) = SWOPELIMINA
        ''Response.Redirect("WF_Documenti.aspx?labelForm=Elimina Ordine")
    End Sub

    Private Sub GridViewSpedT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewSped.SelectedIndexChanged
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        lblErrore.Visible = False
        Dim row As GridViewRow = GridViewSped.SelectedRow
        Try
            Session(IDSPEDIZIONE) = GridViewSped.SelectedDataKey.Value
            Session(IDDOCUMENTI) = row.Cells(CellIdxT.IdDocumenti).Text.Trim
            Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
            Session(NumSPEDIZIONE) = row.Cells(CellIdxT.NumSped).Text.Trim
            LblNumSped.Text = row.Cells(CellIdxT.NumSped).Text.Trim
            LblDataSped.Text = row.Cells(CellIdxT.DataSped).Text.Trim
            Call PopolaTXTDocumento(Session(IDDOCUMENTI))

            GridViewSpedDett.Enabled = True
            GridViewSped.Enabled = True
            BtnModAgg.Enabled = True
            btnModifica.Enabled = True
            btnCreaDDT.Enabled = True
            btnEtiSped.Enabled = True
            btnPrintBrogl.Enabled = True
            btnPrintBroglALL.Enabled = True
            ''Dim Stato As String = row.Cells(CellIdxT.StatoSped).Text.Trim
            ''If Stato.Trim = "Evaso" Then btnCreaDDT.Enabled = False
            ''If Stato.Trim = "Chiuso non evaso" Then btnCreaDDT.Enabled = False
            ''If Stato.Trim = "Non evadibile" Then btnCreaDDT.Enabled = False
        Catch ex As Exception
            lblErrore.Text = "Errore lettura dati: " & ex.Message.Trim
            lblErrore.Visible = True
        End Try
    End Sub

    Private Sub GridViewSpedT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewSped.RowDataBound
        'giu260112 NON ABILITARE e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewSped, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataSped).Text) Then
                e.Row.Cells(CellIdxT.DataSped).Text = Format(CDate(e.Row.Cells(CellIdxT.DataSped).Text), FormatoData).ToString
            End If
            ' ''If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
            ' ''    e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            ' ''End If
            ' ''If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
            ' ''    e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            ' ''End If
        End If
    End Sub

    Private Sub GridViewSpedDett_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewSpedDett.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaAllestita).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaAllestita).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaAllestita).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaAllestita).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaAllestita).Text = ""
                End If
            End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.QtaImpegnata).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.QtaImpegnata).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.QtaImpegnata).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaImpegnata).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.QtaImpegnata).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.QtaResidua).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.QtaResidua).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.QtaResidua).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaResidua).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.QtaResidua).Text = ""
            ' ''    End If
            ' ''End If
        End If
    End Sub

    Private Sub btnChiudiAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChiudiAll.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

    End Sub

    Private Sub btnPrintBrogl_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintBrogl.Click
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        Session(CSTTASTOST) = btnPrintBrogl.ID
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        If Session(NumSPEDIZIONE) <> "" Then
            Call StampaListaCarico(Session(NumSPEDIZIONE))
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "E' necessario selezionare una spedizione.", WUC_ModalPopup.TYPE_ERROR)
        End If

    End Sub
    Private Sub btnPrintBroglALL_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintBroglALL.Click
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        Session(CSTTASTOST) = btnPrintBroglALL.ID
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Call StampaListaCarico(-1)

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


    Protected Sub btnEvadi_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEvadi.Click

    End Sub

    Private Sub StampaListaCarico(ByVal NumeroSpedizione As Integer)
        Dim DsListaCarico1 As New DSListaCarico
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""

        Dim cod1 As String = ""
        Dim cod2 As String = ""

        Try
            Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione
            If ClsPrint.StampaListaCarico(Session(CSTAZIENDARPT), "Lista carico", DsListaCarico1, ObjReport, StrErrore, -1, NumeroSpedizione) Then
                If DsListaCarico1.view_ListaCaricoSpedizione.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsListaCarico1
                    Session(CSTNOBACK) = 0 'giu040512
                    ' ''Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StampaListaCarico", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampaOrdinato()
    End Sub

    ' ''Private Sub GridViewSpedDett_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewSpedDett.SelectedIndexChanged
    ' ''    Dim row As GridViewRow = GridViewSpedDett.SelectedRow
    ' ''    Session(IDDOCUMENTI) = GridViewSpedDett.SelectedDataKey.Value
    ' ''    Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
    ' ''End Sub

    Protected Sub btnEtiSped_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEtiSped.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        If Session(NumSPEDIZIONE) = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "E' necessario selezionare una spedizione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Dim SWSconti As Boolean = False
        Try
            Dim SqlAdapDocAll As New SqlDataAdapter
            Dim SqlDbSelectDettAll As New SqlCommand
            Dim SqlConnDoc As New SqlConnection
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim DsDocAlles As New DataSet
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectDettAll.CommandText = "get_DocAllestiti"
            SqlDbSelectDettAll.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettAll.Connection = SqlConnDoc
            SqlDbSelectDettAll.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettAll.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumSpedizione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocAll.SelectCommand = SqlDbSelectDettAll
            SqlDbSelectDettAll.Parameters("@NumSpedizione").Value = Session(NumSPEDIZIONE)
            SqlAdapDocAll.Fill(DsDocAlles)

            Dim DsEtichette1 As New DSPrintWeb_Documenti
            Dim DsPrinWebDoc As New DSPrintWeb_Documenti
            Dim ObjReport As New Object
            Dim ClsPrint As New Documenti
            Dim StrErrore As String = ""

            Dim idxDoc As Integer
            Dim NDocDaCompletare As String = ""
            For idxDoc = 0 To DsDocAlles.Tables(0).Rows.Count - 1
                TipoDoc = DsDocAlles.Tables(0).Rows(idxDoc).Item("Tipo_Doc")
                If DsDocAlles.Tables(0).Rows(idxDoc).Item("Colli") = 0 Then
                    NDocDaCompletare = NDocDaCompletare & TipoDoc & " " & DsDocAlles.Tables(0).Rows(idxDoc).Item("Numero") & ", "
                End If
            Next idxDoc
            If NDocDaCompletare <> "" Then
                NDocDaCompletare = Mid(NDocDaCompletare, 1, Len(NDocDaCompletare) - 2)
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "E' necessario impostare i colli per i seguenti documenti:" & vbCr & NDocDaCompletare, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If

            Dim I As Integer
            Dim EseguoStampa As Boolean = False
            Dim OrdineEtichette As Integer = 1
            For I = 0 To DsDocAlles.Tables(0).Rows.Count - 1
                EseguoStampa = True
                TipoDoc = DsDocAlles.Tables(0).Rows(I).Item("Tipo_Doc")
                DsPrinWebDoc.Clear() 'GIU020512
                If ClsPrint.StampaDocumento(DsDocAlles.Tables(0).Rows(I).Item("IdDocumenti"), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    EseguoStampa = False
                    Exit Sub
                End If
                If AggiornaDSEtichette(DsEtichette1, DsPrinWebDoc, DsDocAlles.Tables(0).Rows(I).Item("Colli"), 1, OrdineEtichette) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la preparazione del sovracollo.", WUC_ModalPopup.TYPE_ERROR)
                    EseguoStampa = False
                    Exit Sub
                End If
            Next I
            ''''qui mettere richiesta per posizione partenza etichetta....
            If EseguoStampa Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrepEtichette) = DsEtichette1
                Session(CSTDsRitornoEtichette) = "WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni"
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebEtichette.aspx")
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Function AggiornaDSEtichette(ByRef DsEtichette1 As DSPrintWeb_Documenti, ByVal DsPrinWebDoc As DSPrintWeb_Documenti, ByVal NColli As Integer, ByRef PosPartenza As Integer, ByRef OrdineEtichette As Integer) As Boolean
        Dim RowEti As DSPrintWeb_Documenti.EtichetteColloRow
        Dim I As Integer = 0
        Dim K As Integer = 0
        Dim PosFine As Integer
        Dim idxCollo As Integer = 1
        Try
            PosFine = PosPartenza + NColli

            For K = 1 To PosPartenza - 1
                RowEti = DsEtichette1.EtichetteCollo.NewRow
                RowEti.Numero = OrdineEtichette
                RowEti.Item("Riga1") = ""
                RowEti.Item("Riga2") = ""
                RowEti.Item("Riga3") = ""
                RowEti.Item("Riga4") = ""
                RowEti.Item("Riga5") = ""
                RowEti.Item("Riga6") = ""
                RowEti.Item("Riga7") = ""
                RowEti.Item("Riga8") = ""
                RowEti.Item("Riga9") = ""
                RowEti.Desc_Collo = ""
                RowEti.Desc_Spedizione = ""
                DsEtichette1.EtichetteCollo.AddEtichetteColloRow(RowEti)
                OrdineEtichette = OrdineEtichette + 1
            Next K

            Dim NRiga As Integer = 0
            For I = PosPartenza To PosFine - 1
                NRiga = 1

                Dim TmpIntest As New ArrayList
                Dim TmpItem As ListItem
                Dim CAPLocProvCompleto As String = ""
                Dim Tel1 As String = ""

                If DsPrinWebDoc.Clienti.Rows.Count > 0 Then
                    If IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Filiale")) Then
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Rag_Soc").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Rag_Soc")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Denominazione")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Indirizzo").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Indirizzo")
                            TmpIntest.Add(TmpItem)
                        End If
                        CAPLocProvCompleto = DsPrinWebDoc.Clienti.Rows(0).Item("CAP").ToString.Trim
                        CAPLocProvCompleto = CAPLocProvCompleto & " " & DsPrinWebDoc.Clienti.Rows(0).Item("Localita").ToString.Trim
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Provincia").ToString.Trim <> "" Then
                            CAPLocProvCompleto = CAPLocProvCompleto & " (" & DsPrinWebDoc.Clienti.Rows(0).Item("Provincia").ToString.Trim & ")"
                        End If
                        If CAPLocProvCompleto <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = CAPLocProvCompleto
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono1").ToString.Trim <> "" Then
                            Tel1 = "Tel. " & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono1").ToString.Trim
                        End If
                        If Tel1 <> "" Then
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                Tel1 = Tel1 & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim
                            End If
                        Else
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                Tel1 = "Tel. " & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim
                            End If
                        End If
                        If Tel1 <> "" Then
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                Tel1 = Tel1 & " Fax " & DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim
                            End If
                        Else
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                Tel1 = Tel1 & "Fax " & DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim
                            End If
                        End If
                        If Tel1 <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = Tel1
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                    Else
                        'giu180423
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Ragione_Sociale35").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Ragione_Sociale35").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            Else
                                If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                                    TmpItem = New ListItem
                                    TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                                    TmpIntest.Add(TmpItem)
                                End If
                            End If
                        Else
                            If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                                TmpIntest.Add(TmpItem)
                            End If
                        End If
                        'giu180423
                        '''If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                        '''    TmpItem = New ListItem
                        '''    TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                        '''    TmpIntest.Add(TmpItem)
                        '''End If
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            End If
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento35").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento35").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            ElseIf DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            End If
                        End If
                        'giu180423
                        '''If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                        '''    If DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                        '''        TmpItem = New ListItem
                        '''        TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim
                        '''        TmpIntest.Add(TmpItem)
                        '''    End If
                        '''    If DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim <> "" Then
                        '''        TmpItem = New ListItem
                        '''        TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim
                        '''        TmpIntest.Add(TmpItem)
                        '''    End If
                        '''End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3")
                            TmpIntest.Add(TmpItem)
                        End If
                        Tel1 = ""
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono1").ToString.Trim <> "" Then
                                Tel1 = "Tel. " & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono1").ToString.Trim
                            End If
                            If Tel1 <> "" Then
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim
                                End If
                            Else
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                    Tel1 = "Tel. " & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim
                                End If
                            End If
                            If Tel1 <> "" Then
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & " Fax " & DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim
                                End If
                            Else
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & "Fax " & DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim
                                End If
                            End If
                        End If
                        If Tel1 <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = Tel1
                            TmpIntest.Add(TmpItem)
                        End If
                    End If
                End If

                Dim idx As Integer
                RowEti = DsEtichette1.EtichetteCollo.NewRow
                For idx = 0 To TmpIntest.Count - 1
                    Select Case idx
                        Case 0
                            RowEti.Riga1 = TmpIntest(idx).Text
                        Case 1
                            RowEti.Riga2 = TmpIntest(idx).Text
                        Case 2
                            RowEti.Riga3 = TmpIntest(idx).Text
                        Case 3
                            RowEti.Riga4 = TmpIntest(idx).Text
                        Case 4
                            RowEti.Riga5 = TmpIntest(idx).Text
                        Case 5
                            RowEti.Riga6 = TmpIntest(idx).Text
                        Case 6
                            RowEti.Riga7 = TmpIntest(idx).Text
                    End Select
                Next idx

                RowEti.Numero = OrdineEtichette
                RowEti.Desc_Collo = "Collo " & idxCollo & " di " & NColli
                RowEti.Desc_Spedizione = ""
                DsEtichette1.EtichetteCollo.AddEtichetteColloRow(RowEti)

                OrdineEtichette = OrdineEtichette + 1
                idxCollo = idxCollo + 1
            Next I
            Return True
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in AggiornaDSEtichette", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try
    End Function

    Private Sub btnGestVett1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGestVett1.Click
        WFPVettori.WucElement = Me
        WFPVettori.SvuotaCampi()
        Session(F_ANAGRVETTORI_APERTA) = True
        WFPVettori.Show()
    End Sub
    Public Sub CallBackWFPAnagrVettori()
        Session(IDVETTORI) = ""
        Dim rk As StrVettori
        rk = Session(RKVETTORI)
        If IsNothing(rk.IDVettori) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDVETTORI) = rk.IDVettori

        SqlDSVettori.DataBind()
        DDLVettore1.Items.Clear()
        DDLVettore1.Items.Add("")
        DDLVettore1.DataBind()
        '-- mi riposiziono sul vettore/i
        PosizionaItemDDL(Session(IDVETTORI), DDLVettore1)
    End Sub
    Public Sub CancBackWFPAnagrVettori()

    End Sub
    Private Sub SetEnableTxt(ByVal Value As Boolean)
        optDestinatario.Enabled = Value
        optMittente.Enabled = Value
        optVettore.Enabled = Value
        TxtNColli.Enabled = Value
        TxtNPezzi.Enabled = Value
        TxtPesoKG.Enabled = Value
        btnGestVett1.Enabled = Value
        DDLVettore1.Enabled = Value
        If Value Then
            BtnModAgg.Enabled = Value
            BtnModAgg.Text = "Aggiorna"
            BtnModAgg.Width = 108
            'giu260112
            GridViewSpedDett.Enabled = False
            GridViewSped.Enabled = False
            btnModifica.Enabled = False
            btnCreaDDT.Enabled = False
            btnEtiSped.Enabled = False
            btnPrintBrogl.Enabled = False
            btnPrintBroglALL.Enabled = False
        Else
            BtnModAgg.Enabled = Value
            BtnModAgg.Text = "Modifica dati spedizione"
            BtnModAgg.Width = 170
            'giu260112
            If GridViewSped.Rows.Count > 0 Then
                BtnModAgg.Enabled = True
                GridViewSpedDett.Enabled = True
                GridViewSped.Enabled = True
                btnModifica.Enabled = True
                btnCreaDDT.Enabled = True
                btnEtiSped.Enabled = True
                btnPrintBrogl.Enabled = True
                btnPrintBroglALL.Enabled = True
            End If
        End If
        BtnAnnDatiSped.Enabled = Value
        BtnAnnDatiSped.Visible = Value
        GridViewSped.Enabled = Not Value
        If optDestinatario.Checked Or optMittente.Checked Then
            btnGestVett1.Enabled = False
            DDLVettore1.Enabled = False
        End If
    End Sub

    Private Sub BtnModAgg_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnModAgg.Click
        If BtnModAgg.Text = "Aggiorna" Then
            Try
                Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
                Dim SqlConnDoc As New SqlConnection
                Dim SqlDbUpdateCmd As New SqlCommand
                SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
                SqlDbUpdateCmd.CommandText = "[Update_DocTDatiSpedByIDDocumenti]"
                SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbUpdateCmd.Connection = SqlConnDoc
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Peso", System.Data.SqlDbType.Decimal, 11, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Peso", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Colli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Colli", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Pezzi", System.Data.SqlDbType.Decimal, 11, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Pezzi", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_1", System.Data.DataRowVersion.Current, Nothing))
                SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Spedizione", System.Data.SqlDbType.NVarChar, 1, "Tipo_Spedizione"))

                Dim Colli As Integer
                Dim Peso As Decimal
                Dim Pezzi As Integer
                Dim TipoSped As String
                Try
                    Colli = CLng(TxtNColli.Text)
                Catch ex As Exception
                    Colli = 0
                End Try
                Try
                    Pezzi = CLng(TxtNPezzi.Text)
                Catch ex As Exception
                    Pezzi = 0
                End Try
                Try
                    Peso = CDec(TxtPesoKG.Text)
                Catch ex As Exception
                    Peso = 0
                End Try
                TipoSped = "M"
                If optMittente.Checked Then TipoSped = "M"
                If optDestinatario.Checked Then TipoSped = "D"
                If optVettore.Checked Then TipoSped = "V"

                SqlDbUpdateCmd.Parameters("@IDDocumenti").Value = Session(IDDOCUMENTI)
                SqlDbUpdateCmd.Parameters("@Peso").Value = Peso
                SqlDbUpdateCmd.Parameters("@Colli").Value = Colli
                SqlDbUpdateCmd.Parameters("@Pezzi").Value = Pezzi
                If DDLVettore1.SelectedIndex = 0 Then
                    SqlDbUpdateCmd.Parameters("@Vettore_1").Value = DBNull.Value
                Else
                    SqlDbUpdateCmd.Parameters("@Vettore_1").Value = DDLVettore1.SelectedValue
                End If
                SqlDbUpdateCmd.Parameters("@Tipo_Spedizione").Value = TipoSped
                SqlConnDoc.Open()
                SqlDbUpdateCmd.ExecuteNonQuery()
                SqlConnDoc.Close()
                SqlDbUpdateCmd = Nothing
                SqlConnDoc = Nothing
            Catch ex As Exception

            End Try
            SetEnableTxt(False)
        Else
            SetEnableTxt(True)
        End If
    End Sub
    Private Function PopolaTXTDocumento(ByVal IDDocumenti As Long) As Boolean
        Dim dvDocumentoT As DataView
        dvDocumentoT = SqlDSSpedTElenco.Select(DataSourceSelectArguments.Empty)
        If (dvDocumentoT Is Nothing) Then
            TxtNColli.Text = "0"
            TxtNPezzi.Text = "0"
            TxtPesoKG.Text = "0"
            DDLVettore1.SelectedIndex = 0
            optDestinatario.Checked = False
            optMittente.Checked = False
            optVettore.Checked = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura testata ordine", "Nessun ordine presente", WUC_ModalPopup.TYPE_ERROR)
            Return False
            Exit Function
        End If
        If dvDocumentoT.Count > 0 Then
            dvDocumentoT.RowFilter = "IdDocumenti = " & IDDocumenti
            If dvDocumentoT.Count > 0 Then
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Tipo_Doc")) Then
                    Session(CSTTIPODOC) = dvDocumentoT.Item(0).Item("Tipo_Doc")
                Else
                    Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
                End If
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Colli")) Then
                    TxtNColli.Text = dvDocumentoT.Item(0).Item("Colli")
                Else
                    TxtNColli.Text = 0
                End If
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Pezzi")) Then
                    TxtNPezzi.Text = dvDocumentoT.Item(0).Item("Pezzi")
                Else
                    TxtNPezzi.Text = 0
                End If
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Peso")) Then
                    TxtPesoKG.Text = dvDocumentoT.Item(0).Item("Peso")
                Else
                    TxtPesoKG.Text = 0
                End If
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Vettore_1")) Then
                    DDLVettore1.SelectedValue = dvDocumentoT.Item(0).Item("Vettore_1")
                    lblMessVettore1.Visible = False
                    'GIU080523 SE SPAZI DEV'ESSERE SCELTO OBB ERIKA EMAIL 05/05/23
                    '''ElseIf DDLVettore1.Items.Count > 1 Then 'giu170722 Iredeem Zibordi fisso DHL,in questo caso il primo
                    '''    ''giu200722 
                    '''    lblMessVettore1.Visible = False
                    '''    Dim strValore As String = "" : Dim strErrore As String = ""
                    '''    strValore = Session("SpedVettCsv")
                    '''    If IsNothing(strValore) Then
                    '''        strValore = ""
                    '''    End If
                    '''    If String.IsNullOrEmpty(strValore) Then
                    '''        strValore = ""
                    '''    End If
                    '''    If strValore.Trim = "" Then
                    '''        Call GetDatiAbilitazioni(CSTABILAZI, "SpedVettCsv", strValore, strErrore)
                    '''        If strErrore.Trim <> "" Then
                    '''            strValore = ""
                    '''        End If
                    '''        Session("SpedVettCsv") = strValore
                    '''    End If
                    '''    '----------
                    '''    If strValore.Trim <> "" Then
                    '''        PosizionaItemDDL(strValore.Trim, DDLVettore1)
                    '''        lblMessVettore1.Visible = True
                    '''    Else
                    '''        DDLVettore1.SelectedIndex = 0
                    '''    End If
                Else
                    DDLVettore1.SelectedIndex = 0
                    lblMessVettore1.Visible = False
                End If
                If Not IsDBNull(dvDocumentoT.Item(0).Item("Tipo_Spedizione")) Then
                    If dvDocumentoT.Item(0).Item("Tipo_Spedizione") = "M" Then
                        optMittente.Checked = True
                        optDestinatario.Checked = False
                        optVettore.Checked = False
                        btnGestVett1.Enabled = False
                        DDLVettore1.Enabled = False
                        DDLVettore1.SelectedIndex = 0
                    ElseIf dvDocumentoT.Item(0).Item("Tipo_Spedizione") = "D" Then
                        optMittente.Checked = False
                        optDestinatario.Checked = True
                        optVettore.Checked = False
                        btnGestVett1.Enabled = False
                        DDLVettore1.Enabled = False
                        DDLVettore1.SelectedIndex = 0
                    ElseIf dvDocumentoT.Item(0).Item("Tipo_Spedizione") = "V" Then
                        optMittente.Checked = False
                        optDestinatario.Checked = False
                        optVettore.Checked = True
                        btnGestVett1.Enabled = False
                        DDLVettore1.Enabled = False
                    End If
                Else
                    btnGestVett1.Enabled = False
                    DDLVettore1.Enabled = False
                    DDLVettore1.SelectedIndex = 0
                    optDestinatario.Checked = False
                    optMittente.Checked = False
                    optVettore.Checked = False
                End If
            Else
                TxtNColli.Text = "0"
                TxtNPezzi.Text = "0"
                TxtPesoKG.Text = "0"
                DDLVettore1.SelectedIndex = 0
                optDestinatario.Checked = False
                optMittente.Checked = False
                optVettore.Checked = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in lettura testata ordine", "Non trovato documento", WUC_ModalPopup.TYPE_ERROR)
                Return False
                Exit Function
            End If
        Else
            TxtNColli.Text = "0"
            TxtNPezzi.Text = "0"
            TxtPesoKG.Text = "0"
            DDLVettore1.SelectedIndex = 0
            optDestinatario.Checked = False
            optMittente.Checked = False
            optVettore.Checked = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in lettura testata ordine", "Non trovato documento", WUC_ModalPopup.TYPE_ERROR)
            Return False
            Exit Function
        End If

    End Function

    Protected Sub BtnAnnDatiSped_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnAnnDatiSped.Click
        SetEnableTxt(False)
        Call PopolaTXTDocumento(Session(IDDOCUMENTI))
    End Sub

    Private Sub optDestinatario_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optDestinatario.CheckedChanged
        If optDestinatario.Checked Then
            btnGestVett1.Enabled = False
            DDLVettore1.Enabled = False
            DDLVettore1.SelectedIndex = 0
        End If
    End Sub

    Private Sub optMittente_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optMittente.CheckedChanged
        If optMittente.Checked Then
            btnGestVett1.Enabled = False
            DDLVettore1.Enabled = False
            DDLVettore1.SelectedIndex = 0
        End If
    End Sub

    Private Sub optVettore_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optVettore.CheckedChanged
        If optVettore.Checked Then
            btnGestVett1.Enabled = True
            DDLVettore1.Enabled = True
        End If
    End Sub

    Private Sub GridViewSped_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewSped.Sorted
        LnkStampaAll.Visible = False : LnkStampaSing.Visible = False
        Session(IDDOCUMENTI) = ""
        Session(NumSPEDIZIONE) = ""
        LblNumSped.Text = ""
        LblDataSped.Text = ""
        TxtNColli.Text = "0"
        TxtNPezzi.Text = "0"
        TxtPesoKG.Text = "0"
        DDLVettore1.SelectedIndex = 0
        optDestinatario.Checked = False
        optMittente.Checked = False
        optVettore.Checked = False
        BtnSetEnabledTo(False)
        If GridViewSped.Rows.Count > 0 Then
            Try
                GridViewSped.SelectedIndex = -1
            Catch ex As Exception
            End Try
        End If
    End Sub

    'giu220120
    Private Sub OKApriStampaOrdinato()
        'da inserire nel load della pagina
        Dim Rpt As Object = Nothing

        If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticolo Then 'ORDINATO PER ARTICOLO
            Rpt = New OrdArt
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloData Then 'ORDINATO PER ARTICOLO DATA
            Rpt = New OrdArtData
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge Then
            Rpt = New OrdCli
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc Then
            Rpt = New OrdCli_RagSoc
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloCliente Then
            Rpt = New StOrdArtCli
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloFornitore Then 'giu110612
            Rpt = New StOrdArtFor
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdForOrdTutti Then 'giu281013
            Rpt = New StatOrdinatoFornOrdine
            Dim dsStatOrdinatoClienteOrdine1 As dsStatOrdinatoClienteOrdine
            dsStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsStatOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdine Then
            Rpt = New OrdinatoClienteOrdine
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico Then
            Rpt = New ListaCarico
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione Then
            Rpt = New ListaCaricoSpedizione
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDoc Then
            Rpt = New OrdinatoOrdineSortByNDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDoc Then
            Rpt = New OrdinatoOrdineSortByDataDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegna Then
            Rpt = New OrdinatoOrdineSortByDataConsegna
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCogeAg Then
            Rpt = New OrdCliAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSocAg Then
            Rpt = New OrdCli_RagSocAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloAg Then
            Rpt = New OrdArtAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloDataAg Then
            Rpt = New OrdArtDataAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg Then
            Rpt = New StOrdArtCliAg
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdineAG Then
            Rpt = New OrdinatoClienteOrdineAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDocAG Then
            Rpt = New OrdinatoOrdineSortByNDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDocAG Then
            Rpt = New OrdinatoOrdineSortByDataDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegnaAG Then
            Rpt = New OrdinatoOrdineSortByDataConsegnaAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdinatoClienteOrdine Then
            Rpt = New StatOrdinatoClienteOrdine
            Dim DSStatOrdinatoClienteOrdine1 As New dsStatOrdinatoClienteOrdine
            DSStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSStatOrdinatoClienteOrdine1)
            'CrystalReportViewer1.DisplayGroupTree = False
            'CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA ORDINATO SCONOSCIUTA")
            Exit Sub
        End If
        '-
        If CKCSTTipoDocST() = False Then
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        Dim NomeStampa As String = "LISTACARICORDINE.PDF"
        Dim SubDirDOC As String = "Ordini"
        Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        'giu140615 Dim LnkName As String = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/" & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnPrintBrogl.ID Then
            LnkStampaSing.Visible = True
        ElseIf Session(CSTTASTOST) = btnPrintBroglALL.ID Then
            LnkStampaAll.Visible = True
        Else
            LnkStampaSing.Visible = True
        End If

        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnPrintBrogl.ID Then
            LnkStampaSing.HRef = LnkName
        ElseIf Session(CSTTASTOST) = btnPrintBroglALL.ID Then
            LnkStampaAll.HRef = LnkName
        Else
            LnkStampaSing.HRef = LnkName
        End If
    End Sub
    Private Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocST = True
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
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim 'GIU210120
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            Return False
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Return False
        End If
        If CodiceDitta = "" Then
            Return False
        End If
        '-------------------------------------------------------------------
    End Function
    '----
End Class