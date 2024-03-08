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
'Imports Microsoft.Reporting.WebForms
Imports System.IO 'giu140615

Partial Public Class WUC_DocumentiElenco
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""
    Dim SWFatturaPA As Boolean = False 'giu040814
    Dim SWSplitIVA As Boolean = False 'giu040118
    Dim strEser As String = "" 'giu140319
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    'GIU200722 SPEDIZIONI PER DHL FILE CSV
    Private SWSpedVettCsv As Boolean = False
    Private CodSpedVettCsv As String = ""
    '
    Private Enum CellIdxT
        TipoDC = 1
        Stato = 2
        NumDoc = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        ' ''DataVal = 13
        Riferimento = 13
        DataRif = 14
        CAge = 15
        Dest1 = 16
        Dest2 = 17
        Dest3 = 18
        Acconto = 19
        RefDataDDT = 20
        Vettore1 = 21
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        QtaOr = 3
        QtaEv = 4
        QtaRe = 5
        QtaAl = 6
        IVA = 7
        Prz = 8
        TipoSc = 9
        ScV = 10
        Sc1 = 11
        Importo = 12
        Deduzione = 13
        ScR = 14
        PAge = 15
        ImpProvvAge = 16
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
        LnkStampa.Visible = False
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
        '-
        strEser = Session(ESERCIZIO)
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
        'giu250722 controllo Parametri di sessione per eveitare errori
        If String.IsNullOrEmpty(txtDalN.Text.Trim) Then
            txtDalN.Text = "0"
        End If
        If String.IsNullOrEmpty(txtAlN.Text.Trim) Then
            txtAlN.Text = "0"
        End If
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "0"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "0"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "0"
            txtAlN.Text = "0"
        End If
        txtDalN.BackColor = SEGNALA_OK
        txtAlN.BackColor = SEGNALA_OK
        Session("DAL") = txtDalN.Text.Trim
        Session("AL") = txtAlN.Text.Trim
        '-------------------------------------------------------------
        'giu200722 controllo se è attivo 
        Dim strSWSpedVettCsv As String = Session("SWSpedVettCsv")
        If IsNothing(strSWSpedVettCsv) Then
            strSWSpedVettCsv = ""
        End If
        If String.IsNullOrEmpty(strSWSpedVettCsv) Then
            strSWSpedVettCsv = ""
        End If
        Try
            If strSWSpedVettCsv = "" Then
                Dim strErrore As String = ""
                SWSpedVettCsv = GetDatiAbilitazioni(CSTABILAZI, "SpedVettCsv", CodSpedVettCsv, strErrore)
                If strErrore.Trim <> "" Then
                    Chiudi("Errore Lettura chiave SpedVettCsv: " & strErrore.Trim)
                    Exit Sub
                End If
                Session("SWSpedVettCsv") = SWSpedVettCsv
                If CodSpedVettCsv.Trim = "" Or Not IsNumeric(CodSpedVettCsv.Trim) Then
                    Session("SWSpedVettCsv") = False
                    SWSpedVettCsv = False
                    CodSpedVettCsv = "0"
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                ElseIf CInt(CodSpedVettCsv.Trim) = 0 Then
                    Session("SWSpedVettCsv") = False
                    SWSpedVettCsv = False
                    CodSpedVettCsv = "0"
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                Else
                    Session("SWSpedVettCsv") = SWSpedVettCsv
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                End If
            Else
                SWSpedVettCsv = Session("SWSpedVettCsv")
                CodSpedVettCsv = Session("Vettore1")
                If IsNothing(CodSpedVettCsv) Then
                    CodSpedVettCsv = "0"
                End If
                If String.IsNullOrEmpty(CodSpedVettCsv) Then
                    CodSpedVettCsv = "0"
                End If
                If CodSpedVettCsv.Trim = "" Or Not IsNumeric(CodSpedVettCsv.Trim) Then
                    Session("SWSpedVettCsv") = False
                    SWSpedVettCsv = False
                    CodSpedVettCsv = "0"
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                ElseIf CInt(CodSpedVettCsv.Trim) = 0 Then
                    Session("SWSpedVettCsv") = False
                    SWSpedVettCsv = False
                    CodSpedVettCsv = "0"
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                Else
                    Session("SWSpedVettCsv") = SWSpedVettCsv
                    Session("Vettore1") = CInt(CodSpedVettCsv)
                End If
            End If
        Catch ex As Exception
            Session("SWSpedVettCsv") = False
            SWSpedVettCsv = False
            CodSpedVettCsv = "0"
            Session("Vettore1") = CInt(CodSpedVettCsv)
        End Try
        '-------------------------------------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            rbtnFA.AutoPostBack = False : rbtnFA.Enabled = False : rbtnFA.Checked = False
            rbtnFC.AutoPostBack = False : rbtnFC.Enabled = False : rbtnFC.Checked = False
            rbtnNC.AutoPostBack = False : rbtnNC.Enabled = False : rbtnNC.Checked = False
            rbtnFCPA.AutoPostBack = False : rbtnFCPA.Enabled = False : rbtnFCPA.Checked = False 'GIU230714
            rbtnNCPA.AutoPostBack = False : rbtnNCPA.Enabled = False : rbtnNCPA.Checked = False 'GIU230714
            rbtnTipoFT.AutoPostBack = False : rbtnTipoFT.Enabled = False : rbtnTipoFT.Checked = False
            ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0
            rbtnTutti.AutoPostBack = False : rbtnTutti.Enabled = False : rbtnTutti.Checked = False
            '-
            btnElimina.Visible = False
            btnCreaFattura.Visible = False
            btnCopia.Visible = False

            Session("SWSpedVettCsv") = False
            SWSpedVettCsv = False
            CodSpedVettCsv = "0"
            Session("Vettore1") = CInt(CodSpedVettCsv)
        End If
        'GIU290722
        If IsNothing(Session(CSTGESTSPEDVETT)) Then
            Session("SWSpedVettCsv") = False
            SWSpedVettCsv = False
            CodSpedVettCsv = "0"
            Session("Vettore1") = CInt(CodSpedVettCsv)
        ElseIf String.IsNullOrEmpty(Session(CSTGESTSPEDVETT)) Then
            Session("SWSpedVettCsv") = False
            SWSpedVettCsv = False
            CodSpedVettCsv = "0"
            Session("Vettore1") = CInt(CodSpedVettCsv)
        ElseIf Session(CSTGESTSPEDVETT) <> SWSI Then
            Session("SWSpedVettCsv") = False
            SWSpedVettCsv = False
            CodSpedVettCsv = "0"
            Session("Vettore1") = CInt(CodSpedVettCsv)
        End If
        '-
        'GIU200722
        rbtnDTInLista.Visible = SWSpedVettCsv
        rbtnDTDaInviare.Visible = SWSpedVettCsv
        lblDal.Visible = SWSpedVettCsv
        txtDalN.Visible = SWSpedVettCsv
        txtAlN.Visible = SWSpedVettCsv
        lblAl.Visible = SWSpedVettCsv
        btnCercaDDT.Visible = SWSpedVettCsv
        btnGeneraSpedDDT.Visible = SWSpedVettCsv
        '----------------------------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))

        '-----------------------------
        'giu15112011 da ATTIVARE
        btnCambiaStato.Visible = False
        '-----------------------------
        If (Not IsPostBack) Then
            Try
                SqlDataSource2.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
                SqlDataSource2.DataBind()
                ddL_Esercizi.SelectedIndex = 0

                btnSblocca.Text = "Sblocca documento"
                btnSblocca.Visible = False
                btnVisualizza.Visible = True
                btnResoClienteFornitore.Visible = False
                btnResoClienteFornitore.Text = "Creazione  Nota Credito"
                '-----
                btnStampaEti.Text = "Etichette Sovracollo"
                '-
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
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(11).Value = "CG"
                ddlRicerca.Items.Add("Codice CIG/CUP")
                ddlRicerca.Items(12).Value = "CC"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(13).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(14).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(15).Value = "D3"
                '--
                Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
                If IsNothing(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If String.IsNullOrEmpty(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                '-
                'GIU080319
                Dim strStatoDoc As String = Session(CSTSTATODOCSEL)
                If IsNothing(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                If String.IsNullOrEmpty(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                '-----------
                'giu270913 giu090319
                If SWRbtnTD = "" Then
                    If rbtnFC.Checked = True Or rbtnFCPA.Checked = True Then
                        SWRbtnTD = SWTD(TD.FatturaCommerciale)
                    ElseIf rbtnNC.Checked = True Or rbtnNCPA.Checked = True Then
                        SWRbtnTD = SWTD(TD.NotaCredito)
                    ElseIf rbtnFA.Checked = True Then
                        SWRbtnTD = SWTD(TD.FatturaAccompagnatoria)
                    ElseIf rbtnDTNoFatt.Checked = True Or _
                           rbtnNONFatt.Checked = True Or _
                           rbtnCVisione.Checked = True Or _
                           rbtnCDeposito.Checked = True Or _
                           rbtnDT.Checked = True Then
                        SWRbtnTD = SWTD(TD.DocTrasportoClienti)
                    ElseIf rbtnDTFOR.Checked = True Then
                        SWRbtnTD = SWTD(TD.DocTrasportoFornitori)
                    Else
                        SWRbtnTD = SWTD(TD.DocTrasportoClienti)
                    End If
                End If
                '-
                rbtnTipoFT.AutoPostBack = False : rbtnTipoFT.Checked = False : rbtnTipoFT.AutoPostBack = True
                ddlTipoFattur.AutoPostBack = False
                ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0
                ddlTipoFattur.AutoPostBack = True
                rbtnDTDaInviareFALSE()
                Session(CSTSORTPREVTEL) = "*" 'giu201021 PRIMA VOLTA NEL CASO SI VOGLIA SELEZIONARE CON TOP COSI E' PIU VELOCE L'APERTURA
                Session(CSTFATTURAPA) = False 'GIU230714
                Session(CSTSPLITIVA) = False 'GIU150118
                Session(SWOP) = SWOPNESSUNA
                If SWRbtnTD.Trim <> "" Then
                    If SWRbtnTD = SWTD(TD.DocTrasportoFornitori) Then
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori)
                        Session(CSTSTATODOC) = "999"
                        Session(CSTSTATODOCSEL) = "999"
                        'rbtnDTFOR.AutoPostBack = False : rbtnDTFOR.Checked = True : rbtnDTFOR.AutoPostBack = True
                        rbtnDTFOR.Checked = True
                    ElseIf SWRbtnTD = SWTD(TD.DocTrasportoClienti) Then
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                        'rbtnDT.AutoPostBack = False : rbtnDTNoFatt.AutoPostBack = False : rbtnNONFatt.AutoPostBack = False : rbtnCVisione.AutoPostBack = False : rbtnCDeposito.AutoPostBack = False
                        If strStatoDoc = "999" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnDT.Checked = True
                        ElseIf strStatoDoc = "1007" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnDTNoFatt.Checked = True
                        ElseIf strStatoDoc = "1008" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                            Session(CSTSTATODOC) = "1008"
                            Session(CSTSTATODOCSEL) = "1008"
                            rbtnDTInLista.Checked = True
                        ElseIf strStatoDoc = "1009" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                            Session(CSTSTATODOC) = "1009"
                            Session(CSTSTATODOCSEL) = "1009"
                            rbtnDTDaInviare.Checked = True
                            txtDalN.Enabled = True : txtAlN.Enabled = True : btnCercaDDT.Enabled = True
                        ElseIf strStatoDoc = "1002" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnNONFatt.Checked = True
                        ElseIf strStatoDoc = "1001" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnCVisione.Checked = True
                        ElseIf strStatoDoc = "1000" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnCDeposito.Checked = True
                        Else
                            Session(CSTSTATODOC) = "1007"
                            Session(CSTSTATODOCSEL) = "1007"
                            rbtnDTNoFatt.Checked = True
                        End If
                        'rbtnDT.AutoPostBack = True : rbtnDTNoFatt.AutoPostBack = True : rbtnNONFatt.AutoPostBack = True : rbtnCVisione.AutoPostBack = True : rbtnCDeposito.AutoPostBack = True
                    ElseIf SWRbtnTD = SWTD(TD.FatturaCommerciale) Then 'giu080319 PA
                        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
                        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
                        'rbtnFC.AutoPostBack = False : rbtnFCPA.AutoPostBack = False
                        If strStatoDoc = "1004" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnFC.Checked = True
                        ElseIf strStatoDoc = "1003" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnFCPA.Checked = True
                        Else
                            rbtnFC.Checked = True
                            Session(CSTSTATODOC) = "1004"
                            Session(CSTSTATODOCSEL) = "1004"
                        End If
                        'rbtnFC.AutoPostBack = True : rbtnFCPA.AutoPostBack = True
                    ElseIf SWRbtnTD = SWTD(TD.NotaCredito) Then
                        Session(CSTTIPODOC) = SWTD(TD.NotaCredito)
                        Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
                        'rbtnNC.AutoPostBack = False : rbtnNCPA.AutoPostBack = False
                        If strStatoDoc = "1005" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnNC.Checked = True
                        ElseIf strStatoDoc = "1006" Then
                            Session(CSTSTATODOC) = strStatoDoc
                            rbtnNCPA.Checked = True
                        Else
                            Session(CSTSTATODOC) = "1005"
                            Session(CSTSTATODOCSEL) = "1005"
                            rbtnNC.Checked = True
                        End If
                        'rbtnNC.AutoPostBack = True : rbtnNCPA.AutoPostBack = True
                    ElseIf SWRbtnTD = SWTD(TD.FatturaAccompagnatoria) Then
                        Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria)
                        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria)
                        Session(CSTSTATODOC) = "999"
                        Session(CSTSTATODOCSEL) = "999"
                        'rbtnFA.AutoPostBack = False : rbtnFA.Checked = True : rbtnFA.AutoPostBack = True
                        rbtnFA.Checked = True
                    ElseIf strStatoDoc = "1007" Then 'giu130112
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTSTATODOC) = "1007"
                        Session(CSTSTATODOCSEL) = "1007"
                        'rbtnDTNoFatt.AutoPostBack = False : rbtnDTNoFatt.Checked = True : rbtnDTNoFatt.AutoPostBack = True
                        rbtnDTNoFatt.Checked = True
                    ElseIf strStatoDoc = "1008" Then 'giu130722
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTSTATODOC) = "1008"
                        Session(CSTSTATODOCSEL) = "1008"
                        'rbtnDTInLista.AutoPostBack = False : rbtnDTInLista.Checked = True : rbtnDTInLista.AutoPostBack = True
                        rbtnDTInLista.Checked = True
                    ElseIf strStatoDoc = "1009" Then 'giu130722
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTSTATODOC) = "1009"
                        Session(CSTSTATODOCSEL) = "1009"
                        'rbtnDTInLista.AutoPostBack = False : rbtnDTInLista.Checked = True : rbtnDTInLista.AutoPostBack = True
                        rbtnDTDaInviare.Checked = True
                        txtDalN.Enabled = True : txtAlN.Enabled = True : btnCercaDDT.Enabled = True
                    Else
                        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                        Session(CSTSTATODOC) = "1007"
                        Session(CSTSTATODOCSEL) = "1007"
                        'rbtnDTNoFatt.AutoPostBack = False : rbtnDTNoFatt.Checked = True : rbtnDTNoFatt.AutoPostBack = True
                        rbtnDTNoFatt.Checked = True
                    End If
                    Session(CSTSWRbtnTD) = ""
                ElseIf strStatoDoc = "1007" Then 'giu130112
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTSTATODOC) = "1007"
                    Session(CSTSTATODOCSEL) = "1007"
                    'rbtnDTNoFatt.AutoPostBack = False : rbtnDTNoFatt.Checked = True : rbtnDTNoFatt.AutoPostBack = True
                    rbtnDTNoFatt.Checked = True
                ElseIf strStatoDoc = "1008" Then 'giu130722
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTSTATODOC) = "1008"
                    Session(CSTSTATODOCSEL) = "1008"
                    'rbtnDTInLista.AutoPostBack = False : rbtnDTInLista.Checked = True : rbtnDTInLista.AutoPostBack = True
                    rbtnDTInLista.Checked = True
                ElseIf strStatoDoc = "1009" Then 'giu130722
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTSTATODOC) = "1009"
                    Session(CSTSTATODOCSEL) = "1009"
                    'rbtnDTInLista.AutoPostBack = False : rbtnDTInLista.Checked = True : rbtnDTInLista.AutoPostBack = True
                    rbtnDTDaInviare.Checked = True
                    txtDalN.Enabled = True : txtAlN.Enabled = True : btnCercaDDT.Enabled = True
                Else
                    Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                    Session(CSTSTATODOC) = "1007"
                    Session(CSTSTATODOCSEL) = "1007"
                    'rbtnDTNoFatt.AutoPostBack = False : rbtnDTNoFatt.Checked = True : rbtnDTNoFatt.AutoPostBack = True
                    rbtnDTNoFatt.Checked = True
                End If
                'giu030822
                If rbtnDTDaInviare.Checked = True Then
                    btnCercaDDT_Click(Nothing, Nothing)
                End If
                'giu150513 giu090319 end ---------------------------------------------------
                'giu201021 troppo lento esegue piu vole le letture cosi va bene 
                '' ''giu090319 lo esegue il Checked=true  BuidDett()
                ' ''Try
                ' ''    If GridViewPrevT.Rows.Count > 0 Then
                ' ''        Dim savePI = Session("PageIndex")
                ' ''        Dim saveSI = Session("SelIndex")
                ' ''        GridViewPrevT.Sort(Session("SortExp"), Session("SortDir"))
                ' ''        GridViewPrevT.PageIndex = savePI 'Session("PageIndex")
                ' ''        GridViewPrevT.SelectedIndex = saveSI 'Session("SelIndex")
                ' ''        SetFilter()
                ' ''        GridViewPrevT.DataBind()
                ' ''        GridViewPrevD.DataBind()
                ' ''    End If

                ' ''Catch ex As Exception

                ' ''End Try
                '-----------
                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    Try
                        'giu021123 
                        ' ''GridViewPrevT.SelectedIndex = 0 
                        Dim savePI = Session("PageIndex")
                        Dim saveSI = Session("SelIndex")
                        If String.IsNullOrEmpty(Session("SortExp")) Then
                            Session("SortExp") = "Numero"
                        End If
                        If String.IsNullOrEmpty(Session("SortDir")) Then
                            Session("SortDir") = 1
                        End If
                        GridViewPrevT.Sort(Session("SortExp"), Session("SortDir"))
                        GridViewPrevT.PageIndex = savePI
                        GridViewPrevT.SelectedIndex = saveSI
                        '---------
                        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                        Session(CSTDATADOC) = row.Cells(CellIdxT.DataDoc).Text.Trim
                        Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                        Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                        Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                        BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
                        GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
                        GridViewPrevD.DataBind() 'giu201021
                    Catch ex As Exception
                    End Try
                Else
                    btnGeneraSpedDDT.Enabled = False
                    btnSblocca.Visible = False
                    btnVisualizza.Visible = True
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End If
                '-----------------------------------------------
                ddL_Esercizi.AutoPostBack = True

            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Documenti: " & Ex.Message)
                Exit Sub
            End Try
        End If
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            lblEsercizio.BackColor = SEGNALA_KO
            btnCambiaStato.Visible = False
            btnCopia.Visible = False
            btnCreaFattura.Visible = False
            btnElimina.Visible = False
            btnModifica.Visible = False
            btnNuovo.Visible = False
            btnResoClienteFornitore.Visible = False
            btnSblocca.Visible = False
            'giu230419 btnStampa.Visible = False
            btnStampaEti.Visible = False
            btnVisualizza.Visible = False
            'giu230419 lblStampe.Visible = False
            ' ''Else
            ' ''    lblEsercizio.BackColor = SEGNALA_OKLBL
            ' ''    'NON ATTIVO PER IL MOMENTO btnCambiaStato.Visible = True
            ' ''    btnCopia.Visible = True
            ' ''    btnCreaFattura.Visible = True
            ' ''    btnElimina.Visible = True
            ' ''    btnModifica.Visible = True
            ' ''    btnNuovo.Visible = True
            ' ''    btnResoClienteFornitore.Visible = True
            ' ''    btnSblocca.Visible = True
            ' ''    btnStampa.Visible = True
            ' ''    btnStampaEti.Visible = True
            ' ''    btnVisualizza.Visible = True
            ' ''    lblStampe.Visible = True
        Else
            lblStampe.Visible = True
            btnStampa.Visible = True
        End If
        ModalPopup.WucElement = Me
        WFPNotaCreditoCliente.WucElement = Me
        If Session(F_NCAAC_APERTA) Then
            WFPNotaCreditoCliente.Show()
        End If
        'Simone 270317
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
        'giu220120
        WFPETP.WucElement = Me
        If Session(F_ETP_APERTA) Then
            WFPETP.Show()
        End If
       
    End Sub
    'giu080212
    Private Sub BtnSetByStato(ByVal myStato As String, ByVal myAcconto As String, ByVal myRefDataDDT As String, ByVal myVettore1 As String)
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            Exit Sub
        End If
        btnVisualizza.Visible = True
        btnSblocca.Visible = False 'giu110412
        BtnSetEnabledTo(True)
        'giu080312
        Dim SWBloccoModifica As Boolean = False
        Dim SWBloccoElimina As Boolean = False
        Dim SWBloccoCreaFT As Boolean = False
        '---------
        If myStato.Trim = "NON Fatturabile" Then
            btnCreaFattura.Enabled = False : SWBloccoCreaFT = True
        End If
        'GIU230412
        If myStato.Trim = "C/Visione" Then
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        End If
        If myStato.Trim = "C/Deposito" Then
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        End If
        '---------
        If myStato.Trim = "Fatturato" Then
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        End If
        If myStato.Trim = "OK trasf.in CoGe" Then
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
            btnStampaEti.Enabled = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
            btnStampaEti.Enabled = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
            btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
            btnStampaEti.Enabled = False
        End If
        If SWBloccoCreaFT Or SWBloccoElimina Or SWBloccoModifica Then
            btnSblocca.Visible = True
            btnVisualizza.Visible = False
            If SWBloccoCreaFT = True And SWBloccoElimina = False And SWBloccoModifica = False Then
                If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Or _
                    Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria) Then
                    btnSblocca.Visible = False
                    btnVisualizza.Visible = True
                End If
            End If
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria) Then
            btnResoClienteFornitore.Visible = True
        Else
            btnResoClienteFornitore.Visible = False
        End If
        'giu190722
        If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti) And _
            (rbtnDT.Checked = True Or rbtnDTNoFatt.Checked = True Or _
             rbtnDTInLista.Checked = True Or rbtnDTDaInviare.Checked = True Or _
             rbtnTipoFT.Checked = True Or rbtnTutti.Checked = True) And SWSpedVettCsv = True Then
            If String.IsNullOrEmpty(myAcconto) Then myAcconto = ""
            If Not IsNumeric(myAcconto.Trim) Then myAcconto = ""
            If String.IsNullOrEmpty(myRefDataDDT) Then myRefDataDDT = ""
            If Not IsDate(myRefDataDDT.Trim) Then myRefDataDDT = ""
            If String.IsNullOrEmpty(myVettore1) Then myVettore1 = ""
            If Not IsNumeric(myVettore1.Trim) Then myVettore1 = ""
            btnSiLista.Visible = False : btnNoLista.Visible = False : btnSpedNO.Visible = False : btnSpedOK.Visible = False
            If myStato.Trim = "NON Fatturabile" Or myStato.Trim = "C/Visione" Or myStato.Trim = "C/Deposito" Then
            Else
                If CodSpedVettCsv.Trim <> "" And myVettore1.Trim <> "" Then
                    If myVettore1.Trim <> CodSpedVettCsv.Trim Then
                        'scartato per vettore 
                    Else
                        If myAcconto.Trim = "" Then
                            If myRefDataDDT.Trim = "" Then btnSiLista.Visible = True
                        ElseIf myAcconto.Trim = "3" Then
                            If myRefDataDDT.Trim = "" Then btnNoLista.Visible = True
                        Else
                            If myRefDataDDT.Trim = "" Then btnSiLista.Visible = True
                        End If
                        '-
                        If myRefDataDDT.Trim = "" Then
                            If myAcconto <> "3" Then btnSpedOK.Visible = True
                        Else
                            btnSpedNO.Visible = True
                        End If
                    End If
                ElseIf CodSpedVettCsv.Trim <> "" And myVettore1.Trim = "" Then
                    If myAcconto.Trim = "" Then
                        If myRefDataDDT.Trim = "" Then btnSiLista.Visible = True
                    ElseIf myAcconto.Trim = "3" Then
                        If myRefDataDDT.Trim = "" Then btnNoLista.Visible = True
                    Else
                        If myRefDataDDT.Trim = "" Then btnSiLista.Visible = True
                    End If
                    '-
                    If myRefDataDDT.Trim = "" Then
                        If myAcconto <> "3" Then btnSpedOK.Visible = True
                    Else
                        btnSpedNO.Visible = True
                    End If
                Else
                    'nulla manca il Vettore1 predef. in Pargen per invio file XLS/CSV
                End If
            End If
        Else 'anche se gestito prima
            btnSiLista.Visible = False : btnNoLista.Visible = False : btnSpedNO.Visible = False : btnSpedOK.Visible = False
        End If
    End Sub

