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

Partial Public Class WUC_DocumentiDett
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

#Region "Def per gestire il GRID dettagli"
    Private aDataView1 As DataView
    Private aDataView1L As DataView
    '---
    Private SqlAdapDocDett As SqlDataAdapter
    Private SqlAdapDocDettL As SqlDataAdapter
    '---
    Private SqlConnDocDett As SqlConnection
    Private SqlConnDocDettL As SqlConnection
    '---
    Private SqlDbSelectCmd As SqlCommand
    Private SqlDbInserCmd As SqlCommand
    Private SqlDbUpdateCmd As SqlCommand
    Private SqlDbDeleteCmd As SqlCommand
    '---
    Private SqlDbSelectCmdL As SqlCommand
    Private SqlDbInserCmdL As SqlCommand
    Private SqlDbUpdateCmdL As SqlCommand
    Private SqlDbDeleteCmdL As SqlCommand
    '---
    Private DsDocumentiDett As New DSDocumenti
    Private DsDocumentiDettL As New DSDocumenti
    '---

    Public Enum CellIdx
        Riga = 3
        CodArt = 4
        DesArt = 5
        UM = 6
        Qta = 7
        QtaEv = 8
        QtaRe = 9
        QtaAL = 10
        Conf = 11
        IVA = 12
        Prz = 13
        TipoScM = 14
        ScVal = 15
        Sc1 = 16
        Sc2 = 17
        Note = 18
        Importo = 19
        Deduzione = 20
        ScR = 21
        CAge = 22
        PAge = 23 'giu300312
        ImpProvvAge = 24 'giu300312
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
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            Session(ERROREALL) = SWSI
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'GIU170112 giu140512 i lotti solo nei documenti di trasporto
        If myTipoDoc = SWTD(TD.Preventivi) Or _
           myTipoDoc = SWTD(TD.OrdClienti) Or _
           myTipoDoc = SWTD(TD.OrdDepositi) Or _
           myTipoDoc = SWTD(TD.OrdFornitori) Or _
           myTipoDoc = SWTD(TD.PropOrdFornitori) Then
            PanelDettArtLottiNrSerie.Enabled = False
            ' ''PanelDettArtLottiNrSerie.Visible = False
            PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
        End If
        '-----------------------------------------------------------
        Dim myEs As String = Session(ESERCIZIO)
        If IsNothing(myEs) Then
            myEs = ""
        End If
        If String.IsNullOrEmpty(myEs) Then
            myEs = ""
        End If
        If myEs.Trim = "" Or Not IsNumeric(myEs) Then
            _WucElement.Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If
        txtCodArtIns.MaxLength = App.GetParamGestAzi(Session(ESERCIZIO)).LunghezzaMaxCodice 'GIU170112 + CSTLOpz
        'txtCodOpzione.MaxLength = CSTLOpz
        If lblMessAgg.Text.ToString.Trim = "" Then
            lblMessAgg.BorderStyle = BorderStyle.None
        Else
            lblMessAgg.BorderStyle = BorderStyle.Outset
        End If
        If lblSuperatoScMax.Text.ToString.Trim = "" Then
            lblSuperatoScMax.BorderStyle = BorderStyle.None
        Else
            lblSuperatoScMax.BorderStyle = BorderStyle.Outset
        End If
        If Not IsPostBack Then
            lblMessAgg.ForeColor = Drawing.Color.Blue
            lblSuperatoScMax.ForeColor = Drawing.Color.Blue
            checkNoScontoValore.ForeColor = Drawing.Color.DarkBlue
            'giu190412 GIU270412
            SetSWPrezzoALCSG()
            '----------
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            '-
            SetCdmDAdp()
            'giu241111
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            '----------
            If Session(SWOP) = SWOPNUOVO Then
                DsDocumentiDett.DocumentiD.Clear()
                DsDocumentiDett.DocumentiD.AcceptChanges()
                '-
                DsDocumentiDettL.DocumentiDLotti.Clear()
                DsDocumentiDettL.DocumentiDLotti.AcceptChanges()

                Session("aDataView1") = aDataView1
                Session("aSqlAdap") = SqlAdapDocDett
                Session("aDsDett") = DsDocumentiDett
                '------------------------------------------------------------------------------------
            Else
                If myID = "" Or Not IsNumeric(myID) Then
                    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (DocDett.Load)")
                    Exit Sub
                End If
                SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                SqlAdapDocDett.Fill(DsDocumentiDett.DocumentiD)
            End If
            AzzQtaNULL()
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsDocumentiDett
            '---
            Call ImpostaGriglia()
            'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
            '---
            Call AzzeraTxtInsArticoli() 'giu021211
            '---
            aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            Session("aDataView1") = aDataView1
            GridViewDett.DataBind()
            If GridViewDett.Rows.Count = 0 Then
                SetBtnPrimaRigaEnabled(True)
                'giu241111 non dovrebbe mai arrivare come nuovo perche creo subito la testata e imposto MODIFICA
                Dim newRowDocD As DSDocumenti.DocumentiDRow = DsDocumentiDett.DocumentiD.NewDocumentiDRow
                With newRowDocD
                    .BeginEdit()
                    .IDDocumenti = CLng(myID)
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
                    DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
                    newRowDocD = Nothing
                    SqlAdapDocDett = Session("aSqlAdap")
                Catch Ex As Exception
                    _WucElement.Chiudi("Errore nel caricamento DocumentiDett_Load: inserimento 1° Riga: " & Ex.Message)
                    Exit Sub
                End Try
                Session("aDataView1") = aDataView1
                Session("aSqlAdap") = SqlAdapDocDett
                Session("aDsDett") = DsDocumentiDett
                Session("aDataView1") = aDataView1
                GridViewDett.DataBind()
                SetBtnPrimaRigaEnabled(False)
                '------------------------------------------------------------------------------------
                LblTotaleLordo.Text = HTML_SPAZIO
                LblTotaleLordoPL.Text = HTML_SPAZIO
                _WucElement.SetLblTotLMPL(0, 0, 0)
            Else
                SetBtnPrimaRigaEnabled(False)
                'giu120318 aggiorno anche il Totale Merce in 3 pagina  
                Dim TotaleLordoMerce As Decimal = 0
                Dim TotaleLordoMercePL As Decimal = 0
                Dim TotaleDeduzioni As Decimal = 0
                For Each rsDettagli In DsDocumentiDett.DocumentiD.Select("", "Riga") 'giu290519 TOLTO "Importo<>0" 
                    'giu020519 FATTURE PER ACCONTI 
                    rsDettagli.BeginEdit()
                    If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                    rsDettagli.EndEdit()
                    rsDettagli.AcceptChanges()
                    If rsDettagli!DedPerAcconto = True Then
                        TotaleDeduzioni += rsDettagli![Importo]
                    Else
                        Select Case Left(myTipoDoc, 1)
                            Case "O"
                                If rsDettagli![Qta_Ordinata] <> 0 Then
                                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                    TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                End If
                            Case Else
                                If myTipoDoc = "PR" Then
                                    If rsDettagli![Qta_Ordinata] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                    End If
                                Else
                                    If rsDettagli![Qta_Evasa] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Evasa])
                                    End If
                                End If
                        End Select
                    End If
                Next
                'Valuta per i decimali per il calcolo
                Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
                If IsNothing(DecimaliVal) Then DecimaliVal = "2"
                If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
                If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                    DecimaliVal = "2" 'Euro
                End If
                LblTotaleLordo.Text = FormattaNumero(TotaleLordoMerce, Int(DecimaliVal))
                LblTotaleLordoPL.Text = FormattaNumero(TotaleLordoMercePL, Int(DecimaliVal))
                _WucElement.SetLblTotLMPL(TotaleLordoMerce, TotaleLordoMercePL, TotaleDeduzioni)
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
                Call AzzeraTxtInsArticoli() 'giu021211
            End Try
            '-- LOTTI 
            BuildLottiRigaDB(RigaSel)
            '---------
            EnableTOTxtInsArticoli(False)
            Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWMODIFICATO) = SWNO
        End If

        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_Seleziona1.Show()
        End If
        WFPLottiInsert1.WucElement = Me
        If Session(F_CARICALOTTI) = True Then
            WFPLottiInsert1.Show(False)
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
        Dim RowsD() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("")
        Dim RowD As DSDocumenti.DocumentiDRow
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
                    'GIU230512
                    If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = True Then
                        'GIU170112 giu140512 i lotti solo nei documenti di trasporto
                        If myTipoDoc = SWTD(TD.Preventivi) Or _
                           myTipoDoc = SWTD(TD.OrdClienti) Or _
                           myTipoDoc = SWTD(TD.OrdDepositi) Or _
                           myTipoDoc = SWTD(TD.OrdFornitori) Or _
                           myTipoDoc = SWTD(TD.PropOrdFornitori) Then
                            PanelDettArtLottiNrSerie.Enabled = False
                            ' ''PanelDettArtLottiNrSerie.Visible = False
                            PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
                            lblMessAgg.Text = "ATTENZIONE, per inserire Lotti/N°Serie creare il DDT"
                            lblMessAgg.BorderStyle = BorderStyle.Outset
                        End If
                    End If
                    '------------
                    Exit Sub
                Else
                    SetBtnPrimaRigaEnabled(False)
                End If
            End If
            '---------------------
            Dim myRowIndex As Integer = GridViewDett.SelectedIndex + (GridViewDett.PageSize * GridViewDett.PageIndex)
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
                txtQtaEv.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Evasa"), -1)
            Catch ex As Exception
                txtQtaEv.Text = "0"
            End Try
            Try
                txtQtaInv.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Allestita"), -1)
            Catch ex As Exception
                txtQtaInv.Text = "0"
            End Try
            Try
                LblQtaRe.Text = FormattaNumero(aDataView1.Item(myRowIndex).Item("Qta_Residua"), -1)
            Catch ex As Exception
                LblQtaRe.Text = "0"
            End Try
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
            Catch ex As Exception
                txtPrezzoIns.Text = "0"
            End Try
            'giu300419
            Dim myDed As String = ""
            Try
                myDed = aDataView1.Item(myRowIndex).Item("DedPerAcconto")
            Catch ex As Exception
                myDed = ""
            End Try
            If myDed.Trim <> "True" Then
                ChkDedPerAcc.Checked = False
            Else
                ChkDedPerAcc.Checked = True
            End If
            '---------
            'giu160112
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
                    If CDbl(txtQtaIns.Text) > 0 Or _
                        CDbl(txtQtaIns.Text) > 0 Or _
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
            '------------------------------------------
            'giu170112 DATI GIACENZA/DISPONIBILITA'
            GetDatiGiacenza(txtCodArtIns.Text.Trim)
        Catch ex As Exception
            AzzeraTxtInsArticoli()
        End Try
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        'GIU230512
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = True Then
            'GIU170112 giu140512 i lotti solo nei documenti di trasporto
            If myTipoDoc = SWTD(TD.Preventivi) Or _
               myTipoDoc = SWTD(TD.OrdClienti) Or _
               myTipoDoc = SWTD(TD.OrdDepositi) Or _
               myTipoDoc = SWTD(TD.OrdFornitori) Or _
               myTipoDoc = SWTD(TD.PropOrdFornitori) Then
                PanelDettArtLottiNrSerie.Enabled = False
                ' ''PanelDettArtLottiNrSerie.Visible = False
                PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
                lblMessAgg.Text = "ATTENZIONE, per inserire Lotti/N°Serie creare il DDT"
                lblMessAgg.BorderStyle = BorderStyle.Outset
            End If
        End If
        '------------
    End Sub
    Private Function GetDatiGiacenza(ByVal CodArt As String) As Boolean
        'GIU230920
        Dim myCMag As String = Session(IDMAGAZZINO)
        If IsNothing(myCMag) Then
            myCMag = "0"
        End If
        If String.IsNullOrEmpty(myCMag) Then
            myCMag = "0"
        End If
        '---------
        lblGiacenza.Text = "" : lblGiacenza.ForeColor = Drawing.Color.Black
        lblGiacImp.Text = ""
        lblOrdFor.Text = ""
        lblDataArr.Text = ""
        If CodArt.Trim = "" Then Exit Function

        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        Dim rowArt() As DataRow
        If myCMag = "0" Then
            strSQL = "Select * From AnaMag WHERE Cod_Articolo = '" & CodArt.Trim & "'"
        Else
            strSQL = "Select * From ArtDiMag WHERE Codice_Magazzino=" + myCMag.Trim + " AND Cod_Articolo = '" & CodArt.Trim & "'"
        End If
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    rowArt = dsArt.Tables(0).Select()
                    lblGiacenza.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giacenza")), 0, rowArt(0).Item("Giacenza")), -1)
                    If lblGiacenza.Text.Trim = "0" Then lblGiacenza.Text = ""
                    If IsNumeric(lblGiacenza.Text.Trim) Then 'GIU270412
                        If CDbl(lblGiacenza.Text.Trim) < 0 Then
                            lblGiacenza.ForeColor = SEGNALA_KO
                        Else
                            lblGiacenza.ForeColor = Drawing.Color.Black
                        End If
                    End If
                    lblGiacImp.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giac_Impegnata")), 0, rowArt(0).Item("Giac_Impegnata")), -1)
                    If lblGiacImp.Text.Trim = "0" Then lblGiacImp.Text = ""
                    'GIU230920
                    If myCMag = "0" Then
                        lblOrdFor.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Ord_Fornit")), 0, rowArt(0).Item("Ord_Fornit")), -1)
                    Else
                        lblOrdFor.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Ordinati")), 0, rowArt(0).Item("Ordinati")), -1)
                    End If
                    '---
                    If lblOrdFor.Text.Trim = "0" Then lblOrdFor.Text = ""
                    lblDataArr.Text = ""
                    Comodo = IIf(IsDBNull(rowArt(0).Item("Data_Arrivo")), "", rowArt(0).Item("Data_Arrivo"))
                    If IsDate(Comodo.Trim) Then
                        If CDate(Comodo.Trim) = DATANULL Then
                            Comodo = ""
                        End If
                    Else
                        Comodo = ""
                    End If
                    If Comodo.Trim <> "" Then
                        lblDataArr.Text = Format(CDate(Comodo), FormatoData) & " "
                        Comodo = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("QtaArrivoFornit")), 0, rowArt(0).Item("QtaArrivoFornit")), -1)
                        If CDec(Comodo.Trim) > 0 Then
                            lblDataArr.Text += "(" & Comodo.Trim & ")"
                        End If
                    End If
                    'giu040914 DISTINTA BASE
                    ' ''GetDatiGiacenzaDB(CodArt.Trim)
                    '-----------------------
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    If myCMag = "0" Then 'giu230920
                        ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Else
                        ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nel Magazzino: (" + myCMag.Trim + ") - CArt.:" & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                    End If
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                If myCMag = "0" Then 'giu230920
                    ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                Else
                    ModalPopup.Show("Attenzione", "Non trovato Cod.Articolo nel Magazzino: (" + myCMag.Trim + ") - CArt.:" & CodArt.Trim, WUC_ModalPopup.TYPE_ERROR)
                End If
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GetDatiGiacenza", "Lettura articoli: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    
    Private Sub AzzeraTxtInsArticoli()
        'giu021211
        txtCodArtIns.AutoPostBack = False
        txtIVAIns.AutoPostBack = False
        txtCodArtIns.Text = "" : txtCodArtIns.BackColor = SEGNALA_OK : txtDesArtIns.Text = ""
        lblBase.Text = "" : lblOpz.Text = ""
        txtUMIns.Text = "" : txtQtaIns.Text = "" : txtQtaEv.Text = "" : LblQtaRe.Text = HTML_SPAZIO : txtQtaInv.Text = ""
        txtQtaEv.BackColor = SEGNALA_OK
        txtQtaInv.BackColor = SEGNALA_OK
        txtIVAIns.Text = "" : txtPrezzoIns.Text = "" : txtSconto1Ins.Text = ""
        txtCodArtIns.AutoPostBack = True
        txtIVAIns.AutoPostBack = True
        '---
        LblPrezzoNetto.Text = HTML_SPAZIO : LblImportoRiga.Text = HTML_SPAZIO
        ChkDedPerAcc.Checked = False
        'giu170112 DATI GIACENZA/DISPONIBILITA'
        lblGiacenza.Text = "" : lblGiacenza.ForeColor = Drawing.Color.Black
        lblGiacImp.Text = ""
        lblOrdFor.Text = ""
        lblDataArr.Text = ""
        '--------------------------------------
        checkNoScontoValore.Checked = True 'giu160112

        lblMessAgg.BorderStyle = BorderStyle.None
        lblMessAgg.Text = ""
        'giu300312
        lblSuperatoScMax.BorderStyle = BorderStyle.None
        lblSuperatoScMax.Text = ""
    End Sub
    Private Sub EnableTOTxtInsArticoli(ByVal SW As Boolean) 'GIU051211
        'giu021211
        txtCodArtIns.Enabled = SW : txtDesArtIns.Enabled = SW
        txtUMIns.Enabled = SW : txtQtaIns.Enabled = SW : txtQtaEv.Enabled = SW : txtQtaInv.Enabled = SW
        ChkDedPerAcc.Enabled = SW
        txtIVAIns.Enabled = SW : txtPrezzoIns.Enabled = SW : txtSconto1Ins.Enabled = SW
        txtIVAIns.BackColor = Def.SEGNALA_OK
        '---
        btnPrimaRiga.Enabled = SW : BtnSelArticolo.Enabled = SW : btnNewRigaUp.Enabled = SW
        BtnSelArticoloIns.Enabled = SW
        btnAggArtGridSel.Enabled = SW 'giu061211 uso sempre aggriga :: btnInsRigaDett.Enabled = SW
        checkNoScontoValore.Enabled = SW
        btnPrimaRigaL.Enabled = SW
        btnCaricoLotti.Enabled = SW
        'giu070516
        If Session(SWOP) = SWOPMODIFICA Then
            If txtCodArtIns.Text.Trim <> "" Then
                SetBtnCaricoLottiEnabled(True)
                GridViewDettL.Enabled = True
            Else
                SetBtnCaricoLottiEnabled(False)
                GridViewDettL.Enabled = False
            End If
        End If
        If SW = False Then txtPrezzoCosto.Enabled = False
    End Sub
    Private Sub SetBtnPrimaRigaEnabled(ByVal SW As Boolean)
        btnPrimaRiga.Enabled = SW : btnPrimaRiga.Visible = SW
        BtnSelArticolo.Visible = Not SW : BtnSelArticolo.Enabled = Not SW : btnNewRigaUp.Enabled = SW
        btnPrimaRigaL.Enabled = Not SW
        btnCaricoLotti.Enabled = Not SW
        If btnPrimaRiga.Visible = True Then
            lblRigaSel.Text = HTML_SPAZIO
        End If
    End Sub
    Private Sub SetBtnPrimaRigaLEnabled(ByVal SW As Boolean)
        btnPrimaRigaL.Enabled = SW : btnPrimaRigaL.Visible = SW
    End Sub

    Private Sub SetBtnCaricoLottiEnabled(ByVal SW As Boolean)
        btnCaricoLotti.Enabled = SW
    End Sub

