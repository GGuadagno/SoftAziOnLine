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
Imports SoftAziOnLine.Documenti
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_ContrattiElScadPag
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private SWFatturaPA As Boolean = False 'GIU080814
    Private SWSplitIVA As Boolean = False 'giu05018
    'giu150312 per il ricalcolo scadenze
    'DATE DI SCADENZE RATE
    Dim lblDataScad1 As String = ""
    Dim lblDataScad2 As String = ""
    Dim lblDataScad3 As String = ""
    Dim lblDataScad4 As String = ""
    Dim lblDataScad5 As String = ""
    'IMPORTO RATE DI SCADENZE RATE
    Dim lblImpRata1 As String = ""
    Dim lblImpRata2 As String = ""
    Dim lblImpRata3 As String = ""
    Dim lblImpRata4 As String = ""
    Dim lblImpRata5 As String = ""
    Dim LblTotaleRate As String = ""
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""

    Private ArrScadPagCA As ArrayList 'giu030620

    Private aDataViewScPagAtt As DataView
    Private aDataViewScPagAttD As DataView
    Private Const sessDataViewScAtt As String = "sessDVScAttCElScadPag"
    Private Const sessDataViewScAttD As String = "sessDVScAttCElScadPagD"

    Private Enum CellIdxT
        RespArea = 1
        RespVisite = 2
        TotaleF1 = 3
        Sel = 4
        DataSc1 = 5
        Rata1 = 6
        DesRata = 7
        CKAttEvasa = 8
        Serie = 9
        TipoDoc = 10
        NumDoc = 11
        DataDoc = 12
        CodCliForProvv = 13
        RagSoc = 14
        Denom = 15

        Loc = 16
        Pr = 17
        CAP = 18
        Riferimento = 19
        Dest1 = 20
        Dest2 = 21
        Dest3 = 22
        Stato = 23
    End Enum
    Private Enum CellIdxD
        Sel = 0
        TipoDett = 1
        TipoDettR = 2
        SelInRata = 3
        CodArt = 4
        DesArt = 5
        SerieLotto = 6
        UM = 7
        Qta = 8
        QtaEv = 9
        QtaIn = 10
        QtaRe = 11
        QtaFa = 12
        Filiale = 13
        DataSc = 14
        IVA = 15
        Prz = 16
        ScV = 17
        Sc1 = 18
        Importo = 19
        ScR = 20
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ContrattiElScadPag.aspx?labelForm=Elenco Attività da fatturare"
        'giu291119 CONTRATTI
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        WFPDocCollegati.WucElement = Me
        Attesa.WucElement = Me
        'caricamento grid default
        Dim MessRetundDaGest As String = ""
        If (Not IsPostBack) Then
            'giu211123
            ''''giu030323 meglio ricaricare tutto se va in gestione contratti 
            '''Session(IDDOCUMENTI) = ""
            '''Session(sessDataViewScAtt) = Nothing
            '''Session(sessDataViewScAttD) = Nothing
            '''Session(CALLGESTIONE) = SWNO
            '-------------------------------------------------------------
            Dim SaveFilter As String = ""
            Dim SaveSort As String = ""
            btnDeselSc.Enabled = False
            btnEmFattSc.Enabled = False
            Try
                If String.IsNullOrEmpty(Session(CALLGESTIONE)) Then
                    Session(sessDataViewScAtt) = Nothing
                    Session(sessDataViewScAttD) = Nothing
                    Session(CALLGESTIONE) = SWNO
                ElseIf Session(CALLGESTIONE) = SWSI Then
                    'non azzero sempre che non sia scaduta
                    MessRetundDaGest = "!!!Nel caso in cui abbiate modificato dati del Contratto, si consiglia di ricaricare i dati!!!"
                    Session(CALLGESTIONE) = SWNO
                    aDataViewScPagAtt = Session(sessDataViewScAtt)
                    SaveFilter = aDataViewScPagAtt.RowFilter
                    SaveSort = aDataViewScPagAtt.Sort
                    aDataViewScPagAtt.RowFilter = "Sel<>0"
                    If aDataViewScPagAtt.Count > 0 Then
                        btnDeselSc.Enabled = True
                        btnEmFattSc.Enabled = True
                    End If
                    aDataViewScPagAtt.RowFilter = SaveFilter
                    If aDataViewScPagAtt.Count > 0 Then
                        aDataViewScPagAtt.Sort = SaveSort
                    End If
                    '----------------------------------------------------------------------
                Else
                    Session(sessDataViewScAtt) = Nothing
                    Session(sessDataViewScAttD) = Nothing
                    Session(CALLGESTIONE) = SWNO
                End If
            Catch ex As Exception
                Session(sessDataViewScAtt) = Nothing
                Session(sessDataViewScAttD) = Nothing
                Session(CALLGESTIONE) = SWNO
            End Try
        End If
        '----------------------------------
        Dim dsDoc As New DSDocumenti
        If Not Session(sessDataViewScAtt) Is Nothing Then
            aDataViewScPagAtt = Session(sessDataViewScAtt)
        Else
            If aDataViewScPagAtt Is Nothing Then
                aDataViewScPagAtt = New DataView
            End If
        End If
        Try
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = "DataSc1,Cod_Cliente" 'giu030223
        Catch ex As Exception
            'ok
        End Try
        Session(sessDataViewScAtt) = aDataViewScPagAtt
        GridViewPrevT.DataSource = aDataViewScPagAtt
        If (Not IsPostBack) Then
            Try
                If aDataViewScPagAtt.Count > 0 Then
                    chkCTRFatturato.Enabled = True
                    txtRicerca.Text = Session("txtRicerca")
                    chkSoloEvase.Checked = Session("chkSoloEvase")
                    Dim savePI = Session("PageIndex")
                    Dim saveSI = Session("SelIndex")
                    GridViewPrevT.PageIndex = savePI
                    GridViewPrevT.SelectedIndex = saveSI
                    GridViewPrevT.DataBind()
                    GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU211123
                Else
                    chkCTRFatturato.Enabled = False
                End If
                
            Catch ex As Exception

            End Try
        End If
        'GIU260321 giu210721
        Dim myAnnoDataSc As String = ""
        If String.IsNullOrEmpty(Session(DATADOCCOLL)) Then
            myAnnoDataSc = ""
        Else
            myAnnoDataSc = Session(DATADOCCOLL).ToString.Trim
        End If
        Dim myNSerie As String = ""
        If String.IsNullOrEmpty(Session(CSTSERIELOTTO)) Then
            myNSerie = ""
        Else
            myNSerie = Session(CSTSERIELOTTO).ToString.Trim
            If myNSerie.Trim = HTML_SPAZIO Then
                myNSerie = ""
            End If
        End If
        If String.IsNullOrEmpty(myAnnoDataSc) Then myAnnoDataSc = ""
        If Not IsDate(myAnnoDataSc) Then myAnnoDataSc = ""
        If myAnnoDataSc.Trim <> "" Then myAnnoDataSc = CDate(myAnnoDataSc).Year.ToString.Trim
        If String.IsNullOrEmpty(myNSerie) Then myNSerie = ""
        If (Session(IDDOCUMENTI) Is Nothing) Then
            Session(IDDOCUMENTI) = "-1"
        ElseIf String.IsNullOrEmpty(Session(IDDOCUMENTI)) Then
            Session(IDDOCUMENTI) = "-1"
        End If
        'GIU221123 SE SELEZIONO ANNI DIVERSI MA NON TUTTE LE ATTIVITA'
        Dim mySelInRata As String = ""
        If String.IsNullOrEmpty(Session("SelInRata")) Then
            mySelInRata = ""
        Else
            mySelInRata = Session("SelInRata").ToString.Trim
            If mySelInRata.Trim = HTML_SPAZIO Then
                mySelInRata = ""
            End If
        End If
        '--------
        If Not Session(sessDataViewScAttD) Is Nothing Then
            aDataViewScPagAttD = Session(sessDataViewScAttD)
            aDataViewScPagAttD.RowFilter = "IDDocumenti=" + Session(IDDOCUMENTI).ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) "
            If chkVisALLSc.Checked = False Then
                If myAnnoDataSc.Trim <> "" Then
                    aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                Else
                    aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                End If
                '-
                If chkSoloEvase.Checked = True Then
                    aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                End If
                If myNSerie.Trim <> "" Then
                    aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                End If
            Else
                If myNSerie.Trim <> "" Then 'giu011123
                    aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "'  OR ISNULL(Sel,0)<>0)"
                End If
            End If
        Else
            If aDataViewScPagAttD Is Nothing Then
                aDataViewScPagAttD = New DataView(dsDoc.ContrattiDFattALL)
                If aDataViewScPagAttD Is Nothing Then
                    'nulla
                Else
                    aDataViewScPagAttD.RowFilter = "IDDocumenti=" + Session(IDDOCUMENTI).ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) "
                    If chkVisALLSc.Checked = False Then
                        If myAnnoDataSc.Trim <> "" Then
                            aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                        Else
                            aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                        End If
                        '-
                        If chkSoloEvase.Checked = True Then
                            aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                        End If
                        If myNSerie.Trim <> "" Then
                            aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                        End If
                    Else
                        If myNSerie.Trim <> "" Then 'giu011123
                            aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "' OR ISNULL(Sel,0)<>0)"
                        End If
                    End If
                End If
            Else
                aDataViewScPagAttD.RowFilter = "IDDocumenti=" + Session(IDDOCUMENTI).ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) "
                If chkVisALLSc.Checked = False Then
                    If myAnnoDataSc.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                    Else
                        aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                    End If
                    '-
                    If chkSoloEvase.Checked = True Then
                        aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                    End If
                    If myNSerie.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                    End If
                Else
                    If myNSerie.Trim <> "" Then 'giu011123
                        aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "' OR ISNULL(Sel,0)<>0)"
                    End If
                End If
            End If
        End If
        Session(sessDataViewScAttD) = aDataViewScPagAttD
        GridViewPrevD.DataSource = aDataViewScPagAttD
        '---------------------------------
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSCausMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSRespArea.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRespVisite.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-
        If (Not IsPostBack) Then
            Try
                If CDate(Now).Year = Val(Session(ESERCIZIO)) Then
                    txtDataFattura.Text = Format(Now, FormatoData)
                Else
                    txtDataFattura.Text = Format(CDate("31/12/" & Session(ESERCIZIO).ToString.Trim), FormatoData)
                End If

                Dim StrErrore As String = ""
                If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                    txtPrimoNFattura.Text = (GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1).ToString
                    txtPrimoNFatturaPA.Text = (GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1).ToString
                Else
                    Chiudi("Errore Caricamento parametri generali.: " & StrErrore)
                    Exit Sub
                End If
                '-
                Session(CSTNUOVOCADAOC) = SWNO
                ddlRicerca.Items.Add("Numero contratto")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("N° Serie DAE")
                ddlRicerca.Items(1).Value = "NS"
                ddlRicerca.Items.Add("Data contratto")
                ddlRicerca.Items(2).Value = "D"
                ddlRicerca.Items.Add("Data Inizio CA")
                ddlRicerca.Items(3).Value = "DI"
                ddlRicerca.Items.Add("Data Fine CA")
                ddlRicerca.Items(4).Value = "DF"
                ddlRicerca.Items.Add("Data Accettazione")
                ddlRicerca.Items(5).Value = "DA"

                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(6).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(7).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(8).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(9).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(10).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(11).Value = "M"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(12).Value = "NR"
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(13).Value = "CG"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(14).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(15).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(16).Value = "D3"
                '--
                checkCausale.AutoPostBack = False : checkCausale.Checked = False : checkCausale.AutoPostBack = True
                DDLCausali.AutoPostBack = False
                DDLCausali.Enabled = False : DDLCausali.SelectedIndex = 0
                DDLCausali.AutoPostBack = True
                '-----------------------------------------------
                chkTutteProvince.AutoPostBack = False : chkTutteRegioni.AutoPostBack = False
                chkTutteRegioni.Checked = True
                ddlRegioni.Enabled = False
                chkTutteProvince.Checked = True
                ddlProvince.Enabled = False
                Session("CodRegione") = 0
                ddlRegioni.SelectedIndex = -1
                ddlProvince.SelectedIndex = -1
                chkTutteProvince.AutoPostBack = True : chkTutteRegioni.AutoPostBack = True
                'giu240321
                ddlMagazzino.SelectedIndex = 1
                '- Cercare la prima scadenza 
                txtDallaData.AutoPostBack = False : txtAllaData.AutoPostBack = False
                txtDallaData.Text = GetPrimaDataSc()
                If txtDallaData.Text.Trim = "" Then
                    txtDallaData.Text = Now.Date.ToString("01/01/" + "yyyy")
                End If
                ' ''Dim myAllaData As DateTime = DateAdd(DateInterval.Month, 1, Now.Date).ToString("01" + "/MM/yyyy")
                ' ''txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
                '- COME DA MENU FINO A .....
                ' ''myAllaData = CDate(txtAllaData.Text.Trim)
                ' ''Dim NGG As Integer = 30
                ' ''If Not String.IsNullOrEmpty(Session("NGGCAScadPag")) Then
                ' ''    If IsNumeric(Session("NGGCAScadPag")) Then
                ' ''        NGG = Session("NGGCAScadPag")
                ' ''    End If
                ' ''End If
                ' ''If DateAdd(DateInterval.Day, NGG, Now.Date) > myAllaData Then
                ' ''    myAllaData = DateAdd(DateInterval.Day, NGG, Now.Date)
                ' ''    myAllaData = DateAdd(DateInterval.Month, 1, myAllaData).ToString("01" + "/MM/yyyy")
                ' ''    txtAllaData.Text = DateAdd(DateInterval.Day, -1, myAllaData).Date.ToString("dd/MM/yyyy")
                ' ''End If
                ' ''If CDate(txtAllaData.Text) < CDate(txtDallaData.Text) Then
                ' ''    txtAllaData.Text = txtDallaData.Text
                ' ''End If
                If IsDate(txtDallaData.Text) Then
                    If CDate(txtDallaData.Text).Date > Now.Date Then
                        txtDallaData.Text = Now.Date.ToString("dd/MM/yyyy")
                    End If
                End If
                txtAllaData.Text = Now.Date.ToString("dd/MM/yyyy")
                'GIU020323 ERRRORE EVENTI SOVRAPPOSTI txtDallaData.AutoPostBack = True : txtAllaData.AutoPostBack = True
                '-giu210622
                Session(IDRESPAREA) = 0
                Call DataBindRV()
                '--
                If MessRetundDaGest.Trim = "" Then 'giu211123
                    BtnSetEnabledTo(False)
                    btnSelSc.Enabled = False
                    btnDeselSc.Enabled = False
                    btnEmFattSc.Enabled = False
                    btnStFattScEm.Enabled = False
                    Session(IDDOCUMENTI) = ""
                    '''BuidDett()
                End If
                '--
                Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
                Session(SWOP) = SWOPNESSUNA
                Session(SWOPDETTDOC) = SWOPNESSUNA
                Session(SWOPDETTDOCL) = SWOPNESSUNA
                'giu210512 controllo numerazione fattura se corretta
                '--------------------------------------------
                Dim CkNumDoc As Long = -1 : Dim CkNumDocPA As Long = -1
                If CheckNumDoc(CkNumDoc, CkNumDocPA, StrErrore) = False Then
                    Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
                    Exit Sub
                End If
                If CLng(txtPrimoNFattura.Text) <> CkNumDoc Then
                    txtPrimoNFattura.BackColor = SEGNALA_KO
                    txtPrimoNFattura.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!: " + FormattaNumero(CkNumDoc)
                End If
                If CLng(txtPrimoNFatturaPA.Text) <> CkNumDocPA Then
                    txtPrimoNFatturaPA.BackColor = SEGNALA_KO
                    txtPrimoNFatturaPA.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!: " + FormattaNumero(CkNumDocPA)
                End If
                If txtPrimoNFattura.BackColor = SEGNALA_KO Or txtPrimoNFatturaPA.BackColor = SEGNALA_KO Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = "Attenzione, ci sono dei numeri documento da recuperare!!!"
                    ModalPopup.Show("N° Fattura fuori sequenza", "<strong><span> Si consiglia di controllare i N° di Fattura segnalati in rosso prima di procedere." _
                                    & "<br>" + MessRetundDaGest _
                                & "</span></strong>", WUC_ModalPopup.TYPE_ALERT)
                ElseIf MessRetundDaGest.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("!!ATTENZIONE!!", "<strong><span>" + MessRetundDaGest _
                                & "</span></strong>", WUC_ModalPopup.TYPE_ALERT)
                End If
                btnRicerca.Focus()
            Catch ex As Exception
                Chiudi("Errore: Caricamento Elenco contratti: " & ex.Message)
                Exit Sub
            End Try
        End If
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
        'Simone 290317
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
    'giu040620 riportato controllo sequenza N° Fatture/PA
    Private Function CheckNumDoc(ByRef _NDoc As Long, ByRef _NDocPA As Long, ByRef strErrore As String) As Boolean
        CheckNumDoc = True
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero "
        strSQL += "From DocumentiT WHERE "
        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' OR "
        strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
        If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
            strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
        End If
        strSQL += ") AND ISNULL(FatturaPA,0)=0"
        '---------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        _NDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        _NDoc = 1
                    End If
                Else
                    _NDoc = 1
                End If
            Else
                _NDoc = 1
            End If
        Catch Ex As Exception
            _NDoc = -1
            strErrore = Ex.Message
            CheckNumDoc = False
            Exit Function
        End Try
        ds = Nothing
        '-pa
        strSQL = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero "
        strSQL += "From DocumentiT WHERE "
        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' "
        If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
            strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
        End If
        strSQL += ") AND ISNULL(FatturaPA,0)<>0"
        '---------
        Dim dsPA As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsPA)
            ObjDB = Nothing
            If (dsPA.Tables.Count > 0) Then
                If (dsPA.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(dsPA.Tables(0).Rows(0).Item("Numero")) Then
                        _NDocPA = dsPA.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        _NDocPA = 1
                    End If
                Else
                    _NDocPA = 1
                End If
            Else
                _NDocPA = 1
            End If
        Catch Ex As Exception
            _NDocPA = -1
            strErrore = Ex.Message
            CheckNumDoc = False
            Exit Function
        End Try
        dsPA = Nothing
    End Function
    'giu270520 prima data scadenza PAGAMENTO
    Private Function GetPrimaDataSc() As String
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        strSQL = "SELECT MIN(Data_Scadenza_1) AS DataSc FROM ContrattiT WHERE StatoDoc<3"
        Dim ds As New DataSet
        Try
            GetPrimaDataSc = ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataSc")) Then
                        GetPrimaDataSc = Format(ds.Tables(0).Rows(0).Item("DataSc"), FormatoData)
                    End If
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            GetPrimaDataSc = ""
            Exit Function
        End Try
    End Function
    'giu050620
    Private Function GetScadenzario(ByVal _IDDoc As String) As String
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        strSQL = "SELECT ScadPagCA FROM ContrattiT WHERE IDDocumenti=" & _IDDoc.Trim
        Dim ds As New DataSet
        Try
            GetScadenzario = ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ScadPagCA")) Then
                        GetScadenzario = ds.Tables(0).Rows(0).Item("ScadPagCA")
                    End If
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            GetScadenzario = ""
            Exit Function
        End Try
    End Function
    'giu270112
    Private Sub BtnSetByStato(ByVal myStato As String)
        '''Dim SWBloccoCambiaStato As Boolean = False
        ''''--
        '''BtnSetEnabledTo(True)
        '''If myStato.Trim = "Evaso" Or _
        '''                myStato.Trim = "Chiuso non evaso" Or _
        '''                myStato.Trim = "Non evadibile" Or _
        '''                myStato.Trim = "Parz. evaso" Then
        '''    btnVisualizza.Enabled = False
        '''    SWBloccoCambiaStato = True
        '''    'btnVerbale.Enabled = False
        '''    'btnStampa.Enabled = False
        '''End If
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnVisualizza.Enabled = Valore
        btnStampa.Enabled = Valore
        btnVerbale.Enabled = Valore
        btnElencoSc.Enabled = Valore
        btnDocCollegati.Enabled = Valore
    End Sub

    Private Sub SvuodaGridT()
        SetLnk()

        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""
        btnRicerca.BackColor = SEGNALA_KO
        aDataViewScPagAtt = Nothing
        aDataViewScPagAtt = New DataView
        Session(sessDataViewScAtt) = aDataViewScPagAtt
        GridViewPrevT.DataSource = aDataViewScPagAtt
        '-
        aDataViewScPagAttD = Nothing
        aDataViewScPagAttD = New DataView
        Session(sessDataViewScAttD) = aDataViewScPagAttD
        GridViewPrevD.DataSource = aDataViewScPagAttD
        '-----
        BtnSetEnabledTo(False)
        btnSelSc.Enabled = False
        btnDeselSc.Enabled = False
        btnEmFattSc.Enabled = False
        btnStFattScEm.Enabled = False
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind() 'GIU180322
        chkCTRFatturato.Checked = False
        DDLCTRFatturato.Enabled = False
        DDLCTRFatturato.Items.Clear()
        DDLCTRFatturato.Items.Add("")
    End Sub
#Region " Ordinamento e ricerca"
    Private Sub checkCausale_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkCausale.CheckedChanged
        SetLnk()
        SvuodaGridT()
        If checkCausale.Checked = True Then
            DDLCausali.AutoPostBack = False : DDLCausali.Enabled = True : DDLCausali.SelectedIndex = 0 : DDLCausali.AutoPostBack = True
            Dim strValore As String = "" : Dim strErrore As String = ""
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLCausali)
                Call DDLCausali_SelectedIndexChanged(Nothing, Nothing)
                Exit Sub
            End If
        Else
            DDLCausali.AutoPostBack = False : DDLCausali.Enabled = False : DDLCausali.SelectedIndex = 0 : DDLCausali.AutoPostBack = True
        End If
        If checkCausale.Checked = True Then
            DDLCausali.Focus()
        End If
    End Sub
    Private Sub DDLCausali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLCausali.SelectedIndexChanged
        SvuodaGridT()
        SetLnk()
        If DDLCausali.SelectedIndex = 0 Then
            DDLCausali.BackColor = SEGNALA_KO
        Else
            DDLCausali.BackColor = SEGNALA_OK
        End If
    End Sub

    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        SvuodaGridT()
        SetLnk()
        checkParoleContenute.Text = "Parole contenute"
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "DI" Or _
               ddlRicerca.SelectedValue = "DF" Or _
               ddlRicerca.SelectedValue = "DA" Then
            checkParoleContenute.Text = ">= Alla Data"
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        SvuodaGridT()
        SetLnk()
        'default 22%
        Dim strErrore As String = "" : Dim strValore As String = ""
        If String.IsNullOrEmpty(Session("IVAFattCA")) Then
            If App.GetDatiAbilitazioni(CSTABILAZI, "IVAFattCA", strValore, strErrore) = True Then
                Session("IVAFattCA") = strValore
            Else
                Session("IVAFattCA") = 22
            End If
        End If
        Dim IVAFattCA As Integer = 22
        If Not String.IsNullOrEmpty(Session("IVAFattCA")) Then
            If IsNumeric(Session("IVAFattCA")) Then
                IVAFattCA = Session("IVAFattCA")
            End If
        End If
        '-
        Dim FormatValuta As String = "###,###,##0.00"
        '-----
        Dim strFiltroRicerca As String = ""
        Dim SWErrore As Boolean = False
        '----
        txtDataFattura.Text = ConvData(txtDataFattura.Text.ToString.Trim)
        If txtDataFattura.Text.Trim.Length <> 10 Then
            txtDataFattura.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf IsDate(txtDataFattura.Text.Trim) Then
            txtDataFattura.BackColor = SEGNALA_OK
            txtDataFattura.Text = Format(CDate(txtDataFattura.Text), FormatoData)
        Else
            txtDataFattura.BackColor = SEGNALA_KO
            SWErrore = True
        End If
        '-
        txtDallaData.Text = ConvData(txtDallaData.Text.ToString.Trim)
        If txtDallaData.Text.Trim.Length <> 10 Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtDallaData.Text.Trim) Then
            txtDallaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtDallaData.BackColor = SEGNALA_OK
        End If
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        '-
        txtAllaData.Text = ConvData(txtAllaData.Text.ToString.Trim)
        If txtAllaData.Text.Trim.Length <> 10 Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        ElseIf Not IsDate(txtAllaData.Text.Trim) Then
            txtAllaData.BackColor = SEGNALA_KO
            SWErrore = True
        Else
            txtAllaData.BackColor = SEGNALA_OK
        End If
        '------------------------------
        If SWErrore Then
            txtDallaData.Focus()
            Exit Sub
        End If
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        If checkCausale.Checked = True And DDLCausali.SelectedIndex < 1 Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali.Focus()
            Exit Sub
        End If
        If checkRespArea.Checked = True And DDLRespArea.SelectedIndex < 1 Then
            DDLRespArea.BackColor = SEGNALA_KO : DDLRespArea.Focus()
            Exit Sub
        End If
        If CheckRespVisite.Checked = True And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex < 1 Then
            ddlRegioni.BackColor = SEGNALA_KO : ddlRegioni.Focus()
            Exit Sub
        End If
        If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex < 1 Then
            ddlProvince.BackColor = SEGNALA_KO : ddlProvince.Focus()
            Exit Sub
        End If
        '---------
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "DI" Or _
               ddlRicerca.SelectedValue = "DF" Or _
               ddlRicerca.SelectedValue = "DA" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        'giu090520
        'PARAMETRI SP
        If String.IsNullOrEmpty(lblDesCliente.ToolTip) Then
            lblDesCliente.ToolTip = ""
        End If
        If lblDesCliente.ToolTip = "" Then
            SqlDSPrevTElenco.SelectParameters("Cod_Cliente").DefaultValue = "-1"
        Else
            SqlDSPrevTElenco.SelectParameters("Cod_Cliente").DefaultValue = lblDesCliente.ToolTip
        End If
        '-
        Dim myDallaData As String = txtDallaData.Text.Trim : Dim myAllaData As String = txtAllaData.Text.Trim
        SqlDSPrevTElenco.SelectParameters("DallaData").DefaultValue = myDallaData.Trim
        SqlDSPrevTElenco.SelectParameters("AllaData").DefaultValue = myAllaData.Trim
        strFiltroRicerca = "Elenco Attività da fatturare nel Periodo: dal " + myDallaData.Trim + " al " + myAllaData.Trim
        SqlDSPrevTElenco.SelectParameters("DaIncTutti").DefaultValue = 1
        ' ''If rbtnDaFatturare.Checked = True Then
        ' ''    strFiltroRicerca = "Elenco Attività da fatturare nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        ' ''    SqlDSPrevTElenco.SelectParameters("DaIncTutti").DefaultValue = 1
        ' ''ElseIf rbtnFatturate.Checked = True Then
        ' ''    strFiltroRicerca = "Elenco Attività fatturate nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        ' ''    SqlDSPrevTElenco.SelectParameters("DaIncTutti").DefaultValue = 2
        ' ''Else
        ' ''    strFiltroRicerca = "Elenco Attività da fatturare/fatturate nel Periodo: dal " + txtDallaData.Text.Trim + " al " + txtAllaData.Text.Trim
        ' ''    SqlDSPrevTElenco.SelectParameters("DaIncTutti").DefaultValue = 3
        ' ''End If
        If DDLRespArea.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = 0
        Else
            SqlDSPrevTElenco.SelectParameters("RespArea").DefaultValue = CInt(DDLRespArea.SelectedValue.ToString.Trim)
        End If
        If DDLRespVisite.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
        Else
            'giu210622 il respVisite è presente piu volte xke collegato a piu RespArea
            'SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = CInt(DDLRespVisite.SelectedValue.ToString.Trim)
            SqlDSPrevTElenco.SelectParameters("RespVisite").DefaultValue = 0
            '----------
            strFiltroRicerca += " - Solo il Resp.Visita: " + DDLRespVisite.SelectedItem.Text.Trim
        End If
        If DDLCausali.SelectedIndex = 0 Then
            SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = 0
        Else
            SqlDSPrevTElenco.SelectParameters("Causale").DefaultValue = CInt(DDLCausali.SelectedValue.ToString.Trim)
            strFiltroRicerca += " - Tipo Contratto: " + DDLCausali.SelectedItem.Text.Trim
        End If
        '------------------------
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""
        '-
        ImpostaFiltro()
        aDataViewScPagAtt = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)

        Dim dsDoc As New DSDocumenti
        If (aDataViewScPagAtt Is Nothing) Then
            Session("dsDocElencoScadPag") = dsDoc
            aDataViewScPagAtt = New DataView(dsDoc.ScadAttPag)
            Session(sessDataViewScAtt) = aDataViewScPagAtt
            'giu260321
            aDataViewScPagAttD = New DataView(dsDoc.ContrattiDFattALL)
            Session(sessDataViewScAttD) = aDataViewScPagAttD
            '-----
            BuidDett()
            Exit Sub
        End If
        aDataViewScPagAtt.Sort = "IDDocumenti" 'giu301023 per escludere gli eventuali duplicati per effetto dei Resp.Area/Visita e N°Serie
        Dim ObjDB As New DataBaseUtility
        If chkTutteRegioni.Checked = False Then
            'giu090520 filtro REGIONE/PROVINCIA
            strFiltroRicerca += " - Regione: " + ddlRegioni.SelectedItem.Text.Trim
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, "SELECT * FROM Province", dsDoc, "Province")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento Province: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, "SELECT * FROM Regioni", dsDoc, "Regioni")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento Regioni: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else

        End If
        '--------
        ' ''Dim rowRegioni As DSDocumenti.RegioniRow
        Dim rowProvince As DSDocumenti.ProvinceRow
        Dim dc As DataColumn
        Dim IDDocPrec As String = ""
        Try
            If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex > 0 Then
                aDataViewScPagAtt.RowFilter = "Provincia='" & ddlProvince.SelectedValue.Trim & "'"
                strFiltroRicerca += " - Provincia: (" + ddlProvince.SelectedValue.Trim + ")"
            End If
            If txtRicerca.Text.Trim <> "" Then
                strFiltroRicerca += " - Ricerca per: " + ddlRicerca.SelectedItem.Text.Trim + " (" + txtRicerca.Text.Trim + ")"
            End If
            'giu210520 RATE FATTURAZIONE 
            Dim myIDDoc As String = "" : Dim myTipoDoc As String = "" : Dim myNDoc As String = "" : Dim myDataDoc As String = ""
            Dim strDesRata As String = "" : Dim PrimaRata As Integer = 0
            Dim strScadPagCA As String = ""
            Dim NRate As Integer = 0
            Dim TotRate As Decimal = 0
            '-------------------------------------------------
            Dim SQLSelectIN As String = "" 'GIU260321
            '-
            For i = 0 To aDataViewScPagAtt.Count - 1
                If IDDocPrec = "" Then
                    IDDocPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                ElseIf IDDocPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim Then
                    Continue For
                Else
                    IDDocPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                End If
                If chkTutteProvince.Checked = True Then
                    If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex > 0 Then
                        rowProvince = dsDoc.Province.FindByCodice(aDataViewScPagAtt.Item(i)("Provincia"))
                        If rowProvince Is Nothing Then
                            Continue For
                        ElseIf rowProvince.Regione <> ddlRegioni.SelectedValue Then
                            Continue For
                        End If
                    End If
                End If
                'giu210622 
                If CheckRespVisite.Checked And DDLRespVisite.SelectedIndex > 0 And DDLRespVisite.SelectedItem.Text.Trim <> "" Then
                    If DDLRespVisite.SelectedItem.Text.Trim <> aDataViewScPagAtt.Item(i)("DesRespVisite").ToString.Trim Then
                        Continue For
                    End If
                End If
                '---------
                '-
                'giu210520 RATE FATTURAZIONE ScadPagCA
                NRate = 0 : TotRate = 0
                strDesRata = "" : PrimaRata = 0
                myIDDoc = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                myTipoDoc = aDataViewScPagAtt.Item(i)("SiglaCA")
                myNDoc = aDataViewScPagAtt.Item(i)("Numero").ToString.Trim
                myDataDoc = aDataViewScPagAtt.Item(i)("Data_Doc").ToString.Trim
                'giu260321
                If SQLSelectIN.Trim <> "" Then
                    SQLSelectIN += "," + myIDDoc
                Else
                    SQLSelectIN += myIDDoc
                End If
                '-
                If Not IsDBNull(aDataViewScPagAtt.Item(i)("ScadPagCA")) Then
                    strScadPagCA = aDataViewScPagAtt.Item(i)("ScadPagCA")
                    If String.IsNullOrEmpty(strScadPagCA.Trim) Then
                        strScadPagCA = ""
                    End If
                End If
                '-
                dsDoc.ScadPagCA.Clear()
                If strScadPagCA.Trim <> "" Then
                    Try
                        Dim lineaSplit As String() = strScadPagCA.Trim.Split(";")
                        For ii = 0 To lineaSplit.Count - 1
                            If lineaSplit(ii).Trim <> "" And (ii + 8) <= lineaSplit.Count - 1 Then 'giu191223 ii + 6
                                Dim newRowScP As DSDocumenti.ScadPagCARow = dsDoc.ScadPagCA.NewScadPagCARow
                                newRowScP.BeginEdit()
                                newRowScP.IDDocumenti = myIDDoc.Trim
                                newRowScP.NRata = CInt(lineaSplit(ii).Trim)
                                ii += 1
                                newRowScP.DataSc = CDate(lineaSplit(ii).Trim)
                                ii += 1
                                newRowScP.Importo = CDec(lineaSplit(ii).Trim)
                                TotRate += newRowScP.Importo
                                ii += 1
                                newRowScP.Evasa = CBool(lineaSplit(ii).Trim)
                                If CBool(lineaSplit(ii).Trim) = False And strDesRata.Trim = "" Then
                                    strDesRata = Format(newRowScP.NRata, "####0")
                                    PrimaRata = newRowScP.NRata
                                End If
                                ii += 1
                                newRowScP.NFC = lineaSplit(ii).Trim
                                ii += 1
                                newRowScP.DataFC = lineaSplit(ii).Trim
                                ii += 1
                                newRowScP.Serie = lineaSplit(ii).Trim
                                ii += 1
                                newRowScP.ImportoF = CDec(lineaSplit(ii).Trim)
                                ii += 1
                                newRowScP.ImportoR = CDec(lineaSplit(ii).Trim)
                                newRowScP.TotNRate = 0
                                newRowScP.TotRate = 0
                                newRowScP.EndEdit()
                                dsDoc.ScadPagCA.AddScadPagCARow(newRowScP)
                                NRate += 1
                            End If
                        Next
                        strDesRata += " di " + Format(NRate, "####0")
                        'Non mi serve qui
                        ' ''Dim rsScadPagCA As DSDocumenti.ScadPagCARow
                        ' ''For Each rsScadPagCA In dsDoc.Tables("ScadPagCA").Select("")
                        ' ''    rsScadPagCA.BeginEdit()
                        ' ''    rsScadPagCA.TotNRate = NRate
                        ' ''    rsScadPagCA.TotRate = TotRate
                        ' ''    rsScadPagCA.EndEdit()
                        ' ''    rsScadPagCA.AcceptChanges()
                        ' ''Next
                        '-
                    Catch ex As Exception
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Dati Tab1. ScadPagCA: " & "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc & " <br> " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End Try
                Else
                    dsDoc.ScadPagCA.Clear()
                End If
                dsDoc.ScadPagCA.AcceptChanges()
                '--------------------------------------------------------------------------------------------------------------
                '-Aggiungo eventuali altre scadenze
                Try
                    Dim rsScadPagCA As DSDocumenti.ScadPagCARow
                    For Each rsScadPagCA In dsDoc.Tables("ScadPagCA").Select("", "NRata") 'giu300722"NRata<>" + PrimaRata.ToString.Trim, "NRata")
                        If rsScadPagCA.Evasa = True Then
                            Continue For
                        End If
                        If rsScadPagCA.DataSc < CDate(myDallaData) Or rsScadPagCA.DataSc > CDate(myAllaData) Then
                            Continue For
                        End If
                        '--------------------------------------------------------------------------------------------------------------
                        strDesRata = Format(rsScadPagCA.NRata, "####0") + " di " + Format(NRate, "####0")
                        Dim newRow2 As DSDocumenti.ScadAttPagRow = dsDoc.ScadAttPag.NewScadAttPagRow
                        newRow2.BeginEdit()
                        For Each dc In dsDoc.ScadAttPag.Columns
                            If UCase(dc.ColumnName) = "FILTRORICERCA" Then
                                newRow2.Item(dc.ColumnName) = strFiltroRicerca.Trim
                            ElseIf UCase(dc.ColumnName) = "NRATA" Then
                                newRow2.Item(dc.ColumnName) = rsScadPagCA.NRata
                            ElseIf UCase(dc.ColumnName) = "DESRATA" Then
                                newRow2.Item(dc.ColumnName) = strDesRata.Trim
                            ElseIf UCase(dc.ColumnName) = "SEL" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "FATTURAPA" Then
                                newRow2.Item(dc.ColumnName) = False
                            ElseIf UCase(dc.ColumnName) = "TOTIMPON1" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "TOTIVA1" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "TOTIMPONF1" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "TOTIVAF1" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "TOTALEF1" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "IVAFATTCA" Then
                                newRow2.Item(dc.ColumnName) = 0
                            ElseIf UCase(dc.ColumnName) = "CKATTEVALLADATA" Then
                                newRow2.Item(dc.ColumnName) = False
                            Else
                                If IsDBNull(aDataViewScPagAtt.Item(i)(dc.ColumnName)) Then
                                    newRow2.Item(dc.ColumnName) = DBNull.Value
                                Else
                                    newRow2.Item(dc.ColumnName) = aDataViewScPagAtt.Item(i)(dc.ColumnName)
                                End If
                            End If
                        Next
                        If Not newRow2.IsIPANull Then
                            If newRow2.IPA.Trim.Length = 6 Then
                                newRow2.FatturaPA = True
                            End If
                        End If
                        ' ''newRow2.NRata = rsScadPagCA.NRata 'bho non capisco valorizzato anche prima 
                        newRow2.DataSc1 = rsScadPagCA.DataSc
                        newRow2.Rata1 = rsScadPagCA.Importo
                        newRow2.SetDataSc2Null() : newRow2.SetRata2Null()
                        newRow2.SetDataSc3Null() : newRow2.SetRata3Null()
                        newRow2.SetDataSc4Null() : newRow2.SetRata4Null()
                        newRow2.SetDataSc5Null() : newRow2.SetRata5Null()
                        'giu010620 calcolo IMPON1 E IVA1 - ATTENZIONE SE HA LO SPLIT IVA IN QUEL CASO E' IN AGGIUNTA MA NON A SCORPORO
                        'testare il Regime_IVA per evitare di calcare IVA dove non cè
                        If newRow2.Regime_IVA > 49 Then 'TOTALE CON IVA 0 quindi IVA 0
                            newRow2.TotImpon1 = newRow2.Rata1
                            newRow2.TotIVA1 = 0
                            newRow2.IVAFattCA = newRow2.Regime_IVA
                        ElseIf newRow2.SplitIVA = True Then 'il TOTALE RATA E' SENZA IVA: calcolo IVA per determinare il totale
                            newRow2.TotImpon1 = newRow2.Rata1
                            newRow2.TotIVA1 = (newRow2.TotImpon1 / 100) * IVAFattCA
                            newRow2.TotIVA1 = Format(newRow2.TotIVA1, FormatValuta) 'solo ESPOSTA ma nn concorre al totale netto da pagare
                            newRow2.IVAFattCA = IVAFattCA
                        Else 'TOTALE CON IVA GIA DIVISO quindi SCORPORO 
                            'D = (P * 100) / (100 + P)
                            newRow2.TotImpon1 = (newRow2.Rata1 * 100) / (IVAFattCA + 100)
                            newRow2.TotImpon1 = Format(newRow2.TotImpon1, FormatValuta)
                            newRow2.TotIVA1 = Format(newRow2.Rata1 - newRow2.TotImpon1, FormatValuta)
                            newRow2.IVAFattCA = IVAFattCA
                        End If
                        'giu191223
                        If rsScadPagCA.IsSerieNull Then
                            newRow2.SetSerieNull()
                            newRow2.CKAttEvAllaData = CKAttEvAllaData(newRow2.IDDocumenti.ToString.Trim, newRow2.DataSc1, "") 'giu191223
                        Else
                            newRow2.Serie = rsScadPagCA.Serie.Trim
                            newRow2.CKAttEvAllaData = CKAttEvAllaData(newRow2.IDDocumenti.ToString.Trim, newRow2.DataSc1, newRow2.Serie) 'giu191223
                        End If
                        '------------- RIPETERLA ANCHE X LE RATE INSERITE DOPO - CONTROLLO IN FASE DI EMISSIONE FATTURE
                        newRow2.EndEdit()
                        dsDoc.ScadAttPag.AddScadAttPagRow(newRow2)
                        newRow2 = Nothing
                        '-
                    Next
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Dati Tab2. ScadPagCA: " & "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc & " <br> " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
            'giu100620
            If chkSoloEvase.Checked = True Then
                Dim RowScadAtt As DSDocumenti.ScadAttPagRow
                For Each RowScadAtt In dsDoc.ScadAttPag.Select("CKAttEvAllaData=0")
                    RowScadAtt.Delete()
                Next
            End If
            'giu260321 carico tutti i dettagli relativo alle scadenze
            dsDoc.ContrattiDFattALL.Clear()
            If SQLSelectIN.Trim <> "" Then
                Dim strSQL As String = "SELECT *, CASE WHEN DurataNum=0 THEN 'Appar.' ELSE 'Attività' END AS DesTipoDett, DurataNumRiga + 1 AS DesTipoDettR, " & _
                "CASE WHEN ISNULL(OmaggioImponibile,0)=0 AND ISNULL(OmaggioImposta,0)=0 THEN '' " & _
                "WHEN ISNULL(OmaggioImponibile,0)<>0 AND ISNULL(OmaggioImposta,0)<>0 THEN 'SM' " & _
                "WHEN ISNULL(OmaggioImponibile,0)<>0 AND ISNULL(OmaggioImposta,0)=0 THEN 'OM' " & _
                "ELSE '' END AS TipoScontoMerce, ISNULL(Serie,'') + ISNULL(Lotto,'') AS SerieLotto," & _
                "ISNULL(DestClienti.Localita, '') AS Des_Filiale, 0 AS Sel, " & _
                "DATEPART(year, DataSc) AS AnnoDataSc, '' as SelInRata " & _
                "FROM ContrattiD LEFT OUTER JOIN " & _
                "DestClienti ON ContrattiD.Cod_Filiale = DestClienti.Progressivo WHERE DurataNum<>0 AND IDDocumenti IN (" + SQLSelectIN + ") " & _
                "ORDER BY IDDocumenti"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsDoc, "ContrattiDFattALL")
                Catch Ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento Dettaglio attività: " & Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            End If
            'dsDoc.ContrattiDFattALL.AcceptChanges()
            '-
            dsDoc.ScadAttPag.AcceptChanges()
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Seleziona elenco scadenze: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Finally
            ObjDB = Nothing
        End Try
        '-
        Session("dsDocElencoScadPag") = dsDoc
        aDataViewScPagAtt = New DataView(dsDoc.ScadAttPag)
        Session(sessDataViewScAtt) = aDataViewScPagAtt
        '-
        aDataViewScPagAttD = New DataView(dsDoc.ContrattiDFattALL)
        Session(sessDataViewScAttD) = aDataViewScPagAttD
        '---------------------------------
        BuidDett()
    End Sub
    '
    Private Sub ImpostaFiltro()
        SqlDSPrevTElenco.FilterExpression = ""
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "DI" Or _
               ddlRicerca.SelectedValue = "DF" Or _
               ddlRicerca.SelectedValue = "DA" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim <> "" Then
            If ddlRicerca.SelectedValue = "N" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
                Else
                    SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
                End If
            ElseIf ddlRicerca.SelectedValue = "D" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "DI" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataInizio >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataInizio = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "DF" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataFine >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataFine = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "DA" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataAccetta >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataAccetta = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "R" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "L" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "M" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "S" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "P" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "F" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NR" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Riferimento >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D1" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione1 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione1 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D2" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione2 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione2 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D3" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione3 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione3 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NS" Then
                If SqlDSPrevTElenco.FilterExpression <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Serie like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Serie = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            End If
        End If
        If DDLRespArea.SelectedIndex > 0 And checkRespArea.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(RespArea =" & DDLRespArea.SelectedValue.ToString.Trim & ")"
        End If
        If DDLRespVisite.SelectedIndex > 0 And CheckRespVisite.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            'giu210622 il respVisite è presente piu volte xke collegato a piu RespArea
            'SqlDSPrevTElenco.FilterExpression += "(RespVisite =" & DDLRespVisite.SelectedValue.ToString.Trim & ")"
            SqlDSPrevTElenco.FilterExpression += "(DesRespVisite ='" & DDLRespVisite.SelectedItem.Text.Trim & "')"
        End If
        Session("chkSoloEvase") = chkSoloEvase.Checked
        Session("txtRicerca") = txtRicerca.Text.Trim
    End Sub
    Private Sub BuidDett()
        btnRicerca.BackColor = btnVisualizza.BackColor
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(IDDOCUMENTI) = ""

        ' ''Dim dsDoc As New DSDocumenti
        ' ''If (aDataViewScPagAtt Is Nothing) Then
        ' ''    dsDoc = Session("dsDocElencoScadPag")
        ' ''    aDataViewScPagAtt = New DataView(dsDoc.ScadAttPag)
        ' ''    Session(sessDataViewScAtt) = aDataViewScPagAtt
        ' ''End If
        Try
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = "DataSc1,Cod_Cliente" 'giu030223
        Catch ex As Exception
            'ok
        End Try
        GridViewPrevT.DataSource = aDataViewScPagAtt
        GridViewPrevT.DataBind()
        Session(sessDataViewScAtt) = aDataViewScPagAtt
        '---------
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        Session(DATADOCCOLL) = ""
        Session(CSTSERIELOTTO) = ""
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            btnSelSc.Enabled = True
            chkCTRFatturato.Enabled = True
            '''btnDeselSc.Enabled = False
            '''btnEmFattSc.Enabled = False
            '''btnStFattScEm.Enabled = False
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Session(DATADOCCOLL) = row.Cells(CellIdxT.DataSc1).Text.Trim
                Session(CSTSERIELOTTO) = row.Cells(CellIdxT.Serie).Text.Trim
                Session("SelInRata") = row.Cells(CellIdxT.DesRata).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            chkCTRFatturato.Enabled = False
            BtnSetEnabledTo(False)
            btnSelSc.Enabled = False
            btnDeselSc.Enabled = False
            btnEmFattSc.Enabled = False
            btnStFattScEm.Enabled = False
            Session(IDDOCUMENTI) = ""
        End If
        '-
        BuildDettD()
    End Sub
    Private Sub BuildDettD()
        Dim dsDoc As New DSDocumenti
        dsDoc = Session("dsDocElencoScadPag")
        aDataViewScPagAttD = New DataView(dsDoc.ContrattiDFattALL)
        Session(sessDataViewScAttD) = aDataViewScPagAttD
        If (Session(IDDOCUMENTI) Is Nothing) Then
            Session(IDDOCUMENTI) = "-1"
        ElseIf String.IsNullOrEmpty(Session(IDDOCUMENTI)) Then
            Session(IDDOCUMENTI) = "-1"
        End If
        'giu011123
        Dim myNSerie As String = ""
        If String.IsNullOrEmpty(Session(CSTSERIELOTTO)) Then
            myNSerie = ""
        Else
            myNSerie = Session(CSTSERIELOTTO).ToString.Trim
            If myNSerie.Trim = HTML_SPAZIO Then
                myNSerie = ""
            End If
        End If
        'GIU221123 SE SELEZIONO ANNI DIVERSI MA NON TUTTE LE ATTIVITA'
        Dim mySelInRata As String = ""
        If String.IsNullOrEmpty(Session("SelInRata")) Then
            mySelInRata = ""
        Else
            mySelInRata = Session("SelInRata").ToString.Trim
            If mySelInRata.Trim = HTML_SPAZIO Then
                mySelInRata = ""
            End If
        End If
        '--------
        Dim myAnnoOrDataSc As String = ""
        If String.IsNullOrEmpty(Session(DATADOCCOLL)) Then
            myAnnoOrDataSc = ""
        Else
            myAnnoOrDataSc = Session(DATADOCCOLL).ToString.Trim
        End If
        If String.IsNullOrEmpty(myAnnoOrDataSc) Then myAnnoOrDataSc = ""
        If Not IsDate(myAnnoOrDataSc) Then myAnnoOrDataSc = ""
        If myAnnoOrDataSc.Trim <> "" Then myAnnoOrDataSc = CDate(myAnnoOrDataSc).Year.ToString.Trim
        aDataViewScPagAttD.RowFilter = "IDDocumenti=" + Session(IDDOCUMENTI).ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) "
        If chkVisALLSc.Checked = False Then
            If myAnnoOrDataSc.Trim = "" Then myAnnoOrDataSc = "0000"
            If myAnnoOrDataSc.Trim <> "" Then
                aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoOrDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
            Else
                aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
            End If
            '-
            If chkSoloEvase.Checked = True Then
                aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0 OR ISNULL(Sel,0)<>0) "
            End If
            If myNSerie.Trim <> "" Then
                aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "' OR ISNULL(Sel,0)<>0)"
            End If
        Else
            If myNSerie.Trim <> "" Then
                aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "' OR ISNULL(Sel,0)<>0)"
            End If
        End If
        If aDataViewScPagAttD.Count > 0 Then
            aDataViewScPagAttD.Sort = "DataSc"
        End If
        '-
        GridViewPrevD.DataSource = aDataViewScPagAttD
        GridViewPrevD.DataBind()
    End Sub
    'giu050620
    Private Function CKAttEvAllaData(ByVal _IDDoc As String, ByVal _DataScPag As DateTime, ByVal _NSerie As String) As Boolean
        Dim strSQL As String = ""
        Dim myAnnoDataSc As String = _DataScPag.Year.ToString.Trim
        '-
        Dim ObjDB As New DataBaseUtility
        strSQL = "SELECT DataSc, Qta_Evasa, YEAR(DataSc) AS AnnoDataSc FROM ContrattiD WHERE IDDocumenti=" & _IDDoc.Trim & _
        " AND DurataNum=1 AND YEAR(DataSc)=" + _DataScPag.Year.ToString.Trim + ""
        If _NSerie.Trim <> "" Then 'giu191223
            strSQL += " AND (SERIE='" + _NSerie.Trim + "')"
        End If
        '--------------------------------------------------------------------------------------
        Dim ds As New DataSet
        Try
            CKAttEvAllaData = False
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    'giu080622 per  ev.parz attivita  nel periodo 'giu110622 evase solo nell'anno della scadenza
                    If ds.Tables(0).Select("Qta_Evasa<>0 AND DataSc='" + Format(_DataScPag, FormatoData) + "'").Length > 0 Then
                        CKAttEvAllaData = True
                    ElseIf ds.Tables(0).Select("Qta_Evasa<>0 AND AnnoDataSc=" + myAnnoDataSc.Trim + "").Length > 0 Then
                        CKAttEvAllaData = True
                    Else
                        CKAttEvAllaData = False
                    End If
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            CKAttEvAllaData = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errrore", "Verifica in CKAttEvAllaData: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End Try
    End Function
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CLng(myID) < 0 Then 'GIU191223
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(SWOP) = SWOPNESSUNA
        Session(CSTNUOVOCADAOC) = SWNO
        Session(CSTNUOVOCADACA) = SWNO
        Session(CALLGESTIONE) = SWSI
        Response.Redirect("WF_Contratti.aspx?labelForm=Gestione CONTRATTI")
    End Sub

    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer, ByRef strErrore As String) As Boolean
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        ' ''Dim strErrore As String = ""
        strErrore = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbUpdCmd As New SqlCommand

        SqlDbUpdCmd.CommandText = "[Update_NDocPargen]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWOp", System.Data.SqlDbType.VarChar, 1))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N" 'NUOVO NUMERO
        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = TDoc
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                AggiornaNumDoc = False
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore aggiorna numero documento", "NUOVO IDENTIFICATIVO DOCUMENTO GIA' PRESENTE.", WUC_ModalPopup.TYPE_ERROR)
                strErrore = "(AggNDoc) ERRORE, N° Documento già presente."
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                AggiornaNumDoc = False
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore aggiorna numero documento", "NUOVO TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                strErrore = "(AggNDoc) ERRORE, Tipo Documento sconosciuto per assegnare un nuovo N°."
            End If
        Catch ExSQL As SqlException
            AggiornaNumDoc = False
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "(AggNDoc) ERRORE SQL, " + ExSQL.Message.Trim
        Catch Ex As Exception
            AggiornaNumDoc = False
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "(AggNDoc) ERRORE, " + Ex.Message.Trim
        End Try
    End Function

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        SetLnk()
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        aDataViewScPagAtt = Session(sessDataViewScAtt)
        GridViewPrevT.DataSource = aDataViewScPagAtt
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                Session(DATADOCCOLL) = row.Cells(CellIdxT.DataSc1).Text.Trim
                Session(CSTSERIELOTTO) = row.Cells(CellIdxT.Serie).Text.Trim
                Session("SelInRata") = row.Cells(CellIdxT.DesRata).Text.Trim
                BtnSetByStato(Stato)
                '-
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnSelSc.Enabled = False
            btnDeselSc.Enabled = False
            btnEmFattSc.Enabled = False
            btnStFattScEm.Enabled = False
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        SetLnk()
        Session(DATADOCCOLL) = ""
        Session(CSTSERIELOTTO) = ""
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            Session(DATADOCCOLL) = row.Cells(CellIdxT.DataSc1).Text.Trim
            Session(CSTSERIELOTTO) = row.Cells(CellIdxT.Serie).Text.Trim
            Session("SelInRata") = row.Cells(CellIdxT.DesRata).Text.Trim
            BtnSetByStato(Stato)
            chkCTRFatturato.Checked = False
            DDLCTRFatturato.Enabled = False
            DDLCTRFatturato.Items.Clear()
            DDLCTRFatturato.Items.Add("")
            BuildDettD()
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleF1).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleF1).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleF1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleF1).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleF1).Text = ""
                End If
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataSc1).Text) Then
                e.Row.Cells(CellIdxT.DataSc1).Text = Format(CDate(e.Row.Cells(CellIdxT.DataSc1).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.Rata1).Text) Then
                If CDec(e.Row.Cells(CellIdxT.Rata1).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.Rata1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.Rata1).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.Rata1).Text = ""
                End If
            End If
            If e.Row.Cells(CellIdxT.TotaleF1).Text <> e.Row.Cells(CellIdxT.Rata1).Text And IsNumeric(e.Row.Cells(CellIdxT.TotaleF1).Text) Then
                e.Row.Cells(CellIdxT.TotaleF1).ForeColor = Drawing.Color.White
                e.Row.Cells(CellIdxT.TotaleF1).BackColor = Drawing.Color.DarkRed
            Else
                e.Row.Cells(CellIdxT.TotaleF1).ForeColor = e.Row.Cells(CellIdxT.Rata1).ForeColor
                e.Row.Cells(CellIdxT.TotaleF1).BackColor = e.Row.Cells(CellIdxT.Rata1).BackColor
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.CKAttEvasa).Text) Then
                If CDec(e.Row.Cells(CellIdxT.CKAttEvasa).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.CKAttEvasa).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.CKAttEvasa).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.CKAttEvasa).Text = ""
                End If
            ElseIf e.Row.Cells(CellIdxT.CKAttEvasa).Text.Trim.ToUpper = "TRUE" Then
                e.Row.Cells(CellIdxT.CKAttEvasa).Text = ""
            Else
                e.Row.Cells(CellIdxT.CKAttEvasa).Text = "!!NO!!"
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
            If Len(e.Row.Cells(CellIdxT.RagSoc).Text.Trim) > 30 Then
                e.Row.Cells(CellIdxT.RagSoc).Text = Mid(e.Row.Cells(CellIdxT.RagSoc).Text, 1, 30)
            End If
            If Len(e.Row.Cells(CellIdxT.Denom).Text.Trim) > 30 Then
                e.Row.Cells(CellIdxT.Denom).Text = Mid(e.Row.Cells(CellIdxT.Denom).Text, 1, 30)
            End If
            If Len(e.Row.Cells(CellIdxT.Riferimento).Text.Trim) > 30 Then
                e.Row.Cells(CellIdxT.Riferimento).Text = Mid(e.Row.Cells(CellIdxT.Riferimento).Text, 1, 30)
            End If
        End If
    End Sub
    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            Dim mySelInRataT As String = ""
            Dim mySelInRataD As String = ""
            Try
                mySelInRataD = e.Row.Cells(CellIdxD.SelInRata).Text.Trim
                If mySelInRataD.Trim = HTML_SPAZIO Then
                    mySelInRataD = ""
                End If
                mySelInRataT = Session("SelInRata").ToString.Trim
                If mySelInRataT.Trim = HTML_SPAZIO Then
                    mySelInRataT = ""
                End If
                If mySelInRataT <> "" And mySelInRataD <> "" Then
                    If mySelInRataT <> mySelInRataD Then
                        e.Row.Cells(CellIdxD.Sel).Enabled = False
                    Else
                        e.Row.Cells(CellIdxD.Sel).Enabled = True
                    End If
                Else
                    e.Row.Cells(CellIdxD.Sel).Enabled = True
                End If
            Catch ex As Exception
                mySelInRataT = ""
                mySelInRataD = ""
            End Try
            '-
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaIn).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaIn).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaIn).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaIn).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaIn).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaFa).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaFa).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaFa).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaFa).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaFa).Text = ""
                End If
            End If
            If Len(e.Row.Cells(CellIdxD.Filiale).Text.Trim) > 5 Then
                e.Row.Cells(CellIdxD.Filiale).Text = Mid(e.Row.Cells(CellIdxD.Filiale).Text, 1, 5)
            End If
            If IsDate(e.Row.Cells(CellIdxD.DataSc).Text) Then
                e.Row.Cells(CellIdxD.DataSc).Text = Format(CDate(e.Row.Cells(CellIdxD.DataSc).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxD.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub

    Private Sub btnVerbale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerbale.Click
        SetLnk()
        Session(CSTTASTOST) = btnVerbale.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Verbale
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 1
                Session(CSTNOBACK) = 0 'giu040512
                Dim SWSviluppo As String = ConfigurationManager.AppSettings("sviluppo")
                If Not String.IsNullOrEmpty(SWSviluppo) Then
                    If SWSviluppo.Trim.ToUpper = "TRUE" Then
                        Session(CALLGESTIONE) = SWSI
                        Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                        Exit Sub
                    End If
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnStampa.Click
        SetLnk()
        Session(CSTTASTOST) = btnStampa.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.Proforma
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 'giu040512
                Dim SWSviluppo As String = ConfigurationManager.AppSettings("sviluppo")
                If Not String.IsNullOrEmpty(SWSviluppo) Then
                    If SWSviluppo.Trim.ToUpper = "TRUE" Then
                        Session(CALLGESTIONE) = SWSI
                        Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                        Exit Sub
                    End If
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub
    '-
    Private Sub btnElencoSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElencoSc.Click
        SetLnk()
        Session("TipoDocInStampa") = SWTD(TD.ContrattoAssistenza)
        Session(CSTTASTOST) = btnElencoSc.ID
        Session(CSTTIPOELENCOSCATT) = TIPOELENCOSCATT.ScadAttivitaPag
        '''chkVisElenco.Visible = False
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
            txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
            txtDallaData.Focus()
            Exit Sub
        End If
        txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
        If checkCausale.Checked = True And DDLCausali.SelectedIndex < 1 Then
            DDLCausali.BackColor = SEGNALA_KO : DDLCausali.Focus()
            Exit Sub
        End If
        If checkRespArea.Checked = True And DDLRespArea.SelectedIndex < 1 Then
            DDLRespArea.BackColor = SEGNALA_KO : DDLRespArea.Focus()
            Exit Sub
        End If
        If CheckRespVisite.Checked = True And DDLRespVisite.SelectedIndex < 1 Then
            DDLRespVisite.BackColor = SEGNALA_KO : DDLRespVisite.Focus()
            Exit Sub
        End If
        If chkTutteRegioni.Checked = False And ddlRegioni.SelectedIndex < 1 Then
            ddlRegioni.BackColor = SEGNALA_KO : ddlRegioni.Focus()
            Exit Sub
        End If
        If chkTutteProvince.Checked = False And ddlProvince.SelectedIndex < 1 Then
            ddlProvince.BackColor = SEGNALA_KO : ddlProvince.Focus()
            Exit Sub
        End If
        '---------
        If GridViewPrevT.Rows.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare. Ricaricare i dati con il tasto ""Visualizza Scadenze""", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If btnRicerca.BackColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato da stampare. Ricaricare i dati con il tasto ""Visualizza Scadenze""", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session("dsDocElencoScadPag") Is Nothing Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione dati scaduta, ricaricare i dati e riprovare.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSDocumenti
        DsPrinWebDoc = Session("dsDocElencoScadPag")
        '-------------------------------------------------------
        Session(CSTDsPrinWebDoc) = DsPrinWebDoc
        Try
            If chkVisElenco.Checked = False Then
                Session(CSTNOBACK) = 0
                'giu160523 emailIlaria
                Session(CALLGESTIONE) = SWSI
                Response.Redirect("..\WebFormTables\WF_PrintWebCA.aspx?labelForm=Stampa Elenco Scadenze")
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampaElScadCA(DsPrinWebDoc)
    End Sub
    '----------
    Public Sub Chiudi(ByVal strErrore As String)

        Try
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=" & strErrore)
        Catch ex As Exception
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=" & strErrore)
        End Try

    End Sub
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CLng(myID) < 0 Then 'GIU191223
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegatiCM()
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
#End Region

    'GIU210120
    Private Sub OKApriStampa(ByRef DsPrinWebDoc As DSPrintWeb_Documenti)
        ' ''If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
        ' ''    If Session(CSTNOBACK) = 1 Then
        ' ''        btnRitorno.Visible = False
        ' ''        Label1.Visible = False
        ' ''    End If
        ' ''End If
        Dim SWSconti As Integer = 1
        If Not String.IsNullOrEmpty(Session(CSTSWScontiDoc)) Then
            If IsNumeric(Session(CSTSWScontiDoc)) Then
                SWSconti = Session(CSTSWScontiDoc)
            End If
        End If
        '-
        Dim SWConfermaDoc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWConfermaDoc)) Then
            If IsNumeric(Session(CSTSWConfermaDoc)) Then
                SWConfermaDoc = Session(CSTSWConfermaDoc)
            End If
        End If
        'giu110319
        Dim SWRitAcc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWRitAcc)) Then
            If IsNumeric(Session(CSTSWRitAcc)) Then
                SWRitAcc = Session(CSTSWRitAcc)
            End If
        End If
        '---------
        Dim Rpt As Object = Nothing
        ' ''Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        ' ''DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ContrattiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu120319
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Select("RitAcconto=true").Count > 0) Then
                Session(CSTSWRitAcc) = 1
                SWRitAcc = 1
            Else
                Session(CSTSWRitAcc) = 0
                SWRitAcc = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("ContrattiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
            NomeStampa = "PREVOFF.PDF"
            SubDirDOC = "Preventivi"
            If SWSconti = 1 Then
                Rpt = New Preventivo
                If CodiceDitta = "01" Then
                    Rpt = New Preventivo01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Preventivo05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Preventivo0501
                End If
            Else
                Rpt = New PreventivoNOSconti
                If CodiceDitta = "01" Then
                    Rpt = New PreventivoNOSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New PreventivoNOSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New PreventivoNOSconti0501
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
            NomeStampa = "ORDINE.PDF"
            SubDirDOC = "Ordini"
            If SWSconti = 1 Then
                Rpt = New Ordine
                If CodiceDitta = "01" Then
                    Rpt = New Ordine01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Ordine05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Ordine0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New Ordine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New Ordine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New Ordine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New Ordine0501
                '''    End If
                '''Else
                '''    NomeStampa = "CONFORDINE.PDF"
                '''    Rpt = New ConfermaOrdine
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdine01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdine05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdine0501
                '''    End If
                '''End If
            Else
                Rpt = New OrdineNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New OrdineNoSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New OrdineNoSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New OrdineNoSconti0501
                End If
                '''If SWConfermaDoc = 0 Then
                '''    Rpt = New OrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New OrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New OrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New OrdineNoSconti0501
                '''    End If
                '''Else
                '''    NomeStampa = "CONFORDINE.PDF"
                '''    Rpt = New ConfermaOrdineNoSconti
                '''    If CodiceDitta = "01" Then
                '''        Rpt = New ConfermaOrdineNoSconti01
                '''    ElseIf CodiceDitta = "05" Then
                '''        Rpt = New ConfermaOrdineNoSconti05
                '''    ElseIf CodiceDitta = "0501" Then
                '''        Rpt = New ConfermaOrdineNoSconti0501
                '''    End If
                '''End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
            SubDirDOC = "DDTClienti"
            NomeStampa = "DDTCLIENTE.PDF"
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                Rpt = New DDTNoPrezzi05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
            SubDirDOC = "DDTFornit"
            NomeStampa = "DDTFORNIT.PDF"
            Rpt = New DDTNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New DDTNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                Rpt = New DDTNoPrezzi05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New DDTNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
            NomeStampa = "FATTURA.PDF"
            SubDirDOC = "Fatture"
            'giu251211
            If SWSconti = 1 Then
                Rpt = New Fattura
                If CodiceDitta = "01" Then
                    Rpt = New Fattura01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Fattura05
                    If SWRitAcc <> 0 Then
                        Rpt = New Fattura05RA
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Fattura0501
                    If SWRitAcc <> 0 Then
                        Rpt = New Fattura05RA
                    End If
                End If
            Else
                Rpt = New FatturaNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New FatturaNoSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New FatturaNoSconti05
                    If SWRitAcc <> 0 Then
                        Rpt = New Fattura05RA
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New FatturaNoSconti0501
                    If SWRitAcc = True <> 0 Then
                        Rpt = New Fattura05RA
                    End If
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
            NomeStampa = "NOTACREDITO.PDF"
            SubDirDOC = "NoteCredito"
            Rpt = New NotaCredito
            If CodiceDitta = "01" Then
                Rpt = New NotaCredito01
            ElseIf CodiceDitta = "05" Then
                Rpt = New NotaCredito05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New NotaCredito0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
            NomeStampa = "ORDINEFOR.PDF"
            SubDirDOC = "Ordini"
            Rpt = New OrdineFornitore
            If CodiceDitta = "01" Then
                Rpt = New OrdineFornitore01
            ElseIf CodiceDitta = "05" Then
                Rpt = New OrdineFornitore05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New OrdineFornitore0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
            NomeStampa = "MOVMAG.PDF"
            SubDirDOC = "MovMag"
            Rpt = New MMNoPrezzi
            If CodiceDitta = "01" Then
                Rpt = New MMNoPrezzi01
            ElseIf CodiceDitta = "05" Then
                Rpt = New MMNoPrezzi05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New MMNoPrezzi0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            If Session(CSTTASTOST) = btnStampa.ID Then
                NomeStampa = "PROFORMACA.PDF"
                Rpt = New ProformaCA05 'Contratti
                If CodiceDitta = "01" Then
                    Rpt = New ProformaCA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ProformaCA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ProformaCA05 '0501
                End If
            ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
                NomeStampa = "VERBALE.PDF"
                Rpt = New VerbaleVACA05
                If CodiceDitta = "01" Then
                    Rpt = New VerbaleVACA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New VerbaleVACA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New VerbaleVACA05 '0501
                End If
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
            Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale STAMPA DOCUMENTO DA COMPLETARE"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        Else
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'GIU210120 ESEGUITO IN CKSTTipoDocST
        'Per evitare che solo un utente possa elaborare le stampe
        ' ''Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        ' ''If (Utente Is Nothing) Then
        ' ''    Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
        ' ''    Exit Sub
        ' ''End If
        Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
        '---------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
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
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        'giu140615 Dim LnkName As String = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/" & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True
        ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            LnkVerbale.Visible = True
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True
        End If

        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.HRef = LnkName
        ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
            LnkVerbale.HRef = LnkName
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.HRef = LnkName
        Else
            LnkStampa.HRef = LnkName
        End If
    End Sub
    'giu120520
    Private Sub OKApriStampaElScadCA(ByRef DsPrinWebDoc As DSDocumenti)

        Dim SWTabCliFor As String = ""
        Dim Rpt As Object = Nothing
        Try
            If (DsPrinWebDoc.Tables("ScadAttPag").Rows.Count > 0) Then
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("ScadAttPag").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica dati Testata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        If Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza) Or _
                Session(CSTTIPODOC) = SWTD(TD.TipoContratto) Then
            SubDirDOC = "Contratti"
            If Session(CSTTASTOST) = btnElencoSc.ID Then
                If chkVisElenco.Checked Then
                    NomeStampa = "ELENCOSCADPAG.XLS"
                Else
                    NomeStampa = "ELENCOSCADPAG.PDF"
                End If
                '-
                Rpt = New ElencoScadPagCA05 'Contratti
                If CodiceDitta = "01" Then
                    Rpt = New ElencoScadPagCA05 '01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New ElencoScadPagCA05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New ElencoScadPagCA05 '0501
                End If
            ElseIf Session(CSTTASTOST) = btnVerbale.ID Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Tipo stampa non definita", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If
        'ok
        '-----------------------------------
        Rpt.SetDataSource(DsPrinWebDoc)
        'riporto il nome del RESP.AREA nel nome del file creato
        Dim strRespArea As String = DDLRespArea.SelectedItem.Text.Trim
        strRespArea = strRespArea.ToString.Replace(",", " ")
        strRespArea = strRespArea.ToString.Replace(".", " ")
        Dim strDalAl As String = txtDallaData.Text + "_" + txtAllaData.Text
        strDalAl = strDalAl.ToString.Replace("/", "")
        Session(CSTNOMEPDF) = IIf(strRespArea.Trim <> "", strRespArea.Trim & "_", "") & strDalAl.Trim & "_" & InizialiUT.Trim & NomeStampa.Trim
        '---------
        'giu140615 prova con binary 
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            If chkVisElenco.Checked = False Then
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
        If Session(CSTTASTOST) = btnElencoSc.ID Then
            LnkElencoSc.Visible = True
        Else
            LnkElencoSc.Visible = True
        End If

        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnElencoSc.ID Then
            LnkElencoSc.HRef = LnkName
        Else
            LnkElencoSc.HRef = LnkName
        End If
    End Sub
    Public Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocST = True
        Session(CSTTIPODOC) = SWTD(TD.ContrattoAssistenza)
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim 'GIU210120
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        CodiceDitta = Session(CSTCODDITTA)
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        If IsNothing(CodiceDitta) Then
            Return False
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Return False
        End If
        If CodiceDitta = "" Then
            Return False
        End If
        '-------------------------------------------------------------------
    End Function

    Private Sub checkRespArea_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkRespArea.CheckedChanged
        SvuodaGridT()
        SetLnk()
        If checkRespArea.Checked = True Then
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = True : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
        Else
            DDLRespArea.AutoPostBack = False : DDLRespArea.Enabled = False : DDLRespArea.SelectedIndex = 0 : DDLRespArea.AutoPostBack = True
            DDLRespArea.BackColor = SEGNALA_OK
        End If
        Session(IDRESPAREA) = 0
        Call DataBindRV()
        If checkRespArea.Checked = True Then
            If DDLRespArea.Items.Count > 0 Then 'GIU280223
                DDLRespArea.SelectedIndex = 1
                Session(IDRESPAREA) = DDLRespArea.SelectedValue
                PosizionaItemDDL(Session(IDRESPAREA).ToString.Trim, DDLRespArea)
                Call DDLRespArea_SelectedIndexChanged(Nothing, Nothing)
            End If
            DDLRespArea.Focus()
        End If
    End Sub
    Private Sub CheckRespVisite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckRespVisite.CheckedChanged
        SvuodaGridT()
        SetLnk()
        If CheckRespVisite.Checked = True Then
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = True : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
        Else
            DDLRespVisite.AutoPostBack = False : DDLRespVisite.Enabled = False : DDLRespVisite.SelectedIndex = 0 : DDLRespVisite.AutoPostBack = True
            DDLRespVisite.BackColor = SEGNALA_OK
        End If
        If CheckRespVisite.Checked = True Then
            If DDLRespVisite.Items.Count > 0 Then 'GIU280223
                DDLRespVisite.SelectedIndex = 1
                Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
                PosizionaItemDDL(Session(IDRESPVISITE).ToString.Trim, DDLRespVisite)
                Call DDLRespVisite_SelectedIndexChanged(Nothing, Nothing)
            End If
            DDLRespVisite.Focus()
        End If
    End Sub
    Private Sub DDLRespArea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespArea.SelectedIndexChanged
        SvuodaGridT()
        If DDLRespArea.SelectedIndex = 0 Or DDLRespArea.SelectedItem.Text.Trim = "" Then
            Session(IDRESPAREA) = 0
        Else
            Session(IDRESPAREA) = DDLRespArea.SelectedValue
        End If
        Call DataBindRV()
        '--
        If DDLRespArea.SelectedIndex = 0 Then
            DDLRespArea.BackColor = SEGNALA_KO
        Else
            DDLRespArea.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub DataBindRV()
        SqlDSRespVisite.DataBind()
        DDLRespVisite.Items.Clear()
        DDLRespVisite.Items.Add("")
        DDLRespVisite.DataBind()
        DDLRespVisite.BackColor = SEGNALA_OK
        '-- mi riposiziono 
        DDLRespVisite.AutoPostBack = False
        DDLRespVisite.SelectedIndex = -1
        DDLRespVisite.AutoPostBack = True
        If CheckRespVisite.Checked = True Then
            If DDLRespVisite.Items.Count > 0 Then 'GIU280223
                DDLRespVisite.SelectedIndex = 1
                Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
                PosizionaItemDDL(Session(IDRESPVISITE).ToString.Trim, DDLRespVisite)
                Call DDLRespVisite_SelectedIndexChanged(Nothing, Nothing)
            End If
            '' 'DDLRespVisite.Focus()
        End If
    End Sub
    Private Sub DDLRespVisite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespVisite.SelectedIndexChanged
        SvuodaGridT()
        Session(IDRESPVISITE) = DDLRespVisite.SelectedValue
        If DDLRespVisite.SelectedIndex = 0 Then
            DDLRespVisite.BackColor = SEGNALA_KO
        Else
            DDLRespVisite.BackColor = SEGNALA_OK
        End If
    End Sub
    '-
    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        SvuodaGridT()
        Session("CodRegione") = ddlRegioni.SelectedValue
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
        If ddlRegioni.SelectedIndex = 0 Then
            ddlRegioni.BackColor = SEGNALA_KO
        Else
            ddlRegioni.BackColor = SEGNALA_OK
        End If
    End Sub
    Private Sub chkTutteRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        SvuodaGridT()
        If chkTutteRegioni.Checked Then
            ddlRegioni.Enabled = False
            ddlRegioni.SelectedIndex = -1
            ddlRegioni.BackColor = SEGNALA_OK
            Session("CodRegione") = 0
        Else
            ddlRegioni.Enabled = True
            Session("CodRegione") = ddlRegioni.SelectedValue
            If ddlRegioni.Items.Count > 0 Then 'GIU280223
                ddlRegioni.SelectedIndex = 1
                Session("CodRegione") = ddlRegioni.SelectedValue
                PosizionaItemDDL(Session("CodRegione").ToString.Trim, ddlRegioni)
                Call ddlRegioni_SelectedIndexChanged(Nothing, Nothing)
            End If
            ddlRegioni.Focus()
        End If
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
            ddlProvince.BackColor = SEGNALA_OK
        Else
            ddlProvince.Enabled = True
            If ddlProvince.Items.Count > 0 Then 'GIU280223
                ddlProvince.SelectedIndex = 1
                PosizionaItemDDL(ddlProvince.SelectedValue.ToString.Trim, ddlProvince)
                Call ddlProvince_SelectedIndexChanged(Nothing, Nothing)
            End If
        End If
    End Sub
    Private Sub chkTutteProvince_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteProvince.CheckedChanged
        SvuodaGridT()
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
            ddlProvince.BackColor = SEGNALA_OK
        Else
            ddlProvince.Enabled = True
            If ddlProvince.Items.Count > 0 Then 'GIU280223
                ddlProvince.SelectedIndex = 1
                PosizionaItemDDL(ddlProvince.SelectedValue.ToString.Trim, ddlProvince)
                Call ddlProvince_SelectedIndexChanged(Nothing, Nothing)
            End If
            ddlProvince.Focus()
        End If
    End Sub

    Private Sub ddlProvince_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProvince.SelectedIndexChanged
        SvuodaGridT()
        If ddlProvince.SelectedIndex = 0 Then
            ddlProvince.BackColor = SEGNALA_KO
        Else
            ddlProvince.BackColor = SEGNALA_OK
        End If
    End Sub

    'Private Sub txtDaAllaData_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDallaData.TextChanged, txtAllaData.TextChanged
    '    SvuodaGridT()
    '    If IsDate(txtDallaData.Text.Trim) Then
    '        txtDallaData.BackColor = SEGNALA_OK
    '        txtDallaData.Text = Format(CDate(txtDallaData.Text), FormatoData)
    '    Else
    '        txtDallaData.BackColor = SEGNALA_KO
    '        txtDallaData.Text = ""
    '    End If
    '    If IsDate(txtAllaData.Text.Trim) Then
    '        txtAllaData.BackColor = SEGNALA_OK
    '        txtAllaData.Text = Format(CDate(txtAllaData.Text), FormatoData)
    '    Else
    '        txtAllaData.BackColor = SEGNALA_KO
    '        txtAllaData.Text = ""
    '    End If
    '    If Not IsDate(txtDallaData.Text) Or Not IsDate(txtAllaData.Text) Then
    '        txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
    '        txtDallaData.Focus()
    '        Exit Sub
    '    ElseIf CDate(txtDallaData.Text) > CDate(txtAllaData.Text) Then
    '        txtDallaData.BackColor = SEGNALA_KO : txtAllaData.BackColor = SEGNALA_KO
    '        txtDallaData.Focus()
    '        Exit Sub
    '    End If
    '    txtDallaData.BackColor = SEGNALA_OK : txtAllaData.BackColor = SEGNALA_OK
    '    If sender.id = txtDallaData.ID Then
    '        txtAllaData.Focus()
    '    End If
    'End Sub

    'Private Sub txtRicerca_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicerca.TextChanged
    '    SvuodaGridT()
    'End Sub

    Private Sub SetLnk()
        LnkStampa.Visible = False : LnkVerbale.Visible = False : LnkElencoSc.Visible = False ': LnkFatture.Visible = False
    End Sub

    Protected Sub btnCercaAnagrafica1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCercaAnagrafica1.Click

        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()

    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        SvuodaGridT()
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            lblDesCliente.ToolTip = codice
            lblDesCliente.Text = descrizione
        ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
            lblDesCliente.ToolTip = codice
            lblDesCliente.Text = descrizione
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

    Private Sub btnNoCliente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNoCliente.Click
        If lblDesCliente.Text.Trim = "" Or lblDesCliente.ToolTip.Trim = "" Then
            lblDesCliente.Text = "" : lblDesCliente.ToolTip = ""
            Exit Sub
        End If
        lblDesCliente.Text = "" : lblDesCliente.ToolTip = ""
        SvuodaGridT()
    End Sub

    Private Sub btnDeselScSelSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeselSc.Click, btnSelSc.Click

        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        Try
            aDataViewScPagAtt = Session(sessDataViewScAtt)
            aDataViewScPagAttD = Session(sessDataViewScAttD) 'giu280321
            'giu280520 
            If (aDataViewScPagAtt Is Nothing) Then
                SvuodaGridT()
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di ricaricare i dati.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            Dim myAnnoDataSc As String = ""
            Dim myNSerie As String = ""
            Dim myTotImponF1 As Decimal = 0
            Dim myTotIVAF1 As Decimal = 0
            Dim myRegimeIVA As Integer = 0
            Dim mySplitIVA As Boolean = False
            Dim mySelInRata As String = ""
            For i = 0 To aDataViewScPagAtt.Count - 1
                myTotImponF1 = 0
                myTotIVAF1 = 0
                aDataViewScPagAtt.Item(i).Item("TotImponF1") = 0
                aDataViewScPagAtt.Item(i).Item("TotIVAF1") = 0
                aDataViewScPagAtt.Item(i).Item("TotaleF1") = 0
                aDataViewScPagAtt.BeginInit()
                If sender.id = btnDeselSc.ID Then
                    aDataViewScPagAtt.Item(i)("Sel") = 0
                Else
                    aDataViewScPagAtt.Item(i)("Sel") = 1
                End If
                myRegimeIVA = aDataViewScPagAtt.Item(i)("Regime_IVA")
                mySplitIVA = CBool(aDataViewScPagAtt.Item(i)("SplitIVA").ToString.Trim)
                'GIU221123 SE SELEZIONO ANNI DIVERSI MA NON TUTTE LE ATTIVITA'
                mySelInRata = aDataViewScPagAtt.Item(i)("DesRata")
                If String.IsNullOrEmpty(mySelInRata) Then
                    mySelInRata = ""
                Else
                    If mySelInRata.Trim = HTML_SPAZIO Then
                        mySelInRata = ""
                    End If
                End If
                '--------
                'giu280321
                If Not (aDataViewScPagAttD Is Nothing) Then
                    aDataViewScPagAttD.RowFilter = "IDDocumenti=" + aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) "
                    If sender.id = btnDeselSc.ID Then
                        aDataViewScPagAttD.BeginInit()
                        For ii = 0 To aDataViewScPagAttD.Count - 1
                            aDataViewScPagAttD.Item(ii)("Sel") = 0
                            aDataViewScPagAttD.Item(ii).Item("SelInRata") = ""
                            '.........
                            aDataViewScPagAttD.EndInit()
                        Next
                    Else
                        myNSerie = aDataViewScPagAtt.Item(i)("Serie")
                        If myNSerie.Trim = HTML_SPAZIO Then
                            myNSerie = ""
                        End If
                        If String.IsNullOrEmpty(myNSerie) Then myNSerie = ""
                        myAnnoDataSc = Format(aDataViewScPagAtt.Item(i)("DataSc1"), "yyyy")
                        If String.IsNullOrEmpty(myAnnoDataSc) Then myAnnoDataSc = ""
                        If myAnnoDataSc.Trim <> "" Then
                            aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (SelInRata='" + mySelInRata + "'))"
                        Else
                            aDataViewScPagAttD.RowFilter += " AND (SelInRata='" + mySelInRata + "')"
                        End If
                        '-
                        If chkSoloEvase.Checked = True Then
                            aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                        End If
                        If myNSerie.Trim <> "" Then
                            aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                        End If
                        '---------------------------------------------------
                        For ii = 0 To aDataViewScPagAttD.Count - 1
                            aDataViewScPagAttD.BeginInit()
                            aDataViewScPagAttD.Item(ii).Item("SelInRata") = mySelInRata
                            aDataViewScPagAttD.Item(ii)("Sel") = 1
                            'GIU191123
                            If myRegimeIVA > 49 Then 'TOTALE CON IVA 0 quindi IVA 0
                                myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                            Else
                                myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                                myTotIVAF1 += (aDataViewScPagAttD.Item(ii)("Importo") / 100) * aDataViewScPagAttD.Item(ii)("Cod_IVA")
                            End If
                            '.........
                            aDataViewScPagAttD.EndInit()
                        Next
                    End If
                End If
                '---------
                If sender.id = btnDeselSc.ID Then
                    myTotImponF1 = 0
                    myTotIVAF1 = 0
                End If
                aDataViewScPagAtt.Item(i).Item("TotImponF1") = myTotImponF1
                aDataViewScPagAtt.Item(i).Item("TotIVAF1") = myTotIVAF1
                If mySplitIVA = True Then 'il TOTALE RATA E' SENZA IVA: calcolo IVA per determinare il totale
                    aDataViewScPagAtt.Item(i).Item("TotaleF1") = myTotImponF1
                Else
                    aDataViewScPagAtt.Item(i).Item("TotaleF1") = myTotImponF1 + myTotIVAF1
                End If
                '
                If myTotImponF1 = 0 Then
                    aDataViewScPagAtt.Item(i)("Sel") = 0
                End If
                aDataViewScPagAtt.EndInit()
            Next
            aDataViewScPagAtt.RowFilter = ""
            Session(sessDataViewScAtt) = aDataViewScPagAtt
            aDataViewScPagAttD.RowFilter = ""
            Session(sessDataViewScAttD) = aDataViewScPagAttD
            If sender.id = btnDeselSc.ID Then
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = False
                btnEmFattSc.Enabled = False
                btnStFattScEm.Enabled = False
            Else
                btnEmFattSc.Enabled = True
                btnSelSc.Enabled = False
                btnDeselSc.Enabled = True
            End If
        Catch ex As Exception
            SvuodaGridT()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di ricaricare i dati.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '@@@@@@@
        Try
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = "DataSc1,Cod_Cliente" 'giu030223
        Catch ex As Exception
            'ok
        End Try
        GridViewPrevT.DataSource = aDataViewScPagAtt
        GridViewPrevT.DataBind()
        'giu290321
        GridViewPrevD.DataSource = aDataViewScPagAttD
        GridViewPrevD.DataBind()
        '---------
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnSelSc.Enabled = False
            btnDeselSc.Enabled = False
            btnEmFattSc.Enabled = False
            btnStFattScEm.Enabled = False
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Protected Sub chkSel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Try
            'selected row gridview Testata
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            GridViewPrevT.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            '---
            Dim SaveFilter As String = aDataViewScPagAtt.RowFilter
            Dim SaveSort As String = aDataViewScPagAtt.Sort
            '-
            aDataViewScPagAtt = Session(sessDataViewScAtt)
            If myRowIndex = -1 Then
                sender.checked = False
                Exit Sub
            End If
            '---
            'GIU020223 giu191123 aspetto l'importo selezionato
            '''aDataViewScPagAtt.RowFilter = ""
            '''Session(sessDataViewScAtt) = aDataViewScPagAtt
            '''GridViewPrevT.DataSource = aDataViewScPagAtt
            '''GridViewPrevT.EditIndex = -1
            '''GridViewPrevT.DataBind()
            ''''----------
            aDataViewScPagAtt.BeginInit()
            'se flag  = true allora il flag attivo passa a False
            If sender.checked = True Then
                aDataViewScPagAtt.Item(myRowIndex).Item("Sel") = 1
                btnDeselSc.Enabled = True
            Else
                aDataViewScPagAtt.Item(myRowIndex).Item("Sel") = 0
            End If
            'giu191123
            Dim myRegimeIVA As Integer = aDataViewScPagAtt.Item(myRowIndex)("Regime_IVA")
            Dim mySplitIVA As Boolean = CBool(aDataViewScPagAtt.Item(myRowIndex)("SplitIVA").ToString.Trim)
            aDataViewScPagAtt.Item(myRowIndex).Item("TotImponF1") = 0
            aDataViewScPagAtt.Item(myRowIndex).Item("TotIVAF1") = 0
            aDataViewScPagAtt.Item(myRowIndex).Item("TotaleF1") = 0
            Dim myTotImponF1 As Decimal = 0
            Dim myTotIVAF1 As Decimal = 0
            '-----------
            'giu210721
            Dim myIDDocumenti As String = aDataViewScPagAtt.Item(myRowIndex)("IDDocumenti").ToString.Trim
            Dim myAnnoDataSc As String = Format(aDataViewScPagAtt.Item(myRowIndex)("DataSc1"), "yyyy")
            Dim myDataSc As String = Format(aDataViewScPagAtt.Item(myRowIndex)("DataSc1"), FormatoData)
            Dim myNSerie As String = aDataViewScPagAtt.Item(myRowIndex)("Serie").ToString.Trim 'giu011123
            If String.IsNullOrEmpty(myNSerie) Then
                myNSerie = ""
            Else
                If myNSerie.Trim = HTML_SPAZIO Then
                    myNSerie = ""
                End If
            End If
            '---------
            'GIU221123 SE SELEZIONO ANNI DIVERSI MA NON TUTTE LE ATTIVITA'
            Dim mySelInRata As String = aDataViewScPagAtt.Item(myRowIndex)("DesRata").ToString.Trim
            If String.IsNullOrEmpty(mySelInRata) Then
                mySelInRata = ""
            Else
                If mySelInRata.Trim = HTML_SPAZIO Then
                    mySelInRata = ""
                End If
            End If
            '--------
            'giu191123 aspetto l'importo selezionato aDataViewScPagAtt.EndInit()
            'giu280321
            aDataViewScPagAttD = Session(sessDataViewScAttD)
            If Not (aDataViewScPagAttD Is Nothing) Then
                aDataViewScPagAttD.RowFilter = "IDDocumenti=" + myIDDocumenti + " AND (ISNULL(Qta_Fatturata,0)=0) "
                'giu221123 per evitare di riselezionare tutto - ognuno il suo
                If String.IsNullOrEmpty(myAnnoDataSc) Then myAnnoDataSc = ""
                If sender.checked = True Then
                    If myAnnoDataSc.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0))"
                    Else
                        aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0))"
                    End If
                Else
                    If myAnnoDataSc.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR (SelInRata='" + mySelInRata + "') )"
                    Else
                        aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) or (SelInRata='" + mySelInRata + "') )"
                    End If
                End If
                '----------------------------------------------------------
                '''If chkVisALLSc.Checked = False Then
                '''    If myAnnoDataSc.Trim <> "" Then
                '''        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR (SelInRata='" + mySelInRata + "' OR SelInRata='') )"
                '''    Else
                '''        aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) or (SelInRata='" + mySelInRata + "' OR SelInRata='') )"
                '''    End If
                '''Else
                '''    aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR (SelInRata='" + mySelInRata + "' OR SelInRata='') )"
                '''End If
                '-
                If chkSoloEvase.Checked = True Then
                    aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                End If
                If myNSerie.Trim <> "" Then
                    aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                End If
                '---------
                Try
                    For ii = 0 To aDataViewScPagAttD.Count - 1
                        If aDataViewScPagAttD.Item(ii)("SelInRata").ToString.Trim <> "" And mySelInRata <> "" Then
                            If mySelInRata <> aDataViewScPagAttD.Item(ii)("SelInRata").ToString.Trim Then
                                Continue For
                            End If
                        End If
                        aDataViewScPagAttD.BeginInit()
                        If sender.checked = True Then
                            aDataViewScPagAttD.Item(ii)("Sel") = 1
                            aDataViewScPagAttD.Item(ii)("SelInRata") = mySelInRata
                        Else
                            aDataViewScPagAttD.Item(ii)("Sel") = 0
                            aDataViewScPagAttD.Item(ii)("SelInRata") = ""
                        End If
                        'GIU191123
                        If myRegimeIVA > 49 Then 'TOTALE CON IVA 0 quindi IVA 0
                            myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                        Else
                            myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                            myTotIVAF1 += (aDataViewScPagAttD.Item(ii)("Importo") / 100) * aDataViewScPagAttD.Item(ii)("Cod_IVA")
                        End If
                        '.........
                        aDataViewScPagAttD.EndInit()
                    Next
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("(2) Errore chkSel_CheckedChanged: ", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                End Try
                '----
                aDataViewScPagAttD.RowFilter = ""
                Session(sessDataViewScAttD) = aDataViewScPagAttD
                aDataViewScPagAttD.RowFilter = "IDDocumenti=" + myIDDocumenti + " AND (ISNULL(Qta_Fatturata,0)=0) "
                '---------
                If chkVisALLSc.Checked = False Then
                    If String.IsNullOrEmpty(myAnnoDataSc) Then myAnnoDataSc = ""
                    If myAnnoDataSc.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR (ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                    Else
                        aDataViewScPagAttD.RowFilter += " AND ((ISNULL(Sel,0)<>0) OR SelInRata='" + mySelInRata + "')"
                    End If
                    '-
                    If chkSoloEvase.Checked = True Then
                        aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                    End If
                    If myNSerie.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                    End If
                Else
                    If myNSerie.Trim <> "" Then 'giu011123
                        aDataViewScPagAttD.RowFilter += " AND (SERIE='" + myNSerie.Trim + "' OR ISNULL(Sel,0)<>0)"
                    End If
                End If
                '-
                If aDataViewScPagAttD.Count > 0 Then aDataViewScPagAttD.Sort = "DataSc"
                GridViewPrevD.DataSource = aDataViewScPagAttD
                GridViewPrevD.EditIndex = -1
                GridViewPrevD.DataBind()
            End If
            '---------
            If sender.checked = False Then
                myTotImponF1 = 0
                myTotIVAF1 = 0
            End If
            aDataViewScPagAtt.Item(myRowIndex).Item("TotImponF1") = myTotImponF1
            aDataViewScPagAtt.Item(myRowIndex).Item("TotIVAF1") = myTotIVAF1
            If mySplitIVA = True Then 'il TOTALE RATA E' SENZA IVA: calcolo IVA per determinare il totale
                aDataViewScPagAtt.Item(myRowIndex).Item("TotaleF1") = myTotImponF1
            Else
                aDataViewScPagAtt.Item(myRowIndex).Item("TotaleF1") = myTotImponF1 + myTotIVAF1
            End If
            '
            If (myTotImponF1 + myTotIVAF1) = 0 And sender.checked = True Then 'giu191123 
                aDataViewScPagAtt.Item(myRowIndex).Item("Sel") = 0
            End If
            aDataViewScPagAtt.EndInit() 'giu191123 Ok se adesso c'è l'importo abilito anche i tasti di FATTURAZIONE
            aDataViewScPagAtt.RowFilter = "Sel<>0"
            If aDataViewScPagAtt.Count > 0 Then
                btnEmFattSc.Enabled = True
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = True
            Else
                btnEmFattSc.Enabled = False
                btnStFattScEm.Enabled = False
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = False
            End If
            aDataViewScPagAtt.RowFilter = ""
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = SaveSort
            Session(sessDataViewScAtt) = aDataViewScPagAtt
            GridViewPrevT.DataSource = aDataViewScPagAtt
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("(1) Errore chkSel_CheckedChanged: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    'giu260321
    Protected Sub chkSelD_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Try
            'selected row gridview Testata
            Dim myRowIndexT As Integer = GridViewPrevT.SelectedIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            Dim SaveFilterT As String = aDataViewScPagAtt.RowFilter
            Dim SaveSortT As String = aDataViewScPagAtt.Sort
            '-
            aDataViewScPagAtt = Session(sessDataViewScAtt)
            If myRowIndexT = -1 Or aDataViewScPagAtt.Count = 0 Then
                sender.checked = False
                Exit Sub
            End If
            '---
            'selected row gridview dettagli
            Dim cbD As CheckBox = CType(sender, CheckBox)
            Dim myrowD As GridViewRow = CType(cbD.NamingContainer, GridViewRow)
            Dim myRowIndexD As Integer = myrowD.RowIndex + (GridViewPrevD.PageSize * GridViewPrevD.PageIndex)
            GridViewPrevD.SelectedIndex = myrowD.RowIndex        'indice della griglia
            '---
            Dim SaveFilterD As String = aDataViewScPagAttD.RowFilter
            Dim SaveSortD As String = aDataViewScPagAttD.Sort

            If myRowIndexD = -1 Or aDataViewScPagAttD.Count = 0 Then
                sender.checked = False
                Exit Sub
            End If
            '---
            aDataViewScPagAttD.BeginInit()
            'se flag  = true allora il flag attivo passa a False
            If sender.checked = True Then
                aDataViewScPagAttD.Item(myRowIndexD).Item("SelInRata") = aDataViewScPagAtt.Item(myRowIndexT)("DesRata")
                aDataViewScPagAttD.Item(myRowIndexD).Item("Sel") = 1
            Else
                aDataViewScPagAttD.Item(myRowIndexD).Item("SelInRata") = ""
                aDataViewScPagAttD.Item(myRowIndexD).Item("Sel") = 0
            End If
            aDataViewScPagAttD.EndInit()
            aDataViewScPagAttD.RowFilter = ""
            If aDataViewScPagAttD.Count > 0 Then aDataViewScPagAttD.Sort = SaveSortD
            Session(sessDataViewScAttD) = aDataViewScPagAttD
            aDataViewScPagAttD.RowFilter = SaveFilterD
            GridViewPrevD.DataBind()
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ TESTATA - RATA
            'GIU191123 ora sottratto o aggiungo l'importo
            aDataViewScPagAtt.BeginInit()
            Dim myRegimeIVA As Integer = aDataViewScPagAtt.Item(myRowIndexT)("Regime_IVA")
            Dim mySplitIVA As Boolean = CBool(aDataViewScPagAtt.Item(myRowIndexT)("SplitIVA").ToString.Trim)
            Dim myTotImponF1 As Decimal = 0
            Dim myTotIVAF1 As Decimal = 0
            '-----------
            'GIU191123
            If myRegimeIVA > 49 Then 'TOTALE CON IVA 0 quindi IVA 0
                myTotImponF1 += aDataViewScPagAttD.Item(myRowIndexD)("Importo")
            Else
                myTotImponF1 += aDataViewScPagAttD.Item(myRowIndexD)("Importo")
                myTotIVAF1 += (aDataViewScPagAttD.Item(myRowIndexD)("Importo") / 100) * aDataViewScPagAttD.Item(myRowIndexD)("Cod_IVA")
            End If
            If sender.checked = False Then
                myTotImponF1 = myTotImponF1 * -1
                myTotIVAF1 = myTotIVAF1 * -1
            End If
            aDataViewScPagAtt.Item(myRowIndexT).Item("TotImponF1") += myTotImponF1
            aDataViewScPagAtt.Item(myRowIndexT).Item("TotIVAF1") += myTotIVAF1
            If mySplitIVA = True Then 'il TOTALE RATA E' SENZA IVA: calcolo IVA per determinare il totale
                aDataViewScPagAtt.Item(myRowIndexT).Item("TotaleF1") += myTotImponF1
            Else
                aDataViewScPagAtt.Item(myRowIndexT).Item("TotaleF1") += myTotImponF1 + myTotIVAF1
            End If
            '
            Dim SWImportoZERO As Boolean = False
            If aDataViewScPagAtt.Item(myRowIndexT).Item("TotaleF1") = 0 Then 'giu191123 
                aDataViewScPagAtt.Item(myRowIndexT).Item("Sel") = 0
                If sender.checked = True Then SWImportoZERO = True
            Else
                aDataViewScPagAtt.Item(myRowIndexT).Item("Sel") = 1
            End If
            aDataViewScPagAtt.EndInit() 'giu191123 Ok se adesso c'è l'importo abilito anche i tasti di FATTURAZIONE
            aDataViewScPagAtt.RowFilter = "Sel<>0"
            If aDataViewScPagAtt.Count > 0 Then
                btnEmFattSc.Enabled = True
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = True
            Else
                btnEmFattSc.Enabled = False
                btnStFattScEm.Enabled = False
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = False
            End If
            aDataViewScPagAtt.RowFilter = ""
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = SaveSortT
            Session(sessDataViewScAtt) = aDataViewScPagAtt
            GridViewPrevT.DataSource = aDataViewScPagAtt
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
            If SWImportoZERO Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attebzione", "Selezionare un attività con importo diverso da ZERO.", WUC_ModalPopup.TYPE_ALERT)
            End If
            '.........
        Catch Ex As Exception
            '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '''ModalPopup.Show("Errore chkSelD_CheckedChanged: ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
#Region "FATTURAZIONE SCADENZA PAG. CA"
    Private Sub btnEmFattSc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEmFattSc.Click
        aDataViewScPagAtt = Session(sessDataViewScAtt)
        aDataViewScPagAttD = Session(sessDataViewScAttD) 'giu240322
        If (aDataViewScPagAtt Is Nothing) Then
            SvuodaGridT()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di ricaricare i dati.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'IPA lungo 6 FC/PA altrimenti FC
        Dim TipoFC As Integer = 0
        Try
            aDataViewScPagAtt.RowFilter = "FatturaPA=0"
            If aDataViewScPagAtt.Count > 0 Then
                TipoFC = 1
            End If
            aDataViewScPagAtt.RowFilter = "FatturaPA<>0"
            If aDataViewScPagAtt.Count > 0 Then
                If TipoFC = 0 Then
                    TipoFC = 2
                Else
                    TipoFC = 3
                End If
            End If
        Catch ex As Exception
            SvuodaGridT()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Controllo tipo fatture FC/PA.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '-
        If CDate(txtDataFattura.Text.Trim).Date < CDate(ControllaDataDoc(TipoFC)).Date Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "DATA FATTURAZIONE NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA FATTURA INSERITA.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu160118
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DATA FATTURA NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Session(CSTDECIMALIVALUTADOC) = ""
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strDataSc As String = "" : Dim strPr As String = "" : Dim strCausale As String = ""
        Dim myID As String = "" : Dim myNRata As Integer = 0
        Dim NDoc As Long = 0 : Dim NDocOK As Long = 0 : Dim NRev As Integer = 0
        Dim strCPag As String = "" : Dim strTotaleDoc As String = "" : Dim Valuta As String = ""
        Dim strTotaleImp As String = "" : Dim strTotaleIVA As String = ""
        Dim strABI As String = "" : Dim strCAB As String = ""
        '
        Dim strPI As String = "" : Dim strCF As String = ""
        Dim strSplitIVA As String = ""
        Dim SWTotaleZero As Boolean = False
        Dim strTotaleRata As String = ""
        Dim TotFattureEM As Integer = 0
        Dim CKAttEvase As Boolean = False
        Dim CKNGGScadPag As Boolean = False
        Dim strScOltre As String = "" 'giu240322
        Try
            aDataViewScPagAtt.RowFilter = "Sel<>0 AND CKAttEvAllaData=0"
            If aDataViewScPagAtt.Count > 0 Then
                CKAttEvase = False
            Else
                CKAttEvase = True
            End If
            'Controllo gg alert SCADENZA
            aDataViewScPagAtt.RowFilter = "Sel<>0"
            If aDataViewScPagAtt.Count > 0 Then aDataViewScPagAtt.Sort = "DataSc1"
            If aDataViewScPagAtt.Count > 0 Then
                strDataSc = aDataViewScPagAtt.Item(0)("DataSc1").ToString.Trim
                If CDate(strDataSc).Date > Now.Date Then
                    CKNGGScadPag = True
                End If
            End If
            '----------------------------------------------------------------------
            'Controllo Totale N° Fatture da emettere
            TotFattureEM = aDataViewScPagAtt.Count
            If TotFattureEM > 0 Then
                'procedo al controllo
            Else
                btnEmFattSc.Enabled = False
                btnStFattScEm.Enabled = False
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna Scadenza selezionata per la fatturazione.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'GIU251123 SE FATTURA RIEPILOGATIVA CONTEGGIO UNA FATTURA UNICA PER CONTRATTO
            If chkFattRiep.Checked Then
                aDataViewScPagAtt.Sort = "IDDocumenti,DataSc1"
                TotFattureEM = 1
            End If
            Dim myIDCAPrec As String = aDataViewScPagAtt.Item(0)("IDDocumenti").ToString.Trim
            '----------------------------------------------------------------------------------
            For i = 0 To aDataViewScPagAtt.Count - 1
                If chkFattRiep.Checked Then
                    If myIDCAPrec <> aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim Then
                        TotFattureEM += 1
                        myIDCAPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                    End If
                End If
                'N°DOCUMENTO
                strNDoc = aDataViewScPagAtt.Item(i)("Numero").ToString.Trim
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                strDataDoc = aDataViewScPagAtt.Item(i)("Data_Doc").ToString.Trim
                If String.IsNullOrEmpty(strDataDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsDate(CDate(strDataDoc)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                strDataSc = aDataViewScPagAtt.Item(i)("DataSc1").ToString.Trim
                'GIU180322 Francesca 
                ' ''If CDate(strDataSc).Date > CDate(txtDataFattura.Text.Trim) Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "DATA SCADENZA SUPERIORE ALLA DATA DI FATTURAZIONE. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
                ' ''End If
                'giu240322 segnalazione scadenze Fatture e Attività future 
                If CDate(strDataSc).Date.Year <> CDate(txtDataFattura.Text.Trim).Year Then
                    If strScOltre.Trim = "" Then
                        strScOltre = "ATTENZIONE, Sono presenti attività con ANNO SCADENZA DIVERSO dall'anno di fatturazione.:<br>"
                    End If
                    If InStr(strScOltre, Mid(strDataSc.Trim, 1, 10)) = 0 Then
                        strScOltre += "," + Mid(strDataSc.Trim, 1, 10)
                    End If
                End If
                '- Controllo scadenze dettagli attivita' future
                If Not (aDataViewScPagAttD Is Nothing) Then
                    aDataViewScPagAttD.RowFilter = "IDDocumenti=" + aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) AND ISNULL(Sel,0)<>0"
                    For ii = 0 To aDataViewScPagAttD.Count - 1
                        strDataSc = aDataViewScPagAttD.Item(ii)("DataSc").ToString.Trim
                        If CDate(strDataSc).Date.Year <> CDate(txtDataFattura.Text.Trim).Year Then
                            If strScOltre.Trim = "" Then
                                strScOltre = "ATTENZIONE, Sono presenti attività con ANNO SCADENZA DIVERSO dall'anno di fatturazione.:<br>"
                            End If
                            If InStr(strScOltre, Mid(strDataSc.Trim, 1, 10)) = 0 Then
                                strScOltre += "," + Mid(strDataSc.Trim, 1, 10)
                            End If
                        End If
                    Next
                End If
                '-----------------------------------------------
                strTotaleRata = aDataViewScPagAtt.Item(i)("TotaleF1").ToString.Trim 'giu211123 perima era Rata1
                If String.IsNullOrEmpty(strTotaleRata) Then
                    SWTotaleZero = True
                ElseIf Not IsNumeric(strTotaleRata.Trim) Then
                    SWTotaleZero = True
                ElseIf CDec(strTotaleRata) = 0 Then
                    SWTotaleZero = True
                End If
                If SWTotaleZero Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IMPORTO DOCUMENTO UGUALE A ZERO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'SPOSTATO QUI TUTTI I CONTROLLI DA CREAFATTURETUTTE
                'ABI
                strABI = aDataViewScPagAtt.Item(i)("ABI").ToString.Trim
                If String.IsNullOrEmpty(strABI) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE ABI.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'CAB
                strCAB = aDataViewScPagAtt.Item(i)("CAB").ToString.Trim
                If String.IsNullOrEmpty(strCAB) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE CAB.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'PI'CF
                strPI = aDataViewScPagAtt.Item(i)("Partita_IVA").ToString.Trim
                strCF = aDataViewScPagAtt.Item(i)("Codice_Fiscale").ToString.Trim
                If String.IsNullOrEmpty(strPI) And String.IsNullOrEmpty(strCF) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "P.IVA - Cod.Fiscale MANCANTI.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'PROVINCIA
                strPr = aDataViewScPagAtt.Item(i)("Provincia").ToString.Trim
                If String.IsNullOrEmpty(strPr) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "PROVINCIA NON DEFINITA.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'CAUSALE
                strCausale = aDataViewScPagAtt.Item(i)("DesCausale").ToString.Trim
                If String.IsNullOrEmpty(strCausale) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CAUSALE NON DEFINITA.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'Split Payment strSplitIVA
                strSplitIVA = aDataViewScPagAtt.Item(i)("SplitIVA").ToString.Trim
                If String.IsNullOrEmpty(strSplitIVA) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "SPLIT PAYMENT IVA NON DEFINITO. . N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '---------
                'ID DOCUMENTI
                myID = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                Session(IDDOCUMENTI) = myID
                myID = Session(IDDOCUMENTI)
                If IsNothing(myID) Then
                    myID = ""
                End If
                If String.IsNullOrEmpty(myID) Then
                    myID = ""
                End If
                If Not IsNumeric(myID) Then
                    ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'NRATA
                myNRata = aDataViewScPagAtt.Item(i)("NRata").ToString.Trim
                If IsNothing(myNRata) Then
                    myNRata = ""
                End If
                If String.IsNullOrEmpty(myNRata) Then
                    myNRata = ""
                End If
                If Not IsNumeric(myNRata) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IDENTIFICATIVO NRATA SCONOSCIUTO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'giu240821 contollo sul CK FATTURA SE è STATA COMPILATA MA NON EVASA
                'giu251123 TENER CONTO DELLE FATTURE PARZIALI FINO A CHE IL TOTALE SIA = O MAGGIORE FORSE ANCHE MINORE ? 
                Dim SWOKNRata As Boolean = False
                Dim myScadCA As ScadPagCAEntity
                Dim myScadPagCA As String = aDataViewScPagAtt.Item(i)("ScadPagCA")
                If myScadPagCA.Trim <> "" Then
                    Dim lineaSplit As String() = myScadPagCA.Trim.Split(";")
                    For ii = 0 To lineaSplit.Count - 1
                        If lineaSplit(ii).Trim <> "" And (ii + 8) <= lineaSplit.Count - 1 Then 'giu191223 da (ii + 6)

                            myScadCA = New ScadPagCAEntity
                            myScadCA.NRata = lineaSplit(ii).Trim
                            If myNRata.ToString.Trim = lineaSplit(ii).Trim Then
                                SWOKNRata = True
                            Else
                                SWOKNRata = False
                            End If
                            ii += 1
                            myScadCA.Data = lineaSplit(ii).Trim
                            ii += 1
                            myScadCA.Importo = lineaSplit(ii).Trim
                            ii += 1
                            myScadCA.Evasa = lineaSplit(ii).Trim
                            If SWOKNRata = True Then
                                'giu
                                If myScadCA.Evasa = True Then
                                    'giu261123
                                    '''Session(MODALPOPUP_CALLBACK_METHOD) = "CallChiudi"
                                    '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    '''ModalPopup.Show("Errore", "RICALCOLO SCADENZE - SCADENZA GIA' EVASA.N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                                    '''Exit Sub
                                    strScOltre += "<br>Rata già fatturata: " + myNRata.ToString.Trim + " - " + myScadCA.Data.Trim + " - " + myScadCA.Importo.Trim
                                    '----------
                                Else
                                    'giu261123 myScadCA.Evasa = True
                                    Try
                                        If CDec(strTotaleRata.Trim) >= CDec(myScadCA.Importo.Trim) Then
                                            myScadCA.Evasa = True
                                        End If
                                    Catch ex As Exception
                                        'nulla lascio cosi 
                                    End Try
                                End If
                                ii += 1 'N. FATTURA
                                If lineaSplit(ii).Trim <> "" Then
                                    'giu261123 
                                    '''Session(MODALPOPUP_CALLBACK_METHOD) = "CallChiudi"
                                    '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    '''ModalPopup.Show("Errore", "RICALCOLO SCADENZE - N° FATTURA GIA' ASSEGNATA .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                                    '''Exit Sub
                                End If
                                ii += 1 'DATA FATTURA
                                If lineaSplit(ii).Trim <> "" Then
                                    'giu261123
                                    '''Session(MODALPOPUP_CALLBACK_METHOD) = "CallChiudi"
                                    '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    '''ModalPopup.Show("Errore", "RICALCOLO SCADENZE - DATA FATTURA GIA' ASSEGNATA .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                                    '''Exit Sub
                                Else
                                    myScadCA.DataFC = txtDataFattura.Text.Trim 'lineaSplit(ii).Trim
                                End If
                            Else
                                ii += 1 'N. FATTURA
                                myScadCA.NFC = lineaSplit(ii).Trim
                                ii += 1 'DATA FATTURA
                                myScadCA.DataFC = lineaSplit(ii).Trim
                            End If
                            '-----
                            ii += 1
                            myScadCA.Serie = lineaSplit(ii).Trim
                            ii += 1
                            myScadCA.ImportoF = lineaSplit(ii).Trim 'giu191223
                            ii += 1
                            myScadCA.ImportoR = lineaSplit(ii).Trim
                        End If
                    Next
                End If
                '-------------------------------------------------------------------
                'CODICE PAGAMENTO
                strCPag = aDataViewScPagAtt.Item(i)("Cod_Pagamento").ToString.Trim
                If String.IsNullOrEmpty(strCPag) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO.. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strCPag) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf CInt(strCPag) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'VALUTA
                Valuta = aDataViewScPagAtt.Item(i)("Cod_Valuta").ToString.Trim
                If String.IsNullOrEmpty(Valuta) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO. N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Controllo emissione fatture: " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '
        Session(EL_DOC_TOPRINT_SCY) = Nothing
        Session(EL_DOC_TOPRINT_SCN) = Nothing 'NON SERVE MA CMQ LO AZZERO
        '---------
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattureTutte"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Dim strMessNGG As String = ""
        If CKNGGScadPag = True Then
            strMessNGG = "<br>ATTENZIONE, State fatturando Pagamenti non ancora in scadenza<br>Sicuri di voler continuare?"
            'Session(LABELMESSAGERED) = SWSI
        End If
        If strScOltre.Trim <> "" Then 'giu240322
            strMessNGG += "<br>" + Mid(strScOltre.Trim, 1, 150)
            Session(LABELMESSAGERED) = SWSI
        End If
        If String.IsNullOrEmpty(strMessNGG) Then
            strMessNGG = ""
        End If
        If CKAttEvase = True Then
            ModalPopup.Show("Crea TUTTE le Fatture da Scadenze selezionate" + IIf(chkFattRiep.Checked, " (RIEPILOGATIVA)", ""), "Confermi la creazione di TUTTE le Fatture ?<br><strong><span>Totale Fatture: " & FormattaNumero(TotFattureEM.ToString.Trim) & strMessNGG & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            Session(LABELMESSAGERED) = SWSI
            ModalPopup.Show("Crea TUTTE le Fatture da Scadenze selezionate" + IIf(chkFattRiep.Checked, " (RIEPILOGATIVA)", ""), "Confermi la creazione di TUTTE le Fatture ?<br><strong><span>Totale Fatture: " & FormattaNumero(TotFattureEM.ToString.Trim) & "<br>ATTENZIONE, Alcune Scadenze selezionate hanno delle attività non ancora EVASE !!" & strMessNGG & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If

        ' ''End If
    End Sub
    Private Function ControllaDataDoc(ByVal _TipoFC As Integer) As DateTime
        Dim strSQL As String = ""
        'legenda: -1 solo FC - 2 solo FC/PA - 3 ENTRAMBE
        strSQL = "Select MAX(Data_Doc) AS Data_Doc From DocumentiT WHERE Tipo_Doc = 'FC' "
        'GIU110814
        ' ''If chkFatturaPA.Checked = False Then
        ' ''    strSQL += " AND ISNULL(FatturaPA,0) = 0"
        ' ''Else
        ' ''    strSQL += " AND ISNULL(FatturaPA,0) <> 0"
        ' ''End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Data_Doc")) Then
                        ControllaDataDoc = ds.Tables(0).Rows(0).Item("Data_Doc")
                    Else
                        ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                    End If
                    Exit Function
                Else
                    ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                    Exit Function
                End If
            Else
                ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                Exit Function
            End If
        Catch Ex As Exception
            ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    Public Sub CreaFattureTutte() 'TUTTE
        aDataViewScPagAtt = Session(sessDataViewScAtt)
        aDataViewScPagAttD = Session(sessDataViewScAttD) 'giu280321 righe aggiuntive ATTIVITA'
        If (aDataViewScPagAtt Is Nothing) Then
            SvuodaGridT()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di ricaricare i dati.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Session(CSTDECIMALIVALUTADOC) = 2
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strDataSc As String = "" : Dim strPr As String = "" : Dim strCausale As String = ""
        Dim myID As String = "" : Dim myNRata As Integer = 0 : Dim SWOKNRata As Boolean = False
        Dim NDoc As Long = 0 : Dim NDocPA As Long = 0 : Dim NDocOK As Long = 0 : Dim NRev As Integer = 0
        Dim strCPag As String = "" : Dim strTotaleDoc As String = "" : Dim Valuta As String = ""
        Dim strTotaleImp As String = "" : Dim strTotaleIVA As String = ""
        Dim strABI As String = "" : Dim strCAB As String = ""
        '
        Dim strPI As String = "" : Dim strCF As String = ""
        Dim CKSplitIVA As Boolean = False : Dim CKFatturaPA As Boolean = False
        Dim myTotNettoPagare As Decimal = 0 ': Dim myTotNettoPagareR As Decimal = 0
        'VARIABILI PER CALCOLO SCADENZE
        Dim NRate As Integer = 0
        Dim TotRate As Decimal = 0 'giu010220
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myScadPagCA As String = ""
        Dim rowScadPagCa As ScadPagCAEntity = Nothing
        Dim myDataScRata As String = ""
        '-
        Dim myList As String = "" 'giu280321  RIGHE ATTIVITA XXXYYY dove XXX=DurataNumRiga e YYY=Riga 
        '-
        Dim strScOltre As String = "" 'giu240322
        'FINE CONTROLLO FATTO PRIMA 
        Try
            aDataViewScPagAtt.RowFilter = "Sel<>0"
            If aDataViewScPagAtt.Count > 0 Then
                'procedo al controllo
            Else
                btnEmFattSc.Enabled = False
                btnStFattScEm.Enabled = False
                btnSelSc.Enabled = True
                btnDeselSc.Enabled = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna Scadenza selezionata per la fatturazione.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'GIU251123 SE FATTURA RIEPILOGATIVA CONTEGGIO UNA FATTURA UNICA PER CONTRATTO
            If chkFattRiep.Checked Then
                aDataViewScPagAtt.Sort = "IDDocumenti,DataSc1"
            End If
            Dim myIDCAPrec As String = ""
            Dim myDesRataRiep As String = " - RIEPILOGO RATE: "
            '-
            Dim myRegimeIVA As Integer = -1
            Dim mySplitIVA As Boolean = False
            Dim myTotImponF1 As Decimal = 0 : Dim myTotImponF1R As Decimal = 0
            Dim myTotIVAF1 As Decimal = 0 : Dim myTotIVAF1R As Decimal = 0
            '-----------
            '----------------------------------------------------------------------------------
            For i = 0 To aDataViewScPagAtt.Count - 1
                
                If chkFattRiep.Checked Then
                    
                    If myIDCAPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim Then
                        Continue For
                    Else
                        myIDCAPrec = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                        'mi prendo tutte le Rate 
                        myTotImponF1R = 0
                        myTotIVAF1R = 0
                        If aDataViewScPagAtt.Count > 1 Then
                            For x = 0 To aDataViewScPagAtt.Count - 1
                                'NRATA
                                myNRata = aDataViewScPagAtt.Item(x)("NRata").ToString.Trim
                                If IsNothing(myNRata) Then
                                    myNRata = ""
                                End If
                                If String.IsNullOrEmpty(myNRata) Then
                                    myNRata = ""
                                End If
                                If Not IsNumeric(myNRata) Then
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore", "IDENTIFICATIVO NRATA SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Sub
                                End If
                                strDataSc = aDataViewScPagAtt.Item(x)("DataSc1").ToString.Trim
                                If String.IsNullOrEmpty(strDataSc) Then
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Sub
                                ElseIf Not IsDate(CDate(strDataSc)) Then
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Sub
                                End If
                                myDesRataRiep += "(" + myNRata.ToString.Trim + "/" + CDate(strDataSc).Date.Year.ToString.Trim + ")"
                                '-
                                myTotImponF1R += aDataViewScPagAtt.Item(x).Item("TotImponF1")
                                myTotIVAF1R += aDataViewScPagAtt.Item(x).Item("TotIVAF1")
                            Next
                        Else
                            myDesRataRiep = aDataViewScPagAtt.Item(i)("DesRata").ToString.Trim + " Scadenza " + aDataViewScPagAtt.Item(i)("DataSc1").ToString.Trim
                            myTotImponF1R += aDataViewScPagAtt.Item(i).Item("TotImponF1")
                            myTotIVAF1R += aDataViewScPagAtt.Item(i).Item("TotIVAF1")
                        End If
                    End If
                End If
                'N°DOCUMENTO
                strNDoc = aDataViewScPagAtt.Item(i)("Numero").ToString.Trim
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                strDataDoc = aDataViewScPagAtt.Item(i)("Data_Doc").ToString.Trim
                If String.IsNullOrEmpty(strDataDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsDate(CDate(strDataDoc)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                strDataSc = aDataViewScPagAtt.Item(i)("DataSc1").ToString.Trim
                If String.IsNullOrEmpty(strDataSc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsDate(CDate(strDataSc)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'giu180322 Francesca
                ' ''If CDate(strDataSc).Date > CDate(txtDataFattura.Text.Trim) Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "DATA SCADENZA SUPERIORE ALLA DATA DI FATTURAZIONE .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
                ' ''End If
                'giu240322 segnalazione scadenze attività future 
                If CDate(strDataSc).Date.Year <> CDate(txtDataFattura.Text.Trim).Year Then
                    If strScOltre.Trim = "" Then
                        strScOltre = "ATTENZIONE, Sono presenti attività con ANNO SCADENZA DIVERSO dall'anno di fatturazione.:<br>"
                    End If
                    If InStr(strScOltre, Mid(strDataSc.Trim, 1, 10)) = 0 Then
                        strScOltre += "," + Mid(strDataSc.Trim, 1, 10)
                    End If
                End If
                '-----------------------------------------------
                'ID DOCUMENTI
                myID = aDataViewScPagAtt.Item(i)("IDDocumenti").ToString.Trim
                Session(IDDOCUMENTI) = myID
                myID = Session(IDDOCUMENTI)
                If IsNothing(myID) Then
                    myID = ""
                End If
                If String.IsNullOrEmpty(myID) Then
                    myID = ""
                End If
                If Not IsNumeric(myID) Then
                    ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'GIU280221
                myList = ""
                aDataViewScPagAttD.RowFilter = "IDDocumenti=" + myID.Trim + " AND (ISNULL(Qta_Fatturata,0)=0) AND ISNULL(Sel,0)<>0"
                'giu251123
                Dim myAnnoDataSc As String = CDate(strDataSc).Date.Year.ToString.Trim
                If String.IsNullOrEmpty(myAnnoDataSc) Then myAnnoDataSc = ""
                Dim mySelInRata As String = aDataViewScPagAtt.Item(i)("DesRata").ToString.Trim
                If String.IsNullOrEmpty(mySelInRata) Then
                    mySelInRata = ""
                Else
                    If mySelInRata.Trim = HTML_SPAZIO Then
                        mySelInRata = ""
                    End If
                End If
                Dim myNSerie As String = aDataViewScPagAtt.Item(i)("Serie").ToString.Trim
                If String.IsNullOrEmpty(myNSerie) Then
                    myNSerie = ""
                Else
                    If myNSerie.Trim = HTML_SPAZIO Then
                        myNSerie = ""
                    End If
                End If
                '---------
                'giu261123 FATTURA RIEPILOGATIVA PER CONTRATTO
                If chkFattRiep.Checked = False Then
                    If myAnnoDataSc.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND (AnnoDataSc=" + myAnnoDataSc.Trim + " OR SelInRata='" + mySelInRata + "')"
                    Else
                        aDataViewScPagAttD.RowFilter += " AND (SelInRata='" + mySelInRata + "')"
                    End If
                    '-
                    If chkSoloEvase.Checked = True And chkVisALLSc.Checked = False Then
                        aDataViewScPagAttD.RowFilter += " AND (ISNULL(Qta_Evasa,0)<>0) "
                    End If
                    If myNSerie.Trim <> "" Then
                        aDataViewScPagAttD.RowFilter += " AND Serie='" + myNSerie.Trim + "'"
                    End If
                Else
                    aDataViewScPagAttD.RowFilter += " AND (ISNULL(Sel,0)<>0)"
                End If
                '---------------------------------------------
                'giu251123 ricalco Importo documento@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                myRegimeIVA = aDataViewScPagAtt.Item(i)("Regime_IVA")
                mySplitIVA = CBool(aDataViewScPagAtt.Item(i)("SplitIVA").ToString.Trim)
                '-
                myTotImponF1 = 0
                myTotIVAF1 = 0
                '---------
                For ii = 0 To aDataViewScPagAttD.Count - 1
                    If myList.Trim <> "" Then
                        myList += "," + Format(aDataViewScPagAttD.Item(ii)("DurataNumRiga"), "000") + Format(aDataViewScPagAttD.Item(ii)("Riga"), "000")
                    Else
                        myList += Format(aDataViewScPagAttD.Item(ii)("DurataNumRiga"), "000") + Format(aDataViewScPagAttD.Item(ii)("Riga"), "000")
                    End If
                    strDataSc = aDataViewScPagAttD.Item(ii)("DataSc").ToString.Trim
                    'giu240322 - Controllo dettagli attivita'
                    If CDate(strDataSc).Date.Year <> CDate(txtDataFattura.Text.Trim).Year Then
                        If strScOltre.Trim = "" Then
                            strScOltre = "ATTENZIONE, Sono presenti attività con ANNO SCADENZA DIVERSO dall'anno di fatturazione.:<br>"
                        End If
                        If InStr(strScOltre, Mid(strDataSc.Trim, 1, 10)) = 0 Then
                            strScOltre += "," + Mid(strDataSc.Trim, 1, 10)
                        End If
                    End If
                    '----------------------------------------
                    'GIU191123
                    If myRegimeIVA > 49 Then 'TOTALE CON IVA 0 quindi IVA 0
                        myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                    Else
                        myTotImponF1 += aDataViewScPagAttD.Item(ii)("Importo")
                        myTotIVAF1 += (aDataViewScPagAttD.Item(ii)("Importo") / 100) * aDataViewScPagAttD.Item(ii)("Cod_IVA")
                    End If
                    '.........
                Next
                'GIU271123
                If chkFattRiep.Checked = False Then
                    If myTotImponF1 <> aDataViewScPagAtt.Item(i).Item("TotImponF1") Or aDataViewScPagAtt.Item(i).Item("TotIVAF1") <> myTotIVAF1 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "RICALCOLO DOCUMENTO NON CORRISPONDENTE DA QUELLO SELEZIONATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Else
                    '''If myTotImponF1R <> aDataViewScPagAtt.Item(i).Item("TotImponF1") Or aDataViewScPagAtt.Item(i).Item("TotIVAF1") <> myTotIVAF1R Then
                    '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    '''    ModalPopup.Show("Errore", "RICALCOLO DOCUMENTO NON CORRISPONDENTE DA QUELLO SELEZIONATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    '''    Exit Sub
                    '''End If
                End If
                '-
                aDataViewScPagAtt.BeginInit()
                'giu180124 arrotondamenti 
                myTotImponF1 = Math.Round(myTotImponF1, 2)
                myTotIVAF1 = Math.Round(myTotIVAF1, 2)
                '---------
                aDataViewScPagAtt.Item(i).Item("TotImponF1") = myTotImponF1
                aDataViewScPagAtt.Item(i).Item("TotIVAF1") = myTotIVAF1
                If mySplitIVA = True Then 'il TOTALE RATA E' SENZA IVA: calcolo IVA per determinare il totale
                    aDataViewScPagAtt.Item(i).Item("TotaleF1") = myTotImponF1
                Else
                    aDataViewScPagAtt.Item(i).Item("TotaleF1") = myTotImponF1 + myTotIVAF1
                End If
                aDataViewScPagAtt.EndInit()
                '---------
                'NRATA
                myNRata = aDataViewScPagAtt.Item(i)("NRata").ToString.Trim
                If IsNothing(myNRata) Then
                    myNRata = ""
                End If
                If String.IsNullOrEmpty(myNRata) Then
                    myNRata = ""
                End If
                If Not IsNumeric(myNRata) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "IDENTIFICATIVO NRATA SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'CODICE PAGAMENTO
                strCPag = aDataViewScPagAtt.Item(i)("Cod_Pagamento").ToString.Trim
                If String.IsNullOrEmpty(strCPag) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strCPag) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf CInt(strCPag) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'VALUTA
                Valuta = aDataViewScPagAtt.Item(i)("Cod_Valuta").ToString.Trim
                If String.IsNullOrEmpty(Valuta) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'TOTALE DOCUMENTO
                strTotaleDoc = aDataViewScPagAtt.Item(i)("TotaleF1").ToString.Trim 'giu251123 ("Rata1").ToString.Trim
                If String.IsNullOrEmpty(strTotaleDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO .N° DOC.: " & strNDoc & " Rata: " & myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strTotaleDoc.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO .N° DOC.: " & strNDoc & " Rata: " & myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf CDec(strTotaleDoc) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE DOCUMENTO = A ZERO .N° DOC.: " & strNDoc & " Rata: " & myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '
                strTotaleImp = aDataViewScPagAtt.Item(i).Item("TotImponF1") 'giu251123 aDataViewScPagAtt.Item(i)("TotImpon1").ToString.Trim
                If String.IsNullOrEmpty(strTotaleImp) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strTotaleImp.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf CDec(strTotaleImp) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '-
                strTotaleIVA = aDataViewScPagAtt.Item(i).Item("TotIVAF1") 'giu251123 aDataViewScPagAtt.Item(i)("TotIVA1").ToString.Trim
                If String.IsNullOrEmpty(strTotaleIVA) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE IVA.DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strTotaleIVA.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE IVA.DOCUMENTO ERRATO .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                CKFatturaPA = aDataViewScPagAtt.Item(i)("FatturaPA")
                CKSplitIVA = aDataViewScPagAtt.Item(i)("SplitIVA")
                '---------
                If CKSplitIVA = True Then
                    myTotNettoPagare = aDataViewScPagAtt.Item(i).Item("TotImponF1") 'giu251123 aDataViewScPagAtt.Item(i)("TotImpon1")
                Else
                    myTotNettoPagare = aDataViewScPagAtt.Item(i).Item("TotImponF1") + aDataViewScPagAtt.Item(i).Item("TotIVAF1")
                    'giu251123 aDataViewScPagAtt.Item(i)("TotImpon1") + aDataViewScPagAtt.Item(i)("TotIVA1")
                End If
                'NUMERO DOCUMENTO FATTURA/PA
                NDoc = 0 : NDocPA = 0 : NRev = 0
                If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                    If CKFatturaPA = False Then 'GIU110814
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                        NDocOK = NDoc
                    Else
                        NDocPA = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                        NDocOK = NDocPA
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '--------------------------------
                'RICALCOLO SCADENZE
                Try
                    If Calcola_Scadenze(CLng(myID), strNDoc + " Rata: " + myNRata.ToString.Trim, CDate(txtDataFattura.Text), CInt(strCPag), myTotNettoPagare, Valuta, StrErrore) = False Then
                        'giu180124
                        '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        '''ModalPopup.Show("Errore", "RICALCOLO SCADENZE .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                    '-------------------------------------------------------------------------------
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "RICALCOLO SCADENZE .N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                'Aggiorno il NUMERO FATTURA/PA - QUI INVIO FC ALTRIMENTI 'PA' 
                If CKFatturaPA = False Then
                    If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev, StrErrore) = False Then
                        SvuodaGridT()
                        SetLnk()
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "AGGIORNA N. DOCUMENTO. N° " + strNDoc + "<br>" + StrErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Else
                    If AggiornaNumDoc("PA", NDocPA, NRev, StrErrore) = False Then
                        '''Call Chiudi("ERRORE: " + StrErrore)
                        SvuodaGridT()
                        SetLnk()
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "AGGIORNA N. DOCUMENTO. N° " + strNDoc + "<br>" + StrErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                End If
                'OK CREAZIONE NUOVA FATTURA
                SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                '---------------------------
                Dim SqlDbNewCmd As New SqlCommand
                SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCTutteCA]" 'GIU251123 AGG QTA FATTURATA
                SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbNewCmd.Connection = SqlConn
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataFC", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesCausale", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SiglaCA", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRata", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadAttCA", System.Data.SqlDbType.NVarChar, 1073741823))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRefInt", System.Data.SqlDbType.NVarChar, 1073741823))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDTipoFatt", System.Data.SqlDbType.NVarChar, 2))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Pagamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotImpon1", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotIVA1", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IVAFattCA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CMagazzino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MyList", System.Data.SqlDbType.VarChar, 3000))
                '-- ASSEGNAZIONE PARAMETRI
                SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
                SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
                SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
                If CKFatturaPA Then
                    SqlDbNewCmd.Parameters.Item("@Numero").Value = NDocPA
                Else
                    SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
                End If
                '-
                SqlDbNewCmd.Parameters.Item("@DataFC").Value = CDate(txtDataFattura.Text)
                'Blocco Data Scadenza
                If IsNothing(lblDataScad1) Then lblDataScad1 = ""
                If String.IsNullOrEmpty(lblDataScad1) Then lblDataScad1 = ""
                If lblDataScad1.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_1").Value = CDate(lblDataScad1.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_1").Value = DBNull.Value
                    Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - La prima rata risulta non valida (data) .N° DOC.: " & strNDoc)
                    Exit Sub
                End If
                If IsNothing(lblDataScad2) Then lblDataScad2 = ""
                If String.IsNullOrEmpty(lblDataScad2) Then lblDataScad2 = ""
                If lblDataScad2.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_2").Value = CDate(lblDataScad2.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_2").Value = DBNull.Value
                End If
                If IsNothing(lblDataScad3) Then lblDataScad3 = ""
                If String.IsNullOrEmpty(lblDataScad3) Then lblDataScad3 = ""
                If lblDataScad3.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_3").Value = CDate(lblDataScad3.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_3").Value = DBNull.Value
                End If
                If IsNothing(lblDataScad4) Then lblDataScad4 = ""
                If String.IsNullOrEmpty(lblDataScad4) Then lblDataScad4 = ""
                If lblDataScad4.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_4").Value = CDate(lblDataScad4.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_4").Value = DBNull.Value
                End If
                If IsNothing(lblDataScad5) Then lblDataScad5 = ""
                If String.IsNullOrEmpty(lblDataScad5) Then lblDataScad5 = ""
                If lblDataScad5.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_5").Value = CDate(lblDataScad5.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Data_Scadenza_5").Value = DBNull.Value
                End If
                'Blocco Scadenze RATE lblImpRata1
                If IsNothing(lblImpRata1) Then lblImpRata1 = ""
                If String.IsNullOrEmpty(lblImpRata1) Then lblImpRata1 = ""
                If lblImpRata1.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Rata_1").Value = CDec(lblImpRata1.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Rata_1").Value = 0
                    Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - La prima rata risulta non valida (Importo) .N° DOC.: " & strNDoc)
                    Exit Sub
                End If
                If IsNothing(lblImpRata2) Then lblImpRata2 = ""
                If String.IsNullOrEmpty(lblImpRata2) Then lblImpRata2 = ""
                If lblImpRata2.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Rata_2").Value = CDec(lblImpRata2.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Rata_2").Value = 0
                End If
                If IsNothing(lblImpRata3) Then lblImpRata3 = ""
                If String.IsNullOrEmpty(lblImpRata3) Then lblImpRata3 = ""
                If lblImpRata3.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Rata_3").Value = CDec(lblImpRata3.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Rata_3").Value = 0
                End If
                If IsNothing(lblImpRata4) Then lblImpRata4 = ""
                If String.IsNullOrEmpty(lblImpRata4) Then lblImpRata4 = ""
                If lblImpRata4.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Rata_4").Value = CDec(lblImpRata4.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Rata_4").Value = 0
                End If
                If IsNothing(lblImpRata5) Then lblImpRata5 = ""
                If String.IsNullOrEmpty(lblImpRata5) Then lblImpRata5 = ""
                If lblImpRata5.Trim <> "" Then
                    SqlDbNewCmd.Parameters.Item("@Rata_5").Value = CDec(lblImpRata5.Trim)
                Else
                    SqlDbNewCmd.Parameters.Item("@Rata_5").Value = 0
                End If
                '---------------------------------------------------
                SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
                SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                SqlDbNewCmd.Parameters.Item("@DesCausale").Value = Mid(Trim(aDataViewScPagAtt.Item(i)("DesCausale")), 1, 50)
                SqlDbNewCmd.Parameters.Item("@SiglaCA").Value = Mid(Trim(aDataViewScPagAtt.Item(i)("SiglaCA")), 1, 50)
                If chkFattRiep.Checked = False Then
                    SqlDbNewCmd.Parameters.Item("@DesRata").Value = Mid(Trim(aDataViewScPagAtt.Item(i)("DesRata")), 1, 50)
                Else
                    SqlDbNewCmd.Parameters.Item("@DesRata").Value = Mid(myDesRataRiep, 1, 50)
                End If
                'DATE DI SCADENZE RATE 
                Try
                    'GIU050620 NEL CASO IN CUI CI SONO PIU FATTURE DA EMETTERE PER LO STESSO CONTRATTO 
                    'RIELEGGERE LO SCADENZARIO ALTRIMENTI L'ULTIMA FATTURA RIAPRE LE PRECEDENTI E NON VA BENE
                    myScadPagCA = GetScadenzario(myID)
                    If myScadPagCA.Trim = "" Then ' IsDBNull(aDataViewScPagAtt.Item(i)("ScadPagCA")) Then
                        '''aDataViewScPagAtt.Item(i)("ScadPagCA") = ""
                        '''Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - NESSUNA SCADENZA ? .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim)
                        SvuodaGridT()
                        SetLnk()
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "RICALCOLO SCADENZE - NESSUNA SCADENZA. N° " + strNDoc + " Rata: " + myNRata.ToString.Trim, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                    SWOKNRata = False
                    ArrScadPagCA = New ArrayList
                    Dim myScadCA As ScadPagCAEntity
                    If myScadPagCA.Trim <> "" Then ' aDataViewScPagAtt.Item(i)("ScadPagCA") <> "" Then
                        Dim lineaSplit As String() = myScadPagCA.Trim.Split(";") 'aDataViewScPagAtt.Item(i)("ScadPagCA").Split(";")
                        For ii = 0 To lineaSplit.Count - 1
                            If lineaSplit(ii).Trim <> "" And (ii + 8) <= lineaSplit.Count - 1 Then 'giu191223 da + 6 a + 8
                                SWOKNRata = False
                                myScadCA = New ScadPagCAEntity
                                myScadCA.NRata = lineaSplit(ii).Trim
                                'GIU271123 RIEPILOGATIVA RATE
                                If chkFattRiep.Checked = False Then
                                    If myNRata.ToString.Trim = lineaSplit(ii).Trim Then
                                        SWOKNRata = True
                                    Else
                                        SWOKNRata = False
                                    End If
                                Else
                                    SWOKNRata = False
                                    For x = 0 To aDataViewScPagAtt.Count - 1
                                        'NRATA
                                        myNRata = aDataViewScPagAtt.Item(x)("NRata").ToString.Trim
                                        If IsNothing(myNRata) Then
                                            myNRata = ""
                                        End If
                                        If String.IsNullOrEmpty(myNRata) Then
                                            myNRata = ""
                                        End If
                                        If Not IsNumeric(myNRata) Then
                                            SvuodaGridT()
                                            SetLnk()
                                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                            ModalPopup.Show("Errore", "IDENTIFICATIVO NRATA SCONOSCIUTO. N° " + strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                            Exit Sub
                                        End If
                                        If myNRata.ToString.Trim = lineaSplit(ii).Trim Then
                                            SWOKNRata = True
                                        End If
                                        '''strDataSc = aDataViewScPagAtt.Item(x)("DataSc1").ToString.Trim
                                        '''If String.IsNullOrEmpty(strDataSc) Then
                                        '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                        '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        '''    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                        '''    Exit Sub
                                        '''ElseIf Not IsDate(CDate(strDataSc)) Then
                                        '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                        '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        '''    ModalPopup.Show("Errore", "DATA SCADENZA .N° DOC.:  " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                                        '''    Exit Sub
                                        '''End If
                                    Next
                                End If
                                '---------
                                ii += 1
                                myScadCA.Data = lineaSplit(ii).Trim
                                If SWOKNRata = True Then
                                    myDataScRata = lineaSplit(ii).Trim
                                End If
                                ii += 1
                                myScadCA.Importo = lineaSplit(ii).Trim
                                TotRate += CDec(myScadCA.Importo)
                                ii += 1
                                myScadCA.Evasa = lineaSplit(ii).Trim
                                If SWOKNRata = True Then
                                    If myScadCA.Evasa = True Then
                                        'giu261123
                                        '''Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - SCADENZA GIA' EVASA.N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim)
                                        '''Exit Sub
                                        strScOltre += "<br>Rata già fatturata: " + myNRata.ToString.Trim + " - " + myScadCA.Data.Trim + " - " + myScadCA.Importo.Trim
                                        '----------
                                    Else
                                        'giu261123 myScadCA.Evasa = True
                                        Try
                                            If CDec(myTotNettoPagare) >= CDec(myScadCA.Importo.Trim) Then
                                                myScadCA.Evasa = True
                                            End If
                                        Catch ex As Exception
                                            'nulla lascio cosi 
                                        End Try
                                    End If
                                    ii += 1 'N. FATTURA
                                    'giu261123
                                    '''If lineaSplit(ii).Trim <> "" Then
                                    '''    Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - N° FATTURA GIA' ASSEGNATA .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim)
                                    '''    Exit Sub
                                    '''Else
                                    '''    If CKFatturaPA Then
                                    '''        myScadCA.NFC = NDocPA.ToString.Trim 'lineaSplit(ii).Trim
                                    '''    Else
                                    '''        myScadCA.NFC = NDoc.ToString.Trim 'lineaSplit(ii).Trim
                                    '''    End If
                                    '''End If
                                    If CKFatturaPA Then
                                        myScadCA.NFC = IIf(lineaSplit(ii).Trim <> "", lineaSplit(ii).Trim + "-", "") + NDocPA.ToString.Trim + "/PA"
                                    Else
                                        myScadCA.NFC = IIf(lineaSplit(ii).Trim <> "", lineaSplit(ii).Trim + "-", "") + NDoc.ToString.Trim
                                    End If
                                    '---------
                                    ii += 1 'DATA FATTURA
                                    'giu261123
                                    '''If lineaSplit(ii).Trim <> "" Then
                                    '''    Call Chiudi("ERRORE: " + "RICALCOLO SCADENZE - DATA FATTURA GIA' ASSEGNATA .N° DOC.: " & strNDoc + " Rata: " + myNRata.ToString.Trim)
                                    '''    Exit Sub
                                    '''Else
                                    '''    myScadCA.DataFC = txtDataFattura.Text.Trim 'lineaSplit(ii).Trim
                                    '''End If
                                    myScadCA.DataFC = txtDataFattura.Text.Trim
                                    '--------
                                Else
                                    ii += 1 'N. FATTURA
                                    myScadCA.NFC = lineaSplit(ii).Trim
                                    ii += 1 'DATA FATTURA
                                    myScadCA.DataFC = lineaSplit(ii).Trim
                                End If
                                '-----
                                ii += 1
                                myScadCA.Serie = lineaSplit(ii).Trim
                                'giu191223
                                If SWOKNRata = True Then
                                  
                                    Try
                                        If CDec(myTotNettoPagare) >= CDec(myScadCA.Importo.Trim) Then
                                            myScadCA.Evasa = True 'GIU20123
                                            ii += 1
                                            myScadCA.ImportoF = myScadCA.Importo.Trim
                                            ii += 1
                                            myScadCA.ImportoR = "0"
                                        Else
                                            ii += 1
                                            myScadCA.ImportoF = (CDec(lineaSplit(ii).Trim) + CDec(myTotNettoPagare)).ToString.Trim
                                            If CDec(myScadCA.ImportoF) >= CDec(myScadCA.Importo.Trim) Then
                                                myScadCA.Evasa = True 'GIU20123
                                                myScadCA.ImportoF = myScadCA.Importo.Trim
                                                myScadCA.ImportoR = "0"
                                            Else
                                                myScadCA.ImportoR = (CDec(myScadCA.ImportoF) - CDec(myScadCA.Importo.Trim)).ToString.Trim
                                            End If
                                            ii += 1
                                        End If
                                    Catch ex As Exception
                                        'nulla lascio cosi 
                                    End Try
                                Else
                                    ii += 1
                                    myScadCA.ImportoF = lineaSplit(ii).Trim
                                    ii += 1
                                    myScadCA.ImportoR = lineaSplit(ii).Trim
                                End If
                                'GIU201223
                                If CDec(myScadCA.ImportoR.Trim) < 0 Then
                                    myScadCA.ImportoR = (CDec(myScadCA.ImportoR.Trim) * -1).ToString.Trim
                                End If
                                If CDec(myScadCA.ImportoF) >= CDec(myScadCA.Importo.Trim) Then
                                    myScadCA.Evasa = True 'GIU20123
                                End If
                                '----------------
                                ArrScadPagCA.Add(myScadCA)
                                NRate += 1
                            End If
                        Next
                    End If
                Catch ex As Exception
                    '''Call Chiudi("ERRORE: " + "Caricamento scadenze - " + ex.Message)
                    NRate = 0
                    TotRate = 0
                    SvuodaGridT()
                    SetLnk()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento scadenze. N° " + strNDoc + "<br>" + ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                myScadPagCA = ""
                For iii = 0 To ArrScadPagCA.Count - 1 'sempre 1
                    If myScadPagCA.Trim <> "" Then myScadPagCA += ";"
                    rowScadPagCa = ArrScadPagCA(iii)
                    myScadPagCA += rowScadPagCa.NRata.Trim & ";"
                    myScadPagCA += rowScadPagCa.Data.Trim & ";"
                    myScadPagCA += rowScadPagCa.Importo.Trim & ";"
                    myScadPagCA += rowScadPagCa.Evasa.ToString.Trim & ";"
                    myScadPagCA += rowScadPagCa.NFC.Trim & ";"
                    myScadPagCA += rowScadPagCa.DataFC.Trim & ";"
                    myScadPagCA += rowScadPagCa.Serie.Trim & ";"
                    myScadPagCA += rowScadPagCa.ImportoF.Trim & ";" 'giu191223
                    myScadPagCA += rowScadPagCa.ImportoR.Trim
                Next
                '-ok
                SqlDbNewCmd.Parameters.Item("@ScadPagCA").Value = myScadPagCA.Trim 'lo mando ma viene aggiornato DA UPGScadPagCA
                SqlDbNewCmd.Parameters.Item("@ScadAttCA").Value = "" ' qui carico eventualmente le attivita' evase per il dettaglio 
                'GIU271123
                If chkFattRiep.Checked = False Then
                    SqlDbNewCmd.Parameters.Item("@DesRefInt").Value = "Rif. Ns " + aDataViewScPagAtt.Item(i)("SiglaCA") + " N° " _
                       + strNDoc + " Rata " + aDataViewScPagAtt.Item(i)("DesRata") + " Scadenza " + myDataScRata
                Else
                    SqlDbNewCmd.Parameters.Item("@DesRefInt").Value = "Rif. Ns " + aDataViewScPagAtt.Item(i)("SiglaCA") + " N° " _
                       + strNDoc + myDesRataRiep
                End If
                '------
                SqlDbNewCmd.Parameters.Item("@IDTipoFatt").Value = aDataViewScPagAtt.Item(i)("IDTipoFatt")
                SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = aDataViewScPagAtt.Item(i)("FatturaPA")
                SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = aDataViewScPagAtt.Item(i)("SplitIVA")
                SqlDbNewCmd.Parameters.Item("@Cod_Pagamento").Value = aDataViewScPagAtt.Item(i)("Cod_Pagamento")
                SqlDbNewCmd.Parameters.Item("@TotImpon1").Value = aDataViewScPagAtt.Item(i).Item("TotImponF1") 'giu251123 aDataViewScPagAtt.Item(i)("TotImpon1")
                SqlDbNewCmd.Parameters.Item("@TotIVA1").Value = aDataViewScPagAtt.Item(i).Item("TotIVAF1") 'giu251123 aDataViewScPagAtt.Item(i)("TotIVA1")
                SqlDbNewCmd.Parameters.Item("@IVAFattCA").Value = aDataViewScPagAtt.Item(i)("IVAFattCA")
                SqlDbNewCmd.Parameters.Item("@CMagazzino").Value = ddlMagazzino.SelectedValue
                SqlDbNewCmd.Parameters.Item("@MyList").Value = myList 'GIU280321
                'FINE ASSEGNAZIONE
                Try
                    SqlConn.Open()
                    SqlDbNewCmd.CommandTimeout = myTimeOUT
                    SqlDbNewCmd.ExecuteNonQuery()
                    SqlConn.Close()
                    Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
                Catch ExSQL As SqlException
                    '''Call Chiudi("ERRORE: " + "NUOVA FATTURA SQL - " + ExSQL.Message)
                    SvuodaGridT()
                    SetLnk()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUOVA FATTURA SQL. N° " + strNDoc + "<br>" + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                Catch Ex As Exception
                    '''Call Chiudi("ERRORE: " + "NUOVA FATTURA SQL - " + Ex.Message)
                    SvuodaGridT()
                    SetLnk()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUOVA FATTURA SQL. N° " + strNDoc + "<br>" + Ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                '-aggiorno il contratto per le scadenze
                Try
                    'giu270220
                    If UPGScadPagCA(myID.Trim, ArrScadPagCA, myScadPagCA, StrErrore) = False Then
                        '''Call Chiudi("ERRORE: " + "Aggiorna Scadenze. N° " + strNDoc + " - " + StrErrore)
                        SvuodaGridT()
                        SetLnk()
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Aggiorna Scadenze. N° " + strNDoc + "<br>" + StrErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Catch ex As Exception
                    '''Call Chiudi("ERRORE: " + "Aggiorna Scadenze. N° " + strNDoc + " - " + StrErrore)
                    SvuodaGridT()
                    SetLnk()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Aggiorna Scadenze. N° " + strNDoc + "<br>" + ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                'OK PROVA DI STAMPA PER CONTROLLO 
                Try
                    myID = Session(IDDOCUMENTI)
                    If IsNothing(myID) Then
                        myID = ""
                    End If
                    If String.IsNullOrEmpty(myID) Then
                        myID = ""
                    End If
                    If Not IsNumeric(myID) Then
                        '''Call Chiudi("ERRORE: " + "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                        SvuodaGridT()
                        SetLnk()
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO..N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Catch ex As Exception
                    '''Call Chiudi("ERRORE: " + "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                    SvuodaGridT()
                    SetLnk()
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO..N° DOC.: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                Dim IDOKFC As Long = CLng(myID)
                '-----------------------------------------------------------
                If CKFatturaPA = False Then
                    txtPrimoNFattura.Text = NDoc + 1
                    txtPrimoNFattura.Text = FormattaNumero(txtPrimoNFattura.Text.Trim)
                Else
                    txtPrimoNFatturaPA.Text = NDocPA + 1
                    txtPrimoNFatturaPA.Text = FormattaNumero(txtPrimoNFatturaPA.Text.Trim)
                End If
                '-               
                Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
                If OKStampaDoc(IDOKFC, NDocOK, StrErrore) = False Then
                    SvuodaGridT()
                    SetLnk()
                    '''Call Chiudi("ERRORE: " + "CONTROLLO STAMPA FATTURA - " + StrErrore)
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CONTROLLO TOTALE DOCUMENTO.N° DOC.: " & strNDoc + "<br>" + StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            '''Call Chiudi("ERRORE: " + "CREAZIONE FATTURA PER IL DOCUMENTO .N° DOC.: " & strNDoc & " Rata: " & myNRata.ToString.Trim & " - " & ex.Message.Trim)
            SvuodaGridT()
            SetLnk()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "ERRORE: " + "CREAZIONE FATTURA PER IL DOCUMENTO .N° DOC.: " & strNDoc & " Rata: " & myNRata.ToString.Trim & "<br>" & ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        Call StampaDocumentiAll2() 'GIU020223 SOLO SENZA SCONTI

        SvuodaGridT()
        SetLnk()
        If strScOltre.Trim <> "" Then
            Session(LABELMESSAGERED) = SWSI
        End If
        'giu010223 richiesta Francesca stampa subito tutto

        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "StampaDocumentiAll1"
        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CallChiudi"
        ' ''ModalPopup.Show("Crea TUTTE le Fatture da Scadenze selezionate", _
        ' ''    "Creazione fatture avvenuta con sussesso. <br> <strong><span> " & _
        ' ''    Mid(strScOltre.Trim, 1, 150) + "<br>" & _
        ' ''    "Vuole stampare tutte le fatture appena create ?</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        ' ''Exit Sub
        '-------------------------------------------------
    End Sub
    'giu150312
    Private Function Calcola_Scadenze(ByVal IdDocumento As Long, ByVal strNDoc As String, ByVal DataDoc As DateTime, ByVal CPag As Integer, ByVal TotaleDoc As Decimal, ByVal Valuta As String, ByRef strErrore As String) As Boolean

        '------------------------------------
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = GetDatiValute(Valuta, strErrore).Decimali
            If IsNothing(DecimaliVal) Then DecimaliVal = ""
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
        End If
        '------------------------------------
        Dim DecimaliValuta As Integer = Int(DecimaliVal)

        Calcola_Scadenze = True
        Dim Arr_Giorni(5) As String
        Dim Arr_Scad(5) As String
        Dim Arr_Impo(5) As Decimal
        'Dim NumDec As Integer adesso DecimaliValuta
        Dim NumRate As Integer = 0
        Dim Tot_Rate As Decimal = 0
        Dim ind As Integer = 0

        'Codice Pagamento (da TD0)
        If CPag = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. : " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If

        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From Pagamenti WHERE Codice = " & CPag.ToString.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CPag.ToString.Trim & " N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Calcola_Scadenze = False
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CPag.ToString.Trim & " N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Calcola_Scadenze = False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura pagamenti: " & Ex.Message & " N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End Try
        Comodo = IIf(IsDBNull(rowPag(0).Item("Numero_Rate")), "0", rowPag(0).Item("Numero_Rate"))
        If String.IsNullOrEmpty(Comodo) Then Comodo = "0"
        NumRate = CLng(Comodo)

        If CompilaScadenze(rowPag, DataDoc, TotaleDoc, _
            DecimaliValuta, NumRate, Arr_Giorni, Arr_Scad, Arr_Impo, Tot_Rate, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, "") & " N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
        lblDataScad1 = "" : lblDataScad2 = "" : lblDataScad3 = "" : lblDataScad4 = "" : lblDataScad1 = ""
        lblImpRata1 = "" : lblImpRata2 = "" : lblImpRata3 = "" : lblImpRata4 = "" : lblImpRata5 = ""
        Dim TotRate As Decimal = 0
        For ind = 0 To UBound(Arr_Scad) - 1
            Select Case ind
                Case 0
                    lblDataScad1 = Arr_Scad(ind)
                    lblImpRata1 = Arr_Impo(ind)
                    If lblImpRata1.Trim <> "" Then
                        If CDec(lblImpRata1.Trim) > 0 Then
                            lblImpRata1 = FormattaNumero(CDec(lblImpRata1.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata1.Trim)
                        Else
                            lblImpRata1 = ""
                        End If
                    End If
                Case 1
                    lblDataScad2 = Arr_Scad(ind)
                    lblImpRata2 = Arr_Impo(ind)
                    If lblImpRata2.Trim <> "" Then
                        If CDec(lblImpRata2.Trim) > 0 Then
                            lblImpRata2 = FormattaNumero(CDec(lblImpRata2.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata2.Trim)
                        Else
                            lblImpRata2 = ""
                        End If
                    End If
                Case 2
                    lblDataScad3 = Arr_Scad(ind)
                    lblImpRata3 = Arr_Impo(ind)
                    If lblImpRata3.Trim <> "" Then
                        If CDec(lblImpRata3.Trim) > 0 Then
                            lblImpRata3 = FormattaNumero(CDec(lblImpRata3.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata3.Trim)
                        Else
                            lblImpRata3 = ""
                        End If
                    End If
                Case 3
                    lblDataScad4 = Arr_Scad(ind)
                    lblImpRata4 = Arr_Impo(ind)
                    If lblImpRata4.Trim <> "" Then
                        If CDec(lblImpRata4.Trim) > 0 Then
                            lblImpRata4 = FormattaNumero(CDec(lblImpRata4.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata4.Trim)
                        Else
                            lblImpRata4 = ""
                        End If
                    End If
                Case 4
                    lblDataScad5 = Arr_Scad(ind)
                    lblImpRata5 = Arr_Impo(ind)
                    If lblImpRata5.Trim <> "" Then
                        If CDec(lblImpRata5.Trim) > 0 Then
                            lblImpRata5 = FormattaNumero(CDec(lblImpRata5.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata5.Trim)
                        Else
                            lblImpRata5 = ""
                        End If
                    End If
            End Select
        Next ind
        LblTotaleRate = FormattaNumero(TotRate, DecimaliValuta)
        Dim myTotaleDoc As Decimal = FormattaNumero(TotaleDoc, DecimaliValuta)
        If Tot_Rate <> myTotaleDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Totale documento (" & FormattaNumero(TotaleDoc, DecimaliValuta) & ") diverso dal Totale Rate (" & FormattaNumero(TotRate, DecimaliValuta) & ") - N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
    End Function
    'giu160312 GIU010620 SE NON E' RICHIESTA LA STAMPA SINGOLA NON C'è BISOGNO DI ESEGUIRE LA STAMPA 
    'PERO E' MEGLIO IN QUANTO VIENE SEGNALA ERRORE NEL CASO IN CUI I CONTI NON SONO STATI CALCOLATI CORRETTAMENTE
    Private Function OKStampaDoc(ByVal IDDocFC As Long, ByVal NDoc As Long, ByRef strErrore As String) As Boolean
        OKStampaDoc = True
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        strErrore = ""
        Dim SWSconti As Boolean = False
        Dim ELDocToPrintSCY As New List(Of String)
        Dim ELDocToPrintSCN As New List(Of String)
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(IDDocFC.ToString.Trim, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, "Cli", DsPrinWebDoc, ObjReport, SWSconti, strErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 0
                If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
                    ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
                End If
                ELDocToPrintSCN.Add(IDDocFC.ToString.Trim)
                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
                Session(CSTSWConfermaDoc) = 0
            Else
                OKStampaDoc = False
                ELDocToPrintSCN = Nothing
                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
                ELDocToPrintSCY = Nothing
                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            End If
        Catch ex As Exception
            OKStampaDoc = False
            ELDocToPrintSCN = Nothing
            Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
            ELDocToPrintSCY = Nothing
            Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            strErrore = "Errore:" & ex.Message
        End Try

    End Function

    Private Sub StampaDocumentiAll2()
        If IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Non è stato possibile stampare le Fatture appena emesse,<br> si prega di verificare nella sezione Documenti emessi.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrintSCN As List(Of String) = Session(EL_DOC_TOPRINT_SCN)
        Dim TotFatture As Integer = 0
        If (ELDocToPrintSCN.Count > 0) Then
            TotFatture = ELDocToPrintSCN.Count
            Try
                For Each Codice As String In ELDocToPrintSCN
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 0
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Exit Sub
        End If
        ObjDB = Nothing
        '---------
        'GIU010223 DIRETTAMENTE IL PDF
        Call ExportPDF(TotFatture)
        '-----------------------------
        ' ''Session(ATTESA_CALLBACK_METHOD) = "CallChiudi"
        ' ''Session(CSTNOBACK) = 1
        ' ''Attesa.ShowStampaAll2("Totale Fatture NO SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, pLnkRef)
        '-----------------------------
    End Sub
    Public Sub CallChiudi()
        Chiudi("")
    End Sub
    'giu030620 

    Private Function UPGScadPagCA(ByVal _myID As String, ByVal _ArrScadPag As ArrayList, ByVal _ScadPagCA As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        Dim SqlUpd_ConTScadPag = New System.Data.SqlClient.SqlCommand
        SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        Dim strValore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        Try
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '--- Parametri
            SqlUpd_ConTScadPag.CommandText = "[Update_ConTScadPagCAByIDDoc]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_1"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_1", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_2"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_2", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_3"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_3", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_4"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_4", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8, "Data_Scadenza_5"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Rata_5", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadPagCA"))
            '
            SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = _myID
            'giu040520
            Dim rowScadPagCa As ScadPagCAEntity = Nothing
            Dim NScad As Integer = 0
            For i = 0 To _ArrScadPag.Count - 1
                rowScadPagCa = ArrScadPagCA(i)
                If rowScadPagCa.Evasa = False And rowScadPagCa.Data.Trim <> "" Then
                    NScad += 1
                    If NScad < 6 Then
                        If IsDate(rowScadPagCa.Data.Trim) Then
                            SqlUpd_ConTScadPag.Parameters("@Data_Scadenza_" & Format(NScad, "#0") & "").Value = CDate(rowScadPagCa.Data)
                        Else
                            'non passonulla ...defaultNULL
                        End If
                        If rowScadPagCa.Importo.Trim = "" Or Not IsNumeric(rowScadPagCa.Importo.Trim) Then
                            SqlUpd_ConTScadPag.Parameters("@Rata_" & Format(NScad, "#0") & "").Value = 0
                        Else
                            SqlUpd_ConTScadPag.Parameters("@Rata_" & Format(NScad, "#0") & "").Value = CDec(rowScadPagCa.Importo.Trim)
                        End If
                    Else
                        Exit For
                    End If

                End If
            Next
            '---------------
            SqlUpd_ConTScadPag.Parameters("@ScadPagCA").Value = IIf(_ScadPagCA.Trim = "", "", _ScadPagCA.Trim)
            SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
            SqlUpd_ConTScadPag.ExecuteNonQuery()
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If
            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
        End Try
    End Function

    Private Function ExportPDF(ByVal TotFatture As Integer) As Boolean
        ExportPDF = False
        LnkFatturePDF.Visible = False
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim 'GIU210120
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        CodiceDitta = Session(CSTCODDITTA)
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                    End If
                End If
            End If
        Catch ex As Exception
            'nulla 
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        If IsNothing(CodiceDitta) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: CodiceDitta non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: CodiceDitta non valido.")
            Exit Function
        End If
        If CodiceDitta = "" Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: CodiceDitta non valido.")
            Exit Function
        End If
        '-------------------------------------------------------------------
        Dim SWSconti As Integer = 1
        Dim Rpt As Object = Nothing
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("DocumentiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("DocumentiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            'ok
        End Try
        'giu010223 no perche credo il PDF CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        If Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
            'giu251211
            If SWSconti = 1 Then
                Rpt = New Fattura
                If CodiceDitta = "01" Then
                    Rpt = New Fattura01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New Fattura05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New Fattura0501
                End If
            Else
                Rpt = New FatturaNoSconti
                If CodiceDitta = "01" Then
                    Rpt = New FatturaNoSconti01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New FatturaNoSconti05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New FatturaNoSconti0501
                End If
            End If
        Else
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale STAMPA DOCUMENTO NON PREVISTA"
            Try
                Response.Redirect(strRitorno)
                Exit Function
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Function
            End Try
            Exit Function
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)
        'giu010223 no perche credo il PDF CrystalReportViewer1.ReportSource = Rpt
        Dim SubDirDOC = "Fatture"
        Session(CSTNOMEPDF) = "FattContratti" & Format(Now, "yyyyMMddHHmmss") + ".PDF"
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
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
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Function
        End Try
        LnkFatturePDF.Visible = True
        LnkElencoSc.Visible = True
        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        LnkFatturePDF.HRef = LnkName
        LnkElencoSc.HRef = LnkName
        'LnkFatture.Title = "TOTALE: " + FormattaNumero(TotFatture)
        'LnkFatture.HRef = pLnkRef
        'LnkFatture.Visible = True

        ' ''Session(ATTESA_CALLBACK_METHOD) = "CallChiudi"
        ' ''Session(CSTNOBACK) = 1
        ' ''Attesa.ShowStampaAll2("Totale Fatture NO SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, pLnkRef)
    End Function

#End Region

    Private Sub chkSoloEvase_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSoloEvase.CheckedChanged
        SvuodaGridT()
    End Sub

    Private Sub chkVisALLSc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkVisALLSc.CheckedChanged
        BuildDettD()
    End Sub

    'Private Sub txtDataFattura_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataFattura.TextChanged
    '    If IsDate(txtDataFattura.Text.Trim) Then
    '        txtDataFattura.BackColor = SEGNALA_OK
    '        txtDataFattura.Text = Format(CDate(txtDataFattura.Text), FormatoData)
    '    Else
    '        txtDataFattura.BackColor = SEGNALA_KO
    '        txtDataFattura.Text = ""
    '    End If
    'End Sub

    Private Sub chkCTRFatturato_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCTRFatturato.CheckedChanged
        If chkCTRFatturato.Checked = False Then
            DDLCTRFatturato.Enabled = chkCTRFatturato.Checked
            DDLCTRFatturato.SelectedIndex = -1
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun CONTRATTO selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If GetFCToCM() = True Then
            If DDLCTRFatturato.Items.Count > 0 Then
                DDLCTRFatturato.SelectedIndex = 1
            End If
        Else
            chkCTRFatturato.Checked = False
            Exit Sub
        End If
        DDLCTRFatturato.Enabled = chkCTRFatturato.Checked
    End Sub

    Private Function GetFCToCM() As Boolean
        DDLCTRFatturato.Items.Clear()
        DDLCTRFatturato.Items.Add("")
        GetFCToCM = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: SESSIONE SCADENZE SCADUTA (ID)", "Riprovate la modifica uscendo e rientrando (GetFCToCM).", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        '-
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnDocCM As New SqlConnection
        SqlConnDocCM.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'TETSTATA
        Try
            Dim dsT As New DataSet
            Dim SqlAdapDocT As New SqlDataAdapter
            Dim SqlDbSelectDocT As New SqlCommand
            SqlDbSelectDocT.CommandText = "get_DocumentiCollegatiCM" 'get_DocCollegatiByIDCM"
            SqlDbSelectDocT.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocT.Connection = SqlConnDocCM
            SqlDbSelectDocT.CommandTimeout = myTimeOUT
            SqlDbSelectDocT.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocT.SelectCommand = SqlDbSelectDocT
            '==============CARICAMENTO e MERGE DATASET==============
            SqlDbSelectDocT.Parameters.Item("@IDDocumenti").Value = myID.Trim
            SqlAdapDocT.Fill(dsT)
            If (dsT.Tables.Count > 0) Then
                If (dsT.Tables(0).Rows.Count > 0) Then
                    Dim rowPag() As DataRow
                    rowPag = dsT.Tables(0).Select("Tipo_Doc='FC'")
                    If rowPag.Length = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Controllo Fatturato per Contratto", "Nessuna Fattura emessa per l'Ordine collegato.", WUC_ModalPopup.TYPE_INFO)
                        Exit Function
                    End If
                    Dim myAnnoFC As String = ""
                    Dim myDataFC As String = "" : Dim myNumFC As String = "" : Dim myFCPA As String
                    Dim myTotNettoPagare As String = ""
                    Dim SWOKAbbinati As Boolean = False

                    For Each row In dsT.Tables(0).Select("Tipo_Doc='FC'", "Data_Doc DESC")
                        myNumFC = IIf(IsDBNull(row.Item("Numero")), "", row.Item("Numero"))
                        myDataFC = IIf(IsDBNull(row.Item("Data_Doc")), "", row.Item("Data_Doc"))
                        myFCPA = IIf(IsDBNull(row.Item("FatturaPA")), "", row.Item("FatturaPA"))
                        myTotNettoPagare = IIf(IsDBNull(row.Item("TotNettoPagare")), "0", row.Item("TotNettoPagare").ToString.Trim)
                        Try
                            If String.IsNullOrEmpty(myTotNettoPagare) Then
                                myTotNettoPagare = "0"
                            Else
                                myTotNettoPagare = Format(CDec(myTotNettoPagare.Trim), FormatoValEuro)
                            End If
                        Catch ex As Exception
                            myTotNettoPagare = "0"
                        End Try
                        '-
                        DDLCTRFatturato.Items.Add(Format(CDate(myDataFC), FormatoData) + " - " + myTotNettoPagare + " - (N°" + myNumFC.Trim + myFCPA + ")")
                        DDLCTRFatturato.Items(0).Value = row.Item("IDDocumenti")
                    Next
                    GetFCToCM = True
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Controllo Fatturato per Contratto", "Nessuna Fattura collegata.", WUC_ModalPopup.TYPE_INFO)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo Fatturato per Contratto", "Nessuna Fattura collegata.", WUC_ModalPopup.TYPE_INFO)
                Exit Function
            End If
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "Caricamento Documenti collegati al Contratto: " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento Documenti collegati al Contratto: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Private Sub chkFattRiep_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFattRiep.CheckedChanged
        chkCTRFatturato.Checked = False
        DDLCTRFatturato.Enabled = False
        DDLCTRFatturato.Items.Clear()
        DDLCTRFatturato.Items.Add("")
    End Sub
End Class