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
'Imports Microsoft.Reporting.WebForms
Imports System.IO
Partial Public Class WUC_CambioRespAreaVisiteContratti
    Inherits System.Web.UI.UserControl
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRegPrElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDa_Regioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
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
        If (Not IsPostBack) Then
            Session(IDRESPVISITE) = 0
            ddlRespVisiteOLD.Items.Clear()
            ddlRespVisiteOLD.Items.Add("")
            ddlRespVisiteOLD.DataBind()
            '--
            ddlRespVisiteNEW.Items.Clear()
            ddlRespVisiteNEW.Items.Add("")
            ddlRespVisiteNEW.DataBind()
            '--
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            '-
            chkTutteRegioni.Checked = False
            ddlRegioni.Enabled = True
            chkTutteProvince.Checked = True
            ddlProvince.Enabled = False
            Session("CodRegione") = 0
            Session("CodRespVisiteRegPr") = 0
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
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If IIf(IsNumeric(ddlRespVisiteOLD.SelectedValue), ddlRespVisiteOLD.SelectedValue, 0) = 0 Then
            StrErroreCampi = StrErroreCampi & "<BR>- Selezionare il Responsabile Visita da cambiare"
            ErroreCampi = True
        End If
        If chkTutteRegioni.Checked Then
            'NON CAPITERA'MAI
        Else
            CodRegione = ddlRegioni.SelectedValue
            If CodRegione = 0 Then
                StrErroreCampi = StrErroreCampi & "<BR>- Selezionare una Regione"
                ErroreCampi = True
            End If
        End If
        If txtDataDa.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If txtDataA.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If

        If Not IsDate(txtDataDa.Text) Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If Not IsDate(txtDataA.Text) Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If
        If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
            If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                StrErroreCampi = StrErroreCampi & "<BR>- data inizio periodo superiore alla data fine periodo"
                ErroreCampi = True
            End If
        Else
            StrErroreCampi = StrErroreCampi & "<BR>- date inizio/fine periodo non valide"
            ErroreCampi = True
        End If
        '--
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

        Try
            If ClsPrint.StampaContrattiCambioRespVisite(txtDataDa.Text, txtDataA.Text, Session(CSTAZIENDARPT), DsStatVendCliArt1, StrErrore,
                                                   CodRegione, Provincia, "CM", Session(CSTSTATODOC),
                                                   "", Session(IDRESPVISITE), " da " + ddlRespVisiteOLD.SelectedItem.Text + " a NUOVO " + ddlRespVisiteNEW.SelectedItem.Text) Then
                If DsStatVendCliArt1.StatCMRegPrCCliStato.Count > 0 Then
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
            ModalPopup.Show("Errore in ElencoContrattiCAMBIORespVisite.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
        If rbtnPDF.Checked Then
            getOutputRPT(Rpt, "PDF")
        Else
            getOutputRPT(Rpt, "EXEL")
        End If
        '
        Dim strDalAl As String = txtDataDa.Text + "_" + txtDataA.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        Session(CSTNOMEPDF) = strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        ''''---------
        '''Session(CSTESPORTAPDF) = True
        '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        '''Dim stPathReport As String = Session(CSTPATHPDF)
        '''Try 'giu281112 errore che il file Ã¨ gia aperto
        '''    If rbtnPDF.Checked Then
        '''        Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    Else
        '''        Rpt.ExportToDisk(ExportFormatType.ExcelRecord, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    End If
        '''    'giu140124
        '''    Rpt.Close()
        '''    Rpt.Dispose()
        '''    Rpt = Nothing
        '''    '-
        '''    GC.WaitForPendingFinalizers()
        '''    GC.Collect()
        '''    '-------------
        '''Catch ex As Exception
        '''    Rpt = Nothing
        '''    Chiudi("Errore in esporta: " & Session(CSTNOMEPDF) & " " & ex.Message)
        '''    Exit Sub
        '''End Try
        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''lnkElenco.HRef = LnkName
        lnkElenco.Visible = True
    End Sub
    '@@@@@
    Private Function getOutputRPT(ByVal _Rpt As Object, ByVal _Formato As String) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            If _Formato = "PDF" Then
                myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Else
                myStream = _Rpt.ExportToStream(ExportFormatType.Excel)
            End If
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("WebFormStampe") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            _Rpt.Close()
            _Rpt.Dispose()
            _Rpt = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function
    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    '@@@@@
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
#Region "RBTN"

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
#End Region

    Private Sub rbtnPDFXLS_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnPDF.CheckedChanged, rbtnXLS.CheckedChanged
        lnkElenco.Visible = False
    End Sub

    Private Sub ddlRespVisiteOLD_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRespVisiteOLD.SelectedIndexChanged
        Session(IDRESPVISITE) = 0
        ddlRegioni.SelectedIndex = -1
        ddlProvince.SelectedIndex = -1
        Session("CodRespVisiteRegPr") = 0
        GridViewBody.SelectedIndex = -1
        btnAbbinaRegPr.Enabled = False
        lblMessUtente.Text = ""
        Try
            Session(IDRESPVISITE) = IIf(IsNumeric(ddlRespVisiteOLD.SelectedValue), ddlRespVisiteOLD.SelectedValue, 0)
            If IIf(IsNumeric(ddlRespVisiteOLD.SelectedValue), ddlRespVisiteOLD.SelectedValue, 0) > 0 And
               IIf(IsNumeric(ddlRespVisiteNEW.SelectedValue), ddlRespVisiteNEW.SelectedValue, 0) > 0 Then
                Call CKCodResVisitaOLDNEW()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ddlRespVisiteNEW_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRespVisiteNEW.SelectedIndexChanged
        Try
            If IIf(IsNumeric(ddlRespVisiteOLD.SelectedValue), ddlRespVisiteOLD.SelectedValue, 0) > 0 And
               IIf(IsNumeric(ddlRespVisiteNEW.SelectedValue), ddlRespVisiteNEW.SelectedValue, 0) > 0 Then
                If CKCodResVisitaOLDNEW() = True Then
                    If Session("CodRespVisiteRegPr") > 0 Then
                        btnAbbinaRegPr.Enabled = True
                    Else
                        btnAbbinaRegPr.Enabled = False
                    End If
                Else
                    btnAbbinaRegPr.Enabled = False
                End If
            Else
                btnAbbinaRegPr.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function CKCodResVisitaOLDNEW(Optional ByVal NoMess As Boolean = False) As Boolean
        CKCodResVisitaOLDNEW = True
        Try
            If IIf(IsNumeric(ddlRespVisiteNEW.SelectedValue), ddlRespVisiteNEW.SelectedValue, 0) = Session(IDRESPVISITE) Then
                CKCodResVisitaOLDNEW = False
                btnAbbinaRegPr.Enabled = False
                If NoMess = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("ATTENZIONE", "Responsabile di Visita da cambiare dev'essere diverso dal NUOVO Responsabile Visita", WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
        Catch ex As Exception
            btnAbbinaRegPr.Enabled = False
        End Try

    End Function

    Private Sub btnAbbinaRegPr_Click(sender As Object, e As EventArgs) Handles btnAbbinaRegPr.Click

    End Sub
    Private Function OKExecute(ByVal strSQL As String, ByRef strErrore As String) As Boolean
        strErrore = ""
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL)
            ObjDB = Nothing
            OKExecute = True
        Catch Ex As Exception
            strErrore = Ex.Message.Trim
            OKExecute = False
            Exit Function
        End Try
    End Function
    Protected Sub btnEliminaRegPr_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim Riga As Integer = GridViewBody.SelectedDataKey.Value
            If Riga <> 0 Then
                Dim strSQL As String = "delete from RespVisiteRegPr where codice=" + Riga.ToString.Trim
                Dim strErrore As String = ""
                If OKExecute(strSQL, strErrore) = True Then
                    lblMessUtente.Text = "Abbinamento eliminato"
                ElseIf strErrore.Trim <> "" Then
                    lblMessUtente.Text = strErrore.Trim
                    Exit Sub
                End If
                SqlDSRegPrElenco.DataBind()
                GridViewBody.DataBind()
            End If
        Catch Ex As Exception
            lblMessUtente.Text = "Nessun elemento selezionato. (Elimina)"
            Exit Sub
        End Try
    End Sub
    Private Sub GridViewBody_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridViewBody.SelectedIndexChanged
        Try
            Dim Riga As Integer = GridViewBody.SelectedDataKey.Value
            Session("CodRespVisiteRegPr") = Riga
            If Riga > 0 Then
                If CKCodResVisitaOLDNEW(True) = True And IIf(IsNumeric(ddlRespVisiteNEW.SelectedValue), ddlRespVisiteNEW.SelectedValue, 0) > 0 Then
                    btnAbbinaRegPr.Enabled = True
                Else
                    btnAbbinaRegPr.Enabled = False
                End If
            Else
                lblMessUtente.Text = "Nessun dato selezionato."
                Exit Sub
            End If
            Dim row As GridViewRow = GridViewBody.SelectedRow
            Dim DesRegione As String = NormalizzaStringa(row.Cells(1).Text.Trim)
            Dim DesProvincia As String = NormalizzaStringa(row.Cells(2).Text.Trim)
            Dim CodRegione As String = NormalizzaStringa(row.Cells(3).Text.Trim)
            lblMessUtente.Text = "Regione/Provincia selezionata per l'abbinamento al NUOVO Responsabile Visita: " + DesRegione
            lblMessUtente.Text += IIf(Trim(DesProvincia) <> "", " (" + DesProvincia.Trim + ")", "")
            If IsNumeric(CodRegione) Then
                Session("CodRegione") = CodRegione
                PosizionaItemDDL(CodRegione, ddlRegioni)
            End If
            If DesProvincia.Trim <> "" Then
                PosizionaItemDDL(DesProvincia, ddlProvince)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore seleziona Regione/Provincia", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
End Class