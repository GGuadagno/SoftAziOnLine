Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports It.SoftAzi.Model.Facade

Partial Public Class WFP_ElencoEmail
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Private Const F_CATEGORIE As String = "ElencoCategorie"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_CatCli.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftCoge)
        WUCElencoEmail.WucElement = Me
        WFPElencoCategorie.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        '
        If (Not IsPostBack) Then
            SEGNALA_OKBTN = btnVisualizza.BackColor
            txtDallaData.AutoPostBack = False
            txtAllaData.AutoPostBack = False
            Dim MyData As New Date(2019, 1, 1) 'giu290519 Date(Now.Year, Now.Month, 1)
            txtDallaData.Text = Format(MyData, FormatoData)
            txtAllaData.Text = Format(Now, FormatoData)

            txtDallaData.AutoPostBack = True
            txtAllaData.AutoPostBack = True
            '--
            ddlStato.Items.Add("")
            ddlStato.Items(0).Value = "-1"
            ddlStato.Items.Add("Da inviare")
            ddlStato.Items(1).Value = "0"
            ddlStato.Items.Add("Inviata")
            ddlStato.Items(2).Value = "1"
            ddlStato.Items.Add("Sollecito inviato")
            ddlStato.Items(3).Value = "2"
            ddlStato.Items.Add("Parz.Conclusa")
            ddlStato.Items(4).Value = "3"
            ddlStato.Items.Add("Annullata")
            ddlStato.Items(5).Value = "9"
            ddlStato.Items.Add("Conclusa")
            ddlStato.Items(6).Value = "99"
            '-
            chkInvioSollecito.Enabled = False
            chkInvioSollecito.Checked = False
            chkInvioSollecito.BackColor = Drawing.Color.Silver
            '-
            SetControl(False)
        End If
        If WUCElencoEmail.CheckGrid() = True Then
            SetControl(True)
        End If
        'Clienti
        If Session(F_VISUALIZZADETT_APERTA) = False Then
            If Session(F_EMAILINVIATE_APERTA) = False And Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_EMAILINVIATE_APERTA Then
                If Session(F_CLI_RICERCA) = True Then
                    WFP_ElencoCliForn1.Show()
                End If
            End If
        End If
        'GIU030918 AL MOEMENTO NON E' USATO MA NEL CASO ....
        ' ''If Session(F_VISUALIZZADETT_APERTA) = False Then
        ' ''    If Session(F_EMAILINVIATE_APERTA) = False And Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_EMAILINVIATE_APERTA  And _
        ' ''        Session(F_CLI_RICERCA) = False Then
        ' ''        Select Case Session(F_ELENCO_APERTA)
        ' ''            Case F_CATEGORIE
        ' ''                WFPElencoCategorie.Show()
        ' ''        End Select
        ' ''    End If
        ' ''End If
    End Sub

