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
Partial Public Class WUC_FattCliDocAG
    Inherits System.Web.UI.UserControl

    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub
    Private Sub Richiamami()
        Dim strRitorno As String = ""
        strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per cliente/documento"
        If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDoc Or _
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DiffFTDTSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Differenze Fatture/N.C. con DDT"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.DTFTDoppiSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=DDT Fatturati in Fatture diverse"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per cliente/N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocAG Then
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per N°documento per agente"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocReg Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatturato per documento"
        ElseIf Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FTNCCCausErrSintOrdineSortByNDoc Then
            strRitorno = "WF_FatturatoClienteDocumento.aspx?labelForm=Fatture/N.C. con Codice Causale errata"
        Else
            strRitorno = "WF_FatturatoClienteDocumentoAG.aspx?labelForm=Fatturato per cliente/documento"
        End If

        Try
            Response.Redirect(strRitorno)
            Exit Sub
        Catch ex As Exception
            Response.Redirect(strRitorno)
            Exit Sub
        End Try
    End Sub

    Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
        Richiamami()
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        If (Not IsPostBack) Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            Try
                rbClienteDocumento.Checked = False
                rbDocumento.Checked = True
                rbSinteticoDoc.Checked = False
                chkRegioni.Checked = False : ddlRegioni.Enabled = False
                rbOrdinamentoDataDoc.Enabled = True
                rbOrdinamentoNDoc.Enabled = True
                rbOrdinamentoNDoc.Checked = True
            Catch ex As Exception

            End Try

        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If

    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
        Dim ObjReport As New Object
        Dim ClsPrint As New Fatturato
        Dim StrErrore As String = ""
        ' ''Dim Ordinamento As String
        Dim cod1 As String = txtCod1.Text.Trim
        Dim cod2 As String = txtCod1.Text.Trim

        Dim TitoloRpt As String = ""
        Dim CodAgente As Integer

        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtCod1.Text.Trim <> "" And txtDesc1.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice Cliente errato.", WUC_ModalPopup.TYPE_ALERT)
            txtCod1.BackColor = SEGNALA_KO
            txtCod1.Focus()
            Exit Sub
        End If

        If chkTuttiAgenti.Checked Then
            CodAgente = -1
        Else
            CodAgente = ddlAgenti.SelectedValue
        End If

        Try
            Dim SWSintetico As Boolean = False
            If rbClienteDocumento.Checked Then
                TitoloRpt = "Fatturato per cliente/N° documento per agente"
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocAG
            ElseIf rbDocumento.Checked Then
                TitoloRpt = "Fattura"
                If rbOrdinamentoNDoc.Checked Then
                    TitoloRpt = "Fatture (Ordinato per N° documento) per agente"
                    Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByNDocAG
                ElseIf rbOrdinamentoDataDoc.Checked Then
                    TitoloRpt = "Fatture (Ordinato per Data documento) per agente"
                    Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortByDataDocAG
                End If
            ElseIf rbSinteticoDoc.Checked Then
                TitoloRpt = "Fatturato per cliente sintetico (Ordinato per N° documento) per agente"
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattSintOrdineSortByNDocAG
                SWSintetico = True
            End If

            If SWSintetico = False Then
                If ClsPrint.StampaFatturatoClienteFatturaAG(Session(CSTAZIENDARPT), TitoloRpt, dsFatturatoClienteFattura1, ObjReport, StrErrore, cod1, cod2, CodAgente) Then
                    If dsFatturatoClienteFattura1.FatturatoClienteFattura.Count > 0 Then
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDsPrinWebDoc) = dsFatturatoClienteFattura1
                        Session(CSTNOBACK) = 0 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebFatturato.aspx")
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
            Else
                If rbSinteticoDoc.Checked Then
                    TitoloRpt += " - Regione: " + ddlRegioni.SelectedItem.Text.Trim
                    If ClsPrint.StampaFatturatoClienteSinteticoAG(Session(CSTAZIENDARPT), TitoloRpt, dsFatturatoClienteFattura1, ObjReport, StrErrore, cod1, cod2, CodAgente, IIf(chkRegioni.Checked, ddlRegioni.SelectedValue, -1)) Then
                        If dsFatturatoClienteFattura1.FatturatoClienteSintetico.Count > 0 Then
                            Session(CSTObjReport) = ObjReport
                            Session(CSTDsPrinWebDoc) = dsFatturatoClienteFattura1
                            Session(CSTNOBACK) = 0 'giu040512
                            Response.Redirect("..\WebFormTables\WF_PrintWebFatturato.aspx")
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
                End If
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Fatturato.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        End Try
    End Sub
  
    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click
        Try
            ApriElencoClienti1()
        Catch ex As Exception
            txtCod1.BackColor = SEGNALA_KO
        End Try
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        Try
            Session(F_CLI_RICERCA) = False
            Session(F_FOR_RICERCA) = False
            txtCod1.AutoPostBack = False
            txtCod1.Text = codice
            txtDesc1.Text = descrizione
            If txtCod1.Text.Trim <> "" And txtDesc1.Text.Trim = "" Then
                txtCod1.BackColor = SEGNALA_KO
            Else
                txtCod1.BackColor = SEGNALA_OK
            End If
            txtCod1.AutoPostBack = True

            Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
            Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
            Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
            Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
        Catch ex As Exception
            txtCod1.BackColor = SEGNALA_KO
        End Try

    End Sub
    Private Sub ApriElencoClienti1()
        Session(F_CLI_RICERCA) = True
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        Try
            txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.CLIENTI, Session(ESERCIZIO))
            If txtCod1.Text.Trim <> "" And txtDesc1.Text.Trim = "" Then
                txtCod1.BackColor = SEGNALA_KO
            Else
                txtCod1.BackColor = SEGNALA_OK
            End If
        Catch ex As Exception
            txtCod1.BackColor = SEGNALA_KO
        End Try

    End Sub

    Private Sub rbClienteDocumento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbClienteDocumento.CheckedChanged
        If rbClienteDocumento.Checked Then
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False
            ddlRegioni.Enabled = False : chkRegioni.Enabled = False
        End If
    End Sub

    Private Sub rbDocumento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbDocumento.CheckedChanged
        If rbDocumento.Checked Then
            rbOrdinamentoDataDoc.Enabled = True
            rbOrdinamentoNDoc.Enabled = True
            ddlRegioni.Enabled = False : chkRegioni.Enabled = False
        End If
    End Sub

    Private Sub rbSinteticoDoc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbSinteticoDoc.CheckedChanged
        If rbSinteticoDoc.Checked = True Then
            rbOrdinamentoNDoc.Checked = True
            rbOrdinamentoDataDoc.Enabled = False
            rbOrdinamentoNDoc.Enabled = False
            ddlRegioni.Enabled = False : chkRegioni.Enabled = True
        End If
    End Sub

    Private Sub chkRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRegioni.CheckedChanged
        If chkRegioni.Checked Then
            ddlRegioni.Enabled = True
        Else
            ddlRegioni.Enabled = False
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