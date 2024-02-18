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

Partial Public Class WUC_OrdinatoPerCliente
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            rbtnCodice.Checked = True
            'chkTuttiClienti.Checked = True
            'txtCod1.Enabled = False
            'txtCod2.Enabled = False
            'btnCercaAnagrafica1.Enabled = False
            'btnCercaAnagrafica2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            chkTuttiClienti.Checked = True
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
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
        Dim DsOrdinatoPerCliente1 As New DSOrdinatoPerCliente
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""
        Dim Ordinamento As String
        Dim cod1 As String = ""
        Dim cod2 As String = ""
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If chkTuttiClienti.Checked = False Then
            'If rbtnCodice.Checked Then
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
                'Else
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                '    ModalPopup.Show("Attenzione", "Inserire il codice di inizio intervallo e il codice di fine intervallo.", WUC_ModalPopup.TYPE_ALERT)
                '    Exit Sub
            End If
            'Else
            '    If txtDesc1.Text <> "" And txtDesc2.Text <> "" Then
            '        If txtDesc1.Text > txtDesc2.Text Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '            ModalPopup.Show("Attenzione", "La ragione sociale di inizio intervallo è superiore a quella finale.", WUC_ModalPopup.TYPE_ALERT)
            '            Exit Sub
            '        Else
            '            cod1 = txtDesc1.Text.Trim
            '            cod2 = txtDesc2.Text.Trim
            '        End If
            '        'Else
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '        '    ModalPopup.Show("Attenzione", "Inserire la ragione sociale di inizio intervallo e la ragione sociale di fine intervallo.", WUC_ModalPopup.TYPE_ALERT)
            '        Exit Sub
            '    End If
            'End If
        End If
        ' '' ''If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
        ' '' ''    If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
        ' '' ''        If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' '' ''            ModalPopup.Show("Attenzione", "Data inizio intervallo superiore alla data di fine.", WUC_ModalPopup.TYPE_ALERT)
        ' '' ''            Exit Sub
        ' '' ''        End If
        ' '' ''    End If
        ' '' ''End If
        Try
            'If txtDataDa.Text <> "" Or txtDataA.Text <> "" Then
            '    Session(CSTORDINATO) = 2 'REPORT ORDINATO PER ARTICOLO DATA
            'Else
            '    Session(CSTORDINATO) = 1 'REPORT ORDINATO PER ARTICOLO
            'End If
            Dim strErroreGiac As String = ""
            Dim SWNegativi As Boolean = False 'giu190613 forzo anche il riclacolo per l'impegno
            If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            End If
            If rbtnCodice.Checked Then
                Ordinamento = "Codice_CoGe"
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge
            Else
                Ordinamento = "Rag_Soc"
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc
            End If
            'If ClsPrint.StampaOrdinato(Session(CSTORDINATO), txtCod1.Text, txtCod2.Text, txtDesc1.Text, txtDesc2.Text, txtDataDa.Text, txtDataA.Text, "Azienda AAAAABBBB Da Sostituire", Ordinamento, DsOrdinato1, ObjReport, StrErrore) Then
            '    Session(CSTObjReport) = ObjReport
            '    Session(CSTDsOrdinato) = DsOrdinato1
            '    Session(CSTNOBACK) = 0 'giu040512
            '    Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
            'Else
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            'End If
            If ClsPrint.StampaOrdinatoCliente(Session(CSTAZIENDARPT), "Ordinato per cliente", DsOrdinatoPerCliente1, ObjReport, StrErrore, cod1, cod2, Ordinamento) Then
                If DsOrdinatoPerCliente1.OrdinatoPerCliente.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsOrdinatoPerCliente1
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
End Class