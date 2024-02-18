Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino
Partial Public Class WUC_OrdinatoClienteOrdineAG
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        If (Not IsPostBack) Then
            'rbtnCodice.Checked = True
            'chkTuttiClienti.Checked = True
            'txtCod1.Enabled = False
            'txtCod2.Enabled = False
            'btnCercaAnagrafica1.Enabled = False
            'btnCercaAnagrafica2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            'txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            'txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            chkTuttiClienti.Checked = True
            txtCodCli1.Enabled = False
            txtCodCli2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            If Request.QueryString("labelForm") = "Ordinato per ordine per agente" Then
                rbClienteOrdine.Checked = False
                rbOrdine.Checked = True
                rbOrdinamentoDataConsegna.Enabled = True
                rbOrdinamentoDataDoc.Enabled = True
                rbOrdinamentoNDoc.Enabled = True
                rbOrdinamentoNDoc.Checked = True
            Else
                rbClienteOrdine.Checked = True
                rbOrdine.Checked = False
                rbOrdinamentoNDoc.Checked = True
                rbOrdinamentoDataConsegna.Enabled = False
                rbOrdinamentoDataDoc.Enabled = False
                rbOrdinamentoNDoc.Enabled = False
                '-
                rbOrdinamentoDataConsegna.Checked = False
                rbOrdinamentoDataDoc.Checked = False
                rbOrdinamentoNDoc.Checked = True
            End If

        End If
        ModalPopup.WucElement = Me

        WFP_ElencoCliForn1.WucElement = Me
        WFP_ElencoCliForn2.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn2.Show()
            End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""
        ' ''Dim Ordinamento As String
        Dim codCli1 As String = ""
        Dim codCli2 As String = ""
        Dim Agente As Integer = -1

        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If chkTuttiClienti.Checked = False Then
            If txtCodCli1.Text <> "" And txtCodCli2.Text <> "" Then
                If txtCodCli1.Text > txtCodCli2.Text Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Il codice di inizio intervallo è superiore a quello finale.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                Else
                    codCli1 = txtCodCli1.Text.Trim
                    codCli2 = txtCodCli2.Text.Trim
                End If
            End If
        End If
        Try

            Dim strErroreGiac As String = ""
            Dim SWNegativi As Boolean = False 'giu190613 forzo anche il riclacolo per l'impegno
            If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            End If

            If rbClienteOrdine.Checked Then
                TitoloRpt = "Ordinato per cliente/ordine per agente"
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdineAG
            ElseIf rbOrdine.Checked Then
                TitoloRpt = "Ordini per agente"
                If rbOrdinamentoNDoc.Checked Then
                    TitoloRpt = "Ordini per agente (Ordinato per N° documento)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDocAG
                ElseIf rbOrdinamentoDataDoc.Checked Then
                    TitoloRpt = "Ordini per agente (Ordinato per Data documento)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDocAG
                ElseIf rbOrdinamentoDataConsegna.Checked Then
                    TitoloRpt = "Ordini per agente (Ordinato per Data consegna)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegnaAG
                End If
            End If

            If chkTuttiAgenti.Checked Then
                Agente = -1
            Else
                Agente = ddlAgenti.SelectedValue
            End If

            If ClsPrint.StampaOrdinatoClienteOrdineAG(Session(CSTAZIENDARPT), TitoloRpt, DsOrdinatoClienteOrdine1, ObjReport, StrErrore, codCli1, codCli2, Agente) Then
                If DsOrdinatoClienteOrdine1.OrdinatoClienteOrdine.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsOrdinatoClienteOrdine1
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

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampi()
        If chkTuttiClienti.Checked Then
            txtCodCli1.Enabled = False
            txtCodCli2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnStampa.Focus()
        Else
            'If rbtnCodice.Checked = True Then
            AbilitaDisabilitaComponenti(True)
            txtCodCli1.Focus()
            'Else
            'AbilitaDisabilitaComponenti(False)
            'txtDesc1.Focus()
            'End If
        End If
    End Sub
    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCodCli1.Enabled = Abilita
        txtCodCli2.Enabled = Abilita
        btnCercaAnagrafica1.Enabled = Abilita
        btnCercaAnagrafica2.Enabled = Abilita
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
        txtCodCli1.Text = ""
        txtCodCli2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub

    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()

    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodCli1.Text = codice
            txtDesc1.Text = descrizione
            If txtCodCli2.Text.Trim = "" Then
                txtCodCli2.Text = codice
                txtDesc2.Text = descrizione
            End If
        ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
            txtCodCli2.Text = codice
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

    Private Sub btnCercaAnagrafica2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica2.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti2()
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCli1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCodCli1.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCli2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCodCli2.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    'non usato qui
    Private Sub StampaListaCarico()
        Dim DsListaCarico1 As New DSListaCarico
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""

        Dim cod1 As String = ""
        Dim cod2 As String = ""

        Try
            Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione
            If ClsPrint.StampaListaCarico(Session(CSTAZIENDARPT), "Lista carico", DsListaCarico1, ObjReport, StrErrore, -1, 1) Then
                If DsListaCarico1.view_ListaCaricoSpedizione.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsListaCarico1
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

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        StampaListaCarico()
    End Sub

    Private Sub rbClienteOrdine_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbClienteOrdine.CheckedChanged
        If rbClienteOrdine.Checked Then
            rbOrdine.Checked = False
            rbOrdinamentoNDoc.Checked = True
            rbOrdinamentoDataConsegna.Enabled = False
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False
            '-
            rbOrdinamentoDataConsegna.Checked = False
            rbOrdinamentoDataDoc.Checked = False
            rbOrdinamentoNDoc.Checked = True
        End If
    End Sub

    Private Sub rbOrdine_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbOrdine.CheckedChanged
        If rbOrdine.Checked Then
            rbOrdinamentoDataConsegna.Enabled = True
            rbOrdinamentoDataDoc.Enabled = True
            rbOrdinamentoNDoc.Enabled = True
        End If
    End Sub

    Private Sub chkTuttiAgenti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiAgenti.CheckedChanged
        ddlAgenti.SelectedIndex = -1
        If chkTuttiAgenti.Checked Then
            ddlAgenti.Enabled = False
        Else
            ddlAgenti.Enabled = True
        End If
    End Sub
End Class