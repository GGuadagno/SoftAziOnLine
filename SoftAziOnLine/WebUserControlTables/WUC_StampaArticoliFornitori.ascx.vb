'GIU190717
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_StampaArticoliFornitori
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Nel caso si vuole gestire tutti commentare le successive righe
        chkTuttiFornitori.Checked = False
        chkTuttiFornitori.Visible = False
        'giu010221 richiesta Zibordi
        Try
            Dim mylabelForm As String = Request.QueryString("labelForm")
            If InStr(mylabelForm.Trim.ToUpper, "CONFRONTO") > 0 Then
                ' ''lblConfronta.Visible = True
                ' ''txtData.Visible = True
                ' ''imgBtnShowCalendar.Visible = True
                lblConfronta.Visible = False
                txtData.Visible = False
                imgBtnShowCalendar.Visible = False
                '-
                rbtnDescrizione.Visible = False
            Else
                lblConfronta.Visible = False
                txtData.Visible = False
                imgBtnShowCalendar.Visible = False
                rbtnDescrizione.Visible = True
            End If
            
        Catch ex As Exception
            lblConfronta.Visible = False
            txtData.Visible = False
            imgBtnShowCalendar.Visible = False
        End Try
        '----------------------------
        AbilitaDisabilitaCampiFor(True)
        '--------------------------------------------------------------
        If (Not IsPostBack) Then
            rbtnCodice.Checked = True
            'Nel caso si vuole gestire tutti NON commentare le successive righe
            'chkTuttiFornitori.Checked = True
            'AbilitaDisabilitaCampiFor(False)
            '------------------------------------------------------------------
            ' ''Dim myAllaData As Date = Now.Date
            ' ''myAllaData = DateAdd(DateInterval.Month, -1, myAllaData)
            ' ''txtData.Text = myAllaData.Date.ToString("dd/MM/yyyy")
            txtData.Text = ""
            '---- giu010221
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If

    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsAnaMag1 As New DSAnaMag
        Dim ObjReport As New Object
        Dim ClsPrint As New Listini
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim Ordinamento As String = "Cod_Articolo"
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If chkTuttiFornitori.Checked = False Then
            If (txtCodFornitore.Text = "" Or txtDescFornitore.Text = "") Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare il fornitore"
                ErroreCampi = True
            End If
        End If

        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        'giu010221 richiesta Zibordi
        Try
            Dim mylabelForm As String = Request.QueryString("labelForm")
            If InStr(mylabelForm.Trim.ToUpper, "CONFRONTO") > 0 Then
                ' ''If Not IsDate(txtData.Text.Trim) And txtData.Visible = True Then
                ' ''    ModalPopup.Show("Attenzione", "Data contronto prezzi obbligatoria o errata.", WUC_ModalPopup.TYPE_ALERT)
                ' ''    Exit Sub
                ' ''End If
                If rbtnCodice.Checked Then
                    Ordinamento = "Cod_Articolo"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCODP
                ElseIf rbtnDescrizione.Checked Then
                    Ordinamento = "Descrizione"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreDESP
                Else
                    Ordinamento = "Cod_Articolo"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCODP
                End If

                Try
                    '" - Confronto prezzi alla data: " & txtData.Text.Trim
                    If ClsPrint.StampaAnaMagForP(Session(CSTAZIENDARPT), "Stampa articoli del Fornitore: " & txtDescFornitore.Text & " ordinato per " & Ordinamento & " - CONFRONTO con Storico Prezzi", DsAnaMag1, ObjReport, StrErrore, txtCodFornitore.Text, txtData.Text.Trim, Ordinamento) Then
                        If DsAnaMag1.AnaMag.Count > 0 Then
                            Session(CSTObjReport) = ObjReport
                            Session(CSTDsPrinWebDoc) = DsAnaMag1
                            Session(CSTNOBACK) = 0 'giu040512
                            Response.Redirect("..\WebFormTables\WF_PrintWebCR_Mag.aspx")
                        Else
                            ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        End If
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore in StampaArticoliFornitori.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                End Try
            Else
                If rbtnCodice.Checked Then
                    Ordinamento = "Cod_Articolo"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCOD
                ElseIf rbtnDescrizione.Checked Then
                    Ordinamento = "Descrizione"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreDES
                Else
                    Ordinamento = "Cod_Articolo"
                    Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliFornitoreCOD
                End If

                Try
                    If ClsPrint.StampaAnaMagFor(Session(CSTAZIENDARPT), "Stampa articoli del Fornitore: " & txtDescFornitore.Text & " ordinato per " & Ordinamento, DsAnaMag1, ObjReport, StrErrore, txtCodFornitore.Text, Ordinamento) Then
                        If DsAnaMag1.AnaMag.Count > 0 Then
                            Session(CSTObjReport) = ObjReport
                            Session(CSTDsPrinWebDoc) = DsAnaMag1
                            Session(CSTNOBACK) = 0 'giu040512
                            Response.Redirect("..\WebFormTables\WF_PrintWebCR_Mag.aspx")
                        Else
                            ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        End If
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore in StampaArticoliFornitori.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                End Try
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StampaArticoliFornitori.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '----------------------------
    End Sub
    '-
#Region " Routine e controlli"
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Dim mylabelForm As String = Request.QueryString("labelForm")
        If InStr(mylabelForm.Trim.ToUpper, "FORNITORI") > 0 Then
            Session(CSTChiamatoDa) = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            Session(CSTChiamatoDa) = "WF_AnagraficaArticoli.aspx?labelForm=Anagrafica articoli"
        End If
        Dim strRitorno As String
        strRitorno = Session(CSTChiamatoDa)
        If IsNothing(strRitorno) Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If
        If String.IsNullOrEmpty(strRitorno) Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If
        Try
            Response.Redirect(strRitorno)
            Exit Sub
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
   
    Private Sub chkTuttiFornitori_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiFornitori.CheckedChanged
        pulisciCampiFornitore()
        If chkTuttiFornitori.Checked Then
            AbilitaDisabilitaCampiFor(False)
        Else
            AbilitaDisabilitaCampiFor(True)
            txtCodFornitore.Focus()
        End If
    End Sub

    Private Sub AbilitaDisabilitaCampiFor(ByVal Abilita As Boolean)
        txtCodFornitore.Enabled = Abilita
        btnFornitore.Enabled = Abilita
        'txtDescFornitore.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiFornitore()
        txtCodFornitore.Text = ""
        txtDescFornitore.Text = ""
    End Sub

    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub
    
    Private Sub ApriElencoFor1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodFornitore.Text = codice
            txtDescFornitore.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub

    Private Sub btnFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFornitore.Click
        Session(F_FOR_RICERCA) = True
        ApriElencoFor1()
    End Sub

#End Region

End Class