#Region "Imposta Command e DataAdapter"
    Private Function SetCdmDAdp() As Boolean

        SqlConnDocDett = New SqlConnection
        SqlAdapDocDett = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        SqlDbInserCmd = New SqlCommand
        SqlDbUpdateCmd = New SqlCommand
        SqlDbDeleteCmd = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDett.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
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
        SqlDbSelectCmd.CommandText = "get_DocDByIDDocumenti" 'ok select *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDocDett
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1 GIU130412
        '
        SqlDbInserCmd.CommandText = "insert_DocDByIDDocumenti"
        SqlDbInserCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmd.Connection = Me.SqlConnDocDett
        SqlDbInserCmd.CommandTimeout = myTimeOUT
        SqlDbInserCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
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
            New System.Data.SqlClient.SqlParameter("@DedPerAcconto", System.Data.SqlDbType.Bit, 0, "DedPerAcconto")})
        '
        'SqlUpdateCommand1 GIU130412
        '
        SqlDbUpdateCmd.CommandText = "update_DocDByIDDocumenti"
        SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdateCmd.Connection = Me.SqlConnDocDett
        SqlDbUpdateCmd.CommandTimeout = myTimeOUT
        SqlDbUpdateCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
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
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing)})
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmd.CommandText = "delete_DocDByIDDocumenti"
        SqlDbDeleteCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmd.Connection = Me.SqlConnDocDett
        SqlDbDeleteCmd.CommandTimeout = myTimeOUT
        SqlDbDeleteCmd.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Cod_Articolo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Descrizione", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Descrizione", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Descrizione", System.Data.SqlDbType.NVarChar, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Descrizione", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Um", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Um", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Um", System.Data.SqlDbType.NVarChar, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Um", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Qta_Ordinata", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Qta_Ordinata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Cod_Iva", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Cod_Iva", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Cod_Iva", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Iva", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Prezzo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Prezzo", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Prezzo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Importo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Importo", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Importo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Importo", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Qta_Evasa", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Qta_Evasa", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Qta_Evasa", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Evasa", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Qta_Residua", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Qta_Residua", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Qta_Residua", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Residua", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_1", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_1", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_1", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_1", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_2", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_2", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_2", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_2", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_ScontoReale", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "ScontoReale", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_ScontoReale", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoReale", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_ScontoValore", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "ScontoValore", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_ScontoValore", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoValore", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_ImportoProvvigione", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_ImportoProvvigione", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Pro_Agente", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Pro_Agente", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Pro_Agente", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Pro_Agente", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Cod_Agente", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Cod_Agente", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Cod_Agente", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Agente", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Confezione", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Confezione", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Confezione", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Confezione", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Prezzo_Netto", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Prezzo_Netto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_SWPNettoModificato", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "SWPNettoModificato", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_SWPNettoModificato", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "SWPNettoModificato", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Prezzo_Netto_Inputato", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Prezzo_Netto_Inputato", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_3", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_3", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_3", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_3", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_4", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_4", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_4", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_4", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_Pag", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_Pag", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_Pag", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Pag", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Sconto_Merce", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Sconto_Merce", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Sconto_Merce", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Merce", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Note", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Note", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Note", System.Data.SqlDbType.NVarChar, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Note", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@Original_OmaggioImponibile", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OmaggioImponibile", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@Original_OmaggioImposta", System.Data.SqlDbType.Bit, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OmaggioImposta", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_NumeroPagina", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "NumeroPagina", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_NumeroPagina", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NumeroPagina", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_N_Pacchi", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "N_Pacchi", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_N_Pacchi", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "N_Pacchi", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Qta_Casse", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Qta_Casse", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Qta_Casse", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Qta_Casse", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Flag_Imb", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Flag_Imb", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Flag_Imb", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Flag_Imb", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Riga_Trasf", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Riga_Trasf", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Riga_Trasf", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga_Trasf", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_Riga_Appartenenza", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "Riga_Appartenenza", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_Riga_Appartenenza", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga_Appartenenza", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefInt", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefInt", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefInt", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefInt", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefNumPrev", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefNumPrev", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefNumPrev", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefNumPrev", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefDataPrev", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefDataPrev", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefDataPrev", System.Data.SqlDbType.DateTime, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefDataPrev", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefNumOrd", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefNumOrd", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefNumOrd", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefNumOrd", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefDataOrd", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefDataOrd", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefDataOrd", System.Data.SqlDbType.DateTime, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefDataOrd", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefNumDDT", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefNumDDT", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefNumDDT", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefNumDDT", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefDataDDT", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefDataDDT", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefDataDDT", System.Data.SqlDbType.DateTime, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefDataDDT", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefNumNC", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefNumNC", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefNumNC", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefNumNC", System.Data.DataRowVersion.Original, Nothing), New System.Data.SqlClient.SqlParameter("@IsNull_RefDataNC", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, CType(0, Byte), CType(0, Byte), "RefDataNC", System.Data.DataRowVersion.Original, True, Nothing, "", "", ""), New System.Data.SqlClient.SqlParameter("@Original_RefDataNC", System.Data.SqlDbType.DateTime, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RefDataNC", System.Data.DataRowVersion.Original, Nothing)})

        SqlAdapDocDett.SelectCommand = SqlDbSelectCmd
        SqlAdapDocDett.InsertCommand = SqlDbInserCmd
        SqlAdapDocDett.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDocDett.UpdateCommand = SqlDbUpdateCmd
        Session("aSqlAdap") = SqlAdapDocDett

        '--- LOTTI @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        SqlConnDocDettL = New SqlConnection
        SqlAdapDocDettL = New SqlDataAdapter
        SqlDbSelectCmdL = New SqlCommand
        SqlDbInserCmdL = New SqlCommand
        SqlDbUpdateCmdL = New SqlCommand
        SqlDbDeleteCmdL = New SqlCommand

        '' ''Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDettL.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmdL.CommandText = "get_DocDLByIDDocRiga"
        SqlDbSelectCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmdL.Connection = Me.SqlConnDocDettL
        SqlDbSelectCmdL.CommandTimeout = myTimeOUT
        SqlDbSelectCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1
        '
        SqlDbInserCmdL.CommandText = "insert_DocDLByIDDocRiga"
        SqlDbInserCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdL.Connection = Me.SqlConnDocDettL
        SqlDbInserCmdL.CommandTimeout = myTimeOUT
        SqlDbInserCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
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
        SqlDbUpdateCmdL.CommandText = "update_DocDLByIDDocRiga"
        SqlDbUpdateCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdateCmdL.Connection = Me.SqlConnDocDettL
        SqlDbUpdateCmdL.CommandTimeout = myTimeOUT
        SqlDbUpdateCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie"), _
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_Riga", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Riga", System.Data.DataRowVersion.Original, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_NCollo", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NCollo", System.Data.DataRowVersion.Original, Nothing)})
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmdL.CommandText = "delete_DocDLByIDDocRiga"
        SqlDbDeleteCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmdL.Connection = Me.SqlConnDocDettL
        SqlDbDeleteCmdL.CommandTimeout = myTimeOUT
        SqlDbDeleteCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@Original_IDDocumenti", System.Data.SqlDbType.Int, 0, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "IDDocumenti", System.Data.DataRowVersion.Original, Nothing), _
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
        'Tipo Documento per sapere quale Quanita' devo prendere vedi function CalcolaTotaleDoc
        If myTipoDoc = "" Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        ImpostaGrid()
        ImpostaGridL()
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

        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
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
        'GIU271211
        Select Case myTipoDoc
            Case "OC"
                nameColumn5.HeaderText = "Quantità allest."
            Case Else
                nameColumn5.HeaderText = "Quantità evasa"
        End Select
        'nameColumn5.DataFormatString = "{0:###}"
        nameColumn5.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn5.ItemStyle.Wrap = False
        nameColumn5.HeaderStyle.Width = Unit.Pixel(40)
        nameColumn5.ItemStyle.Width = Unit.Pixel(40)
        'GIU271211
        Select Case myTipoDoc
            Case "PR", "PF" 'GIU020212
                nameColumn5.Visible = False
            Case "OF"
                nameColumn5.Visible = True
            Case Else
                nameColumn5.Visible = True
        End Select
        nameColumn5.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn5)

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
        'GIU271211
        'giu010112 nameColumn6.ReadOnly = True
        Select Case myTipoDoc
            Case "PR", "PF" 'GIU020212
                nameColumn6.Visible = False
            Case Else
                nameColumn6.Visible = True
                ' ''Case "O"
                ' ''    nameColumn6.Visible = True
                ' ''Case Else
                ' ''    If myTipoDoc = "PR" OR MYTIPODOC = "PF" Then
                ' ''        nameColumn6.Visible = False
                ' ''    Else
                ' ''        nameColumn6.Visible = True
                ' ''    End If
        End Select
        nameColumn6.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn6)

        Dim nameColumnQAL As New BoundField
        nameColumnQAL.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumnQAL.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumnQAL.HeaderStyle.Wrap = True
        nameColumnQAL.DataField = "Qta_Allestita"
        'GIU271211
        Select Case myTipoDoc
            Case "OC"
                nameColumnQAL.HeaderText = "Quantità inviata"
            Case Else
                nameColumnQAL.HeaderText = "Quantità inviata"
        End Select
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

        Dim nameColumn16 As New BoundField
        nameColumn16.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn16.HeaderText = "Conf."
        nameColumn16.DataField = "Confezione"
        'nameColumn16.DataFormatString = "{0:###}"
        nameColumn16.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn16.ItemStyle.Wrap = False
        nameColumn16.Visible = False
        nameColumn16.HeaderStyle.Width = Unit.Pixel(10)
        nameColumn16.ItemStyle.Width = Unit.Pixel(10)
        nameColumn16.ReadOnly = True 'giu110112
        GridViewDett.Columns.Add(nameColumn16)

        Dim nameColumn7 As New BoundField
        nameColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn7.HeaderText = "IVA"
        nameColumn7.DataField = "Cod_Iva"
        'nameColumn7.DataFormatString = "{0:###}"
        nameColumn7.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn7.ItemStyle.Wrap = False
        nameColumn7.HeaderStyle.Width = Unit.Pixel(10)
        nameColumn7.ItemStyle.Width = Unit.Pixel(10)
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
        nameColDDL.Visible = True
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
        nameColDed.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColDed.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColDed.HeaderStyle.Wrap = True
        nameColDed.HeaderText = "Ded."
        nameColDed.DataField = "DedPerAcconto"
        nameColDed.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColDed.ItemStyle.Wrap = False
        nameColDed.HeaderStyle.Width = Unit.Pixel(15)
        nameColDed.ItemStyle.Width = Unit.Pixel(15)
        nameColDed.Visible = True
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
        nameColumn15.Visible = True
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
        nameColPAge.Visible = True
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
        nameColImpProvvAge.Visible = True
        GridViewDett.Columns.Add(nameColImpProvvAge)
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@ PANNELLLO DETTAGLI
        'giu051211 GIU271211
        lblDedPerAcc.Visible = False
        ChkDedPerAcc.Visible = False
        Select Case myTipoDoc
            Case "PR", "PF" 'GIU020212
                lblQtaEv.Visible = False
                txtQtaEv.Visible = False
                LblQtaInv.Visible = False
                txtQtaInv.Visible = False
                lblLabelQtaRe.Visible = False
                LblQtaRe.Visible = False
            Case "OC"
                lblQtaEv.Text = "Allest."
                lblQtaEv.Visible = True
                txtQtaEv.Visible = True
                LblQtaInv.Visible = True
                txtQtaInv.Visible = True
                lblLabelQtaRe.Visible = True
                LblQtaRe.Visible = True
            Case "OF"
                lblQtaEv.Visible = False
                txtQtaEv.Visible = False
                LblQtaInv.Visible = False
                txtQtaInv.Visible = False
                lblLabelQtaRe.Visible = False
                LblQtaRe.Visible = False
            Case "FC"
                lblQtaEv.Visible = True
                txtQtaEv.Visible = True
                LblQtaInv.Visible = False
                txtQtaInv.Visible = False
                lblLabelQtaRe.Visible = True
                LblQtaRe.Visible = True
                lblDedPerAcc.Visible = True
                ChkDedPerAcc.Visible = True
            Case "FC", "NC"
                lblQtaEv.Visible = True
                txtQtaEv.Visible = True
                LblQtaInv.Visible = False
                txtQtaInv.Visible = False
                lblLabelQtaRe.Visible = True
                LblQtaRe.Visible = True
            Case Else
                lblQtaEv.Visible = True
                txtQtaEv.Visible = True
                LblQtaInv.Visible = True
                txtQtaInv.Visible = True
                lblLabelQtaRe.Visible = True
                LblQtaRe.Visible = True
        End Select
    End Sub

    Private Sub ImpostaGridL()
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Dim ColoreSfondoIntes As String = "#CC6600"
        GridViewDettL.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDettL.Attributes.Add("style", "table-layout:fixed")

        Dim nameColumn0 As New BoundField
        nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.HeaderText = "Riga"
        nameColumn0.DataField = "NCollo"
        nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.ItemStyle.Wrap = False
        nameColumn0.HeaderStyle.Width = Unit.Pixel(25)
        nameColumn0.ItemStyle.Width = Unit.Pixel(25)
        nameColumn0.ReadOnly = True
        GridViewDettL.Columns.Add(nameColumn0)

        Dim nameColumn1 As New BoundField
        nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.HeaderStyle.Wrap = False
        nameColumn1.HeaderText = "Lotto"
        nameColumn1.DataField = "Lotto"
        nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.ItemStyle.Wrap = False
        nameColumn1.HeaderStyle.Width = Unit.Pixel(150)
        nameColumn1.ItemStyle.Width = Unit.Pixel(150)
        GridViewDettL.Columns.Add(nameColumn1)

        Dim nameColumn2 As New BoundField
        nameColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.HeaderText = "N° Serie"
        nameColumn2.DataField = "NSerie"
        nameColumn2.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.ItemStyle.Wrap = True   'per andare a capo
        nameColumn2.HeaderStyle.Width = Unit.Pixel(150)
        nameColumn2.ItemStyle.Width = Unit.Pixel(150)
        GridViewDettL.Columns.Add(nameColumn2)

        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn3.HeaderText = "Colli"
        nameColumn3.DataField = "QtaColli"
        nameColumn3.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn3.ItemStyle.Wrap = False
        nameColumn3.HeaderStyle.Width = Unit.Pixel(30)
        nameColumn3.ItemStyle.Width = Unit.Pixel(30)
        nameColumn3.Visible = True
        GridViewDettL.Columns.Add(nameColumn3)
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
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        BuildLottiRigaDB(RigaSel)
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
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        BuildLottiRigaDB(RigaSel)
        '---------
    End Sub
    Private Sub GridViewDettL_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles GridViewDettL.RowCancelingEdit
        GridViewDettL.EditIndex = -1
        'giu290312
        'RICARICO I LOTTI PER LA RIGA SELEZIONATA
        BuildLottiRigaDB(GridViewDett.SelectedDataKey.Value)
        '-----------
        aDataView1L = Session("aDataView1L")
        If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
        GridViewDettL.DataSource = aDataView1L
        GridViewDettL.DataBind()
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        'Abilito i DETTAGLI 
        GridViewDett.Enabled = True
        'GIU061211btnAggArtGridSel.Enabled = True 'giu061211 uso sempre aggriga :: btnInsRigaDett.Enabled = False
        EnableTOTxtInsArticoli(False)
        '--------------------------------------------------------------
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
                If IsNumeric(e.Row.Cells(CellIdx.QtaRe).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaRe).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaRe).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaRe).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellIdx.QtaAL).Text) Then
                    If CDec(e.Row.Cells(CellIdx.QtaAL).Text) <> 0 Then
                        e.Row.Cells(CellIdx.QtaAL).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.QtaAL).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.QtaAL).Text = ""
                    End If
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
                ' ''If IsDate(e.Row.Cells(6).Text) Then
                ' ''    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
                ' ''End If
            End If
        Catch ex As Exception
            Dim errore As Integer = 0
        End Try
    End Sub
    Private Sub GridViewDettL_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettL.RowDataBound
        Try
            If e.Row.DataItemIndex > -1 Then
                If IsNumeric(e.Row.Cells(CellIdxL.QtaCollo).Text) Then
                    If CDec(e.Row.Cells(CellIdxL.QtaCollo).Text) <> 0 Then
                        e.Row.Cells(CellIdxL.QtaCollo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxL.QtaCollo).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdxL.QtaCollo).Text = ""
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
            Passo = 1
            If CancellaLottiRiga(RigaDel) = False Then
                'NON FACCIO NULLA
            End If
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
            DsDocumentiDett = Session("aDsDett")
            Dim RowD As DSDocumenti.DocumentiDRow
            Passo = 3
            'GIU030117 
            Try
                If myCArtDel.Trim <> "" Then
                    Dim RowsDel() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga>" & RigaDel.ToString.Trim, "Riga")
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
            ' ''Dim RowsDel() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga_Appartenenza=" & RigaDel.ToString.Trim)
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
                aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            End If
            'Rinumero tutto
            Passo = 5
            'giu030117
            '' ''GIU210615
            ' ''Dim SaveRiga As Integer
            ' ''Dim RowsDRA() As DataRow
            ' ''Dim RowDRA As DSDocumenti.DocumentiDRow
            '---------
            Dim RowsD() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("", "Riga")
            Dim Riga As Integer = 1
            For Each RowD In RowsD
                If RowD.Riga <> Riga Then
                    'giu030117 SaveRiga = RowD.Riga
                    If RinumeraLottiRiga(RowD.Riga, Riga) = False Then 'giu170112
                        Exit Sub
                    End If
                    RowD.BeginEdit()
                    RowD.Riga = Riga
                    RowD.EndEdit()
                    'giu030117
                    '' ''GIU210615
                    ' ''RowsDRA = Me.DsDocumentiDett.DocumentiD.Select("Riga_Appartenenza=" & SaveRiga.ToString.Trim)
                    ' ''For Each RowDRA In RowsDRA
                    ' ''    RowDRA.BeginEdit()
                    ' ''    RowDRA.Riga_Appartenenza = Riga
                    ' ''    RowDRA.EndEdit()
                    ' ''Next
                    '---------
                End If
                Riga += 1
            Next
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
            SetBtnPrimaRigaLEnabled(False)
            SetBtnCaricoLottiEnabled(False)
            '-------------------------------------
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsDocumentiDett
            '---
            Passo = 7
            Dim strErrore As String = ""
            If AggiornaImporto(DsDocumentiDett, strErrore) = False Then
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
                AzzeraTxtInsArticoli()
            End Try
            '-- LOTTI 
            BuildLottiRigaDB(RigaSel)
            '-----------
        Catch Ex As Exception
            GridViewDett.Enabled = True
            Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GridViewDett_RowDeleting. Passo: " & Passo.ToString, Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        GridViewDett.Enabled = True
        Session(SWOPDETTDOC) = SWOPNESSUNA 'giu191111
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub
    'GIU170112
    Private Function RinumeraLottiRiga(ByVal RigaRin As Integer, ByVal NewRiga As Integer) As Boolean
        GridViewDettL.Enabled = False
        Try
            BuildLottiRigaDB(RigaRin)
            aDataView1L = Session("aDataView1L")
            SqlAdapDocDettL = Session("aSqlAdapL")
            DsDocumentiDettL = Session("aDsDettL")
            Dim RowD As DSDocumenti.DocumentiDLottiRow
            'Rinumero
            Dim RowsSel() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("")
            For Each RowD In RowsSel
                RowD.BeginEdit()
                RowD.Riga = NewRiga
                RowD.EndEdit()
            Next
            'AGGIORNO SUL DB
            Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
            RowsSel = Nothing
            '---
            If (aDataView1L Is Nothing) Then
                aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            End If
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            '--------------
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.EditIndex = -1
            GridViewDettL.DataBind()
            SetBtnCaricoLottiEnabled(True)
            If GridViewDettL.Rows.Count = 0 Then
                SetBtnPrimaRigaLEnabled(True)
            Else
                SetBtnPrimaRigaLEnabled(False)
            End If
            Session("aDataView1L") = aDataView1L
            Session("aSqlAdapL") = SqlAdapDocDettL
            Session("aDsDettL") = DsDocumentiDettL
            '---
            RinumeraLottiRiga = True
        Catch Ex As Exception
            GridViewDettL.Enabled = True
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.RinumeraLottiRiga", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        GridViewDettL.Enabled = True
        Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
        Session(SWMODIFICATO) = SWSI
    End Function
    Private Function CancellaLottiRiga(ByVal RigaDel As Integer) As Boolean
        GridViewDettL.Enabled = False
        Try
            aDataView1L = Session("aDataView1L")
            SqlAdapDocDettL = Session("aSqlAdapL")
            DsDocumentiDettL = Session("aDsDettL")
            Dim RowD As DSDocumenti.DocumentiDLottiRow
            'Cancello
            Dim RowsDel() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("Riga = " & RigaDel)
            For Each RowD In RowsDel
                RowD.Delete()
                'AGGIORNO SUL DB
                Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
            Next
            RowsDel = Nothing
            '---
            If (aDataView1L Is Nothing) Then
                aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            End If

            'Rinumero tutto
            Dim RowsD() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("", "NCollo")
            Dim NCollo As Integer = 1
            For Each RowD In RowsD
                If RowD.NCollo <> NCollo Then
                    RowD.BeginEdit()
                    RowD.NCollo = NCollo
                    RowD.EndEdit()
                    'AGGIORNO SUL DB
                    Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
                    '---------------
                End If
                NCollo += 1
            Next
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            '--------------
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.EditIndex = -1
            GridViewDettL.DataBind()
            SetBtnCaricoLottiEnabled(True)
            If GridViewDettL.Rows.Count = 0 Then
                SetBtnPrimaRigaLEnabled(True)
            Else
                SetBtnPrimaRigaLEnabled(False)
            End If
            Session("aDataView1L") = aDataView1L
            Session("aSqlAdapL") = SqlAdapDocDettL
            Session("aDsDettL") = DsDocumentiDettL
            '---
            CancellaLottiRiga = True
        Catch Ex As Exception
            GridViewDettL.Enabled = True
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GridViewDettL_RowDeleting", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        GridViewDettL.Enabled = True
        Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
        Session(SWMODIFICATO) = SWSI
    End Function
    Private Sub GridViewDettL_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles GridViewDettL.RowDeleting
        GridViewDettL.Enabled = False
        Try
            Dim myRowIndex As Integer = e.RowIndex + (GridViewDettL.PageSize * GridViewDettL.PageIndex)
            GridViewDettL.SelectedIndex = e.RowIndex
            Dim RigaDel As Integer = GridViewDettL.SelectedDataKey.Value
            aDataView1L = Session("aDataView1L")
            aDataView1L.Item(myRowIndex).Delete()
            SqlAdapDocDettL = Session("aSqlAdapL")
            DsDocumentiDettL = Session("aDsDettL")
            Dim RowD As DSDocumenti.DocumentiDLottiRow 'DocumentiDRow
            '---------------------------------- AGGIORNO
            Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
            If (aDataView1L Is Nothing) Then
                aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            End If
            'Rinumero tutto
            Dim RowsD() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("", "NCollo")
            Dim NCollo As Integer = 1
            For Each RowD In RowsD
                If RowD.NCollo <> NCollo Then
                    RowD.BeginEdit()
                    RowD.NCollo = NCollo
                    RowD.EndEdit()
                    'AGGIORNO SUL DB
                    Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
                    '---------------
                End If
                NCollo += 1
            Next
            DsDocumentiDettL.DocumentiDLotti.AcceptChanges() 'giu290312
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            '--------------
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.EditIndex = -1
            GridViewDettL.DataBind()
            SetBtnCaricoLottiEnabled(True)
            If GridViewDettL.Rows.Count = 0 Then
                SetBtnPrimaRigaLEnabled(True)
            Else
                SetBtnPrimaRigaLEnabled(False)
            End If
            Session("aDataView1L") = aDataView1L
            Session("aSqlAdapL") = SqlAdapDocDettL
            Session("aDsDettL") = DsDocumentiDettL
            '---
        Catch Ex As Exception
            GridViewDettL.Enabled = True
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GridViewDettL_RowDeleting", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'GIU160715 Abilito i DETTAGLI 
        GridViewDett.Enabled = True
        '----------------------------
        GridViewDettL.Enabled = True
        Session(SWOPDETTDOCL) = SWOPNESSUNA 'giu191111
        Session(SWMODIFICATO) = SWSI
    End Sub

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
            ModalPopup.Show("Errore in DocumentiDett.GridViewDett_RowEditing: posizionamento riga da modificare, ritentare la modifica.", ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
            ModalPopup.Show("Errore in DocumentiDett.GridViewDett_RowEditing", ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
        'giu010112 
        'questa è label
        ' ''CType(row.Cells(CellIdx.QtaRe).Controls(0), Label).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaRe).Controls(0), Label).Text, -1)
        Try
            CType(row.Cells(CellIdx.QtaRe).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaRe).Controls(0), TextBox).Text, -1)
        Catch ex As Exception
        End Try
        Try
            CType(row.Cells(CellIdx.QtaAL).Controls(0), TextBox).Text = FormattaNumero(CType(row.Cells(CellIdx.QtaAL).Controls(0), TextBox).Text, -1)
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
        BuildLottiRigaDB(GridViewDett.SelectedDataKey.Value)
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
    Private Sub GridViewDettL_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridViewDettL.RowEditing
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'giu160715
        '-------------------------
        Try
            GridViewDettL.SelectedIndex = e.NewEditIndex
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GridViewDettL_RowEditing: posizionamento riga LOTTI da modificare, ritentare la modifica.", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '------------------------------------------------------------------------
        'GIU301111 disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
        '------------------------------------------------------------------------
        If GridViewDett.EditIndex <> -1 Then
            SqlAdapDocDett = Session("aSqlAdap")
            DsDocumentiDett = Session("aDsDett")
            If (aDataView1 Is Nothing) Then
                aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            End If
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            Session("aDataView1") = aDataView1
            Session("aDsDett") = DsDocumentiDett
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
        End If
        EnableTOTxtInsArticoli(False)
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        GridViewDett.Enabled = False
        '------------------------------------------------------------------------
        GridViewDettL.EditIndex = e.NewEditIndex
        aDataView1L = Session("aDataView1L")
        If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
        GridViewDettL.DataSource = aDataView1L
        GridViewDettL.DataBind()
        ''giu130417 per evitare ilposizionamento sempre alla prima riga GridViewDettL.Focus()
        Session(SWOPDETTDOCL) = SWOPMODIFICA
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub BuildLottiRigaDB(ByVal Riga As Integer)
        Dim Passo As Integer = 1
        Try
            'Simile al LOAD
            SetCdmDAdp()
            Passo = 2
            Session("aSqlAdapL") = SqlAdapDocDettL
            '--
            If (Session("aDsDettL") Is Nothing) Then
                DsDocumentiDettL = New DSDocumenti
            Else
                DsDocumentiDettL = Session("aDsDettL")
            End If
            '--
            Passo = 3
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            Passo = 4
            If Session(SWOP) <> SWOPNUOVO Then 'giu041211
                If myID = "" Or Not IsNumeric(myID) Then
                    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. (BuildLottiRigaDB) Passo: " & Passo.ToString)
                    Exit Sub
                End If
            End If
            Passo = 5
            DsDocumentiDettL.DocumentiDLotti.Clear()
            SqlDbSelectCmdL.Parameters.Item("@IDDocumenti").Value = CLng(IIf(myID.Trim = "", -1, myID.Trim))
            SqlDbSelectCmdL.Parameters.Item("@Riga").Value = Riga
            SqlAdapDocDettL.Fill(DsDocumentiDettL.DocumentiDLotti)
            '---------------------------------------
            Passo = 6
            Session("aSqlAdapL") = SqlAdapDocDettL
            Session("aDsDettL") = DsDocumentiDettL
            '-- LOTTI
            Passo = 7
            aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            GridViewDettL.DataSource = aDataView1L
            Session("aDataView1L") = aDataView1L
            Passo = 8
            GridViewDettL.DataBind()
            Passo = 9
            If txtCodArtIns.Text.Trim <> "" Then
                SetBtnCaricoLottiEnabled(True)
                If GridViewDettL.Rows.Count = 0 Then
                    SetBtnPrimaRigaLEnabled(True)
                Else
                    SetBtnPrimaRigaLEnabled(False)
                End If
            Else
                SetBtnCaricoLottiEnabled(False)
                SetBtnPrimaRigaLEnabled(False)
            End If
            'GIU070515 ERR.: DISABILITO LOTTI SENON SONO IN MODIFICA
            Passo = 10
            If Session(SWOP) = SWOPNESSUNA Then
                SetBtnCaricoLottiEnabled(False)
                SetBtnPrimaRigaLEnabled(False)
                GridViewDettL.Enabled = False
            ElseIf Session(SWOP) = SWOPMODIFICA Then
                If txtCodArtIns.Text.Trim <> "" Then
                    SetBtnCaricoLottiEnabled(True)
                    GridViewDettL.Enabled = True
                Else
                    SetBtnCaricoLottiEnabled(False)
                    SetBtnPrimaRigaLEnabled(False)
                    GridViewDettL.Enabled = False
                End If
            End If
            '-----
        Catch ex As Exception
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.BuildLottiRigaDB. Passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Session(SWOPDETTDOCL) = SWOPNESSUNA
    End Sub

    Private Sub GridViewDett_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridViewDett.RowUpdating
        'giu110112 non allineato per lo sconto VALORE
        btnAggArtGridSel_Click(btnAggArtGridSel, Nothing)

    End Sub
    Private Sub GridViewDettL_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridViewDettL.RowUpdating
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
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

        Try
            Dim myRowIndex As Integer = e.RowIndex + (GridViewDettL.PageSize * GridViewDettL.PageIndex)
            Dim row As GridViewRow = GridViewDettL.Rows(e.RowIndex)
            Dim Comodo As String = CType(row.Cells(CellIdxL.QtaCollo).Controls(0), TextBox).Text
            If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
            Dim myQuantita As Integer = CDbl(Comodo.Trim)
            Dim Comodo1 As String = CType(row.Cells(CellIdxL.Lotto).Controls(0), TextBox).Text
            Dim Comodo2 As String = CType(row.Cells(CellIdxL.NSerie).Controls(0), TextBox).Text
            Dim myErrore As String = ""
            If myQuantita = 0 Then
                myErrore += "Quantità obbligatoria"
            End If
            If Comodo1.Trim = "" And Comodo2.Trim = "" Then
                myErrore += " <br> Lotto/N° di serie obbligatori"
            End If
            '-
            If myErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'Fabio18042016 - Controllo che non si inseriscano seriali identici
            DsDocumentiDettL = Session("aDsDettL")
            If Comodo1.Trim <> "" Then
                If DsDocumentiDettL.DocumentiDLotti.Select("Lotto ='" + Controlla_Apice(Comodo1.Trim) + "' AND NCollo<>" & GridViewDettL.SelectedDataKey.Value.ToString.Trim).Length > 0 Then 'GIU120417 
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è consentito inserire lo stesso lotto più volte!</BR>Operazione interrotta.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
            If Comodo2.Trim <> "" Then
                If DsDocumentiDettL.DocumentiDLotti.Select("NSerie ='" + Controlla_Apice(Comodo2.Trim) + "' AND NCollo<>" & GridViewDettL.SelectedDataKey.Value.ToString.Trim).Length > 0 Then 'GIU120417 
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non è consentito inserire lo stesso N° di serie più volte!</BR>Operazione interrotta.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
            'End Fabio

            aDataView1L = Session("aDataView1L")
            aDataView1L.Item(myRowIndex).Item("IDDocumenti") = CLng(myID)
            aDataView1L.Item(myRowIndex).Item("Riga") = GridViewDett.SelectedDataKey.Value
            aDataView1L.Item(myRowIndex).Item("Cod_Articolo") = txtCodArtIns.Text.Trim
            aDataView1L.Item(myRowIndex).Item("Lotto") = CType(row.Cells(CellIdxL.Lotto).Controls(0), TextBox).Text
            aDataView1L.Item(myRowIndex).Item("NSerie") = CType(row.Cells(CellIdxL.NSerie).Controls(0), TextBox).Text
            aDataView1L.Item(myRowIndex).Item("QtaColli") = myQuantita
            aDataView1L.Item(myRowIndex).Item("Sfusi") = 0

            SqlAdapDocDettL = Session("aSqlAdapL")
            DsDocumentiDettL = Session("aDsDettL")

            Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
            If (aDataView1L Is Nothing) Then
                aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            End If
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            Session("aDataView1L") = aDataView1L
            Session("aDsDettL") = DsDocumentiDettL
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.EditIndex = -1
            GridViewDettL.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore DocumentiDett.GridViewDettL_RowUpdating", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '---
        'Abilito i DETTAGLI 
        GridViewDett.Enabled = True
        '--------------------------------------------------------------
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub

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

        SqlAdapDocDett = Session("aSqlAdap")
        DsDocumentiDett = Session("aDsDett")
        Try
            Dim RigaSel As Integer = GridViewDett.SelectedDataKey.Value
            Dim GrdSel As Integer = GridViewDett.SelectedIndex
            Dim RowsD() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga >=" & RigaSel, "Riga DESC")
            Dim RowD As DSDocumenti.DocumentiDRow
            'giu030117
            ' ''GIU210615
            ''Dim SaveRiga As Integer
            ' ''Dim RowDRA As DSDocumenti.DocumentiDRow
            ' ''Dim RowsDRA() As DataRow
            '---------
            For Each RowD In RowsD
                If RowD.Riga <> RigaSel Then
                    'giu030117 SaveRiga = RowD.Riga 'giu210615
                    If RinumeraLottiRiga(RowD.Riga, RowD.Riga + 1) = False Then 'giu170112
                        Exit Sub
                    End If
                    RowD.BeginEdit()
                    RowD.Riga += 1
                    RowD.EndEdit()
                    'giu030117
                    '' ''GIU210615
                    ' ''RowsDRA = Me.DsDocumentiDett.DocumentiD.Select("Riga_Appartenenza=" & SaveRiga.ToString.Trim)
                    ' ''For Each RowDRA In RowsDRA
                    ' ''    RowDRA.BeginEdit()
                    ' ''    RowDRA.Riga_Appartenenza = SaveRiga + 1
                    ' ''    RowDRA.EndEdit()
                    ' ''Next
                    '' ''---------
                End If
            Next

            Dim newRowDocD As DSDocumenti.DocumentiDRow = DsDocumentiDett.DocumentiD.NewDocumentiDRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
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
                '---------
                .EndEdit()
            End With
            DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
            newRowDocD = Nothing

            aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
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
            'RICARICO I LOTTI PER LA RIGA SELEZIONATA
            BuildLottiRigaDB(RigaSel)
            '-----------
            'GIU230512
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = True Then
                'GIU170112 giu140512 i lotti solo nei documenti di trasporto
                If myTipoDoc = SWTD(TD.Preventivi) Or _
                   myTipoDoc = SWTD(TD.OrdClienti) Or _
                   myTipoDoc = SWTD(TD.OrdDepositi) Or _
                   myTipoDoc = SWTD(TD.OrdFornitori) Or _
                   myTipoDoc = SWTD(TD.PropOrdFornitori) Then
                    PanelDettArtLottiNrSerie.Enabled = False
                    ' ''PanelDettArtLottiNrSerie.Visible = False
                    PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
                    lblMessAgg.Text = "ATTENZIONE, per inserire Lotti/N°Serie creare il DDT"
                    lblMessAgg.BorderStyle = BorderStyle.Outset
                End If
            End If
            '------------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GrdiViewDett_SelectedChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsDocumentiDett
        'ASPETTO CHE MI SCEGLIE UN ARTICOLO
        SetBtnPrimaRigaLEnabled(False)
        SetBtnCaricoLottiEnabled(False)
        '----------------------------------        
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA

        Session(SWMODIFICATO) = SWSI
        'giu160715
        Call GridViewDett_RowEditing(sender, Nothing)
    End Sub
    Private Sub btnNewRigaUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNewRigaUp.Click
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

        SqlAdapDocDett = Session("aSqlAdap")
        DsDocumentiDett = Session("aDsDett")
        Try
            Dim RigaSel As Integer = GridViewDett.SelectedDataKey.Value
            Dim GrdSel As Integer = GridViewDett.SelectedIndex
            Dim RowsD() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga >=" & RigaSel, "Riga DESC")
            Dim RowD As DSDocumenti.DocumentiDRow
            'giu030117
            ' ''GIU210615
            ''Dim SaveRiga As Integer
            ' ''Dim RowDRA As DSDocumenti.DocumentiDRow
            ' ''Dim RowsDRA() As DataRow
            '---------
            For Each RowD In RowsD
                If RinumeraLottiRiga(RowD.Riga, RowD.Riga + 1) = False Then 'giu170112
                    Exit Sub
                End If
                RowD.BeginEdit()
                RowD.Riga += 1
                RowD.EndEdit()
            Next

            Dim newRowDocD As DSDocumenti.DocumentiDRow = DsDocumentiDett.DocumentiD.NewDocumentiDRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
                .Riga = RigaSel
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
                '---------
                .EndEdit()
            End With
            DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
            newRowDocD = Nothing

            aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            'giu230115 se la nuova riga è nella pagina successiva sposto anche indexpage
            If (GrdSel + 2) > (GridViewDett.PageSize * (GridViewDett.PageIndex + 1)) Then
                GridViewDett.PageSize += 1
            End If
            '-----------------
            GridViewDett.DataBind()
            GridViewDett.SelectedIndex = GrdSel

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
            'RICARICO I LOTTI PER LA RIGA SELEZIONATA
            BuildLottiRigaDB(RigaSel)
            '-----------
            'GIU230512
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = True Then
                'GIU170112 giu140512 i lotti solo nei documenti di trasporto
                If myTipoDoc = SWTD(TD.Preventivi) Or _
                   myTipoDoc = SWTD(TD.OrdClienti) Or _
                   myTipoDoc = SWTD(TD.OrdDepositi) Or _
                   myTipoDoc = SWTD(TD.OrdFornitori) Or _
                   myTipoDoc = SWTD(TD.PropOrdFornitori) Then
                    PanelDettArtLottiNrSerie.Enabled = False
                    ' ''PanelDettArtLottiNrSerie.Visible = False
                    PanelDettArtLottiNrSerie.HeaderText = "Per inserire Lotti/N°Serie creare il DDT"
                    lblMessAgg.Text = "ATTENZIONE, per inserire Lotti/N°Serie creare il DDT"
                    lblMessAgg.BorderStyle = BorderStyle.Outset
                End If
            End If
            '------------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GrdiViewDett_SelectedChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsDocumentiDett
        'ASPETTO CHE MI SCEGLIE UN ARTICOLO
        SetBtnPrimaRigaLEnabled(False)
        SetBtnCaricoLottiEnabled(False)
        '----------------------------------        
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA

        Session(SWMODIFICATO) = SWSI
        'giu160715
        Call GridViewDett_RowEditing(sender, Nothing)
    End Sub
    Private Sub GridViewDettL_NewRigaSotto(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDettL.SelectedIndexChanged
        GridViewDettL.EditIndex = -1
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
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If

        SqlAdapDocDettL = Session("aSqlAdapL")
        DsDocumentiDettL = Session("aDsDettL")
        Try
            Dim RigaSel As Integer = GridViewDettL.SelectedDataKey.Value
            Dim GrdSel As Integer = GridViewDettL.SelectedIndex
            Dim RowsD() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("NCollo >=" & RigaSel, "NCollo DESC")
            Dim RowD As DSDocumenti.DocumentiDLottiRow
            For Each RowD In RowsD
                If RowD.NCollo <> RigaSel Then
                    RowD.BeginEdit()
                    RowD.NCollo += 1
                    RowD.EndEdit()
                    Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)
                End If
            Next
            'giu071211 totale qta fino adesso inserito
            Dim RowsDL() As DataRow = Me.DsDocumentiDettL.DocumentiDLotti.Select("")
            Dim RowDL As DSDocumenti.DocumentiDLottiRow
            Dim QtaColli As Integer = 0
            For Each RowDL In RowsDL
                QtaColli += RowDL.QtaColli
            Next
            'giu071211 totale qta fino adesso inserito
            'giu301111
            '--- LOTTI PRENDO I DATI DALL'ARTICOLO DA MEMORIZZARE
            Dim RigaDettArt As Integer = GridViewDett.SelectedDataKey.Value
            Dim Comodo As String = ""
            Dim QtaRich As Integer = 0
            Dim QtaEvasa As Integer = 0 'giu291211 correzione se la qta' è l'evasa o ordinata
            Try
                Dim row As GridViewRow = GridViewDett.SelectedRow
                Comodo = IIf(row.Cells(CellIdx.Qta).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.Qta).Text.Trim)
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                QtaRich = Comodo.Trim
            Catch ex As Exception
                QtaRich = 0
            End Try
            Try 'giu291211 correzione se la qta' è l'evasa o ordinata
                Dim row As GridViewRow = GridViewDett.SelectedRow
                Comodo = IIf(row.Cells(CellIdx.QtaEv).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.QtaEv).Text.Trim)
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                QtaEvasa = Comodo.Trim
            Catch ex As Exception
                QtaEvasa = 0
            End Try
            'giu291211 correzione se la qta' è l'evasa o ordinata
            ' ''If QtaColli > QtaRich Or QtaColli = QtaRich Then
            ' ''    QtaColli = 0
            ' ''ElseIf QtaRich > QtaColli Then
            ' ''    QtaColli = QtaRich - QtaColli
            ' ''End If
            'giu291211 correzione se la qta' è l'evasa o ordinata
            Select Case myTipoDoc
                Case "PR", "PF" 'GIU020212
                    If QtaColli > QtaRich Or QtaColli = QtaRich Then
                        QtaColli = 0
                    ElseIf QtaRich > QtaColli Then
                        QtaColli = QtaRich - QtaColli
                    End If
                Case Else
                    If QtaColli > QtaEvasa Or QtaColli = QtaEvasa Then
                        QtaColli = 0
                    ElseIf QtaEvasa > QtaColli Then
                        QtaColli = QtaEvasa - QtaColli
                    End If
            End Select
            '----------------------------------------------------
            'giu071211 totale qta fino adesso inserito end
            '------------------------------------------------------------------------
            'GIU301111 disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
            '------------------------------------------------------------------------
            If GridViewDett.EditIndex <> -1 Then
                SqlAdapDocDett = Session("aSqlAdap")
                DsDocumentiDett = Session("aDsDett")
                If (aDataView1 Is Nothing) Then
                    aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
                End If
                If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
                Session("aDataView1") = aDataView1
                Session("aDsDett") = DsDocumentiDett
                GridViewDett.DataSource = aDataView1
                GridViewDett.EditIndex = -1
                GridViewDett.DataBind()
            End If
            EnableTOTxtInsArticoli(False)
            'GIU030212
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            GridViewDett.Enabled = False
            '------------------------------------------------------------------------
            '---------
            Dim newRowDocD As DSDocumenti.DocumentiDLottiRow = DsDocumentiDettL.DocumentiDLotti.NewDocumentiDLottiRow
            With newRowDocD
                .BeginEdit()
                .IDDocumenti = CLng(myID)
                .Cod_Articolo = txtCodArtIns.Text.Trim
                .Riga = RigaDettArt
                .NCollo = RigaSel + 1
                .Lotto = ""
                .NSerie = ""
                .Sfusi = 0
                .QtaColli = QtaColli
                .EndEdit()
            End With
            DsDocumentiDettL.DocumentiDLotti.AddDocumentiDLottiRow(newRowDocD)
            newRowDocD = Nothing

            Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)

            aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            GridViewDettL.SelectedIndex = GrdSel + 1
            GridViewDettL.EditIndex = GrdSel + 1
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.DataBind()
            GridViewDettL.Focus()
        Catch Ex As Exception
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GridViewDettL.SelectedIndexChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Session(SWOPDETTDOCL) = SWOPMODIFICA
        Session(SWMODIFICATO) = SWSI
    End Sub

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
        Dim newRowDocD As DSDocumenti.DocumentiDRow = DsDocumentiDett.DocumentiD.NewDocumentiDRow
        With newRowDocD
            .BeginEdit()
            .IDDocumenti = CLng(myID)
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
            DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
            newRowDocD = Nothing

            SqlAdapDocDett = Session("aSqlAdap")

            aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            GridViewDett.DataSource = aDataView1
            GridViewDett.DataBind()
            GridViewDett.SelectedIndex = 0
            'Selezionato
            '--041211
            AzzeraTxtInsArticoli()
            'RICARICO I LOTTI PER LA RIGA SELEZIONATA
            BuildLottiRigaDB(GridViewDett.SelectedDataKey.Value)
            '-----------
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore DocumentiDett.btnPrimaRiga", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsDocumentiDett

        SetBtnPrimaRigaEnabled(False)
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub
    Private Sub btnPrimaRigaL_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrimaRigaL.Click
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

        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        '------------------------------------------------------------------------
        'GIU301111 disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
        '------------------------------------------------------------------------
        If GridViewDett.EditIndex <> -1 Then
            SqlAdapDocDett = Session("aSqlAdap")
            DsDocumentiDett = Session("aDsDett")
            If (aDataView1 Is Nothing) Then
                aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
            End If
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            Session("aDataView1") = aDataView1
            Session("aDsDett") = DsDocumentiDett
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
        End If
        EnableTOTxtInsArticoli(False)
        'GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        GridViewDett.Enabled = False
        '------------------------------------------------------------------------
        'giu301111 'giu291211 correzione se la qta' è l'evasa o ordinata
        '--- LOTTI
        Dim Comodo As String = ""
        Dim QtaRich As Integer = 0
        Dim QtaEvasa As Integer = 0 'giu291211 correzione se la qta' è l'evasa o ordinata
        Try
            Dim row As GridViewRow = GridViewDett.SelectedRow
            Comodo = IIf(row.Cells(CellIdx.Qta).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.Qta).Text.Trim)
            If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
            QtaRich = Comodo.Trim
        Catch ex As Exception
            QtaRich = 0
        End Try
        Try
            Dim row As GridViewRow = GridViewDett.SelectedRow
            Comodo = IIf(row.Cells(CellIdx.QtaEv).Text.Trim = HTML_SPAZIO, 0, row.Cells(CellIdx.QtaEv).Text.Trim)
            If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
            QtaEvasa = Comodo.Trim
        Catch ex As Exception
            QtaEvasa = 0
        End Try
        Dim QtaSel As Integer = 0
        Select Case Left(myTipoDoc, 1)
            Case "O"
                QtaSel = QtaRich
            Case Else
                If myTipoDoc = "PR" Or myTipoDoc = "PF" Then 'GIU020212
                    QtaSel = QtaRich
                Else
                    QtaSel = QtaEvasa
                End If
        End Select
        '---------
        Dim newRowDocD As DSDocumenti.DocumentiDLottiRow = DsDocumentiDettL.DocumentiDLotti.NewDocumentiDLottiRow
        With newRowDocD
            .BeginEdit()
            .IDDocumenti = CLng(myID)
            .Cod_Articolo = txtCodArtIns.Text.Trim
            .Riga = GridViewDett.SelectedDataKey.Value
            .NCollo = 1
            .Lotto = ""
            .NSerie = ""
            .QtaColli = QtaSel
            .Sfusi = 0
            .EndEdit()
        End With

        Try
            DsDocumentiDettL.DocumentiDLotti.AddDocumentiDLottiRow(newRowDocD)
            newRowDocD = Nothing

            SqlAdapDocDettL = Session("aSqlAdapL")
            Me.SqlAdapDocDettL.Update(DsDocumentiDettL.DocumentiDLotti)

            aDataView1L = New DataView(DsDocumentiDettL.DocumentiDLotti)
            'MESSO SOPRA ' ''disabilito i DETTAGLI poi li abilito dopo l'aggiorna o annulla
            ' ''GridViewDett.Enabled = False
            ' ''btnAggArtGridSel.Enabled = False : btnInsRigaDett.Enabled = False
            ' ''
            '' ''--------------------------------------------------------------
            GridViewDettL.EditIndex = GridViewDettL.SelectedIndex
            If aDataView1L.Count > 0 Then aDataView1L.Sort = "NCollo"
            GridViewDettL.DataSource = aDataView1L
            GridViewDettL.DataBind()
            GridViewDettL.SelectedIndex = 0
            GridViewDettL.EditIndex = GridViewDettL.SelectedIndex
        Catch Ex As Exception
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.btnPrimaRigaL", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Session("aDataView1L") = aDataView1L
        Session("aSqlAdapL") = SqlAdapDocDettL
        Session("aDsDettL") = DsDocumentiDettL

        SetBtnPrimaRigaLEnabled(False)
        SetBtnCaricoLottiEnabled(True)
        Session(SWOPDETTDOCL) = SWOPMODIFICA
        Session(SWMODIFICATO) = SWSI
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
        Try
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
        DsDocumentiDett = Session("aDsDett")
        '-------------------------
        'GIU110117
        Try
            Dim RowsDel() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga>" & RigaSel.ToString.Trim, "Riga")
            Dim RowDel As DSDocumenti.DocumentiDRow
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
            Dim RowsD() As DataRow = Me.DsDocumentiDett.DocumentiD.Select("Riga >=" & RigaSel, "Riga DESC")
            Dim RowD As DSDocumenti.DocumentiDRow
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
            Dim newRowDocD As DSDocumenti.DocumentiDRow = DsDocumentiDett.DocumentiD.NewDocumentiDRow
            For Each CodArt As String In listaCodiciArtSel
                'giu301111 per evitare di inserire articoli gia' presenti nel documento
                ' GESTITO DIRETTAMENTE IN SELEZIONA ARTICOLI E NON QUI, mi arrivato i dati effettivi
                ' ''If Me.DsDocumentiDett.DocumentiD.Select("Cod_Articolo = '" & CodArt.Trim & "'").Count = 0 Then
                '----------------------------------------------------------------------
                newRowDocD = DsDocumentiDett.DocumentiD.NewDocumentiDRow
                With newRowDocD
                    .BeginEdit()
                    .IDDocumenti = CLng(myID)
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
                    .EndEdit()
                End With
                iNew += 1
                DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
                newRowDocD = Nothing
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
                                newRowDocD = DsDocumentiDett.DocumentiD.NewDocumentiDRow
                                With newRowDocD
                                    .BeginEdit()
                                    .IDDocumenti = CLng(myID)
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
                                    .EndEdit()
                                End With
                                DsDocumentiDett.DocumentiD.AddDocumentiDRow(newRowDocD)
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
                    ModalPopup.Show("Errore in DocumentiDett.CallBackWFPArticoloSel", "Lettura Descrizione estesa: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit For
                End Try
                '------------------------------------
                ' GESTITO DIRETTAMENTE IN SELEZIONA ARTICOLI E NON QUI, mi arrivato i dati effettivi
                ' ''End If 'giu301111 per evitare di inserire articoli gia' presenti nel documento
                '----------------------------------------------------------------------
            Next
            RowsD = Nothing
            'RINUMERO TUTTO
            'GIU210615
            Dim RowDRA As DSDocumenti.DocumentiDRow
            Dim RowsDRA() As DataRow
            '---------
            Dim RowsR = Me.DsDocumentiDett.DocumentiD.Select("", "Riga")
            iNew = 1
            For Each RowD In RowsR
                If RowD.Riga <> iNew Then
                    'giu210615
                    SaveRiga = RowD.Riga
                    '---------
                    RowD.BeginEdit()
                    RowD.Riga = iNew
                    RowD.EndEdit()
                    'GIU210615
                    RowsDRA = Me.DsDocumentiDett.DocumentiD.Select("Riga_Appartenenza=" & SaveRiga.ToString.Trim)
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
            DsDocumentiDett = Session("aDsDett")
            GridViewDett.Enabled = True
            '---------------
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.CallBackWFPArticoloSel", "Inserimento articoli selezionati: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        'AGGIORNO IL GRID E MI SELEZIONE NUOVAMENTE LA RIGA DA CUI HO INSERITO
        aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
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
            Call AzzeraTxtInsArticoli() 'giu021211
        End Try
        'AGGIORNO DS NELLA SESSIONE
        Dim strErrore As String = ""
        If AggiornaImporto(DsDocumentiDett, strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
        Session("aDataView1") = aDataView1
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsDocumentiDett
        '-
        'Riattivo il GRID
        GridViewDett.Enabled = True
        'ASPETTO CHE MI SELEZIONE L'ARTICOLO
        SetBtnCaricoLottiEnabled(False)
        SetBtnPrimaRigaLEnabled(False)

        Session(SWMODIFICATO) = SWSI

        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
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
            ModalPopup.Show("Errore in DocumentiDett.GetDatiAnaMag.CKPrezzoALCSG", strErrore, WUC_ModalPopup.TYPE_ALERT)
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
                            If myTipoDoc = "PR" Or myTipoDoc = "PF" Then 'GIU020212
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
            ModalPopup.Show("Errore in DocumentiDett.GetDatiAnaMag", "Lettura articoli: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
            ModalPopup.Show("Errore in DocumentiDett.GetDatiAnaMag", strErrore, WUC_ModalPopup.TYPE_ERROR)
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
            RowD.Item("Prezzo") = _myPrezzoListino
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
        ChkDedPerAcc.Checked = False
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
        Passo = 0
        If txtCodArtIns.BackColor = SEGNALA_KO Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice articolo non valido, impossibile l'aggiornamento riga.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu051211 spostato qui dal try
        Passo = 1
        Dim mySCGiacenza As Boolean = False
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor, mySCGiacenza) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        'GIU170412
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
        Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0 : Dim myQtaA As Decimal = 0
        Dim myQuantita As Decimal = 0 : Dim SWQta = ""
        Dim myIVA As Integer = 0
        Dim myPrezzo As Decimal = 0
        Passo = 4
        '--------------------------------------------------------------------------------
        If Not IsNumeric(txtQtaIns.Text.Trim) Then txtQtaIns.Text = "0"
        If Not IsNumeric(txtQtaEv.Text.Trim) Then txtQtaEv.Text = "0"
        If Not IsNumeric(txtQtaInv.Text.Trim) Then txtQtaInv.Text = "0"
        txtIVAIns.AutoPostBack = False
        If Not IsNumeric(txtIVAIns.Text.Trim) Then txtIVAIns.Text = "0"
        txtIVAIns.AutoPostBack = True
        If Not IsNumeric(txtPrezzoIns.Text.Trim) Then txtPrezzoIns.Text = "0"
        myQtaO = CDec(txtQtaIns.Text.Trim)
        myQtaE = CDec(txtQtaEv.Text.Trim)
        'GIU290419
        ' ''Try
        ' ''    myQtaA = aDataView1.Item(myRowIndex).Item("Qta_Allestita")
        ' ''Catch ex As Exception
        ' ''    myQtaA = 0
        ' ''End Try
        myQtaA = CDec(txtQtaInv.Text.Trim)
        '---------
        'GIU090312 LA QTA ALLESTITA NON SERVE NEI CM E NE TANTOMENO NEI OF
        If myTipoDoc = "MM" Or myTipoDoc = "CM" Or myTipoDoc = "SM" Then
            myQtaA = 0
        End If
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
        If (myQtaE + myQtaA) > myQtaO Then
            myQtaR = 0
        Else
            myQtaR = (myQtaE + myQtaA) - myQtaO
            If myQtaR < 0 Then myQtaR = myQtaR * -1
        End If

        LblQtaRe.Text = FormattaNumero(myQtaR, -1)
        Select Case Left(myTipoDoc, 1)
            Case "O"
                myQuantita = myQtaO
                SWQta = "O"
            Case Else
                If myTipoDoc = "PR" Or myTipoDoc = "PF" Then 'GIU020212
                    myQuantita = myQtaO
                    SWQta = "O"
                Else
                    myQuantita = myQtaE
                    SWQta = "E"
                End If
        End Select
        'giu071211 richiesta di Cinzia 
        Dim myErrore As String = ""
        If txtCodArtIns.Text.Trim <> "" Or txtPrezzoIns.Text.Trim <> 0 Then
            If myIVA = 0 Then
                If myQuantita = 0 Then
                    If SWQta = "O" Then myErrore += " Quantità ordinata obbligatoria"
                    If SWQta = "E" Then myErrore += " Quantità evasa a ZERO"
                End If
                If myIVA = 0 Then myErrore += " IVA obbligatoria"
                ' ''If myPrezzo = 0 Then myErrore += " Prezzo obbligatorio"
                If myPrezzo = 0 Then
                    lblMessAgg.Text = "Attenzione, Prezzo a ZERO"
                    lblMessAgg.BorderStyle = BorderStyle.Outset
                End If
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If myPrezzo = 0 And myQuantita <> 0 Then
            If myIVA = 0 Then myErrore += " IVA obbligatoria"
            ' ''If myPrezzo = 0 Then myErrore += " Prezzo obbligatorio"
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
            ' ''Exit Sub
            If myPrezzo = 0 Then
                lblMessAgg.Text = "Attenzione, Prezzo a ZERO"
                lblMessAgg.BorderStyle = BorderStyle.Outset
            End If
        End If
        'Richiesta di Cinzia (Tel. 291211) la qta' evasa mai superiore alla richiesta
        If myQtaE > myQtaO Then
            myErrore += " La quantità Evasa/Allestita non può essere superiore alla quantità Ordinata"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu010112 giu250112 
        If myTipoDoc = "OC" And myQtaA > 0 Then
            If (myQtaE + myQtaA) > myQtaO Then
                myErrore += " La quantità Allestita non può essere superiore alla quantità Ordinata+Inviata"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        ElseIf Left(myTipoDoc, 1) = "D" Or Left(myTipoDoc, 1) = "F" Or Left(myTipoDoc, 1) = "N" Then 'giu290419
            If (myQtaE + myQtaA) > myQtaO Then
                myErrore += " La quantità Evasa+Inviata non può essere superiore alla quantità Ordinata"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If myQtaA > myQtaO Then
            myErrore += " La quantità Allestita/Inviata non può essere superiore alla quantità Ordinata"
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
            WucElement.GetDatiTB3()

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
            Passo = 9
            Try
                Dim row As GridViewRow = GridViewDett.Rows(i)
                aDataView1 = Session("aDataView1")
                'Selezionato

                'AL MOMENTO SONO TUTTI UGUALI POI SI DIFFERENZIANO
                ' ''If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then 
                aDataView1.Item(myRowIndex).Item("IDDocumenti") = CLng(myID)
                '---
                aDataView1.Item(myRowIndex).Item("Cod_Articolo") = txtCodArtIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.CodArt).Controls(0), TextBox).Text

                aDataView1.Item(myRowIndex).Item("Descrizione") = txtDesArtIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.DesArt).Controls(0), TextBox).Text

                aDataView1.Item(myRowIndex).Item("Um") = txtUMIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.UM).Controls(0), TextBox).Text

                Comodo = txtQtaIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.Qta).Controls(0), TextBox).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Ordinata") = CDec(Comodo.Trim)

                Comodo = txtQtaEv.Text.Trim 'giu051211 CType(row.Cells(CellIdx.QtaEv).Controls(0), TextBox).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Evasa") = CDec(Comodo.Trim)

                Comodo = LblQtaRe.Text.Trim 'giu051211 CType(row.Cells(CellIdx.QtaRe).Controls(0), Label).Text
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Residua") = CDec(Comodo.Trim)
                'GIU290419 QTA' INVIATA
                Comodo = txtQtaInv.Text.Trim
                If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                aDataView1.Item(myRowIndex).Item("Qta_Allestita") = CDec(Comodo.Trim)
                '---------
                'giu051211 non gestito per IREDEEM
                ' ''Comodo = CType(row.Cells(CellIdx.Conf).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                ' ''aDataView1.Item(myRowIndex).Item("Confezione") = CDec(Comodo.Trim)

                'GIU150715 VALORIZZATO PRIMA 
                ' ''Comodo = txtIVAIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.IVA).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                ' ''aDataView1.Item(myRowIndex).Item("Cod_Iva") = CDbl(Comodo.Trim)
                ' ''myIVA = CDbl(Comodo.Trim)
                aDataView1.Item(myRowIndex).Item("Cod_Iva") = myIVA
                '---------------------------
                'giu150715 VALORIZZATO PRIMA 
                ' ''Comodo = txtPrezzoIns.Text.Trim 'giu051211 CType(row.Cells(CellIdx.Prz).Controls(0), TextBox).Text
                ' ''If Not IsNumeric(Comodo.Trim) Then Comodo = "0"
                ' ''myPrezzo = CDec(Comodo.Trim)
                'giu160112 checkNoScontoValore
                If checkNoScontoValore.Checked = False Then
                    aDataView1.Item(myRowIndex).Item("Prezzo") = IIf(myPrezzoAL = 0, myPrezzo, myPrezzoAL) 'giu190412 myPrezzoAL
                    If aDataView1.Item(myRowIndex).Item("Prezzo") < 0 Then 'giu260419
                        aDataView1.Item(myRowIndex).Item("Prezzo") = 0
                    End If
                Else
                    aDataView1.Item(myRowIndex).Item("Prezzo") = myPrezzo 'giu190412 CDec(Comodo.Trim)
                End If
                '----------------------------
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
                aDataView1.Item(myRowIndex).Item("Note") = "" 'CType(row.Cells(CellIdx.Note).Controls(0), TextBox).Text
                '--- differenziare in base al tipo di documento sulla qta se evasa etc etc
                'giu101111 Dim PrezzoNetto As Decimal = 0

                'giu051211 dichiarati sopra
                ' ''Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0
                ' ''Dim myQuantita As Decimal = 0 : Dim SWQta = ""
                myQtaO = 0 : myQtaE = 0 : myQtaR = 0
                myQuantita = 0 : SWQta = ""
                '--------------------------
                myQtaO = aDataView1.Item(myRowIndex).Item("Qta_Ordinata")
                myQtaE = aDataView1.Item(myRowIndex).Item("Qta_Evasa")
                Try
                    myQtaA = aDataView1.Item(myRowIndex).Item("Qta_Allestita")
                Catch ex As Exception
                    myQtaA = 0
                End Try
                'GIU090312 LA QTA ALLESTITA NON SERVE NEI CM E NE TANTOMENO NEI OF
                If myTipoDoc = "MM" Or myTipoDoc = "CM" Or myTipoDoc = "SM" Then
                    myQtaA = 0
                End If
                '-----------------------------------------------------------------
                If (myQtaE + myQtaA) > myQtaO Then
                    myQtaR = 0
                Else
                    myQtaR = (myQtaE + myQtaA) - myQtaO
                    If myQtaR < 0 Then myQtaR = myQtaR * -1
                End If
                aDataView1.Item(myRowIndex).Item("Qta_Residua") = myQtaR
                Select Case Left(myTipoDoc, 1)
                    Case "O"
                        myQuantita = aDataView1.Item(myRowIndex).Item("Qta_Ordinata")
                        SWQta = "O"
                    Case Else
                        If myTipoDoc = "PR" Or myTipoDoc = "PF" Then 'GIU020212
                            myQuantita = aDataView1.Item(myRowIndex).Item("Qta_Ordinata")
                            SWQta = "O"
                        Else
                            myQuantita = aDataView1.Item(myRowIndex).Item("Qta_Evasa")
                            SWQta = "E"
                        End If
                End Select
                'giu051211 dichiarati sopra 
                ' ''Dim myErrore As String = ""
                myErrore = ""
                '--------------------------
                If myCodArt.Trim <> "" Or myPrezzo <> 0 Then
                    If myIVA = 0 Then 'GIU150320 'giu071211 myQuantita = 0 Or 
                        If myQuantita = 0 Then
                            If SWQta = "O" Then myErrore += "Quantità ordinata obbligatoria"
                            If SWQta = "E" Then myErrore += "Quantità evasa a ZERO (Proseguo)"
                        End If
                        If myIVA = 0 Then myErrore += " IVA obbligatoria"
                        If myPrezzo = 0 Then myErrore += " Prezzo obbligatorio"
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
                If myPrezzo = 0 And myQuantita <> 0 Then
                    If myIVA = 0 Then myErrore += " IVA obbligatoria"
                    ' ''If myPrezzo = 0 Then myErrore += " Prezzo obbligatorio"
                    If myPrezzo = 0 Then
                        lblMessAgg.BorderStyle = BorderStyle.Outset
                        lblMessAgg.Text = "Attenzione, Prezzo a ZERO - Prezzo base: " & FormattaNumero(myPrezzoAL.ToString, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                    End If
                    'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ' ''ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                    ' ''Exit Sub
                End If
                'Richiesta di Cinzia (Tel. 291211) la qta' evasa mai superiore alla richiesta
                If myQtaE > myQtaO Then
                    myErrore += " La quantità Evasa/Allestita non può essere superiore alla quantità Ordinata"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                'giu010112 giu250112 
                If myTipoDoc = "OC" And myQtaA > 0 Then
                    If (myQtaE + myQtaA) > myQtaO Then
                        myErrore += " La quantità Allestita non può essere superiore alla quantità Ordinata+Inviata"
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
                If myQtaA > myQtaO Then
                    myErrore += " La quantità Allestita/Inviata non può essere superiore alla quantità Ordinata"
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
                Passo = 10
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
                aDataView1.Item(myRowIndex).Item("DedPerAcconto") = ChkDedPerAcc.Checked
                If ChkDedPerAcc.Checked = True And (myQuantita = 0 Or myPrezzo = 0) Then
                    If myQuantita = 0 Then
                        myErrore += " Deduzione non applicabile: Quantità non può essere a ZERO"
                    End If
                    If myPrezzo = 0 Then
                        myErrore += " Deduzione non applicabile: Prezzo non può essere a ZERO"
                    End If
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                ElseIf ChkDedPerAcc.Checked = True Then
                    If checkNoScontoValore.Checked = False Then
                        checkNoScontoValore.Checked = True
                        myErrore += " Deduzione non applicabile: Sconto valore non applicabile"
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                    If aDataView1.Item(myRowIndex).Item("Sconto_1") <> 0 Or aDataView1.Item(myRowIndex).Item("Sconto_2") <> 0 Or _
                       aDataView1.Item(myRowIndex).Item("Sconto_3") <> 0 Or aDataView1.Item(myRowIndex).Item("Sconto_4") <> 0 Or _
                       aDataView1.Item(myRowIndex).Item("Sconto_Pag") <> 0 Or aDataView1.Item(myRowIndex).Item("ScontoValore") <> 0 Then
                        myErrore += " Deduzione non applicabile: Sconti % non applicabili"
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", myErrore, WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
                '---------
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
                Passo = 11
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
                aDataView1.Item(myRowIndex).Item("PrezzoAcquisto") = _myPrezzoAcquisto
                aDataView1.Item(myRowIndex).Item("PrezzoListino") = _myPrezzoListino
                'GIU230412 
                If Not IsNumeric(txtPrezzoCosto.Text) Then txtPrezzoCosto.Text = "0"
                txtPrezzoCosto.Text = FormattaNumero(txtPrezzoCosto.Text, CInt(DecimaliVal))
                aDataView1.Item(myRowIndex).Item("PrezzoCosto") = CDbl(txtPrezzoCosto.Text)
                '---------
                SqlAdapDocDett = Session("aSqlAdap")
                DsDocumentiDett = Session("aDsDett")

                If (aDataView1 Is Nothing) Then
                    aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
                End If
                If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
                Session("aDataView1") = aDataView1
                Session("aDsDett") = DsDocumentiDett
                GridViewDett.DataSource = aDataView1
                GridViewDett.EditIndex = -1
                GridViewDett.DataBind()
                SWPrezzoCosto = txtPrezzoCosto.Enabled 'giu230412
                EnableTOTxtInsArticoli(False)
                'se venisse modificato UN DETT rifare l'UPDATE COME SOPRA QUANDO RIENTRA
                Passo = 12
                strErroreAggRiga = ""
                If AggiornaImporto(DsDocumentiDett, strErroreAggRiga) = False Then
                    'giu300312 spostato sotto
                End If
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in DocumentiDett.btnAggArtGridSel. Passo: " & Passo.ToString, Ex.Message, WUC_ModalPopup.TYPE_ERROR)
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
            ModalPopup.Show("Errore in DocumentiDett.btnAggArtGridSel. Passo: " & Passo.ToString, ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        If strErroreAggRiga.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.AggiornaImporto", strErroreAggRiga, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If strErroreAggAgente.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.Calcola_ProvvAgente", strErroreAggAgente, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu140412 
        If myErrGetPrezziLA.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in DocumentiDett.GetPrezziListinoAcquisto", myErrGetPrezziLA, WUC_ModalPopup.TYPE_ERROR)
            ' ''Exit Sub
        End If
        'giu230412
        If SWPrezzoCosto = True Then
            txtPrezzoCosto.Enabled = False
            If Not IsNumeric(txtPrezzoCosto.Text) Then txtPrezzoCosto.Text = "0"
            If CDbl(txtPrezzoCosto.Text) = 0 And ChkDedPerAcc.Checked = False Then 'giu020519
                txtPrezzoCosto.BackColor = SEGNALA_KO
                ' ''ModalPopup.Show("Attenzione, Costo unitario ZERO", "", WUC_ModalPopup.TYPE_INFO)
                ' ''Exit Sub
            Else
                txtPrezzoCosto.BackColor = SEGNALA_OK
            End If
        Else
            txtPrezzoCosto.BackColor = SEGNALA_OK
        End If
        'giu260412 giu270412
        If myTipoArticolo <> 9 Then 'giu270219

            If myTipoDoc = "MM" Or myTipoDoc = "CM" Or myTipoDoc = "SM" Or _
               myTipoDoc = "DF" Or myTipoDoc = "DT" Or myTipoDoc = "FA" Or mySCGiacenza = True Then
                If String.IsNullOrEmpty(Session(CSTSEGNOGIACENZA)) Then
                    SetSWPrezzoALCSG()
                End If
                Dim Segno_Giacenza As String = Session(CSTSEGNOGIACENZA)
                If String.IsNullOrEmpty(Segno_Giacenza) Then Segno_Giacenza = ""
                Dim myQE As String = txtQtaEv.Text.Trim
                If Not IsNumeric(myQE.Trim) Then myQE = "0"
                Dim myG As String = lblGiacenza.Text.Trim
                If Not IsNumeric(myG.Trim) Then myG = "0"
                Dim myGI As String = lblGiacImp.Text.Trim
                If Not IsNumeric(myGI.Trim) Then myGI = "0"
                '-
                txtQtaEv.BackColor = SEGNALA_OK
                lblGiacenza.ForeColor = Drawing.Color.Black
                lblMessAgg.ForeColor = Drawing.Color.Blue
                '-
                If Segno_Giacenza = "-" Then
                    If CDbl(myQE) > (CDbl(myG) + CDbl(myGI)) Then
                        txtQtaEv.BackColor = SEGNALA_KO
                        lblGiacenza.ForeColor = SEGNALA_KO
                        lblMessAgg.BorderStyle = BorderStyle.Outset
                        lblMessAgg.ForeColor = SEGNALA_KO
                        lblMessAgg.Text = "Attenzione - !!! Giacenza non disponibile !!! - Attenzione"
                    End If
                End If
            End If

        End If
        'GIU030212 GIU171012
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWSI
    End Sub

#End Region

#Region "Gestione con TESTATA (TD0) E TOTALE (TD2)"

    Public Function TD1ReBuildDett(Optional ByVal SWReBuildDettFromDB As Boolean = False) As Boolean
        'Simile al LOAD
        SetCdmDAdp()
        Session("aSqlAdap") = SqlAdapDocDett
        '--
        If (Session("aDsDett") Is Nothing) Then
            DsDocumentiDett = New DSDocumenti
        Else
            DsDocumentiDett = Session("aDsDett")
        End If
        '--
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Session(SWOP) = SWOPNUOVO Then
            DsDocumentiDett.DocumentiD.Clear()
            DsDocumentiDett.DocumentiD.AcceptChanges()
            Session("aDataView1") = aDataView1
            Session("aSqlAdap") = SqlAdapDocDett
            Session("aDsDett") = DsDocumentiDett
            '------------------------------------------------------------------------------------
        Else
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Function
            End If
            'GIU021111 X FUNZ Pier311011 - giu221111
            If SWReBuildDettFromDB = True Then
                DsDocumentiDett.DocumentiD.Clear()
                SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                SqlAdapDocDett.Fill(DsDocumentiDett.DocumentiD)
            End If
            '---------------------------------------
        End If
        AzzQtaNULL()
        Session("aSqlAdap") = SqlAdapDocDett
        Session("aDsDett") = DsDocumentiDett
        '---
        'giu080319 LblImponibile.Text = HTML_SPAZIO : LblImposta.Text = HTML_SPAZIO : LblTotale.Text = HTML_SPAZIO : LblTotaleLordo.Text = HTML_SPAZIO : LblTotaleLordoPL.Text = HTML_SPAZIO
        AzzeraTxtInsArticoli() 'GIU041211
        '---
        aDataView1 = New DataView(DsDocumentiDett.DocumentiD)
        If aDataView1.Count > 0 Then aDataView1.Sort = "Riga" 'giu011111
        GridViewDett.DataSource = aDataView1
        Session("aDataView1") = aDataView1
        GridViewDett.DataBind()
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
        Else
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
            '--
            AzzeraTxtInsArticoli() 'GIU041211
        End Try
        '-- LOTTI 
        BuildLottiRigaDB(RigaSel)
        '---------
        EnableTOTxtInsArticoli(False)
        Dim strErrore As String = ""
        If AggiornaImporto(DsDocumentiDett, strErrore) = False Then
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        End If
        btnPrimaRigaL.Enabled = False
        btnCaricoLotti.Enabled = False
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
    End Function
    'giu231211 da Private a Pubblic
    Public Function AggiornaImporto(ByVal dsDocDett As DataSet, ByRef strErrore As String) As Boolean
        'OBBLIGATORIO perche mi serve per il calcolo del TotaleLordoMerce Qta*Prezzo
        If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
            _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (DocumentiDett.AggiornaImporto)")
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
        If myID = "" Or Not IsNumeric(myID) Then
            If (dsDocDett.Tables("DocumentiD").Rows.Count > 0) Then
                If Not IsDBNull(dsDocDett.Tables("DocumentiD").Rows(0).Item("IDDocumenti")) Then
                    myID = dsDocDett.Tables("DocumentiD").Rows(0).Item("IDDocumenti").ToString.Trim
                End If
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
                Exit Function
            End If
            Session(IDDOCUMENTI) = myID 'GIU060220
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
        'giu030512
        Dim TotaleLordoMercePL As Decimal = 0
        Dim Deduzioni As Decimal = 0
        If CalcolaTotaleDoc(dsDocDett, Iva, Imponibile, Imposta, _
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

        LblImponibile.Text = FormattaNumero(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4), CInt(DecimaliVal))
        LblImposta.Text = FormattaNumero(Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4), CInt(DecimaliVal))
        LblTotale.Text = FormattaNumero(CDec(LblImponibile.Text) + CDec(LblImposta.Text), CInt(DecimaliVal))
        LblTotaleLordoPL.Text = FormattaNumero(CDec(TotaleLordoMercePL), CInt(DecimaliVal))
        LblTotaleLordo.Text = FormattaNumero(CDec(TotaleLordoMerce), CInt(DecimaliVal))
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
                    Exit Function
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
                    Exit Function
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
                    Exit Function
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
        If _WucElement.CalcolaTotSpeseScad(dsDocDett, Iva, Imponibile, Imposta, _
                              CInt(DecimaliVal), MoltiplicatoreValuta, _
                              Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(IDLis)), _
                              myTipoDoc, CDec(Abbuono), strErrore, TotaleLordoMercePL, Deduzioni) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Function
        End If

    End Function
    'giu140319 ricalcolo tutte le righe di dettaglio in caso cambio lo sconto cassa o tipo pagamento
    Public Function AggImportiTot(ByRef strErrore As String) As Boolean
        Try

            AggImportiTot = False
            strErrore = ""
            'OBBLIGATORIO perche mi serve per il calcolo del TotaleLordoMerce Qta*Prezzo
            If _WucElement.CKCSTTipoDoc(myTipoDoc, myTabCliFor) = False Then
                _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (DocumentiDett.AggImportiTot)")
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
            Dim dsDocDett As New DSDocumenti
            dsDocDett = Session("aDsDett")
            If myID = "" Or Not IsNumeric(myID) Then
                If (dsDocDett.Tables("DocumentiD").Rows.Count > 0) Then
                    If Not IsDBNull(dsDocDett.Tables("DocumentiD").Rows(0).Item("IDDocumenti")) Then
                        myID = dsDocDett.Tables("DocumentiD").Rows(0).Item("IDDocumenti").ToString.Trim
                    End If
                End If
                If myID = "" Or Not IsNumeric(myID) Then
                    _WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO (DocumentiDett.AggImportiTot)")
                    Exit Function
                End If
                Session(IDDOCUMENTI) = myID 'GIU060220
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
            Dim RowDett As DSDocumenti.DocumentiDRow
            For Each RowDett In dsDocDett.DocumentiD
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
                '-
                Select Case Left(myTipoDoc, 1)
                    Case "O"
                        myQuantita = RowDett.Qta_Ordinata
                    Case Else
                        If myTipoDoc = "PR" Or myTipoDoc = "PF" Then
                            myQuantita = RowDett.Qta_Ordinata
                        Else
                            myQuantita = RowDett.Qta_Evasa
                        End If
                End Select
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
                aDataView1 = New DataView(dsDocDett.DocumentiD)
            End If
            If aDataView1.Count > 0 Then aDataView1.Sort = "Riga"
            Session("aDataView1") = aDataView1
            Session("aDsDett") = dsDocDett
            GridViewDett.DataSource = aDataView1
            GridViewDett.EditIndex = -1
            GridViewDett.DataBind()
            EnableTOTxtInsArticoli(False)

            AggImportiTot = AggiornaImporto(dsDocDett, strErrore)
        Catch ex As Exception
            strErrore = "Errore AggImportiTot: " & ex.Message.Trim
        End Try
    End Function

    Public Sub SetEnableTxt(ByVal Valore As Boolean)
        GridViewDett.EditIndex = -1
        GridViewDett.Enabled = Valore
        btnPrimaRiga.Enabled = Valore
        BtnSelArticolo.Enabled = Valore : btnNewRigaUp.Enabled = Valore
        EnableTOTxtInsArticoli(Valore)
        'GIU061211 btnAggArtGridSel.Enabled = False 'giu061211 uso sempre aggriga :: btnInsRigaDett.Enabled = False
        If Valore = True Then
            EnableTOTxtInsArticoli(False)
        End If
        If GridViewDett.Rows.Count = 0 Then
            SetBtnPrimaRigaEnabled(True)
            AzzeraTxtInsArticoli()
            BuildLottiRigaDB(-1)
            Exit Sub
        Else
            SetBtnPrimaRigaEnabled(False)
        End If
        '--- LOTTI
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
            '-
            AzzeraTxtInsArticoli()
        End Try
        '-- LOTTI 
        BuildLottiRigaDB(RigaSel)
        '---------
    End Sub
