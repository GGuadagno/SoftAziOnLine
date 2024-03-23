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
Imports SoftAziOnLine.Magazzino
Imports System.Data.SqlClient
'Imports Microsoft.Reporting.WebForms
Imports System.IO
Partial Public Class WUC_ContrattiElScad
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private SWFatturaPA As Boolean = False 'GIU080814
    Private SWSplitIVA As Boolean = False 'giu05018
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""

    Private aDataViewScAtt As DataView
    Private Const sessDataViewScAtt As String = "sessDVScAttCElScad"

    Private Enum CellIdxT
        RespArea = 1
        RespVisite = 2
        DataSc = 3
        CodArt = 4
        DesArt = 5
        SerieLotto = 6
        Modello = 7
        NoteApp = 8
        UM = 9
        Qta = 10
        QtaEv = 11

        RagSoc = 12
        Denom = 13

        LuogoApp = 14
        Importo = 15
        TipoDoc = 16
        NumDoc = 17
        Riga = 18
        DataDoc = 19
        CodCliForProvv = 20

        Loc = 21
        Pr = 22
        CAP = 23
        Riferimento = 24
        Dest1 = 25
        Dest2 = 26
        Dest3 = 27
        Stato = 28
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ContrattiElScad.aspx?labelForm=Elenco Attività in scadenza"
        'giu291119 CONTRATTI
        ModalPopup.WucElement = Me
        WFPCambiaStatoCA.WucElement = Me
        WFPDocCollegati.WucElement = Me
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSRespArea.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        'caricamento grid default
        If (Not IsPostBack) Then
            Try
                If String.IsNullOrEmpty(Session(CALLGESTIONE)) Then
                    Session(sessDataViewScAtt) = Nothing
                    Session(CALLGESTIONE) = SWNO
                ElseIf Session(CALLGESTIONE) = SWSI Then
                    'non azzero sempre che non sia scaduta
                Else
                    Session(sessDataViewScAtt) = Nothing
                    Session(CALLGESTIONE) = SWNO
                End If
            Catch ex As Exception
                Session(sessDataViewScAtt) = Nothing
                Session(CALLGESTIONE) = SWNO
            End Try
        End If
        '----------------------------------
        If Not Session(sessDataViewScAtt) Is Nothing Then
            aDataViewScAtt = Session(sessDataViewScAtt)
        Else
            If aDataViewScAtt Is Nothing Then
                aDataViewScAtt = New DataView
            End If
        End If
        Session(sessDataViewScAtt) = aDataViewScAtt
        '---------------------------------
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Session(CALLGESTIONE) = SWSI
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnElimina.Visible = False
            lblStampe.Visible = False
            btnVerbale.Visible = False
            btnStampa.Visible = False
        End If
        '----------------------------
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
        '-
        ''''@@@ risolto in modo diverso - apro una pagina solo per le stampe da riportare per tutte le altre - NO ANTEPRIMA (WF_PrintWebCR)
        '''If IsPostBack Then
        '''    If Request.Params.Get("__EVENTTARGET").ToString = "LnkStampaPDF" Then
        '''        'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
        '''        VisualizzaRpt(Session("ContrattiElScadStampa"), Session(CSTNOMEPDF), "PDF")
        '''        Exit Sub
        '''    End If
        '''    If Request.Params.Get("__EVENTTARGET").ToString = "lnkElencoScOK" Then
        '''        'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
        '''        If chkVisElenco.Checked Then
        '''            VisualizzaRpt(Session("ContrattiElScadStampa"), Session(CSTNOMEPDF), "PDF")
        '''        Else
        '''            VisualizzaRpt(Session("ContrattiElScadStampa"), Session(CSTNOMEPDF), "EXEL")
        '''        End If
        '''        Exit Sub
        '''    End If
        '''End If
        '@@@
        If (Not IsPostBack) Then
            Try
                Session(CSTNUOVOCADAOC) = SWNO
                ddlRicerca.Items.Add("Numero contratto")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("N° Serie DAE")
                ddlRicerca.Items(1).Value = "NS"
                ddlRicerca.Items.Add("Data contratto")
                ddlRicerca.Items(2).Value = "D"
                ddlRicerca.Items.Add("Data Inizio CA")
                ddlRicerca.Items(3).Value = "DI"
                ddlRicerca.Items.Add("Data Fine CA")
                ddlRicerca.Items(4).Value = "DF"
                ddlRicerca.Items.Add("Data Accettazione")
                ddlRicerca.Items(5).Value = "DA"

                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(6).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(7).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(8).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(9).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(10).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(11).Value = "M"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(12).Value = "NR"
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(13).Value = "CG"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(14).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(15).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(16).Value = "D3"
                '--
                btnSblocca.Text = "Sblocca documento"
                '-----------
                checkCausale.AutoPostBack = False : checkCausale.Checked = False : checkCausale.AutoPostBack = True
                DDLCausali.AutoPostBack = False
                DDLCausali.Enabled = False : DDLCausali.SelectedIndex = 0
                DDLCausali.AutoPostBack = True
                '-----------------------------------------------
                chkTutteProvince.AutoPostBack = False : chkTutteRegioni.AutoPostBack = False
                chkTutteRegioni.Checked = True
                ddlRegioni.Enabled = False
                chkTutteProvince.Checked = True
                ddlProvince.Enabled = False
                Session("CodRegione") = 0
                ddlRegioni.SelectedIndex = -1
                ddlProvince.SelectedIndex = -1
                chkTutteProvince.AutoPostBack = True : chkTutteRegioni.AutoPostBack = True
                '- Cercare la prima scadenza 
                txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
                txtDallaData.Text = GetPrimaDataSc()
                If txtDallaData.Text.Trim = "" Then
                    txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
                End If
                Dim myAllaData As DateTime = DateAdd(DateInterval.Month, 1, Now.Date).ToString("01" + "/MM/yyyy")
                txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
                '- COME DA MENU FINO A .....
                myAllaData = CDate(txtAllaData.Text.Trim)
                Dim NGG As Integer = 30
                If Not String.IsNullOrEmpty(Session("NGGCAScadAtt")) Then
                    If IsNumeric(Session("NGGCAScadAtt")) Then
                        NGG = Session("NGGCAScadAtt")
                    End If
                End If
                If DateAdd(DateInterval.Day, NGG, Now.Date) > myAllaData Then
                    myAllaData = DateAdd(DateInterval.Day, NGG, Now.Date)
                    myAllaData = DateAdd(DateInterval.Month, 1, myAllaData).ToString("01" + "/MM/yyyy")
                    txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
                End If
                'GIU020323 ERRRORE EVENTI SOVRAPPOSTI txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
                '-giu210622
                Session(IDRESPAREA) = 0
                Call DataBindRV()
                '--
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
                Session(IDDOCUMENTI) = ""
                Session(SWOP) = SWOPNESSUNA
                Session(SWOPDETTDOC) = SWOPNESSUNA
                Session(SWOPDETTDOCL) = SWOPNESSUNA
                BuidDett()
            Catch ex As Exception
                Chiudi("Errore: Caricamento Elenco contratti: " & ex.Message)
                Exit Sub
            End Try

        End If

        If Session(F_CAMBIOSTATO_APERTA) Then
            WFPCambiaStatoCA.Show()
        End If
        'Simone 290317
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
    'giu270520 prima data scadenza ATTIVITA
    Private Function GetPrimaDataSc() As String
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        strSQL = "SELECT TOP 1 MIN(ContrattiD.DataSc) AS DataSc " &
                        "FROM     ContrattiT INNER JOIN " &
                        "ContrattiD ON ContrattiT.IDDocumenti = ContrattiD.IDDocumenti " &
                        "WHERE(ContrattiT.StatoDoc < 3) AND ContrattiD.Qta_Ordinata <> ContrattiD.Qta_Evasa  " &
                        "GROUP BY ContrattiD.DataSc"
        Dim ds As New DataSet
        Try
            GetPrimaDataSc = ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataSc")) Then
                        GetPrimaDataSc = Format(ds.Tables(0).Rows(0).Item("DataSc"), FormatoData)
                    End If
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            GetPrimaDataSc = ""
            Exit Function
        End Try
    End Function
    'giu270112
    Private Sub BtnSetByStato(ByVal myStato As String)
        'giu270612
        btnSblocca.Visible = False
        Dim SWBloccoModifica As Boolean = False
        Dim SWBloccoElimina As Boolean = False
        Dim SWBloccoCambiaStato As Boolean = False
        '--
        BtnSetEnabledTo(True)
        If myStato.Trim = "Evaso" Or
                        myStato.Trim = "Chiuso non evaso" Or
                        myStato.Trim = "Non evadibile" Or
                        myStato.Trim = "Parz. evaso" Then
            btnCambiaStato.Enabled = False : SWBloccoCambiaStato = True
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            'btnVerbale.Enabled = False
            'btnStampa.Enabled = False
        End If
        If myStato.Trim = "Parz. evaso" Then
            btnElimina.Enabled = False : SWBloccoElimina = True
            'btnVerbale.Enabled = False
        End If
        If SWBloccoElimina Or SWBloccoModifica Or SWBloccoCambiaStato Then
            btnSblocca.Visible = True
        End If
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnVisualizza.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnStampa.Enabled = Valore
        btnVerbale.Enabled = Valore
        btnElencoSc.Enabled = Valore
        btnOKModInviati.Enabled = Valore
        btnDocCollegati.Enabled = Valore
    End Sub

    Private Sub SvuodaGridT()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""
        btnRicerca.BackColor = SEGNALA_KO
        btnElencoSc.Enabled = False 'giu270223
        aDataViewScAtt = Nothing
        aDataViewScAtt = New DataView
        Session(sessDataViewScAtt) = aDataViewScAtt
        GridViewPrevT.DataSource = aDataViewScAtt
        btnSblocca.Visible = False
        '---
        GridViewPrevT.DataBind()
    End Sub
#Region " Ordinamento e ricerca"
    Private Sub checkCausale_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkCausale.CheckedChanged
        SetLnk()
        SvuodaGridT()
        If checkCausale.Checked = True Then
            DDLCausali.AutoPostBack = False : DDLCausali.Enabled = True : DDLCausali.SelectedIndex = 0 : DDLCausali.AutoPostBack = True
            Dim strValore As String = "" : Dim strErrore As String = ""
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLCausali)
                Call DDLCausali_SelectedIndexChanged(Nothing, Nothing)
                Exit Sub
            End If
        Else
            DDLCausali.AutoPostBack = False : DDLCausali.Enabled = False : DDLCausali.SelectedIndex = 0 : DDLCausali.AutoPostBack = True
        End If
        If checkCausale.Checked = True Then
            DDLCausali.Focus()
        End If
    End Sub
    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        SvuodaGridT()
        SetLnk()
        If DDLCausali.SelectedIndex = 0 Then
            DDLCausali.BackColor = SEGNALA_KO
        Else
            DDLCausali.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub BuidDett()
        btnRicerca.BackColor = btnElencoSc.BackColor
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""

        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        Session(sessDataViewScAtt) = aDataViewScAtt
        '---------
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnSblocca.Visible = False
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        SvuodaGridT()
        SetLnk()
        checkParoleContenute.Text = "Parole contenute"
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
               ddlRicerca.SelectedValue = "DI" Or
               ddlRicerca.SelectedValue = "DF" Or
               ddlRicerca.SelectedValue = "DA" Then
            checkParoleContenute.Text = ">= Alla Data"
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        SvuodaGridT()
        SetLnk()
        chkVisElenco.Visible = True
        Dim strFiltroRicerca As String = ""
        Dim SWSingolo As Boolean = False
        If DDLRespArea.SelectedIndex < 1 Then
            SWSingolo = False
        End If
        Dim SWErrore As Boolean = False
        txtDallaData.Text = ConvData(txtDallaData.Text.ToString.Trim)
        If txtDallaData.Text.Trim.Length <> 10 Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtDallaData.Text.Trim) Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtDallaData.BackColor = SEGNALA_OK
        End If
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        '-
        txtAllaData.Text = ConvData(txtAllaData.Text.ToString.Trim)
        If txtAllaData.Text.Trim.Length <> 10 Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtAllaData.BackColor = SEGNALA_OK
        End If
        '------------------------------
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        If checkCausale.Checked = True And DDLCausali.SelectedIndex < 1 Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali.Focus()
            Exit Sub
        End If
        If checkRespArea.Checked = True And DDLRespArea.SelectedIndex < 1 Then
            DDLRespArea.BackColor = SEGNALA_KO : DDLRespArea.Focus()
            Exit Sub
        End If
        If CheckRespVisite.Checked = True And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex < 1 Then
            ddlRegioni.BackColor = SEGNALA_KO : ddlRegioni.Focus()
            Exit Sub
        End If
        If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex < 1 Then
            ddlProvince.BackColor = SEGNALA_KO : ddlProvince.Focus()
            Exit Sub
        End If
        '---------
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
               ddlRicerca.SelectedValue = "DI" Or
               ddlRicerca.SelectedValue = "DF" Or
               ddlRicerca.SelectedValue = "DA" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        'giu090520
        'PARAMETRI SP
        SqlDSPrevTElenco.SelectParameters("DallaData").DefaultValue = txtDallaData.Text.Trim
        'GIU110423
        If chkScadAnno.Checked = True Then
            SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = "31/12/" + CDate(txtAllaData.Text.Trim).Year.ToString.Trim
        Else
            SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = txtAllaData.Text.Trim
        End If
        '---------
        strFiltroRicerca = "Elenco Scadenze Attività non ancora evase nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        'GIU290623 NON UTILIZZATO E NE GESTITO NELLA SP
        SqlDSPrevTElenco.SelectParameters("Escludi").DefaultValue = True 'giu050124 NON USATO chkNoEscludiInvioM.Checked
        '''If chkNoEscludiInvioM.Checked Then
        '''    strFiltroRicerca += " - NON ESCLUDERE le Scadenze Attività già programmate in precedenza (Inviati i moduli per l'Evasione)"
        '''End If
        If DDLRespArea.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = 0
        Else
            SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = CInt(DDLRespArea.SelectedValue.ToString.Trim)
        End If
        If DDLRespVisite.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
        Else
            'giu210622 il respVisite è presente piu volte xke collegato a piu RespArea
            'SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = CInt(DDLRespVisite.SelectedValue.ToString.Trim)
            SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
            '----------
            strFiltroRicerca += " - Solo il Resp.Visita: " + DDLRespVisite.SelectedItem.Text.Trim
        End If
        If DDLCausali.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = 0
        Else
            SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = CInt(DDLCausali.SelectedValue.ToString.Trim)
            strFiltroRicerca += " - Tipo Contratto: " + DDLCausali.SelectedItem.Text.Trim
        End If
        SqlDSPrevTElenco.SelectParameters("SoloDaEv").DefaultValue = True
        '------------------------
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""
        '-
        ImpostaFiltro()
        aDataViewScAtt = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)

        Dim dsDoc As New DSDocumenti
        If (aDataViewScAtt Is Nothing) Then
            Session("dsDocElencoScad") = dsDoc
            aDataViewScAtt = New DataView(dsDoc.ScadAtt)
            Session(sessDataViewScAtt) = aDataViewScAtt
            BuidDett()
            Exit Sub
        End If
        Dim ObjDB As New DataBaseUtility
        If chkTutteRegioni.Checked = False Then
            'giu090520 filtro REGIONE/PROVINCIA
            strFiltroRicerca += " - Regione: " + ddlRegioni.SelectedItem.Text.Trim
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, "SELECT * FROM Province", dsDoc, "Province")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento Province: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, "SELECT * FROM Regioni", dsDoc, "Regioni")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento Regioni: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else

        End If
        ObjDB = Nothing
        '--------
        ' ''Dim rowRegioni As DSDocumenti.RegioniRow
        Dim rowProvince As DSDocumenti.ProvinceRow
        Dim dc As DataColumn
        'giu110423 per selezionare le scadenze oltre a quelle richieste ma nello stesso periodo
        Dim myIDDocumenti As Long = -1
        Dim myDurataNumRiga As Integer = -1
        '--------------------------------------------------------------------------------------
        Try
            If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex > 0 Then
                aDataViewScAtt.RowFilter = "PrApp='" & ddlProvince.SelectedValue.Trim & "'"
                strFiltroRicerca += " - Provincia: (" + ddlProvince.SelectedValue.Trim + ")"
            End If
            If txtRicerca.Text.Trim <> "" Then
                strFiltroRicerca += " - Ricerca per: " + ddlRicerca.SelectedItem.Text.Trim + " (" + txtRicerca.Text.Trim + ")"
            End If
            Dim myRegione As Integer = 0
            If chkTutteProvince.Checked = True Then
                If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex > 0 Then
                    myRegione = ddlRegioni.SelectedValue
                End If
            End If
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = "IDDocumenti,DurataNumRiga,SerieLotto,DataSc" 'GIU110423
            For i = 0 To aDataViewScAtt.Count - 1
                'giu110423 per selezionare le scadenze oltre a quelle richieste ma nello stesso periodo
                If chkScadAnno.Checked = True Then
                    If CDate(aDataViewScAtt.Item(i)("DataSc")) > CDate(txtAllaData.Text) Then
                        If myIDDocumenti <> aDataViewScAtt.Item(i)("IDDocumenti") Then
                            Continue For
                        ElseIf myDurataNumRiga = aDataViewScAtt.Item(i)("DurataNumRiga") Then
                            'ok lo prendo comunque
                        Else
                            Continue For
                        End If
                    Else
                        myIDDocumenti = aDataViewScAtt.Item(i)("IDDocumenti")
                        myDurataNumRiga = aDataViewScAtt.Item(i)("DurataNumRiga")
                    End If
                End If
                '--------------------------------------------------------------------------------------
                If myRegione > 0 Then
                    rowProvince = dsDoc.Province.FindByCodice(aDataViewScAtt.Item(i)("PrApp"))
                    If rowProvince Is Nothing Then
                        Continue For
                    ElseIf rowProvince.Regione <> myRegione Then
                        Continue For
                    End If
                End If
                'giu210622 
                If CheckRespVisite.Checked And DDLRespVisite.SelectedIndex > 0 And DDLRespVisite.SelectedItem.Text.Trim <> "" Then
                    If DDLRespVisite.SelectedItem.Text.Trim <> aDataViewScAtt.Item(i)("DesRespVisite").ToString.Trim Then
                        Continue For
                    End If
                End If
                '---------
                Dim newRow As DSDocumenti.ScadAttRow = dsDoc.ScadAtt.NewScadAttRow
                newRow.BeginEdit()
                For Each dc In dsDoc.ScadAtt.Columns
                    If UCase(dc.ColumnName) = "NOSTAMPA" Then
                        newRow.Item(dc.ColumnName) = False
                    ElseIf UCase(dc.ColumnName) = "FILTRORICERCA" Then
                        newRow.Item(dc.ColumnName) = strFiltroRicerca.Trim
                    ElseIf UCase(dc.ColumnName) = "SWNOVISITA" Then
                        newRow.Item(dc.ColumnName) = 0
                    ElseIf UCase(dc.ColumnName) = "SWSINGOLO" Then
                        newRow.Item(dc.ColumnName) = SWSingolo
                    Else
                        If IsDBNull(aDataViewScAtt.Item(i)(dc.ColumnName)) Then
                            newRow.Item(dc.ColumnName) = DBNull.Value
                        Else
                            newRow.Item(dc.ColumnName) = aDataViewScAtt.Item(i)(dc.ColumnName)
                        End If
                    End If
                Next
                '-
                newRow.EndEdit()
                dsDoc.ScadAtt.AddScadAttRow(newRow)
            Next
            dsDoc.ScadAtt.AcceptChanges()
            'giu130520 secondo passaggio accorpa scadenze nell'anno
            'serve quando si richiede la stampa
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Seleziona elenco scadenze: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-
        Session("dsDocElencoScad") = dsDoc
        aDataViewScAtt = New DataView(dsDoc.ScadAtt)
        Session(sessDataViewScAtt) = aDataViewScAtt
        '---------------------------------
        BuidDett()
    End Sub
    'giu200617
    Private Sub ImpostaFiltro()
        SqlDSPrevTElenco.FilterExpression = ""
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
               ddlRicerca.SelectedValue = "DI" Or
               ddlRicerca.SelectedValue = "DF" Or
               ddlRicerca.SelectedValue = "DA" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim <> "" Then
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
            ElseIf ddlRicerca.SelectedValue = "DI" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataInizio >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataInizio = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "DF" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataFine >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataFine = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "DA" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataAccetta >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataAccetta = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
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
            ElseIf ddlRicerca.SelectedValue = "NS" Then
                If SqlDSPrevTElenco.FilterExpression <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Serie like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Serie = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            End If
        End If
        'giu010520
        If DDLRespArea.SelectedIndex > 0 And checkRespArea.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(RespArea =" & DDLRespArea.SelectedValue.ToString.Trim & ")"
        End If
        If DDLRespVisite.SelectedIndex > 0 And CheckRespVisite.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            'giu210622 il respVisite è presente piu volte xke collegato a piu RespArea
            'SqlDSPrevTElenco.FilterExpression += "(RespVisite =" & DDLRespVisite.SelectedValue.ToString.Trim & ")"
            SqlDSPrevTElenco.FilterExpression += "(DesRespVisite ='" & DDLRespVisite.SelectedItem.Text.Trim & "')"
        End If
    End Sub
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(SWOP) = SWOPNESSUNA
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Gestione CONTRATTI")
    End Sub
    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(SWOP) = SWOPMODIFICA
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Gestione CONTRATTI")
    End Sub
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewCA"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo CONTRATTO Cliente", "Confermi la creazione CONTRATTO Cliente ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewCA()
        Session(SWOP) = SWOPNUOVO
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
        Session(CSTTIPODOCSEL) = SWTD(TD.ContrattoAssistenza)
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(IDDOCUMENTI) = ""
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Nuovo CONTRATTO")
    End Sub
    'giu020320
    Private Sub btnNuovoDaOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovoDaOC.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewCADaOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo CONTRATTO Cliente da Ordine", "Confermi la creazione CONTRATTO Cliente da Ordine ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewCADaOC()
        Session(SWOP) = SWOPNUOVO
        Session(CSTNUOVOCADAOC) = SWSI
        Session(CSTNUOVOCADACA) = SWNO
        Session(CSTTIPODOCSEL) = SWTD(TD.ContrattoAssistenza)
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(IDDOCUMENTI) = ""
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Nuovo CONTRATTO da Ordine")
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
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
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(SWOP) = SWOPELIMINA
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Elimina CONTRATTO")
    End Sub

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        SetLnk()
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        Session(IDDOCUMENTI) = ""
        aDataViewScAtt = Session(sessDataViewScAtt)
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        GridViewPrevT.SelectedIndex = -1
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                'GIU020323
                ' ''Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                ' ''Dim row As GridViewRow = GridViewPrevT.SelectedRow
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        SetLnk()
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        aDataViewScAtt = Session(sessDataViewScAtt)
        GridViewPrevT.DataSource = aDataViewScAtt
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                'GIU020323
                ' ''Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                ' ''Dim row As GridViewRow = GridViewPrevT.SelectedRow
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevT.Sorting
        Dim sortExpression As String = TryCast(ViewState("_GridView1LastSortExpression_"), String)
        Dim sortDirection As String = TryCast(ViewState("_GridView1LastSortDirection_"), String)

        If e.SortExpression <> sortExpression Then
            sortExpression = e.SortExpression
            sortDirection = "ASC"
        Else

            If sortDirection = "ASC" Then
                sortExpression = e.SortExpression
                sortDirection = "DESC"
            Else
                sortExpression = e.SortExpression
                sortDirection = "ASC"
            End If
        End If

        Try
            ViewState("_GridView1LastSortDirection_") = sortDirection
            ViewState("_GridView1LastSortExpression_") = sortExpression
            aDataViewScAtt = Session(sessDataViewScAtt)
            aDataViewScAtt.Sort = ""
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = sortExpression & " " + sortDirection
            GridViewPrevT.DataSource = aDataViewScAtt
            GridViewPrevT.DataBind()
        Catch
        End Try
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        SetLnk()
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato)

        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataSc).Text) Then
                e.Row.Cells(CellIdxT.DataSc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataSc).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxT.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxT.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxT.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxT.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxT.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.Importo).Text = ""
                End If
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
            If Len(e.Row.Cells(CellIdxT.DesArt).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.DesArt).Text = Mid(e.Row.Cells(CellIdxT.DesArt).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.RagSoc).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.RagSoc).Text = Mid(e.Row.Cells(CellIdxT.RagSoc).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Denom).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Denom).Text = Mid(e.Row.Cells(CellIdxT.Denom).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Riferimento).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Riferimento).Text = Mid(e.Row.Cells(CellIdxT.Riferimento).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest1).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest1).Text = Mid(e.Row.Cells(CellIdxT.Dest1).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest2).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest2).Text = Mid(e.Row.Cells(CellIdxT.Dest2).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxT.Dest3).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxT.Dest3).Text = Mid(e.Row.Cells(CellIdxT.Dest3).Text, 1, 20)
            End If
        End If
    End Sub

    Private Sub btnVerbale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerbale.Click
        SetLnk()
        Session(CSTTASTOST) = btnVerbale.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Verbale
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 1
                Session(CSTNOBACK) = 0 'giu040512
                Dim SWSviluppo As String = ConfigurationManager.AppSettings("sviluppo")
                If Not String.IsNullOrEmpty(SWSviluppo) Then
                    If SWSviluppo.Trim.ToUpper = "TRUE" Then
                        Response.Redirect("WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                        Exit Sub
                    End If
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnStampa.Click
        SetLnk()
        Session(CSTTASTOST) = btnStampa.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Proforma
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 'giu040512
                Dim SWSviluppo As String = ConfigurationManager.AppSettings("sviluppo")
                If Not String.IsNullOrEmpty(SWSviluppo) Then
                    If SWSviluppo.Trim.ToUpper = "TRUE" Then
                        Response.Redirect("WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                        Exit Sub
                    End If
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub
    '-
    Private Sub btnElencoSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElencoSc.Click
        SetLnk()
        Dim SWSingolo As Boolean = False
        If DDLRespArea.SelectedIndex < 1 Then
            SWSingolo = False
        End If
        Session(CSTTASTOST) = btnElencoSc.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivita
        Session("TipoDocInStampa") = SWTD(TD.ContrattoAssistenza)
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim SWErrore As Boolean = False
        txtDallaData.Text = ConvData(txtDallaData.Text.ToString.Trim)
        If txtDallaData.Text.Trim.Length <> 10 Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtDallaData.Text.Trim) Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtDallaData.BackColor = SEGNALA_OK
        End If
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        '-
        txtAllaData.Text = ConvData(txtAllaData.Text.ToString.Trim)
        If txtAllaData.Text.Trim.Length <> 10 Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtAllaData.BackColor = SEGNALA_OK
        End If
        '------------------------------
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        If checkCausale.Checked = True And DDLCausali.SelectedIndex < 1 Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali.Focus()
            Exit Sub
        End If
        If checkRespArea.Checked = True And DDLRespArea.SelectedIndex < 1 Then
            DDLRespArea.BackColor = SEGNALA_KO : DDLRespArea.Focus()
            Exit Sub
        End If
        If CheckRespVisite.Checked = True And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex < 1 Then
            ddlRegioni.BackColor = SEGNALA_KO : ddlRegioni.Focus()
            Exit Sub
        End If
        If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex < 1 Then
            ddlProvince.BackColor = SEGNALA_KO : ddlProvince.Focus()
            Exit Sub
        End If
        '---------
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        If checkCausale.Checked = True And DDLCausali.SelectedIndex < 1 Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali.Focus()
            Exit Sub
        End If
        If checkRespArea.Checked = True And DDLRespArea.SelectedIndex < 1 Then
            DDLRespArea.BackColor = SEGNALA_KO : DDLRespArea.Focus()
            Exit Sub
        End If
        If CheckRespVisite.Checked = True And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex < 1 Then
            ddlRegioni.BackColor = SEGNALA_KO : ddlRegioni.Focus()
            Exit Sub
        End If
        If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex < 1 Then
            ddlProvince.BackColor = SEGNALA_KO : ddlProvince.Focus()
            Exit Sub
        End If
        '---------
        If GridViewPrevT.Rows.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare. Ricaricare i dati con il tasto ""Visualizza Scadenze""", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session("dsDocElencoScad") Is Nothing Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare. Ricaricare i dati con il tasto ""Visualizza Scadenze""", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSDocumenti
        DsPrinWebDoc = Session("dsDocElencoScad")
        Dim DsDocumentiSave As New DSDocumenti
        DsDocumentiSave = DsPrinWebDoc.Copy()

        Dim StrErrore As String = ""
        'GIU160520 PER SAPERE I CODICI VISITA
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = "Select * From TipoContratto"
        Dim myCodVisita As String = "" : Dim Comodo As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsPrinWebDoc, "TipoContratto")
            For Each rsTC In DsPrinWebDoc.Tables("TipoContratto").Select("")
                Comodo = IIf(IsDBNull(rsTC!Descrizione), "", rsTC!CodVisita.ToString.Trim)
                If InStr(myCodVisita, Comodo.Trim) > 0 Then
                Else
                    myCodVisita += Comodo.Trim + ","
                End If
            Next
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento Tabella TipoContratto. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        ObjDB = Nothing
        'giu291023 Clienti bloccati
        Dim ArrClienti As ArrayList
        ArrClienti = GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
        Dim ClientePrec As String = "" : Dim OKDel As Boolean = False
        Dim SWClientiBloccati As Boolean = False
        Dim strElencoClientiBloccati As String = ""
        Dim RowScadAttD As DSDocumenti.ScadAttRow
        For Each RowScadAttD In DsPrinWebDoc.ScadAtt.Select("", "Cod_Cliente")
            If ClientePrec <> RowScadAttD.Cod_Cliente.Trim Then
                ClientePrec = RowScadAttD.Cod_Cliente.Trim
                OKDel = False
                Try
                    Dim trovato = From x In ArrClienti Where x.Codice_CoGe.ToString.Trim.Equals(ClientePrec.Trim)
                    If trovato(0).NoFatt.ToString.Trim <> "0" And trovato(0).Codice_CoGe.ToString.Trim = ClientePrec.Trim Then
                        SWClientiBloccati = True
                        strElencoClientiBloccati += " -  " + ClientePrec.Trim + " - " + trovato(0).Rag_Soc.ToString.Trim
                        If chkIncludiCliBlocco.Checked = False Then
                            RowScadAttD.Delete()
                            OKDel = True
                        End If
                    Else
                        OKDel = False
                    End If
                Catch ex As Exception
                    OKDel = False
                End Try
            Else
                If chkIncludiCliBlocco.Checked = False And OKDel = True Then
                    RowScadAttD.Delete()
                End If
            End If
        Next
        If chkIncludiCliBlocco.Checked = False And SWClientiBloccati = True Then
            DsPrinWebDoc.ScadAtt.AcceptChanges()
        End If
        '..........................
        '-----------------------------------------------------------------
        'giu130520 accorpa scadenze nell'anno SOLO quando ci sono 2 visite nell'anno
        Dim findRow As DSDocumenti.ScadAttRow
        Dim RowScadAtt As DSDocumenti.ScadAttRow
        For Each RowScadAtt In DsPrinWebDoc.ScadAtt.Select("", "DataSc,Cod_Cliente,SerieLotto")

            If InStr(myCodVisita, RowScadAtt.Cod_Articolo.Trim) > 0 Then
                RowScadAtt.BeginEdit()
                RowScadAtt.SWNoVisita = 0
                RowScadAtt.SWSingolo = SWSingolo
                RowScadAtt.EndEdit()
            Else
                RowScadAtt.BeginEdit()
                RowScadAtt.SWNoVisita = 1
                RowScadAtt.SWSingolo = SWSingolo
                RowScadAtt.EndEdit()
                Continue For
            End If
            If RowScadAtt.NoStampa = True Then
                Continue For
            ElseIf Not IsDBNull(RowScadAtt.CodArt2V) Then
                If RowScadAtt.CodArt2V.Trim <> "" Then
                    Continue For
                End If
            ElseIf Not IsDBNull(RowScadAtt.DesArt2V) Then
                If RowScadAtt.DesArt2V.Trim <> "" Then
                    Continue For
                End If
            End If

            '-
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + RowScadAtt.Cod_Cliente + "' AND SerieLotto='" + RowScadAtt.SerieLotto + "'", "DataSc,Cod_Cliente,SerieLotto")
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then

                Else
                    Continue For
                End If
                If findRow.DurataNumRiga = RowScadAtt.DurataNumRiga And findRow.Riga = RowScadAtt.Riga Then
                    Continue For
                End If
                If findRow.DurataNumRiga = RowScadAtt.DurataNumRiga Then
                Else
                    Continue For
                End If
                If findRow.NoStampa = True Then
                    Continue For
                End If
                If Not IsDBNull(findRow.CodArt2V) Then
                    If findRow.CodArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                If Not IsDBNull(findRow.DesArt2V) Then
                    If findRow.DesArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                '-
                If RowScadAtt.NoStampa = True Then
                    Continue For
                End If
                If Not IsDBNull(RowScadAtt.CodArt2V) Then
                    If RowScadAtt.CodArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                If Not IsDBNull(RowScadAtt.DesArt2V) Then
                    If RowScadAtt.DesArt2V.Trim <> "" Then
                        Continue For
                    End If
                End If
                '-
                If RowScadAtt.DataSc.Year <> findRow.DataSc.Year Then
                    Continue For
                End If
                '-
                RowScadAtt.BeginEdit()
                RowScadAtt.CodArt2V = findRow.Cod_Articolo
                RowScadAtt.DesArt2V = findRow.Descrizione
                RowScadAtt.DataSc2V = findRow.DataSc
                RowScadAtt.EndEdit()
                '-
                findRow.BeginEdit()
                findRow.NoStampa = True
                findRow.EndEdit()
                Exit For
            Next
        Next

        'giu240520 accorpo per SerieLotto la data di scadenza al CKECK
        Dim RowScadAttS As DSDocumenti.ScadAttRow
        Dim mySerieLotto As String = "Z" : Dim myCodCliente As String = "Z" : Dim myDataScCK As String = "" : Dim myDurataNumRiga As Integer = -1
        Dim SWAgg As Boolean = True
        For Each RowScadAttS In DsPrinWebDoc.ScadAtt.Select("", "DurataNumRiga,SerieLotto,SWNoVisita,Cod_Cliente")
            If mySerieLotto <> RowScadAttS.SerieLotto Or myCodCliente <> RowScadAttS.Cod_Cliente Or myDurataNumRiga <> RowScadAttS.DurataNumRiga Then
                mySerieLotto = RowScadAttS.SerieLotto
                myCodCliente = RowScadAttS.Cod_Cliente
                myDurataNumRiga = RowScadAttS.DurataNumRiga
                SWAgg = True
                myDataScCK = ""
            End If
            '-
            If InStr(myCodVisita, RowScadAttS.Cod_Articolo.Trim) > 0 Then
                myDataScCK = Format(RowScadAttS.DataSc, FormatoData)
                SWAgg = False
            Else
                Continue For
            End If
            If SWAgg = True Then
                Continue For
            End If
            '-
            SWAgg = True
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + RowScadAttS.Cod_Cliente + "' AND SerieLotto='" + RowScadAttS.SerieLotto + "' AND DurataNumRiga=" + RowScadAttS.DurataNumRiga.ToString.Trim)
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then
                    Continue For
                End If
                '-
                findRow.BeginEdit()
                findRow.DataSc = CDate(myDataScCK)
                findRow.EndEdit()
            Next
            myDataScCK = ""
        Next
        If SWAgg = False And myDataScCK.Trim <> "" Then
            For Each findRow In DsPrinWebDoc.ScadAtt.Select("Cod_Cliente='" + myCodCliente + "' AND SerieLotto='" + mySerieLotto + "' AND DurataNumRiga=" + myDurataNumRiga.ToString.Trim)
                If InStr(myCodVisita, findRow.Cod_Articolo.Trim) > 0 Then
                    Continue For
                End If
                '-
                findRow.BeginEdit()
                findRow.DataSc = myDataScCK
                findRow.EndEdit()
            Next
        End If
        '-----------
        DsPrinWebDoc.AcceptChanges()
        '-------------------------------------------------------
        Session(CSTDsPrinWebDoc) = DsPrinWebDoc
        Session(CSTNOBACK) = 0
        '-
        If chkIncludiCliBlocco.Checked = False And SWClientiBloccati = True Then
            Session("dsDocElencoScad") = DsDocumentiSave
            aDataViewScAtt = New DataView(DsDocumentiSave.ScadAtt)
            Session(sessDataViewScAtt) = aDataViewScAtt
            '---------------------------------
            BuidDett()
        End If
        Call OKApriStampaElScadCA(DsPrinWebDoc, strElencoClientiBloccati)

    End Sub
    '----------
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

        If Not (sTipoUtente.Equals(CSTAMMINISTRATORE)) And
            Not (sTipoUtente.Equals(CSTTECNICO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------
        If CKCSTTipoDoc() = False Then
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
        ModalPopup.Show("Sblocca Documento", "Confermi lo sblocco del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub SbloccaDoc()
        If CKCSTTipoDoc() = False Then
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
            If btnCambiaStato.Enabled = False Then btnCambiaStato.Enabled = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub

    'giu260612 CAMBIO STATO DOCUMENTO
    Private Sub btnCambiaStato_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCambiaStato.Click
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
            'ok PROCEDO
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        WFPCambiaStatoCA.WucElement = Me
        WFPCambiaStatoCA.SetLblMessUtente("Seleziona/modifica dati")
        Session(F_CAMBIOSTATO_APERTA) = True
        WFPCambiaStatoCA.Show(True)
    End Sub
    Public Sub CancBackWFPCambiaStatoCA()
        'nulla
    End Sub
    Public Sub CallBackWFPCambiaStatoCA()
        btnSblocca.Visible = False
    End Sub

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(SWOP) = SWOPNESSUNA
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
#End Region

    'GIU210120
    Private Sub OKApriStampa(ByRef DsPrinWebDoc As DSPrintWeb_Documenti)
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        ' ''If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
        ' ''    If Session(CSTNOBACK) = 1 Then
        ' ''        btnRitorno.Visible = False
        ' ''        Label1.Visible = False
        ' ''    End If
        ' ''End If
        Dim SWSconti As Integer = 1
        If Not String.IsNullOrEmpty(Session(CSTSWScontiDoc)) Then
            If IsNumeric(Session(CSTSWScontiDoc)) Then
                SWSconti = Session(CSTSWScontiDoc)
            End If
        End If
        '-
        Dim SWConfermaDoc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWConfermaDoc)) Then
            If IsNumeric(Session(CSTSWConfermaDoc)) Then
                SWConfermaDoc = Session(CSTSWConfermaDoc)
            End If
        End If
        'giu110319
        Dim SWRitAcc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWRitAcc)) Then
            If IsNumeric(Session(CSTSWRitAcc)) Then
                SWRitAcc = Session(CSTSWRitAcc)
            End If
        End If
        '---------
        Dim Rpt As Object = Nothing
        ' ''Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        ' ''DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ContrattiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu120319
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Select("RitAcconto=true").Count > 0) Then
                Session(CSTSWRitAcc) = 1
                SWRitAcc = 1
            Else
                Session(CSTSWRitAcc) = 0
                SWRitAcc = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("ContrattiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
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
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
            NomeStampa = "PREVOFF.PDF"
            SubDirDOC = "Preventivi"
            If SWSconti = 1 Then
                Rpt = New Preventivo
                If CodiceDitta = "01" Then
                    Rpt = New Preventivo01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Preventivo05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Preventivo0501
                End If
            Else
                Rpt = New PreventivoNOSconti
                If CodiceDitta = "01" Then
                    Rpt = New PreventivoNOSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New PreventivoNOSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New PreventivoNOSconti0501
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
            NomeStampa = "ORDINE.PDF"
            SubDirDOC = "Ordini"
            If SWSconti = 1 Then
                Rpt = New Ordine
                If CodiceDitta = "01" Then
                    Rpt = New Ordine01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Ordine05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Ordine0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New Ordine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New Ordine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New Ordine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New Ordine0501
                '''    End If
                '''Else
                '''    NomeStampa = "CONFORDINE.PDF"
                '''    Rpt = New ConfermaOrdine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdine0501
                '''    End If
                '''End If
            Else
                Rpt = New OrdineNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New OrdineNoSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New OrdineNoSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New OrdineNoSconti0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New OrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New OrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New OrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New OrdineNoSconti0501
                '''    End If
                '''Else
                '''    NomeStampa = "CONFORDINE.PDF"
                '''    Rpt = New ConfermaOrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdineNoSconti0501
                '''    End If
                '''End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
            SubDirDOC = "DDTClienti"
            NomeStampa = "DDTCLIENTE.PDF"
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New DDTNoPrezzi05
                Else
                    Rpt = New DDTNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
            SubDirDOC = "DDTFornit"
            NomeStampa = "DDTFORNIT.PDF"
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New DDTNoPrezzi05
                Else
                    Rpt = New DDTNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
            NomeStampa = "FATTURA.PDF"
            SubDirDOC = "Fatture"
            'giu251211
            If SWSconti = 1 Then
                Rpt = New Fattura
                If CodiceDitta = "01" Then
                    Rpt = New Fattura01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New Fattura05
                    Else
                        Rpt = New Fattura05LT
                    End If
                    '-
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Fattura0501
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                End If
            Else
                Rpt = New FatturaNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New FatturaNoSconti01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New FatturaNoSconti05
                    Else
                        Rpt = New FatturaNoSconti05LT
                    End If
                    '-
                    If SWRitAcc <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New FatturaNoSconti0501
                    If SWRitAcc = True <> 0 Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05RA
                        Else
                            Rpt = New Fattura05RALT
                        End If
                    End If
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            NomeStampa = "NOTACREDITO.PDF"
            SubDirDOC = "NoteCredito"
            Rpt = New NotaCredito
            If CodiceDitta = "01" Then
                Rpt = New NotaCredito01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New NotaCredito05
                Else
                    Rpt = New NotaCredito05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New NotaCredito0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
            NomeStampa = "ORDINEFOR.PDF"
            SubDirDOC = "Ordini"
            Rpt = New OrdineFornitore
            If CodiceDitta = "01" Then
                Rpt = New OrdineFornitore01
            ElseIf CodiceDitta = "05" Then
                Rpt = New OrdineFornitore05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New OrdineFornitore0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
            NomeStampa = "MOVMAG.PDF"
            SubDirDOC = "MovMag"
            Rpt = New MMNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New MMNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                If SWStampaDocLotti = False Then
                    Rpt = New MMNoPrezzi05
                Else
                    Rpt = New MMNoPrezzi05LT
                End If
            ElseIf CodiceDitta = "0501" Then
                Rpt = New MMNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            If Session(CSTTASTOST) = btnStampa.ID Then
                NomeStampa = "PROFORMACA.PDF"
                Rpt = New ProformaCA05 'Contratti
                If CodiceDitta = "01" Then
                    Rpt = New ProformaCA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ProformaCA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ProformaCA05 '0501
                End If
            ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
                NomeStampa = "VERBALE.PDF"
                Rpt = New VerbaleVACA05
                If CodiceDitta = "01" Then
                    Rpt = New VerbaleVACA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New VerbaleVACA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New VerbaleVACA05 '0501
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or
            Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale STAMPA DOCUMENTO DA COMPLETARE"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        Else
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'GIU210120 ESEGUITO IN CKSTTipoDocST
        'Per evitare che solo un utente possa elaborare le stampe
        ' ''Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        ' ''If (Utente Is Nothing) Then
        ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
        ' ''    Exit Sub
        ' ''End If
        Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
        'giu210324
        getOutputRPT(Rpt, "PDF")
        '---------
        ''''giu140615 prova con binary 
        ''''' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        '''Session(CSTESPORTAPDF) = True
        '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        '''Dim stPathReport As String = Session(CSTPATHPDF)
        '''Try 'giu281112 errore che il file Ã¨ gia aperto
        '''    Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    'giu140124
        '''    Rpt.Close()
        '''    Rpt.Dispose()
        '''    Rpt = Nothing
        '''    '-
        '''    GC.WaitForPendingFinalizers()
        '''    GC.Collect()
        '''    '-------------
        '''Catch ex As Exception
        '''    Rpt = Nothing
        '''    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '''    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '''    ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
        '''    Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
        '''    Exit Sub
        '''End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            LnkVerbale.Visible = True
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True
        End If
        'giu210324
        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
        '''    LnkVerbale.HRef = LnkName
        '''    ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
        '''    ' ''    LnkListaCarico.HRef = LnkName
        '''Else
        '''    LnkStampa.HRef = LnkName
        '''End If
    End Sub
    'giu120520
    Private Sub OKApriStampaElScadCA(ByRef DsPrinWebDoc As DSDocumenti, ByVal strElencoClientiBloccati As String)

        Dim SWTabCliFor As String = ""
        Dim Rpt As Object = Nothing
        Try
            If (DsPrinWebDoc.Tables("ScadAtt").Rows.Count > 0) Then
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ScadAtt").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Elenco Scadenze", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
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
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            If Session(CSTTASTOST) = btnElencoSc.ID Then
                If chkVisElenco.Checked Then
                    If rbtnOrdScadenza.Checked Then
                        NomeStampa = "ELENCOSCAD_Ord_Scadenza.PDF"
                    Else
                        NomeStampa = "ELENCOSCAD_Ord_Cliente.PDF"
                    End If
                Else
                    If rbtnOrdScadenza.Checked Then
                        NomeStampa = "ELENCOSCAD_Ord_Scadenza.XLS"
                    Else
                        NomeStampa = "ELENCOSCAD_Ord_Cliente.XLS"
                    End If
                End If
                '-
                If rbtnOrdScadenza.Checked Then
                    Rpt = New ElencoScadCA05 'Contratti
                    If CodiceDitta = "01" Then
                        Rpt = New ElencoScadCA05 '01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New ElencoScadCA05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New ElencoScadCA05 '0501
                    End If
                Else
                    Rpt = New ElencoScadCAOrdCli05 'Contratti
                    If CodiceDitta = "01" Then
                        Rpt = New ElencoScadCAOrdCli05 '01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New ElencoScadCAOrdCli05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New ElencoScadCAOrdCli05 '0501
                    End If
                End If

            ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Tipo stampa non definita", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'riporto il nome del RESP.AREA nel nome del file creato
        Dim strRespArea As String = DDLRespArea.SelectedItem.Text.Trim
        strRespArea = strRespArea.ToString.Replace(",", " ")
        strRespArea = strRespArea.ToString.Replace(".", " ")
        Dim strDalAl As String = txtDallaData.Text + "_" + txtAllaData.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        If strRespArea.Trim <> "" Then
            Session(CSTNOMEPDF) = strRespArea.Trim & "_" & strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        Else
            Session(CSTNOMEPDF) = strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        End If
        '---------
        'giu210324
        If chkVisElenco.Checked Then
            getOutputRPT(Rpt, "PDF")
        Else
            getOutputRPT(Rpt, "EXCEL")
        End If
        '---------
        ''''giu140615 prova con binary 
        ''''' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        '''Session(CSTESPORTAPDF) = True
        '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        '''Dim stPathReport As String = Session(CSTPATHPDF)
        '''Try 'giu281112 errore che il file Ã¨ gia aperto
        '''    If chkVisElenco.Checked Then
        '''        Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    Else
        '''        Rpt.ExportToDisk(ExportFormatType.ExcelRecord, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    End If
        '''    'giu140124
        '''    Rpt.Close()
        '''    Rpt.Dispose()
        '''    Rpt = Nothing
        '''    '-
        '''    GC.WaitForPendingFinalizers()
        '''    GC.Collect()
        '''    '-------------
        '''Catch ex As Exception
        '''    Rpt = Nothing
        '''    Chiudi("Errore in esporta: " & Session(CSTNOMEPDF) & " " & ex.Message)
        '''    Exit Sub
        '''End Try
        If Session(CSTTASTOST) = btnElencoSc.ID Then
            lnkElenco.Visible = True 'giu280224 : btnOKModInviati.Visible = True
        Else
            lnkElenco.Visible = True 'giu280224 : btnOKModInviati.Visible = True
        End If

        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnElencoSc.ID Then
        '''    lnkElencoSc.HRef = LnkName
        '''Else
        '''    lnkElencoSc.HRef = LnkName
        '''End If
        If strElencoClientiBloccati.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'btnRicerca.BackColor = SEGNALA_KO
            ModalPopup.Show("Attenzione", strElencoClientiBloccati.Trim + "<br>Sono presenti Clienti Bloccati. " + IIf(chkIncludiCliBlocco.Checked = True, "(Inclusi nella stampa)", "(Esclusi dalla stampa) "), WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub

    '@@@@@
    Private Function getOutputRPT(ByVal _Rpt As Object, ByVal _Formato As String) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            If _Formato = "PDF" Then
                myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Else
                myStream = _Rpt.ExportToStream(ExportFormatType.Excel)
            End If
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("WebFormStampe") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            _Rpt.Close()
            _Rpt.Dispose()
            _Rpt = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function
    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    '@@@@@

    Public Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
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

    Private Sub checkRespArea_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkRespArea.CheckedChanged
        SvuodaGridT()
        SetLnk()
        If checkRespArea.Checked = True Then
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = True : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
        Else
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = False : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
            DDLRespArea.BackColor = SEGNALA_OK
        End If
        Session(IDRESPAREA) = 0
        Call DataBindRV()
        If checkRespArea.Checked = True Then
            If DDLRespArea.Items.Count > 0 Then 'GIU280223
                DDLRespArea.SelectedIndex = 1
                Session(IDRESPAREA) = DDLRespArea.SelectedValue
                PosizionaItemDDL(Session(IDRESPAREA).ToString.Trim, DDLRespArea)
                Call DDLRespArea_SelectedIndexChanged(Nothing, Nothing)
            End If
            DDLRespArea.Focus()
        End If
    End Sub
    Private Sub CheckRespVisite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckRespVisite.CheckedChanged
        SvuodaGridT()
        SetLnk()
        If CheckRespVisite.Checked = True Then
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = True : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
        Else
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = False : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
            DDLRespVisite.BackColor = SEGNALA_OK
        End If
        If CheckRespVisite.Checked = True Then
            If DDLRespVisite.Items.Count > 0 Then 'GIU280223
                DDLRespVisite.SelectedIndex = 1
                Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
                PosizionaItemDDL(Session(IDRESPVISITE).ToString.Trim, DDLRespVisite)
                Call DDLRespVisite_SelectedIndexChanged(Nothing, Nothing)
            End If
            DDLRespVisite.Focus()
        End If
    End Sub
    Private Sub DDLRespArea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespArea.SelectedIndexChanged
        SvuodaGridT()
        If DDLRespArea.SelectedIndex = 0 Or DDLRespArea.SelectedItem.Text.Trim = "" Then
            Session(IDRESPAREA) = 0
        Else
            Session(IDRESPAREA) = DDLRespArea.SelectedValue
        End If
        Call DataBindRV()
        '--
        If DDLRespArea.SelectedIndex = 0 Then
            DDLRespArea.BackColor = SEGNALA_KO
        Else
            DDLRespArea.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub DataBindRV()
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        DDLRespVisite.BackColor = SEGNALA_OK
        '-- mi riposiziono 
        DDLRespVisite.AutoPostBack = False
        DDLRespVisite.SelectedIndex = -1
        DDLRespVisite.AutoPostBack = True
        If CheckRespVisite.Checked = True Then
            If DDLRespVisite.Items.Count > 0 Then 'GIU280223
                DDLRespVisite.SelectedIndex = 1
                Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
                PosizionaItemDDL(Session(IDRESPVISITE).ToString.Trim, DDLRespVisite)
                Call DDLRespVisite_SelectedIndexChanged(Nothing, Nothing)
            End If
            '' 'DDLRespVisite.Focus()
        End If
    End Sub
    Private Sub DDLRespVisite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespVisite.SelectedIndexChanged
        SvuodaGridT()
        Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
        If DDLRespVisite.SelectedIndex = 0 Then
            DDLRespVisite.BackColor = SEGNALA_KO
        Else
            DDLRespVisite.BackColor = SEGNALA_OK
        End If
    End Sub
    '-
    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        SvuodaGridT()
        Session("CodRegione") = ddlRegioni.SelectedValue
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
        If ddlRegioni.SelectedIndex = 0 Then
            ddlRegioni.BackColor = SEGNALA_KO
        Else
            ddlRegioni.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub chkTutteRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        SvuodaGridT()
        If chkTutteRegioni.Checked Then
            ddlRegioni.Enabled = False
            ddlRegioni.SelectedIndex = -1
            ddlRegioni.BackColor = SEGNALA_OK
            Session("CodRegione") = 0
        Else
            ddlRegioni.Enabled = True
            Session("CodRegione") = ddlRegioni.SelectedValue
            If ddlRegioni.Items.Count > 0 Then 'GIU280223
                ddlRegioni.SelectedIndex = 1
                Session("CodRegione") = ddlRegioni.SelectedValue
                PosizionaItemDDL(Session("CodRegione").ToString.Trim, ddlRegioni)
                Call ddlRegioni_SelectedIndexChanged(Nothing, Nothing)
            End If
            ddlRegioni.Focus()
        End If
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
            ddlProvince.BackColor = SEGNALA_OK
        Else
            ddlProvince.Enabled = True
            If ddlProvince.Items.Count > 0 Then 'GIU280223
                ddlProvince.SelectedIndex = 1
                PosizionaItemDDL(ddlProvince.SelectedValue.ToString.Trim, ddlProvince)
                Call ddlProvince_SelectedIndexChanged(Nothing, Nothing)
            End If
        End If
    End Sub
    Private Sub chkTutteProvince_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteProvince.CheckedChanged
        SvuodaGridT()
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
            ddlProvince.BackColor = SEGNALA_OK
        Else
            ddlProvince.Enabled = True
            If ddlProvince.Items.Count > 0 Then 'GIU280223
                ddlProvince.SelectedIndex = 1
                PosizionaItemDDL(ddlProvince.SelectedValue.ToString.Trim, ddlProvince)
                Call ddlProvince_SelectedIndexChanged(Nothing, Nothing)
            End If
            ddlProvince.Focus()
        End If
    End Sub

    Private Sub ddlProvince_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProvince.SelectedIndexChanged
        SvuodaGridT()
        If ddlProvince.SelectedIndex = 0 Then
            ddlProvince.BackColor = SEGNALA_KO
        Else
            ddlProvince.BackColor = SEGNALA_OK
        End If
    End Sub

    '''Private Sub chkNoEscludiInvioM_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkNoEscludiInvioM.CheckedChanged
    '''    SvuodaGridT()
    '''End Sub

    Private Sub SetLnk()
        LnkStampa.Visible = False : LnkVerbale.Visible = False : lnkElenco.Visible = False : btnOKModInviati.Visible = False
        chkVisElenco.Visible = True
    End Sub

    Private Sub btnOKModInviati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOKModInviati.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If GridViewPrevT.Rows.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da aggiornare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKModInviati"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiorna Stato Attività programmate", "Confermi l'aggiornamento ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub OKModInviati()
        If Session(CSTDsPrinWebDoc) Is Nothing Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione dati scaduta, ricaricare i dati e riprovare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsDoc As New DSDocumenti
        DsDoc = Session(CSTDsPrinWebDoc)
        '-------------------------------
        Dim strErrore As String = ""
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        Dim SqlUpd_ConDSWInvioMod = New System.Data.SqlClient.SqlCommand
        SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        Dim strValore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '--- Parametri
        SqlUpd_ConDSWInvioMod.CommandText = "[update_ConDSWInvioMod]"
        SqlUpd_ConDSWInvioMod.CommandType = System.Data.CommandType.StoredProcedure
        SqlUpd_ConDSWInvioMod.Connection = SqlConn
        SqlUpd_ConDSWInvioMod.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
        SqlUpd_ConDSWInvioMod.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
        SqlUpd_ConDSWInvioMod.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"))
        SqlUpd_ConDSWInvioMod.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 8, "RefDataPrev"))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        Dim RowScadAtt As DSDocumenti.ScadAttRow
        Dim myDataInvioMod As DateTime = Now
        For Each RowScadAtt In DsDoc.ScadAtt.Select()
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Open Then
                    SqlConn.Open()
                    SqlConn = Nothing
                End If
            End If
            Try
                SqlUpd_ConDSWInvioMod.Parameters("@IDDocumenti").Value = RowScadAtt.IDDocumenti
                SqlUpd_ConDSWInvioMod.Parameters("@DurataNumRiga").Value = RowScadAtt.DurataNumRiga
                SqlUpd_ConDSWInvioMod.Parameters("@Riga").Value = RowScadAtt.Riga
                SqlUpd_ConDSWInvioMod.Parameters("@RefDataPrev").Value = myDataInvioMod
                SqlUpd_ConDSWInvioMod.CommandTimeout = myTimeOUT '5000
                SqlUpd_ConDSWInvioMod.ExecuteNonQuery()
            Catch exSQL As SqlException
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = exSQL.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento [SWInvioMod]: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch ex As Exception
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento [SWInvioMod]: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Next
        '''chkNoEscludiInvioM.AutoPostBack = False
        '''chkNoEscludiInvioM.Checked = False
        '''chkNoEscludiInvioM.AutoPostBack = True
        SvuodaGridT()
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiornamento stato Attività programmate", "Aggiornamento terminato con successo.", WUC_ModalPopup.TYPE_INFO)
        Exit Sub
    End Sub

    Private Sub rbtnOrdCliente_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnOrdCliente.CheckedChanged
        lnkElenco.Visible = False
    End Sub

    Private Sub rbtnOrdScadenza_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnOrdScadenza.CheckedChanged
        lnkElenco.Visible = False
    End Sub

    Private Sub chkVisElenco_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkVisElenco.CheckedChanged
        lnkElenco.Visible = False
    End Sub

    Private Sub chkScadAnno_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkScadAnno.CheckedChanged
        SetLnk()
    End Sub

    Private Sub chkIncludiCliBlocco_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIncludiCliBlocco.CheckedChanged
        SetLnk()
    End Sub

End Class