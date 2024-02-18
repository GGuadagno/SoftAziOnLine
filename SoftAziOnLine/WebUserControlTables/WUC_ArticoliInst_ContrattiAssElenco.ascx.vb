Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
'GIU250514 NUOVO METODO LENTO PER IL BINARIO OK PER IL PDF
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
'---------
Partial Public Class WUC_ArticoliInst_ContrattiAssElenco
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private aDataViewPrevT1 As DataView

    Private Enum CellIdxT
        CodCoGe = 1
        RagSoc = 2
        Denom = 3
        CodArt = 4
        DesArt = 5
        DataInst = 6
        NSerieLotto = 7
        DataScadEl = 8
        DataScadBa = 9
        Attivo = 10
        Sostituito = 11
        NReInvio = 12
        DataInvio = 13
        DataScadGaranzia = 14
        Loc = 15
        CAP = 16
        Riferimento = 17
        Cod_Filiale = 18
        Destinazione1 = 19
        Destinazione2 = 20
        Destinazione3 = 21
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Attesa.WucElement = Me 'giu280315
        'giu031219 spostato prima di tutto 
        ModalPopup.WucElement = Me
        WUC_StampaElencoAI1.WucElement = Me
        WFP_ArticoliInstEmail.WucElement = Me
        WFP_ElencoEmail.WucElement = Me
        WUC_Anagrafiche_ModifySint1.WucElement = Me
        WFPDocCollegati.WucElement = Me
        'sim050618  - resi invisibili secondo specifiche richieste
        btnNuovo.Visible = False
        BtnNuovoCopia.Visible = False
        btnElimina.Visible = False
        '----------------------------------
        'caricamento grid default--Sim010618
        If Not Session("aDataViewPrevT1") Is Nothing Then
            aDataViewPrevT1 = Session("aDataViewPrevT1")
        Else
            lblDescrFiltri.Text = ""
            If aDataViewPrevT1 Is Nothing Then
                aDataViewPrevT1 = New DataView
            End If
        End If
        Session("aDataViewPrevT1") = aDataViewPrevT1
        GridViewPrevT.DataSource = aDataViewPrevT1
        '---

        'sim040618-- caricamento label filtri per visualizzazione dettaglio
        lblDescrFiltri.Text = Session("DescFiltri")
        If lblDescrFiltri.Text.Trim = "" Then
            btnCancFiltro.Visible = False
        Else
            btnCancFiltro.Visible = True
        End If
        '------------------------------------------------------------------
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
        btnRicarcaAll.Visible = False
        btnAggiornaScad.Visible = False
        'btnEspPDF.Visible = True
        If (sTipoUtente.Equals(CSTTECNICO)) Then
            btnRicarcaAll.Visible = True
            btnAggiornaScad.Visible = True
        ElseIf (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            btnAggiornaScad.Visible = True
        End If
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnRicarcaAll.Visible = False
            btnAggiornaScad.Visible = False
            btnNuovo.Visible = False
            BtnNuovoCopia.Visible = False
            btnElimina.Visible = False
            btnPreparaEmail.Visible = False
            btnModifica.Visible = False
            btnModificaAnagrafica.Visible = False
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
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCliForFilProvv.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Dim ClsDB As New DataBaseUtility
        Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
        ClsDB = Nothing
        lblLogInvioEmail.Text = LogInvioEmail
        If lblLogInvioEmail.Text.Trim = "SERVIZIO INVIO EMAIL SOSPESO" Or Mid(lblLogInvioEmail.Text.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            lblLogInvioEmail.BackColor = SEGNALA_KO
        Else
            lblLogInvioEmail.BackColor = SEGNALA_OKLBL
        End If
        If (Not IsPostBack) Then
            Try
                SEGNALA_OKBTN = btnRicerca.BackColor
                lblDescrFiltri.ForeColor = Drawing.Color.Blue
                lblInfoRicerca.ForeColor = Drawing.Color.Blue
                lblLogInvioEmail.ForeColor = Drawing.Color.Blue
                btnRicarcaAll.Text = "Ricarica dati da movimenti"
                btnAggiornaScad.Text = "Aggiorna date Scadenze"
                btnNuovo.Text = "Nuova Scadenza"
                BtnNuovoCopia.Text = "Nuovo (Copia dati)"
                btnStampaSingolo.Text = "Stampa scheda Articolo"
                btnPreparaEmail.Text = "Prepara E-mail Scadenze"
                btnElencoEmail.Text = "E-mail inviate"
                'btnEspPDF.Text = "Esporta elenco Scadenze PDF"
                btnModificaAnagrafica.Text = "Modifica E-mail"
                '-----
                Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
                If IsNothing(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If String.IsNullOrEmpty(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If SWRbtnTD = "" Then
                    rbtnTutti.AutoPostBack = False
                    rbtnTutti.Checked = True
                    rbtnTutti.AutoPostBack = True
                End If
                Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
                Session(CSTTIPODOCSEL) = SWTD(TD.ArticoloInstallato)
                'giu020514 non usato Session(CSTSTATODOC) = "999"
                If SWRbtnTD.Equals("rbtnAttivo") Then
                    rbtnAttivo.AutoPostBack = False
                    rbtnAttivo.Checked = True
                    rbtnAttivo.AutoPostBack = True
                ElseIf SWRbtnTD.Equals("rbtnDismesso") Then
                    rbtnDismesso.Checked = True
                ElseIf SWRbtnTD.Equals("rbtnInRiparazione") Then
                    rbtnInRiparazione.AutoPostBack = False
                    rbtnInRiparazione.Checked = True
                    rbtnInRiparazione.AutoPostBack = True
                ElseIf SWRbtnTD.Equals("rbtnInviataEmail") Then
                    rbtnInviataEmail.AutoPostBack = False
                    rbtnInviataEmail.Checked = True
                    rbtnInviataEmail.AutoPostBack = True
                ElseIf SWRbtnTD.Equals("rbtnSostituito") Then
                    rbtnSostituito.AutoPostBack = False
                    rbtnSostituito.Checked = True
                    rbtnSostituito.AutoPostBack = True
                ElseIf SWRbtnTD.Equals("rbtnConScadenze") Then
                    rbtnConScadenze.AutoPostBack = False
                    rbtnConScadenze.Checked = True
                    rbtnConScadenze.AutoPostBack = True
                ElseIf SWRbtnTD.Equals("rbtnTutti") Then
                    rbtnTutti.AutoPostBack = False
                    rbtnTutti.Checked = True
                    rbtnTutti.AutoPostBack = True
                Else
                    rbtnTutti.AutoPostBack = False
                    rbtnTutti.Checked = True
                    rbtnTutti.AutoPostBack = True
                End If

                ddlRicerca.Items.Add("Codice Articolo")
                ddlRicerca.Items(0).Value = "A" 'Cod_Articolo
                ddlRicerca.Items.Add("Descrizione Articolo")
                ddlRicerca.Items(1).Value = "DA" 'Cod_Articolo
                ddlRicerca.Items.Add("N° Serie")
                ddlRicerca.Items(2).Value = "NS" 'NSerie
                '-

                ddlRicerca.Items.Add("Data Invio(1/2) E-Mail")
                ddlRicerca.Items(3).Value = "V" 'Data1Invio Data2Invio
                'giu010514
                ddlRicerca.Items.Add("Data scadenza elettrodi")
                ddlRicerca.Items(4).Value = "SE"
                ddlRicerca.Items.Add("Data scadenza batterie")
                ddlRicerca.Items(5).Value = "SB"
                ddlRicerca.Items.Add("Data scadenza garanzia")
                ddlRicerca.Items(6).Value = "C" 'Data_Scadenza_Garanzia
                '-
                ' ''ddlRicerca.Items.Add("Progressivo contratto")
                ' ''ddlRicerca.Items(7).Value = "N" 'Numero
                ddlRicerca.Items.Add("Data movimento")
                ddlRicerca.Items(7).Value = "D" 'Data_Installazione
                '-
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(8).Value = "R" 'Rag_Soc
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(9).Value = "S" 'Denominazione
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(10).Value = "P" 'Partita_IVA
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(11).Value = "F" 'Codice_Fiscale
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(12).Value = "L" 'Localita
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(13).Value = "M" 'CAP
                '-
                ddlRicerca.Items.Add("Riferimenti documento")
                ddlRicerca.Items(14).Value = "RI" 'Riferimenti: NsRiferimento + Riferimento
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(15).Value = "CG" 'Cod_Coge

                'simone220518
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(16).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(17).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(18).Value = "D3"

                BuidDett()
                Session(SWOP) = SWOPNESSUNA

                'simone300518:  sessione apri popup = true (all'avvio)
                Session(F_VISUALIZZADETT_APERTA) = True
                Session(F_ARTINSTEMAIL_APERTA) = False
                Session(F_EMAILINVIATE_APERTA) = False
                Session(F_ARTINSTEMAIL_EMAILINVIATE) = String.Empty
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Articoli installati: " & Ex.Message)
                Exit Sub
            End Try
        End If
        If ddlRicerca.SelectedValue = "D" Or _
           ddlRicerca.SelectedValue = "V" Or _
           ddlRicerca.SelectedValue = "C" Or _
           ddlRicerca.SelectedValue = "SE" Or _
           ddlRicerca.SelectedValue = "SB" Then
            checkParoleContenute.Text = "Intero mese"
        Else
            checkParoleContenute.Text = "Parole contenute"
        End If
        'se non sono in visualizzazione dettaglio allora avvio popup testate email
        If Session(F_VISUALIZZADETT_APERTA) = False Then
            If Session(F_ARTINSTEMAIL_APERTA) = True Then
                WFP_ArticoliInstEmail.Show()
            ElseIf Session(F_EMAILINVIATE_APERTA) = True Then
                WFP_ElencoEmail.Show()
            End If
        ElseIf Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If

    End Sub

    '-giu210514 modifica anagrafica clienti per l'email ed altro
    Private Sub btnModificaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaAnagrafica.Click

        Session(CSTTABCLIFOR) = TabCliFor
        Session(TIPORK) = "C"
        '-
        Dim myIDCOGE As String = Session(CSTCODCOGE)
        If IsNothing(myIDCOGE) Then
            myIDCOGE = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna anagrafica selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myIDCOGE) Then
            myIDCOGE = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna anagrafica selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myIDCOGE.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice anagrafica non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Session(CSTTABCLIFOR) = TabCliFor
        If Left(myIDCOGE.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            Session(TIPORK) = "C"
            Session(CSTTABCLIFOR) = "Cli"
        ElseIf Left(myIDCOGE.Trim, 1) = "9" Then
            TabCliFor = "For"
            Session(TIPORK) = "F"
            Session(CSTTABCLIFOR) = "For"
        End If
        SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = '" & myIDCOGE.Trim & "')"
        '----------
        Dim dvCliFor As DataView
        dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
        If (dvCliFor Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(IDANAGRCLIFOR) = myIDCOGE.Trim
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
            Rk.EMailInvioScad = dvCliFor.Item(0).Item("EmailInvioScad").ToString.Trim 'sim180518
            Rk.InvioMailScad = dvCliFor.Item(0).Item("InvioMailScad")
            Rk.PECEMail = dvCliFor.Item(0).Item("PECEMail").ToString.Trim 'giu190122
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-ok trovato
        'BLOCCO PER LA DESTINAZIONE CLIENTE --SIMONE210518
        Try
            Dim Progressivo As String = Session(CSTCODFILIALE) 'giu190518 GridViewPrevT.SelectedRow.Cells(CellIdxT.Cod_Filiale).Text.Trim 'CodiceFiliale
            If IsNumeric(Progressivo) = True And IsNumeric(myIDCOGE) = True And Progressivo <> "" And myIDCOGE <> "" Then
                SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [DestClienti] WHERE ([Codice] = '" & myIDCOGE & "') AND ([Progressivo] = " & Progressivo & ")"

                Dim Rk1 As New DestCliForEntity
                Dim dvDestCli As DataView
                dvDestCli = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)

                If Not (dvDestCli Is Nothing) Then
                    If dvDestCli.Count > 0 Then
                        Rk1.Progressivo = dvDestCli.Item(0).Item("Progressivo").ToString
                        Rk1.Ragione_Sociale = dvDestCli.Item(0).Item("Ragione_Sociale").ToString
                        Rk1.Denominazione = dvDestCli.Item(0).Item("Denominazione").ToString
                        Rk1.Riferimento = dvDestCli.Item(0).Item("Riferimento").ToString
                        Rk1.Indirizzo = dvDestCli.Item(0).Item("Indirizzo").ToString
                        Rk1.Cap = dvDestCli.Item(0).Item("CAP").ToString
                        Rk1.Localita = dvDestCli.Item(0).Item("Localita").ToString
                        Rk1.Provincia = dvDestCli.Item(0).Item("Provincia").ToString.Trim
                        Rk1.Stato = dvDestCli.Item(0).Item("Stato").ToString.Trim
                        Rk1.Telefono1 = dvDestCli.Item(0).Item("Telefono1").ToString.Trim
                        Rk1.Telefono2 = dvDestCli.Item(0).Item("Telefono2").ToString.Trim
                        Rk1.Fax = dvDestCli.Item(0).Item("Fax").ToString.Trim
                        Rk1.EMail = dvDestCli.Item(0).Item("EMail").ToString.Trim

                        Session(RKANAGRDESTCLI) = Rk1
                    Else
                        'nessun dato trovato
                        Session(RKANAGRDESTCLI) = Nothing
                    End If
                Else
                    Session(RKANAGRDESTCLI) = Nothing
                End If
            Else
                Session(RKANAGRDESTCLI) = Nothing
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Filiale non trovata in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '----
        btnRicerca.BackColor = SEGNALA_KO
        Session(RKANAGRCLIFOR) = Rk
        WUC_Anagrafiche_ModifySint1.PopolaCampi()
        WUC_Anagrafiche_ModifySint1.Show()
    End Sub
    '-
    Private Sub BtnSetByStato(ByVal myStato As String)
        BtnSetEnabledTo(True)
        '
        ' ''Dim SWBloccoModifica As Boolean = False
        ' ''Dim SWBloccoElimina As Boolean = False
        ' ''Dim SWBloccoCreaFT As Boolean = False
        '---------
        ' ''If myStato.Trim = "NON Fatturabile" Then
        ' ''    btnCreaFattura.Enabled = False : SWBloccoCreaFT = True
        ' ''End If
        '' ''GIU230412
        ' ''If myStato.Trim = "C/Visione" Then
        ' ''    btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''End If
        ' ''If myStato.Trim = "C/Deposito" Then
        ' ''    btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''End If
        '---------
        ' ''If myStato.Trim = "Fatturato" Then
        ' ''    btnModifica.Enabled = False : SWBloccoModifica = True
        ' ''    btnElimina.Enabled = False : SWBloccoElimina = True
        ' ''    ' ''btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''End If
        ' ''If myStato.Trim = "OK trasf.in CoGe" Then
        ' ''    btnModifica.Enabled = False : SWBloccoModifica = True
        ' ''    btnElimina.Enabled = False : SWBloccoElimina = True
        ' ''    ' ''btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
        ' ''    btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''    btnListaCarico.Enabled = False
        ' ''    btnStampaEti.Enabled = False
        ' ''End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
        ' ''    btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''    btnListaCarico.Enabled = False
        ' ''    btnStampaEti.Enabled = False
        ' ''End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
        ' ''    btnCreaFattura.Enabled = False 'giu240412: SWBloccoCreaFT = True
        ' ''    btnListaCarico.Enabled = False
        ' ''    btnStampaEti.Enabled = False
        ' ''End If
        ' ''If SWBloccoCreaFT Or SWBloccoElimina Or SWBloccoModifica Then
        ' ''    btnSblocca.Visible = True
        ' ''    If SWBloccoCreaFT = True And SWBloccoElimina = False And SWBloccoModifica = False Then
        ' ''        If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Or _
        ' ''            Session(CSTTIPODOCSEL) = SWTD(TD.FatturaAccompagnatoria) Then
        ' ''            btnSblocca.Visible = False
        ' ''        End If
        ' ''    End If
        ' ''End If

    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnModificaAnagrafica.Enabled = Valore
        btnNuovo.Enabled = Valore : BtnNuovoCopia.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        'btnStampaElenco.Enabled = Valore
        btnStampaSingolo.Enabled = Valore
        btnVisualizza.Enabled = Valore  'simone220518
        btnPreparaEmail.Enabled = Valore
        If Valore = True Then
            If GridViewPrevT.Rows.Count > 0 And rbtnTutti.Checked = True Then
                If Not IsNothing(Session("DescFiltri")) Then
                    If String.IsNullOrEmpty(Session("DescFiltri")) Then
                        btnPreparaEmail.Enabled = False
                    End If
                End If
                If lblDescrFiltri.Text.Trim <> "" Then
                    btnPreparaEmail.Enabled = True
                End If
            Else
                btnPreparaEmail.Enabled = False
            End If
        Else
            btnPreparaEmail.Enabled = False
        End If
        If Not IsNothing(Session("TIPOFiltri")) Then
            If Not String.IsNullOrEmpty(Session("TIPOFiltri")) Then
                If Session("TIPOFiltri") = "EMAIL" Then
                    btnPreparaEmail.Enabled = False
                End If
            End If
        End If
        If lblDescrFiltri.Text.Trim = "" Then
            btnCancFiltro.Visible = False
        Else
            btnCancFiltro.Visible = True
        End If
    End Sub


#Region "Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        aDataViewPrevT1 = Session("aDataViewPrevT1")
        GridViewPrevT.DataSource = aDataViewPrevT1
        GridViewPrevT.DataBind()
        Session(IDARTICOLOINST) = ""
        Session(CSTCODCOGE) = ""
        Session(CSTCODFILIALE) = ""
        '-
        SetRicerca()
        SetFilter()
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDARTICOLOINST) = ""
        Session(CSTCODCOGE) = ""
        Session(CSTCODFILIALE) = ""
        '-
        aDataViewPrevT1 = Session("aDataViewPrevT1")
        GridViewPrevT.DataSource = aDataViewPrevT1
        GridViewPrevT.DataBind()
        '-
        SetRicerca()
        SetFilter() 'GIU160412
        'giu110412
        GridViewPrevT.DataBind()
        ' ''GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = "" 'row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
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
            ViewState("_GridView1LastSortDirection_") = SortDirection
            ViewState("_GridView1LastSortExpression_") = sortExpression
            aDataViewPrevT1 = Session("aDataViewPrevT1")
            aDataViewPrevT1.Sort = ""
            If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = sortExpression & " " + sortDirection
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.DataBind()
        Catch
        End Try
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataInst).Text) Then
                e.Row.Cells(CellIdxT.DataInst).Text = Format(CDate(e.Row.Cells(CellIdxT.DataInst).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataScadGaranzia).Text) Then
                e.Row.Cells(CellIdxT.DataScadGaranzia).Text = Format(CDate(e.Row.Cells(CellIdxT.DataScadGaranzia).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataScadEl).Text) Then
                e.Row.Cells(CellIdxT.DataScadEl).Text = Format(CDate(e.Row.Cells(CellIdxT.DataScadEl).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataScadBa).Text) Then
                e.Row.Cells(CellIdxT.DataScadBa).Text = Format(CDate(e.Row.Cells(CellIdxT.DataScadBa).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataInvio).Text) Then
                e.Row.Cells(CellIdxT.DataInvio).Text = Format(CDate(e.Row.Cells(CellIdxT.DataInvio).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.NReInvio).Text) Then
                e.Row.Cells(CellIdxT.NReInvio).Text = Format(Val(e.Row.Cells(CellIdxT.NReInvio).Text), "###,###").ToString
            End If
        End If
    End Sub
    Private Sub BuidDett()
        btnRicerca.BackColor = SEGNALA_OKBTN
        SetRicerca()
        SetFilter()

        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = "" 'row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If

    End Sub
    Private Sub SetRicerca()
        txtRicerca.BackColor = SEGNALA_OK
        Dim DataDa As String = ""
        Dim DataA As String = ""
        lblInfoRicerca.Text = ""
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                If txtRicerca.Text.Trim <> "" Then txtRicerca.BackColor = SEGNALA_KO
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "V" Or _
               ddlRicerca.SelectedValue = "C" Or _
               ddlRicerca.SelectedValue = "SE" Or _
               ddlRicerca.SelectedValue = "SB" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                If txtRicerca.Text.Trim <> "" Then txtRicerca.BackColor = SEGNALA_KO
                txtRicerca.Text = ""
            Else
                DataDa = "01/" + CDate(txtRicerca.Text).Month.ToString.Trim + "/" + CDate(txtRicerca.Text).Year.ToString.Trim
                DataDa = Format(CDate(DataDa), FormatoData)
                DataA = DateAdd(DateInterval.Month, 1, CDate(DataDa))
                DataA = DateAdd(DateInterval.Day, -1, CDate(DataA))
                DataA = Format(CDate(DataA), FormatoData)
                If checkParoleContenute.Checked Then
                    lblInfoRicerca.Text = DataDa + " " + DataA
                Else
                    lblInfoRicerca.Text = "Uguale a: " & txtRicerca.Text.Trim
                End If
            End If
        End If

        aDataViewPrevT1.RowFilter = ""
        If ddlRicerca.SelectedValue = "A" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Cod_Articolo like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Cod_Articolo = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "AD" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Descrizione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Descrizione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "NS" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "NSerie like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "NSerie = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "N" And txtRicerca.Text.Trim <> "" Then 'giu290618 lasciato ma non è gestito dalla DDL
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Numero >= " & txtRicerca.Text.Trim
            Else
                aDataViewPrevT1.RowFilter = "Numero = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "D" And txtRicerca.Text.Trim <> "" Then
            If IsDate(txtRicerca.Text.Trim) Then
                If checkParoleContenute.Checked Then
                    aDataViewPrevT1.RowFilter = "Data_Installazione >= '" & CDate(DataDa) & "' AND Data_Installazione <= '" & CDate(DataA) & "'"
                Else
                    aDataViewPrevT1.RowFilter = "Data_Installazione = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "CG" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Cod_Coge like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Cod_Coge = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "R" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "C" And txtRicerca.Text.Trim <> "" Then
            If IsDate(txtRicerca.Text.Trim) Then
                If checkParoleContenute.Checked Then
                    aDataViewPrevT1.RowFilter = "DataScadGaranzia >= '" & CDate(DataDa) & "' AND DataScadGaranzia <= '" & CDate(DataA) & "'"
                Else
                    aDataViewPrevT1.RowFilter = "DataScadGaranzia = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "V" And txtRicerca.Text.Trim <> "" Then 'Data1Invio Data2Invio
            If IsDate(txtRicerca.Text.Trim) Then
                If checkParoleContenute.Checked Then
                    DataDa = txtRicerca.Text
                    DataA = txtRicerca.Text
                End If
                aDataViewPrevT1.RowFilter = "" & _
                    "(Data1InvioScadGa >= '" & CDate(DataDa) & "' AND Data1InvioScadGa <= '" & CDate(DataA) & "') OR " & _
                    "(Data1InvioScadBa >= '" & CDate(DataDa) & "' AND Data1InvioScadBa <= '" & CDate(DataA) & "') OR " & _
                    "(Data1InvioScadEl >= '" & CDate(DataDa) & "' AND Data1InvioScadEl <= '" & CDate(DataA) & "') OR " & _
                    "(Data2InvioScadGa >= '" & CDate(DataDa) & "' AND Data2InvioScadGa <= '" & CDate(DataA) & "') OR " & _
                    "(Data2InvioScadBa >= '" & CDate(DataDa) & "' AND Data2InvioScadBa <= '" & CDate(DataA) & "') OR " & _
                    "(Data2InvioScadEl >= '" & CDate(DataDa) & "' AND Data2InvioScadEl <= '" & CDate(DataA) & "')"
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "SE" And txtRicerca.Text.Trim <> "" Then 'DataScadElettrodi
            If IsDate(txtRicerca.Text.Trim) Then
                If checkParoleContenute.Checked Then
                    aDataViewPrevT1.RowFilter = "DataScadElettrodi >= '" & CDate(DataDa) & "' AND DataScadElettrodi <= '" & CDate(DataA) & "'"
                Else
                    aDataViewPrevT1.RowFilter = "DataScadElettrodi = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "SB" And txtRicerca.Text.Trim <> "" Then 'DataScadBatterie
            If IsDate(txtRicerca.Text.Trim) Then
                If checkParoleContenute.Checked Then
                    aDataViewPrevT1.RowFilter = "DataScadBatterie >= '" & CDate(DataDa) & "' AND DataScadBatterie <= '" & CDate(DataA) & "'"
                Else
                    aDataViewPrevT1.RowFilter = "DataScadBatterie = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "L" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "M" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "S" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "P" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "F" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "RI" And txtRicerca.Text.Trim <> "" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "RiferimentiRic like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "RiferimentiRic >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
            'simone220518
        ElseIf ddlRicerca.SelectedValue = "D1" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Destinazione1 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Destinazione1 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D2" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Destinazione2 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Destinazione2 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "D3" Then
            If checkParoleContenute.Checked = True Then
                aDataViewPrevT1.RowFilter = "Destinazione3 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                aDataViewPrevT1.RowFilter = "Destinazione3 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        End If
        '-
    End Sub
    Private Sub SetFilter()
        Dim Filter As String = ""
        Filter = aDataViewPrevT1.RowFilter.ToString

        If rbtnAttivo.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "Attivo<>0"
        End If
        If rbtnDismesso.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "Attivo=0"
        End If
        If rbtnInRiparazione.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "InRiparazione<>0"
        End If
        If rbtnInviataEmail.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "NOT (Data1InvioScadGa IS NULL AND Data1InvioScadEl IS NULL AND Data1InvioScadBa IS NULL " & _
                      " AND Data2InvioScadGa IS NULL AND Data2InvioScadEl IS NULL AND Data2InvioScadBa IS NULL)"
        End If
        If rbtnSostituito.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "Sostituito<>0"
        End If
        '
        If rbtnConScadenze.Checked Then
            If Filter.Trim <> "" Then
                Filter &= " AND "
            End If
            Filter &= "NOT (DataScadElettrodi IS NULL AND DataScadBatterie IS NULL)"
        End If
        aDataViewPrevT1.RowFilter = Filter
    End Sub
    Private Sub checkParoleContenute_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkParoleContenute.CheckedChanged
        BuidDett()
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        If ddlRicerca.SelectedValue = "D" Or _
           ddlRicerca.SelectedValue = "V" Or _
           ddlRicerca.SelectedValue = "C" Or _
           ddlRicerca.SelectedValue = "SE" Or _
           ddlRicerca.SelectedValue = "SB" Then
            checkParoleContenute.Text = "Intero mese"
        Else
            checkParoleContenute.Text = "Parole contenute"
        End If
        SetRicerca() 'giu190514
        SetFilter()
        '---------
        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If

    End Sub
    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        'se non sono in visualizzazione dettaglio elenco ricarico il datasource con la stored--sim010618
        btnRicerca.BackColor = SEGNALA_OKBTN
        If lblDescrFiltri.Text.Trim = "" Then
            'caricamento grid default--Sim010618
            aDataViewPrevT1 = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.DataBind()
            Session("aDataViewPrevT1") = aDataViewPrevT1
        End If
        '--

        SetRicerca()
        SetFilter()
        '---------
        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If
    End Sub
