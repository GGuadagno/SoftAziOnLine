Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports It.SoftAzi.Model.Facade
Imports SoftAziOnLine.App

Partial Public Class WFP_ArticoliInstEmail
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
        WUCArticoliInstEmail.WucElement = Me
        WFPElencoCategorie.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        '
        Dim strErrore As String = ""
        If (Not IsPostBack) Then
            Try
                SEGNALA_OKBTN = btnVisualizza.BackColor
                Dim myDallaData As DateTime = Now.Date
                Dim myAllaData As DateTime = Now.Date
                If CaricaParametri(Session(ESERCIZIO), strErrore) Then
                    myDallaData = DateAdd(DateInterval.Month, Val(GetParamGestAzi(Session(ESERCIZIO)).SelAIDaData) * -1, Now.Date)
                    myAllaData = DateAdd(DateInterval.Month, Val(GetParamGestAzi(Session(ESERCIZIO)).SelAIAData), Now.Date)
                Else
                    Chiudi("Errore: Caricamento parametri generali. " & strErrore)
                    Exit Sub
                End If
                txtDallaData.AutoPostBack = False
                txtAllaData.AutoPostBack = False

                Dim myAnno As Integer = myDallaData.Year
                Dim MyMese As Integer = myDallaData.Month
                Dim MyDalPeriodo As New Date(myAnno, MyMese, 1)
                txtDallaData.Text = MyDalPeriodo.ToString("MM/yyyy")  'seleziono il mese attuale

                'se mese attuale è dicembre imposto gennaio del prossimo anno
                Dim myNextAnno As Integer = myAllaData.Year
                Dim MyNextMese As Integer = myAllaData.Month 'MyMese + 1
                ' ''If MyMese >= 12 Then
                ' ''    MyNextMese = 1
                ' ''    myNextAnno = Now.Year + 1
                ' ''End If
                Dim MyAlPeriodo As New Date(myNextAnno, MyNextMese, 1)
                txtAllaData.Text = MyAlPeriodo.ToString("MM/yyyy")

                txtDallaData.AutoPostBack = True
                txtAllaData.AutoPostBack = True
                '
                If Not IsDBNull(GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli) Then 'row.IsSelAICatCliNull Then
                    If GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli = -1 Then 'TUTTE LE CATEGORIE
                        ddlCatCli.SelectedIndex = -1
                        chkRaggrCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = False
                        chkTutteCatCli.AutoPostBack = False
                        chkSelCategorie.Checked = False
                        chkTutteCatCli.Checked = True
                        chkSelCategorie.AutoPostBack = True
                        chkTutteCatCli.AutoPostBack = True
                    ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli = 0 Then 'SELEZIONE MULTIPLA CATEGORIE
                        ddlCatCli.SelectedIndex = -1
                        chkRaggrCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = False
                        chkTutteCatCli.AutoPostBack = False
                        chkSelCategorie.Checked = True
                        chkTutteCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = True
                        chkTutteCatCli.AutoPostBack = True
                        Dim NSel As Integer = 0
                        If LeggiCategorie("", NSel) = True Then
                            btnSelCategorie.BackColor = SEGNALA_OKLBL
                            lblMessUtente.BackColor = SEGNALA_OK
                            lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
                        Else
                            btnSelCategorie.BackColor = SEGNALA_KO
                            chkSelCategorie.Checked = False
                        End If
                    ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli > 0 Then 'SELEZIONE SINGOLA CATEGORIA
                        PosizionaItemDDL(GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli, ddlCatCli)
                        chkRaggrCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = False
                        chkTutteCatCli.AutoPostBack = False
                        chkSelCategorie.Checked = False
                        chkTutteCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = True
                        chkTutteCatCli.AutoPostBack = True
                    ElseIf GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli < 0 Then 'SELEZIONE SINGOLA CATEGORIA RAGGRUPPATA
                        PosizionaItemDDL(GetParamGestAzi(Session(ESERCIZIO)).SelAICatCli * -1, ddlCatCli)
                        chkRaggrCatCli.Checked = True
                        chkSelCategorie.AutoPostBack = False
                        chkTutteCatCli.AutoPostBack = False
                        chkSelCategorie.Checked = False
                        chkTutteCatCli.Checked = False
                        chkSelCategorie.AutoPostBack = True
                        chkTutteCatCli.AutoPostBack = True
                    End If
                Else
                    ddlCatCli.SelectedIndex = -1
                    chkRaggrCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = False
                    chkTutteCatCli.AutoPostBack = False
                    chkSelCategorie.Checked = False
                    chkTutteCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = True
                    chkTutteCatCli.AutoPostBack = True
                End If
                chkSelScGa.Checked = GetParamGestAzi(Session(ESERCIZIO)).SelAIScGa
                chkSelScEl.Checked = GetParamGestAzi(Session(ESERCIZIO)).SelAIScEl
                chkSelScBa.Checked = GetParamGestAzi(Session(ESERCIZIO)).SelAIScBa
                '--
                SetControl(False)
                Session(CSTDsArticoliInstEmail) = Nothing
                Session("aDataViewPrevT2") = Nothing
            Catch ex As Exception
                Chiudi("Errore: Caricamento parametri generali. " & strErrore)
                Exit Sub
            End Try
            
        End If
        If WUCArticoliInstEmail.CheckGrid() = True Then
            SetControl(True)
        End If
        'Clienti
        If Session(F_VISUALIZZADETT_APERTA) = False Then
            If Session(F_ARTINSTEMAIL_APERTA) = False And Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_ARTINSTEMAIL_APERTA Then
                If Session(F_CLI_RICERCA) = True Then
                    WFP_ElencoCliForn1.Show()
                End If
            End If
        End If
        If Session(F_VISUALIZZADETT_APERTA) = False Then
            If Session(F_ARTINSTEMAIL_APERTA) = False And Session(F_ARTINSTEMAIL_EMAILINVIATE) = F_ARTINSTEMAIL_APERTA And _
               Session(F_CLI_RICERCA) = False Then
                Select Case Session(F_ELENCO_APERTA)
                    Case F_CATEGORIE
                        WFPElencoCategorie.Show()
                End Select
            End If
        End If
    End Sub
    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            End Try
            Exit Sub
        End If
    End Sub

