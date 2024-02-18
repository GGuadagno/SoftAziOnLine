Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports System.Collections
Imports System.Linq
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
'giu180912 Imports SoftAziOnLine.DataBaseUtility 

'giu180722 Erika/Zibordi per le spedizioni almeno un tel è obbligatorio 
'- per le dest.Merci se non presente lo prendo dall'anagrafica principale - magari con avviso nelle destinazioni
'Session("AvvisaNoTel") = True

Partial Public Class WUC_Clienti
    Inherits System.Web.UI.UserControl
    Const F_ANAGRAGENTI_Proven As String = "F_ANAGRAGENTI_Proven"

    Dim Tab_Vocali() As String
    Dim Tab_Consonanti() As String
    Dim car As String
    Dim Ind1 As Integer
    Dim Ind2 As Integer
    Dim Ind3 As Integer
    Dim Ind4 As Integer
    Dim Cod_Fisc As String
    Dim Codice_Fiscale(17) As String

    Private Enum IdColEC
        NDoc = 0
        DataDoc = 1
        ImportoDoc = 2
        DesPagamento = 3
        ImpResiduo = 4
        Riga = 5
        DataScad = 6
    End Enum

    Private Enum IdColSC
        DataReg = 0
        NReg = 1
        NIva = 2
        NDoc = 3
        DataDoc = 4
        DesCausale = 5
        Dare = 6
        Avere = 7
    End Enum

#Region "Costanti"

    Private Const F_PROVINCE As String = "ElencoProvince"
    Private Const F_NAZIONI As String = "ElencoNazioniCli"
    Private Const F_ALIQUOTAIVA As String = "ElencoAliquotaIVA"
    Private Const F_PAGAMENTI As String = "ElencoPagamenti"
    Private Const F_AGENTI As String = "ElencoAgenti"
    Private Const F_AGENTI_ESE_PREC As String = "ElencoAgentiEsercizioPrecedente"
    Private Const F_ZONE As String = "ElencoZone"
    Private Const F_VETTORI As String = "ElencoVettori"
    Private Const F_CATEGORIE As String = "ElencoCategorie"
    Private Const F_LISTINO As String = "ElencoListinoVendita"
    Private Const F_TIPOFATT As String = "ElencoTipoFatturazione" 'ALBERTO 19/12/2012
    Private Const F_CONTI As String = "ElencoPianoDeiConti"

    Private Const NEXT_CLI As String = "NEXT"
    Private Const PREV_CLI As String = "PREV"
    Private Const LAST_CLI As String = "LAST"
    Private Const FIRST_CLI As String = "FIRST"
    Private Const CLI_CORRENTE As String = "CORRENTE"

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        Session(DBCONNAZI) = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Session(DBCONNCOGE) = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSBancheIBAN.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSEstrConto.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        'GIU240113
        Dim myCoGe As String = Session(COD_CLIENTE)
        Session(CSTCODCOGEDM) = Session(COD_CLIENTE) 'giu010223
        If IsNothing(myCoGe) Then
            myCoGe = ""
        ElseIf String.IsNullOrEmpty(myCoGe) Then
            myCoGe = ""
        End If
        If String.IsNullOrEmpty(myCoGe) Then
            myCoGe = ""
        End If
        If myCoGe = "" Or Not IsNumeric(myCoGe) Then
            Session(CSTRitornoDaStampa) = False
            Session(COD_CLIENTE) = txtCodCliente.Text.Trim
            Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
        End If
        '---------
        If txtCodNazione.Text.Trim <> "I" And txtCodNazione.Text.Trim <> "IT" And txtCodNazione.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
        Try 'giu240113 
            If Session("StampaSCEC") = 1 Then
                Session(CSTRitornoDaStampa) = False 'GIU220222
                btnStampa.Visible = False
                'GIU220322 XKE' SE SI SPOSTASSE PER IL CLIENTE DA CUI E' STATO CHIAMATO >>> GESTIONE DOCUMENTI (TUTTI)
                btnRicerca.Visible = False
                btnNext.Visible = False
                btnPrev.Visible = False
                btnFirst.Visible = False
                btnLast.Visible = False
                '---------
                If TabContainer0.ActiveTabIndex = TB7 Or TabContainer0.ActiveTabIndex = TB8 Then
                    btnNuovo.Visible = False
                    btnModifica.Visible = False
                    btnAggiorna.Visible = False
                    btnAnnulla.Visible = False
                    btnElimina.Visible = False
                Else
                    btnNuovo.Visible = True
                    btnModifica.Visible = True
                    btnAggiorna.Visible = True
                    btnAnnulla.Visible = True
                    btnElimina.Visible = True
                End If
                btnCreaOC.Visible = False
                btnCreaPR.Visible = False
            Else
                Session(CSTChiamatoDa) = "WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti"
                If TabContainer0.ActiveTabIndex = TB7 Or TabContainer0.ActiveTabIndex = TB8 Then
                    btnNuovo.Visible = False
                    btnModifica.Visible = False
                    btnAggiorna.Visible = False
                    btnAnnulla.Visible = False
                    btnElimina.Visible = False
                Else
                    btnNuovo.Visible = True
                    btnModifica.Visible = True
                    btnAggiorna.Visible = True
                    btnAnnulla.Visible = True
                    btnElimina.Visible = True
                End If
                btnCreaOC.Visible = True
                btnCreaPR.Visible = True
            End If
        Catch ex As Exception
            If TabContainer0.ActiveTabIndex = TB7 Or TabContainer0.ActiveTabIndex = TB8 Then
                btnNuovo.Visible = False
                btnModifica.Visible = False
                btnAggiorna.Visible = False
                btnAnnulla.Visible = False
                btnElimina.Visible = False
            Else
                btnNuovo.Visible = True
                btnModifica.Visible = True
                btnAggiorna.Visible = True
                btnAnnulla.Visible = True
                btnElimina.Visible = True
            End If
        End Try
        '-
        If (Not IsPostBack) And Not Session(CSTRitornoDaStampa) Then
            'Session(COD_CLIENTE) = String.Empty commentato alb070213 per correzione errore che dopo stampa SC/EC da gest. doc. torna al 1° cli.
            Session(CSTCODCOGENEW) = "" 'giu240113
            Session(IDDOCOCCLI) = ""
            Session(IDDOCPRCLI) = ""
        End If
        ModalPopup.WucElement = Me
        WUC_Attesa1.WucElement = Me 'giu031211
        GWAltriIndirizzi.WucElement = Me
        DestMerceCliente.WucElement = Me
        '---
        WFPElencoCli.WucElement = Me
        WFPElencoNazioni.WucElement = Me
        WFPElencoProvince.WucElement = Me
        WFPElencoAliquotaIVA.WucElement = Me
        WFPElencoPagamenti.WucElement = Me
        WFPElencoAgenti.WucElement = Me
        WFPElencoAgentiEsePrec.WucElement = Me
        WFPElencoZone.WucElement = Me
        WFPElencoVettori.WucElement = Me
        WFPElencoCategorie.WucElement = Me
        WFPElencoListVenT.WucElement = Me
        WFPElencoConti.WucElement = Me
        WFPZone.WucElement = Me
        WFP_BancheIBAN1.WucElement = Me
        WFPTipoFatturazione.WucElement = Me
        WFPElencoTipoFatt.WucElement = Me
        '--
        Dim strObbligatorio As String = Session(CSTMAXLEVEL)
        If IsNothing(strObbligatorio) Then
            Session(CSTMAXLEVEL) = App.GetDatiDitta(Session(CSTCODDITTA).ToString.Trim, strErrore).MaxLevel.Trim
            strObbligatorio = Session(CSTMAXLEVEL)
        ElseIf String.IsNullOrEmpty(strObbligatorio) Then
            Session(CSTMAXLEVEL) = App.GetDatiDitta(Session(CSTCODDITTA).ToString.Trim, strErrore).MaxLevel.Trim
            strObbligatorio = Session(CSTMAXLEVEL)
        End If
        If String.IsNullOrEmpty(strObbligatorio) Then
            Session(CSTMAXLEVEL) = VALMAXLEVEL
            strObbligatorio = VALMAXLEVEL
        End If
        If strObbligatorio = "" Or Not IsNumeric(strObbligatorio) Then
            Chiudi("Errore: Caricamento dati Società - MaskLevel non definito (Sessione scaduta - effettuare il login)", True)
            Exit Sub
        End If
        If CInt(strObbligatorio) < 1 Then
            Chiudi("Errore: Caricamento dati Società - MaskLevel non definito (Sessione scaduta - effettuare il login)", True)
            Exit Sub
        End If
        txtCodCliente.MaxLength = CInt(strObbligatorio)
        txtCodSede.MaxLength = CInt(strObbligatorio) 'GIU121211
        If (Not IsPostBack) Then
            'giu281212 spostati qui altriemtni vengono sempre reinizializzati
            'GIU311018 modificato ed implementato SWCodAlt
            Dim myDescr As String = ""
            If App.GetDatiAbilitazioni(CSTABILCOGE, "SWCodAlt", myDescr, strErrore) Then
                If strErrore.Trim <> "" Then
                    Chiudi("Errore lettura abilitazioni(SWCodAlt): " & strErrore.Trim, False)
                    Exit Sub
                End If
                lblCMabell.Text = myDescr : lblCMabell.Visible = True : txtCMabell.Visible = True
            ElseIf App.GetDatiAbilitazioni(CSTABILCOGE, "EspMABELL", myDescr, strErrore) Then
                If strErrore.Trim <> "" Then
                    Chiudi("Errore lettura abilitazioni(EspMABELL): " & strErrore.Trim, False)
                    Exit Sub
                End If
                lblCMabell.Text = myDescr : lblCMabell.Visible = True : txtCMabell.Visible = True
            Else
                lblCMabell.Visible = False : txtCMabell.Visible = False
            End If

            txtECDataA.Text = Format(Now.Date, FormatoData)
            txtSCDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtSCDataA.Text = "31/12/" & Session(ESERCIZIO)
            '----------------------------------------------------------------
            CaricaVariabili()
            CampiSetEnabledTo(False)
            setPulsantiModalitaConsulta()
            If Not Session(CSTRitornoDaStampa) Then
                If myCoGe = "" Or Not IsNumeric(myCoGe) Then 'giu230322
                    LocalizzaCliente(FIRST_CLI, True)
                Else
                    LocalizzaCliente(CLI_CORRENTE, True)
                End If
            End If
            Session(SWOPCLI) = SWOPNESSUNA
            Session("TabClienti") = TB0
            TabContainer0.ActiveTabIndex = 0
        End If

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            WFPElencoCli.Show()
        End If

        Select Case Session(F_ELENCO_APERTA)
            Case F_NAZIONI
                WFPElencoNazioni.Show()
            Case F_PROVINCE
                WFPElencoProvince.Show()
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show()
            Case F_PAGAMENTI
                WFPElencoPagamenti.Show()
            Case F_AGENTI
                WFPElencoAgenti.Show()
            Case F_AGENTI_ESE_PREC
                WFPElencoAgentiEsePrec.Show()
            Case F_ZONE
                WFPElencoZone.Show()
            Case F_VETTORI
                WFPElencoVettori.Show()
            Case F_CATEGORIE
                WFPElencoCategorie.Show()
            Case F_LISTINO
                WFPElencoListVenT.Show()
            Case F_CONTI
                WFPElencoConti.Show()
            Case F_TIPOFATT
                WFPElencoTipoFatt.Show()
        End Select

        If Session(F_ANAGRZONE_APERTA) = True Then
            WFPZone.Show()
        End If
        If Session(F_ANAGRTIPOFATT_APERTA) = True Then
            WFPTipoFatturazione.Show()
        End If
        WFP_Vettori1.WucElement = Me
        If Session(F_ANAGRVETTORI_APERTA) = True Then
            WFP_Vettori1.Show()
        End If
        WFP_Agenti1.WucElement = Me
        If Session(F_ANAGRAGENTI_APERTA) = True Then
            WFP_Agenti1.Show()
        End If
        WFP_Categorie1.WucElement = Me
        If Session(F_ANAGRCATEGORIE_APERTA) = True Then
            WFP_Categorie1.Show()
        End If
        If Session(F_BANCHEIBAN_APERTA) = True Then
            WFP_BancheIBAN1.Show()
        End If

        If Session(CSTRitornoDaStampa) Then
            VisualizzaCliente()
            Session(CSTRitornoDaStampa) = False
        End If
    End Sub

    'giu241111
    Public Sub Chiudi(ByVal strErrore As String, Optional ByVal SwSessScaduta As Boolean = False)
        If SwSessScaduta = True Then
            Try
                If strErrore.Trim <> "" Then
                    Response.Redirect("..\Login.aspx?SessioneScaduta=" & strErrore.Trim)
                Else
                    Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                End If
            Catch ex As Exception
                If strErrore.Trim <> "" Then
                    Response.Redirect("..\Login.aspx?SessioneScaduta=" & strErrore.Trim)
                Else
                    Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                End If
            End Try
            Exit Sub
        End If
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
    End Sub

#Region "TextBox"
    Private Sub txtCodCliente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente.TextChanged
        Session(COD_CLIENTE) = txtCodCliente.Text.Trim
        If txtCodCliente.Text.Trim = "" Then
            txtRagSoc.Focus()
            Exit Sub
        End If

        If Session(SWOPCLI) = SWOPNUOVO Then
            If CheckNewCodiceCliente() = True Then
                txtCodCliente.BackColor = Def.SEGNALA_KO
                txtRagSoc.Focus()
                Exit Sub
            Else
                txtCodCliente.BackColor = Def.SEGNALA_OK
                txtRagSoc.Focus()
            End If
        End If
        txtRagSoc.Focus()
    End Sub

    Private Sub txtCodNazione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodNazione.TextChanged
        lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodNazione, lblNazione)
        If txtCodNazione.Text.Trim <> "I" And txtCodNazione.Text.Trim <> "IT" And txtCodNazione.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
    End Sub

    Private Sub txtCodAgente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAgente.TextChanged
        lblAgente.Text = App.GetValoreFromChiave(txtCodAgente.Text, Def.AGENTI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodAgente, lblAgente)
    End Sub

    Private Sub txtCodAgenteEsePrec_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAgenteEsePrec.TextChanged
        lblAgenteEsePrec.Text = App.GetValoreFromChiave(txtCodAgenteEsePrec.Text, Def.AGENTI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodAgenteEsePrec, lblAgenteEsePrec)
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        lblCategorie.Text = App.GetValoreFromChiave(txtCodCategoria.Text, Def.CATEGORIE, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodCategoria, lblCategorie)
    End Sub

    Private Sub txtCodListino_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodListino.TextChanged
        lblListino.Text = App.GetValoreFromChiave(txtCodListino.Text, Def.LISTVEN_T, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodListino, lblListino)
    End Sub

    Private Sub txtCodPagamento_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodPagamento.TextChanged
        lblPagamento.Text = App.GetValoreFromChiave(txtCodPagamento.Text, Def.PAGAMENTI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodPagamento, lblPagamento)
    End Sub

    Private Sub txtCodRegimeIVA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodRegimeIVA.TextChanged
        lblRegimeIva.Text = App.GetValoreFromChiave(txtCodRegimeIVA.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodRegimeIVA, lblRegimeIva, True)
    End Sub

    Private Sub txtCodRicavoFT_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodRicavoFT.TextChanged
        txtRicavoFT.Text = App.GetValoreFromChiave(txtCodRicavoFT.Text, Def.PIANODEICONTI, Session(ESERCIZIO))
        CheckInserimentoCodice(txtCodRicavoFT, txtRicavoFT)
    End Sub

    Private Sub txtCodVettore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodVettore.TextChanged
        lblVettore.Text = App.GetValoreFromChiave(txtCodVettore.Text, Def.VETTORI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodVettore, lblVettore)
    End Sub

    Private Sub txtCodZona_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodZona.TextChanged
        lblZona.Text = App.GetValoreFromChiave(txtCodZona.Text, Def.ZONE, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodZona, lblZona)
    End Sub

    Private Sub txtCodSede_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodSede.TextChanged
        lblSede.Text = App.GetValoreFromChiave(txtCodSede.Text, Def.CLIENTI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodSede, lblSede)
    End Sub

    Private Sub txtRagSoc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRagSoc.TextChanged
        CheckInserimentoCampoObbligatorio(txtRagSoc)
    End Sub

    Private Sub txtIndirizzo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIndirizzo.TextChanged
        CheckInserimentoCampoObbligatorio(txtIndirizzo)
        txtNumCivico.Focus()
    End Sub

    Private Sub txtLocalita_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLocalita.TextChanged
        CheckInserimentoCampoObbligatorio(txtLocalita)
        txtProvincia.Focus()
    End Sub

    Private Sub txtProvincia_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtProvincia.TextChanged
        txtProvincia.Text = App.GetValoreFromChiave(txtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO))
        CheckInserimentoCampoObbligatorio(txtProvincia)
        txtCap.Focus()
    End Sub

    Dim strErrore As String = ""
    Private Sub txtCodABI_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodABI.TextChanged
        lblBanca.Text = GetDatiBanche(txtCodABI.Text.Trim, strErrore).Banca
        If txtCodABI.Text.Trim <> "" And lblBanca.Text.Trim = "" Then
            lblBanca.Text = "[Inesistente]"
            If txtCodABI.Text.Trim = "" Then lblFiliale.Text = ""
            Exit Sub
        End If
        lblFiliale.Text = GetDatiFiliali(txtCodABI.Text.Trim, txtCodCAB.Text.Trim, strErrore).Filiale
        If txtCodABI.Text.Trim <> "" And lblFiliale.Text.Trim = "" Then
            lblFiliale.Text = "[Inesistente]"
        End If
        ' ''CheckInserimentoCodice(txtCodCAB, txtFiliale)
        ' ''CheckInserimentoCodice(txtCodABI, txtBanca)

        txtCodCAB.Focus()
    End Sub

    Private Sub txtCodCAB_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCAB.TextChanged
        lblFiliale.Text = GetDatiFiliali(txtCodABI.Text.Trim, txtCodCAB.Text.Trim, strErrore).Filiale
        If txtCodABI.Text.Trim <> "" And lblFiliale.Text.Trim = "" Then
            lblFiliale.Text = "[Inesistente]"
        End If
        ' ''CheckInserimentoCodice(txtCodCAB, txtFiliale)
        txtNumCC.Focus()
    End Sub

