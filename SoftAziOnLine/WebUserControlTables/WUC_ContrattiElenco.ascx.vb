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
Imports System.IO

Partial Public Class WUC_ContrattiElenco
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private SWFatturaPA As Boolean = False 'GIU080814
    Private SWSplitIVA As Boolean = False 'giu05018
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""

    Private Enum CellIdxT
        Stato = 1
        NumDoc = 2
        RevN = 3
        DataDoc = 4
        DataAccetta = 5

        CodCliForProvv = 6
        RagSoc = 7
        Denom = 8
        Loc = 9
        CAP = 10
        PIVA = 11
        CF = 12
        '------ ....
        DataInizio = 13
        DataFine = 14
        Riferimento = 15
        Dest1 = 16
        Dest2 = 17
        Dest3 = 18
        DurataTipo = 19
        DurataNum = 20
    End Enum
    Private Enum CellIdxD
        TipoDett = 1
        TipoDettR = 2
        CodArt = 3
        DesArt = 4
        SerieLotto = 5
        UM = 6
        Qta = 7
        QtaEv = 8
        QtaRe = 9
        QtaFa = 10
        Filiale = 11
        DataSc = 12
        DataScNext = 13
        IVA = 14
        Prz = 15
        ScV = 16
        Sc1 = 17
        Importo = 18
        ScR = 19
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ContrattiElenco.aspx?labelForm=Elenco CONTRATTI"
        'giu291119 CONTRATTI
        ModalPopup.WucElement = Me
        WFPCambiaStatoCA.WucElement = Me
        WFPFatturaCA.WucElement = Me
        WFPDocCollegati.WucElement = Me

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
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnElimina.Visible = False
            btnNuovaRev.Visible = False
            btnCopia.Visible = False
            btnCreaOF.Visible = False
            btnCreaDDT.Visible = False
            btnFatturaCA.Visible = False
            btnFatturaCAAC.Visible = False
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
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSRespArea.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        '-
        If (Not IsPostBack) Then
            Try
                Session(CSTNUOVOCADAOC) = SWNO
                Session(CSTNUOVOCADACA) = SWNO
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
                ddlRicerca.Items.Add("Codice CIG/CUP")
                ddlRicerca.Items(14).Value = "CC"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(15).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(16).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(17).Value = "D3"
                '--
                btnSblocca.Text = "Sblocca documento"
                btnCreaOF.Text = "Crea ordine Fornitore"
                'giu110319
                Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
                If IsNothing(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If String.IsNullOrEmpty(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                '-
                If SWRbtnTD.Trim = "" Or SWRbtnTD.Trim <> SWTD(TD.ContrattoAssistenza) Then
                    Session(CSTSTATODOCSEL) = "0"
                    Session(CSTSWRbtnTD) = SWTD(TD.ContrattoAssistenza)
                End If
                '-----------
                'GIU080319
                Dim strStatoDoc As String = Session(CSTSTATODOCSEL)
                If IsNothing(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                If String.IsNullOrEmpty(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                '-----------
                checkCausale.AutoPostBack = False : checkCausale.Checked = False : checkCausale.AutoPostBack = True
                DDLCausali.AutoPostBack = False
                DDLCausali.Enabled = False : DDLCausali.SelectedIndex = 0
                DDLCausali.AutoPostBack = True
                '-giu210622
                Session(IDRESPAREA) = 0
                Call DataBindRV()
                '-
                Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
                Session(CSTSORTPREVTEL) = "N"
                Session(SWOP) = SWOPNESSUNA
                'GIU080423
                Session(CSTSTATODOC) = "999"
                rbtnTutti.Checked = True
                strStatoDoc = "999"
                '---------
                If strStatoDoc = "" Then
                    Session(CSTSTATODOC) = "0"
                    rbtnDaEvadere.Checked = True
                ElseIf strStatoDoc = "1" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnEvaso.Checked = True
                ElseIf strStatoDoc = "2" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnParzEvaso.Checked = True
                ElseIf strStatoDoc = "3" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnChiusoNoEvaso.Checked = True
                ElseIf strStatoDoc = "4" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnNonEvadibile.Checked = True
                ElseIf strStatoDoc = "5" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnNonCompleto.Checked = True
                ElseIf strStatoDoc = "999" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnTutti.Checked = True
                ElseIf strStatoDoc = "998" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    ' ''rbtnImpegnati.Checked = True
                ElseIf strStatoDoc = "997" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    ' ''rbtnImpegnati100.Checked = True
                Else
                    Session(CSTSTATODOC) = "999"
                    rbtnTutti.Checked = True 'giu300123 
                End If
                'giu090319 lo esegue il Checked=true  BuidDett()
                Try
                    If GridViewPrevT.Rows.Count > 0 Then
                        Dim savePI = Session("PageIndex")
                        Dim saveSI = Session("SelIndex")
                        If String.IsNullOrEmpty(Session("SortExp")) Then
                            Session("SortExp") = "Numero"
                        End If
                        If String.IsNullOrEmpty(Session("SortDir")) Then
                            Session("SortDir") = 0
                        End If
                        GridViewPrevT.Sort(Session("SortExp"), Session("SortDir"))
                        GridViewPrevT.PageIndex = savePI
                        GridViewPrevT.SelectedIndex = saveSI
                        ImpostaFiltro()
                        GridViewPrevT.DataBind()
                        GridViewPrevD.DataBind()
                    End If

                Catch ex As Exception

                End Try

                '----------------------------------------------
                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    ' ''GridViewPrevT.SelectedIndex = 0
                    Try
                        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                        'giu220220
                        Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
                        Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
                        Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
                        '---------
                        BtnSetByStato(Stato)
                        GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
                    Catch ex As Exception
                        Session(IDDOCUMENTI) = ""
                    End Try
                    Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
                Else
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End If
                '-----------------------------------------------
                ' ''giu090319 viene eseguito dal rbtn....check=true BuidDett()
            Catch ex As Exception
                Chiudi("Errore: Caricamento Elenco contratti: " & ex.Message)
                Exit Sub
            End Try

        End If

        If Session(F_CAMBIOSTATO_APERTA) Then
            WFPCambiaStatoCA.Show()
        End If
        'giu090415
        If Session(F_EVASIONEPARZ_APERTA) Then
            WFPFatturaCA.Show()
        End If
        '-
        'Simone 290317
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
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
            btnCreaDDT.Enabled = False
            btnFatturaCA.Enabled = False
            btnFatturaCAAC.Enabled = False
            btnNuovaRev.Enabled = False
            btnCreaOF.Enabled = False
            'btnVerbale.Enabled = False
            'btnStampa.Enabled = False
        End If
        If myStato.Trim = "Parz. evaso" Then
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnNuovaRev.Enabled = False
            'btnVerbale.Enabled = False
        End If
        If SWBloccoElimina Or SWBloccoModifica Or SWBloccoCambiaStato Then
            btnSblocca.Visible = True
        End If
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaDDT.Enabled = Valore
        btnFatturaCA.Enabled = Valore
        btnFatturaCAAC.Enabled = Valore
        btnCreaOF.Enabled = Valore
        btnStampa.Enabled = Valore
        btnVerbale.Enabled = Valore
        btnCopia.Enabled = Valore
        btnNuovaRev.Enabled = Valore
        btnDocCollegati.Enabled = Valore 'Simone290317
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnDaEvadere_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvadere.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = True
        btnFatturaCA.Enabled = True
        btnFatturaCAAC.Enabled = True
        btnCreaOF.Enabled = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "0"
        Session(CSTSTATODOCSEL) = "0"
        BuidDett()
    End Sub
    Private Sub rbtnEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnEvaso.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = False
        btnFatturaCA.Enabled = False
        btnFatturaCAAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        'btnVerbale.Enabled = False
        'btnStampa.Enabled = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "1"
        Session(CSTSTATODOCSEL) = "1"
        BuidDett()
    End Sub
    Private Sub rbtnParzEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnParzEvaso.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = True
        btnFatturaCA.Enabled = True
        btnFatturaCAAC.Enabled = True
        btnCreaOF.Enabled = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "2"
        Session(CSTSTATODOCSEL) = "2"
        BuidDett()
    End Sub
    Private Sub rbtnChiusoNoEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiusoNoEvaso.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = False
        btnFatturaCA.Enabled = False
        btnFatturaCAAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        'btnVerbale.Enabled = False
        'btnStampa.Enabled = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "3"
        Session(CSTSTATODOCSEL) = "3"
        BuidDett()
    End Sub
    Private Sub rbtnNonEvadibile_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnNonEvadibile.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = False
        btnFatturaCA.Enabled = False
        btnFatturaCAAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnVerbale.Enabled = False
        'btnStampa.Enabled = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "4"
        Session(CSTSTATODOCSEL) = "4"
        BuidDett()
    End Sub
    Private Sub rbtnNonCompleto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnNonCompleto.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = False
        btnFatturaCA.Enabled = False
        btnFatturaCAAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnVerbale.Enabled = False
        'btnStampa.Enabled = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "5"
        Session(CSTSTATODOCSEL) = "5"
        BuidDett()
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        btnCreaDDT.Enabled = True
        btnFatturaCA.Enabled = True
        btnFatturaCAAC.Enabled = True
        btnCreaOF.Enabled = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        BuidDett()
    End Sub
    Private Sub checkCausale_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkCausale.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        If checkCausale.Checked = True Then
            rbtnTutti.AutoPostBack = False : rbtnTutti.Checked = True : rbtnTutti.AutoPostBack = True
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
            rbtnTutti.AutoPostBack = False : rbtnTutti.Checked = True : rbtnTutti.AutoPostBack = True
            LnkStampa.Visible = False : LnkVerbale.Visible = False
            btnCreaDDT.Enabled = True
            btnFatturaCA.Enabled = True
            btnFatturaCAAC.Enabled = True
            btnCreaOF.Enabled = True
            Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
            Session(CSTSTATODOC) = "999"
            Session(CSTSTATODOCSEL) = "999"
        End If
        BuidDett()
        If checkCausale.Checked = True Then
            DDLCausali.Focus()
        End If
    End Sub
    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Session(CSTSORTPREVTEL) = "N"
        If DDLCausali.SelectedIndex = 0 Then
            BtnSetEnabledTo(False)
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            SqlDSPrevTElenco.FilterExpression = ""
            Session(CSTTIPODOC) = "Z" 'NESSUNO solo quando seleziona l'elemento si attiva la ricerca
            Session(CSTTIPODOCSEL) = "Z"
            Session(CSTSTATODOC) = "999"
            Session(CSTSTATODOCSEL) = "999"
            Session(IDDOCUMENTI) = ""
            BuidDett()
            Exit Sub
        End If
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = CInt(DDLCausali.SelectedValue.ToString.Trim) + 1000
        Session(CSTSTATODOCSEL) = CInt(DDLCausali.SelectedValue.ToString.Trim) + 1000
        BuidDett()
        '---------
    End Sub

    Private Sub BuidDett()
        'GIU21062017
        ImpostaFiltro()
        SqlDSPrevTElenco.DataBind()
        '---------
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
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
            Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        checkParoleContenute.Text = "Parole contenute"
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
               ddlRicerca.SelectedValue = "DI" Or
               ddlRicerca.SelectedValue = "DF" Or
               ddlRicerca.SelectedValue = "DS" Or
               ddlRicerca.SelectedValue = "DA" Then
            checkParoleContenute.Text = ">= Alla Data"
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim <> "" Then
            BuidDett()
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
                ddlRicerca.SelectedValue = "DI" Or
                ddlRicerca.SelectedValue = "DF" Or
                ddlRicerca.SelectedValue = "DS" Or
                ddlRicerca.SelectedValue = "DA" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        'GIU090418 CHIAMO SEMPRE BUILDDETT ALTRIMENTI QUANDO AZZERO TXTRICERCA NON RIESEGUE LA RICERCA SU TUTTI
        ' ''If txtRicerca.Text.Trim <> "" Then
        BuidDett()
        ' ''End If
    End Sub
    'giu200617
    Private Sub ImpostaFiltro()
        ' ''Session(CSTRICERCA) = ""
        SqlDSPrevTElenco.FilterExpression = ""
        Session(CSTSORTPREVTEL) = "N"
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or
               ddlRicerca.SelectedValue = "DI" Or
               ddlRicerca.SelectedValue = "DF" Or
               ddlRicerca.SelectedValue = "DS" Or
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
                Session(CSTSORTPREVTEL) = "R"
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
        'giu260120
        If DDLCausali.SelectedIndex > 0 And checkCausale.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(Cod_Causale =" & DDLCausali.SelectedValue.ToString.Trim & ")"
        End If
        'giu010520
        If DDLRespArea.SelectedIndex > 0 And checkRespArea.Checked Then
            Session(CSTSORTPREVTEL) = "R"
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(RespArea =" & DDLRespArea.SelectedValue.ToString.Trim & ")"
        End If
        If DDLRespVisite.SelectedIndex > 0 And CheckRespVisite.Checked Then
            Session(CSTSORTPREVTEL) = "R"
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
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
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
        Response.Redirect("WF_Contratti.aspx?labelForm=Nuovo CONTRATTO da Ordine")
    End Sub

#Region " Crea Ordine Fornitore"
    Private Sub btnCreaOF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaOF.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewOF"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea Ordine a Fornitore da CONTRATTO Cliente", "Confermi la creazione dell'ordine a Fornitore ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewOF()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        If VerificaQtaPrenotata() = 0 Then
            OkCreaNewOF()
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "OkCreaNewOF"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Verifica quantità prenotata", "Attenzione, per questo CONTRATTO è già stato creato un ordine a FORNITORE, <br><br> si vuole procedere comunque alla creazione dell'ordine ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub OkCreaNewOF()
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
        'OK CREAZIONE NUOVO ORDINE A FORNITORE
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocOF]"
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
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub
    Private Function VerificaQtaPrenotata() As Decimal
        VerificaQtaPrenotata = 0
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
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
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select SUM(Qta_Prenotata) AS Tot_Qta_Prenotata From DocumentiD WHERE IDDocumenti = " & myID
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rowTes() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    rowTes = ds.Tables(0).Select()
                    VerificaQtaPrenotata = IIf(IsDBNull(rowTes(0).Item("Tot_Qta_Prenotata")), 0, rowTes(0).Item("Tot_Qta_Prenotata"))
                    Exit Function
                Else
                    VerificaQtaPrenotata = 0
                    Exit Function
                End If
            Else
                VerificaQtaPrenotata = 0
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Quantità prenotata: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
#End Region

#Region " Crea DDT"
    Private Sub btnCreaDDT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaDDT.Click

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
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaDDTRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
        '----
        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''ModalPopup.Show("Crea DDT da CONTRATTO (Evasione CONTRATTO)", "Confermi l'evasione del CONTRATTO selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
        '----------------------------------------------------------------------
    End Sub
    'giu060814
    Private Function CheckNumDoc(ByVal myTipoDoc As String, ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or
            myTipoDoc = SWTD(TD.DocTrasportoFornitori) Or
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
    'giu080814 giu090814
    Public Sub CreaDDT()
        Call CreaDDTOK(False)
    End Sub
    Public Sub CreaDDTRecuperaNum()
        Call CreaDDTOK(True)
    End Sub
    '---------
    Public Sub CreaDDTOK(Optional ByVal SWRecuperaNum As Boolean = False)
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
        'giu100120
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
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
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
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
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
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
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
        'GIU151111 DA IMPLEMENTARE 
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        'ModalPopup.Show("Crea DDT", "Creazione DDT avvenuta con sussesso. <br> Numero: " & NDoc.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Response.Redirect("WF_Documenti.aspx?labelForm=DOCUMENTO DI TRASPORTO CLIENTI")
    End Sub

#End Region

    Private Sub btnCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopia.Click
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaCopia"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo CONTRATTO (Copia CONTRATTO)", "Confermi la copia del CONTRATTO selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaCopia()

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
        'giu220321 prendendo il numero piu alto + 1
        Dim NDoc As Long = GetMaxNDoc(StrErrore) : Dim NRev As Integer = 0
        If NDoc < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante l'impegno numero Contratto Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------
        'OK CREAZIONE NUOVO CONTRATTO
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbScadenzario)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand
        '                          Insert_ConTCreateNewDoc
        SqlDbNewCmd.CommandText = "Insert_ConTCreateNewDoc"
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.ContrattoAssistenza)
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
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(SWOP) = SWOPMODIFICA
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Nuovo CONTRATTO Copia da altro Contratto")
    End Sub
    Private Sub btnNuovaRev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovaRev.Click
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "NuovaRev"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuova Rev.CONTRATTO", "Confermi la creazione nuova Rev. del CONTRATTO selezionato ? <br> Attenzione, la Rev.CONTRATTO precedente <br> sarà ANNULLATO.", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub NuovaRev()

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
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0

        Dim row As GridViewRow = GridViewPrevT.SelectedRow
        Dim strNumero As String = row.Cells(CellIdxT.NumDoc).Text.Trim
        If strNumero.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        ElseIf Not IsNumeric(strNumero.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If

        If GetNewRevN(SWTD(TD.OrdClienti), strNumero.Trim, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        NDoc = CLng(strNumero.Trim)
        'OK CREAZIONE NUOVO CONTRATTO
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdClienti)
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        'AGGIORNO LE REVISIONI PRECEDENTI DA STATO 0 A 3 CHIUSO NON EVASO
        Dim SWOk As Boolean = True : Dim SWOkT As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=3 WHERE IDDocumenti <> " & Session(IDDOCUMENTI).ToString.Trim
            SQLStr += " AND Tipo_Doc = '" & SWTD(TD.OrdClienti) & "'"
            SQLStr += " AND Numero = '" & strNumero.Trim & "'"
            SQLStr += " AND StatoDoc = 0"
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", "Errore durante la cancellazione movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''Exit Sub
        End Try
        ObjDB = Nothing
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Contratti.aspx?labelForm=Gestione CONTRATTI")
    End Sub
    Private Function GetNewRevN(ByVal TDoc As String, ByVal NDoc As String, ByRef NRev As Integer) As Boolean
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MAX(RevisioneNDoc) AS RevisioneNDoc From DocumentiT WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        strSQL += " AND Numero = '" & NDoc.Trim & "'"
        ' ''strSQL += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rowTes() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    rowTes = ds.Tables(0).Select()
                    NRev = IIf(IsDBNull(rowTes(0).Item("RevisioneNDoc")), 0, rowTes(0).Item("RevisioneNDoc"))
                    NRev += 1
                    GetNewRevN = True
                    Exit Function
                Else
                    NRev = 0
                    GetNewRevN = True
                    Exit Function
                End If
            Else
                NRev = 0
                GetNewRevN = True
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica N.Documento/Rev.N° da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    'giu220321
    Private Function GetMaxNDoc(ByRef _strErrore As String) As Long
        _strErrore = ""
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From ContrattiT WHERE Tipo_Doc = '" & SWTD(TD.ContrattoAssistenza) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetMaxNDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetMaxNDoc = 1
                    End If
                    Exit Function
                Else
                    GetMaxNDoc = 1
                    Exit Function
                End If
            Else
                GetMaxNDoc = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetMaxNDoc = -1
            _strErrore = "Errore Verifica N.Contratto da impegnare: " & Ex.Message.Trim
            Exit Function
        End Try

    End Function
    '---------
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
        Response.Redirect("WF_Contratti.aspx?labelForm=Elimina CONTRATTO")
    End Sub

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu210617
        ImpostaFiltro()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                'giu220220
                Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
                Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
                Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
                '---------
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
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
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu210617
        ImpostaFiltro()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                'giu220220
                Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
                Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
                Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
                '---------
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
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
        Session("SortExp") = e.SortExpression
        Session("SortDir") = e.SortDirection
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
        Catch ex As Exception
        End Try
        Session("SortExp") = e.SortExpression
        Session("SortDir") = e.SortDirection
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            'giu220220
            Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
            Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
            Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
            '---------
            BtnSetByStato(Stato)
            DDLTipoDettagli.SelectedIndex = 0
            Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataAccetta).Text) Then
                e.Row.Cells(CellIdxT.DataAccetta).Text = Format(CDate(e.Row.Cells(CellIdxT.DataAccetta).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataInizio).Text) Then
                e.Row.Cells(CellIdxT.DataInizio).Text = Format(CDate(e.Row.Cells(CellIdxT.DataInizio).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataFine).Text) Then
                e.Row.Cells(CellIdxT.DataFine).Text = Format(CDate(e.Row.Cells(CellIdxT.DataFine).Text), FormatoData).ToString
            End If
            'giu031219
            ' ''If IsNumeric(e.Row.Cells(CellIdxT.PercImp).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxT.PercImp).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxT.PercImp).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.PercImp).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxT.PercImp).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxT.PercImPorto).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxT.PercImPorto).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxT.PercImPorto).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.PercImPorto).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxT.PercImPorto).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsDate(e.Row.Cells(CellIdxT.DataRif).Text) Then
            ' ''    e.Row.Cells(CellIdxT.DataRif).Text = Format(CDate(e.Row.Cells(CellIdxT.DataRif).Text), FormatoData).ToString
            ' ''End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
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

    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaFa).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaFa).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaFa).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaFa).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaFa).Text = ""
                End If
            End If
            If Len(e.Row.Cells(CellIdxD.Filiale).Text.Trim) > 5 Then
                e.Row.Cells(CellIdxD.Filiale).Text = Mid(e.Row.Cells(CellIdxD.Filiale).Text, 1, 5)
            End If
            If IsDate(e.Row.Cells(CellIdxD.DataSc).Text) Then
                e.Row.Cells(CellIdxD.DataSc).Text = Format(CDate(e.Row.Cells(CellIdxD.DataSc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxD.DataScNext).Text) Then
                e.Row.Cells(CellIdxD.DataScNext).Text = Format(CDate(e.Row.Cells(CellIdxD.DataScNext).Text), FormatoData).ToString
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

    'giu011223 per il Cambio Periodo
    Private Function GetMyCodVisita() As String
        Dim myCodVisita As String = "" : Dim Comodo As String = ""
        myCodVisita = Session("myCodVisita")
        If IsNothing(myCodVisita) Then
            myCodVisita = ""
        End If
        If String.IsNullOrEmpty(myCodVisita) Then
            myCodVisita = ""
        End If
        If myCodVisita.Trim <> "" Then
            GetMyCodVisita = myCodVisita
            Exit Function
        End If

        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = "Select * From TipoContratto"

        Dim DsDoc As New DSDocumenti
        Try
            DsDoc.Tables("TipoContratto").Clear()
            DsDoc.Tables("TipoContratto").AcceptChanges()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsDoc, "TipoContratto")
            For Each rsTC In DsDoc.Tables("TipoContratto").Select("")
                Comodo = IIf(IsDBNull(rsTC!Descrizione), "", rsTC!CodVisita.ToString.Trim)
                If InStr(myCodVisita, Comodo.Trim) > 0 Then
                Else
                    myCodVisita += Comodo.Trim + ","
                End If
            Next
        Catch ex As Exception
            myCodVisita = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento Tabella TipoContratto. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
        End Try
        ObjDB = Nothing
        Session("myCodVisita") = myCodVisita
        GetMyCodVisita = myCodVisita
    End Function

    Private Sub GridViewPrevD_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevD.SelectedIndexChanged
        Try
            Session("PageIndexD") = GridViewPrevD.PageIndex
            Session("SelIndexD") = GridViewPrevD.SelectedIndex

            Session("DNRRiga") = ""
            Dim row As GridViewRow = GridViewPrevD.SelectedRow
            Session("DNRRigaDATA") = ""
            If DDLTipoDettagli.SelectedValue.Trim <> "0" Then
                If GetMyCodVisita().Trim.ToUpper.Contains(row.Cells(CellIdxD.CodArt).Text.Trim.ToUpper) Then
                    lblAPartireDal.Text = "selezionare l'Attività "
                    btnSostNSerie.Enabled = False
                    txtDataDal.Enabled = False
                    ImgDataDal.Enabled = False
                Else
                    Dim myQtaEv As String = row.Cells(CellIdxD.QtaEv).Text.Trim
                    Dim myQtaFa As String = row.Cells(CellIdxD.QtaFa).Text.Trim
                    If (myQtaEv Is Nothing) Then
                        myQtaEv = "0"
                    End If
                    If String.IsNullOrEmpty(myQtaEv) Then
                        myQtaEv = "0"
                    ElseIf Not IsNumeric(myQtaEv.Trim) Then
                        myQtaEv = "0"
                    End If
                    '-
                    If (myQtaFa Is Nothing) Then
                        myQtaFa = "0"
                    End If
                    If String.IsNullOrEmpty(myQtaFa) Then
                        myQtaFa = "0"
                    ElseIf Not IsNumeric(myQtaFa.Trim) Then
                        myQtaFa = "0"
                    End If
                    If CInt(myQtaEv) > 0 Or CInt(myQtaFa) > 0 Then
                        lblAPartireDal.Text = "selezionare l'Attività "
                        btnSostNSerie.Enabled = False
                        txtDataDal.Enabled = False
                        ImgDataDal.Enabled = False
                    Else
                        Session("DNRRiga") = GridViewPrevD.SelectedDataKey.Value
                        Session("DNRRigaDATA") = row.Cells(CellIdxD.DataSc).Text.Trim
                        Dim myData As String = Session("DNRRigaDATA")
                        If (myData Is Nothing) Then
                            myData = ""
                        End If
                        If String.IsNullOrEmpty(myData) Then
                            myData = ""
                        End If
                        If IsDate(myData) Then
                            lblAPartireDal.Text = "dalla data " + myData + " in "
                            btnSostNSerie.Enabled = chkSostNSerie.Checked
                            txtDataDal.Enabled = chkSostNSerie.Checked
                            ImgDataDal.Enabled = chkSostNSerie.Checked
                            chkSostNSerie.Enabled = True
                        Else
                            lblAPartireDal.Text = "selezionare l'Attività "
                            btnSostNSerie.Enabled = False
                            txtDataDal.Enabled = False
                            ImgDataDal.Enabled = False
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Session("DNRRiga") = ""
        End Try
    End Sub
    Private Sub GridViewPrevD_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevD.Sorted
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        GridViewPrevD.DataBind()
    End Sub
    Private Sub GridViewPrevD_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevD.Sorting
        Session("SortExpD") = e.SortExpression
        Session("SortDirD") = e.SortDirection
    End Sub
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
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnStampa.Click
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Session(CSTTASTOST) = btnStampa.ID

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
        Call OKApriStampa(DsPrinWebDoc)
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
        BuidDett()
    End Sub

    'gu100120