#End Region

#Region "Scelta tipo documenti"
    Private Sub rbtnAttivo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnAttivo.CheckedChanged
        BuidDett()
    End Sub
    Private Sub rbtnInviataEmail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInviataEmail.CheckedChanged
        BuidDett()
    End Sub
    Private Sub rbtnDismesso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDismesso.CheckedChanged
        BuidDett()
    End Sub

    Private Sub rbtnInRiparazione_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInRiparazione.CheckedChanged
        BuidDett()
    End Sub
    '-
    Private Sub rbtnSostituito_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnSostituito.CheckedChanged
        BuidDett()
    End Sub
    '-
    Private Sub rbtnConScadenze_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnConScadenze.CheckedChanged
        BuidDett()
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        BuidDett()
    End Sub
#End Region

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKAbilitato(SWOPMODIFICA) = False Then Exit Sub
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        SaveSWRbtn()
        Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_ArticoliInstallati.aspx?labelForm=Articoli consumabili Clienti")
        'giu010718 usa questi per aggiornare
        ' ''Session("dvDocT") = dvDocT
        ' ''Session("DsDocT") = DsArtInst
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If CKAbilitato(SWOPNUOVO) = False Then Exit Sub
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewDocAI"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo", "Confermi la creazione ? ", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Private Sub BtnNuovoCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnNuovoCopia.Click
        If CKAbilitato(SWOPNUOVO) = False Then Exit Sub
        Dim myIDArticolo As String = Session(IDARTICOLOINST)
        If IsNothing(myIDArticolo) Then
            myIDArticolo = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myIDArticolo) Then
            myIDArticolo = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewDocCAFromAI"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo", "(Copia dati) Confermi la creazione ? ", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Private Function CKAbilitato(ByVal _Op As String) As Boolean
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Function
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
            Exit Function
        End If
        CKAbilitato = True
    End Function
    Private Sub SaveSWRbtn()
        If rbtnAttivo.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnAttivo"
        ElseIf rbtnDismesso.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnDismesso"
        ElseIf rbtnInRiparazione.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnInRiparazione"
        ElseIf rbtnInviataEmail.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnInviataEmail"
        ElseIf rbtnSostituito.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnSostituito"
        ElseIf rbtnTutti.Checked = True Then
            Session(CSTSWRbtnTD) = "rbtnTutti"
        Else
            Session(CSTSWRbtnTD) = "rbtnTutti" 'DEFAULT
        End If
    End Sub
    Public Sub CreaNewDocAI()
        SaveSWRbtn()
        Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
        Session(SWOP) = SWOPNUOVO
        Session(IDARTICOLOINST) = ""
        Session(CSTCODCOGE) = ""
        Session(CSTCODFILIALE) = ""
        Response.Redirect("WF_ArticoliInstallati.aspx?labelForm=Articoli consumabili Clienti")
    End Sub
    Public Function CreaNewDocCAFromAI() As Boolean
        SaveSWRbtn()
        Dim StrErrore As String = ""

        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'giu290512 il nuovo numero verra' richiesto nella gestione non piu da qui
        'Dim NDoc As long = GetNewNumeroDocCA() : Dim NRev As Integer = 0
        Dim NDoc As Long = 0 'GIU120514: Dim NRev As Integer = 0
        'OK CREAZIONE NUOVO CA
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_CreateNewDocCAFromAI]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocOU", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        'GIU120514 SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocOU").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.ArticoloInstallato)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        'GIU120514 SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        'giu190617
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
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDARTICOLOINST) = SqlDbNewCmd.Parameters.Item("@IDDocOU").Value
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

        Try
            myID = Session(IDARTICOLOINST)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        'OK FATTO

        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
        Response.Redirect("WF_ArticoliInstallati.aspx?labelForm=Articoli consumabili Clienti")
    End Function

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CKAbilitato(SWOPELIMINA) = False Then Exit Sub
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
        SaveSWRbtn()
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_ArticoliInstallati.aspx?labelForm=Elimina")
    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
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
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Try
            Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            ' ''BtnSetByStato(Stato)
            Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
            Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
            BtnSetByStato("")
        Catch ex As Exception
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End Try
    End Sub

    'giu280414 GIU280618
    Private Sub btnRicarcaAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicarcaAll.Click
        Session("CicloRicaricaAI") = "INIZIO"
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

        If (sTipoUtente.Equals(CSTTECNICO)) Then
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "RicaricaAllAIDel"
            Session(MODALPOPUP_CALLBACK_METHOD) = "CicloRicaricaAI"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Confermi il ricarico di tutti i prodotti ?" & _
                            "ATTENZIONE, con questa operazione si perderanno tutte le modifiche " & _
                            "fatte dopo l'inserimento automatico.", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            btnRicarcaAll.Enabled = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
    End Sub
    Public Sub CicloRicaricaAI()
        If String.IsNullOrEmpty(Session("CicloRicaricaAI")) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta: CicloRicaricaAI. Impossibile proseguire", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf Session("CicloRicaricaAI").ToString.Trim = "INIZIO" Then
            Session("CicloRicaricaAI") = "RicaricaAllAIDel"
            Session(MODALPOPUP_CALLBACK_METHOD) = "RicaricaAllAIDel"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""

            ModalPopup.Show("Ricarico prodotti installati", "" & _
                            "Cancellazione dati e aggiornamento Anagrafiche articoli." & _
                            "", WUC_ModalPopup.TYPE_CONFIRM_Y)
        ElseIf Session("CicloRicaricaAI").ToString.Trim = "RicaricaAllAIDel" Then
            'PRIMA VOLTA SUL 1° ESERCIZIO - CICLO SU ESERCIZI
            Session("NextEsercizio") = "INIZIO"
            Session("CicloRicaricaAI") = "NextEsercizio"
            Session(MODALPOPUP_CALLBACK_METHOD) = "NextEsercizio"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""

            ModalPopup.Show("Ricarico prodotti installati", "" & _
                            "Inizio caricamento dati per tutti gli esercizi presenti. (NextEsercizio)(INIZIO)" & _
                            "", WUC_ModalPopup.TYPE_CONFIRM_Y)
        ElseIf Session("CicloRicaricaAI").ToString.Trim = "NextEsercizio" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "NextEsercizio"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "" & _
                            "Caricamento dati da movimenti. (NextEsercizio)(" & Session("NextEsercizio").ToString.Trim & ")" & _
                            "", WUC_ModalPopup.TYPE_CONFIRM_Y)
        ElseIf Session("CicloRicaricaAI").ToString.Trim = "FINE" Then
            Call InitializeForm()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "" & _
                            "Terminato Ricarico prodotti installati." & _
                            "", WUC_ModalPopup.TYPE_CONFIRM_Y)
        ElseIf Session("CicloRicaricaAI").ToString.Trim = "" Then
            Call InitializeForm()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "" & _
                            "Terminato con ERRORI." & _
                            "", WUC_ModalPopup.TYPE_CONFIRM_Y)
        End If
    End Sub
    Public Sub RicaricaAllAIDel()
        Session("GetScadProdCons") = ""
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Chiudi("Sessione scaduta: Codice ditta/Esercizio non validi. (RicaricaAllAIDel)")
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim SQLStr As String = ""
        Dim I As Integer
        Dim ObjDB As New DataBaseUtility
        'GIU030614 SOLO DALL'ULTIMO ESERCIZIO SI PUO RICARCARE ALTRIMENTO NO
        If ControllaUltEs(True) = False Then Exit Sub
        If AggNAnni() = False Then Exit Sub
        '-------------------------------------------------------------------
        '-FASE 1 Cancello tutti i prodotti (Scadenzario Tabella fisica mentre nei DB Azi è la Vista che punta allo Scadenzario)
        Try
            'giu290618 tolto l'integrità referenziale con ArticoliInst_ContrattiAss QUINDI OK NON LI CANCELLA DI CONSEGUENZA ID CANCELLATO
            '' ''CANCELLO TUTTE LE EMAIL 
            SQLStr = "DELETE FROM EmailInviateDett"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nella cancellazione EmailInviateDett.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
            SQLStr = "DELETE FROM EmailInviateT"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nella cancellazione EmailInviateT.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
            'CANCELLO SOLO I DATI INSERITI DA DDT E MOVIMENTI DI MAGAZZINO
            SQLStr = "DELETE FROM ArticoliInst_ContrattiAss WHERE ISNULL(IDDocDTMM,0) <>0"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nella cancellazione prodotti installati.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Errore, nella cancellazione prodotti installati." & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        '-
        Session(MODALPOPUP_CALLBACK_METHOD) = "CicloRicaricaAI" 'Session(MODALPOPUP_CALLBACK_METHOD) = "RicaricaAllAI"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Ricarico prodotti installati", "Termine PRIMA fase. Qualiasi tasto per continuare. (RicaricaAllAIDel)" & _
                        "", WUC_ModalPopup.TYPE_CONFIRM_Y)
    End Sub
    Public Function NextEsercizio() As String

        If String.IsNullOrEmpty(Session("NextEsercizio")) Then
            Session("CicloRicaricaAI") = ""
            Session("NextEsercizio") = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta: NextEsercizio. Impossibile proseguire", WUC_ModalPopup.TYPE_ALERT)
            NextEsercizio = ""
            Exit Function
        Else
            NextEsercizio = Session("NextEsercizio").ToString.Trim
        End If
        '-
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim SQLStr As String = ""
        Dim I As Integer
        Dim ObjDB As New DataBaseUtility
        SQLStr = "SELECT * FROM Esercizi WHERE Ditta = '" & myCodDitta & "'"
        If IsNumeric(NextEsercizio) Then
            SQLStr += " AND Esercizio > '" & NextEsercizio.Trim & "'"
        End If
        SQLStr += " ORDER BY Esercizio"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For I = 0 To ds.Tables(0).Rows.Count - 1
                        If Not IsDBNull(ds.Tables(0).Rows(I).Item("Esercizio")) Then
                            Try
                                If ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim <> NextEsercizio.Trim Then
                                    Session("NextEsercizio") = ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim
                                    SQLStr = "EXEC Carica_ArticoliInst_ContrattiAss NULL /* ESERCIZIO: " & Session("NextEsercizio").ToString.Trim & " - NextEsercizio: " & NextEsercizio & " */"
                                    If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.Text, Session("NextEsercizio").ToString.Trim) = False Then
                                        Session("CicloRicaricaAI") = ""
                                        Session("NextEsercizio") = ""
                                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim, WUC_ModalPopup.TYPE_INFO)
                                        Exit Function
                                    End If
                                    Exit For
                                End If
                            Catch ex As Exception
                                Session("CicloRicaricaAI") = ""
                                Session("NextEsercizio") = ""
                                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                                Exit Function
                            End Try
                        Else
                            Session("NextEsercizio") = "FINE"
                            Session("CicloRicaricaAI") = "FINE"
                            Call NextEsercizioFINE()
                        End If
                    Next
                Else
                    Session("NextEsercizio") = "FINE"
                    Session("CicloRicaricaAI") = "FINE"
                    Call NextEsercizioFINE()
                End If
            Else
                'NESSUN ESERCIZIO
                'NON E'POSSIBILE
                Session("CicloRicaricaAI") = ""
                Session("NextEsercizio") = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                Exit Function
            End If
        Catch Ex As Exception
            Session("CicloRicaricaAI") = ""
            Session("NextEsercizio") = ""
            Chiudi("Errore Ricarico prodotti installati: " & Ex.Message.Trim)
            Exit Function
        End Try
        '-
        ObjDB = Nothing
        Call CicloRicaricaAI()
    End Function
    Private Sub NextEsercizioFINE()
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'Aggiorno tutte le date
        Try
            SQLStr = "Update_DateScadAI"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.StoreProcedure) = False Then
                Session("CicloRicaricaAI") = ""
                Session("NextEsercizio") = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. (Aggiorna Scadenze TUTTE)", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch ex As Exception
            Session("CicloRicaricaAI") = ""
            Session("NextEsercizio") = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. (Aggiorna Scadenze TUTTE)" & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        '-
        Try
            SQLStr = "Carica_AI_AggAttivo"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr, TipoCMD.StoreProcedure) = False Then
                Session("CicloRicaricaAI") = ""
                Session("NextEsercizio") = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, aggiornamento prodotti installati. (BilanciamentoAINSerie)", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If

        Catch ex As Exception
            Session("CicloRicaricaAI") = ""
            Session("NextEsercizio") = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Errore, aggiornamento prodotti installati. (BilanciamentoAINSerie)" & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        ObjDB = Nothing
        '---
    End Sub
    '--- VECCHIA GESTIONE UNICA PROCEDURA MA IL TIMEOUT E' TROPPO CORTO ORA USO NEXTESERCIZIO
    Public Sub RicaricaAllAI()
        'GIU280618
        'ECCO LA SP PER SAPERE SE NON C'è STATO ALCUNA REGISTRAZIONE SU ArticoliInst_ContrattiAss
        'OTTENUTO IL CODICE LEGGERE I DOCUMENTID E COSI SAI IDDocumenti da mandare a "EXEC Carica_ArticoliInst_ContrattiAss (IDDocumenti)"
        ' ''SELECT        AnaMag2018.Cod_Articolo, AnaMag2018.NAnniScadElettrodi, AnaMag2018.NAnniScadBatterie
        ' ''FROM            ArticoliInst_ContrattiAss RIGHT OUTER JOIN
        ' ''                         AnaMag2018 ON ArticoliInst_ContrattiAss.Cod_Articolo = AnaMag2018.Cod_Articolo
        ' ''WHERE        (AnaMag2018.NAnniScadElettrodi <> 0) AND (ArticoliInst_ContrattiAss.ID IS NULL) OR
        ' ''                 (AnaMag2018.NAnniScadBatterie <> 0) AND (ArticoliInst_ContrattiAss.ID IS NULL)
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Chiudi("Sessione scaduta: Codice ditta/Esercizio non validi. (RicaricaAllAI)")
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim SQLStr As String = ""
        Dim I As Integer
        Dim ObjDB As New DataBaseUtility
        'GIU030614 SOLO DALL'ULTIMO ESERCIZIO SI PUO RICARCARE ALTRIMENTO NO
        If ControllaUltEs(True) = False Then Exit Sub
        '-
        SQLStr = "SELECT * FROM Esercizi WHERE Ditta = '" & myCodDitta & "' ORDER BY Esercizio"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For I = 0 To ds.Tables(0).Rows.Count - 1
                        If Not IsDBNull(ds.Tables(0).Rows(I).Item("Esercizio")) Then
                            Try
                                SQLStr = "EXEC Carica_ArticoliInst_ContrattiAss NULL"
                                If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.Text, ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim) = False Then
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim, WUC_ModalPopup.TYPE_INFO)
                                    Exit Sub
                                End If
                            Catch ex As Exception
                                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                                Exit Sub
                            End Try
                        Else
                            'NON E'POSSIBILE
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricarico prodotti installati", "Errore, Esercizio errato nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                            Exit Sub
                        End If
                    Next
                Else
                    'NESSUN ESERCIZIO
                    'NON E'POSSIBILE
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Ricarico prodotti installati", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            Else
                'NESSUN ESERCIZIO
                'NON E'POSSIBILE
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch Ex As Exception
            Chiudi("Errore Ricarico prodotti installati: " & Ex.Message.Trim)
            Exit Sub
        End Try
        '-
        ObjDB = Nothing
        Session(MODALPOPUP_CALLBACK_METHOD) = "UpdateDateScadAI"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("<br><strong><span>Ricarico prodotti installati", "Termine SECONDA fase. Qualiasi tasto per continuare <br>" & _
                        "</span></strong> <br>", WUC_ModalPopup.TYPE_CONFIRM_Y)


    End Sub
    Public Sub UpdateDateScadAI()
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'Aggiorno tutte le date
        Try
            SQLStr = "Update_DateScadAI"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.StoreProcedure) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. (Aggiorna Scadenze TUTTE)", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Errore, nel caricamento prodotti installati. (Aggiorna Scadenze TUTTE)" & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        '---------------------
        ObjDB = Nothing
        Session(MODALPOPUP_CALLBACK_METHOD) = "BilanciamentoAINSerie"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("<br><strong><span>Ricarico prodotti installati", "Termine TERZA fase. Qualiasi tasto per ULTIMA FASE <br>" & _
                        "</span></strong> <br>", WUC_ModalPopup.TYPE_CONFIRM_Y)


    End Sub
    Public Sub BilanciamentoAINSerie()
        'OK Bilanciamento RESI e RICARICOI DATI
        Dim ObjDB As New DataBaseUtility
        Try
            Dim SQLStr As String = "Carica_AI_AggAttivo"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr, TipoCMD.StoreProcedure) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati", "Errore, aggiornamento prodotti installati. (BilanciamentoAINSerie)", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If

        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricarico prodotti installati", "Errore, aggiornamento prodotti installati. (BilanciamentoAINSerie)" & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        ObjDB = Nothing
        '---
        Call btnRicerca_Click(btnRicerca, Nothing)
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Ricarico prodotti installati", "Operazione terminata.", WUC_ModalPopup.TYPE_INFO)
    End Sub
    'GIU230514
    Private Sub btnAggiornaScad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiornaScad.Click
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
            Session(MODALPOPUP_CALLBACK_METHOD) = "AggiornaScadAITutte"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "AggiornaScadAI"

            ModalPopup.Show("<br><strong><span>Aggiorna date Scadenze", "Confermi l'aggiornamento date scadenze ? <br>" & _
                            "(Garanzia,Elettrodi e Batteririe)</span></strong> <br>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            btnRicarcaAll.Enabled = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
    End Sub
    Public Sub AggiornaScadAITutte()
        'GIU030614 SOLO DALL'ULTIMO ESERCIZIO SI PUO RICARCARE ALTRIMENTO NO
        If ControllaUltEs(True) = False Then Exit Sub
        If AggNAnni() = False Then Exit Sub
        '-------------------------------------------------------------------
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'Aggiorno tutte le date
        Try
            SQLStr = "Update_DateScadAI"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.StoreProcedure) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiorna date Scadenze tutte", "Errore, nell'aggiornamento date scadenze.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiorna date Scadenze tutte", "Errore, nell'aggiornamento date scadenze." & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        '---------------------
        ObjDB = Nothing
        Call btnRicerca_Click(btnRicerca, Nothing)
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiorna date Scadenze tutte", "Operazione terminata.", WUC_ModalPopup.TYPE_INFO)
    End Sub
    Public Sub AggiornaScadAI()
        'GIU030614 SOLO DALL'ULTIMO ESERCIZIO SI PUO RICARCARE ALTRIMENTO NO
        If ControllaUltEs(True) = False Then Exit Sub
        If AggNAnni() = False Then Exit Sub
        '-------------------------------------------------------------------
        'EXEC	Update_DateScadAI 'A'
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'Aggiorno SCADENZE le date non valorizzare
        Try
            SQLStr = "EXEC Update_DateScadAI  'A'"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.Text) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiorna date Scadenze non valorizzare", "Errore, nell'aggiornamento date scadenze.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiorna date Scadenze non valorizzare", "Errore, nell'aggiornamento date scadenze." & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End Try
        '---------------------
        ObjDB = Nothing
        Call btnRicerca_Click(btnRicerca, Nothing)
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiorna date Scadenze non valorizzare", "Operazione terminata.", WUC_ModalPopup.TYPE_INFO)
    End Sub
    'giu030614
    Public Function ControllaUltEs(ByVal NoMess As Boolean) As Boolean
        ControllaUltEs = False
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Chiudi("Sessione scaduta: Codice ditta/Esercizio non validi. (ControllaUltEs)")
            Exit Function
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'GIU030614 SOLO DALL'ULTIMO ESERCIZIO SI PUO RICARCARE ALTRIMENTO NO
        SQLStr = "SELECT TOP (1) * FROM Esercizi WHERE Ditta = '" & myCodDitta & "' ORDER BY ESERCIZIO DESC"
        Dim dsCTR As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, dsCTR)
            If (dsCTR.Tables.Count > 0) Then
                If (dsCTR.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio")) Then
                        If dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio").ToString.Trim <> myEsercizio.Trim Then
                            If NoMess Then
                                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Ricarico prodotti installati (ControllaUltEs) ", "Attenzione, il caricamento prodotti installati dev'essere eseguito solo dall'Esercizio: " & dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio").ToString.Trim, WUC_ModalPopup.TYPE_INFO)
                            End If
                            Exit Function
                        Else
                            ControllaUltEs = True
                        End If
                    Else
                        'NON E'POSSIBILE
                        If NoMess Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricarico prodotti installati (ControllaUltEs) ", "Errore, Esercizio errato nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                        End If

                        Exit Function
                    End If
                Else
                    'NESSUN ESERCIZIO
                    'NON E'POSSIBILE
                    If NoMess Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Ricarico prodotti installati (ControllaUltEs) ", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                    End If

                    Exit Function
                End If
            Else
                'NESSUN ESERCIZIO
                'NON E'POSSIBILE
                If NoMess Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Ricarico prodotti installati (ControllaUltEs) ", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                End If
                Exit Function
            End If
        Catch Ex As Exception
            Chiudi("(ControllaUltEs) Errore Ricarico prodotti installati: " & Ex.Message.Trim)
            Exit Function
        End Try
        '-------------------------------------------------------------------
    End Function
    '-
    Private Function AggNAnni() As Boolean
        AggNAnni = True
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            AggNAnni = False
            Chiudi("Sessione scaduta: Codice ditta/Esercizio non validi. (AggNAnni)")
            Exit Function
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'GIU030614 AGGIORNO NANNI DI TUTTI GLI ESERCIZI PRECEDENTI UGUALI A QUELLI CODIFICATI A OGGI
        Dim SQLstrAAAA As String = "UPDATE  AnaMagAAAA " & _
            "SET NAnniGaranzia = AnaMag" & myEsercizio.Trim & ".NAnniGaranzia, " & _
            "NAnniScadElettrodi = AnaMag" & myEsercizio.Trim & ".NAnniScadElettrodi, " & _
            "NAnniScadBatterie = AnaMag" & myEsercizio.Trim & ".NAnniScadBatterie " & _
            "FROM AnaMag" & myEsercizio.Trim & " INNER JOIN " & _
            "AnaMagAAAA ON AnaMag" & myEsercizio.Trim & ".Cod_Articolo = AnaMagAAAA.Cod_Articolo"
        SQLStr = "SELECT * FROM Esercizi WHERE Ditta = '" & myCodDitta & "' ORDER BY Esercizio"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For I = 0 To ds.Tables(0).Rows.Count - 1
                        If Not IsDBNull(ds.Tables(0).Rows(I).Item("Esercizio")) Then
                            If ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim <> myEsercizio.Trim Then
                                Try
                                    SQLStr = "UPDATE AnaMag " & _
                                        "SET NAnniGaranzia = NULL, " & _
                                        "NAnniScadElettrodi = NULL, " & _
                                        "NAnniScadBatterie = NULL"
                                    If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr, TipoCMD.Text, ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim) = False Then
                                        AggNAnni = False
                                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        ModalPopup.Show("Ricarico prodotti installati (AggNAnni 1)", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim, WUC_ModalPopup.TYPE_INFO)
                                        Exit Function
                                    End If
                                Catch ex As Exception
                                    AggNAnni = False
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Ricarico prodotti installati (AggNAnni 1)", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                                    Exit Function
                                End Try
                                '-
                                Try
                                    SQLStr = SQLstrAAAA.Replace("AnaMagAAAA", "AnaMag" & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim)
                                    If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr, TipoCMD.Text) = False Then
                                        AggNAnni = False
                                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        ModalPopup.Show("Ricarico prodotti installati (AggNAnni 2)", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim, WUC_ModalPopup.TYPE_INFO)
                                        Exit Function
                                    End If
                                Catch ex As Exception
                                    AggNAnni = False
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Ricarico prodotti installati (AggNAnni 2)", "Errore, nel caricamento prodotti installati. Esercizio: " & ds.Tables(0).Rows(I).Item("Esercizio").ToString.Trim & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                                    Exit Function
                                End Try
                            End If
                        Else
                            'NON E'POSSIBILE
                            AggNAnni = False
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Ricarico prodotti installati (AggNAnni)", "Errore, Esercizio errato nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                            Exit Function
                        End If
                    Next
                Else
                    'NESSUN ESERCIZIO
                    'NON E'POSSIBILE
                    AggNAnni = False
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Ricarico prodotti installati (AggNAnni)", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                    Exit Function
                End If
            Else
                'NESSUN ESERCIZIO
                'NON E'POSSIBILE
                AggNAnni = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricarico prodotti installati (AggNAnni)", "Errore, nessun esercizio presente nella tabella Esercizi", WUC_ModalPopup.TYPE_INFO)
                Exit Function
            End If
        Catch Ex As Exception
            AggNAnni = False
            Chiudi("(AggNAnni) Errore Ricarico prodotti installati: " & Ex.Message.Trim)
            Exit Function
        End Try
    End Function
    '---------------------------------------------------------
    Private Sub btnStampaSingolo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSingolo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Session(CSTTipoStampaAICA) = TipoStampaAICA.SingoloAICA
        TipoDoc = SWTD(TD.ArticoloInstallato)
        Try
            DsPrinWebDoc.ArticoliInstallatiST.Clear()
            If ClsPrint.StampaAICA(Session(IDARTICOLOINST), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTNOBACK) = 0
                Response.Redirect("..\WebFormTables\WF_PrintWeb_AICA.aspx?labelForm=Scheda Articolo." & Session(IDARTICOLOINST).ToString.Trim)
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
    Private Sub btnSelClientiSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelClientiSc.Click
        Session(F_VISUALIZZADETT_APERTA) = False
        Session(F_ARTINSTEMAIL_APERTA) = True
        Session(F_EMAILINVIATE_APERTA) = False
        Session(F_ELENCO_APERTA) = String.Empty
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_ARTINSTEMAIL_APERTA
        WFP_ArticoliInstEmail.Show(True)
        'giu170618 ripresentoidatiricercati prima WFP_ArticoliInstEmail.SvuotaGriglia()
    End Sub
    Private Sub btnElencoEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElencoEmail.Click
        Session(F_VISUALIZZADETT_APERTA) = False
        Session(F_ARTINSTEMAIL_APERTA) = False
        Session(F_EMAILINVIATE_APERTA) = True
        Session(F_ELENCO_APERTA) = String.Empty
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_EMAILINVIATE_APERTA
        WFP_ElencoEmail.Show(True)
    End Sub
    'Private Sub btnEspPDF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEspPDF.Click
    '    Session(CSTESPORTAPDF) = True
    '    Session(F_STAMPAELENCOAI_APERTA) = True
    '    WUC_StampaElencoAI1.Show()
    'End Sub
    'giu13052014 GIU250618 giu310818
    Public Sub CallBackStampaElencoAI(ByVal TipoOrdineST As String, ByVal TipoOrdine As String, _
                ByVal TipoOrdineRPT As Integer, ByVal DataScadenzaDA As DateTime, ByVal DataScadenzaA As DateTime, _
                ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, _
                ByVal Codice_CoGe As String, ByVal Codice_Art As String, ByRef DsPrinWebDoc As DSPrintWeb_Documenti, _
                ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, ByVal DescCatCli As String, ByVal CodCategoria As Integer, _
                ByVal SelCategorie As Boolean, ByVal CodCategSel As String, _
                ByVal CliSenzaMail As Boolean, ByVal CliConMail As Boolean, ByVal CliNoInvioEmail As Boolean, ByVal CliConMailErr As Boolean, _
                Optional ByVal EffettuaStampa As Boolean = False, _
                Optional ByVal CaricaGrigliaDettaglio As Boolean = False, Optional ByVal CodCogeSelezionati As String = "")
        '-
        rbtnTutti.AutoPostBack = False
        rbtnTutti.Checked = True
        rbtnTutti.AutoPostBack = True
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Session(CSTTipoStampaAICA) = TipoOrdineRPT
        Dim AziendaReport As String = Session(CSTAZIENDARPT).ToString.Trim
        Dim TitoloReport As String = ""
        Dim DescFiltri As String = ""

        'DescCateg selezione categoria
        Dim DescCategoria As String = ""
        Session("SelAICatCli") = "" : Session("SelMAICatCli") = ""
        If SelTutteCatCli = True Then
            DescCategoria = "Tutte"
            Session("SelAICatCli") = -1 : Session("SelMAICatCli") = ""
        ElseIf SelCategorie = True Then
            DescCategoria = DescCatCli
            Session("SelAICatCli") = 0 : Session("SelMAICatCli") = CodCategSel
        Else
            If SelRaggrCatCli = True Then
                DescCategoria = DescCatCli & " (raggruppamento)"
                Session("SelAICatCli") = CodCategoria * -1 : Session("SelMAICatCli") = ""
            Else
                DescCategoria = DescCatCli
                Session("SelAICatCli") = CodCategoria : Session("SelMAICatCli") = ""
            End If
        End If

        TitoloReport = "Stampa elenco Scadenze"
        DescFiltri = "Categoria: " & DescCategoria & ""

        Session("DataScadenzaDA") = ""
        Session("DataScadenzaA") = ""
        Session("SWTBA") = "" : Session("SWTEL") = "" : Session("SWTGA") = ""
        If SelScBa = True Or SelScEl = True Or SelScGa = True Then
            Session("DataScadenzaDA") = DataScadenzaDA
            Session("DataScadenzaA") = DataScadenzaA
            DescFiltri &= " - Data di Scadenza dal: " & Format(DataScadenzaDA, FormatoData) & " al: " & Format(DataScadenzaA, FormatoData)
            DescFiltri &= " - Scadenze: "
            If SelScBa = True Then
                DescFiltri &= " Batterie -"
                'SWTBA = 1
                Session("SWTBA") = 1
            Else
                'SWTBA = 0
                Session("SWTBA") = 0
            End If

            If SelScEl = True Then
                DescFiltri &= " Elettrodi -"
                'SWTEL = 1
                Session("SWTEL") = 1
            Else
                'SWTEL = 0
                Session("SWTEL") = 0
            End If

            If SelScGa = True Then
                DescFiltri &= " Garanzia"
                'SWTGA = 1
                Session("SWTGA") = 1
            Else
                'SWTGA = 0
                Session("SWTGA") = 0
            End If
            'se finisce con un trattino finale lo rimuovo
            If DescFiltri.EndsWith("-") Then
                DescFiltri = DescFiltri.Substring(0, DescFiltri.Length - 1) 'rimuovo ultimo trattino
            End If
        Else
            DescFiltri &= " - Data di Scadenza: NESSUNO"
        End If

        If Codice_CoGe.Trim <> "" Then
            DescFiltri += " - Codice Cliente: " & Codice_CoGe.Trim
        End If
        If Codice_Art.Trim <> "" Then
            DescFiltri += " - Codice Articolo: " & Codice_Art.Trim
        End If
        If CliSenzaMail = True Then
            DescFiltri += " - Clienti senza E-mail "
        ElseIf CliConMail = True Then
            DescFiltri += " - Clienti con E-mail "
        ElseIf CliConMailErr = True Then
            DescFiltri += " - Clienti con E-mail Errate "
        ElseIf CliNoInvioEmail = True Then
            DescFiltri += " - Clienti non invio E-mail "
        Else
            DescFiltri += " - Clienti tutti con o senza E-mail "
        End If
        TitoloReport &= " - " & DescFiltri
        Session("DescFiltri") = DescFiltri
        Session("TIPOFiltri") = "SCADENZE"
        lblDescrFiltri.Text = Session("DescFiltri")
        If lblDescrFiltri.Text.Trim = "" Then
            btnCancFiltro.Visible = False
        Else
            btnCancFiltro.Visible = True
        End If
        '-
        Try
            'giu260618 NO ALTRIMENTI MI CANCELLA i dati dei clienti selezionati DsPrinWebDoc.Clear()
            If ClsPrint.BuildArtInstCliDett(AziendaReport, TitoloReport, TipoOrdineST, TipoOrdine, _
                DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, Codice_CoGe, Codice_Art, _
                DsPrinWebDoc, ObjReport, StrErrore, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                SelCategorie, CodCategSel, _
                CliSenzaMail, CliConMail, CliNoInvioEmail, CliConMailErr, EffettuaStampa, CaricaGrigliaDettaglio, CodCogeSelezionati) = True Then
                If DsPrinWebDoc.ArticoliInstallati.Select("").Length = 0 Then
                    'giu210618 non funziona qui il pop
                    WFP_ArticoliInstEmail.SetLblMessUtente("Nessun dato trovato con i parametri di selezione inseriti.")
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc

                'carico dettaglio nella gridviewprevt
                If EffettuaStampa = False Then
                    Session(F_VISUALIZZADETT_APERTA) = True
                    aDataViewPrevT1 = New DataView(DsPrinWebDoc.ArticoliInstallati)
                    GridViewPrevT.DataSource = aDataViewPrevT1
                    Session("aDataViewPrevT1") = aDataViewPrevT1
                    Call btnRicerca_Click(btnRicerca, Nothing)
                    Exit Sub
                End If

                'se no preparo la stampa
                'GIU260618 LA STAMPA CON REPORT NON FUNZIONA DA QUI PERCHE' C'è IL MODAL...
                Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\" '"C:\_RPT"
                Session(CSTPATHPDFWEB) = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/"
                ElaboraRPT("RitornaAI")

            Else
                'giu210618 non funziona qui il pop
                Call Chiudi("Errore: " & StrErrore)
                Exit Sub
            End If
        Catch ex As Exception
            'giu210618 non funziona qui il pop
            Call Chiudi("Errore: " & ex.Message & " " & StrErrore)
            Exit Sub
        End Try
    End Sub
    'giu170718
    Public Sub CallBackStampaElencoEM(ByVal TipoOrdineST As String, ByVal TipoOrdine As String, _
                ByVal TipoOrdineRPT As Integer, ByVal DataEmailDA As DateTime, ByVal DataEmailA As DateTime, _
                ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, _
                ByVal Codice_CoGe As String, ByVal StatoEmail As Integer, ByRef DsPrinWebDoc As DSPrintWeb_Documenti, _
                ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, ByVal DescCatCli As String, ByVal CodCategoria As Integer, _
                ByVal SelCategorie As Boolean, ByVal CodCategSel As String, _
                ByVal DalNumero As Integer, ByVal AlNumero As Integer, ByVal SelDalNAlN As String, _
                Optional ByVal EffettuaStampa As Boolean = False, _
                Optional ByVal CaricaGrigliaDettaglio As Boolean = False, Optional ByVal CodCogeSelezionati As String = "")
        '-
        rbtnTutti.AutoPostBack = False
        rbtnTutti.Checked = True
        rbtnTutti.AutoPostBack = True
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Session(CSTTipoStampaAICA) = TipoOrdineRPT
        Dim AziendaReport As String = Session(CSTAZIENDARPT).ToString.Trim
        Dim TitoloReport As String = ""
        Dim DescFiltri As String = ""
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        'DescCateg selezione categoria
        ' ''Dim DescCategoria As String = ""
        ' ''If SelTutteCatCli = True Then
        ' ''    DescCategoria = "Tutte"
        ' ''ElseIf SelCategorie = True Then
        ' ''    DescCategoria = DescCatCli
        ' ''Else
        ' ''    If SelRaggrCatCli = True Then
        ' ''        DescCategoria = DescCatCli & " (raggruppamento)"
        ' ''    Else
        ' ''        DescCategoria = DescCatCli
        ' ''    End If
        ' ''End If
        ' ''DescFiltri = "Categoria: " & DescCategoria & ""

        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        ' ''Session("SWTBA") = "" : Session("SWTEL") = "" : Session("SWTGA") = ""
        ' ''If SelScBa = True Or SelScEl = True Or SelScGa = True Then
        ' ''    DescFiltri &= " - Data di Scadenza dal: " & Format(DataScadenzaDA, FormatoData) & " al: " & Format(DataScadenzaA, FormatoData)
        ' ''    DescFiltri &= " - Scadenze: "
        ' ''    If SelScBa = True Then
        ' ''        DescFiltri &= " Batterie -"
        ' ''        'SWTBA = 1
        ' ''        Session("SWTBA") = 1
        ' ''    Else
        ' ''        'SWTBA = 0
        ' ''        Session("SWTBA") = 0
        ' ''    End If

        ' ''    If SelScEl = True Then
        ' ''        DescFiltri &= " Elettrodi -"
        ' ''        'SWTEL = 1
        ' ''        Session("SWTEL") = 1
        ' ''    Else
        ' ''        'SWTEL = 0
        ' ''        Session("SWTEL") = 0
        ' ''    End If

        ' ''    If SelScGa = True Then
        ' ''        DescFiltri &= " Garanzia"
        ' ''        'SWTGA = 1
        ' ''        Session("SWTGA") = 1
        ' ''    Else
        ' ''        'SWTGA = 0
        ' ''        Session("SWTGA") = 0
        ' ''    End If
        ' ''    'se finisce con un trattino finale lo rimuovo
        ' ''    If DescFiltri.EndsWith("-") Then
        ' ''        DescFiltri = DescFiltri.Substring(0, DescFiltri.Length - 1) 'rimuovo ultimo trattino
        ' ''    End If
        ' ''Else
        ' ''    DescFiltri &= " - Data di Scadenza: NESSUNO"
        ' ''End If
        If Codice_CoGe.Trim <> "" Then
            If DescFiltri.Trim <> "" Then
                DescFiltri += " - "
            End If
            DescFiltri += "Codice Cliente: " & Codice_CoGe.Trim
        End If
        If DescFiltri.Trim <> "" Then
            DescFiltri += " - Stato E-Mail: "
        Else
            DescFiltri += "Stato E-Mail: "
        End If
        If StatoEmail = -1 Then
            DescFiltri += " Invio Errato "
        ElseIf StatoEmail = 0 Then
            DescFiltri += " Da inviare "
        ElseIf StatoEmail = 1 Then
            DescFiltri += " Inviata "
        ElseIf StatoEmail = 2 Then
            DescFiltri += " Sollecito inviato "
        ElseIf StatoEmail = 3 Then
            DescFiltri += " Parz.Conclusa "
        ElseIf StatoEmail = 9 Then
            DescFiltri += " Annullata "
        ElseIf StatoEmail = 99 Then
            DescFiltri += " Conclusa "
        ElseIf StatoEmail = 999 Then
            DescFiltri += " Tutte "
        Else
            DescFiltri += " ????? "
        End If
        DescFiltri &= " - E-mail Inviate dal: " & Format(DataEmailDA, FormatoData) & " al: " & Format(DataEmailA, FormatoData)
        ' ''DescFiltri &= " - dal N°: " & Format(DalNumero, "###,##0") & " al N°: " & Format(AlNumero, "###,##0")
        'giu030918
        'giu030918 
        If SelDalNAlN.Trim <> "" Then
            Dim arrDalNAlN As String() = SelDalNAlN.Split(";")
            DescFiltri += " - N° E-Mail selezionati: (" & Format(arrDalNAlN.Count, "###,###").ToString 'GIU290519
            ' ''For i = 0 To arrDalNAlN.Count - 1
            ' ''    If arrDalNAlN(i).ToString <> "" Then
            ' ''        DescFiltri += arrDalNAlN(i).ToString & ","
            ' ''    End If
            ' ''Next
            ' ''DescFiltri = DescFiltri.Substring(0, DescFiltri.Length - 1) 'rimuovo ultima virgola
            DescFiltri += ") "
        End If
        '---
        'OK TITOLO
        TitoloReport = "Stampa elenco E-mail Scadenze"
        TitoloReport &= " - " & DescFiltri
        Session("DescFiltri") = DescFiltri
        Session("TIPOFiltri") = "EMAIL"
        lblDescrFiltri.Text = Session("DescFiltri")
        If lblDescrFiltri.Text.Trim = "" Then
            btnCancFiltro.Visible = False
        Else
            btnCancFiltro.Visible = True
        End If
        '-
        Try
            If ClsPrint.BuildEmailCliDett(AziendaReport, TitoloReport, TipoOrdineST, TipoOrdine, _
                DataEmailDA, DataEmailA, SelScGa, SelScEl, SelScBa, Codice_CoGe, StatoEmail, _
                DsPrinWebDoc, ObjReport, StrErrore, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                SelCategorie, CodCategSel, _
                DalNumero, AlNumero, SelDalNAlN, EffettuaStampa, CaricaGrigliaDettaglio, CodCogeSelezionati) = True Then
                If DsPrinWebDoc.ArticoliInstallati.Select("").Length = 0 Then
                    'giu210618 non funziona qui il pop
                    WFP_ArticoliInstEmail.SetLblMessUtente("Nessun dato trovato con i parametri di selezione inseriti.")
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc

                'carico dettaglio nella gridviewprevt
                If EffettuaStampa = False Then
                    Session(F_VISUALIZZADETT_APERTA) = True
                    aDataViewPrevT1 = New DataView(DsPrinWebDoc.ArticoliInstallati)
                    GridViewPrevT.DataSource = aDataViewPrevT1
                    Session("aDataViewPrevT1") = aDataViewPrevT1
                    Call btnRicerca_Click(btnRicerca, Nothing)
                    Exit Sub
                End If

                'se no preparo la stampa
                'GIU260618 LA STAMPA CON REPORT NON FUNZIONA DA QUI PERCHE' C'è IL MODAL...
                Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\" '"C:\_RPT"
                Session(CSTPATHPDFWEB) = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/"
                ElaboraRPT("RitornaEM")

            Else
                'giu210618 non funziona qui il pop
                Call Chiudi("Errore: " & StrErrore)
                Exit Sub
            End If
        Catch ex As Exception
            'giu210618 non funziona qui il pop
            Call Chiudi("Errore: " & ex.Message & " " & StrErrore)
            Exit Sub
        End Try
    End Sub

    Private Sub ElaboraRPT(ByVal _Tipo As String)
        Dim SWTipoStampa As Integer = -1
        If Not String.IsNullOrEmpty(Session(CSTTipoStampaAICA)) Then
            If IsNumeric(Session(CSTTipoStampaAICA)) Then
                SWTipoStampa = Session(CSTTipoStampaAICA)
            End If
        End If
        '-
        'GIU230514 Dim Rpt As Object = Nothing
        Dim Rpt As ReportClass
        '-------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc)
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        Dim CodiceDitta As String = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            CodiceDitta = ""
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            CodiceDitta = ""
        End If
        '---------------------
        If SWTipoStampa = TipoStampaAICA.SingoloAICA Then
            Rpt = New SchedaAICA
            'Uguale per tutte le eventuali aziende, diversamente cambiare il RPT per il CodiceDitta
            If CodiceDitta = "01" Then
                Rpt = New SchedaAICA '01
                Session(CSTNOMEPDF) = "01SchedaAICA.pdf" 'giu230514
            ElseIf CodiceDitta = "05" Then
                Rpt = New SchedaAICA '05
                Session(CSTNOMEPDF) = "05SchedaAICA.pdf" 'giu230514
            End If
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICACliArtNSerie Then
            Rpt = New ElencoAICliArtNSerie
            Session(CSTNOMEPDF) = "ElencoAICliArtNSerie.pdf" 'giu230514
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAArtCliNSerie Then
            Rpt = New ElencoAIArtCliNSerie
            Session(CSTNOMEPDF) = "ElencoAIArtCliNSerie.pdf" 'giu230514
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScGaArtCliNSerie Then
            Rpt = New ElencoAIScGaArtCliNSerie
            Session(CSTNOMEPDF) = "ElencoAIScGaArtCliNSerie.pdf" 'giu230514
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScElArtCliNSerie Then
            Rpt = New ElencoAIScElArtCliNSerie
            Session(CSTNOMEPDF) = "ElencoAIScElArtCliNSerie.pdf" 'giu230514
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScBaArtCliNSerie Then
            Rpt = New ElencoAIScBaArtCliNSerie
            Session(CSTNOMEPDF) = "ElencoAIScBaArtCliNSerie.pdf" 'giu230514
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICASintetico Then
            Rpt = New ElencoAICliCategoria
            Session(CSTNOMEPDF) = "ElencoAICliCategoria.pdf" 'giu230514
        Else
            Session(CSTESPORTAPDF) = False
            Session(CSTESPORTABIN) = False
            Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
            Exit Sub
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)

        'GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT è SUL SERVER,MA BISOGNA AVERE I PERMESSI
        If Not String.IsNullOrEmpty(Session(CSTESPORTAPDF)) And Not String.IsNullOrEmpty(Session(CSTPATHPDF)) And _
            Not String.IsNullOrEmpty(Session(CSTNOMEPDF)) And Not String.IsNullOrEmpty(Session(CSTPATHPDFWEB)) Then
            If Session(CSTESPORTAPDF) = True Then
                Dim stPathReport As String = Session(CSTPATHPDF) '"C:\_RPT"
                Try 'giu281112 errore che il file è gia aperto
                    Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
                    If (Utente Is Nothing) Then
                        Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                        Exit Sub
                    End If
                    Session(CSTNOMEPDF) = Utente.Codice.Trim & Session(CSTNOMEPDF)
                    '---------
                    Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
                    'giu140124
                    Rpt.Close()
                    Rpt.Dispose()
                    Rpt = Nothing
                    '-
                    GC.WaitForPendingFinalizers()
                    GC.Collect()
                    '-------------
                    Session(CSTESPORTAPDF) = False
                    Session(ATTESA_CALLBACK_METHOD) = _Tipo '"Ritorna"
                    Session(CSTNOBACK) = 1
                    Attesa.ShowStampaAll("Fine elaborazione esportazione PDF", "Richiesta dell'apertura di una nuova pagina per la stampa.", _
                                         Attesa.TYPE_CONFIRM, Session(CSTPATHPDFWEB) & Session(CSTNOMEPDF))
                    Exit Sub
                Catch ex As Exception
                    Rpt = Nothing
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Stampa elenco Scadenze", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            End If
        End If

    End Sub

    Public Sub CallChiudi()
        Chiudi("")
    End Sub
    Public Sub RitornaAI()
        Session(CSTESPORTAPDF) = False
        Session(F_VISUALIZZADETT_APERTA) = False
        Session(F_ARTINSTEMAIL_APERTA) = True
        Session(F_EMAILINVIATE_APERTA) = False
        Session(F_ELENCO_APERTA) = String.Empty
        Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_ARTINSTEMAIL_APERTA
        WFP_ArticoliInstEmail.Show(True)
    End Sub
    Public Sub RitornaEM()
        Session(CSTESPORTAPDF) = False
        Session(F_VISUALIZZADETT_APERTA) = False
        Session(F_ARTINSTEMAIL_APERTA) = False
        Session(F_EMAILINVIATE_APERTA) = True
        Session(F_ELENCO_APERTA) = String.Empty
        Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_EMAILINVIATE_APERTA
        WFP_ElencoEmail.Show(True)
    End Sub

    'sim040618  - inizializza form allo stato di default
    Public Sub InitializeForm()
        Try
            Session("aDataViewPrevT1") = Nothing
            aDataViewPrevT1 = New DataView
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.DataBind()
            txtRicerca.Text = ""
            rbtnTutti.AutoPostBack = False
            rbtnTutti.Checked = True
            rbtnTutti.AutoPostBack = True
            Session("DescFiltri") = ""
            lblDescrFiltri.Text = ""
            If lblDescrFiltri.Text.Trim = "" Then
                btnCancFiltro.Visible = False
            Else
                btnCancFiltro.Visible = True
            End If
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            '-
        Catch Ex As Exception
            Chiudi("Errore in InitializeForm: " & Ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        'sim220518
        If CKAbilitato(SWOPNESSUNA) = False Then Exit Sub
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        SaveSWRbtn()
        Session(CSTTIPODOC) = SWTD(TD.ArticoloInstallato)
        Session(SWOP) = SWOPNESSUNA
        Response.Redirect("WF_ArticoliInstallati.aspx?labelForm=Articoli consumabili Clienti")
    End Sub

    Protected Sub chkAttivo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Try
            'selected row gridview
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            GridViewPrevT.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            '---
            Dim SaveFilter As String = aDataViewPrevT1.RowFilter
            Dim SaveSort As String = aDataViewPrevT1.Sort
            '-
            aDataViewPrevT1 = Session("aDataViewPrevT1")
            aDataViewPrevT1.RowFilter = ""

            'per conoscere index dataview filtrato
            If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = "ID"

            If (Session(IDARTICOLOINST) Is Nothing) Or (Session(IDARTICOLOINST).ToString = "") Then
                Exit Sub
            End If

            Dim indexDataView As Integer = aDataViewPrevT1.Find(Session(IDARTICOLOINST))

            If indexDataView = -1 Then
                Exit Sub
            End If
            '---

            'cambio stato flag Attivo
            aDataViewPrevT1.Item(indexDataView).Item("Attivo") = sender.Checked

            'effettuo update
            AggiornaStatoAttivoArticoloDett(aDataViewPrevT1.Item(indexDataView).Item("Attivo"), Session(IDARTICOLOINST))

            Session("aDataViewPrevT1") = aDataViewPrevT1
            If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = SaveSort
            aDataViewPrevT1.RowFilter = SaveFilter
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore chkAttivo_CheckedChanged: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Protected Sub chkSostituito_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Try
            'selected row gridview
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            GridViewPrevT.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            '---
            Dim SaveFilter As String = aDataViewPrevT1.RowFilter
            Dim SaveSort As String = aDataViewPrevT1.Sort
            '-
            aDataViewPrevT1 = Session("aDataViewPrevT1")
            aDataViewPrevT1.RowFilter = ""

            'per conoscere index dataview filtrato
            If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = "ID"

            If (Session(IDARTICOLOINST) Is Nothing) Or (Session(IDARTICOLOINST).ToString = "") Then
                Exit Sub
            End If

            Dim indexDataView As Integer = aDataViewPrevT1.Find(Session(IDARTICOLOINST))

            If indexDataView = -1 Then
                Exit Sub
            End If
            '---

            'se flag sostituito = true allora il flag attivo passa a False
            If sender.checked = True Then
                aDataViewPrevT1.Item(indexDataView).Item("Attivo") = False
            End If
            aDataViewPrevT1.Item(indexDataView).Item("Sostituito") = sender.Checked

            'aggiorno dataSostituzione
            If sender.checked = True Then
                aDataViewPrevT1.Item(indexDataView).Item("DataSostituzione") = Now.Date
            Else
                aDataViewPrevT1.Item(indexDataView).Item("DataSostituzione") = DBNull.Value
            End If

            'effettuo update
            AggiornaStatoSostituitoArticoloDett(aDataViewPrevT1.Item(indexDataView).Item("Attivo"), _
                                        aDataViewPrevT1.Item(indexDataView).Item("Sostituito"), _
                                        Session(IDARTICOLOINST))

            Session("aDataViewPrevT1") = aDataViewPrevT1
            If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = SaveSort
            aDataViewPrevT1.RowFilter = SaveFilter
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore chkAttivo_CheckedChanged: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Public Sub AggiornaStatoAttivoArticoloDett(ByVal Attivo As Boolean, ByVal IDArticolo As Integer)
        If IsNothing(IDArticolo) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility

        'UPDATE STATO ARTICOLO DETTAGLIO
        Try
            SQLStr = "UPDATE ArticoliInst_ContrattiAss SET Attivo = " & IIf(Attivo = True, "1", "0") & _
                     " WHERE ID = " & IDArticolo & ""

            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Aggiornamento non riuscito.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-
    End Sub

    Public Sub AggiornaStatoSostituitoArticoloDett(ByVal Attivo As Boolean, ByVal Sostituito As Boolean, ByVal IDArticolo As Integer)
        If IsNothing(IDArticolo) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility

        'UPDATE STATO ARTICOLO DETTAGLIO
        Try
            SQLStr = "UPDATE ArticoliInst_ContrattiAss SET Attivo = " & IIf(Attivo = True, "1", "0") & " ," & _
                        " Sostituito = " & IIf(Sostituito = True, "1", "0") & " ," & _
                        " DataSostituzione = " & IIf(Sostituito = True, "GETDATE()", "NULL") & _
                        " WHERE ID = " & IDArticolo & ""

            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Aggiornamento non riuscito.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-
    End Sub

    Dim SWScGa As String = "" : Dim SWScEl As String = "" : Dim SWScBa As String = ""
    Dim myDataScadenzaDa As String = "" : Dim myDataScadenzaA As String = ""
    Dim mySelAICatCli As String = "" : Dim mySelMAICatCli As String = ""
    Private Sub btnPreparaEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreparaEmail.Click
        If ControllaUltEs(False) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Attenzione, la preparazione E-mail Scadenze la si può effettuare solo dall'ultimo esercizio.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf txtRicerca.Text.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Attenzione, la preparazione E-mail Scadenze la si può effettuare senza alcuna ricerca/filtro selezionato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If Session(CSTDsArticoliInstEmail) Is Nothing Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessuna Selezione Scadenze effettuata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If lblDescrFiltri.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessuna Selezione Scadenze effettuata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim ClsDB As New DataBaseUtility
        Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
        ClsDB = Nothing
        lblLogInvioEmail.Text = LogInvioEmail
        If lblLogInvioEmail.Text.Trim = "SERVIZIO INVIO EMAIL SOSPESO" Or Mid(lblLogInvioEmail.Text.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            lblLogInvioEmail.BackColor = SEGNALA_KO
        Else
            lblLogInvioEmail.BackColor = SEGNALA_OKLBL
        End If
        If Mid(lblLogInvioEmail.Text.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Attenzione, INVIO EMAIL IN CORSO ... Attendere il termine dell'invio e riprovare.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'Tipo Flag scadenza
        SWScGa = Session("SWTGA")
        If IsNothing(SWScGa) Then
            SWScGa = ""
        ElseIf String.IsNullOrEmpty(SWScGa) Then
            SWScGa = ""
        End If
        SWScEl = Session("SWTEL")
        If IsNothing(SWScEl) Then
            SWScEl = ""
        ElseIf String.IsNullOrEmpty(SWScEl) Then
            SWScEl = ""
        End If
        SWScBa = Session("SWTBA")
        If IsNothing(SWScBa) Then
            SWScBa = ""
        ElseIf String.IsNullOrEmpty(SWScBa) Then
            SWScBa = ""
        End If
        If SWScGa = "" Or SWScEl = "" Or SWScBa = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessun tipo di scadenza è stata selezionata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------
        'Date di scadenza
        myDataScadenzaDa = Session("DataScadenzaDa")
        If IsNothing(myDataScadenzaDa) Then
            myDataScadenzaDa = ""
        ElseIf String.IsNullOrEmpty(myDataScadenzaDa) Then
            myDataScadenzaDa = ""
        End If
        myDataScadenzaA = Session("DataScadenzaA")
        If IsNothing(myDataScadenzaA) Then
            myDataScadenzaA = ""
        ElseIf String.IsNullOrEmpty(myDataScadenzaA) Then
            myDataScadenzaA = ""
        End If
        If Not IsDate(myDataScadenzaDa) Or Not IsDate(myDataScadenzaA) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Date di scadenza non definite.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------
        'Categorie clienti
        mySelAICatCli = Session("SelAICatCli")
        mySelMAICatCli = Session("SelMAICatCli")
        If IsNothing(mySelAICatCli) Then
            mySelAICatCli = ""
        ElseIf String.IsNullOrEmpty(mySelAICatCli) Then
            mySelAICatCli = ""
        End If
        If IsNothing(mySelMAICatCli) Then
            mySelMAICatCli = ""
        ElseIf String.IsNullOrEmpty(mySelMAICatCli) Then
            mySelMAICatCli = ""
        End If
        If mySelAICatCli = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Categorie clienti non definite.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsArticoliInstEmail)
        If DsPrinWebDoc.ArticoliInstEmail.Select("Selezionato<>0").Count = 0 Or DsPrinWebDoc.ArticoliInstallati.Select("Attivo <> 0").Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessun dato trovato con i parametri di selezione inseriti. (Cliente.Selezionato<>0 - ArtInst.Attivo <> 0)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'Verifica filtro email da abilitare
        Dim TotClienti As Integer = DsPrinWebDoc.ArticoliInstEmail.Select("EmailInvio = '' AND Selezionato<>0").Count
        If TotClienti > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Filtro con Clienti senza E-mail.<br>Totale Clienti: " & TotClienti.ToString, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        'Ciclo di pulizia sul raggruppato/dettaglio
        Dim SWErrEmail As Boolean = False
        TotClienti = 0
        '-
        Dim RowArtInstEmail As DSPrintWeb_Documenti.ArticoliInstEmailRow
        'giu031219 cancello clienti a cui l'invio email scad non è stata accettata
        For Each RowArtInstEmail In DsPrinWebDoc.ArticoliInstEmail.Select("Selezionato=0", "EmailInvio")
            RowArtInstEmail.Delete()
        Next
        'giu031219 ricontrollo se sono rimasti dei rks dopo la cancellazione
        TotClienti = DsPrinWebDoc.ArticoliInstEmail.Select("EmailInvio = '' AND Selezionato<>0").Count
        If TotClienti > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Filtro con Clienti senza E-mail.<br>Totale Clienti: " & TotClienti.ToString, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-----------------------------------
        For Each RowArtInstEmail In DsPrinWebDoc.ArticoliInstEmail.Select("Selezionato<>0")
            RowArtInstEmail.BeginEdit()
            If RowArtInstEmail.IsCod_FilialeNull Then
                RowArtInstEmail.Cod_Filiale = 0
            End If
            If RowArtInstEmail.IsEmailInvioNull Then
                RowArtInstEmail.EmailInvio = ""
                RowArtInstEmail.EMailErrata = True
                TotClienti += 1
                SWErrEmail = True
            ElseIf RowArtInstEmail.EmailInvio.Trim <> "" Then
                If ConvalidaEmail(RowArtInstEmail.EmailInvio.Trim) = False Then
                    RowArtInstEmail.EMailErrata = True
                    TotClienti += 1
                    SWErrEmail = True
                Else
                    RowArtInstEmail.EmailInvio = RowArtInstEmail.EmailInvio.Trim.ToLower 'giu250219 
                End If
            Else
                RowArtInstEmail.EMailErrata = True
                TotClienti += 1
                SWErrEmail = True
            End If
            RowArtInstEmail.EndEdit()
        Next
        If TotClienti > 0 Or SWErrEmail = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Filtro con Clienti Errate.<br>Totale Clienti: " & TotClienti.ToString, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-GIU250219 AGGIORNAMENTO EMAILINVIO ALTRIMENTI NON TROVA I DETTAGLI ARTICOLI INSTALLATI (SPAZIO IN TESTA O IN CODA)
        Dim RowArtInst As DSPrintWeb_Documenti.ArticoliInstallatiRow
        For Each RowArtInst In DsPrinWebDoc.ArticoliInstallati.Select("", "EmailInvio")
            RowArtInst.BeginEdit()
            RowArtInst.EmailInvio = RowArtInst.EmailInvio.Trim.ToLower
            RowArtInst.EndEdit()
        Next
        '--------------------------------------------------------------------------------------------------------------------
        DsPrinWebDoc.AcceptChanges()
        Session(CSTDsArticoliInstEmail) = DsPrinWebDoc

        Session(MODALPOPUP_CALLBACK_METHOD) = "PreparaEmail"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Creazione e-mail", "Confermi la preparazione delle E-mail?", WUC_ModalPopup.TYPE_CONFIRM_YN)
    End Sub

    Public Sub PreparaEmail()
        Session("GetScadProdCons") = ""
        Dim ClsDB As New DataBaseUtility
        Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
        ClsDB = Nothing
        lblLogInvioEmail.Text = LogInvioEmail
        If Mid(lblLogInvioEmail.Text.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Attenzione, INVIO EMAIL IN CORSO ... Attendere il termine dell'invio e riprovare.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'Leggo Email dell'utente
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
        Dim strErrore As String = "" : Dim strEmail As String = "EMail"
        SessionUtility.GetDatoOperatore(sUtente, strEmail, strErrore)
        If (strEmail Is Nothing) Then strEmail = ""
        If strEmail.Trim = "" Or strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            If strErrore.Trim <> "" Then
                ModalPopup.Show("Prepara E-mail Scadenze", "Errore recupero E-Mail dell'operatore: " & sUtente.Trim & " Avvisare l'amministratore di sistema. " & strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Else
                ModalPopup.Show("Prepara E-mail Scadenze", "Attenzione, non è stata definita l'E-Mail dell'operatore: " & sUtente.Trim & " Avvisare l'amministratore di sistema.", WUC_ModalPopup.TYPE_ERROR)
            End If
            Exit Sub
        End If
        Session(CSTEMAILUTENTE) = strEmail
        '-----------------------
        'Tipo Flag scadenza
        SWScGa = Session("SWTGA")
        If IsNothing(SWScGa) Then
            SWScGa = ""
        ElseIf String.IsNullOrEmpty(SWScGa) Then
            SWScGa = ""
        End If
        SWScEl = Session("SWTEL")
        If IsNothing(SWScEl) Then
            SWScEl = ""
        ElseIf String.IsNullOrEmpty(SWScEl) Then
            SWScEl = ""
        End If
        SWScBa = Session("SWTBA")
        If IsNothing(SWScBa) Then
            SWScBa = ""
        ElseIf String.IsNullOrEmpty(SWScBa) Then
            SWScBa = ""
        End If
        If SWScGa = "" Or SWScEl = "" Or SWScBa = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessun tipo di scadenza è stata selezionata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '------------------
        'Date di scadenza
        myDataScadenzaDa = Session("DataScadenzaDa")
        If IsNothing(myDataScadenzaDa) Then
            myDataScadenzaDa = ""
        ElseIf String.IsNullOrEmpty(myDataScadenzaDa) Then
            myDataScadenzaDa = ""
        End If
        myDataScadenzaA = Session("DataScadenzaA")
        If IsNothing(myDataScadenzaA) Then
            myDataScadenzaA = ""
        ElseIf String.IsNullOrEmpty(myDataScadenzaA) Then
            myDataScadenzaA = ""
        End If
        If Not IsDate(myDataScadenzaDa) Or Not IsDate(myDataScadenzaA) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Date di scadenza non definite.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------
        'Categorie clienti
        mySelAICatCli = Session("SelAICatCli")
        mySelMAICatCli = Session("SelMAICatCli")
        If IsNothing(mySelAICatCli) Then
            mySelAICatCli = ""
        ElseIf String.IsNullOrEmpty(mySelAICatCli) Then
            mySelAICatCli = ""
        End If
        If IsNothing(mySelMAICatCli) Then
            mySelMAICatCli = ""
        ElseIf String.IsNullOrEmpty(mySelMAICatCli) Then
            mySelMAICatCli = ""
        End If
        If mySelAICatCli = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Categorie clienti non definite.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '----------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc.Clear() 'giu260618 qui ok tanto prendo la sessione
        If Session(CSTDsArticoliInstEmail) Is Nothing Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Nessun dato trovato con i parametri di selezione inseriti.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'SCRIVO CHE STO PREPARANDO EMAIL
        Dim ObjDB As New DataBaseUtility 'giu160920 
        Dim SQLStr As String = "" 'giu160920 
        Try
            SQLStr = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave='InvioEmail') " _
                     & " Update Abilitazioni SET Descrizione='PREPARA EMAIL IN CORSO' WHERE Chiave='InvioEmail' ELSE " _
                     & " INSERT INTO Abilitazioni VALUES ('InvioEmail', 'PREPARA EMAIL IN CORSO', -1)"
            ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, SQLStr, TipoCMD.Text)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Errore scrittura LOG (Abilitazioni)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-----------------------------------
        DsPrinWebDoc = Session(CSTDsArticoliInstEmail)
        DsPrinWebDoc.EmailInviateT.Clear()
        DsPrinWebDoc.EmailInviateDett.Clear()
        DsPrinWebDoc.AcceptChanges()
        '-
        Dim SqlAdapEmailTest As New SqlDataAdapter
        Dim SqlAdapEmailDett As New SqlDataAdapter
        Dim NEmailT As Integer = 0
        Dim NRiga As Integer = 0
        Dim TmpEmail As String = ""
        Dim RowArtInstEmail As DSPrintWeb_Documenti.ArticoliInstEmailRow
        Dim RowArtInst As DSPrintWeb_Documenti.ArticoliInstallatiRow
        Dim RowTestEmail As DSPrintWeb_Documenti.EmailInviateTRow
        Dim RowDettForIns As DSPrintWeb_Documenti.EmailInviateDettRow
        Dim strValore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '-
        Dim OKTransTmp As Boolean = False
        Dim TransTmp As SqlClient.SqlTransaction
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbScadenzario)
        Dim SqlDbNewCmd As New SqlCommand
        Dim SqlDbUpdCmd As New SqlCommand
        Dim SqlDbNewDett As New SqlCommand
        Try
            SqlDbNewCmd.CommandText = "[Insert_EmailInviateT]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.CommandTimeout = myTimeOUT '5000
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Anno", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Anno", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Stato", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Stato", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Email", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NText, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Note", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo1", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo2", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo3", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo4", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo4", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Mittente", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Mittente", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadenzaDa", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "DataScadenzaDa", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadenzaA", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "DataScadenzaA", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagGa", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagGa", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagEl", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagEl", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagBa", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagBa", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAICatCli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SelAICatCli", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelMAICatCli", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SelMAICatCli", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapEmailTest.InsertCommand = SqlDbNewCmd
            '--
            SqlDbUpdCmd.CommandText = "[Update_EmailInviateT_IdMod]"
            SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbUpdCmd.CommandTimeout = myTimeOUT '5000
            SqlDbUpdCmd.Connection = SqlConn
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Anno", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Anno", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Stato", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Stato", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Email", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NText, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Note", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo1", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo1", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo2", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo3", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo3", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdModulo4", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdModulo4", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Mittente", System.Data.SqlDbType.NVarChar, 100, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Mittente", System.Data.DataRowVersion.Current, Nothing))
            SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapEmailTest.UpdateCommand = SqlDbUpdCmd
            '--
            SqlDbNewDett.CommandText = "[Insert_EmailInviateDett]"
            SqlDbNewDett.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewDett.CommandTimeout = myTimeOUT '5000
            SqlDbNewDett.Connection = SqlConn
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDEmailInviateT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDEmailInviateT", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IdArticoliInst_ContrattiAss", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IdArticoliInst_ContrattiAss", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagGa", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagGa", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagEl", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagEl", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FlagBa", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "FlagBa", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewDett.Connection = SqlConn
            SqlAdapEmailDett.InsertCommand = SqlDbNewDett
            'ok apro la connessione e inizio la transazione
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            TransTmp = SqlConn.BeginTransaction(IsolationLevel.ReadCommitted)
            OKTransTmp = True
            SqlDbNewCmd.Transaction = TransTmp
            SqlDbUpdCmd.Transaction = TransTmp
            '-
            SqlDbNewDett.Transaction = TransTmp
            '-----------------------------------------------
            For Each RowArtInstEmail In DsPrinWebDoc.ArticoliInstEmail.Select("Selezionato<>0", "EmailInvio")

                ' rottura email
                ' ''qui portare dentro la scrittuta dei dettagli altrimenti prende 2 volte lo stesso dettaglio 
                ' ''perche è presente la filiale quindi ci sono 2 testate ma un unica email per effetto dell'email uguale anche 
                ' ''PerformanceCounter la destinazione 

                If TmpEmail.Trim.ToLower <> RowArtInstEmail.EmailInvio.Trim.ToLower Then

                    TmpEmail = RowArtInstEmail.EmailInvio.Trim.ToLower

                    '---- ok scrivo nel DB per ottenere ID
                    RowTestEmail = DsPrinWebDoc.EmailInviateT.NewRow
                    RowTestEmail.Id = -1
                    'Inserito in automatico nella SP RowTestEmail.Numero
                    RowTestEmail.Anno = Now.Year
                    RowTestEmail.Stato = 0 'DA INVIARE
                    RowTestEmail.Email = TmpEmail.Trim.ToLower
                    RowTestEmail.Note = ""
                    RowTestEmail.IsIdModulo1Null()
                    RowTestEmail.IsIdModulo2Null()
                    RowTestEmail.IsIdModulo3Null()
                    RowTestEmail.IsIdModulo4Null()
                    If Not String.IsNullOrEmpty(Session(CSTEMAILUTENTE)) Then
                        RowTestEmail.Mittente = Session(CSTEMAILUTENTE).ToString.Trim
                    Else
                        RowTestEmail.IsMittenteNull()
                    End If
                    '-
                    If Not String.IsNullOrEmpty(Session("DataScadenzaDa")) Then
                        RowTestEmail.DataScadenzaDa = CDate(Session("DataScadenzaDa"))
                    Else
                        RowTestEmail.IsDataScadenzaDaNull()
                    End If
                    If Not String.IsNullOrEmpty(Session("DataScadenzaA")) Then
                        RowTestEmail.DataScadenzaA = CDate(Session("DataScadenzaA"))
                    Else
                        RowTestEmail.IsDataScadenzaANull()
                    End If
                    '-
                    RowTestEmail.FlagGa = CBool(SWScGa)
                    RowTestEmail.FlagEl = CBool(SWScEl)
                    RowTestEmail.FlagBa = CBool(SWScBa)
                    '-
                    RowTestEmail.SelAICatCli = mySelAICatCli.Trim
                    RowTestEmail.SelMAICatCli = mySelMAICatCli.Trim
                    '-OK
                    DsPrinWebDoc.EmailInviateT.AddEmailInviateTRow(RowTestEmail)
                    Try
                        SqlAdapEmailTest.Update(DsPrinWebDoc.EmailInviateT)
                        Session(IDEMAILINVIATE) = SqlDbNewCmd.Parameters.Item("@RetVal").Value
                        If SqlDbNewCmd.Parameters.Item("@RetVal").Value = -1 Then
                            If OKTransTmp = True Then TransTmp.Rollback()
                            strErrore = "Errore nell'aggiornamento (SQL -1)!!!"
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore EmailInviateT", strErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Catch ExSQL As SqlException
                        If OKTransTmp = True Then TransTmp.Rollback()
                        strErrore = "Errore EmailInviateT (SQL)!!!: <br><br> " & ExSQL.Message
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore EmailInviateT", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    Catch Ex As Exception
                        If OKTransTmp = True Then TransTmp.Rollback()
                        strErrore = "Errore EmailInviateT (Ex)!!!: <br><br> " & Ex.Message
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore EmailInviateT", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End Try

                    NRiga = 1
                    NEmailT += 1
                    'prendo gli id reltivi sul dettaglio
                    For Each RowArtInst In DsPrinWebDoc.ArticoliInstallati.Select("EmailInvio='" & TmpEmail.Trim.ToLower & "'")

                        'salto riga di dettaglio se il campo attivo è a false
                        If Not IsDBNull(RowArtInst.Item("Attivo")) Then
                            If RowArtInst.Item("Attivo") = False Then
                                Continue For
                            End If
                        End If
                        RowDettForIns = DsPrinWebDoc.EmailInviateDett.NewRow

                        RowDettForIns.IdEmailInviateT = Session(IDEMAILINVIATE)
                        RowDettForIns.IdArticoliInst_ContrattiAss = RowArtInst.ID
                        RowDettForIns.Riga = NRiga
                        'giu310818 IMPORTANTE che il FLAG sia impostrato a true oppure false
                        If CBool(SWScGa) = True Then
                            If Not IsDBNull(RowArtInst.Item("DataScadGaranzia")) Then
                                If RowArtInst.Item("DataScadGaranzia") >= CDate(Session("DataScadenzaDa")) And _
                                   RowArtInst.Item("DataScadGaranzia") <= CDate(Session("DataScadenzaA")) Then
                                    RowDettForIns.FlagGa = CBool(SWScGa)
                                Else
                                    RowDettForIns.FlagGa = False
                                End If
                            Else
                                RowDettForIns.FlagGa = False
                            End If
                        Else
                            RowDettForIns.FlagGa = False
                        End If
                        '-
                        If CBool(SWScEl) = True Then
                            If Not IsDBNull(RowArtInst.Item("DataScadElettrodi")) Then
                                If RowArtInst.Item("DataScadElettrodi") >= CDate(Session("DataScadenzaDa")) And _
                                   RowArtInst.Item("DataScadElettrodi") <= CDate(Session("DataScadenzaA")) Then
                                    RowDettForIns.FlagEl = CBool(SWScEl)
                                Else
                                    RowDettForIns.FlagEl = False
                                End If
                            Else
                                RowDettForIns.FlagEl = False
                            End If
                        Else
                            RowDettForIns.FlagEl = False
                        End If
                        '-
                        '-
                        If CBool(SWScBa) = True Then
                            If Not IsDBNull(RowArtInst.Item("DataScadBatterie")) Then
                                If RowArtInst.Item("DataScadBatterie") >= CDate(Session("DataScadenzaDa")) And _
                                   RowArtInst.Item("DataScadBatterie") <= CDate(Session("DataScadenzaA")) Then
                                    RowDettForIns.FlagBa = CBool(SWScBa)
                                Else
                                    RowDettForIns.FlagBa = False
                                End If
                            Else
                                RowDettForIns.FlagBa = False
                            End If
                        Else
                            RowDettForIns.FlagBa = False
                        End If
                        '-
                        DsPrinWebDoc.EmailInviateDett.AddEmailInviateDettRow(RowDettForIns)
                        NRiga = NRiga + 1
                    Next
                    'giu250219 Controllo che vi sia almeno un dettaglio altrimenti c'è qualcosa che non ha funzionato.
                    If NRiga = 1 Then
                        If OKTransTmp = True Then TransTmp.Rollback()
                        strErrore = "Errore EmailInviateDett Nessun dettaglio per l'email: " & TmpEmail
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore EmailInviateDett", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                End If

            Next
            '-------------------------------------------------------------------------------------------------
            'OK SCRIVO I DETTAGLI
            Try
                SqlAdapEmailDett.Update(DsPrinWebDoc.EmailInviateDett)
                If SqlDbNewDett.Parameters.Item("@RetVal").Value = -1 Then
                    If OKTransTmp = True Then TransTmp.Rollback()
                    strErrore = "Errore EmailInviateDett (SQL -1)!!!"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore EmailInviateDett", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Catch ExSQL As SqlException
                If OKTransTmp = True Then TransTmp.Rollback()
                strErrore = "Errore EmailInviateDett (SQL)!!!: <br><br> " & ExSQL.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore EmailInviateDett", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch Ex As Exception
                If OKTransTmp = True Then TransTmp.Rollback()
                strErrore = "Errore EmailInviateDett (Ex)!!!: <br><br> " & Ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore EmailInviateDett", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            DsPrinWebDoc.AcceptChanges()
            'Adesso leggo tutti i dettagli per aggiornare Moduli1 a 4 nelle testate
            'prendo gli id reltivi sul dettaglio
            For Each RowTestEmail In DsPrinWebDoc.EmailInviateT
                RowTestEmail.BeginEdit()
                RowTestEmail.IsIdModulo1Null()
                RowTestEmail.IsIdModulo2Null()
                RowTestEmail.IsIdModulo3Null()
                RowTestEmail.IsIdModulo4Null()
                If Not String.IsNullOrEmpty(Session(CSTEMAILUTENTE)) Then
                    RowTestEmail.Mittente = Session(CSTEMAILUTENTE).ToString.Trim
                Else
                    RowTestEmail.IsMittenteNull()
                End If
                RowTestEmail.EndEdit()
            Next
            Try
                SqlAdapEmailTest.Update(DsPrinWebDoc.EmailInviateT)
                If SqlDbNewCmd.Parameters.Item("@RetVal").Value = -1 Then
                    If OKTransTmp = True Then TransTmp.Rollback()
                    strErrore = "Errore nell'aggiornamento (SQL -1)!!!"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore EmailInviateT IdMod", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Catch ExSQL As SqlException
                If OKTransTmp = True Then TransTmp.Rollback()
                strErrore = "Errore EmailInviateT (SQL)!!!: <br><br> " & ExSQL.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore EmailInviateT IdMod", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch Ex As Exception
                If OKTransTmp = True Then TransTmp.Rollback()
                strErrore = "Errore EmailInviateT (Ex)!!!: <br><br> " & Ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore EmailInviateT IdMod", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            'OK TERMINATO
            TransTmp.Commit()
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
        Catch ExSQL As SqlException
            If OKTransTmp = True Then TransTmp.Rollback()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch ex As Exception
            If OKTransTmp = True Then TransTmp.Rollback()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Finally
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
        End Try
        'OK
        'SCRIVO CHE STO PREPARANDO EMAIL
        Try
            SQLStr = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave='InvioEmail') " _
                     & " Update Abilitazioni SET Descrizione='TERMINE PREPARA EMAIL: " & CStr(NEmailT) & "' " _
                     & "WHERE Chiave='InvioEmail' ELSE " _
                     & " INSERT INTO Abilitazioni VALUES ('InvioEmail', 'TERMINE PREPARA EMAIL: " & CStr(NEmailT) & "', -1)"
            ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, SQLStr, TipoCMD.Text)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Prepara E-mail Scadenze", "Errore scrittura LOG (Abilitazioni)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-----------------------------------
        ObjDB = Nothing 'giu160920
        '-----------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKPreparaEmail"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Numero totale E-Mail create: " & CStr(NEmailT), "Operazione conclusa con successo.", WUC_ModalPopup.TYPE_CONFIRM_Y)
    End Sub
    Public Sub OKPreparaEmail()
        Session("GetScadProdCons") = ""
        'NO AZZERO TUTTO
        Session(CSTDsArticoliInstEmail) = Nothing
        Session("aDataViewPrevT1") = Nothing
        Session("aDataViewPrevT2") = Nothing
        aDataViewPrevT1 = Session("aDataViewPrevT1")
        GridViewPrevT.DataSource = aDataViewPrevT1
        Session("DescFiltri") = ""
        lblDescrFiltri.Text = ""
        If lblDescrFiltri.Text.Trim = "" Then
            btnCancFiltro.Visible = False
        Else
            btnCancFiltro.Visible = True
        End If
        'RALLENTA TROPPO 
        ' ''rbtnInviataEmail.AutoPostBack = False
        ' ''rbtnInviataEmail.Checked = True
        ' ''rbtnInviataEmail.AutoPostBack = True
        ' ''Call btnRicerca_Click(btnRicerca, Nothing)
        '-
        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDARTICOLOINST) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                ' ''BtnSetByStato(Stato)
                BtnSetByStato("")
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDARTICOLOINST) = ""
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If
    End Sub