#Region " Funzioni e Procedure"
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaFattura.Enabled = Valore
        btnResoClienteFornitore.Enabled = Valore
        btnStampaEti.Enabled = Valore
        btnStampa.Enabled = Valore
        btnCopia.Enabled = Valore
        btnDocCollegati.Enabled = Valore 'Simone280317
        btnSpedOK.Enabled = Valore
        btnSpedNO.Enabled = Valore
        btnSiLista.Enabled = Valore
        btnNoLista.Enabled = Valore
        If rbtnDTDaInviare.Checked Then
            btnGeneraSpedDDT.Enabled = Valore
        Else
            btnGeneraSpedDDT.Enabled = False
        End If
    End Sub
#End Region

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        LnkStampa.Visible = False
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu230617
        SetFilter()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Session(CSTDATADOC) = row.Cells(CellIdxT.DataDoc).Text.Trim
                Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            btnSblocca.Visible = False
            btnVisualizza.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        LnkStampa.Visible = False
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu230617
        SetFilter()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Session(CSTDATADOC) = row.Cells(CellIdxT.DataDoc).Text.Trim
                Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            btnSblocca.Visible = False
            btnVisualizza.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevT.Sorting
        Session("SortExp") = e.SortExpression
        Session("SortDir") = e.SortDirection
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
            ' ''If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
            ' ''    e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            ' ''End If
            If IsDate(e.Row.Cells(CellIdxT.DataRif).Text) Then
                e.Row.Cells(CellIdxT.DataRif).Text = Format(CDate(e.Row.Cells(CellIdxT.DataRif).Text), FormatoData).ToString
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
            If IsNumeric(e.Row.Cells(CellIdxD.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaAl).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaAl).Text = ""
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
            If e.Row.Cells(CellIdxD.Deduzione).Text.Trim <> "True" Then
                e.Row.Cells(CellIdxD.Deduzione).Text = ""
            Else
                e.Row.Cells(CellIdxD.Deduzione).Text = "SI"
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.PAge).Text) Then
                If CDec(e.Row.Cells(CellIdxD.PAge).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.PAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.PAge).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.PAge).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ImpProvvAge).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ImpProvvAge).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ImpProvvAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ImpProvvAge).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ImpProvvAge).Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub BuidDett()
        'GIU160412
        SetFilter()
        SqlDSPrevTElenco.DataBind()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind() 'giu080212
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Session(CSTDATADOC) = row.Cells(CellIdxT.DataDoc).Text.Trim
                Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
            Catch ex As Exception
            End Try
        Else
            btnGeneraSpedDDT.Enabled = False
            btnSblocca.Visible = False
            btnVisualizza.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    'GIU160412
    Private Sub SetFilter()
        SqlDSPrevTElenco.FilterExpression = ""
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
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "CC" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CIG like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%' OR CUP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CIG = '" & Controlla_Apice(txtRicerca.Text.Trim) & "' OR CUP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
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
        End If
        'giu190523 filtro anche per tipo spedizione
        If rbtnDTDaInviare.Checked = True Then
            If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Tipo_Spedizione='V'"
        End If
        '------------------------------------------
        'GIU260617 FUNZIONA SOLO UN FILTRO, QUELLI SOPRA QUINDI I SUCCESSIVI INSERITI NELLA STORE PROCEDURE
        'TIPO FATTURAZIONE 900 + > 0 < 99
        ' ''If rbtnTipoFT.Checked Then
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "Tipo_Fatturazione = " & ddlTipoFattur.SelectedValue.ToString.Trim
        ' ''End If
        'DA FATTURARE STATODOC=0 1007
        ' ''If rbtnDTNoFatt.Checked Then 'GIU090212
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "Fatturabile <> 0 AND CVisione =0 AND CDeposito =0"
        ' ''End If
        'NON FATTURABILI 1002
        ' ''If rbtnNONFatt.Checked Then 'GIU090212
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "Fatturabile = 0 AND CVisione =0 AND CDeposito =0"
        ' ''End If
        'C/VISIONE 1001
        ' ''If rbtnCVisione.Checked Then 'GIU100412
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "Fatturabile = 0 AND CVisione <>0 AND CDeposito =0"
        ' ''End If
        'C/DEPOSITO 1000
        ' ''If rbtnCDeposito.Checked Then 'GIU160412
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "Fatturabile = 0 AND CVisione =0 AND CDeposito <>0"
        ' ''End If
        'GIU220714 
        'Fattura Commerciale 1004 NON PA
        ' ''If rbtnFC.Checked Then
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "FatturaPA =0"
        ' ''End If
        '-
        'FatturaPA 1003
        ' ''If rbtnFCPA.Checked Then
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "FatturaPA <>0"
        ' ''End If
        'Note credito NON PA 1005
        ' ''If rbtnNC.Checked Then
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "FatturaPA =0"
        ' ''End If
        'Note credito NON PA 1006
        ' ''If rbtnNCPA.Checked Then
        ' ''    If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
        ' ''        SqlDSPrevTElenco.FilterExpression += " AND "
        ' ''    End If
        ' ''    SqlDSPrevTElenco.FilterExpression += "FatturaPA <>0"
        ' ''End If
    End Sub

    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        btnSblocca.Visible = False
        LnkStampa.Visible = False
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            '
        Else
            btnVisualizza.Visible = True
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
        If txtRicerca.Text.Trim <> "" Then
            BuidDett()
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        btnSblocca.Visible = False
        LnkStampa.Visible = False
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            '
        Else
            btnVisualizza.Visible = True
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
        'GIU090418 CHIAMO SEMPRE BUILDDETT ALTRIMENTI QUANDO AZZERO TXTRICERCA NON RIESEGUE LA RICERCA SU TUTTI
        ' ''If txtRicerca.Text.Trim <> "" Then
        BuidDett()
        ' ''End If
    End Sub
