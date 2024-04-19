Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App 'written by Marco
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Documenti
Imports System.Drawing

Partial Public Class WUC_ContrattiDett
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    'giu160412
    Private myTipoDoc As String = ""
    Private myTabCliFor As String = ""
    Private SWPrezzoAL As String = ""
    Dim Passo As Integer = 0 'giu230412 per individuare gli errori: dove ero quando è andato in errore

    Dim SWGeneraPeriodi As Boolean = False 'giu090222

#Region "Def per gestire il GRID dettagli"
    Private aDataView1 As DataView
    Private aDataView1L As DataView
    '---
    Private SqlAdapDocDett As SqlDataAdapter
    Private SqlAdapDocDettL As SqlDataAdapter
    Private SqlAdapDocForInsertD As SqlDataAdapter 'GIU190220
    '---
    Private SqlConnDocDett As SqlConnection
    Private SqlConnDocDettL As SqlConnection
    '---
    Private SqlDbSelectCmd As SqlCommand
    Private SqlDbInserCmd As SqlCommand
    Private SqlDbUpdateCmd As SqlCommand
    Private SqlDbDeleteCmd As SqlCommand
    'giu250220
    Private SqlAdapDocALLAtt As SqlDataAdapter
    Private SqlDbSelectCmdALLAtt As SqlCommand
    '---
    Private SqlDbSelectCmdL As SqlCommand
    Private SqlDbInserCmdL As SqlCommand
    Private SqlDbUpdateCmdL As SqlCommand
    Private SqlDbDeleteCmdL As SqlCommand
    '---
    Private DsContrattiDett As New DSDocumenti
    Private DsContrattiDettL As New DSDocumenti
    '---
    Private DsDocDettForInsertD As New DSDocumenti 'GIU190220

    Public Enum CellIdx
        Riga = 3
        CodArt = 4
        DesArt = 5
        SerieLotto = 6
        UM = 7
        Qta = 8
        QtaEv = 9
        QtaIn = 10
        QtaAl = 11
        QtaRe = 12
        QtaFa = 13
        DataSc = 14
        DataEv = 15
        IVA = 16
        Prz = 17
        TipoScM = 18
        ScVal = 19
        Sc1 = 20
        Sc2 = 21
        Note = 22
        Importo = 23
        Deduzione = 24
        ScR = 25
        CAge = 26
        PAge = 27
        ImpProvvAge = 28
    End Enum
    Public Enum CellIdxL
        NCollo = 3
        Lotto = 4
        NSerie = 5
        QtaCollo = 6
    End Enum

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not String.IsNullOrEmpty(Session(ERROREALL)) Then
                If Session(ERROREALL) = SWSI Then
                    Session(ERROREALL) = SWNO
                    Exit Sub
                End If
            End If
        Catch ex As Exception
        End Try
        ModalPopup.WucElement = Me
        WFP_Articolo_Seleziona1.WucElement = Me
        WFPElencoDestCF.WucElement = Me
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRespAreaApp.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDSRespVisiteApp.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        Try
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
                Session(ERROREALL) = SWSI
                _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
                Exit Sub
            End If
        Catch ex As Exception
            Session(ERROREALL) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.Chiudi", ex.Message & " " & "Errore: TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        'GIU170112 giu140512 i lotti solo nei documenti di trasporto
        '''If myTipoDoc = SWTD(TD.Preventivi) Or _
        '''   myTipoDoc = SWTD(TD.OrdClienti) Or _
        '''   myTipoDoc = SWTD(TD.OrdDepositi) Or _
        '''   myTipoDoc = SWTD(TD.OrdFornitori) Or _
        '''   myTipoDoc = SWTD(TD.PropOrdFornitori) Then
        '''    PanelDettArtLottiNrSerie.Enabled = False
        '''    ' ''PanelDettArtLottiNrSerie.Visible = False
        '''    PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
        '''End If
        '-----------------------------------------------------------
        Dim myEs As String = Session(ESERCIZIO)
        If IsNothing(myEs) Then
            myEs = ""
        End If
        If String.IsNullOrEmpty(myEs) Then
            myEs = ""
        End If
        If myEs.Trim = "" Or Not IsNumeric(myEs) Then
            _WucElement.Chiudi("Errore: ESERCIZIO SCONOSCIUTO (ContrattiDett.Load): " & myEs.Trim)
            Exit Sub
        End If
        txtCodArtIns.MaxLength = App.GetParamGestAzi(Session(ESERCIZIO)).LunghezzaMaxCodice 'GIU170112 + CSTLOpz

        If Not IsPostBack Then
            lblMessAgg.ForeColor = Drawing.Color.Blue
            lblSuperatoScMax.ForeColor = Drawing.Color.Blue
            checkNoScontoValore.ForeColor = Drawing.Color.DarkBlue
            'giu190412 GIU270412
            SetSWPrezzoALCSG()
            '----------
            SetCdmDAdp()
            'giu241111
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            'giu050220
            Dim myIDDurataNum As String = Session(IDDURATANUM)
            If IsNothing(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            '-
            Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
            If IsNothing(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            '----------
            If Session(SWOP) = SWOPNUOVO Then
                DsContrattiDett.ContrattiD.Clear()
                DsContrattiDett.ContrattiD.AcceptChanges()
                '-
                DsContrattiDettL.ContrattiDLotti.Clear()
                DsContrattiDettL.ContrattiDLotti.AcceptChanges()

                Session("aDataView1") = aDataView1
                Session("aSqlAdap") = SqlAdapDocDett
                Session("aDsDett") = DsContrattiDett
                '------------------------------------------------------------------------------------
            Else
                If myID = "" Or Not IsNumeric(myID) Then
                    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (DocDett.Load)")
                    Exit Sub
                End If
                SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
                SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
                SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
            End If
            DDLTipoDettagli.AutoPostBack = False
            Call PosizionaItemDDL(myIDDurataNum, DDLTipoDettagli)
            DDLTipoDettagli.AutoPostBack = True
            Call SetDDLDettDurNumRiga()
            AzzQtaNULL()
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            '---
            Call ImpostaGriglia()
            '---
            Call AzzeraTxtInsArticoli() 'giu021211
            '---
            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            Session("aDataView1") = aDataView1
            GridViewDett.DataBind()
            If GridViewDett.Rows.Count = 0 Then
                SetBtnPrimaRigaEnabled(True)
                'giu250220 la prima volta e solo per DurataNum = 0 APP. inserisco in auto la riga per il CHEK 
                Dim newRowDocD As DSDocumenti.ContrattiDRow '= DsContrattiDett.ContrattiD.NewContrattiDRow
                Dim CodArt As String = "" : Dim pNVisite As Integer = 0
                newRowDocD = DsContrattiDett.ContrattiD.NewContrattiDRow
                With newRowDocD
                    .BeginEdit()
                    .IDDocumenti = CLng(myID)
                    .DurataNum = Val(myIDDurataNum)
                    .DurataNumRiga = Val(myIDDurataNumR)
                    .Riga = 1
                    .Prezzo = 0
                    .Prezzo_Netto = 0
                    .Qta_Allestita = 0
                    .Qta_Evasa = 0
                    .Qta_Impegnata = 0
                    .Qta_Ordinata = 0
                    .Qta_Prenotata = 0
                    .Qta_Residua = 0
                    .Importo = 0
                    'giu170412
                    .PrezzoAcquisto = 0
                    .PrezzoListino = 0
                    'giu190412
                    .SWModAgenti = False
                    .PrezzoCosto = 0
                    .Qta_Inviata = 0
                    .Qta_Fatturata = 0
                    '---------
                    'GIU250220
                    .SetRiga_AppartenenzaNull()
                    '---------
                    If Val(myIDDurataNum) = 0 Then
                        chkNoPrezzo.AutoPostBack = False : chkNoPrezzo.Checked = False
                        If GetCodVisitaDATipoCAIdPag(CodArt, pNVisite) = False Then
                            'ok proseguo lo stesso
                        End If
                        If CodArt.Trim <> "" And pNVisite > 0 Then
                            .Cod_Articolo = CodArt
                            If GetDatiAnaMag(newRowDocD, CodArt, True) = False Then
                                'ok proseguo lo stesso
                            End If
                        End If
                        chkNoPrezzo.AutoPostBack = True : chkNoPrezzo.Checked = True
                    End If
                    '-
                    .EndEdit()
                End With
                Try
                    DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                    'GIU250220 inserisco tante righe quante sono il NVisite a partire dalla 2
                    If Val(myIDDurataNum) = 0 And pNVisite > 1 And CodArt.Trim <> "" Then
                        For i = 2 To pNVisite
                            newRowDocD = DsContrattiDett.ContrattiD.NewContrattiDRow
                            With newRowDocD
                                .BeginEdit()
                                .IDDocumenti = CLng(myID)
                                .DurataNum = Val(myIDDurataNum)
                                .DurataNumRiga = Val(myIDDurataNumR)
                                .Riga = i
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Qta_Allestita = 0
                                .Qta_Evasa = 0
                                .Qta_Impegnata = 0
                                .Qta_Ordinata = 0
                                .Qta_Prenotata = 0
                                .Qta_Residua = 0
                                .Importo = 0
                                'giu170412
                                .PrezzoAcquisto = 0
                                .PrezzoListino = 0
                                'giu190412
                                .SWModAgenti = False
                                .PrezzoCosto = 0
                                .Qta_Inviata = 0
                                .Qta_Fatturata = 0
                                '---------
                                'GIU250220
                                .SetRiga_AppartenenzaNull()
                                '---------
                                chkNoPrezzo.AutoPostBack = False : chkNoPrezzo.Checked = False
                                .Cod_Articolo = CodArt
                                If GetDatiAnaMag(newRowDocD, CodArt, True) = False Then
                                    'ok proseguo lo stesso
                                End If
                                chkNoPrezzo.AutoPostBack = True : chkNoPrezzo.Checked = True
                                '-
                                .EndEdit()
                            End With
                            DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                        Next
                    End If
                    '-------------------------------------------------------------------------
                    newRowDocD = Nothing
                    SqlAdapDocDett = Session("aSqlAdap")
                Catch Ex As Exception
                    _WucElement.Chiudi("Errore nel caricamento ContrattiDett_Load: inserimento 1° Riga: " & Ex.Message)
                    Exit Sub
                End Try
                Session("aDataView1") = aDataView1
                Session("aSqlAdap") = SqlAdapDocDett
                Session("aDsDett") = DsContrattiDett
                Session("aDataView1") = aDataView1
                GridViewDett.DataBind()
                SetBtnPrimaRigaEnabled(False)
                '------------------------------------------------------------------------------------
                'GIU17042020 QUI NON AGG. XKE SONO NEL SINGOLO APP.PERIODO _WucElement.SetLblTotLMPL(0, 0)
                lblTotDett.Text = HTML_SPAZIO
            Else
                SetBtnPrimaRigaEnabled(False)
                'GIU17042020 QUI NON AGG. 'giu180420 non agg il TOTALE DOC. XKE SONO NEL SINGOLO APP.PERIODO
                Dim TotaleDett As Decimal = 0
                For Each rsDettagli In DsContrattiDett.ContrattiD.Select("", "Riga")
                    rsDettagli.BeginEdit()
                    If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                    rsDettagli.EndEdit()
                    rsDettagli.AcceptChanges()
                    If rsDettagli!DedPerAcconto = True Then
                        ' ''TotaleDeduzioni += rsDettagli![Importo]
                    Else
                        TotaleDett += rsDettagli![Importo]
                    End If
                Next
                'Valuta per i decimali per il calcolo
                Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
                If IsNothing(DecimaliVal) Then DecimaliVal = "2"
                If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
                If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                    DecimaliVal = "2" 'Euro
                End If
                lblTotDett.Text = FormattaNumero(TotaleDett, Int(DecimaliVal))
                '---------------------------------------------------------------------------------------------------------------
                'giu120318 aggiorno anche il Totale Merce in 3 pagina  
                'GIU17042020 QUI NON AGG. XKE SONO NEL SINGOLO APP.PERIODO
                ' ''Dim TotaleLordoMerce As Decimal = 0
                ' ''Dim TotaleDeduzioni As Decimal = 0
                ' ''For Each rsDettagli In DsContrattiDett.ContrattiD.Select("", "Riga") 'giu290519 TOLTO "Importo<>0" 
                ' ''    'giu020519 FATTURE PER ACCONTI 
                ' ''    rsDettagli.BeginEdit()
                ' ''    If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                ' ''    rsDettagli.EndEdit()
                ' ''    rsDettagli.AcceptChanges()
                ' ''    If rsDettagli!DedPerAcconto = True Then
                ' ''        TotaleDeduzioni += rsDettagli![Importo]
                ' ''    Else
                ' ''        Select Case Left(myTipoDoc, 1)
                ' ''            Case "O"
                ' ''                If rsDettagli![Qta_Ordinata] <> 0 Then
                ' ''                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                ' ''                End If
                ' ''            Case Else
                ' ''                If myTipoDoc = "PR" Or myTipoDoc = "TC" Or myTipoDoc = "CA" Then 'GIU021219
                ' ''                    If rsDettagli![Qta_Ordinata] <> 0 Then
                ' ''                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                ' ''                    End If
                ' ''                Else
                ' ''                    If rsDettagli![Qta_Evasa] <> 0 Then
                ' ''                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                ' ''                    End If
                ' ''                End If
                ' ''        End Select
                ' ''    End If
                ' ''Next
                'Valuta per i decimali per il calcolo
                ' ''Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
                ' ''If IsNothing(DecimaliVal) Then DecimaliVal = "2"
                ' ''If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
                ' ''If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                ' ''    DecimaliVal = "2" 'Euro
                ' ''End If
                'GIU17042020 QUI NON AGG. XKE SONO NEL SINGOLO APP.PERIODO_WucElement.SetLblTotLMPL(TotaleLordoMerce, TotaleDeduzioni)
            End If
            '--- LOTTI
            Dim RigaSel As Integer = 0
            Try
                GridViewDett.SelectedIndex = 0
                RigaSel = GridViewDett.SelectedIndex 'giu180419
                If RigaSel = 0 Then 'giu180419
                    RigaSel = GridViewDett.SelectedDataKey.Value
                End If
                If RigaSel > 0 Then 'giu180419
                    PopolaTxtDett()
                Else
                    AzzeraTxtInsArticoli()
                End If
            Catch ex As Exception
                RigaSel = 0
                'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
                LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
                Call AzzeraTxtInsArticoli() 'giu021211
            End Try
            '-- LOTTI 
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            '''BuildLottiRigaDB(RigaSel)
            '---------
            EnableTOTxtInsArticoli(False)
            Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWMODIFICATO) = SWNO
        End If
        If DDLTipoDettagli.SelectedIndex = 0 Then
            PanelDettArtNoteInterv.Visible = True
        Else
            PanelDettArtNoteInterv.Visible = False
        End If
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_Seleziona1.Show()
        End If
        If Session(F_ELENCO_DESTCFD_APERTA) = True Then
            WFPElencoDestCF.Show()
        End If
    End Sub
    Public Sub SetSWPrezzoALCSG()
        'SG=CSTSEGNOGIACENZA
        'A=Acquisto
        'L=Listino
        'C=Costo (FIFO)
        SWPrezzoAL = "L" 'Listino
        If _WucElement.CKPrezzoALCSG(SWPrezzoAL, "") = False Then
            SWPrezzoAL = "L"
        End If
        If SWPrezzoAL = "A" Then
            lblPrezzoAL.Text = "Prezzo acquisto"
            lblPrezzoAL.ForeColor = Drawing.Color.Blue
        Else
            lblPrezzoAL.ForeColor = Drawing.Color.Black
            lblPrezzoAL.Text = "Prezzo listino"
        End If
    End Sub
    Private Sub AzzQtaNULL()
        Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("")
        Dim RowD As DSDocumenti.ContrattiDRow
        For Each RowD In RowsD
            RowD.BeginEdit()
            If RowD.IsQta_AllestitaNull Then RowD.Qta_Allestita = 0
            If RowD.IsQta_EvasaNull Then RowD.Qta_Evasa = 0
            If RowD.IsQta_ImpegnataNull Then RowD.Qta_Impegnata = 0
            If RowD.IsQta_OrdinataNull Then RowD.Qta_Ordinata = 0
            If RowD.IsQta_PrenotataNull Then RowD.Qta_Prenotata = 0
            If RowD.IsQta_ResiduaNull Then RowD.Qta_Residua = 0
            If RowD.IsPrezzoNull Then RowD.Prezzo = 0
            If RowD.IsPrezzo_NettoNull Then RowD.Prezzo_Netto = 0
            If RowD.IsImportoNull Then RowD.Importo = 0
            'giu170412
            If RowD.IsPrezzoAcquistoNull Then RowD.PrezzoAcquisto = 0
            If RowD.IsPrezzoListinoNull Then RowD.PrezzoListino = 0
            'giu190412
            If RowD.IsSWModAgentiNull Then RowD.SWModAgenti = False
            If RowD.IsPrezzoCostoNull Then RowD.PrezzoCosto = 0
            If RowD.IsQta_InviataNull Then RowD.Qta_Inviata = 0
            If RowD.IsQta_FatturataNull Then RowD.Qta_Fatturata = 0
            '---------
            RowD.EndEdit()
        Next
    End Sub
    Private Sub PopolaTxtDett() 'giu061211
        AzzeraTxtInsArticoli()
        Try
            'GIU260419 
            If (aDataView1 Is Nothing) Then
                aDataView1 = Session("aDataView1")
                If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
                If aDataView1.Count = 0 Then
                    SetBtnPrimaRigaEnabled(True)
                    Session(SWOPDETTDOCR) = SWOPNESSUNA
                    Exit Sub
                Else
                    SetBtnPrimaRigaEnabled(False)
                End If
            End If
            '---------------------
            Dim myRowIndex As Integer = GridViewDett.SelectedIndex + (GridViewDett.PageSize * GridViewDett.PageIndex)
            Try 'giu070222
                DDLModello.SelectedIndex = aDataView1.Item(myRowIndex).Item("QtaDurataNumR0")
            Catch ex As Exception
                DDLModello.SelectedIndex = -1
            End Try
            Try
                lblRigaSel.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Riga"))
            Catch ex As Exception
                lblRigaSel.Text = HTML_SPAZIO
            End Try
            txtCodArtIns.AutoPostBack = False
            Try
                txtCodArtIns.Text = aDataView1.Item(myRowIndex).Item("Cod_Articolo")
            Catch ex As Exception
                txtCodArtIns.Text = ""
            End Try
            txtCodArtIns.AutoPostBack = True
            txtCodArtIns.BackColor = SEGNALA_OK
            Try
                lblBase.Text = aDataView1.Item(myRowIndex).Item("LBase")
            Catch ex As Exception
                lblBase.Text = "0"
            End Try
            Try
                lblOpz.Text = aDataView1.Item(myRowIndex).Item("LOpz")
            Catch ex As Exception
                lblOpz.Text = "0"
            End Try
            Try
                txtDesArtIns.Text = aDataView1.Item(myRowIndex).Item("Descrizione")
            Catch ex As Exception
                txtDesArtIns.Text = ""
            End Try
            Try
                txtUMIns.Text = aDataView1.Item(myRowIndex).Item("UM")
            Catch ex As Exception
                txtUMIns.Text = ""
            End Try
            Try
                txtQtaIns.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Ordinata"), -1)
            Catch ex As Exception
                txtQtaIns.Text = "0"
            End Try
            Try
                lblQtaEv.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Evasa"), -1)
            Catch ex As Exception
                lblQtaEv.Text = "0"
            End Try
            Try
                LblQtaRe.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Residua"), -1)
            Catch ex As Exception
                LblQtaRe.Text = "0"
            End Try
            Try
                lblQtaFa.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Fatturata"), -1)
            Catch ex As Exception
                lblQtaFa.Text = "0"
            End Try
            If CDec(lblQtaFa.Text) > 0 Then
                chkFatturata.AutoPostBack = False
                chkFatturata.Checked = True
                chkFatturata.AutoPostBack = True
            Else
                chkFatturata.AutoPostBack = False
                chkFatturata.Checked = False
                chkFatturata.AutoPostBack = True
            End If
            txtIVAIns.AutoPostBack = False
            Try
                txtIVAIns.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Cod_IVA"))
                txtIVAIns.BackColor = Def.SEGNALA_OK
            Catch ex As Exception
                txtIVAIns.Text = ""
            End Try
            txtIVAIns.AutoPostBack = True
            'giu110112 SCONTO VALORE E CALCOLO PROVVIGIONI
            Dim SWPNettoModificato As Boolean = False
            Try
                SWPNettoModificato = aDataView1.Item(myRowIndex).Item("SWPNettoModificato")
            Catch ex As Exception
                SWPNettoModificato = False
            End Try
            '---------------------------------------------
            Try
                If SWPNettoModificato = False Then
                    txtPrezzoIns.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Prezzo"), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                Else
                    txtPrezzoIns.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Prezzo_Netto_Inputato"), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                End If
                'GIU090222
                chkNoPrezzo.AutoPostBack = False
                If CDec(txtPrezzoIns.Text.Trim) = 0 Then
                    chkNoPrezzo.Checked = True
                Else
                    chkNoPrezzo.Checked = False
                End If
                chkNoPrezzo.AutoPostBack = True
            Catch ex As Exception
                txtPrezzoIns.Text = "0"
            End Try
            checkNoScontoValore.Checked = True
            Try
                If SWPNettoModificato = True Then
                    If aDataView1.Item(myRowIndex).Item("ScontoValore") <> 0 Then
                        checkNoScontoValore.Checked = False
                    Else
                        checkNoScontoValore.Checked = True
                    End If
                End If
            Catch ex As Exception
            End Try
            '---------
            Try
                txtSconto1Ins.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Sconto_1"), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
            Catch ex As Exception
                txtSconto1Ins.Text = "0"
            End Try
            Try
                LblPrezzoNetto.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Prezzo_Netto"), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            Catch ex As Exception
                LblPrezzoNetto.Text = "0"
            End Try
            Try
                LblImportoRiga.Text = Format(aDataView1.Item(myRowIndex).Item("Importo"), FormatoValEuro)
            Catch ex As Exception
                LblImportoRiga.Text = "0"
            End Try
            'giu230412
            Try
                txtPrezzoCosto.BackColor = SEGNALA_OK
                txtPrezzoCosto.Text = Format(aDataView1.Item(myRowIndex).Item("PrezzoCosto"), FormatoValEuro)
                If txtCodArtIns.Text = "" Then
                    If CDbl(txtQtaIns.Text) > 0 Or
                        CDbl(txtQtaIns.Text) > 0 Or
                        CDbl(txtPrezzoIns.Text) > 0 Then
                        txtPrezzoCosto.Enabled = True
                    Else
                        txtPrezzoCosto.Enabled = False
                        txtPrezzoCosto.Text = "0"
                    End If
                Else
                    txtPrezzoCosto.Enabled = False
                    txtPrezzoCosto.Text = "0"
                End If
            Catch ex As Exception
                txtPrezzoCosto.Enabled = False
                txtPrezzoCosto.Text = "0"
            End Try
            '-
            Dim SWSiNo As Boolean = False
            Try
                SWSiNo = aDataView1.Item(myRowIndex).Item("SWSostituito")
            Catch ex As Exception
                SWSiNo = False
            End Try
            chkSWSostituito.Checked = SWSiNo
            Try
                SWSiNo = aDataView1.Item(myRowIndex).Item("SWCalcoloTot")
            Catch ex As Exception
                SWSiNo = False
            End Try
            chkSWCalcoloTot.Checked = SWSiNo
            Dim myData As String = ""
            Try
                myData = aDataView1.Item(myRowIndex).Item("DataSc")
            Catch ex As Exception
                myData = ""
            End Try
            If IsDate(myData) Then
                txtDataSc.Text = Format(CDate(myData), FormatoData)
            Else
                txtDataSc.Text = ""
            End If
            'giu150424
            Try
                myData = aDataView1.Item(myRowIndex).Item("RefDataNC")
            Catch ex As Exception
                myData = ""
            End Try
            If IsDate(myData) Then
                txtDataScCons.Text = Format(CDate(myData), FormatoData)
            Else
                txtDataScCons.Text = ""
            End If
            '---------
            Try
                myData = aDataView1.Item(myRowIndex).Item("DataEv")
            Catch ex As Exception
                myData = ""
            End Try
            If IsDate(myData) Then
                txtDataEv.Text = Format(CDate(myData), FormatoData)
                chkEvasa.AutoPostBack = False
                chkEvasa.Checked = True
                chkEvasa.AutoPostBack = True
            Else
                txtDataEv.Text = ""
                chkEvasa.AutoPostBack = False
                chkEvasa.Checked = False
                chkEvasa.AutoPostBack = True
            End If
            '-
            Dim myStr As String = ""
            Try
                myStr = Formatta.FormattaNomeFile(aDataView1.Item(myRowIndex).Item("Serie").ToString.Trim) 'giu070523
            Catch ex As Exception
                myStr = ""
            End Try
            txtSerie.Text = myStr.Trim
            Try
                myStr = Formatta.FormattaNomeFile(aDataView1.Item(myRowIndex).Item("Lotto")) 'giu070523
            Catch ex As Exception
                myStr = ""
            End Try
            txtLotto.Text = myStr.Trim
            If txtSerie.Text.Trim = "" And txtLotto.Text.Trim = "" Then
                txtSerie.BackColor = SEGNALA_KO
                txtLotto.BackColor = SEGNALA_KO
            Else
                txtSerie.BackColor = SEGNALA_OK
                txtLotto.BackColor = SEGNALA_OK
            End If
            Try
                myStr = aDataView1.Item(myRowIndex).Item("QtaDurataNumR0").ToString.Trim
            Catch ex As Exception
                myStr = "0"
            End Try
            'giu050520 se fosse già selezionato il modello ui non lo modifico assegno quello presente sul DDL
            If DDLModello.SelectedIndex = 0 Then
                Call PosizionaItemDDL(myStr.Trim, DDLModello)
            Else
                aDataView1.Item(myRowIndex).Item("QtaDurataNumR0") = DDLModello.SelectedIndex
            End If
            If DDLModello.SelectedIndex = 0 Then
                DDLModello.BackColor = SEGNALA_KO
            Else
                DDLModello.BackColor = SEGNALA_OK
            End If
            '-------------------------------------------------------------------
            Try
                myStr = aDataView1.Item(myRowIndex).Item("Note")
            Catch ex As Exception
                myStr = ""
            End Try
            txtNote.Text = myStr
            Try
                myStr = aDataView1.Item(myRowIndex).Item("Cod_Filiale")
            Catch ex As Exception
                myStr = ""
            End Try
            Dim myProvApp As String = ""
            Call PopolaDestCliForD(myStr, myProvApp)
            '---------
            'giu231023
            Dim myCArea As String = ""
            Try
                myCArea = aDataView1.Item(myRowIndex).Item("RespArea")
            Catch ex As Exception
                myCArea = ""
            End Try
            Session(IDRESPAREAAPP) = myCArea
            PosizionaItemDDL(myCArea, DDLRespAreaApp)
            '-
            SqlDSRespVisiteApp.DataBind()
            DDLRespVisiteApp.Items.Clear()
            DDLRespVisiteApp.Items.Add("")
            DDLRespVisiteApp.DataBind()
            DDLRespVisiteApp.BackColor = SEGNALA_OK
            '-- mi riposiziono 
            DDLRespVisiteApp.AutoPostBack = False
            DDLRespVisiteApp.SelectedIndex = -1
            DDLRespVisiteApp.AutoPostBack = True
            '-----
            If myCArea.Trim <> "" Then
                Try
                    myStr = aDataView1.Item(myRowIndex).Item("RespVisite")
                Catch ex As Exception
                    myStr = ""
                End Try
                Session(IDRESPVISITEAPP) = myStr
                PosizionaItemDDL(myStr, DDLRespVisiteApp)
                lblMessRespAV.Text = CKRespAreaVisiteByProv(myProvApp, False)
            Else
                lblMessRespAV.Text = CKRespAreaVisiteByProv(myProvApp, False) 'giu1011123 
                Session(IDRESPVISITEAPP) = ""
                myStr = ""
                PosizionaItemDDL(myStr, DDLRespVisiteApp)
                'giu1011123 lblMessRespAV.Text = ""
            End If
        Catch ex As Exception
            AzzeraTxtInsArticoli()
        End Try
        Session(SWOPDETTDOCR) = SWOPNESSUNA
    End Sub
    Private Function CKRespAreaVisiteByProv(ByVal myProvApp As String, ByVal SWAssegna As Boolean) As String
        CKRespAreaVisiteByProv = ""
        If myProvApp.Trim = "" Then
            Exit Function
        End If
        'giu101123 Area/Visita selezionati 
        'se nothing prendo i dati dalla testata
        Dim myCodAreaTest As String = "" : Dim myCodVisitaTest As String = ""
        If _WucElement.GetRespAreaVisite(myCodAreaTest, myCodVisitaTest) = False Then
            myCodAreaTest = "" : myCodVisitaTest = ""
        End If
        Dim myCodRespAreaSel As String = "" : Dim myCodRespVisiteSel As String = ""
        If (DDLRespAreaApp Is Nothing) Or (DDLRespVisiteApp Is Nothing) Then
            myCodRespAreaSel = "" : myCodRespVisiteSel = ""
        Else
            Try
                myCodRespAreaSel = DDLRespAreaApp.SelectedValue.Trim
            Catch ex As Exception
                myCodRespAreaSel = ""
            End Try
            '-
            Try
                myCodRespVisiteSel = DDLRespVisiteApp.SelectedValue.Trim
            Catch ex As Exception
                myCodRespVisiteSel = ""
            End Try
        End If
        '---------------------------------
        Dim myCodRespArea As String = "" : Dim myDesRespArea As String = ""
        Dim myCodRespVisite As String = "" : Dim myDesRespVisite As String = ""
        Dim myCodRegione As String = ""
        Dim strSQL As String = ""
        strSQL = "SELECT RespVisiteRegPr.Codice, RespVisiteRegPr.CodRespVisite, RespVisiteRegPr.CodRegione, RespVisiteRegPr.Provincia, " &
                 "RespVisite.CodRespArea, RespVisite.Descrizione AS DesRespVisite, RespArea.Descrizione AS DesRespArea " &
                 "FROM RespVisiteRegPr INNER JOIN RespVisite ON RespVisiteRegPr.CodRespVisite = RespVisite.Codice INNER JOIN " &
                 "RespArea ON RespVisite.CodRespArea = RespArea.Codice " &
                 "WHERE RespVisiteRegPr.Provincia = '" & myProvApp.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds1 As New DataSet
        Dim ds2 As New DataSet
        Dim ds3 As New DataSet
        Dim rows() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds1)
            If (ds1.Tables.Count > 0) Then
                If (ds1.Tables(0).Rows.Count > 0) Then
                    rows = ds1.Tables(0).Select()
                    myCodRespArea = IIf(IsDBNull(rows(0).Item("CodRespArea")), "", rows(0).Item("CodRespArea").ToString.Trim)
                    myCodRespVisite = IIf(IsDBNull(rows(0).Item("CodRespVisite")), "", rows(0).Item("CodRespVisite").ToString.Trim)
                    myDesRespArea = IIf(IsDBNull(rows(0).Item("DesRespArea")), "", rows(0).Item("DesRespArea").ToString.Trim)
                    myDesRespVisite = IIf(IsDBNull(rows(0).Item("DesRespVisite")), "", rows(0).Item("DesRespVisite").ToString.Trim)
                    If myCodRespAreaSel.Trim = "" And myCodRespVisiteSel.Trim = "" And SWAssegna Then
                        Session(IDRESPAREAAPP) = myCodRespArea
                        PosizionaItemDDL(myCodRespArea, DDLRespAreaApp)
                        '-
                        SqlDSRespVisiteApp.DataBind()
                        DDLRespVisiteApp.Items.Clear()
                        DDLRespVisiteApp.Items.Add("")
                        DDLRespVisiteApp.DataBind()
                        DDLRespVisiteApp.BackColor = SEGNALA_OK
                        '-- mi riposiziono 
                        DDLRespVisiteApp.AutoPostBack = False
                        DDLRespVisiteApp.SelectedIndex = -1
                        DDLRespVisiteApp.AutoPostBack = True
                        '-----
                        If myCodRespArea.Trim <> "" Then
                            Session(IDRESPVISITEAPP) = myCodRespVisite
                            PosizionaItemDDL(myCodRespVisite, DDLRespVisiteApp)
                        Else
                            Session(IDRESPVISITEAPP) = ""
                        End If
                    Else
                        If myCodRespAreaSel.Trim = "" Then myCodRespAreaSel = myCodAreaTest
                        If myCodRespVisiteSel.Trim = "" Then myCodRespVisiteSel = myCodVisitaTest
                        If myCodRespAreaSel.Trim <> myCodRespArea Or myCodRespVisiteSel.Trim <> myCodRespVisite Then
                            CKRespAreaVisiteByProv = "NOTA: Resp.Area/Visite diverso da quelli collegati alla Provincia: " + myDesRespArea + "/" + myDesRespVisite
                        End If
                    End If
                Else
                    strSQL = "SELECT * FROM Province WHERE Codice = '" & myProvApp.Trim & "'"
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds2)
                    If (ds2.Tables.Count > 0) Then
                        If (ds2.Tables(0).Rows.Count > 0) Then
                            rows = ds2.Tables(0).Select()
                            myCodRegione = IIf(IsDBNull(rows(0).Item("Regione")), "", rows(0).Item("Regione").ToString.Trim)
                            strSQL = "SELECT RespVisiteRegPr.Codice, RespVisiteRegPr.CodRespVisite, RespVisiteRegPr.CodRegione, RespVisiteRegPr.Provincia, " &
                                     "RespVisite.CodRespArea, RespVisite.Descrizione AS DesRespVisite, RespArea.Descrizione AS DesRespArea " &
                                     "FROM RespVisiteRegPr INNER JOIN RespVisite ON RespVisiteRegPr.CodRespVisite = RespVisite.Codice INNER JOIN " &
                                     "RespArea ON RespVisite.CodRespArea = RespArea.Codice " &
                                     "WHERE RespVisiteRegPr.CodRegione = " & myCodRegione.Trim & ""
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds3)
                            If (ds3.Tables.Count > 0) Then
                                If (ds3.Tables(0).Rows.Count > 0) Then
                                    rows = ds3.Tables(0).Select()
                                    myCodRespArea = IIf(IsDBNull(rows(0).Item("CodRespArea")), "", rows(0).Item("CodRespArea").ToString.Trim)
                                    myCodRespVisite = IIf(IsDBNull(rows(0).Item("CodRespVisite")), "", rows(0).Item("CodRespVisite").ToString.Trim)
                                    myDesRespArea = IIf(IsDBNull(rows(0).Item("DesRespArea")), "", rows(0).Item("DesRespArea").ToString.Trim)
                                    myDesRespVisite = IIf(IsDBNull(rows(0).Item("DesRespVisite")), "", rows(0).Item("DesRespVisite").ToString.Trim)
                                    If myCodRespAreaSel.Trim = "" And myCodRespVisiteSel.Trim = "" And SWAssegna Then
                                        Session(IDRESPAREAAPP) = myCodRespArea
                                        PosizionaItemDDL(myCodRespArea, DDLRespAreaApp)
                                        '-
                                        SqlDSRespVisiteApp.DataBind()
                                        DDLRespVisiteApp.Items.Clear()
                                        DDLRespVisiteApp.Items.Add("")
                                        DDLRespVisiteApp.DataBind()
                                        DDLRespVisiteApp.BackColor = SEGNALA_OK
                                        '-- mi riposiziono 
                                        DDLRespVisiteApp.AutoPostBack = False
                                        DDLRespVisiteApp.SelectedIndex = -1
                                        DDLRespVisiteApp.AutoPostBack = True
                                        '-----
                                        If myCodRespArea.Trim <> "" Then
                                            Session(IDRESPVISITEAPP) = myCodRespVisite
                                            PosizionaItemDDL(myCodRespVisite, DDLRespVisiteApp)
                                        Else
                                            Session(IDRESPVISITEAPP) = ""
                                        End If
                                    Else
                                        If myCodRespAreaSel.Trim = "" Then myCodRespAreaSel = myCodAreaTest
                                        If myCodRespVisiteSel.Trim = "" Then myCodRespVisiteSel = myCodVisitaTest
                                        If myCodRespAreaSel.Trim <> myCodRespArea Or myCodRespVisiteSel.Trim <> myCodRespVisite Then
                                            CKRespAreaVisiteByProv = "NOTA: Resp.Area/Visite diverso da quelli collegati alla Provincia: " + myDesRespArea + "/" + myDesRespVisite
                                        End If
                                    End If
                                Else
                                    CKRespAreaVisiteByProv = ""
                                    Exit Function
                                End If
                            Else
                                CKRespAreaVisiteByProv = ""
                                Exit Function
                            End If
                        Else
                            CKRespAreaVisiteByProv = ""
                            Exit Function
                        End If
                    Else
                        CKRespAreaVisiteByProv = ""
                        Exit Function
                    End If
                End If
            Else
                CKRespAreaVisiteByProv = ""
                Exit Function
            End If
        Catch Ex As Exception
            CKRespAreaVisiteByProv = ""
            Exit Function
        Finally
            ObjDB = Nothing
        End Try
    End Function

    Private Sub PopolaDestCliForD(ByRef myCGDest As String, ByRef myProvApp As String) 'giu260320
        lblDestSelDett.ToolTip = "" : lblDestSelDett.Text = "" : lblDestSelDett.BackColor = SEGNALA_OKLBL
        myProvApp = ""
        Dim myCG As String = Session(CSTCODCOGE)
        If Not IsNothing(myCG) Then
            If String.IsNullOrEmpty(myCG) Then
                myCG = ""
            End If
        Else
            myCG = ""
        End If
        If myCG.Trim = "" Then
            lblDestSelDett.Text = ""
            Exit Sub
        End If
        If Not IsNothing(myCGDest) Then
            If String.IsNullOrEmpty(myCGDest) Then
                myCGDest = ""
            End If
        Else
            myCGDest = ""
        End If
        If myCGDest.Trim = "" Then
            lblDestSelDett.Text = ""
        End If
        '-ok leggo
        Dim strSQL As String = ""
        strSQL = "Select * From DestClienti WHERE Codice = '" & myCG.ToString.Trim & "'"
        If IsNumeric(myCGDest) Then
            strSQL += " AND Progressivo=" & myCGDest.Trim
        Else
            myCGDest = ""
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myDestCF As DestCliForEntity
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        myDestCF = New DestCliForEntity
                        With myDestCF
                            .Progressivo = IIf(IsDBNull(row.Item("Progressivo")), "", row.Item("Progressivo"))
                            .Ragione_Sociale = IIf(IsDBNull(row.Item("Ragione_Sociale")), "", row.Item("Ragione_Sociale"))
                            .Indirizzo = IIf(IsDBNull(row.Item("Indirizzo")), "", row.Item("Indirizzo"))
                            .Cap = IIf(IsDBNull(row.Item("Cap")), "", row.Item("Cap"))
                            .Localita = IIf(IsDBNull(row.Item("Localita")), "", row.Item("Localita"))
                            .Provincia = IIf(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia"))
                            .Stato = IIf(IsDBNull(row.Item("Stato")), "", row.Item("Stato"))
                            .Denominazione = IIf(IsDBNull(row.Item("Denominazione")), "", row.Item("Denominazione"))
                            .Riferimento = IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
                            .Telefono1 = IIf(IsDBNull(row.Item("Telefono1")), "", row.Item("Telefono1"))
                            .Telefono2 = IIf(IsDBNull(row.Item("Telefono2")), "", row.Item("Telefono2"))
                            .Fax = IIf(IsDBNull(row.Item("Fax")), "", row.Item("Fax"))
                            .EMail = IIf(IsDBNull(row.Item("EMail")), "", row.Item("EMail"))
                            .Tipo = IIf(IsDBNull(row.Item("Tipo")), "", row.Item("Tipo"))
                        End With
                        Exit For
                    Next
                    If (ds.Tables(0).Rows.Count > 0) And myCGDest.Trim = "" Then
                        lblDestSelDett.Text = "[SONO PRESENTI DESTINAZIONI DIVERSE]"
                        Exit Sub
                    ElseIf myCGDest.Trim = "" Then
                        lblDestSelDett.Text = ""
                        Exit Sub
                    Else
                        lblDestSelDett.Text = myCGDest.Trim + " - " + myDestCF.Ragione_Sociale.Trim & " " & myDestCF.Indirizzo.Trim & " " & myDestCF.Localita.Trim & " " & IIf(myDestCF.Provincia.Trim <> "", "(" & myDestCF.Provincia.Trim & ")", "")
                        lblDestSelDett.ToolTip = myDestCF.Denominazione.Trim & " " & myDestCF.Cap.Trim & " " & myDestCF.Localita.Trim & " " & IIf(myDestCF.Provincia.Trim <> "", "(" & myDestCF.Provincia.Trim & ")", "")
                        myProvApp = myDestCF.Provincia.Trim
                    End If
                    Exit Sub
                ElseIf myCGDest.Trim <> "" Then
                    myCGDest = ""
                    lblDestSelDett.Text = "ERRORE Codice non presente in tabella Destinazioni"
                    Call _WucElement.SetLblMessDoc("ERRORE Codice non presente in tabella Destinazioni")
                    SetStatoDoc5()
                    lblDestSelDett.BackColor = SEGNALA_KO
                    Exit Sub
                Else
                    myCGDest = ""
                    lblDestSelDett.Text = ""
                    Exit Sub
                End If
            Else
                myCGDest = ""
                lblDestSelDett.Text = ""
                Exit Sub
            End If
        Catch Ex As Exception
            myCGDest = ""
            lblDestSelDett.Text = "Errore: Lettura DestClienti - " & Ex.Message
            Exit Sub
        End Try
    End Sub
    Public Sub SetDDLLuogoAppAtt(ByVal myOp As String)
        If myOp = "SVUOTA" Then
            lblDestSelDett.ToolTip = "" : lblDestSelDett.Text = ""
        ElseIf myOp = "OK" Then
            lblDestSelDett.ToolTip = "" : lblDestSelDett.Text = "[SONO PRESENTI LUOGHI DIVERSI]"
        Else
            lblDestSelDett.ToolTip = "" : lblDestSelDett.Text = ""
        End If
    End Sub

    Private Sub AzzeraTxtInsArticoli()
        'giu021211
        txtCodArtIns.AutoPostBack = False
        txtIVAIns.AutoPostBack = False
        txtCodArtIns.Text = "" : txtCodArtIns.BackColor = SEGNALA_OK : txtDesArtIns.Text = ""
        lblBase.Text = "" : lblOpz.Text = ""
        txtUMIns.Text = "" : txtQtaIns.Text = "" : lblQtaEv.Text = "" : LblQtaRe.Text = ""
        txtIVAIns.Text = "" : txtPrezzoIns.Text = "" : lblPrezzoAL.ToolTip = "" : txtSconto1Ins.Text = ""
        txtCodArtIns.AutoPostBack = True
        txtIVAIns.AutoPostBack = True
        '---
        LblPrezzoNetto.Text = HTML_SPAZIO : LblImportoRiga.Text = HTML_SPAZIO

        checkNoScontoValore.Checked = True 'giu160112

        lblMessAgg.BorderStyle = BorderStyle.None
        lblMessAgg.Text = ""
        'giu300312
        lblSuperatoScMax.BorderStyle = BorderStyle.None
        lblSuperatoScMax.Text = ""
        'giu130220
        chkSWSostituito.Checked = False
        chkSWCalcoloTot.Checked = False
        txtDataSc.Text = ""
        txtDataScCons.Text = ""
        txtDataEv.Text = ""
        chkEvasa.AutoPostBack = False
        chkEvasa.Checked = False
        chkEvasa.AutoPostBack = True
        chkFatturata.AutoPostBack = False
        chkFatturata.Checked = True
        chkFatturata.AutoPostBack = True
        txtSerie.Text = ""
        txtSerie.BackColor = SEGNALA_OK
        txtLotto.Text = ""
        txtLotto.BackColor = SEGNALA_OK
        'giu050520 no il modello xke posso averlo selezionato prima
        ' ''DDLModello.BackColor = SEGNALA_OK
        ' ''DDLModello.SelectedIndex = 0
        txtNote.Text = ""
        lblDestSelDett.ToolTip = "" : lblDestSelDett.Text = ""
        DDLRespAreaApp.SelectedIndex = -1
        DDLRespVisiteApp.SelectedIndex = -1
        '---------
    End Sub
    Private Sub EnableTOTxtInsArticoli(ByVal SW As Boolean)
        Dim pCodVisita As String = "" : Dim pNVisite As Integer = 0
        If GetCodVisitaDATipoCAIdPag(pCodVisita, pNVisite) = False Then
            lblMessAgg.ForeColor = Color.DarkRed
            lblMessAgg.BorderStyle = BorderStyle.Outset
            lblMessAgg.Text = "Definizione Tipo Contratto errato."
        End If
        If GridViewDett.Enabled = True Then
            btnDelDettagli.Enabled = Not SW

            If SW Then
                If DDLTipoDettagli.SelectedIndex <> 0 Then
                    DDLModello.Enabled = False
                    btnDelDestD.Enabled = False
                    btnCercaDestD.Enabled = False
                    DDLRespAreaApp.Enabled = False
                    DDLRespVisiteApp.Enabled = False
                    'giu071223
                    txtDataEv.Enabled = True : ImgDataEv.Enabled = True
                    chkEvasa.Enabled = True
                    chkFatturata.Enabled = True
                Else
                    DDLModello.Enabled = True 'giu100222 Not SW
                    btnDelDestD.Enabled = True
                    btnCercaDestD.Enabled = True
                    DDLRespAreaApp.Enabled = True
                    DDLRespVisiteApp.Enabled = True
                    'giu071223
                    txtDataEv.Enabled = False : ImgDataEv.Enabled = False
                    chkEvasa.Enabled = False
                    chkFatturata.Enabled = False
                End If
            End If
        Else
            DDLModello.Enabled = False
            btnDelDestD.Enabled = False
            btnCercaDestD.Enabled = False
            DDLRespAreaApp.Enabled = False
            DDLRespVisiteApp.Enabled = False
        End If
        DDLModello.Enabled = SW 'giu131223
        DettArtInsPanel.Enabled = SW
        'giu021211
        txtCodArtIns.Enabled = SW : txtDesArtIns.Enabled = SW
        txtUMIns.Enabled = SW : txtQtaIns.Enabled = SW
        txtIVAIns.Enabled = SW : txtPrezzoIns.Enabled = SW : txtSconto1Ins.Enabled = SW
        txtIVAIns.BackColor = Def.SEGNALA_OK
        '---
        btnPrimaRiga.Enabled = SW : BtnSelArticolo.Enabled = SW
        ' ''btnAggiornaDett.Enabled = Not SW : btnAggiornaDett.Visible = Not SW
        ' ''brnAnnullaDett.Enabled = Not SW : brnAnnullaDett.Visible = Not SW
        BtnSelArticoloIns.Enabled = SW
        btnAggArtGridSel.Enabled = SW 'giu061211 uso sempre aggriga :: btnInsRigaDett.Enabled = SW
        checkNoScontoValore.Enabled = SW
        btnModificaNoteInterv.Enabled = Not SW
        'QUI NO E' SOLO LA MODIFICA btnAnnullaModNoteInterv.Enabled = Not SW
        'giu070516
        If Session(SWOP) = SWOPMODIFICA Then
            btnModificaNoteInterv.Enabled = False
        End If
        If SW = False Then txtPrezzoCosto.Enabled = False
        If txtCodArtIns.Text.Trim = pCodVisita.Trim And pCodVisita.Trim <> "" Then
            txtDataScCons.Enabled = False : ImgDataScCons.Enabled = False
        Else
            txtDataScCons.Enabled = True : ImgDataScCons.Enabled = True
        End If
    End Sub
    Private Sub SetBtnPrimaRigaEnabled(ByVal SW As Boolean)
        btnPrimaRiga.Enabled = SW : btnPrimaRiga.Visible = SW
        BtnSelArticolo.Visible = Not SW : BtnSelArticolo.Enabled = Not SW
        If btnPrimaRiga.Visible = True Then
            lblRigaSel.Text = HTML_SPAZIO
        End If
    End Sub

#Region "Imposta Command e DataAdapter"
    Private Function SetCdmDAdp() As Boolean

        SqlConnDocDett = New SqlConnection
        SqlAdapDocDett = New SqlDataAdapter
        SqlAdapDocForInsertD = New SqlDataAdapter 'GIU190220
        SqlDbSelectCmd = New SqlCommand
        SqlDbInserCmd = New SqlCommand
        SqlDbUpdateCmd = New SqlCommand
        SqlDbDeleteCmd = New SqlCommand
        'giu250220
        SqlAdapDocALLAtt = New SqlDataAdapter
        SqlDbSelectCmdALLAtt = New SqlCommand
        '---------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDett.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
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
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        SqlDbSelectCmd.CommandText = "get_ConDByIDDocumenti" 'ok select *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDocDett
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1 GIU130412
        '
        SqlDbInserCmd.CommandText = "insert_ConDByIDDocumenti"
        SqlDbInserCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmd.Connection = Me.SqlConnDocDett
        SqlDbInserCmd.CommandTimeout = myTimeOUT
        SqlDbInserCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), _
            New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), _
            New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), _
            New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), _
            New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), _
            New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), _
            New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), _
            New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
            New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
            New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti"), _
            New System.Data.SqlClient.SqlParameter("@PrezzoCosto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoCosto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Inviata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Inviata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@DedPerAcconto", System.Data.SqlDbType.Bit, 0, "DedPerAcconto"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 0, "Cod_Filiale"), _
            New System.Data.SqlClient.SqlParameter("@DataEv", System.Data.SqlDbType.DateTime, 0, "DataEv"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@Serie", System.Data.SqlDbType.NVarChar, 0, "Serie"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNum", System.Data.SqlDbType.Int, 0, "QtaDurataNum"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNumR0", System.Data.SqlDbType.Int, 0, "QtaDurataNumR0"), _
            New System.Data.SqlClient.SqlParameter("@DataSc", System.Data.SqlDbType.DateTime, 0, "DataSc"), _
            New System.Data.SqlClient.SqlParameter("@SWInvioMod", System.Data.SqlDbType.Bit, 0, "SWInvioMod"), _
            New System.Data.SqlClient.SqlParameter("@SWCalcoloTot", System.Data.SqlDbType.Bit, 0, "SWCalcoloTot"), _
            New System.Data.SqlClient.SqlParameter("@SWSostituito", System.Data.SqlDbType.Bit, 0, "SWSostituito"), _
            New System.Data.SqlClient.SqlParameter("@SWChiusaNoEv", System.Data.SqlDbType.Bit, 0, "SWChiusaNoEv"), _
            New System.Data.SqlClient.SqlParameter("@RefNumFC", System.Data.SqlDbType.Int, 0, "RefNumFC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataFC", System.Data.SqlDbType.DateTime, 0, "RefDataFC"), _
            New System.Data.SqlClient.SqlParameter("@RespArea", System.Data.SqlDbType.Int, 0, "RespArea"), _
            New System.Data.SqlClient.SqlParameter("@RespVisite", System.Data.SqlDbType.Int, 0, "RespVisite")})
        '
        'SqlUpdateCommand1 GIU130412
        '
        SqlDbUpdateCmd.CommandText = "update_ConDByIDDocumenti"
        SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdateCmd.Connection = Me.SqlConnDocDett
        SqlDbUpdateCmd.CommandTimeout = myTimeOUT
        SqlDbUpdateCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), _
            New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), _
            New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), _
            New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), _
            New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), _
            New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), _
            New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), _
            New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), _
            New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), _
            New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), _
            New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), _
            New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), _
            New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), _
            New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
            New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
            New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
            New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti"), _
            New System.Data.SqlClient.SqlParameter("@PrezzoCosto", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "PrezzoCosto", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Inviata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Inviata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@DedPerAcconto", System.Data.SqlDbType.Bit, 0, "DedPerAcconto"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 0, "Cod_Filiale"), _
            New System.Data.SqlClient.SqlParameter("@DataEv", System.Data.SqlDbType.DateTime, 0, "DataEv"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@Serie", System.Data.SqlDbType.NVarChar, 0, "Serie"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNum", System.Data.SqlDbType.Int, 0, "QtaDurataNum"), _
            New System.Data.SqlClient.SqlParameter("@QtaDurataNumR0", System.Data.SqlDbType.Int, 0, "QtaDurataNumR0"), _
            New System.Data.SqlClient.SqlParameter("@DataSc", System.Data.SqlDbType.DateTime, 0, "DataSc"), _
            New System.Data.SqlClient.SqlParameter("@SWInvioMod", System.Data.SqlDbType.Bit, 0, "SWInvioMod"), _
            New System.Data.SqlClient.SqlParameter("@SWCalcoloTot", System.Data.SqlDbType.Bit, 0, "SWCalcoloTot"), _
            New System.Data.SqlClient.SqlParameter("@SWSostituito", System.Data.SqlDbType.Bit, 0, "SWSostituito"), _
            New System.Data.SqlClient.SqlParameter("@SWChiusaNoEv", System.Data.SqlDbType.Bit, 0, "SWChiusaNoEv"), _
            New System.Data.SqlClient.SqlParameter("@RefNumFC", System.Data.SqlDbType.Int, 0, "RefNumFC"), _
            New System.Data.SqlClient.SqlParameter("@RefDataFC", System.Data.SqlDbType.DateTime, 0, "RefDataFC"), _
            New System.Data.SqlClient.SqlParameter("@RespArea", System.Data.SqlDbType.Int, 0, "RespArea"), _
            New System.Data.SqlClient.SqlParameter("@RespVisite", System.Data.SqlDbType.Int, 0, "RespVisite"), _
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNum", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNumRiga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNumRiga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing)})
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmd.CommandText = "delete_ConDByIDDocumenti"
        SqlDbDeleteCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmd.Connection = Me.SqlConnDocDett
        SqlDbDeleteCmd.CommandTimeout = myTimeOUT
        SqlDbDeleteCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", _
        System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", _
        System.Data.DataRowVersion.Current, Nothing), _
        New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
        New System.Data.SqlClient.SqlParameter("@Original_DurataNum", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Original, Nothing), _
        New System.Data.SqlClient.SqlParameter("@Original_DurataNumRiga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNumRiga", System.Data.DataRowVersion.Original, Nothing), _
        New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing)})
        'giu250220
        SqlDbSelectCmdALLAtt.CommandText = "get_ConDByIDDocumenti" 'ok select *
        SqlDbSelectCmdALLAtt.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmdALLAtt.Connection = Me.SqlConnDocDett
        SqlDbSelectCmdALLAtt.CommandTimeout = myTimeOUT
        SqlDbSelectCmdALLAtt.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdALLAtt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlAdapDocALLAtt.SelectCommand = SqlDbSelectCmdALLAtt
        '
        SqlAdapDocDett.SelectCommand = SqlDbSelectCmd
        SqlAdapDocDett.InsertCommand = SqlDbInserCmd
        SqlAdapDocDett.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDocDett.UpdateCommand = SqlDbUpdateCmd
        Session("aSqlAdap") = SqlAdapDocDett
        'GIU190220
        SqlAdapDocForInsertD.InsertCommand = SqlDbInserCmd
        Session("SqlAdapDocForInsertD") = SqlAdapDocForInsertD
        '---------
        '--- LOTTI @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        SqlConnDocDettL = New SqlConnection
        SqlAdapDocDettL = New SqlDataAdapter
        SqlDbSelectCmdL = New SqlCommand
        SqlDbInserCmdL = New SqlCommand
        SqlDbUpdateCmdL = New SqlCommand
        SqlDbDeleteCmdL = New SqlCommand

        '' ''Dim dbCon As New dbStringaConnesioneFacade
        SqlConnDocDettL.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)

        SqlDbSelectCmdL.CommandText = "get_ConDLByIDDocRiga"
        SqlDbSelectCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmdL.Connection = Me.SqlConnDocDettL
        SqlDbSelectCmdL.CommandTimeout = myTimeOUT
        SqlDbSelectCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1
        '
        SqlDbInserCmdL.CommandText = "insert_ConDLByIDDocRiga"
        SqlDbInserCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdL.Connection = Me.SqlConnDocDettL
        SqlDbInserCmdL.CommandTimeout = myTimeOUT
        SqlDbInserCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie")})
        '
        'SqlUpdateCommand1
        '
        SqlDbUpdateCmdL.CommandText = "update_ConDLByIDDocRiga"
        SqlDbUpdateCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdateCmdL.Connection = Me.SqlConnDocDettL
        SqlDbUpdateCmdL.CommandTimeout = myTimeOUT
        SqlDbUpdateCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"), _
            New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie"), _
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNum", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNumRiga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNumRiga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_NCollo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NCollo", System.Data.DataRowVersion.Original, Nothing)})
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmdL.CommandText = "delete_ConDLByIDDocRiga"
        SqlDbDeleteCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmdL.Connection = Me.SqlConnDocDettL
        SqlDbDeleteCmdL.CommandTimeout = myTimeOUT
        SqlDbDeleteCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNum", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNum", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_DurataNumRiga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DurataNumRiga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_NCollo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NCollo", System.Data.DataRowVersion.Original, Nothing)})

        SqlAdapDocDettL.SelectCommand = SqlDbSelectCmdL
        SqlAdapDocDettL.InsertCommand = SqlDbInserCmdL
        SqlAdapDocDettL.DeleteCommand = SqlDbDeleteCmdL
        SqlAdapDocDettL.UpdateCommand = SqlDbUpdateCmdL
        Session("aSqlAdapL") = SqlAdapDocDettL
    End Function
#End Region

#Region "Imposta GRID dettagli"
    Private Sub ImpostaGriglia()
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'Tipo Documento per sapere quale Quanita' devo prendere vedi function CalcolaTotaleDocCA
        If myTipoDoc = "" Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        ImpostaGrid()
    End Sub

    Private Sub ImpostaGrid()
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Dim ColoreSfondoIntes As String = "#CC6600"
        GridViewDett.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDett.Attributes.Add("style", "table-layout:fixed")

        Dim nameColumn0 As New BoundField
        nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.HeaderText = "Riga"
        nameColumn0.DataField = "Riga"
        'nameColumn0.DataFormatString = "{0:###}"
        nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.ItemStyle.Wrap = False
        nameColumn0.HeaderStyle.Width = Unit.Pixel(20)
        nameColumn0.ItemStyle.Width = Unit.Pixel(20)
        nameColumn0.ReadOnly = True
        GridViewDett.Columns.Add(nameColumn0)

        Dim nameColumn1 As New BoundField
        nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.HeaderStyle.Wrap = False
        nameColumn1.HeaderText = "Codice articolo"
        nameColumn1.DataField = "Cod_Articolo"
        nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.ItemStyle.Wrap = True
        nameColumn1.HeaderStyle.Width = Unit.Pixel(80)
        nameColumn1.ItemStyle.Width = Unit.Pixel(80)
        nameColumn1.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn1)

        Dim nameColumn2 As New BoundField
        nameColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn2.HeaderText = "Descrizione"
        nameColumn2.DataField = "Descrizione"
        nameColumn2.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.ItemStyle.Wrap = True   'per andare a capo
        nameColumn2.HeaderStyle.Width = Unit.Pixel(150)
        nameColumn2.ItemStyle.Width = Unit.Pixel(150)
        nameColumn2.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn2)

        Dim nameColumnSL As New BoundField
        nameColumnSL.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumnSL.HeaderStyle.Wrap = False
        nameColumnSL.HeaderText = "Serie"
        nameColumnSL.DataField = "Serie"
        nameColumnSL.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumnSL.ItemStyle.Wrap = False
        nameColumnSL.HeaderStyle.Width = Unit.Pixel(80)
        nameColumnSL.ItemStyle.Width = Unit.Pixel(80)
        nameColumnSL.ReadOnly = True
        GridViewDett.Columns.Add(nameColumnSL)

        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn3.HeaderText = "UM"
        nameColumn3.DataField = "Um"
        nameColumn3.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn3.ItemStyle.Wrap = False
        nameColumn3.HeaderStyle.Width = Unit.Pixel(10)
        nameColumn3.ItemStyle.Width = Unit.Pixel(10)
        nameColumn3.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn3)

        Dim nameColumn4 As New BoundField
        nameColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn4.HeaderText = "Quantità ordinata"
        nameColumn4.DataField = "Qta_Ordinata"
        'nameColumn4.DataFormatString = "{0:###}"
        nameColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn4.ItemStyle.Wrap = False
        nameColumn4.HeaderStyle.Width = Unit.Pixel(40)
        nameColumn4.ItemStyle.Width = Unit.Pixel(40)
        nameColumn4.Visible = True
        nameColumn4.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn4)

        Dim nameColumn5 As New BoundField
        nameColumn5.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn5.HeaderStyle.Wrap = True
        'giu301211 nameColumn5.HeaderText = "Quantità evasa"
        nameColumn5.DataField = "Qta_Evasa"
        nameColumn5.HeaderText = "Quantità evasa"
        'nameColumn5.DataFormatString = "{0:###}"
        nameColumn5.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn5.ItemStyle.Wrap = False
        nameColumn5.HeaderStyle.Width = Unit.Pixel(40)
        nameColumn5.ItemStyle.Width = Unit.Pixel(40)
        nameColumn5.Visible = True
        nameColumn5.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn5)

        Dim nameColumnQFa As New BoundField
        nameColumnQFa.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumnQFa.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnQFa.HeaderStyle.Wrap = True
        nameColumnQFa.DataField = "Qta_Fatturata"
        nameColumnQFa.HeaderText = "Quantità fatturata"
        'nameColumnQFa.DataFormatString = "{0:###}"
        nameColumnQFa.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumnQFa.ItemStyle.Wrap = False
        'giu010112 nameColumnQFa.ReadOnly = True
        nameColumnQFa.HeaderStyle.Width = Unit.Pixel(40)
        nameColumnQFa.ItemStyle.Width = Unit.Pixel(40)
        nameColumnQFa.Visible = True
        nameColumnQFa.ReadOnly = True
        GridViewDett.Columns.Add(nameColumnQFA)

        Dim nameColumnQAL As New BoundField
        nameColumnQAL.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumnQAL.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnQAL.HeaderStyle.Wrap = True
        nameColumnQAL.DataField = "Qta_Allestita"
        nameColumnQAL.HeaderText = "Quantità allestita"
        'nameColumnQAL.DataFormatString = "{0:###}"
        nameColumnQAL.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumnQAL.ItemStyle.Wrap = False
        'giu010112 nameColumnQAL.ReadOnly = True
        nameColumnQAL.HeaderStyle.Width = Unit.Pixel(40)
        nameColumnQAL.ItemStyle.Width = Unit.Pixel(40)
        'GIU271211
        Select Case myTipoDoc
            Case "OC", "DT"
                nameColumnQAL.Visible = True
            Case Else
                nameColumnQAL.Visible = False
        End Select
        nameColumnQAL.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumnQAL)

        Dim nameColumn6 As New BoundField
        nameColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn6.HeaderStyle.Wrap = True
        nameColumn6.HeaderText = "Quantità residua"
        nameColumn6.DataField = "Qta_Residua"
        'nameColumn6.DataFormatString = "{0:###}"
        nameColumn6.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn6.ItemStyle.Wrap = False
        nameColumn6.HeaderStyle.Width = Unit.Pixel(40)
        nameColumn6.ItemStyle.Width = Unit.Pixel(40)
        nameColumn6.Visible = False
        nameColumn6.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn6)

        Dim nameColumnCF As New BoundField
        nameColumnCF.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumnCF.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnCF.HeaderStyle.Wrap = True
        nameColumnCF.DataField = "Cod_Filiale"
        nameColumnCF.HeaderText = "Luogo App."
        'nameColumnCF.DataFormatString = "{0:###}"
        nameColumnCF.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumnCF.ItemStyle.Wrap = False
        'giu010112 nameColumnQFA.ReadOnly = True
        nameColumnCF.HeaderStyle.Width = Unit.Pixel(40)
        nameColumnCF.ItemStyle.Width = Unit.Pixel(40)
        nameColumnCF.Visible = True
        nameColumnCF.ReadOnly = True
        GridViewDett.Columns.Add(nameColumnCF)

        Dim nameColumnDSc As New BoundField
        nameColumnDSc.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumnDSc.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnDSc.HeaderStyle.Wrap = True
        nameColumnDSc.DataField = "DataSc"
        nameColumnDSc.HeaderText = "Data scadenza"
        'nameColumnDSc.DataFormatString = "{0:###}"
        nameColumnDSc.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumnDSc.ItemStyle.Wrap = False
        nameColumnDSc.HeaderStyle.Width = Unit.Pixel(50)
        nameColumnDSc.ItemStyle.Width = Unit.Pixel(50)
        nameColumnDSc.Visible = True
        nameColumnDSc.ReadOnly = True
        GridViewDett.Columns.Add(nameColumnDSc)

        Dim nameColumnDEv As New BoundField
        nameColumnDEv.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumnDEv.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnDEv.HeaderText = "Data evasione"
        nameColumnDEv.DataField = "DataEv"
        'nameColumn16.DataFormatString = "{0:###}"
        nameColumnDEv.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumnDEv.ItemStyle.Wrap = False
        nameColumnDEv.Visible = True
        nameColumnDEv.HeaderStyle.Width = Unit.Pixel(50)
        nameColumnDEv.ItemStyle.Width = Unit.Pixel(50)
        nameColumnDEv.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumnDEv)

        Dim nameColumn7 As New BoundField
        nameColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn7.HeaderText = "IVA"
        nameColumn7.DataField = "Cod_Iva"
        'nameColumn7.DataFormatString = "{0:###}"
        nameColumn7.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn7.ItemStyle.Wrap = False
        nameColumn7.HeaderStyle.Width = Unit.Pixel(15)
        nameColumn7.ItemStyle.Width = Unit.Pixel(15)
        nameColumn7.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn7)

        Dim nameColumn8 As New BoundField
        nameColumn8.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn8.HeaderText = "Prezzo"
        nameColumn8.DataField = "Prezzo"
        nameColumn8.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn8.ItemStyle.Wrap = False
        nameColumn8.HeaderStyle.Width = Unit.Pixel(60)
        nameColumn8.ItemStyle.Width = Unit.Pixel(60)
        nameColumn8.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn8)

        Dim nameColDDL As New BoundField
        nameColDDL.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColDDL.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColDDL.HeaderStyle.Wrap = True
        nameColDDL.HeaderText = "SM OM"
        nameColDDL.DataField = "TipoScontoMerce"
        nameColDDL.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColDDL.ItemStyle.Wrap = False
        nameColDDL.HeaderStyle.Width = Unit.Pixel(15)
        nameColDDL.ItemStyle.Width = Unit.Pixel(15)
        nameColDDL.Visible = False
        nameColDDL.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColDDL)

        Dim nameColumn11 As New BoundField
        nameColumn11.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn11.HeaderStyle.Wrap = True
        nameColumn11.HeaderText = "Sconto valore"
        nameColumn11.DataField = "ScontoValore"
        nameColumn11.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn11.ItemStyle.Wrap = True
        nameColumn11.ReadOnly = True
        nameColumn11.HeaderStyle.Width = Unit.Pixel(30)
        nameColumn11.ItemStyle.Width = Unit.Pixel(30)
        nameColumn11.Visible = True 'giu110112
        GridViewDett.Columns.Add(nameColumn11)

        Dim nameColumn9 As New BoundField
        nameColumn9.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn9.HeaderStyle.Wrap = True
        nameColumn9.HeaderText = "Sc.(1)"
        nameColumn9.DataField = "Sconto_1"
        nameColumn9.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn9.ItemStyle.Wrap = False
        nameColumn9.HeaderStyle.Width = Unit.Pixel(30)
        nameColumn9.ItemStyle.Width = Unit.Pixel(30)
        nameColumn9.Visible = True
        ' ''If GetParamGestAzi(Session(ESERCIZIO)).NumSconti > 0 Then
        ' ''    nameColumn9.Visible = True
        ' ''Else
        ' ''    nameColumn9.Visible = False
        ' ''End If
        nameColumn9.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn9)

        Dim nameColumn10 As New BoundField
        nameColumn10.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn10.HeaderStyle.Wrap = True
        nameColumn10.HeaderText = "Sc.(2)"
        nameColumn10.DataField = "Sconto_2"
        nameColumn10.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn10.ItemStyle.Wrap = False
        nameColumn10.HeaderStyle.Width = Unit.Pixel(30)
        nameColumn10.ItemStyle.Width = Unit.Pixel(30)
        ' ''If GetParamGestAzi(Session(ESERCIZIO)).NumSconti > 1 Then
        ' ''    nameColumn10.Visible = True
        ' ''Else
        ' ''    nameColumn10.Visible = False
        ' ''End If
        'giu110112 adesso non è gestito 
        nameColumn10.Visible = False
        nameColumn10.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn10)

        Dim nameColumn12 As New BoundField
        nameColumn12.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn12.HeaderText = "Note"
        nameColumn12.DataField = "Note"
        nameColumn12.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn12.ItemStyle.Wrap = True
        nameColumn12.HeaderStyle.Width = Unit.Pixel(100)
        nameColumn12.ItemStyle.Width = Unit.Pixel(100)
        nameColumn12.Visible = False
        nameColumn12.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn12)

        Dim nameColumn13 As New BoundField
        nameColumn13.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn13.HeaderText = "Importo"
        nameColumn13.DataField = "Importo"
        nameColumn13.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn13.ItemStyle.Wrap = False
        nameColumn13.ReadOnly = True
        nameColumn13.HeaderStyle.Width = Unit.Pixel(60)
        nameColumn13.ItemStyle.Width = Unit.Pixel(60)
        GridViewDett.Columns.Add(nameColumn13)

        Dim nameColDed As New BoundField
        nameColDed.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColDed.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColDed.HeaderStyle.Wrap = True
        nameColDed.HeaderText = "Ded."
        nameColDed.DataField = "DedPerAcconto"
        nameColDed.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColDed.ItemStyle.Wrap = False
        nameColDed.HeaderStyle.Width = Unit.Pixel(15)
        nameColDed.ItemStyle.Width = Unit.Pixel(15)
        nameColDed.Visible = False
        nameColDed.ReadOnly = True
        GridViewDett.Columns.Add(nameColDed)

        Dim nameColumn14 As New BoundField
        nameColumn14.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn14.HeaderStyle.Wrap = True
        nameColumn14.HeaderText = "Sc. Riga"
        nameColumn14.DataField = "ScontoReale"
        nameColumn14.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn14.ItemStyle.Wrap = False
        nameColumn14.ReadOnly = True
        nameColumn14.HeaderStyle.Width = Unit.Pixel(25)
        nameColumn14.ItemStyle.Width = Unit.Pixel(25)
        nameColumn14.Visible = True
        GridViewDett.Columns.Add(nameColumn14)

        Dim nameColumn15 As New BoundField
        nameColumn15.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn15.HeaderStyle.Wrap = True
        nameColumn15.HeaderText = "C/ Ag."
        nameColumn15.DataField = "Cod_Agente"
        nameColumn15.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn15.ItemStyle.Wrap = False
        nameColumn15.ReadOnly = True 'giu110112
        nameColumn15.HeaderStyle.Width = Unit.Pixel(20)
        nameColumn15.ItemStyle.Width = Unit.Pixel(20)
        nameColumn15.Visible = False
        GridViewDett.Columns.Add(nameColumn15)
        'giu300312
        Dim nameColPAge As New BoundField
        nameColPAge.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColPAge.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColPAge.HeaderStyle.Wrap = True
        nameColPAge.HeaderText = "Provv."
        nameColPAge.DataField = "Pro_Agente"
        nameColPAge.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColPAge.ItemStyle.Wrap = False
        nameColPAge.ReadOnly = True 'giu110112
        nameColPAge.HeaderStyle.Width = Unit.Pixel(30)
        nameColPAge.ItemStyle.Width = Unit.Pixel(30)
        nameColPAge.Visible = False
        GridViewDett.Columns.Add(nameColPAge)
        '-
        Dim nameColImpProvvAge As New BoundField
        nameColImpProvvAge.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'nameColImpProvvAge.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColImpProvvAge.HeaderStyle.Wrap = True
        nameColImpProvvAge.HeaderText = "Imp. Provv."
        nameColImpProvvAge.DataField = "ImportoProvvigione"
        nameColImpProvvAge.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColImpProvvAge.ItemStyle.Wrap = False
        nameColImpProvvAge.ReadOnly = True 'giu110112
        nameColImpProvvAge.HeaderStyle.Width = Unit.Pixel(50)
        nameColImpProvvAge.ItemStyle.Width = Unit.Pixel(50)
        nameColImpProvvAge.Visible = False
        GridViewDett.Columns.Add(nameColImpProvvAge)

    End Sub

#End Region

#Region "Gestione GRID Dettagli documento (TD1)"
    Private Sub GridViewDett_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewDett.PageIndexChanging
        GridViewDett.EditIndex = -1
        If (e.NewPageIndex = -1) Then
            GridViewDett.PageIndex = 0
        Else
            GridViewDett.PageIndex = e.NewPageIndex
        End If
        GridViewDett.SelectedIndex = -1
        aDataView1 = Session("aDataView1")
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu311011
        GridViewDett.DataSource = aDataView1
        GridViewDett.DataBind()
        Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        '--- LOTTI
        Dim RigaSel As Integer = 0
        Try
            GridViewDett.SelectedIndex = 0
            RigaSel = GridViewDett.SelectedIndex 'giu180419
            If RigaSel = 0 Then 'giu180419
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then 'giu180419
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
    End Sub

    Private Sub GridViewDett_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles GridViewDett.RowCancelingEdit
        EnableTOTxtInsArticoli(False)
        GridViewDett.EditIndex = -1
        aDataView1 = Session("aDataView1")
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu311011
        GridViewDett.DataSource = aDataView1
        GridViewDett.DataBind()
        'Selezionato
        ' ''txtCodArticolo.Text = ""
        ' ''TxtDesArticolo.Text = ""
        '-----------
        Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        '--- LOTTI
        Dim RigaSel As Integer = 0
        Try
            GridViewDett.SelectedIndex = 0
            RigaSel = GridViewDett.SelectedIndex 'giu180419
            If RigaSel = 0 Then 'giu180419
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then 'giu180419
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
    End Sub

    Private Sub GridViewDett_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDett.RowDataBound
        'NON FUNZIONA QUANDO CI SONO I TASTI mODIFICA/AGGIORNA/ELIMINA NEL GRID 
        'attenzione non ABILITARE e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewDett, "Select$" + e.Row.RowIndex.ToString()))
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        Try
            If e.Row.DataItemIndex > -1 Then
                If IsNumeric(e.Row.Cells(CellIdx.Importo).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Importo).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Importo).Text), CInt(DecimaliVal)).ToString
                    Else
                        e.Row.Cells(CellIdx.Importo).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.IVA).Text) Then
                    If CDec(e.Row.Cells(CellIdx.IVA).Text) <> 0 Then
                        e.Row.Cells(CellIdx.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.IVA).Text), 0).ToString
                    Else
                        e.Row.Cells(CellIdx.IVA).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.Prz).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Prz).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                    Else
                        e.Row.Cells(CellIdx.Prz).Text = ""
                    End If
                End If
                'TIPO SCONTO MERCE : da FARE DOPO 
                'IIf([OmaggioImponibile],IIf([OmaggioImposta],'SM','OM'),'NO') AS TipoScontoMerce 
                ' ''If CBool(e.Row.Cells(CellIdx.ScM).Text) = False Then
                ' ''    e.Row.Cells(CellIdx.ScM).Text = "NO"
                ' ''Else
                ' ''    e.Row.Cells(CellIdx.ScM).Text = ""
                ' ''End If
                '--------------------------------------------------------------------------------
                If IsNumeric(e.Row.Cells(CellIdx.Qta).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Qta).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Qta).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.Qta).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.QtaEv).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaEv).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaEv).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaEv).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.QtaIn).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaIn).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaIn).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaIn).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaIn).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.QtaRe).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaRe).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaRe).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaRe).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.QtaFa).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaFa).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaFa).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaFa).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaFa).Text = ""
                    End If
                End If
                If IsDate(e.Row.Cells(CellIdx.DataSc).Text) Then
                    e.Row.Cells(CellIdx.DataSc).Text = Format(CDate(e.Row.Cells(CellIdx.DataSc).Text), FormatoData).ToString
                End If
                If IsDate(e.Row.Cells(CellIdx.DataEv).Text) Then
                    e.Row.Cells(CellIdx.DataEv).Text = Format(CDate(e.Row.Cells(CellIdx.DataEv).Text), FormatoData).ToString
                End If
                '- Sconto valore sul prezzo di Listino 
                If IsNumeric(e.Row.Cells(CellIdx.ScVal).Text) Then
                    If CDec(e.Row.Cells(CellIdx.ScVal).Text) <> 0 Then
                        e.Row.Cells(CellIdx.ScVal).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.ScVal).Text), CInt(DecimaliVal)).ToString
                    Else
                        e.Row.Cells(CellIdx.ScVal).Text = ""
                    End If
                End If
                '- Sconti
                If IsNumeric(e.Row.Cells(CellIdx.Sc1).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Sc1).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                    Else
                        e.Row.Cells(CellIdx.Sc1).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.Sc2).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Sc2).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Sc2).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Sc2).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                    Else
                        e.Row.Cells(CellIdx.Sc2).Text = ""
                    End If
                End If
                If e.Row.Cells(CellIdx.Deduzione).Text.Trim <> "True" Then
                    e.Row.Cells(CellIdx.Deduzione).Text = ""
                Else
                    e.Row.Cells(CellIdx.Deduzione).Text = "SI"
                End If
                If IsNumeric(e.Row.Cells(CellIdx.ScR).Text) Then
                    If CDec(e.Row.Cells(CellIdx.ScR).Text) <> 0 Then
                        e.Row.Cells(CellIdx.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.ScR).Text), 2).ToString
                    Else
                        e.Row.Cells(CellIdx.ScR).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.PAge).Text) Then
                    If CDec(e.Row.Cells(CellIdx.PAge).Text) <> 0 Then
                        e.Row.Cells(CellIdx.PAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.PAge).Text), 2).ToString
                    Else
                        e.Row.Cells(CellIdx.PAge).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.ImpProvvAge).Text) Then
                    If CDec(e.Row.Cells(CellIdx.ImpProvvAge).Text) <> 0 Then
                        e.Row.Cells(CellIdx.ImpProvvAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.ImpProvvAge).Text), 2).ToString
                    Else
                        e.Row.Cells(CellIdx.ImpProvvAge).Text = ""
                    End If
                End If
            End If
        Catch ex As Exception
            Dim errore As Integer = 0
        End Try
    End Sub

    Private Sub GridViewDett_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles GridViewDett.RowDeleting
        'giu250112
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            If myTipoDoc = SWTD(TD.OrdClienti) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Else
            Dim SWStatoDoc As String = Session(CSTSTATODOC)
            If IsNothing(SWStatoDoc) Then
                SWStatoDoc = ""
            End If
            If String.IsNullOrEmpty(SWStatoDoc) Then
                SWStatoDoc = ""
            End If
            If SWStatoDoc = "" Or Not IsNumeric(SWStatoDoc) Then
                SWStatoDoc = ""
            End If
            If myTipoDoc = SWTD(TD.OrdClienti) And SWStatoDoc = "5" Then 'ALLESTIMENTO
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Funzione non abilitata nella fase di Ordine in Allestimento.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        '----------------------------
        Dim Passo As Integer = 0
        GridViewDett.Enabled = False
        'Selezionato
        EnableTOTxtInsArticoli(False)
        '---041211
        lblMessAgg.Text = "" 'GIU130112()
        lblMessAgg.BorderStyle = BorderStyle.None
        lblSuperatoScMax.Text = ""
        lblSuperatoScMax.BorderStyle = BorderStyle.None
        AzzeraTxtInsArticoli()
        Try
            Dim myPageIndex As Integer = GridViewDett.PageIndex 'giu061119

            Dim myRowIndex As Integer = e.RowIndex + (GridViewDett.PageSize * GridViewDett.PageIndex)
            GridViewDett.SelectedIndex = e.RowIndex
            Dim RigaDel As Integer = 0
            RigaDel = GridViewDett.SelectedDataKey.Value 'GIU071119
            'CANCELLO I LOTTI COLLEGATI
            '''Passo = 1
            '''Session(SWOPDETTDOCL) = SWOPNESSUNA
            '''If CancellaLottiRiga(RigaDel) = False Then
            '''    'NON FACCIO NULLA
            '''End If
            Passo = 2
            '--------------------------
            aDataView1 = Session("aDataView1")
            'GIU030117 giu100117
            Dim myCArtDel As String = ""
            Try
                myCArtDel = aDataView1.Item(myRowIndex).Item("Cod_Articolo").ToString.Trim
            Catch ex As Exception
                myCArtDel = ""
            End Try
            '---------
            aDataView1.Item(myRowIndex).Delete()
            SqlAdapDocDett = Session("aSqlAdap")
            DsContrattiDett = Session("aDsDett")
            Dim RowD As DSDocumenti.ContrattiDRow
            Passo = 3
            'GIU030117 
            Try
                If myCArtDel.Trim <> "" Then
                    Dim RowsDel() As DataRow = Me.DsContrattiDett.ContrattiD.Select("Riga>" & RigaDel.ToString.Trim, "Riga")
                    Dim RigaApp As Integer = 0
                    For Each RowD In RowsDel
                        If RowD.IsRiga_AppartenenzaNull Then
                            Exit For
                        ElseIf RowD.Riga_Appartenenza = 0 Then
                            Exit For
                        ElseIf Not RowD.IsCod_ArticoloNull Then
                            If RowD.Cod_Articolo.Trim <> "" Then
                                Exit For
                            End If
                        ElseIf RowD.Qta_Evasa <> 0 Then
                            Exit For
                        ElseIf RowD.Qta_Ordinata <> 0 Then
                            Exit For
                        ElseIf RowD.Prezzo <> 0 Then
                            Exit For
                        End If
                        If RigaApp <> 0 And RowD.Riga_Appartenenza <> RigaApp Then
                            Exit For
                        End If
                        RigaApp = RowD.Riga_Appartenenza
                        If RowD.IsCod_ArticoloNull Then
                            RowD.Delete()
                        ElseIf RowD.Cod_Articolo.Trim = "" Then
                            RowD.Delete()
                        Else
                            Exit For
                        End If
                    Next
                    RowsDel = Nothing
                End If
            Catch ex As Exception
                'NULLA PROSEGUO TANTO NON E' IMPORTANTE
            End Try
            '' ''GIU210615 OK TESTO ANCHE CHE NON ABBIA IL CODICE ARTICOLO
            '' ''Cancello le righe di appartenenza 
            ' ''Dim RowsDel() As DataRow = Me.DsContrattiDett.ContrattiD.Select("Riga_Appartenenza=" & RigaDel.ToString.Trim)
            ' ''For Each RowD In RowsDel
            ' ''    If RowD.IsCod_ArticoloNull Then
            ' ''        RowD.Delete()
            ' ''    ElseIf RowD.Cod_Articolo.Trim = "" Then
            ' ''        RowD.Delete()
            ' ''    End If
            ' ''Next
            ' ''RowsDel = Nothing
            '----------------------------------
            Passo = 4
            If (aDataView1 Is Nothing) Then
                aDataView1 = New DataView(DsContrattiDett.ContrattiD)
            End If
            'Rinumero tutto
            Passo = 5
            'giu030117
            '' ''GIU210615
            ' ''Dim SaveRiga As Integer
            ' ''Dim RowsDRA() As DataRow
            ' ''Dim RowDRA As DSDocumenti.ContrattiDRow
            '---------
            'giu290722
            ' ''Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("", "Riga")
            ' ''Dim Riga As Integer = 1
            ' ''For Each RowD In RowsD
            ' ''    If RowD.Riga <> Riga Then
            ' ''        'giu030117 SaveRiga = RowD.Riga
            ' ''        If RinumeraLottiRiga(RowD.Riga, Riga) = False Then 'giu170112
            ' ''            Exit Sub
            ' ''        End If
            ' ''        RowD.BeginEdit()
            ' ''        RowD.Riga = Riga
            ' ''        RowD.EndEdit()
            ' ''        'giu030117
            ' ''        '' ''GIU210615
            ' ''        ' ''RowsDRA = Me.DsContrattiDett.ContrattiD.Select("Riga_Appartenenza=" & SaveRiga.ToString.Trim)
            ' ''        ' ''For Each RowDRA In RowsDRA
            ' ''        ' ''    RowDRA.BeginEdit()
            ' ''        ' ''    RowDRA.Riga_Appartenenza = Riga
            ' ''        ' ''    RowDRA.EndEdit()
            ' ''        ' ''Next
            ' ''        '---------
            ' ''    End If
            ' ''    Riga += 1
            ' ''Next
            '---------
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            '--------------
            Passo = 6
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
            If GridViewDett.Rows.Count = 0 Then
                SetBtnPrimaRigaEnabled(True)
            Else
                SetBtnPrimaRigaEnabled(False)
                If GridViewDett.Rows.Count > 0 Then 'giu061119 riposiziono nella pagina in cui ero
                    If GridViewDett.PageCount >= myPageIndex Then
                        GridViewDett.PageIndex = myPageIndex
                    End If
                End If
            End If
            'ASPETTO CHE MI SELEZIONI UN DETTAGLIO
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            '---
            Passo = 7
            Dim strErrore As String = ""
            If AggiornaImporto(DsContrattiDett, strErrore) = False Then
                If strErrore.Trim <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                End If
            End If
            'RICARICO I LOTTI PER LA RIGA SELEZIONATA
            Passo = 8
            'GIU071119 BuildLottiRigaDB(-1) 'SVUOTO TUTTI I LOTTI ED ASPETTO CHE SELEZIONI
            '--- LOTTI
            Dim RigaSel As Integer = 0
            Try
                GridViewDett.SelectedIndex = 0
                RigaSel = GridViewDett.SelectedIndex 'giu180419
                If RigaSel = 0 Then 'giu180419
                    RigaSel = GridViewDett.SelectedDataKey.Value
                End If
                If RigaSel > 0 Then 'giu180419
                    PopolaTxtDett()
                Else
                    AzzeraTxtInsArticoli()
                End If
            Catch ex As Exception
                RigaSel = 0
                'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
                LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
                AzzeraTxtInsArticoli()
            End Try
            '-- LOTTI 
            'giu290722 BuildLottiRigaDB(RigaSel)
            '-----------
        Catch Ex As Exception
            GridViewDett.Enabled = True
            Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GridViewDett_RowDeleting. Passo: " & Passo.ToString, Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        GridViewDett.Enabled = True
        Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub
    'giu180420
    Private Sub btnDelDettagli_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelDettagli.Click
        If Session(SWOP) <> SWOPMODIFICA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br>" & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '
        If Session(SWOPDETTDOC) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica dettaglio documento.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'LOTTI
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If GridViewDett.Rows.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dettaglio presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-
        Dim myMess As String = ""
        Dim SWStatoDoc As String = Session(CSTSTATODOC)
        If IsNothing(SWStatoDoc) Then
            SWStatoDoc = ""
        End If
        If String.IsNullOrEmpty(SWStatoDoc) Then
            SWStatoDoc = ""
        End If
        If SWStatoDoc = "" Or Not IsNumeric(SWStatoDoc) Then
            SWStatoDoc = ""
        End If
        If SWStatoDoc <> "0" And SWStatoDoc <> "5" Then
            myMess = "STATO documento risulta: "
            If SWStatoDoc = "1" Then
                myMess += "Evaso"
            ElseIf SWStatoDoc = "2" Then
                myMess += "Parz.Evaso"
            ElseIf SWStatoDoc = "3" Then
                myMess += "Chiuso non evaso"
            ElseIf SWStatoDoc = "2" Then
                myMess += "Non evadibile"
            ElseIf SWStatoDoc = "2" Then
                myMess += "Parz.Evaso"
            Else
                myMess += SWStatoDoc
            End If
            myMess += "<br>"
        End If
        If _WucElement.CKAttEvPagEv = True Then
            myMess += "Risultano EVASE/FATTURATE delle Attività/Scadenze<br>"
        End If
        '----------------------------
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "DelDettagli"
        ModalPopup.Show("Cancella Dettagli selezionati", "Confermi la cancellazione Dettagli selezionati ?" & IIf(myMess.Trim <> "", "<br>ATTENZIONE<br>" & myMess, ""), WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Function DelDettagli() As Boolean

        Dim Passo As Integer = 0
        GridViewDett.Enabled = False
        'Selezionato
        EnableTOTxtInsArticoli(False)
        '---041211
        lblMessAgg.Text = "" 'GIU130112()
        lblMessAgg.BorderStyle = BorderStyle.None
        lblSuperatoScMax.Text = ""
        lblSuperatoScMax.BorderStyle = BorderStyle.None
        AzzeraTxtInsArticoli()
        Try
            Passo = 1
            'NON SONO GESTITI CANCELLO I LOTTI COLLEGATI 
            ' ''If CancellaLottiRiga(RigaDel) = False Then
            ' ''    'NON FACCIO NULLA
            ' ''End If
            '--------------------------
            SqlAdapDocDett = Session("aSqlAdap")
            DsContrattiDett = Session("aDsDett")
            Dim RowD As DSDocumenti.ContrattiDRow
            Passo = 2
            Dim RowsDel() As DataRow = Me.DsContrattiDett.ContrattiD.Select("")
            For Each RowD In RowsDel
                RowD.Delete()
            Next
            RowsDel = Nothing
            '----------------------------------
            Passo = 3
            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
            If GridViewDett.Rows.Count = 0 Then
                SetBtnPrimaRigaEnabled(True)
            Else
                SetBtnPrimaRigaEnabled(False)
            End If
            '-------------------------------------
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            '---
            Passo = 4
            'NON GESTITI BuildLottiRigaDB(-1) 'SVUOTO TUTTI I LOTTI ED ASPETTO CHE SELEZIONI
            Dim RigaSel As Integer = 0
            Try
                GridViewDett.SelectedIndex = 0
                RigaSel = GridViewDett.SelectedIndex 'giu180419
                If RigaSel = 0 Then 'giu180419
                    RigaSel = GridViewDett.SelectedDataKey.Value
                End If
                If RigaSel > 0 Then 'giu180419
                    PopolaTxtDett()
                Else
                    AzzeraTxtInsArticoli()
                End If
            Catch ex As Exception
                RigaSel = 0
                'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
                LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
                AzzeraTxtInsArticoli()
            End Try
            '-- LOTTI 
            '''BuildLottiRigaDB(RigaSel)
            '-----------
        Catch Ex As Exception
            GridViewDett.Enabled = True
            Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DelDettagli. Passo: " & Passo.ToString, Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

        GridViewDett.Enabled = True
        Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Function
    'GIU170112
    '''Private Function RinumeraLottiRiga(ByVal RigaRin As Integer, ByVal NewRiga As Integer) As Boolean
    'giu290722
    ' ''GridViewDettL.Enabled = False
    ' ''Try
    ' ''    BuildLottiRigaDB(RigaRin)
    ' ''    aDataView1L = Session("aDataView1L")
    ' ''    SqlAdapDocDettL = Session("aSqlAdapL")
    ' ''    DsContrattiDettL = Session("aDsDettL")
    ' ''    Dim RowD As DSDocumenti.ContrattiDLottiRow
    ' ''    'Rinumero
    ' ''    Dim RowsSel() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("")
    ' ''    For Each RowD In RowsSel
    ' ''        RowD.BeginEdit()
    ' ''        RowD.Riga = NewRiga
    ' ''        RowD.EndEdit()
    ' ''    Next
    ' ''    'AGGIORNO SUL DB
    ' ''    Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''    RowsSel = Nothing
    ' ''    '---
    ' ''    If (aDataView1L Is Nothing) Then
    ' ''        aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    ' ''    End If
    ' ''    If aDataView1l.Count > 0 Then aDataView1L.Sort = "NCollo"
    ' ''    '--------------
    ' ''    GridViewDettL.DataSource = aDataView1L
    ' ''    GridViewDettL.EditIndex = -1
    ' ''    GridViewDettL.DataBind()
    ' ''    SetBtnCaricoLottiEnabled(True)
    ' ''    If GridViewDettL.Rows.Count = 0 Then
    ' ''        SetBtnPrimaRigaLEnabled(True)
    ' ''    Else
    ' ''        SetBtnPrimaRigaLEnabled(False)
    ' ''    End If
    ' ''    Session("aDataView1L") = aDataView1L
    ' ''    Session("aSqlAdapL") = SqlAdapDocDettL
    ' ''    Session("aDsDettL") = DsContrattiDettL
    ' ''    '---
    ' ''    RinumeraLottiRiga = True
    ' ''Catch Ex As Exception
    ' ''    GridViewDettL.Enabled = True
    ' ''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''    ModalPopup.Show("Errore in ContrattiDett.RinumeraLottiRiga", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''    Exit Function
    ' ''End Try
    ' ''GridViewDettL.Enabled = True
    ''''Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
    ' ''Session(SWMODIFICATO) = SWSI
    '''End Function
    '''Private Function CancellaLottiRiga(ByVal RigaDel As Integer) As Boolean
    'giu290722
    ' ''GridViewDettL.Enabled = False
    ' ''Try
    ' ''    aDataView1L = Session("aDataView1L")
    ' ''    SqlAdapDocDettL = Session("aSqlAdapL")
    ' ''    DsContrattiDettL = Session("aDsDettL")
    ' ''    Dim RowD As DSDocumenti.ContrattiDLottiRow
    ' ''    'Cancello
    ' ''    Dim RowsDel() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("Riga = " & RigaDel)
    ' ''    For Each RowD In RowsDel
    ' ''        RowD.Delete()
    ' ''        'AGGIORNO SUL DB
    ' ''        Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''    Next
    ' ''    RowsDel = Nothing
    ' ''    '---
    ' ''    If (aDataView1L Is Nothing) Then
    ' ''        aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    ' ''    End If

    ' ''    'Rinumero tutto
    ' ''    Dim RowsD() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("", "NCollo")
    ' ''    Dim NCollo As Integer = 1
    ' ''    For Each RowD In RowsD
    ' ''        If RowD.NCollo <> NCollo Then
    ' ''            RowD.BeginEdit()
    ' ''            RowD.NCollo = NCollo
    ' ''            RowD.EndEdit()
    ' ''            'AGGIORNO SUL DB
    ' ''            Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''            '---------------
    ' ''        End If
    ' ''        NCollo += 1
    ' ''    Next
    ' ''    If aDataView1l.Count > 0 Then aDataView1L.Sort = "NCollo"
    ' ''    '--------------
    ' ''    GridViewDettL.DataSource = aDataView1L
    ' ''    GridViewDettL.EditIndex = -1
    ' ''    GridViewDettL.DataBind()
    ' ''    SetBtnCaricoLottiEnabled(True)
    ' ''    If GridViewDettL.Rows.Count = 0 Then
    ' ''        SetBtnPrimaRigaLEnabled(True)
    ' ''    Else
    ' ''        SetBtnPrimaRigaLEnabled(False)
    ' ''    End If
    ' ''    Session("aDataView1L") = aDataView1L
    ' ''    Session("aSqlAdapL") = SqlAdapDocDettL
    ' ''    Session("aDsDettL") = DsContrattiDettL
    ' ''    '---
    ' ''    CancellaLottiRiga = True
    ' ''Catch Ex As Exception
    ' ''    GridViewDettL.Enabled = True
    ' ''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''    ModalPopup.Show("Errore in ContrattiDett.GridViewDettL_RowDeleting", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''    Exit Function
    ' ''End Try
    ' ''GridViewDettL.Enabled = True
    '''Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
    ' ''Session(SWMODIFICATO) = SWSI
    '''End Function
    '''Private Sub GridViewDettL_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles GridViewDettL.RowDeleting
    'giu290722
    ' ''GridViewDettL.Enabled = False
    ' ''Try
    ' ''    Dim myRowIndex As Integer = e.RowIndex + (GridViewDettL.PageSize * GridViewDettL.PageIndex)
    ' ''    GridViewDettL.SelectedIndex = e.RowIndex
    ' ''    Dim RigaDel As Integer = GridViewDettL.SelectedDataKey.Value
    ' ''    aDataView1L = Session("aDataView1L")
    ' ''    aDataView1L.Item(myRowIndex).Delete()
    ' ''    SqlAdapDocDettL = Session("aSqlAdapL")
    ' ''    DsContrattiDettL = Session("aDsDettL")
    ' ''    Dim RowD As DSDocumenti.ContrattiDLottiRow 'ContrattiDRow
    ' ''    '---------------------------------- AGGIORNO
    ' ''    Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''    If (aDataView1L Is Nothing) Then
    ' ''        aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    ' ''    End If
    ' ''    'Rinumero tutto
    ' ''    Dim RowsD() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("", "NCollo")
    ' ''    Dim NCollo As Integer = 1
    ' ''    For Each RowD In RowsD
    ' ''        If RowD.NCollo <> NCollo Then
    ' ''            RowD.BeginEdit()
    ' ''            RowD.NCollo = NCollo
    ' ''            RowD.EndEdit()
    ' ''            'AGGIORNO SUL DB
    ' ''            Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''            '---------------
    ' ''        End If
    ' ''        NCollo += 1
    ' ''    Next
    ' ''    DsContrattiDettL.ContrattiDLotti.AcceptChanges() 'giu290312
    ' ''    If aDataView1l.Count > 0 Then aDataView1L.Sort = "NCollo"
    ' ''    '--------------
    ' ''    GridViewDettL.DataSource = aDataView1L
    ' ''    GridViewDettL.EditIndex = -1
    ' ''    GridViewDettL.DataBind()
    ' ''    SetBtnCaricoLottiEnabled(True)
    ' ''    If GridViewDettL.Rows.Count = 0 Then
    ' ''        SetBtnPrimaRigaLEnabled(True)
    ' ''    Else
    ' ''        SetBtnPrimaRigaLEnabled(False)
    ' ''    End If
    ' ''    Session("aDataView1L") = aDataView1L
    ' ''    Session("aSqlAdapL") = SqlAdapDocDettL
    ' ''    Session("aDsDettL") = DsContrattiDettL
    ' ''    '---
    ' ''Catch Ex As Exception
    ' ''    GridViewDettL.Enabled = True
    ' ''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''    ModalPopup.Show("Errore in ContrattiDett.GridViewDettL_RowDeleting", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''    Exit Sub
    ' ''End Try
    '' ''GIU160715 Abilito i DETTAGLI 
    ' ''GridViewDett.Enabled = True
    '' ''----------------------------
    ' ''GridViewDettL.Enabled = True
    '''Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
    ' ''Session(SWMODIFICATO) = SWSI
    '''End Sub

    Private Sub GridViewDett_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridViewDett.RowEditing
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu160715
        Dim myRiga As Integer = 0
        If (e Is Nothing) Then
            myRiga = GridViewDett.SelectedIndex
        Else
            myRiga = e.NewEditIndex
        End If
        '-------------------------
        Try
            GridViewDett.SelectedIndex = myRiga 'giu160715 e.NewEditIndex
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GridViewDett_RowEditing: posizionamento riga da modificare, ritentare la modifica.", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        EnableTOTxtInsArticoli(True)
        GridViewDett.EditIndex = myRiga 'e.NewEditIndex
        aDataView1 = Session("aDataView1")
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu311011
        GridViewDett.DataSource = aDataView1
        GridViewDett.DataBind()
        '-----------
        'Selezionato
        'giu130112 errore in posizionamento 
        Dim row As GridViewRow = Nothing
        Try
            row = GridViewDett.Rows(myRiga) 'giu160715 e.NewEditIndex)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GridViewDett_RowEditing", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        'GIU281211 PER OVVIARE ALL'ERRORE QUANDO UNA COLONNA NON è VISIBILE: ES. ordini o prveenivi la qta_Evasa non è visibile
        Try
            CType(row.Cells(CellIdx.Qta).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.Qta).Controls(0), TextBox).Text, -1)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.QtaEv).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaEv).Controls(0), TextBox).Text, -1)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.QtaIn).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaIn).Controls(0), TextBox).Text, -1)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.QtaRe).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaRe).Controls(0), TextBox).Text, -1)
        Catch ex As Exception
        End Try

        Try
            CType(row.Cells(CellIdx.Prz).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.Prz).Controls(0), TextBox).Text, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.ScVal).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.ScVal).Controls(0), TextBox).Text, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.Sc1).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.Sc1).Controls(0), TextBox).Text, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.Sc2).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.Sc2).Controls(0), TextBox).Text, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
        Catch ex As Exception
        End Try
        '-----------
        'giu021211 
        lblMessAgg.Text = "" 'GIU130112()
        lblMessAgg.BorderStyle = BorderStyle.None
        lblSuperatoScMax.Text = ""
        lblSuperatoScMax.BorderStyle = BorderStyle.None
        PopolaTxtDett()
        'RICARICO I LOTTI PER LA RIGA SELEZIONATA
        '''BuildLottiRigaDB(GridViewDett.SelectedDataKey.Value)
        '-----------
        Session(SWOPDETTDOC) = SWOPMODIFICA 'giu191111
        Session(SWOPDETTDOCR) = SWOPNESSUNA 'GIU030212
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
        'giu160715 
        If txtCodArtIns.Enabled Then
            txtCodArtIns.Focus()
        Else
            GridViewDett.Focus()
        End If
    End Sub
    '''Private Sub GridViewDettL_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridViewDettL.RowEditing
    'giu290722
    ' ''If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
    ' ''    _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
    ' ''    Exit Sub
    ' ''End If
    '' ''giu160715
    '' ''-------------------------
    ' ''Try
    ' ''    GridViewDettL.SelectedIndex = e.NewEditIndex
    ' ''Catch ex As Exception
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''    ModalPopup.Show("Errore in ContrattiDett.GridViewDettL_RowEditing: posizionamento riga LOTTI da modificare, ritentare la modifica.", ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''    Exit Sub
    ' ''End Try
    '' ''------------------------------------------------------------------------
    '' ''GIU301111 disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
    '' ''------------------------------------------------------------------------
    ' ''If GridViewDett.EditIndex <> -1 Then
    ' ''    SqlAdapDocDett = Session("aSqlAdap")
    ' ''    DsContrattiDett = Session("aDsDett")
    ' ''    If (aDataView1 Is Nothing) Then
    ' ''        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
    ' ''    End If
    ' ''    If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
    ' ''    Session("aDataView1") = aDataView1
    ' ''    Session("aDsDett") = DsContrattiDett
    ' ''    GridViewDett.DataSource = aDataView1
    ' ''    GridViewDett.EditIndex = -1
    ' ''    GridViewDett.DataBind()
    ' ''End If
    ' ''EnableTOTxtInsArticoli(False)
    ' ''Session(SWOPDETTDOC) = SWOPNESSUNA
    ' ''Session(SWOPDETTDOCR) = SWOPNESSUNA
    ' ''GridViewDett.Enabled = False
    '' ''------------------------------------------------------------------------
    ' ''GridViewDettL.EditIndex = e.NewEditIndex
    ' ''aDataView1L = Session("aDataView1L")
    ' ''If aDataView1lL.Count > 0 Then aDataView1L.Sort = "NCollo"
    ' ''GridViewDettL.DataSource = aDataView1L
    ' ''GridViewDettL.DataBind()
    ' '' ''giu130417 per evitare ilposizionamento sempre alla prima riga GridViewDettL.Focus()
    '''Session(SWOPDETTDOCL) = SWOPMODIFICA
    ' ''Session(SWMODIFICATO) = SWSI
    '''End Sub

    '''Private Sub BuildLottiRigaDB(ByVal Riga As Integer)
    '''    'giu100322 GESTIONE LOTTI NON GESTITA ESCO SENZA FARE NULLA
    '''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    '''    Exit Sub
    '''    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    '''    Dim Passo As Integer = 1
    '''    Try
    '''        'Simile al LOAD
    '''        SetCdmDAdp()
    '''        Passo = 2
    '''        Session("aSqlAdapL") = SqlAdapDocDettL
    '''        '--
    '''        If (Session("aDsDettL") Is Nothing) Then
    '''            DsContrattiDettL = New DSDocumenti
    '''        Else
    '''            DsContrattiDettL = Session("aDsDettL")
    '''        End If
    '''        '--
    '''        Passo = 3
    '''        Dim myID As String = Session(IDDOCUMENTI)
    '''        If IsNothing(myID) Then
    '''            myID = ""
    '''        End If
    '''        If String.IsNullOrEmpty(myID) Then
    '''            myID = ""
    '''        End If
    '''        'giu050220
    '''        Dim myIDDurataNum As String = Session(IDDURATANUM)
    '''        If IsNothing(myIDDurataNum) Then
    '''            myIDDurataNum = "0"
    '''        End If
    '''        If String.IsNullOrEmpty(myIDDurataNum) Then
    '''            myIDDurataNum = "0"
    '''        End If
    '''        '-
    '''        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
    '''        If IsNothing(myIDDurataNumR) Then
    '''            myIDDurataNumR = "0"
    '''        End If
    '''        If String.IsNullOrEmpty(myIDDurataNumR) Then
    '''            myIDDurataNumR = "0"
    '''        End If
    '''        '----------
    '''        Passo = 4
    '''        If Session(SWOP) <> SWOPNUOVO Then 'giu041211
    '''            If myID = "" Or Not IsNumeric(myID) Then
    '''                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. (BuildLottiRigaDB) Passo: " & Passo.ToString)
    '''                Exit Sub
    '''            End If
    '''        End If
    '''        Passo = 5
    '''        DsContrattiDettL.ContrattiDLotti.Clear()
    '''        SqlDbSelectCmdL.Parameters.Item("@IDDocumenti").Value = CLng(IIf(myID.Trim = "", -1, myID.Trim))
    '''        SqlDbSelectCmdL.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
    '''        SqlDbSelectCmdL.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
    '''        SqlDbSelectCmdL.Parameters.Item("@Riga").Value = Riga
    '''        SqlAdapDocDettL.Fill(DsContrattiDettL.ContrattiDLotti)
    '''        '---------------------------------------
    '''        Passo = 6
    '''        Session("aSqlAdapL") = SqlAdapDocDettL
    '''        Session("aDsDettL") = DsContrattiDettL
    '''        '-- LOTTI
    '''        Passo = 7
    '''        aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    '''        If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
    '''        GridViewDettL.DataSource = aDataView1L
    '''        Session("aDataView1L") = aDataView1L
    '''        Passo = 8
    '''        GridViewDettL.DataBind()
    '''        Passo = 9
    '''        If txtCodArtIns.Text.Trim <> "" Then
    '''            SetBtnCaricoLottiEnabled(True)
    '''            If GridViewDettL.Rows.Count = 0 Then
    '''                SetBtnPrimaRigaLEnabled(True)
    '''            Else
    '''                SetBtnPrimaRigaLEnabled(False)
    '''            End If
    '''        Else
    '''            SetBtnCaricoLottiEnabled(False)
    '''            SetBtnPrimaRigaLEnabled(False)
    '''        End If
    '''        'GIU070515 ERR.: DISABILITO LOTTI SENON SONO IN MODIFICA
    '''        Passo = 10
    '''        If Session(SWOP) = SWOPNESSUNA Then
    '''            SetBtnCaricoLottiEnabled(False)
    '''            SetBtnPrimaRigaLEnabled(False)
    '''            GridViewDettL.Enabled = False
    '''        ElseIf Session(SWOP) = SWOPMODIFICA Then
    '''            If txtCodArtIns.Text.Trim <> "" Then
    '''                SetBtnCaricoLottiEnabled(True)
    '''                GridViewDettL.Enabled = True
    '''            Else
    '''                SetBtnCaricoLottiEnabled(False)
    '''                SetBtnPrimaRigaLEnabled(False)
    '''                GridViewDettL.Enabled = False
    '''            End If
    '''        End If
    '''        '-----
    '''    Catch ex As Exception
    '''        Session(SWOPDETTDOCL) = SWOPNESSUNA
    '''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '''        ModalPopup.Show("Errore in ContrattiDett.BuildLottiRigaDB. Passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ERROR)
    '''        Exit Sub
    '''    End Try
    '''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    '''End Sub

    Private Sub GridViewDett_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridViewDett.RowUpdating
        'giu110112 non allineato per lo sconto VALORE
        btnAggArtGridSel_Click(btnAggArtGridSel, Nothing)

    End Sub
    '''Private Sub GridViewDettL_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridViewDettL.RowUpdating
    'giu290722
    '' ''----------------------------------------------------------------------------------------
    '' ''ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
    '' ''----------------------------------------------------------------------------------------
    ' ''If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
    ' ''    _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
    ' ''    Exit Sub
    ' ''End If
    ' ''Dim myID As String = Session(IDDOCUMENTI)
    ' ''If IsNothing(myID) Then
    ' ''    myID = ""
    ' ''End If
    ' ''If String.IsNullOrEmpty(myID) Then
    ' ''    myID = ""
    ' ''End If
    ' ''If myID = "" Or Not IsNumeric(myID) Then
    ' ''    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
    ' ''    Exit Sub
    ' ''End If
    '' ''giu050220
    ' ''Dim myIDDurataNum As String = Session(IDDURATANUM)
    ' ''If IsNothing(myIDDurataNum) Then
    ' ''    myIDDurataNum = "0"
    ' ''End If
    ' ''If String.IsNullOrEmpty(myIDDurataNum) Then
    ' ''    myIDDurataNum = "0"
    ' ''End If
    '' ''-
    ' ''Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
    ' ''If IsNothing(myIDDurataNumR) Then
    ' ''    myIDDurataNumR = "0"
    ' ''End If
    ' ''If String.IsNullOrEmpty(myIDDurataNumR) Then
    ' ''    myIDDurataNumR = "0"
    ' ''End If
    '' ''----------
    ' ''Try
    ' ''    Dim myRowIndex As Integer = e.RowIndex + (GridViewDettL.PageSize * GridViewDettL.PageIndex)
    ' ''    Dim row As GridViewRow = GridViewDettL.Rows(e.RowIndex)
    ' ''    Dim Comodo As String = CType(row.Cells(CellIdxL.QtaCollo).Controls(0), TextBox).Text
    ' ''    If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
    ' ''    Dim myQuantita As Integer = CDbl(Comodo.Trim)
    ' ''    Dim Comodo1 As String = CType(row.Cells(CellIdxL.Lotto).Controls(0), TextBox).Text
    ' ''    Dim Comodo2 As String = CType(row.Cells(CellIdxL.NSerie).Controls(0), TextBox).Text
    ' ''    Dim myErrore As String = ""
    ' ''    If myQuantita = 0 Then
    ' ''        myErrore += "Quantità obbligatoria"
    ' ''    End If
    ' ''    If Comodo1.Trim = "" And Comodo2.Trim = "" Then
    ' ''        myErrore += " <br> Lotto/N° di serie obbligatori"
    ' ''    End If
    ' ''    '-
    ' ''    If myErrore.Trim <> "" Then
    ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
    ' ''        Exit Sub
    ' ''    End If
    ' ''    'Fabio18042016 - Controllo che non si inseriscano seriali identici
    ' ''    DsContrattiDettL = Session("aDsDettL")
    ' ''    If Comodo1.Trim <> "" Then
    ' ''        If DsContrattiDettL.ContrattiDLotti.Select("Lotto ='" + Controlla_Apice(Comodo1.Trim) + "' AND NCollo<>" & GridViewDettL.SelectedDataKey.Value.ToString.Trim).Length > 0 Then 'GIU120417 
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''            ModalPopup.Show("Attenzione", "Non è consentito inserire lo stesso lotto più volte!</BR>Operazione interrotta.", WUC_ModalPopup.TYPE_ALERT)
    ' ''            Exit Sub
    ' ''        End If
    ' ''    End If
    ' ''    If Comodo2.Trim <> "" Then
    ' ''        If DsContrattiDettL.ContrattiDLotti.Select("NSerie ='" + Controlla_Apice(Comodo2.Trim) + "' AND NCollo<>" & GridViewDettL.SelectedDataKey.Value.ToString.Trim).Length > 0 Then 'GIU120417 
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''            ModalPopup.Show("Attenzione", "Non è consentito inserire lo stesso N° di serie più volte!</BR>Operazione interrotta.", WUC_ModalPopup.TYPE_ALERT)
    ' ''            Exit Sub
    ' ''        End If
    ' ''    End If
    ' ''    'End Fabio

    ' ''    aDataView1L = Session("aDataView1L")
    ' ''    aDataView1L.Item(myRowIndex).Item("IDDocumenti") = CLng(myID)
    ' ''    aDataView1L.Item(myRowIndex).Item("DurataNum") = Val(myIDDurataNum)
    ' ''    aDataView1L.Item(myRowIndex).Item("DurataNumRiga") = Val(myIDDurataNumR)
    ' ''    aDataView1L.Item(myRowIndex).Item("Riga") = GridViewDett.SelectedDataKey.Value
    ' ''    aDataView1L.Item(myRowIndex).Item("Cod_Articolo") = txtCodArtIns.Text.Trim
    ' ''    aDataView1L.Item(myRowIndex).Item("Lotto") = CType(row.Cells(CellIdxL.Lotto).Controls(0), TextBox).Text
    ' ''    aDataView1L.Item(myRowIndex).Item("NSerie") = CType(row.Cells(CellIdxL.NSerie).Controls(0), TextBox).Text
    ' ''    aDataView1L.Item(myRowIndex).Item("QtaColli") = myQuantita
    ' ''    aDataView1L.Item(myRowIndex).Item("Sfusi") = 0

    ' ''    SqlAdapDocDettL = Session("aSqlAdapL")
    ' ''    DsContrattiDettL = Session("aDsDettL")

    ' ''    Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    ' ''    If (aDataView1L Is Nothing) Then
    ' ''        aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    ' ''    End If
    ' ''    If aDataView1l.Count > 0 Then aDataView1L.Sort = "NCollo"
    ' ''    Session("aDataView1L") = aDataView1L
    ' ''    Session("aDsDettL") = DsContrattiDettL
    ' ''    GridViewDettL.DataSource = aDataView1L
    ' ''    GridViewDettL.EditIndex = -1
    ' ''    GridViewDettL.DataBind()
    ' ''Catch Ex As Exception
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''    ModalPopup.Show("Errore ContrattiDett.GridViewDettL_RowUpdating", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''End Try
    '' ''---
    '' ''Abilito i DETTAGLI 
    ' ''GridViewDett.Enabled = True
    '' ''--------------------------------------------------------------
    '''Session(SWOPDETTDOCL) = SWOPNESSUNA
    ' ''Session(SWMODIFICATO) = SWSI
    '''End Sub

    Private Sub GridViewDett_NewRigaSotto(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDett.SelectedIndexChanged
        EnableTOTxtInsArticoli(False)
        GridViewDett.EditIndex = -1
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        SqlAdapDocDett = Session("aSqlAdap")
        DsContrattiDett = Session("aDsDett")
        Try

            Dim RigaSel As Integer = GridViewDett.SelectedDataKey.Value
            Dim GrdSel As Integer = GridViewDett.SelectedIndex
            Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("Riga >=" & RigaSel, "Riga DESC")
            Dim RowD As DSDocumenti.ContrattiDRow
            For Each RowD In RowsD
                If RowD.Riga <> RigaSel Then
                    'GIU240623
                    Session(SWOPDETTDOCL) = SWOPNESSUNA
                    '''If RinumeraLottiRiga(RowD.Riga, RowD.Riga + 1) = False Then 'giu170112
                    '''    Exit Sub
                    '''End If
                    RowD.BeginEdit()
                    RowD.Riga += 1
                    RowD.EndEdit()
                End If
            Next
            '-
            'giu270320 riporto serie/lotto cod_filiale
            Dim rowCADettS() As DataRow
            If Me.DsContrattiDett.ContrattiD.Select("Serie<>'' AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettS = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettS = Me.DsContrattiDett.ContrattiD.Select("Serie<>''", "Riga")
            End If
            Dim rowCADettF() As DataRow
            If Me.DsContrattiDett.ContrattiD.Select("Cod_Filiale>0 AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettF = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettF = Me.DsContrattiDett.ContrattiD.Select("Cod_Filiale>0", "Riga")
            End If
            'giu231023
            Dim rowCADettRAV() As DataRow
            If Me.DsContrattiDett.ContrattiD.Select("RespArea>0 AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettRAV = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettRAV = Me.DsContrattiDett.ContrattiD.Select("RespArea>0", "Riga")
            End If
            '--------
            Dim newRowDocD As DSDocumenti.ContrattiDRow = DsContrattiDett.ContrattiD.NewContrattiDRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
                .DurataNum = Val(myIDDurataNum)
                .DurataNumRiga = Val(myIDDurataNumR)
                .Riga = RigaSel + 1
                .SetRiga_AppartenenzaNull() 'GIU210615
                .Prezzo = 0
                .Prezzo_Netto = 0
                .Qta_Allestita = 0
                .Qta_Evasa = 0
                .Qta_Impegnata = 0
                .Qta_Ordinata = 0
                .Qta_Prenotata = 0
                .Qta_Residua = 0
                .Importo = 0
                'giu170412
                .PrezzoAcquisto = 0
                .PrezzoListino = 0
                .SWModAgenti = 0
                .PrezzoCosto = 0
                .Qta_Inviata = 0
                .Qta_Fatturata = 0
                If rowCADettS.Count > 0 Then
                    If IsDBNull(rowCADettS(0).Item("Serie")) Then
                        .Serie = ""
                    Else
                        .Serie = rowCADettS(0).Item("Serie")
                    End If
                    If IsDBNull(rowCADettS(0).Item("Lotto")) Then
                        .Lotto = ""
                    Else
                        .Lotto = rowCADettS(0).Item("Lotto")
                    End If
                    If Not IsDBNull(rowCADettS(0).Item("DataSc")) Then
                        .DataSc = rowCADettS(0).Item("DataSc")
                    End If
                    If Not IsDBNull(rowCADettS(0).Item("QtaDurataNumR0")) Then
                        .QtaDurataNumR0 = rowCADettS(0).Item("QtaDurataNumR0")
                    End If
                End If
                If rowCADettF.Count > 0 Then
                    If Not IsDBNull(rowCADettF(0).Item("Cod_Filiale")) Then
                        .Cod_Filiale = rowCADettF(0).Item("Cod_Filiale")
                    End If
                End If
                If rowCADettRAV.Count > 0 Then
                    'giu231023
                    If Not IsDBNull(rowCADettRAV(0).Item("RespArea")) Then
                        .RespArea = rowCADettRAV(0).Item("RespArea")
                    End If
                    If Not IsDBNull(rowCADettRAV(0).Item("RespVisite")) Then
                        .RespVisite = rowCADettRAV(0).Item("RespVisite")
                    End If
                End If
                '-ok
                .EndEdit()
            End With
            DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
            newRowDocD = Nothing

            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            'giu230115 se la nuova riga è nella pagina successiva sposto anche indexpage
            If (GrdSel + 2) > (GridViewDett.PageSize * (GridViewDett.PageIndex + 1)) Then
                GridViewDett.PageSize += 1
            End If
            '-----------------
            GridViewDett.DataBind()
            GridViewDett.SelectedIndex = GrdSel + 1

            'Selezionato
            '--giu041211
            Try 'giu250115
                RigaSel = GridViewDett.SelectedDataKey.Value
            Catch ex As Exception
                GridViewDett.SelectedIndex = GrdSel - 1
                RigaSel = GridViewDett.SelectedDataKey.Value
            End Try
            '-
            lblRigaSel.Text = FormattaNumero(RigaSel)
            lblMessAgg.Text = "" 'GIU130112()
            lblMessAgg.BorderStyle = BorderStyle.None
            lblSuperatoScMax.Text = ""
            lblSuperatoScMax.BorderStyle = BorderStyle.None

            AzzeraTxtInsArticoli()
           
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GrdiViewDett_SelectedChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '----------------------------------        
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA

        Session(SWMODIFICATO) = SWSI
        'giu160715
        Call GridViewDett_RowEditing(sender, Nothing)
    End Sub
    '''Private Sub GridViewDettL_NewRigaSotto(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDettL.SelectedIndexChanged
    '''    'giu290722
    '''    ' ''GridViewDettL.EditIndex = -1
    '''    ' ''Dim myID As String = Session(IDDOCUMENTI)
    '''    ' ''If IsNothing(myID) Then
    '''    ' ''    myID = ""
    '''    ' ''End If
    '''    ' ''If String.IsNullOrEmpty(myID) Then
    '''    ' ''    myID = ""
    '''    ' ''End If
    '''    ' ''If myID = "" Or Not IsNumeric(myID) Then
    '''    ' ''    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
    '''    ' ''    Exit Sub
    '''    ' ''End If
    '''    '' ''giu050220
    '''    ' ''Dim myIDDurataNum As String = Session(IDDURATANUM)
    '''    ' ''If IsNothing(myIDDurataNum) Then
    '''    ' ''    myIDDurataNum = "0"
    '''    ' ''End If
    '''    ' ''If String.IsNullOrEmpty(myIDDurataNum) Then
    '''    ' ''    myIDDurataNum = "0"
    '''    ' ''End If
    '''    '' ''-
    '''    ' ''Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
    '''    ' ''If IsNothing(myIDDurataNumR) Then
    '''    ' ''    myIDDurataNumR = "0"
    '''    ' ''End If
    '''    ' ''If String.IsNullOrEmpty(myIDDurataNumR) Then
    '''    ' ''    myIDDurataNumR = "0"
    '''    ' ''End If
    '''    '' ''----------
    '''    ' ''If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
    '''    ' ''    _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
    '''    ' ''    Exit Sub
    '''    ' ''End If

    '''    ' ''SqlAdapDocDettL = Session("aSqlAdapL")
    '''    ' ''DsContrattiDettL = Session("aDsDettL")
    '''    ' ''Try
    '''    ' ''    Dim RigaSel As Integer = GridViewDettL.SelectedDataKey.Value
    '''    ' ''    Dim GrdSel As Integer = GridViewDettL.SelectedIndex
    '''    ' ''    Dim RowsD() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("NCollo >=" & RigaSel, "NCollo DESC")
    '''    ' ''    Dim RowD As DSDocumenti.ContrattiDLottiRow
    '''    ' ''    For Each RowD In RowsD
    '''    ' ''        If RowD.NCollo <> RigaSel Then
    '''    ' ''            RowD.BeginEdit()
    '''    ' ''            RowD.NCollo += 1
    '''    ' ''            RowD.EndEdit()
    '''    ' ''            Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)
    '''    ' ''        End If
    '''    ' ''    Next
    '''    ' ''    'giu071211 totale qta fino adesso inserito
    '''    ' ''    Dim RowsDL() As DataRow = Me.DsContrattiDettL.ContrattiDLotti.Select("")
    '''    ' ''    Dim RowDL As DSDocumenti.ContrattiDLottiRow
    '''    ' ''    Dim QtaColli As Integer = 0
    '''    ' ''    For Each RowDL In RowsDL
    '''    ' ''        QtaColli += RowDL.QtaColli
    '''    ' ''    Next
    '''    ' ''    'giu071211 totale qta fino adesso inserito
    '''    ' ''    'giu301111
    '''    ' ''    '--- LOTTI PRENDO I DATI DALL'ARTICOLO DA MEMORIZZARE
    '''    ' ''    Dim RigaDettArt As Integer = GridViewDett.SelectedDataKey.Value
    '''    ' ''    Dim Comodo As String = ""
    '''    ' ''    Dim QtaRich As Integer = 0
    '''    ' ''    Dim QtaEvasa As Integer = 0 'giu291211 correzione se la qta' è l'evasa o ordinata
    '''    ' ''    Try
    '''    ' ''        Dim row As GridViewRow = GridViewDett.SelectedRow
    '''    ' ''        Comodo = IIf(row.Cells(CellIdx.Qta).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.Qta).Text.Trim)
    '''    ' ''        If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
    '''    ' ''        QtaRich = Comodo.Trim
    '''    ' ''    Catch ex As Exception
    '''    ' ''        QtaRich = 0
    '''    ' ''    End Try
    '''    ' ''    Try 'giu291211 correzione se la qta' è l'evasa o ordinata
    '''    ' ''        Dim row As GridViewRow = GridViewDett.SelectedRow
    '''    ' ''        Comodo = IIf(row.Cells(CellIdx.QtaEv).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.QtaEv).Text.Trim)
    '''    ' ''        If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
    '''    ' ''        QtaEvasa = Comodo.Trim
    '''    ' ''    Catch ex As Exception
    '''    ' ''        QtaEvasa = 0
    '''    ' ''    End Try
    '''    ' ''    'giu291211 correzione se la qta' è l'evasa o ordinata
    '''    ' ''    ' ''If QtaColli > QtaRich Or QtaColli = QtaRich Then
    '''    ' ''    ' ''    QtaColli = 0
    '''    ' ''    ' ''ElseIf QtaRich > QtaColli Then
    '''    ' ''    ' ''    QtaColli = QtaRich - QtaColli
    '''    ' ''    ' ''End If
    '''    ' ''    'giu291211 correzione se la qta' è l'evasa o ordinata
    '''    ' ''    Select Case myTipoDoc
    '''    ' ''        Case "PR", "PF", "CA", "TC" 'GIU020212
    '''    ' ''            If QtaColli > QtaRich Or QtaColli = QtaRich Then
    '''    ' ''                QtaColli = 0
    '''    ' ''            ElseIf QtaRich > QtaColli Then
    '''    ' ''                QtaColli = QtaRich - QtaColli
    '''    ' ''            End If
    '''    ' ''        Case Else
    '''    ' ''            If QtaColli > QtaEvasa Or QtaColli = QtaEvasa Then
    '''    ' ''                QtaColli = 0
    '''    ' ''            ElseIf QtaEvasa > QtaColli Then
    '''    ' ''                QtaColli = QtaEvasa - QtaColli
    '''    ' ''            End If
    '''    ' ''    End Select
    '''    ' ''    '----------------------------------------------------
    '''    ' ''    'giu071211 totale qta fino adesso inserito end
    '''    ' ''    '------------------------------------------------------------------------
    '''    ' ''    'GIU301111 disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
    '''    ' ''    '------------------------------------------------------------------------
    '''    ' ''    If GridViewDett.EditIndex <> -1 Then
    '''    ' ''        SqlAdapDocDett = Session("aSqlAdap")
    '''    ' ''        DsContrattiDett = Session("aDsDett")
    '''    ' ''        If (aDataView1 Is Nothing) Then
    '''    ' ''            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
    '''    ' ''        End If
    '''    ' ''        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
    '''    ' ''        Session("aDataView1") = aDataView1
    '''    ' ''        Session("aDsDett") = DsContrattiDett
    '''    ' ''        GridViewDett.DataSource = aDataView1
    '''    ' ''        GridViewDett.EditIndex = -1
    '''    ' ''        GridViewDett.DataBind()
    '''    ' ''    End If
    '''    ' ''    EnableTOTxtInsArticoli(False)
    '''    ' ''    'GIU030212
    '''    ' ''    Session(SWOPDETTDOC) = SWOPNESSUNA
    '''    ' ''    Session(SWOPDETTDOCR) = SWOPNESSUNA
    '''    ' ''    Session(SWOPDETTDOCL) = SWOPNESSUNA

    '''    ' ''    GridViewDett.Enabled = False
    '''    ' ''    '------------------------------------------------------------------------
    '''    ' ''    Dim newRowDocD As DSDocumenti.ContrattiDLottiRow = DsContrattiDettL.ContrattiDLotti.NewContrattiDLottiRow
    '''    ' ''    With newRowDocD
    '''    ' ''        .BeginEdit()
    '''    ' ''        .IDDocumenti = CLng(myID)
    '''    ' ''        .DurataNum = Val(myIDDurataNum)
    '''    ' ''        .DurataNumRiga = Val(myIDDurataNumR)
    '''    ' ''        .Cod_Articolo = txtCodArtIns.Text.Trim
    '''    ' ''        .Riga = RigaDettArt
    '''    ' ''        .NCollo = RigaSel + 1
    '''    ' ''        .Lotto = ""
    '''    ' ''        .NSerie = ""
    '''    ' ''        .Sfusi = 0
    '''    ' ''        .QtaColli = QtaColli
    '''    ' ''        .EndEdit()
    '''    ' ''    End With
    '''    ' ''    DsContrattiDettL.ContrattiDLotti.AddContrattiDLottiRow(newRowDocD)
    '''    ' ''    newRowDocD = Nothing

    '''    ' ''    Me.SqlAdapDocDettL.Update(DsContrattiDettL.ContrattiDLotti)

    '''    ' ''    aDataView1L = New DataView(DsContrattiDettL.ContrattiDLotti)
    '''    ' ''    GridViewDettL.SelectedIndex = GrdSel + 1
    '''    ' ''    GridViewDettL.EditIndex = GrdSel + 1
    '''    ' ''    If aDataView1l.Count > 0 Then aDataView1L.Sort = "NCollo"
    '''    ' ''    GridViewDettL.DataSource = aDataView1L
    '''    ' ''    GridViewDettL.DataBind()
    '''    ' ''    GridViewDettL.Focus()
    '''    ' ''Catch Ex As Exception
    '''    ' ''    Session(SWOPDETTDOCL) = SWOPNESSUNA
    '''    ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '''    ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '''    ' ''    ModalPopup.Show("Errore in ContrattiDett.GridViewDettL.SelectedIndexChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
    '''    ' ''    Exit Sub
    '''    ' ''End Try
    '''    Session(SWOPDETTDOCL) = SWOPMODIFICA
    '''    ' ''Session(SWMODIFICATO) = SWSI
    '''End Sub

    Private Sub btnPrimaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrimaRiga.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        Dim newRowDocD As DSDocumenti.ContrattiDRow = DsContrattiDett.ContrattiD.NewContrattiDRow
        With newRowDocD
            .BeginEdit()
            .IDDocumenti = CLng(myID)
            .DurataNum = Val(myIDDurataNum)
            .DurataNumRiga = Val(myIDDurataNumR)
            .Riga = 1
            .Prezzo = 0
            .Prezzo_Netto = 0
            .Qta_Allestita = 0
            .Qta_Evasa = 0
            .Qta_Impegnata = 0
            .Qta_Ordinata = 0
            .Qta_Prenotata = 0
            .Qta_Residua = 0
            .Importo = 0
            'giu170412
            .PrezzoAcquisto = 0
            .PrezzoListino = 0
            .SWModAgenti = 0
            .PrezzoCosto = 0
            .Qta_Inviata = 0
            .Qta_Fatturata = 0
            '---------
            .EndEdit()
        End With

        Try
            DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
            newRowDocD = Nothing

            SqlAdapDocDett = Session("aSqlAdap")

            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            GridViewDett.DataBind()
            GridViewDett.SelectedIndex = 0
            'Selezionato
            '--041211
            AzzeraTxtInsArticoli()
            'RICARICO I LOTTI PER LA RIGA SELEZIONATA
            '''BuildLottiRigaDB(GridViewDett.SelectedDataKey.Value)
            '-----------
            PopolaTxtDett() 'giu190424
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore ContrattiDett.btnPrimaRiga", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett

        SetBtnPrimaRigaEnabled(False)
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
        ''''giu190424 NON FUNGE 
        '''If DDLTipoDettagli.SelectedIndex = 0 Then
        '''    _WucElement.SetBtnDupGen(True)
        '''    PopolaTxtDett()
        '''Else
        '''    _WucElement.SetBtnDupGen(False)
        '''End If
        '''If chkSelModifica.Checked Or GridViewDett.Rows.Count = 0 Then
        '''    Call _WucElement.CallBtnModifica()
        '''Else
        '''    Dim myRowIndex As Integer = GridViewDett.SelectedIndex + (GridViewDett.PageSize * GridViewDett.PageIndex)
        '''    Dim myCodArt As String = ""
        '''    Try
        '''        myCodArt = aDataView1.Item(myRowIndex).Item("Cod_Articolo")
        '''    Catch ex As Exception
        '''        myCodArt = ""
        '''    End Try
        '''    If myCodArt = "" Then
        '''        Call _WucElement.CallBtnModifica()
        '''        Call _WucElement.SetLblMessDoc("Attenzione: Errore in " & DDLTipoDettagli.SelectedItem.Text.Trim & " - Nessun dettaglio presente.")
        '''    End If
        '''End If
        ''''-NoteIntervento 
        '''If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
        '''    Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
        '''    If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
        '''        txtNoteIntervento.Text = ""
        '''        Exit Sub
        '''    End If
        '''    NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
        '''    txtNoteIntervento.Text = GetNoteSL(txtNoteInterventoALL.Text.Trim, NSL.Trim)
        '''Else
        '''    txtNoteIntervento.Text = ""
        '''End If
    End Sub
#End Region

#Region "Ricerca ARTICOLI: BASE OPZIONE oppure Articoli"

    Protected Sub BtnSelArticolo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSelArticolo.Click
        '<SelectParameters>
        '            <asp:SessionParameter DefaultValue="1" Name="CodLis" SessionField="IDLISTINO" Type="Int32" />
        '            <asp:SessionParameter DefaultValue="C" Name="SortListVenD" SessionField="SortListVenD" Type="String" />
        '            </SelectParameters>   
        'Dim IDLT As String = Session(IDLISTINO)
        'If IsNothing(IDLT) Then IDLT = "1"
        'If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
        'DATI_CARICATI(QU) '
        WFP_Articolo_Seleziona1.WucElement = Me
        Dim strMessErr As String = ""
        If WFP_Articolo_Seleziona1.PopolaGriglia(strMessErr) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", strMessErr, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        WFP_Articolo_Seleziona1.DeselezionaTutti()
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_Seleziona1.Show()
    End Sub

    Public Sub CallBackWFPArticoloSel(Optional ByVal SWDaInsArt As Boolean = False)
        Dim RigaSel As Integer = 0
        Dim GrdSel As Integer = 0
        Dim PgIndex As Integer = 0
        Dim TipoDett As Integer = -1 'giu260623
        'giu260623 spostato qui giu270320 prendo il primo N.SERIE/LOTTO E COD_FILIALE DA RIPORTARE A TUTTE LE RIGHE
        Dim mySerie As String = Formatta.FormattaNomeFile(txtSerie.Text.Trim) 'giu070523
        Dim myLotto As String = Formatta.FormattaNomeFile(txtLotto.Text.Trim) 'giu070523
        Dim myCGDest As String = ""
        Dim myPos As Integer = InStr(lblDestSelDett.Text.Trim, "-")
        If myPos > 0 Then
            myCGDest = Mid(lblDestSelDett.Text.Trim, 1, myPos - 1)
        End If
        If Not IsNumeric(myCGDest) Then
            myCGDest = ""
        End If
        If myCGDest.Trim = "" Or Val(myCGDest) = 0 Then
            myCGDest = ""
        End If
        Dim myDataSc As String = txtDataSc.Text.Trim
        Dim Modello As Integer = DDLModello.SelectedIndex
        'giu231023
        Dim myRespAreaApp As Integer = 0 : Dim myRespVisiteApp As Integer = 0
        Try
            myRespAreaApp = DDLRespAreaApp.SelectedValue
        Catch ex As Exception
            myRespAreaApp = 0
        End Try
        Try
            myRespVisiteApp = DDLRespVisiteApp.SelectedValue
        Catch ex As Exception
            myRespVisiteApp = 0
        End Try
        '---------
        Try
            TipoDett = DDLTipoDettagli.SelectedIndex 'giu260623
            'Se arrivo qui devo aver selezionato la riga
            RigaSel = GridViewDett.SelectedIndex
            GrdSel = GridViewDett.SelectedIndex
            If RigaSel < 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                'Riga da cui inserire tutte gli Articoli selezionati
                RigaSel = GridViewDett.SelectedDataKey.Value
                If RigaSel < 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
            '-
            PgIndex = GridViewDett.PageIndex 'giu011111
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata (CallBackWFPArticoloSel): " & ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '-
        EnableTOTxtInsArticoli(False)
        GridViewDett.EditIndex = -1
        GridViewDett.Enabled = False
        '--- IDDocumenti 
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        'LISTA ARTICOLI DA INSERIRE
        Dim listaCodiciArtSel As List(Of String) = Session(ARTICOLI_DA_INS)
        If (listaCodiciArtSel Is Nothing) Then
            Exit Sub
        End If
        If (listaCodiciArtSel.Count = 0) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun articolo selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'LETTURA DS DALLA SESSIONE
        SqlAdapDocDett = Session("aSqlAdap")
        DsContrattiDett = Session("aDsDett")
        '-------------------------
        'GIU110117
        Try
            'giu020320 QUI COME AGGIUNGERE LA RIGA SOPRA DA FARE
            Dim RowsDel() As DataRow = Me.DsContrattiDett.ContrattiD.Select("Riga>" & RigaSel.ToString.Trim, "Riga")
            Dim RowDel As DSDocumenti.ContrattiDRow
            Dim RigaApp As Integer = 0
            For Each RowDel In RowsDel
                If RowDel.IsRiga_AppartenenzaNull Then
                    Exit For
                ElseIf RowDel.Riga_Appartenenza = 0 Then
                    Exit For
                ElseIf Not RowDel.IsCod_ArticoloNull Then
                    If RowDel.Cod_Articolo.Trim <> "" Then
                        Exit For
                    End If
                ElseIf RowDel.Qta_Evasa <> 0 Then
                    Exit For
                ElseIf RowDel.Qta_Ordinata <> 0 Then
                    Exit For
                ElseIf RowDel.Prezzo <> 0 Then
                    Exit For
                End If
                If RigaApp <> 0 And RowDel.Riga_Appartenenza <> RigaApp Then
                    Exit For
                End If
                RigaApp = RowDel.Riga_Appartenenza
                If RowDel.IsCod_ArticoloNull Then
                    RowDel.Delete()
                ElseIf RowDel.Cod_Articolo.Trim = "" Then
                    RowDel.Delete()
                Else
                    Exit For
                End If
            Next
            RowsDel = Nothing
        Catch ex As Exception
            'NULLA PROSEGUO TANTO NON E' IMPORTANTE
        End Try
        '---------
        Try
            'IMPOSTO A 99999+RIGA TUTTE LE RIGHE
            Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("Riga >=" & RigaSel, "Riga DESC")
            Dim RowD As DSDocumenti.ContrattiDRow
            For Each RowD In RowsD
                If RowD.Riga = RigaSel Then
                    RowD.Delete()
                Else
                    RowD.BeginEdit()
                    RowD.Riga += 99999
                    RowD.EndEdit()
                End If
            Next
            'Def per la Descrizione estesa
            Dim dsArtDes As New DataSet
            Dim RowsArtDes() As DataRow
            Dim strSQL As String = ""
            Dim ObjDB As New DataBaseUtility
            'INIZIO INSERIMENTO ARTICOLI
            Dim iNew As Integer = 1
            Dim SaveRiga As Integer = 0
            Dim newRowDocD As DSDocumenti.ContrattiDRow = DsContrattiDett.ContrattiD.NewContrattiDRow
            'giu230202
            Dim SWNoDesEst As String = Session("ckNoDesArtEst")
            If String.IsNullOrEmpty(SWDaInsArt) Then
                Session("ckNoDesArtEst") = "NO"
            End If
            '-
            For Each CodArt As String In listaCodiciArtSel
                'giu301111 per evitare di inserire articoli gia' presenti nel documento
                ' GESTITO DIRETTAMENTE IN SELEZIONA ARTICOLI E NON QUI, mi arrivato i dati effettivi
                ' ''If Me.DsContrattiDett.ContrattiD.Select("Cod_Articolo = '" & CodArt.Trim & "'").Count = 0 Then
                '----------------------------------------------------------------------
                newRowDocD = DsContrattiDett.ContrattiD.NewContrattiDRow
                With newRowDocD
                    .BeginEdit()
                    .IDDocumenti = CLng(myID)
                    .DurataNum = Val(myIDDurataNum)
                    .DurataNumRiga = Val(myIDDurataNumR)
                    .Riga = RigaSel + iNew
                    .Prezzo = 0
                    .Prezzo_Netto = 0
                    .Qta_Allestita = 0
                    .Qta_Evasa = 0
                    .Qta_Impegnata = 0
                    .Qta_Ordinata = 0
                    .Qta_Prenotata = 0
                    .Qta_Residua = 0
                    .Importo = 0
                    'giu170412
                    .PrezzoAcquisto = 0
                    .PrezzoListino = 0
                    .SWModAgenti = 0
                    .PrezzoCosto = 0
                    .Qta_Inviata = 0
                    .Qta_Fatturata = 0
                    '---------
                    SaveRiga = .Riga 'giu210615 RigaSel + iNew
                    'GIU210615
                    .SetRiga_AppartenenzaNull()
                    '---------
                    .Cod_Articolo = CodArt
                    If GetDatiAnaMag(newRowDocD, CodArt, SWDaInsArt) = False Then
                        Exit For
                    End If
                    'giu260623
                    .Serie = mySerie.Trim
                    .Lotto = myLotto.Trim
                    .QtaDurataNumR0 = Modello
                    If myCGDest.Trim <> "" Then
                        .Cod_Filiale = myCGDest.Trim
                    End If
                    If myRespAreaApp > 0 Then
                        .RespArea = myRespAreaApp
                    End If
                    If myRespVisiteApp > 0 Then
                        .RespVisite = myRespVisiteApp
                    End If
                    If .IsDataScNull Then
                        If IsDate(myDataSc.Trim) Then
                            .DataSc = CDate(myDataSc.Trim)
                        End If
                    ElseIf Not IsDate(.DataSc) Then
                        If IsDate(myDataSc.Trim) Then
                            .DataSc = CDate(myDataSc.Trim)
                        End If
                    End If
                    '---------
                    .EndEdit()
                End With
                iNew += 1
                DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                newRowDocD = Nothing
                If SWNoDesEst <> "SI" Then
                    '------------------------------------
                    'Descrizione ESTESA AnaMagDes
                    '------------------------------------
                    strSQL = "Select * From AnaMagDes WHERE Cod_Articolo = '" & CodArt.Trim & "'"
                    dsArtDes.Clear()
                    Try
                        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArtDes)
                        If (dsArtDes.Tables.Count > 0) Then
                            If (dsArtDes.Tables(0).Rows.Count > 0) Then
                                RowsArtDes = dsArtDes.Tables(0).Select("", "Progressivo")
                                'OK DESCRIZIONE ESTESA DELL'ARTICOLO
                                For Each RowArtDes In RowsArtDes
                                    newRowDocD = DsContrattiDett.ContrattiD.NewContrattiDRow
                                    With newRowDocD
                                        .BeginEdit()
                                        .IDDocumenti = CLng(myID)
                                        .DurataNum = Val(myIDDurataNum)
                                        .DurataNumRiga = Val(myIDDurataNumR)
                                        .Prezzo = 0
                                        .Prezzo_Netto = 0
                                        .Qta_Allestita = 0
                                        .Qta_Evasa = 0
                                        .Qta_Impegnata = 0
                                        .Qta_Ordinata = 0
                                        .Qta_Prenotata = 0
                                        .Qta_Residua = 0
                                        .Importo = 0
                                        'giu170412
                                        .PrezzoAcquisto = 0
                                        .PrezzoListino = 0
                                        .SWModAgenti = 0
                                        .PrezzoCosto = 0
                                        .Qta_Inviata = 0
                                        .Qta_Fatturata = 0
                                        '---------
                                        .Descrizione = IIf(IsDBNull(RowArtDes.Item("Descrizione")), "", Mid(RowArtDes.Item("Descrizione"), 1, 150))
                                        .Riga_Appartenenza = SaveRiga 'Salvata sopra nell'insert articolo
                                        .Riga = RigaSel + iNew
                                        '---------
                                        'giu260623
                                        .Serie = mySerie.Trim
                                        .Lotto = myLotto.Trim
                                        .QtaDurataNumR0 = Modello
                                        If myCGDest.Trim <> "" Then
                                            .Cod_Filiale = myCGDest.Trim
                                        End If
                                        If myRespAreaApp > 0 Then
                                            .RespArea = myRespAreaApp
                                        End If
                                        If myRespVisiteApp > 0 Then
                                            .RespVisite = myRespVisiteApp
                                        End If
                                        If .IsDataScNull Then
                                            If IsDate(myDataSc.Trim) Then
                                                .DataSc = CDate(myDataSc.Trim)
                                            End If
                                        ElseIf Not IsDate(.DataSc) Then
                                            If IsDate(myDataSc.Trim) Then
                                                .DataSc = CDate(myDataSc.Trim)
                                            End If
                                        End If
                                        '---------
                                        .EndEdit()
                                    End With
                                    DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                                    newRowDocD = Nothing
                                    iNew += 1
                                Next
                            Else
                                'nessuna descrizione estesa
                            End If
                        Else
                            'nessuna descrizione estesa
                        End If
                    Catch Ex As Exception
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore in ContrattiDett.CallBackWFPArticoloSel", "Lettura Descrizione estesa: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                        Exit For
                    End Try
                End If
                '------------------------------------
                ' GESTITO DIRETTAMENTE IN SELEZIONA ARTICOLI E NON QUI, mi arrivato i dati effettivi
                ' ''End If 'giu301111 per evitare di inserire articoli gia' presenti nel documento
                '----------------------------------------------------------------------
            Next
            RowsD = Nothing
            'RINUMERO TUTTO
            'GIU210615
            Dim RowDRA As DSDocumenti.ContrattiDRow
            Dim RowsDRA() As DataRow
            '---------
            Dim rowCADettS() As DataRow = Nothing
            If Me.DsContrattiDett.ContrattiD.Select("Serie<>'' AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettS = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettS = Me.DsContrattiDett.ContrattiD.Select("Serie<>''", "Riga")
            End If
            Dim rowCADettF() As DataRow = Nothing
            If Me.DsContrattiDett.ContrattiD.Select("Cod_Filiale>0 AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettF = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettF = Me.DsContrattiDett.ContrattiD.Select("Cod_Filiale>0", "Riga")
            End If
            'giu231023
            Dim rowCADettRAV() As DataRow = Nothing
            If Me.DsContrattiDett.ContrattiD.Select("RespArea>0 AND Riga=" & RigaSel.ToString.Trim).Count > 0 Then
                rowCADettRAV = Me.DsContrattiDett.ContrattiD.Select("Riga=" & RigaSel.ToString.Trim, "Riga")
            Else
                rowCADettRAV = Me.DsContrattiDett.ContrattiD.Select("RespArea>0", "Riga")
            End If
            '--------

            Dim RowsR = Me.DsContrattiDett.ContrattiD.Select("", "Riga")
            iNew = 1
            For Each RowD In RowsR
                If RowD.Riga <> iNew Then
                    'giu210615
                    SaveRiga = RowD.Riga
                    '---------
                    RowD.BeginEdit()
                    RowD.Riga = iNew
                    If TipoDett = 0 Then 'giu260623
                        If Not (rowCADettS Is Nothing) Then
                            If rowCADettS.Count > 0 Then
                                If IsDBNull(rowCADettS(0).Item("Serie")) Then
                                    RowD.Serie = ""
                                Else
                                    RowD.Serie = rowCADettS(0).Item("Serie")
                                End If
                                If IsDBNull(rowCADettS(0).Item("Lotto")) Then
                                    RowD.Lotto = ""
                                Else
                                    RowD.Lotto = rowCADettS(0).Item("Lotto")
                                End If
                                If IsDate(myDataSc.Trim) Then
                                    RowD.DataSc = CDate(myDataSc.Trim)
                                End If
                                If IsDBNull(rowCADettS(0).Item("QtaDurataNumR0")) Then
                                    RowD.QtaDurataNumR0 = 0
                                Else
                                    RowD.QtaDurataNumR0 = rowCADettS(0).Item("QtaDurataNumR0")
                                End If
                            End If
                            If mySerie <> "" Then
                                RowD.Serie = mySerie
                            End If
                            If myLotto <> "" Then
                                RowD.Lotto = myLotto
                            End If
                            RowD.QtaDurataNumR0 = Modello
                            '-
                            If RowD.IsDataScNull Then
                                If IsDate(myDataSc.Trim) Then
                                    RowD.DataSc = CDate(myDataSc.Trim)
                                End If
                            ElseIf Not IsDate(RowD.DataSc) Then
                                If IsDate(myDataSc.Trim) Then
                                    RowD.DataSc = CDate(myDataSc.Trim)
                                End If
                            End If
                        Else
                            If mySerie <> "" Then
                                RowD.Serie = mySerie
                            End If
                            If myLotto <> "" Then
                                RowD.Lotto = myLotto
                            End If
                            RowD.QtaDurataNumR0 = Modello
                            '-
                            If RowD.IsDataScNull Then
                                If IsDate(myDataSc.Trim) Then
                                    RowD.DataSc = CDate(myDataSc.Trim)
                                End If
                            ElseIf Not IsDate(RowD.DataSc) Then
                                If IsDate(myDataSc.Trim) Then
                                    RowD.DataSc = CDate(myDataSc.Trim)
                                End If
                            End If
                        End If
                        If Not (rowCADettF Is Nothing) Then
                            If rowCADettF.Count > 0 Then
                                If Not IsDBNull(rowCADettF(0).Item("Cod_Filiale")) Then
                                    RowD.Cod_Filiale = rowCADettF(0).Item("Cod_Filiale")
                                End If
                                If myCGDest <> "" Then
                                    RowD.Cod_Filiale = myCGDest
                                End If
                            End If
                        Else
                            If myCGDest <> "" Then
                                RowD.Cod_Filiale = myCGDest
                            End If
                        End If
                        'giu231023
                        If Not (rowCADettRAV Is Nothing) Then
                            If rowCADettRAV.Count > 0 Then
                                If Not IsDBNull(rowCADettRAV(0).Item("RespArea")) Then
                                    RowD.RespArea = rowCADettRAV(0).Item("RespArea")
                                End If
                                If myRespAreaApp > 0 Then
                                    RowD.RespArea = myRespAreaApp
                                End If
                                '-
                                If Not IsDBNull(rowCADettRAV(0).Item("RespVisite")) Then
                                    RowD.RespVisite = rowCADettRAV(0).Item("RespVisite")
                                End If
                                If myRespVisiteApp > 0 Then
                                    RowD.RespVisite = myRespVisiteApp
                                End If
                            End If
                        Else
                            If myRespAreaApp > 0 Then
                                RowD.RespArea = myRespAreaApp
                            End If
                            If myRespVisiteApp > 0 Then
                                RowD.RespVisite = myRespVisiteApp
                            End If
                        End If
                    Else
                        'GIU260623 ERR. 
                        '''RowD.Serie = mySerie.Trim
                        '''RowD.Lotto = myLotto.Trim
                        '''RowD.QtaDurataNumR0 = Modello
                        '''If myCGDest.Trim <> "" Then
                        '''    RowD.Cod_Filiale = myCGDest.Trim
                        '''End If
                        '''If RowD.IsDataScNull Then
                        '''    If IsDate(myDataSc.Trim) Then
                        '''        RowD.DataSc = CDate(myDataSc.Trim)
                        '''    End If
                        '''ElseIf Not IsDate(RowD.DataSc) Then
                        '''    If IsDate(myDataSc.Trim) Then
                        '''        RowD.DataSc = CDate(myDataSc.Trim)
                        '''    End If
                        '''End If
                    End If
                    RowD.EndEdit()
                    'GIU210615
                    RowsDRA = Me.DsContrattiDett.ContrattiD.Select("Riga_Appartenenza=" & SaveRiga.ToString.Trim)
                    For Each RowDRA In RowsDRA
                        RowDRA.BeginEdit()
                        RowDRA.Riga_Appartenenza = iNew
                        RowDRA.EndEdit()
                    Next
                    '---------
                End If
                iNew += 1
            Next
        Catch ex As Exception
            'NULLA DI FATTO 
            DsContrattiDett = Session("aDsDett")
            GridViewDett.Enabled = True
            '---------------
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.CallBackWFPArticoloSel", "Inserimento articoli selezionati: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        'AGGIORNO IL GRID E MI SELEZIONE NUOVAMENTE LA RIGA DA CUI HO INSERITO
        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
        GridViewDett.DataSource = aDataView1
        'giu011111 salvato all'inizio la pageindex e riposizionati
        If GridViewDett.AllowPaging = True Then
            GridViewDett.PageIndex = PgIndex
        End If

        GridViewDett.DataBind()
        '----------------------------------------
        GridViewDett.SelectedIndex = GrdSel
        Try
            RigaSel = GridViewDett.SelectedDataKey.Value
            PopolaTxtDett()
        Catch ex As Exception
            RigaSel = 0
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
            Call AzzeraTxtInsArticoli() 'giu021211
        End Try
        'AGGIORNO DS NELLA SESSIONE
        Dim strErrore As String = ""
        If AggiornaImporto(DsContrattiDett, strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '-
        'Riattivo il GRID
        GridViewDett.Enabled = True
        If TipoDett = 0 Then
            _WucElement.btnbtnGeneraAttDNumColorRED(True)
        End If
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Function GetDatiAnaMag(ByRef RowD As DataRow, ByVal CodArt As String, Optional ByVal SWDaInsArt As Boolean = False) As Boolean
        If CodArt.Trim = "" Then Exit Function

        GetDatiAnaMag = False 'giu180419

        '=============================================================================
        'GIU170412 ATTENZIONE SE SI MODIFICA QUI VERIFICARE LA CORRISPONDENTE FUNZIONE
        'IN Documenti.GetPrezziListinoAcquisto
        '=============================================================================
        'giu160412 myTabCliFor
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        'giu170412 dal codice causale determino quale prezzo è il prezzo di Listino (Acquisto o Vendita)
        'C/DEPOSITO
        SWPrezzoAL = "L" 'Listino
        Dim strErrore As String = ""
        If _WucElement.CKPrezzoALCSG(SWPrezzoAL, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GetDatiAnaMag.CKPrezzoALCSG", strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If

        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        Dim rowArt() As DataRow
        'LISTINO 'GIU170412 PER USARE UN'UNICA FUNZIONE PER I PREZZI ETC 
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = "1"
        If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
        If IDLT = "" Then IDLT = "1"
        'Agente
        Dim IDCAge As String = Session(IDAGENTE)
        If IsNothing(IDCAge) Then IDCAge = "0"
        If String.IsNullOrEmpty(IDCAge) Then IDCAge = "0"
        If IDCAge = "" Then IDCAge = "0"
        'Regime IVA
        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        'GIU170412 NUOVA FUNZIONE IN DOCUMENTI
        'giu091211 per i documenti OF e DF
        Dim _myPrezzoListino As Decimal = 0 : Dim _myPrezzoAcquisto As Decimal = 0
        Dim _mySconto1 As Decimal = 0 : Dim _mySconto2 As Decimal = 0
        'giu070115 gestione sconto del fornitore (AnaMag) mentre lo sconto 2 è sempre = ZERO
        Dim _myScFornitore As Decimal = 0
        '----------------------------------------------------------------------------
        '------------------------------------
        'AnaMag
        '------------------------------------
        strSQL = "Select * From AnaMag WHERE Cod_Articolo = '" & CodArt.Trim & "'"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    rowArt = dsArt.Tables(0).Select()
                    RowD.Item("Descrizione") = IIf(IsDBNull(rowArt(0).Item("Descrizione")), "", rowArt(0).Item("Descrizione"))
                    _myPrezzoAcquisto = IIf(IsDBNull(rowArt(0).Item("PrezzoAcquisto")), 0, rowArt(0).Item("PrezzoAcquisto"))
                    _myScFornitore = IIf(IsDBNull(rowArt(0).Item("ScFornitore")), 0, rowArt(0).Item("ScFornitore"))
                    RowD.Item("UM") = IIf(IsDBNull(rowArt(0).Item("UM")), DBNull.Value, rowArt(0).Item("UM"))
                    RowD.Item("LBase") = IIf(IsDBNull(rowArt(0).Item("LBase")), 20, rowArt(0).Item("LBase"))
                    RowD.Item("LOpz") = IIf(IsDBNull(rowArt(0).Item("LOpz")), 0, rowArt(0).Item("LOpz"))
                    'muovo fisso 1 nella quantita
                    If SWDaInsArt = False Then 'giu061211
                        RowD.Item("Qta_Ordinata") = 1
                    Else
                        Try
                            If Not IsNumeric(txtQtaIns.Text.Trim) Then
                                txtQtaIns.Text = "1"
                                RowD.Item("Qta_Ordinata") = 1
                            Else
                                RowD.Item("Qta_Ordinata") = CDec(txtQtaIns.Text.Trim)
                            End If
                        Catch ex As Exception
                            RowD.Item("Qta_Ordinata") = 1
                        End Try
                    End If
                    Select Case Left(myTipoDoc, 1)
                        Case "O"
                            RowD.Item("Qta_Evasa") = 0
                            RowD.Item("Qta_Residua") = RowD.Item("Qta_Ordinata") 'giu061211 (1)
                        Case Else
                            If myTipoDoc = "PR" Or myTipoDoc = "PF" Or myTipoDoc = "TC" Or myTipoDoc = "CA" Then 'GIU020212 GIU021219
                                RowD.Item("Qta_Evasa") = 0
                                RowD.Item("Qta_Residua") = RowD.Item("Qta_Ordinata") 'giu061211 (1)
                            Else
                                RowD.Item("Qta_Evasa") = RowD.Item("Qta_Ordinata") 'giu061211 (1)
                                RowD.Item("Qta_Residua") = 0
                            End If
                    End Select
                    If CInt(RegimeIVA) > 49 Then
                        RowD.Item("Cod_Iva") = CInt(RegimeIVA)
                    Else
                        RowD.Item("Cod_Iva") = IIf(IsDBNull(rowArt(0).Item("Cod_Iva")), DBNull.Value, rowArt(0).Item("Cod_Iva"))
                    End If
                    RowD.Item("Cod_Agente") = CDbl(IDCAge)
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GetDatiAnaMag", "Lettura articoli: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '================================================================================================
        'giu170412 nuova gestione per i prezzi 
        '================================================================================================
        If Documenti.GetPrezziListVenD(myTipoDoc, IDLT, CodArt, _
                                    _myPrezzoListino, _mySconto2, _mySconto2, _
                                    strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GetDatiAnaMag", strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'GIU170412
        If SWPrezzoAL = "A" Then 'GIU170412
            RowD.Item("Prezzo") = _myPrezzoAcquisto
            RowD.Item("Sconto_1") = _myScFornitore 'GIU070115 0
            RowD.Item("Sconto_2") = 0
            RowD.Item("Prezzo_Netto") = _myPrezzoAcquisto
            RowD.Item("ScontoValore") = 0
            If SWDaInsArt = True Then 'giu130112
                Try
                    If IsNumeric(txtPrezzoIns.Text.Trim) Then
                        If CDec(txtPrezzoIns.Text.Trim) <> 0 Then
                            RowD.Item("Prezzo") = CDec(txtPrezzoIns.Text.Trim)
                            RowD.Item("Prezzo_Netto") = RowD.Item("Prezzo")
                        End If
                    End If
                Catch ex As Exception
                    RowD.Item("Prezzo") = 0
                    RowD.Item("Prezzo_Netto") = 0
                End Try
                '-
                Try
                    If IsNumeric(txtSconto1Ins.Text.Trim) Then
                        If CDec(txtSconto1Ins.Text.Trim) <> 0 Then
                            RowD.Item("Sconto_1") = CDec(txtSconto1Ins.Text.Trim)
                        End If
                    End If
                Catch ex As Exception
                    RowD.Item("Sconto_1") = 0
                End Try
                RowD.Item("ScontoValore") = 0
            End If
        Else
            If chkNoPrezzo.Checked = False Then
                RowD.Item("Prezzo") = _myPrezzoListino
                lblPrezzoAL.ToolTip = _myPrezzoListino
            Else
                RowD.Item("Prezzo") = 0
                lblPrezzoAL.ToolTip = _myPrezzoListino
            End If

            RowD.Item("Sconto_1") = _mySconto1
            RowD.Item("Sconto_2") = 0 'GIU070115 _mySconto2
            RowD.Item("Prezzo_Netto") = _myPrezzoListino
            RowD.Item("ScontoValore") = 0
            '---------
            If SWDaInsArt = True Then 'giu061211
                Try
                    If IsNumeric(txtPrezzoIns.Text.Trim) Then
                        If CDec(txtPrezzoIns.Text.Trim) <> 0 Then
                            RowD.Item("Prezzo") = CDec(txtPrezzoIns.Text.Trim)
                            RowD.Item("Prezzo_Netto") = RowD.Item("Prezzo")
                        End If
                    End If
                Catch ex As Exception
                    RowD.Item("Prezzo") = 0 'GIU130112
                    RowD.Item("Prezzo_Netto") = 0 'GIU130112
                End Try
                '-
                Try
                    If IsNumeric(txtSconto1Ins.Text.Trim) Then
                        If CDec(txtSconto1Ins.Text.Trim) <> 0 Then
                            RowD.Item("Sconto_1") = CDec(txtSconto1Ins.Text.Trim)
                        End If
                    End If
                Catch ex As Exception
                    RowD.Item("Sconto_1") = 0 'GIU130112
                End Try
            End If
        End If
        'giu020519
        RowD.Item("DedPerAcconto") = False
        '---------
        RowD.Item("PrezzoAcquisto") = _myPrezzoAcquisto
        RowD.Item("PrezzoListino") = _myPrezzoListino
        RowD.Item("SWModAgenti") = False
        RowD.Item("PrezzoCosto") = 0
        RowD.Item("Qta_Inviata") = 0
        RowD.Item("Qta_Fatturata") = 0
        '================================================================================================
        '-------------------------------------------------------------------------------------
        'CALCOLA IMPORTO
        '-------------------------------------------------------------------------------------
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        '---- Calcolo sconto su 
        Dim ScontoSuImporto As Boolean = True
        Dim ScCassaDett As Boolean = False
        Try
            ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        '-------------------------------------------------------------------------------------
        '------------------------------------
        RowD.Item("Sconto_3") = 0
        RowD.Item("Sconto_4") = 0
        RowD.Item("Sconto_Pag") = 0
        RowD.Item("Importo") = 0
        '---
        'giu010119
        WucElement.GetDatiTB3()

        Dim ScCassa As String = Session(CSTSCCASSA)
        If IsNothing(ScCassa) Then ScCassa = "0"
        If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
        If Not IsNumeric(ScCassa) Then ScCassa = "0"
        '---------
        RowD.Item("Importo") = _
        CalcolaImporto(RowD.Item("Prezzo"), RowD.Item("Qta_Ordinata"), _
               RowD.Item("Sconto_1"), _
               RowD.Item("Sconto_2"), _
               RowD.Item("Sconto_3"), _
               RowD.Item("Sconto_4"), _
               RowD.Item("Sconto_Pag"), _
               RowD.Item("ScontoValore"), _
               RowD.Item("Importo"), _
               ScontoSuImporto, _
               CInt(DecimaliVal), _
               RowD.Item("Prezzo_Netto"), ScCassaDett, CDec(ScCassa), RowD.Item("DedPerAcconto")) 'giu010119) 'giu281211 correzione ATTENZIONE IL PREZZO_NETTO E' BYREF

        GetDatiAnaMag = True
    End Function

    Private Sub btnAggArtGridSel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggArtGridSel.Click
        Dim myPos As Integer = 0
        Dim myCGDest As String = ""
        Dim myRespAreapApp As Integer = -1 : Dim myRespVisiteApp As Integer = -1
        Passo = 1
        If txtCodArtIns.BackColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice articolo non valido, impossibile l'aggiornamento riga.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If lblDestSelDett.BackColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Luogo App. non valido, impossibile l'aggiornamento riga.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        lblMessAgg.ForeColor = Drawing.Color.Blue
        Passo = 2
        SWPrezzoAL = "L" 'Listino
        Dim strErroreAggRiga As String = ""
        If _WucElement.CKPrezzoALCSG(SWPrezzoAL, strErroreAggRiga) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in AggiornaRiga.CKPrezzoALCSG", strErroreAggRiga, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu190412
        If SWPrezzoAL = "A" Then
            lblPrezzoAL.Text = "Prezzo acquisto"
            lblPrezzoAL.ForeColor = Drawing.Color.Blue
        Else
            lblPrezzoAL.ForeColor = Drawing.Color.Black
            lblPrezzoAL.Text = "Prezzo listino"
        End If
        '----------
        '--
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = "1"
        If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
        '--
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        'QUI TEST SERIE LOTTO UNIVOCO PER APP.
        If DDLTipoDettagli.SelectedIndex = 0 Then
            If CHECKSerieLotto(myID, myIDDurataNumR, Formatta.FormattaNomeFile(txtSerie.Text.Trim), Formatta.FormattaNomeFile(txtLotto.Text.Trim)) = True Then 'giu070523
                ModalPopup.Show("Attenzione, N° Serie/Lotto già utilizzato.", "Controllo esistenza N° Serie/Lotto", WUC_ModalPopup.TYPE_ALERT)
                '''Session(CSTNONCOMPLETO) = SWSI
                Exit Sub
            Else
                txtSerie.BackColor = SEGNALA_OK
            End If
            'giu231023
            If IsNumeric(DDLRespAreaApp.SelectedValue) And IsNumeric(DDLRespVisiteApp.SelectedValue) Then
                If DDLRespAreaApp.SelectedValue > 0 Then
                    If DDLRespVisiteApp.SelectedValue < 1 Then
                        ModalPopup.Show("Attenzione, Resp.Visita Obbligatorio.", "Controllo Responsabili Area/Visite", WUC_ModalPopup.TYPE_ALERT)
                        '''Session(CSTNONCOMPLETO) = SWSI
                        Exit Sub
                    End If
                Else
                    If DDLRespVisiteApp.SelectedValue > 0 Then
                        ModalPopup.Show("Attenzione, Resp.Area Obbligatorio.", "Controllo Responsabili Area/Visite", WUC_ModalPopup.TYPE_ALERT)
                        '''Session(CSTNONCOMPLETO) = SWSI
                        Exit Sub
                    End If
                End If
            ElseIf Not IsNumeric(DDLRespAreaApp.SelectedValue) And Not IsNumeric(DDLRespVisiteApp.SelectedValue) Then
                'ok nulla prenderà la testata
            Else
                ModalPopup.Show("Attenzione, Resp.Area/Visita Obbligatori.", "Controllo Responsabili Area/Visite", WUC_ModalPopup.TYPE_ALERT)
                '''Session(CSTNONCOMPLETO) = SWSI
                Exit Sub
            End If
            '---------
        End If
        'giu051211 Posizionato dove ?
        Passo = 3
        Dim i As Integer = -1 : Dim myRowIndex As Integer = -1
        Try
            i = GridViewDett.SelectedIndex
            If i < 0 Then
                ModalPopup.Show("Errore in btnAggArtGridSel: Posizionamento riga. passo: " & Passo.ToString, "Nessuna riga selezionata.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            myRowIndex = i + (GridViewDett.PageSize * GridViewDett.PageIndex)
            aDataView1 = Session("aDataView1")
        Catch ex As Exception
            ModalPopup.Show("Errore in btnAggArtGridSel: Posizionamento riga. passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '------------------------------
        Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0
        Dim myQuantita As Decimal = 0 : Dim SWQta = ""
        Dim myIVA As Integer = 0
        Dim myPrezzo As Decimal = 0
        Passo = 4
        '--------------------------------------------------------------------------------
        If Not IsNumeric(txtQtaIns.Text.Trim) Then txtQtaIns.Text = "0"
        If Not IsNumeric(lblQtaEv.Text.Trim) Then lblQtaEv.Text = "0"
        txtIVAIns.AutoPostBack = False
        If Not IsNumeric(txtIVAIns.Text.Trim) Then txtIVAIns.Text = "0"
        txtIVAIns.AutoPostBack = True
        If Not IsNumeric(txtPrezzoIns.Text.Trim) Then txtPrezzoIns.Text = "0"
        myQtaO = CDec(txtQtaIns.Text.Trim)
        myQtaE = CDec(lblQtaEv.Text.Trim)
        '-----------------------------------------------------------------
        'GIU150715 TEST VALIDITA' CAMPO E PIU AVANTI NON LI PRENDOPIU DALLA TEXT
        Dim Comodo As String
        myIVA = CLng(txtIVAIns.Text.Trim)
        'giu151017
        'giu151017 aggiornamento aliquota IVA
        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        Session(CSTREGIMEIVA) = RegimeIVA
        If Val(RegimeIVA) > 49 Then
            myIVA = Val(RegimeIVA)
            txtIVAIns.AutoPostBack = False
            txtIVAIns.Text = myIVA
            txtIVAIns.AutoPostBack = True
        End If
        '---------
        myPrezzo = CDec(txtPrezzoIns.Text.Trim)
        '--- TEST 
        If myQtaE > myQtaO Then
            myQtaR = 0
        Else
            myQtaR = myQtaE - myQtaO
            If myQtaR < 0 Then myQtaR = myQtaR * -1
        End If
        LblQtaRe.Text = FormattaNumero(myQtaR, -1)
        myQuantita = myQtaO
        SWQta = "O"
        'giu071211 richiesta di Cinzia 
        Dim myErrore As String = ""
        If txtCodArtIns.Text.Trim <> "" Or myPrezzo <> 0 Then
            If myQuantita = 0 Then myErrore += " Quantità ordinata obbligatoria"
            If myIVA = 0 Then myErrore += " IVA obbligatoria"
            If myErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If myPrezzo = 0 And myQuantita <> 0 Then
            If myIVA = 0 Then myErrore += " IVA obbligatoria"
            If myPrezzo = 0 Then
                lblMessAgg.Text = "Attenzione, Prezzo a ZERO"
                lblMessAgg.BorderStyle = BorderStyle.Outset
            End If
        End If
        'Richiesta di Cinzia (Tel. 291211) la qta' evasa mai superiore alla richiesta
        If myQtaE > myQtaO Then
            myErrore += " La quantità Evasa non può essere superiore alla quantità Ordinata"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '
        If myQtaE > myQtaO Then
            myErrore += " La quantità Evasa non può essere superiore alla quantità Ordinata"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu200112
        If txtIVAIns.BackColor = Def.SEGNALA_KO Then
            myErrore += " IVA errata"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------------------------------------------------------
        '--------------- @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim myCodArt As String = txtCodArtIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.CodArt).Controls(0), TextBox).Text
        'giu110112 giu140412 sconto valore GESTIONE - PER IL CALCOLO PROVVIGIONE AGENTE
        Dim _myPrezzoListino As Decimal = 0 : Dim _myPrezzoAcquisto As Decimal = 0
        Dim _mySconto1 As Decimal = 0 : Dim _mySconto2 As Decimal = 0
        Dim _Cod_Iva As Integer = 0
        Dim myPrezzoAL As Decimal = 0
        Dim myErrGetPrezziLA As String = ""
        'GIU230412 PER LE VOCI VENDUTE SENZA CODICE 
        Dim SWPrezzoCosto As Boolean = txtPrezzoCosto.Enabled 'giu230412
        '------------------------------------------------------------------------------
        Passo = 5
        Dim myTipoArticolo As Integer = 0
        If myCodArt.Trim <> "" Then
            '--------------------------------------------------------------------
            'giu110112 giu140412 sconto valore GESTIONE - PER IL CALCOLO PROVVIGIONE AGENTE
            Passo = 6
            Dim myErr As Boolean = False 'giu080319
            If Documenti.GetPrezziListinoAcquisto(myTipoDoc, IDLT, myCodArt, _
                 _myPrezzoListino, _myPrezzoAcquisto, _mySconto1, _mySconto2, _Cod_Iva, myErrGetPrezziLA, myTipoArticolo) = False Then
                myErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrGetPrezziLA, WUC_ModalPopup.TYPE_ALERT)
                ' ''Exit Sub
            ElseIf myErrGetPrezziLA.Trim <> "" Then
                myErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrGetPrezziLA, WUC_ModalPopup.TYPE_ALERT)
                ' ''Exit Sub
            End If
            'GIU080319 NEL CASO IN CUI NON SIA PRESENTE NEL LISTINO MUOVO I DATI MEMORIZZATI 
            If myErr = True Then
                Dim myrow As GridViewRow = GridViewDett.Rows(i)
                aDataView1 = Session("aDataView1")
                Comodo = aDataView1.Item(myRowIndex).Item("PrezzoAcquisto")
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                _myPrezzoAcquisto = CDec(Comodo.Trim)
                Comodo = aDataView1.Item(myRowIndex).Item("PrezzoListino")
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                _myPrezzoListino = CDec(Comodo.Trim)
            End If
            '-------------------------------------------------------------------------------
            'GIU170412
            If SWPrezzoAL = "A" Then 'GIU170412
                myPrezzoAL = _myPrezzoAcquisto
                _mySconto1 = 0 : _mySconto2 = 0
            Else
                myPrezzoAL = _myPrezzoListino
            End If
            '-----------
            If myPrezzo > myPrezzoAL Then
                lblMessAgg.BorderStyle = BorderStyle.Outset
                lblMessAgg.Text = "Attenzione, Prezzo maggiore del Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                'giu190412 myPrezzoAL = myPrezzo =========
            ElseIf myPrezzo <> myPrezzoAL Then
                lblMessAgg.BorderStyle = BorderStyle.Outset
                lblMessAgg.Text = "Attenzione, Prezzo MINORE del Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            ElseIf myPrezzo = myPrezzoAL Then
                lblMessAgg.BorderStyle = BorderStyle.None
                lblMessAgg.Text = ""
                If myPrezzo = 0 Then
                    lblMessAgg.BorderStyle = BorderStyle.Outset
                    lblMessAgg.Text = "Attenzione, Prezzo a ZERO - Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                End If
            End If
        End If

        'giu300312 
        Dim strErroreAggAgente As String = ""
        Dim myProvvAg As Decimal = 0
        Dim MySuperatoSconto As Boolean = False
        '---------
        Passo = 7
        Try
            'Valuta per i decimali per il calcolo 
            Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliVal) Then DecimaliVal = ""
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
            '------------------------------------
            '---- Calcolo sconto su 
            Passo = 8
            '---- Calcolo sconto su 
            Dim ScontoSuImporto As Boolean = True
            Dim ScCassaDett As Boolean = False 'giu010119
            Try
                ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
                ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
            Catch ex As Exception
                ScontoSuImporto = True
                ScCassaDett = False
            End Try
            'giu010119
            Passo = 9
            WucElement.GetDatiTB3()
            Passo = 10
            Dim ScCassa As String = Session(CSTSCCASSA)
            If IsNothing(ScCassa) Then ScCassa = "0"
            If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
            If Not IsNumeric(ScCassa) Then ScCassa = "0"
            '------------------------------------
            'Agente messo istruzione ma non usato qui per il momento
            Dim IDCAge As String = Session(IDAGENTE)
            If IsNothing(IDCAge) Then IDCAge = "0"
            If String.IsNullOrEmpty(IDCAge) Then IDCAge = "0"
            If IDCAge = "" Or Not IsNumeric(IDCAge) Then IDCAge = "0"
            '------------------------------------
            SWGeneraPeriodi = False 'GIU090222
            Passo = 11
            Try
                Dim row As GridViewRow = GridViewDett.Rows(i)
                aDataView1 = Session("aDataView1")
                'Selezionato

                'AL MOMENTO SONO TUTTI UGUALI POI SI DIFFERENZIANO
                aDataView1.Item(myRowIndex).Item("IDDocumenti") = CLng(myID)
                aDataView1.Item(myRowIndex).Item("DurataNum") = Val(myIDDurataNum)
                aDataView1.Item(myRowIndex).Item("DurataNumRiga") = Val(myIDDurataNumR)
                '---
                'GIU090222
                Comodo = IIf(IsDBNull(aDataView1.Item(myRowIndex).Item("Cod_Articolo")), "", aDataView1.Item(myRowIndex).Item("Cod_Articolo").ToString.Trim)
                Try
                    If Comodo <> txtCodArtIns.Text.Trim Then
                        SWGeneraPeriodi = True
                    End If
                Catch ex As Exception
                End Try
                '-
                aDataView1.Item(myRowIndex).Item("Cod_Articolo") = txtCodArtIns.Text.Trim
                aDataView1.Item(myRowIndex).Item("Descrizione") = txtDesArtIns.Text.Trim

                aDataView1.Item(myRowIndex).Item("Um") = txtUMIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.UM).Controls(0), TextBox).Text

                'GIU090222
                Comodo = IIf(IsDBNull(aDataView1.Item(myRowIndex).Item("Qta_Ordinata")), 0, aDataView1.Item(myRowIndex).Item("Qta_Ordinata"))
                Try
                    If CDec(Comodo) <> CDec(IIf(Not IsNumeric(txtQtaIns.Text.Trim), 0, txtQtaIns.Text.Trim)) Then
                        SWGeneraPeriodi = True
                    End If
                Catch ex As Exception
                End Try
                '-
                Comodo = txtQtaIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.Qta).Controls(0), TextBox).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Ordinata") = CDec(Comodo.Trim)
                '-
                Comodo = lblQtaEv.Text.Trim 'giu051211 CType(row.Cells(CellIdx.QtaEv).Controls(0), TextBox).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Evasa") = CDec(Comodo.Trim)

                aDataView1.Item(myRowIndex).Item("Qta_Inviata") = 0

                Comodo = LblQtaRe.Text.Trim 'giu051211 CType(row.Cells(CellIdx.QtaRe).Controls(0), Label).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Residua") = CDec(Comodo.Trim)
                'GIU110120 QTA' FATTURATA
                If chkFatturata.Checked = False Then
                    lblQtaFa.Text = "0"
                Else
                    lblQtaFa.Text = txtQtaIns.Text.Trim
                End If
                Comodo = lblQtaFa.Text.Trim
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Fatturata") = CDec(Comodo.Trim)
                
                aDataView1.Item(myRowIndex).Item("Cod_Iva") = myIVA
                '---------------------------
                If checkNoScontoValore.Checked = False Then
                    aDataView1.Item(myRowIndex).Item("Prezzo") = IIf(myPrezzoAL = 0, myPrezzo, myPrezzoAL) 'giu190412 myPrezzoAL
                    If aDataView1.Item(myRowIndex).Item("Prezzo") < 0 Then 'giu260419
                        aDataView1.Item(myRowIndex).Item("Prezzo") = 0
                    End If
                Else
                    aDataView1.Item(myRowIndex).Item("Prezzo") = myPrezzo 'giu190412 CDec(Comodo.Trim)
                End If
                '----------------------------
                'GIU090222
                Comodo = IIf(IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_1")), 0, aDataView1.Item(myRowIndex).Item("Sconto_1"))
                Try
                    txtSconto1Ins.Text = txtSconto1Ins.Text.Replace(".", ",")
                    If CDec(Comodo) <> CDec(IIf(Not IsNumeric(txtSconto1Ins.Text.Trim), 0, txtSconto1Ins.Text.Trim)) Then
                        SWGeneraPeriodi = True
                    End If
                Catch ex As Exception
                End Try
                '-
                txtSconto1Ins.Text = txtSconto1Ins.Text.Replace(".", ",")
                Comodo = txtSconto1Ins.Text.Trim 'giu051211 CType(row.Cells(CellIdx.Sc1).Controls(0), TextBox).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Sconto_1") = CDec(Comodo.Trim)
                'DEBUG If Comodo = "0" Then lblMessAgg.Text = "NESSUN SCONTO"
                'giu051211 non gestito per IREDEEM
                ' ''Comodo = CType(row.Cells(CellIdx.Sc2).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Sconto_2") = 0 'GIU130112
                aDataView1.Item(myRowIndex).Item("Sconto_3") = 0 'GIU130112
                aDataView1.Item(myRowIndex).Item("Sconto_4") = 0 'GIU130112
                aDataView1.Item(myRowIndex).Item("Sconto_Pag") = 0 'GIU130112
                'giu051211 non gestito per IREDEEM
                ' ''Comodo = CType(row.Cells(CellIdx.ScVal).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("ScontoValore") = 0 'GIU130112
                'giu110112 sconto valore GESTIONE - PER IL CALCOLO PROVVIGIONE AGENTE
                If myPrezzo = myPrezzoAL Then
                    checkNoScontoValore.Checked = True
                    aDataView1.Item(myRowIndex).Item("ScontoValore") = 0
                    aDataView1.Item(myRowIndex).Item("SWPNettoModificato") = False
                    aDataView1.Item(myRowIndex).Item("Prezzo_Netto_Inputato") = CDec(0)
                Else
                    If myPrezzo > myPrezzoAL Then
                        lblMessAgg.BorderStyle = BorderStyle.Outset
                        lblMessAgg.Text = "Attenzione, Prezzo maggiore del Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                        ' ''myErrore += " Il Prezzo Netto non può essere superiore al Prezzo base"
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' ''ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        ' ''Exit Sub
                    End If
                    'giu160112 checkNoScontoValore
                    If checkNoScontoValore.Checked = False Then
                        If myPrezzo < 0 Then 'giu260419
                            aDataView1.Item(myRowIndex).Item("ScontoValore") = 0
                            checkNoScontoValore.Checked = True
                            myErrore += " Sconto valore non applicabile: Prezzo dev'essere maggiore di zero"
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                            Exit Sub
                        Else
                            aDataView1.Item(myRowIndex).Item("ScontoValore") = CDec(myPrezzoAL - myPrezzo)
                        End If
                    Else
                        aDataView1.Item(myRowIndex).Item("ScontoValore") = 0
                    End If
                    aDataView1.Item(myRowIndex).Item("SWPNettoModificato") = True
                    aDataView1.Item(myRowIndex).Item("Prezzo_Netto_Inputato") = CDec(myPrezzo)
                End If
                '--------------------------------------------------------------------
                '--- differenziare in base al tipo di documento sulla qta se evasa etc etc
                'giu101111 Dim PrezzoNetto As Decimal = 0

                'giu051211 dichiarati sopra
                myQtaO = 0 : myQtaE = 0 : myQtaR = 0
                myQuantita = 0 : SWQta = ""
                '--------------------------
                myQtaO = aDataView1.Item(myRowIndex).Item("Qta_Ordinata")
                myQtaE = aDataView1.Item(myRowIndex).Item("Qta_Evasa")
                '-----------------------------------------------------------------
                If myQtaE > myQtaO Then
                    myQtaR = 0
                Else
                    myQtaR = myQtaE - myQtaO
                    If myQtaR < 0 Then myQtaR = myQtaR * -1
                End If
                aDataView1.Item(myRowIndex).Item("Qta_Residua") = myQtaR
                myQuantita = aDataView1.Item(myRowIndex).Item("Qta_Ordinata")
                SWQta = "O"
                '--
                myErrore = ""
                '--------------------------
                If myCodArt.Trim <> "" Or myPrezzo <> 0 Then
                    If myQuantita = 0 Then myErrore += "Quantità ordinata obbligatoria"
                    If myIVA = 0 Then myErrore += " IVA obbligatoria"
                    If myErrore.Trim <> "" Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
                If myPrezzo = 0 And myQuantita <> 0 Then
                    If myIVA = 0 Then myErrore += " IVA obbligatoria"
                    If myPrezzo = 0 Then
                        lblMessAgg.BorderStyle = BorderStyle.Outset
                        lblMessAgg.Text = "Attenzione, Prezzo a ZERO - Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                    End If
                    If myErrore.Trim <> "" Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
                'Richiesta di Cinzia (Tel. 291211) la qta' evasa mai superiore alla richiesta
                If myQtaE > myQtaO Then
                    myErrore += " La quantità Evasa non può essere superiore alla quantità Ordinata"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                '----------------------------------------------------------------------------
                '-giu061211
                If lblBase.Text.Trim <> "" And lblBase.Text.Trim <> HTML_SPAZIO Then
                    If IsNumeric(lblBase.Text.Trim) Then
                        aDataView1.Item(myRowIndex).Item("LBase") = CDbl(lblBase.Text.Trim)
                    End If
                End If
                If lblOpz.Text.Trim <> "" And lblOpz.Text.Trim <> HTML_SPAZIO Then
                    If IsNumeric(lblOpz.Text.Trim) Then
                        aDataView1.Item(myRowIndex).Item("LOpz") = CDbl(lblOpz.Text.Trim)
                    End If
                End If
                '------------
                Passo = 12
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Prezzo")) Then aDataView1.Item(myRowIndex).Item("Prezzo") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_1")) Then aDataView1.Item(myRowIndex).Item("Sconto_1") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_2")) Then aDataView1.Item(myRowIndex).Item("Sconto_2") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_3")) Then aDataView1.Item(myRowIndex).Item("Sconto_3") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_4")) Then aDataView1.Item(myRowIndex).Item("Sconto_4") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Sconto_Pag")) Then aDataView1.Item(myRowIndex).Item("Sconto_Pag") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("ScontoValore")) Then aDataView1.Item(myRowIndex).Item("ScontoValore") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Importo")) Then aDataView1.Item(myRowIndex).Item("Importo") = 0
                If IsDBNull(aDataView1.Item(myRowIndex).Item("Prezzo_Netto")) Then aDataView1.Item(myRowIndex).Item("Prezzo_Netto") = 0
                'giu020519
                'GIU300419
                aDataView1.Item(myRowIndex).Item("DedPerAcconto") = False
                'giu130220
                Passo = 13
                aDataView1.Item(myRowIndex).Item("SWSostituito") = chkSWSostituito.Checked
                aDataView1.Item(myRowIndex).Item("SWCalcoloTot") = chkSWCalcoloTot.Checked
                'GIU090222 VERIFICO SE E' CAMBIATO PER ATTIVARE LA RICHIESTA RIGENERA PERIODI
                'giu061223
                txtDataSc.BackColor = SEGNALA_OK : txtDataSc.ToolTip = ""
                If txtDataSc.Text.Trim <> "" And txtDataSc.Text.Trim.Length <> 10 Then
                    txtDataSc.BackColor = SEGNALA_KO
                    txtDataSc.ToolTip = "Data Scadenza errata.!!"
                    Exit Sub
                ElseIf txtDataSc.Text.Trim <> "" And Not IsDate(txtDataSc.Text.Trim) Then
                    txtDataSc.BackColor = SEGNALA_KO
                    txtDataSc.ToolTip = "Data Scadenza errata.!!"
                    Exit Sub
                ElseIf Not IsDate(txtDataSc.Text.Trim) Then
                    txtDataSc.BackColor = SEGNALA_KO
                    txtDataSc.ToolTip = "Data Scadenza obbligatoria.!!"
                    Exit Sub
                End If
                'giu150424
                txtDataScCons.BackColor = SEGNALA_OK : txtDataScCons.ToolTip = ""
                If txtDataScCons.Text.Trim <> "" And txtDataScCons.Text.Trim.Length <> 10 Then
                    txtDataScCons.BackColor = SEGNALA_KO
                    txtDataScCons.ToolTip = "Data Scadenza errata.!!"
                    Exit Sub
                ElseIf txtDataScCons.Text.Trim <> "" And Not IsDate(txtDataScCons.Text.Trim) Then
                    txtDataScCons.BackColor = SEGNALA_KO
                    txtDataScCons.ToolTip = "Data Scadenza errata.!!"
                    Exit Sub
                ElseIf Not IsDate(txtDataScCons.Text.Trim) Then
                    txtDataScCons.BackColor = SEGNALA_KO
                    txtDataScCons.ToolTip = "Data Scadenza obbligatoria.!!"
                    Exit Sub
                End If
                '---------
                txtDataEv.BackColor = SEGNALA_OK : txtDataEv.ToolTip = ""
                If txtDataEv.Text.Trim <> "" And txtDataEv.Text.Trim.Length <> 10 Then
                    txtDataEv.BackColor = SEGNALA_KO
                    txtDataEv.ToolTip = "Data evasione errata.!!"
                    Exit Sub
                ElseIf txtDataEv.Text.Trim <> "" And Not IsDate(txtDataEv.Text.Trim) Then
                    txtDataEv.BackColor = SEGNALA_KO
                    txtDataEv.ToolTip = "Data evasione errata.!!"
                    Exit Sub
                End If
                'giu131223 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim strMess As String = ""
                If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
                    'nulla
                Else
                    strMess = ""
                    If ckDataScadenza(txtDataSc.Text, strMess) = False Then
                        txtDataSc.BackColor = SEGNALA_KO
                        txtDataSc.ToolTip = "Data Scadenza non compresa tra le date di Inizio/Fine contratto o del periodo selezionato.!!"
                        '-
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                    'Periodo in cui è da spostare
                    Dim NewDNRiga As String = Session("NewDNRiga")
                    If (NewDNRiga Is Nothing) Then
                        NewDNRiga = ""
                    End If
                    If String.IsNullOrEmpty(NewDNRiga) Then
                        NewDNRiga = ""
                    End If
                    If NewDNRiga.Trim = "" Then
                        strMess += "Non è stato trovato il Periodo di destinazione<br>"
                    End If
                    '-
                    Dim NewDNRigaDes As String = Session("NewDNRigaDes")
                    If String.IsNullOrEmpty(NewDNRigaDes) Then
                        NewDNRigaDes = ""
                    End If
                    If NewDNRigaDes.Trim = "" Then
                        strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    End If
                    If strMess.Trim <> "" Then
                        txtDataSc.BackColor = SEGNALA_KO
                        txtDataSc.ToolTip = "Data Scadenza non compresa tra le date di Inizio/Fine contratto o del periodo selezionato.!!"
                        '-
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", strMess.Trim, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                    '----------------------------
                End If
                '------@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If txtDataEv.Text.Trim <> "" Then
                    chkEvasa.AutoPostBack = False
                    chkEvasa.Checked = True
                    chkEvasa.AutoPostBack = True
                Else
                    chkEvasa.AutoPostBack = False
                    chkEvasa.Checked = False
                    chkEvasa.AutoPostBack = True
                End If
                '-
                If IsDBNull(aDataView1.Item(myRowIndex).Item("DataSc")) Then
                    Comodo = ""
                Else
                    Comodo = aDataView1.Item(myRowIndex).Item("DataSc").ToString.Trim
                End If
                If IsDate(Comodo) And IsDate(txtDataSc.Text.Trim) Then
                    If CDate(Comodo) <> CDate(txtDataSc.Text.Trim) Then
                        SWGeneraPeriodi = True
                    End If
                ElseIf Not IsDate(Comodo) And Not IsDate(txtDataSc.Text.Trim) Then
                    'NULLA
                Else
                    SWGeneraPeriodi = True
                End If
                '-------------------------
                If IsDate(txtDataSc.Text.Trim) Then
                    aDataView1.Item(myRowIndex).Item("DataSc") = CDate(txtDataSc.Text.Trim)
                Else
                    aDataView1.Item(myRowIndex).Item("DataSc") = DBNull.Value
                End If
                'giu150424
                If IsDate(txtDataScCons.Text.Trim) Then
                    aDataView1.Item(myRowIndex).Item("RefDataNC") = CDate(txtDataScCons.Text.Trim)
                Else
                    aDataView1.Item(myRowIndex).Item("RefDataNC") = DBNull.Value
                End If
                '-
                If IsDate(txtDataEv.Text.Trim) Then
                    aDataView1.Item(myRowIndex).Item("DataEv") = CDate(txtDataEv.Text.Trim)
                    If myQtaE = 0 Then
                        lblMessAgg.ForeColor = Drawing.Color.DarkRed
                        lblMessAgg.Text = "ATTIVITA' EVASA: Impostato la quantità Evasa UGUALE alla quantità Ordinata"
                        myQtaE = myQtaO
                        aDataView1.Item(myRowIndex).Item("Qta_Evasa") = myQtaE
                        lblQtaEv.Text = FormattaNumero(myQtaE, -1)
                        '-----------------------------------------------------------------
                        If myQtaE > myQtaO Then
                            myQtaR = 0
                        Else
                            myQtaR = myQtaE - myQtaO
                            If myQtaR < 0 Then myQtaR = myQtaR * -1
                        End If
                        LblQtaRe.Text = FormattaNumero(myQtaR, -1)
                        aDataView1.Item(myRowIndex).Item("Qta_Residua") = myQtaR
                    End If
                Else
                    aDataView1.Item(myRowIndex).Item("DataEv") = DBNull.Value
                    If myQtaE > 0 Then
                        lblMessAgg.ForeColor = Drawing.Color.DarkRed
                        lblMessAgg.Text = "ATTIVITA' DA EVADERE: Impostato la quantità Evasa UGUALE ZERO"
                        myQtaE = 0
                        aDataView1.Item(myRowIndex).Item("Qta_Evasa") = myQtaE
                        lblQtaEv.Text = FormattaNumero(myQtaE, -1)
                        '-----------------------------------------------------------------
                        If myQtaE > myQtaO Then
                            myQtaR = 0
                        Else
                            myQtaR = myQtaE - myQtaO
                            If myQtaR < 0 Then myQtaR = myQtaR * -1
                        End If
                        LblQtaRe.Text = FormattaNumero(myQtaR, -1)
                        aDataView1.Item(myRowIndex).Item("Qta_Residua") = myQtaR
                    End If
                End If
                If txtDataEv.Text.Trim <> "" Then
                    chkEvasa.AutoPostBack = False
                    chkEvasa.Checked = True
                    chkEvasa.AutoPostBack = True
                Else
                    chkEvasa.AutoPostBack = False
                    chkEvasa.Checked = False
                    chkEvasa.AutoPostBack = True
                End If
                '-
                'GIU090222
                Comodo = IIf(IsDBNull(aDataView1.Item(myRowIndex).Item("Serie")), "", aDataView1.Item(myRowIndex).Item("Serie").ToString.Trim)
                Comodo += IIf(IsDBNull(aDataView1.Item(myRowIndex).Item("Lotto")), "", aDataView1.Item(myRowIndex).Item("Lotto").ToString.Trim)
                Comodo = Formatta.FormattaNomeFile(Comodo)
                If Comodo <> Formatta.FormattaNomeFile(txtSerie.Text.Trim.ToUpper + txtLotto.Text.Trim.ToUpper) Then
                    SWGeneraPeriodi = True
                End If
                '-
                aDataView1.Item(myRowIndex).Item("Serie") = Formatta.FormattaNomeFile(txtSerie.Text.Trim.ToUpper)
                aDataView1.Item(myRowIndex).Item("Lotto") = Formatta.FormattaNomeFile(txtLotto.Text.Trim.ToUpper)
                Try 'giu310320 mi sa che non esiste piu nella griglia lo lascio ma con TRY 
                    aDataView1.Item(myRowIndex).Item("SerieLotto") = Formatta.FormattaNomeFile(txtSerie.Text.Trim.ToUpper + txtLotto.Text.Trim.ToUpper)
                Catch ex As Exception
                    'non importa
                End Try
                '-
                If txtSerie.Text.Trim = "" And txtLotto.Text.Trim = "" Then
                    txtSerie.BackColor = SEGNALA_KO
                    txtLotto.BackColor = SEGNALA_KO
                    SWGeneraPeriodi = True
                Else
                    txtSerie.BackColor = SEGNALA_OK
                    txtLotto.BackColor = SEGNALA_OK
                End If
                '-
                aDataView1.Item(myRowIndex).Item("Note") = txtNote.Text.Trim
                'giu280420
                aDataView1.Item(myRowIndex).Item("QtaDurataNumR0") = DDLModello.SelectedIndex
                'SELEZ.
                Passo = 14
                myCGDest = ""
                myPos = InStr(lblDestSelDett.Text.Trim, "-")
                If myPos > 0 Then
                    myCGDest = Mid(lblDestSelDett.Text.Trim, 1, myPos - 1)
                End If
                If Not IsNumeric(myCGDest) Then
                    myCGDest = ""
                End If
                If myCGDest.Trim = "" Or Val(myCGDest) = 0 Then
                    aDataView1.Item(myRowIndex).Item("Cod_Filiale") = DBNull.Value
                Else
                    aDataView1.Item(myRowIndex).Item("Cod_Filiale") = myCGDest.Trim
                End If
                'giu231023 giu061223
                If (DDLRespAreaApp Is Nothing) Or (DDLRespVisiteApp Is Nothing) Then
                    aDataView1.Item(myRowIndex).Item("RespArea") = DBNull.Value
                    aDataView1.Item(myRowIndex).Item("RespVisite") = DBNull.Value
                Else
                    If Not IsNumeric(DDLRespAreaApp.SelectedValue) Then
                        aDataView1.Item(myRowIndex).Item("RespArea") = DBNull.Value
                        aDataView1.Item(myRowIndex).Item("RespVisite") = DBNull.Value
                    ElseIf DDLRespAreaApp.SelectedValue > 0 Then
                        aDataView1.Item(myRowIndex).Item("RespArea") = DDLRespAreaApp.SelectedValue
                        If Not IsNumeric(DDLRespVisiteApp.SelectedValue) Then
                            aDataView1.Item(myRowIndex).Item("RespArea") = DBNull.Value
                            aDataView1.Item(myRowIndex).Item("RespVisite") = DBNull.Value
                        ElseIf DDLRespVisiteApp.SelectedValue > 0 Then
                            aDataView1.Item(myRowIndex).Item("RespVisite") = DDLRespVisiteApp.SelectedValue
                        Else
                            aDataView1.Item(myRowIndex).Item("RespArea") = DBNull.Value
                            aDataView1.Item(myRowIndex).Item("RespVisite") = DBNull.Value
                        End If
                    Else
                        aDataView1.Item(myRowIndex).Item("RespArea") = DBNull.Value
                        aDataView1.Item(myRowIndex).Item("RespVisite") = DBNull.Value
                    End If
                End If
                '---------
                Passo = 15
                'GIU090222
                Try
                    Dim myrow As GridViewRow = GridViewDett.SelectedRow
                    Comodo = myrow.Cells(CellIdx.Prz).Text.Trim
                    If CDec(Comodo) <> CDec(txtPrezzoIns.Text.Trim) Then
                        SWGeneraPeriodi = True
                    End If
                Catch ex As Exception
                End Try
                '-
                If chkSWCalcoloTot.Checked Then myQuantita = 0
                aDataView1.Item(myRowIndex).Item("Importo") = _
                    CalcolaImporto(aDataView1.Item(myRowIndex).Item("Prezzo"), myQuantita, _
                               aDataView1.Item(myRowIndex).Item("Sconto_1"), _
                               aDataView1.Item(myRowIndex).Item("Sconto_2"), _
                               aDataView1.Item(myRowIndex).Item("Sconto_3"), _
                               aDataView1.Item(myRowIndex).Item("Sconto_4"), _
                               aDataView1.Item(myRowIndex).Item("Sconto_Pag"), _
                               aDataView1.Item(myRowIndex).Item("ScontoValore"), _
                               aDataView1.Item(myRowIndex).Item("Importo"), _
                               ScontoSuImporto, _
                               CInt(DecimaliVal), _
                               aDataView1.Item(myRowIndex).Item("Prezzo_Netto"), ScCassaDett, CDec(ScCassa), aDataView1.Item(myRowIndex).Item("DedPerAcconto")) 'giu010119 giu020519
                LblPrezzoNetto.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Prezzo_Netto"), CInt(DecimaliVal))
                LblImportoRiga.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Importo"), CInt(DecimaliVal))
                'giu110112 sconto valore GESTIONE - PER IL CALCOLO PROVVIGIONE AGENTE
                Passo = 16
                If aDataView1.Item(myRowIndex).Item("Prezzo") > 0 Then
                    'giu160112
                    If myPrezzoAL = 0 Then myPrezzoAL = aDataView1.Item(myRowIndex).Item("Prezzo") 'giu230412
                    If aDataView1.Item(myRowIndex).Item("Prezzo") = myPrezzoAL Then
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = _
                           Math.Round(aDataView1.Item(myRowIndex).Item("Prezzo_Netto") * 100 / aDataView1.Item(myRowIndex).Item("Prezzo"), 4)
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = 100 - aDataView1.Item(myRowIndex).Item("ScontoReale")
                    Else
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = _
                            Math.Round(aDataView1.Item(myRowIndex).Item("Prezzo_Netto") * 100 / myPrezzoAL, 4)
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = 100 - aDataView1.Item(myRowIndex).Item("ScontoReale")
                    End If
                Else
                    If myPrezzoAL = 0 Then
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = 0
                    Else
                        aDataView1.Item(myRowIndex).Item("ScontoReale") = 100
                    End If
                End If
                'giu290312 giu300312
                Passo = 17
                strErroreAggAgente = ""
                If myQtaO <> 0 Or myQtaE <> 0 Or myQuantita <> 0 Or myPrezzo <> 0 Or _
                    aDataView1.Item(myRowIndex).Item("Prezzo_Netto") <> 0 Or _
                    aDataView1.Item(myRowIndex).Item("Cod_Articolo") <> "" Then
                    aDataView1.Item(myRowIndex).Item("Cod_Agente") = CLng(IDCAge)
                    If CLng(IDCAge) = 0 Then
                        aDataView1.Item(myRowIndex).Item("Pro_Agente") = 0
                        aDataView1.Item(myRowIndex).Item("ImportoProvvigione") = 0
                    Else
                        MySuperatoSconto = False
                        lblSuperatoScMax.Text = ""
                        lblSuperatoScMax.BorderStyle = BorderStyle.None
                        If Calcola_ProvvAgente(aDataView1.Item(myRowIndex).Item("Cod_Articolo"), _
                            CLng(IDCAge), myProvvAg, _
                            aDataView1.Item(myRowIndex).Item("ScontoReale"), _
                            MySuperatoSconto, strErroreAggAgente) = False Then
                            aDataView1.Item(myRowIndex).Item("Pro_Agente") = 0
                            aDataView1.Item(myRowIndex).Item("ImportoProvvigione") = 0
                        Else
                            aDataView1.Item(myRowIndex).Item("Pro_Agente") = myProvvAg
                            aDataView1.Item(myRowIndex).Item("ImportoProvvigione") = aDataView1.Item(myRowIndex).Item("Importo") * myProvvAg / 100
                        End If
                        If MySuperatoSconto = True Then
                            lblSuperatoScMax.BorderStyle = BorderStyle.Outset
                            lblSuperatoScMax.Text = "Superato Sc."
                        End If
                    End If
                Else
                    aDataView1.Item(myRowIndex).Item("Cod_Agente") = DBNull.Value
                    aDataView1.Item(myRowIndex).Item("Pro_Agente") = 0
                    aDataView1.Item(myRowIndex).Item("ImportoProvvigione") = 0
                End If
                ' ''Comodo = CType(row.Cells(CellIdx.CAge).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                ' ''aDataView1.Item(myRowIndex).Item("Cod_Agente") = CDbl(Comodo.Trim)
                'giu140412
                Passo = 18
                aDataView1.Item(myRowIndex).Item("PrezzoAcquisto") = _myPrezzoAcquisto
                aDataView1.Item(myRowIndex).Item("PrezzoListino") = _myPrezzoListino
                'GIU230412 
                If Not IsNumeric(txtPrezzoCosto.Text) Then txtPrezzoCosto.Text = "0"
                txtPrezzoCosto.Text = FormattaNumero(txtPrezzoCosto.Text, CInt(DecimaliVal))
                aDataView1.Item(myRowIndex).Item("PrezzoCosto") = CDbl(txtPrezzoCosto.Text)
                '---------
                Passo = 19
                SqlAdapDocDett = Session("aSqlAdap")
                DsContrattiDett = Session("aDsDett")

                If (aDataView1 Is Nothing) Then
                    aDataView1 = New DataView(DsContrattiDett.ContrattiD)
                End If
                If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
                Session("aDataView1") = aDataView1
                Session("aDsDett") = DsContrattiDett
                GridViewDett.DataSource = aDataView1
                GridViewDett.EditIndex = -1
                GridViewDett.DataBind()
                SWPrezzoCosto = txtPrezzoCosto.Enabled 'giu230412
                EnableTOTxtInsArticoli(False)
                'se venisse modificato UN DETT rifare l'UPDATE COME SOPRA QUANDO RIENTRA
                Passo = 20
                strErroreAggRiga = ""
                If AggiornaImporto(DsContrattiDett, strErroreAggRiga) = False Then
                    'giu300312 spostato sotto
                End If
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in ContrattiDett.btnAggArtGridSel. Passo: " & Passo.ToString, Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            'GIU030212
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWMODIFICATO) = SWSI
        Catch ex As Exception
            'va bene cosi vuol dire che non sono riuscito a giu221111
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.btnAggArtGridSel. Passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        If strErroreAggRiga.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.AggiornaImporto", strErroreAggRiga, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If strErroreAggAgente.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.Calcola_ProvvAgente", strErroreAggAgente, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu140412 
        If myErrGetPrezziLA.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GetPrezziListinoAcquisto", myErrGetPrezziLA, WUC_ModalPopup.TYPE_ERROR)
            ' ''Exit Sub
        End If
        'giu230412
        If SWPrezzoCosto = True Then
            txtPrezzoCosto.Enabled = False
            If Not IsNumeric(txtPrezzoCosto.Text) Then txtPrezzoCosto.Text = "0"
            txtPrezzoCosto.BackColor = SEGNALA_OK
        Else
            txtPrezzoCosto.BackColor = SEGNALA_OK
        End If
        
        ''-
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
        'riporto dati riga prec percollegamento IMPORTANTE il n.serie/lotto solo x le APP.
        Passo = 21
        Try
            If DDLTipoDettagli.SelectedIndex = 0 Then
                'SELEZ.
                myCGDest = ""
                If myPos > 0 Then
                    myCGDest = Mid(lblDestSelDett.Text.Trim, 1, myPos - 1)
                End If
                If Not IsNumeric(myCGDest) Then
                    myCGDest = ""
                End If
                'giu231023 'giu061223
                If (DDLRespAreaApp Is Nothing) Or (DDLRespVisiteApp Is Nothing) Then
                    myRespAreapApp = 0
                    myRespVisiteApp = 0
                Else
                    Try
                        myRespAreapApp = DDLRespAreaApp.SelectedValue
                    Catch ex As Exception
                        myRespAreapApp = 0
                    End Try
                    Try
                        myRespVisiteApp = DDLRespVisiteApp.SelectedValue
                    Catch ex As Exception
                        myRespVisiteApp = 0
                    End Try
                End If
                '-
                Dim mytxtSerie As String = Formatta.FormattaNomeFile(txtSerie.Text.Trim.ToUpper)
                Dim mytxtLotto As String = Formatta.FormattaNomeFile(txtLotto.Text.Trim.ToUpper)
                Dim mySerieLotto As String = mytxtSerie + mytxtLotto
                '-
                SqlAdapDocDett = Session("aSqlAdap")
                DsContrattiDett = Session("aDsDett")
                Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("")
                Dim RowD As DSDocumenti.ContrattiDRow
                For Each RowD In RowsD
                    RowD.BeginEdit()
                    If myCGDest.Trim = "" Then
                        RowD.SetCod_FilialeNull()
                    Else
                        RowD.Cod_Filiale = myCGDest.Trim
                    End If
                    If myRespAreapApp = 0 Then
                        RowD.SetRespAreaNull()
                    Else
                        RowD.RespArea = myRespAreapApp
                    End If
                    If myRespVisiteApp = 0 Then
                        RowD.SetRespVisiteNull()
                    Else
                        RowD.RespVisite = myRespVisiteApp
                    End If
                    RowD.Serie = mytxtSerie.Trim
                    RowD.Lotto = mytxtLotto.Trim
                    RowD.QtaDurataNumR0 = DDLModello.SelectedIndex
                    RowD.EndEdit()
                Next
                If (aDataView1 Is Nothing) Then
                    aDataView1 = New DataView(DsContrattiDett.ContrattiD)
                End If
                If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
                Session("aDataView1") = aDataView1
                Session("aDsDett") = DsContrattiDett
                GridViewDett.DataSource = aDataView1
                GridViewDett.EditIndex = -1
                GridViewDett.DataBind()
                If SWGeneraPeriodi = True Then _WucElement.btnbtnGeneraAttDNumColorRED(True)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.btnAggArtGridSel. (Riporto dati App.) Passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

    End Sub

    Private Function ckDataScadenza(ByVal pDataNEW As String, ByRef strMess As String) As Boolean
        ckDataScadenza = False
        Dim myDurataNum As String = Session(CSTDURATANUM)
        If IsNothing(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        ElseIf Not IsNumeric(myDurataNum) Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        ElseIf Val(myDurataNum) = 0 Then
            strMess = "Non è stato possibile recuperare la durata del Contratto"
            Exit Function
        End If
        '-
        Dim myDurataTipo As String = Session(CSTDURATATIPO)
        If IsNothing(myDurataTipo) Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDurataTipo) Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        ElseIf myDurataTipo.Trim = "" Then
            strMess = "Non è stato possibile recuperare la durata Tipo del Contratto"
            Exit Function
        End If
        '-
        Dim myDataInizio As String = Session(CSTDATAINIZIO)
        If IsNothing(myDataInizio) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDataInizio) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        ElseIf Not IsDate(myDataInizio.Trim) Then
            strMess = "Non è stato possibile recuperare la Data Inizio del Contratto"
            Exit Function
        End If
        'giu130424
        Dim myDataFine As String = Session(CSTDATAFINE)
        If IsNothing(myDataFine) Then
            strMess = "Non è stato possibile recuperare la Data Fine del Contratto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myDataFine) Then
            strMess = "Non è stato possibile recuperare la Data Fine del Contratto"
            Exit Function
        ElseIf Not IsDate(myDataFine.Trim) Then
            strMess = "Non è stato possibile recuperare la Data Fine del Contratto"
            Exit Function
        End If
        If CDate(pDataNEW) < CDate(myDataInizio) Or CDate(pDataNEW) > CDate(myDataFine) Then
            strMess += "Non compresa nelle date di Inizio/Fine Contratto<br>"
            Session("NewDNRiga") = ""
            Session("NewDNRigaDes") = ""
            Exit Function
        End If
        Dim myPagAnticipato As Boolean = True
        Try
            myPagAnticipato = CBool(Session(CSTANTICIPATO))
        Catch ex As Exception
            myPagAnticipato = True
        End Try
        '----------
        Dim NewDNRiga As Integer = -1
        Dim NewDNRigaDes As String = ""
        If myDurataTipo.Trim = "M" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "T" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "Q" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "S" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio), FormatoData)
                If CDate(pDataNEW) >= CDate(NewDNRigaDes) And CDate(pDataNEW) <= CDate(myDataInizio) Then
                    NewDNRiga = i + 1
                    Exit For
                ElseIf CDate(pDataNEW) < CDate(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "A" Then
            For i = 0 To Val(myDurataNum) - 1
                NewDNRigaDes = Format(CDate(myDataInizio).Year, "0000")
                If CDate(pDataNEW).Year >= CInt(NewDNRigaDes) And CDate(pDataNEW).Year <= CDate(myDataInizio).Year Then
                    NewDNRiga = i
                    Exit For
                ElseIf CDate(pDataNEW).Year < CInt(NewDNRigaDes) Then
                    strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
                    Session("NewDNRiga") = ""
                    Session("NewDNRigaDes") = ""
                    Exit Function
                End If
                myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
            Next i
        Else 'non capitera' mai
            strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
            Session("NewDNRiga") = ""
            Session("NewDNRigaDes") = ""
            Exit Function
        End If
        If NewDNRiga = -1 Then
            strMess += "Non è stato trovato il Periodo di destinazione Oppure non compresa nelle date di Inizio/Fine Contratto<br>"
            Session("NewDNRiga") = ""
            Session("NewDNRigaDes") = ""
            Exit Function
        End If
        Session("NewDNRiga") = NewDNRiga
        Session("NewDNRigaDes") = NewDNRigaDes
        ckDataScadenza = True
    End Function

#End Region

#Region "Gestione con TESTATA (TD0) E TOTALE (TD2)"
    Public Function ReBuildDett() As Boolean
        'Simile al LOAD
        SetCdmDAdp()
        Session("aSqlAdap") = SqlAdapDocDett
        '--
        If (Session("aDsDett") Is Nothing) Then
            DsContrattiDett = New DSDocumenti
        Else
            DsContrattiDett = Session("aDsDett")
        End If
        '--
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        '
        DsContrattiDett.ContrattiD.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
        SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
        '---------------------------------------
        AzzQtaNULL()
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '---
        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu011111
        GridViewDett.DataSource = aDataView1
        Session("aDataView1") = aDataView1
        GridViewDett.DataBind()
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            Call _WucElement.SetLblMessDoc("ATTENZIONE, Nessun dettaglio presente. <br> per la cancellazione, eliminare tutte le righe presenti e Aggiornare.")
        Else
            Dim ctrCodArt As String = ""
            Try
                ctrCodArt = aDataView1.Item(0).Item("Cod_Articolo")
            Catch ex As Exception
                ctrCodArt = ""
            End Try
            If ctrCodArt.Trim = "" Then
                Call _WucElement.SetLblMessDoc("ATTENZIONE, Nessun dettaglio presente. <br> per la cancellazione, eliminare tutte le righe presenti e Aggiornare.")
            End If
            SetBtnPrimaRigaEnabled(False)
        End If
    End Function
    Public Function TD1ReBuildDett(Optional ByVal SWReBuildDettFromDB As Boolean = False) As Boolean
        'Simile al LOAD
        SetCdmDAdp()
        Session("aSqlAdap") = SqlAdapDocDett
        '--
        If (Session("aDsDett") Is Nothing) Then
            DsContrattiDett = New DSDocumenti
        Else
            DsContrattiDett = Session("aDsDett")
        End If
        '--
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If Session(SWOP) = SWOPNUOVO Then
            DsContrattiDett.ContrattiD.Clear()
            DsContrattiDett.ContrattiD.AcceptChanges()
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            '------------------------------------------------------------------------------------
        Else
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Function
            End If
            'GIU021111 X FUNZ Pier311011 - giu221111
            If SWReBuildDettFromDB = True Then
                DsContrattiDett.ContrattiD.Clear()
                SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
                SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
                SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
            End If
            '---------------------------------------
        End If
        AzzQtaNULL()
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '---
        'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
        lblTotDett.Text = HTML_SPAZIO 'giu180420
        AzzeraTxtInsArticoli() 'GIU041211
        '---
        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu011111
        GridViewDett.DataSource = aDataView1
        Session("aDataView1") = aDataView1
        GridViewDett.DataBind()
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            Call _WucElement.SetLblMessDoc("ATTENZIONE, Nessun dettaglio presente. <br> per la cancellazione, eliminare tutte le righe presenti e Aggiornare.")
        Else
            Dim ctrCodArt As String = ""
            Try
                ctrCodArt = aDataView1.Item(0).Item("Cod_Articolo")
            Catch ex As Exception
                ctrCodArt = ""
            End Try
            If ctrCodArt.Trim = "" Then
                ' ''Call _WucElement.btnbtnGeneraAttDNumColorRED(True)
                Call _WucElement.SetLblMessDoc("ATTENZIONE, Nessun dettaglio presente. <br> per la cancellazione, eliminare tutte le righe presenti e Aggiornare.")
                '-
            End If
            SetBtnPrimaRigaEnabled(False)
        End If
        '--- LOTTI
        Dim RigaSel As Integer = 0
        Try
            GridViewDett.SelectedIndex = 0
            RigaSel = GridViewDett.SelectedIndex 'giu180419
            If RigaSel = 0 Then 'giu180419
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then 'giu180419
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            lblTotDett.Text = HTML_SPAZIO 'giu180420
            AzzeraTxtInsArticoli() 'GIU041211
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
        EnableTOTxtInsArticoli(False)
        Dim strErrore As String = ""
        If AggiornaImporto(DsContrattiDett, strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
       
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Call SetDDLDettDurNumRiga()
        Try
            DDLDurNumRIga.SelectedIndex = Val(myIDDurataNumR)
            Session(IDDURATANUMRIGA) = myIDDurataNumR
        Catch ex As Exception
            DDLDurNumRIga.SelectedIndex = 0
            myIDDurataNumR = DDLDurNumRIga.SelectedIndex.ToString.Trim
            Session(IDDURATANUMRIGA) = myIDDurataNumR
        End Try

    End Function
    'giu231211 da Private a Pubblic
    Public Function AggiornaImporto(ByVal dsDocDett As DataSet, ByRef strErrore As String) As Boolean
        'giu180420 serve solo per i singoli dettagli
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggiornaImporto)")
            Exit Function
        End If
        'giu140319 se richiamato se DSDETTAGLIO DA CAMBIO TIPO PAGAMENTO O ALTRO
        If (dsDocDett Is Nothing) Then
            dsDocDett = Session("aDsDett")
        End If
        '-----------------------------------------------------------------------
        'giu030512
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            If (dsDocDett.Tables("ContrattiD").Rows.Count > 0) Then
                If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti")) Then
                    myID = dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti").ToString.Trim
                    myIDDurataNum = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNum").ToString.Trim
                    myIDDurataNumR = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNumRiga").ToString.Trim
                End If
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Function
            End If
            Session(IDDOCUMENTI) = myID 'GIU060220
            Session(IDDURATANUM) = myIDDurataNum
            Session(IDDURATANUMRIGA) = myIDDurataNumR
        End If
        'Valuta per i decimali per il calcolo OBBLIGATORIO E NON IN SESSIONE SCADUTA
        Dim IDLis As String = Session(IDLISTINO)
        If IsNothing(IDLis) Then IDLis = "1"
        If String.IsNullOrEmpty(IDLis) Then IDLis = "1"
        Session(IDLISTINO) = IDLis
        ' ''Dim CodValuta As String = GetCodValuta(IDLis)
        ' ''Dim DecimaliValutaFinito As Integer = GetDecimali_Valuta(CodValuta)
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        Dim FormatValuta As String = "###,###,##0"
        Select Case CInt(DecimaliVal)
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select
        '--------------------------------------------------
        'Blocco IVA
        Dim Iva(4) As Integer
        Dim Imponibile(4) As Decimal
        Dim Imposta(4) As Decimal

        Dim Totale As Decimal = 0 : Dim TotaleLordoMerce As Decimal = 0
        Dim MoltiplicatoreValuta As Integer 'Dichiarato ma mi sa che non server :)

        WucElement.GetDatiTB3()

        Dim ScCassa As String = Session(CSTSCCASSA)
        If IsNothing(ScCassa) Then ScCassa = "0"
        If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
        If Not IsNumeric(ScCassa) Then ScCassa = "0"
        '-
        Dim Abbuono As String = Session(CSTABBUONO)
        If IsNothing(Abbuono) Then Abbuono = "0"
        If String.IsNullOrEmpty(Abbuono) Then Abbuono = "0"
        If Not IsNumeric(Abbuono) Then Abbuono = "0"
        '-GIU020119
        '---- Calcolo sconto su 
        Dim ScontoSuImporto As Boolean = True 'OK ANCHE SE NON SERVE QUI LA ScCassaDett SI SERVER
        Dim ScCassaDett As Boolean = False 'giu010119
        Try
            ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        'giu010119
        strErrore = ""
        'giu030512
        Dim TotaleLordoMercePL As Decimal = 0
        Dim Deduzioni As Decimal = 0
        If CalcolaTotaleDocCA(dsDocDett, Iva, Imponibile, Imposta, _
                              CInt(DecimaliVal), MoltiplicatoreValuta, _
                              Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
                              myTipoDoc, CDec(Abbuono), CLng(myID), strErrore, ScCassaDett, TotaleLordoMercePL, Deduzioni) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Function
        End If

        Dim LblImponibile As String = FormattaNumero(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4), CInt(DecimaliVal))
        Dim LblImposta As String = "0" 'giu180420 FormattaNumero(Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4), CInt(DecimaliVal))
        lblTotDett.Text = FormattaNumero(CDec(LblImponibile) + CDec(LblImposta), CInt(DecimaliVal))

        'giu250220 non serve a nulla qui COME ERA IN ORIGINE - SOPRA MI SERVE PER DARE IL TOTALE PER SINGOLO PERIODO
        '' ''OBBLIGATORIO perche mi serve per il calcolo del TotaleLordoMerce Qta*Prezzo
        ' ''If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
        ' ''    _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggiornaImporto)")
        ' ''    Exit Function
        ' ''End If
        '' ''giu140319 se richiamato se DSDETTAGLIO DA CAMBIO TIPO PAGAMENTO O ALTRO
        ' ''If (dsDocDett Is Nothing) Then
        ' ''    dsDocDett = Session("aDsDett")
        ' ''End If
        '' ''-----------------------------------------------------------------------
        '' ''giu030512
        ' ''Dim myID As String = Session(IDDOCUMENTI)
        ' ''If IsNothing(myID) Then
        ' ''    myID = ""
        ' ''End If
        ' ''If String.IsNullOrEmpty(myID) Then
        ' ''    myID = ""
        ' ''End If
        '' ''giu050220
        ' ''Dim myIDDurataNum As String = Session(IDDURATANUM)
        ' ''If IsNothing(myIDDurataNum) Then
        ' ''    myIDDurataNum = "0"
        ' ''End If
        ' ''If String.IsNullOrEmpty(myIDDurataNum) Then
        ' ''    myIDDurataNum = "0"
        ' ''End If
        '' ''-
        ' ''Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        ' ''If IsNothing(myIDDurataNumR) Then
        ' ''    myIDDurataNumR = "0"
        ' ''End If
        ' ''If String.IsNullOrEmpty(myIDDurataNumR) Then
        ' ''    myIDDurataNumR = "0"
        ' ''End If
        '' ''----------
        ' ''If myID = "" Or Not IsNumeric(myID) Then
        ' ''    If (dsDocDett.Tables("ContrattiD").Rows.Count > 0) Then
        ' ''        If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti")) Then
        ' ''            myID = dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti").ToString.Trim
        ' ''            myIDDurataNum = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNum").ToString.Trim
        ' ''            myIDDurataNumR = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNumRiga").ToString.Trim
        ' ''        End If
        ' ''    End If
        ' ''    If myID = "" Or Not IsNumeric(myID) Then
        ' ''        _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
        ' ''        Exit Function
        ' ''    End If
        ' ''    Session(IDDOCUMENTI) = myID 'GIU060220
        ' ''    Session(IDDURATANUM) = myIDDurataNum
        ' ''    Session(IDDURATANUMRIGA) = myIDDurataNumR
        ' ''End If
        '' ''Valuta per i decimali per il calcolo OBBLIGATORIO E NON IN SESSIONE SCADUTA
        ' ''Dim IDLis As String = Session(IDLISTINO)
        ' ''If IsNothing(IDLis) Then IDLis = "1"
        ' ''If String.IsNullOrEmpty(IDLis) Then IDLis = "1"
        ' ''Session(IDLISTINO) = IDLis
        '' '' ''Dim CodValuta As String = GetCodValuta(IDLis)
        '' '' ''Dim DecimaliValutaFinito As Integer = GetDecimali_Valuta(CodValuta)
        '' ''Valuta per i decimali per il calcolo
        ' ''Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        ' ''If IsNothing(DecimaliVal) Then DecimaliVal = ""
        ' ''If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        ' ''If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
        ' ''    DecimaliVal = "2" 'Euro
        ' ''End If
        '' ''------------------------------------
        ' ''Dim FormatValuta As String = "###,###,##0"
        ' ''Select Case CInt(DecimaliVal)
        ' ''    Case 0
        ' ''        FormatValuta = "###,###,##0"
        ' ''    Case 2
        ' ''        FormatValuta = "###,###,##0.00"
        ' ''    Case 3
        ' ''        FormatValuta = "###,###,##0.000"
        ' ''    Case 4
        ' ''        FormatValuta = "###,###,##0.0000"
        ' ''    Case Else
        ' ''        FormatValuta = "###,###,##0"
        ' ''End Select
        '' ''--------------------------------------------------
        '' ''Blocco IVA
        ' ''Dim Iva(4) As Integer
        ' ''Dim Imponibile(4) As Decimal
        ' ''Dim Imposta(4) As Decimal
        '' ''Spese Global
        ' ''Dim IvaTrasporto As Integer
        ' ''Dim IvaIncasso As Integer
        ' ''Dim IvaImballo As Integer

        ' ''Dim Totale As Decimal = 0 : Dim TotaleLordoMerce As Decimal = 0
        ' ''Dim MoltiplicatoreValuta As Integer 'Dichiarato ma mi sa che non server :)
        ' ''Dim TmpImp As Decimal
        ' ''Dim Cont As Integer

        ' ''WucElement.GetDatiTB3()

        ' ''Dim ScCassa As String = Session(CSTSCCASSA)
        ' ''If IsNothing(ScCassa) Then ScCassa = "0"
        ' ''If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
        ' ''If Not IsNumeric(ScCassa) Then ScCassa = "0"
        '' ''-
        ' ''Dim Abbuono As String = Session(CSTABBUONO)
        ' ''If IsNothing(Abbuono) Then Abbuono = "0"
        ' ''If String.IsNullOrEmpty(Abbuono) Then Abbuono = "0"
        ' ''If Not IsNumeric(Abbuono) Then Abbuono = "0"
        '' ''-GIU020119
        '' ''---- Calcolo sconto su 
        ' ''Dim ScontoSuImporto As Boolean = True 'OK ANCHE SE NON SERVE QUI LA ScCassaDett SI SERVER
        ' ''Dim ScCassaDett As Boolean = False 'giu010119
        ' ''Try
        ' ''    ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
        ' ''    ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
        ' ''Catch ex As Exception
        ' ''    ScontoSuImporto = True
        ' ''    ScCassaDett = False
        ' ''End Try
        '' ''giu010119
        ' ''strErrore = ""
        '' ''giu030512
        ' ''Dim TotaleLordoMercePL As Decimal = 0
        ' ''Dim Deduzioni As Decimal = 0
        ' ''If CalcolaTotaleDocCA(dsDocDett, Iva, Imponibile, Imposta, _
        ' ''                      CInt(DecimaliVal), MoltiplicatoreValuta, _
        ' ''                      Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
        ' ''                      myTipoDoc, CDec(Abbuono), CLng(myID), strErrore, ScCassaDett, TotaleLordoMercePL, Deduzioni) = False Then
        ' ''    If strErrore <> "" Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''        strErrore = ""
        ' ''    End If
        ' ''    Exit Function
        ' ''End If

        ' ''LblImponibile.Text = FormattaNumero(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4), CInt(DecimaliVal))
        ' ''LblImposta.Text = FormattaNumero(Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4), CInt(DecimaliVal))
        ' ''LblTotale.Text = FormattaNumero(CDec(LblImponibile.Text) + CDec(LblImposta.Text), CInt(DecimaliVal))
        ' ''lblTotDett.Text=?????'giu180420
        ' ''LblTotaleLordoPL.Text = FormattaNumero(CDec(TotaleLordoMercePL), CInt(DecimaliVal))
        ' ''LblTotaleLordo.Text = FormattaNumero(CDec(TotaleLordoMerce), CInt(DecimaliVal))
        ' ''Cont = 0

        ' ''Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        ' ''If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        ' ''If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        ' ''If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '' ''-
        ' ''If Val(RegimeIVA) > 49 Then
        ' ''    IvaTrasporto = Val(RegimeIVA)
        ' ''    IvaImballo = Val(RegimeIVA)
        ' ''    IvaIncasso = Val(RegimeIVA)
        ' ''Else
        ' ''    IvaTrasporto = GetParamGestAzi(Session(ESERCIZIO)).IVATrasporto
        ' ''    IvaIncasso = GetParamGestAzi(Session(ESERCIZIO)).IvaSpese
        ' ''    IvaImballo = GetParamGestAzi(Session(ESERCIZIO)).Iva_Imballo
        ' ''End If

        '' ''ASSEGNAZIONE TOTALE SPESE TRASPORTO
        ' ''Dim SpeseTrasporto As String = Session(CSTSPTRASP)
        ' ''If IsNothing(SpeseTrasporto) Then SpeseTrasporto = "0"
        ' ''If String.IsNullOrEmpty(SpeseTrasporto) Then SpeseTrasporto = "0"
        ' ''If Not IsNumeric(SpeseTrasporto) Then SpeseTrasporto = "0"
        '' ''-
        ' ''If CDec(SpeseTrasporto) Then
        ' ''    While ((IvaTrasporto <> Iva(Cont)) And _
        ' ''            Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
        ' ''        Cont = Cont + 1
        ' ''        If Cont = 4 Then
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''            ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
        ' ''            Exit Function
        ' ''        End If
        ' ''    End While
        ' ''    Iva(Cont) = IvaTrasporto
        ' ''    TmpImp = CDec(SpeseTrasporto)
        ' ''    Imponibile(Cont) = Imponibile(Cont) + TmpImp
        ' ''    If IvaTrasporto < 50 Then
        ' ''        Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
        ' ''    End If
        ' ''End If

        '' ''ASSEGNAZIONE AL TOTALE DELLE SPESE DI IMBALLO E ATTRIBUZIONE NEL COMPUTO DELL'IVA
        ' ''Dim SpeseImballo As String = Session(CSTSPIMBALLO)
        ' ''If IsNothing(SpeseImballo) Then SpeseImballo = "0"
        ' ''If String.IsNullOrEmpty(SpeseImballo) Then SpeseImballo = "0"
        ' ''If Not IsNumeric(SpeseImballo) Then SpeseImballo = "0"
        '' ''-
        ' ''If (CDec(SpeseImballo)) > 0 Then
        ' ''    While ((IvaImballo <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
        ' ''        Cont = Cont + 1
        ' ''        If Cont = 4 Then
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''            ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
        ' ''            Exit Function
        ' ''        End If
        ' ''    End While
        ' ''    Iva(Cont) = IvaImballo
        ' ''    TmpImp = CDec(SpeseImballo)
        ' ''    Imponibile(Cont) = Imponibile(Cont) + TmpImp
        ' ''    If IvaImballo < 50 Then
        ' ''        Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
        ' ''    End If
        ' ''End If

        ' ''Dim SpeseIncasso As String = Session(CSTSPINCASSO)
        ' ''If IsNothing(SpeseIncasso) Then SpeseIncasso = "0"
        ' ''If String.IsNullOrEmpty(SpeseIncasso) Then SpeseIncasso = "0"
        ' ''If Not IsNumeric(SpeseIncasso) Then SpeseIncasso = "0"
        '' ''-
        ' ''If (CDec(SpeseIncasso)) > 0 Then
        ' ''    While ((IvaIncasso <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
        ' ''        Cont = Cont + 1
        ' ''        If Cont = 4 Then
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''            ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
        ' ''            Exit Function
        ' ''        End If
        ' ''    End While
        ' ''    Iva(Cont) = IvaIncasso
        ' ''    TmpImp = CDec(SpeseIncasso)
        ' ''    Imponibile(Cont) = Imponibile(Cont) + TmpImp
        ' ''    If (Iva(Cont) > 0) And (Iva(Cont) < 50) Then
        ' ''        Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
        ' ''    End If
        ' ''End If

        '' ''Chiamo TD0 per richiamare TD3 SPESE,TRASPORTO,TOTALE,SCADENZE AGGIORNARE OK FATTO
        ' ''strErrore = ""
        ' ''If _WucElement.CalcolaTotSpeseScad(dsDocDett, Iva, Imponibile, Imposta, _
        ' ''                      CInt(DecimaliVal), MoltiplicatoreValuta, _
        ' ''                      Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
        ' ''                      myTipoDoc, CDec(Abbuono), strErrore, TotaleLordoMercePL, Deduzioni) = False Then
        ' ''    If strErrore <> "" Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''        strErrore = ""
        ' ''    End If
        ' ''    Exit Function
        ' ''End If

    End Function
    'giu140319 ricalcolo tutte le righe di dettaglio in caso cambio lo sconto cassa o tipo pagamento
    Public Function AggImportiTot(ByRef strErrore As String) As Boolean
        Try
            AggImportiTot = False
            strErrore = ""
            'OBBLIGATORIO perche mi serve per il calcolo del TotaleLordoMerce Qta*Prezzo
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
                _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggImportiTot)")
                Exit Function
            End If
            'giu030512
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            'giu050220
            Dim myIDDurataNum As String = Session(IDDURATANUM)
            If IsNothing(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            '-
            Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
            If IsNothing(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            '-----------
            Dim dsDocDett As New DSDocumenti
            dsDocDett = Session("aDsDett")
            If myID = "" Or Not IsNumeric(myID) Then
                If (dsDocDett.Tables("ContrattiD").Rows.Count > 0) Then
                    If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti")) Then
                        myID = dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti").ToString.Trim
                    End If
                    If myIDDurataNum = "" Or Not IsNumeric(myIDDurataNum) Then
                        If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNum")) Then
                            myIDDurataNum = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNum").ToString.Trim
                        End If
                    End If
                    If myIDDurataNumR = "" Or Not IsNumeric(myIDDurataNumR) Then
                        If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNumRiga")) Then
                            myIDDurataNumR = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNumRiga").ToString.Trim
                        End If
                    End If
                End If
                If myID = "" Or Not IsNumeric(myID) Then
                    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggImportiTot)")
                    Exit Function
                End If
                Session(IDDOCUMENTI) = myID 'GIU060220
                Session(IDDURATANUM) = myIDDurataNum
                Session(IDDURATANUMRIGA) = myIDDurataNumR
            End If
            'giu140319 se richiamato se DSDETTAGLIO DA CAMBIO TIPO PAGAMENTO O ALTRO
            'Valuta per i decimali per il calcolo 
            Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliVal) Then DecimaliVal = ""
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
            '------------------------------------
            '---- Calcolo sconto su 
            Dim ScontoSuImporto As Boolean = True
            Dim ScCassaDett As Boolean = False 'giu010119
            Try
                ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
                ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
            Catch ex As Exception
                ScontoSuImporto = True
                ScCassaDett = False
            End Try
            'giu010119
            WucElement.GetDatiTB3()

            Dim ScCassa As String = Session(CSTSCCASSA)
            If IsNothing(ScCassa) Then ScCassa = "0"
            If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
            If Not IsNumeric(ScCassa) Then ScCassa = "0"
            '------------------------------------
            'ciclo di aggiornamento Importo dettagli
            If _WucElement.CKPrezzoALCSG(SWPrezzoAL, "") = False Then
                SWPrezzoAL = "L"
            End If
            Dim myQuantita As Decimal = 0
            Dim myPrezzoAL As Decimal = 0
            Dim strErroreAggAgente As String = ""
            Dim MySuperatoSconto As Boolean = False
            Dim myProvvAg As Decimal = 0
            Dim myCodArt As String = ""
            Dim CAge As Long = 0
            '-
            Dim RowDett As DSDocumenti.ContrattiDRow
            For Each RowDett In dsDocDett.ContrattiD
                If RowDett.RowState = DataRowState.Deleted Then
                    Continue For
                End If
                RowDett.BeginEdit()

                If RowDett.IsPrezzoNull Then RowDett.Prezzo = 0
                If RowDett.IsSconto_1Null Then RowDett.Sconto_1 = 0
                If RowDett.IsSconto_2Null Then RowDett.Sconto_2 = 0
                If IsDBNull(RowDett.Sconto_3) Then RowDett.Sconto_3 = 0
                If IsDBNull(RowDett.Sconto_4) Then RowDett.Sconto_4 = 0
                If IsDBNull(RowDett.Sconto_Pag) Then RowDett.Sconto_Pag = 0
                If IsDBNull(RowDett.ScontoValore) Then RowDett.ScontoValore = 0
                If IsDBNull(RowDett.Importo) Then RowDett.Importo = 0
                If IsDBNull(RowDett.Prezzo_Netto) Then RowDett.Prezzo_Netto = 0
                If RowDett.IsDedPerAccontoNull Then RowDett.DedPerAcconto = False
                If RowDett.IsSWCalcoloTotNull Then RowDett.SWCalcoloTot = False
                '-
                Select Case Left(myTipoDoc, 1)
                    Case "O"
                        myQuantita = RowDett.Qta_Ordinata
                    Case Else
                        If myTipoDoc = "PR" Or myTipoDoc = "PF" Or myTipoDoc = "TC" Or myTipoDoc = "CA" Then 'GIU021219
                            myQuantita = RowDett.Qta_Ordinata
                        Else
                            myQuantita = RowDett.Qta_Evasa
                        End If
                End Select
                If RowDett.SWCalcoloTot = True Then
                    myQuantita = 0
                End If
                RowDett.Importo = CalcolaImporto(RowDett.Prezzo, myQuantita, _
                                                 RowDett.Sconto_1, _
                                                 RowDett.Sconto_2, _
                                                 RowDett.Sconto_3, _
                                                 RowDett.Sconto_4, _
                                                 RowDett.Sconto_Pag, _
                                                 RowDett.ScontoValore, _
                                                 RowDett.Importo, _
                                                 ScontoSuImporto, _
                                                 CInt(DecimaliVal), _
                                                 RowDett.Prezzo_Netto, ScCassaDett, CDec(ScCassa), RowDett.DedPerAcconto) 'giu020519

                '-
                If SWPrezzoAL = "A" Then 'GIU170412
                    myPrezzoAL = RowDett.PrezzoAcquisto
                Else
                    myPrezzoAL = RowDett.PrezzoListino
                End If
                '-----------
                If RowDett.Prezzo > 0 Then
                    If myPrezzoAL = 0 Then myPrezzoAL = RowDett.Prezzo
                    If RowDett.Prezzo = myPrezzoAL Then
                        RowDett.ScontoReale = _
                           Math.Round(RowDett.Prezzo_Netto * 100 / RowDett.Prezzo, 4)
                        RowDett.ScontoReale = 100 - RowDett.ScontoReale
                    Else
                        RowDett.ScontoReale = _
                            Math.Round(RowDett.Prezzo_Netto * 100 / myPrezzoAL, 4)
                        RowDett.ScontoReale = 100 - RowDett.ScontoReale
                    End If
                Else
                    If myPrezzoAL = 0 Then
                        RowDett.ScontoReale = 0
                    Else
                        RowDett.ScontoReale = 100
                    End If
                End If
                '-
                strErroreAggAgente = ""
                If RowDett.IsCod_ArticoloNull Then
                    myCodArt = ""
                Else
                    myCodArt = RowDett.Cod_Articolo.Trim
                End If
                If RowDett.IsCod_AgenteNull Then
                    CAge = 0
                Else
                    CAge = CLng(RowDett.Cod_Agente)
                End If
                If RowDett.Qta_Ordinata <> 0 Or RowDett.Qta_Evasa <> 0 Or myQuantita <> 0 Or RowDett.Prezzo <> 0 Or _
                    RowDett.Prezzo_Netto <> 0 Or _
                    myCodArt.Trim <> "" Then

                    If CAge = 0 Then
                        RowDett.Pro_Agente = 0
                        RowDett.ImportoProvvigione = 0
                    Else
                        MySuperatoSconto = False
                        ' ''lblSuperatoScMax.Text = ""
                        ' ''lblSuperatoScMax.BorderStyle = BorderStyle.None
                        If Calcola_ProvvAgente(RowDett.Cod_Articolo.Trim, _
                            CLng(RowDett.Cod_Agente), myProvvAg, _
                            RowDett.ScontoReale, _
                            MySuperatoSconto, strErroreAggAgente) = False Then
                            RowDett.Pro_Agente = 0
                            RowDett.ImportoProvvigione = 0
                        Else
                            RowDett.Pro_Agente = myProvvAg
                            RowDett.ImportoProvvigione = RowDett.Importo * myProvvAg / 100
                        End If
                        ' ''If MySuperatoSconto = True Then
                        ' ''    lblSuperatoScMax.BorderStyle = BorderStyle.Outset
                        ' ''    lblSuperatoScMax.Text = "Superato Sc."
                        ' ''End If
                    End If
                Else
                    RowDett.SetCod_AgenteNull()
                    RowDett.Pro_Agente = 0
                    RowDett.ImportoProvvigione = 0
                End If
                RowDett.EndEdit()
            Next
            If (aDataView1 Is Nothing) Then
                aDataView1 = New DataView(dsDocDett.ContrattiD)
            End If
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            Session("aDataView1") = aDataView1
            Session("aDsDett") = dsDocDett
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
            EnableTOTxtInsArticoli(False)

            AggImportiTot = AggiornaImportoALLAtt(dsDocDett, strErrore)
        Catch ex As Exception
            strErrore = "Errore AggImportiTot: " & ex.Message.Trim
        End Try
    End Function

    Public Sub SetEnableTxt(ByVal Valore As Boolean)
        btnDelDettagli.Enabled = Valore

        If Valore Then
            If DDLTipoDettagli.SelectedIndex <> 0 Then
                DDLModello.Enabled = False
                btnDelDestD.Enabled = False
                btnCercaDestD.Enabled = False
                DDLRespAreaApp.Enabled = False
                DDLRespVisiteApp.Enabled = False
            Else
                DDLModello.Enabled = True 'giu100222 
                btnDelDestD.Enabled = True
                btnCercaDestD.Enabled = True
                DDLRespAreaApp.Enabled = True
                DDLRespVisiteApp.Enabled = True
            End If
        Else
            DDLModello.Enabled = False
            btnDelDestD.Enabled = False
            btnCercaDestD.Enabled = False
            DDLRespAreaApp.Enabled = False
            DDLRespVisiteApp.Enabled = False
        End If
        DDLTipoDettagli.Enabled = Not Valore : DDLDurNumRIga.Enabled = Not Valore
        GridViewDett.EditIndex = -1
        GridViewDett.Enabled = Valore
        btnPrimaRiga.Enabled = Valore
        BtnSelArticolo.Enabled = Valore
        EnableTOTxtInsArticoli(Valore)

        'GIU061211 btnAggArtGridSel.Enabled = False 'giu061211 uso sempre aggriga :: btnInsRigaDett.Enabled = False
        If Valore = True Then
            EnableTOTxtInsArticoli(False)
        End If
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            AzzeraTxtInsArticoli()
            '''BuildLottiRigaDB(-1)
            Exit Sub
        Else
            SetBtnPrimaRigaEnabled(False)
        End If
        'GIU071223
        If Valore = False Then
            Exit Sub
        End If
        Dim RigaSel As Integer = 0
        Try
            RigaSel = GridViewDett.SelectedIndex 'giu180419
            If RigaSel = 0 Then 'giu180419
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then 'giu180419
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            LblTotale.Text = HTML_SPAZIO : lblTotDett.Text = HTML_SPAZIO 'giu180420
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
    End Sub
#End Region

    Private Sub txtCodArtIns_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArtIns.TextChanged

        If LeggiArticolo(txtCodArtIns.Text.Trim) = False Then
            txtCodArtIns.BackColor = SEGNALA_KO
            txtDesArtIns.Focus()
        Else
            txtCodArtIns.BackColor = SEGNALA_OK
            Dim listaCodiciArticolo As New List(Of String)
            listaCodiciArticolo.Add(txtCodArtIns.Text.Trim)
            Session(ARTICOLI_DA_INS) = listaCodiciArticolo
            Session("ckNoDesArtEst") = "SI"
            CallBackWFPArticoloSel(True)
            EnableTOTxtInsArticoli(True)
            '---------------------------------------------------------------------------
            txtDesArtIns.Focus()
        End If
        'alb060213 inserito controllo perchè si scatena l'evento dopo aver inserito gli articoli dal popup e non permette più l'aggiornamento
        If sender.enabled = True Then
            Session(SWOPDETTDOCR) = SWMODIFICATO
        End If
        'alb060213 END
    End Sub
    Private Function LeggiArticolo(ByVal myCodArt As String, Optional ByRef myAnaMag As AnaMagEntity = Nothing) As Boolean
        If myCodArt.Trim = "" Then
            LeggiArticolo = True
            AzzeraTxtInsArticoli()
            Exit Function
        End If
        Dim AMSys As New AnaMag
        Dim myAM As AnaMagEntity
        Dim arrAM As ArrayList

        Try
            arrAM = AMSys.getAnaMagByCodice(myCodArt.Trim)
            If (arrAM.Count > 0) Then
                myAM = CType(arrAM(0), AnaMagEntity)
                LeggiArticolo = GetArticolo(myAM)
            Else
                LeggiArticolo = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in LeggiArticolo", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            LeggiArticolo = False
        End Try
    End Function
    Private Function GetArticolo(ByVal AnaMag As AnaMagEntity) As Boolean
        GetArticolo = True 'GIU180419
        Dim ObjDB As New DataBaseUtility

        'Regime IVA
        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        Try
            txtCodArtIns.AutoPostBack = False
            txtIVAIns.AutoPostBack = False
            txtCodArtIns.Text = AnaMag.CodArticolo
            lblBase.Text = AnaMag.LBase
            lblOpz.Text = AnaMag.LOpz
            txtDesArtIns.Text = AnaMag.Descrizione
            txtUMIns.Text = AnaMag.Um
            txtQtaIns.Text = "1"
            lblQtaEv.Text = "0"
            LblQtaRe.Text = "1"
            If CInt(RegimeIVA) > 49 Then
                txtIVAIns.Text = CInt(RegimeIVA)
            Else
                txtIVAIns.Text = AnaMag.CodIva
            End If
            txtCodArtIns.AutoPostBack = True
            txtIVAIns.AutoPostBack = True
            'giu091211 per i documenti OF e DF
            Dim myPrezzoAcquisto As Decimal = AnaMag.PrezzoAcquisto
            Dim myPrezzoListino As Decimal = 0
            Dim MyPrezzoAL As Decimal = 0
            Dim mySconto1 As Decimal = 0 : Dim mySconto2 As Decimal = 0
            'GIU070115 ScFornitore mentre sconto2 è sempre uguale a 0
            Dim myScFornitore As Decimal = AnaMag.ScFornitore
            '--------------------------------------------------------
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
                GetArticolo = False
                _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggiornaImporto)")
                Exit Function
            End If
            'GIU180412 
            SWPrezzoAL = "L" 'Listino
            Dim strErrore As String = ""
            If _WucElement.CKPrezzoALCSG(SWPrezzoAL, strErrore) = False Then
                GetArticolo = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in GetArticolo.CKPrezzoALCSG", strErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
            '--
            Dim IDLT As String = Session(IDLISTINO)
            If IsNothing(IDLT) Then IDLT = "1"
            If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
            'giu080115 di seguito rileggo anamag mentre l'ho gia' letto prima
            ' ''If Documenti.GetPrezziListinoAcquisto(myTipoDoc, IDLT, AnaMag.CodArticolo, _
            ' ''myPrezzoListino, myPrezzoAcquisto, _
            ' ''mySconto1, mySconto2, strErrore) = False Then
            If GetPrezziListVenD(myTipoDoc, IDLT, AnaMag.CodArticolo, _
                                 myPrezzoListino, mySconto1, mySconto2, strErrore) = False Then
                GetArticolo = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in GetArticolo.GetPrezziListVenD", strErrore, WUC_ModalPopup.TYPE_ALERT)
                AzzeraTxtInsArticoli()
                Exit Function
            ElseIf strErrore.Trim <> "" Then
                GetArticolo = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in GetArticolo.GetPrezziListVenD", strErrore, WUC_ModalPopup.TYPE_ALERT)
                AzzeraTxtInsArticoli()
                Exit Function
            End If
            '--
            If SWPrezzoAL = "A" Then 'GIU170412
                MyPrezzoAL = myPrezzoAcquisto
                mySconto1 = 0 : mySconto2 = 0
                'giu080115 sconto del fornitore da AnaMag
                mySconto1 = myScFornitore
            Else
                MyPrezzoAL = myPrezzoListino
            End If
            '---------
            '@@@@@@@@@
            If chkNoPrezzo.Checked = False Then
                txtPrezzoIns.Text = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                lblPrezzoAL.ToolTip = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            Else
                txtPrezzoIns.Text = ""
                lblPrezzoAL.ToolTip = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            End If
            txtSconto1Ins.Text = FormattaNumero(mySconto1)
            LblPrezzoNetto.Text = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            LblImportoRiga.Text = Format(MyPrezzoAL, FormatoValEuro)
            GetArticolo = True
        Catch ex As Exception
            AzzeraTxtInsArticoli()
            GetArticolo = False
        End Try

    End Function
    Private Sub BtnSelArticoloIns_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnSelArticoloIns.Click
        WFP_Articolo_Seleziona1.WucElement = Me
        WFP_Articolo_Seleziona1.DeselezionaTutti()
        WFP_Articolo_Seleziona1.setckNoDesArtEst(True)
        Dim strMessErr As String = ""
        If WFP_Articolo_Seleziona1.PopolaGriglia(strMessErr) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", strMessErr, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_Seleziona1.Show()
    End Sub

    Private Sub txtArtIns_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        txtDesArtIns.TextChanged, txtPrezzoIns.TextChanged, txtUMIns.TextChanged, txtQtaIns.TextChanged, _
        txtSconto1Ins.TextChanged
        'alb060213 inserito controllo perchè si scatena l'evento dopo aver inserito gli articoli dal popup e non permette più l'aggiornamento
        If sender.enabled = True Then
            Session(SWOPDETTDOCR) = SWMODIFICATO
        End If
        'alb060213 END
    End Sub

    Private Sub txtIVAIns_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        txtIVAIns.TextChanged
        If txtIVAIns.Text.Trim = "" Or txtIVAIns.Text.Trim = "0" Then
            txtIVAIns.AutoPostBack = False
            txtIVAIns.Text = "0"
            txtIVAIns.AutoPostBack = True
            lblMessAgg.Text = ""
            lblMessAgg.BorderStyle = BorderStyle.None
            txtIVAIns.BackColor = Def.SEGNALA_OK
            txtPrezzoIns.Focus()
            Exit Sub
        End If
        'alb060213 inserito controllo perchè si scatena l'evento dopo aver inserito gli articoli dal popup e non permette più l'aggiornamento
        If sender.enabled = True Then
            Session(SWOPDETTDOCR) = SWMODIFICATO
        End If
        'alb060213 END
        Dim Comodo As String = App.GetValoreFromChiave(txtIVAIns.Text, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
        lblMessAgg.BorderStyle = BorderStyle.Outset
        If Comodo.Trim = "" Then
            lblMessAgg.Text = "IVA errata !!"
            txtIVAIns.BackColor = Def.SEGNALA_KO
        Else
            lblMessAgg.Text = Comodo.Trim
            txtIVAIns.BackColor = Def.SEGNALA_OK
            CheckRIVA()
        End If
        txtPrezzoIns.Focus()
    End Sub
    Private Function CheckRIVA() As Boolean
        'controlo sul REGIME IVA
        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        Dim Comodo As String = App.GetValoreFromChiave(RegimeIVA, Def.ALIQUOTA_IVA, Session(ESERCIZIO))
        If CInt(RegimeIVA) > 49 Then
            If CInt(txtIVAIns.Text.Trim) < 50 Then
                lblMessAgg.BorderStyle = BorderStyle.Outset
                lblMessAgg.Text = "IVA errata per il Regime IVA " & RegimeIVA & " - " & Comodo
                txtIVAIns.BackColor = Def.SEGNALA_KO
            End If
        End If
    End Function

    'Fabio18042016
#Region "CaricoLottiNSerie"

    '''Private Sub btnCaricoLotti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCaricoLotti.Click

    '''    If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
    '''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '''        ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
    '''        Exit Sub
    '''    End If

    '''    Dim myID As String = Session(IDDOCUMENTI)
    '''    If IsNothing(myID) Then
    '''        myID = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '''        ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
    '''        Exit Sub
    '''    End If
    '''    If String.IsNullOrEmpty(myID) Then
    '''        myID = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '''        ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
    '''        Exit Sub
    '''    End If
    '''    'giu050220
    '''    Dim myIDDurataNum As String = Session(IDDURATANUM)
    '''    If IsNothing(myIDDurataNum) Then
    '''        myIDDurataNum = "0"
    '''    End If
    '''    If String.IsNullOrEmpty(myIDDurataNum) Then
    '''        myIDDurataNum = "0"
    '''    End If
    '''    '-
    '''    Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
    '''    If IsNothing(myIDDurataNumR) Then
    '''        myIDDurataNumR = "0"
    '''    End If
    '''    If String.IsNullOrEmpty(myIDDurataNumR) Then
    '''        myIDDurataNumR = "0"
    '''    End If
    '''    '----------
    '''    Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)

    '''    WFPLottiInsert1.setValoriRiga(txtCodArtIns.Text, txtDesArtIns.Text, txtIVAIns.Text, txtPrezzoIns.Text, txtQtaEv.Text, txtQtaIns.Text, txtSconto1Ins.Text, txtUMIns.Text, LblPrezzoNetto.Text, LblImportoRiga.Text, lblGiacenza.Text, lblGiacImp.Text, lblOrdFor.Text, lblDataArr.Text, lblRigaSel.Text.Trim, lblLabelQtaRe.Text)
    '''    WFPLottiInsert1.WucElement = Me
    '''    WFPLottiInsert1.SetLblMessUtente("Seleziona lotti da caricare")
    '''    Session(F_CARICALOTTI) = True
    '''    WFPLottiInsert1.Show(True)

    '''End Sub

    '''Public Sub PostBackLottiInsert()
    '''    BuildLottiRigaDB(lblRigaSel.Text.Trim)
    '''End Sub

#End Region

    'giu080319
    Public Function PopolaLBLTotaliDoc(ByVal _dvDocT As DataView, ByRef strErrore As String) As Boolean
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = "2"
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If

        LblTotale.Text = FormattaNumero(IIf(IsDBNull(_dvDocT.Item(0).Item("Totale")), 0, _dvDocT.Item(0).Item("Totale")), Int(DecimaliVal))
        'GIU110322
        If Not IsDBNull(_dvDocT.Item(0).Item("NoteRitiro")) Then
            txtNoteInterventoALL.Text = _dvDocT.Item(0).Item("NoteRitiro").ToString.Trim
        Else
            txtNoteInterventoALL.Text = ""
        End If
        Session(L_NSERIELOTTO) = Nothing
    End Function

    Public Function AzzeraLBLTotaliDoc() As Boolean
        LblTotale.Text = HTML_SPAZIO
        lblTotDett.Text = HTML_SPAZIO
        'GIU090322 GIU110322
        txtNoteInterventoALL.Text = ""
        Session(L_NSERIELOTTO) = Nothing
    End Function

    'giu080220 operazioni su DurataNum e DurataNumRiga
    Private Function SetDDLDettDurNumRiga(Optional ByVal SWNuovaApp As Boolean = False) As Boolean
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then myID = "-1"
        ' ''If Not IsNumeric(myID) Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Function
        ' ''End If
        '------------
        SetDDLDettDurNumRiga = True
        DDLDurNumRIga.Items.Clear()
        If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
            Dim strSQL As String = ""
            Dim ObjDB As New DataBaseUtility
            Dim dsArt As New DataSet
            'GIU210420 ERR MANCAVA RIGA=1
            strSQL = "Select DurataNumRiga, Serie From ContrattiD WHERE IDDocumenti=" & myID.Trim & " AND DurataNum=0 GROUP BY DurataNumRiga, Serie ORDER BY DurataNumRiga"
            Dim strComodo As String = ""
            Try
                Dim myRighe As Integer = 0
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
                If (dsArt.Tables.Count > 0) Then
                    If (dsArt.Tables(0).Rows.Count > 0) Then
                        myRighe = dsArt.Tables(0).Rows.Count
                    End If
                End If
                Dim i As Integer = 0
                If myRighe > 0 Then
                    For i = 0 To myRighe - 1
                        strComodo = IIf(IsDBNull(dsArt.Tables(0).Rows(i).Item("Serie")), "[????]", dsArt.Tables(0).Rows(i).Item("Serie"))
                        If strComodo <> "[????]" Then strComodo = FormattaNomeFile(strComodo) 'giu180723
                        DDLDurNumRIga.Items.Add((i + 1).ToString.Trim & " - " & strComodo)
                        DDLDurNumRIga.Items(i).Value = i
                    Next i
                    DDLDurNumRIga.Items.Add((i + 1).ToString.Trim & " - [Nuova]")
                    DDLDurNumRIga.Items(i).Value = i
                    If SWNuovaApp = False Then
                        DDLDurNumRIga.SelectedIndex = 0
                    Else
                        DDLDurNumRIga.SelectedValue = myRighe - 1
                    End If
                Else
                    DDLDurNumRIga.Items.Add("1 - [Nuova]")
                    DDLDurNumRIga.Items(0).Value = 0
                End If
                'GIU110322
                DDLDurNumRIga.SelectedIndex = 0
                '-NoteIntervento giu090322
                If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
                    Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
                    If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
                        txtNoteIntervento.Text = ""
                        Exit Function
                    End If
                    NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
                    txtNoteIntervento.Text = GetNoteSL(txtNoteInterventoALL.Text.Trim, NSL.Trim)
                Else
                    txtNoteIntervento.Text = ""
                End If
            Catch Ex As Exception
                SetDDLDettDurNumRiga = False
                _WucElement.SetTxtlblMessDoc("Errore in ContrattiDett.SetDDLDettDurNumRiga - Lettura articoli: " & Ex.Message)
                Exit Function
            End Try
        Else
            Dim myDurataNum As String = Session(CSTDURATANUM)
            If IsNothing(myDurataNum) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDurataNum) Then
                Exit Function
            ElseIf Not IsNumeric(myDurataNum) Then
                Exit Function
            ElseIf Val(myDurataNum) = 0 Then
                Exit Function
            End If
            '-
            Dim myDurataTipo As String = Session(CSTDURATATIPO)
            If IsNothing(myDurataTipo) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDurataTipo) Then
                Exit Function
            ElseIf myDurataTipo.Trim = "" Then
                Exit Function
            End If
            '-
            Dim myDataInizio As String = Session(CSTDATAINIZIO)
            If IsNothing(myDataInizio) Then
                Exit Function
            End If
            If String.IsNullOrEmpty(myDataInizio) Then
                Exit Function
            ElseIf Not IsDate(myDataInizio.Trim) Then
                Exit Function
            End If
            '----------
            If myDurataTipo.Trim = "M" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "T" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "Q" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "S" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio), FormatoData))
                    DDLDurNumRIga.Items(i).Value = i
                    myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "A" Then
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add(Format(CDate(myDataInizio).Year, "0000"))
                    DDLDurNumRIga.Items(i).Value = i
                    myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
                Next i
            Else 'non capitera' mai
                For i = 0 To Val(myDurataNum) - 1
                    DDLDurNumRIga.Items.Add((i + 1).ToString.Trim & " - ????")
                    DDLDurNumRIga.Items(i).Value = i
                Next i
            End If
            DDLDurNumRIga.SelectedIndex = 0
        End If
    End Function
    Private Sub DDLTipoDettagli_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTipoDettagli.SelectedIndexChanged
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedIndex
        Session(IDDURATANUMRIGA) = "0"
        Call SetDDLDettDurNumRiga()
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SetCdmDAdp()
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Sessione Scaduta, - Riprovate la modifica uscendo e rientrando.", "Lettura contratti. (Identificativo sconosciuto)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
        DsContrattiDett.ContrattiD.Clear()
        SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
        AzzQtaNULL()
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '---
        Call AzzeraTxtInsArticoli()
        '---
        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
        GridViewDett.DataSource = aDataView1
        Session("aDataView1") = aDataView1
        GridViewDett.DataBind()
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            Dim newRowDocD As DSDocumenti.ContrattiDRow = DsContrattiDett.ContrattiD.NewContrattiDRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
                .DurataNum = Val(myIDDurataNum)
                .DurataNumRiga = Val(myIDDurataNumR)
                .Riga = 1
                .Prezzo = 0
                .Prezzo_Netto = 0
                .Qta_Allestita = 0
                .Qta_Evasa = 0
                .Qta_Impegnata = 0
                .Qta_Ordinata = 0
                .Qta_Prenotata = 0
                .Qta_Residua = 0
                .Importo = 0
                'giu170412
                .PrezzoAcquisto = 0
                .PrezzoListino = 0
                'giu190412
                .SWModAgenti = False
                .PrezzoCosto = 0
                .Qta_Inviata = 0
                .Qta_Fatturata = 0
                '---------
                .EndEdit()
            End With
            Try
                DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                newRowDocD = Nothing
                SqlAdapDocDett = Session("aSqlAdap")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in (DDLTipoDettagli_SelectedIndexChanged)", "Lettura contratti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            Session("aDataView1") = aDataView1
            GridViewDett.DataBind()
            SetBtnPrimaRigaEnabled(False)
            '------------------------------------------------------------------------------------
            lblTotDett.Text = HTML_SPAZIO
            'GIU17042020 QUI NON AGG. XKE SONO NEL SINGOLO APP.PERIODO _WucElement.SetLblTotLMPL(0, 0)
        Else
            SetBtnPrimaRigaEnabled(False)
            'GIU17042020 QUI NON AGG. 'giu180420 non agg il TOTALE DOC. XKE SONO NEL SINGOLO APP.PERIODO
            Dim TotaleDett As Decimal = 0
            For Each rsDettagli In DsContrattiDett.ContrattiD.Select("", "Riga")
                rsDettagli.BeginEdit()
                If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                'giu180723
                If IsDBNull(rsDettagli!Serie) Then
                    rsDettagli!Serie = ""
                Else
                    rsDettagli!Serie = FormattaNomeFile(rsDettagli!Serie.ToString.Trim)
                End If
                '-
                If IsDBNull(rsDettagli!Lotto) Then
                    rsDettagli!Lotto = ""
                Else
                    rsDettagli!Lotto = FormattaNomeFile(rsDettagli!Lotto.ToString.Trim)
                End If
                '-
                If IsDBNull(rsDettagli!SerieLotto) Then
                    rsDettagli!SerieLotto = ""
                Else
                    rsDettagli!SerieLotto = FormattaNomeFile(rsDettagli!SerieLotto.ToString.Trim)
                End If
                '---------
                rsDettagli.EndEdit()
                rsDettagli.AcceptChanges()
                If rsDettagli!DedPerAcconto = True Then
                    ' ''TotaleDeduzioni += rsDettagli![Importo]
                Else
                    TotaleDett += rsDettagli![Importo]
                End If
            Next
            'Valuta per i decimali per il calcolo
            Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliVal) Then DecimaliVal = "2"
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
            lblTotDett.Text = FormattaNumero(TotaleDett, Int(DecimaliVal))

        End If
        '--- 
        Dim RigaSel As Integer = 0
        Try
            GridViewDett.SelectedIndex = 0
            RigaSel = GridViewDett.SelectedIndex
            If RigaSel = 0 Then
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            Call AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
        EnableTOTxtInsArticoli(False)
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        If DDLTipoDettagli.SelectedIndex = 0 Then
            If GridViewDett.Rows.Count = 0 Then
                _WucElement.SetBtnDupGen(False)
            Else
                _WucElement.SetBtnDupGen(True)
            End If
            PanelDettArtNoteInterv.Visible = True
        Else
            _WucElement.SetBtnDupGen(False)
            PanelDettArtNoteInterv.Visible = False
        End If
    End Sub

    Private Sub DDLDurNumRIga_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLDurNumRIga.SelectedIndexChanged
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedIndex
        Session(IDDURATANUMRIGA) = DDLDurNumRIga.SelectedIndex
        DDLDurNumRIga.BackColor = SEGNALA_OK
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SetCdmDAdp()
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Sessione Scaduta, - Riprovate la modifica uscendo e rientrando.", "Lettura contratti. (Identificativo sconosciuto)", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = Val(myIDDurataNum)
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = Val(myIDDurataNumR)
        DsContrattiDett.ContrattiD.Clear()
        SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
        AzzQtaNULL()
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsContrattiDett
        '---
        Call AzzeraTxtInsArticoli()
        '---
        aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
        GridViewDett.DataSource = aDataView1
        Session("aDataView1") = aDataView1
        GridViewDett.DataBind()
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            Dim newRowDocD As DSDocumenti.ContrattiDRow = DsContrattiDett.ContrattiD.NewContrattiDRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
                .DurataNum = Val(myIDDurataNum)
                .DurataNumRiga = Val(myIDDurataNumR)
                .Riga = 1
                .Prezzo = 0
                .Prezzo_Netto = 0
                .Qta_Allestita = 0
                .Qta_Evasa = 0
                .Qta_Impegnata = 0
                .Qta_Ordinata = 0
                .Qta_Prenotata = 0
                .Qta_Residua = 0
                .Importo = 0
                'giu170412
                .PrezzoAcquisto = 0
                .PrezzoListino = 0
                'giu190412
                .SWModAgenti = False
                .PrezzoCosto = 0
                .Qta_Inviata = 0
                .Qta_Fatturata = 0
                '---------
                .EndEdit()
            End With
            Try
                DsContrattiDett.ContrattiD.AddContrattiDRow(newRowDocD)
                newRowDocD = Nothing
                SqlAdapDocDett = Session("aSqlAdap")
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in (DDLDurNumRiga_SelectedIndexChanged)", "Lettura contratti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsContrattiDett
            Session("aDataView1") = aDataView1
            GridViewDett.DataBind()
            SetBtnPrimaRigaEnabled(False)
            '------------------------------------------------------------------------------------
            lblTotDett.Text = HTML_SPAZIO
            'GIU17042020 QUI NON AGG. XKE SONO NEL SINGOLO APP.PERIODO _WucElement.SetLblTotLMPL(0,  0)
        Else
            SetBtnPrimaRigaEnabled(False)
            If DDLTipoDettagli.SelectedIndex = 0 Then
                _WucElement.SetBtnDupGen(True)
            Else
                _WucElement.SetBtnDupGen(False)
            End If
            'GIU17042020 QUI NON AGG. 'giu180420 non agg il TOTALE DOC. XKE SONO NEL SINGOLO APP.PERIODO
            Dim TotaleDett As Decimal = 0
            For Each rsDettagli In DsContrattiDett.ContrattiD.Select("", "Riga")
                rsDettagli.BeginEdit()
                If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                'giu180723
                If IsDBNull(rsDettagli!Serie) Then
                    rsDettagli!Serie = ""
                Else
                    rsDettagli!Serie = FormattaNomeFile(rsDettagli!Serie.ToString.Trim)
                End If
                '-
                If IsDBNull(rsDettagli!Lotto) Then
                    rsDettagli!Lotto = ""
                Else
                    rsDettagli!Lotto = FormattaNomeFile(rsDettagli!Lotto.ToString.Trim)
                End If
                '-
                If IsDBNull(rsDettagli!SerieLotto) Then
                    rsDettagli!SerieLotto = ""
                Else
                    rsDettagli!SerieLotto = FormattaNomeFile(rsDettagli!SerieLotto.ToString.Trim)
                End If
                '---------
                rsDettagli.EndEdit()
                rsDettagli.AcceptChanges()
                If rsDettagli!DedPerAcconto = True Then
                    ' ''TotaleDeduzioni += rsDettagli![Importo]
                Else
                    TotaleDett += rsDettagli![Importo]
                End If
            Next
            'Valuta per i decimali per il calcolo
            Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliVal) Then DecimaliVal = "2"
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
            lblTotDett.Text = FormattaNumero(TotaleDett, Int(DecimaliVal))
        End If
        '--- 
        Dim RigaSel As Integer = 0
        Try
            GridViewDett.SelectedIndex = 0
            RigaSel = GridViewDett.SelectedIndex
            If RigaSel = 0 Then
                RigaSel = GridViewDett.SelectedDataKey.Value
            End If
            If RigaSel > 0 Then
                PopolaTxtDett()
            Else
                AzzeraTxtInsArticoli()
            End If
        Catch ex As Exception
            RigaSel = 0
            Call AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        '''BuildLottiRigaDB(RigaSel)
        '---------
        EnableTOTxtInsArticoli(False)
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        If chkSelModifica.Checked Or GridViewDett.Rows.Count = 0 Then
            Call _WucElement.CallBtnModifica()
        Else
            Dim myRowIndex As Integer = GridViewDett.SelectedIndex + (GridViewDett.PageSize * GridViewDett.PageIndex)
            Dim myCodArt As String = ""
            Try
                myCodArt = aDataView1.Item(myRowIndex).Item("Cod_Articolo")
            Catch ex As Exception
                myCodArt = ""
            End Try
            If myCodArt = "" Then
                Call _WucElement.CallBtnModifica()
                Call _WucElement.SetLblMessDoc("Attenzione: Errore in " & DDLTipoDettagli.SelectedItem.Text.Trim & " - Nessun dettaglio presente.")
            End If
        End If
        'giu090322
        '-NoteIntervento giu090322
        If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
            Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
            If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
                txtNoteIntervento.Text = ""
                Exit Sub
            End If
            NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
            txtNoteIntervento.Text = GetNoteSL(txtNoteInterventoALL.Text.Trim, NSL.Trim)
        Else
            txtNoteIntervento.Text = ""
        End If

    End Sub
    'giu090322 giu070523
    Private Function GetNoteSL(ByVal pNoteRitiro As String, ByVal pNSL As String) As String
        GetNoteSL = ""
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        Session(L_NSERIELOTTO) = Nothing
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        Dim SWTrovato As Boolean = False 'giu301123 se non è presente nella lista note l'ho inserisco
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I)) 'giu070523
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
                If mySL.Trim = pNSL.Trim Then
                    GetNoteSL = myNoteRitiro.Trim
                    SWTrovato = True
                End If
            Next
            If SWTrovato = False Then
                ListaSL.Add(pNSL.Trim + "§" + "")
            End If
            Session(L_NSERIELOTTO) = ListaSL
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            Call SetNoteSLALLApp(pNoteRitiro)
            GetNoteSL = pNoteRitiro.Trim
        End If
    End Function
    Private Function SetNoteSLALLApp(ByVal pNoteRitiro As String) As Boolean
        SetNoteSLALLApp = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Serie,'') + ISNULL(Lotto,'') AS SerieLotto From ContrattiD " & _
                 "WHERE (IDDocumenti = " + myID.Trim + ") AND (DurataNum =0) " & _
                 "GROUP BY ISNULL(Serie,'') + ISNULL(Lotto,'')"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim ListaSL As New List(Of String)
        Dim mySL As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        mySL = Formatta.FormattaNomeFile(IIf(IsDBNull(row.Item("SerieLotto")), "", row.Item("SerieLotto"))) 'giu070523
                        If mySL.Trim <> "" Then
                            ListaSL.Add(mySL + "§" + pNoteRitiro.Trim)
                        End If
                    Next
                Else
                    'per adesso proseguo
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): Nennuna App.presente", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                'per adesso proseguo
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): Nennuna App.presente", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            'per adesso proseguo
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        SetNoteSLALLApp = True
        Session(L_NSERIELOTTO) = ListaSL

    End Function
    '------------------------------------------------------------------------------------------------------------------------------------------
    Public Function CKGridDett() As Boolean
        If GridViewDett.Rows.Count = 0 Then
            CKGridDett = False
        Else
            CKGridDett = True
        End If
    End Function
    '-
    Private Function CHECKSerieLotto(ByVal pID As String, ByVal pIDDurataNumR As String, ByVal pSerie As String, ByVal pLotto As String) As Boolean
        CHECKSerieLotto = False
        If String.IsNullOrEmpty(pID) Then Exit Function
        If String.IsNullOrEmpty(pIDDurataNumR) Then Exit Function
        If Not IsNumeric(pID) Then Exit Function
        If Not IsNumeric(pIDDurataNumR) Then Exit Function
        If String.IsNullOrEmpty(pSerie) Then pSerie = ""
        If String.IsNullOrEmpty(pLotto) Then pLotto = ""
        If pSerie.Trim = "" And pLotto.Trim = "" Then
            Return False
            Exit Function
        End If
        '-
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        strSQL = "SELECT Lotto, Serie, IDDocumenti, DurataNum, DurataNumRiga " & _
                    "FROM ContrattiD " & _
                    "WHERE (IDDocumenti = " & pID.Trim & ") AND (DurataNum = 0) AND " & _
                    "(DurataNumRiga <> " & pIDDurataNumR.Trim & ") AND (ISNULL(Serie,'') = N'" & pSerie.Trim & "') AND " & _
                    "(ISNULL(Lotto,'') = N'" & pLotto.Trim & "')"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    CHECKSerieLotto = True
                End If
            End If
        Catch Ex As Exception
            CHECKSerieLotto = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.CHECKSerieLotto", "Lettura contratti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Public Function GetDDLTipoDettagliID() As Integer
        GetDDLTipoDettagliID = DDLTipoDettagli.SelectedIndex
    End Function
    Public Function GetDDLModello(ByRef Modello As Integer) As Integer
        Try
            GetDDLModello = DDLTipoDettagli.SelectedIndex
            Modello = DDLModello.SelectedIndex
        Catch ex As Exception
            GetDDLModello = -1
            Modello = -1
        End Try

    End Function

    Public Function DuplicaApp() As Boolean
        DuplicaApp = False
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedIndex
        Session(IDDURATANUMRIGA) = DDLDurNumRIga.SelectedIndex

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DuplicaApp:", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'VERIFICO CHE ALMENO CI SIA UNA RIGA VALIDA
        Dim strSQL As String = ""
        Dim myRigaMax As Integer = -1
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        strSQL = "SELECT MAX(Riga) FROM ContrattiD WHERE (IDDocumenti = " & myID.Trim & ") AND (DurataNum =" & myIDDurataNum.Trim & ") AND (DurataNumRiga =" & myIDDurataNumR.Trim & ")"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    myRigaMax = IIf(IsDBNull(dsArt.Tables(0).Rows(0).Item(0)), -1, dsArt.Tables(0).Rows(0).Item(0))
                    If myRigaMax = -1 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Duplica Apparecchiatura", "Nessuna riga presente da duplicare.", WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                    myRigaMax += 1
                    'OK
                    strSQL = "EXEC DuplicaApp " & myID.Trim & "," & myIDDurataNum.Trim & "," & myIDDurataNumR.Trim
                    If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore in DuplicaApp", strSQL, WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                    'giu270220
                    strSQL = "UPDATE ContrattiT SET StatoDoc=5 WHERE IDDocumenti=" & myID.Trim
                    If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore in DuplicaApp Cambio stato doc.=5", strSQL, WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Duplica Apparecchiatura", "Nessuna riga presente da duplicare.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DuplicaApp", "Lettura/duplica App. contratti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '-------
        EnableTOTxtInsArticoli(False)
        DuplicaApp = True
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        Call SetDDLDettDurNumRiga(True)
        '-ok
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Duplica apparecchiatura", "Operazione terminata con successo.", WUC_ModalPopup.TYPE_INFO)
        Call DDLDurNumRIga_SelectedIndexChanged(Nothing, Nothing)
    End Function
    '-
    Public Function GeneraAttPeriodo() As Boolean
        GeneraAttPeriodo = False
        DDLTipoDettagli.SelectedIndex = 0 'giu200124 parto sempre dalle apparecchiature
        Session(IDDURATANUM) = DDLTipoDettagli.SelectedIndex
        Session(IDDURATANUMRIGA) = DDLDurNumRIga.SelectedIndex

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in GeneraAttPeriodo:", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'CARICO TUTTE LE APP.
        SetCdmDAdp()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = DBNull.Value  ' 0 giu131223 carico anche i periodi nel caso in cui sia un aggiunta  0 'fisso per le apparecchiature
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
        DsContrattiDett.ContrattiD.Clear()
        SqlAdapDocDett.Fill(DsContrattiDett.ContrattiD)
        If Me.DsContrattiDett.ContrattiD.Select("").Count = 0 Then
            Session(CSTNONCOMPLETO) = SWSI
            Call SetStatoDoc5()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("GeneraAttPeriodo:", "Nessuna apparecchiatura presente in archivio.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim myErrore As String = ""
        If GetAnniScAzzQtaNULL(myErrore) = False Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("GeneraAttPeriodo:", "Errore nella verifica attività apparecchiatura." & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        DsContrattiDett.ContrattiD.AcceptChanges()
        '--------------------
        'prendo la selectionindex da DDL DELL'ANNO FINO ALLA FINE
        Dim myDurataNum As String = Session(CSTDURATANUM)
        If IsNothing(myDurataNum) Then
            myDurataNum = ""
        ElseIf String.IsNullOrEmpty(myDurataNum) Then
            myDurataNum = ""
        ElseIf Not IsNumeric(myDurataNum) Then
            myDurataNum = ""
        ElseIf Val(myDurataNum) = 0 Then
            myDurataNum = ""
        End If
        If myDurataNum = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera attività per periodo.", "Attenzione, non è stata definita la durata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim myDurataTipo As String = Session(CSTDURATATIPO)
        If IsNothing(myDurataTipo) Then
            myDurataTipo = ""
        ElseIf String.IsNullOrEmpty(myDurataTipo) Then
            myDurataTipo = ""
        ElseIf myDurataTipo.Trim = "" Then
            myDurataTipo = ""
        End If
        If myDurataTipo = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera attività per periodo.", "Attenzione, non è stata definita il tipo di durata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'giu281023 Unica Rata 1° periodo
        Dim TipoFatt As String = Session(CSTTIPOFATT)
        If String.IsNullOrEmpty(TipoFatt) Then TipoFatt = ""
        If TipoFatt = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera attività per periodo.", "Tipo Fatturazione non definito.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        '-
        Dim myDataInizio As String = Session(CSTDATAINIZIO)
        If IsNothing(myDataInizio) Then
            myDataInizio = ""
        ElseIf String.IsNullOrEmpty(myDataInizio) Then
            myDataInizio = ""
        ElseIf Not IsDate(myDataInizio.Trim) Then
            myDataInizio = ""
        End If
        If myDataInizio = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Genera attività per periodo.", "Attenzione, non è stata definita la data di inizio.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        If _WucElement.SaveScadenze() = False Then
            Exit Function
        End If
        'ok sono pronto a generare
        Dim RowDett As DSDocumenti.ContrattiDRow
        Dim DsDocDettForInsert As New DSDocumenti
        Dim RowDettForIns As DSDocumenti.ContrattiDForInsertRow
        Dim dc As DataColumn
        Dim DataNext As String = ""
        Dim RowArtAnniSc As DSDocumenti.ArticoliAnniScRow
        Dim NextRiga As Integer = 0
        Dim SWOK As Boolean = True
        Dim SWDataScNOPeriodo As Boolean = False
        Dim SWAggiuntaAPP As Integer = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length
        Dim RowCDett() As DataRow
        Try
            If myDurataTipo.Trim = "A" Then
                For i = 0 To Val(myDurataNum) - 1
                    NextRiga = 0
                    For Each RowDett In DsContrattiDett.ContrattiD.Select("DurataNum=0", "Serie,DataSc") 'giu131223 solo le APP (CI SONO ANCHE I PERIODI ADESSO X L'AGGIUNTA)
                        If RowDett.RowState = DataRowState.Deleted Then
                            Continue For
                        End If
                        If RowDett.IsDataScNull Then
                            Continue For
                        End If
                        'giu151223
                        If RowDett.IsSerieNull Then
                            Continue For
                        End If
                        If SWAggiuntaAPP > 0 Then
                            If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                                Continue For
                            End If
                            RowCDett = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")
                            If RowCDett.Length > 0 Then
                                NextRiga = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            Else
                                NextRiga = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            End If
                        End If
                        '---------
                        RowDett.BeginEdit()
                        SWOK = True
                        NextRiga += 1
                        RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                        For Each dc In DsContrattiDett.ContrattiD.Columns
                            If UCase(dc.ColumnName) = "NEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "NEXTREFDATANC" Then 'giu170424
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "SERIELOTTO" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTREFDATANC" Then
                                Continue For
                            End If
                            If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                            ElseIf UCase(dc.ColumnName) = "DATASC" Then
                                If RowDett.IsCod_ArticoloNull Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        SWOK = False
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    End If
                                    'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                    DataNext = CDate(RowDett.NextDataRefDataNC)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        'SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        'SWOK = False
                                    Else
                                        RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataRefDataNC))
                                        RowDett.NextDataRefDataNC = CDate(DataNext)
                                    End If
                                ElseIf RowDett.Cod_Articolo.Trim = "" Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        SWOK = False
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    End If
                                    'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                    DataNext = CDate(RowDett.NextDataRefDataNC)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        'SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        'SWOK = False
                                    Else
                                        RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataRefDataNC))
                                        RowDett.NextDataRefDataNC = CDate(DataNext)
                                    End If
                                Else
                                    If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim) Is Nothing Then
                                        DataNext = CDate(RowDett.NextDataSc)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                            SWOK = False
                                        Else
                                            RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataSc))
                                            RowDett.NextDataSc = CDate(DataNext)
                                        End If
                                        'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                        DataNext = CDate(RowDett.NextDataRefDataNC)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            'SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                            'SWOK = False
                                        Else
                                            RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataRefDataNC))
                                            RowDett.NextDataRefDataNC = CDate(DataNext)
                                        End If
                                    Else
                                        RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim)
                                        If RowArtAnniSc.AnniSc_EL > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                            'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                            DataNext = CDate(RowDett.NextDataRefDataNC)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                'SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                'SWOK = False
                                            Else
                                                RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataRefDataNC))
                                                RowDett.NextDataRefDataNC = CDate(DataNext)
                                            End If
                                        ElseIf RowArtAnniSc.AnniSc_BA > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                            'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                            DataNext = CDate(RowDett.NextDataRefDataNC)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                'SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                'SWOK = False
                                            Else
                                                RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataRefDataNC))
                                                RowDett.NextDataRefDataNC = CDate(DataNext)
                                            End If
                                        Else
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                            'giu170424 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                            DataNext = CDate(RowDett.NextDataRefDataNC)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                'SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                'SWOK = False
                                            Else
                                                RowDettForIns.Item("NextDataRefDataNC") = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, 1, CDate(RowDett.NextDataRefDataNC))
                                                RowDett.NextDataRefDataNC = CDate(DataNext)
                                            End If
                                        End If
                                    End If
                                End If

                            ElseIf UCase(dc.ColumnName) = "DURATANUM" Then
                                RowDettForIns.Item(dc.ColumnName) = 1 'fisso 1
                            ElseIf UCase(dc.ColumnName) = "DURATANUMRIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = i
                            ElseIf UCase(dc.ColumnName) = "RIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = NextRiga
                            ElseIf TipoFatt <> "U" Then
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            ElseIf i <> 0 Then 'Primo periodo i prezzi altrimenti tutti a ZERO
                                If UCase(dc.ColumnName) = "PREZZO" Or UCase(dc.ColumnName) = "PREZZO_NETTO" Or _
                                    UCase(dc.ColumnName) = "IMPORTO" Or UCase(dc.ColumnName) = "PREZZO_NETTO_INPUTATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "SCONTOREALE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 100
                                ElseIf UCase(dc.ColumnName) = "SWPNETTOMODIFICATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = True
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            End If
                        Next
                        If SWOK = True Then
                            DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                        Else
                            NextRiga -= 1
                        End If
                        RowDett.EndEdit()
                    Next
                    myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "M" Then
                For i = 0 To Val(myDurataNum) - 1
                    NextRiga = 0
                    For Each RowDett In DsContrattiDett.ContrattiD.Select("", "DataSc")
                        If RowDett.RowState = DataRowState.Deleted Then
                            Continue For
                        End If
                        If RowDett.IsDataScNull Then
                            Continue For
                        End If
                        '-
                        'giu151223
                        If RowDett.IsSerieNull Then
                            Continue For
                        End If
                        If SWAggiuntaAPP > 0 Then
                            If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                                Continue For
                            End If
                            RowCDett = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")
                            If RowCDett.Length > 0 Then
                                NextRiga = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            Else
                                NextRiga = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            End If
                        End If
                        '---------
                        RowDett.BeginEdit()
                        SWOK = True
                        NextRiga += 1
                        RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                        For Each dc In DsContrattiDett.ContrattiD.Columns
                            If UCase(dc.ColumnName) = "NEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "SERIELOTTO" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTREFDATANC" Then
                                Continue For
                            End If
                            If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                            ElseIf UCase(dc.ColumnName) = "DATASC" Then
                                If RowDett.IsCod_ArticoloNull Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        SWOK = False
                                    ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                        SWOK = False
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 1, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    End If
                                ElseIf RowDett.Cod_Articolo.Trim = "" Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                        SWOK = False
                                    ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                        SWOK = False
                                    Else
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 1, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    End If
                                Else
                                    If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim) Is Nothing Then
                                        DataNext = CDate(RowDett.NextDataSc)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                            SWOK = False
                                        ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                            SWOK = False
                                        Else
                                            RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Month, 1, CDate(RowDett.NextDataSc))
                                            RowDett.NextDataSc = CDate(DataNext)
                                        End If
                                    Else
                                        RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim)
                                        If RowArtAnniSc.AnniSc_EL > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                        ElseIf RowArtAnniSc.AnniSc_BA > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                        Else
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext).Year <> CDate(myDataInizio).Year Then
                                                SWOK = False
                                            ElseIf CDate(DataNext).Month <> CDate(myDataInizio).Month Then
                                                SWOK = False
                                            Else
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Month, 1, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf UCase(dc.ColumnName) = "DURATANUM" Then
                                RowDettForIns.Item(dc.ColumnName) = 1 'fisso 1
                            ElseIf UCase(dc.ColumnName) = "DURATANUMRIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = i
                            ElseIf UCase(dc.ColumnName) = "RIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = NextRiga
                            ElseIf TipoFatt <> "U" Then
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            ElseIf i <> 0 Then 'Primo periodo i prezzi altrimenti tutti a ZERO
                                If UCase(dc.ColumnName) = "PREZZO" Or UCase(dc.ColumnName) = "PREZZO_NETTO" Or _
                                    UCase(dc.ColumnName) = "IMPORTO" Or UCase(dc.ColumnName) = "PREZZO_NETTO_INPUTATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "SCONTOREALE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 100
                                ElseIf UCase(dc.ColumnName) = "SWPNETTOMODIFICATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = True
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            End If
                        Next
                        If SWOK = True Then
                            DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                        Else
                            NextRiga -= 1
                        End If
                        RowDett.EndEdit()
                    Next
                    myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "T" Then
                For i = 0 To Val(myDurataNum) - 1
                    NextRiga = 0
                    For Each RowDett In DsContrattiDett.ContrattiD.Select("", "DataSc")
                        If RowDett.RowState = DataRowState.Deleted Then
                            Continue For
                        End If
                        If RowDett.IsDataScNull Then
                            Continue For
                        End If
                        '-
                        'giu151223
                        If RowDett.IsSerieNull Then
                            Continue For
                        End If
                        If SWAggiuntaAPP > 0 Then
                            If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                                Continue For
                            End If
                            RowCDett = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")
                            If RowCDett.Length > 0 Then
                                NextRiga = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            Else
                                NextRiga = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            End If
                        End If
                        '---------
                        RowDett.BeginEdit()
                        SWOK = True
                        NextRiga += 1
                        RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                        For Each dc In DsContrattiDett.ContrattiD.Columns
                            If UCase(dc.ColumnName) = "NEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "SERIELOTTO" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTREFDATANC" Then
                                Continue For
                            End If
                            If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                            ElseIf UCase(dc.ColumnName) = "DATASC" Then
                                If RowDett.IsCod_ArticoloNull Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 3, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                ElseIf RowDett.Cod_Articolo.Trim = "" Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 3, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                Else
                                    If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim) Is Nothing Then
                                        DataNext = CDate(RowDett.NextDataSc)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                            RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Month, 3, CDate(RowDett.NextDataSc))
                                            RowDett.NextDataSc = CDate(DataNext)
                                        Else
                                            SWOK = False
                                        End If
                                    Else
                                        RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim)
                                        If RowArtAnniSc.AnniSc_EL > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        ElseIf RowArtAnniSc.AnniSc_BA > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        Else
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 3, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Month, 3, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf UCase(dc.ColumnName) = "DURATANUM" Then
                                RowDettForIns.Item(dc.ColumnName) = 1 'fisso 1
                            ElseIf UCase(dc.ColumnName) = "DURATANUMRIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = i
                            ElseIf UCase(dc.ColumnName) = "RIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = NextRiga
                            ElseIf TipoFatt <> "U" Then
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            ElseIf i <> 0 Then 'Primo periodo i prezzi altrimenti tutti a ZERO
                                If UCase(dc.ColumnName) = "PREZZO" Or UCase(dc.ColumnName) = "PREZZO_NETTO" Or _
                                    UCase(dc.ColumnName) = "IMPORTO" Or UCase(dc.ColumnName) = "PREZZO_NETTO_INPUTATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "SCONTOREALE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 100
                                ElseIf UCase(dc.ColumnName) = "SWPNETTOMODIFICATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = True
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            End If
                        Next
                        If SWOK = True Then
                            DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                        Else
                            NextRiga -= 1
                        End If
                        RowDett.EndEdit()
                    Next
                    myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "Q" Then
                For i = 0 To Val(myDurataNum) - 1
                    NextRiga = 0
                    For Each RowDett In DsContrattiDett.ContrattiD.Select("", "DataSc")
                        If RowDett.RowState = DataRowState.Deleted Then
                            Continue For
                        End If
                        If RowDett.IsDataScNull Then
                            Continue For
                        End If
                        '-
                        'giu151223
                        If RowDett.IsSerieNull Then
                            Continue For
                        End If
                        If SWAggiuntaAPP > 0 Then
                            If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                                Continue For
                            End If
                            RowCDett = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")
                            If RowCDett.Length > 0 Then
                                NextRiga = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            Else
                                NextRiga = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            End If
                        End If
                        '---------
                        RowDett.BeginEdit()
                        SWOK = True
                        NextRiga += 1
                        RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                        For Each dc In DsContrattiDett.ContrattiD.Columns
                            If UCase(dc.ColumnName) = "NEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "SERIELOTTO" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTREFDATANC" Then
                                Continue For
                            End If
                            If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                            ElseIf UCase(dc.ColumnName) = "DATASC" Then
                                If RowDett.IsCod_ArticoloNull Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 4, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                ElseIf RowDett.Cod_Articolo.Trim = "" Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 4, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                Else
                                    If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim) Is Nothing Then
                                        DataNext = CDate(RowDett.NextDataSc)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                            RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Month, 4, CDate(RowDett.NextDataSc))
                                            RowDett.NextDataSc = CDate(DataNext)
                                        Else
                                            SWOK = False
                                        End If
                                    Else
                                        RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim)
                                        If RowArtAnniSc.AnniSc_EL > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        ElseIf RowArtAnniSc.AnniSc_BA > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        Else
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 4, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Month, 4, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf UCase(dc.ColumnName) = "DURATANUM" Then
                                RowDettForIns.Item(dc.ColumnName) = 1 'fisso 1
                            ElseIf UCase(dc.ColumnName) = "DURATANUMRIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = i
                            ElseIf UCase(dc.ColumnName) = "RIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = NextRiga
                            ElseIf TipoFatt <> "U" Then
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            ElseIf i <> 0 Then 'Primo periodo i prezzi altrimenti tutti a ZERO
                                If UCase(dc.ColumnName) = "PREZZO" Or UCase(dc.ColumnName) = "PREZZO_NETTO" Or _
                                    UCase(dc.ColumnName) = "IMPORTO" Or UCase(dc.ColumnName) = "PREZZO_NETTO_INPUTATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "SCONTOREALE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 100
                                ElseIf UCase(dc.ColumnName) = "SWPNETTOMODIFICATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = True
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            End If
                        Next
                        If SWOK = True Then
                            DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                        Else
                            NextRiga -= 1
                        End If
                        RowDett.EndEdit()
                    Next
                    myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
                Next i
            ElseIf myDurataTipo.Trim = "S" Then
                For i = 0 To Val(myDurataNum) - 1
                    NextRiga = 0
                    For Each RowDett In DsContrattiDett.ContrattiD.Select("", "DataSc")
                        If RowDett.RowState = DataRowState.Deleted Then
                            Continue For
                        End If
                        If RowDett.IsDataScNull Then
                            Continue For
                        End If
                        'giu151223
                        If RowDett.IsSerieNull Then
                            Continue For
                        End If
                        If SWAggiuntaAPP > 0 Then
                            If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                                Continue For
                            End If
                            RowCDett = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")
                            If RowCDett.Length > 0 Then
                                NextRiga = DsDocDettForInsert.ContrattiDForInsert.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            Else
                                NextRiga = DsContrattiDett.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" + i.ToString.Trim + "", "Riga DESC")(0)("Riga")
                            End If
                        End If
                        '---------
                        RowDett.BeginEdit()
                        SWOK = True
                        NextRiga += 1
                        RowDettForIns = DsDocDettForInsert.ContrattiDForInsert.NewRow
                        For Each dc In DsContrattiDett.ContrattiD.Columns
                            If UCase(dc.ColumnName) = "NEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATASC" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTDATAEV" Then 'GIU040820
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "SERIELOTTO" Then
                                Continue For
                            End If
                            If UCase(dc.ColumnName) = "TEXTREFDATANC" Then
                                Continue For
                            End If
                            If IsDBNull(RowDett.Item(dc.ColumnName)) Then
                                RowDettForIns.Item(dc.ColumnName) = DBNull.Value
                            ElseIf UCase(dc.ColumnName) = "DATASC" Then
                                If RowDett.IsCod_ArticoloNull Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 6, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                ElseIf RowDett.Cod_Articolo.Trim = "" Then
                                    DataNext = CDate(RowDett.NextDataSc)
                                    If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                        SWDataScNOPeriodo = True
                                    End If
                                    If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                        RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                        DataNext = DateAdd(DateInterval.Month, 6, CDate(RowDett.NextDataSc))
                                        RowDett.NextDataSc = CDate(DataNext)
                                    Else
                                        SWOK = False
                                    End If
                                Else
                                    If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim) Is Nothing Then
                                        DataNext = CDate(RowDett.NextDataSc)
                                        If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                            SWDataScNOPeriodo = True
                                        End If
                                        If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                            RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                            DataNext = DateAdd(DateInterval.Month, 6, CDate(RowDett.NextDataSc))
                                            RowDett.NextDataSc = CDate(DataNext)
                                        Else
                                            SWOK = False
                                        End If
                                    Else
                                        RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowDett.Cod_Articolo.Trim)
                                        If RowArtAnniSc.AnniSc_EL > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_EL, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        ElseIf RowArtAnniSc.AnniSc_BA > 0 Then
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Year, RowArtAnniSc.AnniSc_BA, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        Else
                                            DataNext = CDate(RowDett.NextDataSc)
                                            If CDate(DataNext).Date < CDate(myDataInizio).Date Then
                                                SWDataScNOPeriodo = True
                                            End If
                                            If CDate(DataNext) >= CDate(myDataInizio) And CDate(DataNext) <= DateAdd(DateInterval.Month, 6, CDate(myDataInizio)) Then
                                                RowDettForIns.Item(dc.ColumnName) = CDate(DataNext)
                                                DataNext = DateAdd(DateInterval.Month, 6, CDate(RowDett.NextDataSc))
                                                RowDett.NextDataSc = CDate(DataNext)
                                            Else
                                                SWOK = False
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf UCase(dc.ColumnName) = "DURATANUM" Then
                                RowDettForIns.Item(dc.ColumnName) = 1 'fisso 1
                            ElseIf UCase(dc.ColumnName) = "DURATANUMRIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = i
                            ElseIf UCase(dc.ColumnName) = "RIGA" Then
                                RowDettForIns.Item(dc.ColumnName) = NextRiga
                            ElseIf TipoFatt <> "U" Then
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            ElseIf i <> 0 Then 'Primo periodo i prezzi altrimenti tutti a ZERO
                                If UCase(dc.ColumnName) = "PREZZO" Or UCase(dc.ColumnName) = "PREZZO_NETTO" Or _
                                    UCase(dc.ColumnName) = "IMPORTO" Or UCase(dc.ColumnName) = "PREZZO_NETTO_INPUTATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = 0
                                ElseIf UCase(dc.ColumnName) = "SCONTOREALE" Then
                                    RowDettForIns.Item(dc.ColumnName) = 100
                                ElseIf UCase(dc.ColumnName) = "SWPNETTOMODIFICATO" Then
                                    RowDettForIns.Item(dc.ColumnName) = True
                                Else
                                    RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                                End If
                            Else
                                RowDettForIns.Item(dc.ColumnName) = RowDett.Item(dc.ColumnName)
                            End If
                        Next
                        If SWOK = True Then
                            DsDocDettForInsert.ContrattiDForInsert.AddContrattiDForInsertRow(RowDettForIns)
                        Else
                            NextRiga -= 1
                        End If
                        RowDett.EndEdit()
                    Next
                    myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
                Next i
            Else 'non capitera' mai
                Call SetStatoDoc5()
                Session(CSTNONCOMPLETO) = SWSI
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Genera attività per periodo.", "Attenzione, non è stata definita il tipo di durata.", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch ex As Exception
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.GeneraAttPeriodo", "Si è verificato un errore durante la preparazione Attività Periodo:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End Try
        '------
        Dim TransTmp As SqlClient.SqlTransaction
        Dim SqlConTmp As New SqlConnection
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Try
            SqlConTmp.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
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
            'esempio.CommandTimeout = myTimeOUT
            '---------------------------
            'GIU131223
            Dim sqlCmdDel As SqlCommand
            If SqlConTmp.State <> ConnectionState.Open Then
                SqlConTmp.Open()
            End If
            TransTmp = SqlConTmp.BeginTransaction(IsolationLevel.ReadCommitted)
            'giu190124 non cancella i periodi nel caso venisse modificata un N° serie perche non trova cosi cancello tutto il periodo
            If SWAggiuntaAPP = 0 Then
                sqlCmdDel = New SqlCommand
                sqlCmdDel.CommandText = "Delete From ContrattiD Where IDDocumenti=" & myID.Trim & " AND DurataNum=1"
                sqlCmdDel.Connection = SqlConTmp
                sqlCmdDel.CommandTimeout = myTimeOUT
                sqlCmdDel.CommandType = CommandType.Text
                '-
                sqlCmdDel.Transaction = TransTmp
                sqlCmdDel.ExecuteNonQuery()
                sqlCmdDel = Nothing
            End If
            '---------
            For Each RowDett In DsContrattiDett.ContrattiD.Select("DurataNum=0", "Serie,DataSc") 'giu131223 solo le APP (CI SONO ANCHE I PERIODI ADESSO X L'AGGIUNTA)
                If RowDett.RowState = DataRowState.Deleted Then
                    Continue For
                End If
                If RowDett.IsDataScNull Then
                    Continue For
                End If
                If RowDett.IsSerieNull Then
                    Continue For
                End If
                If DsContrattiDett.ContrattiD.Select("DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "' AND (Qta_Evasa<>0 OR Qta_Fatturata<>0)").Length > 0 Then
                    Continue For
                End If
                '-
                sqlCmdDel = New SqlCommand
                sqlCmdDel.CommandText = "Delete From ContrattiD Where IDDocumenti=" & myID.Trim & " AND DurataNum=1 AND Serie='" + RowDett.Serie.Trim + "'"
                sqlCmdDel.Connection = SqlConTmp
                sqlCmdDel.CommandTimeout = myTimeOUT
                sqlCmdDel.CommandType = CommandType.Text
                '-
                sqlCmdDel.Transaction = TransTmp
                sqlCmdDel.ExecuteNonQuery()
                sqlCmdDel = Nothing
            Next
            'ok INSERT DETTAGLI
            SqlDbInserCmd.Connection = SqlConTmp
            SqlDbInserCmd.CommandTimeout = myTimeOUT
            SqlDbInserCmd.Transaction = TransTmp
            SqlAdapDocForInsertD.Update(DsDocDettForInsert.ContrattiDForInsert)
            TransTmp.Commit()
            If SqlConTmp.State <> ConnectionState.Closed Then
                SqlConTmp.Close()
            End If
            '---------
            GeneraAttPeriodo = True
        Catch ExSQL As SqlException
            TransTmp.Rollback()
            Call SetStatoDoc5()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.GeneraAttPeriodo", "Si è verificato un errore durante l'aggiornamento del database:" & ExSQL.Message, WUC_ModalPopup.TYPE_ALERT)
            GeneraAttPeriodo = False
            Exit Function
        Catch ex As Exception
            TransTmp.Rollback()
            Call SetStatoDoc5()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GeneraAttPeriodo", "Si è verificato un errore durante l'aggiornamento del database:" & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            GeneraAttPeriodo = False
            Exit Function
        End Try
        '============================================================================
        'giu050520 DATA SCADENZA ATTIVITA' NON COMPRESO NEL PERIODO
        If SWDataScNOPeriodo = True Then
            Call SetStatoDoc5()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GeneraAttPeriodo", " Data Scadenza attività inferiore alla data di inizio periodo", WUC_ModalPopup.TYPE_ALERT)
            GeneraAttPeriodo = False
            Exit Function
        End If
        '------------------------------------------------------------------------------------------------------------
        GeneraAttPeriodo = True
        EnableTOTxtInsArticoli(False)
        myErrore = ""
        If AggImportiTot(myErrore) = False Then
            'messaggio segnalato da WUC_Contratti _WucElement.Chiudi("Errore: Aggiornamento importi e scadenze")
            'proseguo Exit Function
        End If
        If AggScadVisToAtt(myErrore) = False Then 'giu180420
            'proseguo
        End If
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        Session("GeneraAttPeriodi") = SWSI
    End Function
    Private Function AggScadVisToAtt(ByRef CKMess As String) As Boolean
        AggScadVisToAtt = False
        CKMess = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", "Attenzione, ID Documento sconosciuto.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim myDurataNum As String = Session(CSTDURATANUM)
        If IsNothing(myDurataNum) Then
            myDurataNum = ""
        ElseIf String.IsNullOrEmpty(myDurataNum) Then
            myDurataNum = ""
        ElseIf Not IsNumeric(myDurataNum) Then
            myDurataNum = ""
        ElseIf Val(myDurataNum) = 0 Then
            myDurataNum = ""
        End If
        If myDurataNum = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", "Attenzione, non è stata definita la durata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim myDurataTipo As String = Session(CSTDURATATIPO)
        If IsNothing(myDurataTipo) Then
            myDurataTipo = ""
        ElseIf String.IsNullOrEmpty(myDurataTipo) Then
            myDurataTipo = ""
        ElseIf myDurataTipo.Trim = "" Then
            myDurataTipo = ""
        End If
        If myDurataTipo = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", "Attenzione, non è stata definita il tipo di durata.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim myDataInizio As String = Session(CSTDATAINIZIO)
        If IsNothing(myDataInizio) Then
            myDataInizio = ""
        ElseIf String.IsNullOrEmpty(myDataInizio) Then
            myDataInizio = ""
        ElseIf Not IsDate(myDataInizio.Trim) Then
            myDataInizio = ""
        End If
        If myDataInizio = "" Then
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", "Attenzione, non è stata definita la data di inizio.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-
        Dim myCKMess As String = ""
        Try
            SetCdmDAdp()

            Dim DsDocDettTmp As New DSDocumenti
            DsDocDettTmp.ContrattiD.Clear()
            SqlDbSelectCmdALLAtt.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNum").Value = DBNull.Value 'GIU130320 PRENDO ANCHE LE APP. POI FILTRO - 1 'fisso per le attività per periodo
            SqlDbSelectCmdALLAtt.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            SqlAdapDocALLAtt.Fill(DsDocDettTmp.ContrattiD)
            '--------------------------------------------------------
            'giu120320 controllo ATTIVITA'/CHECK PER PERIODO
            Dim SaveSerie As String = ""
            Dim myNApp As Integer = 0 '210420 ERR. MANCAVA RIGA 1 DsDocDettTmp.ContrattiD.Select("DurataNum=0 AND Riga=1").Length
            For Each RowDettS In DsDocDettTmp.ContrattiD.Select("DurataNum=0", "Serie")
                If IsDBNull(RowDettS.Item("Serie")) Then
                    SaveSerie = ""
                    myNApp += 1
                ElseIf SaveSerie = RowDettS.Item("Serie").ToString.Trim Then
                    Continue For
                Else
                    myNApp += 1
                    SaveSerie = RowDettS.Item("Serie").ToString.Trim
                End If
            Next

            Dim SWChek As Boolean = True : Dim pCodArt As String = "" : Dim pNVisite As Integer = 0
            SWChek = True
            If myNApp = 0 Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Nessuna apparecchiatura inserita."
            End If
            If IsNothing(myDurataNum) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Durata periodo non definita."
            End If
            If String.IsNullOrEmpty(myDurataNum) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Durata periodo non definita."
            ElseIf Not IsNumeric(myDurataNum) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Durata periodo non definita."
            ElseIf Val(myDurataNum) = 0 Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Durata periodo non definita."
            End If
            '-
            If IsNothing(myDurataTipo) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Tipo Durata periodo non definita."
            End If
            If String.IsNullOrEmpty(myDurataTipo) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Tipo Durata periodo non definita."
            ElseIf myDurataTipo.Trim = "" Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Tipo Durata periodo non definita."
            End If
            '-
            If IsNothing(myDataInizio) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Data Inizio contratto non definita."
            End If
            If String.IsNullOrEmpty(myDataInizio) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Data Inizio contratto non definita."
            ElseIf Not IsDate(myDataInizio.Trim) Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Data Inizio contratto non definita."
            End If
            '----------
            Dim arrPeriodo() As String = Nothing
            If _WucElement.SetArrPeriodo(arrPeriodo) = False Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Definizione Elenco periodo errato."
            End If
            If GetCodVisitaDATipoCAIdPag(pCodArt, pNVisite) = False Then
                SWChek = False
                If CKMess.Trim <> "" Then CKMess += "<br>"
                CKMess += "Definizione Tipo Contratto errato."
            End If
            If SWChek = False Then
                Call SetStatoDoc5()
                Session(CSTNONCOMPLETO) = SWSI
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", CKMess.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
            'ok CONTROLLO E AGGIORNO
            Dim NVisitePeriodo As Integer = 0 : Dim SaveDataScVis As String = ""
            Dim NScadPeriodo As Integer = 0 : Dim SaveDataSc As String = ""
            Dim CheckPeriodo As String = ""
            Dim SaveCodDest As String = "" : Dim SaveCodDestV As String = ""
            '-------------------------------
            Dim strErrore As String = ""
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
            '--- Parametri
            SqlUpd_ConTScadPag.CommandText = "[update_ConDByIDDocRiga]"
            SqlUpd_ConTScadPag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ConTScadPag.Connection = SqlConn
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 0, "DurataNum"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "DurataNumRiga"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataSC", System.Data.SqlDbType.DateTime, 8, "DataSC"))
            SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV")) 'giu040820
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '---------------------------
            Dim SWOKAgg As Boolean = False
            Dim ISave As Integer = -1 'giu150122
            For i = 0 To Val(myDurataNum) - 1

                CheckPeriodo = arrPeriodo(i).Trim
                NScadPeriodo = 0 : SaveDataSc = "" : SaveCodDest = ""
                NVisitePeriodo = 0 : SaveDataScVis = ""
                myCKMess = "" : SWOKAgg = False
                For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim & " AND Cod_Articolo<>'" & pCodArt.Trim & "'", "DataSc")
                    If i = ISave Then
                        Continue For
                    End If
                    ISave = i
                    If RowDettB.RowState <> DataRowState.Deleted Then
                        If IsDBNull(RowDettB.Item("DataSc")) Then
                            If myCKMess.Trim <> "" Then myCKMess += "<br>"
                            myCKMess += CheckPeriodo + " - Manca Data di scadenza"
                            Exit Function
                        Else
                            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                                SaveCodDest = ""
                            Else
                                SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                            End If
                            SWOKAgg = False
                            For Each RowDettBV In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim & " AND Cod_Articolo='" & pCodArt.Trim & "'", "DataSc")
                                If IsDBNull(RowDettBV.Item("DataSc")) Then
                                    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                                    myCKMess += CheckPeriodo + " - Manca Data di scadenza"
                                    Exit Function
                                Else
                                    SaveDataScVis = RowDettBV.Item("DataSc").ToString.Trim
                                    If IsDBNull(RowDettBV.Item("Cod_Filiale")) Then
                                        SaveCodDestV = ""
                                    Else
                                        SaveCodDestV = RowDettBV.Item("Cod_Filiale").ToString.Trim
                                    End If
                                End If
                                'ok
                                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                'giu150322 If SaveDataScVis <> SaveDataSc And SaveCodDestV = SaveCodDest Then
                                SWOKAgg = True
                                Try
                                    If Not IsNothing(SqlConn) Then
                                        If SqlConn.State <> ConnectionState.Open Then
                                            SqlConn.Open()
                                        End If
                                    End If
                                    SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = RowDettBV.Item("IDDocumenti")
                                    SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = RowDettBV.Item("DurataNum")
                                    SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = RowDettBV.Item("DurataNumRiga")
                                    SqlUpd_ConTScadPag.Parameters("@Riga").Value = RowDettBV.Item("Riga")
                                    SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = RowDettBV.Item("Qta_Evasa")
                                    SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = RowDettBV.Item("Qta_Residua")
                                    SqlUpd_ConTScadPag.Parameters("@DataSC").Value = IIf(CDate(SaveDataSc).Date < CDate(SaveDataScVis).Date, CDate(SaveDataSc), CDate(SaveDataScVis))
                                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = RowDettBV.Item("DataEv") 'giu040820
                                    '-
                                    SqlUpd_ConTScadPag.CommandTimeout = myTimeOUT '5000
                                    SqlUpd_ConTScadPag.ExecuteNonQuery()
                                Catch exSQL As SqlException
                                    If Not IsNothing(SqlConn) Then
                                        If SqlConn.State <> ConnectionState.Closed Then
                                            SqlConn.Close()
                                            SqlConn = Nothing
                                        End If
                                    End If
                                    strErrore = exSQL.Message
                                    Session(CSTNONCOMPLETO) = SWSI
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Function
                                Catch ex As Exception
                                    If Not IsNothing(SqlConn) Then
                                        If SqlConn.State <> ConnectionState.Closed Then
                                            SqlConn.Close()
                                            SqlConn = Nothing
                                        End If
                                    End If
                                    strErrore = ex.Message
                                    Session(CSTNONCOMPLETO) = SWSI
                                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                                    Exit Function
                                End Try
                                ''''End If
                                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                ' ''If SWOKAgg = True Then
                                ' ''    Exit For
                                ' ''End If
                            Next

                        End If
                        'old @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                        ' ''For Each RowDettB In DsDocDettTmp.ContrattiD.Select("DurataNum=1 AND DurataNumRiga=" & i.ToString.Trim, "DataSc")
                        ' ''If RowDettB.RowState <> DataRowState.Deleted Then
                        ' ''If IsDBNull(RowDettB.Item("DataSc")) Then
                        ' ''    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                        ' ''    myCKMess += CheckPeriodo + " - Manca Data di scadenza"
                        ' ''Else
                        ' ''    If IsDBNull(RowDettB.Item("Cod_Articolo")) Then
                        ' ''        If SaveDataSc = "" Then
                        ' ''            NScadPeriodo += 1
                        ' ''            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDest = ""
                        ' ''            Else
                        ' ''                SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                        ' ''            NScadPeriodo += 1
                        ' ''            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDest = ""
                        ' ''            Else
                        ' ''                SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        End If
                        ' ''    ElseIf RowDettB.Item("Cod_Articolo").ToString.Trim = pCodArt.Trim And pCodArt.Trim <> "" Then
                        ' ''        NVisitePeriodo += 1
                        ' ''        If SaveDataScVis = "" Then
                        ' ''            SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDestV = ""
                        ' ''            Else
                        ' ''                SaveCodDestV = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        ElseIf CDate(SaveDataScVis).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                        ' ''            SaveDataScVis = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDestV = ""
                        ' ''            Else
                        ' ''                SaveCodDestV = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        End If
                        ' ''    Else
                        ' ''        If SaveDataSc = "" Then
                        ' ''            NScadPeriodo += 1
                        ' ''            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDest = ""
                        ' ''            Else
                        ' ''                SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        ElseIf CDate(SaveDataSc).Date <> CDate(RowDettB.Item("DataSc")).Date Then
                        ' ''            NScadPeriodo += 1
                        ' ''            SaveDataSc = RowDettB.Item("DataSc").ToString.Trim
                        ' ''            If IsDBNull(RowDettB.Item("Cod_Filiale")) Then
                        ' ''                SaveCodDest = ""
                        ' ''            Else
                        ' ''                SaveCodDest = RowDettB.Item("Cod_Filiale").ToString.Trim
                        ' ''            End If
                        ' ''        End If
                        ' ''    End If

                        ' ''End If
                        ' ''-
                        ' ''If SaveDataScVis <> "" And SaveDataSc <> "" Then
                        ' ''    If CDate(SaveDataSc).Date < CDate(SaveDataScVis).Date And SaveCodDest = SaveCodDestV Then
                        ' ''        If myCKMess.Trim <> "" Then myCKMess += "<br>"
                        ' ''        myCKMess += CheckPeriodo + " - Scadenze precedenti alla visita prevista"
                        ' ''    End If
                        ' ''End If
                        ' ''End If
                        'Next

                    End If
                    Exit For
                Next
                ' ''If (NVisitePeriodo / myNApp) <> pNVisite And SaveCodDest = SaveCodDestV Then
                ' ''    If myCKMess.Trim <> "" Then myCKMess += "<br>"
                ' ''    myCKMess += CheckPeriodo + " - N° Visite diverso da quello indicato"
                ' ''End If
            Next
            '-----------------------------------------------
        Catch ex As Exception
            Call SetStatoDoc5()
            Session(CSTNONCOMPLETO) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("ERRORE Aggiona Data CKVisita alla prima data attività per periodo.", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        If myCKMess.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiona Data CKVisita alla prima data attività per periodo.", myCKMess.Trim, WUC_ModalPopup.TYPE_INFO)
        End If

        '-
    End Function
    Public Sub DeletePeriodiAttivita()
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.DeletePeriodiAttivita", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = ""
        Try
            strSQL = "UPDATE ContrattiT SET StatoDoc=5 WHERE IDDocumenti=" & myID.Trim
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.DeletePeriodiAttivita Cambio stato doc.=5", strSQL, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            '-
            strSQL = "Delete From ContrattiD Where IDDocumenti=" & myID.Trim & " AND DurataNum=1"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.DeletePeriodiAttivita", strSQL, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.DeletePeriodiAttivita", "Si è verificato un errore durante l'aggiornamento del database: " & ex.Message, strSQL, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        Session(SWOP) = SWOPNESSUNA
        _WucElement.Chiudi("")
        Exit Sub
        '============================================================================
    End Sub
    Public Function SetStatoDoc5() As Boolean
        'giu270220
        SetStatoDoc5 = False
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Then
            Exit Function
        End If
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = "UPDATE ContrattiT SET StatoDoc=5 WHERE IDDocumenti=" & myID.Trim
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in SetStatoDoc5 Cambio stato doc.=5", strSQL, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in SetStatoDoc5 Cambio stato doc.=5: " & ex.Message, strSQL, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        SetStatoDoc5 = True
    End Function
    Private Function GetAnniScAzzQtaNULL(ByRef myErrore As String) As Boolean
        myErrore = ""
        GetAnniScAzzQtaNULL = True
        Dim ObjDB As New DataBaseUtility
        Dim strSQL As String = ""
        Dim dsArt As New DataSet
        '-
        Me.DsContrattiDett.ArticoliAnniSc.Clear()
        Dim RowArtAnniSc As DSDocumenti.ArticoliAnniScRow
        Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("DurataNum=0") 'giu131223 solo le APP
        Dim RowD As DSDocumenti.ContrattiDRow
        Dim SeriePrec As String = ""
        Dim LottoPrec As String = ""
        Dim Modello As Integer = 0
        Dim DurNumRPrec As Integer = -1
        Try
            For Each RowD In RowsD
                If Not RowD.IsDataScNull And Not RowD.IsCod_ArticoloNull Then
                    If RowD.Cod_Articolo.Trim <> "" Then
                        If Me.DsContrattiDett.ArticoliAnniSc.FindByCod_Articolo(RowD.Cod_Articolo.Trim) Is Nothing Then
                            strSQL = "SELECT Cod_Articolo,ISNULL(NAnniGaranzia,0) AS Anni_GA, ISNULL(NAnniScadElettrodi,0) AS Anni_EL,ISNULL(NAnniScadBatterie,0) AS Anni_BA " & _
                                     "FROM AnaMag WHERE Cod_Articolo='" & RowD.Cod_Articolo.Trim & "'"
                            dsArt.Clear()
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
                            If (dsArt.Tables.Count > 0) Then
                                If (dsArt.Tables(0).Rows.Count > 0) Then
                                    RowArtAnniSc = Me.DsContrattiDett.ArticoliAnniSc.NewArticoliAnniScRow
                                    With RowArtAnniSc
                                        .Cod_Articolo = RowD.Cod_Articolo.Trim
                                        .AnniSc_GA = IIf(IsDBNull(dsArt.Tables(0).Rows(0).Item(1)), 0, dsArt.Tables(0).Rows(0).Item(1))
                                        .AnniSc_EL = IIf(IsDBNull(dsArt.Tables(0).Rows(0).Item(2)), 0, dsArt.Tables(0).Rows(0).Item(2))
                                        .AnniSc_BA = IIf(IsDBNull(dsArt.Tables(0).Rows(0).Item(3)), 0, dsArt.Tables(0).Rows(0).Item(3))
                                        .EndEdit()
                                    End With
                                    Me.DsContrattiDett.ArticoliAnniSc.AddArticoliAnniScRow(RowArtAnniSc)
                                Else
                                    GetAnniScAzzQtaNULL = False
                                    'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                    'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    'ModalPopup.Show("Genera attività per periodo: Scadenza Anni", "Articolo non trovato: " & RowD.Cod_Articolo.Trim, WUC_ModalPopup.TYPE_ERROR)
                                    myErrore = "Articolo non trovato: " & RowD.Cod_Articolo.Trim
                                    Exit Function
                                End If
                            Else
                                GetAnniScAzzQtaNULL = False
                                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                'ModalPopup.Show("Genera attività per periodo: Scadenza Anni", "Articolo non trovato: " & RowD.Cod_Articolo.Trim, WUC_ModalPopup.TYPE_ERROR)
                                myErrore = "Articolo non trovato: " & RowD.Cod_Articolo.Trim
                                Exit Function
                            End If

                        End If

                    End If
                End If
                RowD.BeginEdit()
                If RowD.IsQta_AllestitaNull Then RowD.Qta_Allestita = 0
                If RowD.IsQta_EvasaNull Then RowD.Qta_Evasa = 0
                If RowD.IsQta_ImpegnataNull Then RowD.Qta_Impegnata = 0
                If RowD.IsQta_OrdinataNull Then RowD.Qta_Ordinata = 0
                If RowD.IsQta_PrenotataNull Then RowD.Qta_Prenotata = 0
                If RowD.IsQta_ResiduaNull Then RowD.Qta_Residua = 0
                If RowD.IsPrezzoNull Then RowD.Prezzo = 0
                If RowD.IsPrezzo_NettoNull Then RowD.Prezzo_Netto = 0
                If RowD.IsImportoNull Then RowD.Importo = 0
                'giu170412
                If RowD.IsPrezzoAcquistoNull Then RowD.PrezzoAcquisto = 0
                If RowD.IsPrezzoListinoNull Then RowD.PrezzoListino = 0
                'giu190412
                If RowD.IsSWModAgentiNull Then RowD.SWModAgenti = False
                If RowD.IsPrezzoCostoNull Then RowD.PrezzoCosto = 0
                If RowD.IsQta_InviataNull Then RowD.Qta_Inviata = 0
                If RowD.IsQta_FatturataNull Then RowD.Qta_Fatturata = 0
                '---------
                If RowD.IsDataScNull Then
                    RowD.IsNextDataScNull()
                Else
                    RowD.NextDataSc = RowD.DataSc
                End If
                If RowD.IsLottoNull Then RowD.Lotto = ""
                If RowD.IsSerieNull Then RowD.Serie = ""
                If RowD.IsQtaDurataNumR0Null Then RowD.QtaDurataNumR0 = 0
                If DurNumRPrec <> RowD.DurataNumRiga Then
                    If DurNumRPrec <> -1 Then
                        If SeriePrec = "" And LottoPrec = "" Then
                            GetAnniScAzzQtaNULL = False
                            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            'ModalPopup.Show("Genera attività per periodo", "Attenzione manca il N°Serie/Lotto per l'apparecchiatura: " & (DurNumRPrec + 1).ToString.Trim & "<br> Impossibile procedere.", WUC_ModalPopup.TYPE_ERROR)
                            myErrore = "Attenzione manca il N°Serie/Lotto per l'apparecchiatura: " & (DurNumRPrec + 1).ToString.Trim & "<br> Impossibile procedere."
                            Exit Function
                        End If
                    End If
                    SeriePrec = RowD.Serie
                    LottoPrec = RowD.Lotto
                    DurNumRPrec = RowD.DurataNumRiga
                    Modello = RowD.QtaDurataNumR0
                Else
                    If SeriePrec = "" And RowD.Serie <> "" Then
                        SeriePrec = RowD.Serie
                    End If
                    If LottoPrec = "" And RowD.Lotto <> "" Then
                        LottoPrec = RowD.Lotto
                    End If
                    If Modello = 0 And RowD.QtaDurataNumR0 <> 0 Then
                        Modello = RowD.QtaDurataNumR0
                    End If
                End If
                If RowD.Lotto = "" Then
                    RowD.Lotto = LottoPrec
                End If
                If RowD.Serie = "" Then
                    RowD.Serie = SeriePrec
                End If
                If RowD.QtaDurataNumR0 = 0 Then
                    RowD.QtaDurataNumR0 = Modello
                End If
                RowD.EndEdit()
            Next
            If SeriePrec = "" And LottoPrec = "" Then
                GetAnniScAzzQtaNULL = False
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Genera attività per periodo", "Attenzione manca il N°Serie/Lotto per l'apparecchiatura: " & (DurNumRPrec + 1).ToString.Trim & "<br> Impossibile procedere.", WUC_ModalPopup.TYPE_ERROR)
                myErrore = "Attenzione manca il N°Serie/Lotto per l'apparecchiatura: " & (DurNumRPrec + 1).ToString.Trim & "<br> Impossibile procedere."
                Exit Function
            End If
        Catch ex As Exception
            GetAnniScAzzQtaNULL = False
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Genera attività per periodo", "Errore in ricerca Articolo: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            myErrore = "Errore in ricerca Articolo: " & ex.Message
            Exit Function
        End Try
    End Function
    'giu180220 @@@@@@@@@@@@@ RICALCOLO TUTTE LE ATTIVITA' PER PERIODO
    Public Function AggiornaImportoALLAtt(ByVal dsDocDett As DataSet, ByRef strErrore As String) As Boolean
        AggiornaImportoALLAtt = True
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            strErrore = "Errore: TIPO DOCUMENTO SCONOSCIUTO (ContrattiDett.AggiornaImportoALLAtt)"
            Return False
        End If
        SetCdmDAdp()
        If (dsDocDett Is Nothing) Then
            dsDocDett = Session("aDsDett")
        End If
        '-----------------------------------------------------------------------
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        'giu050220
        Dim myIDDurataNum As String = Session(IDDURATANUM)
        If IsNothing(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNum) Then
            myIDDurataNum = "0"
        End If
        '-
        Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
        If IsNothing(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        If String.IsNullOrEmpty(myIDDurataNumR) Then
            myIDDurataNumR = "0"
        End If
        '----------
        If myID = "" Or Not IsNumeric(myID) Then
            If (dsDocDett.Tables("ContrattiD").Rows.Count > 0) Then
                If Not IsDBNull(dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti")) Then
                    myID = dsDocDett.Tables("ContrattiD").Rows(0).Item("IDDocumenti").ToString.Trim
                    myIDDurataNum = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNum").ToString.Trim
                    myIDDurataNumR = dsDocDett.Tables("ContrattiD").Rows(0).Item("DurataNumRiga").ToString.Trim
                End If
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                strErrore = "Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO"
                Return False
            End If
            Session(IDDOCUMENTI) = myID 'GIU060220
            Session(IDDURATANUM) = myIDDurataNum
            Session(IDDURATANUMRIGA) = myIDDurataNumR
        End If
        'Valuta per i decimali per il calcolo OBBLIGATORIO E NON IN SESSIONE SCADUTA
        Dim IDLis As String = Session(IDLISTINO)
        If IsNothing(IDLis) Then IDLis = "1"
        If String.IsNullOrEmpty(IDLis) Then IDLis = "1"
        Session(IDLISTINO) = IDLis
        ' ''Dim CodValuta As String = GetCodValuta(IDLis)
        ' ''Dim DecimaliValutaFinito As Integer = GetDecimali_Valuta(CodValuta)
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        Dim FormatValuta As String = "###,###,##0"
        Select Case CInt(DecimaliVal)
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select
        '--------------------------------------------------
        'Blocco IVA
        Dim Iva(4) As Integer
        Dim Imponibile(4) As Decimal
        Dim Imposta(4) As Decimal
        'Spese Global
        Dim IvaTrasporto As Integer
        Dim IvaIncasso As Integer
        Dim IvaImballo As Integer

        Dim Totale As Decimal = 0 : Dim TotaleLordoMerce As Decimal = 0
        Dim MoltiplicatoreValuta As Integer 'Dichiarato ma mi sa che non server :)
        Dim TmpImp As Decimal
        Dim Cont As Integer

        WucElement.GetDatiTB3()

        Dim ScCassa As String = Session(CSTSCCASSA)
        If IsNothing(ScCassa) Then ScCassa = "0"
        If String.IsNullOrEmpty(ScCassa) Then ScCassa = "0"
        If Not IsNumeric(ScCassa) Then ScCassa = "0"
        '-
        Dim Abbuono As String = Session(CSTABBUONO)
        If IsNothing(Abbuono) Then Abbuono = "0"
        If String.IsNullOrEmpty(Abbuono) Then Abbuono = "0"
        If Not IsNumeric(Abbuono) Then Abbuono = "0"
        '-GIU020119
        '---- Calcolo sconto su 
        Dim ScontoSuImporto As Boolean = True 'OK ANCHE SE NON SERVE QUI LA ScCassaDett SI SERVER
        Dim ScCassaDett As Boolean = False 'giu010119
        Try
            ScontoSuImporto = App.GetParamGestAzi(ESERCIZIO).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(ESERCIZIO).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        'giu010119
        strErrore = ""
        Dim DsContrattiDettALLAtt As New DSDocumenti
        DsContrattiDettALLAtt.ContrattiD.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
        SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
        'giu030512
        Dim TotaleLordoMercePL As Decimal = 0
        Dim Deduzioni As Decimal = 0
        If CalcolaTotaleDocCA(DsContrattiDettALLAtt, Iva, Imponibile, Imposta, _
                              CInt(DecimaliVal), MoltiplicatoreValuta, _
                              Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
                              myTipoDoc, CDec(Abbuono), CLng(myID), strErrore, ScCassaDett, TotaleLordoMercePL, Deduzioni) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Return False
        End If

        LblTotale.Text = FormattaNumero((Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4)) + (Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4)), CInt(DecimaliVal))
        Cont = 0

        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        If Val(RegimeIVA) > 49 Then
            IvaTrasporto = Val(RegimeIVA)
            IvaImballo = Val(RegimeIVA)
            IvaIncasso = Val(RegimeIVA)
        Else
            IvaTrasporto = GetParamGestAzi(Session(ESERCIZIO)).IVATrasporto
            IvaIncasso = GetParamGestAzi(Session(ESERCIZIO)).IvaSpese
            IvaImballo = GetParamGestAzi(Session(ESERCIZIO)).Iva_Imballo
        End If

        'ASSEGNAZIONE TOTALE SPESE TRASPORTO
        Dim SpeseTrasporto As String = Session(CSTSPTRASP)
        If IsNothing(SpeseTrasporto) Then SpeseTrasporto = "0"
        If String.IsNullOrEmpty(SpeseTrasporto) Then SpeseTrasporto = "0"
        If Not IsNumeric(SpeseTrasporto) Then SpeseTrasporto = "0"
        '-
        If CDec(SpeseTrasporto) Then
            While ((IvaTrasporto <> Iva(Cont)) And _
                    Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
            End While
            Iva(Cont) = IvaTrasporto
            TmpImp = CDec(SpeseTrasporto)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaTrasporto < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        'ASSEGNAZIONE AL TOTALE DELLE SPESE DI IMBALLO E ATTRIBUZIONE NEL COMPUTO DELL'IVA
        Dim SpeseImballo As String = Session(CSTSPIMBALLO)
        If IsNothing(SpeseImballo) Then SpeseImballo = "0"
        If String.IsNullOrEmpty(SpeseImballo) Then SpeseImballo = "0"
        If Not IsNumeric(SpeseImballo) Then SpeseImballo = "0"
        '-
        If (CDec(SpeseImballo)) > 0 Then
            While ((IvaImballo <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
            End While
            Iva(Cont) = IvaImballo
            TmpImp = CDec(SpeseImballo)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaImballo < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        Dim SpeseIncasso As String = Session(CSTSPINCASSO)
        If IsNothing(SpeseIncasso) Then SpeseIncasso = "0"
        If String.IsNullOrEmpty(SpeseIncasso) Then SpeseIncasso = "0"
        If Not IsNumeric(SpeseIncasso) Then SpeseIncasso = "0"
        '-
        If (CDec(SpeseIncasso)) > 0 Then
            While ((IvaIncasso <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", MSGEccedIva, WUC_ModalPopup.TYPE_ERROR)
                    Return False
                End If
            End While
            Iva(Cont) = IvaIncasso
            TmpImp = CDec(SpeseIncasso)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If (Iva(Cont) > 0) And (Iva(Cont) < 50) Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        'Chiamo TD0 per richiamare TD3 SPESE,TRASPORTO,TOTALE,SCADENZE AGGIORNARE OK FATTO
        strErrore = ""
        Try
            If _WucElement.CalcolaTotSpeseScad(DsContrattiDettALLAtt, Iva, Imponibile, Imposta, _
                             CInt(DecimaliVal), MoltiplicatoreValuta, _
                             Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
                             myTipoDoc, CDec(Abbuono), strErrore, TotaleLordoMercePL, Deduzioni) = False Then
                If strErrore <> "" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                    strErrore = ""
                End If
                Return False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett  _WucElement.CalcolaTotSpeseScad", "Verificare il totale documento: " & ex.Message & "<br>Cancella e rigenerare i Periodi Attività.<br>Nel caso l'errore persiste contattare l'amministratore di sistema.", WUC_ModalPopup.TYPE_ERROR)
            btnDelPeriodiAtt.Visible = True
            _WucElement.btnbtnGeneraAttDNumColorRED(True)
        End Try

    End Function
    Public Sub btnDelPeriodiAttVisibile(ByVal SW As Boolean)
        btnDelPeriodiAtt.Visible = SW
    End Sub
    Public Function NuovaApp() As Boolean
        NuovaApp = False
        DDLTipoDettagli.SelectedIndex = 0
        Try
            DDLDurNumRIga.SelectedIndex = DDLDurNumRIga.Items.Count - 1
            Call DDLDurNumRIga_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
        End Try
        NuovaApp = True
    End Function

    Private Sub chkNoPrezzo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkNoPrezzo.CheckedChanged
        If chkNoPrezzo.Checked = True Then
            txtPrezzoIns.Text = "0" : LblPrezzoNetto.Text = "0" : LblImportoRiga.Text = "0"
        ElseIf txtCodArtIns.Text.Trim <> "" Then 'GIU110222
            Dim strErrore As String = ""
            Dim myPrezzoAcquisto As Decimal = 0
            Dim myPrezzoListino As Decimal = 0
            Dim MyPrezzoAL As Decimal = 0
            Dim mySconto1 As Decimal = 0 : Dim mySconto2 As Decimal = 0
            Dim IDLT As String = Session(IDLISTINO)
            If IsNothing(IDLT) Then IDLT = "1"
            If String.IsNullOrEmpty(IDLT) Then IDLT = "1"
            '-
            Dim myAnaMag As AnaMagEntity = Nothing
            If LeggiArticolo(txtCodArtIns.Text.Trim, myAnaMag) = False Then
                txtCodArtIns.BackColor = SEGNALA_KO
                txtCodArtIns.Focus()
                txtPrezzoIns.Text = "0" : LblPrezzoNetto.Text = "0" : LblImportoRiga.Text = "0"
                Exit Sub
            Else
                txtCodArtIns.BackColor = SEGNALA_OK
                If Not (myAnaMag Is Nothing) Then
                    myPrezzoAcquisto = myAnaMag.PrezzoAcquisto
                End If
            End If
            '----
            If GetPrezziListVenD(myTipoDoc, IDLT, txtCodArtIns.Text.Trim, _
                                 myPrezzoListino, mySconto1, mySconto2, strErrore) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in GetArticolo.GetPrezziListVenD", strErrore, WUC_ModalPopup.TYPE_ALERT)
                ' ''AzzeraTxtInsArticoli()
                ' ''Exit Sub
            ElseIf strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in GetArticolo.GetPrezziListVenD", strErrore, WUC_ModalPopup.TYPE_ALERT)
                ' ''AzzeraTxtInsArticoli()
                ' ''Exit Sub
            End If
            '--
            If SWPrezzoAL = "A" Then 'GIU170412
                MyPrezzoAL = myPrezzoAcquisto
            Else
                MyPrezzoAL = myPrezzoListino
            End If
            txtPrezzoIns.Text = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            LblPrezzoNetto.Text = "0" : LblImportoRiga.Text = "0"
        End If
    End Sub
    'giu250220
    Public Function GetCodVisitaDATipoCAIdPag(ByRef pCodVisita As String, ByRef pNVisite As Integer) As Boolean
        GetCodVisitaDATipoCAIdPag = False
        pCodVisita = "" : pNVisite = 0
        Dim CodPag As String = Session(CSTIDPAG)
        If IsNothing(CodPag) Then CodPag = ""
        If String.IsNullOrEmpty(CodPag) Then CodPag = ""
        If Not IsNumeric(CodPag) Then CodPag = ""
        If CodPag = "" Or CodPag = "0" Then Exit Function
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From TipoContratto WHERE Codice = " & CodPag.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                    pCodVisita = IIf(IsDBNull(rowPag(0).Item("CodVisita")), "", rowPag(0).Item("CodVisita"))
                    pNVisite = IIf(IsDBNull(rowPag(0).Item("NVisite")), 0, rowPag(0).Item("NVisite"))
                    GetCodVisitaDATipoCAIdPag = True
                End If
            End If
        Catch Ex As Exception
            GetCodVisitaDATipoCAIdPag = False
            Exit Function
        End Try
    End Function

    Private Sub btnCercaDestD_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaDestD.Click
        Session(F_ELENCO_DESTCFD_APERTA) = True
        WFPElencoDestCF.Show(True)
    End Sub
    Public Sub CallBackWFPElencoDestCF(ByVal codice As String, ByVal descrizione As String)
        Dim myCodFil As String = codice.Trim
        Dim myProvApp As String = ""
        PopolaDestCliForD(myCodFil, myProvApp)
        'azzero/aggiorno tutto
        SqlAdapDocDett = Session("aSqlAdap")
        DsContrattiDett = Session("aDsDett")
        If DDLTipoDettagli.SelectedIndex = 0 Then
            Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("")
            Dim RowD As DSDocumenti.ContrattiDRow
            For Each RowD In RowsD
                RowD.BeginEdit()
                If Not IsNumeric(myCodFil) Then
                    RowD.SetCod_FilialeNull()
                ElseIf Val(myCodFil) > 0 Then
                    RowD.Cod_Filiale = Val(myCodFil)
                Else
                    RowD.SetCod_FilialeNull()
                End If
                RowD.EndEdit()
            Next

        End If

        If (aDataView1 Is Nothing) Then
            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        End If
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
        Session("aDataView1") = aDataView1
        Session("aDsDett") = DsContrattiDett
        GridViewDett.DataSource = aDataView1
        GridViewDett.EditIndex = -1
        GridViewDett.DataBind()
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
        'GIU100222
        ' ''If DDLTipoDettagli.SelectedIndex = 0 Then
        ' ''    _WucElement.btnbtnGeneraAttDNumColorRED(True)
        ' ''End If
        If DDLTipoDettagli.SelectedIndex = 0 Then
            lblMessRespAV.Text = CKRespAreaVisiteByProv(myProvApp, True)
        Else
            lblMessRespAV.Text = CKRespAreaVisiteByProv(myProvApp, False)
        End If

    End Sub

    Private Sub btnDelDestD_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelDestD.Click
        PopolaDestCliForD("", "")
        lblMessRespAV.Text = ""
        'azzero/aggiorno tutto
        SqlAdapDocDett = Session("aSqlAdap")
        DsContrattiDett = Session("aDsDett")
        Dim RowsD() As DataRow = Me.DsContrattiDett.ContrattiD.Select("")
        Dim RowD As DSDocumenti.ContrattiDRow
        If DDLTipoDettagli.SelectedIndex = 0 Then
            For Each RowD In RowsD
                RowD.BeginEdit()
                RowD.SetCod_FilialeNull()
                RowD.EndEdit()
            Next
        Else
            Dim i As Integer = -1 : Dim myRowIndex As Integer = -1
            Try
                i = GridViewDett.SelectedIndex
                If i < 0 Then
                    '-
                Else
                    myRowIndex = i + (GridViewDett.PageSize * GridViewDett.PageIndex)
                End If
                If i >= 0 Then
                    RowsD(i).BeginEdit()
                    RowsD(i).Item("Cod_Filiale") = DBNull.Value
                    RowsD(i).EndEdit()
                End If
            Catch ex As Exception
                i = -1
            End Try
        End If

        If (aDataView1 Is Nothing) Then
            aDataView1 = New DataView(DsContrattiDett.ContrattiD)
        End If
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
        Session("aDataView1") = aDataView1
        Session("aDsDett") = DsContrattiDett
        GridViewDett.DataSource = aDataView1
        GridViewDett.EditIndex = -1
        GridViewDett.DataBind()
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub btnDelPeriodiAtt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelPeriodiAtt.Click
        ' ''If Session(SWOP) <> SWOPNESSUNA Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Completare prima l'operazione <br>" & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        '' ''GIU301111
        ' ''If Session(SWOPDETTDOC) <> SWOPNESSUNA Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica dettaglio documento.", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        '' ''LOTTI
        ' ''If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        ' ''If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
        ' ''    Exit Sub
        ' ''End If
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "DeletePeriodiAttivita"
        ModalPopup.Show("Cancella Periodi Attività", "Confermi la cancellazione di tutti i Periodi Attività ?<br>AL TERMINE RITORNO ALLA SCELTA DEL DOCUMENTO<br>Riselezionarlo per la generazione Periodi.", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub

    'GIU080122
    ' ''Private Sub DDLModello_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLModello.SelectedIndexChanged
    ' ''    _WucElement.btnbtnGeneraAttDNumColorRED(True)
    ' ''    Call _WucElement.SetLblMessDoc("Si prega di rigenerare i Periodo perchè  la modifica abbia effetto")
    ' ''    SetStatoDoc5()
    ' ''End Sub

    Private Sub txtSerie_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSerie.TextChanged
        'GIU11022 PRENDO LOC.APP DALL'APPARECCHIATURA
        'giu070523
        txtSerie.AutoPostBack = False
        txtSerie.Text = Formatta.FormattaNomeFile(txtSerie.Text.Trim)
        If txtSerie.Text.Trim <> "" Then txtSerie.BackColor = SEGNALA_OK
        txtSerie.AutoPostBack = True
        '---------
        If DDLTipoDettagli.SelectedIndex <> 0 And txtSerie.Text.Trim <> "" Then
            '--
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Sub
            End If
            'giu050220
            Dim myIDDurataNum As String = Session(IDDURATANUM)
            If IsNothing(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNum) Then
                myIDDurataNum = "0"
            End If
            '-
            Dim myIDDurataNumR As String = Session(IDDURATANUMRIGA)
            If IsNothing(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            If String.IsNullOrEmpty(myIDDurataNumR) Then
                myIDDurataNumR = "0"
            End If
            '--
            Dim myCodFiliale As String = "" : Dim myDataSc As String = ""
            myCodFiliale = GetDestApp(myID, myIDDurataNumR, Formatta.FormattaNomeFile(txtSerie.Text.Trim), Formatta.FormattaNomeFile(txtLotto.Text.Trim), myDataSc)
            CallBackWFPElencoDestCF(myCodFiliale, "")
            If myDataSc.Trim <> "" Then txtDataSc.Text = myDataSc.Trim.Trim
        Else
            btnDelDestD_Click(Nothing, Nothing)
        End If
        txtNote.Focus()
    End Sub
    Private Function GetDestApp(ByVal pID As String, ByVal pIDDurataNumR As String, ByVal pSerie As String, ByVal pLotto As String, ByRef pDataSc As String) As String
        GetDestApp = "" : pDataSc = ""
        If String.IsNullOrEmpty(pID) Then Exit Function
        If String.IsNullOrEmpty(pIDDurataNumR) Then Exit Function
        If Not IsNumeric(pID) Then Exit Function
        If Not IsNumeric(pIDDurataNumR) Then Exit Function
        If String.IsNullOrEmpty(pSerie) Then pSerie = ""
        If String.IsNullOrEmpty(pLotto) Then pLotto = ""
        If pSerie.Trim = "" And pLotto.Trim = "" Then
            Return ""
            Exit Function
        End If
        '-
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        Dim rowArt() As DataRow
        '(DurataNumRiga = " & pIDDurataNumR.Trim & ") 
        strSQL = "SELECT TOP(1) Cod_Filiale, DataSc " & _
                    "FROM ContrattiD " & _
                    "WHERE (IDDocumenti = " & pID.Trim & ") AND (DurataNum = 0) " & _
                    "AND (ISNULL(Serie,'') = N'" & pSerie.Trim & "') AND " & _
                    "(ISNULL(Lotto,'') = N'" & pLotto.Trim & "')"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    rowArt = dsArt.Tables(0).Select()
                    GetDestApp = IIf(IsDBNull(rowArt(0).Item("Cod_Filiale")), "", rowArt(0).Item("Cod_Filiale").ToString.Trim)
                    Try
                        pDataSc = IIf(IsDBNull(rowArt(0).Item("DataSc")), "", rowArt(0).Item("DataSc").ToString("dd/MM/yyyy"))
                    Catch ex As Exception
                        pDataSc = ""
                    End Try
                Else
                    GetDestApp = ""
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "N° Serie non trovato nelle Apparecchiature: " & pSerie.Trim + " " + pLotto.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                GetDestApp = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "N° Serie non trovato nelle Apparecchiature: " & pSerie.Trim + " " + pLotto.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            GetDestApp = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ContrattiDett.GetDestApp", "Lettura contratti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Private Sub btnModificaNoteInterv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaNoteInterv.Click
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPDETTDOCL
        Session(SWOPDETTDOCL) = SWSI
        btnAnnullaModNoteInterv.Enabled = True
        btnModificaNoteInterv.Visible = False
        btnAggiornaNoteInterv.Visible = True
        txtNoteIntervento.Enabled = True
        If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
            Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
            If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
                txtNoteIntervento.Text = ""
                Exit Sub
            End If
            NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
            txtNoteIntervento.Text = GetNoteSL(txtNoteInterventoALL.Text.Trim, NSL.Trim)
        Else
            txtNoteIntervento.Text = ""
        End If
    End Sub

    Private Sub btnAggiornaNoteInterv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiornaNoteInterv.Click
        Dim strErrore As String = ""
        If Me.UpgNoteIntervento(strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento UpgNoteIntervento: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        btnAnnullaModNoteInterv.Enabled = False
        btnModificaNoteInterv.Visible = True
        btnAggiornaNoteInterv.Visible = False
        txtNoteIntervento.Enabled = False
    End Sub
    Private Function UpgNoteIntervento(ByRef strErrore As String) As Boolean
        UpgNoteIntervento = False
        strErrore = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        'giu030322
        Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
        If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
            strErrore = "Errore: N° Serie non selezionata - Riprovate la modifica uscendo e rientrando (UpgNoteIntervento)"
            Exit Function
        End If
        NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
        If IsNothing(NSL) Then
            NSL = ""
        End If
        If String.IsNullOrEmpty(NSL) Then
            NSL = ""
        End If
        If NSL = "" Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (NSL) - Riprovate la modifica uscendo e rientrando (UpgNoteIntervento)"
            Exit Function
        End If
        '-
        'GIU020322 NOTE PUNTUALI PER N° SERIE LOTTO
        Dim ListaSL As New List(Of String)
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim OKNoteRitiro As String = ""
        txtNoteIntervento.Text.Trim.Replace("§", " ")
        Dim StrDato() As String
        Try
            If Not (Session(L_NSERIELOTTO) Is Nothing) Then
                ListaSL = Session(L_NSERIELOTTO)
            Else
                Call GetNoteSL(txtNoteInterventoALL.Text.Trim, mySL.Trim)
                ListaSL = Session(L_NSERIELOTTO)
            End If
            '-------------------------------------------
            Dim SWTrovatoNSL As Boolean = False 'GIU070423 SE NON TROVO NSL LO INSERISCO 
            If ListaSL.Count > 0 Then
                OKNoteRitiro = ""
                SWTrovatoNSL = False
                For I = 0 To ListaSL.Count - 1
                    StrDato = ListaSL(I).Split("§")
                    mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                    myNoteRitiro = StrDato(1)
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    If mySL = NSL Then
                        SWTrovatoNSL = True
                        OKNoteRitiro += mySL + "§" + txtNoteIntervento.Text.Trim
                    Else
                        OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                    End If
                Next
                If SWTrovatoNSL = False Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                    If OKNoteRitiro.Trim <> "" Then
                        OKNoteRitiro += "§"
                    End If
                    OKNoteRitiro += NSL + "§" + txtNoteIntervento.Text.Trim
                    'If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                End If
            Else
                Call SetNoteSLALLApp(txtNoteIntervento.Text.Trim)
                ListaSL = Session(L_NSERIELOTTO)
                SWTrovatoNSL = False
                If ListaSL.Count > 0 Then
                    OKNoteRitiro = ""
                    For I = 0 To ListaSL.Count - 1
                        StrDato = ListaSL(I).Split("§")
                        mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                        myNoteRitiro = StrDato(1)
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        If mySL = NSL Then
                            SWTrovatoNSL = True
                            OKNoteRitiro += mySL + "§" + txtNoteIntervento.Text.Trim
                        Else
                            OKNoteRitiro += mySL + "§" + myNoteRitiro.Trim
                        End If
                    Next
                    If SWTrovatoNSL = False Then 'GIU070423 SE NON TROVO NSL LO INSERISCO 
                        If OKNoteRitiro.Trim <> "" Then
                            OKNoteRitiro += "§"
                        End If
                        OKNoteRitiro += NSL + "§" + txtNoteIntervento.Text.Trim
                        'If pAllaPresenza.Trim <> "" Then OKNoteRitiro += " |" + pAllaPresenza.Trim + "|"
                    End If
                Else
                    OKNoteRitiro = NSL + "§" + txtNoteIntervento.Text.Trim
                End If
            End If
        Catch ex As Exception
            strErrore = "Errore: Aggiorna Note Intervento (UpgNoteIntervento): " + ex.Message.Trim
            Exit Function
        End Try
        '---------------------------------------------
        'giu060422 non funziona bene strSQL = "UPDATE ContrattiT SET NoteRitiro='" & Controlla_Apice(OKNoteRitiro) & "' Where IDDocumenti=" & myID.Trim
        Dim myErrore As String = ""
        '---------
        Dim ObjDB As New DataBaseUtility
        Try
            'giu040324 corretto errore ''''''' aggiungeva ad ogni agg un apice in piu tolto il contolla_apice
            SWOk = ObjDB.ExecUpgNoteRitiro(myID.Trim, OKNoteRitiro.Trim, myErrore)
            If SWOk = False Then
                ObjDB = Nothing
                strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento)"
                Exit Function
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento) - " & Ex.Message.Trim & " -  " & myErrore
            Exit Function
        End Try
        txtNoteInterventoALL.Text = OKNoteRitiro
        UpgNoteIntervento = True
    End Function

    Private Sub btnAnnullaModNoteInterv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnullaModNoteInterv.Click
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        btnAnnullaModNoteInterv.Enabled = False
        btnModificaNoteInterv.Visible = True
        btnAggiornaNoteInterv.Visible = False
        txtNoteIntervento.Enabled = False
        '-NoteIntervento 
        If DDLTipoDettagli.SelectedIndex = 0 Then 'APPARECCHIATURE
            Dim NSL As String = DDLDurNumRIga.SelectedItem.Text
            If NSL.Trim = "" Or InStr(NSL, "[") > 0 Or InStr(NSL, "Nuova") > 0 Then
                txtNoteIntervento.Text = ""
                Exit Sub
            End If
            NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
            txtNoteIntervento.Text = GetNoteSL(txtNoteInterventoALL.Text.Trim, NSL.Trim)
        Else
            txtNoteIntervento.Text = ""
        End If
    End Sub
    'giu041123
    Public Function GetNoteIntervento() As String
        GetNoteIntervento = txtNoteInterventoALL.Text.Trim
    End Function
    'giu231023
    Private Sub DDLRespAreaApp_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespAreaApp.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        Session(IDRESPAREAAPP) = DDLRespAreaApp.SelectedValue
        '-
        SqlDSRespVisiteApp.DataBind()
        DDLRespVisiteApp.Items.Clear()
        DDLRespVisiteApp.Items.Add("")
        DDLRespVisiteApp.DataBind()
        DDLRespVisiteApp.BackColor = SEGNALA_OK
        '-- mi riposiziono 
        DDLRespVisiteApp.AutoPostBack = False
        DDLRespVisiteApp.SelectedIndex = -1
        DDLRespVisiteApp.AutoPostBack = True
        '--
    End Sub
    Private Sub DDLRespVisiteApp_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRespVisiteApp.SelectedIndexChanged
        Session(SWMODIFICATO) = SWSI
        Session(IDRESPVISITEAPP) = DDLRespVisiteApp.SelectedValue
        DDLRespVisiteApp.BackColor = SEGNALA_OK
    End Sub

    Private Sub chkEvasa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEvasa.CheckedChanged
        If chkEvasa.Checked Then
            If txtDataEv.Text.Trim = "" Then
                txtDataEv.Text = Format(Now.Date, FormatoData)
            End If
        Else
            txtDataEv.Text = ""
        End If
        
    End Sub

    Private Sub chkFatturata_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturata.CheckedChanged
        If chkFatturata.Checked = False Then
            lblQtaFa.Text = "0"
        Else
            lblQtaFa.Text = txtQtaIns.Text.Trim
        End If
    End Sub
End Class