#Region "Gestione controlli"
    Private Sub chkTutteCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteCatCli.CheckedChanged
        ddlCatCli.SelectedIndex = -1
        If chkTutteCatCli.Checked Then
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkSelCategorie.AutoPostBack = False
            chkSelCategorie.Checked = False
            chkSelCategorie.AutoPostBack = True
        Else
            If chkSelCategorie.Checked = False Then
                ddlCatCli.Enabled = True
                chkRaggrCatCli.Enabled = True
            End If
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
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub

    Private Sub txtAllaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAllaData.TextChanged
        SvuotaGriglia()
        btnVisualizza.Focus()
    End Sub

    Private Sub txtDallaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDallaData.TextChanged
        SvuotaGriglia()
        txtAllaData.Focus()
    End Sub

    Private Sub rbtnCliSenzaMail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCliSenzaMail.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnCliTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCliTutti.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub

    Private Sub rbtnCliConEmail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCliConEmail.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub
    Private Sub rbtnCliConEmailErr_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCliConEmailErr.CheckedChanged
        'SvuotaGriglia()
        Call btnVisualizza_Click(btnVisualizza, Nothing)
    End Sub

    Private Sub rbtnNoInvioEmail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnNoInvioEmail.CheckedChanged
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
        Session(F_ARTINSTEMAIL_APERTA) = False
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        ProgrammaticModalPopup.Show()
        Session(F_ARTINSTEMAIL_APERTA) = True
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
#End Region

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMessUtente.BackColor = SEGNALA_OK
        ProgrammaticModalPopup.Hide()
        Session(F_ARTINSTEMAIL_APERTA) = False
        Session(F_VISUALIZZADETT_APERTA) = True
        _WucElement.InitializeForm()    'inizializzo form elenco articoli
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If WUCArticoliInstEmail.CheckGrid = False Then
            Exit Sub
        End If
        If rbtnCliConEmail.Checked = False Then
            lblMessUtente.Text = "Per attivare il tasto (Prepara E-mail) selezionare i Clienti Con E-mail Corrette"
            btnVisualizzaDett.Enabled = False
        Else
            btnVisualizzaDett.Enabled = True
        End If
        lblMessUtente.BackColor = SEGNALA_OK
        WUCArticoliInstEmail.SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If WUCArticoliInstEmail.CheckGrid = False Then
            Exit Sub
        End If
        btnVisualizzaDett.Enabled = False
        lblMessUtente.BackColor = SEGNALA_OK
        WUCArticoliInstEmail.DeselezionaTutti()
    End Sub

    Protected Sub btnModificaMail_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If WUCArticoliInstEmail.CheckGrid = False Then
            Exit Sub
        End If
        btnVisualizza.BackColor = SEGNALA_KO
        lblMessUtente.BackColor = SEGNALA_OK
        WUCArticoliInstEmail.ShowModificaMail()
    End Sub

    Protected Sub btnVisualizzaDett_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Call ChiamaCallBackStampaElencoAI(False, False)
    End Sub

    Protected Sub btnStampaElenco_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(CSTESPORTAPDF) = True
        Call ChiamaCallBackStampaElencoAI(True, False)
    End Sub
    Protected Sub btnStampaSintetica_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(CSTESPORTAPDF) = True
        Call ChiamaCallBackStampaElencoAI(True, True)
    End Sub
    Public Sub ChiamaCallBackStampaElencoAI(ByVal EffettuaStampa As Boolean, ByVal Sintetico As Boolean)

        Dim CaricaGrigliaDettaglio As Boolean = True

        If WUCArticoliInstEmail.CheckGrid = False Then
            Exit Sub
        End If
        lblMessUtente.BackColor = SEGNALA_OK
        Dim CodCogeSelezionati As String = WUCArticoliInstEmail.getCodCogeSelezionati()
        If CodCogeSelezionati.Trim = "" Then
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Nessun dato selezionato"
            Exit Sub
        Else
            lblMessUtente.BackColor = SEGNALA_OK
        End If

        Dim TipoOrdine As String = "Cod_CoGe"
        Dim TipoOrdineRPT As Integer = -1
        If Sintetico = False Then
            TipoOrdineRPT = TipoStampaAICA.ElencoAICACliArtNSerie
        Else
            TipoOrdineRPT = TipoStampaAICA.ElencoAICASintetico
        End If
        Dim TipoOrdineST As String = "Cliente"
        Dim SelScGa As Boolean = chkSelScGa.Checked
        Dim SelScEl As Boolean = chkSelScEl.Checked
        Dim SelScBa As Boolean = chkSelScBa.Checked
        Dim SelTutteCatCli As Boolean = chkTutteCatCli.Checked
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli.Checked
        'giu210618
        Dim SelCliSenzaMail As Boolean = rbtnCliSenzaMail.Checked
        Dim SelCliConMail As Boolean = rbtnCliConEmail.Checked
        Dim SelCliConMailErr As Boolean = rbtnCliConEmailErr.Checked
        Dim SelCliNoInvioEmail As Boolean = rbtnNoInvioEmail.Checked
        '---------
        Dim DataScadenzaDA As DateTime
        Dim DataScadenzaA As DateTime
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
                        lblMessUtente.Text = "Attenzione, avete selezionato (Selezione multipla categorie): si prega di selezionare almeno 2 categorie."
                        Exit Sub
                    End If
                Else
                    lblMessUtente.BackColor = SEGNALA_KO
                    lblMessUtente.Text = "Attenzione, avete selezionato (Selezione multipla categorie): si prega di selezionare almeno 2 categorie."
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
        If Not IsDate(txtDallaData.Text.Trim) Then 'giu040119
            txtDallaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        End If
        If Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        End If
        DataScadenzaDA = CDate(txtDallaData.Text.ToString).Date
        DataScadenzaA = CDate(txtAllaData.Text.ToString).Date

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataScadenzaA.Year, DataScadenzaA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataScadenzaA.Year, DataScadenzaA.Month, DaysInMonth)
        DataScadenzaA = LastDayInMonthDate
        '-

        If DataScadenzaA < DataScadenzaDA Then
            txtDallaData.BackColor = SEGNALA_KO
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            lblMessUtente.BackColor = SEGNALA_KO
            Exit Sub
        Else
            txtDallaData.BackColor = SEGNALA_OK
            txtAllaData.BackColor = SEGNALA_OK
        End If
        '-
        lblMessUtente.BackColor = SEGNALA_OK
        lblMessUtente.Text = "Visualizza dettaglio scadenze"
        'carico dettaglio nella griglia dell'elenco
        Session(F_VISUALIZZADETT_APERTA) = True
        Session(F_ARTINSTEMAIL_APERTA) = False
        ProgrammaticModalPopup.Hide()
        'GIU250618
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        If Session(CSTDsArticoliInstEmail) Is Nothing Then
            DsPrinWebDoc.Clear()
        Else
            DsPrinWebDoc = Session(CSTDsArticoliInstEmail)
        End If
        '-
        _WucElement.CallBackStampaElencoAI(TipoOrdineST, TipoOrdine, TipoOrdineRPT, DataScadenzaDA, DataScadenzaA, _
                                           SelScGa, SelScEl, SelScBa, txtCod1.Text.Trim, "", DsPrinWebDoc, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                                           chkSelCategorie.Checked, CodcategSel, _
                                           SelCliSenzaMail, SelCliConMail, SelCliNoInvioEmail, SelCliConMailErr, _
                                           EffettuaStampa, CaricaGrigliaDettaglio, CodCogeSelezionati)
    End Sub

    Protected Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        btnVisualizza.BackColor = SEGNALA_OKBTN
        lblMessUtente.BackColor = SEGNALA_OK
        Dim SelScGa As Boolean = chkSelScGa.Checked
        Dim SelScEl As Boolean = chkSelScEl.Checked
        Dim SelScBa As Boolean = chkSelScBa.Checked
        Dim SelTutteCatCli As Boolean = chkTutteCatCli.Checked
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli.Checked
        'giu210618
        Dim SelCliSenzaMail As Boolean = rbtnCliSenzaMail.Checked
        Dim SelCliConMail As Boolean = rbtnCliConEmail.Checked
        Dim SelCliConMailErr As Boolean = rbtnCliConEmailErr.Checked
        Dim SelCliNoInvioEmail As Boolean = rbtnNoInvioEmail.Checked
        '---------
        Dim DataScadenzaDA As DateTime
        Dim DataScadenzaA As DateTime
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
        If Not IsDate(txtDallaData.Text.Trim) Then 'giu040119
            txtDallaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        End If
        If Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        End If
        DataScadenzaDA = CDate(txtDallaData.Text.ToString).Date
        DataScadenzaA = CDate(txtAllaData.Text.ToString).Date

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataScadenzaA.Year, DataScadenzaA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataScadenzaA.Year, DataScadenzaA.Month, DaysInMonth)
        DataScadenzaA = LastDayInMonthDate
        '-

        If DataScadenzaA < DataScadenzaDA Then
            txtDallaData.BackColor = SEGNALA_KO
            txtAllaData.BackColor = SEGNALA_KO
            lblMessUtente.BackColor = SEGNALA_KO
            lblMessUtente.Text = "Data di fine periodo deve essere maggiore o uguale alla data inizio periodo."
            Exit Sub
        Else
            txtDallaData.BackColor = SEGNALA_OK
            txtAllaData.BackColor = SEGNALA_OK
        End If
        '-
        lblMessUtente.Text = "Selezione/Deselezione clienti a cui inviare e-mail scadenza"
        'OK FILL GRIGLIA PASSANDO PARAMETRI
        Dim CountGrid As Integer = 0
        CountGrid = WUCArticoliInstEmail.PopolaGridT(DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, txtCod1.Text.Trim, "", _
                                         SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                                         chkSelCategorie.Checked, CodcategSel, _
                                         SelCliSenzaMail, SelCliConMail, SelCliNoInvioEmail, SelCliConMailErr)
        btnVisualizzaDett.Enabled = False
    End Sub

    Public Sub SvuotaGriglia()
        If btnVisualizza.BackColor = SEGNALA_KO Then
            Call SetControl(False)
            Exit Sub
        End If
        btnVisualizza.BackColor = SEGNALA_KO
        WUCArticoliInstEmail.SvuotaGriglia()
        Call SetControl(False)
        lblMessUtente.Text = "Eseguire il tasto visualizza"
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.BackColor = SEGNALA_OK
            lblMessUtente.Text = "Selezione/Deselezione clienti a cui inviare e-mail scadenza"
            btnVisualizza.BackColor = SEGNALA_KO
            WUCArticoliInstEmail.DatiSessioneGriglia()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetControl(ByVal Stato As Boolean)
        btnDeselTutti.Enabled = Stato
        btnSelTutti.Enabled = Stato
        btnModificaMail.Enabled = Stato
        btnStampaElenco.Enabled = Stato
        btnStampaSintetica.Enabled = Stato
        If Stato = True Then
            If rbtnCliConEmail.Checked = True Then
                btnVisualizzaDett.Enabled = Stato
            Else
                btnVisualizzaDett.Enabled = False
            End If
        Else
            btnVisualizzaDett.Enabled = Stato
        End If

    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

    Protected Sub btnSelCategorie_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_ARTINSTEMAIL_APERTA) = False
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
        Session(F_ARTINSTEMAIL_APERTA) = True
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
        Else
            btnSelCategorie.BackColor = SEGNALA_OKLBL
            lblMessUtente.BackColor = SEGNALA_OK
            lblMessUtente.Text = "Selezione/Deselezione clienti a cui inviare e-mail scadenza"
        End If
    End Sub

    Private Sub chkSelCategorie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelCategorie.CheckedChanged
        SvuotaGriglia()
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
        Else
            If chkTutteCatCli.Checked = False Then
                ddlCatCli.Enabled = True
                chkRaggrCatCli.Enabled = True
            End If
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
            lblMessUtente.Text = "Errore LeggiCategorie: " & Ex.Message.Trim
            Exit Function
        End Try
        LeggiCategorie = True
    End Function
End Class