#Region "Gestione controlli"
    Private Sub chkTutteCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteCatCli.CheckedChanged
        ddlCatCli.SelectedIndex = -1
        If chkTutteCatCli.Checked Then
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
        Else
            ddlCatCli.Enabled = True
            chkRaggrCatCli.Enabled = True
        End If
        SvuotaGriglia()
    End Sub
    Private Sub chkSelScBa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelScBa.CheckedChanged
        SvuotaGriglia()
    End Sub
    Private Sub chkSelScEl_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelScEl.CheckedChanged
        SvuotaGriglia()
    End Sub
    Private Sub chkSelScGa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelScGa.CheckedChanged
        SvuotaGriglia()
    End Sub
    Private Sub ddlCatCli_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatCli.SelectedIndexChanged
        SvuotaGriglia()
    End Sub
    Private Sub txtAllaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAllaData.TextChanged
        SvuotaGriglia()
        txtDalNumeno.Focus()
    End Sub
    Private Sub txtDallaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDallaData.TextChanged
        SvuotaGriglia()
        txtAllaData.Focus()
    End Sub
    Private Sub rbtnDaInviare_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDaInviare.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnSollecito_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnSollecito.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnInviata_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnInviata.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnAnnullata_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnAnnullata.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnConclusa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnConclusa.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnInvioErr_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnInvioErr.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub

    Private Sub rbtnTutte_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnTutte.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub

    Private Sub rbtnParzConclusa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnParzConclusa.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    'Clienti
    Private Sub pulisciCampi()
        txtCod1.Text = ""
        txtDesc1.Text = ""
    End Sub

    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click
        pulisciCampi()
        ProgrammaticModalPopup.Hide()
        Session(F_EMAILINVIATE_APERTA) = False
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        ProgrammaticModalPopup.Show()
        Session(F_EMAILINVIATE_APERTA) = True
        If Session(F_CLI_RICERCA) = True Then
            txtCod1.Text = codice
            txtDesc1.Text = descrizione
        End If
        Session(F_CLI_RICERCA) = False
    End Sub
    Private Sub ApriElencoClienti1()
        Session(F_CLI_RICERCA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.CLIENTI, Session(ESERCIZIO))
        SvuotaGriglia()
    End Sub
    Private Sub chkTuttiClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampi()
        SvuotaGriglia()
        If chkTuttiClienti.Checked Then
            txtCod1.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            txtDesc1.Enabled = False
        Else
            txtCod1.Enabled = True
            btnCercaAnagrafica1.Enabled = True
            txtDesc1.Enabled = False
            txtCod1.Focus()
        End If
    End Sub
    Private Sub txtDalNumeno_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDalNumeno.TextChanged
        SvuotaGriglia()
        txtAlNumero.Focus()
    End Sub

    Private Sub txtAlNumero_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAlNumero.TextChanged
        SvuotaGriglia()
    End Sub