#End Region

    Private Sub txtCodArtIns_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArtIns.TextChanged
        
        If LeggiArticolo(txtCodArtIns.Text.Trim) = False Then
            'giu260412
            lblGiacenza.Text = "" : lblGiacenza.ForeColor = Drawing.Color.Black
            lblGiacImp.Text = ""
            lblOrdFor.Text = ""
            lblDataArr.Text = ""
            '---------
            txtCodArtIns.BackColor = SEGNALA_KO
            txtDesArtIns.Focus()
        Else
            txtCodArtIns.BackColor = SEGNALA_OK
            'giu310112 DATI GIACENZA/DISPONIBILITA'
            GetDatiGiacenza(txtCodArtIns.Text.Trim)
            'giu150715 DA TESTARE E FATE ATTENZIONE ALL'EVENT TEXTCHANGED SU SE STESSO
            'GIU160715 OK VA BENE COSI
            Dim listaCodiciArticolo As New List(Of String)
            listaCodiciArticolo.Add(txtCodArtIns.Text.Trim)
            Session(ARTICOLI_DA_INS) = listaCodiciArticolo
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
    Private Function LeggiArticolo(ByVal myCodArt As String) As Boolean
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
            txtQtaEv.Text = "0"
            txtQtaInv.Text = "0"
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
                _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (DocumentiDett.AggiornaImporto)")
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
            txtPrezzoIns.Text = FormattaNumero(MyPrezzoAL, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
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
        txtQtaEv.TextChanged, txtSconto1Ins.TextChanged
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
        Else 'giu150320
            ' ''If CInt(txtIVAIns.Text.Trim) > 49 Then
            ' ''    lblMessAgg.BorderStyle = BorderStyle.Outset
            ' ''    lblMessAgg.Text = "IVA errata per il Regime IVA " & RegimeIVA & " - " & Comodo
            ' ''    txtIVAIns.BackColor = Def.SEGNALA_KO
            ' ''End If
        End If
    End Function

    'Fabio18042016
#Region "CaricoLottiNSerie"

    Private Sub btnCaricoLotti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCaricoLotti.Click

        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)

        WFPLottiInsert1.setValoriRiga(txtCodArtIns.Text, txtDesArtIns.Text, txtIVAIns.Text, txtPrezzoIns.Text, txtQtaEv.Text, txtQtaIns.Text, txtSconto1Ins.Text, txtUMIns.Text, LblPrezzoNetto.Text, LblImportoRiga.Text, lblGiacenza.Text, lblGiacImp.Text, lblOrdFor.Text, lblDataArr.Text, lblRigaSel.Text.Trim, lblLabelQtaRe.Text)
        WFPLottiInsert1.WucElement = Me
        WFPLottiInsert1.SetLblMessUtente("Seleziona lotti da caricare")
        Session(F_CARICALOTTI) = True
        WFPLottiInsert1.Show(True)

    End Sub

    Public Sub subCaricoLotti()
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica lotti documento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna riga selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI)

        WFPLottiInsert1.setValoriRiga(txtCodArtIns.Text, txtDesArtIns.Text, txtIVAIns.Text, txtPrezzoIns.Text, txtQtaEv.Text, txtQtaIns.Text, txtSconto1Ins.Text, txtUMIns.Text, LblPrezzoNetto.Text, LblImportoRiga.Text, lblGiacenza.Text, lblGiacImp.Text, lblOrdFor.Text, lblDataArr.Text, lblRigaSel.Text.Trim, lblLabelQtaRe.Text)
        WFPLottiInsert1.WucElement = Me
        WFPLottiInsert1.SetLblMessUtente("Seleziona lotti da caricare")
        Session(F_CARICALOTTI) = True
        WFPLottiInsert1.Show(True)
    End Sub

    Public Sub PostBackLottiInsert()
        BuildLottiRigaDB(lblRigaSel.Text.Trim)
    End Sub

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
        
        LblTotaleLordo.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("TotaleM")), 0, _dvDocT.Item(0).Item("TotaleM"))
        LblTotaleLordo.Text = FormattaNumero(CDec(LblTotaleLordo.Text), Int(DecimaliVal))

        Dim myTot As Decimal = 0
        myTot = IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile1")), 0, _dvDocT.Item(0).Item("Imponibile1"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile2")), 0, _dvDocT.Item(0).Item("Imponibile2"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile3")), 0, _dvDocT.Item(0).Item("Imponibile3"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile4")), 0, _dvDocT.Item(0).Item("Imponibile4"))
        LblImponibile.Text = FormattaNumero(myTot, Int(DecimaliVal))
        myTot = IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta1")), 0, _dvDocT.Item(0).Item("Imposta1"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta2")), 0, _dvDocT.Item(0).Item("Imposta2"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta3")), 0, _dvDocT.Item(0).Item("Imposta3"))
        myTot += IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta4")), 0, _dvDocT.Item(0).Item("Imposta4"))
        LblImposta.Text = FormattaNumero(myTot, Int(DecimaliVal))

        LblTotale.Text = FormattaNumero(IIf(IsDBNull(_dvDocT.Item(0).Item("Totale")), 0, _dvDocT.Item(0).Item("Totale")), Int(DecimaliVal))
    End Function
    Public Function AzzeraLBLTotaliDoc() As Boolean
        LblTotaleLordoPL.Text = HTML_SPAZIO
        LblTotaleLordo.Text = HTML_SPAZIO
        LblImponibile.Text = HTML_SPAZIO
        LblImposta.Text = HTML_SPAZIO
        LblTotale.Text = HTML_SPAZIO
    End Function

    
End Class