Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino

Partial Public Class WUC_StatOrdinatoClienteOrdine
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        'alb080213
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSAgenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        'alb080213 END

        If (Not IsPostBack) Then
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
            chkTutteProvince.AutoPostBack = False : chkTutteRegioni.AutoPostBack = False : chkTuttiAgenti.AutoPostBack = False : chkTuttiArticoli.AutoPostBack = False
            chkTuttiArticoli.Checked = True
            chkTutteRegioni.Checked = True
            ddlRegioni.Enabled = False
            'alb080213
            chkTutteProvince.Checked = True
            ddlProvince.Enabled = False
            chkTuttiAgenti.Checked = True
            ddlAgenti.Enabled = False
            Session("CodRegione") = 0
            SqlDSProvince.DataBind()
            'alb080213 END
            '-
            ddlRegioni.SelectedIndex = -1
            SqlDSProvince.DataBind()
            ddlProvince.SelectedIndex = -1
            ddlAgenti.SelectedIndex = -1
            '-
            chkTutteProvince.AutoPostBack = True : chkTutteRegioni.AutoPostBack = True : chkTuttiAgenti.AutoPostBack = True : chkTuttiArticoli.AutoPostBack = True
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
                'ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                '    WFP_ElencoCliForn2.Show()
            End If
        End If
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub
    'giu300519
    Private Sub txtCodCli1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCli1.TextChanged
        txtDesCli1.Text = App.GetValoreFromChiave(txtCodCli1.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub
    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()

    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodCli1.Text = codice
            txtDesCli1.Text = descrizione
            'If txtCodCli2.Text.Trim = "" Then
            '    txtCodCli2.Text = codice
            '    txtDesc2.Text = descrizione
            'End If
            'ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
            '    txtCodCli2.Text = codice
            '    txtDesc2.Text = descrizione
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
    '---------
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsStatOrdinatoClienteOrdine1 As New dsStatOrdinatoClienteOrdine
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""
        Dim cod1 As String = ""
        Dim cod2 As String = ""
        Dim codRegione As String = ""
        'alb080213
        Dim codProvincia As String = ""
        Dim codAgente As String = ""
        'alb080213 END
        Dim Data1 As String = ""
        Dim Data2 As String = ""

        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        txtDesCli1.Text = App.GetValoreFromChiave(txtCodCli1.Text, Def.CLIENTI, Session(ESERCIZIO))
        If txtCodCli1.Text.Trim <> "" And txtDesCli1.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice cliente inesistente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                If txtCod1.Text > txtCod2.Text Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Il codice di inizio intervallo è superiore a quello finale.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                Else
                    cod1 = txtCod1.Text.Trim
                    cod2 = txtCod2.Text.Trim
                End If
            End If
        End If

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "La data di inizio intervallo è superiore a quella finale.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                Data1 = txtDataDa.Text.Trim
                Data2 = txtDataA.Text.Trim
            End If
        Else
            If txtDataDa.Text <> "" Then Data1 = txtDataDa.Text.Trim
            If txtDataA.Text <> "" Then Data2 = txtDataDa.Text.Trim
        End If

        If chkTutteRegioni.Checked = False Then
            codRegione = ddlRegioni.SelectedValue
        Else
            codRegione = ""
        End If

        'alb080213
        If chkTutteProvince.Checked = False Then
            codProvincia = ddlProvince.SelectedValue
        Else
            codProvincia = ""
        End If

        If chkTuttiAgenti.Checked = False Then
            codAgente = ddlAgenti.SelectedValue
        Else
            codAgente = ""
        End If
        'alb080213 END

        Try

            Dim strErroreGiac As String = ""
            Dim SWNegativi As Boolean = False 'giu190613 forzo anche il riclacolo per l'impegno
            If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            End If
            'GIU201219
            TitoloRpt = "Statistica ordinato per Regione/Provincia/Agente/cliente/ordine [TUTTI]" & IIf(Data1 <> "", " - Dalla data " & txtDataDa.Text.Trim, "") & _
                        IIf(Data2 <> "", " - Alla data " & txtDataA.Text.Trim, "") & _
                        IIf(chkTutteRegioni.Checked = False And ddlRegioni.SelectedValue <> "", " - Regione " & ddlRegioni.Text, "")
            '---------
            Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdinatoClienteOrdine
            If ClsPrint.StampaStatOrdinatoClienteOrdine(Session(CSTAZIENDARPT), TitoloRpt, DsStatOrdinatoClienteOrdine1, ObjReport, StrErrore, cod1, cod2, codRegione, Data1, Data2, codProvincia, codAgente, txtCodCli1.Text.Trim) Then
                If DsStatOrdinatoClienteOrdine1.StatOrdinatoClienteOrdine.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsStatOrdinatoClienteOrdine1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
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
            ModalPopup.Show("Errore in Ordinato.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampi()
        If chkTuttiArticoli.Checked Then
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnStampa.Focus()
        Else
            AbilitaDisabilitaComponenti(True)
            txtCod1.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
        txtDesc1.Enabled = Not (Abilita)
        txtDesc2.Enabled = Not (Abilita)
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        End Try
    End Sub
    Private Sub pulisciCampi()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub

    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

   Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub

    Private Sub chkTutteRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        If chkTutteRegioni.Checked Then
            ddlRegioni.Enabled = False
            Session("CodRegione") = 0 'alb080213
        Else
            ddlRegioni.Enabled = True
            ddlRegioni.Focus()
            Session("CodRegione") = ddlRegioni.SelectedValue 'alb080213
        End If
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
    End Sub

    'alb080213
    Private Sub chkTutteProvince_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteProvince.CheckedChanged
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = 0
        Else
            ddlProvince.Enabled = True
            ddlProvince.Focus()
        End If
    End Sub

    Private Sub chkTuttiAgenti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiAgenti.CheckedChanged
        If chkTuttiAgenti.Checked Then
            ddlAgenti.Enabled = False
        Else
            ddlAgenti.Enabled = True
            ddlAgenti.Focus()
        End If
    End Sub

    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        Session("CodRegione") = ddlRegioni.SelectedValue
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
    End Sub
End Class
