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

Partial Public Class WUC_Fornitori
    Inherits System.Web.UI.UserControl
    Const F_ANAGRAGENTI_Proven As String = "F_ANAGRAGENTI_Proven"

    ''giu160113
    Dim Tab_Vocali() As String
    Dim Tab_Consonanti() As String
    Dim car As String
    Dim Ind1 As Integer
    Dim Ind2 As Integer
    Dim Ind3 As Integer
    Dim Ind4 As Integer
    Dim Cod_Fisc As String
    Dim Codice_Fiscale(17) As String

#Region "Costanti"

    Private Const NOSEL_BTN As Integer = 0
    Private Const SEL_BTN_NUOVO As Integer = 1
    Private Const SEL_BTN_MODIFICA As Integer = 2
    Private Const CAMPI_MODIFICATI As String = "CampiModificati"
    Private Const TIPO_AGG_FOR As String = "AggiornamentoFornitore" 'giu121211
    Private Const ERRORI_AGGIORNAMENTO As String = "ErroriAggiornamento"

    Private Const F_PROVINCE As String = "ElencoProvince"
    Private Const F_NAZIONI As String = "ElencoNazioniFor" 'giu121211
    Private Const F_NAZIONIIBAN As String = "ElencoNazioniIBAN"
    Private Const F_ALIQUOTAIVA As String = "ElencoAliquotaIVA"
    Private Const F_PAGAMENTI As String = "ElencoPagamenti"
    Private Const F_AGENTI As String = "ElencoAgenti"
    Private Const F_AGENTI_ESE_PREC As String = "ElencoAgentiEsercizioPrecedente"
    Private Const F_ZONE As String = "ElencoZone"
    Private Const F_VETTORI As String = "ElencoVettori"
    Private Const F_CATEGORIE As String = "ElencoCategorie"
    Private Const F_LISTINO As String = "ElencoListinoVendita"
    Private Const F_CONTI As String = "ElencoPianoDeiConti"
    'giu121211
    Private Const NEXT_FOR As String = "NEXT"
    Private Const PREV_FOR As String = "PREV"
    Private Const LAST_FOR As String = "LAST"
    Private Const FIRST_FOR As String = "FIRST"
    Private Const FOR_CORRENTE As String = "CORRENTE"
    'giu121211
#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        Session(DBCONNAZI) = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Session(DBCONNCOGE) = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSBancheIBAN.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            Session(COD_FORNITORE) = String.Empty
            Session(CSTCODCOGEDM) = "" 'giu261111
        End If
        ModalPopup.WucElement = Me
        WUC_Attesa1.WucElement = Me 'giu031211
        GWAltriIndirizzi.WucElement = Me
        DestMerceFornitore.WucElement = Me
        '---
        WFPElencoFor.WucElement = Me
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

        If txtCodNazione.Text.Trim <> "I" And txtCodNazione.Text.Trim <> "IT" And txtCodNazione.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If
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
        If Val(strObbligatorio) < 1 Then
            Chiudi("Errore: Caricamento dati Società - MaskLevel non definito (Sessione scaduta - effettuare il login)", True)
            Exit Sub
        End If
        txtCodFornitore.MaxLength = CInt(strObbligatorio)
        txtCodSede.MaxLength = CInt(strObbligatorio) 'GIU121211
        If (Not IsPostBack) Then
            CaricaVariabili()
            CampiSetEnabledTo(False)
            setPulsantiModalitaConsulta()
            LocalizzaFornitore(FIRST_FOR, True)
            Session(SWOP) = SWOPNESSUNA
        End If

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            WFPElencoFor.Show()
        End If

        Select Case Session(F_ELENCO_APERTA)
            Case F_NAZIONI
                WFPElencoNazioni.Show()
            Case F_NAZIONIIBAN
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
        End Select

        WFPZone.WucElement = Me
        If Session(F_ANAGRZONE_APERTA) = True Then
            WFPZone.Show()
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
        WFP_BancheIBAN1.WucElement = Me
        If Session(F_BANCHEIBAN_APERTA) = True Then
            WFP_BancheIBAN1.Show()
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
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        If txtCodFornitore.Text.Trim = "" Then Exit Sub

        If Session(TIPO_AGG_FOR) = SEL_BTN_NUOVO Or Session(SWOP) = SWOPNUOVO Then
            If CheckNewCodiceFornitore() = True Then
                txtCodFornitore.BackColor = Def.SEGNALA_KO
                Exit Sub
            Else
                txtCodFornitore.BackColor = Def.SEGNALA_OK
            End If
        End If
    End Sub

    Private Sub txtCodNazione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodNazione.TextChanged
        lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodNazione, lblNazione)
    End Sub
    Private Sub txtCodNazioneIBAN_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodNazioneIBAN.TextChanged
        lblNazioneIBAN.Text = App.GetValoreFromChiave(txtCodNazioneIBAN.Text, Def.NAZIONI, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodNazioneIBAN, lblNazioneIBAN)
        Session("AvvisaIBAN") = True
        Call CKIBAN()
    End Sub
    Private Sub CKIBAN()
        lblIBANFor.Text = IIf(txtCodNazioneIBAN.Text.Trim = "", "[Naz.IBAN]", txtCodNazioneIBAN.Text.Trim)
        lblIBANFor.Text = lblIBANFor.Text & " " & IIf(txtCINEU.Text.Trim = "", "[CIN Eu.]", txtCINEU.Text.Trim)
        lblIBANFor.Text = lblIBANFor.Text & " " & IIf(txtCIN.Text.Trim = "", "[CIN Naz.]", txtCIN.Text.Trim)
        lblIBANFor.Text = lblIBANFor.Text & " " & IIf(txtCodABI.Text.Trim = "", "[ABI]", txtCodABI.Text.Trim)
        lblIBANFor.Text = lblIBANFor.Text & " " & IIf(txtCodCAB.Text.Trim = "", "[CAB]", txtCodCAB.Text.Trim)
        lblIBANFor.Text = lblIBANFor.Text & " " & IIf(txtNumCC.Text.Trim = "", "[Numero C/C]", txtNumCC.Text.Trim)
        '-
    End Sub

    ' ''Private Sub txtCodAgente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAgente.TextChanged
    ' ''    lblAgente.Text = App.GetValoreFromChiave(txtCodAgente.Text, Def.AGENTI, Session(ESERCIZIO))
    ' ''    CheckInserimentoCodiceTL(txtCodAgente, lblAgente)
    ' ''End Sub

    ' ''Private Sub txtCodAgenteEsePrec_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodAgenteEsePrec.TextChanged
    ' ''    txtAgenteEsePrec.Text = App.GetValoreFromChiave(txtCodAgenteEsePrec.Text, Def.AGENTI, Session(ESERCIZIO))
    ' ''    CheckInserimentoCodice(txtCodAgenteEsePrec, txtAgenteEsePrec)
    ' ''End Sub

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
        CheckInserimentoCodiceTL(txtCodRegimeIVA, lblRegimeIva)
    End Sub

    Private Sub txtCodRicavoFT_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodRicavoFT.TextChanged
        txtRicavoFT.Text = App.GetValoreFromChiave(txtCodRicavoFT.Text, Def.PIANODEICONTI, Session(ESERCIZIO))
        CheckInserimentoCodice(txtCodRicavoFT, txtRicavoFT)
    End Sub

    ' ''Private Sub txtCodVettore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodVettore.TextChanged
    ' ''    lblVettore.Text = App.GetValoreFromChiave(txtCodVettore.Text, Def.VETTORI, Session(ESERCIZIO))
    ' ''    CheckInserimentoCodiceTL(txtCodVettore, lblVettore)
    ' ''End Sub

    Private Sub txtCodZona_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodZona.TextChanged
        lblZona.Text = App.GetValoreFromChiave(txtCodZona.Text, Def.ZONE, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodZona, lblZona)
    End Sub

    Private Sub txtCodSede_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodSede.TextChanged
        lblSede.Text = App.GetValoreFromChiave(txtCodSede.Text, Def.FORNITORI, Session(ESERCIZIO))
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
        End If
        lblFiliale.Text = GetDatiFiliali(txtCodABI.Text.Trim, txtCodCAB.Text.Trim, strErrore).Filiale
        If txtCodABI.Text.Trim <> "" And lblFiliale.Text.Trim = "" Then
            lblFiliale.Text = "[Inesistente]"
        End If
        ' ''CheckInserimentoCodice(txtCodCAB, txtFiliale)
        ' ''CheckInserimentoCodice(txtCodABI, txtBanca)
        Call CKIBAN()
        Session("AvvisaIBAN") = True
        txtCodCAB.Focus()
    End Sub

    Private Sub txtCodCAB_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCAB.TextChanged
        lblFiliale.Text = GetDatiFiliali(txtCodABI.Text.Trim, txtCodCAB.Text.Trim, strErrore).Filiale
        If txtCodABI.Text.Trim <> "" And lblFiliale.Text.Trim = "" Then
            lblFiliale.Text = "[Inesistente]"
        End If
        ' ''CheckInserimentoCodice(txtCodCAB, txtFiliale)
        Call CKIBAN()
        Session("AvvisaIBAN") = True
        txtNumCC.Focus()
    End Sub

