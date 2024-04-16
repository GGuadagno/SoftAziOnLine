Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino

Imports System.Web.Services.WebService
Imports System.Web.Script.Serialization
Imports System.IO
Partial Public Class WUC_Contratti
    Inherits System.Web.UI.UserControl

    Private DsDocT As New DSDocumenti
    Private dvDocT As DataView
    Private DsDocDettForInsert As New DSDocumenti
    Private DsDocDettForInsertL As New DSDocumenti 'GIU291111

    Private SqlConnDoc As SqlConnection
    Private SqlAdapDoc As SqlDataAdapter
    Private SqlAdapDocForInsert As SqlDataAdapter   'Pier311011
    Private SqlAdapDocForInsertL As SqlDataAdapter 'GIU291111

    Private SqlDbSelectCmd As SqlCommand
    Private SqlDbInserCmd As SqlCommand
    Private SqlDbUpdateCmd As SqlCommand
    Private SqlDbDeleteCmd As SqlCommand

    'giu250220
    Private SqlAdapDocALLAtt As SqlDataAdapter
    Private SqlDbSelectCmdALLAtt As SqlCommand

    Private SqlDbInserCmdForInsert As SqlCommand 'Pier311011
    Private SqlDbInserCmdForInsertL As SqlCommand 'GIU291111

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = "" 'giu260312
    Dim SWFatturaPA As Boolean = False 'giu230714
    Dim SWSplitIVA As Boolean = False 'giu221217
    Dim SWTB0 As Boolean = False
    Dim SWTB1 As Boolean = False
    Dim SWTB2 As Boolean = False
    Dim SWTB3 As Boolean = False
    'giu130219
    Dim SWRifDoc As Boolean = True
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    Private Const AGGPAGSCADTOT As String = "AGGPAGSCADTOT"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(ERROREALL) = ""
        ModalPopup.WucElement = Me
        WUC_ContrattiDett1.WucElement = Me
        WUC_ContrattiSpeseTraspTot1.WucElement = Me
        WFP_RespArea1.WucElement = Me
        WFP_RespVisite1.WucElement = Me
        WFP_Agenti1.WucElement = Me
        '-
        WFP_AnagrProvv_Insert1.WucElement = Me
        WFP_Anagrafiche_Modify1.WucElement = Me
        WFP_DestCliFor1.WucElement = Me
        WFPElencoCli.WucElement = Me
        WFPElencoFor.WucElement = Me
        WFPElencoDestCF.WucElement = Me
        WFP_BancheIBAN1.WucElement = Me
        WFPElencoAliquotaIVA.WucElement = Me
        'GIU060814 IN CKCSTTipoDoc VIENE INIALIZZATA SEMPRE LA VARIABILE SWFatturaPA
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: TIPO CONTRATTO SCONOSCIUTO (Contratti.Load (1))")
            Exit Sub
        End If
        '-
        If Session(CSTTIPODOC) <> SWTD(TD.FatturaCommerciale) Then
            chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False
            chkFatturaAC.Checked = False : chkFatturaAC.Visible = False
            chkScGiacenza.Checked = False : chkScGiacenza.Visible = False
            chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False 'NON LI RIATTIVO 
            ChkAcconto.AutoPostBack = False : ChkAcconto.Checked = False : ChkAcconto.Visible = False 'NON RIATTIVO
        End If
        '---------------------------------------------------------------
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO (Contratti.Load): " & strEser.Trim)
            Exit Sub
        End If
        If Val(strEser) > 2018 Then
            SWRifDoc = True
        Else
            SWRifDoc = False
        End If
        If SWRifDoc = False Then
            txtRiferimento.MaxLength = 150
        Else
            txtRiferimento.MaxLength = 20
        End If
        '---------
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
        LnkSCEC.Visible = True 'GIU220113
        LnkRegimeIVA.Visible = False 'GIU090814
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        '----------------------------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDSCliForFilProvv.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSPagamenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSBancheIBAN.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSRespArea.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSAgenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSListini.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSValuta.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSDurataTipo.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-
        Dim strObbligatorio As String = Session(CSTMAXLEVEL)
        If IsNothing(strObbligatorio) Then
            Session(CSTMAXLEVEL) = App.GetDatiDitta(Session(CSTCODDITTA).ToString.Trim, "").MaxLevel.Trim
            strObbligatorio = Session(CSTMAXLEVEL)
        ElseIf String.IsNullOrEmpty(strObbligatorio) Then
            Session(CSTMAXLEVEL) = App.GetDatiDitta(Session(CSTCODDITTA).ToString.Trim, "").MaxLevel.Trim
            strObbligatorio = Session(CSTMAXLEVEL)
        End If
        If String.IsNullOrEmpty(strObbligatorio) Then
            Session(CSTMAXLEVEL) = VALMAXLEVEL
            strObbligatorio = VALMAXLEVEL
        End If
        If strObbligatorio = "" Or Not IsNumeric(strObbligatorio) Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: Caricamento dati Società - MaskLevel non definito (Sessione scaduta - effettuare il login)")
            Exit Sub
        End If
        If CInt(strObbligatorio) < 1 Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: Caricamento dati Società - MaskLevel non definito (Sessione scaduta - effettuare il login)")
            Exit Sub
        End If
        txtCodCliForFilProvv.MaxLength = CInt(strObbligatorio)
        '---------
        Session(CSTTABCLIFOR) = TabCliFor
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim 'giu240615
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            'GIU220113
            Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            'GIU220113
            LnkSCEC.Visible = False
            Session(COD_CLIENTE) = String.Empty
        End If
        If TabCliFor = "Cli" Then
            lblLabelCliForFilProvv.Text = "Cliente" : Session(CSTTABCLIFOR) = "Cli"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
        ElseIf TabCliFor = "For" Then
            lblLabelCliForFilProvv.Text = "Fornitore" : Session(CSTTABCLIFOR) = "For"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Fornitori] WHERE ([Codice_CoGe] = @Codice)"
        Else 'DEFAULT giu191211
            lblLabelCliForFilProvv.Text = "Cli./For." : Session(CSTTABCLIFOR) = "CliFor"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)"
        End If
        '-
        If (Not IsPostBack) Then
            Session(CSTSCADPAGCA + "SAVE") = Nothing
            Session("GeneraAttPeriodi") = SWNO
            'GIU040914 SE è NUOVO FORZO SEMPRE IL RICALCOLO (Segnalato da Zibordi)
            If Session(SWOP) = SWOPNUOVO Or Session(SWOP) = SWOPELIMINA Then
                Session("SWOKRicalcolaGiac") = True
            Else
                Session("SWOKRicalcolaGiac") = False
            End If
            Session("SWOKRicalcolaGiac") = False 'giu291119
            '------------------------------------------------
            Session(CSTSWRbtnTD) = Session(CSTTIPODOC) 'GIU090814
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            Session(COD_CLIENTE) = String.Empty 'giu230113 
            '-
            SetCdmDAdp()

            'IMPOSTAZIONE SPECIFICHE PER TIPO DOCUMENTO
            ' ''txtRevNDoc.Visible = True : txtRevNDoc.AutoPostBack = True
            '-----------------------------------------------
            'giu191111 GIU030212
            Session(CSTNONCOMPLETO) = SWNO
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            '------------------------------------------
            'giu240321 per evitare che vada senza alcuna def.
            If Session(CSTNUOVOCADACA) <> SWSI And Session(CSTNUOVOCADACA) <> SWNO And Not IsNumeric(Session(CSTNUOVOCADACA)) Then
                Session(CSTNUOVOCADACA) = SWNO
            End If
            If Session(CSTNUOVOCADAOC) <> SWSI And Session(CSTNUOVOCADAOC) <> SWNO And Not IsNumeric(Session(CSTNUOVOCADAOC)) Then
                Session(CSTNUOVOCADAOC) = SWNO
            End If
            If Session(SWOP) = SWOPNUOVO Then
                DsDocT.Clear()
                AzzeraTxtDocT()
                WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2()
                txtNumero.AutoPostBack = False '' ': txtRevNDoc.AutoPostBack = False
                If GetNewNumDoc() = False Then Exit Sub 'giu260312
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = SWFatturaPA 'giu070814
                Session(CSTFATTURAPA) = SWFatturaPA
                chkFatturaPA.AutoPostBack = True
                ChkAcconto.Checked = False

                DDLTipoRapp.SelectedIndex = 2
                ddlMagazzino.AutoPostBack = False
                PosizionaItemDDL("2", ddlMagazzino, True) 'giu210721 richiesta email Francesca del 20/7
                ' ''ddlMagazzino.AutoPostBack = True
                ' ''txtRevNDoc.Text = "0" : txtRevNDoc.AutoPostBack = True
                txtNumero.AutoPostBack = True
                txtDataDoc.AutoPostBack = False
                If ControllaDataDoc1().Date = DATANULL Then
                    txtDataDoc.Text = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData)
                ElseIf Year(Now.Date) <> Val(Session(ESERCIZIO).ToString.Trim) Then
                    txtDataDoc.Text = Format(CDate(ControllaDataDoc1()).Date, FormatoData) 'GIU110118 DA now a ultimo movimento
                Else 'giu010318
                    txtDataDoc.Text = Format(Now.Date, FormatoData)
                End If
                txtDataDoc.AutoPostBack = True
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
                Session(CSTSTATODOC) = "5"
                lblMessDoc.Text = "DOCUMENTO NON COMPLETO<br>Per non rigenerare i periodi usare la funzione Cambia Stato del Contratto" : lblMessDoc.Visible = True
                'giu101219
                Dim strErrore As String = "" : Dim strValore As String = ""
                Call GetDatiAbilitazioni(CSTABILAZI, "DurataTipo", strValore, strErrore)
                If strErrore.Trim <> "" Then
                    strValore = ""
                End If
                If strValore.Trim <> "" Then
                    PosizionaItemDDL(strValore.Trim, DDLDurataTipo)
                Else
                    DDLDurataTipo.SelectedIndex = 0
                End If
                '-
                txtListino.AutoPostBack = False
                txtListino.Text = Session(IDLISTINO)
                txtListino.AutoPostBack = True
                lblCodValuta.Text = Session(CSTVALUTADOC)
                '----------------------------------------
                txtTipoFatt.AutoPostBack = False : DDLTipoFatt.AutoPostBack = False
                Call GetDatiAbilitazioni(CSTABILAZI, "TipoFattCA", strValore, strErrore)
                If strErrore.Trim <> "" Then
                    strValore = ""
                End If
                If strValore.Trim <> "" Then
                    txtTipoFatt.Text = strValore.Trim
                    PosizionaItemDDL(strValore.Trim, DDLTipoFatt)
                Else
                    DDLTipoFatt.SelectedIndex = -1
                End If
                txtTipoFatt.AutoPostBack = True : DDLTipoFatt.AutoPostBack = True
                '-
                txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
                Call GetDatiAbilitazioni(CSTABILAZI, "TipoContr", strValore, strErrore)
                If strErrore.Trim <> "" Then
                    strValore = ""
                End If
                txtPagamento.Text = strValore.Trim
                Session(CSTIDPAG) = strValore.Trim
                If strValore.Trim <> "" Then
                    PosizionaItemDDL(strValore.Trim, DDLPagamento)
                Else
                    DDLPagamento.SelectedIndex = -1
                End If
                If DDLPagamento.SelectedValue.Trim = "" Then 'giu231123
                    txtPagamento.Text = ""
                    DDLPagamento.SelectedIndex = -1
                End If
                txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
                '-
                Dim pCodCaus As Long = 0 : Dim pNVisite As Integer = 0 : Dim pDurataNum As Integer = 0
                If txtPagamento.Text.Trim = "" Then
                    Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
                    If strErrore.Trim <> "" Then
                        strValore = ""
                    End If
                End If
                '- valorizzato prima
                If strValore.Trim <> "" Then
                    txtCodCausale.Text = strValore.Trim
                    PosizionaItemDDL(strValore.Trim, DDLCausali)
                End If
                '-------
                If GetCodCausDATipoCAIdPag(pCodCaus, pNVisite, pDurataNum) = True Then
                    txtNVisite.Text = pNVisite.ToString.Trim
                    strValore = pCodCaus.ToString.Trim
                    txtDurataNum.AutoPostBack = False
                    txtDurataNum.Text = pDurataNum.ToString.Trim
                    txtDurataNum.AutoPostBack = True
                End If
                'GIU121223
                Call SetDateInizioFine()
                'giu101219 Dim strErrore As String = ""
                If AggNuovaTestata(strErrore) = False Then
                    Session(ERROREALL) = SWSI
                    Chiudi("Errore: Inserimento nuovo CONTRATTO. " & strErrore)
                    Exit Sub
                End If
                'giu191211
                txtNumero.AutoPostBack = False
                If Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                     Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                     Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Then
                    Dim IDNew As String = Session(IDDOCUMENTI)
                    If IsNothing(IDNew) Then
                        IDNew = "-1"
                    End If
                    If String.IsNullOrEmpty(IDNew) Then
                        IDNew = "-1"
                    End If
                    If CLng(txtNumero.Text.Trim) < 1 Then
                        txtNumero.Text = IDNew
                    End If
                End If
                txtNumero.AutoPostBack = True
                '---------
                Session(SWOPDETTDOC) = SWOPNESSUNA
                Session(SWOPDETTDOCR) = SWOPNESSUNA
                Session(SWOPDETTDOCL) = SWOPNESSUNA
                Session(SWOPMODSCATT) = SWOPNESSUNA
                Session(CSTNONCOMPLETO) = SWSI
                'giu230321 Ordine da cui caricare i dati:
                If Session(CSTNUOVOCADACA) = SWSI Then
                    lblNOrdine.Text = "Collega N° Ordine:"
                    btnDaOrdine.Visible = False
                    btnCollegaOC.Visible = True
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                ElseIf IsNumeric(Session(CSTNUOVOCADACA).ToString.Trim) Or Session(CSTNUOVOCADAOC) = SWNO Then
                    lblNOrdine.Text = "Collega N° Ordine:"
                    btnCollegaOC.Visible = True
                    btnDaOrdine.Visible = False
                    If IsNumeric(Session(CSTNUOVOCADACA).ToString.Trim) Then
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                        OKCollegaOC()
                    Else
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                    End If
                Else
                    btnCollegaOC.Visible = False
                    lblNOrdine.Text = "Ordine da cui caricare i dati:"
                    If Session(CSTNUOVOCADAOC) = SWSI Then
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = True
                        txtNOrdine.Visible = True
                        txtNOrdineRev.Visible = True
                    ElseIf IsNumeric(Session(CSTNUOVOCADAOC).ToString.Trim) Then
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                        OKDocTDaOCOC()
                    Else
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                    End If
                End If
            Else 'giu230321
                If Session(CSTNUOVOCADACA) = SWSI Then
                    lblNOrdine.Text = "Collega N° Ordine:"
                    btnDaOrdine.Visible = False
                    btnCollegaOC.Visible = True
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                ElseIf IsNumeric(Session(CSTNUOVOCADACA).ToString.Trim) Or Session(CSTNUOVOCADAOC) = SWNO Then
                    lblNOrdine.Text = "Collega N° Ordine:"
                    btnCollegaOC.Visible = True
                    btnDaOrdine.Visible = False
                    If IsNumeric(Session(CSTNUOVOCADACA).ToString.Trim) Then
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                        ' ''OKCollegaOC() verrà eseguito dal tasto
                    Else
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                    End If
                Else
                    btnCollegaOC.Visible = False
                    lblNOrdine.Text = "Ordine da cui caricare i dati:"
                    If Session(CSTNUOVOCADAOC) = SWSI Then
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = True
                        txtNOrdine.Visible = True
                        txtNOrdineRev.Visible = True
                    ElseIf IsNumeric(Session(CSTNUOVOCADAOC).ToString.Trim) Then
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                        ' ''OKDocTDaOCOC() verrà eseguito dal tasto
                    Else
                        btnDaOrdine.Visible = True
                        lblNOrdine.Visible = False
                        txtNOrdine.Visible = False
                        txtNOrdineRev.Visible = False
                    End If
                End If
            End If

            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                Session(ERROREALL) = SWSI
                Chiudi("Errore: IDENTIFICATIVO CONTRATTO SCONOSCIUTO")
                Exit Sub
            End If
            If CLng(myID) < 0 Then 'GIU191223
                Session(ERROREALL) = SWSI
                Chiudi("Errore: IDENTIFICATIVO CONTRATTO SCONOSCIUTO (If CLng(myID) < 0 Then 'GIU191223)")
                Exit Sub
            End If
            'giu050220
            Dim myIDDurataNum As String = Session(IDDURATANUM)
            If IsNothing(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            '-
            Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
            If IsNothing(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            '----------
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlAdapDoc.Fill(DsDocT.ContrattiT)

            dvDocT = New DataView(DsDocT.ContrattiT)
            Session("dvDocT") = dvDocT
            Session("SqlAdapDoc") = SqlAdapDoc
            Session("DsDocT") = DsDocT
            Session("SqlDbSelectCmd") = SqlDbSelectCmd
            Session("SqlDbInserCmd") = SqlDbInserCmd
            Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
            Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
            '
            Try
                If Session(SWOP) <> SWOPNESSUNA Then
                    If dvDocT.Count > 0 Then
                        BtnSetEnabledTo(False)
                        If Session(SWOP) = SWOPELIMINA Then
                            CampiSetEnabledToT(False)
                            btnAnnulla.Enabled = True
                            btnElimina.Enabled = True
                        Else
                            CampiSetEnabledToT(True)
                            btnAnnulla.Enabled = True
                            btnAggiorna.Enabled = True
                        End If
                        PopolaTxtDocT()
                        WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                        WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, "")
                    Else
                        CampiSetEnabledToT(False)
                        BtnSetEnabledTo(False)
                        AzzeraTxtDocT()
                        WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                        WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
                        Session(SWOP) = SWOPNESSUNA
                        btnNuovo.Enabled = True
                        Session(IDDOCUMENTI) = ""
                    End If
                ElseIf Session(SWOP) = SWOPNESSUNA Then
                    CampiSetEnabledToT(False)
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    If dvDocT.Count > 0 Then
                        PopolaTxtDocT()
                        WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                        WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, "")
                        btnElimina.Enabled = True
                        btnModifica.Enabled = True
                        btnCollegaOC.Enabled = True
                        btnDuplicaDNum.Enabled = True
                        btnNuovaDNum.Enabled = True
                        btnGeneraAttDNum.Enabled = True
                        btnStampa.Enabled = True : btnVerbale.Enabled = True
                        If WUC_ContrattiSpeseTraspTot1.CKAttEvPagEv = True And btnGeneraAttDNum.BackColor <> SEGNALA_KO Then
                            ' ''btnDaOrdine.Visible = False
                            btnDuplicaDNum.Enabled = False
                            btnNuovaDNum.Enabled = False
                            btnGeneraAttDNum.Enabled = False
                        End If
                    Else
                        AzzeraTxtDocT()
                        WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                        WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
                    End If
                End If
                Session(SWMODIFICATO) = SWNO
                'GIU030212
                Session(SWOPDETTDOC) = SWOPNESSUNA
                Session(SWOPDETTDOCR) = SWOPNESSUNA
                Session(SWOPDETTDOCL) = SWOPNESSUNA
                Session(SWOPMODSCATT) = SWOPNESSUNA
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
                'GIU020320 GIU191223
                If dvDocT.Count > 0 Then
                    If IsDBNull(dvDocT.Item(0).Item("StatoDoc")) Then
                        lblMessDoc.Text = "Attenzione, STATO DOCUMENTO ERRATO<br>Per non rigenerare i periodi usare la funzione Cambia Stato del Contratto" : lblMessDoc.Visible = True
                    ElseIf dvDocT.Item(0).Item("StatoDoc").ToString = "5" Then
                        lblMessDoc.Text = "DOCUMENTO NON COMPLETO<br>Per non rigenerare i periodi usare la funzione Cambia Stato del Contratto" : lblMessDoc.Visible = True
                    Else
                        ' ''btnDaOrdine.Visible = False
                    End If
                End If
                '--------
            Catch Ex As Exception
                Session(ERROREALL) = SWSI
                Chiudi("Errore: Caricamento GESTIONE CONTRATTI (load): " & Ex.Message)
                Exit Sub
            End Try
        End If
        If Session(SWOP) = SWOPELIMINA Then 'giu210220
            btnElimina.BackColor = SEGNALA_KO
        End If
        If Session(F_ANAGR_PROVV_APERTA) = True Then
            WFP_AnagrProvv_Insert1.Show()
        End If
        If Session(F_ANAGRCLIFOR_APERTA) = True Then
            WFP_Anagrafiche_Modify1.Show()
        End If
        If Session(F_DESTCLIFOR_APERTA) = True Then
            WFP_DestCliFor1.Show()
        End If
        If Session(F_BANCHEIBAN_APERTA) = True Then
            WFP_BancheIBAN1.Show()
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        'giu191211
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(F_CLI_RICERCA) Then
                WFPElencoCli.Show()
            End If
            If Session(F_FOR_RICERCA) Then
                WFPElencoFor.Show()
            End If
        End If
        If Session(F_ELENCO_DESTCF_APERTA) = True Then
            WFPElencoDestCF.Show()
        End If
        If Session(F_ANAGRAGENTI_APERTA) = True Then
            WFP_Agenti1.Show()
        End If
        If Session(F_ANAGRRESPAREA_APERTA) = True Then
            WFP_RespArea1.Show()
        End If
        If Session(F_ANAGRRESPVISITE_APERTA) = True Then
            WFP_RespVisite1.Show()
        End If
        Select Case Session(F_ELENCO_APERTA)
            ' ''Case F_NAZIONI
            ' ''    WFPElencoNazioni.Show()
            ' ''Case F_PROVINCE
            ' ''    WFPElencoProvince.Show()
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show()
                ' ''Case F_PAGAMENTI
                ' ''    WFPElencoPagamenti.Show()
                ' ''Case F_AGENTI
                ' ''    WFPElencoAgenti.Show()
                ' ''Case F_AGENTI_ESE_PREC
                ' ''    WFPElencoAgentiEsePrec.Show()
                ' ''Case F_ZONE
                ' ''    WFPElencoZone.Show()
                ' ''Case F_VETTORI
                ' ''    WFPElencoVettori.Show()
                ' ''Case F_CATEGORIE
                ' ''    WFPElencoCategorie.Show()
                ' ''Case F_LISTINO
                ' ''    WFPElencoListVenT.Show()
                ' ''Case F_CONTI
                ' ''    WFPElencoConti.Show()
        End Select
        'GIU030220
        Session(AGGPAGSCADTOT) = SWSI
        Try
            If Not IsDate(txtDataDoc.Text.Trim) Or Not IsNumeric(txtDurataNum.Text) Or DDLDurataTipo.SelectedValue.Trim = "" Or _
                Not IsDate(txtDataInizio.Text.Trim) Or Not IsDate(txtDataFine.Text.Trim) Or Not IsDate(txtDataAccettazione.Text.Trim) Or _
                DDLTipoFatt.SelectedValue.Trim = "" Or DDLPagamento.SelectedValue.Trim = "" Then
                Session(AGGPAGSCADTOT) = SWNO
            End If
        Catch ex As Exception
            Session(AGGPAGSCADTOT) = SWNO
        Finally
            If Session(AGGPAGSCADTOT) = SWNO Then
                txtDataDoc.AutoPostBack = False
                txtDataInizio.AutoPostBack = False : txtDataFine.AutoPostBack = False : txtDataAccettazione.AutoPostBack = False
            Else
                txtDataDoc.AutoPostBack = True
                txtDataInizio.AutoPostBack = True : txtDataFine.AutoPostBack = True : txtDataAccettazione.AutoPostBack = True
            End If
        End Try
    End Sub
    Private Sub SetDateInizioFine()
        '-----------------------------------------------------------------------------------------------------------------------------------        '@@@@@@@@@@@@@@@@@@@@@@@@@@@
        txtDurataNum.BackColor = SEGNALA_OK
        txtDurataNum.ToolTip = ""
        'GIU091120 giu030420 
        If txtDataInizio.Text.Trim = "" And IsDate(txtDataAccettazione.Text.Trim) Then
            txtDataInizio.Text = "01/01/" + CDate(txtDataAccettazione.Text).Year.ToString.Trim
        ElseIf txtDataInizio.Text.Trim = "" And IsDate(txtDataDoc.Text.Trim) Then
            txtDataInizio.Text = "01/01/" + CDate(txtDataDoc.Text).Year.ToString.Trim
        ElseIf txtDataInizio.Text.Trim = "" Then
            txtDataInizio.Text = "01/01/" + CDate(Now).Year.ToString.Trim
        End If
        '-
        If Not IsNumeric(txtDurataNum.Text.Trim) Then txtDurataNum.Text = "1"
        If txtDataFine.Text.Trim = "" Then
            If DDLDurataTipo.SelectedValue.Trim = "A" Then
                txtDataFine.Text = DateAdd(DateInterval.Year, Int(txtDurataNum.Text) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "M" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, Int(txtDurataNum.Text) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "T" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 3) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "Q" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 4) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "S" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 6) - 1, CDate(txtDataInizio.Text))
            Else 'GIU201023 NEL CASO NON RIESCO A DETERMINARE IMPOSTO IL FINE ANNO DELLA DATA INIZIO
                txtDataFine.Text = "31/12/" + CDate(txtDataInizio.Text).Year.ToString.Trim
            End If
        End If
        'GIU201023 IMPOSTO SEMPRE IL FINE ANNO E NON INIZIO ANNO
        If Not IsDate(txtDataFine.Text.Trim) And IsDate(txtDataInizio.Text.Trim) Then
            txtDataFine.Text = "31/12/" + CDate(txtDataInizio.Text).Year.ToString.Trim
        ElseIf DDLDurataTipo.SelectedValue.Trim = "A" Then
            txtDataFine.Text = "31/12/" + CDate(txtDataFine.Text).Year.ToString.Trim
        Else
            txtDataFine.Text = DateAdd(DateInterval.Month, 1, CDate(txtDataFine.Text))
            txtDataFine.Text = DateAdd(DateInterval.Day, -1, CDate(txtDataFine.Text))
        End If
        '---------------
        Dim myDurata As Long = 0
        If txtDataInizio.Text.Trim <> "" Then
            If Not IsDate(txtDataInizio.Text.Trim) Then
                'txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataFine.Text.Trim) Then
                If CDate(txtDataFine.Text.Trim) < CDate(txtDataInizio.Text.Trim) Then
                    txtDataFine.BackColor = SEGNALA_KO
                    txtDataFine.ToolTip = "La data di fine Contratto dev'essere maggiore della data inizio"
                Else
                    myDurata = 0
                    If DDLDurataTipo.SelectedValue.Trim = "A" Then
                        myDurata = DateDiff(DateInterval.Year, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "M" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "T" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 3
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "Q" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 4
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "S" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 6
                    Else 'default ANNO
                        myDurata = DateDiff(DateInterval.Year, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    End If
                    If myDurata < 0 Then
                        myDurata = myDurata * -1
                    End If
                    myDurata = Math.Round(myDurata, 0)
                    If DDLDurataTipo.SelectedValue.Trim = "A" Or DDLDurataTipo.SelectedValue.Trim = "M" Then
                        myDurata += 1
                    ElseIf myDurata = 0 Then
                        myDurata = 1
                    End If
                    '-
                    If IsNumeric(txtDurataNum.Text.Trim) Then
                        If CLng(txtDurataNum.Text) <> myDurata Then
                            txtDurataNum.BackColor = SEGNALA_INFO
                            txtDurataNum.ToolTip = "(ATTENZIONE) Durata N° prevista e di " & myDurata.ToString
                        End If
                    Else
                        txtDurataNum.Text = myDurata.ToString
                    End If
                End If
            End If
        End If
    End Sub
    'giu260312
    Private Function GetNewNumDoc() As Boolean
        GetNewNumDoc = True
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            GetNewNumDoc = False
            Chiudi("Errore: TIPO CONTRATTO SCONOSCIUTO (GetNewNumDoc)")
            Exit Function
        End If
        Session(IDLISTINO) = "1" 'Listino Base
        Session(IDDOCUMENTI) = ""
        Session(CSTVALUTADOC) = "Euro"
        Session(CSTDECIMALIVALUTADOC) = 2
        '-
        lblCodValuta.Text = "Euro"

        Dim strErrore As String = ""
        Session(SWOPNUOVONUMDOC) = ""
        txtNumero.Text = GetNewCA()
        'Contratti
        strErrore = "" : Dim strValore As String = ""
        Call GetDatiAbilitazioni(CSTABILAZI, "DurataTipo", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            Call PosizionaItemDDL(strValore.Trim, DDLDurataTipo)
        Else
            DDLDurataTipo.SelectedIndex = 0
        End If
        '-
        txtTipoFatt.AutoPostBack = False : DDLTipoFatt.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoFattCA", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtTipoFatt.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLTipoFatt)
        Else
            DDLTipoFatt.SelectedIndex = -1
        End If
        txtTipoFatt.AutoPostBack = True : DDLTipoFatt.AutoPostBack = True
        '-
        txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoContr", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtPagamento.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLPagamento)
        Else
            DDLPagamento.SelectedIndex = -1
        End If
        txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
        '-
        Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtCodCausale.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLCausali)
        End If
        '' ''DDLCausCALoc.Enabled = SW
        ' ''Call GetDatiAbilitazioni(CSTABILAZI, "ConAssLoc", strValore, strErrore)
        ' ''If strErrore.Trim <> "" Then
        ' ''    strValore = ""
        ' ''End If
        ' ''If strValore.Trim <> "" Then
        ' ''    PosizionaItemDDL(strValore.Trim, DDLCausCALoc)
        ' ''Else
        ' ''    DDLCausCALoc.SelectedValue = 0
        ' ''End If
        '' ''DDLCausCATelC.Enabled = SW
        ' ''Call GetDatiAbilitazioni(CSTABILAZI, "ConAssTelC", strValore, strErrore)
        ' ''If strErrore.Trim <> "" Then
        ' ''    strValore = ""
        ' ''End If
        ' ''If strValore.Trim <> "" Then
        ' ''    PosizionaItemDDL(strValore.Trim, DDLCausCATelC)
        ' ''Else
        ' ''    DDLCausCATelC.SelectedValue = 0
        ' ''End If
    End Function
    
    Private Function GetNewCA() As Long
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: TIPO CONTRATTO SCONOSCIUTO (GetNewCA)")
            Exit Function
        End If
        ' ''txtRevNDoc.AutoPostBack = False
        ' ''If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        ' ''txtRevNDoc.AutoPostBack = True
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From ContrattiT WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewCA = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewCA = 1
                    End If
                    Exit Function
                Else
                    GetNewCA = 1
                    Exit Function
                End If
            Else
                GetNewCA = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCA = -1
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Contratti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function
    Private Function AggNuovaTestata(ByRef strErrore As String) As Boolean
        AggNuovaTestata = AggiornaDocT(Nothing, False, strErrore)
        Session(SWOP) = SWOPMODIFICA
        '------
        Session("TabDoc") = TB0
        TabContainer1.ActiveTabIndex = TB0
    End Function

#Region " Imposta Command e DataAdapter"
    Private Function SetCdmDAdp() As Boolean
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
        SqlConnDoc = New SqlConnection
        SqlAdapDoc = New SqlDataAdapter
        SqlAdapDocForInsert = New SqlDataAdapter 'Pier311011
        SqlAdapDocForInsertL = New SqlDataAdapter 'giu291111
        SqlDbSelectCmd = New SqlCommand
        SqlDbInserCmd = New SqlCommand
        SqlDbUpdateCmd = New SqlCommand
        SqlDbDeleteCmd = New SqlCommand
        SqlDbInserCmdForInsert = New SqlCommand  'Pier311011
        SqlDbInserCmdForInsertL = New SqlCommand  'giu291111
        'giu250220
        SqlAdapDocALLAtt = New SqlDataAdapter
        SqlDbSelectCmdALLAtt = New SqlCommand
        '---------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)

        SqlDbSelectCmd.CommandText = "get_ConTByIDDocumenti" 'OK SELECT *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDoc
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1
        '
        SqlDbInserCmd.CommandText = "insert_ConTByIDDocumenti"
        SqlDbInserCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmd.Connection = Me.SqlConnDoc
        SqlDbInserCmd.CommandTimeout = myTimeOUT
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2, "Tipo_Doc"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10, "Numero"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Doc", System.Data.SqlDbType.DateTime, 8, "Data_Doc"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Causale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Fatturazione", System.Data.SqlDbType.NVarChar, 2, "Tipo_Fatturazione"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Cliente", System.Data.SqlDbType.NVarChar, 16, "Cod_Cliente"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Fornitore", System.Data.SqlDbType.NVarChar, 16, "Cod_Fornitore"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Filiale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDAnagrProvv", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDAnagrProvv", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Pagamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Pagamento", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ABI", System.Data.SqlDbType.NVarChar, 5, "ABI"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CAB", System.Data.SqlDbType.NVarChar, 5, "CAB"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Validita", System.Data.SqlDbType.DateTime, 8, "Data_Validita"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riferimento", System.Data.SqlDbType.NVarChar, 150, "Riferimento"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Riferimento", System.Data.SqlDbType.DateTime, 8, "Data_Riferimento"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Agente", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Listino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Listino", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Valuta", System.Data.SqlDbType.NVarChar, 4, "Cod_Valuta"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraConsegna", System.Data.SqlDbType.DateTime, 8, "DataOraConsegna"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Spedizione", System.Data.SqlDbType.NVarChar, 1, "Tipo_Spedizione"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Porto", System.Data.SqlDbType.NVarChar, 1, "Porto"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Incasso", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Incasso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Trasporto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Trasporto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Imballo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Imballo", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SpeseVarie", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "SpeseVarie", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Iva", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero_Fattura", System.Data.SqlDbType.NVarChar, 10, "Numero_Fattura"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Fattura", System.Data.SqlDbType.DateTime, 8, "Data_Fattura"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Doc_Principale", System.Data.SqlDbType.Bit, 1, "Doc_Principale"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CoGe", System.Data.SqlDbType.Bit, 1, "CoGe"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 1073741823, "Note"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva4", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Totale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Totale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotaleM", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotaleM", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Abbuono", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Abbuono", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Sconto_Cassa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Cassa", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Aspetto", System.Data.SqlDbType.NVarChar, 150, "Aspetto"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Peso", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Peso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Colli", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Colli", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione1", System.Data.SqlDbType.NVarChar, 150, "Destinazione1"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione2", System.Data.SqlDbType.NVarChar, 150, "Destinazione2"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione3", System.Data.SqlDbType.NVarChar, 150, "Destinazione3"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceMagazzino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceMagazzino", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_1"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_2"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_3"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_4"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_5"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_5", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraRitiro", System.Data.SqlDbType.DateTime, 8, "DataOraRitiro"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteRitiro", System.Data.SqlDbType.NVarChar, 1073741823, "NoteRitiro"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Descrizione_Imballo", System.Data.SqlDbType.NVarChar, 150, "Descrizione_Imballo"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefInt", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRefInt", System.Data.SqlDbType.NVarChar, 1073741823, "DesRefInt"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefNumDDT", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 8, "RefDataDDT"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Evaso", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Evaso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RiBa", System.Data.SqlDbType.Bit, 1, "RiBa"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Accesso", System.Data.SqlDbType.Bit, 1, "Accesso"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Pezzi", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Pezzi", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodValutaCV", System.Data.SqlDbType.NVarChar, 4, "CodValutaCV"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ValoreValutaCV", System.Data.SqlDbType.NVarChar, 10, "ValoreValutaCV"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DescrizionePorto", System.Data.SqlDbType.NVarChar, 150, "DescrizionePorto"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ADeposito", System.Data.SqlDbType.Bit, 1, "ADeposito"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodDeposito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodDeposito", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SoloModifica", System.Data.SqlDbType.Bit, 1, "SoloModifica"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Operatore", System.Data.SqlDbType.NVarChar, 50, "Operatore"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NomePC", System.Data.SqlDbType.NVarChar, 50, "NomePC"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccatoDalPC", System.Data.SqlDbType.NVarChar, 50, "BloccatoDalPC"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@UltimaPagina", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "UltimaPagina", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumPagineTot", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumPagineTot", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StatoDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "StatoDoc", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ChiusoNonEvaso", System.Data.SqlDbType.Bit, 1, "ChiusoNonEvaso"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodNonevasione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodNonevasione", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteNonEvasione", System.Data.SqlDbType.NVarChar, 1073741823, "NoteNonEvasione"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraChiusoNonEvaso", System.Data.SqlDbType.DateTime, 8, "DataOraChiusoNonEvaso"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StatoAllestimento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "StatoAllestimento", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroDef", System.Data.SqlDbType.NVarChar, 10, "NumeroDef"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDef", System.Data.SqlDbType.DateTime, 8, "DataDef"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoCorrente", System.Data.SqlDbType.NVarChar, 12, "ContoCorrente"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IBAN", System.Data.SqlDbType.NVarChar, 27, "IBAN"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RevisioneNDoc", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Validita", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Validita", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Consegna", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Consegna", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteDocumento", System.Data.SqlDbType.NVarChar, 1073741823, "NoteDocumento"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CIG", System.Data.SqlDbType.NVarChar, 10, "CIG"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CUP", System.Data.SqlDbType.NVarChar, 15, "CUP"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50, "InseritoDa"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50, "ModificatoDa"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoEvTotale", System.Data.SqlDbType.Bit, 1, "SWTipoEvTotale"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoEvSaldo", System.Data.SqlDbType.Bit, 1, "SWTipoEvSaldo"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroMM", System.Data.SqlDbType.NVarChar, 10, "NumeroMM"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataMM", System.Data.SqlDbType.DateTime, 8, "DataMM"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MovPadreMMDBase", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "MovPadreMMDBase", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MovFiglioMMDBase", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "MovFiglioMMDBase", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefNumFattura", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefNumFattura", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroDDT", System.Data.SqlDbType.NVarChar, 10, "NumeroDDT"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceMagazzinoM2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceMagazzinoM2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_CausaleM2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_CausaleM2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteM2", System.Data.SqlDbType.NVarChar, 1073741823, "NoteM2"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1, "FatturaPA"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1, "SplitIVA"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RitAcconto", System.Data.SqlDbType.Bit, 1, "RitAcconto"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ImponibileRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImponibileRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PercRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotaleRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotaleRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotNettoPagare", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotNettoPagare", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaAC", System.Data.SqlDbType.Bit, 1, "FatturaAC"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScGiacenza", System.Data.SqlDbType.Bit, 1, "ScGiacenza"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Acconto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Acconto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoRapportoPA", System.Data.SqlDbType.NVarChar, 2, "TipoRapportoPA"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Bollo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Bollo", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BolloACaricoDel", System.Data.SqlDbType.NChar, 1, "BolloACaricoDel"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RespArea", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RespArea", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RespVisite", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RespVisite", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataInizio", System.Data.SqlDbType.DateTime, 8, "DataInizio"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataFine", System.Data.SqlDbType.DateTime, 8, "DataFine"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataAccetta", System.Data.SqlDbType.DateTime, 8, "DataAccetta"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataTipo", System.Data.SqlDbType.NVarChar, 1, "DurataTipo"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoContratto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "TipoContratto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NVisite", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NVisite", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadPagCA"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadAttCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadAttCA"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccaScPag", System.Data.SqlDbType.Bit, 1, "BloccaScPag"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccaScAtt", System.Data.SqlDbType.Bit, 1, "BloccaScAtt"))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaDurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "QtaDurataNum", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaDurataNumR0", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "QtaDurataNumR0", System.Data.DataRowVersion.Current, Nothing))
        SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoModSint", System.Data.SqlDbType.Bit, 1, "SWTipoModSint"))
        '
        'SqlUpdateCommand1
        '
        SqlDbUpdateCmd.CommandText = "[Update_ConTByIDDocumenti]"
        SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdateCmd.Connection = Me.SqlConnDoc
        SqlDbUpdateCmd.CommandTimeout = myTimeOUT
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2, "Tipo_Doc"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10, "Numero"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Doc", System.Data.SqlDbType.DateTime, 8, "Data_Doc"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Causale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Fatturazione", System.Data.SqlDbType.NVarChar, 2, "Tipo_Fatturazione"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Cliente", System.Data.SqlDbType.NVarChar, 16, "Cod_Cliente"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Fornitore", System.Data.SqlDbType.NVarChar, 16, "Cod_Fornitore"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Filiale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDAnagrProvv", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDAnagrProvv", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Pagamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Pagamento", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ABI", System.Data.SqlDbType.NVarChar, 5, "ABI"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CAB", System.Data.SqlDbType.NVarChar, 5, "CAB"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Validita", System.Data.SqlDbType.DateTime, 8, "Data_Validita"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riferimento", System.Data.SqlDbType.NVarChar, 150, "Riferimento"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Riferimento", System.Data.SqlDbType.DateTime, 8, "Data_Riferimento"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Agente", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Listino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Listino", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Valuta", System.Data.SqlDbType.NVarChar, 4, "Cod_Valuta"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraConsegna", System.Data.SqlDbType.DateTime, 8, "DataOraConsegna"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Spedizione", System.Data.SqlDbType.NVarChar, 1, "Tipo_Spedizione"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Porto", System.Data.SqlDbType.NVarChar, 1, "Porto"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Vettore_3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Vettore_3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Incasso", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Incasso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Trasporto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Trasporto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Spese_Imballo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Spese_Imballo", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SpeseVarie", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "SpeseVarie", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_Iva", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero_Fattura", System.Data.SqlDbType.NVarChar, 10, "Numero_Fattura"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Fattura", System.Data.SqlDbType.DateTime, 8, "Data_Fattura"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Doc_Principale", System.Data.SqlDbType.Bit, 1, "Doc_Principale"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CoGe", System.Data.SqlDbType.Bit, 1, "CoGe"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 1073741823, "Note"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva4", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imponibile4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imponibile4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(9, Byte), "Imposta1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Imposta4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Imposta4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Totale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Totale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotaleM", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotaleM", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Abbuono", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Abbuono", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Sconto_Cassa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Cassa", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Aspetto", System.Data.SqlDbType.NVarChar, 150, "Aspetto"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Peso", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Peso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Colli", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Colli", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione1", System.Data.SqlDbType.NVarChar, 150, "Destinazione1"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione2", System.Data.SqlDbType.NVarChar, 150, "Destinazione2"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione3", System.Data.SqlDbType.NVarChar, 150, "Destinazione3"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceMagazzino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceMagazzino", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_1"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_2"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_3"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_4"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_5"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_1", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_3", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_4", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_5", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraRitiro", System.Data.SqlDbType.DateTime, 8, "DataOraRitiro"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteRitiro", System.Data.SqlDbType.NVarChar, 1073741823, "NoteRitiro"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Descrizione_Imballo", System.Data.SqlDbType.NVarChar, 150, "Descrizione_Imballo"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefInt", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRefInt", System.Data.SqlDbType.NVarChar, 1073741823, "DesRefInt"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefNumDDT", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 8, "RefDataDDT"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Evaso", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Evaso", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RiBa", System.Data.SqlDbType.Bit, 1, "RiBa"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Accesso", System.Data.SqlDbType.Bit, 1, "Accesso"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Pezzi", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Pezzi", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodValutaCV", System.Data.SqlDbType.NVarChar, 4, "CodValutaCV"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ValoreValutaCV", System.Data.SqlDbType.NVarChar, 10, "ValoreValutaCV"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DescrizionePorto", System.Data.SqlDbType.NVarChar, 150, "DescrizionePorto"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ADeposito", System.Data.SqlDbType.Bit, 1, "ADeposito"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodDeposito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodDeposito", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SoloModifica", System.Data.SqlDbType.Bit, 1, "SoloModifica"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Operatore", System.Data.SqlDbType.NVarChar, 50, "Operatore"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NomePC", System.Data.SqlDbType.NVarChar, 50, "NomePC"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccatoDalPC", System.Data.SqlDbType.NVarChar, 50, "BloccatoDalPC"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@UltimaPagina", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "UltimaPagina", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumPagineTot", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumPagineTot", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StatoDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "StatoDoc", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ChiusoNonEvaso", System.Data.SqlDbType.Bit, 1, "ChiusoNonEvaso"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodNonevasione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodNonevasione", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteNonEvasione", System.Data.SqlDbType.NVarChar, 1073741823, "NoteNonEvasione"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataOraChiusoNonEvaso", System.Data.SqlDbType.DateTime, 8, "DataOraChiusoNonEvaso"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StatoAllestimento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "StatoAllestimento", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroDef", System.Data.SqlDbType.NVarChar, 10, "NumeroDef"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDef", System.Data.SqlDbType.DateTime, 8, "DataDef"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoCorrente", System.Data.SqlDbType.NVarChar, 12, "ContoCorrente"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IBAN", System.Data.SqlDbType.NVarChar, 27, "IBAN"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RevisioneNDoc", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Validita", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Validita", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Consegna", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Consegna", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteDocumento", System.Data.SqlDbType.NVarChar, 1073741823, "NoteDocumento"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CIG", System.Data.SqlDbType.NVarChar, 10, "CIG"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CUP", System.Data.SqlDbType.NVarChar, 15, "CUP"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50, "InseritoDa"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50, "ModificatoDa"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoEvTotale", System.Data.SqlDbType.Bit, 1, "SWTipoEvTotale"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoEvSaldo", System.Data.SqlDbType.Bit, 1, "SWTipoEvSaldo"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroMM", System.Data.SqlDbType.NVarChar, 10, "NumeroMM"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataMM", System.Data.SqlDbType.DateTime, 8, "DataMM"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MovPadreMMDBase", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "MovPadreMMDBase", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MovFiglioMMDBase", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "MovFiglioMMDBase", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefNumFattura", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RefNumFattura", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroDDT", System.Data.SqlDbType.NVarChar, 10, "NumeroDDT"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceMagazzinoM2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceMagazzinoM2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_CausaleM2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Cod_CausaleM2", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteM2", System.Data.SqlDbType.NVarChar, 1073741823, "NoteM2"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1, "FatturaPA"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1, "SplitIVA"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RitAcconto", System.Data.SqlDbType.Bit, 1, "RitAcconto"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ImponibileRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImponibileRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PercRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotaleRA", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotaleRA", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotNettoPagare", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "TotNettoPagare", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaAC", System.Data.SqlDbType.Bit, 1, "FatturaAC"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScGiacenza", System.Data.SqlDbType.Bit, 1, "ScGiacenza"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Acconto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Acconto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoRapportoPA", System.Data.SqlDbType.NVarChar, 2, "TipoRapportoPA"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Bollo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Bollo", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BolloACaricoDel", System.Data.SqlDbType.NChar, 1, "BolloACaricoDel"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RespArea", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RespArea", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RespVisite", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RespVisite", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataInizio", System.Data.SqlDbType.DateTime, 8, "DataInizio"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataFine", System.Data.SqlDbType.DateTime, 8, "DataFine"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataAccetta", System.Data.SqlDbType.DateTime, 8, "DataAccetta"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataTipo", System.Data.SqlDbType.NVarChar, 1, "DurataTipo"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoContratto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "TipoContratto", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NVisite", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NVisite", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadPagCA"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadAttCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadAttCA"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccaScPag", System.Data.SqlDbType.Bit, 1, "BloccaScPag"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccaScAtt", System.Data.SqlDbType.Bit, 1, "BloccaScAtt"))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaDurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "QtaDurataNum", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaDurataNumR0", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "QtaDurataNumR0", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWTipoModSint", System.Data.SqlDbType.Bit, 1, "SWTipoModSint"))
        '
        'SqlDeleteCommand1
        'QUI SE SI DOVESSE CANCELLARE ANCHE I LOTTI/MACCHINE ETC ETC E RIPRISTINO DOC CON REFINT
        SqlDbDeleteCmd.CommandText = "[delete_ConTByIDDocumenti]"
        SqlDbDeleteCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmd.Connection = Me.SqlConnDoc
        SqlDbDeleteCmd.CommandTimeout = myTimeOUT
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlDbInserCmdForInsert
        '
        SqlDbInserCmdForInsert.CommandText = "insert_ConDByIDDocumenti_ForInsert"
        SqlDbInserCmdForInsert.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdForInsert.Connection = Me.SqlConnDoc
        SqlDbInserCmdForInsert.CommandTimeout = myTimeOUT
        SqlDbInserCmdForInsert.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), _
            New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), _
            New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), _
            New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), _
            New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), _
            New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), _
            New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), _
            New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
            New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
            New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti"), _
            New System.Data.SqlClient.SqlParameter("@PrezzoCosto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoCosto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Inviata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Inviata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@DedPerAcconto", System.Data.SqlDbType.Bit, 0, "DedPerAcconto"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 0, "Cod_Filiale"), _
            New System.Data.SqlClient.SqlParameter("@DataEv", System.Data.SqlDbType.DateTime, 0, "DataEv"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@Serie", System.Data.SqlDbType.NVarChar, 0, "Serie"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNum", System.Data.SqlDbType.Int, 0, "QtaDurataNum"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNumR0", System.Data.SqlDbType.Int, 0, "QtaDurataNumR0"), _
            New System.Data.SqlClient.SqlParameter("@DataSc", System.Data.SqlDbType.DateTime, 0, "DataSc"), _
            New System.Data.SqlClient.SqlParameter("@SWInvioMod", System.Data.SqlDbType.Bit, 0, "SWInvioMod"), _
            New System.Data.SqlClient.SqlParameter("@SWCalcoloTot", System.Data.SqlDbType.Bit, 0, "SWCalcoloTot"), _
            New System.Data.SqlClient.SqlParameter("@SWSostituito", System.Data.SqlDbType.Bit, 0, "SWSostituito"), _
            New System.Data.SqlClient.SqlParameter("@SWChiusaNoEv", System.Data.SqlDbType.Bit, 0, "SWChiusaNoEv"), _
            New System.Data.SqlClient.SqlParameter("@RefNumFC", System.Data.SqlDbType.Int, 0, "RefNumFC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataFC", System.Data.SqlDbType.DateTime, 0, "RefDataFC"), _
            New System.Data.SqlClient.SqlParameter("@RespArea", System.Data.SqlDbType.Int, 0, "RespArea"), _
            New System.Data.SqlClient.SqlParameter("@RespVisite", System.Data.SqlDbType.Int, 0, "RespVisite")})
        '
        'SqlDbInserCmdForInsertL
        '
        SqlDbInserCmdForInsertL.CommandText = "insert_ConDLByIDDocRiga_ForInsert"
        SqlDbInserCmdForInsertL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdForInsertL.Connection = Me.SqlConnDoc
        SqlDbInserCmdForInsertL.CommandTimeout = myTimeOUT
        SqlDbInserCmdForInsertL.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie")})
        'giu250220
        SqlDbSelectCmdALLAtt.CommandText = "get_ConDByIDDocumenti" 'ok select *
        SqlDbSelectCmdALLAtt.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmdALLAtt.Connection = Me.SqlConnDoc
        SqlDbSelectCmdALLAtt.CommandTimeout = myTimeOUT
        SqlDbSelectCmdALLAtt.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '-
        SqlAdapDocALLAtt.SelectCommand = SqlDbSelectCmdALLAtt
        '
        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        SqlAdapDocForInsert.InsertCommand = SqlDbInserCmdForInsert      'Pier311011
        SqlAdapDocForInsertL.InsertCommand = SqlDbInserCmdForInsertL      'giu291111

        Session("SqlAdapDoc") = SqlAdapDoc
        Session("SqlDbSelectCmd") = SqlDbSelectCmd
        Session("SqlDbInserCmd") = SqlDbInserCmd
        Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
        Session("SqlDbUpdateCmd") = SqlDbUpdateCmd

        Session("SqlAdapDocForInsert") = SqlAdapDocForInsert        'Pier311011
        Session("SqlDbInserCmdForInsert") = SqlDbInserCmdForInsert  'Pier311011
        '---
        Session("SqlAdapDocForInsertL") = SqlAdapDocForInsertL        'giu291111
        Session("SqlDbInserCmdForInsertL") = SqlDbInserCmdForInsertL  'giu291111
    End Function
#End Region

    Public Function CallBackWFPAnagrProvv() As Boolean
        Session(IDANAGRPROVV) = ""
        Dim rk As StrAnagrProvv
        rk = Session(RKANAGRPROVV)
        If IsNothing(rk.Ragione_Sociale) Then
            Exit Function
        End If
        Session(IDANAGRPROVV) = rk.IDAnagrProvv

        'simone040417: Se IDAnagr è provvisorio inserisci "(" + IDAnagrProvv + ")" senno solo IDAnagrReale
        If Session(IDANAGRREALESN) = True Then
            txtCodCliForFilProvv.Text = rk.IDAnagrProvv.ToString.Trim 'IDReale
            Session(CSTUpgAngrProvvCG) = ""
            txtCodCliForFilProvv.ToolTip = ""
            lblCliForFilProvv.ToolTip = ""
        Else
            txtCodCliForFilProvv.Text = "(" & rk.IDAnagrProvv.ToString.Trim & ")"
            Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text.Trim 'GIU111012
            txtCodCliForFilProvv.ToolTip = "(Provvisoria)"
            lblCliForFilProvv.ToolTip = "(Provvisoria)"
            'GIU140417
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            'giu240113 
            Session(COD_CLIENTE) = String.Empty
            '---------
            PopolaDestCliFor()
            WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
            '-
        End If
        lblCliForFilProvv.Text = rk.Ragione_Sociale
        '---------

        lblPICF.Text = rk.Partita_IVA : lblLabelPICF.Text = "Partita IVA"
        If lblPICF.Text.Trim = "" Then
            lblPICF.Text = rk.Codice_Fiscale : lblLabelPICF.Text = "Codice Fiscale"
        End If
        lblIndirizzo.Text = rk.Indirizzo
        lblLocalita.Text = rk.Cap & " " & rk.Localita & " " & IIf(rk.Provincia.ToString.Trim <> "", "(" & rk.Provincia.ToString & ")", "")

        txtListino.Text = "1"
        DDLListini.AutoPostBack = False
        PosizionaItemDDLByTxt(txtListino, DDLListini)
        DDLListini.AutoPostBack = True
        lblCodValuta.Text = GetParamGestAzi(Session(ESERCIZIO)).Cod_Valuta
        lblValuta()
        WUC_ContrattiDett1.SetSWPrezzoALCSG() 'GIU210412
    End Function
    Public Function CallBackWFPAnagrCliFor() As Boolean

        Session(IDANAGRCLIFOR) = ""
        Dim rk As StrAnagrCliFor
        rk = Session(RKANAGRCLIFOR)
        If IsNothing(rk.Rag_Soc) Then
            Exit Function
        End If

        Dim strToolTip As String = ""
        lblCliForFilProvv.Text = rk.Rag_Soc + " " + rk.Denominazione
        strToolTip += txtCodCliForFilProvv.Text.Trim + " " + rk.Rag_Soc + " " + rk.Denominazione + " " + rk.Riferimento + " "
        '---------
        lblPICF.Text = rk.Partita_IVA : lblLabelPICF.Text = "Partita IVA"
        If lblPICF.Text.Trim = "" Then
            lblPICF.Text = rk.Codice_Fiscale : lblLabelPICF.Text = "Codice Fiscale"
        End If
        strToolTip += lblLabelPICF.Text + ": " + lblPICF.Text + " "
        lblIndirizzo.Text = rk.Indirizzo + " " + rk.NumeroCivico
        strToolTip += lblIndirizzo.Text + " "
        lblLocalita.Text = rk.Cap & " " & rk.Localita & " " & IIf(rk.Provincia.ToString.Trim <> "", "(" & rk.Provincia.ToString & ")", "")
        strToolTip += lblLocalita.Text
        '-- OK
        txtCodCliForFilProvv.ToolTip = strToolTip
        lblCliForFilProvv.ToolTip = strToolTip
        WUC_ContrattiDett1.SetSWPrezzoALCSG() 'GIU210412
    End Function
    Public Function CallBackWFPBancheIBAN() As Boolean
        Session(IDBANCHEIBAN) = ""
        Dim rk As StrBancheIBAN
        rk = Session(RKBANCHEIBAN)
        If IsNothing(rk.IBAN) Then
            Exit Function
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Function
        End If
        Session(IDBANCHEIBAN) = rk.IBAN
        lblABI.Text = rk.ABI.ToString.Trim
        lblCAB.Text = rk.CAB.ToString.Trim
        lblBanca.Text = rk.Descrizione.Trim
        lblFiliale.Text = ""
        lblIBAN.Text = rk.IBAN.Trim
        lblContoCorrente.Text = rk.ContoCorrente.Trim
        lblABICAB()

        SqlDSBancheIBAN.DataBind()
        DDLBancheIBAN.Items.Clear()
        DDLBancheIBAN.Items.Add("")
        DDLBancheIBAN.DataBind()

        PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
        If DDLBancheIBAN.SelectedIndex = 0 Then
            DDLBancheIBAN.SelectedIndex = 0
            lblABI.Text = ""
            lblCAB.Text = ""
            lblBanca.Text = ""
            lblFiliale.Text = ""
            lblIBAN.Text = "IBAN"
            lblContoCorrente.Text = "Conto corrente"
        End If
        '---------
    End Function
    Public Function CancBackWFPBancheIBAN() As Boolean
        'nulla
    End Function
    Public Function GetTxtlblMessDoc() As String
        GetTxtlblMessDoc = lblMessDoc.Text.Trim
        If GetTxtlblMessDoc <> "" Then lblMessDoc.Visible = True
    End Function
    Public Sub SetTxtlblMessDoc(ByVal pMessDoc As String)
        lblMessDoc.Text = pMessDoc.Trim
        If pMessDoc.Trim <> "" Then
            lblMessDoc.Visible = True
        Else
            lblMessDoc.Visible = False
        End If
    End Sub
    Public Function GetRespAreaVisite(ByRef myCodArea As String, ByRef myCodVisita As String) As Boolean
        GetRespAreaVisite = True
        myCodArea = txtCodRespArea.Text.Trim
        myCodVisita = txtCodRespVisite.Text.Trim
        If Not IsNumeric(myCodArea) Or myCodArea = "0" Then
            myCodArea = ""
        End If
        If Not IsNumeric(myCodVisita) Or myCodVisita = "0" Then
            myCodVisita = ""
        End If
    End Function
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "", Optional ByRef mySCGiacenza As Boolean = False) As Boolean
        CKCSTTipoDoc = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) 'GIU231023
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        strDesTipoDocumento = ""
        'giu230714 GIU060814
        Dim mySWFatturaPA As String = Session(CSTFATTURAPA)
        If IsNothing(mySWFatturaPA) Then
            SWFatturaPA = False
        End If
        If String.IsNullOrEmpty(mySWFatturaPA) Then
            SWFatturaPA = False
        ElseIf Session(CSTFATTURAPA) Then
            SWFatturaPA = True
        Else
            SWFatturaPA = False
        End If
        Session(CSTFATTURAPA) = SWFatturaPA 'giu250814
        '---------
        'GIU090118
        Dim mySWSplitIVA As String = Session(CSTSPLITIVA)
        If IsNothing(mySWSplitIVA) Then
            SWSplitIVA = False
        End If
        If String.IsNullOrEmpty(mySWSplitIVA) Then
            SWSplitIVA = False
        ElseIf Session(CSTSPLITIVA) Then
            SWSplitIVA = True
        Else
            SWSplitIVA = False
        End If
        Session(CSTSPLITIVA) = SWSplitIVA
        '---------
        'giu270219 
        If chkScGiacenza.Visible = True And chkScGiacenza.Checked = True Then
            mySCGiacenza = True
        End If
        '---------
        strDesTipoDocumento = DesTD(TipoDoc)
        'GIU231023
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        TipoDoc = Session(CSTTIPODOC)
        '---------
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'GIU090312 NEL DUBBIO ESEMIO CM SM E MM VERIFICO IL CODICE CHE è STATO INSERITO SE 9=For O 1=Cli
        'mi serve per sapere quale prezzo prendere se di acquisto o di LISTINO VENDITA
        If TabCliFor = "CliFor" Then
            If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
                TabCliFor = "Cli"
            ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
                TabCliFor = "For"
            End If
            myTabCliFor = TabCliFor
        End If
    End Function
    'giu170412
    Public Function CKPrezzoALCSG(ByRef _PrezzoAL As String, ByRef _strErrore As String) As Boolean
        'SG=CSTSEGNOGIACENZA
        'A=Acquisto
        'L=Listino
        'C=Costo (FIFO)
        CKPrezzoALCSG = True
        If CKCSTTipoDoc() = False Then
            _strErrore = "Errore: TIPO CONTRATTO SCONOSCIUTO"
            Return False
        End If
        'giu170412 dal codice causale determino quale prezzo è il prezzo di Listino (Acquisto o Vendita)
        'C/DEPOSITO: Segno_Giacenza "-" Segno_Deposito = "+"
        '            Segno_Giacenza "+" Segno_Deposito = "-"
        _PrezzoAL = "L" 'Listino
        If txtCodCausale.Text.Trim = "" Or txtCodCausale.Text.Trim = "0" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
            'giu301119
            ' ''_strErrore = "CODICE CAUSALE OBBLIGATORIA"
            ' ''Return False
            If _PrezzoAL = "" Then _PrezzoAL = "L"
            Exit Function
        End If
        Dim strErrore As String = ""
        'giu140512
        Dim myCodCaus As Integer = 0
        If txtCodCausale.Text.Trim = "" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
            myCodCaus = -1
        Else
            myCodCaus = CLng(txtCodCausale.Text.Trim)
        End If
        '----------
        Dim RKCausMag As StrCausMag = GetDatiCausMag(myCodCaus, strErrore)
        If IsNothing(RKCausMag.Codice) Then
            _strErrore = "CODICE CAUSALE NON PRESENTE IN TABELLA"
            Return False
        ElseIf strErrore.Trim <> "" Then
            _strErrore = strErrore.Trim
            Return False
        End If
        _PrezzoAL = RKCausMag.PrezzoAL
        If _PrezzoAL = "" Then _PrezzoAL = "L"
        'giu190412
        Dim Segno_Giacenza As String = RKCausMag.Segno_Giacenza
        Session(CSTSEGNOGIACENZA) = Segno_Giacenza 'GIU270412
        If TipoDoc = SWTD(TD.OrdFornitori) Or TipoDoc = SWTD(TD.DocTrasportoFornitori) Or _
           TipoDoc = SWTD(TD.PropOrdFornitori) Or _
           TipoDoc = SWTD(TD.CaricoMagazzino) Or _
           (TipoDoc = SWTD(TD.MovimentoMagazzino) And Segno_Giacenza = "+") Or _
           TabCliFor = "For" Then
            _PrezzoAL = "A"
        End If
    End Function
    'Chiamato da TD1 DETTAGLI per TD3 TOTALI,SPESE,SCADENZARIO
    Public Function CalcolaTotSpeseScad(ByVal dsDocDett As DataSet, ByRef Iva() As Integer, _
                                        ByRef Imponibile() As Decimal, ByRef Imposta() As Decimal, _
                                        ByVal DecimaliValuta As Integer, ByRef MoltiplicatoreValuta As Integer, _
                                        ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, _
                                        ByVal ScontoCassa As Decimal, Optional ByVal Listino As Long = 9999, _
                                        Optional ByVal TipoDocumento As String = "CA", _
                                        Optional ByVal Abbuono As Decimal = 0, _
                                        Optional ByRef strErrore As String = "", _
                                        Optional ByRef TotaleLordoMercePL As Decimal = 0, _
                                        Optional ByRef Deduzioni As Decimal = 0) As Boolean 'giu020519
        strErrore = ""
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
        '-
        Session(CSTDURATANUM) = txtDurataNum.Text.Trim
        Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
        Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
        Session(CSTIDPAG) = txtPagamento.Text.Trim

        If WUC_ContrattiSpeseTraspTot1.CalcolaTotSpeseScad(dsDocDett, Iva, Imponibile, Imposta, _
                               DecimaliValuta, MoltiplicatoreValuta, _
                               Totale, TotaleLordoMerce, ScontoCassa, Listino, _
                               TipoDocumento, Abbuono, strErrore, TotaleLordoMercePL, Deduzioni) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Function
        End If
    End Function

    Private Sub BtnSetEnabledTo(ByVal SW As Boolean)
        btnAggiorna.Enabled = SW
        btnAnnulla.Enabled = SW
        btnCollegaOC.Enabled = SW
        btnElimina.Enabled = SW
        btnModifica.Enabled = SW
        btnNuovo.Enabled = SW
        btnDuplicaDNum.Enabled = SW
        btnNuovaDNum.Enabled = SW
        btnGeneraAttDNum.Enabled = SW
        btnStampa.Enabled = SW : btnVerbale.Enabled = SW
        If WUC_ContrattiSpeseTraspTot1.CKAttEvPagEv = True And btnGeneraAttDNum.BackColor <> SEGNALA_KO Then
            btnDuplicaDNum.Enabled = False
            btnNuovaDNum.Enabled = False
            btnGeneraAttDNum.Enabled = False
        ElseIf WUC_ContrattiDett1.CKGridDett = False Then
            btnDuplicaDNum.Enabled = False
            btnNuovaDNum.Enabled = False
        End If
    End Sub
    'giu180420
    Public Function CKAttEvPagEv() As Boolean
        CKAttEvPagEv = WUC_ContrattiSpeseTraspTot1.CKAttEvPagEv()
    End Function
#Region " Dati Intestazione"
    Private Sub PopolaTxtDocT()
        Session(CSTCODFILIALE) = String.Empty : lblDestSel.Text = "" : lblDestSel.ToolTip = "" 'giu220520
        SfondoCampiDocT()
        Try
            If (dvDocT Is Nothing) Then
                DsDocT = Session("DsDocT")
                dvDocT = New DataView(DsDocT.ContrattiT)
            End If
            If dvDocT.Count = 0 Then
                AzzeraTxtDocT()
                lblMessDoc.Text = "DOCUMENTO NON COMPLETO" : lblMessDoc.Visible = True
                Exit Sub
            End If
            Session(CSTSTATODOC) = dvDocT.Item(0).Item("StatoDoc").ToString
            Session(CSTSTATODOCSEL) = dvDocT.Item(0).Item("StatoDoc").ToString 'giu300722 per ripristinare lo stato se si annullano le modifiche
            If Session(CSTSTATODOC) = "5" Then
                lblMessDoc.Text = "DOCUMENTO NON COMPLETO<br>Per non rigenerare i periodi usare la funzione Cambia Stato del Contratto" : lblMessDoc.Visible = True
            End If
            txtNumero.AutoPostBack = False : txtDataDoc.AutoPostBack = False
            txtNumero.Text = dvDocT.Item(0).Item("Numero").ToString
            ' ''txtRevNDoc.AutoPostBack = False
            ' ''txtRevNDoc.Text = dvDocT.Item(0).Item("RevisioneNDoc").ToString
            txtDataDoc.BackColor = SEGNALA_OK
            If IsDate(dvDocT.Item(0).Item("Data_Doc")) Then
                txtDataDoc.Text = Format(dvDocT.Item(0).Item("Data_Doc"), FormatoData)
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
            Else
                txtDataDoc.Text = ""
                Session(CSTDATADOC) = ""
            End If
            txtNumero.AutoPostBack = True : txtDataDoc.AutoPostBack = True
            ' ''txtRevNDoc.AutoPostBack = True :
            'GIU101219
            txtDurataNum.Text = dvDocT.Item(0).Item("DurataNum").ToString
            PosizionaItemDDL(dvDocT.Item(0).Item("DurataTipo").ToString.Trim, DDLDurataTipo)
            txtNVisite.Text = dvDocT.Item(0).Item("NVisite").ToString
            '-
            txtDataInizio.BackColor = SEGNALA_OK
            If IsDate(dvDocT.Item(0).Item("DataInizio")) Then
                txtDataInizio.Text = Format(dvDocT.Item(0).Item("DataInizio"), FormatoData)
            Else
                txtDataInizio.Text = ""
            End If
            txtDataFine.BackColor = SEGNALA_OK
            If IsDate(dvDocT.Item(0).Item("DataFine")) Then
                txtDataFine.Text = Format(dvDocT.Item(0).Item("DataFine"), FormatoData)
            Else
                txtDataFine.Text = ""
            End If
            txtDataAccettazione.BackColor = SEGNALA_OK
            If IsDate(dvDocT.Item(0).Item("DataAccetta")) Then
                txtDataAccettazione.Text = Format(dvDocT.Item(0).Item("DataAccetta"), FormatoData)
            Else
                txtDataAccettazione.Text = ""
            End If
            Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
            Session(CSTDATAFINE) = txtDataFine.Text.Trim
            Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
            '-
            txtTipoFatt.AutoPostBack = False : DDLTipoFatt.AutoPostBack = False
            txtTipoFatt.Text = dvDocT.Item(0).Item("Tipo_Fatturazione").ToString
            PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
            txtTipoFatt.AutoPostBack = True : DDLTipoFatt.AutoPostBack = True
            '---------
            If IsDate(dvDocT.Item(0).Item("Data_Validita")) Then
                txtDataValidita.Text = Format(dvDocT.Item(0).Item("Data_Validita"), FormatoData)
            Else
                txtDataValidita.Text = ""
            End If
            txtNGG_Validita.Text = dvDocT.Item(0).Item("NGG_Validita").ToString
            If IsDate(dvDocT.Item(0).Item("DataOraConsegna")) Then
                txtDataConsegna.Text = Format(dvDocT.Item(0).Item("DataOraConsegna"), FormatoData)
            Else
                txtDataConsegna.Text = ""
            End If
            txtNGG_Consegna.Text = dvDocT.Item(0).Item("NGG_Consegna").ToString
            '-
            'giu071211 nuovi campi SWTipoEvTotale SWTipoEvSaldo
            If IsDBNull(dvDocT.Item(0).Item("SWTipoEvTotale")) Then
                optTipoEvTotaleTotale.Checked = True
                optTipoEvTotaleParziale.Checked = False
            ElseIf dvDocT.Item(0).Item("SWTipoEvTotale") Then
                optTipoEvTotaleTotale.Checked = True
                optTipoEvTotaleParziale.Checked = False
            Else
                optTipoEvTotaleTotale.Checked = False
                optTipoEvTotaleParziale.Checked = True
            End If
            '-
            txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
            txtPagamento.Text = dvDocT.Item(0).Item("TipoContratto").ToString
            PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
            Session(CSTIDPAG) = txtPagamento.Text.Trim
            txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
            chkSWTipoModSint.Checked = IIf(IsDBNull(dvDocT.Item(0).Item("SWTipoModSint")), False, CBool(dvDocT.Item(0).Item("SWTipoModSint")))
            'GIU280120 RIEMPITI SOPRA IMPORTANTE
            Session(CSTDURATANUM) = txtDurataNum.Text.Trim
            Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
            Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
            'giu290120 calcolo delle rate
            'giu290320 nuovo algoritmo per la determinazione NumRate
            Session(CSTIDPAG) = txtPagamento.Text.Trim
            Dim CodPag As String = Session(CSTIDPAG)
            If IsNothing(CodPag) Then CodPag = ""
            If String.IsNullOrEmpty(CodPag) Then CodPag = ""
            If Not IsNumeric(CodPag) Then CodPag = ""
            If CodPag = "" Or CodPag = "0" Then CodPag = ""
            Dim NumRate As Integer = 0 : Dim NumGG As Integer = 0 : Dim mystrErrore As String = ""
            ' ''If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            ' ''    TipoDoc = SWTD(TD.ContrattoAssistenza)
            ' ''End If
            Session(CSTANTICIPATO) = "" 'GIU150424
            Dim dsPag As New DataSet
            Dim rowPag() As DataRow = Nothing
            Dim strSQL As String = "" : Dim Comodo = ""
            Dim ObjDB As New DataBaseUtility
            strSQL = "Select * From TipoContratto WHERE Codice = " & CodPag.Trim
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsPag)
                If (dsPag.Tables.Count > 0) Then
                    If (dsPag.Tables(0).Rows.Count > 0) Then
                        rowPag = dsPag.Tables(0).Select()
                        Session(CSTANTICIPATO) = rowPag(0).Item("Anticipato") 'GIU150424
                    Else
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' ''ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                        ' ''Exit Sub
                    End If
                Else
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ' ''ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                    ' ''Exit Sub
                End If
            Catch Ex As Exception
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "Lettura TipoContratto: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Sub
            End Try
            'giu230320 TipoPagamento=4 Pagamento alla scadenza evasione attività (VER DAE O ALTRO QUELLO DEFINITO IN TABELLA CodVisita)
            Dim DecimaliValuta As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliValuta) Then DecimaliValuta = ""
            If String.IsNullOrEmpty(DecimaliValuta) Then DecimaliValuta = ""
            If DecimaliValuta = "" Or Not IsNumeric(DecimaliValuta) Then
                DecimaliValuta = "2" 'Euro
            End If
            If (rowPag Is Nothing) Then
                If WUC_ContrattiSpeseTraspTot1.CalcolaNumRate(NumRate, NumGG, mystrErrore) = False Then
                    If mystrErrore.Trim <> "" Then
                        lblNumRate.Text = mystrErrore
                    Else
                        lblNumRate.Text = "ERRORE CalcolaNumRate"
                    End If
                Else
                    lblNumRate.Text = "Rate di fatturazione: " & FormattaNumero(NumRate)
                End If
                lblNumRate.Text = "ERR.:TipoContratto non valido."
            ElseIf rowPag(0).Item("TipoPagamento") = 4 Then
                'giu200420 DATE DI SCADENZE RATE
                NumRate = 0
                If IsDBNull(dvDocT.Item(0).Item("ScadPagCA")) Then
                    dvDocT.Item(0).Item("ScadPagCA") = ""
                End If
                Dim lineaSplit As String() = dvDocT.Item(0).Item("ScadPagCA").Split(";")
                NumRate = (lineaSplit.Count) / 8
                'GIU191223
                '''If dvDocT.Item(0).Item("ScadPagCA") <> "" Then
                '''    Dim lineaSplit As String() = dvDocT.Item(0).Item("ScadPagCA").Split(";")
                '''    For i = 0 To lineaSplit.Count - 1
                '''        If lineaSplit(i).Trim <> "" And (i + 8) <= lineaSplit.Count - 1 Then 'giu191223 da i + 6
                '''            i += 8 'giu191223 da i + 6
                '''            NumRate += 1
                '''        End If
                '''    Next
                '''End If
                lblNumRate.Text = "Rate di fatturazione: " & FormattaNumero(NumRate)
            Else
                If WUC_ContrattiSpeseTraspTot1.CalcolaNumRate(NumRate, NumGG, mystrErrore) = False Then
                    If mystrErrore.Trim <> "" Then
                        lblNumRate.Text = mystrErrore
                    Else
                        lblNumRate.Text = "ERRORE CalcolaNumRate"
                    End If
                Else
                    lblNumRate.Text = "Rate di fatturazione: " & FormattaNumero(NumRate)
                End If
            End If
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '-
            txtRiferimento.Text = dvDocT.Item(0).Item("Riferimento").ToString.Trim

            If SWRifDoc = True And txtRiferimento.Text.Trim.Length > 20 Then
                txtRiferimento.BackColor = SEGNALA_KO : txtRiferimento.ToolTip = "Lunghezza massima consentita 20 caratteri a partire dal 01/01/2019"
            Else
                txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = txtRiferimento.Text.Trim
            End If
            If IsDBNull(dvDocT.Item(0).Item("Data_Riferimento")) Then
                txtDataRif.Text = ""
            ElseIf IsDate(dvDocT.Item(0).Item("Data_Riferimento")) Then
                txtDataRif.Text = Format(dvDocT.Item(0).Item("Data_Riferimento"), FormatoData)
            Else
                txtDataRif.Text = ""
            End If
            '---------------------------------------------------
            txtCIG.Text = dvDocT.Item(0).Item("CIG").ToString.Trim
            txtCUP.Text = dvDocT.Item(0).Item("CUP").ToString.Trim
            '-
            Session(CSTUpgAngrProvvCG) = "" 'GIU111012
            txtCodCliForFilProvv.AutoPostBack = False
            If dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim = "" Then
                txtCodCliForFilProvv.Text = dvDocT.Item(0).Item("Cod_Cliente").ToString
                Session(CSTCODCOGE) = txtCodCliForFilProvv.Text
                Session(COD_CLIENTE) = txtCodCliForFilProvv.Text
                If txtCodCliForFilProvv.Text.Trim <> "" Then
                    lblLabelCliForFilProvv.Text = "Cliente"
                    Try
                        lblLabelCliForFilProvv.Text += " (" + dvDocT.Item(0).Item("IPA").ToString.Trim + ")"
                    Catch ex As Exception

                    End Try
                    Session(CSTTABCLIFOR) = "Cli"
                Else
                    txtCodCliForFilProvv.Text = dvDocT.Item(0).Item("Cod_Fornitore").ToString
                    If txtCodCliForFilProvv.Text.Trim <> "" Then
                        lblLabelCliForFilProvv.Text = "Fornitore"
                        Session(CSTTABCLIFOR) = "For"
                        'giu220113
                        Session(COD_CLIENTE) = ""
                        '---------
                    End If
                End If
                If IsNumeric(dvDocT.Item(0).Item("Cod_Filiale").ToString) Then
                    Session(CSTCODFILIALE) = dvDocT.Item(0).Item("Cod_Filiale").ToString
                    PICFPagBancaAgeLisByCliFor(True, , dvDocT.Item(0).Item("Cod_Filiale"))
                Else
                    Session(CSTCODFILIALE) = ""
                    PICFPagBancaAgeLisByCliFor(True, , 0)
                End If
            Else
                'giu220113
                Session(COD_CLIENTE) = String.Empty
                '---------
                If Session(CSTTABCLIFOR) = "Cli" Then
                    lblLabelCliForFilProvv.Text = "Cliente"
                    Try
                        lblLabelCliForFilProvv.Text += " (" + dvDocT.Item(0).Item("IPA").ToString.Trim + ")"
                    Catch ex As Exception

                    End Try
                End If

                If Session(CSTTABCLIFOR) = "For" Then lblLabelCliForFilProvv.Text = "Fornitore"
                If Session(CSTTABCLIFOR) = "CliFor" Then
                    lblLabelCliForFilProvv.Text = "Cli./For."
                    Try
                        lblLabelCliForFilProvv.Text += " (" + dvDocT.Item(0).Item("IPA").ToString.Trim + ")"
                    Catch ex As Exception

                    End Try
                End If

                txtCodCliForFilProvv.Text = "(" & dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim & ")"
                Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text 'GIU111012
                'Provvisorie non hanno destinazioni
                Session(CSTCODCOGE) = String.Empty
                Session(CSTCODFILIALE) = String.Empty
                PopolaDestCliFor()
                WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
                VisAnagrProvv("(" & dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim & ")")
            End If
            txtCodCliForFilProvv.AutoPostBack = True

            txtDestinazione1.Text = dvDocT.Item(0).Item("Destinazione1").ToString.Trim
            txtDestinazione2.Text = dvDocT.Item(0).Item("Destinazione2").ToString.Trim
            txtDestinazione3.Text = dvDocT.Item(0).Item("Destinazione3").ToString.Trim
            '-
            lblABI.Text = dvDocT.Item(0).Item("ABI").ToString.Trim
            lblCAB.Text = dvDocT.Item(0).Item("CAB").ToString.Trim
            lblIBAN.Text = dvDocT.Item(0).Item("IBAN").ToString.Trim
            lblContoCorrente.Text = dvDocT.Item(0).Item("ContoCorrente").ToString.Trim
            DDLBancheIBAN.AutoPostBack = False
            PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
            If DDLBancheIBAN.SelectedIndex = 0 Then 'giu120411
                lblABI.Text = "" : lblCAB.Text = ""
                lblIBAN.Text = ""
                lblContoCorrente.Text = ""
                lblBanca.Text = "" : lblFiliale.Text = ""
            End If
            lblABICAB()
            DDLBancheIBAN.AutoPostBack = True
            'giu161219
            txtCodRespArea.AutoPostBack = False
            txtCodRespArea.Text = dvDocT.Item(0).Item("RespArea").ToString
            Session(IDRESPAREA) = txtCodRespArea.Text.Trim
            PosizionaItemDDLByTxt(txtCodRespArea, DDLRespArea)
            Session(IDRESPAREA) = txtCodRespArea.Text.Trim
            txtCodRespArea.AutoPostBack = True
            '-
            txtCodRespVisite.AutoPostBack = False
            txtCodRespVisite.Text = dvDocT.Item(0).Item("RespVisite").ToString
            Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
            PosizionaItemDDLByTxt(txtCodRespVisite, DDLRespVisite)
            Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
            txtCodRespVisite.AutoPostBack = True
            '-
            txtCodAgente.AutoPostBack = False
            txtCodAgente.Text = dvDocT.Item(0).Item("Cod_Agente").ToString
            Session(IDAGENTE) = txtCodAgente.Text.Trim
            PosizionaItemDDLByTxt(txtCodAgente, DDLAgenti)
            Session(IDAGENTE) = txtCodAgente.Text.Trim
            txtCodAgente.AutoPostBack = True
            'giu010412
            Dim myCAge As String = txtCodAgente.Text.Trim
            If Not IsNumeric(myCAge) Then myCAge = "0"
            Dim _CAge As Integer = CtrAgente(_CAge)
            If CInt(myCAge) <> _CAge Then
                lblMessAge.ForeColor = SEGNALA_KO
                lblMessAge.Text = "Assegnato: (" & _CAge.ToString.Trim & ")"
            Else
                lblMessAge.ForeColor = Drawing.Color.Blue
                lblMessAge.Text = ""
            End If
            '-
            txtCodCausale.AutoPostBack = False : DDLCausali.AutoPostBack = False
            txtCodCausale.Text = dvDocT.Item(0).Item("Cod_Causale").ToString
            PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
            txtCodCausale.AutoPostBack = True : DDLCausali.AutoPostBack = True
            'giu260412
            If IsDBNull(dvDocT.Item(0).Item("ADeposito")) Then
                checkADeposito.Checked = False
            ElseIf dvDocT.Item(0).Item("ADeposito") = True Then
                checkADeposito.Checked = True
            Else
                checkADeposito.Checked = False
            End If
            txtListino.AutoPostBack = False : DDLListini.AutoPostBack = False
            txtListino.Text = dvDocT.Item(0).Item("Listino").ToString
            Session(IDLISTINO) = txtListino.Text.Trim
            PosizionaItemDDLByTxt(txtListino, DDLListini)
            Session(IDLISTINO) = txtListino.Text.Trim
            txtListino.AutoPostBack = True : DDLListini.AutoPostBack = True
            LnkRegimeIVA.Text = "Regime IVA " & dvDocT.Item(0).Item("Cod_Iva").ToString.Trim
            Session(CSTREGIMEIVA) = dvDocT.Item(0).Item("Cod_IVA").ToString
            lblCodValuta.Text = dvDocT.Item(0).Item("Cod_Valuta").ToString
            Session(CSTVALUTADOC) = dvDocT.Item(0).Item("Cod_Valuta").ToString
            Session(CSTDECIMALIVALUTADOC) = 0
            lblValuta()
            'giu070814
            chkFatturaPA.AutoPostBack = False
            If IsDBNull(dvDocT.Item(0).Item("FatturaPA")) Then
                chkFatturaPA.Checked = False
            ElseIf dvDocT.Item(0).Item("FatturaPA") = True Then
                chkFatturaPA.Checked = True
            Else
                chkFatturaPA.Checked = False
            End If
            SWFatturaPA = chkFatturaPA.Checked
            Session(CSTFATTURAPA) = SWFatturaPA
            chkFatturaPA.AutoPostBack = True
            'GIU250219
            Dim myVar As String = ""
            If IsDBNull(dvDocT.Item(0).Item("TipoRapportoPA")) Then
                myVar = ""
            Else
                myVar = dvDocT.Item(0).Item("TipoRapportoPA").ToString.Trim
            End If
            PosizionaItemDDL(myVar, DDLTipoRapp)
            If IsDBNull(dvDocT.Item(0).Item("CodiceMagazzino")) Then
                myVar = ""
            Else
                myVar = dvDocT.Item(0).Item("CodiceMagazzino").ToString.Trim
            End If
            PosizionaItemDDL(myVar, ddlMagazzino)
            If IsDBNull(dvDocT.Item(0).Item("Acconto")) Then
                ChkAcconto.Checked = False
            ElseIf dvDocT.Item(0).Item("Acconto") <> 0 Then
                ChkAcconto.Checked = True
            Else
                ChkAcconto.Checked = False
            End If
            'giu040118
            If IsDBNull(dvDocT.Item(0).Item("SplitIVA")) Then
                SWSplitIVA = False
            ElseIf dvDocT.Item(0).Item("SplitIVA") = True Then
                SWSplitIVA = True
            Else
                SWSplitIVA = False
            End If
            Session(CSTSPLITIVA) = SWSplitIVA
            'GIU080814------------------------------------------------------------------
            Dim StrErrore As String = ""
            If txtCodCliForFilProvv.Text.Trim <> "" Then
                Dim mySplitIVA As Boolean = False
                If Documenti.CKClientiIPAByIDConORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, StrErrore) = True Then
                    If StrErrore.Trim <> "" Then
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
                        ' ''Exit Sub
                        'non mi frega
                    ElseIf SWFatturaPA = False Then
                        chkFatturaPA.BackColor = SEGNALA_KO
                    End If
                Else
                    If StrErrore.Trim <> "" Then
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
                        ' ''Exit Sub
                        'non mi frega
                    ElseIf SWFatturaPA = True Then
                        chkFatturaPA.BackColor = SEGNALA_KO
                    End If
                End If
                If mySplitIVA <> SWSplitIVA Then
                    WUC_ContrattiSpeseTraspTot1.SetErrChkSplitIVA(False)
                End If
            End If
            '-----------------------------------------------------------------------------
            'giu091018
            chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False
            If IsDBNull(dvDocT.Item(0).Item("FatturaAC")) Then
                chkFatturaAC.Checked = False
            ElseIf dvDocT.Item(0).Item("FatturaAC") = True Then
                chkFatturaAC.Checked = True
            Else
                chkFatturaAC.Checked = False
            End If
            If IsDBNull(dvDocT.Item(0).Item("ScGiacenza")) Then
                chkScGiacenza.Checked = False
            ElseIf dvDocT.Item(0).Item("ScGiacenza") = True Then
                chkScGiacenza.Checked = True
            Else
                chkScGiacenza.Checked = False
            End If
            If chkFatturaAC.Visible = True And chkFatturaAC.Enabled = True And chkFatturaAC.Checked = True Then
                chkScGiacenza.Enabled = True
            ElseIf chkFatturaAC.Visible = True And chkFatturaAC.Enabled = True And chkFatturaAC.Checked = False Then
                chkScGiacenza.Enabled = False
            End If
            chkFatturaAC.AutoPostBack = True : chkScGiacenza.AutoPostBack = False
            '---------
            txtDesRefInt.Text = dvDocT.Item(0).Item("DesRefInt").ToString.Trim
            txtNoteDocumento.Text = dvDocT.Item(0).Item("NoteDocumento").ToString.Trim
            If TipoDoc <> SWTD(TD.ContrattoAssistenza) And TipoDoc <> SWTD(TD.TipoContratto) Then 'giu041219
                'giu260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
                Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
                If CkNumDoc = -1 Then
                    Exit Sub
                End If
                Dim myNumDoc As Long
                If Not IsNumeric(txtNumero.Text.Trim) Then Exit Sub
                myNumDoc = txtNumero.Text.Trim
                CkNumDoc = CkNumDoc - 1
                If CkNumDoc < 1 Then
                    CkNumDoc = 1
                End If
                If myNumDoc - 1 = CkNumDoc Then
                    'ok proseguo
                ElseIf myNumDoc > CkNumDoc Then
                    txtNumero.BackColor = SEGNALA_KO
                    txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. MAGGIORE di quello inserito. !!!VERIFICARE !!!"
                End If
            End If

        Catch ex As Exception
            Session(ERRORE) = "(PopolaTxtDocT) " & ex.Message.Trim
        End Try
    End Sub

    Private Sub PICFPagBancaAgeLisByCliFor(ByVal SWDoc As Boolean, Optional ByVal SWBanca As Boolean = False, Optional ByVal CodFil As Integer = 0)
        txtCodCliForFilProvv.ToolTip = ""
        lblCliForFilProvv.ToolTip = ""
        'giu080412
        lblCliForFilProvv.ForeColor = Drawing.Color.Black
        lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
        '---------
        If txtCodCliForFilProvv.Text.Trim = "" Then
            'giu220113
            Session(COD_CLIENTE) = String.Empty
            Session(CSTCODCOGE) = String.Empty 'giu240615
            '---------
            LnkRegimeIVA.Text = "Regime IVA"
            lblCliForFilProvv.Text = "" 'giu080412
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            Session(CSTUpgAngrProvvCG) = String.Empty 'GIU111012
            PopolaDestCliFor()
            WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
            'PER ADESSO LE INIT LE FACCIO
            DDLListini.AutoPostBack = False
            Call PosizionaItemDDL("1", DDLListini) 'giu191219
            DDLListini.AutoPostBack = True
            DDLAgenti.SelectedIndex = 0
            Session(IDAGENTE) = ""
            Session(IDLISTINO) = "1"
            txtListino.AutoPostBack = False
            txtListino.Text = "1"
            txtListino.AutoPostBack = True
            Session(CSTVALUTADOC) = "Euro"
            Session(CSTDECIMALIVALUTADOC) = 2
            Session(CSTREGIMEIVA) = "0"
            Session(CSTFATTURAPA) = False
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = False
            chkFatturaPA.AutoPostBack = True
            Session(CSTSPLITIVA) = False
            WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
            Exit Sub
        End If
        'Anagrafiche provvisorie EXIT SUB
        If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
            LnkRegimeIVA.Text = "Regime IVA"
            'giu220113
            Session(COD_CLIENTE) = String.Empty
            '---------
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text.Trim 'GIU111012
            PopolaDestCliFor()
            WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
            'PER ADESSO LE INIT LE FACCIO
            DDLListini.AutoPostBack = False
            Call PosizionaItemDDL("1", DDLListini) 'giu191219
            DDLListini.AutoPostBack = True
            DDLAgenti.SelectedIndex = 0
            Session(IDAGENTE) = ""
            Session(IDLISTINO) = "1"
            txtListino.AutoPostBack = False
            txtListino.Text = "1"
            txtListino.AutoPostBack = True
            Session(CSTVALUTADOC) = "Euro"
            Session(CSTDECIMALIVALUTADOC) = 2
            Session(CSTREGIMEIVA) = "0"
            Session(CSTFATTURAPA) = False
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = False
            chkFatturaPA.AutoPostBack = True
            Session(CSTSPLITIVA) = False
            WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
            Exit Sub
        End If
        'giu191211
        Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim 'giu290321
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            Session(COD_CLIENTE) = ""
        End If
        If TabCliFor = "Cli" Then
            lblLabelCliForFilProvv.Text = "Cliente"
            Session(CSTTABCLIFOR) = "Cli"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
        ElseIf TabCliFor = "For" Then
            lblLabelCliForFilProvv.Text = "Fornitore" : Session(CSTTABCLIFOR) = "For"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Fornitori] WHERE ([Codice_CoGe] = @Codice)"
        Else 'DEFAULT giu191211
            lblLabelCliForFilProvv.Text = "Cli./For."
            Session(CSTTABCLIFOR) = "CliFor"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)"
        End If
        '----------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        'giu220113
        If TabCliFor <> "Cli" Then
            Session(COD_CLIENTE) = String.Empty
        Else
            Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        End If
        '---------
        If SWDoc = False Then
            btnbtnGeneraAttDNumColorRED(True)
            lblMessDoc.Text = "ATTENZIONE, verificare nelle singole App., <br> eventuali Luoghi App. non valide per questo cliente." : lblMessDoc.Visible = True
            '-
            Session(CSTCODFILIALE) = ""
        ElseIf CodFil > 0 Then
            Session(CSTCODFILIALE) = CodFil.ToString.Trim
        End If

        PopolaDestCliFor()
        '--------
        Dim dvCliFor As DataView : Dim strInfo As String = ""
        dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
        If (dvCliFor Is Nothing) Then
            lblCliForFilProvv.Text = ""
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            If SWDoc = False Then
                txtPagamento.Text = "" : DDLPagamento.SelectedIndex = 0
                If SWBanca = True Then
                    lblABI.Text = "" : lblBanca.Text = ""
                    lblCAB.Text = "" : lblFiliale.Text = ""
                    lblIBAN.Text = "" : lblContoCorrente.Text = ""
                End If
                DDLListini.AutoPostBack = False
                Call PosizionaItemDDL("1", DDLListini) 'giu191219
                DDLListini.AutoPostBack = True
                DDLAgenti.SelectedIndex = 0
                Session(IDAGENTE) = ""
                Session(IDLISTINO) = "1"
                txtListino.AutoPostBack = False
                txtListino.Text = "1"
                txtListino.AutoPostBack = True
                Session(CSTVALUTADOC) = "Euro"
                Session(CSTDECIMALIVALUTADOC) = 2
                LnkRegimeIVA.Text = "Regime IVA"
                Session(CSTREGIMEIVA) = "0"
                Session(CSTFATTURAPA) = False
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                Session(CSTSPLITIVA) = False
                WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
            End If
            Exit Sub
        End If
        If dvCliFor.Count > 0 Then
            If dvCliFor.Count > 0 Then
                Try
                    lblLabelCliForFilProvv.Text += " (" + dvCliFor.Item(0).Item("IPA").ToString.Trim + ")"
                Catch ex As Exception

                End Try
                'giu090412
                If Not IsDBNull(dvCliFor.Item(0).Item("NoFatt")) Then
                    If dvCliFor.Item(0).Item("NoFatt") = True Then
                        lblCliForFilProvv.ForeColor = SEGNALA_KO
                        lblPICF.ForeColor = SEGNALA_KO : lblIndirizzo.ForeColor = SEGNALA_KO : lblLocalita.ForeColor = SEGNALA_KO
                    Else
                        lblCliForFilProvv.ForeColor = Drawing.Color.Black
                        lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
                    End If
                Else
                    lblCliForFilProvv.ForeColor = Drawing.Color.Black
                    lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
                End If
                '---------
                lblCliForFilProvv.Text = dvCliFor.Item(0).Item("Rag_Soc").ToString & " " & dvCliFor.Item(0).Item("Denominazione").ToString
                strInfo += txtCodCliForFilProvv.Text.Trim & " "
                strInfo += dvCliFor.Item(0).Item("Rag_Soc").ToString & " "
                strInfo += dvCliFor.Item(0).Item("Denominazione").ToString & " "
                strInfo += dvCliFor.Item(0).Item("Riferimento").ToString & " "
                lblLabelPICF.Text = "Partita IVA"
                lblPICF.Text = dvCliFor.Item(0).Item("Partita_IVA").ToString
                strInfo += "P.IVA: " & lblPICF.Text & " "
                If SWDoc = False Then
                    LnkRegimeIVA.Text = "Regime IVA " & dvCliFor.Item(0).Item("Regime_IVA").ToString
                    'giu171219 txtPagamento.Text = dvCliFor.Item(0).Item("Pagamento_N").ToString
                    'giu171219 PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
                    'GIU280120 
                    Session(CSTDURATANUM) = txtDurataNum.Text.Trim
                    Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
                    Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
                    Session(CSTIDPAG) = txtPagamento.Text.Trim
                    'GIU071211
                    ' ''If SWBanca = True Then
                    ' ''    lblABI.Text = dvCliFor.Item(0).Item("ABI_N").ToString
                    ' ''    lblCAB.Text = dvCliFor.Item(0).Item("CAB_N").ToString
                    ' ''    lblIBAN.Text = dvCliFor.Item(0).Item("NazIBAN").ToString + dvCliFor.Item(0).Item("CINEUIBAN").ToString + dvCliFor.Item(0).Item("CIN").ToString + dvCliFor.Item(0).Item("ABI").ToString + dvCliFor.Item(0).Item("CAB").ToString + dvCliFor.Item(0).Item("Conto_Corrente").ToString
                    ' ''    lblContoCorrente.Text = dvCliFor.Item(0).Item("Conto_Corrente").ToString
                    ' ''    PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
                    ' ''Else
                    ' ''    lblABI.Text = GetParamGestAzi(Session(ESERCIZIO)).ABI
                    ' ''    lblCAB.Text = GetParamGestAzi(Session(ESERCIZIO)).CAB
                    ' ''    lblIBAN.Text = GetParamGestAzi(Session(ESERCIZIO)).NazIBAN + GetParamGestAzi(Session(ESERCIZIO)).CINEUIBAN + GetParamGestAzi(Session(ESERCIZIO)).CIN + GetParamGestAzi(Session(ESERCIZIO)).ABI + GetParamGestAzi(Session(ESERCIZIO)).CAB + GetParamGestAzi(Session(ESERCIZIO)).CC
                    ' ''    lblContoCorrente.Text = GetParamGestAzi(Session(ESERCIZIO)).CC
                    ' ''    PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
                    ' ''End If
                    'Richiesta di Cinzia del 05012012 mettere la banca di DEFAULT
                    'giu150212 nuovo campo in CLIENTI IBAN_Ditta
                    lblIBAN.Text = dvCliFor.Item(0).Item("IBAN_Ditta").ToString
                    lblABI.Text = Mid(Trim(lblIBAN.Text), 6, 5)
                    lblCAB.Text = Mid(Trim(lblIBAN.Text), 11, 5)
                    lblContoCorrente.Text = Mid(Trim(lblIBAN.Text), 16)
                    'SWBanca = True And 
                    If lblIBAN.Text.Trim = "" Then
                        lblABI.Text = GetParamGestAzi(Session(ESERCIZIO)).ABI
                        lblCAB.Text = GetParamGestAzi(Session(ESERCIZIO)).CAB
                        lblIBAN.Text = GetParamGestAzi(Session(ESERCIZIO)).NazIBAN + GetParamGestAzi(Session(ESERCIZIO)).CINEUIBAN + GetParamGestAzi(Session(ESERCIZIO)).CIN + GetParamGestAzi(Session(ESERCIZIO)).ABI + GetParamGestAzi(Session(ESERCIZIO)).CAB + GetParamGestAzi(Session(ESERCIZIO)).CC
                        lblContoCorrente.Text = GetParamGestAzi(Session(ESERCIZIO)).CC
                    End If
                    PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
                    '--------
                    ' ''DDLBancheIBAN.SelectedIndex = 0
                    '--------
                    If DDLBancheIBAN.SelectedIndex = 0 Then 'giu120411
                        lblABI.Text = "" : lblCAB.Text = ""
                        lblIBAN.Text = ""
                        lblContoCorrente.Text = ""
                        lblBanca.Text = "" : lblFiliale.Text = ""
                    End If
                    lblABICAB()
                    If TabCliFor = "Cli" Then
                        txtCodAgente.Text = dvCliFor.Item(0).Item("Agente_N").ToString
                        PosizionaItemDDLByTxt(txtCodAgente, DDLAgenti)
                        'giu311212
                        'giu171219 
                        ' ''txtTipoFatt.Text = dvCliFor.Item(0).Item("IDTipoFatt").ToString
                        ' ''PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
                    Else
                        txtCodAgente.Text = "" : DDLAgenti.SelectedIndex = 0
                    End If
                    Session(IDAGENTE) = txtCodAgente.Text.Trim
                    txtListino.AutoPostBack = False
                    txtListino.Text = dvCliFor.Item(0).Item("Listino").ToString
                    txtListino.AutoPostBack = True
                    Session(IDLISTINO) = txtListino.Text.Trim
                    If txtListino.Text.Trim = "" Or txtListino.Text.Trim = "0" Then
                        txtListino.Text = "1" 'Listino Base
                        Session(CSTVALUTADOC) = "Euro"
                        Session(CSTDECIMALIVALUTADOC) = 2
                    End If
                    Session(IDLISTINO) = txtListino.Text.Trim
                    DDLListini.AutoPostBack = False
                    PosizionaItemDDLByTxt(txtListino, DDLListini)
                    DDLListini.AutoPostBack = True
                    Session(IDLISTINO) = txtListino.Text.Trim
                    lblCodDesValutaByListino()
                    Session(CSTREGIMEIVA) = dvCliFor.Item(0).Item("Regime_IVA").ToString
                    LnkRegimeIVA.Text = "Regime IVA " & dvCliFor.Item(0).Item("Regime_IVA").ToString

                    'giu180219
                    If Left(txtCodCliForFilProvv.Text.Trim, 1) <> "1" Then
                        Session(CSTFATTURAPA) = False
                        chkFatturaPA.AutoPostBack = False
                        chkFatturaPA.Checked = False
                        chkFatturaPA.AutoPostBack = True
                        Session(CSTSPLITIVA) = False
                        WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
                    Else
                        Session(CSTFATTURAPA) = False
                        chkFatturaPA.AutoPostBack = False
                        chkFatturaPA.Checked = False
                        chkFatturaPA.AutoPostBack = True
                        Session(CSTSPLITIVA) = False
                        WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
                        If Not IsDBNull(dvCliFor.Item(0).Item("IPA")) Then
                            If dvCliFor.Item(0).Item("IPA").ToString.Trim <> "" And _
                                dvCliFor.Item(0).Item("IPA").ToString.Trim.Length = 6 Then 'GIU020119
                                Session(CSTFATTURAPA) = True
                                chkFatturaPA.AutoPostBack = False
                                chkFatturaPA.Checked = True
                                chkFatturaPA.AutoPostBack = True
                            End If
                        End If
                        '
                        If Not IsDBNull(dvCliFor.Item(0).Item("SplitIVA")) Then
                            If Val(dvCliFor.Item(0).Item("SplitIVA")) <> 0 Then
                                Session(CSTSPLITIVA) = True
                                WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(True, "")
                            End If
                        End If
                    End If
                    '---------
                End If
            Else
                lblCliForFilProvv.Text = ""
                lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
                If SWDoc = False Then
                    'GIU280120 
                    Session(CSTDURATANUM) = txtDurataNum.Text.Trim
                    Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
                    Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
                    Session(CSTIDPAG) = txtPagamento.Text.Trim
                    If SWBanca = True Then
                        lblABI.Text = "" : lblBanca.Text = ""
                        lblCAB.Text = "" : lblFiliale.Text = ""
                        lblIBAN.Text = "" : lblContoCorrente.Text = ""
                    End If
                    DDLAgenti.SelectedIndex = 0
                    DDLListini.SelectedIndex = 0
                    Session(IDAGENTE) = ""
                    Session(IDLISTINO) = "1"
                    txtListino.Text = "1"
                    Session(CSTVALUTADOC) = "Euro"
                    Session(CSTDECIMALIVALUTADOC) = 2
                    LnkRegimeIVA.Text = "Regime IVA"
                    Session(CSTREGIMEIVA) = "0"
                    Session(CSTFATTURAPA) = False
                    chkFatturaPA.AutoPostBack = False
                    chkFatturaPA.Checked = False
                    chkFatturaPA.AutoPostBack = True
                    Session(CSTSPLITIVA) = False
                    WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
                End If
                Exit Sub
            End If
            If lblPICF.Text.Trim = "" Then
                lblPICF.Text = dvCliFor.Item(0).Item("Codice_Fiscale").ToString
                If lblPICF.Text.Trim <> "" Then
                    lblLabelPICF.Text = "Codice Fiscale"
                    strInfo += "C.Fis.: " & lblPICF.Text & " "
                End If
            Else
                strInfo += "C.Fis.: " & dvCliFor.Item(0).Item("Codice_Fiscale").ToString & " "
            End If
            '-Indirizzo
            lblIndirizzo.Text = dvCliFor.Item(0).Item("Indirizzo").ToString & " " & dvCliFor.Item(0).Item("NumeroCivico").ToString
            strInfo += lblIndirizzo.Text & " "
            ' ''lblLocalita.Text = dvCliFor.Item(0).Item("Localita").ToString
            lblLocalita.Text = dvCliFor.Item(0).Item("CAP").ToString & " " & dvCliFor.Item(0).Item("Localita").ToString & " " & IIf(dvCliFor.Item(0).Item("Provincia").ToString.Trim <> "", "(" & dvCliFor.Item(0).Item("Provincia").ToString & ")", "")
            strInfo += lblLocalita.Text & " "
        Else
            lblCliForFilProvv.Text = ""
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            If SWDoc = False Then
                'GIU280120 
                Session(CSTDURATANUM) = txtDurataNum.Text.Trim
                Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
                Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
                Session(CSTIDPAG) = txtPagamento.Text.Trim
                If SWBanca = True Then
                    lblABI.Text = "" : lblBanca.Text = ""
                    lblCAB.Text = "" : lblFiliale.Text = ""
                    lblIBAN.Text = "" : lblContoCorrente.Text = ""
                End If
                DDLAgenti.SelectedIndex = 0
                DDLListini.SelectedIndex = 0
                Session(IDAGENTE) = ""
                Session(IDLISTINO) = "1"
                txtListino.Text = "1"
                Session(CSTVALUTADOC) = "Euro"
                Session(CSTDECIMALIVALUTADOC) = 2
                LnkRegimeIVA.Text = "Regime IVA"
                Session(CSTREGIMEIVA) = "0"
                Session(CSTFATTURAPA) = False
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                Session(CSTSPLITIVA) = False
                WUC_ContrattiSpeseTraspTot1.SetChkSplitIVA(False, "")
            End If
        End If
        txtCodCliForFilProvv.ToolTip = strInfo
        lblCliForFilProvv.ToolTip = strInfo
    End Sub
    Private Sub lblABICAB()
        If lblABI.Text.Trim = "" Then
            lblCAB.Text = "" : lblCAB.Text = ""
            lblBanca.Text = "" : lblFiliale.Text = ""
            Exit Sub
        End If
        Dim strErrore As String = ""
        lblBanca.Text = GetDatiBanche(lblABI.Text.Trim, strErrore).Banca
        If lblBanca.Text.Trim = "" Then
            lblCAB.Text = "" : lblCAB.Text = ""
            lblBanca.Text = "" : lblFiliale.Text = ""
            Exit Sub
        End If
        lblFiliale.Text = GetDatiFiliali(lblABI.Text.Trim, lblCAB.Text.Trim, strErrore).Filiale

    End Sub
    Private Sub DDLBancheIBAN_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLBancheIBAN.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        lblABI.Text = ""
        lblCAB.Text = ""
        lblBanca.Text = ""
        lblFiliale.Text = ""
        lblIBAN.Text = ""
        lblContoCorrente.Text = ""

        If DDLBancheIBAN.SelectedIndex = 0 Then
            Exit Sub
        End If
        lblIBAN.Text = DDLBancheIBAN.SelectedValue
        If lblIBAN.Text.Trim = "" Then
            Exit Sub
        End If

        Dim dvBI As DataView
        dvBI = SqlDSBancheIBAN.Select(DataSourceSelectArguments.Empty)
        If (dvBI Is Nothing) Then
            lblIBAN.Text = ""
            Exit Sub
        End If
        If dvBI.Count > 0 Then
            dvBI.RowFilter = "IBAN = '" & lblIBAN.Text.Trim & "'"
            If dvBI.Count > 0 Then
                lblABI.Text = dvBI.Item(0).Item("ABI").ToString
                lblCAB.Text = dvBI.Item(0).Item("CAB").ToString
                lblContoCorrente.Text = dvBI.Item(0).Item("ContoCorrente").ToString
            Else
                lblIBAN.Text = ""
                Exit Sub
            End If
        Else
            lblIBAN.Text = ""
            Exit Sub
        End If
        lblABICAB()

    End Sub

    Private Sub lblCodDesValutaByListino()
        If txtListino.Text.Trim = "" Then
            Session(IDLISTINO) = txtListino.Text.Trim
            lblCodValuta.Text = "" : lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
        Dim dvListini As DataView
        dvListini = SqlDSListini.Select(DataSourceSelectArguments.Empty)
        If (dvListini Is Nothing) Then
            lblCodValuta.Text = "" : lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
        If dvListini.Count > 0 Then
            dvListini.RowFilter = "Codice = " & txtListino.Text.Trim
            If dvListini.Count > 0 Then
                lblCodValuta.Text = dvListini.Item(0).Item("Valuta").ToString
                lblValuta()
            Else
                lblCodValuta.Text = "" : lblDesValuta.Text = ""
                Session(CSTVALUTADOC) = ""
                Session(CSTDECIMALIVALUTADOC) = 0
                Exit Sub
            End If
        Else
            lblCodValuta.Text = "" : lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
    End Sub
    Private Sub lblValuta()
        If lblCodValuta.Text.Trim = "" Then
            'lblCodValuta.BackColor = SEGNALA_OK
            lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
        Dim dvValute As DataView
        dvValute = SqlDSValuta.Select(DataSourceSelectArguments.Empty)
        If (dvValute Is Nothing) Then
            lblCodValuta.Text = ""
            lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
        If dvValute.Count > 0 Then
            dvValute.RowFilter = "Codice = '" & lblCodValuta.Text.Trim & "'"
            If dvValute.Count > 0 Then
                'lblCodValuta.BackColor = SEGNALA_OK
                lblDesValuta.Text = dvValute.Item(0).Item("Descrizione").ToString
                Session(CSTVALUTADOC) = lblCodValuta.Text.Trim
                Session(CSTDECIMALIVALUTADOC) = dvValute.Item(0).Item("Decimali")
            Else
                'lblCodValuta.BackColor = SEGNALA_KO
                lblCodValuta.Text = ""
                lblDesValuta.Text = ""
                Session(CSTVALUTADOC) = ""
                Session(CSTDECIMALIVALUTADOC) = 0
                Exit Sub
            End If
        Else
            'lblCodValuta.BackColor = SEGNALA_KO
            lblCodValuta.Text = ""
            lblDesValuta.Text = ""
            Session(CSTVALUTADOC) = ""
            Session(CSTDECIMALIVALUTADOC) = 0
            Exit Sub
        End If
    End Sub

    Private Sub AzzeraTxtDocT()
        SfondoCampiDocT()
        ' ''txtRevNDoc.AutoPostBack = False
        ' ''txtRevNDoc.Text = ""
        ' ''txtRevNDoc.AutoPostBack = True :
        txtNumero.AutoPostBack = False : txtDataDoc.AutoPostBack = False
        txtNumero.Text = "" : txtDataDoc.Text = ""
        txtNumero.AutoPostBack = True : txtDataDoc.AutoPostBack = True
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = ""
        'giu101219
        txtDurataNum.Text = "" : DDLDurataTipo.SelectedIndex = 0
        txtDataDoc.BackColor = SEGNALA_OK
        txtDataInizio.BackColor = SEGNALA_OK : txtDataFine.BackColor = SEGNALA_OK : txtDataAccettazione.BackColor = SEGNALA_OK
        txtDataInizio.Text = "" : txtDataFine.Text = "" : txtDataAccettazione.Text = ""
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
        txtNVisite.Text = ""
        txtDataValidita.Text = "" : txtDataConsegna.Text = ""
        txtNGG_Validita.Text = "" : txtNGG_Consegna.Text = ""
        txtCIG.Text = "" : txtCUP.Text = ""
        '-
        DDLTipoFatt.SelectedIndex = 0
        txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
        txtPagamento.Text = "" : DDLPagamento.SelectedIndex = 0
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
        chkSWTipoModSint.Checked = False
        '----------------
        'GIU280120 
        Session(CSTDURATANUM) = txtDurataNum.Text.Trim
        Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
        Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
        lblNumRate.Text = ""
        '---------
        txtRiferimento.Text = "" : txtDataRif.Text = ""
        optTipoEvTotaleTotale.Checked = True
        optTipoEvTotaleParziale.Checked = False
        '-
        ' ''optTipoEvSaldoSaldo.Checked = False
        ' ''optTipoEvSaldoParziale.Checked = False
        ' ''optTipoEvSaldoNO.Checked = True
        '------------------------------------------------------------
        txtCodCliForFilProvv.AutoPostBack = False
        txtCodCliForFilProvv.Text = ""
        Session(CSTCODCOGE) = "" : Session(CSTCODFILIALE) = "" 'giu220520
        txtCodCliForFilProvv.AutoPostBack = True
        'giu101011 commentato perchè lento con il combo trasformato in Label ed aggiunta la Ricerca
        'DDLCliForFilProvv.SelectedIndex = 0
        lblCliForFilProvv.Text = ""
        lblPICF.Text = ""
        lblLabelCliForFilProvv.Text = "Cliente"
        If TabCliFor = "For" Then lblLabelCliForFilProvv.Text = "Fornitore"
        lblIndirizzo.Text = "" : lblLocalita.Text = ""
        'DESTINAZIONE
        lblDestSel.Text = "" : lblDestSel.ToolTip = ""
        txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
        'GIU240320
        lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
        lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
        lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
        lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
        '---
        WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
        '-
        DDLBancheIBAN.SelectedIndex = 0
        lblABI.Text = "" : lblBanca.Text = "" : lblCAB.Text = "" : lblFiliale.Text = "" : lblIBAN.Text = "" : lblContoCorrente.Text = ""
        'Richiesta di Cinzia del 05012012 mettere la banca di DEFAULT
        lblABI.Text = GetParamGestAzi(Session(ESERCIZIO)).ABI
        lblCAB.Text = GetParamGestAzi(Session(ESERCIZIO)).CAB
        lblIBAN.Text = GetParamGestAzi(Session(ESERCIZIO)).NazIBAN + GetParamGestAzi(Session(ESERCIZIO)).CINEUIBAN + GetParamGestAzi(Session(ESERCIZIO)).CIN + GetParamGestAzi(Session(ESERCIZIO)).ABI + GetParamGestAzi(Session(ESERCIZIO)).CAB + GetParamGestAzi(Session(ESERCIZIO)).CC
        lblContoCorrente.Text = GetParamGestAzi(Session(ESERCIZIO)).CC
        PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
        '--------
        If DDLBancheIBAN.SelectedIndex = 0 Then 'giu120411
            lblABI.Text = "" : lblCAB.Text = ""
            lblIBAN.Text = ""
            lblContoCorrente.Text = ""
            lblBanca.Text = "" : lblFiliale.Text = ""
        End If
        lblABICAB()
        '-------------------------------------------------------------
        txtCodAgente.AutoPostBack = False
        txtCodAgente.Text = "" : DDLAgenti.SelectedIndex = 0
        Session(IDAGENTE) = txtCodAgente.Text.Trim
        txtCodAgente.AutoPostBack = True
        '-
        txtCodRespArea.AutoPostBack = False : DDLRespArea.AutoPostBack = False
        txtCodRespArea.Text = ""
        Session(IDRESPAREA) = ""
        SqlDSRespArea.DataBind()
        DDLRespArea.Items.Clear()
        DDLRespArea.Items.Add("")
        DDLRespArea.DataBind()
        DDLRespArea.SelectedIndex = -1
        DDLRespArea.AutoPostBack = True
        txtCodRespArea.AutoPostBack = True : DDLRespArea.AutoPostBack = True
        '-
        txtCodRespVisite.AutoPostBack = False : DDLRespVisite.AutoPostBack = False
        txtCodRespVisite.Text = ""
        Session(IDRESPVISITE) = ""
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        txtCodRespVisite.AutoPostBack = True : DDLRespVisite.AutoPostBack = True
        '-
        txtCodCausale.AutoPostBack = False
        txtCodCausale.Text = "" : DDLCausali.SelectedIndex = 0
        txtCodCausale.AutoPostBack = True
        checkADeposito.Checked = False 'giu260412
        txtListino.AutoPostBack = False : DDLListini.AutoPostBack = False
        txtListino.Text = "1"
        Call PosizionaItemDDL("1", DDLListini)
        Session(IDLISTINO) = "1" 'Listino Base
        txtListino.AutoPostBack = True : DDLListini.AutoPostBack = True
        Session(CSTVALUTADOC) = "Euro"
        Session(CSTDECIMALIVALUTADOC) = 2
        LnkRegimeIVA.Text = "Regime IVA"
        Session(CSTREGIMEIVA) = ""
        lblCodValuta.Text = "" : lblDesValuta.Text = ""
        'giu191219
        ' ''txtTipoFatt.AutoPostBack = False
        ' ''txtTipoFatt.Text = "" : DDLTipoFatt.SelectedIndex = -1
        ' ''txtTipoFatt.AutoPostBack = True
        txtDesRefInt.Text = ""
        txtNoteDocumento.Text = ""
        chkFatturaPA.AutoPostBack = False : chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False
        chkFatturaPA.Checked = False : chkFatturaAC.Checked = False : chkScGiacenza.Checked = False
        chkFatturaPA.AutoPostBack = True : chkFatturaAC.AutoPostBack = True : chkScGiacenza.AutoPostBack = False
        PosizionaItemDDL("", DDLTipoRapp)
        PosizionaItemDDL("2", ddlMagazzino) 'giu210721
        ChkAcconto.Checked = False
    End Sub
    Private Sub SfondoCampiDocT()
        Try
            If Session(CSTNONCOMPLETO) = SWSI Then
                Exit Sub
            ElseIf Session(CSTSTATODOC) = "5" Then
                Exit Sub
            End If
        Catch ex As Exception
            'OK 
        End Try
        
        txtNumero.BackColor = SEGNALA_OK : txtNumero.ToolTip = ""
        ' ''txtRevNDoc.BackColor = SEGNALA_OK : txtRevNDoc.ToolTip = ""
        txtDataDoc.BackColor = SEGNALA_OK : txtDataDoc.ToolTip = ""
        'giu101219
        txtDurataNum.BackColor = SEGNALA_OK : txtDurataNum.ToolTip = ""
        DDLDurataTipo.BackColor = SEGNALA_OK : DDLDurataTipo.ToolTip = ""
        txtNVisite.BackColor = SEGNALA_OK : txtNVisite.ToolTip = ""
        txtDataInizio.BackColor = SEGNALA_OK : txtDataFine.BackColor = SEGNALA_OK : txtDataAccettazione.BackColor = SEGNALA_OK
        txtDataInizio.ToolTip = "" : txtDataFine.ToolTip = "" : txtDataAccettazione.ToolTip = ""
        txtTipoFatt.BackColor = SEGNALA_OK : txtTipoFatt.ToolTip = ""
        DDLTipoFatt.BackColor = SEGNALA_OK
        DDLPagamento.BackColor = SEGNALA_OK
        '-
        txtDataValidita.BackColor = SEGNALA_OK : txtDataValidita.ToolTip = ""
        txtDataConsegna.BackColor = SEGNALA_OK : txtDataConsegna.ToolTip = ""
        txtNGG_Validita.BackColor = SEGNALA_OK : txtNGG_Consegna.BackColor = SEGNALA_OK
        txtCIG.BackColor = SEGNALA_OK : txtCIG.ToolTip = ""
        txtCUP.BackColor = SEGNALA_OK : txtCUP.ToolTip = ""
        '-
        txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = "" : txtDataRif.BackColor = SEGNALA_OK : txtDataRif.ToolTip = ""
        txtCodCliForFilProvv.BackColor = SEGNALA_OK : txtCodCliForFilProvv.ToolTip = ""

        txtDestinazione1.BackColor = SEGNALA_OK : txtDestinazione1.ToolTip = ""
        txtDestinazione2.BackColor = SEGNALA_OK : txtDestinazione2.ToolTip = ""
        txtDestinazione3.BackColor = SEGNALA_OK : txtDestinazione3.ToolTip = ""
        txtPagamento.BackColor = SEGNALA_OK : txtPagamento.ToolTip = "" : DDLPagamento.BackColor = SEGNALA_OK

        txtCodRespArea.BackColor = SEGNALA_OK : txtCodRespArea.ToolTip = "" : DDLRespArea.BackColor = SEGNALA_OK
        txtCodRespVisite.BackColor = SEGNALA_OK : txtCodRespVisite.ToolTip = "" : DDLRespVisite.BackColor = SEGNALA_OK
        txtCodAgente.BackColor = SEGNALA_OK : txtCodAgente.ToolTip = "" : DDLAgenti.BackColor = SEGNALA_OK
        txtCodCausale.BackColor = SEGNALA_OK : txtCodCausale.ToolTip = "" : DDLCausali.BackColor = SEGNALA_OK
        txtListino.BackColor = SEGNALA_OK : txtListino.ToolTip = "" : DDLListini.BackColor = SEGNALA_OK

        chkFatturaPA.BackColor = SEGNALA_OK : chkFatturaPA.ToolTip = "" 'GIU070814
        DDLTipoRapp.BackColor = SEGNALA_OK
        ddlMagazzino.BackColor = SEGNALA_OK

        txtDesRefInt.BackColor = SEGNALA_OK : txtDesRefInt.ToolTip = ""
        txtNoteDocumento.BackColor = SEGNALA_OK : txtNoteDocumento.ToolTip = ""
    End Sub
    Private Sub CampiSetEnabledToT(ByVal Valore As Boolean)
        btnRitorno.Enabled = Not Valore
        btnRitorno.Visible = Not Valore
        '---
        LnkRegimeIVA.Enabled = Valore
        LnkRegimeIVA.Visible = False 'Valore
        '-
        ' ''LnkAltriDatiDoc.Enabled = Valore
        ' ''LnkAltriDatiDoc.Visible = Valore

        ' ''btnModificaRegimeIVA.Enabled = Valore
        ' ''btnModificaRifDoc.Enabled = Valore
        ' ''txtRevNDoc.Enabled = Valore :
        txtNumero.Enabled = Valore : txtDataDoc.Enabled = Valore
        'giu101219
        txtDurataNum.Enabled = Valore : DDLDurataTipo.Enabled = Valore : txtNVisite.Enabled = Valore
        txtDataInizio.Enabled = Valore : txtDataFine.Enabled = Valore : txtDataAccettazione.Enabled = Valore
        imgBtnShowCalendarDI.Enabled = Valore : imgBtnShowCalendarDF.Enabled = Valore : imgBtnShowCalendarDA.Enabled = Valore
        txtTipoFatt.Enabled = Valore : DDLTipoFatt.Enabled = Valore : btnTipofatt.Enabled = Valore
        txtPagamento.Enabled = Valore : DDLPagamento.Enabled = Valore
        chkSWTipoModSint.Enabled = Valore

        txtDataValidita.Enabled = Valore : txtDataConsegna.Enabled = Valore
        txtNGG_Validita.Enabled = Valore : txtNGG_Consegna.Enabled = Valore
        txtCIG.Enabled = Valore : txtCUP.Enabled = Valore
        txtRiferimento.Enabled = Valore : txtDataRif.Enabled = Valore : imgBtnShowCalendarDR.Enabled = Valore
        optTipoEvTotaleTotale.Enabled = Valore
        optTipoEvTotaleParziale.Enabled = Valore
        '-
        ' ''optTipoEvSaldoSaldo.Enabled = Valore
        ' ''optTipoEvSaldoParziale.Enabled = Valore
        ' ''optTipoEvSaldoNO.Enabled = Valore
        '------------------------------------------------------------
        imgBtnShowCalendar.Enabled = Valore : imgBtnShowCalendarDC.Enabled = Valore
        imgBtnShowCalendarDV.Enabled = Valore
        txtCodCliForFilProvv.Enabled = Valore

        btnInsAnagrProvv.Enabled = Valore
        btnModificaAnagrafica.Enabled = Valore
        btnCercaAnagrafica.Enabled = Valore

        btnInsDest.Enabled = Valore
        btnModificaDest.Enabled = Valore
        btnCercaDest.Enabled = Valore
        btnDelDest.Enabled = Valore
        txtDestinazione1.Enabled = Valore : txtDestinazione2.Enabled = Valore : txtDestinazione3.Enabled = Valore

        btnCercaBanca.Enabled = Valore
        DDLBancheIBAN.Enabled = Valore

        txtCodRespArea.Enabled = Valore : DDLRespArea.Enabled = Valore : btnRespArea.Enabled = Valore
        txtCodRespVisite.Enabled = Valore : DDLRespVisite.Enabled = Valore : btnRespVisite.Enabled = Valore
        txtCodAgente.Enabled = Valore : DDLAgenti.Enabled = Valore : btnAgenti.Enabled = Valore
        txtCodCausale.Enabled = Valore : DDLCausali.Enabled = Valore
        checkADeposito.Enabled = Valore 'giu260412
        txtListino.Enabled = Valore : DDLListini.Enabled = Valore

        chkFatturaPA.Enabled = Valore 'GIU070814
        DDLTipoRapp.Enabled = Valore
        ddlMagazzino.Enabled = Valore
        ChkAcconto.Enabled = Valore

        chkFatturaAC.Enabled = Valore
        chkScGiacenza.Enabled = Valore
        '-
        txtDesRefInt.Enabled = Valore
        txtNoteDocumento.Enabled = Valore
        WUC_ContrattiDett1.SetEnableTxt(Valore)
        WUC_ContrattiSpeseTraspTot1.SetEnableTxt(Valore)
    End Sub
#End Region

    Public Sub LnkRitorno_Click()
        If Session(SWOP) = SWOPELIMINA Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            Chiudi("")
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica dettaglio documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCR) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono stati modificati dati della riga selezionata  <br> se continuate senza aggiornare la riga le modifiche andranno perdute.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        Chiudi("")
    End Sub

    Public Sub LnkAltriDatiDoc_Click()
        ApriElenco(F_ALIQUOTAIVA)
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        If Session(SWOP) = SWOPELIMINA Then
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            Session(SWOP) = SWOPNESSUNA
            Chiudi("")
            Exit Sub
        End If
        'GIU301111
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica dettaglio documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'LOTTI
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnAnnulla)")
            Exit Sub
        End If
        'giu261023
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        btnCollegaOC.BackColor = btnNuovo.BackColor
        btnCollegaOC.ForeColor = Drawing.Color.Black
        '---------
        If Session(SWMODIFICATO) = SWSI Then
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = "AnnullaModificheDocumento"
            ModalPopup.Show("Annulla modifiche documento", "Confermi l'annullamento modifiche del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            AnnullaModificheDocumento()
        End If
        LnkStampa.Visible = False : LnkVerbale.Visible = False
    End Sub
    Public Sub AnnullaModificheDocumento()
        btnGeneraAttDNum.BackColor = btnNuovo.BackColor
        btnAggiorna.BackColor = btnNuovo.BackColor
        btnModifica.BackColor = btnNuovo.BackColor
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        Dim myID As String = Session(IDDOCUMENTI)
        If Session(SWOP) = SWOPNUOVO Then
            myID = Session(IDDOCUMSAVE)
        End If

        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Then
            Chiudi("")
            Exit Sub
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '-----------
        If Session(CSTNONCOMPLETO) = SWSI Then
            'GIU300722
            If IsNothing(Session(CSTSTATODOCSEL)) Then
                WUC_ContrattiDett1.SetStatoDoc5()
            ElseIf String.IsNullOrEmpty(Session(CSTSTATODOCSEL)) Then
                WUC_ContrattiDett1.SetStatoDoc5()
            ElseIf Session(CSTSTATODOCSEL) < 3 Then
                'lascio lo stato come prima come prima 
                lblMessDoc.Visible = False
            Else
                WUC_ContrattiDett1.SetStatoDoc5()
            End If
        End If
        Session(IDDOCUMENTI) = myID
        Session(IDDURATANUM) = myIDDurataNum
        Session(IDDURATANUMRIGA) = myIDDurataNumR
        SqlDbSelectCmd = Session("SqlDbSelectCmd")
        SqlDbInserCmd = Session("SqlDbInserCmd")
        SqlDbDeleteCmd = Session("SqlDbDeleteCmd")
        SqlDbUpdateCmd = Session("SqlDbUpdateCmd")
        SqlAdapDoc = Session("SqlAdapDoc")
        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        DsDocT.ContrattiT.Clear()
        SqlAdapDoc.Fill(DsDocT.ContrattiT)
        'popolo
        Session("DsDocT") = DsDocT
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsDocT.ContrattiT)
        End If
        Session("dvDocT") = dvDocT

        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            btnCollegaOC.Enabled = True
            PopolaTxtDocT()
            Dim strErrore As String = ""
            WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, strErrore)
            WUC_ContrattiSpeseTraspTot1.SfondoCampiDocTTD2()
            WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, strErrore)
            'giu221111 ricarico anche i DETTAGLI ORIGINALI con TRUE
            WUC_ContrattiDett1.TD1ReBuildDett(True)
            If WUC_ContrattiDett1.CKGridDett = False Then
                btnDuplicaDNum.Enabled = False
                btnNuovaDNum.Enabled = False
                ' ''btnGeneraAttDNum.Enabled = False
            End If
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
            WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
            WUC_ContrattiSpeseTraspTot1.SfondoCampiDocTTD2()
            WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2()
            'giu221111 ricarico anche i DETTAGLI ORIGINALI con TRUE
            WUC_ContrattiDett1.TD1ReBuildDett(True)
        End If

    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
                'giu301119 Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
        'giu080312 giu150312
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
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Try
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            End Try
            Exit Sub
        End If
        '-----------
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            '..\WebFormTables\
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            End Try
            Exit Sub
        End If
        Try
            Dim strRitorno As String = "WF_ContrattiElenco.aspx?labelForm=Elenco CONTRATTI"
            If Not String.IsNullOrEmpty(Session(CSTChiamatoDa)) Then
                strRitorno = Session(CSTChiamatoDa)
            End If
            If strRitorno.Trim = "" Then
                strRitorno = "WF_ContrattiElenco.aspx?labelForm=Elenco CONTRATTI"
            End If
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnAggiorna.Click
        If Session(SWOP) = SWOPNESSUNA Then
            ' ''If btnAggiorna.Enabled = True Then
            ' ''    Chiudi("ATTENZIONE, nessuna operazione in corso ma tasti attivati (load)")
            ' ''    Exit Sub
            ' ''End If
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "I dati visualizzati potrebbero non essere quelli aggiornati. <br> Ricarico dati dettaglio documento", WUC_ModalPopup.TYPE_ALERT)
            AnnullaModificheDocumento()
            Exit Sub
        End If
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica dettaglio documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCR) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono stati modificati dati della riga selezionata  <br> se continuate senza aggiornare la riga le modifiche andranno perdute.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'LOTTI
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        LnkStampa.Visible = False : LnkVerbale.Visible = False
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        BtnAggionaDocumento()
        Session(CALLGESTIONE) = SWNO
    End Sub

    'GIU040318 SPOSTATO QUI LE DEF PER I CONTROLLI
    'DEF VALORI DI RITORNO
    'Blocco Sconti e spese sul totale imponibile del documento
    Dim TotMerce As Decimal = 0
    Dim ScCassa As Decimal = 0
    Dim TotMerceLordo As Decimal = 0
    Dim SpIncasso As Decimal = 0
    Dim SpTrasp As Decimal = 0
    Dim Abbuono As Decimal = 0
    Dim SpImballo As Decimal = 0
    Dim DesImballo As String = ""
    'Blocco IVA
    Dim Iva(4) As Integer
    Dim Imponibile(4) As Decimal
    Dim Imposta(4) As Decimal
    'Blocco Scadenze
    Dim DataScad(5) As String
    Dim ImpRata(5) As Decimal
    'Blocco Trasporto
    Dim TipoSped As String = ""
    Dim Vettori(3) As Long
    Dim Porto As String = ""
    Dim DesPorto As String = ""
    Dim Aspetto As String = ""
    Dim Colli As Integer = 0
    Dim Pezzi As Integer = 0
    Dim Peso As Decimal = 0
    'GIU200420 Dim CKSWTipoEvSaldo As Boolean
    'USATI PER IL CALCOLO RATE
    Dim SWNoDivTotRate As Boolean = False
    Dim SWAccorpaRateAA As Boolean = False
    'giu040118 giu120118
    Dim SplitIVA As Boolean = False
    Dim RitAcconto As Boolean = False
    Dim ImponibileRA As Decimal = 0
    Dim PercRA As Decimal = 0
    Dim TotaleRA As Decimal = 0
    Dim TotNettoPagare As Decimal = 0
    'GIU260219
    Dim Bollo As Decimal = 0 : Dim BolloACaricoDel As String = ""

    Private Sub BtnAggionaDocumento()

        If CheckNewNumeroOnTab() = True Then
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            txtNumero.BackColor = SEGNALA_KO
            txtNumero.Focus()
            Exit Sub
        Else
            txtNumero.BackColor = SEGNALA_OK
        End If

        BtnSetEnabledTo(False) 'disattivo i tasti per evitare
        'giu040319
        SWTB0 = True 'TESTATA
        SWTB1 = True 'DATI APPARECCHIATURE
        SWTB2 = True 'DETTAGLI
        SWTB3 = True 'TOTALI E SCADENZE
        '-
        Session(CSTSTATODOC) = "0"
        Session(CSTNONCOMPLETO) = SWNO
        lblMessDoc.Text = "" : lblMessDoc.Visible = False
        SWTB0 = ControllaDocT() 'qui aggiorna anche SWTB3
        Dim SWRicalcola As Boolean = False
        Dim myStrEsito As String = "" 'giu180518
        SWTB2 = ControllaDocD(myStrEsito)
        SWTB3 = ControllaDocS()
        '-
        If SWTB0 = False Or SWTB1 = False Or SWTB2 = False Or SWTB3 = False Then
            If SWTB0 = False Then
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
            ElseIf SWTB1 = False Then
                Session("TabDoc") = TB1
                TabContainer1.ActiveTabIndex = TB1
            ElseIf SWTB2 = False Then
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
            ElseIf SWTB3 = False Then
                Session("TabDoc") = TB3
                TabContainer1.ActiveTabIndex = TB3
            End If
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(CSTSTATODOC) = "5"
            If myStrEsito.Trim = "" Then 'giu180518
                ModalPopup.Show("Controllo dati documento", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Else
                ModalPopup.Show("Controllo dati documento", "Attenzione, i campi segnalati in rosso non sono validi <br> " & myStrEsito.Trim, WUC_ModalPopup.TYPE_ALERT)
            End If
            Exit Sub
        End If
        'giu090412
        If lblCliForFilProvv.ForeColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc" 'giu120320 "ControllaBollo"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
            ModalPopup.Show("Aggiorna documento", "ATTENZIONE, il cliente/fornitore risulta bloccato. <br> " _
                        & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        OkAggiornaDoc() 'giu120320 ControllaBollo()
    End Sub
    'giu040319 giu130320 dopo l'aggiornamento del documento solo messagi di controllo 
    Public Sub ControllaBolloScadAtt(ByRef CkMess As String)
        Dim mySWTB3 As Boolean = True
        CkMess = ""
        'giu260320 rimango dove sono
        ' ''Session("TabDoc") = TB3
        ' ''TabContainer1.ActiveTabIndex = TB3
        'GIU040319 
        Dim ImpBollo As Decimal = 0 : Dim ImpMinBollo As Decimal = 0
        'giu290722 ImpBollo = GetParamGestAzi(Session(ESERCIZIO)).Bollo
        'giu290722 ImpMinBollo = GetParamGestAzi(Session(ESERCIZIO)).ImpMinBollo

        Dim mySplitIVA As Boolean = False : Dim Nazione As String = "" : Dim strErrore As String = ""
        Call Documenti.CKClientiIPAByIDConORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
        If strErrore.Trim <> "" Then
            If CkMess.Trim <> "" Then CkMess += "<br>"
            CkMess += "Errore: Controllo applicazione bollo (Controllo nazione - lettura dati cliente): " & strErrore
        End If
        '---------
        mySWTB3 = WUC_ContrattiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        SWNoDivTotRate, SWAccorpaRateAA, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
        'giu290722
        ' ''If mySWTB3 = False Then
        ' ''    If CkMess.Trim <> "" Then CkMess += "<br>"
        ' ''    CkMess += "Errore: Controllo applicazione bollo (Lettura dati Totale Documento,Spese,Trasp.,Bollo)"
        ' ''End If
        ' ''If ImpBollo <> 0 And ImpMinBollo <> 0 Then
        ' ''    'OK SE PREVISTO OK BOLLO
        ' ''Else
        ' ''    'NON APPLICAZIONE PER I STRANIERI
        ' ''    mySWTB3 = False
        ' ''    WUC_ContrattiSpeseTraspTot1.SetErrBollo(False)
        ' ''    If CkMess.Trim <> "" Then CkMess += "<br>"
        ' ''    CkMess += "ATTENZIONE, Parametri generali BOLLO non definiti."
        ' ''End If
        ' ''If Bollo <> 0 And (Nazione.Trim <> "I" And Nazione.Trim <> "IT" And Nazione.Trim <> "ITA") Then
        ' ''    'NON APPLICAZIONE PER I STRANIERI
        ' ''    mySWTB3 = False
        ' ''    WUC_ContrattiSpeseTraspTot1.SetErrBollo(False)
        ' ''    If CkMess.Trim <> "" Then CkMess += "<br>"
        ' ''    CkMess += "ATTENZIONE, BOLLO NON RICHIESTO! (Cliente straniero)."
        ' ''End If
        '---------
        'ORA VERIFICO SE E' RICHIESTO IL BOLLO
        Dim myTotale As Decimal = 0
        Try
            'GIU250220 CALCOLO IL TOTALE SEMPRE SUI PERIODI ATTIVITA'
            ' ''da sistema qui non ha ancora aggiornato i dettagli
            ' ''Dim DsDocDettTmp As New DSDocumenti
            ' ''DsDocDettTmp = Session("aDsDett")
            SetCdmDAdp()
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                mySWTB3 = False
                If CkMess.Trim <> "" Then CkMess += "<br>"
                CkMess += "Errore:IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (ControllaBolloScadAtt)"
                Exit Sub
            End If
            Dim DsDocDettTmp As New DSDocumenti
            DsDocDettTmp.ContrattiD.Clear()
            SqlDbSelectCmdALLAtt.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNum").Value = DBNull.Value 'GIU130320 PRENDO ANCHE LE APP. POI FILTRO - 1 'fisso per le attività per periodo
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            SqlAdapDocALLAtt.Fill(DsDocDettTmp.ContrattiD)
            '--------------------------------------------------------
            'se cambio il regime IVA del cliente non viene aggiornato subito
            Dim RegimeIVA As String = Session(CSTREGIMEIVA)
            'giu260219 RegimeIVA = "0"
            Dim myRegIVA As String = Trim(Mid(LnkRegimeIVA.Text, 11))
            If IsNothing(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If String.IsNullOrEmpty(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
            '---------------------------------------------------------------
            If CLng(RegimeIVA) > 49 Then
                For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND Importo>0")
                    If RowDettB.RowState <> DataRowState.Deleted Then
                        myTotale += RowDettB.Item("Importo")
                    End If
                Next
            Else
                For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND Importo>0")
                    If RowDettB.RowState <> DataRowState.Deleted Then
                        If (RowDettB.Item("OmaggioImponibile") = True And RowDettB.Item("OmaggioImposta") = True) Or RowDettB.Item("Cod_Iva") > 49 Then
                            myTotale += RowDettB.Item("Importo")
                        End If
                    End If
                Next
            End If
            'giu120320 controllo ATTIVITA'/CHECK PER PERIODO
            Dim myCKMess = "" 'MESSGGI CONTROLLO PERIODI
            'giu120320 controllo ATTIVITA'/CHECK PER PERIODO
            Dim SaveSerie As String = ""
            Dim myNApp As Integer = 0 '210420 ERR. MANCAVA RIGA 1 DsDocDettTmp.ContrattiD.Select("DurataNum=0 AND Riga=1").Length
            For Each RowDettS In DsDocDettTmp.ContrattiD.Select("DurataNum=0", "Serie")
                If IsDBNull(RowDettS.Item("Serie")) Then
                    SaveSerie = ""
                    myNApp += 1
                ElseIf SaveSerie = RowDettS.Item("Serie").ToString.Trim Then
                    Continue For
                Else
                    If IsDBNull(RowDettS.Item("SWSostituito")) Then
                        myNApp += 1
                        SaveSerie = RowDettS.Item("Serie").ToString.Trim
                    ElseIf RowDettS.Item("SWSostituito") = False Then
                        myNApp += 1
                        SaveSerie = RowDettS.Item("Serie").ToString.Trim
                    End If
                End If
            Next
            Dim SWChek As Boolean = True : Dim myDurataNum As String = "" : Dim pCodArt As String = "" : Dim pNVisite As Integer = 0
            myDurataNum = txtDurataNum.Text.Trim
            SWChek = True
            If myNApp = 0 Then
                SWChek = False
                If CkMess.Trim <> "" Then CkMess += "<br>"
                CkMess += "Nessuna apparecchiatura inserita."
            End If
            If IsNothing(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            ElseIf Not IsNumeric(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            ElseIf Val(myDurataNum) = 0 Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            End If
            '-
            Dim myDurataTipo As String = DDLDurataTipo.SelectedValue.Trim
            If IsNothing(myDurataTipo) Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDurataTipo) Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            ElseIf myDurataTipo.Trim = "" Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            '-
            Dim myDataInizio As String = txtDataInizio.Text.Trim
            If IsNothing(myDataInizio) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDataInizio) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            ElseIf Not IsDate(myDataInizio.Trim) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            End If
            '----------
            Dim arrPeriodo() As String = Nothing
            If SetArrPeriodo(arrPeriodo) = False Then
                SWChek = False
            End If
            If WUC_ContrattiDett1.GetCodVisitaDATipoCAIdPag(pCodArt, pNVisite) = False Then
                'ok proseguo lo stesso senza fare il controllo 
                DDLPagamento.BackColor = SEGNALA_KO
            ElseIf SWChek = False Then
                'PROSEGUO PER IL CONTROLLO BOLLO
            Else
                If Not IsNumeric(txtNVisite.Text.Trim) Then
                    txtNVisite.BackColor = SEGNALA_KO
                    pNVisite = 0
                ElseIf Int(txtNVisite.Text.Trim) <> pNVisite Then
                    'giu290722 txtNVisite.BackColor = SEGNALA_KO
                    'diverso dal n° visite previste nel periodo
                    pNVisite = Int(txtNVisite.Text.Trim)
                Else
                    pNVisite = Int(txtNVisite.Text.Trim)
                End If
                Dim NVisitePeriodo As Integer = 0 : Dim SaveDataScVis As String = ""
                Dim NScadPeriodo As Integer = 0 : Dim SaveDataSc As String = ""
                Dim CheckPeriodo As String = ""
                Dim SaveCodDest As String = "" : Dim SaveCodDestV As String = ""
                'ok
                For i = 0 To Val(myDurataNum) - 1
                    CheckPeriodo = arrPeriodo(i).Trim
                    NScadPeriodo = 0 : SaveDataSc = "" : SaveCodDest = ""
                    NVisitePeriodo = 0 : SaveDataScVis = ""
                    myCKMess = ""
                    For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim, "Serie,DataSc")
                        If RowDettB.RowState <> DataRowState.Deleted Then
                            If IsDBNull(RowDettB.Item("DataSc")) Then
                                If myCKMess.Trim <> "" Then myCKMess += "<br>"
                                myCKMess += CheckPeriodo + " - Manca Data di scadenza"
                            Else
                                If IsDBNull(RowDettB.Item("Cod_Articolo")) Then
                                    If SaveDataSc = "" Then
                                        NScadPeriodo += 1
                                        SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDest = ""
                                        Else
                                            SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                        NScadPeriodo += 1
                                        SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDest = ""
                                        Else
                                            SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    End If
                                ElseIf RowDettB.Item("Cod_Articolo").ToString.Trim = pCodArt.Trim And pCodArt.Trim <> "" Then
                                    NVisitePeriodo += 1
                                    If SaveDataScVis = "" Then
                                        SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDestV = ""
                                        Else
                                            SaveCodDestV = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    ElseIf CDate(SaveDataScVis).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                        SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDestV = ""
                                        Else
                                            SaveCodDestV = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    End If
                                Else
                                    If SaveDataSc = "" Then
                                        NScadPeriodo += 1
                                        SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDest = ""
                                        Else
                                            SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                        NScadPeriodo += 1
                                        SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                            SaveCodDest = ""
                                        Else
                                            SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                                        End If
                                    End If
                                End If

                            End If
                            '-
                            If SaveDataScVis <> "" And SaveDataSc <> "" Then
                                If CDate(SaveDataSc).Date < CDate(SaveDataScVis).Date And SaveCodDest = SaveCodDestV Then
                                    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                                    myCKMess += CheckPeriodo + " - Scadenze precedenti alla visita prevista"
                                End If
                            End If
                        End If
                    Next
                    If (NVisitePeriodo / myNApp) <> pNVisite And SaveCodDest = SaveCodDestV Then
                        If myCKMess.Trim <> "" Then myCKMess += "<br>"
                        myCKMess += CheckPeriodo + " - N° Visite diverso da quello indicato Oppure Codice Visita mancante nelle Apparecchiature."
                    End If
                    If WUC_ContrattiDett1.GetDDLTipoDettagliID <> 0 Then
                        If myCKMess.Trim <> "" Then
                            If CkMess.Trim <> "" Then CkMess += "<br>"
                            CkMess += myCKMess
                        End If
                    End If

                Next
            End If
            '-----------------------------------------------
        Catch ex As Exception
            mySWTB3 = False
            If CkMess.Trim <> "" Then CkMess += "<br>"
            CkMess += "Errore:Controllo Scadenze attività. (ControllaBolloScadAtt): " & ex.Message
            Exit Sub
        End Try
        '-
    End Sub
    Public Function SetArrPeriodo(ByRef arrPeriodo() As String) As Boolean
        SetArrPeriodo = False
        Dim myDurataNum As String = txtDurataNum.Text.Trim 'Session(CSTDURATANUM)
        If IsNothing(myDurataNum) Then
            lblMessDoc.Text = "Manca la Durata N°" : lblMessDoc.Visible = True
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataNum) Then
            lblMessDoc.Text = "Manca la Durata N°" : lblMessDoc.Visible = True
            Exit Function
        ElseIf Not IsNumeric(myDurataNum) Then
            lblMessDoc.Text = "Manca la Durata N°" : lblMessDoc.Visible = True
            Exit Function
        ElseIf Val(myDurataNum) = 0 Then
            lblMessDoc.Text = "Manca la Durata N°" : lblMessDoc.Visible = True
            Exit Function
        End If
        '-
        Dim myDurataTipo As String = DDLDurataTipo.SelectedValue.Trim 'Session(CSTDURATATIPO)
        If IsNothing(myDurataTipo) Then
            lblMessDoc.Text = "Manca il tipo Durata N°" : lblMessDoc.Visible = True
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataTipo) Then
            lblMessDoc.Text = "Manca il tipo Durata N°" : lblMessDoc.Visible = True
            Exit Function
        ElseIf myDurataTipo.Trim = "" Then
            lblMessDoc.Text = "Manca il tipo Durata N°" : lblMessDoc.Visible = True
            Exit Function
        End If
        '-
        Dim myDataInizio As String = txtDataInizio.Text.Trim 'Session(CSTDATAINIZIO)
        If IsNothing(myDataInizio) Then
            lblMessDoc.Text = "Manca Data inizio" : lblMessDoc.Visible = True
            Exit Function
        End If
        If String.IsNullOrEmpty(myDataInizio) Then
            lblMessDoc.Text = "Manca Data inizio" : lblMessDoc.Visible = True
            Exit Function
        ElseIf Not IsDate(myDataInizio.Trim) Then
            lblMessDoc.Text = "Manca Data inizio" : lblMessDoc.Visible = True
            Exit Function
        End If
        '----------
        ReDim arrPeriodo(Val(myDurataNum))
        If myDurataTipo.Trim = "M" Then
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = "Mese(" + (i + 1).ToString.Trim + ") " + (Format(CDate(myDataInizio), FormatoData))
                myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "T" Then
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = "Trimestre(" + (i + 1).ToString.Trim + ") " + (Format(CDate(myDataInizio), FormatoData))
                myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "Q" Then
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = "Quadrimestre(" + (i + 1).ToString.Trim + ") " + (Format(CDate(myDataInizio), FormatoData))
                myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "S" Then
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = "Semestre(" + (i + 1).ToString.Trim + ") " + (Format(CDate(myDataInizio), FormatoData))
                myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "A" Then
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = "Anno(" + (i + 1).ToString.Trim + ") " + (Format(CDate(myDataInizio).Year, "0000"))
                myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
            Next i
        Else 'non capitera' mai
            For i = 0 To Val(myDurataNum) - 1
                arrPeriodo(i) = myDurataTipo.Trim + "(" + (i + 1).ToString.Trim + ") - ????"
            Next i
            lblMessDoc.Text = "Errore nel tipo Durata N°" : lblMessDoc.Visible = True
        End If
        SetArrPeriodo = True
    End Function
    'giu090412
    Public Sub OkAggiornaDoc()
        SWTB0 = True 'TESTATA
        SWTB1 = True 'MACCHINE
        SWTB2 = True
        SWTB3 = True
        Session(CSTSTATODOC) = "0"
        Session(CSTNONCOMPLETO) = SWNO
        'ok aggiorno
        Dim DsDettBack As New DSDocumenti
        SWTB3 = AggiornaDocD(DsDettBack) 'giu221111 si aggiorno 
        Dim strErrore As String = ""
        SWTB0 = AggiornaDocT(DsDettBack, False, strErrore) 'giu160118 aggiorna sia SWTB0 che SWTB2 = True

        If SWTB0 = False Or SWTB1 = False Or SWTB2 = False Or SWTB3 = False Then
            'commentato perchè faccio lo show dell'errore nella funzione di AGGIORNA
            'altrimenti segnala l'ultimo che è questo e non quello nella funzione
            If SWTB0 = False Then
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
                If Session("ErrSQL") = SWSI Then
                    Session("TabDoc") = TB2
                    TabContainer1.ActiveTabIndex = TB2
                End If
            ElseIf SWTB1 = False Then
                Session("TabDoc") = TB1
                TabContainer1.ActiveTabIndex = TB1
            ElseIf SWTB2 = False Then
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
            ElseIf SWTB3 = False Then
                Session("TabDoc") = TB3
                TabContainer1.ActiveTabIndex = TB3
            End If
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(CSTNONCOMPLETO) = SWSI
            Session(CSTSTATODOC) = "5"
            Exit Sub
        End If

        SqlDbSelectCmd = Session("SqlDbSelectCmd")
        SqlDbInserCmd = Session("SqlDbInserCmd")
        SqlDbDeleteCmd = Session("SqlDbDeleteCmd")
        SqlDbUpdateCmd = Session("SqlDbUpdateCmd")
        SqlAdapDoc = Session("SqlAdapDoc")
        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '-----------
        If myID = "" Or Not IsNumeric(myID) Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlAdapDoc.Fill(DsDocT.ContrattiT)
        Session("DsDocT") = DsDocT
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsDocT.ContrattiT)
        End If
        Session("dvDocT") = dvDocT
        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            btnCollegaOC.Enabled = True
            PopolaTxtDocT()
            WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, strErrore)
            WUC_ContrattiDett1.TD1ReBuildDett()
        Else
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
            WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2()
            Session(IDDOCUMENTI) = ""
            WUC_ContrattiDett1.TD1ReBuildDett()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiorna dati documento", "Attenzione, nessun documento aggiornato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        'giu120320 
        Dim CKMess As String = "" : strErrore = ""
        Call ControllaBolloScadAtt(CKMess)
        If txtNumero.BackColor = SEGNALA_KO Then 'GIU260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            If strErrore.Trim <> "" Then strErrore += "<br>"
            strErrore += "!!!ERRORE!!! N.Documento !!!VERIFICARE !!!"
        ElseIf DDLPagamento.BackColor = SEGNALA_KO Then
            If strErrore.Trim <> "" Then strErrore += "<br>"
            strErrore += "Manca, il Tipo Contratto, <br>Non è stato possibile eseguire i controlli della Scadenze Attività."
        ElseIf txtNVisite.BackColor = SEGNALA_KO Then
            If strErrore.Trim <> "" Then strErrore += "<br>"
            strErrore += "N° Visite nel singolo periodo, <br>Non definito oppure diverso dal n° visite previste nel periodo."
        End If
        If btnAggiorna.BackColor = SEGNALA_KO Then
            If btnAggiorna.Text = "Aggiorna" Then
                btnAggiorna.Text = "Agg.Scadenze"
                btnModifica.BackColor = SEGNALA_KO
            Else
                If WUC_ContrattiSpeseTraspTot1.RestoreScadenze() = False Then
                    Session("TabDoc") = TB3
                    TabContainer1.ActiveTabIndex = TB3
                Else
                    Session("TabDoc") = TB3
                    TabContainer1.ActiveTabIndex = TB3
                End If
                btnAggiorna.Text = "Aggiorna"
                btnAggiorna.BackColor = btnNuovo.BackColor
                btnModifica.BackColor = btnNuovo.BackColor
            End If
        End If
        '-
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strErrore.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf CKMess.Trim <> "" Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            '-
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", IIf(CKMess.Trim.Length > 300, Mid(CKMess.Trim, 1, 300), CKMess.Trim), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Else
            'GIU071223
            'ok controllo Date scadenze attivita
            Dim strSegnalaDateDiverse As String = ""
            WUC_ContrattiSpeseTraspTot1.VerificaDateCKCons(strSegnalaDateDiverse)
            If strSegnalaDateDiverse.Trim <> "" Then
                Session("TabDoc") = TB3
                TabContainer1.ActiveTabIndex = TB3
            End If
        End If
        '---------
    End Sub
    Public Sub CKScAttBollo()
        'giu120320
        Dim CKMess As String = ""
        Call ControllaBolloScadAtt(CKMess)
        If txtNumero.BackColor = SEGNALA_KO Then 'GIU260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            If CKMess.Trim <> "" Then CKMess += "<br>"
            CKMess += "!!!ERRORE!!! N.Documento !!!VERIFICARE !!!"
        ElseIf DDLPagamento.BackColor = SEGNALA_KO Then
            If CKMess.Trim <> "" Then CKMess += "<br>"
            CKMess += "Manca, il Tipo Contratto, <br>Non è stato possibile eseguire i controlli della Scadenze Attività."
        ElseIf txtNVisite.BackColor = SEGNALA_KO Then
            If CKMess.Trim <> "" Then CKMess += "<br>"
            CKMess += "N° Visite nel singolo periodo, <br>Non definito oppure diverso dal n° visite previste nel periodo."
        End If
        If btnAggiorna.BackColor = SEGNALA_KO Then
            If btnAggiorna.Text = "Aggiorna" Then
                btnAggiorna.Text = "Agg.Scadenze"
                btnModifica.BackColor = SEGNALA_KO
            Else
                btnAggiorna.Text = "Aggiorna"
                btnAggiorna.BackColor = btnNuovo.BackColor
                btnModifica.BackColor = btnNuovo.BackColor
            End If
        End If
        If CKMess.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", IIf(CKMess.Trim.Length > 300, Mid(CKMess.Trim, 1, 300), CKMess.Trim), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
    End Sub
    'GIU120412
    Public Sub NoAggiornaDoc()
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
    End Sub

    Private Function ControllaDocT() As Boolean
        Dim strErrore As String = ""
        ControllaDocT = True
        SfondoCampiDocT()
        If txtNumero.Text.Trim <> "" Then
            If Not IsNumeric(txtNumero.Text.Trim) Then
                txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf CLng(txtNumero.Text.Trim) < 1 Then 'giu191211
                txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf CheckNewNumeroOnTab() = True Then
                txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        Else
            txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        'GIU070814
        'GIU040319 CONTROLLO SPLITIVA / BOLLO APPLICAZIONE
        SWTB3 = WUC_ContrattiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        SWNoDivTotRate, SWAccorpaRateAA, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)

        Dim mySplitIVA As Boolean = False : Dim Nazione As String = ""
        If txtCodCliForFilProvv.Text.Trim <> "" And chkFatturaPA.Visible = True Then 'GIU030219
            SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
            If strErrore.Trim <> "" Then
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False : SWFatturaPA = False
                chkFatturaPA.AutoPostBack = True
                chkFatturaPA.BackColor = SEGNALA_KO : ControllaDocT = False
                chkFatturaPA.ToolTip = strErrore
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
                ' ''Exit Function
            ElseIf chkFatturaPA.Checked = True And SWFatturaPA = False Then
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                chkFatturaPA.BackColor = SEGNALA_KO : ControllaDocT = False
                chkFatturaPA.ToolTip = "Attenzione, il cliente selezionato non ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
            ElseIf chkFatturaPA.Checked = False And SWFatturaPA = True Then
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = True
                chkFatturaPA.AutoPostBack = True
                chkFatturaPA.BackColor = SEGNALA_INFO : ControllaDocT = False
                chkFatturaPA.ToolTip = "Attenzione, il cliente selezionato ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
            Else
                Session(CSTFATTURAPA) = SWFatturaPA
            End If
            Session(CSTFATTURAPA) = SWFatturaPA
        Else
            Call Documenti.CKClientiIPAByIDConORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
        End If
        '-
        If SplitIVA <> mySplitIVA Then
            WUC_ContrattiSpeseTraspTot1.SetErrChkSplitIVA(False)
            SWTB3 = False
        End If
        '---------
        'GIU171012
        txtDataDoc.BackColor = SEGNALA_OK : txtDataDoc.ToolTip = ""
        If txtDataDoc.Text.Trim <> "" Then
            If Not IsDate(txtDataDoc.Text.Trim) Then
                txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
                Session(CSTDATADOC) = ""
            Else
                'giu150312 controllo che la data non sia inferiore all'ultima fattura inserita
                'GIU041113 SBLOCCO CONTROLLO DATA E ULTIMO DOC. INSERITO SOLO PER:
                ' ''GIU050118 ATTIVARE PER TUTTI QUESTI TIPIDOC ANNO DOC = ANNO ESERCIZIO
                ' - CARICHI, SCARICHI E MOVIMENTI DI MAGAZZINO
                '   Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or 
                '   Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino)
                '   Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or 
                If Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                    Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                    If CDate(txtDataDoc.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
                        txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
                        Session(CSTDATADOC) = ""
                        txtDataDoc.ToolTip += "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim
                    ElseIf CDate(txtDataDoc.Text.Trim).Date < CDate(ControllaDataDoc1()).Date Then
                        txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
                        Session(CSTDATADOC) = ""
                        txtDataDoc.ToolTip += "DATA DOCUMENTO NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA DEL DOCUMENTO INSERITO."
                    ElseIf ControllaDataDoc2() = False Then
                        txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
                        Session(CSTDATADOC) = ""
                        txtDataDoc.ToolTip += "NUMERO DOCUMENTO NON COMPRESO NELLA DATA INDICATA."
                    Else
                        Session(CSTDATADOC) = txtDataDoc.Text.Trim
                    End If
                ElseIf Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or _
                       Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Or _
                       Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Then
                    If CDate(txtDataDoc.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
                        txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
                        Session(CSTDATADOC) = ""
                        txtDataDoc.ToolTip += "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim
                    Else
                        Session(CSTDATADOC) = txtDataDoc.Text.Trim
                    End If
                Else
                    Session(CSTDATADOC) = txtDataDoc.Text.Trim
                End If
            End If
        Else
            txtDataDoc.BackColor = SEGNALA_KO : ControllaDocT = False
            Session(CSTDATADOC) = ""
        End If
        'giu101219
        If Not IsNumeric(txtDurataNum.Text.Trim) Then
            txtDurataNum.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf Int(txtDurataNum.Text.Trim) = 0 Then
            txtDurataNum.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If DDLDurataTipo.SelectedValue.Trim = "" Then
            DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If Not IsNumeric(txtNVisite.Text.Trim) Then
            txtNVisite.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        txtDurataNum.BackColor = SEGNALA_OK
        txtDurataNum.ToolTip = ""
        If txtDataInizio.Text.Trim <> "" Then
            If Not IsDate(txtDataInizio.Text.Trim) Then
                txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataFine.Text.Trim) Then
                If CDate(txtDataFine.Text.Trim) < CDate(txtDataInizio.Text.Trim) Then
                    txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                    txtDataFine.ToolTip = "La data di fine Contratto dev'essere maggiore della data inizio"
                Else
                    Dim myDurata As Long = 0
                    If DDLDurataTipo.SelectedValue.Trim = "A" Then
                        myDurata = DateDiff(DateInterval.Year, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "M" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "T" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 3
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "Q" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 4
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "S" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 6
                    Else
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                    End If
                    If myDurata < 0 Then
                        myDurata = myDurata * -1
                    End If
                    myDurata = Math.Round(myDurata, 0)
                    If DDLDurataTipo.SelectedValue.Trim = "A" Or DDLDurataTipo.SelectedValue.Trim = "M" Then
                        myDurata += 1
                    End If
                    '-
                    If IsNumeric(txtDurataNum.Text.Trim) Then
                        If CLng(txtDurataNum.Text) <> myDurata Then
                            txtDurataNum.BackColor = SEGNALA_INFO ': ControllaDocT = False
                            txtDurataNum.ToolTip = "(ATTENDIONE) Durata N° prevista e di " & myDurata.ToString
                        End If
                    Else
                        txtDurataNum.BackColor = SEGNALA_KO : ControllaDocT = False
                        txtDurataNum.ToolTip = "Durata N° prevista e di " & myDurata.ToString
                    End If
                End If
            Else
                txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                txtDataFine.ToolTip = "Manca la data di fine Contratto"
            End If
        Else
            txtDataInizio.ToolTip = "Manca la data di inizio Contratto"
            txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If txtDataFine.Text.Trim <> "" Then
            If Not IsDate(txtDataFine.Text.Trim) Then
                txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf Not IsDate(txtDataInizio.Text.Trim) Then
                txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
                txtDataInizio.ToolTip = "Manca la data di inizio Contratto"
            End If
        Else
            txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
            txtDataFine.ToolTip = "Manca la data di fine Contratto"
        End If
        If txtDataAccettazione.Text.Trim <> "" Then
            If Not IsDate(txtDataAccettazione.Text.Trim) Then
                txtDataAccettazione.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''ElseIf IsDate(txtDataDoc.Text.Trim) Then
                ' ''    If CDate(txtDataAccettazione.Text.Trim) > CDate(txtDataDoc.Text.Trim) Then
                ' ''        txtDataAccettazione.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''        txtDataAccettazione.ToolTip = "La data Accettazione non può essere inferiore alla Data documento"
                ' ''    End If
            End If
        End If
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
        '-
        If txtDataValidita.Text.Trim <> "" Then
            If Not IsDate(txtDataValidita.Text.Trim) Then
                txtDataValidita.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        End If
        If txtDataConsegna.Text.Trim <> "" Then
            If Not IsDate(txtDataConsegna.Text.Trim) Then
                txtDataConsegna.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        ElseIf txtNGG_Consegna.Text.Trim = "" Then
            txtNGG_Consegna.Text = "0"
        End If
        'GIU130219
        If SWRifDoc = True And txtRiferimento.Text.Trim.Length > 20 Then
            txtRiferimento.BackColor = SEGNALA_KO : txtRiferimento.ToolTip = "Lunghezza massima consentita 20 caratteri a partire dal 01/01/2019"
            ControllaDocT = False
        Else
            txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = ""
        End If
        If txtDataRif.Text.Trim <> "" Then
            If Not IsDate(txtDataRif.Text.Trim) Then txtDataRif.BackColor = SEGNALA_KO : txtDataRif.ToolTip = "Data errata" : ControllaDocT = False
            If SWRifDoc = True Then
                If txtRiferimento.Text.Trim = "" Then txtDataRif.BackColor = SEGNALA_KO : txtDataRif.ToolTip = "Riferimento obbligatorio se presente la data" : ControllaDocT = False
            End If
        End If
        If ddlMagazzino.SelectedIndex = 0 Then
            ddlMagazzino.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        'giu280814 spostato qui la lettura della causale x sapere se il codiceCliFor è OBB.
        'giu140512
        Dim myCodCaus As Integer = 0
        If txtCodCausale.Text.Trim = "" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
            myCodCaus = -1
        Else
            myCodCaus = CLng(txtCodCausale.Text.Trim)
        End If
        '----------
        Dim RKCausMag As StrCausMag = GetDatiCausMag(myCodCaus, strErrore)
        If IsNothing(RKCausMag.Codice) Then
            strErrore = "CODICE CAUSALE NON PRESENTE IN TABELLA"
            txtCodCausale.ToolTip = strErrore
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf strErrore.Trim <> "" Then
            strErrore = strErrore.Trim
            txtCodCausale.ToolTip = strErrore
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '---------------------------------------------------------------------------------
        '---------
        'Cli/For/Provv 
        If txtCodCliForFilProvv.Text.Trim = "" Then
            'GIU280814
            If IsNothing(RKCausMag.Tipo) Then
                txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf RKCausMag.Tipo.Trim <> "" Then
                txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
            '---------
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) <> "(" And Left(txtCodCliForFilProvv.Text.Trim, 1) <> "1" And Left(txtCodCliForFilProvv.Text.Trim, 1) <> "9" Then
            txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf lblCliForFilProvv.Text.Trim = "" Then
            txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
        End If

        Session(IDRESPAREA) = txtCodRespArea.Text.Trim
        Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
        Dim myCodArea As Long = 0
        If String.IsNullOrEmpty(Session(IDRESPAREA)) Then
            '-
        ElseIf Not IsNumeric(Session(IDRESPAREA)) Or Session(IDRESPAREA) = "0" Then
            '-
        Else
            myCodArea = CLng(Session(IDRESPAREA))
        End If
        Dim myCodVisita As Long = 0
        If String.IsNullOrEmpty(Session(IDRESPVISITE)) Then
            '-
        ElseIf Not IsNumeric(Session(IDRESPVISITE)) Or Session(IDRESPVISITE) = "0" Then
            '-
        Else
            myCodVisita = CLng(Session(IDRESPVISITE))
        End If
        If myCodVisita = 0 And myCodArea = 0 Then
            strErrore = "Resp.Area obbligatorio"
            txtCodRespArea.ToolTip = strErrore
            txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
            '-
            strErrore = "Resp.Visite obbligatorio"
            txtCodRespVisite.ToolTip = strErrore
            txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf myCodVisita > 0 And myCodArea = 0 Then
            strErrore = "Resp.Area obbligatorio"
            txtCodRespArea.ToolTip = strErrore
            txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf myCodVisita = 0 Then
            strErrore = "Resp.Visite obbligatorio"
            txtCodRespVisite.ToolTip = strErrore
            txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf myCodArea = 0 Then
            strErrore = "Resp.Area obbligatorio"
            txtCodRespArea.ToolTip = strErrore
            txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim row() As DataRow
        If myCodArea > 0 Then
            strSQL = "SELECT * FROM RespArea WHERE (Codice = " & myCodArea.ToString.Trim & ")"
            Dim dsCon As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsCon)
                If (dsCon.Tables.Count > 0) Then
                    If (dsCon.Tables(0).Rows.Count > 0) Then
                        'ok
                    Else
                        strErrore = "Codice Resp.Area inesistente in tabella"
                        txtCodRespArea.ToolTip = strErrore
                        txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
                    End If
                Else
                    strErrore = "Codice Resp.Area inesistente in tabella"
                    txtCodRespArea.ToolTip = strErrore
                    txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            Catch Ex As Exception
                strErrore = "Codice Resp.Area inesistente in tabella"
                txtCodRespArea.ToolTip = strErrore & " - " & Ex.Message.Trim
                txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
            End Try
        End If
        If myCodVisita > 0 Then
            strSQL = "SELECT * FROM RespVisite WHERE (Codice = " & myCodVisita.ToString.Trim & ")"
            Dim dsCon As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsCon)
                If (dsCon.Tables.Count > 0) Then
                    If (dsCon.Tables(0).Rows.Count > 0) Then
                        'ok
                        row = dsCon.Tables(0).Select()
                        If myCodArea <> IIf(IsDBNull(row(0).Item("CodRespArea")), 0, row(0).Item("CodRespArea")) Then
                            strErrore = "Codice Resp.Visita non appartiene al Codice Resp.Area"
                            txtCodRespArea.ToolTip = strErrore
                            txtCodRespVisite.ToolTip = strErrore
                            txtCodRespArea.BackColor = SEGNALA_KO : ControllaDocT = False
                            txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
                        End If
                    Else
                        strErrore = "Codice Resp.Visita inesistente in tabella"
                        txtCodRespVisite.ToolTip = strErrore
                        txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
                    End If
                Else
                    strErrore = "Codice Resp.Visita inesistente in tabella"
                    txtCodRespVisite.ToolTip = strErrore
                    txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            Catch Ex As Exception
                strErrore = "Codice Resp.Visita inesistente in tabella"
                txtCodRespVisite.ToolTip = strErrore & " - " & Ex.Message.Trim
                txtCodRespVisite.BackColor = SEGNALA_KO : ControllaDocT = False
            End Try
        End If
        ObjDB = Nothing
        '----------
        Session(IDAGENTE) = txtCodAgente.Text.Trim
        'giu010412
        Dim myCAge As String = txtCodAgente.Text.Trim
        If Not IsNumeric(myCAge) Then myCAge = "0"
        Dim _CAge As Integer = CtrAgente(_CAge)
        If CInt(myCAge) <> _CAge Then
            lblMessAge.ForeColor = SEGNALA_KO
            lblMessAge.Text = "Assegnato: (" & _CAge.ToString.Trim & ")"
        Else
            lblMessAge.ForeColor = Drawing.Color.Blue
            lblMessAge.Text = ""
        End If
        'giu260410 
        '-
        'GIU310122 NON SERVE QUI A NULLA
        ' ''If RKCausMag.CVisione = True And checkADeposito.Checked = True Then
        ' ''    strErrore = "CODICE CAUSALE NON VALIDO PER IL C/DEPOSITO"
        ' ''    txtCodCausale.ToolTip = strErrore
        ' ''    txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        ' ''End If
        '' ''GIU161012 giu171012 ALTRIMENTI IL VENDUTO LI SCARTA COSI PER IL COSTO DEL VENDUTO
        ' ''If Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
        ' ''    If RKCausMag.Fatturabile = False Then
        ' ''        'strErrore = "CODICE CAUSALE NON VALIDO PER QUESTO TIPO DOCUMENTO"
        ' ''        txtCodCausale.BackColor = SEGNALA_INFO ' : ControllaDocT = False
        ' ''        strErrore = "CODICE CAUSALE NON FATTURABILE"
        ' ''    End If
        ' ''End If
        'GIU310122 NON SERVE QUI A NULLA
        '' ''giu290412 altri controlli
        ' ''If Not String.IsNullOrEmpty(RKCausMag.Segno_Giacenza) Then
        ' ''    If Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.OrdDepositi) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or _
        ' ''        Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
        ' ''        If RKCausMag.Segno_Giacenza = "+" Then
        ' ''            strErrore = "CODICE CAUSALE NON VALIDO PER QUESTO TIPO DOCUMENTO"
        ' ''            txtCodCausale.ToolTip = strErrore
        ' ''            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        ' ''        End If
        ' ''    Else
        ' ''        If RKCausMag.Segno_Giacenza = "-" And Session(CSTTIPODOC) <> SWTD(TD.MovimentoMagazzino) Then
        ' ''            strErrore = "CODICE CAUSALE NON VALIDO PER QUESTO TIPO DOCUMENTO"
        ' ''            txtCodCausale.ToolTip = strErrore
        ' ''            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        ' ''        End If
        ' ''    End If
        ' ''End If
        '-----------------------------
        If txtCodCausale.Text.Trim = "" Or txtCodCausale.Text.Trim = "0" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLCausali.BackColor = SEGNALA_KO
        End If
        If txtListino.Text.Trim = "" Or txtListino.Text.Trim = "0" Then
            txtListino.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLListini.BackColor = SEGNALA_KO
        End If
        'GIU280120
        If txtTipoFatt.Text.Trim = "" Or txtTipoFatt.Text.Trim = "0" Or DDLTipoFatt.SelectedValue.Trim = "" Then
            txtTipoFatt.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLTipoFatt.BackColor = SEGNALA_KO
        End If
        If txtPagamento.Text.Trim = "" Or txtPagamento.Text.Trim = "0" Or DDLPagamento.SelectedValue.Trim = "" Then
            txtPagamento.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLPagamento.BackColor = SEGNALA_KO
        End If
        'giu191219
        If Not IsNumeric(txtDurataNum.Text.Trim) Then
            txtDurataNum.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If DDLDurataTipo.SelectedValue.Trim <> "" And DDLTipoFatt.SelectedValue.Trim <> "" And txtDurataNum.BackColor <> SEGNALA_KO Then
            If DDLDurataTipo.SelectedValue.Trim <> "A" And DDLTipoFatt.SelectedValue = "A" Then
                If DDLDurataTipo.SelectedValue.Trim = "M" Then
                    If Val(txtDurataNum.Text.Trim) < 12 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 12 MESI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "Q" Then
                    If Val(txtDurataNum.Text.Trim) < 3 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 3 QUADRIMESTRI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "S" Then
                    If Val(txtDurataNum.Text.Trim) < 2 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 2 SEMESTRI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "T" Then
                    If Val(txtDurataNum.Text.Trim) < 4 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 4 TRIMESTRI"
                    End If
                End If
            End If
            If DDLDurataTipo.SelectedValue.Trim <> "A" And DDLTipoFatt.SelectedValue = "Q" Then
                If DDLDurataTipo.SelectedValue.Trim = "M" Then
                    If Val(txtDurataNum.Text.Trim) < 4 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 4 MESI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "Q" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 QUADRIMESTRI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "S" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 SEMESTRE"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "T" Then
                    If Val(txtDurataNum.Text.Trim) < 2 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 2 TRIMESTRI"
                    End If
                End If
            End If
            If DDLDurataTipo.SelectedValue.Trim <> "A" And DDLTipoFatt.SelectedValue = "S" Then
                If DDLDurataTipo.SelectedValue.Trim = "M" Then
                    If Val(txtDurataNum.Text.Trim) < 6 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 6 MESI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "Q" Then
                    If Val(txtDurataNum.Text.Trim) < 2 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 2 QUADRIMESTRI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "S" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 SEMESTRE"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "T" Then
                    If Val(txtDurataNum.Text.Trim) < 2 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 2 TRIMESTRI"
                    End If
                End If
            End If
            If DDLDurataTipo.SelectedValue.Trim <> "A" And DDLTipoFatt.SelectedValue = "T" Then
                If DDLDurataTipo.SelectedValue.Trim = "M" Then
                    If Val(txtDurataNum.Text.Trim) < 3 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 3 MESI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "Q" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 QUADRIMESTRI"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "S" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 SEMESTRE"
                    End If
                End If
                If DDLDurataTipo.SelectedValue.Trim = "T" Then
                    If Val(txtDurataNum.Text.Trim) < 1 Then
                        DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                        DDLDurataTipo.ToolTip = "Inserire una durata uguale o superiore a 1 TRIMESTRI"
                    End If
                End If
            End If
        End If
        '-
        Session(IDLISTINO) = txtListino.Text.Trim
        Session(CSTVALUTADOC) = lblCodValuta.Text.Trim
        'giu170412
        Dim DecimaliVal As String = GetDatiValute(lblCodValuta.Text.Trim, "").Decimali
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
            lblCodValuta.BackColor = SEGNALA_KO : ControllaDocT = False
        Else
            lblCodValuta.BackColor = SEGNALA_OK
        End If
        Session(CSTDECIMALIVALUTADOC) = DecimaliVal
    End Function
    'giu150312 giu230312 corretto per i tipo doc in comune es DT e DF unica numerazione idem per le FC FA
    Private Function ControllaDataDoc1() As DateTime
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ControllaDataDoc1)")
            Exit Function
        End If
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            ControllaDataDoc1 = Format(Now.Date, FormatoData)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select TOP 1 Data_Doc From ContrattiT WHERE "
        'GIU070814 Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "')"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
            If SWFatturaPA = True And Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0))"
                Else
                    strSQL += ")"
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            If SWFatturaPA = True Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "')"
                Else
                    strSQL += ")"
                End If
            End If
        Else 'per tutti gli altri
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        End If
        '-----------------------------------------------------------------
        'giu200312
        If txtNumero.Text.Trim <> "" Then
            If Not IsNumeric(txtNumero.Text.Trim) Then
                'nulla
            ElseIf CLng(txtNumero.Text.Trim) < 1 Then
                'nulla
            ElseIf CheckNewNumeroOnTab() = True Then
                strSQL += " AND CONVERT(INT, Numero) < " & CLng(txtNumero.Text.Trim).ToString.Trim 'GIU090814
            Else
                strSQL += " AND CONVERT(INT, Numero) < " & CLng(txtNumero.Text.Trim).ToString.Trim
            End If
        Else
            'nulla
        End If
        strSQL += " ORDER BY Data_Doc DESC" 'GIU231216
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Data_Doc")) Then
                        ControllaDataDoc1 = ds.Tables(0).Rows(0).Item("Data_Doc")
                        txtDataDoc.ToolTip = "(DATA ULTIMO DOCUMENTO: " & Format(ds.Tables(0).Rows(0).Item("Data_Doc"), FormatoData) & ") "
                    Else
                        ControllaDataDoc1 = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu110118 DATANULL
                    End If
                    Exit Function
                Else
                    ControllaDataDoc1 = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu110118 DATANULL
                    Exit Function
                End If
            Else
                ControllaDataDoc1 = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu110118 DATANULL
                Exit Function
            End If
        Catch Ex As Exception
            ControllaDataDoc1 = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu110118 DATANULL
            Exit Function
        End Try
    End Function
    Private Function ControllaDataDoc2() As Boolean
        ControllaDataDoc2 = True
        
        If txtNumero.BackColor = SEGNALA_KO Then
            Exit Function
        End If

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ControllaDataDoc1)")
            Exit Function
        End If
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MIN(CONVERT(INT, Numero)) AS MinN, MAX(CONVERT(INT, Numero)) AS MaxN "
        strSQL += "From ContrattiT WHERE "
        'giu070814 Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "')"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
            If SWFatturaPA = True And Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0))"
                Else
                    strSQL += ")"
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            If SWFatturaPA = True Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "')"
                Else
                    strSQL += ")"
                End If
            End If
        Else 'per tutti gli altri
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        End If
        '-----------------------------------------------------------------
        strSQL += " AND Data_Doc >= CONVERT(DATETIME, '" & txtDataDoc.Text.Trim & "',103) "
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    'giu110814
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("MinN")) And _
                       Not IsDBNull(ds.Tables(0).Rows(0).Item("MaxN")) Then
                        If ds.Tables(0).Rows(0).Item("MinN") > 0 And _
                           ds.Tables(0).Rows(0).Item("MinN") <> ds.Tables(0).Rows(0).Item("MaxN") Then
                            If CLng(txtNumero.Text.Trim) < ds.Tables(0).Rows(0).Item("MinN") Or _
                                CLng(txtNumero.Text.Trim) > ds.Tables(0).Rows(0).Item("MaxN") Then
                                'GIU231216 
                                If CLng(txtNumero.Text.Trim) > ds.Tables(0).Rows(0).Item("MaxN") Then
                                    'OK NULLA
                                Else
                                    txtDataDoc.ToolTip = "(Minimo Massimo Numero documenti: " & FormattaNumero(ds.Tables(0).Rows(0).Item("MinN")) & " - " & FormattaNumero(ds.Tables(0).Rows(0).Item("MaxN")) & ") "
                                    ControllaDataDoc2 = False
                                End If
                                '---------
                                ' ''txtDataDoc.ToolTip = "(Minimo Massimo Numero documenti: " & FormattaNumero(ds.Tables(0).Rows(0).Item("MinN")) & " - " & FormattaNumero(ds.Tables(0).Rows(0).Item("MaxN")) & ") "
                                ' ''ControllaDataDoc2 = False
                                '----------
                            End If
                        End If
                    Else 'NON C'è NESSUN DOCUMENTO QUINDI LEGGO L'ULTIMO NUMERO E ULTIMA DATA
                        ControllaDataDoc2 = ControllaDataDoc3()
                    End If
                    Exit Function
                Else
                    Exit Function
                End If
            Else
                Exit Function
            End If
        Catch Ex As Exception
            Exit Function
        End Try
    End Function
    Private Function ControllaDataDoc3() As Boolean
        ControllaDataDoc3 = True
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ControllaDataDoc1)")
            Exit Function
        End If
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS MaxN "
        strSQL += "From ContrattiT "
        'giu070814 WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "')"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
            If SWFatturaPA = True And Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0))"
                Else
                    strSQL += ")"
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            If SWFatturaPA = True Then
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0))"
                Else
                    strSQL += ")"
                End If
            Else
                strSQL += "((Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "')"
                Else
                    strSQL += ")"
                End If
            End If
        Else 'per tutti gli altri
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        End If
        '-----------------------------------------------------------------------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("MaxN")) Then
                        If ds.Tables(0).Rows(0).Item("MaxN") > 0 Then
                            If CLng(txtNumero.Text.Trim) < ds.Tables(0).Rows(0).Item("MaxN") Then
                                txtDataDoc.ToolTip = "(N° massimo documenti: " & FormattaNumero(ds.Tables(0).Rows(0).Item("MaxN")) & ") "
                                ControllaDataDoc3 = False
                            End If
                        End If
                    End If
                    Exit Function
                Else
                    Exit Function
                End If
            Else
                Exit Function
            End If
        Catch Ex As Exception
            Exit Function
        End Try
    End Function
    '010412 Controllo Agente se collegato al cliente
    Private Function CtrAgente(ByRef _CAge As Integer) As Integer
        CtrAgente = 0
        _CAge = 0
        If txtCodCliForFilProvv.Text.Trim = "" Then
            Exit Function
        End If
        'Anagrafiche provvisorie EXIT SUB
        If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
            Exit Function
        End If
        'giu191211
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            Exit Function
        End If
        SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
        '----------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        '--------
        'giu240113 
        Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        '---------
        Try
            Dim dvCliFor As DataView
            dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
            If (dvCliFor Is Nothing) Then
                Exit Function
            End If
            If dvCliFor.Count > 0 Then
                If dvCliFor.Count > 0 Then
                    If Not IsDBNull(dvCliFor.Item(0).Item("Agente_N")) Then
                        CtrAgente = dvCliFor.Item(0).Item("Agente_N")
                        _CAge = dvCliFor.Item(0).Item("Agente_N")
                    End If
                End If
            Else
                Exit Function
            End If
        Catch ex As Exception
            CtrAgente = 0
            _CAge = 0
        End Try

    End Function

    Private Function ControllaDocD(ByRef myStrEsito As String) As Boolean

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (WUC_Contratti.ControllaDocD)")
            Exit Function
        End If
        ControllaDocD = True

        myStrEsito = ""
        Dim myStatoDoc As String = Session(CSTSTATODOC)
        If IsNothing(myStatoDoc) Then
            myStatoDoc = "0"
        End If
        If String.IsNullOrEmpty(myStatoDoc) Then
            myStatoDoc = "0"
        End If
        If TipoDoc = "" Then
            myStatoDoc = "0"
        End If
        Dim OKQtaEv As Boolean = False
        Dim DsDocDettTmp As New DSDocumenti
        '-
        Try
            DsDocDettTmp = Session("aDsDett")
        Catch ex As Exception
            ControllaDocD = False
            myStrEsito = "Errore, controllo DETTAGLI"
            Exit Function
        End Try
        'giu290420 non serve
        ' ''If myStatoDoc = "5" And TipoDoc = SWTD(TD.OrdClienti) Then
        ' ''    Try
        ' ''        OKQtaEv = False
        ' ''        For Each RowDett In DsDocDettTmp.ContrattiD
        ' ''            If RowDett.RowState <> DataRowState.Deleted Then
        ' ''                If RowDett.Item("Qta_Evasa") > 0 Then
        ' ''                    OKQtaEv = True
        ' ''                    Exit For
        ' ''                End If
        ' ''            End If
        ' ''        Next
        ' ''        If OKQtaEv = False Then
        ' ''            ControllaDocD = False
        ' ''            myStrEsito = "Attenzione, inserire almeno una quantità d'allestire"
        ' ''            Exit Function
        ' ''        Else
        ' ''            ControllaDocD = True
        ' ''        End If
        ' ''    Catch ex As Exception
        ' ''        ControllaDocD = False
        ' ''        myStrEsito = "Errore, controllo quantità d'allestire"
        ' ''        Exit Function
        ' ''    End Try
        ' ''Else
        ' ''    ControllaDocD = True
        ' ''End If
        '-
        'giu050319 CONTROLLO IMPORTO RIGA (SCONTO CASSA)
        Dim strErrore As String = ""
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = "2" ' Session(CSTDECIMALIVALUTADOC)
        Dim CodValuta As String = "Euro"
        Dim ScontoSuImporto As Boolean = True
        Dim ScCassaDett As Boolean = False 'giu010119 
        Try
            ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        '-
        DecimaliVal = App.GetDatiValute(CodValuta, strErrore).Decimali
        If strErrore.Trim <> "" Then
            ControllaDocD = False
            myStrEsito = strErrore & " - Controllo Totale Documento (lettura GetDatiValute)."
            Return False
            Exit Function
        End If
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '-
        Dim myQuantita As Decimal = 0
        Dim MyImporto As Decimal = 0
        Dim Importo As Decimal = 0
        '-
        For Each rsDettagli In DsDocDettTmp.Tables("ContrattiD").Select("", "Riga")
            Select Case Left(TipoDoc, 1)
                Case "O"
                    myQuantita = rsDettagli![Qta_Ordinata]
                Case Else
                    If TipoDoc = "PR" Or TipoDoc = "TC" Or TipoDoc = "CA" Then 'GIU021219
                        myQuantita = rsDettagli![Qta_Ordinata]
                    Else
                        myQuantita = rsDettagli![Qta_Evasa]
                    End If
            End Select
            'giu020519 FATTURE PER ACCONTI 
            rsDettagli.BeginEdit()
            If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
            If IsDBNull(rsDettagli!SWCalcoloTot) Then rsDettagli!SWCalcoloTot = False
            rsDettagli.EndEdit()
            rsDettagli.AcceptChanges()
            'CONTROLLO IMPORTO CHE SIANO UGLUALI
            If rsDettagli!SWCalcoloTot = True Then myQuantita = 0
            MyImporto = Documenti.CalcolaImporto(rsDettagli![Prezzo], myQuantita, _
                    rsDettagli![Sconto_1], _
                    rsDettagli![Sconto_2], _
                    rsDettagli![Sconto_3], _
                    rsDettagli![Sconto_4], _
                    rsDettagli![Sconto_Pag], _
                    rsDettagli![ScontoValore], _
                    rsDettagli![Importo], _
                    ScontoSuImporto, _
                    CInt(DecimaliVal), _
                    rsDettagli![Prezzo_Netto], ScCassaDett, CDec(ScCassa), rsDettagli!DedPerAcconto) 'giu020519
            If rsDettagli![Importo] <> MyImporto Then
                ControllaDocD = False
                myStrEsito = "Errore CONTROLLO Documento: Importo riga: (" & rsDettagli![Riga].ToString.Trim & ") diverso dall'importo riga ricalcolato. <br> " & _
                "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe. <br> " & _
                "Oppure controllare lo Sconto Cassa."
                Return False
                Exit Function
            End If
        Next
    End Function

    Private Function ControllaDocS() As Boolean
        ControllaDocS = True
        'cntrollo VETTORE SE RICHIESTO 
        ControllaDocS = WUC_ContrattiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        SWNoDivTotRate, SWAccorpaRateAA, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
        'giu160118
        RitAcconto = False
        ImponibileRA = 0
        PercRA = 0
        TotaleRA = 0
        'If RitAcconto = True Then
        '    Dim ErrRitAcc As Boolean = False
        '    If ImponibileRA = 0 Then
        '        ControllaDocS = False
        '        ErrRitAcc = True
        '    End If
        '    If PercRA = 0 Then
        '        ControllaDocS = False
        '        ErrRitAcc = True
        '    End If
        '    If TotaleRA = 0 Then
        '        ControllaDocS = False
        '        ErrRitAcc = True
        '    End If
        '    If ErrRitAcc = True Then
        '        WUC_ContrattiSpeseTraspTot1.SetErrRitAcc(False)
        '    End If
        'End If
        '---------
        'GIU040319 OK PER IL BOLLO MENTRE LO SPLIT IVA E' CONTROLLATO IN ControllaDocT
        If Bollo = 0 Then
            BolloACaricoDel = ""
            'myBolloSI VERRA' CONTROLLATO DOPO
        ElseIf BolloACaricoDel.Trim = "" Then
            ControllaDocS = False
            WUC_ContrattiSpeseTraspTot1.SetErrBollo(False)
            'myBolloSI VERRA' CONTROLLATO DOPO
        End If
    End Function

    Public Function GetDatiTB3() As Boolean
        GetDatiTB3 = WUC_ContrattiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                     SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                     ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                     SWNoDivTotRate, SWAccorpaRateAA, SplitIVA, RitAcconto, _
                     ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
    End Function

    Public Function GetCodCausDATipoCAIdPag(ByRef pCod_Causale As Long, ByRef pNVisite As Integer, ByRef pDurataNum As Integer) As Boolean
        GetCodCausDATipoCAIdPag = True
        pCod_Causale = 0 : pNVisite = 0 : pDurataNum = 0
        Dim CodPag As String = Session(CSTIDPAG)
        If IsNothing(CodPag) Then CodPag = ""
        If String.IsNullOrEmpty(CodPag) Then CodPag = ""
        If Not IsNumeric(CodPag) Then CodPag = ""
        If CodPag = "" Or CodPag = "0" Then Exit Function
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From TipoContratto WHERE Codice = " & CodPag.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                    pCod_Causale = IIf(IsDBNull(rowPag(0).Item("Cod_Causale")), 0, rowPag(0).Item("Cod_Causale"))
                    pNVisite = IIf(IsDBNull(rowPag(0).Item("NVisite")), 0, rowPag(0).Item("NVisite"))
                    pDurataNum = IIf(IsDBNull(rowPag(0).Item("DurataNum")), 0, rowPag(0).Item("DurataNum"))
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                    GetCodCausDATipoCAIdPag = False
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                GetCodCausDATipoCAIdPag = False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura TipoContratto: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            GetCodCausDATipoCAIdPag = False
            Exit Function
        End Try
    End Function
    Public Function AggiornaDocT(ByVal DSDettBack As DSDocumenti, ByVal SWNonAggNumDoc As Boolean, ByRef strErrore As String) As Boolean
        Session("ErrSQL") = SWNO
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            ' ''Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.AggiornaDocT)")
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO (Contratti.AggiornaDocT)<br>Assegno il tipo: CONTRATTO ASSISTENZA", WUC_ModalPopup.TYPE_ERROR)
            TipoDoc = SWTD(TD.ContrattoAssistenza)
            Session(CSTTIPODOC) = TipoDoc
        End If
        AggiornaDocT = True
        Dim SWErr As Boolean = False
        '--------------------------------------------------------------------------------------
        'giu231211 giu241211 CALCOLO IL TOTALE SPESE, TRASPORTO, TOTALE SE HO SOLO IMPUTATO UNO DI QUESTI
        'giu190220
        Session(CSTDURATANUM) = txtDurataNum.Text.Trim
        Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
        Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        'giu190220
        ' ''If Not (DSDettBack Is Nothing) Then
        ' ''    If WUC_ContrattiDett1.AggiornaImporto(DSDettBack, strErrore) = False Then
        ' ''        If strErrore.Trim <> "" Then
        ' ''            AggiornaDocT = False 'giu160118
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''            ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''            Exit Function
        ' ''        End If
        ' ''    End If
        ' ''End If
        'giu190220
        If Not (DSDettBack Is Nothing) Then
            If WUC_ContrattiDett1.AggiornaImportoALLAtt(DSDettBack, strErrore) = False Then
                If strErrore.Trim <> "" Then
                    AggiornaDocT = False 'giu160118
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Session(CSTNONCOMPLETO) = SWSI
                    Exit Function
                End If
            End If
        End If
        
        'Chiamo TD0 per richiamare TD3 SPESE,TRASPORTO,TOTALE,SCADENZE AGGIORNARE OK FATTO
        GetDatiTB3() 'TD3 Spese, Trasporto, Totale
        'giu041123 prendo le note ritiro all da ContrattiDett
        Dim NoteIntervento As String = WUC_ContrattiDett1.GetNoteIntervento
        '----------------------------------------------------
        'giu160118
        If RitAcconto = True Then
            If ImponibileRA = 0 Then SWTB3 = False
            If PercRA = 0 Then SWTB3 = False
            If TotaleRA = 0 Then SWTB3 = False
        End If
        '---------
        '--------------------------------------------------------------------------------------
        ''txtRevNDoc.AutoPostBack = False
        ''If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        ''txtRevNDoc.AutoPostBack = True
        If Not IsNumeric(txtNGG_Consegna.Text.Trim) Then txtNGG_Consegna.Text = "0"
        If Not IsNumeric(txtNGG_Validita.Text.Trim) Then txtNGG_Validita.Text = "0"
        Dim pCod_Causale As Long = 0 : Dim pNVisite As Integer = 0 : Dim pDurataNum As Integer = 0
        If Session(SWOP) = SWOPNUOVO Then
            Dim newRowDocT As DSDocumenti.ContrattiTRow = DsDocT.ContrattiT.NewContrattiTRow
            With newRowDocT
                .BeginEdit()
                .Tipo_Doc = Session(CSTTIPODOC)
                If SWNonAggNumDoc = False Then
                    .Numero = txtNumero.Text.Trim
                    .RevisioneNDoc = 0 ' Int(txtRevNDoc.Text.Trim)
                End If
                If IsDate(txtDataDoc.Text) Then
                    .Data_Doc = CDate(txtDataDoc.Text)
                    Session(CSTDATADOC) = txtDataDoc.Text.Trim
                Else
                    .SetData_DocNull()
                    Session(CSTDATADOC) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtDataDoc.BackColor = SEGNALA_KO
                End If
                '-
                If IsDate(txtDataInizio.Text) Then
                    .DataInizio = CDate(txtDataInizio.Text)
                    Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
                Else
                    .SetDataInizioNull()
                    Session(CSTDATAINIZIO) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtDataInizio.BackColor = SEGNALA_KO
                End If
                If IsDate(txtDataFine.Text) Then
                    .DataFine = CDate(txtDataFine.Text)
                    Session(CSTDATAFINE) = txtDataFine.Text.Trim
                Else
                    .SetDataFineNull()
                    Session(CSTDATAFINE) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtDataFine.BackColor = SEGNALA_KO
                End If
                If IsDate(txtDataAccettazione.Text) Then
                    .DataAccetta = CDate(txtDataAccettazione.Text)
                    Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
                Else
                    .SetDataAccettaNull()
                    Session(CSTDATAACCETTA) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtDataAccettazione.BackColor = SEGNALA_KO
                End If
                '---------
                If txtTipoFatt.Text.Trim = "" Then
                    .SetTipo_FatturazioneNull()
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    DDLTipoFatt.BackColor = SEGNALA_KO
                Else
                    .Tipo_Fatturazione = txtTipoFatt.Text.Trim
                End If
                '---------
                If IsDate(txtDataValidita.Text) Then
                    .Data_Validita = CDate(txtDataValidita.Text)
                Else
                    .SetData_ValiditaNull()
                End If
                .NGG_Validita = Int(txtNGG_Validita.Text.Trim)
                If IsDate(txtDataConsegna.Text) Then
                    .DataOraConsegna = CDate(txtDataConsegna.Text)
                Else
                    .SetDataOraConsegnaNull()
                End If
                .NGG_Consegna = Int(txtNGG_Consegna.Text.Trim)
                .CIG = txtCIG.Text.Trim
                .CUP = txtCUP.Text.Trim
                .Riferimento = txtRiferimento.Text.Trim
                'giu071211
                If IsDate(txtDataRif.Text) Then
                    .Data_Riferimento = CDate(txtDataRif.Text)
                Else
                    .SetData_RiferimentoNull()
                End If
                '-
                'GIU200420 USATO PER LE RATE SCADENZA
                .SWTipoEvTotale = SWNoDivTotRate
                .SWTipoEvSaldo = SWAccorpaRateAA
                ' ''If optTipoEvTotaleTotale.Checked = True Then
                ' ''    .SWTipoEvTotale = True
                ' ''ElseIf optTipoEvTotaleParziale.Checked = True Then
                ' ''    .SWTipoEvTotale = False
                ' ''Else
                ' ''    .SetSWTipoEvTotaleNull()
                ' ''End If
                '' ''-
                ' ''If CKSWTipoEvSaldo = True Then
                ' ''    .SWTipoEvSaldo = True
                ' ''Else
                ' ''    .SWTipoEvSaldo = False
                ' ''End If
                If Not IsNumeric(txtPagamento.Text.Trim) Or txtPagamento.Text.Trim = "0" Then
                    .SetTipoContrattoNull()
                    Session(CSTIDPAG) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    DDLPagamento.BackColor = SEGNALA_KO
                Else
                    .TipoContratto = txtPagamento.Text.Trim
                    Session(CSTIDPAG) = txtPagamento.Text.Trim
                End If
                .SWTipoModSint = CBool(chkSWTipoModSint.Checked)
                txtCodCausale.AutoPostBack = False
                If GetCodCausDATipoCAIdPag(pCod_Causale, pNVisite, pDurataNum) = False Then
                    AggiornaDocT = False
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    DDLPagamento.BackColor = SEGNALA_KO
                End If
                '-
                txtDurataNum.AutoPostBack = False
                txtDurataNum.Text = pDurataNum.ToString.Trim
                txtDurataNum.AutoPostBack = True
                If Not IsNumeric(txtDurataNum.Text.Trim) Then txtDurataNum.Text = "0"
                .DurataNum = Int(txtDurataNum.Text.Trim)
                If DDLDurataTipo.SelectedIndex > 0 Then
                    .DurataTipo = DDLDurataTipo.SelectedValue
                Else
                    .SetDurataTipoNull()
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    DDLDurataTipo.BackColor = SEGNALA_KO
                End If
                '-
                txtNVisite.Text = pNVisite.ToString.Trim
                If Not IsNumeric(txtNVisite.Text.Trim) Then txtNVisite.Text = "0"
                .NVisite = Int(txtNVisite.Text.Trim)
                '-
                '-----------
                txtCodCausale.Text = pCod_Causale.ToString.Trim
                txtCodCausale.AutoPostBack = True
                If Not IsNumeric(txtCodCausale.Text.Trim) Or txtCodCausale.Text.Trim = "0" Then
                    .SetCod_CausaleNull()
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    DDLPagamento.BackColor = SEGNALA_KO
                Else
                    .Cod_Causale = txtCodCausale.Text.Trim
                End If
                '--------------------------------------------------
                If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
                    Dim strIDAnagrProvv As String = txtCodCliForFilProvv.Text.Trim
                    strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
                    If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then 'giu150513
                        strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
                    End If
                    .IDAnagrProvv = strIDAnagrProvv.Trim
                    .SetCod_ClienteNull()
                    .SetCod_FornitoreNull()
                    'GIU020420
                    SplitIVA = False
                    Session(CSTSPLITIVA) = SplitIVA
                    '----------------
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtCodCliForFilProvv.BackColor = SEGNALA_KO
                Else
                    If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "1" Then
                        .Cod_Cliente = txtCodCliForFilProvv.Text.Trim
                        .SetIDAnagrProvvNull()
                        .SetCod_FornitoreNull()
                    ElseIf Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "9" Then
                        .Cod_Fornitore = txtCodCliForFilProvv.Text.Trim
                        .SetIDAnagrProvvNull()
                        .SetCod_ClienteNull()
                        'GIU020420
                        SplitIVA = False
                        Session(CSTSPLITIVA) = SplitIVA
                        '----------------
                    Else
                        .Cod_Cliente = txtCodCliForFilProvv.Text.Trim
                        .SetIDAnagrProvvNull()
                        .SetCod_FornitoreNull()
                    End If
                End If
                '-
                If lblDestSel.Text.Trim <> "" Then 'giu260320
                    Dim myCGDest As String = Session(CSTCODFILIALE)
                    If Not IsNothing(myCGDest) Then
                        If String.IsNullOrEmpty(myCGDest) Then
                            myCGDest = ""
                        End If
                    Else
                        myCGDest = ""
                    End If
                    If myCGDest.Trim = "" Then
                        myCGDest = lblDestSel.ToolTip 'giu230520
                    End If
                    If IsNumeric(myCGDest) Then
                        .Cod_Filiale = myCGDest.Trim 'DDLDestinazioni.SelectedValue
                    Else
                        .SetCod_FilialeNull()
                        lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                        Session(CSTCODFILIALE) = ""
                        ' ''txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                        lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                        lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                        lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                        lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                    End If
                Else
                    .SetCod_FilialeNull()
                    lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                    Session(CSTCODFILIALE) = ""
                    ' ''txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                    lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                    lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                    lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                    lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                End If
                .Destinazione1 = txtDestinazione1.Text.Trim
                .Destinazione2 = txtDestinazione2.Text.Trim
                .Destinazione3 = txtDestinazione3.Text.Trim
                '-
                If lblABI.Text.Trim = "" Then
                    .SetABINull()
                Else
                    .ABI = lblABI.Text.Trim
                End If
                If lblCAB.Text.Trim = "" Then
                    .SetCABNull()
                Else
                    .CAB = lblCAB.Text.Trim
                End If
                If lblIBAN.Text.Trim = "" Then
                    .SetIBANNull()
                Else
                    .IBAN = lblIBAN.Text.Trim
                End If
                If lblContoCorrente.Text.Trim = "" Then
                    .SetContoCorrenteNull()
                Else
                    .ContoCorrente = lblContoCorrente.Text.Trim
                End If
                'giu161219
                If Not IsNumeric(txtCodRespArea.Text.Trim) Or txtCodRespArea.Text.Trim = "0" Then
                    .SetRespAreaNull()
                    Session(IDRESPAREA) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtCodRespArea.BackColor = SEGNALA_KO
                Else
                    .RespArea = txtCodRespArea.Text.Trim
                    Session(IDRESPAREA) = txtCodRespArea.Text.Trim
                End If
                If Not IsNumeric(txtCodRespVisite.Text.Trim) Or txtCodRespVisite.Text.Trim = "0" Then
                    .SetRespVisiteNull()
                    Session(IDRESPVISITE) = ""
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                    txtCodRespVisite.BackColor = SEGNALA_KO
                Else
                    .RespVisite = txtCodRespVisite.Text.Trim
                    Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
                End If
                '-
                If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                    .SetCod_AgenteNull()
                    Session(IDAGENTE) = ""
                Else
                    .Cod_Agente = txtCodAgente.Text.Trim
                    Session(IDAGENTE) = txtCodAgente.Text.Trim
                End If
                
                .ADeposito = checkADeposito.Checked 'giu260412
                If Not IsNumeric(txtListino.Text.Trim) Or txtListino.Text.Trim = "0" Then
                    .SetListinoNull()
                    Session(IDLISTINO) = ""
                Else
                    .Listino = txtListino.Text.Trim
                    Session(IDLISTINO) = txtListino.Text.Trim
                End If
                If lblCodValuta.Text.Trim = "" Then
                    .SetCod_ValutaNull()
                    Session(CSTVALUTADOC) = ""
                    Session(CSTDECIMALIVALUTADOC) = 0
                Else
                    .Cod_Valuta = lblCodValuta.Text.Trim
                    Session(CSTVALUTADOC) = lblCodValuta.Text.Trim
                End If
                'giu070814
                .FatturaPA = chkFatturaPA.Checked
                .TipoRapportoPA = DDLTipoRapp.SelectedValue
                .CodiceMagazzino = ddlMagazzino.SelectedValue
                .Acconto = IIf(ChkAcconto.Checked, 1, 0)
                .FatturaAC = chkFatturaAC.Checked
                .ScGiacenza = chkScGiacenza.Checked
                '-
                'TD3 Chiamato da ControllaDocS
                If TipoSped <> "" Then
                    .Tipo_Spedizione = TipoSped
                Else
                    .SetTipo_SpedizioneNull()
                End If
                If Porto <> "" Then
                    .Porto = Porto
                Else
                    .SetPortoNull()
                End If
                If Vettori(0) <> 0 Then
                    .Vettore_1 = Vettori(0)
                Else
                    .SetVettore_1Null()
                End If
                If Vettori(1) <> 0 Then
                    .Vettore_2 = Vettori(1)
                Else
                    .SetVettore_2Null()
                End If
                If Vettori(2) <> 0 Then
                    .Vettore_3 = Vettori(2)
                Else
                    .SetVettore_3Null()
                End If
                .Spese_Incasso = CDec(SpIncasso)
                .Spese_Trasporto = CDec(SpTrasp)
                .Spese_Imballo = CDec(SpImballo)
                .SpeseVarie = 0
                Dim RegimeIVA As String = Session(CSTREGIMEIVA)
                'giu260219 RegimeIVA = "0"
                Dim myRegIVA As String = Trim(Mid(LnkRegimeIVA.Text, 11))
                If IsNothing(RegimeIVA) Then
                    If IsNumeric(myRegIVA) Then
                        RegimeIVA = myRegIVA
                    Else
                        RegimeIVA = "0"
                    End If
                End If
                If String.IsNullOrEmpty(RegimeIVA) Then
                    If IsNumeric(myRegIVA) Then
                        RegimeIVA = myRegIVA
                    Else
                        RegimeIVA = "0"
                    End If
                End If
                If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
                .Cod_Iva = CLng(RegimeIVA)
                'Blocco IVA - Aliquota IVA
                If Iva(0) <> 0 Then
                    .Iva1 = Iva(0)
                Else
                    .SetIva1Null()
                End If
                If Iva(1) <> 0 Then
                    .Iva2 = Iva(1)
                Else
                    .SetIva2Null()
                End If
                If Iva(2) <> 0 Then
                    .Iva3 = Iva(2)
                Else
                    .SetIva3Null()
                End If
                If Iva(3) <> 0 Then
                    .Iva4 = Iva(3)
                Else
                    .SetIva4Null()
                End If
                'Blocco IVA - Imponibile
                If Imponibile(0) <> 0 Then
                    .Imponibile1 = CDec(Imponibile(0))
                Else
                    .Imponibile1 = 0
                End If
                If Imponibile(1) <> 0 Then
                    .Imponibile2 = Imponibile(1)
                Else
                    .Imponibile2 = 0
                End If
                If Imponibile(2) <> 0 Then
                    .Imponibile3 = CDec(Imponibile(2))
                Else
                    .Imponibile3 = 0
                End If
                If Imponibile(3) <> 0 Then
                    .Imponibile4 = CDec(Imponibile(3))
                Else
                    .Imponibile4 = 0
                End If
                'Blocco IVA - Imposta
                If Imposta(0) <> 0 Then
                    .Imposta1 = CDec(Imposta(0))
                Else
                    .Imposta1 = 0
                End If
                If Imposta(1) <> 0 Then
                    .Imposta2 = CDec(Imposta(1))
                Else
                    .Imposta2 = 0
                End If
                If Imposta(2) <> 0 Then
                    .Imposta3 = CDec(Imposta(2))
                Else
                    .Imposta3 = 0
                End If
                If Imposta(3) <> 0 Then
                    .Imposta4 = CDec(Imposta(3))
                Else
                    .Imposta4 = 0
                End If
                .Totale = CDec(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4) + Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4))
                .TotaleM = CDec(TotMerceLordo) 'giu050319 CDec(TotMerce)
                .Abbuono = CDec(Abbuono)
                .Sconto_Cassa = CDec(ScCassa)
                .ScontoReale = 0 'per adessso sempre 0
                .Aspetto = Aspetto
                .Peso = CDec(Peso)
                .Colli = Colli
                .Pezzi = Pezzi
                'Blocco Data Scadenza QUI SONO SEMPRE VUOTI
                ' ''If DataScad(0) <> "" Then
                ' ''    .Data_Scadenza_1 = CDate(DataScad(0))
                ' ''Else
                ' ''    .SetData_Scadenza_1Null()
                ' ''End If
                ' ''If DataScad(1) <> "" Then
                ' ''    .Data_Scadenza_2 = CDate(DataScad(1))
                ' ''Else
                ' ''    .SetData_Scadenza_2Null()
                ' ''End If
                ' ''If DataScad(2) <> "" Then
                ' ''    .Data_Scadenza_3 = CDate(DataScad(2))
                ' ''Else
                ' ''    .SetData_Scadenza_3Null()
                ' ''End If
                ' ''If DataScad(3) <> "" Then
                ' ''    .Data_Scadenza_4 = CDate(DataScad(3))
                ' ''Else
                ' ''    .SetData_Scadenza_4Null()
                ' ''End If
                ' ''If DataScad(4) <> "" Then
                ' ''    .Data_Scadenza_5 = CDate(DataScad(4))
                ' ''Else
                ' ''    .SetData_Scadenza_5Null()
                ' ''End If
                '' ''Blocco Scadenze RATE
                ' ''.Rata_1 = CDec(ImpRata(0))
                ' ''.Rata_2 = CDec(ImpRata(1))
                ' ''.Rata_3 = CDec(ImpRata(2))
                ' ''.Rata_4 = CDec(ImpRata(3))
                ' ''.Rata_5 = CDec(ImpRata(4))
                'giu020220 GIU060220
                .SetData_Scadenza_1Null()
                .SetData_Scadenza_2Null()
                .SetData_Scadenza_3Null()
                .SetData_Scadenza_4Null()
                .SetData_Scadenza_5Null()
                .Rata_1 = 0
                .Rata_2 = 0
                .Rata_3 = 0
                .Rata_4 = 0
                .Rata_5 = 0
                .ScadPagCA = ""
                .ScadAttCA = ""
                '---------
                'giu041123 .DataOraRitiro = DataOraRitiro
                .Descrizione_Imballo = DesImballo
                .DescrizionePorto = DesPorto
                '-----------------------------
                .DesRefInt = txtDesRefInt.Text.Trim
                .NoteDocumento = txtNoteDocumento.Text.Trim
                .NoteRitiro = NoteIntervento 'giu041123 
                .StatoDoc = 0
                .InseritoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                Session(CSTSTATODOC) = "0"
                'giu270220
                If Session(CSTNONCOMPLETO) = SWSI Then
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                ElseIf Session(CSTNONCOMPLETO) = SWNO Then
                    .StatoDoc = 0
                    Session(CSTSTATODOC) = "0"
                End If
                'giu120118
                .SplitIVA = SplitIVA
                .RitAcconto = RitAcconto
                .ImponibileRA = ImponibileRA
                .PercRA = PercRA
                .TotaleRA = TotaleRA
                .TotNettoPagare = TotNettoPagare
                If TotNettoPagare <= 0 Then
                    Session(CSTNONCOMPLETO) = SWSI
                    .StatoDoc = 5
                    Session(CSTSTATODOC) = "5"
                End If
                '---------
                'giu260219
                .Bollo = Bollo
                If Bollo <> 0 Then
                    .BolloACaricoDel = BolloACaricoDel
                Else
                    .BolloACaricoDel = ""
                End If

                .EndEdit()
            End With
            DsDocT.ContrattiT.AddContrattiTRow(newRowDocT)
            newRowDocT = Nothing
            Session("DsDocT") = DsDocT
        Else 'MODIFICA
            Dim myNonCompleto = SWNO
            dvDocT = Session("dvDocT")
            If SWNonAggNumDoc = False Then
                dvDocT.Item(0).Item("Numero") = txtNumero.Text.Trim
                dvDocT.Item(0).Item("RevisioneNDoc") = 0 ' Int(txtRevNDoc.Text.Trim)
            End If

            If IsDate(txtDataDoc.Text) Then
                dvDocT.Item(0).Item("Data_Doc") = CDate(txtDataDoc.Text)
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
            Else
                dvDocT.Item(0).Item("Data_Doc") = DBNull.Value
                Session(CSTDATADOC) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtDataDoc.BackColor = SEGNALA_KO
            End If
            'giu101219 giu280120
            dvDocT.Item(0).Item("DurataNum") = Int(txtDurataNum.Text.Trim)
            If DDLDurataTipo.SelectedIndex > 0 Then
                dvDocT.Item(0).Item("DurataTipo") = DDLDurataTipo.SelectedValue
            Else
                dvDocT.Item(0).Item("DurataTipo") = DBNull.Value
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            dvDocT.Item(0).Item("NVisite") = Int(txtNVisite.Text.Trim)
            '-
            If IsDate(txtDataInizio.Text) Then
                dvDocT.Item(0).Item("DataInizio") = CDate(txtDataInizio.Text)
                Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
            Else
                dvDocT.Item(0).Item("DataInizio") = DBNull.Value
                Session(CSTDATAINIZIO) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtDataInizio.BackColor = SEGNALA_KO
            End If
            If IsDate(txtDataFine.Text) Then
                dvDocT.Item(0).Item("DataFine") = CDate(txtDataFine.Text)
                Session(CSTDATAFINE) = txtDataFine.Text.Trim
            Else
                dvDocT.Item(0).Item("DataFine") = DBNull.Value
                Session(CSTDATAFINE) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtDataFine.BackColor = SEGNALA_KO
            End If
            If IsDate(txtDataAccettazione.Text) Then
                dvDocT.Item(0).Item("DataAccetta") = CDate(txtDataAccettazione.Text)
                Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
            Else
                dvDocT.Item(0).Item("DataAccetta") = DBNull.Value
                Session(CSTDATAACCETTA) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtDataAccettazione.BackColor = SEGNALA_KO
            End If
            If DDLTipoFatt.SelectedIndex > 0 Then
                dvDocT.Item(0).Item("Tipo_Fatturazione") = DDLDurataTipo.SelectedValue
            Else
                dvDocT.Item(0).Item("Tipo_Fatturazione") = DBNull.Value
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            '---------
            If IsDate(txtDataValidita.Text) Then
                dvDocT.Item(0).Item("Data_Validita") = CDate(txtDataValidita.Text)
            Else
                dvDocT.Item(0).Item("Data_Validita") = DBNull.Value
            End If
            dvDocT.Item(0).Item("NGG_Validita") = Int(txtNGG_Validita.Text.Trim)
            If IsDate(txtDataConsegna.Text) Then
                dvDocT.Item(0).Item("DataOraConsegna") = CDate(txtDataConsegna.Text)
            Else
                dvDocT.Item(0).Item("DataOraConsegna") = DBNull.Value
            End If
            dvDocT.Item(0).Item("NGG_Consegna") = Int(txtNGG_Consegna.Text.Trim)
            dvDocT.Item(0).Item("CIG") = txtCIG.Text.Trim
            dvDocT.Item(0).Item("CUP") = txtCUP.Text.Trim
            dvDocT.Item(0).Item("Riferimento") = txtRiferimento.Text.Trim
            'giu071211
            If IsDate(txtDataRif.Text) Then
                dvDocT.Item(0).Item("Data_Riferimento") = CDate(txtDataRif.Text)
            Else
                dvDocT.Item(0).Item("Data_Riferimento") = DBNull.Value
            End If
            dvDocT.Item(0).Item("SWTipoEvTotale") = SWNoDivTotRate 'giu200420 CBool(optTipoEvTotaleTotale.Checked)
            '
            dvDocT.Item(0).Item("SWTipoEvSaldo") = SWAccorpaRateAA 'giu200420 CKSWTipoEvSaldo
            '--------------
            If Not IsNumeric(txtPagamento.Text.Trim) Or txtPagamento.Text.Trim = "0" Then
                dvDocT.Item(0).Item("TipoContratto") = DBNull.Value
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                DDLPagamento.BackColor = SEGNALA_KO
            Else
                dvDocT.Item(0).Item("TipoContratto") = txtPagamento.Text.Trim
            End If
            dvDocT.Item(0).Item("SWTipoModSint") = CBool(chkSWTipoModSint.Checked)
            'giu050220
            Session(CSTIDPAG) = txtPagamento.Text.Trim
            txtCodCausale.AutoPostBack = False
            If GetCodCausDATipoCAIdPag(pCod_Causale, 0, 0) = False Then
                AggiornaDocT = False
                Session(CSTNONCOMPLETO) = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                DDLPagamento.BackColor = SEGNALA_KO
            End If
            txtCodCausale.Text = pCod_Causale.ToString.Trim
            txtCodCausale.AutoPostBack = True
            If Not IsNumeric(txtCodCausale.Text.Trim) Or txtCodCausale.Text.Trim = "0" Then
                dvDocT.Item(0).Item("Cod_Causale") = DBNull.Value
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                DDLPagamento.BackColor = SEGNALA_KO
            Else
                dvDocT.Item(0).Item("Cod_Causale") = txtCodCausale.Text.Trim
            End If
            '---------------
            If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
                Dim strIDAnagrProvv As String = txtCodCliForFilProvv.Text.Trim
                strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
                If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then 'giu150513
                    strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
                End If
                dvDocT.Item(0).Item("IDAnagrProvv") = strIDAnagrProvv.Trim
                dvDocT.Item(0).Item("Cod_Cliente") = DBNull.Value
                dvDocT.Item(0).Item("Cod_Fornitore") = DBNull.Value
                'GIU020420
                SplitIVA = False
                Session(CSTSPLITIVA) = SplitIVA
                '----------------
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtCodCliForFilProvv.BackColor = SEGNALA_KO
            Else
                'giu191211
                ' ''If TabCliFor = "Cli" Then
                ' ''    dvDocT.Item(0).Item("Cod_Cliente") = txtCodCliForFilProvv.Text.Trim
                ' ''    dvDocT.Item(0).Item("IDAnagrProvv") = DBNull.Value
                ' ''    dvDocT.Item(0).Item("Cod_Fornitore") = DBNull.Value
                ' ''ElseIf TabCliFor = "For" Then
                ' ''    dvDocT.Item(0).Item("Cod_Fornitore") = txtCodCliForFilProvv.Text.Trim
                ' ''    dvDocT.Item(0).Item("IDAnagrProvv") = DBNull.Value
                ' ''    dvDocT.Item(0).Item("Cod_Cliente") = DBNull.Value
                ' ''End If
                If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "1" Then
                    dvDocT.Item(0).Item("Cod_Cliente") = txtCodCliForFilProvv.Text.Trim
                    dvDocT.Item(0).Item("IDAnagrProvv") = DBNull.Value
                    dvDocT.Item(0).Item("Cod_Fornitore") = DBNull.Value
                ElseIf Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "9" Then
                    dvDocT.Item(0).Item("Cod_Fornitore") = txtCodCliForFilProvv.Text.Trim
                    dvDocT.Item(0).Item("IDAnagrProvv") = DBNull.Value
                    dvDocT.Item(0).Item("Cod_Cliente") = DBNull.Value
                    'GIU020420
                    SplitIVA = False
                    Session(CSTSPLITIVA) = SplitIVA
                    '----------------
                Else
                    dvDocT.Item(0).Item("Cod_Fornitore") = txtCodCliForFilProvv.Text.Trim
                    dvDocT.Item(0).Item("IDAnagrProvv") = DBNull.Value
                    dvDocT.Item(0).Item("Cod_Cliente") = DBNull.Value
                End If
            End If
            'giu260320
            If lblDestSel.Text.Trim <> "" Then
                Dim myCGDest As String = Session(CSTCODFILIALE)
                If Not IsNothing(myCGDest) Then
                    If String.IsNullOrEmpty(myCGDest) Then
                        myCGDest = ""
                    End If
                Else
                    myCGDest = ""
                End If
                If myCGDest.Trim = "" Then
                    myCGDest = lblDestSel.ToolTip
                End If
                If IsNumeric(myCGDest) Then
                    dvDocT.Item(0).Item("Cod_Filiale") = myCGDest.Trim 'DDLDestinazioni.SelectedValue
                Else
                    dvDocT.Item(0).Item("Cod_Filiale") = DBNull.Value
                    lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                    Session(CSTCODFILIALE) = ""
                    ' ''txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                    lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                    lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                    lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                    lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                End If
            Else
                dvDocT.Item(0).Item("Cod_Filiale") = DBNull.Value
                lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                Session(CSTCODFILIALE) = ""
                ' ''txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
            End If
            '-----------------
            dvDocT.Item(0).Item("Destinazione1") = txtDestinazione1.Text.Trim
            dvDocT.Item(0).Item("Destinazione2") = txtDestinazione2.Text.Trim
            dvDocT.Item(0).Item("Destinazione3") = txtDestinazione3.Text.Trim
            '-
            If lblABI.Text.Trim = "" Then
                dvDocT.Item(0).Item("ABI") = DBNull.Value
            Else
                dvDocT.Item(0).Item("ABI") = lblABI.Text.Trim
            End If
            If lblCAB.Text.Trim = "" Then
                dvDocT.Item(0).Item("CAB") = DBNull.Value
            Else
                dvDocT.Item(0).Item("CAB") = lblCAB.Text.Trim
            End If
            If lblIBAN.Text.Trim = "" Then
                dvDocT.Item(0).Item("IBAN") = DBNull.Value
            Else
                dvDocT.Item(0).Item("IBAN") = lblIBAN.Text.Trim
            End If
            If lblContoCorrente.Text.Trim = "" Then
                dvDocT.Item(0).Item("ContoCorrente") = DBNull.Value
            Else
                dvDocT.Item(0).Item("ContoCorrente") = lblContoCorrente.Text.Trim
            End If
            If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                dvDocT.Item(0).Item("Cod_Agente") = DBNull.Value
                Session(IDAGENTE) = ""
            Else
                dvDocT.Item(0).Item("Cod_Agente") = txtCodAgente.Text.Trim
                Session(IDAGENTE) = txtCodAgente.Text.Trim
            End If
            'giu161219
            If Not IsNumeric(txtCodRespArea.Text.Trim) Or txtCodRespArea.Text.Trim = "0" Then
                dvDocT.Item(0).Item("RespArea") = DBNull.Value
                Session(IDRESPAREA) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtCodRespArea.BackColor = SEGNALA_KO
            Else
                dvDocT.Item(0).Item("RespArea") = txtCodRespArea.Text.Trim
                Session(IDRESPAREA) = txtCodRespArea.Text.Trim
            End If
            If Not IsNumeric(txtCodRespVisite.Text.Trim) Or txtCodRespVisite.Text.Trim = "0" Then
                dvDocT.Item(0).Item("RespVisite") = DBNull.Value
                Session(IDRESPVISITE) = ""
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                txtCodRespVisite.BackColor = SEGNALA_KO
            Else
                dvDocT.Item(0).Item("RespVisite") = txtCodRespVisite.Text.Trim
                Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
            End If
            '---------

            dvDocT.Item(0).Item("ADeposito") = checkADeposito.Checked 'giu260412
            If Not IsNumeric(txtListino.Text.Trim) Or txtListino.Text.Trim = "0" Then
                dvDocT.Item(0).Item("Listino") = DBNull.Value
                Session(IDLISTINO) = ""
            Else
                dvDocT.Item(0).Item("Listino") = txtListino.Text.Trim
                Session(IDLISTINO) = txtListino.Text.Trim
            End If
            dvDocT.Item(0).Item("FatturaPA") = chkFatturaPA.Checked 'GIU070814
            '-
            dvDocT.Item(0).Item("TipoRapportoPA") = DDLTipoRapp.SelectedValue
            dvDocT.Item(0).Item("CodiceMagazzino") = ddlMagazzino.SelectedValue
            dvDocT.Item(0).Item("Acconto") = IIf(ChkAcconto.Checked, 1, 0)
            'GIU091018
            dvDocT.Item(0).Item("FatturaAC") = chkFatturaAC.Checked
            dvDocT.Item(0).Item("ScGiacenza") = chkScGiacenza.Checked
            '---------
            If lblCodValuta.Text.Trim = "" Then
                dvDocT.Item(0).Item("Cod_Valuta") = DBNull.Value
                Session(CSTVALUTADOC) = ""
                Session(CSTDECIMALIVALUTADOC) = 0
            Else
                dvDocT.Item(0).Item("Cod_Valuta") = lblCodValuta.Text.Trim
                Session(CSTVALUTADOC) = lblCodValuta.Text.Trim
            End If
            'TD3 Chiamato da ControllaDocS
            If TipoSped <> "" Then
                dvDocT.Item(0).Item("Tipo_Spedizione") = TipoSped
            Else
                dvDocT.Item(0).Item("Tipo_Spedizione") = DBNull.Value
            End If
            If Porto <> "" Then
                dvDocT.Item(0).Item("Porto") = Porto
            Else
                dvDocT.Item(0).Item("Porto") = ""
            End If
            If Vettori(0) <> 0 Then
                dvDocT.Item(0).Item("Vettore_1") = Vettori(0)
            Else
                dvDocT.Item(0).Item("Vettore_1") = DBNull.Value
            End If
            If Vettori(1) <> 0 Then
                dvDocT.Item(0).Item("Vettore_2") = Vettori(1)
            Else
                dvDocT.Item(0).Item("Vettore_2") = DBNull.Value
            End If
            If Vettori(2) <> 0 Then
                dvDocT.Item(0).Item("Vettore_3") = Vettori(2)
            Else
                dvDocT.Item(0).Item("Vettore_3") = DBNull.Value
            End If
            dvDocT.Item(0).Item("Spese_Incasso") = CDec(SpIncasso)
            dvDocT.Item(0).Item("Spese_Trasporto") = CDec(SpTrasp)
            dvDocT.Item(0).Item("Spese_Imballo") = CDec(SpImballo)
            dvDocT.Item(0).Item("SpeseVarie") = 0
            Dim RegimeIVA As String = Session(CSTREGIMEIVA)
            'giu260219 RegimeIVA = "0"
            Dim myRegIVA As String = Trim(Mid(LnkRegimeIVA.Text, 11))
            If IsNothing(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If String.IsNullOrEmpty(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
            dvDocT.Item(0).Item("Cod_Iva") = CLng(RegimeIVA)
            'Blocco IVA - Aliquota IVA
            If Iva(0) <> 0 Then
                dvDocT.Item(0).Item("Iva1") = Iva(0)
            Else
                dvDocT.Item(0).Item("Iva1") = DBNull.Value
            End If
            If Iva(1) <> 0 Then
                dvDocT.Item(0).Item("Iva2") = Iva(1)
            Else
                dvDocT.Item(0).Item("Iva2") = DBNull.Value
            End If
            If Iva(2) <> 0 Then
                dvDocT.Item(0).Item("Iva3") = Iva(2)
            Else
                dvDocT.Item(0).Item("Iva3") = DBNull.Value
            End If
            If Iva(3) <> 0 Then
                dvDocT.Item(0).Item("Iva4") = Iva(3)
            Else
                dvDocT.Item(0).Item("Iva4") = DBNull.Value
            End If
            'Blocco IVA - Imponibile
            If Imponibile(0) <> 0 Then
                dvDocT.Item(0).Item("Imponibile1") = CDec(Imponibile(0))
            Else
                dvDocT.Item(0).Item("Imponibile1") = 0
            End If
            If Imponibile(1) <> 0 Then
                dvDocT.Item(0).Item("Imponibile2") = CDec(Imponibile(1))
            Else
                dvDocT.Item(0).Item("Imponibile2") = 0
            End If
            If Imponibile(2) <> 0 Then
                dvDocT.Item(0).Item("Imponibile3") = CDec(Imponibile(2))
            Else
                dvDocT.Item(0).Item("Imponibile3") = 0
            End If
            If Imponibile(3) <> 0 Then
                dvDocT.Item(0).Item("Imponibile4") = CDec(Imponibile(3))
            Else
                dvDocT.Item(0).Item("Imponibile4") = 0
            End If
            'Blocco IVA - Imposta
            If Imposta(0) <> 0 Then
                dvDocT.Item(0).Item("Imposta1") = CDec(Imposta(0))
            Else
                dvDocT.Item(0).Item("Imposta1") = 0
            End If
            If Imposta(1) <> 0 Then
                dvDocT.Item(0).Item("Imposta2") = CDec(Imposta(1))
            Else
                dvDocT.Item(0).Item("Imposta2") = 0
            End If
            If Imposta(2) <> 0 Then
                dvDocT.Item(0).Item("Imposta3") = CDec(Imposta(2))
            Else
                dvDocT.Item(0).Item("Imposta3") = 0
            End If
            If Imposta(3) <> 0 Then
                dvDocT.Item(0).Item("Imposta4") = CDec(Imposta(3))
            Else
                dvDocT.Item(0).Item("Imposta4") = 0
            End If
            dvDocT.Item(0).Item("Totale") = CDec(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4) + Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4))
            dvDocT.Item(0).Item("TotaleM") = CDec(TotMerceLordo) 'giu050319 CDec(TotMerce)
            dvDocT.Item(0).Item("Abbuono") = CDec(Abbuono)
            dvDocT.Item(0).Item("Sconto_Cassa") = CDec(ScCassa)
            dvDocT.Item(0).Item("ScontoReale") = 0 'per adessso sempre 0
            dvDocT.Item(0).Item("Aspetto") = Aspetto
            dvDocT.Item(0).Item("Peso") = CDec(Peso)
            dvDocT.Item(0).Item("Colli") = Colli
            dvDocT.Item(0).Item("Pezzi") = Pezzi
            'GIU060220 POI LI VALORIZZO DALLA SESSIONE CHE MEMORIZZO SOLO SULLA SCADENZA 1
            'Blocco Data Scadenza
            ' ''If DataScad(0) <> "" Then
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_1") = CDate(DataScad(0))
            ' ''Else
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_1") = DBNull.Value
            ' ''End If
            ' ''If DataScad(1) <> "" Then
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_2") = CDate(DataScad(1))
            ' ''Else
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_2") = DBNull.Value
            ' ''End If
            ' ''If DataScad(2) <> "" Then
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_3") = CDate(DataScad(2))
            ' ''Else
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_3") = DBNull.Value
            ' ''End If
            ' ''If DataScad(3) <> "" Then
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_4") = CDate(DataScad(3))
            ' ''Else
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_4") = DBNull.Value
            ' ''End If
            ' ''If DataScad(4) <> "" Then
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_5") = CDate(DataScad(4))
            ' ''Else
            ' ''    dvDocT.Item(0).Item("Data_Scadenza_5") = DBNull.Value
            ' ''End If
            '' ''Blocco Scadenze RATE
            ' ''dvDocT.Item(0).Item("Rata_1") = CDec(ImpRata(0))
            ' ''dvDocT.Item(0).Item("Rata_2") = CDec(ImpRata(1))
            ' ''dvDocT.Item(0).Item("Rata_3") = CDec(ImpRata(2))
            ' ''dvDocT.Item(0).Item("Rata_4") = CDec(ImpRata(3))
            ' ''dvDocT.Item(0).Item("Rata_5") = CDec(ImpRata(4))
            'giu020220 VALORIZZO DALLA SESSIONE CHE MEMORIZZO SOLO SULLA SCADENZA 1
            dvDocT.Item(0).Item("Data_Scadenza_1") = DBNull.Value
            dvDocT.Item(0).Item("Data_Scadenza_2") = DBNull.Value
            dvDocT.Item(0).Item("Data_Scadenza_3") = DBNull.Value
            dvDocT.Item(0).Item("Data_Scadenza_4") = DBNull.Value
            dvDocT.Item(0).Item("Data_Scadenza_5") = DBNull.Value
            dvDocT.Item(0).Item("Rata_1") = 0
            dvDocT.Item(0).Item("Rata_2") = 0
            dvDocT.Item(0).Item("Rata_3") = 0
            dvDocT.Item(0).Item("Rata_4") = 0
            dvDocT.Item(0).Item("Rata_5") = 0
            '-
            Dim NScad As Integer = 0 'giu040520 memorizzo le 5 rate non ancora evase per le statistiche
            Dim myScadPagCA As String = ""
            If Not (Session(CSTSCADPAGCA) Is Nothing) Then
                Dim ArrScadPagCA As ArrayList
                ArrScadPagCA = Session(CSTSCADPAGCA)
                Dim rowScadPagCa As ScadPagCAEntity = Nothing
                Dim Cont As Integer = 0
                For i = 0 To ArrScadPagCA.Count - 1
                    If myScadPagCA.Trim <> "" Then myScadPagCA += ";"
                    rowScadPagCa = ArrScadPagCA(i)
                    If rowScadPagCa.Evasa = False And rowScadPagCa.Data.ToString.Trim <> "" Then
                        NScad += 1
                        If NScad < 6 Then
                            dvDocT.Item(0).Item("Data_Scadenza_" & Format(NScad, "#0") & "") = CDate(rowScadPagCa.Data.ToString.Trim)
                            dvDocT.Item(0).Item("Rata_" & Format(NScad, "#0") & "") = CDec(rowScadPagCa.Importo.ToString.Trim)
                        End If
                    End If
                    myScadPagCA += rowScadPagCa.NRata.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.Data.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.Importo.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.Evasa.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.NFC.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.DataFC.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.Serie.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.ImportoF.ToString.Trim & ";" 'giu191223
                    myScadPagCA += rowScadPagCa.ImportoR.ToString.Trim
                Next
            ElseIf TotNettoPagare = 0 And dvDocT.Item(0).Item("Totale") = 0 Then 'giu281023
                Session(CSTNONCOMPLETO) = SWNO
                myNonCompleto = SWNO
                dvDocT.Item(0).Item("StatoDoc") = 0
                Session(CSTSTATODOC) = "0"
                If myScadPagCA.Trim <> "" Then myScadPagCA += ";"
                NScad += 1
                If NScad < 6 Then
                    dvDocT.Item(0).Item("Data_Scadenza_" & Format(NScad, "#0") & "") = CDate(txtDataInizio.Text.Trim)
                    dvDocT.Item(0).Item("Rata_" & Format(NScad, "#0") & "") = 0
                End If
                myScadPagCA += "1;"
                myScadPagCA += txtDataInizio.Text.Trim & ";"
                myScadPagCA += "0;"
                myScadPagCA += "False;"
                myScadPagCA += ";"
                myScadPagCA += ";"
                myScadPagCA += ";"
                myScadPagCA += "0;" 'giu191223
                myScadPagCA += "0" 'giu191223
            Else
                Session(CSTNONCOMPLETO) = SWSI
                myNonCompleto = SWSI
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
                lblMessDoc.Text = "DOCUMENTO NON COMPLETO<br>Verificare SCADENZE RATE FATTURAZIONE." : lblMessDoc.Visible = True
                ' '' NON BLOCCO PROSEGUO SWErr = True
                'giu280320
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Attenzione", "Documento non completo; Verificare dettagli e scadenze.", WUC_ModalPopup.TYPE_ERROR)
                '--------------
                ' ''strErrore = "Attenzione, nessun RATA DI SCADENZA."
            End If
            dvDocT.Item(0).Item("ScadPagCA") = myScadPagCA.Trim
            dvDocT.Item(0).Item("ScadAttCA") = ""
            '---------
            'giu041123 dvDocT.Item(0).Item("DataOraRitiro") = DataOraRitiro
            dvDocT.Item(0).Item("Descrizione_Imballo") = DesImballo
            dvDocT.Item(0).Item("DescrizionePorto") = DesPorto
            '-----------------------------
            If txtTipoFatt.Text.Trim = "" Then
                dvDocT.Item(0).Item("Tipo_Fatturazione") = DBNull.Value
            Else
                dvDocT.Item(0).Item("Tipo_Fatturazione") = txtTipoFatt.Text.Trim
            End If
            dvDocT.Item(0).Item("DesRefInt") = txtDesRefInt.Text.Trim
            dvDocT.Item(0).Item("NoteDocumento") = txtNoteDocumento.Text.Trim
            dvDocT.Item(0).Item("NoteRitiro") = NoteIntervento 'giu041123 
            dvDocT.Item(0).Item("ModificatoDa") = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            'giu120118
            dvDocT.Item(0).Item("SplitIVA") = SplitIVA
            dvDocT.Item(0).Item("RitAcconto") = RitAcconto
            dvDocT.Item(0).Item("ImponibileRA") = ImponibileRA
            dvDocT.Item(0).Item("PercRA") = PercRA
            dvDocT.Item(0).Item("TotaleRA") = TotaleRA
            dvDocT.Item(0).Item("TotNettoPagare") = TotNettoPagare
            'giu270220
            If btnAggiorna.BackColor = SEGNALA_KO And myNonCompleto = SWNO Then
                Session(CSTNONCOMPLETO) = SWNO
            End If
            If Session(CSTNONCOMPLETO) = SWSI Then
                dvDocT.Item(0).Item("StatoDoc") = 5
                Session(CSTSTATODOC) = "5"
            ElseIf Session(CSTNONCOMPLETO) = SWNO Then
                dvDocT.Item(0).Item("StatoDoc") = 0
                Session(CSTSTATODOC) = "0"
            End If
            Session(CSTSTATODOC) = dvDocT.Item(0).Item("StatoDoc").ToString
            '--------
            'giu260219
            dvDocT.Item(0).Item("Bollo") = Bollo
            dvDocT.Item(0).Item("BolloACaricoDel") = IIf(Bollo <> 0, BolloACaricoDel, "")
            DsDocT = Session("DsDocT")
        End If
        'giu280320 stato documento per errori
        'giu270220
        If Session(CSTNONCOMPLETO) = SWSI Or Session(CSTSTATODOC) = "5" Then
            If lblMessDoc.Text.Trim = "" Then
                lblMessDoc.Text = "Attenzione DOCUMENTO NON COMPLETO per ERRORI<br>" : lblMessDoc.Visible = True
            End If
        ElseIf Session(CSTNONCOMPLETO) <> SWSI And Session(CSTSTATODOC) <> "5" Then
            lblMessDoc.Text = "" : lblMessDoc.Visible = False
        End If
        '-----------------------------------------------------
        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsDocT.ContrattiT)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsDocT.ContrattiT)
            End If
            If dvDocT.Count > 0 Then
                Session(IDDOCUMENTI) = dvDocT.Item(0).Item("IDDocumenti")
            Else
                Session(IDDOCUMENTI) = ""
                SWErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiornamento testata", "Nessun dato è stato salavato", WUC_ModalPopup.TYPE_ERROR)
                strErrore = "Errore aggiornamento testata. Nessun dato è stato salavato"
            End If
            Session("dvDocT") = dvDocT
            Session("DsDocT") = DsDocT
        Catch ExSQL As SqlException
            Session(ERROREALL) = SWSI
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL in Contratti.AggiornaDocT. ", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore SQL in Contratti.AggiornaDocT. " & ExSQL.Message
        Catch Ex As Exception
            Session(ERROREALL) = SWSI
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.AggiornaDocT. ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore SQL in Contratti.AggiornaDocT. " & Ex.Message
        End Try
        If SWErr = True Then
            Session("ErrSQL") = SWSI
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            AggiornaDocT = False
            lblMessDoc.Text = "ERRORE: Controllare le APPARECCHIATURE <br> che siano complete e riprovare a generare i PERIODI.<br> Se l'errore persiste <br>contattare l'amministratore di sistema"
            lblMessDoc.Visible = True
            WUC_ContrattiDett1.btnDelPeriodiAttVisibile(True)
        Else
            Session(SWOP) = SWOPNESSUNA
            'giu051123
            If Session(CSTNONCOMPLETO) = SWSI Or Session(CSTSTATODOC) = "5" Then
                'nulla
            ElseIf Session(CSTNONCOMPLETO) <> SWSI And Session(CSTSTATODOC) <> "5" Then
                'giu071223 spostato in OKAggiornaDOC
                'ok controllo Date scadenze attivita
                '''Dim strSegnalaDateDiverse As String = ""
                '''WUC_ContrattiSpeseTraspTot1.VerificaDateCKCons(strSegnalaDateDiverse)
                '''If strSegnalaDateDiverse.Trim <> "" Then
                '''    Session("TabDoc") = TB3
                '''    TabContainer1.ActiveTabIndex = TB3
                '''End If
            End If
        End If
    End Function
    Public Function AggiornaDocD(ByRef DSDettBack As DSDocumenti) As Boolean
        Dim strErrore As String = ""
        Dim myTipoDoc As String = "" : Dim myTabCliFor As String = ""
        If CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.AggiornaDocD", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            AggiornaDocD = False
            Exit Function
        End If
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = "1"
        If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
        'giu170412 dal codice causale determino quale prezzo è il prezzo di Listino (Acquisto o Vendita)
        'C/DEPOSITO
        Dim SWPrezzoAL As String = "L" 'Listino
        If CKPrezzoALCSG(SWPrezzoAL, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.AggiornaDocD", strErrore, WUC_ModalPopup.TYPE_ALERT)
            AggiornaDocD = False
            Exit Function
        End If
        '-------------------------------------------------------+---------------------------------------
        Dim myID As String = Session(IDDOCUMENTI)
        Try
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.AggiornaDocD", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            End If
            'giu050220
            Dim myIDDurataNum As String = Session(IDDURATANUM)
            If IsNothing(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            '-
            Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
            If IsNothing(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            '-----------
            SqlAdapDocForInsert = Session("SqlAdapDocForInsert")
            SqlDbInserCmdForInsert = Session("SqlDbInserCmdForInsert")
            Dim DsDocDettTmp As New DSDocumenti
            Dim RowDett As DSDocumenti.ContrattiDRow
            Dim RowDettForIns As DSDocumenti.ContrattiDForInsertRow
            Dim RowDettBKIns As DSDocumenti.ContrattiDRow 'giupier 231211
            DsDocDettTmp = Session("aDsDett")
            DsDocDettForInsert = New DSDocumenti
            'giu021111
            Dim dc As DataColumn
            '---------
            'GIU280312
            Dim SWOKAge As Boolean = False
            'giu140412 
            Dim myPrezzoAL As Decimal = 0
            Dim _myPrezzoListino As Decimal = 0 : Dim _myPrezzoAcquisto As Decimal = 0
            Dim _mySconto1 As Decimal = 0 : Dim _mySconto2 As Decimal = 0
            '------------------------------------------------------------------------------
            Dim myProvvAg As Decimal = 0
            Dim MySuperatoSconto As Boolean = False
            Dim myCodArt As String = "" 'giu270814
            'giu151017 aggiornamento aliquota IVA
            Dim RegimeIVA As String = Session(CSTREGIMEIVA)
            'giu260219 RegimeIVA = "0"
            Dim myRegIVA As String = Trim(Mid(LnkRegimeIVA.Text, 11))
            If IsNothing(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If String.IsNullOrEmpty(RegimeIVA) Then
                If IsNumeric(myRegIVA) Then
                    RegimeIVA = myRegIVA
                Else
                    RegimeIVA = "0"
                End If
            End If
            If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
            Session(CSTREGIMEIVA) = RegimeIVA
            Dim _Cod_Iva As Integer = 0
            '------------------------------------
            'giu220220 Serie/Lotto/Dest per DurataNum=0 APP
            Dim mySerie As String = "" : Dim myLotto As String = "" : Dim myCodDest As Integer = -1
            Dim myRespAreaApp As Integer = -1 : Dim myRespVisiteApp As Integer = -1 'giu231023
            'giu280420
            Dim Modello As Integer = -1
            Dim TipoDett As Integer = WUC_ContrattiDett1.GetDDLModello(Modello)
            '--------------
            'giu300312 ricalcolo ScontoReale 
            For Each RowDett In DsDocDettTmp.ContrattiD.Select("", "DurataNum,DurataNumRiga,Riga")
                If RowDett.RowState <> DataRowState.Deleted Then
                    RowDett.BeginEdit()

                    If myIDDurataNum = "0" Then
                        RowDett.Item("QtaDurataNumR0") = Modello
                        If mySerie = "" Then
                            If Not IsDBNull(RowDett.Item("Serie")) Then
                                mySerie = Formatta.FormattaNomeFile(RowDett.Item("Serie"))
                            End If
                        ElseIf IsDBNull(RowDett.Item("Serie")) Then
                            RowDett.Item("Serie") = mySerie
                        ElseIf RowDett.Item("Serie") = "" Then
                            RowDett.Item("Serie") = mySerie
                        ElseIf RowDett.Item("Serie") <> mySerie Then
                            RowDett.Item("Serie") = mySerie
                        End If
                        If myLotto = "" Then
                            If Not IsDBNull(RowDett.Item("Lotto")) Then
                                myLotto = Formatta.FormattaNomeFile(RowDett.Item("Lotto"))
                            End If
                        ElseIf IsDBNull(RowDett.Item("Lotto")) Then
                            RowDett.Item("Lotto") = myLotto
                        ElseIf RowDett.Item("Lotto") = "" Then
                            RowDett.Item("Lotto") = myLotto
                        ElseIf RowDett.Item("Lotto") <> myLotto Then
                            RowDett.Item("Lotto") = myLotto
                        End If
                        If myCodDest = -1 Then
                            If Not IsDBNull(RowDett.Item("Cod_Filiale")) Then
                                myCodDest = RowDett.Item("Cod_Filiale")
                            End If
                        ElseIf IsDBNull(RowDett.Item("Cod_Filiale")) Then
                            RowDett.Item("Cod_Filiale") = myCodDest
                        ElseIf RowDett.Item("Cod_Filiale") < 1 Then
                            RowDett.Item("Cod_Filiale") = myCodDest
                        ElseIf RowDett.Item("Cod_Filiale") <> myCodDest Then
                            RowDett.Item("Cod_Filiale") = myCodDest
                        End If
                        'giu231023
                        If myRespAreaApp = -1 Then
                            If Not IsDBNull(RowDett.Item("RespArea")) Then
                                myRespAreaApp = RowDett.Item("RespArea")
                            End If
                        ElseIf IsDBNull(RowDett.Item("RespArea")) Then
                            RowDett.Item("RespArea") = myRespAreaApp
                        ElseIf RowDett.Item("RespArea") < 1 Then
                            RowDett.Item("RespArea") = myRespAreaApp
                        ElseIf RowDett.Item("RespArea") <> myRespAreaApp Then
                            RowDett.Item("RespArea") = myRespAreaApp
                        End If
                        '-
                        If myRespVisiteApp = -1 Then
                            If Not IsDBNull(RowDett.Item("RespVisite")) Then
                                myRespVisiteApp = RowDett.Item("RespVisite")
                            End If
                        ElseIf IsDBNull(RowDett.Item("RespVisite")) Then
                            RowDett.Item("RespVisite") = myRespVisiteApp
                        ElseIf RowDett.Item("RespVisite") < 1 Then
                            RowDett.Item("RespVisite") = myRespVisiteApp
                        ElseIf RowDett.Item("RespVisite") <> myRespVisiteApp Then
                            RowDett.Item("RespVisite") = myRespVisiteApp
                        End If
                    End If
                    '-
                    RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                    RowDettBKIns = DSDettBack.ContrattiD.NewRow 'giupier 231211
                    'GIU280312 giu290312
                    SWOKAge = False
                    myPrezzoAL = 0
                    _myPrezzoAcquisto = 0 : _myPrezzoListino = 0
                    _mySconto1 = 0 : _mySconto2 = 0 'non servono qui ma sono parametri
                    _Cod_Iva = 0
                    'giu270814
                    myCodArt = ""
                    If IsDBNull(RowDett.Item("Cod_Articolo")) Then
                        'nulla
                    ElseIf RowDett.Item("Cod_Articolo") = "" Then
                        'nulla
                    Else
                        myCodArt = RowDett.Item("Cod_Articolo").ToString.Trim
                    End If
                    '---------
                    If myCodArt.Trim = "" Then 'giu270814
                        'nulla
                        SWOKAge = False
                    Else
                        SWOKAge = True
                        If Documenti.GetPrezziListinoAcquisto(myTipoDoc, IDLT, myCodArt, _
                                                             _myPrezzoListino, _myPrezzoAcquisto, _
                                                             _mySconto1, _mySconto2, _Cod_Iva, strErrore) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore in Contratti.AggiornaDocD.GetPrezziListinoAcquisto", strErrore, WUC_ModalPopup.TYPE_ALERT)
                            ' ''AggiornaDocD = False
                            ' ''Exit Function
                        ElseIf strErrore.Trim <> "" Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore in Contratti.AggiornaDocD.GetPrezziListinoAcquisto", strErrore, WUC_ModalPopup.TYPE_ALERT)
                            ' ''AggiornaDocD = False
                            ' ''Exit Function
                        End If
                        'GIU170412
                        If SWPrezzoAL = "A" Then 'GIU170412
                            myPrezzoAL = _myPrezzoAcquisto
                            _mySconto1 = 0 : _mySconto2 = 0
                        Else
                            myPrezzoAL = _myPrezzoListino
                        End If
                        '-----------
                        'giu151017
                        If Val(RegimeIVA) > 49 Then
                            RowDett.Item("Cod_Iva") = Val(RegimeIVA)
                        ElseIf IsDBNull(RowDett.Item("Cod_Iva")) Then
                            RowDett.Item("Cod_Iva") = _Cod_Iva
                            'giu270520 NON DEVO CAMBIARE L'IVA SE VENISSE INSERITA UNA IVA SPECIFICA
                            ' ''ElseIf RowDett.Item("Cod_Iva") > 49 Then
                            ' ''    RowDett.Item("Cod_Iva") = _Cod_Iva
                        End If
                    End If
                    RowDett.Item("PrezzoAcquisto") = _myPrezzoAcquisto
                    RowDett.Item("PrezzoListino") = _myPrezzoListino
                    '--
                    If IsDBNull(RowDett.Item("Qta_Evasa")) Then
                        'nulla
                    ElseIf RowDett.Item("Qta_Evasa") = 0 Then
                        'nulla
                    Else
                        SWOKAge = True
                    End If
                    '---
                    If IsDBNull(RowDett.Item("Qta_Ordinata")) Then
                        'nulla
                    ElseIf RowDett.Item("Qta_Ordinata") = 0 Then
                        'nulla
                    Else
                        SWOKAge = True
                    End If
                    '---
                    If IsDBNull(RowDett.Item("Prezzo")) Then
                        RowDett.Item("Prezzo") = 0
                    ElseIf RowDett.Item("Prezzo") = 0 Then
                        'nulla
                    Else
                        SWOKAge = True
                    End If
                    '---
                    If IsDBNull(RowDett.Item("Prezzo_Netto")) Then
                        RowDett.Item("Prezzo_Netto") = 0
                    ElseIf RowDett.Item("Prezzo_Netto") = 0 Then
                        'nulla
                    Else
                        SWOKAge = True
                    End If
                    '---
                    'giu300312 ricalcolo ScontoReale
                    If RowDett.Item("Prezzo") > 0 Then
                        'giu160112
                        If myPrezzoAL = 0 Then myPrezzoAL = RowDett.Item("Prezzo") 'giu230412
                        If RowDett.Item("Prezzo") = myPrezzoAL Then
                            RowDett.Item("ScontoReale") = _
                              Math.Round(RowDett.Item("Prezzo_Netto") * 100 / RowDett.Item("Prezzo"), 4)
                            RowDett.Item("ScontoReale") = 100 - RowDett.Item("ScontoReale")
                        Else
                            RowDett.Item("ScontoReale") = _
                                Math.Round(RowDett.Item("Prezzo_Netto") * 100 / myPrezzoAL, 4)
                            RowDett.Item("ScontoReale") = 100 - RowDett.Item("ScontoReale")
                        End If
                    Else
                        If myPrezzoAL = 0 Then
                            RowDett.Item("ScontoReale") = 0
                        Else
                            RowDett.Item("ScontoReale") = 100
                        End If
                    End If
                    'adesso prendo la scaletta ......
                    If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                        SWOKAge = False
                    End If
                    If SWOKAge = False Then
                        RowDett.Item("Pro_Agente") = 0
                        RowDett.Item("ImportoProvvigione") = 0
                    Else
                        MySuperatoSconto = False
                        If Documenti.Calcola_ProvvAgente(RowDett.Item("Cod_Articolo"), _
                            CLng(txtCodAgente.Text.Trim), myProvvAg, _
                            RowDett.Item("ScontoReale"), _
                            MySuperatoSconto, "") = False Then
                            RowDett.Item("Pro_Agente") = 0
                            RowDett.Item("ImportoProvvigione") = 0
                        Else
                            RowDett.Item("Pro_Agente") = myProvvAg
                            RowDett.Item("ImportoProvvigione") = RowDett.Item("Importo") * myProvvAg / 100
                        End If
                        ' ''If MySuperatoSconto = True Then
                        ' ''    lblSuperatoScMax.BorderStyle = BorderStyle.Outset
                        ' ''    lblSuperatoScMax.Text = "Superato Sconto"
                        ' ''End If
                        '-----------------------------
                    End If
                    'GIU040319
                    Dim myQuantita As Decimal = 0 : Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0
                    If IsDBNull(RowDett.Item("Qta_Evasa")) Then
                        'nulla
                    Else
                        myQtaE = RowDett.Item("Qta_Evasa")
                    End If
                    '---
                    If IsDBNull(RowDett.Item("Qta_Ordinata")) Then
                        'nulla
                    Else
                        myQtaO = RowDett.Item("Qta_Ordinata")
                    End If
                    Select Case Left(myTipoDoc, 1)
                        Case "O"
                            myQuantita = myQtaO
                        Case Else
                            If myTipoDoc = "PR" Or myTipoDoc = "PF" Or myTipoDoc = "TC" Or myTipoDoc = "CA" Then 'GIU020212 GIU021219
                                myQuantita = myQtaO
                            Else
                                myQuantita = myQtaE
                            End If
                    End Select
                    If RowDett.IsTipoScontoMerceNull Then
                        RowDett.OmaggioImponibile = False
                        RowDett.OmaggioImposta = False
                        RowDett.Sconto_Merce = 0
                    ElseIf RowDett.TipoScontoMerce.Trim = "NO" Then
                        RowDett.OmaggioImponibile = False
                        RowDett.OmaggioImposta = False
                        RowDett.Sconto_Merce = 0
                    ElseIf RowDett.TipoScontoMerce.Trim = "OM" Then
                        RowDett.OmaggioImponibile = True
                        RowDett.OmaggioImposta = False
                        RowDett.Sconto_Merce = myQuantita
                    ElseIf RowDett.TipoScontoMerce.Trim = "SM" Then
                        RowDett.OmaggioImponibile = True
                        RowDett.OmaggioImposta = True
                        RowDett.Sconto_Merce = myQuantita
                    Else
                        RowDett.OmaggioImponibile = False
                        RowDett.OmaggioImposta = False
                        RowDett.Sconto_Merce = 0
                    End If
                    '---------
                    RowDett.EndEdit()
                    'giu021111
                    For Each dc In DsDocDettTmp.ContrattiD.Columns
                        If UCase(dc.ColumnName) = "NEXTDATASC" Then
                            Continue For
                        End If
                        If UCase(dc.ColumnName) = "TEXTDATASC" Then
                            Continue For
                        End If
                        If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                            Continue For
                        End If
                        If UCase(dc.ColumnName) = "TEXTREFDATANC" Then 'GIU310123
                            Continue For
                        End If
                        If UCase(dc.ColumnName) = "SERIELOTTO" Then
                            Continue For
                        End If
                        If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                            If SWOKAge = True Then
                                If UCase(dc.ColumnName) = "COD_AGENTE" Then
                                    If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                                        RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                                        RowDettBKIns.Item(dc.ColumnName) = DBNull.Value
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = txtCodAgente.Text.Trim
                                        RowDettBKIns.Item(dc.ColumnName) = txtCodAgente.Text.Trim
                                    End If
                                ElseIf UCase(dc.ColumnName) = "PRO_AGENTE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                    RowDettBKIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "IMPORTOPROVVIGIONE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                    RowDettBKIns.Item(dc.ColumnName) = 0
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                                    RowDettBKIns.Item(dc.ColumnName) = DBNull.Value 'GIUPIER 280312
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                                RowDettBKIns.Item(dc.ColumnName) = DBNull.Value 'GIUPIER 280312
                            End If
                        Else
                            'GIUPIER280312
                            If SWOKAge = True Then
                                If UCase(dc.ColumnName) = "COD_AGENTE" Then
                                    If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                                        RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                                        RowDettBKIns.Item(dc.ColumnName) = DBNull.Value
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = txtCodAgente.Text.Trim
                                        RowDettBKIns.Item(dc.ColumnName) = txtCodAgente.Text.Trim
                                    End If
                                ElseIf UCase(dc.ColumnName) = "PRO_AGENTE" Then
                                    If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                                        RowDettForIns.Item(dc.ColumnName) = 0
                                        RowDettBKIns.Item(dc.ColumnName) = 0
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                        RowDettBKIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                    End If
                                ElseIf UCase(dc.ColumnName) = "IMPORTOPROVVIGIONE" Then
                                    If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                                        RowDettForIns.Item(dc.ColumnName) = 0
                                        RowDettBKIns.Item(dc.ColumnName) = 0
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                        RowDettBKIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                    End If
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                    RowDettBKIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName) 'giupier 231211
                                End If
                            Else
                                If UCase(dc.ColumnName) = "COD_AGENTE" Then
                                    RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                                    RowDettBKIns.Item(dc.ColumnName) = DBNull.Value
                                ElseIf UCase(dc.ColumnName) = "PRO_AGENTE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                    RowDettBKIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "IMPORTOPROVVIGIONE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                    RowDettBKIns.Item(dc.ColumnName) = 0
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                    RowDettBKIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName) 'giupier 231211
                                End If
                            End If
                        End If
                    Next
                    '---------
                    DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                    DSDettBack.ContrattiD.AddContrattiDRow(RowDettBKIns) 'giupier 231211
                End If
            Next

            Dim TransTmp As SqlClient.SqlTransaction
            Dim SqlConTmp As New SqlConnection
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Try
                SqlConTmp.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
                Dim strValore As String = ""
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
                Dim sqlCmdDel As New SqlCommand
                sqlCmdDel.CommandText = "Delete From ContrattiD Where IDDocumenti=" & myID.Trim & " AND DurataNum=" & myIDDurataNum.Trim & " AND DurataNumRiga=" & myIDDurataNumR.Trim
                sqlCmdDel.Connection = SqlConTmp
                sqlCmdDel.CommandTimeout = myTimeOUT
                sqlCmdDel.CommandType = CommandType.Text

                SqlDbInserCmdForInsert.Connection = SqlConTmp
                SqlDbInserCmdForInsert.CommandTimeout = myTimeOUT
                If SqlConTmp.State <> ConnectionState.Open Then
                    SqlConTmp.Open()
                End If

                TransTmp = SqlConTmp.BeginTransaction(IsolationLevel.ReadCommitted)
                sqlCmdDel.Transaction = TransTmp
                SqlDbInserCmdForInsert.Transaction = TransTmp

                sqlCmdDel.ExecuteNonQuery()
                SqlAdapDocForInsert.Update(DsDocDettForInsert.ContrattiDForInsert)

                TransTmp.Commit()
                If SqlConTmp.State <> ConnectionState.Closed Then
                    SqlConTmp.Close()
                End If
                AggiornaDocD = True

            Catch ExSQL As SqlException
                TransTmp.Rollback()
                lblMessDoc.Text = "ERRORE: Controllare le APPARECCHIATURE <br> che siano complete e riprovare a generare i PERIODI.<br> Se l'errore persiste <br>contattare l'amministratore di sistema"
                lblMessDoc.Visible = True
                WUC_ContrattiDett1.btnDelPeriodiAttVisibile(True)
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
                '-
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento del database:" & ExSQL.Message, WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            Catch ex As Exception
                TransTmp.Rollback()
                lblMessDoc.Text = "ERRORE: Controllare le APPARECCHIATURE <br> che siano complete e riprovare a generare i PERIODI.<br> Se l'errore persiste <br>contattare l'amministratore di sistema"
                lblMessDoc.Visible = True
                WUC_ContrattiDett1.btnDelPeriodiAttVisibile(True)
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
                '-
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento del database:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            End Try
            '============================================================================
            'SE IL DOCUMENTO E' NUOVO MA ANCHE SE MODIFICO AGGIORNOLA CAUSALE IN TESTATA
            'ALTRIMENTI LA PROCEDURA DI AGGIORNAMENTO DISTINTA BASE NON FUNZIONA
            '============================================================================
            strErrore = ""
            Dim strSQL As String = ""
            Dim SWOk As Boolean = True
            strSQL = "UPDATE ContrattiT SET Cod_Causale = " & txtCodCausale.Text.Trim & " Where IDDocumenti=" & myID.Trim
            '-
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL)
                If SWOk = False Then
                    ObjDB = Nothing
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore in Contratti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento testata Cod_Causale", WUC_ModalPopup.TYPE_ALERT)
                    AggiornaDocD = False
                    Exit Function
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento testata Cod_Causale. " & Ex.Message, WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            End Try
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'GIU241023 ??? SERVE ??? DsDocDettTmp.AcceptChanges()
            '============================================================================
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.AggiornaDocD", "Si è verificato un errore durante la lettura dei dati:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            AggiornaDocD = False
        End Try
      
    End Function
    
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnNuovo_Click)")
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu261023
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        btnCollegaOC.BackColor = btnNuovo.BackColor
        btnCollegaOC.ForeColor = Drawing.Color.Black
        '---------
        Dim strErrore As String = "" : Dim myNuovoNumero As Long = 0
        Session(CSTTIPODOC) = SWTD(TD.TipoContratto)
        myNuovoNumero = GetNewCA()
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDoc(strErrore)
        If CkNumDoc = -1 Then
            Chiudi("Errore: Verifica N° Documento da impegnare. " & strErrore)
            Exit Sub
        End If
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewDoc"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewDocRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewDoc"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    'giu260312 verifica la sequenza se è completa
    Private Function CheckNumDoc(ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From ContrattiT WHERE "
        strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
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

    Public Sub CreaNewDoc()
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnNuovo_Click)")
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnNuovo_Click)")
            Exit Sub
        End If

        Session(SWOP) = SWOPNUOVO
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        '---------
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)
        Session(IDDOCUMENTI) = "-1" 'giu140512
        dvDocT = Session("dvDocT")
        DsDocT = Session("DsDocT")
        DsDocT.Clear() : DsDocT.AcceptChanges()
        Session("dvDocT") = dvDocT
        Session("DsDocT") = DsDocT

        CampiSetEnabledToT(True)
        AzzeraTxtDocT()
        WUC_ContrattiDett1.TD1ReBuildDett()
        WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2()
        Session(SWOPNUOVONUMDOC) = SWSI
        If GetNewNumDoc() = False Then Exit Sub 'giu260312

        Session("TabDoc") = TB0
        TabContainer1.ActiveTabIndex = 0
        ' ''txtRevNDoc.AutoPostBack = False 'giu300419
        ' ''txtRevNDoc.Text = "0"
        ' ''txtRevNDoc.AutoPostBack = True 'giu300419
        txtDataDoc.AutoPostBack = False
        txtDataDoc.Text = Format(Now, FormatoData)
        txtDataDoc.AutoPostBack = True
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = "0"
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
        Dim strErrore As String = "" : Dim strValore As String = ""
        Call GetDatiAbilitazioni(CSTABILAZI, "DurataTipo", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            PosizionaItemDDL(strValore.Trim, DDLDurataTipo)
        Else
            DDLDurataTipo.SelectedIndex = 0
        End If
        txtNVisite.Text = ""
        '-
        txtTipoFatt.AutoPostBack = False : DDLTipoFatt.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoFattCA", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtTipoFatt.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLTipoFatt)
        Else
            DDLTipoFatt.SelectedIndex = -1
        End If
        txtTipoFatt.AutoPostBack = True : DDLTipoFatt.AutoPostBack = True
        '-
        txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoContr", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtPagamento.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLPagamento)
        Else
            DDLPagamento.SelectedIndex = -1
        End If
        If DDLPagamento.SelectedValue.Trim = "" Then 'giu231123
            txtPagamento.Text = ""
            DDLPagamento.SelectedIndex = -1
        End If
        txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
        '-
        'GIU060220
        Dim pCodCaus As Integer = 0 : Dim pNVisite As Integer = 0 : Dim pDurataNum As Integer = 0
        If GetCodCausDATipoCAIdPag(pCodCaus, pNVisite, pDurataNum) = True Then
            strValore = pCodCaus.ToString.Trim
        Else
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
        End If
        If strValore.Trim <> "" Then
            txtCodCausale.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLCausali)
        End If
        txtNVisite.Text = pNVisite.ToString.Trim
        txtDurataNum.Text = pDurataNum.ToString.Trim
        'giu101219 Dim strErrore As String = ""
        If AggNuovaTestata(strErrore) = False Then
            Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
            Exit Sub
        End If
        BtnSetEnabledTo(False)
        WUC_ContrattiDett1.SetEnableTxt(True)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session(SWMODIFICATO) = SWNO
    End Sub
    Public Sub CreaNewDocRecuperaNum()
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnNuovo_Click)")
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------
        Session(SWOP) = SWOPNUOVO
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        '---------
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)
        Session(IDDOCUMENTI) = "-1" 'giu140512
        dvDocT = Session("dvDocT")
        DsDocT = Session("DsDocT")
        DsDocT.Clear() : DsDocT.AcceptChanges()
        Session("dvDocT") = dvDocT
        Session("DsDocT") = DsDocT

        CampiSetEnabledToT(True)
        AzzeraTxtDocT()
        WUC_ContrattiDett1.TD1ReBuildDett()
        WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2()
        'giu120412 ok GetNewNumDoc gestisce anche il recupero
        Session(SWOPNUOVONUMDOC) = SWNO
        If GetNewNumDoc() = False Then Exit Sub 'giu260312

        Session("TabDoc") = TB0
        TabContainer1.ActiveTabIndex = 0
        ' ''txtRevNDoc.AutoPostBack = False 'giu300419
        ' ''txtRevNDoc.Text = "0"
        ' ''txtRevNDoc.AutoPostBack = True 'giu300419
        txtDataDoc.AutoPostBack = False
        txtDataDoc.Text = Format(Now, FormatoData)
        txtDataDoc.AutoPostBack = True
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = "0"
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim
        'giu171219
        Dim strErrore As String = "" : Dim strValore As String = ""
        Call GetDatiAbilitazioni(CSTABILAZI, "DurataTipo", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            PosizionaItemDDL(strValore.Trim, DDLDurataTipo)
        Else
            DDLDurataTipo.SelectedIndex = 0
        End If
        txtNVisite.Text = ""
        '-
        txtTipoFatt.AutoPostBack = False : DDLTipoFatt.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoFattCA", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        txtTipoFatt.Text = strValore.Trim
        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
        txtTipoFatt.AutoPostBack = True : DDLTipoFatt.AutoPostBack = True
        '-
        txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
        Call GetDatiAbilitazioni(CSTABILAZI, "TipoContr", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtPagamento.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLPagamento)
        Else
            DDLPagamento.SelectedIndex = -1
        End If
        txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
        '-
        Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim <> "" Then
            txtCodCausale.Text = strValore.Trim
            PosizionaItemDDL(strValore.Trim, DDLCausali)
        End If
        '-
        If AggNuovaTestata(strErrore) = False Then
            Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
            Exit Sub
        End If

        BtnSetEnabledTo(False)
        WUC_ContrattiDett1.SetEnableTxt(True)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnElimina_Click)")
            Exit Sub
        End If
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        '-
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "EliminaDocumento"
        'giu020914
        Dim myStatoDoc As String = Session(CSTSTATODOC)
        If IsNothing(myStatoDoc) Then
            myStatoDoc = "0"
        End If
        If String.IsNullOrEmpty(myStatoDoc) Then
            myStatoDoc = "0"
        End If
        If TipoDoc = "" Then
            myStatoDoc = "0"
        End If
        Dim strMess As String = "Confermi l'eliminazione del documento ? <br>" & strDesTipoDocumento
        'giu261023
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        btnCollegaOC.BackColor = btnNuovo.BackColor
        btnCollegaOC.ForeColor = Drawing.Color.Black
        '---------
        ModalPopup.Show("Elimina documento", strMess, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub EliminaDocumento()
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        Session(SWOP) = SWOPELIMINA
        Dim SWErr As Boolean = False
        'giu030914 mi serve per sapere ID del doc appena cancella e cancellare a catena 
        'i movimenti correlati: DISTINTA BASE, ARTICOLI INSTALLATI ETC ETC
        Dim myIDDocCanc As String = Session(IDDOCUMENTI).ToString.Trim
        Dim SQLStr As String = "" 'giu030914 SPOSTATO QUI COSI LA POSSO USARE PIU VOLTE
        '------------------------------------------------------------------------------
        'GIU040213 CANCELLO IL CARICO DI INIZIO ANNO SE PRESENTE NELLE ABILITAZIONI
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnElimina_Click)")
            Exit Sub
        End If
        '--------------------------------------------------------------------------
        dvDocT = Session("dvDocT")
        DsDocT = Session("DsDocT")
        dvDocT.Item(0).Delete()
        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsDocT.ContrattiT)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsDocT.ContrattiT)
            End If
            If dvDocT.Count > 0 Then
                Session(IDDOCUMENTI) = dvDocT.Item(0).Item("IDDocumenti")
            Else
                Session(IDDOCUMENTI) = ""
            End If
            Session("dvDocT") = dvDocT
            Session("DsDocT") = DsDocT
        Catch ExSQL As SqlException
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL in Contratti.EliminaDocumento", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.EliminaDocumento", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        
        Session(SWOP) = SWOPNESSUNA
        Chiudi("")
    End Sub

    Private Sub VisAnagrProvv(ByVal strIDAnagrProvv As String)
        strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
        If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then 'giu150513
            strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
        End If
        If strIDAnagrProvv.Trim <> "" Then
            Dim strSQL As String = "SELECT IDAnagrProvv, ISNULL(Ragione_Sociale,'') AS Ragione_Sociale, " & _
                "ISNULL(Codice_Fiscale,'') AS Codice_Fiscale, ISNULL(Partita_IVA,'') AS Partita_IVA, " & _
                "ISNULL(Indirizzo,'') AS Indirizzo, ISNULL(Cap,'') AS Cap, ISNULL(Localita,'') AS Localita, " & _
                "ISNULL(Provincia,'') AS Provincia, ISNULL(Stato,'') AS Stato, ISNULL(Tipo,'') AS Tipo " & _
                "FROM AnagrProvv WHERE IDAnagrProvv = " & strIDAnagrProvv.Trim
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                ObjDB = Nothing
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        txtCodCliForFilProvv.ToolTip = "(Provvisoria)"
                        lblCliForFilProvv.ToolTip = "(Provvisoria)"

                        lblPICF.Text = ds.Tables(0).Rows(0).Item("Partita_IVA").ToString : lblLabelPICF.Text = "Partita IVA"
                        If lblPICF.Text.Trim = "" Then
                            lblPICF.Text = ds.Tables(0).Rows(0).Item("Codice_Fiscale").ToString : lblLabelPICF.Text = "Codice Fiscale"
                        End If
                        lblIndirizzo.Text = ds.Tables(0).Rows(0).Item("Indirizzo").ToString
                        lblLocalita.Text = ds.Tables(0).Rows(0).Item("Cap").ToString & " " & ds.Tables(0).Rows(0).Item("Localita").ToString & " " & IIf(ds.Tables(0).Rows(0).Item("Provincia").ToString.Trim <> "", "(" & ds.Tables(0).Rows(0).Item("Provincia").ToString & ")", "")
                        '-
                        Session(IDANAGRPROVV) = ds.Tables(0).Rows(0).Item("IDAnagrProvv")

                        txtCodCliForFilProvv.Text = "(" & strIDAnagrProvv.Trim & ")"
                        Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text.Trim 'GIU111012
                        lblCliForFilProvv.Text = ds.Tables(0).Rows(0).Item("Ragione_Sociale").ToString
                        txtListino.Text = "1"
                        lblCodValuta.Text = GetParamGestAzi(Session(ESERCIZIO)).Cod_Valuta
                        lblValuta()
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Non trovata anagrafica provvisoria in tabella", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovata anagrafica provvisoria in tabella", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Carico anagrafica provvisoria: " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End Try
        End If
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        CampiSetEnabledToT(True)
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        'giu261023
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        btnCollegaOC.BackColor = btnNuovo.BackColor
        btnCollegaOC.ForeColor = Drawing.Color.Black
        txtCodCliForFilProvv.Focus()
    End Sub

    Private Sub DDLPagamento_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLPagamento.SelectedIndexChanged
        txtPagamento.AutoPostBack = False 'giu191219
        txtPagamento.Text = DDLPagamento.SelectedValue
        txtPagamento.AutoPostBack = True 'giu191219
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        Session(SWMODIFICATO) = SWSI
        Dim pCod_Causale As Long = 0 : Dim pNV As Integer = 0 : Dim pDN As Integer = 0
        txtCodCausale.AutoPostBack = False
        If GetCodCausDATipoCAIdPag(pCod_Causale, pNV, pDN) = False Then
            Exit Sub
        End If
        If txtDurataNum.Text.Trim = "" Then
            txtDurataNum.Text = pDN.ToString.Trim
        End If
        If txtNVisite.Text.Trim = "" Then
            txtNVisite.Text = pNV.ToString.Trim
        End If

        txtCodCausale.Text = pCod_Causale.ToString.Trim
        DDLCausali.AutoPostBack = False
        Call PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
        DDLCausali.AutoPostBack = True
        txtCodCausale.AutoPostBack = True
        Call RicalcolaRateTotali()
    End Sub

    Private Sub DDLAgenti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLAgenti.SelectedIndexChanged
        txtCodAgente.AutoPostBack = False 'giu191219
        txtCodAgente.Text = DDLAgenti.SelectedValue
        txtCodAgente.AutoPostBack = True 'giu191219
        Session(SWMODIFICATO) = SWSI
        Session(IDAGENTE) = txtCodAgente.Text.Trim
        'giu010412
        Dim myCAge As String = txtCodAgente.Text.Trim
        If Not IsNumeric(myCAge) Then myCAge = "0"
        Dim _CAge As Integer = CtrAgente(_CAge)
        If CInt(myCAge) <> _CAge Then
            lblMessAge.ForeColor = SEGNALA_KO
            lblMessAge.Text = "Assegnato: (" & _CAge.ToString.Trim & ")"
        Else
            lblMessAge.ForeColor = Drawing.Color.Blue
            lblMessAge.Text = ""
        End If
    End Sub

    Private Sub DDLRespArea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespArea.SelectedIndexChanged
        txtCodRespArea.AutoPostBack = False
        txtCodRespArea.Text = DDLRespArea.SelectedValue
        txtCodRespArea.AutoPostBack = True
        Session(SWMODIFICATO) = SWSI
        Session(IDRESPAREA) = txtCodRespArea.Text.Trim
        txtCodRespArea.BackColor = SEGNALA_OK
        '-
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        DDLRespVisite.BackColor = SEGNALA_OK
        '-- mi riposiziono 
        DDLRespVisite.AutoPostBack = False : txtCodRespVisite.AutoPostBack = False
        txtCodRespVisite.Text = ""
        DDLRespVisite.SelectedIndex = -1
        DDLRespVisite.AutoPostBack = True : txtCodRespVisite.AutoPostBack = True
        '--
    End Sub
    Private Sub DDLRespVisite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespVisite.SelectedIndexChanged
        txtCodRespVisite.AutoPostBack = False
        txtCodRespVisite.Text = DDLRespVisite.SelectedValue
        txtCodRespVisite.AutoPostBack = True
        Session(SWMODIFICATO) = SWSI
        Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim
        txtCodRespVisite.BackColor = SEGNALA_OK
        DDLRespVisite.BackColor = SEGNALA_OK
    End Sub

    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        txtCodCausale.AutoPostBack = False 'giu191219
        txtCodCausale.Text = DDLCausali.SelectedValue
        txtCodCausale.AutoPostBack = True 'giu191219
        Session(SWMODIFICATO) = SWSI
        WUC_ContrattiDett1.SetSWPrezzoALCSG() 'GIU190412
    End Sub

    Private Sub DDLListini_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLListini.SelectedIndexChanged
        txtListino.AutoPostBack = False 'giu191219
        txtListino.Text = DDLListini.SelectedValue
        txtListino.AutoPostBack = True
        Session(IDLISTINO) = txtListino.Text.Trim
        lblCodDesValutaByListino()
        Session(SWMODIFICATO) = SWSI
    End Sub

    Public Function RicalcolaRateTotali(Optional ByVal OKDataFine As Boolean = False) As Boolean
        'giu300120 calcolo delle rate
        btnbtnGeneraAttDNumColorRED(True)
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTDATAINIZIO) = txtDataInizio.Text.Trim
        Session(CSTDATAFINE) = txtDataFine.Text.Trim
        Session(CSTDATAACCETTA) = txtDataAccettazione.Text.Trim

        Session(CSTDURATANUM) = txtDurataNum.Text.Trim
        Session(CSTDURATATIPO) = DDLDurataTipo.SelectedValue
        Session(CSTTIPOFATT) = DDLTipoFatt.SelectedValue
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        Dim CodPag As String = Session(CSTIDPAG)
        If IsNothing(CodPag) Then CodPag = ""
        If String.IsNullOrEmpty(CodPag) Then CodPag = ""
        If Not IsNumeric(CodPag) Then CodPag = ""
        If CodPag = "" Or CodPag = "0" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice TipoContratto non valido.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim NumRate As Integer = 0 : Dim NumGG As Integer = 0 : Dim strErrore As String = ""
        'giu290320 nuovo algoritmo per la determinazione NumRate
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            TipoDoc = SWTD(TD.ContrattoAssistenza)
        End If
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        strSQL = "Select * From TipoContratto WHERE Codice = " & CodPag.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura TipoContratto: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        'giu230320 TipoPagamento=4 Pagamento alla scadenza evasione attività (VER DAE O ALTRO QUELLO DEFINITO IN TABELLA CodVisita)
        Dim DecimaliValuta As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliValuta) Then DecimaliValuta = ""
        If String.IsNullOrEmpty(DecimaliValuta) Then DecimaliValuta = ""
        If DecimaliValuta = "" Or Not IsNumeric(DecimaliValuta) Then
            DecimaliValuta = "2" 'Euro
        End If
        If rowPag(0).Item("TipoPagamento") = 4 Then
            Try
                NumRate = 0
                If (dvDocT Is Nothing) Then
                    DsDocT = Session("DsDocT")
                    dvDocT = New DataView(DsDocT.ContrattiT)
                End If
                If dvDocT.Count = 0 Then
                    ' ''AzzeraTxtDocT()
                    ' ''lblMessDoc.Text = "DOCUMENTO NON COMPLETO" : lblMessDoc.Visible = True
                    ' ''Exit Function
                Else
                    If IsDBNull(dvDocT.Item(0).Item("ScadPagCA")) Then
                        dvDocT.Item(0).Item("ScadPagCA") = ""
                    End If
                    'GIU191223
                    Dim lineaSplit As String() = dvDocT.Item(0).Item("ScadPagCA").Split(";")
                    NumRate = (lineaSplit.Count) / 8
                    '''If dvDocT.Item(0).Item("ScadPagCA") <> "" Then
                    '''    Dim lineaSplit As String() = dvDocT.Item(0).Item("ScadPagCA").Split(";")
                    '''    For i = 0 To lineaSplit.Count - 1
                    '''        If lineaSplit(i).Trim <> "" And (i + 8) <= lineaSplit.Count - 1 Then 'giu191223 da i + 6
                    '''            i += 8 'giu191223 da i + 6
                    '''            NumRate += 1
                    '''        End If
                    '''    Next
                    '''End If
                End If
                lblNumRate.Text = "Rate di fatturazione: " & FormattaNumero(NumRate)
            Catch ex As Exception
                lblNumRate.Text = "Err.: " + ex.Message.Trim
            End Try
            
        Else
            If WUC_ContrattiSpeseTraspTot1.CalcolaNumRate(NumRate, NumGG, strErrore) = False Then
                If strErrore.Trim <> "" Then
                    lblNumRate.Text = strErrore
                Else
                    lblNumRate.Text = "ERRORE CalcolaNumRate"
                End If
            Else
                lblNumRate.Text = "Rate di fatturazione: " & FormattaNumero(NumRate)
            End If
        End If
        '-----------------------------------------------------------------------------------------------------------------------------------        '@@@@@@@@@@@@@@@@@@@@@@@@@@@
        txtDurataNum.BackColor = SEGNALA_OK
        txtDurataNum.ToolTip = ""
        'GIU091120 giu030420 
        If txtDataInizio.Text.Trim = "" And IsDate(txtDataAccettazione.Text.Trim) Then
            txtDataInizio.Text = "01/01/" + CDate(txtDataAccettazione.Text).Year.ToString.Trim
        ElseIf txtDataInizio.Text.Trim = "" And IsDate(txtDataDoc.Text.Trim) Then
            txtDataInizio.Text = "01/01/" + CDate(txtDataDoc.Text).Year.ToString.Trim
        ElseIf txtDataInizio.Text.Trim = "" Then
            txtDataInizio.Text = "01/01/" + CDate(Now).Year.ToString.Trim
        End If
        '-
        If txtDataFine.Text.Trim = "" Or OKDataFine = True Then
            If DDLDurataTipo.SelectedValue.Trim = "A" Then
                txtDataFine.Text = DateAdd(DateInterval.Year, Int(txtDurataNum.Text) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "M" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, Int(txtDurataNum.Text) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "T" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 3) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "Q" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 4) - 1, CDate(txtDataInizio.Text))
            ElseIf DDLDurataTipo.SelectedValue.Trim = "S" Then
                txtDataFine.Text = DateAdd(DateInterval.Month, (Int(txtDurataNum.Text) * 6) - 1, CDate(txtDataInizio.Text))
            Else 'GIU201023 NEL CASO NON RIESCO A DETERMINARE IMPOSTO IL FINE ANNO DELLA DATA INIZIO
                txtDataFine.Text = "31/12/" + CDate(txtDataInizio.Text).Year.ToString.Trim
            End If
        End If
        'GIU201023 IMPOSTO SEMPRE IL FINE ANNO E NON INIZIO ANNO
        If Not IsDate(txtDataFine.Text.Trim) And IsDate(txtDataInizio.Text.Trim) Then
            txtDataFine.Text = "31/12/" + CDate(txtDataInizio.Text).Year.ToString.Trim
        ElseIf DDLDurataTipo.SelectedValue.Trim = "A" Then
            txtDataFine.Text = "31/12/" + CDate(txtDataFine.Text).Year.ToString.Trim
        Else
            txtDataFine.Text = DateAdd(DateInterval.Month, 1, CDate(txtDataFine.Text))
            txtDataFine.Text = DateAdd(DateInterval.Day, -1, CDate(txtDataFine.Text))
        End If
        '---------------
        Dim myDurata As Long = 0
        If txtDataInizio.Text.Trim <> "" Then
            If Not IsDate(txtDataInizio.Text.Trim) Then
                'txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataFine.Text.Trim) Then
                If CDate(txtDataFine.Text.Trim) < CDate(txtDataInizio.Text.Trim) Then
                    'txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                    'txtDataFine.ToolTip = "La data di fine Contratto dev'essere maggiore della data inizio"
                Else
                    myDurata = 0
                    If DDLDurataTipo.SelectedValue.Trim = "A" Then
                        myDurata = DateDiff(DateInterval.Year, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "M" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "T" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 3
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "Q" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 4
                    ElseIf DDLDurataTipo.SelectedValue.Trim = "S" Then
                        myDurata = DateDiff(DateInterval.Month, CDate(txtDataFine.Text), CDate(txtDataInizio.Text))
                        myDurata = myDurata / 6
                    Else
                        'DDLDurataTipo.BackColor = SEGNALA_KO : ControllaDocT = False
                    End If
                    If myDurata <> 0 Then
                        If myDurata < 0 Then
                            myDurata = myDurata * -1
                        End If
                        myDurata = Math.Round(myDurata, 0)
                        If DDLDurataTipo.SelectedValue.Trim = "A" Or DDLDurataTipo.SelectedValue.Trim = "M" Then
                            myDurata += 1
                        End If
                        '-
                        If IsNumeric(txtDurataNum.Text.Trim) Then
                            If CLng(txtDurataNum.Text) <> myDurata Then
                                txtDurataNum.BackColor = SEGNALA_INFO
                                txtDurataNum.ToolTip = "(ATTENZIONE) Durata N° prevista e di " & myDurata.ToString
                            End If
                        Else
                            txtDurataNum.Text = myDurata.ToString
                        End If
                    End If
                End If
            End If
        End If
        If Session(AGGPAGSCADTOT) = SWNO Or strErrore.Trim <> "" Then
            RicalcolaRateTotali = True
            Exit Function
        End If
        strErrore = ""
        If WUC_ContrattiDett1.AggImportiTot(strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiornamento importi e scadenze (RicalcolaRateTotali)", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
    End Function
    Private Sub DDLDurataTipo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLDurataTipo.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        txtDataFine.Text = ""
        Call RicalcolaRateTotali(True)
        txtDataInizio.Focus()
    End Sub
    Private Sub txtDurataNum_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDurataNum.TextChanged
        Session(SWMODIFICATO) = SWSI
        txtDataFine.Text = ""
        Call RicalcolaRateTotali(True)
        txtDataInizio.Focus()
    End Sub
    Private Sub DDLTipoFatt_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoFatt.SelectedIndexChanged
        txtTipoFatt.AutoPostBack = False 'giu191219
        txtTipoFatt.Text = DDLTipoFatt.SelectedValue
        Session(CSTTIPOFATT) = txtTipoFatt.Text.Trim
        txtTipoFatt.AutoPostBack = True 'giu191219
        Session(SWMODIFICATO) = SWSI
        Call RicalcolaRateTotali()
        txtRiferimento.Focus()
    End Sub

    Private Sub txtCodCliForFilProvv_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliForFilProvv.TextChanged
        If Mid(txtCodCliForFilProvv.Text, 1, 1) = "(" Then
            'giu220113
            Session(COD_CLIENTE) = String.Empty
            '---------
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text 'GIU111012
            '
            PopolaDestCliFor()
            WUC_ContrattiDett1.SetDDLLuogoAppAtt("SVUOTA")
            VisAnagrProvv(txtCodCliForFilProvv.Text)
            'giu010312 vediamo se rimane dove ha clikkato txtPagamento.Focus()
            Exit Sub
        End If
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim 'giu240615
        'GIU230113
        If Mid(txtCodCliForFilProvv.Text, 1, 1) = "1" Then
            Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        Else
            Session(COD_CLIENTE) = String.Empty 'giu230615
        End If
        Session(CSTCODFILIALE) = String.Empty 'giu220520
        '---------
        PICFPagBancaAgeLisByCliFor(False)
        WUC_ContrattiDett1.SetSWPrezzoALCSG() 'GIU210412
        Session(SWMODIFICATO) = SWSI
        'giu010312 vediamo se rimane dove ha clikkato txtPagamento.Focus()
        'giu111012 GESTIONE CAMBIO ANAGRAFICHE PROVVISORIE CON CODICE COGE QUINDI Inattivo
        Dim myIDAnagrProvv As String = Session(CSTUpgAngrProvvCG)
        If IsNothing(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        If myIDAnagrProvv = "" Then
            Exit Sub
        Else
            If CIAnagrProvv(myIDAnagrProvv.Trim) > 1 Then 'OK POSSO CANCELLARLO SE è QUESTO
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = "EliminaAnagrProvv"
                ModalPopup.Show("Cambio e disattiva Anagrafica provvisoria", "Confermi la trasformazione dell'anagrafica  provvisoria ?" & _
                                "<br> Codice. " & myIDAnagrProvv.Trim & " con il codice definitivo: " & txtCodCliForFilProvv.Text, WUC_ModalPopup.TYPE_CONFIRM)
            End If
        End If
    End Sub
    'giu111012 GESTIONE CAMBIO ANAGRAFICHE PROVVISORIE CON CODICE COGE QUINDI Inattivo
    Public Function EliminaAnagrProvv() As Boolean
        'giu180912 giu101012 aggiorna MySQL Clienti per IREDEEM
        Dim strIDAnagrProvv As String = Session(CSTUpgAngrProvvCG).ToString.Trim
        If IsNothing(strIDAnagrProvv) Then
            strIDAnagrProvv = ""
        End If
        If Mid(strIDAnagrProvv, 1, 1) <> "(" Then
            Exit Function
        End If
        strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
        If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then 'giu150513
            strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
        End If
        If strIDAnagrProvv.Trim = "" Then Exit Function
        '-----
        EliminaAnagrProvv = False
        Dim errorMsg As String = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "MySQLCli" + Session(ESERCIZIO).ToString.Trim, "", errorMsg) = True Then
            errorMsg = ""
            If DataBaseUtility.MySQLClienti(strIDAnagrProvv.Trim, CSTInattivo, errorMsg, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Cambio e disattiva Anagrafica provvisoria (MySQL)", _
                    String.Format("Errore, contattare l'amministratore di sistema. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
            'Ok Disattiva
            Try
                Dim ObjDB As New DataBaseUtility
                '' 'NON SI PUO Dim strSQL As String = "Delete From AnagrProvv Where IDAnagrProvv=" & strIDAnagrProvv.Trim
                Dim strSQL As String = "UPDATE AnagrProvv SET Tipo='A' Where IDAnagrProvv=" & strIDAnagrProvv.Trim
                If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL) = False Then
                    ModalPopup.Show("Disattiva Anagrafica provvisoria (AnagrProvv)", _
                    String.Format("Errore, disattiva Anagrafica provvisoria. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
                EliminaAnagrProvv = True
            Catch ex As Exception
                ModalPopup.Show("Disattiva Anagrafica provvisoria (AnagrProvv)", _
                    String.Format("Errore, disattiva Anagrafica provvisoria. Errore: {0}", ex.Message), _
                    WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
        End If
        '--------------------------------------------
    End Function

    Private Sub txtPagamento_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPagamento.TextChanged
        DDLPagamento.AutoPostBack = False
        PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
        DDLPagamento.AutoPostBack = True
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        Session(SWMODIFICATO) = SWSI
        Dim pCod_Causale As Long = 0 : Dim pNV As Integer = 0 : Dim pDN As Integer = 0
        txtCodCausale.AutoPostBack = False
        If GetCodCausDATipoCAIdPag(pCod_Causale, pNV, pDN) = False Then
            Exit Sub
        End If
        txtDurataNum.Text = pDN.ToString.Trim
        txtNVisite.Text = pNV.ToString.Trim
        txtCodCausale.Text = pCod_Causale.ToString.Trim
        DDLCausali.AutoPostBack = False
        Call PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
        DDLCausali.AutoPostBack = True
        txtCodCausale.AutoPostBack = True
        Call RicalcolaRateTotali()
        txtRiferimento.Focus()
    End Sub

    Private Sub txtCodAgente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAgente.TextChanged
        DDLAgenti.AutoPostBack = False 'giu161219
        PosizionaItemDDLByTxt(txtCodAgente, DDLAgenti)
        DDLAgenti.AutoPostBack = True 'giu161219
        Session(IDAGENTE) = txtCodAgente.Text.Trim
        'giu020412
        Dim myCAge As String = txtCodAgente.Text.Trim
        If Not IsNumeric(myCAge) Then myCAge = "0"
        Dim _CAge As Integer = CtrAgente(_CAge)
        If CInt(myCAge) <> _CAge Then
            lblMessAge.ForeColor = SEGNALA_KO
            lblMessAge.Text = "Assegnato: (" & _CAge.ToString.Trim & ")"
        Else
            lblMessAge.ForeColor = Drawing.Color.Blue
            lblMessAge.Text = ""
        End If
        Session(SWMODIFICATO) = SWSI
        ' ''txtCodCausale.Focus()
    End Sub
    Private Sub btnAgenti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAgenti.Click
        WFP_Agenti1.WucElement = Me
        WFP_Agenti1.SvuotaCampi()
        Session(F_ANAGRAGENTI_APERTA) = True
        WFP_Agenti1.Show()
    End Sub
    Public Sub CallBackWFPAnagrAgenti()
        Session(IDAGENTI) = ""
        Dim rk As StrAgenti
        rk = Session(RKAGENTI)
        If IsNothing(rk.IDAgenti) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDAGENTI) = rk.IDAgenti

        SqlDSAgenti.DataBind()
        DDLAgenti.Items.Clear()
        DDLAgenti.Items.Add("")
        DDLAgenti.DataBind()
        '-- mi riposiziono sul vettore/i
        txtCodAgente.Text = Session(IDAGENTI).ToString.Trim
        DDLAgenti.AutoPostBack = False 'giu161219
        PosizionaItemDDL(Session(IDAGENTI), DDLAgenti)
        DDLAgenti.AutoPostBack = True 'giu161219
        Session(SWMODIFICATO) = SWSI
    End Sub
    Public Sub CancBackWFPAnagrAgenti()
        'nulla
    End Sub

    Private Sub txtCodRespArea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodRespArea.TextChanged
        DDLRespArea.AutoPostBack = False
        PosizionaItemDDLByTxt(txtCodRespArea, DDLRespArea)
        DDLRespArea.AutoPostBack = True
        Session(IDRESPAREA) = txtCodRespArea.Text.Trim
        '-
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        '-- mi riposiziono 
        txtCodRespVisite.Text = ""
        DDLRespVisite.AutoPostBack = False
        DDLRespVisite.SelectedIndex = -1
        DDLRespVisite.AutoPostBack = True
        '-
        Session(SWMODIFICATO) = SWSI
        txtCodRespVisite.Focus()
    End Sub
    Private Sub txtCodRespVisite_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodRespVisite.TextChanged
        DDLRespVisite.AutoPostBack = False
        PosizionaItemDDLByTxt(txtCodRespVisite, DDLRespVisite)
        DDLRespVisite.AutoPostBack = True
        Session(IDRESPVISITE) = txtCodRespVisite.Text.Trim

        Session(SWMODIFICATO) = SWSI
        txtCodCausale.Focus()
    End Sub

    Private Sub txtCodCausale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCausale.TextChanged
        DDLCausali.AutoPostBack = False
        PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
        DDLCausali.AutoPostBack = True
        Session(SWMODIFICATO) = SWSI
        WUC_ContrattiDett1.SetSWPrezzoALCSG() 'GIU190412
        txtListino.Focus()
    End Sub

    Private Sub txtListino_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtListino.TextChanged
        DDLListini.AutoPostBack = False
        PosizionaItemDDLByTxt(txtListino, DDLListini)
        DDLListini.AutoPostBack = True
        Session(IDLISTINO) = txtListino.Text.Trim
        Session(SWMODIFICATO) = SWSI
        lblCodDesValutaByListino()
        txtTipoFatt.Focus()
    End Sub

    Private Sub txtTipoFatt_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTipoFatt.TextChanged
        DDLTipoFatt.AutoPostBack = False
        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
        DDLTipoFatt.AutoPostBack = True
        Session(SWMODIFICATO) = SWSI
        Call RicalcolaRateTotali()
        txtNoteDocumento.Focus()
    End Sub

    Private Sub txtNumero_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        txtNumero.TextChanged ', txtRevNDoc.TextChanged
        If CheckNewNumeroOnTab() = True Then
            txtNumero.BackColor = SEGNALA_KO
            ' ''txtRevNDoc.BackColor = SEGNALA_KO
        Else
            txtNumero.BackColor = SEGNALA_OK
            ' ''txtRevNDoc.BackColor = SEGNALA_OK
        End If
        txtDataDoc.Focus()
    End Sub
    Private Function CheckNewNumeroOnTab() As Boolean
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.CheckNewNumeroOnTab)")
            Exit Function
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" And Session(SWOP) <> SWOPNUOVO Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strErrore As String = ""
        If txtNumero.Text.Trim <> "" And IsNumeric(txtNumero.Text.Trim) Then
            Dim strSQL As String = "Select IDDocumenti From ContrattiT WHERE Tipo_Doc = '" & TipoDoc & "'"
            If Session(SWOP) = SWOPNUOVO Then
                strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
                strSQL += " AND RevisioneNDoc =0 " ' & txtRevNDoc.Text.Trim & ""
            Else
                strSQL += " AND IDDocumenti <> " & myID.Trim
                strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
                strSQL += " AND RevisioneNDoc =0 " '& txtRevNDoc.Text.Trim & ""
            End If
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        CheckNewNumeroOnTab = True
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Numero documento già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                        Exit Function
                    End If
                    'giu260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
                    Dim CkNumDoc As Long = CheckNumDoc(strErrore)
                    If CkNumDoc = -1 Then
                        Exit Function
                    End If
                    Dim myNumDoc As Long
                    If Not IsNumeric(txtNumero.Text.Trim) Then Exit Function
                    myNumDoc = txtNumero.Text.Trim
                    CkNumDoc = CkNumDoc - 1
                    If myNumDoc - 1 = CkNumDoc Then

                    ElseIf myNumDoc > CkNumDoc Then
                        CheckNewNumeroOnTab = True
                        txtNumero.BackColor = SEGNALA_KO
                        CkNumDoc += 1 'GIU231019
                        txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. MAGGIORE di quello inserito. !!!VERIFICARE !!!"
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Numero documento MAGGIORE di quello inserito. !!!VERIFICARE!!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                        Exit Function
                    End If
                End If
            Catch Ex As Exception
                CheckNewNumeroOnTab = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Finally
                ObjDB = Nothing
            End Try
        End If
    End Function
    'giu130520
    Private Sub btnVerbale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerbale.Click
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Session(CSTTASTOST) = btnVerbale.ID

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
                        Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
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
        Call OKApriStampa()
    End Sub
    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampa.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then Exit Sub 'giu191111
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Session(CSTTASTOST) = btnStampa.ID
        'giu201211 Impostazione btnStampa per i MM SM CM
        If Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
            Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Or _
            Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Then
            StampaMM()
            Exit Sub
        End If
        '-----------------------------------------------

        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnStampa_Click)")
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
                        Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
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
            ModalPopup.Show("Errore in Contratti.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa()
    End Sub
    Private Sub StampaMM()
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.StampaMM)")
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
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim selezione As String = ""
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Lotti/N° Serie collegati)", selezione, dsMovMag1, Errore, "", CLng(myID), "", True, "", "", False, False) Then
                Session(CSTDsPrinWebDoc) = dsMovMag1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

#Region "Gestione Anagrafiche / provvisorie e Gestione BancheIBAN"

    Private Sub btnInsAnagrProvv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInsAnagrProvv.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnInsAnagrProvv_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "C"
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
        Else 'sessione scaduta no per CLIFOR
            'GIU191211
            Session(TIPORK) = "C"
            ' ''Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''Exit Sub
        End If
        Session(IDANAGRPROVV) = "" 'GIU220819
        WFP_AnagrProvv_Insert1.WucElement = Me
        Session(F_ANAGR_PROVV_APERTA) = True
        Dim strMessErr As String = ""
        If WFP_AnagrProvv_Insert1.PopolaDLL(strMessErr) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", strMessErr, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        WFP_AnagrProvv_Insert1.SvuotaCampi()
        WFP_AnagrProvv_Insert1.Show()
    End Sub
    'GIU150513 PER LA MODIFICA DELL'ANAGRAFICA PROVVISORIA
    Private Sub subModificaAngrProvv()
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.subModificaAngrProvv)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "C"
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
        Else 'sessione scaduta no per CLIFOR
            'GIU191211
            Session(TIPORK) = "C"
            ' ''Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''Exit Sub
        End If
        WFP_AnagrProvv_Insert1.WucElement = Me
        Session(F_ANAGR_PROVV_APERTA) = True 'GIU100118 SPOSTATO PRIMA ALTRIMENTI NON CARICA NULLA
        WFP_AnagrProvv_Insert1.PopolaCampiModifica()
        Session(F_ANAGR_PROVV_APERTA) = True
        WFP_AnagrProvv_Insert1.Show()
    End Sub
    '-----------------------------------------------------
    Private Sub btnModificaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaAnagrafica.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnModificaAnagrafica_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "C"
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
            'giu191211 
            ' ''Else 'sessione scaduta
            ' ''    Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''    Exit Sub
        End If

        If txtCodCliForFilProvv.Text.Trim = "" Or Left(txtCodCliForFilProvv.Text.Trim, 1) = "(" Then
            If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
                Dim strIDAnagrProvv As String = txtCodCliForFilProvv.Text.Trim
                strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
                If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then
                    strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
                    If IsNumeric(strIDAnagrProvv.Trim) Then
                        Session(IDANAGRPROVV) = strIDAnagrProvv.Trim
                        subModificaAngrProvv()
                    Else
                        Session(IDANAGRPROVV) = "" 'giu100118
                    End If
                Else
                    Session(IDANAGRPROVV) = "" 'giu100118
                End If
                Exit Sub
            End If
        End If
        If Not IsNumeric(txtCodCliForFilProvv.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----- Cerco
        'giu191211
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            Session(TIPORK) = "C"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            Session(TIPORK) = "F"
        End If
        If TabCliFor = "Cli" Then
            lblLabelCliForFilProvv.Text = "Cliente" : Session(CSTTABCLIFOR) = "Cli"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
        ElseIf TabCliFor = "For" Then
            lblLabelCliForFilProvv.Text = "Fornitore" : Session(CSTTABCLIFOR) = "For"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Fornitori] WHERE ([Codice_CoGe] = @Codice)"
        Else 'DEFAULT giu191211
            lblLabelCliForFilProvv.Text = "Cli./For." : Session(CSTTABCLIFOR) = "CliFor"
            SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)"
        End If
        '----------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        Dim dvCliFor As DataView
        dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
        If (dvCliFor Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(IDANAGRCLIFOR) = txtCodCliForFilProvv.Text.Trim
        Dim Rk As StrAnagrCliFor = Nothing
        If dvCliFor.Count > 0 Then
            Rk.Rag_Soc = dvCliFor.Item(0).Item("Rag_Soc").ToString
            Rk.Denominazione = dvCliFor.Item(0).Item("Denominazione").ToString
            Rk.Riferimento = dvCliFor.Item(0).Item("Riferimento").ToString
            Rk.Codice_Fiscale = dvCliFor.Item(0).Item("Codice_Fiscale").ToString
            Rk.Partita_IVA = dvCliFor.Item(0).Item("Partita_IVA").ToString
            Rk.Indirizzo = dvCliFor.Item(0).Item("Indirizzo").ToString + " " + dvCliFor.Item(0).Item("NumeroCivico").ToString
            Rk.NumeroCivico = "" 'dvCliFor.Item(0).Item("NumeroCivico").ToString
            Rk.Cap = dvCliFor.Item(0).Item("CAP").ToString
            Rk.Localita = dvCliFor.Item(0).Item("Localita").ToString
            Rk.Provincia = dvCliFor.Item(0).Item("Provincia").ToString.Trim
            Rk.Nazione = dvCliFor.Item(0).Item("Nazione").ToString.Trim
            Rk.Telefono1 = dvCliFor.Item(0).Item("Telefono1").ToString.Trim
            Rk.Telefono2 = dvCliFor.Item(0).Item("Telefono2").ToString.Trim
            Rk.Fax = dvCliFor.Item(0).Item("Fax").ToString.Trim
            Rk.Regime_Iva = dvCliFor.Item(0).Item("Regime_Iva").ToString.Trim
            Rk.EMail = dvCliFor.Item(0).Item("EMail").ToString.Trim 'giu070514
            Rk.PECEMail = dvCliFor.Item(0).Item("PECEMail").ToString.Trim 'giu190122
            If TabCliFor = "Cli" Then
                Rk.IPA = dvCliFor.Item(0).Item("IPA").ToString.Trim
                Rk.SplitIVA = CBool(dvCliFor.Item(0).Item("SplitIVA"))
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-ok trovato
        Session("AvvisaDoppio") = True 'giu080113
        Session(RKANAGRCLIFOR) = Rk
        WFP_Anagrafiche_Modify1.WucElement = Me
        WFP_Anagrafiche_Modify1.PopolaCampi()
        Session(F_ANAGRCLIFOR_APERTA) = True
        WFP_Anagrafiche_Modify1.Show()
    End Sub
    'giu290320
    Private Sub btnDelDest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelDest.Click
        Session(CSTCODFILIALE) = ""
        Session(SWMODIFICATO) = SWSI
        PopolaDestCliFor()
    End Sub
    Private Sub btnModificaDest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaDest.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnModificaAnagrafica_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "C"
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
        End If
        Dim myCG As String = txtCodCliForFilProvv.Text.Trim
        If Not IsNothing(myCG) Then
            If String.IsNullOrEmpty(myCG) Then
                myCG = ""
            End If
        Else
            myCG = ""
        End If
        If myCG.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----- Cerco
        If lblDestSel.Text.Trim = "" Then Session(CSTCODFILIALE) = String.Empty 'giu220520
        Dim myCGDest As String = Session(CSTCODFILIALE)
        If Not IsNothing(myCGDest) Then
            If String.IsNullOrEmpty(myCGDest) Then
                myCGDest = ""
            End If
        Else
            myCGDest = ""
        End If
        'giu220520
        If myCGDest.Trim = "" Then
            myCGDest = lblDestSel.ToolTip
        End If
        If Not IsNumeric(myCGDest) Then
            myCGDest = ""
        End If
        Session(CSTCODFILIALE) = myCGDest.Trim
        '--------------
        If myCGDest.Trim = "" Then
            If lblDestSel.Text.Trim = "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "NuovaDest"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Nuova Destinazione", "Vuole inserire una NUOVA Destinazione?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                Exit Sub
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = "SelezionaDest"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NuovaDest"
                ModalPopup.Show("Attenzione", "Sono presenti Destinazioni diverse. <br>Vuole selezionarle? (S)i <br> Oppure Vuole inserire una NUOVA ? (N)o", WUC_ModalPopup.TYPE_CONFIRM_YNA)
                Exit Sub
            End If
        End If
        '-ok leggo
        Dim strSQL As String = ""
        strSQL = "Select * From DestClienti WHERE Codice = '" & myCG.ToString.Trim & "'"
        If IsNumeric(myCGDest) Then
            strSQL += " AND Progressivo=" & myCGDest.Trim
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myDestCF As DestCliForEntity = Nothing 'GIU220520
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myDestCF = New DestCliForEntity
                        With myDestCF
                            .Progressivo = IIf(IsDBNull(row.Item("Progressivo")), "", row.Item("Progressivo"))
                            .Ragione_Sociale = IIf(IsDBNull(row.Item("Ragione_Sociale")), "", row.Item("Ragione_Sociale"))
                            .Indirizzo = IIf(IsDBNull(row.Item("Indirizzo")), "", row.Item("Indirizzo"))
                            .Cap = IIf(IsDBNull(row.Item("Cap")), "", row.Item("Cap"))
                            .Localita = IIf(IsDBNull(row.Item("Localita")), "", row.Item("Localita"))
                            .Provincia = IIf(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia"))
                            .Stato = IIf(IsDBNull(row.Item("Stato")), "", row.Item("Stato"))
                            .Denominazione = IIf(IsDBNull(row.Item("Denominazione")), "", row.Item("Denominazione"))
                            .Riferimento = IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
                            .Telefono1 = IIf(IsDBNull(row.Item("Telefono1")), "", row.Item("Telefono1"))
                            .Telefono2 = IIf(IsDBNull(row.Item("Telefono2")), "", row.Item("Telefono2"))
                            .Fax = IIf(IsDBNull(row.Item("Fax")), "", row.Item("Fax"))
                            .EMail = IIf(IsDBNull(row.Item("EMail")), "", row.Item("EMail"))
                            .Tipo = IIf(IsDBNull(row.Item("Tipo")), "", row.Item("Tipo"))
                        End With
                        Exit For
                    Next
                   If myCGDest.Trim = "" Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Codice Destinazione non valido", WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    Else
                        'ok
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Codice Destinazione non valido", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice Destinazione non valido", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Errore: Lettura DestClienti - " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If (myDestCF Is Nothing) Then 'GIU220520 
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice Destinazione non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-ok trovato
        Session(IDDESTCLIFOR) = myDestCF.Progressivo
        Session(RKANAGRDESTCLI) = myDestCF
        WFP_DestCliFor1.WucElement = Me
        WFP_DestCliFor1.PopolaCampi()
        Session(F_DESTCLIFOR_APERTA) = True
        WFP_DestCliFor1.Show()
    End Sub
    Public Sub SelezionaDest()
        btnCercaDest_Click(Nothing, Nothing)
    End Sub
    Public Function CallBackWFPDestCliFor() As Boolean

        Session(IDDESTCLIFOR) = ""
        Dim rk As DestCliForEntity
        rk = Session(RKANAGRDESTCLI)
        If IsNothing(rk.Ragione_Sociale) Then
            Exit Function
        End If
        Session(CSTCODFILIALE) = rk.Progressivo.Trim
        Call PopolaDestCliFor()
    End Function
    Public Sub NuovaDest()
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.NuovaDest)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "C"
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
        End If
        Dim myCG As String = txtCodCliForFilProvv.Text.Trim
        If Not IsNothing(myCG) Then
            If String.IsNullOrEmpty(myCG) Then
                myCG = ""
            End If
        Else
            myCG = ""
        End If
        If myCG.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim strSQL As String = ""
        If TabCliFor = "Cli" Then
            strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & myCG.ToString.Trim & "'"
        Else
            strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & myCG.ToString.Trim & "'"
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myDestCF As DestCliForEntity = Nothing
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myDestCF = New DestCliForEntity
                        With myDestCF
                            .Progressivo = "-1"
                            .Ragione_Sociale = IIf(IsDBNull(row.Item("Rag_Soc")), "", row.Item("Rag_Soc"))
                            .Indirizzo = IIf(IsDBNull(row.Item("Indirizzo")), "", row.Item("Indirizzo"))
                            .Cap = IIf(IsDBNull(row.Item("Cap")), "", row.Item("Cap"))
                            .Localita = IIf(IsDBNull(row.Item("Localita")), "", row.Item("Localita"))
                            .Provincia = IIf(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia"))
                            .Stato = IIf(IsDBNull(row.Item("Nazione")), "", row.Item("Nazione"))
                            .Denominazione = IIf(IsDBNull(row.Item("Denominazione")), "", row.Item("Denominazione"))
                            .Riferimento = IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
                            .Telefono1 = IIf(IsDBNull(row.Item("Telefono1")), "", row.Item("Telefono1"))
                            .Telefono2 = IIf(IsDBNull(row.Item("Telefono2")), "", row.Item("Telefono2"))
                            .Fax = IIf(IsDBNull(row.Item("Fax")), "", row.Item("Fax"))
                            .EMail = IIf(IsDBNull(row.Item("EMail")), "", row.Item("EMail"))
                            .Tipo = (ds.Tables(0).Rows.Count + 1).ToString.Trim
                        End With
                        Exit For
                    Next
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Errore: Lettura Clienti/Fornitori - " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If (myDestCF Is Nothing) Then 'GIU220520 
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice Destinazione non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-ok trovato
        Session(IDDESTCLIFOR) = myDestCF.Progressivo
        Session(RKANAGRDESTCLI) = myDestCF
        WFP_DestCliFor1.WucElement = Me
        WFP_DestCliFor1.PopolaCampi()
        Session(F_DESTCLIFOR_APERTA) = True
        WFP_DestCliFor1.Show()
    End Sub
    Private Sub btnCercaBanca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaBanca.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnCercaBanca_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            Session(TIPORK) = "A" 'BANCHE AZIENDA che usa il programma
        ElseIf TabCliFor = "For" Then
            Session(TIPORK) = "F"
        Else 'sessione scaduta NONPIU' GIU191211
            If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
                Session(TIPORK) = "A"
            ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
                Session(TIPORK) = "F"
            Else
                Session(TIPORK) = "A"
            End If
            ' ''Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''Exit Sub
        End If
        WFP_BancheIBAN1.WucElement = Me
        WFP_BancheIBAN1.SvuotaCampi()
        Session(F_BANCHEIBAN_APERTA) = True
        WFP_BancheIBAN1.Show()
    End Sub
#End Region

#Region "Ricerca Clienti/Fornitori"


    Private Sub btnCercaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Contratti.btnCercaAnagrafica_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            ApriElencoClienti()
        ElseIf TabCliFor = "For" Then
            ApriElencoFornitori()
        Else 'sessione scaduta
            If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
                SWCercaCli()
                Exit Sub
            ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
                SWCercaFor()
                Exit Sub
            End If
            ' ''Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''Exit Sub
            Session(MODALPOPUP_CALLBACK_METHOD) = "SWCercaCli"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "SWCercaFor"
            ModalPopup.Show("Ricerca anagrafiche Clienti/Fornitori", "Scegli il tipo di ricerca ......", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub SWCercaCli()
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti()
    End Sub
    Public Sub SWCercaFor()
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori()
    End Sub

    Private Sub ApriElencoClienti()
        Session(F_CLI_RICERCA) = True
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoCli.Show(True)
    End Sub
    Private Sub ApriElencoFornitori()
        Session(F_FOR_RICERCA) = True
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoFor.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        'giu191211
        Session(F_CLI_RICERCA) = False
        Session(F_FOR_RICERCA) = False
        '------
        txtCodCliForFilProvv.Text = codice
        lblCliForFilProvv.Text = descrizione
        PICFPagBancaAgeLisByCliFor(False)
        Session(SWMODIFICATO) = SWSI
        'giu111012 GESTIONE CAMBIO ANAGRAFICHE PROVVISORIE CON CODICE COGE QUINDI Inattivo
        Dim myIDAnagrProvv As String = Session(CSTUpgAngrProvvCG)
        If IsNothing(myIDAnagrProvv) Then
            myIDAnagrProvv = ""
        End If
        If myIDAnagrProvv = "" Then
            Exit Sub
        Else
            If CIAnagrProvv(myIDAnagrProvv.Trim) > 1 Then 'OK POSSO CANCELLARLO SE è QUESTO
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = "EliminaAnagrProvv"
                ModalPopup.Show("Cambio e disattiva Anagrafica provvisoria", "Confermi la trasformazione dell'anagrafica  provvisoria ?" & _
                                "<br> Codice. " & myIDAnagrProvv.Trim & " con il codice definitivo: " & txtCodCliForFilProvv.Text, WUC_ModalPopup.TYPE_CONFIRM)
            End If
        End If
    End Sub

    Private Sub btnCercaDest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaDest.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattibtnCercaDest_Click)")
            Exit Sub
        End If
        'GIU220520
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim 'giu240615
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            'OK
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            'OK
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Selezionare il Cliente/Fornitore", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU220520 RICARICO SEMPRE 
        Session(DESTCLIFOR) = Nothing
        '----------------
        Session(F_ELENCO_DESTCF_APERTA) = True
        WFPElencoDestCF.Show(True)
    End Sub
    Public Sub CallBackWFPElencoDestCF(ByVal codice As String, ByVal descrizione As String)
        If IsNumeric(codice) Then
            Session(CSTCODFILIALE) = codice
        Else
            Session(CSTCODFILIALE) = ""
        End If
        Session(SWMODIFICATO) = SWSI
        PopolaDestCliFor()
    End Sub
    Private Sub PopolaDestCliFor() 'giu250320
        Dim myCG As String = txtCodCliForFilProvv.Text.Trim
        If Not IsNothing(myCG) Then
            If String.IsNullOrEmpty(myCG) Then
                myCG = ""
            End If
        Else
            myCG = ""
        End If
        If myCG.Trim = "" Then
            lblDestSel.Text = "" : lblDestSel.ToolTip = ""
            Session(CSTCODFILIALE) = ""
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
            lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
            lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
            lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
            Exit Sub
        End If
        Dim myCGDest As String = Session(CSTCODFILIALE)
        If Not IsNothing(myCGDest) Then
            If String.IsNullOrEmpty(myCGDest) Then
                myCGDest = ""
            End If
        Else
            myCGDest = ""
        End If
        If myCGDest.Trim = "" Then
            lblDestSel.Text = "" : lblDestSel.ToolTip = ""
            Session(CSTCODFILIALE) = ""
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
            lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
            lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
            lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
        End If
        '-ok leggo
        Dim strSQL As String = ""
        strSQL = "Select * From DestClienti WHERE Codice = '" & myCG.ToString.Trim & "'"
        If IsNumeric(myCGDest) Then
            strSQL += " AND Progressivo=" & myCGDest.Trim
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myDestCF As DestCliForEntity = Nothing
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myDestCF = New DestCliForEntity
                        With myDestCF
                            .Progressivo = IIf(IsDBNull(row.Item("Progressivo")), "", row.Item("Progressivo"))
                            .Ragione_Sociale = IIf(IsDBNull(row.Item("Ragione_Sociale")), "", row.Item("Ragione_Sociale"))
                            .Indirizzo = IIf(IsDBNull(row.Item("Indirizzo")), "", row.Item("Indirizzo"))
                            .Cap = IIf(IsDBNull(row.Item("Cap")), "", row.Item("Cap"))
                            .Localita = IIf(IsDBNull(row.Item("Localita")), "", row.Item("Localita"))
                            .Provincia = IIf(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia"))
                            .Stato = IIf(IsDBNull(row.Item("Stato")), "", row.Item("Stato"))
                            .Denominazione = IIf(IsDBNull(row.Item("Denominazione")), "", row.Item("Denominazione"))
                            .Riferimento = IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
                            .Telefono1 = IIf(IsDBNull(row.Item("Telefono1")), "", row.Item("Telefono1"))
                            .Telefono2 = IIf(IsDBNull(row.Item("Telefono2")), "", row.Item("Telefono2"))
                            .Fax = IIf(IsDBNull(row.Item("Fax")), "", row.Item("Fax"))
                            .EMail = IIf(IsDBNull(row.Item("EMail")), "", row.Item("EMail"))
                            .Tipo = IIf(IsDBNull(row.Item("Tipo")), "", row.Item("Tipo"))
                        End With
                        Exit For
                    Next
                    If (ds.Tables(0).Rows.Count > 0) And myCGDest.Trim = "" Then
                        lblDestSel.Text = "[SONO PRESENTI DESTINAZIONI DIVERSE]"
                        lblDestSel.ToolTip = ""
                        Session(CSTCODFILIALE) = ""
                        txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                        lblDenominazioneD.Text = ""
                        lblRiferimentoD.Text = ""
                        lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                        lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                        lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                        Exit Sub
                    ElseIf myCGDest.Trim = "" Then
                        lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                        Session(CSTCODFILIALE) = ""
                        txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                        lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                        lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                        lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                        lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                        Exit Sub
                    ElseIf Not (myDestCF Is Nothing) Then
                        lblDestSel.ToolTip = myCGDest.Trim
                        lblDestSel.Text = Mid("(" + IIf(String.IsNullOrEmpty(myDestCF.Tipo), "", myDestCF.Tipo.Trim) + ") " + myDestCF.Ragione_Sociale.Trim, 1, 40)
                        txtDestinazione1.Text = myDestCF.Ragione_Sociale.Trim
                        txtDestinazione2.Text = myDestCF.Indirizzo.Trim
                        txtDestinazione3.Text = myDestCF.Cap.Trim & " " & myDestCF.Localita.Trim & " " & IIf(myDestCF.Provincia.Trim <> "", "(" & myDestCF.Provincia.Trim & ")", "")
                        lblDenominazioneD.Text = Mid(myDestCF.Denominazione.Trim, 1, 45)
                        lblRiferimentoD.Text = Mid(myDestCF.Riferimento.Trim, 1, 45)
                        lblDenominazioneD.ToolTip = myDestCF.Denominazione.Trim
                        lblRiferimentoD.ToolTip = myDestCF.Riferimento.Trim
                        lblTel1D.Text = Mid(myDestCF.Telefono1.Trim, 1, 15) : lblTel1D.ToolTip = myDestCF.Telefono1.Trim
                        lblTel2D.Text = Mid(myDestCF.Telefono2.Trim, 1, 15) : lblTel2D.ToolTip = myDestCF.Telefono2.Trim
                        lblFaxD.Text = Mid(myDestCF.Fax.Trim, 1, 15) : lblFaxD.ToolTip = myDestCF.Fax.Trim
                        lblEMailD.Text = Mid(myDestCF.EMail.Trim, 1, 45) : lblEMailD.ToolTip = myDestCF.EMail.Trim
                    Else
                        lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                        Session(CSTCODFILIALE) = ""
                        txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                        lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                        lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                        lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                        lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                        Exit Sub
                    End If
                    Exit Sub
                Else
                    lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                    Session(CSTCODFILIALE) = ""
                    txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                    lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                    lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                    lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                    lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                    Exit Sub
                End If
            Else
                lblDestSel.Text = "" : lblDestSel.ToolTip = ""
                Session(CSTCODFILIALE) = ""
                txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
                lblDenominazioneD.Text = "" : lblRiferimentoD.Text = ""
                lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
                lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
                lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
                Exit Sub
            End If
        Catch Ex As Exception
            lblDestSel.Text = "" : lblDestSel.ToolTip = ""
            Session(CSTCODFILIALE) = ""
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            lblDenominazioneD.Text = "Errore: Lettura DestClienti - " & Ex.Message
            lblRiferimentoD.Text = ""
            lblTel1D.Text = "" : lblTel2D.Text = "" : lblFaxD.Text = "" : lblEMailD.Text = ""
            lblDenominazioneD.ToolTip = "" : lblRiferimentoD.ToolTip = ""
            lblTel1D.ToolTip = "" : lblTel2D.ToolTip = "" : lblFaxD.ToolTip = "" : lblEMailD.ToolTip = ""
            Exit Sub
        End Try
    End Sub
#End Region

    Private Sub btnRespArea_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRespArea.Click
        WFP_RespArea1.WucElement = Me
        WFP_RespArea1.SvuotaCampi()
        WFP_RespArea1.SetlblMessaggi("Inserimento nuovo Responsabile Area")
        Session(F_ANAGRRESPAREA_APERTA) = True
        WFP_RespArea1.Show()
    End Sub
    Public Sub CallBackWFPAnagrRespArea()
        Session(IDRESPAREA) = ""
        Dim rk As StrRespArea
        rk = Session(RKRESPAREA)
        If IsNothing(rk.IDRespArea) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDRESPAREA) = rk.IDRespArea

        SqlDSRespArea.DataBind()
        DDLRespArea.Items.Clear()
        DDLRespArea.Items.Add("")
        DDLRespArea.DataBind()
        '-- mi riposiziono 
        txtCodRespArea.BackColor = SEGNALA_KO
        txtCodRespArea.Text = Session(IDRESPAREA).ToString.Trim
        DDLRespArea.AutoPostBack = False
        PosizionaItemDDL(txtCodRespArea.Text, DDLRespArea)
        DDLRespArea.AutoPostBack = True
        '-
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        '-- mi riposiziono 
        txtCodRespVisite.Text = ""
        DDLRespVisite.AutoPostBack = False
        DDLRespVisite.SelectedIndex = -1
        DDLRespVisite.AutoPostBack = True
        '--
        Session(SWMODIFICATO) = SWSI
    End Sub
    Public Sub CancBackWFPAnagrRespArea()
        'nulla
    End Sub
    'giu161219
    Private Sub btnRespVisite_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRespVisite.Click
        If txtCodRespArea.Text.Trim = "" Or Not IsNumeric(txtCodRespArea.Text.Trim) Then
            txtCodRespArea.BackColor = SEGNALA_KO : txtCodRespArea.ToolTip = "Selezionare il Responsabile Area"
            Exit Sub
        ElseIf IsNumeric(txtCodRespArea.Text.Trim) Then
            If Int(txtCodRespArea.Text.Trim) = 0 Then
                txtCodRespArea.BackColor = SEGNALA_KO : txtCodRespArea.ToolTip = "Selezionare il Responsabile Area"
                Exit Sub
            End If
        End If
        WFP_RespVisite1.WucElement = Me
        WFP_RespVisite1.SvuotaCampi()
        WFP_RespVisite1.FiltraRespArea()
        WFP_RespVisite1.SetlblMessaggi("Inserimento nuovo Responsabile Visita")
        Session(F_ANAGRRESPVISITE_APERTA) = True
        WFP_RespVisite1.Show()
    End Sub
    Public Sub CallBackWFPAnagrRespVisite()
        Session(IDRESPVISITE) = ""
        Dim rk As StrRespVisite
        rk = Session(RKRESPVISITE)
        If IsNothing(rk.IDRespVisite) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDRESPVISITE) = rk.IDRespVisite
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        '-- mi riposiziono 
        txtCodRespVisite.AutoPostBack = False
        txtCodRespVisite.BackColor = SEGNALA_OK
        txtCodRespVisite.Text = Session(IDRESPVISITE).ToString.Trim
        txtCodRespVisite.AutoPostBack = True
        DDLRespVisite.AutoPostBack = False
        PosizionaItemDDL(Session(IDRESPVISITE).ToString.Trim, DDLRespVisite)
        DDLRespVisite.AutoPostBack = True
        Session(SWMODIFICATO) = SWSI
    End Sub
    Public Sub CancBackWFPAnagrRespVisite()
        'nulla
    End Sub
    '---------
    'GIU111012 PRIMA DI CANCELLARE L'ANGRProvv per def. CG verifico che non sia usata 
    Private Function CIAnagrProvv(ByVal MyID As String) As Integer
        If MyID.Trim = "" Then Return 0

        Dim strIDAnagrProvv As String = MyID.Trim
        If Mid(strIDAnagrProvv, 1, 1) <> "(" Then
            Return 0
        End If
        strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 2)
        If Not IsNumeric(Right(strIDAnagrProvv, 1)) Then 'giu150513
            strIDAnagrProvv = Mid(strIDAnagrProvv.Trim, 1, Len(strIDAnagrProvv) - 1)
        End If
        If strIDAnagrProvv.Trim = "" Then Return 0
        '-----
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 2 DocumentiT.IDAnagrProvv"
        strSQL = strSQL & " FROM DocumentiT"
        strSQL = strSQL & " WHERE (DocumentiT.IDAnagrProvv = " & strIDAnagrProvv.Trim & ")"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            'PROSEGUO ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CIAnagrProvv = ds.Tables(0).Rows.Count
                    'PROSEGUO Exit Function
                Else
                    CIAnagrProvv = 0
                    'PROSEGUO Exit Function
                End If
            Else
                CIAnagrProvv = 0
                'PROSEGUO Exit Function
            End If
        Catch Ex As Exception
            CIAnagrProvv = -1
            Exit Function
        End Try
        'giu291119 controllo anche nei CONTRATTI
        strSQL = "SELECT TOP 2 ContrattiT.IDAnagrProvv"
        strSQL = strSQL & " FROM ContrattiT"
        strSQL = strSQL & " WHERE (ContrattiT.IDAnagrProvv = " & strIDAnagrProvv.Trim & ")"
        Dim dsCon As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsCon)
            ObjDB = Nothing
            If (dsCon.Tables.Count > 0) Then
                If (dsCon.Tables(0).Rows.Count > 0) Then
                    CIAnagrProvv += dsCon.Tables(0).Rows.Count
                    Exit Function
                Else
                    CIAnagrProvv += 0
                    Exit Function
                End If
            Else
                CIAnagrProvv += 0
                Exit Function
            End If
        Catch Ex As Exception
            CIAnagrProvv = -1
            Exit Function
        End Try
    End Function
#Region "Elenco Tabelle"
    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            ' ''Case F_NAZIONI
            ' ''    WFPElencoNazioni.Show(True)
            ' ''Case F_PROVINCE
            ' ''    WFPElencoProvince.Show(True)
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show(True)
                ' ''Case F_PAGAMENTI
                ' ''    WFPElencoPagamenti.Show(True)
                ' ''Case F_AGENTI
                ' ''    WFPElencoAgenti.Show(True)
                ' ''Case F_AGENTI_ESE_PREC
                ' ''    WFPElencoAgentiEsePrec.Show(True)
                ' ''Case F_ZONE
                ' ''    WFPElencoZone.Show(True)
                ' ''Case F_VETTORI
                ' ''    WFPElencoVettori.Show(True)
                ' ''Case F_CATEGORIE
                ' ''    WFPElencoCategorie.Show(True)
                ' ''Case F_LISTINO
                ' ''    WFPElencoListVenT.Show(True)
                ' ''Case F_CONTI
                ' ''    WFPElencoConti.Show(True)
        End Select
    End Sub
    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        Select Case (finestra)
            ' ''Case F_NAZIONI
            ' ''    txtCodNazione.Text = codice
            ' ''    txtCodNazione.BackColor = Def.SEGNALA_OK
            ' ''    lblNazione.Text = descrizione
            ' ''Case F_PROVINCE
            ' ''    txtProvincia.Text = codice
            ' ''    txtProvincia.BackColor = Def.SEGNALA_OK
            Case F_ALIQUOTAIVA
                Session(CSTREGIMEIVA) = codice.Trim
                LnkRegimeIVA.Text = "Regime IVA " & codice.Trim

                ' ''txtCodRegimeIVA.Text = codice
                ' ''txtCodRegimeIVA.BackColor = Def.SEGNALA_OK
                ' ''lblRegimeIva.Text = descrizione

                ' ''Case F_PAGAMENTI
                ' ''    txtCodPagamento.Text = codice
                ' ''    txtCodPagamento.BackColor = Def.SEGNALA_OK
                ' ''    lblPagamento.Text = descrizione
                ' ''Case F_AGENTI
                ' ''    txtCodAgente.Text = codice
                ' ''    txtCodAgente.BackColor = Def.SEGNALA_OK
                ' ''    lblAgente.Text = descrizione
                ' ''Case F_AGENTI_ESE_PREC
                ' ''    txtCodAgenteEsePrec.Text = codice
                ' ''    txtCodAgenteEsePrec.BackColor = Def.SEGNALA_OK
                ' ''    lblAgenteEsePrec.Text = descrizione
                ' ''Case F_ZONE
                ' ''    txtCodZona.Text = codice
                ' ''    txtCodZona.BackColor = Def.SEGNALA_OK
                ' ''    lblZona.Text = descrizione
                ' ''Case F_VETTORI
                ' ''    txtCodVettore.Text = codice
                ' ''    txtCodVettore.BackColor = Def.SEGNALA_OK
                ' ''    lblVettore.Text = descrizione
                ' ''Case F_CATEGORIE
                ' ''    txtCodCategoria.Text = codice
                ' ''    txtCodCategoria.BackColor = Def.SEGNALA_OK
                ' ''    lblCategorie.Text = descrizione
                ' ''Case F_LISTINO
                ' ''    txtCodListino.Text = codice
                ' ''    txtCodListino.BackColor = Def.SEGNALA_OK
                ' ''    lblListino.Text = descrizione
                ' ''Case F_CONTI
                ' ''    txtCodRicavoFT.Text = codice
                ' ''    txtCodRicavoFT.BackColor = Def.SEGNALA_OK
                ' ''    txtRicavoFT.Text = descrizione
        End Select
    End Sub
#End Region

    'giu070814
    Private Sub chkFatturaPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaPA.CheckedChanged
        'GIU070814
        Dim strErrore As String = "" : Dim mySplitIVA As Boolean = False 'giu050118
        chkFatturaPA.BackColor = SEGNALA_OK
        chkFatturaPA.ToolTip = ""
        If txtCodCliForFilProvv.Text.Trim <> "" Then
            SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore)
            If strErrore.Trim <> "" Then
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                chkFatturaPA.BackColor = SEGNALA_KO
                'chkFatturaPA.ToolTip = strErrore
                SWFatturaPA = False
                Session(CSTFATTURAPA) = SWFatturaPA
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
                ' ''Exit Function
            ElseIf chkFatturaPA.Checked = True And SWFatturaPA = False Then
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                chkFatturaPA.BackColor = SEGNALA_INFO
                SWFatturaPA = False
                Session(CSTFATTURAPA) = SWFatturaPA
                'chkFatturaPA.ToolTip = "Attenzione, il cliente selezionato non ha il codice IPA"
                strErrore = "Attenzione, il cliente selezionato non ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
            ElseIf chkFatturaPA.Checked = False And SWFatturaPA = True Then
                chkFatturaPA.BackColor = SEGNALA_INFO
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = True
                chkFatturaPA.AutoPostBack = True
                Session(CSTFATTURAPA) = SWFatturaPA
                'chkFatturaPA.ToolTip = "Attenzione, il cliente selezionato ha il codice IPA"
                strErrore = "Attenzione, il cliente selezionato ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
            End If

        End If
        '----
    End Sub

    Private Sub chkFatturaAC_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaAC.CheckedChanged
        chkScGiacenza.Enabled = chkFatturaAC.Checked
        If chkScGiacenza.Enabled = False Then
            chkScGiacenza.Checked = False
        End If
    End Sub

    Public Function SetLblTotLMPL(ByVal _TotLM As Decimal, ByVal _TotDeduzioni As Decimal) As Boolean
        WUC_ContrattiSpeseTraspTot1.SetLblTotLMPL(_TotLM, _TotDeduzioni)
    End Function
    'giu140319 ricacolo tutti i dettagli con eventuali sc.cassa aggiunto oppure cambio tipo pagamento ricalcola le scadenze
    Public Function AggImportiTot(ByRef _Errore As String) As Boolean
        _Errore = ""
        If WUC_ContrattiDett1.AggImportiTot(_Errore) = False Then
            If _Errore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiornamento importi e scadenze", _Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
    End Function

    'GIU210120 
    Private Sub OKApriStampa()
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        LnkStampa.Visible = False : LnkVerbale.Visible = False
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
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ContrattiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
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
        Try 'giu080324 giu281112 errore che il file Ã¨ gia aperto
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
            Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
            getOutputRPT(Rpt, "PDF")
            '---------
            '''Session(CSTESPORTAPDF) = True
            '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
            '''Dim stPathReport As String = Session(CSTPATHPDF)

            '''Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            ''''giu140124
            '''Rpt.Close()
            '''Rpt.Dispose()
            '''Rpt = Nothing
            ''''-
            '''GC.WaitForPendingFinalizers()
            '''GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            LnkVerbale.Visible = True
        Else
            LnkStampa.Visible = True
            LnkVerbale.Visible = True
        End If

        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
        '''    LnkVerbale.HRef = LnkName
        '''Else
        '''    LnkVerbale.HRef = LnkName
        '''    LnkVerbale.HRef = LnkName
        '''End If
    End Sub
    '@@@@@
    Private Function getOutputRPT(ByVal _Rpt As Object, ByVal _Formato As String) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            If _Formato = "PDF" Then
                myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Else
                myStream = _Rpt.ExportToStream(ExportFormatType.ExcelRecord)
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

    Private Sub txtDataInizio_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataInizio.TextChanged
        Session(SWMODIFICATO) = SWSI
        If IsDate(txtDataInizio.Text.Trim) Then
            txtDataInizio.BackColor = SEGNALA_OK
            txtDataInizio.Text = Format(CDate(txtDataInizio.Text), FormatoData)
            Call RicalcolaRateTotali(True)
            txtDataFine.Focus()
        Else
            txtDataInizio.BackColor = SEGNALA_KO
            txtDataInizio.Text = ""
        End If
    End Sub

    Private Sub txtDataFine_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataFine.TextChanged
        Session(SWMODIFICATO) = SWSI
        If IsDate(txtDataFine.Text.Trim) Then
            txtDataFine.BackColor = SEGNALA_OK
            txtDataFine.Text = Format(CDate(txtDataFine.Text), FormatoData)
            Call RicalcolaRateTotali()
            txtDataAccettazione.Focus()
        Else
            txtDataFine.BackColor = SEGNALA_KO
            txtDataFine.Text = ""
        End If
    End Sub

    Private Sub txtDataAccettazione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataAccettazione.TextChanged
        Session(SWMODIFICATO) = SWSI
        If IsDate(txtDataAccettazione.Text.Trim) Then
            txtDataAccettazione.BackColor = SEGNALA_OK
            txtDataAccettazione.Text = Format(CDate(txtDataAccettazione.Text), FormatoData)
            Call RicalcolaRateTotali()
            txtRiferimento.Focus()
        Else
            txtDataAccettazione.BackColor = SEGNALA_KO
            txtDataAccettazione.Text = ""
        End If
    End Sub

    Private Sub txtDataDoc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataDoc.TextChanged
        Session(SWMODIFICATO) = SWSI
        If IsDate(txtDataDoc.Text.Trim) Then
            txtDataDoc.BackColor = SEGNALA_OK
            txtDataDoc.Text = Format(CDate(txtDataDoc.Text), FormatoData)
            Call RicalcolaRateTotali()
            txtDurataNum.Focus()
        Else
            txtDataDoc.BackColor = SEGNALA_KO
            txtDataDoc.Text = ""
        End If
    End Sub

    Public Sub CallBtnModifica()
        btnModifica_Click(Nothing, Nothing)
    End Sub
    Public Sub SetLblMessDoc(ByVal strMess As String)
        lblMessDoc.Text = strMess.Trim
        If strMess.Trim <> "" Then lblMessDoc.Visible = True
    End Sub
    Public Sub SetBtnDupGen(ByVal SW As Boolean)
        If TabContainer1.ActiveTabIndex = TB2 Then
            If btnAggiorna.Enabled = True Then
                btnDuplicaDNum.Enabled = False
                btnNuovaDNum.Enabled = False
                btnGeneraAttDNum.Enabled = False
            Else
                btnDuplicaDNum.Enabled = SW
                btnNuovaDNum.Enabled = SW
                btnGeneraAttDNum.Enabled = SW
            End If
        Else
            btnDuplicaDNum.Enabled = False
            btnNuovaDNum.Enabled = False
            btnGeneraAttDNum.Enabled = False
        End If
    End Sub
    Public Sub btnbtnGeneraAttDNumColorRED(ByVal SW As Boolean)
        If SW Then
            btnGeneraAttDNum.BackColor = SEGNALA_KO
            Session(CSTNONCOMPLETO) = SWSI
            lblMessDoc.Text = "Generare i periodi Attività<br><br>Per non rigenerare i periodi usare la funzione Cambia Stato del Contratto<br>Oppure<br>Annullare le modifiche" : lblMessDoc.Visible = True
        Else
            btnGeneraAttDNum.BackColor = btnNuovo.BackColor
            Session(CSTNONCOMPLETO) = SWNO
        End If
    End Sub
    
    Private Sub btnDuplicaDNum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDuplicaDNum.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (btnDuplicaDNum_Click)")
            Exit Sub
        End If
        If TabContainer1.ActiveTabIndex = TB2 Then
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = "OKDuplicaApp"
            Dim strMess As String = "Confermi la creazione di una nuova apparecchiatura, <br> con le stesse righe qui selezionate ?"
            ModalPopup.Show("Duplica apparecchiatura", strMess, WUC_ModalPopup.TYPE_CONFIRM)
            btnModifica_Click(Nothing, Nothing)
        Else
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPMODSCATT) = SWOPNESSUNA
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Dim strMess As String = "Selezionare l'apparecchiatura da duplicare"
            ModalPopup.Show("Duplica apparecchiatura", strMess, WUC_ModalPopup.TYPE_CONFIRM)
        End If

    End Sub
    Public Sub OKDuplicaApp()
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        If WUC_ContrattiDett1.DuplicaApp() = False Then
            Exit Sub
        End If
    End Sub
    Private Function AggDataScVis1Scad(ByRef CkMess As String) As Boolean
        AggDataScVis1Scad = True
        Dim mySWTB3 As Boolean = True
        CkMess = ""
        'giu260320 rimango dove sono
        ' ''Session("TabDoc") = TB3
        ' ''TabContainer1.ActiveTabIndex = TB3
        Try
            SetCdmDAdp()
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                mySWTB3 = False
                If CkMess.Trim <> "" Then CkMess += "<br>"
                CkMess += "Errore:IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (AggDatScVis1Scad)"
                Exit Function
            End If
            Dim DsDocDettTmp As New DSDocumenti
            DsDocDettTmp.ContrattiD.Clear()
            SqlDbSelectCmdALLAtt.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNum").Value = DBNull.Value 'GIU130320 PRENDO ANCHE LE APP. POI FILTRO - 1 'fisso per le attività per periodo
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            SqlAdapDocALLAtt.Fill(DsDocDettTmp.ContrattiD)
            '--------------------------------------------------------
            'giu120320 controllo ATTIVITA'/CHECK PER PERIODO
            Dim myCKMess = "" 'MESSGGI CONTROLLO PERIODI
            'giu120320 controllo ATTIVITA'/CHECK PER PERIODO
            Dim SaveSerie As String = ""
            Dim myNApp As Integer = 0 '210420 ERR. MANCAVA RIGA 1 DsDocDettTmp.ContrattiD.Select("DurataNum=0 AND Riga=1").Length
            For Each RowDettS In DsDocDettTmp.ContrattiD.Select("DurataNum=0", "Serie")
                If IsDBNull(RowDettS.Item("Serie")) Then
                    SaveSerie = ""
                    myNApp += 1
                ElseIf SaveSerie = RowDettS.Item("Serie").ToString.Trim Then
                    Continue For
                Else
                    myNApp += 1
                    SaveSerie = RowDettS.Item("Serie").ToString.Trim
                End If
            Next
            Dim SWChek As Boolean = True : Dim myDurataNum As String = "" : Dim pCodArt As String = "" : Dim pNVisite As Integer = 0
            myDurataNum = txtDurataNum.Text.Trim
            SWChek = True
            If myNApp = 0 Then
                SWChek = False
                If CkMess.Trim <> "" Then CkMess += "<br>"
                CkMess += "Nessuna apparecchiatura inserita."
            End If
            If IsNothing(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            ElseIf Not IsNumeric(myDurataNum) Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            ElseIf Val(myDurataNum) = 0 Then
                SWChek = False
                txtDurataNum.BackColor = SEGNALA_KO
            End If
            '-
            Dim myDurataTipo As String = DDLDurataTipo.SelectedValue.Trim
            If IsNothing(myDurataTipo) Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDurataTipo) Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            ElseIf myDurataTipo.Trim = "" Then
                SWChek = False
                DDLDurataTipo.BackColor = SEGNALA_KO
            End If
            '-
            Dim myDataInizio As String = txtDataInizio.Text.Trim
            If IsNothing(myDataInizio) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            End If
            If String.IsNullOrEmpty(myDataInizio) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            ElseIf Not IsDate(myDataInizio.Trim) Then
                SWChek = False
                txtDataInizio.BackColor = SEGNALA_KO
            End If
            '----------
            Dim arrPeriodo() As String = Nothing
            If SetArrPeriodo(arrPeriodo) = False Then
                SWChek = False
            End If
            If WUC_ContrattiDett1.GetCodVisitaDATipoCAIdPag(pCodArt, pNVisite) = False Then
                'ok proseguo lo stesso senza fare il controllo 
                DDLPagamento.BackColor = SEGNALA_KO
            ElseIf SWChek = False Then
                'PROSEGUO
            Else
                If Not IsNumeric(txtNVisite.Text.Trim) Then
                    txtNVisite.BackColor = SEGNALA_KO
                    pNVisite = 0
                ElseIf Int(txtNVisite.Text.Trim) <> pNVisite Then
                    'giu290622 txtNVisite.BackColor = SEGNALA_INFO
                    pNVisite = Int(txtNVisite.Text.Trim)
                Else
                    pNVisite = Int(txtNVisite.Text.Trim)
                End If
                'GIU200320 PER AGGIORNARELE SCADENZE CHECK/VER DAE
                Dim strErrore As String = ""
                Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
                Dim SqlConn = New System.Data.SqlClient.SqlConnection
                Dim SqlUpd_ConTScadPag = New System.Data.SqlClient.SqlCommand
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
                SqlUpd_ConTScadPag.CommandText = "[update_ConDScVisAllByIDDocRiga]"
                SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
                SqlUpd_ConTScadPag.Connection = SqlConn
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"))
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 0, "Cod_Filiale"))
                SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataSC", System.Data.SqlDbType.DateTime, 8, "DataSC"))
                If SqlConn.State <> ConnectionState.Open Then
                    SqlConn.Open()
                End If
                'giu210320 inizializzo i campu null
                For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1")
                    RowDettB.BeginEdit()
                    If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        RowDettB.Item("Cod_Filiale") = 0
                    End If
                    RowDettB.EndEdit()
                Next
                DsDocDettTmp.ContrattiD.AcceptChanges()
                '---------------------------
                Dim NVisitePeriodo As Integer = 0 : Dim SaveDataScVis As String = ""
                Dim NScadPeriodo As Integer = 0 : Dim SaveDataSc As String = ""
                Dim CheckPeriodo As String = ""
                Dim SaveRigaVis As Integer = 0 : Dim OKAggVis As Boolean = False
                Dim SaveCodFil As String = ""
                'ok
                For i = 0 To Val(myDurataNum) - 1
                    CheckPeriodo = arrPeriodo(i).Trim
                    SaveCodFil = ""
                    NScadPeriodo = 0 : SaveDataSc = ""
                    NVisitePeriodo = 0 : SaveDataScVis = ""
                    myCKMess = ""
                    SaveRigaVis = 0 : OKAggVis = False
                    'giu21320 for per destinazione merce
                    For Each RowDettBDest In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim, "Cod_Filiale")
                        If SaveCodFil = "" Then
                            SaveCodFil = RowDettBDest.Item("Cod_Filiale").ToString.Trim
                        ElseIf SaveCodFil = RowDettBDest.Item("Cod_Filiale").ToString.Trim Then
                            Continue For
                        Else
                            SaveCodFil = RowDettBDest.Item("Cod_Filiale").ToString.Trim
                        End If
                        For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim & " AND Cod_Filiale=" & RowDettBDest.Item("Cod_Filiale").ToString.Trim, "DataSc")
                            If OKAggVis = True Then
                                SaveDataSc = ""
                                SaveDataScVis = ""
                                SaveRigaVis = 0 : OKAggVis = False
                                'giu091120 segnalazione Francesca del 06/11/2020 non aggiornava la scadenza del CHECK ALLA SCADENZA REALE DELLA BATTERIA NEL 2023 Exit For
                            End If
                            If RowDettB.RowState <> DataRowState.Deleted Then
                                If IsDBNull(RowDettB.Item("DataSc")) Then
                                    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                                    myCKMess += CheckPeriodo + " - Manca Data di scadenza"
                                Else
                                    If IsDBNull(RowDettB.Item("Cod_Articolo")) Then
                                        If SaveDataSc = "" Then
                                            NScadPeriodo += 1
                                            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                            NScadPeriodo += 1
                                            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        End If
                                    ElseIf RowDettB.Item("Cod_Articolo").ToString.Trim = pCodArt.Trim And pCodArt.Trim <> "" Then
                                        NVisitePeriodo += 1
                                        If SaveDataScVis = "" Then
                                            SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                                        ElseIf CDate(SaveDataScVis).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                            SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                                        End If
                                        SaveRigaVis = RowDettB.Item("Riga")
                                    Else
                                        If SaveDataSc = "" Then
                                            NScadPeriodo += 1
                                            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                                            NScadPeriodo += 1
                                            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                                        End If
                                    End If

                                End If
                                '-
                                If SaveDataScVis <> "" And SaveDataSc <> "" Then
                                    OKAggVis = True
                                    Try
                                        SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = RowDettB.Item("IDDocumenti")
                                        SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = RowDettB.Item("DurataNum")
                                        SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = RowDettB.Item("DurataNumRiga")
                                        SqlUpd_ConTScadPag.Parameters("@Riga").Value = SaveRigaVis
                                        SqlUpd_ConTScadPag.Parameters("@Cod_Filiale").Value = RowDettBDest.Item("Cod_Filiale")
                                        'giu270322
                                        If RowDettB.Item("Cod_Articolo").ToString.Trim = pCodArt.Trim And pCodArt.Trim <> "" Then
                                            SqlUpd_ConTScadPag.Parameters("@DataSC").Value = IIf(CDate(SaveDataSc).Date < CDate(SaveDataScVis).Date, CDate(SaveDataSc), CDate(SaveDataScVis))
                                        Else
                                            SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(SaveDataSc)
                                        End If
                                        '---------
                                        SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
                                        SqlUpd_ConTScadPag.ExecuteNonQuery()
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
                                        ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                                        Exit Function
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
                                        ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                                        Exit Function
                                    End Try
                                    'GIU240320 End If
                                End If
                            End If
                        Next
                    Next
                    If (NVisitePeriodo / myNApp) <> pNVisite Then
                        If myCKMess.Trim <> "" Then myCKMess += "<br>"
                        myCKMess += CheckPeriodo + " - N° Visite diverso da quello indicato Oppure Codice Visita mancante nelle Apparecchiature."
                    End If
                    'giu220320 importante che siano sempre successive alla visita
                    ' ''If NScadPeriodo <> pNVisite And NScadPeriodo <> 0 Then
                    ' ''    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                    ' ''    myCKMess += CheckPeriodo + " - N° Scadenze diverso dal N° Visite previste (Accorpare date)"
                    ' ''End If
                    If myCKMess.Trim <> "" Then
                        If CkMess.Trim <> "" Then CkMess += "<br>"
                        CkMess += myCKMess
                    End If
                Next
                '-----------------------------------------------
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
            End If
            '-----------------------------------------------
        Catch ex As Exception
            mySWTB3 = False
            If CkMess.Trim <> "" Then CkMess += "<br>"
            CkMess += "Errore:Controllo Scadenze attività. (AggDataScVis1Scad): " & ex.Message
            Exit Function
        End Try
    End Function

    Private Sub btnGeneraAttDNum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGeneraAttDNum.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (btnGeneraAttDNum_Click)")
            Exit Sub
        End If
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        '-
        Session(SWOP) = SWOPNESSUNA
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKGeneraAttPeriodo"
        Dim strMess As String = "Confermi la creazione delle attività per periodo ?<br><b>" + _
        "ATTENZIONE, verranno rigenerati solo i periodi con attività non ancora Evase<br>" + _
        "ATTENZIONE, NON E' POSSIBIBLE, cambiare il tipo di fatturazione a Unica Rata con delle attività già evase, perchè l'importo è solo sul 1° Periodo<br>" + _
        "di conseguenza il Totale contratto sarà errato.</b>"
        ModalPopup.Show("Genera attività per periodo", strMess, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub OKGeneraAttPeriodo()
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        Session("TabDoc") = TB2
        TabContainer1.ActiveTabIndex = TB2
        If WUC_ContrattiDett1.GeneraAttPeriodo() = True Then
            'giu200320 giu220320 qui nessuna segnalazione prima deve aggiornare i dettagli
            Dim CKMess As String = ""
            Call AggDataScVis1Scad(CKMess)
            If txtNumero.BackColor = SEGNALA_KO Then 'GIU260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "!!!ERRORE!!! N.Documento !!!VERIFICARE !!!"
            ElseIf DDLPagamento.BackColor = SEGNALA_KO Then
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Manca, il Tipo Contratto, <br>Non è stato possibile eseguire i controlli della Scadenze Attività."
            ElseIf txtNVisite.BackColor = SEGNALA_KO Then
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "N° Visite nel singolo periodo, <br>Codice Visita mancante nelle Apparecchiature oppure diverso dal n° visite previste nel periodo."
            End If
            ' ''If CKMess.Trim <> "" Then
            ' ''    WUC_ContrattiDett1.SetStatoDoc5()
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("Attenzione", CKMess, WUC_ModalPopup.TYPE_ALERT)
            ' ''    Exit Sub
            ' ''End If
            '---------
            If CKMess.Trim <> "" Then 'GIU290722
                btnbtnGeneraAttDNumColorRED(True)
                lblMessDoc.Text = "Generare i periodi Attività<br>" + CKMess.Trim : lblMessDoc.Visible = True
            Else
                btnbtnGeneraAttDNumColorRED(True)
            End If
            '-
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'giu220320 ModalPopup.Show("Generazione attività per periodo", "Operazione terminata con successo.<BR>" + CKMess.Trim, WUC_ModalPopup.TYPE_INFO)
            If CKMess.Trim <> "" Then 'GIU290722
                ModalPopup.Show("Generazione attività per periodo", "Operazione terminata con successo.<br>Controllare messaggi", WUC_ModalPopup.TYPE_INFO)
            Else
                ModalPopup.Show("Generazione attività per periodo", "Operazione terminata con successo.", WUC_ModalPopup.TYPE_INFO)
            End If
            '-
            btnModifica_Click(Nothing, Nothing)
            btnAggiorna.BackColor = SEGNALA_KO
            btnGeneraAttDNum.BackColor = btnNuovo.BackColor
            Session(CSTNONCOMPLETO) = SWSI
        Else 'SEGNALATO PRIMA
            WUC_ContrattiDett1.SetStatoDoc5()
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Generazione attività per periodo", "Operazione terminata con ERRORI.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub

    Public Function SaveScadenze() As Boolean
        SaveScadenze = WUC_ContrattiSpeseTraspTot1.SaveScadenze()
    End Function
    Private Sub btnNuovaDNum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovaDNum.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        Session(SWOP) = SWOPMODIFICA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        CampiSetEnabledToT(True)
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session("TabDoc") = TB2
        TabContainer1.ActiveTabIndex = TB2
        Call WUC_ContrattiDett1.NuovaApp()
    End Sub

    Public Sub TD1ReBuildDett()
        WUC_ContrattiDett1.TD1ReBuildDett(True)
    End Sub
    Public Sub ReBuildDett()
        WUC_ContrattiDett1.ReBuildDett()
    End Sub

    Private Sub txtNOrdine_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNOrdine.TextChanged, _
        txtNOrdineRev.TextChanged
        If txtNOrdine.Text.Trim = "" Then Exit Sub
        Dim myIDNOrdine As String = ""
        Dim strErrore As String = ""
        Dim strCollegato As String = ""
        If CheckNumDocOC(txtNOrdine.Text.Trim, txtNOrdineRev.Text.Trim, myIDNOrdine, strErrore, strCollegato) = False Then
            Exit Sub
        End If
        ' ''btnDaOrdine.Visible = True
        If Session(CSTNUOVOCADACA) = SWSI Then
            Session(CSTNUOVOCADACA) = myIDNOrdine.Trim
        ElseIf Session(CSTNUOVOCADACA) = SWNO Then
            Session(CSTNUOVOCADAOC) = myIDNOrdine.Trim
        ElseIf IsNumeric(Session(CSTNUOVOCADACA)) Then
            Session(CSTNUOVOCADACA) = myIDNOrdine.Trim
        ElseIf IsNumeric(Session(CSTNUOVOCADAOC)) Then
            Session(CSTNUOVOCADAOC) = myIDNOrdine.Trim
        End If
        'giu240122 richiesta francesca CheckNumDocOC = False
        If strCollegato.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo N° Ordine", strCollegato, WUC_ModalPopup.TYPE_INFO)
        End If
       
    End Sub
    Private Function CheckNumDocOC(ByVal _NOrdine As String, ByVal _NOrdineRev As String, ByRef _IDNOrdine As String, ByRef strErrore As String, ByRef strCollegato As String) As Boolean
        CheckNumDocOC = False
        Dim strSQL As String = ""
        strCollegato = ""
        If Not IsNumeric(_NOrdineRev.Trim) Then _NOrdineRev = "0"
        strSQL = "Select IDDocumenti From DocumentiT WHERE Tipo_Doc = '" & SWTD(TD.OrdClienti) & "' AND " & _
                 "Numero='" & _NOrdine.Trim & "' AND RevisioneNDoc=" & _NOrdineRev.Trim & ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CheckNumDocOC = True
                    _IDNOrdine = ds.Tables(0).Rows(0).Item("IDDocumenti")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Controllo N° Ordine", "Attenzione, N° Ordine inesistente.", WUC_ModalPopup.TYPE_INFO)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo N° Ordine", "Attenzione, N° Ordine inesistente.", WUC_ModalPopup.TYPE_INFO)
                Exit Function
            End If
            '230321 CONTROLLO CHE NON SIA GIA' COLLEGATO
            ds.Clear()
            strSQL = "Select Numero From ContrattiT WHERE Refint = -" & _IDNOrdine.Trim & ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    'giu240122 richiesta francesca CheckNumDocOC = False
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ' ''ModalPopup.Show("Controllo N° Ordine", "Attenzione, N° Ordine già collegato al Contratto N° " & ds.Tables(0).Rows(0).Item("Numero").ToString.Trim + "<br>Proseguo comunque", WUC_ModalPopup.TYPE_INFO)
                    strCollegato = "N° Ordine già collegato al Contratto N° " & ds.Tables(0).Rows(0).Item("Numero").ToString.Trim + " <br> Proseguo comunque"
                End If
                Exit Function
            Else
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo N° Ordine", "Errore ricerca N° Ordine: " & strErrore, WUC_ModalPopup.TYPE_INFO)
            Exit Function
        Finally
            ObjDB = Nothing
        End Try

    End Function

    Private Sub btnDaOrdine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDaOrdine.Click
        Dim strCollegato As String = ""
        If lblNOrdine.Visible = False Then
            lblNOrdine.Visible = True
            txtNOrdine.Visible = True
            txtNOrdineRev.Visible = True
            btnDaOrdine.ForeColor = Drawing.Color.White
            btnDaOrdine.BackColor = Drawing.Color.DarkRed
            txtNOrdine.Focus()
            Exit Sub
        Else
            If txtNOrdine.Text.Trim <> "" Then
                Dim strErrore As String = ""
                Dim myIDNOrdine As String = ""
                If CheckNumDocOC(txtNOrdine.Text.Trim, txtNOrdineRev.Text.Trim, myIDNOrdine, strErrore, strCollegato) = False Then
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                    txtNOrdine.Focus()
                    Exit Sub
                End If
                '--- IDDocumenti 
                Session(CSTNUOVOCADAOC) = myIDNOrdine
                If IsNothing(myIDNOrdine) Then
                    myIDNOrdine = ""
                End If
                If String.IsNullOrEmpty(myIDNOrdine) Then
                    myIDNOrdine = ""
                End If
                If myIDNOrdine = "" Or Not IsNumeric(myIDNOrdine) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Nuovo Contratto da Ordine", "Indicare N° Ordine da cui caricare i dati.", WUC_ModalPopup.TYPE_INFO)
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                    txtNOrdine.Focus()
                    Exit Sub
                End If
            Else
                lblNOrdine.Visible = False
                txtNOrdine.Visible = False
                txtNOrdineRev.Visible = False
                btnDaOrdine.BackColor = btnNuovo.BackColor
                btnDaOrdine.ForeColor = Drawing.Color.Black
                Exit Sub
            End If

            lblNOrdine.Visible = False
            txtNOrdine.Visible = False
            txtNOrdineRev.Visible = False
            btnDaOrdine.BackColor = btnNuovo.BackColor
            btnDaOrdine.ForeColor = Drawing.Color.Black
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "OKDocTDaOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Carica dati da Ordine", "Confermi l'aggiornamento di tutti i dati dall'Ordine  ?" _
                    & "<br>N° Ordine:<strong><span>" & FormattaNumero(txtNOrdine.Text) _
                    & "<br><br>ATTENZIONE, saranno persi tutti i dettagli Apparecchiature" _
                    & "<br><br>Attenzione, l'Ordine verrà EVASO automaticamente</span></strong>" _
                    & "<br>se lo stato attuale è Da Evadere" & IIf(strCollegato.Trim <> "", "<br><br>" + strCollegato, ""), WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    'giu230321
    Private Sub btnCollegaOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCollegaOC.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session("TabDoc") = TB3
            TabContainer1.ActiveTabIndex = TB3
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session("TabDoc") = TB2
            TabContainer1.ActiveTabIndex = TB2
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim strCollegato As String = ""
        If lblNOrdine.Visible = False Then
            lblNOrdine.Visible = True
            txtNOrdine.Visible = True
            txtNOrdineRev.Visible = True
            btnCollegaOC.ForeColor = Drawing.Color.White
            btnCollegaOC.BackColor = Drawing.Color.DarkRed
            txtNOrdine.Focus()
            Exit Sub
        Else
            If txtNOrdine.Text.Trim <> "" Then
                Dim strErrore As String = ""
                Dim myIDNOrdine As String = ""
                If CheckNumDocOC(txtNOrdine.Text.Trim, txtNOrdineRev.Text.Trim, myIDNOrdine, strErrore, strCollegato) = False Then
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                    txtNOrdine.Focus()
                    Exit Sub
                End If
                '--- IDDocumenti 
                Session(CSTNUOVOCADACA) = myIDNOrdine
                If IsNothing(myIDNOrdine) Then
                    myIDNOrdine = ""
                End If
                If String.IsNullOrEmpty(myIDNOrdine) Then
                    myIDNOrdine = ""
                End If
                If myIDNOrdine = "" Or Not IsNumeric(myIDNOrdine) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Collega N° Ordine a Contratto", "Indicare N° Ordine da collegare.", WUC_ModalPopup.TYPE_INFO)
                    lblNOrdine.Visible = True
                    txtNOrdine.Visible = True
                    txtNOrdineRev.Visible = True
                    txtNOrdine.Focus()
                    Exit Sub
                End If
            Else
                lblNOrdine.Visible = False
                txtNOrdine.Visible = False
                txtNOrdineRev.Visible = False
                btnCollegaOC.BackColor = btnNuovo.BackColor
                btnCollegaOC.ForeColor = Drawing.Color.Black
                Exit Sub
            End If

            lblNOrdine.Visible = False
            txtNOrdine.Visible = False
            txtNOrdineRev.Visible = False
            btnCollegaOC.BackColor = btnNuovo.BackColor
            btnCollegaOC.ForeColor = Drawing.Color.Black
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "OKCollegaOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Collega N° Ordine al Contratto", "Confermi il collegamento N° Ordine al Contratto ?" _
                    & "<br>N° Ordine:<strong><span>" + FormattaNumero(txtNOrdine.Text) _
                    & "<br><br>Attenzione, l'Ordine verrà EVASO automaticamente</span></strong>" _
                    & "<br>se lo stato attuale è Da Evadere" & IIf(strCollegato.Trim <> "", "<br><br>" + strCollegato, ""), WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    '-
    Public Sub OKCollegaOC()
        Dim strErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Collega N° Ordine a Contratto", "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myIDNOrdine As String = Session(CSTNUOVOCADACA)
        Session(CSTNUOVOCADACA) = myIDNOrdine
        If IsNothing(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If String.IsNullOrEmpty(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If myIDNOrdine = "" Or Not IsNumeric(myIDNOrdine) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Collega N° Ordine a Contratto", "Errore N° Ordine: ID sconosciuto.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        '-
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        If UPGDocTDaOCColl(myID, myIDNOrdine, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Collega N° Ordine a Contratto", "Errore: " & strErrore.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        '-
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        SetCdmDAdp()
        DsDocT.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlAdapDoc.Fill(DsDocT.ContrattiT)

        dvDocT = New DataView(DsDocT.ContrattiT)
        Session("dvDocT") = dvDocT
        Session("SqlAdapDoc") = SqlAdapDoc
        Session("DsDocT") = DsDocT
        Session("SqlDbSelectCmd") = SqlDbSelectCmd
        Session("SqlDbInserCmd") = SqlDbInserCmd
        Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
        Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
        '
        Try
            If dvDocT.Count > 0 Then
                CampiSetEnabledToT(False)
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                PopolaTxtDocT()
                'giu271023 XKE'??? SE HO MODIFICATO SOLO REFINT E DES REFINT Call TD1ReBuildDett()
                WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, "")
                btnElimina.Enabled = True
                btnModifica.Enabled = True
                btnCollegaOC.Enabled = True
                btnDuplicaDNum.Enabled = True
                btnNuovaDNum.Enabled = True
                btnGeneraAttDNum.Enabled = True
                btnStampa.Enabled = True : btnVerbale.Enabled = True
                If WUC_ContrattiSpeseTraspTot1.CKAttEvPagEv = True And btnGeneraAttDNum.BackColor <> SEGNALA_KO Then
                    ' ''btnDaOrdine.Visible = False
                    btnDuplicaDNum.Enabled = False
                    btnNuovaDNum.Enabled = False
                    btnGeneraAttDNum.Enabled = False
                End If
            Else
                CampiSetEnabledToT(False)
                BtnSetEnabledTo(False)
                AzzeraTxtDocT()
                WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
                Session(SWOP) = SWOPNESSUNA
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End If
            Session(SWMODIFICATO) = SWNO
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            '
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Collega N° Ordine a Contratto", "Errore: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Collega N° Ordine a Contratto", "Operazione terminata con successo." _
                        & "<br><br><strong><span>Attenzione, l'Ordine è stato EVASO automaticamente</span></strong>" _
                        & "<br>se lo stato attuale è Da Evadere" & IIf(strErrore.Trim <> "", "<br><br>" + strErrore, ""), WUC_ModalPopup.TYPE_INFO)
        Exit Sub
    End Sub
    
    'giu030420
    Public Sub OKDocTDaOCOC()
        Dim strErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myIDNOrdine As String = Session(CSTNUOVOCADAOC)
        ' ''If CheckNumDocOC(txtNOrdine.Text.Trim, txtNOrdineRev.Text.Trim, myIDNOrdine, strErrore) = False Then
        ' ''    Exit Sub
        ' ''End If
        '--- IDDocumenti 
        Session(CSTNUOVOCADAOC) = myIDNOrdine
        If IsNothing(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If String.IsNullOrEmpty(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If myIDNOrdine = "" Or Not IsNumeric(myIDNOrdine) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore N° Ordine: ID sconosciuto.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        'Update_ConTByIDDocOrdine
        ' ''btnDaOrdine.Visible = False
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        If UPGDocTDaOC(myID, myIDNOrdine, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: " & strErrore.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        '-
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        SetCdmDAdp()
        DsDocT.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlAdapDoc.Fill(DsDocT.ContrattiT)

        dvDocT = New DataView(DsDocT.ContrattiT)
        Session("dvDocT") = dvDocT
        Session("SqlAdapDoc") = SqlAdapDoc
        Session("DsDocT") = DsDocT
        Session("SqlDbSelectCmd") = SqlDbSelectCmd
        Session("SqlDbInserCmd") = SqlDbInserCmd
        Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
        Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
        '
        Try
            If dvDocT.Count > 0 Then
                BtnSetEnabledTo(False)
                If Session(SWOP) = SWOPELIMINA Then
                    CampiSetEnabledToT(False)
                    btnAnnulla.Enabled = True
                    btnElimina.Enabled = True
                Else
                    CampiSetEnabledToT(True)
                    btnAnnulla.Enabled = True
                    btnAggiorna.Enabled = True
                End If
                PopolaTxtDocT()
                Call TD1ReBuildDett()
                WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, "")
            Else
                CampiSetEnabledToT(False)
                BtnSetEnabledTo(False)
                AzzeraTxtDocT()
                WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
                Session(SWOP) = SWOPNESSUNA
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End If
            ' ''Session(SWMODIFICATO) = SWNO
            ' ''Session(SWOPDETTDOC) = SWOPNESSUNA
            ' ''Session(SWOPDETTDOCR) = SWOPNESSUNA
            ' ''Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            '
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Nuovo Contratto da Ordine", "Operazione terminata con successo." _
                        & "<br><br><strong><span>Attenzione, l'Ordine è stato EVASO automaticamente</span></strong>" _
                        & "<br>se lo stato attuale è Da Evadere" & IIf(strErrore.Trim <> "", "<br><br>" + strErrore, ""), WUC_ModalPopup.TYPE_INFO)
        Exit Sub
    End Sub
    '@@@030420
    Public Sub OKDocTDaOC()
        Dim strErrore As String = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myIDNOrdine As String = ""
        Dim strCollegato As String = ""
        If CheckNumDocOC(txtNOrdine.Text.Trim, txtNOrdineRev.Text.Trim, myIDNOrdine, strErrore, strCollegato) = False Then
            Exit Sub
        End If
        '--- IDDocumenti 
        Session(CSTNUOVOCADAOC) = myIDNOrdine
        If IsNothing(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If String.IsNullOrEmpty(myIDNOrdine) Then
            myIDNOrdine = ""
        End If
        If myIDNOrdine = "" Or Not IsNumeric(myIDNOrdine) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore N° Ordine: ID sconosciuto.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        'Update_ConTByIDDocOrdine
        ' ''btnDaOrdine.Visible = False
        lblNOrdine.Visible = False
        txtNOrdine.Visible = False
        txtNOrdineRev.Visible = False
        If UPGDocTDaOC(myID, myIDNOrdine, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: " & strErrore.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        '-
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        SetCdmDAdp()
        DsDocT.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlAdapDoc.Fill(DsDocT.ContrattiT)

        dvDocT = New DataView(DsDocT.ContrattiT)
        Session("dvDocT") = dvDocT
        Session("SqlAdapDoc") = SqlAdapDoc
        Session("DsDocT") = DsDocT
        Session("SqlDbSelectCmd") = SqlDbSelectCmd
        Session("SqlDbInserCmd") = SqlDbInserCmd
        Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
        Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
        '
        Try
            If dvDocT.Count > 0 Then
                BtnSetEnabledTo(False)
                If Session(SWOP) = SWOPELIMINA Then
                    CampiSetEnabledToT(False)
                    btnAnnulla.Enabled = True
                    btnElimina.Enabled = True
                Else
                    CampiSetEnabledToT(True)
                    btnAnnulla.Enabled = True
                    btnAggiorna.Enabled = True
                End If
                PopolaTxtDocT()
                Call TD1ReBuildDett()
                WUC_ContrattiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                WUC_ContrattiDett1.PopolaLBLTotaliDoc(dvDocT, "")
            Else
                CampiSetEnabledToT(False)
                BtnSetEnabledTo(False)
                AzzeraTxtDocT()
                WUC_ContrattiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                WUC_ContrattiDett1.AzzeraLBLTotaliDoc()
                Session(SWOP) = SWOPNESSUNA
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End If
            ' ''Session(SWMODIFICATO) = SWNO
            ' ''Session(SWOPDETTDOC) = SWOPNESSUNA
            ' ''Session(SWOPDETTDOCR) = SWOPNESSUNA
            ' ''Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            '
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo Contratto da Ordine", "Errore: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Nuovo Contratto da Ordine", "Operazione terminata con successo." _
                        & "<br><br><strong><span>Attenzione, l'Ordine è stato EVASO automaticamente</span></strong>" _
                        & "<br>se lo stato attuale è Da Evadere" & IIf(strErrore.Trim <> "", "<br><br>" + strErrore, "") _
                        & IIf(strCollegato.Trim <> "", "<br><br>" + strCollegato, ""), WUC_ModalPopup.TYPE_INFO)
        Exit Sub
    End Sub
    Private Function UPGDocTDaOC(ByVal _myID As String, ByVal _myIDDocOCIN As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        Dim SqlUpd_ConTScadPag = New System.Data.SqlClient.SqlCommand
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
        '---------------------------
        Try
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '--- Parametri
            SqlUpd_ConTScadPag.CommandText = "[Update_ConTByIDDocOrdine]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            '
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = _myID
            SqlUpd_ConTScadPag.Parameters("@IDDocIN").Value = _myIDDocOCIN
            SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
            SqlUpd_ConTScadPag.ExecuteNonQuery()
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
        End Try
    End Function
    'giu230321
    Private Function UPGDocTDaOCColl(ByVal _myID As String, ByVal _myIDDocOCIN As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        Dim SqlUpd_ConTScadPag = New System.Data.SqlClient.SqlCommand
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
        '---------------------------
        Try
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '--- Parametri
            SqlUpd_ConTScadPag.CommandText = "[Update_ConTByIDDocOCColl]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            '
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = _myID
            SqlUpd_ConTScadPag.Parameters("@IDDocIN").Value = _myIDDocOCIN
            SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
            SqlUpd_ConTScadPag.ExecuteNonQuery()
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
        End Try
    End Function

End Class