#Region "Fatturazione Parziale contratto"
    Private Sub btnFatturaCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFatturaCA.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        '.
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
        '------------------------------------------
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "FatturaCAParz"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Fatturazione CONTRATTO", "Confermi l'emissione della fattura del CONTRATTO selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub FatturaCAParz()
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
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'PER L'EVASIONE PARZIALE

        ApriFattCAParz()
    End Sub
    Private Sub ApriFattCAParz()
        WFPFatturaCA.WucElement = Me
        WFPFatturaCA.SetLblMessUtente("Seleziona/modifica Quantità articoli da fatturare")
        Session(F_EVASIONEPARZ_APERTA) = True
        WFPFatturaCA.Show(True)
    End Sub
    Public Sub CallBackWFPFatturaParzCA()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
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

        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            'ok proseguo 'giu090415
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu050415
        StrErrore = ""
        'giu040814 salto il test visto che il DDT ha il segnale se DEVe essere PA oppure no
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
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
        Dim strDesTipoDocumento As String = "FATTURA COMMERCIALE"
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        If myNuovoNumero <> CkNumDocFC Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaFatturaRecuperaNum"
            ModalPopup.Show("Crea nuova Fattura da CONTRATTO", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDocFC) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da CONTRATTO", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
#End Region

#Region " Crea Fattura"
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
        'giu100120
        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            Dim StrSql As String
            Try
                StrSql = "Update ContrattiD Set Qta_Selezionata = 0 Where IdDocumenti = " & Session(IDDOCUMENTI)
                ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata=0 Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            '---------------------
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            '-
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update ContrattiD Set Qta_Selezionata = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata=QtaDaEv Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu100120
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
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
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCCA]"
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
        ' per una data specifica SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Doc", System.Data.SqlDbType.DateTime, 0, "Data_Doc"))
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
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE DA CONTRATTO")
    End Sub
    'giu120412 GESTIONE RECUPERO NUMERI FATTURA NON USATI
    Public Sub CreaFatturaRecuperaNum()
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
        'giu090415
        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            Dim StrSql As String
            Try
                StrSql = "Update ContrattiD Set Qta_Selezionata = 0 Where IdDocumenti = " & Session(IDDOCUMENTI)
                ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata = 0 Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            '---------------------
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            '-
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update ContrattiD Set Qta_Selezionata = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Selezionata = QtaDaEv Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu100120
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
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
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCCA]"
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
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE DA CONTRATTO")
    End Sub