#End Region

#Region "Pulsanti"

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicerca.Click
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori()
    End Sub

    Private Sub btnTrovaSede_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaSede.Click
        Session(F_FOR_RICERCA) = False
        ApriElencoFornitori()
    End Sub

    Private Sub btnTrovaNazione_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaNazione.Click
        ApriElenco(F_NAZIONI)
    End Sub
    Private Sub btnNazioneIBAN_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNazioneIBAN.Click
        ApriElenco(F_NAZIONIIBAN)
    End Sub

    Private Sub btnTrovaProvincia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaProvincia.Click
        ApriElenco(F_PROVINCE)
    End Sub

    Private Sub btnTrovaRegimeIVA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaRegimeIVA.Click
        ApriElenco(F_ALIQUOTAIVA)
    End Sub

    '' ''Private Sub btnTrovaAgente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaAgente.Click
    '' ''    ApriElenco(F_AGENTI)
    '' ''End Sub

    '' ''Private Sub btnTrovaAgenteEsePrec_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaAgenteEsePrec.Click
    '' ''    ApriElenco(F_AGENTI_ESE_PREC)
    '' ''End Sub

    Private Sub btnTrovaZona_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaZona.Click
        ApriElenco(F_ZONE)
    End Sub

    ' ''Private Sub btnTrovaVettore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTrovaVettore.Click
    ' ''    ApriElenco(F_VETTORI)
    ' ''End Sub

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
        SvuotaCampi()
        CaricaVariabili()
        Session(SWOP) = SWOPNESSUNA
        CampiSetEnabledTo(False)
        LocalizzaFornitore(FOR_CORRENTE)
        lblLabelNEW.Visible = False
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session("AvvisaDoppioCF") = True
        Session("AvvisaDoppioPI") = True
        Session("AvvisaIBAN") = True
        setPulsantiModalitaAggiorna()
        Session(SWOP) = SWOPMODIFICA
        CampiSetEnabledTo(True)
        lblLabelNEW.Visible = False
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        Session("AvvisaDoppioCF") = True
        Session("AvvisaDoppioPI") = True
        Session("AvvisaIBAN") = True
        setPulsantiModalitaAggiorna()

        SvuotaCampi()
        Session(SWOP) = SWOPNUOVO
        CampiSetEnabledTo(True)
        Tabs.ActiveTabIndex = 0
        txtCodFornitore.Text = LocalizzaNEWFornitore()
        Session(CSTCODCOGEDM) = txtCodFornitore.Text.Trim 'GIU281111
        Session(COD_FORNITORE) = txtCodFornitore.Text.Trim
        lblLabelNEW.Text = "Nuovo codice " & txtCodFornitore.Text
        lblLabelNEW.Visible = True

        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim strErrAll As String = ""
        txtCodListino.Text = "" 'giu121211 "9"
        If App.GetDatiAbilitazioni(CSTABILCOGE, "ListinoFor", strValore, strErrore) = True Then
            txtCodListino.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        lblListino.Text = App.GetValoreFromChiave(txtCodListino.Text, Def.LISTVEN_T, Session(ESERCIZIO))
        CheckInserimentoCodiceTL(txtCodListino, lblListino)
        txtCodNazione.Text = "I"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "NazioneFor", strValore, strErrore) = True Then
            txtCodNazione.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodNazione.Text.Trim <> "" Then
            lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
            CheckInserimentoCodiceTL(txtCodNazione, lblNazione)
        End If
        '-
        txtCodNazioneIBAN.Text = "IT"
        If txtCodNazioneIBAN.Text.Trim <> "" Then
            lblNazioneIBAN.Text = App.GetValoreFromChiave(txtCodNazioneIBAN.Text, Def.NAZIONI, Session(ESERCIZIO))
            CheckInserimentoCodiceTL(txtCodNazioneIBAN, lblNazioneIBAN)
        End If
        '--------

        rdPersonaGiuridica.Checked = True
        txtDataNascita.Enabled = False
        imgBtnShowCalendar.Enabled = False
        txtCodRegimeIVA.Text = "0" : lblRegimeIva.Text = "REGIME NORMALE"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "RegIVAFor", strValore, strErrore) = True Then
            txtCodRegimeIVA.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodRegimeIVA.Text.Trim <> "" Then
            lblRegimeIva.Text = App.GetValoreFromChiave(txtCodRegimeIVA.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
            CheckInserimentoCodiceTL(txtCodRegimeIVA, lblRegimeIva)
        End If
        'giu251111 txtCodPagamento.Text = "35"
        strErrore = "" : strValore = ""
        If App.GetDatiAbilitazioni(CSTABILCOGE, "CodPagFor", strValore, strErrore) = True Then
            txtCodPagamento.Text = strValore.Trim
        End If
        If strErrore.Trim <> "" Then
            strErrAll += strErrore + " <br> "
        End If
        If txtCodPagamento.Text.Trim <> "" Then
            lblPagamento.Text = App.GetValoreFromChiave(txtCodPagamento.Text, Def.PAGAMENTI, Session(ESERCIZIO))
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
        '-----------------
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
    Private Function LocalizzaNEWFornitore() As String
        Dim NewCodice As Long = 0
        Dim Format0 As String = "" : Dim Format9 As String = ""
        For i = 2 To txtCodFornitore.MaxLength
            Format0 += "0" : Format9 += "9"
        Next
        Dim listaFornitori As ArrayList = App.GetLista(Def.FORNITORI, Session(ESERCIZIO))
        Dim posizione As Integer = 0
        If (listaFornitori.Count > 0) Then
            posizione = listaFornitori.Count - 1
            If (posizione < 0) Then
                NewCodice = 0
            Else
                Dim Ultimo As String = listaFornitori(posizione).Codice_CoGe
                NewCodice = CInt(Trim(Mid(Ultimo, 2)))
            End If
        Else
            NewCodice = 0
        End If
        NewCodice += 1
        If NewCodice > CInt(Format9) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nuovo fornitore", "Attenzione, superato il limite massimo nuovi codici fornitori.", WUC_ModalPopup.TYPE_INFO)
            LocalizzaNEWFornitore = ""
            Exit Function
        End If
        LocalizzaNEWFornitore = "9" & Format(NewCodice, Format0)
    End Function

    Private Sub btnNEXT_FORck(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click
        LocalizzaFornitore(NEXT_FOR)
    End Sub

    Private Sub btnLAST_FORck(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLast.Click
        LocalizzaFornitore(LAST_FOR)
    End Sub

    Private Sub btnPREV_FORck(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrev.Click
        LocalizzaFornitore(PREV_FOR)
    End Sub

    Private Sub btnFIRST_FORck(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        LocalizzaFornitore(FIRST_FOR)
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Dim myFornitore As FornitoreEntity = Nothing
        Dim errorMsg As String = String.Empty

        If Session(SWOP) = SWOPNUOVO Then
            If CheckNewCodiceFornitore() = True Then
                txtCodFornitore.BackColor = Def.SEGNALA_KO
                Exit Sub
            Else
                txtCodFornitore.BackColor = Def.SEGNALA_OK
            End If
        End If
        Dim strElErrori As String = "" 'giu160113
        ControlloCampiObbligatori(strElErrori)
        If (Session(SWERRORI_AGGIORNAMENTO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati fornitore", "Attenzione, i campi segnalati in rosso non sono validi: <br>" & strElErrori, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        InitCampi(myFornitore)
        ControllaEsistenzaCampiInErrore(strElErrori)
        If (Session(SWERRORI_AGGIORNAMENTO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati fornitore", "Attenzione, i campi segnalati in rosso non sono validi: <br>" & strElErrori, WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If

        If (ControlloCampiDati()) Then 'giu160113 c'è il controllo del C.F. e P.I. SE SONO DOPPI (STAMPA)
            Aggiornamento(myFornitore)
            setPulsantiModalitaConsulta()
            If (Not App.CaricaFornitori(Session(ESERCIZIO), errorMsg)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Caricamento Dati", _
                    String.Format("Errore nel caricamento Elenco Fornitore, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                    WUC_ModalPopup.TYPE_INFO)
                SessionUtility.LogOutUtente(Session, Response)
                Exit Sub
            End If
            LocalizzaFornitore(FOR_CORRENTE)
            lblLabelNEW.Visible = False
        Else
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        CaricaVariabili()
    End Sub

    Dim CIFornitoreCG As Boolean = True : Dim CIFornitoreAZI As Boolean = True : Dim CIFornitoreSCAD As Boolean = True
    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click

        CIFornitoreCG = True : CIFornitoreAZI = True : CIFornitoreSCAD = True
        Dim ForSys As New Fornitore
        CIFornitoreCG = ForSys.CIFornitoreByCodice(Session(COD_FORNITORE))
        CIFornitoreAZI = ForSys.CIFornitoreByCodiceAZI(Session(COD_FORNITORE))
        CIFornitoreSCAD = ForSys.CIFornitoreByCodiceSCAD(Session(COD_FORNITORE))
        If CIFornitoreCG = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella contabilità, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CIFornitoreAZI = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella gestione aziendale, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CIFornitoreSCAD = False Then 'GIU261111 IDEM PER AZI NEI DOCUMENTI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nello scadenzario, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfermaEliminaFornitore"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Elimina fornitore", "Si vuole cancellare il fornitore selezionato?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub

#End Region

#Region "Radio button"

    Private Sub rdPersonaFisica_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdPersonaFisica.CheckedChanged
        If (rdPersonaFisica.Checked) Then
            txtDataNascita.Enabled = True
            imgBtnShowCalendar.Enabled = True
            txtDataNascita_CalendarExtender.Enabled = True
            txtCodSede.Text = String.Empty
            lblSede.Text = String.Empty
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
        Session(SWOP) = SWOPNESSUNA
        Session(F_FOR_RICERCA) = False
        Session(SWERRORI_AGGIORNAMENTO) = False
    End Sub

#End Region

#Region "Metodi private"

#Region "Gestione Campi"

    Private Sub CampiSetEnabledTo(ByVal valore As Boolean)
        '--- intestazione ---
        If valore = True Then
            If Session(TIPO_AGG_FOR) = SEL_BTN_NUOVO Or Session(SWOP) = SWOPNUOVO Then
                txtCodFornitore.Enabled = valore
            End If
        Else
            txtCodFornitore.Enabled = valore
        End If
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
        txtCodNazioneIBAN.Enabled = valore
        txtPartitaIVA.Enabled = valore
        txtTelefono1.Enabled = valore
        txtTelefono2.Enabled = valore
        txtTelefonoFax.Enabled = valore
        txtCodFiscale.Enabled = valore
        txtEmail.Enabled = valore
        txtPECEmail.Enabled = valore
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
        btnNazioneIBAN.Enabled = valore
        rdPersonaFisica.Enabled = valore
        rdPersonaGiuridica.Enabled = valore
        '--- commerciale ---
        txtCodRegimeIVA.Enabled = valore
        txtCodPagamento.Enabled = valore

        txtCodNazioneIBAN.Enabled = valore
        txtCodABI.Enabled = valore
        txtCodCAB.Enabled = valore
        txtNumCC.Enabled = valore
        txtSWIFT.Enabled = valore
        txtCIN.Enabled = valore
        txtCINEU.Enabled = valore
        'txtCodAgente.Enabled = valore
        'txtCodAgenteEsePrec.Enabled = valore
        txtCodZona.Enabled = valore
        'txtCodVettore.Enabled = valore
        txtCodCategoria.Enabled = valore
        txtCodListino.Enabled = valore
        txtCodRicavoFT.Enabled = valore
        txtMaxCredito.Enabled = valore
        btnTrovaRegimeIVA.Enabled = valore
        btnTrovaPagamento.Enabled = valore
        'btnTrovaAgente.Enabled = valore
        'btnTrovaAgenteEsePrec.Enabled = valore
        btnTrovaZona.Enabled = valore
        'btnTrovaVettore.Enabled = valore
        btnTrovaCategoria.Enabled = valore
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

        btnZone.Enabled = valore
        'btnVettori.Enabled = valore
        'btnAgente.Enabled = valore
        btnCategorie.Enabled = valore
        btnCercaBanca.Enabled = valore
        DDLBancheIBAN.Enabled = valore
    End Sub

    Private Sub SvuotaCampi()
        '--- intestazione ---                           
        ResetCampoStringaValidato(txtCodFornitore)
        ResetCampoStringaValidato(txtRagSoc)
        '--- dati anagrafici ---                        
        ResetCampoStringaValidato(txtDenominazione)
        ResetCampoStringaValidato(txtTitolare)
        ResetCampoStringaValidato(txtIndirizzo)
        ResetCampoStringaValidato(txtLocalita)
        txtNumCivico.Text = String.Empty
        ResetCampoStringaValidato(txtProvincia)
        ResetCampoStringaValidato(txtCap)
        ResetCampoStringaValidato(txtCodNazione)
        ResetCampoStringaValidato(txtCodNazioneIBAN)
        lblNazione.Text = String.Empty
        ResetCampoStringaValidato(txtPartitaIVA)
        txtTelefono1.Text = String.Empty
        txtTelefono2.Text = String.Empty
        txtTelefonoFax.Text = String.Empty
        ResetCampoStringaValidato(txtCodFiscale)
        ResetCampoStringaValidato(txtEmail)
        ResetCampoStringaValidato(txtPECEmail)
        txtRiferimento.Text = String.Empty
        ResetCampoStringaValidato(txtCodSede)
        lblSede.Text = String.Empty
        checkAllIVA.Checked = False
        ResetCampoStringaValidato(txtDataNascita)
        rdPersonaGiuridica.Checked = False
        rdPersonaFisica.Checked = False
        '--- commerciale ---                            
        ResetCampoStringaValidato(txtCodRegimeIVA)
        lblRegimeIva.Text = String.Empty
        ResetCampoStringaValidato(txtCodPagamento)
        lblPagamento.Text = String.Empty

        ResetCampoStringaValidato(txtCodNazioneIBAN)
        lblNazioneIBAN.Text = String.Empty
        ResetCampoStringaValidato(txtCodABI)
        lblBanca.Text = ""
        ResetCampoStringaValidato(txtCodCAB)
        lblFiliale.Text = ""
        ResetCampoStringaValidato(txtNumCC)
        ResetCampoStringaValidato(txtSWIFT)
        ResetCampoStringaValidato(txtCIN)
        ResetCampoStringaValidato(txtCINEU)
        'ResetCampoStringaValidato(txtCodAgente)
        'lblAgente.Text = String.Empty
        'ResetCampoStringaValidato(txtCodAgenteEsePrec)
        'txtAgenteEsePrec.Text = String.Empty
        ResetCampoStringaValidato(txtCodZona)
        lblZona.Text = String.Empty
        'ResetCampoStringaValidato(txtCodVettore)
        'lblVettore.Text = String.Empty
        ResetCampoStringaValidato(txtCodCategoria)
        lblCategorie.Text = String.Empty
        ResetCampoStringaValidato(txtCodListino)
        lblListino.Text = String.Empty
        ResetCampoStringaValidato(txtCodRicavoFT)
        ResetCampoStringaValidato(txtRicavoFT)
        txtMaxCredito.Text = FormattaNumero("0", 2)
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
    End Sub

    Private Sub PopolaCampi(ByVal Fornitore As FornitoreEntity)
        '--- intestazione ---
        txtCodFornitore.Text = Fornitore.Codice_CoGe : txtCodFornitore.BackColor = Def.SEGNALA_OK
        txtRagSoc.Text = Fornitore.Rag_Soc : txtRagSoc.BackColor = Def.SEGNALA_OK
        '--- dati anagrafici ---
        txtDenominazione.Text = Fornitore.Denominazione : txtDenominazione.BackColor = Def.SEGNALA_OK
        txtTitolare.Text = Fornitore.Titolare : txtTitolare.BackColor = Def.SEGNALA_OK
        txtIndirizzo.Text = Fornitore.Indirizzo : txtIndirizzo.BackColor = Def.SEGNALA_OK
        txtLocalita.Text = Fornitore.Localita : txtLocalita.BackColor = Def.SEGNALA_OK
        txtNumCivico.Text = Fornitore.NumeroCivico : txtNumCivico.BackColor = Def.SEGNALA_OK
        txtProvincia.Text = Fornitore.Provincia : txtProvincia.BackColor = Def.SEGNALA_OK
        txtCap.Text = Fornitore.CAP : txtCap.BackColor = Def.SEGNALA_OK
        txtCodNazione.Text = Fornitore.Nazione : txtCodNazione.BackColor = Def.SEGNALA_OK
        lblNazione.Text = App.GetValoreFromChiave(txtCodNazione.Text, Def.NAZIONI, Session(ESERCIZIO))
        If txtCodNazione.Text.Trim <> "I" And txtCodNazione.Text.Trim <> "IT" And txtCodNazione.Text.Trim <> "ITA" Then
            txtPartitaIVA.MaxLength = 20
        Else
            txtPartitaIVA.MaxLength = 11
        End If

        txtPartitaIVA.Text = Fornitore.Partita_IVA : txtPartitaIVA.BackColor = Def.SEGNALA_OK
        txtTelefono1.Text = Fornitore.Telefono1 : txtTelefono1.BackColor = Def.SEGNALA_OK
        txtTelefono2.Text = Fornitore.Telefono2 : txtTelefono2.BackColor = Def.SEGNALA_OK
        txtTelefonoFax.Text = Fornitore.Fax : txtTelefonoFax.BackColor = Def.SEGNALA_OK
        txtCodFiscale.Text = Fornitore.Codice_Fiscale : txtCodFiscale.BackColor = Def.SEGNALA_OK
        txtEmail.Text = Fornitore.Email : txtEmail.BackColor = Def.SEGNALA_OK
        txtPECEmail.Text = Fornitore.PECEMail : txtPECEmail.BackColor = Def.SEGNALA_OK
        txtRiferimento.Text = Fornitore.Riferimento : txtRiferimento.BackColor = Def.SEGNALA_OK
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        txtCodSede.Text = Fornitore.Codice_SEDE : txtCodSede.BackColor = Def.SEGNALA_OK
        'giu121211
        If txtCodSede.Text.Trim <> "" Then
            lblSede.Text = App.GetValoreFromChiave(txtCodSede.Text, Def.FORNITORI, Session(ESERCIZIO))
        Else
            lblSede.Text = ""
        End If
        'giu121211------------------
        txtDataNascita.Text = IIf(Fornitore.Data_Nascita = CDate(DATANULL), "", Format(Fornitore.Data_Nascita, FormatoData)) 'GIU231011
        txtDataNascita.BackColor = Def.SEGNALA_OK
        checkAllIVA.Checked = Fornitore.CSAggrAllIVA
        If Fornitore.Societa Then
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
        txtCodRegimeIVA.Text = Fornitore.Regime_IVA : txtCodRegimeIVA.BackColor = Def.SEGNALA_OK
        lblRegimeIva.Text = App.GetValoreFromChiave(txtCodRegimeIVA.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))

        txtCodPagamento.Text = Fornitore.Pagamento_N : txtCodPagamento.BackColor = Def.SEGNALA_OK
        lblPagamento.Text = App.GetValoreFromChiave(txtCodPagamento.Text, Def.PAGAMENTI, Session(ESERCIZIO))
        '-
        txtCodNazioneIBAN.Text = Fornitore.NazIBAN : txtCodNazioneIBAN.BackColor = Def.SEGNALA_OK
        lblNazioneIBAN.Text = App.GetValoreFromChiave(txtCodNazioneIBAN.Text, Def.NAZIONI, Session(ESERCIZIO))
        txtCodABI.Text = Fornitore.ABI_N.Trim : txtCodABI.BackColor = Def.SEGNALA_OK
        txtCodCAB.Text = Fornitore.CAB_N.Trim : txtCodCAB.BackColor = Def.SEGNALA_OK
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
        txtNumCC.Text = Fornitore.Conto_Corrente : txtNumCC.BackColor = Def.SEGNALA_OK
        txtSWIFT.Text = Fornitore.SWIFT : txtSWIFT.BackColor = Def.SEGNALA_OK
        '
        txtCIN.Text = Fornitore.CIN : txtCIN.BackColor = Def.SEGNALA_OK
        txtCINEU.Text = Fornitore.CINEUIBAN : txtCINEU.BackColor = Def.SEGNALA_OK
        '-
        Call CKIBAN()
        '-
        'giu121211 txtCodAgente.Text = Fornitore.Agente_N

        'Pier220612 ======================================
        lblIBAN.Text = Fornitore.IBAN_Ditta
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
        '== Fine Pier =====================================

        'lblAgente.Text = App.GetValoreFromChiave(txtCodAgente.Text, Def.AGENTI, Session(ESERCIZIO))
        'giu121211 txtCodAgenteEsePrec.Text = Fornitore.Agente_N_Prec
        'txtAgenteEsePrec.Text = App.GetValoreFromChiave(txtCodAgenteEsePrec.Text, Def.AGENTI, Session(ESERCIZIO))
        txtCodZona.Text = Fornitore.Zona : txtCodZona.BackColor = Def.SEGNALA_OK
        lblZona.Text = App.GetValoreFromChiave(txtCodZona.Text, Def.ZONE, Session(ESERCIZIO))
        'giu121211 txtCodVettore.Text = Fornitore.Vettore_N
        'lblVettore.Text = App.GetValoreFromChiave(txtCodVettore.Text, Def.VETTORI, Session(ESERCIZIO))
        txtCodCategoria.Text = Fornitore.Categoria : txtCodCategoria.BackColor = Def.SEGNALA_OK
        lblCategorie.Text = App.GetValoreFromChiave(txtCodCategoria.Text, Def.CATEGORIE, Session(ESERCIZIO))
        txtCodListino.Text = Fornitore.Listino : txtCodListino.BackColor = Def.SEGNALA_OK
        lblListino.Text = App.GetValoreFromChiave(txtCodListino.Text, Def.LISTVEN_T, Session(ESERCIZIO))
        'giu121211 txtCodRicavoFT.Text = Fornitore.Codice_Ricavo
        txtRicavoFT.Text = App.GetValoreFromChiave(txtCodRicavoFT.Text, Def.PIANODEICONTI, Session(ESERCIZIO))
        txtRicavoFT.BackColor = Def.SEGNALA_OK
        'giu121211 txtMaxCredito.Text = Format(Fornitore.Credito_1, FormatoValEuro)
        checkEscudiAllegatiIVA.Checked = IIf(Fornitore.Allegato_IVA = 0, False, True)
        'giu121211 checkIVAInSospensione.Checked = IIf(Fornitore.IVASosp = 0, False, True)
        checkNonFatturabile.Checked = IIf(Fornitore.NoFatt = 0, False, True)
        'giu121211 
        ' ''checkMattino1.Checked = IIf(Fornitore.ChiusuraMattino_1 = 0, False, True)
        ' ''checkMattino2.Checked = IIf(Fornitore.ChiusuraMattino_2 = 0, False, True)
        ' ''checkPomeriggio1.Checked = IIf(Fornitore.ChiusuraPomeriggio_1 = 0, False, True)
        ' ''checkPomeriggio2.Checked = IIf(Fornitore.ChiusuraPomeriggio_2 = 0, False, True)
        ' ''ddlPrimoGiorno1.SelectedIndex = Fornitore.GiornoChiusura_1
        ' ''ddlPrimoGiorno2.SelectedIndex = Fornitore.GiornoChiusura_2
        'giu121211 
        '--- saldi ---
        'GIU060113
        lblSaldoAperturaDare.Text = Format(0, FormatoValEuro)
        lblSaldoAperturaAvere.Text = Format(0, FormatoValEuro)
        If Not IsDBNull(Fornitore.DA_Apertura) Then
            If Fornitore.DA_Apertura.Trim <> "" Then
                If Fornitore.DA_Apertura.Trim = "D" Then
                    lblSaldoAperturaDare.Text = Format(Fornitore.Apertura, FormatoValEuro)
                ElseIf Fornitore.DA_Apertura.Trim = "A" Then
                    lblSaldoAperturaAvere.Text = Format(Fornitore.Apertura, FormatoValEuro)
                End If
            End If
        End If
        '---------
        lblSaldoAttualeAvere.Text = Format(Fornitore.Avere_Chiusura, FormatoValEuro)
        lblSaldoAttualeDare.Text = Format(Fornitore.Dare_Chiusura, FormatoValEuro)
        lblSaldoChiusuraAvere.Text = Format(Fornitore.Avere_Chiusura, FormatoValEuro)
        lblSaldoChiusuraDare.Text = Format(Fornitore.Dare_Chiusura, FormatoValEuro)
        lblSaldoProgAver.Text = Format(Fornitore.Saldo_Avere, FormatoValEuro)
        lblSaldoProgDare.Text = Format(Fornitore.Saldo_Dare, FormatoValEuro)
        'giu160113
        If lblSaldoProgAver.Text.Trim <> "" And IsNumeric(lblSaldoProgAver.Text.Trim) And lblSaldoProgDare.Text.Trim <> "" And IsNumeric(lblSaldoProgDare.Text.Trim) Then
            If CDec(lblSaldoProgAver.Text.Trim) > CDec(lblSaldoProgDare.Text.Trim) Then
                lblSaldoAttualeAvere.Text = FormattaNumero(CDec(lblSaldoProgAver.Text.Trim) - CDec(lblSaldoProgDare.Text.Trim), 2)
            Else
                lblSaldoAttualeDare.Text = FormattaNumero(CDec(lblSaldoProgDare.Text.Trim) - CDec(lblSaldoProgAver.Text.Trim), 2)
            End If
        End If
        '----------------
        If CDate(Fornitore.Data_Agg_Saldi) = DATANULL Then
            lblDataSaldoUltimoAgg.Text = ""
        Else
            lblDataSaldoUltimoAgg.Text = Format(Fornitore.Data_Agg_Saldi, FormatoData)
        End If
        '--- note ---
        'giu121211 txtNote.Text = Fornitore.Note
    End Sub

    Private Function InitCampi(ByRef myFornitore As FornitoreEntity) As Boolean
        myFornitore = New FornitoreEntity

        myFornitore.Codice_CoGe = txtCodFornitore.Text
        myFornitore.Rag_Soc = txtRagSoc.Text
        myFornitore.Denominazione = txtDenominazione.Text
        myFornitore.Titolare = txtTitolare.Text
        myFornitore.Indirizzo = txtIndirizzo.Text
        myFornitore.Localita = txtLocalita.Text
        myFornitore.NumeroCivico = txtNumCivico.Text
        myFornitore.Provincia = txtProvincia.Text.Trim.ToUpper
        myFornitore.CAP = txtCap.Text
        myFornitore.Nazione = txtCodNazione.Text.Trim.ToUpper
        myFornitore.NazIBAN = txtCodNazioneIBAN.Text.Trim.ToUpper
        myFornitore.Partita_IVA = txtPartitaIVA.Text
        myFornitore.Telefono1 = txtTelefono1.Text
        myFornitore.Telefono2 = txtTelefono2.Text
        myFornitore.Fax = txtTelefonoFax.Text
        myFornitore.Codice_Fiscale = txtCodFiscale.Text.Trim.ToUpper
        myFornitore.Email = txtEmail.Text.Trim
        myFornitore.PECEMail = txtPECEmail.Text.Trim
        myFornitore.Riferimento = txtRiferimento.Text
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        If (rdPersonaGiuridica.Checked) Then
            myFornitore.Societa = -1
        Else
            myFornitore.Societa = 0
        End If
        myFornitore.Data_Nascita = GetDataValida(txtDataNascita)
        myFornitore.CSAggrAllIVA = checkAllIVA.Checked
        myFornitore.Regime_IVA = GetCodiceNumericoValido(txtCodRegimeIVA)
        myFornitore.Pagamento_N = GetCodiceNumericoValido(txtCodPagamento)
        myFornitore.ABI_N = txtCodABI.Text.Trim
        myFornitore.CAB_N = txtCodCAB.Text.Trim
        myFornitore.Conto_Corrente = txtNumCC.Text.Trim
        myFornitore.SWIFT = txtSWIFT.Text.Trim
        myFornitore.CIN = txtCIN.Text.Trim.ToUpper
        myFornitore.CINEUIBAN = txtCINEU.Text.Trim.ToUpper
        'giu121211 myFornitore.Agente_N = GetCodiceNumericoValido(txtCodAgente)
        'giu121211 myFornitore.Agente_N_Prec = GetCodiceNumericoValido(txtCodAgenteEsePrec)
        myFornitore.Zona = GetCodiceNumericoValido(txtCodZona)
        'giu121211 myFornitore.Vettore_N = GetCodiceNumericoValido(txtCodVettore)
        myFornitore.Categoria = GetCodiceNumericoValido(txtCodCategoria)
        myFornitore.Listino = GetCodiceNumericoValido(txtCodListino)
        'giu121211 myFornitore.Codice_Ricavo = txtCodRicavoFT.Text
        'giu121211 myFornitore.Credito_1 = CDec(txtMaxCredito.Text)
        'GIU231011 corretto è stato interpretato all'incontrario e non si memorizza qui il codice sede
        myFornitore.Codice_SEDE = txtCodSede.Text.Trim
        myFornitore.Allegato_IVA = IIf(checkEscudiAllegatiIVA.Checked, -1, 0)
        'giu121211 myFornitore.IVASosp = IIf(checkIVAInSospensione.Checked, -1, 0)
        myFornitore.NoFatt = IIf(checkNonFatturabile.Checked, -1, 0)
        'giu121211 myFornitore.ChiusuraMattino_1 = IIf(checkMattino1.Checked, -1, 0)
        'giu121211 myFornitore.ChiusuraMattino_2 = IIf(checkMattino2.Checked, -1, 0)
        'giu121211 myFornitore.ChiusuraPomeriggio_1 = IIf(checkPomeriggio1.Checked, -1, 0)
        'giu121211 myFornitore.ChiusuraPomeriggio_2 = IIf(checkPomeriggio2.Checked, -1, 0)
        'giu121211 myFornitore.GiornoChiusura_1 = ddlPrimoGiorno1.SelectedIndex
        'giu121211 myFornitore.GiornoChiusura_2 = ddlPrimoGiorno2.SelectedIndex
        'giu251111 DATO NON MODIFICABILE DA QUI
        ' ''myFornitore.Apertura = CDec(txtSaldoAperturaAvere.Text)
        ' ''myFornitore.Apertura_2 = CDec(txtSaldoAperturaDare.Text)
        ' ''myFornitore.Avere_Chiusura = CDec(txtSaldoAttualeAvere.Text)
        ' ''myFornitore.Dare_Chiusura = CDec(txtSaldoAttualeDare.Text)
        ' ''myFornitore.Avere_Chiusura = CDec(txtSaldoChiusuraAvere.Text)
        ' ''myFornitore.Dare_Chiusura = CDec(txtSaldoChiusuraDare.Text)
        ' ''myFornitore.Saldo_Avere = CDec(txtSaldoProgAver.Text)
        ' ''myFornitore.Saldo_Dare = CDec(txtSaldoProgDare.Text)
        ' ''myFornitore.Data_Agg_Saldi = GetDataValida(txtDataSaldoUltimoAgg)
        'giu121211 myFornitore.Note = txtNote.Text
        myFornitore.Ragg_P = App.GetProgressiviCoGe(Session(ESERCIZIO)).Ragg_Bil_Fornit
        If Session(TIPO_AGG_FOR) = SEL_BTN_NUOVO Then
            myFornitore.InseritoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Else
            myFornitore.ModificatoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        End If
        myFornitore.IBAN_Ditta = lblIBAN.Text.Trim.Trim.ToUpper
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
    'giu160113
    'giu051118
    Private Sub ControllaEsistenzaCampiInErrore(ByRef _ElErr As String)
        _ElErr = ""
        Session(SWERRORI_AGGIORNAMENTO) = False
        If ( _
            txtCodNazione.BackColor = Def.SEGNALA_KO Or _
            txtCodNazioneIBAN.BackColor = Def.SEGNALA_KO Or _
            txtProvincia.BackColor = Def.SEGNALA_KO Or _
            txtCodSede.BackColor = Def.SEGNALA_KO Or _
            txtCodRegimeIVA.BackColor = Def.SEGNALA_KO Or _
            txtCodPagamento.BackColor = Def.SEGNALA_KO Or _
            txtCodABI.BackColor = Def.SEGNALA_KO Or _
            txtCodCAB.BackColor = Def.SEGNALA_KO Or _
            txtCodZona.BackColor = Def.SEGNALA_KO Or _
            txtCodCategoria.BackColor = Def.SEGNALA_KO Or _
            txtCodListino.BackColor = Def.SEGNALA_KO Or _
            txtCodRicavoFT.BackColor = Def.SEGNALA_KO Or _
            txtEmail.BackColor = Def.SEGNALA_KO Or _
            txtPECEmail.BackColor = Def.SEGNALA_KO _
        ) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
            If txtCodNazione.BackColor = Def.SEGNALA_KO Then _ElErr += " - Nazione"
            If txtCodNazioneIBAN.BackColor = Def.SEGNALA_KO Then _ElErr += " - Nazione IBAN"
            If txtProvincia.BackColor = Def.SEGNALA_KO Then _ElErr += " - Provincia"
            If txtCodSede.BackColor = Def.SEGNALA_KO Then _ElErr += " - Sede"
            If txtCodRegimeIVA.BackColor = Def.SEGNALA_KO Then _ElErr += " - Regime IVA"
            If txtCodPagamento.BackColor = Def.SEGNALA_KO Then _ElErr += " - Pagamento"
            If txtCodABI.BackColor = Def.SEGNALA_KO Then _ElErr += " - ABI"
            If txtCodCAB.BackColor = Def.SEGNALA_KO Then _ElErr += " - CAB"
            If txtCodZona.BackColor = Def.SEGNALA_KO Then _ElErr += " - Zona"
            If txtCodCategoria.BackColor = Def.SEGNALA_KO Then _ElErr += " - Categoria"
            If txtCodListino.BackColor = Def.SEGNALA_KO Then _ElErr += " - Listino"
            If txtCodRicavoFT.BackColor = Def.SEGNALA_KO Then _ElErr += " - Cod. Ricavo"
            If txtEmail.BackColor = Def.SEGNALA_KO Or _
                txtPECEmail.BackColor = Def.SEGNALA_KO Then
                _ElErr += " - E-mail"
            End If
        End If

        'txtCodAgente.BackColor = Def.SEGNALA_KO Or _
        'txtCodAgenteEsePrec.BackColor = Def.SEGNALA_KO Or _
        'txtCodVettore.BackColor = Def.SEGNALA_KO Or _

    End Sub
    'giu160113
    Private Sub ControlloCampiObbligatori(ByRef _ElErr As String)
        _ElErr = ""
        Session(SWERRORI_AGGIORNAMENTO) = False
        If (String.IsNullOrEmpty(txtCodFornitore.Text)) Then
            txtCodFornitore.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice fornitore"
        ElseIf Not IsNumeric(txtCodFornitore.Text.Trim) Then
            txtCodFornitore.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice fornitore"
        ElseIf txtCodFornitore.Text.Trim.Length <> txtCodFornitore.MaxLength Then
            txtCodFornitore.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice fornitore"
        ElseIf Left(txtCodFornitore.Text.Trim, 1) <> "9" Then
            txtCodFornitore.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
            _ElErr += " - Codice fornitore"
        Else
            txtCodFornitore.BackColor = SEGNALA_OK
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
        End If
        'giu051118
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
        If txtPECEmail.Text.Trim <> "" Then
            If ConvalidaEmail(txtPECEmail.Text.Trim) = False Then
                txtPECEmail.BackColor = SEGNALA_KO
                Session(SWERRORI_AGGIORNAMENTO) = True
                _ElErr += " - E-mail PEC"
            Else
                txtPECEmail.BackColor = SEGNALA_OK
            End If
        Else
            txtPECEmail.BackColor = SEGNALA_OK
        End If
        '-------------------------------
        'GIU160113 CONTROLLO P.I. E C.F. (importato dai clienti dopo averlo testato)
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
    End Sub
    'giu160113
    Private Function ControlloCampiDati() As Boolean
        ControlloCampiDati = True
        '''If Len(Trim(Replace(lblIBANFor.Text, " ", ""))) <> 27 And (Trim(txtCodABI.Text) <> "" Or Trim(txtCodCAB.Text) <> "") Then
        '''    If Session("AvvisaIBAN") = True = True Then
        '''        Session("AvvisaIBAN") = False
        '''        ControlloCampiDati = False
        '''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '''        ModalPopup.Show("Controllo dati fornitore", "Codice IBAN Fornitore non valido (Lunghezza 27 caratteri). <br>" & _
        '''                        "Aggiornare nuovamente se si vuole prosegure senza questi dati.", WUC_ModalPopup.TYPE_ALERT)
        '''        Exit Function
        '''    End If
        '''End If
        If (String.IsNullOrEmpty(txtCodFiscale.Text.Trim) And String.IsNullOrEmpty(txtPartitaIVA.Text.Trim)) Then
            If Session("AvvisaDoppioCF") = True Or Session("AvvisaDoppioPI") = True Then
                Session("AvvisaDoppioCF") = False
                Session("AvvisaDoppioPI") = False
                ControlloCampiDati = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati fornitore", "Attenzione, Partiva IVA e Codice fiscale mancanti. <br>" & _
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
                    WUC_Attesa1.ShowCFPivaDoppi("Attenzione", "Il Codice fiscale inserito è utilizzato per altre anagrafiche. Fare click su 'Ok Stampa' per vedere l'elenco.", WUC_Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR_Clienti.aspx?labelForm=Fornitori con codice fiscale " & txtCodFiscale.Text)
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
                    WUC_Attesa1.ShowCFPivaDoppi("Attenzione", "La Partita IVA inserita è utilizzata per altre anagrafiche. Fare click su 'Ok Stampa' per vedere l'elenco.", WUC_Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR_Clienti.aspx?labelForm=Fornitori con partita IVA " & txtPartitaIVA.Text)
                    Exit Function
                End If
            End If
        End If
    End Function

    Private Shared Sub ResetCampoStringaValidato(ByRef txt As TextBox)
        txt.Text = String.Empty
        txt.BackColor = Def.SEGNALA_OK
    End Sub

    Private Sub CheckInserimentoCodice(ByRef txtCod As TextBox, ByVal txtDesc As TextBox)
        If (String.IsNullOrEmpty(txtCod.Text)) Then
            txtDesc.Text = String.Empty
            txtCod.BackColor = Def.SEGNALA_OK
        ElseIf txtCod.Text.Trim = "0" Then 'GIU131211
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

    Private Sub Aggiornamento(ByRef myFornitore As FornitoreEntity)

        If (AggiornaFornitore(myFornitore)) Then
            Session(COD_FORNITORE) = txtCodFornitore.Text.Trim
            Session(CSTCODCOGEDM) = txtCodFornitore.Text.Trim 'giu261111
            'GIU251111 SvuotaCampi() NON SERVE A NULLA perche' dopo fa il visualizza
            CampiSetEnabledTo(False)
            'GIU251111 Tabs.ActiveTabIndex = 0  
            'GIU251111 VisualizzaFornitore()
            setPulsantiModalitaConsulta()
            Session(SWOP) = SWOPNESSUNA
        End If
    End Sub

    Private Function AggiornaFornitore(ByVal myFornitore As FornitoreEntity) As Boolean
        Dim ForSys As New Fornitore

        Try
            If (Not ForSys.InsertUpdateFornitore(myFornitore)) Then
                If (Session(SWOP).Equals(SEL_BTN_NUOVO)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile inserire il fornitore", WUC_ModalPopup.TYPE_ALERT)
                ElseIf (Session(SWOP).Equals(SEL_BTN_MODIFICA)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è possibile modificare il fornitore", WUC_ModalPopup.TYPE_ALERT)
                End If
                AggiornaFornitore = False
                Exit Function
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in AggiornaFornitore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            AggiornaFornitore = False
            Exit Function
        End Try
        AggiornaFornitore = True
    End Function

#End Region

    Private Sub VisualizzaFornitore(Optional ByVal SWNoSvuota As Boolean = False)
        If SWNoSvuota = False Then 'giu030714
            GWAltriIndirizzi.SvuotaCampi()
            DestMerceFornitore.SvuotaCampi()
        End If

        Dim listaFornitori As ArrayList = App.GetElencoCompleto(Def.FORNITORI, Session(ESERCIZIO))
        Dim myFornitori As FornitoreEntity
        Dim posizione As Integer = 0
        Try
            If Not String.IsNullOrEmpty(Session(COD_FORNITORE)) Then
                Dim f = From x In listaFornitori Where x.Codice_CoGe.Equals(Session(COD_FORNITORE))
                myFornitori = f(0)
                posizione = listaFornitori.IndexOf(f(0))
                If (posizione < 0) Then
                    SvuotaCampi()
                Else
                    PopolaCampi(myFornitori)
                End If
            Else
                SvuotaCampi()
            End If
            CampiSetEnabledTo(False)
            setPulsantiModalitaConsulta()
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in VisualizzaFornitore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Return
        End Try
    End Sub

    Private Sub LocalizzaFornitore(ByVal condizione As String, Optional ByVal SWNoSvuota As Boolean = False) 'giu030714 SWNoSvuota
        Dim listaFornitori As ArrayList = App.GetLista(Def.FORNITORI, Session(ESERCIZIO))
        Dim posizione As Integer = 0
        If (listaFornitori Is Nothing) Then Exit Sub 'giu010212
        If (listaFornitori.Count > 0) Then
            Select Case condizione
                Case FOR_CORRENTE
                    Dim f = From x In listaFornitori Where x.Codice_CoGe.Equals(Session(COD_FORNITORE))
                    posizione = listaFornitori.IndexOf(f(0))
                Case FIRST_FOR
                    posizione = 0
                Case PREV_FOR
                    Dim f = From x In listaFornitori Where x.Codice_CoGe.Equals(Session(COD_FORNITORE))
                    posizione = listaFornitori.IndexOf(f(0))
                    posizione = posizione - 1
                    posizione = IIf(posizione < 0, 0, posizione)
                Case NEXT_FOR
                    Dim f = From x In listaFornitori Where x.Codice_CoGe.Equals(Session(COD_FORNITORE))
                    posizione = listaFornitori.IndexOf(f(0))
                    posizione = posizione + 1
                    posizione = IIf(posizione = listaFornitori.Count, listaFornitori.Count - 1, posizione)
                Case LAST_FOR
                    posizione = listaFornitori.Count - 1
            End Select
            If (posizione < 0) Then
                Exit Sub
            End If
            Session(COD_FORNITORE) = listaFornitori(posizione).Codice_CoGe
            Session(CSTCODCOGEDM) = listaFornitori(posizione).Codice_CoGe 'giu261111
            VisualizzaFornitore(SWNoSvuota)
        End If
    End Sub
    'GIU010223
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
        btnStampaElencoAna.Enabled = True
        btnStampaElencoCodici.Enabled = True
        btnStampaElencoSint.Enabled = True
        btnStampaRubrica.Enabled = True
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaConsulta()
        DestMerceFornitore.setPulsantiModalitaConsulta()
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
        btnStampaElencoAna.Enabled = False
        btnStampaElencoCodici.Enabled = False
        btnStampaElencoSint.Enabled = False
        btnStampaRubrica.Enabled = False
        '--
        btnAnnulla.Enabled = True
        btnAggiorna.Enabled = True
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaBlocco()
        DestMerceFornitore.setPulsantiModalitaBlocco()
    End Sub

    Private Sub setPulsantiModalitaBlocco()
        btnNuovo.Enabled = True
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnStampaElencoAna.Enabled = False
        btnStampaElencoCodici.Enabled = False
        btnStampaElencoSint.Enabled = False
        btnStampaRubrica.Enabled = False
        btnAnnulla.Enabled = False
        btnAggiorna.Enabled = False
        'giu non va bene cosi 
        GWAltriIndirizzi.setPulsantiModalitaBlocco()
        DestMerceFornitore.setPulsantiModalitaBlocco()
    End Sub

    Private Function CheckNewCodiceFornitore() As Boolean
        If txtCodFornitore.Text.Trim = "" Then Exit Function
        'giu241111 non era implementato di nulla
        Dim listaFornitori As ArrayList = App.GetLista(Def.FORNITORI, Session(ESERCIZIO)) 'GIU121211
        Dim posizione As Integer = 0
        If (listaFornitori.Count > 0) Then
            Dim f = From x In listaFornitori Where x.Codice_CoGe.Equals(txtCodFornitore.Text.Trim)
            posizione = listaFornitori.IndexOf(f(0))
            If (posizione < 0) Then
                Exit Function
            Else
                CheckNewCodiceFornitore = True
                Session(MODALPOPUP_CALLBACK_METHOD) = "GetNewCoge"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice fornitore già presente in tabella: <br>  " & _
                                listaFornitori(posizione).Rag_Soc & "  <br> " & _
                                "Vuole un nuovo codice CoGe ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                Exit Function
            End If
        End If
    End Function
    'GIU040512
    Public Sub GetNewCoge()
        txtCodFornitore.Text = LocalizzaNEWFornitore()
        Session(CSTCODCOGEDM) = txtCodFornitore.Text.Trim 'GIU281111
        Session(COD_FORNITORE) = txtCodFornitore.Text.Trim
        lblLabelNEW.Text = "Nuovo codice " & txtCodFornitore.Text
        lblLabelNEW.Visible = True
    End Sub

    Private Sub ApriElencoFornitori()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoFor.Show(True)
    End Sub

    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            Case F_NAZIONI
                WFPElencoNazioni.Show(True)
            Case F_NAZIONIIBAN
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
        End Select
    End Sub

#End Region

#Region "Metodi public"

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If (Session(F_FOR_RICERCA)) Then
            txtCodFornitore.Text = codice
            txtRagSoc.Text = descrizione
            Session(COD_FORNITORE) = codice
            Session(CSTCODCOGEDM) = codice 'giu261111
            VisualizzaFornitore()
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
            Case F_NAZIONIIBAN
                txtCodNazioneIBAN.Text = codice
                txtCodNazioneIBAN.BackColor = Def.SEGNALA_OK
                lblNazioneIBAN.Text = descrizione
                Call CKIBAN()
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
                ''Case F_AGENTI
                ''    txtCodAgente.Text = codice
                ''    txtCodAgente.BackColor = Def.SEGNALA_OK
                ''    lblAgente.Text = descrizione
                ''Case F_AGENTI_ESE_PREC
                ''    txtCodAgenteEsePrec.Text = codice
                ''    txtCodAgenteEsePrec.BackColor = Def.SEGNALA_OK
                ''    txtAgenteEsePrec.Text = descrizione
            Case F_ZONE
                txtCodZona.Text = codice
                txtCodZona.BackColor = Def.SEGNALA_OK
                lblZona.Text = descrizione
                ''Case F_VETTORI
                ''    txtCodVettore.Text = codice
                ''    txtCodVettore.BackColor = Def.SEGNALA_OK
                ''    lblVettore.Text = descrizione
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
        End Select
    End Sub

    Public Sub ConfermaEliminaFornitore()
        Dim ForSys As New Fornitore
        Dim errorMsg As String = String.Empty
        Try
            If ForSys.CIFornitoreByCodice(Session(COD_FORNITORE)) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella contabilità, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If ForSys.CIFornitoreByCodiceAZI(Session(COD_FORNITORE)) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nella gestione aziendale, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If ForSys.CIFornitoreByCodiceSCAD(Session(COD_FORNITORE)) = False Then 'GIU261111 IDEM PER AZI NEI DOCUMENTIT
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sono presenti delle registrazioni nello scadenzario, <br> impossibile eliminare questo conto", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'OK CANCELLO 
            If (ForSys.delFornitoreByCodice(Session(COD_FORNITORE))) Then
                If (Not App.CaricaFornitori(Session(ESERCIZIO), errorMsg)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Caricamento Dati", _
                        String.Format("Errore nel caricamento Parametri generali azienda, contattare l'amministratore di sistema. La sessione utente verrà chiusa. Errore: {0}", errorMsg), _
                        WUC_ModalPopup.TYPE_INFO)
                    SessionUtility.LogOutUtente(Session, Response)
                    Exit Sub
                End If
                LocalizzaFornitore(FIRST_FOR)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Il Fornitore non può essere eliminato. (delFornitoriByCodice)", WUC_ModalPopup.TYPE_ALERT)
            End If
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ConfermaEliminaFornitore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub

    ' ''Private Sub btnAgente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAgente.Click
    ' ''    WFP_Agenti1.WucElement = Me
    ' ''    WFP_Agenti1.SvuotaCampi()
    ' ''    WFP_Agenti1.SetlblMessaggi("")
    ' ''    Session(F_ANAGRAGENTI_APERTA) = True
    ' ''    Session(F_ANAGRAGENTI_Proven) = "Agente" 'Provenienza apertura
    ' ''    WFP_Agenti1.Show()
    ' ''End Sub

    ' ''Public Sub CallBackWFPAnagrAgenti()
    ' ''    Session(IDAGENTI) = ""
    ' ''    Dim rk As StrAgenti
    ' ''    rk = Session(RKAGENTI)
    ' ''    If IsNothing(rk.IDAgenti) Then
    ' ''        Exit Sub
    ' ''    End If
    ' ''    If IsNothing(rk.Descrizione) Then
    ' ''        Exit Sub
    ' ''    End If
    ' ''    Session(IDAGENTI) = rk.IDAgenti
    ' ''    If Not IsNothing(Session(F_ANAGRAGENTI_Proven)) Then
    ' ''        If Session(F_ANAGRAGENTI_Proven).ToString.ToUpper = "Agente".ToString.ToUpper Then
    ' ''            txtCodAgente.Text = rk.IDAgenti
    ' ''            lblAgente.Text = rk.Descrizione
    ' ''        ElseIf Session(F_ANAGRAGENTI_Proven).ToString.ToUpper = "AgenteEsePrec".ToString.ToUpper Then
    ' ''            txtCodAgenteEsePrec.Text = rk.IDAgenti
    ' ''            'lblAgenteEsePrec.Text = rk.Descrizione
    ' ''        End If
    ' ''    Else
    ' ''        ''Non assegno a nessuna label
    ' ''    End If
    ' ''    Session(F_ANAGRAGENTI_Proven) = ""
    ' ''End Sub
    ' ''Public Sub CancBackWFPAnagrAgenti()

    ' ''End Sub

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

    ''Private Sub btnVettori_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVettori.Click
    ''    WFP_Vettori1.WucElement = Me
    ''    WFP_Vettori1.SvuotaCampi()
    ''    WFP_Vettori1.SetlblMessaggi("")
    ''    Session(F_ANAGRVETTORI_APERTA) = True
    ''    WFP_Vettori1.Show()
    ''End Sub
    ''Public Sub CallBackWFPAnagrVettori()
    ''    Session(IDVETTORI) = ""
    ''    Dim rk As StrVettori
    ''    rk = Session(RKVETTORI)
    ''    If IsNothing(rk.IDVettori) Then
    ''        Exit Sub
    ''    End If
    ''    If IsNothing(rk.Descrizione) Then
    ''        Exit Sub
    ''    End If
    ''    Session(IDVETTORI) = rk.IDVettori
    ''    txtCodVettore.Text = rk.IDVettori
    ''    lblVettore.Text = rk.Descrizione
    ''End Sub
    ''Public Sub CancBackWFPAnagrVettori()

    ''End Sub

    Private Sub btnCategorie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCategorie.Click
        WFP_Categorie1.WucElement = Me
        WFP_Categorie1.SvuotaCampi()
        WFP_Categorie1.SetlblMessaggi("")
        Session(F_ANAGRCATEGORIE_APERTA) = True
        WFP_Categorie1.Show()
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


    'giu281111
    Public Sub setPulsantiModalitaBloccoByWUC(ByVal SWWUC As String)
        btnFirst.Enabled = False
        btnNext.Enabled = False
        btnPrev.Enabled = False
        btnLast.Enabled = False
        btnNuovo.Enabled = False
        btnModifica.Enabled = False
        btnElimina.Enabled = False
        btnStampaElencoAna.Enabled = False
        btnStampaElencoCodici.Enabled = False
        btnStampaElencoSint.Enabled = False
        btnStampaRubrica.Enabled = False
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
        btnStampaElencoAna.Enabled = True
        btnStampaElencoCodici.Enabled = True
        btnStampaElencoSint.Enabled = True
        btnStampaRubrica.Enabled = True
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

#Region " Metodi di Stampa"

    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampa.Click
        Session(CSTFinestraChiamante) = "Fornitori"
        Session(CSTRitornoDaStampa) = True
        Response.Redirect("..\WebFormTables\WF_OrdineStampaCli.aspx?labelForm=Stampa anagrafica fornitori")
    End Sub
#End Region
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

    'giu160113
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
                SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Fornitori WHERE Codice_Fiscale = '" & Stringa & "' AND Codice_CoGe <> '" & Session(COD_FORNITORE) & "'"
            ElseIf Tipo = "PIVA" Then
                SQLCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Codice_Fiscale, Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Fornitori WHERE Partita_IVA = '" & Stringa & "' AND Codice_CoGe <> '" & Session(COD_FORNITORE) & "'"
            End If

            SQLCmd.CommandText = SQLCmd.CommandText & " UNION SELECT '" & txtCodFornitore.Text.Trim & "' AS Codice_CoGe, '" & txtRagSoc.Text.Trim & "' AS Rag_Soc, '" & txtCodFiscale.Text.Trim & "' AS Codice_Fiscale, '" & txtPartitaIVA.Text.Trim & "' AS Partita_IVA, '" & Session(CSTAZIENDARPT) & "' AS Ditta FROM Fornitori"

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
                ModalPopup.Show("Attenzione", "Errore durante il controllo sul codice fiscale duplicato. È possibile che sia già presente un cliente con questo codice fiscale.", WUC_ModalPopup.TYPE_ERROR)
            ElseIf Tipo = "PIVA" Then
                ModalPopup.Show("Attenzione", "Errore durante il controllo sulla partita IVA duplicata. È possibile che sia già presente un cliente con questa partita IVA.", WUC_ModalPopup.TYPE_ERROR)
            End If
            ControlloDoppio = False
        End Try
    End Function
    Function PI_controllo(ByVal Partita_Iva As String) As Integer
        If Partita_Iva.Trim = "" Then
            Return True
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
End Class