#End Region

#Region "Pulsanti"

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicerca.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti()
    End Sub

    Private Sub btnTrovaSede_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaSede.Click
        Session(F_CLI_RICERCA) = False
        ApriElencoClienti()
    End Sub

    Private Sub btnTrovaNazione_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaNazione.Click
        ApriElenco(F_NAZIONI)
    End Sub

    Private Sub btnTrovaProvincia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaProvincia.Click
        ApriElenco(F_PROVINCE)
    End Sub

    Private Sub btnTrovaRegimeIVA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaRegimeIVA.Click
        ApriElenco(F_ALIQUOTAIVA)
    End Sub

    Private Sub btnTrovaAgente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaAgente.Click
        ApriElenco(F_AGENTI)
    End Sub

    Private Sub btnTrovaAgenteEsePrec_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaAgenteEsePrec.Click
        ApriElenco(F_AGENTI_ESE_PREC)
    End Sub

    Private Sub btnTrovaZona_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaZona.Click
        ApriElenco(F_ZONE)
    End Sub

    Private Sub btnTrovaVettore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaVettore.Click
        ApriElenco(F_VETTORI)
    End Sub

    Private Sub btnTrovaCategoria_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaCategoria.Click
        ApriElenco(F_CATEGORIE)
    End Sub

    Private Sub btnTrovaListino_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaListino.Click
        ApriElenco(F_LISTINO)
    End Sub

    Private Sub btnTrovaRicavoFT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaRicavoFT.Click
        ApriElenco(F_CONTI)
    End Sub

    Private Sub btnTrovaPagamento_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaPagamento.Click
        ApriElenco(F_PAGAMENTI)
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        setPulsantiModalitaConsulta()
        CaricaVariabili()
        SvuotaCampi()
        Session(SWOPCLI) = SWOPNESSUNA
        CampiSetEnabledTo(False)
        LocalizzaCliente(CLI_CORRENTE)
        lblLabelNEW.Visible = False
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session("AvvisaDoppioCF") = True
        Session("AvvisaDoppioPI") = True
        'giu180722 Erika/Zibordi per le spedizioni almeno un tel è obbligatorio 
        '- per le dest.Merci se non presente lo prendo dall'anagrafica principale - magari con avviso nelle destinazioni
        Session("AvvisaNoTel") = True
        setPulsantiModalitaAggiorna()
        Session(SWOPCLI) = SWOPMODIFICA
        CampiSetEnabledTo(True)
        lblLabelNEW.Visible = False
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        Session("AvvisaDoppioCF") = True
        Session("AvvisaDoppioPI") = True
        Session("AvvisaNoTel") = True
        setPulsantiModalitaAggiorna()

        SvuotaCampi()
        'giu301212
        GridViewEC.Visible = False
        'giu150113
        TblScadenze.Visible = False
        lblDesTotScadenze.Visible = False
        lblTotScadenze.Visible = False
        '---------
        GridViewSC.Visible = False
        tblSaldi.Visible = False
        lblSaldoAttAvere.Visible = False
        lblSaldoAttDare.Visible = False
        lblSaldoPrecAvere.Visible = False
        lblSaldoPrecDare.Visible = False
        lblTotMovAvere.Visible = False
        lblTotMovDare.Visible = False
        '---------
        Session(SWOPCLI) = SWOPNUOVO
        CampiSetEnabledTo(True)
        TabContainer0.ActiveTabIndex = 0
        Session("TabClienti") = TabContainer0.ActiveTabIndex
        TabContainer1.ActiveTabIndex = 0
        txtCodCliente.Text = LocalizzaNEWCliente()
        Session(CSTCODCOGENEW) = txtCodCliente.Text 'GIU281111
        Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
        Session(COD_CLIENTE) = txtCodCliente.Text.Trim
        lblLabelNEW.Text = "Nuovo codice " & txtCodCliente.Text
        ' ''lblLabelNEW.Visible = True

        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim strErrAll As String = ""
        txtCodListino.Text = "1"
        If App.GetDatiAbilitazioni(CSTABILCOGE, "ListinoCli", strValore, strErrore) = True Then
            txtCodListino.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        lblListino.Text = App.GetValoreFromChiave(txtCodListino.Text, Def.LISTVEN_T, Session(ESERCIZIO))
        ' ''CheckInserimentoCodice(txtCodListino, txtListino)
        CheckInserimentoCodiceTL(txtCodListino, lblListino)
        txtCodNazione.Text = "I"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "NazioneCli", strValore, strErrore) = True Then
            txtCodNazione.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodNazione.Text.Trim <> "" Then
            lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
            ' ''CheckInserimentoCodice(txtCodNazione, txtNazione)
            CheckInserimentoCodiceTL(txtCodNazione, lblNazione)
        End If

        rdPersonaGiuridica.Checked = True
        txtDataNascita.Enabled = False
        imgBtnShowCalendar.Enabled = False
        txtCodRegimeIVA.Text = "0" : lblRegimeIva.Text = "REGIME NORMALE"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "RegIVACli", strValore, strErrore) = True Then
            txtCodRegimeIVA.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodRegimeIVA.Text.Trim <> "" Then
            lblRegimeIva.Text = App.GetValoreFromChiave(txtCodRegimeIVA.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
            ' ''CheckInserimentoCodice(txtCodRegimeIVA, txtRegimeIva)
            CheckInserimentoCodiceTL(txtCodRegimeIVA, lblRegimeIva)
        End If
        'giu251111 txtCodPagamento.Text = "35"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "CodPagCli", strValore, strErrore) = True Then
            txtCodPagamento.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodPagamento.Text.Trim <> "" Then
            lblPagamento.Text = App.GetValoreFromChiave(txtCodPagamento.Text, Def.PAGAMENTI, Session(ESERCIZIO))
            ' ''CheckInserimentoCodice(txtCodPagamento, txtPagamento)
            CheckInserimentoCodiceTL(txtCodPagamento, lblPagamento)
        End If
        '-----------------
        lblABI.Text = GetParamGestAzi(Session(ESERCIZIO)).ABI
        lblCAB.Text = GetParamGestAzi(Session(ESERCIZIO)).CAB
        lblIBAN.Text = GetParamGestAzi(Session(ESERCIZIO)).NazIBAN + GetParamGestAzi(Session(ESERCIZIO)).CINEUIBAN + GetParamGestAzi(Session(ESERCIZIO)).CIN + GetParamGestAzi(Session(ESERCIZIO)).ABI + GetParamGestAzi(Session(ESERCIZIO)).CAB + GetParamGestAzi(Session(ESERCIZIO)).CC
        lblContoCorrente.Text = GetParamGestAzi(Session(ESERCIZIO)).CC
        PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
        If DDLBancheIBAN.SelectedIndex = 0 Then
            DDLBancheIBAN.SelectedIndex = 0
            lblABI.Text = ""
            lblCAB.Text = ""
            lblIBAN.Text = ""
            lblContoCorrente.Text = ""
        End If

        txtRagSoc.Focus()
        Try
            If Session(CSTABILMSG) = SWSI Then
                If strErrAll.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Lettura Abilitazioni", strErrAll.Trim, WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
    'giu241111
    Private Function LocalizzaNEWCliente() As String
        Dim NewCodice As Long = 0
        Dim Format0 As String = "" : Dim Format9 As String = ""
        For i = 2 To txtCodCliente.MaxLength
            Format0 += "0" : Format9 += "9"
        Next
        Dim listaClienti As ArrayList = App.GetLista(Def.CLIENTI, Session(ESERCIZIO))
        Dim posizione As Integer = 0
        If (listaClienti.Count > 0) Then
            posizione = listaClienti.Count - 1
            If (posizione < 0) Then
                NewCodice = 0
            Else
                Dim Ultimo As String = listaClienti(posizione).Codice_CoGe
                NewCodice = CInt(Trim(Mid(Ultimo, 2)))
            End If
        Else
            NewCodice = 0
        End If
        NewCodice += 1
        If NewCodice > CInt(Format9) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo cliente", "Attenzione, superato il limite massimo nuovi codici clienti.", WUC_ModalPopup.TYPE_INFO)
            LocalizzaNEWCliente = ""
            Exit Function
        End If
        LocalizzaNEWCliente = "1" & Format(NewCodice, Format0)
    End Function

    Private Sub btnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click
        LocalizzaCliente(NEXT_CLI)
    End Sub

    Private Sub btnLast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLast.Click
        LocalizzaCliente(LAST_CLI)
    End Sub

    Private Sub btnPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrev.Click
        LocalizzaCliente(PREV_CLI)
    End Sub

    Private Sub btnFirst_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        LocalizzaCliente(FIRST_CLI)
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim myCliente As ClientiEntity = Nothing
        Dim errorMsg As String = String.Empty
        If Session(SWOPCLI) = SWOPNUOVO Then
            If CheckNewCodiceCliente() = True Then
                txtCodCliente.BackColor = Def.SEGNALA_KO
                Exit Sub
            Else
                txtCodCliente.BackColor = Def.SEGNALA_OK
            End If
        End If
        Dim strElErrori As String = ""
        ControlloCampiObbligatori(strElErrori)
        If (Session(SWERRORI_AGGIORNAMENTO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati cliente", "Attenzione, i campi segnalati in rosso non sono validi: <br>" & strElErrori, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        InitCampi(myCliente)
        ControllaEsistenzaCampiInErrore(strElErrori)
        If (Session(SWERRORI_AGGIORNAMENTO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati cliente", "Attenzione, i campi segnalati in rosso non sono validi: <br>" & strElErrori, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If (ControlloCampiDati()) Then 'giu090113 c'è il controllo del C.F. e P.I. SE SONO DOPPI (STAMPA)
            Aggiornamento(myCliente)
            setPulsantiModalitaConsulta()
            If (Not App.CaricaClienti(Session(ESERCIZIO), errorMsg)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Caricamento Dati", _
                    String.Format("Errore nel caricamento Elenco Clienti, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_INFO)
                SessionUtility.LogOutUtente(Session, Response)
                Exit Sub
            End If
            LocalizzaCliente(CLI_CORRENTE)
            lblLabelNEW.Visible = False
            'giu180912 giu101012 aggiorna MySQL Clienti per IREDEEM
            If App.GetDatiAbilitazioni(CSTABILCOGE, "MySQLCli" + Session(ESERCIZIO).ToString.Trim, "", strErrore) = True Then
                'ok chiamo la SP per aggiornare ma senza TRANSAZIONE
                'Session(COD_CLIENTE)
                If DataBaseUtility.MySQLClienti(Session(COD_CLIENTE), CSTAttivo, errorMsg) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Aggiorna dati cliente (MySQL)", _
                        String.Format("Errore, contattare l'amministratore di sistema. Errore: {0}", errorMsg), _
                        WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
            '--------------------------------------------
        Else
            Exit Sub
        End If
        Session(SWOPCLI) = SWOPNESSUNA
    End Sub

    Dim CIClienteCG As Boolean = True : Dim CIClienteAZI As Boolean = True : Dim CIClienteSCAD As Boolean = True
    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click

        CIClienteCG = True : CIClienteAZI = True : CIClienteSCAD = True
        Dim Cliys As New Clienti
        CIClienteCG = Cliys.CIClienteByCodice(Session(COD_CLIENTE))
        CIClienteAZI = Cliys.CIClienteByCodiceAZI(Session(COD_CLIENTE))
        CIClienteSCAD = Cliys.CIClienteByCodiceSCAD(Session(COD_CLIENTE))
        If CIClienteCG = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella contabilità, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CIClienteAZI = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella gestione aziendale, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CIClienteSCAD = False Then 'GIU261111 IDEM PER AZI NEI DOCUMENTI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nello scadenzario, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfermaEliminaCliente"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Elimina cliente", "Si vuole cancellare il cliente selezionato?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub

#End Region

#Region "Radio button"

    Private Sub rdPersonaFisica_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdPersonaFisica.CheckedChanged
        If (rdPersonaFisica.Checked) Then
            txtDataNascita.Enabled = True
            imgBtnShowCalendar.Enabled = True
            txtDataNascita_CalendarExtender.Enabled = True
            txtCodSede.Text = String.Empty
            lblSede.Text = ""
            checkAllIVA.Checked = False
            '-
            btnTrovaSede.Enabled = False
            txtCodSede.Enabled = False
            checkAllIVA.Enabled = False
        End If
    End Sub

    Private Sub rdPersonaGiuridica_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdPersonaGiuridica.CheckedChanged
        If (rdPersonaGiuridica.Checked) Then
            txtDataNascita.Text = ""
            txtDataNascita.Enabled = False
            imgBtnShowCalendar.Enabled = False
            txtDataNascita_CalendarExtender.Enabled = False
            '-
            btnTrovaSede.Enabled = True
            txtCodSede.Enabled = True
            checkAllIVA.Enabled = True
        End If
    End Sub

#End Region

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SWOPCLI) = SWOPNESSUNA
        Session(F_CLI_RICERCA) = False
        Session(SWERRORI_AGGIORNAMENTO) = False
    End Sub

#End Region

#Region "Metodi private"

#Region "Gestione Campi"

    Private Sub CampiSetEnabledTo(ByVal valore As Boolean)
        '--- intestazione ---
        If valore = True Then
            If Session(SWOPCLI) = SWOPNUOVO Then
                txtCodCliente.Enabled = valore
            End If
        Else
            txtCodCliente.Enabled = valore
        End If
        txtIPA.Enabled = valore : txtCMabell.Enabled = valore
        checkSpliIVA.Enabled = valore
        txtRagSoc.Enabled = valore
        '--- dati anagrafici ---
        txtDenominazione.Enabled = valore
        txtTitolare.Enabled = valore
        txtIndirizzo.Enabled = valore
        txtLocalita.Enabled = valore
        txtNumCivico.Enabled = valore
        txtProvincia.Enabled = valore
        txtCap.Enabled = valore
        txtCodNazione.Enabled = valore
        txtPartitaIVA.Enabled = valore
        txtTelefono1.Enabled = valore
        txtTelefono2.Enabled = valore
        txtTelefonoFax.Enabled = valore
        txtCodFiscale.Enabled = valore
        txtEmail.Enabled = valore
        txtPECEMail.Enabled = valore
        txtEmailInvioFatt.Enabled = valore
        txtEmailInvioScad.Enabled = valore
        CheckInvioMailScad.Enabled = valore
        txtRiferimento.Enabled = valore

        If valore = True Then
            If (rdPersonaGiuridica.Checked) Then
                txtDataNascita.Enabled = False
                btnTrovaSede.Enabled = valore
                txtCodSede.Enabled = valore
                checkAllIVA.Enabled = valore
            Else
                txtDataNascita.Enabled = valore
                btnTrovaSede.Enabled = False
                txtCodSede.Enabled = False
                checkAllIVA.Enabled = False
            End If
        Else
            txtDataNascita.Enabled = valore
            btnTrovaSede.Enabled = False
            txtCodSede.Enabled = False
            checkAllIVA.Enabled = False
        End If
        
        btnTrovaProvincia.Enabled = valore
        btnTrovaNazione.Enabled = valore
        rdPersonaFisica.Enabled = valore
        rdPersonaGiuridica.Enabled = valore
        '--- commerciale ---
        txtCodRegimeIVA.Enabled = valore
        txtCodPagamento.Enabled = valore
        txtCodABI.Enabled = valore
        txtCodCAB.Enabled = valore
        txtNumCC.Enabled = valore
        txtCIN.Enabled = valore
        'giu150212
        DDLBancheIBAN.Enabled = valore
        btnCercaBanca.Enabled = valore
        txtCodAgente.Enabled = valore
        txtCodAgenteEsePrec.Enabled = valore
        txtCodZona.Enabled = valore
        txtCodVettore.Enabled = valore
        txtCodCategoria.Enabled = valore
        txtCodListino.Enabled = valore
        txtCodRicavoFT.Enabled = valore
        txtMaxCredito.Enabled = valore
        btnTrovaRegimeIVA.Enabled = valore
        btnTrovaPagamento.Enabled = valore
        btnTrovaAgente.Enabled = valore
        btnAgente.Enabled = valore
        btnTrovaAgenteEsePrec.Enabled = valore
        btnAgenteEsePrec.Enabled = valore
        btnTrovaZona.Enabled = valore
        btnZone.Enabled = valore
        btnTrovaVettore.Enabled = valore
        btnVettori.Enabled = valore
        btnTrovaCategoria.Enabled = valore
        btnCategorie.Enabled = valore
        btnTrovaListino.Enabled = valore
        btnTrovaRicavoFT.Enabled = valore
        btnTrovaBanca.Enabled = valore
        checkEscudiAllegatiIVA.Enabled = valore
        checkIVAInSospensione.Enabled = valore
        checkNonFatturabile.Enabled = valore
        checkMattino1.Enabled = valore
        checkMattino2.Enabled = valore
        checkPomeriggio1.Enabled = valore
        checkPomeriggio2.Enabled = valore
        ddlPrimoGiorno1.Enabled = valore
        ddlPrimoGiorno2.Enabled = valore
        '--- note ---
        txtNote.Enabled = valore
        chkSegnalaNote.Enabled = valore
        'alb18/12/2012
        txtCodTipoFatt.Enabled = valore
        btnTrovaTipoFatt.Enabled = valore
        btnGestTipoFatt.Enabled = valore
        '----
    End Sub

    Private Sub SvuotaCampi()
        '--- intestazione ---                           
        ResetCampoStringaValidato(txtCodCliente) : txtCodCliente.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtIPA) : txtIPA.BackColor = SEGNALA_OK
        checkSpliIVA.Checked = False
        ResetCampoStringaValidato(txtCMabell) : txtCMabell.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtRagSoc) : txtRagSoc.BackColor = SEGNALA_OK
        '--- dati anagrafici ---                        
        ResetCampoStringaValidato(txtDenominazione) : txtDenominazione.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtTitolare) : txtTitolare.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtIndirizzo) : txtIndirizzo.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtLocalita) : txtLocalita.BackColor = SEGNALA_OK
        txtNumCivico.Text = String.Empty : txtNumCivico.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtProvincia) : txtProvincia.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCap) : txtCap.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCodNazione) : txtCodNazione.BackColor = SEGNALA_OK
        lblNazione.Text = ""
        ResetCampoStringaValidato(txtPartitaIVA) : txtPartitaIVA.BackColor = SEGNALA_OK
        txtTelefono1.Text = String.Empty : txtTelefono1.BackColor = SEGNALA_OK
        txtTelefono2.Text = String.Empty : txtTelefono2.BackColor = SEGNALA_OK
        txtTelefonoFax.Text = String.Empty : txtTelefonoFax.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCodFiscale) : txtCodFiscale.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtEmail) : txtEmail.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtPECEMail) : txtPECEMail.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtEmailInvioFatt) : txtEmailInvioFatt.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtEmailInvioScad) : txtEmailInvioScad.BackColor = SEGNALA_OK
        CheckInvioMailScad.AutoPostBack = False
        CheckInvioMailScad.Checked = True
        CheckInvioMailScad.AutoPostBack = True
        txtRiferimento.Text = String.Empty : txtRiferimento.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCodSede) : txtCodSede.BackColor = SEGNALA_OK
        lblSede.Text = "" 'GIU121211
        checkAllIVA.Checked = False
        ResetCampoStringaValidato(txtDataNascita) : txtDataNascita.BackColor = SEGNALA_OK
        rdPersonaGiuridica.AutoPostBack = False : rdPersonaFisica.AutoPostBack = False
        rdPersonaGiuridica.Checked = True
        rdPersonaFisica.Checked = False
        rdPersonaGiuridica.AutoPostBack = True : rdPersonaFisica.AutoPostBack = True
        '--- commerciale ---                            
        ResetCampoStringaValidato(txtCodRegimeIVA) : txtCodRegimeIVA.BackColor = SEGNALA_OK
        lblRegimeIva.Text = ""
        ResetCampoStringaValidato(txtCodPagamento) : txtCodPagamento.BackColor = SEGNALA_OK
        lblPagamento.Text = ""
        ResetCampoStringaValidato(txtCodABI) : txtCodABI.BackColor = SEGNALA_OK
        lblBanca.Text = ""
        ResetCampoStringaValidato(txtCodCAB) : txtCodCAB.BackColor = SEGNALA_OK
        lblFiliale.Text = ""
        ResetCampoStringaValidato(txtNumCC) : txtNumCC.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCIN) : txtCIN.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtCodAgente) : txtCodAgente.BackColor = SEGNALA_OK
        lblAgente.Text = ""
        ResetCampoStringaValidato(txtCodAgenteEsePrec) : txtCodAgenteEsePrec.BackColor = SEGNALA_OK
        lblAgenteEsePrec.Text = ""
        ResetCampoStringaValidato(txtCodZona) : txtCodZona.BackColor = SEGNALA_OK
        lblZona.Text = ""
        ResetCampoStringaValidato(txtCodVettore) : txtCodVettore.BackColor = SEGNALA_OK
        lblVettore.Text = ""
        ResetCampoStringaValidato(txtCodCategoria) : txtCodCategoria.BackColor = SEGNALA_OK
        lblCategorie.Text = ""
        ResetCampoStringaValidato(txtCodListino) : txtCodListino.BackColor = SEGNALA_OK
        lblListino.Text = ""
        ResetCampoStringaValidato(txtCodTipoFatt) : txtCodTipoFatt.BackColor = SEGNALA_OK 'ALBERTO 19/12/2012
        lblTipoFatt.Text = "" 'ALBERTO 19/12/2012
        ResetCampoStringaValidato(txtCodRicavoFT) : txtCodRicavoFT.BackColor = SEGNALA_OK
        ResetCampoStringaValidato(txtRicavoFT) : txtRicavoFT.BackColor = SEGNALA_OK
        txtMaxCredito.Text = FormattaNumero("0", 2) : txtMaxCredito.BackColor = SEGNALA_OK
        checkEscudiAllegatiIVA.Checked = False
        checkIVAInSospensione.Checked = False
        checkNonFatturabile.Checked = False
        checkMattino1.Checked = False
        checkMattino2.Checked = False
        checkPomeriggio1.Checked = False
        checkPomeriggio2.Checked = False
        ddlPrimoGiorno1.SelectedIndex = 0
        ddlPrimoGiorno2.SelectedIndex = 0
        '--- saldi ---                                  
        lblSaldoAperturaAvere.Text = FormattaNumero("0", 2)
        lblSaldoAperturaDare.Text = FormattaNumero("0", 2)
        lblSaldoAttualeAvere.Text = FormattaNumero("0", 2)
        lblSaldoAttualeDare.Text = FormattaNumero("0", 2)
        lblSaldoChiusuraAvere.Text = FormattaNumero("0", 2)
        lblSaldoChiusuraDare.Text = FormattaNumero("0", 2)
        lblSaldoProgAver.Text = FormattaNumero("0", 2)
        lblSaldoProgDare.Text = FormattaNumero("0", 2)
        lblDataSaldoUltimoAgg.Text = ""
        '--- note ---                                   
        txtNote.Text = String.Empty
        chkSegnalaNote.Checked = False
    End Sub

    Private Sub PopolaCampi(ByVal Cliente As ClientiEntity)
        SvuotaCampi()
        '--- intestazione ---
        txtCodCliente.Text = Cliente.Codice_CoGe
        Session(COD_CLIENTE) = txtCodCliente.Text.Trim
        txtIPA.Text = Cliente.IPA
        checkSpliIVA.Checked = IIf(Cliente.SplitIVA = 0, False, True)
        txtCMabell.Text = Cliente.CodiceMABELL
        '---
        txtRagSoc.Text = Cliente.Rag_Soc
        '--- dati anagrafici ---
        txtDenominazione.Text = Cliente.Denominazione
        txtTitolare.Text = Cliente.Titolare
        txtIndirizzo.Text = Cliente.Indirizzo
        txtLocalita.Text = Cliente.Localita
        txtNumCivico.Text = Cliente.NumeroCivico
        txtProvincia.Text = Cliente.Provincia

        txtCap.Text = Cliente.CAP
        txtCodNazione.Text = Cliente.Nazione
        lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
        If txtCodNazione.Text.Trim <> "I" And txtCodNazione.Text.Trim <> "IT" And txtCodNazione.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
        txtPartitaIVA.Text = Cliente.Partita_IVA
        txtTelefono1.Text = Cliente.Telefono1
        txtTelefono2.Text = Cliente.Telefono2
        txtTelefonoFax.Text = Cliente.Fax
        txtCodFiscale.Text = Cliente.Codice_Fiscale
        txtEmail.Text = Cliente.Email.Trim
        txtPECEMail.Text = Cliente.PECEmail.Trim
        txtEmailInvioFatt.Text = Cliente.EmailInvioFatt.Trim
        txtEmailInvioScad.Text = Cliente.EmailInvioScad.Trim
        CheckInvioMailScad.Checked = IIf(Cliente.InvioMailScad = 0, False, True)
        txtRiferimento.Text = Cliente.Riferimento
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        txtCodSede.Text = Cliente.Codice_SEDE
        'giu121211
        If txtCodSede.Text.Trim <> "" Then
            lblSede.Text = App.GetValoreFromChiave(txtCodSede.Text, Def.CLIENTI, Session(ESERCIZIO))
        Else
            lblSede.Text = ""
        End If
        'giu121211------------------
        txtDataNascita.Text = IIf(Cliente.Data_Nascita = CDate(DATANULL), "", Format(Cliente.Data_Nascita, FormatoData)) 'GIU231011
        checkAllIVA.Checked = Cliente.CSAggrAllIVA
        If CBool(Cliente.Societa) Then
            rdPersonaGiuridica.Checked = True
            rdPersonaFisica.Checked = False
            txtDataNascita.Enabled = False
            'qui no 
            ' ''btnTrovaSede.Enabled = True
            ' ''checkAllIVA.Enabled = True
        Else
            rdPersonaGiuridica.Checked = False
            rdPersonaFisica.Checked = True
            txtDataNascita.Enabled = True
            btnTrovaSede.Enabled = False
            checkAllIVA.Enabled = False
            checkAllIVA.Checked = False
        End If
        '--- commerciale ---
        txtCodRegimeIVA.Text = Cliente.Regime_IVA
        lblRegimeIva.Text = App.GetValoreFromChiave(txtCodRegimeIVA.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
        txtCodPagamento.Text = Cliente.Pagamento_N
        lblPagamento.Text = App.GetValoreFromChiave(txtCodPagamento.Text, Def.PAGAMENTI, Session(ESERCIZIO))
        txtCodABI.Text = Cliente.ABI_N.Trim
        txtCodCAB.Text = Cliente.CAB_N.Trim
        'GIU251111
        lblBanca.Text = GetDatiBanche(txtCodABI.Text.Trim, strErrore).Banca
        lblFiliale.Text = GetDatiFiliali(txtCodABI.Text.Trim, txtCodCAB.Text.Trim, strErrore).Filiale
        If txtCodABI.Text.Trim <> "" And lblBanca.Text.Trim = "" Then
            lblBanca.Text = "[Inesistente]"
        End If
        If txtCodABI.Text.Trim <> "" And lblFiliale.Text.Trim = "" Then
            lblFiliale.Text = "[Inesistente]"
        End If
        '------------
        txtNumCC.Text = Cliente.Conto_Corrente
        txtCIN.Text = Cliente.CIN
        'giu150212
        lblIBAN.Text = Cliente.IBAN_Ditta
        lblABI.Text = Mid(Trim(lblIBAN.Text), 6, 5)
        lblCAB.Text = Mid(Trim(lblIBAN.Text), 11, 5)
        lblContoCorrente.Text = Mid(Trim(lblIBAN.Text), 16)
        PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
        If DDLBancheIBAN.SelectedIndex = 0 Then
            DDLBancheIBAN.SelectedIndex = 0
            lblABI.Text = ""
            lblCAB.Text = ""
            lblIBAN.Text = ""
            lblContoCorrente.Text = ""
        End If
        '---------
        txtCodAgente.Text = Cliente.Agente_N
        lblAgente.Text = App.GetValoreFromChiave(txtCodAgente.Text, Def.AGENTI, Session(ESERCIZIO))
        txtCodAgenteEsePrec.Text = Cliente.Agente_N_Prec
        lblAgenteEsePrec.Text = App.GetValoreFromChiave(txtCodAgenteEsePrec.Text, Def.AGENTI, Session(ESERCIZIO))
        txtCodZona.Text = Cliente.Zona
        lblZona.Text = App.GetValoreFromChiave(txtCodZona.Text, Def.ZONE, Session(ESERCIZIO))
        txtCodVettore.Text = Cliente.Vettore_N
        lblVettore.Text = App.GetValoreFromChiave(txtCodVettore.Text, Def.VETTORI, Session(ESERCIZIO))
        txtCodCategoria.Text = Cliente.Categoria
        lblCategorie.Text = App.GetValoreFromChiave(txtCodCategoria.Text, Def.CATEGORIE, Session(ESERCIZIO))
        txtCodListino.Text = Cliente.Listino
        lblListino.Text = App.GetValoreFromChiave(txtCodListino.Text, Def.LISTVEN_T, Session(ESERCIZIO))
        txtCodTipoFatt.Text = IIf(IsDBNull(Cliente.TipoFatt), "", Cliente.TipoFatt) 'ALBERTO 19/12/2012
        lblTipoFatt.Text = App.GetValoreFromChiave(txtCodTipoFatt.Text, Def.TIPOFATT, Session(ESERCIZIO)) 'ALBERTO 19/12/2012
        txtCodRicavoFT.Text = Cliente.Codice_Ricavo
        txtRicavoFT.Text = App.GetValoreFromChiave(txtCodRicavoFT.Text, Def.PIANODEICONTI, Session(ESERCIZIO))
        txtMaxCredito.Text = Format(Cliente.Credito_1, FormatoValEuro)
        checkEscudiAllegatiIVA.Checked = IIf(Cliente.Allegato_IVA = 0, False, True)
        checkIVAInSospensione.Checked = IIf(Cliente.IVASosp = 0, False, True)
        checkNonFatturabile.Checked = IIf(Cliente.NoFatt = 0, False, True)
        checkMattino1.Checked = IIf(Cliente.ChiusuraMattino_1 = 0, False, True)
        checkMattino2.Checked = IIf(Cliente.ChiusuraMattino_2 = 0, False, True)
        checkPomeriggio1.Checked = IIf(Cliente.ChiusuraPomeriggio_1 = 0, False, True)
        checkPomeriggio2.Checked = IIf(Cliente.ChiusuraPomeriggio_2 = 0, False, True)
        ddlPrimoGiorno1.SelectedIndex = Cliente.GiornoChiusura_1
        ddlPrimoGiorno2.SelectedIndex = Cliente.GiornoChiusura_2
        '--- saldi ---
        'GIU060113
        lblSaldoAperturaDare.Text = Format(0, FormatoValEuro)
        lblSaldoAperturaAvere.Text = Format(0, FormatoValEuro)
        If Not IsDBNull(Cliente.DA_Apertura) Then
            If Cliente.DA_Apertura.Trim <> "" Then
                If Cliente.DA_Apertura.Trim = "D" Then
                    lblSaldoAperturaDare.Text = Format(Cliente.Apertura, FormatoValEuro)
                ElseIf Cliente.DA_Apertura.Trim = "A" Then
                    lblSaldoAperturaAvere.Text = Format(Cliente.Apertura, FormatoValEuro)
                End If
            End If
        End If
        '---------
        lblSaldoAttualeAvere.Text = Format(Cliente.Avere_Chiusura, FormatoValEuro)
        lblSaldoAttualeDare.Text = Format(Cliente.Dare_Chiusura, FormatoValEuro)
        lblSaldoChiusuraAvere.Text = Format(Cliente.Avere_Chiusura, FormatoValEuro)
        lblSaldoChiusuraDare.Text = Format(Cliente.Dare_Chiusura, FormatoValEuro)
        lblSaldoProgAver.Text = Format(Cliente.Saldo_Avere, FormatoValEuro)
        lblSaldoProgDare.Text = Format(Cliente.Saldo_Dare, FormatoValEuro)
        'alb28/12/2012  giu281212 FormattaNumero("0", 2)
        If lblSaldoProgAver.Text.Trim <> "" And IsNumeric(lblSaldoProgAver.Text.Trim) And lblSaldoProgDare.Text.Trim <> "" And IsNumeric(lblSaldoProgDare.Text.Trim) Then
            If CDec(lblSaldoProgAver.Text.Trim) > CDec(lblSaldoProgDare.Text.Trim) Then
                lblSaldoAttualeAvere.Text = FormattaNumero(CDec(lblSaldoProgAver.Text.Trim) - CDec(lblSaldoProgDare.Text.Trim), 2)
            Else
                lblSaldoAttualeDare.Text = FormattaNumero(CDec(lblSaldoProgDare.Text.Trim) - CDec(lblSaldoProgAver.Text.Trim), 2)
            End If
        End If
        '----------------
        If CDate(Cliente.Data_Agg_Saldi) = DATANULL Then
            lblDataSaldoUltimoAgg.Text = ""
        Else
            lblDataSaldoUltimoAgg.Text = Format(Cliente.Data_Agg_Saldi, FormatoData)
        End If

        '--- note ---
        txtNote.Text = Cliente.Note
        If IsDBNull(Cliente.DEM) Then
            chkSegnalaNote.Checked = False
        Else
            chkSegnalaNote.Checked = CBool(Cliente.DEM)
        End If

        'giu080412
        Dim myCAge As String = txtCodAgente.Text.Trim
        If Not IsNumeric(myCAge) Then myCAge = "0"
        Dim _CAgeAssegnati As String = ""
        If CtrAgente(txtProvincia.Text.Trim, CInt(myCAge), _CAgeAssegnati) = False Then
            lblMessAge.ForeColor = SEGNALA_KO
            lblMessAge.Text = _CAgeAssegnati.ToString.Trim
        Else
            lblMessAge.ForeColor = Drawing.Color.Blue
            lblMessAge.Text = _CAgeAssegnati.ToString.Trim
        End If
        '---------
    End Sub

    Private Function InitCampi(ByRef myCliente As ClientiEntity) As Boolean
        myCliente = New ClientiEntity

        myCliente.Codice_CoGe = txtCodCliente.Text
        myCliente.IPA = txtIPA.Text
        myCliente.SplitIVA = checkSpliIVA.Checked
        myCliente.CodiceMABELL = txtCMabell.Text
        '------
        myCliente.Rag_Soc = txtRagSoc.Text
        myCliente.Denominazione = txtDenominazione.Text
        myCliente.Titolare = txtTitolare.Text
        myCliente.Indirizzo = txtIndirizzo.Text
        myCliente.Localita = txtLocalita.Text
        myCliente.NumeroCivico = txtNumCivico.Text
        myCliente.Provincia = txtProvincia.Text
        myCliente.CAP = txtCap.Text
        myCliente.Nazione = txtCodNazione.Text
        myCliente.Partita_IVA = txtPartitaIVA.Text
        myCliente.Telefono1 = txtTelefono1.Text
        myCliente.Telefono2 = txtTelefono2.Text
        myCliente.Fax = txtTelefonoFax.Text
        myCliente.Codice_Fiscale = txtCodFiscale.Text
        myCliente.Email = txtEmail.Text
        myCliente.PECEmail = txtPECEMail.Text
        myCliente.EmailInvioFatt = txtEmailInvioFatt.Text
        myCliente.EmailInvioScad = txtEmailInvioScad.Text
        myCliente.InvioMailScad = CheckInvioMailScad.Checked
        myCliente.Riferimento = txtRiferimento.Text
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        If (rdPersonaGiuridica.Checked) Then
            myCliente.Societa = -1
        Else
            myCliente.Societa = 0
        End If
        myCliente.Data_Nascita = GetDataValida(txtDataNascita)
        myCliente.CSAggrAllIVA = checkAllIVA.Checked
        myCliente.Regime_IVA = GetCodiceNumericoValido(txtCodRegimeIVA)
        myCliente.Pagamento_N = GetCodiceNumericoValido(txtCodPagamento)
        myCliente.ABI_N = txtCodABI.Text
        myCliente.CAB_N = txtCodCAB.Text
        myCliente.Conto_Corrente = txtNumCC.Text
        myCliente.CIN = txtCIN.Text
        myCliente.IBAN_Ditta = lblIBAN.Text.Trim
        myCliente.Agente_N = GetCodiceNumericoValido(txtCodAgente)
        myCliente.Agente_N_Prec = GetCodiceNumericoValido(txtCodAgenteEsePrec)
        myCliente.Zona = GetCodiceNumericoValido(txtCodZona)
        myCliente.Vettore_N = GetCodiceNumericoValido(txtCodVettore)
        myCliente.Categoria = GetCodiceNumericoValido(txtCodCategoria)
        myCliente.Listino = GetCodiceNumericoValido(txtCodListino)
        myCliente.Codice_Ricavo = txtCodRicavoFT.Text
        myCliente.Credito_1 = CDec(txtMaxCredito.Text)
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        myCliente.Codice_SEDE = txtCodSede.Text.Trim
        myCliente.Allegato_IVA = IIf(checkEscudiAllegatiIVA.Checked, -1, 0)
        myCliente.IVASosp = IIf(checkIVAInSospensione.Checked, -1, 0)
        myCliente.NoFatt = IIf(checkNonFatturabile.Checked, -1, 0)
        myCliente.ChiusuraMattino_1 = IIf(checkMattino1.Checked, -1, 0)
        myCliente.ChiusuraMattino_2 = IIf(checkMattino2.Checked, -1, 0)
        myCliente.ChiusuraPomeriggio_1 = IIf(checkPomeriggio1.Checked, -1, 0)
        myCliente.ChiusuraPomeriggio_2 = IIf(checkPomeriggio2.Checked, -1, 0)
        myCliente.GiornoChiusura_1 = ddlPrimoGiorno1.SelectedIndex
        myCliente.GiornoChiusura_2 = ddlPrimoGiorno2.SelectedIndex
        'giu251111 DATO NON MODIFICABILE DA QUI
        ' ''myCliente.Apertura = CDec(txtSaldoAperturaAvere.Text)
        ' ''myCliente.Apertura_2 = CDec(txtSaldoAperturaDare.Text)
        ' ''myCliente.Avere_Chiusura = CDec(txtSaldoAttualeAvere.Text)
        ' ''myCliente.Dare_Chiusura = CDec(txtSaldoAttualeDare.Text)
        ' ''myCliente.Avere_Chiusura = CDec(txtSaldoChiusuraAvere.Text)
        ' ''myCliente.Dare_Chiusura = CDec(txtSaldoChiusuraDare.Text)
        ' ''myCliente.Saldo_Avere = CDec(txtSaldoProgAver.Text)
        ' ''myCliente.Saldo_Dare = CDec(txtSaldoProgDare.Text)
        ' ''myCliente.Data_Agg_Saldi = GetDataValida(txtDataSaldoUltimoAgg)
        myCliente.Note = txtNote.Text
        myCliente.DEM = chkSegnalaNote.Checked
        myCliente.Ragg_P = App.GetProgressiviCoGe(Session(ESERCIZIO)).Ragg_Bil_Clienti
        myCliente.TipoFatt = txtCodTipoFatt.Text
        If Session(SWOPCLI) = SWOPNUOVO Then
            myCliente.InseritoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Else
            myCliente.ModificatoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        End If
    End Function

    Private Function GetCodiceNumericoValido(ByRef txtCod As TextBox) As Integer
        If (IsNumeric(txtCod.Text)) Then
            GetCodiceNumericoValido = CInt(txtCod.Text)
            txtCod.BackColor = Def.SEGNALA_OK
        ElseIf (String.IsNullOrEmpty(txtCod.Text)) Then
            GetCodiceNumericoValido = 0
            txtCod.BackColor = Def.SEGNALA_OK
        Else
            GetCodiceNumericoValido = -1
            txtCod.BackColor = Def.SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Function

    Private Function GetDataValida(ByRef txtData As TextBox) As Date
        If (IsDate(txtData.Text)) Then
            GetDataValida = CDate(txtData.Text)
            txtData.BackColor = Def.SEGNALA_OK
        ElseIf (String.IsNullOrEmpty(txtData.Text)) Then
            GetDataValida = Def.DATANULL
            txtData.BackColor = Def.SEGNALA_OK
        Else
            GetDataValida = Def.DATANULL
            txtData.BackColor = Def.SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Function

    Private Sub ControllaEsistenzaCampiInErrore(ByRef _ElErr As String)
        _ElErr = ""
        Session(SWERRORI_AGGIORNAMENTO) = False
        'giu180912 tolto errore bloccante sull'agente
        'lblMessAge.BackColor = Def.SEGNALA_KO _
        If (txtCodNazione.BackColor = Def.SEGNALA_KO Or _
            txtProvincia.BackColor = Def.SEGNALA_KO Or _
            txtCodSede.BackColor = Def.SEGNALA_KO Or _
            txtCodRegimeIVA.BackColor = Def.SEGNALA_KO Or _
            txtCodPagamento.BackColor = Def.SEGNALA_KO Or _
            txtCodABI.BackColor = Def.SEGNALA_KO Or _
            txtCodCAB.BackColor = Def.SEGNALA_KO Or _
            txtCodAgente.BackColor = Def.SEGNALA_KO Or _
            txtCodAgenteEsePrec.BackColor = Def.SEGNALA_KO Or _
            txtCodZona.BackColor = Def.SEGNALA_KO Or _
            txtCodVettore.BackColor = Def.SEGNALA_KO Or _
            txtCodCategoria.BackColor = Def.SEGNALA_KO Or _
            txtCodListino.BackColor = Def.SEGNALA_KO Or _
            txtCodRicavoFT.BackColor = Def.SEGNALA_KO Or _
            txtCodTipoFatt.BackColor = Def.SEGNALA_KO Or _
            txtEmail.BackColor = Def.SEGNALA_KO Or _
            txtEmailInvioFatt.BackColor = Def.SEGNALA_KO Or _
            txtEmailInvioScad.BackColor = Def.SEGNALA_KO Or _
            txtPECEMail.BackColor = Def.SEGNALA_KO) Then

            Session(SWERRORI_AGGIORNAMENTO) = True
            If txtCodNazione.BackColor = Def.SEGNALA_KO Then _ElErr += " - Nazione"
            If txtProvincia.BackColor = Def.SEGNALA_KO Then _ElErr += " - Provincia"
            If txtCodSede.BackColor = Def.SEGNALA_KO Then _ElErr += " - Sede"
            If txtCodRegimeIVA.BackColor = Def.SEGNALA_KO Then _ElErr += " - Regime IVA"
            If txtCodPagamento.BackColor = Def.SEGNALA_KO Then _ElErr += " - Pagamento"
            If txtCodABI.BackColor = Def.SEGNALA_KO Then _ElErr += " - ABI"
            If txtCodCAB.BackColor = Def.SEGNALA_KO Then _ElErr += " - CAB"
            If txtCodAgente.BackColor = Def.SEGNALA_KO Then _ElErr += " - Agente"
            If txtCodAgenteEsePrec.BackColor = Def.SEGNALA_KO Then _ElErr += " - Agente Esercizio precedente"
            If txtCodZona.BackColor = Def.SEGNALA_KO Then _ElErr += " - Zona"
            If txtCodVettore.BackColor = Def.SEGNALA_KO Then _ElErr += " - Vettore"
            If txtCodCategoria.BackColor = Def.SEGNALA_KO Then _ElErr += " - Categoria"
            If txtCodListino.BackColor = Def.SEGNALA_KO Then _ElErr += " - Listino"
            If txtCodRicavoFT.BackColor = Def.SEGNALA_KO Then _ElErr += " - Cod. Ricavo"
            If txtCodTipoFatt.BackColor = Def.SEGNALA_KO Then _ElErr += " - Cod. Tipo fatturazione"
            If txtEmail.BackColor = Def.SEGNALA_KO Or _
                txtEmailInvioFatt.BackColor = Def.SEGNALA_KO Or _
                txtEmailInvioScad.BackColor = Def.SEGNALA_KO Or _
                txtPECEMail.BackColor = Def.SEGNALA_KO Then _ElErr += " - E-mail"
            'giu180912 tolto errore bloccante sull'agente
            'If lblMessAge.ForeColor = Def.SEGNALA_KO Then _ElErr += " - Agente assegnato"
        End If
    End Sub

    Private Sub ControlloCampiObbligatori(ByRef _ElErr As String)
        _ElErr = ""
        Session(SWERRORI_AGGIORNAMENTO) = False
        If (String.IsNullOrEmpty(txtCodCliente.Text)) Then
            txtCodCliente.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice cliente"
        ElseIf Not IsNumeric(txtCodCliente.Text.Trim) Then
            txtCodCliente.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice cliente"
        ElseIf txtCodCliente.Text.Trim.Length <> txtCodCliente.MaxLength Then
            txtCodCliente.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice cliente"
        ElseIf Left(txtCodCliente.Text.Trim, 1) <> "1" Then
            txtCodCliente.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice cliente"
        Else
            txtCodCliente.BackColor = SEGNALA_OK
        End If
        If Not (String.IsNullOrEmpty(txtIPA.Text)) Then 'giu020119
            If txtIPA.Text.Trim.Length <> 6 And txtIPA.Text.Trim.Length <> 7 Then
                txtIPA.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - Codice IPA errato: Lunghezza 6 per la PA altrimenti 7 per Privati/Ditte"
            End If
        End If
        If (String.IsNullOrEmpty(txtRagSoc.Text)) Then
            txtRagSoc.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Ragione sociale"
        Else
            txtRagSoc.BackColor = SEGNALA_OK
        End If
        If (String.IsNullOrEmpty(txtIndirizzo.Text)) Then
            txtIndirizzo.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Indirizzo"
        Else
            txtIndirizzo.BackColor = SEGNALA_OK
        End If
        If (String.IsNullOrEmpty(txtLocalita.Text)) Then
            txtLocalita.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Località"
        Else
            txtLocalita.BackColor = SEGNALA_OK
        End If
        If (String.IsNullOrEmpty(txtCodNazione.Text)) Then
            txtCodNazione.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Nazione"
        Else
            txtCodNazione.BackColor = SEGNALA_OK
        End If
        If (String.IsNullOrEmpty(txtProvincia.Text)) Then
            txtProvincia.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Provincia"
        ElseIf App.GetValoreFromChiave(txtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
            txtProvincia.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Provincia"
        Else
            txtProvincia.BackColor = SEGNALA_OK
            'giu010412
            Dim myCAge As String = txtCodAgente.Text.Trim
            If Not IsNumeric(myCAge) Then myCAge = "0"
            Dim _CAgeAssegnati As String = ""
            If CtrAgente(txtProvincia.Text.Trim, CInt(myCAge), _CAgeAssegnati) = False Then
                lblMessAge.ForeColor = SEGNALA_KO
                lblMessAge.Text = _CAgeAssegnati.ToString.Trim
                'giu180912 tolto controllo sull'agente non assegnato in provincia x le eccezioni
                ' ''txtCodAgente.BackColor = SEGNALA_KO
                ' ''_ElErr += " - Agente assegnato"
                ' ''Session(SWERRORI_AGGIORNAMENTO) = True
            Else
                lblMessAge.ForeColor = Drawing.Color.Blue
                lblMessAge.Text = _CAgeAssegnati.ToString.Trim
            End If
        End If
        If txtEmail.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmail.Text.Trim) = False Then
                txtEmail.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - E-mail"
            Else
                txtEmail.BackColor = SEGNALA_OK
            End If
        Else
            txtEmail.BackColor = SEGNALA_OK
        End If
        If txtEmailInvioFatt.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmailInvioFatt.Text.Trim) = False Then
                txtEmailInvioFatt.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - E-mail invio fatture"
            Else
                txtEmailInvioFatt.BackColor = SEGNALA_OK
            End If
        Else
            txtEmailInvioFatt.BackColor = SEGNALA_OK
        End If
        If txtEmailInvioScad.Text.Trim <> "" Then
            If ConvalidaEmail(txtEmailInvioScad.Text.Trim) = False Then
                txtEmailInvioScad.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - E-mail invio scadenze"
            Else
                txtEmailInvioScad.BackColor = SEGNALA_OK
            End If
        Else
            txtEmailInvioScad.BackColor = SEGNALA_OK
        End If
        If txtPECEMail.Text.Trim <> "" Then
            If ConvalidaEmail(txtPECEMail.Text.Trim) = False Then
                txtPECEMail.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - E-mail PEC"
            Else
                txtPECEMail.BackColor = SEGNALA_OK
            End If
        Else
            txtPECEMail.BackColor = SEGNALA_OK
        End If
        '-------------------------------
        'GIU080113 CONTROLLO P.I. E C.F. (FATTO DA ALBERTO E MODIFICATO DA GIU)
        '-------------------------------
        txtPartitaIVA.BackColor = SEGNALA_OK
        txtCodFiscale.BackColor = SEGNALA_OK
        If rdPersonaFisica.Checked Then 'PERSONA FISICA
            txtCodFiscale.Text = UCase(txtCodFiscale.Text).Trim
            If Not String.IsNullOrEmpty(txtCodFiscale.Text.Trim) Then
                If CF_Controllo("", "", txtDataNascita.Text, "", "", txtCodFiscale.Text) = False Then
                    txtCodFiscale.BackColor = SEGNALA_KO
                    Session(SWERRORI_AGGIORNAMENTO) = True
                    _ElErr += " - Codice Fiscale"
                End If
            Else
                txtCodFiscale.BackColor = SEGNALA_KO
            End If
            If Not String.IsNullOrEmpty(txtPartitaIVA.Text.Trim) Then 'FACOLTATIVO
                Dim Result As Integer
                'Se la nazione non è Italia salta il controllo sulla partita iva
                If txtCodNazione.Text = "I" Or txtCodNazione.Text = "IT" Or txtCodNazione.Text = "ITA" Then
                    ' 0 = corretta
                    ' 1 = Lunghezza errata
                    ' 2 = Presenza di caratteri alfabetici
                    ' 3 = Le prime 7 cifre sono uguali a zero
                    ' 4 = Ufficio IVA errato o mancante in tabella
                    ' 5 = Codice di controllo errato
                    Result = PI_controllo(txtPartitaIVA.Text)
                    Select Case Result
                        Case 1
                            _ElErr += " - Partita IVA (deve essere di 11 cifre)"
                        Case 2
                            _ElErr += " - Partita IVA (deve essere solo numerica)"
                        Case 3
                            _ElErr += " - Partita IVA (le prime 7 cifre non possono essere zeri)"
                        Case 4
                            _ElErr += " - Partita IVA (ufficio IVA errato)"
                        Case 5
                            _ElErr += " - Partita IVA (codice controllo errato)"
                    End Select
                    If Result <> 0 Then
                        txtPartitaIVA.BackColor = SEGNALA_KO
                        Session(SWERRORI_AGGIORNAMENTO) = True
                    End If
                End If
            End If
        Else 'PERSONA GIURIDICA
            If Not String.IsNullOrEmpty(txtCodFiscale.Text.Trim) Then 'FACOLTATIVO
                If IsNumeric(txtCodFiscale.Text.Trim) And txtCodFiscale.Text.Trim.Length = 11 Then
                    'OK PER LE PERSONE GIURIDICHE 
                ElseIf CF_Controllo("", "", txtDataNascita.Text, "", "", txtCodFiscale.Text) = False Then
                    txtCodFiscale.BackColor = SEGNALA_KO
                    Session(SWERRORI_AGGIORNAMENTO) = True
                    _ElErr += " - Codice Fiscale"
                End If
            End If
            txtPartitaIVA.Text = txtPartitaIVA.Text.Trim
            Dim Result As Integer
            If Not (String.IsNullOrEmpty(txtPartitaIVA.Text)) Then
                'Se la nazione non è Italia salta il controllo sulla partita iva
                If txtCodNazione.Text = "I" Or txtCodNazione.Text = "IT" Or txtCodNazione.Text = "ITA" Then
                    ' 0 = corretta
                    ' 1 = Lunghezza errata
                    ' 2 = Presenza di caratteri alfabetici
                    ' 3 = Le prime 7 cifre sono uguali a zero
                    ' 4 = Ufficio IVA errato o mancante in tabella
                    ' 5 = Codice di controllo errato
                    Result = PI_controllo(txtPartitaIVA.Text)
                    Select Case Result
                        Case 1
                            _ElErr += " - Partita IVA (deve essere di 11 cifre)"
                        Case 2
                            _ElErr += " - Partita IVA (deve essere solo numerica)"
                        Case 3
                            _ElErr += " - Partita IVA (le prime 7 cifre non possono essere zeri)"
                        Case 4
                            _ElErr += " - Partita IVA (ufficio IVA errato)"
                        Case 5
                            _ElErr += " - Partita IVA (codice controllo errato)"
                    End Select
                    If Result <> 0 Then
                        txtPartitaIVA.BackColor = SEGNALA_KO
                        Session(SWERRORI_AGGIORNAMENTO) = True
                    End If
                End If
            Else 'PER LE PERSONE GIURIDICHE E' OBBLIGATORIO LA P.I.
                txtPartitaIVA.BackColor = SEGNALA_KO
            End If
        End If
        'GIU180722 VIENE SEGNALATO DOPO 
        txtTelefono1.BackColor = SEGNALA_OK
        txtTelefono2.BackColor = SEGNALA_OK
        If (String.IsNullOrEmpty(txtTelefono1.Text.Trim) And String.IsNullOrEmpty(txtTelefono2.Text.Trim)) Then
            txtTelefono1.BackColor = SEGNALA_KO : txtTelefono2.BackColor = SEGNALA_KO
            ' ''Session(SWERRORI_AGGIORNAMENTO) = True
            ' ''_ElErr += " - Dati Obbligatori per le Spedizioni, Telefono 1 / Telefono 2 Mancanti."
        ElseIf txtTelefono1.Text.Trim = "" And txtTelefono2.Text.Trim = "" Then
            txtTelefono1.BackColor = SEGNALA_KO : txtTelefono2.BackColor = SEGNALA_KO
            ' ''Session(SWERRORI_AGGIORNAMENTO) = True
            ' ''_ElErr += " - Dati Obbligatori per le Spedizioni, Telefono 1 / Telefono 2 Mancanti."
        End If
    End Sub
    'giu030412
    Private Function CtrAgente(ByVal _Pr As String, ByVal _CAge As Integer, ByRef _CAgeAssegnati As String) As Boolean
        CtrAgente = False
        _CAgeAssegnati = "Assegnato: "
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0
        strSQL = "Select CodAgente "
        strSQL += "From AgentiProvince WHERE CodProvinciaOrigine = '" & _Pr.Trim & "'"
        strSQL += " AND DataAttrib <= CONVERT(DATETIME, '" & Format(Now, FormatoData) & "',103) "
        strSQL += " AND DataNonAttrib >= CONVERT(DATETIME, '" & Format(Now, FormatoData) & "',103) "
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("CodAgente")) Then
                            _CAgeAssegnati += "(" & ds.Tables(0).Rows(ii).Item("CodAgente") & ")"
                            If ds.Tables(0).Rows(ii).Item("CodAgente") = _CAge Then
                                CtrAgente = True
                            End If
                        End If
                        ii += 1 'giu200912
                    Next
                Else
                    If CtrAgente = False And _CAge = 0 Then CtrAgente = True
                    Exit Function
                End If
            Else
                If CtrAgente = False And _CAge = 0 Then CtrAgente = True
                Exit Function
            End If
        Catch Ex As Exception
            CtrAgente = False
            If CtrAgente = False And _CAge = 0 Then CtrAgente = True
            _CAgeAssegnati = "Errore: " & Ex.Message
        End Try
        If CtrAgente = False And _CAge = 0 Then CtrAgente = True
    End Function

    Private Function ControlloCampiDati() As Boolean
        ControlloCampiDati = True
        'giu180722 
        If (String.IsNullOrEmpty(txtTelefono1.Text.Trim) And String.IsNullOrEmpty(txtTelefono2.Text.Trim)) Then
            If Session("AvvisaNoTel") = True Then
                Session("AvvisaNoTel") = False
                ControlloCampiDati = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati Obbligatori per le Spedizioni", "Attenzione, Telefono 1 / Telefono 2 Mancanti. <br>" & _
                                "Aggiornare nuovamente se si vuole prosegure senza questi dati.", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        ElseIf txtTelefono1.Text.Trim = "" And txtTelefono2.Text.Trim = "" Then
            If Session("AvvisaNoTel") = True Then
                Session("AvvisaNoTel") = False
                ControlloCampiDati = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Dati Obbligatori per le Spedizioni", "Attenzione, Telefono 1 / Telefono 2 Mancanti. <br>" & _
                                "Aggiornare nuovamente se si vuole prosegure senza questi dati.", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        End If
        '-
        If (String.IsNullOrEmpty(txtCodFiscale.Text.Trim) And String.IsNullOrEmpty(txtPartitaIVA.Text.Trim)) Then
            If Session("AvvisaDoppioCF") = True Or Session("AvvisaDoppioPI") = True Then
                Session("AvvisaDoppioCF") = False
                Session("AvvisaDoppioPI") = False
                ControlloCampiDati = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati cliente", "Attenzione, Partiva IVA e Codice fiscale mancanti. <br>" & _
                                "Aggiornare nuovamente se si vuole prosegure senza questi dati.", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        End If
        '-
        If Not String.IsNullOrEmpty(txtCodFiscale.Text.Trim) Then
            If ControlloDoppio(txtCodFiscale.Text, "CF") Then
                If Session("AvvisaDoppioCF") Then
                    Session("AvvisaDoppioCF") = False
                    Session(ATTESA_CALLBACK_METHOD) = ""
                    Session(CSTNOBACK) = 1
                    ControlloCampiDati = False
                    WUC_Attesa1.ShowCFPivaDoppi("Attenzione", "Il Codice fiscale inserito è utilizzato per altre anagrafiche. Fare click su 'Ok Stampa' per vedere l'elenco.", WUC_Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR_Clienti.aspx?labelForm=Clienti con codice fiscale " & txtCodFiscale.Text)
                    Exit Function
                End If
            End If
        End If
        '-
        If Not String.IsNullOrEmpty(txtPartitaIVA.Text.Trim) Then
            If ControlloDoppio(txtPartitaIVA.Text, "PIVA") Then
                If Session("AvvisaDoppioPI") Then
                    Session("AvvisaDoppioPI") = False
                    Session(ATTESA_CALLBACK_METHOD) = ""
                    Session(CSTNOBACK) = 1
                    ControlloCampiDati = False
                    WUC_Attesa1.ShowCFPivaDoppi("Attenzione", "La Partita IVA inserita è utilizzata per altre anagrafiche. Fare click su 'Ok Stampa' per vedere l'elenco.", WUC_Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR_Clienti.aspx?labelForm=Clienti con partita IVA " & txtPartitaIVA.Text)
                    Exit Function
                End If
            End If
        End If
    End Function

    Private Shared Sub ResetCampoStringaValidato(ByRef txt As TextBox)
        txt.Text = String.Empty
        txt.BackColor = Def.SEGNALA_OK
    End Sub
    'giu200112 SW0 VALIDO ANCHE IL CODICE 0 (REGIME IVA NORMALE)
    Private Sub CheckInserimentoCodice(ByRef txtCod As TextBox, ByVal txtDesc As TextBox, Optional ByVal SW0 As Boolean = False)
        If (String.IsNullOrEmpty(txtCod.Text)) Then
            txtDesc.Text = String.Empty
            txtCod.BackColor = Def.SEGNALA_OK
        ElseIf txtCod.Text.Trim = "0" Then 'GIU131211
            If SW0 = False Then
                txtDesc.Text = String.Empty
                txtCod.BackColor = Def.SEGNALA_OK
            Else
                If (String.IsNullOrEmpty(txtDesc.Text)) Then
                    txtCod.BackColor = Def.SEGNALA_KO
                    Session(SWERRORI_AGGIORNAMENTO) = True
                Else
                    txtCod.Text = txtCod.Text.ToUpper
                    txtCod.BackColor = Def.SEGNALA_OK
                End If
            End If
        Else
            If (String.IsNullOrEmpty(txtDesc.Text)) Then
                txtCod.BackColor = Def.SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
            Else
                txtCod.Text = txtCod.Text.ToUpper
                txtCod.BackColor = Def.SEGNALA_OK
            End If
        End If
    End Sub
    Private Sub CheckInserimentoCodiceTL(ByRef txtCod As TextBox, ByVal lblDesc As Label, Optional ByVal SW0 As Boolean = False)
        If (String.IsNullOrEmpty(txtCod.Text)) Then
            lblDesc.Text = ""
            txtCod.BackColor = Def.SEGNALA_OK
        ElseIf txtCod.Text.Trim = "0" Then 'GIU131211
            If SW0 = False Then
                lblDesc.Text = String.Empty
                txtCod.BackColor = Def.SEGNALA_OK
            Else
                If (String.IsNullOrEmpty(lblDesc.Text)) Then
                    txtCod.BackColor = Def.SEGNALA_KO
                    Session(SWERRORI_AGGIORNAMENTO) = True
                Else
                    txtCod.Text = txtCod.Text.ToUpper
                    txtCod.BackColor = Def.SEGNALA_OK
                End If
            End If
        Else
            If (String.IsNullOrEmpty(lblDesc.Text)) Then
                txtCod.BackColor = Def.SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
            Else
                txtCod.Text = txtCod.Text.ToUpper
                txtCod.BackColor = Def.SEGNALA_OK
            End If
        End If
    End Sub

    Private Sub CheckInserimentoCampoObbligatorio(ByRef txt As TextBox)
        If (String.IsNullOrEmpty(txt.Text)) Then
            txt.BackColor = Def.SEGNALA_KO
        Else
            txt.BackColor = Def.SEGNALA_OK
        End If
    End Sub

#End Region

#Region "Aggiornamento"

    Private Sub Aggiornamento(ByRef myCliente As ClientiEntity)

        If (AggiornaCliente(myCliente)) Then
            Session(COD_CLIENTE) = txtCodCliente.Text.Trim
            Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
            'GIU251111 SvuotaCampi() NON SERVE A NULLA perche' dopo fa il visualizza
            CampiSetEnabledTo(False)
            'GIU251111 Tabs.ActiveTabIndex = 0  
            'GIU251111 VisualizzaCliente()
            setPulsantiModalitaConsulta()
            Session(SWOPCLI) = SWOPNESSUNA
        End If
    End Sub

    Private Function AggiornaCliente(ByVal myCliente As ClientiEntity) As Boolean
        Dim CliSys As New Clienti

        Try
            If (Not CliSys.InsertUpdateCliente(myCliente)) Then
                If (Session(SWOPCLI).Equals(SWOPNUOVO)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile inserire il cliente", WUC_ModalPopup.TYPE_ALERT)
                ElseIf (Session(SWOPCLI).Equals(SWOPMODIFICA)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile modificare il cliente", WUC_ModalPopup.TYPE_ALERT)
                End If
                AggiornaCliente = False
                Exit Function
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in AggiornaCliente", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaCliente = False
            Exit Function
        End Try
        AggiornaCliente = True
    End Function

#End Region

    Private Sub VisualizzaCliente(Optional ByVal SWNoSvuota As Boolean = False)
        If SWNoSvuota = False Then 'giu030714
            GWAltriIndirizzi.SvuotaCampi()
            DestMerceCliente.SvuotaCampi()
        End If
        
        Dim listaClienti As ArrayList = App.GetElencoCompleto(Def.CLIENTI, Session(ESERCIZIO))
        Dim myClienti As ClientiEntity
        Dim posizione As Integer = 0
        Try
            If Not String.IsNullOrEmpty(Session(COD_CLIENTE)) Then
                Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(Session(COD_CLIENTE))
                myClienti = f(0)
                posizione = listaClienti.IndexOf(f(0))
                If (posizione < 0) Then
                    SvuotaCampi()
                Else
                    PopolaCampi(myClienti)
                End If
            Else
                SvuotaCampi()
            End If
            GridViewEC.Visible = False
            'giu150113
            TblScadenze.Visible = False
            lblDesTotScadenze.Visible = False
            lblTotScadenze.Visible = False
            '---------
            GridViewSC.Visible = False
            tblSaldi.Visible = False
            lblSaldoAttAvere.Visible = False
            lblSaldoAttDare.Visible = False
            lblSaldoPrecAvere.Visible = False
            lblSaldoPrecDare.Visible = False
            lblTotMovAvere.Visible = False
            lblTotMovDare.Visible = False
            CampiSetEnabledTo(False)
            setPulsantiModalitaConsulta()
            If TabContainer0.ActiveTabIndex = TB7 Or TabContainer0.ActiveTabIndex = TB8 Then
                Session(CSTCODCOGEOCPR) = txtCodCliente.Text.Trim
            Else
                Session(CSTCODCOGEOCPR) = "-1"
            End If
            Call ClientiOCPregr1.CaricaOCCliente() 'giu170321
            Call ClientiPRPregr1.CaricaPRCliente() 'GIU221021
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in VisualizzaCliente", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Return
        End Try
    End Sub

    Private Sub LocalizzaCliente(ByVal condizione As String, Optional ByVal SWNoSvuota As Boolean = False) 'giu030714 SWNoSvuota
        Dim listaClienti As ArrayList = App.GetLista(Def.CLIENTI, Session(ESERCIZIO))
        Dim posizione As Integer = 0

        If (listaClienti.Count > 0) Then
            Select Case condizione
                Case CLI_CORRENTE
                    Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(Session(COD_CLIENTE))
                    posizione = listaClienti.IndexOf(f(0))
                Case FIRST_CLI
                    posizione = 0
                Case PREV_CLI
                    Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(Session(COD_CLIENTE))
                    posizione = listaClienti.IndexOf(f(0))
                    posizione = posizione - 1
                    posizione = IIf(posizione < 0, 0, posizione)
                Case NEXT_CLI
                    Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(Session(COD_CLIENTE))
                    posizione = listaClienti.IndexOf(f(0))
                    posizione = posizione + 1
                    posizione = IIf(posizione = listaClienti.Count, listaClienti.Count - 1, posizione)
                Case LAST_CLI
                    posizione = listaClienti.Count - 1
            End Select
            If (posizione < 0) Then
                Exit Sub
            End If
            Session(COD_CLIENTE) = listaClienti(posizione).Codice_CoGe
            Session(CSTCODCOGEDM) = listaClienti(posizione).Codice_CoGe 'giu030714 per le dest.Merce ed altro (usato sia dai clienti che fornitori)
            Session(CSTCODCOGENEW) = listaClienti(posizione).Codice_CoGe 'giu261111
            VisualizzaCliente(SWNoSvuota)
        End If
    End Sub
    'GIU180722
    Public Function CKTelefono() As Boolean
        CKTelefono = True
        If (String.IsNullOrEmpty(txtTelefono1.Text.Trim) And String.IsNullOrEmpty(txtTelefono2.Text.Trim)) Then
            CKTelefono = False
        ElseIf txtTelefono1.Text.Trim = "" And txtTelefono2.Text.Trim = "" Then
            CKTelefono = False
        End If
    End Function
    Public Sub setPulsantiModalitaConsulta() 'giu030714 per essere chiamato dai WUC...SOTTOSTANTI(Dest.Merce etc.)
        btnRicerca.Enabled = True
        btnNext.Enabled = True
        btnPrev.Enabled = True
        btnFirst.Enabled = True
        btnLast.Enabled = True
        btnNuovo.Enabled = True
        btnModifica.Enabled = True
        btnElimina.Enabled = True
        'giu281212
        btnStampaEC.Enabled = True : btnStampaSC.Enabled = True
        btnVisualizzaEC.Enabled = True : btnVisualizzaSC.Enabled = True
        '---------
        btnStampa.Enabled = True 'giu250614
        btnCreaPR.Enabled = True
        btnCreaOC.Enabled = True
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaConsulta()
        DestMerceCliente.setPulsantiModalitaConsulta()
    End Sub

    Private Sub setPulsantiModalitaAggiorna()
        btnRicerca.Enabled = False
        btnNext.Enabled = False
        btnPrev.Enabled = False
        btnFirst.Enabled = False
        btnLast.Enabled = False
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        'giu281212
        btnStampaEC.Enabled = False : btnStampaSC.Enabled = False
        btnVisualizzaEC.Enabled = False : btnVisualizzaSC.Enabled = False
        '---------
        btnStampa.Enabled = False 'giu250614
        btnCreaPR.Enabled = False
        btnCreaOC.Enabled = False
        '--
        btnAnnulla.Enabled = True
        btnAggiorna.Enabled = True
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaBlocco()
        DestMerceCliente.setPulsantiModalitaBlocco()
    End Sub

    Private Sub setPulsantiModalitaBlocco()
        btnNuovo.Enabled = True
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        'giu281212
        btnStampaEC.Enabled = False : btnStampaSC.Enabled = False
        btnVisualizzaEC.Enabled = False : btnVisualizzaSC.Enabled = False
        '---------
        btnStampa.Enabled = False 'giu250614
        btnCreaPR.Enabled = False
        btnCreaOC.Enabled = False
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaBlocco()
        DestMerceCliente.setPulsantiModalitaBlocco()
    End Sub

    Private Function CheckNewCodiceCliente() As Boolean
        If txtCodCliente.Text.Trim = "" Then Exit Function
        'giu241111 non era implementato di nulla
        Dim listaClienti As ArrayList = App.GetLista(Def.CLIENTI, Session(ESERCIZIO))
        Dim posizione As Integer = 0
        If (listaClienti.Count > 0) Then
            Dim f = From x In listaClienti Where x.Codice_CoGe.Equals(txtCodCliente.Text.Trim)
            posizione = listaClienti.IndexOf(f(0))
            If (posizione < 0) Then
                Exit Function
            Else
                CheckNewCodiceCliente = True
                Session(MODALPOPUP_CALLBACK_METHOD) = "GetNewCoge"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice cliente già presente in tabella: <br> " & _
                                listaClienti(posizione).Rag_Soc & "  <br> " & _
                                "Vuole un nuovo codice CoGe ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                Exit Function
            End If
        End If
    End Function
    'GIU040512
    Public Sub GetNewCoge()
        txtCodCliente.Text = LocalizzaNEWCliente()
        Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
        Session(CSTCODCOGENEW) = txtCodCliente.Text 'GIU281111
        Session(COD_CLIENTE) = txtCodCliente.Text.Trim
        lblLabelNEW.Text = "Nuovo codice " & txtCodCliente.Text
        ' ''lblLabelNEW.Visible = True
    End Sub

    Private Sub ApriElencoClienti()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoCli.Show(True)
    End Sub

    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            Case F_NAZIONI
                WFPElencoNazioni.Show(True)
            Case F_PROVINCE
                WFPElencoProvince.Show(True)
            Case F_ALIQUOTAIVA
                WFPElencoAliquotaIVA.Show(True)
            Case F_PAGAMENTI
                WFPElencoPagamenti.Show(True)
            Case F_AGENTI
                WFPElencoAgenti.Show(True)
            Case F_AGENTI_ESE_PREC
                WFPElencoAgentiEsePrec.Show(True)
            Case F_ZONE
                WFPElencoZone.Show(True)
            Case F_VETTORI
                WFPElencoVettori.Show(True)
            Case F_CATEGORIE
                WFPElencoCategorie.Show(True)
            Case F_LISTINO
                WFPElencoListVenT.Show(True)
            Case F_CONTI
                WFPElencoConti.Show(True)
            Case F_TIPOFATT
                WFPElencoTipoFatt.Show(True)
        End Select
    End Sub

#End Region

#Region "Metodi public"

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If (Session(F_CLI_RICERCA)) Then
            txtCodCliente.Text = codice.Trim
            txtRagSoc.Text = descrizione.Trim
            Session(COD_CLIENTE) = codice
            Session(CSTCODCOGEDM) = codice 'giu261111
            VisualizzaCliente()
        Else
            txtCodSede.Text = codice
            txtCodSede.BackColor = Def.SEGNALA_OK
            lblSede.Text = descrizione
        End If
    End Sub

    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        Select Case (finestra)
            Case F_NAZIONI
                txtCodNazione.Text = codice
                txtCodNazione.BackColor = Def.SEGNALA_OK
                lblNazione.Text = descrizione
            Case F_PROVINCE
                txtProvincia.Text = codice
                txtProvincia.BackColor = Def.SEGNALA_OK
            Case F_ALIQUOTAIVA
                txtCodRegimeIVA.Text = codice
                txtCodRegimeIVA.BackColor = Def.SEGNALA_OK
                lblRegimeIva.Text = descrizione
            Case F_PAGAMENTI
                txtCodPagamento.Text = codice
                txtCodPagamento.BackColor = Def.SEGNALA_OK
                lblPagamento.Text = descrizione
            Case F_AGENTI
                txtCodAgente.Text = codice
                txtCodAgente.BackColor = Def.SEGNALA_OK
                lblAgente.Text = descrizione
            Case F_AGENTI_ESE_PREC
                txtCodAgenteEsePrec.Text = codice
                txtCodAgenteEsePrec.BackColor = Def.SEGNALA_OK
                lblAgenteEsePrec.Text = descrizione
            Case F_ZONE
                txtCodZona.Text = codice
                txtCodZona.BackColor = Def.SEGNALA_OK
                lblZona.Text = descrizione
            Case F_VETTORI
                txtCodVettore.Text = codice
                txtCodVettore.BackColor = Def.SEGNALA_OK
                lblVettore.Text = descrizione
            Case F_CATEGORIE
                txtCodCategoria.Text = codice
                txtCodCategoria.BackColor = Def.SEGNALA_OK
                lblCategorie.Text = descrizione
            Case F_LISTINO
                txtCodListino.Text = codice
                txtCodListino.BackColor = Def.SEGNALA_OK
                lblListino.Text = descrizione
            Case F_CONTI
                txtCodRicavoFT.Text = codice
                txtCodRicavoFT.BackColor = Def.SEGNALA_OK
                txtRicavoFT.Text = descrizione
            Case F_TIPOFATT
                txtCodTipoFatt.Text = codice
                txtCodTipoFatt.BackColor = Def.SEGNALA_OK
                lblTipoFatt.Text = descrizione
        End Select
    End Sub

    Public Sub ConfermaEliminaCliente()
        Dim Cliys As New Clienti
        Dim errorMsg As String = String.Empty
        Try
            If Cliys.CIClienteByCodice(Session(COD_CLIENTE)) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella contabilità, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If Cliys.CIClienteByCodiceAZI(Session(COD_CLIENTE)) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella gestione aziendale, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If Cliys.CIClienteByCodiceSCAD(Session(COD_CLIENTE)) = False Then 'GIU261111 IDEM PER AZI NEI DOCUMENTIT
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nello scadenzario, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'OK CANCELLO 
            'giu180912 giu101012 aggiorna MySQL Clienti per IREDEEM
            If App.GetDatiAbilitazioni(CSTABILCOGE, "MySQLCli" + Session(ESERCIZIO).ToString.Trim, "", strErrore) = True Then
                'ok chiamo la SP per aggiornare ma senza TRANSAZIONE
                'Session(COD_CLIENTE)
                If DataBaseUtility.MySQLClienti(Session(COD_CLIENTE), CSTInattivo, errorMsg) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Elimina cliente (MySQL)", _
                        String.Format("Errore, contattare l'amministratore di sistema. Errore: {0}", errorMsg), _
                        WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
            '--------------------------------------------
            If (Cliys.delClientiByCodice(Session(COD_CLIENTE))) Then
                If (Not App.CaricaClienti(Session(ESERCIZIO), errorMsg)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Caricamento Dati", _
                        String.Format("Errore nel caricamento Parametri generali azienda, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                        WUC_ModalPopup.TYPE_INFO)
                    SessionUtility.LogOutUtente(Session, Response)
                    Exit Sub
                End If
                LocalizzaCliente(FIRST_CLI)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Il cliente non può essere eliminato. (delClientiByCodice)", WUC_ModalPopup.TYPE_ALERT)
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ConfermaEliminaCliente", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub

    'giu281111
    Public Sub setPulsantiModalitaBloccoByWUC(ByVal SWWUC As String)
        btnFirst.Enabled = False
        btnNext.Enabled = False
        btnPrev.Enabled = False
        btnLast.Enabled = False
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        'giu281212
        btnStampaEC.Enabled = False : btnStampaSC.Enabled = False
        btnVisualizzaEC.Enabled = False : btnVisualizzaSC.Enabled = False
        '---------
        btnStampa.Enabled = False
        btnCreaPR.Enabled = False
        btnCreaOC.Enabled = False

        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        btnRicerca.Enabled = False
        TabPanelDA.Enabled = True 'Dati anagrafici
        TabPanelDADA.Enabled = False 'Dati anagrafici
        If SWWUC = "DAAD" Then
            TabPanelDM.Enabled = False 'Destinazione merce
            TabPanelDAAD.Enabled = True 'Altri dati anagrafici
        Else
            TabPanelDM.Enabled = True 'Destinazione merce
            TabPanelDA.Enabled = False 'Dati anagrafici
            TabPanelDAAD.Enabled = False 'Altri dati anagrafici
        End If
        TabPanelC.Enabled = False 'Commerciale
        TabPanelS.Enabled = False 'Saldi

        TabPanelN.Enabled = False 'Note
    End Sub
    Public Sub setPulsantiModalitaSbloccoByWUC()
        btnFirst.Enabled = True
        btnNext.Enabled = True
        btnPrev.Enabled = True
        btnLast.Enabled = True
        btnNuovo.Enabled = True
        btnModifica.Enabled = True
        btnElimina.Enabled = True
        'giu281212
        btnStampaEC.Enabled = True : btnStampaSC.Enabled = True
        btnVisualizzaEC.Enabled = True : btnVisualizzaSC.Enabled = True
        '---------
        btnStampa.Enabled = True
        btnCreaPR.Enabled = True
        btnCreaOC.Enabled = True

        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        btnRicerca.Enabled = True

        TabPanelDA.Enabled = True 'Dati anagrafici
        TabPanelDADA.Enabled = True 'Dati anagrafici
        TabPanelDM.Enabled = True 'Destinazione merce
        TabPanelDAAD.Enabled = True 'Altri dati anagrafici
        TabPanelC.Enabled = True 'Commerciale
        TabPanelS.Enabled = True 'Saldi

        TabPanelN.Enabled = True 'Note
    End Sub

    Public Sub GetDatiClientiByWUC(ByRef RagSoc As String, ByRef Pr As String, ByRef Naz As String)
        RagSoc = txtRagSoc.Text.Trim
        Pr = txtProvincia.Text.Trim
        Naz = txtCodNazione.Text.Trim
    End Sub
#End Region
#Region "MetodiStampa"
    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampa.Click
        'giu230113 GESTITO NEL MASTERPAGE in base .... Session(CSTNOBACK) = 0 'giu040512
        Session(CSTFinestraChiamante) = "Clienti"
        Session(CSTRitornoDaStampa) = True
        Response.Redirect("..\WebFormTables\WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica clienti")
    End Sub
#End Region

    Private Sub btnZone_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnZone.Click
        WFPZone.WucElement = Me
        WFPZone.SvuotaCampi()
        WFPZone.SetlblMessaggi("")
        Session(F_ANAGRZONE_APERTA) = True
        WFPZone.Show()
    End Sub
    Public Sub CallBackWFPAnagrZone()
        Dim rk As StrZone
        rk = Session(RKZONE)
        If IsNothing(rk.IDZone) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        txtCodZona.Text = rk.IDZone
        lblZona.Text = rk.Descrizione
    End Sub
    Public Sub CancBackWFPAnagrZone()

    End Sub
    '--
    Private Sub btnVettori_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVettori.Click
        WFP_Vettori1.WucElement = Me
        WFP_Vettori1.SvuotaCampi()
        WFP_Vettori1.SetlblMessaggi("")
        Session(F_ANAGRVETTORI_APERTA) = True
        WFP_Vettori1.Show()
    End Sub
    Private Sub btnCategorie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCategorie.Click
        WFP_Categorie1.WucElement = Me
        WFP_Categorie1.SvuotaCampi()
        WFP_Categorie1.SetlblMessaggi("")
        Session(F_ANAGRCATEGORIE_APERTA) = True
        WFP_Categorie1.Show()
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
        txtCodVettore.Text = rk.IDVettori
        lblVettore.Text = rk.Descrizione
    End Sub
    Public Sub CancBackWFPAnagrVettori()

    End Sub

    Private Sub btnAgente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAgente.Click
        WFP_Agenti1.WucElement = Me
        WFP_Agenti1.SvuotaCampi()
        WFP_Agenti1.SetlblMessaggi("")
        Session(F_ANAGRAGENTI_APERTA) = True
        Session(F_ANAGRAGENTI_Proven) = "Agente" 'Provenienza apertura
        WFP_Agenti1.Show()
    End Sub

    Private Sub btnAgenteEsePrec_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAgenteEsePrec.Click
        WFP_Agenti1.WucElement = Me
        WFP_Agenti1.SvuotaCampi()
        WFP_Agenti1.SetlblMessaggi("")
        Session(F_ANAGRAGENTI_APERTA) = True
        Session(F_ANAGRAGENTI_Proven) = "AgenteEsePrec" 'Provenienza apertura
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
        If Not IsNothing(Session(F_ANAGRAGENTI_Proven)) Then
            If Session(F_ANAGRAGENTI_Proven).ToString.ToUpper = "Agente".ToString.ToUpper Then
                txtCodAgente.Text = rk.IDAgenti
                lblAgente.Text = rk.Descrizione
            ElseIf Session(F_ANAGRAGENTI_Proven).ToString.ToUpper = "AgenteEsePrec".ToString.ToUpper Then
                txtCodAgenteEsePrec.Text = rk.IDAgenti
                lblAgenteEsePrec.Text = rk.Descrizione
            End If
        Else
            ''Non assegno a nessuna label
        End If
        Session(F_ANAGRAGENTI_Proven) = ""
    End Sub
    Public Sub CancBackWFPAnagrAgenti()

    End Sub
    Public Sub CallBackWFPAnagrCategorie()
        Session(IDCATEGORIE) = ""
        Dim rk As StrCategorie
        rk = Session(RKCATEGORIE)
        If IsNothing(rk.IDCategorie) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDCATEGORIE) = rk.IDCategorie
        txtCodCategoria.Text = rk.IDCategorie
        lblCategorie.Text = rk.Descrizione
    End Sub
    Public Sub CancBackWFPAnagrCategorie()

    End Sub
    Private Sub DDLBancheIBAN_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLBancheIBAN.SelectedIndexChanged
        ' ''qui non è gestito Session(SWMODIFICATO) = SWSI

        If DDLBancheIBAN.SelectedIndex = 0 Then
            lblABI.Text = ""
            lblCAB.Text = ""
            lblIBAN.Text = ""
            lblContoCorrente.Text = ""
            Exit Sub
        End If
        lblIBAN.Text = DDLBancheIBAN.SelectedValue
        lblABI.Text = Mid(Trim(lblIBAN.Text), 6, 5)
        lblCAB.Text = Mid(Trim(lblIBAN.Text), 11, 5)
        lblContoCorrente.Text = Mid(Trim(lblIBAN.Text), 16)
    End Sub
    Private Sub btnCercaBanca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaBanca.Click
        Session(TIPORK) = "A" 'BANCHE AZIENDA che usa il programma
        WFP_BancheIBAN1.WucElement = Me
        WFP_BancheIBAN1.SvuotaCampi()
        Session(F_BANCHEIBAN_APERTA) = True
        WFP_BancheIBAN1.Show()
    End Sub
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
        ' ''lblBanca.Text = rk.Descrizione.Trim
        lblIBAN.Text = rk.IBAN.Trim
        lblContoCorrente.Text = rk.ContoCorrente.Trim
        ' ''lblABICAB()

        SqlDSBancheIBAN.DataBind()
        DDLBancheIBAN.Items.Clear()
        DDLBancheIBAN.Items.Add("")
        DDLBancheIBAN.DataBind()

        PosizionaItemDDL(lblIBAN.Text, DDLBancheIBAN)
        If DDLBancheIBAN.SelectedIndex = 0 Then
            DDLBancheIBAN.SelectedIndex = 0
            lblABI.Text = ""
            lblCAB.Text = ""
            lblIBAN.Text = ""
            lblContoCorrente.Text = ""
        End If
        '---------
    End Function
    Public Function CancBackWFPBancheIBAN() As Boolean

    End Function

    Private Sub btnVisualizzaEC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizzaEC.Click
        If txtECDataA.Text = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Data di fine intervallo obbligatoria.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("ECCDataA") = CDate(txtECDataA.Text.Trim)
        GridViewEC.DataBind()
        GridViewEC.Visible = True
        'giu150113 Not IsDBNull(dvCliFor.Item(0).Item("Data_Nascita"))
        Dim dvEC As DataView
        Dim TotEC As Decimal = 0
        Try
            dvEC = SqlDSEstrConto.Select(DataSourceSelectArguments.Empty)
            TotEC = 0
            If Not (dvEC Is Nothing) Then
                If dvEC.Count > 0 Then
                    For i = 0 To dvEC.Count - 1
                        If Not IsDBNull(dvEC.Item(i).Item("Importo_Residuo")) Then
                            If IsNumeric(dvEC.Item(i).Item("Importo_Residuo")) Then
                                TotEC += dvEC.Item(i).Item("Importo_Residuo")
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            TotEC = 0
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Errore: " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
        Finally
            lblTotScadenze.Text = Format(TotEC, FormatoValEuro)
            TblScadenze.Visible = True
            lblDesTotScadenze.Visible = True
            lblTotScadenze.Visible = True
        End Try
        
    End Sub

    Private Sub btnStampaEC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaEC.Click
        Dim dsClienti1 As New dsClienti
        Dim ObjReport As New Object
        Dim ClsPrint As New StampaClienti
        Dim StrErrore As String = ""
        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtECDataA.Text = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Data di fine intervallo obbligatoria.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Try
            TitoloRpt = "Estratto conto cliente al " & txtECDataA.Text
            Session("TipoStampa") = 5
            Session(CSTFinestraChiamante) = "Clienti"
            Session(CSTRitornoDaStampa) = True

            If ClsPrint.StampaECCliente(Session(CSTAZIENDARPT), TitoloRpt, dsClienti1, ObjReport, StrErrore, txtECDataA.Text, Session(COD_CLIENTE), Session(CSTCODDITTA)) Then
                If dsClienti1.EstrContoPerStampa.Count > 0 Then
                    Session("dsStampa") = dsClienti1
                    'giu230113 GESTITO NEL MASTERPAGE in base .... Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Clienti.btnStampaEC", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnVisualizzaSC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizzaSC.Click
        Dim TotDare As Decimal
        Dim TotAvere As Decimal
        Dim TotAtt As Decimal
        Dim SQLStr As String
        Dim SQLConnIn As New SqlConnection
        Dim SQLConnUT As New SqlConnection
        Dim SQLConnAP As New SqlConnection
        Dim SQLCmdEser As New SqlCommand
        Dim SQLCmdAP As New SqlCommand
        Dim SQLAdapEser As New SqlDataAdapter
        Dim SQLAdapAP As New SqlDataAdapter
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim DSClienti1 As New dsClienti
        Dim RigaEser As dsClienti.EserciziRow
        Dim RowTMP As dsClienti.ScCont_VisRow
        'Dim NewRow As dsClienti.ScFinaleRow

        SQLConnIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)

        lblTotMovAvere.Text = Format(0, FormatoValEuro)
        lblTotMovDare.Text = Format(0, FormatoValEuro)
        lblSaldoAttAvere.Text = Format(0, FormatoValEuro)
        lblSaldoAttDare.Text = Format(0, FormatoValEuro)
        lblSaldoPrecAvere.Text = Format(0, FormatoValEuro)
        lblSaldoPrecDare.Text = Format(0, FormatoValEuro)


        If txtSCDataDa.Text = "" Or txtSCDataA.Text = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Entrambe le date sono obbligatorie.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If CDate(txtSCDataDa.Text) > CDate(txtSCDataA.Text) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Data inizio periodo maggiore della data di fine periodo.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Try
            DSClienti1.Esercizi.Clear()
            SQLAdapEser.SelectCommand = SQLCmdEser
            SQLCmdEser.Connection = SQLConnIn
            SQLCmdEser.CommandType = CommandType.Text
            SQLCmdEser.CommandText = "SELECT [Ditta], [Esercizio] FROM [Esercizi] ORDER BY [Esercizio] DESC" ' WHERE Ditta = '" & Session(CSTCODDITTA) & "' AND CAST([Esercizio] AS int) >= " & Year(CDate(txtSCDataDa.Text.Trim)) _
            '& " AND CAST([Esercizio] AS int) <= " & Year(CDate(txtSCDataA.Text.Trim)) & " ORDER BY [Esercizio] DESC"

            SQLAdapEser.Fill(DSClienti1.Esercizi)

            DSClienti1.ScCont_Vis.Clear()
            'giu150617
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
            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            For Each RigaEser In DSClienti1.Esercizi.Rows
                SQLConnAP.ConnectionString = dbCon.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftCoge, RigaEser.Esercizio)
                'GIU280417 aggiunto ISNULL(....,'') per tutti i campi altrimenti và in errore la griglia
                SQLStr = "SELECT DISTINCT ISNULL(MovCont.Codice_CoGe,'') AS Codice_CoGe, ISNULL(MovCont.Data_Reg,'') AS Data_Reg, ISNULL(MovCont.N_Reg,'') AS N_Reg, " _
                & "ISNULL(MovCont.N_Riga,0) AS N_Riga, ISNULL(MovCont.Sezione,'') AS Sezione, ISNULL(MovCont.Data_Doc,'') AS Data_Doc, ISNULL(MovCont.N_Doc,'') AS N_Doc, ISNULL(MovCont.Des_Causale,'') AS Des_Causale, " 'Simone300317 (aggiunto ISNULL MovCont.DataDoc)
                SQLStr = SQLStr & "(CASE [MovCont].[Sezione] WHEN 'D' THEN [MovCont].[Importo] ELSE 0 END) AS Dare, " _
                & "(CASE [MovCont].[Sezione] WHEN 'A' THEN [MovCont].[Importo] ELSE 0 END) AS Avere, "
                SQLStr = SQLStr & "0 AS Saldo, ISNULL(MovCont.Contro_Partita,'') AS Contro_Partita, ISNULL(Clienti.Rag_Soc,'') AS Rag_Soc, ISNULL(Fornitori.Rag_Soc,'') AS Rag_Soc, " _
                & "ISNULL(PianoDeiConti.Descrizione,'') AS Descrizione, ISNULL(MovCont.N_IVA,0) AS N_IVA, '" & RigaEser.Esercizio & "' AS Esercizio, ISNULL(Autotrasporti,0) AS Autotrasporti, ISNULL(Stornata,0) AS Stornata " _
                & "FROM ((MovCont LEFT JOIN Clienti ON MovCont.Contro_Partita = Clienti.Codice_CoGe) " _
                & "LEFT JOIN Fornitori ON MovCont.Contro_Partita = Fornitori.Codice_CoGe) " _
                & "LEFT JOIN PianoDeiConti ON MovCont.Contro_Partita = PianoDeiConti.Codice_CoGe " _
                & "Where MovCont.Codice_CoGe = '" & Session(COD_CLIENTE) & "' "
                If Year(CDate(txtSCDataA.Text.Trim)) = CInt(RigaEser.Esercizio) Then
                    SQLStr = SQLStr & "AND MovCont.Data_Reg <= CONVERT(datetime, '" & txtSCDataA.Text.Trim & "', 103) "
                End If
                SQLStr = SQLStr & "ORDER BY ISNULL(MovCont.Data_Reg,''), ISNULL(MovCont.N_Reg,''), ISNULL(MovCont.N_Riga,0)"

                SQLAdapAP.SelectCommand = SQLCmdAP
                SQLCmdAP.Connection = SQLConnAP
                SQLCmdAP.CommandType = CommandType.Text
                SQLCmdAP.CommandText = SQLStr
                SQLCmdAP.CommandTimeout = myTimeOUT '5000
                SQLAdapAP.Fill(DSClienti1.ScCont_Vis)

                SQLConnAP.Close()
            Next

            'For Each RowTMP In DSClienti1.ScCont_Vis.Rows
            '    If RowTMP.Data_Reg >= CDate(txtSCDataDa.Text) And RowTMP.Data_Reg <= CDate(txtSCDataA.Text) Then
            '        NewRow = DSClienti1.ScFinale.NewScFinaleRow
            '        With NewRow
            '            .BeginEdit()
            '            .Data_Reg = RowTMP.Data_Reg
            '            .N_Reg = RowTMP.N_Reg
            '            .N_IVA = RowTMP.N_IVA
            '            .N_Doc = RowTMP.N_Doc
            '            .Data_Doc = RowTMP.Data_Doc
            '            .Des_Causale = RowTMP.Des_Causale
            '            If RowTMP.Dare = 0 Then
            '                .Dare = 0
            '            Else
            '                .Dare = RowTMP.Dare
            '            End If
            '            If RowTMP.Avere = 0 Then
            '                .Avere = 0
            '            Else
            '                .Avere = RowTMP.Avere
            '            End If
            '            .Esercizio = RowTMP.Esercizio
            '            .Autotrasporti = RowTMP.Autotrasporti
            '            .Stornata = RowTMP.Stornata
            '            .EndEdit()
            '        End With
            '        DSClienti1.ScFinale.AddScFinaleRow(NewRow)
            '        NewRow = Nothing
            '    End If
            'Next

            GridViewSC.Visible = True

            TotDare = 0
            TotAvere = 0
            For Each RowTMP In DSClienti1.ScCont_Vis.Rows
                If RowTMP.Data_Reg < CDate(txtSCDataDa.Text) Then
                    TotDare = TotDare + RowTMP.Dare
                    TotAvere = TotAvere + RowTMP.Avere
                    RowTMP.Delete()
                End If
            Next
            DSClienti1.AcceptChanges()

            GridViewSC.DataSource = DSClienti1.ScCont_Vis.Select("", "Data_Reg, N_Reg") 'giu240112 SORT
            GridViewSC.DataBind()

            'Saldo Precedente
            If TotDare > TotAvere Then
                TotAtt = TotDare - TotAvere
                lblSaldoPrecDare.Text = Format(TotAtt, FormatoValEuro)
                lblSaldoPrecAvere.Text = Format(0, FormatoValEuro)
            ElseIf TotAvere > TotDare Then
                TotAtt = TotAvere - TotDare
                lblSaldoPrecAvere.Text = Format(TotAtt, FormatoValEuro)
                lblSaldoPrecDare.Text = Format(0, FormatoValEuro)
            Else
                lblSaldoPrecAvere.Text = Format(0, FormatoValEuro)
                lblSaldoPrecDare.Text = Format(0, FormatoValEuro)
            End If

            TotDare = 0
            TotAvere = 0
            For Each RowTMP In DSClienti1.ScCont_Vis.Rows
                If RowTMP.Data_Reg >= CDate(txtSCDataDa.Text) And RowTMP.Data_Reg <= CDate(txtSCDataA.Text) Then
                    TotDare = TotDare + RowTMP.Dare
                    TotAvere = TotAvere + RowTMP.Avere
                End If
            Next

            lblTotMovDare.Text = Format(TotDare, FormatoValEuro)
            lblTotMovAvere.Text = Format(TotAvere, FormatoValEuro)

            TotDare = 0
            TotAvere = 0

            If lblSaldoPrecDare.Text <> "" And CDec(lblSaldoPrecDare.Text) <> 0 Then
                TotDare = TotDare + CDec(lblSaldoPrecDare.Text)
            End If
            If lblSaldoPrecAvere.Text <> "" And CDec(lblSaldoPrecAvere.Text) <> 0 Then
                TotAvere = TotAvere + CDec(lblSaldoPrecAvere.Text)
            End If
            If lblTotMovDare.Text <> "" And CDec(lblTotMovDare.Text) <> 0 Then
                TotDare = TotDare + CDec(lblTotMovDare.Text)
            End If
            If lblTotMovAvere.Text <> "" And CDec(lblTotMovAvere.Text) <> 0 Then
                TotAvere = TotAvere + CDec(lblTotMovAvere.Text)
            End If

            If TotDare > TotAvere Then
                TotAtt = TotDare - TotAvere
                lblSaldoAttDare.Text = Format(TotAtt, FormatoValEuro)
                lblSaldoAttAvere.Text = Format(0, FormatoValEuro)
            ElseIf TotAvere > TotDare Then
                TotAtt = TotAvere - TotDare
                lblSaldoAttAvere.Text = Format(TotAtt, FormatoValEuro)
                lblSaldoAttDare.Text = Format(0, FormatoValEuro)
            Else
                lblSaldoAttAvere.Text = Format(0, FormatoValEuro)
                lblSaldoAttDare.Text = Format(0, FormatoValEuro)
            End If

            tblSaldi.Visible = True
            lblSaldoAttAvere.Visible = True
            lblSaldoAttDare.Visible = True
            lblSaldoPrecAvere.Visible = True
            lblSaldoPrecDare.Visible = True
            lblTotMovAvere.Visible = True
            lblTotMovDare.Visible = True

        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Errore: " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
        End Try
    End Sub

    Private Sub btnStampaSC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSC.Click
        Dim dsClienti1 As New dsClienti
        Dim ObjReport As New Object
        Dim ClsPrint As New StampaClienti
        Dim StrErrore As String = ""
        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtSCDataA.Text.Trim = "" Or txtSCDataDa.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Entrambe le date sono obbligatorie.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Try
            TitoloRpt = "Scheda Contabile A4"
            Session("TipoStampa") = 6
            Session(CSTFinestraChiamante) = "Clienti"
            Session(CSTRitornoDaStampa) = True

            If ClsPrint.StampaSCCliente(Session(CSTAZIENDARPT), TitoloRpt, dsClienti1, ObjReport, StrErrore, txtSCDataDa.Text.Trim, txtSCDataA.Text.Trim, Session(COD_CLIENTE), Session(CSTCODDITTA)) Then
                If dsClienti1.ScCont_Vis.Count > 0 Then
                    Session("dsStampa") = dsClienti1
                    'giu230113 GESTITO NEL MASTERPAGE in base .... Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Clienti.btnStampaEC", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub GridViewEC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewEC.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(IdColEC.DataDoc).Text) Then
                e.Row.Cells(IdColEC.DataDoc).Text = Format(CDate(e.Row.Cells(IdColEC.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(IdColEC.DataScad).Text) Then
                e.Row.Cells(IdColEC.DataScad).Text = Format(CDate(e.Row.Cells(IdColEC.DataScad).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(IdColEC.ImportoDoc).Text) Then
                e.Row.Cells(IdColEC.ImportoDoc).Text = Format(CDec(e.Row.Cells(IdColEC.ImportoDoc).Text), FormatoValEuro).ToString
            End If
            If IsNumeric(e.Row.Cells(IdColEC.ImpResiduo).Text) Then
                e.Row.Cells(IdColEC.ImpResiduo).Text = Format(CDec(e.Row.Cells(IdColEC.ImpResiduo).Text), FormatoValEuro).ToString
            End If
        End If
    End Sub

    Private Sub GridViewSC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewSC.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(IdColSC.DataDoc).Text) Then
                e.Row.Cells(IdColSC.DataDoc).Text = Format(CDate(e.Row.Cells(IdColSC.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(IdColSC.DataReg).Text) Then
                e.Row.Cells(IdColSC.DataReg).Text = Format(CDate(e.Row.Cells(IdColSC.DataReg).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(IdColSC.Dare).Text) Then
                If CDec(e.Row.Cells(IdColSC.Dare).Text) = 0 Then
                    e.Row.Cells(IdColSC.Dare).Text = ""
                Else
                    e.Row.Cells(IdColSC.Dare).Text = Format(CDec(e.Row.Cells(IdColSC.Dare).Text), FormatoValEuro).ToString
                End If
            End If
            If IsNumeric(e.Row.Cells(IdColSC.Avere).Text) Then
                If CDec(e.Row.Cells(IdColSC.Avere).Text) = 0 Then
                    e.Row.Cells(IdColSC.Avere).Text = ""
                Else
                    e.Row.Cells(IdColSC.Avere).Text = Format(CDec(e.Row.Cells(IdColSC.Avere).Text), FormatoValEuro).ToString
                End If
            End If
            'Simone300317 (Date a NULL(01/01/1900) non visualizzate)
            If e.Row.Cells(IdColSC.DataDoc).Text = "01/01/1900" Then
                e.Row.Cells(IdColSC.DataDoc).Text = ""
            End If
        End If
    End Sub

    Private Sub txtCodTipoFatt_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodTipoFatt.TextChanged
        If txtCodTipoFatt.Text.Length < 2 Then
            txtCodTipoFatt.Text = "0" & txtCodTipoFatt.Text
        End If
        lblTipoFatt.Text = App.GetValoreFromChiave(txtCodTipoFatt.Text, Def.TIPOFATT, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodTipoFatt, lblTipoFatt)
    End Sub

    Private Sub btnTrovaTipoFatt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaTipoFatt.Click
        ApriElenco(F_TIPOFATT)
    End Sub

    Private Sub btnGestTipoFatt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGestTipoFatt.Click
        WFPTipoFatturazione.WucElement = Me
        WFPTipoFatturazione.SvuotaCampi()
        WFPTipoFatturazione.SetlblMessaggi("")
        Session(F_ANAGRTIPOFATT_APERTA) = True
        WFPTipoFatturazione.Show()
    End Sub

    Public Sub CallBackWFPTipoFatt()
        Dim rk As StrTipoFatt
        rk = Session(RKTIPOFATT)
        If IsNothing(rk.Codice) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        txtCodTipoFatt.Text = rk.Codice
        lblTipoFatt.Text = rk.Descrizione
    End Sub

    Public Sub CancBackWFPTipoFatt()

    End Sub

    Function CF_Controllo(ByVal Cognome, ByVal Nome, ByVal Data_Nascita, ByVal Cod_Catasto, ByVal Sesso, ByVal Cod_Fiscale) As Object
        If Cod_Fiscale.Trim = "" Then
            Return True
        End If
        'Controllo del codice fiscale
        'giu160108 SOLO CONTROLLO FORMALE se i dati anagrafici sono vuoti
        Dim giorno_nascita, mese_nascita, anno_nascita, errore

        If Len(Cod_Fiscale) <> 16 Then
            CF_Controllo = False
            Exit Function
        End If

        'Trasformo il cognome in tutte lettere maiuscole
        Call Dividi_Lettere(Cognome)

        Ind4 = 0
        If Trim(Cognome) <> "" Then 'GIU160108
            If Ind2 = 0 Then
                Codice_Fiscale(1) = Tab_Vocali(1)
                Codice_Fiscale(2) = Tab_Vocali(2)
                Codice_Fiscale(3) = "X"
            Else
                If Ind2 > 2 Then
                    Codice_Fiscale(1) = Tab_Consonanti(1)
                    Codice_Fiscale(2) = Tab_Consonanti(2)
                    Codice_Fiscale(3) = Tab_Consonanti(3)
                Else
                    If Ind2 = 2 Then
                        Codice_Fiscale(1) = Tab_Consonanti(1)
                        Codice_Fiscale(2) = Tab_Consonanti(2)
                        Codice_Fiscale(3) = Tab_Vocali(1)
                    Else
                        Codice_Fiscale(1) = Tab_Consonanti(1)
                        Codice_Fiscale(2) = Tab_Vocali(1)
                        If Ind3 = 1 Then
                            Codice_Fiscale(3) = "X"
                        Else
                            Codice_Fiscale(3) = Tab_Vocali(2)
                        End If
                    End If
                End If
            End If
        Else 'metto quello che hanno digitato
            Codice_Fiscale(1) = Mid(Cod_Fiscale, 1, 1)
            Codice_Fiscale(2) = Mid(Cod_Fiscale, 2, 1)
            Codice_Fiscale(3) = Mid(Cod_Fiscale, 3, 1)
        End If
        Ind4 = 3
        'Trasformo il nome in tutte lettere maiuscole
        ReDim Tab_Vocali(0)
        ReDim Tab_Consonanti(0)
        Call Dividi_Lettere(Nome)
        If Trim(Cognome) <> "" Then 'GIU160108
            If Ind2 = 0 Then
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = Tab_Vocali(1)
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = Tab_Vocali(2)
                Ind4 = Ind4 + 1
                Codice_Fiscale(Ind4) = "X"
            Else
                If Ind2 > 3 Then
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(3)
                    Ind4 = Ind4 + 1
                    Codice_Fiscale(Ind4) = Tab_Consonanti(4)
                Else
                    If Ind2 > 2 Then
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(2)
                        Ind4 = Ind4 + 1
                        Codice_Fiscale(Ind4) = Tab_Consonanti(3)
                    Else
                        If Ind2 = 2 Then
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(2)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Vocali(1)
                        Else
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Consonanti(1)
                            Ind4 = Ind4 + 1
                            Codice_Fiscale(Ind4) = Tab_Vocali(1)
                            If Ind3 = 1 Then
                                Ind4 = Ind4 + 1
                                Codice_Fiscale(Ind4) = "X"
                            Else
                                Ind4 = Ind4 + 1
                                Codice_Fiscale(Ind4) = Tab_Vocali(2)
                            End If
                        End If
                    End If
                End If
            End If
        Else 'metto quello che hanno digitato
            Codice_Fiscale(4) = Mid(Cod_Fiscale, 4, 1)
            Codice_Fiscale(5) = Mid(Cod_Fiscale, 5, 1)
            Codice_Fiscale(6) = Mid(Cod_Fiscale, 6, 1)
        End If
        'Anno di nascita
        Ind4 = Ind4 + 1
        Ind4 = 7 'giu160108
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 7, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        anno_nascita = Codice_Fiscale(Ind4)
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 8, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        anno_nascita = anno_nascita + Codice_Fiscale(Ind4)
        anno_nascita = Val(anno_nascita)

        'Mese di nascita
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 9, 1)
        errore = True
        mese_nascita = 0
        Select Case Codice_Fiscale(Ind4)
            Case "A"
                errore = False
                mese_nascita = 1
            Case "B"
                errore = False
                mese_nascita = 2
            Case "C"
                errore = False
                mese_nascita = 3
            Case "D"
                errore = False
                mese_nascita = 4
            Case "E"
                errore = False
                mese_nascita = 5
            Case "H"
                errore = False
                mese_nascita = 6
            Case "L"
                errore = False
                mese_nascita = 7
            Case "M"
                errore = False
                mese_nascita = 8
            Case "P"
                errore = False
                mese_nascita = 9
            Case "R"
                errore = False
                mese_nascita = 10
            Case "S"
                errore = False
                mese_nascita = 11
            Case "T"
                errore = False
                mese_nascita = 12
        End Select
        If errore Then
            CF_Controllo = False
            Exit Function
        End If

        'giorno di nascita
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 10, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        Ind4 = Ind4 + 1
        Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, 11, 1)
        If Not IsNumeric(Codice_Fiscale(Ind4)) Then
            CF_Controllo = False
            Exit Function
        End If
        '---
        giorno_nascita = Codice_Fiscale(10) + Codice_Fiscale(11)
        giorno_nascita = Val(giorno_nascita)
        If giorno_nascita > 40 Then
            giorno_nascita = giorno_nascita - 40
            Sesso = "F"
        Else
            Sesso = "M"
        End If
        If giorno_nascita < 1 Or giorno_nascita > 31 Then
            CF_Controllo = False
            Exit Function
        End If

        'Codice catasto
        If Trim(Cod_Catasto) <> "" Then 'GIU160108
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 1, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 2, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 3, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Catasto, 4, 1)
        Else
            Ind4 = 12
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
            Ind4 = Ind4 + 1
            Codice_Fiscale(Ind4) = Mid(Cod_Fiscale, Ind4, 1)
            Cod_Catasto = Cod_Catasto & Mid(Cod_Fiscale, Ind4, 1)
        End If
        'Codice di controllo
        If Not Codice_Controllo() Then
            CF_Controllo = False
            Exit Function
        End If

        'Fine creazione porta la tabella a stringa
        Cod_Fisc = ""
        For Ind1 = 1 To Ind4
            Cod_Fisc = Cod_Fisc + Codice_Fiscale(Ind1)
        Next Ind1

        If Cod_Fisc <> Cod_Fiscale Then
            CF_Controllo = False
        Else
            CF_Controllo = True
        End If

        Dim myData_Nascita As String = LTrim(Str(giorno_nascita)) + "/" + LTrim(Str(mese_nascita)) + "/" + LTrim(Str(anno_nascita))
        If IsDate(Data_Nascita.ToString.Trim) Then
            If CDate(Data_Nascita.ToString.Trim) <> CDate(myData_Nascita) Then
                CF_Controllo = False
                txtDataNascita.BackColor = SEGNALA_KO
            End If
        End If
    End Function
    Sub Dividi_Lettere(ByRef Stringa As String)
        Dim Testo As String
        Testo = Stringa

        Testo = UCase(Testo)
        Ind2 = 0
        Ind3 = 0
        For Ind1 = 1 To Len(Testo)
            'elimino i caratteri che non mi interessano
            If Asc(Mid(Testo, Ind1, 1)) < 64 Or Asc(Mid(Testo, Ind1, 1)) > 91 Then
                car = " "
            Else
                car = Mid(Testo, Ind1, 1)
            End If
            'divido le vocali dalle consonanti e le metto in
            'tabelle separate
            If car = "A" Or car = "E" Or car = "I" Or car = "O" Or car = "U" Then
                Ind3 = Ind3 + 1
                ReDim Preserve Tab_Vocali(Ind3 + 1)
                Tab_Vocali(Ind3) = car
            Else
                If car <> " " Then
                    Ind2 = Ind2 + 1
                    ReDim Preserve Tab_Consonanti(Ind2 + 1)
                    Tab_Consonanti(Ind2) = car
                End If
            End If
        Next Ind1
        'ind2 contiene il numero di consonanti
        'ind3 contiene il numero di vocali
    End Sub
    Function Codice_Controllo() As Object

        Dim Pari, Ind, Totale, resto

        Pari = 0
        Call Controllo_Pari(Pari)
        Call Controllo_Dispari(Pari)

        Totale = Int(Pari / 26)
        resto = Pari - (Totale * 26)

        Ind = 0
        Ind4 = Ind4 + 1
        Do
            If resto = Ind Then
                Codice_Fiscale(Ind4) = Chr(65 + Ind)
                Exit Do
            End If
            Ind = Ind + 1
        Loop Until Ind > 25

        'se il resto supera 25 c'è un errore
        If Ind > 25 Then
            Codice_Controllo = False
        Else
            Codice_Controllo = True
        End If

    End Function
    Sub Controllo_Pari(ByRef Pari As Integer)

        Dim Ind
        Ind = 0
        Do
            Ind = Ind + 2
            If Ind > 14 Then Exit Do

            Select Case Codice_Fiscale(Ind)
                Case "A", "0"
                    Pari = Pari + 0
                Case "B", "1"
                    Pari = Pari + 1
                Case "C", "2"
                    Pari = Pari + 2
                Case "D", "3"
                    Pari = Pari + 3
                Case "E", "4"
                    Pari = Pari + 4
                Case "F", "5"
                    Pari = Pari + 5
                Case "G", "6"
                    Pari = Pari + 6
                Case "H", "7"
                    Pari = Pari + 7
                Case "I", "8"
                    Pari = Pari + 8
                Case "J", "9"
                    Pari = Pari + 9
                Case "K"
                    Pari = Pari + 10
                Case "L"
                    Pari = Pari + 11
                Case "M"
                    Pari = Pari + 12
                Case "N"
                    Pari = Pari + 13
                Case "O"
                    Pari = Pari + 14
                Case "P"
                    Pari = Pari + 15
                Case "Q"
                    Pari = Pari + 16
                Case "R"
                    Pari = Pari + 17
                Case "S"
                    Pari = Pari + 18
                Case "T"
                    Pari = Pari + 19
                Case "U"
                    Pari = Pari + 20
                Case "V"
                    Pari = Pari + 21
                Case "W"
                    Pari = Pari + 22
                Case "X"
                    Pari = Pari + 23
                Case "Y"
                    Pari = Pari + 24
                Case "Z"
                    Pari = Pari + 25
            End Select
        Loop
    End Sub
    Sub Controllo_Dispari(ByRef Dispari As Integer)

        Dim Ind
        Ind = 1
        Do
            Select Case Codice_Fiscale(Ind)
                Case "A", "0"
                    Dispari = Dispari + 1
                Case "B", "1"
                    Dispari = Dispari + 0
                Case "C", "2"
                    Dispari = Dispari + 5
                Case "D", "3"
                    Dispari = Dispari + 7
                Case "E", "4"
                    Dispari = Dispari + 9
                Case "F", "5"
                    Dispari = Dispari + 13
                Case "G", "6"
                    Dispari = Dispari + 15
                Case "H", "7"
                    Dispari = Dispari + 17
                Case "I", "8"
                    Dispari = Dispari + 19
                Case "J", "9"
                    Dispari = Dispari + 21
                Case "K"
                    Dispari = Dispari + 2
                Case "L"
                    Dispari = Dispari + 4
                Case "M"
                    Dispari = Dispari + 18
                Case "N"
                    Dispari = Dispari + 20
                Case "O"
                    Dispari = Dispari + 11
                Case "P"
                    Dispari = Dispari + 3
                Case "Q"
                    Dispari = Dispari + 6
                Case "R"
                    Dispari = Dispari + 8
                Case "S"
                    Dispari = Dispari + 12
                Case "T"
                    Dispari = Dispari + 14
                Case "U"
                    Dispari = Dispari + 16
                Case "V"
                    Dispari = Dispari + 10
                Case "W"
                    Dispari = Dispari + 22
                Case "X"
                    Dispari = Dispari + 25
                Case "Y"
                    Dispari = Dispari + 24
                Case "Z"
                    Dispari = Dispari + 23
            End Select

            Ind = Ind + 2

        Loop Until Ind > 15
    End Sub
    Private Function ControlloDoppio(ByVal Stringa As String, ByVal Tipo As String) As Boolean
        If Stringa.Trim = "" Then
            Return False
        End If
        Dim SQLCmd As New SqlCommand
        Dim SQLAdap As New SqlDataAdapter
        Dim ds As New dsClienti

        Try
            SQLCmd.Connection = New SqlConnection
            SQLCmd.Connection.ConnectionString = Session(DBCONNCOGE)
            SQLCmd.CommandType = CommandType.Text

            If Tipo = "CF" Then
                SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Clienti WHERE Codice_Fiscale = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(COD_CLIENTE) & "'"
            ElseIf Tipo = "PIVA" Then
                SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Clienti WHERE Partita_IVA = '" & Controlla_Apice(Stringa) & "' AND Codice_CoGe <> '" & Session(COD_CLIENTE) & "'"
            End If

            SQLCmd.CommandText = SQLCmd.CommandText & " UNION SELECT '" & Controlla_Apice(txtCodCliente.Text.Trim) & "' AS Codice_CoGe, '" & Controlla_Apice(txtRagSoc.Text.Trim) & "' AS Rag_Soc, '" & Controlla_Apice(txtCodFiscale.Text.Trim) & "' AS Codice_Fiscale, '" & Controlla_Apice(txtPartitaIVA.Text.Trim) & "' AS Partita_IVA, '" & Controlla_Apice(Session(CSTAZIENDARPT)) & "' AS Ditta FROM Clienti"

            SQLAdap.SelectCommand = SQLCmd

            SQLAdap.Fill(ds.Doppi)

            If ds.Doppi.Rows.Count > 1 Then
                ControlloDoppio = True
                Session("dsStampa") = ds
            Else
                ControlloDoppio = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            If Tipo = "CF" Then
                ModalPopup.Show("Attenzione", "Errore durante il controllo sul codice fiscale duplicato. <br> " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ElseIf Tipo = "PIVA" Then
                ModalPopup.Show("Attenzione", "Errore durante il controllo sulla partita IVA duplicata. <br> " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            End If
            ControlloDoppio = False
        End Try
    End Function
    Function PI_controllo(ByVal Partita_Iva As String) As Integer
        If Partita_Iva.Trim = "" Then
            Return 0
        End If
        ' 0 = corretta
        ' 1 = Lunghezza errata
        ' 2 = Presenza di caratteri alfabetici
        ' 3 = Le prime 7 cifre sono uguali a zero
        ' 4 = Ufficio IVA errato o mancante in tabella
        ' 5 = Codice di controllo errato
        'Controllo della partita iva
        Dim Tab_Iva(12) As String
        Dim Ind As Integer
        Dim Tot_Iva As Long
        Dim Per_Due As Long
        Dim Tot_Iva_Ult As String
        Dim Contr As Object

        PI_controllo = 0

        If Len(Partita_Iva) <> 11 Then
            PI_controllo = 1
            Exit Function
        End If

        If Not IsNumeric(Partita_Iva) Then
            PI_controllo = 2
            Exit Function
        End If

        If Left(Partita_Iva, 7) = "0000000" Then
            PI_controllo = 3
            Exit Function
        End If

        For Ind = 1 To 11
            Tab_Iva(Ind) = Mid(Partita_Iva, Ind, 1)
        Next Ind

        Tot_Iva = 0

        Per_Due = 2 * Tab_Iva(2)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(4)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(6)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(8)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Per_Due = 2 * Tab_Iva(10)
        Per_Due = LTrim(Str(Per_Due))
        If Len(Per_Due) > 1 Then
            Tot_Iva = Tot_Iva + Val(Left(Per_Due, 1))
            Tot_Iva = Tot_Iva + Val(Mid(Per_Due, 2, 1))
        Else
            Tot_Iva = Tot_Iva + Val(Per_Due)
        End If

        Tot_Iva = Tot_Iva + Val(Tab_Iva(1))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(3))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(5))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(7))
        Tot_Iva = Tot_Iva + Val(Tab_Iva(9))

        Tot_Iva_Ult = Mid(LTrim(Str(Tot_Iva)), Len(LTrim(Str(Tot_Iva))), 1)
        If Tot_Iva_Ult = "0" Then
            Contr = Tot_Iva_Ult
        Else
            Contr = 10 - Val(Tot_Iva_Ult)
        End If
        Contr = Right(LTrim(Str(Contr)), 1)

        If Contr <> Tab_Iva(11) Then
            PI_controllo = 5
            Exit Function
        End If

        PI_controllo = 0
    End Function

    Private Sub txtPartitaIVA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPartitaIVA.TextChanged
        If Session("AvvisaDoppioPI") = False Then
            Session("AvvisaDoppioPI") = True
        End If
        txtCodFiscale.Focus()
    End Sub

    Private Sub txtCodFiscale_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFiscale.TextChanged
        If Session("AvvisaDoppioCF") = False Then
            Session("AvvisaDoppioCF") = True
        End If
        txtTelefono1.Focus()
    End Sub

    Private Sub btnCreaOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaOC.Click
        If Session(SWOPCLI) <> SWOPNESSUNA Then Exit Sub
        Session(CSTFinestraChiamante) = "Clienti"
        Session(CSTRitornoDaStampa) = True
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo ordine Cliente", "Confermi la creazione dell'ordine Cliente ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaNewOC()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
    End Sub

    Private Sub btnCreaPR_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaPR.Click
        If Session(SWOPCLI) <> SWOPNESSUNA Then Exit Sub
        Session(CSTFinestraChiamante) = "Clienti"
        Session(CSTRitornoDaStampa) = True
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewPR"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo preventivo Cliente", "Confermi la creazione del preventivo Cliente ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaNewPR()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Session(CSTTIPODOC) = SWTD(TD.Preventivi)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione preventivi/offerte CLIENTI")
    End Sub

    Private Sub TabContainer0_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabContainer0.ActiveTabChanged
        Session("TabClienti") = TabContainer0.ActiveTabIndex
        If TabContainer0.ActiveTabIndex = TB0 Then 'GIU290323 altri indirizzi
            Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
            GWAltriIndirizzi.CaricaAltriIndir()
        ElseIf TabContainer0.ActiveTabIndex = TB7 Or TabContainer0.ActiveTabIndex = TB8 Then
            Session(COD_CLIENTE) = txtCodCliente.Text.Trim 'GIU010223 ORA USO CSTCODCOGEOCPR
            Session(CSTCODCOGEOCPR) = txtCodCliente.Text.Trim
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnAggiorna.Visible = False
            btnAnnulla.Visible = False
            btnElimina.Visible = False
            Call ClientiOCPregr1.CaricaOCCliente() 'giu170321
            Call ClientiPRPregr1.CaricaPRCliente() 'GIU221021
        ElseIf TabContainer0.ActiveTabIndex = TB2 Then 'GIU280323 DESTINAZIONE MERCE
            Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
            DestMerceCliente.CaricaDest()
        Else
            btnNuovo.Visible = True
            btnModifica.Visible = True
            btnAggiorna.Visible = True
            btnAnnulla.Visible = True
            btnElimina.Visible = True
        End If
    End Sub
    Private Sub TabContainer1_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabContainer1.ActiveTabChanged
        Session("TabClienti") = TabContainer0.ActiveTabIndex
        If TabContainer0.ActiveTabIndex = TB0 Then 'GIU290323 altri indirizzi
            Session(CSTCODCOGEDM) = txtCodCliente.Text.Trim
            GWAltriIndirizzi.CaricaAltriIndir()
        End If
    End Sub
    'giu180722
    Private Sub txtTelefono1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTelefono1.TextChanged
        If Session("AvvisaNoTel") = False Then
            Session("AvvisaNoTel") = True
        End If
        txtTelefono2.Focus()
    End Sub

    Private Sub txtTelefono2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTelefono2.TextChanged
        If Session("AvvisaNoTel") = False Then
            Session("AvvisaNoTel") = True
        End If
        txtTelefonoFax.Focus()
    End Sub

    
End Class