#End Region

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSORTPREVTEL) = "N"
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
        WFPDocCollegati.PopolaGrigliaWUCDocCollegatiCM()
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
#End Region

#Region "Emissione fattura per ACCONTO/SALDO FORNITURA"
    Private Sub btnFatturaCAAC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFatturaCAAC.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        '.
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

        Session(MODALPOPUP_CALLBACK_METHOD) = "FatturaCAAC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "FatturaAC"
        ModalPopup.Show("Emissione Fattura d'Acconto/Saldo", "Confermi l'emissione della fattura d'Acconto/Saldo ?<BR>NOTA, Non sarà evasa alcuna voce del CONTRATTO.", WUC_ModalPopup.TYPE_CONFIRM_YNA)
    End Sub
    'NESSUN DOCUMENTO COLLEGATO
    Public Sub FatturaAC()
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
        SWFatturaPA = False
        SWSplitIVA = False
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        'GIU230312 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim strErrore As String = "" : Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(strErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCAC"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewFCACRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCAC"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub CreaNewFCAC()
        '----------------------------
        Session(CSTFATTURAPA) = False
        SWFatturaPA = False
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWSI
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Public Sub CreaNewFCACRecuperaNum()
        '----------------------------
        Session(CSTFATTURAPA) = False
        SWFatturaPA = False
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWNO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    'giu020519 CON DOCUMENTO COLLEGATO 
    Public Sub FatturaCAAC()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        'giu080312
        Dim StrErrore As String = ""
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
        '------------------------------------------
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
        '@@@@@@@@@@@ DOCUMENTO IN 
        'giu100120
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
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
        '-
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        '--------------------------------------------
        'GESTIONE RECUPERO BUCHI NUMERAZIONE
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(StrErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Ultimo numero Fattura. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If NDoc <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCACOR"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewFCACORRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(NDoc) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCACOR"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(NDoc) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub CreaNewFCACOR()
        Call CreaNewFCACOR_OK(False)
    End Sub
    Public Sub CreaNewFCACORRecuperaNum()
        Call CreaNewFCACOR_OK(True)
    End Sub
    Private Sub CreaNewFCACOR_OK(Optional ByVal SWRecuperaNum As Boolean = False)
        Dim StrErrore As String = ""
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
        '------------------------------------------
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
        '@@@@@@@@@@@ DOCUMENTO IN 
        'giu100120
        SWFatturaPA = Documenti.CKClientiIPAByIDConORCod(CLng(myID), "", SWSplitIVA, StrErrore)
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
        '-
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        '--------------------------------------------
        'GESTIONE RECUPERO BUCHI NUMERAZIONE
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(StrErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Ultimo numero Fattura. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf SWRecuperaNum = True Then
            NDoc = CkNumDoc
        End If

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
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCAcconto]"
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
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
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
        'giu140615 Dim LnkName As String = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/" & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            LnkVerbale.Visible = True
        Else
            LnkStampa.Visible = True
        End If

        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''If Session(CSTTASTOST) = btnStampa.ID Then
        '''    LnkStampa.HRef = LnkName
        '''ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
        '''    LnkVerbale.HRef = LnkName
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

    'giu220220
    Private Function SetDDLDettDurNumRiga() As Boolean
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then myID = "-1"
        ' ''If Not IsNumeric(myID) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Function
        ' ''End If
        '------------
        SetDDLDettDurNumRiga = True
        DDLDurNumRIga.Items.Clear()
        DDLDurNumRIga.Items.Add("[Tutti]")
        DDLDurNumRIga.Items(0).Value = -1
        If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
            Dim strSQL As String = ""
            Dim ObjDB As New DataBaseUtility
            Dim dsArt As New DataSet
            'giu190723 AND Riga=1"
            strSQL = "Select DurataNumRiga, Serie From ContrattiD WHERE IDDocumenti=" & myID.Trim & " AND DurataNum=0 GROUP BY DurataNumRiga, Serie ORDER BY DurataNumRiga"
            Dim strComodo As String = ""
            Try
                Dim myRighe As Integer = 0
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
                If (dsArt.Tables.Count > 0) Then
                    If (dsArt.Tables(0).Rows.Count > 0) Then
                        myRighe = dsArt.Tables(0).Rows.Count
                    End If
                End If
                If myRighe > 0 Then
                    For i = 0 To myRighe - 1
                        strComodo = IIf(IsDBNull(dsArt.Tables(0).Rows(i).Item("Serie")), "[????]", dsArt.Tables(0).Rows(i).Item("Serie"))
                        If strComodo <> "[????]" Then strComodo = FormattaNomeFile(strComodo) 'giu180723
                        DDLDurNumRIga.Items.Add((i + 1).ToString.Trim & " - " & strComodo)
                        DDLDurNumRIga.Items(i + 1).Value = i
                    Next i
                Else
                    DDLDurNumRIga.Items.Add("[Nessuna]")
                    DDLDurNumRIga.Items(0).Value = 0
                End If
                ' ''If myRighe > 0 Then
                ' ''    DDLDurNumRIga.Items.Add((myRighe + 1).ToString.Trim & " - [Nuova]")
                ' ''    DDLDurNumRIga.Items(myRighe + 1).Value = myRighe
                ' ''End If
                DDLDurNumRIga.SelectedIndex = 0
            Catch Ex As Exception
                SetDDLDettDurNumRiga = False
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore in ContrattiDett.GetDatiGiacenza", "Lettura articoli: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Function
            End Try
        Else
            Dim myDurataNum As String = Session(CSTDURATANUM)
            If IsNothing(myDurataNum) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDurataNum) Then
                Exit Function
            ElseIf Not IsNumeric(myDurataNum) Then
                Exit Function
            ElseIf Val(myDurataNum) = 0 Then
                Exit Function
            End If
            '-
            Dim myDurataTipo As String = Session(CSTDURATATIPO)
            If IsNothing(myDurataTipo) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDurataTipo) Then
                Exit Function
            ElseIf myDurataTipo.Trim = "" Then
                Exit Function
            End If
            '-
            Dim myDataInizio As String = Session(CSTDATAINIZIO)
            If IsNothing(myDataInizio) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDataInizio) Then
                Exit Function
            ElseIf Not IsDate(myDataInizio.Trim) Then
                Exit Function
            End If
            '----------

            If myDurataTipo.Trim = "M" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i + 1).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "T" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i + 1).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "Q" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i + 1).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "S" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i + 1).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "A" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio).Year, "0000"))
                    DDLDurNumRIga.Items(i + 1).Value = i
                    myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
                Next i
            Else 'non capitera' mai
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add((i + 1).ToString.Trim & " - ????")
                    DDLDurNumRIga.Items(i + 1).Value = i
                Next i
            End If
            DDLDurNumRIga.SelectedIndex = 0
        End If
    End Function
    Private Sub DDLTipoDettagli_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoDettagli.SelectedIndexChanged
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedValue
        Session(IDDURATANUMRIGA) = "-1" '"0"
        Session("DNRRiga") = ""
        Session("DNRRigaDATA") = ""
        DDLDurNumRIga.BackColor = SEGNALA_OK
        txtSerieNEW.BackColor = SEGNALA_OK
        txtDataDal.BackColor = SEGNALA_OK
        chkSostNSerie.Enabled = False
        txtSerieNEW.Enabled = False
        txtDataDal.Enabled = False
        btnSostNSerie.Enabled = False
        If DDLTipoDettagli.SelectedValue.Trim <> "0" Then
            GridViewPrevD.SelectedIndex = -1
            chkSostNSerie.Text = "Cambio Periodo Consumabile"
            txtSerieNEW.Visible = False
            lblAPartireDal.Text = "selezionare l'Attività "
            btnSostNSerie.Text = "OK Cambia Periodo"
            '-
            chkSostNSerie.Enabled = True
            txtSerieNEW.Enabled = chkSostNSerie.Checked
            txtDataDal.Enabled = chkSostNSerie.Checked
            ImgDataDal.Enabled = chkSostNSerie.Checked
        Else
            chkSostNSerie.Text = "Sostituzione N°Serie in"
            txtSerieNEW.Visible = True
            lblAPartireDal.Text = "a partire dal"
            btnSostNSerie.Text = "OK Sostituzione N°Serie"
            '-
            chkSostNSerie.Enabled = True
            txtSerieNEW.Enabled = chkSostNSerie.Checked
            txtDataDal.Enabled = chkSostNSerie.Checked
            btnSostNSerie.Enabled = chkSostNSerie.Checked
            ImgDataDal.Enabled = chkSostNSerie.Checked
        End If
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            'giu220220
            Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
            Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
            Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
            '---------
        Catch ex As Exception

        End Try
        Call SetDDLDettDurNumRiga()

    End Sub
    Private Sub DDLDurNumRIga_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLDurNumRIga.SelectedIndexChanged
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedValue
        Session(IDDURATANUMRIGA) = DDLDurNumRIga.SelectedValue
        DDLDurNumRIga.BackColor = SEGNALA_OK
        If DDLTipoDettagli.SelectedValue.Trim <> "0" Then
            lblAPartireDal.Text = "selezionare l'Attività "
            Session("DNRRiga") = ""
            Session("DNRRigaDATA") = ""
            GridViewPrevD.SelectedIndex = -1
            DDLDurNumRIga.BackColor = SEGNALA_OK
            txtSerieNEW.BackColor = SEGNALA_OK
            txtDataDal.BackColor = SEGNALA_OK
            chkSostNSerie.Enabled = False
            txtSerieNEW.Enabled = False
            txtDataDal.Enabled = False
            btnSostNSerie.Enabled = False
        End If
    End Sub

    Private Sub checkRespArea_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkRespArea.CheckedChanged
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        If checkRespArea.Checked = True Then
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = True : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
        Else
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = False : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
        End If
        Session(IDRESPAREA) = 0
        Call DataBindRV()
        BuidDett()
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
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        If CheckRespVisite.Checked = True Then
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = True : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
        Else
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = False : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
        End If
        BuidDett()
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
        If DDLRespArea.SelectedIndex = 0 Or DDLRespArea.SelectedItem.Text.Trim = "" Then
            Session(IDRESPAREA) = 0
        Else
            Session(IDRESPAREA) = DDLRespArea.SelectedValue
        End If
        Call DataBindRV()
        BuidDett()
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
        Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
        DDLRespVisite.BackColor = SEGNALA_OK
        BuidDett()
        If DDLRespVisite.SelectedIndex = 0 Then
            DDLRespVisite.BackColor = SEGNALA_KO
        Else
            DDLRespVisite.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub btnSostNSerie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSostNSerie.Click

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
        Dim strMess As String = ""
        Dim myData As String = Session("DNRRigaDATA")
        If DDLTipoDettagli.SelectedValue.Trim <> "0" Then 'CAMBIO ANNO CONSUMABILE
            If lblAPartireDal.Text.Trim = "selezionare l'Attività" Then
                strMess += "Selezionare l'Attività<br>"
            Else
                '-
                If (myData Is Nothing) Then
                    myData = ""
                End If
                If String.IsNullOrEmpty(myData) Then
                    myData = ""
                End If
                If myData.Trim = "" Then
                    lblAPartireDal.Text = "selezionare l'Attività "
                    btnSostNSerie.Enabled = False
                ElseIf IsDate(myData) Then
                    lblAPartireDal.Text = "dalla data " + myData + " in "
                    btnSostNSerie.Enabled = True
                    '-
                    Dim myDNRRiga = Session("DNRRiga")
                    If (myDNRRiga Is Nothing) Then
                        myDNRRiga = ""
                    End If
                    If String.IsNullOrEmpty(myDNRRiga) Then
                        myDNRRiga = ""
                    End If
                    If myDNRRiga = "" Then
                        lblAPartireDal.Text = "selezionare l'Attività (myDNRRiga)"
                        btnSostNSerie.Enabled = False
                    End If
                Else
                    lblAPartireDal.Text = "selezionare l'Attività "
                    btnSostNSerie.Enabled = False
                End If
            End If
            If Not IsDate(txtDataDal.Text.Trim) Then
                strMess += "NON è stata definita la nuova data OPPURE non compresa nelle Date Inizio/Fine Contratto<br>"
                txtDataDal.BackColor = SEGNALA_KO
            Else
                txtDataDal.BackColor = SEGNALA_OK
            End If
            If myData.Trim = txtDataDal.Text.Trim Then
                strMess += "Le date non posso essere uguali<br>"
                txtDataDal.BackColor = SEGNALA_KO
            End If
            '-
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                strMess = ""
                If ckCambioPeriodo(txtDataDal.Text, strMess) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                'Periodo in cui è da spostare
                Dim NewDNRiga As String = Session("NewDNRiga")
                If (NewDNRiga Is Nothing) Then
                    NewDNRiga = ""
                End If
                If String.IsNullOrEmpty(NewDNRiga) Then
                    NewDNRiga = ""
                End If
                If NewDNRiga.Trim = "" Then
                    strMess += "Non è stato trovato il Periodo di destinazione<br>"
                End If
                '-
                Dim NewDNRigaDes As String = Session("NewDNRigaDes")
                If String.IsNullOrEmpty(NewDNRigaDes) Then
                    NewDNRigaDes = ""
                End If
                If NewDNRigaDes.Trim = "" Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                End If
                If strMess.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                '----------------------------
                strMess = "Sicuro di voler procedere al cambio Periodo: <br><b>" + lblAPartireDal.Text + " " + txtDataDal.Text.Trim + "</b> ? <br>" +
                "Periodo trovato: " + NewDNRiga.Trim + " - " + NewDNRigaDes
                Session(MODALPOPUP_CALLBACK_METHOD) = "OKCambioPeriodoData"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_CONFIRM_YN)
            End If
        Else 'SOSTITUZIONE N° SERIE
            If DDLDurNumRIga.SelectedIndex = 0 Then
                strMess += "Nessun 'N°Serie' da Sostituire NON è stato selezionato<br>"
                DDLDurNumRIga.BackColor = SEGNALA_KO
            Else
                DDLDurNumRIga.BackColor = SEGNALA_OK
            End If
            If IsNumeric(txtDataDal.Text.Trim) = True Then
                If CInt(txtDataDal.Text.Trim) > 0 Then
                    txtDataDal.BackColor = SEGNALA_OK
                Else
                    strMess += "'A partire dal' NON è stato definito<br>"
                    txtDataDal.BackColor = SEGNALA_KO
                End If
            ElseIf Not IsDate(txtDataDal.Text.Trim) Then
                strMess += "'A partire dal' NON è stato definito<br>"
                txtDataDal.BackColor = SEGNALA_KO
            Else
                txtDataDal.BackColor = SEGNALA_OK
            End If
            txtSerieNEW.Text = Formatta.FormattaNomeFile(txtSerieNEW.Text.Trim)
            If txtSerieNEW.Text.Trim = "" Then
                strMess += "'Sostituzione N°Serie in' NON è stato definito<br>"
                txtSerieNEW.BackColor = SEGNALA_KO
            Else
                txtSerieNEW.BackColor = SEGNALA_OK
            End If
            txtSerieNEW.Text = txtSerieNEW.Text.ToUpper
            '-Controllo che non esista già
            Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
            NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
            If NSL.Trim = txtSerieNEW.Text.Trim Then
                strMess += "N°Serie già presente<br>"
                txtSerieNEW.BackColor = SEGNALA_KO
            Else
                txtSerieNEW.BackColor = SEGNALA_OK
                Dim saveidx As Integer = DDLDurNumRIga.SelectedIndex
                For i = 0 To DDLDurNumRIga.Items.Count - 1
                    DDLDurNumRIga.SelectedIndex = i
                    If DDLDurNumRIga.SelectedItem.Text.Trim.ToUpper = txtSerieNEW.Text.Trim.ToUpper Then
                        strMess += "N°Serie già presente<br>"
                        txtSerieNEW.BackColor = SEGNALA_KO
                        Exit For
                    End If
                Next
                DDLDurNumRIga.SelectedIndex = saveidx
            End If
            'GIU081223
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            strMess = ""
            Dim strDataDal As String = txtDataDal.Text.Trim
            If IsNumeric(txtDataDal.Text.Trim) = True Then
                If CInt(txtDataDal.Text.Trim) > 0 Then
                    txtDataDal.BackColor = SEGNALA_OK
                    strDataDal = "01/01/" + strDataDal
                Else
                    strMess += "'A partire dal' NON è stato definito<br>"
                    txtDataDal.BackColor = SEGNALA_KO
                End If
            ElseIf Not IsDate(txtDataDal.Text.Trim) Then
                strMess += "'A partire dal' NON è stato definito<br>"
                txtDataDal.BackColor = SEGNALA_KO
            Else
                txtDataDal.BackColor = SEGNALA_OK
            End If
            'GIU201223
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '-
            If ckCambioPeriodo(strDataDal, strMess) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'Periodo in cui è da spostare
            Dim NewDNRiga As String = Session("NewDNRiga")
            If (NewDNRiga Is Nothing) Then
                NewDNRiga = ""
            End If
            If String.IsNullOrEmpty(NewDNRiga) Then
                NewDNRiga = ""
            End If
            If NewDNRiga.Trim = "" Then
                strMess += "Non è stato trovato il Periodo di destinazione<br>"
            End If
            '-
            Dim NewDNRigaDes As String = Session("NewDNRigaDes")
            If String.IsNullOrEmpty(NewDNRigaDes) Then
                NewDNRigaDes = ""
            End If
            If NewDNRigaDes.Trim = "" Then
                strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
            End If
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '----------------------------
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                NSL = DDLDurNumRIga.SelectedItem.Text
                NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
                strMess = "Saranno aggiornate solo le attività ancora da Evadere/Fatturare<br>" +
                "Sicuro di voler procedere alla sostituzione del N°Serie: <b>" + NSL + "</b> in <br>" +
                "<b>" + txtSerieNEW.Text.Trim + "</b> a partire dal <b>" + txtDataDal.Text.Trim + "</b> ? "
                Session(MODALPOPUP_CALLBACK_METHOD) = "OKSostNSerie"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_CONFIRM_YN)
            End If
        End If
    End Sub

    Private Sub chkSostNSerie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSostNSerie.CheckedChanged
        If DDLTipoDettagli.SelectedValue.Trim <> "0" Then
            txtSerieNEW.Enabled = chkSostNSerie.Checked
            txtDataDal.Enabled = chkSostNSerie.Checked
            If lblAPartireDal.Text.Trim = "selezionare l'Attività" Then
                btnSostNSerie.Enabled = False
            Else
                btnSostNSerie.Enabled = chkSostNSerie.Checked
            End If
            ImgDataDal.Enabled = chkSostNSerie.Checked
        Else
            txtSerieNEW.Enabled = chkSostNSerie.Checked
            txtDataDal.Enabled = chkSostNSerie.Checked
            btnSostNSerie.Enabled = chkSostNSerie.Checked
            ImgDataDal.Enabled = chkSostNSerie.Checked
        End If
    End Sub

    Public Function OKSostNSerie() As Boolean
        If Session(SWOP) <> SWOPNESSUNA Then Exit Function
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
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
        SqlUpd_ConTScadPag.CommandText = "[updateSostNSerie_ConTDByIDDoc]"
        SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
        SqlUpd_ConTScadPag.Connection = SqlConn
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerieOrig", System.Data.SqlDbType.NVarChar, 0, "NSerieOrig"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerieNew", System.Data.SqlDbType.NVarChar, 0, "NSerieNew"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@APartireAAAA", System.Data.SqlDbType.NVarChar, 0, "APartireAAAA"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@APartireData", System.Data.SqlDbType.NVarChar, 0, "APartireData"))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        Dim strMess As String = ""
        Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
        NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
        Try
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = CLng(myID)
            SqlUpd_ConTScadPag.Parameters("@NSerieOrig").Value = NSL
            SqlUpd_ConTScadPag.Parameters("@NSerieNew").Value = txtSerieNEW.Text.Trim
            SqlUpd_ConTScadPag.Parameters("@APartireAAAA").Value = ""
            SqlUpd_ConTScadPag.Parameters("@APartireData").Value = ""
            If IsNumeric(txtDataDal.Text.Trim) = True Then
                If CInt(txtDataDal.Text.Trim) > 0 Then
                    txtDataDal.BackColor = SEGNALA_OK
                    SqlUpd_ConTScadPag.Parameters("@APartireAAAA").Value = txtDataDal.Text.Trim

                Else
                    strMess += "'A partire dal' NON è stato definito<br>"
                    txtDataDal.BackColor = SEGNALA_KO
                End If
            ElseIf Not IsDate(txtDataDal.Text.Trim) Then
                strMess += "'A partire dal' NON è stato definito<br>"
                txtDataDal.BackColor = SEGNALA_KO
            Else
                txtDataDal.BackColor = SEGNALA_OK
                SqlUpd_ConTScadPag.Parameters("@APartireData").Value = txtDataDal.Text.Trim
            End If
            If strMess.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess, WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
            'OK
            SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
            SqlUpd_ConTScadPag.ExecuteNonQuery()
            '-
            Dim txtNoteIntervento As String = "Sostituzione N° Serie : " + NSL + " con " + txtSerieNEW.Text.Trim + " dal " + txtDataDal.Text.Trim
            If UpgNoteIntervento(strErrore, txtSerieNEW.Text.Trim, txtNoteIntervento, NSL) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Agg.Note App.(Al termine verificare le NOTE): ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
            '- Scadenzario se è presente il N° di serie, anche qui se non è stato emesso fattura totale cioe' solo con flag evasa
            If UpgScadNSerie(strErrore, txtSerieNEW.Text.Trim, NSL, txtDataDal.Text.Trim) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Agg.Scadenze App.(Al termine verificare le Scadenze/N°Serie): ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If

        Catch exSQL As SqlException
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
            strErrore = exSQL.Message.Trim
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
            strErrore = ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        strMess = "Sostituzione del N°Serie: <b>" + NSL + "</b> in <br>" +
            "<b>" + txtSerieNEW.Text.Trim + "</b> a partire dal <b>" + txtDataDal.Text.Trim + "<br>Effettuata con successo.</b><br>Si prega di verificare i dati appena aggiornati."
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Sostituzione N° Serie", strMess.Trim, WUC_ModalPopup.TYPE_INFO)
        '-
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            'giu220220
            Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
            Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
            Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
            '---------
            BtnSetByStato(Stato)
            DDLTipoDettagli.SelectedIndex = 0
            Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
        End Try
    End Function
    Private Function UpgNoteIntervento(ByRef strErrore As String, ByVal pSerieNEW As String, ByVal txtNoteIntervento As String, ByVal pSerieOLD As String) As Boolean
        UpgNoteIntervento = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            strErrore = "Errore: SESSIONE SCADUTA (ID) - (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        Dim SWOk As Boolean = True
        Dim NSL As String = pSerieNEW
        If String.IsNullOrEmpty(NSL) Then
            NSL = ""
        End If
        If NSL = "" Then
            strErrore = "Errore: Nuovo N° Serie non valido (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        'GIU020322 NOTE PUNTUALI PER N° SERIE LOTTO
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = ""
        Dim myNoteRitiro As String = ""
        strSQL = "Select NoteRitiro From ContrattiT WHERE (IDDocumenti = " + myID.Trim + ")"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myNoteRitiro = IIf(IsDBNull(row.Item("NoteRitiro")), "", row.Item("NoteRitiro"))
                        Exit For
                    Next
                Else
                    'Nennuna App.presente
                End If
            Else
                'Nennuna App.presente
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura Note Intervento: " + Ex.Message.Trim
            Exit Function
        End Try
        '------------------------------------
        Dim ListaSL As New List(Of String)
        Dim mySL As String = NSL
        Dim OKNoteRitiro As String = ""
        txtNoteIntervento.Trim.Replace("§", " ")
        Dim StrDato() As String
        Try
            Call GetNoteSL(myNoteRitiro.Trim, mySL.Trim)
            ListaSL = Session(L_NSERIELOTTO)
            '-------------------------------------------
            Dim SWTrovatoNSL As Boolean = False
            If ListaSL.Count > 0 Then
                OKNoteRitiro = ""
                SWTrovatoNSL = False
                For I = 0 To ListaSL.Count - 1
                    StrDato = ListaSL(I).Split("§")
                    mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                    myNoteRitiro = StrDato(1)
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    If mySL = NSL Then
                        SWTrovatoNSL = True
                        OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                    ElseIf mySL = pSerieOLD Then
                        OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                    Else
                        OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                    End If
                Next
                If SWTrovatoNSL = False Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    OKNoteRitiro += NSL + "§" + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                End If
            Else
                If SetNoteSLALLApp(myNoteRitiro.Trim, strErrore) = False Then
                    Exit Function
                End If
                ListaSL = Session(L_NSERIELOTTO)
                SWTrovatoNSL = False
                If ListaSL.Count > 0 Then
                    OKNoteRitiro = ""
                    For I = 0 To ListaSL.Count - 1
                        StrDato = ListaSL(I).Split("§")
                        mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                        myNoteRitiro = StrDato(1)
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        If mySL = NSL Then
                            SWTrovatoNSL = True
                            OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                        ElseIf mySL = pSerieOLD Then
                            OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                        Else
                            OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                        End If
                    Next
                    If SWTrovatoNSL = False Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        OKNoteRitiro += NSL + "§" + " - " + Format(Now, "dddd d MMMM yyyy, HH:mm") + " - " + txtNoteIntervento.Trim
                    End If
                Else
                    OKNoteRitiro = NSL + "§" + txtNoteIntervento.Trim
                End If
            End If
        Catch ex As Exception
            strErrore = "Errore: Aggiorna Note Intervento (UpgNoteIntervento): " + ex.Message.Trim
            Exit Function
        End Try
        '---------------------------------------------
        Dim myErrore As String = ""
        Try
            'giu040324 corretto errore ''''''' aggiungeva ad ogni agg un apice in piu tolto il contolla_apice
            SWOk = ObjDB.ExecUpgNoteRitiro(myID.Trim, OKNoteRitiro.Trim, myErrore)
            If SWOk = False Then
                strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento)"
                Exit Function
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento) - " & Ex.Message.Trim & " -  " & myErrore
            Exit Function
        Finally
            ObjDB = Nothing
        End Try
        UpgNoteIntervento = True
    End Function
    Private Function GetNoteSL(ByVal pNoteRitiro As String, ByVal pNSL As String) As String
        GetNoteSL = ""
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        Session(L_NSERIELOTTO) = Nothing
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        Dim SWTrovato As Boolean = False 'giu301123 se non è presente nella lista note l'ho inserisco
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I)) 'giu070523
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
                If mySL.Trim = pNSL.Trim Then
                    GetNoteSL = myNoteRitiro.Trim
                    SWTrovato = True
                End If
            Next
            If SWTrovato = False Then
                ListaSL.Add(pNSL.Trim + "§" + "")
            End If
            Session(L_NSERIELOTTO) = ListaSL
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            Dim strErrore As String = ""
            Call SetNoteSLALLApp(pNoteRitiro, strErrore)
            GetNoteSL = pNoteRitiro.Trim
        End If
    End Function
    Private Function SetNoteSLALLApp(ByVal pNoteRitiro As String, ByRef strErrore As String) As Boolean
        SetNoteSLALLApp = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Serie,'') + ISNULL(Lotto,'') AS SerieLotto From ContrattiD " &
                 "WHERE (IDDocumenti = " + myID.Trim + ") AND (DurataNum =0) " &
                 "GROUP BY ISNULL(Serie,'') + ISNULL(Lotto,'')"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim ListaSL As New List(Of String)
        Dim mySL As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        mySL = Formatta.FormattaNomeFile(IIf(IsDBNull(row.Item("SerieLotto")), "", row.Item("SerieLotto"))) 'giu070523
                        If mySL.Trim <> "" Then
                            ListaSL.Add(mySL + "§" + pNoteRitiro.Trim)
                        End If
                    Next
                Else
                    'per adesso proseguo
                    strErrore = "Errore: Assegna Note Intervento(SetNoteSLALLApp): Nennuna App.presente"
                    Exit Function
                End If
            Else
                'per adesso proseguo
                strErrore = "Errore: Assegna Note Intervento(SetNoteSLALLApp): Nennuna App.presente"
                Exit Function
            End If
        Catch Ex As Exception
            'per adesso proseguo
            strErrore = "Errore: Assegna Note Intervento(SetNoteSLALLApp): " & Ex.Message
            Exit Function
        End Try
        SetNoteSLALLApp = True
        Session(L_NSERIELOTTO) = ListaSL

    End Function
    Private Function UpgScadNSerie(ByRef strErrore As String, ByVal pSerieNEW As String, ByVal pSerieOLD As String, ByVal pDataDal As String) As Boolean
        UpgScadNSerie = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            strErrore = "Errore: SESSIONE SCADUTA (ID) - (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        Dim SWOk As Boolean = True
        Dim NSL As String = pSerieNEW
        If String.IsNullOrEmpty(NSL) Then
            NSL = ""
        End If
        If NSL = "" Then
            strErrore = "Errore: Nuovo N° Serie non valido (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        'GIU020322 NOTE PUNTUALI PER N° SERIE LOTTO
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = ""
        Dim myScadPagCA As String = ""
        strSQL = "Select ScadPagCA From ContrattiT WHERE (IDDocumenti = " + myID.Trim + ")"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myScadPagCA = IIf(IsDBNull(row.Item("ScadPagCA")), "", row.Item("ScadPagCA"))
                        Exit For
                    Next
                Else
                    'Nennuna App.presente
                End If
            Else
                'Nennuna App.presente
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura Scadenze: " + Ex.Message.Trim
            Exit Function
        End Try
        '-----------------------------------
        Dim DalDataAAAA As String = ""
        Dim DalDataGGMMAAA As String = ""
        If IsNumeric(pDataDal.Trim) = True Then
            If CInt(pDataDal.Trim) > 0 Then
                DalDataAAAA = pDataDal.Trim
            Else
                strErrore = "Errore: A partire dal' NON è stato definito"
                Exit Function
            End If
        ElseIf Not IsDate(txtDataDal.Text.Trim) Then
            strErrore = "Errore: A partire dal' NON è stato definito"
            Exit Function
        Else
            DalDataGGMMAAA = pDataDal.Trim
        End If
        Dim NRate As Integer = 0
        Dim ArrScadPagCA As ArrayList
        Try
            ArrScadPagCA = New ArrayList
            Dim myScadCA As ScadPagCAEntity
            If myScadPagCA <> "" Then
                Dim lineaSplit As String() = myScadPagCA.Split(";")
                For i = 0 To lineaSplit.Count - 1
                    If lineaSplit(i).Trim <> "" And (i + 8) <= lineaSplit.Count - 1 Then 'giu191223 da i + 6

                        myScadCA = New ScadPagCAEntity
                        myScadCA.NRata = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Data = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Importo = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Evasa = lineaSplit(i).Trim
                        i += 1
                        myScadCA.NFC = lineaSplit(i).Trim
                        i += 1
                        myScadCA.DataFC = lineaSplit(i).Trim
                        i += 1
                        If myScadCA.Evasa Then
                            myScadCA.Serie = lineaSplit(i).Trim
                        Else
                            SWOk = False
                            If DalDataAAAA.Trim <> "" Then
                                If CDate(myScadCA.Data).Year >= CInt(DalDataAAAA) Then
                                    SWOk = True
                                End If
                            ElseIf IsDate(DalDataGGMMAAA.Trim) And DalDataGGMMAAA.Trim <> "" Then
                                If CDate(myScadCA.Data) >= CDate(DalDataGGMMAAA) Then
                                    SWOk = True
                                End If
                            End If
                            If SWOk = True Then
                                If lineaSplit(i).Trim = pSerieOLD Then
                                    myScadCA.Serie = pSerieNEW.Trim
                                Else
                                    myScadCA.Serie = lineaSplit(i).Trim
                                End If
                            Else
                                myScadCA.Serie = lineaSplit(i).Trim
                            End If
                        End If
                        i += 1
                        myScadCA.ImportoF = lineaSplit(i).Trim
                        i += 1
                        myScadCA.ImportoR = lineaSplit(i).Trim
                        ArrScadPagCA.Add(myScadCA)
                        NRate += 1
                    End If
                Next
            End If
        Catch ex As Exception
            strErrore = "Errore: Carico Scadenze: " + ex.Message.Trim
            Exit Function
        End Try
        Dim OKScadPagCA As String = ""
        Dim rowScadPagCa As ScadPagCAEntity = Nothing
        For i = 0 To ArrScadPagCA.Count - 1
            If OKScadPagCA.Trim <> "" Then OKScadPagCA += ";"
            rowScadPagCa = ArrScadPagCA(i)
            OKScadPagCA += rowScadPagCa.NRata.Trim & ";"
            OKScadPagCA += rowScadPagCa.Data.Trim & ";"
            OKScadPagCA += rowScadPagCa.Importo.Trim & ";"
            OKScadPagCA += rowScadPagCa.Evasa.ToString.Trim & ";"
            OKScadPagCA += rowScadPagCa.NFC.Trim & ";"
            OKScadPagCA += rowScadPagCa.DataFC.Trim & ";"
            OKScadPagCA += rowScadPagCa.Serie.Trim + ";"
            OKScadPagCA += rowScadPagCa.ImportoF.Trim & ";" 'giu191223
            OKScadPagCA += rowScadPagCa.ImportoR.Trim
        Next
        'OK AGGIORNO
        Try
            strErrore = ""
            If UPGScadPagCA(myID.Trim, ArrScadPagCA, OKScadPagCA, strErrore) = False Then
                strErrore = "Errore: Aggiorna Scadenze: " + strErrore.Trim
                Exit Function
            End If
        Catch ex As Exception
            strErrore = "Errore: Aggiorna Scadenze: " + ex.Message.Trim
            Exit Function
        End Try
        'OK FINE
        UpgScadNSerie = True
    End Function
    Private Function UPGScadPagCA(ByVal _myID As String, ByVal _ArrScadPag As ArrayList, ByVal _ScadPagCA As String, ByRef strErrore As String) As Boolean
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
            SqlUpd_ConTScadPag.CommandText = "[Update_ConTScadPagCAByIDDoc]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_1"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_1", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_2"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_2", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_3"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_3", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_4"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_4", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_5"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_5", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadPagCA"))
            '
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = _myID
            'giu040520
            Dim rowScadPagCa As ScadPagCAEntity = Nothing
            Dim NScad As Integer = 0
            For i = 0 To _ArrScadPag.Count - 1
                rowScadPagCa = _ArrScadPag(i)
                If rowScadPagCa.Evasa = False And rowScadPagCa.Data.Trim <> "" Then
                    NScad += 1
                    If NScad < 6 Then
                        If IsDate(rowScadPagCa.Data.Trim) Then
                            SqlUpd_ConTScadPag.Parameters("@Data_Scadenza_" & Format(NScad, "#0") & "").Value = CDate(rowScadPagCa.Data)
                        Else
                            'non passonulla ...defaultNULL
                        End If
                        If rowScadPagCa.Importo.Trim = "" Or Not IsNumeric(rowScadPagCa.Importo.Trim) Then
                            SqlUpd_ConTScadPag.Parameters("@Rata_" & Format(NScad, "#0") & "").Value = 0
                        Else
                            SqlUpd_ConTScadPag.Parameters("@Rata_" & Format(NScad, "#0") & "").Value = CDec(rowScadPagCa.Importo.Trim)
                        End If
                    Else
                        Exit For
                    End If

                End If
            Next
            '---------------
            SqlUpd_ConTScadPag.Parameters("@ScadPagCA").Value = IIf(_ScadPagCA.Trim = "", "", _ScadPagCA.Trim)
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

    'giu011223
    Private Function ckCambioPeriodo(ByVal pDataNEW As String, ByRef strMess As String) As Boolean
        ckCambioPeriodo = False
        Dim myDurataNum As String = Session(CSTDURATANUM)
        If IsNothing(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        ElseIf Not IsNumeric(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        ElseIf Val(myDurataNum) = 0 Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        End If
        '-
        Dim myDurataTipo As String = Session(CSTDURATATIPO)
        If IsNothing(myDurataTipo) Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataTipo) Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        ElseIf myDurataTipo.Trim = "" Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        End If
        '-
        Dim myDataInizio As String = Session(CSTDATAINIZIO)
        If IsNothing(myDataInizio) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDataInizio) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        ElseIf Not IsDate(myDataInizio.Trim) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        End If
        '----------
        Dim NewDNRiga As Integer = -1
        Dim NewDNRigaDes As String = ""
        If myDurataTipo.Trim = "M" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "T" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "Q" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "S" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "A" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio).Year, "0000")
                If CDate(pDataNEW).Year >= CInt(NewDNRigaDes) And CDate(pDataNEW).Year <= CDate(myDataInizio).Year Then
                    NewDNRiga = i
                    Exit For
                ElseIf CDate(pDataNEW).Year < CInt(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
            Next i
        Else 'non capitera' mai
            strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
            Session("NewDNRiga") = ""
            Session("NewDNRigaDes") = ""
            Exit Function
        End If
        If NewDNRiga = -1 Then
            strMess += "Non è stato trovato il Periodo di destinazione OPPURE non compresa nelle date di Inizio/Fine Contratto<br>"
            Session("NewDNRiga") = ""
            Session("NewDNRigaDes") = ""
            Exit Function
        End If
        Session("NewDNRiga") = NewDNRiga
        Session("NewDNRigaDes") = NewDNRigaDes
        ckCambioPeriodo = True
    End Function

    Public Function OKCambioPeriodoData() As Boolean
        If Session(SWOP) <> SWOPNESSUNA Then Exit Function
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        '-Riga
        Dim strMess As String = ""
        Dim myData As String = Session("DNRRigaDATA")
        If (myData Is Nothing) Then
            myData = ""
        End If
        Dim myDNRRiga = Session("DNRRiga")
        If String.IsNullOrEmpty(myData) Then
            myData = ""
        End If
        If myData.Trim = "" Then
            strMess += "Selezionare l'Attività<br>"
        Else 'Periodo OLD da cambiare
            If (myDNRRiga Is Nothing) Then
                myDNRRiga = ""
            End If
            If String.IsNullOrEmpty(myDNRRiga) Then
                myDNRRiga = ""
            End If
            If myDNRRiga.Trim = "" Then
                strMess += "Selezionare l'Attività<br>"
            End If
        End If
        If Not IsDate(txtDataDal.Text.Trim) Then
            strMess += "Selezionare la nuova data dell'Attività<br>"
        End If
        'Periodo in cui è da spostare
        Dim NewDNRiga As String = Session("NewDNRiga")
        Dim NewDNRigaDes As String = Session("NewDNRigaDes")
        If (NewDNRiga Is Nothing) Then
            NewDNRiga = ""
        End If
        If String.IsNullOrEmpty(NewDNRiga) Then
            NewDNRiga = ""
        End If
        If NewDNRiga.Trim = "" Then
            strMess += "Non è stato trovato il Periodo di destinazione<br>"
        End If
        '----------------------------
        If strMess.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        Else
            'ok procedo
        End If
        '-----
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
        '---------------------------
        strErrore = ""
        Try
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '--- Parametri
            SqlUpd_ConTScadPag.CommandText = "[Update_ConDCambioPeriodo]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, "DurataNumRiga"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, "Riga"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRigaNEW", System.Data.SqlDbType.Int, 4, "DurataNumRigaNEW"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNEW", System.Data.SqlDbType.NVarChar, 10, "DataNEW"))
            '
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = myID
            '
            Dim pDurataNumRiga As String = "" : Dim pRiga As String = ""
            If myDNRRiga.Trim = "" Then
                strMess += "Attenzione, non è stata Selezionata l'Attività<br>(pDurataNumRiga) 1"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
            pDurataNumRiga = Trim(Mid(myDNRRiga, 1, InStr(myDNRRiga, ",") - 1))
            If pDurataNumRiga.Trim = "" Then
                strMess += "Attenzione, non è stata Selezionata l'Attività<br>(pDurataNumRiga) 2"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
            SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = CInt(pDurataNumRiga)
            pRiga = Trim(Mid(myDNRRiga, InStr(myDNRRiga, ",") + 1))
            If pRiga.Trim = "" Then
                strMess += "Attenzione, non è stata Selezionata l'Attività<br>(pRiga)"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
            SqlUpd_ConTScadPag.Parameters("@Riga").Value = CInt(pRiga)
            '-
            SqlUpd_ConTScadPag.Parameters("@DurataNumRigaNEW").Value = CInt(NewDNRiga)
            SqlUpd_ConTScadPag.Parameters("@DataNEW").Value = txtDataDal.Text.Trim
            'OK
            SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
            SqlUpd_ConTScadPag.ExecuteNonQuery()
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            strMess += "Errore, OKCambioPeriodoData<br>" + strErrore
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            strMess += "Errore, OKCambioPeriodoData<br>" + strErrore
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
            Return False
        Finally
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
        End Try
        '-
        LnkStampa.Visible = False : LnkVerbale.Visible = False
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            'giu220220
            Session(CSTDURATANUM) = row.Cells(CellIdxT.DurataNum).Text.Trim
            Session(CSTDURATATIPO) = row.Cells(CellIdxT.DurataTipo).Text.Trim
            Session(CSTDATAINIZIO) = row.Cells(CellIdxT.DataInizio).Text.Trim
            '---------
            BtnSetByStato(Stato)
            '''DDLTipoDettagli.SelectedIndex = 0
            '''Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
            Try
                DDLDurNumRIga.SelectedIndex = 0
                DDLDurNumRIga_SelectedIndexChanged(Nothing, Nothing)
            Catch ex As Exception
                DDLTipoDettagli.SelectedIndex = 0
                Call DDLTipoDettagli_SelectedIndexChanged(Nothing, Nothing)
            End Try

        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
        End Try
        'ok fatto
        strMess = "Cambio Periodo: dalla data <b>" + myData + " in " + txtDataDal.Text.Trim + "</b>" +
            "<br>Effettuato con successo.</b><br>Si prega di verificare i dati appena aggiornati."
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Cambio Periodo/Data", strMess.Trim, WUC_ModalPopup.TYPE_INFO)
        OKCambioPeriodoData = True
    End Function

End Class