#End Region

#Region "Scelta tipo documenti"
    Private Sub rbtnDTDaInviareFALSE()
        txtAlN.Enabled = False : txtDalN.Enabled = False : btnCercaDDT.Enabled = False : btnGeneraSpedDDT.Enabled = False
        btnSpedOK.Visible = False : btnSpedNO.Visible = False : btnNoLista.Visible = False : btnSiLista.Visible = False
        lnkSpedDDT.Visible = False
    End Sub
    Private Sub rbtnFC_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnFC.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTSWRbtnTD) = SWTD(TD.FatturaCommerciale) 'giu150513
        Session(CSTSTATODOC) = "1004"
        Session(CSTSTATODOCSEL) = "1004"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'giu220714
    Private Sub rbtnFA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnFA.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria)
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria)
        Session(CSTSWRbtnTD) = SWTD(TD.FatturaAccompagnatoria)
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    '-
    Private Sub rbtnFCPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnFCPA.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTSWRbtnTD) = SWTD(TD.FatturaCommerciale)
        Session(CSTSTATODOC) = "1003"
        Session(CSTSTATODOCSEL) = "1003"
        Session(CSTFATTURAPA) = True 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    '---------
    Private Sub rbtnTipoFT_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnTipoFT.CheckedChanged
        LnkStampa.Visible = False
        If rbtnTipoFT.Checked = True Then
            ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = True : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        Else
            ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        End If
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
        Session(CSTTIPODOCSEL) = "ZZ"
        Session(CSTSWRbtnTD) = "" 'giu150513
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
        If rbtnTipoFT.Checked = True Then
            ddlTipoFattur.Focus()
        End If
    End Sub
    '-
    Private Sub rbtnNC_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnNC.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = SWTD(TD.NotaCredito)
        Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
        Session(CSTSWRbtnTD) = SWTD(TD.NotaCredito) 'giu150513
        Session(CSTSTATODOC) = "1005"
        Session(CSTSTATODOCSEL) = "1005"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'giu220714
    Private Sub rbtnNCPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnNCPA.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTTIPODOC) = SWTD(TD.NotaCredito)
        Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
        Session(CSTSWRbtnTD) = SWTD(TD.NotaCredito) 'giu150513
        Session(CSTSTATODOC) = "1006"
        Session(CSTSTATODOCSEL) = "1006"
        Session(CSTFATTURAPA) = True 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    '-
    Private Sub rbtnDT_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDT.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        'btnSiLista.Visible = True : btnNoLista.Visible = True
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoClienti) 'giu150513
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'Da fatturare escluso i C/Visione e simili
    Private Sub rbtnDTNoFatt_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDTNoFatt.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        'btnSiLista.Visible = True : btnNoLista.Visible = True
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoClienti) 'giu150513
        Session(CSTSTATODOC) = "1007"
        Session(CSTSTATODOCSEL) = "1007"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'GIU130722 DDT IN LISTA DA CONSEGNARE
    Private Sub rbtnDTInLista_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDTInLista.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        btnSpedOK.Visible = True : btnSpedNO.Visible = False : btnNoLista.Visible = True : btnSiLista.Visible = False
        Session("Vettore1") = CInt(CodSpedVettCsv)
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = "" 'giu15051
        Session(CSTSTATODOC) = "1008"
        Session(CSTSTATODOCSEL) = "1008"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'GIU140722
    Private Sub rbtnDTDaInviare_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDTDaInviare.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        txtAlN.Enabled = True : txtDalN.Enabled = True : btnCercaDDT.Enabled = True : btnGeneraSpedDDT.Enabled = False
        btnSpedOK.Visible = True : btnSpedNO.Visible = False : btnNoLista.Visible = False : btnSiLista.Visible = True
        '-
        If String.IsNullOrEmpty(txtDalN.Text.Trim) Then
            txtDalN.Text = "0"
        End If
        If String.IsNullOrEmpty(txtAlN.Text.Trim) Then
            txtAlN.Text = "0"
        End If
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "0"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "0"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "0"
            txtAlN.Text = "0"
        End If
        txtDalN.BackColor = SEGNALA_OK
        txtAlN.BackColor = SEGNALA_OK
        Session("DAL") = txtDalN.Text.Trim
        Session("AL") = txtAlN.Text.Trim
        Session("Vettore1") = CInt(CodSpedVettCsv)
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) '"Z" 'NOTA 2 ZZ OK SELEZIONA ALTRIMRNTI 1 Z NON SELEZIONA NULLA
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = "" 'giu15051
        Session(CSTSTATODOC) = "1009"
        Session(CSTSTATODOCSEL) = "1009"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = False 'GIU020822
        BuidDett()
        If rbtnDTDaInviare.Checked = True Then
            txtDalN.Focus()
        End If

    End Sub
    Private Sub btnCercaDDT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaDDT.Click
        LnkStampa.Visible = False
        lnkSpedDDT.Visible = False
        '-
        If String.IsNullOrEmpty(txtDalN.Text.Trim) Then
            txtDalN.Text = "0"
        End If
        If String.IsNullOrEmpty(txtAlN.Text.Trim) Then
            txtAlN.Text = "0"
        End If
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "0"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "0"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "0"
            txtAlN.Text = "0"
        End If
        txtDalN.BackColor = SEGNALA_OK
        txtAlN.BackColor = SEGNALA_OK
        Session("DAL") = txtDalN.Text.Trim
        Session("AL") = txtAlN.Text.Trim
        Session("Vettore1") = CInt(CodSpedVettCsv)
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = "" 'giu15051
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSTATODOC) = "1009"
        Session(CSTSTATODOCSEL) = "1009"
        Session(IDDOCUMENTI) = ""
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = False 'GIU020822
        BuidDett()
    End Sub
    '-
    Private Sub rbtnDTFOR_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDTFOR.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        btnCreaFattura.Enabled = False
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoFornitori) 'giu150513
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    '-
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnTutti.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = "ZZ"
        Session(CSTTIPODOCSEL) = "ZZ"
        Session(CSTSWRbtnTD) = "" 'giu150513
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'giu090212 NON FATTURABILI
    Private Sub rbtnNONFatt_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnNONFatt.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoClienti) 'giu150513
        Session(CSTSTATODOC) = "1002"
        Session(CSTSTATODOCSEL) = "1002"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'giu100412 C/Visione
    Private Sub rbtnCVisione_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCVisione.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoClienti) 'giu150513
        Session(CSTSTATODOC) = "1001"
        Session(CSTSTATODOCSEL) = "1001"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
    'giu100412 C/Deposito
    Private Sub rbtnCDeposito_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCDeposito.CheckedChanged
        LnkStampa.Visible = False
        ddlTipoFattur.AutoPostBack = False : ddlTipoFattur.Enabled = False : ddlTipoFattur.SelectedIndex = 0 : ddlTipoFattur.AutoPostBack = True
        rbtnDTDaInviareFALSE()
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Session(CSTSWRbtnTD) = SWTD(TD.DocTrasportoClienti) 'giu150513
        Session(CSTSTATODOC) = "1000"
        Session(CSTSTATODOCSEL) = "1000"
        Session(CSTFATTURAPA) = False 'GIU230714
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
    End Sub
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
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
        'GIU221217 CONTROLLO BLOCCO DOCUMENTO SE SI DECIDESSE DI INSERIRLO
        'CI SONO GIA' LE FUNZIONI DI BLOCCO E CKBLOCCO IN DOCUMENTI:
        'BloccaDoc(....) E CKStatoDoc(....)
        '---------
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNESSUNA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
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
        'GIU221217 CONTROLLO BLOCCO DOCUMENTO SE SI DECIDESSE DI INSERIRLO
        'CI SONO GIA' LE FUNZIONI DI BLOCCO E CKBLOCCO IN DOCUMENTI:
        'BloccaDoc(....) E CKStatoDoc(....)
        '---------
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
            If rbtnFC.Checked = True Or _
                rbtnFA.Checked = True Or _
                rbtnFCPA.Checked = True Or _
                rbtnNC.Checked = True Or _
                rbtnNCPA.Checked = True Or _
                rbtnTutti.Checked = True Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    If rbtnFC.Checked = True Or _
        ' ''        rbtnFA.Checked = True Or _
        ' ''        rbtnFCPA.Checked = True Or _
        ' ''        rbtnNC.Checked = True Or _
        ' ''        rbtnNCPA.Checked = True Or _
        ' ''        rbtnTutti.Checked = True Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''        Exit Sub
        ' ''    End If
        ' ''End If
        '----------------------------
        SWFatturaPA = False 'GIU060814
        SWSplitIVA = False

        If rbtnDT.Checked = True Or rbtnCVisione.Checked = True Or rbtnCDeposito.Checked = True Or _
            rbtnDTNoFatt.Checked = True Or _
            rbtnNONFatt.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        ElseIf rbtnDTFOR.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori)
        ElseIf rbtnFC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
            'giu220714
        ElseIf rbtnFA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria)
        ElseIf rbtnFCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
            SWFatturaPA = True 'GIU060814
        ElseIf rbtnNC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
            'giu220714
        ElseIf rbtnNCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
            SWFatturaPA = True 'GIU060814
        Else
            Session(CSTTIPODOCSEL) = "" 'DEFAULT
        End If
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU230312 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim strErrore As String = "" : Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
                'giu220714
                If rbtnFCPA.Checked = True Or SWFatturaPA Then
                    myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                Else
                    myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                End If
                '---------
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFA + 1
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.FatturaScontrino) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
                'GIU220714
                If rbtnNCPA.Checked = True Or SWFatturaPA Then
                    If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = True Then
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPA + 1
                    Else
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                Else
                    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = True Then
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                    Else
                        myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    End If
                End If
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.NotaCorrispondenza) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaCdenza + 1
            ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.BuonoConsegna) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroBC + 1
            Else
                Chiudi("Errore: Tipo documento non gestito: (" & Session(CSTTIPODOCSEL).ToString & ")")
                Exit Sub
            End If
        Else
            Chiudi("Errore: Caricamento parametri generali. " & strErrore)
            Exit Sub
        End If
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
    'giu230312 giu260312 Recupero Numeri non impegnati
    'giu260312 verifica la sequenza se è completa
    'giu220714 (PA)
    'giu230819 procedura copiata anche in gestione Parametri generali se modificate qui MODIFICARE anche da altre parti
    Private Function CheckNumDoc(ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti) Or _
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Or _
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "'"
        ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Or _
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaScontrino) Then
            'GIU220714 giu110814
            If (rbtnFCPA.Checked = True Or SWFatturaPA = True) And Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
                strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0)"
                End If
            Else
                strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0)"
                End If
            End If
        ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
        ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
            'giu220714 giu110814
            If rbtnNCPA.Checked = True Or SWFatturaPA = True Then
                strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0)"
                End If
            Else
                strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
                If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                End If
            End If
        ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.NotaCorrispondenza) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
        ElseIf Session(CSTTIPODOCSEL) = SWTD(TD.BuonoConsegna) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & Session(CSTTIPODOCSEL).ToString.Trim & "'"
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
        '----------------------------
        Session(CSTFATTURAPA) = False 'giu230714
        SWFatturaPA = False 'GIU060814
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        If rbtnDT.Checked = True Or rbtnCVisione.Checked = True Or rbtnCDeposito.Checked = True Or _
            rbtnDTNoFatt.Checked = True Or _
            rbtnNONFatt.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        ElseIf rbtnDTFOR.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori)
        ElseIf rbtnFC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        ElseIf rbtnFA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria)
        ElseIf rbtnFCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
            Session(CSTFATTURAPA) = True
            SWFatturaPA = True   'GIU060814
        ElseIf rbtnNC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
        ElseIf rbtnNCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
            Session(CSTFATTURAPA) = True
            SWFatturaPA = True   'GIU060814
        Else
            Session(CSTTIPODOCSEL) = "" 'DEFAULT
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTFATTURAPA) = SWFatturaPA 'giu070814
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWSI
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Public Sub CreaNewDocRecuperaNum()
        Session(CSTFATTURAPA) = False 'giu230714
        SWFatturaPA = False 'GIU060814
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        '----------------------------
        If rbtnDT.Checked = True Or rbtnCVisione.Checked = True Or rbtnCDeposito.Checked = True Or _
            rbtnDTNoFatt.Checked = True Or _
            rbtnNONFatt.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        ElseIf rbtnDTFOR.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori)
        ElseIf rbtnFC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        ElseIf rbtnFA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria)
        ElseIf rbtnFCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
            Session(CSTFATTURAPA) = True 'giu230714
            SWFatturaPA = True
        ElseIf rbtnNC.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
        ElseIf rbtnNCPA.Checked = True Then
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito)
            Session(CSTFATTURAPA) = True 'giu230714
            SWFatturaPA = True
        Else
            Session(CSTTIPODOCSEL) = "" 'DEFAULT
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWNO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu080312 120412
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
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        '----------------------------
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
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina documento " & strDesTipoDocumento)
    End Sub
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim myID As String = ""
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        '-
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim = strEser Then
            Session(CSTTASTOST) = btnStampa.ID
            ' ''LnkConfOrdine.Visible = False
            ' ''LnkListaCarico.Visible = False
            LnkStampa.Visible = False
            'giu230419 attivato anche qui la stampa di documenti diversi dall'esercizio corrente
            If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
            myID = Session(IDDOCUMENTI)
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

            If CKCSTTipoDocSel() = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
            Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
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
                ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

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
                ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Session(CSTTASTOST) = btnStampa.ID
            ' ''LnkConfOrdine.Visible = False
            ' ''LnkListaCarico.Visible = False
            LnkStampa.Visible = False
            If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
            myID = Session(IDDOCUMENTI)
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
            If CKCSTTipoDocSel() = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
            Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
            '-
            Dim myDataDoc As String = Session(CSTDATADOC)
            If IsNothing(myDataDoc) Then
                myDataDoc = ""
            End If
            If String.IsNullOrEmpty(myDataDoc) Then
                myDataDoc = ""
            End If
            If Not IsDate(myDataDoc) Then
                myID = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessun documento selezionato (Data)", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '---------
            Try
                DsPrinWebDoc.Clear() 'GIU020512
                If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, CDate(myDataDoc).Year.ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                    ' ''Session(CSTObjReport) = ObjReport
                    ' ''Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                    If SWSconti = True Then
                        Session(CSTSWScontiDoc) = 1
                    Else
                        Session(CSTSWScontiDoc) = 0
                    End If
                    Session(CSTSWConfermaDoc) = 0
                    ' ''Session(CSTNOBACK) = 0 
                    ' ''Response.Redirect("..\WebFormTables\WF_PrintWebCR.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                    Session(ATTESA_CALLBACK_METHOD) = ""
                    Session(CSTNOBACK) = 1
                    'giu150320
                    ' ''Attesa.ShowStampaAll2("Stampa documento", "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Stampa documento")
                    ' ''Exit Sub
                Else
                    myID = ""
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore stampa documento: " & StrErrore.Trim, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            Catch ex As Exception
                myID = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore stampa documento: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End Try
        End If
        Call OKApriStampa(DsPrinWebDoc)
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
        strDesTipoDocumento = DesTD(TipoDoc)
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    Private Sub ddlTipoFattur_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTipoFattur.SelectedIndexChanged
        'GIU260617 ''SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        LnkStampa.Visible = False
        Session(CSTSORTPREVTEL) = "N"
        Session(CSTTIPODOC) = "ZZ"
        Session(CSTTIPODOCSEL) = "ZZ"
        If ddlTipoFattur.SelectedIndex = 0 Then
            BtnSetEnabledTo(False)
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            SqlDSPrevTElenco.FilterExpression = ""
            Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
            Session(CSTTIPODOCSEL) = "Z"
            Session(CSTSTATODOC) = "999"
            Session(CSTSTATODOCSEL) = "999"
            Session(IDDOCUMENTI) = ""
            GridViewPrevT.AllowPaging = True 'GIU020822
            BuidDett()
            Exit Sub
        End If
        'GIU260617
        Session(CSTSTATODOC) = CInt(ddlTipoFattur.SelectedValue.ToString.Trim) + 900
        Session(CSTSTATODOCSEL) = CInt(ddlTipoFattur.SelectedValue.ToString.Trim) + 900
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
        '---------
        ' ''SqlDSPrevTElenco.FilterExpression = "Tipo_Fatturazione = " & ddlTipoFattur.SelectedValue.ToString.Trim
        ' ''SqlDSPrevTElenco.DataBind()

        ' ''BtnSetEnabledTo(False)
        ' ''btnNuovo.Enabled = True

        ' ''Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        ' ''GridViewPrevT.DataBind()
        ' ''GridViewPrevD.DataBind() 'giu080212
        ' ''If GridViewPrevT.Rows.Count > 0 Then
        ' ''    BtnSetEnabledTo(True)
        ' ''    GridViewPrevT.SelectedIndex = 0
        ' ''    Try
        ' ''        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
        ' ''        Dim row As GridViewRow = GridViewPrevT.SelectedRow
        ' ''        Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
        ' ''        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
        ' ''        BtnSetByStato(Stato)
        ' ''    Catch ex As Exception
        ' ''        Session(IDDOCUMENTI) = ""
        ' ''        Session(CSTTIPODOCSEL) = ""
        ' ''    End Try
        ' ''Else
        ' ''    btnSblocca.Visible = False
        ' ''    BtnSetEnabledTo(False)
        ' ''    btnNuovo.Enabled = True
        ' ''    Session(IDDOCUMENTI) = ""
        ' ''    Session(CSTTIPODOCSEL) = ""
        ' ''End If

    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        LnkStampa.Visible = False
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
            Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
            Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
            BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End Try
    End Sub

    Private Sub btnCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopia.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim StrErrore As String = ""
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaCopia"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Documento", "Confermi la copia del documento selezionato ? <br>" & strDesTipoDocumento & "<br><strong><span> " _
                        & "Attenzione, Il tipo Pagamento sarà aggiornato a quello in essere.<br>Si prega di verificare i dati Pagamento.</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaCopia()

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
        'giu040814 non mi serve saperlo tanto sono nei filtri dei tipi doc
        ' ''SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", StrErrore)
        ' ''Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        ' ''If StrErrore.Trim <> "" Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        '---------
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        Session(CSTFATTURAPA) = False : SWFatturaPA = False 'giu070814
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            If TipoDoc = SWTD(TD.FatturaCommerciale) Then
                If rbtnFCPA.Checked = True Then 'giu230714
                    Session(CSTFATTURAPA) = True : SWFatturaPA = True    'giu070814
                    NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                Else
                    NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                End If
            ElseIf TipoDoc = SWTD(TD.NotaCredito) Then
                If rbtnNCPA.Checked = True Then 'giu230714
                    Session(CSTFATTURAPA) = True : SWFatturaPA = True    'giu070814
                    If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep Then
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPA + 1
                    Else
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                Else
                    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata Then
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                    Else
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    End If
                End If
            ElseIf TipoDoc = SWTD(TD.DocTrasportoClienti) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            ElseIf TipoDoc = SWTD(TD.DocTrasportoFornitori) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO: " & TipoDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        'GIU070814
        If TipoDoc = SWTD(TD.FatturaCommerciale) Then
            If rbtnFCPA.Checked = True Then
                If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                    Exit Sub
                End If
            Else
                If AggiornaNumDoc(TipoDoc, NDoc, NRev) = False Then
                    Exit Sub
                End If
            End If
        ElseIf TipoDoc = SWTD(TD.NotaCredito) Then
            If rbtnNCPA.Checked = True Then
                If AggiornaNumDoc("PN", NDoc, NRev) = False Then
                    Exit Sub
                End If
            Else
                If AggiornaNumDoc(TipoDoc, NDoc, NRev) = False Then
                    Exit Sub
                End If
            End If
        Else
            If AggiornaNumDoc(TipoDoc, NDoc, NRev) = False Then
                Exit Sub
            End If
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
        Dim myIDNew As String = "" 'giu210423
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myIDNew = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "<b>Verificare i dati della Copia appena creata(" + NDoc.ToString.Trim + ").</b> Errore: " + ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "<b>Verificare i dati della Copia appena creata(" + NDoc.ToString.Trim + ").</b> Errore: " + Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
            myID = myIDNew
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
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    Private Sub btnCreaFattura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaFattura.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        'giu080312 giu120412
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
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        '----------------------------
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
        '-
        Dim StrErrore As String = ""
        'giu040814 salto il test visto che il DDT ha il segnale se DEVe essere PA oppure no
        Dim myTotNettoPagare As Decimal = 0 : Dim myTotaleDoc As Decimal = 0 'giu290519
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore, "", myTotaleDoc, myTotNettoPagare)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'GIU230312 'giu120412 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim myNuovoNumero As Long = 0
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Chiudi("Errore: Caricamento parametri generali. " & StrErrore)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Chiudi("Errore: Caricamento parametri generali. " & StrErrore)
                Exit Sub
            End If
        End If
        '--------------------------------------------
        Dim CkNumDocFC As Long = CheckNumDocFC(StrErrore)
        If CkNumDocFC = -1 Then
            Chiudi("Errore: Verifica N° Fattura da impegnare. " & StrErrore)
            Exit Sub
        End If
        '-
        Dim strMessSpese As String = ""
        If CKSpeseAdd(myID, strMessSpese) = False Then 'giu010420
            Exit Sub
        End If
        'giu230822
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNoCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNoCVett, StrErrore)
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
            '''If swNoDestM Then
            '''    strBloccoCliente += "SENZA Destinazione Merce<br>"
            '''    swOK = True
            '''ElseIf swNODatiCorr Then
            '''    strBloccoCliente += "SENZA Dati corriere<br>"
            '''    swOK = True
            '''End If
            strBloccoCliente += "</span></strong>"
            If swOK = False Then
                strBloccoCliente = ""
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore verifica Cliente", strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU200323
        If SWNoPIVACF Or SWNoCodIPA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE CREARE FATTURA<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------
        strDesTipoDocumento = "FATTURA COMMERCIALE"
        If myTotaleDoc = 0 Or myTotNettoPagare = 0 Then
            If myNuovoNumero <> CkNumDocFC Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaFatturaRecuperaNum"
                ModalPopup.Show("Crea nuova Fattura da DDT", "Confermi la creazione del documento ? <br><strong><span> " _
                            & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & FormattaNumero(myNuovoNumero) & " <br>" _
                            & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                            & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                            & FormattaNumero(CkNumDocFC) & " <br> ATTENZIONE, Totale documento uguale a ZERO !!! </span></strong><br>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Confermi la creazione del documento ? <br><strong><span> " _
                            & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & FormattaNumero(myNuovoNumero) & "<br> ATTENZIONE, Totale documento uguale a ZERO !!! </span></strong><br>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
            End If
        Else
            If myNuovoNumero <> CkNumDocFC Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaFatturaRecuperaNum"
                ModalPopup.Show("Crea nuova Fattura da DDT", "Confermi la creazione del documento ? <br><strong><span> " _
                            & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & FormattaNumero(myNuovoNumero) & " <br>" _
                            & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                            & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                            & FormattaNumero(CkNumDocFC) & " </span></strong><br>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Confermi la creazione del documento ? <br><strong><span> " _
                            & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & FormattaNumero(myNuovoNumero) & " </span></strong><br>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
            End If
        End If
    End Sub
    'giu010420
    Private Function CKSpeseAdd(ByVal myID As String, ByRef strMess As String) As Boolean
        CKSpeseAdd = False
        If Not IsNumeric(myID.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO Documento ERRATO.".Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If

        Dim strSQL As String = "" : Dim TotSpese = "" : Dim TotSpeseAdd = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsDocT As New DataSet
        Dim rowDocT() As DataRow
        strSQL = "Select (ISNULL(Spese_Incasso,0)+ISNULL(Spese_Trasporto,0)+ISNULL(Spese_Imballo,0)+ISNULL(SpeseVarie,0)) " & _
                        "AS TotSpese,TotSpeseAddebitate " & _
                        "From DocumentiT Where IDDocumenti=" & myID.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsDocT)
            If (dsDocT.Tables.Count > 0) Then
                If (dsDocT.Tables(0).Rows.Count > 0) Then
                    rowDocT = dsDocT.Tables(0).Select()
                    TotSpese = IIf(IsDBNull(rowDocT(0).Item("TotSpese")), "0", rowDocT(0).Item("TotSpese"))
                    If String.IsNullOrEmpty(TotSpese) Then TotSpese = "0"
                    '-
                    TotSpeseAdd = IIf(IsDBNull(rowDocT(0).Item("TotSpeseAddebitate")), "0", rowDocT(0).Item("TotSpeseAddebitate"))
                    If String.IsNullOrEmpty(TotSpeseAdd) Then TotSpeseAdd = "0"
                    'ok
                    If CDec(TotSpese) <> 0 And CDec(TotSpeseAdd) = 0 Then
                        strMess = "<br><br><strong><span>Attenzione, sono presenti delle spese non ancora addebitate,</span></strong><br>" & _
                                         "SARANNO ADDEBITATE LE SPESE in caso contrario TOGLIERLE manualmente<br>" & _
                                         "Alla successiva emissione non saranno più inserite."
                    ElseIf CDec(TotSpese) <> 0 And CDec(TotSpeseAdd) <> 0 Then
                        strMess = "<br><br><strong><span>Attenzione, sono presenti delle spese già addebitate,</span></strong><br>" & _
                                        "NON SARANNO ADDEBITATE LE SPESE in caso contrario INSERIRLE manualmente."
                    End If
                    CKSpeseAdd = True
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Documento in archivio.".Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Documento in archivio.".Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura Documento (CKSpeseAdd): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    'giu120412 GESTIONE RECUPERO NUMERI FATTURE NON USATI
    'giu040814 ok FATTUREPA
    Private Function CheckNumDocFC(ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero "
        strSQL += "From DocumentiT WHERE "
        If SWFatturaPA = False Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
            End If
            strSQL += ") AND ISNULL(FatturaPA,0)=0"
        Else
            strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' "
            If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
            End If
            strSQL += ") AND ISNULL(FatturaPA,0)<>0"
        End If

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDocFC = ds.Tables(0).Rows(0).Item("Numero") + 1
                        'giu260819 non va bene, es. i preventivi ci sono le REVISION quindi sicuramente il numero è superiore
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    'CheckNumDocFC = (ds.Tables(0).Rows(0).Item("TotDoc") + 1)
                        ' ''    'GIU171012
                        ' ''    CheckNumDocFC = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < CheckNumDocFC, CheckNumDocFC, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
                        ' ''End If
                    Else
                        CheckNumDocFC = 1
                    End If
                    Exit Function
                Else
                    CheckNumDocFC = 1
                    Exit Function
                End If
            Else
                CheckNumDocFC = 1
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            CheckNumDocFC = -1
            Exit Function
        End Try

    End Function
    '------------------------------
    Public Sub CreaFattura()
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
        '-
        'giu040814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        'giu080614
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '--------------------------------------------
        '-
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        'giu060814
        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
        'OK CREAZIONE NUOVA FATTURA
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFC]"
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Try
            myID = ""
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
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
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE")
    End Sub
    'giu120412 GESTIONE RECUPERO NUMERI FATTURA NON USATI
    Public Sub CreaFatturaRecuperaNum()

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
        'giu040814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        NDoc = CheckNumDocFC(StrErrore)
        If NDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "N° Fattura da impegnare (Recupero).", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        'giu060814
        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
        'OK CREAZIONE NUOVA FATTURA
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFC]"
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Try
            myID = ""
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
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
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE")
    End Sub

    Public Sub AvviaRicerca()
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub

    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer) As Boolean
        'GIU240714 FATTURE/NC PA=FCPA PN=NCPA
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

    Private Sub btnStampaEti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaEti.Click
        
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
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
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
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
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrepEtichette) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 'giu040512
                Session(CSTDsRitornoEtichette) = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
                ' ''Response.Redirect("..\WebFormTables\WF_EtichettePrepara.aspx?labelForm=Prepara etichette")
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

    Private Sub btnSblocca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSblocca.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnSblocca.Visible = False
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
        btnVisualizza.Visible = True
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

#Region "Creazione NOTA DI CREDITO"
    'giu250612 GESTIONE NOTE DI CREDITO (FALSA RIGA DI MOVIMENTI ELENCO GESTIONE RESI unica store procedure)
    Private Sub btnResoClienteFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnResoClienteFornitore.Click
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            btnVisualizza.Visible = False
            Exit Sub
        End If
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
        If Mid(CodiceCoGe, 1, 1) = "1" And _
            (myTD = SWTD(TD.FatturaCommerciale) Or myTD = SWTD(TD.FatturaAccompagnatoria)) Then 'NOTA DI CREDITO
            Dim myErrore As String = ""
            If Documenti.BloccaDoc(CLng(Session(IDDOCUMENTI)), myErrore, myModificaDA, "", sUtente, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            WFPNotaCreditoCliente.WucElement = Me
            WFPNotaCreditoCliente.PopolaGrigliaWUCDocTD() 'giu120417
            WFPNotaCreditoCliente.SetLblMessUtente("Seleziona/modifica Quantità articoli resi")
            Session(F_NCAAC_APERTA) = True
            WFPNotaCreditoCliente.Show(True)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Nota di Credito", "Non è previsto la creazione della NC per questo tipo di movimento.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    Public Sub CancBackWFPNotaCreditoCliente()
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
    Public Sub CallBackWFPNotaCreditoCliente()
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
            'giu110814
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaNotaCreditoCliente"
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Crea nuova Nota di Credito", "Confermi la creazione della NC a Cliente ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
            Call OKCreaNotaCreditoCliente()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Nota di Credito", "Nessun articolo è stato selezionato.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub
    'GIU150513 BEGIN PER CONTROLLO SEQUENZA NUMERI IMPEGNATI XKè MANCAVA
    Public Sub OKCreaNotaCreditoCliente()
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
        'giu060814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        '-
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

            Session(CSTTIPODOC) = SWTD(TD.NotaCredito)
            Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) 'serve a GetNewNumDoc per prendere il primo numero libero
            StrErrore = "" : Dim myCodCausale As Integer = 0
            Dim NDoc As Long = GetNewNumDoc(myCodCausale, StrErrore)
            If NDoc < 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'impegno numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'giu150513 mancava il controlo sequenza numeri impegnati
            '--------------------------------------------
            Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
            If CkNumDoc = -1 Then
                Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
                Exit Sub
            End If
            'GIU150513
            Session("CreaNewNCNDoc") = NDoc
            Session("CreaNewNCCkNumDoc") = CkNumDoc
            Session("CreaNewNCCODCAUS") = myCodCausale
            '-
            If NDoc <> CkNumDoc Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewNC"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewNCRecuperaNum"
                ModalPopup.Show("Crea nuova Nota di Credito", "Nuovo numero: <strong><span>" _
                            & FormattaNumero(NDoc) & " <br>" _
                            & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                            & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                            & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
            Else
                'giu110814
                ' ''Call OKCreaNewNC()
                Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaNewNC"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Nota di Credito", "Confermi la creazione della NC a Cliente ? <br>" _
                                & "<strong><span>Nuovo numero: " _
                                & FormattaNumero(NDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
            End If
        End If
        '-------------------------------------------------------
    End Sub
    'GIU150513
    Public Sub CreaNewNC()
        Session("CreaNewNCNDoc") = Session("CreaNewNCNDoc")
        Call OKCreaNewNC()
    End Sub
    Public Sub CreaNewNCRecuperaNum()
        Session("CreaNewNCNDoc") = Session("CreaNewNCCkNumDoc")
        Call OKCreaNewNC()
    End Sub
    Public Sub OKCreaNewNC()
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
        'giu060814 GIU050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        'GIU150513
        Dim myCodCausale As String = Session("CreaNewNCCODCAUS")
        If IsNothing(myCodCausale) Then
            myCodCausale = ""
        End If
        If String.IsNullOrEmpty(myCodCausale) Then
            myCodCausale = ""
        End If
        If Not IsNumeric(myCodCausale) Then
            myCodCausale = 0
        End If
        '-
        Dim NDoc As String = Session("CreaNewNCNDoc")
        If IsNothing(NDoc) Then
            NDoc = ""
        End If
        If String.IsNullOrEmpty(NDoc) Then
            NDoc = ""
        End If
        If Not IsNumeric(NDoc) Then
            NDoc = 0
            Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
            Exit Sub
        End If
        '---------
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
        '---------------------------------
        Dim NRev As Integer = 0
        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.NotaCredito), CLng(NDoc), NRev) = False Then
                'SEGNALATO NELLA FUNZIONE
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "Errore durante l'aggiornamento numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PN", CLng(NDoc), NRev) = False Then
                'SEGNALATO NELLA FUNZIONE
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "Errore durante l'aggiornamento numero documento Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '-----
        If myCodCausale = 0 Then myCodCausale = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocNC]"
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
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1, "FatturaPA"))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1, "SplitIVA"))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.NotaCredito)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = myCodCausale
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        'giu060814
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'GIU050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Dim myIDNew As String = "" 'giu200423
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myIDNew = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "<b>Verificare i dati della NC appena creata(" + NDoc.ToString.Trim + ").</b> Errore: " + ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "<b>Verificare i dati della NC appena creata(" + NDoc.ToString.Trim + ").</b> Errore: " + Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        myID = myIDNew
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
        Session(CSTTIPODOC) = SWTD(TD.NotaCredito)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=NOTA CREDITO")
    End Sub
    'GIU150513 END
    Private Function GetNewNumDoc(ByRef _CodCausale As Integer, ByRef strErrore As String) As Long
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
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                If rbtnFCPA.Checked = True Or SWFatturaPA = True Then 'giu230714 giu060814
                    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                Else
                    GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                End If
                ' ''GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFA + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                If rbtnNCPA.Checked = True Or SWFatturaPA = True Then 'giu230714 giu060814
                    If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = True Then
                        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPA + 1
                    Else
                        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                Else
                    If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = True Then
                        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaAccredito + 1
                    Else
                        GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    End If
                End If
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CausNCResi
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                GetNewNumDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroNotaCdenza + 1
                _CodCausale = GetParamGestAzi(Session(ESERCIZIO)).CodCausaleVendita
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
    'COPIATA MA NON USATA RICHIAMATA DA GetNewNumDoc
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
#End Region
    'Simone280317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        'giu24082021 altrimenti il dettaglio esempio della fattura rimane anche per il DDT 
        '' ''GIU191219
        ' ''Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        ' ''Session(CSTSORTPREVTEL) = "N"
        ' ''Session(SWOP) = SWOPNESSUNA
        ' ''rbtnDTNoFatt.Checked = True
    End Sub

    Public Sub CallBackWFPDocCollegati()
        'giu24082021 altrimenti il dettaglio esempio della fattura rimane anche per il DDT 
        ' ''Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        ' ''Session(CSTSORTPREVTEL) = "N"
        ' ''Session(SWOP) = SWOPNESSUNA
        ' ''rbtnDTNoFatt.Checked = True
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

    Private Sub ddL_Esercizi_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddL_Esercizi.SelectedIndexChanged
        'giu140319
        LnkStampa.Visible = False
        If ddL_Esercizi.SelectedValue.Trim <> "" And ddL_Esercizi.SelectedValue.Trim <> strEser Then
            lblEsercizio.BackColor = SEGNALA_KO
            btnCambiaStato.Visible = False
            btnCopia.Visible = False
            btnCreaFattura.Visible = False
            btnElimina.Visible = False
            btnModifica.Visible = False
            btnNuovo.Visible = False
            btnResoClienteFornitore.Visible = False
            btnSblocca.Visible = False
            'giu230419 btnStampa.Visible = False
            btnStampaEti.Visible = False
            btnVisualizza.Visible = False
            'giu230419 lblStampe.Visible = False
            lblStampe.Visible = True
            btnStampa.Visible = True
        Else
            lblEsercizio.BackColor = SEGNALA_OKLBL
            'NON ATTIVO PER IL MOMENTO btnCambiaStato.Visible = True
            btnCopia.Visible = True
            btnCreaFattura.Visible = True
            btnElimina.Visible = True
            btnModifica.Visible = True
            btnNuovo.Visible = True
            btnResoClienteFornitore.Visible = True
            btnSblocca.Visible = True
            btnStampa.Visible = True
            btnStampaEti.Visible = True
            btnVisualizza.Visible = True
            lblStampe.Visible = True
        End If
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, IIf(ddL_Esercizi.SelectedValue.Trim <> "", ddL_Esercizi.SelectedValue.Trim, ""))
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
        BuidDett()
    End Sub

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
            'GIU210120 ESEGUITO IN CKSTTipoDocST
            'Per evitare che solo un utente possa elaborare le stampe
            ' ''Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            ' ''If (Utente Is Nothing) Then
            ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            ' ''    Exit Sub
            ' ''End If
            Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
            'giu150320 GIU180320 TROPPO LENTO
            ' ''LnkStampa.HRef = "~/WebFormTables/Stampa.aspx"
            ' ''LnkStampa.Visible = True
            '' '' ''LnkConfOrdine.HRef = "~/WebFormTables/Stampa.aspx"
            '' '' ''LnkListaCarico.HRef = "~/WebFormTables/Stampa.aspx"
            ' ''Dim myStream As Stream
            ' ''Dim ms As New MemoryStream
            ' ''Dim myOBJ() As Byte = Nothing
            ' ''Try
            ' ''    myStream = Rpt.ExportToStream(ExportFormatType.PortableDocFormat)

            ' ''    Dim Ret As Integer
            ' ''    Do
            ' ''        Ret = myStream.ReadByte() 'netstream.Read(Bytes, 0, Bytes.Length)
            ' ''        If Ret > 0 Then
            ' ''            ReDim Preserve myOBJ(myStream.Position - 1)
            ' ''            myOBJ(myStream.Position - 1) = Ret
            ' ''        End If
            ' ''    Loop Until Ret = -1

            ' ''Catch ex As Exception
            ' ''    Chiudi("Errore in elaborazione stampa: " & ex.Message)
            ' ''End Try
            ' ''Session("objReport") = myOBJ
            ' ''If Session(CSTTASTOST) = btnStampa.ID Then
            ' ''    LnkStampa.HRef = LnkName
            ' ''    ' ''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            ' ''    ' ''    LnkConfOrdine.HRef = LnkName
            ' ''    ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    ' ''    LnkListaCarico.HRef = LnkName
            ' ''Else
            ' ''    LnkStampa.HRef = LnkName
            ' ''End If
            ' ''Exit Sub
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '' ''---------
            'giu140615 prova con binary 
            '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
            Session(CSTESPORTAPDF) = True
            Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
            Dim stPathReport As String = Session(CSTPATHPDF)

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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.HRef = LnkName
            LnkStampa.Visible = True
            ' ''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            ' ''    LnkConfOrdine.HRef = LnkName
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.HRef = LnkName
        Else
            LnkStampa.HRef = LnkName
            LnkStampa.Visible = True
        End If

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

#Region "FUNZIONI SPEDIZIONI ELENCO DDT XLS/CSV VETTORE"

    Private Sub btnDDTSpedUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
            btnSpedNO.Click, btnSpedOK.Click, _
            btnSiLista.Click, btnNoLista.Click

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
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        Dim strSQL As String = ""
        If sender.ID = btnSpedNO.ID Then
            strSQL = "Update DocumentiT Set RefDataDDT = NULL Where IdDocumenti = " & myID.Trim
        ElseIf sender.ID = btnSpedOK.ID Then
            strSQL = "Update DocumentiT Set RefDataDDT = GETDATE() Where IdDocumenti = " & myID.Trim
        ElseIf sender.ID = btnSiLista.ID Then
            strSQL = "Update DocumentiT Set Acconto = 3 Where IdDocumenti = " & myID.Trim
        ElseIf sender.ID = btnNoLista.ID Then
            strSQL = "Update DocumentiT Set Acconto = 0 Where IdDocumenti = " & myID.Trim
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show(sender.text.Trim, "Funzione non abilitata.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        Session("DDTSpedUpdate") = strSQL
        Session(MODALPOPUP_CALLBACK_METHOD) = "DDTSpedUpdate"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show(sender.text.Trim, "Confermi il cambio di stato Spedizione del DDT ?<br>" + sender.ID.ToString.Trim, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub DDTSpedUpdate()
        Dim StrErrore As String = "" : Dim StrConferma As String = ""
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
        '-
        Dim strSQL As String = Session("DDTSpedUpdate")
        If IsNothing(strSQL) Then
            strSQL = ""
        End If
        If String.IsNullOrEmpty(strSQL) Then
            strSQL = ""
        End If
        If strSQL.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Cambio stato DDT per Spedizioni", "Funzione non abilitata.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        '---------------------------------------------------------
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim strValore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand
        SqlDbNewCmd.CommandText = strSQL
        SqlDbNewCmd.CommandType = System.Data.CommandType.Text
        SqlDbNewCmd.Connection = SqlConn
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            'SE MI RITORNA -1 ERRORE
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
        'giu271122 per evitare che si riposizioni, deve ricaricare se è nella sezione Spedizioni
        If rbtnDTInLista.Checked Or rbtnDTDaInviare.Checked Then
            Call btnRicerca_Click(Nothing, Nothing)
        ElseIf GridViewPrevT.Rows.Count > 0 Then 'qui aggiorno nel dataset lo stato 
            'BtnSetEnabledTo(True)
            Try
                'Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                'Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                'Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                'Session(CSTDATADOC) = row.Cells(CellIdxT.DataDoc).Text.Trim
                'Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                'Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                'Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                'BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
                'GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
                If InStr(strSQL.Trim.ToUpper, "Set RefDataDDT = NULL".ToUpper) > 0 Then
                    row.Cells(CellIdxT.RefDataDDT).Text = ""
                ElseIf InStr(strSQL.Trim.ToUpper, "Set RefDataDDT = GETDATE()".ToUpper) > 0 Then
                    row.Cells(CellIdxT.RefDataDDT).Text = Now.Date.ToString.Trim
                ElseIf InStr(strSQL.Trim.ToUpper, "Set Acconto = 3".ToUpper) > 0 Then
                    row.Cells(CellIdxT.Acconto).Text = "3"
                ElseIf InStr(strSQL.Trim.ToUpper, "Set Acconto = 0".ToUpper) > 0 Then
                    row.Cells(CellIdxT.Acconto).Text = "0"
                End If
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Dim myAcconto As String = row.Cells(CellIdxT.Acconto).Text.Trim
                Dim myRefDataDDT As String = row.Cells(CellIdxT.RefDataDDT).Text.Trim
                Dim myVettore1 As String = row.Cells(CellIdxT.Vettore1).Text.Trim
                BtnSetByStato(Stato, myAcconto, myRefDataDDT, myVettore1)
            Catch ex As Exception
                Call btnRicerca_Click(Nothing, Nothing)
            End Try
        Else
            Call btnRicerca_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnGeneraSpedDDT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGeneraSpedDDT.Click
        lnkSpedDDT.Visible = False
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
        Dim dvElDoc As DataView
        dvElDoc = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)
        'non serve in quanto prende il rowfilter quando viene assegnato
        'dvElDoc.RowFilter = SqlDSPrevTElenco.FilterExpression
        Dim TotDDT As Integer = 0
        If (dvElDoc Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        ElseIf dvElDoc.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Else
            TotDDT = dvElDoc.Count
        End If
        '--------------------------------------------------------------------
        Session(EL_DOC_TOPRINT) = Nothing
        Session(MODALPOPUP_CALLBACK_METHOD) = "ControlloDDTSped"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Generazione Spedizione DDT", "Confermi il Selezione/Controllo DDT ? <br> " & _
                        "Totale DDT selezionati: " & TotDDT.ToString.Trim, WUC_ModalPopup.TYPE_CONFIRM_YN)

    End Sub

    Public Sub ControlloDDTSped() 'TUTTI

        Session(CSTDECIMALIVALUTADOC) = ""
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strCVett As String = "" : Dim strTSped As String = ""
        Dim myID As String = ""
        'CONTROLLO
        GridViewPrevT.AllowPaging = True 'GIU020822
        BuidDett()
        Dim dvElDoc As DataView
        dvElDoc = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)
        If dvElDoc.Count > 0 Then dvElDoc.Sort = "Numero"
        If (dvElDoc Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If dvElDoc.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim i As Integer = 0 : Dim ii As Integer = 0
        '------------------------------------------------------
        Dim ELDocToPrint As New List(Of String)
        Try
            For i = 1 To dvElDoc.Count
                'N°DOCUMENTO
                If Not IsDBNull(dvElDoc.Item(ii).Item("Numero")) Then
                    strNDoc = FormattaNumero(dvElDoc.Item(ii).Item("Numero"))
                Else
                    strNDoc = ""
                End If
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                If Not IsDBNull(dvElDoc.Item(ii).Item("Data_Doc")) Then
                    strDataDoc = Format(dvElDoc.Item(ii).Item("Data_Doc"), FormatoData)
                Else
                    strDataDoc = ""
                End If
                If String.IsNullOrEmpty(strDataDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsDate(CDate(strDataDoc)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                If Not IsDBNull(dvElDoc.Item(ii).Item("Vettore_1")) Then
                    strCVett = FormattaNumero(dvElDoc.Item(ii).Item("Vettore_1"))
                Else
                    strCVett = ""
                End If
                'giu190523
                If Not IsDBNull(dvElDoc.Item(ii).Item("Tipo_Spedizione")) Then
                    strTSped = (dvElDoc.Item(ii).Item("Tipo_Spedizione")).ToString.Trim
                Else
                    strTSped = ""
                End If
                'SE ARRIVA QUI DEVE AVERE PER FORZA IL VETTORE
                If String.IsNullOrEmpty(strCVett) Then 'And strTSped = "V" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "VETTORE OBBLIGATORIO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strCVett) Then 'And strTSped = "V" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "VETTORE OBBLIGATORIO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Val(strCVett) < 1 Then 'And strTSped = "V" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "VETTORE OBBLIGATORIO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'ID DOCUMENTI
                'ID = rowCTR.Cells(CellIdxT.IDDoc).Text
                If Not IsDBNull(dvElDoc.Item(ii).Item("IDDocumenti")) Then
                    myID = dvElDoc.Item(ii).Item("IDDocumenti").ToString.Trim
                Else
                    myID = ""
                End If
                Session(IDDOCUMENTI) = myID
                myID = Session(IDDOCUMENTI)
                If IsNothing(myID) Then
                    myID = ""
                End If
                If String.IsNullOrEmpty(myID) Then
                    myID = ""
                End If
                If Not IsNumeric(myID) Then
                    ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '-
                If Not IsNothing(Session(EL_DOC_TOPRINT)) Then
                    ELDocToPrint = Session(EL_DOC_TOPRINT)
                End If
                ELDocToPrint.Add(myID.ToString.Trim)
                Session(EL_DOC_TOPRINT) = ELDocToPrint

                ii += 1
            Next
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Controlli DDT Spedizione. " + ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'FINE CONTROLLO
        '' ''OK FATTO
        Session(MODALPOPUP_CALLBACK_METHOD) = "EstraiDatiSpedDDT"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CallChiudi"
        ModalPopup.Show("Generazione Spedizione DDT", _
            "Selezione DDT avvenuta con successo. <br> <strong><span> " & _
            "Vuole procedere alla Generazione della Spedizione DDT  ?</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
    End Sub

    Public Sub EstraiDatiSpedDDT()
        If IsNothing(Session(EL_DOC_TOPRINT)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrint As List(Of String) = Session(EL_DOC_TOPRINT)
        Dim TotDDT As Integer = 0
        If (ELDocToPrint.Count > 0) Then
            TotDDT = ELDocToPrint.Count
            Try
                For Each Codice As String In ELDocToPrint
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.DocTrasportoClienti), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore, True)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'estrazione DDT. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 0
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'estrazione DDT. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Exit Sub
        End If
        ObjDB = Nothing
        '--------------
        Call OKGeneraSpedDDT(DsPrinWebDoc)
    End Sub
    Public Sub CallChiudi()
        ''Chiudi("")
    End Sub
    Private Sub OKGeneraSpedDDT(ByRef DsPrinWebDoc As DSPrintWeb_Documenti)
        Session("OKFILEDDT") = ""
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Sub
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim
        '--
        Dim rsTestata As DataRow
        'Dim rowDitta() As DataRow
        Dim rowDest() As DataRow
        Dim rowNazioni() As DataRow
        Dim CCliente As String = ""
        Dim CFiliale As String = ""
        Dim DestMerce1 As String = ""
        Dim DestMerce2 As String = ""
        Dim DestMerce3 As String = ""
        Dim CAPDestMerce As String = ""
        Dim LocDestMerce As String = ""
        Dim rowFiliale() As DataRow
        DsPrinWebDoc.SpedDDTDHL.Clear()
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim NumeroDDT As String = ""
        '-
        Call GetDatiAbilitazioni(CSTABILAZI, "AccountSped", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        Dim AccountSpedDDT As String = strValore.Trim
        If AccountSpedDDT.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito il DHL_Account nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '-
        Call GetDatiAbilitazioni(CSTABILAZI, "EmailSped", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        Dim EmailSpedDDT As String = strValore.Trim
        If EmailSpedDDT.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito Email per SpedDDT nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '-giu261022
        Call GetDatiAbilitazioni(CSTABILAZI, "TelSped", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        Dim TelSpedDDT As String = strValore.Trim
        If TelSpedDDT.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito Telefono SpedDDT nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Else 'giu290523
            TelSpedDDT = SoloNumeri(TelSpedDDT)
        End If
        '------
        'giu080423 Carico il Fornitore/Cliente dal Vettore>>Codice_Coge richiesta Erika 23/03/2023
        Call GetDatiAbilitazioni(CSTABILAZI, "SpedVettCsv", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        If strValore.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito il Vettore per SpedDDT nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        If DsPrinWebDoc.Tables("Vettori_1").Select("Codice=" + strValore.Trim).Length = 0 Then
            'carico il vettore per prendere il codice
            strSQL = "Select * From Vettori WHERE (Codice = " & strValore.Trim & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettori_1")
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento dati Vettore per SpedDDT nei Parametri Generali(1)", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
        End If
        Dim rowVettori1() As DataRow
        Dim CGCliForVettore As String = ""
        rowVettori1 = DsPrinWebDoc.Tables("Vettori_1").Select("Codice=" + strValore.Trim)
        If rowVettori1.Length = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento dati Vettore per SpedDDT nei Parametri Generali(2)", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If IsDBNull(rowVettori1(0).Item("Codice_CoGe")) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito il Vettore per SpedDDT nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Else
            CGCliForVettore = rowVettori1(0).Item("Codice_CoGe").ToString.Trim
        End If
        'carico l'anagrafica in Clienti/Fornitore
        If Left(CGCliForVettore, 1) <> "1" And Left(CGCliForVettore, 1) <> "9" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il Vettore per SpedDDT nei Parametri Generali deve contenere un codice Cliente/Fornitore", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        ElseIf Left(CGCliForVettore, 1) = "1" Then
            strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & CGCliForVettore.Trim & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti")
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento dati (Cliente) Vettore per SpedDDT nei Parametri Generali Codice: " + CGCliForVettore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
        ElseIf Left(CGCliForVettore, 1) = "9" Then
            strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & CGCliForVettore.Trim & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti")
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento dati (Fornitore) Vettore per SpedDDT nei Parametri Generali Codice: " + CGCliForVettore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il Vettore per SpedDDT nei Parametri Generali deve contenere un codice Cliente/Fornitore", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim rowCliForVett() As DataRow
        rowCliForVett = DsPrinWebDoc.Tables("Clienti").Select("Codice_Coge='" + CGCliForVettore.Trim + "'")
        If rowCliForVett.Length = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato presente (Cliente/Fornitore) Vettore per SpedDDT - Codice: " + CGCliForVettore, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '--------------------------------------------------------------
        'GIU261122 CONTROLLO FORMALE: ,NNN,X, per ogni codice inserito
        '-giu231122
        Call GetDatiAbilitazioni(CSTABILAZI, "CPagCA", strValore, strErrore)
        If strErrore.Trim <> "" Then
            strValore = ""
        End If
        Dim strCPagCA As String = strValore.Trim
        If strCPagCA.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato definito il Tipo Pag. Contrassegno nei Parametri Generali", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim myPos As Integer = 0
        Dim StrDato() As String = Nothing
        If strCPagCA.Trim <> "" Then
            myPos = 0
            myPos = InStr(strCPagCA, ",")
            If myPos > 0 Then
                myPos = 0
                StrDato = strCPagCA.Split(",")
                For I = 0 To StrDato.Count - 1
                    myPos += 1
                    If myPos > 3 Then myPos = 1
                Next
                If myPos <> 3 Then
                    strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Formalmente errato (,NNN,X, per ogni codice inserito)"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " & _
                                    strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Else
                strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Formalmente errato (,NNN,X, per ogni codice inserito)"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " & _
                                strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
        Dim myCPagCA As String = ""
        '------
        Dim PrimoN As String = "" : Dim UltimoN As String = ""
        Dim TotDDT As String = ""
        myPos = 0
        '-
        Try
            
            TotDDT = DsPrinWebDoc.Tables("DocumentiT").Rows.Count.ToString.Trim
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'giu090423 ora i dati Shipper sono i dati collegati al Vettore>>Codice_Coge
                '''rowDitta = DsPrinWebDoc.Ditta.Select()
                '''If rowDitta.Count = 0 Then
                '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                '''    ModalPopup.Show("Attenzione", "Dati Ditta Mittente insesistente.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                '''    Exit Sub
                '''End If
                For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                    NumeroDDT = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero).Replace(";", " ")
                    Dim newRow As DSPrintWeb_Documenti.SpedDDTDHLRow = DsPrinWebDoc.SpedDDTDHL.NewSpedDDTDHLRow
                    If PrimoN.Trim = "" Then
                        PrimoN = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero).Replace(";", " ")
                    End If
                    UltimoN = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero).Replace(";", " ")
                    newRow.BeginEdit()
                    '''newRow.Shipper_Contact = IIf(IsDBNull(rowDitta(0).Item("Descrizione")), "", rowDitta(0).Item("Descrizione")).Replace(";", " ")
                    newRow.Shipper_Contact = IIf(IsDBNull(rowCliForVett(0).Item("Rag_Soc")), "", rowCliForVett(0).Item("Rag_Soc")).Replace(";", " ")
                    strValore = IIf(IsDBNull(rowCliForVett(0).Item("Riferimento")), "", rowCliForVett(0).Item("Riferimento")).Replace(";", " ")
                    If strValore.Trim <> "" Then
                        newRow.Shipper_Contact = strValore.Trim
                    End If
                    'GIU290523
                    strValore = newRow.Shipper_Contact.Trim
                    newRow.Shipper_Contact = NoLettereAcc(strValore.Trim)
                    '---------
                    '''newRow.Shipper_Company = IIf(IsDBNull(rowDitta(0).Item("Descrizione")), "", rowDitta(0).Item("Descrizione")).Replace(";", " ")
                    newRow.Shipper_Company = IIf(IsDBNull(rowCliForVett(0).Item("Rag_Soc")), "", rowCliForVett(0).Item("Rag_Soc")).Replace(";", " ")
                    'GIU290523
                    strValore = newRow.Shipper_Company.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Shipper_Company = strValore.Trim
                    '---------
                    '''newRow.Shipper_Address1 = IIf(IsDBNull(rowDitta(0).Item("Indirizzo")), "", rowDitta(0).Item("Indirizzo")).Replace(";", " ")
                    newRow.Shipper_Address1 = IIf(IsDBNull(rowCliForVett(0).Item("Indirizzo")), "", rowCliForVett(0).Item("Indirizzo")).Replace(";", " ")
                    'GIU290523
                    strValore = newRow.Shipper_Address1.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Shipper_Address1 = strValore.Trim
                    '---------
                    newRow.Shipper_Address2 = ""
                    newRow.Shipper_Address3 = ""
                    '''newRow.Shipper_Zip_Code = IIf(IsDBNull(rowDitta(0).Item("CAP")), "", rowDitta(0).Item("CAP")).Replace(";", " ")
                    newRow.Shipper_Zip_Code = IIf(IsDBNull(rowCliForVett(0).Item("CAP")), "", rowCliForVett(0).Item("CAP")).Replace(";", " ")
                    newRow.Shipper_Zip_Code = SoloNumeri(newRow.Shipper_Zip_Code) 'giu290523
                    '''newRow.Shipper_City = IIf(IsDBNull(rowDitta(0).Item("Citta")), "", rowDitta(0).Item("Citta")).Replace(";", " ")
                    newRow.Shipper_City = IIf(IsDBNull(rowCliForVett(0).Item("Localita")), "", rowCliForVett(0).Item("Localita")).Replace(";", " ")
                    'GIU290523
                    strValore = newRow.Shipper_City.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Shipper_City = strValore.Trim
                    '---------
                    'GIU040523 
                    'newRow.Shipper_Country_Code = IIf(IsDBNull(rowCliForVett(0).Item("Nazione")), "IT", rowCliForVett(0).Item("Nazione")).Replace(";", " ") '"IT"
                    newRow.Shipper_Country_Code = "IT" 'GIU040523 FISSO COME IL CODE PHONE
                    newRow.Shipper_Mail = EmailSpedDDT
                    newRow.Shipper_Country_Code_Phone = "39" 'giu160523 aggiunta 'giu080523 EMAIL ERIKA 
                    'giu290523
                    strValore = SoloNumeri(TelSpedDDT)
                    If IsNumeric(strValore.Trim) Then
                        strValore = Mid(strValore.Trim, 1, 3) + " " + Mid(strValore.Trim, 4)
                    End If
                    TelSpedDDT = strValore.Trim
                    '-
                    newRow.Shipper_Phone = TelSpedDDT.Trim 'IIf(IsDBNull(rowDitta(0).Item("Telefono")), "", rowDitta(0).Item("Telefono")).Replace(";", " ")
                    newRow.Shipper_Reference = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero).Replace(";", " ")
                    newRow.DHL_ACCOUNT = AccountSpedDDT
                    'DESTINATARIO
                    CCliente = IIf(IsDBNull(rsTestata!Cod_Cliente), "", rsTestata!Cod_Cliente).ToString.Trim
                    rowDest = DsPrinWebDoc.Clienti.Select("Codice_CoGe='" + CCliente + "'")
                    If rowDest.Count = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Dati Destinatario non trovato in tabella.(" + CCliente.Trim + ") DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    'FILIALE/DESTINAZIONE MERCE
                    CFiliale = IIf(IsDBNull(rsTestata!Cod_Filiale), "", rsTestata!Cod_Filiale).ToString.Trim
                    rowFiliale = Nothing
                    If CFiliale.Trim <> "" Then
                        rowFiliale = DsPrinWebDoc.DestClienti.Select("Codice='" + CCliente + "' AND Progressivo= " + CFiliale.Trim)
                        If rowFiliale.Count = 0 Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Dati Filiale non trovato in tabella. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                    End If
                    '''newRow.Receiver_Contact = IIf(IsDBNull(rowDest(0).Item("Rag_Soc")), "", rowDest(0).Item("Rag_Soc")).Replace(";", " ")
                    '''newRow.Receiver_Contact += " " + IIf(IsDBNull(rowDest(0).Item("Denominazione")), "", rowDest(0).Item("Denominazione")).Replace(";", " ")
                    ''''-
                    '''DestMerce1 = IIf(IsDBNull(rsTestata!Destinazione1), "", rsTestata!Destinazione1).Replace(";", " ")
                    '''DestMerce2 = IIf(IsDBNull(rsTestata!Destinazione2), "", rsTestata!Destinazione2).Replace(";", " ")
                    '''DestMerce3 = IIf(IsDBNull(rsTestata!Destinazione3), "", rsTestata!Destinazione3).Replace(";", " ")
                    '''If DestMerce1.Trim <> "" And DestMerce3.Trim <> "" And DestMerce3.Trim <> "" Then
                    '''    newRow.Receiver_Company = DestMerce1.Trim
                    '''    newRow.Receiver_Address1 = DestMerce2.Trim
                    '''    newRow.Receiver_Address2 = ""
                    '''    newRow.Receiver_Address3 = ""
                    '''    CAPDestMerce = Mid(DestMerce3, 1, 5)
                    '''    LocDestMerce = Mid(DestMerce3, 7)
                    '''    If IsNumeric(CAPDestMerce) And LocDestMerce.Trim <> "" Then
                    '''        If CFiliale.Trim <> "" Then
                    '''            newRow.Receiver_Company += " " + IIf(IsDBNull(rowFiliale(0).Item("Denominazione")), "", rowFiliale(0).Item("Denominazione")).Replace(";", " ")
                    '''        End If
                    '''        newRow.Receiver_Zip_Code = CAPDestMerce
                    '''        newRow.Receiver_City = LocDestMerce.Replace(";", " ")
                    '''        'giu220922 tolgo la provincia altrimenti non trova corrispondenza
                    '''        myPos = InStr(LocDestMerce, "(")
                    '''        If myPos > 0 Then
                    '''            newRow.Receiver_City = Mid(LocDestMerce.Trim, 1, myPos - 1).Replace(";", " ")
                    '''        End If
                    '''        '----------------------------------------------------------------
                    '''        If CFiliale.Trim <> "" Then
                    '''            newRow.Receiver_Country_Code = IIf(IsDBNull(rowFiliale(0).Item("Stato")), "", rowFiliale(0).Item("Stato")).Replace(";", " ")
                    '''        Else
                    '''            newRow.Receiver_Country_Code = IIf(IsDBNull(rowDest(0).Item("Nazione")), "", rowDest(0).Item("Nazione")).Replace(";", " ")
                    '''        End If
                    '''    ElseIf CFiliale.Trim <> "" Then
                    '''        newRow.Receiver_Company = IIf(IsDBNull(rowFiliale(0).Item("Ragione_Sociale")), "", rowFiliale(0).Item("Ragione_Sociale")).Replace(";", " ")
                    '''        newRow.Receiver_Company += " " + IIf(IsDBNull(rowFiliale(0).Item("Denominazione")), "", rowFiliale(0).Item("Denominazione")).Replace(";", " ")
                    '''        newRow.Receiver_Address1 = IIf(IsDBNull(rowFiliale(0).Item("Indirizzo")), "", rowFiliale(0).Item("Indirizzo")).Replace(";", " ")
                    '''        newRow.Receiver_Address2 = ""
                    '''        newRow.Receiver_Address3 = ""
                    '''        newRow.Receiver_Zip_Code = IIf(IsDBNull(rowFiliale(0).Item("CAP")), "", rowFiliale(0).Item("CAP")).Replace(";", " ")
                    '''        newRow.Receiver_City = IIf(IsDBNull(rowFiliale(0).Item("Localita")), "", rowFiliale(0).Item("Localita")).Replace(";", " ")
                    '''        newRow.Receiver_Country_Code = IIf(IsDBNull(rowFiliale(0).Item("Stato")), "", rowFiliale(0).Item("Stato")).Replace(";", " ")
                    '''    End If
                    '''ElseIf CFiliale.Trim <> "" Then
                    If CFiliale.Trim <> "" Then
                        strValore = IIf(IsDBNull(rowFiliale(0).Item("Ragione_Sociale35")), "", rowFiliale(0).Item("Ragione_Sociale35")).Replace(";", " ")
                        If strValore.Trim <> "" Then
                            newRow.Receiver_Company = strValore.Trim
                        Else
                            newRow.Receiver_Company = IIf(IsDBNull(rowFiliale(0).Item("Ragione_Sociale")), "", rowFiliale(0).Item("Ragione_Sociale")).Replace(";", " ")
                        End If
                        If newRow.Receiver_Company.Trim.Length > 0 And newRow.Receiver_Company.Trim.Length < 36 Then
                            'OK
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Receiver_Company > 35 o mancante. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        newRow.Receiver_Address1 = IIf(IsDBNull(rowFiliale(0).Item("Indirizzo")), "", rowFiliale(0).Item("Indirizzo")).Replace(";", " ")
                        strValore = IIf(IsDBNull(rowFiliale(0).Item("Riferimento35")), "", rowFiliale(0).Item("Riferimento35")).Replace(";", " ")
                        If strValore.Trim <> "" Then
                            newRow.Receiver_Contact = strValore.Trim
                        Else
                            newRow.Receiver_Contact = IIf(IsDBNull(rowFiliale(0).Item("Riferimento")), "", rowFiliale(0).Item("Riferimento")).Replace(";", " ")
                        End If
                        If newRow.Receiver_Contact.Trim.Length > 0 And newRow.Receiver_Contact.Trim.Length < 36 Then
                            'OK
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Receiver_Contact > 35 o mancante. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        newRow.Receiver_Address2 = IIf(IsDBNull(rowFiliale(0).Item("Denominazione")), "", rowFiliale(0).Item("Denominazione")).Replace(";", " ")
                        newRow.Receiver_Address3 = ""
                        newRow.Receiver_Zip_Code = IIf(IsDBNull(rowFiliale(0).Item("CAP")), "", rowFiliale(0).Item("CAP")).Replace(";", " ")
                        newRow.Receiver_City = IIf(IsDBNull(rowFiliale(0).Item("Localita")), "", rowFiliale(0).Item("Localita")).Replace(";", " ")
                        newRow.Receiver_Country_Code = IIf(IsDBNull(rowFiliale(0).Item("Stato")), "", rowFiliale(0).Item("Stato")).Replace(";", " ")
                    Else
                        'giu200423 richiesta Ilaria/Erika sempre la destinazione merce
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Manca la Destinazione Merce. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                        '-------------------------------------------------------------
                        newRow.Receiver_Company = IIf(IsDBNull(rowDest(0).Item("Rag_Soc")), "", rowDest(0).Item("Rag_Soc")).Replace(";", " ")
                        If newRow.Receiver_Company.Trim.Length > 0 And newRow.Receiver_Company.Trim.Length < 36 Then
                            'OK
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Receiver_Company > 35 o mancante. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        newRow.Receiver_Address1 = IIf(IsDBNull(rowDest(0).Item("Indirizzo")), "", rowDest(0).Item("Indirizzo")).Replace(";", " ")
                        newRow.Receiver_Contact = IIf(IsDBNull(rowDest(0).Item("Riferimento")), "", rowDest(0).Item("Riferimento")).Replace(";", " ")
                        If newRow.Receiver_Contact.Trim.Length > 0 And newRow.Receiver_Contact.Trim.Length < 36 Then
                            'OK
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Receiver_Contact > 35 o mancante. DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        newRow.Receiver_Address2 = IIf(IsDBNull(rowDest(0).Item("Denominazione")), "", rowDest(0).Item("Denominazione")).Replace(";", " ")
                        newRow.Receiver_Address3 = ""
                        newRow.Receiver_Zip_Code = IIf(IsDBNull(rowDest(0).Item("CAP")), "", rowDest(0).Item("CAP")).Replace(";", " ")
                        newRow.Receiver_City = IIf(IsDBNull(rowDest(0).Item("Localita")), "", rowDest(0).Item("Localita")).Replace(";", " ")
                        newRow.Receiver_Country_Code = IIf(IsDBNull(rowDest(0).Item("Nazione")), "", rowDest(0).Item("Nazione")).Replace(";", " ")
                    End If
                    'GIU290523
                    strValore = newRow.Receiver_Contact.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_Contact = strValore.Trim
                    '-
                    strValore = newRow.Receiver_Company.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_Company = strValore.Trim
                    '-
                    strValore = newRow.Receiver_Address1.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_Address1 = strValore.Trim
                    '-
                    strValore = newRow.Receiver_Address2.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_Address2 = strValore.Trim
                    '-
                    strValore = newRow.Receiver_Address3.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_Address3 = strValore.Trim
                    '---------
                    newRow.Receiver_Zip_Code = SoloNumeri(newRow.Receiver_Zip_Code) 'giu290523
                    '-
                    strValore = newRow.Receiver_City.Trim
                    strValore = NoLettereAcc(strValore.Trim)
                    newRow.Receiver_City = strValore.Trim
                    '-----
                    rowNazioni = DsPrinWebDoc.Nazioni.Select("Codice='" + newRow.Receiver_Country_Code + "'")
                    If rowNazioni.Count = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Dati Nazioni non trovato in tabella.(" + newRow.Receiver_Country_Code.Trim + ") DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    strValore = IIf(IsDBNull(rowNazioni(0).Item("Codice_ISO")), "", rowNazioni(0).Item("Codice_ISO")).Replace(";", " ")
                    If strValore.Trim <> "" Then
                        newRow.Receiver_Country_Code = strValore.Trim
                    Else
                        strValore = IIf(IsDBNull(rowNazioni(0).Item("Codice")), "", rowNazioni(0).Item("Codice")).Replace(";", " ")
                        If strValore.Trim <> "" And strValore.Trim.Length <> 2 Then
                            newRow.Receiver_Country_Code = strValore.Trim
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", "Dati Nazioni non trovato in tabella o > di 2.(" + newRow.Receiver_Country_Code.Trim + ") DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                    End If
                    'GIU040523
                    If newRow.Receiver_Country_Code = "I" Then newRow.Receiver_Country_Code = "IT"
                    If newRow.Receiver_Country_Code = "ITA" Then newRow.Receiver_Country_Code = "IT"
                    '---------
                    'email
                    'GIU180123 SE PRESENTE PIU EMAIL INSERIRE SOLO LA PRIMA - DI DEFAULT SEMPRE PER PRIMA EMAIL DELLA FILIALE
                    If CFiliale.Trim <> "" Then
                        strValore = IIf(IsDBNull(rowFiliale(0).Item("Email")), "", rowFiliale(0).Item("Email").ToString.Trim)
                        ' mi serve dopo ).Replace(";", " ")
                        If strValore.Trim <> "" Then
                            'dopo per prendere solo una EMAIL newRow.Receiver_Mail = strValore.Trim
                        Else
                            strValore = IIf(IsDBNull(rowDest(0).Item("Email")), "", rowDest(0).Item("Email").ToString.Trim)
                            ').Replace(";", " ")
                            'DOPO newRow.Receiver_Mail = strValore.Trim
                        End If
                    Else
                        strValore = IIf(IsDBNull(rowDest(0).Item("Email")), "", rowDest(0).Item("Email").ToString.Trim)
                        ').Replace(";", " ")
                        'DOPO newRow.Receiver_Mail = strValore.Trim
                    End If
                    'giu180123 prendo solo la prima Email se presente piu di una (Erika)
                    If strValore.Trim <> "" Then
                        strValore = strValore.Trim.Replace(":", " ")
                        strValore = strValore.Trim.Replace(";", " ")
                        strValore = strValore.Trim.Replace(",", " ")
                        '-
                        myPos = InStr(strValore.Trim, " ")
                        If myPos > 0 Then
                            strValore = Mid(strValore.Trim, 1, myPos)
                        End If
                        'OK
                        If strValore.Trim <> "" Then
                            newRow.Receiver_Mail = strValore.Trim
                        End If
                    End If
                    '-
                    strValore = IIf(IsDBNull(rowNazioni(0).Item("Prefisso")), "", rowNazioni(0).Item("Prefisso")).Replace(";", " ")
                    If strValore.Trim <> "" Then
                        newRow.Receiver_Phone_Country_Code = strValore.Trim
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Non è stato definito il Prefisso nella Nazione: <br>" + rowNazioni(0).Item("Descrizione").ToString.Trim + " (" + rowNazioni(0).Item("Codice") + ")", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    '-
                    'Telefono
                    If CFiliale.Trim <> "" Then
                        strValore = IIf(IsDBNull(rowFiliale(0).Item("Telefono1")), "", rowFiliale(0).Item("Telefono1")).Replace(";", " ")
                        If strValore.Trim <> "" Then
                            newRow.Receiver_Phone = strValore.Trim
                        Else
                            strValore = IIf(IsDBNull(rowFiliale(0).Item("Telefono2")), "", rowFiliale(0).Item("Telefono2")).Replace(";", " ")
                            If strValore.Trim <> "" Then
                                newRow.Receiver_Phone = strValore.Trim
                            Else
                                strValore = IIf(IsDBNull(rowDest(0).Item("Telefono1")), "", rowDest(0).Item("Telefono1")).Replace(";", " ")
                                If strValore.Trim <> "" Then
                                    newRow.Receiver_Phone = strValore.Trim
                                Else
                                    strValore = IIf(IsDBNull(rowDest(0).Item("Telefono2")), "", rowDest(0).Item("Telefono2")).Replace(";", " ")
                                    If strValore.Trim <> "" Then
                                        newRow.Receiver_Phone = strValore.Trim
                                    End If
                                End If
                            End If
                        End If
                    Else
                        strValore = IIf(IsDBNull(rowDest(0).Item("Telefono1")), "", rowDest(0).Item("Telefono1")).Replace(";", " ")
                        If strValore.Trim <> "" Then
                            newRow.Receiver_Phone = strValore.Trim
                        Else
                            strValore = IIf(IsDBNull(rowDest(0).Item("Telefono2")), "", rowDest(0).Item("Telefono2")).Replace(";", " ")
                            If strValore.Trim <> "" Then
                                newRow.Receiver_Phone = strValore.Trim
                            End If
                        End If
                    End If
                    If newRow.IsReceiver_PhoneNull Then 'NumeroDDT
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Non è stato definito il Telefono1/2: DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    ElseIf newRow.Receiver_Phone.Trim = "" Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Non è stato definito il Telefono1/2: DDT n. " + NumeroDDT, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    'giu290523
                    strValore = SoloNumeri(newRow.Receiver_Phone)
                    If IsNumeric(strValore.Trim) Then
                        strValore = Mid(strValore.Trim, 1, 3) + " " + Mid(strValore.Trim, 4)
                    End If
                    newRow.Receiver_Phone = strValore.Trim
                    '-
                    strValore = IIf(IsDBNull(rowNazioni(0).Item("Prodotto_DHL")), "", rowNazioni(0).Item("Prodotto_DHL")).Replace(";", " ")
                    If strValore.Trim <> "" Then
                        newRow.Prodotto_DHL = strValore.Trim
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Non è stato definito il Prodotto_DHL nella Nazione: <br>" + rowNazioni(0).Item("Descrizione").ToString.Trim + " (" + rowNazioni(0).Item("Codice") + ")", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    '-
                    strValore = IIf(IsDBNull(rsTestata!Colli), "0", Format(rsTestata!Colli, "###0")).ToString.Replace(";", " ")
                    newRow.NR_Colli = strValore.Trim
                    strValore = IIf(IsDBNull(rsTestata!Peso), "0,00", Format(rsTestata!Peso, "###0.00")).ToString.Replace(";", " ")
                    newRow.Peso = strValore.Trim
                    '-
                    newRow.Lunghezza = "1"
                    newRow.Altezza = "1"
                    newRow.Profondita = "1"
                    newRow.Descrizione_Contenuto = "Materiale elettromedicale"
                    If newRow.Prodotto_DHL = "WPX" Or newRow.Prodotto_DHL = "DOX" Then
                        strValore = IIf(IsDBNull(rsTestata!TotNettoPagare), "", Format(rsTestata!TotNettoPagare, "###0.00")).ToString.Replace(";", " ")
                        newRow.Valore = strValore.Trim
                        newRow.Valuta = "EUR"
                    End If
                    newRow.Tipo_Spedizione = "G"
                    newRow.Sabato_Si = "N"
                    'GIU241122 Aggiunta PAGAMENTO CONTRASSEGNO
                    myCPagCA = IIf(IsDBNull(rsTestata!Cod_Pagamento), "", rsTestata!Cod_Pagamento).ToString.Replace(";", " ")
                    If InStr(strCPagCA.Trim.ToUpper, "," + myCPagCA.Trim.ToUpper + ",") > 0 Then
                        newRow.Servizio_Contrassegno = "Y" 'GIU230323 RICHIESTA EMAIL ERIKA
                        newRow.Sigla_Servizio_Contrassegno = "KB"
                        strValore = IIf(IsDBNull(rsTestata!TotNettoPagare), "", Format(rsTestata!TotNettoPagare, "###0.00")).ToString.Replace(";", " ")
                        newRow.Contrassegno_Importo = strValore.Trim
                        newRow.Contrassegno_Valuta = "EUR"
                        newRow.Tipologia_Incasso = ""
                        For I = 0 To StrDato.Count - 1
                            If StrDato(I).Trim.ToUpper = myCPagCA.Trim.ToUpper Then
                                I += 1
                                If I > StrDato.Count - 1 Then
                                    strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Formalmente errato (,NNN,X, per ogni codice inserito)"
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Attenzione", strErrore, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Sub
                                Else
                                    newRow.Tipologia_Incasso = StrDato(I).Trim
                                    Exit For
                                End If
                            End If
                        Next
                        If newRow.Tipologia_Incasso = "" Then
                            strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Tipologia_Incasso = SPAZI"
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", strErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Else
                        newRow.Servizio_Contrassegno = "N"
                        newRow.Sigla_Servizio_Contrassegno = ""
                        newRow.Contrassegno_Importo = "0,00"
                    End If
                    '-----------------------------------------
                    newRow.EndEdit()
                    DsPrinWebDoc.SpedDDTDHL.AddSpedDDTDHLRow(newRow)
                Next
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Scrittura dati Spedizione DDT.(1): " + ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        DsPrinWebDoc.AcceptChanges()
        '-
        'Dim readerfile As System.IO.StreamReader
        Dim writefile As System.IO.StreamWriter
        Dim NomeStampa As String = "SPEDIZIONE_DDT"
        Dim SubDirDOC As String = "DDTClienti"
        Dim strDataSPED As String = Format(Now, "yyyyMMddHHmm")
        'riporto il N° dal al DDT
        Dim strDalAl As String = PrimoN.Trim + "_" + UltimoN.Trim
        NomeStampa = NomeStampa.Trim & "_" & strDataSPED.Trim & "_" & strDalAl.Trim & "_TotDDT_" & TotDDT.Trim & "_" & InizialiUT.Trim & ".csv"
        Dim stPathReport As String = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        'esempio per la lettura
        'If File.Exists(pathfilename) Then
        '    readerfile = My.Computer.FileSystem.OpenTextFileReader(stPathReport + NomeStampa)
        '    dataultimoaggiornamento = readerfile.ReadToEnd().Replace(vbCr, "").Replace(vbLf, "")
        '    readerfile.Close()
        'End If
        '---------
        Try
            writefile = My.Computer.FileSystem.OpenTextFileWriter(stPathReport + NomeStampa, False)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Aperura file Spedizione DDT. " + ex.Message, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        Dim SWTitoli As Boolean = False
        Dim dc As DataColumn
        Try
            If (DsPrinWebDoc.Tables("SpedDDTDHL").Rows.Count > 0) Then
                SWTitoli = False
                For Each rsTestata In DsPrinWebDoc.Tables("SpedDDTDHL").Select("")
                    If SWTitoli = False Then
                        strValore = ""
                        SWTitoli = True
                        For Each dc In DsPrinWebDoc.SpedDDTDHL.Columns
                            strValore += dc.ColumnName.Trim + ";"
                        Next
                        strValore = Mid(strValore, 1, strValore.Length - 1)
                        writefile.WriteLine(strValore)
                    End If
                    strValore = ""
                    For Each dc In DsPrinWebDoc.SpedDDTDHL.Columns
                        If IsDBNull(rsTestata.Item(dc.ColumnName)) Then
                            strValore += ";"
                        Else
                            strValore += rsTestata.Item(dc.ColumnName).ToString.Trim.Replace(";", " ").Trim + ";"
                        End If
                    Next
                    strValore = Mid(strValore, 1, strValore.Length - 1)
                    writefile.WriteLine(strValore)
                Next
                writefile.Flush()
                writefile.Close()
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessun documento selezionato per la generazione Spedizione DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Scrittura file Spedizione DDT.(2) " + ex.Message, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        '---------------------
        lnkSpedDDT.Visible = True
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & NomeStampa
        lnkSpedDDT.HRef = LnkName
        Session("OKFILEDDT") = LnkName

        Session(MODALPOPUP_CALLBACK_METHOD) = "OKSpeditoDDT"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CallChiudi"
        ModalPopup.Show("Generazione Spedizione DDT", _
            "Generazione Spedizione DDT avvenuta con successo. <br> <strong><span> " & _
            "Vuole procedere all'aggiornamento stato SPEDITO SI per tutti i DDT  ?</span></strong><br>" & _
            "NOTA Dopo quest'aggiornamento i DDT non saranno più selezionati per le Spedizioni", WUC_ModalPopup.TYPE_CONFIRM_YN)
    End Sub
    Public Sub OKSpeditoDDT() 'TUTTI
        If IsNothing(Session(EL_DOC_TOPRINT)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per l'agg.stato SPEDITO SI DDT.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '--------
        Dim SQLStr As String = ""
        Dim ELDocToPrint As List(Of String) = Session(EL_DOC_TOPRINT)
        Dim TotDDT As Integer = 0
        If (ELDocToPrint.Count > 0) Then
            TotDDT = ELDocToPrint.Count
            For Each Codice As String In ELDocToPrint
                SQLStr = "Update DocumentiT Set RefDataDDT = GETDATE() Where IdDocumenti = " & Codice.Trim
                If AggStatoSpedDDT(SQLStr) = False Then Exit For
            Next
        Else
            Exit Sub
        End If
        '--------------
        '' ''OK FATTO
        Call btnRicerca_Click(Nothing, Nothing)
        'giu250722
        If lnkSpedDDT.HRef.ToString.Trim = "" Then
            Dim LnkRef As String = Session("OKFILEDDT")
            If IsNothing(Session("OKFILEDDT")) Then
                LnkRef = ""
            ElseIf String.IsNullOrEmpty(LnkRef) Then
                LnkRef = ""
            End If
            If LnkRef.Trim <> "" Then
                lnkSpedDDT.HRef = LnkRef
                lnkSpedDDT.Visible = True
            End If
        Else
            lnkSpedDDT.Visible = True
        End If
        '--------------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Agg.Stato SPEDITO DDT Spedizione", _
            "Operazione terminata con successo.", WUC_ModalPopup.TYPE_CONFIRM_Y)
    End Sub
    
    Private Function AggStatoSpedDDT(ByVal strSQL As String) As Boolean
        '---------------------------------------------------------
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim strValore As String = "" : Dim StrErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand
        SqlDbNewCmd.CommandText = strSQL
        SqlDbNewCmd.CommandType = System.Data.CommandType.Text
        SqlDbNewCmd.Connection = SqlConn
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            'SE MI RITORNA -1 ERRORE
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        AggStatoSpedDDT = True
    End Function
#End Region

End Class