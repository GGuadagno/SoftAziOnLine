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

Partial Public Class WUC_StatVendCliArt
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'GIU051112
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        '-------
        btnControllo.Text = "Stampa di controllo" 'giu151012
        If (Not IsPostBack) Then
            rbtnPrezzoVendita.Checked = True
            rbtnCliente.Checked = True
            chkTuttiArticoli.Checked = True
            chkTuttiClienti.Checked = True
            chkRegioni.Checked = False : ddlRegioni.Enabled = False 'giu051112
            rbtnVenduto.Checked = True
            AbilitaDisabilitaCampiArticolo(False)
            AbilitaDisabilitaCampiCliente(False)
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            txtNCData.Text = "31/12/" & Session(ESERCIZIO)
            ' ''Try
            ' ''    impostaDate()
            ' ''Catch ex As Exception
            ' ''    ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''End Try
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            If Request.QueryString("labelForm") = "Venduto/Fatturato per articolo/cliente" Then
                rbtnArticolo.Checked = True
                rbtnCliente.Checked = False
            Else
                rbtnCliente.Checked = True
                rbtnArticolo.Checked = False
            End If
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
        If Session(F_SEL_ARTICOLO_APERTA) Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub
    Private Sub impostaDate()

        Dim objDB As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim tmpConn As New SqlConnection
        tmpConn.ConnectionString = objDB.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftAzi)
        Dim tmpCommand As New SqlCommand
        tmpCommand.Connection = tmpConn
        tmpCommand.CommandType = CommandType.StoredProcedure
        tmpCommand.CommandText = "get_dateDocPerRiepVendutoFatturato"
        tmpCommand.Parameters.Add("@DaData", SqlDbType.DateTime)
        tmpCommand.Parameters("@DaData").Direction = ParameterDirection.Output
        tmpCommand.Parameters.Add("@AData", SqlDbType.DateTime)
        tmpCommand.Parameters("@AData").Direction = ParameterDirection.Output

        tmpConn.Open()
        tmpCommand.ExecuteNonQuery()

        If Not IsDBNull(tmpCommand.Parameters("@DaData").Value) Then
            txtDataDa.Text = Format(CDate(tmpCommand.Parameters("@DaData").Value), FormatoData)
        End If
        If Not IsDBNull(tmpCommand.Parameters("@AData").Value) Then
            txtDataA.Text = Format(CDate(tmpCommand.Parameters("@AData").Value), FormatoData)
            txtNCData.Text = txtDataA.Text
        End If

        tmpConn.Close()
        tmpConn.Dispose()
        tmpConn = Nothing
        tmpCommand.Dispose()
        tmpCommand = Nothing
        objDB = Nothing

    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim Ordinamento As Integer = 0 'INDICA SE REPORT E' ORDINATO PER CLIENTE/ARTICOLO O ARTICOLO/CLIENTE
        Dim VisualizzaPrezzoVendita As Boolean = False
        Dim Statistica As Integer
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtDataDa.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If txtDataA.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If
        ' ''If rbtnDaFatturare.Checked = True Then
        ' ''    Dim SWAnnoCorr As Boolean = True
        ' ''    If txtDataDa.Text <> "01/01/" & Session(ESERCIZIO) Then
        ' ''        SWAnnoCorr = False
        ' ''    End If
        ' ''    If txtDataA.Text <> "31/12/" & Session(ESERCIZIO) Then
        ' ''        SWAnnoCorr = False
        ' ''    End If
        ' ''    If txtNCData.Text <> "31/12/" & Session(ESERCIZIO) Then
        ' ''        SWAnnoCorr = False
        ' ''    End If
        ' ''    If SWAnnoCorr = False Then
        ' ''        StrErroreCampi = StrErroreCampi & "<BR>- per la stampa 'Da fatturare' si può richiedere solo l'esercizio in corso"
        ' ''        ErroreCampi = True
        ' ''    End If
        ' ''End If
        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data inizio periodo superiore alla data fine periodo"
                    ErroreCampi = True
                End If
            End If
        End If

        If txtDataA.Text <> "" And txtNCData.Text <> "" Then
            If IsDate(txtDataA.Text) And IsDate(txtNCData.Text) Then
                If CDate(txtDataA.Text) > CDate(txtNCData.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data fine periodo superiore alla data fine periodo NC"
                    ErroreCampi = True
                End If
            End If
        End If

        If chkTuttiClienti.Checked = False Then
            If (txtCodCliente.Text = "" Or txtDescCliente.Text = "") Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare il cliente"
                ErroreCampi = True
            End If
        End If

        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text = "" And txtCod2.Text = "" Then
                StrErroreCampi = StrErroreCampi & "<BR>- occorre inserire il codice articolo di inizio o di fine intervallo"
                ErroreCampi = True
            End If
            If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                If txtCod1.Text > txtCod2.Text Then
                    StrErroreCampi = StrErroreCampi & "<BR>- il codice articolo di inizio intervallo è superiore a quello finale"
                    ErroreCampi = True
                End If
            End If
            'Else
            'If txtDesc1.Text <> "" And txtDesc2.Text <> "" Then
            '    If txtDesc1.Text > txtDesc2.Text Then
            '        ModalPopup.Show("Attenzione", "La descrizione di inizio intervallo è superiore a quella finale.", WUC_ModalPopup.TYPE_ALERT)
            '        Exit Sub
            '    End If
            'End If
        End If
        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If txtNCData.Text = "" Then
            txtNCData.Text = txtDataA.Text
        End If

        If rbtnCliente.Checked Then
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticolo  '1 ORDINATO PER CLIENTE / ARTICOLO
            If chkRegioni.Checked = True Then
                Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloREG
            End If
        Else
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloCliente  '2 ORDINATO PER ARTICOLO / CLIENTE 
            If chkRegioni.Checked = True Then
                Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG
            End If
        End If

        VisualizzaPrezzoVendita = rbtnPrezzoVendita.Checked

        If rbtnVenduto.Checked Then
            Statistica = 0  'venduto e fatturato
        ElseIf rbtnFatturato.Checked Then
            Statistica = 1  'fatturato
        ElseIf rbtnDaFatturare.Checked Then
            Statistica = 2  'da fatturare
        Else
            Statistica = 0
        End If
        Dim InvioEmail As Boolean = chkTuttiClientiEmail.Checked
        Dim EscludiCliCateg As Boolean = chkEscludiCliDaCateg.Checked
        Dim EscludiCliNoEmail As Boolean = chkEscludiCliNoInvio.Checked
        If InvioEmail = False Then
            EscludiCliCateg = False
            EscludiCliNoEmail = False
        End If
        Try
            If ClsPrint.StampaStatisticheVendutoArticoloCliente(txtCod1.Text, txtCod2.Text, txtCodCliente.Text, txtDataDa.Text, txtDataA.Text, txtNCData.Text, Session(CSTSTATISTICHE), Statistica, VisualizzaPrezzoVendita, chkRegioni.Checked, ddlRegioni.SelectedValue, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, InvioEmail, EscludiCliCateg, EscludiCliNoEmail) Then
                If DsStatVendCliArt1.StCliArt.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDSStatVendCliArt) = DsStatVendCliArt1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
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

    'giu151012
    Private Sub btnControllo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnControllo.Click
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"

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

        If txtDataA.Text <> "" And txtNCData.Text <> "" Then
            If IsDate(txtDataA.Text) And IsDate(txtNCData.Text) Then
                If CDate(txtDataA.Text) > CDate(txtNCData.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data fine periodo superiore alla data fine periodo NC"
                    ErroreCampi = True
                End If
            End If
        End If
        chkTuttiClienti.Checked = True
        txtCodCliente.Text = "" : txtCodCliente.Enabled = False
        txtDescCliente.Text = "" : txtDescCliente.Enabled = False
        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text = "" And txtCod2.Text = "" Then
                StrErroreCampi = StrErroreCampi & "<BR>- occorre inserire il codice articolo di inizio o di fine intervallo"
                ErroreCampi = True
            End If
            If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                If txtCod1.Text > txtCod2.Text Then
                    StrErroreCampi = StrErroreCampi & "<BR>- il codice articolo di inizio intervallo è superiore a quello finale"
                    ErroreCampi = True
                End If
            End If
        End If
        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If txtNCData.Text = "" Then
            txtNCData.Text = txtDataA.Text
        End If

        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "ElaboraControlloAll1"
        Session(MODALPOPUP_CALLBACK_METHOD) = "ElaboraControlloAll0"
        ModalPopup.Show("Stampa di controllo", "Vuole solo gli articoli con totali non uguali ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub ElaboraControlloAll1()
        OKControllo(True)
    End Sub
    Public Sub ElaboraControlloAll0()
        OKControllo(False)
    End Sub
    Private Sub OKControllo(ByVal SWAll As Boolean)
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim StrErrore As String = ""
        Dim Ordinamento As Integer = 0 'INDICA SE REPORT E' ORDINATO PER CLIENTE/ARTICOLO O ARTICOLO/CLIENTE
        Dim VisualizzaPrezzoVendita As Boolean = False
        Dim Statistica As Integer
        '-
        Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.ControlloVendutoCVByArt
        rbtnPrezzoVendita.Checked = True
        VisualizzaPrezzoVendita = rbtnPrezzoVendita.Checked
        rbtnVenduto.Checked = True
        Statistica = 0  'venduto e fatturato
        Dim InvioEmail As Boolean = chkTuttiClientiEmail.Checked
        Dim EscludiCliCateg As Boolean = chkEscludiCliDaCateg.Checked
        Dim EscludiCliNoEmail As Boolean = chkEscludiCliNoInvio.Checked
        If InvioEmail = False Then
            EscludiCliCateg = False
            EscludiCliNoEmail = False
        End If
        Try
            If ClsPrint.StampaStatisticheVendutoArticoloCliente(txtCod1.Text, txtCod2.Text, txtCodCliente.Text, txtDataDa.Text, txtDataA.Text, txtNCData.Text, Session(CSTSTATISTICHE), Statistica, VisualizzaPrezzoVendita, False, 0, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, InvioEmail, EscludiCliCateg, EscludiCliNoEmail) Then
                If DsStatVendCliArt1.StCliArt.Count > 0 Then
                    If ElaboraCS(DsStatVendCliArt1) = True Then
                        If SWAll = False Then 'SOLO LE DIFFERENZE
                            For Each rowST As DsStatVendCliArt.StCtrArtSTCVRow In DsStatVendCliArt1.StCtrArtSTCV.Rows
                                'If rowST.TotQta_ST = rowST.TotQta_CV And rowST.Imponibile_ST = rowST.Imponibile_CV Then
                                If Math.Round(rowST.Imponibile_ST, 2) = Math.Round(rowST.Imponibile_CV, 2) Then
                                    rowST.Delete()
                                End If
                            Next
                            DsStatVendCliArt1.AcceptChanges()
                        End If
                        Session(CSTObjReport) = ObjReport
                        Session(CSTDSStatVendCliArt) = DsStatVendCliArt1
                        Session(CSTNOBACK) = 1 'giu040512
                        Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
                    Else
                        Exit Sub 'FALLITO ERRORI GIA SEGNALATI NELLA CHIAMATA
                    End If
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ModalPopup.Show("Errore in Statistiche.btnControllo", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Private Function ElaboraCS(ByRef DSStatVendCliArt1 As DsStatVendCliArt) As Boolean
        '----------------------------
        '@@@@@@@@@
        Dim DsMagazzino1 As New DsMagazzino
        Dim ObjReport As New Object
        Dim ClsPrint As New Magazzino
        Dim Errore As Boolean = False
        Dim Selezione As String = ""
        Dim StrErrore As String = ""
        Dim RagrForn As Boolean = False

        If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
            If txtCod2.Text.Trim < txtCod1.Text.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codici articoli incongruenti. <br> (Al codice < di Dal codice).", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        End If
        If Errore Then
            ModalPopup.Show("Controllo dati stampa", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If

        Dim strErroreGiac As String = ""
        Dim SWNegativi As Boolean = False
        If Magazzino.Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then 'giu190613 aggiunto SWNegativi
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If

        'FILTRO DA ARTICOLO 
        If txtCod1.Text <> "" Then
            Selezione = " WHERE Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
        Else
            Selezione = ""
        End If
        'FILTRO A ARTICOLO
        If txtCod2.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            Else
                Selezione = " WHERE Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            End If
        End If
        Selezione += " ORDER BY Cod_Articolo"
        '=======================================================
        '
        Try
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO
            'GIU210920 SU MAGAZZINO 0 PRINCIPALE
            If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Stampa Controllo articoli venduti / fatturati per articolo", "", Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, False, True, 0) Then
                If DsMagazzino1.DispMagazzino.Count > 0 Then
                    'FILTRO VALORIZZAZIONE ALLA DATA
                    Selezione = "WHERE Data_Doc >= CONVERT(DATETIME, '" & Format(CDate(txtDataDa.Text), FormatoData) & "', 103)"
                    Selezione += " AND Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtDataA.Text), FormatoData) & "', 103)"
                    '--------------------------------------------------------------------------------------
                    'FILTRO DA ARTICOLO 
                    If txtCod1.Text <> "" Then
                        Selezione = Selezione & " AND view_MovMagValorizzazioneCS.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                    End If
                    'FILTRO A ARTICOLO
                    If txtCod2.Text <> "" Then
                        Selezione = Selezione & " AND view_MovMagValorizzazioneCS.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                    End If
                    Selezione += " ORDER BY view_MovMagValorizzazioneCS.Cod_Articolo, Data_Doc"
                    If ClsPrint.ValMagFIFO_CostoVendutoFIFO(Session(CSTAZIENDARPT), "Stampa Controllo articoli venduti / fatturati per articolo", "", Selezione, DsMagazzino1, StrErrore, False, False, True, SWNegativi) Then
                        If DsMagazzino1.DispMagazzino.Count > 0 Then
                            Session(CSTDsPrinWebDoc) = DsMagazzino1
                            ' ''Session(CSTNOBACK) = 0 'giu040512
                            ' ''Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            '---
                            Dim rowCTR() As DataRow
                            'VENDUTO ST
                            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                                For Each rowST As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                                    'Aggiorno le Qta del Venduto
                                    If DSStatVendCliArt1.StCtrArtSTCV.Select("Cod_Articolo='" & Replace(rowST.Cod_Articolo.Trim, "'", "''") & "'").Length > 0 Then
                                        rowCTR = DSStatVendCliArt1.StCtrArtSTCV.Select("Cod_Articolo='" & Replace(rowST.Cod_Articolo.Trim, "'", "''") & "'")
                                        rowCTR(0).BeginEdit()
                                        rowCTR(0).Item("TotQta_ST") += rowST.TotQta
                                        rowCTR(0).Item("Imponibile_ST") += rowST.Imponibile
                                        rowCTR(0).EndEdit()
                                    Else
                                        Dim newRowArt As DsStatVendCliArt.StCtrArtSTCVRow = DSStatVendCliArt1.StCtrArtSTCV.NewStCtrArtSTCVRow
                                        With newRowArt
                                            .BeginEdit()
                                            .Azienda = Session(CSTAZIENDARPT)
                                            .TitoloReport = "Stampa Controllo articoli venduti dal " & txtDataDa.Text & " al " & txtDataA.Text '& " con N.C. al " & txtDataNC
                                            .Cod_Articolo = rowST.Cod_Articolo
                                            .Descrizione = rowST.Descrizione
                                            '
                                            .TotQta_ST = rowST.TotQta
                                            .Imponibile_ST = rowST.Imponibile
                                            '----------
                                            .EndEdit()
                                        End With
                                        DSStatVendCliArt1.StCtrArtSTCV.AddStCtrArtSTCVRow(newRowArt)
                                        newRowArt = Nothing
                                    End If
                                Next
                            End If
                            '' ''----
                            DSStatVendCliArt1.AcceptChanges()
                            'VENDUTO CS
                            Dim myQta As Decimal = 0 : Dim myImporto As Decimal = 0
                            If DsMagazzino1.DispMagazzino.Count > 0 Then
                                For Each rowCS As DsMagazzino.MovMagValorizzazioneRow In DsMagazzino1.MovMagValorizzazione.Rows
                                    'QUANTITA come da REPORT ValMagCostoVenduto.Rpt
                                    If rowCS.Segno_Giacenza = "-" Then
                                        If rowCS.CausCostoVenduto = True Then
                                            If rowCS.Tipo_Doc = "FC" And rowCS.CDeposito = False And rowCS.CVisione = False Then
                                                myQta = 0
                                            Else
                                                'per le CESSIONI GRATUITE
                                                If rowCS.Fatturabile = False Then
                                                    myQta = 0
                                                Else
                                                    myQta = rowCS.Qta_Evasa * -1
                                                End If
                                                '------------------------
                                            End If
                                        Else
                                            myQta = 0
                                        End If
                                    Else
                                        If rowCS.Reso = True And rowCS.Tipo_Doc <> "NC" Then
                                            myQta = rowCS.Qta_Evasa * -1
                                        Else
                                            myQta = 0
                                        End If
                                    End If
                                    'IMPORTO come da REPORT ValMagCostoVenduto.Rpt
                                    If rowCS.Segno_Giacenza = "-" Then
                                        If rowCS.CausCostoVenduto = True Then
                                            'per le CESSIONI GRATUITE
                                            If rowCS.Fatturabile = False Then
                                                myImporto = 0
                                            Else
                                                myImporto = (rowCS.Qta_Evasa * -1) * rowCS.Prezzo_Netto
                                            End If
                                            '' ' myImporto = (rowCS.Qta_Evasa * -1) * rowCS.Prezzo_Netto
                                            '------------------------
                                        Else
                                            myImporto = 0
                                        End If
                                    Else
                                        If rowCS.Tipo_Doc = "NC" Then
                                            myImporto = (rowCS.Qta_Evasa * -1) * rowCS.Prezzo_Netto
                                        Else
                                            myImporto = 0
                                        End If
                                    End If
                                    'Aggiorno le Qta del Costo del Venduto
                                    If DSStatVendCliArt1.StCtrArtSTCV.Select("Cod_Articolo='" & Replace(rowCS.Cod_Articolo.Trim, "'", "''") & "'").Length > 0 Then
                                        rowCTR = DSStatVendCliArt1.StCtrArtSTCV.Select("Cod_Articolo='" & Replace(rowCS.Cod_Articolo.Trim, "'", "''") & "'")
                                        rowCTR(0).BeginEdit()
                                        rowCTR(0).Item("TotQta_CV") += myQta
                                        rowCTR(0).Item("Imponibile_CV") += myImporto
                                        rowCTR(0).EndEdit()
                                    Else
                                        Dim newRowArt As DsStatVendCliArt.StCtrArtSTCVRow = DSStatVendCliArt1.StCtrArtSTCV.NewStCtrArtSTCVRow
                                        With newRowArt
                                            .BeginEdit()
                                            .Azienda = Session(CSTAZIENDARPT)
                                            .TitoloReport = "Stampa Controllo articoli venduti dal " & txtDataDa.Text & " al " & txtDataA.Text '& " con N.C. al " & txtDataNC
                                            .Cod_Articolo = rowCS.Cod_Articolo
                                            .Descrizione = rowCS.DesArtDocD
                                            '
                                            .TotQta_CV = myQta
                                            .Imponibile_CV = myImporto
                                            '----------
                                            .EndEdit()
                                        End With
                                        DSStatVendCliArt1.StCtrArtSTCV.AddStCtrArtSTCVRow(newRowArt)
                                        newRowArt = Nothing
                                    End If
                                Next
                            End If
                            '' ''----
                            DSStatVendCliArt1.AcceptChanges()
                            Return True
                        Else
                            ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        End If
                    Else
                        ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch ex As Exception
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Function

#Region " Routine e controlli"
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub
    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub
    Private Sub chkTuttiClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampiCliente()
        If chkTuttiClienti.Checked Then
            'giu130619
            chkTuttiClientiEmail.Checked = False
            chkEscludiCliDaCateg.Checked = False
            chkEscludiCliNoInvio.Checked = False
            chkEscludiCliDaCateg.Enabled = False
            chkEscludiCliNoInvio.Enabled = False
            '-
            AbilitaDisabilitaCampiCliente(False)
            If chkRegioni.Checked = False Then 'GIU051112
                btnControllo.Enabled = True
            Else
                btnControllo.Enabled = False
            End If
        Else
            AbilitaDisabilitaCampiCliente(True)
            btnControllo.Enabled = False
            txtCodCliente.Focus()
        End If
        If rbtnVenduto.Checked = False Then
            btnControllo.Enabled = False
        ElseIf rbtnPrezzoVendita.Checked = False Then
            btnControllo.Enabled = False
        ElseIf chkTuttiClienti.Checked = False Then
            btnControllo.Enabled = False
        Else
            If chkRegioni.Checked = False Then 'GIU051112
                btnControllo.Enabled = True
            Else
                btnControllo.Enabled = False
            End If
        End If
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
        'txtDesc1.Enabled = Abilita
        'txtDesc2.Enabled = Abilita
    End Sub
    Private Sub AbilitaDisabilitaCampiCliente(ByVal Abilita As Boolean)
        txtCodCliente.Enabled = Abilita
        btnCliente.Enabled = Abilita
        'txtDescCliente.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiArticolo()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub
    Private Sub pulisciCampiCliente()
        txtCodCliente.Text = ""
        txtDescCliente.Text = ""
    End Sub

    Private Sub txtCodCliente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente.TextChanged
        txtDescCliente.Text = App.GetValoreFromChiave(txtCodCliente.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub
    'GIU300512
    Private Sub txtDataA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataA.TextChanged
        txtNCData.Text = txtDataA.Text
        txtNCData.Focus()
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
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub

    Private Sub ApriElencoClienti1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodCliente.Text = codice
            txtDescCliente.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub

    Private Sub btnCliente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCliente.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()
    End Sub

#End Region

    
    Private Sub rbtnVenduto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnVenduto.CheckedChanged, rbtnFatturato.CheckedChanged, rbtnDaFatturare.CheckedChanged
        If rbtnVenduto.Checked = False Then
            btnControllo.Enabled = False
        ElseIf rbtnPrezzoVendita.Checked = False Then
            btnControllo.Enabled = False
        ElseIf chkTuttiClienti.Checked = False Then
            btnControllo.Enabled = False
        Else
            btnControllo.Enabled = True
        End If
    End Sub

    Private Sub rbtnPrezzoVendita_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnPrezzoVendita.CheckedChanged, rbtnPrezzoListino.CheckedChanged
        If rbtnVenduto.Checked = False Then
            btnControllo.Enabled = False
        ElseIf rbtnPrezzoVendita.Checked = False Then
            btnControllo.Enabled = False
        ElseIf chkTuttiClienti.Checked = False Then
            btnControllo.Enabled = False
        Else
            btnControllo.Enabled = True
        End If
    End Sub

    Private Sub chkRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRegioni.CheckedChanged
        ddlRegioni.Enabled = chkRegioni.Checked
        btnControllo.Enabled = Not chkRegioni.Checked
        If chkTuttiClienti.Checked = False Then
            btnControllo.Enabled = False
        End If
    End Sub

    Private Sub chkTuttiClientiEmail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClientiEmail.CheckedChanged
        If chkTuttiClientiEmail.Checked = True Then
            chkTuttiClienti.Checked = True
            txtCodCliente.Text = "" : txtDescCliente.Text = ""
            chkEscludiCliDaCateg.Enabled = True
            chkEscludiCliNoInvio.Enabled = True
            btnControllo.Visible = False
        Else
            chkEscludiCliDaCateg.Checked = False
            chkEscludiCliNoInvio.Checked = False
            chkEscludiCliDaCateg.Enabled = False
            chkEscludiCliNoInvio.Enabled = False
            btnControllo.Visible = True
        End If
    End Sub
End Class