#End Region

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlStato.SelectedIndex = 0
        lblMessUtente.BackColor = SEGNALA_OK
        ProgrammaticModalPopup.Hide()
        Session(F_EMAILINVIATE_APERTA) = False
        Session(F_VISUALIZZADETT_APERTA) = True
        _WucElement.InitializeForm()    'inizializzo form elenco articoli
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlStato.SelectedIndex = 0
        If WUCElencoEmail.CheckGrid = False Then
            Exit Sub
        End If
        WUCElencoEmail.SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlStato.SelectedIndex = 0
        If WUCElencoEmail.CheckGrid = False Then
            Exit Sub
        End If
        lblMessUtente.BackColor = SEGNALA_OK
        WUCElencoEmail.DeselezionaTutti()
    End Sub

    Protected Sub btnModificaMail_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ddlStato.SelectedIndex = 0
        If WUCElencoEmail.CheckGrid = False Then
            Exit Sub
        End If
        btnVisualizza.BackColor = SEGNALA_KO
        lblMessUtente.BackColor = SEGNALA_OK
        WUCElencoEmail.ShowModificaMail()
    End Sub

    Protected Sub btnVisualizzaDett_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlStato.SelectedIndex = 0
        Call ChiamaCallBackStampaElencoEM(False)
    End Sub

    Protected Sub btnStampaElenco_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlStato.SelectedIndex = 0
        Session(CSTESPORTAPDF) = True
        Call ChiamaCallBackStampaElencoEM(True)
    End Sub

    Public Sub ChiamaCallBackStampaElencoEM(ByVal EffettuaStampa As Boolean)
        ddlStato.SelectedIndex = 0
        Dim CaricaGrigliaDettaglio As Boolean = True

        If WUCElencoEmail.CheckGrid = False Then
            Exit Sub
        End If
        lblMessUtente.BackColor = SEGNALA_OK
        Dim CodCogeSelezionati As String = "" 'giu030918 OK NULLA WUCElencoEmail.getCodCogeSelezionati()
        ' ''If CodCogeSelezionati.Trim = "" Then
        ' ''    lblMessUtente.BackColor = SEGNALA_KO
        ' ''    lblMessUtente.Text = "Nessun dato selezionato"
        ' ''    Exit Sub
        ' ''Else
        ' ''    lblMessUtente.BackColor = SEGNALA_OK
        ' ''End If

        Dim TipoOrdine As String = "Cod_CoGe"
        Dim TipoOrdineRPT As Integer = TipoStampaAICA.ElencoAICACliArtNSerie
        Dim TipoOrdineST As String = "Cliente"
        Dim SelScGa As Boolean = chkSelScGa.Checked
        Dim SelScEl As Boolean = chkSelScEl.Checked
        Dim SelScBa As Boolean = chkSelScBa.Checked
        Dim SelTutteCatCli As Boolean = chkTutteCatCli.Checked
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli.Checked
        '
        Dim SelStatoEmail As Integer = 9999
        If rbtnInvioErr.Checked Then
            SelStatoEmail = -1
        ElseIf rbtnDaInviare.Checked Then
            SelStatoEmail = 0
        ElseIf rbtnInviata.Checked Then
            SelStatoEmail = 1
        ElseIf rbtnSollecito.Checked Then
            SelStatoEmail = 2
        ElseIf rbtnParzConclusa.Checked Then
            SelStatoEmail = 3
        ElseIf rbtnAnnullata.Checked Then
            SelStatoEmail = 9
        ElseIf rbtnConclusa.Checked Then
            SelStatoEmail = 99
        ElseIf rbtnTutte.Checked Then
            SelStatoEmail = 999 'TUTTE
        Else
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Attenzione, selezionare lo stato E-mail."
            Exit Sub
        End If
        '---------
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        'gestione categorie
        Dim DescCatCli As String = ""
        Dim CodCategoria As Integer = ddlCatCli.SelectedValue
        'Sel.Multipla
        Dim CodcategSel As String = ""
        Dim NSel As Integer = 0
        '--
        Dim SWTratt As Integer = 0
        SWTratt = InStr(ddlCatCli.SelectedItem.Text.Trim, " - ")
        If chkTutteCatCli.Checked = False Then
            If chkSelCategorie.Checked = True Then
                If btnSelCategorie.BackColor = SEGNALA_OKLBL Then
                    If LeggiCategorie(CodcategSel, NSel) = True Then
                        lblMessUtente.BackColor = SEGNALA_OK
                        lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
                        DescCatCli = "Selezione multipla"
                    Else
                        lblMessUtente.BackColor = SEGNALA_KO
                        lblMessUtente.Text = "Attenzione, selezionare almeno 2 categorie."
                        Exit Sub
                    End If
                Else
                    lblMessUtente.BackColor = SEGNALA_KO
                    lblMessUtente.Text = "Attenzione, selezionare almeno 2 categorie."
                    Exit Sub
                End If
            ElseIf chkRaggrCatCli.Checked = True Then
                If SWTratt = 0 Then
                    DescCatCli = ddlCatCli.SelectedItem.Text.Trim
                Else
                    DescCatCli = Left(ddlCatCli.SelectedItem.Text.Trim, SWTratt - 1)
                End If
            Else
                DescCatCli = ddlCatCli.SelectedItem.Text.Trim
            End If
        ElseIf chkSelCategorie.Checked = True Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Attenzione, selezionare tutte le caterorie oppure Selezione multipla ma non entrambe."
            Exit Sub
        Else
            DescCatCli = "Tutte"
        End If
        '-----
        Dim DataEmailDA As DateTime
        Dim DataEmailA As DateTime
        DataEmailDA = CDate(txtDallaData.Text.ToString).Date
        DataEmailA = CDate(txtAllaData.Text.ToString).Date

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataEmailA.Year, DataEmailA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataEmailA.Year, DataEmailA.Month, DaysInMonth)
        DataEmailA = LastDayInMonthDate
        '-
        If DataEmailA < DataEmailDA Then
            txtDallaData.BackColor = SEGNALA_KO
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        Else
            txtDallaData.BackColor = SEGNALA_OK
            txtAllaData.BackColor = SEGNALA_OK
        End If
        Dim DalNumero As Integer = -1
        Dim AlNumero As Integer = -1
        If Not IsNumeric(txtDalNumeno.Text.Trim) Then
            txtDalNumeno.Text = "0"
        End If
        DalNumero = txtDalNumeno.Text
        If Not IsNumeric(txtAlNumero.Text.Trim) Then
            txtAlNumero.Text = "99999"
        End If
        AlNumero = txtAlNumero.Text
        If AlNumero < DalNumero Then
            txtDalNumeno.BackColor = SEGNALA_KO
            txtAlNumero.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Dal numero deve essere maggiore o uguale al numero."
            Exit Sub
        Else
            txtDalNumeno.BackColor = SEGNALA_OK
            txtAlNumero.BackColor = SEGNALA_OK
        End If
        '-
        'giu030918
        Dim SelDalNAlN As String = WUCElencoEmail.getSelDalNAlN()
        If SelDalNAlN.Trim = "" Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Nessun dato selezionato"
            Exit Sub
        Else
            lblMessUtente.BackColor = SEGNALA_OK
        End If
        '---------
        lblMessUtente.BackColor = SEGNALA_OK
        lblMessUtente.Text = "Visualizza dettaglio scadenze"
        'carico dettaglio nella griglia dell'elenco
        Session(F_VISUALIZZADETT_APERTA) = True
        Session(F_EMAILINVIATE_APERTA) = False
        ProgrammaticModalPopup.Hide()
        'GIU250618
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        If Session(CSTDsEmailInviate) Is Nothing Then
            DsPrinWebDoc.Clear()
        Else
            DsPrinWebDoc = Session(CSTDsEmailInviate)
        End If
        '-
       
        _WucElement.CallBackStampaElencoEM(TipoOrdineST, TipoOrdine, TipoOrdineRPT, DataEmailDA, DataEmailA, _
                                           SelScGa, SelScEl, SelScBa, txtCod1.Text.Trim, SelStatoEmail, DsPrinWebDoc, _
                                           SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                                           chkSelCategorie.Checked, CodcategSel, _
                                           DalNumero, AlNumero, SelDalNAlN, _
                                           EffettuaStampa, CaricaGrigliaDettaglio, CodCogeSelezionati)
        lblMessUtente.BackColor = SEGNALA_OK
        lblMessUtente.Text = "Selezione/Deselezione E-mail clienti"
    End Sub

    Protected Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        btnVisualizza.BackColor = SEGNALA_OKBTN
        lblMessUtente.BackColor = SEGNALA_OK
        ddlStato.SelectedIndex = 0
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        Dim SelScGa As Boolean = chkSelScGa.Checked
        Dim SelScEl As Boolean = chkSelScEl.Checked
        Dim SelScBa As Boolean = chkSelScBa.Checked
        Dim SelTutteCatCli As Boolean = chkTutteCatCli.Checked
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli.Checked
        '
        Dim SelStatoEmail As Integer = 9999
        If rbtnInvioErr.Checked Then
            SelStatoEmail = -1
        ElseIf rbtnDaInviare.Checked Then
            SelStatoEmail = 0
        ElseIf rbtnInviata.Checked Then
            SelStatoEmail = 1
        ElseIf rbtnSollecito.Checked Then
            SelStatoEmail = 2
        ElseIf rbtnParzConclusa.Checked Then
            SelStatoEmail = 3
        ElseIf rbtnAnnullata.Checked Then
            SelStatoEmail = 9
        ElseIf rbtnConclusa.Checked Then
            SelStatoEmail = 99
        ElseIf rbtnTutte.Checked Then
            SelStatoEmail = 999 'TUTTE
        Else
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Attenzione, selezionare lo stato E-mail."
            Exit Sub
        End If
        '---------
        'GIU030918
        If txtCod1.Text.Trim = "" And chkTuttiClienti.Checked = False Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Attenzione, avete selezionato (Tutti i clienti): si prega di selezionare il cliente singolo oppure tutti i clienti."
            Exit Sub
        ElseIf chkTuttiClienti.Checked = True Then
            txtCod1.AutoPostBack = False
            txtCod1.Text = ""
            txtCod1.AutoPostBack = True
        End If
        '-------------------------------------------------------------------
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        'gestione categorie
        Dim DescCatCli As String = ""
        Dim CodCategoria As Integer = ddlCatCli.SelectedValue
        'Sel.Multipla
        Dim CodcategSel As String = ""
        Dim NSel As Integer = 0
        '--
        Dim SWTratt As Integer = 0
        SWTratt = InStr(ddlCatCli.SelectedItem.Text.Trim, " - ")
        If chkTutteCatCli.Checked = False Then
            If chkSelCategorie.Checked = True Then
                If btnSelCategorie.BackColor = SEGNALA_OKLBL Then
                    If LeggiCategorie(CodcategSel, NSel) = True Then
                        lblMessUtente.BackColor = SEGNALA_OK
                        lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
                        DescCatCli = "Selezione multipla"
                    Else
                        lblMessUtente.BackColor = SEGNALA_KO
                        lblMessUtente.Text = "Attenzione, selezionare almeno 2 categorie."
                        Exit Sub
                    End If
                Else
                    lblMessUtente.BackColor = SEGNALA_KO
                    lblMessUtente.Text = "Attenzione, selezionare almeno 2 categorie."
                    Exit Sub
                End If
            ElseIf chkRaggrCatCli.Checked = True Then
                If SWTratt = 0 Then
                    DescCatCli = ddlCatCli.SelectedItem.Text.Trim
                Else
                    DescCatCli = Left(ddlCatCli.SelectedItem.Text.Trim, SWTratt - 1)
                End If
            Else
                DescCatCli = ddlCatCli.SelectedItem.Text.Trim
            End If
        ElseIf chkSelCategorie.Checked = True Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Attenzione, selezionare tutte le caterorie oppure Selezione multipla ma non entrambe."
            Exit Sub
        Else
            DescCatCli = "Tutte"
        End If
        '-----
        Dim DataEmailDA As DateTime
        Dim DataEmailA As DateTime
        DataEmailDA = CDate(txtDallaData.Text.ToString).Date
        DataEmailA = CDate(txtAllaData.Text.ToString).Date

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataEmailA.Year, DataEmailA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataEmailA.Year, DataEmailA.Month, DaysInMonth)
        DataEmailA = LastDayInMonthDate
        '-

        If DataEmailA < DataEmailDA Then
            txtDallaData.BackColor = SEGNALA_KO
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        Else
            txtDallaData.BackColor = SEGNALA_OK
            txtAllaData.BackColor = SEGNALA_OK
        End If
        Dim DalNumero As Integer = -1
        Dim AlNumero As Integer = -1
        If Not IsNumeric(txtDalNumeno.Text.Trim) Then
            txtDalNumeno.Text = "0"
        End If
        DalNumero = txtDalNumeno.Text
        If Not IsNumeric(txtAlNumero.Text.Trim) Then
            txtAlNumero.Text = "99999"
        End If
        AlNumero = txtAlNumero.Text
        If AlNumero < DalNumero Then
            txtDalNumeno.BackColor = SEGNALA_KO
            txtAlNumero.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Dal numero deve essere maggiore o uguale al numero."
            Exit Sub
        Else
            txtDalNumeno.BackColor = SEGNALA_OK
            txtAlNumero.BackColor = SEGNALA_OK
        End If
        '-
        lblMessUtente.Text = "Selezione/Deselezione E-mail clienti"
        'OK FILL GRIGLIA PASSANDO PARAMETRI
        Dim CountGrid As Integer = 0
        CountGrid = WUCElencoEmail.PopolaGridT(DataEmailDA, DataEmailA, SelScGa, SelScEl, SelScBa, txtCod1.Text.Trim, SelStatoEmail, _
                                                SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                                                chkSelCategorie.Checked, CodcategSel, DalNumero, AlNumero)

    End Sub

    Public Sub SvuotaGriglia()
        If btnVisualizza.BackColor = SEGNALA_KO Then
            Call SetControl(False)
            Exit Sub
        End If
        btnVisualizza.BackColor = SEGNALA_KO
        ddlStato.SelectedIndex = 0
        WUCElencoEmail.SvuotaGriglia()
        Call SetControl(False)
        lblMessUtente.Text = "Eseguire il tasto visualizza"
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.BackColor = SEGNALA_OK
            lblMessUtente.Text = "Selezione/Deselezione E-mail clienti"
            btnVisualizza.BackColor = SEGNALA_KO
            WUCElencoEmail.DatiSessioneGriglia()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetControl(ByVal Stato As Boolean)
        btnDeselTutti.Enabled = Stato
        btnSelTutti.Enabled = Stato
        btnModificaMail.Enabled = Stato
        btnStampaElenco.Enabled = Stato
        btnVisualizzaDett.Enabled = Stato
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

    Protected Sub btnSelCategorie_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_EMAILINVIATE_APERTA) = False
        ApriElenco(F_CATEGORIE)
    End Sub
    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            Case F_CATEGORIE
                WFPElencoCategorie.Show(True)
        End Select
    End Sub
    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        ProgrammaticModalPopup.Show()
        Session(F_EMAILINVIATE_APERTA) = True
        If chkSelCategorie.Checked = True Then
            Dim NSel As Integer = 0
            If LeggiCategorie("", NSel) = True Then
                btnSelCategorie.BackColor = SEGNALA_OKLBL
                lblMessUtente.BackColor = SEGNALA_OK
                lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
            Else
                btnSelCategorie.BackColor = SEGNALA_KO
                chkSelCategorie.Checked = False
                Exit Sub
            End If
            ddlCatCli.SelectedIndex = -1
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkTutteCatCli.Checked = False
        End If
    End Sub

    Private Sub chkSelCategorie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelCategorie.CheckedChanged
        WUCElencoEmail.SvuotaGriglia()
        Call SetControl(False)
        If chkSelCategorie.Checked = True Then
            Dim NSel As Integer = 0
            If LeggiCategorie("", NSel) = True Then
                btnSelCategorie.BackColor = SEGNALA_OKLBL
                lblMessUtente.BackColor = SEGNALA_OK
                lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
            Else
                btnSelCategorie.BackColor = SEGNALA_KO
                chkSelCategorie.Checked = False
                Exit Sub
            End If
            ddlCatCli.SelectedIndex = -1
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkTutteCatCli.Checked = False
        End If
    End Sub
    Private Function LeggiCategorie(ByRef CodCategSel As String, ByRef NSel As Integer) As Boolean
        LeggiCategorie = False
        CodCategSel = ""
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            strSQL = "SELECT Codice FROM Categorie WHERE ISNULL(SelSc,0)<>0 AND ISNULL(InvioMailSc,0)<>0"
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 1) Then
                    NSel = ds.Tables(0).Rows.Count
                    'ok
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        CodCategSel &= ds.Tables(0).Rows(i).Item("Codice").ToString & ";"
                    Next
                    CodCategSel = CodCategSel.Substring(0, CodCategSel.Length - 1) 'rimuovo ultimo ;
                Else
                    lblMessUtente.BackColor = SEGNALA_KO
                    lblMessUtente.Text = "Selezionare almeno 2 categorie."
                    Exit Function
                End If
            Else
                lblMessUtente.BackColor = SEGNALA_KO
                lblMessUtente.Text = "Selezionare almeno 2 categorie."
                Exit Function
            End If
        Catch Ex As Exception
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Selezionare almeno 2 categorie."
            Exit Function
        End Try
        LeggiCategorie = True
    End Function


    Private Sub btnCambiaStato_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCambiaStato.Click
        If ddlStato.SelectedIndex = 0 Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Selezionare il nuovo Stato E-mail da assegnare"
            Exit Sub
        End If
        If WUCElencoEmail.CheckGrid = False Then
            ddlStato.SelectedIndex = 0
            Exit Sub
        End If
        lblMessUtente.BackColor = SEGNALA_OK
        Dim Selezionati As String = WUCElencoEmail.getSelDalNAlN()
        If Selezionati.Trim = "" Then
            ddlStato.SelectedIndex = 0
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Nessun dato selezionato"
            Exit Sub
        Else
            lblMessUtente.BackColor = SEGNALA_OK
        End If
        Dim strErrori As String = ""
        WUCElencoEmail.CambiaStatoEmail(ddlStato.SelectedValue, chkInvioSollecito.Checked, strErrori)
        If strErrori.Trim <> "" Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = strErrori.Trim
            Exit Sub
        End If
        ddlStato.SelectedIndex = 0
        Session("GetScadProdCons") = ""
    End Sub

    Private Sub ddlStato_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlStato.SelectedIndexChanged
        If ddlStato.SelectedValue = "0" Then
            chkInvioSollecito.Enabled = True
            chkInvioSollecito.BackColor = SEGNALA_OK
        Else
            chkInvioSollecito.Enabled = False
            chkInvioSollecito.Checked = False
            chkInvioSollecito.BackColor = Drawing.Color.Silver
        End If
    End Sub

    
End Class