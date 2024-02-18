Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WUC_OrdineStampaCli
    Inherits System.Web.UI.UserControl
    Private Chiamante As String
    Private AzReport As String

    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'ATTENZIONE ATTENZIONE giu300312 queste istruzioni fa si di eseguire il comando 2 volte
        'può essere che la pria volta postback sia false e poi a true
        ' ''Me.btnAnnulla.Attributes("onclick") = "this.disabled=true;" & btnAnnulla.Page.GetPostBackEventReference(btnAnnulla).ToString
        ' ''Me.btnCercaAnagrafica1.Attributes("onclick") = "this.disabled=true;" & btnCercaAnagrafica1.Page.GetPostBackEventReference(btnCercaAnagrafica1).ToString
        ' ''Me.btnCercaAnagrafica2.Attributes("onclick") = "this.disabled=true;" & btnCercaAnagrafica2.Page.GetPostBackEventReference(btnCercaAnagrafica2).ToString
        ' ''Me.btnStampaAn.Attributes("onclick") = "this.disabled=true;" & btnStampaAn.Page.GetPostBackEventReference(btnStampaAn).ToString
        ' ''Me.btnStampaCod.Attributes("onclick") = "this.disabled=true;" & btnStampaCod.Page.GetPostBackEventReference(btnStampaCod).ToString
        ' ''Me.btnStampaRub.Attributes("onclick") = "this.disabled=true;" & btnStampaRub.Page.GetPostBackEventReference(btnStampaRub).ToString
        ' ''Me.btnStampaSint.Attributes("onclick") = "this.disabled=true;" & btnStampaSint.Page.GetPostBackEventReference(btnStampaSint).ToString

        ImpostaConnString()

        Chiamante = Session(CSTFinestraChiamante)
        AzReport = Session(CSTAZIENDARPT)
        WFP_ElencoCliForn1.WucElement = Me
        WFP_ElencoCliForn2.WucElement = Me
        WFP_ElencoCliForn3.WucElement = Me
        WFP_ElencoCliForn4.WucElement = Me
        WUC_SceltaStampaCFRubrica1.WUCElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn2.Show()
            ElseIf Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn3.Show()
            ElseIf Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
                WFP_ElencoCliForn4.Show()
            End If
        End If
        If (Not IsPostBack) Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            If Chiamante = "Clienti" Then
                lblAgente.Visible = True
                DDLAgenti.Visible = True
            Else
                lblAgente.Visible = False
                DDLAgenti.Visible = False
            End If
        End If
        'giu230312
        ' ''If Me.Page.IsPostBack Then
        ' ''    Dim eventTarget = IIf(Me.Request("__EVENTTARGET") = Nothing, String.Empty, Me.Request("__EVENTTARGET")) 'Me.Page.Request("__EVENTTARGET")
        ' ''    Dim eventArg = IIf(Me.Request("__EVENTARGUMENT") = Nothing, String.Empty, Me.Request("__EVENTARGUMENT")) 'Me.Page.Request("__EVENTARGUMENT")
        ' ''    Me.Page.Response.Write(String.Format("Controllo={0} ; Argomento={1}", eventTarget, eventArg))
        ' ''End If

    End Sub
    ' ''Public Sub DisabledAll()
    ' ''    'giu230312 da verifica con calma
    ' ''    DisabilitaTuttiComponenti()
    ' ''    btnAnnulla.Enabled = False
    ' ''    btnCercaAnagrafica1.Enabled = False
    ' ''    btnCercaAnagrafica2.Enabled = False
    ' ''    btnStampaAn.Enabled = False
    ' ''    btnStampaCod.Enabled = False
    ' ''    btnStampaRub.Enabled = False
    ' ''    btnStampaSint.Enabled = False
    ' ''    ddlCategorie.Enabled = False
    ' ''    ddlProvince.Enabled = False
    ' ''    ddlZone.Enabled = False
    ' ''End Sub
    'Private Sub rbtn_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnRagSoc.CheckedChanged, rbtnCodice.CheckedChanged, rbtnLocalita.CheckedChanged, rbtnPIva.CheckedChanged
    'If rbtnRagSoc.Checked = True Then
    '    AbilitaDisabilitaComponenti(False)
    '    lblDal.Text = "Dalla ragione sociale"
    '    lblAl.Text = "Alla ragione sociale"
    '    txtCod1.Text = ""
    '    txtCod2.Text = ""
    '    txtDesc1.Text = ""
    '    txtDesc2.Text = ""
    '    txtDesc1.Focus()
    'ElseIf rbtnLocalita.Checked = True Then
    '    DisabilitaTuttiComponenti()
    '    txtCod1.Text = ""
    '    txtCod2.Text = ""
    '    txtDesc1.Text = ""
    '    txtDesc2.Text = ""
    'ElseIf rbtnPIva.Checked = True Then
    '    AbilitaDisabilitaComponenti(False)
    '    lblDal.Text = "Dalla partita IVA"
    '    lblAl.Text = "Alla partita IVA"
    '    txtCod1.Text = ""
    '    txtCod2.Text = ""
    '    txtDesc1.Text = ""
    '    txtDesc2.Text = ""
    '    txtDesc1.Focus()
    'ElseIf rbtnCodice.Checked = True Then
    '    AbilitaDisabilitaComponenti(True)
    '    lblDal.Text = "Dal codice"
    '    lblAl.Text = "Al codice"
    '    txtCod1.Text = ""
    '    txtCod2.Text = ""
    '    txtDesc1.Text = ""
    '    txtDesc2.Text = ""
    '    txtCod1.Focus()
    'End If
    'End Sub

    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        'btnCod1.Enabled = Abilita
        'btnCod2.Enabled = Abilita
        txtDesc1.Enabled = Not (Abilita)
        txtDesc2.Enabled = Not (Abilita)
    End Sub



    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            If Chiamante = "Clienti" Then
                Response.Redirect("..\WebFormTables\WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
                Exit Sub
            Else
                Response.Redirect("..\WebFormTables\WF_AnagraficaFornitori.aspx?labelForm=Anagrafica fornitori")
                Exit Sub
            End If
        Catch ex As Exception
            If Chiamante = "Clienti" Then
                Response.Redirect("..\WebFormTables\WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti")
            Else
                Response.Redirect("..\WebFormTables\WF_AnagraficaFornitori.aspx?labelForm=Anagrafica fornitori")
            End If
            Exit Sub
        End Try
    End Sub

    Private Sub DisabilitaTuttiComponenti()
        txtCod1.Enabled = False
        txtCod2.Enabled = False
        'btnCod1.Enabled = False
        'btnCod2.Enabled = False
        txtDesc1.Enabled = False
        txtDesc2.Enabled = False
    End Sub

    Private Sub ImpostaConnString()
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim connStr As String
        connStr = dbCon.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftCoge)
        SqlDSCategorie.ConnectionString = connStr
        ' ''SqlDSRegioni.ConnectionString = connStr
        SqlDSProvince.SelectCommand = "SELECT * FROM Province ORDER BY Descrizione"
        SqlDSProvince.ConnectionString = connStr
        SqlDSZone.ConnectionString = connStr
        SqlDSAgenti.ConnectionString = connStr
    End Sub

    '' ''Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
    '' ''    If Trim(ddlRegioni.SelectedValue) <> "" Then
    '' ''        SqlDSProvince.SelectCommand = "SELECT * FROM Province WHERE Regione = " & Trim(ddlRegioni.SelectedValue)
    '' ''    Else
    '' ''        SqlDSProvince.SelectCommand = "SELECT * FROM Province WHERE Regione = -1"
    '' ''    End If
    '' ''    SqlDSProvince.SelectCommandType = SqlDataSourceCommandType.Text
    '' ''    ddlProvince.Items.Clear()
    '' ''    ddlProvince.DataBind()
    '' ''End Sub

    Protected Sub btnStampaAn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampaAn.Click
        Dim clsPrintCLI As New StampaClienti
        Dim clsPrintFOR As New StampaFornitori
        Dim dsStampa As New dsClienti
        Dim Errore As String = ""
        Dim Ordinamento As String = ""
        Dim Where As String = ""
        Dim strFiltri As String = ""

        If txtCod1.Text.Trim > txtCod2.Text.Trim And txtCod2.Text.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il codice di fine selezione deve essere successivo al codice di inizio selezione", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TipoStampa") = 3

        Try
            If rbtnCodice.Checked Then
                Ordinamento = "Codice_CoGe"
            ElseIf rbtnRagSoc.Checked Then
                Ordinamento = "Rag_Soc"
            ElseIf rbtnPIva.Checked Then
                Ordinamento = "Partita_IVA"
            ElseIf rbtnLocalita.Checked Then
                Ordinamento = "Localita"
            End If

            Where = CostruisciWhere()

            strFiltri = "Filtri: "
            If txtCod1.Text.Trim <> "" Then
                strFiltri = strFiltri & " dal codice " & txtCod1.Text.Trim
            End If
            If txtCod2.Text.Trim <> "" Then
                strFiltri = strFiltri & " fino al codice " & txtCod2.Text.Trim
            Else
                strFiltri = strFiltri & " fino all'ultimo codice"
            End If
            If ddlCategorie.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - categoria: " & ddlCategorie.SelectedItem.Text.Trim
            End If
            If ddlProvince.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - provincia: " & ddlProvince.SelectedItem.Text.Trim
            End If
            If ddlZone.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - zona: " & ddlZone.SelectedItem.Text.Trim
            End If
            If DDLAgenti.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - agente: " & DDLAgenti.SelectedItem.Text.Trim
            End If
            If Chiamante = "Clienti" Then
                If clsPrintCLI.StampaClienti(AzReport, dsStampa, Errore, 3, Ordinamento, Where, strFiltri) Then
                    If dsStampa.vi_CliAnalit.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                    
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If clsPrintFOR.StampaFornitori(AzReport, dsStampa, Errore, 3, Ordinamento, Where, strFiltri) Then
                    If dsStampa.vi_ForAnalit.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Attenzione, " & ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in btnStampaElencoSint", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnStampaCod_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaCod.Click
        Dim clsPrintCLI As New StampaClienti
        Dim clsPrintFOR As New StampaFornitori
        Dim dsStampa As New dsClienti
        Dim Errore As String = ""
        Dim Ordinamento As String = ""
        Dim Where As String = ""
        If txtCod1.Text.Trim > txtCod2.Text.Trim And txtCod2.Text.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il codice di fine selezione deve essere successivo al codice di inizio selezione", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TipoStampa") = 4

        Try
            If rbtnCodice.Checked Then
                Ordinamento = "Codice_CoGe"
            ElseIf rbtnRagSoc.Checked Then
                Ordinamento = "Rag_Soc"
            ElseIf rbtnPIva.Checked Then
                Ordinamento = "Partita_IVA"
            ElseIf rbtnLocalita.Checked Then
                Ordinamento = "Localita"
            End If

            Where = CostruisciWhere()
            Dim strFiltri As String = "Filtri: "
            If txtCod1.Text.Trim <> "" Then
                strFiltri = strFiltri & " dal codice " & txtCod1.Text.Trim
            End If
            If txtCod2.Text.Trim <> "" Then
                strFiltri = strFiltri & " fino al codice " & txtCod2.Text.Trim
            Else
                strFiltri = strFiltri & " fino all'ultimo codice"
            End If
            If ddlCategorie.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - categoria: " & ddlCategorie.SelectedItem.Text.Trim
            End If
            If ddlProvince.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - provincia: " & ddlProvince.SelectedItem.Text.Trim
            End If
            If ddlZone.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - zona: " & ddlZone.SelectedItem.Text.Trim
            End If
            If DDLAgenti.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - agente: " & DDLAgenti.SelectedItem.Text.Trim
            End If
            If Chiamante = "Clienti" Then
                If clsPrintCLI.StampaClienti(AzReport, dsStampa, Errore, 4, Ordinamento, Where, strFiltri) Then
                    If dsStampa.vi_CliCod.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                   
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If clsPrintFOR.StampaFornitori(AzReport, dsStampa, Errore, 4, Ordinamento, Where, strFiltri) Then
                    If dsStampa.vi_ForCod.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Attenzione, " & ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in btnStampaElencoSint", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnStampaRub_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaRub.Click
        WUC_SceltaStampaCFRubrica1.Show()
    End Sub

    Private Sub btnStampaSint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSint.Click
        Dim clsPrintCLI As New StampaClienti
        Dim clsPrintFOR As New StampaFornitori
        Dim dsStampa As New dsClienti
        Dim Errore As String = ""
        Dim Ordinamento As String = ""
        Dim Where As String = ""
        If txtCod1.Text.Trim > txtCod2.Text.Trim And txtCod2.Text.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il codice di fine selezione deve essere successivo al codice di inizio selezione", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TipoStampa") = 1

        Try
            If rbtnCodice.Checked Then
                Ordinamento = "Codice_CoGe"
            ElseIf rbtnRagSoc.Checked Then
                Ordinamento = "Rag_Soc"
            ElseIf rbtnPIva.Checked Then
                Ordinamento = "Partita_IVA"
            ElseIf rbtnLocalita.Checked Then
                Ordinamento = "Localita"
            End If

            Where = CostruisciWhere()
            Dim strFiltri As String = "Filtri: "
            If txtCod1.Text.Trim <> "" Then
                strFiltri = strFiltri & " dal codice " & txtCod1.Text.Trim
            End If
            If txtCod2.Text.Trim <> "" Then
                strFiltri = strFiltri & " fino al codice " & txtCod2.Text.Trim
            Else
                strFiltri = strFiltri & " fino all'ultimo codice"
            End If
            If ddlCategorie.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - categoria: " & ddlCategorie.SelectedItem.Text.Trim
            End If
            If ddlProvince.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - provincia: " & ddlProvince.SelectedItem.Text.Trim
            End If
            If ddlZone.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - zona: " & ddlZone.SelectedItem.Text.Trim
            End If
            If DDLAgenti.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - agente: " & DDLAgenti.SelectedItem.Text.Trim
            End If
            If Chiamante = "Clienti" Then
                If clsPrintCLI.StampaClienti(AzReport, dsStampa, Errore, 1, Ordinamento, Where, strFiltri) Then
                    If dsStampa.vi_CliSint.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If clsPrintFOR.StampaFornitori(AzReport, dsStampa, Errore, 1, Ordinamento, Where, strFiltri) Then                    
                    If dsStampa.vi_ForSint.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Attenzione, " & ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in btnStampaElencoSint", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Function CostruisciWhere() As String
        Dim strWhere As String = ""

        If txtCod1.Text.Trim <> "" Then
            strWhere = " WHERE (Codice_CoGe >= '" & Controlla_Apice(txtCod1.Text.Trim) & "' "
        Else
            strWhere = " WHERE (Codice_CoGe >= '' "
        End If

        If txtCod2.Text.Trim <> "" Then                       
            strWhere = strWhere & " AND Codice_Coge <= '" & Controlla_Apice(txtCod2.Text.Trim) & "') "
        Else
            strWhere = strWhere & " AND Codice_Coge <= 'ZZZZZZZZZZZZZZZZ') "
        End If

        If ddlCategorie.SelectedIndex <> 0 Then           
            strWhere = strWhere & " AND (Categoria = " & ddlCategorie.SelectedValue & ") "
        End If

        If ddlProvince.SelectedIndex <> 0 Then
            strWhere = strWhere & " AND (Provincia = '" & ddlProvince.SelectedValue & "'" & ") "
        End If

        If ddlZone.SelectedIndex <> 0 Then
            strWhere = strWhere & " AND (Zona = " & ddlZone.SelectedValue & ")"
        End If
        If DDLAgenti.SelectedIndex <> 0 Then
            strWhere = strWhere & " AND (Agente_N = " & DDLAgenti.SelectedValue & ")"
        End If
        CostruisciWhere = strWhere
    End Function


    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        If Chiamante = "Clienti" Then
            Session(F_CLI_RICERCA) = True
            ApriElencoClienti1()
        Else
            Session(F_FOR_RICERCA) = True
            ApriElencoFornitori1()
        End If

    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCod1.Text = codice
            txtDesc1.Text = descrizione
            If txtCod2.Text.Trim = "" Then
                txtCod2.Text = codice
                txtDesc2.Text = descrizione
            End If
        ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
            txtCod2.Text = codice
            txtDesc2.Text = descrizione
        End If
        'Session(COD_CLIENTE) = codice
        'Session(CSTCODCOGE) = codice 'giu261111
        'VisualizzaCliente()
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub

    Private Sub ApriElencoClienti1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Private Sub ApriElencoClienti2()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = True
        WFP_ElencoCliForn2.Show(True)
    End Sub

    Private Sub ApriElencoFornitori1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn3.Show(True)
    End Sub

    Private Sub ApriElencoFornitori2()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = True
        WFP_ElencoCliForn4.Show(True)
    End Sub

    Private Sub Chiudi(ByVal strErrore As String)

        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StampaClienti.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub btnCercaAnagrafica2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica2.Click       
        If Chiamante = "Clienti" Then
            Session(F_CLI_RICERCA) = True
            ApriElencoClienti2()
        Else
            Session(F_FOR_RICERCA) = True
            ApriElencoFornitori2()
        End If
    End Sub

    Protected Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Protected Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Public Sub StampaRubrica(ByVal DettagliStampa As Integer)
        Dim clsPrintCLI As New StampaClienti
        Dim clsPrintFOR As New StampaFornitori
        Dim dsStampa As New dsClienti
        Dim Errore As String = ""
        Dim Ordinamento As String = ""
        Dim Where As String = ""

        If txtCod1.Text.Trim > txtCod2.Text.Trim And txtCod2.Text.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Il codice di fine selezione deve essere successivo al codice di inizio selezione", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TipoStampa") = 2

        Try
            If rbtnCodice.Checked Then
                Ordinamento = "Codice_CoGe"
            ElseIf rbtnRagSoc.Checked Then
                Ordinamento = "Rag_Soc"
            ElseIf rbtnPIva.Checked Then
                Ordinamento = "Partita_IVA"
            ElseIf rbtnLocalita.Checked Then
                Ordinamento = "Localita"
            End If

            If DettagliStampa = -1 Then
                Exit Sub
            End If

            Session("DettagliStampa") = DettagliStampa

            Where = CostruisciWhere()
            Dim strFiltri As String = "Filtri: "
            If txtCod1.Text.Trim <> "" Then
                strFiltri = strFiltri & " dal codice " & txtCod1.Text.Trim
            End If
            If txtCod2.Text.Trim <> "" Then
                strFiltri = strFiltri & " fino al codice " & txtCod2.Text.Trim
            Else
                strFiltri = strFiltri & " fino all'ultimo codice"
            End If
            If ddlCategorie.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - categoria: " & ddlCategorie.SelectedItem.Text.Trim
            End If
            If ddlProvince.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - provincia: " & ddlProvince.SelectedItem.Text.Trim
            End If
            If ddlZone.SelectedIndex <> 0 Then
                strFiltri = strFiltri & " - zona: " & ddlZone.SelectedItem.Text.Trim
            End If
            If Chiamante = "Clienti" Then
                If clsPrintCLI.StampaClienti(AzReport, dsStampa, Errore, 2, Ordinamento, Where, strFiltri, DettagliStampa) Then
                    If dsStampa.vi_CliSint.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If clsPrintFOR.StampaFornitori(AzReport, dsStampa, Errore, 2, Ordinamento, Where, strFiltri, DettagliStampa) Then
                    If dsStampa.vi_ForRub.Count > 0 Then
                        Session("dsStampa") = dsStampa
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebCR_Clienti.aspx")
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Nessun dato soddisfa i criteri di ricerca specificati.", Errore, WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            Debug.Write(ex.Message)
            lblMessUtente.Text = "Attenzione, " & ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in btnStampaElencoSint", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
End Class