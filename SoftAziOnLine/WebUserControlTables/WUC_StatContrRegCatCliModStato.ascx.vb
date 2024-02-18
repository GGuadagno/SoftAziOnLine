Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Partial Public Class WUC_StatContrRegCatCliModStato
    Inherits System.Web.UI.UserControl
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Regioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDa_CatCli.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        '---------------------------------
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Session(CALLGESTIONE) = SWSI
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------
        If (Not IsPostBack) Then
            chkTuttiModelli.Checked = True
            'DDLModello.SelectedIndex = -1
            'DDLModello.Enabled = False
            chkTuttiCategorie.Checked = True

            AbilitaDisabilitaCampiCat(False)
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            '-
            chkTutteRegioni.Checked = True
            ddlRegioni.Enabled = False
            chkTutteProvince.Checked = True
            ddlProvince.Enabled = False
            Session("CodRegione") = 0
            Session(CSTSTATODOC) = "6"
            Session(CSTSTATODOCSEL) = "6"
            SqlDSProvince.DataBind()
            '-----
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        lnkElenco.Visible = False
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim CodRegione As Integer
        Dim Provincia As String
        Dim CodCateg As Integer
        Dim SWRaggrCatCli As Boolean
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtDataDa.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If txtDataA.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data inizio periodo superiore alla data fine periodo"
                    ErroreCampi = True
                End If
            End If
        End If
        If chkTuttiCategorie.Checked = False Then
            If (ddlCatCli.SelectedValue = "" Or ddlCatCli.SelectedIndex = -1) Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare la categoria"
                ErroreCampi = True
            End If
        End If

        If chkTuttiModelli.Checked = True Then
            If chkC1.Checked Or chkC2.Checked Or chkFR2.Checked Or chkFR3.Checked Or chkFRX.Checked Or chkHS1.Checked Or chkMisti.Checked Then
                chkC1.Checked = False
                chkC2.Checked = False : chkFR2.Checked = False : chkFR3.Checked = False : chkFRX.Checked = False : chkHS1.Checked = False
                chkMisti.Checked = False
                'StrErroreCampi = StrErroreCampi & "<BR>- Seleziona tutti i modelli "
                'ErroreCampi = True
            End If
        Else
            If Not chkC1.Checked And Not chkC2.Checked And Not chkFR2.Checked And Not chkFR3.Checked And Not chkFRX.Checked And Not chkHS1.Checked And Not chkMisti.Checked Then
                StrErroreCampi = StrErroreCampi & "<BR>- Selezionare almeno un modello"
                ErroreCampi = True
            End If
        End If
        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(CSTSTATISTICHE) = TIPOELENCOSCATT.StatCMRegPrCCliStato
        If chkTutteRegioni.Checked Then
            CodRegione = -1
        Else
            CodRegione = ddlRegioni.SelectedValue
        End If
        If chkTutteProvince.Checked Then
            Provincia = ""
        Else
            Provincia = ddlProvince.SelectedValue
        End If

        If chkTuttiCategorie.Checked Then
            CodCateg = -1
        Else
            CodCateg = ddlCatCli.SelectedValue
        End If
        If chkRaggrCatCli.Checked Then
            SWRaggrCatCli = True
        Else
            SWRaggrCatCli = False
        End If
        Dim strCategRagg As String = ""
        Dim SWTratt As Integer = 0
        Dim AccorpaCR As Boolean = False
        SWTratt = InStr(ddlCatCli.SelectedItem.Text.Trim, " - ")
        If chkTuttiCategorie.Checked = False And chkRaggrCatCli.Checked = True Then
            If SWTratt = 0 Then
                strCategRagg = ddlCatCli.SelectedItem.Text.Trim
            Else
                'VA BENE ANCHE QUESTA strCategRagg = Mid(ddlCatCli.SelectedItem.Text.Trim, 1, SWTratt - 1)
                strCategRagg = Left(ddlCatCli.SelectedItem.Text.Trim, SWTratt - 1)
            End If
            AccorpaCR = chkAccorpaCC.Checked
        Else
            strCategRagg = ""
            AccorpaCR = False
        End If
        Dim Modello As String = ""
        If chkTuttiModelli.Checked Then
            Modello = "ZZZ"
        ElseIf chkMisti.Checked Then
            Modello = "XXX"
        Else
            If chkC1.Checked = True Then
                Modello += chkC1.Text + ","
            End If
            If chkC2.Checked = True Then
                Modello += chkC2.Text + ","
            End If
            If chkFR2.Checked = True Then
                Modello += chkFR2.Text + ","
            End If
            If chkFR3.Checked = True Then
                Modello += chkFR3.Text + ","
            End If
            If chkFRX.Checked = True Then
                Modello += chkFRX.Text + ","
            End If
            If chkHS1.Checked = True Then
                Modello += chkHS1.Text + ","
            End If
        End If
        Try
            If ClsPrint.StampaContrattiRegPrCatCli(txtDataDa.Text, txtDataA.Text, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, CodRegione, Provincia, CodCateg, strCategRagg, AccorpaCR, "CM", Session(CSTSTATODOC), Modello) Then
                If DsStatVendCliArt1.StatCMRegPrCCliStato.Count > 0 Then
                    '''Session(CSTObjReport) = ObjReport
                    '''Session(CSTDSStatVendCliArt) = DsStatVendCliArt1
                    '''Session(CSTNOBACK) = 0 'giu040512
                    '''Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
                    Session(CSTDsPrinWebDoc) = DsStatVendCliArt1
                    Call OKApriStampa(DsStatVendCliArt1)
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            ModalPopup.Show("Errore in Statistiche.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Private Sub OKApriStampa(ByRef DsPrinWebDoc As DsStatVendCliArt)

        Dim SWTabCliFor As String = ""
        Dim Rpt As Object = Nothing
        Try
            If (DsPrinWebDoc.Tables("StatCMRegPrCCliStato").Rows.Count > 0) Then
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Elenco Contratti", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati estratti.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = "ELENCOCONTRATTI"
        If rbtnPDF.Checked Then
            NomeStampa += ".PDF"
        Else
            NomeStampa += ".XLS"
        End If
        Dim SubDirDOC As String = "Contratti"
        Rpt = New ElencoCMRegCCliStato
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        '
        Dim strDalAl As String = txtDataDa.Text + "_" + txtDataA.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        Session(CSTNOMEPDF) = strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        '---------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            If rbtnPDF.Checked Then
                Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            Else
                Rpt.ExportToDisk(ExportFormatType.ExcelRecord, Trim(stPathReport & Session(CSTNOMEPDF)))
            End If
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            Chiudi("Errore in esporta: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        lnkElenco.HRef = LnkName
        lnkElenco.Visible = True
    End Sub
    Public Sub Chiudi(ByVal strErrore As String)

        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale " & strErrore)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Private Sub chkTuttiCategorie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiCategorie.CheckedChanged
        pulisciCampiCategoria()
        If chkTuttiCategorie.Checked Then
            AbilitaDisabilitaCampiCat(False)
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        Else
            AbilitaDisabilitaCampiCat(True)
            chkRaggrCatCli.Enabled = True
            ddlCatCli.Focus()
        End If
        lnkElenco.Visible = False
    End Sub
    
    Private Sub AbilitaDisabilitaCampiCat(ByVal Abilita As Boolean)
        ddlCatCli.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiCategoria()
        ddlCatCli.SelectedIndex = -1
    End Sub

    Private Sub chkTuttiRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        If chkTutteRegioni.Checked Then
            ddlRegioni.Enabled = False
            Session("CodRegione") = 0 'alb080213
            ddlRegioni.SelectedIndex = -1
            ddlProvince.SelectedIndex = -1
            ddlProvince.Enabled = False
            chkTutteProvince.Checked = True
        Else
            Session("CodRegione") = ddlRegioni.SelectedValue 'alb080213
            ddlRegioni.Enabled = True
            ddlRegioni.Focus()
        End If
        lnkElenco.Visible = False
    End Sub

    Private Sub chkRaggrCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRaggrCatCli.CheckedChanged
        If chkRaggrCatCli.Checked = True Then
            chkAccorpaCC.Enabled = True
        Else
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        End If
        lnkElenco.Visible = False
    End Sub

    Private Sub chkTutteProvince_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteProvince.CheckedChanged
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
        Else
            ddlProvince.Enabled = True
            ddlProvince.Focus()
        End If
        lnkElenco.Visible = False
    End Sub

    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        Session("CodRegione") = ddlRegioni.SelectedValue
        lnkElenco.Visible = False
    End Sub

    Private Sub rbtnDaEvadere_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvadere.CheckedChanged
        lnkElenco.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "0"
        Session(CSTSTATODOCSEL) = "0"
    End Sub
    Private Sub rbtnEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnEvaso.CheckedChanged
        lnkElenco.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "1"
        Session(CSTSTATODOCSEL) = "1"
        lnkElenco.Visible = False
    End Sub
    Private Sub rbtnParzEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnParzEvaso.CheckedChanged
        lnkElenco.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "2"
        Session(CSTSTATODOCSEL) = "2"
        lnkElenco.Visible = False
    End Sub
    Private Sub rbtnDaEvParEv_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvParEv.CheckedChanged
        lnkElenco.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "6"
        Session(CSTSTATODOCSEL) = "6"
        lnkElenco.Visible = False
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        lnkElenco.Visible = False
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        lnkElenco.Visible = False
    End Sub
    '-

    Private Sub chkModelli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkC1.CheckedChanged, _
        chkC2.CheckedChanged, chkFR2.CheckedChanged, chkFR3.CheckedChanged, chkFRX.CheckedChanged, chkHS1.CheckedChanged
        If chkC1.Checked Or chkC2.Checked Or chkFR2.Checked Or chkFR3.Checked Or chkFRX.Checked Or chkHS1.Checked Then
            chkTuttiModelli.Checked = False
            chkMisti.Checked = False
        ElseIf chkMisti.Checked = False Then
            chkTuttiModelli.Checked = True
        Else
            chkTuttiModelli.Checked = False
        End If
        lnkElenco.Visible = False
    End Sub
    Private Sub chkMisti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkMisti.CheckedChanged
        If chkMisti.Checked Then
            chkTuttiModelli.Checked = False
            chkC1.Checked = False
            chkC2.Checked = False : chkFR2.Checked = False : chkFR3.Checked = False : chkFRX.Checked = False : chkHS1.Checked = False
        End If
        lnkElenco.Visible = False
    End Sub
    Private Sub chkTuttiModelli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiModelli.CheckedChanged
        If chkTuttiModelli.Checked Then
            chkC1.Checked = False
            chkC2.Checked = False : chkFR2.Checked = False : chkFR3.Checked = False : chkFRX.Checked = False : chkHS1.Checked = False
            chkMisti.Checked = False
        End If
        lnkElenco.Visible = False
    End Sub

    Private Sub rbtnPDFXLS_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnPDF.CheckedChanged, rbtnXLS.CheckedChanged
        lnkElenco.Visible = False
    End Sub
End Class