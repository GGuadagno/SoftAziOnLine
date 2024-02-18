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
Partial Public Class WUC_ValorizzaCarichiScarichiDoc
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            ddlMagazzino.SelectedValue = 1
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False

            chkTutti.Checked = True
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCercaAnagrafica1.Enabled = False
            btnCercaAnagrafica2.Enabled = False
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            rbFornitoreDocumento.Checked = True
            rbDocumento.Checked = False
            rbOrdinamentoNDoc.Checked = True
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False

            'alberto 11/05/2012
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = Format(Now, FormatoData)
            '----------------
        End If
        ModalPopup.WucElement = Me

        WFP_ElencoCliForn1.WucElement = Me
        WFP_ElencoCliForn2.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            ElseIf Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
                WFP_ElencoCliForn2.Show()
            End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Session(CSTNOMEPDF) = "" 'giu300315
        Dim dsValorizzaCMSMFor As New DSMovMag
        Dim ObjReport As New Object
        Dim ClsPrint As New Fatturato
        Dim StrErrore As String = ""
        ' ''Dim Ordinamento As String
        Dim cod1 As String = ""
        Dim cod2 As String = ""

        Dim TitoloRpt As String = ""
        Dim FiltriTitolo As String = "" 'alberto 11/05/2012

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If ddlMagazzino.SelectedItem.Text = "" Or Not IsNumeric(ddlMagazzino.SelectedValue.Trim) Then
            ddlMagazzino.BackColor = SEGNALA_KO
            Exit Sub
        Else
            ddlMagazzino.BackColor = SEGNALA_OK
        End If
        If chkTutti.Checked = False Then
            If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                If txtCod1.Text > txtCod2.Text Then
                    ModalPopup.Show("Attenzione", "Il codice di inizio intervallo è superiore a quello finale.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                Else
                    cod1 = txtCod1.Text.Trim
                    cod2 = txtCod2.Text.Trim
                End If
            End If
        End If

        If txtDataA.Text.Trim = "" And txtDataDa.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Inserire intervallo date desiderato.", WUC_ModalPopup.TYPE_ALERT)
            txtDataDa.Focus()
            Exit Sub
        End If
        If txtDataA.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Inserire data di fine intervallo.", WUC_ModalPopup.TYPE_ALERT)
            txtDataA.Focus()
            Exit Sub
        End If
        If txtDataDa.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Inserire data di inizio intervallo.", WUC_ModalPopup.TYPE_ALERT)
            txtDataDa.Focus()
            Exit Sub
        End If

        FiltriTitolo = "dal " & txtDataDa.Text.Trim & " al " & txtDataA.Text.Trim

        Try
            'giu060212 non serve per le fatture
            ' ''Dim strErroreGiac As String = ""
            ' ''If Ricalcola_Giacenze("", strErroreGiac) = False Then
            ' ''    ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            ' ''End If
            Dim SWSintetico As Boolean = False
            If rbFornitoreDocumento.Checked Then
                TitoloRpt = "Valorizza Carichi/Scarichi Fornitore - Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim + " (Ordinato per Fornitore/N° documento) " & FiltriTitolo
                Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortCliForNDoc
            ElseIf rbDocumento.Checked Then
                TitoloRpt = "Valorizza Carichi/Scarichi"
                If rbOrdinamentoNDoc.Checked Then
                    TitoloRpt = "Valorizza Carichi/Scarichi Fornitore - Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim + " (Ordinato per N° documento) " & FiltriTitolo
                    Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByNDoc
                ElseIf rbOrdinamentoDataDoc.Checked Then
                    TitoloRpt = "Valorizza Carichi/Scarichi Fornitore - Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim + " (Ordinato per Data documento) " & FiltriTitolo
                    Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMOrdineSortByDataDoc
                End If
            ElseIf rbSinteticoDoc.Checked Then
                TitoloRpt = "Valorizza Carichi/Scarichi Fornitore sintetico - Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim + " (Ordinato per N° documento) " & FiltriTitolo
                Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ValCMSMSintOrdineSortByNDoc
                SWSintetico = True
            End If

            If SWSintetico = False Then
                If ClsPrint.StampaValorizzaCMSMFor(Session(CSTAZIENDARPT), TitoloRpt, dsValorizzaCMSMFor, ObjReport, StrErrore, cod1, cod2, txtDataDa.Text.Trim, txtDataA.Text.Trim, ddlMagazzino.SelectedValue) Then
                    If dsValorizzaCMSMFor.ValorizzaCMSMFor.Count > 0 Then
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDsPrinWebDoc) = dsValorizzaCMSMFor
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx")
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                End If
            Else
                If ClsPrint.StampaValorizzaCMSMForSintetico(Session(CSTAZIENDARPT), TitoloRpt, dsValorizzaCMSMFor, ObjReport, StrErrore, cod1, cod2, txtDataDa.Text.Trim, txtDataA.Text.Trim, ddlMagazzino.SelectedValue) Then
                    If dsValorizzaCMSMFor.ValorizzaCMSMForSintetico.Count > 0 Then
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDsPrinWebDoc) = dsValorizzaCMSMFor
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx")
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            ModalPopup.Show("Errore in Fatturato.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutti.CheckedChanged
        pulisciCampi()
        If chkTutti.Checked Then
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

        Session(F_FOR_RICERCA) = True
        ApriElencoFor1()

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
    Private Sub ApriElencoFor1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Private Sub ApriElencoFor2()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = True
        WFP_ElencoCliForn2.Show(True)
    End Sub

    Private Sub btnCercaAnagrafica2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica2.Click
        Session(F_FOR_RICERCA) = True
        ApriElencoFor2()
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub

    Private Sub rbClienteDocumento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbFornitoreDocumento.CheckedChanged
        If rbFornitoreDocumento.Checked Then
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False
        End If
    End Sub

    Private Sub rbDocumento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbDocumento.CheckedChanged
        If rbDocumento.Checked Then
            rbOrdinamentoDataDoc.Enabled = True
            rbOrdinamentoNDoc.Enabled = True
        End If
    End Sub
End Class