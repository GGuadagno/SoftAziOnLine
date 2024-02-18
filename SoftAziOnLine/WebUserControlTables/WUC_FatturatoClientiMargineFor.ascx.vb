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
Partial Public Class WUC_FatturatoClientiMargineFor
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        If (Not IsPostBack) Then
            'giu180522
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            '---------
            txtDesc1.Enabled = False
            txtCod1.Enabled = True
            btnCercaAnagrafica1.Enabled = True
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me

        WFP_ElencoCliForn1.WucElement = Me
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If

    End Sub

    'giu300122
    Private Sub chkStampaSintetica_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkStampaSintetica.CheckedChanged
        If chkStampaSintetica.Checked = True Then
            DDlRegioniMarg.Enabled = False : DDlRegioniMarg.SelectedIndex = 0
            ddlAgentiMarg.Enabled = False : ddlAgentiMarg.SelectedIndex = 0
            chkAgentiMarg.Enabled = False : chkAgentiMarg.Checked = False
            chkRegioniMarg.Enabled = False : chkRegioniMarg.Checked = False
        Else
            DDlRegioniMarg.Enabled = False
            ddlAgentiMarg.Enabled = False
            chkAgentiMarg.Enabled = True : chkAgentiMarg.Checked = False
            chkRegioniMarg.Enabled = True : chkRegioniMarg.Checked = False
        End If
       
    End Sub

    Private Sub rbMargine_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbClienteDocumentoMarFF.CheckedChanged, rbClienteDocumentoMarMP.CheckedChanged

        If chkStampaSintetica.Checked = False And (rbClienteDocumentoMarFF.Checked Or rbClienteDocumentoMarMP.Checked) Then
            ddlAgentiMarg.Enabled = chkAgentiMarg.Checked : chkAgentiMarg.Enabled = True
            DDlRegioniMarg.Enabled = chkRegioniMarg.Checked : chkRegioniMarg.Enabled = True
        End If
    End Sub
    Private Sub chkAgentiMarg_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAgentiMarg.CheckedChanged
        ddlAgentiMarg.Enabled = chkAgentiMarg.Checked
        If chkAgentiMarg.Checked = True Then chkRegioniMarg.Checked = Not chkAgentiMarg.Checked
        If chkAgentiMarg.Checked = True Then DDlRegioniMarg.Enabled = Not chkAgentiMarg.Checked
    End Sub
    Private Sub chkRegioniMarg_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRegioniMarg.CheckedChanged
        DDlRegioniMarg.Enabled = chkRegioniMarg.Checked
        If chkRegioniMarg.Checked = True Then chkAgentiMarg.Checked = Not chkRegioniMarg.Checked
        If chkRegioniMarg.Checked = True Then ddlAgentiMarg.Enabled = Not chkRegioniMarg.Checked
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim dsFatturatoClienteFattura1 As New DSFatturatoClienteFattura
        Dim ObjReport As New Object
        Dim ClsPrint As New Fatturato
        Dim StrErrore As String = ""
        Dim CodFor As String = "-1"
        Dim TitoloRpt As String = ""
        'GIU180523
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        If Not IsDate(txtDataDa.Text) Or txtDataDa.Text.Length <> 10 Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If Not IsDate(txtDataA.Text) Or txtDataA.Text.Length<>10 Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If
        If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
            If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                StrErroreCampi = StrErroreCampi & "<BR>- data inizio periodo superiore alla data fine periodo"
                ErroreCampi = True
            End If
        Else
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio/fine periodo"
            ErroreCampi = True
        End If
        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '------------------------------------
        If txtCod1.Text.Trim = "" Or txtDesc1.Text.Trim = "" Then
            CodFor = "-1"
        Else
            CodFor = txtCod1.Text.Trim
        End If
        '-
        Session("FatturatoClientiMargineFor") = SWSI
        Try
            If rbClienteDocumentoMarMP.Checked Then
                TitoloRpt = "Fatturato per Cliente/N°Doc. (con Margine - Costo medio ponderato)"
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargMP
            ElseIf rbClienteDocumentoMarFF.Checked Then
                TitoloRpt = "Fatturato per Cliente/N°Doc. (con Margine - Costo FIFO)"
                Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF
            End If
            If CodFor = "-1" Then
                TitoloRpt += " - Articoli di Tutti i Fornitori"
            ElseIf CodFor <> "" Then
                TitoloRpt += " - Articoli del Fornitore: " + txtDesc1.Text.Trim
            End If
            TitoloRpt += IIf(chkAgentiMarg.Checked, " - Raggrruppato per Agente: " + ddlAgentiMarg.SelectedItem.Text.Trim, "") _
                & IIf(chkRegioniMarg.Checked, " - Raggrruppato per Regione: " + DDlRegioniMarg.SelectedItem.Text.Trim, "")
            Session("FatturatoClientiMargineFor") = SWSI
            Session("FatturatoClientiMargineAGE") = SWNO
            Session("FatturatoClientiMargineREG") = SWNO
            If chkAgentiMarg.Checked Then
                Session("FatturatoClientiMargineAGE") = SWSI
            End If
            If chkRegioniMarg.Checked Then
                Session("FatturatoClientiMargineREG") = SWSI
            End If
            TitoloRpt += " (Dal " + txtDataDa.Text.Trim + " al " + txtDataA.Text.Trim + ")"
            If ClsPrint.StampaFatturatoClienteFattura(Session(CSTAZIENDARPT), TitoloRpt, dsFatturatoClienteFattura1, ObjReport, StrErrore, "", "", _
                    CodFor, IIf(chkRegioniMarg.Checked, DDlRegioniMarg.SelectedValue, -1), _
                    IIf(chkAgentiMarg.Checked, ddlAgentiMarg.SelectedValue, -1), txtDataDa.Text, txtDataA.Text) Then

                If dsFatturatoClienteFattura1.FatturatoClienteFattura.Count > 0 Then
                    If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Or _
                        Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargMP Then
                        If SettaMargine(dsFatturatoClienteFattura1, StrErrore) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "Calcolo Margine: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    End If
                    '- OK 
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = dsFatturatoClienteFattura1
                    Session(CSTNOBACK) = 0 'giu040512
                    If chkStampaSintetica.Checked = True Then
                        Session("FatturatoClientiMargineFor") = "S"
                    End If
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
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Fatturato.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    'giu181018
    Private Function SettaMargine(ByRef DSFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef _StrErrore As String) As Boolean
        SettaMargine = False
        Try
            Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim DsMagazzino1 As New DsMagazzino
            Dim rowGiac As DsMagazzino.GiacInizioAnnoRow
            Dim rowMovMag() As DsMagazzino.MovMagValorizzazioneRow
            Dim ClsPrint As New Magazzino
            Dim Selezione As String = ""
            Dim ConnessioneAzi As SqlConnection
            ConnessioneAzi = New SqlConnection
            ConnessioneAzi.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
            'FILTRO ARTICOLI 
            Dim strArticoli As String = "" : Dim myCodArt As String = ""
            Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteFatturaRow
            For Each r_OrdPerCli In DSFatturatoClienteFattura1.FatturatoClienteFattura.Select("", "Cod_Articolo")
                If Not r_OrdPerCli.IsCod_ArticoloNull Then
                    If r_OrdPerCli.Cod_Articolo.Trim <> "" Then
                        If myCodArt <> r_OrdPerCli.Cod_Articolo.Trim Then
                            myCodArt = r_OrdPerCli.Cod_Articolo.Trim
                            If strArticoli.Trim <> "" Then strArticoli += ";"
                            strArticoli += r_OrdPerCli.Cod_Articolo.Trim
                        End If
                    End If
                End If
                r_OrdPerCli.CostoVenduto = 0
            Next
            DSFatturatoClienteFattura1.AcceptChanges()
            If strArticoli.Trim <> "" Then
                Dim arrCodArt As String() = strArticoli.Split(";")
                Selezione += " WHERE Cod_Articolo IN ("
                For i = 0 To arrCodArt.Count - 1
                    If arrCodArt(i).ToString <> "" Then
                        Selezione += "'" & arrCodArt(i).ToString & "',"
                    End If
                Next
                Selezione = Selezione.Substring(0, Selezione.Length - 1) 'rimuovo ultima virgola
                Selezione += ") "
            End If
            '---
            'FIFO
            Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
            If ClsPrint.ValMagFIFO_CostoVendutoFIFO("", "", "", Selezione, DsMagazzino1, _StrErrore, False, True, False, False, ConnessioneAzi) Then
                If DsMagazzino1.MovMagValorizzazione.Count > 0 Then
                    DsMagazzino1.GiacInizioAnno.Clear()
                    For Each rowVal As DsMagazzino.MovMagValorizzazioneRow In DsMagazzino1.MovMagValorizzazione.Select("Segno_Giacenza='+'")
                        'COSTO MEDIO PONDERATO
                        If DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo) Is Nothing Then
                            rowGiac = DsMagazzino1.GiacInizioAnno.NewGiacInizioAnnoRow
                            With rowGiac
                                .Cod_Articolo = rowVal.Cod_Articolo
                                .Qta_Rimanente = rowVal.QtaCS
                                .ImportoTotale = rowVal.Prezzo_Netto * rowVal.QtaCS
                                If .Qta_Rimanente > 0 Then
                                    .ImportoUnita = .ImportoTotale / .Qta_Rimanente
                                End If
                                .EndEdit()
                            End With
                            DsMagazzino1.GiacInizioAnno.AddGiacInizioAnnoRow(rowGiac)
                        Else
                            rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo)
                            With rowGiac
                                .BeginEdit()
                                .Qta_Rimanente = .Qta_Rimanente + rowVal.QtaCS
                                .ImportoTotale = .ImportoTotale + rowVal.Prezzo_Netto * rowVal.QtaCS
                                If .Qta_Rimanente > 0 Then
                                    .ImportoUnita = .ImportoTotale / .Qta_Rimanente
                                End If
                                .EndEdit()
                            End With
                        End If
                    Next
                    DsMagazzino1.AcceptChanges()
                    'OK RIPORTO IL COSTO NEL DS PER LA STAMPA
                    For Each r_OrdPerCli In DSFatturatoClienteFattura1.FatturatoClienteFattura
                        If r_OrdPerCli.Acconto = True Then
                            r_OrdPerCli.Riferimento += " *** FT.ACCONTO/SALDO ***"
                            r_OrdPerCli.Importo = 0
                            r_OrdPerCli.CostoVenduto = 0
                            r_OrdPerCli.Qta_Evasa = 0
                        End If
                        If Not r_OrdPerCli.IsCod_ArticoloNull Then
                            If r_OrdPerCli.Cod_Articolo.Trim <> "" Then
                                If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargMP Then
                                    If Not DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(r_OrdPerCli.Cod_Articolo.Trim) Is Nothing Then
                                        rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(r_OrdPerCli.Cod_Articolo.Trim)
                                        r_OrdPerCli.CostoVenduto = rowGiac.ImportoUnita * r_OrdPerCli.Qta_Evasa
                                    End If
                                End If
                                If Session(CSTFATTURATO) = TIPOSTAMPAFATTURATO.FattOrdineSortClienteNDocMargFF Then
                                    rowMovMag = DsMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & r_OrdPerCli.Cod_Articolo.Trim & "' AND Segno_Giacenza='+' AND Data_Doc<='" & CDate(r_OrdPerCli.Data_Doc) & "' AND Prezzo_Netto<>0")
                                    If rowMovMag.Count > 0 Then
                                        r_OrdPerCli.CostoVenduto = rowMovMag(0).Prezzo_Netto * r_OrdPerCli.Qta_Evasa
                                    End If
                                End If
                            ElseIf r_OrdPerCli.Importo < 0 Or r_OrdPerCli.DedPerAcconto = True Then 'giu070320 DEDUZIONE
                                r_OrdPerCli.Descrizione += " ***DEDUZIONE***"
                                r_OrdPerCli.Importo = 0
                                r_OrdPerCli.CostoVenduto = 0
                                r_OrdPerCli.Qta_Evasa = 0
                            End If
                        ElseIf r_OrdPerCli.Importo < 0 Or r_OrdPerCli.DedPerAcconto = True Then 'giu070320 DEDUZIONE
                            r_OrdPerCli.Descrizione += " ***DEDUZIONE***"
                            r_OrdPerCli.Importo = 0
                            r_OrdPerCli.CostoVenduto = 0
                            r_OrdPerCli.Qta_Evasa = 0
                        End If
                    Next
                    DSFatturatoClienteFattura1.AcceptChanges()
                Else
                    ' ''_StrErrore.Trim = "Attenzione, Nessun articolo selezionato."
                    ' ''Exit Function
                End If
                SettaMargine = True
            Else
                ' ''_StrErrore = _StrErrore.Trim
            End If
            '------------------
            'giu14042020 cancello tutte le righe vuote per per il fornitore -1 SCONOSCIUTO
            For Each r_OrdPerCli In DSFatturatoClienteFattura1.FatturatoClienteFattura.Select("Qta_Evasa=0")
                r_OrdPerCli.Delete()
            Next
            DSFatturatoClienteFattura1.AcceptChanges()
        Catch ex As Exception
            _StrErrore = ex.Message.Trim
        End Try
    End Function

    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        btnCercaAnagrafica1.Enabled = Abilita
        txtDesc1.Enabled = Not (Abilita)
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
        txtDesc1.Text = ""
    End Sub

    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()

    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(F_ELENCO_CLIFORN_APERTA) = True Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                txtCod1.Text = codice
                txtDesc1.Text = descrizione
            End If
        End If
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

    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub

    
End Class