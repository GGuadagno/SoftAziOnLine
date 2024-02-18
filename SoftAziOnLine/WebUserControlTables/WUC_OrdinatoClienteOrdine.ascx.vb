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

Partial Public Class WUC_OrdinatoClienteOrdine
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            If Request.QueryString("labelForm") = "Ordinato per ordine" Then
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
        Dim cod1 As String = ""
        Dim cod2 As String = ""

        Dim TitoloRpt As String = ""

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If chkTuttiClienti.Checked = False Then
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
        Try

            ''If rbtnCodice.Checked Then
            ''    Ordinamento = "Codice_CoGe"
            ''    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge
            ''Else
            ''    Ordinamento = "Rag_Soc"
            ''    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc
            ''End If
            Dim strErroreGiac As String = ""
            Dim SWNegativi As Boolean = False 'giu190613 forzo anche il riclacolo per l'impegno
            If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            End If

            If rbClienteOrdine.Checked Then
                TitoloRpt = "Ordinato per cliente/ordine"
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdine
            ElseIf rbOrdine.Checked Then
                TitoloRpt = "Ordini"
                If rbOrdinamentoNDoc.Checked Then
                    TitoloRpt = "Ordini (Ordinato per N° documento)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDoc
                ElseIf rbOrdinamentoDataDoc.Checked Then
                    TitoloRpt = "Ordini (Ordinato per Data documento)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDoc
                ElseIf rbOrdinamentoDataConsegna.Checked Then
                    TitoloRpt = "Ordini (Ordinato per Data consegna)"
                    Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegna
                End If
            End If

            If ClsPrint.StampaOrdinatoClienteOrdine(Session(CSTAZIENDARPT), TitoloRpt, DsOrdinatoClienteOrdine1, ObjReport, StrErrore, cod1, cod2) Then
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

    'Private Sub rbtnCodice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCodice.CheckedChanged
    'If rbtnCodice.Checked = True Then
    '    pulisciCampi()
    '    If Not chkTuttiClienti.Checked Then
    '        AbilitaDisabilitaComponenti(True)
    '        lblDal.Text = "Dal codice"
    '        lblAl.Text = "Al codice"
    '        txtCod1.Focus()
    '    Else
    '        lblDal.Text = "Dal codice"
    '        lblAl.Text = "Al codice"
    '    End If
    'End If
    'End Sub

    'Private Sub rbtnDescrizione_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnRagSoc.CheckedChanged
    'If rbtnRagSoc.Checked = True Then
    '    pulisciCampi()
    '    If Not chkTuttiClienti.Checked Then
    '        AbilitaDisabilitaComponenti(False)
    '        lblDal.Text = "Dalla ragione sociale"
    '        lblAl.Text = "Alla ragione sociale"
    '        txtDesc1.Focus()
    '    Else
    '        lblDal.Text = "Dalla ragione sociale"
    '        lblAl.Text = "Alla ragione sociale"
    '    End If
    'End If
    'End Sub

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampi()
        If chkTuttiClienti.Checked Then
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnStampa.Focus()
        Else
            'If rbtnCodice.Checked = True Then
            AbilitaDisabilitaComponenti(True)
            txtCod1.Focus()
            'Else
            'AbilitaDisabilitaComponenti(False)
            'txtDesc1.Focus()
            'End If
        End If
    End Sub
    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
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
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub

    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()

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

    Private Sub btnCercaAnagrafica2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica2.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti2()
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.CLIENTI, Session(ESERCIZIO))
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
            rbOrdinamentoDataConsegna.Enabled = False
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False
        End If
    End Sub

    Private Sub rbOrdine_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbOrdine.CheckedChanged
        If rbOrdine.Checked Then
            rbOrdinamentoDataConsegna.Enabled = True
            rbOrdinamentoDataDoc.Enabled = True
            rbOrdinamentoNDoc.Enabled = True
        End If
    End Sub
End Class