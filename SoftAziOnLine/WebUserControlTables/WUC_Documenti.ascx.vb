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
Imports System.IO 'giu150320

Partial Public Class WUC_Documenti
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

    Private SqlDbInserCmdForInsert As SqlCommand 'Pier311011
    Private SqlDbInserCmdForInsertL As SqlCommand 'GIU291111

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = "" 'giu260312
    Dim SWFatturaPA As Boolean = False 'giu230714
    Dim SWSplitIVA As Boolean = False 'giu221217
    Dim SWTB0 As Boolean = False
    Dim SWTB1 As Boolean = False
    Dim SWTB2 As Boolean = False
    'giu130219
    Dim SWRifDoc As Boolean = True
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'giu150320
        LnkConfOrdine.Visible = False
        LnkListaCarico.Visible = False
        LnkStampa.Visible = False
        '-
        Session(ERROREALL) = ""
        ModalPopup.WucElement = Me
        WUC_DocumentiDett1.WucElement = Me
        WUC_DocumentiSpeseTraspTot1.WucElement = Me
        '-
        TipoFatturazione.WucElement = Me
        WFP_Agenti1.WucElement = Me
        WFP_AnagrProvv_Insert1.WucElement = Me
        WFP_Anagrafiche_Modify1.WucElement = Me
        WFP_DestCliFor1.WucElement = Me
        WFPElencoCli.WucElement = Me
        WFPElencoFor.WucElement = Me
        WFPElencoDestCF.WucElement = Me
        WFP_BancheIBAN1.WucElement = Me
        WFPElencoAliquotaIVA.WucElement = Me
        WFP_LeadSource1.WucElement = Me
        'giu301220
        If String.IsNullOrEmpty(Session(CSTLEAD)) Then
            Session(CSTLEAD) = SWNO
        End If
        If Session(CSTLEAD) = SWNO Then
            lblLead.Visible = False
            btnLead.Visible = False
            DDLLead.Visible = False
        End If
        'GIU060814 IN CKCSTTipoDoc VIENE INIALIZZATA SEMPRE LA VARIABILE SWFatturaPA
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.Load (1))")
            Exit Sub
        End If
        '-
        If Session(CSTTIPODOC) <> SWTD(TD.FatturaCommerciale) And _
           Session(CSTTIPODOC) <> SWTD(TD.FatturaAccompagnatoria) And _
           Session(CSTTIPODOC) <> SWTD(TD.FatturaScontrino) Then 'GIU080722
            chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False
            chkFatturaAC.Checked = False : chkFatturaAC.Visible = False
            chkScGiacenza.Checked = False : chkScGiacenza.Visible = False
            chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False 'NON LI RIATTIVO 
            ChkAcconto.AutoPostBack = False : ChkAcconto.Checked = False : ChkAcconto.Visible = False 'NON RIATTIVO
        End If
        'giu281221 Zibordi giu080722 Zibordi file elenco spedizioni
        If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
            lblPagAntEff.Visible = True
            optPagAnticipato.AutoPostBack = True
            optPagEffettuato.AutoPostBack = True
            optPagLista.AutoPostBack = True
            optPagSconosciuto.AutoPostBack = True
            '-
            optPagAnticipato.Visible = True
            optPagEffettuato.Visible = True
            optPagLista.Visible = True
            optPagSconosciuto.Visible = True
        Else
            optPagAnticipato.AutoPostBack = False
            optPagEffettuato.AutoPostBack = False
            optPagLista.AutoPostBack = False
            optPagSconosciuto.AutoPostBack = False
            '-
            lblPagAntEff.Visible = False
            optPagAnticipato.Visible = False
            optPagEffettuato.Visible = False
            optPagLista.Visible = False
            optPagSconosciuto.Visible = False
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
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO (Documenti.Load): " & strEser.Trim)
            Exit Sub
        End If
        If Val(strEser) > 2018 Then
            If Left(Session(CSTTIPODOC), 1) <> "C" And _
                Left(Session(CSTTIPODOC), 1) <> "M" And _
                Left(Session(CSTTIPODOC), 1) <> "S" And _
                Left(Session(CSTTIPODOC), 1) <> "P" And _
                Left(Session(CSTTIPODOC), 1) <> "I" And _
                Session(CSTTIPODOC) <> "OF" Then 'GIU230120 GIU081121
                SWRifDoc = True
            Else
                SWRifDoc = False
            End If
        Else
            SWRifDoc = False
        End If
        If SWRifDoc = False Then
            txtRiferimento.MaxLength = 150
            txtCCommessa.Visible = False : lblCCommessa.Visible = False
        Else
            txtRiferimento.MaxLength = 20
            txtCCommessa.Visible = True : lblCCommessa.Visible = True
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
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            ' ''txtCodCliForFilProvv.Enabled = False
            ' ''btnCercaAnagrafica.Visible = False
            ' ''btnModificaAnagrafica.Visible = False
            ' ''btnInsAnagrProvv.Visible = False
            ' ''btnCercaBanca.Visible = False
            btnNuovo.Visible = False
            btnElimina.Visible = False
            ' ''lblStampe.Visible = False
            btnConfOrdine.Visible = False
            If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                btnStampa.Visible = False
            End If
            LnkSCEC.Visible = False 'GIU220113
            LnkRegimeIVA.Visible = False 'GIU090814
        Else
            LnkSCEC.Visible = True 'GIU220113
            LnkRegimeIVA.Visible = True 'GIU090814
        End If
        '----------------------------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDSCliForFilProvv.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSPagamenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSLead.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSBancheIBAN.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSAgenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSListini.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSValuta.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu150113
        ' ''SqlDSEstrConto.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge) 'GIU260614 ???
        '--
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
        'giu120321 ---------------------------------------------------------------
        Dim myChiamatoDa As String = ""
        Dim myCoGe As String = ""
        If (Not IsPostBack) Then
            If Session(SWOP) = SWOPNUOVO Then
                myChiamatoDa = Session(CSTChiamatoDa)
                'Session(CSTChiamatoDa) = "WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti"
                If IsNothing(myChiamatoDa) Then
                    myChiamatoDa = ""
                ElseIf String.IsNullOrEmpty(myChiamatoDa) Then
                    myChiamatoDa = ""
                End If
                If InStr(myChiamatoDa.Trim.ToUpper, "WF_ANAGRAFICACLIENTI") > 0 Then
                    myCoGe = Session(COD_CLIENTE)
                    If IsNothing(myCoGe) Then
                        myCoGe = ""
                    ElseIf String.IsNullOrEmpty(myCoGe) Then
                        myCoGe = ""
                    End If
                    If String.IsNullOrEmpty(myCoGe) Then
                        myCoGe = ""
                    End If
                    If myCoGe = "" Or Not IsNumeric(myCoGe) Then
                        myCoGe = ""
                    End If
                    If myCoGe.Trim <> "" Then
                        txtCodCliForFilProvv.AutoPostBack = False
                        txtCodCliForFilProvv.Text = myCoGe
                        txtCodCliForFilProvv.AutoPostBack = True
                        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
                    End If
                End If
            End If
        End If
        '--------------------------------------------------------------------------------
        Session(CSTTABCLIFOR) = TabCliFor
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim 'giu240615
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
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
        btnEtichette.Text = "Etichette Sovracollo"
        btnListaCarico.Text = "Lista di carico"
        btnCaricoLotti.Text = "Carico Lotti con Lettore"
        'giu111012 controllo AnagrProvv solo per i preventivi
        'Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) NON ABILITATO ETICHETTE PER I FORNI
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
            (Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) And chkFatturaAC.Checked) Then 'GIU080722 giu310323
            btnEtichette.Visible = True
            btnCaricoLotti.Visible = True 'giu220323
        Else
            btnEtichette.Visible = False
            btnCaricoLotti.Visible = False 'giu220323
        End If
        If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
            btnListaCarico.Visible = True
        Else
            btnListaCarico.Visible = False
        End If
        If Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Then
            LnkSCEC.Visible = False 'GIU080722
            lblStampe.Visible = False 'nessuna stampa
            btnStampa.Visible = False
            btnListaCarico.Visible = False
            btnEtichette.Visible = False
            btnCaricoLotti.Visible = False
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then 'GIU080722
            LnkSCEC.Visible = False
        End If
        '-

        If (Not IsPostBack) Then
            'GIU040914 SE è NUOVO FORZO SEMPRE IL RICALCOLO (Segnalato da Zibordi)
            If Session(SWOP) = SWOPNUOVO Or Session(SWOP) = SWOPELIMINA Then
                Session("SWOKRicalcolaGiac") = True
            Else
                Session("SWOKRicalcolaGiac") = False
                Session(COD_CLIENTE) = String.Empty
            End If
            '------------------------------------------------
            Session(CSTSWRbtnTD) = Session(CSTTIPODOC) 'GIU090814
            Session(CSTCODCOGE) = String.Empty
            Session(CSTCODFILIALE) = String.Empty
            Session("CKTelOBB") = "" 'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
            Session("CKTelOBBSede") = "" 'giu060423
            Session("CKRagSocSede35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
            Session("CKRiferimSede35") = "" 'GIU060423 CONTROLLO riferimento >35
            Session("CKRagSoc35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
            Session("CKRiferim35") = "" 'GIU060423 CONTROLLO riferimento >35
            Session("CKPIVACF") = "" 'GIU200423
            Session("CKIPAObb") = "" 'GIU200423
            Session("SWNOTEAVVISO") = ""
            '---------
            SetCdmDAdp()

            'IMPOSTAZIONE SPECIFICHE PER TIPO DOCUMENTO
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
               Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Or _
               Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Or _
               Session(CSTTIPODOC) = SWTD(TD.OrdDepositi) Then
                txtRevNDoc.Visible = True : txtRevNDoc.AutoPostBack = True 'giu300419
                If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                    btnConfOrdine.Text = "Conferma Ordine"
                    If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                        btnConfOrdine.Visible = False
                    Else
                        btnStampa.Visible = False
                        btnConfOrdine.Visible = True
                    End If
                Else
                    btnConfOrdine.Visible = False
                End If
            Else
                txtRevNDoc.Visible = False : txtRevNDoc.AutoPostBack = False 'giu300419
                btnConfOrdine.Visible = False
            End If
            'giu201211 Impostazione btnStampa per i MM SM CM
            If Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Then
                txtRevNDoc.Visible = False : txtRevNDoc.AutoPostBack = False 'giu300419
                btnStampa.Height = Unit.Pixel(50) : btnStampa.Width = Unit.Pixel(108)
                btnStampa.Text = "Stampa movim. (Si Lotti)"
            End If
            '-----------------------------------------------
            'giu191111 GIU030212
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            '------------------------------------------
            If Session(SWOP) = SWOPNUOVO Then
                DsDocT.Clear()
                AzzeraTxtDocT()
                WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2()
                txtNumero.AutoPostBack = False : txtRevNDoc.AutoPostBack = False
                If GetNewNumDoc() = False Then Exit Sub 'giu260312
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = SWFatturaPA 'giu070814
                Session(CSTFATTURAPA) = SWFatturaPA
                chkFatturaPA.AutoPostBack = True
                ChkAcconto.Checked = False
                If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then 'GIU080722 ACCONTO/SALDO SOLO PER LE FATTURE
                    optPagAnticipato.AutoPostBack = False
                    optPagEffettuato.AutoPostBack = False
                    optPagLista.AutoPostBack = False
                    optPagSconosciuto.AutoPostBack = False
                    '-
                    optPagSconosciuto.Checked = True
                    optPagAnticipato.Checked = False
                    optPagEffettuato.Checked = False
                    optPagLista.Checked = False
                    '-
                    optPagAnticipato.AutoPostBack = True
                    optPagEffettuato.AutoPostBack = True
                    optPagLista.AutoPostBack = True
                    optPagSconosciuto.AutoPostBack = True
                    '-
                End If
                '-
                If Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then 'GIU080722
                    If Session(CSTFCACSA) = SWSI Then
                        ChkAcconto.Checked = True
                    End If
                End If
                DDLTipoRapp.SelectedIndex = 0
                txtRevNDoc.Text = "0"
                txtNumero.AutoPostBack = True : txtRevNDoc.AutoPostBack = True
                If ControllaDataDoc1().Date = DATANULL Then
                    txtDataDoc.Text = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData)
                ElseIf Year(Now.Date) <> Val(Session(ESERCIZIO).ToString.Trim) Then
                    txtDataDoc.Text = Format(CDate(ControllaDataDoc1()).Date, FormatoData) 'GIU110118 DA now a ultimo movimento
                Else 'giu010318
                    txtDataDoc.Text = Format(Now.Date, FormatoData)
                End If
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
                Session(CSTSTATODOC) = "0"
                'giu281114 valorizzato in GetNewDoc
                txtListino.AutoPostBack = False
                txtListino.Text = Session(IDLISTINO)
                txtListino.AutoPostBack = True
                lblCodValuta.Text = Session(CSTVALUTADOC)
                '----------------------------------------
                txtTipoFatt.AutoPostBack = False
                txtTipoFatt.Text = GetParamGestAzi(Session(ESERCIZIO)).CodTipoFatt
                txtTipoFatt.AutoPostBack = True
                'giu091020
                ddlMagazzino.AutoPostBack = False
                PosizionaItemDDL("1", ddlMagazzino, True)
                ddlMagazzino.AutoPostBack = True
                'giu100321
                myChiamatoDa = Session(CSTChiamatoDa)
                'Session(CSTChiamatoDa) = "WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti"
                If IsNothing(myChiamatoDa) Then
                    myChiamatoDa = ""
                ElseIf String.IsNullOrEmpty(myChiamatoDa) Then
                    myChiamatoDa = ""
                End If
                If InStr(myChiamatoDa.Trim.ToUpper, "WF_ANAGRAFICACLIENTI") > 0 Then
                    myCoGe = Session(COD_CLIENTE)
                    If IsNothing(myCoGe) Then
                        myCoGe = ""
                    ElseIf String.IsNullOrEmpty(myCoGe) Then
                        myCoGe = ""
                    End If
                    If String.IsNullOrEmpty(myCoGe) Then
                        myCoGe = ""
                    End If
                    If myCoGe = "" Or Not IsNumeric(myCoGe) Then
                        myCoGe = ""
                    End If
                    If myCoGe.Trim <> "" Then
                        txtCodCliForFilProvv.AutoPostBack = False
                        txtCodCliForFilProvv.Text = myCoGe
                        txtCodCliForFilProvv.AutoPostBack = True
                        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
                        '-
                        PICFPagBancaAgeLisByCliFor(False)
                        WUC_DocumentiDett1.SetSWPrezzoALCSG()
                    End If
                End If
                '---------
                Dim strErrore As String = ""
                If AggNuovaTestata(strErrore) = False Then
                    Session(ERROREALL) = SWSI
                    Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
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
                Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Sub
            End If
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlAdapDoc.Fill(DsDocT.DocumentiT)

            dvDocT = New DataView(DsDocT.DocumentiT)
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
                        WUC_DocumentiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                        WUC_DocumentiDett1.PopolaLBLTotaliDoc(dvDocT, "")
                    Else
                        CampiSetEnabledToT(False)
                        BtnSetEnabledTo(False)
                        AzzeraTxtDocT()
                        WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                        WUC_DocumentiDett1.AzzeraLBLTotaliDoc()
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
                        WUC_DocumentiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, "") 'GIU100118
                        WUC_DocumentiDett1.PopolaLBLTotaliDoc(dvDocT, "")
                        btnElimina.Enabled = True
                        btnModifica.Enabled = True
                        btnStampa.Enabled = True
                        btnEtichette.Enabled = True
                        btnCaricoLotti.Enabled = True
                        btnConfOrdine.Enabled = True
                        btnListaCarico.Enabled = True
                    Else
                        btnCaricoLotti.Enabled = False
                        AzzeraTxtDocT()
                        WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2() 'GIU100118
                        WUC_DocumentiDett1.AzzeraLBLTotaliDoc()
                    End If
                End If
                Session(SWMODIFICATO) = SWNO
                'GIU030212
                Session(SWOPDETTDOC) = SWOPNESSUNA
                Session(SWOPDETTDOCR) = SWOPNESSUNA
                Session(SWOPDETTDOCL) = SWOPNESSUNA
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
            Catch Ex As Exception
                Session(ERROREALL) = SWSI
                Chiudi("Errore: Caricamento GESTIONE DOCUMENTI (load): " & Ex.Message)
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
        'giu210612
        If Session(F_ANAGRTIPOFATT_APERTA) = True Then
            TipoFatturazione.Show()
        End If
        If Session(F_LEAD_APERTA) = True Then
            WFP_LeadSource1.Show()
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
        'giu220120
        WFPETP.WucElement = Me
        If Session(F_ETP_APERTA) Then
            WFPETP.Show()
        End If
    End Sub

    'giu260312
    Private Function GetNewNumDoc() As Boolean
        GetNewNumDoc = True
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            GetNewNumDoc = False
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (GetNewNumDoc)")
            Exit Function
        End If
        Session(IDLISTINO) = "1" 'Listino Base
        Session(IDDOCUMENTI) = ""
        Session(CSTVALUTADOC) = "Euro"
        Session(CSTDECIMALIVALUTADOC) = 2
        '-
        lblCodValuta.Text = "Euro"

        Dim strErrore As String = ""
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroPreventivo + 1
                txtNGG_Validita.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Then 'giu030212
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroRiordinoFornitore + 1
                txtNGG_Validita.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodiceCausaleRiordino
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineCliente + 1
                txtNGG_Validita.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then 'giu201211
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
                txtNGG_Validita.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodiceCausaleRiordino
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
                txtNGG_Validita.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
                txtNGG_Validita.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                'giu260312 txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
                txtNGG_Validita.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = "0" 'giu260312 GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                If SWFatturaPA Then
                    txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                Else
                    txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                End If
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroFA + 1
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                'GIU230312
                If SWFatturaPA Then 'giu230714
                    If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = True Then
                        txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPA + 1
                    Else
                        txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                Else
                    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = True Then
                        txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                    Else
                        txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    End If
                End If
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CausNCResi
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaCdenza + 1
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                txtNumero.Text = GetParamGestAzi(Session(ESERCIZIO)).NumeroBC + 1
                txtNGG_Validita.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Validita
                txtNGG_Consegna.Text = GetParamGestAzi(Session(ESERCIZIO)).NGG_Consegna
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Then
                Session(SWOPNUOVONUMDOC) = ""
                txtNumero.Text = GetNewMM()
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            ElseIf Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Then
                Session(SWOPNUOVONUMDOC) = ""
                txtNumero.Text = GetNewMM()
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            ElseIf Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                Session(SWOPNUOVONUMDOC) = ""
                txtNumero.Text = GetNewMM()
                txtCodCausale.Text = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
            Else
                Session(ERROREALL) = SWSI
                GetNewNumDoc = False
                Chiudi("Errore: Tipo documento non gestito: (" & Session(CSTTIPODOC).ToString & ")")
                Exit Function
            End If
        Else
            Session(ERROREALL) = SWSI
            GetNewNumDoc = False
            Chiudi("Errore: Caricamento parametri generali. " & strErrore)
            Exit Function
        End If
        'giu230312 Recupero numeri non impegnati
        If Session(SWOPNUOVONUMDOC) = SWNO Then
            Session(SWOPNUOVONUMDOC) = ""
            txtNumero.Text = RecuperaNumDoc()
        End If
        '---------
    End Function
    'giu030519 solo documenti FISCALI: DDT/FOR FC/FS/FA NC BC MA ANCHE ALTRI OK
    Private Function RecuperaNumDoc() As Long
        RecuperaNumDoc = 0 'provvisorio
        'giu120412 recupero numdoc ultimo acquisito + 1
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            RecuperaNumDoc = 0
            Exit Function
        End If
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "')"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
            'GIU220714
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            'giu220714
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        End If

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        RecuperaNumDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                        'giu260819 non va bene, es. i preventivi ci sono le REVISION quindi sicuramente il numero è superiore
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    'RecuperaNumDoc = (ds.Tables(0).Rows(0).Item("TotDoc") + 1)
                        ' ''    'GIU171012
                        ' ''    RecuperaNumDoc = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < RecuperaNumDoc, RecuperaNumDoc, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
                        ' ''End If
                    Else
                        RecuperaNumDoc = 1
                    End If
                    Exit Function
                Else
                    RecuperaNumDoc = 1
                    Exit Function
                End If
            Else
                RecuperaNumDoc = 1
                Exit Function
            End If
        Catch Ex As Exception
            'strErrore = Ex.Message
            RecuperaNumDoc = -1
            Exit Function
        End Try

    End Function
    Private Function GetNewMM() As Long
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (GetNewMM)")
            Exit Function
        End If
        txtRevNDoc.AutoPostBack = False
        If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True
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
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
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

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandText = "get_DocTByIDDocumenti" 'OK SELECT *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDoc
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1
        '
        SqlDbInserCmd.CommandText = "insert_DocTByIDDocumenti"
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
        '
        'SqlUpdateCommand1
        '
        SqlDbUpdateCmd.CommandText = "[Update_DocTByIDDocumenti]"
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
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmd.CommandText = "[delete_DocTByIDDocumenti]"
        SqlDbDeleteCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmd.Connection = Me.SqlConnDoc
        SqlDbDeleteCmd.CommandTimeout = myTimeOUT
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlDbInserCmdForInsert
        '
        SqlDbInserCmdForInsert.CommandText = "insert_DocDByIDDocumenti_ForInsert"
        SqlDbInserCmdForInsert.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdForInsert.Connection = Me.SqlConnDoc
        SqlDbInserCmdForInsert.CommandTimeout = myTimeOUT
        SqlDbInserCmdForInsert.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
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
            New System.Data.SqlClient.SqlParameter("@DedPerAcconto", System.Data.SqlDbType.Bit, 0, "DedPerAcconto")})
        '
        'SqlDbInserCmdForInsertL
        '
        SqlDbInserCmdForInsertL.CommandText = "insert_DocDLByIDDocRiga_ForInsert"
        SqlDbInserCmdForInsertL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdForInsertL.Connection = Me.SqlConnDoc
        SqlDbInserCmdForInsertL.CommandTimeout = myTimeOUT
        SqlDbInserCmdForInsertL.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie")})

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
            Session(COD_CLIENTE) = String.Empty
            PopolaDestCliFor()
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
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
        PosizionaItemDDLByTxt(txtListino, DDLListini)
        lblCodValuta.Text = GetParamGestAzi(Session(ESERCIZIO)).Cod_Valuta
        lblValuta()
        WUC_DocumentiDett1.SetSWPrezzoALCSG() 'GIU210412
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
        If rk.Rag_Soc.Trim.Length > 0 And rk.Rag_Soc.Trim.Length < 36 Then
            Session("CKRagSocSede35") = "OK"
        Else
            Session("CKRagSocSede35") = "NO"
        End If
        If rk.Riferimento.Trim.Length > 0 And rk.Riferimento.Trim.Length < 36 Then
            Session("CKRiferimSede35") = "OK"
        Else
            Session("CKRiferimSede35") = "NO"
        End If
        strToolTip += txtCodCliForFilProvv.Text.Trim + " " + rk.Rag_Soc + " " + rk.Denominazione + " " + rk.Riferimento + " "
        '---------
        lblPICF.Text = rk.Partita_IVA : lblLabelPICF.Text = "Partita IVA"
        If lblPICF.Text.Trim = "" Then
            lblPICF.Text = rk.Codice_Fiscale : lblLabelPICF.Text = "Codice Fiscale"
        End If
        If lblPICF.Text.Trim = "" Then 'GIU200423
            Session("CKPIVACF") = "NO"
        Else
            Session("CKPIVACF") = "OK"
        End If
        If rk.IPA.Trim = "" Then
            Session("CKIPAObb") = "NO"
        Else
            Session("CKIPAObb") = "OK"
        End If
        strToolTip += lblLabelPICF.Text + ": " + lblPICF.Text + " "
        lblIndirizzo.Text = rk.Indirizzo + " " + rk.NumeroCivico
        strToolTip += lblIndirizzo.Text + " "
        lblLocalita.Text = rk.Cap & " " & rk.Localita & " " & IIf(rk.Provincia.ToString.Trim <> "", "(" & rk.Provincia.ToString & ")", "")
        strToolTip += lblLocalita.Text
        '-- OK
        txtCodCliForFilProvv.ToolTip = strToolTip
        lblCliForFilProvv.ToolTip = strToolTip
        'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
        If rk.Telefono1.Trim <> "" Then
            Session("CKTelOBBSede") = "OK"
        ElseIf rk.Telefono2.Trim <> "" Then
            Session("CKTelOBBSede") = "OK"
        End If
        '---------------------------------------------------------
        WUC_DocumentiDett1.SetSWPrezzoALCSG() 'GIU210412
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
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "", Optional ByRef mySCGiacenza As Boolean = False) As Boolean
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
    Public Function CKPrezzoALCSG(ByRef _PrezzoAL As String, ByRef _strErrore As String, Optional ByVal swPosDDLM2 As Boolean = False) As Boolean
        'SG=CSTSEGNOGIACENZA
        'A=Acquisto
        'L=Listino
        'C=Costo (FIFO)
        CKPrezzoALCSG = True
        If CKCSTTipoDoc() = False Then
            _strErrore = "Errore: TIPO DOCUMENTO SCONOSCIUTO"
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
        If TipoDoc = SWTD(TD.OrdFornitori) Or TipoDoc = SWTD(TD.DocTrasportoFornitori) Or
           TipoDoc = SWTD(TD.PropOrdFornitori) Or
           TipoDoc = SWTD(TD.CaricoMagazzino) Or
           (TipoDoc = SWTD(TD.MovimentoMagazzino) And Segno_Giacenza = "+") Or
           TabCliFor = "For" Then
            _PrezzoAL = "A"
        End If
        'GIU121020 PER TUTTI I DOC.
        If RKCausMag.Movimento_Magazzini Then
            lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
            lblCausale2.Visible = True : DDLCausali2.Visible = True
            'GIU260224
            If swPosDDLM2 = True Then
                Call PosizionaItemDDL(RKCausMag.CausMag2, DDLCausali2)
                'giu150224 CausCVenditaDaCVisione
                Call PosizionaItemDDL(RKCausMag.CausCVenditaDaCVisione, DDLMagazzino2)
            End If
            '------------
        Else
            DDLMagazzino2.SelectedIndex = 0 : DDLCausali2.SelectedIndex = 0
            lblMagazzino2.Visible = False : DDLMagazzino2.Visible = False
            lblCausale2.Visible = False : DDLCausali2.Visible = False
        End If
    End Function
    'Chiamato da TD1 DETTAGLI per TD3 TOTALI,SPESE,SCADENZARIO
    Public Function CalcolaTotSpeseScad(ByVal dsDocDett As DataSet, ByRef Iva() As Integer, _
                                        ByRef Imponibile() As Decimal, ByRef Imposta() As Decimal, _
                                        ByVal DecimaliValuta As Integer, ByRef MoltiplicatoreValuta As Integer, _
                                        ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, _
                                        ByVal ScontoCassa As Decimal, Optional ByVal Listino As Long = 9999, _
                                        Optional ByVal TipoDocumento As String = "DT", _
                                        Optional ByVal Abbuono As Decimal = 0, _
                                        Optional ByRef strErrore As String = "", _
                                        Optional ByRef TotaleLordoMercePL As Decimal = 0, _
                                        Optional ByRef Deduzioni As Decimal = 0) As Boolean 'giu020519
        strErrore = ""
        Session(CSTIDPAG) = txtPagamento.Text.Trim 'giu020914
        If WUC_DocumentiSpeseTraspTot1.CalcolaTotSpeseScad(dsDocDett, Iva, Imponibile, Imposta, _
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
        btnElimina.Enabled = SW
        btnModifica.Enabled = SW
        btnNuovo.Enabled = SW
        btnStampa.Enabled = SW
        btnEtichette.Enabled = SW
        'giu220323 btnCaricoLotti.Enabled = SW
        btnConfOrdine.Enabled = SW
        btnListaCarico.Enabled = SW
    End Sub

#Region " Dati Intestazione"
    Private Sub PopolaTxtDocT()
        Session(CSTCODFILIALE) = String.Empty : lblDestSel.Text = "" : lblDestSel.ToolTip = "" 'giu220520
        Session("CKTelOBB") = "" 'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
        Session("CKPIVACF") = ""
        Session("CKIPAObb") = ""
        Session("CKTelOBBSede") = ""
        Session("CKRagSocSede35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
        Session("CKRiferimSede35") = "" 'GIU060423 CONTROLLO riferimento >35
        Session("CKRagSoc35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
        Session("CKRiferim35") = "" 'GIU060423 CONTROLLO riferimento >35
        SfondoCampiDocT()
        Try
            If (dvDocT Is Nothing) Then
                DsDocT = Session("DsDocT")
                dvDocT = New DataView(DsDocT.DocumentiT)
            End If
            If dvDocT.Count = 0 Then
                AzzeraTxtDocT()
                Exit Sub
            End If
            'giu291220
            Dim myCLead As String = ""
            If IsDBNull(dvDocT.Item(0).Item("NumPagineTot")) Then
                myCLead = ""
            Else
                myCLead = dvDocT.Item(0).Item("NumPagineTot")
            End If
            If IsNothing(myCLead) Then
                myCLead = "0"
            End If
            If String.IsNullOrEmpty(myCLead) Then
                myCLead = "0"
            End If
            If myCLead = "" Then
                myCLead = "0"
            End If
            PosizionaItemDDL(myCLead.Trim, DDLLead)
            '---------
            'giu080920 giu230920
            Dim myCMag As String = ""
            If IsDBNull(dvDocT.Item(0).Item("CodiceMagazzino")) Then
                myCMag = ""
            Else
                myCMag = dvDocT.Item(0).Item("CodiceMagazzino")
            End If
            If IsNothing(myCMag) Then
                myCMag = "0"
            End If
            If String.IsNullOrEmpty(myCMag) Then
                myCMag = "0"
            End If
            If TipoDoc = "" Then
                myCMag = "0"
            End If
            Session(IDMAGAZZINO) = myCMag
            '---------
            ddlMagazzino.AutoPostBack = False
            PosizionaItemDDL(myCMag.Trim, ddlMagazzino, True)
            ddlMagazzino.AutoPostBack = True
            'GIU230920
            If IsDBNull(dvDocT.Item(0).Item("CodiceMagazzinoM2")) Then
                myCMag = ""
            Else
                myCMag = dvDocT.Item(0).Item("CodiceMagazzinoM2")
            End If
            If IsNothing(myCMag) Then
                myCMag = ""
            End If
            If String.IsNullOrEmpty(myCMag) Then
                myCMag = ""
            End If
            If TipoDoc = "" Then
                myCMag = ""
            End If
            If myCMag.Trim <> "" Then
                lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
                lblCausale2.Visible = True : DDLCausali2.Visible = True
                ' ''Else
                ' ''    lblMagazzino2.Visible = False : DDLMagazzino2.Visible = False
                ' ''    lblCausale2.Visible = False : DDLCausali2.Visible = False
            End If
            DDLMagazzino2.AutoPostBack = False
            If myCMag <> "" Then
                PosizionaItemDDL(myCMag.Trim, DDLMagazzino2, True)
            Else
                DDLMagazzino2.SelectedIndex = 0
            End If
            DDLMagazzino2.AutoPostBack = True
            If ddlMagazzino.SelectedValue = DDLMagazzino2.SelectedValue And myCMag.Trim <> "" Then
                DDLMagazzino2.BackColor = SEGNALA_KO
                lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
                lblCausale2.Visible = True : DDLCausali2.Visible = True
            Else
                DDLMagazzino2.BackColor = SEGNALA_OK
            End If
            '-CAUS
            Dim myCCaus As String = ""
            If IsDBNull(dvDocT.Item(0).Item("Cod_CausaleM2")) Then
                myCCaus = ""
            Else
                myCCaus = dvDocT.Item(0).Item("Cod_CausaleM2").ToString.Trim
            End If
            If IsNothing(myCCaus) Then
                myCCaus = ""
            End If
            If String.IsNullOrEmpty(myCCaus) Then
                myCCaus = ""
            End If
            If myCCaus.Trim <> "" Then
                lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
                lblCausale2.Visible = True : DDLCausali2.Visible = True
                ' ''Else
                ' ''    lblMagazzino2.Visible = False : DDLMagazzino2.Visible = False
                ' ''    lblCausale2.Visible = False : DDLCausali2.Visible = False
            End If
            DDLCausali2.AutoPostBack = False
            If myCCaus.Trim <> "" Then
                PosizionaItemDDL(myCCaus, DDLCausali2)
            Else
                DDLCausali2.SelectedIndex = 0
            End If
            DDLCausali2.AutoPostBack = True
            If DDLCausali.SelectedValue = DDLCausali2.SelectedValue And myCCaus.Trim <> "" Then
                DDLCausali2.BackColor = SEGNALA_KO
                lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
                lblCausale2.Visible = True : DDLCausali2.Visible = True
            Else
                DDLCausali2.BackColor = SEGNALA_OK
            End If
            '-----------------
            Session(CSTSTATODOC) = dvDocT.Item(0).Item("StatoDoc").ToString
            txtNumero.AutoPostBack = False : txtRevNDoc.AutoPostBack = False
            txtNumero.Text = dvDocT.Item(0).Item("Numero").ToString
            txtRevNDoc.Text = dvDocT.Item(0).Item("RevisioneNDoc").ToString
            If IsDate(dvDocT.Item(0).Item("Data_Doc")) Then
                txtDataDoc.Text = Format(dvDocT.Item(0).Item("Data_Doc"), FormatoData)
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
            Else
                txtDataDoc.Text = ""
                Session(CSTDATADOC) = ""
            End If
            txtNumero.AutoPostBack = True : txtRevNDoc.AutoPostBack = True

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
            'giu250321
            ' ''txtRiferimento.Text = dvDocT.Item(0).Item("Riferimento").ToString.Trim
            Dim myPos As Integer = 0
            Dim myRif As String = "" : Dim myComm As String = ""
            If Not IsDBNull(dvDocT.Item(0).Item("Riferimento")) Then
                myRif = dvDocT.Item(0).Item("Riferimento")
            End If
            myPos = InStr(myRif, "§")
            If myPos > 0 Then
                myComm = Mid(myRif, myPos + 1)
                myRif = Mid(myRif, 1, myPos - 1)
            End If
            txtRiferimento.Text = myRif
            txtCCommessa.Text = myComm
            '---------
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
                txtCodCliForFilProvv.Text = dvDocT.Item(0).Item("Cod_Cliente").ToString.Trim
                Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
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
                        LnkSCEC.Visible = False 'GIU080722
                        lblLabelCliForFilProvv.Text = "Fornitore"
                        Session(CSTTABCLIFOR) = "For"
                        Session(COD_CLIENTE) = ""
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
                Session(COD_CLIENTE) = String.Empty
                If Session(CSTTABCLIFOR) = "Cli" Then
                    lblLabelCliForFilProvv.Text = "Cliente"
                    Try
                        lblLabelCliForFilProvv.Text += " (" + dvDocT.Item(0).Item("IPA").ToString.Trim + ")"
                    Catch ex As Exception

                    End Try
                End If

                If Session(CSTTABCLIFOR) = "For" Then
                    lblLabelCliForFilProvv.Text = "Fornitore"
                    LnkSCEC.Visible = False 'GIU080722
                End If
                If Session(CSTTABCLIFOR) = "CliFor" Then
                    lblLabelCliForFilProvv.Text = "Cli./For."
                    lblLabelCliForFilProvv.Text = "Cliente"
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
                VisAnagrProvv("(" & dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim & ")")
            End If
            txtCodCliForFilProvv.AutoPostBack = True

            txtDestinazione1.Text = dvDocT.Item(0).Item("Destinazione1").ToString.Trim
            txtDestinazione2.Text = dvDocT.Item(0).Item("Destinazione2").ToString.Trim
            txtDestinazione3.Text = dvDocT.Item(0).Item("Destinazione3").ToString.Trim
            '-
            txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
            txtPagamento.Text = dvDocT.Item(0).Item("Cod_Pagamento").ToString
            PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
            Session(CSTIDPAG) = txtPagamento.Text.Trim
            txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
            If IsDBNull(dvDocT.Item(0).Item("NumPagineTot")) Then
                DDLLead.SelectedIndex = 0
            Else
                PosizionaItemDDL(dvDocT.Item(0).Item("NumPagineTot").ToString, DDLLead)
            End If

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
            txtCodCausale.AutoPostBack = False
            txtCodCausale.Text = dvDocT.Item(0).Item("Cod_Causale").ToString
            PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
            txtCodCausale.AutoPostBack = True
            'GIU230920()
            Dim myCodCaus As Integer = 0
            If txtCodCausale.Text.Trim = "" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
                myCodCaus = -1
            Else
                myCodCaus = CLng(txtCodCausale.Text.Trim)
            End If
            Dim StrErrore As String = ""
            Dim RKCausMag As StrCausMag = GetDatiCausMag(myCodCaus, StrErrore)
            If IsNothing(RKCausMag.Codice) Then
                StrErrore = "CODICE CAUSALE NON PRESENTE IN TABELLA"
                txtCodCausale.BackColor = SEGNALA_KO
            ElseIf StrErrore.Trim <> "" Then
                StrErrore = StrErrore.Trim
                txtCodCausale.BackColor = SEGNALA_KO
            End If
            'GIU121020
            If RKCausMag.Movimento_Magazzini Then
                lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
                lblCausale2.Visible = True : DDLCausali2.Visible = True
            Else
                DDLMagazzino2.BackColor = SEGNALA_OK : DDLCausali2.BackColor = SEGNALA_OK
                DDLMagazzino2.SelectedIndex = 0 : DDLCausali2.SelectedIndex = 0
                lblMagazzino2.Visible = False : DDLMagazzino2.Visible = False
                lblCausale2.Visible = False : DDLCausali2.Visible = False
            End If
            '----------------------------------------------
            'giu260412
            If IsDBNull(dvDocT.Item(0).Item("ADeposito")) Then
                checkADeposito.Checked = False
            ElseIf dvDocT.Item(0).Item("ADeposito") = True Then
                checkADeposito.Checked = True
            Else
                checkADeposito.Checked = False
            End If
            txtListino.AutoPostBack = False
            txtListino.Text = dvDocT.Item(0).Item("Listino").ToString
            Session(IDLISTINO) = txtListino.Text.Trim
            PosizionaItemDDLByTxt(txtListino, DDLListini)
            Session(IDLISTINO) = txtListino.Text.Trim
            txtListino.AutoPostBack = True
            LnkRegimeIVA.Text = "Regime IVA " & dvDocT.Item(0).Item("Cod_Iva").ToString.Trim
            Session(CSTREGIMEIVA) = dvDocT.Item(0).Item("Cod_IVA").ToString
            lblCodValuta.Text = dvDocT.Item(0).Item("Cod_Valuta").ToString
            Session(CSTVALUTADOC) = dvDocT.Item(0).Item("Cod_Valuta").ToString
            Session(CSTDECIMALIVALUTADOC) = 0
            lblValuta()

            txtTipoFatt.AutoPostBack = False
            txtTipoFatt.Text = dvDocT.Item(0).Item("Tipo_Fatturazione").ToString
            PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
            txtTipoFatt.AutoPostBack = True
            'giu070814
            chkFatturaPA.AutoPostBack = False
            If IsDBNull(dvDocT.Item(0).Item("FatturaPA")) Then
                chkFatturaPA.Checked = False
            ElseIf dvDocT.Item(0).Item("FatturaPA") = True Then
                chkFatturaPA.Checked = True
            Else
                chkFatturaPA.Checked = False
            End If
            'giu310323
            If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                (Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) And chkFatturaAC.Checked) Then 'GIU080722 giu310323
                btnEtichette.Visible = True
                btnCaricoLotti.Visible = True 'giu220323
            Else
                btnEtichette.Visible = False
                btnCaricoLotti.Visible = False 'giu220323
            End If
            '---------
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
            'giu281221 GIU080722 ACCONTO/SALDO SOLO PER LE FATTURE
            If Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                If IsDBNull(dvDocT.Item(0).Item("Acconto")) Then
                    ChkAcconto.Checked = False
                ElseIf dvDocT.Item(0).Item("Acconto") <> 0 Then
                    ChkAcconto.Checked = True
                Else
                    ChkAcconto.Checked = False
                End If
                'giu040118
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then 'GIU080722 ACCONTO/SALDO SOLO PER LE FATTURE
                optPagAnticipato.AutoPostBack = False
                optPagEffettuato.AutoPostBack = False
                optPagLista.AutoPostBack = False
                optPagSconosciuto.AutoPostBack = False
                '-
                If IsDBNull(dvDocT.Item(0).Item("Acconto")) Then
                    optPagSconosciuto.Checked = True
                    optPagAnticipato.Checked = False
                    optPagEffettuato.Checked = False
                    optPagLista.Checked = False
                ElseIf dvDocT.Item(0).Item("Acconto") = 1 Then
                    optPagAnticipato.Checked = True
                    optPagSconosciuto.Checked = False
                    optPagEffettuato.Checked = False
                    optPagLista.Checked = False
                ElseIf dvDocT.Item(0).Item("Acconto") = 2 Then
                    optPagEffettuato.Checked = True
                    optPagSconosciuto.Checked = False
                    optPagAnticipato.Checked = False
                    optPagLista.Checked = False
                ElseIf dvDocT.Item(0).Item("Acconto") = 3 Then
                    optPagEffettuato.Checked = False
                    optPagSconosciuto.Checked = False
                    optPagAnticipato.Checked = False
                    optPagLista.Checked = True
                Else
                    optPagSconosciuto.Checked = True
                    optPagAnticipato.Checked = False
                    optPagEffettuato.Checked = False
                    optPagLista.Checked = False
                End If
                optPagAnticipato.AutoPostBack = True
                optPagEffettuato.AutoPostBack = True
                optPagLista.AutoPostBack = True
                optPagSconosciuto.AutoPostBack = True
                '-
            Else
                ChkAcconto.Checked = False
                '---
                optPagAnticipato.AutoPostBack = False
                optPagEffettuato.AutoPostBack = False
                optPagLista.AutoPostBack = False
                optPagSconosciuto.AutoPostBack = False
                '-
                optPagSconosciuto.Checked = True
                optPagAnticipato.Checked = False
                optPagEffettuato.Checked = False
                optPagLista.Checked = False
                
            End If
            '---------
            If IsDBNull(dvDocT.Item(0).Item("SplitIVA")) Then
                SWSplitIVA = False
            ElseIf dvDocT.Item(0).Item("SplitIVA") = True Then
                SWSplitIVA = True
            Else
                SWSplitIVA = False
            End If
            Session(CSTSPLITIVA) = SWSplitIVA
            'GIU080814------------------------------------------------------------------
            StrErrore = ""
            If txtCodCliForFilProvv.Text.Trim <> "" Then
                Dim mySplitIVA As Boolean = False
                If Documenti.CKClientiIPAByIDDocORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, StrErrore) = True Then
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
                    WUC_DocumentiSpeseTraspTot1.SetErrChkSplitIVA(False)
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
            'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
            If Not IsDBNull(dvDocT.Item(0).Item("RefDataDDT")) Then
                If IsDate(dvDocT.Item(0).Item("RefDataDDT")) Then
                    Session("CKTelOBB") = "OK" 'GIU060423 MELGIO LASCIARLO NEL CASO VENGA RITRASMESSSO ? 
                    Session("CKTelOBBSede") = "OK"
                    Session("CKRagSocSede35") = "OK" 'GIU060423 CONTROLLO RAG_SOC >35
                    Session("CKRiferimSede35") = "OK" 'GIU060423 CONTROLLO riferimento >35
                    Session("CKRagSoc35") = "OK" 'GIU060423 CONTROLLO RAG_SOC >35
                    Session("CKRiferim35") = "OK" 'GIU060423 CONTROLLO riferimento >35
                End If
            End If
            '----------------------------------------------------------
            'giu260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
            Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
            If CkNumDoc = -1 Then
                Exit Sub
            End If
            Dim myNumDoc As Long
            If Not IsNumeric(txtNumero.Text.Trim) Then Exit Sub
            myNumDoc = txtNumero.Text.Trim
            CkNumDoc = CkNumDoc - 1
            If myNumDoc - 1 = CkNumDoc Then
                'ok proseguo
            ElseIf myNumDoc > CkNumDoc Then
                txtNumero.BackColor = SEGNALA_KO
                txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. MAGGIORE di quello inserito. !!!VERIFICARE !!!"
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
        Session("CKTelOBB") = "" 'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
        Session("CKTelOBBSede") = ""
        Session("CKRagSocSede35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
        Session("CKRiferimSede35") = "" 'GIU060423 CONTROLLO riferimento >35
        Session("CKRagSoc35") = "" 'GIU060423 CONTROLLO RAG_SOC >35
        Session("CKRiferim35") = "" 'GIU060423 CONTROLLO riferimento >35
        Session("CKPIVACF") = ""
        Session("CKIPAObb") = ""
        Session("SWNOTEAVVISO") = ""
        If txtCodCliForFilProvv.Text.Trim = "" Then
            LnkRegimeIVA.Text = "Regime IVA"
            lblCliForFilProvv.Text = "" 'giu080412
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = String.Empty
            Session(COD_CLIENTE) = ""
            Session(CSTCODFILIALE) = String.Empty
            Session(CSTUpgAngrProvvCG) = String.Empty 'GIU111012
            PopolaDestCliFor()
            'PER ADESSO LE INIT LE FACCIO
            DDLAgenti.SelectedIndex = 0
            DDLListini.SelectedIndex = 0
            Session(IDAGENTE) = ""
            Session(IDLISTINO) = "1"
            txtListino.Text = "1"
            Session(CSTVALUTADOC) = "Euro"
            Session(CSTDECIMALIVALUTADOC) = 2
            Session(CSTREGIMEIVA) = "0"
            Session(CSTIDPAG) = ""
            'giu180219
            Session(CSTFATTURAPA) = False
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = False
            chkFatturaPA.AutoPostBack = True
            Session(CSTSPLITIVA) = False
            WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
            Exit Sub
        End If
        'Anagrafiche provvisorie EXIT SUB
        If Mid(txtCodCliForFilProvv.Text.Trim, 1, 1) = "(" Then
            LnkRegimeIVA.Text = "Regime IVA"
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = String.Empty
            Session(COD_CLIENTE) = ""
            Session(CSTCODFILIALE) = String.Empty
            Session(CSTUpgAngrProvvCG) = txtCodCliForFilProvv.Text.Trim 'GIU111012
            PopolaDestCliFor()
            'PER ADESSO LE INIT LE FACCIO
            DDLAgenti.SelectedIndex = 0
            DDLListini.SelectedIndex = 0
            Session(IDAGENTE) = ""
            Session(IDLISTINO) = "1"
            txtListino.Text = "1"
            Session(CSTVALUTADOC) = "Euro"
            Session(CSTDECIMALIVALUTADOC) = 2
            Session(CSTREGIMEIVA) = "0"
            Session(CSTIDPAG) = ""
            'giu180219
            Session(CSTFATTURAPA) = False
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = False
            chkFatturaPA.AutoPostBack = True
            Session(CSTSPLITIVA) = False
            WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
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
        'giu220113
        If TabCliFor <> "Cli" Then
            Session(COD_CLIENTE) = String.Empty
        Else
            Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
        End If
        '---------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        If SWDoc = False Then
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
                DDLAgenti.SelectedIndex = 0
                DDLListini.SelectedIndex = 0
                Session(IDAGENTE) = ""
                Session(IDLISTINO) = "1"
                txtListino.Text = "1"
                Session(CSTVALUTADOC) = "Euro"
                Session(CSTDECIMALIVALUTADOC) = 2
                LnkRegimeIVA.Text = "Regime IVA"
                Session(CSTREGIMEIVA) = "0"
                Session(CSTIDPAG) = ""
                'giu180219
                Session(CSTFATTURAPA) = False
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                Session(CSTSPLITIVA) = False
                WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
            End If
            Exit Sub
        End If
        If dvCliFor.Count > 0 Then
            If dvCliFor.Count > 0 Then
                Try
                    lblLabelCliForFilProvv.Text += " (" + dvCliFor.Item(0).Item("IPA").ToString.Trim + ")"
                    'giu200423
                    If Not IsDBNull(dvCliFor.Item(0).Item("IPA")) Then
                        If dvCliFor.Item(0).Item("IPA").ToString.Trim <> "" Then
                            Session("CKIPAObb") = "OK"
                        End If
                    Else
                        Session("CKIPAObb") = "NO"
                    End If
                    'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
                    'giu060423
                    If dvCliFor.Item(0).Item("Rag_Soc").ToString.Trim.Length > 0 And dvCliFor.Item(0).Item("Rag_Soc").ToString.Trim.Length < 36 Then
                        Session("CKRagSocSede35") = "OK"
                    Else
                        Session("CKRagSocSede35") = "NO"
                    End If
                    If dvCliFor.Item(0).Item("Riferimento").ToString.Trim.Length > 0 And dvCliFor.Item(0).Item("Riferimento").ToString.Trim.Length < 36 Then
                        Session("CKRiferimSede35") = "OK"
                    Else
                        Session("CKRiferimSede35") = "NO"
                    End If
                    '-
                    If Not IsDBNull(dvCliFor.Item(0).Item("Telefono1")) Then
                        If dvCliFor.Item(0).Item("Telefono1").ToString.Trim <> "" Then
                            Session("CKTelOBBSede") = "OK"
                        ElseIf Not IsDBNull(dvCliFor.Item(0).Item("Telefono2")) Then
                            If dvCliFor.Item(0).Item("Telefono2").ToString.Trim <> "" Then
                                Session("CKTelOBBSede") = "OK"
                            End If
                        End If
                    ElseIf Not IsDBNull(dvCliFor.Item(0).Item("Telefono2")) Then
                        If dvCliFor.Item(0).Item("Telefono2").ToString.Trim <> "" Then
                            Session("CKTelOBBSede") = "OK"
                        End If
                    End If
                    '---------------------------------------------------------
                    If Not IsDBNull(dvCliFor.Item(0).Item("DEM")) Then
                        If CBool(dvCliFor.Item(0).Item("DEM")) Then
                            Dim strNoteAvviso As String = ""
                            If Not IsDBNull(dvCliFor.Item(0).Item("Note")) Then
                                strNoteAvviso = dvCliFor.Item(0).Item("Note").ToString.Trim
                            End If
                            If strNoteAvviso.Trim <> "" Then
                                Session("SWNOTEAVVISO") = strNoteAvviso
                                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("AVVISO", strNoteAvviso, WUC_ModalPopup.TYPE_INFO)
                            End If
                        End If
                    End If
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
                If lblPICF.Text.Trim = "" Then 'GIU200423
                    Session("CKPIVACF") = "NO"
                Else
                    Session("CKPIVACF") = "OK"
                End If
                strInfo += "P.IVA: " & lblPICF.Text & " "
                If SWDoc = False Then
                    LnkRegimeIVA.Text = "Regime IVA " & dvCliFor.Item(0).Item("Regime_IVA").ToString
                    txtPagamento.Text = dvCliFor.Item(0).Item("Pagamento_N").ToString
                    PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
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
                        txtTipoFatt.Text = dvCliFor.Item(0).Item("IDTipoFatt").ToString
                        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
                    Else
                        txtCodAgente.Text = "" : DDLAgenti.SelectedIndex = 0
                    End If
                    Session(IDAGENTE) = txtCodAgente.Text.Trim
                    txtListino.Text = dvCliFor.Item(0).Item("Listino").ToString
                    Session(IDLISTINO) = txtListino.Text.Trim
                    If txtListino.Text.Trim = "" Or txtListino.Text.Trim = "0" Then
                        txtListino.Text = "1" 'Listino Base
                        Session(CSTVALUTADOC) = "Euro"
                        Session(CSTDECIMALIVALUTADOC) = 2
                    End If
                    Session(IDLISTINO) = txtListino.Text.Trim
                    PosizionaItemDDLByTxt(txtListino, DDLListini)
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
                        WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
                    Else
                        Session(CSTFATTURAPA) = False
                        chkFatturaPA.AutoPostBack = False
                        chkFatturaPA.Checked = False
                        chkFatturaPA.AutoPostBack = True
                        Session(CSTSPLITIVA) = False
                        WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
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
                                WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(True, "")
                            End If
                        End If
                    End If
                    '---------
                End If
            Else
                lblCliForFilProvv.Text = ""
                lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
                If SWDoc = False Then
                    txtPagamento.Text = "" : DDLPagamento.SelectedIndex = 0
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
                    Session(CSTIDPAG) = ""
                    'giu180219
                    Session(CSTFATTURAPA) = False
                    chkFatturaPA.AutoPostBack = False
                    chkFatturaPA.Checked = False
                    chkFatturaPA.AutoPostBack = True
                    Session(CSTSPLITIVA) = False
                    WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
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
            If lblPICF.Text.Trim = "" Then 'GIU200423
                Session("CKPIVACF") = "NO"
            Else
                Session("CKPIVACF") = "OK"
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
                txtPagamento.Text = "" : DDLPagamento.SelectedIndex = 0
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
                Session(CSTIDPAG) = ""
                'giu180219
                Session(CSTFATTURAPA) = False
                chkFatturaPA.AutoPostBack = False
                chkFatturaPA.Checked = False
                chkFatturaPA.AutoPostBack = True
                Session(CSTSPLITIVA) = False
                WUC_DocumentiSpeseTraspTot1.SetChkSplitIVA(False, "")
            End If
        End If
        txtCodCliForFilProvv.ToolTip = strInfo
        'giu101011 commentato perchè lento con il combo trasformato in Label ed aggiunta la Ricerca
        'DDLCliForFilProvv.ToolTip = strInfo
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

        ' ''Dim dvABI As DataView
        ' ''dvABI = SqlDSBanca.Select(DataSourceSelectArguments.Empty)
        ' ''If (dvABI Is Nothing) Then
        ' ''    'lblABI.BackColor = SEGNALA_KO
        ' ''    'If lblCAB.Text.Trim <> "" Then lblCAB.BackColor = SEGNALA_KO
        ' ''    lblBanca.Text = "" : lblFiliale.Text = ""
        ' ''    Exit Sub
        ' ''End If
        ' ''If dvABI.Count > 0 Then
        ' ''    dvABI.RowFilter = "ABI = '" & lblABI.Text.Trim & "'"
        ' ''    If dvABI.Count > 0 Then
        ' ''        'lblABI.BackColor = SEGNALA_OK
        ' ''        lblBanca.Text = dvABI.Item(0).Item("Banca").ToString
        ' ''    Else
        ' ''        'lblABI.BackColor = SEGNALA_KO
        ' ''        lblBanca.Text = "" : lblFiliale.Text = ""
        ' ''        'If lblCAB.Text.Trim <> "" Then lblCAB.BackColor = SEGNALA_KO
        ' ''        Exit Sub
        ' ''    End If
        ' ''Else
        ' ''    'lblABI.BackColor = SEGNALA_KO
        ' ''    'If lblCAB.Text.Trim <> "" Then lblCAB.BackColor = SEGNALA_KO
        ' ''    lblBanca.Text = "" : lblFiliale.Text = ""
        ' ''    Exit Sub
        ' ''End If
        '' ''Filiale
        ' ''If lblCAB.Text.Trim = "" Then
        ' ''    'lblCAB.BackColor = SEGNALA_OK
        ' ''    lblFiliale.Text = ""
        ' ''    Exit Sub
        ' ''End If
        ' ''Session("ABI") = lblABI.Text.Trim
        ' ''SqlDSFiliale.DataBind()
        ' ''Dim dvCAB As DataView
        ' ''dvCAB = SqlDSFiliale.Select(DataSourceSelectArguments.Empty)
        ' ''If (dvCAB Is Nothing) Then
        ' ''    lblFiliale.Text = ""
        ' ''    'lblCAB.BackColor = SEGNALA_KO
        ' ''    Exit Sub
        ' ''End If
        ' ''If dvCAB.Count > 0 Then
        ' ''    dvCAB.RowFilter = "ABI = '" & lblABI.Text.Trim & "' AND CAB = '" & lblCAB.Text.Trim & "'"
        ' ''    If dvCAB.Count > 0 Then
        ' ''        'lblCAB.BackColor = SEGNALA_OK
        ' ''        lblFiliale.Text = dvCAB.Item(0).Item("Filiale").ToString
        ' ''    Else
        ' ''        lblFiliale.Text = ""
        ' ''        'lblCAB.BackColor = SEGNALA_KO
        ' ''        Exit Sub
        ' ''    End If
        ' ''Else
        ' ''    'lblCAB.BackColor = SEGNALA_KO
        ' ''    lblFiliale.Text = ""
        ' ''    Exit Sub
        ' ''End If
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
        txtNumero.AutoPostBack = False : txtRevNDoc.AutoPostBack = False
        txtNumero.Text = "" : txtRevNDoc.Text = "" : txtDataDoc.Text = ""
        txtNumero.AutoPostBack = True : txtRevNDoc.AutoPostBack = True
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = ""
        txtDataValidita.Text = "" : txtDataConsegna.Text = ""
        txtNGG_Validita.Text = "" : txtNGG_Consegna.Text = ""
        txtCIG.Text = "" : txtCUP.Text = ""
        txtRiferimento.Text = "" : txtDataRif.Text = "" : txtCCommessa.Text = ""
        optTipoEvTotaleTotale.Checked = True
        optTipoEvTotaleParziale.Checked = False
        '-
        ' ''optTipoEvSaldoSaldo.Checked = False
        ' ''optTipoEvSaldoParziale.Checked = False
        ' ''optTipoEvSaldoNO.Checked = True
        '------------------------------------------------------------
        txtCodCliForFilProvv.AutoPostBack = False
        txtCodCliForFilProvv.Text = ""
        txtCodCliForFilProvv.AutoPostBack = True
        'giu101011 commentato perchè lento con il combo trasformato in Label ed aggiunta la Ricerca
        'DDLCliForFilProvv.SelectedIndex = 0
        lblCliForFilProvv.Text = ""
        Session(CSTCODCOGE) = "" : Session(CSTCODFILIALE) = "" 'giu220520
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
        txtPagamento.AutoPostBack = False : DDLPagamento.AutoPostBack = False
        txtPagamento.Text = "" : DDLPagamento.SelectedIndex = 0
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        txtPagamento.AutoPostBack = True : DDLPagamento.AutoPostBack = True
        DDLLead.SelectedIndex = 0
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
        txtCodCausale.AutoPostBack = False
        txtCodCausale.Text = "" : DDLCausali.SelectedIndex = 0
        txtCodCausale.AutoPostBack = True
        checkADeposito.Checked = False 'giu260412
        txtListino.AutoPostBack = False
        txtListino.Text = "" : DDLListini.SelectedIndex = 0
        Session(IDLISTINO) = "1" 'Listino Base
        txtListino.AutoPostBack = True
        Session(CSTVALUTADOC) = "Euro"
        Session(CSTDECIMALIVALUTADOC) = 2
        LnkRegimeIVA.Text = "Regime IVA"
        Session(CSTREGIMEIVA) = ""
        lblCodValuta.Text = "" : lblDesValuta.Text = ""
        txtTipoFatt.AutoPostBack = False
        txtTipoFatt.Text = "" : DDLTipoFatt.SelectedIndex = 0
        txtTipoFatt.AutoPostBack = True
        txtDesRefInt.Text = ""
        txtNoteDocumento.Text = ""
        chkFatturaPA.AutoPostBack = False : chkFatturaAC.AutoPostBack = False : chkScGiacenza.AutoPostBack = False
        chkFatturaPA.Checked = False : chkFatturaAC.Checked = False : chkScGiacenza.Checked = False
        chkFatturaPA.AutoPostBack = True : chkFatturaAC.AutoPostBack = True : chkScGiacenza.AutoPostBack = False
        PosizionaItemDDL("", DDLTipoRapp)
        'giu091020
        ddlMagazzino.AutoPostBack = False
        PosizionaItemDDL("1", ddlMagazzino, True)
        ddlMagazzino.AutoPostBack = True
        DDLMagazzino2.AutoPostBack = False
        DDLMagazzino2.SelectedIndex = -1
        DDLMagazzino2.AutoPostBack = True
        DDLCausali2.AutoPostBack = False
        DDLCausali2.SelectedIndex = -1
        DDLCausali2.AutoPostBack = True
        '---------
        ChkAcconto.Checked = False
        'giu281221
        optPagAnticipato.AutoPostBack = False
        optPagEffettuato.AutoPostBack = False
        optPagLista.AutoPostBack = False
        optPagSconosciuto.AutoPostBack = False
        '-
        optPagSconosciuto.Checked = True
        optPagAnticipato.Checked = False
        optPagEffettuato.Checked = False
        optPagLista.Checked = False

        If Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
            optPagAnticipato.AutoPostBack = True
            optPagEffettuato.AutoPostBack = True
            optPagLista.AutoPostBack = True
            optPagSconosciuto.AutoPostBack = True
        End If
    End Sub
    Private Sub SfondoCampiDocT()
        ddlMagazzino.BackColor = SEGNALA_OK : DDLMagazzino2.BackColor = SEGNALA_OK
        DDLCausali2.BackColor = SEGNALA_OK
        '-
        txtNumero.BackColor = SEGNALA_OK : txtNumero.ToolTip = ""
        txtRevNDoc.BackColor = SEGNALA_OK : txtRevNDoc.ToolTip = ""
        txtDataDoc.BackColor = SEGNALA_OK : txtDataDoc.ToolTip = ""
        txtDataValidita.BackColor = SEGNALA_OK : txtDataValidita.ToolTip = ""
        txtDataConsegna.BackColor = SEGNALA_OK : txtDataConsegna.ToolTip = ""
        txtNGG_Validita.BackColor = SEGNALA_OK : txtNGG_Consegna.BackColor = SEGNALA_OK
        txtCIG.BackColor = SEGNALA_OK : txtCIG.ToolTip = ""
        txtCUP.BackColor = SEGNALA_OK : txtCUP.ToolTip = ""
        txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = "" : txtDataRif.BackColor = SEGNALA_OK : txtDataRif.ToolTip = ""
        txtCCommessa.BackColor = SEGNALA_OK : txtCCommessa.ToolTip = ""

        txtCodCliForFilProvv.BackColor = SEGNALA_OK : txtCodCliForFilProvv.ToolTip = ""

        txtDestinazione1.BackColor = SEGNALA_OK : txtDestinazione1.ToolTip = ""
        txtDestinazione2.BackColor = SEGNALA_OK : txtDestinazione2.ToolTip = ""
        txtDestinazione3.BackColor = SEGNALA_OK : txtDestinazione3.ToolTip = ""
        txtPagamento.BackColor = SEGNALA_OK : txtPagamento.ToolTip = "" : DDLPagamento.BackColor = SEGNALA_OK
        DDLLead.BackColor = SEGNALA_OK

        txtCodAgente.BackColor = SEGNALA_OK : txtCodAgente.ToolTip = "" : DDLAgenti.BackColor = SEGNALA_OK
        txtCodCausale.BackColor = SEGNALA_OK : txtCodCausale.ToolTip = "" : DDLCausali.BackColor = SEGNALA_OK
        txtListino.BackColor = SEGNALA_OK : txtListino.ToolTip = "" : DDLListini.BackColor = SEGNALA_OK

        txtTipoFatt.BackColor = SEGNALA_OK : txtTipoFatt.ToolTip = "" : DDLTipoFatt.BackColor = SEGNALA_OK
        chkFatturaPA.BackColor = SEGNALA_OK : chkFatturaPA.ToolTip = "" 'GIU070814
        DDLTipoRapp.BackColor = SEGNALA_OK

        txtDesRefInt.BackColor = SEGNALA_OK : txtDesRefInt.ToolTip = ""
        txtNoteDocumento.BackColor = SEGNALA_OK : txtNoteDocumento.ToolTip = ""
    End Sub
    Private Sub CampiSetEnabledToT(ByVal Valore As Boolean)
        btnRitorno.Enabled = Not Valore
        btnRitorno.Visible = Not Valore
        '---
        LnkRegimeIVA.Enabled = Valore
        LnkRegimeIVA.Visible = Valore
        '-
        'per adesso non è usato : usiamo il riferimento per indicare i dati del documento 
        If Valore = True Then 'SOLO PER MM CM e SM GIU190112
            If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
                Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.CampiSetEnabledToT)")
                Exit Sub
            End If
            If Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                LnkDatiDocMM.Enabled = Valore
                LnkDatiDocMM.Visible = Valore
            Else
                LnkDatiDocMM.Enabled = False
                LnkDatiDocMM.Visible = False
            End If
        Else
            LnkDatiDocMM.Enabled = Valore
            LnkDatiDocMM.Visible = Valore
        End If
        'per adesso non è usato : usiamo il riferimento per indicare i dati del documento
        LnkDatiDocMM.Enabled = False
        LnkDatiDocMM.Visible = False
        '---
        ' ''LnkAltriDatiDoc.Enabled = Valore
        ' ''LnkAltriDatiDoc.Visible = Valore

        ' ''btnModificaRegimeIVA.Enabled = Valore
        ' ''btnModificaRifDoc.Enabled = Valore
        'GIU230920
        ddlMagazzino.Enabled = Valore 'giu090820
        DDLMagazzino2.Enabled = Valore
        DDLCausali2.Enabled = Valore
        '---------
        txtNumero.Enabled = Valore : txtRevNDoc.Enabled = Valore : txtDataDoc.Enabled = Valore

        txtDataValidita.Enabled = Valore : txtDataConsegna.Enabled = Valore
        txtNGG_Validita.Enabled = Valore : txtNGG_Consegna.Enabled = Valore
        txtCIG.Enabled = Valore : txtCUP.Enabled = Valore
        txtRiferimento.Enabled = Valore : txtDataRif.Enabled = Valore : imgBtnShowCalendar0.Enabled = Valore
        txtCCommessa.Enabled = Valore
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
        txtPagamento.Enabled = Valore : DDLPagamento.Enabled = Valore
        btnLead.Enabled = Valore : DDLLead.Enabled = Valore
        btnCercaBanca.Enabled = Valore
        DDLBancheIBAN.Enabled = Valore

        txtCodAgente.Enabled = Valore : DDLAgenti.Enabled = Valore : btnAgenti.Enabled = Valore
        txtCodCausale.Enabled = Valore : DDLCausali.Enabled = Valore
        checkADeposito.Enabled = Valore 'giu260412
        txtListino.Enabled = Valore : DDLListini.Enabled = Valore

        txtTipoFatt.Enabled = Valore : DDLTipoFatt.Enabled = Valore : btnTipofatt.Enabled = Valore
        chkFatturaPA.Enabled = Valore 'GIU070814
        DDLTipoRapp.Enabled = Valore
        ChkAcconto.Enabled = Valore
        'giu281221
        optPagSconosciuto.Enabled = Valore
        optPagAnticipato.Enabled = Valore
        optPagEffettuato.Enabled = Valore
        optPagLista.Enabled = Valore
        '---------
        chkFatturaAC.Enabled = Valore
        chkScGiacenza.Enabled = Valore
        '-
        txtDesRefInt.Enabled = Valore
        txtNoteDocumento.Enabled = Valore
        WUC_DocumentiDett1.SetEnableTxt(Valore)
        WUC_DocumentiSpeseTraspTot1.SetEnableTxt(Valore)
    End Sub
#End Region

    Public Sub LnkRitorno_Click()
        If Session(SWOP) = SWOPELIMINA Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Chiudi("")
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnAnnulla)")
            Exit Sub
        End If
        If Session(SWMODIFICATO) = SWSI Then
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = "AnnullaModificheDocumento"
            ModalPopup.Show("Annulla modifiche documento", "Confermi l'annullamento modifiche del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            AnnullaModificheDocumento()
        End If
        LnkStampa.Visible = False 'giu200220
    End Sub
    Public Sub AnnullaModificheDocumento()
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
        Session(IDDOCUMENTI) = myID
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
        DsDocT.DocumentiT.Clear()
        SqlAdapDoc.Fill(DsDocT.DocumentiT)
        'popolo
        Session("DsDocT") = DsDocT
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsDocT.DocumentiT)
        End If
        Session("dvDocT") = dvDocT

        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            PopolaTxtDocT()
            Dim strErrore As String = ""
            WUC_DocumentiSpeseTraspTot1.SfondoCampiDocTTD2()
            WUC_DocumentiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, strErrore)
            'giu221111 ricarico anche i DETTAGLI ORIGINALI con TRUE
            WUC_DocumentiDett1.TD1ReBuildDett(True)
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
            WUC_DocumentiSpeseTraspTot1.SfondoCampiDocTTD2()
            WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2()
            'giu221111 ricarico anche i DETTAGLI ORIGINALI con TRUE
            WUC_DocumentiDett1.TD1ReBuildDett(True)
        End If

    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Documenti.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
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
            'giu120321
            Dim myChiamatoDa As String = Session(CSTChiamatoDa)
            'Session(CSTChiamatoDa) = "WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti"
            If IsNothing(myChiamatoDa) Then
                myChiamatoDa = ""
            ElseIf String.IsNullOrEmpty(myChiamatoDa) Then
                myChiamatoDa = ""
            End If
            If InStr(myChiamatoDa.Trim.ToUpper, "WF_ANAGRAFICACLIENTI") > 0 Then
                Session(CSTRitornoDaStampa) = True 'PER POSIZIONARMI SUL CLIENTE 
                Response.Redirect(myChiamatoDa)
                Session(COD_CLIENTE) = txtCodCliForFilProvv.Text.Trim
            ElseIf InStr(myChiamatoDa.Trim.ToUpper, "WF_Fatturazione".ToUpper) > 0 Then
                Response.Redirect(myChiamatoDa)
            ElseIf InStr(myChiamatoDa.Trim.ToUpper, "WF_ElencoDistinteSped".ToUpper) > 0 Then
                Response.Redirect(myChiamatoDa)
            ElseIf InStr(myChiamatoDa.Trim.ToUpper, "WF_ElencoMovMag".ToUpper) > 0 Then
                Response.Redirect(myChiamatoDa)
            ElseIf Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
                Response.Redirect("WF_ElencoPreventivi.aspx?labelForm=Gestione preventivi/offerte")
            ElseIf Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Then
                Response.Redirect("WF_ElencoPropostaRiordino.aspx?labelForm=Gestione proposte di riordino FORNITORI")
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                Response.Redirect("WF_ElencoOrdini.aspx?labelForm=Gestione ordini")
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
                Response.Redirect("WF_ElencoOrdiniFornitori.aspx?labelForm=Gestione ordini fornitori")
            ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                Response.Redirect("WF_ElencoMovMag.aspx?labelForm=Movimenti di magazzino")
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                Response.Redirect("WF_DocumentiElenco.aspx?labelForm=Gestione documenti")
            ElseIf myChiamatoDa <> "" Then
                Response.Redirect(myChiamatoDa)
            Else
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
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
        'LOTTI
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        LnkStampa.Visible = False 'giu200220
        BtnAggionaDocumento()

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
    Dim DataScad(12) As String
    Dim ImpRata(12) As Decimal
    'Blocco Trasporto
    Dim TipoSped As String = ""
    Dim Vettori(3) As Long
    Dim Porto As String = ""
    Dim DesPorto As String = ""
    Dim Aspetto As String = ""
    Dim Colli As Integer = 0
    Dim Pezzi As Integer = 0
    Dim Peso As Decimal = 0
    Dim DataOraRitiro As String = "" 'nota ritorna dd/mm/yyyy hh.mm COMPLETA di data+ora
    Dim NoteRitiro As String = ""
    Dim CKSWTipoEvSaldo As Boolean
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
        SWTB0 = True
        SWTB1 = True
        SWTB2 = True
        '-
        Dim myStrEsito As String = "" 'giu180518
        SWTB0 = ControllaDocT() 'qui aggiorna anche SWTB2
        Dim SWRicalcola As Boolean = False
        SWTB1 = ControllaDocD(myStrEsito)
        Dim myStrEsitoS As String = ""
        Dim mySWTB2 As Boolean = ControllaDocS(myStrEsitoS)
        If mySWTB2 = False Then
            SWTB2 = False
        End If
        '-
        If SWTB0 = False Or SWTB1 = False Or SWTB2 = False Then
            If SWTB0 = False Then
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
            ElseIf SWTB1 = False Then
                Session("TabDoc") = TB1
                TabContainer1.ActiveTabIndex = TB1
            ElseIf SWTB2 = False Then
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
            End If
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            If myStrEsito.Trim = "" And myStrEsitoS.Trim = "" Then 'giu180518
                ModalPopup.Show("Controllo dati documento", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Else
                ModalPopup.Show("Controllo dati documento", "Attenzione, i campi segnalati in rosso non sono validi <br> " & myStrEsito.Trim + " " + myStrEsitoS.Trim, WUC_ModalPopup.TYPE_ALERT)
            End If
            Exit Sub
        End If
        'giu090412
        If lblCliForFilProvv.ForeColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ControllaBollo"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
            ModalPopup.Show("Aggiorna documento", "ATTENZIONE, il cliente/fornitore risulta bloccato. <br> " _
                        & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        ControllaBollo()
    End Sub
    'giu040319
    Public Sub ControllaBollo()
        Session("TabDoc") = TB2
        TabContainer1.ActiveTabIndex = TB2
        'GIU040319 
        Dim ImpBollo As Decimal = 0 : Dim ImpMinBollo As Decimal = 0
        ImpBollo = GetParamGestAzi(Session(ESERCIZIO)).Bollo
        ImpMinBollo = GetParamGestAzi(Session(ESERCIZIO)).ImpMinBollo
        'giu210920()
        Dim myTipoDoc As String = ""
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            myTipoDoc = "" 'SE VOGLIO FORZARE MUOVO ==> SWTD(TD.DocTrasportoClienti)
        Else
            myTipoDoc = TipoDoc
        End If
        '---------
        Dim mySplitIVA As Boolean = False : Dim Nazione As String = "" : Dim strErrore As String = ""
        Call Documenti.CKClientiIPAByIDDocORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
        If strErrore.Trim <> "" And myTipoDoc <> SWTD(TD.MovimentoMagazzino) Then
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Controllo applicazione bollo (Controllo nazione)", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        SWTB2 = WUC_DocumentiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        DataOraRitiro, NoteRitiro, CKSWTipoEvSaldo, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
        If SWTB2 = False Then
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "WUC_DocumentiSpeseTraspTot1.GetDati (Lettura dati Totale Documento,Spese,Trasp.,Bollo)", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If ImpBollo <> 0 And ImpMinBollo <> 0 Then
            'OK SE PREVISTO OK BOLLO
        Else
            'NON APPLICAZIONE PER I STRANIERI
            SWTB2 = False
            WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
            Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
            ModalPopup.Show("Aggiorna documento", "ATTENZIONE, Parametri generali BOLLO non definiti. <br> " _
                        & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        If Bollo <> 0 And (Nazione.Trim <> "I" And Nazione.Trim <> "IT" And Nazione.Trim <> "ITA") Then
            'NON APPLICAZIONE PER I STRANIERI
            SWTB2 = False
            WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
            Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
            ModalPopup.Show("Aggiorna documento", "ATTENZIONE, BOLLO NON RICHIESTO! (Cliente straniero). <br> " _
                        & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        'ORA VERIFICO SE E' RICHIESTO IL BOLLO
        Dim myTotale As Decimal = 0
        Try
            Dim DsDocDettTmp As New DSDocumenti
            DsDocDettTmp = Session("aDsDett")
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
                For Each RowDettB In DsDocDettTmp.DocumentiD.Select("Importo>0")
                    If RowDettB.RowState <> DataRowState.Deleted Then
                        myTotale += RowDettB.Item("Importo")
                    End If
                Next
            Else
                For Each RowDettB In DsDocDettTmp.DocumentiD.Select("Importo>0")
                    If RowDettB.RowState <> DataRowState.Deleted Then
                        If (RowDettB.Item("OmaggioImponibile") = True And RowDettB.Item("OmaggioImposta") = True) Or RowDettB.Item("Cod_Iva") > 49 Then
                            myTotale += RowDettB.Item("Importo")
                        End If
                    End If
                Next
            End If

        Catch ex As Exception
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Controllo applicazione bollo (Calcolo totale imponibile)", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '-
        If myTotale > ImpMinBollo Then
            If Bollo = 0 Then
                If (Nazione.Trim <> "I" And Nazione.Trim <> "IT" And Nazione.Trim <> "ITA") Then
                    'OK PER ESTERI NO BOLLO
                Else
                    SWTB2 = False
                    WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
                    BolloACaricoDel = ""
                    Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
                    ModalPopup.Show("Aggiorna documento", "ATTENZIONE, BOLLO RICHIESTO! (ImpMinBollo superiore). <br> " _
                                & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                    Exit Sub
                End If

            ElseIf BolloACaricoDel.Trim = "" Then
                If (Nazione.Trim <> "I" And Nazione.Trim <> "IT" And Nazione.Trim <> "ITA") Then
                    'OK PER ESTERI NO BOLLO
                Else
                    btnAggiorna.Enabled = True
                    btnAnnulla.Enabled = True
                    SWTB2 = False
                    WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Manca il dato a carico del ", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            ElseIf Bollo <> ImpBollo Then
                SWTB2 = False
                WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
                Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
                ModalPopup.Show("Aggiorna documento", "ATTENZIONE, BOLLO DIVERSO DA QUELLO DEFINITO nei parametri generali!. <br> " _
                            & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                Exit Sub
            End If
        ElseIf Bollo <> 0 Then
            SWTB2 = False
            WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
            Session(MODALPOPUP_CALLBACK_METHOD) = "OkAggiornaDoc"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "NoAggiornaDoc"
            ModalPopup.Show("Aggiorna documento", "ATTENZIONE, BOLLO NON RICHIESTO! (ImpMinBollo inferiore). <br> " _
                        & "Si vuole procedere all'aggiornamento del documento ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        '----------------------------------------
        Call OkAggiornaDoc()
    End Sub
    'giu090412
    Public Sub OkAggiornaDoc()
        SWTB0 = True
        SWTB1 = True
        SWTB2 = True
        'ok aggiorno
        Dim DsDettBack As New DSDocumenti
        SWTB1 = AggiornaDocD(DsDettBack) 'giu221111 si aggiorno 
        Dim strErrore As String = ""
        SWTB0 = AggiornaDocT(DsDettBack, False, strErrore) 'giu160118 aggiorna sia SWTB0 che SWTB2 = True

        If SWTB0 = False Or SWTB1 = False Or SWTB2 = False Then
            'commentato perchè faccio lo show dell'errore nella funzione di AGGIORNA
            'altrimenti segnala l'ultimo che è questo e non quello nella funzione
            If SWTB0 = False Then
                Session("TabDoc") = TB0
                TabContainer1.ActiveTabIndex = TB0
            ElseIf SWTB1 = False Then
                Session("TabDoc") = TB1
                TabContainer1.ActiveTabIndex = TB1
            ElseIf SWTB2 = False Then
                Session("TabDoc") = TB2
                TabContainer1.ActiveTabIndex = TB2
            End If
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
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
        If myID = "" Or Not IsNumeric(myID) Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlAdapDoc.Fill(DsDocT.DocumentiT)
        Session("DsDocT") = DsDocT
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsDocT.DocumentiT)
        End If
        Session("dvDocT") = dvDocT
        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            PopolaTxtDocT()
            WUC_DocumentiSpeseTraspTot1.PopolaTxtDocTTD2(dvDocT, strErrore)
            WUC_DocumentiDett1.TD1ReBuildDett()
        Else
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
            WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2()
            Session(IDDOCUMENTI) = ""
            WUC_DocumentiDett1.TD1ReBuildDett()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiorna dati documento", "Attenzione, nessun documento aggiornato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        'GIU020514 giu260814
        Dim myTipoDoc As String = ""
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            myTipoDoc = "" 'SE VOGLIO FORZARE MUOVO ==> SWTD(TD.DocTrasportoClienti)
        Else
            myTipoDoc = TipoDoc
        End If
        'GIU260814 PER VELOCIZZARE IL SALVATAGGIO
        'GIU150618 OK ALTRI TEST SONO NELLA SP
        'giu100718 TD.FatturaCommerciale solo per gli ordini fatturati
        If myTipoDoc = SWTD(TD.FatturaCommerciale) Or chkFatturaAC.Checked = True Or _
           myTipoDoc = SWTD(TD.BuonoConsegna) Or myTipoDoc = SWTD(TD.DocTrasportoCLavoro) Or myTipoDoc = SWTD(TD.FatturaScontrino) Or _
           myTipoDoc = SWTD(TD.DocTrasportoClienti) Or myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
           myTipoDoc = SWTD(TD.CaricoMagazzino) Or _
           myTipoDoc = SWTD(TD.ScaricoMagazzino) Or _
           myTipoDoc = SWTD(TD.MovimentoMagazzino) Then
            Session("GetScadProdCons") = ""
            Dim SQLStr As String = ""
            Dim ObjDB As New DataBaseUtility
            Try
                SQLStr = "EXEC Carica_ArticoliInst_ContrattiAss " & myID.Trim
                If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Carico prodotti installati", "Errore, carico prodotti installati.", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Carico prodotti installati", "Errore, carico prodotti installati.: " & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End Try
            ObjDB = Nothing
        End If
        '-
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
        Dim strMess As String = ""
        'GIU130121 Richiesta Zibordi
        Dim RegimeIVADiverso As String = Session("RegimeIVADiverso")
        If IsNothing(RegimeIVADiverso) Then
            RegimeIVADiverso = SWNO
        ElseIf String.IsNullOrEmpty(RegimeIVADiverso) Then
            RegimeIVADiverso = SWNO
        End If
        If RegimeIVADiverso = SWSI Then
            strMess += "!!!NOTA!!! Regime IVA diverso da quello definito in Anagrafica articoli <br> IVA Articoli !!!SOSTITUITO!!!"
        End If
        '---------
        If txtNumero.BackColor = SEGNALA_KO Then 'GIU260819 CONTROLLO SEQUENZIALITA' NUMERI DOCUMENTO
            Session("TabDoc") = TB0
            TabContainer1.ActiveTabIndex = TB0
            strMess += "<br>!!!ERRORE!!! N.Documento !!!VERIFICARE !!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
           TipoDoc = SWTD(TD.FatturaCommerciale) Or _
           TipoDoc = SWTD(TD.FatturaScontrino) Then
            If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                Exit Sub
            End If
            If myStatoDoc = "1" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "" 'giu200423 default altrimenti non disabilita aggiorna annulla fino a aquando,,,,,,NoAggiornaDoc
                If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
                    strMess += "<br>Il DDT è stato fatturato, <br>assicuratevi che la relativa fattura venga corretta."
                    If CKRagSoc35() = False Then
                        strMess += "<br>Ragione Sociale > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKRiferim35() = False Then
                        strMess += "<br>Riferimento > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKTelObb() = False Then
                        strMess += "<br>Manca il N° di Telefono.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                Else
                    strMess += "<br>Il documento è stato trasferito in CoGe, <br>assicuratevi che venga corretto anche in CoGe/Ri.Ba.."
                End If
                If CKPIVACFObb() = False Then
                    strMess += "<br>Mancano P.IVA / C.Fiscale."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                If CKIPAObb() = False Then
                    strMess += "<br>Manca Codice IPA."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf strMess.Trim <> "" Then 'GIU130121
                If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                    Exit Sub
                End If
                'giu200423 Ilaria 13/04 nessun controllo spedizioni solo DDT
                Session(MODALPOPUP_CALLBACK_METHOD) = "" 'giu200423 default altrimenti non disabilita aggiorna annulla fino a aquando,,,,,,NoAggiornaDoc
                If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
                    If CKRagSoc35() = False Then 'GIU050423
                        strMess += "<br>Ragione Sociale > 35 caratteri o mancante.(Obbligatorio per la  Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKRiferim35() = False Then 'GIU050423
                        strMess += "<br>Riferimento > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKTelObb() = False Then 'GIU050423
                        strMess += "<br>Manca il N° di Telefono.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                End If
                If CKPIVACFObb() = False Then
                    strMess += "<br>Mancano P.IVA / C.Fiscale."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                If CKIPAObb() = False Then
                    strMess += "<br>Manca Codice IPA."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            Else
                If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                    Exit Sub
                End If
                'giu200423 Ilaria 13/04 nessun controllo spedizioni solo DDT
                Session(MODALPOPUP_CALLBACK_METHOD) = "" 'giu200423 default altrimenti non disabilita aggiorna annulla fino a aquando,,,,,,NoAggiornaDoc
                If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
                    If CKRagSoc35() = False Then 'GIU050423
                        strMess += "<br>Ragione Sociale > 35 caratteri o mancante.(Obbligatorio per la  Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKRiferim35() = False Then 'GIU050423
                        strMess += "<br>Riferimento > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                    If CKTelObb() = False Then 'GIU050423
                        strMess += "<br>Manca il N° di Telefono.(Obbligatorio per la Spedizione)"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                    End If
                End If
                If CKPIVACFObb() = False Then
                    strMess += "<br>Mancano P.IVA / C.Fiscale."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                If CKIPAObb() = False Then
                    strMess += "<br>Manca Codice IPA."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "NoAggiornaDocOBB"
                End If
                If strMess.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
            End If
        ElseIf strMess.Trim <> "" Then 'GIU130121
            'giu200423 Ilaria 13/04 nessun controllo spedizioni DDT
            '''If CKTelObb() = False And (TipoDoc = SWTD(TD.Preventivi) Or TipoDoc = SWTD(TD.OrdClienti)) Then 'GIU060423
            '''    strMess += "<br>Manca il N° di Telefono.(Obbligatorio per la Spedizione)"
            '''End If
            '''If CKRagSoc35() = False And TipoDoc = SWTD(TD.DocTrasportoClienti) Then 'GIU050423
            '''    strMess += "<br>Ragione Sociale > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
            '''End If
            '''If CKRiferim35() = False And TipoDoc = SWTD(TD.DocTrasportoClienti) Then 'GIU050423
            '''    strMess += "<br>Riferimento > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
            '''End If
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf (TipoDoc = SWTD(TD.Preventivi) Or TipoDoc = SWTD(TD.OrdClienti)) Then 'GIU060423
            If String.IsNullOrEmpty(Session("SWNOTEAVVISO")) Then 'giu281123
                Dim strNoteAvviso As String = Session("SWNOTEAVVISO").ToString.Trim
                If strNoteAvviso.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("AVVISO", strNoteAvviso, WUC_ModalPopup.TYPE_INFO)
                End If
            End If

            'giu200423 Ilaria 13/04 nessun controllo spedizioni
            '''If CKRagSoc35() = False Then 'GIU050423
            '''    strMess += "<br>Ragione Sociale > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
            '''End If
            '''If CKRiferim35() = False Then 'GIU050423
            '''    strMess += "<br>Riferimento > 35 caratteri o mancante.(Obbligatorio per la Spedizione)"
            '''End If
            '''If CKTelObb() = False Then 'GIU050423
            '''    strMess += "<br>Manca il N° di Telefono.(Obbligatorio per la Spedizione)"
            '''End If
            ''''-
            '''If strMess.Trim <> "" Then
            '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '''    ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_ALERT)
            '''    Exit Sub
            '''End If
        End If
        '---------
    End Sub
    Private Function CKPIVACFObb() As Boolean
        CKPIVACFObb = True
        If Not String.IsNullOrEmpty(Session("CKPIVACF")) Then
            If Session("CKPIVACF") = "OK" Then
                CKPIVACFObb = True
            Else
                CKPIVACFObb = False
            End If
        Else
            CKPIVACFObb = False
        End If
    End Function
    Private Function CKIPAObb() As Boolean
        CKIPAObb = True
        If Not String.IsNullOrEmpty(Session("CKIPAObb")) Then
            If Session("CKIPAObb") = "OK" Then
                CKIPAObb = True
            Else
                CKIPAObb = False
            End If
        Else
            CKIPAObb = False
        End If
    End Function
    Private Function CKTelObb() As Boolean
        CKTelObb = True
        'GIU290722 giu090822
        Dim SWSpedVettCsv As Boolean = True
        If IsNothing(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf String.IsNullOrEmpty(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf Session(CSTCKSPEDVETT) <> SWSI Then
            SWSpedVettCsv = False
        End If
        If Not String.IsNullOrEmpty(Session("CKTelOBB")) Then
            If Session("CKTelOBB") = "OK" Then
                SWSpedVettCsv = False
            End If
        End If
        If Not String.IsNullOrEmpty(Session("CKTelOBBSede")) Then
            If Session("CKTelOBBSede") = "OK" Then
                SWSpedVettCsv = False
            End If
        End If
        If SWSpedVettCsv = True Then
            If String.IsNullOrEmpty(Session("CKTelOBB")) And String.IsNullOrEmpty(Session("CKTelOBBSede")) Then
                CKTelObb = False
            ElseIf Session("CKTelOBB") <> "OK" And Session("CKTelOBBSede") <> "OK" Then
                CKTelObb = False
            End If
        End If
        '-
    End Function
    Private Function CKRagSoc35() As Boolean
        CKRagSoc35 = True
        Dim SWSpedVettCsv As Boolean = True
        If IsNothing(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf String.IsNullOrEmpty(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf Session(CSTCKSPEDVETT) <> SWSI Then
            SWSpedVettCsv = False
        End If
        If Not String.IsNullOrEmpty(Session("CKRagSoc35")) Then
            If Session("CKRagSoc35") = "OK" Then
                SWSpedVettCsv = False
            End If
        End If
        'giu200423 Ilaria 13/04 destMerce OBB
        '''If Not String.IsNullOrEmpty(Session("CKRagSocSede35")) Then
        '''    If Session("CKRagSocSede35") = "OK" Then
        '''        SWSpedVettCsv = False
        '''    End If
        '''End If
        If SWSpedVettCsv = True Then
            'giu200423 Ilaria 13/04 destMerce OBB
            'If String.IsNullOrEmpty(Session("CKRagSoc35")) And String.IsNullOrEmpty(Session("CKRagSocSede35")) Then
            '    CKRagSoc35 = False
            'ElseIf Session("CKRagSoc35") <> "OK" And Session("CKRagSocSede35") <> "OK" Then
            '    CKRagSoc35 = False
            'End If
            If String.IsNullOrEmpty(Session("CKRagSoc35")) Then
                CKRagSoc35 = False
            ElseIf Session("CKRagSoc35") <> "OK" Then
                CKRagSoc35 = False
            End If
        End If
        '-
    End Function
    Private Function CKRiferim35() As Boolean
        CKRiferim35 = True
        Dim SWSpedVettCsv As Boolean = True
        If IsNothing(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf String.IsNullOrEmpty(Session(CSTCKSPEDVETT)) Then
            SWSpedVettCsv = False
        ElseIf Session(CSTCKSPEDVETT) <> SWSI Then
            SWSpedVettCsv = False
        End If
        If Not String.IsNullOrEmpty(Session("CKRiferim35")) Then
            If Session("CKRiferim35") = "OK" Then
                SWSpedVettCsv = False
            End If
        End If
        'giu200423 Ilaria 13/04 destMerce OBB
        'If Not String.IsNullOrEmpty(Session("CKRiferimSede35")) Then
        '    If Session("CKRiferimSede35") = "OK" Then
        '        SWSpedVettCsv = False
        '    End If
        'End If
        If SWSpedVettCsv = True Then
            'giu200423 Ilaria 13/04 destMerce OBB
            'If String.IsNullOrEmpty(Session("CKRiferim35")) And String.IsNullOrEmpty(Session("CKRiferimSede35")) Then
            '    CKRiferim35 = False
            'ElseIf Session("CKRiferim35") <> "OK" And Session("CKRiferimSede35") <> "OK" Then
            '    CKRiferim35 = False
            'End If
            If String.IsNullOrEmpty(Session("CKRiferim35")) Then
                CKRiferim35 = False
            ElseIf Session("CKRiferim35") <> "OK" Then
                CKRiferim35 = False
            End If
        End If
        '-
    End Function
    'GIU120412
    Public Sub NoAggiornaDoc()
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
    End Sub

    'GIU200423
    Public Sub NoAggiornaDocOBB()
        btnModifica_Click(Nothing, Nothing)
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
        If ddlMagazzino.SelectedIndex = 0 Then
            ddlMagazzino.BackColor = SEGNALA_KO : ControllaDocT = False
        Else
            ddlMagazzino.BackColor = SEGNALA_OK
        End If
        Call CKPrezzoALCSG("", "")
        'GIU230920()
        If ddlMagazzino.SelectedValue = DDLMagazzino2.SelectedValue And DDLMagazzino2.Visible = True Then
            ddlMagazzino.BackColor = SEGNALA_KO
            DDLMagazzino2.BackColor = SEGNALA_KO
            lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
            lblCausale2.Visible = True : DDLCausali2.Visible = True
            ControllaDocT = False
        ElseIf DDLMagazzino2.SelectedValue.Trim = "" And DDLMagazzino2.Visible = True Then
            ddlMagazzino.BackColor = SEGNALA_KO
            DDLMagazzino2.BackColor = SEGNALA_KO
            lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
            lblCausale2.Visible = True : DDLCausali2.Visible = True
            ControllaDocT = False
        ElseIf ddlMagazzino.SelectedIndex > 0 Then
            ddlMagazzino.BackColor = SEGNALA_OK
            DDLMagazzino2.BackColor = SEGNALA_OK
        End If
        If DDLCausali.SelectedValue = DDLCausali2.SelectedValue And DDLCausali2.Visible = True Then
            DDLCausali.BackColor = SEGNALA_KO
            DDLCausali2.BackColor = SEGNALA_KO
            lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
            lblCausale2.Visible = True : DDLCausali2.Visible = True
            ControllaDocT = False
        ElseIf DDLCausali2.SelectedValue.Trim = "" And DDLCausali2.Visible = True Then
            DDLCausali.BackColor = SEGNALA_KO
            DDLCausali2.BackColor = SEGNALA_KO
            lblMagazzino2.Visible = True : DDLMagazzino2.Visible = True
            lblCausale2.Visible = True : DDLCausali2.Visible = True
            ControllaDocT = False
        Else
            DDLCausali.BackColor = SEGNALA_OK
            DDLCausali2.BackColor = SEGNALA_OK
        End If
        '---------
        'GIU070814
        'GIU040319 CONTROLLO SPLITIVA / BOLLO APPLICAZIONE
        SWTB2 = WUC_DocumentiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        DataOraRitiro, NoteRitiro, CKSWTipoEvSaldo, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)

        Dim mySplitIVA As Boolean = False : Dim Nazione As String = ""
        If txtCodCliForFilProvv.Text.Trim <> "" And chkFatturaPA.Visible = True Then 'GIU030219
            SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
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
            Call Documenti.CKClientiIPAByIDDocORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore, Nazione)
        End If
        '-
        If SplitIVA <> mySplitIVA Then
            WUC_DocumentiSpeseTraspTot1.SetErrChkSplitIVA(False)
            SWTB2 = False
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
        'GIU130219 giu221021 BLOCCO escluso il MAGAZZINO
        If SWRifDoc = True And txtRiferimento.Text.Trim.Length > 20 Then
            txtRiferimento.BackColor = SEGNALA_KO : txtRiferimento.ToolTip = "Lunghezza massima consentita 20 caratteri a partire dal 01/01/2019"
            If Not (Session(CSTTIPOUTENTE).Equals(CSTMAGAZZINO)) Then ControllaDocT = False
        ElseIf SWRifDoc = True And txtRiferimento.Text.Trim.Length = 0 Then 'giu211021 rif email Erika 07/10/2021
            txtRiferimento.BackColor = SEGNALA_KO : txtRiferimento.ToolTip = "Riferimento obbligatorio"
            If Not (Session(CSTTIPOUTENTE).Equals(CSTMAGAZZINO)) Then ControllaDocT = False
        Else
            txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = ""
        End If
        If txtDataRif.Text.Trim <> "" Then
            If Not IsDate(txtDataRif.Text.Trim) Then txtDataRif.BackColor = SEGNALA_KO : txtDataRif.ToolTip = "Data errata" : ControllaDocT = False
            If SWRifDoc = True Then
                If txtRiferimento.Text.Trim = "" Then txtDataRif.BackColor = SEGNALA_KO : txtDataRif.ToolTip = "Riferimento obbligatorio se presente la data" : If Not (Session(CSTTIPOUTENTE).Equals(CSTMAGAZZINO)) Then ControllaDocT = False
            End If
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
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        ElseIf strErrore.Trim <> "" Then
            strErrore = strErrore.Trim
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Or Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then 'GIU080722 SOLO FORNITORI ALTRIMENTI FA CASINO 
            If Left(txtCodCliForFilProvv.Text.Trim, 1) <> "9" Then
                txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        ElseIf lblCliForFilProvv.Text.Trim = "" Then
            txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
        End If

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
        If RKCausMag.CVisione = True And checkADeposito.Checked = True Then
            strErrore = "CODICE CAUSALE NON VALIDO PER IL C/DEPOSITO"
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
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
        'giu290412 altri controlli
        If Not String.IsNullOrEmpty(RKCausMag.Segno_Giacenza) Then
            If Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdDepositi) Or _
                Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                If RKCausMag.Segno_Giacenza = "+" Then
                    strErrore = "CODICE CAUSALE NON VALIDO PER QUESTO TIPO DOCUMENTO"
                    txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            Else
                If RKCausMag.Segno_Giacenza = "-" And Session(CSTTIPODOC) <> SWTD(TD.MovimentoMagazzino) Then
                    strErrore = "CODICE CAUSALE NON VALIDO PER QUESTO TIPO DOCUMENTO"
                    txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        End If
        txtCodCausale.ToolTip = strErrore
        '-----------------------------
        If txtListino.Text.Trim = "" Or txtListino.Text.Trim = "0" Then
            txtListino.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLListini.BackColor = SEGNALA_KO
        End If
        If txtCodCausale.Text.Trim = "" Or txtCodCausale.Text.Trim = "0" Or Not IsNumeric(txtCodCausale.Text.Trim) Then
            txtCodCausale.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLCausali.BackColor = SEGNALA_KO
        End If
        If txtTipoFatt.Text.Trim = "" Or txtTipoFatt.Text.Trim = "0" Then
            txtTipoFatt.BackColor = SEGNALA_KO : ControllaDocT = False
            DDLTipoFatt.BackColor = SEGNALA_KO
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
        Dim strSQL As String = ""
        strSQL = "Select TOP 1 Data_Doc From DocumentiT WHERE "
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
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
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
        Dim strSQL As String = ""
        strSQL = "Select MIN(CONVERT(INT, Numero)) AS MinN, MAX(CONVERT(INT, Numero)) AS MaxN "
        strSQL += "From DocumentiT WHERE "
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
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
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
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS MaxN "
        strSQL += "From DocumentiT "
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
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
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
        'GIU180518
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (WUC_Documenti.ControllaDocD)")
            Exit Function
        End If
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
        If myStatoDoc = "5" And TipoDoc = SWTD(TD.OrdClienti) Then
            Try
                OKQtaEv = False
                For Each RowDett In DsDocDettTmp.DocumentiD
                    If RowDett.RowState <> DataRowState.Deleted Then
                        If RowDett.Item("Qta_Evasa") > 0 Then
                            OKQtaEv = True
                            Exit For
                        End If
                    End If
                Next
                If OKQtaEv = False Then
                    ControllaDocD = False
                    myStrEsito = "Attenzione, inserire almeno una quantità d'allestire"
                    Exit Function
                Else
                    ControllaDocD = True
                End If
            Catch ex As Exception
                ControllaDocD = False
                myStrEsito = "Errore, controllo quantità d'allestire"
                Exit Function
            End Try
        Else
            ControllaDocD = True
        End If
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
        For Each rsDettagli In DsDocDettTmp.Tables("DocumentiD").Select("", "Riga")
            Select Case Left(TipoDoc, 1)
                Case "O"
                    myQuantita = rsDettagli![Qta_Ordinata]
                Case Else
                    If TipoDoc = "PR" Or TipoDoc = "PF" Then
                        myQuantita = rsDettagli![Qta_Ordinata]
                    Else
                        myQuantita = rsDettagli![Qta_Evasa]
                    End If
            End Select
            'giu020519 FATTURE PER ACCONTI 
            rsDettagli.BeginEdit()
            If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
            rsDettagli.EndEdit()
            rsDettagli.AcceptChanges()
            'CONTROLLO IMPORTO CHE SIANO UGLUALI
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

    Private Function ControllaDocS(ByRef strEsito As String) As Boolean
        ControllaDocS = True
        strEsito = ""
        'cntrollo VETTORE SE RICHIESTO 
        ControllaDocS = WUC_DocumentiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                        SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                        ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                        DataOraRitiro, NoteRitiro, CKSWTipoEvSaldo, SplitIVA, RitAcconto, _
                        ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
        'giu160118
        If RitAcconto = True Then
            Dim ErrRitAcc As Boolean = False
            If ImponibileRA = 0 Then
                ControllaDocS = False
                ErrRitAcc = True
            End If
            If PercRA = 0 Then
                ControllaDocS = False
                ErrRitAcc = True
            End If
            If TotaleRA = 0 Then
                ControllaDocS = False
                ErrRitAcc = True
            End If
            If ErrRitAcc = True Then
                WUC_DocumentiSpeseTraspTot1.SetErrRitAcc(False)
            End If
        End If
        '---------
        'GIU040319 OK PER IL BOLLO MENTRE LO SPLIT IVA E' CONTROLLATO IN ControllaDocT
        If Bollo = 0 Then
            BolloACaricoDel = ""
            'myBolloSI VERRA' CONTROLLATO DOPO
        ElseIf BolloACaricoDel.Trim = "" Then
            ControllaDocS = False
            WUC_DocumentiSpeseTraspTot1.SetErrBollo(False)
            'myBolloSI VERRA' CONTROLLATO DOPO
        End If
        Dim myTipoDoc As String = ""
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            myTipoDoc = "" 'SE VOGLIO FORZARE MUOVO ==> SWTD(TD.DocTrasportoClienti)
        Else
            myTipoDoc = TipoDoc
        End If
        If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            If Vettori(0) <> 0 Then 'giu070523
                'ok
            ElseIf TipoSped.Trim = "V" Then
                strEsito = "VETTORE OBBLIGATORIO"
                ControllaDocS = False
            End If
        End If
        
    End Function

    Public Function GetDatiTB3() As Boolean
        GetDatiTB3 = WUC_DocumentiSpeseTraspTot1.GetDati(TotMerce, ScCassa, TotMerceLordo, SpIncasso, _
                     SpTrasp, Abbuono, SpImballo, DesImballo, Iva, Imponibile, Imposta, DataScad, _
                     ImpRata, TipoSped, Vettori, Porto, DesPorto, Aspetto, Colli, Pezzi, Peso, _
                     DataOraRitiro, NoteRitiro, CKSWTipoEvSaldo, SplitIVA, RitAcconto, _
                     ImponibileRA, PercRA, TotaleRA, TotNettoPagare, Bollo, BolloACaricoDel)
    End Function

    Public Function AggiornaDocT(ByVal DSDettBack As DSDocumenti, ByVal SWNonAggNumDoc As Boolean, ByRef strErrore As String) As Boolean

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Session(ERROREALL) = SWSI
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.AggiornaDocT)")
            Exit Function
        End If
        If SWNonAggNumDoc = False Then
            If AggiornaNumDoc(strErrore) = False Then
                'giu140512
                txtNumero.BackColor = SEGNALA_KO
                txtNumero.ToolTip = strErrore
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore Aggiornamento numero documento", strErrore, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
                '---------
            End If
        End If
        AggiornaDocT = True
        Dim SWErr As Boolean = False
        '--------------------------------------------------------------------------------------
        'giu231211 giu241211 CALCOLO IL TOTALE SPESE, TRASPORTO, TOTALE SE HO SOLO IMPUTATO UNO DI QUESTI
        If Not (DSDettBack Is Nothing) Then
            Session(CSTIDPAG) = txtPagamento.Text.Trim 'giu020914 
            If WUC_DocumentiDett1.AggiornaImporto(DSDettBack, strErrore) = False Then
                If strErrore.Trim <> "" Then
                    AggiornaDocT = False 'giu160118
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            End If
        End If
        'Chiamo TD0 per richiamare TD3 SPESE,TRASPORTO,TOTALE,SCADENZE AGGIORNARE OK FATTO
        GetDatiTB3() 'TD3 Spese, Trasporto, Totale
        'giu160118
        If RitAcconto = True Then
            If ImponibileRA = 0 Then SWTB2 = False
            If PercRA = 0 Then SWTB2 = False
            If TotaleRA = 0 Then SWTB2 = False
        End If
        '---------
        '--------------------------------------------------------------------------------------
        txtRevNDoc.AutoPostBack = False
        If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True
        If Not IsNumeric(txtNGG_Consegna.Text.Trim) Then txtNGG_Consegna.Text = "0"
        If Not IsNumeric(txtNGG_Validita.Text.Trim) Then txtNGG_Validita.Text = "0"
        If Session(SWOP) = SWOPNUOVO Then
            Dim newRowDocT As DSDocumenti.DocumentiTRow = DsDocT.DocumentiT.NewDocumentiTRow
            With newRowDocT
                .BeginEdit()
                If ddlMagazzino.SelectedIndex = 0 Then
                    .CodiceMagazzino = 0
                Else
                    .CodiceMagazzino = ddlMagazzino.SelectedValue.Trim
                End If
                'GIU230920
                If DDLMagazzino2.SelectedIndex = 0 Or DDLMagazzino2.Visible = False Then
                    .SetCodiceMagazzinoM2Null()
                Else
                    .CodiceMagazzinoM2 = DDLMagazzino2.SelectedValue.Trim
                End If
                If DDLCausali2.SelectedIndex = 0 Or DDLCausali2.Visible = False Then
                    .SetCod_CausaleM2Null()
                Else
                    .Cod_CausaleM2 = DDLCausali2.SelectedValue.Trim
                End If
                '---------
                .Tipo_Doc = Session(CSTTIPODOC)
                If SWNonAggNumDoc = False Then
                    .Numero = txtNumero.Text.Trim
                    .RevisioneNDoc = Int(txtRevNDoc.Text.Trim)
                End If
                If IsDate(txtDataDoc.Text) Then
                    .Data_Doc = CDate(txtDataDoc.Text)
                    Session(CSTDATADOC) = txtDataDoc.Text.Trim
                Else
                    .SetData_DocNull()
                    Session(CSTDATADOC) = ""
                End If
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
                .Riferimento = txtRiferimento.Text.Trim + IIf(txtCCommessa.Text.Trim <> "", "§" + txtCCommessa.Text.Trim, "") 'giu250321
                'giu071211
                If IsDate(txtDataRif.Text) Then
                    .Data_Riferimento = CDate(txtDataRif.Text)
                Else
                    .SetData_RiferimentoNull()
                End If
                '-
                If optTipoEvTotaleTotale.Checked = True Then
                    .SWTipoEvTotale = True
                ElseIf optTipoEvTotaleParziale.Checked = True Then
                    .SWTipoEvTotale = False
                Else
                    .SetSWTipoEvTotaleNull()
                End If
                '-
                If CKSWTipoEvSaldo = True Then
                    .SWTipoEvSaldo = True
                Else
                    .SWTipoEvSaldo = False
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
                '.
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
                        myCGDest = lblDestSel.ToolTip
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
                If Not IsNumeric(txtPagamento.Text.Trim) Or txtPagamento.Text.Trim = "0" Then
                    .SetCod_PagamentoNull()
                Else
                    .Cod_Pagamento = txtPagamento.Text.Trim
                End If
                If Not IsNumeric(DDLLead.SelectedValue.Trim) Or DDLLead.SelectedValue.Trim = "0" Then
                    .SetNumPagineTotNull()
                Else
                    .NumPagineTot = DDLLead.SelectedValue.Trim
                End If
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
                If Not IsNumeric(txtCodAgente.Text.Trim) Or txtCodAgente.Text.Trim = "0" Then
                    .SetCod_AgenteNull()
                    Session(IDAGENTE) = ""
                Else
                    .Cod_Agente = txtCodAgente.Text.Trim
                    Session(IDAGENTE) = txtCodAgente.Text.Trim
                End If
                If Not IsNumeric(txtCodCausale.Text.Trim) Or txtCodCausale.Text.Trim = "0" Then
                    .SetCod_CausaleNull()
                Else
                    .Cod_Causale = txtCodCausale.Text.Trim
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
                'giu281221 GIU080722 ACCONTO/SALDO PER LE FATTURE 
                If Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                    .Acconto = IIf(ChkAcconto.Checked, 1, 0)
                ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                    If optPagSconosciuto.Checked Then
                        .Acconto = 0
                    ElseIf optPagAnticipato.Checked Then
                        .Acconto = 1
                    ElseIf optPagEffettuato.Checked Then
                        .Acconto = 2
                    ElseIf optPagLista.Checked Then
                        .Acconto = 3
                    Else
                        .Acconto = 0
                    End If
                Else
                    .Acconto = 0
                End If
                '---------
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
                'Blocco Data Scadenza
                If DataScad(0) <> "" Then
                    .Data_Scadenza_1 = CDate(DataScad(0))
                Else
                    .SetData_Scadenza_1Null()
                End If
                If DataScad(1) <> "" Then
                    .Data_Scadenza_2 = CDate(DataScad(1))
                Else
                    .SetData_Scadenza_2Null()
                End If
                If DataScad(2) <> "" Then
                    .Data_Scadenza_3 = CDate(DataScad(2))
                Else
                    .SetData_Scadenza_3Null()
                End If
                If DataScad(3) <> "" Then
                    .Data_Scadenza_4 = CDate(DataScad(3))
                Else
                    .SetData_Scadenza_4Null()
                End If
                If DataScad(4) <> "" Then
                    .Data_Scadenza_5 = CDate(DataScad(4))
                Else
                    .SetData_Scadenza_5Null()
                End If
                'Blocco Scadenze RATE
                .Rata_1 = CDec(ImpRata(0))
                .Rata_2 = CDec(ImpRata(1))
                .Rata_3 = CDec(ImpRata(2))
                .Rata_4 = CDec(ImpRata(3))
                .Rata_5 = CDec(ImpRata(4))
                'giu020321
                Dim NScad As Integer = 0 'giu040520 memorizzo le 5 rate non ancora evase per le statistiche
                Dim myScadPag As String = ""
                If Not (Session(CSTSCADPAG) Is Nothing) Then
                    Dim ArrScadPag As ArrayList
                    ArrScadPag = Session(CSTSCADPAG)
                    Dim rowScadPag As ScadPagEntity = Nothing
                    Dim Cont As Integer = 0
                    For i = 0 To ArrScadPag.Count - 1
                        If myScadPag.Trim <> "" Then myScadPag += ";"
                        rowScadPag = ArrScadPag(i)
                        If rowScadPag.Data <> "" Then
                            NScad += 1
                            If NScad < 6 Then
                                .Item("Data_Scadenza_" & Format(NScad, "#0") & "") = CDate(rowScadPag.Data.ToString.Trim)
                                .Item("Rata_" & Format(NScad, "#0") & "") = CDec(rowScadPag.Importo.ToString.Trim)
                            End If
                        End If
                        myScadPag += rowScadPag.NRata.ToString.Trim & ";"
                        myScadPag += rowScadPag.Data.ToString.Trim & ";"
                        myScadPag += rowScadPag.Importo.ToString.Trim & ";"
                    Next
                Else
                    For i = 0 To 4
                        If DataScad(i) <> "" Then
                            If myScadPag.Trim <> "" Then myScadPag += ";"
                            myScadPag += (i + 1).ToString.Trim & ";"
                            myScadPag += DataScad(i).Trim & ";"
                            myScadPag += ImpRata(i).ToString.Trim & ";"
                        End If
                    Next
                End If
                .Item("NoteNonEvasione") = myScadPag.Trim
                '---------------------------------------------------------------------
                If DataOraRitiro <> "" Then
                    If IsDate(DataOraRitiro) Then
                        .DataOraRitiro = CDate(DataOraRitiro)
                    Else
                        .SetDataOraRitiroNull()
                    End If
                Else
                    .SetDataOraRitiroNull()
                End If
                .Descrizione_Imballo = DesImballo
                .DescrizionePorto = DesPorto
                '-----------------------------
                If txtTipoFatt.Text.Trim = "" Then
                    .SetTipo_FatturazioneNull()
                Else
                    .Tipo_Fatturazione = txtTipoFatt.Text.Trim
                End If
                .DesRefInt = txtDesRefInt.Text.Trim
                .NoteDocumento = txtNoteDocumento.Text.Trim
                .NoteRitiro = NoteRitiro
                .StatoDoc = 0
                .InseritoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                Session(CSTSTATODOC) = "0"
                'giu120118
                .SplitIVA = SplitIVA
                .RitAcconto = RitAcconto
                .ImponibileRA = ImponibileRA
                .PercRA = PercRA
                .TotaleRA = TotaleRA
                .TotNettoPagare = TotNettoPagare
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
            DsDocT.DocumentiT.AddDocumentiTRow(newRowDocT)
            newRowDocT = Nothing
            Session("DsDocT") = DsDocT
        Else 'MODIFICA
            dvDocT = Session("dvDocT")
            If ddlMagazzino.SelectedIndex = 0 Then
                dvDocT.Item(0).Item("CodiceMagazzino") = 0
            Else
                dvDocT.Item(0).Item("CodiceMagazzino") = ddlMagazzino.SelectedValue.Trim
            End If
            'GIU230920
            If DDLMagazzino2.SelectedIndex = 0 Or DDLMagazzino2.Visible = False Then
                dvDocT.Item(0).Item("CodiceMagazzinoM2") = DBNull.Value
            Else
                dvDocT.Item(0).Item("CodiceMagazzinoM2") = DDLMagazzino2.SelectedValue.Trim
            End If
            If DDLCausali2.SelectedIndex = 0 Or DDLCausali2.Visible = False Then
                dvDocT.Item(0).Item("Cod_CausaleM2") = DBNull.Value
            Else
                dvDocT.Item(0).Item("Cod_CausaleM2") = DDLCausali2.SelectedValue.Trim
            End If
            '---------
            If SWNonAggNumDoc = False Then
                dvDocT.Item(0).Item("Numero") = txtNumero.Text.Trim
                dvDocT.Item(0).Item("RevisioneNDoc") = Int(txtRevNDoc.Text.Trim)
            End If

            If IsDate(txtDataDoc.Text) Then
                dvDocT.Item(0).Item("Data_Doc") = CDate(txtDataDoc.Text)
                Session(CSTDATADOC) = txtDataDoc.Text.Trim
            Else
                dvDocT.Item(0).Item("Data_Doc") = DBNull.Value
                Session(CSTDATADOC) = ""
            End If
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
            dvDocT.Item(0).Item("Riferimento") = txtRiferimento.Text.Trim + IIf(txtCCommessa.Text.Trim <> "", "§" + txtCCommessa.Text.Trim, "") 'giu250321
            'giu071211
            If IsDate(txtDataRif.Text) Then
                dvDocT.Item(0).Item("Data_Riferimento") = CDate(txtDataRif.Text)
            Else
                dvDocT.Item(0).Item("Data_Riferimento") = DBNull.Value
            End If
            dvDocT.Item(0).Item("SWTipoEvTotale") = CBool(optTipoEvTotaleTotale.Checked)
            '
            dvDocT.Item(0).Item("SWTipoEvSaldo") = CKSWTipoEvSaldo
            '--------------
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
            If Not IsNumeric(txtPagamento.Text.Trim) Or txtPagamento.Text.Trim = "0" Then
                dvDocT.Item(0).Item("Cod_Pagamento") = DBNull.Value
            Else
                dvDocT.Item(0).Item("Cod_Pagamento") = txtPagamento.Text.Trim
            End If
            If Not IsNumeric(DDLLead.SelectedValue.Trim) Or DDLLead.SelectedValue.Trim = "0" Then
                dvDocT.Item(0).Item("NumPagineTot") = DBNull.Value
            Else
                dvDocT.Item(0).Item("NumPagineTot") = DDLLead.SelectedValue.Trim
            End If
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
            If Not IsNumeric(txtCodCausale.Text.Trim) Or txtCodCausale.Text.Trim = "0" Then
                dvDocT.Item(0).Item("Cod_Causale") = DBNull.Value
            Else
                dvDocT.Item(0).Item("Cod_Causale") = txtCodCausale.Text.Trim
            End If
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
            'giu281221 GIU080722 ACCONTO SALDO SOLO PER LE FATTURE
            If Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                    Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                dvDocT.Item(0).Item("Acconto") = IIf(ChkAcconto.Checked, 1, 0)
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                If optPagSconosciuto.Checked Then
                    dvDocT.Item(0).Item("Acconto") = 0
                ElseIf optPagAnticipato.Checked Then
                    dvDocT.Item(0).Item("Acconto") = 1
                ElseIf optPagEffettuato.Checked Then
                    dvDocT.Item(0).Item("Acconto") = 2
                ElseIf optPagLista.Checked Then
                    dvDocT.Item(0).Item("Acconto") = 3
                Else
                    dvDocT.Item(0).Item("Acconto") = 0
                End If
            Else
                dvDocT.Item(0).Item("Acconto") = 0
            End If
            '---------
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
            'Blocco Data Scadenza
            If DataScad(0) <> "" Then
                dvDocT.Item(0).Item("Data_Scadenza_1") = CDate(DataScad(0))
            Else
                dvDocT.Item(0).Item("Data_Scadenza_1") = DBNull.Value
            End If
            If DataScad(1) <> "" Then
                dvDocT.Item(0).Item("Data_Scadenza_2") = CDate(DataScad(1))
            Else
                dvDocT.Item(0).Item("Data_Scadenza_2") = DBNull.Value
            End If
            If DataScad(2) <> "" Then
                dvDocT.Item(0).Item("Data_Scadenza_3") = CDate(DataScad(2))
            Else
                dvDocT.Item(0).Item("Data_Scadenza_3") = DBNull.Value
            End If
            If DataScad(3) <> "" Then
                dvDocT.Item(0).Item("Data_Scadenza_4") = CDate(DataScad(3))
            Else
                dvDocT.Item(0).Item("Data_Scadenza_4") = DBNull.Value
            End If
            If DataScad(4) <> "" Then
                dvDocT.Item(0).Item("Data_Scadenza_5") = CDate(DataScad(4))
            Else
                dvDocT.Item(0).Item("Data_Scadenza_5") = DBNull.Value
            End If
            'Blocco Scadenze RATE
            dvDocT.Item(0).Item("Rata_1") = CDec(ImpRata(0))
            dvDocT.Item(0).Item("Rata_2") = CDec(ImpRata(1))
            dvDocT.Item(0).Item("Rata_3") = CDec(ImpRata(2))
            dvDocT.Item(0).Item("Rata_4") = CDec(ImpRata(3))
            dvDocT.Item(0).Item("Rata_5") = CDec(ImpRata(4))
            'giu020321
            Dim NScad As Integer = 0 'giu040520 memorizzo le 5 rate non ancora evase per le statistiche
            Dim myScadPag As String = ""
            If Not (Session(CSTSCADPAG) Is Nothing) Then
                Dim ArrScadPag As ArrayList
                ArrScadPag = Session(CSTSCADPAG)
                Dim rowScadPag As ScadPagEntity = Nothing
                Dim Cont As Integer = 0
                For i = 0 To ArrScadPag.Count - 1
                    If myScadPag.Trim <> "" Then myScadPag += ";"
                    rowScadPag = ArrScadPag(i)
                    If rowScadPag.Data <> "" Then
                        NScad += 1
                        If NScad < 6 Then
                            dvDocT.Item(0).Item("Data_Scadenza_" & Format(NScad, "#0") & "") = CDate(rowScadPag.Data.ToString.Trim)
                            dvDocT.Item(0).Item("Rata_" & Format(NScad, "#0") & "") = CDec(rowScadPag.Importo.ToString.Trim)
                        End If
                    End If
                    myScadPag += rowScadPag.NRata.ToString.Trim & ";"
                    myScadPag += rowScadPag.Data.ToString.Trim & ";"
                    myScadPag += rowScadPag.Importo.ToString.Trim & ";"
                Next
            Else
                For i = 0 To 4
                    If DataScad(i) <> "" Then
                        If myScadPag.Trim <> "" Then myScadPag += ";"
                        myScadPag += (i + 1).ToString.Trim & ";"
                        myScadPag += DataScad(i).Trim & ";"
                        myScadPag += ImpRata(i).ToString.Trim & ";"
                    End If
                Next
            End If
            dvDocT.Item(0).Item("NoteNonEvasione") = myScadPag.Trim
            '---------------------------------------------------------------------
            If DataOraRitiro <> "" Then
                If IsDate(DataOraRitiro) Then
                    dvDocT.Item(0).Item("DataOraRitiro") = CDate(DataOraRitiro)
                Else
                    dvDocT.Item(0).Item("DataOraRitiro") = DBNull.Value
                End If
            Else
                dvDocT.Item(0).Item("DataOraRitiro") = DBNull.Value
            End If
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
            dvDocT.Item(0).Item("NoteRitiro") = NoteRitiro
            'GIU180120 se sono in allestimento non aggiorno la modificato da (Elena 1
            If Left(dvDocT.Item(0).Item("Tipo_Doc"), 1) <> "O" Then
                dvDocT.Item(0).Item("ModificatoDa") = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            ElseIf dvDocT.Item(0).Item("StatoDoc").ToString <> "5" Then
                dvDocT.Item(0).Item("ModificatoDa") = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            End If
            '---------
            Session(CSTSTATODOC) = dvDocT.Item(0).Item("StatoDoc").ToString
            'giu120118
            dvDocT.Item(0).Item("SplitIVA") = SplitIVA
            dvDocT.Item(0).Item("RitAcconto") = RitAcconto
            dvDocT.Item(0).Item("ImponibileRA") = ImponibileRA
            dvDocT.Item(0).Item("PercRA") = PercRA
            dvDocT.Item(0).Item("TotaleRA") = TotaleRA
            dvDocT.Item(0).Item("TotNettoPagare") = TotNettoPagare
            '--------
            'giu260219
            dvDocT.Item(0).Item("Bollo") = Bollo
            dvDocT.Item(0).Item("BolloACaricoDel") = IIf(Bollo <> 0, BolloACaricoDel, "")
            DsDocT = Session("DsDocT")
        End If

        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsDocT.DocumentiT)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsDocT.DocumentiT)
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
            ModalPopup.Show("Errore SQL in Documenti.AggiornaDocT. ", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore SQL in Documenti.AggiornaDocT. " & ExSQL.Message
        Catch Ex As Exception
            Session(ERROREALL) = SWSI
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.AggiornaDocT. ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore SQL in Documenti.AggiornaDocT. " & Ex.Message
        End Try
        If SWErr = True Then
            AggiornaDocT = False
        Else
            Session(SWOP) = SWOPNESSUNA
        End If
    End Function
    Public Function AggiornaDocD(ByRef DSDettBack As DSDocumenti) As Boolean
        'GIU1304012 PER LA FUNZIONE GETPrezzo/i Listino Acquisto
        Dim strErrore As String = "" : Dim myErrGetPrezziLA As String = ""
        Dim myTipoDoc As String = "" : Dim myTabCliFor As String = ""
        If CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.AggiornaDocD", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Errore in Documenti.AggiornaDocD", strErrore, WUC_ModalPopup.TYPE_ALERT)
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
                ModalPopup.Show("Errore in Documenti.AggiornaDocD", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            End If

            SqlAdapDocForInsert = Session("SqlAdapDocForInsert")
            SqlDbInserCmdForInsert = Session("SqlDbInserCmdForInsert")
            Dim DsDocDettTmp As New DSDocumenti
            Dim RowDett As DSDocumenti.DocumentiDRow
            Dim RowDettForIns As DSDocumenti.DocumentiDForInsertRow
            Dim RowDettBKIns As DSDocumenti.DocumentiDRow 'giupier 231211
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
            'giu130121 Richiesta di Zibordi
            Session("RegimeIVADiverso") = SWNO
            '------------------------------------
            'giu300312 ricalcolo ScontoReale
            For Each RowDett In DsDocDettTmp.DocumentiD
                If RowDett.RowState <> DataRowState.Deleted Then
                    RowDett.BeginEdit()
                    RowDettForIns = DsDocDettForInsert.DocumentiDForInsert.NewRow
                    RowDettBKIns = DSDettBack.DocumentiD.NewRow 'giupier 231211
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
                            ModalPopup.Show("Errore in Documenti.AggiornaDocD.GetPrezziListinoAcquisto", strErrore, WUC_ModalPopup.TYPE_ALERT)
                            ' ''AggiornaDocD = False
                            ' ''Exit Function
                        ElseIf strErrore.Trim <> "" Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore in Documenti.AggiornaDocD.GetPrezziListinoAcquisto", strErrore, WUC_ModalPopup.TYPE_ALERT)
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
                        ElseIf Val(RegimeIVA) <> 0 Then
                            If Val(RegimeIVA) <> _Cod_Iva Then
                                Session("RegimeIVADiverso") = SWSI
                            End If
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
                            If myTipoDoc = "PR" Or myTipoDoc = "PF" Then 'GIU020212
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
                    For Each dc In DsDocDettTmp.DocumentiD.Columns
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
                    DsDocDettForInsert.DocumentiDForInsert.AddDocumentiDForInsertRow(RowDettForIns)
                    DSDettBack.DocumentiD.AddDocumentiDRow(RowDettBKIns) 'giupier 231211
                End If
            Next

            Dim TransTmp As SqlClient.SqlTransaction
            Dim SqlConTmp As New SqlConnection
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Try
                SqlConTmp.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
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
                Dim sqlCmdDel As New SqlCommand
                sqlCmdDel.CommandText = "Delete From DocumentiD Where IDDocumenti=" & myID
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
                SqlAdapDocForInsert.Update(DsDocDettForInsert.DocumentiDForInsert)

                TransTmp.Commit()
                If SqlConTmp.State <> ConnectionState.Closed Then
                    SqlConTmp.Close()
                End If
                AggiornaDocD = True

            Catch ExSQL As SqlException
                TransTmp.Rollback()
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Documenti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento del database:" & ExSQL.Message, WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            Catch ex As Exception
                TransTmp.Rollback()
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Documenti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento del database:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
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
            strSQL = "UPDATE DocumentiT SET Cod_Causale = " & txtCodCausale.Text.Trim & " Where IDDocumenti=" & myID.Trim
            'giu121020 GIU290920 ALTRIMENTI NON CREA IL MOVIMENTO MM PER I TRASFERIMENTI FRA MAGAZZINI
            If DDLMagazzino2.SelectedValue.Trim <> "" And DDLMagazzino2.Visible = True And _
                     DDLCausali2.SelectedValue.Trim <> "" And DDLCausali2.Visible = True Then
                strSQL = "UPDATE DocumentiT SET Cod_Causale = " & txtCodCausale.Text.Trim & ", CodiceMagazzinoM2=" & DDLMagazzino2.SelectedValue.Trim & ", Cod_CausaleM2=" & DDLCausali2.SelectedValue.Trim & " " & _
                         " Where IDDocumenti=" & myID.Trim
            End If
            '-
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL)
                If SWOk = False Then
                    ObjDB = Nothing
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore in Documenti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento testata Cod_Causale.", WUC_ModalPopup.TYPE_ALERT)
                    AggiornaDocD = False
                    Exit Function
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Documenti.AggiornaDocD", "Si è verificato un errore durante l'aggiornamento  testata Cod_Causale. " & Ex.Message, WUC_ModalPopup.TYPE_ALERT)
                AggiornaDocD = False
                Exit Function
            End Try
            '============================================================================
            'GIU030914 DISTINTA BASE GIU230920 GESTIONE MAGAZZINI
            '============================================================================
            If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                            myTipoDoc = SWTD(TD.CaricoMagazzino) Or _
                            myTipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                            myTipoDoc = SWTD(TD.MovimentoMagazzino) Then
                If DistBaseOK(strErrore, myID.Trim) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore Distinta Base", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    AggiornaDocD = False
                    Exit Function
                End If
                'giu230920 MovFraMag
                If DDLMagazzino2.SelectedValue.Trim <> "" And DDLMagazzino2.Visible = True And _
                    DDLCausali2.SelectedValue.Trim <> "" And DDLCausali2.Visible = True Then
                    If MovFraMag(strErrore, myID.Trim) = False Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore Movimenti fra Magazzini", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        AggiornaDocD = False
                        Exit Function
                    End If
                End If
            End If
            '============================================================================
            'GIU270814 PER VELOCIZZARE IL SALVATAGGIO: RICALCOLO PER ARTICOLO LA GIACENZA
            '============================================================================
            Dim strErroreGiac As String = ""
            Dim myCodArtOR As String = ""
            Dim SWNegativi As Boolean = False
            Dim SWOKRicalcolaGiac As Boolean = False
            '============================================================================
            For Each RowDett In DsDocDettTmp.DocumentiD
                myCodArt = "" : myCodArtOR = ""
                If RowDett.RowState <> DataRowState.Deleted Then
                    If IsDBNull(RowDett.Item("Cod_Articolo")) Then
                        'nulla
                    ElseIf RowDett.Item("Cod_Articolo") = "" Then
                        'nulla
                    Else
                        myCodArt = RowDett.Item("Cod_Articolo").ToString.Trim
                    End If
                Else
                    If IsDBNull(RowDett.Item("Cod_Articolo", DataRowVersion.Original)) Then
                        'nulla
                    ElseIf RowDett.Item("Cod_Articolo", DataRowVersion.Original) = "" Then
                        'nulla
                    Else
                        myCodArtOR = RowDett.Item("Cod_Articolo", DataRowVersion.Original).ToString.Trim
                    End If
                End If
                '-----
                If myCodArt <> "" Then
                    SWOKRicalcolaGiac = False
                    If RowDett.RowState = DataRowState.Added Then
                        SWOKRicalcolaGiac = True
                    Else
                        Try
                            If myCodArt.Trim <> RowDett.Item("Cod_Articolo", DataRowVersion.Original).ToString.Trim Then
                                SWOKRicalcolaGiac = True
                            End If
                            If RowDett.Item("Qta_Ordinata") <> RowDett.Item("Qta_Ordinata", DataRowVersion.Original) Then
                                SWOKRicalcolaGiac = True
                            End If
                            '-
                            If RowDett.Item("Qta_Evasa") <> RowDett.Item("Qta_Evasa", DataRowVersion.Original) Then
                                SWOKRicalcolaGiac = True
                            End If
                            'GIU040914 Qta_Residua
                            If RowDett.Item("Qta_Allestita") <> RowDett.Item("Qta_Allestita", DataRowVersion.Original) Then
                                SWOKRicalcolaGiac = True
                            End If
                            'GIU040914 Qta_Residua
                            If RowDett.Item("Qta_Residua") <> RowDett.Item("Qta_Residua", DataRowVersion.Original) Then
                                SWOKRicalcolaGiac = True
                            End If
                            '-
                        Catch ex As Exception
                            SWOKRicalcolaGiac = True
                        End Try
                    End If
                    'GIU040914 SE è NUOVO FORZO SEMPRE IL RICALCOLO (Segnalato da Zibordi)
                    Try
                        If Session("SWOKRicalcolaGiac") = True Then
                            SWOKRicalcolaGiac = True
                        End If
                    Catch ex As Exception

                    End Try
                    '------------------------------------------------
                    If SWOKRicalcolaGiac = True Then
                        If myTipoDoc = SWTD(TD.OrdClienti) Or _
                            myTipoDoc = SWTD(TD.DocTrasportoClienti) Or myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                            myTipoDoc = SWTD(TD.CaricoMagazzino) Or _
                            myTipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                            myTipoDoc = SWTD(TD.MovimentoMagazzino) Then
                            If Ricalcola_Giacenze(myCodArt, strErroreGiac, SWNegativi, True) = False Then
                                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
                                AggiornaDocD = False
                                Exit Function
                            End If
                        End If
                    End If
                End If
                '-
                If myCodArtOR <> "" Then
                    If myTipoDoc = SWTD(TD.OrdClienti) Or _
                        myTipoDoc = SWTD(TD.DocTrasportoClienti) Or myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                        myTipoDoc = SWTD(TD.CaricoMagazzino) Or _
                        myTipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                        myTipoDoc = SWTD(TD.MovimentoMagazzino) Then
                        If Ricalcola_Giacenze(myCodArtOR, strErroreGiac, SWNegativi, True) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
                            AggiornaDocD = False
                            Exit Function
                        End If
                    End If
                End If
                'GIU030914 SE SI VUOLE AGGIORNARE LA GIACENZA PER GLI EVENTUALI COMPONENTI
                'AGIRE QUI

                '-------------------------------------------------------------------------
            Next
            DsDocDettTmp.AcceptChanges()
            '============================================================================
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.AggiornaDocD", "Si è verificato un errore durante la lettura dei dati:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            AggiornaDocD = False
        End Try
        'giu140412 
        If myErrGetPrezziLA.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.AggiornaDocD.GetPrezziListinoAcquisto", myErrGetPrezziLA, WUC_ModalPopup.TYPE_ERROR)
        End If
    End Function
    
    Private Function AggiornaNumDoc(ByRef strErrore As String) As Boolean
        strErrore = ""
        AggiornaNumDoc = True
        'SE L'ANNO DELLA DATA DOCUMENTO E' DIVERSO DALL'ANNO DI SESSIONE ESERCIZIO ESCO
        'NON AGGIORNO MAI
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
        SqlDbUpdCmd.CommandTimeout = myTimeOUT
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWOp", System.Data.SqlDbType.VarChar, 1))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        If Session(SWOP) = SWOPNUOVO Then
            SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N"
        Else
            SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMENTI)
            SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "M"
        End If
        'GIU060814 passare PA x le fatture mentre PN x le note di credito
        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = Session(CSTTIPODOC)
        If SWFatturaPA = True Then
            If Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = "PA"
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = "PN"
            End If
        End If
        '------------------------------------------------------------------
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = txtNumero.Text.Trim
        txtRevNDoc.AutoPostBack = False 'giu300419
        If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True 'giu300419
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = txtRevNDoc.Text.Trim
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                strErrore = "N° Documento risulta già impegnato(-1)!!!"
                AggiornaNumDoc = False
                TabContainer1.ActiveTabIndex = TB0
                txtNumero.BackColor = SEGNALA_KO
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                strErrore = "Errore nell'aggiornamento del N° Documento(-2)!!!"
                AggiornaNumDoc = False
                TabContainer1.ActiveTabIndex = TB0
                txtNumero.BackColor = SEGNALA_KO
            End If
        Catch ExSQL As SqlException
            strErrore = "Errore nell'aggiornamento del N° Documento(SQL)!!!: <br><br> " & ExSQL.Message
            AggiornaNumDoc = False
            TabContainer1.ActiveTabIndex = TB0
            txtNumero.BackColor = SEGNALA_KO
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore SQL in Documenti.AggiornaNumDoc", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
        Catch Ex As Exception
            strErrore = "Errore nell'aggiornamento del N° Documento(Ex)!!!: <br><br> " & Ex.Message
            AggiornaNumDoc = False
            TabContainer1.ActiveTabIndex = TB0
            txtNumero.BackColor = SEGNALA_KO
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.AggiornaNumDoc", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Function

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnNuovo_Click)")
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
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        '----------------------------
        'GIU230312 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim strErrore As String = "" : Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPreventivo + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori) Then 'giu030212
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroRiordinoFornitore + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineCliente + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then 'giu201211
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                If SWFatturaPA = False Then 'GIU070814
                    myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                Else
                    myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFA + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                If SWFatturaPA = False Then 'GIU070814
                    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = True Then
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                    Else
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    End If
                Else
                    If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = True Then
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPA + 1
                    Else
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaCdenza + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroBC + 1
            ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Then
                myNuovoNumero = GetNewMM()
            ElseIf Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Then
                myNuovoNumero = GetNewMM()
            ElseIf Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                myNuovoNumero = GetNewMM()
            Else
                Chiudi("Errore: Tipo documento non gestito: (" & Session(CSTTIPODOC).ToString & ")")
                Exit Sub
            End If
        Else
            Chiudi("Errore: Caricamento parametri generali. " & strErrore)
            Exit Sub
        End If
        '--------------------------------------------
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        'giu260312 ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
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
    'giu230312 giu260312 Recupero Numeri non impegnati
    'giu260312 verifica la sequenza se è completa
    Private Function CheckNumDoc(ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "')"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
            'GIU070814
            If SWFatturaPA = True And Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
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
            '---------
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            'GIU070814
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
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

    Public Sub CreaNewDoc()
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnNuovo_Click)")
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
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        '----------------------------

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnNuovo_Click)")
            Exit Sub
        End If

        Session(SWOP) = SWOPNUOVO
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
        WUC_DocumentiDett1.TD1ReBuildDett()
        WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2()
        Session(SWOPNUOVONUMDOC) = SWSI
        If GetNewNumDoc() = False Then Exit Sub 'giu260312

        Session("TabDoc") = TB0
        TabContainer1.ActiveTabIndex = 0
        txtRevNDoc.AutoPostBack = False 'giu300419
        txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True 'giu300419
        txtDataDoc.Text = Format(Now, FormatoData)
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = "0"
        txtTipoFatt.Text = GetParamGestAzi(Session(ESERCIZIO)).CodTipoFatt
        'giu091020
        ddlMagazzino.AutoPostBack = False
        PosizionaItemDDL("1", ddlMagazzino, True)
        ddlMagazzino.AutoPostBack = True
        '---------
        Dim strErrore As String = ""
        If AggNuovaTestata(strErrore) = False Then
            Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
            Exit Sub
        End If
        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
        'giu091020
        ddlMagazzino.AutoPostBack = False
        PosizionaItemDDL("1", ddlMagazzino, True)
        ddlMagazzino.AutoPostBack = True
        '---------
        BtnSetEnabledTo(False)
        WUC_DocumentiDett1.SetEnableTxt(True)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session(SWMODIFICATO) = SWNO
    End Sub
    Public Sub CreaNewDocRecuperaNum()
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnNuovo_Click)")
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
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Or _
                Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
                Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Or _
                Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        '----------------------------
        Session(SWOP) = SWOPNUOVO
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
        WUC_DocumentiDett1.TD1ReBuildDett()
        WUC_DocumentiSpeseTraspTot1.AzzeraTxtDocTTD2()
        'giu120412 ok GetNewNumDoc gestisce anche il recupero
        Session(SWOPNUOVONUMDOC) = SWNO
        If GetNewNumDoc() = False Then Exit Sub 'giu260312

        Session("TabDoc") = TB0
        TabContainer1.ActiveTabIndex = 0
        txtRevNDoc.AutoPostBack = False 'giu300419
        txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True 'giu300419
        txtDataDoc.Text = Format(Now, FormatoData)
        Session(CSTDATADOC) = txtDataDoc.Text.Trim
        Session(CSTSTATODOC) = "0"
        txtTipoFatt.Text = GetParamGestAzi(Session(ESERCIZIO)).CodTipoFatt
        Dim strErrore As String = ""
        If AggNuovaTestata(strErrore) = False Then
            Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
            Exit Sub
        End If
        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
        BtnSetEnabledTo(False)
        WUC_DocumentiDett1.SetEnableTxt(True)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnElimina_Click)")
            Exit Sub
        End If
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
        If TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
           TipoDoc = SWTD(TD.FatturaCommerciale) Or _
           TipoDoc = SWTD(TD.FatturaScontrino) Then
            If myStatoDoc = "1" Then
                If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
                    strMess += "<br><strong>Attenzione</strong> il DDT è stato fatturato, <br>assicuratevi che la relativa fattura venga cancellata."
                Else
                    strMess += "<br><strong>Attenzione</strong> il documento è stato trasferito in CoGe, <br>assicuratevi che venga cancellato anche in CoGe/RiBa.."
                End If
            End If
        End If
        '---------
        ModalPopup.Show("Elimina documento", strMess, WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub EliminaDocumento()
        Session(SWOP) = SWOPELIMINA
        Dim SWErr As Boolean = False
        'giu030914 mi serve per sapere ID del doc appena cancella e cancellare a catena 
        'i movimenti correlati: DISTINTA BASE, ARTICOLI INSTALLATI ETC ETC
        Dim myIDDocCanc As String = Session(IDDOCUMENTI).ToString.Trim
        Dim SQLStr As String = "" 'giu030914 SPOSTATO QUI COSI LA POSSO USARE PIU VOLTE
        '------------------------------------------------------------------------------
        'GIU040213 CANCELLO IL CARICO DI INIZIO ANNO SE PRESENTE NELLE ABILITAZIONI
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnElimina_Click)")
            Exit Sub
        End If
        If TipoDoc = SWTD(TD.CaricoMagazzino) Then
            'giu080920
            Dim Chiave As String = "CMInizio"
            Dim myCMag As String = ddlMagazzino.SelectedValue.ToString.Trim
            If IsNothing(myCMag) Then
                myCMag = "0"
            End If
            If String.IsNullOrEmpty(myCMag) Then
                myCMag = "0"
            End If
            If TipoDoc = "" Then
                myCMag = "0"
            End If
            If myCMag.Trim <> "0" Then
                Chiave = myCMag.Trim + "Inizio"
            End If
            '---------
            Try
                Dim ObjDB As New DataBaseUtility
                Dim strSQL As String = "Delete From " & CSTABILAZI & " Where " & _
                "Chiave='" & Chiave.Trim & Session(ESERCIZIO) & "' AND " & _
                "Descrizione='" & myIDDocCanc & "'"
                If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                    SWErr = True
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Cancellazione in Abilitazioni del N°Riferimento carico Inizio anno in EliminaDocumento", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                ObjDB = Nothing
            Catch ex As Exception
                SWErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Cancellazione in Abilitazioni del N°Riferimento carico Inizio anno in EliminaDocumento: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        End If
        'GIU020514 giu130614 corretto errore IDDocDTMM e non IDDocumenti
        'GIU260814 PER VELOCIZZARE IL SALVATAGGIO GIU150618 AGGIUNTO GLI ORDINI PER LE FATTURE 
        'giu100718 TD.FatturaCommerciale solo per gli ordini fatturati 
        If TipoDoc = SWTD(TD.FatturaCommerciale) Or TipoDoc = SWTD(TD.DocTrasportoClienti) Or _
           TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
           TipoDoc = SWTD(TD.CaricoMagazzino) Or _
           TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
           TipoDoc = SWTD(TD.MovimentoMagazzino) Then
            Session("GetScadProdCons") = ""
            Dim ObjDBUt As New DataBaseUtility
            Try
                SQLStr = "DELETE FROM ArticoliInst_ContrattiAss WHERE IDDocDTMM= " & myIDDocCanc
                If ObjDBUt.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                    'GIU260124
                    '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    '''ModalPopup.Show("Cancella prodotti installati", "Errore, Cancella prodotti installati.", WUC_ModalPopup.TYPE_ERROR)
                    '''Exit Sub
                Else
                    '-
                    SQLStr = "Exec Carica_AI_AggAttivo"
                    If ObjDBUt.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                        'GIU260124
                        '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        '''ModalPopup.Show("Aggiorna stato prodotti installati", "Errore, Aggiorna stato prodotti installati.", WUC_ModalPopup.TYPE_ERROR)
                        '''Exit Sub
                    End If
                End If
            Catch ex As Exception
                'GIU260124
                '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                '''ModalPopup.Show("Cancella/Aggiorna stato prodotti installati", "Errore, Cancella/Aggiorna prodotti installati.", WUC_ModalPopup.TYPE_ERROR)
                '''Exit Sub
            End Try
            ObjDBUt = Nothing
        End If
        '-
        '--------------------------------------------------------------------------
        dvDocT = Session("dvDocT")
        DsDocT = Session("DsDocT")
        dvDocT.Item(0).Delete()
        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsDocT.DocumentiT)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsDocT.DocumentiT)
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
            ModalPopup.Show("Errore SQL in Documenti.EliminaDocumento", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.EliminaDocumento", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '----------------------------------------------------------------------------------------------
        'GIU030914 DISTINTA BASE CANCELLO A CATENA I MOVIMENTI DEI COMPONENTI ED EVENTUALI PADRE
        'giu220920 GESTIONE MAGAZZINI ... CANCELLA A CATENA I MOVIMENTI DEL SECONDO MAGAZZINO 
        If TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
            TipoDoc = SWTD(TD.CaricoMagazzino) Or _
            TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
            TipoDoc = SWTD(TD.MovimentoMagazzino) Then
            Dim ObjDBUt As New DataBaseUtility
            Try
                SQLStr = "DELETE FROM DocumentiD WHERE IDDocumenti IN " & _
                "(SELECT IDDocumenti FROM DocumentiT WHERE MovPadreMMDBase = " & myIDDocCanc & ")"
                If ObjDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Cancella DistiBase/MovFraMagazzini", "Errore, Cancella movimenti (Dettagli).", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '-
                SQLStr = "DELETE FROM DocumentiT WHERE MovPadreMMDBase = " & myIDDocCanc
                If ObjDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Cancella movimenti DistiBase/MovFraMagazzini", "Errore, Cancella movimenti (Testate).", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Cancella movimenti DistiBase/MovFraMagazzini", "Errore, Cancella movimenti DistiBase/MovFraMagazzini.<br>" + ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            ObjDBUt = Nothing
        End If
        '---------------------------------------------------------------------------------------
        '============================================================================
        'GIU270814 PER VELOCIZZARE IL SALVATAGGIO: RICALCOLO PER ARTICOLO LA GIACENZA
        Dim DsDocDettTmp As New DSDocumenti
        Dim RowDett As DSDocumenti.DocumentiDRow
        DsDocDettTmp = Session("aDsDett")
        '--
        Dim strErroreGiac As String = ""
        Dim myCodArt As String = "" : Dim myCodArtOR As String = ""
        Dim SWNegativi As Boolean = False
        '============================================================================
        If (DsDocDettTmp Is Nothing) Then
            'TUTTO
            If TipoDoc = SWTD(TD.OrdClienti) Or _
                TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                TipoDoc = SWTD(TD.CaricoMagazzino) Or _
                TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                TipoDoc = SWTD(TD.MovimentoMagazzino) Then
                If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
        Else
            For Each RowDett In DsDocDettTmp.DocumentiD
                myCodArt = "" : myCodArtOR = ""
                If RowDett.RowState <> DataRowState.Deleted Then
                    If IsDBNull(RowDett.Item("Cod_Articolo")) Then
                        'nulla
                    ElseIf RowDett.Item("Cod_Articolo") = "" Then
                        'nulla
                    Else
                        myCodArt = RowDett.Item("Cod_Articolo").ToString.Trim
                    End If
                Else
                    If IsDBNull(RowDett.Item("Cod_Articolo", DataRowVersion.Original)) Then
                        'nulla
                    ElseIf RowDett.Item("Cod_Articolo", DataRowVersion.Original) = "" Then
                        'nulla
                    Else
                        myCodArtOR = RowDett.Item("Cod_Articolo", DataRowVersion.Original).ToString.Trim
                    End If
                End If
                '-----
                If myCodArt <> "" Then
                    If TipoDoc = SWTD(TD.OrdClienti) Or _
                        TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                        TipoDoc = SWTD(TD.CaricoMagazzino) Or _
                        TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                        TipoDoc = SWTD(TD.MovimentoMagazzino) Then
                        If Ricalcola_Giacenze(myCodArt, strErroreGiac, SWNegativi, True) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
                            Exit Sub
                        End If
                    End If
                End If
                '-
                If myCodArtOR <> "" Then
                    If TipoDoc = SWTD(TD.OrdClienti) Or _
                        TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                        TipoDoc = SWTD(TD.CaricoMagazzino) Or _
                        TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                        TipoDoc = SWTD(TD.MovimentoMagazzino) Then
                        If Ricalcola_Giacenze(myCodArtOR, strErroreGiac, SWNegativi, True) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
                            Exit Sub
                        End If
                    End If
                End If
                'GIU030914 DISTINTA BASE
                'SE SI VUOLE RICALCOLARE LA GIACENZE DEI FIGLI ????
            Next
        End If
        '============================================================================
        Session(SWOP) = SWOPNESSUNA
        Chiudi("")
    End Sub
    
#Region "DISTINTA BASE SETTEMBRE 2014 - GESTIONE MAGAZZINI - MOVIMENTAZIONE FRA MAGAZZINI 22/09/2020"
    'GIU030914 DISTINTA BASE
    Private Function DistBaseOK(ByRef strErrore As String, ByVal _IDDocIN As String) As Boolean
        strErrore = ""
        DistBaseOK = True
        If _IDDocIN.Trim = "" Then
            strErrore = "Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO"
            DistBaseOK = False
            Exit Function
        End If
        '-
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

        SqlDbUpdCmd.CommandText = "[InsertUpdate_MovDocTD_DistBase]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.CommandTimeout = myTimeOUT
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumentiC", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumentiP", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocIN").Value = CLng(_IDDocIN)
        SqlDbUpdCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbUpdCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then
                strErrore = "Errore nell'aggiornamento (SQL -1)!!!"
                DistBaseOK = False
            End If
        Catch ExSQL As SqlException
            strErrore = "Errore nell'aggiornamento (SQL)!!!: " & ExSQL.Message
            DistBaseOK = False
        Catch Ex As Exception
            strErrore = "Errore nell'aggiornamento (Ex)!!!: " & Ex.Message
            DistBaseOK = False
        End Try
    End Function
    'GIU230920 MOVIMENTI FRA MAGAZZINI
    Private Function MovFraMag(ByRef strErrore As String, ByVal _IDDocIN As String) As Boolean
        strErrore = ""
        MovFraMag = True
        If _IDDocIN.Trim = "" Then
            strErrore = "Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO"
            MovFraMag = False
            Exit Function
        End If
        '-
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

        SqlDbUpdCmd.CommandText = "[InsertUpdate_MovDocTD_MovMag]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.CommandTimeout = myTimeOUT
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocIN").Value = CLng(_IDDocIN)
        SqlDbUpdCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbUpdCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then
                strErrore = "Errore nell'aggiornamento (SQL -1)!!!"
                MovFraMag = False
            End If
        Catch ExSQL As SqlException
            strErrore = "Errore nell'aggiornamento (SQL)!!!: " & ExSQL.Message
            MovFraMag = False
        Catch Ex As Exception
            strErrore = "Errore nell'aggiornamento (Ex)!!!: " & Ex.Message
            MovFraMag = False
        End Try
    End Function
#End Region

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
        Session(SWOP) = SWOPMODIFICA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        CampiSetEnabledToT(True)
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        txtCodCliForFilProvv.Focus()
    End Sub

    Private Sub DDLPagamento_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLPagamento.SelectedIndexChanged
        txtPagamento.AutoPostBack = False 'giu191219
        txtPagamento.Text = DDLPagamento.SelectedValue
        txtPagamento.AutoPostBack = True 'giu191219
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        Session(SWMODIFICATO) = SWSI
        'GIU140319
        Dim strErrore As String = ""
        If WUC_DocumentiDett1.AggImportiTot(strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiornamento importi e scadenze", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
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

    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        txtCodCausale.AutoPostBack = False 'giu1219
        txtCodCausale.Text = DDLCausali.SelectedValue
        txtCodCausale.AutoPostBack = True 'giu1219
        Call CKPrezzoALCSG("", "", True) 'GIU260224
        Session(SWMODIFICATO) = SWSI
        If DDLCausali2.Visible = False Then Exit Sub
        If DDLCausali.SelectedValue = DDLCausali2.SelectedValue Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali2.BackColor = SEGNALA_KO
        Else
            DDLCausali.BackColor = SEGNALA_OK : DDLCausali2.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub DDLCausali2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali2.SelectedIndexChanged
        If DDLCausali2.Visible = False Then Exit Sub
        Session(SWMODIFICATO) = SWSI
        If DDLCausali.SelectedValue = DDLCausali2.SelectedValue Then
            DDLCausali2.BackColor = SEGNALA_KO
        Else
            DDLCausali2.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub DDLListini_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLListini.SelectedIndexChanged
        txtListino.AutoPostBack = False 'giu191219
        txtListino.Text = DDLListini.SelectedValue
        txtListino.AutoPostBack = True 'giu191219
        Session(IDLISTINO) = txtListino.Text.Trim
        lblCodDesValutaByListino()
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub DDLTipoFatt_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoFatt.SelectedIndexChanged
        txtTipoFatt.AutoPostBack = False 'giu1219
        txtTipoFatt.Text = DDLTipoFatt.SelectedValue
        txtTipoFatt.AutoPostBack = True 'giu1219
        Session(SWMODIFICATO) = SWSI
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
        WUC_DocumentiDett1.SetSWPrezzoALCSG() 'GIU210412
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
        PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
        Session(CSTIDPAG) = txtPagamento.Text.Trim
        Session(SWMODIFICATO) = SWSI
        txtCodAgente.Focus()
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
        txtCodCausale.Focus()
    End Sub

    Private Sub txtCodCausale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCausale.TextChanged
        PosizionaItemDDLByTxt(txtCodCausale, DDLCausali)
        Session(SWMODIFICATO) = SWSI
        Call CKPrezzoALCSG("", "", True) 'GIU260224
        txtListino.Focus()
    End Sub

    Private Sub txtListino_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtListino.TextChanged
        PosizionaItemDDLByTxt(txtListino, DDLListini)
        Session(IDLISTINO) = txtListino.Text.Trim
        Session(SWMODIFICATO) = SWSI
        lblCodDesValutaByListino()
        txtTipoFatt.Focus()
    End Sub

    Private Sub txtTipoFatt_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTipoFatt.TextChanged
        PosizionaItemDDLByTxt(txtTipoFatt, DDLTipoFatt)
        Session(SWMODIFICATO) = SWSI
        txtNoteDocumento.Focus()
    End Sub

    Private Sub txtNumero_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        txtNumero.TextChanged, txtRevNDoc.TextChanged
        If CheckNewNumeroOnTab() = True Then
            txtNumero.BackColor = SEGNALA_KO
            txtRevNDoc.BackColor = SEGNALA_KO
        Else
            txtNumero.BackColor = SEGNALA_OK
            txtRevNDoc.BackColor = SEGNALA_OK
        End If
        txtDataDoc.Focus()
    End Sub
    Private Function CheckNewNumeroOnTab() As Boolean
        'GIU260312 NUMERAZIONE SEPARATA E NUMEROFA 
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.CheckNewNumeroOnTab)")
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
        'giu260312
        Dim strErrore As String = ""
        If CaricaParametri(Session(ESERCIZIO), strErrore) = False Then
            CheckNewNumeroOnTab = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in CheckNewNumeroOnTab.CaricaParametri", "Verifica N.Documento da impegnare: " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        txtRevNDoc.AutoPostBack = False 'giu300419
        If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        txtRevNDoc.AutoPostBack = True 'giu300419
        If txtNumero.Text.Trim <> "" And IsNumeric(txtNumero.Text.Trim) Then
            Dim strSQL As String = "Select IDDocumenti From DocumentiT WHERE "
            If TipoDoc = SWTD(TD.DocTrasportoClienti) Or _
                TipoDoc = SWTD(TD.DocTrasportoFornitori) Or _
                TipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
                strSQL += "(Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "') "
            ElseIf TipoDoc = SWTD(TD.FatturaCommerciale) Or _
                TipoDoc = SWTD(TD.FatturaScontrino) Then
                'GIU070814
                If SWFatturaPA = True And TipoDoc = SWTD(TD.FatturaCommerciale) Then
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
            ElseIf TipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
            ElseIf TipoDoc = SWTD(TD.NotaCredito) Then
                'GIU070814
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
            Else
                strSQL += "Tipo_Doc = '" & TipoDoc & "'"
            End If
            Dim strSQLVerif As String = strSQL 'giu270819
            If Session(SWOP) = SWOPNUOVO Then
                strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
                strSQL += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
            Else
                strSQL += " AND IDDocumenti <> " & myID.Trim
                strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
                strSQL += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
            End If
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        CheckNewNumeroOnTab = True
                        'GIU270819
                        'Dim CkNumDoc As Long = CheckNumDoc(strErrore)
                        'Dim myNumDoc As Long
                        'If Not IsNumeric(txtNumero.Text.Trim) Then Exit Function
                        'myNumDoc = txtNumero.Text.Trim
                        'If Session(SWOP) = SWOPNUOVO Then
                        '    'OK
                        'ElseIf myNumDoc = CkNumDoc - 1 Then
                        '    CkNumDoc = myNumDoc
                        'End If
                        '---------
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        'ModalPopup.Show("Attenzione", "Numero documento già presente in tabella !!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                        ModalPopup.Show("Attenzione", "Numero documento già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                        Exit Function
                    Else 'giu260819
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
                            'ok proseguo
                            'altro controllo che esista quello precedente 
                            'myNumDoc = myNumDoc - 1
                            'If Session(SWOP) = SWOPNUOVO Then
                            '    strSQLVerif += " AND Numero = '" & myNumDoc.ToString.Trim & "'"
                            '    strSQLVerif += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
                            'Else
                            '    strSQLVerif += " AND IDDocumenti <> " & myID.Trim
                            '    strSQLVerif += " AND Numero = '" & myNumDoc.ToString.Trim & "'"
                            '    strSQLVerif += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
                            'End If
                            'ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                            'If (ds.Tables.Count > 0) Then
                            '    If (ds.Tables(0).Rows.Count > 0) Then
                            '        'ok esiste
                            '    Else
                            '        CheckNewNumeroOnTab = True
                            '        txtNumero.BackColor = SEGNALA_KO
                            '        txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. PRECEDENTE INESISTENTE !!!VERIFICARE !!!"
                            '        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            '        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            '        ModalPopup.Show("Attenzione", "Numero documento PRECEDENTE INESISTENTE. !!!VERIFICARE!!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                            '        Exit Function
                            '    End If
                            'Else
                            '    CheckNewNumeroOnTab = True
                            '    txtNumero.BackColor = SEGNALA_KO
                            '    txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. PRECEDENTE INESISTENTE !!!VERIFICARE !!!"
                            '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            '    ModalPopup.Show("Attenzione", "Numero documento PRECEDENTE INESISTENTE. !!!VERIFICARE!!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                            '    Exit Function
                            'End If
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
                Else
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
                        'ok proseguo
                        'altro controllo che esista quello precedente 
                        'myNumDoc = myNumDoc - 1
                        'If Session(SWOP) = SWOPNUOVO Then
                        '    strSQLVerif += " AND Numero = '" & myNumDoc.ToString.Trim & "'"
                        '    strSQLVerif += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
                        'Else
                        '    strSQLVerif += " AND IDDocumenti <> " & myID.Trim
                        '    strSQLVerif += " AND Numero = '" & myNumDoc.ToString.Trim & "'"
                        '    strSQLVerif += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
                        'End If
                        'ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                        'If (ds.Tables.Count > 0) Then
                        '    If (ds.Tables(0).Rows.Count > 0) Then
                        '        'ok esiste
                        '    Else
                        '        CheckNewNumeroOnTab = True
                        '        txtNumero.BackColor = SEGNALA_KO
                        '        txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. PRECEDENTE INESISTENTE !!!VERIFICARE !!!"
                        '        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        '        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        '        ModalPopup.Show("Attenzione", "Numero documento PRECEDENTE INESISTENTE. !!!VERIFICARE!!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                        '        Exit Function
                        '    End If
                        'Else
                        '    CheckNewNumeroOnTab = True
                        '    txtNumero.BackColor = SEGNALA_KO
                        '    txtNumero.ToolTip = "!!!Previsto: " & CkNumDoc.ToString.Trim & "!!! N.Doc. PRECEDENTE INESISTENTE !!!VERIFICARE !!!"
                        '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        '     Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        '     ModalPopup.Show("Attenzione", "Numero documento PRECEDENTE INESISTENTE. !!!VERIFICARE!!!Previsto: " & CkNumDoc.ToString.Trim & "!!!", WUC_ModalPopup.TYPE_ALERT)
                        '     Exit Function
                        'End If
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

    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampa.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then Exit Sub 'giu191111
        Session(CSTTASTOST) = btnStampa.ID
        LnkConfOrdine.Visible = False
        LnkListaCarico.Visible = False
        LnkStampa.Visible = False
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                ' ''Session(CSTObjReport) = ObjReport
                ' ''Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub
    Private Sub StampaMM()
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim selezione As String = ""
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Lotti/N° Serie collegati)", selezione, dsMovMag1, Errore, "", CLng(myID), "", True, "", "", False, False, False) Then
                ' ''Session(CSTDsPrinWebDoc) = dsMovMag1
                Session(CSTNOBACK) = 0 
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampaMovMag(dsMovMag1)
    End Sub
    Private Sub btnConfOrdine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfOrdine.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then Exit Sub 'giu191111
        Session(CSTTASTOST) = btnConfOrdine.ID
        LnkConfOrdine.Visible = False
        LnkListaCarico.Visible = False
        LnkStampa.Visible = False

        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                ' ''Session(CSTObjReport) = ObjReport
                ' ''Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 1
                Session(CSTNOBACK) = 0 
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Documenti.btnConfOrdine", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

#Region "Gestione Anagrafiche / provvisorie e Gestione BancheIBAN"

    Private Sub btnInsAnagrProvv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInsAnagrProvv.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnInsAnagrProvv_Click)")
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
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.subModificaAngrProvv)")
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
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnModificaAnagrafica_Click)")
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
        Dim Rk As StrAnagrCliFor
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
                            .Ragione_Sociale35 = IIf(IsDBNull(row.Item("Ragione_Sociale35")), "", row.Item("Ragione_Sociale35"))
                            .Riferimento35 = IIf(IsDBNull(row.Item("Riferimento35")), "", row.Item("Riferimento35"))
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
                            .Ragione_Sociale35 = "" ' lascio vuoto per il momento IIf(IsDBNull(row.Item("Rag_Soc")), "", row.Item("Rag_Soc")) 'GIU040423
                            .Riferimento35 = "" ' lascio vuoto per il momento IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
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
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnCercaBanca_Click)")
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
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (Documenti.btnCercaAnagrafica_Click)")
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
        'GIU200423 ILARIA 13/04 OBB LA DEST.MERCE PER I DDT
        Session("CKRagSoc35") = "NO"
        Session("CKRiferim35") = "NO"

        '----------------------------
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
                            .Ragione_Sociale35 = IIf(IsDBNull(row.Item("Ragione_Sociale35")), "", row.Item("Ragione_Sociale35")) 'GIU040423
                            .Riferimento35 = IIf(IsDBNull(row.Item("Riferimento35")), "", row.Item("Riferimento35"))
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
                        'giu050423
                        If myDestCF.Ragione_Sociale35.Trim = "" Then
                            lblDestSel.Text = Mid("(" + IIf(String.IsNullOrEmpty(myDestCF.Tipo), "", myDestCF.Tipo.Trim) + ") " + myDestCF.Ragione_Sociale.Trim, 1, 40)
                            txtDestinazione1.Text = myDestCF.Ragione_Sociale.Trim
                            'GIU200423 ILARIA 13/04 OBB LA DEST.MERCE PER I DDT
                            If myDestCF.Ragione_Sociale.Trim.Length > 0 And myDestCF.Ragione_Sociale.Trim.Length < 36 Then
                                'GIU200423 Session("CKRagSoc35") = "OK"
                            Else
                                Session("CKRagSoc35") = "NO"
                            End If
                        Else
                            lblDestSel.Text = Mid("(" + IIf(String.IsNullOrEmpty(myDestCF.Tipo), "", myDestCF.Tipo.Trim) + ") " + myDestCF.Ragione_Sociale35.Trim, 1, 40)
                            txtDestinazione1.Text = myDestCF.Ragione_Sociale35.Trim
                            If myDestCF.Ragione_Sociale35.Trim.Length > 0 And myDestCF.Ragione_Sociale35.Trim.Length < 36 Then
                                Session("CKRagSoc35") = "OK"
                            Else
                                Session("CKRagSoc35") = "NO"
                            End If
                        End If
                        '---------
                        txtDestinazione2.Text = myDestCF.Indirizzo.Trim
                        txtDestinazione3.Text = myDestCF.Cap.Trim & " " & myDestCF.Localita.Trim & " " & IIf(myDestCF.Provincia.Trim <> "", "(" & myDestCF.Provincia.Trim & ")", "")
                        lblDenominazioneD.Text = Mid(myDestCF.Denominazione.Trim, 1, 45)
                        lblDenominazioneD.ToolTip = myDestCF.Denominazione.Trim
                        'giu050423
                        If myDestCF.Riferimento35.Trim = "" Then
                            lblRiferimentoD.Text = Mid(myDestCF.Riferimento.Trim, 1, 45)
                            lblRiferimentoD.ToolTip = myDestCF.Riferimento.Trim
                            'GIU200423 ILARIA 13/04 OBB LA DEST.MERCE PER I DDT
                            If myDestCF.Riferimento.Trim.Length > 0 And myDestCF.Riferimento.Trim.Length < 36 Then
                                'GIU200423 Session("CKRiferim35") = "OK"
                            Else
                                Session("CKRiferim35") = "NO"
                            End If
                        Else
                            lblRiferimentoD.Text = myDestCF.Riferimento35.Trim
                            lblRiferimentoD.ToolTip = myDestCF.Riferimento35.Trim
                            If myDestCF.Riferimento35.Trim.Length > 0 And myDestCF.Riferimento35.Trim.Length < 36 Then
                                Session("CKRiferim35") = "OK"
                            Else
                                Session("CKRiferim35") = "NO"
                            End If
                        End If
                        '---------
                        lblTel1D.Text = Mid(myDestCF.Telefono1.Trim, 1, 15) : lblTel1D.ToolTip = myDestCF.Telefono1.Trim
                        lblTel2D.Text = Mid(myDestCF.Telefono2.Trim, 1, 15) : lblTel2D.ToolTip = myDestCF.Telefono2.Trim
                        lblFaxD.Text = Mid(myDestCF.Fax.Trim, 1, 15) : lblFaxD.ToolTip = myDestCF.Fax.Trim
                        lblEMailD.Text = Mid(myDestCF.EMail.Trim, 1, 45) : lblEMailD.ToolTip = myDestCF.EMail.Trim
                        'GIU090822 SE ATTIVATO LA CREAZIONE DEL FILE SPEDIZIONI CSV 
                        If myDestCF.Telefono1.Trim <> "" Then
                            Session("CKTelOBB") = "OK"
                        ElseIf myDestCF.Telefono2.Trim <> "" Then
                            Session("CKTelOBB") = "OK"
                        End If
                        '---------------------------------------------------------
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

    Private Sub btnEtichette_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEtichette.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                'GIU180320 ok SESSIONE
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrepEtichette) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTDsRitornoEtichette) = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
                Session(F_ETP_APERTA) = True
                WFPETP.Show(True)
                Exit Sub
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnListaCarico_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnListaCarico.Click
        Session(CSTTASTOST) = btnListaCarico.ID
        LnkConfOrdine.Visible = False
        LnkListaCarico.Visible = False
        LnkStampa.Visible = False

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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU270112 RECUPERO IL NUMERO SPEDIZIONE COSI LA STAMPA AVVIENE PER SPEDIZIONE 
        'RAPPORTO 1 A 1 MA VA BENE ANCHE RAPPORTO 1 A MOLTI
        Dim myNSped As Long = GetNSpedizione(myID)
        '-----------------------------------------------------------------------------
        Dim DsListaCarico1 As New DSListaCarico
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""

        Dim cod1 As String = ""
        Dim cod2 As String = ""

        Try
            'se si stampa la lista carico usare:
            If myNSped > 0 Then 'giu270112
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione
                If ClsPrint.StampaListaCarico(Session(CSTAZIENDARPT), "Lista carico", DsListaCarico1, ObjReport, StrErrore, -1, myNSped) Then
                    If DsListaCarico1.view_ListaCaricoSpedizione.Count > 0 Then
                        ' ''Session(CSTObjReport) = ObjReport
                        ' ''Session(CSTDsPrinWebDoc) = DsListaCarico1
                        Session(CSTNOBACK) = 0 
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
            Else
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico
                If ClsPrint.StampaListaCarico(Session(CSTAZIENDARPT), "Lista Carico allestimento", DsListaCarico1, ObjReport, StrErrore, CLng(myID), -1) Then
                    'se si stampa la lista carico usare:
                    ' ''If DsListaCarico1.view_ListaCaricoSpedizione.Count > 0 Then
                    If DsListaCarico1.view_ListaCarico.Count > 0 Then
                        ' ''Session(CSTObjReport) = ObjReport
                        ' ''Session(CSTDsPrinWebDoc) = DsListaCarico1
                        Session(CSTNOBACK) = 0 
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
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiElenco.btnListaCarico", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampaOrdinato(DsListaCarico1)
    End Sub
    Private Function GetNSpedizione(ByVal MyIDDoc As String) As Long
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 Spedizioni.NumeroSpedizione"
        strSQL = strSQL & " FROM Spedizioni INNER JOIN"
        strSQL = strSQL & " SpedizioniDett ON Spedizioni.ID = SpedizioniDett.IDSpedizione"
        strSQL = strSQL & " WHERE (SpedizioniDett.IDDocumenti = " & MyIDDoc.Trim & ")"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("NumeroSpedizione")) Then
                        GetNSpedizione = ds.Tables(0).Rows(0).Item("NumeroSpedizione")
                    Else
                        GetNSpedizione = -1
                    End If
                    Exit Function
                Else
                    GetNSpedizione = -1
                    Exit Function
                End If
            Else
                GetNSpedizione = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNSpedizione = -1
            Exit Function
        End Try

    End Function

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
    'GIU291220
    Private Sub btnLead_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLead.Click
        WFP_LeadSource1.WucElement = Me
        WFP_LeadSource1.SvuotaCampi()
        Session(F_LEAD_APERTA) = True
        WFP_LeadSource1.Show()
    End Sub
    Public Sub CallBackWFPLeadSource()
        Session(IDLEADSOURCE) = ""
        Dim rk As StrLeadSource
        rk = Session(RKLEADSOURCE)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDLEADSOURCE) = rk.Codice

        SqlDSLead.DataBind()
        DDLLead.Items.Clear()
        DDLLead.Items.Add("")
        DDLLead.DataBind()
        PosizionaItemDDL(Session(IDLEADSOURCE), DDLLead)
        Session(SWMODIFICATO) = SWSI
    End Sub
    Public Sub CancBackWFPLeadSource()
        'nulla
    End Sub
    'giu210612
    Private Sub btnTipofatt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTipofatt.Click
        TipoFatturazione.WucElement = Me
        TipoFatturazione.SvuotaCampi()
        Session(F_ANAGRTIPOFATT_APERTA) = True
        TipoFatturazione.Show()
    End Sub
    Public Sub CallBackWFPTipoFatt()
        Session(CODICETIPOFATT) = ""
        Dim rk As StrTipoFatt
        rk = Session(RKTIPOFATT)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(CODICETIPOFATT) = rk.Codice

        SqlDSTipoFatt.DataBind()
        DDLTipoFatt.Items.Clear()
        DDLTipoFatt.Items.Add("")
        DDLTipoFatt.DataBind()
        '-- mi riposiziono sul ....
        txtTipoFatt.Text = Session(CODICETIPOFATT).ToString.Trim
        PosizionaItemDDL(Session(CODICETIPOFATT), DDLTipoFatt)
        Session(SWMODIFICATO) = SWSI
    End Sub
    Public Sub CancBackWFPTipoFatt()
        'nulla
    End Sub

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
            'proseguo ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CIAnagrProvv = ds.Tables(0).Rows.Count
                    'proseguo Exit Function
                Else
                    CIAnagrProvv = 0
                    'proseguo Exit Function
                End If
            Else
                CIAnagrProvv = 0
                'proseguo Exit Function
            End If
        Catch Ex As Exception
            CIAnagrProvv = -1
            Exit Function
        End Try
        'giu161219 controllo anche nei CONTRATTI
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
            SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(0, txtCodCliForFilProvv.Text.Trim, mySplitIVA, strErrore)
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
        'GIU280722 SE è UNA FATTURA PER ACCONTO/SALDO NON DEVE MAI SCARICARE LE GIACENZE
        If ChkAcconto.Checked Then
            chkScGiacenza.Enabled = False
            chkScGiacenza.Checked = False
        End If
    End Sub

    Public Function SetLblTotLMPL(ByVal _TotLM As Decimal, ByVal _TotLMPL As Decimal, ByVal _TotDeduzioni As Decimal) As Boolean
        WUC_DocumentiSpeseTraspTot1.SetLblTotLMPL(_TotLM, _TotLMPL, _TotDeduzioni)
    End Function

    'giu140319 ricacolo tutti i dettagli con eventuali sc.cassa aggiunto oppure cambio tipo pagamento ricalcola le scadenze
    Public Function AggImportiTot(ByRef _Errore As String) As Boolean
        _Errore = ""
        If WUC_DocumentiDett1.AggImportiTot(_Errore) = False Then
            If _Errore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiornamento importi e scadenze", _Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
    End Function

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
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("DocumentiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica tipo documento.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        'giu120319
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Select("RitAcconto=true").Count > 0) Then
                Session(CSTSWRitAcc) = 1
                SWRitAcc = 1
            Else
                Session(CSTSWRitAcc) = 0
                SWRitAcc = 0
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Ritenuta d'acconto.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("DocumentiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica presenza Sconti.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Tipo documento.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        Try 'giu080324 giu281112 errore che il file Ã¨ gia aperto
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
                'giu021223
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
            ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or
            Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa tipodocumento non prevista.: " + Session(CSTTIPODOC), WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa tipodocumento non prevista.: " + Session(CSTTIPODOC), WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'ok
            '-----------------------------------
            Rpt.SetDataSource(DsPrinWebDoc)
            Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
            getOutputRPT(Rpt, "PDF")
            'giu240324
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            LnkConfOrdine.Visible = True
        ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True
        End If
        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
        '''    LnkConfOrdine.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
        '''    LnkListaCarico.HRef = LnkName
        '''Else
        '''    LnkStampa.HRef = LnkName
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
    Private Sub OKApriStampaOrdinato(ByRef DSListaCarico1 As DSListaCarico)
        'da inserire nel load della pagina
        Dim Rpt As Object = Nothing
        'giu080324
        Dim NomeStampa As String = "ORDINE.PDF"
        Dim SubDirDOC As String = "Ordini"
        Try 'giu080324 giu281112 errore che il file Ã¨ gia aperto
            If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico Then
                Rpt = New ListaCarico
                ' ''Dim DSListaCarico1 As New DSListaCarico
                ' ''DSListaCarico1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DSListaCarico1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione Then
                Rpt = New ListaCaricoSpedizione
                ' ''Dim DSListaCarico1 As New DSListaCarico
                ' ''DSListaCarico1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DSListaCarico1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa non prevista", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '-
            If CKCSTTipoDocST() = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Verifica tipo documento.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            NomeStampa = "ORDINE.PDF"
            SubDirDOC = "Ordini"
            Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
            getOutputRPT(Rpt, "PDF")
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            LnkConfOrdine.Visible = True
        ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True
        End If
        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
        '''    LnkConfOrdine.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
        '''    LnkListaCarico.HRef = LnkName
        '''Else
        '''    LnkStampa.HRef = LnkName
        '''End If
    End Sub
    Private Sub OKApriStampaMovMag(ByRef DsMovMag1 As DSMovMag)
        Dim Rpt As Object = Nothing
        'giu080324
        Dim NomeStampa As String = "MOVMAG.PDF"
        Dim SubDirDOC As String = "MovMag"
        Try 'giu080324 giu281112 errore che il file Ã¨ gia aperto
            If Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagDaDataAData Or
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti Then
                Rpt = New MovMag
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                'CrystalReportViewer1.DisplayGroupTree = False
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByArticolo Then
                Rpt = New MovMagPerArt
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                'CrystalReportViewer1.DisplayGroupTree = False
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortCliForNDoc Then
                Rpt = New ValCMSMCliForNDoc 'FatturatoClienteFattura
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByNDoc Then
                Rpt = New ValCMSMOrdineSortByNDoc 'FatturatoOrdineSortByNDoc
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByDataDoc Then
                Rpt = New ValCMSMOrdineSortByDataDoc 'FatturatoOrdineSortByDataDoc
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            ElseIf Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMSintOrdineSortByNDoc Then
                Rpt = New ValCMSMSintOrdineSortByNDoc 'FattSintOrdineSortByNDoc
                ' ''Dim DsMovMag1 As New DSMovMag
                ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
                'CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
                Rpt.SetDataSource(DsMovMag1)
                'CrystalReportViewer1.DisplayGroupTree = False
                'CrystalReportViewer1.ReportSource = Rpt
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa non prevista.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '-
            If CKCSTTipoDocST() = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            NomeStampa = "MOVMAG.PDF"
            SubDirDOC = "MovMag"
            Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
            getOutputRPT(Rpt, "PDF")
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            ' ''Session(CSTESPORTAPDF) = True
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Eroore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            LnkConfOrdine.Visible = True
        ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True
        End If

        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
        '''    LnkConfOrdine.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
        '''    LnkListaCarico.HRef = LnkName
        '''Else
        '''    LnkStampa.HRef = LnkName
        '''End If
    End Sub
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
    '----

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        Session(IDMAGAZZINO) = ddlMagazzino.SelectedValue
        If ddlMagazzino.SelectedValue = DDLMagazzino2.SelectedValue Then
            ddlMagazzino.BackColor = SEGNALA_KO
        End If
        If DDLMagazzino2.Visible = False Then Exit Sub
        Session(SWMODIFICATO) = SWSI
        If DDLMagazzino2.SelectedValue = ddlMagazzino.SelectedValue Then
            ddlMagazzino.BackColor = SEGNALA_KO : DDLMagazzino2.BackColor = SEGNALA_KO
        Else
            ddlMagazzino.BackColor = SEGNALA_OK : DDLMagazzino2.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub DDLMagazzino2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLMagazzino2.SelectedIndexChanged
        If DDLMagazzino2.Visible = False Then Exit Sub
        Session(SWMODIFICATO) = SWSI
        If DDLMagazzino2.SelectedValue = ddlMagazzino.SelectedValue Then
            ddlMagazzino.BackColor = SEGNALA_KO : DDLMagazzino2.BackColor = SEGNALA_KO
        Else
            ddlMagazzino.BackColor = SEGNALA_OK : DDLMagazzino2.BackColor = SEGNALA_OK
        End If
    End Sub

    'GIU291221
    Private Sub optPagEffettuato_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optPagEffettuato.CheckedChanged
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myCPagEff As Integer = 196
        If App.GetDatiAbilitazioni(CSTABILCOGE, "CPagEff", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CInt(strValore.Trim) > myCPagEff Then
                    myCPagEff = CInt(strValore.Trim)
                End If
            End If
        End If
        If optPagEffettuato.Checked Then
            If CInt(txtPagamento.Text) <> myCPagEff Then
                Session("CPagEffPrec") = txtPagamento.Text.Trim
                txtPagamento.Text = myCPagEff.ToString.Trim
                '-
                PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
                Session(CSTIDPAG) = txtPagamento.Text.Trim
                Session(SWMODIFICATO) = SWSI
            End If
        End If
    End Sub

    Private Sub optPagEffAntScon_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optPagAnticipato.CheckedChanged, _
        optPagLista.CheckedChanged, optPagSconosciuto.CheckedChanged
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myCPagEff As Integer = 196
        If App.GetDatiAbilitazioni(CSTABILCOGE, "CPagEff", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CInt(strValore.Trim) > myCPagEff Then
                    myCPagEff = CInt(strValore.Trim)
                End If
            End If
        End If
        If sender.Checked Then
            If CInt(txtPagamento.Text.Trim) = myCPagEff Then
                If Not String.IsNullOrEmpty(Session("CPagEffPrec")) Then
                    If IsNumeric(Session("CPagEffPrec")) Then
                        txtPagamento.Text = Session("CPagEffPrec").ToString.Trim
                        '-
                        PosizionaItemDDLByTxt(txtPagamento, DDLPagamento)
                        Session(CSTIDPAG) = txtPagamento.Text.Trim
                        Session(SWMODIFICATO) = SWSI
                    End If

                End If

            End If
        End If
    End Sub

    Private Sub ChkAcconto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkAcconto.CheckedChanged
        If ChkAcconto.Checked Then 'giu280722 se è fattura per acconto NON SCARICA MAI LE GIACENZE
            chkScGiacenza.Enabled = False
            chkScGiacenza.Checked = False
        End If
    End Sub
    'giu220323
    Private Sub btnCaricoLotti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCaricoLotti.Click
        Session("TabDoc") = TB1
        TabContainer1.ActiveTabIndex = TB1
        WUC_DocumentiDett1.subCaricoLotti()
    End Sub
End Class