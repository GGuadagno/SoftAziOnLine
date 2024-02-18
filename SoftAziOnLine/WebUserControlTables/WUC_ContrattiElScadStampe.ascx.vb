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
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Partial Public Class WUC_ContrattiElScadStampe
    Inherits System.Web.UI.UserControl

    Private aDataViewScAtt As DataView
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ContrattiElScadStampe.aspx?labelForm=Ordinato per articolo/cliente/Responsabile Visita"
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        If (Not IsPostBack) Then
            Session("aDataViewScAtt") = Nothing
            chkRespVisite.AutoPostBack = False
            chkRespVisite.Checked = True
            chkRespVisite.AutoPostBack = True
            DDLRespVisite.Enabled = False
            '-
            chkTuttiArticoli.AutoPostBack = False
            chkTuttiArticoli.Checked = True
            chkTuttiArticoli.AutoPostBack = True
            txtCod1.Enabled = False
            btnCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod2.Enabled = False
            '-
            txtDataDa.Text = Format(Now.Date, FormatoData)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        '--------------------------------------------------
        If Not Session("aDataViewScAtt") Is Nothing Then
            aDataViewScAtt = Session("aDataViewScAtt")
        Else
            If aDataViewScAtt Is Nothing Then
                aDataViewScAtt = New DataView
            End If
        End If
        Session("aDataViewScAtt") = aDataViewScAtt
        '-------------------------------------------------
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If

    End Sub
    Private Sub SetLnk()
        lnkElencoSc.Visible = False
    End Sub
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        SetLnk()
        Dim strFiltroRicerca As String = ""
        If Not IsDate(txtDataDa.Text) Or Not IsDate(txtDataA.Text) Then
            txtDataDa.BackColor = SEGNALA_KO : txtDataA.BackColor = SEGNALA_KO
            txtDataDa.Focus()
            Exit Sub
        ElseIf CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
            txtDataDa.BackColor = SEGNALA_KO : txtDataA.BackColor = SEGNALA_KO
            txtDataDa.Focus()
            Exit Sub
        End If
        txtDataDa.BackColor = SEGNALA_OK : txtDataA.BackColor = SEGNALA_OK
        '-
        Dim strCCausCMDAE As String = "" : Dim strErrore As String = ""
        Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strCCausCMDAE, strErrore)
        If strErrore.Trim <> "" Then
            strCCausCMDAE = ""
            lblTipoContratto.ForeColor = SEGNALA_KO
            lblTipoContratto.Text = "Errore: Non è stata definita la Causale dei Contratti di Manutenzione DAE - Contattare l'Amministratore di sistema"
            Exit Sub
        ElseIf Not IsNumeric(strCCausCMDAE.Trim) Then
            lblTipoContratto.ForeColor = SEGNALA_KO
            lblTipoContratto.Text = "Errore: Non è stata definita la Causale dei Contratti di Manutenzione DAE - Contattare l'Amministratore di sistema"
            Exit Sub
        End If
        If chkRespVisite.Checked = False And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        DDLRespVisite.BackColor = SEGNALA_OK
        If txtCod1.Text.Trim > txtCod2.Text.Trim Then
            txtCod1.BackColor = SEGNALA_KO : txtCod2.BackColor = SEGNALA_KO
            txtCod1.Focus()
            Exit Sub
        ElseIf txtCod2.Text.Trim < txtCod1.Text.Trim Then
            txtCod1.BackColor = SEGNALA_KO : txtCod2.BackColor = SEGNALA_KO
            txtCod1.Focus()
            Exit Sub
        End If
        txtCod1.BackColor = SEGNALA_OK : txtCod2.BackColor = SEGNALA_OK
        '---------
        Dim dsDoc As New DSOrdinatoArtCli
        Dim myCodVisita As String = ""
        Try
            strFiltroRicerca = "Elenco Scadenze Attività non ancora evase nel Periodo: dal " + txtDataDa.Text.Trim + " al " + txtDataA.Text.Trim
            If chkRespVisite.Checked = False And DDLRespVisite.SelectedIndex > 0 Then
                strFiltroRicerca += " - Solo il Resp.Visita: " + DDLRespVisite.SelectedItem.Text.Trim
            End If
            If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
                strFiltroRicerca += " - Articoli dal Codice: " + txtCod1.Text.Trim + " al Codice: " + txtCod2.Text.Trim
            End If
            Try
                Dim ObjDB As New DataBaseUtility
                Dim strSQL As String = ""
                Dim Comodo As String = ""
                dsDoc.TipoContratto.Clear()
                dsDoc.OrdArtCliRespVisite.Clear()
                Try
                    strSQL = "exec get_ElencoScadCliRespVis @DallaData=N'" + txtDataDa.Text.Trim + "'," + _
                    "@AllaData=N'" + txtDataA.Text.Trim + "',@Cod1='" + txtCod1.Text.Trim + "',@Cod2='" + txtCod2.Text.Trim + "'," + _
                    "@RespArea=0,@RespVisite=0,@Causale=" + strCCausCMDAE.Trim + ",@SoloDaEv=1"
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsDoc, "OrdArtCliRespVisite1")
                    '-
                    aDataViewScAtt = New DataView(dsDoc.OrdArtCliRespVisite1)
                    Session("aDataViewScAtt") = aDataViewScAtt
                    '---------------------------------
                    If aDataViewScAtt.Count = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Nessun dato da stampare.", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                    '----
                    strSQL = "Select * From TipoContratto"
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsDoc, "TipoContratto")
                    For Each rsTC In dsDoc.Tables("TipoContratto").Select("")
                        Comodo = IIf(IsDBNull(rsTC!Descrizione), "", rsTC!CodVisita.ToString.Trim)
                        If InStr(myCodVisita, Comodo.Trim) > 0 Then
                        Else
                            myCodVisita += Comodo.Trim + ","
                        End If
                    Next
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento Tabella TipoContratto. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End Try
                ObjDB = Nothing
                '-----------------------------------------------------------------
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento Scasenze. : " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End Try
            '--------
            Dim dc As DataColumn
            If aDataViewScAtt.Count > 0 Then aDataViewScAtt.Sort = "Cod_articolo,Cod_Cliente,DesRespVisite"
            dsDoc.OrdArtCliRespVisite.Clear()
            For i = 0 To aDataViewScAtt.Count - 1
                If chkRespVisite.Checked = False And DDLRespVisite.SelectedIndex > 0 And DDLRespVisite.SelectedItem.Text.Trim <> "" Then
                    If DDLRespVisite.SelectedItem.Text.Trim <> aDataViewScAtt.Item(i)("DesRespVisite").ToString.Trim Then
                        Continue For
                    End If
                End If
                If chkEscludiVERDAE.Checked Then
                    If InStr(myCodVisita, aDataViewScAtt.Item(i)("Cod_Articolo").ToString.Trim) > 0 Then
                        Continue For
                    End If
                End If
                '---------
                Dim newRow As DSOrdinatoArtCli.OrdArtCliRespVisiteRow = dsDoc.OrdArtCliRespVisite.NewOrdArtCliRespVisiteRow
                newRow.BeginEdit()
                For Each dc In dsDoc.OrdArtCliRespVisite.Columns
                    If UCase(dc.ColumnName) = UCase("TitoloReport") Then
                        newRow.Item(dc.ColumnName) = strFiltroRicerca.Trim
                    ElseIf UCase(dc.ColumnName) = UCase("AziendaReport") Then
                        newRow.Item(dc.ColumnName) = strFiltroRicerca.Trim
                    Else
                        If IsDBNull(aDataViewScAtt.Item(i)(dc.ColumnName)) Then
                            newRow.Item(dc.ColumnName) = DBNull.Value
                        Else
                            newRow.Item(dc.ColumnName) = aDataViewScAtt.Item(i)(dc.ColumnName)
                        End If
                    End If
                Next
                '-
                newRow.EndEdit()
                dsDoc.OrdArtCliRespVisite.AddOrdArtCliRespVisiteRow(newRow)
            Next
            dsDoc.OrdArtCliRespVisite.AcceptChanges()
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Seleziona elenco scadenze: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '-
        Session("dsDocElencoScad") = dsDoc
        aDataViewScAtt = New DataView(dsDoc.OrdArtCliRespVisite)
        Session("aDataViewScAtt") = aDataViewScAtt
        '---------------------------------
        If aDataViewScAtt.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        
        SetLnk()
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ElArtCliRespVis
        Session(CSTTASTOST) = ""
        Dim DsPrinWebDoc As New DSOrdinatoArtCli
        DsPrinWebDoc = Session("dsDocElencoScad")

        strErrore = ""
        Session(CSTDsPrinWebDoc) = DsPrinWebDoc
        Try
            Session(CSTNOBACK) = 0
            Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=Ordinato per articolo/cliente/Responsabile Visita")
            ' ''Dim SWSviluppo As String = ConfigurationManager.AppSettings("sviluppo")
            ' ''If Not String.IsNullOrEmpty(SWSviluppo) Then
            ' ''    If SWSviluppo.Trim.ToUpper = "TRUE" Then
            ' ''        Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=Ordinato per articolo/cliente/Responsabile Visita")
            ' ''        Exit Sub
            ' ''    End If
            ' ''End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'se dovesse servire in PDF o XLS
        ' ''Call OKApriStampaElScadCA(DsPrinWebDoc)
    End Sub
    Private Sub OKApriStampaElScadCA(ByRef DsPrinWebDoc As DSOrdinatoArtCli)

        Dim SWTabCliFor As String = ""
        Dim Rpt As Object = Nothing
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = ""
        Dim SubDirDOC As String = "Contratti"
        If chkVisElenco.Checked Then
            NomeStampa = "ELENCOARTCLIRESPVIS.PDF"
        Else
            NomeStampa = "ELENCOARTCLIRESPVIS.XLS"
        End If
        '-
        Rpt = New StOrdArtCliRespVisite 'Contratti
        'ok
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'Utente
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Sub
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim
        'riporto il nome del RESP.AREA nel nome del file creato
        Dim strRespVisita As String = DDLRespVisite.SelectedItem.Text.Trim
        strRespVisita = strRespVisita.ToString.Replace(",", " ")
        strRespVisita = strRespVisita.ToString.Replace(".", " ")
        Dim strDalAl As String = txtDataDa.Text + "_" + txtDataA.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        If strRespVisita.Trim <> "" Then
            Session(CSTNOMEPDF) = strRespVisita.Trim & "_" & strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        Else
            Session(CSTNOMEPDF) = strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        End If
        '---------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            If chkVisElenco.Checked Then
                Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            Else
                Rpt.ExportToDisk(ExportFormatType.ExcelRecord, Trim(stPathReport & Session(CSTNOMEPDF)))
            End If
            '-
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
        lnkElencoSc.Visible = True

        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        lnkElencoSc.HRef = LnkName
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub


    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
        If txtCod2.Text.Trim = "" Then
            txtCod2.Text = txtCod1.Text
            txtDesc2.Text = txtDesc1.Text
        End If
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub


    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampiArticolo()
        If chkTuttiArticoli.Checked Then
            AbilitaDisabilitaCampiArticolo(False)
        Else
            AbilitaDisabilitaCampiArticolo(True)
            txtCod1.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiArticolo(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
    End Sub
   
    Private Sub pulisciCampiArticolo()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
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
       
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            If txtCod2.Text.Trim = "" Then
                txtCod2.Text = txtCod1.Text
                txtDesc2.Text = txtDesc1.Text
            End If
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = txtCod1.Text
                    txtDesc2.Text = txtDesc1.Text
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = txtCod1.Text
                    txtDesc2.Text = txtDesc1.Text
                End If
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            If txtCod2.Text.Trim = "" Then
                txtCod2.Text = txtCod1.Text
                txtDesc2.Text = txtDesc1.Text
            End If
        End If
    End Sub
    
    Private Sub chkRespVisite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRespVisite.CheckedChanged
        If chkRespVisite.Checked = True Then
            DDLRespVisite.Enabled = False : DDLRespVisite.SelectedIndex = 0
            DDLRespVisite.BackColor = SEGNALA_OK
        Else
            DDLRespVisite.Enabled = True
        End If
        If chkRespVisite.Checked = False Then
            DDLRespVisite.Focus()
        End If
    End Sub
End Class