#Region "Creazione DOCUMENTI COLLEGATI"

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
    End Sub

    Public Sub CancBackWFPDocCollegati()
        'nulla
    End Sub

    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'trovo l'ID DOCUMENTO
        Dim SaveFilter As String = aDataViewPrevT1.RowFilter
        Dim SaveSort As String = aDataViewPrevT1.Sort
        '-
        aDataViewPrevT1 = Session("aDataViewPrevT1")
        aDataViewPrevT1.RowFilter = ""
        'per conoscere index dataview filtrato
        If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = "ID"
        Dim indexDataView As Integer = aDataViewPrevT1.Find(Session(IDARTICOLOINST))

        If indexDataView = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato. (aDataViewPrevT1.Find(Session(IDARTICOLOINST))", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---

        'cambio stato flag Attivo
        If IsDBNull(aDataViewPrevT1.Item(indexDataView).Item("IDDocDTMM")) Then
            Session(IDDOCUMENTI) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento collegato.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Else
            Session(IDDOCUMENTI) = aDataViewPrevT1.Item(indexDataView).Item("IDDocDTMM")
        End If
        'sembra che non serva
        ' ''Session("aDataViewPrevT1") = aDataViewPrevT1
        ' ''If aDataViewPrevT1.Count > 0 Then aDataViewPrevT1.Sort = SaveSort
        ' ''aDataViewPrevT1.RowFilter = SaveFilter
        ' ''GridViewPrevT.DataSource = aDataViewPrevT1
        ' ''GridViewPrevT.EditIndex = -1
        ' ''GridViewPrevT.DataBind()
        '-
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
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegati()
        ' ''WFPDocCollegati.WucElement = Me
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
#End Region

    Private Sub btnCancFiltro_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancFiltro.Click
        Try
            Session("aDataViewPrevT1") = Nothing
            aDataViewPrevT1 = New DataView
            GridViewPrevT.DataSource = aDataViewPrevT1
            GridViewPrevT.DataBind()
            ' ''txtRicerca.Text = ""
            ' ''rbtnTutti.AutoPostBack = False
            ' ''rbtnTutti.Checked = True
            ' ''rbtnTutti.AutoPostBack = True
            Session("DescFiltri") = ""
            lblDescrFiltri.Text = ""
            btnCancFiltro.Visible = False
        Catch Ex As Exception
            Chiudi("Errore in InitializeForm: " & Ex.Message)
            Exit Sub
        End Try
    End Sub
End Class
